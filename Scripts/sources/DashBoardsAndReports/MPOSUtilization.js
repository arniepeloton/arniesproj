require(['common', 'promise!/Scripts/jquery.filedownload.min.js',],
    function () {
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

        var chartWeeklyProgress = null;
        var chartFranchisedWeeklyProgress = null;
        var chartDeviceUtilCO = null;
        var chartDeviceUtilFO = null;
        var chartFullyUtilizedPerRegionCO = null;
        var chartFullyUtilizedPerRegionFO = null;
        var chartOrganizationWideUtilization = null;
        var chartURCO = null;
        var chartURFO = null;
        var dateFrom = '', dateTo = '';
        var refreshInterval;
        var switchDMReportInterval;
        var customDateFrom, customDateTo;
        var FHDateFrom, FHDateTo;

        $(document).ready(function () {
            Chart.defaults.font.family = 'Outfit';

            $('input[name="btnMPOSUtil"]').change(function () {
                var val = $(this).val();
                $('#btnMPOSUtilDownloadReports').show();
                $('.colDate').hide();
                if (val == 'systemwide') {
                    $('.colDate').show();
                    $('#divSystemWide').pageshow();

                    if (typeof refreshInterval == 'number') clearInterval(refreshInterval);

                    GetOrganizationWideUtilization();
                } else if (val == 'monthlyutil') {
                    $('#divMPOSUtilizationReport').pageshow();
                    if (typeof refreshInterval == 'number') clearInterval(refreshInterval);
                } else if (val == 'franchisedstores') {
                    $('#divByFranchisedStores').pageshow();
                    if (typeof refreshInterval == 'number') clearInterval(refreshInterval);
                    //$('#btnMPOSUtilDownloadReports').hide();
                }
            })

            $('#btnMPOSUtilDownloadReports').click(function () {                
                var pgvisible = $('[subpage="pageMPOSUtil"]:visible').attr('id');

                if (pgvisible == 'divSystemWide') {
                    var selSWReport = $('#selSWReport').val();

                    if (selSWReport == 'Summary') {
                        if (typeof switchDMReportInterval == 'number') clearTimeout(switchDMReportInterval);
                        $('#tbodyFORegionDMs tr').show();
                        ExportAsImage($('#divSWSummary'), function () {
                            Bs.Loading('hide');
                            NewMessageBox('All reports have been exported as images. Please save them if needed. Thank you!');
                            $('#tbodyFORegionDMs tr').hide();
                            $('.dm-' + $('[name="radFORegion"]:checked').attr('id').split('region-')[1]).show();
                            SwitchDMReport();
                        });
                    } else if (selSWReport == 'ByStore') {
                        ExportAsImage($('#divSWByStores'), function () {
                            Bs.Loading('hide');
                            NewMessageBox('All reports have been exported as images. Please save them if needed. Thank you!')
                        });
                    } else if (selSWReport == 'Breakdown') {
                        ExportAsImage($('#divSWBreakdown'), function () {
                            Bs.Loading('hide');
                            NewMessageBox('All reports have been exported as images. Please save them if needed. Thank you!')
                        });
                    }                    
                } else if (pgvisible == 'divMPOSUtilizationReport') {
                    Bs.Loading('show');
                    AjaxSend({
                        url: '/CincoCorporation/DownloadMonthlyUtil',
                        data: {
                            Period: $('#selMUPeriod').val(),
                            DisplayBy: $('#selMUDisplayReportBy').val(),
                            Year: $('#selMUYear').val(),
                            DateFrom: CStr2(customDateFrom),
                            DateTo: CStr2(customDateTo),
                        },
                        success: function (e) {
                            Bs.Loading('hide');
                            if (e.Result != 'error') {
                                window.open(MergeUrlParameters({
                                    OrgID: sessionStorage.OrgID,
                                    FileName: e.Result
                                }, '/CincoCorporation/DownloadFile'), "_blank");
                            } else {
                                NewMessageBox('There\'s an error while downloading your file.')
                            }
                        }
                    })
                } else if (pgvisible == 'divByFranchisedStores') {
                    var region = $('#selMPOSFORegions').val();

                    if (region == '' || dateFrom == '' || dateTo == '') {
                        return;
                    }

                    Bs.Loading('show');
                    AjaxSend({
                        url: '/CincoCorporation/DownloadFranchiseReport',
                        data: {
                            Region: region,
                            DateFrom: dateFrom,
                            DateTo: dateTo
                        }, success: function (e) {
                            Bs.Loading('hide');
                            if (e.Result != 'error') {
                                window.open(MergeUrlParameters({
                                    OrgID: sessionStorage.OrgID,
                                    FileName: e.Result
                                }, '/CincoCorporation/DownloadFile'), "_blank");
                            } else {
                                NewMessageBox('There\'s an error while downloading your file.')
                            }
                        }
                    })
                }
            })

            $('#btnDownloadFewestTrans').click(function () {
                $.fileDownload(MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    Date: $('#txtMPOSDate').val(),
                    Ownership: $('#selFewestStoreOwnership').val(),
                    IsDownloadData: true,
                }, '/CincoCorporation/GetStoreWithTheFewestNoOfTransactions'), {
                    prepareCallback: function () {
                        Bs.Loading('show', 'Preparing your report. Please wait ...');
                    },
                    successCallback: function () {
                        Bs.Loading('hide');
                    },
                    failCallback: function (responseHtml, url, error) {
                        var res = JSON.parse(responseHtml).d.error;
                        NewMessageBox(res, 'Error');
                    }
                });
            })

            $('#btnDownloadStoreStatus').click(function () {
                $.fileDownload(MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    IsDownloadData: true
                }, '/CincoCorporation/GetStoreUtilStatus'), {
                    prepareCallback: function () {
                        Bs.Loading('show', 'Preparing your report. Please wait ...');
                    },
                    successCallback: function () {
                        Bs.Loading('hide');
                    },
                    failCallback: function (responseHtml, url, error) {
                        var res = JSON.parse(responseHtml).d.error;
                        NewMessageBox(res, 'Error');
                    }
                });
            })

            $('#selMPOSFORegions').change(function () {
                GetUtilPerRegion();
            })

            $('#btnDatePeriodFORegions').click(function () {
                Bs.DateRange({
                    MaxFromDate: $.format.date(new Date(), 'MM-dd-yyyy'),
                    MaxToDate: $.format.date(new Date(), 'MM-dd-yyyy'),
                    Click: function (e) {
                        dateFrom = e.From;
                        dateTo = e.To;
                        $('#btnDatePeriodFORegions').text(dateFrom + ' - ' + dateTo);
                        GetUtilPerRegion();
                    }
                }, new Date(), new Date())
            })

            $('#txtSearchMPOSFORegions').keyup(function () {
                var value = $(this).val().toLowerCase();

                $('#tblMPOSSummaryFOStores tbody tr:not(.region)').filter(function () {
                    $(this).toggle(
                        $(this).text().toLowerCase().indexOf(value) > -1
                    );
                });
            });

            $('#txtSearchMPOSDetailedFORegions').keyup(function () {
                var value = $(this).val().toLowerCase();

                $('#tblMPOSDetailedFOStores tbody tr').filter(function () {
                    $(this).toggle(
                        $(this).text().toLowerCase().indexOf(value) > -1
                    );
                });
            });

            $('#txtFewestStoreStatusSearch').keyup(function () {
                var value = $(this).val().toLowerCase();

                $('#tblMPOSFewestTrans tbody tr').filter(function () {
                    $(this).toggle(
                        $(this).text().toLowerCase().indexOf(value) > -1
                    );
                });
            });

            $('#txtStoreStatusSearch').keyup(function () {
                var value = $(this).val().toLowerCase();

                $('#tblMPOSStoreStatus tbody tr').filter(function () {
                    $(this).toggle(
                        $(this).text().toLowerCase().indexOf(value) > -1
                    );
                });
            });

            $('#txtSDFlushingSearch').keyup(function () {
                var value = $(this).val().toLowerCase();

                $('#tblMPOSFlushing tbody tr').filter(function () {
                    $(this).toggle(
                        $(this).text().toLowerCase().indexOf(value) > -1
                    );
                });
            });

            $('#selMUDisplayReportBy').change(function () {
                var val = $('#selMUPeriod').val();

                if (val == 'Monthly' && $('#selMUYear').val() != '') {
                    GetMonthlyUtil();
                } else if (val == 'Custom' && customDateFrom != '' && customDateTo != '') {
                    GetMonthlyUtil();
                } else if (val == 'Yearly') {
                    GetMonthlyUtil();
                }
            })

            $('#selSWReport').change(function () {
                var val = $(this).val();

                $('.divSummaryDate').show();
                $('.divFHDate').hide();

                if (val == 'Summary') {
                    $('#divSWSummary').pageshow();
                } else if (val == 'ByStore') {
                    $('#divSWByStores').pageshow();
                } else if (val == 'Breakdown') {
                    $('#divSWBreakdown').pageshow();
                } else if (val == 'Flushing History') {
                    $('.divSummaryDate').hide();
                    $('.divFHDate').show();
                    $('#divSWFlushingHistory').pageshow();
                }
            })

            $('#butSWFHDate').click(function () {
                Bs.DateRange({
                    MaxFromDate: $.format.date(new Date(), 'MM-dd-yyyy'),
                    MaxToDate: $.format.date(new Date(), 'MM-dd-yyyy'),
                    Click: function (e) {
                        $('#butSWFHDate').text(e.From + ' - ' + e.To);
                        FHDateFrom = e.From;
                        FHDateTo = e.To;
                        GetSystemWideFlushingHistory();
                    }
                }, new Date(), new Date())
            })

            $('#selSDFHGroup').change(function () {
                GetSystemWideFlushingHistory();
            })

            $('#selStoreOwnership, #selStoreStatus').change(function () {
                GetStoreUtilStatus();
            })

            $('#selFewestStoreOwnership').change(function () {
                GetStoreWithTheFewestNoOfTransactions();
            })

            $('#btnSummaryCOClosed').click(function () {
                $('#selStoreOwnership').val('CO');
                $('#selStoreStatus').val('Closed');
                GetStoreUtilStatus();
            })

            $('#btnSummaryFOClosed').click(function () {
                $('#selStoreOwnership').val('FO');
                $('#selStoreStatus').val('Closed');
                GetStoreUtilStatus();
            })

            $('#txtMPOSDate').val($.format.date(new Date(), 'yyyy-MM-dd'));

            $('#btnSystemWideDate').text($.format.date(new Date(), 'MMM dd, yyyy')).click(function () {
                Bs.Calendar({
                    MaxDate: $.format.date(new Date(), 'MM-dd-yyyy'),
                    Click: function (e) {
                        $('#txtMPOSDate').val($.format.date(new Date(e), 'yyyy-MM-dd'));
                        $('#btnSystemWideDate').text($.format.date(new Date(e), 'MMM dd, yyyy'))
                        if (typeof refreshInterval == 'number') clearInterval(refreshInterval);

                        GetOrganizationWideUtilization();
                    }
                })                
            })

            $('#btnMUCustomPeriod').click(function () {
                Bs.DateRange({
                    MaxFromDate: $.format.date(new Date(), 'MM-dd-yyyy'),
                    MaxToDate: $.format.date(new Date(), 'MM-dd-yyyy'),
                    Click: function (e) {
                        $('#btnMUCustomPeriod').text(e.From + ' - ' + e.To);
                        customDateFrom = e.From;
                        customDateTo = e.To;
                        GetMonthlyUtil();
                    }
                }, new Date(), new Date())                
            })

            $('#selMUPeriod').change(function () {
                var val = $(this).val();

                customDateFrom = '';
                customDateTo = '';
                $('[for="selMUYear"], [for="btnMUCustomPeriod"]').parent().hide();

                if (val == 'Monthly') {
                    $('#selMUYear').val('');
                    $('[for="selMUYear"]').parent().show();
                } else if (val == 'Custom') {
                    $('#btnMUCustomPeriod').text('Set Date');
                    $('[for="btnMUCustomPeriod"]').parent().show();
                } else {
                    GetMonthlyUtil();
                }
            })

            $('#selMUYear').change(function () {
                if ($(this).val() == '') return;

                GetMonthlyUtil();
            })

            $('body').tooltip({ selector: '[data-toggle="tooltip"]', trigger: 'hover' });
            
        })

        function GetSystemWideFlushingHistory() {
            Bs.Loading('show');

            AjaxSend({
                url: 'CincoCorporation/GetSystemWideFlushingHistory',
                data: {
                    DateFrom: FHDateFrom,
                    DateTo: FHDateTo,
                    Group: $('#selSDFHGroup').val()
                }, success: function (e) {
                    var vRows = e.Results.Rows, ctr = 0;
                    var tblSWFlushingHistory = $('#tblSWFlushingHistory');

                    if ($.fn.DataTable.isDataTable(tblSWFlushingHistory)) {
                        tblSWFlushingHistory.find('tbody').empty();
                        tblSWFlushingHistory.DataTable().destroy();
                    }                    

                    var datatable = tblSWFlushingHistory.DataTable({
                        pageLength: 25,
                        "ordering": false,
                        "data": vRows,
                        stateSave: true,
                        "columns": [
                            {
                                "data": function (data) {
                                    ctr += 1;
                                    return ctr + '.';
                                }, width: '50px'
                            },
                            { "data": "GroupName", width: '150px' },
                            { "data": "Code", width: '150px' },
                            { "data": "Name" },                            
                            { "data": "UnflushedCount", width: '150px', className: 'text-center' },
                        ],
                        dom: 'tip',
                        rowCallback: function (row, data) {
                            $(row).data('data', data)
                                .click(function () {
                                    var data = $(this).data('data');

                                    Bs.Loading('show')
                                    $('#lblFHStoreName').text(data.Name);

                                    AjaxSend({
                                        url: 'CincoCorporation/StoreUnflushedCountHistory',
                                        data: {
                                            DateFrom: FHDateFrom,
                                            DateTo: FHDateTo,
                                            FowstID: data.FowstID,
                                            OrgID: (!CBool(data.IsFranchise) ? sessionStorage.OrgID : '892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba') //PC Franchise ID
                                        },
                                        success: function (e) {
                                            Bs.Loading('hide')

                                            var tbody = $('#tblMPOSFlushingHistory tbody').empty();
                                            var unflusheddata = e.Results.Rows;

                                            for (var i in unflusheddata) {
                                                tbody.append($('<tr>')
                                                    .data('data', unflusheddata[i])
                                                    .append('<td class="text-center">' + (CInt2(i) + 1) + '.</td>')
                                                    .append('<td class="text-center">' + $.format.date(new Date(unflusheddata[i].FlushingDate), 'MM-dd-yyyy') + '</td>')
                                                    .append('<td class="text-center">' + unflusheddata[i].UnflushedCount + '</td>')
                                                    .click(function () {
                                                        Bs.Loading('show');

                                                        var uData = $(this).data('data');

                                                        AjaxSend({
                                                            url: 'CincoCorporation/FlushingBreakdown',
                                                            data: {
                                                                Date: uData.FlushingDate,
                                                                FowstID: data.FowstID,
                                                                OrgID: (!CBool(data.IsFranchise) ? sessionStorage.OrgID : '892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba') //PC Franchise ID
                                                            },
                                                            success: function (e) {
                                                                Bs.Loading('hide')

                                                                var tbody = $('#tblMPOSFlushingBreakdown tbody').empty();
                                                                breakdowndata = e.Results.Rows;

                                                                $('#lblFBDate').text($.format.date(new Date(uData.FlushingDate), 'MMM dd, yyyy'));

                                                                if (breakdowndata.length > 0) {
                                                                    for (var i in breakdowndata) {
                                                                        tbody.append($('<tr>')
                                                                            .data('data', breakdowndata[i])
                                                                            .append('<td class="text-center">' + (CInt2(i) + 1) + '.</td>')
                                                                            .append('<td class="text-center">' + $.format.date(new Date(breakdowndata[i].TransactionDate), 'MM-dd-yyyy') + '</td>')
                                                                            .append('<td class="text-center">' + breakdowndata[i].UnflushedCount + '</td>')
                                                                            .click(function () {

                                                                            })
                                                                        )
                                                                    }
                                                                } else {
                                                                    tbody.append($('<tr>').append('<td colspan="4" class="text-center">No results found</td>'))
                                                                }


                                                                $('#popupFlushingBreakdown').modal('show');
                                                            }
                                                        })
                                                    })
                                                )
                                            }

                                            $('#popupFlushingHistory').modal('show');
                                        }
                                    })
                                })
                        },
                        initComplete: function () {
                            Bs.Loading('hide');
                            tblSWFlushingHistory.css('width', '100%').removeClass('dataTable');
                        }
                    });

                    $('#txtSDFHSearch').unbind().keyup(function () {
                        datatable.search($(this).val()).draw();
                    })
                }, complete: function () {
                    Bs.Loading('hide');
                }
            })
        }

        function GetMonthlyUtil() {
            var period = $('#selMUPeriod').val();
            var year = $('#selMUYear').val();
            var thead = $('#tblMPOSMonthlyUtil thead tr').empty();
            var tbody = $('#tblMPOSMonthlyUtil tbody').empty();

            Bs.Loading('show');
            AjaxSend({
                url: '/CincoCorporation/GetMonthlyUtil',
                data: {
                    Period: period,
                    DisplayBy: $('#selMUDisplayReportBy').val(),
                    Year: year,
                    DateFrom: CStr2(customDateFrom),
                    DateTo: CStr2(customDateTo),
                },
                success: function (e) {
                    Bs.Loading('hide');
                    var regions = e.Results;
                    var co = [], fo = [], ctrRgb = 0, chartLbl = [];
                    var rgb = [
                        '37, 70, 100',       
                        '223, 89, 76',       
                        '31, 120, 106',       
                        '248, 210, 100',     
                        '166, 77, 121',     
                        '125, 85, 72',     
                        '96, 125, 139',     
                        '128, 0, 0'
                    ]

                    thead.append('<th>Regions</th>');
                    thead.append('<th>No. of Stores</th>');

                    if (period == 'Monthly') {
                        chartLbl = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

                        for (var i in chartLbl) {
                            thead.append('<th>' + chartLbl[i] + '</th>');
                        }
                    } else if (period == 'Custom') {
                        var from = new Date(customDateFrom);
                        var to = new Date(customDateTo);
                        for (var i = 0; from <= to; i++) {
                            thead.append('<th>' + $.format.date(new Date(from), 'MMM d') + '</th>');      
                            chartLbl.push($.format.date(new Date(from), 'MMM d'));
                            from = new Date(from.setDate(from.getDate() + 1));
                        }
                    } else if (period == 'Yearly') {
                        for (var i = 2025; i <= new Date().getFullYear(); i++) {
                            thead.append('<th>' + i + '</th>');      
                            chartLbl.push(i);
                        }
                    }

                    for (var i in regions) {
                        var tr = $('<tr>')
                        var regData = regions[i];
                        var dtSet = {
                            label: regData.GroupName,
                            fill: false,
                            borderColor: 'rgb(' + rgb[ctrRgb] + ', 1)',
                            backgroundColor: 'rgb(' + rgb[ctrRgb] + ', 1)',
                            tension: 0.1,
                            data: []
                        }

                        tr.append('<td>' + regData.GroupName + '</td>');
                        tr.append('<td class="text-center">' + regData.NoOfStores + '</td>');

                        if (period == 'Monthly') {
                            for (var d = 1; d <= 12; d++) {
                                dtSet.data.push(CDec2(AccountingFormat(regData[d])))
                                tr.append('<td class="text-center ' + PercentageColor(regData[d]) + '">' + AccountingFormat(regData[d]) + '</td>');
                            }
                        } else if (period == 'Custom') {
                            var from = new Date(customDateFrom);
                            var to = new Date(customDateTo);
                            for (var i = 0; from <= to; i++) {
                                var avg = CDec2(AccountingFormat(regData[$.format.date(new Date(from), 'MM-dd-yyyy')]));
                                dtSet.data.push(avg)
                                tr.append('<td class="text-center ' + PercentageColor(avg) + '">' + AccountingFormat(avg) + '</td>');                                
                                from = new Date(from.setDate(from.getDate() + 1));
                            }
                        } else if (period == 'Yearly') {
                            for (var i = 2025; i <= new Date().getFullYear(); i++) {
                                var avg = CDec2(AccountingFormat(regData[i]));
                                dtSet.data.push(avg)
                                tr.append('<td class="text-center ' + PercentageColor(avg) + '">' + AccountingFormat(avg) + '</td>');
                            }
                        }
                        tbody.append(tr);

                        if (regData.GroupName.indexOf('CO') == 0) {
                            co.push(dtSet);
                        } else {
                            fo.push(dtSet);
                        }

                        ctrRgb += 1;
                        if (ctrRgb == 7) ctrRgb = 0;
                    }

                    var arrChart = [{
                        Title: 'Company-owned',
                        data: co,
                        container: 'chartMUCO'
                    }, {
                        Title: 'Franchise',
                        data: fo,
                        container: 'chartMUFO'
                        }]

                    for (var i in arrChart) {
                        var chrt = document.getElementById(arrChart[i].container);

                        if (arrChart[i].Title == 'Company-owned') {
                            if (chartURCO != null) {
                                chartURCO.destroy();
                            }
                        } else {
                            if (chartURFO != null) {
                                chartURFO.destroy();
                            }
                        }
                        

                        var initChrt = new Chart(chrt, {
                            type: 'line',
                            data: {
                                labels: chartLbl,
                                datasets: arrChart[i].data
                            },
                            options: {
                                plugins: {
                                    legend: {
                                        display: $('#selMUDisplayReportBy').val() == 'By Group'
                                    },
                                    title: {
                                        display: true,
                                        text: arrChart[i].Title,
                                        font: {
                                            size: 18,
                                            weight: 'bold'
                                        }
                                    }
                                },
                                scales: {
                                    x: {
                                        ticks: {
                                            font: {
                                                size: 14
                                            }
                                        }
                                    },
                                    y: {
                                        min: 0,
                                        max: 100
                                    }
                                },
                                responsive: true,
                                maintainAspectRatio: false,
                                //indexAxis: 'y',
                            }
                        });

                        if (arrChart[i].Title == 'Company-owned') {
                            chartURCO = initChrt;
                        } else {
                            chartURFO = initChrt;
                        }
                    }
                }
            })
        }

        function GetUtilPerRegion() {
            var region = $('#selMPOSFORegions').val();

            if (region == '' || dateFrom == '' || dateTo == '') {
                return;
            }
            Bs.Loading('show');
            AjaxSend({
                url: 'CincoCorporation/GetUtilSummaryPerRegion',
                data: {
                    Region: region,
                    DateFrom: dateFrom,
                    DateTo: dateTo
                },
                success: function (e) {
                    Bs.Loading('hide');
                    var vData = e.Results;
                    var percentage;
                    var tbMPOSFOStores = $('#tblMPOSSummaryFOStores tbody').empty();

                    for (var i in vData) {
                        var data = vData[i];
                        percentage = (data.FullyUtilized / data.NoOfStores * 100);

                        tbMPOSFOStores.append($('<tr class="region">')
                            .append('<td class="align-content-center text-center" ' + (CInt2(i) % 2 == 0 ? '' : 'style="background:none;"') + ' rowspan="' + (CInt2(data.AM.length) + 2) + '">' + data.GroupName + '</td>')
                        )

                        tbMPOSFOStores.append($('<tr title="District Manager">')
                            .append($('<td class="font-weight-bold text-center text-primary"><a href="#">' + data.ManagerName + '</a></td>')
                                .data('data', data)
                                .click(function () {
                                    var mData = $(this).data('data');

                                    GetDetailedDirectManagerReport({
                                        DMName: mData.ManagerName,
                                        DMID: mData.ManagerID,
                                        Region: region,
                                        DateFrom: dateFrom,
                                        DateTo: dateTo
                                    })
                                })
                            )
                            .append('<td class="font-weight-bold text-center text-primary">' + data.FullyUtilized + '</td>')
                            .append('<td class="font-weight-bold text-center text-primary">' + data.Utilized + '</td>')
                            .append('<td class="font-weight-bold text-center text-primary">' + data.NotUtilized + '</td>')
                            .append('<td class="font-weight-bold text-center text-primary">' + data.NoOfStores + '</td>')
                            .append('<td class="font-weight-bold text-center ' + PercentageColor(percentage) + '">' + AccountingFormat(percentage) + '</td>')
                        )

                        for (var a in data.AM) {
                            var amData = data.AM[a];
                            percentage = (amData.FullyUtilized / amData.NoOfStores * 100);

                            tbMPOSFOStores.append($('<tr title="Area Manager">')
                                .append('<td>' + amData.ManagerName + '</td>')
                                .append('<td class="text-center">' + amData.FullyUtilized + '</td>')
                                .append('<td class="text-center">' + amData.Utilized + '</td>')
                                .append('<td class="text-center">' + amData.NotUtilized + '</td>')
                                .append('<td class="text-center">' + amData.NoOfStores + '</td>')
                                .append('<td class="text-center ' + PercentageColor(percentage) + '">' + AccountingFormat(percentage) + '</td>')
                            )
                        }
                    }
                }
            })
        }

        function GetDetailedDirectManagerReport(data) {
            Bs.Loading('show');
            var mdata = JSON.parse(JSON.stringify(data));
            AjaxSend({
                url: '/CincoCorporation/GetDetailedDirectManagerReport',
                data: data,
                success: function (e) {
                    Bs.Loading('hide');
                    $('#popupMPOSDetailedFOStores').modal('show');
                    $('#popupMPOSDetailedFOStores .lblDMName').text(mdata.DMName);

                    var vData = e.Result;
                    var vDateFrom = new Date(dateFrom);
                    var vDateTo = new Date(dateTo);
                    var th = $('#tblMPOSDetailedFOStores thead tr').empty();
                    var tb = $('#tblMPOSDetailedFOStores tbody').empty();

                    th.append('<th class="border-0" style="position: sticky; top: 0; background: white; z-index: 2;">#</th>');
                    th.append('<th class="border-0" style="min-width:200px; position: sticky; top: 0; background: white; z-index: 2;">Area Manager Name</th>');
                    th.append('<th class="border-0" style="min-width:200px; position: sticky; top: 0; background: white; z-index: 2;">Store Name</th>');
                    th.append('<th class="border-0" style="min-width:150px; position: sticky; top: 0; background: white; z-index: 2;">Store Code</th>');
                    th.append('<th class="border-0" style="min-width:150px; position: sticky; top: 0; background: white; z-index: 2;">Region</th>');

                    while (vDateFrom <= vDateTo) {
                        th.append('<th class="border-0" style="min-width:90px; position: sticky; top: 0; background: white; z-index: 2;">' + $.format.date(new Date(vDateFrom), 'MMM dd') + '</th>');
                        vDateFrom = new Date(vDateFrom.setDate(vDateFrom.getDate() + 1))
                    }

                    if (typeof vData == 'object') {
                        vData = vData.Rows;

                        for (var i in vData) {
                            var data = vData[i];
                            var row = $('<tr>')
                                .data('data', data)
                                .append('<td>' + (CInt2(i) + 1) + '.</td>')
                                .append('<td>' + CStr2(data['Manager Name']) + '</td>')
                                .append($('<td><a href="#">' + CStr2(data['Store Name']) + '</a></td>')
                                    .data('data', data)
                                    .click(function () {
                                        var data = $(this).data('data');
                                        $('#btnSaveStoreStatusTagging').unbind('click');
                                        Bs.Loading('show');
                                        AjaxSend({
                                            url: '/CincoCorporation/GetStoreStatus',
                                            data: {
                                                FOWSTID: data.FOWSTID
                                            }, success: function (e) {
                                                Bs.Loading('hide');
                                                $('#popupStoreStatusTagging .modal-title').text(CStr2(data['Store Code']) + ' ' + data['Store Name'])
                                                $('#selOrgFOWSTStatus').val(e.Result.Rows[0].Status);
                                                $('#popupStoreStatusTagging').modal('show');

                                                $('#btnSaveStoreStatusTagging').click(function () {
                                                    Bs.Loading('show');
                                                    AjaxSend({
                                                        url: '/CincoCorporation/SaveStoreStatus',
                                                        data: {
                                                            FOWSTID: data.FOWSTID,
                                                            Status: $('#selOrgFOWSTStatus').val()
                                                        }, success: function (e) {
                                                            Bs.Loading('hide');
                                                            $('#popupStoreStatusTagging').modal('hide');

                                                            if (e.Result == 'saved') {
                                                                WebToast('Successfully tagged');
                                                            }

                                                            if (CStr2(e.Error) != '') {
                                                                NewMessageBox('Error', e.Error);
                                                            }
                                                        }
                                                    })
                                                })
                                            }
                                        })
                                    })
                                )
                                .append('<td>' + CStr2(data['Store Code']) + '</td>')
                                .append('<td>' + CStr2(data['Region']) + '</td>')

                            vDateFrom = new Date(dateFrom);

                            while (vDateFrom <= vDateTo) {
                                var status = CStr2(data[$.format.date(new Date(vDateFrom), 'yyyy-MM-dd')]);

                                if (status == 'Utilized') {
                                    row.append('<td class="text-center" title="Utilized"><span class="bi-check-circle-fill text-success"></span></td>');
                                } else if (status == 'Not Utilized') {
                                    row.append('<td class="text-center" title="Not Utilized"><span class="bi-exclamation-circle-fill text-warning"></span></td>');
                                } else {
                                    row.append('<td class="text-center" title="Closed"><span class="bi-x-circle-fill text-secondary"></span></td>');
                                }

                                vDateFrom = new Date(vDateFrom.setDate(vDateFrom.getDate() + 1))
                            }

                            tb.append(row);
                        }

                        setTimeout(function () {
                            th = $('#tblMPOSDetailedFOStores thead th')
                            tb = $('#tblMPOSDetailedFOStores tbody tr')
                            var offset = 0;

                            for (var i = 0; i < 4; i++) {
                                $(th[i]).css({
                                    position: 'sticky',
                                    left: offset + 'px',
                                    zIndex: 3
                                })

                                tb.each(function () {
                                    var td = $(this).find('td:eq(' + i + ')');
                                    td.css({
                                        position: 'sticky',
                                        left: offset + 'px',
                                        zIndex: 2
                                    })
                                })

                                offset += $(th[i]).outerWidth()
                            }
                        }, 1000)                        
                    }                    
                }
            })            
        }

        function GetFORegions() {
            AjaxSend({
                url: 'CincoCorporation/GetFORegions',
                success: function (e) {
                    var vRows = e.Result.Rows;
                    var selMPOSFORegions = $('#selMPOSFORegions').empty();

                    selMPOSFORegions.append(new Option('Please select', ''))
                    selMPOSFORegions.append(new Option('All', 'All'))

                    for (var i in vRows) {
                        selMPOSFORegions.append(new Option(vRows[i].GroupName, vRows[i].GroupName))
                    }                    
                }
            })
        }

        function ExportAsImage(div, callback) {
            Bs.Loading('show');
            setTimeout(function () {
                domtoimage.toPng(div[0])
                    .then(function (dataUrl) {
                        var link = $("<a>")
                            .attr("href", dataUrl)
                            .attr("download", 'MPOS Utilization - ' + $('#selSWReport').val() + ' as of ' + $.format.date(new Date(), 'MM-dd-yyyy') + '.png');

                        $("body").append(link);
                        link[0].click();
                        link.remove();

                        callback();
                    })
                    .catch(function (error) {
                        console.error("dom-to-image error:", error);
                    });
            }, 2000)            
        }

        function Init() {

            $('#selMUYear').append(new Option('Please select', ''));
            for (var i = 2025; i <= new Date().getFullYear(); i++) {
                $('#selMUYear').append(new Option(i, i));
            }

            GetOrganizationWideUtilization();
            GetFORegions();

        }

        function WeeklyProgress(vRows) {
            var ads = document.getElementById('chartWeeklyProgress');
            if (chartWeeklyProgress != null) {
                chartWeeklyProgress.destroy();
            }

            for (var i in vRows[0].data) {
                vRows[0].backgroundColor.push('rgba(' + rgb[i] + ', 0.5)') ;
            }

            chartWeeklyProgress = new Chart(ads, {
                type: 'bar',
                data: {
                    labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
                    datasets: vRows,
                },
                options: {
                    plugins: {
                        title: {
                            display: true,
                            text: 'Weekly Performance'
                        },
                        legend: {
                            display: false
                        }
                    },
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

        function FranchisedWeeklyProgress(vRows) {
            var ads = document.getElementById('chartFranchisedWeeklyProgress');
            if (chartFranchisedWeeklyProgress != null) {
                chartFranchisedWeeklyProgress.destroy();
            }

            for (var i in vRows[0].data) {
                vRows[0].backgroundColor.push('rgba(' + rgb[i] + ', 0.5)');
            }

            chartFranchisedWeeklyProgress = new Chart(ads, {
                type: 'bar',
                data: {
                    labels: ['North Luzon', 'South Luzon', 'Visayas', 'Mindanao', 'GMAS', 'GMAN', 'CEBU'],
                    datasets: vRows,
                },
                options: {
                    plugins: {
                        title: {
                            display: true,
                            text: 'Franchised Weekly Performance'
                        },
                        legend: {
                            display: false
                        }
                    },
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
                    indexAxis: 'y',
                }
            });
        }

        window.GetOrganizationWideUtilization = GetOrganizationWideUtilization
        function GetOrganizationWideUtilization() {
            AjaxSend({
                url: 'CincoCorporation/GetOrganizationWideUtilization',
                data: {
                    Date: $('#txtMPOSDate').val()
                },
                success: function (vData) {

                    var nValues = [];
                    var nLabels = [];
                    var vRows = vData.Result.Rows;
                    var nTotalDeviceFullyUtilized = 0;
                    var nTotalDeviceNotUtilized = 0;
                    var nTotalDeviceClosed = 0;
                    var nTotalDevice = 0;
                    var nBackgroundcolor = [];                    

                    for (var i = 0; i <= vRows.length - 1; i++) {

                        nTotalDevice += CInt2(vRows[i].Count);
                        nValues.push(vRows[i].Count);
                        nLabels.push(vRows[i].Status);

                        if (vRows[i].Status == 'Utilized') {
                            nTotalDeviceFullyUtilized += CInt2(vRows[i].Count);
                            nBackgroundcolor.push('rgba(243, 169, 53)');
                        } else if (vRows[i].Status == 'Not Utilized') {
                            nTotalDeviceNotUtilized += CInt2(vRows[i].Count);
                            nBackgroundcolor.push('rgba(37, 134, 164)');
                        } else {
                            nTotalDeviceClosed += CInt2(vRows[i].Count);
                            nBackgroundcolor.push('rgba(110, 190, 159)');
                        }

                    }

                    vRows = vData.ClosedStores.Rows
                    $('#lblCloseCOStoreCount, #lblCloseFOStoreCount').text(0);
                    for (var i in vRows) {
                        if (vRows[i].OwnershipType == 'Franchise') {
                            $('#lblCloseFOStoreCount').text(vRows[i].Count);
                        } else {
                            $('#lblCloseCOStoreCount').text(vRows[i].Count);
                        }                        
                    }

                    var percentage = nTotalDeviceFullyUtilized / (nTotalDevice - nTotalDeviceClosed) * 100;
                    //$('#lblSystemWideStoreCount').text(nTotalDeviceFullyUtilized + '/' + (nTotalDevice - nTotalDeviceClosed) + ' (' + AccountingFormat(nTotalDeviceFullyUtilized / (nTotalDevice - nTotalDeviceClosed) * 100) + '%)');
                    $('#lblSystemWideStoreCount').text(nTotalDeviceFullyUtilized).parent().unbind().click(function () {
                        $('#selStoreOwnership').val('All');
                        $('#selStoreStatus').val('Utilized');
                        GetStoreUtilStatus()
                    });
                    $('#lblSystemWideStoreCountNU').text(nTotalDeviceNotUtilized).parent().unbind().click(function () {
                        $('#selStoreOwnership').val('All');
                        $('#selStoreStatus').val('Not Utilized');
                        GetStoreUtilStatus()
                    });
                    $('#lblSystemWideStoreCountPercentage').removeClass('text-success text-danger').addClass((percentage < 45 ? 'text-danger' : 'text-success')).text('(' + AccountingFormat(percentage) + '%)');
                    $('#lblSystemWideStoreCountOver').text((nTotalDevice - nTotalDeviceClosed)).parent().unbind().click(function () {
                        $('#selStoreOwnership').val('All');
                        $('#selStoreStatus').val('All');
                        GetStoreUtilStatus()
                    });                    

                    var vData = [{
                        backgroundColor: nBackgroundcolor,
                        data: nValues,
                        labels: nLabels,
                        type: 'doughnut'
                    }];

                    DisplayOrganizationWideUtilizationReport(vData);

                }, error: function () {
                    //GetOrganizationWideUtilization();
                }
            });
        }

        function DisplayOrganizationWideUtilizationReport(vData) {
            var ads = document.getElementById('chartDeviceUtilCOAndFO');
            if (chartOrganizationWideUtilization != null) {
                chartOrganizationWideUtilization.destroy();
            }

            //for (var i in vData[0].data) {
            //    vData[0].backgroundColor.push('rgba(' + rgb[i] + ', 0.5)');
            //}

            chartOrganizationWideUtilization = new Chart(ads, {
                type: 'doughnut',
                data: {
                    labels: vData[0].labels,
                    datasets: vData,
                },
                options: {
                    plugins: {
                        title: {
                            display: true,
                            text: 'System-Wide'
                        }
                    },
                    responsive: true,
                    maintainAspectRatio: false,
                    indexAxis: 'y',
                }
            });

            GetUtilizationByOwnershipType();
        }

        function GetUtilizationByOwnershipType() {
            AjaxSend({
                url: 'CincoCorporation/GetUtilizationByOwnershipType',
                data: {
                    Date: $('#txtMPOSDate').val()
                },
                success: function (vData) {

                    var nValuesCO = [];
                    var nLabelsCO = [];

                    var nValuesFO = [];
                    var nLabelsFO = [];                    
                    
                    var nTotalDeviceCOFullyUtilized = 0;
                    var nTotalDeviceCONotUtilized = 0;
                    var nTotalDeviceCOClosed = 0;
                    var nTotalDeviceCOUtilizedAndNotUtilized = 0;

                    var nTotalDeviceFOFullyUtilized = 0;
                    var nTotalDeviceFONotUtilized = 0;
                    var nTotalDeviceFOClosed = 0;
                    var nTotalDeviceFOUtilizedAndNotUtilized = 0;

                    var nBackgroundcolorCO = [];
                    var nBackgroundcolorFO = [];

                    var vRows = vData.Result.Rows;

                    for (var i = 0; i <= vRows.length - 1; i++) {
                        if (vRows[i].OwnershipType == 'Company-Owned') {
                            nValuesCO.push(vRows[i].Count);
                            nLabelsCO.push(vRows[i].Status);

                            if (vRows[i].Status == 'Utilized') {
                                nTotalDeviceCOFullyUtilized += CInt2(vRows[i].Count);
                                nBackgroundcolorCO.push('rgba(243, 169, 53)');
                            } else  if (vRows[i].Status == 'Not Utilized') {
                                nTotalDeviceCONotUtilized += CInt2(vRows[i].Count);
                                nBackgroundcolorCO.push('rgba(37, 134, 164)');
                            } else {
                                nTotalDeviceCOClosed += CInt2(vRows[i].Count);
                                nBackgroundcolorCO.push('rgba(110, 190, 159)');
                            }
                            

                        } else {
                            nValuesFO.push(vRows[i].Count);
                            nLabelsFO.push(vRows[i].Status);

                            if (vRows[i].Status == 'Utilized') {
                                nTotalDeviceFOFullyUtilized += CInt2(vRows[i].Count);
                                nBackgroundcolorFO.push('rgba(243, 169, 53)');
                            } else if (vRows[i].Status == 'Not Utilized') {
                                nTotalDeviceFONotUtilized += CInt2(vRows[i].Count);
                                nBackgroundcolorFO.push('rgba(37, 134, 164)');
                            } else {
                                nTotalDeviceFOClosed += CInt2(vRows[i].Count);
                                nBackgroundcolorFO.push('rgba(110, 190, 159)');
                            }
                        }                        
                       
                    }

                    nTotalDeviceCOUtilizedAndNotUtilized = nTotalDeviceCOFullyUtilized + nTotalDeviceCONotUtilized;
                    nTotalDeviceFOUtilizedAndNotUtilized = nTotalDeviceFOFullyUtilized + nTotalDeviceFONotUtilized;

                    //$('#lblCompanyOwnedStoreCount').text(nTotalDeviceCOFullyUtilized + '/' + nTotalDeviceCOUtilizedAndNotUtilized + ' (' + AccountingFormat(nTotalDeviceCOFullyUtilized / nTotalDeviceCOUtilizedAndNotUtilized * 100) + '%)');
                    //$('#lblFranchiseStoreCount').text(nTotalDeviceFOFullyUtilized + '/' + nTotalDeviceFOUtilizedAndNotUtilized + ' (' + AccountingFormat(nTotalDeviceFOFullyUtilized / nTotalDeviceFOUtilizedAndNotUtilized * 100) + '%)');
                    var percentage = nTotalDeviceCOFullyUtilized / (nTotalDeviceCOUtilizedAndNotUtilized - nTotalDeviceCOClosed) * 100;
                    $('#lblCompanyOwnedStoreCount').text(nTotalDeviceCOFullyUtilized).parent().unbind().click(function () {
                        $('#selStoreOwnership').val('CO');
                        $('#selStoreStatus').val('Utilized');
                        GetStoreUtilStatus()
                    });
                    $('#lblCONotUtilizedCount').text(nTotalDeviceCONotUtilized).parent().unbind().click(function () {
                        $('#selStoreOwnership').val('CO');
                        $('#selStoreStatus').val('Not Utilized');
                        GetStoreUtilStatus()
                    });
                    $('#lblCompanyOwnedStoreCountPercentage').removeClass('text-success text-danger').addClass((percentage < 45 ? 'text-danger' : 'text-success')).text('(' + AccountingFormat(percentage) + '%)');
                    $('#lblCompanyOwnedStoreCounttOver').text(nTotalDeviceCOUtilizedAndNotUtilized).parent().unbind().click(function () {
                        $('#selStoreOwnership').val('CO');
                        $('#selStoreStatus').val('All');
                        GetStoreUtilStatus()
                    });

                    percentage = nTotalDeviceFOFullyUtilized / (nTotalDeviceFOUtilizedAndNotUtilized - nTotalDeviceFOClosed) * 100;
                    $('#lblFranchiseStoreCount').text(nTotalDeviceFOFullyUtilized).parent().unbind().click(function () {
                        $('#selStoreOwnership').val('FO');
                        $('#selStoreStatus').val('Utilized');
                        GetStoreUtilStatus()
                    });
                    $('#lblFONotUtilizedCount').text(nTotalDeviceFONotUtilized).parent().unbind().click(function () {
                        $('#selStoreOwnership').val('FO');
                        $('#selStoreStatus').val('Not Utilized');
                        GetStoreUtilStatus()
                    });
                    $('#lblFranchiseStorePercentage').removeClass('text-success text-danger').addClass((percentage < 45 ? 'text-danger' : 'text-success')).text('(' + AccountingFormat(percentage) + '%)');
                    $('#lblFranchiseStoreCounttOver').text(nTotalDeviceFOUtilizedAndNotUtilized).parent().unbind().click(function () {
                        $('#selStoreOwnership').val('FO');
                        $('#selStoreStatus').val('All');
                        GetStoreUtilStatus()
                    });

                    var vDataCO = [{
                        backgroundColor: nBackgroundcolorCO,
                        data: nValuesCO,
                        labels: nLabelsCO,
                        type: 'doughnut'
                    }]

                    var vDataFO = [{
                        backgroundColor: nBackgroundcolorFO,
                        data: nValuesFO,
                        labels: nLabelsFO,
                        type: 'doughnut'
                    }]

                    DisplayUtilizationByOwnershipType(vDataCO, vDataFO);

                }, error: function () {
                    GetUtilizationByOwnershipType();
                }
            });
        }

        function DisplayUtilizationByOwnershipType(vDataCO, vDataFO) {

            var ads = document.getElementById('chartDeviceUtilCO');
            if (chartDeviceUtilCO != null) {
                chartDeviceUtilCO.destroy();
            }

            for (var i in vDataCO[0].data) {
                vDataCO[0].backgroundColor.push('rgba(' + rgb[i] + ', 0.5)');
            }

            chartDeviceUtilCO = new Chart(ads, {
                type: 'doughnut',
                data: {
                    labels: vDataCO[0].labels,
                    datasets: vDataCO,
                },
                options: {
                    plugins: {
                        title: {
                            display: true,
                            text: 'Company-Owned'
                        }
                    },
                    responsive: true,
                    maintainAspectRatio: false,
                    indexAxis: 'y',
                }
            });

            var ads = document.getElementById('chartDeviceUtilFO');
            if (chartDeviceUtilFO != null) {
                chartDeviceUtilFO.destroy();
            }

            for (var i in vDataFO[0].data) {
                vDataFO[0].backgroundColor.push('rgba(' + rgb[i] + ', 0.5)');
            }

            chartDeviceUtilFO = new Chart(ads, {
                type: 'doughnut',
                data: {
                    labels: vDataFO[0].labels,
                    datasets: vDataFO,
                },
                options: {
                    plugins: {
                        title: {
                            display: true,
                            text: 'Franchise'
                        }
                    },
                    responsive: true,
                    maintainAspectRatio: false,
                    indexAxis: 'y',
                }
            });

            GetUtilizationByOwnershipTypeAndGroup();

        }

        function GetUtilizationByOwnershipTypeAndGroup() {

            AjaxSend({
                url: 'CincoCorporation/GetUtilizationByOwnershipTypeAndGroup',
                data: {
                    Date: $('#txtMPOSDate').val()
                },
                success: function (vData) {

                    var nValuesCO = [];
                    var nLabelsCO = [];

                    var nValuesFO = [];
                    var nLabelsFO = [];

                    var nTotalDeviceCOFullyUtilized = 0;
                    var nTotalDeviceCONotUtilized = 0;

                    var nTotalDeviceFOFullyUtilized = 0;
                    var nTotalDeviceFONotUtilized = 0;

                    var nBackgroundcolorFO = [];
                    var nBackgroundcolorCO = [];

                    var vRows = vData.Result.Rows;

                    for (var i = 0; i <= vRows.length - 1; i++) {
                        if (vRows[i].OwnershipType == 'Company-Owned') {                            

                            if (vRows[i].Status == 'Utilized') {
                                nValuesCO.push(vRows[i].Count);                                
                                nLabelsCO.push(vRows[i].GroupName);
                                nBackgroundcolorFO.push('rgba(37, 134, 164, 1');
                            } 

                        } else {                           

                            if (vRows[i].Status == 'Utilized') {
                                nValuesFO.push(vRows[i].Count);
                                nLabelsFO.push(vRows[i].GroupName);
                                nBackgroundcolorCO.push('rgba(37, 134, 164, 1');
                            } 
                        }

                    }

                    //$('#lblCompanyOwnedStoreCount').text(nTotalDeviceCOFullyUtilized + '/' + nTotalDeviceCONotUtilized + ' (' + AccountingFormat(nTotalDeviceCOFullyUtilized / nTotalDeviceCONotUtilized * 100) + '%)');
                    //$('#lblFranchiseStoreCount').text(nTotalDeviceFOFullyUtilized + '/' + nTotalDeviceFONotUtilized + ' (' + AccountingFormat(nTotalDeviceFOFullyUtilized / nTotalDeviceFONotUtilized * 100) + '%)');

                    var vDataCO = [{
                        backgroundColor: nBackgroundcolorFO,
                        data: nValuesCO,
                        labels: nLabelsCO,
                        type: 'bar',
                        barThickness: 30
                    }]

                    var vDataFO = [{
                        backgroundColor: nBackgroundcolorCO,
                        data: nValuesFO,
                        labels: nLabelsFO,
                        type: 'bar',
                        barThickness: 30
                    }]

                    DisplayUtilizationByOwnershipTypeAndGroup(vDataCO, vDataFO);

                    DisplayUtilizationByGroup(vRows);
                }, error: function () {
                    GetUtilizationByOwnershipTypeAndGroup();
                }
            });
        }

        function DisplayUtilizationByOwnershipTypeAndGroup(vDataCO, vDataFO) {

            var ads = document.getElementById('chartFullyUtilizedPerRegionCO');
            if (chartFullyUtilizedPerRegionCO != null) {
                chartFullyUtilizedPerRegionCO.destroy();
            }

            for (var i in vDataCO[0].data) {
                vDataCO[0].backgroundColor.push('rgba(37, 134, 164, 1)');
            }

            chartFullyUtilizedPerRegionCO = new Chart(ads, {
                type: 'bar',
                data: {
                    labels: vDataCO[0].labels,
                    datasets: vDataCO,
                },
                options: {
                    plugins: {
                        legend: {
                            display: false
                        },
                        title: {
                            display: true,
                            text: 'Company-Owned',
                            font: {
                                size: 18,
                                weight: 'bold'
                            }
                        }
                    },
                    scales: {
                        x: {
                            ticks: {
                                font: {
                                    size: 18,
                                    weight: 'bold'
                                }
                            }
                        },
                        y: {
                            min: 0,
                            max: 100
                        }
                    },
                    responsive: true,
                    maintainAspectRatio: false,
                    //indexAxis: 'y',
                }
            });

            var ads = document.getElementById('chartFullyUtilizedPerRegionFO');
            if (chartFullyUtilizedPerRegionFO != null) {
                chartFullyUtilizedPerRegionFO.destroy();
            }

            for (var i in vDataFO[0].data) {
                vDataFO[0].backgroundColor.push('rgba(37, 134, 164, 1)');
            }

            chartFullyUtilizedPerRegionFO = new Chart(ads, {
                type: 'bar',
                data: {
                    labels: vDataFO[0].labels,
                    datasets: vDataFO,
                },
                options: {
                    plugins: {
                        legend: {
                            display: false
                        },
                        title: {
                            display: true,
                            text: 'Franchise',
                            font: {
                                size: 18,
                                weight: 'bold'
                            }
                        }
                    },
                    scales: {
                        x: {
                            ticks: {
                                font: {
                                    size: 18,
                                    weight: 'bold'
                                }
                            }
                        },
                        y: {
                            min: 0,
                            max: 100
                        }
                    },
                    responsive: true,
                    maintainAspectRatio: false,
                    //indexAxis: 'y',
                }
            });

            //GetUtilizationByOwnershipTypeAndGroup();

        }

        function DisplayUtilizationByGroup(vRows) {
            var COGMANUtilized = 0;
            var COGMASUtilized = 0;
            var CONorthLuzonUtilized = 0;
            var COSouthLuzonUtilized = 0;
            var COVisayasUtilized = 0;
            var COMindanaoUtilized = 0;

            var CebuUtilized = 0;
            var FOGMANUtilized = 0;
            var FOGMASUtilized = 0;
            var FONorthLuzonUtilized = 0;
            var FOSouthLuzonUtilized = 0;
            var FOVisayasUtilized = 0;
            var FOMindanaoUtilized = 0;

            var COGMANNotUtilized = 0;
            var COGMASNotUtilized = 0;
            var CONorthLuzonNotUtilized = 0;
            var COSouthLuzonNotUtilized = 0;
            var COVisayasNotUtilized = 0;
            var COMindanaoNotUtilized = 0;

            var CebuNotUtilized = 0;
            var FOGMANNotUtilized = 0;
            var FOGMASNotUtilized = 0;
            var FONorthLuzonNotUtilized = 0;
            var FOSouthLuzonNotUtilized = 0;
            var FOVisayasNotUtilized = 0;
            var FOMindanaoNotUtilized = 0;

            for (var i = 0; i <= vRows.length - 1; i++) {

                if (vRows[i].OwnershipType == 'Company-Owned') {

                    if (vRows[i].Status == 'Utilized') {
                        switch (vRows[i].GroupName) {
                            case "CO - GMAN": COGMANUtilized += CInt2(vRows[i].Count); break;
                            case 'CO - GMAS': COGMASUtilized += CInt2(vRows[i].Count); break;
                            case 'CO - North Luzon': CONorthLuzonUtilized += CInt2(vRows[i].Count); break;
                            case 'CO - South Luzon': COSouthLuzonUtilized += CInt2(vRows[i].Count); break;
                            case 'CO - Visayas': COVisayasUtilized += CInt2(vRows[i].Count); break;
                            case 'CO - Mindanao': COMindanaoUtilized += CInt2(vRows[i].Count); break;
                        }
                    } else {
                        switch (vRows[i].GroupName) {
                            case 'CO - GMAN': COGMANNotUtilized += CInt2(vRows[i].Count); break;
                            case 'CO - GMAS': COGMASNotUtilized += CInt2(vRows[i].Count); break;
                            case 'CO - North Luzon': CONorthLuzonNotUtilized += CInt2(vRows[i].Count); break;
                            case 'CO - South Luzon': COSouthLuzonNotUtilized += CInt2(vRows[i].Count); break;
                            case 'CO - Visayas': COVisayasNotUtilized += CInt2(vRows[i].Count); break;
                            case 'CO - Mindanao': COMindanaoNotUtilized += CInt2(vRows[i].Count); break;                            
                        }
                    }

                } else {

                    if (vRows[i].Status == 'Utilized') {
                        switch (vRows[i].GroupName) {
                            case 'Cebu': CebuUtilized += CInt2(vRows[i].Count); break;
                            case 'FO - GMAN': FOGMANUtilized += CInt2(vRows[i].Count); break;
                            case 'FO - GMAS': FOGMASUtilized += CInt2(vRows[i].Count); break;
                            case 'FO - North Luzon': FONorthLuzonUtilized += CInt2(vRows[i].Count); break;
                            case 'FO - South Luzon': FOSouthLuzonUtilized += CInt2(vRows[i].Count); break;
                            case 'FO - Visayas': FOVisayasUtilized += CInt2(vRows[i].Count); break;
                            case 'FO - Mindanao': FOMindanaoUtilized += CInt2(vRows[i].Count); break;
                        }
                    } else {
                        switch (vRows[i].GroupName) {
                            case 'Cebu': CebuNotUtilized += CInt2(vRows[i].Count); break;
                            case 'FO - GMAN': FOGMANNotUtilized += CInt2(vRows[i].Count); break;
                            case 'FO - GMAS': FOGMASNotUtilized += CInt2(vRows[i].Count); break;
                            case 'FO - North Luzon': FONorthLuzonNotUtilized += CInt2(vRows[i].Count); break;
                            case 'FO - South Luzon': FOSouthLuzonNotUtilized += CInt2(vRows[i].Count); break;
                            case 'FO - Visayas': FOVisayasNotUtilized += CInt2(vRows[i].Count); break;
                            case 'FO - Mindanao': FOMindanaoNotUtilized += CInt2(vRows[i].Count); break;
                        }
                    }
                }
            }

            var percentage = COGMANUtilized / (COGMANUtilized + COGMANNotUtilized) * 100;
            $('#COGMANPercentage').text(AccountingFormat(percentage) + '%');
            $('#pgCOGMANPercentage').css('width', CStr2((AccountingFormat(percentage) + '%'))).css('background-color', ProgressBarColor(percentage));

            percentage = COGMASUtilized / (COGMASUtilized + COGMASNotUtilized) * 100;
            $('#COGMASPercentage').text(AccountingFormat(percentage) + '%');
            $('#pgCOGMASPercentage').css('width', CStr2((AccountingFormat(percentage) + '%'))).css('background-color', ProgressBarColor(percentage));

            percentage = CONorthLuzonUtilized / (CONorthLuzonUtilized + CONorthLuzonNotUtilized) * 100;
            $('#CONorthLuzonPercentage').text(AccountingFormat(percentage) + '%');
            $('#pgCONorthLuzonPercentage').css('width', CStr2((AccountingFormat(percentage) + '%'))).css('background-color', ProgressBarColor(percentage));

            percentage = COSouthLuzonUtilized / (COSouthLuzonUtilized + COSouthLuzonNotUtilized) * 100;
            $('#COSouthLuzonPercentage').text(AccountingFormat(percentage) + '%');
            $('#pgCOSouthLuzonPercentage').css('width', CStr2((AccountingFormat(percentage)) + '%')).css('background-color', ProgressBarColor(percentage));

            percentage = COVisayasUtilized / (COVisayasUtilized + COVisayasNotUtilized) * 100;
            $('#COVisayasPercentage').text(AccountingFormat(percentage) + '%');
            $('#pgCOVisayasPercentage').css('width', CStr2((AccountingFormat(percentage) + '%'))).css('background-color', ProgressBarColor(percentage));

            percentage = COMindanaoUtilized / (COMindanaoUtilized + COMindanaoNotUtilized) * 100;
            $('#COMindanaoPercentage').text(AccountingFormat(percentage) + '%');
            $('#pgCOMindanaoPercentage').css('width', CStr2((AccountingFormat(percentage) + '%'))).css('background-color', ProgressBarColor(percentage));

            percentage = CebuUtilized / (CebuUtilized + CebuNotUtilized) * 100;
            $('#FOCebuPercentage').text(AccountingFormat(percentage) + '%');
            $('#pgFOCebuPercentage').css('width', CStr2((AccountingFormat(percentage) + '%'))).css('background-color', ProgressBarColor(percentage));

            percentage = FOGMANUtilized / (FOGMANUtilized + FOGMANNotUtilized) * 100;
            $('#FOGMANPercentage').text(AccountingFormat(percentage) + '%');
            $('#pgFOGMANPercentage').css('width', CStr2((AccountingFormat(percentage) + '%'))).css('background-color', ProgressBarColor(percentage));

            percentage = FOGMASUtilized / (FOGMASUtilized + FOGMASNotUtilized) * 100;
            $('#FOGMASPercentage').text(AccountingFormat(percentage) + '%');
            $('#pgFOGMASPercentage').css('width', CStr2((AccountingFormat(percentage) + '%'))).css('background-color', ProgressBarColor(percentage));

            percentage = FONorthLuzonUtilized / (FONorthLuzonUtilized + FONorthLuzonNotUtilized) * 100;
            $('#FONorthLuzonPercentage').text(AccountingFormat(percentage) + '%');
            $('#pgFONorthLuzonPercentage').css('width', CStr2((AccountingFormat(percentage) + '%'))).css('background-color', ProgressBarColor(percentage));

            percentage = FOSouthLuzonUtilized / (FOSouthLuzonUtilized + FOSouthLuzonNotUtilized) * 100;
            $('#FOSouthLuzonPercentage').text(AccountingFormat(percentage) + '%');
            $('#pgFOSouthLuzonPercentage').css('width', CStr2((AccountingFormat(percentage) + '%'))).css('background-color', ProgressBarColor(percentage));

            percentage = FOVisayasUtilized / (FOVisayasUtilized + FOVisayasNotUtilized) * 100;
            $('#FOVisayasPercentage').text(AccountingFormat(percentage) + '%');
            $('#pgFOVisayasPercentage').css('width', CStr2((AccountingFormat(percentage) + '%'))).css('background-color', ProgressBarColor(percentage));

            percentage = FOMindanaoUtilized / (FOMindanaoUtilized + FOMindanaoNotUtilized) * 100;
            $('#FOMindanaoPercentage').text(AccountingFormat(percentage) + '%');
            $('#pgFOMindanaoPercentage').css('width', CStr2((AccountingFormat(percentage) + '%'))).css('background-color', ProgressBarColor(percentage));

            GetCountStoreWithTheFewestNoOfTransactions();
        }

        function GetCountStoreWithTheFewestNoOfTransactions() {            
            AjaxSend({
                url: 'CincoCorporation/GetCountStoreWithTheFewestNoOfTransactions',
                data: {
                    Count: 1,
                    Date: $('#txtMPOSDate').val(),
                }, success: function (e) {
                    var vRows = e.Result.Rows;

                    for (var i in vRows) {
                        if (vRows[i].OwnershipType == 'Franchise') {
                            $('#lblFewestFOTransCount').text(vRows[i].Count).parent().unbind().click(function () {
                                $('#selFewestStoreOwnership').val('Franchise');
                                GetStoreWithTheFewestNoOfTransactions();
                            });
                        } else {
                            $('#lblFewestCOTransCount').text(vRows[i].Count).parent().unbind().click(function () {
                                $('#selFewestStoreOwnership').val('Company-Owned');
                                GetStoreWithTheFewestNoOfTransactions();
                            });
                        }
                    }
                    FORegionsSummary();
                }, error: function (err) {
                    GetCountStoreWithTheFewestNoOfTransactions();
                }
            })
        }

        function GetStoreWithTheFewestNoOfTransactions() {
            Bs.Loading('show');
            AjaxSend({
                url: 'CincoCorporation/GetStoreWithTheFewestNoOfTransactions',
                data: {
                    Count: 1,
                    Date: $('#txtMPOSDate').val(),
                    Ownership: $('#selFewestStoreOwnership').val()
                }, success: function (e) {
                    Bs.Loading('hide');
                    var vRows = e.Result.Rows;
                    var tblMPOSFewestTrans = $('#tblMPOSFewestTrans tbody').empty();

                    if (vRows.length > 0) {
                        for (var i in vRows) {
                            var vData = vRows[i];
                            tblMPOSFewestTrans.append($('<tr>')
                                .append('<td>' + (CInt2(i) + 1) + '.</td>')
                                .append('<td>' + vData['Region'] + '</td>')
                                .append('<td>' + vData['Store Code'] + '</td>')
                                .append('<td>' + vData['Store Name'] + '</td>')
                                .append('<td class="text-center">' + vData['Status'] + '</td>')
                            )
                        }
                    } else {
                        tblMPOSFewestTrans.append($('<tr>')
                            .append('<td colspan="6" class="text-center">No results found</td>')
                        )
                    }

                    $('#popupFewestTrans').modal('show');
                }, error: function (err) {
                    Bs.Loading('hide');
                    GetStoreWithTheFewestNoOfTransactions();
                }
            })            
        }
        
        function FORegionsSummary() {
            AjaxSend({
                url: 'CincoCorporation/GetSystemWideUtilSummaryPerRegion',
                data: {
                    Region: 'All',
                    DateFrom: $('#txtMPOSDate').val(),
                    DateTo: $('#txtMPOSDate').val()
                },
                success: function (e) {
                    Bs.Loading('hide');
                    var vData = e.Results;
                    var divFORegions = $('#divFORegions');
                    var tbodyFORegionDMs = $('#tbodyFORegionDMs')

                    if (typeof switchDMReportInterval == 'number') clearTimeout(switchDMReportInterval);

                    if (vData.length > 0) {
                        for (var i in vData) {
                            var data = vData[i];
                            var idval = data.ManagerID + '-' + data.GroupName.replaceAll(" ", "");

                            if ($('#region-' + idval).length == 0) {
                                divFORegions
                                    .append($('<input data-role="none" type="radio" class="btn-check" id="region-' + idval + '" value="' + idval + '" name="radFORegion" autocomplete="off">')
                                        .data('data', data)
                                        .change(function () {
                                            tbodyFORegionDMs.find('tr').hide();
                                            $('.dm-' + $(this).data('data').ManagerID + '-' + $(this).data('data').GroupName.replaceAll(" ", "")).show();
                                            if (typeof switchDMReportInterval == 'number') clearTimeout(switchDMReportInterval);
                                            SwitchDMReport();
                                            document.getElementById('lbl-' + $('[name="radFORegion"]:checked').attr('id').split('region-')[1]).scrollIntoView({ behavior: "smooth", block: "center", inline: "center" });
                                        })
                                    )
                                    .append('<label class="btn btn-outline-secondary border-0 rounded" id="lbl-' + idval + '" for="region-' + idval + '">' + data.GroupName + '</label>')
                            } else {
                                $('.dm-' + idval).remove();
                            }

                            var percentage = (data.FullyUtilized / data.NoOfStores * 100);

                            tbodyFORegionDMs.append($('<tr title="District Manager" class="dm-' + idval + '" style="display:none;">')
                                .append($('<td class="font-weight-bold text-center text-primary"><a href="#">' + data.ManagerName + '</a></td>')
                                    .data('data', data)
                                    .click(function () {
                                        var mData = $(this).data('data');

                                        GetDetailedDirectManagerReport({
                                            DMName: mData.ManagerName,
                                            DMID: mData.ManagerID,
                                            Region: mData.GroupName,
                                            DateFrom: $('#txtMPOSDate').val(),
                                            DateTo: $('#txtMPOSDate').val()
                                        })
                                    })
                                )
                                .append('<td class="font-weight-bold text-center text-primary">' + data.FullyUtilized + '</td>')
                                .append('<td class="font-weight-bold text-center text-primary">' + data.Utilized + '</td>')
                                .append('<td class="font-weight-bold text-center text-primary">' + data.NotUtilized + '</td>')
                                .append('<td class="font-weight-bold text-center text-primary">' + data.NoOfStores + '</td>')
                                .append('<td class="font-weight-bold text-center ' + PercentageColor(percentage) + '">' + AccountingFormat(percentage) + '</td>')
                            )

                            for (var a in data.AM) {
                                var amData = data.AM[a];
                                percentage = (amData.FullyUtilized / amData.NoOfStores * 100);
                                tbodyFORegionDMs.append($('<tr title="Area Manager" class="dm-' + idval + '" style="display:none;">')
                                    .append('<td>' + amData.ManagerName + '</td>')
                                    .append('<td class="text-center">' + amData.FullyUtilized + '</td>')
                                    .append('<td class="text-center">' + amData.Utilized + '</td>')
                                    .append('<td class="text-center">' + amData.NotUtilized + '</td>')
                                    .append('<td class="text-center">' + amData.NoOfStores + '</td>')
                                    .append('<td class="text-center ' + PercentageColor(percentage) + '">' + AccountingFormat(percentage) + '</td>')
                                )
                            }
                        }

                        if ($('[name="radFORegion"]:checked').length == 0) {
                            $('[value="' + vData[0].ManagerID + '-' + vData[0].GroupName.replaceAll(" ", "") + '"]').prop('checked', 1).change();
                        } else {
                            $('.dm-' + $('[name="radFORegion"]:checked').attr('id').split('region-')[1]).show();
                        }

                        SwitchDMReport();
                    } else {
                        divFORegions.empty();
                        $('#divDMReport').empty();
                    }

                    GetFlushingSummary();
                }, error: function (err) {
                    //FORegionsSummary();
                }
            })
        }

        function PercentageColor(percentage) {
            if (percentage >= 45) {
                return 'text-success';
            } else if (percentage < 45) {
                return 'text-danger';
            }
        }

        function Refresh() {
            refreshInterval = setInterval(function () {
                if ($('#divDBPOSUtilization').css('display') == 'block') {
                    clearInterval(refreshInterval);
                    if ($('#txtMPOSDate').val() == $.format.date(new Date(), 'yyyy-MM-dd')) {
                        GetOrganizationWideUtilization();
                    } else {
                        GetFlushingSummary();
                    }
                }                
            }, 60000) // refresh every 3mins
        }

        function SwitchDMReport() {
            switchDMReportInterval = setTimeout(function () {
                clearTimeout(switchDMReportInterval);
                var nextchb = $('[name="radFORegion"]:checked').nextAll('input').first();

                if (nextchb.length > 0) nextchb.prop('checked', 1).change();
                else $('[name="radFORegion"]').first().prop('checked', 1).change()

                document.getElementById('lbl-' + $('[name="radFORegion"]:checked').attr('id').split('region-')[1]).scrollIntoView({ behavior: "smooth", block: "center", inline: "center" });
            }, 15000)
        }

        function GetFlushingSummary() {
            AjaxSend({
                url: '/CincoCorporation/GetFlushingSummary',
                data: {
                    Date: $('#txtMPOSDate').val()
                },
                success: function (e) {
                    $('#lblCOUtilizedCount').text(e.CO.Rows[0]['CO-NoOfFlushing']).parent().unbind().click(function () {
                        GetDeviceFlushingQueue(false)
                    });

                    $('#lblFOUtilizedCount').text(e.FO.Rows[0]['FO-NoOfFlushing']).parent().unbind().click(function () {
                        GetDeviceFlushingQueue(true)
                    });

                    Refresh();
                }, error: function () {
                    GetFlushingSummary();
                }
            })
        }

        function GetDeviceFlushingQueue(IsFranchise) {
            Bs.Loading('show');

            $('#btnRefreshFlushing').unbind().click(function () {
                GetDeviceFlushingQueue(IsFranchise);
            })

            AjaxSend({
                url: 'CincoCorporation/GetStoreUnflushedCount',
                data: {
                    Date: $('#txtMPOSDate').val(),
                    OrgID: (!IsFranchise ? sessionStorage.OrgID : '892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba') //PC Franchise ID
                },
                success: function (e) {
                    Bs.Loading('hide');
                    var vRows = e.Results.Rows;
                    var tblMPOSFlushing = $('#tblMPOSFlushing tbody').empty();

                    if (vRows.length > 0) {
                        for (var i in vRows) {
                            var vData = vRows[i];

                            tblMPOSFlushing.append($('<tr>')
                                .data('data', vData)
                                .append('<td>' + (CInt2(i) + 1) + '.</td>')
                                .append('<td>' + vData.Code + ' ' + vData.Name + '</td>')
                                .append('<td class="text-center">' + $.format.date(new Date(vData.FlushingDate), 'MM-dd-yyyy') + '</td>')
                                .append('<td class="text-center">' + vData.UnflushedCount + '</td>')
                                .click(function () {
                                    var data = $(this).data('data');

                                })
                            )
                        }
                    } else {
                        tblMPOSFlushing.append($('<tr>')
                            .append('<td colspan="5" class="text-center">No results found</td>')
                        )
                    }

                    $('#popupFlushing .modal-title').text((!IsFranchise ? 'CO' : 'FO') + ' - Flushing Queues')
                    $('#popupFlushing').modal('show');
                }
            })
        }

        function GetStoreUtilStatus() {
            Bs.Loading('show');
            AjaxSend({
                url: 'CincoCorporation/GetStoreUtilStatus',
                data: {
                    Ownership: $('#selStoreOwnership').val(),
                    Status: $('#selStoreStatus').val(),
                    Date: $('#txtMPOSDate').val()
                },
                success: function (e) {
                    Bs.Loading('hide');
                    var vRows = e.Results.Rows;
                    var tblMPOSStoreStatus = $('#tblMPOSStoreStatus tbody').empty();

                    if (vRows.length > 0) {
                        for (var i in vRows) {
                            var vData = vRows[i];

                            tblMPOSStoreStatus.append($('<tr>')
                                .append('<td>' + (CInt2(i) + 1) + '.</td>')
                                .append('<td>' + vData["Region"] + '</td>')
                                .append('<td>' + vData["Store Code"] + '</td>')
                                .append('<td>' + vData["Store Name"] + '</td>')
                                .append('<td class="text-center">' + vData.Status + '</td>')
                            )
                        }
                    } else {
                        tblMPOSStoreStatus.append($('<tr>')
                            .append('<td colspan="6" class="text-center">No results found</td>')
                        )
                    }

                    $('#popupStoreStatus .modal-title').text('Details');
                    $('#popupStoreStatus').modal('show');
                }
            })
        }

        function ProgressBarColor(percentage) {
            if (CDec2(percentage) >= 70) {
                return '#59a84b'; // green
            }

            if (CDec2(percentage) >= 40 && CDec2(percentage) < 70) {
                return '#f2bb46'; // orange
            }

            if (CDec2(percentage) >= 0 && CDec2(percentage) < 40) {
                return '#ca5952'; // red
            }
        }

        //Init();
    }
)