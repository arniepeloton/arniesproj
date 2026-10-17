define([
    'common',
    'jquery.plugin.html2canvas',
    GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc='),
    '/Scripts/sources/ReportsPOS/SalesReport_offline.js?crc=',
    (OfflineMode ? '/Scripts3/Offline/Browser/LibraryFowst.js?crc=' : '')
    ], function () {

        var reqJS;

        if (OfflineMode) {
            reqJS = [
                '/Scripts/sources/SalesPayments/SalesPaymentsExport_offline.js?crc=',
                '/Scripts/sources/SalesPayments/SalesPayments_offline.js?crc=',
                '/Scripts3/SalesMenu_offline.js?crc='
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

                var newdateFrom = new Date();
                var newdateTo = new Date();

                setMonthCollection("selZReportByDateMonth");
                setYearCollection("selZReportByDateYear");

                if (IsMobile) {
                    if (Web.HandleOrganization() == false)
                        return;
                }

                $(document).ready(function () {

                    GetAllFOWST()
                    dateFrom = new Date();

                    pDeferred.resolve({});
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


                $('body').on('click', '#btnPOSSalesZReportView', function (e) {
                    e.preventDefault();
                    ViewZReport();

                })
                .on('click', '#btnPOSSalesZReportPrint', function (e) {
                    e.preventDefault();

                    PrintSKUUnitSalesReport(false);
                })
                
                .on('click', '#btnPOSSalesZReportCancel', function (e) {
                    e.preventDefault();

                    window.open(GetPathFormat("/People/PeopleMenu"), "_self");
                })
                .on('click', '.btnZReportSelectFowst', function (e) {
                    e.preventDefault();
                    //$('#panZReportFowst').popup('open');
                    BrowsePopup('LibraryFowst', 'Select FOWST', [
                        {
                            DisplayText: [{ Column: 'FowstName' }],
                            Offline: true,
                            AutoSearch: true,
                            WithSearchbox: false,
                            Click: function (e) {
                                if (CStr2(e.IsFOWSTInsideAMall) == '1' && CStr2(e.FOWSTMallName) == 'SM Mall') {
                                    IsFowstInSMMall = true;
                                } else {
                                    IsFowstInSMMall = false;
                                }
                                sessionStorage.FowstID = e.ID;
                                sessionStorage.SelectedFOWST = e.ID;
                                $('.lblZReportSelectedFowstGroup').text(e.FowstName);
                                $('#panZReportFowst').popup('close');
                                $('.btnZReportSelectFowst').removeClass('btnRequired').addClass('btnNotRequired');
                                ViewZReport();
                            }
                        }
                    ], { FilterFowst: 1 })
                })
                .on('click', '#btnZReportDateRange', function (e) {
                    PopupPeriod({
                        Display2DaysAgo: false,
                        DisplayYesterday: true,
                        DisplayToday: true,
                        DisplayWeekToDate: false,
                        DisplayMonthToDate: false,
                        DisplayCustomPeriod: true,
                        Click: function (e) {
                            if (e.Period == 'Yesterday') {
                                rangeType = "Yesterday";
                                var date = $.format.date(new Date(e.From), 'MM d yyyy');
                                dateFrom = date;
                                dateTo = date;
                                $('#btnZReportDateRange').text('Yesterday');
                            } else if (e.Period == 'Today') {
                                rangeType = "Today";
                                var date = $.format.date(new Date(e.From), 'MM d yyyy');
                                dateFrom = date;
                                dateTo = date;
                                $('#btnZReportDateRange').text('Today');
                            } else if (e.Period == 'Custom') {
                                dateFrom = $.format.date(new Date(e.From), 'MM/dd/yyyy');
                                dateTo = $.format.date(new Date(e.To), 'MM/dd/yyyy');
                                dateYear = '';
                                rangeType = "By Date";
                                $('#btnZReportDateRange').text(dateFrom + ' to ' + dateTo);

                                if ($('#lblZReportSelectedFowstGroup').text() != "FOWST Location") {
                                    ViewZReport();
                                }
                            }
                        }
                    })
                    //$('#panZReportDate').popup('open');
                })
                //.on('click', '#liZReportYesterday', function (e) {
                //    e.preventDefault();
                //    rangeType = "Yesterday";

                //    dateFrom = new Date((newdateFrom.getMonth() + 1) + "/" + (newdateFrom.getDate() - 1) + "/" + newdateFrom.getFullYear());
                //    dateTo = new Date((newdateTo.getMonth() + 1) + "/" + (newdateTo.getDate() - 1) + "/" + newdateFrom.getFullYear());

                //    $('#ZReportDateRangeFrom').text(rangeType);
                //    $('#ZReportDateRangeSeparator, #ZReportDateRangeTo').hide();
                //    $('#panZReportDate').popup('close');
                //})
                //.on('click', '#liZReportToday', function (e) {
                //    e.preventDefault();
                    

                //    dateFrom = new Date((newdateFrom.getMonth() + 1) + "/" + (newdateFrom.getDate()) + "/" + newdateFrom.getFullYear());
                //    dateTo = new Date((newdateTo.getMonth() + 1) + "/" + (newdateTo.getDate()) + "/" + newdateFrom.getFullYear());

                //    $('#ZReportDateRangeFrom').text(rangeType);
                //    $('#ZReportDateRangeSeparator, #ZReportDateRangeTo').hide();
                //    $('#panZReportDate').popup('close');
                //})
                //.on('click', '#liZReportCustomPeriod', function (e) {
                //    rangeType = "By Date";
                //    $('#panZReportDate').popup('close');
                //    $('#panZreportCustomDate').popup('open');
                //})
                //.on('click', '#btnZreportCancel', function (e) {
                //    $('#panZreportCustomDate').popup('close');
                //})
                //.on('click', '#btnZreportDone', function (e) {
                //    dateFrom = new Date($('#txtZreportDateFrom').text());
                //    dateTo = new Date($('#txtZreportDateTo').text());
                //    var dateNow = new Date();

                //    if ((dateFrom > dateTo || dateFrom > dateNow || dateTo > dateNow) || $('#txtZreportDateFrom').text() == "" || $('#txtZreportDateTo').text() == "") {
                //        $('#spanZreportDateRangeError').show();
                //    }
                //    else {
                //        $('#spanZreportDateRangeError').hide();
                //        $('#btnZReportDateRange').removeClass('btnRequired').addClass('btnNotRequired').attr('title', $('#txtZreportDateFrom').text() + ' to ' + $('#txtZreportDateTo').text())
                //        $('#ZReportDateRangeFrom').text($('#txtZreportDateFrom').text());
                //        $('#ZReportDateRangeTo').text($('#txtZreportDateTo').text());
                //        $('#ZReportDateRangeSeparator').show();
                //        $('#panZreportCustomDate').popup('close');
                //        //dateFrom = $('#txtZreportDateFrom').text();
                //        //dateTo = $('#txtZreportDateTo').text();
                //        dateYear = '';
                //        newRangeType = "Day";

                //        if ($('#lblZReportSelectedFowstGroup').text() != "FOWST Location") {
                //            ViewZReport();
                //        }
                //    }
                //});


                
                function ViewZReport() {
                    if (OfflineMode) {
                        // 2016_01_09__13_43_09_6615
                        // dont flush anymore.
                        PrintSKUUnitSalesReport(true);

                        //flushedCounter = 0;
                        //mInterval = setInterval(FlushDeviceData, 1000);
                    } else {
                        PrintSKUUnitSalesReport(true);
                    }
                }

                function PrintSKUUnitSalesReport(vIsView) {
                    var intFAFCTotalLenght = 40;
                    var vDataInfo;
                    var vDataPrint;
                    var strPrint = '';

                    //dateFrom = new Date($('#txtCashierDateFrom').text());

                    //New set of text to print
                    //NewPrintBuilder();

                    if (vIsView) {

                    } else {
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
                        }

                        vDataPrint = SalesPayment_offline().getOrgFOWSTPrinterIP();

                        if (vDataPrint.Rows.length > 0) {
                            strPrinterIP = vDataPrint.Rows[0].PrinterIPAddress;
                        }                        

                        vDataHeader = SalesReport_offline().getReportHeader(0);                        

                        //Display Header
                        //DisplayHeader(vDataHeader, vIsView);

                        pSql = 'Select * from TransSalesProductCount where date(TransactionDate) = ' + ReadyString($.format.date(dateFrom, "yyyy-MM-dd"));

                        vDataInfo = Sql.Execute(pSql);

                        if (isPrintInWPS) {
                            WindowsPrinter('Z-READ', [1, 0, 0, 1, 0, 36]);

                        } else {
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

                            strPrint = FormatTextToPrint(intFAFCTotalLenght, "Sales by SKU Report", intFAFCTotalLenght, '', 0, '', 0, 0);
                            ReadyTextToPrint(strPrint, 3, 2, 1, 1, 1, 1, 0, 0, 0);
                        }

                        if (vDataInfo.Rows.length > 0) {

                            if (vIsView) {
                                $("#lblZDate").text('Date : ' + $.format.date(new Date(vDataInfo.Rows[0].SalesDate), 'MM/dd/yyyy'));
                            } else {
                                if (isPrintInWPS) {
                                    WindowsPrinter('Date : ' + $.format.date(new Date(vDataInfo.Rows[0].SalesDate), 'MM/dd/yyyy'), [1, 0, 0, 1, 0, 36]);
                                    WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                } else {
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Date : ' + $.format.date(new Date(vDataInfo.Rows[0].TransactionDate), 'MM/dd/yyyy'), intFAFCTotalLenght, '', 0, '', 0, 0);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                }

                            }
                        }

                        strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);

                        strPrint = strPrint + '\n';

                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                        strPrint = '';

                        for (var p = 0; p <= vDataInfo.Rows.length - 1; p++) {
                            var strStockDescription = $.trim((vDataInfo.Rows[p].ProductDescription || ''));

                            var strQty = SetPrefixPostfix(AccountingFormat((vDataInfo.Rows[p].Qty || '0'), 0), 8, ' ', 0);
                            var strUnitMeasure = SetPrefixPostfix((vDataInfo.Rows[p].UnitMeasure || ''), 10, ' ', 0);
                            var strTotalSales = SetPrefixPostfix((AccountingFormat(vDataInfo.Rows[p].TotalSales) || ''), 10, ' ', 0);

                            strTemp = FormatTextToPrint(intFAFCTotalLenght, '   ' + strStockDescription, intFAFCTotalLenght, '', 0, '', 0, 0);
                            strPrint = strPrint + strTemp;
                                                        
                            strTemp = FormatTextToPrint(intFAFCTotalLenght, '   ' + 'Qty Sold:   ' + strQty, 25, strUnitMeasure, intFAFCTotalLenght, '', 0, 1);
                            strPrint = strPrint + strTemp;

                            strTemp = FormatTextToPrint(intFAFCTotalLenght, '   ' + 'Sales Amount:' + strTotalSales, 25, '', intFAFCTotalLenght, '', 0, 0);
                            strPrint = strPrint + strTemp;

                            strPrint = strPrint + '\n';

                        }

                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                        strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                        ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);

                        if (vIsView) {
                            
                        } else {
                            if (isPrintInWPS) {
                                
                            
                            } else {

                                PrintText(strPrinterIP);
                                //ClearCommandBuffer();
                            }

                        } //if (vIsView) {

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
                        
                    }
                    else {

                        BrowsePopup('LibraryFowst', 'Select FOWST', [
                        {
                            DisplayText: [{ Column: 'FowstName' }],
                            Offline: true,
                            AutoSearch: true,
                            WithSearchbox: false,
                            Click: function (e) {
                                if (CStr2(e.IsFOWSTInsideAMall) == '1' && CStr2(e.FOWSTMallName) == 'SM Mall') {
                                    IsFowstInSMMall = true;
                                } else {
                                    IsFowstInSMMall = false;
                                }
                                sessionStorage.FowstID = e.ID;
                                sessionStorage.SelectedFOWST = e.ID;
                                $('.lblZReportSelectedFowstGroup').text(e.FowstName);
                                $('#panZReportFowst').popup('close');
                                $('.btnZReportSelectFowst').removeClass('btnRequired').addClass('btnNotRequired');
                                ViewZReport();
                            }
                        }
                        ], { FilterFowst: 1 })
                    }

                    try {
                        pLV.listview('refresh');
                    }
                    catch (e) {
                        pLV.listview();
                    }

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
                            //NewPrintBuilder();

                            //strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].OrgBusinessTradeName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                            //strPrint = '';
                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].SubscriberOrganizationName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            //strPrint = strPrint + strTemp;

                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].OrgBranchName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            //strPrint = strPrint + strTemp;

                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].OrgCompleteAddress || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            //strPrint = strPrint + strTemp;

                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'TIN ' + $.trim((vDataHeader.Rows[0].OrgTIN || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            //strPrint = strPrint + strTemp;

                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Tel. No.' + $.trim((vDataHeader.Rows[0].OrgTelNo || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            //strPrint = strPrint + strTemp;

                            //strPrint = strPrint + '\n';

                            //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);
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


                    if (vDataInfo.Rows.length > 0) {

                        if (vIsView) {
                            $("#lblZDate").text('Date : ' + $.format.date(new Date(vDataInfo.Rows[0].SalesDate), 'MM/dd/yyyy'));
                        } else {
                            if (isPrintInWPS) {
                                WindowsPrinter('Date : ' + $.format.date(new Date(vDataInfo.Rows[0].SalesDate), 'MM/dd/yyyy'), [1, 0, 0, 1, 0, 36]);
                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                            } else {
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Date : ' + $.format.date(new Date(vDataInfo.Rows[0].SalesDate), 'MM/dd/yyyy'), intFAFCTotalLenght, '', 0, '', 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                            }

                        }

                        var dblTotalSales = 0;
                        var dblTotalSalesDiscount = 0;
                        var dblTotalVATExempt = 0;
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
                            dblTotalSalesDiscount = dblTotalSalesDiscount + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalDiscount || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].CurSCDiscount || 0);
                            dblTotalVATExempt = dblTotalVATExempt + parseFloat(vDataInfo.ReportPOS.Rows[p].CurVATExempt || 0);
                            dblAdjRefund = dblAdjRefund + parseFloat(vDataInfo.ReportPOS.Rows[p].CurSalesReturnRefund || 0);
                            dblAdjSRV = dblAdjSRV + parseFloat(vDataInfo.ReportPOS.Rows[p].CurSalesReturnExchange || 0);

                            dblCash = dblCash + parseFloat(vDataInfo.ReportPOS.Rows[p].CashAmount || 0);
                            dblCheck = dblCheck + parseFloat(vDataInfo.ReportPOS.Rows[p].CheckPaymentAmount || 0);
                            dblDebitCard = dblDebitCard + parseFloat(vDataInfo.ReportPOS.Rows[p].DebitCardAmount || 0);
                            dblCreditCard = dblCreditCard + parseFloat(vDataInfo.ReportPOS.Rows[p].CreditCardAmount || 0);
                            dblSRV = dblSRV + parseFloat(vDataInfo.ReportPOS.Rows[p].SRVAmount || 0);
                            dblOnCredit = dblOnCredit + parseFloat(vDataInfo.ReportPOS.Rows[p].OnCreditAmount || 0);
                            dblOtherPaymentType = dblOtherPaymentType + parseFloat(vDataInfo.ReportPOS.Rows[p].OtherTypeAmount || 0);
                            dblCharges = dblCharges + parseFloat(vDataInfo.ReportPOS.Rows[p].ChargeAmount || 0);
                            dblSMAC = dblSMAC + parseFloat(vDataInfo.ReportPOS.Rows[p].SMACAmount || 0);
                            dblEPlus = dblEPlus + parseFloat(vDataInfo.ReportPOS.Rows[p].EPlusAmount || 0);
                            dblVoucher = dblVoucher + parseFloat(vDataInfo.ReportPOS.Rows[p].VoucherAmount || 0);
                            dblServiceCharge = dblServiceCharge + parseFloat(vDataInfo.ReportPOS.Rows[p].ServiceCharge || 0);
                            dblLocalTax = dblLocalTax + parseFloat(vDataInfo.ReportPOS.Rows[p].LocalTax || 0);


                            dblVAT = dblVAT + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVatablePortionOfSCDTransaction || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVATSales || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVATSalesSalesReturn || 0);
                            dblVATAmount = dblVATAmount + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVatAmountPortionOfSCDTransaction || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVAT || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVATAmountSalesReturn || 0);
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

                            dblDailySale = dblDailySale + parseFloat(vDataInfo.ReportPOS.Rows[p].CurGrossSales || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].CurVATExempt || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].CurSCDiscount || 0) - (vDataInfo.ReportPOS.Rows[p].CurTotalDiscount || 0) - (vDataInfo.ReportPOS.Rows[p].CurSalesReturnRefund || 0) - (vDataInfo.ReportPOS.Rows[p].CurSalesReturnExchange || 0) - (vDataInfo.ReportPOS.Rows[p].CurServiceCharge || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVatAmountPortionOfSCDTransaction || 0) - (vDataInfo.ReportPOS.Rows[p].CurTotalVAT || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].ChargeAmount || 0);
                            dblSalesVAT = dblSalesVAT + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVatAmountPortionOfSCDTransaction || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVAT || 0);
                            dblOldGT = dblOldGT + parseFloat(vDataInfo.ReportPOS.Rows[p].BegBal || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].PrevGrossSales || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].PrevTotalDiscount || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].PrevSalesReturnRefund || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].PrevSalesReturnExchange || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].PrevServiceCharge || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].PrevChargeAmount || 0);

                            zReadCount += CInt2(vDataInfo.ReportPOS.Rows[p].ZCount);
                        };

                        dblNewGT = dblOldGT + dblDailySale + dblSalesVAT;
                        dblNetSales = dblTotalSales - dblTotalSalesDiscount - dblTotalVATExempt - dblAdjRefund - dblAdjSRV + dblCharges;
                        dblTotalPayment = dblCash + dblCheck + dblDebitCard + dblCreditCard + dblSRV + dblOtherPaymentType + dblOnCredit;
                        dblTotalVAT = dblVAT + dblVATAmount + dblNonVAT + dblVATExempt + dblZeroRatedVAT;

                        if (vIsView) {
                            //Date Sales Summary
                            $("#lblGrossSales").text(AccountingFormat(dblTotalSales));
                            $("#lblSalesDiscount").text(AccountingFormat(dblTotalSalesDiscount));
                            $("#lblVatExemptTAX").text(AccountingFormat(dblTotalVATExempt));
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
                            $("#lblOnCredit").text(AccountingFormat(dblOnCredit));
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
                                WindowsPrinter(" Gross Sales ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblTotalSales), [0, 15, 0, 0, 0, 36]);

                                //var strTotalSales = SetPrefixPostfix(AccountingFormat(dblTotalSales), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Gross Sales: ', 14, strTotalSales, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Sales Discount ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblTotalSalesDiscount), [0, 15, 0, 0, 0, 36]);
                                //WindowsPrinter(' Sales Discount: ' + (AccountingFormat(dblTotalSalesDiscount) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strTotalSalesDiscount = SetPrefixPostfix(AccountingFormat(dblTotalSalesDiscount), intFAFCTotalLenght - 19, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Sales Discount: ', 17, strTotalSalesDiscount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Vat Exempt Tax ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblTotalVATExempt), [0, 15, 0, 0, 0, 36]);

                                WindowsPrinter(" Service Charge ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblServiceCharge), [0, 15, 0, 0, 0, 36]);
                                //WindowsPrinter(' Service Charge: ' + (AccountingFormat(dblServiceCharge) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strServiceCharge = SetPrefixPostfix(AccountingFormat(dblServiceCharge), intFAFCTotalLenght - 19, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Service Charge: ', 17, strServiceCharge, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Local Tax ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblLocalTax), [0, 15, 0, 0, 0, 36]);
                                //WindowsPrinter(' Local Tax: ' + (AccountingFormat(dblLocalTax) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strLocalTax = SetPrefixPostfix(AccountingFormat(dblLocalTax), intFAFCTotalLenght - 19, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Local Tax: ', 17, strLocalTax, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                WindowsPrinter(' Summary of Adjustments ', [1, 0, 0, 1, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Summary of Adjustments', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Total Sales Refund for the  Period ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblAdjRefund), [0, 15, 0, 0, 0, 36]);
                                //WindowsPrinter(' Total Sales Refund for the  Period ' + (AccountingFormat(dblAdjRefund) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strAdjRefund = SetPrefixPostfix(AccountingFormat(dblAdjRefund), intFAFCTotalLenght - 29, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Sales Refund for the  Period', 27, strAdjRefund, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Total Sales Returns Subject to Merchadise Exchange           (SRV Issued) ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblAdjSRV), [0, 15, 0, 0, 0, 36]);
                                //WindowsPrinter(' Total Sales Returns Subject to Merchadise Exchange           (SRV Issued) ' + (AccountingFormat(dblAdjSRV) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strAdjSRV = SetPrefixPostfix(AccountingFormat(dblAdjSRV), intFAFCTotalLenght - 32, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Sales Returns Subject to Merchadise Exchange           (SRV Issued)', 30, strAdjSRV, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Net of Adjustments ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblAdjRefund), [0, 15, 0, 0, 0, 36]);
                                //WindowsPrinter(' Net of Adjustments: ' + (AccountingFormat(dblAdjRefund) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strNetSalesAdj = SetPrefixPostfix(AccountingFormat(parseFloat(dblAdjRefund) + parseFloat(dblAdjSRV)), intFAFCTotalLenght - 23, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Net of Adjustments: ', 21, strNetSalesAdj, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                //WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                WindowsPrinter(" Net Sales ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblNetSales), [0, 15, 0, 0, 0, 36]);
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

                                WindowsPrinter(" Cash       ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblCash), [0, 20, 0, 0, 0, 36]);
                                //WindowsPrinter(' Cash       ' + (AccountingFormat(dblCash) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strTotalCash = SetPrefixPostfix(AccountingFormat(dblCash), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Cash       ', 12, ': ' + strTotalCash, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Check      ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblCheck), [0, 20, 0, 0, 0, 36]);
                                //WindowsPrinter(' Check      ' + (AccountingFormat(dblCheck) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strTotalCheck = SetPrefixPostfix(AccountingFormat(dblCheck), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Check      ', 12, ': ' + strTotalCheck, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Debit Card ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblDebitCard), [0, 20, 0, 0, 0, 36]);
                                //WindowsPrinter(' Debit Card ' + (AccountingFormat(dblDebitCard) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strTotalDebitCard = SetPrefixPostfix(AccountingFormat(dblDebitCard), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Debit Card ', 12, ': ' + strTotalDebitCard, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Credit Card", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblCreditCard), [0, 20, 0, 0, 0, 36]);
                                //WindowsPrinter(' Credit Card' + (AccountingFormat(dblCreditCard) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strTotalCreditCard = SetPrefixPostfix(AccountingFormat(dblCreditCard), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Credit Card', 12, ': ' + strTotalCreditCard, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" SRV        ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblSRV), [0, 20, 0, 0, 0, 36]);
                                //WindowsPrinter(' SRV        ' + (AccountingFormat(dblSRV) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strTotalSRV = SetPrefixPostfix(AccountingFormat(dblSRV), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SRV        ', 12, ': ' + strTotalSRV, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                if (IsFowstInSMMall) {
                                    WindowsPrinter(" SMAC        ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblSMAC), [0, 20, 0, 0, 0, 36]);
                                    //WindowsPrinter(' SMAC        ' + (AccountingFormat(dblSMAC) || ''), [1, 0, 0, 1, 0, 36]);
                                    //var strTotalSMAC = SetPrefixPostfix(AccountingFormat(dblSMAC), intFAFCTotalLenght - 16, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SMAC        ', 12, ': ' + strTotalSMAC, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                    WindowsPrinter(" E-Plus      ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblEPlus), [0, 20, 0, 0, 0, 36]);
                                    //WindowsPrinter(' E-Plus      ' + (AccountingFormat(dblEPlus) || ''), [1, 0, 0, 1, 0, 36]);
                                    //var strTotalEPlus = SetPrefixPostfix(AccountingFormat(dblEPlus), intFAFCTotalLenght - 16, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' E-Plus      ', 12, ': ' + strTotalEPlus, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                    WindowsPrinter(" Voucher     ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblVoucher), [0, 20, 0, 0, 0, 36]);
                                    //WindowsPrinter(' Voucher     ' + (AccountingFormat(dblVoucher) || ''), [1, 0, 0, 1, 0, 36]);
                                    //var strTotalVoucher = SetPrefixPostfix(AccountingFormat(dblVoucher), intFAFCTotalLenght - 16, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Voucher     ', 12, ': ' + strTotalVoucher, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);


                                } else {
                                    WindowsPrinter(" Other Tender", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblOtherPaymentType), [0, 20, 0, 0, 0, 36]);
                                    //WindowsPrinter(' Other Tender' + (AccountingFormat(dblOtherPaymentType) || ''), [1, 0, 0, 1, 0, 36]);
                                    //var strTotalOther = SetPrefixPostfix(AccountingFormat(dblOtherPaymentType), intFAFCTotalLenght - 16, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Other Tender', 12, ': ' + strTotalOther, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                }

                                WindowsPrinter(" On Credit     ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblOnCredit), [0, 20, 0, 0, 0, 36]);

                                WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                WindowsPrinter("            ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblTotalPayment), [0, 15, 0, 0, 0, 36]);
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

                                WindowsPrinter("VAT-able Sales", [1, 16, 0, 0, 0, 36], AccountingFormat(dblVAT), [0, 15, 0, 0, 0, 36], AccountingFormat(dblTotalVATSalesPercent) + '%', [0, 10, 0, 0, 1, 36]);
                                //strPrint = ' ';
                                //var strTotalVATSales = SetPrefixPostfix(AccountingFormat(dblVAT), 14, ' ', 0);
                                //var strTotalVATSalesPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATSalesPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-able Sales', 14, strTotalVATSales, 30, strTotalVATSalesPercent, intFAFCTotalLenght, 1);
                                //strPrint = strPrint + strTemp;

                                WindowsPrinter("VAT Amount", [1, 16, 0, 0, 0, 36], AccountingFormat(dblVATAmount), [0, 15, 0, 0, 0, 36], AccountingFormat(dblTotalVATAmountPercent) + '%', [0, 10, 0, 0, 1, 36]);
                                //var strTotalVATAmount = SetPrefixPostfix(AccountingFormat(dblVATAmount), 14, ' ', 0);
                                //var strTotalVATAmountPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATAmountPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT Amount', 14, strTotalVATAmount, 30, strTotalVATAmountPercent, intFAFCTotalLenght, 1);
                                //strPrint = strPrint + strTemp;

                                WindowsPrinter("Non-VAT", [1, 16, 0, 0, 0, 36], AccountingFormat(dblNonVAT), [0, 15, 0, 0, 0, 36], AccountingFormat(dblTotalNonVATPercent) + '%', [0, 10, 0, 0, 1, 36]);
                                //var strTotalNonVAT = SetPrefixPostfix(AccountingFormat(dblNonVAT), 14, ' ', 0);
                                //var strTotalNonVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalNonVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Non-VAT', 14, strTotalNonVAT, 30, strTotalNonVATPercent, intFAFCTotalLenght, 1);
                                //strPrint = strPrint + strTemp;

                                WindowsPrinter("VAT-Exempt", [1, 16, 0, 0, 0, 36], AccountingFormat(dblVATExempt), [0, 15, 0, 0, 0, 36], AccountingFormat(dblTotalVATExemptPercent) + '%', [0, 10, 0, 0, 1, 36]);
                                //var strTotalVATExempt = SetPrefixPostfix(AccountingFormat(dblVATExempt), 14, ' ', 0);
                                //var strTotalVATExemptPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATExemptPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-Exempt', 14, strTotalVATExempt, 30, strTotalVATExemptPercent, intFAFCTotalLenght, 1);
                                //strPrint = strPrint + strTemp;

                                WindowsPrinter("Zero-Rated VAT", [1, 16, 0, 0, 0, 36], AccountingFormat(dblZeroRatedVAT), [0, 15, 0, 0, 0, 36], AccountingFormat(dblTotalZeroRatedVATPercent) + '%', [0, 10, 0, 0, 1, 36]);
                                //var strTotalZeroRatedVAT = SetPrefixPostfix(AccountingFormat(dblZeroRatedVAT), 14, ' ', 0);
                                //var strTotalZeroRatedVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalZeroRatedVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Zero-Rated VAT', 14, strTotalZeroRatedVAT, 30, strTotalZeroRatedVATPercent, intFAFCTotalLenght, 1);
                                //strPrint = strPrint + strTemp;

                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                WindowsPrinter("              ", [1, 16, 0, 0, 0, 36], AccountingFormat(dblTotalVAT), [0, 15, 0, 0, 0, 36], AccountingFormat(dblTotalVATPercent) + '%', [0, 10, 0, 0, 1, 36]);
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

                                    WindowsPrinter(" Regular Discount ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblRegDiscount), [0, 15, 0, 0, 0, 36]);
                                    //WindowsPrinter(' Regular Discount ' + AccountingFormat(dblRegDiscount), [1, 0, 0, 1, 0, 36]);
                                    //var strTotalRegDiscount = SetPrefixPostfix(AccountingFormat(dblRegDiscount), intFAFCTotalLenght - 32, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Regular Discount', 30, strTotalRegDiscount, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    WindowsPrinter(" Employee Discount ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblEmpDiscount), [0, 15, 0, 0, 0, 36]);
                                    //WindowsPrinter(' Employee Discount ' + AccountingFormat(dblEmpDiscount), [1, 0, 0, 1, 0, 36]);
                                    //var strTotalEmpDiscount = SetPrefixPostfix(AccountingFormat(dblEmpDiscount), intFAFCTotalLenght - 32, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Employee Discount', 30, strTotalEmpDiscount, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    WindowsPrinter(" Senior Citizen ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblSCD), [0, 15, 0, 0, 0, 36]);
                                    //WindowsPrinter(' Senior Citizen ' + AccountingFormat(dblSCD), [1, 0, 0, 1, 0, 36]);
                                    //var strTotalSCD = SetPrefixPostfix(AccountingFormat(dblSCD), intFAFCTotalLenght - 32, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Senior Citizen', 30, strTotalSCD, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    WindowsPrinter(" PWD Discount ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblPWD), [0, 15, 0, 0, 0, 36]);
                                    //WindowsPrinter(' PWD Discount ' + AccountingFormat(dblPWD), [1, 0, 0, 1, 0, 36]);
                                    //var strTotalPWD = SetPrefixPostfix(AccountingFormat(dblPWD), intFAFCTotalLenght - 32, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' PWD Discount', 30, strTotalPWD, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    WindowsPrinter(" VIP Discount ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblVIP), [0, 15, 0, 0, 0, 36]);
                                    //WindowsPrinter(' VIP Discount ' + AccountingFormat(dblVIP), [1, 0, 0, 1, 0, 36]);
                                    //var strTotalVIP = SetPrefixPostfix(AccountingFormat(dblVIP), intFAFCTotalLenght - 32, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' VIP Discount', 30, strTotalVIP, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    WindowsPrinter(" GPC ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblGPC), [0, 15, 0, 0, 0, 36]);
                                    //WindowsPrinter(' GPC ' + AccountingFormat(dblGPC), [1, 0, 0, 1, 0, 36]);
                                    //var strTotalGPC = SetPrefixPostfix(AccountingFormat(dblGPC), intFAFCTotalLenght - 32, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' GPC ', 30, strTotalGPC, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    WindowsPrinter(" Discount 1 ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblDiscount1), [0, 15, 0, 0, 0, 36]);
                                    //WindowsPrinter(' Discount 1 ' + AccountingFormat(dblDiscount1), [1, 0, 0, 1, 0, 36]);
                                    //var strTotalDiscount1 = SetPrefixPostfix(AccountingFormat(dblDiscount1), intFAFCTotalLenght - 32, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Discount 1 ', 30, strTotalDiscount1, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                    WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                    WindowsPrinter(" ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblTotalSalesDiscount), [0, 15, 0, 0, 0, 36]);
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

                                WindowsPrinter("Old Grand Total ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblOldGT), [0, 20, 0, 0, 0, 36]);
                                //WindowsPrinter('Old Grand Total ' + AccountingFormat(dblOldGT), [1, 0, 0, 1, 0, 36]);
                                //var strOldGT = SetPrefixPostfix(AccountingFormat(dblOldGT), intFAFCTotalLenght - 17, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Old Grand Total', 15, strOldGT, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter("New Grand Total ", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblNewGT), [0, 20, 0, 0, 0, 36]);
                                //WindowsPrinter('New Grand Total ' + AccountingFormat(dblNewGT), [1, 0, 0, 1, 0, 36]);
                                //var strNewGT = SetPrefixPostfix(AccountingFormat(dblNewGT), intFAFCTotalLenght - 17, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'New Grand Total', 15, strNewGT, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                //END Other Informatoin

                                WindowsPrinter("Z-READ Print Count", [1, 16, 0, 0, 0, 36], ': ' + zReadCount, [0, 10, 0, 0, 0, 36]);

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

                                var strTotalVatExempt = SetPrefixPostfix(AccountingFormat(dblTotalVATExempt), intFAFCTotalLenght - 19, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Vat Exempt TAX: ', 17, strTotalVatExempt, intFAFCTotalLenght, '', 0, 1);
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
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Z-READ Print Count', 19, strzReadCount, intFAFCTotalLenght, '', 0, 1);
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
                                //ClearCommandBuffer();
                            }


                        } //if (vIsView) {

                    } else {
                        if (!vIsView) {
                            ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);
                            PrintText(strPrinterIP);
                            //ClearCommandBuffer();
                        }
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

            });

        }); //require(reqJS, function () {
        var pDeferred = $.Deferred();
        return pDeferred.promise();
    });