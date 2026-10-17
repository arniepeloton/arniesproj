define([
    'common',
    'jquery.plugin.html2canvas',
    GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc='),
    (OfflineMode ? '/Scripts3/Offline/Browser/LibraryFowst.js?crc=' : '')
    ], function () {

        var reqJS;

        if (OfflineMode) {
            reqJS = [
                '/Scripts/sources/ReportsPOS/SalesReport_offline.js?crc=',
                '/Scripts/sources/SalesPayments/SalesPaymentsExport_offline.js?crc=',
                '/Scripts/sources/SalesPayments/SalesPayments_offline.js?crc=',
                '/Scripts3/SalesMenu_offline.js?crc='
            ]
        }

        require(reqJS, function () {


            //$(function () {

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
                var IsVAT;

                var discountCols = [], discountlist, LastTotalDataMSR;

                setMonthCollection("selZReportByDateMonth");
                setYearCollection("selZReportByDateYear");

                if (IsMobile) {
                    if (Web.HandleOrganization() == false)
                        return;
                }

                if (OfflineMode) {
                    Browser.OfflineMode = true;
                    IsVAT = Sql.ExecuteScalar('Select IsVAT from LibraryOrganization WHERE ID = ' + ReadyString(sessionStorage.OrgID));
                } else {
                    AjaxSend({
                        url: 'ReportPOS/GetVATType',
                        success: function (vData) {
                            if (vData != undefined) {
                                IsVAT = CBool(vData.ReportPOS)
                            }
                        }
                    });
                }

                window.PrintSalesZReport = function (vIsView) {
                    var strTemp = '';
                    var strPrint = '';
                    var strDate = '';
                    var pData = {};

                    pData.UUID = CStr2(localStorage.getItem('UUID'));

                    if ($("#selZReportRange").val() == "") {
                        MessageBox("Please select report range.",
                                "Report Range",
                                [
                                    {
                                        Text: 'Ok'
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
                                        {
                                            Text: 'Ok'
                                        }
                                    ]
                            );
                            return;
                        }

                    }

                    strDate = $("#selZReportByDateMonth").val() + "/" + $("#selZReportByDateDay").val() + "/" + $("#selZReportByDateYear").val();

                    //always get offline zread since for the longest time, wala tayong nagiging problem with offline z-read
                    //Francis Hernandez
                    //Oct. 11, 2022

                    if (window.IsMobile)
                        sessionStorage.PrintingOfflineData = true;

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

                        //pData.RangeType = rangeType || 'Today'; //$.trim($("#selZReportRange").val());
                        pData.RangeType = rangeType || 'By Date'; //$.trim($("#selZReportRange").val());

                        if (!Business.SystemDefaults.GetEnforceZRead()) {
                            pData.RangeType = rangeType || 'Today'; //$.trim($("#selZReportRange").val());
                            pData.ZDate = dateFrom || ''; //strDate
                        } else {
                            //Sept 06, 2017
                            pData.ZDate = SalesMenu_offline().GetZReadDay();
                        }

                        if (true) {
                            var vDataPrinterType = SalesPayment_offline().getOrgFOWSTPrinterType();

                            if (vDataPrinterType.Rows.length > 0) {
                                if (vDataPrinterType.Rows[0].PrinterType == 'Epson TM-U220B Printer') {
                                    //PrintPOSReceiptDotMatrix(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg);
                                } else if (vDataPrinterType.Rows[0].PrinterType == '80MM Mini Thermal Printer') {

                                } else if (vDataPrinterType.Rows[0].PrinterType == 'Epson G Printer') {
                                    sessionStorage.Printer = 'GPrinter';
                                    isPrintInWPS = false;
                                } else if (vDataPrinterType.Rows[0].PrinterType == 'Epson G Printer') {
                                    sessionStorage.Printer = 'GPrinter';
                                    isPrintInWPS = false;
                                } else if (vDataPrinterType.Rows[0].PrinterType == 'Sunmi 58MM Printer') {
                                    sessionStorage.Printer = 'SunmiPrinter';
                                    isPrintInWPS = false;
                                } else if (vDataPrinterType.Rows[0].PrinterType == 'IMin IMin 58MM Printer') {

                                    try {
                                        window.pPrint.InitializeBluetoothPrinter();
                                    } catch (e) {
                                        console.log('InitializeBluetoothPrinter: ' + e);
                                    };

                                    sessionStorage.Printer = 'IMin';

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

                        if (rangeType == 'Today' || dateFrom == undefined) {
                            dateFrom = new Date();
                        } else if (rangeType == 'Yesterday') {
                            dateFrom = new Date();
                            dateFrom.setDate(dateFrom.getDate() - 1);
                        }

                        vDataHeader = SalesReport_offline().getReportHeader(1);
                        //SalesPayment_offline().getReceiptHeaderInfo('');

                        //Get Invoice Header

                        //Display Header
                        DisplayHeader(vDataHeader, vIsView);

                        //pSql = 'Select * from CacheZRead where date(SalesDate) <= ' + ReadyString($.format.date(dateFrom, "yyyy-MM-dd")) + ' OR SalesDate <= ' + ReadyString($.format.date(dateFrom, "MM/dd/yy")) +
                        //        ' Order by SalesDate desc limit 1';

                        //" WHERE date(SalesDate) <= " + ReadyString($.format.date(dateFrom, 'yyyy-MM-dd')) + change 2/19/2021, added getting zreadcahe while changing database

                        pSql = "Select * FROM CacheZRead " +
                                " WHERE SalesDate <= " + ReadyString($.format.date(new Date(dateFrom), 'yyyy-MM-dd')) +
                                " AND FOWSTID = " + ReadyString(sessionStorage.FowstID) +
                                " AND DeviceID = " + ReadyString(Web.DeviceID()) +
                                " ORDER BY SalesDate DESC" +
                                " LIMIT 1"

                        pDT = Sql.Execute(pSql);

                        if (pDT.Rows.length == 0) {
                            //UpdateZReadCache(dateFrom, Web.DeviceID(), sessionStorage.FowstID)
                            //PrintSalesZReport(vIsView);
                            $('#divZReportNoData').show();
                            $('#divZReportView').hide();
                            return;
                        }

                        pSql = "SELECT IFNULL(SUM(inv.SCDiscount), 0) as Total " +
                            " FROM TransSalesReturnLedger as ledger " +
                            " INNER JOIN TransMemo as memo ON memo.ID = ledger.SourceMemoID " +
                            " INNER JOIN TransCustomerPaymentInvoices as cuspayinv ON cuspayinv.InvoiceID = ledger.SourceInvoiceID " +
                            " INNER JOIN TransCustomerPayment custpay ON cuspayinv.CustomerPaymentID = custpay.ID " +
                            " INNER JOIN TransInvoice inv ON (cuspayinv.InvoiceID = inv.ID) " +
                            " WHERE ledger.FowstID = " + ReadyString(sessionStorage.FowstID) +
                            " AND ledger.DeviceID = " + ReadyString(Web.DeviceID()) +
                            " AND date(ledger.LastUpdated) = " + ReadyString($.format.date(new Date(dateFrom), 'yyyy-MM-dd')) +
                            " AND IFNULL(RefundNum, 0) > 0 "

                        pDT.Rows[0].RefundedSCDiscount = CDec2(Sql.ExecuteScalar(pSql));

                        pDT.ReportPOS = pDT;

                        dateFrom = CStr2(pDT.ReportPOS.Rows[0].SalesDate);

                        DisplaySalesZReportSummary(vDataHeader, pDT, vIsView);

                        discountCols = [];
                        discountlist = Sql.Execute('SELECT * FROM LibraryDiscountSettings').Rows;
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
                                name = 'Total' + discountlist[i].DiscountType.replaceAll(' ', '');
                            }

                            discountCols.push(name);
                            discountlist[i].ColumnName = name;
                            discountlist[i].DiscountTypeID = discountlist[i].ID;
                        }

                        LastTotalDataMSR = Sql.Execute('SELECT * FROM cache.CacheMSR WHERE OrgID = ' + ReadyString(sessionStorage.OrgID) + ' \n' +
                            ' AND FowstID = ' + ReadyString(sessionStorage.FowstID) + ' \n' +
                            ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' \n' +
                            ' AND SalesDate = ' + ReadyString($.format.date(new Date(dateFrom), 'yyyy-MM-dd'))).Rows

                        if (LastTotalDataMSR.length > 0) {
                            var LastTotal = LastTotalDataMSR[0];
                            var discbreakdown = [];
                            for (var i in discountlist) {
                                if (CDec2(LastTotal[discountlist[i].ColumnName]) > 0) {
                                    discountlist[i].DiscountAmount = CDec2(LastTotal[discountlist[i].ColumnName]);

                                    if (discountlist[i].ColumnName == 'TotalSCDiscount') {
                                        discountlist[i].DiscountAmount = CDec2(discountlist[i].DiscountAmount) - CDec2(pDT.Rows[0].RefundedSCDiscount);
                                    }

                                    discbreakdown.push(discountlist[i]);
                                }                                
                            }

                            DisplayDiscountsBreakdown(discbreakdown);
                        }

                        //var pSql = 'Select * from CacheZRead where FOWSTID = ' + ReadyString(sessionStorage.SelectedFOWST);
                        //var pDTZRead = Sql.Execute(pSql);

                        //var mData = { ReportPOS: pDTZRead }

                        //DisplaySalesZReportSummary(vDataHeader, mData, vIsView);

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

                                //pData.RangeType = rangeType || 'Today'; //$.trim($("#selZReportRange").val());
                                pData.RangeType = rangeType || 'By Date'; //$.trim($("#selZReportRange").val());

                                if (IsMobile) {
                                    if (!Business.SystemDefaults.GetEnforceZRead()) {
                                        pData.RangeType = rangeType || 'Today'; //$.trim($("#selZReportRange").val());
                                        pData.ZDate = dateFrom || ''; //strDate
                                    } else {
                                        //Sept 06, 2017
                                        pData.ZDate = SalesMenu_offline().GetZReadDay();
                                    }
                                }

                                if (IsMobile) {
                                    pData.DeviceID = Web.DeviceID();
                                    var vDataPrinterType = SalesPayment_offline().getOrgFOWSTPrinterType();

                                    if (vDataPrinterType.Rows.length > 0) {
                                        if (vDataPrinterType.Rows[0].PrinterType == 'Epson TM-U220B Printer') {
                                            //PrintPOSReceiptDotMatrix(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg);
                                        } else if (vDataPrinterType.Rows[0].PrinterType == '80MM Mini Thermal Printer') {

                                        } else if (vDataPrinterType.Rows[0].PrinterType == 'Windows Print Server') {
                                            isPrintInWPS = true;
                                        } else if (vDataPrinterType.Rows[0].PrinterType == 'Sunmi 58MM Printer') {
                                            sessionStorage.Printer = 'SunmiPrinter';
                                            intFAFCTotalLenght = 47;
                                        } else if (vDataPrinterType.Rows[0].PrinterType == 'IMin IMin 58MM Printer') {

                                            try {
                                                window.pPrint.InitializeBluetoothPrinter();
                                            } catch (e) {
                                                console.log('InitializeBluetoothPrinter: ' + e);
                                            };

                                            sessionStorage.Printer = 'IMin';
                                            intFAFCTotalLenght = 47;
                                        } else {
                                            isPrintInWPS = false;
                                        }
                                    } else {
                                        isPrintInWPS = false;
                                    }

                                    if (vDataPrinterType.Rows.length > 0) {
                                        strPrinterIP = (vDataPrinterType.Rows[0].PrinterIPAddress || '');
                                    }

                                } else if (CBool(sessionStorage.IsWebPos)) {
                                    pData.ZDate = dateFrom;
                                }
                                else {
                                    isPrintInWPS = false;
                                }
                                //pData.ZDate = '4/11/2023';
                                pData.DeviceID = '808ecdfd-86a6-4e77-acc3-1aad0955a179'

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
                                                dateFrom = CStr2(vDataInfo.ReportPOS.Rows[0].SalesDate);
                                                DisplaySalesZReportSummary(vDataHeader, vDataInfo, vIsView);
                                                var pLV = $('#tblOtherPaymentType tbody');
                                                var totalPotatoCornerGC = 0; //PC GC
                                                pLV.empty();

                                                for (var i in vDataInfo.OtherPaymentType.Rows) {
                                                    var opname = vDataInfo.OtherPaymentType.Rows[i].PaymentType;

                                                    if (opname.toLowerCase() == 'potatocornergc') {
                                                        //exclude anf GC ni Potato Corner and iadd sya sa original GC;
                                                        totalPotatoCornerGC += CDec2(vDataInfo.OtherPaymentType.Rows[i].Amount);
                                                        continue;
                                                    }
                                                    pLV.append(CreateOtherPaymentTypeRow(vDataInfo.OtherPaymentType.Rows[i].PaymentType, vDataInfo.OtherPaymentType.Rows[i].Amount))
                                                }

                                                //iadd nato ang PC GC sa original GC
                                                $('#lblZReadCheck').text(AccountingFormat(CDec2($('#lblZReadCheck').text()) + totalPotatoCornerGC));

                                                DisplayDiscountsBreakdown(vDataInfo.Discounts.Rows);
                                            }
                                        });

                                    }
                                });


                            }
                        });
                    }

                    //SalesMenu_offline()



                    //SalesMenu_offline().HandleEnforceZRead({
                    //    callback: function (e) {
                    //        if (e) {

                    //        }
                    //    }
                    //});
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

                $('#btnPOSSalesZReportPrintTemp').click(function (e) {

                    e.preventDefault();

                    PrintDesktopSalesZReportByMINTemp();

                });

                $('body').on('click', '#btnPOSSalesZReportView', function (e) {
                    e.preventDefault();
                    ViewZReport();

                })
                .on('click', '#btnPOSSalesZReportTemp', function (e) {
                    $('#panEnterZReadAmount').popup('open');
                })
                .on('click', '#btnDoneZRead', function (e) {
                    PrintSalesZReportFromDUCERP($('#txtGrandTotalAmount').val());
                })
                .on('click', '#btnPOSSalesZReportPrint', function (e) {
                    e.preventDefault();

                    //PrintSalesZReport(true); //true = view z-read on display

                    if (window.InAndroid) {
                        alert('go')
                        NewMessageBox('Printing this ZRead report will close the current ZRead Date <br/> (more specifically, increment the current ZRead date by +1 day). ' +
                                        '<br/> <br/> Are you sure you want to continue and close the current ZRead date?' +
                                        '<br/> <br/> Note: If your system date settings (in the system administration module) is not set to enforce the ZRead Date, you can still print this ZRead Report but doing so will have no effect on your system date.', '', [
                                        {
                                            Text: 'Yes',
                                                Click: function () {
                                                SalesMenu_offline().CheckIfInsideTheMall(dateFrom);
                                                PrintSalesZReport(false);                                                
                                            }
                                        },
                                        {
                                            Text: 'No'
                                        }
                                        ]);
                    } else {
                        if (true)
                            PrintSalesZReport(false);
                        else if (localStorage.UUID)
                            PrintMobizControllerSalesZReport();
                        else
                            PrintDesktopSalesZReport();
                    }

                    //UpdateZReportPrintCount();

                })
                .on('click', '#btnPOSSalesZReportEOD', function (e) {
                    e.preventDefault();

                    if (InAndroid) {
                        NewMessageBox('This will close the current ZRead Date <br/> (more specifically, increment the current ZRead date by +1 day). ' +
                                    '<br/> <br/> Are you sure you want to continue and close the current ZRead date?' +
                                    '<br/> <br/> Note: If your system date settings (in the system administration module) is not set to enforce the ZRead Date, you can still print this ZRead Report but doing so will have no effect on your system date.', '', [
                                    {
                                        Text: 'Yes',
                                        Click: function () {
                                            if (Business.SystemDefaults.GetEnforceZRead()) {                                                
                                                SalesMenu_offline().IncrementZReadDay();
                                                PrintSalesZReport(true);
                                            }
                                        }
                                    },
                                    {
                                        Text: 'No'
                                    }
                                    ]);
                    }
                    //CloseForSMCOIN();
                })
                .on('click', '#btnPOSSalesEJournal', function (e) {
                    e.preventDefault();

                    if ($("#selZReportRange").val() == "") {
                        MessageBox("Please select report range.",
                                "Report Range",
                                [
                                    {
                                        Text: 'Ok'
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
                                        {
                                            Text: 'Ok'
                                        }
                                    ]
                            );
                            return;
                        }

                    }

                    strDate = $("#selZReportByDateMonth").val() + "/" + $("#selZReportByDateDay").val() + "/" + $("#selZReportByDateYear").val();

                    DownloadEJournal();

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
                            //WithSearchbox: false, // commented by arvin 2022_01_05__11_07_09_2328
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
                                ViewZReport();
                                $('#btnZReportDateRange').text('Yesterday');
                            } else if (e.Period == 'Today') {
                                dateYear = "This year.";
                                rangeType = "Today";
                                var date = $.format.date(new Date(e.From), 'MM d yyyy');
                                dateFrom = date;
                                dateTo = date;
                                ViewZReport();
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
                    var pData = {};

                    pData.RangeType = rangeType || 'Today'; //$.trim($("#selZReportRange").val());
                    pData.ZDate = dateFrom || ''; //strDate
                    pData.DeviceID = Web.DeviceID();

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

                        //to be removed
                        //$('#panZReportFowst').popup('open');

                        //for (var i = 0; i <= vRows.Rows.length - 1; i++) {
                        //    pItem = $('<li>')
                        //                .append($('<a>')
                        //                .attr('ID', vRows.Rows[i].ID)
                        //                 .attr('IsFOWSTInsideAMall', vRows.Rows[i].IsFOWSTInsideAMall)
                        //                  .attr('FOWSTMallName', vRows.Rows[i].FOWSTMallName)
                        //                    .click(function () {
                        //                        if ($(this).attr('IsFOWSTInsideAMall') == '1' && $(this).attr('FOWSTMallName') == 'SM Mall') {
                        //                            IsFowstInSMMall = true;
                        //                        } else {
                        //                            IsFowstInSMMall = false;
                        //                        }
                        //                        sessionStorage.FowstID = $(this).attr('id');
                        //                        sessionStorage.SelectedFOWST = $(this).attr('id');
                        //                        $('.lblZReportSelectedFowstGroup').text($(this).text());
                        //                        $('#panZReportFowst').popup('close');
                        //                        $('.btnZReportSelectFowst').removeClass('btnRequired').addClass('btnNotRequired');
                        //                        ViewZReport();
                        //                    })
                        //                    .append($('<h3>')
                        //                        .text(vRows.Rows[i].FowstName)
                        //                    )
                        //                )
                        //    pLV.append(pItem);
                        //}
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

                function CreateZRead(mDate, mFOWST) {
                    //new data

                    pSql = "Select * FROM CacheZRead " +
                    " WHERE date(SalesDate) < " + ReadyString($.format.date(mDate, 'yyyy-MM-dd')) +
                    " AND FOWSTID = " + ReadyString(mFOWST) +
                    " AND DeviceID = " + ReadyString(Web.DeviceID()) +
                    " Order by SalesDate desc limit 1"

                    dtPrevZRead = Sql.Execute(pSql);

                    var pIU = new InsertUpdate('CacheZRead');

                    pIU.Add('ID', ReadyString(NewID()));
                    pIU.Add('DeviceID', ReadyString(Web.DeviceID()));
                    pIU.Add('FOWSTID', ReadyString(mFOWST));

                    if (dtPrevZRead.Rows.length > 0) {
                        if (CDec2(dtPrevZRead.Rows[0].PresentationPrevGrossSales) == 0) {
                            pIU.Add('PresentationPrevGrossSales', CDec2(dtPrevZRead.Rows[0].PrevGrossSales) + CDec2(dtPrevZRead.Rows[0].CurGrossSales))
                        } else {
                            pIU.Add('PresentationPrevGrossSales', CDec2(dtPrevZRead.Rows[0].PresentationPrevGrossSales) + CDec2(dtPrevZRead.Rows[0].CurGrossSales))
                        }

                        pIU.Add('PrevGrossSales', CDec2(dtPrevZRead.Rows[0].PrevGrossSales) + CDec2(dtPrevZRead.Rows[0].CurGrossSales))
                        pIU.Add('PrevTotalDiscount', CDec2(dtPrevZRead.Rows[0].PrevTotalDiscount) + CDec2(dtPrevZRead.Rows[0].CurTotalDiscount))
                        pIU.Add('PrevSalesReturnRefund', CDec2(dtPrevZRead.Rows[0].PrevSalesReturnRefund) + CDec2(dtPrevZRead.Rows[0].CurSalesReturnRefund))
                        pIU.Add('PrevSalesReturnExchange', CDec2(dtPrevZRead.Rows[0].PrevSalesReturnExchange) + CDec2(dtPrevZRead.Rows[0].CurSalesReturnExchange))
                        pIU.Add('PrevTotalVAT', CDec2(dtPrevZRead.Rows[0].PrevTotalVAT) + CDec2(dtPrevZRead.Rows[0].CurTotalVAT))
                        pIU.Add('PrevServiceCharge', CDec2(dtPrevZRead.Rows[0].PrevServiceCharge) + CDec2(dtPrevZRead.Rows[0].CurServiceCharge))
                        pIU.Add('PrevChargeAmount', CDec2(dtPrevZRead.Rows[0].PrevChargeAmount) + CDec2(dtPrevZRead.Rows[0].ChargeAmount))

                    } else {
                        //new data

                        pIU.Add('PresentationPrevGrossSales', 0)
                        pIU.Add('PrevGrossSales', 0);
                        pIU.Add('PrevTotalDiscount', 0);
                        pIU.Add('PrevSalesReturnRefund', 0);
                        pIU.Add('PrevSalesReturnExchange', 0);
                        pIU.Add('PrevTotalVAT', 0);
                        pIU.Add('PrevServiceCharge', 0);
                        pIU.Add('PrevChargeAmount', 0);
                    }

                    pIU.Add('SalesDate', ReadyString($.format.date(mDate, 'yyyy-MM-dd')));
                    pIU.Add('PresentationCurrGrossSales', 0);
                    pIU.Add('CurGrossSales', 0);
                    pIU.Add('CurTotalDiscount', 0);
                    pIU.Add('CurSCDiscount', 0);
                    pIU.Add('CurTotalVATSales', 0);
                    pIU.Add('CurTotalVAT', 0);
                    pIU.Add('CurVATExempt', 0);
                    pIU.Add('CurLocalTax', 0);
                    pIU.Add('CurServiceCharge', 0);
                    pIU.Add('ChargeAmount', 0)
                    pIU.Add('CurNoTAXSales', 0);
                    pIU.Add('CurVATExemptSales', 0);
                    pIU.Add('CurZeroRatedVATSales', 0);
                    pIU.Add('CurOthers', 0);
                    pIU.Add('CashAmount', 0);
                    pIU.Add('DebitCardAmount', 0);
                    pIU.Add('CreditCardAmount', 0);
                    pIU.Add('SMACAmount', 0);
                    pIU.Add('CheckPaymentAmount', 0);
                    pIU.Add('OtherTypeAmount', 0);
                    pIU.Add('OnCreditAmount', 0);
                    pIU.Add('EPlusAmount', 0);
                    pIU.Add('VoucherAmount', 0);
                    pIU.Add('CurBegInvoiceNo', CInt2(dtPrevZRead.Rows[0].CurEndInvoiceNo));
                    pIU.Add('CurEndInvoiceNo', CInt2(dtPrevZRead.Rows[0].CurEndInvoiceNo));

                    pIU.Add('CurTransInvItemCount', 0);
                    pIU.Add('CurTransCountMemoReturnExchange', 0);
                    pIU.Add('CurSalesReturnExchangeItemCount', 0);
                    pIU.Add('CurTransCountMemoRefund', 0);
                    pIU.Add('CurSalesRefundItemCount', 0);
                    pIU.Add('CurTransCountInv', 0);
                    pIU.Add('CurTransInvItemCount', 0);

                    pSql = pIU.GetInsert();
                    pSql = Sql.Execute(pSql);
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

                        } else if (sessionStorage.Printer == 'IMin') {

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


                            strTemp = $.trim((vDataHeader.Rows[0].OrgCompleteAddress || '')) + '\n';

                            for (var i = 0; i < strTemp.length; i += 28) {
                                strPrint = '';
                                strPrint = strTemp.substring(i, ((i + 28) < strTemp.length ? (i + 28) : strTemp.length));

                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                            }

                            //strPrint = '';
                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, vDataHeader.Rows[0].OrgCompleteAddress, intFAFCTotalLenght, '', 0, '', 0, 1);

                            //strTemp = $.trim((vDataHeader.Rows[0].OrgCompleteAddress || ''));
                            //strPrint = strTemp;
                            //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                            strPrint = '';
                            strTemp = 'VAT TIN ' + $.trim((vDataHeader.Rows[0].OrgTIN || ''));
                            strPrint = strTemp;
                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);                                                      

                            strPrint = '';
                            strTemp = 'Tel. No.' + $.trim((vDataHeader.Rows[0].OrgTelNo || ''));
                            strPrint = strTemp;
                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                            IminfeedPaper();

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

                            if (CBool(sessionStorage.PrintingOfflineData)) {
                                $('#lblIsOffline').text("(OFLZ)")
                            }

                        } else {
                            if (isPrintInWPS) {
                                WindowsPrinter('Date : ' + $.format.date(new Date(vDataInfo.ReportPOS.Rows[0].SalesDate), 'MM/dd/yyyy'), [1, 0, 0, 1, 0, 36]);
                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                            } else {
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Date : ' + $.format.date(new Date(vDataInfo.ReportPOS.Rows[0].SalesDate), 'MM/dd/yyyy'), intFAFCTotalLenght, '', 0, '', 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                if (sessionStorage.Printer == 'IMin') IminfeedPaper();
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

                        var PrevInvoiceCount = 0;
                        var BeginningInvoiceCount = 0;
                        var EndingInvoiceCount = 0;
                        var pValues = [];
                        var vDataOtherPaymentType;
                        var mOtherPaymentTypeColumns;
                        var dblRefundedSCDiscount = 0;

                        for (var p = 0; p <= vDataInfo.ReportPOS.Rows.length - 1; p++) {
                            dblRefundedSCDiscount = dblRefundedSCDiscount + CDec2(vDataInfo.ReportPOS.Rows[0].RefundedSCDiscount);

                            //dblTotalSales = dblTotalSales + parseFloat(vDataInfo.ReportPOS.Rows[p].CurGrossSales || 0);
                            dblTotalSales = dblTotalSales + parseFloat(vDataInfo.ReportPOS.Rows[p].PresentationCurrGrossSales || 0) - dblRefundedSCDiscount;
                            dblTotalSalesDiscount = dblTotalSalesDiscount + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalDiscount || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].CurSCDiscount || 0) - dblRefundedSCDiscount;
                            dblTotalVATExempt = dblTotalVATExempt + parseFloat(vDataInfo.ReportPOS.Rows[p].CurVATExempt || 0);
                            dblAdjRefund = dblAdjRefund + parseFloat(vDataInfo.ReportPOS.Rows[p].CurSalesReturnRefund || 0);
                            dblAdjSRV = dblAdjSRV + parseFloat(vDataInfo.ReportPOS.Rows[p].CurSalesReturnExchange || 0);

                            dblCash = dblCash + parseFloat(vDataInfo.ReportPOS.Rows[p].CashAmount || 0) //- parseFloat(vDataInfo.ReportPOS.Rows[p].CurSalesReturnRefund || 0);
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

                            //dblDailySale = dblDailySale + parseFloat(vDataInfo.ReportPOS.Rows[p].CurGrossSales || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].CurVATExempt || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].CurSCDiscount || 0) - (vDataInfo.ReportPOS.Rows[p].CurTotalDiscount || 0) - (vDataInfo.ReportPOS.Rows[p].CurSalesReturnRefund || 0) - (vDataInfo.ReportPOS.Rows[p].CurSalesReturnExchange || 0) - (vDataInfo.ReportPOS.Rows[p].CurServiceCharge || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVatAmountPortionOfSCDTransaction || 0) - (vDataInfo.ReportPOS.Rows[p].CurTotalVAT || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].ChargeAmount || 0);
                            dblDailySale = dblDailySale + parseFloat(vDataInfo.ReportPOS.Rows[p].PresentationCurrGrossSales || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].CurVATExempt || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].CurSCDiscount || 0) - (vDataInfo.ReportPOS.Rows[p].CurTotalDiscount || 0) - (vDataInfo.ReportPOS.Rows[p].CurSalesReturnRefund || 0) - (vDataInfo.ReportPOS.Rows[p].CurSalesReturnExchange || 0) - (vDataInfo.ReportPOS.Rows[p].CurServiceCharge || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVatAmountPortionOfSCDTransaction || 0) - (vDataInfo.ReportPOS.Rows[p].CurTotalVAT || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].ChargeAmount || 0);
                            dblSalesVAT = dblSalesVAT + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVatAmountPortionOfSCDTransaction || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].CurTotalVAT || 0);
                            //dblOldGT = dblOldGT + parseFloat(vDataInfo.ReportPOS.Rows[p].BegBal || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].PrevGrossSales || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].PrevTotalDiscount || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].PrevSalesReturnRefund || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].PrevSalesReturnExchange || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].PrevServiceCharge || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].PrevChargeAmount || 0);

                            zReadCount += CInt2(vDataInfo.ReportPOS.Rows[p].ZCount);

                            PrevInvoiceCount = CInt2(vDataInfo.ReportPOS.Rows[p].CurBegInvoiceNo) - 1;
                            BeginningInvoiceCount = CInt2(vDataInfo.ReportPOS.Rows[p].CurBegInvoiceNo);
                            EndingInvoiceCount = CInt2(vDataInfo.ReportPOS.Rows[p].CurEndInvoiceNo);

                            if (CBool(vDataHeader.Rows[0].IsAuthorizedForPOS)) {
                                dblOldGT = dblOldGT + parseFloat(vDataInfo.ReportPOS.Rows[p].BegBal || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].PresentationPrevGrossSales || 0) - (parseFloat(vDataInfo.ReportPOS.Rows[p].PrevTotalDiscount || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].PrevSCDiscount || 0)) - parseFloat(vDataInfo.ReportPOS.Rows[p].PrevSalesReturnRefund || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].PrevSalesReturnExchange || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].PrevServiceCharge || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].PrevVATExempt || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].PrevChargeAmount || 0);
                            } else {
                                
                                //if not registered to bir, set old gt to zero
                                
                                dblOldGT = 0
                                PrevInvoiceCount = CInt2(0) ;
                                BeginningInvoiceCount = CInt2(1);
                                EndingInvoiceCount = CInt2(intTransCount);
                            }
                        };

                        dblNewGT = dblOldGT + dblDailySale + dblSalesVAT;
                        dblNetSales = dblTotalSales - dblTotalSalesDiscount - dblTotalVATExempt - dblAdjRefund - dblAdjSRV + dblCharges + dblServiceCharge;
                        dblTotalPayment = dblCash + dblCheck + dblDebitCard + dblCreditCard + dblSRV + dblOtherPaymentType + dblOnCredit;
                        dblTotalVAT = dblVAT + dblVATAmount + dblNonVAT + dblVATExempt + dblZeroRatedVAT;

                        pValues.push({
                            Key: 'Refunded SC Discount',
                            DataType: 'numeric',
                            Value: dblRefundedSCDiscount
                        });

                        pValues.push({
                            Key: 'Sales Date',
                            DataType: 'date',
                            Value: vDataInfo.ReportPOS.Rows[0].SalesDate
                        });

                        pValues.push({
                            Key: 'Gross Sales',
                            DataType: 'numeric',
                            Value: dblTotalSales
                        });

                        pValues.push({
                            Key: 'Sales Discount',
                            DataType: 'numeric',
                            Value: dblTotalSalesDiscount
                        });

                        pValues.push({
                            Key: 'Vat Exempt Tax',
                            DataType: 'numeric',
                            Value: dblTotalVATExempt
                        });

                        pValues.push({
                            Key: 'Service Charge',
                            DataType: 'numeric',
                            Value: dblServiceCharge
                        });

                        pValues.push({
                            Key: 'Local Tax',
                            DataType: 'numeric',
                            Value: dblLocalTax
                        });       

                        pValues.push({
                            Key: 'Total Sales Refund for the Period',
                            DataType: 'numeric',
                            Value: dblAdjRefund
                        });

                        pValues.push({
                            Key: 'Total Sales Returns Subject to Merchadise Exchange',
                            DataType: 'numeric',
                            Value: dblAdjSRV
                        });

                        pValues.push({
                            Key: 'Net of Adjustments',
                            DataType: 'numeric',
                            Value: parseFloat(dblAdjRefund) + parseFloat(dblAdjSRV)
                        });

                        pValues.push({
                            Key: 'Net Sales',
                            DataType: 'numeric',
                            Value: dblNetSales
                        });

                        pValues.push({
                            Key: 'Cash',
                            DataType: 'numeric',
                            Value: dblCash
                        });

                        pValues.push({
                            Key: 'Check',
                            DataType: 'numeric',
                            Value: dblCheck
                        });

                        pValues.push({
                            Key: 'Debit Card',
                            DataType: 'numeric',
                            Value: dblDebitCard
                        });

                        pValues.push({
                            Key: 'Credit Card',
                            DataType: 'numeric',
                            Value: dblCreditCard
                        });

                        pValues.push({
                            Key: 'SRV',
                            DataType: 'numeric',
                            Value: dblSRV
                        });

                        pValues.push({
                            Key: 'SMAC',
                            DataType: 'numeric',
                            Value: dblSMAC
                        });

                        pValues.push({
                            Key: 'E-Plus',
                            DataType: 'numeric',
                            Value: dblEPlus
                        });

                        pValues.push({
                            Key: 'Voucher',
                            DataType: 'numeric',
                            Value: dblVoucher
                        });

                        pValues.push({
                            Key: 'Other Tender',
                            DataType: 'numeric',
                            Value: dblOtherPaymentType
                        });

                        pValues.push({
                            Key: 'On Credit',
                            DataType: 'numeric',
                            Value: dblOnCredit
                        });

                        var dblTotalVATSalesPercent = (dblVAT / dblTotalVAT) * 100;
                        var dblTotalVATAmountPercent = (dblVATAmount / dblTotalVAT) * 100;
                        var dblTotalNonVATPercent = (dblNonVAT / dblTotalVAT) * 100;
                        var dblTotalVATExemptPercent = (dblVATExempt / dblTotalVAT) * 100;
                        var dblTotalZeroRatedVATPercent = (dblZeroRatedVAT / dblTotalVAT) * 100;
                        var dblTotalVATPercent = dblTotalVATSalesPercent + dblTotalVATAmountPercent + dblTotalNonVATPercent + dblTotalVATExemptPercent + dblTotalZeroRatedVATPercent;

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

                        pValues.push({
                            Key: 'Total Trans. Count',
                            DataType: 'numeric',
                            Value: intTransCount
                        });

                        pValues.push({
                            Key: 'Sales Trans. Count',
                            DataType: 'numeric',
                            Value: intSalesTransCount
                        });

                        pValues.push({
                            Key: 'Sales Line Item Count',
                            DataType: 'numeric',
                            Value: intSalesItemCount
                        });

                        pValues.push({
                            Key: 'SRV Trans. Count',
                            DataType: 'numeric',
                            Value: intSRVTransCount
                        });

                        pValues.push({
                            Key: 'SRV Line Item Count',
                            DataType: 'numeric',
                            Value: intSRVItemCount
                        });

                        pValues.push({
                            Key: 'Refund Trans. Count',
                            DataType: 'numeric',
                            Value: intRefTransCount
                        });

                        pValues.push({
                            Key: 'Refund Line Item Count',
                            DataType: 'numeric',
                            Value: intRefItemCount
                        });

                        pValues.push({
                            Key: 'Customer Count',
                            DataType: 'numeric',
                            Value: intSalesTransCount
                        });

                        pValues.push({
                            Key: 'Prev. Day EOD Invoice Count',
                            DataType: 'numeric',
                            Value: PrevInvoiceCount
                        });

                        pValues.push({
                            Key: 'Curr. Day Starting Invoice Count',
                            DataType: 'numeric',
                            Value: BeginningInvoiceCount
                        });

                        pValues.push({
                            Key: 'Curr. Day EOD Invoice Count',
                            DataType: 'numeric',
                            Value: EndingInvoiceCount
                        });

                        pValues.push({
                            Key: 'Old Grand Total',
                            DataType: 'numeric',
                            Value: dblOldGT
                        });

                        pValues.push({
                            Key: 'New Grand Total',
                            DataType: 'numeric',
                            Value: dblNewGT
                        });

                        pValues.push({
                            Key: 'Z-READ Print Count',
                            DataType: 'numeric',
                            Value: zReadCount
                        });

                        try {
                            //pValues.push({
                            //    Key: 'Printed By',
                            //    DataType: 'string',
                            //    Value: 'Juan Reyes'//vDataHeader.ReportPOS.Rows[0].PrintedBy
                            //});

                            //pValues.push({
                            //    Key: 'Date and Time',
                            //    DataType: 'date',
                            //    Value: vDataHeader.ReportPOS.Rows[0].PrintDateTime
                            //});
                        } catch (e) {
                            console.log(e);
                        }
                        
                        if (CStr2(localStorage.getItem('UUID')) == '' && IsMobile) {

                            pSql = "Select * FROM cache.CacheMSR " +
                                " WHERE SalesDate = " + ReadyString($.format.date((new Date(dateFrom || SalesMenu_offline().GetZReadDay())), 'yyyy-MM-dd')) +
                                " AND FOWSTID = " + ReadyString(sessionStorage.FowstID) +
                                " AND DeviceID = " + ReadyString(Web.DeviceID()) +
                                " ORDER BY SalesDate DESC" +
                                " LIMIT 1";

                            vDataOtherPaymentType = Sql.Execute(pSql);
                        }

                        if (vIsView) {
                            //Date Sales Summary
                            $("#lblZReadGrossSales").text(AccountingFormat(dblTotalSales));
                            $("#lblZReadSalesDiscount").text(AccountingFormat(dblTotalSalesDiscount));
                            $("#lblZReadVatExemptTAX").text(AccountingFormat(dblTotalVATExempt));
                            $("#lblZReadTotalSalesRefund").text(AccountingFormat(dblAdjRefund));
                            $("#lblZReadTotalSRV").text(AccountingFormat(dblAdjSRV));
                            $("#lblZReadNetOfAdjustment").text(AccountingFormat(parseFloat(dblAdjRefund) + parseFloat(dblAdjSRV)));
                            $("#lblZReadNetSales").text(AccountingFormat(dblNetSales));
                            //End Date Sales Summary 

                            //Sales Grouped by Mode of Payment----------------------------
                            $("#lblZReadCash").text(AccountingFormat(dblCash));
                            $("#lblZReadCheck").text(AccountingFormat(dblCheck));
                            $("#lblZReadDebitCard").text(AccountingFormat(dblDebitCard));
                            $("#lblZReadCreditCard").text(AccountingFormat(dblCreditCard));
                            $("#lblZReadSRV").text(AccountingFormat(dblSRV));
                            $("#lblZReadOnCredit").text(AccountingFormat(dblOnCredit));
                            $("#lblZReadOtherPaymentType").text(AccountingFormat(dblOtherPaymentType));

                            $("#lblZReadSMAC").text(AccountingFormat(dblSMAC));
                            $("#lblZReadEPlus").text(AccountingFormat(dblEPlus));
                            $("#lblZReadVoucher").text(AccountingFormat(dblVoucher));
                            $("#lblZReadTotalInvPayment").text(AccountingFormat(dblTotalPayment));

                            
                            if (dblCharges > 0)
                                $("#divCharges").show();
                            else
                                $("#divCharges").hide();

                            $("#lblZReadCharges").text(AccountingFormat(dblCharges+dblServiceCharge));

                            //add other payment                           

                            if (vDataOtherPaymentType != undefined) {


                                if (vDataOtherPaymentType.Rows.length > 0) {

                                    mOtherPaymentTypeColumns = Object.entries(vDataOtherPaymentType.Columns)

                                    var pLV = $('#tblOtherPaymentType tbody');
                                    var totalPotatoCornerGC = 0; //PC GC
                                    pLV.empty();


                                    //i=10 since magsugod ang other payment type sa 
                                    for (var i = 28; i <= mOtherPaymentTypeColumns.length - 1; i++) {
                                        if (CDec2(vDataOtherPaymentType.Rows[0][mOtherPaymentTypeColumns[i][0]]) > 0) {
                                            //mga OTHER PAYMENT lang pwede idisplay - added by Arnie 5-10-2023
                                            var opname = mOtherPaymentTypeColumns[i][0].replace('OtherPayment', '');

                                            if (opname.toLowerCase() == 'potatocornergc') {
                                                //exclude anf GC ni Potato Corner and iadd sya sa original GC;
                                                totalPotatoCornerGC += CDec2(vDataOtherPaymentType.Rows[0][mOtherPaymentTypeColumns[i][0]]);
                                                continue;
                                            }

                                            if (CStr2(mOtherPaymentTypeColumns[i][0]).indexOf('OtherPayment') == 0) {
                                                pItem = CreateOtherPaymentTypeRow(opname, vDataOtherPaymentType.Rows[0][mOtherPaymentTypeColumns[i][0]]);
                                                pLV.append(pItem);
                                            }                                            
                                        }
                                    }

                                    //iadd nato ang PC GC sa original GC
                                    $('#lblZReadCheck').text(AccountingFormat(CDec2($('#lblZReadCheck').text()) + totalPotatoCornerGC));

                                    //if (CDec2(vDataOtherPaymentType.Rows[0].OtherPaymentAlipay) > 0) {
                                    //    pItem = CreateOtherPaymentTypeRow('AliPay', vDataOtherPaymentType.Rows[0].OtherPaymentAlipay);
                                    //    pLV.append(pItem);
                                    //}
                                    //if (CDec2(vDataOtherPaymentType.Rows[0].OtherPaymentComplimentary) > 0) {
                                    //    pItem = CreateOtherPaymentTypeRow('Complimentary', vDataOtherPaymentType.Rows[0].OtherPaymentComplimentary);
                                    //    pLV.append(pItem);
                                    //}
                                    //if (CDec2(vDataOtherPaymentType.Rows[0].OtherPaymentDiscountVoucher) > 0) {
                                    //    pItem = CreateOtherPaymentTypeRow('Discount Voucher', vDataOtherPaymentType.Rows[0].OtherPaymentDiscountVoucher);
                                    //    pLV.append(pItem);
                                    //}
                                    //if (CDec2(vDataOtherPaymentType.Rows[0].OtherPaymentEPlus) > 0) {
                                    //    pItem = CreateOtherPaymentTypeRow('EPlus', vDataOtherPaymentType.Rows[0].OtherPaymentEPlus);
                                    //    pLV.append(pItem);
                                    //}
                                    //if (CDec2(vDataOtherPaymentType.Rows[0].OtherPaymentGCash) > 0) {
                                    //    pItem = CreateOtherPaymentTypeRow('G Cash', vDataOtherPaymentType.Rows[0].OtherPaymentGCash);
                                    //    pLV.append(pItem);
                                    //}
                                    //if (CDec2(vDataOtherPaymentType.Rows[0].OtherPaymentGoldCard) > 0) {
                                    //    pItem = CreateOtherPaymentTypeRow('Gold Card', vDataOtherPaymentType.Rows[0].OtherPaymentGoldCard);
                                    //    pLV.append(pItem);
                                    //}
                                    //if (CDec2(vDataOtherPaymentType.Rows[0].OtherPaymentPayMaya) > 0) {
                                    //    pItem = CreateOtherPaymentTypeRow('PayMaya', vDataOtherPaymentType.Rows[0].OtherPaymentPayMaya);
                                    //    pLV.append(pItem);
                                    //}
                                    //if (CDec2(vDataOtherPaymentType.Rows[0].OtherPaymentSMAC) > 0) {
                                    //    pItem = CreateOtherPaymentTypeRow('SMAC', vDataOtherPaymentType.Rows[0].OtherPaymentSMAC);
                                    //    pLV.append(pItem);
                                    //}
                                    //if (CDec2(vDataOtherPaymentType.Rows[0].OtherPaymentShopeeEVoucher) > 0) {
                                    //    pItem = CreateOtherPaymentTypeRow('Shopee E-Voucher', vDataOtherPaymentType.Rows[0].OtherPaymentShopeeEVoucher);
                                    //    pLV.append(pItem);
                                    //}
                                    //if (CDec2(vDataOtherPaymentType.Rows[0].OtherPaymentShopeePay) > 0) {
                                    //    pItem = CreateOtherPaymentTypeRow('Shopee Pay', vDataOtherPaymentType.Rows[0].OtherPaymentShopeePay);
                                    //    pLV.append(pItem);
                                    //}
                                    //if (CDec2(vDataOtherPaymentType.Rows[0].OtherPaymentVPLUS) > 0) {
                                    //    pItem = CreateOtherPaymentTypeRow('VPlus', vDataOtherPaymentType.Rows[0].OtherPaymentVPLUS);
                                    //    pLV.append(pItem);
                                    //} if (CDec2(vDataOtherPaymentType.Rows[0].OtherPaymentFoodPanda) > 0) {
                                    //    pItem = CreateOtherPaymentTypeRow('Food Panda', vDataOtherPaymentType.Rows[0].OtherPaymentFoodPanda);
                                    //    pLV.append(pItem);
                                    //}
                                    //if (CDec2(vDataOtherPaymentType.Rows[0].OtherPaymentGrabFood) > 0) {
                                    //    pItem = CreateOtherPaymentTypeRow('Grab Food', vDataOtherPaymentType.Rows[0].OtherPaymentGrabFood);
                                    //    pLV.append(pItem);
                                    //}
                                    //if (CDec2(vDataOtherPaymentType.Rows[0].OtherPaymentLocalDelivery) > 0) {
                                    //    pItem = CreateOtherPaymentTypeRow('Local Delivery', vDataOtherPaymentType.Rows[0].OtherPaymentLocalDelivery);
                                    //    pLV.append(pItem);
                                    //}

                                    $('#divOtherType').css('visibility', 'visible');
                                    $('#divOtherType').css('display', 'block');

                                } else {
                                    $('#divOtherType').css('visibility', 'hidden');
                                    $('#divOtherType').css('display', 'none');
                                }
                            }
                            //End Sales Grouped by Mode of Payment----------------------------

                            //Sales Grouped by VAT TYPE----------------------------
                            var dblTotalVATSalesPercent = (dblVAT / dblTotalVAT) * 100;
                            var dblTotalVATAmountPercent = (dblVATAmount / dblTotalVAT) * 100;
                            var dblTotalNonVATPercent = (dblNonVAT / dblTotalVAT) * 100;
                            var dblTotalVATExemptPercent = (dblVATExempt / dblTotalVAT) * 100;
                            var dblTotalZeroRatedVATPercent = (dblZeroRatedVAT / dblTotalVAT) * 100;
                            var dblTotalVATPercent = dblTotalVATSalesPercent + dblTotalVATAmountPercent + dblTotalNonVATPercent + dblTotalVATExemptPercent + dblTotalZeroRatedVATPercent;

                            if (IsVAT != undefined) {
                                if (!CBool(IsVAT)) {
                                    dblTotalVATSalesPercent = 0;
                                    dblTotalVATAmountPercent = 0;
                                    dblTotalNonVATPercent = 100;

                                    dblNonVAT = CDec2(dblVAT) + CDec2(dblVATAmount);
                                    dblVAT = 0;
                                    dblVATAmount = 0;                                    
                                }
                            }

                            $("#lblZReadVATableSales").text(AccountingFormat(dblVAT));
                            $("#lblZReadVATableSalesPercent").text(AccountingFormat(dblTotalVATSalesPercent) + '%');
                            $("#lblZReadVATAmount").text(AccountingFormat(dblVATAmount));
                            $("#lblZReadVATAmountPercent").text(AccountingFormat(dblTotalVATAmountPercent) + '%');
                            $("#lblZReadNonVATAmount").text(AccountingFormat(dblNonVAT));
                            $("#lblZReadNonVATPercent").text(AccountingFormat(dblTotalNonVATPercent) + '%');
                            $("#lblZReadVATExemptAmount").text(AccountingFormat(dblVATExempt));
                            $("#lblZReadVATExemptPercent").text(AccountingFormat(dblTotalVATExemptPercent) + '%');
                            $("#lblZReadZeroRatedVATAmount").text(AccountingFormat(dblZeroRatedVAT));
                            $("#lblZReadZeroRatedVATPercent").text(AccountingFormat(dblTotalZeroRatedVATPercent) + '%');
                            $("#lblZReadTotalVATType").text(AccountingFormat(dblTotalVAT));
                            $("#lblZReadTotalVATTypePercent").text(AccountingFormat(dblTotalVATPercent) + '%');
                            //End Sales Grouped by VAT TYPE----------------------------

                            //Other Informatoin
                            $("#lblZReadTotalTransCount").text(intTransCount + '');
                            $("#lblZReadSalesTransCount").text(intSalesTransCount + '');
                            $("#lblZReadSalesLineItemCount").text(intSalesItemCount + '');
                            $("#lblZReadSRVTransCount").text(intSRVTransCount + '');
                            $("#lblZReadSRVLineItemCount").text(intSRVItemCount + '');
                            $("#lblZReadRefundTransCount").text(intRefTransCount + '');
                            $("#lblZReadRefundLineItemCount").text(intRefItemCount + '');
                            $("#lblZReadCustomerCount").text(intSalesTransCount + '');

                            $("#lblZReadPrevInvoiceCount").text(PrevInvoiceCount);
                            $("#lblZReadStartingInvoiceCount").text(BeginningInvoiceCount);
                            $("#lblZReadEndingInvoiceCount").text(EndingInvoiceCount);

                            $("#lblZReadOldGrandTotal").text(AccountingFormat(dblOldGT));
                            $("#lblZReadNewGrandTotal").text(AccountingFormat(dblNewGT));
                            $('#lblZReadZReadPrintCount').text(zReadCount);
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
                            var totalPotatoCornerGC = 0;
                            if (vDataOtherPaymentType != undefined) {
                                if (vDataOtherPaymentType.Rows.length > 0) {
                                    mOtherPaymentTypeColumns = Object.entries(vDataOtherPaymentType.Columns);
                                    //i=10 since magsugod ang other payment type sa 
                                    for (var i = 28; i <= mOtherPaymentTypeColumns.length - 1; i++) {
                                        var opname = mOtherPaymentTypeColumns[i][0].replace('OtherPayment', '');

                                        if (opname.toLowerCase() == 'potatocornergc') {
                                            //exclude anf GC ni Potato Corner and iadd sya sa original GC;
                                            totalPotatoCornerGC += CDec2(vDataOtherPaymentType.Rows[0][mOtherPaymentTypeColumns[i][0]]);
                                            continue;
                                        }
                                    }
                                }
                            }

                            if (isPrintInWPS) {
                                //Date Sales Summary

                                //WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                WindowsPrinter(" Gross Sales     : ", [1, 25, 0, 0, 0, 36], AccountingFormat(dblTotalSales), [0, 15, 0, 0, 1, 36]);

                                //var strTotalSales = SetPrefixPostfix(AccountingFormat(dblTotalSales), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Gross Sales: ', 14, strTotalSales, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Sales Discount  : ", [1, 25, 0, 0, 0, 36], AccountingFormat(dblTotalSalesDiscount), [0, 15, 0, 0, 1, 36]);

                                if (LastTotalDataMSR.length > 0) {
                                    var LastTotal = LastTotalDataMSR[0];
                                    var discbreakdown = [];
                                    for (var i in discountlist) {
                                        if (CDec2(LastTotal[discountlist[i].ColumnName]) > 0) {
                                            discountlist[i].DiscountAmount = CDec2(LastTotal[discountlist[i].ColumnName]);

                                            if (discountlist[i].ColumnName == 'TotalSCDiscount') {
                                                discountlist[i].DiscountAmount = CDec2(discountlist[i].DiscountAmount) - CDec2(dblRefundedSCDiscount);
                                            }

                                            discbreakdown.push(discountlist[i]);
                                        }
                                    }

                                    DisplayDiscountsBreakdown(discbreakdown, true);
                                }
                                //WindowsPrinter(' Sales Discount: ' + (AccountingFormat(dblTotalSalesDiscount) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strTotalSalesDiscount = SetPrefixPostfix(AccountingFormat(dblTotalSalesDiscount), intFAFCTotalLenght - 19, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Sales Discount: ', 17, strTotalSalesDiscount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Vat Exempt Tax  : ", [1, 25, 0, 0, 0, 36], AccountingFormat(dblTotalVATExempt), [0, 15, 0, 0, 1, 36]);
                                
                                WindowsPrinter(" Service Charge  : ", [1, 25, 0, 0, 0, 36], AccountingFormat(dblServiceCharge), [0, 15, 0, 0, 1, 36]);
                                
                                //WindowsPrinter(' Service Charge: ' + (AccountingFormat(dblServiceCharge) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strServiceCharge = SetPrefixPostfix(AccountingFormat(dblServiceCharge), intFAFCTotalLenght - 19, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Service Charge: ', 17, strServiceCharge, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Local Tax       : ", [1, 25, 0, 0, 0, 36],  AccountingFormat(dblLocalTax), [0, 15, 0, 0, 1, 36]);

                                //WindowsPrinter(' Local Tax: ' + (AccountingFormat(dblLocalTax) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strLocalTax = SetPrefixPostfix(AccountingFormat(dblLocalTax), intFAFCTotalLenght - 19, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Local Tax: ', 17, strLocalTax, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                WindowsPrinter(' Summary of Adjustments ', [1, 0, 0, 1, 0, 36]);
                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Summary of Adjustments', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Total Sales Refund for the Period : ", [1, 16, 0, 0, 0, 36], AccountingFormat(dblAdjRefund), [0, 15, 0, 0, 1, 36]);
                                

                                //WindowsPrinter(' Total Sales Refund for the  Period ' + (AccountingFormat(dblAdjRefund) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strAdjRefund = SetPrefixPostfix(AccountingFormat(dblAdjRefund), intFAFCTotalLenght - 29, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Sales Refund for the  Period', 27, strAdjRefund, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Total Sales Returns Subject to Merchadise Exchange (SRV Issued) : ", [1, 16, 0, 0, 0, 36], AccountingFormat(dblAdjSRV), [0, 15, 0, 0, 1, 36]);
                                

                                //WindowsPrinter(' Total Sales Returns Subject to Merchadise Exchange           (SRV Issued) ' + (AccountingFormat(dblAdjSRV) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strAdjSRV = SetPrefixPostfix(AccountingFormat(dblAdjSRV), intFAFCTotalLenght - 32, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Sales Returns Subject to Merchadise Exchange           (SRV Issued)', 30, strAdjSRV, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Net of Adjustments  : ", [1, 16, 0, 0, 0, 36], AccountingFormat(dblAdjRefund), [0, 15, 0, 0, 1, 36]);
                                //WindowsPrinter(' Net of Adjustments: ' + (AccountingFormat(dblAdjRefund) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strNetSalesAdj = SetPrefixPostfix(AccountingFormat(parseFloat(dblAdjRefund) + parseFloat(dblAdjSRV)), intFAFCTotalLenght - 23, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Net of Adjustments: ', 21, strNetSalesAdj, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                //WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                WindowsPrinter(" Net Sales       : ", [1, 25, 0, 0, 0, 36], AccountingFormat(dblNetSales), [0, 15, 0, 0, 1, 36]);
                                //WindowsPrinter(' Net Sales: ' + (AccountingFormat(dblNetSales) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strNetSales = SetPrefixPostfix(AccountingFormat(dblNetSales), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Net Sales: ', 12, '  ' + strNetSales, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //End Date Sales Summary

                                //Sales Grouped by Mode of Payment----------------------------
                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                WindowsPrinter(' Inventory of Payments ', [1, 0, 0, 1, 0, 36]);
                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                //WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, "Inventory of Payments", intFAFCTotalLenght, '', 0, '', 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                WindowsPrinter(" Cash            : ", [1, 25, 0, 0, 0, 36], AccountingFormat(dblCash), [0, 15, 0, 0, 1, 36]);
                                //WindowsPrinter(' Cash       ' + (AccountingFormat(dblCash) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strTotalCash = SetPrefixPostfix(AccountingFormat(dblCash), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Cash       ', 12, ': ' + strTotalCash, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);                                

                                WindowsPrinter(" Check           : ", [1, 25, 0, 0, 0, 36], AccountingFormat(dblCheck + totalPotatoCornerGC), [0, 20, 0, 0, 1, 36]);
                                //WindowsPrinter(' Check      ' + (AccountingFormat(dblCheck) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strTotalCheck = SetPrefixPostfix(AccountingFormat(dblCheck), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Check      ', 12, ': ' + strTotalCheck, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Debit Card      : ", [1, 25, 0, 0, 0, 36], AccountingFormat(dblDebitCard), [0, 20, 0, 0, 1, 36]);
                                //WindowsPrinter(' Debit Card ' + (AccountingFormat(dblDebitCard) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strTotalDebitCard = SetPrefixPostfix(AccountingFormat(dblDebitCard), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Debit Card ', 12, ': ' + strTotalDebitCard, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" Credit Card     : ", [1, 25, 0, 0, 0, 36], AccountingFormat(dblCreditCard), [0, 20, 0, 0, 1, 36]);
                                //WindowsPrinter(' Credit Card' + (AccountingFormat(dblCreditCard) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strTotalCreditCard = SetPrefixPostfix(AccountingFormat(dblCreditCard), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Credit Card', 12, ': ' + strTotalCreditCard, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                WindowsPrinter(" SRV             : ", [1, 25, 0, 0, 0, 36], AccountingFormat(dblSRV), [0, 20, 0, 0, 1, 36]);
                                //WindowsPrinter(' SRV        ' + (AccountingFormat(dblSRV) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strTotalSRV = SetPrefixPostfix(AccountingFormat(dblSRV), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SRV        ', 12, ': ' + strTotalSRV, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                if (IsFowstInSMMall) {
                                    WindowsPrinter(" SMAC           : ", [1, 25, 0, 0, 0, 36], AccountingFormat(dblSMAC), [0, 20, 0, 0, 1, 36]);
                                    //WindowsPrinter(' SMAC        ' + (AccountingFormat(dblSMAC) || ''), [1, 0, 0, 1, 0, 36]);
                                    //var strTotalSMAC = SetPrefixPostfix(AccountingFormat(dblSMAC), intFAFCTotalLenght - 16, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SMAC        ', 12, ': ' + strTotalSMAC, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                    WindowsPrinter(" E-Plus         : ", [1, 25, 0, 0, 0, 36], AccountingFormat(dblEPlus), [0, 20, 0, 0, 1, 36]);
                                    //WindowsPrinter(' E-Plus      ' + (AccountingFormat(dblEPlus) || ''), [1, 0, 0, 1, 0, 36]);
                                    //var strTotalEPlus = SetPrefixPostfix(AccountingFormat(dblEPlus), intFAFCTotalLenght - 16, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' E-Plus      ', 12, ': ' + strTotalEPlus, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                    WindowsPrinter(" Voucher        : ", [1, 25, 0, 0, 0, 36], AccountingFormat(dblVoucher), [0, 20, 0, 0, 1, 36]);
                                    //WindowsPrinter(' Voucher     ' + (AccountingFormat(dblVoucher) || ''), [1, 0, 0, 1, 0, 36]);
                                    //var strTotalVoucher = SetPrefixPostfix(AccountingFormat(dblVoucher), intFAFCTotalLenght - 16, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Voucher     ', 12, ': ' + strTotalVoucher, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);


                                } else {
                                    //WindowsPrinter(" Other Tender", [1, 16, 0, 0, 0, 36], ': ' + AccountingFormat(dblOtherPaymentType), [0, 20, 0, 0, 0, 36]);
                                    //WindowsPrinter(' Other Tender' + (AccountingFormat(dblOtherPaymentType) || ''), [1, 0, 0, 1, 0, 36]);
                                    //var strTotalOther = SetPrefixPostfix(AccountingFormat(dblOtherPaymentType), intFAFCTotalLenght - 16, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Other Tender', 12, ': ' + strTotalOther, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                }

                                WindowsPrinter(" On Credit       : ", [1, 25, 0, 0, 0, 36], AccountingFormat(dblOnCredit), [0, 10, 0, 0, 1, 36]);

                                if (vDataOtherPaymentType != undefined) {
                                    if (vDataOtherPaymentType.Rows.length > 0) {
                                        mOtherPaymentTypeColumns = Object.entries(vDataOtherPaymentType.Columns);
                                        //i=10 since magsugod ang other payment type sa 
                                        for (var i = 28; i <= mOtherPaymentTypeColumns.length - 1; i++) {
                                            if (CDec2(vDataOtherPaymentType.Rows[0][mOtherPaymentTypeColumns[i][0]]) > 0) {
                                                var OPAmount = vDataOtherPaymentType.Rows[0][mOtherPaymentTypeColumns[i][0]];

                                                if (mOtherPaymentTypeColumns[i][0].replace('OtherPayment', '').toLowerCase() == 'potatocornergc') continue;
                                                if (CDec2(OPAmount) > 0 && CStr2(mOtherPaymentTypeColumns[i][0]).indexOf('OtherPayment') == 0) {
                                                    WindowsPrinter(" " + mOtherPaymentTypeColumns[i][0].replace('OtherPayment', '') + "           : ", [1, 25, 0, 0, 0, 36], AccountingFormat(OPAmount), [0, 20, 0, 0, 1, 36]);
                                                }                                                
                                            }
                                        }
                                    }
                                }

                                WindowsPrinter('____________________________________', [0, 0, 0, 0, 0, 36]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                WindowsPrinter("            ", [1, 16, 0, 0, 0, 36], AccountingFormat(dblTotalPayment), [0, 15, 0, 0, 1, 36]);
                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                //WindowsPrinter('            ' + (AccountingFormat(dblTotalPayment) || ''), [1, 0, 0, 1, 0, 36]);
                                //var strTotalClassPayment = SetPrefixPostfix(AccountingFormat(dblTotalPayment), intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, '            ', 12, '  ' + strTotalClassPayment, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);


                                //End Sales Grouped by Mode of Payment----------------------------

                                //Sales Grouped by VAT TYPE----------------------------
                                //WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                WindowsPrinter('Sales Grouped by VAT Type', [1, 0, 0, 1, 0, 36]);
                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
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
                                WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, "Other Information", intFAFCTotalLenght, '', 0, '', 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                WindowsPrinter("Total Trans. Count : ", [1, 16, 0, 0, 0, 36], intTransCount, [0, 10, 0, 0, 1, 36]);
                                //WindowsPrinter('Total Trans. Count ' + intTransCount, [1, 0, 0, 1, 0, 36]);
                                //var strTransCount = SetPrefixPostfix(intTransCount + '', intFAFCTotalLenght - 20, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Total Trans. Count', 18, strTransCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter("Sales Trans. Count : ", [1, 16, 0, 0, 0, 36], intSalesTransCount, [0, 10, 0, 0, 1, 36]);
                                //WindowsPrinter('Sales Trans. Count ' + intSalesTransCount, [1, 0, 0, 1, 0, 36]);
                                //var strSalesTransCount = SetPrefixPostfix(intSalesTransCount + '', intFAFCTotalLenght - 20, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Sales Trans. Count', 18, strSalesTransCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter("Sales Line Item Count : ", [1, 16, 0, 0, 0, 36], intSalesItemCount, [0, 10, 0, 0, 1, 36]);
                                //WindowsPrinter('Sales Line Item Count ' + intSalesItemCount, [1, 0, 0, 1, 0, 36]);
                                //var strSalesItemCount = SetPrefixPostfix(intSalesItemCount + '', intFAFCTotalLenght - 23, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Sales Line Item Count', 21, strSalesItemCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter("SRV Trans. Count : ", [1, 16, 0, 0, 0, 36], intSRVTransCount, [0, 10, 0, 0, 1, 36]);
                                //WindowsPrinter('SRV Trans. Count ' + intSRVTransCount, [1, 0, 0, 1, 0, 36]);
                                //var strSRVTransCount = SetPrefixPostfix(intSRVTransCount + '', intFAFCTotalLenght - 18, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'SRV Trans. Count', 16, strSRVTransCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter("SRV Line Item Count : ", [1, 16, 0, 0, 0, 36], intSRVItemCount, [0, 10, 0, 0, 1, 36]);
                                //WindowsPrinter('SRV Line Item Count ' + intSRVItemCount, [1, 0, 0, 1, 0, 36]);
                                //var strSRVItemCount = SetPrefixPostfix(intSRVItemCount + '', intFAFCTotalLenght - 21, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'SRV Line Item Count', 19, strSRVItemCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter("Refund Trans. Count : ", [1, 16, 0, 0, 0, 36], intRefTransCount, [0, 10, 0, 0, 1, 36]);
                                //WindowsPrinter('Refund Trans. Count ' + intRefTransCount, [1, 0, 0, 1, 0, 36]);
                                //var strRefTransCount = SetPrefixPostfix(intRefTransCount + '', intFAFCTotalLenght - 21, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Refund Trans. Count', 19, strRefTransCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter("Refund Line Item Count : ", [1, 16, 0, 0, 0, 36], intRefItemCount, [0, 10, 0, 0, 1, 36]);
                                //WindowsPrinter('Refund Line Item Count ' + intRefItemCount, [1, 0, 0, 1, 0, 36]);
                                //var strRefItemCount = SetPrefixPostfix(intRefItemCount + '', intFAFCTotalLenght - 24, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Refund Line Item Count', 22, strRefItemCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter("Customer Count   : ", [1, 16, 0, 0, 0, 36], intSalesTransCount, [0, 10, 0, 0, 1, 36]);
                                //WindowsPrinter('Customer Count ' + intSalesTransCount, [1, 0, 0, 1, 0, 36]);
                                //var strCustomerCount = SetPrefixPostfix(intSalesTransCount + '', intFAFCTotalLenght - 16, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Customer Count', 14, strCustomerCount, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter("Prev. Day EOD Invoice Count : ", [1, 16, 0, 0, 0, 36], PrevInvoiceCount, [0, 20, 0, 0, 1, 36]);
                                WindowsPrinter("Curr. Day Starting Invoice Count : ", [1, 16, 0, 0, 0, 36], BeginningInvoiceCount, [0, 20, 0, 0, 1, 36]);
                                WindowsPrinter("Curr. Day EOD Invoice Count : ", [1, 16, 0, 0, 0, 36], EndingInvoiceCount, [0, 20, 0, 0, 1, 36]);

                                WindowsPrinter("Old Grand Total  : ", [1, 16, 0, 0, 0, 36], AccountingFormat(dblOldGT), [0, 20, 0, 0, 1, 36]);
                                //WindowsPrinter('Old Grand Total ' + AccountingFormat(dblOldGT), [1, 0, 0, 1, 0, 36]);
                                //var strOldGT = SetPrefixPostfix(AccountingFormat(dblOldGT), intFAFCTotalLenght - 17, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Old Grand Total', 15, strOldGT, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                WindowsPrinter("New Grand Total  : ", [1, 16, 0, 0, 0, 36], AccountingFormat(dblNewGT), [0, 20, 0, 0, 1, 36]);
                                //WindowsPrinter('New Grand Total ' + AccountingFormat(dblNewGT), [1, 0, 0, 1, 0, 36]);
                                //var strNewGT = SetPrefixPostfix(AccountingFormat(dblNewGT), intFAFCTotalLenght - 17, ' ', 0);
                                //strPrint = FormatTextToPrint(intFAFCTotalLenght, 'New Grand Total', 15, strNewGT, intFAFCTotalLenght, '', 0, 1);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                //END Other Informatoin

                                WindowsPrinter("Z-READ Print Count:", [1, 16, 0, 0, 0, 36], zReadCount, [0, 20, 0, 0, 1, 36]);

                                //WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                //WindowsPrinter("newLine", [0, 0, 0, 0, 0, 0]);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //strPrint = '';

                                if (!CBool(sessionStorage.PrintingOfflineData)) {
                                    WindowsPrinter(' Printed by ' + (vDataHeader.ReportPOS.Rows[0].PrintedBy || ''), [1, 0, 0, 1, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Printed by ', 15, ': ' + (vDataHeader.ReportPOS.Rows[0].PrintedBy || ''), intFAFCTotalLenght, '', 0, 1);
                                    //strPrint = strPrint + strTemp;

                                    WindowsPrinter(' Date and Time ' + ($.format.date(new Date(vDataHeader.ReportPOS.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), [1, 0, 0, 1, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataHeader.ReportPOS.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                    //strPrint = strPrint + strTemp;
                                } else {
                                    WindowsPrinter(' Printed by ' + (vDataHeader.Rows[0].PrintedBy || ''), [1, 0, 0, 1, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Printed by ', 15, ': ' + (vDataHeader.ReportPOS.Rows[0].PrintedBy || ''), intFAFCTotalLenght, '', 0, 1);
                                    //strPrint = strPrint + strTemp;

                                    WindowsPrinter(' Date and Time ' + ($.format.date(new Date(vDataHeader.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), [1, 0, 0, 1, 0, 36]);
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataHeader.ReportPOS.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                    //strPrint = strPrint + strTemp;
                                }
                                

                                WindowsPrinter("address", [0, 0, 0, 0, 0, 0], strPrinterIP, [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);
                                WindowsPrinter("startPrint", [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);
                            } else if (sessionStorage.Printer == 'IMin') {

                                //Notes
                                //ang size ng ' ' is not the same width as the regular letter. 
                                //' ' = 1/2 size ng small letter
                                //regular lenght if puro letters = 33, if may mga blank, dapat double ang blank to compensate sa isa letter

                                //strPrint = '';
                                //strTemp = '';
                                //intFAFCTotalLenght = 47;

                                //var strTotalSales = SetPrefixPostfix(AccountingFormat(dblTotalSales), intFAFCTotalLenght - 22, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Gross Sales:', 12, strTotalSales, intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;

                                strPrint = '';
                                strTemp = 'Gross Sales: ';

                                IminPrintInColumns(strTemp, "", AccountingFormat(dblTotalSales), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, AccountingFormat(strTotalSales || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                
                                //var strTotalSalesDiscount = SetPrefixPostfix(AccountingFormat(dblTotalSalesDiscount), intFAFCTotalLenght - 22, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Sales Disc.: ', 12, strTotalSalesDiscount, intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;

                                strPrint = '';
                                strTemp = 'Sales Disc.: ';
                                
                                IminPrintInColumns(strTemp, "", AccountingFormat(dblTotalSalesDiscount), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, AccountingFormat(strTotalSalesDiscount || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);


                                //var strTotalVatExempt = SetPrefixPostfix(AccountingFormat(dblTotalVATExempt), intFAFCTotalLenght - 22, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Vat Ex. TAX:', 12, strTotalVatExempt, intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;

                                strPrint = '';
                                strTemp = 'Vat Ex. TAX:';

                                IminPrintInColumns(strTemp, "", AccountingFormat(dblTotalVATExempt), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, AccountingFormat(strTotalVatExempt || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strServiceCharge = SetPrefixPostfix(AccountingFormat(dblServiceCharge), intFAFCTotalLenght - 22, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Charge:        ', 12, strServiceCharge, intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;

                                strPrint = '';
                                strTemp = 'Charge:        ';
                                
                                IminPrintInColumns(strTemp, "", AccountingFormat(dblServiceCharge), 5, 1, 4, 0, 0, 2, 24, 24, 24);
                                //mpadding = AddPadding(strTemp, AccountingFormat(strServiceCharge || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                //var strLocalTax = SetPrefixPostfix(AccountingFormat(dblLocalTax), intFAFCTotalLenght - 22, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Local Tax:    ', 14, strLocalTax + '\n', intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;

                                strPrint = '';
                                strTemp = 'Local Tax:    ';

                                IminPrintInColumns(strTemp, "", AccountingFormat(dblLocalTax), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, AccountingFormat(strLocalTax || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                IminfeedPaper();

                                ////left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);

                                //strPrint = '';
                                //strTemp = '';

                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Summary of Adjustments \n', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //strPrint = strPrint + strTemp;

                                strPrint = '';
                                strTemp = $.trim((' Summary of Adjustments ' || ''));
                                strPrint = strTemp;
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);
                                IminfeedPaper();


                                ////center align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 1, 0, 0, 0);

                                strPrint = '';
                                strTemp = '';

                                var strAdjRefund = SetPrefixPostfix(AccountingFormat(dblAdjRefund), intFAFCTotalLenght - 20, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Total Refund ', 14, strAdjRefund, intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;

                                strPrint = '';
                                strTemp = 'Total Refund ';

                                IminPrintInColumns(strTemp, "", AccountingFormat(dblAdjRefund), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, AccountingFormat(strAdjRefund || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);
                                
                                var strAdjSRV = SetPrefixPostfix(AccountingFormat(dblAdjSRV), intFAFCTotalLenght - 20, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Total SRV ', 14, strAdjSRV + '\n', intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;

                                strPrint = '';
                                strTemp = 'Total SRV ';

                                IminPrintInColumns(strTemp, "", AccountingFormat(dblAdjSRV), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, AccountingFormat(strAdjSRV || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                IminfeedPaper();

                                var strNetSalesAdj = SetPrefixPostfix(AccountingFormat(parseFloat(dblAdjRefund) + parseFloat(dblAdjSRV)), intFAFCTotalLenght - 20, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Net of Adj.: ', 14, strNetSalesAdj, intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;

                                strPrint = '';
                                strTemp = 'Net of Adj.: ';

                                IminPrintInColumns(strTemp, "", AccountingFormat(dblAdjRefund), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, AccountingFormat(strNetSalesAdj || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                strPrint = '';
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                strPrint = strPrint + strTemp;
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strNetSales = SetPrefixPostfix(AccountingFormat(dblNetSales), intFAFCTotalLenght - 20, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Net Sales: ', 14, strNetSales, intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;

                                strPrint = '';
                                strTemp = ' Net Sales: ';

                                IminPrintInColumns(strTemp, "", AccountingFormat(dblNetSales), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, AccountingFormat(strNetSalesAdj || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                ////left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);

                                IminfeedPaper();

                                strPrint = '';
                                strTemp = '';
                                
                                //End Date Sales Summary

                                //Sales Grouped by Mode of Payment----------------------------
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, "Inventory of Payments \n", intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                //center align
                                ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 1, 0, 0, 0);

                                IminfeedPaper();

                                //strPrint = '';
                                //strTemp = '';

                                var strTotalCash = SetPrefixPostfix(AccountingFormat(dblCash), intFAFCTotalLenght - 22, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Cash: ', 14, strTotalCash, intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;
                                strPrint = '';
                                strTemp = ' Cash: ';

                                IminPrintInColumns(strTemp, "", AccountingFormat(dblCash), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, AccountingFormat(strTotalCash || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                ////left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);

                                var strTotalCheck = SetPrefixPostfix(AccountingFormat(dblCheck), intFAFCTotalLenght - 22, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Check: ', 14, strTotalCheck, intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;
                                strPrint = '';
                                strTemp = ' Check: ';

                                IminPrintInColumns(strTemp, "", AccountingFormat(dblCheck + totalPotatoCornerGC), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, AccountingFormat(strTotalCheck || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                ////left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);


                                var strTotalDebitCard = SetPrefixPostfix(AccountingFormat(dblDebitCard), intFAFCTotalLenght - 22, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Debit Card: ', 14, strTotalDebitCard, intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;
                                strPrint = '';
                                strTemp = ' Debit Card: ';

                                IminPrintInColumns(strTemp, "", AccountingFormat(dblDebitCard), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, AccountingFormat(strTotalDebitCard || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                ////left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);

                                var strTotalCreditCard = SetPrefixPostfix(AccountingFormat(dblCreditCard), intFAFCTotalLenght - 22, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Credit Card: ', 14, strTotalCreditCard, intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;
                                strPrint = '';
                                strTemp = ' Credit Card: ';

                                IminPrintInColumns(strTemp, "", AccountingFormat(dblCreditCard), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, AccountingFormat(strTotalCreditCard || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                ////left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);

                                var strTotalSRV = SetPrefixPostfix(AccountingFormat(dblSRV), intFAFCTotalLenght - 22, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, ' SRV: ', 14, strTotalSRV, intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;
                                strPrint = '';
                                strTemp = ' SRV: ';

                                IminPrintInColumns(strTemp, "", AccountingFormat(dblSRV), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, AccountingFormat(strTotalSRV || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                ////left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);
                                

                                //if (IsFowstInSMMall) {
                                //    var strTotalSMAC = SetPrefixPostfix(AccountingFormat(dblSMAC), intFAFCTotalLenght - 16, ' ', 0);
                                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SMAC        ', 14, ': ' + strTotalSMAC, intFAFCTotalLenght, '', 0, 1);
                                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //    var strTotalEPlus = SetPrefixPostfix(AccountingFormat(dblEPlus), intFAFCTotalLenght - 16, ' ', 0);
                                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' E-Plus      ', 14, ': ' + strTotalEPlus, intFAFCTotalLenght, '', 0, 1);
                                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //    var strTotalVoucher = SetPrefixPostfix(AccountingFormat(dblVoucher), intFAFCTotalLenght - 16, ' ', 0);
                                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Voucher     ', 14, ': ' + strTotalVoucher, intFAFCTotalLenght, '', 0, 1);
                                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                //} else {
                                //    var strTotalOther = SetPrefixPostfix(AccountingFormat(dblOtherPaymentType), intFAFCTotalLenght - 16, ' ', 0);
                                //    strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Other Tender: ', 14, strTotalOther, intFAFCTotalLenght, '', 0, 1);
                                //    strPrint = strPrint + strTemp;
                                //}

                                // continue here 
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                strPrint = strPrint + strTemp;
                                ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);

                                var strTotalClassPayment = SetPrefixPostfix(AccountingFormat(dblTotalPayment), intFAFCTotalLenght - 22, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, '            ', 14, '            ' + strTotalClassPayment, intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;

                                strPrint = '';
                                strTemp = '';

                                IminPrintInColumns(strTemp, "", AccountingFormat(dblTotalPayment), 1, 1, 8, 0, 0, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, AccountingFormat(strTotalClassPayment || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                ////left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);

                                IminfeedPaper();

                                ////left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);

                                strPrint = '';
                                strTemp = '';

                                //End Sales Grouped by Mode of Payment----------------------------

                                //Sales Grouped by VAT TYPE----------------------------
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, "Sales Grouped by VAT Type ", intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 1, 0, 0, 0);

                                IminfeedPaper();

                                var dblTotalVATSalesPercent = (dblVAT / dblTotalVAT) * 100;
                                var dblTotalVATAmountPercent = (dblVATAmount / dblTotalVAT) * 100;
                                var dblTotalNonVATPercent = (dblNonVAT / dblTotalVAT) * 100;
                                var dblTotalVATExemptPercent = (dblVATExempt / dblTotalVAT) * 100;
                                var dblTotalZeroRatedVATPercent = (dblZeroRatedVAT / dblTotalVAT) * 100;
                                var dblTotalVATPercent = dblTotalVATSalesPercent + dblTotalVATAmountPercent + dblTotalNonVATPercent + dblTotalVATExemptPercent + dblTotalZeroRatedVATPercent;
                                                                
                                var strTotalVATSales = SetPrefixPostfix(AccountingFormat(dblVAT), 14, ' ', 0);
                                var strTotalVATSalesPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATSalesPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                var strTemp2 = FormatTextToPrint(intFAFCTotalLenght, 'VAT-able Sales', 14, strTotalVATSales, 30, strTotalVATSalesPercent, intFAFCTotalLenght, 1);
                                //strPrint = strPrint + strTemp;

                                strPrint = '';
                                strTemp = 'VAT-able Sales';

                                IminPrintInColumns(strTemp, AccountingFormat(dblVAT), AccountingFormat(dblTotalVATSalesPercent) + '%', 5, 3, 3, 0, 1, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, strTemp2);
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp;

                                ////left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);


                                var strTotalVATAmount = SetPrefixPostfix(AccountingFormat(dblVATAmount), 14, ' ', 0);
                                var strTotalVATAmountPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATAmountPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                strTemp2 = FormatTextToPrint(intFAFCTotalLenght, 'VAT Amount', 14, strTotalVATAmount, 30, strTotalVATAmountPercent, intFAFCTotalLenght, 1);
                                //strPrint = strPrint + strTemp;
                                strPrint = '';
                                strTemp = 'VAT Amount';

                                IminPrintInColumns(strTemp, AccountingFormat(dblVATAmount), AccountingFormat(dblTotalVATAmountPercent) + '%', 5, 3, 3, 0, 1, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, strTemp2);
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp;

                                ////left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);


                                var strTotalNonVAT = SetPrefixPostfix(AccountingFormat(dblNonVAT), 14, ' ', 0);
                                var strTotalNonVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalNonVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                strTemp2 = FormatTextToPrint(intFAFCTotalLenght, 'Non-VAT', 14, strTotalNonVAT, 30, strTotalNonVATPercent, intFAFCTotalLenght, 1);
                                //strPrint = strPrint + strTemp;
                                strPrint = '';
                                strTemp = 'Non-VAT';

                                IminPrintInColumns(strTemp, AccountingFormat(dblNonVAT), AccountingFormat(dblTotalNonVATPercent) + '%', 5, 3, 3, 0, 1, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, strTemp2);
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp;

                                ////left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);


                                var strTotalVATExempt = SetPrefixPostfix(AccountingFormat(dblVATExempt), 14, ' ', 0);
                                var strTotalVATExemptPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATExemptPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                strTemp2 = FormatTextToPrint(intFAFCTotalLenght, 'VAT-Exempt', 14, strTotalVATExempt, 30, strTotalVATExemptPercent, intFAFCTotalLenght, 1);
                                //strPrint = strPrint + strTemp;
                                strPrint = '';
                                strTemp = 'VAT-Exempt';

                                IminPrintInColumns(strTemp, AccountingFormat(dblVATExempt), AccountingFormat(dblTotalVATExemptPercent) + '%', 5, 3, 3, 0, 1, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, strTemp2);
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp;

                                ////left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);


                                var strTotalZeroRatedVAT = SetPrefixPostfix(AccountingFormat(dblZeroRatedVAT), 14, ' ', 0);
                                var strTotalZeroRatedVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalZeroRatedVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                strTemp2 = FormatTextToPrint(intFAFCTotalLenght, 'Zero-Rated VAT', 14, strTotalZeroRatedVAT, 30, strTotalZeroRatedVATPercent, intFAFCTotalLenght, 1);
                                //strPrint = strPrint + strTemp;
                                strPrint = '';
                                strTemp = 'Zero-Rated VAT';

                                IminPrintInColumns(strTemp, AccountingFormat(dblZeroRatedVAT), AccountingFormat(dblTotalZeroRatedVATPercent) + '%', 5, 3, 3, 0, 1, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, strTemp2);
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp;

                                ////left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);

                                strPrint = '';
                                strTemp = '';

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                strPrint = strPrint + strTemp;
                                ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);

                                strPrint = '';
                                strTemp = '';

                                var strTotalAllVAT = SetPrefixPostfix(AccountingFormat(dblTotalVAT), 14, ' ', 0);
                                var strTotalAllVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);

                                //strTemp2 = FormatTextToPrint(intFAFCTotalLenght, '-                -', 14, strTotalAllVAT, 32, strTotalAllVATPercent, intFAFCTotalLenght, 1);
                         
                                IminPrintInColumns(strTemp, AccountingFormat(dblTotalVAT), AccountingFormat(dblTotalVATPercent) + '%', 1, 3, 3, 0, 1, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, strTemp2);
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp;
                                ////left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);


                                //End Sales Grouped by VAT TYPE----------------------------

                                //additional by janine june 17 2015 for sm coin. they require a breakdown of discounts in z report
                                //if (IsFowstInSMMall) {
                                //    ///
                                //    //Discount Breakdown----------------------------
                                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, "Breakdown of Discount", intFAFCTotalLenght, '', 0, '', 0, 0);
                                //    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                //    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);


                                //    var strTotalRegDiscount = SetPrefixPostfix(AccountingFormat(dblRegDiscount), intFAFCTotalLenght - 32, ' ', 0);
                                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Regular Discount', 30, strTotalRegDiscount, intFAFCTotalLenght, '', 0, 1);
                                //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                //    var strTotalEmpDiscount = SetPrefixPostfix(AccountingFormat(dblEmpDiscount), intFAFCTotalLenght - 32, ' ', 0);
                                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Employee Discount', 30, strTotalEmpDiscount, intFAFCTotalLenght, '', 0, 1);
                                //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                //    var strTotalSCD = SetPrefixPostfix(AccountingFormat(dblSCD), intFAFCTotalLenght - 32, ' ', 0);
                                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Senior Citizen', 30, strTotalSCD, intFAFCTotalLenght, '', 0, 1);
                                //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                //    var strTotalPWD = SetPrefixPostfix(AccountingFormat(dblPWD), intFAFCTotalLenght - 32, ' ', 0);
                                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' PWD Discount', 30, strTotalPWD, intFAFCTotalLenght, '', 0, 1);
                                //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                //    var strTotalVIP = SetPrefixPostfix(AccountingFormat(dblVIP), intFAFCTotalLenght - 32, ' ', 0);
                                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' VIP Discount', 30, strTotalVIP, intFAFCTotalLenght, '', 0, 1);
                                //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                //    var strTotalGPC = SetPrefixPostfix(AccountingFormat(dblGPC), intFAFCTotalLenght - 32, ' ', 0);
                                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' GPC ', 30, strTotalGPC, intFAFCTotalLenght, '', 0, 1);
                                //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);


                                //    var strTotalDiscount1 = SetPrefixPostfix(AccountingFormat(dblDiscount1), intFAFCTotalLenght - 32, ' ', 0);
                                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Discount 1 ', 30, strTotalDiscount1, intFAFCTotalLenght, '', 0, 1);
                                //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);




                                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);


                                //    var strTotalSalesDiscount2 = SetPrefixPostfix(AccountingFormat(dblTotalSalesDiscount), intFAFCTotalLenght - 32, ' ', 0);
                                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' ', 30, strTotalSalesDiscount2, intFAFCTotalLenght, '', 0, 1);
                                //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);



                                //    ///
                                //}

                                IminfeedPaper();

                                strPrint = '';
                                strTemp = '';

                                //Other Informatoin
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, "Other Information \n", intFAFCTotalLenght, '', 0, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                //center align
                                ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 1, 0, 0, 0);

                                IminfeedPaper();

                                strPrint = '';
                                strTemp = '';

                                var strTransCount = SetPrefixPostfix(intTransCount + '', intFAFCTotalLenght - 22, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Total Trans. Count', 14, strTransCount, intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;
                                strPrint = '';
                                strTemp = 'Total Trans. Count';

                                IminPrintInColumns(strTemp, "", intTransCount, 6, 1, 3, 0, 0, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, AccountingFormat(strTransCount || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                //left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);

                                var strSalesTransCount = SetPrefixPostfix(intSalesTransCount + '', intFAFCTotalLenght - 22, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Sales Trans. Count', 14, strSalesTransCount, intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;

                                strPrint = '';
                                strTemp = 'Sales Trans. Count';

                                IminPrintInColumns(strTemp, "", intSalesTransCount, 6, 1, 3, 0, 0, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, AccountingFormat(strSalesTransCount || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                ////left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);

                                var strSalesItemCount = SetPrefixPostfix(intSalesItemCount + '', intFAFCTotalLenght - 22, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Sales Line Item Count', 14, strSalesItemCount, intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;
                                strPrint = '';
                                strTemp = 'Sales Item Count';

                                IminPrintInColumns(strTemp, "", intSalesItemCount, 6, 1, 3, 0, 0, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, AccountingFormat(strSalesItemCount || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                ////left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);

                                var strSRVTransCount = SetPrefixPostfix(intSRVTransCount + '', intFAFCTotalLenght - 22, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'SRV Trans. Count', 14, strSRVTransCount, intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;
                                strPrint = '';
                                strTemp = 'SRV Trans. Count';

                                IminPrintInColumns(strTemp, "", intSRVTransCount, 6, 1, 3, 0, 0, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, AccountingFormat(strSRVTransCount || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                ////left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);

                                var strSRVItemCount = SetPrefixPostfix(intSRVItemCount + '', intFAFCTotalLenght - 22, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'SRV Line Item Count', 14, strSRVItemCount, intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;
                                strPrint = '';
                                strTemp = 'SRV Item Count';

                                IminPrintInColumns(strTemp, "", intSRVItemCount, 6, 1, 3, 0, 0, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, AccountingFormat(strSRVItemCount || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                ////left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);

                                var strRefTransCount = SetPrefixPostfix(intRefTransCount + '', intFAFCTotalLenght - 22, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Refund Trans. Count', 14, strRefTransCount, intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;
                                strPrint = '';
                                strTemp = 'Refund Trans. Count';

                                IminPrintInColumns(strTemp, "", intRefTransCount, 6, 1, 3, 0, 0, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, AccountingFormat(strRefTransCount || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                ////left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);


                                var strRefItemCount = SetPrefixPostfix(intRefItemCount + '', intFAFCTotalLenght - 22, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Refund Line Item Count', 14, strRefItemCount, intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;
                                strPrint = '';
                                strTemp = 'Refund Item Count';

                                IminPrintInColumns(strTemp, "", intRefItemCount, 6, 1, 3, 0, 0, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, AccountingFormat(strRefItemCount || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                ////left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);

                                var strCustomerCount = SetPrefixPostfix(intSalesTransCount + '', intFAFCTotalLenght - 22, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Customer Count', 14, strCustomerCount, intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;
                                strPrint = '';
                                strTemp = 'Customer Count';

                                IminPrintInColumns(strTemp, "", intSalesTransCount, 6, 1, 3, 0, 0, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, AccountingFormat(strCustomerCount || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                ////left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);

                                var strPrevInvoiceCount = SetPrefixPostfix(AccountingFormat(PrevInvoiceCount), intFAFCTotalLenght - 22, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Prev. EOD Inv. Count', 14, strPrevInvoiceCount, intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;
                                strPrint = '';
                                strTemp = 'Prev. Inv. Count';

                                IminPrintInColumns(strTemp, "", PrevInvoiceCount, 6, 1, 3, 0, 0, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, AccountingFormat(strPrevInvoiceCount || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                ////left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);

                                var strBeginningInvoiceCount = SetPrefixPostfix(AccountingFormat(BeginningInvoiceCount), intFAFCTotalLenght - 22, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Curr. Starting Inv. Count', 14, strBeginningInvoiceCount, intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;
                                strPrint = '';
                                strTemp = 'Curr. Start. Inv. Count';

                                IminPrintInColumns(strTemp, "", BeginningInvoiceCount, 6, 1, 3, 0, 0, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, AccountingFormat(strBeginningInvoiceCount || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                ////left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);

                                var strEndingInvoiceCount = SetPrefixPostfix(AccountingFormat(EndingInvoiceCount), intFAFCTotalLenght - 22, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Curr. EOD Inv. Count ', 14, strEndingInvoiceCount, intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;

                                strPrint = '';
                                strTemp = 'Curr. EOD Inv. Count ';

                                IminPrintInColumns(strTemp, "", AccountingFormat(EndingInvoiceCount), 6, 1, 3, 0, 0, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, AccountingFormat(strEndingInvoiceCount || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                ////left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);

                                var strOldGT = SetPrefixPostfix(AccountingFormat(dblOldGT), intFAFCTotalLenght - 22, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Old GT      ', 14, strOldGT, intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;
                                strPrint = '';
                                strTemp = 'Old GT      -';

                                IminPrintInColumns(strTemp, "", AccountingFormat(dblOldGT), 5, 1, 4, 0, 0, 2, 24, 24, 24);


                                //mpadding = AddPadding(strTemp, AccountingFormat(strOldGT || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                ////left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);

                                var strNewGT = SetPrefixPostfix(AccountingFormat(dblNewGT), intFAFCTotalLenght - 22, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'New GT      ', 14, strNewGT, intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;
                                strPrint = '';
                                strTemp = 'New GT      -';

                                IminPrintInColumns(strTemp, "", AccountingFormat(dblNewGT), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, AccountingFormat(strNewGT || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                ////left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);


                                var strzReadCount = SetPrefixPostfix('Z-' + zReadCount + '', intFAFCTotalLenght - 22, ' ', 0);
                                //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Print Count', 14, strzReadCount, intFAFCTotalLenght, '', 0, 1);
                                //strPrint = strPrint + strTemp;
                                strPrint = '';
                                strTemp = 'Print Count';

                                IminPrintInColumns(strTemp, "", AccountingFormat(zReadCount), 5, 1, 4, 0, 0, 2, 24, 24, 24);

                                //mpadding = AddPadding(strTemp, AccountingFormat(strzReadCount || '0'));
                                //strTemp += mpadding;
                                //strPrint = strPrint + strTemp + '\n';

                                ////left align
                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);

                                //END Other Informatoin
                                
                                strPrint = '';

                                if (!CBool(sessionStorage.PrintingOfflineData)) {
                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Printed by ', 12, ': ' + (vDataHeader.ReportPOS.Rows[0].PrintedBy || ''), intFAFCTotalLenght, '', 0, 1);
                                    //strPrint = strPrint + strTemp;

                                    IminPrintInColumns("Printed by ", "", (vDataHeader.ReportPOS.Rows[0].PrintedBy || ''), 3, 1, 6, 0, 0, 2, 24, 24, 24);

                                    //strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataHeader.ReportPOS.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                    //strPrint = strPrint + strTemp;

                                    IminPrintInColumns("Date and Time ", "", ($.format.date(new Date(vDataHeader.ReportPOS.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), 5, 0, 6, 0, 0, 2, 24, 24, 24);

                                } else {
                                    
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Printed by ', 12, ': ' + (vDataHeader.Rows[0].PrintedBy || ''), intFAFCTotalLenght, '', 0, 1);
                                    strPrint = strPrint + strTemp;

                                    IminPrintInColumns("Printed by ", "", (vDataHeader.Rows[0].PrintedBy || ''), 3, 1, 6, 0, 0, 2, 24, 24, 24);

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataHeader.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                    strPrint = strPrint + strTemp;

                                    IminPrintInColumns("Date and Time ", "", ($.format.date(new Date(vDataHeader.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), 5, 0, 6, 0, 0, 2, 24, 24, 24);
                                }

                                //ReadyTextToPrint(strPrint, 3, 24, 1, 0, 0, 0, 0, 0, 0);

                                //PrintText(strPrinterIP);
                                
                                IminfeedPaper();
                                IminfeedPaper();
                                IminfeedPaper();

                            } else if (window.InAndroid) {

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

                                var strTotalCheck = SetPrefixPostfix(AccountingFormat(dblCheck + totalPotatoCornerGC), intFAFCTotalLenght - 16, ' ', 0);
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
                                    //var strTotalOther = SetPrefixPostfix(AccountingFormat(dblOtherPaymentType), intFAFCTotalLenght - 16, ' ', 0);
                                    //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Other Tender', 12, ': ' + strTotalOther, intFAFCTotalLenght, '', 0, 1);
                                    //ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                    if (vDataOtherPaymentType != undefined) {
                                        if (vDataOtherPaymentType.Rows.length > 0) {
                                            mOtherPaymentTypeColumns = Object.entries(vDataOtherPaymentType.Columns);
                                            //i=10 since magsugod ang other payment type sa 
                                            for (var i = 28; i <= mOtherPaymentTypeColumns.length - 1; i++) {
                                                if (CDec2(vDataOtherPaymentType.Rows[0][mOtherPaymentTypeColumns[i][0]]) > 0) {
                                                    var OPAmount = vDataOtherPaymentType.Rows[0][mOtherPaymentTypeColumns[i][0]];

                                                    if (mOtherPaymentTypeColumns[i][0].replace('OtherPayment', '').toLowerCase() == 'potatocornergc') continue;
                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, mOtherPaymentTypeColumns[i][0].replace('OtherPayment', ''), 12, ': ' + AccountingFormat(OPAmount), intFAFCTotalLenght, '', 0, 1);
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                                }
                                            }
                                        }
                                    }
                                }

                                // continue here 
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

                                var strPrevInvoiceCount = SetPrefixPostfix(AccountingFormat(PrevInvoiceCount), intFAFCTotalLenght - 17, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Prev. Day EOD Invoice Count', 15, strPrevInvoiceCount, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strBeginningInvoiceCount = SetPrefixPostfix(AccountingFormat(BeginningInvoiceCount), intFAFCTotalLenght - 17, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Curr. Day Starting Invoice Count', 15, strBeginningInvoiceCount, intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                var strEndingInvoiceCount = SetPrefixPostfix(AccountingFormat(EndingInvoiceCount), intFAFCTotalLenght - 17, ' ', 0);
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Curr. Day EOD Invoice Count', 15, strEndingInvoiceCount, intFAFCTotalLenght, '', 0, 1);
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

                                if (!CBool(sessionStorage.PrintingOfflineData)) {
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Printed by ', 15, ': ' + (vDataHeader.ReportPOS.Rows[0].PrintedBy || ''), intFAFCTotalLenght, '', 0, 1);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataHeader.ReportPOS.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                    strPrint = strPrint + strTemp;
                                } else {
                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Printed by ', 15, ': ' + (vDataHeader.Rows[0].PrintedBy || ''), intFAFCTotalLenght, '', 0, 1);
                                    strPrint = strPrint + strTemp;

                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataHeader.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                    strPrint = strPrint + strTemp;
                                }
                                
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);

                                PrintText(strPrinterIP);
                                ClearCommandBuffer();
                            }

                            //gi-comment bcause we're printing z-read report according to its device-fowst

                            //MessageBox("Would you like to print Z-READ for every MIN?",
                            //        "Z-READ By MIN",
                            //        [
                            //            {
                            //                Text: 'Yes',
                            //                Click: function () {

                            //                    $.mobile.changePage(window.pageZReportPage);
                            //                    //Get Z-Report BY MIN
                            //                    AjaxSend({
                            //                        url: "ReportsPOS/getSalesZReportSummary",
                            //                        data: pData,
                            //                        success: function (vData) {

                            //                            PrintSalesZReportByMIN(vDataHeader, vData, 0);
                            //                        }
                            //                    });
                            //                }
                            //            },
                            //            {
                            //                Text: 'No',
                            //                Click: function () {
                            //                    CloseForSMCOIN();
                            //                }
                            //            }
                            //        ]
                            //);

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
                        $('#divZReportNoData').hide();
                    } else if (window.InAndroid) {
                        if (Business.SystemDefaults.GetEnforceZRead()) {
                            alert('increment zread day')
                            return
                            SalesMenu_offline().IncrementZReadDay({
                                DontLimit: true
                            });
                            PrintSalesZReport(true);
                        }
                    } else {
                        NewMessageBox('Creating document for printing.', 'Please Wait');
                        AjaxSend({
                            url: '/ReportsPOS/CreateZReadPdf',
                            data: {
                                Values: JSON.stringify(pValues)
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
                } // END function DisplaySalesZReportSummary(vDataHeader, vDataInfo, vIsView) {

                function DisplayDiscountsBreakdown(vData, isPrint) {
                    var tblDiscountsBreakdown = $('#tblDiscountsBreakdown');
                    tblDiscountsBreakdown.empty();

                    for (var i in vData) {
                        var code = '';

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
                        }

                        if (!CBool(isPrint)) {
                            tblDiscountsBreakdown.append($('<tr>')
                                .append($('<td class="text-right">').text(code))
                                .append($('<td class="pl-3">').text(AccountingFormat(vData[i].DiscountAmount)))
                            )
                        } else {
                            if (isPrintInWPS) {
                                WindowsPrinter("    " + code + " : " + AccountingFormat(vData[i].DiscountAmount), [1, 25, 0, 0, 0, 36], "", [0, 15, 0, 0, 1, 36]);
                            }
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

                function PrintMobizControllerSalesZReport(vIsView) {

                    var pData = {};
                    var strDate = $("#selZReportByDateMonth").val() + "/" + $("#selZReportByDateDay").val() + "/" + $("#selZReportByDateYear").val();
                    pData.RangeType = 'By Date'; // $.trim($("#selZReportRange").val());
                    pData.ZDate = dateTo;

                    NewMessageBox('Retrieving data from server.', 'Please Wait');
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
                } // END PrintMobizControllerSalesZReport

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
                                                            dblOldGT = dblOldGT + parseFloat(vDataInfo.ReportPOS.Rows[p].BegBal || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].PrevGrossSales || 0) - (parseFloat(vDataInfo.ReportPOS.Rows[p].PrevTotalDiscount || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].PrevSCDiscount || 0)) - parseFloat(vDataInfo.ReportPOS.Rows[p].PrevSalesReturnRefund || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].PrevSalesReturnExchange || 0) - parseFloat(vDataInfo.ReportPOS.Rows[p].PrevVATExempt || 0) + parseFloat(vDataInfo.ReportPOS.Rows[p].PrevServiceCharge || 0);
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

                                dblCash = dblCash + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CashAmount || 0) //- dblAdjRefund - dblAdjSRV;
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

                                dblOldGT = dblOldGT + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].BegBal || 0) + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].PrevGrossSales || 0) - (parseFloat(vDataInfo.ReportPOS.Rows[intIndex].PrevTotalDiscount || 0) + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].PrevSCDiscount || 0)) - parseFloat(vDataInfo.ReportPOS.Rows[intIndex].PrevSalesReturnRefund || 0) - parseFloat(vDataInfo.ReportPOS.Rows[intIndex].PrevSalesReturnExchange || 0) - parseFloat(vDataInfo.ReportPOS.Rows[intIndex].PrevVATExempt || 0) + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].PrevServiceCharge || 0);
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

                                var strTotalVATSales = SetPrefixPostfix(AccountingFormat(dblVAT), 14, ' ', 0);
                                var strTotalVATSalesPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATSalesPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                var strTotalVATAmount = SetPrefixPostfix(AccountingFormat(dblVATAmount), 14, ' ', 0);
                                var strTotalVATAmountPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATAmountPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                var strTotalNonVAT = SetPrefixPostfix(AccountingFormat(dblNonVAT), 14, ' ', 0);
                                var strTotalNonVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalNonVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                var strTotalVATExempt = SetPrefixPostfix(AccountingFormat(dblVATExempt), 14, ' ', 0);
                                var strTotalVATExemptPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATExemptPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                var strTotalZeroRatedVAT = SetPrefixPostfix(AccountingFormat(dblZeroRatedVAT), 14, ' ', 0);
                                var strTotalZeroRatedVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalZeroRatedVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                var strTotalAllVAT = SetPrefixPostfix(AccountingFormat(dblTotalVAT), 14, ' ', 0);
                                var strTotalAllVATPercent = SetPrefixPostfix(AccountingFormat(dblTotalVATPercent) + '%', intFAFCTotalLenght - 32, ' ', 0);

                                if (IsVAT != undefined) {
                                    if (!CBool(IsVAT)) {
                                        strTotalVATSales = SetPrefixPostfix(AccountingFormat(0), 14, ' ', 0);
                                        strTotalVATSalesPercent = SetPrefixPostfix(AccountingFormat(0) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                        strTotalVATAmount = SetPrefixPostfix(AccountingFormat(0), 14, ' ', 0);
                                        strTotalVATAmountPercent = SetPrefixPostfix(AccountingFormat(0) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                        strTotalNonVAT = SetPrefixPostfix(AccountingFormat(CDec2(dblVAT) + CDec2(dblVATAmount)), 14, ' ', 0);
                                        strTotalNonVATPercent = SetPrefixPostfix(AccountingFormat(100) + '%', intFAFCTotalLenght - 32, ' ', 0);
                                    }
                                }

                                strPrint = ' ';
                                
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-able Sales', 14, strTotalVATSales, 30, strTotalVATSalesPercent, intFAFCTotalLenght, 1);
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

                                        dblCash = dblCash + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].CashAmount || 0) //- dblAdjRefund - dblAdjSRV;
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

                                        dblOldGT = dblOldGT + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].BegBal || 0) + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].PrevGrossSales || 0) - (parseFloat(vDataInfo.ReportPOS.Rows[intIndex].PrevTotalDiscount || 0) + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].PrevSCDiscount || 0)) - parseFloat(vDataInfo.ReportPOS.Rows[intIndex].PrevSalesReturnRefund || 0) - parseFloat(vDataInfo.ReportPOS.Rows[intIndex].PrevSalesReturnExchange || 0) - parseFloat(vDataInfo.ReportPOS.Rows[intIndex].PrevVATExempt || 0) + parseFloat(vDataInfo.ReportPOS.Rows[intIndex].PrevServiceCharge || 0);
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

                //function PrintSalesZReportFromDUCERP(dblOldGT) {
                //    setPrinterConnectionType('TCP/IP', 'Epson TM-U220B Printer', false)

                //    NewPrintBuilder();

                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim(('The Paper Tree' || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                //    strPrint = '';
                //    strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim(('Davao Unified Corporation' || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                //    strPrint = strPrint + strTemp;

                //    strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim(('The Paper Tree - SPC' || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                //    strPrint = strPrint + strTemp;

                //    strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim(('Guerrero St., Fronting San Pedro College, Davao City, Philippines 8000' || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                //    strPrint = strPrint + strTemp;

                //    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'TIN ' + $.trim(('005-161-114-008' || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                //    strPrint = strPrint + strTemp;

                //    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Tel. No.' + $.trim(('(082) 305 3107' || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                //    strPrint = strPrint + strTemp;

                //    strPrint = strPrint + '\n';

                //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, "Z-READ", intFAFCTotalLenght, '', 0, '', 0, 0);
                //    ReadyTextToPrint(strPrint, 3, 2, 1, 1, 1, 1, 0, 0, 0);

                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Date : ' + $.format.date(new Date(), 'MM/dd/yyyy'), intFAFCTotalLenght, '', 0, '', 0, 0);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                //    //Date Sales Summary
                //    var strTotalSales = SetPrefixPostfix(AccountingFormat(0), intFAFCTotalLenght - 16, ' ', 0);
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Gross Sales: ', 14, strTotalSales, intFAFCTotalLenght, '', 0, 1);
                //    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                //    var strTotalSalesDiscount = SetPrefixPostfix(AccountingFormat(0), intFAFCTotalLenght - 19, ' ', 0);
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Sales Discount: ', 17, strTotalSalesDiscount, intFAFCTotalLenght, '', 0, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                //    var strTotalVatExempt = SetPrefixPostfix(AccountingFormat(0), intFAFCTotalLenght - 19, ' ', 0);
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Vat Exempt TAX: ', 17, strTotalVatExempt, intFAFCTotalLenght, '', 0, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                //    var strServiceCharge = SetPrefixPostfix(AccountingFormat(0), intFAFCTotalLenght - 19, ' ', 0);
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Service Charge: ', 17, strServiceCharge, intFAFCTotalLenght, '', 0, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                //    var strLocalTax = SetPrefixPostfix(AccountingFormat(0), intFAFCTotalLenght - 19, ' ', 0);
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Local Tax: ', 17, strLocalTax, intFAFCTotalLenght, '', 0, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Summary of Adjustments', intFAFCTotalLenght, '', 0, '', 0, 1);
                //    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                //    var strAdjRefund = SetPrefixPostfix(AccountingFormat(0), intFAFCTotalLenght - 29, ' ', 0);
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Sales Refund for the  Period', 27, strAdjRefund, intFAFCTotalLenght, '', 0, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                //    var strAdjSRV = SetPrefixPostfix(AccountingFormat(0), intFAFCTotalLenght - 32, ' ', 0);
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Sales Returns Subject to Merchadise Exchange           (SRV Issued)', 30, strAdjSRV, intFAFCTotalLenght, '', 0, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                //    var strNetSalesAdj = SetPrefixPostfix(AccountingFormat(parseFloat(0) + parseFloat(0)), intFAFCTotalLenght - 23, ' ', 0);
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Net of Adjustments: ', 21, strNetSalesAdj, intFAFCTotalLenght, '', 0, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                //    var strNetSales = SetPrefixPostfix(AccountingFormat(0), intFAFCTotalLenght - 16, ' ', 0);
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Net Sales: ', 12, '  ' + strNetSales, intFAFCTotalLenght, '', 0, 1);
                //    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                //    //End Date Sales Summary

                //    //Sales Grouped by Mode of Payment----------------------------
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, "Inventory of Payments", intFAFCTotalLenght, '', 0, '', 0, 0);
                //    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                //    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                //    var strTotalCash = SetPrefixPostfix(AccountingFormat(0), intFAFCTotalLenght - 16, ' ', 0);
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Cash       ', 12, ': ' + strTotalCash, intFAFCTotalLenght, '', 0, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                //    var strTotalCheck = SetPrefixPostfix(AccountingFormat(0), intFAFCTotalLenght - 16, ' ', 0);
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Check      ', 12, ': ' + strTotalCheck, intFAFCTotalLenght, '', 0, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                //    var strTotalDebitCard = SetPrefixPostfix(AccountingFormat(0), intFAFCTotalLenght - 16, ' ', 0);
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Debit Card ', 12, ': ' + strTotalDebitCard, intFAFCTotalLenght, '', 0, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                //    var strTotalCreditCard = SetPrefixPostfix(AccountingFormat(0), intFAFCTotalLenght - 16, ' ', 0);
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Credit Card', 12, ': ' + strTotalCreditCard, intFAFCTotalLenght, '', 0, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                //    var strTotalSRV = SetPrefixPostfix(AccountingFormat(0), intFAFCTotalLenght - 16, ' ', 0);
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, ' SRV        ', 12, ': ' + strTotalSRV, intFAFCTotalLenght, '', 0, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                        
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                //    var strTotalClassPayment = SetPrefixPostfix(AccountingFormat(0), intFAFCTotalLenght - 16, ' ', 0);
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, '            ', 12, '  ' + strTotalClassPayment, intFAFCTotalLenght, '', 0, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);


                //    //End Sales Grouped by Mode of Payment----------------------------

                //    //Sales Grouped by VAT TYPE----------------------------
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, "Sales Grouped by VAT Type", intFAFCTotalLenght, '', 0, '', 0, 0);
                //    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                //    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                //    strPrint = ' ';
                //    var strTotalVATSales = SetPrefixPostfix(AccountingFormat(0), 14, ' ', 0);
                //    var strTotalVATSalesPercent = SetPrefixPostfix(AccountingFormat(0) + '%', intFAFCTotalLenght - 32, ' ', 0);
                //    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-able Sales', 14, strTotalVATSales, 30, strTotalVATSalesPercent, intFAFCTotalLenght, 1);
                //    strPrint = strPrint + strTemp;

                //    var strTotalVATAmount = SetPrefixPostfix(AccountingFormat(0), 14, ' ', 0);
                //    var strTotalVATAmountPercent = SetPrefixPostfix(AccountingFormat(0) + '%', intFAFCTotalLenght - 32, ' ', 0);
                //    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT Amount', 14, strTotalVATAmount, 30, strTotalVATAmountPercent, intFAFCTotalLenght, 1);
                //    strPrint = strPrint + strTemp;

                //    var strTotalNonVAT = SetPrefixPostfix(AccountingFormat(0), 14, ' ', 0);
                //    var strTotalNonVATPercent = SetPrefixPostfix(AccountingFormat(0) + '%', intFAFCTotalLenght - 32, ' ', 0);
                //    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Non-VAT', 14, strTotalNonVAT, 30, strTotalNonVATPercent, intFAFCTotalLenght, 1);
                //    strPrint = strPrint + strTemp;

                //    var strTotalVATExempt = SetPrefixPostfix(AccountingFormat(0), 14, ' ', 0);
                //    var strTotalVATExemptPercent = SetPrefixPostfix(AccountingFormat(0) + '%', intFAFCTotalLenght - 32, ' ', 0);
                //    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-Exempt', 14, strTotalVATExempt, 30, strTotalVATExemptPercent, intFAFCTotalLenght, 1);
                //    strPrint = strPrint + strTemp;

                //    var strTotalZeroRatedVAT = SetPrefixPostfix(AccountingFormat(0), 14, ' ', 0);
                //    var strTotalZeroRatedVATPercent = SetPrefixPostfix(AccountingFormat(0) + '%', intFAFCTotalLenght - 32, ' ', 0);
                //    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Zero-Rated VAT', 14, strTotalZeroRatedVAT, 30, strTotalZeroRatedVATPercent, intFAFCTotalLenght, 1);
                //    strPrint = strPrint + strTemp;

                //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                //    var strTotalAllVAT = SetPrefixPostfix(AccountingFormat(0), 14, ' ', 0);
                //    var strTotalAllVATPercent = SetPrefixPostfix(AccountingFormat(0) + '%', intFAFCTotalLenght - 32, ' ', 0);
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, '              ', 14, strTotalAllVAT, 30, strTotalAllVATPercent, intFAFCTotalLenght, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                //    //End Sales Grouped by VAT TYPE----------------------------

                    
                //    //Other Informatoin
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, "Other Information", intFAFCTotalLenght, '', 0, '', 0, 0);
                //    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                //    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                //    var strTransCount = SetPrefixPostfix(0 + '', intFAFCTotalLenght - 20, ' ', 0);
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Total Trans. Count', 18, strTransCount, intFAFCTotalLenght, '', 0, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                //    var strSalesTransCount = SetPrefixPostfix(0 + '', intFAFCTotalLenght - 20, ' ', 0);
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Sales Trans. Count', 18, strSalesTransCount, intFAFCTotalLenght, '', 0, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                //    var strSalesItemCount = SetPrefixPostfix(0 + '', intFAFCTotalLenght - 23, ' ', 0);
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Sales Line Item Count', 21, strSalesItemCount, intFAFCTotalLenght, '', 0, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                //    var strSRVTransCount = SetPrefixPostfix(0 + '', intFAFCTotalLenght - 18, ' ', 0);
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, 'SRV Trans. Count', 16, strSRVTransCount, intFAFCTotalLenght, '', 0, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                //    var strSRVItemCount = SetPrefixPostfix(0 + '', intFAFCTotalLenght - 21, ' ', 0);
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, 'SRV Line Item Count', 19, strSRVItemCount, intFAFCTotalLenght, '', 0, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                //    var strRefTransCount = SetPrefixPostfix(0 + '', intFAFCTotalLenght - 21, ' ', 0);
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Refund Trans. Count', 19, strRefTransCount, intFAFCTotalLenght, '', 0, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                //    var strRefItemCount = SetPrefixPostfix(0 + '', intFAFCTotalLenght - 24, ' ', 0);
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Refund Line Item Count', 22, strRefItemCount, intFAFCTotalLenght, '', 0, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                //    var strCustomerCount = SetPrefixPostfix(0 + '', intFAFCTotalLenght - 16, ' ', 0);
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Customer Count', 14, strCustomerCount, intFAFCTotalLenght, '', 0, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                //    var strOldGT = SetPrefixPostfix(AccountingFormat(dblOldGT), intFAFCTotalLenght - 17, ' ', 0);
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Old Grand Total', 15, strOldGT, intFAFCTotalLenght, '', 0, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                //    var strNewGT = SetPrefixPostfix(AccountingFormat(dblOldGT), intFAFCTotalLenght - 17, ' ', 0);
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, 'New Grand Total', 15, strNewGT, intFAFCTotalLenght, '', 0, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                //    var strzReadCount = SetPrefixPostfix('Z-' + 2253 + '', intFAFCTotalLenght - 21, ' ', 0);
                //    strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Z-READ Print Count', 19, strzReadCount, intFAFCTotalLenght, '', 0, 1);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                //    //END Other Informatoin

                //    //ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                //    strPrint = '';
                //    //strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Printed by ', 15, ': ' + (vDataHeader.ReportPOS.Rows[0].PrintedBy || ''), intFAFCTotalLenght, '', 0, 1);
                //    //strPrint = strPrint + strTemp;

                //    strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                //    strPrint = strPrint + strTemp;

                //    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                //    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                //    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 0, 0, 0, 1);

                //    PrintText(strPrinterIP);
                //    ClearCommandBuffer();
                //}

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

                    pData.RangeType = rangeType || 'Today'; //$('#selZReportRange').val();
                    pData.SalesDate = $('#txtZreportDateFrom').text();
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

                function CreateOtherPaymentTypeRow(OtherPaymentType, OtherPaymentTypeAmount) {
                    return $('<tr>')
                            .append($('<td style="text-align:left">')
                                .text(OtherPaymentType)
                            )
                            .append($('<td style="text-align:right">')
                                .text(AccountingFormat(OtherPaymentTypeAmount))
                            )
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

            //});

        }); //require(reqJS, function () {
        var pDeferred = $.Deferred();
        return pDeferred.promise();
    });