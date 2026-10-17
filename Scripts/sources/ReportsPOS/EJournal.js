require([
    'common',
    'jquery.plugin.html2canvas',
    GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc='),
    (OfflineMode ? '/Scripts3/Offline/Browser/LibraryFowst.js?crc=' : '')
    ], function () {

        var reqJS;

        if (OfflineMode) {
            reqJS = [
                '/Scripts/sources/SalesPayments/SalesPaymentsExport_offline.js?crc=',
                '/Scripts/sources/SalesPayments/SalesPayments_offline.js?crc='
            ]
        }

        require(reqJS, function () {


            $(function () {

                var IsFowstInSMMall = false;
                var intFAFCTotalLenght = 40;
                var intFBTotalLenght = 27;
                var intFCDoubleTotalLenght = 18;

                var intDesktopPrintLenght = 42;

                var flushedCounter = 0;
                var mInterval;

                var dateRange;
                var rangeType;
                var dateFrom;
                var dateTo;
                var isPrintInWPS = false;
                var strPrinterIP = '';
                var fowstID = '';

                setMonthCollection("selZReportByDateMonth");
                setYearCollection("selZReportByDateYear");

                if (IsMobile) {
                    if (Web.HandleOrganization() == false)
                        return;
                }

                $(document).ready(function () {

                    if (!window.InAndroid) {
                        $('<applet name="jzebra" code="jzebra.PrintApplet.class" archive="/Scripts/jzebra.jar" width="10" height="10"></applet>').appendTo($('body'));
                    }

                    window.pageZReportPage = $.mobile.activePage;

                    //if (sessionStorage.FowstID == undefined || sessionStorage.FowstID == "" || sessionStorage.FowstID == null) {
                    //    GetFowst();
                    //}

                    GetAllFOWST()

                    $('#txtEJournalDateFrom').val($.format.date(new Date(), 'yyyy-MM-01'));
                    $('#txtEJournalDateTo').val($.format.date(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0), 'yyyy-MM-dd'));
                });

                $(".ZreportfromCalendar").calendar({
                    format: "mmmm dd, yyyy",
                    multiSelect: false,
                    otherDays: true,
                    getDates: function (data) {
                        $("#txtZreportDateFrom").text(data);
                    }
                });

                $(".ZreporttoCalendar").calendar({
                    format: "mmmm dd, yyyy",
                    multiSelect: false,
                    otherDays: true,
                    getDates: function (data) {
                        $("#txtZreportDateTo").text(data);
                    }
                });

                $('#selZReportByDateMonth').change(function (e) {
                    e.preventDefault();

                    setDayCollection('selZReportByDateMonth', 'selZReportByDateDay', 'selZReportByDateYear', true);

                });

                $('#selZReportByDateYear').change(function (e) {
                    e.preventDefault();

                    setDayCollection('selZReportByDateMonth', 'selZReportByDateDay', 'selZReportByDateYear', true);

                });

                $('#selZReportRange').change(function () {

                    $("#divZReportByDate").hide();

                    if ($('#selZReportRange').val() == "By Date") {
                        $("#divZReportByDate").show();
                    }

                });

                $('#btnPOSSalesZReportPrintTemp').click(function (e) {

                    e.preventDefault();

                    PrintDesktopSalesZReportByMINTemp();

                });

                $('body').on('click', '#btnPOSSalesZReportView', function (e) {
                    e.preventDefault();
                    ViewZReport();

                    // commented on 2016_01_09__13_46_34_5388
                    ////Flush First before printing the report
                    //if (OfflineMode) {
                    //    flushedCounter = 0;
                    //    mInterval = setInterval(FlushDeviceData, 1000);
                    //} else {
                    //    PrintSalesZReport(true);
                    //}
                })
                .on('click', '#btnPOSSalesZReportPrint', function (e) {
                    e.preventDefault();

                    PrintSalesZReport(true); //true = view z-read on display

                    if (window.InAndroid) {
                        PrintSalesZReport(false);
                    } else {
                        if (window.InWinRt)
                            PrintSalesZReport(false);
                        else
                            PrintDesktopSalesZReport();
                    }

                    UpdateZReportPrintCount();

                })
                .on('click', '#btnPOSSalesZReportEOD', function (e) {
                    e.preventDefault();

                    CloseForSMCOIN();
                })
                .on('click', '#btnPOSSalesEJournal', function (e) {
                    e.preventDefault();

                    //if ($("#selZReportRange").val() == "") {
                    //    MessageBox("Please select report range.",
                    //            "Report Range",
                    //            [
                    //                {
                    //                    Text: 'Ok'
                    //                }
                    //            ]
                    //        );
                    //    return;
                    //}

                    //if ($("#selZReportRange").val() == "By Date") {
                    //    if ($.trim($("#selZReportByDateMonth").val()) == "" ||
                    //    $.trim($("#selZReportByDateDay").val()) == "" ||
                    //    $.trim($("#selZReportByDateYear").val()) == "") {

                    //        MessageBox("Please enter the date of Z-Read.",
                    //                "Date",
                    //                [
                    //                    {
                    //                        Text: 'Ok'
                    //                    }
                    //                ]
                    //        );
                    //        return;
                    //    }

                    //}

                    //strDate = $("#selZReportByDateMonth").val() + "/" + $("#selZReportByDateDay").val() + "/" + $("#selZReportByDateYear").val();

                    DownloadEJournal();

                })
                .on('click', '#btnPOSSalesZReportCancel', function (e) {
                    e.preventDefault();

                    window.open(GetPathFormat("/People/PeopleMenu"), "_self");
                })
                .on('click', '.btnEJournaleSelectFowst', function (e) {
                    e.preventDefault();
                    var dis = $(this);
                    BrowsePopup('LibraryFowst', 'Select Store', [
                        {
                            AutoSearch: true,
                            DisplayText: [{ Column: 'FowstName' }],
                            Click: function (e) {
                                fowstID = e.ID;
                                dis.text(e.FowstName);

                                var selEJournalFOWSTDevices = $('#selEJournalFOWSTDevices').empty();

                                AjaxSend({
                                    url: 'Browser/LibraryDeviceFowst',
                                    data: { FOWSTID: fowstID },
                                    success: function (vDevice) {

                                        selEJournalFOWSTDevices.append(new Option('Please select machine', ''));

                                        for (var i in vDevice.Results.Rows) {
                                            selEJournalFOWSTDevices.append(new Option(vDevice.Results.Rows[i].PhoneName, vDevice.Results.Rows[i].DeviceID));
                                        }
                                    }
                                })
                            }
                        }
                    ], { FilterFowst: 1 })
                    //$('#panZReportFowst').popup('open');
                })
                .on('click', '#btnZReportDateRange', function (e) {
                    $('#panZReportDate').popup('open');
                })
                .on('click', '#liZReportYesterday', function (e) {
                    e.preventDefault();
                    rangeType = "Yesterday";
                    $('#ZReportDateRangeFrom').text(rangeType);
                    $('#ZReportDateRangeSeparator, #ZReportDateRangeTo').hide();
                    ViewZReport();
                    $('#panZReportDate').popup('close');
                })
                .on('click', '#liZReportToday', function (e) {
                    e.preventDefault();
                    rangeType = "Today";
                    $('#ZReportDateRangeFrom').text(rangeType);
                    $('#ZReportDateRangeSeparator, #ZReportDateRangeTo').hide();
                    ViewZReport();
                    $('#panZReportDate').popup('close');
                })
                .on('click', '#liZReportCustomPeriod', function (e) {
                    rangeType = "By Date";
                    $('#panZReportDate').popup('close');
                    $('#panZreportCustomDate').popup('open');
                })
                .on('click', '#btnZreportCancel', function (e) {
                    $('#panZreportCustomDate').popup('close');
                })
                .on('click', '#btnZreportDone', function (e) {
                    dateFrom = new Date($('#txtZreportDateFrom').text());
                    dateTo = new Date($('#txtZreportDateTo').text());
                    var dateNow = new Date();

                    if ((dateFrom > dateTo || dateFrom > dateNow || dateTo > dateNow) || $('#txtZreportDateFrom').text() == "" || $('#txtZreportDateTo').text() == "") {
                        $('#spanZreportDateRangeError').show();
                    }
                    else {
                        $('#spanZreportDateRangeError').hide();
                        $('#btnZReportDateRange').removeClass('btnRequired').addClass('btnNotRequired').attr('title', $('#txtZreportDateFrom').text() + ' to ' + $('#txtZreportDateTo').text())
                        $('#ZReportDateRangeFrom').text($('#txtZreportDateFrom').text());
                        $('#ZReportDateRangeTo').text($('#txtZreportDateTo').text());
                        $('#ZReportDateRangeSeparator').show();
                        $('#panZreportCustomDate').popup('close');
                        dateFrom = $('#txtZreportDateFrom').text();
                        dateTo = $('#txtZreportDateTo').text();
                        dateYear = '';
                        newRangeType = "Day";

                        if ($('#lblZReportSelectedFowstGroup').text() != "FOWST Location") {
                            ViewZReport();
                        }
                    }
                });

                function DownloadEJournal() {
                    var pParams = {
                        OrgID: sessionStorage.OrgID,
                        FOWSTID: fowstID,
                        FowstID: fowstID, //for CSR
                        DeviceID: $('#selEJournalFOWSTDevices option:selected').val(),
                        From: $('#txtEJournalDateFrom').val(),
                        To: $('#txtEJournalDateTo').val(),
                        GenerateZeroSales: false,
                        ForceNONVAT: true,
                        DownloadDataOnly: true,
                        GetFromRawData: true,
                        ExcludeInvoice: false,
                        ExcludeZRead: false,
                        ExcludeXRead: false,
                        ExcludeReturns: false
                    }

                    Bs.Loading('show');

                    AjaxSend({
                        url: 'ExternalServices/DownloadNewEJournal',
                        data: pParams,
                        timeout: 600000,
                        success: function (vData) {
                            if (vData != null) {
                                NewMessageBox('Your e-Journal has been generated. Please download the file.');
                                window.location = GetPathFormat("/ReportsPOS/DownloadReportFile?OrgID=" + sessionStorage.OrgID + '&ReportLink=' + vData.ReportPOS);
                            }
                        }, complete: function () {
                            Bs.Loading('hide');
                        }
                    })
                    return
                    var pData = {};

                    pData.RangeType = rangeType || 'Today'; //$.trim($("#selZReportRange").val());
                    pData.DateFrom = $('#txtEJournalDateFrom').val();
                    pData.DateTo = $('#txtEJournalDateTo').val();
                    pData.FOWSTID = fowstID;

                    if (IsMobile) {
                        pData.DeviceID = Web.DeviceID();
                    }
                    //else {
                    //    pData.DeviceID = '';
                    //}

                   
                        //Get X-Report Info
                    AjaxSend({
                        url: "ReportsPOS/getEJournal",
                        data: pData,
                        success: function (vData) {
                            if (OfflineMode) {
                                Transfers.Download({
                                    Source: 'ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + vData.ReportPOS,
                                    Destination: vData.ReportPOS,
                                    Success: function (vData) {
                                        alert('Downloading EJournal Successful');
                                    }
                                });
                            } else {
                                window.location = GetPathFormat('ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + vData.ReportPOS);
                            }
                        }
                    });
                }

                function ViewZReport() {
                    return;
                    if (OfflineMode) {
                        // 2016_01_09__13_43_09_6615
                        // dont flush anymore.
                        PrintSalesZReport(true);

                        //flushedCounter = 0;
                        //mInterval = setInterval(FlushDeviceData, 1000);
                    } else {
                        PrintSalesZReport(true);
                    }
                }

                function GetAllFOWST() {
                    if (OfflineMode) {
                        //Sql.AttachDatabase(sessionStorage.OrgID);
                        if (Web.HandleOrganization() == false)
                            return;
                        var vData = OfflineBrowser.LibraryFowst.GetData({
                            FilterFowst: 1
                        });
                        ShowFOWST({ Results: vData });
                    }
                    else {
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


                    pLV = $('#listZReportFowst');
                    pLV.empty();

                    var vRows = vData.Results;

                    if (vRows.Rows.length == 1) {
                        if (vRows.Rows[0].IsFOWSTInsideAMall == 1 && vRows.Rows[0].FOWSTMallName == 'SM Mall') {
                            IsFowstInSMMall = true;
                        } else {
                            IsFowstInSMMall = false;
                        }

                        sessionStorage.FowstID = vRows.Rows[0].ID;
                        sessionStorage.SelectedFOWST = vRows.Rows[0].ID;
                        $('.lblZReportSelectedFowstGroup').text(vRows.Rows[0].FowstName);
                        $('#panZReportFowst').popup('close');
                        $('.btnZReportSelectFowst').removeClass('btnRequired').addClass('btnNotRequired');
                        ViewZReport();


                    }
                    else {

                        $('#panZReportFowst').popup('open');

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
                                                $('.lblZReportSelectedFowstGroup').text($(this).text());
                                                $('#panZReportFowst').popup('close');
                                                $('.btnZReportSelectFowst').removeClass('btnRequired').addClass('btnNotRequired');
                                                ViewZReport();
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

                function FlushDeviceData(vOrgID) {

                    clearInterval(mInterval);
                    SetPopUpMessage(3, "Flushing Data to Server");

                    if (flushedCounter < 3) {
                        var pDt;
                        var pDt2;
                        var pDt3;
                        var pSql = '';
                        var pSql2 = '';
                        var pSql3 = '';

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

                        if (pDt.Rows.length > 0 || pDt2.Rows.length > 0 || pDt3.Rows.length > 0) {  //add to checking
                            flushedCounter++;
                            //flushed dire na side
                            window.SalesPaymentsExport_offline().Export({
                                //////////                                OnUploaded: function () {
                                //////////                                    FollowUp();
                                //////////                                }
                                OnDone: function (vData) {
                                    if (vData.Result == 'ok') {
                                        FlushDeviceData(sessionStorage.OrgID);
                                    }
                                }
                            }); // END window.SalesPaymentsExport_offline().Export   

                            //////////                            function FollowUp() {
                            //////////                                var pResult; // boolean

                            //////////                                pResult = window.SalesPaymentsExport_offline().FollowUp({
                            //////////                                    OnDone: function (vData) {
                            //////////                                        if (vData.Result == 'ok') {
                            //////////                                            FlushDeviceData(sessionStorage.OrgID);
                            //////////                                        }
                            //////////                                    },
                            //////////                                    OnProgress: function (vData) {
                            //////////                                        console.log(vData.Message); // vData.Message = string
                            //////////                                    }
                            //////////                                });
                            //////////                                if (pResult) {
                            //////////                                    //if pResult = false, error ang flushing?
                            //////////                                    // expect OnDone (and optionally OnProgress) to be called
                            //////////                                } else {
                            //////////                                    // OnDone and OnProgress will not be called
                            //////////                                }
                            //////////                            } // END FollowUp()

                        }
                        else {
                            $("#dvpopup").popup("close");
                            PrintSalesZReport(true);
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
                    var pData = { FilterFowst: 1 };

                    IsFowstInSMMall = false;

                    AjaxSend({
                        url: 'Browser/LibraryFowst',
                        data: pData,
                        success: function (vFowstData) {

                            if (vFowstData.Results.Rows.length == 1) {
                                sessionStorage.FowstID = vFowstData.Results.Rows[0].ID;

                                if (vFowstData.Results.Rows[0].IsFOWSTInsideAMall == 1 && vFowstData.Results.Rows[0].FOWSTMallName == 'SM Mall') {
                                    IsFowstInSMMall = true;
                                }

                            } else {
                                BrowseAddresses();
                            }
                        }
                    });
                };

                function BrowseAddresses() {

                    IsFowstInSMMall = false;

                    Browser.LibraryFowst.InputData.OrganizationID = sessionStorage.OrgID;
                    Browser.LibraryFowst.InputData.FilterFowst = 1;
                    Browser.LibraryFowst.Click = function (e) {
                        sessionStorage.FowstID = e.ID;

                        if (e.IsFOWSTInsideAMall == 1 && e.FOWSTMallName == 'SM Mall') {
                            IsFowstInSMMall = true;
                        }

                    };
                    Browser.LibraryFowst.Show();

                };

                function PrintSalesZReport(vIsView) {                    
                    var strTemp = '';
                    var strPrint = '';
                    var strDate = '';
                    var pData = {};

                    if ($("#selZReportRange").val() == "") {
                        MessageBox("Please select report range.",
                                "Report Range",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                            );
                        return;
                    }

                    if ($("#selZReportRange").val() == "By Date") {
                        if ($.trim($("#selZReportByDateMonth").val()) == "" ||
                        $.trim($("#selZReportByDateDay").val()) == "" ||
                        $.trim($("#selZReportByDateYear").val()) == "") {

                            MessageBox("Please enter the date of Z-Read.",
                                    "Date",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                            );
                            return;
                        }

                    }

                    strDate = $("#selZReportByDateMonth").val() + "/" + $("#selZReportByDateDay").val() + "/" + $("#selZReportByDateYear").val();

                    if (CBool(sessionStorage.PrintingOfflineData)) {
                        vDataPrint = SalesPayment_offline().getOrgFOWSTPrinterIP();

                        if (!vIsView) {
                            if (vDataPrint.Rows.length > 0) {
                                //strPrinterIP = vDataPrint.OrganizationFOWSTData.Rows[0].PrinterIPAddress;
                                strPrinterIP = vDataPrint.Rows[0].PrinterIPAddress;
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
                                return;
                            }
                        }

                        pData.RangeType = rangeType || 'Today'; //$.trim($("#selZReportRange").val());
                        pData.ZDate = dateFrom || ''; //strDate

                        if (IsMobile) {
                            var vDataPrinterType = SalesPayment_offline().getOrgFOWSTPrinterType();

                            if (vDataPrinterType.Rows.length > 0) {
                                if (vDataPrinterType.Rows[0].PrinterType == 'Epson TM-U220B Printer') {
                                    //PrintPOSReceiptDotMatrix(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg);
                                } else if (vDataPrinterType.Rows[0].PrinterType == '80MM Mini Thermal Printer') {

                                } else if (vDataPrinterType.Rows[0].PrinterType == 'Windows Print Server') {
                                    isPrintInWPS = true;
                                } else {
                                    isPrintInWPS = false;
                                }
                            } else {
                                isPrintInWPS = false;
                            }
                        } else {
                            isPrintInWPS = false;
                        }

                        vDataHeader = SalesPayment_offline().getReceiptHeaderInfo('');
                        //Get Invoice Header

                        //Display Header
                        DisplayHeader(vDataHeader, vIsView);

                        var pSql = 'Select * from TransZReadCache where FOWSTID = ' + ReadyString(sessionStorage.SelectedFOWST);
                        var pDTZRead = Sql.Execute(pSql);

                        var mData = { ReportPOS: pDTZRead }

                        DisplaySalesZReportSummary(vDataHeader, mData, vIsView);

                    } else {

                        AjaxSend({
                            url: "Organization/getOrgFOWSTPrinterIP",
                            success: function (vDataPrint) {

                                if (!vIsView) {
                                    if (vDataPrint.OrganizationFOWSTData.Rows.length > 0) {
                                        if (vDataPrint.OrganizationFOWSTData.Rows[0].PrinterType == 'Epson TM-U220B Printer') {
                                            //PrintPOSReceiptDotMatrix(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg);
                                        } else if (vDataPrint.OrganizationFOWSTData.Rows[0].PrinterType == '80MM Mini Thermal Printer') {

                                        } else if (vDataPrint.OrganizationFOWSTData.Rows[0].PrinterType == 'Windows Print Server') {
                                            isPrintInWPS = true;
                                        } else {
                                            isPrintInWPS = false;
                                        }
                                        strPrinterIP = vDataPrint.OrganizationFOWSTData.Rows[0].PrinterIPAddress;
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
                                        return;
                                    }
                                }

                                pData.RangeType = rangeType || 'Today'; //$.trim($("#selZReportRange").val());
                                pData.ZDate = dateFrom || ''; //strDate
                                
                                if (IsMobile) {
                                    pData.DeviceID = Web.DeviceID();
                                    var vDataPrinterType = SalesPayment_offline().getOrgFOWSTPrinterType();

                                    if (vDataPrinterType.Rows.length > 0) {
                                        if (vDataPrinterType.Rows[0].PrinterType == 'Epson TM-U220B Printer') {
                                            //PrintPOSReceiptDotMatrix(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg);
                                        } else if (vDataPrinterType.Rows[0].PrinterType == '80MM Mini Thermal Printer') {

                                        } else if (vDataPrinterType.Rows[0].PrinterType == 'Windows Print Server') {
                                            isPrintInWPS = true;
                                        } else {
                                            isPrintInWPS = false;
                                        }
                                    } else {
                                        isPrintInWPS = false;
                                    }

                                    strPrinterIP = vDataPrinterType.Rows[0].PrinterIPAddress;
                                } else {
                                    isPrintInWPS = false;
                                }


                                //Get Header
                                AjaxSend({
                                    url: "ReportsPOS/getReportHeader",
                                    success: function (vDataHeader) {
                                        //Display Header
                                        DisplayHeader(vDataHeader.ReportPOS, vIsView);

                                        //Get X-Report Info
                                        AjaxSend({
                                            url: "ReportsPOS/getSalesZReportSummary",
                                            data: pData,
                                            success: function (vDataInfo) {
                                                //Display SalesZReportSummary
                                                DisplaySalesZReportSummary(vDataHeader, vDataInfo, vIsView);
                                            }
                                        });

                                    }
                                });


                            }
                        });
                    }

                }


                function DisplayHeader(vDataHeader, vIsView) {

                    if (vIsView) {

                        $("#lblTradeName").text($.trim((vDataHeader.Rows[0].OrgBusinessTradeName || '')));
                        $("#lblCorporateName").text($.trim((vDataHeader.Rows[0].SubscriberOrganizationName || '')));
                        $("#lblBranchName").text($.trim((vDataHeader.Rows[0].OrgBranchName || '')));
                        $("#lblAddress").text($.trim((vDataHeader.Rows[0].OrgCompleteAddress || '')));
                        $("#lblTIN").text('TIN ' + $.trim((vDataHeader.Rows[0].OrgTIN || '')));
                        $("#lblTelNo").text('Tel. No.' + $.trim((vDataHeader.Rows[0].OrgTelNo || '')));

                    } else {

                        if (isPrintInWPS) {

                            WindowsPrinter($.trim((vDataHeader.Rows[0].OrgBusinessTradeName || '')), [0, 0, 0, 1, 0, 36]);

                            WindowsPrinter($.trim((vDataHeader.Rows[0].SubscriberOrganizationName || '')), [0, 0, 0, 1, 0, 36]);

                            WindowsPrinter($.trim((vDataHeader.Rows[0].OrgBranchName || '')), [1, 0, 0, 1, 0, 36]);

                            WindowsPrinter($.trim((vDataHeader.Rows[0].OrgCompleteAddress || '')), [1, 0, 0, 1, 0, 36]);

                            WindowsPrinter('TIN' + $.trim((vDataHeader.Rows[0].OrgTIN || '')), [1, 0, 0, 1, 0, 36]);

                            WindowsPrinter('Tel. No.' + $.trim((vDataHeader.Rows[0].OrgTelNo || '')), [1, 0, 0, 1, 0, 36]);

                            WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);

                        } else {
                            //if (vDataHeader.ReportPOS.Rows.length > 0) {
                            //New set of text to print
                            NewPrintBuilder();

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
                    }
                }

                function DisplaySalesZReportSummary(vDataHeader, vDataInfo, vIsView) {

                    if (!vIsView) {
                        if (isPrintInWPS) {
                            WindowsPrinter('Z-READ', [1, 0, 0, 1, 0, 36]);

                        } else {
                            strPrint = FormatTextToPrint(intFAFCTotalLenght, "Z-READ", intFAFCTotalLenght, '', 0, '', 0, 0);
                            ReadyTextToPrint(strPrint, 3, 2, 1, 1, 1, 1, 0, 0, 0);
                        }
                    }


                    if (vDataInfo.ReportPOS.Rows.length > 0) {

                        if (vIsView) {
                            $("#lblZDate").text('Date : ' + $.format.date(new Date(vDataInfo.ReportPOS.Rows[0].SalesDate), 'MM/dd/yyyy'));
                        } else {
                            if (isPrintInWPS) {
                                WindowsPrinter('Date : ' + $.format.date(new Date(vDataInfo.ReportPOS.Rows[0].SalesDate), 'MM/dd/yyyy'), [1, 0, 0, 1, 0, 36]);
                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                            } else {
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Date : ' + $.format.date(new Date(vDataInfo.ReportPOS.Rows[0].SalesDate), 'MM/dd/yyyy'), intFAFCTotalLenght, '', 0, '', 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                            }

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
                        var dblDailySale = 0;
                        var dblSalesVAT = 0;
                        var dblOldGT = 0;
                        var dblNewGT = 0;

                        var dblRegDiscount = 0;
                        var dblEmpDiscount = 0;
                        var dblSCD = 0;
                        var dblPWD = 0;
                        var dblVIP = 0;
                        var dblGPC = 0;
                        var dblDiscount1 = 0;

                        var dblServiceCharge = 0;
                        var dblLocalTax = 0;
                        var zReadCount = 0;

                        for (var p = 0; p <= vDataInfo.ReportPOS.Rows.length - 1; p++) {

                            dblTotalSales = dblTotalSales + parseFloat(vDataInfo.ReportPOS.Rows[p].CurGrossSales || 0);
                            dblTotalSalesDiscount = dblTotalSalesDiscount + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalDiscount || 0);
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
                            dblEPlus = dblEPlus + parseFloat(vDataInfo.ReportPOS.Rows[p].EPlusAmount || 0);
                            dblVoucher = dblVoucher + parseFloat(vDataInfo.ReportPOS.Rows[p].VoucherAmount || 0);
                            dblServiceCharge = dblServiceCharge + parseFloat(vDataInfo.ReportPOS.Rows[p].ServiceCharge || 0);
                            dblLocalTax = dblLocalTax + parseFloat(vDataInfo.ReportPOS.Rows[p].LocalTax || 0);


                            dblVAT = dblVAT + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVATSales || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVATSalesSalesReturn || 0);
                            dblVATAmount = dblVATAmount + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVAT || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVATAmountSalesReturn || 0);
                            dblNonVAT = dblNonVAT + parseFloat(vDataInfo.ReportPOS.Rows[p].CurNoTAXSales || 0);
                            dblVATExempt = dblVATExempt + parseFloat(vDataInfo.ReportPOS.Rows[p].CurVATExemptSales || 0);
                            dblZeroRatedVAT = dblZeroRatedVAT + 0;

                            dblRegDiscount = dblRegDiscount + parseFloat(vDataInfo.ReportPOS.Rows[p].RegularDiscount || 0);
                            dblEmpDiscount = dblEmpDiscount + parseFloat(vDataInfo.ReportPOS.Rows[p].EmployeeDiscount || 0);
                            dblSCD = dblSCD + parseFloat(vDataInfo.ReportPOS.Rows[p].SeniorCitizenDiscount || 0);
                            dblPWD = dblPWD + parseFloat(vDataInfo.ReportPOS.Rows[p].PWDDiscount || 0);
                            dblVIP = dblVIP + parseFloat(vDataInfo.ReportPOS.Rows[p].VIPDiscount || 0);
                            dblGPC = dblGPC + parseFloat(vDataInfo.ReportPOS.Rows[p].GPCDiscount || 0);
                            dblDiscount1 = dblDiscount1 + parseFloat(vDataInfo.ReportPOS.Rows[p].Discount1 || 0);



                            intTransCount = intTransCount + parseInt(vDataInfo.ReportPOS.Rows[p].CurTransCountInv || 0) + parseInt(vDataInfo.ReportPOS.Rows[p].CurTransCountMemoReturnExchange || 0) + parseInt(vDataInfo.ReportPOS.Rows[p].CurTransCountMemoRefund || 0);
                            intSalesTransCount = intSalesTransCount + parseInt(vDataInfo.ReportPOS.Rows[p].CurTransCountInv || 0);
                            intSalesItemCount = intSalesItemCount + parseInt(vDataInfo.ReportPOS.Rows[p].CurTransInvItemCount || 0);
                            intSRVTransCount = intSRVTransCount + parseInt(vDataInfo.ReportPOS.Rows[p].CurTransCountMemoReturnExchange || 0);
                            intSRVItemCount = intSRVItemCount + parseInt(vDataInfo.ReportPOS.Rows[p].CurSalesReturnExchangeItemCount || 0);
                            intRefTransCount = intRefTransCount + parseInt(vDataInfo.ReportPOS.Rows[p].CurTransCountMemoRefund || 0);
                            intRefItemCount = intRefItemCount + parseInt(vDataInfo.ReportPOS.Rows[p].CurSalesRefundItemCount || 0);

                            dblDailySale = dblDailySale + parseFloat(vDataInfo.ReportPOS.Rows[p].CurGrossSales || 0) - (vDataInfo.ReportPOS.Rows[p].CurTotalDiscount || 0) - (vDataInfo.ReportPOS.Rows[p].CurSalesReturnRefund || 0) - (vDataInfo.ReportPOS.Rows[p].CurSalesReturnExchange || 0) - (vDataInfo.ReportPOS.Rows[p].CurServiceCharge || 0) - (vDataInfo.ReportPOS.Rows[p].CurTotalVAT || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].ChargeAmount || 0);
                            dblSalesVAT = dblSalesVAT + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVAT || 0);
                            dblOldGT = dblOldGT + parseFloat(vDataInfo.ReportPOS.Rows[p].BegBal || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].PrevGrossSales || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].PrevTotalDiscount || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].PrevSalesReturnRefund || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].PrevSalesReturnExchange || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].PrevServiceCharge || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].PrevChargeAmount || 0);

                            zReadCount += CInt2(vDataInfo.ReportPOS.Rows[p].ZCount);
                        };

                        dblNewGT = dblOldGT + dblDailySale + dblSalesVAT;
                        dblNetSales = dblTotalSales - dblTotalSalesDiscount - dblAdjRefund - dblAdjSRV + dblCharges;
                        dblTotalPayment = dblCash + dblCheck + dblDebitCard + dblCreditCard + dblSRV + dblOtherPaymentType;
                        dblTotalVAT = dblVAT + dblVATAmount + dblNonVAT + dblVATExempt + dblZeroRatedVAT;

                        if (vIsView) {
                            //Date Sales Summary
                            $("#lblGrossSales").text(AccountingFormat(dblTotalSales));
                            $("#lblSalesDiscount").text(AccountingFormat(dblTotalSalesDiscount));
                            $("#lblTotalSalesRefund").text(AccountingFormat(dblAdjRefund));
                            $("#lblTotalSRV").text(AccountingFormat(dblAdjSRV));
                            $("#lblNetOfAdjustment").text(AccountingFormat(parseFloat(dblAdjRefund) + parseFloat(dblAdjSRV)));
                            $("#lblNetSales").text(AccountingFormat(dblNetSales));
                            //End Date Sales Summary

                            //Sales Grouped by Mode of Payment----------------------------
                            $("#lblCash").text(AccountingFormat(dblCash));
                            $("#lblCheck").text(AccountingFormat(dblCheck));
                            $("#lblDebitCard").text(AccountingFormat(dblDebitCard));
                            $("#lblCreditCard").text(AccountingFormat(dblCreditCard));
                            $("#lblSRV").text(AccountingFormat(dblSRV));
                            $("#lblOtherPaymentType").text(AccountingFormat(dblOtherPaymentType));

                            $("#lblSMAC").text(AccountingFormat(dblSMAC));
                            $("#lblEPlus").text(AccountingFormat(dblEPlus));
                            $("#lblVoucher").text(AccountingFormat(dblVoucher));
                            $("#lblTotalInvPayment").text(AccountingFormat(dblTotalPayment));


                            if (dblCharges > 0)
                                $("#divCharges").show();
                            else
                                $("#divCharges").hide();

                            $("#lblCharges").text(AccountingFormat(dblCharges));

                            //End Sales Grouped by Mode of Payment----------------------------

                            //Sales Grouped by VAT TYPE----------------------------
                            var dblTotalVATSalesPercent = (dblVAT / dblTotalVAT) * 100;
                            var dblTotalVATAmountPercent = (dblVATAmount / dblTotalVAT) * 100;
                            var dblTotalNonVATPercent = (dblNonVAT / dblTotalVAT) * 100;
                            var dblTotalVATExemptPercent = (dblVATExempt / dblTotalVAT) * 100;
                            var dblTotalZeroRatedVATPercent = (dblZeroRatedVAT / dblTotalVAT) * 100;
                            var dblTotalVATPercent = dblTotalVATSalesPercent + dblTotalVATAmountPercent + dblTotalNonVATPercent + dblTotalVATExemptPercent + dblTotalZeroRatedVATPercent;

                            $("#lblVATableSales").text(AccountingFormat(dblVAT));
                            $("#lblVATableSalesPercent").text(AccountingFormat(dblTotalVATSalesPercent) + '%');
                            $("#lblVATAmount").text(AccountingFormat(dblVATAmount));
                            $("#lblVATAmountPercent").text(AccountingFormat(dblTotalVATAmountPercent) + '%');
                            $("#lblNonVATAmount").text(AccountingFormat(dblNonVAT));
                            $("#lblNonVATPercent").text(AccountingFormat(dblTotalNonVATPercent) + '%');
                            $("#lblVATExemptAmount").text(AccountingFormat(dblVATExempt));
                            $("#lblVATExemptPercent").text(AccountingFormat(dblTotalVATExemptPercent) + '%');
                            $("#lblZeroRatedVATAmount").text(AccountingFormat(dblZeroRatedVAT));
                            $("#lblZeroRatedVATPercent").text(AccountingFormat(dblTotalZeroRatedVATPercent) + '%');
                            $("#lblTotalVATType").text(AccountingFormat(dblTotalVAT));
                            $("#lblTotalVATTypePercent").text(AccountingFormat(dblTotalVATPercent) + '%');
                            //End Sales Grouped by VAT TYPE----------------------------

                            //Other Informatoin
                            $("#lblTotalTransCount").text(intTransCount + '');
                            $("#lblSalesTransCount").text(intSalesTransCount + '');
                            $("#lblSalesLineItemCount").text(intSalesItemCount + '');
                            $("#lblSRVTransCount").text(intSRVTransCount + '');
                            $("#lblSRVLineItemCount").text(intSRVItemCount + '');
                            $("#lblRefundTransCount").text(intRefTransCount + '');
                            $("#lblRefundLineItemCount").text(intRefItemCount + '');
                            $("#lblCustomerCount").text(intSalesTransCount + '');
                            $("#lblOldGrandTotal").text(AccountingFormat(dblOldGT));
                            $("#lblNewGrandTotal").text(AccountingFormat(dblNewGT));
                            $('#lblZReadPrintCount').text(zReadCount);
                            //END Other Informatoin

                            $("#divPOSSalesZReportPrint").show();


                            if (IsFowstInSMMall) {
                                $("#divPOSSalesZReportEOD").show();
                                $("#divOtherTypeDetailed").show();
                                $("#divOtherType").hide();
                            } else {
                                $("#divOtherTypeDetailed").hide();
                                $("#divOtherType").show();
                            }



                        } else {
                            if (isPrintInWPS) {
                                //Date Sales Summary

                                //WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                WindowsPrinter(" Gross Sales ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblTotalSales), [0, 10, 0, 0, 0, 36]);

                                //var strTotalSales = SetPrefixPostfix(AccountingFormat(dblTotalSales), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Gross Sales: ', 14, strTotalSales, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Sales Discount ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblTotalSalesDiscount), [0, 10, 0, 0, 0, 36]);
                                //WindowsPrinter(' Sales Discount: ' + (AccountingFormat(dblTotalSalesDiscount) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strTotalSalesDiscount = SetPrefixPostfix(AccountingFormat(dblTotalSalesDiscount), intFAFCTotalLenght - 19, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Sales Discount: ', 17, strTotalSalesDiscount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Service Charge ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblServiceCharge), [0, 10, 0, 0, 0, 36]);
                                //WindowsPrinter(' Service Charge: ' + (AccountingFormat(dblServiceCharge) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strServiceCharge = SetPrefixPostfix(AccountingFormat(dblServiceCharge), intFAFCTotalLenght - 19, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Service Charge: ', 17, strServiceCharge, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Local Tax ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblLocalTax), [0, 10, 0, 0, 0, 36]);
                                //WindowsPrinter(' Local Tax: ' + (AccountingFormat(dblLocalTax) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strLocalTax = SetPrefixPostfix(AccountingFormat(dblLocalTax), intFAFCTotalLenght - 19, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Local Tax: ', 17, strLocalTax, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                WindowsPrinter(' Summary of Adjustments ', [1, 0, 0, 1, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Summary of Adjustments', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Total Sales Refund for the  Period ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblAdjRefund), [0, 10, 0, 0, 0, 36]);
                                //WindowsPrinter(' Total Sales Refund for the  Period ' + (AccountingFormat(dblAdjRefund) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strAdjRefund = SetPrefixPostfix(AccountingFormat(dblAdjRefund), intFAFCTotalLenght - 29, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Sales Refund for the  Period', 27, strAdjRefund, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Total Sales Returns Subject to Merchadise Exchange           (SRV Issued) ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblAdjSRV), [0, 10, 0, 0, 0, 36]);
                                //WindowsPrinter(' Total Sales Returns Subject to Merchadise Exchange           (SRV Issued) ' + (AccountingFormat(dblAdjSRV) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strAdjSRV = SetPrefixPostfix(AccountingFormat(dblAdjSRV), intFAFCTotalLenght - 32, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Sales Returns Subject to Merchadise Exchange           (SRV Issued)', 30, strAdjSRV, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Net of Adjustments ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblAdjRefund), [0, 10, 0, 0, 0, 36]);
                                //WindowsPrinter(' Net of Adjustments: ' + (AccountingFormat(dblAdjRefund) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strNetSalesAdj = SetPrefixPostfix(AccountingFormat(parseFloat(dblAdjRefund) + parseFloat(dblAdjSRV)), intFAFCTotalLenght - 23, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Net of Adjustments: ', 21, strNetSalesAdj, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                //WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                WindowsPrinter(" Net Sales ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblNetSales), [0, 10, 0, 0, 0, 36]);
                                //WindowsPrinter(' Net Sales: ' + (AccountingFormat(dblNetSales) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strNetSales = SetPrefixPostfix(AccountingFormat(dblNetSales), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Net Sales: ', 12, '  ' + strNetSales, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //End Date Sales Summary

                                //Sales Grouped by Mode of Payment----------------------------
                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                WindowsPrinter(' Inventory of Payments ', [1, 0, 0, 1, 0, 36]);
                                //WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, "Inventory of Payments", intFAFCTotalLenght, '', 0, '', 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                WindowsPrinter(" Cash       ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblCash), [0, 10, 0, 0, 0, 36]);
                                //WindowsPrinter(' Cash       ' + (AccountingFormat(dblCash) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strTotalCash = SetPrefixPostfix(AccountingFormat(dblCash), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Cash       ', 12, ': ' + strTotalCash, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Check      ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblCheck), [0, 10, 0, 0, 0, 36]);
                                //WindowsPrinter(' Check      ' + (AccountingFormat(dblCheck) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strTotalCheck = SetPrefixPostfix(AccountingFormat(dblCheck), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Check      ', 12, ': ' + strTotalCheck, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Debit Card ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblDebitCard), [0, 10, 0, 0, 0, 36]);
                                //WindowsPrinter(' Debit Card ' + (AccountingFormat(dblDebitCard) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strTotalDebitCard = SetPrefixPostfix(AccountingFormat(dblDebitCard), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Debit Card ', 12, ': ' + strTotalDebitCard, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Credit Card", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblCreditCard), [0, 10, 0, 0, 0, 36]);
                                //WindowsPrinter(' Credit Card' + (AccountingFormat(dblCreditCard) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strTotalCreditCard = SetPrefixPostfix(AccountingFormat(dblCreditCard), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Credit Card', 12, ': ' + strTotalCreditCard, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" SRV        ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblSRV), [0, 10, 0, 0, 0, 36]);
                                //WindowsPrinter(' SRV        ' + (AccountingFormat(dblSRV) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strTotalSRV = SetPrefixPostfix(AccountingFormat(dblSRV), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SRV        ', 12, ': ' + strTotalSRV, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                if (IsFowstInSMMall) {
                                    WindowsPrinter(" SMAC        ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblSMAC), [0, 10, 0, 0, 0, 36]);
                                    //WindowsPrinter(' SMAC        ' + (AccountingFormat(dblSMAC) || ''), [1, 0, 0, 1, 0, 36]);
                                    //var strTotalSMAC = SetPrefixPostfix(AccountingFormat(dblSMAC), intFAFCTotalLenght - 16, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SMAC        ', 12, ': ' + strTotalSMAC, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                    WindowsPrinter(" E-Plus      ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblEPlus), [0, 10, 0, 0, 0, 36]);
                                    //WindowsPrinter(' E-Plus      ' + (AccountingFormat(dblEPlus) || ''), [1, 0, 0, 1, 0, 36]);
                                    //var strTotalEPlus = SetPrefixPostfix(AccountingFormat(dblEPlus), intFAFCTotalLenght - 16, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' E-Plus      ', 12, ': ' + strTotalEPlus, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                    WindowsPrinter(" Voucher     ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblVoucher), [0, 10, 0, 0, 0, 36]);
                                    //WindowsPrinter(' Voucher     ' + (AccountingFormat(dblVoucher) || ''), [1, 0, 0, 1, 0, 36]);
                                    //var strTotalVoucher = SetPrefixPostfix(AccountingFormat(dblVoucher), intFAFCTotalLenght - 16, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Voucher     ', 12, ': ' + strTotalVoucher, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);


                                } else {
                                    WindowsPrinter(" Other Tender", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblOtherPaymentType), [0, 10, 0, 0, 0, 36]);
                                    //WindowsPrinter(' Other Tender' + (AccountingFormat(dblOtherPaymentType) || ''), [1, 0, 0, 1, 0, 36]);
                                    //var strTotalOther = SetPrefixPostfix(AccountingFormat(dblOtherPaymentType), intFAFCTotalLenght - 16, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Other Tender', 12, ': ' + strTotalOther, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                }


                                WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                WindowsPrinter("            ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblTotalPayment), [0, 10, 0, 0, 0, 36]);
                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                //WindowsPrinter('            ' + (AccountingFormat(dblTotalPayment) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strTotalClassPayment = SetPrefixPostfix(AccountingFormat(dblTotalPayment), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '            ', 12, '  ' + strTotalClassPayment, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);


                                //End Sales Grouped by Mode of Payment----------------------------

                                //Sales Grouped by VAT TYPE----------------------------
                                //WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                WindowsPrinter('Sales Grouped by VAT Type', [1, 0, 0, 1, 0, 36]);
                                //WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
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

                                WindowsPrinter("VAT-able Sales", [1, 16, 0, 0, 0, 36], AccountingFormat(dblVAT), [0, 10, 0, 0, 0, 36], AccountingFormat(dblTotalVATSalesPercent) + '%', [0, 10, 0, 0, 1, 36]);
                                //strPrint = ' ';
                                //var strTotalVATSales = SetPrefixPostfix(AccountingFormat(dblVAT), 14, ' ', 0);
                                //var strTotalVATSalesPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATSalesPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-able Sales', 14, strTotalVATSales, 30, strTotalVATSalesPercent, intFAFCTotalLenght, 1);
                                //strPrint = strPrint + strTemp;

                                WindowsPrinter("VAT Amount", [1, 16, 0, 0, 0, 36], AccountingFormat(dblVATAmount), [0, 10, 0, 0, 0, 36], AccountingFormat(dblTotalVATAmountPercent) + '%', [0, 10, 0, 0, 1, 36]);
                                //var strTotalVATAmount = SetPrefixPostfix(AccountingFormat(dblVATAmount), 14, ' ', 0);
                                //var strTotalVATAmountPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATAmountPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT Amount', 14, strTotalVATAmount, 30, strTotalVATAmountPercent, intFAFCTotalLenght, 1);
                                //strPrint = strPrint + strTemp;

                                WindowsPrinter("Non-VAT", [1, 16, 0, 0, 0, 36], AccountingFormat(dblNonVAT), [0, 10, 0, 0, 0, 36], AccountingFormat(dblTotalNonVATPercent) + '%', [0, 10, 0, 0, 1, 36]);
                                //var strTotalNonVAT = SetPrefixPostfix(AccountingFormat(dblNonVAT), 14, ' ', 0);
                                //var strTotalNonVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalNonVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Non-VAT', 14, strTotalNonVAT, 30, strTotalNonVATPercent, intFAFCTotalLenght, 1);
                                //strPrint = strPrint + strTemp;

                                WindowsPrinter("VAT-Exempt", [1, 16, 0, 0, 0, 36], AccountingFormat(dblVATExempt), [0, 10, 0, 0, 0, 36], AccountingFormat(dblTotalVATExemptPercent) + '%', [0, 10, 0, 0, 1, 36]);
                                //var strTotalVATExempt = SetPrefixPostfix(AccountingFormat(dblVATExempt), 14, ' ', 0);
                                //var strTotalVATExemptPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATExemptPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-Exempt', 14, strTotalVATExempt, 30, strTotalVATExemptPercent, intFAFCTotalLenght, 1);
                                //strPrint = strPrint + strTemp;

                                WindowsPrinter("Zero-Rated VAT", [1, 16, 0, 0, 0, 36], AccountingFormat(dblZeroRatedVAT), [0, 10, 0, 0, 0, 36], AccountingFormat(dblTotalZeroRatedVATPercent) + '%', [0, 10, 0, 0, 1, 36]);
                                //var strTotalZeroRatedVAT = SetPrefixPostfix(AccountingFormat(dblZeroRatedVAT), 14, ' ', 0);
                                //var strTotalZeroRatedVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalZeroRatedVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Zero-Rated VAT', 14, strTotalZeroRatedVAT, 30, strTotalZeroRatedVATPercent, intFAFCTotalLenght, 1);
                                //strPrint = strPrint + strTemp;

                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                WindowsPrinter("              ", [1, 16, 0, 0, 0, 36], AccountingFormat(dblTotalVAT), [0, 10, 0, 0, 0, 36], AccountingFormat(dblTotalVATPercent) + '%', [0, 10, 0, 0, 1, 36]);
                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                //var strTotalAllVAT = SetPrefixPostfix(AccountingFormat(dblTotalVAT), 14, ' ', 0);
                                //var strTotalAllVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '              ', 14, strTotalAllVAT, 30, strTotalAllVATPercent, intFAFCTotalLenght, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //End Sales Grouped by VAT TYPE----------------------------

                                //additional by janine june 17 2015 for sm coin. they require a breakdown of discounts in z report
                                if (IsFowstInSMMall) {
                                    ///
                                    //Discount Breakdown----------------------------
                                    //WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);

                                    WindowsPrinter('Breakdown of Discount', [1, 0, 0, 1, 0, 36]);
                                    //WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, "Breakdown of Discount", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                    //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                    WindowsPrinter(" Regular Discount ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblRegDiscount), [0, 10, 0, 0, 0, 36]);
                                    //WindowsPrinter(' Regular Discount ' + AccountingFormat(dblRegDiscount), [1, 0, 0, 1, 0, 36]);
                                    //var strTotalRegDiscount = SetPrefixPostfix(AccountingFormat(dblRegDiscount), intFAFCTotalLenght - 32, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Regular Discount', 30, strTotalRegDiscount, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    WindowsPrinter(" Employee Discount ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblEmpDiscount), [0, 10, 0, 0, 0, 36]);
                                    //WindowsPrinter(' Employee Discount ' + AccountingFormat(dblEmpDiscount), [1, 0, 0, 1, 0, 36]);
                                    //var strTotalEmpDiscount = SetPrefixPostfix(AccountingFormat(dblEmpDiscount), intFAFCTotalLenght - 32, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Employee Discount', 30, strTotalEmpDiscount, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    WindowsPrinter(" Senior Citizen ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblSCD), [0, 10, 0, 0, 0, 36]);
                                    //WindowsPrinter(' Senior Citizen ' + AccountingFormat(dblSCD), [1, 0, 0, 1, 0, 36]);
                                    //var strTotalSCD = SetPrefixPostfix(AccountingFormat(dblSCD), intFAFCTotalLenght - 32, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Senior Citizen', 30, strTotalSCD, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    WindowsPrinter(" PWD Discount ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblPWD), [0, 10, 0, 0, 0, 36]);
                                    //WindowsPrinter(' PWD Discount ' + AccountingFormat(dblPWD), [1, 0, 0, 1, 0, 36]);
                                    //var strTotalPWD = SetPrefixPostfix(AccountingFormat(dblPWD), intFAFCTotalLenght - 32, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' PWD Discount', 30, strTotalPWD, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    WindowsPrinter(" VIP Discount ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblVIP), [0, 10, 0, 0, 0, 36]);
                                    //WindowsPrinter(' VIP Discount ' + AccountingFormat(dblVIP), [1, 0, 0, 1, 0, 36]);
                                    //var strTotalVIP = SetPrefixPostfix(AccountingFormat(dblVIP), intFAFCTotalLenght - 32, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' VIP Discount', 30, strTotalVIP, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    WindowsPrinter(" GPC ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblGPC), [0, 10, 0, 0, 0, 36]);
                                    //WindowsPrinter(' GPC ' + AccountingFormat(dblGPC), [1, 0, 0, 1, 0, 36]);
                                    //var strTotalGPC = SetPrefixPostfix(AccountingFormat(dblGPC), intFAFCTotalLenght - 32, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' GPC ', 30, strTotalGPC, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    WindowsPrinter(" Discount 1 ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblDiscount1), [0, 10, 0, 0, 0, 36]);
                                    //WindowsPrinter(' Discount 1 ' + AccountingFormat(dblDiscount1), [1, 0, 0, 1, 0, 36]);
                                    //var strTotalDiscount1 = SetPrefixPostfix(AccountingFormat(dblDiscount1), intFAFCTotalLenght - 32, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Discount 1 ', 30, strTotalDiscount1, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                    WindowsPrinter(" ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblTotalSalesDiscount), [0, 10, 0, 0, 0, 36]);
                                    //WindowsPrinter(' ' + AccountingFormat(dblTotalSalesDiscount), [1, 0, 0, 1, 0, 36]);
                                    //var strTotalSalesDiscount2 = SetPrefixPostfix(AccountingFormat(dblTotalSalesDiscount), intFAFCTotalLenght - 32, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' ', 30, strTotalSalesDiscount2, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                    WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);


                                    ///
                                }

                                //Other Informatoin
                                //WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                WindowsPrinter('Other Information', [1, 0, 0, 1, 0, 36]);
                                //WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, "Other Information", intFAFCTotalLenght, '', 0, '', 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                WindowsPrinter("Total Trans. Count ", [1, 16, 0, 0, 0, 36], ': ' + intTransCount, [0, 10, 0, 0, 0, 36]);
                                //WindowsPrinter('Total Trans. Count ' + intTransCount, [1, 0, 0, 1, 0, 36]);
                                //var strTransCount = SetPrefixPostfix(intTransCount + '', intFAFCTotalLenght - 20, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Total Trans. Count', 18, strTransCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter("Sales Trans. Count ", [1, 16, 0, 0, 0, 36], ': ' + intSalesTransCount, [0, 10, 0, 0, 0, 36]);
                                //WindowsPrinter('Sales Trans. Count ' + intSalesTransCount, [1, 0, 0, 1, 0, 36]);
                                //var strSalesTransCount = SetPrefixPostfix(intSalesTransCount + '', intFAFCTotalLenght - 20, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Sales Trans. Count', 18, strSalesTransCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter("Sales Line Item Count ", [1, 16, 0, 0, 0, 36], ': ' + intSalesItemCount, [0, 10, 0, 0, 0, 36]);
                                //WindowsPrinter('Sales Line Item Count ' + intSalesItemCount, [1, 0, 0, 1, 0, 36]);
                                //var strSalesItemCount = SetPrefixPostfix(intSalesItemCount + '', intFAFCTotalLenght - 23, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Sales Line Item Count', 21, strSalesItemCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter("SRV Trans. Count ", [1, 16, 0, 0, 0, 36], ': ' + intSRVTransCount, [0, 10, 0, 0, 0, 36]);
                                //WindowsPrinter('SRV Trans. Count ' + intSRVTransCount, [1, 0, 0, 1, 0, 36]);
                                //var strSRVTransCount = SetPrefixPostfix(intSRVTransCount + '', intFAFCTotalLenght - 18, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'SRV Trans. Count', 16, strSRVTransCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter("SRV Line Item Count ", [1, 16, 0, 0, 0, 36], ': ' + intSRVItemCount, [0, 10, 0, 0, 0, 36]);
                                //WindowsPrinter('SRV Line Item Count ' + intSRVItemCount, [1, 0, 0, 1, 0, 36]);
                                //var strSRVItemCount = SetPrefixPostfix(intSRVItemCount + '', intFAFCTotalLenght - 21, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'SRV Line Item Count', 19, strSRVItemCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter("Refund Trans. Count ", [1, 16, 0, 0, 0, 36], ': ' + intRefTransCount, [0, 10, 0, 0, 0, 36]);
                                //WindowsPrinter('Refund Trans. Count ' + intRefTransCount, [1, 0, 0, 1, 0, 36]);
                                //var strRefTransCount = SetPrefixPostfix(intRefTransCount + '', intFAFCTotalLenght - 21, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Refund Trans. Count', 19, strRefTransCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter("Refund Line Item Count ", [1, 16, 0, 0, 0, 36], ': ' + intRefItemCount, [0, 10, 0, 0, 0, 36]);
                                //WindowsPrinter('Refund Line Item Count ' + intRefItemCount, [1, 0, 0, 1, 0, 36]);
                                //var strRefItemCount = SetPrefixPostfix(intRefItemCount + '', intFAFCTotalLenght - 24, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Refund Line Item Count', 22, strRefItemCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter("Customer Count ", [1, 16, 0, 0, 0, 36], ': ' + intSalesTransCount, [0, 10, 0, 0, 0, 36]);
                                //WindowsPrinter('Customer Count ' + intSalesTransCount, [1, 0, 0, 1, 0, 36]);
                                //var strCustomerCount = SetPrefixPostfix(intSalesTransCount + '', intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Customer Count', 14, strCustomerCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter("Old Grand Total ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblOldGT), [0, 10, 0, 0, 0, 36]);
                                //WindowsPrinter('Old Grand Total ' + AccountingFormat(dblOldGT), [1, 0, 0, 1, 0, 36]);
                                //var strOldGT = SetPrefixPostfix(AccountingFormat(dblOldGT), intFAFCTotalLenght - 17, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Old Grand Total', 15, strOldGT, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter("New Grand Total ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblNewGT), [0, 10, 0, 0, 0, 36]);
                                //WindowsPrinter('New Grand Total ' + AccountingFormat(dblNewGT), [1, 0, 0, 1, 0, 36]);
                                //var strNewGT = SetPrefixPostfix(AccountingFormat(dblNewGT), intFAFCTotalLenght - 17, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'New Grand Total', 15, strNewGT, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                //END Other Informatoin

                                WindowsPrinter("Z-Read Print Count", [1, 16, 0, 0, 0, 36], ': ' + zReadCount, [0, 10, 0, 0, 0, 36]);

                                //WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                //WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //strPrint = '';

                                WindowsPrinter(' Printed by ' + (vDataHeader.ReportPOS.Rows[0].PrintedBy || ''), [1, 0, 0, 1, 0, 36]);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Printed by ', 15, ': ' + (vDataHeader.ReportPOS.Rows[0].PrintedBy || ''), intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;

                                WindowsPrinter(' Date and Time ' + ($.format.date(new Date(vDataHeader.ReportPOS.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), [1, 0, 0, 1, 0, 36]);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataHeader.ReportPOS.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;

                                WindowsPrinter("address", [0, 0, 0, 0, 0, 0], strPrinterIP, [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);
                                WindowsPrinter("startPrint", [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);
                            } else {

                                //Date Sales Summary
                                var strTotalSales = SetPrefixPostfix(AccountingFormat(dblTotalSales), intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Gross Sales: ', 14, strTotalSales, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                var strTotalSalesDiscount = SetPrefixPostfix(AccountingFormat(dblTotalSalesDiscount), intFAFCTotalLenght - 19, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Sales Discount: ', 17, strTotalSalesDiscount, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                var strServiceCharge = SetPrefixPostfix(AccountingFormat(dblServiceCharge), intFAFCTotalLenght - 19, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Service Charge: ', 17, strServiceCharge, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                var strLocalTax = SetPrefixPostfix(AccountingFormat(dblLocalTax), intFAFCTotalLenght - 19, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Local Tax: ', 17, strLocalTax, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

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
                                //End Date Sales Summary

                                //Sales Grouped by Mode of Payment----------------------------
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
                                    var strTotalOther = SetPrefixPostfix(AccountingFormat(dblOtherPaymentType), intFAFCTotalLenght - 16, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Other Tender', 12, ': ' + strTotalOther, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                }


                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                var strTotalClassPayment = SetPrefixPostfix(AccountingFormat(dblTotalPayment), intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '            ', 12, '  ' + strTotalClassPayment, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);


                                //End Sales Grouped by Mode of Payment----------------------------

                                //Sales Grouped by VAT TYPE----------------------------
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

                                strPrint = ' ';
                                var strTotalVATSales = SetPrefixPostfix(AccountingFormat(dblVAT), 14, ' ', 0);
                                var strTotalVATSalesPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATSalesPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-able Sales', 14, strTotalVATSales, 30, strTotalVATSalesPercent, intFAFCTotalLenght, 1);
                                strPrint = strPrint + strTemp;

                                var strTotalVATAmount = SetPrefixPostfix(AccountingFormat(dblVATAmount), 14, ' ', 0);
                                var strTotalVATAmountPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATAmountPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT Amount', 14, strTotalVATAmount, 30, strTotalVATAmountPercent, intFAFCTotalLenght, 1);
                                strPrint = strPrint + strTemp;

                                var strTotalNonVAT = SetPrefixPostfix(AccountingFormat(dblNonVAT), 14, ' ', 0);
                                var strTotalNonVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalNonVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Non-VAT', 14, strTotalNonVAT, 30, strTotalNonVATPercent, intFAFCTotalLenght, 1);
                                strPrint = strPrint + strTemp;

                                var strTotalVATExempt = SetPrefixPostfix(AccountingFormat(dblVATExempt), 14, ' ', 0);
                                var strTotalVATExemptPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATExemptPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-Exempt', 14, strTotalVATExempt, 30, strTotalVATExemptPercent, intFAFCTotalLenght, 1);
                                strPrint = strPrint + strTemp;

                                var strTotalZeroRatedVAT = SetPrefixPostfix(AccountingFormat(dblZeroRatedVAT), 14, ' ', 0);
                                var strTotalZeroRatedVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalZeroRatedVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Zero-Rated VAT', 14, strTotalZeroRatedVAT, 30, strTotalZeroRatedVATPercent, intFAFCTotalLenght, 1);
                                strPrint = strPrint + strTemp;

                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                var strTotalAllVAT = SetPrefixPostfix(AccountingFormat(dblTotalVAT), 14, ' ', 0);
                                var strTotalAllVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '              ', 14, strTotalAllVAT, 30, strTotalAllVATPercent, intFAFCTotalLenght, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //End Sales Grouped by VAT TYPE----------------------------

                                //additional by janine june 17 2015 for sm coin. they require a breakdown of discounts in z report
                                if (IsFowstInSMMall) {
                                    ///
                                    //Discount Breakdown----------------------------
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, "Breakdown of Discount", intFAFCTotalLenght, '', 0, '', 0, 0);
                                    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);


                                    var strTotalRegDiscount = SetPrefixPostfix(AccountingFormat(dblRegDiscount), intFAFCTotalLenght - 32, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Regular Discount', 30, strTotalRegDiscount, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    var strTotalEmpDiscount = SetPrefixPostfix(AccountingFormat(dblEmpDiscount), intFAFCTotalLenght - 32, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Employee Discount', 30, strTotalEmpDiscount, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    var strTotalSCD = SetPrefixPostfix(AccountingFormat(dblSCD), intFAFCTotalLenght - 32, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Senior Citizen', 30, strTotalSCD, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    var strTotalPWD = SetPrefixPostfix(AccountingFormat(dblPWD), intFAFCTotalLenght - 32, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' PWD Discount', 30, strTotalPWD, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    var strTotalVIP = SetPrefixPostfix(AccountingFormat(dblVIP), intFAFCTotalLenght - 32, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' VIP Discount', 30, strTotalVIP, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    var strTotalGPC = SetPrefixPostfix(AccountingFormat(dblGPC), intFAFCTotalLenght - 32, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' GPC ', 30, strTotalGPC, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);


                                    var strTotalDiscount1 = SetPrefixPostfix(AccountingFormat(dblDiscount1), intFAFCTotalLenght - 32, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Discount 1 ', 30, strTotalDiscount1, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);




                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);


                                    var strTotalSalesDiscount2 = SetPrefixPostfix(AccountingFormat(dblTotalSalesDiscount), intFAFCTotalLenght - 32, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' ', 30, strTotalSalesDiscount2, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);



                                    ///
                                }

                                //Other Informatoin
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, "Other Information", intFAFCTotalLenght, '', 0, '', 0, 0);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                var strTransCount = SetPrefixPostfix(intTransCount + '', intFAFCTotalLenght - 20, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Total Trans. Count', 18, strTransCount, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strSalesTransCount = SetPrefixPostfix(intSalesTransCount + '', intFAFCTotalLenght - 20, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Sales Trans. Count', 18, strSalesTransCount, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strSalesItemCount = SetPrefixPostfix(intSalesItemCount + '', intFAFCTotalLenght - 23, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Sales Line Item Count', 21, strSalesItemCount, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strSRVTransCount = SetPrefixPostfix(intSRVTransCount + '', intFAFCTotalLenght - 18, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'SRV Trans. Count', 16, strSRVTransCount, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strSRVItemCount = SetPrefixPostfix(intSRVItemCount + '', intFAFCTotalLenght - 21, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'SRV Line Item Count', 19, strSRVItemCount, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strRefTransCount = SetPrefixPostfix(intRefTransCount + '', intFAFCTotalLenght - 21, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Refund Trans. Count', 19, strRefTransCount, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strRefItemCount = SetPrefixPostfix(intRefItemCount + '', intFAFCTotalLenght - 24, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Refund Line Item Count', 22, strRefItemCount, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strCustomerCount = SetPrefixPostfix(intSalesTransCount + '', intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Customer Count', 14, strCustomerCount, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strOldGT = SetPrefixPostfix(AccountingFormat(dblOldGT), intFAFCTotalLenght - 17, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Old Grand Total', 15, strOldGT, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strNewGT = SetPrefixPostfix(AccountingFormat(dblNewGT), intFAFCTotalLenght - 17, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'New Grand Total', 15, strNewGT, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strzReadCount = SetPrefixPostfix('Z-' + zReadCount + '', intFAFCTotalLenght - 21, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Z-Read Print Count', 19, strzReadCount, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                //END Other Informatoin

                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                strPrint = '';
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Printed by ', 15, ': ' + (vDataHeader.ReportPOS.Rows[0].PrintedBy || ''), intFAFCTotalLenght, '', 0, 1);
                                strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataHeader.ReportPOS.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                strPrint = strPrint + strTemp;

                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);

                                PrintText(strPrinterIP);
                                ClearCommandBuffer();
                            }

                            MessageBox("Would you like to print Z-READ for every MIN?",
                                    "Z-READ By MIN",
                                    [
                                        {
                                            Text: 'Yes',
                                            Click: function () {

                                                $.mobile.changePage(window.pageZReportPage);
                                                //Get Z-Report BY MIN
                                                AjaxSend({
                                                    url: "ReportsPOS/getSalesZReportSummary",
                                                    data: pData,
                                                    success: function (vData) {

                                                        PrintSalesZReportByMIN(vDataHeader, vData, 0);
                                                    }
                                                });
                                            }
                                        },
                                        {
                                            Text: 'No',
                                            Click: function () {
                                                CloseForSMCOIN();
                                            }
                                        }
                                    ]
                            );

                        } //if (vIsView) {

                    } else {
                        if (!vIsView) {
                            ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);
                            PrintText(strPrinterIP);
                            ClearCommandBuffer();
                        }
                    }

                    if (vIsView) {
                        
                        $("#divPOSSalesZReportPrint").show();
                        $("#divZReportView").show();
                    }
                }

                //COLS1-TEXT1/COLUMN 1
                //ARGS1-ALIGNMENT1 ETC LEFT RIGHT CENTER
                //COLS2-TEXT1/COLUMN 1
                //ARGS2-ALIGNMENT2 ETC LEFT RIGHT CENTER


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
                        Android.WindowsPrinting(
												{
												    "data": [
                                                              { "startPrint": "startPrint" }
												    ]
												}
						);
                    } else if (cols1 == "address") {
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

                function PrintDesktopSalesZReport() {
                    var strTemp = '';
                    var strPrint = '';
                    var strDate = '';
                    var pData = {};

                    if ($("#selZReportRange").val() == "") {
                        MessageBox("Please select report range.",
                                "Report Range",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                            );
                        return;
                    }

                    if ($("#selZReportRange").val() == "By Date") {
                        if ($.trim($("#selZReportByDateMonth").val()) == "" ||
                        $.trim($("#selZReportByDateDay").val()) == "" ||
                        $.trim($("#selZReportByDateYear").val()) == "") {

                            MessageBox("Please enter the date of Z-READ.",
                                    "Date",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                            );
                            return;
                        }

                    }

                    strDate = $("#selZReportByDateMonth").val() + "/" + $("#selZReportByDateDay").val() + "/" + $("#selZReportByDateYear").val();

                    var japplet = document.jzebra;
                    var blnIsPrinterFound = false;

                    if (japplet != null) {
                        while (!blnIsPrinterFound) {

                            if (FindDesktopPrinter("")) {
                                blnIsPrinterFound = true;

                                pData.RangeType = $.trim($("#selZReportRange").val());
                                pData.ZDate = strDate;

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

                                            //Get Z-Report Summary
                                            AjaxSend({
                                                url: "ReportsPOS/getSalesZReportSummary",
                                                data: pData,
                                                success: function (vDataInfo) {

                                                    if (vDataInfo.ReportPOS.Rows.length > 0) {

                                                        strPrint = SetTextAlign("Z-READ", 1, intDesktopPrintLenght);
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = SetTextAlign('Date : ' + $.format.date(new Date(vDataInfo.ReportPOS.Rows[0].SalesDate), 'MM/dd/yyyy'), 1, intDesktopPrintLenght);
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
                                                        var dblDailySale = 0;
                                                        var dblSalesVAT = 0;
                                                        var dblOldGT = 0;
                                                        var dblNewGT = 0;

                                                        for (var p = 0; p <= vDataInfo.ReportPOS.Rows.length - 1; p++) {
                                                            dblTotalSales = dblTotalSales + parseFloat(vDataInfo.ReportPOS.Rows[p].CurGrossSales || 0);
                                                            dblTotalSalesDiscount = dblTotalSalesDiscount + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalDiscount || 0);
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
                                                            dblEPlus = dblEPlus + parseFloat(vDataInfo.ReportPOS.Rows[p].EPlusAmount || 0);
                                                            dblVoucher = dblVoucher + parseFloat(vDataInfo.ReportPOS.Rows[p].VoucherAmount || 0);

                                                            dblVAT = dblVAT + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVATSales || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVATSalesSalesReturn || 0);
                                                            dblVATAmount = dblVATAmount + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVAT || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVATAmountSalesReturn || 0);
                                                            dblNonVAT = dblNonVAT + parseFloat(vDataInfo.ReportPOS.Rows[p].CurNoTAXSales || 0);
                                                            dblVATExempt = dblVATExempt + parseFloat(vDataInfo.ReportPOS.Rows[p].CurVATExemptSales || 0);
                                                            dblZeroRatedVAT = dblZeroRatedVAT + 0;

                                                            intTransCount = intTransCount + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTransCountInv || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTransCountMemoReturnExchange || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTransCountMemoRefund || 0);
                                                            intSalesTransCount = intSalesTransCount + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTransCountInv || 0);
                                                            intSalesItemCount = intSalesItemCount + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTransInvItemCount || 0);
                                                            intSRVTransCount = intSRVTransCount + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTransCountMemoReturnExchange || 0);
                                                            intSRVItemCount = intSRVItemCount + parseFloat(vDataInfo.ReportPOS.Rows[p].CurSalesReturnExchangeItemCount || 0);
                                                            intRefTransCount = intRefTransCount + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTransCountMemoRefund || 0);
                                                            intRefItemCount = intRefItemCount + parseFloat(vDataInfo.ReportPOS.Rows[p].CurSalesRefundItemCount || 0);

                                                            dblDailySale = dblDailySale + parseFloat(vDataInfo.ReportPOS.Rows[p].CurGrossSales || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalDiscount || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].CurSalesReturnRefund || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].CurSalesReturnExchange || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].CurServiceCharge || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVAT || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].ChargeAmount || 0);
                                                            dblSalesVAT = dblSalesVAT + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVAT || 0);
                                                            dblOldGT = dblOldGT + parseFloat(vDataInfo.ReportPOS.Rows[p].BegBal || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].PrevGrossSales || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].PrevTotalDiscount || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].PrevSalesReturnRefund || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].PrevSalesReturnExchange || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].PrevServiceCharge || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].PrevChargeAmount || 0);
                                                        };

                                                        dblNewGT = dblNewGT + dblOldGT + dblDailySale + dblSalesVAT;
                                                        dblNetSales = dblTotalSales - dblTotalSalesDiscount - dblAdjRefund - dblAdjSRV + dblCharges;
                                                        dblTotalPayment = dblCash + dblCheck + dblDebitCard + dblCreditCard + dblSRV + dblOtherPaymentType;

                                                        dblTotalVAT = dblVAT + dblVATAmount + dblNonVAT + dblVATExempt + dblZeroRatedVAT;

                                                        //Date Sales Summary
                                                        japplet.append("\n");
                                                        var strTotalSales = SetPrefixPostfix(AccountingFormat(dblTotalSales), intDesktopPrintLenght - 16, ' ', 0);
                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Gross Sales ', 0, 12), 12, ': ' + strTotalSales, intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        //Sales Discount
                                                        var strSalesDiscount = SetPrefixPostfix(AccountingFormat(dblTotalSalesDiscount), intDesktopPrintLenght - 22, ' ', 0);
                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Total Sales Discount', 20, strSalesDiscount, intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        //charges
                                                        var strCharges = SetPrefixPostfix(AccountingFormat(dblCharges), intDesktopPrintLenght - 22, ' ', 0);
                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Total Charges', 20, strCharges, intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");

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
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                                        japplet.append('\n' + strPrint + "\n");

                                                        var strNetSales = SetPrefixPostfix(AccountingFormat(dblNetSales), intDesktopPrintLenght - 11, ' ', 0);
                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Net Sales', 9, strNetSales, intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");
                                                        //End Date Sales Summary

                                                        //Sales Grouped by Mode of Payment----------------------------
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
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('E=Plus      ', 0, 12), 12, ': ' + strTotalEPlus, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            var strTotalVoucher = SetPrefixPostfix(AccountingFormat(dblVoucher), intDesktopPrintLenght - 16, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Voucher     ', 0, 12), 12, ': ' + strTotalVoucher, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");
                                                        }
                                                        else {
                                                            var strTotalOther = SetPrefixPostfix(AccountingFormat(dblOtherPaymentType), intDesktopPrintLenght - 16, ' ', 0);
                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Other Tender', 0, 12), 12, ': ' + strTotalOther, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                        }


                                                        strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                                        japplet.append(strPrint + "\n");


                                                        var strTotalClassPayment = SetPrefixPostfix(AccountingFormat(dblTotalPayment), intDesktopPrintLenght - 16, ' ', 0);
                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('            ', 0, 12), 12, ': ' + strTotalClassPayment, intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        //End Sales Grouped by Mode of Payment----------------------------

                                                        //Sales Grouped by VAT TYPE----------------------------
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
                                                        //End Sales Grouped by VAT TYPE----------------------------

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
                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Customer Count', 0, 14), 14, strCustomerCount + '', intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        var strOldGT = SetPrefixPostfix(AccountingFormat(dblOldGT), intDesktopPrintLenght - 17, ' ', 0);
                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Old Grand Total', 0, 15), 15, strOldGT + '', intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        var strNewGT = SetPrefixPostfix(AccountingFormat(dblNewGT), intDesktopPrintLenght - 17, ' ', 0);
                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('New Grand Total', 0, 15), 15, strNewGT + '', intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");
                                                        //END Other Informatoin

                                                        japplet.append("\n");
                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Printed by ', 0, 14), 14, ': ' + (vDataHeader.ReportPOS.Rows[0].PrintedBy || ''), intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Date and Time', 0, 14), 14, ': ' + ($.format.date(new Date(vDataHeader.ReportPOS.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        japplet.append("\n\n\n\n\n\n\n\n\n\n\n\n\n");
                                                        japplet.print();

                                                        MessageBox("Would you like to print Z-READ for every MIN?",
                                                                "Z-READ By MIN",
                                                                [
                                                                    { Text: 'Yes',
                                                                        Click: function () {

                                                                            $.mobile.changePage(window.pageZReportPage);
                                                                            //Get Z-Report BY MIN
                                                                            AjaxSend({
                                                                                url: "ReportsPOS/getSalesZReportSummary",
                                                                                data: pData,
                                                                                success: function (vData) {

                                                                                    PrintDesktopSalesZReportByMIN(vDataHeader, vData, 0);
                                                                                }
                                                                            });
                                                                        }
                                                                    },
                                                                    { Text: 'No',
                                                                        Click: function () {
                                                                            CloseForSMCOIN();
                                                                        }
                                                                    }
                                                                ]
                                                        );

                                                    } else {
                                                        japplet.append("\n\n\n\n\n\n\n\n\n\n\n\n\n");
                                                        japplet.print();
                                                    }
                                                }
                                            });

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

                function PrintSalesZReportByMIN(vDataHeader, vDataInfo, intIndex) {
                    var strTemp = '';
                    var strPrint = '';

                    var japplet = document.jzebra;
                    var blnIsPrinterFound = false;

                    if (intIndex >= vDataInfo.ReportPOS.Rows.length) {

                        CloseForSMCOIN();
                        return;
                    }

                    if (vDataHeader.ReportPOS.Rows.length > 0) {

                        NewPrintBuilder();

                        strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.ReportPOS.Rows[0].OrgBusinessTradeName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                        strPrint = '';
                        strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.ReportPOS.Rows[0].SubscriberOrganizationName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                        strPrint = strPrint + strTemp;

                        strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.ReportPOS.Rows[0].OrgBranchName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                        strPrint = strPrint + strTemp;

                        strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.ReportPOS.Rows[0].OrgCompleteAddress || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                        strPrint = strPrint + strTemp;

                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'TIN ' + $.trim((vDataHeader.ReportPOS.Rows[0].OrgTIN || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                        strPrint = strPrint + strTemp;

                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Tel. No.' + $.trim((vDataHeader.ReportPOS.Rows[0].OrgTelNo || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                        strPrint = strPrint + strTemp;

                        strPrint = strPrint + '\n';

                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                        if (vDataInfo.ReportPOS.Rows.length > 0) {

                            strPrint = FormatTextToPrint(intFAFCTotalLenght, "Z-READ", intFAFCTotalLenght, '', 0, '', 0, 0);
                            ReadyTextToPrint(strPrint, 3, 2, 1, 1, 1, 1, 0, 0, 0);

                            if (vDataInfo.ReportPOS.Rows.length > 0) {

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'MIN : ' + vDataInfo.ReportPOS.Rows[intIndex].FowstNo + '-' + vDataInfo.ReportPOS.Rows[intIndex].CurTermNum, intFAFCTotalLenght, '', 0, '', 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Date : ' + $.format.date(new Date(vDataInfo.ReportPOS.Rows[intIndex].SalesDate), 'MM/dd/yyyy'), intFAFCTotalLenght, '', 0, '', 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                var dblTotalSales = 0;
                                var dblTotalSalesDiscount = 0;
                                var dblAdjRefund = 0;
                                var dblAdjSRV = 0;
                                var dblNetSales = 0;

                                var dblCash = 0;
                                var dblCheck = 0;
                                var dblCreditCard = 0;
                                var dblDebitCard = 0;
                                var dblSRV = 0;
                                var dblOtherPaymentType = 0;
                                var dblCharges = 0;
                                var dblSMAC = 0;
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
                                var dblDailySale = 0;
                                var dblSalesVAT = 0;
                                var intInvStartNo = 0;
                                var intInvendNo = 0;
                                var dblOldGT = 0;
                                var dblNewGT = 0;

                                dblTotalSales = dblTotalSales + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurGrossSales || 0);
                                dblTotalSalesDiscount = dblTotalSalesDiscount + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTotalDiscount || 0);
                                dblAdjRefund = dblAdjRefund + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurSalesReturnRefund || 0);
                                dblAdjSRV = dblAdjSRV + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurSalesReturnExchange || 0);
                                dblNetSales = dblNetSales + dblTotalSales - dblTotalSalesDiscount - dblAdjRefund - dblAdjSRV + dblCharges;

                                dblCash = dblCash + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CashAmount || 0) - dblAdjRefund - dblAdjSRV;
                                dblCheck = dblCheck + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CheckPaymentAmount || 0);
                                dblDebitCard = dblDebitCard + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].DebitCardAmount || 0);
                                dblCreditCard = dblCreditCard + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CreditCardAmount || 0);
                                dblSRV = dblSRV + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].SRVAmount || 0);
                                dblOtherPaymentType = dblOtherPaymentType + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].OtherTypeAmount || 0);
                                dblCharges = dblCharges + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].ChargeAmount || 0);
                                dblSMAC = dblSMAC + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].SMACAmount || 0);
                                dblEPlus = dblEPlus + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].EPlusAmount || 0);
                                dblVoucher = dblVoucher + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].VoucherAmount || 0);
                                dblTotalPayment = dblTotalPayment + dblCash + dblCheck + dblDebitCard + dblCreditCard + dblSRV + dblOtherPaymentType;

                                dblVAT = dblVAT + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTotalVATSales || 0) - parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTotalVATSalesSalesReturn || 0);
                                dblVATAmount = dblVATAmount + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTotalVAT || 0) - parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTotalVATAmountSalesReturn || 0);
                                dblNonVAT = dblNonVAT + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurNoTAXSales || 0);
                                dblVATExempt = dblVATExempt + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurVATExemptSales || 0);
                                dblZeroRatedVAT = dblZeroRatedVAT + 0;
                                dblTotalVAT = dblTotalVAT + dblVAT + dblVATAmount + dblNonVAT + dblVATExempt + dblZeroRatedVAT

                                intTransCount = intTransCount + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTransCountInv || 0) + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTransCountMemoReturnExchange || 0) + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTransCountMemoRefund || 0);
                                intSalesTransCount = intSalesTransCount + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTransCountInv || 0);
                                intSalesItemCount = intSalesItemCount + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTransInvItemCount || 0);
                                intSRVTransCount = intSRVTransCount + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTransCountMemoReturnExchange || 0);
                                intSRVItemCount = intSRVItemCount + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurSalesReturnExchangeItemCount || 0);
                                intRefTransCount = intRefTransCount + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTransCountMemoRefund || 0);
                                intRefItemCount = intRefItemCount + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurSalesRefundItemCount || 0);

                                dblDailySale = dblDailySale + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurGrossSales || 0) - parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTotalDiscount || 0) - parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurSalesReturnRefund || 0) - parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurSalesReturnExchange || 0) - parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurServiceCharge || 0) - parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTotalVAT || 0) + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].ChargeAmount || 0);
                                dblSalesVAT = dblSalesVAT + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTotalVAT || 0);

                                intInvStartNo = parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurBegInvoiceNo || 0);
                                intInvendNo = parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurEndInvoiceNo || 0);

                                dblOldGT = dblOldGT + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].BegBal || 0) + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].PrevGrossSales || 0) - parseFloat(vDataInfo.ReportPOS.Rows[intIndex].PrevTotalDiscount || 0) - parseFloat(vDataInfo.ReportPOS.Rows[intIndex].PrevSalesReturnRefund || 0) - parseFloat(vDataInfo.ReportPOS.Rows[intIndex].PrevSalesReturnExchange || 0) - parseFloat(vDataInfo.ReportPOS.Rows[intIndex].PrevServiceCharge || 0) + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].PrevChargeAmount || 0);
                                dblNewGT = dblNewGT + dblOldGT + dblDailySale + dblSalesVAT;

                                //Date Sales Summary
                                var strTotalSales = SetPrefixPostfix(AccountingFormat(dblTotalSales), intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Gross Sales: ', 14, strTotalSales, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                var strTotalSalesDiscount = SetPrefixPostfix(AccountingFormat(dblTotalSalesDiscount), intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Sales Discount: ', 23, strTotalSalesDiscount, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                var strTotalCharges = SetPrefixPostfix(AccountingFormat(dblCharges), intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Charges: ', 23, strTotalCharges, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

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
                                //End Date Sales Summary

                                //Sales Grouped by Mode of Payment----------------------------
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
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Debit Card  ', 12, ': ' + strTotalDebitCard, intFAFCTotalLenght, '', 0, 1);
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
                                    var strTotalOther = SetPrefixPostfix(AccountingFormat(dblOtherPaymentType), intFAFCTotalLenght - 16, ' ', 0);
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Other Tender', 12, ': ' + strTotalOther, intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                }


                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                var strTotalClassPayment = SetPrefixPostfix(AccountingFormat(dblTotalPayment), intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '            ', 12, '  ' + strTotalClassPayment, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);



                                //End Sales Grouped by Mode of Payment----------------------------

                                //Sales Grouped by VAT TYPE----------------------------
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

                                strPrint = ' ';
                                var strTotalVATSales = SetPrefixPostfix(AccountingFormat(dblVAT), 14, ' ', 0);
                                var strTotalVATSalesPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATSalesPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-able Sales', 14, strTotalVATSales, 30, strTotalVATSalesPercent, intFAFCTotalLenght, 1);
                                strPrint = strPrint + strTemp;

                                var strTotalVATAmount = SetPrefixPostfix(AccountingFormat(dblVATAmount), 14, ' ', 0);
                                var strTotalVATAmountPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATAmountPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT Amount', 14, strTotalVATAmount, 30, strTotalVATAmountPercent, intFAFCTotalLenght, 1);
                                strPrint = strPrint + strTemp;

                                var strTotalNonVAT = SetPrefixPostfix(AccountingFormat(dblNonVAT), 14, ' ', 0);
                                var strTotalNonVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalNonVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Non-VAT', 14, strTotalNonVAT, 30, strTotalNonVATPercent, intFAFCTotalLenght, 1);
                                strPrint = strPrint + strTemp;

                                var strTotalVATExempt = SetPrefixPostfix(AccountingFormat(dblVATExempt), 14, ' ', 0);
                                var strTotalVATExemptPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATExemptPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-Exempt', 14, strTotalVATExempt, 30, strTotalVATExemptPercent, intFAFCTotalLenght, 1);
                                strPrint = strPrint + strTemp;

                                var strTotalZeroRatedVAT = SetPrefixPostfix(AccountingFormat(dblZeroRatedVAT), 14, ' ', 0);
                                var strTotalZeroRatedVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalZeroRatedVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Zero-Rated VAT', 14, strTotalZeroRatedVAT, 30, strTotalZeroRatedVATPercent, intFAFCTotalLenght, 1);
                                strPrint = strPrint + strTemp;

                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                var strTotalAllVAT = SetPrefixPostfix(AccountingFormat(dblTotalVAT), 14, ' ', 0);
                                var strTotalAllVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '              ', 14, strTotalAllVAT, 30, strTotalAllVATPercent, intFAFCTotalLenght, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //End Sales Grouped by VAT TYPE----------------------------

                                //Other Informatoin
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, "Other Information", intFAFCTotalLenght, '', 0, '', 0, 0);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                var strTransCount = SetPrefixPostfix(intTransCount + '', intFAFCTotalLenght - 20, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Total Trans. Count', 18, strTransCount, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strSalesTransCount = SetPrefixPostfix(intSalesTransCount + '', intFAFCTotalLenght - 20, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Sales Trans. Count', 18, strSalesTransCount, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strSalesItemCount = SetPrefixPostfix(intSalesItemCount + '', intFAFCTotalLenght - 23, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Sales Line Item Count', 21, strSalesItemCount, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strSRVTransCount = SetPrefixPostfix(intSRVTransCount + '', intFAFCTotalLenght - 18, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'SRV Trans. Count', 16, strSRVTransCount, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strSRVItemCount = SetPrefixPostfix(intSRVItemCount + '', intFAFCTotalLenght - 21, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'SRV Line Item Count', 19, strSRVItemCount, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strRefTransCount = SetPrefixPostfix(intRefTransCount + '', intFAFCTotalLenght - 21, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Refund Trans. Count', 19, strRefTransCount, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strRefItemCount = SetPrefixPostfix(intRefItemCount + '', intFAFCTotalLenght - 24, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Refund Line Item Count', 22, strRefItemCount, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strCustomerCount = SetPrefixPostfix(intSalesTransCount + '', intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Customer Count', 14, strCustomerCount, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strInvStart = SetPrefixPostfix(intInvStartNo + '', intFAFCTotalLenght - 16, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Start Inv. No.', 14, strInvStart, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strInvEnd = SetPrefixPostfix(intInvendNo + '', intFAFCTotalLenght - 14, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'End Inv. No.', 12, strInvEnd, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strOldGT = SetPrefixPostfix(AccountingFormat(dblOldGT), intFAFCTotalLenght - 17, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Old Grand Total', 15, strOldGT, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strNewGT = SetPrefixPostfix(AccountingFormat(dblNewGT), intFAFCTotalLenght - 17, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'New Grand Total', 15, strNewGT, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                //END Other Informatoin

                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                strPrint = '';
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Printed by ', 15, ': ' + (vDataHeader.ReportPOS.Rows[0].PrintedBy || ''), intFAFCTotalLenght, '', 0, 1);
                                strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataHeader.ReportPOS.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                strPrint = strPrint + strTemp;

                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);

                                PrintText(strPrinterIP);
                                ClearCommandBuffer();

                                PrintSalesZReportByMIN(vDataHeader, vDataInfo, intIndex + 1);
                            }

                        } else {
                            ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);
                            PrintText(strPrinterIP);
                            ClearCommandBuffer();
                        }

                    }
                }

                function PrintDesktopSalesZReportByMIN(vDataHeader, vDataInfo, intIndex) {
                    var strTemp = '';
                    var strPrint = '';

                    var japplet = document.jzebra;
                    var blnIsPrinterFound = false;

                    if (intIndex >= vDataInfo.ReportPOS.Rows.length) {

                        CloseForSMCOIN();
                        return;
                    }

                    if (japplet != null) {
                        while (!blnIsPrinterFound) {

                            if (FindDesktopPrinter("")) {
                                blnIsPrinterFound = true;

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


                                    if (vDataInfo.ReportPOS.Rows.length > 0) {

                                        strPrint = SetTextAlign("Z-READ", 1, intDesktopPrintLenght);
                                        japplet.append(strPrint + "\n");

                                        strPrint = SetTextAlign("MIN : " + vDataInfo.ReportPOS.Rows[intIndex].FowstNo + '-' + vDataInfo.ReportPOS.Rows[intIndex].CurTermNum, 1, intDesktopPrintLenght);
                                        japplet.append(strPrint + "\n");

                                        strPrint = SetTextAlign('Date : ' + $.format.date(new Date(vDataInfo.ReportPOS.Rows[intIndex].SalesDate), 'MM/dd/yyyy'), 1, intDesktopPrintLenght);
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
                                        var dblDailySale = 0;
                                        var dblSalesVAT = 0;
                                        var intInvStartNo = 0;
                                        var intInvendNo = 0;
                                        var dblOldGT = 0;
                                        var dblNewGT = 0;

                                        dblTotalSales = dblTotalSales + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurGrossSales || 0);
                                        dblTotalSalesDiscount = dblTotalSalesDiscount + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTotalDiscount || 0);
                                        dblAdjRefund = dblAdjRefund + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurSalesReturnRefund || 0);
                                        dblAdjSRV = dblAdjSRV + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurSalesReturnExchange || 0);
                                        dblNetSales = dblNetSales + dblTotalSales - dblTotalSalesDiscount - dblAdjRefund - dblAdjSRV + dblCharges;

                                        dblCash = dblCash + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CashAmount || 0) - dblAdjRefund - dblAdjSRV;
                                        dblCheck = dblCheck + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CheckPaymentAmount || 0);
                                        dblDebitCard = dblDebitCard + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].DebitCardAmount || 0);
                                        dblCreditCard = dblCreditCard + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CreditCardAmount || 0);
                                        dblSRV = dblSRV + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].SRVAmount || 0);
                                        dblOtherPaymentType = dblOtherPaymentType + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].OtherTypeAmount || 0);
                                        dblCharges = dblCharges + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].ChargeAmount || 0);
                                        dblSMAC = dblSMAC + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].SMACAmount || 0);
                                        dblEPlus = dblEPlus + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].EPlusAmount || 0);
                                        dblVoucher = dblVoucher + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].VoucherAmount || 0);
                                        dblTotalPayment = dblTotalPayment + dblCash + dblCheck + dblDebitCard + dblCreditCard + dblSRV + dblOtherPaymentType;

                                        dblVAT = dblVAT + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTotalVATSales || 0) - parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTotalVATSalesSalesReturn || 0);
                                        dblVATAmount = dblVATAmount + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTotalVAT || 0) - parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTotalVATAmountSalesReturn || 0);
                                        dblNonVAT = dblNonVAT + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurNoTAXSales || 0);
                                        dblVATExempt = dblVATExempt + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurVATExemptSales || 0);
                                        dblZeroRatedVAT = dblZeroRatedVAT + 0;
                                        dblTotalVAT = dblTotalVAT + dblVAT + dblVATAmount + dblNonVAT + dblVATExempt + dblZeroRatedVAT

                                        intTransCount = intTransCount + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTransCountInv || 0) + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTransCountMemoReturnExchange || 0) + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTransCountMemoRefund || 0);
                                        intSalesTransCount = intSalesTransCount + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTransCountInv || 0);
                                        intSalesItemCount = intSalesItemCount + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTransInvItemCount || 0);
                                        intSRVTransCount = intSRVTransCount + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTransCountMemoReturnExchange || 0);
                                        intSRVItemCount = intSRVItemCount + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurSalesReturnExchangeItemCount || 0);
                                        intRefTransCount = intRefTransCount + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTransCountMemoRefund || 0);
                                        intRefItemCount = intRefItemCount + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurSalesRefundItemCount || 0);

                                        dblDailySale = dblDailySale + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurGrossSales || 0) - parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTotalDiscount || 0) - parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurSalesReturnRefund || 0) - parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurSalesReturnExchange || 0) - parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurServiceCharge || 0) - parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTotalVAT || 0) + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].ChargeAmount || 0);
                                        dblSalesVAT = dblSalesVAT + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurTotalVAT || 0);

                                        intInvStartNo = parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurBegInvoiceNo || 0);
                                        intInvendNo = parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CurEndInvoiceNo || 0);

                                        dblOldGT = dblOldGT + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].BegBal || 0) + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].PrevGrossSales || 0) - parseFloat(vDataInfo.ReportPOS.Rows[intIndex].PrevTotalDiscount || 0) - parseFloat(vDataInfo.ReportPOS.Rows[intIndex].PrevSalesReturnRefund || 0) - parseFloat(vDataInfo.ReportPOS.Rows[intIndex].PrevSalesReturnExchange || 0) - parseFloat(vDataInfo.ReportPOS.Rows[intIndex].PrevServiceCharge || 0) + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].PrevChargeAmount || 0);
                                        dblNewGT = dblNewGT + dblOldGT + dblDailySale + dblSalesVAT;

                                        //Date Sales Summary
                                        japplet.append("\n");
                                        var strTotalSales = SetPrefixPostfix(AccountingFormat(dblTotalSales), intDesktopPrintLenght - 16, ' ', 0);
                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Gross Sales ', 0, 12), 12, ': ' + strTotalSales, intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        //Sales Discount
                                        var strSalesDiscount = SetPrefixPostfix(AccountingFormat(dblTotalSalesDiscount), intDesktopPrintLenght - 22, ' ', 0);
                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Total Sales Discount', 20, strSalesDiscount, intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        //charges
                                        var strCharges = SetPrefixPostfix(AccountingFormat(dblCharges), intDesktopPrintLenght - 22, ' ', 0);
                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Total Charges', 20, strCharges, intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");


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
                                        japplet.append(strPrint + "\n");

                                        strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                        japplet.append('\n' + strPrint + "\n");

                                        var strNetSales = SetPrefixPostfix(AccountingFormat(dblNetSales), intDesktopPrintLenght - 11, ' ', 0);
                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Net Sales', 9, strNetSales, intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");
                                        //End Date Sales Summary

                                        //Sales Grouped by Mode of Payment----------------------------
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

                                        var strTotalClassPayment = SetPrefixPostfix(AccountingFormat(dblTotalPayment), intDesktopPrintLenght - 16, ' ', 0);
                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('            ', 0, 12), 12, ': ' + strTotalClassPayment, intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");


                                        //End Sales Grouped by Mode of Payment----------------------------

                                        //Sales Grouped by VAT TYPE----------------------------
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
                                        var strTotalVATPercentAmount = SetPrefixPostfix(AccountingFormat(dblTotalVATAmountPercent) + '%', intDesktopPrintLenght - 32, ' ', 0);
                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('VAT Amount', 0, 14), 14, strTotalVATAmount, 30, strTotalVATPercentAmount, intDesktopPrintLenght);
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
                                        //End Sales Grouped by VAT TYPE----------------------------

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
                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Customer Count', 0, 14), 14, strCustomerCount + '', intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        var strInvStart = SetPrefixPostfix(intInvStartNo + '', intDesktopPrintLenght - 16, ' ', 0);
                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Start Inv. No.', 0, 14), 14, strInvStart + '', intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        var strInvEnd = SetPrefixPostfix(intInvendNo + '', intDesktopPrintLenght - 14, ' ', 0);
                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('End Inv. No.', 0, 12), 12, strInvEnd + '', intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        var strOldGT = SetPrefixPostfix(AccountingFormat(dblOldGT), intDesktopPrintLenght - 17, ' ', 0);
                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Old Grand Total', 0, 15), 15, strOldGT + '', intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        var strNewGT = SetPrefixPostfix(AccountingFormat(dblNewGT), intDesktopPrintLenght - 17, ' ', 0);
                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('New Grand Total', 0, 15), 15, strNewGT + '', intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");
                                        //END Other Informatoin

                                        japplet.append("\n");
                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Printed by ', 0, 14), 14, ': ' + (vDataHeader.ReportPOS.Rows[0].PrintedBy || ''), intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Date and Time', 0, 14), 14, ': ' + ($.format.date(new Date(vDataHeader.ReportPOS.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        japplet.append("\n\n\n\n\n\n\n\n\n\n\n\n\n");
                                        japplet.print();

                                        PrintDesktopSalesZReportByMIN(vDataHeader, vDataInfo, intIndex + 1);

                                    } else {
                                        japplet.append("\n\n\n\n\n\n\n\n\n\n\n\n\n");
                                        japplet.print();
                                    }

                                }
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

                function CloseForSMCOIN() {
                    //if Fowst is in SM Mall
                    if (IsFowstInSMMall) {
                        MessageBox("Do you want to conclude the transaction for today and generate the final SM COIN report?",
                                    "SM COIN Report",
                                [
                                    { Text: 'Yes',
                                        Click: function () {
                                            GenerateSMCOINReport();
                                            return;
                                        }
                                    },
                                    { Text: 'No',
                                        Click: function () {
                                            $.mobile.changePage(window.pageZReportPage);
                                            return;
                                        }
                                    }
                                ]
                        );
                    } else {
                        $.mobile.changePage(window.pageZReportPage);
                    }
                }

                function GenerateSMCOINReport() {
                    var strDate = '';
                    var pData = {};

                    strDate = $("#selZReportByDateMonth").val() + "/" + $("#selZReportByDateDay").val() + "/" + $("#selZReportByDateYear").val();

                    pData.RangeType = $('#selZReportRange').val();
                    pData.SalesDate = strDate;
                    pData.FlagClose = 1;

                    AjaxSend({
                        url: "MallLessorSM/GenerateReportFile",
                        data: pData,
                        success: function (vDataGen) {

                            if (vDataGen.Error) {
                                MessageBox(vDataGen.Error);
                                return;
                            };

                            if (vDataGen.GeneratedReport != "") {

                                if (!IsMobile) {
                                    window.location = GetPathFormat("/MallLessorSM/DownloadReportFile?ReportOrgID=" + sessionStorage.OrgID + "&ReportFowstID=" + sessionStorage.FowstID + "&ReportLink=" + vDataGen.GeneratedReport);
                                    $.mobile.changePage(window.pageZReportPage);
                                } else {
                                    var strFileName = vDataGen.GeneratedReport;
                                    Transfers.Download({
                                        Source: MergeUrlParameters({
                                            ReportOrgID: sessionStorage.OrgID
                                                    , ReportFowstID: sessionStorage.FowstID
                                                    , ReportLink: vDataGen.GeneratedReport
                                        }, '/MallLessorSM/DownloadReportFile'),
                                        Destination: "SMCOIN/" + sessionStorage.OrgID + "/" + sessionStorage.FowstID + "/" + vDataGen.GeneratedReport,
                                        Success: function (vData) {

                                            Transfers.ExtractZipFile({
                                                Source: vData.Destination,
                                                Destination: "SMCOIN",
                                                ExtractToFiles: true,
                                                Success: function (vDataExtractFiles) {
                                                    MessageBox("Downloaded file can be viewed in SMCOIN/" + sessionStorage.OrgID + "/" + sessionStorage.FowstID + "/" + strFileName,
                                                    "Download SM COIN Report",
                                                    [
                                                        {
                                                            Text: 'Ok',
                                                            Click: function () {
                                                                $.mobile.changePage(window.pageZReportPage);
                                                            }
                                                        }
                                                    ]
                                            );
                                                    return;

                                                }
                                            })


                                        },
                                        Error: function (e) {
                                            Android.Toast('Error while downloading file');
                                        }
                                    });
                                }

                            } else {

                                MessageBox('Report file generation failed.',
                                    "Generate Report",
                                    [
                                        { Text: 'Ok',
                                            Click: function () {
                                                $.mobile.changePage(window.pageZReportPage);
                                            }
                                        }
                                    ]
                                );

                            }


                        }
                    });

                }

                function UpdateZReportPrintCount() {
                    var pData = {};

                    if (IsMobile) {
                        pData.FOWSTID = sessionStorage.FowstID;
                        pData.DeviceID = Web.DeviceID();

                        AjaxSend({
                            url: "ReportsPOS/UpdateZReportPrintCount",
                            data: pData,
                            success: function (vDataGen) {

                            },
                            error: function (e) {
                            
                            }
                        });
                    }
                }

                //            function PrintDesktopSalesZReportByMINTemp() {
                //                var strTemp = '';
                //                var strPrint = '';

                //                var strDate = "01/01/2014"; //------------ Set This
                //                var strTradeName = 'Athletic Club Sports Co., Inc.'; //------------ Set This
                //                var strCorporateName = 'Athletic Club Sports Co., Inc.'; //------------ Set This
                //                var strBranchAddress = '2095 2nd Level Abreeza Ayala Mall, Davao City, Philippines'; //------------ Set This
                //                var strTIN = '006-378-995-003'; //------------ Set This
                //                var strTel = '63 82 3210081'; //------------ Set This

                //                var strMIN = '110224833'; //------------ Set This
                //                var strPrintedBy = 'FERENAL, NAREL BITOY'; //------------ Set This
                //                var strPrintedDate = '01/01/2014 09:04:52 PM'; //------------ Set This

                //                var strInvStart = SetPrefixPostfix('432963', intDesktopPrintLenght - 16, ' ', 0); //------------ Set This
                //                var strInvEnd = SetPrefixPostfix('432988', intDesktopPrintLenght - 14, ' ', 0); //------------ Set This
                //                var strTransCount = SetPrefixPostfix('26', intDesktopPrintLenght - 20, ' ', 0); //------------ Set This

                //                var japplet = document.jzebra;
                //                var blnIsPrinterFound = false;

                //                if (japplet != null) {
                //                    while (!blnIsPrinterFound) {

                //                        if (FindDesktopPrinter("")) {
                //                            blnIsPrinterFound = true;

                //                            strPrint = SetTextAlign($.trim(strTradeName || ''), 1, intDesktopPrintLenght);
                //                            japplet.append(strPrint + "\n");

                //                            strPrint = SetTextAlign($.trim(strCorporateName || ''), 1, intDesktopPrintLenght);
                //                            japplet.append(strPrint + "\n");

                //                            strPrint = SetTextAlign($.trim(strBranchAddress || ''), 1, intDesktopPrintLenght);
                //                            japplet.append(strPrint + "\n");

                //                            strPrint = SetTextAlign('TIN ' + $.trim(strTIN || ''), 1, intDesktopPrintLenght);
                //                            japplet.append(strPrint + "\n");

                //                            strPrint = SetTextAlign('Tel. No.' + $.trim(strTel || ''), 1, intDesktopPrintLenght);
                //                            japplet.append(strPrint + "\n\n");

                //                            strPrint = SetTextAlign("X-REPORT", 1, intDesktopPrintLenght);
                //                            japplet.append(strPrint + "\n");

                //                            strPrint = SetTextAlign("MIN : " + strMIN, 1, intDesktopPrintLenght);
                //                            japplet.append(strPrint + "\n");

                //                            strPrint = SetTextAlign('Date : ' + strDate, 1, intDesktopPrintLenght);
                //                            japplet.append(strPrint + "\n");

                //                            var dblTotalSales = 0;
                //                            var dblTotalSalesDiscount = 0;
                //                            var dblAdjRefund = 0;
                //                            var dblAdjSRV = 0;
                //                            var dblNetSales = 0;

                //                            var dblCash = 0;
                //                            var dblCheck = 0;
                //                            var dblDebitCard = 0;
                //                            var dblCreditCard = 0;
                //                            var dblSRV = 0;
                //                            var dblTotalPayment = 0;

                //                            var dblVAT = 0;
                //                            var dblVATAmount = 0;
                //                            var dblNonVAT = 0;
                //                            var dblVATExempt = 0;
                //                            var dblZeroRatedVAT = 0;
                //                            var dblTotalVAT = 0;

                //                            var intTransCount = 0;
                //                            var intSalesTransCount = 0;
                //                            var intSalesItemCount = 0;
                //                            var intSRVTransCount = 0;
                //                            var intSRVItemCount = 0;
                //                            var intRefTransCount = 0;
                //                            var intRefItemCount = 0;
                //                            var dblDailySale = 0;
                //                            var dblSalesVAT = 0;
                //                            var intInvStartNo = 0;
                //                            var intInvendNo = 0;
                //                            var dblOldGT = 0;
                //                            var dblNewGT = 0;

                //                            dblTotalSales = 64176.34; //------------ Set This
                //                            dblNetSales = dblTotalSales;

                //                            dblCash = dblTotalSales;
                //                            dblTotalPayment = dblTotalSales;

                //                            dblVAT = dblTotalSales / 1.12;
                //                            dblVATAmount = dblTotalSales - dblVAT;
                //                            dblNonVAT = 0;
                //                            dblVATExempt = 0;
                //                            dblZeroRatedVAT = 0;
                //                            dblTotalVAT = dblVAT + dblVATAmount;

                //                            intSalesTransCount = intTransCount;
                //                            intSRVTransCount = 0;
                //                            intRefTransCount = 0;

                //                            dblDailySale = dblTotalSales;
                //                            dblSalesVAT = dblTotalVAT;

                //                            dblOldGT = 60091647.68; //------------ Set This
                //                            dblNewGT = dblOldGT + dblDailySale;

                //                            //Date Sales Summary
                //                            japplet.append("\n");
                //                            var strTotalSales = SetPrefixPostfix(AccountingFormat(dblTotalSales), intDesktopPrintLenght - 16, ' ', 0);
                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Gross Sales ', 0, 12), 12, ': ' + strTotalSales, intDesktopPrintLenght, '', 0);
                //                            japplet.append(strPrint + "\n");

                //                            //Sales Discount
                //                            var strSalesDiscount = SetPrefixPostfix(AccountingFormat(dblTotalSalesDiscount), intDesktopPrintLenght - 22, ' ', 0);
                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Total Sales Discount', 20, strSalesDiscount, intDesktopPrintLenght, '', 0);
                //                            japplet.append(strPrint + "\n");

                //                            japplet.append("\n");
                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Summary of Adjustments', 0, intDesktopPrintLenght), intDesktopPrintLenght, '', 0, '', 0);
                //                            japplet.append(strPrint + "\n");

                //                            var strAdjRefund = SetPrefixPostfix(AccountingFormat(dblAdjRefund), intDesktopPrintLenght - 28, ' ', 0);
                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Total Sales Refund for the Period', 26, strAdjRefund, intDesktopPrintLenght, '', 0);
                //                            japplet.append(strPrint + "\n");

                //                            var strAdjSRV = SetPrefixPostfix(AccountingFormat(dblAdjSRV), intDesktopPrintLenght - 32, ' ', 0);
                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Total Sales Returns Subject to Merchadise Exchange           (SRV Issued)', 30, strAdjSRV, intDesktopPrintLenght, '', 0);
                //                            japplet.append(strPrint + "\n");

                //                            var strNetSalesAdj = SetPrefixPostfix(AccountingFormat(parseFloat(dblAdjRefund) + parseFloat(dblAdjSRV)), intDesktopPrintLenght - 20, ' ', 0);
                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Net of Adjustments', 18, strNetSalesAdj, intDesktopPrintLenght, '', 0);
                //                            japplet.append(strPrint + "\n");

                //                            strPrint = SetTextAlign('___________________________________________', 1, intDesktopPrintLenght);
                //                            japplet.append('\n' + strPrint + "\n");

                //                            var strNetSales = SetPrefixPostfix(AccountingFormat(dblNetSales), intDesktopPrintLenght - 11, ' ', 0);
                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Net Sales', 9, strNetSales, intDesktopPrintLenght, '', 0);
                //                            japplet.append(strPrint + "\n");
                //                            //End Date Sales Summary

                //                            //Sales Grouped by Mode of Payment----------------------------
                //                            japplet.append("\n");
                //                            strPrint = SetTextAlign("Inventory of Payments", 1, intDesktopPrintLenght);
                //                            japplet.append(strPrint + "\n");
                //                            japplet.append("\n");

                //                            var strTotalCash = SetPrefixPostfix(AccountingFormat(dblCash), intDesktopPrintLenght - 16, ' ', 0);
                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Cash        ', 0, 12), 12, ': ' + strTotalCash, intDesktopPrintLenght, '', 0);
                //                            japplet.append(strPrint + "\n");

                //                            var strTotalCheck = SetPrefixPostfix(AccountingFormat(dblCheck), intDesktopPrintLenght - 16, ' ', 0);
                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Check       ', 0, 12), 12, ': ' + strTotalCheck, intDesktopPrintLenght, '', 0);
                //                            japplet.append(strPrint + "\n");

                //                            var strTotalDebitCard = SetPrefixPostfix(AccountingFormat(dblDebitCard), intDesktopPrintLenght - 16, ' ', 0);
                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Debit Card  ', 0, 12), 12, ': ' + strTotalDebitCard, intDesktopPrintLenght, '', 0);
                //                            japplet.append(strPrint + "\n");

                //                            var strTotalCreditCard = SetPrefixPostfix(AccountingFormat(dblCreditCard), intDesktopPrintLenght - 16, ' ', 0);
                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Credit Card ', 0, 12), 12, ': ' + strTotalCreditCard, intDesktopPrintLenght, '', 0);
                //                            japplet.append(strPrint + "\n");

                //                            var strTotalSRV = SetPrefixPostfix(AccountingFormat(dblSRV), intDesktopPrintLenght - 16, ' ', 0);
                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('SRV         ', 0, 12), 12, ': ' + strTotalSRV, intDesktopPrintLenght, '', 0);
                //                            japplet.append(strPrint + "\n");

                //                            strPrint = SetTextAlign('___________________________________________', 1, intDesktopPrintLenght);
                //                            japplet.append(strPrint + "\n");

                //                            var strTotalClassPayment = SetPrefixPostfix(AccountingFormat(dblTotalPayment), intDesktopPrintLenght - 16, ' ', 0);
                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('            ', 0, 12), 12, ': ' + strTotalClassPayment, intDesktopPrintLenght, '', 0);
                //                            japplet.append(strPrint + "\n");
                //                            //End Sales Grouped by Mode of Payment----------------------------

                //                            //Sales Grouped by VAT TYPE----------------------------
                //                            japplet.append("\n");
                //                            strPrint = SetTextAlign("Sales Grouped by VAT Type", 1, intDesktopPrintLenght);
                //                            japplet.append(strPrint + "\n");
                //                            japplet.append("\n");

                //                            var dblTotalVATSalesPercent = (dblVAT / dblTotalVAT) * 100;
                //                            var dblTotalVATAmountPercent = (dblVATAmount / dblTotalVAT) * 100;
                //                            var dblTotalNonVATPercent = (dblNonVAT / dblTotalVAT) * 100;
                //                            var dblTotalVATExemptPercent = (dblVATExempt / dblTotalVAT) * 100;
                //                            var dblTotalZeroRatedVATPercent = (dblZeroRatedVAT / dblTotalVAT) * 100;
                //                            var dblTotalVATPercent = dblTotalVATSalesPercent + dblTotalVATAmountPercent + dblTotalNonVATPercent + dblTotalVATExemptPercent + dblTotalZeroRatedVATPercent;

                //                            var strTotalVAT = SetPrefixPostfix(AccountingFormat(dblVAT), 14, ' ', 0);
                //                            var strTotalVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATSalesPercent) + '%', intDesktopPrintLenght - 32, ' ', 0);
                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('VAT-able Sales', 0, 14), 14, strTotalVAT, 30, strTotalVATPercent, intDesktopPrintLenght);
                //                            japplet.append(strPrint + "\n");

                //                            var strTotalVATAmount = SetPrefixPostfix(AccountingFormat(dblVATAmount), 14, ' ', 0);
                //                            var strTotalVATPercentAmount = SetPrefixPostfix(AccountingFormat(dblTotalVATAmountPercent) + '%', intDesktopPrintLenght - 32, ' ', 0);
                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('VAT Amount', 0, 14), 14, strTotalVATAmount, 30, strTotalVATPercentAmount, intDesktopPrintLenght);
                //                            japplet.append(strPrint + "\n");

                //                            var strTotalNonVAT = SetPrefixPostfix(AccountingFormat(dblNonVAT), 14, ' ', 0);
                //                            var strTotalNonVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalNonVATPercent) + '%', intDesktopPrintLenght - 32, ' ', 0);
                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Non-VAT', 0, 14), 14, strTotalNonVAT, 30, strTotalNonVATPercent, intDesktopPrintLenght);
                //                            japplet.append(strPrint + "\n");

                //                            var strTotalVATExempt = SetPrefixPostfix(AccountingFormat(dblVATExempt), 14, ' ', 0);
                //                            var strTotalVATExemptPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATExemptPercent) + '%', intDesktopPrintLenght - 32, ' ', 0);
                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('VAT-Exempt', 0, 14), 14, strTotalVATExempt, 30, strTotalVATExemptPercent, intDesktopPrintLenght);
                //                            japplet.append(strPrint + "\n");

                //                            var strTotalZeroRatedVAT = SetPrefixPostfix(AccountingFormat(dblZeroRatedVAT), 14, ' ', 0);
                //                            var strTotalZeroRatedVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalZeroRatedVATPercent) + '%', intDesktopPrintLenght - 32, ' ', 0);
                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Zero-Rated VAT', 0, 14), 14, strTotalZeroRatedVAT, 30, strTotalZeroRatedVATPercent, intDesktopPrintLenght);
                //                            japplet.append(strPrint + "\n");

                //                            strPrint = SetTextAlign('___________________________________________', 1, intDesktopPrintLenght);
                //                            japplet.append(strPrint + "\n");

                //                            var strTotalAllVAT = SetPrefixPostfix(AccountingFormat(dblTotalVAT), 14, ' ', 0);
                //                            var strTotalAllVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATPercent) + '%', intDesktopPrintLenght - 32, ' ', 0);
                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, '              ', 14, strTotalAllVAT, 30, strTotalAllVATPercent, intDesktopPrintLenght);
                //                            japplet.append(strPrint + "\n");
                //                            //End Sales Grouped by VAT TYPE----------------------------

                //                            //Other Informatoin
                //                            japplet.append("\n");
                //                            strPrint = SetTextAlign('Other Information', 1, intDesktopPrintLenght);
                //                            japplet.append(strPrint + "\n\n");

                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Total Trans. Count', 0, 18), 18, strTransCount + '', intDesktopPrintLenght, '', 0);
                //                            japplet.append(strPrint + "\n");

                //                            var strSalesTransCount = SetPrefixPostfix($.format.number(intSalesTransCount, '#,##0') + '', intDesktopPrintLenght - 20, ' ', 0);
                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Sales Trans. Count', 0, 18), 18, strSalesTransCount + '', intDesktopPrintLenght, '', 0);
                //                            japplet.append(strPrint + "\n");

                //                            var strSRVTransCount = SetPrefixPostfix(intSRVTransCount + '', intDesktopPrintLenght - 18, ' ', 0);
                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('SRV Trans. Count', 0, 16), 16, strSRVTransCount + '', intDesktopPrintLenght, '', 0);
                //                            japplet.append(strPrint + "\n");

                //                            var strRefTransCount = SetPrefixPostfix(intRefTransCount + '', intDesktopPrintLenght - 21, ' ', 0);
                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Refund Trans. Count', 0, 19), 19, strRefTransCount + '', intDesktopPrintLenght, '', 0);
                //                            japplet.append(strPrint + "\n");

                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Start Inv. No.', 0, 14), 14, strInvStart + '', intDesktopPrintLenght, '', 0);
                //                            japplet.append(strPrint + "\n");

                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('End Inv. No.', 0, 12), 12, strInvEnd + '', intDesktopPrintLenght, '', 0);
                //                            japplet.append(strPrint + "\n");

                //                            var strOldGT = SetPrefixPostfix(AccountingFormat(dblOldGT), intDesktopPrintLenght - 17, ' ', 0);
                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Old Grand Total', 0, 15), 15, strOldGT + '', intDesktopPrintLenght, '', 0);
                //                            japplet.append(strPrint + "\n");

                //                            var strNewGT = SetPrefixPostfix(AccountingFormat(dblNewGT), intDesktopPrintLenght - 17, ' ', 0);
                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('New Grand Total', 0, 15), 15, strNewGT + '', intDesktopPrintLenght, '', 0);
                //                            japplet.append(strPrint + "\n");
                //                            //END Other Informatoin

                //                            japplet.append("\n");
                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Printed by: ' + strPrintedBy, intDesktopPrintLenght, '', 0, '', 0);
                //                            japplet.append(strPrint + "\n");

                //                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Date and Time: ' + strPrintedDate, intDesktopPrintLenght, '', 0, '', 0);
                //                            japplet.append(strPrint + "\n");

                //                            japplet.append("\n\n\n\n\n\n\n\n\n\n\n\n\n");
                //                            japplet.print();

                //                        }

                //                    }

                //                } else {
                //                    MessageBox("Jzebra applet was not loaded.",
                //                                "Jzebra",
                //                                [
                //                                    { Text: 'Ok'
                //                                    }
                //                                ]
                //                    );
                //                }
                //            }

            });

        }); //require(reqJS, function () {
    });