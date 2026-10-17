require(['common', 'promise!/Scripts/jquery.filedownload.min.js'],
    function () {
        var systemwideDateFrom = $.format.date(new Date(new Date().setDate(new Date().getDate() - 1)), 'MM-dd-yyyy');
        var systemwideDateTo = $.format.date(new Date(new Date().setDate(new Date().getDate() - 1)), 'MM-dd-yyyy');
        var refreshIntervalSystemWide, refreshIntervalDetailedReport;
        var dttblSDCompleted, dttblSDNotSending, dttblSDExempted;
        var chartSummary, chartSummaryCO, chartSummaryFO;

        $(document).ready(function () {

            $('input[name="btnMPOSSalesDashboard"]').change(function () {
                var val = $(this).val();

                if (typeof refreshIntervalSystemWide == 'number') clearInterval(refreshIntervalSystemWide);
                if (typeof refreshIntervalDetailedReport == 'number') clearInterval(refreshIntervalDetailedReport);

                if (val == 'systemwide') {
                    $('#divSDSystemWide').pageshow();
                    $('#btnSDRefreshSystemWide').show();
                    $('#btnSDRefreshDetailedReport').hide();
                    GetOrganizationSummarySales();
                } else if (val == 'detailedreport') {
                    $('#divSDDetails').pageshow();
                    $('#btnSDRefreshSystemWide').hide();
                    $('#btnSDRefreshDetailedReport').show();
                    GetDashboardDetailedReport();
                }
            })

            $('#btnSystemWideSDDate').text($.format.date(new Date(systemwideDateFrom), 'MMM dd, yyyy')).click(function () {
                Bs.Calendar({
                    MaxDate: $.format.date(new Date(new Date().setDate(new Date().getDate() - 1)), 'MM-dd-yyyy'),
                    Click: function (e) {
                        systemwideDateFrom = e;
                        systemwideDateTo = e;
                        $('#btnSystemWideSDDate').text($.format.date(new Date(e), 'MMM dd, yyyy'));
                        if (typeof refreshIntervalSystemWide == 'number') clearInterval(refreshIntervalSystemWide);
                        GetOrganizationSummarySales();
                    }
                })
            })

            $('#selSDGroup, #selSDDetailedStatus').change(function () {
                Bs.Loading('show');
                GetMPOSDashboardDetails();
            })

            $('#selSDDetailedOwnership').change(function () {
                var val = $(this).val();

                if (val == 'All') {
                    $('[name="group_co"], [name="group_fo"]').show();
                } else if (val == 'Company-Owned') {
                    $('[name="group_co"]').show();
                    $('[name="group_fo"]').hide();
                } else if (val == 'Franchise') {
                    $('[name="group_co"]').hide();
                    $('[name="group_fo"]').show();
                }

                Bs.Loading('show');
                GetMPOSDashboardDetails();
            })

            $('#txtSDSearchDetails').keyup(function () {
                var value = $(this).val().toLowerCase();

                $('#tblSDCompleted tbody tr').filter(function () {
                    $(this).toggle(
                        $(this).text().toLowerCase().indexOf(value) > -1
                    );
                });

                $('#tblSDNotSending tbody tr').filter(function () {
                    $(this).toggle(
                        $(this).text().toLowerCase().indexOf(value) > -1
                    );
                });

                $('#tblSDExempted tbody tr').filter(function () {
                    $(this).toggle(
                        $(this).text().toLowerCase().indexOf(value) > -1
                    );
                });
            });

            $('#btnSDRefreshSystemWide').click(function () {
                GetOrganizationSummarySales();
            })

            $('#btnSDRefreshDetailedReport').click(function () {
                GetDashboardDetailedReport();
            })

            $('#selSDPOSVendor').change(function () {
                GetDashboardDetailedReport();
            })

            $('body').tooltip({ selector: '[data-toggle="tooltip"]', trigger: 'hover' });
        })

        function GetDashboardDetailedReport() {
            if (typeof refreshIntervalDetailedReport == 'number') clearInterval(refreshIntervalDetailedReport);

            $('#btnSDRefreshDetailedReport').html('<div class="spinner-border spinner-border-sm text-dark" role="status"></div> Updating...');

            AjaxSend({
                url: 'CincoCorporation/GetMPOSDashboardDetailedReport',
                data: { POSVendor: $('#selSDPOSVendor').val() },
                success: function (e) {
                    var report = ['Yesterday', 'MTD', 'YTD'];
                    var reporttype = ['Sales', 'TC'];                    

                    $('#DR_Yesterday, #DR_MTD, #DR_YTD').empty();

                    $('[name="lblDateYesterday"]').text($.format.date(new Date(new Date().setDate(new Date().getDate() - 1)), 'MMM dd, yyyy'));
                    $('[name="lblDateMTD"]').text($.format.date(new Date(), 'MMM 01')  + ' - ' + $.format.date(new Date(), 'dd, yyyy'));
                    $('[name="lblDateYTD"]').text('Jan 01 - ' + $.format.date(new Date(), 'MMM dd, yyyy'));

                    for (var i in report) {
                        for (var t in reporttype) {
                            var template = $('#templateDetailedReport').clone();

                            template.removeClass('d-none');
                            template.removeAttr('id');

                            var dSales = e[report[i] + '' + reporttype[t]][0];
                            var dMix = e[report[i] + '' + reporttype[t] + 'Mix'][0];

                            template.find('[name="lblSDTitle"]').text(reporttype[t] == 'TC' ? 'Transaction Count' : reporttype[t]);
                            template.find('[name="lblSD"]').text(reporttype[t]);
                            template.find('[name="lblSDActualSales"]').text(AccountingFormat(dSales.Actual, reporttype[t] == 'TC' ? 0 : 2, '0.00'));
                            template.find('[name="lblSDLYSDS"]').text(AccountingFormat(dSales.LastYearDate, 2, '0.00'));
                            template.find('[name="lblSDLYSDSGrowth"]').addClass(dSales.Growth > 0 ? 'text-success' : 'text-danger').text(AccountingFormat(dSales.Growth, 2, '0', '-', '') + '%');
                            template.find('[name="lblSDAvgDailySales"]').text(AccountingFormat(dSales.AvgDaily, 2, '0.00'));
                            template.find('[name="lblSDNoOfStores"]').text(AccountingFormat(dSales.StoreCount, 2, '0'));
                            template.find('[name="lblSDSSS"]').text(AccountingFormat(0, 2, '0.00'));
                            template.find('[name="lblSDSSSGrowth"]').addClass(0 > 0 ? 'text-success' : 'text-danger').text(AccountingFormat(0, 2, '0', '-', '') + '%');
                            template.find('[name="lblSDDineIn"]').text(AccountingFormat(dMix.DineIn, reporttype[t] == 'TC' ? 0 : 2, '0'));
                            template.find('[name="lblSDTakeOut"]').text(AccountingFormat(dMix.TakeOut, reporttype[t] == 'TC' ? 0 : 2, '0'));
                            template.find('[name="lblSDDelivery"]').text(AccountingFormat(dMix.Delivery, reporttype[t] == 'TC' ? 0 : 2, '0'));
                            template.find('[name="lblSDDriveThru"]').text(AccountingFormat(dMix.DriveThru, reporttype[t] == 'TC' ? 0 : 2, '0'));

                            if (report[i] == 'Yesterday') {
                                $('#DR_Yesterday').append(template);
                            } else if (report[i] == 'MTD') {
                                $('#DR_MTD').append(template);
                            } else if (report[i] == 'YTD') {
                                $('#DR_YTD').append(template);
                            }
                        }                        
                    }

                    $('#btnSDRefreshDetailedReport').html('<span class="bi-arrow-clockwise"></span> Last Update: ' + $.format.date(new Date(), 'MMM dd, yyyy, hh:mm a'));
                    RefreshDetailedReport();
                }
            })
        }

        function GetOrganizationSummarySales() {
            if (typeof refreshIntervalSystemWide == 'number') clearInterval(refreshIntervalSystemWide);

            $('#btnSDRefreshSystemWide').html('<div class="spinner-border spinner-border-sm text-dark" role="status"></div> Updating...');

            AjaxSend({
                url: 'CincoCorporation/GetOrganizationSummarySales',
                data: {
                    DateFrom: systemwideDateFrom,
                    DateTo: systemwideDateTo,
                }, success: function (e) {
                    var sumOrg = e.SummaryPerOwnership.Rows;
                    //var sumRegion = e.SummaryPerRegion.Rows;
                    var ctrCO = 0, ctrFO = 0;

                    $('[name="pgbar"]').css('width', '0%');
                    $('[name="lblSDPercentage"]').text('0 / 0');
                    $('[name="lblNoOfStoresMPOS"], [name="lblNoOfStores3PP"]').text(0);
                    $('[name="lblOverallMPOS"], [name="lblOverall3PP"]').text(0);
                    $('[name="lblCompletedMPOS"], [name="lblCompleted3PP"]').text(0);
                    $('[name="lblNotSendingMPOS"], [name="lblNotSending3PP"]').text(0);
                    $('[name="lblExemptedMPOS"], [name="lblExempted3PP"]').text(0);
                    $('[name="lblTotalNofStores"], [name="lblTotalOverall"], [name="lblTotalExempted"], [name="lblTotalCompleted"], [name="lblTotalNotSending"]').text(0);
                    $('[name="lblPercentageMPOS"], [name="lblPercentage3PP"]').text('0%');
                    chartSummaryCO = DisplaySummaryProgress('chartSDCOPercentage', 0, {});
                    chartSummaryFO = DisplaySummaryProgress('chartSDFOPercentage', 0, {});

                    $('#divSDCO').find('[name="lblNoOfStores3PP"]').text(CDec2(e['3PP-COStores']));
                    $('#divSDFO').find('[name="lblNoOfStores3PP"]').text(CDec2(e['3PP-FOStores']));

                    if (sumOrg.length > 0) {
                        for (var i in sumOrg) {
                            var parent, vRow = sumOrg[i];
                            if (vRow.OwnershipType == 'Company-Owned') {
                                parent = $('#divSDCO');
                            } else {
                                parent = $('#divSDFO');
                            }

                            if (vRow.GroupCategory == 'CO' || vRow.GroupCategory == 'FO') {
                                parent.find('[name="lblCompletedMPOS"]').text(CDec2(vRow.Completed));
                                parent.find('[name="lblNotSendingMPOS"]').text(CDec2(vRow.NotSending));
                                parent.find('[name="lblExemptedMPOS"]').text(CDec2(vRow.Exempted));
                                parent.find('[name="lblOverallMPOS"]').text(CDec2(vRow.Open));
                                parent.find('[name="lblPercentageMPOS"]').text(CDec2(AccountingFormat((CDec2(vRow.Completed) / CDec2(vRow.Open)) * 100, 1, '0')) + '%');

                                if (vRow.GroupCategory == 'CO') {
                                    parent.find('[name="lblNoOfStoresMPOS"]').text(CDec2(e['CO-OperationalStores']));
                                } else {
                                    parent.find('[name="lblNoOfStoresMPOS"]').text(CDec2(e['FO-OperationalStores']));
                                }

                                continue;
                            } else if (vRow.GroupCategory == 'Third Party POS') {
                                parent.find('[name="lblCompleted3PP"]').text(CDec2(vRow.Completed));
                                parent.find('[name="lblNotSending3PP"]').text(CDec2(vRow.NotSending));
                                parent.find('[name="lblExempted3PP"]').text(CDec2(vRow.Exempted));
                                parent.find('[name="lblOverall3PP"]').text(CDec2(vRow.Open));
                                parent.find('[name="lblPercentage3PP"]').text(CDec2(AccountingFormat((CDec2(vRow.Completed) / CDec2(vRow.Open)) * 100, 1, '0')) + '%');

                                continue;
                            }

                            parent.find('[name="lblSDPercentage"]').text(CDec2(vRow.Completed) + ' / ' + CDec2(vRow.Open));                            
                            parent.find('[name="lblTotalOverall"]').text(CDec2(vRow.Open));
                            parent.find('[name="lblTotalExempted"]').text(CDec2(vRow.Exempted));
                            parent.find('[name="lblTotalCompleted"]').text(CDec2(vRow.Completed));
                            parent.find('[name="lblTotalNotSending"]').text(CDec2(vRow.NotSending));

                            if (vRow.OwnershipType == 'Company-Owned' && vRow.GroupCategory == 'Overall') {
                                parent.find('[name="lblTotalNofStores"]').text(CDec2(e['CO-OperationalStores']) + CDec2(e['3PP-COStores']));
                                chartSummaryCO = DisplaySummaryProgress('chartSDCOPercentage', CDec2(AccountingFormat(vRow.Percentage, 1)), vRow);                                
                            } else if (vRow.OwnershipType == 'Franchise' && vRow.GroupCategory == 'Overall') {
                                parent.find('[name="lblTotalNofStores"]').text(CDec2(e['FO-OperationalStores']) + CDec2(e['3PP-FOStores']));
                                chartSummaryFO = DisplaySummaryProgress('chartSDFOPercentage', CDec2(AccountingFormat(vRow.Percentage, 1)), vRow);
                            }
                        }
                    }
                    
                    GetMPOSDashboardDetails();

                    //if (sumRegion.length > 0) {
                    //    for (var i in sumRegion) {
                    //        var tbl, ctrRow;

                    //        if (sumRegion[i].OwnershipType == 'Company-Owned') {
                    //            tbl = $('#tblCOSummaryPerRegion tbody');
                    //            ctrCO += 1;
                    //            ctrRow = ctrCO;
                    //        } else {
                    //            tbl = $('#tblFOSummaryPerRegion tbody');
                    //            ctrFO += 1;
                    //            ctrRow = ctrFO;
                    //        }

                    //        tbl.append($('<tr>')
                    //            .append('<td>' + ctrRow + '.</td>')
                    //            .append('<td>' + sumRegion[i].GroupName + '</td>')
                    //            .append('<td class="text-center">' + sumRegion[i].TransactionCount + '</td>')
                    //            .append('<td class="text-center">' + AccountingFormat(sumRegion[i].GrossSales) + '</td>')
                    //        )
                    //    }
                    //}
                }, error: function () {
                    //GetOrganizationSummarySales();
                }
            })
        }

        function DisplaySummaryProgress(canvas, percentage, vRow) {
            var ads = document.getElementById(canvas);
            if (chartSummaryCO != null && chartSummaryFO != null) {
                if (canvas == 'chartSDCOPercentage') {
                    chartSummaryCO.data.datasets[0].data = [percentage, 100 - percentage];
                    chartSummaryCO.options.plugins.centerText.text = percentage + '%';
                    chartSummaryCO.update();
                    return chartSummaryCO;
                } else {
                    chartSummaryFO.data.datasets[0].data = [percentage, 100 - percentage];
                    chartSummaryFO.options.plugins.centerText.text = percentage + '%';
                    chartSummaryFO.update();
                    return chartSummaryFO;
                }
            }

            chartSummary = new Chart(canvas, {
                type: 'doughnut',
                data: {
                    datasets: [{
                        data: [percentage, 100 - percentage],
                        backgroundColor: [
                            (canvas == 'chartSDCOPercentage' ? '#0d6efd' : '#28a745'), '#e9ecef'
                        ],
                        borderWidth: 0
                    }],
                },
                options: {
                    cutout: '80%',
                    plugins: {
                        tooltip: { enabled: false },
                        legend: { display: false },
                        title: {
                            display: false
                        },
                        centerText: {
                            text: percentage + '%',
                            font: 'bold 28px Outfit',
                            color: (canvas == 'chartSDCOPercentage' ? '#0d6efd' : '#28a745')
                        }
                    },
                    responsive: true,
                    maintainAspectRatio: false,
                    indexAxis: 'y',
                },
                plugins: [{
                    id: 'centerText',
                    beforeDraw(chart, args, options) {
                        const { ctx, width, height } = chart;
                        ctx.save();
                        ctx.font = options.font || 'bold 28px Arial';
                        ctx.fillStyle = options.color || '#333';
                        ctx.textAlign = 'center';
                        ctx.textBaseline = 'middle';
                        ctx.fillText(options.text || '', width / 2, height / 2);
                        ctx.restore();
                    }
                }]
            });

            return chartSummary;
        }

        function GetMPOSDashboardDetails() {
            if (typeof refreshIntervalSystemWide == 'number') clearInterval(refreshIntervalSystemWide);

            AjaxSend({
                url: 'CincoCorporation/GetMPOSDashboardDetails',
                data: {
                    Ownership: $('#selSDDetailedOwnership').val(),
                    Group: $('#selSDGroup').val(),
                    Status: $('#selSDDetailedStatus').val(),
                    DateFrom: systemwideDateFrom,
                    DateTo: systemwideDateTo
                }, success: function (e) {
                    dttblSDCompleted = PopulateDatatable(e.Completed.Rows, '#tblSDCompleted');
                    dttblSDNotSending = PopulateDatatable(e.NotSending.Rows, '#tblSDNotSending');
                    dttblSDExempted = PopulateDatatable(e.Exempted.Rows, '#tblSDExempted');

                    $('#btnSDRefreshSystemWide').html('<span class="bi-arrow-clockwise"></span> Last Update: ' + $.format.date(new Date(), 'MMM dd, yyyy, hh:mm a'));
                    RefreshSystemWide();
                }, error: function () {
                    //GetMPOSDashboardDetails();
                }
            })
        }

        function PopulateDatatable(data, table) {
            if ($.fn.DataTable.isDataTable($(table))) {
                $(table).empty();
                $(table).DataTable().destroy();
            }

            var buttons = [];
            if (!IsMobile) {
                buttons.push({ extend: 'csvHtml5', text: 'Download CSV', title: '' })
            }

            var ctr = 0;

            var datatable = $(table).DataTable({
                pageLength: 25,
                "ordering": false,
                "data": data,
                stateSave: true,
                "columns": [
                    {
                        "data": function (data) {
                            ctr += 1;
                            return ctr + '.';
                        }, width: '50px'
                    },
                    { "data": "GroupName", width: '150px' },
                    { "data": "FOWSTCode", width: '150px' },
                    { "data": "FOWSTName" },
                    //{
                    //    "data": function (data) {
                    //        var bg;

                    //        if (data.Status == 'Utilized') {
                    //            bg = 'bg-success';
                    //        } else if (data.Status == 'Not Utilized') {
                    //            bg = 'bg-warning';
                    //        } else if (data.Status == 'Closed') {
                    //            bg = 'bg-danger';
                    //        }

                    //        return '<span class="badge '+ bg +'">' + data.Status + '</span>';
                    //    }, width: '150px', className: 'text-center'
                    //},
                    {
                        "data": function (data) {
                            return AccountingFormat(data.GrossSales)
                        }, width: '150px', className: 'text-center'
                    }
                ],
                dom: 'tip',
                buttons: buttons,
                initComplete: function () {
                    Bs.Loading('hide');
                    $(table).css('width', '100%').removeClass('dataTable');
                }
            });

            return datatable;
        }

        function RefreshSystemWide() {
            refreshIntervalSystemWide = setInterval(function () {
                if ($('#divNewDashboard').css('display') == 'block' && $('#divSDSystemWide').css('display') == 'block') {
                    clearInterval(refreshIntervalSystemWide);
                    GetOrganizationSummarySales();
                }
            }, 60000)
        }

        function RefreshDetailedReport() {
            refreshIntervalDetailedReport = setInterval(function () {
                if ($('#divNewDashboard').css('display') == 'block' && $('#divSDDetails').css('display') == 'block') {
                    clearInterval(refreshIntervalDetailedReport);
                    GetDashboardDetailedReport();
                }
            }, 60000)
        }

        function Init() {
            GetOrganizationSummarySales();
        }

        Init()
    }
)