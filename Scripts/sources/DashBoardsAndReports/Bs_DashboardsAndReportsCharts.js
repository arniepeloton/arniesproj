require([
        'common',
        'promise!/Scripts3/Browser/LibraryFowst.js?crc=',
        '/Scripts/chart.min.js'
    ], function () {
        var m_Data;
        var m_DashBoardChartsData;
        var GroupDateBy;
        var IsSalesDistribution = true;

        var GroupDateBy; //para madisplay ang date sa chart
        var m_Ticks;
        var m_TicksBar;
        var m_Labels;
        var m_Slice;
        var m_Dice;
        var sliceDateFrom = null, sliceDateTo = null;

        $(function () {


            //$('#pageMain').attr('class', 'ui-page ui-body-c ui-page-active');
            if (!CBool(sessionStorage.PrintingOfflineData)) {
                //GetAllFeatures();
            }

            sessionStorage.SearchMode = 'false';
            $('.txtSearch').val('');


            $('#liSecondChart').click(function () {
                GetAllFeatures('slice');
            });


            $('#liThirdChartLabel').click(function () {
                GetAllFeatures('dice');
            });

            $('#dvMainChart').click(function () {
                if (IsSalesDistribution) {
                    GetAllFeatures('slice');
                }
            });

            $('#dvSecondChart').bind('jqplotDataClick',
                function (ev, seriesIndex, pointIndex, data) {
                    m_Filter1 = m_TicksBar[pointIndex];

                    GetAllFeatures('dice');

                    $('#liThirdChartMainLabel').text('Showing Sales>' + m_Filter1 + '>');

                    ShowHideDiceList();

                }
            );

            $('#selCashier').change(function () {
                m_DashBoardChartsData.CashierID = $('#selCashier').val();
                GetPOSSalesInvoiceHeader(m_DashBoardChartsData);
            })

            //dire ni sya kay ang value sa fowst depends on clicking sa chart
            $('#lnkTransactionDetails').click(function () {
                $('#txtSearchProduct').val('');
                //$('#divTransDetails').hide();
                GetPOSSalesInvoiceHeader(m_DashBoardChartsData);
                $('#chbTransViewOpt').prop('checked', false);//.checkboxradio('refresh');
                AddBreadCrumb({
                    text: 'POS Sales Transactions', Click: function () {
                        $('#panTransactionDetails').pageshow();
                    }
                }, '.ItemsBreadCrumbs')
                //GetTransactionDetails(m_DashBoardChartsData);
            });

            //dire ni sya kay ang value sa fowst depends on clicking sa chart
            $('#txtSearchProductTransDetails').keypress(function (e) {
                if (e.keyCode == 13) {
                    m_DashBoardChartsData.IsSearch = 1;
                    m_DashBoardChartsData.SearchString = $('#txtSearchProductTransDetails').val();
                    GetTransactionDetails(m_DashBoardChartsData);
                }
            });

            $('#txtSearchHeader').keypress(function (e) {
                if (e.keyCode == 13) {
                    m_DashBoardChartsData.IsSearch = 1;
                    m_DashBoardChartsData.SearchString = $('#txtSearchHeader').val();
                    GetPOSSalesInvoiceHeader(m_DashBoardChartsData);
                }
            });

            $('.txtSearch').keypress(function (e) {
                if (e.keyCode == 13) {
                    $('.btnSearch').click();
                }
            });

            $('.btnSearch').click(function () {

                BrowseAddresses();

            })
            //$('.txtSearch').keypress(function (e) {
            //    if (e.keyCode == 13) {

            //BrowseAddresses();

            //    }
            //});

            $('#btnGraphViewDetails').click(function () {
                AddBreadCrumb({
                    text: 'Groups\' Sales Details', Click: function () {
                        $('#divGroupSalesReport').pageshow();
                    }
                }, '.ItemsBreadCrumbs')
                $('#divGroupSalesReport').pageshow();
            })
        });
        var rgb = [
            '255, 0, 0',       // Red
            '0, 255, 0',       // Green
            '0, 0, 255',       // Blue
            '255, 255, 0',     // Yellow
            '255, 0, 255',     // Magenta
            '0, 255, 255',     // Cyan
            '128, 128, 128',   // Gray
            '255, 128, 0',     // Orange
            '128, 0, 128',     // Purple
            '0, 128, 0',       // Dark Green
            '128, 0, 0',       // Maroon
            '0, 0, 128',       // Navy
            '128, 128, 0',     // Olive
            '0, 128, 128',     // Teal
            '192, 192, 192',   // Silver
            '255, 165, 0',     // Orange
            '139, 0, 0',       // Dark Red
            '0, 139, 0',       // Dark Green
            '0, 0, 139',       // Dark Blue
            '255, 192, 203',   // Pink
            '255, 105, 180',   // Hot Pink
            '0, 128, 128',     // Teal
            '255, 255, 255',   // White
            '70, 130, 180',    // Steel Blue
            '34, 139, 34',     // Forest Green
            '173, 216, 230',   // Light Blue
            '255, 255, 224',   // Light Yellow
            '250, 128, 114',   // Salmon
            '0, 128, 0',       // Green
            '255, 69, 0',      // Red-Orange
            '0, 0, 255',       // Blue
            '128, 128, 128',   // Gray
            '255, 0, 255',     // Fuchsia
            '255, 165, 0',     // Orange
            '139, 69, 19',     // Saddle Brown
            '139, 0, 139',     // Dark Magenta
            '0, 139, 139',     // Dark Cyan
            '255, 140, 0',     // Dark Orange
            '128, 0, 0',       // Dark Red
            '0, 0, 128',       // Navy
            '255, 255, 0',     // Yellow
            '0, 255, 0',       // Lime
            '0, 255, 255',     // Aqua
            '128, 128, 0',     // Olive
            '255, 0, 0',       // Red
            '255, 192, 203',   // Pink
            '255, 105, 180',   // Hot Pink
        ]

        var piechart = null;
        window.DisplayPieChart = function (vRows, type) {
            var ctx = document.getElementById('chartOtherGrap');            
            var label = [], data = [], bgColor = [], borderColor = [];
            var totalWalkIn = 0, totalDelivery = 0, title = '';

            for (i = 0; i < vRows.length; i++) {
                if (type == 'revbreakdown') {
                    totalDelivery += CDec2(vRows[i].FoodPanda) + CDec2(vRows[i].GrabFood) + CDec2(vRows[i].LocalDelivery);
                    totalWalkIn += (vRows[i].GrossSales) - CDec2(vRows[i].FoodPanda) - CDec2(vRows[i].GrabFood) - CDec2(vRows[i].LocalDelivery);
                } else if (type == 'procat') {
                    label.push(vRows[i].ProductFeature);
                    data.push(CDec2(vRows[i].Total));
                    bgColor.push('rgba(' + rgb[i] + ', 0.5)');
                    borderColor.push('rgba(' + rgb[i] + ', 1)');
                }                
            }

            if (type == 'revbreakdown') {
                label = ['Walk-in', 'Delivery'];
                data.push(totalWalkIn);
                data.push(totalDelivery);

                bgColor.push('rgba(' + rgb[0] + ', 0.5)');
                borderColor.push('rgba(' + rgb[0] + ', 1)');

                bgColor.push('rgba(' + rgb[1] + ', 0.5)');
                borderColor.push('rgba(' + rgb[1] + ', 1)');
            }            

            if (piechart != null) {
                piechart.destroy();
            }

            if (type == 'revbreakdown') {
                title = 'Revenue Breakdown';
            } else if (type == 'procat') {
                title = 'Product Categories Gross Sales';
            }

            piechart = new Chart(ctx, {
                type: 'pie',
                data: {
                    labels: label,
                    datasets: [{
                        //label: '',
                        data: data,
                        backgroundColor: bgColor,
                        borderColor: borderColor,
                        borderWidth: 1
                    }]
                },
                options: {
                    maintainAspectRatio: false,
                    responsive: true,
                    plugins: {
                        legend: {
                            position: 'top',
                        },
                        title: {
                            display: true,
                            text: title
                        },
                        tooltip: {
                            callbacks: {
                                label: function (tooltipItem, mdata) {
                                    var total = 0;
                                    for (var i = 0; i <= tooltipItem.dataset.data.length - 1; i++) {
                                        total += CDec2(tooltipItem.dataset.data[i]);
                                    }
                                    return tooltipItem.label + ': ' + AccountingFormat(tooltipItem.raw) + ' (' + AccountingFormat((CDec2(tooltipItem.raw) / total) * 100) + '%)';
                                }
                            }
                        }
                    }
                    //scales: {
                    //    y: {
                    //        beginAtZero: true
                    //    }
                    //}
                }
            });
        }

        var chartGroupSales = null;
        window.DisplaySalesGroupChart = function (vRows) {
            var ctx = document.getElementById('chartGroupSales');
            var label = [], data = [], bgColor = [], borderColor = [], GroupID = [];
            var tbodyGroupSales = $('#tbodyGroupSales'), total = 0;

            tbodyGroupSales.empty();
            $('#selSliceByGroups').empty();
            $('#selSliceByGroups').append(new Option('All', 'All'));
            $('#selSliceByGroups').append(new Option('By Group/Region', 'Group'));

            for (i = 0; i < vRows.length; i++) {
                total += CDec2(vRows[i].GrossSales);
            }

            for (i = 0; i < vRows.length; i++) {
                label.push(vRows[i].GroupName);
                data.push(vRows[i].GrossSales);
                GroupID.push(vRows[i].GroupID);
                bgColor.push('rgba(' + rgb[i] + ', 0.5)');
                borderColor.push('rgba(' + rgb[i] + ', 1)');

                tbodyGroupSales.append($('<tr>')
                        .append($('<td>').text(vRows[i].GroupName))
                        .append($('<td class="text-center">').text(AccountingFormat(vRows[i].GrossSales)))
                        .append($('<td class="text-center">').text(AccountingFormat((CDec2(vRows[i].GrossSales) / total) * 100) + '%'))
                        .append($('<td class="text-center">').text(AccountingFormat(CDec2(vRows[i].TotalDiscounts))))
                        .append($('<td class="text-center">').text(AccountingFormat(CDec2(vRows[i].ForegoneVAT))))
                        .append($('<td class="text-center">').text(AccountingFormat(CDec2(vRows[i].TotalRefund))))
                        .append($('<td class="text-center">').text(AccountingFormat(CDec2(vRows[i].TotalSrv))))
                        .append($('<td class="text-center">').text(AccountingFormat(CDec2(vRows[i].GrossSales) - CDec2(vRows[i].TotalDiscounts) - CDec2(vRows[i].TotalRefund) - CDec2(vRows[i].TotalSrv) - CDec2(vRows[i].ForegoneVAT == undefined ? '0' : vRows[i].ForegoneVAT))))
                    )

                $('#selSliceByGroups').append(new Option(vRows[i].GroupName, vRows[i].GroupID));
            }

            if (vRows.length == 0) {
                tbodyGroupSales.append($('<tr>')
                        .append($('<td class="text-center" colspan="10">').text('No data available in table'))
                    )
            }

            if (chartGroupSales != null) {
                chartGroupSales.destroy();
            }

            chartGroupSales = new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: label,
                    datasets: [{
                        //label: '',
                        data: data,
                        backgroundColor: bgColor,
                        borderColor: borderColor,
                        borderWidth: 1
                    }]
                },
                options: {
                    maintainAspectRatio: false,
                    responsive: true,
                    plugins: {
                        legend: {
                            position: 'top',
                        },
                        title: {
                            display: true,
                            text: 'Groups\' Gross Sales'
                        },
                        tooltip: {
                            callbacks: {
                                label: function (tooltipItem, mdata) {
                                    var total = 0;
                                    for (var i = 0; i<=tooltipItem.dataset.data.length - 1; i++) {
                                        total += CDec2(tooltipItem.dataset.data[i]);
                                    }
                                    return tooltipItem.label + ': ' + AccountingFormat(tooltipItem.raw) + ' (' + AccountingFormat((CDec2(tooltipItem.raw) / total) * 100) + '%)';
                                }
                            }
                        }
                    },
                    onClick: function (e) {
                        var activePoints = chartGroupSales.getElementsAtEventForMode(e, 'nearest', { intersect: true }, false);
                        if (activePoints[0]) {

                            var dashboardfilters = [
                                { ModuleName: 'Hide Company (All) option in dashboards filtering', Value: 'All' },
                                { ModuleName: 'Hide By Group/Region option Filtering in dashboards filtering', Value: 'Group' }
                            ]

                            function HideDashboardFilters(vRows) {
                                for (var i in vRows) {
                                    $.grep(dashboardfilters, function (e, index) {
                                        if (e.ModuleName == vRows[i].ModuleName) {
                                            $('#selSliceByGroups option[value="' + e.Value + '"]').remove();
                                        }
                                    })
                                }

                                //var chartData = activePoints[0].config.data;
                                var idx = activePoints[0].index;

                                var label = chartGroupSales.data.labels[idx];
                                var value = chartGroupSales.data.datasets[0].data[idx];

                                $('#selSliceByGroups').val(GroupID[idx]);
                                $('#btnPeriodSlice').text($('.btnPeriod:first').text());
                                $('#popSliceByGroups').modal('show');
                                sliceDateFrom = null;
                                sliceDateTo = null;
                                GetGroupSalesReport(GroupID[idx], label);
                                sessionStorage.SelectedGroup = GroupID[idx];
                                GetTopRankingStores(true);
                            }

                            if (IsMobile) {
                                var vRows = Sql.Execute(" SELECT md.ModuleName, rmd.IshasAccess as IsHidden, md.SortOrder \n" +
                                    " FROM LibrarySecurityRoleUser usr \n" +
                                    " INNER JOIN LibrarySecurityRole r ON (usr.RoleID = r.ID) \n" +
                                    " INNER JOIN LibrarySecurityRoleModules rmd ON (rmd.RoleID = r.ID) \n" +
                                    " INNER JOIN LibrarySecurityModule md ON (rmd.ModuleID = md.ID) \n" +
                                    " WHERE rmd.IshasAccess = 1 \n" +
                                    " AND usr.PeopleID = " + ReadyString(sessionStorage.PeopleID) + " \n" +
                                    " AND md.ID IN ('b6f38aec-c892-48e6-bcc0-50d1ae80fe2b', '39a3ecfe-7b6f-4897-b79d-c5846a86460a') \n" +
                                    " ORDER BY SortOrder ASC \n").Rows;

                                HideDashboardFilters(vRows);
                            } else {
                                AjaxSend({
                                    url: '/DashboardsAndReports/DashboardDonutChartFilters',
                                    success: function (e) {
                                        var vRows = e.Results.Rows;

                                        HideDashboardFilters(vRows);
                                    }
                                })
                            }                            
                        }
                    }
                }
            });
        }

        var chartADS = null;
        window.DisplayADSChart = function (vRows) {
            var ads = document.getElementById('chartADS');

            if (chartADS != null) {
                chartADS.destroy();
            }

            for (var i in vRows) {
                vRows[i].backgroundColor = 'rgba(' + rgb[i] + ', 0.5)';
            }

            chartADS = new Chart(ads, {
                type: 'bar',
                data: {
                    labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
                    datasets: vRows,                   
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: {
                        xAxes: [{
                            //stacked: true,
                            stacked: false,
                            ticks: {
                                beginAtZero: true,
                                maxRotation: 0,
                                minRotation: 0
                            }
                        }],
                        yAxes: [{
                            stacked: false,
                        }]
                    },                  
                }
            });
        }

        window.GetPOSSalesReportByTrendOrSalesDistribution = function (data) {
            data.FowstID = data.Fowst //para makuha sa WebRequest.Instance.FowstID
            m_DashBoardChartsData = data;
            GroupDateBy = 'Day';
            ClearCharts();
            Bs.Loading('show');
            AjaxSend({
                url: 'DashboardsAndReports/GetPOSSalesReportByTrendOrSalesDistribution',
                data: data,
                success: function (vData) {
                    Bs.Loading('hide');
                    if (vData.ReportPOS.Rows.length > 0) {
                        $('#IsDataPresent').css('display', 'none');
                        $('#lblMainChartInstruction').css('visibility', 'visible');
                        $('#lblMainChartInstruction').text('* Click the chart to breakdown data');

                        m_Data = vData.ReportPOS.Rows;
                        DisplayTrend();
                    }
                    else {
                        $('#IsDataPresent').css('display', 'block');
                        $('#lblMainChartInstruction').css('visibility', 'hidden');


                    }
                }
            }); //end ajaxsend
        }

        var chartDeviceUtilStoreStatus = null;
        window.DisplayDeviceUtilStoreStatus = function (vRows) {
            var ctx = document.getElementById('chartDeviceUtilStoreStatus');
            var label = [], data = [], bgColor = [], borderColor = [];

            for (i = 0; i < vRows.length; i++) {
                var randomcolor = rgb[i];
                label.push(vRows[i].Status);
                data.push(vRows[i].Percentage);
                bgColor.push('rgba(' + randomcolor + ', 0.5)');
                borderColor.push('rgba(' + randomcolor + ', 1)');
            }

            if (chartDeviceUtilStoreStatus != null) {
                chartDeviceUtilStoreStatus.destroy();
            }

            chartDeviceUtilStoreStatus = new Chart(ctx, {
                type: 'pie',
                data: {
                    labels: label,
                    datasets: [{
                        //label: '',
                        data: data,
                        backgroundColor: bgColor,
                        borderColor: borderColor,
                        borderWidth: 1
                    }]
                },
                options: {
                    maintainAspectRatio: false,
                    responsive: true,
                    plugins: {
                        legend: {
                            position: 'bottom',
                        },
                        //title: {
                        //    display: true,
                        //    text: 'Groups\' Gross Sales'
                        //},
                        tooltip: {
                            callbacks: {
                                label: function (tooltipItem, mdata) {
                                    return tooltipItem.label;
                                }
                            }
                        }
                    },
                    onClick: function (e) {

                    }
                }
            });
        }

        var chartDeviceUtilUsage = null;
        window.DisplayDeviceUtilUsage = function (vRows) {
            var ctx = document.getElementById('chartDeviceUtilUsage');
            var label = [], data = [], bgColor = [], borderColor = [];

            for (i = 0; i < vRows.length; i++) {
                var randomcolor = rgb[i];
                label.push(vRows[i].UsageLabel);
                data.push(vRows[i].Percentage);
                bgColor.push('rgba(' + randomcolor + ', 0.5)');
                borderColor.push('rgba(' + randomcolor + ', 1)');
            }

            if (chartDeviceUtilUsage != null) {
                chartDeviceUtilUsage.destroy();
            }

            chartDeviceUtilUsage = new Chart(ctx, {
                type: 'pie',
                data: {
                    labels: label,
                    datasets: [{
                        //label: '',
                        data: data,
                        backgroundColor: bgColor,
                        borderColor: borderColor,
                        borderWidth: 1
                    }]
                },
                options: {
                    maintainAspectRatio: false,
                    responsive: true,
                    plugins: {
                        legend: {
                            position: 'bottom'
                        },
                        //title: {
                        //    display: true,
                        //    text: 'Groups\' Gross Sales'
                        //},
                        tooltip: {
                            callbacks: {
                                label: function (tooltipItem, mdata) {
                                    return tooltipItem.label;
                                }
                            }
                        }
                    },
                    onClick: function (e) {
                        
                    }
                }
            });
        }

        function RandomRGB() {
            var r = RandomInteger(1, 255);
            var g = RandomInteger(1, 255);
            var b = RandomInteger(1, 255);

            return r + ', ' + g + ', ' + b;
        }

        function BrowseAddresses() {

            Browser.LibraryFowst.InputData.OrganizationID = sessionStorage.OrgID;
            Browser.LibraryFowst.InputData.FilterFowst = 1;
            Browser.LibraryFowst.Click = function (e) {
                sessionStorage.FowstID = e.ID;
                sessionStorage.SearchMode = 'true';
                $('#txtSearchProduct').val('');
                window.GetData();
            };
            Browser.LibraryFowst.Show();

        };

        var dataTableDailySalesMainChart;

        function DisplayTrend() {
            var m_Totals = [];
            var m_Days = [];
            var m_Months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
            var TotalSales = 0;
            var TotalForegoneVAT = 0;
            var TotalSalesToday = 0;
            var TotalDiscounts = 0;
            var tData;

            m_Totals[0] = [];

            if ($.fn.DataTable.isDataTable($('#tblMainChart'))) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                $('#tblMainChart').empty();
                dataTableDailySalesMainChart.destroy();
            }

            var buttons = [];
            if (!IsMobile) {
                buttons.push({ extend: 'csvHtml5', text: 'Download CSV', title: 'Sales Details' })
            }

            var ctr = 0;
            dataTableDailySalesMainChart = $('#tblMainChart').DataTable({
                pageLength: 32,
                "ordering": false,
                "data": m_Data,
                "columns": [
                     {
                         "data": function (data) {
                             return CStr2($.format.date(new Date(data.InvoiceDate), 'MMM dd, yyyy hh:mm a'));
                         }
                     },
                    {
                        "data": function (data) {
                            return AccountingFormat(data.Total);
                        }
                    },
                    {
                        "data": function (data) {
                            return AccountingFormat(data.DiscountTotal);
                        }
                    },
                    {
                        "data": function (data) {
                            return AccountingFormat(data.ForegoneVAT);
                        }
                    },
                    {
                        "data": function (data) {
                            return AccountingFormat(data.SalesReturn);
                        }
                    },
                    {
                        "data": function (data) {
                            return AccountingFormat(data.ChargeAmount);
                        }
                    },
                    {
                        "data": function (data) {
                            return AccountingFormat(CDec2(data.Total) - CDec2(data.DiscountTotal) - CDec2(data.ForegoneVAT) - CDec2(data.SalesReturn) + CDec2(data.ChargeAmount));
                        }
                    },
                ],
                createdRow: function (row, data, index) {
                   
                },
                dom: 'Brtip',
                buttons: buttons,
                initComplete: function () {
                    $('#tblMainChart').removeClass('dataTable');
                },
                footerCallback: function (row, data, start, end, display) {
                    var api = this.api(), data;

                    // Remove the formatting to get integer data for summation
                    var intVal = function (i) {
                        return typeof i === 'string' ?
                            i.replace(/[\$,]/g, '') * 1 :
                            typeof i === 'number' ?
                            i : 0;
                    };
                    for (i = 1; i <= 6; i++) {
                        // Total over all pages
                      
                        total = api
                            .column(i)
                            .data()
                            .reduce(function (a, b) {
                                return intVal(CDec2(a)) + intVal(CDec2(b));
                            }, 0);
                        
                        // Total over this page
                        pageTotal = api
                            .column(i, { page: 'current' })
                            .data()
                            .reduce(function (a, b) {
                                return intVal(CDec2(a)) + intVal(CDec2(b));
                            }, 0);

                        // Update footer
                        $(api.column(i).footer()).html(
                            AccountingFormat(total)
                        );
                    }
                }
            });

            //var pLV = $('#tblMainChartBody');//.html('');

            //tData = $('<tr class="custom-theme-f">')
            //                .append($('<th style="text-align:left">')
            //                    .text('Date')
            //                )
            //                .append($('<th style="text-align:right">')
            //                    .text('Gross Sales')
            //                )
            //                .append($('<th style="text-align:right">')
            //                    .text('Discount')
            //                )
            //                .append($('<th style="text-align:right">')
            //                    .text('Vat Exempt Tax')
            //                )
            //                .append($('<th style="text-align:right">')
            //                    .text('Refunds and Exchanges')
            //                )
            //                .append($('<th style="text-align:right">')
            //                    .text('Net Sales')
            //                )
            //                .append($('<br />'))

            //$('#tblMainChartHeader').empty().append(tData);

            //pLV.empty();

            if (m_Data.length > 0) {

                for (var i = 0; i < m_Data.length; i++) {

                    var m_Date = new Date(m_Data[i].InvoiceDate);
                    var m_Today = new Date();

                    m_Totals[0][i] = $.format.number(m_Data[i].Total);
                    TotalSales = TotalSales + parseFloat(m_Data[i].Total);
                    TotalForegoneVAT = TotalForegoneVAT + parseFloat(m_Data[i].ForegoneVAT);
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

                    //tData = $('<tr>')
                    //        .append($('<th style="text-align:left">')
                    //            .text(m_Months[m_Date.getMonth()] + ' ' + m_Date.getDate() + ', ' + m_Date.getFullYear())
                    //        )
                    //        .append($('<th style="text-align:right">')
                    //            .text(AccountingFormat(m_Data[i].Total))
                    //        )
                    //        .append($('<th style="text-align:right">')
                    //            .text(AccountingFormat(m_Data[i].DiscountTotal))
                    //        )
                    //        .append($('<th style="text-align:right">')
                    //            .text(AccountingFormat(m_Data[i].ForegoneVAT))
                    //        )
                    //        .append($('<th style="text-align:right">')
                    ////returns
                    //            .text('')
                    //        )
                    //        .append($('<th style="text-align:right">')
                    //            .text(AccountingFormat(parseFloat(m_Data[i].Total) - parseFloat(m_Data[i].DiscountTotal) - parseFloat(m_Data[i].ForegoneVAT)))
                    //        )
                    //        .append($('<br />'))

                    //pLV.append(tData);

                }

                //tData = $('<tr>')
                //            .append($('<th style="text-align:left; font-size:x-large; font-weight:bolder">')
                //                .text('Total')
                //            )
                //            .append($('<th style="text-align:right; font-size:x-large; font-weight:bolder">')
                //                .text(AccountingFormat(TotalSales))
                //            )
                //            .append($('<th style="text-align:right; font-size:x-large; font-weight:bolder">')
                //                .text(AccountingFormat(TotalDiscounts))
                //            )
                //            .append($('<th style="text-align:right; font-size:x-large; font-weight:bolder">')
                //                .text(AccountingFormat(TotalForegoneVAT))
                //            )
                //            .append($('<th style="text-align:right; font-size:x-large; font-weight:bolder">')
                ////returns
                //                .text('')
                //            )
                //            .append($('<th style="text-align:right; font-size:x-large; font-weight:bolder">')
                //                .text(AccountingFormat(parseFloat(TotalSales) - parseFloat(TotalDiscounts) - parseFloat(TotalForegoneVAT)))
                //            )
                //            .append($('<br />'))

                $('#tblMainChartFooter').empty().append(tData);

                $('#lblMainChartInstruction').css('visibility', 'visible');
                $('#lblMainChartInstruction').text('* Click the chart to breakdown data');

                m_Ticks = m_Days;

                DisplayLineGraph(m_Totals);
            }
        }

        function DisplayLineGraph(m_Totals) {

            $('#dvMainChart').empty();

            var plot1 = $.jqplot('dvMainChart', m_Totals, {
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

            //$('#tblDateFormatLabel').css('visibility', 'visible')
        }

        function GetAllFeatures(type) {
            AjaxSend({
                url: 'DashboardsAndReports/GetOrganizationProductFeatures',
                success: function (vData) {
                    DisplayOrganizationProductFeatures(vData.Results.Rows, type);
                }
            });
        }

        function DisplayOrganizationProductFeatures(vRows, type) {
            var pLV = $('#lstSlice');
            var pLV2 = $('#lstDice');
            var options = [
                  {
                     Text: 'Age', Click: function () {                        
                         if (type == 'slice') {
                             m_Slice = 'Age';
                             SliceData('Age');
                             ShowHideDiceList();
                         } else {
                             DiceData('Age', 'tblThirdChart', 'dvThirdChart', 'dvDice');
                         }
                     }
                 }, {
                    Text: 'Brand', Click: function () {                        
                        if (type == 'slice') {
                            m_Slice = 'Brand';
                            SliceData('Brand');
                            ShowHideDiceList();
                        } else {
                            DiceData('Product', 'tblThirdChart', 'dvThirdChart', 'dvDice');
                        }

                    }
                 }, {
                     Text: 'Customer', Click: function () {                        
                         GetPOSSalesReportByCustomer();
                     }
                 }, {
                    Text: 'Customer Industry', Click: function () {
                        if (type == 'slice') {
                            m_Slice = 'CustomerIndustry';
                            SliceData('Customer Industry');
                            ShowHideDiceList();
                        } else {
                            DiceData('Customer Industry', 'tblThirdChart', 'dvThirdChart', 'dvDice');
                        }
                    }
                }, {
                    Text: 'Division', Click: function () {
                        if (type == 'slice') {
                            m_Slice = 'Division';
                            SliceData('Division');
                            ShowHideDiceList();
                        } else {
                            DiceData('Division', 'tblThirdChart', 'dvThirdChart', 'dvDice');
                        }
                    }
                }, {
                    Text: 'Product', Click: function () {                        
                        if (type == 'slice') {
                            m_Slice = 'Product';
                            SliceData('Product');
                            ShowHideDiceList();
                        } else {
                            DiceData('Product', 'tblThirdChart', 'dvThirdChart', 'dvDice');
                        }
                    }
                }, {
                    Text: 'Delivery Method', Click: function () {                        
                        if (type == 'slice') {
                            m_Slice = 'Delivery Method';
                            SliceData('Delivery Method');
                            ShowHideDiceList();
                        } else {
                            DiceData('Delivery Method', 'tblThirdChart', 'dvThirdChart', 'dvDice');
                        }
                    }
                }, {
                    Text: 'Gender', Click: function () {
                        if (type == 'slice') {
                            m_Slice = 'Gender';
                            SliceData('Gender');
                            ShowHideDiceList();
                        } else {
                            DiceData('Gender', 'tblThirdChart', 'dvThirdChart', 'dvDice');
                        }
                    }
                }, {
                    Text: 'Order Method', Click: function () {                        
                        if (type == 'slice') {
                            m_Slice = 'Order Method';
                            SliceData('Order Method');
                            ShowHideDiceList();
                        } else {
                            DiceData('Order Method', 'tblThirdChart', 'dvThirdChart', 'dvDice');
                        }
                    }
                }, {
                    Text: 'Product or SKU', Click: function () {                        
                        if (type == 'slice') {
                            m_Slice = 'Product';
                            SliceData('Product');
                            ShowHideDiceList();
                        } else {
                            DiceData('Product', 'tblThirdChart', 'dvThirdChart', 'dvDice');
                        }
                    }
                }, {
                    Text: 'Sales Channel', Click: function () {
                        if (type == 'slice') {
                            m_Slice = 'Sales Channel';
                            SliceData('Sales Channel');
                            ShowHideDiceList();
                        } else {
                            DiceData('Sales Channel', 'tblThirdChart', 'dvThirdChart', 'dvDice');
                        }
                    }
                }, {
                    Text: 'Time', Click: function () {
                        if (type == 'slice') {
                            m_Slice = 'Time';
                            SliceData('Time');
                            ShowHideDiceList();
                        } else {
                            DiceData('Product', 'tblThirdChart', 'dvThirdChart', 'dvDice');
                        }
                    }
                }
            ];

            for (i = 0; i <= vRows.length - 1; i++) {
                options.push({
                    data: vRows[i], Text: vRows[i].Label, Click: function (e) {                        
                        if (type == 'slice') {
                            m_Slice = e.Label;
                            SliceData(e.Label);
                            ShowHideDiceList();
                        } else {
                            DiceData(e.Label, 'tblThirdChart', 'dvThirdChart', 'dvDice');
                        }
                    }
                })
            }

            PopupButtonList('', 'Sales Distribution', options);
        }

        function SliceData(SliceBy) {
            delete m_DashBoardChartsData["Filter1"];
            delete m_DashBoardChartsData["Filter1Option"];

            switch (SliceBy) {
                case "Day":
                    m_DashBoardChartsData.SliceBy = 'Day';
                    break;
                case "Time":
                    m_DashBoardChartsData.SliceBy = 'optTime';
                    window.location = '#dvSlice';
                    break;
                case "Brand":
                    m_DashBoardChartsData.SliceBy = 'optBrand';
                    window.location = '#dvSlice';
                    break;
                case "Customer Industry":
                    m_DashBoardChartsData.SliceBy = 'optCustomerIndustry';
                    window.location = '#dvSlice';
                    break;
                case "Division":
                    m_DashBoardChartsData.SliceBy = 'optDivisionDescription';
                    break;
                //additional by janine oct 30 12pm                       
                case "Product":
                    m_DashBoardChartsData.SliceBy = 'optProduct';
                    break;
                case "Delivery Method":
                    m_DashBoardChartsData.SliceBy = 'optDeliveryMethod';
                    break;
                case "Order Method":
                    m_DashBoardChartsData.SliceBy = 'optOrderMethod';
                    break;
                case "Gender":
                    m_DashBoardChartsData.SliceBy = 'optGender';
                    break;
                case "Age":
                    m_DashBoardChartsData.SliceBy = 'optAge';
                    break;
                //end of additional                 
                case "Season/Year":
                    m_DashBoardChartsData.SliceBy = 'Season/Year';
                    break;
                case "SalesRepresentative":
                    m_DashBoardChartsData.SliceBy = 'optSalesRepresentative';
                    break;
                case "Sales Channel":
                    m_DashBoardChartsData.SliceBy = 'optSalesChannel';
                    break;
                default:
                    m_DashBoardChartsData.SliceBy = SliceBy;
                    break;
            }

            window.location = '#dvSlice';

            $('#dvSlice').css('display', 'block');

            $('#tblThirdChart').empty();
            $('#dvThirdChart').empty();
            $('#dvDice').css('display', 'none');

            $('#liSecondChartMainLabel').text('Showing Sales>');
            $('#liSecondChart').text(SliceBy);
            $('#lblSecondChartInstruction').text('* Click on a ' + SliceBy + ' to break down data')

            GetPOSSalesReportBySlice();
        }

        var dataTableSecondChart;

        function GetPOSSalesReportByCustomer() {
            var pData = {
                Fowst: m_DashBoardChartsData.Fowst,
                From: m_DashBoardChartsData.From,
                To: m_DashBoardChartsData.To
            };
            AjaxSend({
                url: 'DashboardsAndReports/GetPOSSalesReportByCustomer',
                data: pData,
                success: function (vData) {
                    DisplaySalesReportByCustomer(vData);
                    
                }
            }); //end ajaxsend

        }  // END function GetPOSSalesReportByCustomer() {

        function DisplaySalesReportByCustomer(vData) {

            var vRows = vData.ReportPOS.Rows;

            $('#btnDownloadSliceAsExcel').css('visibility', 'visible');
            //$('#tblSecondChart').html('');

            if ($.fn.DataTable.isDataTable($('#tblSecondChart'))) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                $('#tblSecondChart').empty();
                dataTableSecondChart.destroy();
            }

            var buttons = [];
            if (!IsMobile) {
                buttons.push({ extend: 'csvHtml5', text: 'Download CSV', title: 'Sasles Report By Customer' });
            }

            dataTableSecondChart = $('#tblSecondChart').DataTable({
                pageLength: 25,
                //"order": [[ 4, "desc" ]],
                "data": vRows,
                "columns": [
                    {
                        title: "Customer",
                        "data": function (data) {
                            return CStr2(data.CompanyName);
                        }
                    },
                    {   
                        title: 'Gross Invoice Amount',
                        "data": function (data) {
                            return AccountingFormat(data.TotalAmount);
                        }
                    },
                    {   
                        title: 'Adjustments Amount',
                        "data": function (data) {
                            return AccountingFormat(data.AdjustmentAmount);
                        }
                    },
                    {   
                        title: 'Net Invoice Amount',
                        "data": function (data) {
                            return AccountingFormat(data.NetInvoiceAmount);
                        }
                    },
                    {   
                        title: 'Payment Amount',
                        "data": function (data) {
                            return AccountingFormat(data.TotalPayment);
                        }
                    },
                    {   
                        title: 'Net Invoice Balance',
                        "data": function (data) {
                            return AccountingFormat(data.NetInvoiceBalance);
                        }
                    }
                ],
                dom: 'Brtip',
                buttons: buttons
            });

            $('#tblSecondChart thead tr').addClass('custom-theme-f');
            $('#dvSlice').css('display', 'block');

            //dataTableSecondChart.on('click', 'td', function () {
            //    var data = dataTableSecondChart.row(this).data();
            //    if (data != undefined) {
            //        m_Filter1 = data.ProductFeature;
            //        $('#panDice').popup('open');
            //        $('#liThirdChartMainLabel').text('Showing Sales > ' + data.ProductFeature + ' > ');
            //        ShowHideDiceList();
            //    }
            //});
          
        } // END function DisplaySalesReportByCustomer(vData) {

        function GetPOSSalesReportBySlice() {
            var m_Totals = [];
            var m_TotalGross = 0;
            var m_TotalTransactionCount = 0;
            var m_TotalProductCount = 0;

            AjaxSend({
                url: 'DashboardsAndReports/GetPOSSalesReportBySlice',
                data: m_DashBoardChartsData,
                success: function (vData) {
                    if (vData.ReportPOS.Rows.length > 0) {

                        var vRows = vData.ReportPOS.Rows;

                        $('#btnDownloadSliceAsExcel').css('visibility', 'visible');
                        m_Totals[0] = [];
                        m_TicksBar = [];
                        m_Labels = [];

                        var tData = '';
                        var i = 0;

                        if ($.fn.DataTable.isDataTable($('#tblSecondChart'))) {
                            //empty the table first before destroying datatabase para dili magdoble ang header
                            $('#tblSecondChart').empty();
                            dataTableSecondChart.destroy();
                        }

                        var buttons = [];
                        if (!IsMobile) {
                            buttons.push({ extend: 'csvHtml5', text: 'Download CSV', title: m_Slice + ' breakdown' })
                        }


                        m_Totals[1] = [];

                        if (m_Slice == 'Time') {
                            for (var j = 0; j < 24; j++) {
                                m_TicksBar[j] = j + ':00';
                                m_Labels[j] = 0;
                                m_Totals[0][j] = 0;
                                m_Totals[1][j] = 0;
                            }
                        }

                        //additional by janine. show count. october 30 5pm

                        for (i = 0; i <= vRows.length - 1; i++) {
                            if (m_Slice == 'Time') {
                                for (var j = 0;j<24;j++){
                                    if (vRows[i].ProductFeature == j + ':00') {
                                        m_Totals[0][j] = $.format.number(vRows[i].TransactionCount, vRows[i].ProductFeature);
                                    }
                                }
                            } else {
                                m_Totals[0][i] = $.format.number(vRows[i].TransactionCount, vRows[i].ProductFeature);

                                m_TotalTransactionCount = m_TotalTransactionCount + CDec2(vRows[i].TransactionCount);

                                m_TotalProductCount = m_TotalProductCount + (CDec2(vRows[i].ProductCount) == 0 ? CDec2(vRows[i].ProductCountSO) : CDec2(vRows[i].ProductCount));
                            }                            
                        }

                        //for chart, it will sort desc

                        for (i = 0; i <= vRows.length - 1; i++) {

                            if (m_Slice == 'Time') {
                                for (var j = 0; j < 24; j++) {
                                    if (vRows[i].ProductFeature == j + ':00') {
                                        m_Totals[1][j] = $.format.number(vRows[i].Total, vRows[i].ProductFeature);
                                    }
                                }
                            } else {
                                m_Totals[1][i] = $.format.number(vRows[i].Total, vRows[i].ProductFeature);                                
                            }

                            m_TotalGross = m_TotalGross + $.format.number(vRows[i].Total);
                        }
                        //end of additional

                        //manually sort desc for table details

                        dataTableSecondChart = $('#tblSecondChart').DataTable({
                            pageLength: 25,
                            "order": [[ 4, "desc" ]],
                            "data": vRows,
                            "columns": [
                                {
                                    title: m_Slice,
                                    "data": function (data) {
                                        return CStr2(data.ProductFeature);
                                    }
                                },
                                {
                                    title: "Category",
                                    "data": function (data) {
                                        return CStr2(data.Category);
                                    }
                                },
                                {   
                                    title: 'Gross Sales',
                                    "data": function (data) {
                                        return AccountingFormat($.format.number(data.Total))
                                    }
                                },
                                {
                                    title: 'Contribution Margin',
                                    "data": function (data) {
                                        return AccountingFormat((parseFloat(data.Total) / parseFloat(m_TotalGross)) * 100) + '%';
                                    }
                                },
                                {
                                    title: 'Transaction Count',
                                    "data": function (data) {
                                        return CDec2(data.TransactionCount);
                                    }
                                },
                                {
                                    title: 'Product Count',
                                    "data": function (data) {
                                        return CDec2((CDec2(data.ProductCount) == 0 ? data.ProductCountSO : data.ProductCount))
                                    }
                                }
                            ],
                            dom: 'Brtip',
                            buttons: buttons,
                            initComplete: function () {
                                $('#tblSecondChart').removeClass('dataTable');
                            },
                            footerCallback: function (row, data, start, end, display) {
                                var api = this.api(), data;

                                // Remove the formatting to get integer data for summation
                                var intVal = function (i) {
                                    return typeof i === 'string' ?
                                        i.replace(/[\$,]/g, '') * 1 :
                                        typeof i === 'number' ?
                                            i : 0;
                                };
                                for (i = 1; i <= 5; i++) {
                                    // Total over all pages

                                    total = api
                                        .column(i)
                                        .data()
                                        .reduce(function (a, b) {
                                            return intVal(CDec2(a)) + intVal(CDec2(CStr2(b).replace(/%/g, '')));
                                        }, 0);

                                    // Total over this page
                                    pageTotal = api
                                        .column(i, { page: 'current' })
                                        .data()
                                        .reduce(function (a, b) {
                                            return intVal(CDec2(a)) + intVal(CDec2(b));
                                        }, 0);

                                    // Update footer
                                    $(api.column(i).footer()).html(
                                        AccountingFormat(total)
                                    );
                                }
                            }
                        });

                        if (m_Slice != 'Product') {
                            dataTableSecondChart.column(1).visible(false);
                        }

                        $('#tblSecondChart thead tr').addClass('custom-theme-f');

                        dataTableSecondChart.on('click', 'td', function () {
                            var data = dataTableSecondChart.row(this).data();
                            if (data != undefined) {
                                m_Filter1 = data.ProductFeature;
                                GetAllFeatures('dice');
                                $('#liThirdChartMainLabel').text('Showing Sales > ' + data.ProductFeature + ' > ');
                                ShowHideDiceList();
                            }
                        });
                        
                        for (i = vRows.length - 1; i >= 0; i--) {
                            if (m_Slice == 'Time') {
                                for (var j = 0; j < 24; j++) {
                                    if (m_TicksBar[j] == vRows[i].ProductFeature) {
                                        m_Labels[j] = AccountingFormat($.format.number(vRows[i].Total));
                                    }
                                }
                            } else {
                                m_TicksBar[i] = vRows[i].ProductFeature;
                                m_Labels[i] = AccountingFormat($.format.number(vRows[i].Total));
                            }
                        }

                        if (m_Slice == 'Time') {
                            DisplaySliceBarGraphVertical(m_Totals);
                        } else {
                            DisplaySliceBarGraph(m_Totals);
                        }
                        
                    }
                }
            }); //end ajaxsend
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
                        barDirection: 'horizontal',
                        barWidth: 40
                    }
                },
                axes: {
                    xaxis: {
                        drawBaseline: false
                    },
                    yaxis: {
                        renderer: $.jqplot.CategoryAxisRenderer,
                        ticks: m_TicksBar,
                        tickOptions: {
                            fontSize: '9pt'
                        }
                    }
                },
                highlighter: { show: false }
            });

            var scHeight = plot3._height = (plot3.axes.yaxis._dataBounds.max * 40) + 300;

            $('#dvSecondChart .jqplot-xaxis-tick').css('display', 'none');
            $('#dvSecondChart').css('height', scHeight);

            plot3.replot();
        }

        //for 2nd chart
        function DisplaySliceBarGraphVertical(m_Totals) {

            $('#dvSecondChart').empty('');

            var plot3 = $.jqplot('dvSecondChart', m_Totals, {
                animate: !$.jqplot.use_excanvas,
                seriesDefaults: {
                    renderer: $.jqplot.BarRenderer,
                    pointLabels: {
                        show: true
                    },
                    rendererOptions: {
                        barDirection: 'vertical'
                    }
                },
                axes: {
                    xaxis: {
                        renderer: $.jqplot.CategoryAxisRenderer,
                        ticks: m_TicksBar
                    },
                    yaxis: {
                        min: 0
                    }
                },
                highlighter: { show: true, showLabel: true } 
            });

        }

        function ShowHideDiceList() {
            $('#lstDice li a').each(function () {
                $(this).show();
            });

            //$('#Dice' + m_Slice.replace(' ', '').replace('(', '').replace(')', '').replace('/', '').replace('\'', '').replace('Slice', '')).hide();

            //$('#lstDice').listview('refresh');
        }

        function DiceData(DiceBy, table, divChart, div) {
            var data;
            var Fowst;

            //Fowst = GetSelectedFowst();

            m_Dice = DiceBy;

            if (Fowst == 0) {
                NewMessageBox("Please select FOWST",
                                "FOWST",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                            );
                $('#selDice').val('');
                //$('#selDice').selectmenu('refresh');
                return;
            }

            switch (DiceBy) {
                case "Time":
                    m_DashBoardChartsData.SliceBy = 'optTime';
                    m_DashBoardChartsData.Filter1Option = m_Slice;
                    m_DashBoardChartsData.Filter1 = m_Filter1;
                    window.location = '#' + divChart;
                    break;
                case "Brand":
                    m_DashBoardChartsData.SliceBy = 'optBrand';
                    m_DashBoardChartsData.Filter1Option = m_Slice;
                    m_DashBoardChartsData.Filter1 = m_Filter1;
                    window.location = '#' + divChart;
                    break;
                case "Customer Industry":
                    m_DashBoardChartsData.SliceBy = 'optCustomerIndustry';
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

            $('#' + div).css('display', 'block');

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

        var dataTableThirdChart;

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

                        if ($.fn.DataTable.isDataTable($('#' + table))) {
                            //empty the table first before destroying datatabase para dili magdoble ang header
                            $('#' + table).empty();
                            dataTableThirdChart.destroy();
                        }

                        var buttons = [];
                        if (!IsMobile) {
                            buttons.push({ extend: 'csvHtml5', text: 'Download CSV', title: m_FilterLabel + ' breakdown' })
                        }

                        m_Totals[0] = [];
                        m_TicksPie = [];
                        m_Labels = [];

                        m_Totals[1] = [];

                        if (m_Dice == 'Time') {
                            for (var j = 0; j < 24; j++) {
                                m_TicksPie[j] = j + ':00';
                                m_Labels[j] = 0;
                                m_Totals[0][j] = 0;
                                m_Totals[1][j] = 0;
                            }
                        }

                        var tData = '';

                        //additional by janine. show count. october 30 5pm

                        for (i = 0; i <= vRows.length - 1; i++) {

                            if (m_Dice == 'Time') {
                                for (var j = 0; j < 24; j++) {
                                    if (vRows[i].ProductFeature == j + ':00') {
                                        m_Totals[0][j] = $.format.number(vRows[i].TransactionCount, vRows[i].ProductFeature);
                                    }
                                }
                            } else {

                                m_Totals[0][i] = $.format.number(vRows[i].TransactionCount, vRows[i].ProductFeature);
                            }

                            m_TotalTransactionCount = m_TotalTransactionCount + CDec2(vRows[i].TransactionCount);

                            m_TotalProductCount = m_TotalProductCount + (CDec2(vRows[i].ProductCount) == 0 ? CDec2(vRows[i].ProductCountSO) : CDec2(vRows[i].ProductCount));
                        }
                        //for chart, it will sort desc
                        //m_Totals[1] = [];
                        for (i = 0; i <= vRows.length - 1; i++) {
                            if (m_Dice == 'Time') {
                                for (var j = 0; j < 24; j++) {
                                    if (vRows[i].ProductFeature == j + ':00') {
                                        m_Totals[1][j] = $.format.number(vRows[i].Total, vRows[i].ProductFeature);
                                    }
                                }
                            } else {
                                m_Totals[1][i] = $.format.number(vRows[i].Total, vRows[i].ProductFeature);
                            }

                            m_TotalGross = m_TotalGross + $.format.number(vRows[i].Total);
                        }
                        //end of additional
                        
                        dataTableThirdChart = $('#' + table).DataTable({
                            pageLength: 25,
                            "order": [[4, "desc"]],
                            "data": vRows,
                            "columns": [
                                {
                                    title: m_FilterLabel,
                                    "data": function (data) {
                                        return CStr2(data.ProductFeature);
                                    }
                                },
                                {
                                    title: "Category",
                                    "data": function (data) {
                                        return CStr2(data.Category);
                                    }
                                },
                                {
                                    title: 'Gross Sales',
                                    "data": function (data) {
                                        return AccountingFormat($.format.number(data.Total))
                                    }
                                },
                                {
                                    title: 'Contribution Margin',
                                    "data": function (data) {
                                        return AccountingFormat((parseFloat(data.Total) / parseFloat(m_TotalGross)) * 100) + '%';
                                    }
                                },
                                {
                                    title: 'Transaction Count',
                                    "data": function (data) {
                                        return CDec2(data.TransactionCount);
                                    }
                                },
                                {
                                    title: 'Product Count',
                                    "data": function (data) {
                                        return CDec2((CDec2(data.ProductCount) == 0 ? data.ProductCountSO : data.ProductCount))
                                    }
                                }
                            ],
                            dom: 'Brtip',
                            buttons: buttons
                        });

                        if (m_Dice != 'Product') {
                            dataTableThirdChart.column(1).visible(false);
                        }

                        $('#' + table + ' thead tr').addClass('custom-theme-f');

                        for (var i = vRows.length - 1; i >= 0; i--) {
                            if (m_Dice == 'Time') {
                                for (var j = 0; j < 24; j++) {
                                    if (m_TicksPie[j] == vRows[i].ProductFeature) {
                                        m_Labels[j] = AccountingFormat($.format.number(vRows[i].Total));
                                    }
                                }
                            } else {                                
                                m_Labels[i] = AccountingFormat($.format.number(vRows[i].Total));
                                m_TicksPie[i] = vRows[i].ProductFeature;
                            }
                        }

                        if (m_Dice == 'Time') {
                            DisplayDiceBarGraphVertical(m_Totals);
                        } else {
                            DisplayDiceBarGraph(m_Totals);
                        }
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
                        barDirection: 'horizontal',
                        barWidth: 40
                    }
                },
                axes: {
                    xaxis: {
                        drawBaseline: false
                    },
                    yaxis: {
                        renderer: $.jqplot.CategoryAxisRenderer,
                        ticks: m_TicksPie,
                        tickOptions: {
                            fontSize: '9pt'
                        }
                    }
                },
                highlighter: { show: false }
            });

            var scHeight = plot1._height = (plot1.axes.yaxis._dataBounds.max * 40) + 300;

            $('#dvThirdChart .jqplot-xaxis-tick').css('display', 'none');
            $('#dvThirdChart').css('height', scHeight);

            plot1.replot();
        }

        function DisplayDiceBarGraphVertical(m_Totals) {

            $('#dvThirdChart').empty('');

            var plot3 = $.jqplot('dvThirdChart', m_Totals, {
                animate: !$.jqplot.use_excanvas,
                seriesDefaults: {
                    renderer: $.jqplot.BarRenderer,
                    pointLabels: {
                        show: true
                    },
                    rendererOptions: {
                        barDirection: 'vertical'
                    }
                },
                axes: {
                    xaxis: {
                        renderer: $.jqplot.CategoryAxisRenderer,
                        ticks: m_TicksPie
                    },
                    yaxis: {
                        min: 0
                    }
                },
                highlighter: { show: true, showLabel: true }
            });

        }

        window.ClearCharts = function () {

            $('#dvSalesTrends').css('visibility', 'hidden');
            $('#dvSalesTrends').css('display', 'none');

            $('#dvMainChart').empty();
            $('#dvSecondChart').empty();
            $('#dvThirdChart').empty();

            //$('#tblSecondChart').empty();
            $('#tblThirdChart').empty();

            $('#tdTotalGrossSales').html("0");
            $('#tdSalesToday').html("0");

            $('#dvSlice').css('display', 'none');
            $('#dvDice').css('display', 'none');

            $('#btnDownloadSliceAsExcel').css('visibility', 'hidden');
            $('#btnDownloadDiceAsExcel').css('visibility', 'hidden');

            $('#selSlice').val('');
            $('#selDice').val('');

            $('#selSlice').selectmenu('refresh');
            $('#selDice').selectmenu('refresh');

            $('#tblSalesOrder').css('visibility', 'hidden');
            $('#tblSalesOrderWeek').css('visibility', 'hidden');

            $('#IsDataPresent').html('-Graph will be shown when you click on a group or FOWST (Factory, Office, Store, Warehouse or Truck) name-')
            $('#IsDataPresent').css('display', 'none');
            $('#IsDataPresent').css('lnkTransactionDetails', 'hidden');
        }



        window.GetSearchData = function (data) {

            m_DashBoardChartsData = data;
            GetTransactionDetails(data);
        }

        //Main Dashboard Popup
        //========================================== Sales Report =====================================

       

        var mSalesReportFrom;
        var mSalesReportTo;
        var FOWSTOrGroupID = [];
        var FOWSTOrGroupName = [];
        var SliceByName = [];

        var FOWSTOrGroupNameString = '';
        var SliceByFilter = ''; 
        var SelectedGraph = '';
        var DiceByFilter = '';
        var SelectedSliceGraph = '';

        var SliceBy = '';
        var DiceBy = '';

        $('#selSliceByGroups').unbind('change').change(function () {
            GetGroupSalesReport($('#selSliceByGroups').val(), $('#selSliceByGroups :selected').text());
            sessionStorage.SelectedGroup = $('#selSliceByGroups').val();
            if ($('#selSliceByGroups').val() == 'All' || $('#selSliceByGroups').val() == 'Group') {
                GetTopRankingStores(false);
            } else {
                GetTopRankingStores(true);
            }            
        })

        $('#btnPeriodSlice').click(function () {
            PopupPeriod({
                Click: function (e) {
                    if (e.Period == 'Custom') {
                        $('#btnPeriodSlice').text(e.From + ' ' + e.To);
                    } else {
                        $('#btnPeriodSlice').text(e.Period);
                    }
                    sliceDateFrom = e.From;
                    sliceDateTo = e.To;
                    GetGroupSalesReport($('#selSliceByGroups').val(), $('#selSliceByGroups :selected').text());
                    GetTopRankingStores(true, e)
                }
            })
        })

        function GetGroupSalesReport(SelectedGroupID, SelectedGroup) {
        
            mSalesReportFrom = (sliceDateFrom == null ? $('#txtDateFrom').text() : sliceDateFrom);
            mSalesReportTo = (sliceDateTo == null ? $('#txtDateTo').text() : sliceDateTo);

            if (CStr2(mSalesReportFrom) == '' || CStr2(mSalesReportTo) == '') {
                return;
            }

            sessionStorage.SelectedGroup = SelectedGroupID; //para magamit sa charts

            var data = {
                dateFrom: mSalesReportFrom,
                dateTo: mSalesReportTo,
                GroupFOWSTID: SelectedGroupID
            }
            AjaxSend({
                url: 'CincoCorporation/GetSalesReportSliceAndDice',
                data: data,
                success: function (vData) {
                    
                    var mLabel = [];
                    var mData = [];
                    var mColor = [];
                    var pData = {};
                    var dataset = {};
                                        
                    for (var i = 0; i <= vData.Results.Rows.length - 1; i++) {

                        var randomColor = Math.floor(Math.random() * 16777215).toString(16);

                        mData[i] = CDec2(vData.Results.Rows[i].GrossSales);
                        mLabel[i] = vData.Results.Rows[i].FOWSTOrGroupName;
                        mColor[i] = "#" + randomColor;
                        FOWSTOrGroupID[i] = vData.Results.Rows[i].ID;
                        FOWSTOrGroupName[i] = vData.Results.Rows[i].FOWSTOrGroupName;
                    }

                    //dataset.label = SelectedGroup; //inner label
                    dataset.label = SelectedGroup; //inner label
                    dataset.data = mData;
                    dataset.backgroundColor = mColor;

                    pData.labels = mLabel;//outer label
                    pData.datasets = [dataset];

                    DisplayGroupSalesReportChart(pData);
                    //DisplayGroupSalesReportTable(vData.Rows);
                }
            });
        }

        var chartGroupSalesReport = null;        

        function DisplayGroupSalesReportChart(pData) {
            var groupchart = document.getElementById('chartGroupSalesReport');

            if (chartGroupSalesReport != null) {
                chartGroupSalesReport.destroy();
            }

            chartGroupSalesReport = new Chart(groupchart, {
                type: 'bar',
                data: [pData][0],
                options: {
                    barThickness: 50,
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: {
                        xAxes: [{
                            //stacked: true,
                            stacked: false,
                            ticks: {
                                beginAtZero: true,
                                maxRotation: 0,
                                minRotation: 0
                            }
                        }],
                        yAxes: [{
                            stacked: false,
                        }]
                    },
                    plugins: {
                        legend: {
                            position: 'top',
                        },
                        title: {
                            display: true,
                            text: 'Click graph to breakdown sales'
                        },
                    },
                    onClick: function (event, item) {
                        if (chartGroupSalesReport.getActiveElements(event)[0] == undefined) return;

                        var clickIndex = chartGroupSalesReport.getActiveElements(event)[0].index;
                        
                        SliceByFilter = FOWSTOrGroupID[clickIndex];
                        FOWSTOrGroupNameString = FOWSTOrGroupName[clickIndex];

                        SelectedGraph = 'ForSlice';

                        GroupSalesReportChart();
                                                
                    }
                },
                
            });
        }

        function GroupSalesReportChart() {
            AjaxSend({
                url: 'DashboardsAndReports/GetOrganizationProductFeatures',
                success: function (vData) {
                    DisplayGroupSalesProductFeatures(vData.Results.Rows);
                }
            });
        }

        function DisplayGroupSalesProductFeatures(vRows) {
            var pLV = $('#lstSlice');
            var pLV2 = $('#lstDice');
            var options = [
                //{
                //    Text: 'Time', Click: function () {
                //        m_Slice = 'Time';
                //        if (SelectedGraph == 'ForSlice') {
                //            SliceBy = 'Time';
                //            GetGroupSliceData();
                //            ShowHideDiceList();
                //        } else {
                //            DiceBy = 'Time';
                //            GroupSalesReportDiceData();
                //        }          
                //    }
                //},
                {
                    Text: 'Brand', Click: function () {
                        m_Slice = 'Brand';
                        if (SelectedGraph == 'ForSlice') {
                            SliceBy = 'Brand';
                            GetGroupSliceData();
                            ShowHideDiceList();
                        } else {
                            DiceBy = 'Brand';
                            GroupSalesReportDiceData();
                        }
                        
                    }
                },
                //{
                //    Text: 'Sales Representative', Click: function () {
                //        m_Slice = 'SalesRepresentative';
                //        if (SelectedGraph == 'ForSlice') {
                //            SliceBy = 'SalesRepresentative';
                //            GetGroupSliceData();
                //            ShowHideDiceList();
                //        } else {
                //            DiceBy = 'SalesRepresentative';
                //            GroupSalesReportDiceData();
                //        }
                //    }
                //}, {
                //    Text: 'Division', Click: function () {
                //        m_Slice = 'Division';
                //        if (SelectedGraph == 'ForSlice') {
                //            SliceBy = 'Division';
                //            GetGroupSliceData();
                //            ShowHideDiceList();
                //        } else {
                //            DiceBy = 'Division';
                //            GroupSalesReportDiceData();
                //        }
                //    }
                //},
                {
                    Text: 'Product', Click: function () {
                        m_Slice = 'Product';
                        if (SelectedGraph == 'ForSlice') {
                            SliceBy = 'Product';
                            GetGroupSliceData();
                            ShowHideDiceList();
                        } else {
                            DiceBy = 'Product';
                            GroupSalesReportDiceData();
                        }
                    }
                }
                //, {
                //    Text: 'Delivery Method', Click: function () {
                //        m_Slice = 'Delivery Method';
                //        if (SelectedGraph == 'ForSlice') {
                //            SliceBy = 'Delivery Method';
                //            GetGroupSliceData();
                //            ShowHideDiceList();
                //        } else {
                //            DiceBy = 'Delivery Method';
                //            GroupSalesReportDiceData();
                //        }
                //    }
                //}, {
                //    Text: 'Order Method', Click: function () {
                //        m_Slice = 'Order Method';
                //        if (SelectedGraph == 'ForSlice') {
                //            SliceBy = 'Order Method';
                //            GetGroupSliceData();
                //            ShowHideDiceList();
                //        } else {
                //            DiceBy = 'Order Method';
                //            GroupSalesReportDiceData();
                //        }
                //    }
                //}, {
                //    Text: 'Gender', Click: function () {
                //        m_Slice = 'Gender';
                //        if (SelectedGraph == 'ForSlice') {
                //            SliceBy = 'Gender';
                //            GetGroupSliceData();
                //            ShowHideDiceList();
                //        } else {
                //            DiceBy = 'Gender';
                //            GroupSalesReportDiceData();
                //        }
                //    }
                //}, {
                //    Text: 'Age', Click: function () {
                //        m_Slice = 'Age';
                //        if (SelectedGraph == 'ForSlice') {
                //            SliceBy = 'Age';
                //            GetGroupSliceData();
                //            ShowHideDiceList();
                //        } else {
                //            DiceBy = 'Age';
                //            GroupSalesReportDiceData();
                //        }
                //    }
                //}, {
                //    Text: 'Customer', Click: function () {
                //        GetPOSSalesReportByCustomer();
                //    }
                //}
            ];

            for (i = 0; i <= vRows.length - 1; i++) {
                options.push({
                    data: vRows[i], Text: vRows[i].Label, Click: function (e) {
                        if (SelectedGraph == 'ForSlice') {
                            SliceBy = e.Label;
                            GetGroupSliceData();
                        } else {
                            DiceBy = e.Label;
                            GroupSalesReportDiceData();
                        }
                        
                                              
                    }
                })
            }

            PopupButtonList('', 'Sales Distribution', options);
        }
        
        //DisplayGroupSalesFeatures
        function GetGroupSliceData() {

            GetGroupSalesReportBySlice();
      
        }

        var ProductFeature = [];
        var DiceByFilter = '';

        function GetGroupSalesReportBySlice() {
            var data = {
                dateFrom: mSalesReportFrom,
                dateTo: mSalesReportTo,
                GroupFOWSTID: sessionStorage.SelectedGroup,
                SliceBy: SliceBy,
                SliceByFilter: SliceByFilter,
                DiceBy: ''
            }
            
            AjaxSend({
                url: 'CincoCorporation/GetSalesReportSliceAndDice',
                data: data,
                success: function (vData) {

                    //for chart
                    var mLabel = [];
                    var mData = [];
                    var mColor = [];
                    var pData = {};
                    var dataset = {};
                    var m_TotalGross = 0;

                    for (var i = 0; i <= vData.Results.Rows.length - 1; i++) {

                        var randomColor = Math.floor(Math.random() * 16777215).toString(16);

                        mData[i] = CDec2(vData.Results.Rows[i].Total);
                        mLabel[i] = vData.Results.Rows[i].ProductFeature;
                        mColor[i] = "#" + randomColor;

                        ProductFeature[i] = vData.Results.Rows[i].ProductFeature;

                        m_TotalGross += CDec2(vData.Results.Rows[i].Total);
                    }

                    //dataset.label = SelectedGroup; //inner label
                    dataset.label = SliceBy; //inner label
                    dataset.data = mData;
                    dataset.backgroundColor = mColor;

                    pData.labels = mLabel;//outer label
                    pData.datasets = [dataset];

                    $('#lblSliceByLabel').text('Showing ' + FOWSTOrGroupNameString + '> ' + SliceBy);

                    GroupSalesReportSliceChart(pData);
                    //end of chart

                    //for table

                    if ($.fn.DataTable.isDataTable($('#tblGroupSalesReportTable1'))) {
                        $('#tblGroupSalesReportTable1').empty();
                        dataTableSecondChart.destroy();
                    }

                    var buttons = [];
                    if (!IsMobile) {
                        buttons.push({ extend: 'csvHtml5', text: 'Download CSV', title: 'Sasles Report By Customer' });
                    }

                    dataTableSecondChart = $('#tblGroupSalesReportTable1').DataTable({
                        pageLength: 25,
                        "order": [[4, "desc"]],
                        "data": vData.Results.Rows,
                        "columns": [
                            {
                                title: SliceBy,
                                "data": function (data) {
                                    return CStr2(data.ProductFeature);
                                }
                            },
                            {
                                title: 'Gross Sales',
                                "data": function (data) {
                                    return AccountingFormat($.format.number(data.Total))
                                }
                            },
                            {
                                title: '% Contribution',
                                "data": function (data) {
                                    return AccountingFormat((parseFloat(data.Total) / parseFloat(m_TotalGross)) * 100) + '%';
                                }
                            },
                            {
                                title: 'Transaction Count',
                                "data": function (data) {
                                    return CDec2(data.TransactionCount);
                                }
                            },
                            {
                                title: 'Product Count',
                                "data": function (data) {
                                    return CDec2((CDec2(data.ProductCount) == 0 ? data.ProductCountSO : data.ProductCount))
                                }
                            }
                        ],
                        dom: 'Brtip',
                        buttons: buttons
                    });
                    //end of table
                }
            }); //end ajaxsend
        }

        var chartGroupSalesReport1 = null;

        function GroupSalesReportSliceChart(pData) {
            var groupchart = document.getElementById('chartGroupSalesReport1');

            if (chartGroupSalesReport1 != null) {
                chartGroupSalesReport1.destroy();
            }

            
            //indexAxis: 'y',
            chartGroupSalesReport1 = new Chart(groupchart, {
                type: 'bar',
                data: [pData][0],
                options: {
                    barThickness: 50,
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: {
                        xAxes: [{
                            //stacked: true,
                            stacked: false,
                            ticks: {
                                beginAtZero: true,
                                maxRotation: 0,
                                minRotation: 0
                            }
                        }],
                        yAxes: [{
                            stacked: false,
                        }]
                    }, plugins: {
                        legend: {
                            position: 'top',
                        },
                        title: {
                            display: true,
                            text: 'Click graph to breakdown sales'
                        },
                    },
                    onClick: function (event, item) {
                        if (chartGroupSalesReport1.getActiveElements(event)[0] == undefined) return;

                        var clickIndex = chartGroupSalesReport1.getActiveElements(event)[0].index;

                        DiceByFilter = ProductFeature[clickIndex];
                                                
                        SelectedGraph = 'ForDice';

                        GroupSalesReportChart();
                    }
                },

            });
        }
        
        function GroupSalesReportDiceData() {
            var data = {
                dateFrom: mSalesReportFrom,
                dateTo: mSalesReportTo,
                GroupFOWSTID: sessionStorage.SelectedGroup,
                SliceBy: SliceBy,
                SliceByFilter: SliceByFilter,
                DiceBy: DiceBy,
                DiceByFilter: DiceByFilter
            }

            AjaxSend({
                url: 'CincoCorporation/GetSalesReportSliceAndDice',
                data: data,
                success: function (vData) {

                    //====for charts
                    var mLabel = [];
                    var mData = [];
                    var mColor = [];
                    var pData = {};
                    var dataset = {};
                    var m_TotalGross = 0;

                    for (var i = 0; i <= vData.Results.Rows.length - 1; i++) {

                        var randomColor = Math.floor(Math.random() * 16777215).toString(16);

                        mData[i] = CDec2(vData.Results.Rows[i].Total);
                        mLabel[i] = vData.Results.Rows[i].ProductFeature;
                        mColor[i] = "#" + randomColor;

                        ProductFeature[i] = vData.Results.Rows[i].ProductFeature;
                        m_TotalGross += 0;
                    }

                    //dataset.label = SelectedGroup; //inner label
                    dataset.label = SliceBy; //inner label
                    dataset.data = mData;
                    dataset.backgroundColor = mColor;

                    pData.labels = mLabel;//outer label
                    pData.datasets = [dataset];

                    
                    $('#lblDiceByLabel').text('Showing ' + FOWSTOrGroupNameString + '> ' + SliceBy + '> ' + DiceBy);

                    GroupSalesReportDiceChart(pData);
                    //===end of charts  

                    if ($.fn.DataTable.isDataTable($('#tblGroupSalesReportTable2'))) {
                        $('#tblGroupSalesReportTable2').empty();
                        dataTableSecondChart.destroy();
                    }

                    //for table

                    var buttons = [];
                    if (!IsMobile) {
                        buttons.push({ extend: 'csvHtml5', text: 'Download CSV', title: 'Sasles Report By Customer' });
                    }

                    dataTableSecondChart = $('#tblGroupSalesReportTable2').DataTable({
                        pageLength: 25,
                        "order": [[4, "desc"]],
                        "data": vData.Results.Rows,
                        "columns": [
                            {
                                title: SliceBy,
                                "data": function (data) {
                                    return CStr2(data.ProductFeature);
                                }
                            },
                            {
                                title: 'Gross Sales',
                                "data": function (data) {
                                    return AccountingFormat($.format.number(data.Total))
                                }
                            },
                            {
                                title: '% Contribution',
                                "data": function (data) {
                                    return AccountingFormat((parseFloat(data.Total) / parseFloat(m_TotalGross)) * 100) + '%';
                                }
                            },
                            {
                                title: 'Transaction Count',
                                "data": function (data) {
                                    return CDec2(data.TransactionCount);
                                }
                            },
                            {
                                title: 'Product Count',
                                "data": function (data) {
                                    return CDec2((CDec2(data.ProductCount) == 0 ? data.ProductCountSO : data.ProductCount))
                                }
                            }
                        ],
                        dom: 'Brtip',
                        buttons: buttons
                    });
                    //end of table
                }
            }); //end ajaxsend
        }

        var chartGroupSalesReport2 = null;

        function GroupSalesReportDiceChart(pData) {
            var groupchart = document.getElementById('chartGroupSalesReport2');

            if (chartGroupSalesReport2 != null) {
                chartGroupSalesReport2.destroy();
            }

            //indexAxis: 'y',
            chartGroupSalesReport2 = new Chart(groupchart, {
                type: 'bar',
                data: [pData][0],
                options: {
                    barThickness: 50,
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: {
                        xAxes: [{
                            //stacked: true,
                            stacked: false,
                            ticks: {
                                beginAtZero: true,
                                maxRotation: 0,
                                minRotation: 0
                            }
                        }],
                        yAxes: [{
                            stacked: false,
                        }]
                    },
                    onClick: function (event, item) {
                       
                    }
                },

            });
        }

        //========================================== End of Sales Report =====================================
    });