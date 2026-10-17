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
        (OfflineMode ? '/Scripts3/Offline/Browser/LibraryFowst.js?crc=' : ''),
        'promise!/Scripts3/NewMessageBox.js?crc=' + assetversion
], function () {

    var mDivSalesReturns; // dynamically created inside divMainContentForDashboard
    var mDivOfflineTransactionHistory; // dynamically created inside divMainContentForDashboard
    var mDivOfflineProductHistory; // dynamically created inside divMainContentForDashboard
    var isInventoryAccLoaded = false;
    var isInventoryLedgerDoneClicked = false;
    var InventoryAccPageLoaded = '';
    var ctrCheckAllILAD = 0;

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
                            var viewtransreqdata;

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

                                if (location.pathname.toLowerCase() != '/erp' && !CBool(sessionStorage.PrintingOfflineData)) {
                                    AddBreadCrumb({
                                        text: 'Main Dashboard', Click: function () {
                                            $('#divMainDashboard').pageshow();
                                        }
                                    }, '.ItemsBreadCrumbs')
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
                                    "moveBlanksDesc": moveBlanksDesc,
                                    "accounting": function (a, b) {
                                        if (CDec2(a) < CDec2(b))
                                            return 1;
                                        if (CDec2(a) > CDec2(b))
                                            return -1;
                                        return 0;
                                    },
                                    "percentage": function (a, b) {
                                        var vA = a.replace('%', '');
                                        var vB = b.replace('%', '');
                                        if (CDec2(vA) < CDec2(vB))
                                            return 1;
                                        if (CDec2(vA) > CDec2(vB))
                                            return -1;
                                        return 0;
                                    }
                                });

                                table.on("beforetablesort", function (event, data) {
                                    // data.column - the index of the column sorted after a click
                                    // data.direction - the sorting direction (either asc or desc)
                                    $("#msg").text("Sorting index " + data.column)
                                    $(this).find('tbody').find('tr').each(function () {
                                        $(this).find("td:eq(" + data.column + ")").find('b').detach();
                                    })
                                });

                                table.on("aftertablesort", function (event, data) {

                                    var th = $(this).find("th");
                                    th.find(".arrow").remove();
                                    var dir = $.fn.stupidtable.dir;

                                    var arrow = data.direction === dir.ASC ? "&uarr;" : "&darr;";
                                    th.eq(data.column).append('<span class="arrow">' + arrow + '</span>');

                                    try {
                                        $(this).table();
                                    }
                                    catch (e) {
                                        $(this).table().table('refresh');
                                    }
                                });

                                $("tr").slice(1).click(function () {

                                    $(".awesome").removeClass("awesome");
                                    $(this).addClass("awesome");
                                });                                

                                var dateNow = new Date();

                                //            $('.btnSearch').click(function () {
                                //                var valtxtSearch = $('txtSearch');

                                //                if (ValueBtnBack.val() == "") {

                                //                }
                                //            })

                                window.McrDashboard = {
                                    LoadData: function () {
                                        ClickEvent();
                                        PageShow();                                        
                                    }
                                }

                                ClickEvent();

                                function ClickEvent() {
                                    //gibutang nko diri kay dili mogana ang click event sa eas pag 2nd or more call sa page - by arnie 8717
                                    // diri ibutang ang mga event sa mga html tags

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

                                        $('#divSalesOrderProcessing, #divRequestAndApprovals, #divCacMcr').hide();
                                        $('#panTransactionDetails').hide();
                                        $('#divMainDashboard').hide();
                                        $('#divSalesAnalytics').hide();
                                        $('#divSalesReplenishmentProcessing').hide();
                                        $('#divFranchiseRequest').hide();
                                        $('#divOnPrintPOSSalesReport').show();

                                        //$('.ItemsBreadCrumbs').text('');
                                        //breadcrumbs.prepend(arrowR, 'Physical Inventory and Variance Report');
                                        //$('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                        //BreadCrumbsClickEvent();
                                        AddBreadCrumb({
                                            UpdateLastchild: 1,
                                            text: 'Physical Inventory and Variance Report', Click: function () {
                                            }
                                        }, '.ItemsBreadCrumbs')

                                        $('#panOption').popup('close');
                                        $('#panGoTo').popup('close');
                                    })

                                    $('.btnInventoryLevel').click(function () {
                                        AjaxSend({
                                            ExactUrl: '/InventoryCount/ReportInventoryLevel?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                                            type: 'GET',
                                            dataType: 'text',
                                            success: function (vData) {
                                                $('#divOnPrintPOSSalesReport').empty();
                                                $('#divOnPrintPOSSalesReport').append(vData).trigger('create');
                                                App.Deferred.fulfill('FQ26E5TP76');
                                            }
                                        });

                                        $('#divSalesOrderProcessing, #divRequestAndApprovals').hide();
                                        $('#panTransactionDetails').hide();
                                        $('#divMainDashboard').hide();
                                        $('#divSalesAnalytics').hide();
                                        $('#divSalesReplenishmentProcessing').hide();
                                        $('#divFranchiseRequest, #divCacMcr').hide();
                                        $('#divOnPrintPOSSalesReport').show();

                                        //$('.ItemsBreadCrumbs').text('');
                                        //breadcrumbs.prepend(arrowR, 'Stock Inventory Levels');
                                        //$('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                        //BreadCrumbsClickEvent();
                                        AddBreadCrumb({
                                            UpdateLastchild: 1,
                                            text: 'Stock Inventory Levels', Click: function () {
                                            }
                                        }, '.ItemsBreadCrumbs')

                                        $('#panOption').popup('close');
                                        $('#panGoTo').popup('close');
                                    })

                                    $('.btnHome').click(function () {
                                        window.location = ValueBtnBack
                                    });

                                    $('.btnMoreReport').click(function () {
                                        if (!InAndroid) {
                                            $('#popPOSReportZReport').closest('li').eq(0).hide();                                          
                                        }
                                        if (sessionStorage.PrintingOfflineData == "false") {
                                            $('#panOption').popup('open');
                                        }
                                        else {
                                            $('#panGoToOffline').popup('open');
                                        }
                                    });

                                    $('.btnPeriod').click(function () {
                                        //$('#panDates').popup('open'); dili mogawas ang popup sa laing device . wala ko kabalo ngano - arnie 3-6-2017
                                        PopupPeriod({
                                            Click: function (e) {
                                                if (e.Period == '2 days ago') {
                                                    $('#li2DaysAgo').click();
                                                } else if (e.Period == 'Yesterday') {
                                                    $('#liYesterday').click();
                                                } else if (e.Period == 'Today') {
                                                    $('#liToday').click();
                                                } else if (e.Period == 'Week to Date') {
                                                    $('#liWeekToDate').click();
                                                } else if (e.Period == 'Month to Date') {
                                                    $('#liMonthToDate').click();
                                                } else if (e.Period == 'Custom') {
                                                    m_Period = 'CustomPeriod';
                                                    sessionStorage.Period = m_Period;
                                                    $('#txtDateFrom').text(e.From);
                                                    $('#txtDateTo').text(e.To);
                                                    $('#btnDone').click();
                                                }
                                            }
                                        })
                                    });

                                    $('#btnCancel').click(function () {
                                        $('#panCustomDates').popup('close');
                                    })

                                    $('#li2DaysAgo').unbind('click').click(function () {
                                        $('#panDates').popup('close');
                                        $('.lblPeriod').text($('#li2DaysAgo').text());
                                        m_Period = '2DaysAgo';
                                        sessionStorage.Period = m_Period;
                                        setDashboardDate(m_Period);
                                        GetData();
                                        if (sessionStorage.PrintingOfflineData == "false") {
                                            if (CBool(sessionStorage.SalesByOrg)) {
                                                GetTotalGrossAndDiscountsByOrg();
                                            }
                                            else {
                                                GetTotalGrossAndDiscountsByFOWST();
                                            }
                                        }
                                        $('#divTurnOverDetails').hide();
                                    });

                                    $('#liYesterday').unbind('click').click(function () {
                                        $('#panDates').popup('close');
                                        $('.lblPeriod').text($('#liYesterday').text());
                                        m_Period = 'Yesterday';
                                        sessionStorage.Period = m_Period;
                                        setDashboardDate(m_Period);
                                        GetData();
                                        if (sessionStorage.PrintingOfflineData == "false") {
                                            if (CBool(sessionStorage.SalesByOrg)) {
                                                GetTotalGrossAndDiscountsByOrg();
                                            }
                                            else {
                                                GetTotalGrossAndDiscountsByFOWST();
                                            }
                                        }
                                        $('#divTurnOverDetails').hide();
                                    });

                                    $('#liToday').unbind('click').click(function () {
                                        $('#panDates').popup('close');
                                        $('.lblPeriod').text($('#liToday').text());
                                        m_Period = 'Today';
                                        sessionStorage.Period = m_Period;
                                        setDashboardDate(m_Period);
                                        GetData();
                                        if (sessionStorage.PrintingOfflineData == "false") {
                                            if (CBool(sessionStorage.SalesByOrg)) {
                                                GetTotalGrossAndDiscountsByOrg();
                                            }
                                            else {
                                                GetTotalGrossAndDiscountsByFOWST();
                                            }
                                        }
                                        $('#divTurnOverDetails').hide();
                                    });

                                    $('#liWeekToDate').unbind('click').click(function () {
                                        $('#panDates').popup('close');
                                        $('.lblPeriod').text($('#liWeekToDate').text());
                                        m_Period = 'WeekToDate';
                                        sessionStorage.Period = m_Period;
                                        setDashboardDate(m_Period);
                                        GetData();
                                        if (sessionStorage.PrintingOfflineData == "false") {
                                            if (CBool(sessionStorage.SalesByOrg)) {
                                                GetTotalGrossAndDiscountsByOrg();
                                            }
                                            else {
                                                GetTotalGrossAndDiscountsByFOWST();
                                            }
                                        }
                                        $('#divTurnOverDetails').hide();
                                    });

                                    $('#liMonthToDate').unbind('click').click(function () {
                                        $('#panDates').popup('close');
                                        $('.lblPeriod').text($('#liMonthToDate').text());
                                        m_Period = 'MonthToDate';
                                        sessionStorage.Period = m_Period;
                                        setDashboardDate(m_Period);
                                        GetData();
                                        if (sessionStorage.PrintingOfflineData == "false") {
                                            if (CBool(sessionStorage.SalesByOrg)) {
                                                GetTotalGrossAndDiscountsByOrg();
                                            }
                                            else {
                                                GetTotalGrossAndDiscountsByFOWST();
                                            }
                                        }
                                        $('#divTurnOverDetails').hide();
                                    });

                                    $('#liCustomPeriod').unbind('click').click(function () {
                                        $('#panDates').popup('close');
                                        $('#panCustomDates').popup('open');
                                        m_Period = 'CustomPeriod';
                                        sessionStorage.Period = m_Period;
                                    });

                                    $('#btnDone').unbind('click').click(function () {
                                        $('.lblPeriod').text($.format.date(new Date($('#txtDateFrom').text()), 'MMM dd, yyyy') + " to " + $.format.date(new Date($('#txtDateTo').text()), 'MMM dd, yyyy'));
                                        GetData();
                                        if (sessionStorage.PrintingOfflineData == "false") {
                                            if (CBool(sessionStorage.SalesByOrg)) {
                                                GetTotalGrossAndDiscountsByOrg();
                                            }
                                            else {
                                                GetTotalGrossAndDiscountsByFOWST();
                                            }
                                        }
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

                                    var isLoadCac = false;
                                    $('.btnCreditAndCollectionPage').click(function () {
                                        AjaxSend({
                                            url: 'Login3/TestCookies',
                                            success: function (vData) {
                                                PromptUserAccess('Credit and Collection Dashboard', {
                                                    OnClickOKSuccess: function () {
                                                        $('#divCacMcr').pageshow();
                                                        $('#panOption').popup('close');
                                                        if (!isLoadCac) {
                                                            require(['promise!/Scripts/sources/ERP/OrderProcessing/dashboard/controller.js?v=' + assetversion], function () {
                                                                $('#divCacMcr').html('Loading ...')
                                                                AjaxSend({
                                                                    ExactUrl: '/CreditAndCollection?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                                                                    type: 'GET',
                                                                    dataType: 'text',
                                                                    success: function (vData) {
                                                                        $('#divCacMcr').html('');
                                                                        $('#divCacMcr').append(vData).trigger('create');
                                                                        isLoadCac = true;
                                                                        $('#tabCacDashboard, #tabCacSettings').hidetabmenu();
                                                                        $('#tabCacWork').opentabmenu();
                                                                        $('#divCacMcrFowst').show();
                                                                    }
                                                                });
                                                            });
                                                        }
                                                        //$('.ItemsBreadCrumbs').text('');
                                                        //breadcrumbs.prepend(arrowR, 'Credit and Collection Dashboard');
                                                        //$('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                                        //BreadCrumbsClickEvent();
                                                        AddBreadCrumb({
                                                            UpdateLastchild: 1,
                                                            text: 'Credit and Collection Dashboard', Click: function () {
                                                            }
                                                        }, '.ItemsBreadCrumbs')
                                                    }
                                                });

                                            },
                                            error: function (vError) {
                                                NewMessageBox("This feature requires internet connection.",
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
                                    })

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
                                                        $('#divMainDashboard, #divRequestAndApprovals, #divCacMcr').hide();
                                                        $('#divSalesAnalytics,' +
                                                            '#divOnPrintPOSSalesReport,' +
                                                            '#divSearchResult, #divSpecialReports, #divAuditTrail, #divSettings, #divSalesOrderProcessing, #panTransactionDetails, #divFranchiseRequest').hide();

                                                        $('#divSalesReplenishmentProcessing').show();

                                                        $('#divFranchiseeOrders').show();
                                                        $('#divProductBreakdown').hide();
                                                        $('#divSalesBreakdown').hide();
                                                        $('#divFranchiseeLastReceived').hide();


                                                        //$('.ItemsBreadCrumbs').text('');
                                                        //breadcrumbs.prepend(arrowR, 'Store Replenishment Processing & Analytics');
                                                        //$('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                                        //BreadCrumbsClickEvent();
                                                        AddBreadCrumb({
                                                            UpdateLastchild: 1,
                                                            text: 'Store Replenishment Processing & Analytics', Click: function () {
                                                            }
                                                        }, '.ItemsBreadCrumbs')
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
                                                PromptUserAccess('Access Order Fulfillment Module*', {
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

                                                        $('#divMainDashboard, #divSalesAnalytics, #divRequestAndApprovals, #divCacMcr,' +
                                                            '#divOnPrintPOSSalesReport,' +
                                                            '#divSearchResult, #divSpecialReports, #divAuditTrail, #divSettings, #divSalesReplenishmentProcessing, #panTransactionDetails,#divFranchiseRequest').hide();

                                                        $('#divSalesOrderProcessing').show();
                                                        $('#pageOrderStatus').hide();
                                                        $('#pageOrderCentral').show();

                                                        //$('.ItemsBreadCrumbs').text('');
                                                        //breadcrumbs.prepend(arrowR, 'Sales Order Processing & Analytics');
                                                        //$('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                                        //BreadCrumbsClickEvent();
                                                        AddBreadCrumb({
                                                            UpdateLastchild: 1,
                                                            text: 'Sales Order Processing & Analytics', Click: function () {
                                                            }
                                                        }, '.ItemsBreadCrumbs')

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

                                    $('#liOfflineTransactionHistory').click(function (e) {
                                        ShowOfflineTransactionHistory();
                                    });

                                    $('#liOfflineProductHistory').click(function (e) {
                                        ShowOfflineProductHistory();
                                    });

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
                                        GetCashierSalesReportPage(0);
                                    });

                                    $('#popPOSReportManagerReport').click(function (e) {
                                        e.preventDefault();
                                        AjaxSend({
                                            url: 'Login3/TestCookies',
                                            success: function (vData) {

                                                AccessForInventoryOfPayment('Manager Sales Report', function () {
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
                                                })
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


                                        $('#divMainDashboard, #divRequestAndApprovals, #divCacMcr').hide();
                                        $('#divSalesAnalytics').hide();
                                        $('#divSalesReplenishmentProcessing').hide();
                                        $('#divFranchiseRequest').hide();
                                        $('#divSalesOrderProcessing').hide();
                                        $('#panTransactionDetails').hide();

                                        $('#divOnPrintPOSSalesReport').show();

                                        //$('.ItemsBreadCrumbs').text('');
                                        //breadcrumbs.prepend(arrowR, 'Manager\'s Sales Report');
                                        //$('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                        //BreadCrumbsClickEvent();

                                        AddBreadCrumb({
                                            UpdateLastchild: 1,
                                            text: 'Manager\'s Sales Report', Click: function () {
                                            }
                                        }, '.ItemsBreadCrumbs')

                                        //TempValueBtnBack = "BackToDashboard";

                                        $('#panOption').popup('close');
                                    });

                                    $('#popPOSReportSKUSalesReport').click(function (e) {

                                        e.preventDefault();

                                        AjaxSend({
                                            url: 'Login3/TestCookies',
                                            success: function (vData) {

                                                AjaxSend({
                                                    ExactUrl: '/ReportsPOS/SKUUnitSalesReport?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
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

                                                //PromptUserAccess('Z Report', {
                                                //    OnClickOKSuccess: function () {

                                                //        AjaxSend({
                                                //            ExactUrl: '/ReportsPOS/SKUUnitSalesReport?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                                                //            type: 'GET',
                                                //            dataType: 'text',
                                                //            success: function (vData) {

                                                //                $('#divOnPrintPOSSalesReport').empty();
                                                //                $('#divOnPrintPOSSalesReport').append(vData).trigger('create');

                                                //                App.Deferred.fulfill('FQ26E5TP76');

                                                //                setMonthCollection("selZReportByDateMonth");
                                                //                setYearCollection("selZReportByDateYear");

                                                //                setDashboardDate(m_Period);
                                                //            }
                                                //        });
                                                //    }
                                                //});
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

                                        $('#divMainDashboard, #divRequestAndApprovals, #divCacMcr').hide();
                                        $('#divSalesAnalytics').hide();
                                        $('#divSalesReplenishmentProcessing').hide();
                                        $('#divFranchiseRequest').hide();
                                        $('#divSalesOrderProcessing').hide();
                                        $('#panTransactionDetails').hide();

                                        $('#divOnPrintPOSSalesReport').show();

                                        //$('.ItemsBreadCrumbs').text('');
                                        //breadcrumbs.prepend(arrowR, 'SKU Unit Sales Report');
                                        //$('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                        //BreadCrumbsClickEvent();

                                        AddBreadCrumb({
                                            UpdateLastchild: 1,
                                            text: 'SKU Unit Sales Report', Click: function () {
                                            }
                                        }, '.ItemsBreadCrumbs')

                                        $('#panOption').popup('close');
                                    })

                                    $('#popPOSReportZReport').click(function (e) {
                                        e.preventDefault();

                                        AjaxSend({
                                            url: 'Login3/TestCookies',
                                            success: function (vData) {

                                                AccessForInventoryOfPayment('Z Report', function () {
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
                                                })
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

                                        $('#divMainDashboard, #divRequestAndApprovals, #divCacMcr').hide();
                                        $('#divSalesAnalytics').hide();
                                        $('#divSalesReplenishmentProcessing').hide();
                                        $('#divFranchiseRequest').hide();
                                        $('#divSalesOrderProcessing').hide();
                                        $('#panTransactionDetails').hide();

                                        $('#divOnPrintPOSSalesReport').show();

                                        //$('.ItemsBreadCrumbs').text('');
                                        //breadcrumbs.prepend(arrowR, 'Z Report');
                                        //$('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                        //BreadCrumbsClickEvent();

                                        AddBreadCrumb({
                                            UpdateLastchild: 1,
                                            text: 'Z Report', Click: function () {
                                            }
                                        }, '.ItemsBreadCrumbs')

                                        //TempValueBtnBack = "BackToDashboard";

                                        $('#panOption').popup('close');
                                    });

                                    $('#popPOSReportEJournal').click(function (e) {
                                        e.preventDefault();

                                        AjaxSend({
                                            url: 'Login3/TestCookies',
                                            success: function (vData) {

                                                //PromptUserAccess('E Journal', {
                                                //    OnClickOKSuccess: function () {

                                                AjaxSend({
                                                    ExactUrl: '/ReportsPOS/EJournal?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
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
                                                //    }
                                                //});
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

                                        $('#divMainDashboard, #divRequestAndApprovals, #divCacMcr').hide();
                                        $('#divSalesAnalytics').hide();
                                        $('#divSalesReplenishmentProcessing').hide();
                                        $('#divFranchiseRequest').hide();
                                        $('#divSalesOrderProcessing').hide();
                                        $('#panTransactionDetails').hide();

                                        $('#divOnPrintPOSSalesReport').show();

                                        //$('.ItemsBreadCrumbs').text('');
                                        //breadcrumbs.prepend(arrowR, 'E Journal');
                                        //$('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                        //BreadCrumbsClickEvent();

                                        AddBreadCrumb({
                                            UpdateLastchild: 1,
                                            text: 'E Journal', Click: function () {
                                            }
                                        }, '.ItemsBreadCrumbs')

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
                                                        sessionStorage.ReportType = 'DepositReport';
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

                                                                sessionStorage.Inventory = ''; //clear this so that when you select fowst, it wont get balances/inventory.
                                                                //sessionStorage['ReportType'] = 'MoniesAccountability';

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

                                        $('#divMainDashboard, #divRequestAndApprovals, #divCacMcr').hide();
                                        $('#divSalesAnalytics').hide();
                                        $('#divSalesReplenishmentProcessing').hide();
                                        $('#divFranchiseRequest').hide();
                                        $('#divSalesOrderProcessing').hide();
                                        $('#panTransactionDetails').hide();

                                        $('#divOnPrintPOSSalesReport').show();

                                        //$('.ItemsBreadCrumbs').text('');
                                        //breadcrumbs.prepend(arrowR, 'Deposits Report');
                                        //$('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                        //BreadCrumbsClickEvent();

                                        AddBreadCrumb({
                                            UpdateLastchild: 1,
                                            text: 'Deposits Report', Click: function () {
                                            }
                                        }, '.ItemsBreadCrumbs')

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

                                    $('#popPOSReportSCDAndPWDReports').click(function () {

                                        AjaxSend({
                                            url: 'Login3/TestCookies',
                                            success: function (vData) {

                                                var pData = {};
                                                pData.OrgID = sessionStorage["OrgID"];
                                                pData.POSSalesReportType = 0;

                                                AjaxSend({
                                                    ExactUrl: '/SCDAndPWDReports?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
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


                                            },
                                            error: function (vError) {
                                              
                                            }
                                        })

                                        $('#divMainDashboard, #divRequestAndApprovals, #divCacMcr').hide();
                                        $('#divSalesAnalytics').hide();
                                        $('#divSalesReplenishmentProcessing').hide();
                                        $('#divFranchiseRequest').hide();
                                        $('#divSalesOrderProcessing').hide();
                                        $('#panTransactionDetails').hide();

                                        $('#divOnPrintPOSSalesReport').show();

                                        //$('.ItemsBreadCrumbs').text('');

                                        if (CBool(sessionStorage.PrintingOfflineData)) {
                                            //$('.ItemsBreadCrumbs').text('Inventory Form Report');
                                            //RemoveBreadCrumb('Main Dashboard', '.ItemsBreadCrumbs');
                                        }
                                        else {
                                            //breadcrumbs.prepend(arrowR, 'Inventory Form Report');
                                            //$('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                            //BreadCrumbsClickEvent();
                                        }

                                        AddBreadCrumb({
                                            UpdateLastchild: 1,
                                            text: 'Senior Citizen And Person With Disability Discount Report', Click: function () {
                                            }
                                        }, '.ItemsBreadCrumbs')

                                        //TempValueBtnBack = "BackToDashboard";

                                        $('#panOption').popup('close');
                                    })

                                    $('#popPOSSRVHistory').click(function () {

                                        AjaxSend({
                                            url: 'Login3/TestCookies',
                                            success: function (vData) {

                                                var pData = {};
                                                pData.OrgID = sessionStorage["OrgID"];
                                                pData.POSSalesReportType = 0;

                                                AjaxSend({
                                                    ExactUrl: '/SalesReturns/SRVHistory?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
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


                                            },
                                            error: function (vError) {

                                            }
                                        })

                                        $('#divMainDashboard, #divRequestAndApprovals, #divCacMcr').hide();
                                        $('#divSalesAnalytics').hide();
                                        $('#divSalesReplenishmentProcessing').hide();
                                        $('#divFranchiseRequest').hide();
                                        $('#divSalesOrderProcessing').hide();
                                        $('#panTransactionDetails').hide();

                                        $('#divOnPrintPOSSalesReport').show();

                                        if (CBool(sessionStorage.PrintingOfflineData)) {
                                            
                                            //RemoveBreadCrumb('Main Dashboard', '.ItemsBreadCrumbs');
                                        }
                                        else {
                                          
                                        }

                                        AddBreadCrumb({
                                            UpdateLastchild: 1,
                                            text: 'SRV History', Click: function () {
                                            }
                                        }, '.ItemsBreadCrumbs')

                                        $('#panOption').popup('close');
                                    });

                                    $('#popPOSReportInventoryForm').click(function (e) {

                                        //if (CBool(sessionStorage.PrintingOfflineData)) {
                                        AjaxSend({
                                            url: 'Login3/TestCookies',
                                            success: function (vData) {

                                                var pData = {};
                                                pData.OrgID = sessionStorage["OrgID"];
                                                pData.POSSalesReportType = 0;

                                                AjaxSend({
                                                    ExactUrl: '/InventoryForm?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
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


                                            },
                                            error: function (vError) {
                                                //PromptUserAccess('Cashier Sales Report', {
                                                //    OnClickOKSuccess: function () {
                                                //        AjaxSend({
                                                //            ExactUrl: '/ReportsPOS/SalesReport?cache=1&OrgID=' + sessionStorage.OrgID + '&IsManagerSalesReport=0&IsMobile=' + IsMobile,
                                                //            type: 'GET',
                                                //            dataType: 'text',
                                                //            success: function (vData) {
                                                //                $('#divOnPrintPOSSalesReport').empty();
                                                //                $('#divOnPrintPOSSalesReport').append(vData).trigger('create');

                                                //                App.Deferred.fulfill('FQ26E5TP76');

                                                //                setMonthCollection("selPOSSalesReportByDateFromMonth");
                                                //                setYearCollection("selPOSSalesReportByDateFromYear");

                                                //                setMonthCollection("selPOSSalesReportByDateToMonth");
                                                //                setYearCollection("selPOSSalesReportByDateToYear");

                                                //                setDashboardDate(m_Period);

                                                //                $('.btnSelectFowst').show();
                                                //            }
                                                //        });
                                                //    }
                                                //});
                                            }
                                        });

                                        //CounterCashierSalesReport = false;
                                        //} else {
                                        //    PromptUserAccess('Cashier Sales Report', {
                                        //        OnClickOKSuccess: function () {
                                        //            var isPageReady = false;
                                        //            var pData = {};
                                        //            pData.OrgID = sessionStorage["OrgID"];
                                        //            pData.POSSalesReportType = 0;

                                        //            AjaxSend({
                                        //                url: 'ReportsPOS/setPOSSalesReportType',
                                        //                data: pData,
                                        //                success: function (vData) {
                                        //                    if (cashiermanagerpage == "") {
                                        //                        AjaxSend({
                                        //                            ExactUrl: '/ReportsPOS/SalesReport?cache=1&OrgID=' + sessionStorage.OrgID + '&IsManagerSalesReport=0&IsMobile=' + IsMobile,
                                        //                            type: 'GET',
                                        //                            dataType: 'text',
                                        //                            success: function (vData) {
                                        //                                isPageReady = true;
                                        //                                cashiermanagerpage = vData;
                                        //                                $('#divOnPrintPOSSalesReport').empty();
                                        //                                $('#divOnPrintPOSSalesReport').append(vData).trigger('create');

                                        //                                App.Deferred.fulfill('FQ26E5TP76');

                                        //                                setMonthCollection("selPOSSalesReportByDateFromMonth");
                                        //                                setYearCollection("selPOSSalesReportByDateFromYear");

                                        //                                setMonthCollection("selPOSSalesReportByDateToMonth");
                                        //                                setYearCollection("selPOSSalesReportByDateToYear");

                                        //                                setDashboardDate(m_Period);

                                        //                                $('.btnSelectFowst').show();
                                        //                            }
                                        //                        });
                                        //                    }
                                        //                    else {
                                        //                        $('#divOnPrintPOSSalesReport').empty();
                                        //                        $('#divOnPrintPOSSalesReport').append(cashiermanagerpage).trigger('create');
                                        //                        SalesReport.GetAllFowstinSalesReportPage();
                                        //                    }
                                        //                }
                                        //            });
                                        //        }
                                        //    });
                                        //}


                                        $('#divMainDashboard, #divRequestAndApprovals, #divCacMcr').hide();
                                        $('#divSalesAnalytics').hide();
                                        $('#divSalesReplenishmentProcessing').hide();
                                        $('#divFranchiseRequest').hide();
                                        $('#divSalesOrderProcessing').hide();
                                        $('#panTransactionDetails').hide();

                                        $('#divOnPrintPOSSalesReport').show();

                                        //$('.ItemsBreadCrumbs').text('');

                                        if (CBool(sessionStorage.PrintingOfflineData)) {
                                            //$('.ItemsBreadCrumbs').text('Inventory Form Report');
                                            //RemoveBreadCrumb('Main Dashboard', '.ItemsBreadCrumbs');
                                        }
                                        else {
                                            //breadcrumbs.prepend(arrowR, 'Inventory Form Report');
                                            //$('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                            //BreadCrumbsClickEvent();
                                        }

                                        AddBreadCrumb({
                                            UpdateLastchild: 1,
                                            text: 'Inventory Form Report', Click: function () {
                                            }
                                        }, '.ItemsBreadCrumbs')

                                        //TempValueBtnBack = "BackToDashboard";

                                        $('#panOption').popup('close');
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
                                                        $('.btnMoreOptionInventoryLedger').click();
                                                        //window.open(GetPathFormat("/InventoryLedger"), "_self");
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

                                                        //$('.ItemsBreadCrumbs').text('');
                                                        //breadcrumbs.prepend(arrowR, 'Franchise Requests');
                                                        //$('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                                        //BreadCrumbsClickEvent();
                                                        AddBreadCrumb({
                                                            UpdateLastchild: 1,
                                                            text: 'Franchise Requests', Click: function () {
                                                            }
                                                        }, '.ItemsBreadCrumbs')
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

                                                PromptUserAccess('Inventory Ledger', {
                                                    OnClickOKSuccess: function () {
                                                        if (location.hash == '#&ui-state=dialog')
                                                            history.back();

                                                        sessionStorage.InventoryBalanceProductName = '';

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
                                                        $('#divSalesReplenishmentProcessing, #divRequestAndApprovals, #divCacMcr').hide();

                                                        $('#divMainDashboard, #divSalesAnalytics, #divOnPrintPOSSalesReport').hide();
                                                        $('#divFranchiseRequest').show();
                                                        $('#divSalesOrderProcessing').hide();
                                                        $('#panTransactionDetails').hide();
                                                        $('#divRequestAndApprovals').hide();

                                                        $('#pageOrderStatus').hide();
                                                        $('#pageOrderCentral').hide();

                                                        //$('.ItemsBreadCrumbs').text('');
                                                        //breadcrumbs.prepend(arrowR, 'Inventory Ledger');
                                                        //$('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                                        //BreadCrumbsClickEvent();
                                                        AddBreadCrumb({
                                                            UpdateLastchild: 1,
                                                            text: 'Inventory Ledger', Click: function () {
                                                            }
                                                        }, '.ItemsBreadCrumbs')
                                                        $('#panGoTo, #panOption').popup('close');
                                                        $('.btnSelectFowst, #divBtnPeriod').show();
                                                        //GetAllFOWST()

                                                        $('.btnSelectFowst').show();

                                                        if (!CBool(sessionStorage.IsFromInventoryBalance)) {
                                                            //$('.btnSelectFowst').click();
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

                                    var htmlinventoryledger = '';
                                    window.InventoryLedger = {
                                        Page: function (callback) {
                                            if (htmlinventoryledger == '') {
                                                AjaxSend({
                                                    ExactUrl: '/InventoryLedger?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=1',
                                                    type: 'GET',
                                                    dataType: 'text',
                                                    success: function (vData) {
                                                        htmlinventoryledger = vData;
                                                        callback(htmlinventoryledger, false);
                                                    }
                                                });
                                                //CounterFranchiseRequest = false;

                                            } else {
                                                callback(htmlinventoryledger, true);
                                            }
                                        }
                                    }

                                    var RecTransPage = '';
                                    $('.btnMoreOptionStockReceivingAndTransfers').click(function (e) {
                                        AjaxSend({
                                            url: 'Login3/TestCookies',
                                            success: function (vData) {

                                                PromptUserAccess('Stock Receiving and Transfers', {
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
                                                        $('#divSalesReplenishmentProcessing, #divRequestAndApprovals, #divCacMcr').hide();

                                                        $('#divMainDashboard, #divSalesAnalytics, #divOnPrintPOSSalesReport').hide();
                                                        $('#divFranchiseRequest').show();
                                                        $('#divSalesOrderProcessing').hide();
                                                        $('#panTransactionDetails').hide();

                                                        $('#pageOrderStatus').hide();
                                                        $('#pageOrderCentral').hide();

                                                        //$('.ItemsBreadCrumbs').text('');
                                                        //breadcrumbs.prepend(arrowR, 'Stock Receiving and Transfers');
                                                        //$('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                                        //BreadCrumbsClickEvent();
                                                        AddBreadCrumb({
                                                            UpdateLastchild: 1,
                                                            text: 'Stock Receiving and Transfers', Click: function () {
                                                            }
                                                        }, '.ItemsBreadCrumbs')
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
                                        //$('.ItemsBreadCrumbs').text('');
                                        //breadcrumbs.prepend(arrowR, 'Inventory Balances');
                                        //$('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                        //BreadCrumbsClickEvent();
                                        AddBreadCrumb({
                                            UpdateLastchild: 1,
                                            text: 'Inventory Balances', Click: function () {
                                            }
                                        }, '.ItemsBreadCrumbs')
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

                                                PromptUserAccess('Inventory Balance', {
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

                                                                    //Used for "Group Inventory Level" only
                                                                    if (sessionStorage.IsFromPrevTab) {
                                                                        $('.lblSelectedFowstGroup').text(GetUrlParameters().Name);
                                                                        $('.btnSelectFowst').removeClass('btnRequired').addClass('btnNotRequired');
                                                                        $('#btnInventoryBalanceOptions').removeClass('btnRequired').addClass('btnNotRequired');
                                                                    }
                                                                    sessionStorage.IsFromPrevTab = false;
                                                                    App.Deferred.fulfill('FQ26E5TP76');
                                                                }
                                                            });
                                                            //CounterFranchiseRequest = false;

                                                        } else {
                                                            //Do Nothing Here
                                                        }
                                                        $('#divSalesReplenishmentProcessing, #divRequestAndApprovals, #divCacMcr').hide();

                                                        $('#divMainDashboard, #divSalesAnalytics, #divOnPrintPOSSalesReport').hide();
                                                        $('#divFranchiseRequest').show();
                                                        $('#divSalesOrderProcessing').hide();
                                                        $('#panTransactionDetails').hide();

                                                        $('#pageOrderStatus').hide();
                                                        $('#pageOrderCentral').hide();
                                                        $('#divBtnPeriod').hide();

                                                        //$('.ItemsBreadCrumbs').text('');
                                                        //breadcrumbs.prepend(arrowR, 'Inventory Balance');
                                                        //$('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                                        //BreadCrumbsClickEvent();
                                                        AddBreadCrumb({
                                                            UpdateLastchild: 1,
                                                            text: 'Inventory Balance', Click: function () {
                                                            }
                                                        }, '.ItemsBreadCrumbs')
                                                        $('#panGoTo, #panOption').popup('close');

                                                        //GetAllFOWST()
                                                        $('.btnSelectFowst').show();
                                                        //$('.btnSelectFowst').click();
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
                                                $('#divDateDepositReport').hide();
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

                                        if (InventoryAccPageLoaded == '') {
                                            AjaxSend({
                                                ExactUrl: '/InventoryCount/Report?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                                                type: 'GET',
                                                dataType: 'text',
                                                success: function (vData) {
                                                    $('#divInventoriesAcc').empty();
                                                    $('#divInventoriesAcc').append(vData).trigger('create');
                                                    isInventoryAccLoaded = true;
                                                    InventoryAccPageLoaded = vData;
                                                    App.Deferred.fulfill('FQ26E5TP76');
                                                }
                                            });
                                        }
                                        else {
                                            $('#divInventoriesAcc').empty();
                                            $('#divInventoriesAcc').append(InventoryAccPageLoaded).trigger('create');
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
                                        if (CBool(sessionStorage.SalesByOrg)) {
                                            GetTotalGrossAndDiscountsByOrg();
                                        }
                                        else {
                                            GetTotalGrossAndDiscountsByFOWST();
                                        }
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

                                    $('#btnOfflineManager').click(function () {
                                        sessionStorage.OfflineType = 'managers';
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

                                    $('#btnOfflineZRead').click(function () {
                                        sessionStorage.OfflineType = 'zreport';
                                        PageShow();
                                        $('#panGoToOffline').popup('close');
                                    });

                                    $('#btnOfflineSMCOINReport').click(function () {
                                        sessionStorage.OfflineType = 'smcoinreport';
                                        PageShow();
                                        $('#panGoToOffline').popup('close');
                                    });

                                    $('#btnOrgGroupTotal').click(function () {
                                        if (location.pathname != '/erp') {
                                            //$('.ItemsBreadCrumbs').text('');
                                            //breadcrumbs.prepend(arrowR, 'Reports for ' + sessionStorage.OrgName);
                                            //$('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                            //BreadCrumbsClickEvent();
                                            AddBreadCrumb({
                                                UpdateLastchild: 1,
                                                text: 'Reports for ' + sessionStorage.OrgName, Click: function () {
                                                }
                                            }, '.ItemsBreadCrumbs')
                                        }

                                        sessionStorage.GroupName = sessionStorage.OrgName;
                                        sessionStorage.SalesByOrg = true;
                                        $('#lblGroupName').text(sessionStorage.OrgName + ' Sales');
                                        $('input[name="filterGroupSales"]').attr('checked', 'true').checkboxradio('refresh');
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
                                        GetTotalGrossAndDiscountsByOrg();

                                        //for erp
                                        $('#selRevenueDashboard').val('Revenue Details');
                                    })

                                    $('#chbTransViewOpt').click(function () {
                                        $('#chbTransViewOpt').checkboxradio('disable');
                                        if ($(this).is(':checked')) {
                                            $('#divTransDetails').show();
                                            $('#divTransHeader').hide();
                                            GetTransactionDetails(viewtransreqdata);
                                        }
                                        else {
                                            $('#divTransHeader').show();
                                            $('#divTransDetails').hide();
                                            $('#lnkTransactionDetails').click(); //DashboardsAndReportsCharts.js
                                        }
                                        $('#chbTransViewOpt').checkboxradio('enable')
                                    })

                                    $('.btnCloseILAP').click(function () {
                                        $('#divILAppDisHeader').show();
                                        $('#divILAppDisDetails, #popInventoryLevelApproveDisapprove, #divbtnILAD').hide();
                                        $('#btnCheckAll').prop('checked', false).checkboxradio('refresh');
                                        $('#tblILADHistoryBody').empty();
                                        $('li[custom-tab-content]:first').click()
                                    })

                                    $('#btnBackILAP').click(function () {
                                        $('#divILAppDisHeader').show();
                                        $('#divILAppDisDetails, #divbtnILAD').hide();
                                        $('#btnCheckAll').prop('checked', false).checkboxradio('refresh');

                                        GetInventoryLevelApproveDisapprove(sessionStorage.ILADFowstID, sessionStorage.ILADFowstFowstName);
                                    })
                                }                                

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
                                                $('#pageOrderStatus, #pageOrderCentral').hide();
                                                $('#divRequestAndApprovals').show();

                                                //$('.ItemsBreadCrumbs').text('Request and Approvals');
                                                //RemoveBreadCrumb('Main Dashboard', '.ItemsBreadCrumbs')
                                                AddBreadCrumb({
                                                    UpdateLastchild: 1,
                                                    text: 'Request And Approvals', Click: function () {
                                                    }
                                                }, '.ItemsBreadCrumbs')
                                                $('#panGoTo, #panOption').popup('close');
                                                mDivSalesReturns.show();
                                            }
                                        }
                                    });
                                }                                

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
                                        $('#divMainDashboard, #divSalesAnalytics, #divRequestAndApprovals, #divCacMcr,' +
                                            '#divOnPrintPOSSalesReport,' +
                                            '#divSearchResult, #divSpecialReports, #divAuditTrail, #divSettings, #divSalesReplenishmentProcessing, #panTransactionDetails,#divFranchiseRequest').hide();

                                        $('#divSalesOrderProcessing').hide();
                                        $('#pageOrderStatus').hide();
                                        $('#pageOrderCentral').show();

                                        //$('.ItemsBreadCrumbs').text('Offline Product History');
                                        //RemoveBreadCrumb('Main Dashboard', '.ItemsBreadCrumbs')
                                        AddBreadCrumb({
                                            UpdateLastchild: 1,
                                            text: 'Offline Product History', Click: function () {
                                            }
                                        }, '.ItemsBreadCrumbs')
                                        $('#panGoTo, #panOption').popup('close');
                                        if (mDivSalesReturns != null)
                                            mDivSalesReturns.hide();
                                        if (mDivOfflineTransactionHistory != null)
                                            mDivOfflineTransactionHistory.hide();
                                        mDivOfflineProductHistory.show();
                                    }
                                } // END ShowOfflineProductHistory                                

                                function GetTotalGrossAndDiscountsByOrg() {
                                    TempValueBtnBack = 'BackToDashboard';
                                    data.GroupBy = 'ByOrganization';

                                    if (isFilterByOwnType) {
                                        data.FilterOwnType = arrFilterByOwnType;
                                    }
                                    else {
                                        data.FilterOwnType = [];
                                    }

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
                                                NoOfUnapprovedCounts: {
                                                    Text: 'No Of Unapproved Counts',
                                                    DataType: 'number',
                                                    Format: '#,###',
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
                                                    $('#popInventoryLevelApproveDisapproveOption').popup('open');
                                                    pDR = $($(e1.target).closest('tr')).data('data');
                                                    sessionStorage.ILADFowstID = pDR.FowstID;
                                                    sessionStorage.ILADFowstName = pDR.FowstName;
                                                });
                                            }
                                        });

                                        var pDR;

                                        $('#divGroupInventoryLevelsResult').append($('<div>').css('overflow', 'auto').append(pTable));
                                        pTable.table();
                                        pTable.trigger('create');

                                        $('#liViewSetInventoryBalance').unbind('click').click(function () {
                                            $('#panTableClick').popup('close');
                                            var pUrl = MergeUrlParameters({
                                                OrgID: sessionStorage.OrgID,
                                                FowstID: pDR.FowstID,
                                                View: 'Inventory',
                                                Name: pDR.FowstName,
                                                FPage: sessionStorage.IsShowFirstPage
                                            }, '/DashBoardsAndReports');
                                            window.open(pUrl, '_blank');
                                        })

                                        $('#liViewSetInventoryLevelApproval').unbind('click').click(function () {
                                            $('#popInventoryLevelApproveDisapproveOption').popup('close');
                                            $('#popInventoryLevelApproveDisapprove').show();
                                            GetInventoryLevelApproveDisapprove(pDR.FowstID, pDR.FowstName);
                                        })

                                        $('#btnRefreshILAP').unbind('click').click(function () {
                                            GetInventoryLevelApproveDisapprove(pDR.FowstID, pDR.FowstName);
                                        })

                                        $('#btnCheckAll').unbind('click').click(function () {
                                            var isChecked;
                                            if ($(this).is(':checked')) {
                                                isChecked = true; $('#divbtnILAD').show();
                                            }
                                            else {
                                                isChecked = false; $('#divbtnILAD').hide();
                                            }
                                            $('#tblnventoryLevelApproveDisapproveDetails tr').each(function () {
                                                $(this).find('td:eq(0)').find('input[type="checkbox"]').prop('checked', isChecked);
                                            })

                                            $('.chbILAD').checkboxradio('refresh');
                                        })

                                        $('#btnILADChbApp').unbind('click').click(function () {
                                            NewMessageBox('Please wait while saving ...');
                                            $('#tblnventoryLevelApproveDisapproveDetails tr').each(function () {
                                                var chb = $(this).find('td:eq(0)').find('input[type="checkbox"]');
                                                if (chb.is(':checked')) {
                                                    var chbData = chb.data();
                                                    var data = {
                                                        ID: chbData.ID,
                                                        CountDate: chbData.CountDate,
                                                        InventoryCountID: chbData.InventoryProductCountID,
                                                        FOWSTID: chbData.FowstID,
                                                        ProductID: chbData.ProductID,
                                                        BinID: chbData.BinID,
                                                        Variance: chbData.Variance,
                                                        Count: chbData.Quantity,
                                                        IsVarianceDisApproved: 0,
                                                        BatchID: chbData.BatchID,
                                                        FowstName: chbData.FowstName
                                                    }
                                                    ApprovedInventoryCount(data, true);
                                                }
                                            })
                                        })

                                        $('#btnILADChbDisapp').unbind('click').click(function () {
                                            NewMessageBox('Please wait while saving ...');
                                            $('#tblnventoryLevelApproveDisapproveDetails tr').each(function () {
                                                var chb = $(this).find('td:eq(0)').find('input[type="checkbox"]');
                                                if (chb.is(':checked')) {
                                                    var chbData = chb.data();
                                                    var data = {
                                                        ID: chbData.ID,
                                                        CountDate: chbData.CountDate,
                                                        InventoryCountID: chbData.InventoryProductCountID,
                                                        FOWSTID: chbData.FowstID,
                                                        ProductID: chbData.ProductID,
                                                        BinID: chbData.BinID,
                                                        Variance: chbData.Variance,
                                                        Count: chbData.Quantity,
                                                        IsVarianceDisApproved: 1,
                                                        BatchID: chbData.BatchID,
                                                        FowstName: chbData.FowstName
                                                    }
                                                    ApprovedInventoryCount(data, true);
                                                }
                                            })
                                        })

                                        $('#btnTabILADHistory').unbind('click').click(function () {
                                            if ($('#tblILADHistoryBody tr').length == 0) {
                                                GetILAPHistory(pDR.FowstID);
                                            }
                                        })

                                        $('#btnRefreshILAPHistory').unbind('click').click(function () {
                                            GetILAPHistory(pDR.FowstID);
                                        })

                                    } // END Display
                                } // END LoadGroupInventoryLevels

                                function GetILAPHistory(fowstid) {
                                    var tblILADHistoryBody = $('#tblILADHistoryBody');

                                    tblILADHistoryBody.empty().append('<tr><td colspan="11" style="text-align:center">Please wait while loading ...</td></tr>');

                                    AjaxSend({
                                        url: 'InventoryCount/GetInventoryCountForApprovalHistory',
                                        data: { FowstID: fowstid, IsAllItem: true },
                                        success: function (vData) {
                                            var vRows = vData.Results.Rows;
                                            tblILADHistoryBody.empty()

                                            if (vRows.length > 0) {
                                                for (var i = 0; i < vRows.length; i++) {
                                                    tblILADHistoryBody
                                                    .append($('<tr>')
                                                        .append($('<td>')
                                                            .text((i + 1) + '.')
                                                        )
                                                        .append($('<td>')
                                                            .text(vRows[i].FOWST)
                                                        )
                                                        .append($('<td>')
                                                            .text(vRows[i].Bin)
                                                        )
                                                        .append($('<td>')
                                                            .text($.format.date(new Date(vRows[i].CountDate), 'MMM dd, yyyy hh:mm a'))
                                                        )
                                                        .append($('<td>')
                                                            .text(vRows[i].ProductName)
                                                        )
                                                        .append($('<td>')
                                                            .text(vRows[i].Quantity)
                                                        )
                                                        .append($('<td>')
                                                            .text(vRows[i].UnitMeasure)
                                                        )
                                                        .append($('<td>')
                                                            .text(vRows[i].Variance)
                                                        )
                                                        .append($('<td>')
                                                            .css('color', (vRows[i].Status == 'Approved' ? '#168E03' : '#F00'))
                                                            .text(vRows[i].Status)
                                                        )
                                                        .append($('<td>')
                                                            .text(vRows[i].ModifiedBy)
                                                        )
                                                        .append($('<td>')
                                                            .text($.format.date(new Date(vRows[i].ModifiedDate), 'MMM dd, yyyy hh:mm a'))
                                                        )
                                                    )
                                                }
                                            }
                                            else {
                                                tblILADHistoryBody.append('<tr><td colspan="11" style="text-align:center">No Result(s) Found</td></tr>')
                                            }

                                            tblILADHistoryBody.parent().table('refresh');
                                        }
                                    })
                                }

                                function GetInventoryLevelApproveDisapprove(id, name) {
                                    var tblnventoryLevelApproveDisapprove = $('#tblnventoryLevelApproveDisapprove');

                                    tblnventoryLevelApproveDisapprove.empty().append('<tr><td colspan="5" style="text-align:center;">Please wait while loading ...</td></tr>');

                                    $('#txtInventoryLevelApproveDisapproveFowstName').text(name);
                                    $('#txtILADHistoryFowstName').text(name);

                                    AjaxSend({
                                        url: 'DashBoardsAndReports/GetInventoryLevelApproveDisapprove',
                                        data: { FowstID: id },
                                        success: function (vData) {
                                            var vRows = vData.Results.Rows;

                                            tblnventoryLevelApproveDisapprove.empty()

                                            if (vRows.length > 0) {
                                                for (var i = 0; i < vRows.length; i++) {
                                                    var cDate = vRows[i].InventoryCountDate;
                                                    var mPos = (cDate.length == 17 ? 17 : 16);
                                                    var mDate = cDate.substring(0, mPos) + ' ' + cDate.substr(-2);
                                                    vRows[i].InventoryCountDate = $.format.date(new Date(mDate), 'MMM dd, yyyy hh:mm a');
                                                    vRows[i].FowstName = name;
                                                    tblnventoryLevelApproveDisapprove.append($('<tr>')
                                                        .append($('<td>')
                                                            .text((i + 1) + '.')
                                                        )
                                                        .append($('<td>')
                                                            .text(vRows[i].InventoryCountDate)
                                                        )
                                                        .append($('<td>')
                                                            .text(CInt2(vRows[i].NoOfUnapprovedCounts))
                                                        )
                                                        .append($('<td>')
                                                            .text(vRows[i].CountedBy)
                                                        )
                                                        .append($('<td>')
                                                            .text(vRows[i].Remarks)
                                                        )
                                                        .data(vRows[i])
                                                        .css('cursor', 'pointer')
                                                        .click(function () {
                                                            var data = $(this).data();
                                                            $('#divILAppDisHeader').hide();
                                                            $('#divILAppDisDetails').show();
                                                            $('#txtInventoryLevelApproveDisapproveDate').text('Unapproved Count as of ' + data.InventoryCountDate)
                                                            GetILAPDetails(data, id);
                                                        })
                                                    )
                                                }
                                            }
                                            else {
                                                tblnventoryLevelApproveDisapprove.append('<tr><td colspan="5" style="text-align:center;">No Result(s) Found</td></tr>')
                                            }
                                        }
                                    })
                                }

                                function GetILAPDetails(mdata, fowstid) {
                                    var tblnventoryLevelApproveDisapproveDetails = $('#tblnventoryLevelApproveDisapproveDetails');

                                    tblnventoryLevelApproveDisapproveDetails.empty().append('<tr><td colspan="10" style="text-align:center;">Please wait while loading ...</td></tr>');

                                    AjaxSend({
                                        url: 'DashBoardsAndReports/GetInventoryLevelApproveDisapproveDetails',
                                        data: { BatchID: mdata.BatchID, FowstID: fowstid },
                                        success: function (vData) {
                                            var vRows = vData.Results.Rows;

                                            tblnventoryLevelApproveDisapproveDetails.empty();

                                            if (vRows.length > 0) {
                                                for (var i = 0; i < vRows.length; i++) {
                                                    vRows[i].BatchID = mdata.BatchID;

                                                    tblnventoryLevelApproveDisapproveDetails.append($('<tr>')
                                                        .attr('ilad', vRows[i].ProductID)
                                                        .append($('<td>')
                                                            .append($('<label style="position:relative;right:-0.5em;">')
                                                                .append($('<input type="checkbox" id="chb_' + i + '" name="radPriceLevel" class="chbILAD" data-iconpos="notext"/>')
                                                                    .data(vRows[i])
                                                                    .click(function () {
                                                                        var isChecked;
                                                                        if ($('.chbILAD:checked').length > 0) {
                                                                            $('#divbtnILAD').show();
                                                                        }
                                                                        else {
                                                                            $('#divbtnILAD').hide();
                                                                        }
                                                                    })
                                                                )
                                                            )
                                                        )
                                                        .append($('<td>')
                                                            .text((i + 1) + '.')
                                                        )
                                                        .append($('<td>')
                                                            .text(vRows[i].Bin)
                                                        )
                                                        .append($('<td>')
                                                            .text($.format.date(new Date(vRows[i].CountDate), 'MMM dd, yyyy'))
                                                        )
                                                        .append($('<td>')
                                                            .text(vRows[i].ProductName)
                                                        )
                                                        .append($('<td>')
                                                            .text(AccountingFormat(vRows[i].Quantity))
                                                        )
                                                        .append($('<td>')
                                                            .text(vRows[i].UnitMeasure)
                                                        )
                                                        .append($('<td>')
                                                            .text(AccountingFormat(vRows[i].Variance))
                                                        )
                                                        .append($('<td>')
                                                            .html('<a href="#" data-role="button">Approve Count </a>')
                                                            .attr('ID', vRows[i].ID)
                                                            .data('data', vRows[i])
                                                            .click(function (e) {
                                                                e.preventDefault();
                                                                e.stopPropagation();

                                                                var data = {
                                                                    ID: $(this).attr('ID'),
                                                                    CountDate: $(this).data('data').CountDate,
                                                                    InventoryCountID: $(this).data('data').InventoryProductCountID,
                                                                    FOWSTID: fowstid,
                                                                    ProductID: $(this).data('data').ProductID,
                                                                    BinID: $(this).data('data').BinID,
                                                                    Variance: $(this).data('data').Variance,
                                                                    Count: $(this).data('data').Quantity,
                                                                    IsVarianceDisApproved: 0,
                                                                    BatchID: mdata.BatchID,
                                                                    FowstName: mdata.FowstName
                                                                }
                                                                ApprovedInventoryCount(data, false);
                                                            })
                                                        )
                                                        .append($('<td>')
                                                            .html('<a href="#" data-role="button">Disapprove Count </a>')
                                                            .attr('ID', vRows[i].ID)
                                                            .data('data', vRows[i])
                                                            .click(function (e) {
                                                                e.preventDefault();
                                                                e.stopPropagation();

                                                                var data = {
                                                                    ID: $(this).attr('ID'),
                                                                    CountDate: $(this).data('data').CountDate,
                                                                    InventoryCountID: $(this).data('data').InventoryProductCountID,
                                                                    FOWSTID: fowstid,
                                                                    ProductID: $(this).data('data').ProductID,
                                                                    BinID: $(this).data('data').BinID,
                                                                    Variance: $(this).data('data').Variance,
                                                                    Count: $(this).data('data').Quantity,
                                                                    IsVarianceDisApproved: 1,
                                                                    BatchID: mdata.BatchID,
                                                                    FowstName: mdata.FowstName
                                                                }

                                                                ApprovedInventoryCount(data, false);
                                                            })
                                                        )
                                                        .data(vRows[i])
                                                    )
                                                }
                                            }
                                            else {
                                                tblnventoryLevelApproveDisapproveDetails.append('<tr><td colspan="10" style="text-align:center;">No Result(s) Found</td></tr>');
                                            }
                                            tblnventoryLevelApproveDisapproveDetails.parent().table('refresh');
                                            $('input[type="checkbox"]').checkboxradio().checkboxradio('refresh')
                                        }
                                    })
                                }

                                function ApprovedInventoryCount(mdata, isfromcheckall) {
                                    ctrCheckAllILAD = 1;
                                    AjaxSend({
                                        url: 'InventoryCount/ApprovedInventoryCount',
                                        data: mdata,
                                        success: function (vData) {
                                            if (vData.Results == 'True') {
                                                if (isfromcheckall) {
                                                    if ($('.chbILAD:checked').length == ctrCheckAllILAD) {
                                                        NewMessageBox('Successfully saved');
                                                        GetILAPDetails(mdata, mdata.FOWSTID);
                                                        $('#btnCheckAll').prop('checked', false).checkboxradio('refresh');
                                                    }
                                                    ctrCheckAllILAD = ctrCheckAllILAD + 1;
                                                }
                                                else {
                                                    if ($('#tblnventoryLevelApproveDisapproveDetails tr').length == 0) {
                                                        $('#divILAppDisHeader').show();
                                                        $('#divILAppDisDetails').hide();
                                                        GetInventoryLevelApproveDisapprove(mdata.FOWSTID);
                                                    }
                                                    else {
                                                        GetILAPDetails(mdata, mdata.FOWSTID)
                                                    }
                                                    $('#btnCheckAll').prop('checked', false).checkboxradio('refresh');
                                                }
                                            }
                                        }
                                    });
                                }

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
                                        sessionStorage.IsFromPrevTab = true;
                                        sessionStorage.IsShowFirstPage = GetUrlParameters().FPage;
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
                                $('#divMainDashboard, .btnPeriod, .lblPeriod').fadeIn();
                                //$('#btnPeriodMobile').addClass('btnPeriod').removeClass('btn-disable').on('click.disabled');
                                $('#divSalesAnalytics, #divOnPrintPOSSalesReport,' +
                                    ' #divOrderCentral,' +
                                    '#divSpecialReports, #divAuditTrail,' +
                                    '#divSettings, .divSearchFilterDateRange,#panTransactionDetails, #divInventoryBalancePage').hide();
                                $('#divFranchiseRequest, #divRequestAndApprovals, #divCacMcr').hide();
                                $('#divSalesReplenishmentProcessing').hide();
                                $('#divSalesOrderProcessing').hide();

                                sessionStorage.SearchMode = 'false';

                                if (sessionStorage.IsShowFirstPage != 'false') {
                                    TempValueBtnBack = null;
                                }

                                GetData();

                                AddBreadCrumb({
                                    text: 'Main Dashboard', Click: function () {
                                        $('#divMainDashboard').pageshow();
                                    }
                                }, '.ItemsBreadCrumbs')
                            }

                            function AccessForInventoryOfPayment(module, callback) {
                                PromptUserAccess(module, {
                                    OnClickOKSuccess: function () {
                                        if (CStr2(GetUrlParameters().InvetoryOfPaymentDone) == '1') {
                                            //Cash handling module gikan ang param
                                            callback();
                                            return;
                                        }
                                        if (IsMobile) {
                                            var accessrights = security_offline().CheckSecurityCurrentUserAcccess({
                                                ModuleName: 'Input of Inventory of Payments is Required Prior to Showing Dashboards and On Print Reports?',
                                                UserID: sessionStorage.PeopleID
                                            }).Rows

                                            if (accessrights.length > 0) {
                                                if (CBool(accessrights[0].IsHasAccess)) {
                                                    NewMessageBox('Before you can view your report, you are required to count or turnover your cash. What would you like to do?', 'Message', [
                                                        {
                                                            Text: 'Turnover / Deposit money', Click: function () {
                                                                window.location = '/CashHandling/DepositMonies?IsMobile=1&type=turnover&viewreport=1'
                                                            }
                                                        }, {
                                                            Text: 'Cash count only', Click: function () {
                                                                window.location = '/CashHandling/DepositMonies?IsMobile=1&type=cashcountonly&viewreport=1'
                                                            }
                                                        }
                                                    ])
                                                    return;
                                                }
                                            }
                                        }
                                        
                                        callback();
                                    }
                                });
                            }

                            function GetCashierSalesReportPage(isManagersSalesReport) {
                                sessionStorage.POSSalesReportType = isManagersSalesReport;                                

                                if (CounterCashierSalesReport == true) {
                                    if (CBool(sessionStorage.PrintingOfflineData)) {
                                        //AjaxSend({
                                        //    url: 'Login3/TestCookies',
                                        //    success: function (vData) {
                                        AccessForInventoryOfPayment((CBool(isManagersSalesReport) ? 'Manager Sales Report' : 'Cashier Sales Report'), function () {
                                            var pData = {};
                                            pData.OrgID = sessionStorage["OrgID"];
                                            pData.POSSalesReportType = 0;

                                            //AjaxSend({
                                            //    url: 'ReportsPOS/setPOSSalesReportType',
                                            //    data: pData,
                                            //    success: function (vData) {
                                            AjaxSend({
                                                ExactUrl: '/ReportsPOS/SalesReport?cache=1&OrgID=' + sessionStorage.OrgID + '&IsManagerSalesReport=' + isManagersSalesReport + '&IsMobile=' + IsMobile,
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
                                            //    }
                                            //});
                                        })
                                        //},
                                        //error: function (vError) {
                                        //    PromptUserAccess('Cashier Sales Report', {
                                        //        OnClickOKSuccess: function () {
                                        //            AjaxSend({
                                        //                ExactUrl: '/ReportsPOS/SalesReport?cache=1&OrgID=' + sessionStorage.OrgID + '&IsManagerSalesReport=0&IsMobile=' + IsMobile,
                                        //                type: 'GET',
                                        //                dataType: 'text',
                                        //                success: function (vData) {
                                        //                    $('#divOnPrintPOSSalesReport').empty();
                                        //                    $('#divOnPrintPOSSalesReport').append(vData).trigger('create');

                                        //                    App.Deferred.fulfill('FQ26E5TP76');

                                        //                    setMonthCollection("selPOSSalesReportByDateFromMonth");
                                        //                    setYearCollection("selPOSSalesReportByDateFromYear");

                                        //                    setMonthCollection("selPOSSalesReportByDateToMonth");
                                        //                    setYearCollection("selPOSSalesReportByDateToYear");

                                        //                    setDashboardDate(m_Period);

                                        //                    $('.btnSelectFowst').show();
                                        //                }
                                        //            });
                                        //        }
                                        //    });
                                        //}
                                        //});

                                        //CounterCashierSalesReport = false;
                                    } else {
                                        AccessForInventoryOfPayment((CBool(isManagersSalesReport) ? 'Manager Sales Report' : 'Cashier Sales Report'), function () {
                                            var isPageReady = false;
                                            var pData = {};
                                            pData.OrgID = sessionStorage["OrgID"];
                                            pData.POSSalesReportType = isManagersSalesReport;

                                            AjaxSend({
                                                url: 'ReportsPOS/setPOSSalesReportType',
                                                data: pData,
                                                success: function (vData) {
                                                    if (cashiermanagerpage == "") {
                                                        AjaxSend({
                                                            ExactUrl: '/ReportsPOS/SalesReport?cache=1&OrgID=' + sessionStorage.OrgID + '&IsManagerSalesReport=' + isManagersSalesReport + '&IsMobile=' + IsMobile,
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
                                        })
                                    }
                                } else {
                                    //Do Nothing Here
                                }                                

                                $('#divMainDashboard, #divRequestAndApprovals, #divCacMcr').hide();
                                $('#divSalesAnalytics').hide();
                                $('#divSalesReplenishmentProcessing').hide();
                                $('#divFranchiseRequest').hide();
                                $('#divSalesOrderProcessing').hide();
                                $('#panTransactionDetails').hide();

                                $('#divOnPrintPOSSalesReport').show();

                                //$('.ItemsBreadCrumbs').text('');

                                if (CBool(sessionStorage.PrintingOfflineData)) {
                                    //$('.ItemsBreadCrumbs').text('Cashier\'s Sales Report');
                                    ////RemoveBreadCrumb('Main Dashboard', '.ItemsBreadCrumbs')
                                }
                                else {
                                    //breadcrumbs.prepend(arrowR, 'Cashier\'s Sales Report');
                                    //$('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                    //BreadCrumbsClickEvent();
                                }
                                
                                AddBreadCrumb({
                                    UpdateLastchild: 1,
                                    text: 'Cashier\'s Sales Report', Click: function () {
                                    }
                                }, '.ItemsBreadCrumbs')

                                //TempValueBtnBack = "BackToDashboard";

                                $('#panOption').popup('close');
                            }

                            function SalesbySKUReport() {
                                var pData = {};
                                pData.OrgID = sessionStorage["OrgID"];
                                pData.POSSalesReportType = 0;

                                AjaxSend({
                                    ExactUrl: '/ReportsPOS/SKUUnitSalesReport?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                                    type: 'GET',
                                    dataType: 'text',
                                    success: function (vData) {

                                        $('#divOnPrintPOSSalesReport').empty();
                                        $('#divOnPrintPOSSalesReport').append(vData).trigger('create');

                                        App.Deferred.fulfill('FQ26E5TP76');
                                        
                                    }
                                });


                                $('#divMainDashboard, #divRequestAndApprovals, #divCacMcr').hide();
                                $('#divSalesAnalytics').hide();
                                $('#divSalesReplenishmentProcessing').hide();
                                $('#divFranchiseRequest').hide();
                                $('#divSalesOrderProcessing').hide();
                                $('#panTransactionDetails').hide();

                                $('#divOnPrintPOSSalesReport').show();

                                $('#panOption').popup('close');
                            }

                            function GetSMCOINReportPage() {
                                var pData = {};
                                pData.OrgID = sessionStorage["OrgID"];
                                pData.POSSalesReportType = 0;

                                AjaxSend({
                                    ExactUrl: '/MallLessorSM?cache=1&OrgID=' + sessionStorage.OrgID + 'IsMobile=' + IsMobile,
                                    type: 'GET',
                                    dataType: 'text',
                                    success: function (vData) {

                                        $('#divOnPrintPOSSalesReport').empty();
                                        $('#divOnPrintPOSSalesReport').append(vData).trigger('create');

                                        App.Deferred.fulfill('FQ26E5TP76');

                                        //setMonthCollection("selPOSSalesReportByDateFromMonth");
                                        //setYearCollection("selPOSSalesReportByDateFromYear");

                                        //setMonthCollection("selPOSSalesReportByDateToMonth");
                                        //setYearCollection("selPOSSalesReportByDateToYear");

                                        //setDashboardDate(m_Period);

                                        //$('.btnSelectFowst').show();

                                    }
                                });


                                $('#divMainDashboard, #divRequestAndApprovals, #divCacMcr').hide();
                                $('#divSalesAnalytics').hide();
                                $('#divSalesReplenishmentProcessing').hide();
                                $('#divFranchiseRequest').hide();
                                $('#divSalesOrderProcessing').hide();
                                $('#panTransactionDetails').hide();

                                $('#divOnPrintPOSSalesReport').show();

                                //$('.ItemsBreadCrumbs').text('');

                                //if (CBool(sessionStorage.PrintingOfflineData)) {
                                //    $('.ItemsBreadCrumbs').text('Cashier\'s Sales Report');
                                //}
                                //else {
                                //    breadcrumbs.prepend(arrowR, 'Cashier\'s Sales Report');
                                //    $('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                //    BreadCrumbsClickEvent();
                                //}

                                $('#panOption').popup('close');
                            
                            }


                            window.GetData = function () {
                                var m_DateFrom = new Date();
                                var m_DateTo = new Date();

                                clearData();

                                switch (m_Period) {
                                    case "2DaysAgo":
                                        //back date from
                                        m_DateFrom.setDate(m_DateFrom.getDate() - 2);
                                        m_DateTo.setDate(m_DateTo.getDate() - 2);
                                        break;
                                        ;
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
                                        var newDateFrom = new Date($('#txtDateFrom').text());
                                        var newDateTo = new Date($('#txtDateTo').text());
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

                                if (sessionStorage.ReportType == 'MoniesAccountability' || sessionStorage.ReportType == 'DepositReport') {
                                    GetDepositReport(); //DepositReport.js
                                }

                                if (sessionStorage.PrintingOfflineData == "false") {
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
                                        if (CBool(sessionStorage.IsShowFirstPage)) {
                                            sessionStorage.IsFromDashboardPage = true;
                                            GetTotalGrossAndDiscounts();
                                            data.GroupBy = 'ByOrganizationGroup';
                                            GetTotalGrossAndDiscountsByGROUP();
                                        } else {
                                            if (TempValueBtnBack == null) {
                                                TempValueBtnBack == 'BackToDashboard';
                                            }
                                            GetTotalGrossAndDiscountsByFOWST();
                                        }
                                    }
                                }
                            }


                            function GetTotalGrossAndDiscounts() {
                                AjaxSend({
                                    url: 'DashboardsAndReports/GetTotalGrossAndDiscounts',
                                    data: data,
                                    success: function (vData) {
                                        if (vData.Results.Rows.length > 0) {
                                            DisplayTotalGrossAndDiscounts(vData.Results.Rows);
                                        }

                                        if (CBool(sessionStorage.IsFromDashboardPage)) {
                                            GetTotalGrossAndDiscountsByGROUP();
                                        }
                                        
                                    }
                                });
                            }

                            function DisplayTotalGrossAndDiscounts(vRows) {

                                $('#TotalExchanges').html(AccountingFormat(vRows[0].TotalReturns));
                                $('#TotalGrossSales').text(AccountingFormat(vRows[0].GrossSales));
                                $('#TotalDiscounts').text(AccountingFormat(vRows[0].TotalDiscounts));
                                $('#ForegoneVAT').text(AccountingFormat(vRows[0].ForegoneVAT));

                                $('#lblSalesTransactionCount').text(CInt2(vRows[0].InvoiceCount));
                                $('#lblSalesReturnsTransactionCount').text(CInt2(vRows[0].SalesReturnsCount));


                                $('#TotalNetSales').text(AccountingFormat(CDec2($('#TotalGrossSales').text()) - CDec2($('#TotalDiscounts').text()) - CDec2($('#TotalExchanges').text()) - CDec2($('#ForegoneVAT').text())));

                                $('#TotalRefunds').text('-');
                            }

                            function GetTotalGrossAndDiscountsByGROUP() {
                                AjaxSend({
                                    url: 'DashboardsAndReports/GetTotalGrossAndDiscountsByGROUP',
                                    data: data,
                                    success: function (vData) {
                                        if (vData.Results.Rows.length > 0) {
                                            DisplayTotalGrossAndDiscountsByGroup(vData.Results.Rows);
                                        } else {
                                            $('#MainFrame').empty().append($('<div style="text-align:center;">').append($('<h3>').text('No sales data found')));
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

                                    if ((i % 4) == 0) {
                                        insideGrid = 0;
                                        pItem = $('<div class="ui-grid-' + (CBool(sessionStorage.IsViewAllInfo) ? 'b' : 'c') + '">')
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
                                        case 3:
                                            var insideGridBlock = "d";
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

                                                    .append($("<div class='row'>")
                                                        .append($("<div class='col-sm-6' style='text-align:left; font-size:13px'>")
                                                            .text('Net Sales')
                                                        )
                                                        .append($("<div class='col-sm-6' style='text-align:right; font-size:13px'>")
                                                            .text(AccountingFormat(parseFloat(vRows[i].GrossSales) - parseFloat(vRows[i].TotalDiscounts) - parseFloat(vRows[i].TotalReturns == undefined ? '0' : vRows[i].TotalReturns) - parseFloat(vRows[i].ForegoneVAT == undefined ? '0' : vRows[i].ForegoneVAT)))
                                                        )
                                                    )

                                                    .append($("<div class='row' style='margin-top:15px'>")
                                                        .append($("<div class='col-sm-6' style='text-align:left; font-size:13px'>")
                                                            .text('Gross Sales')
                                                        )
                                                        .append($("<div class='col-sm-6' style='text-align:right; font-size:13px'>")
                                                            .text(AccountingFormat(vRows[i].GrossSales))
                                                        )
                                                    )
                                                    
                                                    .append($("<div style='text-align:left; font-size:13px; margin-top:15px'>")
                                                        .text('Less:')
                                                    )

                                                    .append($("<div class='row' style='margin-left:5px'>")
                                                        .append($("<div class='col-sm-6' style='text-align:left; font-size:13px'>")
                                                            .text('Discounts')
                                                        )
                                                        .append($("<div class='col-sm-6' style='text-align:right; font-size:13px'>")
                                                            .text(AccountingFormat(vRows[i].TotalDiscounts))
                                                        )
                                                    )

                                                    .append($("<div class='row' style='margin-left:5px'>")
                                                        .append($("<div class='col-sm-6' style='text-align:left; font-size:13px'>")
                                                            .text('Vat Exempt Tax')
                                                        )
                                                        .append($("<div class='col-sm-6' style='text-align:right; font-size:13px'>")
                                                            .text(AccountingFormat(vRows[i].ForegoneVAT))
                                                        )
                                                    )

                                                    .append($("<div class='row' style='margin-left:5px'>")
                                                        .append($("<div class='col-sm-6' style='text-align:left; font-size:13px'>")
                                                            .text('Refunds')
                                                        )
                                                        .append($("<div class='col-sm-6' style='text-align:right; font-size:13px'>")
                                                            .text('-')
                                                        )
                                                    )
                                                    
                                                    .append($("<div class='row' style='margin-left:5px'>")
                                                        .append($("<div class='col-sm-6' style='text-align:left; font-size:13px'>")
                                                            .text('Exchanges')
                                                        )
                                                        .append($("<div class='col-sm-6' style='text-align:right; font-size:13px'>")
                                                            .text(AccountingFormat(vRows[i].TotalReturns))
                                                        )
                                                    )

                                                    .append($("<div class='row' style='margin-top:15px'>")
                                                        .append($("<div class='col-sm-6' style='text-align:left; font-size:13px'>")
                                                            .text('Sales Transaction Count')
                                                        )
                                                        .append($("<div class='col-sm-6' style='text-align:right; font-size:13px'>")
                                                            .text(CInt2(vRows[i].InvoiceCount))
                                                        )
                                                    )

                                                    .append($("<div class='row'>")
                                                        .append($("<div class='col-sm-6' style='text-align:left; font-size:13px'>")
                                                            .text('Refunds/Exchanges Transaction Count')
                                                        )
                                                        .append($("<div class='col-sm-6' style='text-align:right; font-size:13px'>")
                                                            .text(CInt2(vRows[i].SalesReturnsCount))
                                                        )
                                                    )
                                                    //.append($('<div style="text-align:left; font-size:13px">')
                                                    //    .text('Net Sales')
                                                    //)
                                                    //.append($('<div style="text-align:right; font-size:15px;"">')
                                                    //    .text(AccountingFormat(parseFloat(vRows[i].GrossSales) - parseFloat(vRows[i].TotalDiscounts) - parseFloat(vRows[i].TotalReturns == undefined ? '0' : vRows[i].TotalReturns) - parseFloat(vRows[i].ForegoneVAT == undefined ? '0' : vRows[i].ForegoneVAT)))
                                                    //)
                                                    //.append($('<div style="text-align:left; font-size:13px">')
                                                    //    .text('Gross Sales')
                                                    //)
                                                    //.append($('<div style="text-align:right; font-size:15px;"">')
                                                    //    .text(AccountingFormat(vRows[i].GrossSales))
                                                    //)
                                                    //.append($('<div style="text-align:left; font-size:13px">')
                                                    //    .text('Discounts')
                                                    //)
                                                    //.append($('<div style="text-align:right; font-size:15px;"">')
                                                    //    .text(AccountingFormat(vRows[i].TotalDiscounts))
                                                    //)
                                                    //.append($('<div style="text-align:left; font-size:13px">')
                                                    //    .text('Vat Exempt Tax')
                                                    //)
                                                    //.append($('<div style="text-align:right; font-size:15px;"">')
                                                    //    .text(AccountingFormat(vRows[i].ForegoneVAT))
                                                    //)
                                                    //.append($('<div style="text-align:left; font-size:13px">')
                                                    //    .text('Refunds')
                                                    //)
                                                    //.append($('<div style="text-align:right; font-size:15px;"">')
                                                    //    .text('-')
                                                    //)
                                                    //.append($('<div style="text-align:left; font-size:13px">')
                                                    //    .text('Exchanges')
                                                    //)
                                                    //.append($('<div style="text-align:right; font-size:15px;">')
                                                    //    .text(AccountingFormat(vRows[i].TotalReturns))
                                                    //)
                                                )
                                                .data('data', vRows[i])
                                                .click(function () {
                                                    This = $(this);
                                                    //$('#panGoTo').popup('open');
                                                    $('.btnSalesAnalytics').unbind('click').click(function () {
                                                        sessionStorage.GroupName = This.data('data').GroupName;
                                                        sessionStorage.SalesByOrg = false;
                                                        $('#panGoTo').popup('close');
                                                        if (location.pathname != '/erp') {
                                                            //$('.ItemsBreadCrumbs').text('');
                                                            //breadcrumbs.prepend(arrowR, 'Group Reports for ' + This.data('data').GroupName + ' Group');
                                                            //$('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                                            //BreadCrumbsClickEvent();
                                                            AddBreadCrumb({
                                                                UpdateLastchild: 1,
                                                                text: 'Group Reports for ' + This.data('data').GroupName + ' Group', Click: function () {
                                                                }
                                                            }, '.ItemsBreadCrumbs')
                                                        }

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
                                                        $('#lblGroupNetSales').text(AccountingFormat(parseFloat(This.data('data').GrossSales) - parseFloat(This.data('data').TotalDiscounts) - parseFloat(This.data('data').TotalReturns == undefined ? '0' : This.data('data').TotalReturns) - parseFloat(This.data('data').ForegoneVAT == undefined ? '0' : This.data('data').ForegoneVAT)));
                                                        $('#lblGroupGrossSales').text(AccountingFormat(This.data('data').GrossSales));
                                                        $('#lblGroupDiscounts').text(AccountingFormat(This.data('data').TotalDiscounts));
                                                        $('#lblGroupForegoneVAT').text(AccountingFormat(This.data('data').ForegoneVAT));
                                                        $('#lblGroupRefunds').text('-');
                                                        $('#lblGroupExchanges').text(AccountingFormat(This.data('data').TotalReturns));
                                                        $('input[name="filterGroupSales"]').attr('checked', 'true').checkboxradio('refresh');
                                                        $('#btnReportGroupSales').click();

                                                        TempValueBtnBack = 'BackToDashboard';
                                                        data.GroupBy = 'ByOrganizationFOWST';
                                                        GroupID = This.data('data').GroupID;
                                                        GetTotalGrossAndDiscountsByFOWST();
                                                        isFilterByOwnType = false;

                                                        //for erp
                                                        $('#selRevenueDashboard').val('Revenue Details');
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
                                        $('#divSalesOrderProcessing, #divCacMcr').hide();
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
                                        $('#divSalesOrderProcessing, #divCacMcr').hide();
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
                                $('#btnGroupReports').click(function () {
                                    $('#divSalesAnalytics').show();
                                    $('#panTransactionDetails').hide();

                                    //$('.ItemsBreadCrumbs').text('');
                                    if (CBool(sessionStorage.SalesByOrg)) {
                                        AddBreadCrumb({
                                            UpdateLastchild: 1,
                                            text: 'Reports for ' + sessionStorage.GroupName, Click: function () {
                                            }
                                        }, '.ItemsBreadCrumbs')
                                    }
                                    else {
                                        AddBreadCrumb({
                                            UpdateLastchild: 1,
                                            text: 'Group Reports for ' + This.data('data').GroupName + ' Group', Click: function () {
                                            }
                                        }, '.ItemsBreadCrumbs')
                                    }
                                    //$('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                    //BreadCrumbsClickEvent();
                                })
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
                                            //$('.ItemsBreadCrumbs').text('');
                                            //breadcrumbs.prepend(arrowR, ProductName + ' Ledger');
                                            //breadcrumbs.prepend(arrowR, '<a id="btnBCInventoryBalances">Inventory Balances</a>');
                                            //$('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                            //BreadCrumbsClickEvent();
                                            AddBreadCrumb({
                                                UpdateLastchild: 1,
                                                text: ProductName + ' Ledger', Click: function () {
                                                }
                                            }, '.ItemsBreadCrumbs')
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
                                var TotalForegoneVAT = 0;
                                pLv.empty();

                                if (TempValueBtnBack == 'BackToDashboard') {
                                    for (i = 0; i <= vRows.length - 1; i++) {
                                        TotalNetSales += parseFloat(vRows[i].GrossSales) - parseFloat(vRows[i].TotalDiscounts) - (vRows[i].TotalReturns == undefined ? 0 : parseFloat(vRows[i].TotalReturns)) - (vRows[i].ForegoneVAT == undefined ? 0 : parseFloat(vRows[i].ForegoneVAT));
                                        TotalGrossSales += parseFloat(vRows[i].GrossSales);
                                        TotalDiscounts += parseFloat(vRows[i].TotalDiscounts);
                                        TotalReturns += (vRows[i].TotalReturns == undefined ? 0 : parseFloat(vRows[i].TotalReturns));
                                        TotalForegoneVAT += (vRows[i].ForegoneVAT == undefined ? 0 : parseFloat(vRows[i].ForegoneVAT));
                                    }

                                    $('#lblGroupNetSales').html(AccountingFormat(TotalNetSales));
                                    $('#lblGroupGrossSales').html(AccountingFormat(TotalGrossSales));
                                    $('#lblGroupDiscounts').html(AccountingFormat(TotalDiscounts));
                                    $('#lblGroupForegoneVAT').html(AccountingFormat(TotalForegoneVAT));
                                    $('#lblGroupExchanges').html(AccountingFormat(TotalReturns));
                                }

                                if (vRows.length > 0) {
                                    for (var i = 0; i <= vRows.length - 1; i++) {
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
                                            .text(vRows[i].FOWSTName)
                                        )
                                        .append($('<td>')
                                            .text(vRows[i].OwnershipType)
                                        )
                                        .append($('<td>')
                                            .text((vRows[i].Region == undefined ? "" : vRows[i].Region.split('-')[0]))
                                        )
                                        .append($('<td>')
                                            .text((vRows[i].BusinessOwner == undefined ? "" : vRows[i].BusinessOwner))
                                        )
                                        .append($('<td>')
                                            .text((vRows[i].OrganizationName == undefined ? "" : vRows[i].OrganizationName))
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text(AccountingFormat(parseFloat(vRows[i].GrossSales) - parseFloat(vRows[i].TotalDiscounts) - (vRows[i].TotalReturns == undefined ? 0 : parseFloat(vRows[i].TotalReturns)) - (vRows[i].ForegoneVAT == undefined ? 0 : parseFloat(vRows[i].ForegoneVAT))))
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text(AccountingFormat(((parseFloat(vRows[i].GrossSales) - parseFloat(vRows[i].TotalDiscounts) - CDec2(vRows[i].TotalReturns == 0 ? 0 : parseFloat(vRows[i].TotalReturns)) - CDec2(vRows[i].ForegoneVAT == 0 ? 0 : parseFloat(vRows[i].ForegoneVAT))) / CDec2($('#lblGroupNetSales').text())) * 100) + '%')
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text(AccountingFormat(vRows[i].GrossSales))
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text(AccountingFormat((CDec2(vRows[i].GrossSales) / CDec2($('#lblGroupGrossSales').text())) * 100) + '%')
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text(AccountingFormat(vRows[i].TotalDiscounts))
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text((CDec2(vRows[i].TotalDiscounts) / CDec2($('#lblGroupDiscounts').text())) == 0 ? '-' : (AccountingFormat((CDec2(vRows[i].TotalDiscounts) / CDec2($('#lblGroupDiscounts').text())) * 100) + '%'))
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text(AccountingFormat(vRows[i].ForegoneVAT))
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text((CDec2(vRows[i].ForegoneVAT) / CDec2($('#lblGroupForegoneVAT').text())) == 0 ? '-' : (AccountingFormat((CDec2(vRows[i].ForegoneVAT) / CDec2($('#lblGroupForegoneVAT').text())) * 100) + '%'))
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text('-')
                                        )
                                             .append($('<td style="text-align:right">')
                                            .text('-')
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text(AccountingFormat(vRows[i].TotalReturns))
                                        )
                                             .append($('<td style="text-align:right">')
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
                                            viewtransreqdata = vData;
                                            GetPOSSalesReportByTrendOrSalesDistribution(vData);
                                            GetReceiptPrintedStatus(vData);

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
                                            .attr('colspan', '15')
                                            .css('text-align', 'center')
                                            .text('No Result(s)')
                                        )
                                    )
                                }
                                pLv.parent().table('refresh');
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

                                if (vRows.Rows.length == 1) {
                                    sessionStorage.FowstID = vRows.Rows[0].ID;
                                    sessionStorage.SelectedFOWST = vRows.Rows[0].ID;
                                    sessionStorage.SelectedFOWSTName = vRows.Rows[0].FowstName;
                                    $('.btnSelectFowst').removeClass('btnRequired').addClass('btnNotRequired');
                                }

                                if (vRows.Rows.length > 0) {
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
                                                    $('button[id="btnInventoryLedgerFowst"]').removeClass('btnRequired').addClass('btnNotRequired');
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
                                                    } else if (sessionStorage.ReportType == 'MoniesAccountability' || sessionStorage.ReportType == 'DepositReport') {
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
                                }
                                else {
                                    pLV.append($('<li>')
                                        .append('No Result(s) Found')
                                    )
                                }

                                try {
                                    pLV.listview('refresh');
                                }
                                catch (e) {
                                    pLV.listview();
                                }
                            }

                            function GetOrganizationName() {
                                AjaxSend({
                                    url: 'DashboardsAndReports/GetOrganizationName',
                                    success: function (vData) {
                                        if (vData.Results.Rows.length > 0) {
                                            $('.OrganizationNameHeader').text(vData.Results.Rows[0].CompanyName)
                                            $('#OrganizationName').html(vData.Results.Rows[0].CompanyNameInitial)
                                        }
                                    }
                                });
                            }

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

                                if (ValidPrevPage(arguments)) {
                                    if (CBool(sessionStorage.PrintingOfflineData)) {
                                        $('.OrganizationNameHeader').text(sessionStorage.OrgName);
                                        ////RemoveBreadCrumb('Main Dashboard', '.ItemsBreadCrumbs')

                                        if (sessionStorage.OfflineType == 'cashier') {
                                            $('#divSalesAnalytics, #divOfflineTransactionHistoryMain, #divFranchiseRequest, #divOnPrintPOSSalesReport').hide();
                                            GetCashierSalesReportPage(0);
                                            //$('.ItemsBreadCrumbs').text('Cashiers Sales Report');
                                            AddBreadCrumb({
                                                UpdateLastchild: 1,
                                                text: 'Cashiers Sales Report', Click: function () {
                                                }
                                            }, '.ItemsBreadCrumbs')
                                        }
                                        else if (sessionStorage.OfflineType == 'managers') {
                                            $('#divSalesAnalytics, #divOfflineTransactionHistoryMain, #divFranchiseRequest, #divOnPrintPOSSalesReport').hide();
                                            GetCashierSalesReportPage(1);
                                            //$('.ItemsBreadCrumbs').text('Managers Sales Report');
                                            AddBreadCrumb({
                                                UpdateLastchild: 1,
                                                text: 'Managers Sales Report', Click: function () {
                                                }
                                            }, '.ItemsBreadCrumbs')
                                        }
                                        else if (sessionStorage.OfflineType == 'monies') {
                                            $('#divSalesAnalytics').show();
                                            $('#divOnPrintPOSSalesReport').hide();
                                            $('#divGroupSales').hide();
                                            $('#dvSalesTrends').hide();
                                            $('#divInventoryLevels').hide();
                                            $('#divInventoriesAcc').hide();
                                            $('#divFranchiseRequest').hide();
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
                                            //$('.ItemsBreadCrumbs').text('Monies Accountability');
                                            AddBreadCrumb({
                                                UpdateLastchild: 1,
                                                text: 'Monies Accountability', Click: function () {
                                                }
                                            }, '.ItemsBreadCrumbs')
                                        }
                                        else if (sessionStorage.OfflineType == 'inventories') {
                                            $('#divSalesAnalytics').show();
                                            $('#divOnPrintPOSSalesReport').hide();
                                            $('#divGroupSales').hide();
                                            $('#dvSalesTrends').hide();
                                            $('#divInventoryLevels').hide();
                                            $('#divMoniesAcc').hide();
                                            $('#divFranchiseRequest').hide();
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
                                            //$('.ItemsBreadCrumbs').text('Inventories Accountability');
                                            AddBreadCrumb({
                                                UpdateLastchild: 1,
                                                text: 'Inventories Accountability', Click: function () {
                                                }
                                            }, '.ItemsBreadCrumbs')
                                        }
                                        else if (sessionStorage.OfflineType == 'transaction') {
                                            $('#divSalesAnalytics, #divOfflineTransactionHistoryMain, #divFranchiseRequest, #divOnPrintPOSSalesReport').hide();
                                            ShowOfflineTransactionHistory();
                                        }
                                        else if (sessionStorage.OfflineType == 'inventoryledger') {
                                            sessionStorage.InventoryBalanceProductName = '';
                                            $('#divSalesAnalytics, #divOfflineTransactionHistoryMain, #divOnPrintPOSSalesReport').hide();

                                            AjaxSend({
                                                ExactUrl: '/InventoryLedger?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=1',
                                                type: 'GET',
                                                dataType: 'text',
                                                success: function (vData) {
                                                    sessionStorage["Inventory"] = "ledger";
                                                    $('#divFranchiseRequest').empty();
                                                    $('#divFranchiseRequest').append(vData).trigger('create');

                                                    $('.mainContentForDashboard').css('margin-top', $('#divHeaderForDashboard').height());
                                                    isInventoryLedgerDoneClicked = true;
                                                    App.Deferred.fulfill('FQ26E5TP76');
                                                }
                                            });

                                            if (isInventoryLedgerDoneClicked) {
                                                InventoryLedger.CheckSource();
                                            }

                                            $('#divFranchiseRequest').show();
                                            $('#panReportTypeOption').popup('close');
                                            $('#txtReportType').text('Inventory Ledger');
                                            //$('.ItemsBreadCrumbs').text('Inventory Ledger');
                                            AddBreadCrumb({
                                                UpdateLastchild: 1,
                                                text: 'Inventory Ledger', Click: function () {
                                                }
                                            }, '.ItemsBreadCrumbs')
                                        }
                                        else if (sessionStorage.OfflineType == 'inventorycounthistory') {
                                            sessionStorage.InventoryBalanceProductName = '';
                                            $('#divSalesAnalytics, #divOfflineTransactionHistoryMain, #divOnPrintPOSSalesReport').hide();

                                            AjaxSend({
                                                ExactUrl: '/InventoryCount/InventoryCountHistoryOffline?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=1',
                                                type: 'GET',
                                                dataType: 'text',
                                                success: function (vData) {
                                                    sessionStorage["Inventory"] = "inventorycounthistory";
                                                    $('#divFranchiseRequest').empty();
                                                    $('#divFranchiseRequest').append(vData).trigger('create');

                                                    $('.mainContentForDashboard').css('margin-top', $('#divHeaderForDashboard').height());
                                                    isInventoryLedgerDoneClicked = true;
                                                    App.Deferred.fulfill('FQ26E5TP76');
                                                }
                                            });

                                            if (isInventoryLedgerDoneClicked) {
                                                InventoryLedger.CheckSource();
                                            }

                                            $('#divFranchiseRequest').show();
                                            $('#panReportTypeOption').popup('close');
                                            $('#txtReportType').text('Inventory Ledger');
                                            //$('.ItemsBreadCrumbs').text('Inventory Ledger');
                                            AddBreadCrumb({
                                                UpdateLastchild: 1,
                                                text: 'Inventory Ledger', Click: function () {
                                                }
                                            }, '.ItemsBreadCrumbs')
                                        }
                                        else if (sessionStorage.OfflineType == 'zreport') {
                                            $('#divSalesAnalytics').hide();
                                            $('#divGroupSales').hide();
                                            $('#dvSalesTrends').hide();
                                            $('#divInventoryLevels').hide();
                                            $('#divInventoriesAcc').hide();
                                            $('#divFranchiseRequest').hide();
                                            $('#divMoniesAcc').hide();
                                            $('.btnPeriod').hide();

                                            AccessForInventoryOfPayment('Z Report', function () {
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
                                            })                                            

                                            $('#divOnPrintPOSSalesReport').show();

                                            $('#panReportTypeOption').popup('close');
                                            //$('.ItemsBreadCrumbs').text('Z Report');
                                            AddBreadCrumb({
                                                UpdateLastchild: 1,
                                                text: 'Z Report', Click: function () {
                                                }
                                            }, '.ItemsBreadCrumbs')

                                            //settimeout(function () {
                                            //    $('#popposreportzreport').trigger('click');
                                            //}, 500)

                                            //sessionstorage.printingofflinedata = false; //para sa mobiz server sya magkuha ug data instead na sa device
                                        }
                                        else if (sessionStorage.OfflineType == 'cashierSalesReportFromEnforeZRead') {

                                            $('#divSalesAnalytics, #divOfflineTransactionHistoryMain, #divFranchiseRequest, #divOnPrintPOSSalesReport').hide();
                                            sessionStorage.PrintingOfflineData = false; //para sa mobiz server sya magkuha ug data instead na sa device
                                            GetCashierSalesReportPage(0);
                                            //$('.ItemsBreadCrumbs').text('Cashier\'s Sales Report');
                                            AddBreadCrumb({
                                                UpdateLastchild: 1,
                                                text: 'Cashier\'s Sales Report', Click: function () {
                                                }
                                            }, '.ItemsBreadCrumbs')
                                        }
                                        else if (sessionStorage.OfflineType == 'managerSalesReportFromEnforeZRead') {

                                            $('#divSalesAnalytics, #divOfflineTransactionHistoryMain, #divFranchiseRequest, #divOnPrintPOSSalesReport').hide();
                                            sessionStorage.PrintingOfflineData = false; //para sa mobiz server sya magkuha ug data instead na sa device
                                            GetCashierSalesReportPage(1);
                                            //$('.ItemsBreadCrumbs').text('Manager\'s Sales Report');
                                            AddBreadCrumb({
                                                UpdateLastchild: 1,
                                                text: 'Manager\'s Sales Report', Click: function () {
                                                }
                                            }, '.ItemsBreadCrumbs')
                                        }
                                        else if (sessionStorage.OfflineType == 'smcoinreport') {
                                            $('#divSalesAnalytics, #divOfflineTransactionHistoryMain, #divFranchiseRequest, #divOnPrintPOSSalesReport').hide();
                                            sessionStorage.PrintingOfflineData = false; //para sa mobiz server sya magkuha ug data instead na sa device
                                            GetSMCOINReportPage();
                                            //$('.ItemsBreadCrumbs').text('SM COIN Report');
                                            AddBreadCrumb({
                                                UpdateLastchild: 1,
                                                text: 'SM COIN Report', Click: function () {
                                                }
                                            }, '.ItemsBreadCrumbs')
                                        }
                                        else if (sessionStorage.OfflineType == 'salesbyskureport') {
                                            $('#divSalesAnalytics, #divOfflineTransactionHistoryMain, #divFranchiseRequest, #divOnPrintPOSSalesReport').hide();
                                            sessionStorage.PrintingOfflineData = false; //para sa mobiz server sya magkuha ug data instead na sa device
                                            SalesbySKUReport();
                                            //$('.ItemsBreadCrumbs').text('Sales by SKU Report');
                                            AddBreadCrumb({
                                                UpdateLastchild: 1,
                                                text: 'Sales by SKU Report', Click: function () {
                                                }
                                            }, '.ItemsBreadCrumbs')
                                        }
                                    } else {
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

                                        GetOrganizationName();

                                        AjaxSend({
                                            url: 'DashboardsAndReports/GetPageAccess',
                                            data: mdata,
                                            success: function (vData) {
                                                if (CBool(vData.Results)) {
                                                    sessionStorage.IsShowFirstPage = 'true'; //TODO                                                    

                                                    AjaxSend({
                                                        url: 'DashboardsAndReports/IsViewAllMainPageInformation',
                                                        success: function (isviewall) {
                                                            sessionStorage.IsViewAllInfo = CBool(isviewall.Results);
                                                            if (!CBool(isviewall.Results)) {
                                                                AjaxSend({
                                                                    url: 'DashboardsAndReports/IsGroupHead',
                                                                    success: function (isgrouphead) {                                                                        
                                                                        if (CBool(isgrouphead.Results)) {                                                                            
                                                                            $('#divGroupSalesReport').removeClass('thirtyseventy-block-b').css('width', '100%').show();
                                                                            $('#divAllSalesReport').hide();
                                                                            $('#divMainDashboard').show();
                                                                            if (location.hash != '#RequestAndApprovals')
                                                                                //$('#panOption').popup('open');
                                                                                $('#liToday').click();
                                                                        } else {
                                                                            SecondPage();
                                                                        }
                                                                    }
                                                                })
                                                            } else {
                                                                $('#divMainDashboard, #divGroupSalesReport, #divAllSalesReport').show();

                                                                if (location.hash != '#RequestAndApprovals')
                                                                    //$('#panOption').popup('open');
                                                                    $('#liToday').click();
                                                            }
                                                        }
                                                    })
                                                } else {
                                                    SecondPage();
                                                }                                                
                                            }
                                        });

                                    }
                                }

                                function SecondPage() {
                                    if (IsMobile) {
                                        var accessrights = security_offline().CheckSecurityCurrentUserAcccess({
                                            ModuleName: 'Input of Inventory of Payments is Required Prior to Showing Dashboards and On Print Reports?',
                                            UserID: sessionStorage.PeopleID
                                        }).Rows

                                        if (accessrights.length > 0) {
                                            if (CBool(accessrights[0].IsHasAccess) && CStr2(GetUrlParameters().InvetoryOfPaymentDone) != '1') {
                                                NewMessageBox('Before you can view your report, you are required to count or turnover your cash. What would you like to do?', 'Message', [
                                                    {
                                                        Text: 'Turnover / Deposit money', Click: function () {
                                                            window.location = '/CashHandling/DepositMonies?IsMobile=1&type=turnover&viewreport=1'
                                                        }
                                                    }, {
                                                        Text: 'Cash count only', Click: function () {
                                                            window.location = '/CashHandling/DepositMonies?IsMobile=1&type=cashcountonly&viewreport=1'
                                                        }
                                                    }
                                                ])
                                                return;
                                            }
                                        }
                                    }

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

                                    //for erp
                                    $('#selRevenueDashboard option[value="Revenue Dashboard"]').remove()

                                    if (location.hash != '#RequestAndApprovals')
                                        //$('#panOption').popup('open');
                                        $('#liToday').click();
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
                                    $('#divMainDashboard, #divSalesAnalytics, #divRequestAndApprovals, #divCacMcr,' +
                                            '#divOnPrintPOSSalesReport,' +
                                            '#divSearchResult, #divSpecialReports, #divAuditTrail, #divSettings, #divSalesReplenishmentProcessing, #panTransactionDetails,#divFranchiseRequest').hide();

                                    $('#divSalesOrderProcessing').hide();
                                    $('#pageOrderStatus').hide();
                                    $('#pageOrderCentral').show();

                                    //$('.ItemsBreadCrumbs').text('');

                                    if (CBool(sessionStorage.PrintingOfflineData)) {
                                        //RemoveBreadCrumb('Main Dashboard', '.ItemsBreadCrumbs')
                                    }

                                    AddBreadCrumb({
                                        UpdateLastchild: 1,
                                        text: 'Offline Transaction History', Click: function () {
                                        }
                                    }, '.ItemsBreadCrumbs')

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
                                $('#ForegoneVAT').text('-');
                                $('#TotalRefunds').text('-');
                                $('#TotalExchanges').text('-');
                                $('#lblSalesTransactionCount').text('-');
                                $('#lblSalesReturnsTransactionCount').text('-');
                                //$('#TotalFowstCount').text('-');

                                //2nd page
                                $('#lblGroupNetSales').text('-');
                                $('#lblGroupGrossSales').text('-');
                                $('#lblGroupDiscounts').text('-');
                                $('#lblGroupForegoneVAT').text('-');
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
                                        ;
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
                                mdata.FilterTrans = ($('#chbTransViewOpt').is(':checked') == true ? 'Yes' : 'No');

                                AjaxSend({
                                    url: 'DashboardsAndReports/GetPOSSalesReportByTrendOrSalesDistribution',
                                    data: mdata,
                                    success: function (vData) {
                                        if ($('#chbTransViewOpt').is(':checked')) {
                                            DisplayTransactionDetailsSKU(vData.ReportPOS.Rows);
                                        }
                                        else {
                                            DisplayTransactionDetails(vData.ReportPOS.Rows);
                                            if (vData.ReportPOS.Rows.length > 0) {
                                                GetSeniorCitizenPWDInfo(mdata);
                                            }
                                        }

                                        //m_SalesReturnData.IsSearch = 0;
                                    }
                                });
                            }

                            window.GetPOSSalesInvoiceHeader = function (mdata) {
                                mdata.IsRawData = 1;

                                GetSalesReturnsDetails(mdata);
                                AjaxSend({
                                    url: 'DashboardsAndReports/GetPOSSalesInvoiceHeader',
                                    data: mdata,
                                    success: function (vData) {
                                        $('#divTransHeader, #panTransactionDetails').show();
                                        $('#lblSalesTransactionDate').text($.format.date(new Date(mdata.From), 'MMM dd, yyyy') + '-' + $.format.date(new Date(mdata.To), 'MMM dd, yyyy'));
                                        $('#lblFOWST').text($('#lblSalesTrend').text().split('for ').pop());
                                        $(window).scrollTop(0);
                                        DisplayTransactionHeader(vData.ReportPOS.Rows, mdata);
                                        DisplayCashiers(vData.POSCashier.Rows)
                                        var dblTotalSales = 0;
                                        var dblTotalDeductive = 0;
                                        var dblTotalAdditive = 0;
                                        var NetSales = 0;
                                        var dblCash = 0;
                                        var dblCheck = 0;
                                        var dblDebitCard = 0;
                                        var dblCreditCard = 0;
                                        var dblSRV = 0;
                                        var dblOnCredit = 0;
                                        var dblOtherPaymentType = 0;
                                        var vCashiers;
                                        var lstCashiersSales = '';
                                        $('#dvSalesByCashier').css('display', 'none');
                                        if (mdata.CashierID != undefined) {
                                            if (mdata.CashierID != '') {
                                                vRows = vData.POSCashierSalesReport.Rows;
                                            } else {
                                                //manager sales report
                                                vRows = vData.POSManagerSalesReport.Rows;
                                                vCashiers = vData.POSManagerSalesReportByCashier.Rows;
                                                for (var i = 0; i <= vCashiers.length - 1; i++) {
                                                    lstCashiersSales += vCashiers[i].CashierName + ': ' + AccountingFormat((vCashiers[i].TotalSales || 0)) + '| '
                                                }
                                                $('#dvSalesByCashier').css('display', 'block');
                                            }
                                        } else {
                                            //manager sales report
                                            vRows = vData.POSManagerSalesReport.Rows;
                                            vCashiers = vData.POSManagerSalesReportByCashier.Rows;
                                            for (var i = 0; i <= vCashiers.length - 1; i++) {
                                                lstCashiersSales += vCashiers[i].CashierName + ': ' + AccountingFormat((vCashiers[i].TotalSales || 0)) + '| '
                                            }
                                            $('#dvSalesByCashier').css('display', 'block');
                                        }
                                        for (var p = 0; p <= vRows.length - 1; p++) {
                                            dblTotalSales += CDec2(vRows[p].CurGrossSales);
                                            dblTotalDeductive += CDec2(vRows[p].CurSalesDiscount) + CDec2(vRows[p].CurSCDiscount) + CDec2(vRows[p].CurSalesReturnRefund) + CDec2(vRows[p].CurSalesReturnExchange) + CDec2(vRows[p].CurVATExempt)
                                            dblTotalAdditive += CDec2(vRows[p].ChargeAmount);
                                            NetSales = dblTotalSales + dblTotalAdditive - dblTotalDeductive;
                                            dblCash += parseFloat(vRows[p].CashAmount || 0);
                                            dblCheck += parseFloat(vRows[p].CheckPaymentAmount || 0);
                                            dblDebitCard += parseFloat(vRows[p].DebitCardAmount || 0);
                                            dblCreditCard = parseFloat(vRows[p].CreditCardAmount || 0);
                                            dblSRV += parseFloat(vRows[p].SRVAmount || 0);
                                            dblOnCredit += parseFloat(vRows[p].OnCreditAmount || 0);
                                            dblOtherPaymentType += parseFloat(vRows[p].OtherTypeAmount || 0);
                                        }
                                        //cashier sales report
                                        $('#lblTotalGrossSales').text(AccountingFormat(dblTotalSales || 0));
                                        $('#lblTotalDeductiveAdjustments').text(AccountingFormat(dblTotalDeductive));
                                        $('#lblTotalAdditiveAdjustments').text(AccountingFormat(dblTotalAdditive || 0));
                                        $('#lblNetSales').text(AccountingFormat(NetSales));
                                        $('#lblTotalCash').text(AccountingFormat(dblCash));
                                        $('#lblTotalCheck').text(AccountingFormat(dblCheck));
                                        $('#lblTotalDebitCard').text(AccountingFormat(dblDebitCard));
                                        $('#lblCreditCard').text(AccountingFormat(dblCreditCard));
                                        $('#lblTotalSRV').text(AccountingFormat(dblSRV));
                                        $('#lblTotalOnCredit').text(AccountingFormat(dblOnCredit));
                                        $('#lblTotalOtherPayments').text(AccountingFormat(dblOtherPaymentType));
                                        $('#lblSalesByCashier').text(lstCashiersSales);
                                    }
                                });
                            }

                            var dataTableTransactionHeader;
                            function DisplayTransactionHeader(vRows, mdata) {
                                if ($.fn.DataTable.isDataTable($('#tblTransactionHeader'))) {
                                    //empty the table first before destroying datatabase para dili magdoble ang header
                                    $('#tblTransactionHeader').empty();
                                    dataTableTransactionHeader.destroy();
                                }

                                var buttons = [];
                                if (!IsMobile) {
                                    buttons.push({ extend: 'csvHtml5', text: 'Download CSV', title: 'Invoices' })
                                }

                                var ctr = 0;
                                dataTableTransactionHeader = $('#tblTransactionHeader').DataTable({
                                    pageLength: 25,
                                    "ordering": false,
                                    "data": vRows,
                                    "columns": [
                                        //{
                                        //    "data": function (data) {
                                        //        ctr += 1;
                                        //        return ctr + '.';
                                        //    }
                                        //},
                                        {
                                            "data": function (data) {
                                                return CStr2($.format.date(new Date(data.InvoiceDate), 'MMM dd, yyyy hh:mm a'))
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return CStr2(data.InvoiceNum);
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return CStr2(data.CompanyName);
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return AccountingFormat(data.Total)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return CDec2(data.SkuCount)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return CStr2(data.SalesRep)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return AccountingFormat(data.CashAmount)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return AccountingFormat(data.CreditCardAmount)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return AccountingFormat(data.DebitCardAmount)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return AccountingFormat(data.CheckPaymentAmount)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return AccountingFormat(data.SRVAmount)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return AccountingFormat(data.OnCreditAmount)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return AccountingFormat(data.OtherPayments)
                                        }
                                        },
                                        {
                                            "data": function (data) {
                                                return data.PaymentType;
                                            }
                                        },
                                        { "data": "Cashier" }
                                    ],
                                    dom: 'Brtip',
                                    buttons: buttons
                                });

                                dataTableTransactionHeader.on('click', 'td', function () {
                                    var data = dataTableTransactionHeader.row(this).data();
                                    if (data != undefined) {
                                        mdata.InvoiceID = data.ID;
                                        $('#popTransDetails').show();
                                        GetTransactionDetails(mdata)
                                    }
                                });

                                return;
                                //modified by arnie 52717 - to be removed
                                $('#tblTransactionHeader').empty();

                                if (vRows.length > 0) {
                                    for (var i = 0; i < vRows.length; i++) {
                                        $('#tblTransactionHeader').append($('<tr>')
                                            .append($('<td>')
                                                .text(i + 1 + '.')
                                            )
                                            .append($('<td>')
                                                .text($.format.date(new Date(vRows[i].InvoiceDate), 'MMM dd, yyyy hh:mm a'))
                                            )
                                            .append($('<td>')
                                                .text(vRows[i].InvoiceNum)
                                            )
                                            .append($('<td>')
                                                .text(vRows[i].CompanyName)
                                            )
                                            .append($('<td>')
                                                .css('text-align', 'right')
                                                .text(AccountingFormat(vRows[i].Total))
                                            )
                                            .append($('<td>')
                                                .css('text-align', 'right')
                                                .text(vRows[i].SkuCount)
                                            )
                                            .append($('<td>')
                                                .text((vRows[i].SalesRep || ''))
                                            )
                                            .append($('<td>')
                                                .css('text-align', 'right')
                                                .text(AccountingFormat(vRows[i].CashAmount))
                                            )
                                            .append($('<td>')
                                                .css('text-align', 'right')
                                                .text(AccountingFormat(vRows[i].CreditCardAmount))
                                            )
                                            .append($('<td>')
                                                .css('text-align', 'right')
                                                .text(AccountingFormat(vRows[i].DebitCardAmount))
                                            )
                                            .append($('<td>')
                                                .css('text-align', 'right')
                                                .text(AccountingFormat(vRows[i].CheckPaymentAmount))
                                            )
                                            .append($('<td>')
                                                .css('text-align', 'right')
                                                .text(AccountingFormat(vRows[i].SRVAmount))
                                            )
                                            .append($('<td>')
                                                .css('text-align', 'right')
                                                .text(AccountingFormat(vRows[i].OnCreditAmount))
                                            )
                                            .css('cursor', 'pointer')
                                            .data('data', vRows[i])
                                            .click(function () {
                                                mdata.InvoiceID = $(this).data('data').ID;
                                                $('#popTransDetails').show();
                                                GetTransactionDetails(mdata);
                                            })
                                        )
                                    }
                                }
                                else {
                                    $('#tblTransactionHeader').append($('<tr>')
                                        .append($('<td>')
                                            .attr('colspan', 15)
                                            .css('text-align', 'center')
                                            .text('No Result(s) Found')
                                        )
                                    )
                                }

                                $('#panTransactionDetails').show();

                                //$('#divMainDashboard').hide();
                                //$('#divSalesAnalytics,' +
                                //    '#divOnPrintPOSSalesReport,' +
                                //    '#divSearchResult, #divSpecialReports, #divAuditTrail, #divSettings, #divSalesOrderProcessing, #divSalesReplenishmentProcessing').hide();

                                //$('.ItemsBreadCrumbs').text('');
                                //breadcrumbs.prepend(arrowR, 'View Transaction Details');
                                //if (CBool(sessionStorage.SalesByOrg)) {
                                //    breadcrumbs.prepend(arrowR, '<a id="btnGroupReports">Reports for ' + sessionStorage.GroupName + '</a>');
                                //}
                                //else {
                                //    breadcrumbs.prepend(arrowR, '<a id="btnGroupReports">Group Reports for ' + sessionStorage.GroupName + ' Group </a>');
                                //}
                                //$('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                                //BreadCrumbsClickEvent();

                                //('#tblTransactionHeader').table('refresh');
                            }

                            function DisplayCashiers(vRows) {
                                selCashiers = $('#selCashier')
                                selCashiers.empty();
                                selCashiers.append(new Option("-Select Cashier-", "", false));
                                selCashiers.append(new Option("All Cashiers", "", true));
                                for (var i = 0; i <= vRows.length - 1; i++) {
                                    selCashiers.append(new Option(vRows[i].Cashier, vRows[i].UserID, false));
                                }
                            }
                            var dataTableTransactionDetails;
                            function DisplayTransactionDetails(vRows) {
                                if ($.fn.DataTable.isDataTable($('#tblTransactionDetailsBody'))) {
                                    //empty the table first before destroying datatabase para dili magdoble ang header
                                    $('#tblTransactionDetailsBody').empty();
                                    dataTableTransactionDetails.destroy();
                                }

                                var buttons = [], invno;
                                if (!IsMobile) {
                                    buttons.push({ extend: 'csvHtml5', text: 'Download CSV', title: 'Details for Invoice No ' + invno })
                                }

                                var ctr = 0;
                                dataTableTransactionDetails = $('#tblTransactionDetailsBody').DataTable({
                                    pageLength: 25,
                                    "ordering": false,
                                    "data": vRows,
                                    "columns": [
                                        {
                                            "data": function (data) {
                                                return CStr2(data.ProductDescription);
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return CDec2(data.Qty)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return CDec2(data.QuantityReturned)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return CStr2(data.UnitMeasure)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return CStr2(data.RebatesProduct)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return AccountingFormat((data.QtyRebates * data.Qty))
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return CStr2(data.UnitMeasureRebates)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return AccountingFormat(data.Price)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return AccountingFormat(data.SubTotal)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return AccountingFormat(data.Discount)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return CStr2(data.Customer)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return CStr2(data.UpdatedByName)
                                            }
                                        }
                                    ],
                                    createdRow: function (row, data, index) {
                                        var hasReturned = { backgroundColor: '' };
                                        if (data.QuantityReturned != undefined) {
                                            hasReturned = { backgroundColor: '#FFFCB2' }
                                        }
                                        $('#txtInvoiceDate').text(data.InvoiceDate)
                                        $('#txtInvoiceNumber').text(data.InvoiceNo)
                                        $('#txtSalesRep').text((data.SalesRepresentative || ''))
                                        $(row).css(hasReturned);
                                        invno = data.InvoiceNo;
                                    },
                                    dom: 'Brtip',
                                    buttons: buttons
                                });
                                $('#panTransactionDetails').show();
                                return;
                                //modified by arnie 52717 - to be removed
                                var pLV = $('#tblTransactionDetailsBody');

                                $('#tblTransactionDetailsBody').empty();
                                //$('#lstTransactionDetails li').not(':eq(0)').not(':eq(0)').remove();

                                if (vRows.length > 0) {
                                    for (var i = 0; i <= vRows.length - 1; i++) {
                                        var hasReturned = { backgroundColor: '' };
                                        if (vRows[i].QuantityReturned != undefined) {
                                            hasReturned = { backgroundColor: '#FFFCB2' }
                                        }
                                        $('#txtInvoiceDate').text(vRows[i].InvoiceDate)
                                        $('#txtInvoiceNumber').text(vRows[i].InvoiceNo)
                                        $('#txtSalesRep').text((vRows[i].SalesRepresentative || ''))

                                        pItem = $('<tr>')
                                            .css(hasReturned)
                                        //                                                .append($('<td>')
                                        //                                                    .text(vRows[i].InvoiceNo)
                                        //                                                )
                                        //                                                .append($('<td>')
                                        //                                                    .text(vRows[i].InvoiceDate)
                                        //                                                )
                                            .append($('<td>')
                                                .text(vRows[i].ProductDescription)
                                            )
                                            .append($('<td style="text-align: center;">')
                                                .text(AccountingFormat(vRows[i].Qty))
                                            )
                                            .append($('<td style="text-align: center;">')
                                                .text(AccountingFormat(vRows[i].QuantityReturned))
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
                                        //                                                .append($('<td>')
                                        //                                                    .text(vRows[i].SalesRepresentative)
                                        //                                                )
                                            .append($('<td>')
                                                .text(vRows[i].Customer)
                                            )
                                            .append($('<td>')
                                                .text(vRows[i].UpdatedByName)
                                            );
                                        pLV.append(pItem);
                                    }
                                }
                                else {
                                    pLV.append($('<tr>')
                                        .append($('<td>')
                                            .attr('colspan', 15)
                                            .css('text-align', 'center')
                                            .text('No Result(s) Found')
                                        )
                                    )
                                }
                                //            try {
                                //                $('#lstFOWSTFrame').listview('refresh');
                                //            }
                                //            catch (e) {
                                //                $('#lstFOWSTFrame').listview();
                                //            }

                                //            $('#panTransactionDetails').popup('open');
                                $('#panTransactionDetails').show();


                                //$('#divMainDashboard').hide();
                                //$('#divSalesAnalytics,' +
                                //    '#divOnPrintPOSSalesReport,' +
                                //    '#divSearchResult, #divSpecialReports, #divAuditTrail, #divSettings, #divSalesOrderProcessing, #divSalesReplenishmentProcessing').hide();

                                //            $('#panGoTo, #panOption').popup('close');
                                //
                                pLV.parent().table('refresh');
                            }

                            function GetSeniorCitizenPWDInfo(mdata) {
                                AjaxSend({
                                    url: 'DashboardsAndReports/GetSeniorCitizenPWDInfo',
                                    data: mdata,
                                    success: function (vData) {
                                        if (vData.Results.Rows.length > 0) {
                                            $('#dvSeniorCitizen').css('visibility', 'visible');
                                            $('#dvSeniorCitizen').css('display', 'block');
                                            DisplaySeniorCitizenPWDInfo(vData.Results.Rows);
                                        } else {
                                            $('#dvSeniorCitizen').css('visibility', 'hidden');
                                            $('#dvSeniorCitizen').css('display', 'none');
                                        }
                                    }
                                });
                            }

                            var dataTableDisplaySeniorCitizenPWDInfo;

                            function DisplaySeniorCitizenPWDInfo(vRows) {
                                var pBody = $('#tbodySeniorCitizen')

                                pBody.empty();

                                for (var i = 0; i <= vRows.length - 1; i++) {
                                    pItem = $('<tr>')                                           
                                           .append($('<td>')
                                               .text(vRows[i].Name)
                                           )
                                           .append($('<td>')
                                               .data(vRows)
                                               .html("<img src='Content/MobizPeople/" + vRows[i].IDImage + "' height=50px width=50px />")
                                               .click(function () {
                                                   var photo;
                                                   var FullName = $(this).data()[0].Name;
                                                   var val = "Content/MobizPeople/" + $(this).data()[0].IDImage;

                                                   $('#lblSCFullName').text(FullName);

                                                   $('#imgSCIDImage').empty();

                                                   $('#imgSCIDImage').append(
                                                       "<img src=" + val + " style='width: 400px; height: 400px'>"
                                                       )

                                                   $('#divImage').show();
                                               })
                                           )

                                    pBody.append(pItem);
                                }
                            }

                            var dataTableTransactionDetailsSKU;
                            function DisplayTransactionDetailsSKU(vRows) {
                                if ($.fn.DataTable.isDataTable($('#tblTransactionDetails'))) {
                                    //empty the table first before destroying datatabase para dili magdoble ang header
                                    $('#tblTransactionDetails').empty();
                                    dataTableTransactionDetailsSKU.destroy();
                                }

                                var buttons = [];
                                if (!IsMobile) {
                                    buttons.push({ extend: 'csvHtml5', text: 'Download CSV', title: 'Inv Details' })
                                }

                                var ctr = 0;
                                dataTableTransactionDetailsSKU = $('#tblTransactionDetails').DataTable({
                                    pageLength: 25,
                                    "ordering": false,
                                    "data": vRows,
                                    "columns": [
                                        {
                                            "data": function (data) {
                                                return CStr2(data.InvoiceNo)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return CStr2($.format.date(new Date(data.InvoiceDate), 'MMM dd, yyyy hh:mm a'));
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return CStr2(data.ProductDescription);
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return CDec2(data.Qty)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return CDec2(data.QuantityReturned)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return CStr2(data.UnitMeasure)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return CStr2(data.RebatesProduct)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return AccountingFormat((data.QtyRebates * data.Qty))
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return CStr2(data.UnitMeasureRebates)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return AccountingFormat(data.Price)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return AccountingFormat(data.SubTotal)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return AccountingFormat(data.Discount)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return CStr2(data.SalesRepresentative)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return CStr2(data.Customer)
                                            }
                                        },
                                        {
                                            "data": function (data) {
                                                return CStr2(data.UpdatedByName)
                                            }
                                        }
                                    ],
                                    createdRow: function (row, data, index) {
                                        var hasReturned = { backgroundColor: '' };
                                        if (data.QuantityReturned != undefined) {
                                            hasReturned = { backgroundColor: '#FFFCB2' }
                                        }
                                        $('#txtInvoiceDate').text(data.InvoiceDate)
                                        $('#txtInvoiceNumber').text(data.InvoiceNo)
                                        $('#txtSalesRep').text((data.SalesRepresentative || ''))
                                        $(row).css(hasReturned);
                                        invno = data.InvoiceNo;
                                    },
                                    dom: 'Brtip',
                                    buttons: buttons
                                });

                                return;
                                //modified by arnie 52717 - to be removed
                                var pLV = $('#tblTransactionDetails');

                                pLV.empty();

                                if (vRows.length > 0) {
                                    for (var i = 0; i <= vRows.length - 1; i++) {
                                        var hasReturned = { backgroundColor: '' };
                                        if (vRows[i].QuantityReturned != undefined) {
                                            hasReturned = { backgroundColor: '#FFFCB2' }
                                        }
                                        $('#txtInvoiceDate').text(vRows[i].InvoiceDate)
                                        $('#txtInvoiceNumber').text(vRows[i].InvoiceNo)
                                        $('#txtSalesRep').text((vRows[i].SalesRepresentative || ''))

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
                                            .append($('<td style="text-align: center;">')
                                                .text(AccountingFormat(vRows[i].QuantityReturned))
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
                                }
                                else {
                                    pLV.append($('<tr>')
                                        .append($('<td>')
                                            .attr('colspan', 15)
                                            .css('text-align', 'center')
                                            .text('No Result(s) Found')
                                        )
                                    )
                                }
                                //            try {
                                //                $('#lstFOWSTFrame').listview('refresh');
                                //            }
                                //            catch (e) {
                                //                $('#lstFOWSTFrame').listview();
                                //            }

                                //            $('#panTransactionDetails').popup('open');
                                $('#panTransactionDetails').show();


                                //$('#divMainDashboard').hide();
                                //$('#divSalesAnalytics,' +
                                //    '#divOnPrintPOSSalesReport,' +
                                //    '#divSearchResult, #divSpecialReports, #divAuditTrail, #divSettings, #divSalesOrderProcessing, #divSalesReplenishmentProcessing').hide();

                                //            $('#panGoTo, #panOption').popup('close');
                                //
                                pLV.parent().table('refresh');
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

                                if (vRows.length > 0) {
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
                                            .append($('<td style="text-align: right;">')
                                                .text(vRows[i].Cashier)
                                            )

                                        pLV.append(pItem);

                                    }
                                }
                                else {
                                    pLV.append($('<tr>')
                                        .append($('<td>')
                                            .attr('colspan', 15)
                                            .css('text-align', 'center')
                                            .text('No Result(s) Found')
                                        )
                                    )
                                }

                                $('#lstFOWSTFrame').listview('refresh');
                                pLV.parent().table('refresh');
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
                                GetOrganizationName();
                                AjaxSend({
                                    url: 'DashboardsAndReports/GetPageAccess',
                                    data: mdata,
                                    success: function (vData) {
                                        if (CBool(vData.Results)) {
                                            sessionStorage.IsShowFirstPage = 'true'; //TODO
                                            sessionStorage.IsFromDashboardPage = true;

                                            GetTotalGrossAndDiscounts();
                                            GetFOWSTCount();

                                            data.GroupBy = 'ByOrganizationGroup';
                                            //GetTotalGrossAndDiscountsByGROUP();
                                            //GetPageAccess();
                                        } else {
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
                                        }
                                    }
                                });
                            }
                        });
                });
        });

    function GetReceiptPrintedStatus(vData) {
        AjaxSend({
            url: '/DashboardsAndReports/GetReceiptPrintedStatus',
            data: vData,
            success: function (vData) {
                DisplayReceiptPrintedStatus(vData.Result.Rows);
            }
        }); // END AjaxSend
    } // END function GetReceiptPrintedStatus(vData) {

    function DisplayReceiptPrintedStatus(vData) {
        var pTable = $('#tableReceiptPrintedStatus');
        var pTrTemplate = pTable.find('tr[name=data]:first').clone();
        var pCount = 0;
        var pNetInvoiceAmount = 0;
        var pTR;

        pTable.find('tr[name=data]').remove();
        for (var p = 0; p <= vData.length - 1; p++) {
            var pDR = vData[p];
            pTR = pTrTemplate.clone();
            pTR.find('td[name=Status]').text(pDR.StatusText);
            pTR.find('td[name=Count]').text(AccountingFormat(pDR.Count, 0));
            pCount += CDec2(pDR.Count);
            pTR.find('td[name=NetInvoiceAmount]').text(AccountingFormat(pDR.NetInvoiceAmount));
            pNetInvoiceAmount += CDec2(pDR.NetInvoiceAmount);
            pTable.append(pTR);
        }

        pTR = pTable.find('tr[name=footer]:first');
        pTR.find('td[name=Count]').text(AccountingFormat(pCount, 0));
        pTR.find('td[name=NetInvoiceAmount]').text(AccountingFormat(pNetInvoiceAmount));
        pTR.remove();
        pTable.append(pTR);

    } // END DisplayReceiptPrintedStatus
});