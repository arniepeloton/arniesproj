require([
        'common',
        'promise!/Scripts3/Browser/LibraryFowst.js?crc='
], function () {
    var m_Data;
    var m_DashBoardChartsData;
    var GroupDateBy;
    var IsSalesDistribution = true;

    var GroupDateBy; //para madisplay ang date sa chart
    var m_Ticks;
    var m_TicksBar;
    var m_Labels;

    var h_Data; // header chart

    $(function () {


        //$('#pageMain').attr('class', 'ui-page ui-body-c ui-page-active');
        //if (!CBool(sessionStorage.PrintingOfflineData)) {
        //    GetAllFeatures();
        DisplayOrganizationProductFeatures();
        //}

        //sessionStorage.SearchMode = 'false';
        //$('.txtSearch').val('');


        $('#liSecondChart').click(function () {
            $('#panSalesDistribution').popup('open');
        });


        $('#liThirdChartLabel').click(function () {
            $('#panDice').popup('open');
        });

        $('#dvMainChart').click(function () {
            if (IsSalesDistribution) {
                $('#panSalesDistribution').popup('open');
            }
        });

        $('#dvSecondChart').bind('jqplotDataClick',
            function (ev, seriesIndex, pointIndex, data) {
//                m_Filter1 = m_TicksBar[pointIndex];

//                $('#panDice').popup('open');

//                $('#liThirdChartMainLabel').text('Showing Sales>' + m_Filter1 + '>');

//                ShowHideDiceList();

            }
        );

        //dire ni sya kay ang value sa fowst depends on clicking sa chart
        //$('#lnkTransactionDetails').click(function () {
        //    $('#txtSearchProduct').val('');

        //    GetTransactionDetails(m_DashBoardChartsData);
        //});

        //dire ni sya kay ang value sa fowst depends on clicking sa chart
        //$('#txtSearchProduct').keypress(function (e) {
        //    if (e.keyCode == 13) {
        //        m_DashBoardChartsData.IsSearch = 1;
        //        m_DashBoardChartsData.SearchString = $('#txtSearchProduct').val();
        //        GetTransactionDetails(m_DashBoardChartsData);
        //    }
        //});

        //$('.txtSearch').keypress(function (e) {
        //    if (e.keyCode == 13) {
        //        $('.btnSearch').click();
        //    }
        //});

        //$('.btnSearch').click(function () {

        //    BrowseAddresses();

        //})
        //$('.txtSearch').keypress(function (e) {
        //    if (e.keyCode == 13) {

        //BrowseAddresses();

        //    }
        //});


    });

    window.GetPOSSalesReportByTrendOrSalesDistribution = function (data) {
        m_DashBoardChartsData = data;
        GroupDateBy = 'Day';
        ClearCharts();

        AjaxSend({
            url: 'DashboardsAndReports/GetPOSSalesReportByTrendOrSalesDistribution',
            data: data,
            success: function (vData) {
                if (vData.ReportPOS.Rows.length > 0) {
                    $('#IsDataPresent').css('visibility', 'hidden');
                    $('#lblMainChartInstruction').css('visibility', 'visible');
                    $('#lblMainChartInstruction').text('* Click the chart to breakdown data');

                    m_Data = vData.ReportPOS.Rows;
                    DisplayTrend();
                }
                else {
                    $('#IsDataPresent').css('visibility', 'visible');
                    $('#lblMainChartInstruction').css('visibility', 'hidden');


                }
            }
        }); //end ajaxsend
    }

    window.GetTotalForAllFowst = function (data) {
        h_Data = data;

        $('#dvGroupChart').empty('').text('Loading chart ...');

        AjaxSend({
            url: 'TenantMonitoringSystem/GetTotalForAllFowst',
            data: data,
            success: function (vData) {
                $('#dvGroupChart').empty('');

                var tGrossSales = [];
                if (vData.Results.Rows.length > 0) {
                    for (var i = 0; i < vData.Results.Rows.length; i++) {
                        var sales = CDec2(vData.Results.Rows[i].GrossSales);
                        var date;
                        var name;

                        if (data.GraphType == "Trend") {
                            var date = $.format.date(new Date(vData.Results.Rows[i].InvoiceDate), 'MM/dd');
                            tGrossSales.push([date, sales]);
                        }
                        else {
                            if(data.GroupBy == "Hour"){
                                name = vData.Results.Rows[i].Name + ":00-" + $.format.date(new Date('01-01-2015 ' + vData.Results.Rows[i].Ends), 'HH:mm');
                            }
                            else {
                                name = vData.Results.Rows[i].Name
                            }
                            tGrossSales.push([sales, name]);
                        }
                    }

                    if (data.GraphType == "Trend") {
                        DisplayHeaderChart(tGrossSales);
                    }
                    else {
                        DisplayHeaderChartDistribution(tGrossSales);
                    }
                }
                else {
                    $('#dvGroupChart').css('height', 0).text('No Result(s) Found');
                }
            }
        })
    }

    window.DistributeHeaderChart = function (gBy) {
        DistributeHeaderChart(gBy);
    }

    function DisplayHeaderChartDistribution(arrDis) {
        $('#dvGroupChart').empty('').css('height', '400px');
        $.jqplot('dvGroupChart', [arrDis], {
            animate: true,
            captureRightClick: true,
            seriesDefaults: {
                renderer: $.jqplot.BarRenderer,
                shadowAngle: 135,
                rendererOptions: {
                    barDirection: 'horizontal',
                    highlightMouseDown: true
                },
                pointLabels: { show: true, formatString: '%d' }
            },
            axes: {
                yaxis: {
                    renderer: $.jqplot.CategoryAxisRenderer
                }
            }
        });
    }

    function DisplayHeaderChart(data) {
        $('#dvGroupChart').empty('').css('height', '400px');

        $.jqplot('dvGroupChart', [data], {
            animate: true,
            animateReplot: true,
            axes: {
                xaxis: {
                    renderer: $.jqplot.CategoryAxisRenderer
                },
                yaxis: {
                    min: 0
                }
            },
            highlighter: {
                show: true,
                showLabel: true,
                tooltipAxes: 'y'
            }
        });

        $('#dvGroupChart').click(function () {
            return false;
            PopupButtonList('', 'Filter By', [
                {
                    Text: 'Class Code',
                    Click: function () {
                        $('#selDistributionOption').val('Class Code');
                        $('#popChart, #divMainDistribution').show();
                        $('#divSecondaryDistribution').hide();
                        DistributeHeaderChart('Class Code');
                    }
                },
                {
                    Text: 'Trade Code',
                    Click: function () {
                        $('#selDistributionOption').val('Trade Code');
                        $('#popChart, #divMainDistribution').show();
                        $('#divSecondaryDistribution').hide();
                        DistributeHeaderChart('Trade Code');
                    }
                },
                {
                    Text: 'Sales Type',
                    Click: function () {
                        $('#selDistributionOption').val('Sales Type');
                        $('#popChart, #divMainDistribution').show();
                        $('#divSecondaryDistribution').hide();
                        DistributeHeaderChart('Sales Type');
                    }
                }
            ], { AutoDismiss: true })
        })
    }

    function DistributeHeaderChart(groupby) {
        h_Data.GroupBy = groupby;

        $('#divDistributionChart').empty().text('Loading chart ...');

        AjaxSend({
            url: 'TenantMonitoringSystem/GetTotalForAllFowst',
            data: h_Data,
            success: function (vData) {
                var arrDis = [];

                $('#divDistributionChart').empty();

                if (vData.Results.Rows.length > 0) {
                    for (var i = 0; i < vData.Results.Rows.length; i++) {
                        arrDis.push([CDec2(vData.Results.Rows[i].GrossSales), vData.Results.Rows[i].Name]);
                    }                   

                    $.jqplot('divDistributionChart', [arrDis], {
                        animate: true,
                        captureRightClick: true,
                        seriesDefaults: {
                            renderer: $.jqplot.BarRenderer,
                            shadowAngle: 135,
                            rendererOptions: {
                                barDirection: 'horizontal',
                                highlightMouseDown: true
                            },
                            pointLabels: { show: true, formatString: '%d' }
                        },
                        axes: {
                            yaxis: {
                                renderer: $.jqplot.CategoryAxisRenderer
                            }
                        }
                    });

                    $('#divDistributionChart').unbind('jqplotDataClick').bind('jqplotDataClick',
                        function (ev, seriesIndex, pointIndex, data) {
                            PopupButtonList('', 'Graph Type', [
                                {
                                    Text: 'Sales Trend',
                                    Click: function () {
                                        $('#divMainDistribution').hide();
                                        $('#divSecondaryDistribution').show();
                                        GetTrendByFOWST($('#selDistributionOption').val(), data[1]);
                                    }
                                },
                                {
                                    Text: 'Sales Distribution',
                                    Click: function () {
                                        $('#divMainDistribution').hide();
                                        $('#divSecondaryDistribution').show();
                                        GetDistibutionByFOWST($('#selDistributionOption').val(), data[1]);
                                    }
                                }
                            ])
                        }
                    );

                    $('#btnBackDistribution').click(function () {
                        $('#divSecondaryDistribution').hide();
                        $('#divMainDistribution').show();
                    })
                }
                else {
                    $('#divDistributionChart').text('No Result(s) Found');
                }
            }
        })
    }

    function GetTrendByFOWST(groupby, FilVal) {
        h_Data.GroupBy = groupby;
        h_Data.FilVal = FilVal;

        $('#divSecondDistributionChart').empty().text('Loading chart ...');

        AjaxSend({
            url: 'TenantMonitoringSystem/GetTrendByFOWST',
            data: h_Data,
            success: function (vData) {
                $('#divSecondDistributionChart').empty();

                if (vData.Results == 0) {
                    $('#divSecondDistributionChart').text('No Result(s) Found');
                }
                else {
                    if (vData.Results.length > 0) {
                        var grid = $('<div class="ui-grid-a ui-responsive" style="height:300px;">');
                        var ctrGrid = 1;
                        var newGrid = true;
                        var block;

                        for (var i = 0; i < vData.Results.length; i++) {
                            var fowst = JSON.parse(vData.Results[i]);
                            var arrTrend = [];

                            if (ctrGrid == 2) {
                                block = 'b';
                                newGrid = true;
                                ctrGrid = 1;
                            }
                            else {
                                block = 'a';
                            }

                            grid.append($('<div id="div_' + fowst.d.Rows[0].FowstID + '" class="ui-block-' + block + '"  style="height:300px;">'))
                            //$('#divSecondDistributionChart').append('<div id="div_' + fowst.d.Rows[0].FowstID + '"></div><br>')

                            for (var m = 0; m < fowst.d.Rows.length; m++) {
                                arrTrend.push([$.format.date(new Date(fowst.d.Rows[m].InvoiceDate), 'MM/dd'), CDec2(fowst.d.Rows[m].GrossSales)])
                            }

                            if (newGrid) {
                                $('#divSecondDistributionChart').append(grid)
                            }

                            $.jqplot('div_' + fowst.d.Rows[0].FowstID, [arrTrend], {
                                animate: true,
                                animateReplot: true,
                                axes: {
                                    xaxis: {
                                        renderer: $.jqplot.CategoryAxisRenderer,
                                        label: fowst.d.Rows[0].FowstName
                                    },
                                    yaxis: {
                                        min: 0
                                    }
                                },
                                highlighter: {
                                    show: true,
                                    showLabel: true,
                                    tooltipAxes: 'y'
                                }
                            });

                            ctrGrid = ctrGrid + 1;
                            newGrid = false;
                        }
                    }
                }
            }
        })
    }

    function GetDistibutionByFOWST(groupby, FilVal) {
        h_Data.GroupBy = groupby;
        h_Data.FilVal = FilVal;

        $('#divSecondDistributionChart').empty().text('Loading chart ...');

        AjaxSend({
            url: 'TenantMonitoringSystem/GetDistibutionByFOWST',
            data: h_Data,
            success: function (vData) {
                var arrDis = [];
                if (vData.Results.Rows.length > 0) {
                    for (var i = 0; i < vData.Results.Rows.length; i++) {
                        arrDis.push([CDec2(vData.Results.Rows[i].GrossSales), vData.Results.Rows[i].FowstName]);
                    }

                    $('#divSecondDistributionChart').empty();

                    $.jqplot('divSecondDistributionChart', [arrDis], {
                        animate: true,
                        captureRightClick: true,
                        seriesDefaults: {
                            renderer: $.jqplot.BarRenderer,
                            shadowAngle: 135,
                            rendererOptions: {
                                barDirection: 'horizontal',
                                highlightMouseDown: true
                            },
                            pointLabels: { show: true, formatString: '%d' }
                        },
                        axes: {
                            yaxis: {
                                renderer: $.jqplot.CategoryAxisRenderer
                            }
                        }
                    });
                }
                else {
                    $('#divSecondDistributionChart').text('No chart to shown');
                }
            }
        })
    }

    //function BrowseAddresses() {

    //    Browser.LibraryFowst.InputData.OrganizationID = sessionStorage.OrgID;
    //    Browser.LibraryFowst.InputData.FilterFowst = 1;
    //    Browser.LibraryFowst.Click = function (e) {
    //        sessionStorage.FowstID = e.ID;
    //        sessionStorage.SearchMode = 'true';
    //        $('#txtSearchProduct').val('');
    //        window.GetData();
    //    };
    //    Browser.LibraryFowst.Show();

    //};

    function DisplayTrend() {
        var m_Totals = [];
        var m_LandedTotals = [];
        var m_GrossProfit = [];
        var m_Days = [];
        var m_Months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
        var TotalSales = 0;
        var TotalSalesToday = 0;
        var TotalDiscounts = 0;
        var tData;

        //m_Totals[0] = [];
        //m_LandedTotals[0] = [];
        //m_GrossProfit[0] = [];

        var pLV = $('#tblMainChart').html('');

        tData = $('<tr class="custom-theme-f">')
                        .append($('<th style="text-align:left">')
                            .text('Date')
                        )
                        .append($('<th style="text-align:right">')
                            .text('Gross Sales')
                        )
                        .append($('<th style="text-align:right">')
                            .text('Discount')
                        )
                        .append($('<th style="text-align:right">')
                            .text('Refunds and Exchanges')
                        )
                        .append($('<th style="text-align:right">')
                            .text('Net Sales')
                        )
                        .append($('<br />'))

        pLV.append(tData);

        if (m_Data.length > 0) {

            for (var i = 0; i < m_Data.length; i++) {

                var m_Date = new Date(m_Data[i].InvoiceDate);
                var m_Today = new Date();

                m_Totals[i] = $.format.number(m_Data[i].Total);
                m_LandedTotals[i] = $.format.number(m_Data[i].TotalLandedCost);
                m_GrossProfit[i] = $.format.number(m_Data[i].GrossProfit);

                TotalSales = TotalSales + parseFloat(m_Data[i].Total);
                TotalDiscounts = TotalDiscounts + parseFloat(m_Data[i].DiscountTotal);

                if ($.format.date(m_Today, "MM/dd/yyyy") == $.format.date(new Date(m_Data[i].InvoiceDate), "MM/dd/yyyy")) {
                    TotalSalesToday += parseFloat(m_Data[i].Total);
                }

                if (GroupDateBy == 'Day') {
                    m_Days[i] = (m_Date.getMonth() + 1) + '/' + m_Date.getDate();
                    $('#lblDateFormatMessage').css('visibility', 'hidden');
                }
                else if (GroupDateBy == 'Month') {
                    m_Days[i] = m_Months[m_Data[i].InvoiceDate - 1];
                    $('#lblDateFormatMessage').text('Date Format: Month/Date');
                    $('#lblDateFormatMessage').css('visibility', 'hidden');
                }

                tData = $('<tr>')
                        .append($('<th style="text-align:left">')
                            .text(m_Months[m_Date.getMonth()] + ' ' + m_Date.getDate() + ', ' + m_Date.getFullYear())
                        )
                        .append($('<th style="text-align:right">')
                            .text(AccountingFormat(m_Data[i].Total))
                        )
                        .append($('<th style="text-align:right">')
                            .text(AccountingFormat(m_Data[i].DiscountTotal))
                        )
                        .append($('<th style="text-align:right">')
                            .text(AccountingFormat(m_Data[i].TotalReturns))
                        )
                        .append($('<th style="text-align:right">')
                            .text(AccountingFormat(parseFloat(m_Data[i].Total) - parseFloat(m_Data[i].DiscountTotal) - parseFloat(m_Data[i].TotalReturns)))
                        )
                        .append($('<br />'))

                pLV.append(tData);

            }

            $('#lblMainChartInstruction').css('visibility', 'visible');
            $('#lblMainChartInstruction').text('* Click the chart to breakdown data');

            m_Ticks = m_Days;

            DisplayLineGraph(m_Totals, m_LandedTotals, m_GrossProfit);
        }
    }

    function DisplayLineGraph(m_Totals, m_LandedTotals, m_GrossProfit) {

        $('#dvMainChart').empty();

        var plot1 = $.jqplot('dvMainChart', [m_Totals, m_LandedTotals, m_GrossProfit], {
            //seriesColors: ["rgb(78, 135, 194)", "rgb(211, 235, 59)", "rgb(111, 135, 29)"],
            animate: true,
            animateReplot: true,
            axes: {
                xaxis: {
                    renderer: $.jqplot.CategoryAxisRenderer,
                    ticks: m_Ticks
                },
                yaxis: {
                    min: 0
                    //max: 500000
                }
            },
            highlighter: {
                show: true,
                showLabel: true,
                tooltipAxes: 'y',
                sizeAdjust: 7.5, tooltipLocation: 'ne',
                tooltipContentEditor: function (str, seriesIndex, pointIndex, jqPlot) {
                    return AccountingFormat(m_Totals[seriesIndex][pointIndex]);
                }
            },
            //series: [
            //            {
            //                label: 'Gross Sales'
            //            },
            //            {
            //                label: 'Landed Cost'
            //            },
            //            {
            //                label: 'Gross Profit'
            //            }
            //],
            //legend: {
            //    show: true,
            //    placement: 'inside'
            //}
        });

        //$('#tblDateFormatLabel').css('visibility', 'visible')
    }

    //function GetAllFeatures() {
    //    AjaxSend({
    //        url: 'DashboardsAndReports/GetOrganizationProductFeatures',
    //        success: function (vData) {
    //            DisplayOrganizationProductFeatures(vData.Results.Rows);
    //        }
    //    });
    //}

    function DisplayOrganizationProductFeatures(vRows) {
        var pLV = $('#lstSlice');
        var pLV2 = $('#lstDice');

        //brand slice
        pItem = $('<li>')
            .append($('<a>')
                .attr('ID', 'SliceBrand')
                .attr('name', 'Day')
                .click(function (e) {
                    m_Slice = $(this).attr('name');
                    SliceData(m_Slice);
                    ShowHideDiceList();
                    $('#panSalesDistribution').popup('close');
                })
                .text('Day')
            );

        pLV.append(pItem);

        //brand dice
        pItem = $('<li>')
            .append($('<a>')
                .attr('ID', 'DiceBrand')
                .attr('name', 'Day')
                .click(function (e) {
                    DiceData($(this).attr('name'), 'tblThirdChart', 'dvThirdChart', 'dvDice');
                    $('#panDice').popup('close');
                })
                .text('Day')
            );

        pLV2.append(pItem);

        //sales associate slice
        pItem = $('<li>')
            .append($('<a>')
                .attr('ID', 'SliceSalesRepresentative')
                .attr('name', 'Hour')
                .click(function (e) {
                    m_Slice = $(this).attr('name');
                    SliceData(m_Slice);
                    ShowHideDiceList();
                    $('#panSalesDistribution').popup('close');
                })
                .text('Hour')
            );

        pLV.append(pItem);


        //sales associate dice
        pItem = $('<li>')
            .append($('<a>')
                .attr('ID', 'DiceSalesRepresentative')
                .attr('name', 'Hour')
                .click(function (e) {
                    DiceData($(this).attr('name'), 'tblThirdChart', 'dvThirdChart', 'dvDice');
                    $('#panDice').popup('close');
                })
                .text('Hour')
            );

        pLV2.append(pItem);

        try {
            pLV.listview('refresh');
            pLV2.listview('refresh');
        }
        catch (e) {
            pLV.listview();
            pLV2.listview();
        }
    }

    window.SliceData = function (SliceBy) {
        switch (SliceBy) {
            //case "Day":                                                                                     
            //    m_DashBoardChartsData.SliceBy = 'Day';                                                                                     
            //    break;                                                                                     
            //case "Brand":                                                                                     
            //    m_DashBoardChartsData.SliceBy = 'optBrand';                                                                                     
            //    window.location = '#dvSlice';                                                                                     
            //    break;                                                                                     
            //case "Division":                                                                                     
            //    m_DashBoardChartsData.SliceBy = 'optDivisionDescription';                                                                                     
            //    break;                                                                                     
            //    //additional by janine oct 30 12pm                                                                                                        
            //case "Product":                                                                                     
            //    m_DashBoardChartsData.SliceBy = 'optProduct';                                                                                     
            //    break;                                                                                     
            //case "Delivery Method":                                                                                     
            //    m_DashBoardChartsData.SliceBy = 'optDeliveryMethod';                                                                                     
            //    break;                                                                                     
            //case "Order Method":                                                                                     
            //    m_DashBoardChartsData.SliceBy = 'optOrderMethod';                                                                                     
            //    break;                                                                                     
            //case "Gender":                                                                                     
            //    m_DashBoardChartsData.SliceBy = 'optGender';                                                                                     
            //    break;                                                                                     
            //case "Age":                                                                                     
            //    m_DashBoardChartsData.SliceBy = 'optAge';                                                                                     
            //    break;                                                                                     
            //    //end of additional                                                                                                  
            //case "Season/Year":                                                                                     
            //    m_DashBoardChartsData.SliceBy = 'Season/Year';                                                                                     
            //    break;                                                                                     
            //case "SalesRepresentative":                                                                                     
            //    m_DashBoardChartsData.SliceBy = 'optSalesRepresentative';                                                                                     
            //    break;                                                                                     
            case "Date":
                m_DashBoardChartsData.GroupBy = 'Date';
                m_DashBoardChartsData.SliceBy = 'Date';
                break;
            case "Hour":
                m_DashBoardChartsData.GroupBy = 'Hour';
                m_DashBoardChartsData.SliceBy = 'Hour';
                break;
            case "Day":
                m_DashBoardChartsData.GroupBy = 'Day';
                m_DashBoardChartsData.SliceBy = 'Day';
                break;
            default:
                m_DashBoardChartsData.SliceBy = SliceBy;
                break;
        }

        window.location = '#dvSlice';

        $('#dvSlice').css('visibility', 'visible');

        $('#tblThirdChart').empty();
        $('#dvThirdChart').empty();
        $('#dvDice').css('visibility', 'hidden');

        $('#liSecondChartMainLabel').text('Showing Sales>');
        $('#liSecondChart').text(SliceBy);
        $('#lblSecondChartInstruction').text('* Click on a ' + SliceBy + ' to break down data')

        GetPOSSalesReportBySlice();
    }

    function GetPOSSalesReportBySlice() {
        var m_Totals = [];
        var m_TotalGross = 0;
        var m_TotalTransactionCount = 0;
        var m_TotalProductCount = 0;

        AjaxSend({
            //url: 'DashboardsAndReports/GetPOSSalesReportBySlice',
            url: 'TenantMonitoringSystem/GetPOSSalesReportBySalesDistributionByTime',
            data: m_DashBoardChartsData,
            success: function (vData) {
                if (vData.ReportPOS.Rows.length > 0) {

                    var vRows = vData.ReportPOS.Rows;

                    $('#btnDownloadSliceAsExcel').css('visibility', 'visible');
                    m_Totals[0] = [];
                    m_TicksBar = [];
                    m_Labels = [];
                    var mDays = [];
                    var tData = '';
                    var i = 0;
                    var m_Date;

                    $('#tblSecondChart').html('');

                    if(m_Slice == "Hour"){
                        $('#dvSecondChart').css('height', '400px');
                    }
                    else {
                        $('#dvSecondChart').css('height', '0px');
                    }

                    //additional by janine. show count. october 30 5pm

                    for (i = 0; i <= vRows.length - 1; i++) {
                        m_Date = new Date(vRows[i].InvoiceDate);
                        mDays[i] = (m_Date.getMonth() + 1) + '/' + m_Date.getDate();
                        //m_Totals[0][i] = $.format.number(vRows[i].TransactionCount, vRows[i].InvoiceDate);
                        //m_Totals[0][i] = $.format.number(vRows[i].InvoiceDate); // excluded week day
                        m_TotalTransactionCount = m_TotalTransactionCount + CDec2(vRows[i].TransactionCount);

                        m_TotalProductCount = m_TotalProductCount + (CDec2(vRows[i].ProductCount) == 0 ? CDec2(vRows[i].ProductCountSO) : CDec2(vRows[i].ProductCount));
                    }
                    //for chart, it will sort desc
                    m_Totals[0] = [];
                    for (i = 0; i <= vRows.length - 1; i++) {

                        //m_Totals[1][i] = $.format.number(vRows[i].Total, vRows[i].InvoiceDate);
                        m_Totals[0][i] = $.format.number(vRows[i].Total);
                        m_TotalGross = m_TotalGross + $.format.number(vRows[i].Total);
                    }
                    //end of additional

                    if (m_DashBoardChartsData.GroupBy == 'Date') {
                        m_Ticks = m_Date;
                        DisplaySliceGroupChart(m_Totals);
                        return;
                    } else {

                        tData = $('<tr class="custom-theme-f">')
                            .append($('<th>')
                                .text(m_Slice)
                            )
                            .append($('<th style="text-align:right">')
                                .text('Gross Sales')
                            )
                            .append($('<th style="text-align:right">')
                                .text('Contribution Margin')
                            )

                            .append($('<br />'))

                        $('#tblSecondChart').append(tData);

                        //manually sort desc for table details
                        for (i = vRows.length - 1; i >= 0; i--) {

                            m_Labels[i] = AccountingFormat($.format.number(vRows[i].Total));

                            var mHourOrDay;
                            var mDays = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

                            if (m_Slice == 'Hour') {
                                mHourOrDay = vRows[i].InvoiceDate + ':00-' + $.format.date(new Date('01-01-2015 ' + vRows[i].Ends), 'HH:mm');
                                m_TicksBar[i] = vRows[i].InvoiceDate + ':00-' + $.format.date(new Date('01-01-2015 ' + vRows[i].Ends), 'HH:mm');
                            } else {
                                mHourOrDay = mDays[CInt2(vRows[i].InvoiceDate) - 1]; //-1 because sunday in vb starts at 1, sunday in js starts at 0
                                m_TicksBar[i] = mDays[vRows[i].InvoiceDate - 1];
                            }

                            tData = $('<tr>')
                                .attr('data', vRows[i])
                                .append($('<td>')
                                    .text(mHourOrDay)
                                )
                                .append($('<td style="text-align:right">')
                                    .text(AccountingFormat($.format.number(vRows[i].Total)))
                                )
                                .append($('<td style="text-align:right">')
                                    .text(AccountingFormat((parseFloat(vRows[i].Total) / parseFloat(m_TotalGross)) * 100) + '%')
                                )

                                .click(function () {
//                                    m_Filter1 = $(this).attr('data');
//                                    $('#panDice').popup('open');
//                                    $('#liThirdChartMainLabel').text('Showing Sales>' + $(this).attr('data') + '>');
//                                    ShowHideDiceList();
                                })

                            $('#tblSecondChart').append(tData);
                        }

                        tData = $('<tr>')
                            .append($('<td colspan="7">')
                                .append($('<hr>'))
                            )

                        $('#tblSecondChart').append(tData);

                        tData = $('<tr>')
                            .append($('<td>')
                                .text('Total')
                            )
                           .append($('<td style="text-align:right">')
                                .text(AccountingFormat(m_TotalGross))
                            )
                            .append($('<td style="text-align:right">')
                            )


                        $('#tblSecondChart').append(tData);

                        DisplaySliceBarGraph(m_Totals);
                    }
                }
            }
        }); //end ajaxsend
    }

    //for Group Chart
    function DisplaySliceGroupChart(m_Totals) {

        $('#dvGroupChart').empty('');

        var plot1 = $.jqplot('dvGroupChart', m_Totals, {
            animate: true,
            animateReplot: true,
            axes: {
                xaxis: {
                    renderer: $.jqplot.CategoryAxisRenderer,
                    ticks: m_Ticks
                },
                yaxis: {
                    min: 0
                    //max: 500000
                }
            },
            highlighter: {
                show: true,
                showLabel: true,
                tooltipAxes: 'y',
                sizeAdjust: 7.5, tooltipLocation: 'ne',
                tooltipContentEditor: function (str, seriesIndex, pointIndex, jqPlot) {
                    return AccountingFormat(m_Totals[seriesIndex][pointIndex]);
                }
            }
        });

        //var plot3 = $.jqplot('dvGroupChart', m_Totals, {
        //    animate: !$.jqplot.use_excanvas,
        //    seriesDefaults: {
        //        renderer: $.jqplot.BarRenderer,
        //        pointLabels: {
        //            show: true
        //            //                        labels: m_Labels
        //        },
        //        rendererOptions: {
        //            barDirection: 'vertical'
        //        }
        //    },
        //    axes: {
        //        xaxis: {
        //            drawBaseline: false
        //        },
        //        yaxis: {
        //            renderer: $.jqplot.CategoryAxisRenderer,
        //            ticks: m_TicksBar
        //        }
        //    },
        //    highlighter: { show: false }
        //});

        $('#dvGroupChart .jqplot-xaxis-tick').css('display', 'none');
    }


    //for 2nd chart
    function DisplaySliceBarGraph(m_Totals) {
        $('#dvSecondChart').empty('');

        var plot3 = $.jqplot('dvSecondChart', m_Totals, {
            animate: !$.jqplot.use_excanvas,
            seriesDefaults: {
                renderer: $.jqplot.BarRenderer,
                pointLabels: {
                    show: true
                    //                        labels: m_Labels
                },
                rendererOptions: {
                    barDirection: 'horizontal'
                }
            },
            axes: {
                xaxis: {
                    drawBaseline: false
                },
                yaxis: {
                    renderer: $.jqplot.CategoryAxisRenderer,
                    ticks: m_TicksBar
                }
            },
            highlighter: { show: false }
        });

        $('#dvSecondChart .jqplot-xaxis-tick').css('display', 'none');
    }

    function ShowHideDiceList() {
        $('#lstDice li a').each(function () {
            $(this).show();
        });

        $('#Dice' + m_Slice.replace(' ', '').replace('(', '').replace(')', '').replace('/', '').replace('\'', '').replace('Slice', '')).hide();

        //$('#lstDice').listview('refresh');
    }

    function DiceData(DiceBy, table, divChart, div) {
        var data;
        var Fowst;

        //Fowst = GetSelectedFowst();

        if (Fowst == 0) {
            MessageBox("Please select FOWST",
                            "FOWST",
                            [
                                {
                                    Text: 'Ok'
                                }
                            ]
                        );
            $('#selDice').val('');
            $('#selDice').selectmenu('refresh');
            return;
        }

        switch (DiceBy) {
            case "Brand":
                m_DashBoardChartsData.SliceBy = 'optBrand';
                m_DashBoardChartsData.Filter1Option = m_Slice;
                m_DashBoardChartsData.Filter1 = m_Filter1;
                window.location = '#' + divChart;
                break;
            case "Division":
                m_DashBoardChartsData.SliceBy = 'optDivisionDescription';
                m_DashBoardChartsData.Filter1Option = m_Slice;
                m_DashBoardChartsData.Filter1 = m_Filter1;
                window.location = '#' + divChart;
                break;
            case "SalesRepresentative":
                m_DashBoardChartsData.SliceBy = 'optSalesRepresentative';
                m_DashBoardChartsData.Filter1Option = m_Slice;
                m_DashBoardChartsData.Filter1 = m_Filter1;
                window.location = '#' + divChart;
                break;
            //additional by janine nov 15                                                                                                
            case "Product":
                m_DashBoardChartsData.SliceBy = 'optProduct';
                m_DashBoardChartsData.Filter1Option = m_Slice;
                m_DashBoardChartsData.Filter1 = m_Filter1;
                window.location = '#' + divChart;
                break;
            case "Delivery Method":
                m_DashBoardChartsData.SliceBy = 'optDeliveryMethod';
                m_DashBoardChartsData.Filter1Option = m_Slice;
                m_DashBoardChartsData.Filter1 = m_Filter1;
                window.location = '#' + divChart;
                break;
            case "Order Method":
                m_DashBoardChartsData.SliceBy = 'optOrderMethod';
                m_DashBoardChartsData.Filter1Option = m_Slice;
                m_DashBoardChartsData.Filter1 = m_Filter1;
                window.location = '#' + divChart;
                break;
            case "Gender":
                m_DashBoardChartsData.SliceBy = 'optGender';
                m_DashBoardChartsData.Filter1Option = m_Slice;
                m_DashBoardChartsData.Filter1 = m_Filter1;
                window.location = '#' + divChart;
                break;
            case "Age":
                m_DashBoardChartsData.SliceBy = 'optAge';
                m_DashBoardChartsData.Filter1Option = m_Slice;
                m_DashBoardChartsData.Filter1 = m_Filter1;
                window.location = '#' + divChart;
                break;
            //end of additional                                                                                                
            default:
                m_DashBoardChartsData.SliceBy = DiceBy;
                m_DashBoardChartsData.Filter1Option = m_Slice;
                m_DashBoardChartsData.Filter1 = m_Filter1;
                window.location = '#' + divChart;
        }

        $('#' + div).css('visibility', 'visible');

        switch (DiceBy) {
            case 'Size ID':
                m_FilterLabel = 'Size';
                break;
            case 'Sport Activity':
                m_FilterLabel = 'Sport';
                break;
            case 'Gender Age':
                m_FilterLabel = 'Gender';
                break;
            case 'Season/Year':
                m_FilterLabel = 'Season';
                break;
            default:
                m_FilterLabel = DiceBy;
        }

        $('#liThirdChartLabel').text(m_FilterLabel);

        GetPOSSalesReportByDice('tblThirdChart', 'dvThirdChart');
    }

    function GetPOSSalesReportByDice(table, div) {
        var m_Totals = [];
        var m_TotalGross = 0;
        var m_TotalTransactionCount = 0;
        var m_TotalProductCount = 0;

        AjaxSend({
            url: 'DashboardsAndReports/GetPOSSalesReportBySlice',
            data: m_DashBoardChartsData,
            success: function (vData) {

                $('#' + table).html('');
                $('#' + div).empty();

                if (vData.ReportPOS.Rows.length > 0) {

                    var vRows = vData.ReportPOS.Rows;

                    $('#' + div).css('visibility', 'visible');
                    $('#btnDownloadDiceAsExcel').css('visibility', 'visible');

                    m_Totals[0] = [];
                    m_TicksPie = [];
                    m_Labels = [];

                    var tData = '';

                    //additional by janine. show count. october 30 5pm

                    for (i = 0; i <= vRows.length - 1; i++) {

                        m_Totals[0][i] = $.format.number(vRows[i].TransactionCount, vRows[i].ProductFeature);

                        m_TotalTransactionCount = m_TotalTransactionCount + CDec2(vRows[i].TransactionCount);

                        m_TotalProductCount = m_TotalProductCount + (CDec2(vRows[i].ProductCount) == 0 ? CDec2(vRows[i].ProductCountSO) : CDec2(vRows[i].ProductCount));
                    }
                    //for chart, it will sort desc
                    m_Totals[1] = [];
                    for (i = 0; i <= vRows.length - 1; i++) {

                        m_Totals[1][i] = $.format.number(vRows[i].Total, vRows[i].ProductFeature);

                        m_TotalGross = m_TotalGross + $.format.number(vRows[i].Total);
                    }
                    //end of additional
                    tData = $('<tr class="custom-theme-f">')
                                    .append($('<th>')
                                        .text(m_FilterLabel)
                                    )
                                    .append($('<th style="text-align:right">')
                                        .text('Gross Sales')
                                    )
                                    .append($('<th style="text-align:right">')
                                        .text('Contribution Margin')
                                    )
                                    .append($('<th style="text-align:right">')
                                        .text('Transaction Count')
                                    )
                                    .append($('<th style="text-align:right">')
                                        .text('Product Count')
                                    )
                                    .append($('<br />'))

                    $('#' + table).append(tData);

                    //                        tData = $('<tr>')
                    //                                .append($('<td colspan="3">')
                    //                                    .append($('<br />'))
                    //                                )

                    //                        $('#' + table).append(tData);

                    for (var i = vRows.length - 1; i >= 0; i--) {
                        m_TicksPie[i] = vRows[i].ProductFeature;
                        m_Labels[i] = AccountingFormat($.format.number(vRows[i].Total));

                        tData = $('<tr>')
                                    .append($('<td>')
                                        .text(vRows[i].ProductFeature)
                                    )
                                    .append($('<td style="text-align:right">')
                                        .text(AccountingFormat($.format.number(vRows[i].Total)))
                                    )
                                    .append($('<td style="text-align:right">')
                                        .text(AccountingFormat((parseFloat(vRows[i].Total) / parseFloat(m_TotalGross)) * 100) + '%')
                                    )
                                    .append($('<td style="text-align:right">')
                                        .text(vRows[i].TransactionCount)
                                    )
                                    .append($('<td style="text-align:right">')
                                        .text(AccountingFormat((CDec2(vRows[i].ProductCount) == 0 ? vRows[i].ProductCountSO : vRows[i].ProductCount)))
                                    )

                        $('#' + table).append(tData);
                    }

                    tData = $('<tr>')
                            .append($('<td colspan="7">')
                                .append($('<hr>'))
                            )

                    $('#' + table).append(tData);

                    tData = $('<tr>')
                                    .append($('<td>')
                                        .text('Total')
                                    )
                                    .append($('<td style="text-align:right">')
                                        .text(AccountingFormat(m_TotalGross))
                                    )
                                    .append($('<td style="text-align:right">')
                                    )
                                    .append($('<td style="text-align:right">')
                                        .text(m_TotalTransactionCount)
                                    )
                                    .append($('<td style="text-align:right">')
                                        .text(AccountingFormat(m_TotalProductCount))
                                    )

                    $('#' + table).append(tData);
                    DisplayDiceBarGraph(m_Totals);
                }
            }
        }); //end ajaxsend
    }

    function DisplayDiceBarGraph(m_Totals) {
        var plot1 = $.jqplot('dvThirdChart', m_Totals, {
            animate: !$.jqplot.use_excanvas,
            seriesDefaults: {
                renderer: $.jqplot.BarRenderer,
                pointLabels: {
                    show: true
                    //                        labels: m_Labels
                },
                rendererOptions: {
                    barDirection: 'horizontal'
                }
            },
            axes: {
                xaxis: {
                    drawBaseline: false
                },
                yaxis: {
                    renderer: $.jqplot.CategoryAxisRenderer,
                    ticks: m_TicksPie
                }
            },
            highlighter: { show: false }
        });

        $('#dvThirdChart .jqplot-xaxis-tick').css('display', 'none');
    }

    window.ClearCharts = function () {

        $('#dvSalesTrends').css('visibility', 'hidden');
        $('#dvSalesTrends').css('display', 'none');

        $('#dvMainChart').empty();
        $('#dvSecondChart').empty();
        $('#dvThirdChart').empty();

        $('#tblSecondChart').empty();
        $('#tblThirdChart').empty();

        $('#tdTotalGrossSales').html("0");
        $('#tdSalesToday').html("0");

        $('#dvSlice').css('visibility', 'hidden');
        $('#dvDice').css('visibility', 'hidden');

        $('#btnDownloadSliceAsExcel').css('visibility', 'hidden');
        $('#btnDownloadDiceAsExcel').css('visibility', 'hidden');

        $('#selSlice').val('');
        $('#selDice').val('');

        $('#selSlice').selectmenu('refresh');
        $('#selDice').selectmenu('refresh');

        $('#tblSalesOrder').css('visibility', 'hidden');
        $('#tblSalesOrderWeek').css('visibility', 'hidden');

        $('#IsDataPresent').html('-Graph will be shown when you click on a group or FOWST (Factory, Office, Store, Warehouse or Truck) name-')
        $('#IsDataPresent').css('visibility', 'visible');
        $('#IsDataPresent').css('lnkTransactionDetails', 'hidden');
    }



    window.GetSearchData = function (data) {

        m_DashBoardChartsData = data;
        //GetTransactionDetails(data);
    }


});