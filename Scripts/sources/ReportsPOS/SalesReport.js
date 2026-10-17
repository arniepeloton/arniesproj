define([
    'common',
    //'jquery.plugin.html2canvas',
    //'promise!/Scripts3/Browser/LibraryFowst.js?crc=',
    //GetPathFormat('/Scripts3/Browser/LibraryPeople.js?crc='),
    (IsMobile ? '/Scripts/sources/SalesPayments/SalesPayments_offline.js?crc=' : '')
    ], function () {

        var reqJS;

        var IsPrintingOfflineData = false;
        
        var OfflineSalesTransCount = 0;
        var OfflineSalesItemCount = 0;

        var OfflineTotalSales = 0;
        var OfflineTotalSalesDiscount = 0;
        var OfflineAdjRefund = 0;
        var OfflineAdjSRV = 0;

        var OfflineCash = 0;
        var OffLineCheck = 0;
        var OffLineDebitCard = 0;
        var OffLineCreditCard = 0; 
        var OffLineSRV = 0;
        var OffLineOtherPayment = 0;
        var OffLineOnCredit = 0;
        var OffLineCharges = 0;

        var OffLineVAT = 0;
        var OffLineVATAmount = 0;
        var OffLineNonVAT = 0;
        var OffLineVATExempt = 0;
        var OffLineZeroRatedVAT = 0;
        var OffLineForeGoneVAT = 0;
        var OffLineTotalNumberOfCustomers = 0;
        var OffLineEPlus = 0;
        var OffLineGCash = 0;
        var OffLineSMAC = 0;
        var OffLineVoucher = 0;
        var OffLineVoidedItemQty = 0;
        var OffLineTotalVoidAmount = 0;
        var FirstInvoiceDate;
        var LastInvoiceDate;
        var newdateFrom = new Date();
        var newdateTo = new Date();
        var dateFrom = new Date();
        var dateTo = new Date();
        var dateYear;
        var newRangeType = "Today";
        var IsFowstInSMMall = false;

        var IsAyalaConsolidatedReport = false;

        var IsVAT, LastTotalDataCSR, discountCols;

        if (OfflineMode) {
            reqJS = [
                '/Scripts/sources/ReportsPOS/SalesReport_offline.js?crc=',
                '/Scripts3/SalesMenu_offline.js?crc=',
                '/Scripts/sources/SalesPayments/SalesPaymentsExport_offline.js?crc=',
                '/Scripts3/Offline/Browser/LibraryFowst.js?crc='
            ]
        }

        require(reqJS, function () {

            $(function () {

                var intFAFCTotalLenght = 40;
                var intFBTotalLenght = 27;
                var intFCDoubleTotalLenght = 18;

                var intDesktopPrintLenght = 45;

                var flushedCounter = 0;
                var mInterval;

                var IsViewOnly = 0;

                var countFowst = 0;

                var DepositToID = '';

                setMonthCollection("selPOSSalesReportByDateFromMonth");
                setYearCollection("selPOSSalesReportByDateFromYear");

                setMonthCollection("selPOSSalesReportByDateToMonth");
                setYearCollection("selPOSSalesReportByDateToYear");

                setMonthCollection("selPOSSalesReportRangeByMonthFrom");
                setMonthCollection("selPOSSalesReportRangeByMonthTo");

                setYearCollection("selPOSSalesReportRangeByYear");

                SetWeekRange();

                $(".CashierfromCalendar").calendar({
                    format: "MM-dd-yyyy",
                    multiSelect: false,
                    otherDays: true,
                    getDates: function (data) {
                        $("#txtCashierDateFrom").text(data);
                    }
                });

                $(".CashiertoCalendar").calendar({
                    format: "MM-dd-yyyy",
                    multiSelect: false,
                    otherDays: true,
                    getDates: function (data) {
                        $("#txtCashierDateTo").text(data);
                    }
                });

                if (IsMobile) {
                    if (Web.HandleOrganization() == false)
                        return;
                }

                $(document).ready(function () {

                    if (!window.InAndroid) {
                        $('<applet name="jzebra" code="jzebra.PrintApplet.class" archive="/Scripts/jzebra.jar" width="10" height="10"></applet>').appendTo($('body'));
                    }

                    if (OfflineMode) {
                        Browser.OfflineMode = true;
                        IsVAT = Sql.ExecuteScalar('Select IsVAT from LibraryOrganization WHERE ID = ' + ReadyString(sessionStorage.OrgID));
                    } else {
                        AjaxSend({
                            url: 'ReportsPOS/GetVATType',
                            success: function (vData) {
                                if (vData != undefined) {
                                    IsVAT = CBool(vData.ReportPOS)
                                }
                            }
                        });
                    }

                    sessionStorage.PrintingOfflineXReadData = IsMobile;

                    if (CBool(IsMobile)) {
                        $('#btnPOSSalesReportPrint').text('Print');
                    } else {
                        $('[for=selXReadMachine], [for=selXReadCashier]').parent().parent().removeClass('d-none');
                        $('#btnPOSSalesReportPrint').text('Export');
                    }

                    //if (sessionStorage.FowstID == undefined || sessionStorage.FowstID == "" || sessionStorage.FowstID == null) {
                    //    GetAllFOWST();
                    //    //GetFowst();
                    //}
                    //else if (CBool(sessionStorage.PrintingOfflineXReadData)) {
                    //    $('#dvSalesReportOfflineMessage').css('visibility', 'visible');
                    //    $('#dvSalesReportOfflineMessage').css('display', 'block');
                    //    $('#selPOSSalesReportRange').val(newRangeType);
                    //    //$('#btnPOSSalesReportView').click();
                    //    IsViewOnly = 1;
                    //    //SetAyalaReport();
                    //    DoPrintSalesReport();
                    //}

                    pDeferred.resolve({});

                    GetAllFOWST();
                });

                window.SalesReport = {
                    GetAllFowstinSalesReportPage: function () {
                        $('#btnCashierDateRange').text('Today');
                        newRangeType = "Today";
                        GetAllFOWST();
                    }
                }

                $('body').on('click', '.btnCashierSelectFowst', function () {
                    SearchFowst();
                    //if (countFowst > 1) {
                    //    $('#panCashierFowst').popup('open');
                    //}
                })
                .on('click', '#btnCashierDateRange', function () {
                    PopupPeriod({
                        Display2DaysAgo: false,
                        DisplayYesterday: true,
                        DisplayToday: true,
                        DisplayWeekToDate: false,
                        DisplayMonthToDate: false,
                        DisplayCustomPeriod: true,
                        Click: function (e) {
                            if (e.Period == 'Yesterday') {
                                dateYear = "This year.";
                                newRangeType = "Yesterday";
                                $('#selPOSSalesReportRange').val(newRangeType);
                                var date = $.format.date(new Date(e.From), 'MM d yyyy');
                                dateFrom = date;
                                dateTo = date;
                                IsViewOnly = 1;
                                DoPrintSalesReport();
                                $('#btnCashierDateRange').text('Yesterday');
                            } else if (e.Period == 'Today') {
                                dateYear = "This year.";
                                newRangeType = "Today";
                                $('#selPOSSalesReportRange').val(newRangeType);
                                var date = $.format.date(new Date(e.From), 'MM d yyyy');
                                dateFrom = date;
                                dateTo = date;
                                IsViewOnly = 1;
                                DoPrintSalesReport();
                                $('#btnCashierDateRange').text('Today');
                            } else if (e.Period == 'Custom Date') {
                                dateFrom = $.format.date(new Date(e.From), 'MM-dd-yyyy');
                                dateTo = $.format.date(new Date(e.To), 'MM-dd-yyyy');
                                dateYear = '';
                                newRangeType = "Day";
                                $('#selPOSSalesReportRange').val(newRangeType);
                                $('#btnCashierDateRange').text(dateFrom);

                                if ($('#lblCashierSelectedFowstGroup').text() != "FOWST Location") {
                                    IsViewOnly = 1;
                                    DoPrintSalesReport();
                                }
                            } else if (e.Period == 'Custom') {
                                dateFrom = $.format.date(new Date(e.From), 'MM-dd-yyyy');
                                dateTo = $.format.date(new Date(e.To), 'MM-dd-yyyy');
                                dateYear = '';
                                newRangeType = "Day";
                                $('#selPOSSalesReportRange').val(newRangeType);
                                $('#btnCashierDateRange').text(dateFrom + ' to ' + dateTo);

                                if ($('#lblCashierSelectedFowstGroup').text() != "FOWST Location") {
                                    IsViewOnly = 1;
                                    DoPrintSalesReport();
                                }
                            }
                        }
                    })
                    //$('#panCashierDate').popup('open');
                })
                .on('click', '.btnCashierCancel', function () {
                    $('#panCashierDate').popup('close');
                })
                .on('click', '#btnPOSSalesReportPrint', function (e) {
                    e.preventDefault();
                    IsViewOnly = 0;
                    IsAyalaConsolidatedReport = 0;
                    if (!IsMobile) {
                        //var pParams = {
                        //    OrgID: sessionStorage.OrgID,
                        //    FOWSTID: sessionStorage.FowstID,
                        //    DeviceID: $('#selXReadMachine option:selected').val(),
                        //    UserID: $('#selXReadCashier option:selected').val(),
                        //    RangeType: 'Day',
                        //    RangeFrom:$.format.date(new Date(dateFrom), 'MM-dd-yyyy'),
                        //    RangeTo: $.format.date(new Date(dateTo), 'MM-dd-yyyy'),
                        //    RangeYear: ''
                        //};
                        //var pUrl = MergeUrlParameters(pParams, '/Utilities/DownloadXRead');
                        //window.open(pUrl, '_blank');
                        //return
                        //CheckForUnflushedData();
                        domtoimage.toPng($('#divPrintReportContainer > div')[0])
                            .then(function (dataUrl) {
                                var link = $("<a>")
                                    .attr("href", dataUrl)
                                    .attr("download", 'X-Reading of ' + $('.lblCashierSelectedFowstGroup').text() + ' from ' + $.format.date(new Date(dateFrom), 'MM-dd-yyyy') + ' - ' + $.format.date(new Date(dateTo), 'MM-dd-yyyy') + '.png');

                                $("body").append(link);
                                link[0].click();
                                link.remove();
                            })
                            .catch(function (error) {
                                console.error("dom-to-image error:", error);
                            });
                    } else {
                        DoPrintSalesReport();
                    }
                })
                .on('click', '#btnPOSSalesReportCancel', function (e) {
                    e.preventDefault();
                    window.open(GetPathFormat("/People/PeopleMenu") + (IsMobile ? "?IsMobile=1" : ""), "_self");
                })
                .on('click', '#dvPOSSalesReportPrintAyalaConsolidatedReport', function (e) {
                    IsAyalaConsolidatedReport = true;
                    IsViewOnly = 0;
                    DoPrintSalesReport();
                })

                $('#selPOSSalesReportRange').change(function () {

                    HideRangeType();

                    if ($('#selPOSSalesReportRange').val() == "Day") {
                        $("#divPOSSalesReportRangeByDate").show();
                    } else if ($('#selPOSSalesReportRange').val() == "Week") {
                        $("#divPOSSalesReportRangeByWeek").show();
                    } else if ($('#selPOSSalesReportRange').val() == "Month") {
                        $("#divPOSSalesReportRangeByMonth").show();
                    } else if ($('#selPOSSalesReportRange').val() == "Quarter") {
                        $("#divPOSSalesReportRangeByQuarter").show();
                    } else if ($('#selPOSSalesReportRange').val() == "Year") {
                        $("#divPOSSalesReportRangeByYear").show();
                    }

                });

                $('#selPOSSalesReportByDateFromMonth').change(function (e) {
                    e.preventDefault();

                    setDayCollection('selPOSSalesReportByDateFromMonth', 'selPOSSalesReportByDateFromDay', 'selPOSSalesReportByDateFromYear', true);

                });

                $('#selPOSSalesReportByDateFromYear').change(function (e) {
                    e.preventDefault();

                    setDayCollection('selPOSSalesReportByDateFromMonth', 'selPOSSalesReportByDateFromDay', 'selPOSSalesReportByDateFromYear', true);

                });

                $('#selPOSSalesReportByDateToMonth').change(function (e) {
                    e.preventDefault();

                    setDayCollection('selPOSSalesReportByDateToMonth', 'selPOSSalesReportByDateToDay', 'selPOSSalesReportByDateToYear', true);

                });

                $('#selPOSSalesReportByDateToYear').change(function (e) {
                    e.preventDefault();

                    setDayCollection('selPOSSalesReportByDateToMonth', 'selPOSSalesReportByDateToDay', 'selPOSSalesReportByDateToYear', true);

                });

                $('.btnPOSSalesReportView').click(function (e) {
                    e.preventDefault();
                    IsViewOnly = 1;

                    if (!CBool(sessionStorage.PrintingOfflineXReadData)) {
                        CheckForUnflushedData();
                    } else {
                        DoPrintSalesReport();
                    }
                });

                $('#btnDepositMonies').click(function () {
                    if (IsDate(dateFrom)) {
                        sessionStorage['DepositDateFrom'] = dateFrom;
                    } else {
                        if (newRangeType == 'Today') {
                            sessionStorage['DepositDateFrom'] = (newdateFrom.getMonth() - 1) + "/" + (newdateFrom.getDate()) + "/" + (newdateFrom.getFullYear());
                        } else if (newRangeType == 'Yesterday') {
                            sessionStorage['DepositDateFrom'] = (newdateFrom.getMonth() - 1) + "/" + (newdateFrom.getDate()-1) + "/" + (newdateFrom.getFullYear());
                        }
                    }

                    
                    window.open('/CashHandling/DepositMonies' + (IsMobile ? "?IsMobile=1" : ""), '_self')
                });

                function GetAllFOWST() {


                    if (OfflineMode) {
                        if (Web.HandleOrganization() == false)
                            return;

                        var vData = OfflineBrowser.LibraryFowst.GetData({
                            FilterFowst: 1
                        });

                        ShowFOWST(vData);

                    } else {

                        AjaxSend({
                            url: 'Browser/LibraryFowst',
                            data: { FilterFowst: 1 },
                            success: function (vData) {
                                ShowFOWST(vData);
                            }
                        });

                    }


                }

                function ShowFOWST(vData) {


                    pLV = $('#listCashierFowst');
                    pLV.empty();

                    var vRows;

                    if (OfflineMode)
                        vRows = vData;
                    else
                        vRows = vData.Results;

                    countFowst = vRows.Rows.length;

                    dateFrom = (newdateFrom.getMonth() + 1) + "/" + (newdateFrom.getDate()) + "/" + (newdateFrom.getFullYear());
                    dateTo = (newdateTo.getMonth() + 1) + "/" + (newdateTo.getDate()) + "/" + (newdateFrom.getFullYear());

                    if (vRows.Rows.length == 1) {
                        DisplaySalesReportInfo(vRows.Rows[0]);
                        return

                        if (vRows.Rows[0].IsFOWSTInsideAMall == '1' && vRows.Rows[0].FOWSTMallName == 'SM Mall') {
                            IsFowstInSMMall = true;
                        } else {
                            IsFowstInSMMall = false;
                        }


                        sessionStorage.FowstID = vRows.Rows[0].ID;
                        sessionStorage.SelectedFOWST = vRows.Rows[0].ID;
                        $('.lblCashierSelectedFowstGroup').text(vRows.Rows[0].FowstName);
                        $('#panCashierFowst').popup('close');
                        $('.btnCashierSelectFowst').removeClass('btnRequired').addClass('btnNotRequired');
                        dateYear = "This year.";
                        newRangeType = (newRangeType == "" ? "Today" : newRangeType);
                        $('#selPOSSalesReportRange').val(newRangeType).selectmenu('refresh');
                        IsViewOnly = 1;
                        //SetAyalaReport();
                        DoPrintSalesReport();


                    }
                    else {
                        SearchFowst();
                        return;
                        $('#panCashierFowst').popup('open');

                        for (var i = 0; i <= vRows.Rows.length - 1; i++) {
                            pItem = $('<li>')
                                        .append($('<a>')
                                        .attr('ID', vRows.Rows[i].ID)
                                        .attr('IsFOWSTInsideAMall', vRows.Rows[i].IsFOWSTInsideAMall)
                                         .attr('FOWSTMallName', vRows.Rows[i].FOWSTMallName)
                                            .click(function () {

                                                if ($(this).attr('IsFOWSTInsideAMall') == '1' && $(this).attr('FOWSTMallName') == 'SM Mall') {
                                                    IsFowstInSMMall = true;
                                                } else {
                                                    IsFowstInSMMall = false;
                                                }

                                                sessionStorage.FowstID = $(this).attr('id');
                                                sessionStorage.SelectedFOWST = $(this).attr('id');
                                                $('.lblCashierSelectedFowstGroup').text($(this).text());
                                                $('.lblCashierSelectedFowstGroup2').text($(this).text()); //for Cash Handling UI
                                                $('#panCashierFowst').popup('close');
                                                $('.btnCashierSelectFowst').removeClass('btnRequired').addClass('btnNotRequired');
                                                //$('#btnPOSSalesReportView').click();
                                                dateYear = "This year.";
                                                newRangeType = (newRangeType == "" ? "Today" : newRangeType);
                                                $('#selPOSSalesReportRange').val(newRangeType).selectmenu('refresh');
                                                IsViewOnly = 1;
                                                //SetAyalaReport();
                                                DoPrintSalesReport();
                                            })
                                            .append($('<h3>')
                                                .text(vRows.Rows[i].FowstName)
                                            )
                                        )
                            pLV.append(pItem);
                        }
                    }

                    try {
                        pLV.listview('refresh');
                    }
                    catch (e) {
                        pLV.listview();
                    }

                }

                function SearchFowst() {
                    var settings = [
                            {
                                AutoSearch: true,
                                DisplayText: [{ Column: 'FowstName' }],
                                Click: function (e) {
                                    DisplaySalesReportInfo(e);
                                }
                            }
                    ]

                    if (OfflineMode) {
                        settings[0].Offline = true;
                    }

                    BrowsePopup('LibraryFowst', 'Select a FOWST', settings, {
                        FilterFowst: 1
                    })
                }

                function DisplaySalesReportInfo(data) {
                    if (data.IsFOWSTInsideAMall == '1' && data.FOWSTMallName == 'SM Mall') {
                        IsFowstInSMMall = true;
                    } else {
                        IsFowstInSMMall = false;
                    }


                    sessionStorage.FowstID = data.ID;
                    sessionStorage.SelectedFOWST = data.ID;
                    $('.lblCashierSelectedFowstGroup').text(data.FowstName);
                    $('#panCashierFowst').popup('close');
                    $('.btnCashierSelectFowst').removeClass('btnRequired').addClass('btnNotRequired');
                    dateYear = "This year.";
                    newRangeType = (newRangeType == "" ? "Today" : newRangeType);
                    $('#selPOSSalesReportRange').val(newRangeType).selectmenu('refresh');
                    IsViewOnly = 1;
                    //SetAyalaReport();

                    if (IsMobile) {
                        DoPrintSalesReport();
                    } else {
                        GetStoreMachines(data.ID)
                    }
                }

                function CheckForUnflushedData() {
                    //Flush First before printing the report
                    if (OfflineMode) {
                        if (SalesPaymentsExport_offline().HasUnflushedRecords()) {
                            MessageBox('Oops! This device has some transaction data that has not yet been sent to Mobiz servers. We need to send all transaction data on this device to Mobiz servers to make sure that your sales reports are accurate.', '',
                                [
                                    {
                                        Text: 'Send all unsent transaction data to Mobiz server'
                                    }
                                ],
                                {
                                    OnClose: function () {
                                        AjaxSend({
                                            url: 'Login3/TestCookies',
                                            success: function (vData) {
                                                flushedCounter = 0;
                                                mInterval = setInterval(FlushDeviceData, 1000);
                                            },
                                            error: function (vError) {
                                                DoPrintSalesReport();
                                            }
                                        });


                                    }
                                }
                            );
                        } else {
                            $("#dvpopup").popup("close");
                            DoPrintSalesReport();
                        }
                    } else {
                        DoPrintSalesReport();
                    }
                }

                function DoPrintSalesReport() {
                    if (window.InAndroid) {
                        PrintSalesReport();
                    } else {                        
                        PrintSalesReport();
                        //PrintDesktopSalesReport(sessionStorage.OrgID);
                    }
                }

                function FlushDeviceData() {

                    clearInterval(mInterval);
                    SetPopUpMessage(3, "Flushing Data to Server");

                    if (flushedCounter < 3) {
                        if (SalesPaymentsExport_offline().HasUnflushedRecords()) {
                            flushedCounter++;
                            //flushed dire na side
                            window.SalesPaymentsExport_offline().Export({
                                ////////                                OnUploaded: function () {
                                ////////                                    FollowUp();
                                ////////                                }
                                OnDone: function (vData) {
                                    if (vData.Result == 'ok') {
                                        FlushDeviceData(sessionStorage.OrgID);
                                    }
                                }
                            }); // END window.SalesPaymentsExport_offline().Export   

                            ////////                            function FollowUp() {
                            ////////                                var pResult; // boolean

                            ////////                                pResult = window.SalesPaymentsExport_offline().FollowUp({
                            ////////                                    OnDone: function (vData) {
                            ////////                                        if (vData.Result == 'ok') {
                            ////////                                            FlushDeviceData(sessionStorage.OrgID);
                            ////////                                        }
                            ////////                                    },
                            ////////                                    OnProgress: function (vData) {
                            ////////                                        console.log(vData.Message); // vData.Message = string
                            ////////                                    }
                            ////////                                });
                            ////////                                if (pResult) {
                            ////////                                    //if pResult = false, error ang flushing?
                            ////////                                    // expect OnDone (and optionally OnProgress) to be called
                            ////////                                } else {
                            ////////                                    // OnDone and OnProgress will not be called
                            ////////                                }
                            ////////                            } // END FollowUp()
                        } else {
                            $("#dvpopup").popup("close");
                            DoPrintSalesReport();
                        }
                    }
                    else {
                        MessageBox('Failed flushing data to server. Please try again later.');
                    }
                };

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

                    $("#dvpopup").popup("close");
                    $('#popMessage').html(msg);
                    $("#dvpopup").popup("open");
                }

                function GetFowst() {

                    if (OfflineMode) {
                        var pUser;
                        pUser = SalesMenu_offline().GetUser();
                        if (pUser.FowstID == undefined) {
                            if (pUser.FowstCount == undefined) {
                                MessageBox('You or the device you are currently using is not registered to any FOWST in the current organization.', null, null, {
                                    OnClose: function () {
                                        var pParam = {};
                                        if (IsMobile)
                                            pParam.IsMobile = 1;
                                        pParam = $.param(pParam);
                                        window.location = "/People/PeopleMenu?" + pParam;
                                    } // OnClose
                                }); // MessageBox
                            } else {
                                // if (pUser.FowstCount == undefined) {
                                BrowseAddresses();
                            }; // if (pUser.FowstCount == undefined) {
                        }; // if (pUser.FowstID == undefined) {
                        sessionStorage.FowstID = pUser.FowstID;
                    } else {
                        var pData = { FilterFowst: 1 };
                        AjaxSend({
                            url: 'Browser/LibraryFowst',
                            data: pData,
                            success: function (vFowstData) {

                                if (vFowstData.Results.Rows.length == 1) {
                                    sessionStorage.FowstID = vFowstData.Results.Rows[0].ID;
                                } else {
                                    BrowseAddresses();
                                }
                            }
                        });
                    }
                };

                function BrowseAddresses() {

                    Browser.LibraryFowst.InputData.OrganizationID = sessionStorage.OrgID;
                    Browser.LibraryFowst.InputData.FilterFowst = 1;
                    Browser.LibraryFowst.Click = function (e) {
                        sessionStorage.FowstID = e.ID;
                    };
                    Browser.LibraryFowst.Show();

                };

                function HideRangeType() {
                    $("#divPOSSalesReportRangeByDate").hide();
                    $("#divPOSSalesReportRangeByWeek").hide();
                    $("#divPOSSalesReportRangeByMonth").hide();
                    $("#divPOSSalesReportRangeByQuarter").hide();
                    $("#divPOSSalesReportRangeByYear").hide();
                }

                function SetWeekRange() {
                    $('#selPOSSalesReportRangeByWeekFrom').empty();
                    $('#selPOSSalesReportRangeByWeekTo').empty();

                    $('#selPOSSalesReportRangeByWeekFrom').append(new Option('Please Select', ''));
                    $('#selPOSSalesReportRangeByWeekTo').append(new Option('Please Select', ''));

                    for (var p = 1; p <= 53; p++) {
                        $('#selPOSSalesReportRangeByWeekFrom').append(new Option('Week ' + p, p, false, false));
                        $('#selPOSSalesReportRangeByWeekTo').append(new Option('Week ' + p, p, false, false));
                    }
                }

                function SetAyalaReport() {
                    var pSql = 'select MerchantsLeaseOutNumber from LibraryOrganizationFOWSTMallInfoAyala WHERE FOWSTID = ' + ReadyString(sessionStorage.SelectedFOWST);

                    var mData = CStr2(Sql.ExecuteScalar(pSql))

                    if (mData != '') {
                        $('#dvPOSSalesReportPrintAyalaConsolidatedReport').css('visibility', 'visible')
                        $('#dvPOSSalesReportPrintAyalaConsolidatedReport').css('display', 'block')
                    } else {
                        $('#dvPOSSalesReportPrintAyalaConsolidatedReport').css('visibility', 'hidden')
                        $('#dvPOSSalesReportPrintAyalaConsolidatedReport').css('display', 'none')
                    }
                }
                
                function PrintSalesReport() {
                    var strPrinterIP = '';
                    var strTemp = '';
                    var strPrint = '';
                    var pData = {};
                    var strFrom = '';
                    var strTo = '';
                    var strYear = '';

                    IsPrintingOfflineData = false;

                    //Validate
                    if ($("#selPOSSalesReportRange2").val() == "Day") {
                        if ($.trim($("#selPOSSalesReportByDateFromMonth").val()) == "" ||
                    $.trim($("#selPOSSalesReportByDateFromDay").val()) == "" ||
                    $.trim($("#selPOSSalesReportByDateFromYear").val()) == "") {

                            MessageBox("Please enter start date of report.",
                                "Start Date",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                        );
                            return;
                        }

                        if ($.trim($("#selPOSSalesReportByDateToMonth").val()) == "" ||
                    $.trim($("#selPOSSalesReportByDateToDay").val()) == "" ||
                    $.trim($("#selPOSSalesReportByDateToYear").val()) == "") {

                            MessageBox("Please enter end date of report.",
                                    "End Date",
                                    [
                                        {
                                            Text: 'Ok'
                                        }
                                    ]
                                        );
                            return;
                        }

                        strFrom = $("#selPOSSalesReportByDateFromMonth").val() + "/" + $("#selPOSSalesReportByDateFromDay").val() + "/" + $("#selPOSSalesReportByDateFromYear").val();
                        strTo = $("#selPOSSalesReportByDateToMonth").val() + "/" + $("#selPOSSalesReportByDateToDay").val() + "/" + $("#selPOSSalesReportByDateToYear").val();
                        strYear = '';

                    } else if ($("#selPOSSalesReportRange").val() == "Week") {
                        if ($.trim($("#selPOSSalesReportRangeByWeekFrom").val()) == "") {

                            MessageBox("Please select starting week number of report.",
                            "Start Week",
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                            return;
                        }

                        if ($.trim($("#selPOSSalesReportRangeByWeekTo").val()) == "") {

                            MessageBox("Please select end week number of report.",
                            "End Week",
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                            return;
                        }

                        if ($.trim($("#selPOSSalesReportRangeByWeekYear").val()) == "") {

                            MessageBox("Please select year of the report.",
                            "Year of the Week",
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                            return;
                        }

                        strFrom = $.trim($("#selPOSSalesReportRangeByWeekFrom").val());
                        strTo = $.trim($("#selPOSSalesReportRangeByWeekTo").val());
                        strYear = $.trim($("#selPOSSalesReportRangeByWeekYear").val());

                    } else if ($("#selPOSSalesReportRange").val() == "Month") {
                        if ($.trim($("#selPOSSalesReportRangeByMonthFrom").val()) == "") {

                            MessageBox("Please select starting month of report.",
                            "Start Month",
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                            return;
                        }

                        if ($.trim($("#selPOSSalesReportRangeByMonthTo").val()) == "") {

                            MessageBox("Please select year of the report.",
                            "Month Year",
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                            return;
                        }

                        if ($.trim($("#selPOSSalesReportRangeByMonthYear").val()) == "") {

                            MessageBox("Please select year of the report.",
                            "End Month",
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                            return;
                        }

                        strFrom = $.trim($("#selPOSSalesReportRangeByMonthFrom").val());
                        strTo = $.trim($("#selPOSSalesReportRangeByMonthTo").val());
                        strYear = $.trim($("#selPOSSalesReportRangeByMonthYear").val());

                    } else if ($("#selPOSSalesReportRange").val() == "Quarter") {
                        if ($.trim($("#selPOSSalesReportRangeByQtrFrom").val()) == "") {

                            MessageBox("Please select starting quarter of report.",
                            "Start Quarter",
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                            return;
                        }

                        if ($.trim($("#selPOSSalesReportRangeByQtrTo").val()) == "") {

                            MessageBox("Please select end quarter of report.",
                            "End Quarter",
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                            return;
                        }

                        if ($.trim($("#selPOSSalesReportRangeByQtrYear").val()) == "") {

                            MessageBox("Please select quarter year of the report.",
                            "Quarter Year",
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                            return;
                        }

                        strFrom = $.trim($("#selPOSSalesReportRangeByQtrFrom").val());
                        strTo = $.trim($("#selPOSSalesReportRangeByQtrTo").val());
                        strYear = $.trim($("#selPOSSalesReportRangeByQtrYear").val());

                    } else if ($("#selPOSSalesReportRange").val() == "Year") {
                        if ($.trim($("#selPOSSalesReportRangeByYear").val()) == "") {

                            MessageBox("Please select year of the report.",
                            "Year",
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                            return;
                        }

                        strFrom = '';
                        strTo = '';
                        strYear = $.trim($("#selPOSSalesReportRangeByYear").val());

                    } else if ($("#selPOSSalesReportRange2").val() == "") {
                        MessageBox("Please select report range.",
                            "Report Range",
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                        return;
                    }

                    if (!IsMobile) {
                        if ($('#selXReadMachine').val() == '') {
                            //MessageBox("Please select machine");
                            return;
                        }

                        if ($('#selXReadCashier').val() == '') {
                            //MessageBox("Please select cashier");
                            return;
                        }
                    }

                    pData.RangeType = newRangeType;
                    pData.RangeFrom = $.format.date(new Date(dateFrom), 'MM-dd-yyyy') + (IsMobile ? '' : ' 12:00:00 AM');
                    pData.RangeTo = $.format.date(new Date(dateTo), 'MM-dd-yyyy') + (IsMobile ? '' : ' 11:59:59 PM');
                    pData.RangeYear = newRangeType;

                    sessionStorage['ReportDate'] = dateFrom;
                    getSalesReportData(pData, dateFrom, dateTo, dateYear);
                }

                function GetStoreMachines(vFowstid) {
                    var selXReadMachine = $('#selXReadMachine').empty();

                    selXReadMachine.append(new Option('Please Select', ''));
                    selXReadMachine.unbind().change(function () {
                        GetStoreCashiers();
                        if ($(this).val() != '') PrintSalesReport();
                    })

                    AjaxSend({
                        url: 'Browser/LibraryDeviceFowst',
                        data: { FOWSTID: vFowstid }, success: function (vData) {
                            var vRows = vData.Results.Rows;

                            for (var i in vRows) {
                                selXReadMachine.append(new Option(vRows[i].PhoneName, vRows[i].DeviceID));
                            }

                            GetStoreCashiers(vFowstid);
                        }
                    })
                }

                function GetStoreCashiers(vFowstid) {
                    var selXReadCashier = $('#selXReadCashier').empty();

                    selXReadCashier.append(new Option('Please Select', ''));
                    selXReadCashier.unbind().change(function () {
                        IsViewOnly = 1;
                        if ($(this).val() != '') PrintSalesReport();
                    })

                    if ($('#selXReadMachine').val() == '') return;

                    AjaxSend({
                        url: 'Browser/LibraryFOWSTRealCashier',
                        data: { FOWSTID: vFowstid, DeviceID: $('#selXReadMachine').val() }, success: function (vData) {
                            var vRows = vData.Results.Rows;

                            for (var i in vRows) {
                                selXReadCashier.append(new Option(vRows[i].CashierName, vRows[i].ID));
                            }
                        }
                    })
                }

                window.getSalesReportData = function(pData, strFrom, strTo, strYear) {
                    var vDataPrint;
                    var vDataHeader;
                    var vDataInfo;
                    var vDataTurnOverAndDeposits;
                    var vDataSalesByCashier;
                    var vDataSalesByMachine;
                    var vDataSalesByCashierMachine;
                    var vDataDataSalesReportOtherPaymentInfo;
                    var pParams = GetUrlParameters();

                    pData.FowstID = sessionStorage.FowstID;
                    pData.DeviceID = IsMobile ? Web.DeviceID() : $('#selXReadMachine').val();
                    pData.UserID = IsMobile ? Web.PeopleUserID() : $('#selXReadCashier').val();
                    pData.DateFrom = strFrom + (IsMobile ? '' : ' 12:00:00 AM');
                    pData.DateTo = strTo + (IsMobile ? '' : ' 11:59:59 PM');

                    //Cashier Sales report only if in Offline mode
                    //add MSR
                    if (CBool(sessionStorage.PrintingOfflineXReadData)) {
                        //&& sessionStorage.OfflineType == 'cashier'

                        ////Get Printer IP Address
                        //vDataPrint = SalesReport_offline().getOrgFOWSTPrinterIP();
                        ////Get Header
                        //vDataHeader = SalesReport_offline().getReportHeader(pParams.IsManagerSalesReport);

                        ////if (vDataHeader.Rows.length > 0) {
                        //if (vDataHeader.Rows[0].POSReportType == 0) {
                        //    //Get Details
                        //    vDataInfo = SalesReport_offline().getCashierSalesTransactionSummary(pData);
                        //} else {
                        //    //Get Details
                        //    vDataInfo = SalesReport_offline().getManagerSalesTransactionSummary(pData);
                        //}
                        ////}

                        ////Get Payment INfo By Cashier
                        //vDataSalesByCashier = SalesReport_offline().getManagerSalesReportPaymentInfoByCashier(pData);
                        ////Get Payment INfo By Machine
                        //vDataSalesByMachine = SalesReport_offline().getManagerSalesReportPaymentInfoByMachine(pData);
                        ////Get Payment INfo By Cashier and Machine
                        //vDataSalesByCashierMachine = SalesReport_offline().getManagerSalesReportPaymentInfoByCashierMachine(pData);

                        //PrintSaleReportNow(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, strFrom, strTo, strYear);

                        if (IsMobile) { //IsMobile because it needs the file saved in the device
                            $('#dvSalesReportOfflineMessage').css('visibility', 'hidden');
                            $('#dvSalesReportOfflineMessage').css('display', 'none');
                            //var mDate = new Date();

                            var mDate = strFrom;
                            IsFowstInSMMall = false; // make this false by janine apr 27

                            //PrintSalesReportUsingOfflineData(pData, $.format.date(mDate, 'MM/dd/yy'), $.format.date(mDate, 'MM/dd/yy'), mDate.getFullYear());
                            PrintSalesReportUsingOfflineData(pData, $.format.date(new Date(strFrom), 'MM/dd/yy'), $.format.date(new Date(strTo), 'MM/dd/yy'), (new Date(mDate)).getFullYear());
                            return;
                        }

                    } else {
                        Bs.Loading('show');
                        //Get Printer IP Address
                        AjaxSend({
                            url: "Organization/getOrgFOWSTPrinterIP",
                            success: function (vRetDataPrint) {
                                vDataPrint = vRetDataPrint.OrganizationFOWSTData;
                                Bs.Loading('show');
                                //Get Header
                                AjaxSend({
                                    url: "ReportsPOS/getReportHeader",
                                    data: pData,
                                    success: function (vRetDataHeader) {
                                        vDataHeader = vRetDataHeader.ReportPOS;
                                        Bs.Loading('show');
                                        if (vDataHeader.Rows.length == 0) {
                                            if (sessionStorage.POSSalesReportType == 0) {
                                                var Rows = { POSReportType: 0 };
                                                vDataHeader = Rows;
                                            } else {
                                                var Rows = { POSReportType: 1 };
                                                vDataHeader = Rows;
                                            }
                                        }

                                        if (vDataHeader.Rows[0].POSReportType == 0) {

                                            //Get Payment INfo
                                            AjaxSend({
                                                url: "ReportsPOS/getCashierSalesTransactionSummary",
                                                data: pData,
                                                success: function (vRetDataInfo) {
                                                    vDataInfo = vRetDataInfo.ReportPOS;
                                                    vDataDataSalesReportOtherPaymentInfo = vRetDataInfo.OtherPayments;
                                                    discountCols = vRetDataInfo.Discounts.Rows;
                                                    //Turn Over Report
                                                    AjaxSend({
                                                        url: "ReportsPOS/getCashierTurnOverAndDeposits",
                                                        data: pData,
                                                        success: function (vRetTurnOverAndDeposits) {
                                                            vDataTurnOverAndDeposits = vRetTurnOverAndDeposits.ReportPOS
                                                            Bs.Loading('hide');
                                                            //if (IsMobile) {
                                                            //    pSql = "Select * FROM cache.CacheMSR \n " +
                                                            //        " WHERE SalesDate BETWEEN " + ReadyString($.format.date((new Date(pData.RangeFrom || new Date())), 'yyyy-MM-dd')) + ' AND ' + ReadyString($.format.date((new Date(pData.RangeTo || new Date())), 'yyyy-MM-dd')) + ' \n' +
                                                            //        " AND FOWSTID = " + ReadyString(sessionStorage.FowstID) + ' \n' +
                                                            //        " AND DeviceID = " + ReadyString(Web.DeviceID()) + ' \n' +
                                                            //        " ORDER BY SalesDate DESC \n"

                                                            //    vDataDataSalesReportOtherPaymentInfo = ConsolidatedOtherPayment(Sql.Execute(pSql));
                                                            //}

                                                            DisplayDiscountsBreakdown(discountCols);

                                                            if (CBool(IsViewOnly)) {
                                                                DisplayReport(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                                                                IsViewOnly = 0;
                                                            } else if ($.trim(localStorage.UUID) != '') {
                                                                DisplayReport(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                                                            } else {
                                                                var vDataPrinterType = SalesPayment_offline().getOrgFOWSTPrinterType();

                                                                //PrintSaleReportNow(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, strFrom, strTo, strYear, vDataTurnOverAndDeposits);

                                                                if (vDataPrinterType.Rows.length > 0) {
                                                                    if (vDataPrinterType.Rows[0].PrinterType == 'Epson TM-U220B Printer') {
                                                                        PrintSaleReportNow(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                                                                    } else if (vDataPrinterType.Rows[0].PrinterType == '80MM Mini Thermal Printer') {

                                                                    } else if (vDataPrinterType.Rows[0].PrinterType == 'Epson G Printer') {
                                                                        sessionStorage.Printer = 'GPrinter';
                                                                        PrintSaleReportNow(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                                                                    } else if (vDataPrinterType.Rows[0].PrinterType == 'Sunmi 58MM Printer') {
                                                                        sessionStorage.Printer = 'SunmiPrinter';
                                                                        intFAFCTotalLenght = 33;
                                                                        PrintSaleReportNow(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                                                                    } else if (vDataPrinterType.Rows[0].PrinterType == 'IMin IMin 58MM Printer') {
                                                                        try {
                                                                            window.pPrint.InitializeBluetoothPrinter();
                                                                        } catch (e) {
                                                                            console.log('InitializeBluetoothPrinter: ' + e);
                                                                        };

                                                                        sessionStorage.Printer = 'IMin';

                                                                        PrintSaleReportNowUsingiMin(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                                                                    } else if (vDataPrinterType.Rows[0].PrinterType == 'Windows Print Server') {
                                                                        PrintSaleReportNowViaWindowsPrintServer(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits)
                                                                        //PrintSaleReportNowViaWindowsPrintServer(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg)
                                                                    } else {
                                                                        PrintSaleReportNow(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                                                                    }
                                                                } else {
                                                                    PrintSaleReportNow(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                                                                }
                                                            }
                                                        },
                                                        error: function () {
                                                            Bs.Loading('hide');
                                                        }
                                                    })
                                                },
                                                error: function () {
                                                    Bs.Loading('hide');
                                                }
                                            });

                                        } else {

                                            //Get Payment Info
                                            AjaxSend({
                                                url: "ReportsPOS/getManagerSalesTransactionSummary",
                                                data: pData,
                                                success: function (vRetDataInfo) {
                                                    vDataInfo = vRetDataInfo.ReportPOS;

                                                    AjaxSend({
                                                        url: "ReportsPOS/getManagerSalesReportPaymentInfoByCashier",
                                                        data: pData,
                                                        success: function (vRetDataSalesByCashier) {
                                                            vDataSalesByCashier = vRetDataSalesByCashier.ReportPOS;

                                                            //Get Sales by Machine
                                                            AjaxSend({
                                                                url: "ReportsPOS/getManagerSalesReportPaymentInfoByMachine",
                                                                data: pData,
                                                                success: function (vRetDataSalesByMachine) {
                                                                    vDataSalesByMachine = vRetDataSalesByMachine.ReportPOS;

                                                                    //Get Sales by Cashier and Machine
                                                                    AjaxSend({
                                                                        url: "ReportsPOS/getManagerSalesReportOtherPaymentInfo",
                                                                        data: pData,
                                                                        success: function (vRetDataSalesReportOtherPaymentInfo) {
                                                                            vDataDataSalesReportOtherPaymentInfo = vRetDataSalesReportOtherPaymentInfo.ReportPOS;

                                                                            //Get Sales by Cashier and Machine
                                                                            AjaxSend({
                                                                                url: "ReportsPOS/getManagerSalesReportPaymentInfoByCashierMachine",
                                                                                data: pData,
                                                                                success: function (vRetDataSalesByCashierMachine) {
                                                                                    Bs.Loading('hide');
                                                                                    vDataSalesByCashierMachine = vRetDataSalesByCashierMachine.ReportPOS;

                                                                                    if (IsViewOnly) {
                                                                                        DisplayReport(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear);
                                                                                        IsViewOnly = 0;
                                                                                    } else if ($.trim(localStorage.UUID) != '') {
                                                                                        DisplayReport(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear);
                                                                                    } else {
                                                                                        if (CBool(IsAyalaConsolidatedReport)) {
                                                                                            PrintAyalaConsolidatedReport(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear);
                                                                                        } else {
                                                                                            var vDataPrinterType = SalesPayment_offline().getOrgFOWSTPrinterType();

                                                                                            if (vDataPrinterType.Rows.length > 0) {
                                                                                                if (vDataPrinterType.Rows[0].PrinterType == 'Epson TM-U220B Printer') {
                                                                                                    PrintSaleReportNow(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                                                                                                } else if (vDataPrinterType.Rows[0].PrinterType == '80MM Mini Thermal Printer') {

                                                                                                } else if (vDataPrinterType.Rows[0].PrinterType == 'Epson G Printer') {
                                                                                                    sessionStorage.Printer = 'GPrinter';
                                                                                                    PrintSaleReportNow(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                                                                                                } else if (vDataPrinterType.Rows[0].PrinterType == 'Sunmi 58MM Printer') {
                                                                                                    sessionStorage.Printer = 'SunmiPrinter';
                                                                                                    intFAFCTotalLenght = 33;
                                                                                                    PrintSaleReportNow(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                                                                                                } else if (vDataPrinterType.Rows[0].PrinterType == 'Windows Print Server') {
                                                                                                    PrintSaleReportNowViaWindowsPrintServer(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits)
                                                                                                    //PrintSaleReportNowViaWindowsPrintServer(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg)
                                                                                                } else if (vDataPrinterType.Rows[0].PrinterType == 'IMin IMin 58MM Printer') {
                                                                                                    try {
                                                                                                        window.pPrint.InitializeBluetoothPrinter();
                                                                                                    } catch (e) {
                                                                                                        Android.AddLog('InitializeBluetoothPrinter' + e);
                                                                                                        console.log('InitializeBluetoothPrinter: ' + e);
                                                                                                    };

                                                                                                    sessionStorage.Printer = 'IMin';

                                                                                                    PrintSaleReportNowUsingiMin(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                                                                                                } else {
                                                                                                    PrintSaleReportNow(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                                                                                                }
                                                                                            } else {
                                                                                                PrintSaleReportNow(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                                                                                            }
                                                                                            //PrintSaleReportNow(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, strFrom, strTo, strYear);
                                                                                        }
                                                                                    }
                                                                                },
                                                                                error: function () {
                                                                                    Bs.Loading('hide');
                                                                                }
                                                                            });
                                                                        },
                                                                        error: function () {
                                                                            Bs.Loading('hide');
                                                                        }

                                                                    });

                                                                },
                                                                error: function () {
                                                                    Bs.Loading('hide');
                                                                }

                                                            });
                                                        },
                                                        error: function () {
                                                            Bs.Loading('hide');
                                                        }
                                                    });

                                                },
                                                error: function () {
                                                    Bs.Loading('hide');
                                                }
                                            });

                                        }
                                    },
                                    error: function () {
                                        Bs.Loading('hide');
                                    }
                                });
                            },
                            error: function (vError) {
                                Bs.Loading('hide');
                                //                            MessageBox("This feature requires internet connection.",
                                //                                       "Requires Internet Connection",
                                //                                        [
                                //                                            { Text: 'Ok'
                                //                                            }
                                //                                        ]
                                //                                );
                                //MessageBox("Please note that this offline cashier’s sales " +
                                //            "report may not be complete as some data " +
                                //            "may have already been sent to the Cloud. " +
                                //            "Complete Cashier Sales Reports will  " +
                                //            "become available when internet service resumes. ",
                                //            "Cannot connect to server", [
                                //                {
                                //                    Text: 'Yes',
                                //                    Click: function () {
                                //                        PrintSalesReportUsingOfflineData(pData, strFrom, strTo, strYear);
                                //                        $('.ui-dialog').dialog('close');
                                //                    }
                                //                },
                                //                {
                                //                    Text: 'No'
                                //                }
                                //            ])
                                if (IsMobile) { //IsMobile because it needs the file saved in the device
                                    $('#dvSalesReportOfflineMessage').css('visibility', 'hidden');
                                    $('#dvSalesReportOfflineMessage').css('display', 'none');
                                    //var mDate = new Date();
                                    var mDate = strFrom;
                                    
                                    IsFowstInSMMall = false; // make this false by janine apr 27
                                    PrintSalesReportUsingOfflineData(pData, $.format.date(new Date(mDate), 'MM/dd/yy'), $.format.date(new Date(mDate), 'MM/dd/yy'), (new Date(mDate)).getFullYear());
                                    return;
                                }
                            }

                        });
                    }
                }

                function ConsolidatedOtherPayment(op) {
                    var opbreakdown = [], opcolbreakdown = [];

                    if (op.Rows.length > 0) {
                        var opcols = Object.entries(op.Columns)

                        for (var o in op.Rows) {
                            //i=10 since magsugod ang other payment type sa 
                            for (var i = 28; i <= opcols.length - 1; i++) {
                                var opamt = CDec2(op.Rows[o][opcols[i][0]]);
                                if (opamt > 0) {
                                    //mga OTHER PAYMENT lang pwede idisplay - added by Arnie 5-10-2023
                                    var opname = opcols[i][0];

                                    if (opname.includes('OtherPayment')) {
                                        //opname = opname.replace('OtherPayment', '');
                                        var isOPExist = false, opindex = null;

                                        $.grep(opbreakdown, function (data, index) {
                                            if (CDec2(data[opname]) > 0) {
                                                isOPExist = true;
                                                opindex = index;
                                            }
                                        })

                                        if (isOPExist) {
                                            opbreakdown[opindex][opname] = CDec2(opbreakdown[opindex][opname]) + opamt;
                                        } else {
                                            opcolbreakdown.push(opname);
                                            opbreakdown.push({ [opname]: opamt });
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return {
                        Columns: opcolbreakdown,
                        Rows: opbreakdown
                    };
                }

                function CreateOtherPaymentTypeRow(OtherPaymentType, OtherPaymentTypeAmount) {
                    return $('<tr>')
                        .append($('<td style="text-align:left">')
                            .text(OtherPaymentType)
                        )
                        .append($('<td style="text-align:right">')
                            .text(AccountingFormat(OtherPaymentTypeAmount))
                        )
                }

                function ConsolidateDiscountBreakdown() {
                    var discbreakdown = [];
                    for (var m = 0; m < LastTotalDataCSR.length; m++) {
                        var LastTotal = LastTotalDataCSR[m];
                        for (var i in discountlist) {
                            if (CDec2(LastTotal[discountlist[i].ColumnName]) > 0) {
                                var discamt = CDec2(LastTotal[discountlist[i].ColumnName]);

                                if (discountlist[i].ColumnName == 'TotalSCDiscount') {
                                    discamt = CDec2(discamt) //- CDec2(pDT.Rows[0].RefundedSCDiscount);
                                }

                                var isDiscExist = false, indexDisc = null;
                                $.grep(discbreakdown, function (e, ind) {
                                    if (e.ColumnName == discountlist[i].ColumnName) {
                                        isDiscExist = true;
                                        indexDisc = ind;
                                        return false;
                                    }
                                })

                                if (isDiscExist) {
                                    discbreakdown[indexDisc].DiscountAmount = CDec2(discbreakdown[indexDisc].DiscountAmount) + CDec2(discamt);
                                } else {
                                    discountlist[i].DiscountAmount = discamt;
                                    discbreakdown.push(discountlist[i]);
                                }
                            }
                        }
                    }

                    return discbreakdown;
                }

                function DisplayDiscountsBreakdown(vData, isPrint) {
                    var listDiscountBreakdown = $('#listDiscountBreakdown');

                    if (!CBool(isPrint)) {
                        listDiscountBreakdown.empty();
                    }                    

                    for (var i in vData) {
                        var code = '';

                        if (CStr2(vData[i].DiscountTypeID) == '') {
                            continue;
                        }

                        if (vData[i].DiscountTypeID.toLowerCase() == '35f954a0-c67c-499c-af60-f08e64477fdc') {
                            code = "VIPDISC";
                        } else if (vData[i].DiscountTypeID.toLowerCase() == '8e2dca9c-433f-42fc-a644-04476dba056a') {
                            code = "REGDISC3";
                        } else if (vData[i].DiscountTypeID.toLowerCase() == 'c4199347-6a00-45c9-be11-8934dd63ecd1') {
                            code = "REGDISC2";
                        } else if (vData[i].DiscountTypeID.toLowerCase() == '764edacb-0131-4033-8e44-7b903368008c') {
                            code = "REGDISC";
                        } else if (vData[i].DiscountTypeID.toLowerCase() == '7804a35a-b531-4380-945e-fc9643e99843') {
                            code = "EMPDISC"
                        } else if (vData[i].DiscountTypeID.toLowerCase() == '055a0a33-dd98-47ec-add3-8b2009f3944a') {
                            code = "SENDISC";
                        } else if (vData[i].DiscountTypeID.toLowerCase() == '34a4032f-a142-4c15-a868-4ac4410434df') {
                            code = "PWDDISC";
                        } else if (vData[i].DiscountTypeID.toLowerCase() == '674ad791-188b-4238-b95c-97b5c2b9d544') {
                            code = "SPESENDISC";
                        } else if (vData[i].DiscountTypeID.toLowerCase() == 'c2218c86-0450-4cd3-b019-3edd43e51741') {
                            code = "GPCDISC";
                        } else if (vData[i].DiscountTypeID.toLowerCase() == '21ebf55b-526c-4276-b3b7-0cbca504f425') {
                            code = "NAACDISC";
                        } else if (vData[i].DiscountTypeID.toLowerCase() == 'e95f0b64-1eea-47b4-bde5-b795234b462d') {
                            code = "SOLOPARENTDISC";
                        } else {
                            code = vData[i].DiscountType;
                        }

                        if (!CBool(isPrint)) {
                            listDiscountBreakdown.append($('<li>')
                                .text(code + ' - ' + AccountingFormat(vData[i].DiscountAmount))
                            )
                        } else {
                            if (sessionStorage.Printer == 'IMin') {
                                IminPrintInColumns("    " + code + ": " + AccountingFormat(vData[i].DiscountAmount), "", "", 8, 0, 0, 0, 0, 2, 18, 18, 18);
                            } else {
                                var strPrint = FormatTextToPrint(intFAFCTotalLenght, '** ' + code, 12, ': ' + SetPrefixPostfix(AccountingFormat(vData[i].DiscountAmount), intFAFCTotalLenght - 16, ' ', 0), intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //ReadyTextToPrint('** ' +code + ': ' + AccountingFormat(vData[i].DiscountAmount), 3, 1, 1, 1, 0, 0, 0, 0, 0);
                            }
                        }
                    }
                }

                function PrintSalesReportUsingOfflineData(pData, strFrom, strTo, strYear) {
                    var vDataPrint;
                    var vDataHeader;
                    var vDataInfo;
                    var vDataSalesByCashier;
                    var vDataSalesByMachine;
                    var vDataSalesByCashierMachine;
                    var vDataPrinterType;

                    IsPrintingOfflineData = true;

                    vDataPrinterType = SalesPayment_offline().getOrgFOWSTPrinterType();
                    vRetDataPrint = SalesReport_offline().getOrgFOWSTPrinterIP();
                    vDataPrint = vRetDataPrint;
                    vRetDataHeader = SalesReport_offline().getReportHeader(CInt2(sessionStorage.POSSalesReportType));
                    vDataHeader = vRetDataHeader;

                    vRetDataInfo = SalesReport_offline().getCashierSalesTransactionSummary(pData);
                    vDataInfo = vRetDataInfo

                    vRetDataInfo = SalesReport_offline().getOrgFOWSTPrinterIP();

                    var cacheTable = ['CacheMSR', 'CacheCSR'];
                    var newCacheColumns = ['CurVoid', 'CurVoidItemQty', 'PrintCount', 'LastDateOfReprint'];                    

                    for (var ct in cacheTable) {
                        var cacheColumns = Sql.Execute("PRAGMA cache.table_info(" + cacheTable[ct] +")");

                        for (var i in newCacheColumns) {
                            var isExist = false;
                            for (var m in cacheColumns.Rows) {
                                if (cacheColumns.Rows[m].name == newCacheColumns[i]) {
                                    isExist = true;
                                    break;
                                }
                            }

                            if (!isExist) {
                                pSql = " ALTER TABLE cache." + cacheTable[ct] + " " +
                                    " ADD " + newCacheColumns[i] + " money"
                                Sql.Execute(pSql);
                            }
                        }
                    }                    

                    if (!CBool(sessionStorage.POSSalesReportType)) {
                        //var mDate = new Date(proj.GetZReadDateSql());
                        var mDate = new Date(strFrom);
                        
                        if (!IsDate(mDate)) {
                            mDate = new Date();
                        }

                        if (!IsViewOnly) {
                            pSql = "UPDATE cache.CacheCSR SET PrintCount = IFNULL(PrintCount, 0) + 1, LastDateOfReprint = " + proj.SqliteNow + " \n" +
                                " WHERE SalesDate BETWEEN " + ReadyString($.format.date(new Date(dateFrom), 'yyyy-MM-dd')) + " AND " + ReadyString($.format.date(new Date(dateTo), 'yyyy-MM-dd')) +
                                " AND FOWSTID = " + ReadyString(sessionStorage.FowstID) +
                                " AND UserID = " + ReadyString(Web.PeopleUserID()) +
                                " AND DeviceID = " + ReadyString(Web.DeviceID())

                            Sql.Execute(pSql);
                        }

                        var LastTotalData = GetOfflineCacheReport($.format.date(new Date(strFrom), 'yyyy-MM-dd'), $.format.date(new Date(strTo), 'yyyy-MM-dd'), 'CacheCSR') //Transfers.ReadFile(sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + Web.PeopleUserID() + ($.format.number(mDate.getFullYear(), '00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + '.dat');
                        //Transfers.ReadFile(sessionStorage.OrgID + ($.format.number(mDate.getFullYear(), '00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + '.dat')
                        
                        if (LastTotalData.length > 0) {

                            var LastTotal = LastTotalData[0];

                            OfflineTotalSales = CDec2(LastTotal.TotalSales);
                            OfflineTotalSalesDiscount = CDec2(LastTotal.TotalSalesDiscount);
                            OfflineAdjRefund = CDec2(LastTotal.AdjRefund);
                            OfflineAdjSRV = CDec2(LastTotal.AdjSRV);

                            OfflineCash = CDec2(LastTotal.Cash);
                            OffLineCheck = CDec2(LastTotal.Checks);
                            OffLineDebitCard = CDec2(LastTotal.DebitCard);
                            OffLineCreditCard = CDec2(LastTotal.CreditCard);
                            OffLineSRV = CDec2(LastTotal.SRV);
                            OffLineOtherPayment = CDec2(LastTotal.TotalOtherPayment);
                            OffLineOnCredit = CDec2(LastTotal.OnCredit);
                            OffLineCharges = CDec2(LastTotal.Charges);

                            OffLineVAT = CDec2(CDec2(LastTotal.VAT).toFixed(2));
                            OffLineVATAmount = CDec2(CDec2(LastTotal.VATAmount).toFixed(2));
                            OffLineNonVAT = CDec2(CDec2(LastTotal.NonVAT).toFixed(2));
                            OffLineVATExempt = CDec2(CDec2(LastTotal.VATExempt).toFixed(2));
                            OffLineZeroRatedVAT = CDec2(CDec2(LastTotal.ZeroRatedVAT).toFixed(2));
                            OffLineForeGoneVAT = CDec2(LastTotal.TotalForeGoneVAT);
                            OffLineEPlus = CDec2(LastTotal.OtherPaymentEPlus);
                            OffLineGCash = CDec2(LastTotal.OtherPaymentGCash);
                            OffLineSMAC = CDec2(LastTotal.OtherPaymentSMAC);
                            OffLineVoucher = CDec2(LastTotal.OtherPaymentDiscountVoucher);
                            OffLineVoidedItemQty = CDec2(LastTotal.CurVoidItemQty);
                            OffLineTotalVoidAmount= CDec2(LastTotal.CurVoid);
                            //OffLineTotalNumberOfCustomers = CDec2(LastTotal[21]); for MSR
                            //FirstInvoiceDate = LastTotal[14];
                            //LastInvoiceDate = LastTotal[15];

                            vDataHeader.Rows[0].PrintCount = LastTotal.PrintCount
                        } else {

                            pSql = "Select InvoiceDate from TransInvoice Where TransInvoice.InvoiceNumber > 0 AND date(InvoiceDate) = " + ReadyString(mDate.getFullYear().toString() + '-' + (mDate.getMonth() + 1).toString() + '-' + mDate.getDate().toString()) + " Order By InvoiceDate Asc LIMIT 1"
                            FirstInvoiceDate = Sql.ExecuteScalar(pSql);

                            pSql = "Select InvoiceDate from TransInvoice Where TransInvoice.InvoiceNumber > 0 AND date(InvoiceDate) = " + ReadyString(mDate.getFullYear().toString() + '-' + (mDate.getMonth() + 1).toString() + '-' + mDate.getDate().toString()) + " Order By InvoiceDate Desc LIMIT 1"
                            LastInvoiceDate = Sql.ExecuteScalar(pSql);
                        }

                        vDataTurnOverAndDeposits = SalesReport_offline().getCashierTurnOverAndDeposits($.format.date(mDate, 'yyyy-MM-dd'));                        

                        pSql = "Select * FROM cache.CacheCSR \n " +
                            " WHERE SalesDate BETWEEN " + ReadyString($.format.date((new Date(strFrom)), 'yyyy-MM-dd')) + ' AND ' + ReadyString($.format.date((new Date(strTo)), 'yyyy-MM-dd')) + ' \n' +
                            " AND FOWSTID = " + ReadyString(sessionStorage.FowstID) + ' \n' +
                            " AND DeviceID = " + ReadyString(Web.DeviceID()) + ' \n' +
                            " AND UserID = " + ReadyString(Web.PeopleUserID()) + ' \n' +
                            " ORDER BY SalesDate DESC \n"

                        vDataDataSalesReportOtherPaymentInfo = ConsolidatedOtherPayment(Sql.Execute(pSql));

                        discountCols = [];
                        discountlist = Sql.Execute('SELECT ID, DiscountType FROM private.LibraryDiscountSettings UNION ALL SELECT ID, Name as DiscountType FROM private.LibraryCustomDiscounts WHERE IsActive = 1').Rows;

                        var name = '';
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
                                name = 'Total' + discountlist[i].DiscountType.replace(/ /g, '');
                            }

                            discountCols.push(name);
                            discountlist[i].ColumnName = name;
                            discountlist[i].DiscountTypeID = discountlist[i].ID;
                        }

                        pSql = 'SELECT * FROM cache.CacheCSR WHERE OrgID = ' + ReadyString(sessionStorage.OrgID) + ' \n' +
                            ' AND FowstID = ' + ReadyString(sessionStorage.FowstID) + ' \n' +
                            ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' \n' +
                            ' AND UserID = ' + ReadyString(Web.PeopleUserID()) + ' \n' +
                            ' AND date(SalesDate) BETWEEN ' + ReadyString($.format.date(new Date(strFrom), 'yyyy-MM-dd')) + ' \n ' +
                            ' AND ' + ReadyString($.format.date(new Date(strTo), 'yyyy-MM-dd'))

                        LastTotalDataCSR = Sql.Execute(pSql).Rows

                        if (LastTotalDataCSR.length > 0) {
                            DisplayDiscountsBreakdown(ConsolidateDiscountBreakdown());
                        } else {
                            $('#listDiscountBreakdown').empty();
                        }

                        if (IsViewOnly) {
                            DisplayReport(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                            IsViewOnly = 0;
                        } else {
                            if (vDataPrinterType.Rows.length > 0) {
                                if (vDataPrinterType.Rows[0].PrinterType == 'Epson TM-U220B Printer') {
                                    PrintSaleReportNow(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                                } else if (vDataPrinterType.Rows[0].PrinterType == '80MM Mini Thermal Printer') {

                                } else if (vDataPrinterType.Rows[0].PrinterType == 'Epson G Printer') {
                                    sessionStorage.Printer = 'GPrinter';
                                    PrintSaleReportNow(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                                } else if (vDataPrinterType.Rows[0].PrinterType == 'Sunmi 58MM Printer') {
                                    sessionStorage.Printer = 'SunmiPrinter';
                                    intFAFCTotalLenght = 33;
                                    PrintSaleReportNow(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                                } else if (vDataPrinterType.Rows[0].PrinterType == 'IMin IMin 58MM Printer') {
                                    try {
                                        window.pPrint.InitializeBluetoothPrinter();
                                    } catch (e) {
                                        console.log('InitializeBluetoothPrinter: ' + e);
                                    };

                                    sessionStorage.Printer = 'IMin';

                                    PrintSaleReportNowUsingiMin(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                                } else if (vDataPrinterType.Rows[0].PrinterType == 'Windows Print Server') {
                                    PrintSaleReportNowViaWindowsPrintServer(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits)
                                    //PrintSaleReportNowViaWindowsPrintServer(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg)
                                } else if (vDataPrinterType.Rows[0].PrinterType == 'IMin IMin 58MM Printer') {
                                    try {
                                        window.pPrint.InitializeBluetoothPrinter();
                                    } catch (e) {
                                        Android.AddLog('InitializeBluetoothPrinter' + e);
                                        console.log('InitializeBluetoothPrinter: ' + e);
                                    };

                                    sessionStorage.Printer = 'IMin';

                                    PrintSaleReportNowUsingiMin(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                                } else {
                                    PrintSaleReportNow(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                                }
                            } else {
                                PrintSaleReportNow(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                            }
                            //PrintSaleReportNow(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                        }
                    } else if (CBool(sessionStorage.POSSalesReportType)) {
                        //var mDate = new Date(proj.GetZReadDateSql());
                        var mDate = new Date(strFrom);

                        if (!IsDate(mDate)) {
                            mDate = new Date();
                        }

                        var vDataDataSalesReportOtherPaymentInfo = Transfers.ReadFile('OtherPaymentTypes' + sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + ($.format.number(mDate.getFullYear(), '00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + '.dat');
                        var LastTotalData = GetOfflineCacheReport($.format.date(new Date(strFrom), 'yyyy-MM-dd'), $.format.date(new Date(strTo), 'yyyy-MM-dd'), 'CacheMSR') //Transfers.ReadFile(sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + ($.format.number(mDate.getFullYear(), '00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + '.dat');
                        var LastTransactionItemsCountData = Transfers.ReadFile('LastTransactionItemsCount' + sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + ($.format.number(mDate.getFullYear(), '00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + '.dat');
                        //Transfers.ReadFile(sessionStorage.OrgID + ($.format.number(mDate.getFullYear(), '00').toString() + '-' + $.format.number((mDate.getMonth() + 1), '00').toString() + '-' + mDate.getDate().toString()) + '.dat')

                        if (LastTransactionItemsCountData != undefined) {
                            var LastTotalTransactionCount = LastTransactionItemsCountData.split('|');

                            OfflineSalesTransCount = CInt2(LastTotalTransactionCount[0]);
                            OfflineSalesItemCount = CInt2(LastTotalTransactionCount[1]);
                        } 

                        if (LastTotalData.length > 0) {

                            var LastTotal = LastTotalData[0];

                            OfflineTotalSales = CDec2(LastTotal.TotalSales);
                            OfflineTotalSalesDiscount = CDec2(LastTotal.TotalSalesDiscount);
                            OfflineAdjRefund = CDec2(LastTotal.AdjRefund);
                            OfflineAdjSRV = CDec2(LastTotal.AdjSRV);

                            OfflineCash = CDec2(LastTotal.Cash);
                            OffLineCheck = CDec2(LastTotal.Checks);
                            OffLineDebitCard = CDec2(LastTotal.DebitCard);
                            OffLineCreditCard = CDec2(LastTotal.CreditCard);
                            OffLineSRV = CDec2(LastTotal.SRV);
                            OffLineOtherPayment = CDec2(LastTotal.TotalOtherPayment);
                            OffLineOnCredit = CDec2(LastTotal.OnCredit);
                            OffLineCharges = CDec2(LastTotal.Charges);

                            OffLineVAT = CDec2(LastTotal.VAT);
                            OffLineVATAmount = CDec2(LastTotal.VATAmount);
                            OffLineNonVAT = CDec2(LastTotal.NonVAT);
                            OffLineVATExempt = CDec2(LastTotal.VATExempt);
                            OffLineZeroRatedVAT = CDec2(LastTotal.ZeroRatedVAT);
                            OffLineForeGoneVAT = CDec2(LastTotal.TotalForeGoneVAT);
                            OffLineEPlus = CDec2(LastTotal.OtherPaymentEPlus);
                            OffLineGCash = CDec2(LastTotal.OtherPaymentGCash);
                            OffLineSMAC = CDec2(LastTotal.OtherPaymentSMAC);
                            OffLineVoucher = CDec2(LastTotal.OtherPaymentDiscountVoucher);
                            OffLineVoidedItemQty = CDec2(LastTotal.CurVoidItemQty);
                            OffLineTotalVoidAmount = CDec2(LastTotal.CurVoid);
                            //OffLineTotalNumberOfCustomers = CDec2(LastTotal[21]); for MSR
                            //FirstInvoiceDate = LastTotal[14];
                            //LastInvoiceDate = LastTotal[15];

                        } else {

                            pSql = "Select InvoiceDate from TransInvoice Where TransInvoice.InvoiceNumber > 0 AND date(InvoiceDate) = " + ReadyString(mDate.getFullYear().toString() + '-' + (mDate.getMonth() + 1).toString() + '-' + mDate.getDate().toString()) + " Order By InvoiceDate Asc LIMIT 1"
                            FirstInvoiceDate = Sql.ExecuteScalar(pSql);

                            pSql = "Select InvoiceDate from TransInvoice Where TransInvoice.InvoiceNumber > 0 AND date(InvoiceDate) = " + ReadyString(mDate.getFullYear().toString() + '-' + (mDate.getMonth() + 1).toString() + '-' + mDate.getDate().toString()) + " Order By InvoiceDate Desc LIMIT 1"
                            LastInvoiceDate = Sql.ExecuteScalar(pSql);
                        }

                        vDataTurnOverAndDeposits = SalesReport_offline().getCashierTurnOverAndDeposits($.format.date(mDate, 'yyyy-MM-dd'));
                        vRetDataSalesByCashier = SalesReport_offline().getManagerSalesReportPaymentInfoByCashier(pData);
                        vDataSalesByCashier = vRetDataSalesByCashier.ReportPOS;

                        if (IsViewOnly) {
                            DisplayReport(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                            IsViewOnly = 0;
                        } else {
                            if (vDataPrinterType.Rows.length > 0) {
                                if (vDataPrinterType.Rows[0].PrinterType == 'Epson TM-U220B Printer') {
                                    PrintSaleReportNow(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                                } else if (vDataPrinterType.Rows[0].PrinterType == '80MM Mini Thermal Printer') {

                                } else if (vDataPrinterType.Rows[0].PrinterType == 'Epson G Printer') {
                                    sessionStorage.Printer = 'GPrinter';
                                    PrintSaleReportNow(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                                } else if (vDataPrinterType.Rows[0].PrinterType == 'Sunmi 58MM Printer') {
                                    sessionStorage.Printer = 'SunmiPrinter';
                                    intFAFCTotalLenght = 33;
                                    PrintSaleReportNow(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                                } else if (vDataPrinterType.Rows[0].PrinterType == 'IMin IMin 58MM Printer') {
                                    try {
                                        window.pPrint.InitializeBluetoothPrinter();
                                    } catch (e) {
                                        console.log('InitializeBluetoothPrinter: ' + e);
                                    };

                                    sessionStorage.Printer = 'IMin';

                                    PrintSaleReportNowUsingiMin(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                                } else if (vDataPrinterType.Rows[0].PrinterType == 'Windows Print Server') {
                                    PrintSaleReportNowViaWindowsPrintServer(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits)
                                    //PrintSaleReportNowViaWindowsPrintServer(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg)
                                } else if (vDataPrinterType.Rows[0].PrinterType == 'IMin IMin 58MM Printer') {
                                    try {
                                        window.pPrint.InitializeBluetoothPrinter();
                                    } catch (e) {
                                        Android.AddLog('InitializeBluetoothPrinter' + e);
                                        console.log('InitializeBluetoothPrinter: ' + e);
                                    };

                                    sessionStorage.Printer = 'IMin';

                                    PrintSaleReportNowUsingiMin(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                                } else {                                    
                                    PrintSaleReportNow(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                                }
                            } else {
                                PrintSaleReportNow(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                            }
                            //PrintSaleReportNow(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                        }
                    } else {

                        vRetDataInfo = SalesReport_offline().getManagerSalesTransactionSummary();
                        vDataInfo = vRetDataInfo.ReportPOS;

                        vRetDataSalesByCashier = SalesReport_offline().getManagerSalesReportPaymentInfoByCashier();
                        vDataSalesByCashier = vRetDataSalesByCashier.ReportPOS;

                        vRetDataSalesByMachine = SalesReport_offline().getManagerSalesReportPaymentInfoByMachine();
                        vDataSalesByMachine = vRetDataSalesByMachine.ReportPOS;

                        vRetDataSalesByCashierMachine = SalesReport_offline().getManagerSalesReportPaymentInfoByMachine();
                        vDataSalesByCashierMachine = vRetDataSalesByCashierMachine.ReportPOS;

                        PrintSaleReportNow(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits);
                    }
                }

                function GetOfflineCacheReport(datefrom, dateto, table) {
                    var pSql = "SELECT SUM(TotalSales) as TotalSales, \n" +
                                " SUM(TotalSalesDiscount) as TotalSalesDiscount, \n" +
                                " SUM(AdjRefund) as AdjRefund, \n" +
                                " SUM(AdjSRV) as AdjSRV, \n" +
                                " SUM(Cash) as Cash, \n" +
                                " SUM([Check]) as Checks, \n" +
                                " SUM(DebitCard) as DebitCard, \n" +
                                " SUM(CreditCard) as CreditCard, \n" +
                                " SUM(SRV) as SRV, \n" +
                                " SUM(OnCredit) as OnCredit, \n" +
                                " SUM(Charges) as Charges, \n" +
                                " SUM(VAT) as VAT, \n" +
                                " SUM(VATAmount) as VATAmount, \n" +
                                " SUM(NonVAT) as NonVAT, \n" +
                                " SUM(VATExempt) as VATExempt, \n" +
                                " SUM(ZeroRatedVAT) as ZeroRatedVAT, \n" +
                                " SUM(TotalVATSales) as TotalVATSales, \n" +
                                " SUM(TotalVATAmount) as TotalVATAmount, \n" +
                                " SUM(TotalVATSalesSalesReturn) as TotalVATSalesSalesReturn, \n" +
                                " SUM(TotalVATAmountSalesReturn) as TotalVATAmountSalesReturn, \n" +
                                " SUM(TotalForeGoneVAT) as TotalForeGoneVAT, \n" +
                                " SUM(TotalOtherPayment) as TotalOtherPayment, \n" +
                                " SUM(OtherPaymentAlipay) as OtherPaymentAlipay, \n" +
                                " SUM(OtherPaymentComplimentary) as OtherPaymentComplimentary, \n" +
                                " SUM(OtherPaymentDiscountVoucher) as OtherPaymentDiscountVoucher, \n" +
                                " SUM(OtherPaymentEPlus) as OtherPaymentEPlus, \n" +
                                " SUM(OtherPaymentFoodPanda) as OtherPaymentFoodPanda, \n" +
                                " SUM(OtherPaymentGCash) as OtherPaymentGCash, \n" +
                                " SUM(OtherPaymentGoldCard) as OtherPaymentGoldCard, \n" +
                                " SUM(OtherPaymentGrabFood) as OtherPaymentGrabFood, \n" +
                                " SUM(OtherPaymentLocalDelivery) as OtherPaymentLocalDelivery, \n" +
                                " SUM(OtherPaymentPayMaya) as OtherPaymentPayMaya, \n" +
                                " SUM(OtherPaymentShopeeEVoucher) as OtherPaymentShopeeEVoucher, \n" +
                                " SUM(OtherPaymentShopeePay) as OtherPaymentShopeePay, \n" +
                                " SUM(OtherPaymentSMAC) as OtherPaymentSMAC, \n" +
                                " SUM(OtherPaymentVPLUS) as OtherPaymentVPLUS, \n" +
                                " SUM(CurVoid) as CurVoid, \n" +
                                " SUM(CurVoidItemQty) as CurVoidItemQty \n" +
                                " , PrintCount \n"

                    if (table == 'CacheMSR') {
                        pSql += " , SUM(TotalNoOfCustomers) as TotalNoOfCustomers \n"
                    }

                    pSql += " FROM cache." + table + " \n" +
                            " WHERE OrgID = "+ ReadyString(sessionStorage.OrgID) +" \n" +
                            " AND FowstID = "+ ReadyString(sessionStorage.FowstID) +" \n" +
                            " AND DeviceID = "+ ReadyString(Web.DeviceID()) +" \n" +
                            " AND (DATE(SalesDate) BETWEEN DATE(" + ReadyString(datefrom) + ") AND DATE(" + ReadyString(dateto) + ")) \n"

                    if (table == 'CacheCSR') {
                        pSql += " AND UserID = " + ReadyString(Web.PeopleUserID()) + " \n"
                    }

                    return Sql.Execute(pSql).Rows;
                }

                function PrintSaleReportNow(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits) {

                    if (vDataPrint.Rows.length > 0) {

                        //var vDataPrint = SalesPayments_offline().getOrgFOWSTPrinterIP();

                        strPrinterIP = vDataPrint.Rows[0].PrinterIPAddress;

                        sessionStorage['ReportDate'] = strFrom;

                        //New set of text to print
                        NewPrintBuilder();

                        if (vDataHeader.Rows[0].OrgBusinessTradeName != undefined) {
                            //Get Header
                            strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].OrgBusinessTradeName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                            strPrint = '';
                            strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].SubscriberOrganizationName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            strPrint = strPrint + strTemp;

                            strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].OrgBranchName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            strPrint = strPrint + strTemp;

                            strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].OrgCompleteAddress || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            strPrint = strPrint + strTemp;

                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'TIN ' + $.trim((vDataHeader.Rows[0].OrgTIN || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            strPrint = strPrint + strTemp;

                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Tel. No.' + $.trim((vDataHeader.Rows[0].OrgTelNo || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            strPrint = strPrint + strTemp;

                            strPrint = strPrint + '\n';

                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);
                        }

                        if (vDataHeader.Rows[0].POSReportType == 0) {

                            if (vDataInfo.Rows.length > 0) {
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, (CBool(sessionStorage.PrintingOfflineXReadData) ? 'DRAFT ' : '') + "CASHIER'S", intFAFCTotalLenght, '', 0, '', 0, 0);
                                ReadyTextToPrint(strPrint, 3, 2, 1, 1, 1, 1, 0, 0, 0);

                                if ($("#selPOSSalesReportRange").val() == "Day") {

                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "SALES AND MONIES REPORT (CSMR)", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? 'Date: ' + strFrom : 'From ' + strFrom + ' To ' + strTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Week") {

                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "Same-Store Comparative Sales Report", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? 'Week: ' + strFrom : 'From Week ' + strFrom + ' To Week ' + strTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Month") {

                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "Same-Store Comparative Sales Report", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? 'Month: ' + $.format.date(new Date(strFrom + '/1/2013'), 'MMM') : 'From ' + $.format.date(new Date(strFrom + '/1/2013'), 'MMM') + ' To ' + $.format.date(new Date(strTo + '/1/2013'), 'MMM')), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Quarter") {

                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "Same-Store Comparative Sales Report", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? (strFrom == '1' ? "1st Quarter" : (strFrom == '2' ? "2nd Quarter" : (strFrom == '3' ? "3rd Quarter" : (strFrom == '4' ? "4th Quarter" : "")))) : 'From ' + (strFrom == '1' ? "1st Quarter" : (strFrom == '2' ? "2nd Quarter" : (strFrom == '3' ? "3rd Quarter" : (strFrom == '4' ? "4th Quarter" : "")))) + ' To ' + (strTo == '1' ? "1st Quarter" : (strTo == '2' ? "2nd Quarter" : (strTo == '3' ? "3rd Quarter" : (strTo == '4' ? "4th Quarter" : ""))))), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Year") {
                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "SALES REPORT", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Year: ' + strYear, intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "SALES REPORT (X-READ)", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    if (IsPrintingOfflineData) {
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Date: From: ' + strFrom + ' To: ' + strTo, intFAFCTotalLenght, '', 0, '', 0, 0);
                                        strPrint = strPrint + strTemp;
                                    } else {
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Date : ' + $.format.date(new Date(vDataInfo.Rows[0].SalesDate), 'MM-dd-yyyy'), intFAFCTotalLenght, '', 0, '', 0, 0);
                                        strPrint = strPrint + strTemp;
                                    }

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Date : ' + $.format.date(new Date(vDataInfo.Rows[0].SalesDate), 'MM-dd-yyyy'), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;
                                    

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                    UpdateCSRPrintCount();
                                }

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Cashier', 9, ' : ' + (vDataInfo.Rows[0].CashierName || ''), intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var dblTotalSales = 0;
                                var dblTotalSalesDiscount = 0;
                                var dblTotalVATExemptAmount = 0;
                                var dblAdjRefund = 0;
                                var dblAdjSRV = 0;
                                var dblNetSales = 0;

                                var dblCash = 0;
                                var dblCheck = 0;
                                var dblDebitCard = 0;
                                var dblCreditCard = 0;
                                var dblSRV = 0;
                                var dblOtherPaymentType = 0;
                                var dblOnCredit = 0;
                                var dblCharges = 0;
                                var dblSMAC = 0;
                                var dblGCash = 0;
                                var dblEPlus = 0;
                                var dblVoucher = 0;
                                var dblTotalPayment = 0;

                                var dblVAT = 0;
                                var dblVATAmount = 0;
                                var dblNonVAT = 0;
                                var dblVATExempt = 0;
                                var dblZeroRatedVAT = 0;
                                var dblTotalVAT = 0;
                                var XReadCount = 0;

                                if (!IsPrintingOfflineData) {
                                    for (var p = 0; p <= vDataInfo.Rows.length - 1; p++) {

                                        dblTotalSales = dblTotalSales + parseFloat(vDataInfo.Rows[p].CurGrossSales || 0);
                                        dblTotalSalesDiscount = dblTotalSalesDiscount + parseFloat(vDataInfo.Rows[p].CurSalesDiscount || 0) + parseFloat(vDataInfo.Rows[p].CurSCDiscount || 0);
                                        dblTotalVATExemptAmount = dblTotalVATExemptAmount + parseFloat(vDataInfo.Rows[p].CurVATExempt || 0);
                                        dblAdjRefund = dblAdjRefund + parseFloat(vDataInfo.Rows[p].CurSalesReturnRefund || 0);
                                        dblAdjSRV = dblAdjSRV + parseFloat(vDataInfo.Rows[p].CurSalesReturnExchange || 0);

                                        dblCash = dblCash + parseFloat(vDataInfo.Rows[p].CashAmount || 0);
                                        dblCheck = dblCheck + parseFloat(vDataInfo.Rows[p].CheckPaymentAmount || 0);
                                        dblDebitCard = dblDebitCard + parseFloat(vDataInfo.Rows[p].DebitCardAmount || 0);
                                        dblCreditCard = dblCreditCard + parseFloat(vDataInfo.Rows[p].CreditCardAmount || 0);
                                        dblSRV = dblSRV + parseFloat(vDataInfo.Rows[p].SRVAmount || 0);
                                        dblOtherPaymentType = dblOtherPaymentType + parseFloat(vDataInfo.Rows[p].OtherTypeAmount || 0);
                                        dblCharges = dblCharges + parseFloat(vDataInfo.Rows[p].ChargeAmount || 0);
                                        dblSMAC = dblSMAC + parseFloat(vDataInfo.Rows[p].SMACAmount || 0);
                                        dblGCash = dblGCash + parseFloat(vDataInfo.Rows[p].GCashAmount || 0);
                                        dblEPlus = dblEPlus + parseFloat(vDataInfo.Rows[p].EPlusAmount || 0);
                                        dblVoucher = dblVoucher + parseFloat(vDataInfo.Rows[p].VoucherAmount || 0);

                                        dblVAT = dblVAT + parseFloat(vDataInfo.Rows[p].CurTotalVatablePortionOfSCDTransaction || 0) + parseFloat(vDataInfo.Rows[p].CurTotalVATSales || 0) - parseFloat(vDataInfo.Rows[p].CurTotalVATSalesSalesReturn || 0);
                                        dblVATAmount = dblVATAmount + parseFloat(vDataInfo.Rows[p].CurTotalVatAmountPortionOfSCDTransaction || 0) + parseFloat(vDataInfo.Rows[p].CurTotalVATAmount || 0) - parseFloat(vDataInfo.Rows[p].CurTotalVATAmountSalesReturn || 0);
                                        dblNonVAT = dblNonVAT + parseFloat(vDataInfo.Rows[p].CurNoTAXSales || 0);
                                        dblVATExempt = dblVATExempt + parseFloat(vDataInfo.Rows[p].CurVATExemptSales || 0);
                                        dblZeroRatedVAT = dblZeroRatedVAT + parseFloat(vDataInfo.Rows[p].CurZeroRatedVATSales || 0);

                                        if (vDataInfo.Rows[p].CSRPrintCount != undefined) {
                                            XReadCount = vDataInfo.Rows[p].CSRPrintCount;
                                        }
                                        
                                    };
                                }
                                else {
                                    //if (IsPrintingOfflineData) {
                                    dblTotalSales += OfflineTotalSales;
                                    dblTotalSalesDiscount += OfflineTotalSalesDiscount;
                                    //dblTotalVATExemptAmount += OfflineTotalVATExemptAmount;
                                    dblAdjRefund += OfflineAdjRefund;
                                    dblAdjSRV += OfflineAdjSRV;

                                    dblCash += OfflineCash;
                                    dblCheck += OffLineCheck;
                                    dblDebitCard += OffLineDebitCard;
                                    dblCreditCard += OffLineCreditCard;
                                    dblSRV += OffLineSRV;
                                    dblOtherPaymentType += OffLineOtherPayment;
                                    dblOnCredit += OffLineOnCredit;
                                    dblCharges += OffLineCharges;

                                    dblVAT += OffLineVAT;
                                    dblVATAmount += OffLineVATAmount;
                                    dblNonVAT += OffLineNonVAT;
                                    dblVATExempt += OffLineVATExempt;
                                    dblZeroRatedVAT += OffLineZeroRatedVAT;
                                    dblTotalVATExemptAmount += OffLineForeGoneVAT;
                                }

                                dblCash = dblCash - dblAdjRefund;
                                dblNetSales = dblTotalSales - dblTotalSalesDiscount - dblTotalVATExemptAmount - dblAdjRefund - dblAdjSRV + dblCharges;
                                dblTotalPayment = dblCash + dblCheck + dblDebitCard + dblCreditCard + dblSRV + dblOtherPaymentType;
                                dblTotalVAT = dblVAT + dblVATAmount + dblNonVAT + dblVATExempt + dblZeroRatedVAT;

                                var strTotalSales = SetPrefixPostfix(AccountingFormat(dblTotalSales), intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Gross Sales: ', 14, strTotalSales, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //Sales Discount
                                var strSalesDiscount = SetPrefixPostfix(AccountingFormat(dblTotalSalesDiscount), intFAFCTotalLenght - 23, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Sales Discount', 21, strSalesDiscount, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                //VAT Exempt
                                var strVATExemptAmount = SetPrefixPostfix(AccountingFormat(dblTotalVATExemptAmount), intFAFCTotalLenght - 23, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Vat Exempt Tax', 21, strVATExemptAmount, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                //charges
                                if (dblCharges > 0) {
                                    var strTotalCharges = SetPrefixPostfix(AccountingFormat(dblCharges), intFAFCTotalLenght - 23, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Charges', 21, strTotalCharges, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                }
                                
                                //Sales Adjustment
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Summary of Adjustments', intFAFCTotalLenght, '', 0, '', 0, 1);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                var strAdjRefund = SetPrefixPostfix(AccountingFormat(dblAdjRefund), intFAFCTotalLenght - 29, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Sales Refund for the  Period', 27, strAdjRefund, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strAdjSRV = SetPrefixPostfix(AccountingFormat(dblAdjSRV), intFAFCTotalLenght - 32, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Sales Returns Subject to Merchadise Exchange           (SRV Issued)', 30, strAdjSRV, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strNetSalesAdj = SetPrefixPostfix(AccountingFormat(parseFloat(dblAdjRefund) + parseFloat(dblAdjSRV)), intFAFCTotalLenght - 23, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Net of Adjustments: ', 21, strNetSalesAdj, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                var strNetSales = SetPrefixPostfix(AccountingFormat(dblNetSales), intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Net Sales: ', 12, '  ' + strNetSales, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                var strIssuedSRV = SetPrefixPostfix(AccountingFormat(dblAdjSRV), intFAFCTotalLenght - 20, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Add : Issued SRV:', 18, strIssuedSRV, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                var strTotalSales = SetPrefixPostfix(AccountingFormat(dblNetSales + dblAdjSRV), intFAFCTotalLenght - 21, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Net Sales Adjusted for Exchanges', 33, '', 0, '', 0, 1);
                                //                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '', 0, '', 0, strTotalSales, intFAFCTotalLenght, 1);
                                ReadyTextToPrint(strPrint, 3, 2, 2, 0, 1, 1, 0, 0, 0);

                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' for Exchanges', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //END Sales Adjustment

                                //Sales Grouped By Mode of Payment
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, "Inventory of Payments", intFAFCTotalLenght, '', 0, '', 0, 0);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                var strTotalCash = SetPrefixPostfix(AccountingFormat(dblCash), intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Cash       ', 12, ': ' + strTotalCash, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                var strTotalCheck = SetPrefixPostfix(AccountingFormat(dblCheck), intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Check      ', 12, ': ' + strTotalCheck, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                var strTotalDebitCard = SetPrefixPostfix(AccountingFormat(dblDebitCard), intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Debit Card ', 12, ': ' + strTotalDebitCard, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                var strTotalCreditCard = SetPrefixPostfix(AccountingFormat(dblCreditCard), intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Credit Card', 12, ': ' + strTotalCreditCard, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                var strTotalSRV = SetPrefixPostfix(AccountingFormat(dblSRV), intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SRV        ', 12, ': ' + strTotalSRV, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                if (IsFowstInSMMall) {
                                    var strTotalSMAC = SetPrefixPostfix(AccountingFormat(dblSMAC), intFAFCTotalLenght - 16, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SMAC        ', 12, ': ' + strTotalSMAC, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                    var strTotalEPlus = SetPrefixPostfix(AccountingFormat(dblEPlus), intFAFCTotalLenght - 16, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' E-Plus      ', 12, ': ' + strTotalEPlus, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                    var strTotalVoucher = SetPrefixPostfix(AccountingFormat(dblVoucher), intFAFCTotalLenght - 16, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Voucher     ', 12, ': ' + strTotalVoucher, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);


                                } else {
                                    //May 14 2018 gi comment ang Other Tender kay magconflict sa G-Cash na payment type. pwede ang g-cash inside and outside sa mall

                                    //Feb 8 2020 gibalik ang code para maisa ang pagdisplay sa tanan other types na payment

                                    //var strTotalOther = SetPrefixPostfix(AccountingFormat(dblOtherPaymentType), intFAFCTotalLenght - 16, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Other Tender', 12, ': ' + strTotalOther, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);


                                    if (!CBool(sessionStorage.PrintingOfflineXReadData)) {
                                        if (vDataDataSalesReportOtherPaymentInfo != undefined) {
                                            if (vDataDataSalesReportOtherPaymentInfo.Rows.length > 0) {
                                                var mOtherPaymentTypeColumns = Object.entries(vDataDataSalesReportOtherPaymentInfo.Columns);
                                                //i=10 since magsugod ang other payment type sa 
                                                for (var o in vDataDataSalesReportOtherPaymentInfo.Rows) {
                                                    for (var c in vDataDataSalesReportOtherPaymentInfo.Columns) {
                                                        var opname = vDataDataSalesReportOtherPaymentInfo.Columns[c].replace('OtherPayment', '');
                                                        var amt = CDec2(vDataDataSalesReportOtherPaymentInfo.Rows[o][vDataDataSalesReportOtherPaymentInfo.Columns[c]]);

                                                        if (amt == 0) continue;

                                                        if (opname.toLowerCase() == 'potatocornergc') {
                                                            continue;
                                                        } else {
                                                            var strTotalOther = SetPrefixPostfix(AccountingFormat(amt), intFAFCTotalLenght - 16, ' ', 0);
                                                            strPrint = FormatTextToPrint(intFAFCTotalLenght, ' ' + opname, 12, ': ' + strTotalOther, intFAFCTotalLenght, '', 0, 1);
                                                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    } else {

                                        if (vDataDataSalesReportOtherPaymentInfo != undefined) {
                                            var LastOtherPaymentTypesTotalData = JSON.parse(vDataDataSalesReportOtherPaymentInfo);

                                            for (var otherpaymentcount = 0; otherpaymentcount <= Object.keys(LastOtherPaymentTypesTotalData).length - 1; otherpaymentcount++) {
                                                var strTotalOther = SetPrefixPostfix(AccountingFormat(LastOtherPaymentTypesTotalData[Object.keys(LastOtherPaymentTypesTotalData)[otherpaymentcount]]), intFAFCTotalLenght - 16, ' ', 0);
                                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '    ' + Object.keys(LastOtherPaymentTypesTotalData)[otherpaymentcount], 12, ': ' + strTotalOther, intFAFCTotalLenght, '', 0, 1);
                                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                            }
                                        }
                                    }
                                }
                                //Feb 8 2020 gicomment ni para maisa ang pagdisplay sa tanan other types na payment
                                //if (dblGCash > 0) {
                                //    var strTotalGCash = SetPrefixPostfix(AccountingFormat(dblGCash), intFAFCTotalLenght - 16, ' ', 0);
                                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' GCash       ', 12, ': ' + strTotalGCash, intFAFCTotalLenght, '', 0, 1);
                                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //}
                                

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                var strTotalClassPayment = SetPrefixPostfix(AccountingFormat(dblTotalPayment), intFAFCTotalLenght - 21, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Inventory of Payments', 28, '', 0, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' of Payments', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '', 0, '', 0, strTotalClassPayment, intFAFCTotalLenght, 1);
                                ReadyTextToPrint(strPrint, 3, 2, 2, 0, 1, 1, 0, 0, 0);

                                var dblTotalOverPayment = dblTotalPayment - dblNetSales;
                                //////                                                    var strTotalOverPayment = SetPrefixPostfix(AccountingFormat((dblTotalOverPayment < 0 ? (dblTotalOverPayment * -1) : dblTotalOverPayment)), intFAFCTotalLenght - 25, ' ', 0);
                                var strTotalOverPayment = SetPrefixPostfix(AccountingFormat(dblTotalOverPayment), intFAFCTotalLenght - 25, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Excess of Inventory of', 23, strTotalOverPayment, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Payments vs Net Sales', intFAFCTotalLenght, '', 0, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //END Sales Grouped By Mode of Payment

                                //Inventory of Payments Audit
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, "Inventory of Payments Audit", intFAFCTotalLenght, '', 0, '', 0, 0);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '  Net Sales Adjusted', 19, strTotalSales, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' for Exchanges', intFAFCTotalLenght, '', 0, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);


                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '  Total Inventory', 19, strTotalClassPayment, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' of Payments', intFAFCTotalLenght, '', 0, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);


                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                var dblNetSalesAdjustedLessTotalInvPayment = (dblNetSales + dblAdjSRV) - dblTotalPayment + dblCharges;
                                var strTotalNetSalesAdjustedLessTotalInvPayment = SetPrefixPostfix(AccountingFormat(dblNetSalesAdjustedLessTotalInvPayment), intFAFCTotalLenght - 13, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, (dblNetSalesAdjustedLessTotalInvPayment < 0 ? ' Shortage' : ' Overage '), 9, ': ' + strTotalNetSalesAdjustedLessTotalInvPayment, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //END Inventory of Payments Audit

                                var TotalTurnOverAndDeposits = 0;

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, "Turnovers and Deposits", intFAFCTotalLenght, '', 0, '', 0, 0);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                if (sessionStorage.Printer == 'SunmiPrinter') {
                                    strPrint = ' ';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Action', 7, 'To', 25, 'Amount', intFAFCTotalLenght, 1);
                                    strPrint = strPrint + strTemp;
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                } else {
                                    strPrint = ' ';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Action', 14, 'To', 30, 'Amount', intFAFCTotalLenght, 1);
                                    strPrint = strPrint + strTemp;
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                }
                                
                                strPrint = ' ';

                                for (var toCounter = 0; toCounter <= vDataTurnOverAndDeposits.Rows.length - 1; toCounter++) {
                                                                                                 
                                    if (vDataTurnOverAndDeposits.Rows[toCounter].IsTurnOver!=undefined){
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataTurnOverAndDeposits.Rows[toCounter].IsTurnOver == 1 ? 'Turned Over' : 'Deposited'), 14, vDataTurnOverAndDeposits.Rows[toCounter].DepositedTo, 30, vDataTurnOverAndDeposits.Rows[toCounter].TotalAmount, intFAFCTotalLenght, 1);
                                        strPrint = strPrint + strTemp;

                                        TotalTurnOverAndDeposits += CDec2(vDataTurnOverAndDeposits.Rows[toCounter].TotalAmount);
                                    }                                    
                                }

                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                strPrint = '';
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Total Turnovers and Deposits', 30, ': ' + TotalTurnOverAndDeposits, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                
                                //Sales Grouped by VAT TYPE----------------------------
                                if ((strFrom == strTo && $("#selPOSSalesReportRange").val() == "Day") ||
                                        $("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, "Sales Grouped by VAT Type", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                    var dblTotalVATSalesPercent = (dblVAT / dblTotalVAT) * 100;
                                    var dblTotalVATAmountPercent = (dblVATAmount / dblTotalVAT) * 100;
                                    var dblTotalNonVATPercent = (dblNonVAT / dblTotalVAT) * 100;
                                    var dblTotalVATExemptPercent = (dblVATExempt / dblTotalVAT) * 100;
                                    var dblTotalZeroRatedVATPercent = (dblZeroRatedVAT / dblTotalVAT) * 100;
                                    var dblTotalVATPercent = dblTotalVATSalesPercent + dblTotalVATAmountPercent + dblTotalNonVATPercent + dblTotalVATExemptPercent + dblTotalZeroRatedVATPercent;


                                    var strTotalVATSales = SetPrefixPostfix(AccountingFormat(dblVAT), 14, ' ', 0);
                                    var strTotalVATAmount = SetPrefixPostfix(AccountingFormat(dblVATAmount), 14, ' ', 0);
                                    var strTotalNonVAT = SetPrefixPostfix(AccountingFormat(dblNonVAT), 14, ' ', 0);
                                    var strTotalVATExempt = SetPrefixPostfix(AccountingFormat(dblVATExempt), 14, ' ', 0);
                                    var strTotalZeroRatedVAT = SetPrefixPostfix(AccountingFormat(dblZeroRatedVAT), 14, ' ', 0);
                                    var strTotalAllVAT = SetPrefixPostfix(AccountingFormat(dblTotalVAT), 14, ' ', 0);

                                    var strTotalVATSalesPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATSalesPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalVATAmountPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATAmountPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalNonVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalNonVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalVATExemptPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATExemptPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalZeroRatedVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalZeroRatedVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalAllVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);

                                    if (IsVAT != undefined) {
                                        if (!CBool(IsVAT)) {
                                            strTotalVATSales = SetPrefixPostfix(AccountingFormat(0), 14, ' ', 0);
                                            strTotalVATAmount = SetPrefixPostfix(AccountingFormat(0), 14, ' ', 0);
                                            strTotalNonVAT = SetPrefixPostfix(AccountingFormat(CDec2(dblVAT) + CDec2(dblVATAmount)), 14, ' ', 0);

                                            strTotalVATSalesPercent = SetPrefixPostfix(AccountingFormat(0) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                            strTotalVATAmountPercent = SetPrefixPostfix(AccountingFormat(0) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                            strTotalNonVATPercent = SetPrefixPostfix(AccountingFormat(100) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                        }
                                    }

                                    if (sessionStorage.Printer == 'SunmiPrinter') {

                                        strTotalVATSales = SetPrefixPostfix(AccountingFormat(dblVAT), 7, ' ', 0); //7=14
                                        strTotalVATAmount = SetPrefixPostfix(AccountingFormat(dblVATAmount), 7, ' ', 0);
                                        strTotalNonVAT = SetPrefixPostfix(AccountingFormat(dblNonVAT), 7, ' ', 0);
                                        strTotalVATExempt = SetPrefixPostfix(AccountingFormat(dblVATExempt), 7, ' ', 0);
                                        strTotalZeroRatedVAT = SetPrefixPostfix(AccountingFormat(dblZeroRatedVAT), 7, ' ', 0);
                                        strTotalAllVAT = SetPrefixPostfix(AccountingFormat(dblTotalVAT), 7, ' ', 0);

                                        strPrint = ' ';
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-able Sales', 14, strTotalVATSales, 25, strTotalVATSalesPercent, intFAFCTotalLenght, 1);
                                        strPrint = strPrint + strTemp;

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-Amount', 14, strTotalVATAmount, 25, strTotalVATAmountPercent, intFAFCTotalLenght, 1);
                                        strPrint = strPrint + strTemp;

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Non-VAT', 14, strTotalNonVAT, 25, strTotalNonVATPercent, intFAFCTotalLenght, 1);
                                        strPrint = strPrint + strTemp;

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-Exempt', 14, strTotalVATExempt, 25, strTotalVATExemptPercent, intFAFCTotalLenght, 1);
                                        strPrint = strPrint + strTemp;

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Zero-Rated VAT', 14, strTotalZeroRatedVAT, 25, strTotalZeroRatedVATPercent, intFAFCTotalLenght, 1);
                                        strPrint = strPrint + strTemp;
                                    } else {
                                        strPrint = ' ';
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-able Sales', 14, strTotalVATSales, 30, strTotalVATSalesPercent, intFAFCTotalLenght, 1);
                                        strPrint = strPrint + strTemp;

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-Amount', 14, strTotalVATAmount, 30, strTotalVATAmountPercent, intFAFCTotalLenght, 1);
                                        strPrint = strPrint + strTemp;

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Non-VAT', 14, strTotalNonVAT, 30, strTotalNonVATPercent, intFAFCTotalLenght, 1);
                                        strPrint = strPrint + strTemp;

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-Exempt', 14, strTotalVATExempt, 30, strTotalVATExemptPercent, intFAFCTotalLenght, 1);
                                        strPrint = strPrint + strTemp;

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Zero-Rated VAT', 14, strTotalZeroRatedVAT, 30, strTotalZeroRatedVATPercent, intFAFCTotalLenght, 1);
                                        strPrint = strPrint + strTemp;
                                    }
                                   

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    if (sessionStorage.Printer == 'SunmiPrinter') {
                                        strPrint = FormatTextToPrint(intFAFCTotalLenght, '               ', 14, strTotalAllVAT, 23, strTotalAllVATPercent, intFAFCTotalLenght, 1);
                                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                    } else {
                                        strPrint = FormatTextToPrint(intFAFCTotalLenght, '               ', 14, strTotalAllVAT, 30, strTotalAllVATPercent, intFAFCTotalLenght, 1);
                                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                    }
                                    

                                }
                                //End Sales Grouped by VAT TYPE----------------------------

                                strPrint = '';
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Printed by ', 15, ': ' + (vDataHeader.Rows[0].PrintedBy || ''), intFAFCTotalLenght, '', 0, 1);
                                strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, ' XRead Print Count ', 20, ': ' + XReadCount, intFAFCTotalLenght, '', 0, 1);
                                strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataHeader.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                strPrint = strPrint + strTemp;

                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);

                                PrintText(strPrinterIP);
                                ClearCommandBuffer();

                            } else {
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);
                                PrintText(strPrinterIP);
                                ClearCommandBuffer();
                            }

                        } else {

                            if (vDataInfo.Rows.length > 0) {

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, (CBool(sessionStorage.PrintingOfflineXReadData) ? 'DRAFT ' : '') + "MANAGER'S", intFAFCTotalLenght, '', 0, '', 0, 0);

                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, "MANAGER'S", intFAFCTotalLenght, '', 0, '', 0, 0);
                                ReadyTextToPrint(strPrint, 3, 2, 1, 1, 1, 1, 0, 0, 0);

                                if ($("#selPOSSalesReportRange").val() == "Day") {

                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "SALES REPORT", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? 'Date: ' + strFrom : 'From ' + strFrom + ' To ' + strTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Week") {

                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "Same-Store Comparative Sales Report", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? 'Week: ' + strFrom : 'From Week ' + strFrom + ' To Week ' + strTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Month") {

                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "Same-Store Comparative Sales Report", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? 'Month: ' + $.format.date(new Date(strFrom + '/1/2013'), 'MMM') : 'From ' + $.format.date(new Date(strFrom + '/1/2013'), 'MMM') + ' To ' + $.format.date(new Date(strTo + '/1/2013'), 'MMM')), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Quarter") {

                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "Same-Store Comparative Sales Report", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? (strFrom == '1' ? "1st Quarter" : (strFrom == '2' ? "2nd Quarter" : (strFrom == '3' ? "3rd Quarter" : (strFrom == '4' ? "4th Quarter" : "")))) : 'From ' + (strFrom == '1' ? "1st Quarter" : (strFrom == '2' ? "2nd Quarter" : (strFrom == '3' ? "3rd Quarter" : (strFrom == '4' ? "4th Quarter" : "")))) + ' To ' + (strTo == '1' ? "1st Quarter" : (strTo == '2' ? "2nd Quarter" : (strTo == '3' ? "3rd Quarter" : (strTo == '4' ? "4th Quarter" : ""))))), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Year") {
                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "SALES REPORT", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Year: ' + strYear, intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {
                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "SALES REPORT", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Date : ' + $.format.date(new Date(vDataInfo.Rows[0].SalesDate), 'MM-dd-yyyy'), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                }

                                var dblTotalSales = 0;
                                var dblTotalSalesDiscount = 0;
                                var dblTotalVATExemptAmount = 0;
                                var dblAdjRefund = 0;
                                var dblAdjSRV = 0;
                                var dblNetSales = 0;

                                var dblCash = 0;
                                var dblCheck = 0;
                                var dblDebitCard = 0;
                                var dblCreditCard = 0;
                                var dblSRV = 0;
                                var dblOtherPaymentType = 0;
                                var dblCharges = 0;
                                var dblSMAC = 0;
                                var dblGCash = 0;
                                var dblEPlus = 0;
                                var dblVoucher = 0;
                                var dblTotalPayment = 0;

                                var dblVAT = 0;
                                var dblVATAmount = 0;
                                var dblNonVAT = 0;
                                var dblVATExempt = 0;
                                var dblZeroRatedVAT = 0;
                                var dblTotalVAT = 0;

                                var intTransCount = 0;
                                var intSalesTransCount = 0;
                                var intSalesItemCount = 0;
                                var intSRVTransCount = 0;
                                var intSRVItemCount = 0;
                                var intRefTransCount = 0;
                                var intRefItemCount = 0;

                                if (!IsPrintingOfflineData) {
                                    for (var p = 0; p <= vDataInfo.Rows.length - 1; p++) {

                                        dblTotalSales = dblTotalSales + parseFloat(vDataInfo.Rows[p].CurGrossSales || 0);
                                        dblTotalSalesDiscount = dblTotalSalesDiscount + parseFloat(vDataInfo.Rows[p].CurSalesDiscount || 0) + parseFloat(vDataInfo.Rows[p].CurSCDiscount || 0);
                                        dblTotalVATExemptAmount = dblTotalVATExemptAmount + parseFloat(vDataInfo.Rows[p].CurVATExempt || 0);
                                        dblAdjRefund = dblAdjRefund + parseFloat(vDataInfo.Rows[p].CurSalesReturnRefund || 0);
                                        dblAdjSRV = dblAdjSRV + parseFloat(vDataInfo.Rows[p].CurSalesReturnExchange || 0);

                                        dblCash = dblCash + parseFloat(vDataInfo.Rows[p].CashAmount || 0);
                                        dblCheck = dblCheck + parseFloat(vDataInfo.Rows[p].CheckPaymentAmount || 0);
                                        dblDebitCard = dblDebitCard + parseFloat(vDataInfo.Rows[p].DebitCardAmount || 0);
                                        dblCreditCard = dblCreditCard + parseFloat(vDataInfo.Rows[p].CreditCardAmount || 0);
                                        dblSRV = dblSRV + parseFloat(vDataInfo.Rows[p].SRVAmount || 0);
                                        dblOtherPaymentType = dblOtherPaymentType + parseFloat(vDataInfo.Rows[p].OtherTypeAmount || 0);
                                        dblCharges = dblCharges + parseFloat(vDataInfo.Rows[p].ChargeAmount || 0);
                                        dblSMAC = dblSMAC + parseFloat(vDataInfo.Rows[p].SMACAmount || 0);
                                        dblGCash = dblGCash + parseFloat(vDataInfo.Rows[p].GCashAmount || 0);
                                        dblEPlus = dblEPlus + parseFloat(vDataInfo.Rows[p].EPlusAmount || 0);
                                        dblVoucher = dblVoucher + parseFloat(vDataInfo.Rows[p].VoucherAmount || 0);

                                        dblVAT = dblVAT + parseFloat(vDataInfo.Rows[p].CurTotalVatablePortionOfSCDTransaction || 0) + parseFloat(vDataInfo.Rows[p].CurTotalVATSales || 0) - parseFloat(vDataInfo.Rows[p].CurTotalVATSalesSalesReturn || 0);
                                        dblVATAmount = dblVATAmount + parseFloat(vDataInfo.Rows[p].CurTCurTotalVatAmountPortionOfSCDTransactionotalVATAmount || 0) + parseFloat(vDataInfo.Rows[p].CurTotalVATAmount || 0) - parseFloat(vDataInfo.Rows[p].CurTotalVATAmountSalesReturn || 0);
                                        dblNonVAT = dblNonVAT + parseFloat(vDataInfo.Rows[p].CurNoTAXSales || 0);
                                        dblVATExempt = dblVATExempt + parseFloat(vDataInfo.Rows[p].CurVATExemptSales || 0);
                                        dblZeroRatedVAT = dblZeroRatedVAT + parseFloat(vDataInfo.Rows[p].CurZeroRatedVATSales || 0);

                                        intTransCount = intTransCount + parseFloat(vDataInfo.Rows[p].CurTransCountInv || 0) + parseFloat(vDataInfo.Rows[p].CurTransCountMemoReturnExchange || 0) + parseFloat(vDataInfo.Rows[p].CurTransCountMemoRefund || 0);
                                        intSalesTransCount = intSalesTransCount + parseFloat(vDataInfo.Rows[p].CurTransCountInv || 0);
                                        intSalesItemCount = intSalesItemCount + parseFloat(vDataInfo.Rows[p].CurTransInvItemCount || 0);
                                        intSRVTransCount = intSRVTransCount + parseFloat(vDataInfo.Rows[p].CurTransCountMemoReturnExchange || 0);
                                        intSRVItemCount = intSRVItemCount + parseFloat(vDataInfo.Rows[p].CurSalesReturnExchangeItemCount || 0);
                                        intRefTransCount = intRefTransCount + parseFloat(vDataInfo.Rows[p].CurTransCountMemoRefund || 0);
                                        intRefItemCount = intRefItemCount + parseFloat(vDataInfo.Rows[p].CurSalesRefundItemCount || 0);

                                    };
                                }
                                else {
                                    dblTotalSales += OfflineTotalSales;
                                    dblTotalSalesDiscount += OfflineTotalSalesDiscount;
                                    //dblTotalVATExemptAmount += OfflineTotalVATExemptAmount;
                                    dblAdjRefund += OfflineAdjRefund;
                                    dblAdjSRV += OfflineAdjSRV;

                                    dblCash += OfflineCash;
                                    dblCheck += OffLineCheck;
                                    dblDebitCard += OffLineDebitCard;
                                    dblCreditCard += OffLineCreditCard;
                                    dblSRV += OffLineSRV;
                                    dblOtherPaymentType += OffLineOtherPayment;
                                    dblOnCredit += OffLineOnCredit;
                                    dblCharges += OffLineCharges;

                                    dblVAT += OffLineVAT;
                                    dblVATAmount += OffLineVATAmount;
                                    dblNonVAT += OffLineNonVAT;
                                    dblVATExempt += OffLineVATExempt;
                                    dblZeroRatedVAT += OffLineZeroRatedVAT;
                                    dblTotalVATExemptAmount += OffLineForeGoneVAT;

                                    intTransCount = OfflineSalesTransCount;
                                    intSalesItemCount = OfflineSalesItemCount;
                                }

                                dblCash = dblCash - dblAdjRefund;
                                dblNetSales = dblTotalSales - dblTotalSalesDiscount - dblTotalVATExemptAmount - dblAdjRefund - dblAdjSRV + dblCharges;
                                dblTotalPayment = dblCash + dblCheck + dblDebitCard + dblCreditCard + dblSRV + dblOtherPaymentType;
                                dblTotalVAT = dblVAT + dblVATAmount + dblNonVAT + dblVATExempt + dblZeroRatedVAT;

                                var strTotalSales = SetPrefixPostfix(AccountingFormat(dblTotalSales), intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Gross Sales: ', 14, strTotalSales, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //Sales Discount
                                var strSalesDiscount = SetPrefixPostfix(AccountingFormat(dblTotalSalesDiscount), intFAFCTotalLenght - 23, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Sales Discount', 21, strSalesDiscount, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                //VAT Exempt
                                var strVATExemptAmount = SetPrefixPostfix(AccountingFormat(dblTotalVATExemptAmount), intFAFCTotalLenght - 23, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total VAT Exempt Tax', 21, strVATExemptAmount, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                //charges
                                if (dblCharges > 0) {
                                    var strTotalCharges = SetPrefixPostfix(AccountingFormat(dblCharges), intFAFCTotalLenght - 23, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Charges', 21, strTotalCharges, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                }


                                //Sales Adjustment
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Summary of Adjustments', intFAFCTotalLenght, '', 0, '', 0, 1);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                var strAdjRefund = SetPrefixPostfix(AccountingFormat(dblAdjRefund), intFAFCTotalLenght - 29, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Sales Refund for the  Period', 27, strAdjRefund, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strAdjSRV = SetPrefixPostfix(AccountingFormat(dblAdjSRV), intFAFCTotalLenght - 32, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Sales Returns Subject to Merchadise Exchange           (SRV Issued)', 30, strAdjSRV, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strNetSalesAdj = SetPrefixPostfix(AccountingFormat(parseFloat(dblAdjRefund) + parseFloat(dblAdjSRV)), intFAFCTotalLenght - 23, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Net of Adjustments: ', 21, strNetSalesAdj, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                var strNetSales = SetPrefixPostfix(AccountingFormat(dblNetSales), intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Net Sales: ', 12, '  ' + strNetSales, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                var strIssuedSRV = SetPrefixPostfix(AccountingFormat(dblAdjSRV), intFAFCTotalLenght - 22, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Add : Issued SRV:', 18, strIssuedSRV, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                var strTotalSales = SetPrefixPostfix(AccountingFormat(dblNetSales + dblAdjSRV), intFAFCTotalLenght - 21, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Net Sales Adjusted for Exchanges', 34, '', intFAFCTotalLenght, '', 0, 1);
                                //                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' for Exchanges', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '', 0, '', 0, strTotalSales, intFAFCTotalLenght, 1);
                                ReadyTextToPrint(strPrint, 3, 2, 2, 0, 1, 1, 0, 0, 0);


                                //END Sales Adjustment

                                //Sales Grouped By Mode of Payment
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, "Inventory of Payments", intFAFCTotalLenght, '', 0, '', 0, 0);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                var strTotalCash = SetPrefixPostfix(AccountingFormat(dblCash), intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Cash       ', 12, ': ' + strTotalCash, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                var strTotalCheck = SetPrefixPostfix(AccountingFormat(dblCheck), intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Check      ', 12, ': ' + strTotalCheck, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                var strTotalDebitCard = SetPrefixPostfix(AccountingFormat(dblDebitCard), intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Debit Card', 12, ': ' + strTotalDebitCard, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                var strTotalCreditCard = SetPrefixPostfix(AccountingFormat(dblCreditCard), intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Credit Card', 12, ': ' + strTotalCreditCard, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                var strTotalSRV = SetPrefixPostfix(AccountingFormat(dblSRV), intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SRV        ', 12, ': ' + strTotalSRV, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                if (IsFowstInSMMall) {
                                    var strTotalSMAC = SetPrefixPostfix(AccountingFormat(dblSMAC), intFAFCTotalLenght - 16, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SMAC        ', 12, ': ' + strTotalSMAC, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                    var strTotalEPlus = SetPrefixPostfix(AccountingFormat(dblEPlus), intFAFCTotalLenght - 16, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' E-Plus      ', 12, ': ' + strTotalEPlus, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                    var strTotalVoucher = SetPrefixPostfix(AccountingFormat(dblVoucher), intFAFCTotalLenght - 16, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Voucher     ', 12, ': ' + strTotalVoucher, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                } else {
                                    //May 14 2018 gi comment ang Other Tender kay magconflict sa G-Cash na payment type. pwede ang g-cash inside and outside sa mall

                                    //Feb 8 2020 gibalik ang code para maisa ang pagdisplay sa tanan other types na payment

                                    //var strTotalOther = SetPrefixPostfix(AccountingFormat(dblOtherPaymentType), intFAFCTotalLenght - 16, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Other Tender', 12, ': ' + strTotalOther, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                    if (!CBool(sessionStorage.PrintingOfflineXReadData)) {
                                        if (vDataDataSalesReportOtherPaymentInfo != undefined) {
                                            if (vDataDataSalesReportOtherPaymentInfo.Rows.length > 0) {
                                                for (var otherpaymentcount = 0; otherpaymentcount <= vDataDataSalesReportOtherPaymentInfo.Rows.length - 1; otherpaymentcount++) {

                                                    var strTotalOther = SetPrefixPostfix(AccountingFormat(vDataDataSalesReportOtherPaymentInfo.Rows[otherpaymentcount].TotalAmount), intFAFCTotalLenght - 16, ' ', 0);
                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' ' + vDataDataSalesReportOtherPaymentInfo.Rows[otherpaymentcount].PaymentType, 12, ': ' + strTotalOther, intFAFCTotalLenght, '', 0, 1);
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                                }
                                            }
                                        }
                                    }else {
                                        if (vDataDataSalesReportOtherPaymentInfo != undefined) {
                                            var LastOtherPaymentTypesTotalData = JSON.parse(vDataDataSalesReportOtherPaymentInfo);

                                            for (var otherpaymentcount = 0; otherpaymentcount <= Object.keys(LastOtherPaymentTypesTotalData).length - 1; otherpaymentcount++) {
                                                var strTotalOther = SetPrefixPostfix(AccountingFormat(LastOtherPaymentTypesTotalData[Object.keys(LastOtherPaymentTypesTotalData)[otherpaymentcount]]), intFAFCTotalLenght - 16, ' ', 0);
                                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '    ' + Object.keys(LastOtherPaymentTypesTotalData)[otherpaymentcount], 12, ': ' + strTotalOther, intFAFCTotalLenght, '', 0, 1);
                                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                            }
                                        }
                                    }
                                }

                                //Feb 8 2020 gicomment ni para maisa ang pagdisplay sa tanan other types na payment
                                //if (dblGCash > 0) {
                                //    var strTotalGCash = SetPrefixPostfix(AccountingFormat(dblGCash), intFAFCTotalLenght - 16, ' ', 0);
                                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' GCash       ', 12, ': ' + strTotalGCash, intFAFCTotalLenght, '', 0, 1);
                                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //}

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                var strTotalClassPayment = SetPrefixPostfix(AccountingFormat(dblTotalPayment), intFAFCTotalLenght - 20, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Inventory of Payments', 28, '', intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' of Payments', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '', 0, '', 0, strTotalClassPayment, intFAFCTotalLenght, 1);
                                ReadyTextToPrint(strPrint, 3, 2, 2, 0, 1, 1, 0, 0, 0);

                                var dblTotalOverPayment = dblTotalPayment - dblNetSales;
                                //////                                                    var strTotalOverPayment = SetPrefixPostfix(AccountingFormat((dblTotalOverPayment < 0 ? (dblTotalOverPayment * -1) : dblTotalOverPayment)), intFAFCTotalLenght - 25, ' ', 0);
                                var strTotalOverPayment = SetPrefixPostfix(AccountingFormat(dblTotalOverPayment), intFAFCTotalLenght - 25, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Excess of Inventory of', 23, strTotalOverPayment, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Payments vs Net Sales', intFAFCTotalLenght, '', 0, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //END Sales Grouped By Mode of Payment

                                //Inventory of Payments Audit
                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, "Inventory of Payments Audit", intFAFCTotalLenght, '', 0, '', 0, 0);
                                //                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Net Sales Adjusted', 19, strTotalSales, intFAFCTotalLenght, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' for Exchanges', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Inventory', 16, strTotalClassPayment, intFAFCTotalLenght, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' of Payments', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                var dblNetSalesAdjustedLessTotalInvPayment = (dblNetSales + dblAdjSRV) - dblTotalPayment + dblCharges;
                                var strTotalNetSalesAdjustedLessTotalInvPayment = SetPrefixPostfix(AccountingFormat(dblNetSalesAdjustedLessTotalInvPayment), intFAFCTotalLenght - 13, ' ', 0);
                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, (dblNetSalesAdjustedLessTotalInvPayment < 0 ? ' Shortage' : ' Overage '), 9, ': ' + strTotalNetSalesAdjustedLessTotalInvPayment, intFAFCTotalLenght, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //END Inventory of Payments Audit

                                //Sales Grouped by VAT TYPE----------------------------
                                if ((strFrom == strTo && $("#selPOSSalesReportRange").val() == "Day") ||
                                $("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "Sales Grouped by VAT Type", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                    var dblTotalVATSalesPercent = (dblVAT / dblTotalVAT) * 100;
                                    var dblTotalVATAmountPercent = (dblVATAmount / dblTotalVAT) * 100;
                                    var dblTotalNonVATPercent = (dblNonVAT / dblTotalVAT) * 100;
                                    var dblTotalVATExemptPercent = (dblVATExempt / dblTotalVAT) * 100;
                                    var dblTotalZeroRatedVATPercent = (dblZeroRatedVAT / dblTotalVAT) * 100;
                                    var dblTotalVATPercent = dblTotalVATSalesPercent + dblTotalVATAmountPercent + dblTotalNonVATPercent + dblTotalVATExemptPercent + dblTotalZeroRatedVATPercent;

                                    var strTotalVATSales = SetPrefixPostfix(AccountingFormat(dblVAT), 14, ' ', 0);
                                    var strTotalVATAmount = SetPrefixPostfix(AccountingFormat(dblVATAmount), 14, ' ', 0);
                                    var strTotalNonVAT = SetPrefixPostfix(AccountingFormat(dblNonVAT), 14, ' ', 0);
                                    var strTotalVATExempt = SetPrefixPostfix(AccountingFormat(dblVATExempt), 14, ' ', 0);
                                    var strTotalZeroRatedVAT = SetPrefixPostfix(AccountingFormat(dblZeroRatedVAT), 14, ' ', 0);
                                    var strTotalAllVAT = SetPrefixPostfix(AccountingFormat(dblTotalVAT), 14, ' ', 0);

                                    var strTotalVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATSalesPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalVATAmountPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATAmountPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalNonVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalNonVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalVATExemptPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATExemptPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalZeroRatedVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalZeroRatedVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalAllVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);

                                    if (IsVAT != undefined) {
                                        if (!CBool(IsVAT)) {
                                            strTotalVATSales = SetPrefixPostfix(AccountingFormat(0), 14, ' ', 0);
                                            strTotalVATAmount = SetPrefixPostfix(AccountingFormat(0), 14, ' ', 0);
                                            strTotalNonVAT = SetPrefixPostfix(AccountingFormat(CDec2(dblVAT) + CDec2(dblVATAmount)), 14, ' ', 0);

                                            strTotalVATPercent = SetPrefixPostfix(AccountingFormat(0) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                            strTotalVATAmountPercent = SetPrefixPostfix(AccountingFormat(0) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                            strTotalNonVATPercent = SetPrefixPostfix(AccountingFormat(100) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                        }
                                    }

                                    strPrint = ' ';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-able Sales', 14, strTotalVATSales, 30, strTotalVATPercent, intFAFCTotalLenght, 1);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT Amount', 14, strTotalVATAmount, 30, strTotalVATAmountPercent, intFAFCTotalLenght, 1);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Non-VAT', 14, strTotalNonVAT, 30, strTotalNonVATPercent, intFAFCTotalLenght, 1);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-Exempt', 14, strTotalVATExempt, 30, strTotalVATExemptPercent, intFAFCTotalLenght, 1);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Zero-Rated VAT', 14, strTotalZeroRatedVAT, 30, strTotalZeroRatedVATPercent, intFAFCTotalLenght, 1);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '              ', 14, strTotalAllVAT, 30, strTotalAllVATPercent, intFAFCTotalLenght, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                }
                                //End Sales Grouped by VAT TYPE----------------------------

                                //Get Sales by Cashier
                                if ((strFrom == strTo && $("#selPOSSalesReportRange").val() == "Day") ||
                                                $("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                    if (!IsPrintingOfflineData) {
                                        if (vDataSalesByCashier.Rows.length > 0) {

                                            strPrint = '';
                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, "Sales Grouped by Cashier", intFAFCTotalLenght, '', 0, '', 0, 0);
                                            strPrint = strPrint + strTemp;

                                            ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                            ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                            //Get Total Sales
                                            var dblAllTotalSales = 0;
                                            for (var p = 0; p <= vDataSalesByCashier.Rows.length - 1; p++) {
                                                dblAllTotalSales = dblAllTotalSales + parseFloat(vDataSalesByCashier.Rows[p].TotalSales);
                                            };

                                            var dblAllTotalSalesPercent = 0;

                                            strPrint = '';

                                            for (var p = 0; p <= vDataSalesByCashier.Rows.length - 1; p++) {

                                                var strTotalSales = SetPrefixPostfix(AccountingFormat((vDataSalesByCashier.Rows[p].TotalSales || 0)), 14, ' ', 0);
                                                var dblTotalSalesPercent = (parseFloat(vDataSalesByCashier.Rows[p].TotalSales || 0) / dblAllTotalSales) * 100;
                                                var strTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                                dblAllTotalSalesPercent = dblAllTotalSalesPercent + dblTotalSalesPercent;

                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, vDataSalesByCashier.Rows[p].CashierName, 14, strTotalSales, 30, strTotalSalesPercent, intFAFCTotalLenght, 0);
                                                strPrint = strPrint + strTemp;

                                            };

                                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                            var strAllTotalSales = SetPrefixPostfix(AccountingFormat(dblAllTotalSales), 14, ' ', 0);
                                            var strAllTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblAllTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                            strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                            strPrint = FormatTextToPrint(intFAFCTotalLenght, '              ', 15, strAllTotalSales, 31, strAllTotalSalesPercent, intFAFCTotalLenght, 1);
                                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                        }
                                    }
                                }

                                //Get Sales by Machine
                                if ((strFrom == strTo && $("#selPOSSalesReportRange").val() == "Day") ||
                                            $("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                    if (!IsPrintingOfflineData) {
                                        if (vDataSalesByMachine.Rows.length > 0) {

                                            strPrint = '';
                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, "Sales Grouped by Machine (MIN)", intFAFCTotalLenght, '', 0, '', 0, 0);
                                            strPrint = strPrint + strTemp;

                                            ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                            ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                            //Get Total Sales
                                            dblAllTotalSales = 0;
                                            for (var p = 0; p <= vDataSalesByMachine.Rows.length - 1; p++) {
                                                dblAllTotalSales = dblAllTotalSales + parseFloat(vDataSalesByMachine.Rows[p].TotalSales);
                                            };

                                            dblAllTotalSalesPercent = 0;

                                            strPrint = '';
                                            for (var p = 0; p <= vDataSalesByMachine.Rows.length - 1; p++) {

                                                var strTotalSales = SetPrefixPostfix(AccountingFormat((vDataSalesByMachine.Rows[p].TotalSales || 0)), 14, ' ', 0);
                                                var dblTotalSalesPercent = (parseFloat(vDataSalesByMachine.Rows[p].TotalSales || 0) / dblAllTotalSales) * 100;
                                                var strTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                                dblAllTotalSalesPercent = dblAllTotalSalesPercent + dblTotalSalesPercent;

                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, vDataSalesByMachine.Rows[p].DeviceName, 14, strTotalSales, 30, strTotalSalesPercent, intFAFCTotalLenght, 0);
                                                strPrint = strPrint + strTemp;

                                            };

                                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                            strAllTotalSales = SetPrefixPostfix(AccountingFormat(dblAllTotalSales), 14, ' ', 0);
                                            strAllTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblAllTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                            strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                            strPrint = FormatTextToPrint(intFAFCTotalLenght, '              ', 15, strAllTotalSales, 31, strAllTotalSalesPercent, intFAFCTotalLenght, 1);
                                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                        }
                                    }
                                }

                                //Get Sales by Cashier and Machine
                                if ((strFrom == strTo && $("#selPOSSalesReportRange").val() == "Day") ||
                                            $("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                    if (!IsPrintingOfflineData) {
                                        if (vDataSalesByCashierMachine.Rows.length > 0) {

                                            strPrint = '';
                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, "Sales Grouped by Cashier & Machine (MIN)", intFAFCTotalLenght, '', 0, '', 0, 0);
                                            strPrint = strPrint + strTemp;

                                            ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                            ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                            //Get Total Sales
                                            dblAllTotalSales = 0;
                                            for (var p = 0; p <= vDataSalesByCashierMachine.Rows.length - 1; p++) {
                                                dblAllTotalSales = dblAllTotalSales + parseFloat(vDataSalesByCashierMachine.Rows[p].TotalSales);
                                            };

                                            dblAllTotalSalesPercent = 0;

                                            var strCashierName = "";
                                            var strDeviceName = "";
                                            var dblDeviceSales = 0;

                                            dblAllTotalSalesPercent = 0;

                                            strPrint = '';
                                            for (var p = 0; p <= vDataSalesByCashierMachine.Rows.length - 1; p++) {

                                                if (strCashierName != vDataSalesByCashierMachine.Rows[p].CashierName) {
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, vDataSalesByCashierMachine.Rows[p].CashierName, intFAFCTotalLenght, '', 0, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    strCashierName = vDataSalesByCashierMachine.Rows[p].CashierName;
                                                }

                                                dblDeviceSales = parseFloat(vDataSalesByCashierMachine.Rows[p].TotalSales || 0);
                                                strTotalSales = SetPrefixPostfix(AccountingFormat(dblDeviceSales), 14, ' ', 0);
                                                dblTotalSalesPercent = (dblDeviceSales / dblAllTotalSales) * 100;
                                                strTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                                dblAllTotalSalesPercent = dblAllTotalSalesPercent + dblTotalSalesPercent;

                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, '     ' + vDataSalesByCashierMachine.Rows[p].DeviceName, 14, strTotalSales, 30, strTotalSalesPercent, intFAFCTotalLenght, 0);
                                                strPrint = strPrint + strTemp;

                                            };

                                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                            strAllTotalSales = SetPrefixPostfix(AccountingFormat(dblAllTotalSales), 14, ' ', 0);
                                            strAllTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblAllTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                            strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                            strPrint = FormatTextToPrint(intFAFCTotalLenght, '              ', 15, strAllTotalSales, 31, strAllTotalSalesPercent, intFAFCTotalLenght, 1);
                                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                        }
                                    }
                                }

                                if ($("#selPOSSalesReportRange").val() == "Day" ||
                                    $("#selPOSSalesReportRange").val() == "Today" ||
                                    $("#selPOSSalesReportRange").val() == "Yesterday") {

                                    //Other Informatoin
                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "Other Information", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                    var strTransCount = SetPrefixPostfix(intTransCount + '', intFAFCTotalLenght - 21, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Trans. Count', 19, strTransCount, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    var strSalesTransCount = SetPrefixPostfix(intTransCount + '', intFAFCTotalLenght - 21, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Sales Trans. Count', 19, strSalesTransCount, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    var strSalesItemCount = SetPrefixPostfix(intSalesItemCount + '', intFAFCTotalLenght - 24, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Sales Line Item Count', 22, strSalesItemCount, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    var strSRVTransCount = SetPrefixPostfix(intSRVTransCount + '', intFAFCTotalLenght - 19, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SRV Trans. Count', 17, strSRVTransCount, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    var strSRVItemCount = SetPrefixPostfix(intSRVItemCount + '', intFAFCTotalLenght - 22, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SRV Line Item Count', 20, strSRVItemCount, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    var strRefTransCount = SetPrefixPostfix(intRefTransCount + '', intFAFCTotalLenght - 22, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Refund Trans. Count', 20, strRefTransCount, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    var strRefItemCount = SetPrefixPostfix(intRefItemCount + '', intFAFCTotalLenght - 25, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Refund Line Item Count', 23, strRefItemCount, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    var strCustomerCount = SetPrefixPostfix(OffLineTotalNumberOfCustomers + '', intFAFCTotalLenght - 17, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Total Customer', 15, strCustomerCount, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                    //END Other Informatoin

                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Printed by ', 15, ': ' + (vDataHeader.Rows[0].PrintedBy || ''), intFAFCTotalLenght, '', 0, 1);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataHeader.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);

                                    PrintText(strPrinterIP);
                                    ClearCommandBuffer();

                                } else {
                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Printed by ', 15, ': ' + (vDataHeader.Rows[0].PrintedBy || ''), intFAFCTotalLenght, '', 0, 1);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataHeader.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);

                                    PrintText(strPrinterIP);
                                    ClearCommandBuffer();
                                }

                            } else {
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);
                                PrintText(strPrinterIP);
                                ClearCommandBuffer();
                            }

                        }


                    }
                    else {
                        MessageBox("No available printer for this branch. Please contact your system administrator.",
                            "Printer",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                    }

                }

                function PrintSaleReportNowUsingiMin(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits) {

                    if (vDataPrint.Rows.length > 0) {

                        //var vDataPrint = SalesPayments_offline().getOrgFOWSTPrinterIP();
                        strPrinterIP = vDataPrint.Rows[0].PrinterIPAddress;

                        sessionStorage['ReportDate'] = strFrom;

                        //New set of text to print
                        NewPrintBuilder();

                        if (vDataHeader.Rows[0].OrgBusinessTradeName != undefined) {
                            //Get Header

                            if (CInt2(vDataHeader.Rows[0].PrintCount) > 1) {
                                IminfeedPaper();
                                IminPrintInColumns("", "*** REPRINT ***", "", 0, 10, 0, 0, 1, 0, 0, 25, 0);
                                IminPrintInColumns("", $.format.date(new Date(vDataHeader.Rows[0].PrintDateTime), 'MMM dd, yyyy, hh:mm a'), "", 0, 10, 0, 0, 1, 0, 0, 25, 0);
                                IminPrintInColumns("", "***************", "", 0, 10, 0, 0, 1, 0, 0, 25, 0);
                                IminfeedPaper();
                                IminfeedPaper();
                            }

                            strPrint = '';
                            strTemp = $.trim((vDataHeader.Rows[0].OrgBusinessTradeName || ''));
                            strPrint = strTemp;
                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                            strPrint = '';
                            strTemp = $.trim((vDataHeader.Rows[0].SubscriberOrganizationName || ''));
                            strPrint = strTemp;
                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                            strPrint = '';
                            strTemp = $.trim((vDataHeader.Rows[0].OrgBranchName || ''));
                            strPrint = strTemp;
                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);


                            strTemp = WrapTextByWords((vDataHeader.Rows[0].OrgCompleteAddress || ''), 32);

                            for (var i = 0; i < strTemp.length; i++) {
                                ReadyTextToPrint(strTemp[i], 3, 1, 1, 0, 0, 1, 0, 0, 0);
                            }

                            strPrint = '';
                            strTemp = WrapTextByWords((CBool(Sql.ExecuteScalar('Select IFNULL(IsVAT, 1) from LibraryOrganizationFOWST WHERE ID = ' + ReadyString(sessionStorage.FowstID))) ? '' : 'NON-') + 'VAT REG TIN ' + $.trim((vDataHeader.Rows[0].TIN || vDataHeader.Rows[0].OrgTIN || '')), 32);
                            for (var i = 0; i < strTemp.length; i++) {
                                ReadyTextToPrint(strTemp[i], 3, 1, 1, 0, 0, 1, 0, 0, 0);
                            }

                            //strPrint = '';
                            //strTemp = 'Tel. No.' + $.trim((vDataHeader.Rows[0].OrgTelNo || ''));
                            //strPrint = strTemp;
                            //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                            strTemp = 'Machine S/N: ' + $.trim((vDataHeader.Rows[0].UuidDisplay || ''));
                            for (var i = 0; i < strTemp.length; i += 28) {
                                strPrint = '';
                                strPrint = strTemp.substring(i, ((i + 28) < strTemp.length ? (i + 28) : strTemp.length));
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                            }

                            strTemp = 'PTU No.: ' + $.trim((vDataHeader.Rows[0].POSPermitNo || ''));
                            for (var i = 0; i < strTemp.length; i += 28) {
                                strPrint = '';
                                strPrint = strTemp.substring(i, ((i + 28) < strTemp.length ? (i + 28) : strTemp.length));
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                            }

                            strTemp = 'MIN: ' + $.trim((vDataHeader.Rows[0].MIN || ''));
                            for (var i = 0; i < strTemp.length; i += 28) {
                                strPrint = '';
                                strPrint = strTemp.substring(i, ((i + 28) < strTemp.length ? (i + 28) : strTemp.length));
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                            }

                            IminfeedPaper();
                        }

                        if (vDataHeader.Rows[0].POSReportType == 0) {

                            if (vDataInfo.Rows.length > 0) {
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, (CBool(sessionStorage.PrintingOfflineXReadData) ? '' : '') + "CASHIER'S", intFAFCTotalLenght, '', 0, '', 0, 0);
                                ReadyTextToPrint(strPrint, 3, 2, 1, 1, 1, 1, 0, 0, 0);

                                if ($("#selPOSSalesReportRange").val() == "Day") {
                                    
                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "SALES REPORT (X-READ)", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    if ((new Date(strFrom) - new Date(strTo)) != 0) {
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Date: ' + strFrom + ' to ' + strTo, intFAFCTotalLenght, '', 0, '', 0, 0);
                                        strPrint = strPrint + strTemp;
                                    } else {
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Date : ' + strFrom, intFAFCTotalLenght, '', 0, '', 0, 0);
                                        strPrint = strPrint + strTemp;
                                    }

                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Date : ' + $.format.date(new Date(vDataInfo.Rows[0].SalesDate), 'MM-dd-yyyy'), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;


                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                    IminfeedPaper();

                                    UpdateCSRPrintCount();

                                    //strPrint = '';
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, "SALES AND MONIES REPORT (CSMR)", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? 'Date: ' + strFrom : 'From ' + strFrom + ' To ' + strTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Week") {

                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "Same-Store Comparative Sales Report", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? 'Week: ' + strFrom : 'From Week ' + strFrom + ' To Week ' + strTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Month") {

                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "Same-Store Comparative Sales Report", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? 'Month: ' + $.format.date(new Date(strFrom + '/1/2013'), 'MMM') : 'From ' + $.format.date(new Date(strFrom + '/1/2013'), 'MMM') + ' To ' + $.format.date(new Date(strTo + '/1/2013'), 'MMM')), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Quarter") {

                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "Same-Store Comparative Sales Report", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? (strFrom == '1' ? "1st Quarter" : (strFrom == '2' ? "2nd Quarter" : (strFrom == '3' ? "3rd Quarter" : (strFrom == '4' ? "4th Quarter" : "")))) : 'From ' + (strFrom == '1' ? "1st Quarter" : (strFrom == '2' ? "2nd Quarter" : (strFrom == '3' ? "3rd Quarter" : (strFrom == '4' ? "4th Quarter" : "")))) + ' To ' + (strTo == '1' ? "1st Quarter" : (strTo == '2' ? "2nd Quarter" : (strTo == '3' ? "3rd Quarter" : (strTo == '4' ? "4th Quarter" : ""))))), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Year") {
                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "SALES REPORT", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Year: ' + strYear, intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "SALES REPORT (X-READ)", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    if ((new Date(strFrom) - new Date(strTo)) != 0) {
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Date: ' + strFrom + ' to ' + strTo, intFAFCTotalLenght, '', 0, '', 0, 0);
                                        strPrint = strPrint + strTemp;
                                    } else {
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Date : ' + $.format.date(new Date(strFrom), 'MM-dd-yyyy'), intFAFCTotalLenght, '', 0, '', 0, 0);
                                        strPrint = strPrint + strTemp;
                                    }

                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Date : ' + $.format.date(new Date(vDataInfo.Rows[0].SalesDate), 'MM-dd-yyyy'), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;


                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                    IminfeedPaper();

                                    UpdateCSRPrintCount();
                                }

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Cashier', 9, ' : ' + (vDataInfo.Rows[0].CashierName || ''), intFAFCTotalLenght, '', 0, 1);                                
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                IminfeedPaper();

                                var dblTotalSales = 0;
                                var dblTotalSalesDiscount = 0;
                                var dblTotalVATExemptAmount = 0;
                                var dblAdjRefund = 0;
                                var dblAdjSRV = 0;
                                var dblNetSales = 0;

                                var dblCash = 0;
                                var dblCheck = 0;
                                var dblDebitCard = 0;
                                var dblCreditCard = 0;
                                var dblSRV = 0;
                                var dblOtherPaymentType = 0;
                                var dblOnCredit = 0;
                                var dblCharges = 0;
                                var dblSMAC = 0;
                                var dblGCash = 0;
                                var dblEPlus = 0;
                                var dblVoucher = 0;
                                var dblTotalPayment = 0;

                                var dblVAT = 0;
                                var dblVATAmount = 0;
                                var dblNonVAT = 0;
                                var dblVATExempt = 0;
                                var dblZeroRatedVAT = 0;
                                var dblTotalVAT = 0;
                                var XReadCount = 0;
                                var intVoidedItemQty = 0;
                                var dblTotalVoidAmount = 0;

                                if (!IsPrintingOfflineData) {
                                    for (var p = 0; p <= vDataInfo.Rows.length - 1; p++) {

                                        dblTotalSales = dblTotalSales + parseFloat(vDataInfo.Rows[p].CurGrossSales || 0);
                                        dblTotalSalesDiscount = dblTotalSalesDiscount + parseFloat(vDataInfo.Rows[p].CurSalesDiscount || 0) + parseFloat(vDataInfo.Rows[p].CurSCDiscount || 0);
                                        dblTotalVATExemptAmount = dblTotalVATExemptAmount + parseFloat(vDataInfo.Rows[p].CurVATExempt || 0);
                                        dblAdjRefund = dblAdjRefund + parseFloat(vDataInfo.Rows[p].CurSalesReturnRefund || 0);
                                        dblAdjSRV = dblAdjSRV + parseFloat(vDataInfo.Rows[p].CurSalesReturnExchange || 0);

                                        dblCash = dblCash + parseFloat(vDataInfo.Rows[p].CashAmount || 0);
                                        dblCheck = dblCheck + parseFloat(vDataInfo.Rows[p].CheckPaymentAmount || 0);
                                        dblDebitCard = dblDebitCard + parseFloat(vDataInfo.Rows[p].DebitCardAmount || 0);
                                        dblCreditCard = dblCreditCard + parseFloat(vDataInfo.Rows[p].CreditCardAmount || 0);
                                        dblSRV = dblSRV + parseFloat(vDataInfo.Rows[p].SRVAmount || 0);
                                        dblOtherPaymentType = dblOtherPaymentType + parseFloat(vDataInfo.Rows[p].OtherTypeAmount || 0);
                                        dblCharges = dblCharges + parseFloat(vDataInfo.Rows[p].ChargeAmount || 0);
                                        dblSMAC = dblSMAC + parseFloat(vDataInfo.Rows[p].SMACAmount || 0);
                                        dblGCash = dblGCash + parseFloat(vDataInfo.Rows[p].GCashAmount || 0);
                                        dblEPlus = dblEPlus + parseFloat(vDataInfo.Rows[p].EPlusAmount || 0);
                                        dblVoucher = dblVoucher + parseFloat(vDataInfo.Rows[p].VoucherAmount || 0);

                                        dblVAT = dblVAT + parseFloat(vDataInfo.Rows[p].CurTotalVatablePortionOfSCDTransaction || 0) + parseFloat(vDataInfo.Rows[p].CurTotalVATSales || 0) - parseFloat(vDataInfo.Rows[p].CurTotalVATSalesSalesReturn || 0);
                                        dblVATAmount = dblVATAmount + parseFloat(vDataInfo.Rows[p].CurTotalVatAmountPortionOfSCDTransaction || 0) + parseFloat(vDataInfo.Rows[p].CurTotalVATAmount || 0) - parseFloat(vDataInfo.Rows[p].CurTotalVATAmountSalesReturn || 0);
                                        dblNonVAT = dblNonVAT + parseFloat(vDataInfo.Rows[p].CurNoTAXSales || 0);
                                        dblVATExempt = dblVATExempt + parseFloat(vDataInfo.Rows[p].CurVATExemptSales || 0);
                                        dblZeroRatedVAT = dblZeroRatedVAT + parseFloat(vDataInfo.Rows[p].CurZeroRatedVATSales || 0);

                                        if (vDataInfo.Rows[p].CSRPrintCount != undefined) {
                                            XReadCount = vDataInfo.Rows[p].CSRPrintCount;
                                        }

                                    };
                                }
                                else {
                                    //if (IsPrintingOfflineData) {
                                    dblTotalSales += OfflineTotalSales;
                                    dblTotalSalesDiscount += OfflineTotalSalesDiscount;
                                    //dblTotalVATExemptAmount += OfflineTotalVATExemptAmount;
                                    dblAdjRefund += OfflineAdjRefund;
                                    dblAdjSRV += OfflineAdjSRV;

                                    dblCash += OfflineCash;
                                    dblCheck += OffLineCheck;
                                    dblDebitCard += OffLineDebitCard;
                                    dblCreditCard += OffLineCreditCard;
                                    dblSRV += OffLineSRV;
                                    dblOtherPaymentType += OffLineOtherPayment;
                                    dblOnCredit += OffLineOnCredit;
                                    dblCharges += OffLineCharges;

                                    intVoidedItemQty += OffLineVoidedItemQty;
                                    dblTotalVoidAmount += OffLineTotalVoidAmount;

                                    dblVAT += OffLineVAT;
                                    dblVATAmount += OffLineVATAmount;
                                    dblNonVAT += OffLineNonVAT;
                                    dblVATExempt += OffLineVATExempt;
                                    dblZeroRatedVAT += OffLineZeroRatedVAT;
                                    dblTotalVATExemptAmount += OffLineForeGoneVAT;
                                }

                                dblCash = dblCash;
                                dblNetSales = dblTotalSales - dblTotalSalesDiscount - dblTotalVATExemptAmount - dblAdjRefund - dblAdjSRV + dblCharges;
                                dblTotalPayment = dblCash + dblCheck + dblDebitCard + dblCreditCard + dblSRV;
                                dblTotalVAT = dblVAT + dblVATAmount + dblNonVAT + dblVATExempt + dblZeroRatedVAT;

                                //var strTotalSales = SetPrefixPostfix(AccountingFormat(dblTotalSales), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Gross Sales: ', 14, strTotalSales, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                strPrint = '';
                                strTemp = 'Gross Sales: ';

                                IminPrintInColumns(strTemp, "", AccountingFormat(dblTotalSales), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                //Sales Discount
                                //var strSalesDiscount = SetPrefixPostfix(AccountingFormat(dblTotalSalesDiscount), intFAFCTotalLenght - 23, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Sales Discount', 21, strSalesDiscount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                strPrint = '';
                                strTemp = 'Sales Disc: ';

                                IminPrintInColumns(strTemp, "", AccountingFormat(dblTotalSalesDiscount), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                if (CBool(sessionStorage.PrintingOfflineXReadData) && LastTotalDataCSR.length > 0) {
                                    DisplayDiscountsBreakdown(ConsolidateDiscountBreakdown(), true);
                                } else {
                                    DisplayDiscountsBreakdown(discountCols, true);
                                }

                                //VAT Exempt
                                //var strVATExemptAmount = SetPrefixPostfix(AccountingFormat(dblTotalVATExemptAmount), intFAFCTotalLenght - 23, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Vat Exempt Tax', 21, strVATExemptAmount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                strPrint = '';
                                strTemp = 'Vat-Ex Tax: ';

                                IminPrintInColumns(strTemp, "", AccountingFormat(dblTotalVATExemptAmount), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                IminfeedPaper();                                

                                //Sales Adjustment
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Summary of Adjustments', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Summary of Adjustments ', intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;                                
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                IminfeedPaper();

                                //var strAdjRefund = SetPrefixPostfix(AccountingFormat(dblAdjRefund), intFAFCTotalLenght - 29, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Sales Refund for the  Period', 27, strAdjRefund, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                strPrint = '';
                                strTemp = 'Total Refund: ';
                                IminPrintInColumns(strTemp, "", AccountingFormat(dblAdjRefund), 5, 1, 4, 0, 0, 2, 24, 24, 24);
                                
                                //var strAdjSRV = SetPrefixPostfix(AccountingFormat(dblAdjSRV), intFAFCTotalLenght - 32, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Sales Returns Subject to Merchadise Exchange           (SRV Issued)', 30, strAdjSRV, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                strPrint = '';
                                strTemp = 'Total SRV: ';
                                IminPrintInColumns(strTemp, "", AccountingFormat(dblAdjSRV), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                //var strNetSalesAdj = SetPrefixPostfix(AccountingFormat(parseFloat(dblAdjRefund) + parseFloat(dblAdjSRV)), intFAFCTotalLenght - 23, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Net of Adjustments: ', 21, strNetSalesAdj, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                IminfeedPaper();

                                strPrint = '';
                                strTemp = 'Net of Adj.: ';
                                IminPrintInColumns(strTemp, "", AccountingFormat(dblAdjRefund + dblAdjSRV), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                strPrint = '';
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                strPrint = strPrint + strTemp;
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                //var strNetSales = SetPrefixPostfix(AccountingFormat(dblNetSales), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Net Sales: ', 12, '  ' + strNetSales, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //charges
                                strPrint = '';
                                strTemp = 'Add Charges: ';
                                IminPrintInColumns(strTemp, "", AccountingFormat(dblCharges), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                strPrint = '';
                                strTemp = 'Net Sales: ';
                                IminPrintInColumns(strTemp, "", AccountingFormat(dblNetSales), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                //strPrint = '';
                                //strTemp = 'Add Used Up SRV: ';

                                //IminPrintInColumns(strTemp, "", AccountingFormat(dblSRV), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                //strPrint = '';
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //strPrint = strPrint + strTemp;
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                //strPrint = '';
                                //strTemp = 'Net Sales Adj. for Exchanges: ';

                                //IminPrintInColumns(strTemp, "", AccountingFormat(parseFloat(dblNetSales) + parseFloat(dblSRV)), 5, 1, 4, 0, 0, 2, 24, 24, 24);
                                
                                IminfeedPaper();

                                ////var strIssuedSRV = SetPrefixPostfix(AccountingFormat(dblAdjSRV), intFAFCTotalLenght - 20, ' ', 0);
                                ////strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Add : Issued SRV:', 18, strIssuedSRV, intFAFCTotalLenght, '', 0, 1);
                                ////ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                //strPrint = '';
                                //strTemp = 'Add : Issued SRV: ';
                                //IminPrintInColumns(strTemp, "", AccountingFormat(dblAdjSRV), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                ////strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                ////ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                //strPrint = '';
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //strPrint = strPrint + strTemp;
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                //var strTotalSales = SetPrefixPostfix(AccountingFormat(dblNetSales + dblAdjSRV), intFAFCTotalLenght - 21, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Net Sales Adjusted for Exchanges', 33, '', 0, '', 0, 1);
                                ////                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //strPrint = '';
                                //strTemp = 'Net Sales Adjusted for Exchanges: ';
                                //IminPrintInColumns(strTemp, "", AccountingFormat(dblNetSales + dblAdjSRV), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '', 0, '', 0, strTotalSales, intFAFCTotalLenght, 1);
                                //ReadyTextToPrint(strPrint, 3, 2, 2, 0, 1, 1, 0, 0, 0);

                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' for Exchanges', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //END Sales Adjustment

                                //Sales Grouped By Mode of Payment
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, "Inventory of Payments", intFAFCTotalLenght, '', 0, '', 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Inventory of Payments', intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                IminfeedPaper();

                                //var strTotalCash = SetPrefixPostfix(AccountingFormat(dblCash), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Cash       ', 12, ': ' + strTotalCash, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                strPrint = '';
                                strTemp = 'Cash: ';
                                IminPrintInColumns(strTemp, "", AccountingFormat(dblCash), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                //var strTotalCheck = SetPrefixPostfix(AccountingFormat(dblCheck), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Check      ', 12, ': ' + strTotalCheck, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                strPrint = '';
                                strTemp = 'Check: ';
                                IminPrintInColumns(strTemp, "", AccountingFormat(dblCheck), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                //var strTotalDebitCard = SetPrefixPostfix(AccountingFormat(dblDebitCard), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Debit Card ', 12, ': ' + strTotalDebitCard, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                strPrint = '';
                                strTemp = 'Debit Card: ';
                                IminPrintInColumns(strTemp, "", AccountingFormat(dblDebitCard), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                //var strTotalCreditCard = SetPrefixPostfix(AccountingFormat(dblCreditCard), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Credit Card', 12, ': ' + strTotalCreditCard, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                strPrint = '';
                                strTemp = 'Credit Card: ';
                                IminPrintInColumns(strTemp, "", AccountingFormat(dblCreditCard), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                //var strTotalSRV = SetPrefixPostfix(AccountingFormat(dblSRV), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SRV        ', 12, ': ' + strTotalSRV, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                strPrint = '';
                                strTemp = 'SRV: ';
                                IminPrintInColumns(strTemp, "", AccountingFormat(dblSRV), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                var totalOtherPayment = 0;
                                if (IsFowstInSMMall) {
                                    var strTotalSMAC = SetPrefixPostfix(AccountingFormat(dblSMAC), intFAFCTotalLenght - 16, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SMAC        ', 12, ': ' + strTotalSMAC, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                    var strTotalEPlus = SetPrefixPostfix(AccountingFormat(dblEPlus), intFAFCTotalLenght - 16, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' E-Plus      ', 12, ': ' + strTotalEPlus, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                    var strTotalVoucher = SetPrefixPostfix(AccountingFormat(dblVoucher), intFAFCTotalLenght - 16, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Voucher     ', 12, ': ' + strTotalVoucher, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);


                                } else {
                                    //May 14 2018 gi comment ang Other Tender kay magconflict sa G-Cash na payment type. pwede ang g-cash inside and outside sa mall

                                    //Feb 8 2020 gibalik ang code para maisa ang pagdisplay sa tanan other types na payment

                                    //var strTotalOther = SetPrefixPostfix(AccountingFormat(0), intFAFCTotalLenght - 16, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Other Tender', 12, ': ' + strTotalOther, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                    if (vDataDataSalesReportOtherPaymentInfo != undefined) {
                                        if (vDataDataSalesReportOtherPaymentInfo.Rows.length > 0) {
                                            if (CBool(sessionStorage.PrintingOfflineXReadData)) {
                                                var mOtherPaymentTypeColumns = Object.entries(vDataDataSalesReportOtherPaymentInfo.Columns);
                                                //i=10 since magsugod ang other payment type sa 
                                                for (var o in vDataDataSalesReportOtherPaymentInfo.Rows) {
                                                    for (var c in vDataDataSalesReportOtherPaymentInfo.Columns) {
                                                        var opname = vDataDataSalesReportOtherPaymentInfo.Columns[c].replace('OtherPayment', '');
                                                        var amt = CDec2(vDataDataSalesReportOtherPaymentInfo.Rows[o][vDataDataSalesReportOtherPaymentInfo.Columns[c]]);

                                                        if (amt == 0) continue;

                                                        if (opname.toLowerCase() == 'potatocornergc') {
                                                            continue;
                                                        } else {
                                                            totalOtherPayment += amt;
                                                            IminPrintInColumns(opname + ": ", "", AccountingFormat(amt), 5, 1, 4, 0, 0, 2, 24, 24, 24);
                                                        }
                                                    }
                                                }
                                            } else {
                                                for (var o in vDataDataSalesReportOtherPaymentInfo.Rows) {
                                                    if (CDec2(vDataDataSalesReportOtherPaymentInfo.Rows[o].Amount) == 0) continue;

                                                    totalOtherPayment += CDec2(vDataDataSalesReportOtherPaymentInfo.Rows[o].Amount);
                                                    IminPrintInColumns(vDataDataSalesReportOtherPaymentInfo.Rows[o].PaymentType + ": ", "", AccountingFormat(vDataDataSalesReportOtherPaymentInfo.Rows[o].Amount), 5, 1, 4, 0, 0, 2, 24, 24, 24);
                                                }
                                            }                                            
                                        }
                                    }
                                }
                                //Feb 8 2020 gicomment ni para maisa ang pagdisplay sa tanan other types na payment
                                //if (dblGCash > 0) {
                                //    var strTotalGCash = SetPrefixPostfix(AccountingFormat(dblGCash), intFAFCTotalLenght - 16, ' ', 0);
                                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' GCash       ', 12, ': ' + strTotalGCash, intFAFCTotalLenght, '', 0, 1);
                                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //}


                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                strPrint = '';
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                strPrint = strPrint + strTemp;
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                
                                //var strTotalClassPayment = SetPrefixPostfix(AccountingFormat(dblTotalPayment), intFAFCTotalLenght - 21, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Inventory of Payments', 28, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                dblTotalPayment += totalOtherPayment;

                                strPrint = '';
                                strTemp = 'Total Payments: ';
                                IminPrintInColumns(strTemp, "", AccountingFormat(dblTotalPayment), 7, 0, 4, 0, 0, 2, 24, 24, 24);

                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '', 0, '', 0, strTotalClassPayment, intFAFCTotalLenght, 1);
                                //ReadyTextToPrint(strPrint, 3, 2, 2, 0, 1, 1, 0, 0, 0);

                                var dblTotalOverPayment = dblTotalPayment - (parseFloat(dblNetSales)); // + parseFloat(dblSRV)
                                //////                                                    var strTotalOverPayment = SetPrefixPostfix(AccountingFormat((dblTotalOverPayment < 0 ? (dblTotalOverPayment * -1) : dblTotalOverPayment)), intFAFCTotalLenght - 25, ' ', 0);
                                //var strTotalOverPayment = SetPrefixPostfix(AccountingFormat(dblTotalOverPayment), intFAFCTotalLenght - 25, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Excess of Inventory of', 23, strTotalOverPayment, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                if (dblAdjSRV > 0) {
                                    strPrint = '';
                                    strTemp = 'Adjustments (SRV';
                                    IminPrintInColumns(strTemp, "", '-' + AccountingFormat(dblAdjSRV), 7, 1, 4, 0, 0, 2, 24, 24, 24);
                                    strTemp = 'Liability):';
                                    IminPrintInColumns(strTemp, "", "", 7, 1, 4, 0, 0, 2, 24, 24, 24);

                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                    strPrint = strPrint + strTemp;
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    strPrint = '';
                                    strTemp = 'Current Standing: ';
                                    IminPrintInColumns(strTemp, "", AccountingFormat(dblTotalPayment - dblAdjSRV), 7, 0, 4, 0, 0, 2, 24, 24, 24);
                                }                                
                                                     
                                IminfeedPaper();                                

                                //END Sales Grouped By Mode of Payment

                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Inventory of Payments Audit', intFAFCTotalLenght, '', 0, '', 0, 0);
                                //strPrint = strPrint + strTemp;
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                //IminfeedPaper();

                                //strPrint = '';
                                //strTemp = 'Net Sales Adj. for Exchanges: ';

                                //IminPrintInColumns(strTemp, "", AccountingFormat(parseFloat(dblNetSales) + parseFloat(dblSRV)), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                //IminfeedPaper();

                                //strPrint = '';
                                //strTemp = 'Total Payments: ';
                                //IminPrintInColumns(strTemp, "", AccountingFormat(dblTotalPayment), 7, 0, 4, 0, 0, 2, 24, 24, 24);


                                //Inventory of Payments Audit
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, "Inventory of Payments Audit", intFAFCTotalLenght, '', 0, '', 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);


                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Inventory of Payments Audit', intFAFCTotalLenght, '', 0, '', 0, 0);
                                //strPrint = strPrint + strTemp;
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '  Net Sales Adjusted', 19, strTotalSales, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' for Exchanges', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //strPrint = '';
                                //strTemp = 'Net Sales Adjusted for Exchanges: ';
                                //IminPrintInColumns(strTemp, "", AccountingFormat(dblNetSales + dblAdjSRV), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '  Total Inventory', 19, strTotalClassPayment, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' of Payments', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //strPrint = '';
                                //strTemp = 'Total Inventory of Payments: ';
                                //IminPrintInColumns(strTemp, "", AccountingFormat(dblTotalPayment), 5, 1, 4, 0, 0, 2, 24, 24, 24);
                                
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                //strPrint = '';
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //strPrint = strPrint + strTemp;
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                //var dblNetSalesAdjustedLessTotalInvPayment = (dblNetSales + dblSRV) - dblTotalPayment + dblCharges;
                                //strPrint = '';
                                //strTemp = (dblNetSalesAdjustedLessTotalInvPayment < 0 ? ' Shortage: ' : ' Overage: ');
                                //IminPrintInColumns(strTemp, "", AccountingFormat(dblNetSalesAdjustedLessTotalInvPayment), 7, 0, 4, 0, 0, 2, 24, 24, 24);

                                //IminfeedPaper();
                                ////END Inventory of Payments Audit

                                //strPrint = '';
                                //strTemp = (dblNetSalesAdjustedLessTotalInvPayment < 0 ? ' Shortage' : ' Overage ');
                                //IminPrintInColumns(strTemp, "", AccountingFormat(dblNetSalesAdjustedLessTotalInvPayment), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                //var TotalTurnOverAndDeposits = 0;

                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, "Turnovers and Deposits", intFAFCTotalLenght, '', 0, '', 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                //if (sessionStorage.Printer == 'SunmiPrinter') {
                                //    strPrint = ' ';
                                //    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Action', 7, 'To', 25, 'Amount', intFAFCTotalLenght, 1);
                                //    strPrint = strPrint + strTemp;
                                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //} else {
                                //    strPrint = ' ';
                                //    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Action', 14, 'To', 30, 'Amount', intFAFCTotalLenght, 1);
                                //    strPrint = strPrint + strTemp;
                                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //}

                                //strPrint = ' ';

                                //for (var toCounter = 0; toCounter <= vDataTurnOverAndDeposits.Rows.length - 1; toCounter++) {

                                //    if (vDataTurnOverAndDeposits.Rows[toCounter].IsTurnOver != undefined) {
                                //        strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataTurnOverAndDeposits.Rows[toCounter].IsTurnOver == 1 ? 'Turned Over' : 'Deposited'), 14, vDataTurnOverAndDeposits.Rows[toCounter].DepositedTo, 30, vDataTurnOverAndDeposits.Rows[toCounter].TotalAmount, intFAFCTotalLenght, 1);
                                //        strPrint = strPrint + strTemp;

                                //        TotalTurnOverAndDeposits += CDec2(vDataTurnOverAndDeposits.Rows[toCounter].TotalAmount);
                                //    }
                                //}

                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                //strPrint = '';
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Total Turnovers and Deposits', 30, ': ' + TotalTurnOverAndDeposits, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //Sales Grouped by VAT TYPE----------------------------
                                //if ((strFrom == strTo && $("#selPOSSalesReportRange").val() == "Day") ||
                                //        $("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, "Sales Grouped by VAT Type", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                    //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Sales Grouped by VAT Type', intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                    IminfeedPaper();

                                    var dblTotalVATSalesPercent = (dblVAT / dblTotalVAT) * 100;
                                    var dblTotalVATAmountPercent = (dblVATAmount / dblTotalVAT) * 100;
                                    var dblTotalNonVATPercent = (dblNonVAT / dblTotalVAT) * 100;
                                    var dblTotalVATExemptPercent = (dblVATExempt / dblTotalVAT) * 100;
                                    var dblTotalZeroRatedVATPercent = (dblZeroRatedVAT / dblTotalVAT) * 100;
                                    var dblTotalVATPercent = dblTotalVATSalesPercent + dblTotalVATAmountPercent + dblTotalNonVATPercent + dblTotalVATExemptPercent + dblTotalZeroRatedVATPercent;


                                    var strTotalVATSales = SetPrefixPostfix(AccountingFormat(dblVAT), 14, ' ', 0);
                                    var strTotalVATAmount = SetPrefixPostfix(AccountingFormat(dblVATAmount), 14, ' ', 0);
                                    var strTotalNonVAT = SetPrefixPostfix(AccountingFormat(dblNonVAT), 14, ' ', 0);
                                    var strTotalVATExempt = SetPrefixPostfix(AccountingFormat(dblVATExempt), 14, ' ', 0);
                                    var strTotalZeroRatedVAT = SetPrefixPostfix(AccountingFormat(dblZeroRatedVAT), 14, ' ', 0);
                                    var strTotalAllVAT = SetPrefixPostfix(AccountingFormat(dblTotalVAT), 14, ' ', 0);

                                    var strTotalVATSalesPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATSalesPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalVATAmountPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATAmountPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalNonVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalNonVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalVATExemptPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATExemptPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalZeroRatedVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalZeroRatedVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalAllVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);

                                    if (IsVAT != undefined) {
                                        if (!CBool(IsVAT)) {
                                            strTotalVATSales = SetPrefixPostfix(AccountingFormat(0), 14, ' ', 0);
                                            strTotalVATAmount = SetPrefixPostfix(AccountingFormat(0), 14, ' ', 0);
                                            strTotalNonVAT = SetPrefixPostfix(AccountingFormat(CDec2(dblVAT) + CDec2(dblVATAmount)), 14, ' ', 0);

                                            strTotalVATSalesPercent = SetPrefixPostfix(AccountingFormat(0) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                            strTotalVATAmountPercent = SetPrefixPostfix(AccountingFormat(0) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                            strTotalNonVATPercent = SetPrefixPostfix(AccountingFormat(100) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                        }
                                    }

                                    if (sessionStorage.Printer == 'SunmiPrinter') {

                                        strTotalVATSales = SetPrefixPostfix(AccountingFormat(dblVAT), 7, ' ', 0); //7=14
                                        strTotalVATAmount = SetPrefixPostfix(AccountingFormat(dblVATAmount), 7, ' ', 0);
                                        strTotalNonVAT = SetPrefixPostfix(AccountingFormat(dblNonVAT), 7, ' ', 0);
                                        strTotalVATExempt = SetPrefixPostfix(AccountingFormat(dblVATExempt), 7, ' ', 0);
                                        strTotalZeroRatedVAT = SetPrefixPostfix(AccountingFormat(dblZeroRatedVAT), 7, ' ', 0);
                                        strTotalAllVAT = SetPrefixPostfix(AccountingFormat(dblTotalVAT), 7, ' ', 0);

                                        strPrint = ' ';
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-able Sales', 14, strTotalVATSales, 25, strTotalVATSalesPercent, intFAFCTotalLenght, 1);
                                        strPrint = strPrint + strTemp;

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-Amount', 14, strTotalVATAmount, 25, strTotalVATAmountPercent, intFAFCTotalLenght, 1);
                                        strPrint = strPrint + strTemp;

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Non-VAT', 14, strTotalNonVAT, 25, strTotalNonVATPercent, intFAFCTotalLenght, 1);
                                        strPrint = strPrint + strTemp;

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-Exempt', 14, strTotalVATExempt, 25, strTotalVATExemptPercent, intFAFCTotalLenght, 1);
                                        strPrint = strPrint + strTemp;

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Zero-Rated VAT', 14, strTotalZeroRatedVAT, 25, strTotalZeroRatedVATPercent, intFAFCTotalLenght, 1);
                                        strPrint = strPrint + strTemp;
                                    } else {
                                        //strPrint = ' ';
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-able Sales', 14, strTotalVATSales, 30, strTotalVATSalesPercent, intFAFCTotalLenght, 1);
                                        //strPrint = strPrint + strTemp;

                                        strPrint = '';
                                        strTemp = 'VAT-able Sales: ';
                                        IminPrintInColumns(strTemp, "", (CBool(vDataHeader.Rows[0].IsVAT) ? AccountingFormat(dblVAT) : '-'), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-Amount', 14, strTotalVATAmount, 30, strTotalVATAmountPercent, intFAFCTotalLenght, 1);
                                        //strPrint = strPrint + strTemp;

                                        strPrint = '';
                                        strTemp = 'VAT-Amount: ';
                                        IminPrintInColumns(strTemp, "", (CBool(vDataHeader.Rows[0].IsVAT) ? AccountingFormat(dblVATAmount) : '-'), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Non-VAT', 14, strTotalNonVAT, 30, strTotalNonVATPercent, intFAFCTotalLenght, 1);
                                        //strPrint = strPrint + strTemp;

                                        strPrint = '';
                                        strTemp = 'Non-VAT: ';
                                        IminPrintInColumns(strTemp, "", (CBool(vDataHeader.Rows[0].IsVAT) ? AccountingFormat(dblNonVAT) : AccountingFormat(dblNetSales)), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-Exempt', 14, strTotalVATExempt, 30, strTotalVATExemptPercent, intFAFCTotalLenght, 1);
                                        //strPrint = strPrint + strTemp;

                                        strPrint = '';
                                        strTemp = 'VAT-Exempt: ';
                                        IminPrintInColumns(strTemp, "", (CBool(vDataHeader.Rows[0].IsVAT) ? AccountingFormat(dblVATExempt) : '-'), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Zero-Rated VAT', 14, strTotalZeroRatedVAT, 30, strTotalZeroRatedVATPercent, intFAFCTotalLenght, 1);
                                        //strPrint = strPrint + strTemp;

                                        strPrint = '';
                                        strTemp = 'Zero-Rated VAT: ';
                                        IminPrintInColumns(strTemp, "", (CBool(vDataHeader.Rows[0].IsVAT) ? AccountingFormat(dblZeroRatedVAT) : '-'), 5, 1, 4, 0, 0, 2, 24, 24, 24);
                                    }


                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                    strPrint = strPrint + strTemp;
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    strPrint = '';
                                    strTemp = ' ';
                                    IminPrintInColumns(strTemp, "", (CBool(vDataHeader.Rows[0].IsVAT) ? AccountingFormat(dblTotalVAT) : AccountingFormat(dblNetSales)), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                    IminfeedPaper();

                                    //strPrint = '';
                                    //strTemp = 'Zero-Rated VAT: ';
                                    //IminPrintInColumns(strTemp, "", AccountingFormat(dblTotalVAT), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                    //if (sessionStorage.Printer == 'SunmiPrinter') {
                                    //    strPrint = FormatTextToPrint(intFAFCTotalLenght, '               ', 14, strTotalAllVAT, 23, strTotalAllVATPercent, intFAFCTotalLenght, 1);
                                    //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                    //} else {
                                    //    strPrint = FormatTextToPrint(intFAFCTotalLenght, '               ', 14, strTotalAllVAT, 30, strTotalAllVATPercent, intFAFCTotalLenght, 1);
                                    //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                    //}


                                //}
                                //End Sales Grouped by VAT TYPE----------------------------

                                //strPrint = '';
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Printed by ', 15, ': ' + (vDataHeader.Rows[0].PrintedBy || ''), intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Other Information', intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                //IminfeedPaper();

                                //IminPrintInColumns("Voided Qty", "", intVoidedItemQty, 6, 1, 3, 0, 0, 2, 24, 24, 24);
                                //IminPrintInColumns("Total Void Amt", "", AccountingFormat(dblTotalVoidAmount), 6, 1, 3, 0, 0, 2, 24, 24, 24);

                                IminfeedPaper();
                                IminfeedPaper();

                                IminPrintInColumns("Printed by ", "", (vDataHeader.Rows[0].PrintedBy || ''), 3, 1, 6, 0, 0, 2, 24, 24, 24);

                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, ' XRead Print Count ', 20, ': ' + XReadCount, intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;

                                //IminPrintInColumns("Print Count ", "", (XReadCount || ''), 5, 0, 6, 0, 0, 2, 24, 24, 24);


                                IminPrintInColumns("Date and Time ", "", ($.format.date(new Date(vDataHeader.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), 5, 0, 6, 0, 0, 2, 24, 24, 24);

                                IminfeedPaper();
                                IminfeedPaper();
                                IminfeedPaper();
                                IminfeedPaper();
                                IminfeedPaper();
                                IminfeedPaper();

                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataHeader.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;

                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);

                                //PrintText(strPrinterIP);
                                //ClearCommandBuffer();

                            } else {
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);
                                PrintText(strPrinterIP);
                                ClearCommandBuffer();
                            }

                        } else {
                            if (vDataInfo.Rows.length > 0) {

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, (CBool(sessionStorage.PrintingOfflineXReadData) ? 'DRAFT ' : '') + "MANAGER'S", intFAFCTotalLenght, '', 0, '', 0, 0);

                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, "MANAGER'S", intFAFCTotalLenght, '', 0, '', 0, 0);
                                ReadyTextToPrint(strPrint, 3, 2, 1, 1, 1, 1, 0, 0, 0);

                                if ($("#selPOSSalesReportRange").val() == "Day") {

                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "SALES REPORT", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? 'Date: ' + strFrom : 'From ' + strFrom + ' To ' + strTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Week") {

                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "Same-Store Comparative Sales Report", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? 'Week: ' + strFrom : 'From Week ' + strFrom + ' To Week ' + strTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Month") {

                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "Same-Store Comparative Sales Report", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? 'Month: ' + $.format.date(new Date(strFrom + '/1/2013'), 'MMM') : 'From ' + $.format.date(new Date(strFrom + '/1/2013'), 'MMM') + ' To ' + $.format.date(new Date(strTo + '/1/2013'), 'MMM')), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Quarter") {

                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "Same-Store Comparative Sales Report", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? (strFrom == '1' ? "1st Quarter" : (strFrom == '2' ? "2nd Quarter" : (strFrom == '3' ? "3rd Quarter" : (strFrom == '4' ? "4th Quarter" : "")))) : 'From ' + (strFrom == '1' ? "1st Quarter" : (strFrom == '2' ? "2nd Quarter" : (strFrom == '3' ? "3rd Quarter" : (strFrom == '4' ? "4th Quarter" : "")))) + ' To ' + (strTo == '1' ? "1st Quarter" : (strTo == '2' ? "2nd Quarter" : (strTo == '3' ? "3rd Quarter" : (strTo == '4' ? "4th Quarter" : ""))))), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Year") {
                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "SALES REPORT", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Year: ' + strYear, intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {
                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "SALES REPORT", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Date : ' + $.format.date(new Date(strFrom), 'MM-dd-yyyy'), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                }

                                var dblTotalSales = 0;
                                var dblTotalSalesDiscount = 0;
                                var dblTotalVATExemptAmount = 0;
                                var dblAdjRefund = 0;
                                var dblAdjSRV = 0;
                                var dblNetSales = 0;

                                var dblCash = 0;
                                var dblCheck = 0;
                                var dblDebitCard = 0;
                                var dblCreditCard = 0;
                                var dblSRV = 0;
                                var dblOtherPaymentType = 0;
                                var dblCharges = 0;
                                var dblSMAC = 0;
                                var dblGCash = 0;
                                var dblEPlus = 0;
                                var dblVoucher = 0;
                                var dblTotalPayment = 0;

                                var dblVAT = 0;
                                var dblVATAmount = 0;
                                var dblNonVAT = 0;
                                var dblVATExempt = 0;
                                var dblZeroRatedVAT = 0;
                                var dblTotalVAT = 0;

                                var intTransCount = 0;
                                var intSalesTransCount = 0;
                                var intSalesItemCount = 0;
                                var intSRVTransCount = 0;
                                var intSRVItemCount = 0;
                                var intRefTransCount = 0;
                                var intRefItemCount = 0;

                                if (!IsPrintingOfflineData) {
                                    for (var p = 0; p <= vDataInfo.Rows.length - 1; p++) {

                                        dblTotalSales = dblTotalSales + parseFloat(vDataInfo.Rows[p].CurGrossSales || 0);
                                        dblTotalSalesDiscount = dblTotalSalesDiscount + parseFloat(vDataInfo.Rows[p].CurSalesDiscount || 0) + parseFloat(vDataInfo.Rows[p].CurSCDiscount || 0);
                                        dblTotalVATExemptAmount = dblTotalVATExemptAmount + parseFloat(vDataInfo.Rows[p].CurVATExempt || 0);
                                        dblAdjRefund = dblAdjRefund + parseFloat(vDataInfo.Rows[p].CurSalesReturnRefund || 0);
                                        dblAdjSRV = dblAdjSRV + parseFloat(vDataInfo.Rows[p].CurSalesReturnExchange || 0);

                                        dblCash = dblCash + parseFloat(vDataInfo.Rows[p].CashAmount || 0);
                                        dblCheck = dblCheck + parseFloat(vDataInfo.Rows[p].CheckPaymentAmount || 0);
                                        dblDebitCard = dblDebitCard + parseFloat(vDataInfo.Rows[p].DebitCardAmount || 0);
                                        dblCreditCard = dblCreditCard + parseFloat(vDataInfo.Rows[p].CreditCardAmount || 0);
                                        dblSRV = dblSRV + parseFloat(vDataInfo.Rows[p].SRVAmount || 0);
                                        dblOtherPaymentType = dblOtherPaymentType + parseFloat(vDataInfo.Rows[p].OtherTypeAmount || 0);
                                        dblCharges = dblCharges + parseFloat(vDataInfo.Rows[p].ChargeAmount || 0);
                                        dblSMAC = dblSMAC + parseFloat(vDataInfo.Rows[p].SMACAmount || 0);
                                        dblGCash = dblGCash + parseFloat(vDataInfo.Rows[p].GCashAmount || 0);
                                        dblEPlus = dblEPlus + parseFloat(vDataInfo.Rows[p].EPlusAmount || 0);
                                        dblVoucher = dblVoucher + parseFloat(vDataInfo.Rows[p].VoucherAmount || 0);

                                        dblVAT = dblVAT + parseFloat(vDataInfo.Rows[p].CurTotalVatablePortionOfSCDTransaction || 0) + parseFloat(vDataInfo.Rows[p].CurTotalVATSales || 0) - parseFloat(vDataInfo.Rows[p].CurTotalVATSalesSalesReturn || 0);
                                        dblVATAmount = dblVATAmount + parseFloat(vDataInfo.Rows[p].CurTCurTotalVatAmountPortionOfSCDTransactionotalVATAmount || 0) + parseFloat(vDataInfo.Rows[p].CurTotalVATAmount || 0) - parseFloat(vDataInfo.Rows[p].CurTotalVATAmountSalesReturn || 0);
                                        dblNonVAT = dblNonVAT + parseFloat(vDataInfo.Rows[p].CurNoTAXSales || 0);
                                        dblVATExempt = dblVATExempt + parseFloat(vDataInfo.Rows[p].CurVATExemptSales || 0);
                                        dblZeroRatedVAT = dblZeroRatedVAT + parseFloat(vDataInfo.Rows[p].CurZeroRatedVATSales || 0);

                                        intTransCount = intTransCount + parseFloat(vDataInfo.Rows[p].CurTransCountInv || 0) + parseFloat(vDataInfo.Rows[p].CurTransCountMemoReturnExchange || 0) + parseFloat(vDataInfo.Rows[p].CurTransCountMemoRefund || 0);
                                        intSalesTransCount = intSalesTransCount + parseFloat(vDataInfo.Rows[p].CurTransCountInv || 0);
                                        intSalesItemCount = intSalesItemCount + parseFloat(vDataInfo.Rows[p].CurTransInvItemCount || 0);
                                        intSRVTransCount = intSRVTransCount + parseFloat(vDataInfo.Rows[p].CurTransCountMemoReturnExchange || 0);
                                        intSRVItemCount = intSRVItemCount + parseFloat(vDataInfo.Rows[p].CurSalesReturnExchangeItemCount || 0);
                                        intRefTransCount = intRefTransCount + parseFloat(vDataInfo.Rows[p].CurTransCountMemoRefund || 0);
                                        intRefItemCount = intRefItemCount + parseFloat(vDataInfo.Rows[p].CurSalesRefundItemCount || 0);

                                    };
                                }
                                else {
                                    dblTotalSales += OfflineTotalSales;
                                    dblTotalSalesDiscount += OfflineTotalSalesDiscount;
                                    //dblTotalVATExemptAmount += OfflineTotalVATExemptAmount;
                                    dblAdjRefund += OfflineAdjRefund;
                                    dblAdjSRV += OfflineAdjSRV;

                                    dblCash += OfflineCash;
                                    dblCheck += OffLineCheck;
                                    dblDebitCard += OffLineDebitCard;
                                    dblCreditCard += OffLineCreditCard;
                                    dblSRV += OffLineSRV;
                                    dblOtherPaymentType += OffLineOtherPayment;
                                    dblOnCredit += OffLineOnCredit;
                                    dblCharges += OffLineCharges;

                                    dblVAT += OffLineVAT;
                                    dblVATAmount += OffLineVATAmount;
                                    dblNonVAT += OffLineNonVAT;
                                    dblVATExempt += OffLineVATExempt;
                                    dblZeroRatedVAT += OffLineZeroRatedVAT;
                                    dblTotalVATExemptAmount += OffLineForeGoneVAT;

                                    intTransCount = OfflineSalesTransCount;
                                    intSalesItemCount = OfflineSalesItemCount;
                                }

                                dblCash = dblCash - dblAdjRefund;
                                dblNetSales = dblTotalSales - dblTotalSalesDiscount - dblTotalVATExemptAmount - dblAdjRefund - dblAdjSRV + dblCharges;
                                dblTotalPayment = dblCash + dblCheck + dblDebitCard + dblCreditCard + dblSRV + dblOtherPaymentType;
                                dblTotalVAT = dblVAT + dblVATAmount + dblNonVAT + dblVATExempt + dblZeroRatedVAT;

                                var strTotalSales = SetPrefixPostfix(AccountingFormat(dblTotalSales), intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Gross Sales: ', 14, strTotalSales, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //Sales Discount
                                var strSalesDiscount = SetPrefixPostfix(AccountingFormat(dblTotalSalesDiscount), intFAFCTotalLenght - 23, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Sales Discount', 21, strSalesDiscount, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                //VAT Exempt
                                var strVATExemptAmount = SetPrefixPostfix(AccountingFormat(dblTotalVATExemptAmount), intFAFCTotalLenght - 23, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total VAT Exempt Tax', 21, strVATExemptAmount, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                //charges
                                if (dblCharges > 0) {
                                    var strTotalCharges = SetPrefixPostfix(AccountingFormat(dblCharges), intFAFCTotalLenght - 23, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Charges', 21, strTotalCharges, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                }


                                //Sales Adjustment
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Summary of Adjustments', intFAFCTotalLenght, '', 0, '', 0, 1);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                var strAdjRefund = SetPrefixPostfix(AccountingFormat(dblAdjRefund), intFAFCTotalLenght - 29, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Sales Refund for the  Period', 27, strAdjRefund, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strAdjSRV = SetPrefixPostfix(AccountingFormat(dblAdjSRV), intFAFCTotalLenght - 32, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Sales Returns Subject to Merchadise Exchange           (SRV Issued)', 30, strAdjSRV, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strNetSalesAdj = SetPrefixPostfix(AccountingFormat(parseFloat(dblAdjRefund) + parseFloat(dblAdjSRV)), intFAFCTotalLenght - 23, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Net of Adjustments: ', 21, strNetSalesAdj, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                var strNetSales = SetPrefixPostfix(AccountingFormat(dblNetSales), intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Net Sales: ', 12, '  ' + strNetSales, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                var strIssuedSRV = SetPrefixPostfix(AccountingFormat(dblAdjSRV), intFAFCTotalLenght - 22, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Add : Issued SRV:', 18, strIssuedSRV, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                var strTotalSales = SetPrefixPostfix(AccountingFormat(dblNetSales + dblAdjSRV), intFAFCTotalLenght - 21, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Net Sales Adjusted for Exchanges', 34, '', intFAFCTotalLenght, '', 0, 1);
                                //                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' for Exchanges', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '', 0, '', 0, strTotalSales, intFAFCTotalLenght, 1);
                                ReadyTextToPrint(strPrint, 3, 2, 2, 0, 1, 1, 0, 0, 0);


                                //END Sales Adjustment

                                //Sales Grouped By Mode of Payment
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, "Inventory of Payments", intFAFCTotalLenght, '', 0, '', 0, 0);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                var strTotalCash = SetPrefixPostfix(AccountingFormat(dblCash), intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Cash       ', 12, ': ' + strTotalCash, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                var strTotalCheck = SetPrefixPostfix(AccountingFormat(dblCheck), intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Check      ', 12, ': ' + strTotalCheck, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                var strTotalDebitCard = SetPrefixPostfix(AccountingFormat(dblDebitCard), intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Debit Card', 12, ': ' + strTotalDebitCard, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                var strTotalCreditCard = SetPrefixPostfix(AccountingFormat(dblCreditCard), intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Credit Card', 12, ': ' + strTotalCreditCard, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                var strTotalSRV = SetPrefixPostfix(AccountingFormat(dblSRV), intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SRV        ', 12, ': ' + strTotalSRV, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                if (IsFowstInSMMall) {
                                    var strTotalSMAC = SetPrefixPostfix(AccountingFormat(dblSMAC), intFAFCTotalLenght - 16, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SMAC        ', 12, ': ' + strTotalSMAC, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                    var strTotalEPlus = SetPrefixPostfix(AccountingFormat(dblEPlus), intFAFCTotalLenght - 16, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' E-Plus      ', 12, ': ' + strTotalEPlus, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                    var strTotalVoucher = SetPrefixPostfix(AccountingFormat(dblVoucher), intFAFCTotalLenght - 16, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Voucher     ', 12, ': ' + strTotalVoucher, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                } else {
                                    //May 14 2018 gi comment ang Other Tender kay magconflict sa G-Cash na payment type. pwede ang g-cash inside and outside sa mall

                                    //Feb 8 2020 gibalik ang code para maisa ang pagdisplay sa tanan other types na payment

                                    //var strTotalOther = SetPrefixPostfix(AccountingFormat(0), intFAFCTotalLenght - 16, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Other Tender', 12, ': ' + strTotalOther, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                    if (!CBool(sessionStorage.PrintingOfflineXReadData)) {
                                        if (vDataDataSalesReportOtherPaymentInfo != undefined) {
                                            if (vDataDataSalesReportOtherPaymentInfo.Rows.length > 0) {
                                                for (var otherpaymentcount = 0; otherpaymentcount <= vDataDataSalesReportOtherPaymentInfo.Rows.length - 1; otherpaymentcount++) {

                                                    var strTotalOther = SetPrefixPostfix(AccountingFormat(vDataDataSalesReportOtherPaymentInfo.Rows[otherpaymentcount].TotalAmount), intFAFCTotalLenght - 16, ' ', 0);
                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' ' + vDataDataSalesReportOtherPaymentInfo.Rows[otherpaymentcount].PaymentType, 12, ': ' + strTotalOther, intFAFCTotalLenght, '', 0, 1);
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                                }
                                            }
                                        }
                                    } else {
                                        if (vDataDataSalesReportOtherPaymentInfo != undefined) {
                                            var LastOtherPaymentTypesTotalData = JSON.parse(vDataDataSalesReportOtherPaymentInfo);

                                            for (var otherpaymentcount = 0; otherpaymentcount <= Object.keys(LastOtherPaymentTypesTotalData).length - 1; otherpaymentcount++) {
                                                var strTotalOther = SetPrefixPostfix(AccountingFormat(LastOtherPaymentTypesTotalData[Object.keys(LastOtherPaymentTypesTotalData)[otherpaymentcount]]), intFAFCTotalLenght - 16, ' ', 0);
                                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '    ' + Object.keys(LastOtherPaymentTypesTotalData)[otherpaymentcount], 12, ': ' + strTotalOther, intFAFCTotalLenght, '', 0, 1);
                                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                            }
                                        }
                                    }
                                }

                                //Feb 8 2020 gicomment ni para maisa ang pagdisplay sa tanan other types na payment
                                //if (dblGCash > 0) {
                                //    var strTotalGCash = SetPrefixPostfix(AccountingFormat(dblGCash), intFAFCTotalLenght - 16, ' ', 0);
                                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' GCash       ', 12, ': ' + strTotalGCash, intFAFCTotalLenght, '', 0, 1);
                                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //}

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                var strTotalClassPayment = SetPrefixPostfix(AccountingFormat(dblTotalPayment), intFAFCTotalLenght - 20, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Inventory of Payments', 28, '', intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' of Payments', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '', 0, '', 0, strTotalClassPayment, intFAFCTotalLenght, 1);
                                ReadyTextToPrint(strPrint, 3, 2, 2, 0, 1, 1, 0, 0, 0);

                                var dblTotalOverPayment = dblTotalPayment - dblNetSales;
                                //////                                                    var strTotalOverPayment = SetPrefixPostfix(AccountingFormat((dblTotalOverPayment < 0 ? (dblTotalOverPayment * -1) : dblTotalOverPayment)), intFAFCTotalLenght - 25, ' ', 0);
                                var strTotalOverPayment = SetPrefixPostfix(AccountingFormat(dblTotalOverPayment), intFAFCTotalLenght - 25, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Excess of Inventory of', 23, strTotalOverPayment, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Payments vs Net Sales', intFAFCTotalLenght, '', 0, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //END Sales Grouped By Mode of Payment

                                //Inventory of Payments Audit
                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, "Inventory of Payments Audit", intFAFCTotalLenght, '', 0, '', 0, 0);
                                //                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Net Sales Adjusted', 19, strTotalSales, intFAFCTotalLenght, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' for Exchanges', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Inventory', 16, strTotalClassPayment, intFAFCTotalLenght, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' of Payments', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                var dblNetSalesAdjustedLessTotalInvPayment = (dblNetSales + dblAdjSRV) - dblTotalPayment + dblCharges;
                                var strTotalNetSalesAdjustedLessTotalInvPayment = SetPrefixPostfix(AccountingFormat(dblNetSalesAdjustedLessTotalInvPayment), intFAFCTotalLenght - 13, ' ', 0);
                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, (dblNetSalesAdjustedLessTotalInvPayment < 0 ? ' Shortage' : ' Overage '), 9, ': ' + strTotalNetSalesAdjustedLessTotalInvPayment, intFAFCTotalLenght, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //END Inventory of Payments Audit

                                //Sales Grouped by VAT TYPE----------------------------
                                if ((strFrom == strTo && $("#selPOSSalesReportRange").val() == "Day") ||
                                $("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "Sales Grouped by VAT Type", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                    var dblTotalVATSalesPercent = (dblVAT / dblTotalVAT) * 100;
                                    var dblTotalVATAmountPercent = (dblVATAmount / dblTotalVAT) * 100;
                                    var dblTotalNonVATPercent = (dblNonVAT / dblTotalVAT) * 100;
                                    var dblTotalVATExemptPercent = (dblVATExempt / dblTotalVAT) * 100;
                                    var dblTotalZeroRatedVATPercent = (dblZeroRatedVAT / dblTotalVAT) * 100;
                                    var dblTotalVATPercent = dblTotalVATSalesPercent + dblTotalVATAmountPercent + dblTotalNonVATPercent + dblTotalVATExemptPercent + dblTotalZeroRatedVATPercent;

                                    var strTotalVATSales = SetPrefixPostfix(AccountingFormat(dblVAT), 14, ' ', 0);
                                    var strTotalVATAmount = SetPrefixPostfix(AccountingFormat(dblVATAmount), 14, ' ', 0);
                                    var strTotalNonVAT = SetPrefixPostfix(AccountingFormat(dblNonVAT), 14, ' ', 0);
                                    var strTotalVATExempt = SetPrefixPostfix(AccountingFormat(dblVATExempt), 14, ' ', 0);
                                    var strTotalZeroRatedVAT = SetPrefixPostfix(AccountingFormat(dblZeroRatedVAT), 14, ' ', 0);
                                    var strTotalAllVAT = SetPrefixPostfix(AccountingFormat(dblTotalVAT), 14, ' ', 0);

                                    var strTotalVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATSalesPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalVATAmountPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATAmountPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalNonVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalNonVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalVATExemptPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATExemptPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalZeroRatedVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalZeroRatedVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalAllVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);

                                    if (IsVAT != undefined) {
                                        if (!CBool(IsVAT)) {
                                            strTotalVATSales = SetPrefixPostfix(AccountingFormat(0), 14, ' ', 0);
                                            strTotalVATAmount = SetPrefixPostfix(AccountingFormat(0), 14, ' ', 0);
                                            strTotalNonVAT = SetPrefixPostfix(AccountingFormat(CDec2(dblVAT) + CDec2(dblVATAmount)), 14, ' ', 0);

                                            strTotalVATPercent = SetPrefixPostfix(AccountingFormat(0) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                            strTotalVATAmountPercent = SetPrefixPostfix(AccountingFormat(0) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                            strTotalNonVATPercent = SetPrefixPostfix(AccountingFormat(100) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                        }
                                    }

                                    strPrint = ' ';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-able Sales', 14, strTotalVATSales, 30, strTotalVATPercent, intFAFCTotalLenght, 1);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT Amount', 14, strTotalVATAmount, 30, strTotalVATAmountPercent, intFAFCTotalLenght, 1);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Non-VAT', 14, strTotalNonVAT, 30, strTotalNonVATPercent, intFAFCTotalLenght, 1);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-Exempt', 14, strTotalVATExempt, 30, strTotalVATExemptPercent, intFAFCTotalLenght, 1);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Zero-Rated VAT', 14, strTotalZeroRatedVAT, 30, strTotalZeroRatedVATPercent, intFAFCTotalLenght, 1);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '              ', 14, strTotalAllVAT, 30, strTotalAllVATPercent, intFAFCTotalLenght, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                }
                                //End Sales Grouped by VAT TYPE----------------------------

                                //Get Sales by Cashier
                                if ((strFrom == strTo && $("#selPOSSalesReportRange").val() == "Day") ||
                                                $("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                    if (!IsPrintingOfflineData) {
                                        if (vDataSalesByCashier.Rows.length > 0) {

                                            strPrint = '';
                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, "Sales Grouped by Cashier", intFAFCTotalLenght, '', 0, '', 0, 0);
                                            strPrint = strPrint + strTemp;

                                            ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                            ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                            //Get Total Sales
                                            var dblAllTotalSales = 0;
                                            for (var p = 0; p <= vDataSalesByCashier.Rows.length - 1; p++) {
                                                dblAllTotalSales = dblAllTotalSales + parseFloat(vDataSalesByCashier.Rows[p].TotalSales);
                                            };

                                            var dblAllTotalSalesPercent = 0;

                                            strPrint = '';

                                            for (var p = 0; p <= vDataSalesByCashier.Rows.length - 1; p++) {

                                                var strTotalSales = SetPrefixPostfix(AccountingFormat((vDataSalesByCashier.Rows[p].TotalSales || 0)), 14, ' ', 0);
                                                var dblTotalSalesPercent = (parseFloat(vDataSalesByCashier.Rows[p].TotalSales || 0) / dblAllTotalSales) * 100;
                                                var strTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                                dblAllTotalSalesPercent = dblAllTotalSalesPercent + dblTotalSalesPercent;

                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, vDataSalesByCashier.Rows[p].CashierName, 14, strTotalSales, 30, strTotalSalesPercent, intFAFCTotalLenght, 0);
                                                strPrint = strPrint + strTemp;

                                            };

                                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                            var strAllTotalSales = SetPrefixPostfix(AccountingFormat(dblAllTotalSales), 14, ' ', 0);
                                            var strAllTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblAllTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                            strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                            strPrint = FormatTextToPrint(intFAFCTotalLenght, '              ', 15, strAllTotalSales, 31, strAllTotalSalesPercent, intFAFCTotalLenght, 1);
                                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                        }
                                    }
                                }

                                //Get Sales by Machine
                                if ((strFrom == strTo && $("#selPOSSalesReportRange").val() == "Day") ||
                                            $("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                    if (!IsPrintingOfflineData) {
                                        if (vDataSalesByMachine.Rows.length > 0) {

                                            strPrint = '';
                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, "Sales Grouped by Machine (MIN)", intFAFCTotalLenght, '', 0, '', 0, 0);
                                            strPrint = strPrint + strTemp;

                                            ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                            ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                            //Get Total Sales
                                            dblAllTotalSales = 0;
                                            for (var p = 0; p <= vDataSalesByMachine.Rows.length - 1; p++) {
                                                dblAllTotalSales = dblAllTotalSales + parseFloat(vDataSalesByMachine.Rows[p].TotalSales);
                                            };

                                            dblAllTotalSalesPercent = 0;

                                            strPrint = '';
                                            for (var p = 0; p <= vDataSalesByMachine.Rows.length - 1; p++) {

                                                var strTotalSales = SetPrefixPostfix(AccountingFormat((vDataSalesByMachine.Rows[p].TotalSales || 0)), 14, ' ', 0);
                                                var dblTotalSalesPercent = (parseFloat(vDataSalesByMachine.Rows[p].TotalSales || 0) / dblAllTotalSales) * 100;
                                                var strTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                                dblAllTotalSalesPercent = dblAllTotalSalesPercent + dblTotalSalesPercent;

                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, vDataSalesByMachine.Rows[p].DeviceName, 14, strTotalSales, 30, strTotalSalesPercent, intFAFCTotalLenght, 0);
                                                strPrint = strPrint + strTemp;

                                            };

                                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                            strAllTotalSales = SetPrefixPostfix(AccountingFormat(dblAllTotalSales), 14, ' ', 0);
                                            strAllTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblAllTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                            strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                            strPrint = FormatTextToPrint(intFAFCTotalLenght, '              ', 15, strAllTotalSales, 31, strAllTotalSalesPercent, intFAFCTotalLenght, 1);
                                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                        }
                                    }
                                }

                                //Get Sales by Cashier and Machine
                                if ((strFrom == strTo && $("#selPOSSalesReportRange").val() == "Day") ||
                                            $("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                    if (!IsPrintingOfflineData) {
                                        if (vDataSalesByCashierMachine.Rows.length > 0) {

                                            strPrint = '';
                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, "Sales Grouped by Cashier & Machine (MIN)", intFAFCTotalLenght, '', 0, '', 0, 0);
                                            strPrint = strPrint + strTemp;

                                            ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                            ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                            //Get Total Sales
                                            dblAllTotalSales = 0;
                                            for (var p = 0; p <= vDataSalesByCashierMachine.Rows.length - 1; p++) {
                                                dblAllTotalSales = dblAllTotalSales + parseFloat(vDataSalesByCashierMachine.Rows[p].TotalSales);
                                            };

                                            dblAllTotalSalesPercent = 0;

                                            var strCashierName = "";
                                            var strDeviceName = "";
                                            var dblDeviceSales = 0;

                                            dblAllTotalSalesPercent = 0;

                                            strPrint = '';
                                            for (var p = 0; p <= vDataSalesByCashierMachine.Rows.length - 1; p++) {

                                                if (strCashierName != vDataSalesByCashierMachine.Rows[p].CashierName) {
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, vDataSalesByCashierMachine.Rows[p].CashierName, intFAFCTotalLenght, '', 0, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    strCashierName = vDataSalesByCashierMachine.Rows[p].CashierName;
                                                }

                                                dblDeviceSales = parseFloat(vDataSalesByCashierMachine.Rows[p].TotalSales || 0);
                                                strTotalSales = SetPrefixPostfix(AccountingFormat(dblDeviceSales), 14, ' ', 0);
                                                dblTotalSalesPercent = (dblDeviceSales / dblAllTotalSales) * 100;
                                                strTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                                dblAllTotalSalesPercent = dblAllTotalSalesPercent + dblTotalSalesPercent;

                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, '     ' + vDataSalesByCashierMachine.Rows[p].DeviceName, 14, strTotalSales, 30, strTotalSalesPercent, intFAFCTotalLenght, 0);
                                                strPrint = strPrint + strTemp;

                                            };

                                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                            strAllTotalSales = SetPrefixPostfix(AccountingFormat(dblAllTotalSales), 14, ' ', 0);
                                            strAllTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblAllTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                            strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                            strPrint = FormatTextToPrint(intFAFCTotalLenght, '              ', 15, strAllTotalSales, 31, strAllTotalSalesPercent, intFAFCTotalLenght, 1);
                                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                        }
                                    }
                                }

                                if ($("#selPOSSalesReportRange").val() == "Day" ||
                                    $("#selPOSSalesReportRange").val() == "Today" ||
                                    $("#selPOSSalesReportRange").val() == "Yesterday") {

                                    //Other Informatoin
                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "Other Information", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                    var strTransCount = SetPrefixPostfix(intTransCount + '', intFAFCTotalLenght - 21, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Trans. Count', 19, strTransCount, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    var strSalesTransCount = SetPrefixPostfix(intTransCount + '', intFAFCTotalLenght - 21, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Sales Trans. Count', 19, strSalesTransCount, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    var strSalesItemCount = SetPrefixPostfix(intSalesItemCount + '', intFAFCTotalLenght - 24, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Sales Line Item Count', 22, strSalesItemCount, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    var strSRVTransCount = SetPrefixPostfix(intSRVTransCount + '', intFAFCTotalLenght - 19, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SRV Trans. Count', 17, strSRVTransCount, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    var strSRVItemCount = SetPrefixPostfix(intSRVItemCount + '', intFAFCTotalLenght - 22, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SRV Line Item Count', 20, strSRVItemCount, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    var strRefTransCount = SetPrefixPostfix(intRefTransCount + '', intFAFCTotalLenght - 22, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Refund Trans. Count', 20, strRefTransCount, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    var strRefItemCount = SetPrefixPostfix(intRefItemCount + '', intFAFCTotalLenght - 25, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Refund Line Item Count', 23, strRefItemCount, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    var strCustomerCount = SetPrefixPostfix(OffLineTotalNumberOfCustomers + '', intFAFCTotalLenght - 17, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Total Customer', 15, strCustomerCount, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                    //END Other Informatoin

                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Printed by ', 15, ': ' + (vDataHeader.Rows[0].PrintedBy || ''), intFAFCTotalLenght, '', 0, 1);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataHeader.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);

                                    PrintText(strPrinterIP);
                                    ClearCommandBuffer();

                                } else {
                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Printed by ', 15, ': ' + (vDataHeader.Rows[0].PrintedBy || ''), intFAFCTotalLenght, '', 0, 1);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataHeader.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);

                                    PrintText(strPrinterIP);
                                    ClearCommandBuffer();
                                }

                            } else {
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);
                                PrintText(strPrinterIP);
                                ClearCommandBuffer();
                            }

                        }


                    }
                    else {
                        MessageBox("No available printer for this branch. Please contact your system administrator.",
                            "Printer",
                            [
                                {
                                    Text: 'Ok'
                                }
                            ]
                    );
                    }

                }

                function PrintAyalaConsolidatedReport(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits) {

                    var dblTotalSales = 0;
                    var dblTotalSalesDiscount = 0;
                    var dblAdjRefund = 0;
                    var dblAdjSRV = 0;
                    var dblNetSales = 0;

                    var dblCash = 0;
                    var dblCheck = 0;
                    var dblDebitCard = 0;
                    var dblCreditCard = 0;
                    var dblSRV = 0;
                    var dblOtherPaymentType = 0;
                    var dblCharges = 0;
                    var dblSMAC = 0;
                    var dblGCash = 0;
                    var dblEPlus = 0;
                    var dblVoucher = 0;
                    var dblTotalPayment = 0;

                    var dblVAT = 0;
                    var dblVATAmount = 0;
                    var dblNonVAT = 0;
                    var dblVATExempt = 0;
                    var dblZeroRatedVAT = 0;
                    var dblTotalVAT = 0;

                    var intTransCount = 0;
                    var intSalesTransCount = 0;
                    var intSalesItemCount = 0;
                    var intSRVTransCount = 0;
                    var intSRVItemCount = 0;
                    var intRefTransCount = 0;
                    var intRefItemCount = 0;

                    if (vDataPrint.Rows.length > 0) {

                        //var vDataPrint = SalesPayments_offline().getOrgFOWSTPrinterIP();

                        strPrinterIP = vDataPrint.Rows[0].PrinterIPAddress;

                        sessionStorage['ReportDate'] = strFrom;

                        //New set of text to print
                        NewPrintBuilder();

                        if (vDataHeader.Rows[0].OrgBusinessTradeName != undefined) {
                            //Get Header
                            strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].OrgBusinessTradeName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                            strPrint = '';
                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].SubscriberOrganizationName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            //strPrint = strPrint + strTemp;

                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].OrgBranchName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            //strPrint = strPrint + strTemp;

                            strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].OrgCompleteAddress || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            strPrint = strPrint + strTemp;

                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'TIN ' + $.trim((vDataHeader.Rows[0].OrgTIN || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            strPrint = strPrint + strTemp;
                            
                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Machine Ident No. ' + $.trim(('123456' || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            strPrint = strPrint + strTemp;

                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'PTUN ' + $.trim(('123456' || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            strPrint = strPrint + strTemp;

                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'BIR Accredit No ' + $.trim(('13200516111400019366756' || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            strPrint = strPrint + strTemp;

                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Machine Ident No.', 20, ' ', 1), 19, ': ' + vDataHeader.Rows[0].MIN + ' \n', intFAFCTotalLenght, '', 0, 0);
                            //strPrint = strPrint + strTemp;

                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('PTUN ', 20, ' ', 1), 19, ': ' + $.trim((vDataHeader.Rows[0].POSPermitNo || '')), intFAFCTotalLenght, '', 0, 0);
                            //strPrint = strPrint + strTemp;
                            
                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('BIR Accredit No', 20, ' ', 1), 19, ': 13200516111400019366756', intFAFCTotalLenght, '', 0, 0);
                            //strPrint = strPrint + strTemp;

                            strPrint = strPrint + '\n';

                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);
                        }

                        for (var p = 0; p <= vDataInfo.Rows.length - 1; p++) {

                            dblTotalSales = dblTotalSales + parseFloat(vDataInfo.Rows[p].CurGrossSales || 0);
                            dblTotalSalesDiscount = dblTotalSalesDiscount + parseFloat(vDataInfo.Rows[p].CurSalesDiscount || 0) + parseFloat(vDataInfo.Rows[p].CurSCDiscount || 0);
                            dblAdjRefund = dblAdjRefund + parseFloat(vDataInfo.Rows[p].CurSalesReturnRefund || 0);
                            dblAdjSRV = dblAdjSRV + parseFloat(vDataInfo.Rows[p].CurSalesReturnExchange || 0);

                            dblCash = dblCash + parseFloat(vDataInfo.Rows[p].CashAmount || 0);
                            dblCheck = dblCheck + parseFloat(vDataInfo.Rows[p].CheckPaymentAmount || 0);
                            dblDebitCard = dblDebitCard + parseFloat(vDataInfo.Rows[p].DebitCardAmount || 0);
                            dblCreditCard = dblCreditCard + parseFloat(vDataInfo.Rows[p].CreditCardAmount || 0);
                            dblSRV = dblSRV + parseFloat(vDataInfo.Rows[p].SRVAmount || 0);
                            dblOtherPaymentType = dblOtherPaymentType + parseFloat(vDataInfo.Rows[p].OtherTypeAmount || 0);
                            dblCharges = dblCharges + parseFloat(vDataInfo.Rows[p].ChargeAmount || 0);
                            dblSMAC = dblSMAC + parseFloat(vDataInfo.Rows[p].SMACAmount || 0);
                            dblGCash = dblGCash + parseFloat(vDataInfo.Rows[p].GCashAmount || 0);
                            dblEPlus = dblEPlus + parseFloat(vDataInfo.Rows[p].EPlusAmount || 0);
                            dblVoucher = dblVoucher + parseFloat(vDataInfo.Rows[p].VoucherAmount || 0);

                            dblVAT = dblVAT + parseFloat(vDataInfo.Rows[p].CurTotalVatablePortionOfSCDTransaction || 0) + parseFloat(vDataInfo.Rows[p].CurTotalVATSales || 0) - parseFloat(vDataInfo.Rows[p].CurTotalVATSalesSalesReturn || 0);
                            dblVATAmount = dblVATAmount + parseFloat(vDataInfo.Rows[p].CurTotalVatAmountPortionOfSCDTransaction || 0) + parseFloat(vDataInfo.Rows[p].CurTotalVATAmount || 0) - parseFloat(vDataInfo.Rows[p].CurTotalVATAmountSalesReturn || 0);
                            dblNonVAT = dblNonVAT + parseFloat(vDataInfo.Rows[p].CurNoTAXSales || 0);
                            dblVATExempt = dblVATExempt + parseFloat(vDataInfo.Rows[p].CurVATExemptSales || 0);
                            dblZeroRatedVAT = dblZeroRatedVAT + parseFloat(vDataInfo.Rows[p].CurZeroRatedVATSales || 0);

                            dblNetSales = dblTotalSales - dblTotalSalesDiscount - dblAdjRefund - dblAdjSRV + dblCharges;

                            intTransCount = intTransCount + parseFloat(vDataInfo.Rows[p].CurTransCountInv || 0) + parseFloat(vDataInfo.Rows[p].CurTransCountMemoReturnExchange || 0) + parseFloat(vDataInfo.Rows[p].CurTransCountMemoRefund || 0);
                            intSalesTransCount = intSalesTransCount + parseFloat(vDataInfo.Rows[p].CurTransCountInv || 0);
                            intSalesItemCount = intSalesItemCount + parseFloat(vDataInfo.Rows[p].CurTransInvItemCount || 0);
                            intSRVTransCount = intSRVTransCount + parseFloat(vDataInfo.Rows[p].CurTransCountMemoReturnExchange || 0);
                            intSRVItemCount = intSRVItemCount + parseFloat(vDataInfo.Rows[p].CurSalesReturnExchangeItemCount || 0);
                            intRefTransCount = intRefTransCount + parseFloat(vDataInfo.Rows[p].CurTransCountMemoRefund || 0);
                            intRefItemCount = intRefItemCount + parseFloat(vDataInfo.Rows[p].CurSalesRefundItemCount || 0);
                        };

                        strPrint = '';
                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'CONSOLIDATED REPORT Z-READ', intFAFCTotalLenght, '', 0, '', 0, 1);                        
                        strPrint = strPrint + strTemp;
                        ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                        strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                        var strTotalSales = SetPrefixPostfix('QTY/AMOUNT', intFAFCTotalLenght - 16, ' ', 0);
                        strPrint = FormatTextToPrint(intFAFCTotalLenght, ' DESCRIPTION ', 14, strTotalSales, intFAFCTotalLenght, '', 0, 1);
                        ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                        strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                        var strTotalSales = SetPrefixPostfix(AccountingFormat(dblTotalSales), intFAFCTotalLenght - 16, ' ', 0);
                        strPrint = FormatTextToPrint(intFAFCTotalLenght, ' DAILY SALES ', 14, strTotalSales, intFAFCTotalLenght, '', 0, 1);
                        ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                        var strVATAmount = SetPrefixPostfix(AccountingFormat(dblVATAmount), intFAFCTotalLenght - 16, ' ', 0);
                        strPrint = FormatTextToPrint(intFAFCTotalLenght, ' VAT ', 14, strVATAmount, intFAFCTotalLenght, '', 0, 1);
                        ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                        var strVAT = SetPrefixPostfix(AccountingFormat(dblVAT), intFAFCTotalLenght - 16, ' ', 0);
                        strPrint = FormatTextToPrint(intFAFCTotalLenght, ' VATABLE SALES ', 14, strVAT, intFAFCTotalLenght, '', 0, 1);
                        ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                        var strNonVAT = SetPrefixPostfix(AccountingFormat(dblNonVAT), intFAFCTotalLenght - 21, ' ', 0);
                        strPrint = FormatTextToPrint(intFAFCTotalLenght, ' NON-VATABLE SALES ', 19, strNonVAT, intFAFCTotalLenght, '', 0, 1);
                        ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                        var strTotalSalesDiscount = SetPrefixPostfix(AccountingFormat(dblTotalSalesDiscount), intFAFCTotalLenght - 16, ' ', 0);
                        strPrint = FormatTextToPrint(intFAFCTotalLenght, ' LESS SC DISC ', 14, strTotalSalesDiscount, intFAFCTotalLenght, '', 0, 1);
                        ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                        var strVATExempt = SetPrefixPostfix(AccountingFormat(dblVATExempt), intFAFCTotalLenght - 16, ' ', 0);
                        strPrint = FormatTextToPrint(intFAFCTotalLenght, ' VAT EXEMPT ', 14, strVATExempt, intFAFCTotalLenght, '', 0, 1);
                        ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                        var strZeroRatedVAT = SetPrefixPostfix(AccountingFormat(dblZeroRatedVAT), intFAFCTotalLenght - 16, ' ', 0);
                        strPrint = FormatTextToPrint(intFAFCTotalLenght, ' ZERO RATED ', 14, strZeroRatedVAT, intFAFCTotalLenght, '', 0, 1);
                        ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                        var strNetSales = SetPrefixPostfix(AccountingFormat(dblNetSales + dblAdjSRV), intFAFCTotalLenght - 16, ' ', 0);
                        strPrint = FormatTextToPrint(intFAFCTotalLenght, ' NET SALES ', 14, strNetSales, intFAFCTotalLenght, '', 0, 1);
                        ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                        
                        var strSalesItemCount = SetPrefixPostfix(intSalesItemCount + '', intFAFCTotalLenght - 21, ' ', 0);
                        strPrint = FormatTextToPrint(intFAFCTotalLenght, ' TOTAL QTY. SOLD ', 19, strSalesItemCount, intFAFCTotalLenght, '', 0, 1);
                        ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                        var strSalesTransCount = SetPrefixPostfix(intTransCount + '', intFAFCTotalLenght - 21, ' ', 0);
                        strPrint = FormatTextToPrint(intFAFCTotalLenght, ' TRANSACTION COUNT ', 19, strSalesTransCount, intFAFCTotalLenght, '', 0, 1);                        
                        ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                        
                        strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Date : ' + $.format.date(new Date(vDataInfo.Rows[0].SalesDate), 'MM-dd-yyyy'), intFAFCTotalLenght, '', 0, '', 0, 1);
                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, '' + $.trim(($.format.date(new Date(strFrom), 'MM-dd-yyyy') || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                        strPrint = strTemp;
                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                        ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);

                        PrintText(strPrinterIP);
                        ClearCommandBuffer();
                    }
                }

                function PrintSaleReportNowViaWindowsPrintServer(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits) {

                    if (vDataPrint.Rows.length > 0) {

                        //var vDataPrint = SalesPayments_offline().getOrgFOWSTPrinterIP();

                        strPrinterIP = vDataPrint.Rows[0].PrinterIPAddress;

                        sessionStorage['ReportDate'] = strFrom;

                        //New set of text to print
                        //NewPrintBuilder();

                        if (vDataHeader.Rows[0].OrgBusinessTradeName != undefined) {
                            //Get Header
                            WindowsPrinter($.trim((vDataHeader.Rows[0].OrgBusinessTradeName || '')), [0, 0, 0, 1, 0, 36]);
                            //strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].OrgBusinessTradeName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                            //strPrint = '';
                            WindowsPrinter($.trim((vDataHeader.Rows[0].SubscriberOrganizationName || '')), [0, 0, 0, 1, 0, 36]);
                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].SubscriberOrganizationName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            //strPrint = strPrint + strTemp;

                            WindowsPrinter($.trim((vDataHeader.Rows[0].OrgBranchName || '')), [1, 0, 0, 1, 0, 36]);
                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].OrgBranchName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            //strPrint = strPrint + strTemp;

                            WindowsPrinter($.trim((vDataHeader.Rows[0].OrgCompleteAddress || '')), [1, 0, 0, 1, 0, 36]);
                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].OrgCompleteAddress || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            //strPrint = strPrint + strTemp;

                            WindowsPrinter('TIN ' + $.trim((vDataHeader.Rows[0].OrgTIN || '')), [0, 0, 0, 1, 0, 36]);
                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'TIN ' + $.trim((vDataHeader.Rows[0].OrgTIN || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            //strPrint = strPrint + strTemp;

                            WindowsPrinter('Tel. No.' + $.trim((vDataHeader.Rows[0].OrgTelNo || '')), [1, 0, 0, 1, 0, 36]);
                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Tel. No.' + $.trim((vDataHeader.Rows[0].OrgTelNo || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            //strPrint = strPrint + strTemp;

                            WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);

                            //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);
                        }

                        if (vDataHeader.Rows[0].POSReportType == 0) {

                            if (vDataInfo.Rows.length > 0) {
                                WindowsPrinter((CBool(sessionStorage.PrintingOfflineXReadData) ? 'DRAFT ' : '') + "CASHIER'S", [1, 0, 0, 1, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, (CBool(sessionStorage.PrintingOfflineXReadData) ? 'DRAFT ' : '') + "CASHIER'S", intFAFCTotalLenght, '', 0, '', 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 2, 1, 1, 1, 1, 0, 0, 0);

                                if ($("#selPOSSalesReportRange").val() == "Day") {

                                    //strPrint = '';
                                    WindowsPrinter("SALES AND MONIES REPORT (CSMR)", [1, 0, 0, 1, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, "SALES AND MONIES REPORT (CSMR)", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    WindowsPrinter((strFrom == strTo ? 'Date: ' + strFrom : 'From ' + strFrom + ' To ' + strTo), [1, 0, 0, 1, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? 'Date: ' + strFrom : 'From ' + strFrom + ' To ' + strTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Week") {

                                    //strPrint = '';
                                    WindowsPrinter("Same-Store Comparative Sales Report", [1, 0, 0, 1, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, "Same-Store Comparative Sales Report", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    WindowsPrinter((strFrom == strTo ? 'Week: ' + strFrom : 'From Week ' + strFrom + ' To Week ' + strTo), [1, 0, 0, 1, 0, 36]);

                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? 'Week: ' + strFrom : 'From Week ' + strFrom + ' To Week ' + strTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    WindowsPrinter((vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), [1, 0, 0, 1, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Month") {

                                    //strPrint = '';
                                    WindowsPrinter("Same-Store Comparative Sales Report", [1, 0, 0, 1, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, "Same-Store Comparative Sales Report", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    WindowsPrinter((strFrom == strTo ? 'Month: ' + $.format.date(new Date(strFrom + '/1/2013'), 'MMM') : 'From ' + $.format.date(new Date(strFrom + '/1/2013'), 'MMM') + ' To ' + $.format.date(new Date(strTo + '/1/2013'), 'MMM')), [1, 0, 0, 1, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? 'Month: ' + $.format.date(new Date(strFrom + '/1/2013'), 'MMM') : 'From ' + $.format.date(new Date(strFrom + '/1/2013'), 'MMM') + ' To ' + $.format.date(new Date(strTo + '/1/2013'), 'MMM')), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    WindowsPrinter((vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), [1, 0, 0, 1, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Quarter") {

                                    //strPrint = '';
                                    WindowsPrinter("Same-Store Comparative Sales Report", [1, 0, 0, 1, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, "Same-Store Comparative Sales Report", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    WindowsPrinter((strFrom == strTo ? (strFrom == '1' ? "1st Quarter" : (strFrom == '2' ? "2nd Quarter" : (strFrom == '3' ? "3rd Quarter" : (strFrom == '4' ? "4th Quarter" : "")))) : 'From ' + (strFrom == '1' ? "1st Quarter" : (strFrom == '2' ? "2nd Quarter" : (strFrom == '3' ? "3rd Quarter" : (strFrom == '4' ? "4th Quarter" : "")))) + ' To ' + (strTo == '1' ? "1st Quarter" : (strTo == '2' ? "2nd Quarter" : (strTo == '3' ? "3rd Quarter" : (strTo == '4' ? "4th Quarter" : ""))))), [1, 0, 0, 1, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? (strFrom == '1' ? "1st Quarter" : (strFrom == '2' ? "2nd Quarter" : (strFrom == '3' ? "3rd Quarter" : (strFrom == '4' ? "4th Quarter" : "")))) : 'From ' + (strFrom == '1' ? "1st Quarter" : (strFrom == '2' ? "2nd Quarter" : (strFrom == '3' ? "3rd Quarter" : (strFrom == '4' ? "4th Quarter" : "")))) + ' To ' + (strTo == '1' ? "1st Quarter" : (strTo == '2' ? "2nd Quarter" : (strTo == '3' ? "3rd Quarter" : (strTo == '4' ? "4th Quarter" : ""))))), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Year") {
                                    //strPrint = '';
                                    WindowsPrinter("SALES REPORT ", [1, 0, 0, 1, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, "SALES REPORT", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    WindowsPrinter('Year: ' + strYear, [1, 0, 0, 1, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Year: ' + strYear, intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                    //strPrint = '';
                                    WindowsPrinter("SALES REPORT (X-READ)", [1, 0, 0, 1, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, "SALES REPORT", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    if (IsPrintingOfflineData) {
                                        WindowsPrinter('Date: From: ' + strFrom + ' To: ' + strTo, [1, 0, 0, 1, 0, 36]);
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Date: From: ' + strFrom + ' To: ' + strTo, intFAFCTotalLenght, '', 0, '', 0, 0);
                                        //strPrint = strPrint + strTemp;
                                    } else {
                                        WindowsPrinter('Date : ' + $.format.date(new Date(vDataInfo.Rows[0].SalesDate), 'MM-dd-yyyy'), [1, 0, 0, 1, 0, 36]);
                                        //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Date : ' + $.format.date(new Date(vDataInfo.Rows[0].SalesDate), 'MM-dd-yyyy'), intFAFCTotalLenght, '', 0, '', 0, 0);
                                        //strPrint = strPrint + strTemp;
                                    }

                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                }
                                                                
                                WindowsPrinter('Cashier ' + $.trim((vDataInfo.Rows[0].CashierName || '')), [0, 0, 0, 0, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Cashier', 9, ' : ' + (vDataInfo.Rows[0].CashierName || ''), intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var dblTotalSales = 0;
                                var dblTotalSalesDiscount = 0;
                                var dblTotalVATExemptAmount = 0;
                                var dblAdjRefund = 0;
                                var dblAdjSRV = 0;
                                var dblNetSales = 0;

                                var dblCash = 0;
                                var dblCheck = 0;
                                var dblDebitCard = 0;
                                var dblCreditCard = 0;
                                var dblSRV = 0;
                                var dblOnCredit = 0;
                                var dblOtherPaymentType = 0;
                                var dblCharges = 0;
                                var dblSMAC = 0;
                                var dblGCash = 0;
                                var dblEPlus = 0;
                                var dblVoucher = 0;
                                var dblTotalPayment = 0;

                                var dblVAT = 0;
                                var dblVATAmount = 0;
                                var dblNonVAT = 0;
                                var dblVATExempt = 0;
                                var dblZeroRatedVAT = 0;
                                var dblTotalVAT = 0;
                                var XReadCount = 0;

                                if (!IsPrintingOfflineData) {
                                    for (var p = 0; p <= vDataInfo.Rows.length - 1; p++) {

                                        dblTotalSales = dblTotalSales + parseFloat(vDataInfo.Rows[p].CurGrossSales || 0);
                                        dblTotalSalesDiscount = dblTotalSalesDiscount + parseFloat(vDataInfo.Rows[p].CurSalesDiscount || 0) + parseFloat(vDataInfo.Rows[p].CurSCDiscount || 0);
                                        dblTotalVATExemptAmount = dblTotalVATExemptAmount + parseFloat(vDataInfo.Rows[p].CurVATExempt || 0);
                                        dblAdjRefund = dblAdjRefund + parseFloat(vDataInfo.Rows[p].CurSalesReturnRefund || 0);
                                        dblAdjSRV = dblAdjSRV + parseFloat(vDataInfo.Rows[p].CurSalesReturnExchange || 0);

                                        dblCash = dblCash + parseFloat(vDataInfo.Rows[p].CashAmount || 0);
                                        dblCheck = dblCheck + parseFloat(vDataInfo.Rows[p].CheckPaymentAmount || 0);
                                        dblDebitCard = dblDebitCard + parseFloat(vDataInfo.Rows[p].DebitCardAmount || 0);
                                        dblCreditCard = dblCreditCard + parseFloat(vDataInfo.Rows[p].CreditCardAmount || 0);
                                        dblSRV = dblSRV + parseFloat(vDataInfo.Rows[p].SRVAmount || 0);
                                        dblOnCredit = dblOnCredit + parseFloat(vDataInfo.Rows[p].OnCreditAmount || 0);
                                        dblOtherPaymentType = dblOtherPaymentType + parseFloat(vDataInfo.Rows[p].OtherTypeAmount || 0);
                                        dblCharges = dblCharges + parseFloat(vDataInfo.Rows[p].ChargeAmount || 0);
                                        dblSMAC = dblSMAC + parseFloat(vDataInfo.Rows[p].SMACAmount || 0);
                                        dblGCash = dblGCash + parseFloat(vDataInfo.Rows[p].GCashAmount || 0);
                                        dblEPlus = dblEPlus + parseFloat(vDataInfo.Rows[p].EPlusAmount || 0);
                                        dblVoucher = dblVoucher + parseFloat(vDataInfo.Rows[p].VoucherAmount || 0);

                                        dblVAT = dblVAT + parseFloat(vDataInfo.Rows[p].CurTotalVatablePortionOfSCDTransaction || 0) + parseFloat(vDataInfo.Rows[p].CurTotalVATSales || 0) - parseFloat(vDataInfo.Rows[p].CurTotalVATSalesSalesReturn || 0);
                                        dblVATAmount = dblVATAmount + parseFloat(vDataInfo.Rows[p].CurTotalVatAmountPortionOfSCDTransaction || 0) + parseFloat(vDataInfo.Rows[p].CurTotalVATAmount || 0) - parseFloat(vDataInfo.Rows[p].CurTotalVATAmountSalesReturn || 0);
                                        dblNonVAT = dblNonVAT + parseFloat(vDataInfo.Rows[p].CurNoTAXSales || 0);
                                        dblVATExempt = dblVATExempt + parseFloat(vDataInfo.Rows[p].CurVATExemptSales || 0);
                                        dblZeroRatedVAT = dblZeroRatedVAT + parseFloat(vDataInfo.Rows[p].CurZeroRatedVATSales || 0);

                                        if (vDataInfo.Rows[p].CSRPrintCount != undefined) {
                                            XReadCount = vDataInfo.Rows[p].CSRPrintCount;
                                        }
                                    };
                                }
                                else {
                                    //if (IsPrintingOfflineData) {
                                    dblTotalSales += OfflineTotalSales;
                                    dblTotalSalesDiscount += OfflineTotalSalesDiscount;
                                    //dblTotalVATExemptAmount += OfflineTotalVATExemptAmount;
                                    dblAdjRefund += OfflineAdjRefund;
                                    dblAdjSRV += OfflineAdjSRV;

                                    dblCash += OfflineCash;
                                    dblCheck += OffLineCheck;
                                    dblDebitCard += OffLineDebitCard;
                                    dblCreditCard += OffLineCreditCard;
                                    dblSRV += OffLineSRV;
                                    dblOtherPaymentType += OffLineOtherPayment;
                                    dblOnCredit += OffLineOnCredit;
                                    dblCharges += OffLineCharges;

                                    dblVAT += OffLineVAT;
                                    dblVATAmount += OffLineVATAmount;
                                    dblNonVAT += OffLineNonVAT;
                                    dblVATExempt += OffLineVATExempt;
                                    dblZeroRatedVAT += OffLineZeroRatedVAT;
                                    dblTotalVATExemptAmount += OffLineForeGoneVAT;
                                }

                                dblCash = dblCash - dblAdjRefund;
                                dblNetSales = dblTotalSales - dblTotalSalesDiscount - dblTotalVATExemptAmount - dblAdjRefund - dblAdjSRV + dblCharges;
                                dblTotalPayment = dblCash + dblCheck + dblDebitCard + dblCreditCard + dblSRV + dblOtherPaymentType + dblOnCredit;
                                dblTotalVAT = dblVAT + dblVATAmount + dblNonVAT + dblVATExempt + dblZeroRatedVAT;

                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                WindowsPrinter(" Gross Sales: ", [0, 0, 0, 0, 0, 36], AccountingFormat(dblTotalSales), [0, 0, 0, 0, 1, 36]);
                                //var strTotalSales = SetPrefixPostfix(AccountingFormat(dblTotalSales), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Gross Sales: ', 14, strTotalSales, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //Sales Discount
                                
                                WindowsPrinter(" Total Sales Discount", [0, 0, 0, 0, 0, 36], AccountingFormat(dblTotalSalesDiscount), [0, 0, 0, 0, 1, 36]);

                                WindowsPrinter(" Total VAT Exempt Tax", [0, 0, 0, 0, 0, 36], AccountingFormat(dblTotalVATExemptAmount), [0, 0, 0, 0, 1, 36]);

                                //var strSalesDiscount = SetPrefixPostfix(AccountingFormat(dblTotalSalesDiscount), intFAFCTotalLenght - 23, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Sales Discount', 21, strSalesDiscount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                //charges
                                if (dblCharges > 0) {
                                    WindowsPrinter(" Total Charges", [0, 0, 0, 0, 0, 36], AccountingFormat(dblCharges), [0, 0, 0, 0, 1, 36]);
                                    //var strTotalCharges = SetPrefixPostfix(AccountingFormat(dblCharges), intFAFCTotalLenght - 23, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Charges', 21, strTotalCharges, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                }



                                //Sales Adjustment
                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                WindowsPrinter(" Summary of Adjustments", [0, 0, 0, 0, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Summary of Adjustments', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter("Total Sales Refund for the  Period", [0, 0, 0, 0, 0, 36], AccountingFormat(dblAdjRefund), [0, 0, 0, 0, 1, 36]);
                                //var strAdjRefund = SetPrefixPostfix(AccountingFormat(dblAdjRefund), intFAFCTotalLenght - 29, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Sales Refund for the  Period', 27, strAdjRefund, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                //WindowsPrinter(" Total Sales Returns Subject to Merchadise Exchange           (SRV Issued)", [0, 0, 0, 0, 0, 36], AccountingFormat(dblAdjSRV), [0, 0, 0, 0, 1, 36]);
                                WindowsPrinter(" Total Sales Returns Subject to Merchadise Exchange (SRV Issued)", [1, 16, 0, 0, 0, 36], ': ', [0, 10, 0, 0, 0, 36], AccountingFormat(dblAdjSRV), [0, 10, 0, 0, 1, 36]);
                                //var strAdjSRV = SetPrefixPostfix(AccountingFormat(dblAdjSRV), intFAFCTotalLenght - 32, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Sales Returns Subject to Merchadise Exchange           (SRV Issued)', 30, strAdjSRV, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Net of Adjustments: ", [0, 0, 0, 0, 0, 36], AccountingFormat(parseFloat(dblAdjRefund) + parseFloat(dblAdjSRV)), [0, 0, 0, 0, 1, 36]);
                                //var strNetSalesAdj = SetPrefixPostfix(AccountingFormat(parseFloat(dblAdjRefund) + parseFloat(dblAdjSRV)), intFAFCTotalLenght - 23, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Net of Adjustments: ', 21, strNetSalesAdj, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter("________________________________________", [0, 0, 0, 0, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                WindowsPrinter(" Net Sales: ", [0, 0, 0, 0, 0, 36], AccountingFormat(dblNetSales), [0, 0, 0, 0, 1, 36]);
                                //var strNetSales = SetPrefixPostfix(AccountingFormat(dblNetSales), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Net Sales: ', 12, '  ' + strNetSales, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Add : Issued SRV:", [0, 0, 0, 0, 0, 36], AccountingFormat(dblAdjSRV), [0, 0, 0, 0, 1, 36]);
                                //var strIssuedSRV = SetPrefixPostfix(AccountingFormat(dblAdjSRV), intFAFCTotalLenght - 20, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Add : Issued SRV:', 18, strIssuedSRV, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter("________________________________________", [0, 0, 0, 0, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                WindowsPrinter(" Net Sales Adjusted for Exchanges", [0, 0, 0, 0, 0, 36], AccountingFormat(dblNetSales + dblAdjSRV), [0, 0, 0, 0, 1, 36]);
                                //var strTotalSales = SetPrefixPostfix(AccountingFormat(dblNetSales + dblAdjSRV), intFAFCTotalLenght - 21, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Net Sales Adjusted for Exchanges', 33, '', 0, '', 0, 1);
                                ////                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '', 0, '', 0, strTotalSales, intFAFCTotalLenght, 1);
                                //ReadyTextToPrint(strPrint, 3, 2, 2, 0, 1, 1, 0, 0, 0);

                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' for Exchanges', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //END Sales Adjustment

                                //Sales Grouped By Mode of Payment
                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                WindowsPrinter("Inventory of Payments", [0, 0, 0, 0, 0, 36]);                                
                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, "Inventory of Payments", intFAFCTotalLenght, '', 0, '', 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                WindowsPrinter(" Cash       ", [0, 0, 0, 0, 0, 36], ': ' + AccountingFormat(dblCash), [0, 0, 0, 0, 1, 36]);

                                //var strTotalCash = SetPrefixPostfix(AccountingFormat(dblCash), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Cash       ', 12, ': ' + strTotalCash, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Check      ", [0, 0, 0, 0, 0, 36], ': ' + AccountingFormat(dblCheck), [0, 0, 0, 0, 1, 36]);

                                //var strTotalCheck = SetPrefixPostfix(AccountingFormat(dblCheck), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Check      ', 12, ': ' + strTotalCheck, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Debit Card ", [0, 0, 0, 0, 0, 36], ': ' + AccountingFormat(dblDebitCard), [0, 0, 0, 0, 1, 36]);

                                //var strTotalDebitCard = SetPrefixPostfix(AccountingFormat(dblDebitCard), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Debit Card ', 12, ': ' + strTotalDebitCard, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Credit Card", [0, 0, 0, 0, 0, 36], ': ' + AccountingFormat(dblCreditCard), [0, 0, 0, 0, 1, 36]);

                                //var strTotalCreditCard = SetPrefixPostfix(AccountingFormat(dblCreditCard), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Credit Card', 12, ': ' + strTotalCreditCard, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" SRV        ", [0, 0, 0, 0, 0, 36], ': ' + AccountingFormat(dblSRV), [0, 0, 0, 0, 1, 36]);

                                //var strTotalSRV = SetPrefixPostfix(AccountingFormat(dblSRV), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SRV        ', 12, ': ' + strTotalSRV, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                if (IsFowstInSMMall) {
                                    WindowsPrinter(" SMAC        ", [0, 0, 0, 0, 0, 36], ': ' + AccountingFormat(dblSMAC), [0, 0, 0, 0, 1, 36]);
                                    //var strTotalSMAC = SetPrefixPostfix(AccountingFormat(dblSMAC), intFAFCTotalLenght - 16, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SMAC        ', 12, ': ' + strTotalSMAC, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                    WindowsPrinter(" E-Plus      ", [0, 0, 0, 0, 0, 36], ': ' + AccountingFormat(dblEPlus), [0, 0, 0, 0, 1, 36]);
                                    //var strTotalEPlus = SetPrefixPostfix(AccountingFormat(dblEPlus), intFAFCTotalLenght - 16, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' E-Plus      ', 12, ': ' + strTotalEPlus, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                    WindowsPrinter(" Voucher     ", [0, 0, 0, 0, 0, 36], ': ' + AccountingFormat(dblVoucher), [0, 0, 0, 0, 1, 36]);
                                    //var strTotalVoucher = SetPrefixPostfix(AccountingFormat(dblVoucher), intFAFCTotalLenght - 16, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Voucher     ', 12, ': ' + strTotalVoucher, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);


                                } else {
                                    WindowsPrinter(" On Credit        ", [0, 0, 0, 0, 0, 36], ': ' + AccountingFormat(dblOnCredit), [0, 0, 0, 0, 1, 36]);

                                    //May 14 2018 gi comment ang Other Tender kay magconflict sa G-Cash na payment type. pwede ang g-cash inside and outside sa mall

                                    //Feb 8 2020 gibalik ang code para maisa ang pagdisplay sa tanan other types na payment

                                    //WindowsPrinter(" Other Tender", [0, 0, 0, 0, 0, 36], ' ', [0, 0, 0, 0, 1, 36]);
                                    
                                    if (!CBool(sessionStorage.PrintingOfflineXReadData)) {
                                        if (vDataDataSalesReportOtherPaymentInfo != undefined) {
                                            if (vDataDataSalesReportOtherPaymentInfo.Rows.length > 0) {
                                                var mOtherPaymentTypeColumns = Object.entries(vDataDataSalesReportOtherPaymentInfo.Columns);
                                                //i=10 since magsugod ang other payment type sa 
                                                for (var o in vDataDataSalesReportOtherPaymentInfo.Rows) {
                                                    for (var c in vDataDataSalesReportOtherPaymentInfo.Columns) {
                                                        var opname = vDataDataSalesReportOtherPaymentInfo.Columns[c].replace('OtherPayment', '');
                                                        var amt = CDec2(vDataDataSalesReportOtherPaymentInfo.Rows[o][vDataDataSalesReportOtherPaymentInfo.Columns[c]]);

                                                        if (amt == 0) continue;

                                                        if (opname.toLowerCase() == 'potatocornergc') {
                                                            continue;
                                                        } else {
                                                            WindowsPrinter(" " + opname, [0, 0, 0, 0, 0, 36], AccountingFormat(amt), [0, 0, 0, 0, 1, 36]);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }else {

                                        if (vDataDataSalesReportOtherPaymentInfo != undefined) {
                                            var LastOtherPaymentTypesTotalData = JSON.parse(vDataDataSalesReportOtherPaymentInfo);

                                            for (var otherpaymentcount = 0; otherpaymentcount <= Object.keys(LastOtherPaymentTypesTotalData).length - 1; otherpaymentcount++) {

                                                WindowsPrinter("     " + Object.keys(LastOtherPaymentTypesTotalData)[otherpaymentcount], [0, 0, 0, 0, 0, 36], AccountingFormat(LastOtherPaymentTypesTotalData[Object.keys(LastOtherPaymentTypesTotalData)[otherpaymentcount]]), [0, 0, 0, 0, 1, 36]);

                                            }
                                        }
                                    }
                                }

                                //Feb 8 2020 gicomment ni para maisa ang pagdisplay sa tanan other types na payment

                                //if (dblGCash > 0) {
                                //    WindowsPrinter(" GCash       ", [0, 0, 0, 0, 0, 36], ': ' + AccountingFormat(dblGCash), [0, 0, 0, 0, 1, 36]);
                                //}

                                WindowsPrinter("________________________________________", [0, 0, 0, 0, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                //WindowsPrinter(" Total Inventory of Payments", [0, 0, 0, 0, 0, 36]);
                                WindowsPrinter(" Total Inventory of Payments", [0, 0, 0, 0, 0, 36], ': ' + AccountingFormat(dblTotalPayment), [0, 0, 0, 0, 1, 36]);
                                //var strTotalClassPayment = SetPrefixPostfix(AccountingFormat(dblTotalPayment), intFAFCTotalLenght - 21, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Inventory of Payments', 28, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //WindowsPrinter(" of Payments", [0, 0, 0, 0, 0, 36]);
                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' of Payments', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //WindowsPrinter(AccountingFormat(dblTotalPayment), [0, 0, 0, 0, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '', 0, '', 0, strTotalClassPayment, intFAFCTotalLenght, 1);
                                //ReadyTextToPrint(strPrint, 3, 2, 2, 0, 1, 1, 0, 0, 0);

                                var dblTotalOverPayment = dblTotalPayment - dblNetSales;
                                //////                                                    var strTotalOverPayment = SetPrefixPostfix(AccountingFormat((dblTotalOverPayment < 0 ? (dblTotalOverPayment * -1) : dblTotalOverPayment)), intFAFCTotalLenght - 25, ' ', 0);
                                //WindowsPrinter(" Excess of Inventory of", [0, 0, 0, 0, 0, 36], ': ' + AccountingFormat(dblTotalOverPayment), [0, 0, 0, 0, 0, 36]);
                                WindowsPrinter(" Excess of Inventory of ", [0, 0, 0, 0, 0, 36], ': ' + AccountingFormat(dblTotalOverPayment), [0, 0, 0, 0, 1, 36]);
                                //var strTotalOverPayment = SetPrefixPostfix(AccountingFormat(dblTotalOverPayment), intFAFCTotalLenght - 25, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Excess of Inventory of', 23, strTotalOverPayment, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Payments vs Net Sales", [0, 0, 0, 0, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Payments vs Net Sales', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //END Sales Grouped By Mode of Payment

                                //Inventory of Payments Audit
                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                WindowsPrinter("Inventory of Payments Audit", [0, 0, 0, 0, 0, 36]);                                
                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, "Inventory of Payments Audit", intFAFCTotalLenght, '', 0, '', 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                WindowsPrinter(" Net Sales Adjusted", [0, 0, 0, 0, 0, 36], ': ' + AccountingFormat(dblNetSales + dblAdjSRV), [0, 0, 0, 0, 1, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '  Net Sales Adjusted', 19, strTotalSales, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" for Exchanges", [0, 0, 0, 0, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' for Exchanges', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                
                                WindowsPrinter("  Total Inventory", [0, 0, 0, 0, 0, 36], ': ' + AccountingFormat(dblTotalPayment), [0, 0, 0, 0, 1, 36]);

                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '  Total Inventory', 19, strTotalClassPayment, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" of Payments", [0, 0, 0, 0, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' of Payments', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter("________________________________________", [0, 0, 0, 0, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                var dblNetSalesAdjustedLessTotalInvPayment = (dblNetSales + dblAdjSRV) - dblTotalPayment + dblCharges;
                                var strTotalNetSalesAdjustedLessTotalInvPayment = SetPrefixPostfix(AccountingFormat(dblNetSalesAdjustedLessTotalInvPayment), intFAFCTotalLenght - 13, ' ', 0);

                                WindowsPrinter((dblNetSalesAdjustedLessTotalInvPayment < 0 ? ' Shortage' : ' Overage '), [0, 0, 0, 0, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, (dblNetSalesAdjustedLessTotalInvPayment < 0 ? ' Shortage' : ' Overage '), 9, ': ' + strTotalNetSalesAdjustedLessTotalInvPayment, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //END Inventory of Payments Audit

                                var TotalTurnOverAndDeposits = 0;

                                WindowsPrinter("Turnovers and Deposits", [0, 0, 0, 0, 0, 36]);
                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, "Turnovers and Deposits", intFAFCTotalLenght, '', 0, '', 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                //strPrint = ' ';
                                WindowsPrinter("Action", [1, 16, 0, 0, 0, 36], "To", [0, 10, 0, 0, 0, 36], "Amount", [0, 10, 0, 0, 1, 36]);                                
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Action', 14, 'To', 30, 'Amount', intFAFCTotalLenght, 1);
                                //strPrint = strPrint + strTemp;
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                //strPrint = ' ';

                                for (var toCounter = 0; toCounter <= vDataTurnOverAndDeposits.Rows.length - 1; toCounter++) {

                                    WindowsPrinter((vDataTurnOverAndDeposits.Rows[toCounter].IsTurnOver == 1 ? 'Turned Over' : 'Deposited'), [1, 16, 0, 0, 0, 36], vDataTurnOverAndDeposits.Rows[toCounter].DepositedTo, [0, 10, 0, 0, 0, 36], vDataTurnOverAndDeposits.Rows[toCounter].TotalAmount, [0, 10, 0, 0, 1, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataTurnOverAndDeposits.Rows[toCounter].IsTurnOver == 1 ? 'Turned Over' : 'Deposited'), 14, vDataTurnOverAndDeposits.Rows[toCounter].DepositedTo, 30, vDataTurnOverAndDeposits.Rows[toCounter].TotalAmount, intFAFCTotalLenght, 1);
                                    //strPrint = strPrint + strTemp;

                                    TotalTurnOverAndDeposits += CDec2(vDataTurnOverAndDeposits.Rows[toCounter].TotalAmount);
                                }

                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter("________________________________________", [0, 0, 0, 0, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                WindowsPrinter("Total Turnovers and Deposits", [0, 0, 0, 0, 0, 36], ': ' + TotalTurnOverAndDeposits, [0, 0, 0, 0, 1, 36]);

                                //strPrint = '';
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Total Turnovers and Deposits', 30, ': ' + TotalTurnOverAndDeposits, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //Sales Grouped by VAT TYPE----------------------------
                                if ((strFrom == strTo && $("#selPOSSalesReportRange").val() == "Day") ||
                                        $("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                    WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                    WindowsPrinter("Sales Grouped by VAT Type", [0, 0, 0, 0, 0, 36]);
                                    WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, "Sales Grouped by VAT Type", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                    //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                    var dblTotalVATSalesPercent = (dblVAT / dblTotalVAT) * 100;
                                    var dblTotalVATAmountPercent = (dblVATAmount / dblTotalVAT) * 100;
                                    var dblTotalNonVATPercent = (dblNonVAT / dblTotalVAT) * 100;
                                    var dblTotalVATExemptPercent = (dblVATExempt / dblTotalVAT) * 100;
                                    var dblTotalZeroRatedVATPercent = (dblZeroRatedVAT / dblTotalVAT) * 100;
                                    var dblTotalVATPercent = dblTotalVATSalesPercent + dblTotalVATAmountPercent + dblTotalNonVATPercent + dblTotalVATExemptPercent + dblTotalZeroRatedVATPercent;


                                    var strTotalVATSales = SetPrefixPostfix(AccountingFormat(dblVAT), 14, ' ', 0);
                                    var strTotalVATAmount = SetPrefixPostfix(AccountingFormat(dblVATAmount), 14, ' ', 0);
                                    var strTotalNonVAT = SetPrefixPostfix(AccountingFormat(dblNonVAT), 14, ' ', 0);
                                    var strTotalVATExempt = SetPrefixPostfix(AccountingFormat(dblVATExempt), 14, ' ', 0);
                                    var strTotalZeroRatedVAT = SetPrefixPostfix(AccountingFormat(dblZeroRatedVAT), 14, ' ', 0);
                                    var strTotalAllVAT = SetPrefixPostfix(AccountingFormat(dblTotalVAT), 14, ' ', 0);

                                    var strTotalVATSalesPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATSalesPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalVATAmountPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATAmountPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalNonVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalNonVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalVATExemptPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATExemptPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalZeroRatedVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalZeroRatedVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalAllVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);

                                    if (IsVAT != undefined) {
                                        if (!CBool(IsVAT)) { 
                                            dblNonVAT = dblVAT + dblVATAmount;
                                            dblVAT = 0;
                                            dblVATAmount = 0;

                                            dblTotalVATSalesPercent = 0;
                                            dblTotalVATAmountPercent = 0;
                                            dblTotalNonVATPercent = 100;
                                        }
                                    }

                                    WindowsPrinter("VAT-able Sales", [1, 16, 0, 0, 0, 36], AccountingFormat(dblVAT), [0, 10, 0, 0, 0, 36], AccountingFormat(dblTotalVATSalesPercent) + '%', [0, 10, 0, 0, 1, 36]);
                                    //strPrint = ' ';
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-able Sales', 14, strTotalVATSales, 30, strTotalVATSalesPercent, intFAFCTotalLenght, 1);
                                    //strPrint = strPrint + strTemp;

                                    WindowsPrinter("VAT-Amount", [1, 16, 0, 0, 0, 36], AccountingFormat(dblVATAmount), [0, 10, 0, 0, 0, 36], AccountingFormat(dblTotalVATAmountPercent) + '%', [0, 10, 0, 0, 1, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-Amount', 14, strTotalVATAmount, 30, strTotalVATAmountPercent, intFAFCTotalLenght, 1);
                                    //strPrint = strPrint + strTemp;

                                    WindowsPrinter("Non-VAT", [1, 16, 0, 0, 0, 36], AccountingFormat(dblNonVAT), [0, 10, 0, 0, 0, 36], AccountingFormat(dblTotalNonVATPercent) + '%', [0, 10, 0, 0, 1, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Non-VAT', 14, strTotalNonVAT, 30, strTotalNonVATPercent, intFAFCTotalLenght, 1);
                                    //strPrint = strPrint + strTemp;

                                    WindowsPrinter("VAT-Exempt", [1, 16, 0, 0, 0, 36], AccountingFormat(dblVATExempt), [0, 10, 0, 0, 0, 36], AccountingFormat(dblTotalVATExemptPercent) + '%', [0, 10, 0, 0, 1, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-Exempt', 14, strTotalVATExempt, 30, strTotalVATExemptPercent, intFAFCTotalLenght, 1);
                                    //strPrint = strPrint + strTemp;

                                    WindowsPrinter("Zero-Rated VAT", [1, 16, 0, 0, 0, 36], AccountingFormat(dblZeroRatedVAT), [0, 10, 0, 0, 0, 36], AccountingFormat(dblTotalZeroRatedVATPercent) + '%', [0, 10, 0, 0, 1, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Zero-Rated VAT', 14, strTotalZeroRatedVAT, 30, strTotalZeroRatedVATPercent, intFAFCTotalLenght, 1);
                                    //strPrint = strPrint + strTemp;

                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    WindowsPrinter("________________________________________", [0, 0, 0, 0, 0, 36]);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    WindowsPrinter("               ", [1, 16, 0, 0, 0, 36], AccountingFormat(dblTotalVAT), [0, 10, 0, 0, 0, 36], AccountingFormat(dblTotalVATPercent) + '%', [0, 10, 0, 0, 1, 36]);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '               ', 14, strTotalAllVAT, 30, strTotalAllVATPercent, intFAFCTotalLenght, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                }
                                //End Sales Grouped by VAT TYPE----------------------------

                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);

                                WindowsPrinter(" Printed by ", [0, 0, 0, 0, 0, 36], ': ' + (vDataHeader.Rows[0].PrintedBy || ''), [0, 00, 0, 0, 1, 36]);

                                WindowsPrinter(" XRead Print Count ", [0, 0, 0, 0, 0, 36], ': ' + XReadCount, [0, 0, 0, 0, 1, 36]);

                                //strPrint = '';
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Printed by ', 15, ': ' + (vDataHeader.Rows[0].PrintedBy || ''), intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;

                                WindowsPrinter(" Date and Time ", [0, 0, 0, 0, 0, 36], ': ' + ($.format.date(new Date(vDataHeader.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), [0, 0, 0, 0, 1, 36]);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataHeader.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;

                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);

                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);

                                WindowsPrinter("address", [0, 0, 0, 0, 0, 0], strPrinterIP, [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);
                                WindowsPrinter("startPrint", [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0])

                                //PrintText(strPrinterIP);
                                //ClearCommandBuffer();

                            } else {
                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                WindowsPrinter("address", [0, 0, 0, 0, 0, 0], strPrinterIP, [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);
                                WindowsPrinter("startPrint", [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0])
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);
                                //PrintText(strPrinterIP);
                                //ClearCommandBuffer();
                            }

                        } else {

                            if (vDataInfo.Rows.length > 0) {

                                //WindowsPrinter("MANAGER'S", [1, 16, 0, 0, 0, 36]);
                                WindowsPrinter("MANAGER'S", [0, 0, 0, 1, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, "MANAGER'S", intFAFCTotalLenght, '', 0, '', 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 2, 1, 1, 1, 1, 0, 0, 0);

                                if ($("#selPOSSalesReportRange").val() == "Day") {

                                    //strPrint = '';
                                    //WindowsPrinter("SALES REPORT", [1, 16, 0, 0, 0, 36]);
                                    WindowsPrinter("SALES REPORT", [0, 0, 0, 1, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, "SALES REPORT", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    //WindowsPrinter((strFrom == strTo ? 'Date: ' + strFrom : 'From ' + strFrom + ' To ' + strTo), [1, 16, 0, 0, 0, 36]);
                                    WindowsPrinter('Date : ' + strFrom, [1, 0, 0, 1, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? 'Date: ' + strFrom : 'From ' + strFrom + ' To ' + strTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Week") {

                                    //strPrint = '';
                                    WindowsPrinter("Same-Store Comparative Sales Report", [1, 16, 0, 0, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, "Same-Store Comparative Sales Report", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    WindowsPrinter("(strFrom == strTo ? 'Week: ' + strFrom : 'From Week ' + strFrom + ' To Week ' + strTo)", [1, 16, 0, 0, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? 'Week: ' + strFrom : 'From Week ' + strFrom + ' To Week ' + strTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    WindowsPrinter((vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), [1, 16, 0, 0, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Month") {

                                    //strPrint = '';
                                    WindowsPrinter("Same-Store Comparative Sales Report", [1, 16, 0, 0, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, "Same-Store Comparative Sales Report", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    WindowsPrinter((strFrom == strTo ? 'Month: ' + $.format.date(new Date(strFrom + '/1/2013'), 'MMM') : 'From ' + $.format.date(new Date(strFrom + '/1/2013'), 'MMM') + ' To ' + $.format.date(new Date(strTo + '/1/2013'), 'MMM')), [1, 16, 0, 0, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? 'Month: ' + $.format.date(new Date(strFrom + '/1/2013'), 'MMM') : 'From ' + $.format.date(new Date(strFrom + '/1/2013'), 'MMM') + ' To ' + $.format.date(new Date(strTo + '/1/2013'), 'MMM')), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    WindowsPrinter((vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), [1, 16, 0, 0, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Quarter") {

                                    //strPrint = '';
                                    WindowsPrinter("Same-Store Comparative Sales Report", [1, 16, 0, 0, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, "Same-Store Comparative Sales Report", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    WindowsPrinter((strFrom == strTo ? (strFrom == '1' ? "1st Quarter" : (strFrom == '2' ? "2nd Quarter" : (strFrom == '3' ? "3rd Quarter" : (strFrom == '4' ? "4th Quarter" : "")))) : 'From ' + (strFrom == '1' ? "1st Quarter" : (strFrom == '2' ? "2nd Quarter" : (strFrom == '3' ? "3rd Quarter" : (strFrom == '4' ? "4th Quarter" : "")))) + ' To ' + (strTo == '1' ? "1st Quarter" : (strTo == '2' ? "2nd Quarter" : (strTo == '3' ? "3rd Quarter" : (strTo == '4' ? "4th Quarter" : ""))))), [1, 16, 0, 0, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? (strFrom == '1' ? "1st Quarter" : (strFrom == '2' ? "2nd Quarter" : (strFrom == '3' ? "3rd Quarter" : (strFrom == '4' ? "4th Quarter" : "")))) : 'From ' + (strFrom == '1' ? "1st Quarter" : (strFrom == '2' ? "2nd Quarter" : (strFrom == '3' ? "3rd Quarter" : (strFrom == '4' ? "4th Quarter" : "")))) + ' To ' + (strTo == '1' ? "1st Quarter" : (strTo == '2' ? "2nd Quarter" : (strTo == '3' ? "3rd Quarter" : (strTo == '4' ? "4th Quarter" : ""))))), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    WindowsPrinter((vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), [1, 16, 0, 0, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Year") {
                                    //strPrint = '';
                                    WindowsPrinter("SALES REPORT", [1, 16, 0, 0, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, "SALES REPORT", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Year: ' + strYear, intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                } else if ($("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {
                                    //strPrint = '';
                                    WindowsPrinter("SALES REPORT", [1, 16, 0, 0, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, "SALES REPORT", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    //WindowsPrinter('Date : ' + $.format.date(new Date(vDataInfo.Rows[0].SalesDate), 'MM-dd-yyyy'), [1, 16, 0, 0, 0, 36], "", [0, 10, 0, 0, 0, 36]);
                                    WindowsPrinter('Date : ' + $.format.date(new Date(vDataInfo.Rows[0].SalesDate), 'MM-dd-yyyy'), [1, 16, 0, 0, 0, 36]);

                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Date : ' + $.format.date(new Date(vDataInfo.Rows[0].SalesDate), 'MM-dd-yyyy'), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                }

                                var dblTotalSales = 0;
                                var dblTotalSalesDiscount = 0;
                                var dblTotalVATExemptAmount = 0;
                                var dblAdjRefund = 0;
                                var dblAdjSRV = 0;
                                var dblNetSales = 0;

                                var dblCash = 0;
                                var dblCheck = 0;
                                var dblDebitCard = 0;
                                var dblCreditCard = 0;
                                var dblSRV = 0;
                                var dblOnCredit = 0;                                
                                var dblOtherPaymentType = 0;
                                var dblCharges = 0;
                                var dblSMAC = 0;
                                var dblGCash = 0;
                                var dblEPlus = 0;
                                var dblVoucher = 0;
                                var dblTotalPayment = 0;

                                var dblVAT = 0;
                                var dblVATAmount = 0;
                                var dblNonVAT = 0;
                                var dblVATExempt = 0;
                                var dblZeroRatedVAT = 0;
                                var dblTotalVAT = 0;

                                var intTransCount = 0;
                                var intSalesTransCount = 0;
                                var intSalesItemCount = 0;
                                var intSRVTransCount = 0;
                                var intSRVItemCount = 0;
                                var intRefTransCount = 0;
                                var intRefItemCount = 0;

                                if (!IsPrintingOfflineData) {
                                    for (var p = 0; p <= vDataInfo.Rows.length - 1; p++) {

                                        dblTotalSales = dblTotalSales + parseFloat(vDataInfo.Rows[p].CurGrossSales || 0);
                                        dblTotalSalesDiscount = dblTotalSalesDiscount + parseFloat(vDataInfo.Rows[p].CurSalesDiscount || 0) + parseFloat(vDataInfo.Rows[p].CurSCDiscount || 0);
                                        dblTotalVATExemptAmount = dblTotalVATExemptAmount + parseFloat(vDataInfo.Rows[p].CurVATExempt || 0);
                                        dblAdjRefund = dblAdjRefund + parseFloat(vDataInfo.Rows[p].CurSalesReturnRefund || 0);
                                        dblAdjSRV = dblAdjSRV + parseFloat(vDataInfo.Rows[p].CurSalesReturnExchange || 0);

                                        dblCash = dblCash + parseFloat(vDataInfo.Rows[p].CashAmount || 0);
                                        dblCheck = dblCheck + parseFloat(vDataInfo.Rows[p].CheckPaymentAmount || 0);
                                        dblDebitCard = dblDebitCard + parseFloat(vDataInfo.Rows[p].DebitCardAmount || 0);
                                        dblCreditCard = dblCreditCard + parseFloat(vDataInfo.Rows[p].CreditCardAmount || 0);
                                        dblSRV = dblSRV + parseFloat(vDataInfo.Rows[p].SRVAmount || 0);
                                        dblOnCredit = dblOnCredit + parseFloat(vDataInfo.Rows[p].OnCreditAmount || 0);
                                        dblOtherPaymentType = dblOtherPaymentType + parseFloat(vDataInfo.Rows[p].OtherTypeAmount || 0);
                                        dblCharges = dblCharges + parseFloat(vDataInfo.Rows[p].ChargeAmount || 0);
                                        dblSMAC = dblSMAC + parseFloat(vDataInfo.Rows[p].SMACAmount || 0);
                                        dblGCash = dblGCash + parseFloat(vDataInfo.Rows[p].GCashAmount || 0);
                                        dblEPlus = dblEPlus + parseFloat(vDataInfo.Rows[p].EPlusAmount || 0);
                                        dblVoucher = dblVoucher + parseFloat(vDataInfo.Rows[p].VoucherAmount || 0);

                                        dblVAT = dblVAT + parseFloat(vDataInfo.Rows[p].CurTotalVatablePortionOfSCDTransaction || 0) + parseFloat(vDataInfo.Rows[p].CurTotalVATSales || 0) - parseFloat(vDataInfo.Rows[p].CurTotalVATSalesSalesReturn || 0);
                                        dblVATAmount = dblVATAmount + parseFloat(vDataInfo.Rows[p].CurTotalVatAmountPortionOfSCDTransaction || 0) + parseFloat(vDataInfo.Rows[p].CurTotalVATAmount || 0) - parseFloat(vDataInfo.Rows[p].CurTotalVATAmountSalesReturn || 0);
                                        dblNonVAT = dblNonVAT + parseFloat(vDataInfo.Rows[p].CurNoTAXSales || 0);
                                        dblVATExempt = dblVATExempt + parseFloat(vDataInfo.Rows[p].CurVATExemptSales || 0);
                                        dblZeroRatedVAT = dblZeroRatedVAT + parseFloat(vDataInfo.Rows[p].CurZeroRatedVATSales || 0);

                                        intTransCount = intTransCount + parseFloat(vDataInfo.Rows[p].CurTransCountInv || 0) + parseFloat(vDataInfo.Rows[p].CurTransCountMemoReturnExchange || 0) + parseFloat(vDataInfo.Rows[p].CurTransCountMemoRefund || 0);
                                        intSalesTransCount = intSalesTransCount + parseFloat(vDataInfo.Rows[p].CurTransCountInv || 0);
                                        intSalesItemCount = intSalesItemCount + parseFloat(vDataInfo.Rows[p].CurTransInvItemCount || 0);
                                        intSRVTransCount = intSRVTransCount + parseFloat(vDataInfo.Rows[p].CurTransCountMemoReturnExchange || 0);
                                        intSRVItemCount = intSRVItemCount + parseFloat(vDataInfo.Rows[p].CurSalesReturnExchangeItemCount || 0);
                                        intRefTransCount = intRefTransCount + parseFloat(vDataInfo.Rows[p].CurTransCountMemoRefund || 0);
                                        intRefItemCount = intRefItemCount + parseFloat(vDataInfo.Rows[p].CurSalesRefundItemCount || 0);

                                    };
                                } else {
                                    dblTotalSales += OfflineTotalSales;
                                    dblTotalSalesDiscount += OfflineTotalSalesDiscount;
                                    //dblTotalVATExemptAmount += OfflineTotalVATExemptAmount;
                                    dblAdjRefund += OfflineAdjRefund;
                                    dblAdjSRV += OfflineAdjSRV;

                                    dblCash += OfflineCash;
                                    dblCheck += OffLineCheck;
                                    dblDebitCard += OffLineDebitCard;
                                    dblCreditCard += OffLineCreditCard;
                                    dblSRV += OffLineSRV;
                                    dblOtherPaymentType += OffLineOtherPayment;
                                    dblOnCredit += OffLineOnCredit;
                                    dblCharges += OffLineCharges;

                                    dblVAT += OffLineVAT;
                                    dblVATAmount += OffLineVATAmount;
                                    dblNonVAT += OffLineNonVAT;
                                    dblVATExempt += OffLineVATExempt;
                                    dblZeroRatedVAT += OffLineZeroRatedVAT;

                                    intTransCount = OfflineSalesTransCount;
                                    intSalesItemCount = OfflineSalesItemCount;
                                }

                                dblCash = dblCash - dblAdjRefund;
                                dblNetSales = dblTotalSales - dblTotalSalesDiscount - dblAdjRefund - dblTotalVATExemptAmount - dblAdjSRV + dblCharges;
                                dblTotalPayment = dblCash + dblCheck + dblDebitCard + dblCreditCard + dblSRV + dblOtherPaymentType + dblOnCredit;
                                dblTotalVAT = dblVAT + dblVATAmount + dblNonVAT + dblVATExempt + dblZeroRatedVAT;

                                //var strTotalSales = SetPrefixPostfix(AccountingFormat(dblTotalSales), intFAFCTotalLenght - 16, ' ', 0);
                                //WindowsPrinter(" Gross Sales: ", [1, 16, 0, 0, 0, 36], AccountingFormat(dblTotalSales), [0, 10, 0, 0, 0, 36]);
                                WindowsPrinter(" Gross Sales: ", [0, 0, 0, 0, 0, 36], AccountingFormat(dblTotalSales), [0, 0, 0, 0, 1, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Gross Sales: ', 14, strTotalSales, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //Sales Discount
                                //var strSalesDiscount = SetPrefixPostfix(AccountingFormat(dblTotalSalesDiscount), intFAFCTotalLenght - 23, ' ', 0);
                                //WindowsPrinter(" Total Sales Discount", [1, 16, 0, 0, 0, 36], AccountingFormat(dblTotalSalesDiscount), [0, 10, 0, 0, 0, 36]);
                                WindowsPrinter(" Total Sales Discount: ", [0, 0, 0, 0, 0, 36], AccountingFormat(dblTotalSalesDiscount), [0, 0, 0, 0, 1, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Sales Discount', 21, strSalesDiscount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Total VAT Exempt Tax: ", [0, 0, 0, 0, 0, 36], AccountingFormat(dblTotalVATExemptAmount), [0, 0, 0, 0, 1, 36]);

                                //charges
                                if (dblCharges > 0) {
                                    //var strTotalCharges = SetPrefixPostfix(AccountingFormat(dblCharges), intFAFCTotalLenght - 23, ' ', 0);
                                    //WindowsPrinter(" Total Charges", [1, 16, 0, 0, 0, 36], AccountingFormat(dblCharges), [0, 10, 0, 0, 0, 36]);
                                    WindowsPrinter(" Total Charges: ", [0, 0, 0, 0, 0, 36], AccountingFormat(dblCharges), [0, 0, 0, 0, 1, 36]);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Charges', 21, strTotalCharges, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                }


                                //Sales Adjustment
                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                //WindowsPrinter(" Summary of Adjustments", [1, 16, 0, 0, 0, 36], AccountingFormat(dblCharges), [0, 10, 0, 0, 0, 36]);
                                WindowsPrinter(" Summary of Adjustments: ", [0, 0, 0, 0, 0, 36], AccountingFormat(dblCharges), [0, 0, 0, 0, 1, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Summary of Adjustments', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //WindowsPrinter(" Total Sales Refund for the  Period", [1, 16, 0, 0, 0, 36], AccountingFormat(dblAdjRefund), [0, 10, 0, 0, 0, 36]);
                                WindowsPrinter(" Total Sales Refund for the  Period: ", [0, 0, 0, 0, 0, 36], AccountingFormat(dblAdjRefund), [0, 0, 0, 0, 1, 36]);
                                //var strAdjRefund = SetPrefixPostfix(AccountingFormat(dblAdjRefund), intFAFCTotalLenght - 29, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Sales Refund for the  Period', 27, strAdjRefund, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                //WindowsPrinter("Total Sales Returns Subject to Merchadise Exchange           (SRV Issued)", [1, 16, 0, 0, 0, 36], AccountingFormat(dblAdjSRV), [0, 10, 0, 0, 0, 36]);
                                WindowsPrinter("Total Sales Returns Subject to Merchadise Exchange           (SRV Issued): ", [0, 0, 0, 0, 0, 36], AccountingFormat(dblAdjSRV), [0, 0, 0, 0, 1, 36]);
                                //var strAdjSRV = SetPrefixPostfix(AccountingFormat(dblAdjSRV), intFAFCTotalLenght - 32, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Sales Returns Subject to Merchadise Exchange           (SRV Issued)', 30, strAdjSRV, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                //WindowsPrinter(" Net of Adjustments: ", [1, 16, 0, 0, 0, 36], AccountingFormat(parseFloat(dblAdjRefund) + parseFloat(dblAdjSRV)), [0, 10, 0, 0, 0, 36]);
                                WindowsPrinter(" Net of Adjustments: ", [0, 0, 0, 0, 0, 36], AccountingFormat(parseFloat(dblAdjRefund) + parseFloat(dblAdjSRV)), [0, 0, 0, 0, 1, 36]);
                                //var strNetSalesAdj = SetPrefixPostfix(AccountingFormat(parseFloat(dblAdjRefund) + parseFloat(dblAdjSRV)), intFAFCTotalLenght - 23, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Net of Adjustments: ', 21, strNetSalesAdj, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //WindowsPrinter("________________________________________", [1, 16, 0, 0, 0, 36]);
                                WindowsPrinter("________________________________________", [0, 0, 0, 0, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                //WindowsPrinter(" Net Sales: ", [1, 16, 0, 0, 0, 36], AccountingFormat(dblNetSales), [0, 10, 0, 0, 0, 36]);
                                WindowsPrinter(" Net Sales: ", [0, 0, 0, 0, 0, 36], AccountingFormat(dblNetSales), [0, 0, 0, 0, 1, 36]);
                                //var strNetSales = SetPrefixPostfix(AccountingFormat(dblNetSales), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Net Sales: ', 12, '  ' + strNetSales, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //WindowsPrinter(" Add : Issued SRV:", [1, 16, 0, 0, 0, 36], AccountingFormat(dblAdjSRV), [0, 10, 0, 0, 0, 36]);
                                WindowsPrinter(" Add : Issued SRV:", [0, 0, 0, 0, 0, 36], AccountingFormat(dblAdjSRV), [0, 0, 0, 0, 1, 36]);
                                //var strIssuedSRV = SetPrefixPostfix(AccountingFormat(dblAdjSRV), intFAFCTotalLenght - 22, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Add : Issued SRV:', 18, strIssuedSRV, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter("________________________________________", [0, 0, 0, 0, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                //WindowsPrinter(" Net Sales Adjusted for Exchanges", [1, 16, 0, 0, 0, 36], AccountingFormat(dblNetSales + dblAdjSRV), [0, 10, 0, 0, 0, 36]);
                                //WindowsPrinter(" Net Sales Adjusted for Exchanges: ", [0, 0, 0, 0, 0, 36], AccountingFormat(dblNetSales + dblAdjSRV), [0, 0, 0, 0, 1, 36]);
                                WindowsPrinter(" Net Sales Adjusted for Exchanges ", [1, 16, 0, 0, 0, 36], ': ', [0, 10, 0, 0, 0, 36], AccountingFormat(dblNetSales + dblAdjSRV), [0, 10, 0, 0, 1, 36]);
                                //var strTotalSales = SetPrefixPostfix(AccountingFormat(dblNetSales + dblAdjSRV), intFAFCTotalLenght - 21, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Net Sales Adjusted for Exchanges', 34, '', intFAFCTotalLenght, '', 0, 1);
                                ////                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' for Exchanges', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                //WindowsPrinter(AccountingFormat(dblNetSales + dblAdjSRV), [0, 10, 0, 0, 0, 36]);
                                WindowsPrinter(" ", [0, 0, 0, 0, 0, 36], AccountingFormat(AccountingFormat(dblNetSales + dblAdjSRV)), [0, 0, 0, 0, 1, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '', 0, '', 0, strTotalSales, intFAFCTotalLenght, 1);
                                //ReadyTextToPrint(strPrint, 3, 2, 2, 0, 1, 1, 0, 0, 0);


                                //END Sales Adjustment

                                //Sales Grouped By Mode of Payment
                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                WindowsPrinter("Inventory of Payments", [0, 10, 0, 0, 0, 36]);
                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, "Inventory of Payments", intFAFCTotalLenght, '', 0, '', 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                //WindowsPrinter(" Cash       ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblCash), [0, 10, 0, 0, 0, 36]);
                                WindowsPrinter(" Cash ", [1, 16, 0, 0, 0, 36], ': ', [0, 10, 0, 0, 0, 36], AccountingFormat(dblCash), [0, 10, 0, 0, 1, 36]);
                                //var strTotalCash = SetPrefixPostfix(AccountingFormat(dblCash), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Cash       ', 12, ': ' + strTotalCash, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //WindowsPrinter(" Check      ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblCheck), [0, 10, 0, 0, 0, 36]);
                                WindowsPrinter(" Check ", [1, 16, 0, 0, 0, 36], ': ', [0, 10, 0, 0, 0, 36], AccountingFormat(dblCheck), [0, 10, 0, 0, 1, 36]);
                                //var strTotalCheck = SetPrefixPostfix(AccountingFormat(dblCheck), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Check      ', 12, ': ' + strTotalCheck, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //WindowsPrinter(" Debit Card", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblDebitCard), [0, 10, 0, 0, 0, 36]);
                                WindowsPrinter(" Debit Card ", [1, 16, 0, 0, 0, 36], ': ', [0, 10, 0, 0, 0, 36], AccountingFormat(dblDebitCard), [0, 10, 0, 0, 1, 36]);
                                //var strTotalDebitCard = SetPrefixPostfix(AccountingFormat(dblDebitCard), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Debit Card', 12, ': ' + strTotalDebitCard, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //WindowsPrinter(" Credit Card", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblCreditCard), [0, 10, 0, 0, 0, 36]);
                                WindowsPrinter(" Credit Card ", [1, 16, 0, 0, 0, 36], ': ', [0, 10, 0, 0, 0, 36], AccountingFormat(dblCreditCard), [0, 10, 0, 0, 1, 36]);
                                //var strTotalCreditCard = SetPrefixPostfix(AccountingFormat(dblCreditCard), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Credit Card', 12, ': ' + strTotalCreditCard, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //WindowsPrinter(" SRV        ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblSRV), [0, 10, 0, 0, 0, 36]);
                                WindowsPrinter(" SRV ", [1, 16, 0, 0, 0, 36], ': ', [0, 10, 0, 0, 0, 36], AccountingFormat(dblSRV), [0, 10, 0, 0, 1, 36]);
                                //var strTotalSRV = SetPrefixPostfix(AccountingFormat(dblSRV), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SRV        ', 12, ': ' + strTotalSRV, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                if (IsFowstInSMMall) {
                                    //WindowsPrinter(" SMAC        ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblSMAC), [0, 10, 0, 0, 0, 36]);
                                    WindowsPrinter(" SMAC ", [1, 16, 0, 0, 0, 36], ': ', [0, 10, 0, 0, 0, 36], AccountingFormat(dblSMAC), [0, 10, 0, 0, 1, 36]);
                                    //var strTotalSMAC = SetPrefixPostfix(AccountingFormat(dblSMAC), intFAFCTotalLenght - 16, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SMAC        ', 12, ': ' + strTotalSMAC, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                    //WindowsPrinter(" E-Plus      ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblEPlus), [0, 10, 0, 0, 0, 36]);
                                    WindowsPrinter(" E-Plus ", [1, 16, 0, 0, 0, 36], ': ', [0, 10, 0, 0, 0, 36], AccountingFormat(dblEPlus), [0, 10, 0, 0, 1, 36]);
                                    //var strTotalEPlus = SetPrefixPostfix(AccountingFormat(dblEPlus), intFAFCTotalLenght - 16, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' E-Plus      ', 12, ': ' + strTotalEPlus, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                    //WindowsPrinter(" Voucher     ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblVoucher), [0, 10, 0, 0, 0, 36]);
                                    WindowsPrinter(" Voucher ", [1, 16, 0, 0, 0, 36], ': ', [0, 10, 0, 0, 0, 36], AccountingFormat(dblVoucher), [0, 10, 0, 0, 1, 36]);
                                    //var strTotalVoucher = SetPrefixPostfix(AccountingFormat(dblVoucher), intFAFCTotalLenght - 16, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Voucher     ', 12, ': ' + strTotalVoucher, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                } else {

                                    WindowsPrinter(" On Credit ", [1, 16, 0, 0, 0, 36], ': ', [0, 10, 0, 0, 0, 36], AccountingFormat(dblOnCredit), [0, 10, 0, 0, 1, 36]);

                                    //May 14 2018 gi comment ang Other Tender kay magconflict sa G-Cash na payment type. pwede ang g-cash inside and outside sa mall
                                    //WindowsPrinter(" Other Tender", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblOtherPaymentType), [0, 10, 0, 0, 0, 36]);
                                    //WindowsPrinter(" Other Tender ", [1, 16, 0, 0, 0, 36], ': ', [0, 10, 0, 0, 0, 36], AccountingFormat(dblOtherPaymentType), [0, 10, 0, 0, 1, 36]);
                                    //var strTotalOther = SetPrefixPostfix(AccountingFormat(dblOtherPaymentType), intFAFCTotalLenght - 16, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Other Tender', 12, ': ' + strTotalOther, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                    //Feb 8 2020 gibalik ang code para maisa ang pagdisplay sa tanan other types na payment

                                    //WindowsPrinter(" Other Tender", [0, 0, 0, 0, 0, 36], ' ', [0, 0, 0, 0, 1, 36]);

                                    if (!CBool(sessionStorage.PrintingOfflineXReadData)) {
                                        if (vDataDataSalesReportOtherPaymentInfo != undefined) {
                                            if (vDataDataSalesReportOtherPaymentInfo.Rows.length > 0) {
                                                for (var otherpaymentcount = 0; otherpaymentcount <= vDataDataSalesReportOtherPaymentInfo.Rows.length - 1; otherpaymentcount++) {

                                                    WindowsPrinter(" " + vDataDataSalesReportOtherPaymentInfo.Rows[otherpaymentcount].PaymentType, [0, 0, 0, 0, 0, 36], AccountingFormat(vDataDataSalesReportOtherPaymentInfo.Rows[otherpaymentcount].TotalAmount), [0, 0, 0, 0, 1, 36]);

                                                }
                                            }
                                        }
                                    }else {

                                        if (vDataDataSalesReportOtherPaymentInfo != undefined) {
                                            var LastOtherPaymentTypesTotalData = JSON.parse(vDataDataSalesReportOtherPaymentInfo);

                                            for (var otherpaymentcount = 0; otherpaymentcount <= Object.keys(LastOtherPaymentTypesTotalData).length - 1; otherpaymentcount++) {

                                                WindowsPrinter("     " + Object.keys(LastOtherPaymentTypesTotalData)[otherpaymentcount], [0, 0, 0, 0, 0, 36], AccountingFormat(LastOtherPaymentTypesTotalData[Object.keys(LastOtherPaymentTypesTotalData)[otherpaymentcount]]), [0, 0, 0, 0, 1, 36]);

                                            }
                                        }
                                    }
                                }

                                //if (dblGCash > 0) {
                                //    WindowsPrinter(" GCash ", [1, 16, 0, 0, 0, 36], ': ', [0, 10, 0, 0, 0, 36], AccountingFormat(dblGCash), [0, 10, 0, 0, 1, 36]);
                                //}

                                //WindowsPrinter("________________________________________", [1, 16, 0, 0, 0, 36]);
                                WindowsPrinter("________________________________________", [0, 0, 0, 0, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                //WindowsPrinter(" Total Inventory of Payments", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblTotalPayment), [0, 10, 0, 0, 0, 36]);
                                WindowsPrinter(" Total Inventory of Payments ", [1, 16, 0, 0, 0, 36], ': ', [0, 10, 0, 0, 0, 36], AccountingFormat(dblTotalPayment), [0, 10, 0, 0, 1, 36]);
                                //var strTotalClassPayment = SetPrefixPostfix(AccountingFormat(dblTotalPayment), intFAFCTotalLenght - 20, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Inventory of Payments', 28, '', intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' of Payments', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //WindowsPrinter("", [1, 16, 0, 0, 0, 36], AccountingFormat(dblTotalPayment), [0, 10, 0, 0, 0, 36]);
                                //WindowsPrinter(" ", [0, 0, 0, 0, 0, 36], AccountingFormat(dblTotalPayment), [0, 0, 0, 0, 1, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '', 0, '', 0, strTotalClassPayment, intFAFCTotalLenght, 1);
                                //ReadyTextToPrint(strPrint, 3, 2, 2, 0, 1, 1, 0, 0, 0);

                                var dblTotalOverPayment = dblTotalPayment - dblNetSales;
                                //////                                                    var strTotalOverPayment = SetPrefixPostfix(AccountingFormat((dblTotalOverPayment < 0 ? (dblTotalOverPayment * -1) : dblTotalOverPayment)), intFAFCTotalLenght - 25, ' ', 0);
                                var strTotalOverPayment = SetPrefixPostfix(AccountingFormat(dblTotalOverPayment), intFAFCTotalLenght - 25, ' ', 0);

                                //WindowsPrinter("Excess of Inventory of", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblTotalOverPayment), [0, 10, 0, 0, 0, 36]);
                                WindowsPrinter(" Excess of Inventory of ", [1, 16, 0, 0, 0, 36], ': ', [0, 10, 0, 0, 0, 36], AccountingFormat(dblTotalOverPayment), [0, 10, 0, 0, 1, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Excess of Inventory of', 23, strTotalOverPayment, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                
                                WindowsPrinter(" Payments vs Net Sales", [1, 16, 0, 0, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Payments vs Net Sales', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //END Sales Grouped By Mode of Payment

                                //Inventory of Payments Audit
                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, "Inventory of Payments Audit", intFAFCTotalLenght, '', 0, '', 0, 0);
                                //                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Net Sales Adjusted', 19, strTotalSales, intFAFCTotalLenght, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' for Exchanges', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Inventory', 16, strTotalClassPayment, intFAFCTotalLenght, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' of Payments', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                var dblNetSalesAdjustedLessTotalInvPayment = (dblNetSales + dblAdjSRV) - dblTotalPayment + dblCharges;
                                var strTotalNetSalesAdjustedLessTotalInvPayment = SetPrefixPostfix(AccountingFormat(dblNetSalesAdjustedLessTotalInvPayment), intFAFCTotalLenght - 13, ' ', 0);
                                //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, (dblNetSalesAdjustedLessTotalInvPayment < 0 ? ' Shortage' : ' Overage '), 9, ': ' + strTotalNetSalesAdjustedLessTotalInvPayment, intFAFCTotalLenght, '', 0, 1);
                                //                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //END Inventory of Payments Audit

                                //Sales Grouped by VAT TYPE----------------------------
                                if ((strFrom == strTo && $("#selPOSSalesReportRange").val() == "Day") ||
                                $("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                    //strPrint = '';
                                    WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                    //WindowsPrinter("Sales Grouped by VAT Type", [1, 16, 0, 0, 0, 36]);
                                    WindowsPrinter("Sales Grouped by VAT Type", [0, 0, 0, 1, 0, 36]);
                                    WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, "Sales Grouped by VAT Type", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //strPrint = strPrint + strTemp;

                                    //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                    //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                    var dblTotalVATSalesPercent = (dblVAT / dblTotalVAT) * 100;
                                    var dblTotalVATAmountPercent = (dblVATAmount / dblTotalVAT) * 100;
                                    var dblTotalNonVATPercent = (dblNonVAT / dblTotalVAT) * 100;
                                    var dblTotalVATExemptPercent = (dblVATExempt / dblTotalVAT) * 100;
                                    var dblTotalZeroRatedVATPercent = (dblZeroRatedVAT / dblTotalVAT) * 100;
                                    var dblTotalVATPercent = dblTotalVATSalesPercent + dblTotalVATAmountPercent + dblTotalNonVATPercent + dblTotalVATExemptPercent + dblTotalZeroRatedVATPercent;

                                    var strTotalVATSales = SetPrefixPostfix(AccountingFormat(dblVAT), 14, ' ', 0);
                                    var strTotalVATAmount = SetPrefixPostfix(AccountingFormat(dblVATAmount), 14, ' ', 0);
                                    var strTotalNonVAT = SetPrefixPostfix(AccountingFormat(dblNonVAT), 14, ' ', 0);
                                    var strTotalVATExempt = SetPrefixPostfix(AccountingFormat(dblVATExempt), 14, ' ', 0);
                                    var strTotalZeroRatedVAT = SetPrefixPostfix(AccountingFormat(dblZeroRatedVAT), 14, ' ', 0);
                                    var strTotalAllVAT = SetPrefixPostfix(AccountingFormat(dblTotalVAT), 14, ' ', 0);

                                    var strTotalVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATSalesPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalVATAmountPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATAmountPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalNonVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalNonVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalVATExemptPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATExemptPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalZeroRatedVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalZeroRatedVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    var strTotalAllVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);

                                    if (IsVAT != undefined) {
                                        if (!CBool(IsVAT)) {
                                            dblNonVAT = dblVAT + dblVATAmount;
                                            dblVAT = 0;
                                            dblVATAmount = 0;

                                            dblTotalVATSalesPercent = 0;
                                            dblTotalVATAmountPercent = 0;
                                            dblTotalNonVATPercent = 100;
                                        }
                                    }

                                    WindowsPrinter("VAT-able Sales", [1, 16, 0, 0, 0, 36], AccountingFormat(dblVAT), [0, 10, 0, 0, 0, 36], AccountingFormat(dblTotalVATSalesPercent) + '%', [0, 10, 0, 0, 1, 36]);
                                    //strPrint = ' ';
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-able Sales', 14, strTotalVATSales, 30, strTotalVATPercent, intFAFCTotalLenght, 1);
                                    //strPrint = strPrint + strTemp;

                                    WindowsPrinter("VAT Amount", [1, 16, 0, 0, 0, 36], AccountingFormat(dblVATAmount), [0, 10, 0, 0, 0, 36], AccountingFormat(dblTotalVATAmountPercent) + '%', [0, 10, 0, 0, 1, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT Amount', 14, strTotalVATAmount, 30, strTotalVATAmountPercent, intFAFCTotalLenght, 1);
                                    //strPrint = strPrint + strTemp;

                                    WindowsPrinter("Non-VAT", [1, 16, 0, 0, 0, 36], AccountingFormat(dblNonVAT), [0, 10, 0, 0, 0, 36], AccountingFormat(dblTotalNonVATPercent) + '%', [0, 10, 0, 0, 1, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Non-VAT', 14, strTotalNonVAT, 30, strTotalNonVATPercent, intFAFCTotalLenght, 1);
                                    //strPrint = strPrint + strTemp;

                                    WindowsPrinter("VAT-Exempt", [1, 16, 0, 0, 0, 36], AccountingFormat(dblVATExempt), [0, 10, 0, 0, 0, 36], AccountingFormat(dblTotalVATExemptPercent) + '%', [0, 10, 0, 0, 1, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-Exempt', 14, strTotalVATExempt, 30, strTotalVATExemptPercent, intFAFCTotalLenght, 1);
                                    //strPrint = strPrint + strTemp;

                                    WindowsPrinter("Zero-Rated VAT", [1, 16, 0, 0, 0, 36], AccountingFormat(dblZeroRatedVAT), [0, 10, 0, 0, 0, 36], AccountingFormat(dblTotalZeroRatedVATPercent) + '%', [0, 10, 0, 0, 1, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Zero-Rated VAT', 14, strTotalZeroRatedVAT, 30, strTotalZeroRatedVATPercent, intFAFCTotalLenght, 1);
                                    //strPrint = strPrint + strTemp;

                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    WindowsPrinter("________________________________________", [0, 0, 0, 0, 0, 36]);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                    WindowsPrinter("Zero-Rated VAT", [1, 16, 0, 0, 0, 36], AccountingFormat(dblTotalVAT), [0, 10, 0, 0, 0, 36], AccountingFormat(dblTotalVATPercent) + '%', [0, 10, 0, 0, 1, 36]);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '              ', 14, strTotalAllVAT, 30, strTotalAllVATPercent, intFAFCTotalLenght, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                }
                                //End Sales Grouped by VAT TYPE----------------------------

                                //Get Sales by Cashier
                                if ((strFrom == strTo && $("#selPOSSalesReportRange").val() == "Day") ||
                                                $("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                    if (vDataSalesByCashier != undefined) {
                                        if (vDataSalesByCashier.Rows.length > 0) {

                                            //WindowsPrinter('Sales Grouped by Cashier', [0, 0, 0, 0, 0, 36]);
                                            WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                            WindowsPrinter("Sales Grouped by Cashier", [0, 0, 0, 1, 0, 36]);
                                            //strPrint = '';
                                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, "Sales Grouped by Cashier", intFAFCTotalLenght, '', 0, '', 0, 0);
                                            //strPrint = strPrint + strTemp;

                                            //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                            //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                            //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                            //Get Total Sales
                                            var dblAllTotalSales = 0;
                                            for (var p = 0; p <= vDataSalesByCashier.Rows.length - 1; p++) {
                                                dblAllTotalSales = dblAllTotalSales + parseFloat(vDataSalesByCashier.Rows[p].TotalSales);
                                            };

                                            var dblAllTotalSalesPercent = 0;

                                            strPrint = '';

                                            for (var p = 0; p <= vDataSalesByCashier.Rows.length - 1; p++) {

                                                var strTotalSales = SetPrefixPostfix(AccountingFormat((vDataSalesByCashier.Rows[p].TotalSales || 0)), 10, ' ', 0);
                                                var dblTotalSalesPercent = (parseFloat(vDataSalesByCashier.Rows[p].TotalSales || 0) / dblAllTotalSales) * 100;
                                                var strTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                                dblAllTotalSalesPercent = dblAllTotalSalesPercent + dblTotalSalesPercent;

                                                WindowsPrinter(vDataSalesByCashier.Rows[p].CashierName, [1, 16, 0, 0, 0, 36], strTotalSales, [0, 10, 0, 0, 0, 36], strTotalSalesPercent, [0, 10, 0, 0, 1, 36]);

                                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, vDataSalesByCashier.Rows[p].CashierName, 14, strTotalSales, 30, strTotalSalesPercent, intFAFCTotalLenght, 0);
                                                //strPrint = strPrint + strTemp;

                                            };

                                            //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                            var strAllTotalSales = SetPrefixPostfix(AccountingFormat(dblAllTotalSales), 10, ' ', 0);
                                            var strAllTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblAllTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                            WindowsPrinter("________________________________________", [0, 0, 0, 0, 0, 36]);
                                            //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                            //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);


                                            //strPrint = FormatTextToPrint(intFAFCTotalLenght, '              ', 15, strAllTotalSales, 31, strAllTotalSalesPercent, intFAFCTotalLenght, 1);
                                            //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                        }
                                    }
                                }

                                //Get Sales by Machine
                                if ((strFrom == strTo && $("#selPOSSalesReportRange").val() == "Day") ||
                                            $("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                    if (vDataSalesByMachine != undefined) {
                                        if (vDataSalesByMachine.Rows.length > 0) {

                                            WindowsPrinter('Sales Grouped by Machine (MIN) ', [0, 0, 0, 0, 0, 36]);
                                            //strPrint = '';
                                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, "Sales Grouped by Machine (MIN)", intFAFCTotalLenght, '', 0, '', 0, 0);
                                            //strPrint = strPrint + strTemp;

                                            //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                            //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                            //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                            //Get Total Sales
                                            dblAllTotalSales = 0;
                                            for (var p = 0; p <= vDataSalesByMachine.Rows.length - 1; p++) {
                                                dblAllTotalSales = dblAllTotalSales + parseFloat(vDataSalesByMachine.Rows[p].TotalSales);
                                            };

                                            dblAllTotalSalesPercent = 0;

                                            strPrint = '';
                                            for (var p = 0; p <= vDataSalesByMachine.Rows.length - 1; p++) {

                                                var strTotalSales = SetPrefixPostfix(AccountingFormat((vDataSalesByMachine.Rows[p].TotalSales || 0)), 14, ' ', 0);
                                                var dblTotalSalesPercent = (parseFloat(vDataSalesByMachine.Rows[p].TotalSales || 0) / dblAllTotalSales) * 100;
                                                var strTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                                dblAllTotalSalesPercent = dblAllTotalSalesPercent + dblTotalSalesPercent;

                                                WindowsPrinter('     ' + vDataSalesByCashierMachine.Rows[p].DeviceName, [1, 16, 0, 0, 0, 36], strTotalSales, [0, 10, 0, 0, 0, 36], strTotalSalesPercent, [0, 10, 0, 0, 1, 36]);
                                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, vDataSalesByMachine.Rows[p].DeviceName, 14, strTotalSales, 30, strTotalSalesPercent, intFAFCTotalLenght, 0);
                                                //strPrint = strPrint + strTemp;

                                            };

                                            //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                            strAllTotalSales = SetPrefixPostfix(AccountingFormat(dblAllTotalSales), 14, ' ', 0);
                                            strAllTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblAllTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                            WindowsPrinter("________________________________________", [0, 0, 0, 0, 0, 36]);
                                            //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                            //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                            WindowsPrinter('              ', [1, 16, 0, 0, 0, 36], strAllTotalSales, [0, 10, 0, 0, 0, 36], strAllTotalSalesPercent, [0, 10, 0, 0, 1, 36]);

                                            //strPrint = FormatTextToPrint(intFAFCTotalLenght, '              ', 15, strAllTotalSales, 31, strAllTotalSalesPercent, intFAFCTotalLenght, 1);
                                            //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                        }
                                    }
                                }

                                //Get Sales by Cashier and Machine
                                if ((strFrom == strTo && $("#selPOSSalesReportRange").val() == "Day") ||
                                            $("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                    if (vDataSalesByCashierMachine != undefined) {
                                        if (vDataSalesByCashierMachine.Rows.length > 0) {

                                            WindowsPrinter('Sales Grouped by Cashier & Machine (MIN)', [0, 0, 0, 0, 0, 36]);

                                            //strPrint = '';
                                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, "Sales Grouped by Cashier & Machine (MIN)", intFAFCTotalLenght, '', 0, '', 0, 0);
                                            //strPrint = strPrint + strTemp;

                                            //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                            //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                            //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                            //Get Total Sales
                                            dblAllTotalSales = 0;
                                            for (var p = 0; p <= vDataSalesByCashierMachine.Rows.length - 1; p++) {
                                                dblAllTotalSales = dblAllTotalSales + parseFloat(vDataSalesByCashierMachine.Rows[p].TotalSales);
                                            };

                                            dblAllTotalSalesPercent = 0;

                                            var strCashierName = "";
                                            var strDeviceName = "";
                                            var dblDeviceSales = 0;

                                            dblAllTotalSalesPercent = 0;

                                            strPrint = '';
                                            for (var p = 0; p <= vDataSalesByCashierMachine.Rows.length - 1; p++) {

                                                if (strCashierName != vDataSalesByCashierMachine.Rows[p].CashierName) {
                                                    WindowsPrinter(vDataSalesByCashierMachine.Rows[p].CashierName, [0, 0, 0, 0, 0, 36]);
                                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, vDataSalesByCashierMachine.Rows[p].CashierName, intFAFCTotalLenght, '', 0, '', 0, 0);
                                                    //strPrint = strPrint + strTemp;

                                                    strCashierName = vDataSalesByCashierMachine.Rows[p].CashierName;
                                                }

                                                dblDeviceSales = parseFloat(vDataSalesByCashierMachine.Rows[p].TotalSales || 0);
                                                strTotalSales = SetPrefixPostfix(AccountingFormat(dblDeviceSales), 14, ' ', 0);
                                                dblTotalSalesPercent = (dblDeviceSales / dblAllTotalSales) * 100;
                                                strTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                                dblAllTotalSalesPercent = dblAllTotalSalesPercent + dblTotalSalesPercent;

                                                WindowsPrinter('     ' + vDataSalesByCashierMachine.Rows[p].DeviceName, [1, 16, 0, 0, 0, 36], strTotalSales, [0, 10, 0, 0, 0, 36], strTotalSalesPercent, [0, 10, 0, 0, 1, 36]);

                                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, '     ' + vDataSalesByCashierMachine.Rows[p].DeviceName, 14, strTotalSales, 30, strTotalSalesPercent, intFAFCTotalLenght, 0);
                                                //strPrint = strPrint + strTemp;

                                            };

                                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                            strAllTotalSales = SetPrefixPostfix(AccountingFormat(dblAllTotalSales), 14, ' ', 0);
                                            strAllTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblAllTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                            WindowsPrinter("________________________________________", [0, 0, 0, 0, 0, 36]);
                                            //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                            //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                            WindowsPrinter('              ', [1, 16, 0, 0, 0, 36], strAllTotalSales, [0, 10, 0, 0, 0, 36], strAllTotalSalesPercent, [0, 10, 0, 0, 1, 36]);

                                            //strPrint = FormatTextToPrint(intFAFCTotalLenght, '              ', 15, strAllTotalSales, 31, strAllTotalSalesPercent, intFAFCTotalLenght, 1);
                                            //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                        }
                                    }
                                }

                                if ($("#selPOSSalesReportRange").val() == "Day" ||
                                    $("#selPOSSalesReportRange").val() == "Today" ||
                                    $("#selPOSSalesReportRange").val() == "Yesterday") {

                                    //Other Informatoin
                                    strPrint = '';
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "Other Information", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    strPrint = strPrint + strTemp;

                                    //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                    //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                    var strTransCount = SetPrefixPostfix(intTransCount + '', intFAFCTotalLenght - 21, ' ', 0);

                                    //WindowsPrinter(" Total Trans. Count", [1, 16, 0, 0, 0, 36], ': ' + strTransCount, [0, 10, 0, 0, 0, 36]);
                                    WindowsPrinter(" Total Trans. Count ", [1, 16, 0, 0, 0, 36], ': ', [0, 10, 0, 0, 0, 36], strTransCount, [0, 10, 0, 0, 1, 36]);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Trans. Count', 19, strTransCount, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    var strSalesTransCount = SetPrefixPostfix(intTransCount + '', intFAFCTotalLenght - 21, ' ', 0);

                                    //WindowsPrinter(" Sales Trans. Count", [1, 16, 0, 0, 0, 36], ': ' + strSalesTransCount, [0, 10, 0, 0, 0, 36]);
                                    WindowsPrinter(" Sales Trans. Count ", [1, 16, 0, 0, 0, 36], ': ', [0, 10, 0, 0, 0, 36], strSalesTransCount, [0, 10, 0, 0, 1, 36]);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Sales Trans. Count', 19, strSalesTransCount, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    var strSalesItemCount = SetPrefixPostfix(intSalesItemCount + '', intFAFCTotalLenght - 24, ' ', 0);

                                    //WindowsPrinter(" Sales Line Item Count", [1, 16, 0, 0, 0, 36], ': ' + strSalesItemCount, [0, 10, 0, 0, 0, 36]);
                                    WindowsPrinter(" Sales Line Item Count ", [1, 16, 0, 0, 0, 36], ': ', [0, 10, 0, 0, 0, 36], strSalesItemCount, [0, 10, 0, 0, 1, 36]);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Sales Line Item Count', 22, strSalesItemCount, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    var strSRVTransCount = SetPrefixPostfix(intSRVTransCount + '', intFAFCTotalLenght - 19, ' ', 0);

                                    //WindowsPrinter(" SRV Trans. Count", [1, 16, 0, 0, 0, 36], ': ' + strSRVTransCount, [0, 10, 0, 0, 0, 36]);
                                    WindowsPrinter(" SRV Trans. Count ", [1, 16, 0, 0, 0, 36], ': ', [0, 10, 0, 0, 0, 36], strSRVTransCount, [0, 10, 0, 0, 1, 36]);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SRV Trans. Count', 17, strSRVTransCount, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    var strSRVItemCount = SetPrefixPostfix(intSRVItemCount + '', intFAFCTotalLenght - 22, ' ', 0);

                                    //WindowsPrinter(" SRV Line Item Count", [1, 16, 0, 0, 0, 36], ': ' + strSRVItemCount, [0, 10, 0, 0, 0, 36]);
                                    WindowsPrinter(" SRV Line Item Count ", [1, 16, 0, 0, 0, 36], ': ', [0, 10, 0, 0, 0, 36], strSRVItemCount, [0, 10, 0, 0, 1, 36]);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SRV Line Item Count', 20, strSRVItemCount, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    var strRefTransCount = SetPrefixPostfix(intRefTransCount + '', intFAFCTotalLenght - 22, ' ', 0);

                                    //WindowsPrinter(" Refund Trans. Count", [1, 16, 0, 0, 0, 36], ': ' + strRefTransCount, [0, 10, 0, 0, 0, 36]);
                                    WindowsPrinter(" Refund Trans. Count ", [1, 16, 0, 0, 0, 36], ': ', [0, 10, 0, 0, 0, 36], strRefTransCount, [0, 10, 0, 0, 1, 36]);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Refund Trans. Count', 20, strRefTransCount, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    var strRefItemCount = SetPrefixPostfix(intRefItemCount + '', intFAFCTotalLenght - 25, ' ', 0);

                                    //WindowsPrinter(" Refund Line Item Count", [1, 16, 0, 0, 0, 36], ': ' + strRefItemCount, [0, 10, 0, 0, 0, 36]);
                                    WindowsPrinter(" Refund Line Item Count ", [1, 16, 0, 0, 0, 36], ': ', [0, 10, 0, 0, 0, 36], strRefItemCount, [0, 10, 0, 0, 1, 36]);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Refund Line Item Count', 23, strRefItemCount, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    var strCustomerCount = SetPrefixPostfix(intSalesTransCount + '', intFAFCTotalLenght - 17, ' ', 0);

                                    //WindowsPrinter(" Customer Count", [1, 16, 0, 0, 0, 36], ': ' + strCustomerCount, [0, 10, 0, 0, 0, 36]);
                                    WindowsPrinter(" Total Customer ", [1, 16, 0, 0, 0, 36], ': ', [0, 10, 0, 0, 0, 36], strCustomerCount, [0, 10, 0, 0, 1, 36]);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Customer Count', 15, strCustomerCount, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                    //END Other Informatoin

                                    WindowsPrinter(" Printed by ", [1, 16, 0, 0, 0, 36], ': ' + (vDataHeader.Rows[0].PrintedBy || ''), [0, 10, 0, 0, 0, 36]);
                                    //WindowsPrinter(" Printed by ", [1, 16, 0, 0, 0, 36], ': ', [0, 10, 0, 0, 0, 36], strCustomerCount, [0, 10, 0, 0, 1, 36]);

                                    //strPrint = '';
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Printed by ', 15, ': ' + (vDataHeader.Rows[0].PrintedBy || ''), intFAFCTotalLenght, '', 0, 1);
                                    //strPrint = strPrint + strTemp;

                                    WindowsPrinter(" Date and Time ", [1, 16, 0, 0, 0, 36], ': ' + ($.format.date(new Date(vDataHeader.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), [0, 10, 0, 0, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataHeader.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                    //strPrint = strPrint + strTemp;

                                    //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                    //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);

                                    //PrintText(strPrinterIP);
                                    //ClearCommandBuffer();
                                    WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                    WindowsPrinter("address", [0, 0, 0, 0, 0, 0], strPrinterIP, [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);
                                    WindowsPrinter("startPrint", [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);

                                } else {
                                    WindowsPrinter(" Printed by ", [1, 16, 0, 0, 0, 36], ': ' + (vDataHeader.Rows[0].PrintedBy || ''), [0, 10, 0, 0, 0, 36]);
                                    //strPrint = '';
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Printed by ', 15, ': ' + (vDataHeader.Rows[0].PrintedBy || ''), intFAFCTotalLenght, '', 0, 1);
                                    //strPrint = strPrint + strTemp;

                                    WindowsPrinter(" Date and Time ", [1, 16, 0, 0, 0, 36], ': ' + ($.format.date(new Date(vDataHeader.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), [0, 10, 0, 0, 0, 36]);

                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataHeader.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                    //strPrint = strPrint + strTemp;

                                    //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                    //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);

                                    //PrintText(strPrinterIP);
                                    //ClearCommandBuffer();
                                    WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                    WindowsPrinter("address", [0, 0, 0, 0, 0, 0], strPrinterIP, [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);
                                    WindowsPrinter("startPrint", [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);
                                }

                            } else {
                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                WindowsPrinter("address", [0, 0, 0, 0, 0, 0], strPrinterIP, [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);
                                WindowsPrinter("startPrint", [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);
                                //PrintText(strPrinterIP);
                                //ClearCommandBuffer();
                            }

                        }


                    }
                    else {
                        MessageBox("No available printer for this branch. Please contact your system administrator.",
                            "Printer",
                            [
                                {
                                    Text: 'Ok'
                                }
                            ]
                    );
                    }

                }

                function WindowsPrinter(cols1, args1, cols2, args2, cols3, args3) {
                    if ((cols3 == null || cols3 == undefined) && (cols2 != null || cols2 != undefined)) {
                        Android.WindowsPrinting(
												{
												    "data": [
                                                              { "cols1": cols1 },
                                                              { "args1": [args1[0].toString(), args1[1].toString(), args1[2].toString(), args1[3].toString(), args1[4].toString(), args1[5].toString()] },
                                                              { "cols2": cols2 },
                                                              { "args2": [args2[0].toString(), args2[1].toString(), args2[2].toString(), args2[3].toString(), args2[4].toString(), args2[5].toString()] }
												    ]
												}
						);
                    } else if ((cols3 == undefined || cols3 == null) && (cols2 == null || cols2 == undefined)) {
                        Android.WindowsPrinting(
												{
												    "data": [
                                                              { "cols1": cols1 },
                                                              { "args1": [args1[0].toString(), args1[1].toString(), args1[2].toString(), args1[3].toString(), args1[4].toString(), args1[5].toString()] }
												    ]
												}
						);
                    } else if (cols1 == "newLine") {
                        Android.WindowsPrinting(
												{
												    "data": [
                                                              { "newLine": "newLine" }
												    ]
												}
						);
                    } else if (cols1 == "startPrint") {
                        try {
                            Android.WindowsPrinting(
                                                    {
                                                        "data": [
                                                                  { "startPrint": "startPrint" }
                                                        ]
                                                    }
                            );
                        } catch (e) {
                            //get ip address from settings
                            var MobizSettingsIPAddress = '';

                            MobizSettingsIPAddress = Transfers.ReadFile('printerip.txt', null, true)

                            if (MobizSettingsIPAddress != undefined && MobizSettingsIPAddress != '') {

                                Android.AddLog("Overriding printing IP with " + MobizSettingsIPAddress);

                                WindowsPrinter("address", [0, 0, 0, 0, 0, 0], MobizSettingsIPAddress, [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);

                                try {
                                    Android.WindowsPrinting(
                                                                {
                                                                    "data": [
                                                                              { "startPrint": "startPrint" }
                                                                    ]
                                                                }
                                                            );
                                } catch (e) {
                                    Android.AddLog("A problem occured while printing receipt using WPS method.")
                                }
                            } else {
                                Android.AddLog("A problem occured while printing receipt using WPS method.")
                            }
                        }
                    } else if (cols1 == "address") {

                        Android.AddLog("WPS IP address.: " + cols2);

                        Android.WindowsPrinting(
												{
												    "data": [
                                                              { "address": cols2 }
												    ]
												}
						);
                    } else {
                        Android.WindowsPrinting(
												{
												    "data": [
                                                              { "cols1": cols1 },
                                                              { "args1": [args1[0].toString(), args1[1].toString(), args1[2].toString(), args1[3].toString(), args1[4].toString(), args1[5].toString()] },
                                                              { "cols2": cols2 },
                                                              { "args2": [args2[0].toString(), args2[1].toString(), args2[2].toString(), args2[3].toString(), args2[4].toString(), args2[5].toString()] },
                                                              { "cols3": cols3 },
                                                              { "args3": [args3[0].toString(), args3[1].toString(), args3[2].toString(), args3[3].toString(), args3[4].toString(), args3[5].toString()] }
												    ]
												}
						);
                    }
                }

                function DisplayReport(vDataPrint, vDataHeader, vDataInfo, vDataSalesByCashier, vDataSalesByMachine, vDataSalesByCashierMachine, vDataDataSalesReportOtherPaymentInfo, strFrom, strTo, strYear, vDataTurnOverAndDeposits) {
                    var pInput = {
                        ByCashier: [],
                        ByMachine: [],
                        ByCashierMachine: []
                    };
                    var pValues = [];
                    //$('#tblPOSSalesReport').css('visibility', 'visible');
                    //$('#tblPOSSalesReport').css('display', 'block');
                    $('#divDisplayPOSSalesReport').show();
                    $('#divPrintReportContainer').pageshow();

                    sessionStorage.ReportHeader = JSON.stringify(vDataHeader.Rows[0]);

                    $('#lblOrganizationBusinessName').text($.trim((vDataHeader.Rows[0].OrgBusinessTradeName || '')))
                    pValues.push({
                        Key: 'OrganizationBusinessName',
                        DataType: 'string',
                        Value: $.trim((vDataHeader.Rows[0].OrgBusinessTradeName || ''))
                    });
                    $('#lblOrganizationName').text($.trim((vDataHeader.Rows[0].SubscriberOrganizationName || '')))
                    pValues.push({
                        Key: 'OrganizationName',
                        DataType: 'string',
                        Value: $.trim((vDataHeader.Rows[0].SubscriberOrganizationName || ''))
                    });
                    $('#lblOrganizationFOWST').text($.trim((vDataHeader.Rows[0].OrgBranchName || '')))
                    pValues.push({
                        Key: 'OrganizationFOWST',
                        DataType: 'string',
                        Value: $.trim((vDataHeader.Rows[0].OrgBranchName || ''))
                    });
                    $('#lblOrganizationFOWSTAddress').text($.trim((vDataHeader.Rows[0].OrgCompleteAddress || '')))
                    pValues.push({
                        Key: 'OrganizationFOWSTAddress',
                        DataType: 'string',
                        Value: $.trim((vDataHeader.Rows[0].OrgCompleteAddress || ''))
                    });
                    $('#lblOrganizationFOWSTTIN').text((CBool(vDataHeader.Rows[0].IsVAT) ? 'VAT' : 'NON-VAT') + 'REG TIN ' + $.trim((vDataHeader.Rows[0].TIN || vDataHeader.Rows[0].OrgTIN || '')))
                    pValues.push({
                        Key: 'OrganizationFOWSTTIN',
                        DataType: 'string',
                        Value: $.trim((vDataHeader.Rows[0].OrgTIN || ''))
                    });
                    $('#lblOrganizationFOWSTContactNumber').text('Tel. No. ' + $.trim((vDataHeader.Rows[0].OrgTelNo || '')))
                    pValues.push({
                        Key: 'OrganizationFOWSTContactNumber',
                        DataType: 'string',
                        Value: $.trim('Tel. No. ' + $.trim((vDataHeader.Rows[0].OrgTelNo || '')))
                    });

                    $("#lblOrganizationFOWSTMachineSN").text('Machine S/N: ' + $.trim((vDataHeader.Rows[0].UuidDisplay || '')));
                    $("#lblOrganizationFOWSTPTUNo").text('PTU No.: ' + $.trim((vDataHeader.Rows[0].POSPermitNo || '')));
                    $("#lblOrganizationFOWSTMIN").text('MIN: ' + $.trim((vDataHeader.Rows[0].MIN || '')));

                    if (vDataHeader.Rows[0].POSReportType == 0) {
                        $('#lblCashierLabel').show();
                        $('#lblCashierName').css('visibility', 'visible');
                        //$('#dvManagerSalesReportExcessSection').css('visibility', 'hidden');
                        $('#dvManagerSalesReportExcessSection').css('display', 'none');

                        if (vDataInfo.Rows.length > 0) {

                            $('#lblReportType').text("CASHIER'S");

                            //if ($("#selPOSSalesReportRange").val() == "Day") - OLD
                            if ($("#selPOSSalesReportRange").val() == "Day") {
                                $('#lblReportRange').text("SALES REPORT (X-READ)");
                               
                                $('#lblReportDate').text((strFrom == strTo ? 'Date: ' + strFrom : 'Date: ' + strFrom + ' to ' + strTo));
                                
                            } else if ($("#selPOSSalesReportRange").val() == "Week") {

                                strPrint = '';
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, "Same-Store Comparative Sales Report", intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? 'Week: ' + strFrom : 'From Week ' + strFrom + ' To Week ' + strTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                            } else if ($("#selPOSSalesReportRange").val() == "Month") {

                                strPrint = '';
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, "Same-Store Comparative Sales Report", intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? 'Month: ' + $.format.date(new Date(strFrom + '/1/2013'), 'MMM') : 'From ' + $.format.date(new Date(strFrom + '/1/2013'), 'MMM') + ' To ' + $.format.date(new Date(strTo + '/1/2013'), 'MMM')), intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                            } else if ($("#selPOSSalesReportRange").val() == "Quarter") {

                                strPrint = '';
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, "Same-Store Comparative Sales Report", intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? (strFrom == '1' ? "1st Quarter" : (strFrom == '2' ? "2nd Quarter" : (strFrom == '3' ? "3rd Quarter" : (strFrom == '4' ? "4th Quarter" : "")))) : 'From ' + (strFrom == '1' ? "1st Quarter" : (strFrom == '2' ? "2nd Quarter" : (strFrom == '3' ? "3rd Quarter" : (strFrom == '4' ? "4th Quarter" : "")))) + ' To ' + (strTo == '1' ? "1st Quarter" : (strTo == '2' ? "2nd Quarter" : (strTo == '3' ? "3rd Quarter" : (strTo == '4' ? "4th Quarter" : ""))))), intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                            } else if ($("#selPOSSalesReportRange").val() == "Year") {
                                strPrint = '';
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, "SALES REPORT", intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Year: ' + strYear, intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                            } else if ($("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {
                                $('#lblReportRange').text("SALES REPORT (X-READ)");

                                if ((new Date(strFrom) - new Date(strTo)) != 0) {
                                    $('#lblReportDate').text('Date: ' + strFrom + ' to ' + strTo);
                                    //$('#lblReportDate').text((dateFrom == dateTo ? 'Date: ' + dateFrom : 'From ' + dateFrom + ' To ' + dateTo));
                                } else {
                                    $('#lblReportDate').text('Date : ' + $.format.date(new Date(strFrom), 'MM-dd-yyyy'));
                                    //if (newRangeType == "Day") {
                                    //    $('#lblReportDate').text((dateFrom == dateTo ? 'Date: ' + dateFrom : 'Date: ' + dateFrom + ' to ' + dateTo));
                                    //}
                                }

                            }

                            pValues.push({
                                Key: 'ReportType',
                                DataType: 'string',
                                Value: $('#lblReportType').text()
                            });
                            
                            pValues.push({
                                Key: 'ReportRange',
                                DataType: 'string',
                                Value: $('#lblReportRange').text()
                            });
                                
                            pValues.push({
                                Key: 'ReportDate',
                                DataType: 'string',
                                Value: $('#lblReportDate').text()
                            });                                

                            $('#lblCashierName').text((vDataInfo.Rows[0].CashierName || ''));
                            pValues.push({
                                Key: 'CashierName',
                                DataType: 'string',
                                Value: $('#lblCashierName').text()
                            });    

                            var dblTotalSales = 0;
                            var dblTotalSalesDiscount = 0;
                            var dblTotalVATExemptAmount = 0;
                            var dblAdjRefund = 0;
                            var dblAdjSRV = 0;
                            var dblNetSales = 0;

                            var dblCash = 0;
                            var dblCheck = 0;
                            var dblDebitCard = 0;
                            var dblCreditCard = 0;
                            var dblSRV = 0;
                            var dblOnCredit = 0;
                            var dblOtherPaymentType = 0;
                            var dblCharges = 0;
                            var dblSMAC = 0;
                            var dblGCash = 0;
                            var dblEPlus = 0;
                            var dblVoucher = 0;
                            var dblTotalPayment = 0;

                            var dblVAT = 0;
                            var dblVATAmount = 0;
                            var dblNonVAT = 0;
                            var dblVATExempt = 0;
                            var dblZeroRatedVAT = 0;
                            var dblTotalVAT = 0;
                            var intVoidedItemQty = 0;
                            var dblTotalVoidAmount = 0;

                            if (!IsPrintingOfflineData) {
                                for (var p = 0; p <= vDataInfo.Rows.length - 1; p++) {

                                    dblTotalSales = dblTotalSales + parseFloat(vDataInfo.Rows[p].CurGrossSales || 0);
                                    dblTotalSalesDiscount = dblTotalSalesDiscount + parseFloat(vDataInfo.Rows[p].CurSalesDiscount || 0) + parseFloat(vDataInfo.Rows[p].CurSCDiscount || 0);
                                    dblTotalVATExemptAmount = dblTotalVATExemptAmount + parseFloat(vDataInfo.Rows[p].CurVATExempt || 0);
                                    dblAdjRefund = dblAdjRefund + parseFloat(vDataInfo.Rows[p].CurSalesReturnRefund || 0);
                                    dblAdjSRV = dblAdjSRV + parseFloat(vDataInfo.Rows[p].CurSalesReturnExchange || 0);

                                    dblCash = dblCash + parseFloat(vDataInfo.Rows[p].CashAmount || 0);
                                    dblCheck = dblCheck + parseFloat(vDataInfo.Rows[p].CheckPaymentAmount || 0) + parseFloat(vDataInfo.Rows[p].CurPotatoCornerGC || 0);
                                    dblDebitCard = dblDebitCard + parseFloat(vDataInfo.Rows[p].DebitCardAmount || 0);
                                    dblCreditCard = dblCreditCard + parseFloat(vDataInfo.Rows[p].CreditCardAmount || 0);
                                    dblSRV = dblSRV + parseFloat(vDataInfo.Rows[p].SRVAmount || 0);
                                    dblOnCredit = dblOnCredit + parseFloat(vDataInfo.Rows[p].OnCreditAmount || 0);
                                    dblOtherPaymentType = dblOtherPaymentType + parseFloat(vDataInfo.Rows[p].OtherTypeAmount || 0);
                                    dblCharges = dblCharges + parseFloat(vDataInfo.Rows[p].ChargeAmount || 0);
                                    dblSMAC = dblSMAC + parseFloat(vDataInfo.Rows[p].SMACAmount || 0);
                                    dblGCash = dblGCash + parseFloat(vDataInfo.Rows[p].GCashAmount || 0);
                                    dblEPlus = dblEPlus + parseFloat(vDataInfo.Rows[p].EPlusAmount || 0);
                                    dblVoucher = dblVoucher + parseFloat(vDataInfo.Rows[p].VoucherAmount || 0);
                                    
                                    dblVAT = dblVAT + parseFloat(vDataInfo.Rows[p].CurTotalVatablePortionOfSCDTransaction || 0) + parseFloat(vDataInfo.Rows[p].CurTotalVATSales || 0) - parseFloat(vDataInfo.Rows[p].CurTotalVATSalesSalesReturn || 0);
                                    dblVATAmount = dblVATAmount  + parseFloat(vDataInfo.Rows[p].CurTotalVatAmountPortionOfSCDTransaction || 0) + parseFloat(vDataInfo.Rows[p].CurTotalVATAmount || 0) - parseFloat(vDataInfo.Rows[p].CurTotalVATAmountSalesReturn || 0);
                                    dblNonVAT = dblNonVAT + parseFloat(vDataInfo.Rows[p].CurNoTAXSales || 0);
                                    dblVATExempt = dblVATExempt + parseFloat(vDataInfo.Rows[p].CurVATExemptSales || 0);
                                    dblZeroRatedVAT = dblZeroRatedVAT + parseFloat(vDataInfo.Rows[p].CurZeroRatedVATSales || 0);

                                    intVoidedItemQty += parseFloat(vDataInfo.Rows[p].CurVoidItemQty || 0);
                                    dblTotalVoidAmount += parseFloat(vDataInfo.Rows[p].CurVoid || 0);
                                };
                            }
                            else {
                                dblTotalSales += OfflineTotalSales;
                                dblTotalSalesDiscount += OfflineTotalSalesDiscount;
                                //dblTotalVATExemptAmount += OfflineTotalVATExemptAmount;
                                dblAdjRefund += OfflineAdjRefund;
                                dblAdjSRV += OfflineAdjSRV;

                                dblCash += OfflineCash;
                                dblCheck += OffLineCheck;
                                dblDebitCard += OffLineDebitCard;
                                dblCreditCard += OffLineCreditCard;
                                dblSRV += OffLineSRV;
                                dblOtherPaymentType += OffLineOtherPayment;
                                dblOnCredit += OffLineOnCredit;
                                dblCharges += OffLineCharges;

                                dblVAT += OffLineVAT;
                                dblVATAmount += OffLineVATAmount;
                                dblNonVAT += OffLineNonVAT;
                                dblVATExempt += OffLineVATExempt;
                                dblZeroRatedVAT += OffLineZeroRatedVAT;
                                dblTotalVATExemptAmount += OffLineForeGoneVAT;

                                intVoidedItemQty += OffLineVoidedItemQty;
                                dblTotalVoidAmount += OffLineTotalVoidAmount;

                                dblEPlus = OffLineEPlus;
                                dblGCash = OffLineGCash;
                                dblSMAC = OffLineSMAC;
                                dblVoucher = OffLineVoucher;
                            }

                            dblCash = dblCash;
                            dblNetSales = dblTotalSales - dblTotalSalesDiscount - dblTotalVATExemptAmount - dblAdjRefund - dblAdjSRV + dblCharges;
                            dblTotalPayment = dblCash + dblCheck + dblDebitCard + dblCreditCard + dblSRV + dblOnCredit;
                            dblTotalVAT = dblVAT + dblVATAmount + dblNonVAT + dblVATExempt + dblZeroRatedVAT;

                            var strTotalSales = SetPrefixPostfix(AccountingFormat(dblTotalSales), intFAFCTotalLenght - 16, ' ', 0);
                            $('#lblGrossSales').text(strTotalSales);
                            pValues.push({
                                Key: 'GrossSales',
                                DataType: 'numeric',
                                Value: dblTotalSales
                            });    

                            //Sales Discount
                            var strSalesDiscount = SetPrefixPostfix(AccountingFormat(dblTotalSalesDiscount), intFAFCTotalLenght - 23, ' ', 0);
                            $('#lblTotalSalesDiscount').text(strSalesDiscount);
                            pValues.push({
                                Key: 'TotalSalesDiscount',
                                DataType: 'numeric',
                                Value: dblTotalSalesDiscount
                            });    

                            //VAT Exempt Tax
                            var strVATExemptAmount = SetPrefixPostfix(AccountingFormat(dblTotalVATExemptAmount), intFAFCTotalLenght - 23, ' ', 0);
                            $('#lblTotalVATExemptAmount').text(strVATExemptAmount);
                            pValues.push({
                                Key: 'TotalVATExemptAmount',
                                DataType: 'numeric',
                                Value: dblTotalVATExemptAmount
                            });    

                            //Sales Adjustment                            
                            var strAdjRefund = SetPrefixPostfix(AccountingFormat(dblAdjRefund), intFAFCTotalLenght - 29, ' ', 0);
                            $('#lblTotalSalesRefundforthePeriod').text(strAdjRefund);
                            pValues.push({
                                Key: 'TotalSalesRefundforthePeriod',
                                DataType: 'numeric',
                                Value: dblAdjRefund
                            });    

                            var strAdjSRV = SetPrefixPostfix(AccountingFormat(dblAdjSRV), intFAFCTotalLenght - 32, ' ', 0);
                            $('#lblTotalSalesReturnsSubjecttoMerchandiseExchange').text(strAdjSRV);
                            $('#lblSRVLiability').text(dblAdjSRV > 0 ? '-' + CStr2(strAdjSRV) : '-');

                            if (dblAdjSRV > 0) {
                                $('.divSRVLiability').show();
                            } else $('.divSRVLiability').hide();

                            pValues.push({
                                Key: 'TotalSalesReturnsSubjecttoMerchandiseExchange',
                                DataType: 'numeric',
                                Value: dblAdjSRV
                            });    

                            var strNetSalesAdj = SetPrefixPostfix(AccountingFormat(parseFloat(dblAdjRefund) + parseFloat(dblAdjSRV)), intFAFCTotalLenght - 23, ' ', 0);
                            $('#lblTotalNetofAdjustments').text(strNetSalesAdj);
                            pValues.push({
                                Key: 'TotalNetofAdjustments',
                                DataType: 'numeric',
                                Value: parseFloat(dblAdjRefund) + parseFloat(dblAdjSRV)
                            });    

                            var strNetSales = SetPrefixPostfix(AccountingFormat(dblNetSales), intFAFCTotalLenght - 16, ' ', 0);
                            $('#lblTotalNetSales').text(strNetSales);
                            pValues.push({
                                Key: 'TotalNetSales',
                                DataType: 'numeric',
                                Value: dblNetSales
                            });    

                            var strIssuedSRV = SetPrefixPostfix(AccountingFormat(dblSRV), intFAFCTotalLenght - 20, ' ', 0);
                            $('#lblTotalIssuedSRV').text(strIssuedSRV);
                            pValues.push({
                                Key: 'TotalIssuedSRV',
                                DataType: 'numeric',
                                Value: dblAdjSRV
                            });    

                            var strTotalSales = SetPrefixPostfix(AccountingFormat(dblNetSales + dblSRV), intFAFCTotalLenght - 21, ' ', 0);
                            $('#lblTotalNetSalesAdjustedforExchanges').text(strTotalSales);
                            pValues.push({
                                Key: 'TotalNetSalesAdjustedforExchanges',
                                DataType: 'numeric',
                                Value: dblNetSales + dblAdjSRV
                            });    

                            //END Sales Adjustment

                            //Sales Grouped By Mode of Payment

                            var strTotalCash = SetPrefixPostfix(AccountingFormat(dblCash), intFAFCTotalLenght - 16, ' ', 0);
                            $('#lblTotalCash').text(strTotalCash);
                            pValues.push({
                                Key: 'TotalCash',
                                DataType: 'numeric',
                                Value: dblCash
                            });                                

                            var strTotalCheck = SetPrefixPostfix(AccountingFormat(dblCheck), intFAFCTotalLenght - 16, ' ', 0);
                            $('#lblTotalCheck').text(strTotalCheck);
                            pValues.push({
                                Key: 'TotalCheck',
                                DataType: 'numeric',
                                Value: dblCheck
                            });                                

                            var strTotalDebitCard = SetPrefixPostfix(AccountingFormat(dblDebitCard), intFAFCTotalLenght - 16, ' ', 0);
                            $('#lblTotalDebitCard').text(strTotalDebitCard);
                            pValues.push({
                                Key: 'TotalDebitCard',
                                DataType: 'numeric',
                                Value: dblDebitCard
                            });                                

                            var strTotalCreditCard = SetPrefixPostfix(AccountingFormat(dblCreditCard), intFAFCTotalLenght - 16, ' ', 0);
                            $('#lblTotalCreditCard').text(strTotalCreditCard);
                            pValues.push({
                                Key: 'TotalCreditCard',
                                DataType: 'numeric',
                                Value: dblCreditCard
                            });                                

                            var strTotalSRV = SetPrefixPostfix(AccountingFormat(dblSRV), intFAFCTotalLenght - 16, ' ', 0);
                            $('#lblTotalSRV').text(strTotalSRV);
                            pValues.push({
                                Key: 'TotalSRV',
                                DataType: 'numeric',
                                Value: dblSRV
                            });                                

                            var strOnCredit = SetPrefixPostfix(AccountingFormat(dblOnCredit), intFAFCTotalLenght - 16, ' ', 0);
                            $('#lblOnCredit').text(strOnCredit);
                            pValues.push({
                                Key: 'OnCredit',
                                DataType: 'numeric',
                                Value: dblOnCredit
                            });                                

                            var strTotalEPlus = SetPrefixPostfix(AccountingFormat(dblEPlus), intFAFCTotalLenght - 16, ' ', 0);
                            $('#lblTotalEPlus').text(strTotalEPlus);
                            pValues.push({
                                Key: 'TotalEPlus',
                                DataType: 'numeric',
                                Value: dblEPlus
                            });                                

                            var strTotalSMAC = SetPrefixPostfix(AccountingFormat(dblSMAC), intFAFCTotalLenght - 16, ' ', 0);
                            $('#lblTotalSMAC').text(strTotalSMAC);
                            pValues.push({
                                Key: 'TotalSMAC',
                                DataType: 'numeric',
                                Value: dblSMAC
                            });                                

                            var strTotalGCash = SetPrefixPostfix(AccountingFormat(dblGCash), intFAFCTotalLenght - 16, ' ', 0);
                            $('#lblTotalGCash').text(strTotalGCash);
                            pValues.push({
                                Key: 'TotalGCash',
                                DataType: 'numeric',
                                Value: dblGCash
                            });                                

                            var strTotalVoucher = SetPrefixPostfix(AccountingFormat(dblVoucher), intFAFCTotalLenght - 16, ' ', 0);
                            $('#lblTotalVoucher').text(strTotalVoucher);
                            pValues.push({
                                Key: 'TotalVoucher',
                                DataType: 'numeric',
                                Value: dblVoucher
                            });                                

                            //May 14 2018 gi comment ang Other Tender kay magconflict sa G-Cash na payment type. pwede ang g-cash inside and outside sa mall
                            //var strTotalOther = SetPrefixPostfix(AccountingFormat(dblOtherPaymentType), intFAFCTotalLenght - 16, ' ', 0);
                            //$('#lblTotalOtherTender').text(strTotalOther);

                            //if (!CBool(sessionStorage.PrintingOfflineXReadData)) {
                            //    if (vDataDataSalesReportOtherPaymentInfo != undefined) {
                            //        if (vDataDataSalesReportOtherPaymentInfo.Rows.length > 0) {
                            //            var pOtherPayment = $('#tblOtherPaymentBody').empty()

                            //            $('#divOtherTenderDetails').css('display', 'block')
                            //            $('#divOtherTenderDetails').css('visibility', 'visible')

                            //            for (var otherpaymentcount = 0; otherpaymentcount <= vDataDataSalesReportOtherPaymentInfo.Rows.length - 1; otherpaymentcount++) {

                            //                var tData = $('<tr> ')
                            //                        .append($('<td style="text-align:left">')
                            //                            .text('     ' + vDataDataSalesReportOtherPaymentInfo.Rows[otherpaymentcount].PaymentType)
                            //                    )
                            //                        .append($('<td style="text-align:right">')
                            //                            .text(AccountingFormat(vDataDataSalesReportOtherPaymentInfo.Rows[otherpaymentcount].TotalAmount))
                            //                    )
                            //                pOtherPayment.append(tData);
                            //            }
                            //        } else {
                            //            $('#divOtherTenderDetails').css('display', 'none')
                            //            $('#divOtherTenderDetails').css('visibility', 'hidden')
                            //        }
                            //    }
                            //} else {

                            //    if (vDataDataSalesReportOtherPaymentInfo != undefined) {
                            //        var LastOtherPaymentTypesTotalData = JSON.parse(vDataDataSalesReportOtherPaymentInfo);

                            //        var pOtherPayment = $('#tblOtherPaymentBody').empty()

                            //        $('#divOtherTenderDetails').css('display', 'block')
                            //        $('#divOtherTenderDetails').css('visibility', 'visible')


                            //        if (LastOtherPaymentTypesTotalData != undefined) {
                            //            for (var otherpaymentcount = 0; otherpaymentcount <= Object.keys(LastOtherPaymentTypesTotalData).length - 1; otherpaymentcount++) {

                            //                var tData = $('<tr> ')
                            //                        .append($('<td style="text-align:left">')
                            //                            .text('     ' + Object.keys(LastOtherPaymentTypesTotalData)[otherpaymentcount])
                            //                    )
                            //                        .append($('<td style="text-align:right">')
                            //                            .text(AccountingFormat(LastOtherPaymentTypesTotalData[Object.keys(LastOtherPaymentTypesTotalData)[otherpaymentcount]]))
                            //                    )
                            //                pOtherPayment.append(tData);
                            //            }
                            //        } else {
                            //            $('#divOtherTenderDetails').css('display', 'none')
                            //            $('#divOtherTenderDetails').css('visibility', 'hidden')
                            //        }
                            //    }
                            //}
                            var pLV = $('#tblOtherPaymentBody').empty();
                            if (vDataDataSalesReportOtherPaymentInfo != undefined) {


                                if (vDataDataSalesReportOtherPaymentInfo.Rows.length > 0) {
                                    var mOtherPaymentTypeColumns = Object.entries(vDataDataSalesReportOtherPaymentInfo.Columns);
                                    
                                    var totalPotatoCornerGC = 0; //PC GC

                                    if (CBool(sessionStorage.PrintingOfflineXReadData)) {
                                        for (var o in vDataDataSalesReportOtherPaymentInfo.Rows) {
                                            for (var c in vDataDataSalesReportOtherPaymentInfo.Columns) {
                                                var opname = vDataDataSalesReportOtherPaymentInfo.Columns[c].replace('OtherPayment', '');
                                                var amt = CDec2(vDataDataSalesReportOtherPaymentInfo.Rows[o][vDataDataSalesReportOtherPaymentInfo.Columns[c]]);

                                                if (amt == 0) continue;

                                                if (opname.toLowerCase() == 'potatocornergc') {
                                                    //exclude anf GC ni Potato Corner and iadd sya sa original GC;
                                                    totalPotatoCornerGC += amt;
                                                    continue;
                                                } else {
                                                    dblTotalPayment += amt;
                                                    pItem = CreateOtherPaymentTypeRow(opname, amt);
                                                    pLV.append(pItem);
                                                }
                                            }
                                        }
                                    } else {
                                        for (var o in vDataDataSalesReportOtherPaymentInfo.Rows) {
                                            if (CDec2(vDataDataSalesReportOtherPaymentInfo.Rows[o].Amount) == 0) continue;

                                            dblTotalPayment += CDec2(vDataDataSalesReportOtherPaymentInfo.Rows[o].Amount);
                                            pItem = CreateOtherPaymentTypeRow(vDataDataSalesReportOtherPaymentInfo.Rows[o].PaymentType, CDec2(vDataDataSalesReportOtherPaymentInfo.Rows[o].Amount));
                                            pLV.append(pItem);
                                        }
                                    }                                   

                                    //iadd nato ang PC GC sa original GC
                                    $('#lblTotalCheck').text(AccountingFormat(CDec2($('#lblTotalCheck').text()) + totalPotatoCornerGC));

                                    

                                } else {
                                    
                                }
                            }

                            var strTotalCharges = SetPrefixPostfix(AccountingFormat(dblCharges), intFAFCTotalLenght - 16, ' ', 0);
                            $('#lblTotalCharges').text(strTotalCharges);
                            pValues.push({
                                Key: 'TotalCharges',
                                DataType: 'numeric',
                                Value: dblCharges
                            });                                

                            var strTotalClassPayment = SetPrefixPostfix(AccountingFormat(dblTotalPayment), intFAFCTotalLenght - 21, ' ', 0);
                            $('#lblTotalInventoryofPayments').text(strTotalClassPayment);
                            pValues.push({
                                Key: 'TotalInventoryofPayments',
                                DataType: 'numeric',
                                Value: dblTotalPayment
                            });                                

                            var dblTotalOverPayment = dblTotalPayment - (dblNetSales); //dblSRV

                            var strTotalOverPayment = SetPrefixPostfix(AccountingFormat(dblTotalOverPayment), intFAFCTotalLenght - 25, ' ', 0);
                            $('#lblExcessofInventoryofPaymentsvsNetSales').text(AccountingFormat(dblTotalPayment - dblAdjSRV));
                            pValues.push({
                                Key: 'ExcessofInventoryofPaymentsvsNetSales',
                                DataType: 'numeric',
                                Value: dblTotalOverPayment
                            });                                

                            //if (dblCharges > 0) {
                            //    $("#divCharges").show();
                            //} else {
                            //    $("#divCharges").hide();
                            //}

                            $("#divGCash").show();

                            if (IsFowstInSMMall) {
                                $("#divSMAC").show();
                                $("#divVoucher").show();
                                $("#divEPlus").show();
                                $("#divOtherTender").hide();
                            } else {
                                $("#divSMAC").hide();
                                $("#divVoucher").hide();
                                $("#divEPlus").hide();
                                $("#divOtherTender").show();
                            }

                            //END Sales Grouped By Mode of Payment

                            //Inventory of Payments Audit
                            $('#InventoryofPaymentsAudit').text('Inventory of Payments Audit');

                            $('#lblNetSalesAdjustedforExchanges').text(strTotalSales);
                            pValues.push({
                                Key: 'NetSalesAdjustedforExchanges',
                                DataType: 'numeric',
                                Value: dblOnCredit
                            });                                

                            $('#lblTotalInventoryofPayments2').text(strTotalClassPayment);

                            var dblNetSalesAdjustedLessTotalInvPayment = (dblNetSales + dblAdjSRV) - dblTotalPayment;
                            var strTotalNetSalesAdjustedLessTotalInvPayment = SetPrefixPostfix(AccountingFormat(dblNetSalesAdjustedLessTotalInvPayment), intFAFCTotalLenght - 13, ' ', 0);

                            $('#lblOverageLabel').text((dblNetSalesAdjustedLessTotalInvPayment < 0 ? ' Shortage' : ' Overage '));
                            pValues.push({
                                Key: 'OverageLabel',
                                DataType: 'string',
                                Value: dblNetSalesAdjustedLessTotalInvPayment < 0 ? ' Shortage' : ' Overage '
                            });                                
                            $('#lblOverage').text(strTotalNetSalesAdjustedLessTotalInvPayment);
                            pValues.push({
                                Key: 'Overage',
                                DataType: 'numeric',
                                Value: dblNetSalesAdjustedLessTotalInvPayment
                            });                                
                            //END Inventory of Payments Audit

                            $('#tBodyTurnOverAndDeposits').html('');

                            var TotalTurnOverAndDeposits = 0;

                            for (var toCounter = 0; toCounter <= vDataTurnOverAndDeposits.Rows.length-1; toCounter++) {
                                var tData = $('<tr> ')
                                                                .append($('<td style="text-align:left">')
                                                                    .text(vDataTurnOverAndDeposits.Rows[toCounter].IsTurnOver == 1 ? 'Turned Over' : 'Deposited')
                                                            )
                                                                .append($('<td style="text-align:left">')
                                                                    .text(vDataTurnOverAndDeposits.Rows[toCounter].DepositedTo)
                                                            )
                                                            .append($('<td style="text-align:right">')
                                                                    .text(AccountingFormat(vDataTurnOverAndDeposits.Rows[toCounter].TotalAmount))
                                                            )

                                $('#tBodyTurnOverAndDeposits').append(tData);

                                TotalTurnOverAndDeposits += CDec2(vDataTurnOverAndDeposits.Rows[toCounter].TotalAmount);
                            }

                            $('#lblTotalTotalTurnoversAndDeposits').text(AccountingFormat(TotalTurnOverAndDeposits));
                            pValues.push({
                                Key: 'TotalTurnoversAndDeposits',
                                DataType: 'numeric',
                                Value: TotalTurnOverAndDeposits
                            });                                

                            //Sales Grouped by VAT TYPE----------------------------
                            //if ((strFrom == strTo && $("#selPOSSalesReportRange").val() == "Day") ||
                            //            $("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                var dblTotalVATSalesPercent = (dblVAT / dblTotalVAT) * 100;
                                var dblTotalVATAmountPercent = (dblVATAmount / dblTotalVAT) * 100;
                                var dblTotalNonVATPercent = (dblNonVAT / dblTotalVAT) * 100;
                                var dblTotalVATExemptPercent = (dblVATExempt / dblTotalVAT) * 100;
                                var dblTotalZeroRatedVATPercent = (dblZeroRatedVAT / dblTotalVAT) * 100;
                                var dblTotalVATPercent = dblTotalVATSalesPercent + dblTotalVATAmountPercent + dblTotalNonVATPercent + dblTotalVATExemptPercent + dblTotalZeroRatedVATPercent;


                                var strTotalVATSales = SetPrefixPostfix(AccountingFormat(dblVAT), 14, ' ', 0);
                                var strTotalVATAmount = SetPrefixPostfix(AccountingFormat(dblVATAmount), 14, ' ', 0);
                                var strTotalNonVAT = SetPrefixPostfix(AccountingFormat(dblNonVAT), 14, ' ', 0);
                                var strTotalVATExempt = SetPrefixPostfix(AccountingFormat(dblVATExempt), 14, ' ', 0);
                                var strTotalZeroRatedVAT = SetPrefixPostfix(AccountingFormat(dblZeroRatedVAT), 14, ' ', 0);
                                var strTotalAllVAT = SetPrefixPostfix(AccountingFormat(dblTotalVAT), 14, ' ', 0);

                                var strTotalVATSalesPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATSalesPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                var strTotalVATAmountPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATAmountPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                var strTotalNonVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalNonVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                var strTotalVATExemptPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATExemptPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                var strTotalZeroRatedVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalZeroRatedVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                var strTotalAllVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);

                                if (IsVAT != undefined) {
                                    if (!CBool(IsVAT)) {
                                        strTotalVATSales = SetPrefixPostfix(AccountingFormat(0), 14, ' ', 0);
                                        strTotalVATAmount = SetPrefixPostfix(AccountingFormat(0), 14, ' ', 0);
                                        strTotalNonVAT = SetPrefixPostfix(AccountingFormat(CDec2(dblVAT) + CDec2(dblVATAmount)), 14, ' ', 0);

                                        strTotalVATSalesPercent = SetPrefixPostfix(AccountingFormat(0) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                        strTotalVATAmountPercent = SetPrefixPostfix(AccountingFormat(0) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                        strTotalNonVATPercent = SetPrefixPostfix(AccountingFormat(100) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    }
                                }

                                $('#lblVATableSales').text((CBool(vDataHeader.Rows[0].IsVAT) ? strTotalVATSales : '-'));
                                $('#lblVATableSalesPercent').text((CBool(vDataHeader.Rows[0].IsVAT) ? strTotalVATSalesPercent : '-%'));

                                $('#lblVATAmount').text((CBool(vDataHeader.Rows[0].IsVAT) ? strTotalVATAmount : '-'));
                                $('#lblVATAmountPercent').text((CBool(vDataHeader.Rows[0].IsVAT) ? strTotalVATAmountPercent : '-%'));

                                $('#lblNonVAT').text((CBool(vDataHeader.Rows[0].IsVAT) ? strTotalNonVAT : AccountingFormat(dblNetSales)));
                                $('#lblNonVATPercent').text((CBool(vDataHeader.Rows[0].IsVAT) ? strTotalNonVATPercent : '100%'));

                                $('#lblVATExempt').text((CBool(vDataHeader.Rows[0].IsVAT) ? strTotalVATExempt : '-'));
                                $('#lblVATExemptPercent').text((CBool(vDataHeader.Rows[0].IsVAT) ? strTotalVATExemptPercent : '-'));

                                $('#lblZeroRatedVAT').text((CBool(vDataHeader.Rows[0].IsVAT) ? strTotalZeroRatedVAT : '-'));
                                $('#lblZeroRatedVATPercent').text((CBool(vDataHeader.Rows[0].IsVAT) ? strTotalZeroRatedVATPercent : '-'));

                                $('#lblTotalAllVat').text((CBool(vDataHeader.Rows[0].IsVAT) ? strTotalAllVAT : AccountingFormat(dblNetSales)));
                                $('#lblTotalAllVatPercent').text((CBool(vDataHeader.Rows[0].IsVAT) ? strTotalAllVATPercent : '100%'));

                                pValues.push({
                                    Key: 'VAT-able Sales Amount',
                                    DataType: 'numeric',
                                    Value: dblVAT
                                });

                                pValues.push({
                                    Key: 'VAT-able Sales Percent',
                                    DataType: 'numeric',
                                    Value: dblTotalVATSalesPercent
                                });

                                pValues.push({
                                    Key: 'VAT Amount',
                                    DataType: 'numeric',
                                    Value: dblVATAmount
                                });

                                pValues.push({
                                    Key: 'VAT Percent',
                                    DataType: 'numeric',
                                    Value: dblTotalVATAmountPercent
                                });

                                pValues.push({
                                    Key: 'Non-VAT Amount',
                                    DataType: 'numeric',
                                    Value: dblNonVAT
                                });

                                pValues.push({
                                    Key: 'Non-VAT Percent',
                                    DataType: 'numeric',
                                    Value: dblTotalNonVATPercent
                                });

                                pValues.push({
                                    Key: 'VAT-Exempt Amount',
                                    DataType: 'numeric',
                                    Value: dblVATExempt
                                });

                                pValues.push({
                                    Key: 'VAT-Exempt Percent',
                                    DataType: 'numeric',
                                    Value: dblTotalVATExemptPercent
                                });

                                pValues.push({
                                    Key: 'Zero-Rated VAT Amount',
                                    DataType: 'numeric',
                                    Value: dblZeroRatedVAT
                                });

                                pValues.push({
                                    Key: 'Zero-Rated VAT Percent',
                                    DataType: 'numeric',
                                    Value: dblTotalZeroRatedVATPercent
                                });

                                pValues.push({
                                    Key: 'Total VAT Amount',
                                    DataType: 'numeric',
                                    Value: dblTotalVAT
                                });

                                pValues.push({
                                    Key: 'Total VAT Percent',
                                    DataType: 'numeric',
                                    Value: dblTotalVATPercent
                                });
                                //                                $('#lblTotalAllVatPercent').text(strTotalAllVATPercent)
                                //                                $('#lblVATableSalesPercent').text(strTotalVATSales);
                            //}
                            //End Sales Grouped by VAT TYPE----------------------------
                            $('#lblVoidedItemQty').text(intVoidedItemQty);
                            $('#lblTotalVoidAmount').text(AccountingFormat(dblTotalVoidAmount));
                            $('#lblDateandTime').text($.format.date(new Date(vDataHeader.Rows[0].PrintDateTime)));
                        } else {
                            //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);
                            PrintText(strPrinterIP);
                            ClearCommandBuffer();
                        }

                    } else {

                        $('#lblCashierLabel').hide();
                        $('#lblCashierName').css('visibility', 'hidden');
                        //$('#dvManagerSalesReportExcessSection').css('visibility', 'visible');
                        $('#dvManagerSalesReportExcessSection').css('display', 'block');
                        if (vDataInfo.Rows.length > 0) {


                            $('#lblReportType').text("MANAGER'S");

                            if ($("#selPOSSalesReportRange").val() == "Day") {
                                $('#lblReportRange').text("SALES REPORT");
                                $('#lblReportDate').text((strFrom == strTo ? 'Date: ' + strFrom : 'From ' + strFrom + ' To ' + strTo));

                            } else if ($("#selPOSSalesReportRange").val() == "Week") {

                                strPrint = '';
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, "Same-Store Comparative Sales Report", intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? 'Week: ' + strFrom : 'From Week ' + strFrom + ' To Week ' + strTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                            } else if ($("#selPOSSalesReportRange").val() == "Month") {

                                strPrint = '';
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, "Same-Store Comparative Sales Report", intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? 'Month: ' + $.format.date(new Date(strFrom + '/1/2013'), 'MMM') : 'From ' + $.format.date(new Date(strFrom + '/1/2013'), 'MMM') + ' To ' + $.format.date(new Date(strTo + '/1/2013'), 'MMM')), intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                            } else if ($("#selPOSSalesReportRange").val() == "Quarter") {

                                strPrint = '';
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, "Same-Store Comparative Sales Report", intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? (strFrom == '1' ? "1st Quarter" : (strFrom == '2' ? "2nd Quarter" : (strFrom == '3' ? "3rd Quarter" : (strFrom == '4' ? "4th Quarter" : "")))) : 'From ' + (strFrom == '1' ? "1st Quarter" : (strFrom == '2' ? "2nd Quarter" : (strFrom == '3' ? "3rd Quarter" : (strFrom == '4' ? "4th Quarter" : "")))) + ' To ' + (strTo == '1' ? "1st Quarter" : (strTo == '2' ? "2nd Quarter" : (strTo == '3' ? "3rd Quarter" : (strTo == '4' ? "4th Quarter" : ""))))), intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataInfo.Rows[0].YearFrom == vDataInfo.Rows[0].YearTo ? "Year : " + vDataInfo.Rows[0].YearFrom : "Year : " + vDataInfo.Rows[0].YearFrom + "-" + vDataInfo.Rows[0].YearTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                            } else if ($("#selPOSSalesReportRange").val() == "Year") {
                                strPrint = '';
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, "SALES REPORT", intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Year: ' + strYear, intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                            } else if ($("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {
                                $('#lblReportRange').text("SALES REPORT");

                                if (IsPrintingOfflineData) {
                                    //$('#lblReportDate').text('Date: From: ' + strFrom + ' To: ' + strTo);
                                    $('#lblReportDate').text((dateFrom == dateTo ? 'Date: ' + dateFrom : 'From ' + dateFrom + ' To ' + dateTo));
                                } else {
                                    $('#lblReportDate').text('Date : ' + $.format.date(new Date(vDataInfo.Rows[0].SalesDate), 'MM-dd-yyyy'));
                                    if (newRangeType == "Day") {
                                        $('#lblReportDate').text((dateFrom == dateTo ? 'Date: ' + dateFrom : 'Date: ' + dateFrom + ' to ' + dateTo));
                                    }
                                }
                            }

                            pValues.push({
                                Key: 'ReportType',
                                DataType: 'string',
                                Value: $('#lblReportType').text()
                            });
                            
                            pValues.push({
                                Key: 'ReportRange',
                                DataType: 'string',
                                Value: $('#lblReportRange').text()
                            });
                                
                            pValues.push({
                                Key: 'ReportDate',
                                DataType: 'string',
                                Value: $('#lblReportDate').text()
                            });    

                            var dblTotalSales = 0;
                            var dblTotalSalesDiscount = 0;
                            var dblTotalVATExemptAmount = 0;
                            var dblAdjRefund = 0;
                            var dblAdjSRV = 0;
                            var dblNetSales = 0;

                            var dblCash = 0;
                            var dblCheck = 0;
                            var dblDebitCard = 0;
                            var dblCreditCard = 0;
                            var dblSRV = 0;
                            var dblOnCredit = 0;
                            var dblOtherPaymentType = 0;
                            var dblCharges = 0;
                            var dblSMAC = 0;
                            var dblGCash = 0;
                            var dblEPlus = 0;
                            var dblVoucher = 0;
                            var dblTotalPayment = 0;

                            var dblVAT = 0;
                            var dblVATAmount = 0;
                            var dblNonVAT = 0;
                            var dblVATExempt = 0;
                            var dblZeroRatedVAT = 0;
                            var dblTotalVAT = 0;

                            var intTransCount = 0;
                            var intSalesTransCount = 0;
                            var intSalesItemCount = 0;
                            var intSRVTransCount = 0;
                            var intSRVItemCount = 0;
                            var intRefTransCount = 0;
                            var intRefItemCount = 0;

                            if (!IsPrintingOfflineData) {
                                for (var p = 0; p <= vDataInfo.Rows.length - 1; p++) {

                                    dblTotalSales = dblTotalSales + parseFloat(vDataInfo.Rows[p].CurGrossSales || 0);
                                    dblTotalSalesDiscount = dblTotalSalesDiscount + parseFloat(vDataInfo.Rows[p].CurSalesDiscount || 0) + parseFloat(vDataInfo.Rows[p].CurSCDiscount || 0);
                                    dblTotalVATExemptAmount = dblTotalVATExemptAmount + parseFloat(vDataInfo.Rows[p].CurVATExempt || 0);
                                    dblAdjRefund = dblAdjRefund + parseFloat(vDataInfo.Rows[p].CurSalesReturnRefund || 0);
                                    dblAdjSRV = dblAdjSRV + parseFloat(vDataInfo.Rows[p].CurSalesReturnExchange || 0);

                                    dblCash = dblCash + parseFloat(vDataInfo.Rows[p].CashAmount || 0);
                                    dblCheck = dblCheck + parseFloat(vDataInfo.Rows[p].CheckPaymentAmount || 0);
                                    dblDebitCard = dblDebitCard + parseFloat(vDataInfo.Rows[p].DebitCardAmount || 0);
                                    dblCreditCard = dblCreditCard + parseFloat(vDataInfo.Rows[p].CreditCardAmount || 0);
                                    dblSRV = dblSRV + parseFloat(vDataInfo.Rows[p].SRVAmount || 0);
                                    dblOnCredit = dblOnCredit + parseFloat(vDataInfo.Rows[p].OnCreditAmount || 0);
                                    dblOtherPaymentType = dblOtherPaymentType + parseFloat(vDataInfo.Rows[p].OtherTypeAmount || 0);
                                    dblCharges = dblCharges + parseFloat(vDataInfo.Rows[p].ChargeAmount || 0);
                                    dblSMAC = dblSMAC + parseFloat(vDataInfo.Rows[p].SMACAmount || 0);
                                    dblGCash = dblGCash + parseFloat(vDataInfo.Rows[p].GCashAmount || 0);
                                    dblEPlus = dblEPlus + parseFloat(vDataInfo.Rows[p].EPlusAmount || 0);
                                    dblVoucher = dblVoucher + parseFloat(vDataInfo.Rows[p].VoucherAmount || 0);


                                    dblVAT = dblVAT + parseFloat(vDataInfo.Rows[p].CurTotalVatablePortionOfSCDTransaction || 0) + parseFloat(vDataInfo.Rows[p].CurTotalVATSales || 0) - parseFloat(vDataInfo.Rows[p].CurTotalVATSalesSalesReturn || 0);
                                    dblVATAmount = dblVATAmount + parseFloat(vDataInfo.Rows[p].CurTotalVatAmountPortionOfSCDTransaction || 0) + parseFloat(vDataInfo.Rows[p].CurTotalVATAmount || 0) - parseFloat(vDataInfo.Rows[p].CurTotalVATAmountSalesReturn || 0);
                                    dblNonVAT = dblNonVAT + parseFloat(vDataInfo.Rows[p].CurNoTAXSales || 0);
                                    dblVATExempt = dblVATExempt + parseFloat(vDataInfo.Rows[p].CurVATExemptSales || 0);
                                    dblZeroRatedVAT = dblZeroRatedVAT + parseFloat(vDataInfo.Rows[p].CurZeroRatedVATSales || 0);

                                    intVoidedItemQty += parseFloat(vDataInfo.Rows[p].CurVoidItemQty || 0);
                                    dblTotalVoidAmount += parseFloat(vDataInfo.Rows[p].CurVoid || 0);

                                    intTransCount = intTransCount + parseFloat(vDataInfo.Rows[p].CurTransCountInv || 0) + parseFloat(vDataInfo.Rows[p].CurTransCountMemoReturnExchange || 0) + parseFloat(vDataInfo.Rows[p].CurTransCountMemoRefund || 0);
                                    intSalesTransCount = intSalesTransCount + parseFloat(vDataInfo.Rows[p].CurTransCountInv || 0);
                                    intSalesItemCount = intSalesItemCount + parseFloat(vDataInfo.Rows[p].CurTransInvItemCount || 0);
                                    intSRVTransCount = intSRVTransCount + parseFloat(vDataInfo.Rows[p].CurTransCountMemoReturnExchange || 0);
                                    intSRVItemCount = intSRVItemCount + parseFloat(vDataInfo.Rows[p].CurSalesReturnExchangeItemCount || 0);
                                    intRefTransCount = intRefTransCount + parseFloat(vDataInfo.Rows[p].CurTransCountMemoRefund || 0);
                                    intRefItemCount = intRefItemCount + parseFloat(vDataInfo.Rows[p].CurSalesRefundItemCount || 0);
                                    OffLineTotalNumberOfCustomers += parseFloat(vDataInfo.Rows[p].CurCustomerCount || 0);
                                };
                            }
                            else {
                                $('#btnRecomputeFromLocalDatabase').css('visibility', 'visible');
                                $('#btnRecomputeFromLocalDatabase').css('display', 'block');

                                dblTotalSales += OfflineTotalSales;
                                dblTotalSalesDiscount += OfflineTotalSalesDiscount;
                                //dblTotalVATExemptAmount += OfflineTotalVATExemptAmount;
                                dblAdjRefund += OfflineAdjRefund;
                                dblAdjSRV += OfflineAdjSRV;

                                dblCash += OfflineCash;
                                dblCheck += OffLineCheck;
                                dblDebitCard += OffLineDebitCard;
                                dblCreditCard += OffLineCreditCard;
                                dblSRV += OffLineSRV;
                                dblOtherPaymentType += OffLineOtherPayment;
                                dblOnCredit += OffLineOnCredit;
                                dblCharges += OffLineCharges;

                                dblVAT += OffLineVAT;
                                dblVATAmount += OffLineVATAmount;
                                dblNonVAT += OffLineNonVAT;
                                dblVATExempt += OffLineVATExempt;
                                dblZeroRatedVAT += OffLineZeroRatedVAT;
                                dblTotalVATExemptAmount += OffLineForeGoneVAT;

                                intVoidedItemQty += OffLineVoidedItemQty;
                                dblTotalVoidAmount += OffLineTotalVoidAmount;

                                intTransCount = OfflineSalesTransCount;
                                intSalesItemCount = OfflineSalesItemCount;
                            }
                            dblCash = dblCash - dblAdjRefund;
                            dblNetSales = dblTotalSales - dblTotalSalesDiscount - dblTotalVATExemptAmount - dblAdjRefund - dblAdjSRV + dblCharges;
                            dblTotalPayment = dblCash + dblCheck + dblDebitCard + dblCreditCard + dblSRV + dblOtherPaymentType + dblOnCredit;
                            dblTotalVAT = dblVAT + dblVATAmount + dblNonVAT + dblVATExempt + dblZeroRatedVAT;

                            var strTotalSales = SetPrefixPostfix(AccountingFormat(dblTotalSales), intFAFCTotalLenght - 16, ' ', 0);
                            $('#lblGrossSales').text(strTotalSales);
                            pValues.push({
                                Key: 'GrossSales',
                                DataType: 'numeric',
                                Value: dblTotalSales
                            });   

                            //Sales Discount
                            var strSalesDiscount = SetPrefixPostfix(AccountingFormat(dblTotalSalesDiscount), intFAFCTotalLenght - 29, ' ', 0);
                            $('#lblTotalSalesDiscount').text(strSalesDiscount);
                            pValues.push({
                                Key: 'TotalSalesDiscount',
                                DataType: 'numeric',
                                Value: dblTotalSalesDiscount
                            });    

                            //VAT Exempt
                            var strVATExemptAmount = SetPrefixPostfix(AccountingFormat(dblTotalVATExemptAmount), intFAFCTotalLenght - 29, ' ', 0);
                            $('#lblTotalVATExemptAmount').text(strVATExemptAmount);
                            pValues.push({
                                Key: 'TotalVATExemptAmount',
                                DataType: 'numeric',
                                Value: dblTotalVATExemptAmount
                            });    

                            //Sales Adjustment
                            strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Summary of Adjustments', intFAFCTotalLenght, '', 0, '', 0, 1);

                            var strAdjRefund = SetPrefixPostfix(AccountingFormat(dblAdjRefund), intFAFCTotalLenght - 29, ' ', 0);
                            $('#lblTotalSalesRefundforthePeriod').text(strAdjRefund);
                            pValues.push({
                                Key: 'TotalSalesRefundforthePeriod',
                                DataType: 'numeric',
                                Value: dblAdjRefund
                            });    

                            var strAdjSRV = SetPrefixPostfix(AccountingFormat(dblAdjSRV), intFAFCTotalLenght - 32, ' ', 0);
                            $('#lblTotalSalesReturnsSubjecttoMerchandiseExchange').text(strAdjSRV);                            
                            pValues.push({
                                Key: 'TotalSalesReturnsSubjecttoMerchandiseExchange',
                                DataType: 'numeric',
                                Value: dblAdjSRV
                            });    

                            var strNetSalesAdj = SetPrefixPostfix(AccountingFormat(parseFloat(dblAdjRefund) + parseFloat(dblAdjSRV)), intFAFCTotalLenght - 23, ' ', 0);
                            $('#lblTotalNetofAdjustments').text(strNetSalesAdj);
                            pValues.push({
                                Key: 'TotalNetofAdjustments',
                                DataType: 'numeric',
                                Value: parseFloat(dblAdjRefund) + parseFloat(dblAdjSRV)
                            });    

                            var strNetSales = SetPrefixPostfix(AccountingFormat(dblNetSales), intFAFCTotalLenght - 16, ' ', 0);
                            $('#lblTotalNetSales').text(strNetSales);
                            pValues.push({
                                Key: 'TotalNetSales',
                                DataType: 'numeric',
                                Value: dblNetSales
                            });    

                            var strIssuedSRV = SetPrefixPostfix(AccountingFormat(dblAdjSRV), intFAFCTotalLenght - 22, ' ', 0);
                            $('#lblTotalIssuedSRV').text(strIssuedSRV);
                            pValues.push({
                                Key: 'TotalIssuedSRV',
                                DataType: 'numeric',
                                Value: dblAdjSRV
                            });    

                            var strTotalSales = SetPrefixPostfix(AccountingFormat(dblNetSales + dblAdjSRV), intFAFCTotalLenght - 21, ' ', 0);
                            $('#lblTotalNetSalesAdjustedforExchanges').text(strTotalSales);
                            pValues.push({
                                Key: 'TotalNetSalesAdjustedforExchanges',
                                DataType: 'numeric',
                                Value: dblNetSales + dblAdjSRV
                            });    

                            //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' for Exchanges', intFAFCTotalLenght, '', 0, '', 0, 1);
                            //                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                            //                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                            //                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                            strPrint = FormatTextToPrint(intFAFCTotalLenght, '', 0, '', 0, strTotalSales, intFAFCTotalLenght, 1);
                            //ReadyTextToPrint(strPrint, 3, 2, 2, 0, 1, 1, 0, 0, 0);

                            //END Sales Adjustment

                            //Sales Grouped By Mode of Payment
                            strPrint = FormatTextToPrint(intFAFCTotalLenght, "Inventory of Payments", intFAFCTotalLenght, '', 0, '', 0, 0);
                            //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                            //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                            //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                            var strTotalCash = SetPrefixPostfix(AccountingFormat(dblCash), intFAFCTotalLenght - 16, ' ', 0);
                            $('#lblTotalCash').text(strTotalCash);
                            pValues.push({
                                Key: 'TotalCash',
                                DataType: 'numeric',
                                Value: dblCash
                            });                         

                            var strTotalCheck = SetPrefixPostfix(AccountingFormat(dblCheck), intFAFCTotalLenght - 16, ' ', 0);
                            $('#lblTotalCheck').text(strTotalCheck);
                            pValues.push({
                                Key: 'TotalCheck',
                                DataType: 'numeric',
                                Value: dblCheck
                            });                      

                            var strTotalDebitCard = SetPrefixPostfix(AccountingFormat(dblDebitCard), intFAFCTotalLenght - 16, ' ', 0);
                            $('#lblTotalDebitCard').text(strTotalDebitCard);
                            pValues.push({
                                Key: 'TotalDebitCard',
                                DataType: 'numeric',
                                Value: dblDebitCard
                            });                          

                            var strTotalCreditCard = SetPrefixPostfix(AccountingFormat(dblCreditCard), intFAFCTotalLenght - 16, ' ', 0);
                            $('#lblTotalCreditCard').text(strTotalCreditCard);
                            pValues.push({
                                Key: 'TotalCreditCard',
                                DataType: 'numeric',
                                Value: dblCreditCard
                            });                 

                            var strTotalSRV = SetPrefixPostfix(AccountingFormat(dblSRV), intFAFCTotalLenght - 16, ' ', 0);
                            $('#lblTotalSRV').text(strTotalSRV);
                            pValues.push({
                                Key: 'TotalSRV',
                                DataType: 'numeric',
                                Value: dblSRV
                            });                                

                            var strOnCredit = SetPrefixPostfix(AccountingFormat(dblOnCredit), intFAFCTotalLenght - 16, ' ', 0);
                            $('#lblOnCredit').text(strOnCredit);
                            pValues.push({
                                Key: 'OnCredit',
                                DataType: 'numeric',
                                Value: dblOnCredit
                            });                

                            var strTotalVoucher = SetPrefixPostfix(AccountingFormat(dblVoucher), intFAFCTotalLenght - 16, ' ', 0);
                            $('#lblTotalVoucher').text(strTotalVoucher);
                            pValues.push({
                                Key: 'TotalVoucher',
                                DataType: 'numeric',
                                Value: dblVoucher
                            });                                

                            //May 14 2018 gi comment ang Other Tender kay magconflict sa G-Cash na payment type. pwede ang g-cash inside and outside sa mall
                            //var strTotalOther = SetPrefixPostfix(AccountingFormat(dblOtherPaymentType), intFAFCTotalLenght - 16, ' ', 0);
                            //$('#lblTotalOtherTender').text(strTotalOther);

                            //var strTotalEPlus = SetPrefixPostfix(AccountingFormat(dblEPlus), intFAFCTotalLenght - 16, ' ', 0);
                            //$('#lblTotalEPlus').text(strTotalEPlus);

                            //var strTotalSMAC = SetPrefixPostfix(AccountingFormat(dblSMAC), intFAFCTotalLenght - 16, ' ', 0);
                            //$('#lblTotalSMAC').text(strTotalSMAC);

                            var strTotalGCash = SetPrefixPostfix(AccountingFormat(dblGCash), intFAFCTotalLenght - 16, ' ', 0);
                            $('#lblTotalGCash').text(strTotalGCash);
                            pValues.push({
                                Key: 'TotalGCash',
                                DataType: 'numeric',
                                Value: dblGCash
                            });          
                            
                            var pOtherPayment = $('#tblOtherPaymentBody').empty();

                            if (!CBool(sessionStorage.PrintingOfflineXReadData)) {
                                if (vDataDataSalesReportOtherPaymentInfo != undefined) {

                                    if (vDataDataSalesReportOtherPaymentInfo.Rows.length > 0) {
                                        $('#divOtherPayment').css('display', 'block')
                                        $('#divOtherPayment').css('visibility', 'visible')

                                        $('#divOtherTenderDetails').css('display', 'block')
                                        $('#divOtherTenderDetails').css('visibility', 'visible')

                                        for (var otherpaymentcount = 0; otherpaymentcount <= vDataDataSalesReportOtherPaymentInfo.Rows.length - 1; otherpaymentcount++) {

                                            var tData = $('<tr> ')
                                                    .append($('<td style="text-align:left">')
                                                        .text('     ' + vDataDataSalesReportOtherPaymentInfo.Rows[otherpaymentcount].PaymentType)
                                                )
                                                    .append($('<td style="text-align:right">')
                                                        .text(AccountingFormat(vDataDataSalesReportOtherPaymentInfo.Rows[otherpaymentcount].TotalAmount))
                                                )
                                            pOtherPayment.append(tData);
                                        }
                                    }
                                } else {
                                    $('#divOtherPayment').css('display', 'none')
                                    $('#divOtherPayment').css('visibility', 'hidden')

                                    $('#divOtherTenderDetails').css('display', 'none')
                                    $('#divOtherTenderDetails').css('visibility', 'hidden')
                                }
                            }else{

                                if (vDataDataSalesReportOtherPaymentInfo != undefined) {

                                    var LastOtherPaymentTypesTotalData = JSON.parse(vDataDataSalesReportOtherPaymentInfo);

                                    if (LastOtherPaymentTypesTotalData != undefined) {
                                        $('#divOtherPayment').css('display', 'block')
                                        $('#divOtherPayment').css('visibility', 'visible')

                                        $('#divOtherTenderDetails').css('display', 'block')
                                        $('#divOtherTenderDetails').css('visibility', 'visible')

                                        for (var otherpaymentcount = 0; otherpaymentcount <= Object.keys(LastOtherPaymentTypesTotalData).length-1; otherpaymentcount++) {

                                            var tData = $('<tr> ')
                                                    .append($('<td style="text-align:left">')
                                                        .text('     ' + Object.keys(LastOtherPaymentTypesTotalData)[otherpaymentcount])
                                                )
                                                    .append($('<td style="text-align:right">')
                                                        .text(AccountingFormat(LastOtherPaymentTypesTotalData[Object.keys(LastOtherPaymentTypesTotalData)[otherpaymentcount]]))
                                                )
                                            pOtherPayment.append(tData);
                                        }
                                    } else {
                                        $('#divOtherPayment').css('display', 'none')
                                        $('#divOtherPayment').css('visibility', 'hidden')

                                        $('#divOtherTenderDetails').css('display', 'none')
                                        $('#divOtherTenderDetails').css('visibility', 'hidden')
                                    }
                                }
                            }
                            var strTotalCharges = SetPrefixPostfix(AccountingFormat(dblCharges), intFAFCTotalLenght - 16, ' ', 0);
                            $('#lblTotalCharges').text(strTotalCharges);
                            pValues.push({
                                Key: 'TotalCharges',
                                DataType: 'numeric',
                                Value: dblCharges
                            });                                

                            var strTotalClassPayment = SetPrefixPostfix(AccountingFormat(dblTotalPayment), intFAFCTotalLenght - 20, ' ', 0);
                            $('#lblTotalInventoryofPayments').text(strTotalClassPayment);
                            pValues.push({
                                Key: 'TotalInventoryofPayments',
                                DataType: 'numeric',
                                Value: dblTotalPayment
                            });                                

                            strPrint = FormatTextToPrint(intFAFCTotalLenght, '', 0, '', 0, strTotalClassPayment, intFAFCTotalLenght, 1);
                            //ReadyTextToPrint(strPrint, 3, 2, 2, 0, 1, 1, 0, 0, 0);

                            var dblTotalOverPayment = dblTotalPayment - dblNetSales - dblAdjSRV;
                            //////                                                    var strTotalOverPayment = SetPrefixPostfix(AccountingFormat((dblTotalOverPayment < 0 ? (dblTotalOverPayment * -1) : dblTotalOverPayment)), intFAFCTotalLenght - 25, ' ', 0);
                            var strTotalOverPayment = SetPrefixPostfix(AccountingFormat(dblTotalOverPayment), intFAFCTotalLenght - 25, ' ', 0);
                            $('#lblNetSalesAdjustedforExchanges').text(strTotalSales);
                            $('#lblExcessofInventoryofPaymentsvsNetSales').text(strTotalOverPayment);
                            pValues.push({
                                Key: 'ExcessofInventoryofPaymentsvsNetSales',
                                DataType: 'numeric',
                                Value: dblTotalOverPayment
                            });                                

                            strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Payments vs Net Sales', intFAFCTotalLenght, '', 0, '', 0, 1);
                            //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                            if (dblCharges > 0) {
                                $("#divCharges").show();
                            } else {
                                $("#divCharges").hide();
                            }

                            $("#divGCash").show();

                            if (IsFowstInSMMall) {
                                $("#divSMAC").show();
                                $("#divVoucher").show();
                                $("#divEPlus").show();
                                $("#divOtherTender").hide();
                            } else {
                                $("#divSMAC").hide();
                                $("#divVoucher").hide();
                                $("#divEPlus").hide();
                                $("#divOtherTender").show();
                            }

                            //END Sales Grouped By Mode of Payment

                            //Inventory of Payments Audit

                            $('#InventoryofPaymentsAudit').text('Sales Groupe: by VAT Type');

                            $('#lblTotalInventoryofPayments2').text(strTotalClassPayment);
                            var dblNetSalesAdjustedLessTotalInvPayment = (dblNetSales + dblAdjSRV) - dblTotalPayment;
                            var strTotalNetSalesAdjustedLessTotalInvPayment = SetPrefixPostfix(AccountingFormat(dblNetSalesAdjustedLessTotalInvPayment), intFAFCTotalLenght - 13, ' ', 0);
                            $('#lblOverageLabel').text((dblNetSalesAdjustedLessTotalInvPayment < 0 ? ' Shortage' : ' Overage '));
                            pValues.push({
                                Key: 'OverageLabel',
                                DataType: 'string',
                                Value: dblNetSalesAdjustedLessTotalInvPayment < 0 ? ' Shortage' : ' Overage '
                            });                
                            $('#lblOverage').text(strTotalNetSalesAdjustedLessTotalInvPayment);
                            pValues.push({
                                Key: 'Overage',
                                DataType: 'numeric',
                                Value: dblNetSalesAdjustedLessTotalInvPayment
                            });                          
                            //END Inventory of Payments Audit

                            //Sales Grouped by VAT TYPE----------------------------
                            if ((strFrom == strTo && $("#selPOSSalesReportRange").val() == "Day") ||
                                $("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                strPrint = '';
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, "Sales Grouped by VAT Type", intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                var dblTotalVATSalesPercent = (dblVAT / dblTotalVAT) * 100;
                                var dblTotalVATAmountPercent = (dblVATAmount / dblTotalVAT) * 100;
                                var dblTotalNonVATPercent = (dblNonVAT / dblTotalVAT) * 100;
                                var dblTotalVATExemptPercent = (dblVATExempt / dblTotalVAT) * 100;
                                var dblTotalZeroRatedVATPercent = (dblZeroRatedVAT / dblTotalVAT) * 100;
                                var dblTotalVATPercent = dblTotalVATSalesPercent + dblTotalVATAmountPercent + dblTotalNonVATPercent + dblTotalVATExemptPercent + dblTotalZeroRatedVATPercent;

                                var strTotalVATSales = SetPrefixPostfix(AccountingFormat(dblVAT), 14, ' ', 0);
                                var strTotalVATAmount = SetPrefixPostfix(AccountingFormat(dblVATAmount), 14, ' ', 0);
                                var strTotalNonVAT = SetPrefixPostfix(AccountingFormat(dblNonVAT), 14, ' ', 0);
                                var strTotalVATExempt = SetPrefixPostfix(AccountingFormat(dblVATExempt), 14, ' ', 0);
                                var strTotalZeroRatedVAT = SetPrefixPostfix(AccountingFormat(dblZeroRatedVAT), 14, ' ', 0);
                                var strTotalAllVAT = SetPrefixPostfix(AccountingFormat(dblTotalVAT), 14, ' ', 0);

                                var strTotalVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATSalesPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                var strTotalVATAmountPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATAmountPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                var strTotalNonVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalNonVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                var strTotalVATExemptPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATExemptPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                var strTotalZeroRatedVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalZeroRatedVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                var strTotalAllVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);

                                if (IsVAT != undefined) {
                                    if (!CBool(IsVAT)) {
                                        strTotalVATSales = SetPrefixPostfix(AccountingFormat(0), 14, ' ', 0);
                                        strTotalVATAmount = SetPrefixPostfix(AccountingFormat(), 14, ' ', 0);
                                        strTotalNonVAT = SetPrefixPostfix(AccountingFormat(CDec2(dblVAT) + CDec2(dblVATAmount)), 14, ' ', 0);

                                        strTotalVATPercent = SetPrefixPostfix(AccountingFormat(0) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                        strTotalVATAmountPercent = SetPrefixPostfix(AccountingFormat(0) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                        strTotalNonVATPercent = SetPrefixPostfix(AccountingFormat(100) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    }
                                }

                                strPrint = ' ';
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-able Sales', 14, strTotalVATSales, 30, strTotalVATPercent, intFAFCTotalLenght, 1);
                                strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT Amount', 14, strTotalVATAmount, 30, strTotalVATAmountPercent, intFAFCTotalLenght, 1);
                                strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Non-VAT', 14, strTotalNonVAT, 30, strTotalNonVATPercent, intFAFCTotalLenght, 1);
                                strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-Exempt', 14, strTotalVATExempt, 30, strTotalVATExemptPercent, intFAFCTotalLenght, 1);
                                strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Zero-Rated VAT', 14, strTotalZeroRatedVAT, 30, strTotalZeroRatedVATPercent, intFAFCTotalLenght, 1);
                                strPrint = strPrint + strTemp;

                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '              ', 14, strTotalAllVAT, 30, strTotalAllVATPercent, intFAFCTotalLenght, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                            }
                            //End Sales Grouped by VAT TYPE----------------------------

                            //Get Sales by Cashier
                            if ((strFrom == strTo && $("#selPOSSalesReportRange").val() == "Day") ||
                                                $("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                if (!CBool(sessionStorage.PrintingOfflineXReadData)) {
                                    if (vDataSalesByCashier.Rows.length > 0) {

                                        strPrint = '';
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, "Sales Grouped by Cashier", intFAFCTotalLenght, '', 0, '', 0, 0);
                                        strPrint = strPrint + strTemp;

                                        //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                        //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                        //Get Total Sales
                                        var dblAllTotalSales = 0;
                                        for (var p = 0; p <= vDataSalesByCashier.Rows.length - 1; p++) {
                                            dblAllTotalSales = dblAllTotalSales + parseFloat(vDataSalesByCashier.Rows[p].TotalSales);
                                        };

                                        var dblAllTotalSalesPercent = 0;

                                        strPrint = '';

                                        for (var p = 0; p <= vDataSalesByCashier.Rows.length - 1; p++) {

                                            var strTotalSales = SetPrefixPostfix(AccountingFormat((vDataSalesByCashier.Rows[p].TotalSales || 0)), 14, ' ', 0);
                                            var dblTotalSalesPercent = (parseFloat(vDataSalesByCashier.Rows[p].TotalSales || 0) / dblAllTotalSales) * 100;
                                            var strTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                            dblAllTotalSalesPercent = dblAllTotalSalesPercent + dblTotalSalesPercent;

                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, vDataSalesByCashier.Rows[p].CashierName, 14, strTotalSales, 30, strTotalSalesPercent, intFAFCTotalLenght, 0);
                                            strPrint = strPrint + strTemp;

                                        };

                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                        var strAllTotalSales = SetPrefixPostfix(AccountingFormat(dblAllTotalSales), 14, ' ', 0);
                                        var strAllTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblAllTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                        strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                        strPrint = FormatTextToPrint(intFAFCTotalLenght, '              ', 15, strAllTotalSales, 31, strAllTotalSalesPercent, intFAFCTotalLenght, 1);
                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                    }
                                }
                            }

                            //Get Sales by Machine
                            if ((strFrom == strTo && $("#selPOSSalesReportRange").val() == "Day") ||
                                            $("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                if (!CBool(sessionStorage.PrintingOfflineXReadData)) {
                                    if (vDataSalesByMachine.Rows.length > 0) {

                                        strPrint = '';
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, "Sales Grouped by Machine (MIN)", intFAFCTotalLenght, '', 0, '', 0, 0);
                                        strPrint = strPrint + strTemp;

                                        //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                        //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                        //Get Total Sales
                                        dblAllTotalSales = 0;
                                        for (var p = 0; p <= vDataSalesByMachine.Rows.length - 1; p++) {
                                            dblAllTotalSales = dblAllTotalSales + parseFloat(vDataSalesByMachine.Rows[p].TotalSales);
                                        };

                                        dblAllTotalSalesPercent = 0;

                                        strPrint = '';
                                        for (var p = 0; p <= vDataSalesByMachine.Rows.length - 1; p++) {

                                            var strTotalSales = SetPrefixPostfix(AccountingFormat((vDataSalesByMachine.Rows[p].TotalSales || 0)), 14, ' ', 0);
                                            var dblTotalSalesPercent = (parseFloat(vDataSalesByMachine.Rows[p].TotalSales || 0) / dblAllTotalSales) * 100;
                                            var strTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                            dblAllTotalSalesPercent = dblAllTotalSalesPercent + dblTotalSalesPercent;

                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, vDataSalesByMachine.Rows[p].DeviceName, 14, strTotalSales, 30, strTotalSalesPercent, intFAFCTotalLenght, 0);
                                            strPrint = strPrint + strTemp;

                                        };

                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                        strAllTotalSales = SetPrefixPostfix(AccountingFormat(dblAllTotalSales), 14, ' ', 0);
                                        strAllTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblAllTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                        strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                        strPrint = FormatTextToPrint(intFAFCTotalLenght, '              ', 15, strAllTotalSales, 31, strAllTotalSalesPercent, intFAFCTotalLenght, 1);
                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                    }
                                }
                            }

                            //Get Sales by Cashier and Machine
                            if ((strFrom == strTo && $("#selPOSSalesReportRange").val() == "Day") ||
                                            $("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                if (!CBool(sessionStorage.PrintingOfflineXReadData)) {
                                    if (vDataSalesByCashierMachine.Rows.length > 0) {

                                        strPrint = '';
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, "Sales Grouped by Cashier & Machine (MIN)", intFAFCTotalLenght, '', 0, '', 0, 0);
                                        strPrint = strPrint + strTemp;

                                        //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                        //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                        //Get Total Sales
                                        dblAllTotalSales = 0;
                                        for (var p = 0; p <= vDataSalesByCashierMachine.Rows.length - 1; p++) {
                                            dblAllTotalSales = dblAllTotalSales + parseFloat(vDataSalesByCashierMachine.Rows[p].TotalSales);
                                        };

                                        dblAllTotalSalesPercent = 0;

                                        var strCashierName = "";
                                        var strDeviceName = "";
                                        var dblDeviceSales = 0;

                                        dblAllTotalSalesPercent = 0;

                                        strPrint = '';
                                        for (var p = 0; p <= vDataSalesByCashierMachine.Rows.length - 1; p++) {

                                            if (strCashierName != vDataSalesByCashierMachine.Rows[p].CashierName) {
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, vDataSalesByCashierMachine.Rows[p].CashierName, intFAFCTotalLenght, '', 0, '', 0, 0);
                                                strPrint = strPrint + strTemp;

                                                strCashierName = vDataSalesByCashierMachine.Rows[p].CashierName;
                                            }

                                            dblDeviceSales = parseFloat(vDataSalesByCashierMachine.Rows[p].TotalSales || 0);
                                            strTotalSales = SetPrefixPostfix(AccountingFormat(dblDeviceSales), 14, ' ', 0);
                                            dblTotalSalesPercent = (dblDeviceSales / dblAllTotalSales) * 100;
                                            strTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                            dblAllTotalSalesPercent = dblAllTotalSalesPercent + dblTotalSalesPercent;

                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, '     ' + vDataSalesByCashierMachine.Rows[p].DeviceName, 14, strTotalSales, 30, strTotalSalesPercent, intFAFCTotalLenght, 0);
                                            strPrint = strPrint + strTemp;

                                        };

                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                        strAllTotalSales = SetPrefixPostfix(AccountingFormat(dblAllTotalSales), 14, ' ', 0);
                                        strAllTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblAllTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                        strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                        strPrint = FormatTextToPrint(intFAFCTotalLenght, '              ', 15, strAllTotalSales, 31, strAllTotalSalesPercent, intFAFCTotalLenght, 1);
                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                    }
                                }
                            }

                            if ($("#selPOSSalesReportRange").val() == "Day" ||
                                    $("#selPOSSalesReportRange").val() == "Today" ||
                                    $("#selPOSSalesReportRange").val() == "Yesterday") {

                                //Other Informatoin
                                strPrint = '';
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, "Other Information", intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                var strTransCount = SetPrefixPostfix(intTransCount + '', intFAFCTotalLenght - 21, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Trans. Count', 19, strTransCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strSalesTransCount = SetPrefixPostfix(intTransCount + '', intFAFCTotalLenght - 21, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Sales Trans. Count', 19, strSalesTransCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strSalesItemCount = SetPrefixPostfix(intSalesItemCount + '', intFAFCTotalLenght - 24, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Sales Line Item Count', 22, strSalesItemCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strSRVTransCount = SetPrefixPostfix(intSRVTransCount + '', intFAFCTotalLenght - 19, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SRV Trans. Count', 17, strSRVTransCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strSRVItemCount = SetPrefixPostfix(intSRVItemCount + '', intFAFCTotalLenght - 22, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SRV Line Item Count', 20, strSRVItemCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strRefTransCount = SetPrefixPostfix(intRefTransCount + '', intFAFCTotalLenght - 22, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Refund Trans. Count', 20, strRefTransCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strRefItemCount = SetPrefixPostfix(intRefItemCount + '', intFAFCTotalLenght - 25, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Refund Line Item Count', 23, strRefItemCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strCustomerCount = SetPrefixPostfix(intSalesTransCount + '', intFAFCTotalLenght - 17, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Customer', 15, strCustomerCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                //END Other Informatoin

                                strPrint = '';
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Printed by ', 15, ': ' + (vDataHeader.Rows[0].PrintedBy || ''), intFAFCTotalLenght, '', 0, 1);
                                strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataHeader.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                strPrint = strPrint + strTemp;

                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);


                            } else {
                                strPrint = '';
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Printed by ', 15, ': ' + (vDataHeader.Rows[0].PrintedBy || ''), intFAFCTotalLenght, '', 0, 1);
                                strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataHeader.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                strPrint = strPrint + strTemp;

                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);

                                PrintText(strPrinterIP);
                                ClearCommandBuffer();
                            }
                            //-------------------

                            var dblTotalVATSalesPercent = (dblVAT / dblTotalVAT) * 100;
                            var dblTotalVATAmountPercent = (dblVATAmount / dblTotalVAT) * 100;
                            var dblTotalNonVATPercent = (dblNonVAT / dblTotalVAT) * 100;
                            var dblTotalVATExemptPercent = (dblVATExempt / dblTotalVAT) * 100;
                            var dblTotalZeroRatedVATPercent = (dblZeroRatedVAT / dblTotalVAT) * 100;
                            var dblTotalVATPercent = dblTotalVATSalesPercent + dblTotalVATAmountPercent + dblTotalNonVATPercent + dblTotalVATExemptPercent + dblTotalZeroRatedVATPercent;

                            var strTotalVATSales = SetPrefixPostfix(AccountingFormat(dblVAT), 14, ' ', 0);
                            var strTotalVATAmount = SetPrefixPostfix(AccountingFormat(dblVATAmount), 14, ' ', 0);
                            var strTotalNonVAT = SetPrefixPostfix(AccountingFormat(dblNonVAT), 14, ' ', 0);
                            var strTotalVATExempt = SetPrefixPostfix(AccountingFormat(dblVATExempt), 14, ' ', 0);
                            var strTotalZeroRatedVAT = SetPrefixPostfix(AccountingFormat(dblZeroRatedVAT), 14, ' ', 0);
                            var strTotalAllVAT = SetPrefixPostfix(AccountingFormat(dblTotalVAT), 14, ' ', 0);

                            var strTotalVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATSalesPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                            var strTotalVATAmountPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATAmountPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                            var strTotalNonVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalNonVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                            var strTotalVATExemptPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATExemptPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                            var strTotalZeroRatedVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalZeroRatedVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                            var strTotalAllVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);

                            if (IsVAT != undefined) {
                                if (!CBool(IsVAT)) {
                                    strTotalVATSales = SetPrefixPostfix(AccountingFormat(0), 14, ' ', 0);
                                    strTotalVATAmount = SetPrefixPostfix(AccountingFormat(0), 14, ' ', 0);
                                    strTotalNonVAT = SetPrefixPostfix(AccountingFormat(CDec2(dblVAT) + CDec2(dblVATAmount)), 14, ' ', 0);

                                    strTotalVATPercent = SetPrefixPostfix(AccountingFormat(0) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    strTotalVATAmountPercent = SetPrefixPostfix(AccountingFormat(0) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    strTotalNonVATPercent = SetPrefixPostfix(AccountingFormat(100) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                }
                            }

                            $('#lblVATableSales').text(strTotalVATSales);
                            $('#lblVATableSalesPercent').text(strTotalVATPercent);

                            $('#lblVATAmount').text(strTotalVATAmount);
                            $('#lblVATAmountPercent').text(strTotalVATAmountPercent);

                            $('#lblNonVAT').text(strTotalNonVAT);
                            $('#lblNonVATPercent').text(strTotalNonVATPercent);

                            $('#lblVATExempt').text(strTotalVATExempt);
                            $('#lblVATExemptPercent').text(strTotalVATExemptPercent);

                            $('#lblZeroRatedVAT').text(strTotalZeroRatedVAT);
                            $('#lblZeroRatedVATPercent').text(strTotalZeroRatedVATPercent);

                            $('#lblTotalAllVat').text(strTotalAllVAT);
                            $('#lblTotalAllVatPercent').text(strTotalAllVATPercent);

                            pValues.push({
                                Key: 'VAT-able Sales Amount',
                                DataType: 'numeric',
                                Value: dblVAT
                            });

                            pValues.push({
                                Key: 'VAT-able Sales Percent',
                                DataType: 'numeric',
                                Value: dblTotalVATSalesPercent
                            });

                            pValues.push({
                                Key: 'VAT Amount',
                                DataType: 'numeric',
                                Value: dblVATAmount
                            });

                            pValues.push({
                                Key: 'VAT Percent',
                                DataType: 'numeric',
                                Value: dblTotalVATAmountPercent
                            });

                            pValues.push({
                                Key: 'Non-VAT Amount',
                                DataType: 'numeric',
                                Value: dblNonVAT
                            });

                            pValues.push({
                                Key: 'Non-VAT Percent',
                                DataType: 'numeric',
                                Value: dblTotalNonVATPercent
                            });

                            pValues.push({
                                Key: 'VAT-Exempt Amount',
                                DataType: 'numeric',
                                Value: dblVATExempt
                            });

                            pValues.push({
                                Key: 'VAT-Exempt Percent',
                                DataType: 'numeric',
                                Value: dblTotalVATExemptPercent
                            });

                            pValues.push({
                                Key: 'Zero-Rated VAT Amount',
                                DataType: 'numeric',
                                Value: dblZeroRatedVAT
                            });

                            pValues.push({
                                Key: 'Zero-Rated VAT Percent',
                                DataType: 'numeric',
                                Value: dblTotalZeroRatedVATPercent
                            });

                            pValues.push({
                                Key: 'Total VAT Amount',
                                DataType: 'numeric',
                                Value: dblTotalVAT
                            });

                            pValues.push({
                                Key: 'Total VAT Percent',
                                DataType: 'numeric',
                                Value: dblTotalVATPercent
                            });
                            //End Sales Grouped by VAT TYPE----------------------------

                            //Get Sales by Cashier
                            if ((strFrom == strTo && $("#selPOSSalesReportRange").val() == "Day") ||
                                                $("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                if (!CBool(sessionStorage.PrintingOfflineXReadData)) {
                                    if (vDataSalesByCashier.Rows.length > 0) {

                                        strPrint = '';
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, "Sales Grouped by Cashier", intFAFCTotalLenght, '', 0, '', 0, 0);
                                        strPrint = strPrint + strTemp;

                                        //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                        //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                        //Get Total Sales
                                        var dblAllTotalSales = 0;
                                        for (var p = 0; p <= vDataSalesByCashier.Rows.length - 1; p++) {
                                            dblAllTotalSales = dblAllTotalSales + parseFloat(vDataSalesByCashier.Rows[p].TotalSales);
                                        };

                                        var dblAllTotalSalesPercent = 0;

                                        strPrint = '';

                                        pValues.push
                                        for (var p = 0; p <= vDataSalesByCashier.Rows.length - 1; p++) {

                                            var strTotalSales = SetPrefixPostfix(AccountingFormat((vDataSalesByCashier.Rows[p].TotalSales || 0)), 14, ' ', 0);
                                            var dblTotalSalesPercent = (parseFloat(vDataSalesByCashier.Rows[p].TotalSales || 0) / dblAllTotalSales) * 100;
                                            var strTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                            dblAllTotalSalesPercent = dblAllTotalSalesPercent + dblTotalSalesPercent;
                                            $('#tbGroupByCashier').html('');
                                            var tData = $('<tr> ')
                                                                            .append($('<td>')
                                                                                .text(vDataSalesByCashier.Rows[p].CashierName)
                                                                        )
                                                                            .append($('<td>')
                                                                                .text(vDataSalesByCashier.Rows[p].TotalSales)
                                                                        )
                                                                        .append($('<td>')
                                                                                .text(AccountingFormat(dblTotalSalesPercent) + '%')
                                                                        )

                                            $('#tbGroupByCashier').append(tData);
                                            pInput.ByCashier.push({
                                                Key: vDataSalesByCashier.Rows[p].CashierName,
                                                Value: vDataSalesByCashier.Rows[p].TotalSales,
                                                Percent: dblTotalSalesPercent
                                            });
                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, vDataSalesByCashier.Rows[p].CashierName, 14, strTotalSales, 30, strTotalSalesPercent, intFAFCTotalLenght, 0);
                                            strPrint = strPrint + strTemp;
                                        };

                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                        var strAllTotalSales = SetPrefixPostfix(AccountingFormat(dblAllTotalSales), 14, ' ', 0);
                                        var strAllTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblAllTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                        strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                        strPrint = FormatTextToPrint(intFAFCTotalLenght, '              ', 15, strAllTotalSales, 31, strAllTotalSalesPercent, intFAFCTotalLenght, 1);
                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                    }
                                }
                            }

                            //Get Sales by Machine
                            if ((strFrom == strTo && $("#selPOSSalesReportRange").val() == "Day") ||
                                            $("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                if (!CBool(sessionStorage.PrintingOfflineXReadData)) {
                                    if (vDataSalesByMachine.Rows.length > 0) {

                                        strPrint = '';
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, "Sales Grouped by Machine (MIN)", intFAFCTotalLenght, '', 0, '', 0, 0);
                                        strPrint = strPrint + strTemp;

                                        //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                        //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                        //Get Total Sales
                                        dblAllTotalSales = 0;
                                        for (var p = 0; p <= vDataSalesByMachine.Rows.length - 1; p++) {
                                            dblAllTotalSales = dblAllTotalSales + parseFloat(vDataSalesByMachine.Rows[p].TotalSales);
                                        };

                                        dblAllTotalSalesPercent = 0;

                                        strPrint = '';
                                        for (var p = 0; p <= vDataSalesByMachine.Rows.length - 1; p++) {

                                            var strTotalSales = SetPrefixPostfix(AccountingFormat((vDataSalesByMachine.Rows[p].TotalSales || 0)), 14, ' ', 0);
                                            var dblTotalSalesPercent = (parseFloat(vDataSalesByMachine.Rows[p].TotalSales || 0) / dblAllTotalSales) * 100;
                                            var strTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                            dblAllTotalSalesPercent = dblAllTotalSalesPercent + dblTotalSalesPercent;
                                            $('#tbGroupByMachine').html('');
                                            var tData = $('<tr> ')
                                                                            .append($('<td>')
                                                                                .text(vDataSalesByMachine.Rows[p].DeviceName)
                                                                        )
                                                                            .append($('<td>')
                                                                                .text(strTotalSales)
                                                                        )
                                                                        .append($('<td>')
                                                                                .text(strTotalSalesPercent)
                                                                        )

                                            $('#tbGroupByMachine').append(tData);
                                            pInput.ByMachine.push({
                                                Key: vDataSalesByMachine.Rows[p].DeviceName,
                                                Value: vDataSalesByMachine.Rows[p].TotalSales,
                                                Percent: dblTotalSalesPercent
                                            });

                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, vDataSalesByMachine.Rows[p].DeviceName, 14, strTotalSales, 30, strTotalSalesPercent, intFAFCTotalLenght, 0);
                                            strPrint = strPrint + strTemp;

                                        };

                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                        strAllTotalSales = SetPrefixPostfix(AccountingFormat(dblAllTotalSales), 14, ' ', 0);
                                        strAllTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblAllTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                        strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                        strPrint = FormatTextToPrint(intFAFCTotalLenght, '              ', 15, strAllTotalSales, 31, strAllTotalSalesPercent, intFAFCTotalLenght, 1);
                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                    }
                                }
                            }

                            //Get Sales by Cashier and Machine
                            if ((strFrom == strTo && $("#selPOSSalesReportRange").val() == "Day") ||
                                            $("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                if (!CBool(sessionStorage.PrintingOfflineXReadData)) {
                                    if (vDataSalesByCashierMachine.Rows.length > 0) {

                                        strPrint = '';
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, "Sales Grouped by Cashier & Machine (MIN)", intFAFCTotalLenght, '', 0, '', 0, 0);
                                        strPrint = strPrint + strTemp;

                                        //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                        //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                        //Get Total Sales
                                        dblAllTotalSales = 0;
                                        for (var p = 0; p <= vDataSalesByCashierMachine.Rows.length - 1; p++) {
                                            dblAllTotalSales = dblAllTotalSales + parseFloat(vDataSalesByCashierMachine.Rows[p].TotalSales);
                                        };

                                        dblAllTotalSalesPercent = 0;

                                        var strCashierName = "";
                                        var strDeviceName = "";
                                        var dblDeviceSales = 0;

                                        dblAllTotalSalesPercent = 0;

                                        strPrint = '';
                                        for (var p = 0; p <= vDataSalesByCashierMachine.Rows.length - 1; p++) {

                                            if (strCashierName != vDataSalesByCashierMachine.Rows[p].CashierName) {
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, vDataSalesByCashierMachine.Rows[p].CashierName, intFAFCTotalLenght, '', 0, '', 0, 0);
                                                strPrint = strPrint + strTemp;

                                                strCashierName = vDataSalesByCashierMachine.Rows[p].CashierName;
                                            }

                                            dblDeviceSales = parseFloat(vDataSalesByCashierMachine.Rows[p].TotalSales || 0);
                                            strTotalSales = SetPrefixPostfix(AccountingFormat(dblDeviceSales), 14, ' ', 0);
                                            dblTotalSalesPercent = (dblDeviceSales / dblAllTotalSales) * 100;
                                            strTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                            dblAllTotalSalesPercent = dblAllTotalSalesPercent + dblTotalSalesPercent;

                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, '     ' + vDataSalesByCashierMachine.Rows[p].DeviceName, 14, strTotalSales, 30, strTotalSalesPercent, intFAFCTotalLenght, 0);
                                            strPrint = strPrint + strTemp;

                                            $('#tbGroupByCashierAndMachine').html('');

                                            var tData = $('<tr> ')
                                                            .append($('<td>')
                                                                .text(strCashierName + ' ' + vDataSalesByCashierMachine.Rows[p].DeviceName)
                                                        )
                                                            .append($('<td>')
                                                                .text(strTotalSales)
                                                        )
                                                        .append($('<td>')
                                                                .text(strTotalSalesPercent)
                                                        )

                                            $('#tbGroupByCashierAndMachine').append(tData);
                                            pInput.ByCashierMachine.push({
                                                Key: strCashierName + ' ' + vDataSalesByCashierMachine.Rows[p].DeviceName,
                                                Value: dblDeviceSales,
                                                Percent: dblTotalSalesPercent
                                            });
                                        };

                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                        strAllTotalSales = SetPrefixPostfix(AccountingFormat(dblAllTotalSales), 14, ' ', 0);
                                        strAllTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblAllTotalSalesPercent) + '%', intFAFCTotalLenght - 33, ' ', 0);

                                        strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                        strPrint = FormatTextToPrint(intFAFCTotalLenght, '              ', 15, strAllTotalSales, 31, strAllTotalSalesPercent, intFAFCTotalLenght, 1);
                                        //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                    }
                                }
                            }

                            if ($("#selPOSSalesReportRange").val() == "Day" ||
                                    $("#selPOSSalesReportRange").val() == "Today" ||
                                    $("#selPOSSalesReportRange").val() == "Yesterday") {

                                //Other Informatoin
                                strPrint = '';
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, "Other Information", intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                var strTransCount = SetPrefixPostfix(intTransCount + '', intFAFCTotalLenght - 21, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Trans. Count', 19, strTransCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                $('#lblTotalTransCount').text(strTransCount);
                                pValues.push({
                                    Key: 'TotalTransCount',
                                    DataType: 'numeric',
                                    Value: intTransCount
                                });   
                                
                                var strSalesTransCount = SetPrefixPostfix(intTransCount + '', intFAFCTotalLenght - 21, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Sales Trans. Count', 19, strSalesTransCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                $('#lblSalesTransCount').text(strSalesTransCount);

                                var strSalesItemCount = SetPrefixPostfix(intSalesItemCount + '', intFAFCTotalLenght - 24, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Sales Line Item Count', 22, strSalesItemCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                $('#lblSalesLineItemCount').text(strSalesItemCount);
                                pValues.push({
                                    Key: 'SalesLineItemCount',
                                    DataType: 'numeric',
                                    Value: intSalesItemCount
                                });   

                                var strSRVTransCount = SetPrefixPostfix(intSRVTransCount + '', intFAFCTotalLenght - 19, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SRV Trans. Count', 17, strSRVTransCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                $('#lblSRVTransCount').text(strSRVTransCount);
                                pValues.push({
                                    Key: 'SrvTransCount',
                                    DataType: 'numeric',
                                    Value: intSRVTransCount
                                });   

                                var strSRVItemCount = SetPrefixPostfix(intSRVItemCount + '', intFAFCTotalLenght - 22, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SRV Line Item Count', 20, strSRVItemCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                $('#lblSRVLineItemCount').text(strSRVItemCount);
                                pValues.push({
                                    Key: 'SrvLineItemCount',
                                    DataType: 'numeric',
                                    Value: intSRVItemCount
                                });   

                                var strRefTransCount = SetPrefixPostfix(intRefTransCount + '', intFAFCTotalLenght - 22, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Refund Trans. Count', 20, strRefTransCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                $('#lblRefundTransCount').text(strRefTransCount);
                                pValues.push({
                                    Key: 'RefundTransCount',
                                    DataType: 'numeric',
                                    Value: intRefTransCount
                                });   

                                var strRefItemCount = SetPrefixPostfix(intRefItemCount + '', intFAFCTotalLenght - 25, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Refund Line Item Count', 23, strRefItemCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                $('#lblRefundLineItemCount').text(strRefItemCount);
                                pValues.push({
                                    Key: 'RefundLineItemCount',
                                    DataType: 'numeric',
                                    Value: intRefItemCount
                                });   

                                var strCustomerCount = SetPrefixPostfix(OffLineTotalNumberOfCustomers + '', intFAFCTotalLenght - 17, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Customer', 15, strCustomerCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                $('#lblCustomerCount').text(strCustomerCount);
                                pValues.push({
                                    Key: 'TotalNumberOfCustomers',
                                    DataType: 'numeric',
                                    Value: OffLineTotalNumberOfCustomers
                                });   

                                //END Other Informatoin

                                strPrint = '';
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Printed by ', 15, ': ' + (vDataHeader.Rows[0].PrintedBy || ''), intFAFCTotalLenght, '', 0, 1);
                                strPrint = strPrint + strTemp;
                                $('#lblPrintedby').text((vDataHeader.Rows[0].PrintedBy || ''));

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataHeader.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                strPrint = strPrint + strTemp;
                                $('#lblDateandTime2').text(($.format.date(new Date(vDataHeader.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''));

                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);

                                //                                PrintText(strPrinterIP);
                                //                                ClearCommandBuffer();

                            } else {
                                strPrint = '';
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Printed by ', 15, ': ' + (vDataHeader.Rows[0].PrintedBy || ''), intFAFCTotalLenght, '', 0, 1);
                                strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataHeader.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                strPrint = strPrint + strTemp;

                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);

                                PrintText(strPrinterIP);
                                ClearCommandBuffer();
                            }



                            //                                $('#lblTotalAllVatPercent').text(strTotalAllVATPercent)
                            //                                $('#lblVATableSalesPercent').text(strTotalVATSales);

                        } else {
                            //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);
                            PrintText(strPrinterIP);
                            ClearCommandBuffer();
                        }

                    }

                    if (IsViewOnly == 0) {
                        var pUrl = '/ReportsPOS/';
                        if (vDataHeader.Rows[0].POSReportType == 0) {
                            // cashiers sales report
                            pUrl += 'CreateCashierSalesReportPdf';
                        } else {
                            // managers sales report
                            pUrl += 'CreateManagerSalesReportPdf';
                        }
                        pInput.Values = pValues;
                        NewMessageBox('Creating document for printing.', 'Please Wait');
                        AjaxSend({
                            url: pUrl,
                            data: {
                                Input: pInput
                            },
                            success: function(vData) {
                                if (vData.Error) {
                                    if (vData.Error.indexOf('PrintHash') == -1) {
                                        if (vData.Error == 'UUID is required.')
                                            NewMessageBox('You cannot print out documents because your login did not go through the Mobiz Controller app. <br /><br /> \n\n' +
                                                'This is by design to enforce document print controls. <br /><br />\n\n' + 
                                                'Please close your Mobiz EAS and then click on the Mobiz Controller app to relaunch Mobiz EAS and enable printing of documents. <br /><br />\n\n' +
                                                'Note: Mobiz Controller is not yet available on None-Windows devices. Please contact Mobiz for methods on printing using a none-Windows device.\n\n', '',
                                                [
                                                    {
                                                        Text: 'Download Mobiz Controller',
                                                        Click: function() {
                                                            window.open('/Content/PrintApplet.zip', '_blank');
                                                        }
                                                    },
                                                    {
                                                        Text: 'Cancel'
                                                    }
                                                ]);
                                        else
                                            NewMessageBox(vData.Error);
                                    } else
                                        NoPrintHash();
                                } else if (vData.Result == 'ok') 
                                    NewMessageBox('Your document will be printed by your local print server.', '', null, {
                                        Close: function() {
                                            
                                        }
                                    });
                            }
                        });
                    }
                }

                function PrintDesktopSalesReport() {
                    var strTemp = '';
                    var strPrint = '';
                    var pData = {};
                    var strFrom = '';
                    var strTo = '';
                    var strYear = '';


                    if ($("#selPOSSalesReportRange").val() == "Day") {
                        if ($.trim($("#selPOSSalesReportByDateFromMonth").val()) == "" ||
                    $.trim($("#selPOSSalesReportByDateFromDay").val()) == "" ||
                    $.trim($("#selPOSSalesReportByDateFromYear").val()) == "") {

                            MessageBox("Please enter start date of report.",
                            "Start Date",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                            return;
                        }

                        if ($.trim($("#selPOSSalesReportByDateToMonth").val()) == "" ||
                    $.trim($("#selPOSSalesReportByDateToDay").val()) == "" ||
                    $.trim($("#selPOSSalesReportByDateToYear").val()) == "") {

                            MessageBox("Please enter end date of report.",
                            "End Date",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                            return;
                        }

                        strFrom = $("#selPOSSalesReportByDateFromMonth").val() + "/" + $("#selPOSSalesReportByDateFromDay").val() + "/" + $("#selPOSSalesReportByDateFromYear").val();
                        strTo = $("#selPOSSalesReportByDateToMonth").val() + "/" + $("#selPOSSalesReportByDateToDay").val() + "/" + $("#selPOSSalesReportByDateToYear").val();
                        strYear = '';

                    } else if ($("#selPOSSalesReportRange").val() == "Week") {
                        if ($.trim($("#selPOSSalesReportRangeByWeekFrom").val()) == "") {

                            MessageBox("Please select starting week number of report.",
                            "Start Week",
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                            return;
                        }

                        if ($.trim($("#selPOSSalesReportRangeByWeekTo").val()) == "") {

                            MessageBox("Please select end week number of report.",
                            "End Week",
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                            return;
                        }

                        if ($.trim($("#selPOSSalesReportRangeByWeekYear").val()) == "") {

                            MessageBox("Please select year of the report.",
                            "Year of the Week",
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                            return;
                        }

                        strFrom = $.trim($("#selPOSSalesReportRangeByWeekFrom").val());
                        strTo = $.trim($("#selPOSSalesReportRangeByWeekTo").val());
                        strYear = $.trim($("#selPOSSalesReportRangeByWeekYear").val());

                    } else if ($("#selPOSSalesReportRange").val() == "Month") {
                        if ($.trim($("#selPOSSalesReportRangeByMonthFrom").val()) == "") {

                            MessageBox("Please select starting month of report.",
                            "Start Month",
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                            return;
                        }

                        if ($.trim($("#selPOSSalesReportRangeByMonthTo").val()) == "") {

                            MessageBox("Please select year of the report.",
                            "Month Year",
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                            return;
                        }

                        if ($.trim($("#selPOSSalesReportRangeByMonthYear").val()) == "") {

                            MessageBox("Please select year of the report.",
                            "End Month",
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                            return;
                        }

                        strFrom = $.trim($("#selPOSSalesReportRangeByMonthFrom").val());
                        strTo = $.trim($("#selPOSSalesReportRangeByMonthTo").val());
                        strYear = $.trim($("#selPOSSalesReportRangeByMonthYear").val());

                    } else if ($("#selPOSSalesReportRange").val() == "Quarter") {
                        if ($.trim($("#selPOSSalesReportRangeByQtrFrom").val()) == "") {

                            MessageBox("Please select starting quarter of report.",
                            "Start Quarter",
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                            return;
                        }

                        if ($.trim($("#selPOSSalesReportRangeByQtrTo").val()) == "") {

                            MessageBox("Please select end quarter of report.",
                            "End Quarter",
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                            return;
                        }

                        if ($.trim($("#selPOSSalesReportRangeByQtrYear").val()) == "") {

                            MessageBox("Please select quarter year of the report.",
                            "Quarter Year",
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                            return;
                        }

                        strFrom = $.trim($("#selPOSSalesReportRangeByQtrFrom").val());
                        strTo = $.trim($("#selPOSSalesReportRangeByQtrTo").val());
                        strYear = $.trim($("#selPOSSalesReportRangeByQtrYear").val());

                    } else if ($("#selPOSSalesReportRange").val() == "Year") {
                        if ($.trim($("#selPOSSalesReportRangeByYear").val()) == "") {

                            MessageBox("Please select year of the report.",
                            "Year",
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                            return;
                        }

                        strFrom = '';
                        strTo = '';
                        strYear = $.trim($("#selPOSSalesReportRangeByYear").val());

                    } else if ($("#selPOSSalesReportRange").val() == "") {
                        MessageBox("Please select report range.",
                            "Report Range",
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                        return;
                    }

                    var japplet = document.jzebra;
                    var blnIsPrinterFound = false;

                    if (japplet != null) {
                        while (!blnIsPrinterFound) {

                            if (FindDesktopPrinter("")) {
                                blnIsPrinterFound = true;

                                pData.RangeType = $.trim($("#selPOSSalesReportRange").val());
                                pData.RangeFrom = strFrom;
                                pData.RangeTo = strTo;
                                pData.RangeYear = strYear;

                                //Get Header
                                AjaxSend({
                                    url: "ReportsPOS/getReportHeader",
                                    success: function (vDataHeader) {

                                        if (vDataHeader.ReportPOS.Rows.length > 0) {

                                            strPrint = SetTextAlign($.trim(vDataHeader.ReportPOS.Rows[0].OrgBusinessTradeName || ''), 1, intDesktopPrintLenght);
                                            japplet.append(strPrint + "\n");

                                            strPrint = SetTextAlign($.trim(vDataHeader.ReportPOS.Rows[0].SubscriberOrganizationName || ''), 1, intDesktopPrintLenght);
                                            japplet.append(strPrint + "\n");

                                            strPrint = SetTextAlign($.trim(vDataHeader.ReportPOS.Rows[0].OrgBranchName || ''), 1, intDesktopPrintLenght);
                                            japplet.append(strPrint + "\n");

                                            strPrint = SetTextAlign($.trim(vDataHeader.ReportPOS.Rows[0].OrgCompleteAddress || ''), 1, intDesktopPrintLenght);
                                            japplet.append(strPrint + "\n");

                                            strPrint = SetTextAlign('TIN ' + $.trim(vDataHeader.ReportPOS.Rows[0].OrgTIN || ''), 1, intDesktopPrintLenght);
                                            japplet.append(strPrint + "\n");

                                            strPrint = SetTextAlign('Tel. No.' + $.trim(vDataHeader.ReportPOS.Rows[0].OrgTelNo || ''), 1, intDesktopPrintLenght);
                                            japplet.append(strPrint + "\n\n");

                                            if (vDataHeader.ReportPOS.Rows[0].POSReportType == 0) {
                                                //Get Payment INfo
                                                AjaxSend({
                                                    url: "ReportsPOS/getCashierSalesTransactionSummary",
                                                    data: pData,
                                                    success: function (vDataInfo) {

                                                        if (vDataInfo.ReportPOS.Rows.length > 0) {
                                                            if ($("#selPOSSalesReportRange").val() == "Day") {

                                                                strPrint = SetTextAlign("CASHIER'S SALES REPORT", 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign((strFrom == strTo ? 'Date: ' + strFrom : 'From ' + strFrom + ' To ' + strTo), 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                            } else if ($("#selPOSSalesReportRange").val() == "Week") {

                                                                strPrint = SetTextAlign("CASHIER'S", 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign("Same-Store Comparative Sales Report", 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign((strFrom == strTo ? 'Week: ' + strFrom : 'From Week ' + strFrom + ' To Week ' + strTo), 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign((vDataInfo.ReportPOS.Rows[0].YearFrom == vDataInfo.ReportPOS.Rows[0].YearTo ? "Year : " + vDataInfo.ReportPOS.Rows[0].YearFrom : "Year : " + vDataInfo.ReportPOS.Rows[0].YearFrom + "-" + vDataInfo.ReportPOS.Rows[0].YearTo), 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                            } else if ($("#selPOSSalesReportRange").val() == "Month") {

                                                                strPrint = SetTextAlign("CASHIER'S", 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign("Same-Store Comparative Sales Report", 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign((strFrom == strTo ? 'Month: ' + $.format.date(new Date(strFrom + '/1/2013'), 'MMM') : 'From ' + $.format.date(new Date(strFrom + '/1/2013'), 'MMM') + ' To ' + $.format.date(new Date(strTo + '/1/2013'), 'MMM')), 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign((vDataInfo.ReportPOS.Rows[0].YearFrom == vDataInfo.ReportPOS.Rows[0].YearTo ? "Year : " + vDataInfo.ReportPOS.Rows[0].YearFrom : "Year : " + vDataInfo.ReportPOS.Rows[0].YearFrom + "-" + vDataInfo.ReportPOS.Rows[0].YearTo), 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                            } else if ($("#selPOSSalesReportRange").val() == "Quarter") {

                                                                strPrint = SetTextAlign("CASHIER'S", 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign("Same-Store Comparative Sales Report", 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign((strFrom == strTo ? (strFrom == '1' ? "1st Quarter" : (strFrom == '2' ? "2nd Quarter" : (strFrom == '3' ? "3rd Quarter" : (strFrom == '4' ? "4th Quarter" : "")))) : 'From ' + (strFrom == '1' ? "1st Quarter" : (strFrom == '2' ? "2nd Quarter" : (strFrom == '3' ? "3rd Quarter" : (strFrom == '4' ? "4th Quarter" : "")))) + ' To ' + (strTo == '1' ? "1st Quarter" : (strTo == '2' ? "2nd Quarter" : (strTo == '3' ? "3rd Quarter" : (strTo == '4' ? "4th Quarter" : ""))))), 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign((vDataInfo.ReportPOS.Rows[0].YearFrom == vDataInfo.ReportPOS.Rows[0].YearTo ? "Year : " + vDataInfo.ReportPOS.Rows[0].YearFrom : "Year : " + vDataInfo.ReportPOS.Rows[0].YearFrom + "-" + vDataInfo.ReportPOS.Rows[0].YearTo), 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                            } else if ($("#selPOSSalesReportRange").val() == "Year") {

                                                                strPrint = SetTextAlign("CASHIER'S SALES REPORT", 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign('Year: ' + strYear, 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                            } else if ($("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                                                strPrint = SetTextAlign("CASHIER'S SALES REPORT", 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign('Date : ' + $.format.date(new Date(vDataInfo.ReportPOS.Rows[0].SalesDate), 'MM-dd-yyyy'), 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");
                                                            }

                                                            japplet.append("\n");
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Cashier ', 0, 8), 8, ': ' + (vDataInfo.ReportPOS.Rows[0].CashierName || ''), intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            var dblTotalSales = 0;
                                                            var dblTotalSalesDiscount = 0;
                                                            var dblAdjRefund = 0;
                                                            var dblAdjSRV = 0;
                                                            var dblNetSales = 0;

                                                            var dblCash = 0;
                                                            var dblCheck = 0;
                                                            var dblDebitCard = 0;
                                                            var dblCreditCard = 0;
                                                            var dblSRV = 0;
                                                            var dblOtherPaymentType = 0;
                                                            var dblCharges = 0;
                                                            var dblSMAC = 0;
                                                            var dblGCash = 0;
                                                            var dblEPlus = 0;
                                                            var dblVoucher = 0;
                                                            var dblTotalPayment = 0;

                                                            var dblVAT = 0;
                                                            var dblVATAmount = 0;
                                                            var dblNonVAT = 0;
                                                            var dblVATExempt = 0;
                                                            var dblZeroRatedVAT = 0;
                                                            var dblTotalVAT = 0;

                                                            for (var p = 0; p <= vDataInfo.ReportPOS.Rows.length - 1; p++) {

                                                                dblTotalSales = dblTotalSales + parseFloat(vDataInfo.ReportPOS.Rows[p].CurGrossSales || 0);
                                                                dblTotalSalesDiscount = dblTotalSalesDiscount + parseFloat(vDataInfo.ReportPOS.Rows[p].CurSalesDiscount || 0) + parseFloat(vDataInfo.Rows[p].CurSCDiscount || 0);
                                                                dblAdjRefund = dblAdjRefund + parseFloat(vDataInfo.ReportPOS.Rows[p].CurSalesReturnRefund || 0);
                                                                dblAdjSRV = dblAdjSRV + parseFloat(vDataInfo.ReportPOS.Rows[p].CurSalesReturnExchange || 0);

                                                                dblCash = dblCash + parseFloat(vDataInfo.ReportPOS.Rows[p].CashAmount || 0);
                                                                dblCheck = dblCheck + parseFloat(vDataInfo.ReportPOS.Rows[p].CheckPaymentAmount || 0);
                                                                dblDebitCard = dblDebitCard + parseFloat(vDataInfo.ReportPOS.Rows[p].DebitCardAmount || 0);
                                                                dblCreditCard = dblCreditCard + parseFloat(vDataInfo.ReportPOS.Rows[p].CreditCardAmount || 0);
                                                                dblSRV = dblSRV + parseFloat(vDataInfo.ReportPOS.Rows[p].SRVAmount || 0);
                                                                dblOtherPaymentType = dblOtherPaymentType + parseFloat(vDataInfo.ReportPOS.Rows[p].OtherTypeAmount || 0);
                                                                dblCharges = dblCharges + parseFloat(vDataInfo.ReportPOS.Rows[p].ChargeAmount || 0);
                                                                dblSMAC = dblSMAC + parseFloat(vDataInfo.ReportPOS.Rows[p].SMACAmount || 0);
                                                                dblGCash = dblGCash + parseFloat(vDataInfo.Rows[p].GCashAmount || 0);
                                                                dblEPlus = dblEPlus + parseFloat(vDataInfo.ReportPOS.Rows[p].EPlusAmount || 0);
                                                                dblVoucher = dblVoucher + parseFloat(vDataInfo.ReportPOS.Rows[p].VoucherAmount || 0);


                                                                dblVAT = dblVAT + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVatablePortionOfSCDTransaction || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVATSales || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVATSalesSalesReturn || 0);
                                                                dblVATAmount = dblVATAmount + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVatAmountPortionOfSCDTransaction || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVATAmount || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVATAmountSalesReturn || 0);
                                                                dblNonVAT = dblNonVAT + parseFloat(vDataInfo.ReportPOS.Rows[p].CurNoTAXSales || 0);
                                                                dblVATExempt = dblVATExempt + parseFloat(vDataInfo.Rows[p].CurVATExemptSales || 0);
                                                                dblZeroRatedVAT = dblZeroRatedVAT + parseFloat(vDataInfo.Rows[p].CurZeroRatedVATSales || 0);
                                                            };

                                                            dblCash = dblCash - dblAdjRefund;
                                                            dblNetSales = dblTotalSales - dblTotalSalesDiscount - dblAdjRefund - dblAdjSRV + dblCharges;
                                                            dblTotalPayment = dblCash + dblCheck + dblDebitCard + dblCreditCard + dblSRV + dblOtherPaymentType;

                                                            dblTotalVAT = dblVAT + dblVATAmount + dblNonVAT + db
                                                            dblTotalVAT = dblVAT + dblVATAmount + dblNonVAT + dblVATExempt + dblZeroRatedVAT;

                                                            var strTotalSales = SetPrefixPostfix(AccountingFormat(dblTotalSales), intDesktopPrintLenght - 16, ' ', 0);
                                                            japplet.append("\n");
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Gross Sales ', 0, 12), 12, ': ' + strTotalSales, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            //Sales Discount
                                                            var strSalesDiscount = SetPrefixPostfix(AccountingFormat(dblTotalSalesDiscount), intDesktopPrintLenght - 22, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Total Sales Discount', 20, strSalesDiscount, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            //charges

                                                            if (dblCharges > 0) {
                                                                var strTotalCharges = SetPrefixPostfix(AccountingFormat(dblCharges), intDesktopPrintLenght - 22, ' ', 0);
                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Total Charges', 20, strTotalCharges, intDesktopPrintLenght, '', 0);
                                                                japplet.append(strPrint + "\n");
                                                            }

                                                            //Sales Adjustment
                                                            japplet.append("\n");
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Summary of Adjustments', 0, intDesktopPrintLenght), intDesktopPrintLenght, '', 0, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            var strAdjRefund = SetPrefixPostfix(AccountingFormat(dblAdjRefund), intDesktopPrintLenght - 28, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Total Sales Refund for the Period', 26, strAdjRefund, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            var strAdjSRV = SetPrefixPostfix(AccountingFormat(dblAdjSRV), intDesktopPrintLenght - 32, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Total Sales Returns Subject to Merchadise Exchange           (SRV Issued)', 30, strAdjSRV, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            var strNetSalesAdj = SetPrefixPostfix(AccountingFormat(parseFloat(dblAdjRefund) + parseFloat(dblAdjSRV)), intDesktopPrintLenght - 20, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Net of Adjustments', 18, strNetSalesAdj, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint);

                                                            strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                                            japplet.append('\n' + strPrint + "\n");

                                                            var strNetSales = SetPrefixPostfix(AccountingFormat(dblNetSales), intDesktopPrintLenght - 11, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Net Sales', 9, strNetSales, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            var strIssuedSRV = SetPrefixPostfix(AccountingFormat(dblAdjSRV), intDesktopPrintLenght - 18, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Add : Issued SRV', 16, strIssuedSRV, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint);
                                                            strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                                            japplet.append('\n' + strPrint + "\n");

                                                            var strTotals = SetPrefixPostfix(AccountingFormat(dblNetSales + dblAdjSRV), intDesktopPrintLenght - 20, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Net Sales Adjusted', 18, strTotals, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'for Exchanges', intDesktopPrintLenght, 0, '', '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            //END Sales Adjustment

                                                            //Sales Grouped By Mode of Payment
                                                            japplet.append("\n");
                                                            strPrint = SetTextAlign("Inventory of Payments", 1, intDesktopPrintLenght);
                                                            japplet.append(strPrint + "\n");
                                                            japplet.append("\n");

                                                            var strTotalCash = SetPrefixPostfix(AccountingFormat(dblCash), intDesktopPrintLenght - 16, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Cash        ', 0, 12), 12, ': ' + strTotalCash, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            var strTotalCheck = SetPrefixPostfix(AccountingFormat(dblCheck), intDesktopPrintLenght - 16, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Check       ', 0, 12), 12, ': ' + strTotalCheck, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            var strTotalDebitCard = SetPrefixPostfix(AccountingFormat(dblDebitCard), intDesktopPrintLenght - 16, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Debit Card  ', 0, 12), 12, ': ' + strTotalDebitCard, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            var strTotalCreditCard = SetPrefixPostfix(AccountingFormat(dblCreditCard), intDesktopPrintLenght - 16, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Credit Card ', 0, 12), 12, ': ' + strTotalCreditCard, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            var strTotalSRV = SetPrefixPostfix(AccountingFormat(dblSRV), intDesktopPrintLenght - 16, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('SRV         ', 0, 12), 12, ': ' + strTotalSRV, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");


                                                            if (IsFowstInSMMall) {
                                                                var strTotalSMAC = SetPrefixPostfix(AccountingFormat(dblSMAC), intDesktopPrintLenght - 16, ' ', 0);
                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('SMAC        ', 0, 12), 12, ': ' + strTotalSMAC, intDesktopPrintLenght, '', 0);
                                                                japplet.append(strPrint + "\n");

                                                                var strTotalEPlus = SetPrefixPostfix(AccountingFormat(dblEPlus), intDesktopPrintLenght - 16, ' ', 0);
                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('E-Plus      ', 0, 12), 12, ': ' + strTotalEPlus, intDesktopPrintLenght, '', 0);
                                                                japplet.append(strPrint + "\n");

                                                                var strTotalVoucher = SetPrefixPostfix(AccountingFormat(dblVoucher), intDesktopPrintLenght - 16, ' ', 0);
                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Voucher     ', 0, 12), 12, ': ' + strTotalVoucher, intDesktopPrintLenght, '', 0);
                                                                japplet.append(strPrint + "\n");

                                                            } else {
                                                                var strTotalOther = SetPrefixPostfix(AccountingFormat(dblOtherPaymentType), intDesktopPrintLenght - 16, ' ', 0);
                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Other Tender', 0, 12), 12, ': ' + strTotalOther, intDesktopPrintLenght, '', 0);
                                                                japplet.append(strPrint + "\n");

                                                            }


                                                            strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                                            japplet.append(strPrint + "\n");

                                                            var strTotalClassPayment = SetPrefixPostfix(AccountingFormat(dblTotalPayment), intDesktopPrintLenght - 17, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Total Inventory', 15, strTotalClassPayment, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'of Payments', intDesktopPrintLenght, 0, '', '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            var dblTotalOverPayment = dblTotalPayment - dblNetSales;
                                                            //////                                                        var strTotalOverPayment = SetPrefixPostfix(AccountingFormat((dblTotalOverPayment < 0 ? (dblTotalOverPayment * -1) : dblTotalOverPayment)), intDesktopPrintLenght - 16, ' ', 0);
                                                            var strTotalOverPayment = SetPrefixPostfix(AccountingFormat(dblTotalOverPayment), intDesktopPrintLenght - 26, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Excess of Inventory of', 24, strTotalOverPayment, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Payments vs Net Sales', intDesktopPrintLenght, 0, '', '', 0);
                                                            japplet.append(strPrint + "\n");
                                                            //END Sales Grouped By Mode of Payment

                                                            //Inventory of Payments Audit
                                                            japplet.append("\n");
                                                            strPrint = SetTextAlign("Inventory of Payments Audit", 1, intDesktopPrintLenght);
                                                            japplet.append(strPrint + "\n");
                                                            japplet.append("\n");

                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Net Sales Adjusted', 18, strTotals, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'for Exchanges', intDesktopPrintLenght, 0, '', '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Total Inventory', 15, strTotalClassPayment, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'of Payments', intDesktopPrintLenght, 0, '', '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                                            japplet.append(strPrint + "\n");

                                                            var dblNetSalesAdjustedLessTotalInvPayment = (dblNetSales + dblAdjSRV) - dblTotalPayment + dblCharges;
                                                            var strTotalNetSalesAdjustedLessTotalInvPayment = SetPrefixPostfix(AccountingFormat((dblNetSalesAdjustedLessTotalInvPayment < 0 ? (dblNetSalesAdjustedLessTotalInvPayment * -1) : dblNetSalesAdjustedLessTotalInvPayment)), intDesktopPrintLenght - 10, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, (dblNetSalesAdjustedLessTotalInvPayment < 0 ? 'Shortage' : 'Overage '), 8, strTotalNetSalesAdjustedLessTotalInvPayment, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");
                                                            //END Inventory of Payments Audit

                                                            //Sales Grouped by VAT TYPE----------------------------
                                                            if ((strFrom == strTo && $("#selPOSSalesReportRange").val() == "Day") ||
                                                                $("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                                                japplet.append("\n");
                                                                strPrint = SetTextAlign("Sales Grouped by VAT Type", 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");
                                                                japplet.append("\n");

                                                                var dblTotalVATSalesPercent = (dblVAT / dblTotalVAT) * 100;
                                                                var dblTotalVATAmountPercent = (dblVATAmount / dblTotalVAT) * 100;
                                                                var dblTotalNonVATPercent = (dblNonVAT / dblTotalVAT) * 100;
                                                                var dblTotalVATExemptPercent = (dblVATExempt / dblTotalVAT) * 100;
                                                                var dblTotalZeroRatedVATPercent = (dblZeroRatedVAT / dblTotalVAT) * 100;
                                                                var dblTotalVATPercent = dblTotalVATSalesPercent + dblTotalVATAmountPercent + dblTotalNonVATPercent + dblTotalVATExemptPercent + dblTotalZeroRatedVATPercent;

                                                                var strTotalVAT = SetPrefixPostfix(AccountingFormat(dblVAT), 14, ' ', 0);
                                                                var strTotalVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATSalesPercent) + '%', intDesktopPrintLenght - 32, ' ', 0);
                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('VAT-able Sales', 0, 14), 14, strTotalVAT, 30, strTotalVATPercent, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                var strTotalVATAmount = SetPrefixPostfix(AccountingFormat(dblVATAmount), 14, ' ', 0);
                                                                var strTotalVATAmountPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATAmountPercent) + '%', intDesktopPrintLenght - 32, ' ', 0);
                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('VAT Amount', 0, 14), 14, strTotalVATAmount, 30, strTotalVATAmountPercent, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                var strTotalNonVAT = SetPrefixPostfix(AccountingFormat(dblNonVAT), 14, ' ', 0);
                                                                var strTotalNonVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalNonVATPercent) + '%', intDesktopPrintLenght - 32, ' ', 0);
                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Non-VAT', 0, 14), 14, strTotalNonVAT, 30, strTotalNonVATPercent, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                var strTotalVATExempt = SetPrefixPostfix(AccountingFormat(dblVATExempt), 14, ' ', 0);
                                                                var strTotalVATExemptPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATExemptPercent) + '%', intDesktopPrintLenght - 32, ' ', 0);
                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('VAT-Exempt', 0, 14), 14, strTotalVATExempt, 30, strTotalVATExemptPercent, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                var strTotalZeroRatedVAT = SetPrefixPostfix(AccountingFormat(dblZeroRatedVAT), 14, ' ', 0);
                                                                var strTotalZeroRatedVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalZeroRatedVATPercent) + '%', intDesktopPrintLenght - 32, ' ', 0);
                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Zero-Rated VAT', 0, 14), 14, strTotalZeroRatedVAT, 30, strTotalZeroRatedVATPercent, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                var strTotalAllVAT = SetPrefixPostfix(AccountingFormat(dblTotalVAT), 14, ' ', 0);
                                                                var strTotalAllVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATPercent) + '%', intDesktopPrintLenght - 32, ' ', 0);
                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, '              ', 14, strTotalAllVAT, 30, strTotalAllVATPercent, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");
                                                            }
                                                            //End Sales Grouped by VAT TYPE----------------------------

                                                            japplet.append("\n");
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Printed by ', 0, 14), 14, ': ' + (vDataHeader.ReportPOS.Rows[0].PrintedBy || ''), intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Date and Time', 0, 14), 14, ': ' + ($.format.date(new Date(vDataHeader.ReportPOS.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            japplet.append("\n\n\n\n\n\n\n\n\n\n\n\n\n");
                                                            japplet.print();

                                                        } else {
                                                            japplet.append("\n\n\n\n\n\n\n\n\n\n\n\n\n");
                                                            japplet.print();
                                                        }
                                                    }
                                                });

                                            } else {

                                                //Get Payment INfo
                                                AjaxSend({
                                                    url: "ReportsPOS/getManagerSalesTransactionSummary",
                                                    data: pData,
                                                    success: function (vDataInfo) {

                                                        if (vDataInfo.ReportPOS.Rows.length > 0) {

                                                            if ($("#selPOSSalesReportRange").val() == "Day") {

                                                                strPrint = SetTextAlign("MANAGER'S SALES REPORT", 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign((strFrom == strTo ? 'Date: ' + strFrom : 'From ' + strFrom + ' To ' + strTo), 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                            } else if ($("#selPOSSalesReportRange").val() == "Week") {

                                                                strPrint = SetTextAlign("MANAGER'S", 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign("Same-Store Comparative Sales Report", 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign((strFrom == strTo ? 'Week: ' + strFrom : 'From Week ' + strFrom + ' To Week ' + strTo), 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign((vDataInfo.ReportPOS.Rows[0].YearFrom == vDataInfo.ReportPOS.Rows[0].YearTo ? "Year : " + vDataInfo.ReportPOS.Rows[0].YearFrom : "Year : " + vDataInfo.ReportPOS.Rows[0].YearFrom + "-" + vDataInfo.ReportPOS.Rows[0].YearTo), 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                            } else if ($("#selPOSSalesReportRange").val() == "Month") {

                                                                strPrint = SetTextAlign("MANAGER'S", 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign("Same-Store Comparative Sales Report", 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign((strFrom == strTo ? 'Month: ' + $.format.date(new Date(strFrom + '/1/2013'), 'MMM') : 'From ' + $.format.date(new Date(strFrom + '/1/2013'), 'MMM') + ' To ' + $.format.date(new Date(strTo + '/1/2013'), 'MMM')), 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign((vDataInfo.ReportPOS.Rows[0].YearFrom == vDataInfo.ReportPOS.Rows[0].YearTo ? "Year : " + vDataInfo.ReportPOS.Rows[0].YearFrom : "Year : " + vDataInfo.ReportPOS.Rows[0].YearFrom + "-" + vDataInfo.ReportPOS.Rows[0].YearTo), 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                            } else if ($("#selPOSSalesReportRange").val() == "Quarter") {

                                                                strPrint = SetTextAlign("MANAGER'S", 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign("Same-Store Comparative Sales Report", 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign((strFrom == strTo ? (strFrom == '1' ? "1st Quarter" : (strFrom == '2' ? "2nd Quarter" : (strFrom == '3' ? "3rd Quarter" : (strFrom == '4' ? "4th Quarter" : "")))) : 'From ' + (strFrom == '1' ? "1st Quarter" : (strFrom == '2' ? "2nd Quarter" : (strFrom == '3' ? "3rd Quarter" : (strFrom == '4' ? "4th Quarter" : "")))) + ' To ' + (strTo == '1' ? "1st Quarter" : (strTo == '2' ? "2nd Quarter" : (strTo == '3' ? "3rd Quarter" : (strTo == '4' ? "4th Quarter" : ""))))), 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign((vDataInfo.ReportPOS.Rows[0].YearFrom == vDataInfo.ReportPOS.Rows[0].YearTo ? "Year : " + vDataInfo.ReportPOS.Rows[0].YearFrom : "Year : " + vDataInfo.ReportPOS.Rows[0].YearFrom + "-" + vDataInfo.ReportPOS.Rows[0].YearTo), 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                            } else if ($("#selPOSSalesReportRange").val() == "Year") {


                                                                strPrint = SetTextAlign("MANAGER'S SALES REPORT", 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign('Year: ' + strYear, 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                            } else if ($("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                                                strPrint = SetTextAlign("MANAGER'S SALES REPORT", 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign('Date : ' + $.format.date(new Date(vDataInfo.ReportPOS.Rows[0].SalesDate), 'MM-dd-yyyy'), 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");
                                                            }

                                                            var dblTotalSales = 0;
                                                            var dblTotalSalesDiscount = 0;
                                                            var dblAdjRefund = 0;
                                                            var dblAdjSRV = 0;
                                                            var dblNetSales = 0;

                                                            var dblCash = 0;
                                                            var dblCheck = 0;
                                                            var dblDebitCard = 0;
                                                            var dblCreditCard = 0;
                                                            var dblSRV = 0;
                                                            var dblOtherPaymentType = 0;
                                                            var dblCharges = 0;
                                                            var dblSMAC = 0;
                                                            var dblGCash = 0;
                                                            var dblEPlus = 0;
                                                            var dblVoucher = 0;
                                                            var dblTotalPayment = 0;


                                                            var dblVAT = 0;
                                                            var dblVATAmount = 0;
                                                            var dblNonVAT = 0;
                                                            var dblVATExempt = 0;
                                                            var dblZeroRatedVAT = 0;
                                                            var dblTotalVAT = 0;

                                                            var intTransCount = 0;
                                                            var intSalesTransCount = 0;
                                                            var intSalesItemCount = 0;
                                                            var intSRVTransCount = 0;
                                                            var intSRVItemCount = 0;
                                                            var intRefTransCount = 0;
                                                            var intRefItemCount = 0;

                                                            for (var p = 0; p <= vDataInfo.ReportPOS.Rows.length - 1; p++) {

                                                                dblTotalSales = dblTotalSales + parseFloat(vDataInfo.ReportPOS.Rows[p].CurGrossSales || 0);
                                                                dblTotalSalesDiscount = dblTotalSalesDiscount + parseFloat(vDataInfo.ReportPOS.Rows[p].CurSalesDiscount || 0) + parseFloat(vDataInfo.Rows[p].CurSCDiscount || 0);
                                                                dblAdjRefund = dblAdjRefund + parseFloat(vDataInfo.ReportPOS.Rows[p].CurSalesReturnRefund || 0);
                                                                dblAdjSRV = dblAdjSRV + parseFloat(vDataInfo.ReportPOS.Rows[p].CurSalesReturnExchange || 0);

                                                                dblCash = dblCash + parseFloat(vDataInfo.ReportPOS.Rows[p].CashAmount || 0);
                                                                dblCheck = dblCheck + parseFloat(vDataInfo.ReportPOS.Rows[p].CheckPaymentAmount || 0);
                                                                dblDebitCard = dblDebitCard + parseFloat(vDataInfo.ReportPOS.Rows[p].DebitCardAmount || 0);
                                                                dblCreditCard = dblCreditCard + parseFloat(vDataInfo.ReportPOS.Rows[p].CreditCardAmount || 0);
                                                                dblSRV = dblSRV + parseFloat(vDataInfo.ReportPOS.Rows[p].SRVAmount || 0);
                                                                dblOtherPaymentType = dblOtherPaymentType + parseFloat(vDataInfo.ReportPOS.Rows[p].OtherTypeAmount || 0);
                                                                dblCharges = dblCharges + parseFloat(vDataInfo.ReportPOS.Rows[p].ChargeAmount || 0);
                                                                dblSMAC = dblSMAC + parseFloat(vDataInfo.ReportPOS.Rows[p].SMACAmount || 0);
                                                                dblGCash = dblGCash + parseFloat(vDataInfo.Rows[p].GCashAmount || 0);
                                                                dblEPlus = dblEPlus + parseFloat(vDataInfo.ReportPOS.Rows[p].EPlusAmount || 0);
                                                                dblVoucher = dblVoucher + parseFloat(vDataInfo.ReportPOS.Rows[p].VoucherAmount || 0);


                                                                dblVAT = dblVAT + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVatablePortionOfSCDTransaction || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVATSales || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVATSalesSalesReturn || 0);
                                                                dblVATAmount = dblVATAmount + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVatAmountPortionOfSCDTransaction || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVATAmount || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVATAmountSalesReturn || 0);
                                                                dblNonVAT = dblNonVAT + parseFloat(vDataInfo.ReportPOS.Rows[p].CurNoTAXSales || 0);
                                                                dblVATExempt = dblVATExempt + parseFloat(vDataInfo.Rows[p].CurVATExemptSales || 0);
                                                                dblZeroRatedVAT = dblZeroRatedVAT + parseFloat(vDataInfo.Rows[p].CurZeroRatedVATSales || 0);

                                                                intTransCount = intTransCount + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTransCountInv || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTransCountMemoReturnExchange || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTransCountMemoRefund || 0);
                                                                intSalesTransCount = intSalesTransCount + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTransCountInv || 0);
                                                                intSalesItemCount = intSalesItemCount + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTransInvItemCount || 0);
                                                                intSRVTransCount = intSRVTransCount + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTransCountMemoReturnExchange || 0);
                                                                intSRVItemCount = intSRVItemCount + parseFloat(vDataInfo.ReportPOS.Rows[p].CurSalesReturnExchangeItemCount || 0);
                                                                intRefTransCount = intRefTransCount + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTransCountMemoRefund || 0);
                                                                intRefItemCount = intRefItemCount + parseFloat(vDataInfo.ReportPOS.Rows[p].CurSalesRefundItemCount || 0);

                                                            };

                                                            dblCash = dblCash - dblAdjRefund;
                                                            dblNetSales = dblTotalSales - dblTotalSalesDiscount - dblAdjRefund - dblAdjSRV + dblCharges;
                                                            dblTotalPayment = dblCash + dblCheck + dblDebitCard + dblCreditCard + dblSRV + dblOtherPaymentType;

                                                            dblTotalVAT = dblVAT + dblVATAmount + dblNonVAT + dblVATExempt + dblZeroRatedVAT;

                                                            var strTotalSales = SetPrefixPostfix(AccountingFormat(dblTotalSales), intDesktopPrintLenght - 16, ' ', 0);
                                                            japplet.append("\n");
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Gross Sales ', 0, 12), 12, ': ' + strTotalSales, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            //Sales Discount
                                                            var strTotalSalesDiscount = SetPrefixPostfix(AccountingFormat(dblTotalSalesDiscount), intDesktopPrintLenght - 22, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Total Sales Discount', 20, strTotalSalesDiscount, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            //charges
                                                            if (dblCharges > 0) {
                                                                var strTotalCharges = SetPrefixPostfix(AccountingFormat(dblCharges), intDesktopPrintLenght - 22, ' ', 0);
                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Total Charges', 20, strTotalCharges, intDesktopPrintLenght, '', 0);
                                                                japplet.append(strPrint + "\n");
                                                            }


                                                            //Sales Adjustment
                                                            japplet.append("\n");
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Summary of Adjustments', 0, intDesktopPrintLenght), intDesktopPrintLenght, '', 0, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            var strAdjRefund = SetPrefixPostfix(AccountingFormat(dblAdjRefund), intDesktopPrintLenght - 28, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Total Sales Refund for the Period', 26, strAdjRefund, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            var strAdjSRV = SetPrefixPostfix(AccountingFormat(dblAdjSRV), intDesktopPrintLenght - 32, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Total Sales Returns Subject to Merchadise Exchange           (SRV Issued)', 30, strAdjSRV, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            var strNetSalesAdj = SetPrefixPostfix(AccountingFormat(parseFloat(dblAdjRefund) + parseFloat(dblAdjSRV)), intDesktopPrintLenght - 20, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Net of Adjustments', 18, strNetSalesAdj, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint);

                                                            strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                                            japplet.append('\n' + strPrint + "\n");

                                                            var strNetSales = SetPrefixPostfix(AccountingFormat(dblNetSales), intDesktopPrintLenght - 11, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Net Sales', 9, strNetSales, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            var strIssuedSRV = SetPrefixPostfix(AccountingFormat(dblAdjSRV), intDesktopPrintLenght - 18, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Add : Issued SRV', 16, strIssuedSRV, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint);

                                                            strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                                            japplet.append('\n' + strPrint + "\n");

                                                            var strTotals = SetPrefixPostfix(AccountingFormat(dblNetSales + dblAdjSRV), intDesktopPrintLenght - 20, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Net Sales Adjusted', 18, strTotals, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'for Exchanges', intDesktopPrintLenght, 0, '', '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            //END Sales Adjustment

                                                            //Sales Grouped By Mode of Payment
                                                            japplet.append("\n");
                                                            strPrint = SetTextAlign("Inventory of Payments", 1, intDesktopPrintLenght);
                                                            japplet.append(strPrint + "\n");
                                                            japplet.append("\n");

                                                            var strTotalCash = SetPrefixPostfix(AccountingFormat(dblCash), intDesktopPrintLenght - 16, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Cash        ', 0, 12), 12, ': ' + strTotalCash, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            var strTotalCheck = SetPrefixPostfix(AccountingFormat(dblCheck), intDesktopPrintLenght - 16, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Check       ', 0, 12), 12, ': ' + strTotalCheck, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            var strTotalDebitCard = SetPrefixPostfix(AccountingFormat(dblDebitCard), intDesktopPrintLenght - 16, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Debit Card  ', 0, 12), 12, ': ' + strTotalDebitCard, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            var strTotalCreditCard = SetPrefixPostfix(AccountingFormat(dblCreditCard), intDesktopPrintLenght - 16, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Credit Card ', 0, 12), 12, ': ' + strTotalCreditCard, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            var strTotalSRV = SetPrefixPostfix(AccountingFormat(dblSRV), intDesktopPrintLenght - 16, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('SRV         ', 0, 12), 12, ': ' + strTotalSRV, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");


                                                            if (IsFowstInSMMall) {
                                                                var strTotalSMAC = SetPrefixPostfix(AccountingFormat(dblSMAC), intDesktopPrintLenght - 16, ' ', 0);
                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('SMAC        ', 0, 12), 12, ': ' + strTotalSMAC, intDesktopPrintLenght, '', 0);
                                                                japplet.append(strPrint + "\n");

                                                                var strTotalEPlus = SetPrefixPostfix(AccountingFormat(dblEPlus), intDesktopPrintLenght - 16, ' ', 0);
                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('E-Plus      ', 0, 12), 12, ': ' + strTotalEPlus, intDesktopPrintLenght, '', 0);
                                                                japplet.append(strPrint + "\n");

                                                                var strTotalVoucher = SetPrefixPostfix(AccountingFormat(dblVoucher), intDesktopPrintLenght - 16, ' ', 0);
                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Voucher     ', 0, 12), 12, ': ' + strTotalVoucher, intDesktopPrintLenght, '', 0);
                                                                japplet.append(strPrint + "\n");

                                                            } else {
                                                                var strTotalOther = SetPrefixPostfix(AccountingFormat(dblOtherPaymentType), intDesktopPrintLenght - 16, ' ', 0);
                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Other Tender', 0, 12), 12, ': ' + strTotalOther, intDesktopPrintLenght, '', 0);
                                                                japplet.append(strPrint + "\n");

                                                            }

                                                            strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                                            japplet.append(strPrint + "\n");

                                                            var strTotalClassPayment = SetPrefixPostfix(AccountingFormat(dblTotalPayment), intDesktopPrintLenght - 17, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Total Inventory', 15, strTotalClassPayment, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'of Payments', intDesktopPrintLenght, 0, '', '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            var dblTotalOverPayment = dblTotalPayment - dblNetSales;
                                                            //////                                                        var strTotalOverPayment = SetPrefixPostfix(AccountingFormat((dblTotalOverPayment < 0 ? (dblTotalOverPayment * -1) : dblTotalOverPayment)), intDesktopPrintLenght - 16, ' ', 0);
                                                            var strTotalOverPayment = SetPrefixPostfix(AccountingFormat(dblTotalOverPayment), intDesktopPrintLenght - 26, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Excess of Inventory of', 24, strTotalOverPayment, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Payments vs Net Sales', intDesktopPrintLenght, 0, '', '', 0);
                                                            japplet.append(strPrint + "\n");
                                                            //END Sales Grouped By Mode of Payment

                                                            //Inventory of Payments Audit
                                                            japplet.append("\n");
                                                            strPrint = SetTextAlign("Inventory of Payments Audit", 1, intDesktopPrintLenght);
                                                            japplet.append(strPrint + "\n");
                                                            japplet.append("\n");

                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Net Sales Adjusted', 18, strTotals, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'for Exchanges', intDesktopPrintLenght, 0, '', '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Total Inventory', 15, strTotalClassPayment, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'of Payments', intDesktopPrintLenght, 0, '', '', 0);
                                                            japplet.append(strPrint + "\n");


                                                            strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                                            japplet.append(strPrint + "\n");

                                                            var dblNetSalesAdjustedLessTotalInvPayment = (dblNetSales + dblAdjSRV) - dblTotalPayment + dblCharges;
                                                            var strTotalNetSalesAdjustedLessTotalInvPayment = SetPrefixPostfix(AccountingFormat((dblNetSalesAdjustedLessTotalInvPayment < 0 ? (dblNetSalesAdjustedLessTotalInvPayment * -1) : dblNetSalesAdjustedLessTotalInvPayment)), intDesktopPrintLenght - 10, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, (dblNetSalesAdjustedLessTotalInvPayment < 0 ? 'Shortage' : 'Overage '), 8, strTotalNetSalesAdjustedLessTotalInvPayment, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");
                                                            //END Inventory of Payments Audit

                                                            //Sales Grouped by VAT TYPE----------------------------
                                                            if ((strFrom == strTo && $("#selPOSSalesReportRange").val() == "Day") ||
                                                                $("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                                                japplet.append("\n");
                                                                strPrint = SetTextAlign("Sales Grouped by VAT Type", 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");
                                                                japplet.append("\n");

                                                                var dblTotalVATSalesPercent = (dblVAT / dblTotalVAT) * 100;
                                                                var dblTotalVATAmountPercent = (dblVATAmount / dblTotalVAT) * 100;
                                                                var dblTotalNonVATPercent = (dblNonVAT / dblTotalVAT) * 100;
                                                                var dblTotalVATExemptPercent = (dblVATExempt / dblTotalVAT) * 100;
                                                                var dblTotalZeroRatedVATPercent = (dblZeroRatedVAT / dblTotalVAT) * 100;
                                                                var dblTotalVATPercent = dblTotalVATSalesPercent + dblTotalVATAmountPercent + dblTotalNonVATPercent + dblTotalVATExemptPercent + dblTotalZeroRatedVATPercent;

                                                                var strTotalVAT = SetPrefixPostfix(AccountingFormat(dblVAT), 14, ' ', 0);
                                                                var strTotalVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATSalesPercent) + '%', intDesktopPrintLenght - 32, ' ', 0);
                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('VAT-able Sales', 0, 14), 14, strTotalVAT, 30, strTotalVATPercent, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                var strTotalVATAmount = SetPrefixPostfix(AccountingFormat(dblVATAmount), 14, ' ', 0);
                                                                var strTotalVATAmountPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATAmountPercent) + '%', intDesktopPrintLenght - 32, ' ', 0);
                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('VAT Amount', 0, 14), 14, strTotalVATAmount, 30, strTotalVATAmountPercent, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                var strTotalNonVAT = SetPrefixPostfix(AccountingFormat(dblNonVAT), 14, ' ', 0);
                                                                var strTotalNonVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalNonVATPercent) + '%', intDesktopPrintLenght - 32, ' ', 0);
                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Non-VAT', 0, 14), 14, strTotalNonVAT, 30, strTotalNonVATPercent, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                var strTotalVATExempt = SetPrefixPostfix(AccountingFormat(dblVATExempt), 14, ' ', 0);
                                                                var strTotalVATExemptPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATExemptPercent) + '%', intDesktopPrintLenght - 32, ' ', 0);
                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('VAT-Exempt', 0, 14), 14, strTotalVATExempt, 30, strTotalVATExemptPercent, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                var strTotalZeroRatedVAT = SetPrefixPostfix(AccountingFormat(dblZeroRatedVAT), 14, ' ', 0);
                                                                var strTotalZeroRatedVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalZeroRatedVATPercent) + '%', intDesktopPrintLenght - 32, ' ', 0);
                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Zero-Rated VAT', 0, 14), 14, strTotalZeroRatedVAT, 30, strTotalZeroRatedVATPercent, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                var strTotalAllVAT = SetPrefixPostfix(AccountingFormat(dblTotalVAT), 14, ' ', 0);
                                                                var strTotalAllVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATPercent) + '%', intDesktopPrintLenght - 32, ' ', 0);
                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, '              ', 14, strTotalAllVAT, 30, strTotalAllVATPercent, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");
                                                            }
                                                            //End Sales Grouped by VAT TYPE----------------------------

                                                            //Get Sales by Cashier
                                                            AjaxSend({
                                                                url: "ReportsPOS/getManagerSalesReportPaymentInfoByCashier",
                                                                data: pData,
                                                                success: function (vDataSalesByCashier) {

                                                                    if ((strFrom == strTo && $("#selPOSSalesReportRange").val() == "Day") ||
                                                                    $("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                                                        if (vDataSalesByCashier.ReportPOS.Rows.length > 0) {

                                                                            japplet.append("\n");
                                                                            strPrint = SetTextAlign("Sales Grouped by Cashier", 1, intDesktopPrintLenght);
                                                                            japplet.append(strPrint + "\n");
                                                                            japplet.append("\n");

                                                                            //Get Total Sales
                                                                            var dblAllTotalSales = 0;
                                                                            for (var p = 0; p <= vDataSalesByCashier.ReportPOS.Rows.length - 1; p++) {
                                                                                dblAllTotalSales = dblAllTotalSales + parseFloat(vDataSalesByCashier.ReportPOS.Rows[p].TotalSales);
                                                                            };

                                                                            var dblAllTotalSalesPercent = 0;

                                                                            strPrint = '';
                                                                            for (var p = 0; p <= vDataSalesByCashier.ReportPOS.Rows.length - 1; p++) {

                                                                                var strTotalSales = SetPrefixPostfix(AccountingFormat((vDataSalesByCashier.ReportPOS.Rows[p].TotalSales || 0)), 14, ' ', 0);
                                                                                var dblTotalSalesPercent = (parseFloat(vDataSalesByCashier.ReportPOS.Rows[p].TotalSales || 0) / dblAllTotalSales) * 100;
                                                                                var strTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblTotalSalesPercent) + '%', intDesktopPrintLenght - 32, ' ', 0);

                                                                                dblAllTotalSalesPercent = dblAllTotalSalesPercent + dblTotalSalesPercent;

                                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, vDataSalesByCashier.ReportPOS.Rows[p].CashierName, 14, strTotalSales, 30, strTotalSalesPercent, intDesktopPrintLenght);
                                                                                japplet.append(strPrint + "\n");
                                                                            };

                                                                            var strAllTotalSales = SetPrefixPostfix(AccountingFormat(dblAllTotalSales), 14, ' ', 0);
                                                                            var strAllTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblAllTotalSalesPercent) + '%', intDesktopPrintLenght - 32, ' ', 0);

                                                                            strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                                                            japplet.append(strPrint + "\n");

                                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, '              ', 14, strAllTotalSales, 30, strAllTotalSalesPercent, intDesktopPrintLenght);
                                                                            japplet.append(strPrint + "\n");

                                                                        }
                                                                    }


                                                                    //Get Sales by Machine
                                                                    AjaxSend({
                                                                        url: "ReportsPOS/getManagerSalesReportPaymentInfoByMachine",
                                                                        data: pData,
                                                                        success: function (vDataSalesByMachine) {

                                                                            if ((strFrom == strTo && $("#selPOSSalesReportRange").val() == "Day") ||
                                                                            $("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                                                                if (vDataSalesByMachine.ReportPOS.Rows.length > 0) {

                                                                                    japplet.append("\n");
                                                                                    strPrint = SetTextAlign("Sales Grouped by Machine (MIN)", 1, intDesktopPrintLenght);
                                                                                    japplet.append(strPrint + "\n");
                                                                                    japplet.append("\n");

                                                                                    //Get Total Sales
                                                                                    dblAllTotalSales = 0;
                                                                                    for (var p = 0; p <= vDataSalesByMachine.ReportPOS.Rows.length - 1; p++) {
                                                                                        dblAllTotalSales = dblAllTotalSales + parseFloat(vDataSalesByMachine.ReportPOS.Rows[p].TotalSales);
                                                                                    };

                                                                                    dblAllTotalSalesPercent = 0;

                                                                                    strPrint = '';
                                                                                    for (var p = 0; p <= vDataSalesByMachine.ReportPOS.Rows.length - 1; p++) {

                                                                                        var strTotalSales = SetPrefixPostfix(AccountingFormat((vDataSalesByMachine.ReportPOS.Rows[p].TotalSales || 0)), 14, ' ', 0);
                                                                                        var dblTotalSalesPercent = (parseFloat(vDataSalesByMachine.ReportPOS.Rows[p].TotalSales || 0) / dblAllTotalSales) * 100;
                                                                                        var strTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblTotalSalesPercent) + '%', intDesktopPrintLenght - 32, ' ', 0);

                                                                                        dblAllTotalSalesPercent = dblAllTotalSalesPercent + dblTotalSalesPercent;

                                                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, vDataSalesByMachine.ReportPOS.Rows[p].DeviceName, 14, strTotalSales, 30, strTotalSalesPercent, intDesktopPrintLenght);
                                                                                        japplet.append(strPrint + "\n");
                                                                                    };

                                                                                    strAllTotalSales = SetPrefixPostfix(AccountingFormat(dblAllTotalSales), 14, ' ', 0);
                                                                                    strAllTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblAllTotalSalesPercent) + '%', intDesktopPrintLenght - 32, ' ', 0);

                                                                                    strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                                                                    japplet.append(strPrint + "\n");

                                                                                    strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, '              ', 14, strAllTotalSales, 30, strAllTotalSalesPercent, intDesktopPrintLenght);
                                                                                    japplet.append(strPrint + "\n");

                                                                                }
                                                                            }

                                                                            //Get Sales by Cashier and Machine
                                                                            AjaxSend({
                                                                                url: "ReportsPOS/getManagerSalesReportPaymentInfoByCashierMachine",
                                                                                data: pData,
                                                                                success: function (vDataSalesByCashierMachine) {

                                                                                    if ((strFrom == strTo && $("#selPOSSalesReportRange").val() == "Day") ||
                                                                                    $("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                                                                        if (vDataSalesByCashierMachine.ReportPOS.Rows.length > 0) {

                                                                                            japplet.append("\n");
                                                                                            strPrint = SetTextAlign("Sales Grouped by Cashier & Machine (MIN)", 1, intDesktopPrintLenght);
                                                                                            japplet.append(strPrint + "\n");
                                                                                            japplet.append("\n");

                                                                                            //Get Total Sales
                                                                                            dblAllTotalSales = 0;
                                                                                            for (var p = 0; p <= vDataSalesByCashierMachine.ReportPOS.Rows.length - 1; p++) {
                                                                                                dblAllTotalSales = dblAllTotalSales + parseFloat(vDataSalesByCashierMachine.ReportPOS.Rows[p].TotalSales);
                                                                                            };

                                                                                            dblAllTotalSalesPercent = 0;

                                                                                            var strCashierName = "";
                                                                                            var strDeviceName = "";
                                                                                            var dblDeviceSales = 0;

                                                                                            dblAllTotalSalesPercent = 0;

                                                                                            strPrint = '';
                                                                                            for (var p = 0; p <= vDataSalesByCashierMachine.ReportPOS.Rows.length - 1; p++) {

                                                                                                if (strCashierName != vDataSalesByCashierMachine.ReportPOS.Rows[p].CashierName) {

                                                                                                    strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign(vDataSalesByCashierMachine.ReportPOS.Rows[p].CashierName, 0, intDesktopPrintLenght), intDesktopPrintLenght, '', 0, '', 0);
                                                                                                    japplet.append(strPrint + "\n");

                                                                                                    strCashierName = vDataSalesByCashierMachine.ReportPOS.Rows[p].CashierName;
                                                                                                }

                                                                                                dblDeviceSales = parseFloat(vDataSalesByCashierMachine.ReportPOS.Rows[p].TotalSales || 0);
                                                                                                strTotalSales = SetPrefixPostfix(AccountingFormat(dblDeviceSales), 14, ' ', 0);
                                                                                                dblTotalSalesPercent = (dblDeviceSales / dblAllTotalSales) * 100;
                                                                                                strTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblTotalSalesPercent) + '%', intDesktopPrintLenght - 32, ' ', 0);

                                                                                                dblAllTotalSalesPercent = dblAllTotalSalesPercent + dblTotalSalesPercent;

                                                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('   ' + vDataSalesByCashierMachine.ReportPOS.Rows[p].DeviceName, 0, 14), 14, strTotalSales, 30, strTotalSalesPercent, intDesktopPrintLenght);
                                                                                                japplet.append(strPrint + "\n");

                                                                                            };

                                                                                            strAllTotalSales = SetPrefixPostfix(AccountingFormat(dblAllTotalSales), 14, ' ', 0);
                                                                                            strAllTotalSalesPercent = SetPrefixPostfix(AccountingFormat(dblAllTotalSalesPercent) + '%', intDesktopPrintLenght - 32, ' ', 0);

                                                                                            strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                                                                            japplet.append(strPrint + "\n");

                                                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, '              ', 14, strAllTotalSales, 30, strAllTotalSalesPercent, intDesktopPrintLenght);
                                                                                            japplet.append(strPrint + "\n");
                                                                                        }
                                                                                    }

                                                                                    if ($("#selPOSSalesReportRange").val() == "Day" ||
                                                                                    $("#selPOSSalesReportRange").val() == "Today" ||
                                                                                    $("#selPOSSalesReportRange").val() == "Yesterday") {

                                                                                        //Other Informatoin
                                                                                        japplet.append("\n");
                                                                                        strPrint = SetTextAlign('Other Information', 1, intDesktopPrintLenght);
                                                                                        japplet.append(strPrint + "\n\n");

                                                                                        var strTransCount = SetPrefixPostfix(intTransCount + '', intDesktopPrintLenght - 20, ' ', 0);
                                                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Total Trans. Count', 0, 18), 18, strTransCount + '', intDesktopPrintLenght, '', 0);
                                                                                        japplet.append(strPrint + "\n");

                                                                                        var strSalesTransCount = SetPrefixPostfix(intSalesTransCount + '', intDesktopPrintLenght - 20, ' ', 0);
                                                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Sales Trans. Count', 0, 18), 18, strSalesTransCount + '', intDesktopPrintLenght, '', 0);
                                                                                        japplet.append(strPrint + "\n");

                                                                                        var strSalesItemCount = SetPrefixPostfix(intSalesItemCount + '', intDesktopPrintLenght - 23, ' ', 0);
                                                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Sales Line Item Count', 0, 21), 21, strSalesItemCount + '', intDesktopPrintLenght, '', 0);
                                                                                        japplet.append(strPrint + "\n");

                                                                                        var strSRVTransCount = SetPrefixPostfix(intSRVTransCount + '', intDesktopPrintLenght - 18, ' ', 0);
                                                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('SRV Trans. Count', 0, 16), 16, strSRVTransCount + '', intDesktopPrintLenght, '', 0);
                                                                                        japplet.append(strPrint + "\n");

                                                                                        var strSRVItemCount = SetPrefixPostfix(intSRVItemCount + '', intDesktopPrintLenght - 21, ' ', 0);
                                                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('SRV Line Item Count', 0, 19), 19, strSRVItemCount + '', intDesktopPrintLenght, '', 0);
                                                                                        japplet.append(strPrint + "\n");

                                                                                        var strRefTransCount = SetPrefixPostfix(intRefTransCount + '', intDesktopPrintLenght - 21, ' ', 0);
                                                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Refund Trans. Count', 0, 19), 19, strRefTransCount + '', intDesktopPrintLenght, '', 0);
                                                                                        japplet.append(strPrint + "\n");

                                                                                        var strRefItemCount = SetPrefixPostfix(intRefItemCount + '', intDesktopPrintLenght - 24, ' ', 0);
                                                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Refund Line Item Count', 0, 22), 22, strRefItemCount + '', intDesktopPrintLenght, '', 0);
                                                                                        japplet.append(strPrint + "\n");

                                                                                        var strCustomerCount = SetPrefixPostfix(intSalesTransCount + '', intDesktopPrintLenght - 16, ' ', 0);
                                                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Total Customer', 0, 14), 14, strCustomerCount + '', intDesktopPrintLenght, '', 0);
                                                                                        japplet.append(strPrint + "\n");
                                                                                        //END Other Informatoin

                                                                                        japplet.append("\n");
                                                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Printed by ', 0, 14), 14, ': ' + (vDataHeader.ReportPOS.Rows[0].PrintedBy || ''), intDesktopPrintLenght, '', 0);
                                                                                        japplet.append(strPrint + "\n");

                                                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Date and Time', 0, 14), 14, ': ' + ($.format.date(new Date(vDataHeader.ReportPOS.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intDesktopPrintLenght, '', 0);
                                                                                        japplet.append(strPrint + "\n");

                                                                                        japplet.append("\n\n\n\n\n\n\n\n\n\n\n\n\n");
                                                                                        japplet.print();

                                                                                    } else {
                                                                                        japplet.append("\n");
                                                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Printed by ', 0, 14), 14, ': ' + (vDataHeader.ReportPOS.Rows[0].PrintedBy || ''), intDesktopPrintLenght, '', 0);
                                                                                        japplet.append(strPrint + "\n");

                                                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Date and Time', 0, 14), 14, ': ' + ($.format.date(new Date(vDataHeader.ReportPOS.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intDesktopPrintLenght, '', 0);
                                                                                        japplet.append(strPrint + "\n");

                                                                                        japplet.append("\n\n\n\n\n\n\n\n\n\n\n\n\n");
                                                                                        japplet.print();
                                                                                    }
                                                                                }

                                                                            });

                                                                        }

                                                                    });
                                                                }
                                                            });

                                                        } else {
                                                            japplet.append("\n\n\n\n\n\n\n\n\n\n\n\n\n");
                                                            japplet.print();
                                                        }
                                                    }
                                                });

                                            }
                                        }
                                    }
                                });


                            }

                        }

                    } else {
                        MessageBox("Jzebra applet was not loaded.",
                                "Jzebra",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                    );
                    }

                }


                function UpdateCSRPrintCount() {
                    var pData = {};

                    if (IsMobile) {
                        pData.FOWSTID = sessionStorage.FowstID;
                        pData.DeviceID = Web.DeviceID();

                        AjaxSend({
                            url: "ReportsPOS/UpdateCSRPrintCount",
                            data: pData,
                            success: function (vDataGen) {

                            },
                            error: function (e) {

                            }
                        });
                    }
                }

                $('#btnRecomputeFromLocalDatabase').click(function () {

                    var dblTotalSalesMSR = 0;
                    var dblTotalSalesDiscountMSR = 0;
                    var dblTotalForeGoneVATMSR = 0;
                    var dblAdjRefundMSR = 0;
                    var dblAdjSRVMSR = 0;
                    var dblNetSalesMSR = 0;

                    var dblCashMSR = 0;
                    var dblCheckMSR = 0;
                    var dblDebitCardMSR = 0;
                    var dblCreditCardMSR = 0;
                    var dblSRVMSR = 0;
                    var dblOtherPaymentTypeMSR = 0;
                    var dblOnCreditMSR = 0;
                    var dblChargesMSR = 0;
                    var dblTotalPaymentMSR = 0;

                    var dblVATMSR = 0;
                    var dblVATAmountMSR = 0;
                    var dblNonVATMSR = 0;
                    var dblVATExemptMSR = 0;
                    var dblZeroRatedVATMSR = 0;
                    var dblTotalVATMSR = 0;

                    var dblTotalVATSalesMSR = 0;
                    var dblTotalVATAmountMSR = 0;
                    var dblTotalVATSalesSalesReturnMSR = 0;
                    var dblTotalVATAmountSalesReturnMSR = 0;

                    var pSql = "Select SUM(case when quantity=(0) then (0) when (((ifnull(vatablesale,(0))+ifnull(vatexemptsale,(0)))+ifnull(zeroratedsale,(0)))+ifnull(nonvatsale,(0)))=(0) then ifnull(subtotal,(0)) else ((ifnull(vatablesale,(0))+ifnull(vatexemptsale,(0)))+ifnull(zeroratedsale,(0)))+ifnull(nonvatsale,(0)) end) CurGrossSales, " +
                    "SUM(IFNULL(invitem.DiscountAmount,0)) + inv.SCDiscount CurSalesDiscount, " +
                    "inv.ForeGoneVAT CurVATExemptTax, " +
                    "SUM(IFNULL(invitem.TaxSales,0) - IFNULL(invitem.DiscountSales,0)) CurTotalVATSales, " +
                    "SUM(IFNULL(invitem.TaxAmount,0)- IFNULL((case when vatexemptsale>(0) then (0) else invitem.DiscountAmount-invitem.DiscountSales end),0)) CurTotalVATAmount, " +
                    "SUM(invitem.NonVATSale) CurNoTAXSales, " +
                    "SUM(invitem.VATExemptSale) CurVATExemptSales, " +
                    "SUM(invitem.ZeroRatedSale) CurZeroRatedVATSales " +
                    "FROM TransCustomerPaymentInvoices custpayinv " +
                    " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                    " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                    " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " +
                    "Where date(inv.InvoiceDate) = " + ReadyString($.format.date(new Date(dateFrom), 'yyyy-MM-dd'))

                    var vDataInvoice = Sql.Execute(pSql);

                    pSql = "Select SUM(IFNULL(custpay.CashAmount,0) - IFNULL(custpay.CustomerChangeCash,0)) CashAmount, " +
                    "SUM(IFNULL(custpay.DebitCardAmount,0)) DebitCardAmount, " +
                    "SUM(IFNULL(custpay.CreditCardAmount,0)) CreditCardAmount, " +
                    "SUM(IFNULL(custpay.CheckPaymentAmount,0)) CheckPaymentAmount, " +
                    "SUM(IFNULL(custpay.SRVAmount,0)) SRVAmount, " +
                    "SUM(IFNULL(custpay.OtherTypeAmount,0)) OtherTypeAmount, " +
                    "SUM(IFNULL(custpay.OnCreditAmount,0)) OnCreditAmount, " +
                    "SUM(IFNULL(custpay.ChargeAmount,0)) ChargeAmount " +
                    "FROM TransCustomerPaymentInvoices custpayinv " +
                    " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                    " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                    "Where date(inv.InvoiceDate) = " + ReadyString($.format.date(new Date(dateFrom), 'yyyy-MM-dd'))

                    var vDataPayments = Sql.Execute(pSql);

                    if (vDataInvoice.Rows.length > 0) {
                        dblTotalSales = CDec2(vDataInvoice.Rows[0].CurGrossSales)
                        dblTotalSalesDiscount = CDec2(vDataInvoice.Rows[0].CurSalesDiscount);
                        dblTotalForeGoneVAT = CDec2(vDataInvoice.Rows[0].CurVATExemptTax);

                        dblTotalVATSales = CDec2(vDataInvoice.Rows[0].CurTotalVATSales || 0); //- parseFloat(vDataInfo.Rows[p].CurTotalVATSalesSalesReturn || 0);
                        dblTotalVATAmount = CDec2(vDataInvoice.Rows[0].CurTotalVATAmount || 0); // - parseFloat(vDataInfo.Rows[p].CurTotalVATAmountSalesReturn || 0);
                        dblNonVAT = CDec2(vDataInvoice.Rows[0].CurNoTAXSales || 0);
                        dblVATExempt = CDec2(vDataInvoice.Rows[0].CurVATExemptSales || 0);
                        dblZeroRatedVAT = CDec2(vDataInvoice.Rows[0].CurZeroRatedVATSales || 0);

                        dblTotalSalesMSR = CDec2(vDataInvoice.Rows[0].CurGrossSales)
                        dblTotalSalesDiscountMSR = CDec2(vDataInvoice.Rows[0].CurSalesDiscount);
                        dblTotalForeGoneVATMSR = CDec2(vDataInvoice.Rows[0].CurVATExemptTax);

                        dblTotalVATSalesMSR = CDec2(vDataInvoice.Rows[0].CurTotalVATSales || 0); //- parseFloat(vDataInfo.Rows[p].CurTotalVATSalesSalesReturn || 0);
                        dblTotalVATAmountMSR = CDec2(vDataInvoice.Rows[0].CurTotalVATAmount || 0); // - parseFloat(vDataInfo.Rows[p].CurTotalVATAmountSalesReturn || 0);
                        dblNonVATMSR = CDec2(vDataInvoice.Rows[0].CurNoTAXSales || 0);
                        dblVATExemptMSR = CDec2(vDataInvoice.Rows[0].CurVATExemptSales || 0);
                        dblZeroRatedVATMSR = CDec2(vDataInvoice.Rows[0].CurZeroRatedVATSales || 0);

                        dblTotalNumberOfCustomers = CDec2(sessionStorage.TotalNumberOfCustomer);
                    }

                    if (vDataPayments.Rows.length > 0) {

                        dblCash = parseFloat(vDataPayments.Rows[0].CashAmount || 0);
                        dblCheck = parseFloat(vDataPayments.Rows[0].CheckPaymentAmount || 0);
                        dblDebitCard = parseFloat(vDataPayments.Rows[0].DebitCardAmount || 0);
                        dblCreditCard = parseFloat(vDataPayments.Rows[0].CreditCardAmount || 0);
                        dblSRV = parseFloat(vDataPayments.Rows[0].SRVAmount || 0);
                        dblOtherPaymentType = parseFloat(vDataPayments.Rows[0].OtherTypeAmount || 0);
                        dblOnCredit = parseFloat(vDataPayments.Rows[0].OnCreditAmount || 0);
                        dblCharges = parseFloat(vDataPayments.Rows[0].ChargeAmount || 0);

                        dblCashMSR = parseFloat(vDataPayments.Rows[0].CashAmount || 0);
                        dblCheckMSR = parseFloat(vDataPayments.Rows[0].CheckPaymentAmount || 0);
                        dblDebitCardMSR = parseFloat(vDataPayments.Rows[0].DebitCardAmount || 0);
                        dblCreditCardMSR = parseFloat(vDataPayments.Rows[0].CreditCardAmount || 0);
                        dblSRVMSR = parseFloat(vDataPayments.Rows[0].SRVAmount || 0);
                        dblOtherPaymentTypeMSR = parseFloat(vDataPayments.Rows[0].OtherTypeAmount || 0);
                        dblOnCreditMSR = parseFloat(vDataPayments.Rows[0].OnCreditAmount || 0);
                        dblChargesMSR = parseFloat(vDataPayments.Rows[0].ChargeAmount || 0);
                    }

                    var ReportDate;
                    ReportDate = new Date(dateFrom);

                    var LastTotalDataMSR = Transfers.ReadFile(sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + ($.format.number(ReportDate.getFullYear(), '00').toString() + '-' + $.format.number((ReportDate.getMonth() + 1), '00').toString() + '-' + ReportDate.getDate().toString()) + '.dat');

                    if (LastTotalDataMSR != undefined) {
                        var LastTotal = LastTotalDataMSR.split('|');
                        dblTotalVATSalesSalesReturnMSR = CDec2(LastTotal[16]);
                        dblTotalVATAmountSalesReturnMSR = CDec2(LastTotal[17]);
                        dblTotalForeGoneVATMSR += CDec2(LastTotal[20]);
                    }

                    if (dblTotalSalesMSR > 0) {
                        Transfers.WriteFile(sessionStorage.OrgID + sessionStorage.FowstID + Web.DeviceID() + ($.format.number(ReportDate.getFullYear(), '00').toString() + '-' + $.format.number((ReportDate.getMonth() + 1), '00').toString() + '-' + ReportDate.getDate().toString()) + '.dat', null,
                                            dblTotalSalesMSR + '|' + dblTotalSalesDiscountMSR + '|' + dblAdjRefundMSR + '|' + dblAdjSRVMSR + '|' +
                                                dblCashMSR + '|' + dblCheckMSR + '|' + dblDebitCardMSR + '|' + dblCreditCardMSR + '|' + dblSRVMSR + '|' +
                                                dblVATMSR + '|' + dblVATAmountMSR + '|' + dblNonVATMSR + '|' + dblVATExemptMSR + '|' + dblZeroRatedVATMSR + '|' +
                                                dblTotalVATSalesMSR + '|' + dblTotalVATAmountMSR + '|' + dblTotalVATSalesSalesReturnMSR + '|' + dblTotalVATAmountSalesReturnMSR + '|' +
                                                dblOtherPaymentTypeMSR + '|' + dblChargesMSR + '|' + dblTotalForeGoneVATMSR + '|' + dblTotalNumberOfCustomers + '|' + dblOnCreditMSR);
                    }

                    IsViewOnly = 1;

                    DoPrintSalesReport();
                })

            });
        }); // require SalesReport_offline
        var pDeferred = $.Deferred();
        return pDeferred.promise();
    });