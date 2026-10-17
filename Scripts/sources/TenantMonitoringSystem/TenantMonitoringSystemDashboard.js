require([
        'common',
        'barRenderer',
        'pieRenderer',
        'categoryAxisRenderer',
        'pointLabels',
        'cursor',
        'highlighter',
        'tablesort',
        GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc='),
        (OfflineMode ? '/Scripts3/Offline/Browser/LibraryFowst.js?crc=' : '')
], function () {

    var mDivSalesReturns; // dynamically created inside divMainContentForDashboard
    var mDivOfflineTransactionHistory; // dynamically created inside divMainContentForDashboard
    var mDivOfflineProductHistory; // dynamically created inside divMainContentForDashboard
    var isInventoryAccLoaded = false;
    var m_GroupBy = 'Date'; // default

    //metro.min.js needs jquery to load first before it can load itself

    require([
            '/Scripts/jquery.widget.min.js'
    ],
        function () {
            require([
                    '/Scripts/metro.min.js'
            ],
                function () {
                    //metro-calendar.js, metro-datepicker.js needs jquery.widget.min.js andmetro.min.js  to load first before it can load itself
                    require([
                            '/Scripts/metro-calendar.js',
                            '/Scripts/metro-datepicker.js'
                    ],
                        function () {

                            sessionStorage.IsShowFirstPage = 'false';
                            sessionStorage.Period = 'Today';
                            sessionStorage.ReportType = '';

                            var m_Period = 'Today';
                            var data;
                            var ValueBtnBack = '/People/PeopleMenu?IsMobile=' + (CBool(IsMobile) ? 1 : 0).toString();
                            var TempValueBtnBack = null;
                            var CounterStoreReplenish = true;
                            var CounterCashierSalesReport = true;
                            var CounterOrderCentral = true;
                            var CounterFranchiseRequest = true;
                            var GroupID;
                            var cashiermanagerpage = "";
                            var arrFilterByOwnType = new Array();
                            var isFilterByOwnType = false;
                            var c_DateFrom, c_DateFrom;

                            var This;

                            var breadcrumbs = $('<div class="ItemsBreadCrumbs"></div>'),
                                arrowR = ' <span class="ui-icon ui-icon-arrow-r" style="display:inline-block; margin:0px 10px 0px 10px;"></span> ';

                            $(function () {

                                if (CBool(sessionStorage.PrintingOfflineData)) {
                                    $('#divMainDashboard').hide();
                                }
                                else {
                                    $('#divMainDashboard').show();
                                }


                                PageShow();

                                $('.mainContentForDashboard').css('margin-top', $('#divHeaderForDashboard').height());
                                $(window).resize(function () {
                                    $('.mainContentForDashboard').css('margin-top', $('#divHeaderForDashboard').height());
                                })

                                function RemoveOfflineItems() {
                                    if (CBool(window.OfflineMode) == false) {
                                        $('#liOfflineTransactionHistory').closest('li').remove();
                                    } // END if (window.OfflineMode) {
                                } // END RemoveOfflineItems
                                RemoveOfflineItems();

                                if (sessionStorage.IsRequestAndApproval == 1)
                                    ShowRequestAndApprovals();
                                window.Display = {
                                    GetAllFowst: function () {
                                        GetAllFOWST();
                                    }
                                }

                                var date_from_string = function (str) {
                                    var months = ["jan", "feb", "mar", "apr", "may", "jun", "jul",
                      "aug", "sep", "oct", "nov", "dec"];
                                    var pattern = "^([a-zA-Z]{3})\\s*(\\d{2}),\\s*(\\d{4})$";
                                    var re = new RegExp(pattern);
                                    var DateParts = re.exec(str).slice(1);

                                    var Year = DateParts[2];
                                    var Month = $.inArray(DateParts[0].toLowerCase(), months);
                                    var Day = DateParts[1];
                                    return new Date(Year, Month, Day);
                                }

                                var moveBlanks = function (a, b) {

                                    if (a < b) {
                                        if (a == "")
                                            return 1;
                                        else
                                            return -1;
                                    }
                                    if (a > b) {
                                        if (b == "")
                                            return -1;
                                        else
                                            return 1;
                                    }
                                    return 0;
                                };
                                var moveBlanksDesc = function (a, b) {
                                    // Blanks are by definition the smallest value, so we don't have to
                                    // worry about them here
                                    if (a < b)
                                        return 1;
                                    if (a > b)
                                        return -1;
                                    return 0;
                                };

                                var table = $("table").stupidtable({

                                    "date": function (a, b) {
                                        // Get these into date objects for comparison.

                                        aDate = new Date(a);
                                        bDate = new Date(b);

                                        return aDate - bDate;
                                    },
                                    "moveBlanks": moveBlanks,
                                    "moveBlanksDesc": moveBlanksDesc
                                });

                                table.on("beforetablesort", function (event, data) {
                                    // data.column - the index of the column sorted after a click
                                    // data.direction - the sorting direction (either asc or desc)
                                    $("#msg").text("Sorting index " + data.column)
                                });

                                table.on("aftertablesort", function (event, data) {

                                    var th = $(this).find("th");
                                    th.find(".arrow").remove();
                                    var dir = $.fn.stupidtable.dir;

                                    var arrow = data.direction === dir.ASC ? "&uarr;" : "&darr;";
                                    th.eq(data.column).append('<span class="arrow">' + arrow + '</span>');




                                });

                                $("tr").slice(1).click(function () {

                                    $(".awesome").removeClass("awesome");
                                    $(this).addClass("awesome");
                                });

                                $('#divTransactionDetails').attachDragger();

                                $(".fromCalendar").calendar({
                                    format: "mmmm dd, yyyy",
                                    multiSelect: false,
                                    otherDays: true,
                                    getDates: function (data) {
                                        $("#txtDateFrom").text(data);
                                    }
                                });

                                $(".toCalendar").calendar({
                                    format: "mmmm dd, yyyy",
                                    multiSelect: false,
                                    otherDays: true,
                                    getDates: function (data) {
                                        $("#txtDateTo").text(data);
                                    }
                                });

                                $(".ReceivingTransferfromCalendar").calendar({
                                    format: "mm.dd.yyyy",
                                    multiSelect: false,
                                    otherDays: true,
                                    getDates: function (data) {
                                        $("#txtReceivingTransferDateFrom").text(data);
                                    }
                                });

                                $(".ReceivingTransfertoCalendar").calendar({
                                    format: "mm.dd.yyyy",
                                    multiSelect: false,
                                    otherDays: true,
                                    getDates: function (data) {
                                        $("#txtReceivingTransferDateTo").text(data);
                                    }
                                });

                                var dateNow = new Date();

                                //            $('.btnSearch').click(function () {
                                //                var valtxtSearch = $('txtSearch');

                                //                if (ValueBtnBack.val() == "") {

                                //                }
                                //            })

                                $('.btnInventoryAnalytics').click(function (e) {
                                    $('.IventoryAnalyticsSubList').slideToggle('fast', function () {

                                    })
                                })

                                $('.btnInventoryCounts').click(function () {
                                    AjaxSend({
                                        ExactUrl: '/InventoryCount/Report?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                                        type: 'GET',
                                        dataType: 'text',
                                        success: function (vData) {

                                            $('#divOnPrintPOSSalesReport').empty();
                                            $('#divOnPrintPOSSalesReport').append(vData).trigger('create');

                                            App.Deferred.fulfill('FQ26E5TP76');
                                        }
                                    });

                                    $('#divSalesOrderProcessing').hide();
                                    $('#panTransactionDetails').hide();
                                    $('#divMainDashboard').hide();
                                    $('#divSalesAnalytics').hide();
                                    $('#divSalesReplenishmentProcessing').hide();
                                    $('#divFranchiseRequest').hide();
                                    $('#divOnPrintPOSSalesReport').show();

                                    $('.ItemsBreadCrumbs').text('');
                                    breadcrumbs.prepend(arrowR, 'Physical Inventory and Variance Report');
                                    $('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                    BreadCrumbsClickEvent();

                                    $('#panOption').popup('close');
                                    $('#panGoTo').popup('close');
                                })


                                $('.btnHome').click(function () {
                                    window.location = ValueBtnBack
                                });

                                $('.btnMoreReport').click(function () {
                                    //if (sessionStorage.PrintingOfflineData == "false") {
                                    $('#panOption').popup('open');
                                    //}
                                    //else {
                                    //    $('#panGoToOffline').popup('open');
                                    //}
                                });

                                $('.btnPeriod').click(function () {
                                    $('#panDates').popup('open');
                                });

                                $('#btnColumns').click(function () {
                                    $('#popColumn').popup('open');
                                })

                                $('.chbColumns').change(function () {
                                    if ($(this).is(':checked')) {
                                        $('#reportTableHeader th[headno="' + $(this).attr('headno') + '"]').show();
                                        $('#reportTable tr td[columnno="' + $(this).attr('headno') + '"]').show();
                                    }
                                    else {
                                        $('#reportTableHeader th[headno="' + $(this).attr('headno') + '"]').hide();
                                        $('#reportTable tr td[columnno="' + $(this).attr('headno') + '"]').hide();
                                    }
                                })

                                $('#btnViewType').click(function () {
                                    PopupButtonList('', 'Graph Type', [
                                        {
                                            Text: 'Sales Trend',
                                            Click: function () {
                                                $('#txtViewType').text('Sales Trend');
                                                setDate();

                                                var data = {
                                                    GraphType: 'Trend',
                                                    GroupBy: 'Date',
                                                    Fowst: 'All',
                                                    From: (m_Period == "CustomPeriod" ? c_DateFrom : $('#txtDateFrom').text()),
                                                    To: (m_Period == "CustomPeriod" ? c_DateTo : $('#txtDateTo').text()),
                                                    IsRawData: 0
                                                }
                                                GetTotalForAllFowst(data);
                                            }
                                        },
                                        {
                                            Text: 'Sales Distibution',
                                            Click: function () {
                                                PopupButtonList('', 'Sales Distribution', [
                                                    {
                                                        Text: 'Day',
                                                        Click: function () {
                                                            GetTMSSalesDistribution('Day')
                                                        }
                                                    },
                                                    {
                                                        Text: 'Hour',
                                                        Click: function () {
                                                            GetTMSSalesDistribution('Hour')
                                                        }
                                                    },
                                                    {
                                                        Text: 'Store Name',
                                                        Click: function () {
                                                            GetTMSSalesDistribution('Store Name')
                                                        }
                                                    },
                                                    {
                                                        Text: 'Store Address',
                                                        Click: function () {
                                                            GetTMSSalesDistribution('Store Address')
                                                        }
                                                    },
                                                    {
                                                        Text: 'Mall or Branch Code',
                                                        Click: function () {
                                                            GetTMSSalesDistribution('Mall Code')
                                                        }
                                                    },
                                                    {
                                                        Text: 'Tenant No.',
                                                        Click: function () {
                                                            GetTMSSalesDistribution('Tenant No')
                                                        }
                                                    },
                                                    {
                                                        Text: 'Class Code',
                                                        Click: function () {
                                                            GetTMSSalesDistribution('Class Code')
                                                        }
                                                    },
                                                    {
                                                        Text: 'Trade Code',
                                                        Click: function () {
                                                            GetTMSSalesDistribution('Trade Code')
                                                        }
                                                    }
                                                    ,
                                                    {
                                                        Text: 'Outlet No.',
                                                        Click: function () {
                                                            GetTMSSalesDistribution('Outlet No')
                                                        }
                                                    },
                                                    {
                                                        Text: 'Sales Type',
                                                        Click: function () {
                                                            GetTMSSalesDistribution('Sales Type')
                                                        }
                                                    }
                                                ], { AutoDismiss: true })
                                            }
                                        }
                                    ], { AutoDismiss: true })
                                });

                                $('#selDistributionOption').change(function () {
                                    DistributeHeaderChart($(this).val());
                                })

                                $('#btnCancel').click(function () {
                                    $('#panCustomDates').popup('close');
                                })

                                $('#li2DaysAgo').click(function () {
                                    $('#panDates').popup('close');
                                    $('.lblPeriod').text($('#li2DaysAgo').text());
                                    m_Period = '2DaysAgo';
                                    sessionStorage.Period = m_Period;
                                    //if (sessionStorage.PrintingOfflineData == "false") {
                                    setDashboardDate(m_Period);
                                    GetData();
                                    GetTotalGrossAndDiscountsByFOWST();
                                    //}
                                    $('#divTurnOverDetails').hide();
                                });

                                $('#liYesterday').click(function () {
                                    $('#panDates').popup('close');
                                    $('.lblPeriod').text($('#liYesterday').text());
                                    m_Period = 'Yesterday';
                                    sessionStorage.Period = m_Period;
                                    //if (sessionStorage.PrintingOfflineData == "false") {
                                    setDashboardDate(m_Period);
                                    GetData();
                                    GetTotalGrossAndDiscountsByFOWST();
                                    //}
                                    $('#divTurnOverDetails').hide();
                                });

                                $('#liToday').click(function () {
                                    $('#panDates').popup('close');
                                    $('.lblPeriod').text($('#liToday').text());
                                    m_Period = 'Today';
                                    sessionStorage.Period = m_Period;
                                    setDashboardDate(m_Period);
                                    GetData();

                                    //if (sessionStorage.PrintingOfflineData == "false") {
                                    GetTotalGrossAndDiscountsByFOWST();
                                    //}
                                    $('#divTurnOverDetails').hide();
                                });

                                $('#liWeekToDate').click(function () {
                                    $('#panDates').popup('close');
                                    $('.lblPeriod').text($('#liWeekToDate').text());
                                    m_Period = 'WeekToDate';
                                    sessionStorage.Period = m_Period;
                                    //if (sessionStorage.PrintingOfflineData == "false") {
                                    setDashboardDate(m_Period);
                                    GetData();
                                    GetTotalGrossAndDiscountsByFOWST();
                                    //}
                                    $('#divTurnOverDetails').hide();
                                });

                                $('#liMonthToDate').click(function () {
                                    $('#panDates').popup('close');
                                    $('.lblPeriod').text($('#liMonthToDate').text());
                                    m_Period = 'MonthToDate';
                                    sessionStorage.Period = m_Period;
                                    //if (sessionStorage.PrintingOfflineData == "false") {
                                    setDashboardDate(m_Period);
                                    GetData();
                                    GetTotalGrossAndDiscountsByFOWST();
                                    //}
                                    $('#divTurnOverDetails').hide();
                                });

                                $('#liCustomPeriod').click(function () {
                                    $('#panDates').popup('close');
                                    $('#panCustomDates').popup('open');
                                    m_Period = 'CustomPeriod';
                                    sessionStorage.Period = m_Period;
                                });

                                $('#btnDone').click(function () {
                                    c_DateFrom = $('#txtDateFrom').text();
                                    c_DateTo = $('#txtDateTo').text();
                                    $('.lblPeriod').text($('#txtDateFrom').text() + " to " + $('#txtDateTo').text());
                                    //if (sessionStorage.PrintingOfflineData == "false") {
                                    GetData();
                                    //GetTotalGrossAndDiscountsByFOWST();
                                    //}
                                    $('#divTurnOverDetails').hide();
                                });

                                $('.btnSalesAnalyticsPage').click(function () {
                                    AjaxSend({
                                        url: 'DashboardsAndReports/GetPageAccess',
                                        success: function (vData) {
                                            if (CBool(vData.Results)) {
                                                sessionStorage.IsShowFirstPage = 'true'; //TODO
                                            } else {
                                                sessionStorage.IsShowFirstPage = 'false';
                                            }

                                            GetMainDashboardPage();
                                            $('#panOption').popup('close');
                                        }
                                    });
                                });

                                $('#btnCloseMoreOption').click(function () {
                                    AjaxSend({
                                        url: 'DashboardsAndReports/GetPageAccess',
                                        success: function (vData) {
                                            if (CBool(vData.Results)) {
                                                sessionStorage.IsShowFirstPage = 'true'; //TODO
                                            } else {
                                                sessionStorage.IsShowFirstPage = 'false';
                                            }

                                            GetMainDashboardPage();
                                            $('#panOption').popup('close');
                                        }
                                    });
                                });

                                $('.btnMainDashboardPage').click(function () {
                                    AjaxSend({
                                        url: 'DashboardsAndReports/GetPageAccess',
                                        success: function (vData) {
                                            if (CBool(vData.Results)) {
                                                sessionStorage.IsShowFirstPage = 'true'; //TODO
                                            } else {
                                                sessionStorage.IsShowFirstPage = 'false';
                                            }

                                            GetMainDashboardPage();
                                            $('#panOption').popup('close');
                                        }
                                    });
                                });

                                $('.btnReplenishmentProcessing').click(function () {
                                    AjaxSend({
                                        url: 'Login3/TestCookies',
                                        success: function (vData) {
                                            PromptUserAccess('Store Replenishment Processing & Analytics', {
                                                OnClickOKSuccess: function () {
                                                    if (location.hash == '#&ui-state=dialog')
                                                        history.back();

                                                    if (CounterStoreReplenish == true) {
                                                        AjaxSend({
                                                            ExactUrl: '/DashboardsAndReports/Franchise?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=1',
                                                            type: 'GET',
                                                            dataType: 'text',
                                                            success: function (vData) {
                                                                $('#divSalesReplenishmentProcessing').empty();
                                                                $('#divSalesReplenishmentProcessing').append(vData).trigger('create');

                                                                App.Deferred.fulfill('FQ26E5TP76');
                                                            }
                                                        });
                                                        CounterStoreReplenish = false;

                                                    } else {
                                                        //Do Nothing Here
                                                    }
                                                    $('#divMainDashboard').hide();
                                                    $('#divSalesAnalytics,' +
                                                        '#divOnPrintPOSSalesReport,' +
                                                        '#divSearchResult, #divSpecialReports, #divAuditTrail, #divSettings, #divSalesOrderProcessing, #panTransactionDetails, #divFranchiseRequest').hide();

                                                    $('#divSalesReplenishmentProcessing').show();

                                                    $('#divFranchiseeOrders').show();
                                                    $('#divProductBreakdown').hide();
                                                    $('#divSalesBreakdown').hide();
                                                    $('#divFranchiseeLastReceived').hide();


                                                    $('.ItemsBreadCrumbs').text('');
                                                    breadcrumbs.prepend(arrowR, 'Store Replenishment Processing & Analytics');
                                                    $('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                                    BreadCrumbsClickEvent();
                                                    $('#panGoTo, #panOption').popup('close');
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

                                $('.btnSalesOrderProcessingAnalyticsPage').click(function () {
                                    AjaxSend({
                                        url: 'Login3/TestCookies',
                                        success: function (vData) {
                                            PromptUserAccess('Sales Order Processing and Analytics', {
                                                OnClickOKSuccess: function () {
                                                    if (location.hash == '#&ui-state=dialog')
                                                        history.back();

                                                    if (CounterOrderCentral == true) {
                                                        AjaxSend({
                                                            ExactUrl: '/DashboardsAndReports/SalesOrder?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=1',
                                                            type: 'GET',
                                                            dataType: 'text',
                                                            success: function (vData) {
                                                                $('#divSalesOrderProcessing').empty();

                                                                $('#divSalesOrderProcessing').append(vData).trigger('create');

                                                                App.Deferred.fulfill('FQ26E5TP76');
                                                            }
                                                        });

                                                        CounterOrderCentral = false;

                                                    } else {
                                                        //Do Nothing Here
                                                    }

                                                    $('#divMainDashboard, #divSalesAnalytics,' +
                                                        '#divOnPrintPOSSalesReport,' +
                                                        '#divSearchResult, #divSpecialReports, #divAuditTrail, #divSettings, #divSalesReplenishmentProcessing, #panTransactionDetails,#divFranchiseRequest').hide();

                                                    $('#divSalesOrderProcessing').show();
                                                    $('#pageOrderStatus').hide();
                                                    $('#pageOrderCentral').show();

                                                    $('.ItemsBreadCrumbs').text('');
                                                    breadcrumbs.prepend(arrowR, 'Sales Order Processing & Analytics');
                                                    $('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                                    BreadCrumbsClickEvent();

                                                    $('#panGoTo, #panOption').popup('close');
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

                                $('#butPendingSalesReturns').click(function (e) {
                                    proj.PromptTestInternet(ShowRequestAndApprovals); // END proj.PromptTestInternet(
                                }); // END butPendingSalesReturns

                                function ShowRequestAndApprovals() {
                                    PromptUserAccess('POS Receipt Reprint', { // todo: add access to Pending Sales Returns
                                        OnClickOKSuccess: function () {
                                            if (mDivSalesReturns == null) {
                                                AjaxSend({
                                                    ExactUrl: '/DashboardsAndReports/RequestAndApprovals?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=1',
                                                    type: 'GET',
                                                    dataType: 'text',
                                                    success: function (vData) {
                                                        mDivSalesReturns = $('#divMainContentForDashboard');
                                                        mDivSalesReturns.append($('<div id="divRequestAndApprovals">').append(vData)).trigger('create');
                                                        Show();
                                                    }
                                                });
                                            } else
                                                Show();

                                            function Show() {
                                                $('#divMainDashboard, #divSalesAnalytics,' +
                                                    '#divOnPrintPOSSalesReport,' +
                                                    '#divSearchResult, #divSpecialReports, #divAuditTrail, #divSettings, #divSalesReplenishmentProcessing, #panTransactionDetails,#divFranchiseRequest').hide();

                                                $('#divSalesOrderProcessing').hide();
                                                $('#pageOrderStatus').hide();
                                                $('#pageOrderCentral').show();

                                                $('.ItemsBreadCrumbs').text('Request and Approvals');
                                                $('#panGoTo, #panOption').popup('close');
                                                mDivSalesReturns.show();
                                            }
                                        }
                                    });
                                }

                                $('#liOfflineTransactionHistory').click(function (e) {
                                    ShowOfflineTransactionHistory();
                                });

                                $('#liOfflineProductHistory').click(function (e) {
                                    ShowOfflineProductHistory();
                                });

                                function ShowOfflineProductHistory() {
                                    if (mDivOfflineProductHistory == null) {
                                        AjaxSend({
                                            ExactUrl: '/DashboardsAndReports/OfflineProductHistory?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=1',
                                            type: 'GET',
                                            dataType: 'text',
                                            success: function (vData) {
                                                mDivOfflineProductHistory = $('<div>').appendTo('#divMainContentForDashboard');
                                                mDivOfflineProductHistory.append(vData).trigger('create');
                                                Show();
                                            }
                                        });
                                    } else
                                        Show();

                                    function Show() {
                                        $('#divMainDashboard, #divSalesAnalytics,' +
                                            '#divOnPrintPOSSalesReport,' +
                                            '#divSearchResult, #divSpecialReports, #divAuditTrail, #divSettings, #divSalesReplenishmentProcessing, #panTransactionDetails,#divFranchiseRequest').hide();

                                        $('#divSalesOrderProcessing').hide();
                                        $('#pageOrderStatus').hide();
                                        $('#pageOrderCentral').show();

                                        $('.ItemsBreadCrumbs').text('Offline Product History');
                                        $('#panGoTo, #panOption').popup('close');
                                        if (mDivSalesReturns != null)
                                            mDivSalesReturns.hide();
                                        if (mDivOfflineTransactionHistory != null)
                                            mDivOfflineTransactionHistory.hide();
                                        mDivOfflineProductHistory.show();
                                    }
                                } // END ShowOfflineProductHistory

                                $('.btnTrackDevice').click(function () {
                                    GetUser();

                                    function GetUser() {

                                        AjaxSend({
                                            url: 'Login3/GetUser',
                                            success: function (vData) {
                                                if (vData.Error != undefined)
                                                    MessageBox(vData.Error);
                                                else
                                                    window.open(GetPathFormat("/DashboardsAndReports/Device"), "_self");
                                            },
                                            error: function (vError) {
                                                if (IsMobile)
                                                    proj.PromptRequiresInternet({
                                                        Retry: GetUser
                                                    });
                                                else {
                                                    console.log('Error getting Login/GetUser. Retrying...');
                                                    GetUser();
                                                }
                                                ;
                                            }
                                        });
                                    }
                                });

                                $('#popPOSReportCashierSalesReport').click(function () {
                                    GetCashierSalesReportPage();
                                });

                                $('#popPOSReportManagerReport').click(function (e) {
                                    e.preventDefault();
                                    AjaxSend({
                                        url: 'Login3/TestCookies',
                                        success: function (vData) {

                                            PromptUserAccess('Manager Sales Report', {
                                                OnClickOKSuccess: function () {
                                                    var pData = {};
                                                    var isPageReady = false;
                                                    pData.OrgID = sessionStorage["OrgID"];
                                                    pData.POSSalesReportType = 1;

                                                    AjaxSend({
                                                        url: 'ReportsPOS/setPOSSalesReportType',
                                                        data: pData,
                                                        success: function (vData) {

                                                            if (cashiermanagerpage == "") {
                                                                AjaxSend({
                                                                    ExactUrl: '/ReportsPOS/SalesReport?cache=1&OrgID=' + sessionStorage.OrgID + '&IsManagerSalesReport=1&IsMobile=' + IsMobile,
                                                                    type: 'GET',
                                                                    dataType: 'text',
                                                                    success: function (vData) {
                                                                        isPageReady = true;
                                                                        cashiermanagerpage = vData;
                                                                        $('#divOnPrintPOSSalesReport').empty();
                                                                        $('#divOnPrintPOSSalesReport').append(vData).trigger('create');

                                                                        App.Deferred.fulfill('FQ26E5TP76');

                                                                        setMonthCollection("selPOSSalesReportByDateFromMonth");
                                                                        setYearCollection("selPOSSalesReportByDateFromYear");

                                                                        setMonthCollection("selPOSSalesReportByDateToMonth");
                                                                        setYearCollection("selPOSSalesReportByDateToYear");

                                                                        setDashboardDate(m_Period);
                                                                    }
                                                                });
                                                            } else {
                                                                $('#divOnPrintPOSSalesReport').empty();
                                                                $('#divOnPrintPOSSalesReport').append(cashiermanagerpage).trigger('create');
                                                                SalesReport.GetAllFowstinSalesReportPage();
                                                            }
                                                        }
                                                    });
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


                                    $('#divMainDashboard').hide();
                                    $('#divSalesAnalytics').hide();
                                    $('#divSalesReplenishmentProcessing').hide();
                                    $('#divFranchiseRequest').hide();
                                    $('#divSalesOrderProcessing').hide();
                                    $('#panTransactionDetails').hide();

                                    $('#divOnPrintPOSSalesReport').show();

                                    $('.ItemsBreadCrumbs').text('');
                                    breadcrumbs.prepend(arrowR, 'Manager\'s Sales Report');
                                    $('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                    BreadCrumbsClickEvent();

                                    //TempValueBtnBack = "BackToDashboard";

                                    $('#panOption').popup('close');
                                });

                                $('#popPOSReportZReport').click(function (e) {
                                    e.preventDefault();

                                    AjaxSend({
                                        url: 'Login3/TestCookies',
                                        success: function (vData) {

                                            PromptUserAccess('Z Report', {
                                                OnClickOKSuccess: function () {

                                                    AjaxSend({
                                                        ExactUrl: '/ReportsPOS/ZReport?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                                                        type: 'GET',
                                                        dataType: 'text',
                                                        success: function (vData) {

                                                            $('#divOnPrintPOSSalesReport').empty();
                                                            $('#divOnPrintPOSSalesReport').append(vData).trigger('create');

                                                            App.Deferred.fulfill('FQ26E5TP76');

                                                            setMonthCollection("selZReportByDateMonth");
                                                            setYearCollection("selZReportByDateYear");

                                                            setDashboardDate(m_Period);
                                                        }
                                                    });
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

                                    $('#divMainDashboard').hide();
                                    $('#divSalesAnalytics').hide();
                                    $('#divSalesReplenishmentProcessing').hide();
                                    $('#divFranchiseRequest').hide();
                                    $('#divSalesOrderProcessing').hide();
                                    $('#panTransactionDetails').hide();

                                    $('#divOnPrintPOSSalesReport').show();

                                    $('.ItemsBreadCrumbs').text('');
                                    breadcrumbs.prepend(arrowR, 'Z Report');
                                    $('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                    BreadCrumbsClickEvent();

                                    //TempValueBtnBack = "BackToDashboard";

                                    $('#panOption').popup('close');
                                });

                                $('#popPOSReportDeposit').click(function (e) {
                                    e.preventDefault();

                                    AjaxSend({
                                        url: 'Login3/TestCookies',
                                        success: function (vData) {

                                            PromptUserAccess('Manager Sales Report', {
                                                OnClickOKSuccess: function () {

                                                    AjaxSend({
                                                        ExactUrl: '/CashHandling/DepositReport?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                                                        type: 'GET',
                                                        dataType: 'text',
                                                        success: function (vData) {

                                                            $('#divOnPrintPOSSalesReport').empty();
                                                            $('#divOnPrintPOSSalesReport').append(vData).trigger('create');

                                                            App.Deferred.fulfill('FQ26E5TP76');

                                                            setMonthCollection("selZReportByDateMonth");
                                                            setYearCollection("selZReportByDateYear");

                                                            setDashboardDate(m_Period);
                                                            GetAllFOWST();
                                                        }
                                                    });
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

                                    $('#divMainDashboard').hide();
                                    $('#divSalesAnalytics').hide();
                                    $('#divSalesReplenishmentProcessing').hide();
                                    $('#divFranchiseRequest').hide();
                                    $('#divSalesOrderProcessing').hide();
                                    $('#panTransactionDetails').hide();

                                    $('#divOnPrintPOSSalesReport').show();

                                    $('.ItemsBreadCrumbs').text('');
                                    breadcrumbs.prepend(arrowR, 'Deposits Report');
                                    $('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                    BreadCrumbsClickEvent();

                                    //TempValueBtnBack = "BackToDashboard";

                                    $('#panOption').popup('close');
                                });
                                $('#popPOSReportMallLessorAyala').click(function (e) {
                                    e.preventDefault();

                                    GetUser();

                                    function GetUser() {
                                        AjaxSend({
                                            url: 'Login3/GetUser',
                                            success: function (vData) {
                                                if (vData.Error != undefined)
                                                    MessageBox(vData.Error);
                                                else {
                                                    window.pageSecuritySourcePage = $.mobile.activePage;
                                                    PromptUserAccess('Mall Lessor Report', {
                                                        OnClickOKSuccess: function () {

                                                            window.open(GetPathFormat("/MallLessorAyala") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                                                        }
                                                    });
                                                }
                                            },
                                            error: function (vError) {
                                                if (IsMobile)
                                                    proj.PromptRequiresInternet({
                                                        Retry: GetUser
                                                    });
                                                else {
                                                    console.log('Error getting Login/GetUser. Retrying...');
                                                    GetUser();
                                                }
                                                ;
                                            }
                                        });
                                    }
                                });

                                $('#popPOSReportMallLessorSM').click(function (e) {
                                    e.preventDefault();

                                    GetUser();

                                    function GetUser() {

                                        AjaxSend({
                                            url: 'Login3/GetUser',
                                            success: function (vData) {
                                                if (vData.Error != undefined)
                                                    MessageBox(vData.Error);
                                                else {
                                                    window.pageSecuritySourcePage = $.mobile.activePage;
                                                    PromptUserAccess('Mall Lessor Report', {
                                                        OnClickOKSuccess: function () {

                                                            window.open(GetPathFormat("/MallLessorSM") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                                                        }
                                                    });
                                                }
                                            },
                                            error: function (vError) {
                                                if (IsMobile)
                                                    proj.PromptRequiresInternet({
                                                        Retry: GetUser
                                                    });
                                                else {
                                                    console.log('Error getting Login/GetUser. Retrying...');
                                                    GetUser();
                                                }
                                                ;
                                            }
                                        });
                                    }
                                });

                                $('#popPOSReportPrincipal').click(function (e) {
                                    GetUser();

                                    function GetUser() {

                                        AjaxSend({
                                            url: 'Login3/GetUser',
                                            success: function (vData) {
                                                if (vData.Error != undefined)
                                                    MessageBox(vData.Error);
                                                else
                                                    window.open(GetPathFormat("/ReportToPrincipal") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                                            },
                                            error: function (vError) {
                                                if (IsMobile)
                                                    proj.PromptRequiresInternet({
                                                        Retry: GetUser
                                                    });
                                                else {
                                                    console.log('Error getting Login/GetUser. Retrying...');
                                                    GetUser();
                                                }
                                                ;
                                            }
                                        });
                                    }
                                });

                                $('#popPOSReportShopperTrak').click(function (e) {
                                    GetUser();

                                    function GetUser() {

                                        AjaxSend({
                                            url: 'Login3/GetUser',
                                            success: function (vData) {
                                                if (vData.Error != undefined)
                                                    MessageBox(vData.Error);
                                                else
                                                    window.open(GetPathFormat("/ShopperTrak") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                                            },
                                            error: function (vError) {
                                                if (IsMobile)
                                                    proj.PromptRequiresInternet({
                                                        Retry: GetUser
                                                    });
                                                else {
                                                    console.log('Error getting Login/GetUser. Retrying...');
                                                    GetUser();
                                                }
                                                ;
                                            }
                                        });
                                    }
                                });

                                $('#liPOSTransactionDetail').click(function () {
                                    GetUser();

                                    function GetUser() {

                                        AjaxSend({
                                            url: 'Login3/GetUser',
                                            success: function (vData) {
                                                if (vData.Error != undefined)
                                                    MessageBox(vData.Error);
                                                else
                                                    window.open(GetPathFormat("/DashBoardsAndReports/POSTransactionDetail"), "_self");
                                            },
                                            error: function (vError) {
                                                if (IsMobile)
                                                    proj.PromptRequiresInternet({
                                                        Retry: GetUser
                                                    });
                                                else {
                                                    console.log('Error getting Login/GetUser. Retrying...');
                                                    GetUser();
                                                }
                                                ;
                                            }
                                        });
                                    }
                                });

                                $('#liInventoryLedger').click(function () {
                                    GetUser();

                                    function GetUser() {

                                        AjaxSend({
                                            url: 'Login3/GetUser',
                                            success: function (vData) {
                                                if (vData.Error != undefined)
                                                    MessageBox(vData.Error);
                                                else
                                                    window.open(GetPathFormat("/InventoryLedger"), "_self");
                                            },
                                            error: function (vError) {
                                                if (IsMobile)
                                                    proj.PromptRequiresInternet({
                                                        Retry: GetUser
                                                    });
                                                else {
                                                    console.log('Error getting Login/GetUser. Retrying...');
                                                    GetUser();
                                                }
                                                ;
                                            }
                                        });
                                    }
                                });

                                //tusoy oct 7, 2013 3:20pm
                                $('#liMDMDashboard').click(function (e) {
                                    e.preventDefault();

                                    window.pageSecuritySourcePage = $.mobile.activePage;
                                    PromptUserAccess('Devices', {
                                        OnClickOKSuccess: function () {
                                            window.open(GetPathFormat("/DeviceDashboard") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                                        }
                                    });


                                });

                                $('#liOfflineTransactionHistory').click(function (e) {
                                    e.preventDefault();
                                });

                                $('#liDashboardSettings').click(function (e) {
                                    location = MergeUrlParameters({
                                        OrgID: sessionStorage.OrgID
                                    }, '/DashBoardsAndReports/Settings');
                                });
                                $('#liFranchiseGroupSettings').click(function (e) {
                                    AjaxSend({
                                        url: 'Login3/TestCookies',
                                        success: function (vData) {

                                            PromptUserAccess('Group Settings', {
                                                OnClickOKSuccess: function () {
                                                    if (location.hash == '#&ui-state=dialog')
                                                        history.back();

                                                    //                                                        location = MergeUrlParameters({
                                                    //                                                            OrgID: sessionStorage.OrgID
                                                    //                                                        }, '/Franchisor');
                                                    var ismob = CBool(IsMobile) ? 1 : 0;
                                                    window.open('/SetupChecklist/SystemAdministration?OrgID=' + sessionStorage.OrgID + '&IsMobile=' + ismob + '#LibraryPage', '_self')
                                                    sessionStorage["LibraryName"] = 'Group Settings';
                                                    sessionStorage["LabelLibraryName"] = 'Group';
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


                                $('#liFranchiseRequests').click(function (e) {
                                    AjaxSend({
                                        url: 'Login3/TestCookies',
                                        success: function (vData) {

                                            PromptUserAccess('Group Settings', {
                                                OnClickOKSuccess: function () {
                                                    if (location.hash == '#&ui-state=dialog')
                                                        history.back();

                                                    if (CounterFranchiseRequest == true) {
                                                        AjaxSend({
                                                            ExactUrl: '/DashboardsAndReports/FranchiseRequest?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=1',
                                                            type: 'GET',
                                                            dataType: 'text',
                                                            success: function (vData) {
                                                                $('#divFranchiseRequest').empty();
                                                                $('#divFranchiseRequest').append(vData).trigger('create');

                                                                App.Deferred.fulfill('FQ26E5TP76');
                                                            }
                                                        });
                                                        CounterFranchiseRequest = false;

                                                    } else {
                                                        //Do Nothing Here
                                                    }
                                                    $('#divSalesReplenishmentProcessing').hide();

                                                    $('#divMainDashboard, #divSalesAnalytics, #divOnPrintPOSSalesReport').hide();
                                                    $('#divFranchiseRequest').show();
                                                    $('#divSalesOrderProcessing').hide();
                                                    $('#panTransactionDetails').hide();

                                                    $('#pageOrderStatus').hide();
                                                    $('#pageOrderCentral').hide();

                                                    $('.ItemsBreadCrumbs').text('');
                                                    breadcrumbs.prepend(arrowR, 'Franchise Requests');
                                                    $('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                                    BreadCrumbsClickEvent();
                                                    $('#panGoTo, #panOption').popup('close');


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

                                $('.btnMoreOptionInventoryLedger').click(function (e) {
                                    AjaxSend({
                                        url: 'Login3/TestCookies',
                                        success: function (vData) {

                                            PromptUserAccess('Group Settings', {
                                                OnClickOKSuccess: function () {
                                                    if (location.hash == '#&ui-state=dialog')
                                                        history.back();

                                                    if (CounterFranchiseRequest == true) {
                                                        AjaxSend({
                                                            ExactUrl: '/InventoryLedger?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=1',
                                                            type: 'GET',
                                                            dataType: 'text',
                                                            success: function (vData) {
                                                                sessionStorage["Inventory"] = "ledger";
                                                                $('#divFranchiseRequest').empty();
                                                                $('#divFranchiseRequest').append(vData).trigger('create');

                                                                App.Deferred.fulfill('FQ26E5TP76');
                                                            }
                                                        });
                                                        //CounterFranchiseRequest = false;

                                                    } else {
                                                        //Do Nothing Here
                                                    }
                                                    $('#divSalesReplenishmentProcessing').hide();

                                                    $('#divMainDashboard, #divSalesAnalytics, #divOnPrintPOSSalesReport').hide();
                                                    $('#divFranchiseRequest').show();
                                                    $('#divSalesOrderProcessing').hide();
                                                    $('#panTransactionDetails').hide();
                                                    $('#divRequestAndApprovals').hide();

                                                    $('#pageOrderStatus').hide();
                                                    $('#pageOrderCentral').hide();

                                                    $('.ItemsBreadCrumbs').text('');
                                                    breadcrumbs.prepend(arrowR, 'Inventory Ledger');
                                                    $('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                                    BreadCrumbsClickEvent();
                                                    $('#panGoTo, #panOption').popup('close');
                                                    $('.btnSelectFowst, #divBtnPeriod').show();
                                                    GetAllFOWST()

                                                    $('.btnSelectFowst').show();

                                                    if (!CBool(sessionStorage.IsFromInventoryBalance)) {
                                                        $('.btnSelectFowst').click();
                                                    }
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

                                var RecTransPage = '';
                                $('.btnMoreOptionStockReceivingAndTransfers').click(function (e) {
                                    AjaxSend({
                                        url: 'Login3/TestCookies',
                                        success: function (vData) {

                                            PromptUserAccess('Group Settings', {
                                                OnClickOKSuccess: function () {
                                                    if (location.hash == '#&ui-state=dialog')
                                                        history.back();

                                                    if (CounterFranchiseRequest == true) {
                                                        if (RecTransPage == '') {
                                                            AjaxSend({
                                                                ExactUrl: '/Receiving/ReceivingAndTransfersReport?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=1',
                                                                type: 'GET',
                                                                dataType: 'text',
                                                                success: function (vData) {
                                                                    RecTransPage = vData;
                                                                    $('#divFranchiseRequest').empty();
                                                                    $('#divFranchiseRequest').append(vData).trigger('create');

                                                                    App.Deferred.fulfill('FQ26E5TP76');
                                                                }
                                                            });
                                                        }
                                                        else {
                                                            $('#divFranchiseRequest').empty();
                                                            $('#divFranchiseRequest').append(RecTransPage).trigger('create');
                                                            ReceivingTransfer.GetRequiredData();
                                                        }
                                                        //CounterFranchiseRequest = false;

                                                    } else {
                                                        //Do Nothing Here
                                                    }
                                                    $('#divSalesReplenishmentProcessing').hide();

                                                    $('#divMainDashboard, #divSalesAnalytics, #divOnPrintPOSSalesReport').hide();
                                                    $('#divFranchiseRequest').show();
                                                    $('#divSalesOrderProcessing').hide();
                                                    $('#panTransactionDetails').hide();

                                                    $('#pageOrderStatus').hide();
                                                    $('#pageOrderCentral').hide();

                                                    $('.ItemsBreadCrumbs').text('');
                                                    breadcrumbs.prepend(arrowR, 'Stock Receiving and Transfers');
                                                    $('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                                    BreadCrumbsClickEvent();
                                                    $('#panGoTo, #panOption').popup('close');
                                                    $('#divHeaderRequiredParameters').hide();
                                                    $('#divSearchBox').css('visibility', 'hidden');
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

                                $('#btnTransDetailsBack').click(function () {
                                    $('#panTransactionDetails').popup('close');
                                });


                                $('#tblInventoryBalances').on('click', '.btnInventoryBalanceBreakdown', function () {
                                    GetProductBreakdown('ByProductBreakdown', $(this).attr('productid'), $(this).find('.productname').text(), 'Today');
                                })

                                $('#selHeaderListOfFowstGroup').on('click', '.btnLIListofFowstGroup', function () {
                                    //GetInventoryBalanceByFowst($(this).attr('fowstid'), 'Today');
                                    $('.lblSelectedFowstGroup').text($(this).text());
                                    $('#panHeaderListOfGroupFowst').popup('close');
                                })

                                $('#btnGoViewInventoryBalance').click(function () {
                                    $('.ItemsBreadCrumbs').text('');
                                    breadcrumbs.prepend(arrowR, 'Inventory Balances');
                                    $('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                    BreadCrumbsClickEvent();
                                    $('#divMainDashboard').hide();
                                    $('#divInventoryBalancePage').show();
                                    GetInventoryBalanceByFowst($('#selListOfFowstGroup').val(), 'Today');
                                    $('.lblSelectedFowstGroup').text($('#selListOfFowstGroup option[value=' + $('#selListOfFowstGroup').val() + ']').text());
                                    $('.btnSelectFowst, #divBtnPeriod2').show();
                                    $('#divBtnPeriod').hide();
                                })

                                $('.btnMoreOptionInventoryBalance').click(function () {
                                    //$('#panOption').popup('close');
                                    //$('#panListOfFowstGroup').popup('open');
                                    AjaxSend({
                                        url: 'Login3/TestCookies',
                                        success: function (vData) {

                                            PromptUserAccess('Group Settings', {
                                                OnClickOKSuccess: function () {
                                                    if (location.hash == '#&ui-state=dialog')
                                                        history.back();

                                                    if (CounterFranchiseRequest == true) {
                                                        AjaxSend({
                                                            ExactUrl: '/InventoryLedger/InventoryBalance?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=1',
                                                            type: 'GET',
                                                            dataType: 'text',
                                                            success: function (vData) {
                                                                sessionStorage["Inventory"] = "balance";
                                                                $('#divFranchiseRequest').empty();
                                                                $('#divFranchiseRequest').append(vData).trigger('create');

                                                                App.Deferred.fulfill('FQ26E5TP76');
                                                            }
                                                        });
                                                        //CounterFranchiseRequest = false;

                                                    } else {
                                                        //Do Nothing Here
                                                    }
                                                    $('#divSalesReplenishmentProcessing').hide();

                                                    $('#divMainDashboard, #divSalesAnalytics, #divOnPrintPOSSalesReport').hide();
                                                    $('#divFranchiseRequest').show();
                                                    $('#divSalesOrderProcessing').hide();
                                                    $('#panTransactionDetails').hide();

                                                    $('#pageOrderStatus').hide();
                                                    $('#pageOrderCentral').hide();
                                                    $('#divBtnPeriod').hide();

                                                    $('.ItemsBreadCrumbs').text('');
                                                    breadcrumbs.prepend(arrowR, 'Inventory Balance');
                                                    $('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                                    BreadCrumbsClickEvent();
                                                    $('#panGoTo, #panOption').popup('close');

                                                    GetAllFOWST()
                                                    $('.btnSelectFowst').show();
                                                    $('.btnSelectFowst').click();
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
                                //$('.btnMoreOptionInventoryLedger').click(function () {
                                //    $('#panOption').popup('close');
                                //    location = MergeUrlParameters({
                                //        OrgID: sessionStorage.OrgID
                                //    }, '/InventoryLedger');
                                //});
                                $('.btnSelectFowst').click(function () {
                                    $('#panHeaderListOfGroupFowst').popup('open');
                                })

                                $('#btnReportType').click(function () {
                                    if (sessionStorage.PrintingOfflineData == "true") {
                                        $('#lstReportType li:eq(0)').hide();
                                        $('#lstReportType li:eq(1)').hide();
                                    }
                                    else {
                                        $('#lstReportType li:eq(0)').show();
                                        $('#lstReportType li:eq(1)').show();
                                    }
                                    $('#panReportTypeOption').popup('open');
                                })

                                $('#btnReportGroupSales').click(function () {
                                    $('#divGroupSales').show();
                                    $('#dvSalesTrends').hide();
                                    $('#divInventoryLevels').hide();
                                    $('#divMoniesAcc').hide();
                                    $('#divInventoriesAcc').hide();
                                    $('#panReportTypeOption').popup('close');
                                    $('#txtReportType').text('Group Sales');
                                    $('.btnPeriod').show();
                                });

                                $('#btnReportGroupSalesInventory').click(function () {
                                    $('#divGroupSales').hide();
                                    $('#dvSalesTrends').hide();
                                    $('#divInventoryLevels').show();
                                    $('#divMoniesAcc').hide();
                                    $('#divInventoriesAcc').hide();
                                    $('#panReportTypeOption').popup('close');
                                    $('#txtReportType').text('Group Inventory Levels');
                                    $('.btnPeriod').show();
                                    LoadGroupInventoryLevels();
                                })

                                $('#btnReportMoniesAccountability').click(function () {
                                    $('#divGroupSales').hide();
                                    $('#dvSalesTrends').hide();
                                    $('#divInventoryLevels').hide();
                                    $('#divInventoriesAcc').hide();
                                    $('.btnPeriod').show();

                                    AjaxSend({
                                        ExactUrl: '/CashHandling/DepositReport?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                                        type: 'GET',
                                        dataType: 'text',
                                        success: function (vData) {
                                            $('#divMoniesAcc').empty();
                                            $('#divMoniesAcc').append(vData).trigger('create');

                                            App.Deferred.fulfill('FQ26E5TP76');

                                            //setDashboardDate(m_Period);
                                            GetAllFOWST();
                                            sessionStorage.Inventory = ''; //clear this so that when you select fowst, it wont get balances/inventory.
                                            sessionStorage['ReportType'] = 'MoniesAccountability';
                                        }
                                    });

                                    $('#divMoniesAcc').show();

                                    $('#panReportTypeOption').popup('close');
                                    $('#txtReportType').text('Monies Accountability');
                                })

                                $('#btnReportInventoriesAccountability').click(function () {
                                    $('#divGroupSales').hide();
                                    $('#dvSalesTrends').hide();
                                    $('#divInventoryLevels').hide();
                                    $('#divMoniesAcc').hide();
                                    $('.btnPeriod').hide();

                                    AjaxSend({
                                        ExactUrl: '/InventoryCount/Report?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                                        type: 'GET',
                                        dataType: 'text',
                                        success: function (vData) {
                                            $('#divInventoriesAcc').empty();
                                            $('#divInventoriesAcc').append(vData).trigger('create');
                                            isInventoryAccLoaded = true;
                                            App.Deferred.fulfill('FQ26E5TP76');
                                        }
                                    });

                                    if (isInventoryAccLoaded) {
                                        Report(); // InventoryCountReport.js
                                    }

                                    $('#divInventoriesAcc').show();

                                    $('#panReportTypeOption').popup('close');
                                    $('#txtReportType').text('Inventories Accountability');
                                })

                                $('input[name="filterGroupSales"]').click(function () {
                                    arrFilterByOwnType = new Array();
                                    isFilterByOwnType = true;
                                    $('input[name="filterGroupSales"]').checkboxradio('disable');
                                    $('input[name="filterGroupSales"]:checked').each(function () {
                                        arrFilterByOwnType.push($(this).val());
                                    })
                                    GetTotalGrossAndDiscountsByFOWST();
                                });

                                $('#chbInventoryLvlCompanyOwned').click(function (e) {
                                    FilterGroupInventoryLevels();
                                });

                                $('#chbInventoryLvlFranchised').click(function (e) {
                                    FilterGroupInventoryLevels();
                                });

                                $('#chbInventoryLvlCoOwned').click(function (e) {
                                    FilterGroupInventoryLevels();
                                });

                                $('#chbInventoryLvlUnidentified').click(function (e) {
                                    FilterGroupInventoryLevels();
                                });

                                $('#divGroupInventoryLevelsButtons').click(function (e) {
                                    LoadGroupInventoryLevels(true);
                                });

                                $('#btnOfflineCashier').click(function () {
                                    sessionStorage.OfflineType = 'cashier';
                                    PageShow();
                                    $('#panGoToOffline').popup('close');
                                })

                                $('#btnOfflineMonies').click(function () {
                                    sessionStorage.OfflineType = 'monies';
                                    PageShow();
                                    $('#panGoToOffline').popup('close');
                                })

                                $('#btnOfflineInventories').click(function () {
                                    sessionStorage.OfflineType = 'inventories';
                                    PageShow();
                                    $('#panGoToOffline').popup('close');
                                })

                                $('#btnOfflineTransaction').click(function () {
                                    sessionStorage.OfflineType = 'transaction';
                                    PageShow();
                                    $('#panGoToOffline').popup('close');
                                })

                                $('#btnOfflineInventoryLedger').click(function () {
                                    sessionStorage.OfflineType = 'inventoryledger';
                                    PageShow();
                                    $('#panGoToOffline').popup('close');
                                })

                                function GetTMSSalesDistribution(s) {
                                    $('#txtViewType').text('Sales Distribution By ' + s);
                                    setDate();

                                    var data = {
                                        GraphType: 'Distibution',
                                        GroupBy: s,
                                        Fowst: 'All',
                                        From: (m_Period == "CustomPeriod" ? c_DateFrom : $('#txtDateFrom').text()),
                                        To: (m_Period == "CustomPeriod" ? c_DateTo : $('#txtDateTo').text()),
                                        IsRawData: 0
                                    }
                                    m_GroupBy = s;
                                    GetTotalForAllFowst(data);
                                }

                                function setDate() {
                                    var m_Period = '';

                                    if ($.trim($('.lblPeriod:first').text()) == "2 Days Ago") {
                                        m_Period = '2DaysAgo'
                                    }
                                    else if ($.trim($('.lblPeriod:first').text()) == "Yesterday") {
                                        m_Period = 'Yesterday'
                                    }
                                    else if ($.trim($('.lblPeriod:first').text()) == "Today") {
                                        m_Period = 'Today'
                                    }
                                    else if ($.trim($('.lblPeriod:first').text()) == "Week To Date") {
                                        m_Period = 'WeekToDate'
                                    }
                                    else if ($.trim($('.lblPeriod:first').text()) == "Month To Date") {
                                        m_Period = 'MonthToDate'
                                    }
                                    else {
                                        m_Period = 'CustomPeriod'
                                    }

                                    setDashboardDate(m_Period);
                                }

                                function LoadGroupInventoryLevels(vRecache) {
                                    // bookmark: arvin, GroupInventoryLevels

                                    if (CBool(vRecache))
                                        $('#divGroupInventoryLevelsResult').html('Getting latest data. This could take a few minutes.');
                                    else
                                        $('#divGroupInventoryLevelsResult').html('');

                                    AjaxSend({
                                        url: '/DashBoardsAndReports/GroupInventoryLevels',
                                        data: {
                                            GroupID: GroupID,
                                            Recache: CBool(vRecache) ? 1 : 0
                                        },
                                        success: function (vData) {
                                            $('#divGroupInventoryLevelsResult').html('');
                                            if (vData.Error)
                                                MessageBox(vData.Error);
                                            else
                                                Display(vData);
                                        },
                                        error: function () {
                                            $('#divGroupInventoryLevelsResult').html('');
                                            MessageBox('Connection error');
                                        }
                                    }); // END AjaxSend

                                    function Display(vDT) {
                                        var pTable = CreateTable(vDT, {
                                            attr: {
                                                'class': 'ui-body-d ui-shadow table-stripe ui-responsive',
                                                'data-column-btn-theme': 'b'
                                            },
                                            theadAttr: {
                                                'class': 'custom-theme-f'
                                            },
                                            AddAllColumns: false,
                                            Columns: {
                                                FowstName: {
                                                    Text: 'FOWST'
                                                },
                                                OwnershipType: {
                                                    Text: 'Ownership Type'
                                                },
                                                Region: {
                                                    Text: 'Geographical Region'
                                                },
                                                OrgOwners: {
                                                    Text: 'Owners'
                                                },
                                                CompanyName: {
                                                    Text: 'Organization Name'
                                                },
                                                SrpValue: {
                                                    Text: 'Value by SRP',
                                                    DataType: 'number',
                                                    Format: '#,###.00',
                                                    HAlign: 'right'
                                                },
                                                SrpValueOfOpenOrders: {
                                                    Text: 'SRP Value of Open Orders',
                                                    DataType: 'number',
                                                    Format: '#,###.00',
                                                    HAlign: 'right'
                                                },
                                                LastUpdated: {
                                                    Text: 'As Of',
                                                    AsHtml: true,
                                                    DataType: 'date',
                                                    WrapDate: true
                                                }
                                            },
                                            BeforeCell: function (e) {
                                                switch (e.Key) {
                                                    case 'SrpValue':
                                                    case 'SrpValueOfOpenOrders':
                                                        // e.Text = AccountingFormat(e.DataRow.Total);
                                                        e.Text = AccountingFormat(e.Text);
                                                        return true;

                                                    default:
                                                        return true;
                                                }
                                            },
                                            AfterRow: function (e) {
                                                e.Row.css('cursor', 'pointer');
                                                e.Row.click(function (e1) {
                                                    var pDR = $($(e1.target).closest('tr')).data('data');
                                                    var pUrl = MergeUrlParameters({
                                                        OrgID: sessionStorage.OrgID,
                                                        FowstID: pDR.FowstID,
                                                        View: 'Inventory'
                                                    }, '/DashBoardsAndReports');
                                                    window.open(pUrl, '_blank');
                                                });
                                            }
                                        });
                                        $('#divGroupInventoryLevelsResult').append(pTable);
                                        pTable.table();
                                        pTable.trigger('create');

                                    } // END Display
                                } // END LoadGroupInventoryLevels

                                function FilterGroupInventoryLevels() {
                                    var pTable = $('#divGroupInventoryLevelsResult tr');
                                    if (pTable.length == 0)
                                        return;
                                    var pCompanyOwned = $('#chbInventoryLvlCompanyOwned').prop('checked');
                                    var pFranchised = $('#chbInventoryLvlFranchised').prop('checked');
                                    var pCoOwned = $('#chbInventoryLvlCoOwned').prop('checked');
                                    var pUnidentified = $('#chbInventoryLvlUnidentified').prop('checked');
                                    var pData;
                                    var pRow;
                                    var pVisible;

                                    for (var pIndex = 0; pIndex <= pTable.length; pIndex++) {
                                        pVisible = false;
                                        pRow = $(pTable[pIndex]);
                                        pData = pRow.data('data');
                                        if (pData == null)
                                            continue;
                                        if (pData.OwnershipType == 'Company-Owned')
                                            pVisible = pVisible || pCompanyOwned;

                                        if (pData.OwnershipType == 'Franchised')
                                            pVisible = pVisible || pFranchised;

                                        if (pData.OwnershipType == 'Co-Owned')
                                            pVisible = pVisible || pCoOwned;

                                        if (pData.OwnershipType == 'Unidentified')
                                            pVisible = pVisible || pUnidentified;

                                        if (pVisible)
                                            pRow.show();
                                        else
                                            pRow.hide();
                                    }
                                }

                                (function Init() {
                                    var pParams = GetUrlParameters();
                                    var pData;

                                    if ((pParams.View == 'Inventory')
                                        && (IsGuid(pParams.FowstID))) {
                                        // bookmark: arvin, parked
                                        $('.btnMoreOptionInventoryBalance').click();
                                    }
                                })(); // END Init
                            }); // END $(function () {

                            function DateFormat(Month, Date, Year) {
                                var months = {
                                    '1': 'Jan',
                                    '2': 'Feb',
                                    '3': 'Mar',
                                    '4': 'Apr',
                                    '5': 'May',
                                    '6': 'Jun',
                                    '7': 'Jul',
                                    '8': 'Aug',
                                    '9': 'Sep',
                                    '10': 'Oct',
                                    '11': 'Nov',
                                    '12': 'Dec'
                                };
                                var validDate = months[Month] + " " + Date + ", " + Year;
                                return validDate;
                            }

                            function GetMainDashboardPage() {
                                if (sessionStorage.IsShowFirstPage == 'false') {

                                    $('#divSalesAnalytics, .btnPeriod, .lblPeriod').fadeIn();
                                    //$('#btnPeriodMobile').addClass('btnPeriod').removeClass('btn-disable').on('click.disabled');
                                    $('#divMainDashboard, #divOnPrintPOSSalesReport,' +
                                        ' #divOrderCentral,' +
                                        '#divSpecialReports, #divAuditTrail,' +
                                        '#divSettings, .divSearchFilterDateRange,#panTransactionDetails, #divInventoryBalancePage').hide();
                                    $('#divFranchiseRequest').hide();
                                    $('#divSalesReplenishmentProcessing').hide();
                                    $('#divSalesOrderProcessing').hide();
                                    $('.ItemsBreadCrumbs').text('Main Dashboard');
                                    //                    TempValueBtnBack = null;
                                    sessionStorage.SearchMode = 'false';
                                    GetPageAccess();
                                    GetData()
                                } else {
                                    $('#divMainDashboard, .btnPeriod, .lblPeriod').fadeIn();
                                    //$('#btnPeriodMobile').addClass('btnPeriod').removeClass('btn-disable').on('click.disabled');
                                    $('#divSalesAnalytics, #divOnPrintPOSSalesReport,' +
                                        ' #divOrderCentral,' +
                                        '#divSpecialReports, #divAuditTrail,' +
                                        '#divSettings, .divSearchFilterDateRange,#panTransactionDetails, #divInventoryBalancePage').hide();
                                    $('#divFranchiseRequest').hide();
                                    $('#divSalesReplenishmentProcessing').hide();
                                    $('#divSalesOrderProcessing').hide();
                                    $('.ItemsBreadCrumbs').text('Main Dashboard');
                                    TempValueBtnBack = null;
                                    sessionStorage.SearchMode = 'false';
                                    //GetPageAccess();
                                    GetData();
                                }
                            }

                            function GetCashierSalesReportPage() {
                                sessionStorage.POSSalesReportType = 0;
                                if (CounterCashierSalesReport == true) {
                                    if (CBool(sessionStorage.PrintingOfflineData)) {
                                        AjaxSend({
                                            url: 'Login3/TestCookies',
                                            success: function (vData) {
                                                PromptUserAccess('Cashier Sales Report', {
                                                    OnClickOKSuccess: function () {

                                                        var pData = {};
                                                        pData.OrgID = sessionStorage["OrgID"];
                                                        pData.POSSalesReportType = 0;

                                                        AjaxSend({
                                                            url: 'ReportsPOS/setPOSSalesReportType',
                                                            data: pData,
                                                            success: function (vData) {
                                                                AjaxSend({
                                                                    ExactUrl: '/ReportsPOS/SalesReport?cache=1&OrgID=' + sessionStorage.OrgID + '&IsManagerSalesReport=0&IsMobile=' + IsMobile,
                                                                    type: 'GET',
                                                                    dataType: 'text',
                                                                    success: function (vData) {

                                                                        $('#divOnPrintPOSSalesReport').empty();
                                                                        $('#divOnPrintPOSSalesReport').append(vData).trigger('create');

                                                                        App.Deferred.fulfill('FQ26E5TP76');

                                                                        setMonthCollection("selPOSSalesReportByDateFromMonth");
                                                                        setYearCollection("selPOSSalesReportByDateFromYear");

                                                                        setMonthCollection("selPOSSalesReportByDateToMonth");
                                                                        setYearCollection("selPOSSalesReportByDateToYear");

                                                                        setDashboardDate(m_Period);

                                                                        $('.btnSelectFowst').show();

                                                                    }
                                                                });
                                                            }
                                                        });

                                                    }
                                                });
                                            },
                                            error: function (vError) {
                                                PromptUserAccess('Cashier Sales Report', {
                                                    OnClickOKSuccess: function () {
                                                        AjaxSend({
                                                            ExactUrl: '/ReportsPOS/SalesReport?cache=1&OrgID=' + sessionStorage.OrgID + '&IsManagerSalesReport=0&IsMobile=' + IsMobile,
                                                            type: 'GET',
                                                            dataType: 'text',
                                                            success: function (vData) {
                                                                $('#divOnPrintPOSSalesReport').empty();
                                                                $('#divOnPrintPOSSalesReport').append(vData).trigger('create');

                                                                App.Deferred.fulfill('FQ26E5TP76');

                                                                setMonthCollection("selPOSSalesReportByDateFromMonth");
                                                                setYearCollection("selPOSSalesReportByDateFromYear");

                                                                setMonthCollection("selPOSSalesReportByDateToMonth");
                                                                setYearCollection("selPOSSalesReportByDateToYear");

                                                                setDashboardDate(m_Period);

                                                                $('.btnSelectFowst').show();
                                                            }
                                                        });
                                                    }
                                                });
                                            }
                                        });

                                        CounterCashierSalesReport = false;
                                    } else {
                                        PromptUserAccess('Cashier Sales Report', {
                                            OnClickOKSuccess: function () {
                                                var isPageReady = false;
                                                var pData = {};
                                                pData.OrgID = sessionStorage["OrgID"];
                                                pData.POSSalesReportType = 0;

                                                AjaxSend({
                                                    url: 'ReportsPOS/setPOSSalesReportType',
                                                    data: pData,
                                                    success: function (vData) {
                                                        if (cashiermanagerpage == "") {
                                                            AjaxSend({
                                                                ExactUrl: '/ReportsPOS/SalesReport?cache=1&OrgID=' + sessionStorage.OrgID + '&IsManagerSalesReport=0&IsMobile=' + IsMobile,
                                                                type: 'GET',
                                                                dataType: 'text',
                                                                success: function (vData) {
                                                                    isPageReady = true;
                                                                    cashiermanagerpage = vData;
                                                                    $('#divOnPrintPOSSalesReport').empty();
                                                                    $('#divOnPrintPOSSalesReport').append(vData).trigger('create');

                                                                    App.Deferred.fulfill('FQ26E5TP76');

                                                                    setMonthCollection("selPOSSalesReportByDateFromMonth");
                                                                    setYearCollection("selPOSSalesReportByDateFromYear");

                                                                    setMonthCollection("selPOSSalesReportByDateToMonth");
                                                                    setYearCollection("selPOSSalesReportByDateToYear");

                                                                    setDashboardDate(m_Period);

                                                                    $('.btnSelectFowst').show();
                                                                }
                                                            });
                                                        }
                                                        else {
                                                            $('#divOnPrintPOSSalesReport').empty();
                                                            $('#divOnPrintPOSSalesReport').append(cashiermanagerpage).trigger('create');
                                                            SalesReport.GetAllFowstinSalesReportPage();
                                                        }
                                                    }
                                                });
                                            }
                                        });
                                    }
                                } else {
                                    //Do Nothing Here
                                }

                                $('#divMainDashboard').hide();
                                $('#divSalesAnalytics').hide();
                                $('#divSalesReplenishmentProcessing').hide();
                                $('#divFranchiseRequest').hide();
                                $('#divSalesOrderProcessing').hide();
                                $('#panTransactionDetails').hide();

                                $('#divOnPrintPOSSalesReport').show();

                                $('.ItemsBreadCrumbs').text('');

                                if (CBool(sessionStorage.PrintingOfflineData)) {
                                    $('.ItemsBreadCrumbs').text('Cashier\'s Sales Report');
                                }
                                else {
                                    breadcrumbs.prepend(arrowR, 'Cashier\'s Sales Report');
                                    $('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                    BreadCrumbsClickEvent();
                                }

                                //TempValueBtnBack = "BackToDashboard";

                                $('#panOption').popup('close');
                            }

                            window.GetData = function () {
                                var m_DateFrom = new Date(); //$.format.date(new Date(), 'MM/dd/yyyy');
                                var m_DateTo = new Date(); //$.format.date(new Date(), 'MM/dd/yyyy');

                                clearData();

                                switch (m_Period) {
                                    case "2DaysAgo":
                                        //back date from
                                        m_DateFrom.setDate(m_DateFrom.getDate() - 2);
                                        m_DateTo.setDate(m_DateTo.getDate() - 2);
                                        break;
                                    case "Yesterday":
                                        //back date from    
                                        m_DateFrom.setDate(m_DateFrom.getDate() - 1);
                                        m_DateTo.setDate(m_DateTo.getDate() - 1);
                                        break;
                                    case "Today":
                                        //                        m_DateFrom = (m_DateFrom.getMonth() + 1) + '/' + m_DateFrom.getDate() + '/' + m_DateFrom.getFullYear();
                                        //                        m_DateTo = (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear();
                                        break;
                                    case "WeekToDate":
                                        m_DateFrom.setDate(m_DateFrom.getDate() - m_DateFrom.getDay());
                                        m_DateTo = (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear();
                                        break;
                                    case "MonthToDate":
                                        GroupDateBy = 'Month';
                                        m_DateFrom = (m_DateFrom.getMonth() + 1) + '/1/' + m_DateFrom.getFullYear();
                                        m_DateTo = (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear();
                                        break;
                                    default:
                                        var newDateFrom = new Date($('.fromCalendar').calendar('getDates'));
                                        var newDateTo = new Date($('.toCalendar').calendar('getDates'));
                                        GroupDateBy = 'Month';
                                        m_DateFrom = (newDateFrom.getMonth() + 1) + '/' + newDateFrom.getDate() + '/' + newDateFrom.getFullYear();
                                        m_DateTo = (newDateTo.getMonth() + 1) + '/' + newDateTo.getDate() + '/' + newDateTo.getFullYear();

                                        //for cashier or manager sales reports
                                        $("#selPOSSalesReportRange").val("Day").selectmenu('refresh');
                                        $('#selPOSSalesReportByDateFromMonth').val(newDateFrom.getMonth() + 1).selectmenu('refresh');
                                        $('#selPOSSalesReportByDateFromYear').val(newDateFrom.getFullYear()).selectmenu('refresh');
                                        setDayCollection('selPOSSalesReportByDateFromMonth', 'selPOSSalesReportByDateFromDay', 'selPOSSalesReportByDateFromYear', true);
                                        $('#selPOSSalesReportByDateFromDay').val(newDateFrom.getDate()).selectmenu('refresh');

                                        $('#selPOSSalesReportByDateToMonth').val(newDateTo.getMonth() + 1).selectmenu('refresh');
                                        $('#selPOSSalesReportByDateToYear').val(newDateTo.getFullYear()).selectmenu('refresh');
                                        setDayCollection('selPOSSalesReportByDateToMonth', 'selPOSSalesReportByDateToDay', 'selPOSSalesReportByDateToYear', true);
                                        $('#selPOSSalesReportByDateToDay').val(newDateTo.getDate()).selectmenu('refresh');

                                        //for z report
                                        $("#selZReportRange").val("By Date").selectmenu('refresh');
                                        $('#selZReportByDateMonth').val(newDateFrom.getMonth() + 1).selectmenu('refresh');
                                        $('#selZReportByDateYear').val(newDateFrom.getFullYear()).selectmenu('refresh');
                                        setDayCollection('selZReportByDateMonth', 'selZReportByDateDay', 'selZReportByDateYear', true);
                                        $('#selZReportByDateDay').val(newDateFrom.getDate()).selectmenu('refresh');

                                        break;
                                }

                                if (!IsDate(m_DateFrom)) {
                                    $('#txtDateFrom').text('Please select date from');
                                    return;
                                } else if (!IsDate(m_DateTo)) {
                                    $('#txtDateTo').text('Please select date to');
                                    return;
                                } else {
                                    $('#panCustomDates').popup('close');
                                }

                                if (sessionStorage.ReportType == 'MoniesAccountability') {
                                    GetDepositReport(); //DepositReport.js
                                }

                                //if (sessionStorage.PrintingOfflineData == "false") {
                                data = {
                                    GroupBy: 'All',
                                    Fowst: 'All',
                                    From: m_DateFrom,
                                    To: m_DateTo,
                                    IsRawData: 0
                                };

                                if (sessionStorage.SearchMode == 'true') {
                                    m_SearchData = {
                                        GroupBy: 'Day',
                                        Fowst: sessionStorage.FowstID,
                                        From: data.From,
                                        To: data.To,
                                        IsRawData: 0
                                    };

                                    m_SearchData.IsSearch = 1;
                                    m_SearchData.SearchString = $('.txtSearch').val();
                                    window.GetSearchData(m_SearchData);

                                } else {

                                    GetTotalGrossAndDiscounts();
                                    data.GroupBy = 'ByOrganizationGroup';
                                    GetTotalGrossAndDiscountsByGROUP();
                                    GetTotalGrossAndDiscountsByFOWST();

                                    data.GraphType = ($('#txtViewType').text() == "Sales Trend" ? "Trend" : "Distribution");
                                    data.GroupBy = m_GroupBy;

                                    GetTotalForAllFowst(data); //TenantMonitoringSystemChart.js

                                    window.GetSearchData(data);
                                    //SliceData('Date');

                                    //if (CBool(sessionStorage.IsShowFirstPage)) {
                                    //    GetTotalGrossAndDiscounts();
                                    //    data.GroupBy = 'ByOrganizationGroup';
                                    //    GetTotalGrossAndDiscountsByGROUP();
                                    //} else {
                                    //    if (TempValueBtnBack == null) {
                                    //        TempValueBtnBack == 'BackToDashboard';
                                    //    }
                                    //    GetTotalGrossAndDiscountsByFOWST();
                                    //}
                                }
                                //}
                            }


                            function GetTotalGrossAndDiscounts() {
                                AjaxSend({
                                    url: 'DashboardsAndReports/GetTotalGrossAndDiscounts',
                                    data: data,
                                    success: function (vData) {
                                        if (vData.Results.Rows.length > 0) {
                                            DisplayTotalGrossAndDiscounts(vData.Results.Rows);
                                        }
                                    }
                                });
                            }

                            function DisplayTotalGrossAndDiscounts(vRows) {

                                $('#TotalExchanges').html(AccountingFormat(vRows[0].TotalReturns));
                                $('#TotalGrossSales').text(AccountingFormat(vRows[0].GrossSales));
                                $('#TotalDiscounts').text(AccountingFormat(vRows[0].TotalDiscounts));

                                $('#TotalNetSales').text(AccountingFormat(CDec2($('#TotalGrossSales').text()) - CDec2($('#TotalDiscounts').text()) - CDec2($('#TotalExchanges').text())));

                                $('#TotalRefunds').text('-');
                            }

                            function GetTotalGrossAndDiscountsByGROUP() {
                                AjaxSend({
                                    url: 'DashboardsAndReports/GetTotalGrossAndDiscountsByGROUP',
                                    data: data,
                                    success: function (vData) {
                                        if (vData.Results.Rows.length > 0) {
                                            DisplayTotalGrossAndDiscountsByGroup(vData.Results.Rows);
                                        }
                                    }
                                });
                            }

                            function GetTotalSalesReturnsDataByGROUP(GroupID) {
                                if (sessionStorage.IsShowFirstPage == 'true') {
                                    data.GroupID = GroupID;
                                }
                                AjaxSend({
                                    url: 'DashboardsAndReports/GetTotalSalesReturnsDataByGROUP',
                                    data: data,
                                    success: function (vData) {
                                        if (vData.Results.Rows.length > 0) {
                                            $('#' + (vData.Results.Rows[0].GroupID == undefined ? 'Unsorted' : vData.Results.Rows[0].GroupID)).text(AccountingFormat(vData.Results.Rows[0].Total));
                                        }
                                    }
                                });
                            }

                            function DisplayTotalGrossAndDiscountsByGroup(vRows) {
                                var pLV = $('#MainFrame')
                                var outsideGrid = 0;
                                var outsideGridBlock = "a";
                                var insideGrid = 0;
                                var insideGridBlock = "a";
                                var pItem = '';

                                pLV.empty();

                                for (i = 0; i <= vRows.length - 1; i++) {

                                    if ((i % 3) == 0) {
                                        insideGrid = 0;
                                        pItem = $('<div class="ui-grid-b">')
                                    }

                                    switch (insideGrid) {
                                        case 0:
                                            var insideGridBlock = "a";
                                            break;
                                        case 1:
                                            var insideGridBlock = "b";
                                            break;
                                        case 2:
                                            var insideGridBlock = "c";
                                            break;
                                    }


                                    pItem.append($('<div class="ui-block-' + insideGridBlock + '" style="padding-right:10px; padding-top:10px;">')
                                        .append($('<span>')
                                            .append($('<a class="btn custom-btn-theme-c">')
                                                .append($('<div style="padding:15px;font-weight:normal; color:#000; font-size:20px; margin:0px auto;">')
                                                    .append($('<div style="font-weight:bold; font-size:20px; text-align:left; text-overflow:ellipsis; white-space:nowrap; overflow:hidden;">')
                                                        .html("<span>" + vRows[i].GroupName + "</span>")
                                                    )
                                                    .append($('<hr style="border:1px solid #CCC"/>')
                                                    )
                                                    .append($('<div style="text-align:left; font-size:13px">')
                                                        .text('Net Sales')
                                                    )
                                                    .append($('<div style="text-align:right; font-size:15px;"">')
                                                        .text(AccountingFormat(parseFloat(vRows[i].GrossSales) - parseFloat(vRows[i].TotalDiscounts) - parseFloat(vRows[i].TotalReturns == undefined ? '0' : vRows[i].TotalReturns)))
                                                    )
                                                    .append($('<div style="text-align:left; font-size:13px">')
                                                        .text('Gross Sales')
                                                    )
                                                    .append($('<div style="text-align:right; font-size:15px;"">')
                                                        .text(AccountingFormat(vRows[i].GrossSales))
                                                    )
                                                    .append($('<div style="text-align:left; font-size:13px">')
                                                        .text('Discounts')
                                                    )
                                                    .append($('<div style="text-align:right; font-size:15px;"">')
                                                        .text(AccountingFormat(vRows[i].TotalDiscounts))
                                                    )
                                                    .append($('<div style="text-align:left; font-size:13px">')
                                                        .text('Refunds')
                                                    )
                                                    .append($('<div style="text-align:right; font-size:15px;"">')
                                                        .text('-')
                                                    )
                                                    .append($('<div style="text-align:left; font-size:13px">')
                                                        .text('Exchanges')
                                                    )
                                                    .append($('<div style="text-align:right; font-size:15px;">')
                                                        .text(AccountingFormat(vRows[i].TotalReturns))
                                                    )
                                                )
                                                .data('data', vRows[i])
                                                .click(function () {
                                                    This = $(this);
                                                    //$('#panGoTo').popup('open');
                                                    $('.btnSalesAnalytics').click(function () {
                                                        $('#panGoTo').popup('close');
                                                        $('.ItemsBreadCrumbs').text('');
                                                        breadcrumbs.prepend(arrowR, 'Group Reports for ' + This.data('data').GroupName + ' Group');
                                                        $('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                                        BreadCrumbsClickEvent();
                                                        $('#divMainDashboard').hide();

                                                        $('#divSalesAnalytics').fadeIn('fast', function () {

                                                            var ns = $('#NetSalesHeight').height(),
                                                                gs = $('#GrossSalesHeight').height(),
                                                                et = $('#ExtrasTotalHeight').height();

                                                            ClearCharts();
                                                            TempValueBtnBack = 'BackToDashboard';

                                                            if ($(window).width() >= 576) {
                                                                if (ns > gs) {
                                                                    $('#GrossSalesHeight, #ExtrasTotalHeight').css('height', ns);
                                                                } else if (ns < gs) {
                                                                    $('#NetSalesHeight, #ExtrasTotalHeight').css('height', gs);
                                                                } else if (et > ns || et > gs) {
                                                                    $('#NetSalesHeight, #GrossSalesHeight').css('height', et);
                                                                } else if (ns == gs) {
                                                                    $('#NetSalesHeight, #ExtrasTotalHeight').css('height', gs);
                                                                }
                                                            } else {
                                                                $('#NetSalesHeight, #GrossSalesHeight, #ExtrasTotalHeight').css('height', 'auto');
                                                            }
                                                        });

                                                        $('#lblGroupName').text(This.data('data').GroupName + ' Group Sales')
                                                        $('#lblGroupNetSales').text(AccountingFormat(parseFloat(This.data('data').GrossSales) - parseFloat(This.data('data').TotalDiscounts) - parseFloat(This.data('data').TotalReturns == undefined ? '0' : This.data('data').TotalReturns)));
                                                        $('#lblGroupGrossSales').text(AccountingFormat(This.data('data').GrossSales));
                                                        $('#lblGroupDiscounts').text(AccountingFormat(This.data('data').TotalDiscounts));
                                                        $('#lblGroupRefunds').text('-');
                                                        $('#lblGroupExchanges').text(AccountingFormat(This.data('data').TotalReturns));
                                                        $('input[name="filterGroupSales"]').attr('checked', 'true').checkboxradio('refresh');
                                                        $('#btnReportGroupSales').click();

                                                        TempValueBtnBack == 'BackToDashboard';
                                                        data.GroupBy = 'ByOrganizationFOWST';
                                                        GroupID = This.data('data').GroupID;
                                                        GetTotalGrossAndDiscountsByFOWST();
                                                        isFilterByOwnType = false;
                                                    });

                                                    $('.btnInventoryBalance').click(function () {
                                                        //$('#lblgroupid').text(This.data('data').GroupID);
                                                        $('#panGoTo').popup('close');
                                                        //$('#panListOfFowstGroup').popup('open');
                                                        $('.btnMoreOptionInventoryBalance').click();
                                                    });

                                                    $('.btnInventoryLedger').click(function () {
                                                        $('#panGoTo').popup('close');
                                                        $('.btnMoreOptionInventoryLedger').click();
                                                    });

                                                    $('.btnStockReceivingTransfers').click(function () {
                                                        $('#panGoTo').popup('close');
                                                        $('.btnMoreOptionStockReceivingAndTransfers').click();
                                                    });

                                                    $('.btnSalesAnalytics').click();
                                                })
                                            )//.append($('<a
                                        )
                                    )
                                    pLV.append(pItem)

                                    insideGrid++;
                                }
                            }

                            function BreadCrumbsClickEvent() {

                                if (sessionStorage.IsShowFirstPage == 'false') {
                                    $('#btnMainDashboard').click(function () {

                                        $('#divSalesAnalytics, .btnPeriod, .lblPeriod').show();
                                        //$('#btnPeriodMobile').addClass('btnPeriod').removeClass('btn-disable').on('click.disabled');
                                        $('#divMainDashboard, #divOnPrintPOSSalesReport,' +
                                            ' #divOrderCentral,' +
                                            '#divSpecialReports, #divAuditTrail,' +
                                            '#divSettings, .divSearchFilterDateRange,#panTransactionDetails, #divInventoryBalancePage').hide();
                                        $('#divFranchiseRequest').hide();
                                        $('#divOfflineTransactionHistoryMain').hide();
                                        $('#divSalesReplenishmentProcessing').hide();
                                        $('#divSalesOrderProcessing').hide();
                                        $('.ItemsBreadCrumbs').text('Main Dashboard');
                                        $('.btnPeriod').show();
                                        //                    TempValueBtnBack = null;
                                        sessionStorage.SearchMode = 'false';
                                        GetData();
                                        $('.btnSelectFowst, #divBtnPeriod2').hide();
                                    })
                                } else {
                                    $('#btnMainDashboard').click(function () {

                                        $('#divMainDashboard, .btnPeriod, .lblPeriod').show();
                                        //$('#btnPeriodMobile').addClass('btnPeriod').removeClass('btn-disable').on('click.disabled');
                                        $('#divSalesAnalytics, #divOnPrintPOSSalesReport,' +
                                            ' #divOrderCentral,' +
                                            '#divSpecialReports, #divAuditTrail,' +
                                            '#divSettings, .divSearchFilterDateRange,#panTransactionDetails, #divInventoryBalancePage, #divInventoryProductBreakdown').hide();
                                        $('#divFranchiseRequest').hide();
                                        $('#divOfflineTransactionHistoryMain, #divOfflineTransactionDetails').hide();
                                        $('#divOfflineTransactionMain').show();
                                        $('#divSalesReplenishmentProcessing').hide();
                                        $('#divSalesOrderProcessing').hide();
                                        $('.ItemsBreadCrumbs').text('Main Dashboard');
                                        $('.btnPeriod').show();
                                        TempValueBtnBack = null;
                                        sessionStorage.SearchMode = 'false';
                                        GetData();
                                        $('.btnSelectFowst, #divBtnPeriod2').hide();
                                    });
                                    $('#btnBCInventoryBalances').click(function () {

                                        $('#divInventoryBalancePage').fadeIn();
                                        //$('#btnPeriodMobile').addClass('btnPeriod').removeClass('btn-disable').on('click.disabled');
                                        $('#divSalesAnalytics, #divOnPrintPOSSalesReport,' +
                                            ' #divOrderCentral,' +
                                            '#divSpecialReports, #divAuditTrail,' +
                                            '#divSettings, .divSearchFilterDateRange, #panTransactionDetails, #divMainDashboard, #divInventoryProductBreakdown').hide();
                                        $('#divFranchiseRequest').hide();
                                        $('#divSalesReplenishmentProcessing').hide();
                                        $('#divSalesOrderProcessing').hide();
                                        $('.ItemsBreadCrumbs').text('');
                                        breadcrumbs.prepend(arrowR, 'Inventory Balances');
                                        $('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                        BreadCrumbsClickEvent();
                                        TempValueBtnBack = null;
                                        sessionStorage.SearchMode = 'false';
                                        GetData();
                                    });
                                }
                            }

                            function GetGroupToSelectMenu() {
                                AjaxSend({
                                    url: 'DashboardsAndReports/GetFranchiseGroups',
                                    success: function (vData) {
                                        var vRows = vData.FranchiseData.Rows;

                                        $('#divMainDashboard').trigger('create');

                                        $('#selListOfFowstGroup').empty();
                                        $('#selHeaderListOfFowstGroup').empty();

                                        $('#selListOfFowstGroup').append('<option value="All">All</option>');
                                        $('#selHeaderListOfFowstGroup').append('<li class="btnLIListofFowstGroup" fowstid="All" style="padding-bottom:2px;">' +
                                            '<a href="#"class="btn custom-btn-theme-c txt-left" style="text-decoration:none; padding-bottom:10px; padding-top:10px;">' +
                                                '<span style="padding:10px; margin-left:7px;">All</span></a></li>');

                                        for (var p = 0; p <= vRows.length - 1; p++) {
                                            $('#selListOfFowstGroup').append('<option value="' + vRows[p].ID + '">' + vRows[p].GroupName + '</option>');
                                            $('#selHeaderListOfFowstGroup').append('<li class="btnLIListofFowstGroup" fowstid="' + vRows[p].ID + '" style="padding-bottom:2px;">' +
                                                '<a href="#" class="btn custom-btn-theme-c txt-left" style="text-decoration:none; padding-bottom:10px; padding-top:10px;">' +
                                                    '<span style="padding:10px; margin-left:7px;">' + vRows[p].GroupName + '</span></a></li>');
                                        }

                                        $('#selListOfFowstGroup').selectmenu('refresh', true);
                                        //$('#selHeaderListOfFowstGroup').listview('refresh');

                                    } // end of success
                                });
                            }

                            function GetInventoryBalanceByFowst(GroupBy, m_Period) {
                                $('#panListOfFowstGroup').popup('close');

                                $('#tblInventoryBalances').empty();
                                $('#divInventoryProductBreakdown').hide();

                                var m_DateFrom = new Date();
                                var m_DateTo = new Date();

                                switch (m_Period) {
                                    case "Today":
                                        m_DateFrom = (m_DateFrom.getMonth() + 1) + '/' + m_DateFrom.getDate() + '/' + m_DateFrom.getFullYear();
                                        m_DateTo = (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear();
                                        break;
                                }

                                data.GroupBy = GroupBy;
                                data.From = m_DateFrom;
                                data.To = m_DateTo;

                                AjaxSend({
                                    url: 'DashboardsAndReports/InventoryBalance',
                                    data: data,
                                    success: function (vData) {
                                        if (vData.Results.Rows.length == 0) {
                                            $('#tblInventoryBalances').append('<tr><td colspan="6" style="text-align:center;">No Result(s) Found</td></tr>');
                                        }
                                        else {
                                            var count = 1;
                                            for (var i = 0; i < vData.Results.Rows.length; i++) {
                                                $('#tblInventoryBalances').append('' +
                                                    '<tr style="cursor:pointer;" class="btnInventoryBalanceBreakdown" productid="' + vData.Results.Rows[i].ProductID + '">' +
                                                        '<td>' + count + '.</td>' +
                                                        '<td>' + vData.Results.Rows[i].ProductNo + '</td>' +
                                                        '<td><span  class="productname">' + vData.Results.Rows[i].ProductName + '</span></td>' +
                                                        '<td>' + vData.Results.Rows[i].Balance + '</td>' +
                                                        '<td>' + vData.Results.Rows[i].UnitMeasure + '</td>' +
                                                        '<td>' + vData.Results.Rows[i].LastUpdated + '</td>' +
                                                    '</tr>');
                                                count += 1;
                                            }
                                        }
                                    }
                                });
                            }

                            function GetProductBreakdown(GroupBy, ProductId, ProductName, m_Period) {
                                var m_DateFrom = new Date();
                                var m_DateTo = new Date();

                                switch (m_Period) {
                                    case "Today":
                                        m_DateFrom = (m_DateFrom.getMonth() + 1) + '/' + m_DateFrom.getDate() + '/' + m_DateFrom.getFullYear();
                                        m_DateTo = (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear();
                                        break;
                                }

                                data.GroupBy = GroupBy;
                                data.ProductId = ProductId;
                                data.From = m_DateFrom;
                                data.To = m_DateTo;

                                $('#tblInventoryProductBreakdown').empty();
                                AjaxSend({
                                    url: 'DashboardsAndReports/GetProductBreakdown',
                                    data: data,
                                    success: function (vData) {
                                        if (vData.Results.Rows.length == 0) {
                                            $('#tblInventoryProductBreakdown').append('<tr><td colspan="6">No Result(s) Found</td></tr>');
                                        }
                                        else {
                                            var count = 1;
                                            for (var i = 0; i < vData.Results.Rows.length; i++) {
                                                $('#tblInventoryProductBreakdown').append('' +
                                                    '<tr productid="' + vData.Results.Rows[i].ProductID + '">' +
                                                        '<td>' + count + '.</td>' +
                                                        '<td>' + vData.Results.Rows[i].LastUpdated + '</td>' +
                                                        '<td>' + vData.Results.Rows[i].Transaction + '</td>' +
                                                        '<td>' + vData.Results.Rows[i].Qty + '</td>' +
                                                        '<td>' + vData.Results.Rows[i].UnitMeasure + '</td>' +
                                                        '<td>' + vData.Results.Rows[i].Balance + '</td>' +
                                                    '</tr>');
                                                count += 1;
                                            }
                                            $('#divInventoryBalancePage').hide();
                                            $('#divInventoryProductBreakdown').show()
                                            $('.ItemsBreadCrumbs').text('');
                                            breadcrumbs.prepend(arrowR, ProductName + ' Ledger');
                                            breadcrumbs.prepend(arrowR, '<a id="btnBCInventoryBalances">Inventory Balances</a>');
                                            $('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                            BreadCrumbsClickEvent();
                                        }
                                    }
                                });
                            }

                            function GetTotalGrossAndDiscountsByFOWST() {
                                if (sessionStorage.IsShowFirstPage == 'true') {
                                    if (GroupID != "") {
                                        data.GroupID = GroupID;
                                    }
                                }

                                if (isFilterByOwnType) {
                                    data.FilterOwnType = arrFilterByOwnType;
                                }
                                else {
                                    data.FilterOwnType = [];
                                }

                                $('#reportTableHeader th').show();
                                $('.chbColumns').prop('checked', true).checkboxradio('refresh');

                                AjaxSend({
                                    url: 'DashboardsAndReports/GetTotalGrossAndDiscountsByFOWST',
                                    data: data,
                                    success: function (vData) {
                                        $('input[name="filterGroupSales"]').checkboxradio('enable');
                                        if (isFilterByOwnType) {
                                            if (data.FilterOwnType.length > 0) {
                                                DisplayTotalGrossAndDiscountsByFOWST(vData.Results.Rows);
                                            }
                                            else {
                                                $('#reportTable').empty().append($('<tr>')
                                                                    .append($('<td>')
                                                                        .attr('colspan', '15')
                                                                        .css('text-align', 'center')
                                                                        .text('No Result(s)')
                                                                    )
                                                                )
                                            }
                                        }
                                        else {
                                            DisplayTotalGrossAndDiscountsByFOWST(vData.Results.Rows);
                                        }
                                    }
                                });
                            }

                            function DisplayTotalGrossAndDiscountsByFOWST(vRows) {
                                var pLv = $('#reportTable');
                                var tData;

                                var TotalNetSales = 0;
                                var TotalGrossSales = 0;
                                var TotalDiscounts = 0;
                                var TotalReturns = 0;

                                pLv.empty();

                                if (TempValueBtnBack == 'BackToDashboard') {
                                    for (i = 0; i <= vRows.length - 1; i++) {
                                        TotalNetSales += parseFloat(vRows[i].GrossSales) - parseFloat(vRows[i].TotalDiscounts) - (vRows[i].TotalReturns == undefined ? 0 : parseFloat(vRows[i].TotalReturns));
                                        TotalGrossSales += parseFloat(vRows[i].GrossSales);
                                        TotalDiscounts += parseFloat(vRows[i].TotalDiscounts);
                                        TotalReturns += (vRows[i].TotalReturns == undefined ? 0 : parseFloat(vRows[i].TotalReturns));
                                    }

                                    $('#lblGroupNetSales').html(AccountingFormat(TotalNetSales));
                                    $('#lblGroupGrossSales').html(AccountingFormat(TotalGrossSales));
                                    $('#lblGroupDiscounts').html(AccountingFormat(TotalDiscounts));
                                    $('#lblGroupExchanges').html(AccountingFormat(TotalReturns));
                                }

                                if (vRows.length > 0) {
                                    for (i = 0; i <= vRows.length - 1; i++) {
                                        //                                            var type;

                                        //                                            if (vRows[i].FranchiseeInvoiceNumber == undefined && vRows[i].PurchaseOrderNo == undefined) {
                                        //                                                type = "Company-Owned";
                                        //                                            }
                                        //                                            else {
                                        //                                                type = "Franchised";
                                        //                                            }

                                        tData = $('<tr>')
                                        .data('data', vRows[i])
                                        .append($('<td>')
                                            .attr('columnno', 1)
                                            .text(vRows[i].FOWSTName)
                                        )
                                        .append($('<td>')
                                            .attr('columnno', 2)
                                            .text(vRows[i].FOWSTAddress)
                                        )
                                        .append($('<td>')
                                            .attr('columnno', 3)
                                            .text(vRows[i].MallCode)
                                        )
                                        .append($('<td>')
                                            .attr('columnno', 4)
                                            .text(vRows[i].TenantNo)
                                        )
                                        .append($('<td>')
                                            .attr('columnno', 5)
                                            .text(vRows[i].ClassCode)
                                        )
                                        .append($('<td>')
                                            .attr('columnno', 6)
                                            .text(vRows[i].TradeCode)
                                        )
                                        .append($('<td>')
                                            .attr('columnno', 7)
                                            .text(vRows[i].OutletNo)
                                        )
                                        .append($('<td>')
                                            .attr('columnno', 8)
                                            .text(vRows[i].SaleType)
                                        )
                                        .append($('<td style="text-align:right">')
                                            .attr('columnno', 9)
                                            .text(AccountingFormat(parseFloat(vRows[i].GrossSales) - parseFloat(vRows[i].TotalDiscounts) - (vRows[i].TotalReturns == undefined ? 0 : parseFloat(vRows[i].TotalReturns))))
                                        )
                                        .append($('<td style="text-align:right">')
                                            .attr('columnno', 10)
                                            .text(AccountingFormat(((parseFloat(vRows[i].GrossSales) - parseFloat(vRows[i].TotalDiscounts) - CDec2(vRows[i].TotalReturns == 0 ? 0 : parseFloat(vRows[i].TotalReturns))) / CDec2($('#lblGroupNetSales').text())) * 100) + '%')
                                        )
                                        .append($('<td style="text-align:right">')
                                            .attr('columnno', 11)
                                            .text(AccountingFormat(vRows[i].GrossSales))
                                        )
                                        .append($('<td style="text-align:right">')
                                            .attr('columnno', 12)
                                            .text(AccountingFormat((CDec2(vRows[i].GrossSales) / CDec2($('#lblGroupGrossSales').text())) * 100) + '%')
                                        )
                                        .append($('<td style="text-align:right">')
                                            .attr('columnno', 13)
                                            .text('-')
                                        )
                                        .append($('<td style="text-align:right">')
                                            .attr('columnno', 14)
                                            .text('-')
                                        )
                                        .append($('<td style="text-align:right">')
                                            .attr('columnno', 15)
                                            .text(AccountingFormat(vRows[i].TotalReturns))
                                        )
                                        .append($('<td style="text-align:right">')
                                            .attr('columnno', 16)
                                            .text((CDec2(vRows[i].TotalReturns) / CDec2($('#lblGroupExchanges').text())) == 0 ? '-' : (AccountingFormat((CDec2(vRows[i].TotalReturns) / CDec2($('#lblGroupExchanges').text())) * 100) + '%'))
                                        )
                                        .append($('<br />'))
                                        .click(function () {
                                            //$('#panSalesTrends').popup('open');
                                            var vData;
                                            vData = {
                                                GroupBy: 'Day',
                                                Fowst: $(this).data('data').FOWSTID,
                                                From: data.From,
                                                To: data.To,
                                                IsRawData: 0,
                                                HasGroup: sessionStorage.IsShowFirstPage
                                            };

                                            //$('#dvSalesTrendFOWST').html($(this).data('data').FOWSTName)

                                            GetPOSSalesReportByTrendOrSalesDistribution(vData);

                                            $('#lnkTransactionDetails').css('visibility', 'visible');

                                            $('#lblSalesTrend').text('Sales data for ' + $(this).data('data').FOWSTName)

                                            window.location = '#lblSalesTrend';

                                            $('#dvSalesTrends').css('visibility', 'visible');
                                            $('#dvSalesTrends').css('display', 'block');
                                        })

                                        pLv.append(tData);
                                        //tData = $('<tr>')
                                        //    .append($('<td>')
                                        //        .text('')
                                        //    )
                                        //    .append($('<td style="text-align:right">')
                                        //        .text(AccountingFormat(((parseFloat(vRows[i].GrossSales) - parseFloat(vRows[i].TotalDiscounts) - CDec2(vRows[i].TotalReturns == 0 ? 0 : parseFloat(vRows[i].TotalReturns))) / CDec2($('#lblGroupNetSales').text())) * 100) + '%')
                                        //    )
                                        //    .append($('<td style="text-align:right">')
                                        //        .text(AccountingFormat((CDec2(vRows[i].GrossSales) / CDec2($('#lblGroupGrossSales').text())) * 100) + '%')
                                        //    )
                                        //    .append($('<td style="text-align:right">')
                                        //        .text((CDec2(vRows[i].TotalDiscounts) / CDec2($('#lblGroupDiscounts').text())) == 0 ? '-' : (AccountingFormat((CDec2(vRows[i].TotalDiscounts) / CDec2($('#lblGroupDiscounts').text())) * 100) + '%'))
                                        //    )
                                        //    .append($('<td style="text-align:right">')
                                        //        .text('-')
                                        //    )
                                        //    .append($('<td style="text-align:right">')
                                        //        .text((CDec2(vRows[i].TotalReturns) / CDec2($('#lblGroupExchanges').text())) == 0 ? '-' : (AccountingFormat((CDec2(vRows[i].TotalReturns) / CDec2($('#lblGroupExchanges').text())) * 100) + '%'))
                                        //    )
                                        //    .append($('<br />'))

                                        //pLv.append(tData);
                                    }
                                }
                                else {
                                    pLv.append($('<tr>')
                                        .append($('<td>')
                                            .attr('colspan', '16')
                                            .css('text-align', 'center')
                                            .text('No Result(s)')
                                        )
                                    )
                                }
                                $('#tblReport').table("refresh");
                            }

                            function GetFOWSTCount() {
                                AjaxSend({
                                    url: 'DashboardsAndReports/GetFOWSTCount',
                                    success: function (vData) {
                                        if (vData.Results.Rows.length > 0) {
                                            $('#TotalFowstCount').html(vData.Results.Rows[0].FOWSTCount)
                                        }
                                    }
                                });
                            }

                            function GetAllFOWST() {
                                if (OfflineMode) {
                                    //Sql.AttachDatabase(sessionStorage.OrgID);
                                    if (Web.HandleOrganization() == false)
                                        return;
                                    var vData = OfflineBrowser.LibraryFowst.GetData({
                                        FilterFowst: 1
                                    });
                                    DisplayFowst(vData);
                                }
                                else {
                                    AjaxSend({
                                        url: 'Browser/LibraryFowst',
                                        data: data,
                                        success: function (vData) {

                                            var vRows = vData.Results;
                                            DisplayFowst(vRows)
                                        }
                                    });
                                }
                            }

                            function DisplayFowst(vRows) {
                                var pLV = $('#selHeaderListOfFowstGroup');
                                pLV.empty();
                                for (var i = 0; i <= vRows.Rows.length - 1; i++) {
                                    pItem = $('<li>')
                                        .append($('<a>')
                                        .attr('ID', vRows.Rows[i].ID)
                                        .attr('class', 'btnLIListofFowstGroup')
                                            .click(function () {
                                                sessionStorage.SelectedFOWST = $(this).attr('id');
                                                sessionStorage.SelectedFOWSTName = $(this).text();
                                                $('.lblSelectedFowstGroup').text($(this).text());
                                                $('.btnSelectFowst').removeClass('btnRequired').addClass('btnNotRequired');
                                                $('#btnInventoryLedgerFowst').removeClass('btnRequired').addClass('btnNotRequired');
                                                $('#panHeaderListOfGroupFowst').popup('close');

                                                if (sessionStorage.Inventory == "balance" || sessionStorage.Inventory == "ledger") {
                                                    if ($('.lblSelectedInventoryType').text() == "All Inventory Types") {
                                                        $('#liViewAll').click();
                                                    }
                                                    else if ($('.lblSelectedInventoryType').text() == "Finished Goods") {
                                                        $('#liViewProducts').click();
                                                    }
                                                    else if ($('.lblSelectedInventoryType').text() == "Intermediate Goods and Raw Materials") {
                                                        $('#liViewRawMaterials').click();
                                                    }
                                                } else if (sessionStorage.ReportType == 'MoniesAccountability') {
                                                    //GetDepositReport(); //DepositReport.js
                                                    if ($.trim($('.lblPeriod:first').text()) == "2 Days Ago") {
                                                        $('#li2DaysAgo').click();
                                                    }
                                                    else if ($.trim($('.lblPeriod:first').text()) == "Yesterday") {
                                                        $('#liYesterday').click();
                                                    }
                                                    else if ($.trim($('.lblPeriod:first').text()) == "Today") {
                                                        $('#liToday').click();
                                                    }
                                                    else if ($.trim($('.lblPeriod:first').text()) == "Week To Date") {
                                                        $('#liWeekToDate').click();
                                                    }
                                                    else if ($.trim($('.lblPeriod:first').text()) == "Month To Date") {
                                                        $('#liMonthToDate').click();
                                                    }
                                                    else {
                                                        $('#btnDone').click();
                                                    }
                                                    $('#divTurnOverDetails').hide();
                                                } else {
                                                    //this is for cash handling, put else if to add other modules.
                                                    $('#btnReceivingTransferDone').click();
                                                }
                                            })
                                            .append($('<h3>')
                                                .text(vRows.Rows[i].FowstName)
                                            )
                                        )
                                    pLV.append(pItem);
                                }

                                try {
                                    pLV.listview('refresh');
                                }
                                catch (e) {
                                    pLV.listview();
                                }
                            }

                            //function GetOrganizationName() {
                            //    AjaxSend({
                            //        url: 'DashboardsAndReports/GetOrganizationName',
                            //        success: function (vData) {
                            //            if (vData.Results.Rows.length > 0) {
                            //                $('.OrganizationNameHeader').text(vData.Results.Rows[0].CompanyName)
                            //                $('#OrganizationName').html(vData.Results.Rows[0].CompanyNameInitial)
                            //            }
                            //        }
                            //    });
                            //}

                            function GetTotalSalesReturnsData() {
                                AjaxSend({
                                    url: 'DashboardsAndReports/GetTotalSalesReturnsData',
                                    data: data,
                                    success: function (vData) {
                                        if (vData.Results.Rows) {

                                        }

                                    }
                                });
                            }

                            function PageShow() {
                                //GetGroupToSelectMenu();

                                sessionStorage.OrgID = GetUrlParameters().OrgID;

                                if (ValidPrevPage(arguments)) {
                                    //if (CBool(sessionStorage.PrintingOfflineData)) {
                                    //    $('.OrganizationNameHeader').text(sessionStorage.OrgName);
                                    //    if (sessionStorage.OfflineType == 'cashier') {
                                    //        $('#divSalesAnalytics, #divOfflineTransactionHistoryMain, #divFranchiseRequest').hide();
                                    //        GetCashierSalesReportPage();
                                    //    }
                                    //    else if (sessionStorage.OfflineType == 'monies') {
                                    //        $('#divSalesAnalytics').show();
                                    //        $('#divOnPrintPOSSalesReport').hide();
                                    //        $('#divGroupSales').hide();
                                    //        $('#dvSalesTrends').hide();
                                    //        $('#divInventoryLevels').hide();
                                    //        $('#divInventoriesAcc').hide();
                                    //        $('#divFranchiseRequest').hide();
                                    //        $('.btnPeriod').show();

                                    //        AjaxSend({
                                    //            ExactUrl: '/CashHandling/DepositReport?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                                    //            type: 'GET',
                                    //            dataType: 'text',
                                    //            success: function (vData) {
                                    //                $('#divMoniesAcc').empty();
                                    //                $('#divMoniesAcc').append(vData).trigger('create');

                                    //                App.Deferred.fulfill('FQ26E5TP76');

                                    //                //setDashboardDate(m_Period);
                                    //                GetAllFOWST();
                                    //                sessionStorage.Inventory = ''; //clear this so that when you select fowst, it wont get balances/inventory.
                                    //                sessionStorage['ReportType'] = 'MoniesAccountability';
                                    //            }
                                    //        });

                                    //        $('#divMoniesAcc').show();

                                    //        $('#panReportTypeOption').popup('close');
                                    //        $('#txtReportType').text('Monies Accountability');
                                    //        $('.ItemsBreadCrumbs').text('Monies Accountability');
                                    //    }
                                    //    else if (sessionStorage.OfflineType == 'inventories') {
                                    //        $('#divSalesAnalytics').show();
                                    //        $('#divOnPrintPOSSalesReport').hide();
                                    //        $('#divGroupSales').hide();
                                    //        $('#dvSalesTrends').hide();
                                    //        $('#divInventoryLevels').hide();
                                    //        $('#divMoniesAcc').hide();
                                    //        $('#divFranchiseRequest').hide();
                                    //        $('.btnPeriod').hide();

                                    //        AjaxSend({
                                    //            ExactUrl: '/InventoryCount/Report?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                                    //            type: 'GET',
                                    //            dataType: 'text',
                                    //            success: function (vData) {
                                    //                $('#divInventoriesAcc').empty();
                                    //                $('#divInventoriesAcc').append(vData).trigger('create');
                                    //                isInventoryAccLoaded = true;
                                    //                App.Deferred.fulfill('FQ26E5TP76');
                                    //            }
                                    //        });

                                    //        if (isInventoryAccLoaded) {
                                    //            Report(); // InventoryCountReport.js
                                    //        }

                                    //        $('#divInventoriesAcc').show();

                                    //        $('#panReportTypeOption').popup('close');
                                    //        $('#txtReportType').text('Inventories Accountability');
                                    //        $('.ItemsBreadCrumbs').text('Inventories Accountability');
                                    //    }
                                    //    else if (sessionStorage.OfflineType == 'transaction') {
                                    //        $('#divSalesAnalytics, #divOfflineTransactionHistoryMain, #divFranchiseRequest').hide();
                                    //        ShowOfflineTransactionHistory();
                                    //    }
                                    //    else if (sessionStorage.OfflineType == 'inventoryledger') {
                                    //        $('#divSalesAnalytics, #divOfflineTransactionHistoryMain').hide();

                                    //        AjaxSend({
                                    //            ExactUrl: '/InventoryLedger?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=1',
                                    //            type: 'GET',
                                    //            dataType: 'text',
                                    //            success: function (vData) {
                                    //                sessionStorage["Inventory"] = "ledger";
                                    //                $('#divFranchiseRequest').empty();
                                    //                $('#divFranchiseRequest').append(vData).trigger('create');

                                    //                $('.mainContentForDashboard').css('margin-top', $('#divHeaderForDashboard').height());

                                    //                App.Deferred.fulfill('FQ26E5TP76');
                                    //            }
                                    //        });

                                    //        $('#divFranchiseRequest').show();
                                    //        $('#panReportTypeOption').popup('close');
                                    //        $('#txtReportType').text('Inventory Ledger');
                                    //    }
                                    //} else {
                                    var mdata = {
                                        OrgID: sessionStorage.OrgID
                                    }

                                    var m_Date = new Date();

                                    data = {
                                        GroupBy: 'All',
                                        FowstID: 'All',
                                        From: (m_Date.getMonth() + 1) + '/' + m_Date.getDate() + '/' + m_Date.getFullYear(), //change the month, copy the 'To' Month
                                        To: (m_Date.getMonth() + 1) + '/' + m_Date.getDate() + '/' + m_Date.getFullYear(),
                                        IsRawData: 0
                                    }

                                    GetTotalGrossAndDiscounts();
                                    data.GroupBy = 'ByOrganizationGroup';
                                    GetTotalGrossAndDiscountsByGROUP();
                                    GetTotalGrossAndDiscountsByFOWST();

                                    data.GraphType = ($('#txtViewType').text() == "Sales Trend" ? "Trend" : "Distribution");
                                    data.GroupBy = m_GroupBy;
                                    GetTotalForAllFowst(data); // TenantMonitoringSystemChart.js

                                    window.GetSearchData(data);
                                    //SliceData('Date');

                                    //GetTotalGrossAndDiscountsByFOWST();

                                    //GetOrganizationName();

                                    //AjaxSend({
                                    //    url: 'DashboardsAndReports/GetPageAccess',
                                    //    data: mdata,
                                    //    success: function (vData) {
                                    //        //if (CBool(vData.Results)) {
                                    //        //    sessionStorage.IsShowFirstPage = 'true'; //TODO
                                    //        //} else {
                                    //        //    sessionStorage.IsShowFirstPage = 'false';
                                    //            $('#divMainDashboard').hide();
                                    //            TempValueBtnBack = 'BackToDashboard';

                                    //            $('#divSalesAnalytics').fadeIn('fast', function () {
                                    //                var ns = $('#NetSalesHeight').height();
                                    //                var gs = $('#GrossSalesHeight').height();
                                    //                var et = $('#ExtrasTotalHeight').height();

                                    //                if ($(window).width() >= 576) {
                                    //                    if (ns > gs) {
                                    //                        $('#GrossSalesHeight, #ExtrasTotalHeight').css('height', ns);
                                    //                    } else if (ns < gs) {
                                    //                        $('#NetSalesHeight, #ExtrasTotalHeight').css('height', gs);
                                    //                    } else if (et > ns || et > gs) {
                                    //                        $('#NetSalesHeight, #GrossSalesHeight').css('height', et);
                                    //                    } else if (ns == gs) {
                                    //                        $('#NetSalesHeight, #ExtrasTotalHeight').css('height', gs);
                                    //                    }
                                    //                } else {
                                    //                    $('#NetSalesHeight, #GrossSalesHeight, #ExtrasTotalHeight').css('height', 'auto');
                                    //                }
                                    //            });
                                    //        //}
                                    //        //if (location.hash != '#RequestAndApprovals')
                                    //            //$('#panOption').popup('open');
                                    //            //$('#liToday').click();
                                    //    }
                                    //});

                                    //}
                                }

                                function ValidPrevPage(args) {
                                    try {
                                        return args[1].prevPage.attr('data-role') != 'dialog';
                                    } catch (e) {
                                        return true;
                                    }
                                    ;
                                }

                                ;
                            }

                            function ShowOfflineTransactionHistory() {
                                if (mDivOfflineTransactionHistory == null) {
                                    AjaxSend({
                                        ExactUrl: '/DashboardsAndReports/OfflineTransactionHistory?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=1',
                                        type: 'GET',
                                        dataType: 'text',
                                        success: function (vData) {
                                            mDivOfflineTransactionHistory = $('<div id="divOfflineTransactionHistoryMain">').appendTo('#divMainContentForDashboard');
                                            mDivOfflineTransactionHistory.append(vData).trigger('create');
                                            Show();
                                        }
                                    });
                                } else
                                    Show();

                                function Show() {
                                    $('#divMainDashboard, #divSalesAnalytics,' +
                                            '#divOnPrintPOSSalesReport,' +
                                            '#divSearchResult, #divSpecialReports, #divAuditTrail, #divSettings, #divSalesReplenishmentProcessing, #panTransactionDetails,#divFranchiseRequest').hide();

                                    $('#divSalesOrderProcessing').hide();
                                    $('#pageOrderStatus').hide();
                                    $('#pageOrderCentral').show();

                                    $('.ItemsBreadCrumbs').text('');

                                    //if (sessionStorage.PrintingOfflineData == "false") {
                                    breadcrumbs.prepend(arrowR, 'Offline Transaction History');
                                    $('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                    BreadCrumbsClickEvent();
                                    //}
                                    //else {
                                    //  $('.ItemsBreadCrumbs').text('Offline Transaction History');
                                    //}

                                    $('#panGoTo, #panOption').popup('close');
                                    if (mDivSalesReturns != null)
                                        mDivSalesReturns.hide();
                                    if (mDivOfflineProductHistory != null)
                                        mDivOfflineProductHistory.hide();
                                    mDivOfflineTransactionHistory.show();
                                }
                            }

                            function clearData() {
                                $('#TotalNetSales').text('-');
                                $('#TotalGrossSales').text('-');
                                $('#TotalDiscounts').text('-');
                                $('#TotalRefunds').text('-');
                                $('#TotalExchanges').text('-');
                                //$('#TotalFowstCount').text('-');

                                //2nd page
                                $('#lblGroupNetSales').text('-');
                                $('#lblGroupGrossSales').text('-');
                                $('#lblGroupDiscounts').text('-');
                                $('#lblGroupRefunds').text('-');
                                $('#lblGroupExchanges').text('-');

                                $('#reportTable').empty();
                                $('#MainFrame').empty();

                                ClearCharts();
                            }

                            function setDashboardDate(m_Period) {
                                var m_DateFrom = new Date();
                                var m_DateTo = new Date();

                                $('.fromCalendar').calendar('unsetDate');
                                $('.toCalendar').calendar('unsetDate');

                                switch (m_Period) {
                                    case "2DaysAgo":
                                        //back date from
                                        m_DateFrom.setDate(m_DateFrom.getDate() - 2);
                                        m_DateTo.setDate(m_DateTo.getDate() - 2);
                                        $("#selPOSSalesReportRange").val("Day").selectmenu('refresh');
                                        $("#selZReportRange").val("By Date").selectmenu('refresh');
                                        break;
                                    case "Yesterday":
                                        //back date from    
                                        m_DateFrom.setDate(m_DateFrom.getDate() - 1);
                                        m_DateTo.setDate(m_DateTo.getDate() - 1);
                                        $("#selPOSSalesReportRange").val("Yesterday").selectmenu('refresh');
                                        $("#selZReportRange").val("Yesterday").selectmenu('refresh');
                                        break;
                                    case "Today":
                                        //m_DateFrom and m_DateTo are date today
                                        $("#selPOSSalesReportRange").val("Today").selectmenu('refresh');
                                        $("#selZReportRange").val("Today").selectmenu('refresh');
                                        break;
                                    case "WeekToDate":
                                        m_DateFrom.setDate(m_DateFrom.getDate() - m_DateFrom.getDay());
                                        //m_DateTo is date today
                                        $("#selPOSSalesReportRange").val("Day").selectmenu('refresh');
                                        $("#selZReportRange").val("By Date").selectmenu('refresh');
                                        break;
                                    case "MonthToDate":
                                        GroupDateBy = 'Month';
                                        m_DateFrom.setDate(m_DateFrom.getDate() - (m_DateFrom.getDate() - 1));
                                        //m_DateTo is date today
                                        $("#selPOSSalesReportRange").val("Day").selectmenu('refresh');
                                        $("#selZReportRange").val("By Date").selectmenu('refresh');
                                        break;
                                    case "CustomPeriod":
                                        break;
                                    default:
                                        var newDateFrom = new Date($('#txtDateFrom').DateCombo('val'));
                                        var newDateTo = new Date($('#txtDateTo').DateCombo('val'));
                                        GroupDateBy = 'Month';
                                        m_DateFrom.setDate(newDateFrom.getMonth() + 1) + '/' + newDateFrom.getDate() + '/' + newDateFrom.getFullYear();
                                        m_DateTo.setDate(newDateTo.getMonth() + 1) + '/' + newDateTo.getDate() + '/' + newDateTo.getFullYear();
                                        $("#selPOSSalesReportRange").val("Day").selectmenu('refresh');
                                        $("#selZReportRange").val("By Date").selectmenu('refresh');
                                        break;
                                }

                                $('.fromCalendar').calendar('setDate', m_DateFrom);
                                $('.toCalendar').calendar('setDate', m_DateTo);

                                //for cashier and manager sales report

                                $('#selPOSSalesReportByDateFromMonth').val(m_DateFrom.getMonth() + 1).selectmenu('refresh');
                                $('#selPOSSalesReportByDateFromYear').val(m_DateFrom.getFullYear()).selectmenu('refresh');
                                setDayCollection('selPOSSalesReportByDateFromMonth', 'selPOSSalesReportByDateFromDay', 'selPOSSalesReportByDateFromYear', true);
                                $('#selPOSSalesReportByDateFromDay').val(m_DateFrom.getDate()).selectmenu('refresh');


                                $('#selPOSSalesReportByDateToMonth').val(m_DateTo.getMonth() + 1).selectmenu('refresh');
                                $('#selPOSSalesReportByDateToYear').val(m_DateTo.getFullYear()).selectmenu('refresh');
                                setDayCollection('selPOSSalesReportByDateToMonth', 'selPOSSalesReportByDateToDay', 'selPOSSalesReportByDateToYear', true);
                                $('#selPOSSalesReportByDateToDay').val(m_DateTo.getDate()).selectmenu('refresh');


                                //for z report

                                $('#selZReportByDateMonth').val(m_DateFrom.getMonth() + 1).selectmenu('refresh');
                                $('#selZReportByDateYear').val(m_DateFrom.getFullYear()).selectmenu('refresh');
                                setDayCollection('selZReportByDateMonth', 'selZReportByDateDay', 'selZReportByDateYear', true);
                                $('#selZReportByDateDay').val(m_DateFrom.getDate()).selectmenu('refresh');

                                //                    $('#txtDateFrom').DateCombo('val', m_DateFrom);
                                //                    $('#txtDateTo').DateCombo('val', m_DateTo);
                            }

                            window.GetTransactionDetails = function (mdata) {

                                mdata.IsRawData = 1;
                                GetSalesReturnsDetails(mdata);

                                AjaxSend({
                                    url: 'DashboardsAndReports/GetPOSSalesReportByTrendOrSalesDistribution',
                                    data: mdata,
                                    success: function (vData) {
                                        DisplayTransactionDetails(vData.ReportPOS.Rows);
                                        //m_SalesReturnData.IsSearch = 0;
                                    }
                                });
                            }

                            function DisplayTransactionDetails(vRows) {
                                var pLV = $('#tblTransactionDetailsBody');

                                $('#tblTransactionDetailsBody tr').empty();
                                //$('#lstTransactionDetails li').not(':eq(0)').not(':eq(0)').remove();

                                for (var i = 0; i <= vRows.length - 1; i++) {
                                    var hasReturned = { backgroundColor: '' };
                                    if (vRows[i].QuantityReturned != undefined) {
                                        hasReturned = { backgroundColor: '#FFFCB2' }
                                    }

                                    pItem = $('<tr>')
                                        .css(hasReturned)
                                        .append($('<td>')
                                            .text(vRows[i].InvoiceNo)
                                        )
                                        .append($('<td>')
                                            .text(vRows[i].InvoiceDate)
                                        )
                                        .append($('<td>')
                                            .text(vRows[i].ProductDescription)
                                        )
                                        .append($('<td style="text-align: center;">')
                                            .text(AccountingFormat(vRows[i].Qty))
                                        )
                                        .append($('<td>')
                                            .text(vRows[i].UnitMeasure)
                                        )
                                        .append($('<td>')
                                            .text(vRows[i].RebatesProduct)
                                        )
                                        .append($('<td>')
                                            .text(AccountingFormat((vRows[i].QtyRebates * vRows[i].Qty)))
                                        )
                                        .append($('<td>')
                                            .text(vRows[i].UnitMeasureRebates)
                                        )
                                        .append($('<td style="text-align: right;">')
                                            .text(AccountingFormat(vRows[i].Price))
                                        )
                                        .append($('<td style="text-align: right;">')
                                            .text(AccountingFormat(vRows[i].SubTotal))
                                        )
                                        .append($('<td style="text-align: right;">')
                                            .text(AccountingFormat(vRows[i].Discount))
                                        )
                                        .append($('<td>')
                                            .text(vRows[i].SalesRepresentative)
                                        )
                                        .append($('<td>')
                                            .text(vRows[i].Customer)
                                        )
                                        .append($('<td>')
                                            .text(vRows[i].UpdatedByName)
                                        );
                                    pLV.append(pItem);

                                }

                                //            try {
                                //                $('#lstFOWSTFrame').listview('refresh');
                                //            }
                                //            catch (e) {
                                //                $('#lstFOWSTFrame').listview();
                                //            }

                                //            $('#panTransactionDetails').popup('open');
                                $('#panTransactionDetails').show();


                                $('#divMainDashboard').hide();
                                $('#divSalesAnalytics,' +
                                    '#divOnPrintPOSSalesReport,' +
                                    '#divSearchResult, #divSpecialReports, #divAuditTrail, #divSettings, #divSalesOrderProcessing, #divSalesReplenishmentProcessing').hide();


                                $('.ItemsBreadCrumbs').text('');
                                breadcrumbs.prepend(arrowR, 'View Transaction Details');
                                $('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                BreadCrumbsClickEvent();
                                //            $('#panGoTo, #panOption').popup('close');
                                //                            

                            }

                            function GetSalesReturnsDetails(mdata) {

                                mdata.IsRawData = 1;

                                AjaxSend({
                                    url: 'DashboardsAndReports/GetSalesReturnsData',
                                    data: mdata,
                                    success: function (vData) {
                                        DisplaySalesReturnDetails(vData.Results.Rows);
                                    }
                                });
                            }

                            function DisplaySalesReturnDetails(vRows) {
                                var pLV = $('#tblSalesReturnDetailsBody')

                                $('#tblSalesReturnDetailsBody tr').empty();
                                //$('#lstTransactionDetails li').not(':eq(0)').not(':eq(0)').remove();

                                for (var i = 0; i <= vRows.length - 1; i++) {

                                    pItem = $('<tr>')
                                        .append($('<td>')
                                            .text(vRows[i].MemoNumber)
                                        )
                                        .append($('<td>')
                                            .text(vRows[i].MemoDate)
                                        )
                                        .append($('<td>')
                                            .text(vRows[i].SrvNum)
                                        )
                                        .append($('<td style="text-align: right;">')
                                            .text(AccountingFormat(vRows[i].Amount))
                                        )

                                    pLV.append(pItem);

                                }

                                $('#lstFOWSTFrame').listview('refresh');
                            }

                            function GetPageAccess() {
                                var mdata = {
                                    OrgID: sessionStorage.OrgID
                                }

                                var m_Date = new Date();

                                data = {
                                    GroupBy: 'All',
                                    FowstID: 'All',
                                    From: (m_Date.getMonth() + 1) + '/' + m_Date.getDate() + '/' + m_Date.getFullYear(), //change the month, copy the 'To' Month
                                    To: (m_Date.getMonth() + 1) + '/' + m_Date.getDate() + '/' + m_Date.getFullYear(),
                                    IsRawData: 0
                                }
                                //GetOrganizationName();
                                AjaxSend({
                                    url: 'DashboardsAndReports/GetPageAccess',
                                    data: mdata,
                                    success: function (vData) {
                                        //if (CBool(vData.Results)) {
                                        //    sessionStorage.IsShowFirstPage = 'true'; //TODO

                                        //    GetTotalGrossAndDiscounts();
                                        //    GetFOWSTCount();

                                        //    data.GroupBy = 'ByOrganizationGroup';
                                        //    GetTotalGrossAndDiscountsByGROUP();
                                        //    //GetPageAccess();
                                        //} else {
                                        sessionStorage.IsShowFirstPage = 'false';

                                        $('#divMainDashboard').hide();
                                        TempValueBtnBack = 'BackToDashboard';

                                        $('#divSalesAnalytics').fadeIn('fast', function () {
                                            var ns = $('#NetSalesHeight').height();
                                            var gs = $('#GrossSalesHeight').height();
                                            var et = $('#ExtrasTotalHeight').height();

                                            if ($(window).width() >= 576) {
                                                if (ns > gs) {
                                                    $('#GrossSalesHeight, #ExtrasTotalHeight').css('height', ns);
                                                } else if (ns < gs) {
                                                    $('#NetSalesHeight, #ExtrasTotalHeight').css('height', gs);
                                                } else if (et > ns || et > gs) {
                                                    $('#NetSalesHeight, #GrossSalesHeight').css('height', et);
                                                } else if (ns == gs) {
                                                    $('#NetSalesHeight, #ExtrasTotalHeight').css('height', gs);
                                                }
                                            } else {
                                                $('#NetSalesHeight, #GrossSalesHeight, #ExtrasTotalHeight').css('height', 'auto');
                                            }
                                        });

                                        GetTotalGrossAndDiscountsByFOWST();
                                        //}
                                    }
                                });
                            }
                        });
                });
        });
});
