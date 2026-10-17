require([
    'common',
    'jquery.plugin.html2canvas',
    '/Scripts/sources/SalesPayments/SalesPaymentsExport_offline.js?crc=',
    GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc=')
    ], function () {

        var reqJS;

        if (OfflineMode) {
            reqJS = [
                // '/Scripts/sources/SalesPayments/SalesPaymentsExport_offline.j s?crc=' // commented by Arvin. double na sa taas. kihiwalay nako ang j ug s kay ma detect sa CRC
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

                setMonthCollection("selPOSSalesReportByDateFromMonth");
                setYearCollection("selPOSSalesReportByDateFromYear");

                setMonthCollection("selPOSSalesReportByDateToMonth");
                setYearCollection("selPOSSalesReportByDateToYear");

                setMonthCollection("selPOSSalesReportRangeByMonthFrom");
                setMonthCollection("selPOSSalesReportRangeByMonthTo");

                setYearCollection("selPOSSalesReportRangeByYear");

                SetWeekRange();

                if (IsMobile) {
                    if (Web.HandleOrganization() == false)
                        return;
                }

                $(document).ready(function () {

                    if (!window.InAndroid) {
                        $('<applet name="jzebra" code="jzebra.PrintApplet.class" archive="/Scripts/jzebra.jar" width="10" height="10"></applet>').appendTo($('body'));
                    }

                    if (sessionStorage.FowstID == undefined || sessionStorage.FowstID == "" || sessionStorage.FowstID == null) {
                        GetFowst();
                    }
                });

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

                $('#btnPOSSalesAdjReportPrint').click(function (e) {

                    e.preventDefault();

                    //Flush First before printing the report
                    if (OfflineMode) {
                        flushedCounter = 0;
                        mInterval = setInterval(FlushDeviceData, 1000);
                    } else {
                        if (window.InAndroid) {
                            PrintSalesReport();
                        } else {
                            PrintDesktopSalesReport();
                        }

                    }
                });

                $('#btnPOSSalesAdjReportCancel').click(function (e) {

                    e.preventDefault();

                    window.open(GetPathFormat("/People/PeopleMenu"), "_self");
                });

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

                        //Sql.AttachDatabase(sessionStorage.OrgID);

                        Web.HandleOrganization(true);

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

                        }
                        else {
                            $("#dvpopup").popup("close");
                            PrintSalesReport();
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

                function PrintSalesReport() {
                    var strPrinterIP = '';
                    var strTemp = '';
                    var strPrint = '';
                    var pData = {};
                    var strFrom = '';
                    var strTo = '';
                    var strYear = '';

                    //Validate
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

                    AjaxSend({
                        url: "Organization/getOrgFOWSTPrinterIP",
                        success: function (vDataPrint) {

                            if (vDataPrint.OrganizationFOWSTData.Rows.length > 0) {
                                strPrinterIP = vDataPrint.OrganizationFOWSTData.Rows[0].PrinterIPAddress;

                                pData.RangeType = $.trim($("#selPOSSalesReportRange").val());
                                pData.RangeFrom = strFrom;
                                pData.RangeTo = strTo;
                                pData.RangeYear = strYear;

                                //Get Header
                                AjaxSend({
                                    url: "ReportsPOS/getReportHeader",
                                    success: function (vDataHeader) {

                                        //New set of text to print
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

                                        //Get sale sales adjusment info
                                        AjaxSend({
                                            url: "ReportsPOS/getSalesAdjustmentReportInfo",
                                            data: pData,
                                            success: function (vDataInfo) {

                                                strPrint = FormatTextToPrint(intFAFCTotalLenght, "Sales Adjustment Summary", intFAFCTotalLenght, '', 0, '', 0, 0);
                                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                                if ($("#selPOSSalesReportRange").val() == "Day") {

                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? 'Date: ' + strFrom : 'From ' + strFrom + ' To ' + strTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                                } else if ($("#selPOSSalesReportRange").val() == "Week") {

                                                    strPrint = '';
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? 'Week: ' + strFrom : 'From Week ' + strFrom + ' To Week ' + strTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataInfo.ReportPOS.Rows[0].YearFrom == vDataInfo.ReportPOS.Rows[0].YearTo ? "Year : " + vDataInfo.ReportPOS.Rows[0].YearFrom : "Year : " + vDataInfo.ReportPOS.Rows[0].YearFrom + "-" + vDataInfo.ReportPOS.Rows[0].YearTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                                } else if ($("#selPOSSalesReportRange").val() == "Month") {

                                                    strPrint = '';
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? 'Month: ' + $.format.date(new Date(strFrom + '/1/2013'), 'MMM') : 'From ' + $.format.date(new Date(strFrom + '/1/2013'), 'MMM') + ' To ' + $.format.date(new Date(strTo + '/1/2013'), 'MMM')), intFAFCTotalLenght, '', 0, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataInfo.ReportPOS.Rows[0].YearFrom == vDataInfo.ReportPOS.Rows[0].YearTo ? "Year : " + vDataInfo.ReportPOS.Rows[0].YearFrom : "Year : " + vDataInfo.ReportPOS.Rows[0].YearFrom + "-" + vDataInfo.ReportPOS.Rows[0].YearTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                                } else if ($("#selPOSSalesReportRange").val() == "Quarter") {

                                                    strPrint = '';
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (strFrom == strTo ? (strFrom == '1' ? "1st Quarter" : (strFrom == '2' ? "2nd Quarter" : (strFrom == '3' ? "3rd Quarter" : (strFrom == '4' ? "4th Quarter" : "")))) : 'From ' + (strFrom == '1' ? "1st Quarter" : (strFrom == '2' ? "2nd Quarter" : (strFrom == '3' ? "3rd Quarter" : (strFrom == '4' ? "4th Quarter" : "")))) + ' To ' + (strTo == '1' ? "1st Quarter" : (strTo == '2' ? "2nd Quarter" : (strTo == '3' ? "3rd Quarter" : (strTo == '4' ? "4th Quarter" : ""))))), intFAFCTotalLenght, '', 0, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, (vDataInfo.ReportPOS.Rows[0].YearFrom == vDataInfo.ReportPOS.Rows[0].YearTo ? "Year : " + vDataInfo.ReportPOS.Rows[0].YearFrom : "Year : " + vDataInfo.ReportPOS.Rows[0].YearFrom + "-" + vDataInfo.ReportPOS.Rows[0].YearTo), intFAFCTotalLenght, '', 0, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                                } else if ($("#selPOSSalesReportRange").val() == "Year") {
                                                    strPrint = '';
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Year: ' + strYear, intFAFCTotalLenght, '', 0, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                                } else if ($("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {
                                                    strPrint = '';
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Date : ' + $.format.date(new Date(vDataInfo.ReportPOS.Rows[0].SalesDate), 'MM/dd/yyyy'), intFAFCTotalLenght, '', 0, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                                }

                                                //Get Refund Info
                                                AjaxSend({
                                                    url: "ReportsPOS/getSalesAdjustmentRefundReport",
                                                    data: pData,
                                                    success: function (vDataRefundInfo) {

                                                        if (vDataRefundInfo.ReportPOS.Rows.length > 0) {

                                                            strPrint = '';
                                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, "Sales Refund", intFAFCTotalLenght, '', 0, '', 0, 0);
                                                            strPrint = strPrint + strTemp;

                                                            ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                                            ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                                            var dblAllTotalSales = 0;
                                                            strPrint = ' ';
                                                            for (var p = 0; p <= vDataRefundInfo.ReportPOS.Rows.length - 1; p++) {

                                                                dblAllTotalSales = dblAllTotalSales + parseFloat(vDataRefundInfo.ReportPOS.Rows[p].GrossRefundAmount);

                                                                var strSRVRefund = 'SRV ' + SetPrefixPostfix($.trim((vDataRefundInfo.ReportPOS.Rows[p].SRVNo || '')), 12, '0', 0);
                                                                var strSRVRefundDate = ($.format.date(new Date(vDataRefundInfo.ReportPOS.Rows[p].TransDate), 'MM/dd/yyyy') || '')
                                                                var strRefundAmount = SetPrefixPostfix(AccountingFormat(parseFloat(vDataRefundInfo.ReportPOS.Rows[p].GrossRefundAmount) || 0), intFAFCTotalLenght - 29, ' ', 0);

                                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, strSRVRefund, 16, strSRVRefundDate, 27, strRefundAmount, intFAFCTotalLenght, 1);
                                                                strPrint = strPrint + strTemp;

                                                            };

                                                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                            strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                            strAllTotalSales = SetPrefixPostfix(AccountingFormat(dblAllTotalSales), intFAFCTotalLenght - 17, ' ', 0);

                                                            strPrint = FormatTextToPrint(intFAFCTotalLenght, '              ', 15, strAllTotalSales, intFAFCTotalLenght, '', 0, 1);
                                                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                                        }

                                                        //Get Adjustment SRV
                                                        AjaxSend({
                                                            url: "ReportsPOS/getSalesAdjustmentSRVReport",
                                                            data: pData,
                                                            success: function (vDataSRVInfo) {

                                                                if (vDataSRVInfo.ReportPOS.Rows.length > 0) {
                                                                    strPrint = '';
                                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, "Sales Return (SRV Issued)", intFAFCTotalLenght, '', 0, '', 0, 0);
                                                                    strPrint = strPrint + strTemp;

                                                                    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                                                    ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                                                    var dblAllSRVTotalSales = 0;
                                                                    strPrint = ' ';
                                                                    for (var p = 0; p <= vDataSRVInfo.ReportPOS.Rows.length - 1; p++) {

                                                                        dblAllSRVTotalSales = dblAllSRVTotalSales + parseFloat(vDataSRVInfo.ReportPOS.Rows[p].GrossSrvAmount);

                                                                        var strSRV = 'SRV ' + SetPrefixPostfix($.trim((vDataSRVInfo.ReportPOS.Rows[p].SRVNo || '')), 12, '0', 0);
                                                                        var strSRVDate = ($.format.date(new Date(vDataSRVInfo.ReportPOS.Rows[p].TransDate), 'MM/dd/yyyy') || '')
                                                                        var strSRVAmount = SetPrefixPostfix(AccountingFormat(parseFloat(vDataSRVInfo.ReportPOS.Rows[p].GrossSrvAmount)), intFAFCTotalLenght - 29, ' ', 0);

                                                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, strSRV, 16, strSRVDate, 27, strSRVAmount, intFAFCTotalLenght, 1);
                                                                        strPrint = strPrint + strTemp;

                                                                    };

                                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                                    strAllTotalSales = SetPrefixPostfix(AccountingFormat(dblAllSRVTotalSales), intFAFCTotalLenght - 17, ' ', 0);

                                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '              ', 15, strAllTotalSales, intFAFCTotalLenght, '', 0, 1);
                                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                                                }

                                                                strPrint = '';
                                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Printed by ', 15, ': ' + (vDataHeader.ReportPOS.Rows[0].PrintedBy || ''), intFAFCTotalLenght, '', 0, 1);
                                                                strPrint = strPrint + strTemp;

                                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 15, ': ' + ($.format.date(new Date(vDataHeader.ReportPOS.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intFAFCTotalLenght, '', 0, 1);
                                                                strPrint = strPrint + strTemp;

                                                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 0);
                                                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);
                                                                ReadyTextToPrint('\n', 3, 1, 1, 1, 0, 1, 0, 0, 1);

                                                                PrintText(strPrinterIP);
                                                                ClearCommandBuffer();
                                                            }

                                                        });
                                                    }
                                                });
                                            }
                                        });

                                    }
                                });

                            } else {
                                MessageBox("No available printer for this branch. Please contact your system administrator.",
                                       "Printer",
                                        [
                                            { Text: 'Ok'
                                            }
                                        ]
                                );
                            }


                        }
                    });

                }

                function PrintDesktopSalesReport() {
                    var strTemp = '';
                    var strPrint = '';
                    var pData = {};
                    var strFrom = '';
                    var strTo = '';
                    var strYear = '';

                    //Validate
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


                                            //Get sale sales adjusment info
                                            AjaxSend({
                                                url: "ReportsPOS/getSalesAdjustmentReportInfo",
                                                data: pData,
                                                success: function (vDataInfo) {

                                                    if (vDataInfo.ReportPOS.Rows.length > 0) {

                                                        strPrint = SetTextAlign("Sales Adjustment Summary", 1, intDesktopPrintLenght);
                                                        japplet.append(strPrint + "\n");

                                                        if ($("#selPOSSalesReportRange").val() == "Day") {

                                                            strPrint = SetTextAlign((strFrom == strTo ? 'Date: ' + strFrom : 'From ' + strFrom + ' To ' + strTo), 1, intDesktopPrintLenght);
                                                            japplet.append(strPrint + "\n");

                                                        } else if ($("#selPOSSalesReportRange").val() == "Week") {

                                                            strPrint = SetTextAlign((strFrom == strTo ? 'Week: ' + strFrom : 'From Week ' + strFrom + ' To Week ' + strTo), 1, intDesktopPrintLenght);
                                                            japplet.append(strPrint + "\n");

                                                            strPrint = SetTextAlign((vDataInfo.ReportPOS.Rows[0].YearFrom == vDataInfo.ReportPOS.Rows[0].YearTo ? "Year : " + vDataInfo.ReportPOS.Rows[0].YearFrom : "Year : " + vDataInfo.ReportPOS.Rows[0].YearFrom + "-" + vDataInfo.ReportPOS.Rows[0].YearTo), 1, intDesktopPrintLenght);
                                                            japplet.append(strPrint + "\n");

                                                        } else if ($("#selPOSSalesReportRange").val() == "Month") {

                                                            strPrint = SetTextAlign((strFrom == strTo ? 'Month: ' + $.format.date(new Date(strFrom + '/1/2013'), 'MMM') : 'From ' + $.format.date(new Date(strFrom + '/1/2013'), 'MMM') + ' To ' + $.format.date(new Date(strTo + '/1/2013'), 'MMM')), 1, intDesktopPrintLenght);
                                                            japplet.append(strPrint + "\n");

                                                            strPrint = SetTextAlign((vDataInfo.ReportPOS.Rows[0].YearFrom == vDataInfo.ReportPOS.Rows[0].YearTo ? "Year : " + vDataInfo.ReportPOS.Rows[0].YearFrom : "Year : " + vDataInfo.ReportPOS.Rows[0].YearFrom + "-" + vDataInfo.ReportPOS.Rows[0].YearTo), 1, intDesktopPrintLenght);
                                                            japplet.append(strPrint + "\n");

                                                        } else if ($("#selPOSSalesReportRange").val() == "Quarter") {

                                                            strPrint = SetTextAlign((strFrom == strTo ? (strFrom == '1' ? "1st Quarter" : (strFrom == '2' ? "2nd Quarter" : (strFrom == '3' ? "3rd Quarter" : (strFrom == '4' ? "4th Quarter" : "")))) : 'From ' + (strFrom == '1' ? "1st Quarter" : (strFrom == '2' ? "2nd Quarter" : (strFrom == '3' ? "3rd Quarter" : (strFrom == '4' ? "4th Quarter" : "")))) + ' To ' + (strTo == '1' ? "1st Quarter" : (strTo == '2' ? "2nd Quarter" : (strTo == '3' ? "3rd Quarter" : (strTo == '4' ? "4th Quarter" : ""))))), 1, intDesktopPrintLenght);
                                                            japplet.append(strPrint + "\n");

                                                            strPrint = SetTextAlign((vDataInfo.ReportPOS.Rows[0].YearFrom == vDataInfo.ReportPOS.Rows[0].YearTo ? "Year : " + vDataInfo.ReportPOS.Rows[0].YearFrom : "Year : " + vDataInfo.ReportPOS.Rows[0].YearFrom + "-" + vDataInfo.ReportPOS.Rows[0].YearTo), 1, intDesktopPrintLenght);
                                                            japplet.append(strPrint + "\n");

                                                        } else if ($("#selPOSSalesReportRange").val() == "Year") {

                                                            strPrint = SetTextAlign('Year: ' + strYear, 1, intDesktopPrintLenght);
                                                            japplet.append(strPrint + "\n");

                                                        } else if ($("#selPOSSalesReportRange").val() == "Today" || $("#selPOSSalesReportRange").val() == "Yesterday") {

                                                            strPrint = SetTextAlign('Date : ' + $.format.date(new Date(vDataInfo.ReportPOS.Rows[0].SalesDate), 'MM/dd/yyyy'), 1, intDesktopPrintLenght);
                                                            japplet.append(strPrint + "\n");
                                                        }

                                                        //Get Refund Info
                                                        AjaxSend({
                                                            url: "ReportsPOS/getSalesAdjustmentRefundReport",
                                                            data: pData,
                                                            success: function (vDataRefundInfo) {

                                                                if (vDataRefundInfo.ReportPOS.Rows.length > 0) {

                                                                    strPrint = SetTextAlign("Sales Refund", 1, intDesktopPrintLenght);
                                                                    japplet.append("\n" + strPrint + "\n");

                                                                    var dblAllTotalSales = 0;
                                                                    strPrint = ' ';
                                                                    for (var p = 0; p <= vDataRefundInfo.ReportPOS.Rows.length - 1; p++) {

                                                                        dblAllTotalSales = dblAllTotalSales + parseFloat(vDataRefundInfo.ReportPOS.Rows[p].GrossRefundAmount);

                                                                        var strSRVRefund = 'SRV ' + SetPrefixPostfix($.trim((vDataRefundInfo.ReportPOS.Rows[p].SRVNo || '')), 12, '0', 0);
                                                                        var strSRVRefundDate = ($.format.date(new Date(vDataRefundInfo.ReportPOS.Rows[p].TransDate), 'MM/dd/yyyy') || '')
                                                                        var strRefundAmount = SetPrefixPostfix(AccountingFormat(parseFloat(vDataRefundInfo.ReportPOS.Rows[p].GrossRefundAmount) || 0), intDesktopPrintLenght - 29, ' ', 0);

                                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, strSRVRefund, 16, strSRVRefundDate, 27, strRefundAmount, intDesktopPrintLenght);
                                                                        japplet.append(strPrint + "\n");

                                                                    };

                                                                    strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                                                    japplet.append(strPrint + "\n");

                                                                    var strAllTotalSales = SetPrefixPostfix(AccountingFormat(dblAllTotalSales), intDesktopPrintLenght - 17, ' ', 0);

                                                                    strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, '               ', 14, strAllTotalSales, intDesktopPrintLenght, '', 0);
                                                                    japplet.append(strPrint + "\n");

                                                                }

                                                                //Get Adjustment SRV
                                                                AjaxSend({
                                                                    url: "ReportsPOS/getSalesAdjustmentSRVReport",
                                                                    data: pData,
                                                                    success: function (vDataSRVInfo) {

                                                                        if (vDataSRVInfo.ReportPOS.Rows.length > 0) {

                                                                            strPrint = SetTextAlign("Sales Return (SRV Issued)", 1, intDesktopPrintLenght);
                                                                            japplet.append("\n" + strPrint + "\n");

                                                                            var dblAllSRVTotalSales = 0;
                                                                            strPrint = ' ';
                                                                            for (var p = 0; p <= vDataSRVInfo.ReportPOS.Rows.length - 1; p++) {

                                                                                dblAllSRVTotalSales = dblAllSRVTotalSales + parseFloat(vDataSRVInfo.ReportPOS.Rows[p].GrossSrvAmount);

                                                                                var strSRV = 'SRV ' + SetPrefixPostfix($.trim((vDataSRVInfo.ReportPOS.Rows[p].SRVNo || '')), 12, '0', 0);
                                                                                var strSRVDate = ($.format.date(new Date(vDataSRVInfo.ReportPOS.Rows[p].TransDate), 'MM/dd/yyyy') || '')
                                                                                var strSRVAmount = SetPrefixPostfix(AccountingFormat(parseFloat(vDataSRVInfo.ReportPOS.Rows[p].GrossSrvAmount)), intDesktopPrintLenght - 29, ' ', 0);

                                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, strSRV, 16, strSRVDate, 27, strSRVAmount, intDesktopPrintLenght);
                                                                                japplet.append(strPrint + "\n");

                                                                            };

                                                                            strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                                                            japplet.append(strPrint + "\n");

                                                                            var strAllTotalSales = SetPrefixPostfix(AccountingFormat(dblAllSRVTotalSales), intDesktopPrintLenght - 17, ' ', 0);

                                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, '               ', 14, strAllTotalSales, intDesktopPrintLenght, '', 0);
                                                                            japplet.append(strPrint + "\n");
                                                                        }

                                                                        japplet.append("\n");
                                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Printed by ', 0, 14), 14, ': ' + (vDataHeader.ReportPOS.Rows[0].PrintedBy || ''), intDesktopPrintLenght, '', 0);
                                                                        japplet.append(strPrint + "\n");

                                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Date and Time', 0, 14), 14, ': ' + ($.format.date(new Date(vDataHeader.ReportPOS.Rows[0].PrintDateTime), 'MMM dd, yyyy HH:mm') || ''), intDesktopPrintLenght, '', 0);
                                                                        japplet.append(strPrint + "\n");

                                                                        japplet.append("\n\n\n\n\n\n\n\n\n\n\n\n\n");
                                                                        japplet.print();

                                                                    }

                                                                });
                                                            }
                                                        });
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
            });
        }); //require(reqJS, function () {
    });