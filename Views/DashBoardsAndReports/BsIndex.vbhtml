@Code
    ViewData("Title") = "Dashboards and Reports"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code

@Section head
    @*<link rel="stylesheet" href="/Content/css/jquery.jqplot.min.css" />
    <link rel="stylesheet" href="/Content/css/ui.dynatree.css" />
    <link rel="stylesheet" href="/Content/css/qunit.css" />
    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <link rel="stylesheet" href="/Content/bootstrap-custom.css" />
    <link rel="stylesheet" href="/Content/metro-calendar.css" />*@
    <link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
     
    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
                require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/Bs_DashboardsAndReportsCharts.js?crc=' + assetversion)], function () {
                    require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/Bs_SalesDashboardIndex.js?crc=' + assetversion)]);
                    require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/flatui.js?crc=')]);
                });                
            })
        })        
    </script>
    
    <style>    
     .jqplot-xaxis-hide {
            display: none
        }
    .btn-toggle-nav a {
      display: inline-flex;
      padding: .1875rem .5rem;
      margin-top: .125rem;
      margin-left: 2.25rem;
      text-decoration: none;
    }
    .btn-toggle-nav a:hover,
    .btn-toggle-nav a:focus,
    .btn-toggle-nav a.active {
      background-color: #1d2124;
    }

    .foronline {
        display:block;
    }

    .foroffline {
        display:none;
    }

    .fordevice {
        display:none;
    }

    .listpanelmenu > li{
        padding: 5px 0px;
    }

    .listpanelmenu > li:not(:last-child) {
        border-bottom: 1px solid #6c757d;        
    }

    .nav-menu:hover {
        background-color:#0d6efd;
    }

    .nav-menu > span {
        margin-right:10px;
    }

    #divMainContentForDashboard > div {
        width: 100%;
        padding: 1rem !important;
    }

        /*.es-blue {
            background-color: #00b0f0 !important;
        }*/

        .es-orange {
            background-color: #ffc000 !important;
        }

        .es-green {
            background-color: #92d050 !important;
        }
    </style>
End Section

<div id="divDashboardReportLoading" subpage="divDashboardReport" class="d-flex position-absolute w-100 h-100 justify-content-center align-items-center">
    <div class="divspinner text-center">
        <div class="spinner-border text-primary text-center" role="status">
            <span class="visually-hidden"></span>
        </div>
        <div>Preparing your dashboard ...</div>
    </div>
</div>

<table id="divDashboardReport" border="0" cellpadding="0" style="display:none; width:100%; height:100%; top:0; bottom:0; position:absolute;">
    <tr>
        <td class="divpanelmenu bg-dark">
            <div style="height:100vh; display:grid; grid-template-rows: 50px 1fr 0px;">
                <div class="row">
                    <div class="col">
                        <button data-role="none" class="btn btn-dark btn-block btnhome btn-lg rounded-0" style="height:100%;">
                            <span class="bi-house-fill"></span>
                            Home
                        </button>
                    </div>
                    <div class="col divclosepanel">
                        <button id="btnclosepanelmenu" data-role="none" class="btn btn-dark btn-block btn-lg rounded-0" style="height:100%;">
                            <span class="bi-x-circle"></span>
                            Close
                        </button>
                    </div>
                </div>
                <div style="flex:1; overflow:auto;">
                    <ul class="nav listpanelmenu nav-pills flex-column m-2">
                        <li class="nav-item">
                            <a href="#" data-role="none" class="nav-link nav-menu text-white active" data-toggle="collapse" data-target="#collapse-salesdashboard">
                                <span class="bi-speedometer2"></span>
                                Sales Dashboard
                            </a>
                            <div class="collapse show" id="collapse-salesdashboard" style="">
                                <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
                                    <li class="foronline"><a href="#" data-role="none" class="link-light active rounded btnMainDashboardPage"><span class="bi-dot"></span> Main Sales Dashboard</a></li>
                                    <li style="display:none;"><a href="#" data-role="none" id="btnNewDashboardPage" class="link-light rounded"><span class="bi-dot"></span> <span>MPOS Sales Dashboard <span class="text-warning">(Beta version)</span></span></a></li>
                                    <li class="foronline"><a href="#" data-role="none" id="btnCMSalesReport" class="link-light rounded"><span class="bi-dot"></span> Summarized Sales By Store</a></li>
                                    <li class="foronline"><a href="#" data-role="none" id="btnCMHourlySalesReport" class="link-light rounded"><span class="bi-dot"></span> Hourly Sales</a></li>
                                    <li class="foronline"><a href="#" data-role="none" id="btnCMMonthlySalesReport" class="link-light rounded"><span class="bi-dot"></span> Monthly Sales</a></li>
                                    <li><a href="#" data-role="none" id="popPOSReportSKUSalesReport" class="link-light rounded"><span class="bi-dot"></span> Sales By SKU</a></li>
                                    @*<li><a href="#" data-role="none" id="liOfflineProductHistory" class="link-light rounded"><span class="bi-dot"></span> Offline Product History</a></li>*@
                                    <li class="foroffline"><a href="#" data-role="none" id="liOfflineTransactionHistory" class="link-light rounded"><span class="bi-dot"></span> Offline Transaction History</a></li>
                                    @*<li><a href="#" data-role="none" class="link-secondary disabled rounded"><span class="bi-dot"></span> Sales Return > Sales Refunds (Voided Sales)</a></li>
                        <li><a href="#" data-role="none" class="link-secondary disabled rounded"><span class="bi-dot"></span> Sales Return > Merchandise Exchange</a></li>*@
                                </ul>
                            </div>
                        </li>
                        <li class="nav-item">
                            <a href="#" data-role="none" class="nav-link nav-menu text-white" data-toggle="collapse" data-target="#collapse-reports">
                                <span class="bi-printer-fill"></span>
                                On-Print POS Sales Report
                            </a>
                            <div class="collapse show" id="collapse-reports" style="">
                                <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
                                    <li><a href="#" data-role="none" id="popPOSReportCashierSalesReport" class="link-light rounded"><span class="bi-dot"></span> Cashier's Sales Report (X-Read)</a></li>
                                    @*<li><a href="#" data-role="none" id="popPOSReportManagerReport" class="link-light rounded"><span class="bi-dot"></span> Manager's Sales Report</a></li>*@
                                    <li><a href="#" data-role="none" id="popPOSReportZReport" class="link-light rounded"><span class="bi-dot"></span> Z-Read</a></li>
                                </ul>
                            </div>
                        </li>
                        <li class="nav-item">
                            <a href="#" data-role="none" class="nav-link nav-menu text-white" data-toggle="collapse" data-target="#collapse-cashhandling">
                                <span class="bi-cash-stack"></span>
                                Cash Flow Reports
                            </a>
                            <div class="collapse show" id="collapse-cashhandling" style="">
                                <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
                                    @*<li><a href="#" data-role="none" class="link-secondary disabled rounded"><span class="bi-dot"></span> Route and Expense Tracking</a></li>*@
                                    <li><a href="#" data-role="none" id="popPOSReportDeposit" class="link-light rounded"><span class="bi-dot"></span> Monies Accountability</a></li>
                                </ul>
                            </div>
                        </li>
                        @*<li class="nav-item">
                <a href="#" data-role="none" class="nav-link nav-menu text-secondary" data-toggle="collapse" data-target="#home-collapse">
                    <span class="bi-chevron-right"></span>
                    Credit and Collection
                </a>
            </li>*@
                        <li class="nav-item foronline">
                            <a href="#" data-role="none" class="nav-link nav-menu text-white btnReplenishmentProcessing" data-toggle="collapse" data-target="#home-collapse">
                                <span class="bi-graph-up"></span>
                                Store Replenishment Processing and Analytics
                            </a>
                        </li>
                        <li class="nav-item foronline">
                            <a href="#" data-role="none" class="nav-link nav-menu text-white btnSalesOrderProcessingAnalyticsPage" data-toggle="collapse" data-target="#home-collapse">
                                <span class="bi-graph-up"></span>
                                Sales Order Processing and Analytics
                            </a>
                        </li>
                        <li class="nav-item foronline">
                            <a href="#" data-role="none" id="butPendingSalesReturns" class="nav-link nav-menu text-light" data-toggle="collapse">
                                <span class="bi-hand-thumbs-up-fill"></span>
                                Request and Approvals
                            </a>
                        </li>
                        <li class="nav-item foronline">
                            <a href="#" data-role="none" class="nav-link nav-menu text-light btnTrackDevice" data-toggle="collapse">
                                <span class="bi-geo-alt-fill"></span>
                                GPS Location
                            </a>
                        </li>
                        <li class="nav-item">
                            <a href="#" data-role="none" class="nav-link nav-menu text-white" data-toggle="collapse" data-target="#collapse-inventory">
                                <span class="bi-clipboard-data"></span>
                                Inventory Reports and Analytics
                            </a>
                            <div class="collapse show" id="collapse-inventory" style="">
                                <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
                                    <li class="foroffline"><a href="#" data-role="none" id="btnInventoryCountHistory" class="link-light rounded"><span class="bi-dot"></span> Inventory Count History</a></li>
                                    <li class="foronline"><a href="#" data-role="none" class="link-light rounded btnMoreOptionInventoryBalance"><span class="bi-dot"></span> Inventory Balance</a></li>
                                    <li><a href="#" data-role="none" class="link-light rounded btnMoreOptionInventoryLedger"><span class="bi-dot"></span> Inventory Ledger</a></li>
                                    <li class="foronline"><a href="#" data-role="none" id="btnCMInventoryMovement" class="link-light rounded"><span class="bi-dot"></span> Inventory Movement</a></li>
                                    <li><a href="#" data-role="none" class="link-light rounded btnInventoryCounts"><span class="bi-dot"></span> Physical Inventory and Variance Report</a></li>
                                    <li class="foronline"><a href="#" data-role="none" class="link-light rounded btnInventoryLevel"><span class="bi-dot"></span> Stock Inventory Levels</a></li>
                                    <li class="foronline"><a href="#" data-role="none" class="link-light rounded btnMoreOptionStockReceivingAndTransfers"><span class="bi-dot"></span> Stock Receiving and Transfers</a></li>
                                    <li class="foronline"><a href="#" data-role="none" id="btnCMDeliveryReport" class="link-light rounded"><span class="bi-dot"></span> Stock Receiving (Delivery Receipt) Report</a></li>
                                </ul>
                            </div>
                        </li>
                        <li class="nav-item">
                            <a href="#" data-role="none" class="nav-link nav-menu text-white" data-toggle="collapse" data-target="#collapse-specialreports">
                                <span class="bi-star-fill"></span>
                                Special Reports
                            </a>
                            <div class="collapse show" id="collapse-specialreports" style="">
                                <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
                                    <li class="foronline"><a href="#" data-role="none" id="liPOSTransactionDetail" class="link-light rounded"><span class="bi-dot"></span> Audit Trail</a></li>
                                    <li class="foronline"><a href="#" data-role="none" id="liFranchiseRequests" class="link-light rounded"><span class="bi-dot"></span> Franchise Requests</a></li>
                                    <li class="foronline"><a href="#" data-role="none" id="popPOSReportEJournal" class="link-light rounded"><span class="bi-dot"></span> E-Journal Report</a></li>
                                    <li class="foronline"><a href="#" data-role="none" id="popPOSReportInventoryForm" class="link-light rounded"><span class="bi-dot"></span> Inventory Form Report</a></li>
                                    <li class="foronline"><a href="#" data-role="none" id="popPOSReportPrincipal" class="link-light rounded"><span class="bi-dot"></span> Report to Principal</a></li>
                                    <li class="foronline"><a href="#" data-role="none" id="popPOSReportSCDAndPWDReports" class="link-light rounded"><span class="bi-dot"></span> Discount Report</a></li>
                                    <li class="foronline"><a href="#" data-role="none" id="popPOSReportShopperTrak" class="link-light rounded"><span class="bi-dot"></span> ShopperTrak</a></li>
                                    <li class="foronline"><a href="#" data-role="none" id="btnCMSpecialDiscountReport" class="link-light rounded"><span class="bi-dot"></span> Special Discount Report</a></li>
				    <li class="foronline"><a href="#" data-role="none" id="btnCMSpecialPromoReport" class="link-light rounded"><span class="bi-dot"></span> Special Promo Report</a></li>
                                    <li class="foronline"><a href="#" data-role="none" id="popPOSSRVHistory" class="link-light rounded"><span class="bi-dot"></span> Sales Return History</a></li>
                                    <li class="foronline"><a href="#" data-role="none" id="popPOSReportMallLessorAyala" class="link-light rounded"><span class="bi-dot"></span> Ayala Malls Tenant Monitoring Report</a></li>
                                    <li><a href="#" data-role="none" id="popPOSReportMallLessorMegaworld" class="link-light rounded"><span class="bi-dot"></span> Tenant Monitoring Reports</a></li>
                                    <li><a href="#" data-role="none" id="popPOSReportMallLessorSM" class="link-light rounded"><span class="bi-dot"></span> SM Coin Report Tenant Monitoring Report</a></li>
                                    <li class="foronline"><a href="#" data-role="none" id="btnDeviceMonitoring" class="link-light rounded"><span class="bi-dot"></span> Device Monitoring </a></li>
                                    <li class="foronline"><a href="#" data-role="none" id="btnMPOSUtilization" class="link-light rounded"><span class="bi-dot"></span> MPOS Utilization </a></li>
                                    <li class="foronline"><a href="#" data-role="none" id="btnVoucherReport" class="link-light rounded"><span class="bi-dot"></span> Vouchers Report </a></li>
                                    <li class="foronline"><a href="#" data-role="none" id="btnVoidItemsReport" class="link-light rounded"><span class="bi-dot"></span> Void Items Report </a></li>
                                </ul>
                            </div>
                        </li>
			            <li class="nav-item">
	                        <a href="#" data-role="none" class="nav-link nav-menu text-white" data-toggle="collapse" data-target="#collapse-marketingreports">
	                            <span class="bi-star-fill"></span>
	                            Marketing Reports
	                        </a>
	                        <div class="collapse show" id="collapse-marketingreports" style="">
	                            <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
	                                <li class="foronline"><a href="#" data-role="none" id="liProductPerformanceTracker" class="link-light rounded"><span class="bi-dot"></span> Product Performance Tracker </a></li>
					<li class="foronline"><a href="#" data-role="none" id="liProductMix" class="link-light rounded"><span class="bi-dot"></span> Product Mix </a></li>
					<li class="foronline"><a href="#" data-role="none" id="liProductMixperWeek" class="link-light rounded"><span class="bi-dot"></span> Product Mix per Week</a></li>
                                	<li class="foronline"><a href="#" data-role="none" id="liFlavorMix" class="link-light rounded"><span class="bi-dot"></span> Flavor Mix </a></li>
					<li class="foronline"><a href="#" data-role="none" id="liComboProductMix" class="link-light rounded"><span class="bi-dot"></span> Combos Product Mix </a></li>
	                                @*<li class="foronline"><a href="#" data-role="none" id="liMRMonthlySales" class="link-light rounded"><span class="bi-dot"></span> Monthly Sales Report </a></li>*@
					<li class="foronline"><a href="#" data-role="none" id="liFlavorMonitoringPerStore" class="link-light rounded"><span class="bi-dot"></span> Flavor Monitoring per Store </a></li>
	                            </ul>
	                        </div>
	                    </li>
                        <li class="nav-item">
                            <a href="#" data-role="none" class="nav-link nav-menu text-white" data-toggle="collapse" data-target="#collapse-accountingreports">
                                <span class="bi-star-fill"></span>
                                Accounting Reports
                            </a>
                            <div class="collapse show" id="collapse-accountingreports" style="">
                                <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
                                    <li><a href="#" data-role="none" id="liConsumptionReport" class="link-light rounded"><span class="bi-dot"></span> Consumption Report </a></li>
                                    <li><a href="#" data-role="none" id="liConsolidatedUsageperDay" class="link-light rounded"><span class="bi-dot"></span> Consolidated Usage per Day </a></li>
                                    <li><a href="#" data-role="none" id="liConsolidatedEndingBalanceperDay" class="link-light rounded"><span class="bi-dot"></span> Consolidated Ending Balance per Day </a></li>
                                    <li><a href="#" data-role="none" id="liConsolidatedSCDReport" class="link-light rounded"><span class="bi-dot"></span> Consolidated SCD Report</a></li>
                                    <li><a href="#" data-role="none" id="liConsolidatedFoodAggregatorsReport" class="link-light rounded"><span class="bi-dot"></span> Consolidated Food Aggregators Report per Store</a></li>
                                    <li><a href="#" data-role="none" id="liConsolidatedPromoReport" class="link-light rounded"><span class="bi-dot"></span> Consolidated Promo Report</a></li>
                                    <li><a href="#" data-role="none" id="liDailySalesPerSKUReport" class="link-light rounded"><span class="bi-dot"></span> Daily Sales per SKU </a></li>
                                    <li><a href="#" data-role="none" id="liDailySalesPerStoreReport" class="link-light rounded"><span class="bi-dot"></span> Daily Sales per Store </a></li>
                                    <li><a href="#" data-role="none" id="liESalesReport" class="link-light rounded"><span class="bi-dot"></span> E-Sales Report </a></li>
                                    <li><a href="#" data-role="none" id="liInvoiceList" class="link-light rounded"><span class="bi-dot"></span> Invoice List </a></li>
   				    <li><a href="#" data-role="none" id="liSalesPaymentInformationReport" class="link-light rounded"><span class="bi-dot"></span> Other Payment Information </a></li>
				    <li><a href="#" data-role="none" id="liCrewOnDuty" class="link-light rounded"><span class="bi-dot"></span> Crew on Duty </a></li>
                                    <li><a href="#" data-role="none" id="liInventoryReportCrewVSAM" class="link-light rounded"><span class="bi-dot"></span> Inventory Report Crew vs AM </a></li>                                    
                                </ul>
                            </div>
                        </li>
		                <li class="nav-item">
                            <a href="#" data-role="none" class="nav-link nav-menu text-white" data-toggle="collapse" data-target="#collapse-managementreports">
                                <span class="bi-star-fill"></span>
                                Management Reports
                            </a>
                            <div class="collapse show" id="collapse-managementreports" style="">
                                <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
                                    <li><a href="#" data-role="none" id="liDailyHourlySalesReport" class="link-light rounded"><span class="bi-dot"></span> Daily Sales by Hour Report</a></li>
				                    <li><a href="#" data-role="none" id="liDailySalesSummary" class="link-light rounded"><span class="bi-dot"></span> Daily Sales Summary Report</a></li>
                                    <li><a href="#" data-role="none" id="liSalesAndTransactionCountMix" class="link-light rounded"><span class="bi-dot"></span> Sales and Transaction Count Mix</a></li>
                                    <li><a href="#" data-role="none" id="liADSandATCperStore" class="link-light rounded"><span class="bi-dot"></span> ADS and ATC per Store</a></li>
				                    <li><a href="#" data-role="none" id="liHourlySalesByWeekCategoryReport" class="link-light rounded"><span class="bi-dot"></span> Hourly Sales By Week Category Report</a></li>
				                    <li><a href="#" data-role="none" id="liDailyProductMixReport" class="link-light rounded"><span class="bi-dot"></span> Daily Product Mix Report </a></li>
                                    <li><a href="#" data-role="none" id="liProductMonitoringMonthlySummary" class="link-light rounded"><span class="bi-dot"></span> Product Monitoring Monthly Summary </a></li>                             
				                    <li><a href="#" data-role="none" id="liProductMonitoringReport" class="link-light rounded"><span class="bi-dot"></span> Product Monitoring per Store </a></li>
                                    <li><a href="#" data-role="none" id="liProductMonitoringPerStoreProductReport" class="link-light rounded"><span class="bi-dot"></span> Product Monitoring per Store-Product </a></li>
				                    <li><a href="#" data-role="none" id="liWeeklySalesReport" class="link-light rounded"><span class="bi-dot"></span> Weekly Sales Report</a></li>
                                    <li><a href="#" data-role="none" id="liWeeklyAverageDailySalesReport" class="link-light rounded"><span class="bi-dot"></span> Weekly Average Daily Sales Report</a></li>
                                    <li><a href="#" data-role="none" id="liWeeklyTransactionCountReport" class="link-light rounded"><span class="bi-dot"></span> Weekly Transaction Count Report</a></li>
                                    <li><a href="#" data-role="none" id="liWeeklyAverageCheckReport" class="link-light rounded"><span class="bi-dot"></span> Weekly Average Check Report</a></li>
                                    <li><a href="#" data-role="none" id="liMonthlySalesReport" class="link-light rounded"><span class="bi-dot"></span> Monthly Sales Report</a></li>
                                    <li><a href="#" data-role="none" id="liProductMixReportPerStore" class="link-light rounded"><span class="bi-dot"></span> Product Mix Report per Store</a></li>
                                </ul>
                            </div>
                        </li>
                            @*<li class="nav-item">
                    <a href="#" data-role="none" class="nav-link nav-menu text-secondary" data-toggle="collapse" data-target="#collapse-audittrail">
                        <span class="bi-chevron-right"></span>
                        Time & Attendance Summary Report
                    </a>
                </li>*@
                    </ul>
                </div>
            </div>
        </td>
        <td class="divpanelmenucontent">
            <div class="d-md-flex flex-column" style="height:100%; width:100%; overflow:auto; position:absolute; right:0; left:0;">
                <div class="bg-primary text-white flex-shrink-1" style="width:100%;">
                    <div class="p-3">
                        <button data-role="none" id="btnpaneltoggle" class="btn btn-primary btn-lg border-white show" style="">
                            <span class="bi-justify"></span>
                        </button>
                        <span class="OrganizationNameHeader ml-2" style="text-overflow:ellipsis; font-size:20px; white-space:nowrap; overflow:hidden;"></span>
                    </div>
                    <div class="row" style="margin:0px; padding:0.5em; width:100%; color:#FFF; background-color:#1969de; font-weight:bold;">
                        <div class="BreadCrumbs">
                            <div class="ItemsBreadCrumbs">
                                @*Main Dashboard*@
                            </div>
                        </div>
                    </div>
                </div>

                <div class="d-flex flex-fill align-items-start flex-column overflow-auto" id="divMainContentForDashboard">
                    <div id="divMainDashboard" subpage="dashboard" style="display:none;">
                        <fieldset class="border p-2 mb-3">
                            <legend class="w-auto float-none text-secondary" style="font-size:14px;">Filter Dashboard Data</legend>
                            <div class="row m-0 gy-2 gx-3 mb-3 align-items-center">
                                <div class="col-md-auto">
                                    <div class="row">
                                        <label for="btnPeriod" class="col-2 col-md-auto col-form-label">Period:</label>
                                        <div class="col col-md-auto">
                                            <button data-role="none" class="btn btn-light border btn-block btnPeriod">
                                                <span class="lblPeriod"> Today </span>
                                            </button>
                                            <button data-role="none" id="btnMCRMoreOptionForEAS" style="display:none;" class="btn btn-light border btn-block btnMoreReport">Go To</button>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-auto d-none">
                                    <div class="row">
                                        <label for="btnOverview" class="col-2 col-md-auto col-form-label">Store:</label>
                                        <div class="col col-md-auto">
                                            <button data-role="none" class="btn btn-light border btn-block btnOverview">
                                                <span class="lblOverview"> All </span>
                                            </button>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-auto">
                                    <div class="row">
                                        <label for="btnRegion" class="col-2 col-md-auto col-form-label">Region:</label>
                                        <div class="col col-md-auto">
                                            <button data-role="none" class="btn btn-light border btn-block btnRegion">
                                                <span class="lblRegion"> All </span>
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </fieldset>

                        <div class="row mb-3" id="divAllSalesReport">
                            <div class="col-md-4 mb-3">
                                <a data-role="none" class="btn btn-success" id="btnOrgGroupTotal" style="padding:0px; width:100%">
                                    <div style="padding:23px; font-weight:normal; text-shadow:0; font-size:30px; margin:0px auto; text-align:left">
                                        <div style="font-weight:bold; font-size:32px; text-align:left; text-overflow:ellipsis; white-space:nowrap; overflow:hidden; padding:0px;" id="OrganizationName">Organization</div>
                                        <hr style="border:1px solid #297E6D" />

                                        <div class="row">
                                            <div class="col-sm-auto">
                                                <div style="text-align:left; font-size:25px; font-weight:bold">Net Sales</div>
                                            </div>
                                            <div class="col-sm">
                                                <div style="text-align:right; font-size:25px; font-weight:bold; word-break:break-all;" id="TotalNetSales">-</div>
                                            </div>
                                        </div>

                                        <div class="row" style="margin-top:20px">
                                            <div class="col-sm-6">
                                                <div style="text-align:left; font-size:18px">Gross Sales</div>
                                            </div>
                                            <div class="col-sm-6">
                                                <div style="text-align:right; font-size:18px;" id="TotalGrossSales">-</div>
                                            </div>
                                        </div>

                                        <div style="text-align:left; font-size:16px; margin-top:20px">Less:</div>

                                        <div class="row" style="margin-left:5px">
                                            <div class="col-sm-6">
                                                <div style="text-align:left; font-size:16px">Discounts</div>
                                            </div>
                                            <div class="col-sm-6">
                                                <div style="text-align:right; font-size:16px;" id="TotalDiscounts">-</div>
                                            </div>
                                        </div>

                                        <div class="row" style="margin-left:5px">
                                            <div class="col-sm-6">
                                                <div style="text-align:left; font-size:16px">Vat Exempt Tax</div>
                                            </div>
                                            <div class="col-sm-6">
                                                <div style="text-align:right; font-size:16px;" id="ForegoneVAT">-</div>
                                            </div>
                                        </div>

                                        <div class="row" style="margin-left:5px">
                                            <div class="col-sm-6">
                                                <div style="text-align:left; font-size:16px">Refunds</div>
                                            </div>
                                            <div class="col-sm-6">
                                                <div style="text-align:right; font-size:16px;" id="TotalRefunds">-</div>
                                            </div>
                                        </div>

                                        <div class="row" style="margin-left:5px">
                                            <div class="col-sm-6">
                                                <div style="text-align:left; font-size:16px">Exchanges</div>
                                            </div>
                                            <div class="col-sm-6">
                                                <div style="text-align:right; font-size:16px;" id="TotalExchanges">-</div>
                                            </div>
                                        </div>

                                        <div style="text-align:left; font-size:16px; margin-top:20px">Add:</div>

                                        <div class="row" style="margin-left:5px">
                                            <div class="col-sm-6">
                                                <div style="text-align:left; font-size:16px">Charges </div>
                                            </div>
                                            <div class="col-sm-6">
                                                <div style="text-align:right; font-size:16px;" id="TotalCharges">-</div>
                                            </div>
                                        </div>

                                        <div class="row" style="margin-top:20px">
                                            <div class="col-sm-6">
                                                <div style="text-align:left; font-size:16px">Sales Transaction Count</div>
                                            </div>
                                            <div class="col-sm-6">
                                                <div style="text-align:right; font-size:16px;" id="lblSalesTransactionCount">-</div>
                                            </div>
                                        </div>

                                        <div class="row" style="margin-top:5px">
                                            <div class="col-sm-6">
                                                <div style="text-align:left; font-size:16px">Refunds/Exchanges Transaction Count</div>
                                            </div>
                                            <div class="col-sm-6">
                                                <div style="text-align:right; font-size:16px;" id="lblSalesReturnsTransactionCount">-</div>
                                            </div>
                                        </div>

                                        @*<div class="row" style="margin-top:20px">
                                <div class="col-sm-6">
                                    <div style="text-align:left; font-size:16px">FOWST Count:</div>
                                </div>
                                <div class="col-sm-6">
                                    <div style="text-align:right; font-size:16px;" id="TotalFowstCount">-</div>
                                </div>
                            </div>*@

                                    </div>
                                </a>
                            </div>
                            <div class="col-md-8" id="divDoughnutGraph" style="overflow:auto;">
                                <div id="divAllGroupNoData" subpage="divAllSalesReport" style="display:block; text-align:center">
                                    <h3>No sales data found</h3>
                                </div>
                                <div class="divAllGroupWithData" subpage="divAllSalesReport" style="display:none;">
                                    <div class="row mb-3">
                                        <div class="col-md-8">
                                            <div id="graphGrossSales" subpage="graph" style="max-height:400px;">
                                                <canvas id="chartGroupSales"></canvas>
                                            </div>
                                            <div id="graphOthers" subpage="graph" style="max-height:400px; display:none;">
                                                <canvas id="chartOtherGrap"></canvas>
                                            </div>
                                        </div>
                                        <div class="col-md-4">
                                            Graph Settings:
                                            <br />
                                            <div class="form-check">
                                                <input data-role="none" class="form-check-input" name="graphsettings" type="radio" id="chbGSGrossSales" value="grosssales" checked>
                                                <label class="form-check-label" for="chbGSGrossSales">Gross Sales</label>
                                            </div>
                                            <div class="form-check">
                                                <input data-role="none" class="form-check-input" name="graphsettings" type="radio" id="chbGSRevBreakdown" value="revbreakdown">
                                                <label class="form-check-label" for="chbGSRevBreakdown">Revenue Breakdown</label>
                                            </div>
                                            <div class="form-check">
                                                <input data-role="none" class="form-check-input" name="graphsettings" type="radio" id="chGSProductCat" value="procat">
                                                <label class="form-check-label" for="chGSProductCat">Product Category</label>
                                            </div>
                                            @*<div class="form-check">
                                    <input data-role="none" class="form-check-input" name="graphsettings" type="radio" id="chbGSSalesMix" value="salesmix">
                                    <label class="form-check-label" for="chbGSSalesMix">Sales Mix</label>
                                </div>*@
                                        </div>
                                    </div>

                                    <div class="mb-3 text-center">
                                        <a data-role="none" id="btnGraphViewDetails" class="link-primary">Click here to view more details</a>
                                    </div>

                                    <div class="table-responsive">
                                        <table data-role="none" class="table table-light table-striped table-hover">
                                            <thead>
                                                <tr class="align-top text-center" style="font-size:14px;">
                                                    <th>Group</th>
                                                    <th>Gross Sales</th>
                                                    <th>% Contribution</th>
                                                    <th>Discounts</th>
                                                    <th>VAT Exempt Tax</th>
                                                    <th>Sales Refunds</th>
                                                    <th>Sales Returns</th>
                                                    <th>Net Sales</th>
                                                </tr>
                                            </thead>
                                            <tbody id="tbodyGroupSales"></tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="divAllGroupWithData" subpage="divAllSalesReport" style="display:none;">
                            <div class="row">
                                <div class="col-md-4">
                                    <table data-role="none" class="table table-light table-striped table-hover" id="tbltopStores" style="width:100%;">
                                        <thead>
                                            <tr class="align-top text-center" style="font-size:14px;">
                                                <th style="width:50px;">#</th>
                                                <th>Top 5 Store</th>
                                                <th style="width:100px;">Gross Sales</th>
                                                <th style="width:100px;">ADS</th>
                                            </tr>
                                        </thead>
                                        <tbody></tbody>
                                    </table>
                                </div>
                                <div class="col-md-8">
                                    <h4 style="text-align:center">ADS per Day of the Week</h4> <br />
                                    <div style="max-height:400px;">
                                        <canvas id="chartADS" width="400" height="400"></canvas>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="divNewDashboard" subpage="dashboard" class="h-100 bg-light" style="display:none;">
                        
                    </div>

                    <div id="divGroupSalesReport" subpage="dashboard" style="display:none;">
                        <div class="mb-3 row align-items-center">
                            <div class="col-auto">
                                Period:
                            </div>
                            <div class="col-auto">
                                <button data-role="none" class="btn btn-secondary btnPeriod">
                                    <span class="lblPeriod"> Today </span>
                                </button>
                            </div>
                        </div>
                        <div class="mb-3">
                            Click any group to view more details.
                        </div>
                        <div class="MainFrame row row-cols-1 row-cols-sm-2 row-cols-md-2 row-cols-lg-2 row-cols-xl-3" id="MainFrame" style="overflow:auto;">
                        </div>
                    </div>

                    <div id="divSalesAnalytics" subpage="dashboard" style="display:none;">
                        <div class="mb-3 d-none">
                            <button data-role="none" class="btn btn-secondary btnPeriod">
                                <span class="lblPeriod"> Today </span>
                            </button>
                            <button data-role="none" id="btnReportType" class="btn btn-secondary">
                                <span id="txtReportType"> Group Sales </span>
                            </button>
                        </div>

                        <div id="divGroupSales" subpage="divSalesAnalytics" style="display:block;">
                            <div class="accordion mt-3">
                                <div class="card">
                                    <div class="card-header p-0">
                                        <button class="btn btn-light btn-block text-left font-weight-bold p-3 rounded-0" data-role="none" id="lblGroupName"></button>
                                    </div>
                                    <div class="collapse show p-3">
                                        <div class="row m-0">
                                            <div class="col-md-4 mb-3">
                                                <div class="text-white bg-success rounded" style="padding:15px;font-weight:normal; font-size:20px; margin:0px auto; height:100%;">
                                                    <div style="font-weight:bold; font-size:20px; text-align:center;">GROUP NET SALES</div>
                                                    <hr style="border:1px solid #297E6D" />

                                                    <div style="text-align:center; word-break:break-all;" class="SalesFontSize">
                                                        <label id="lblGroupNetSales" style="font-size:30px;"></label>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-md-4 mb-3">
                                                <div class="bg-light rounded" style="padding:15px;font-weight:normal; font-size:20px; margin:0px auto; height:100%;">
                                                    <div style="font-weight:bold; font-size:20px; text-align:center;">GROUP GROSS SALES</div>
                                                    <hr style="border:1px solid #CCC" />

                                                    <div style="text-align:center; word-break:break-all;" class="SalesFontSize">
                                                        <label id="lblGroupGrossSales" style="font-size:20px;"></label>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-md-4 mb-3">
                                                <div class="bg-light rounded" style="padding:15px;font-weight:normal; font-size:20px; margin:0px auto; height:100%;">
                                                    <div style="text-align:left; font-size:13px">Group Discounts</div>
                                                    <div style="text-align:right; font-size:20px;"><label id="lblGroupDiscounts"></label></div>

                                                    <div style="text-align:left; font-size:13px">Vat Exempt Tax</div>
                                                    <div style="text-align:right; font-size:20px;"><label id="lblGroupForegoneVAT"></label></div>

                                                    <div style="text-align:left; font-size:13px">Group Refunds</div>
                                                    <div style="text-align:right; font-size:20px;"><label id="lblGroupRefunds"></label></div>

                                                    <div style="text-align:left; font-size:13px">Group Exchanges</div>
                                                    <div style="text-align:right; font-size:20px;"><label id="lblGroupExchanges"></label></div>

                                                    <div style="text-align:left; font-size:13px">Add: Group Charges</div>
                                                    <div style="text-align:right; font-size:20px;"><label id="lblGroupCharges"></label></div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="mt-4 d-none">
                                            <div class="row align-items-center">
                                                <div class="col-md-auto">Display these Store locations:</div>
                                                <div class="col-md">
                                                    <div class="form-check form-check-inline">
                                                        <input type="checkbox" data-role="none" name="filterGroupSales" class="form-check-input" id="chbGroupSalesCompanyOwned" value="Company-Owned" checked />
                                                        <label class="form-check-label" for="chbGroupSalesCompanyOwned">Company-Owned</label>
                                                    </div>
                                                    <div class="form-check form-check-inline">
                                                        <input type="checkbox" data-role="none" name="filterGroupSales" class="form-check-input" id="chbGroupSalesFranchised" value="Franchised" checked />
                                                        <label class="form-check-label" for="chbGroupSalesFranchised">Franchised</label>
                                                    </div>
                                                    <div class="form-check form-check-inline">
                                                        <input type="checkbox" data-role="none" name="filterGroupSales" class="form-check-input" id="chbGroupSalesCoOwned" value="Co-Owned" checked />
                                                        <label class="form-check-label" for="chbGroupSalesCoOwned">Co-Owned</label>
                                                    </div>
                                                    <div class="form-check form-check-inline">
                                                        <input type="checkbox" data-role="none" name="filterGroupSales" class="form-check-input" id="chbGroupSalesUnidentified" value="NULL" checked />
                                                        <label class="form-check-label" for="chbGroupSalesUnidentified">Unidentified</label>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="table-responsive mt-3">
                                            <table data-role="none" data-mode="reflow" style="margin-right:5px; display:table;" class="table table-light table-striped table-hover headerfitwidth">
                                                <thead>
                                                    <tr class="align-top text-center" style="font-size:14px;">
                                                        <th data-sort="string" style="cursor:pointer;">Sales Date</th>
                                                        <th data-sort="string" style="cursor:pointer;">Store Name</th>
                                                        @*<th data-sort="string" style="cursor:pointer;">Ownership Type</th>
        <th data-sort="string" style="cursor:pointer;">Geographical Region</th>*@
                                                        @*<th data-sort="string" style="cursor:pointer;">Owner</th>
                                                        <th data-sort="string" style="cursor:pointer;">Organization Name</th>*@
                                                        <th data-sort="accounting" style="text-align:center;cursor:pointer;">Net Sales</th>
                                                        @*<th data-sort="percentage" style="text-align:center;cursor:pointer;">Net Sales Contribution(%)</th>*@
                                                        <th data-sort="accounting" style="text-align:center;cursor:pointer;">Gross Sales</th>
                                                        @*<th data-sort="accounting" style="text-align:center;cursor:pointer;">Gross Sales Contribution(%)</th>*@
                                                        <th data-sort="accounting" style="text-align:center;cursor:pointer;">Discounts</th>
                                                        @*<th data-sort="percentage" style="text-align:center;cursor:pointer;">Discounts Contribution(%)</th>*@
                                                        <th data-sort="accounting" style="text-align:center;cursor:pointer;">Vat Exempt Tax</th>
                                                        @*<th data-sort="percentage" style="text-align:center;cursor:pointer;">Vat Exempt Tax Contribution(%)</th>*@
                                                        @*<th data-sort="accounting" style="text-align:center;cursor:pointer;">Refunds</th>
        <th data-sort="percentage" style="text-align:center;cursor:pointer;">Refunds Contribution(%)</th>*@
                                                        <th data-sort="accounting" style="text-align:center;cursor:pointer;">Adjustments</th>
                                                        @*<th data-sort="percentage" style="text-align:center;cursor:pointer;">Adjustments Contribution(%)</th>*@
                                                        <th data-sort="accounting" style="text-align:center;cursor:pointer;">Charges</th>
                                                    </tr>
                                                </thead>
                                                <tbody id="reportTable"></tbody>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div id="dvSalesTrends" class="accordion mt-3" style="display:none;">
                                <div class="card">
                                    <div class="card-header p-0">
                                        <button class="btn btn-light btn-block text-left font-weight-bold p-3 rounded-0" data-role="none" id="lblSalesTrend">Store</button>
                                    </div>
                                    <div class="collapse show p-3">
                                        <div class="mb-3">
                                            <label id="IsDataPresent"> -Graph will be shown when you click on a group or FOWST (Factory, Office, Store, Warehouse or Truck)  name- </label>
                                            <div id="lblMainChartInstruction" style="color:Red; visibility: hidden; font-size:small; text-align:center;"> * Click the chart to breakdown data</div>
                                            <div id="dvMainChart" class="mb-3"></div>
                                            <div class="text-center">
                                                <a href="#" id="lnkTransactionDetails" style="visibility:hidden">  Click here to view transaction details </a>
                                            </div>
                                            <br /><br />
                                            <table data-role="none" id="tblMainChart" class="table table-light table-hover table-striped" style="width:100%;">
                                                <thead>
                                                    <tr class="align-top text-center" style="font-size:14px;">
                                                        <th>Date</th>
                                                        <th>Gross Sales</th>
                                                        <th>Discount</th>
                                                        <th>Vat Exempt Tax</th>
                                                        <th>Refunds and Exchanges</th>
                                                        <th>Charges</th>
                                                        <th>Net Sales</th>
                                                    </tr>
                                                </thead>
                                                <tfoot>
                                                    <tr>
                                                        <th style="text-align:right">Totals:</th>
                                                        <th></th>
                                                        <th></th>
                                                        <th></th>
                                                        <th></th>
                                                        <th></th>
                                                        <th></th>
                                                    </tr>
                                                </tfoot>
                                            </table>

                                            <div class="table-responsive" style="display:none;">
                                                <table data-role="none" class="table table-light table-striped table-hover">
                                                    <thead>
                                                        <tr class="align-top text-center" style="font-size:14px;">
                                                            <th>Store</th>
                                                            <th>Gross Sales</th>
                                                            <th>% Contribution</th>
                                                            <th>Discounts</th>
                                                            <th>VAT Exempt Tax</th>
                                                            <th>Sales Refunds</th>
                                                            <th>Sales Returns</th>
                                                            <th>Net Sales</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody id="tbodyGroupSales"></tbody>
                                                </table>
                                            </div>
                                        </div>

                                        <div id="dvSlice" style="display:none;" class="mb-3">
                                            <h5 class="mb-3">
                                                <a id="liSecondChartMainLabel" data-role="none"></a>
                                                <a href="#" id="liSecondChart" data-role="none"></a>
                                            </h5>
                                            <div id="dvSecondChart"></div>
                                            <table>
                                                <thead>
                                                    <tr>
                                                        <th align="right">
                                                            <label style="background-color:#4BB2C5"> &nbsp; &nbsp; &nbsp; &nbsp; </label> &nbsp; Transaction Count
                                                        </th>
                                                        <th>
                                                            &nbsp; &nbsp; <label style="background-color:#eaa228"> &nbsp; &nbsp; &nbsp; &nbsp; </label> &nbsp; Gross Sales
                                                        </th>
                                                    </tr>
                                                </thead>
                                            </table>
                                            <div id="lblSecondChartInstruction" class="mb-3" style="color:Red; visibility: hidden; font-size:small; text-align:center;"></div>

                                            <div class="table-responsive mb-3">
                                                <table id="tblSecondChart" data-role="none" style="display:table;" class="table table-light table-striped table-hover headerfitwidth">
                                                    <thead id="tblSecondChartHeader"></thead>
                                                    <tbody id="tblSecondChartBody"></tbody>
                                                    <tfoot id="tblSecondChartFooter">
                                                        <tr>
                                                            <th style="text-align:right">Totals:</th>
                                                            <th></th>
                                                            <th></th>
                                                            <th></th>
                                                            <th></th>
                                                            <th></th>
                                                        </tr>
                                                    </tfoot>
                                                </table>
                                            </div>
                                        </div>

                                        <div id="dvDice" style="display:none;" class="mb-3">
                                            <h5 class="mb-3">
                                                <a id="liThirdChartMainLabel" data-role="none"></a>
                                                <a href="#" id="liThirdChartLabel" data-role="none"></a>
                                            </h5>
                                            <div id="dvThirdChart"></div>
                                            <table>
                                                <thead>
                                                    <tr>
                                                        <th align="right">
                                                            <label style="background-color:#4BB2C5"> &nbsp; &nbsp; &nbsp; &nbsp; </label> &nbsp; Transaction Count
                                                        </th>
                                                        <th>
                                                            &nbsp; &nbsp; <label style="background-color:#eaa228"> &nbsp; &nbsp; &nbsp; &nbsp; </label> &nbsp; Gross Sales
                                                        </th>
                                                    </tr>
                                                </thead>
                                            </table>
                                            <div id="lblSecondChartInstruction" class="mb-3" style="color:Red; visibility: hidden; font-size:small; text-align:center;"></div>

                                            <div class="table-responsive mb-3">
                                                <table id="tblThirdChart" data-role="none" style="display:table;" class="table table-light table-striped table-hover headerfitwidth">
                                                    <thead id="tblThirdChartHeader"></thead>
                                                    <tbody id="tblThirdChartBody"></tbody>
                                                    <tfoot id="tblThirdChartFooter"></tfoot>
                                                </table>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>

                        @*<div data-role="popup" style="display:none;" id="panSalesTrends" data-overlay-theme="a" data-theme="a" data-corners="false">*@

                        <div id="divInventoryLevels" subpage="divSalesAnalytics" style="display:none;">
                            <div class="row mb-3">
                                <div class="col-md-9 mb-3">
                                    Display these FOWST locations:
                                    <div class="btn-group flex-wrap" role="group" aria-label="Basic checkbox toggle button group">
                                        <input type="checkbox" data-role="none" name="filterInventoryLvl" class="btn-check" id="chbInventoryLvlCompanyOwned" value="Company-Owned" checked />
                                        <label class="btn btn-outline-dark" for="chbInventoryLvlCompanyOwned">Company-Owned</label>

                                        <input type="checkbox" data-role="none" name="filterInventoryLvl" class="btn-check" id="chbInventoryLvlFranchised" value="Franchised" checked />
                                        <label class="btn btn-outline-dark" for="chbInventoryLvlFranchised">Franchised</label>

                                        <input type="checkbox" data-role="none" name="filterInventoryLvl" class="btn-check" id="chbInventoryLvlCoOwned" value="Co-Owned" checked />
                                        <label class="btn btn-outline-dark" for="chbInventoryLvlCoOwned">Co-Owned</label>

                                        <input type="checkbox" data-role="none" name="filterInventoryLvl" class="btn-check" id="chbInventoryLvlUnidentified" value="NULL" checked />
                                        <label class="btn btn-outline-dark" for="chbInventoryLvlUnidentified">Unidentified</label>
                                    </div>
                                </div>
                                <div class="col-md-3">
                                    <div id="divGroupInventoryLevelsButtons" style="display:block; float:right">
                                        <button id="butGroupInventoryLevelsRecache" data-role="none" class="btn btn-success">
                                            Get latest data
                                        </button>
                                    </div>
                                </div>
                            </div>

                            @* bookmark: arvin, GroupInventoryLevels *@
                            <div id="divGroupInventoryLevelsResult">
                                <div class="table-responsive mt-3">
                                    <table data-role="none" id="tblGroupInventoryLevels" class="table table-light table-striped table-hover headerfitwidth">
                                        <thead>
                                            <tr class="align-top text-center" style="font-size:14px;">
                                                <th data-sort="string" style="cursor:pointer; min-width:200px;">FOWST</th>
                                                <th data-sort="string" style="cursor:pointer;">Ownership Type</th>
                                                <th data-sort="string" style="cursor:pointer;">Geographical Region</th>
                                                <th data-sort="string" style="cursor:pointer; min-width:200px;">Owner</th>
                                                <th data-sort="string" style="cursor:pointer;">Organization Name</th>
                                                <th data-sort="accounting" style="text-align:center;cursor:pointer;">Value by SRP</th>
                                                <th data-sort="accounting" style="text-align:center;cursor:pointer;">SRP Value of Open Orders</th>
                                                <th data-sort="accounting" style="text-align:center;cursor:pointer;">No Of Unapproved Counts</th>
                                                <th data-sort="accounting" style="text-align:center;cursor:pointer; min-width:200px;">As Of</th>
                                            </tr>
                                        </thead>
                                        <tbody></tbody>
                                    </table>
                                </div>
                            </div>
                        </div>

                        <div id="divMoniesAcc" subpage="divSalesAnalytics" style="display:none;">

                        </div>

                        <div id="divInventoriesAcc" subpage="divSalesAnalytics" style="display:none;">

                        </div>
                    </div>

                    <div id="panTransactionDetails" subpage="dashboard" style="display:none;">
                        <div class="mb-3 row">
                            <div class="col-md-8">
                                <table>
                                    <tr>
                                        <td>Date:</td>
                                        <td><label style="font-weight:bolder; font-size:large" id="lblSalesTransactionDate"> </label></td>
                                    </tr>
                                    <tr>
                                        <td>Store:</td>
                                        <td><label style="font-weight:bolder; font-size:large" id="lblFOWST"> </label></td>
                                    </tr>
                                </table>
                            </div>
                            <div class="col-md-4">
                                <div class="form-check text-right" style="font-size:25px;">
                                    <label class="form-check-label" for="chbTransViewOpt">
                                        <input type="checkbox" data-role="none" class="form-check-input" name="chbTransViewOpt" id="chbTransViewOpt" />
                                        Show All SKU Details
                                    </label>
                                </div>
                            </div>
                        </div>

                        <div class="row m-0 gx-3 align-items-center">
                            <div class="col-sm-auto">
                                <div class="row">
                                    <label for="selCashier" class="col-auto col-form-label">Cashier:</label>
                                    <div class="col">
                                        <select id="selCashier" data-role="none" class="form-select">
                                            <option> All Cashier </option>
                                        </select>
                                    </div>
                                </div>
                            </div>
                            <div class="col-sm">
                                <input id="txtSearchHeader" placeholder="Search invoice no." type="search" class="form-control" data-role="none" />
                                <input id="txtSearchProductTransDetails" data-role="none" placeholder="Search invoice no., product description, sales representative or customer" type="search" class="form-control" style="display:none;" />
                            </div>
                        </div>

                        @*<div class="mb-3">
            <h4 class="mb-3">Total</h4>

            <div class="table-responsive">
                <table class="table table-bordered">
                    <tr>
                        <td>Gross Sales: <label style="font-weight:bolder; font-size:large" id="lblDashboardTotalGrossSales"> 0.00 </label></td>
                        <td>Deductive Adjustments: <label style="font-weight:bolder; font-size:large" id="lblDashboardTotalDeductiveAdjustments"> 0.00 </label></td>
                        <td>Additive Adjustments: <label style="font-weight:bolder; font-size:large" id="lblDashboardTotalAdditiveAdjustments"> 0.00 </label></td>
                        <td>Net Sales: <label style="font-weight:bolder; font-size:large" id="lblDashboardNetSales"> 0.00 </label></td>
                    </tr>
                    <tr>
                        <td>Cash: <label style="font-weight:bolder; font-size:large" id="lblDashboardTotalCash"> 0.00 </label></td>
                        <td>Check: <label style="font-weight:bolder; font-size:large" id="lblDashboardTotalCheck"> 0.00 </label></td>
                        <td>Debit Card: <label style="font-weight:bolder; font-size:large" id="lblDashboardTotalDebitCard"> 0.00 </label></td>
                        <td>Credit Card: <label style="font-weight:bolder; font-size:large" id="lblDashboardCreditCard"> 0.00 </label></td>
                    </tr>
                    <tr>
                        <td>SRV: <label style="font-weight:bolder; font-size:large" id="lblDashboardTotalSRV"> 0.00 </label></td>
                        <td>On-Credit: <label style="font-weight:bolder; font-size:large" id="lblDashboardTotalOnCredit"> 0.00 </label></td>
                        <td>Other Payments: <label style="font-weight:bolder; font-size:large" id="lblDashboardTotalOtherPayments"> 0.00 </label></td>
                    </tr>
                </table>
            </div>
        </div>*@

                        @*<div id="dvSalesByCashier" class="mb-3">
            Cashiers: <label style="font-weight:bolder; font-size:large" id="lblSalesByCashier"> 0.00 </label>
        </div>*@

                        <div id="divTransHeader" class="mb-3" style="display:block;">
                            <div class="table-responsive mb-3">
                                <table data-role="none" id="tblTransactionHeader" class="table table-light table-hover table-striped headerfitwidth">
                                    <thead>
                                        <tr class="align-top text-center" style="font-size:14px;">
                                            @*<th data-sort="float" style="cursor:pointer;"> # </th>*@
                                            <th data-sort="date" style="cursor:pointer;"> Invoice Date </th>
                                            <th data-sort="int" style="cursor:pointer;"> Invoice No. </th>
                                            <th data-sort="string" style="cursor:pointer;"> Customer </th>
                                            <th data-sort="accounting" style="cursor:pointer;"> Gross Sales </th>
                                            <th data-sort="int" style="cursor:pointer;"> SKU Count </th>
                                            <th data-sort="string" style="cursor:pointer;"> Sales Representative </th>

                                            <th data-sort="accounting" style="cursor:pointer;"> Cash Payment </th>
                                            <th data-sort="accounting" style="cursor:pointer;"> Credit Card Payment </th>
                                            <th data-sort="accounting" style="cursor:pointer;"> Debit Card Payment</th>
                                            <th data-sort="accounting" style="cursor:pointer;"> Cheque Payment</th>
                                            <th data-sort="accounting" style="cursor:pointer;"> SRV Payment</th>
                                            <th data-sort="accounting" style="cursor:pointer;"> On Credit Payment</th>
                                            <th data-sort="accounting" style="cursor:pointer;"> Other Payment </th>
                                            <th data-sort="accounting" style="cursor:pointer;"> Other Payment Type</th>
                                            <th data-sort="accounting" style="cursor:pointer;"> Cashier </th>
                                        </tr>
                                    </thead>
                                    <tbody></tbody>
                                </table>
                            </div>
                        </div>

                        <div id="divTransDetails" class="mb-3" style="display:none;">                            
                            <div class="table-responsive mb-3">
                                <table data-role="none" id="tblTransactionDetails" class="table table-light table-hover table-striped headerfitwidth">
                                    <thead>
                                        <tr class="align-top text-center" style="font-size:14px;">
                                            <th data-sort="int" style="cursor:pointer;"> Invoice No. </th>
                                            <th data-sort="date" style="cursor:pointer;"> Invoice Date </th>
                                            <th data-sort="string" style="cursor:pointer;"> Product Description </th>
                                            <th data-sort="accounting" style="text-align: center; cursor:pointer;"> Qty </th>
                                            <th data-sort="accounting" style="text-align: center; cursor:pointer;"> Qty Returned </th>
                                            <th data-sort="string" style="cursor:pointer;"> Unit Measure </th>
                                            <th data-sort="string" style="cursor:pointer;"> Returned Product For Rebate</th>
                                            <th data-sort="accounting" style="cursor:pointer;"> Rebate Qty </th>
                                            <th data-sort="string" style="cursor:pointer;"> Rebate Unit Measure</th>
                                            <th data-sort="accounting" style="text-align: right; cursor:pointer;"> Unit Price </th>
                                            <th data-sort="accounting" style="text-align: right; cursor:pointer;"> Sub Total </th>
                                            <th data-sort="accounting" style="text-align: right; cursor:pointer;"> Discount </th>
                                            <th data-sort="string" style="cursor:pointer;"> Sales Representative(s) </th>
                                            <th data-sort="string" style="cursor:pointer;"> Customer </th>
                                            <th data-sort="string" style="cursor:pointer;"> Last Updated By </th>
                                        </tr>
                                    </thead>
                                    <tbody></tbody>
                                </table>
                            </div>
                        </div>

                        <h4 class="mb-3">POS Sales Returns</h4>

                        <div class="table-responsive">
                            <table data-role="none" id="tblSalesReturnDetails" class="table table-light table-hover table-striped headerfitwidth">
                                <thead>
                                    <tr class="align-top text-center" style="font-size:14px;">
                                        <th data-sort="int" style="cursor:pointer;"> Memo Number </th>
                                        <th data-sort="date" style="cursor:pointer;"> Memo Date </th>
                                        <th data-sort="int" style="cursor:pointer;"> Srv No. </th>
                                        <th data-sort="accounting" style="text-align: right; cursor:pointer;"> Amount </th>
                                        <th data-sort="int" style="cursor:pointer;"> Cashier </th>
                                    </tr>
                                </thead>
                                <tbody id="tblSalesReturnDetailsBody"></tbody>
                            </table>
                        </div>
                    </div>

                    <div id="divSalesReplenishmentProcessing" subpage="dashboard" style="display:none;"></div>

                    <div id="divSalesOrderProcessing" subpage="dashboard" style="display:none;"></div>

                    <div id="divOnPrintPOSSalesReport" subpage="dashboard" style="display:none;"></div>

                    <div id="divEJournalReport" subpage="dashboard" style="display:none;"></div>

                    <div id="divFranchiseRequest" subpage="dashboard" style="display:none;"></div>

                    <div id="divDBInventoryBalance" subpage="dashboard" style="display:none;"></div>

                    <div id="divDBInventoryLedger" subpage="dashboard" style="display:none;"></div>

                    <div id="divDBStockReceivingTransfers" subpage="dashboard" style="display:none;"></div>

                    <div id="divDBPhysicalInventory" subpage="dashboard" style="display:none;"></div>

                    <div id="divDBStockInventoryLevels" subpage="dashboard" style="display:none;"></div>

                    <div id="divDBZReport" subpage="dashboard" style="display:none;"></div>

                    <div id="divDBSalesBySkuReport" subpage="dashboard" style="display:none;"></div>

                    <div id="divDBInventoryFormReport" subpage="dashboard" style="display:none;"></div>

                    <div id="divDBScdAndPwdReport" subpage="dashboard" style="display:none;"></div>

                    <div id="divDBSrvHistory" subpage="dashboard" style="display:none;"></div>

                    <div id="divDBPOSTransactionDetail" subpage="dashboard" style="display:none;"></div>

                    <div id="divDBReportToPrincipal" subpage="dashboard" style="display:none;"></div>

                    <div id="divDBShopperTrak" subpage="dashboard" style="display:none;"></div>

                    <div id="divDBAyala" subpage="dashboard" style="display:none;"></div>

                    <div id="divDBSMCoin" subpage="dashboard" style="display:none;"></div>

                    <div id="divDBMegaworld" subpage="dashboard" style="display:none;"></div>

                    <div id="divDBVouchersReport" subpage="dashboard" style="display:none;"></div>

                    <div id="divDBPOSUtilization" subpage="dashboard" class="h-100 bg-light" style="display:none;"></div>

                    <div id="divDBMoniesAcc" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Date: <br />
                                <button id="btnMoniesAccDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblMoniesAccDate"> Today </span>
                                </button>
                            </div>


                            <div class="col-auto mb-3">
                                Group: <br />
                                <button id="btnMoniesAccGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary">
                                    <span class="lblMoniesAccReportGroupFOWST"> Group Store </span>
                                </button>
                            </div>

                            @*<div class="col-auto mb-3">
                    Store Type:
                    <select data-role="none" id="selDailySalesReportStoreType" class="form-select selstoretypeSalesReport"></select>
                </div>
                <div class="col-auto mb-3">
                    Store Location Type:
                    <select data-role="none" id="selDailySalesReportStoreLocationType" class="form-select selstorelocationtypeSalesReport"></select>
                </div>*@
                        </div>

                        <div class="table-responsive mb-3">
                            <table data-role="none" id="tblMoniesAccReport" class="table table-light table-hover table-striped" style="width:100%;">
                                <thead>

                                    <tr class="align-top text-center" style="font-size:14px;">
                                        <th data-sort="date" style="cursor:pointer;"> Store Name </th>
                                        @*<th data-sort="int" style="cursor:pointer;"> Group </th>*@

                                        <th data-sort="accounting" style="cursor:pointer;"> POS Transaction Date </th>
                                        <th data-sort="accounting" style="cursor:pointer;"> Deposited By </th>
                                        <th data-sort="accounting" style="cursor:pointer;"> Bank Branch </th>
                                        <th data-sort="accounting" style="cursor:pointer;"> Service Charge </th>
                                        <th data-sort="accounting" style="cursor:pointer;"> Deposit Reference No </th>

                                        <th data-sort="accounting" style="cursor:pointer;">Total Amount</th>
                                        <th data-sort="accounting" style="cursor:pointer;">Total Cash</th>
                                        <th data-sort="accounting" style="cursor:pointer;">Credit Card</th>
                                        <th data-sort="accounting" style="cursor:pointer;">Debit Card</th>
                                        <th data-sort="accounting" style="cursor:pointer;">Cheque</th>
                                        <th data-sort="accounting" style="cursor:pointer;">G Cash</th>
                                        <th data-sort="accounting" style="cursor:pointer;">Pay Maya</th>
                                        @*<th data-sort="accounting" style="cursor:pointer;">SMAC</th>*@
                                        <th data-sort="accounting" style="cursor:pointer;">Gold Card</th>
                                        <th data-sort="accounting" style="cursor:pointer;">Shopee Pay</th>
                                        <th data-sort="accounting" style="cursor:pointer;">Shopee E-Voucher</th>

                                        <th data-sort="accounting" style="cursor:pointer;">Food Panda</th>
                                        <th data-sort="accounting" style="cursor:pointer;">Grab</th>
                                        <th data-sort="accounting" style="cursor:pointer;">Pood Trip</th>
                                        <th data-sort="accounting" style="cursor:pointer;">DeliRush</th>
                                        <th data-sort="accounting" style="cursor:pointer;">Local Delivery Partners</th>

                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>
                    </div>

                    <div id="divSalesBySKUReport" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-6 col-md-auto mb-3">
                                Date: <br />
                                <button id="btnSalesBySKUDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblSalesBySKUDate"> Today </span>
                                </button>
                            </div>

                            <div class="col-6 col-md-auto mb-3">
                                Store: <br />
                                <button id="btnSalesBySKUFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblSalesBySKUFOWST"> Store </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Group: <br />
                                <button id="btnSalesBySKUGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary">
                                    <span class="lblSalesBySKUReportGroupFOWST"> Group Store </span>
                                </button>
                            </div>
                        </div>

                        <div class="table-responsive mb-3">
                            <table data-role="none" id="tblSalesBySKUReport" class="table table-light table-hover table-striped" style="width:100%;">
                                <thead>

                                    <tr class="align-top text-center" style="font-size:14px;">
                                        <th data-sort="date" style="cursor:pointer; text-align:left"> Product </th>

                                        <th data-sort="accounting" style="cursor:pointer; text-align:left"> Unit Measure </th>
                                        <th data-sort="accounting" style="cursor:pointer; text-align:left"> Quantity </th>
                                        <th data-sort="accounting" style="cursor:pointer; text-align:left"> Sub Total </th>

                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>
                    </div>

                    <div id="divProductPerformanceTracker" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Date: <br />
                                <button id="btnProductPerformanceTrackerDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblProductPerformanceTrackerDate"> Today </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Product Type: <br />

                                <select data-role="none" id="selProductType" class="form-select">
                                    <option value="optAllFlavors">All Flavors</option>
                                    <option value="optFinishProducts">All Finished Products</option>
                                    @*<option value="optComboMix">All Combo Mix</option>*@
                                </select>
                            </div>
                            @*
                <div class="col-auto mb-3">
                    Store : <br />

                    <select data-role="none" id="selStoreTypeProductPerformanceTracker" class="form-select">
                        <option value="optCompanyOwnedStores">Company-Owned Stores</option>
                        <option value="optFranchiseeStores">Franchisee Stores</option>
                    </select>
                </div>

                <div class="col-auto mb-3" id="divProductPerformanceTrackerSearchProduct">
                    <br />
                    <input data-role="none" class="form-control" placeholder="Search Product" id="txtProductPerformanceTrackerProduct" />
                </div>*@

                        </div>

                        <div class="table-responsive mb-3">
                            @*<table data-role="none" id="tblProductPerformanceTrackerReport" class="table table-light table-hover table-striped" style="width:100%;">
                    <thead>

                        <tr class="align-top text-center" style="font-size:14px;">
                            <th data-sort="date" style="cursor:pointer; text-align:left"> Product </th>
                            <th data-sort="accounting" style="cursor:pointer; text-align:left"> Unit Measure </th>
                            <th data-sort="accounting" style="cursor:pointer; text-align:left"> Quantity </th>
                            <th data-sort="accounting" style="cursor:pointer; text-align:left"> Sub Total </th>

                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>*@

                            <div id="dvProductPerformanceTrackerReport">
                                <table data-role="none" id="tblProductPerformanceTrackerReport" class="table table-light table-hover table-striped" style="width:100%;">
                                    <thead>

                                    </thead>
                                    <tbody></tbody>
                                </table>
                            </div>

                            <div id="dvProductPerformanceTrackerNonFlavorReport">
                                <table data-role="none" id="tblProductPerformanceTrackerNonFlavorReport" class="table table-light table-hover table-striped" style="width:100%">
                                    <thead>

                                    </thead>
                                    <tbody></tbody>
                                </table>
                            </div>


                        </div>
                    </div>

                    <div id="divProductMix" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Date: <br />
                                <button id="btnProductMixDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblProductMixDate"> Today </span>
                                </button>
                            </div>
                            
                            <div class="col-auto mb-3">
                                Store: <br />
                                <button id="btnProductMixFowst" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="btnProductMixFowst"> All Stores </span>
                                </button>
                            </div>
                        </div>

                        <div class="table-responsive mb-3">
                            <div id="dvProductMixReport">
                                <table data-role="none" id="tblProductMixReport" class="table table-light table-hover table-striped" style="width:100%;">
                                    <thead>
                                        <tr>
                                            <th class="table-active" data-sort="date" style="cursor:pointer; text-align:left"> Product Name </th>
                                            <th class="table-active" data-sort="date" style="cursor:pointer; text-align:left"> Total Sales </th>
                                            <th class="table-active" data-sort="int" style="cursor:pointer; text-align:left"> Total Transaction Count </th>
                                            <th class="table-active" data-sort="int" style="cursor:pointer; text-align:left"> Total Product Count </th>
                                            <th class="table-active" data-sort="int" style="cursor:pointer; text-align:left"> No. of Stores </th>
                                            <th class="table-active" data-sort="int" style="cursor:pointer; text-align:left"> ADQ </th>
                                        </tr>
                                    </thead>
                                    <tbody></tbody>
                                </table>
                            </div>
                        </div>
                    </div>

                    <div id="divProductMixperWeek" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Date: <br />
                                <button id="btnProductMixperWeekDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblProductMixperWeekDate"> Today </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Store: <br />
                                <button id="btnProductMixPerWeekFowst" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="btnProductMixPerWeekFowst"> All Stores </span>
                                </button>
                            </div>
                        </div>

                        <div class="table-responsive mb-3">
                            <div id="dvProductMixReport">
                                <table data-role="none" id="tblProductMixperWeekReport" class="table table-light table-hover table-striped" style="width:100%;">
                                    <thead>
                                        <tr>
                                            <th class="table-active" data-sort="date" style="cursor:pointer; text-align:left"> Product Name </th>
                                            <th class="table-active" data-sort="date" style="cursor:pointer; text-align:left"> Total Sales </th>
                                            <th class="table-active" data-sort="int" style="cursor:pointer; text-align:left"> Total Transaction Count </th>
                                            <th class="table-active" data-sort="int" style="cursor:pointer; text-align:left"> Total Product Count </th>
                                            <th class="table-active" data-sort="int" style="cursor:pointer; text-align:left"> No. of Stores </th>
                                            <th class="table-active" data-sort="int" style="cursor:pointer; text-align:left"> ADQ </th>
                                            <th class="table-active" data-sort="int" style="cursor:pointer; text-align:left"> Week </th>
                                        </tr>
                                    </thead>
                                    <tbody></tbody>
                                </table>
                            </div>
                        </div>
                    </div>

                    <div id="divComboProductMix" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Date: <br />
                                <button id="btnComboProductMixDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblComboProductMixDate"> Today </span>
                                </button>
                            </div>
                            <div class="col-auto mb-3">
                                Store: <br />
                                <button id="btnComboProductMixFowst" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="btnComboProductMixFowst"> All Stores </span>
                                </button>
                            </div>

                        </div>

                        <div class="table-responsive mb-3">
                            <div id="dvComboProductMixReport">
                                <table data-role="none" id="tblComboProductMixReport" class="table table-light table-hover table-striped" style="width:100%;">
                                    <thead>
                                        <tr>
                                            <th class="table-active" data-sort="date" style="cursor:pointer; text-align:left"> Product Name </th>
                                            <th class="table-active" data-sort="date" style="cursor:pointer; text-align:left"> Total Sales </th>
                                            <th class="table-active" data-sort="int" style="cursor:pointer; text-align:left"> Total Transaction Count </th>
                                            <th class="table-active" data-sort="int" style="cursor:pointer; text-align:left"> Total Product Count </th>
                                            <th class="table-active" data-sort="int" style="cursor:pointer; text-align:left"> No. of Stores </th>
                                            <th class="table-active" data-sort="int" style="cursor:pointer; text-align:left"> ADQ </th>
                                        </tr>
                                    </thead>
                                    <tbody></tbody>
                                </table>
                            </div>
                        </div>
                    </div>

                    <div id="divFlavorMix" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Date: <br />
                                <button id="btnFlavorMixDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblFlavorMixDate"> Today </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Store: <br />
                                <button id="btnFlavorMixFowst" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="btnFlavorMixFowst"> All Stores </span>
                                </button>
                            </div>
                        </div>

                        <div class="table-responsive mb-3">
                            <div id="dvFlavorMixReport">
                                <table data-role="none" id="tblFlavorMixReport" class="table table-light table-hover table-striped" style="width:100%;">
                                    <thead>
                                        <tr>
                                            <th class="table-active" data-sort="date" style="cursor:pointer; text-align:left"> Flavor </th>
                                            <th class="table-active" data-sort="date" style="cursor:pointer; text-align:left"> Total Sales </th>
                                            <th class="table-active" data-sort="int" style="cursor:pointer; text-align:left"> Total Transaction Count </th>
                                            <th class="table-active" data-sort="int" style="cursor:pointer; text-align:left"> Total Product Count </th>
                                            <th class="table-active" data-sort="int" style="cursor:pointer; text-align:left"> No. of Stores </th>
                                            <th class="table-active" data-sort="int" style="cursor:pointer; text-align:left"> ADQ </th>
                                        </tr>
                                    </thead>
                                    <tbody></tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                    
                    <div id="divFlavorMonitoringReport" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Date: <br />
                                <button id="btnFlavorMonitoringReportDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblFlavorMonitoringReportDate"> Today </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Ownership: <br />
                                <div class="col-md">
                                    <button data-role="none" class="btn btn-secondary btn-block btnProductMonitoring">
                                        <span class="lblFlavorMonitoringOwnership"> All </span>
                                    </button>
                                </div>
                            </div>

                            <div class="col-auto mb-3">
                                FOWST/Store: <br />
                                <button id="btnFlavorMonitoringReportFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblFlavorMonitoringReportFOWST"> Store </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Group: <br />
                                <button id="btnFlavorMonitoringReportGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary">
                                    <span class="lblFlavorMonitoringReportGroupFOWST"> Group Store </span>
                                </button>
                            </div>

                            @*<div class="col-auto mb-3">
                                Store Location Type:
                                <select data-role="none" id="selDailySalesReportStoreLocationType" class="form-select selstorelocationtypeProductMonitoringReport"></select>
                            </div>*@

                            <div class="col-auto mb-3">
                                Report Type:
                                <select data-role="none" id="selFlavorMonitoringReportType" class="form-select">
                                    <option value="Sales">Sales</option>
                                    <option value="Quantity">Quantity</option>
                                    <option value="TransactionCount">Transaction Count</option>
                                </select>
                            </div>
                        </div>

                        <div class="table-responsive mb-3">
                            <table data-role="none" id="tblFlavorMonitoringReport" class="table table-light table-hover table-striped" style="width:100%;">
                                <tbody></tbody>
                            </table>
                        </div>

                        <div id="popupDeviceStatus" data-bs-backdrop="true" class="modal fade" style="display:none;">
                            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"></h5>
                                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                    </div>
                                    <div class="modal-body">

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>


                    <div id="divMRMonthlySales" subpage="dashboard" style="display:none;">
                        <div class="row g-3 mb-3 align-items-center">
                            <div class="col-4 col-sm-auto">
                                Year:
                            </div>
                            <div class="col-8 col-sm-auto">
                                <select data-role="none" id="selMRMonthlySalesYear" class="form-select"></select>
                            </div>
                            <div class="col-4 col-sm-auto">
                                Report Type:
                            </div>
                            <div class="col-8 col-sm-auto">
                                <select data-role="none" id="selMRMonthlySalesReportType" class="form-select">
                                    <option value="Monthly Sales">Monthly Sales</option>
                                    <option value="Monthly Average Daily Sales">Monthly Average Daily Sales</option>
                                    <option value="Monthly Average Check">Monthly Average Check</option>
                                    <option value="Monthly Average Transaction Count">Monthly Average Transaction Count</option>
                                    <option value="Monthly Number Of Operating Stores">Monthly Number Of Operating Stores</option>
                                    <option value="Monthly Operating Days">Monthly Operating Days</option>
                                </select>
                            </div>
                        </div>

                        <div style="display: grid; height:70vh; grid-template-rows: 100% 1fr 0px;">
                            <div class="table-fixed" style="flex: 1;">
                                <table id="tblMRMonthlySales" data-role="none" class="table table-light table-hover table-striped" style="width:100%;">
                                    <thead></thead>
                                </table>
                            </div>
                        </div>
                    </div>

                    <div id="divSalesSummaryReport" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Date: <br />
                                <button id="btnSalesSummaryReportDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblSalesSummaryReportDate"> Today </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Store: <br />
                                <button id="btnDailySalesSummaryReportFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblDailySalesSummaryReportFOWST"> Store </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Group: <br />
                                <button id="btnDailySalesSummaryReportGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary">
                                    <span class="lblDailySalesSummaryReportGroupFOWST"> Group Store </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Store Type:
                                <select data-role="none" id="selDailySalesReportStoreType" class="form-select selstoretypeSalesSummaryReport"></select>
                            </div>

                            <div class="col-auto mb-3">
                                Store Location Type:
                                <select data-role="none" id="selDailySalesReportStoreLocationType" class="form-select selstorelocationtypeSalesSummaryReport"></select>
                            </div>
                        </div>

                        <div class="table-responsive mb-3">
                            <table data-role="none" id="tblSalesSummaryReport" class="table table-light table-hover table-striped" style="width:100%;">
                                <thead class="align-top text-center" style="font-size:14px;">
                                    <tr>
                                        <th class="table-active" data-sort="date" style="cursor:pointer; text-align:left"> Transaction Date </th>
                                        <th class="table-active" data-sort="date" style="cursor:pointer; text-align:left"> Store Name </th>
                                        <th class="table-active" data-sort="date" style="cursor:pointer; text-align:left"> Store Name </th>
                                        <th class="table-active" data-sort="int" style="cursor:pointer; text-align:left"> Ownership </th>
                                        <th class="table-active" data-sort="int" style="cursor:pointer; text-align:left"> Region </th>
                                        <th class="table-active" data-sort="int" style="cursor:pointer; text-align:left"> Store Type </th>
                                        <th class="table-active" data-sort="int" style="cursor:pointer; text-align:left"> Location Type </th>

                                        <th data-sort="accounting" style="cursor:pointer; text-align:left"> Net Sales </th>
                                        <th data-sort="accounting" style="cursor:pointer; text-align:left"> Transaction Count </th>
                                        <th data-sort="accounting" style="cursor:pointer; text-align:left"> Average Check </th>

                                        <th data-sort="accounting" style="cursor:pointer; text-align:left">Take Out Sales</th>
                                        <th data-sort="accounting" style="cursor:pointer; text-align:left">Take Out Transaction Count</th>
                                        <th data-sort="accounting" style="cursor:pointer; text-align:left">Take Out Average Check</th>
                                        <th data-sort="accounting" style="cursor:pointer; text-align:left">Delivery Sales</th>
                                        <th data-sort="accounting" style="cursor:pointer; text-align:left">Delivery Transaction Count</th>
                                        <th data-sort="accounting" style="cursor:pointer; text-align:left">Delivery Average Check</th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                                <tfoot>
                                    <tr>
                                        <th colspan="6" style="text-align:right">Totals:</th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                    </tr>
                                </tfoot>
                            </table>
                        </div>

                        <div id="popupDeviceStatus" data-bs-backdrop="true" class="modal fade" style="display:none;">
                            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"></h5>
                                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                    </div>
                                    <div class="modal-body">

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="divSalesAndTransactionCountMixReport" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Date: <br />
                                <button id="btnSalesAndTransactionCountMixReportDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblSalesAndTransactionCountMixReportDate"> Today </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Store: <br />
                                <button id="btnSalesAndTransactionCountMixReportFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblSalesAndTransactionCountMixReportFOWST"> Store </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Group: <br />
                                <button id="btnSalesAndTransactionCountMixReportGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary">
                                    <span class="lblSalesAndTransactionCountMixReportGroupFOWST"> Group Store </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Store Type:
                                <select data-role="none" id="selSalesAndTransactionCountMixReportStoreType" class="form-select selstoretypeSalesAndTransactionCountMixReport"></select>
                            </div>

                            <div class="col-auto mb-3">
                                Store Location Type:
                                <select data-role="none" id="selSalesAndTransactionCountMixReportStoreLocationType" class="form-select selstorelocationtypeSalesAndTransactionCountMixReport"></select>
                            </div>
                        </div>

                        <div class="table-responsive mb-3">
                            <table data-role="none" id="tblSalesAndTransactionCountMixReport" class="table table-light table-hover table-striped" style="width:100%;">
                                <thead class="align-top text-center" style="font-size:14px;">
                                    <tr>
                                        <th class="table-active" data-sort="date" style="cursor:pointer; text-align:left"> Transaction Date </th>
                                        <th class="table-active" data-sort="date" style="cursor:pointer; text-align:left"> Store Code </th>
                                        <th class="table-active" data-sort="date" style="cursor:pointer; text-align:left"> Store Name </th>
                                        <th class="table-active" data-sort="int" style="cursor:pointer; text-align:left"> Ownership </th>
                                        <th class="table-active" data-sort="int" style="cursor:pointer; text-align:left"> Region </th>
                                        <th class="table-active" data-sort="int" style="cursor:pointer; text-align:left"> Store Type </th>
                                        <th class="table-active" data-sort="int" style="cursor:pointer; text-align:left"> Location Type </th>

                                        <th data-sort="accounting" style="cursor:pointer; text-align:left"> Walk-In Sales </th>
                                        <th data-sort="accounting" style="cursor:pointer; text-align:left"> Delivery Sales </th>

                                        <th data-sort="accounting" style="cursor:pointer; text-align:left"> Walk-In Sales % </th>
                                        <th data-sort="accounting" style="cursor:pointer; text-align:left"> Delivery Sales % </th>

                                        <th data-sort="accounting" style="cursor:pointer; text-align:left"> Walk-In Sales TC </th>
                                        <th data-sort="accounting" style="cursor:pointer; text-align:left"> Delivery Sales TC </th>

                                        <th data-sort="accounting" style="cursor:pointer; text-align:left"> Walk-In Sales TC %</th>
                                        <th data-sort="accounting" style="cursor:pointer; text-align:left"> Delivery Sales TC %</th>

                                    </tr>
                                </thead>
                                <tbody></tbody>
                                <tfoot>
                                    <tr>
                                        <th colspan="5" style="text-align:right">Totals:</th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                    </tr>
                                </tfoot>
                            </table>
                        </div>

                        <div id="popupDeviceStatus" data-bs-backdrop="true" class="modal fade" style="display:none;">
                            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"></h5>
                                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                    </div>
                                    <div class="modal-body">

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="divADSandATCperStoreReport" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Date: <br />
                                <button id="btnADSandATCperStoreReportDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblADSandATCperStoreReportDate"> Today </span>
                                </button>
                            </div>


                            <div class="col-auto mb-3">
                                Group: <br />
                                <button id="btnADSandATCperStoreReportGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary">
                                    <span class="lblADSandATCperStoreReportGroupFOWST"> Group Store </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Store Type:
                                <select data-role="none" id="selADSandATCperStoreReportStoreType" class="form-select selstoretypeADSandATCperStoreReport"></select>
                            </div>

                            <div class="col-auto mb-3">
                                Store Location Type:
                                <select data-role="none" id="selADSandATCperStoreReportStoreLocationType" class="form-select selstorelocationtypeADSandATCperStoreReport"></select>
                            </div>
                        </div>

                        <div class="table-responsive mb-3">
                            <table data-role="none" id="tblADSandATCperStoreReport" class="table table-light table-hover table-striped" style="width:100%;">
                                <thead class="align-top text-center" style="font-size:14px;">
                                    <tr>
                                        <th class="table-active" data-sort="date" style="cursor:pointer; text-align:left"> Store Code </th>
                                        <th class="table-active" data-sort="date" style="cursor:pointer; text-align:left"> Store Name </th>
                                        <th class="table-active" data-sort="int" style="cursor:pointer; text-align:left"> Group </th>
                                        <th class="table-active" data-sort="int" style="cursor:pointer; text-align:left"> Store Type </th>
                                        <th class="table-active" data-sort="int" style="cursor:pointer; text-align:left"> Location Type </th>

                                        <th data-sort="accounting" style="cursor:pointer; text-align:left"> Walk-In ADS </th> @*Average Daily Sales*@
                                        <th data-sort="accounting" style="cursor:pointer; text-align:left"> Delivery ADS </th>
                                        <th data-sort="accounting" style="cursor:pointer; text-align:left"> Blended ADS </th>

                                        <th data-sort="accounting" style="cursor:pointer; text-align:left"> Walk-In ATC </th> @*Average Transaction Count*@
                                        <th data-sort="accounting" style="cursor:pointer; text-align:left"> Delivery ATC </th>
                                        <th data-sort="accounting" style="cursor:pointer; text-align:left"> Blended ATC </th>

                                    </tr>
                                </thead>
                                <tbody></tbody>
                                <tfoot>
                                    <tr>
                                        <th colspan="5" style="text-align:right">Totals:</th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                    </tr>
                                </tfoot>
                            </table>
                        </div>

                        <div id="popupDeviceStatus" data-bs-backdrop="true" class="modal fade" style="display:none;">
                            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"></h5>
                                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                    </div>
                                    <div class="modal-body">

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="divConsumptionReport" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Date: <br />
                                <button id="btnConsumptionReportDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblConsumptionReportDate"> Today </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Store: <br />
                                <button id="btnConsumptionReportFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblConsumptionReportFOWST"> Store </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Group: <br />
                                <button id="btnConsumptionReportGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary">
                                    <span class="lblConsumptionReportGroupFOWST"> Group Store </span>
                                </button>
                            </div>

                        </div>

                        <div class="table-responsive mb-3">
                            <table data-role="none" id="tblConsumptionReport" class="table table-light table-hover table-striped" style="width:100%;">
                                <thead>

                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>
                    </div>

                    <div id="divDailySalesPerSKUReport" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Date: <br />
                                <button id="btnDailySalesPerSKUReportDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblDailySalesPerSKUReportDate"> Today </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Store: <br />
                                <button id="btnDailySalesPerSKUReportFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblDailySalesPerSKUReportFOWST"> Store </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Group: <br />
                                <button id="btnDailySalesPerSKUReportGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary">
                                    Select Group
                                </button>
                            </div>
                        </div>

                        <table data-role="none" id="tblDailySalesPerSKUReport" class="table table-light table-hover table-striped" style="width:100%;">
                            <thead>

                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>

                    <div id="divDailySalesPerStoreReport" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Date: <br />
                                <button id="btnDailySalesPerStoreReportDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblDailySalesPerStoreReport"> Today </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Store: <br />
                                <button id="btnDailySalesPerStoreReportFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    Select Store
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Group: <br />
                                <button id="btnDailySalesPerStoreReportGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary">
                                    <span class="lblDailySalesPerStoreReportGroupFOWST"> Select Group </span>
                                </button>
                            </div>
                        </div>

                        <table data-role="none" id="tblDailySalesPerStoreReport" class="table table-light table-hover table-striped" style="width:100%;">
                            <thead>

                            </thead>
                            <tbody></tbody>
                        </table>

                        <div id="popupDeviceStatus" data-bs-backdrop="true" class="modal fade" style="display:none;">
                            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"></h5>
                                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                    </div>
                                    <div class="modal-body">

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="divESalesReport" subpage="dashboard" style="display:none;">
                        <div class="row mb-3 gy-2 gx-3 mb-4 align-items-center">
                            <div class="col-md-auto">
                                <div class="row">
                                    <label for="btnESalesDateFrom" class="col-2 col-md-auto col-form-label">Date:</label>
                                    <div class="col col-md-auto">
                                        <div class="btn-group">
                                            <button data-role="none" class="btn btn-light border rounded-1" id="btnESalesDateFrom">From</button>
                                            <button data-role="none" class="btn">to</button>
                                            <button data-role="none" class="btn btn-light border rounded-1" id="btnESalesDateTo">To</button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-auto d-none">

                                @*<div class="input-group">
                    <select data-role="none" class="form-select" id="selESalesDateMonth"></select>
                    <select data-role="none" class="form-select" id="selESalesDateYear"></select>
                </div>*@
                            </div>
                            <div class="col-md-auto">
                                <div class="row">
                                    <label for="btnESalesFowst" class="col-2 col-md-auto col-form-label">Store:</label>
                                    <div class="col col-md-auto">
                                        <button style="margin-right:10px;" data-role="none" class="btn btn-light border btn-block" id="btnESalesFowst">
                                            Select Store
                                        </button>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-auto">
                                <div class="row">
                                    <label for="selESalesMachine" class="col-2 col-md-auto col-form-label">Machine:</label>
                                    <div class="col col-md-auto">
                                        <select data-role="none" class="form-select" id="selESalesMachine">
                                            <option>(Select a store first)</option>
                                        </select>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md d-none">
                                <input data-role="none" class="form-control" id="txtESalesSearch" placeholder="Search here" />
                            </div>
                            <div class="col-md text-right">
                                <button data-role="none" class="btn btn-secondary" id="btnESalesExportToCSV">Export to PDF</button>
                            </div>
                        </div>

                        <h4 class="mb-3 d-none">Summary</h4>
                        <div class="table-responsive mb-4 d-none">
                            <table id="tblESalesReport" data-role="none" class="table table-light table-hover table-striped">
                                <thead>
                                    <tr class="text-center align-top" style="font-size:14px;">
                                        <th class="fixed-col text-center" style="width:50px;">#</th>
                                        <th class="fixed-col text-center">Store Code</th>
                                        <th class="fixed-col text-center">Store Name</th>
                                        @*<th class="fixed-col text-center">Store Address</th>*@
                                        <th class="fixed-col text-center">Gross Sales</th>
                                        <th class="fixed-col text-center">Vatable Sales</th>
                                        <th class="fixed-col text-center">VAT Exempt Sales</th>
                                        <th class="fixed-col text-center">VAT</th>
                                        <th class="fixed-col text-center">Discount</th>
                                        <th class="fixed-col text-center">Beginning Invoice No.</th>
                                        <th class="fixed-col text-center">Ending Invoice No.</th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>

                        <h4 class="mb-3">Details</h4>
                        <div class="d-inline-block mb-3 overflow-auto w-100">
                            <ul class="nav nav-tabs" role="tablist" style="display:-webkit-inline-box">
                                <li class="nav-item" role="presentation">
                                    <button data-role="none" class="nav-link active" data-toggle="tab" data-target="#tabESalesSummary">Details</button>
                                </li>
                                <li class="nav-item" role="presentation">
                                    <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabESalesSCReport">Senior Citizen Book Report</button>
                                </li>
                                <li class="nav-item" role="presentation">
                                    <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabESalesPWDReport">PWD Book Report</button>
                                </li>
                                <li class="nav-item" role="presentation">
                                    <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabESalesNAACReport">NAAC (Athlete) Book Report</button>
                                </li>
                                <li class="nav-item" role="presentation">
                                    <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabESalesSPReport">Solo Parent Book Report</button>
                                </li>
                            </ul>
                        </div>                        

                        <div class="tab-content">
                            <div class="tab-pane fade show active" id="tabESalesSummary">
                                <table id="tblESalesSummary" class="table table-light table-striped table-hover w-100">
                                    <thead>
                                        <tr class="text-center align-top">
                                            <th rowspan="3" class="text-nowrap">Date</th>
                                            <th rowspan="3" class="text-nowrap">Beginning SI/OR No.</th>
                                            <th rowspan="3" class="text-nowrap">Ending SI/OR No.</th>
                                            <th rowspan="3" class="text-nowrap es-blue">Grand Accum. Sales <br />Ending Balance</th>
                                            <th rowspan="3" class="text-nowrap es-blue">Grand Accum. Sales <br />Beg. Balance</th>
                                            <th rowspan="3" class="text-nowrap es-blue">Sales Issued w/ Manual <br />SI/OR (per RR 16-2018)</th>
                                            <th rowspan="3" class="text-nowrap es-blue">Gross Sales for the Day</th>
                                            <th rowspan="3" class="text-nowrap es-blue">VATable Sales</th>
                                            <th rowspan="3" class="text-nowrap es-blue">VAT Amount</th>
                                            <th rowspan="3" class="text-nowrap es-blue">VAT Exempt Sales</th>
                                            <th rowspan="3" class="text-nowrap es-blue">Zero Rated</th>
                                            <th colspan="8" class="text-nowrap es-orange">Deductions</th>
                                            <th colspan="6" class="text-nowrap es-green">Adjustment on VAT</th>
                                            <th rowspan="3" class="text-nowrap">VAT Payable</th>
                                            @*<th rowspan="3" class="text-nowrap">Total Charges</th>*@
                                            <th rowspan="3" class="text-nowrap">Net Sales</th>
                                            <th rowspan="3" class="text-nowrap">Sales Overrun/Overflow</th>
                                            <th rowspan="3" class="text-nowrap">Total Income</th>
                                            <th rowspan="3" class="text-nowrap">Reset Counter</th>
                                            <th rowspan="3" class="text-nowrap">Z-Counter</th>
                                        </tr>
                                        <tr class="text-center align-top">
                                            <th colspan="5" class="text-nowrap es-orange">Discount</th>
                                            <th rowspan="2" class="text-nowrap es-orange">Returns</th>
                                            <th rowspan="2" class="text-nowrap es-orange">Refund</th>
                                            <th rowspan="2" class="text-nowrap es-orange">Total Deduction</th>
                                            <th colspan="3" class="text-nowrap es-green">Discount</th>
                                            <th rowspan="2" class="text-nowrap es-green">VAT on Returns</th>
                                            <th rowspan="2" class="text-nowrap es-green">Others</th>
                                            <th rowspan="2" class="text-nowrap es-green">Total VAT Adjustment</th>
                                        </tr>
                                        <tr class="text-center align-top">
                                            <th class="text-nowrap es-orange border-bottom-0">SC</th>
                                            <th class="text-nowrap es-orange border-bottom-0">PWD</th>
                                            <th class="text-nowrap es-orange border-bottom-0">NAAC</th>
                                            <th class="text-nowrap es-orange border-bottom-0">Solo Parent</th>
                                            <th class="text-nowrap es-orange border-bottom-0">Others</th>
                                            <th class="text-nowrap es-green border-bottom-0">SC</th>
                                            <th class="text-nowrap es-green border-bottom-0">PWD</th>
                                            <th class="text-nowrap es-green border-bottom-0">Others</th>
                                        </tr>
                                    </thead>
                                </table>
                            </div>

                            <div class="tab-pane fade" id="tabESalesSCReport">
                                <table id="tblESalesSCReport" class="table table-light table-striped table-hover"></table>
                            </div>

                            <div class="tab-pane fade" id="tabESalesPWDReport">
                                <table id="tblESalesPWDReport" class="table table-light table-striped table-hover"></table>
                            </div>

                            <div class="tab-pane fade" id="tabESalesNAACReport">
                                <table id="tblESalesNAACReport" class="table table-light table-striped table-hover"></table>
                            </div>

                            <div class="tab-pane fade" id="tabESalesSPReport">
                                <table id="tblESalesSPReport" class="table table-light table-striped table-hover"></table>
                            </div>
                        </div>
                    </div>

                    <div id="divInvoiceList" subpage="dashboard" style="display:none;">
                        <div class="row mb-3 gy-2 gx-3 align-items-center">
                            <div class="col-md-auto">
                                <div class="row">
                                    <label for="btnILDate" class="col-2 col-md-auto col-form-label">Date:</label>
                                    <div class="col col-md-auto">
                                        <button data-role="none" id="btnILDate" class="btn btn-light border btn-block">Today</button>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-auto">
                                <div class="row">
                                    <label for="btnILStore" class="col-2 col-md-auto col-form-label">Store:</label>
                                    <div class="col col-md-auto">
                                        <button data-role="none" id="btnILStore" class="btn btn-light border btn-block">Select Store</button>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-auto">
                                <div class="row">
                                    <label for="selILMachine" class="col-2 col-md-auto col-form-label">Machine:</label>
                                    <div class="col col-md-auto">
                                        <select data-role="none" class="form-select" id="selILMachine">
                                            <option>(Select a store first)</option>
                                        </select>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md text-right">
                                <button data-role="none" class="btn btn-secondary" id="btnInvoiceListExportToCSV">Export to CSV</button>
                            </div>
                        </div>

                        <div class="table-responsive">
                            <table id="tblInvoiceList" data-role="none" class="table table-light table-hover table-striped text-nowrap">
                                <thead>
                                    <tr class="text-center align-top" style="font-size:14px;">
                                        <th class="fixed-col text-center text-nowrap" style="width:50px;">#</th>
                                        <th class="fixed-col text-center text-nowrap">Invoice Date</th>
                                        <th class="fixed-col text-center text-nowrap">Invoice Number</th>
                                        <th class="fixed-col text-center text-nowrap">Gross Sales</th>
                                        <th class="fixed-col text-center text-nowrap" style="width:50px;"></th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>                        
                    </div>

                    <div id="divVoidItemsReport" subpage="dashboard" style="display:none;">
                        <div class="row mb-3 gy-2 gx-3 align-items-center">
                            <div class="col-md-auto">
                                <div class="row">
                                    <label for="btnVIDate" class="col-2 col-md-auto col-form-label">Date:</label>
                                    <div class="col col-md-auto">
                                        <button data-role="none" id="btnVIDate" class="btn btn-light border btn-block">Today</button>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-auto">
                                <div class="row">
                                    <label for="btnVIStore" class="col-2 col-md-auto col-form-label">Store:</label>
                                    <div class="col col-md-auto">
                                        <button data-role="none" id="btnVIStore" class="btn btn-light border btn-block">Select Store</button>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md text-right">
                                <button data-role="none" class="btn btn-secondary" id="btnVIExportToCSV">Export to CSV</button>
                            </div>
                        </div>

                        <div class="table-responsive">
                            <table id="tblVI" data-role="none" class="table table-light table-hover table-striped text-nowrap">
                                <thead>
                                    <tr class="text-center align-top" style="font-size:14px;">
                                        <th class="fixed-col text-center" style="width:50px;">#</th>
                                        <th class="fixed-col text-center">Void Number</th>
                                        <th class="fixed-col text-center">Invoice Date</th>
                                        <th class="fixed-col text-center">Invoice Number</th>
                                        <th class="fixed-col text-center">Product Description</th>
                                        <th class="fixed-col text-center">Quantity</th>
                                        <th class="fixed-col text-center">Unit Price</th>
                                        <th class="fixed-col text-center">Total</th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>                        
                    </div>

                    <div id="divFoodAggregatorsReport" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Date: <br />
                                <button id="btnFoodAggregatorsReportDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblFoodAggregatorsReport"> Today </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Store: <br />
                                <button id="btnFoodAggregatorsReportFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblFoodAggregatorsReportFOWST"> Store </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Group: <br />
                                <button id="btnFoodAggregatorsReportGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary">
                                    <span class="lblFoodAggregatorsReportGroupFOWST"> Group Store </span>
                                </button>
                            </div>
                        </div>

                        <div class="table-responsive mb-3">
                            <table data-role="none" id="tblFoodAggregatorsReport" class="table table-light table-hover table-striped" style="width:100%;">
                                <thead>

                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>
                    </div>

                    <div id="divConsolidatedPromoReport" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Date: <br />
                                <button id="btnConsolidatedPromoReportDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblConsolidatedPromoReport"> Today </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Store: <br />
                                <button id="btnConsolidatedPromoReportFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblConsolidatedPromoReportFOWST"> Store </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Group: <br />
                                <button id="btnConsolidatedPromoReportGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary">
                                    <span class="lblConsolidatedPromoReportGroupFOWST"> Group Store </span>
                                </button>
                            </div>
                        </div>

                        <div class="table-responsive mb-3">
                            <table data-role="none" id="tblConsolidatedPromoReport" class="table table-light table-hover table-striped" style="width:100%;">
                                <thead>

                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>
                    </div>

                    <div id="divConsolidatedSCDReport" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Date: <br />
                                <button id="btnConsolidatedSCDReportDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblConsolidatedSCDReportDate "> Today </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Store: <br />
                                <button id="btnConsolidatedSCDReportFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary">
                                    Select Store
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Group: <br />
                                <button id="btnConsolidatedSCDReportGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary">
                                    <span class="lblConsolidatedSCDReportGroupFOWST"> Select Group </span>
                                </button>
                            </div>

                        </div>

                        <div class="table-responsive mb-3">
                            <table data-role="none" id="tblConsolidatedSCDReport" class="table table-light table-hover table-striped" style="width:100%;">
                                <thead>

                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>

                        <div id="popupDeviceStatus" data-bs-backdrop="true" class="modal fade" style="display:none;">
                            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"></h5>
                                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                    </div>
                                    <div class="modal-body">

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="divDailyHourlySalesByWeekCategoryReport" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Date: <br />
                                <button id="btnDailyHourlySalesByWeekCategoryReportDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblDailyHourlySalesByWeekCategoryReportDate "> Today </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Store: <br />
                                <button id="btnDailyHourlySalesByWeekCategoryReportFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblDailyHourlySalesByWeekCategoryReportFOWST"> Store </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Group: <br />
                                <button id="btnDailyHourlySalesByWeekCategoryReportGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary">
                                    <span class="lblDailyHourlySalesByWeekCategoryReportGroupFOWST"> Group Store </span>
                                </button>
                            </div>
                        </div>

                        <div class="table-responsive mb-3">
                            <table data-role="none" id="tblDailyHourlySalesByWeekCategoryReport" class="table table-light table-hover table-striped" style="width:100%;">
                                <thead>
                                    <tr class="align-top" style="font-size:14px;">
                                        <th data-sort="date" style="cursor:pointer;">Time</th>
                                        <th data-sort="string" style="cursor:pointer;">Weekday</th>
                                        <th data-sort="string" style="cursor:pointer;">WeekEnd</th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>

                        <div id="popupDeviceStatus" data-bs-backdrop="true" class="modal fade" style="display:none;">
                            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"></h5>
                                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                    </div>
                                    <div class="modal-body">

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="divDailyProductMixReport" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Date: <br />
                                <button id="btnDailyProductMixReportDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblDailyProductMixReportDate "> Today </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3" id="divDailyProductMixReportSearchProduct">
                                <br />
                                <input data-role="none" class="form-control" placeholder="Search Product" id="txtDailyProductMixReportSearchProduct" />
                            </div>

                            @*<div class="col-auto mb-3">
                    FOWST/Store: <br />
                    <button id="btnDailyHourlySalesByWeekCategoryReportFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                        <span class="lblDailyHourlySalesByWeekCategoryReportFOWST"> Store </span>
                    </button>
                </div>*@

                            @*<div class="col-auto mb-3">
                    Group: <br />
                    <button id="btnDailyProductMixReportGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary">
                        <span class="lblDailyProductMixReportGroupFOWST"> Group Store </span>
                    </button>
                </div>*@
                        </div>

                        <div class="table-responsive mb-3">
                            <table data-role="none" id="tblDailyProductMixReport" class="table table-light table-hover table-striped" style="width:100%;">
                                <thead>
                                    <tr class="align-top" style="font-size:14px;">
                                        <th data-sort="date" style="cursor:pointer;">Date</th>
                                        <th data-sort="string" style="cursor:pointer;">Product Name</th>
                                        <th data-sort="string" style="cursor:pointer;">Net Sales</th>
                                        <th data-sort="string" style="cursor:pointer;">Quantity</th>
                                        <th data-sort="string" style="cursor:pointer;">Transaction Count</th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>

                        <div id="popupDeviceStatus" data-bs-backdrop="true" class="modal fade" style="display:none;">
                            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"></h5>
                                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                    </div>
                                    <div class="modal-body">

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>


                    <div id="divProductMixReportByStore" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Date: <br />
                                <button id="btnProductMixReportByStoreDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblProductMixReportByStoreDate "> Today </span>
                                </button>
                            </div>

                            @*ownership*@

                            <div class="col-auto mb-3">
                                Group: <br />
                                <button id="btnProductMixReportByStoreGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary">
                                    <span class="lblProductMixReportByStoreGroupFOWST"> Group Store </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                FOWST/Store: <br />
                                <button id="btnProductMixReportByStoreFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblProductMixReportByStoreFOWST"> Store </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Store Location Type:
                                <select data-role="none" id="selProductMixReportByStoreStoreLocationType" class="form-select selstorelocationtypeProductMonitoringReport"></select>
                            </div>

                            <div class="col-auto mb-3" id="divProductMixReportByStoreSearchProduct">
                                <br />
                                <input data-role="none" class="form-control" placeholder="Search Product" id="txtProductMixReportByStoreSearchProduct" />
                            </div>
                        </div>

                        <div class="table-responsive mb-3">
                            <table data-role="none" id="tblDailyProductMixPerStoreReport" class="table table-light table-hover table-striped" style="width:100%;">
                                <thead>
                                    <tr class="align-top" style="font-size:14px;">
                                        <th data-sort="date" style="cursor:pointer;">Store Code</th>
                                        <th data-sort="date" style="cursor:pointer;">Store Name</th>
                                        <th data-sort="string" style="cursor:pointer;">Ownership</th>
                                        <th data-sort="string" style="cursor:pointer;">Region</th>
                                        <th data-sort="string" style="cursor:pointer;">Location Type</th>
                                        <th data-sort="string" style="cursor:pointer;">Product Name</th>
                                        <th data-sort="string" style="cursor:pointer;">Net Sales</th>
                                        <th data-sort="string" style="cursor:pointer;">Quantity Sold</th>
                                        <th data-sort="string" style="cursor:pointer;">Operating Days</th>
                                        <th data-sort="string" style="cursor:pointer;">Average Transaction</th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>

                        <div id="popupDeviceStatus" data-bs-backdrop="true" class="modal fade" style="display:none;">
                            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"></h5>
                                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                    </div>
                                    <div class="modal-body">

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="divProductMonitoringReport" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Date: <br />
                                <button id="btnProductMonitoringReportDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblProductMonitoringReportDate"> Today </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Ownership: <br />
                                <div class="col-md">
                                    <button data-role="none" class="btn btn-secondary btn-block btnProductMonitoring">
                                        <span class="lblProductMonitoringOwnership"> All </span>
                                    </button>
                                </div>
                            </div>

                            <div class="col-auto mb-3">
                                FOWST/Store: <br />
                                <button id="btnProductMonitoringReportFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblProductMonitoringReportFOWST"> Store </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Group: <br />
                                <button id="btnProductMonitoringReportGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary">
                                    <span class="lblProductMonitoringReportGroupFOWST"> Group Store </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Store Location Type:
                                <select data-role="none" id="selDailySalesReportStoreLocationType" class="form-select selstorelocationtypeProductMonitoringReport"></select>
                            </div>

                            <div class="col-auto mb-3">
                                Report Type:
                                <select data-role="none" id="selProductMonitoringReportType" class="form-select">
                                    <option value="Sales">Sales</option>
                                    <option value="Quantity">Quantity</option>
                                    <option value="TransactionCount">Transaction Count</option>
                                </select>
                            </div>
                        </div>

                        <div class="table-responsive mb-3">
                            <table data-role="none" id="tblProductMonitoringReport" class="table table-light table-hover table-striped" style="width:100%;">
                                @*<thead>
                        <tr class="align-top" style="font-size:14px;">
                            <th data-sort="date" style="cursor:pointer;">Date</th>
                            <th data-sort="string" style="cursor:pointer;">Product Name</th>
                            <th data-sort="string" style="cursor:pointer;">Net Sales</th>
                            <th data-sort="string" style="cursor:pointer;">Quantity</th>
                            <th data-sort="string" style="cursor:pointer;">Transaction Count</th>
                        </tr>
                    </thead>*@
                                <tbody></tbody>
                            </table>
                        </div>

                        <div id="popupDeviceStatus" data-bs-backdrop="true" class="modal fade" style="display:none;">
                            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"></h5>
                                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                    </div>
                                    <div class="modal-body">

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="divProductMonitoringPerStoreProductReport" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Date: <br />
                                <button id="btnProductMonitoringPerStoreProductReportDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblProductMonitoringPerStoreProductReportDate"> Today </span>
                                </button>
                            </div>

                            @*<div class="col-auto mb-3">
                    Ownership: <br />
                    <div class="col-md">
                        <button data-role="none" class="btn btn-secondary btn-block btnProductMonitoring">
                            <span class="lblProductMonitoringOwnership"> All </span>
                        </button>
                    </div>
                </div>*@

                            <div class="col-auto mb-3">
                                FOWST/Store: <br />
                                <button id="btnProductMonitoringPerStoreProductReportFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblProductMonitoringPerStoreProductReportFOWST"> Store </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Group: <br />
                                <button id="btnProductMonitoringPerStoreProductReportGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary">
                                    <span class="lblProductMonitoringPerStoreProductReportGroupFOWST"> Group Store </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Store Location Type:
                                <select data-role="none" id="selDailySalesReportStoreLocationType" class="form-select selstorelocationtypeProductMonitoringPerStoreProductReport"></select>
                            </div>

                            <div class="col-auto mb-3">
                                Product:
                                <br />
                                <input data-role="none" class="form-control" placeholder="Search Product" id="txtDailyProductMonitoringPerStoreProductReportSearchProduct" />
                            </div>

                        </div>

                        <div class="table-responsive mb-3">
                            <table data-role="none" id="tblProductMonitoringPerStoreProductReport" class="table table-light table-hover table-striped" style="width:100%;">
                                <thead>
                                    <tr class="align-top" style="font-size:14px;">
                                        <th data-sort="date" style="cursor:pointer;">Store Code</th>
                                        <th data-sort="string" style="cursor:pointer;">Store Name</th>
                                        <th data-sort="string" style="cursor:pointer;">Ownership</th>
                                        <th data-sort="string" style="cursor:pointer;">Region</th>
                                        <th data-sort="string" style="cursor:pointer;">LocationType</th>
                                        <th data-sort="string" style="cursor:pointer;">Product Code</th>
                                        <th data-sort="string" style="cursor:pointer;">Product Name</th>
                                        <th data-sort="string" style="cursor:pointer;">Net Sales</th>
                                        <th data-sort="string" style="cursor:pointer;">Quantity</th>
                                        <th data-sort="string" style="cursor:pointer;">Transaction Count</th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>

                        <div id="popupDeviceStatus" data-bs-backdrop="true" class="modal fade" style="display:none;">
                            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"></h5>
                                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                    </div>
                                    <div class="modal-body">

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="divProductMonitoringMonthlySummary" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Report Type:
                                <select data-role="none" id="selProductMonitoringMonthlySummaryReportType" class="form-select">
                                    <option value="Sales">Sales</option>
                                    <option value="Quantity">Quantity</option>
                                    <option value="TransactionCount">Transaction Count</option>
                                </select>
                            </div>
                            <div class="col-auto mb-3">
                                <br />
                                <button id="btnProductMonitoringMonthlySummaryGo" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblProductMonitoringMonthlySummaryGo"> Go </span>
                                </button>
                            </div>

                            @*<div class="col-auto mb-3">
                    Ownership: <br />
                    <div class="col-md">
                        <button data-role="none" class="btn btn-secondary btn-block btnProductMonitoring">
                            <span class="lblProductMonitoringOwnership"> All </span>
                        </button>
                    </div>
                </div>*@

                            @*<div class="col-auto mb-3">
                    FOWST/Store: <br />
                    <button id="btnProductMonitoringPerStoreProductReportFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                        <span class="lblProductMonitoringPerStoreProductReportFOWST"> Store </span>
                    </button>
                </div>

                <div class="col-auto mb-3">
                    Group: <br />
                    <button id="btnProductMonitoringPerStoreProductReportGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary">
                        <span class="lblProductMonitoringPerStoreProductReportGroupFOWST"> Group Store </span>
                    </button>
                </div>

                <div class="col-auto mb-3">
                    Store Location Type:
                    <select data-role="none" id="selDailySalesReportStoreLocationType" class="form-select selstorelocationtypeProductMonitoringPerStoreProductReport"></select>
                </div>

                <div class="col-auto mb-3">
                    Product:
                    <br />
                    <input data-role="none" class="form-control" placeholder="Search Product" id="txtDailyProductMonitoringPerStoreProductReportSearchProduct" />
                </div>*@

                        </div>

                        <div class="table-responsive mb-3">
                            <table data-role="none" id="tblProductProductMonitoringMonthlySummary" class="table table-light table-hover table-striped" style="width:100%;">
                            </table>
                        </div>

                        <div id="popupDeviceStatus" data-bs-backdrop="true" class="modal fade" style="display:none;">
                            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"></h5>
                                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                    </div>
                                    <div class="modal-body">

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="divWeeklySalesReport" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Week:
                                <select data-role="none" id="selWeeklySalesReportWeek" class="form-select"></select>
                            </div>

                            <div class="col-auto mb-3">
                                Year: <br />
                                <select data-role="none" id="selWeeklySalesReportYear" class="form-select"></select>
                            </div>

                            <div class="col-auto mb-3">
                                Date Range: <br />
                                <button id="btnWeeklySalesReportDateRange" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblWeeklySalesReportDateRange"> - </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                <br />
                                <button id="btnWeeklySalesReportGetReport" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblWeeklySalesReportGetReport"> Get Report </span>
                                </button>
                            </div>

                            <div class="col" style="text-align:right">
                                <br />
                                <button id="btnFourWeeksSalesReport" style="margin-right:10px;" data-role="none" class="btn btn-secondary">
                                    <span class="btnFourWeeksSalesReport"> View 4 Weeks Sales Report</span>
                                </button>
                            </div>
                        </div>

                        <div class="table-responsive mb-3" id="dvWeekySalesReport">
                            <div id="divManagementReportBodyRow1" class="row row-cols-1 row-cols-sm-1 row-cols-md-2 row-cols-lg-3 row-cols-xl-3 row-cols-xxl-3">

                            </div>
                            <div id="divManagementReportBodyRow2" class="row row-cols-1 row-cols-sm-1 row-cols-md-2 row-cols-lg-3 row-cols-xl-3 row-cols-xxl-3">

                            </div>
                            <div id="divManagementReportBodyRow3" class="row row-cols-1 row-cols-sm-1 row-cols-md-2 row-cols-lg-3 row-cols-xl-3 row-cols-xxl-3">

                            </div>
                        </div>

                        <div class="table-responsive mb-3" id="dvFourWeeksSalesReport">
                            <table data-role="none" id="tblFourWeeksSalesReport" class="table table-light table-hover table-striped" style="width:100%;"></table>
                        </div>

                        <div id="popupDeviceStatus" data-bs-backdrop="true" class="modal fade" style="display:none;">
                            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"></h5>
                                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                    </div>
                                    <div class="modal-body">

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="divWeeklyAverageDailySalesReport" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Week:
                                <select data-role="none" id="selWeeklyAverageDailySalesReportWeek" class="form-select"></select>
                            </div>

                            <div class="col-auto mb-3">
                                Year: <br />
                                <select data-role="none" id="selWeeklyAverageDailySalesReportYear" class="form-select"></select>
                            </div>

                            <div class="col-auto mb-3">
                                Date Range: <br />
                                <button id="btnWeeklyAverageDailySalesReportDateRange" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblWeeklyAverageDailySalesReportDateRange"> - </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                <br />
                                <button id="btnWeeklyAverageDailySalesReportGetReport" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblWeeklyAverageDailySalesReportGetReport"> Get Report </span>
                                </button>
                            </div>

                            @*<div class="col" style="text-align:right">
                    <br />
                    <button id="btnFourWeeksSalesReport" style="margin-right:10px;" data-role="none" class="btn btn-secondary">
                        <span class="btnFourWeeksSalesReport"> View 4 Weeks Sales Report</span>
                    </button>
                </div>*@
                        </div>

                        <div class="table-responsive mb-3" id="dvWeekySalesReport">
                            <div id="divManagementReportBodyRow1" class="row row-cols-1 row-cols-sm-1 row-cols-md-2 row-cols-lg-3 row-cols-xl-3 row-cols-xxl-3">

                            </div>
                            <div id="divManagementReportBodyRow2" class="row row-cols-1 row-cols-sm-1 row-cols-md-2 row-cols-lg-3 row-cols-xl-3 row-cols-xxl-3">

                            </div>
                            <div id="divManagementReportBodyRow3" class="row row-cols-1 row-cols-sm-1 row-cols-md-2 row-cols-lg-3 row-cols-xl-3 row-cols-xxl-3">

                            </div>
                        </div>

                        <div class="table-responsive mb-3" id="dvFourWeeksSalesReport">
                            <table data-role="none" id="tblFourWeeksADSReport" class="table table-light table-hover table-striped" style="width:100%;"></table>
                        </div>

                        <div id="popupDeviceStatus" data-bs-backdrop="true" class="modal fade" style="display:none;">
                            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"></h5>
                                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                    </div>
                                    <div class="modal-body">

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="divWeeklyWeeklyTransactionCountReport" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Week:
                                <select data-role="none" id="selWeeklyTransactionCountReportWeek" class="form-select"></select>
                            </div>

                            <div class="col-auto mb-3">
                                Year: <br />
                                <select data-role="none" id="selWeeklyTransactionCountReportYear" class="form-select"></select>
                            </div>

                            <div class="col-auto mb-3">
                                Date Range: <br />
                                <button id="btnWeeklyTransactionCountReportDateRange" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblWeeklyTransactionCountReportDateRange"> - </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                <br />
                                <button id="btnWeeklyTransactionCountReportGetReport" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblWeeklyTransactionCountReportGetReport"> Get Report </span>
                                </button>
                            </div>

                            @*<div class="col" style="text-align:right">
                    <br />
                    <button id="btnFourWeeksSalesReport" style="margin-right:10px;" data-role="none" class="btn btn-secondary">
                        <span class="btnFourWeeksSalesReport"> View 4 Weeks Sales Report</span>
                    </button>
                </div>*@
                        </div>

                        <div class="table-responsive mb-3" id="dvWeekySalesReport">
                            <div id="divManagementReportBodyRow1" class="row row-cols-1 row-cols-sm-1 row-cols-md-2 row-cols-lg-3 row-cols-xl-3 row-cols-xxl-3">

                            </div>
                            <div id="divManagementReportBodyRow2" class="row row-cols-1 row-cols-sm-1 row-cols-md-2 row-cols-lg-3 row-cols-xl-3 row-cols-xxl-3">

                            </div>
                            <div id="divManagementReportBodyRow3" class="row row-cols-1 row-cols-sm-1 row-cols-md-2 row-cols-lg-3 row-cols-xl-3 row-cols-xxl-3">

                            </div>
                        </div>

                        <div class="table-responsive mb-3" id="dvFourWeeksSalesReport">
                            <table data-role="none" id="tblFourWeeksWeeklyTransactionCountReport" class="table table-light table-hover table-striped" style="width:100%;"></table>
                        </div>

                        <div id="popupDeviceStatus" data-bs-backdrop="true" class="modal fade" style="display:none;">
                            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"></h5>
                                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                    </div>
                                    <div class="modal-body">

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="divWeeklyAverageCheckReport" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Week:
                                <select data-role="none" id="selWeeklyAverageCheckReportWeek" class="form-select"></select>
                            </div>

                            <div class="col-auto mb-3">
                                Year: <br />
                                <select data-role="none" id="selWeeklyAverageCheckReportYear" class="form-select"></select>
                            </div>

                            <div class="col-auto mb-3">
                                Date Range: <br />
                                <button id="btnWeeklyAverageCheckReportDateRange" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblWeeklyAverageCheckReportDateRange"> - </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                <br />
                                <button id="btnWeeklyAverageCheckReportGetReport" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblWeeklyAverageCheckReportGetReport"> Get Report </span>
                                </button>
                            </div>

                        </div>

                        <div class="table-responsive mb-3" id="dvFourWeeksSalesReport">
                            <table data-role="none" id="tblFourWeeksWeeklyAverageCheckReport" class="table table-light table-hover table-striped" style="width:100%;"></table>
                        </div>

                        <div id="popupDeviceStatus" data-bs-backdrop="true" class="modal fade" style="display:none;">
                            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"></h5>
                                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                    </div>
                                    <div class="modal-body">

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="divManagementMonthlySalesReport" subpage="dashboard" style="display:none;">
                        <div class="row">
                            @*<div class="col-auto mb-3">
                    Month
                    <select data-role="none" id="selMonthlySalesReportMonth" class="form-select"></select>
                </div>*@

                            <div class="col-auto mb-3">
                                Year: <br />
                                <select data-role="none" id="selMonthlySalesReportYear" class="form-select"></select>
                            </div>

                            <div class="col-auto mb-3">
                                <br />
                                <button id="btnMonthlySalesReportGetReport" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblMonthlySalesReportGetReport"> Get Report </span>
                                </button>
                            </div>
                        </div>

                        <div class="table-responsive mb-3" id="dvWeekySalesReport">
                            <div id="divManagementReportBodyRow1" class="row row-cols-1 row-cols-sm-1 row-cols-md-2 row-cols-lg-3 row-cols-xl-3 row-cols-xxl-3">

                            </div>
                            <div id="divManagementReportBodyRow2" class="row row-cols-1 row-cols-sm-1 row-cols-md-2 row-cols-lg-3 row-cols-xl-3 row-cols-xxl-3">

                            </div>
                            <div id="divManagementReportBodyRow3" class="row row-cols-1 row-cols-sm-1 row-cols-md-2 row-cols-lg-3 row-cols-xl-3 row-cols-xxl-3">

                            </div>
                        </div>

                        <div class="table-responsive mb-3" id="dvFourWeeksSalesReport">
                            <table data-role="none" id="tblMonthlySalesReports" class="table table-light table-hover table-striped" style="width:100%;"></table>
                        </div>

                        <div id="popupDeviceStatus" data-bs-backdrop="true" class="modal fade" style="display:none;">
                            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"></h5>
                                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                    </div>
                                    <div class="modal-body">

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="divSalesPaymentInformationReport" subpage="dashboard" style="display:none;">
                        <div class="row mb-3 gy-2 gx-3 align-items-center">
                            <div class="col-md-auto">
                                <div class="row">
                                    <label for="btnSalesPaymentInformationReportDate" class="col-2 col-md-auto col-form-label">Date:</label>
                                    <div class="col col-md-auto">
                                        <button data-role="none" id="btnSalesPaymentInformationReportDate" class="btn btn-light border btn-block"><span class="lblSalesPaymentInformationReportDate "> Today </span></button>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-auto">
                                <div class="row">
                                    <label for="btnSalesPaymentInformationReportFOWST" class="col-2 col-md-auto col-form-label">Store:</label>
                                    <div class="col col-md-auto">
                                        <button data-role="none" id="btnSalesPaymentInformationReportFOWST" class="btn btn-light border btn-block"><span class="lblSalesPaymentInformationReportFOWST"> Select Store </span></button>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-auto d-none">
                                <div class="row">
                                    <label for="btnSalesPaymentInformationReportGroupFOWST" class="col-2 col-md-auto col-form-label">Group:</label>
                                    <div class="col col-md-auto">
                                        <button data-role="none" id="btnSalesPaymentInformationReportGroupFOWST" class="btn btn-light border btn-block"><span class="lblSalesPaymentInformationReportGroupFOWST"> Select Group </span></button>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="table-responsive mb-3">
                            <table data-role="none" id="tblSalesPaymentInformationReport" class="table table-light table-hover table-striped" style="width:100%;">
                                <thead>
                                    <tr class="align-top" style="font-size:14px;">
                                        <th data-sort="date" style="cursor:pointer;">Store</th>
                                        <th data-sort="string" style="cursor:pointer;">Invoice Date</th>
                                        <th data-sort="string" style="cursor:pointer;">Invoice No</th>
                                        <th data-sort="string" style="cursor:pointer;">Payment Type</th>
                                        <th data-sort="string" style="cursor:pointer;">Amount</th>
                                        <th data-sort="string" style="cursor:pointer;">Reference No</th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>

                        <div id="popupDeviceStatus" data-bs-backdrop="true" class="modal fade" style="display:none;">
                            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"></h5>
                                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                    </div>
                                    <div class="modal-body">

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="divCrewOnDutyReport" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Date: <br />
                                <button id="btnCrewOnDutyReportDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblCrewOnDutyReportDate "> Today </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Store: <br />
                                <button id="btnCrewOnDutyReportFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblCrewOnDutyReportFOWST"> Store </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Group: <br />
                                <button id="btnCrewOnDutyReportGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary">
                                    <span class="lblCrewOnDutyReportGroupFOWST"> Group Store </span>
                                </button>
                            </div>
                        </div>

                        <div class="table-responsive mb-3">
                            <table data-role="none" id="tblCrewOnDutyReport" class="table table-light table-hover table-striped" style="width:100%;">
                                <thead>
                                    <tr class="align-top" style="font-size:14px;">
                                        <th data-sort="date" style="cursor:pointer;">Transaction Date</th>
                                        <th data-sort="string" style="cursor:pointer;">Store Name</th>
                                        <th data-sort="string" style="cursor:pointer;">Region</th>
                                        <th data-sort="string" style="cursor:pointer;">Crew Name</th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>

                        <div id="popupDeviceStatus" data-bs-backdrop="true" class="modal fade" style="display:none;">
                            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"></h5>
                                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                    </div>
                                    <div class="modal-body">

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="divInventoryReportCrewVSAM" subpage="dashboard" style="display:none;">
                        <div class="row">

                            <div class="col-auto mb-3">
                                Group: <br />
                                <button id="btnInventoryReportCrewVSAMGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary">
                                    <span class="lblInventoryReportCrewVSAMGroupFOWST"> Group FOWST/Store </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                FOWST/Store: <br />
                                <button id="btnInventoryReportCrewVSAMFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblInventoryReportCrewVSAMFOWST"> FOWST/Store </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Date: <br />
                                <button id="btnInventoryReportCrewVSAMDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblInventoryReportCrewVSAMDate"> Select Date </span>
                                </button>

                                @*<select data-role="none" id="selInventoryReportCrewVSAMDate" class="form-select">
                        <option value="">Select Date</option>
                    </select>*@
                            </div>
                        </div>

                        <div class="table-responsive mb-3" id="dvInventoryReportCrewVSAMReport">
                            <table data-role="none" id="tblInventoryReportCrewVSAMReport" class="table table-light table-hover table-striped" style="width:100%;">
                                <thead>
                                    <tr class="align-top text-center" style="font-size:14px;">
                                        <th data-sort="string" style="cursor:pointer;"> Product </th>
                                        <th data-sort="string" style="cursor:pointer;"> Cost </th>
                                        <th data-sort="string" style="cursor:pointer;"> Unit Measure </th>
                                        <th data-sort="int" style="cursor:pointer;"> Physical Beginning Inventory </th>
                                        <th data-sort="int" style="cursor:pointer;"> Delivery </th>
                                        <th data-sort="int" style="cursor:pointer;"> Receive from Branch </th>
                                        <th data-sort="int" style="cursor:pointer;"> Transfer to Branch </th>
                                        <th data-sort="int" style="cursor:pointer;"> Shortlanded </th>
                                        <th data-sort="int" style="cursor:pointer;"> Theoretical Usage </th>
                                        <th data-sort="int" style="cursor:pointer;"> Ending Balance </th>
                                        @*<th data-sort="int" style="cursor:pointer;"> Variance </th>*@
                                        <th data-sort="int" style="cursor:pointer;"> Area Manager </th>
                                        <th data-sort="int" style="cursor:pointer;"> Area Manager theoretical Balance  </th>
                                        <th data-sort="int" style="cursor:pointer;"> Area Manager Count </th>
                                        <th data-sort="int" style="cursor:pointer;"> Area Manager Variance </th>
                                        <th data-sort="int" style="cursor:pointer;"> Area Manager Count Date </th>
                                        <th data-sort="int" style="cursor:pointer;"> Crew </th>
                                        <th data-sort="int" style="cursor:pointer;"> Crew theoretical Balance  </th>
                                        <th data-sort="int" style="cursor:pointer;"> Crew Count </th>
                                        <th data-sort="int" style="cursor:pointer;"> Area Count Variance </th>
                                        <th data-sort="int" style="cursor:pointer;"> Crew Count Date </th>

                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>

                        <div class="table-responsive mb-3" id="dvInventoryReportCrewVSAMReportPerGroup">
                            <table data-role="none" id="tblInventoryReportCrewVSAMReportPerGroup" class="table table-light table-hover table-striped" style="width:100%;"></table>
                        </div>

                        <div id="popupDeviceStatus" data-bs-backdrop="true" class="modal fade" style="display:none;">
                            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"></h5>
                                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                    </div>
                                    <div class="modal-body">

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="divFoodCostReport" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Date: <br />
                                <button id="btnFoodCostReportDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblFoodCostReportDate "> Today </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                FOWST/Store: <br />
                                <button id="btnFoodCostReportFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblFoodCostReportFOWST"> FOWST/Store </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Group: <br />
                                <button id="btnFoodCostReportGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary">
                                    <span class="lblFoodCostReportGroupFOWST"> Group FOWST/Store </span>
                                </button>
                            </div>
                        </div>

                        <div class="table-responsive mb-3">
                            <table data-role="none" id="tblFoodCostReport" class="table table-light table-hover table-striped" style="width:100%;">
                                <thead>
                                    <tr class="align-top" style="font-size:14px;">
                                        <th data-sort="date" style="cursor:pointer;">Product</th>
                                        <th data-sort="string" style="cursor:pointer;">SRP</th>
                                        <th data-sort="string" style="cursor:pointer;">Vat Ex SRP </th>
                                        <th data-sort="string" style="cursor:pointer;">Qty Sold</th>
                                        <th data-sort="string" style="cursor:pointer;">Vat Ex Sales</th>
                                        <th data-sort="string" style="cursor:pointer;">Cost</th>
                                        <th data-sort="string" style="cursor:pointer;">Total Cost</th>
                                        <th data-sort="string" style="cursor:pointer;">Margin %</th>
                                        <th data-sort="string" style="cursor:pointer;">Total Cost %</th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>

                        <div id="popupDeviceStatus" data-bs-backdrop="true" class="modal fade" style="display:none;">
                            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"></h5>
                                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                    </div>
                                    <div class="modal-body">

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="divDailyHourlySalesReport" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Date: <br />
                                <button id="btnDailyHourlySalesReportDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblDailyHourlySalesReportDate "> Today </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Store: <br />
                                <button id="btnDailyHourlySalesReportFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblDailyHourlySalesReportFOWST"> Store </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Group: <br />
                                <button id="btnDailyHourlySalesReportGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary">
                                    <span class="lblDailyHourlySalesReportGroupFOWST"> Group Store </span>
                                </button>
                            </div>
                        </div>

                        <div class="table-responsive mb-3">
                            <table data-role="none" id="tblDailyHourlySalesReport" class="table table-light table-hover table-striped" style="width:100%;">
                                <thead>

                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>

                        <div id="popupDeviceStatus" data-bs-backdrop="true" class="modal fade" style="display:none;">
                            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"></h5>
                                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                    </div>
                                    <div class="modal-body">

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="divConsolidatedUsageperDay" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Date: <br />
                                <button id="btnConsolidatedUsageperDayDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblConsolidatedUsageperDayDate"> Today </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Store: <br />
                                <button id="btnConsolidatedUsageperDayFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblConsolidatedUsageperDayFOWST"> Store </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Group: <br />
                                <button id="btnConsolidatedUsageperDayGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary">
                                    <span class="lblConsolidatedUsageperDayGroupFOWST"> Group Store </span>
                                </button>
                            </div>
                        </div>

                        <div class="table-responsive mb-3">
                            <table data-role="none" id="tblConsolidatedUsageperDay" class="table table-light table-hover table-striped" style="width:100%;">
                                <thead>

                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>

                        <div id="popupDeviceStatus" data-bs-backdrop="true" class="modal fade" style="display:none;">
                            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"></h5>
                                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                    </div>
                                    <div class="modal-body">

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="divConsolidatedEndingBalanceperDay" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-auto mb-3">
                                Date: <br />
                                <button id="btnConsolidatedEndingBalanceperDayDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblConsolidatedEndingBalanceperDayDate"> Today </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Store: <br />
                                <button id="btnConsolidatedEndingBalanceperDayFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblConsolidatedEndingBalanceperDayFOWST"> Store </span>
                                </button>
                            </div>

                            <div class="col-auto mb-3">
                                Group: <br />
                                <button id="btnConsolidatedEndingBalanceperDayGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary">
                                    <span class="lblConsolidatedEndingBalanceperDayGroupFOWST"> Group Store </span>
                                </button>
                            </div>
                        </div>

                        <div class="table-responsive mb-3">
                            <table data-role="none" id="tblConsolidatedEndingBalanceperDay" class="table table-light table-hover table-striped" style="width:100%;">
                                <thead>

                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>

                        <div id="popupDeviceStatus" data-bs-backdrop="true" class="modal fade" style="display:none;">
                            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"></h5>
                                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                    </div>
                                    <div class="modal-body">

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="divDBDeviceTracker" subpage="dashboard" style="display:none;"></div>

                    <div id="divDBOfflineTransactionHistory" subpage="dashboard" style="display:none;"></div>

                    <div id="divDBOfflineProductHistory" subpage="dashboard" style="display:none;"></div>

                    <div id="divDBInventoryCountHistory" subpage="dashboard" style="display:none;"></div>

                    <div id="divSalesReport" subpage="dashboard" style="display:none;">
                        <div class="row">
                            <div class="col-6 col-sm-4 col-md-3 mb-3">
                                Date: <br />
                                <button id="btnSalesReportDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblSalesReportDate"> Today </span>
                                </button>
                            </div>


                            <div class="col-6 col-sm-4 col-md-3 mb-3">
                                Group: <br />
                                <button id="btnDailySalesReportGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblDailySalesReportGroupFOWST"> Group Store </span>
                                </button>
                            </div>

                            <div class="col-6 col-sm-4 col-md-3 mb-3">
                                Store Type:
                                <select data-role="none" id="selDailySalesReportStoreType" class="form-select selstoretypeSalesReport"></select>
                            </div>
                            <div class="col-6 col-sm-4 col-md-3 mb-3">
                                Store Location Type:
                                <select data-role="none" id="selDailySalesReportStoreLocationType" class="form-select selstorelocationtypeSalesReport"></select>
                            </div>

                            <div class="col-6 col-sm-4 col-md-3 mb-3">
                                No. of Company stores with sales data:
                                <button data-role="none" class="btn btn-light btn-block border" id="lblNoOfActiveStoresCO">-</button>
                            </div>

                            <div class="col-6 col-sm-4 col-md-3 mb-3">
                                No. of Franchise stores with sales data:
                                <button data-role="none" class="btn btn-light btn-block border" id="lblNoOfActiveStoresFO">-</button>
                            </div>

                            <div class="col-6 col-sm-4 col-md-3 mb-3">
                                No. of stores without sales data:
                                <button data-role="none" class="btn btn-light btn-block border" id="lblNoOfInactiveStores">-</button>
                            </div>

                            <div class="col-6 col-sm-4 col-md-3 mb-3">
                                No. of closed stores:
                                <button data-role="none" class="btn btn-light btn-block border" id="lblNoOfCloseStores">-</button>
                            </div>
                        </div>

                        <div class=" mb-3">
                            <div class="row m-0 gy-2 gx-3 mb-3 align-items-center">
                                <div class="col-sm-auto">
                                    Ownership:
                                </div>
                                <div class="col-sm">
                                    <select data-role="none" id="selSalesReportOwnership" class="form-select fixed">
                                        <option value="All">All</option>
                                        <option value="0">Company-Owned</option>
                                        <option value="1">Franchise</option>
                                    </select>
                                </div>
                                <div class="col-sm text-right">
                                    <button data-role="none" class="btn btn-secondary" id="btnDownloadSalesReport">Download CSV</button>
                                </div>
                            </div>

                            <table data-role="none" id="tblSalesReport" class="table table-light table-hover table-striped" style="width:100%;">
                                <thead class="align-top text-center" style="font-size:14px;">
                                    <tr>
                                        <th class="table-active fixed-col excludecolspan" colspan="5">Store Information</th>
                                        <th colspan="10">Sales Information</th>
                                        <th class="table-active thSalesByStorePaymentType" colspan="1">Card Payment <button data-role="none" class="btn" id="btnToggleSalesByStorePaymentTypeDetails" data-toggle="tooltip" title="Show details"><span class="bi-chevron-right"></span></button></th>
                                        <th class="thSalesByStoreEWallet" colspan="1">E-Wallet <button data-role="none" class="btn" id="btnToggleSalesByStoreEWalletDetails" data-toggle="tooltip" title="Show details"><span class="bi-chevron-right"></span></button></th>
                                        <th class="table-active thSalesByStoreDeliveries" colspan="1">Deliveries <button data-role="none" class="btn" id="btnToggleSalesByStoreDeliveriesDetails" data-toggle="tooltip" title="Show details"><span class="bi-chevron-right"></span></button></th>
                                        <th colspan="5">Order Method</th>
                                    </tr>
                                    <tr>
                                        <th class="table-active fixed-col" data-sort="date" style="cursor:pointer;"> Store Code </th>
                                        <th class="table-active fixed-col" data-sort="date" style="cursor:pointer;"> Store Name </th>
                                        <th class="table-active fixed-col" data-sort="int" style="cursor:pointer;"> Group </th>
                                        <th class="table-active fixed-col" data-sort="int" style="cursor:pointer;"> Store Type </th>
                                        <th class="table-active fixed-col" data-sort="int" style="cursor:pointer;"> Location Type </th>

                                        <th data-sort="accounting" class="text-nowrap fixed-col" style="cursor:pointer;"> Transaction Count </th>
                                        <th data-sort="accounting" class="text-nowrap fixed-col" style="cursor:pointer;"> Average Check </th>
                                        <th data-sort="accounting" class="text-nowrap fixed-col" style="cursor:pointer;"> Gross Sales </th>
                                        <th data-sort="accounting" class="text-nowrap fixed-col" style="cursor:pointer;"> Sales Return/Refund </th>
                                        <th data-sort="accounting" class="text-nowrap fixed-col" style="cursor:pointer;"> Total Discount </th>
                                        <th data-sort="accounting" class="text-nowrap fixed-col" style="cursor:pointer;"> Net Sales </th>
                                        <th data-sort="accounting" class="text-nowrap fixed-col" style="cursor:pointer;"> Total Cash</th>
                                        <th data-sort="accounting" class="text-nowrap fixed-col" style="cursor:pointer;"> Total Gift Cert</th>
                                        <th data-sort="accounting" class="text-nowrap fixed-col" style="cursor:pointer;"> Total Voucher</th>
                                        <th data-sort="accounting" class="text-nowrap fixed-col" style="cursor:pointer;"> Total PAS</th>

                                        <th class="table-active text-nowrap fixed-col colSalesByStoreAccumulatedPayment" data-sort="accounting" style="cursor:pointer; display:revert; min-width:200px;">Total Card Payment</th>
                                        <th class="table-active text-nowrap fixed-col colSalesByStorePaymentdetails" data-sort="accounting" style="cursor:pointer; display:none;">Credit Card</th>
                                        <th class="table-active text-nowrap fixed-col colSalesByStorePaymentdetails" data-sort="accounting" style="cursor:pointer; display:none;">Debit Card</th>

                                        <th class="colSalesByStoreAccumulatedEWallet text-nowrap fixed-col" data-sort="accounting" style="cursor:pointer; display:revert; min-width:200px;">Total E-Wallet</th>
                                        <th class="colSalesByStoreEWalletdetails text-nowrap fixed-col" data-sort="accounting" style="cursor:pointer; display:none;">G Cash</th>
                                        <th class="colSalesByStoreEWalletdetails text-nowrap fixed-col" data-sort="accounting" style="cursor:pointer; display:none;">Pay Maya</th>
                                        <th class="colSalesByStoreEWalletdetails text-nowrap fixed-col" data-sort="accounting" style="cursor:pointer; display:none;">SMAC</th>
                                        <th class="colSalesByStoreEWalletdetails text-nowrap fixed-col" data-sort="accounting" style="cursor:pointer; display:none;">Gold Card</th>
                                        <th class="colSalesByStoreEWalletdetails text-nowrap fixed-col" data-sort="accounting" style="cursor:pointer; display:none;">Shopee Pay</th>
                                        <th class="colSalesByStoreEWalletdetails text-nowrap fixed-col" data-sort="accounting" style="cursor:pointer; display:none;">Shopee E-Voucher</th>
                                        <th class="colSalesByStoreEWalletdetails text-nowrap fixed-col" data-sort="accounting" style="cursor:pointer; display:none;">Lazada</th>
                                        <th class="colSalesByStoreEWalletdetails text-nowrap fixed-col" data-sort="accounting" style="cursor:pointer; display:none;">Share Treats</th>

                                        <th class="table-active colSalesByStoreAccumulatedDeliveries text-nowrap fixed-col" data-sort="accounting" style="cursor:pointer; display:revert; min-width:200px;">Total Delivery Aggregators</th>
                                        <th class="table-active colSalesByStoreDeliveryDetails text-nowrap fixed-col" data-sort="accounting" style="cursor:pointer; display:none;">Food Panda</th>
                                        <th class="table-active colSalesByStoreDeliveryDetails text-nowrap fixed-col" data-sort="accounting" style="cursor:pointer; display:none;">Grab</th>
                                        <th class="table-active colSalesByStoreDeliveryDetails text-nowrap fixed-col" data-sort="accounting" style="cursor:pointer; display:none;">Pood Trip</th>
                                        <th class="table-active colSalesByStoreDeliveryDetails text-nowrap fixed-col" data-sort="accounting" style="cursor:pointer; display:none;">DeliRush</th>
                                        <th class="table-active colSalesByStoreDeliveryDetails text-nowrap fixed-col" data-sort="accounting" style="cursor:pointer; display:none;">Eatigo</th>
                                        <th class="table-active colSalesByStoreDeliveryDetails text-nowrap fixed-col" data-sort="accounting" style="cursor:pointer; display:none;">Booky</th>
                                        <th class="table-active colSalesByStoreDeliveryDetails text-nowrap fixed-col" data-sort="accounting" style="cursor:pointer; display:none;">Pickaroo</th>
                                        <th class="table-active colSalesByStoreDeliveryDetails text-nowrap fixed-col" data-sort="accounting" style="cursor:pointer; display:none;">Local Delivery Partners</th>

                                        <th data-sort="accounting" class="text-nowrap fixed-col" style="cursor:pointer;">Total E-Payment</th>
                                        <th data-sort="accounting" class="text-nowrap fixed-col" style="cursor:pointer;">Total Walk In</th>
                                        <th data-sort="accounting" class="text-nowrap fixed-col" style="cursor:pointer;">Total Delivery</th>
                                        <th data-sort="accounting" class="text-nowrap fixed-col" style="cursor:pointer; min-width: 200px;">Cashier</th>
                                        <th data-sort="accounting" class="text-nowrap fixed-col" style="cursor:pointer;">Latest Transaction Date</th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                                <tfoot>
                                    <tr>
                                        <th class="fixed-col excludecolspan" colspan="5" style="text-align:right">Totals:</th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                    </tr>
                                </tfoot>
                            </table>
                        </div>

                        <div id="popupDeviceStatus" data-bs-backdrop="true" class="modal fade" style="display:none;">
                            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title"></h5>
                                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                    </div>
                                    <div class="modal-body">

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="divInventoryMovement" subpage="dashboard" style="display:none;">
                        @*Inventory Movement*@
                        <div class="row">
                            <div class="col-6 col-md-auto mb-3">
                                Date: <br />
                                <button id="btnInventoryMovementDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblInventoryMovementDate"> Today </span>
                                </button>
                            </div>
                            <div class="col-6 col-md-auto mb-3">
                                Store: <br />
                                <button id="btnInventoryMovementFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblInventoryMovementFOWST"> Store </span>
                                </button>
                            </div>
                        </div>

                        <div class="table-responsive mb-3">
                            <table data-role="none" id="tblInventoryMovementReport" class="table table-light table-hover table-striped" style="width:100%;">
                                <thead>
                                    <tr class="align-top text-center" style="font-size:14px;">
                                        <th data-sort="string" style="cursor:pointer;"> Product </th>
                                        <th data-sort="string" style="cursor:pointer;"> Unit Measure </th>
                                        <th data-sort="int" style="cursor:pointer;"> Physical Beginning Inventory </th>
                                        <th data-sort="int" style="cursor:pointer;"> Delivery </th>
                                        <th data-sort="int" style="cursor:pointer;"> Receive from Branch </th>
                                        <th data-sort="int" style="cursor:pointer;"> Transfer to Branch </th>
                                        <th data-sort="int" style="cursor:pointer;"> Shortlanded </th>
                                        @*<th data-sort="int" style="cursor:pointer;"> Wastage </th>*@
                                        <th data-sort="int" style="cursor:pointer;"> Theoretical Usage </th>
                                        <th data-sort="int" style="cursor:pointer;"> Ending Balance </th>
                                        <th data-sort="int" style="cursor:pointer;"> Variance </th>
                                        <th data-sort="int" style="cursor:pointer;"> Actual Count </th>
                                        <th data-sort="int" style="cursor:pointer;"> Theoretical Ending Balance </th>
                                        <th data-sort="string" style="cursor:pointer;"> Cost </th>
                                        <th data-sort="string" style="cursor:pointer;"> Total Cost </th>
                                        <th data-sort="string" style="cursor:pointer;"> Sales N/ VAT </th>

                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>
                    </div>

                    <div id="divDeliveryReport" subpage="dashboard" style="display:none;">
                        @*Delivery Report*@
                        <div class="row">
                            <div class="col-6 col-md-4 col-lg-auto mb-3">
                                Date: <br />
                                <button id="btnDeliveryReport" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblDeliveryReport"> Today </span>
                                </button>
                            </div>
                            <div class="col-6 col-md-4 col-lg-auto mb-3">
                                Group: <br />
                                <button id="btnDeliveryReportGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    Select Group
                                </button>
                            </div>
                            <div class="col-6 col-md-4 col-lg-auto mb-3">
                                Store: <br />
                                <button id="btnDeliveryReportFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblDeliveryReportFOWST"> Select Store </span>
                                </button>
                            </div>
                            <div class="col-12 col-md-4 col-lg-auto mb-3">
                                Transaction Type:
                                <select data-role="none" id="selDeliveryReport" class="form-select">
                                    <option value="StockReceivingFromFOWST">Stock Receiving from other Store</option>
                                    <option value="StockReceivingFromSupplier">Stock Receiving from other Supplier</option>
                                    <option value="StockTransferFromFOWST">Stock Transfers to other Store</option>
                                    <option value="Wastage">Spoilage/Wastage</option>
                                    <option value="ForInventoryStore">Inventory Store</option>
                                    @*<option value="ForInventoryStore">Sampling/Testing</option>*@
                                    <option value="Consumables">Consumables</option>
                                    <option value="Overlanded">Overlanded</option>
                                    <option value="Shortlanded">Shortlanded</option>
                                    <option value="ProductDefect">Product Defect</option>
                                </select>
                            </div>
                        </div>

                        <table data-role="none" id="tblDeliveryReport" class="table table-light table-hover table-striped" style="width:100%;">
                            <thead>
                                <tr class="align-top text-center" style="font-size:14px;">
                                    <th data-sort="string" style="cursor:pointer;" id="headerReceivedIn" class="fixed-col"> Received In </th>
                                    <th data-sort="string" style="cursor: pointer;" id="headerReceivedFrom" class="fixed-col"> Received From </th>
                                    <th data-sort="string" style="cursor:pointer;" class="fixed-col"> Transported By </th>
                                    <th data-sort="string" style="cursor:pointer;" class="fixed-col"> Transaction Date </th>
                                    <th data-sort="string" style="cursor:pointer;" class="fixed-col"> Transaction # </th>
                                    <th data-sort="string" style="cursor:pointer;" class="fixed-col"> Category </th>
                                    <th data-sort="string" style="cursor:pointer;" class="fixed-col"> Product </th>
                                    <th data-sort="string" style="cursor:pointer;" class="fixed-col"> Unit Measure </th>
                                    <th data-sort="int" style="cursor:pointer;" class="fixed-col"> Quantity </th>
                                    <th data-sort="int" style="cursor:pointer;" class="fixed-col"> Unit Price </th>
                                    <th data-sort="int" style="cursor:pointer;" class="fixed-col"> Crew Name </th>
                                </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>

                    <div id="divHourlySalesReport" subpage="dashboard" style="display:none;">
                        @*Hourly Sales Report*@
                        <div class="row">
                            <div class="col-6 col-sm-4 col-md-4 col-lg-auto mb-3">
                                Date: <br />
                                <button id="btnHourlySalesReport" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblHourlySalesReport"> Today </span>
                                </button>
                            </div>

                            <div class="col-6 col-sm-4 col-md-4 col-lg-auto mb-3">
                                Store: <br />
                                <button id="btnHourlySalesReportFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblHourlySalesReportFOWST"> Store </span>
                                </button>
                            </div>
                            <div class="col-6 col-sm-4 col-md-4 col-lg-auto mb-3">
                                Group: <br />
                                <button id="btnHourlySalesReportGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblHourlySalesReportGroupFOWST"> Group Store </span>
                                </button>
                            </div>

                            <div class="col-6 col-sm-4 col-md-4 col-lg-auto mb-3">
                                Store Type:
                                <select data-role="none" id="selHourlySalesReportStoreType" class="form-select selstoretypeHourlySales"></select>
                            </div>
                            <div class="col-6 col-sm-4 col-md-4 col-lg-auto mb-3">
                                Store Location Type:
                                <select data-role="none" id="selHourlySalesReportStoreLocationType" class="form-select selstorelocationtypeHourlySales"></select>
                            </div>

                        </div>

                        <div class="table-responsive mb-3">
                            <table data-role="none" id="tblHourlySalesReport" class="table table-light table-hover table-striped" style="width:100%;">
                                <thead>
                                    <tr class="align-top text-center" style="font-size:14px;">
                                        <th data-sort="date" style="cursor:pointer;text-align:left"> Transaction Hour </th>
                                        <th data-sort="int" style="cursor:pointer;text-align:left"> Product Count </th>
                                        <th data-sort="string" style="cursor:pointer;text-align:left"> Product Count(%) </th>
                                        <th data-sort="string" style="cursor:pointer;text-align:left"> Transaction Count </th>
                                        <th data-sort="string" style="cursor:pointer;text-align:left"> Transaction Count(%) </th>
                                        <th data-sort="accounting" style="cursor:pointer;text-align:left"> Net Sales </th>
                                        <th data-sort="accounting" style="cursor:pointer;text-align:left"> Net Sales(%) </th>

                                        @*<th data-sort="string" style="cursor:pointer;"> Total Walk-In </th>
                            <th data-sort="string" style="cursor:pointer;"> Total Delivery </th>*@
                                    </tr>
                                </thead>
                                <tbody></tbody>
                                <tfoot>
                                    <tr>
                                        <th style="text-align:right">Totals:</th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                    </tr>
                                </tfoot>
                            </table>
                        </div>
                    </div>

                    <div id="divMonthlySalesReport" subpage="dashboard" style="display:none;">
                        @*Monthly Sales Report*@
                        <div class="row">
                            <div class="col-6 col-sm-4 col-md-4 col-lg-auto mb-3">
                                Date: <br />
                                <button id="btnMonthlySalesReport" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblMonthlySalesReport"> Today </span>
                                </button>
                            </div>

                            @*<div class="col-6 col-sm-4 col-md-4 col-lg-auto mb-3">
                    Store: <br />
                    <button id="btnMonthlySalesReportFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                        <span class="lblMonthlySalesReportFOWST"> Store </span>
                    </button>
                </div>*@

                            <div class="col-6 col-sm-4 col-md-4 col-lg-auto mb-3">
                                Group: <br />
                                <button id="btnMonthlySalesReportGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblMonthlySalesReportGroupFOWST"> Group Store </span>
                                </button>
                            </div>

                            <div class="col-6 col-sm-4 col-md-4 col-lg-auto mb-3">
                                Store Type:
                                <select data-role="none" id="selMonthlySalesReportStoreType" class="form-select selstoretype"></select>
                            </div>
                            <div class="col-6 col-sm-4 col-md-4 col-lg-auto mb-3">
                                Store Location Type:
                                <select data-role="none" id="selMonthlySalesReportStoreLocationType" class="form-select selstorelocationtype"></select>
                            </div>
                        </div>

                        <div class="table-responsive mb-3">
                            <table data-role="none" id="tblMonthlySalesReport" class="table table-light table-hover table-striped" style="width:100%;">
                                <thead class="align-top text-center" style="font-size:14px;">
                                    <tr>
                                        <th class="table-active" colspan="5">Store Information</th>
                                        <th colspan="8">Sales Information</th>
                                        <th class="table-active thMonthlySRPaymentType" colspan="1">Payment Type <button data-role="none" class="btn" id="btnToggleMonthlySRPaymentTypeDetails" data-toggle="tooltip" title="Show details"><span class="bi-chevron-right"></span></button></th>
                                        <th class="thMonthlySRDeliveries" colspan="1">Deliveries <button data-role="none" class="btn" id="btnToggleMonthlySRDeliveriesDetails" data-toggle="tooltip" title="Show details"><span class="bi-chevron-right"></span></button></th>
                                        <th class="table-active" colspan="4">Order Method</th>
                                    </tr>
                                    <tr>
                                        <th class="table-active" data-sort="date" style="cursor:pointer;"> Store Code </th>
                                        <th class="table-active" data-sort="date" style="cursor:pointer;"> Store Name </th>
                                        <th class="table-active" data-sort="int" style="cursor:pointer;"> Group </th>
                                        <th class="table-active" data-sort="int" style="cursor:pointer;"> Store Type </th>
                                        <th class="table-active" data-sort="int" style="cursor:pointer;"> Location Type </th>

                                        <th data-sort="accounting" style="cursor:pointer;"> Transaction Count </th>
                                        <th data-sort="accounting" style="cursor:pointer;"> Average Check </th>
                                        <th data-sort="accounting" style="cursor:pointer;"> Target Sales </th>
                                        <th data-sort="accounting" style="cursor:pointer;"> Gross Sales </th>
                                        <th data-sort="accounting" style="cursor:pointer;"> Sales Return/Refund </th>
                                        <th data-sort="accounting" style="cursor:pointer;"> Total Discount </th>
                                        <th data-sort="accounting" style="cursor:pointer;"> Net Sales </th>
                                        <th data-sort="accounting" style="cursor:pointer;"> Achievement </th>

                                        <th class="table-active colMonthlySRAccumulatedPayment" data-sort="accounting" style="cursor:pointer; display:revert; min-width:200px;">Accumulated Payment</th>
                                        <th class="table-active colMonthlySRPaymentDetails" data-sort="accounting" style="cursor:pointer; display:none;">Cash</th>
                                        <th class="table-active colMonthlySRPaymentDetails" data-sort="accounting" style="cursor:pointer; display:none;">Credit Card</th>
                                        <th class="table-active colMonthlySRPaymentDetails" data-sort="accounting" style="cursor:pointer; display:none;">Debit Card</th>
                                        <th class="table-active colMonthlySRPaymentDetails" data-sort="accounting" style="cursor:pointer; display:none;">Cheque</th>
                                        <th class="table-active colMonthlySRPaymentDetails" data-sort="accounting" style="cursor:pointer; display:none;">Voucher</th>
                                        <th class="table-active colMonthlySRPaymentDetails" data-sort="accounting" style="cursor:pointer; display:none;">G Cash</th>
                                        <th class="table-active colMonthlySRPaymentDetails" data-sort="accounting" style="cursor:pointer; display:none;">Pay Maya</th>
                                        <th class="table-active colMonthlySRPaymentDetails" data-sort="accounting" style="cursor:pointer; display:none;">SMAC</th>
                                        @*<th class="table-active colMonthlySRPaymentDetails" data-sort="accounting" style="cursor:pointer; display:none;">V-Plus</th>*@
                                        <th class="table-active colMonthlySRPaymentDetails" data-sort="accounting" style="cursor:pointer; display:none;">Gold Card</th>
                                        <th class="table-active colMonthlySRPaymentDetails" data-sort="accounting" style="cursor:pointer; display:none;">Shopee Pay</th>
                                        <th class="table-active colMonthlySRPaymentDetails" data-sort="accounting" style="cursor:pointer; display:none;">Shopee E-Voucher</th>
                                        <th class="table-active colMonthlySRPaymentDetails" data-sort="accounting" style="cursor:pointer; display:none;">Lazada</th>
                                        <th class="table-active colMonthlySRPaymentDetails" data-sort="accounting" style="cursor:pointer; display:none;">Share Treats</th>

                                        <th class="colMonthlySRAccumulatedDeliveries" data-sort="accounting" style="cursor:pointer; display:revert; min-width:200px;">Accumulated Deliveries</th>
                                        <th class="colMonthlySRDeliveryDetails" data-sort="accounting" style="cursor:pointer; display:none;">Food Panda</th>
                                        <th class="colMonthlySRDeliveryDetails" data-sort="accounting" style="cursor:pointer; display:none;">Grab</th>
                                        <th class="colMonthlySRDeliveryDetails" data-sort="accounting" style="cursor:pointer; display:none;">Pood Trip</th>
                                        <th class="colMonthlySRDeliveryDetails" data-sort="accounting" style="cursor:pointer; display:none;">DeliRush</th>
                                        <th class="colMonthlySRDeliveryDetails" data-sort="accounting" style="cursor:pointer; display:none;">Eatigo</th>
                                        <th class="colMonthlySRDeliveryDetails" data-sort="accounting" style="cursor:pointer; display:none;">Booky</th>
                                        <th class="colMonthlySRDeliveryDetails" data-sort="accounting" style="cursor:pointer; display:none;">Pickaroo</th>
                                        <th class="colMonthlySRDeliveryDetails" data-sort="accounting" style="cursor:pointer; display:none;">Local Delivery Partners</th>

                                        <th data-sort="accounting" style="cursor:pointer;">Total E-Payment</th>
                                        <th data-sort="accounting" style="cursor:pointer;">Total Walk In</th>
                                        <th data-sort="accounting" style="cursor:pointer;">Total Delivery</th>
                                        <th data-sort="accounting" style="cursor:pointer;">Latest Transaction Date</th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                                <tfoot>
                                    <tr>
                                        <th colspan="4" style="text-align:right">Totals:</th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                    </tr>
                                </tfoot>
                            </table>
                        </div>
                    </div>

                    <div id="divSpecialPromoReport" subpage="dashboard" style="display:none;">
                        @*Special Discount Report*@
                        <div class="row">
                            <div class="col-auto mb-3">
                                Date: <br />
                                <button id="btnSpecialPromoReport" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblSpecialPromoReport"> Today </span>
                                </button>
                            </div>
                            <div class="col-auto mb-3">
                                Store:
                                <br />
                                <button id="btnSpecialPromoReportFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblSpecialPromoReportFOWST"> Store </span>
                                </button>
                            </div>
                            <div class="col-auto mb-3">
                                Group:
                                <br />
                                <button id="btnSpecialPromoGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblSpecialPromoGroupFOWST"> Group Store </span>
                                </button>

                            </div>
                            @*<div class="col-auto mb-3">
                Filter by Discount Type: <br />
                <select data-role="none" id="selSpecialPromoType" class="form-select fixed">
                    <option value="">Please select Date and FOWST first</option>
                </select>
            </div>*@
                        </div>

                        <div class="table-responsive mb-3">
                            <table data-role="none" id="tblSpecialPromoReport" class="table table-light table-hover table-striped" style="width:100%;">
                                <thead>
                                    <tr class="align-top text-left" style="font-size:14px;">
                                        <th class="table-active" data-sort="date" style="cursor:pointer;"> Store Name </th>
                                        <th class="table-active" data-sort="date" style="cursor:pointer;"> Group  </th>
                                        <th class="table-active" data-sort="date" style="cursor:pointer;"> Transaction Date </th>
                                        <th class="table-active" data-sort="accounting" style="cursor:pointer;"> Promo Name </th>
                                        @*<th class="table-active" data-sort="int" style="cursor:pointer;"> Product </th>
                        <th class="table-active" data-sort="string" style="cursor:pointer;"> Unit Measure </th>*@
                                        <th class="table-active" data-sort="string" style="cursor:pointer;"> Quantity </th>

                                        @*<th class="table-active" data-sort="date" style="cursor:pointer;"> Store Name </th>*@

                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>

                        <div id="popupSpecialPromo" data-bs-backdrop="true" class="modal fade" style="display:none;">
                            <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title">Detailed Report</h5>
                                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                    </div>
                                    <div class="modal-body">
                                        <div class="table-responsive">
                                            <table id="tblSpecialPromoDetails" data-role="none" class="table table-light table-hover table-striped">
                                                <thead>
                                                    <tr class="text-center">
                                                        <th style="width:50px;">#</th>
                                                        <th>Invoice No.</th>
                                                        <th>Date</th>
                                                        <th>Time</th>
                                                        <th>Store</th>
                                                        <th>Employee Number Used</th>
                                                        <th>Purchased Item</th>
                                                        <th>Quantity</th>
                                                        <th>Amount</th>
                                                        <th>Mode of Payment</th>
                                                    </tr>
                                                </thead>
                                                <tbody></tbody>
                                            </table>
                                        </div>                                        
                                    </div>
                                    <div class="modal-footer">
                                        <div class="row">
                                            <div class="col text-right">
                                                <button data-role="none" data-dismiss="modal" class="btn btn-secondary">Close</button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="divSpecialDiscountReport" subpage="dashboard" style="display:none;">
                        @*Special Discount Report*@
                        <div class="row">
                            <div class="col-6 col-sm-4 col-md-4 col-lg-auto mb-3">
                                Date: <br />
                                <button id="btnSpecialDiscountReport" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblSpecialDiscountReport"> Today </span>
                                </button>
                            </div>
                            <div class="col-6 col-sm-4 col-md-4 col-lg-auto mb-3">
                                Store:
                                <br />
                                <button id="btnSpecialDiscountReportFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    <span class="lblSpecialDiscountReportFOWST"> Store </span>
                                </button>
                            </div>
                            <div class="col-6 col-sm-4 col-md-4 col-lg-auto mb-3">
                                Group:
                                <br />
                                <button id="btnSpecialDiscountGroupFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                    Select Group
                                </button>
                            </div>
                            <div class="col-6 col-sm-4 col-md-4 col-lg-auto mb-3">
                                Filter by Discount Type: <br />
                                <select data-role="none" id="selSpecialDiscountType" class="form-select">
                                    <option value="">Please select Date and FOWST first</option>
                                </select>
                            </div>
                        </div>

                        <div class="table-responsive mb-3">
                            <table data-role="none" id="tblSpecialDiscountReport" class="table table-light table-hover table-striped" style="width:100%;">
                                <thead>
                                    <tr class="align-top text-center" style="font-size:14px;">
                                        <th data-sort="date" style="cursor:pointer;"> Store Code </th>
                                        <th data-sort="date" style="cursor:pointer;"> Store Name </th>
                                        <th data-sort="date" style="cursor:pointer;"> Group Name </th>
                                        <th data-sort="date" style="cursor:pointer;"> Store Type </th>
                                        <th data-sort="date" style="cursor:pointer;"> Store Location Type </th>
                                        <th data-sort="date" style="cursor:pointer;"> Transaction Date </th>
                                        <th data-sort="int" style="cursor:pointer;"> Invoice Number </th>
                                        <th data-sort="string" style="cursor:pointer;"> Product </th>
                                        <th data-sort="accounting" style="cursor:pointer;"> Discount Type </th>
                                        <th data-sort="accounting" style="cursor:pointer;"> Gross Amount </th>
                                        <th data-sort="accounting" style="cursor:pointer;"> Discount Amount </th>
                                        <th data-sort="accounting" style="cursor:pointer;"> Net Amount </th>
                                        <th data-sort="accounting" style="cursor:pointer;"> Customer Detail </th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>
                    </div>

                    <div id="divDeviceMonitoring" subpage="dashboard" style="display:none;">

                        <div class="row g-3 mb-3">
                            <div class="col-auto">
                                <div class="row gy-2 gx-3 align-items-center">
                                    <div class="col-auto">
                                        Report Type:
                                    </div>
                                    <div class="col">
                                        <select data-role="none" id="selDeviceMonitoringReport" class="form-select">
                                            <option>Please select</option>
                                            @*<option value="selDeviceAvailability">Device Availability Report</option>*@
                                            <option value="selDeviceFlushingHistory">Device Flushing Status Report</option>
                                            <option value="selDeviceFlushingQueueMonitoring">Flushing Queue History Report</option>
                                            <option value="selDeviceUtilizationDailyReport">Device Utilization Daily Report</option>
                                            <option value="selDeviceUtilizationHourlyReport">Device Utilization Hourly Report</option>
                                            <option value="selDeviceFlushingQueue">Current Flushing Queue</option>
                                        </select>
                                    </div>
                                </div>
                            </div>
                            <div class="col-auto" id="dvGroupDeviceMonitoringGroupFOWST" style="display:none;">
                                <div class="row gy-2 gx-3 align-items-center">
                                    <div class="col-auto">
                                        Group:
                                    </div>
                                    <div class="col">
                                        <button id="btnDeviceMonitoringGroupFOWST" data-role="none" class="btn btn-secondary">
                                            <span class="lblDeviceMonitoringGroupFOWST"> Group Store </span>
                                        </button>
                                    </div>
                                </div>
                            </div>
                            <div class="col-auto" id="dvDeviceMonitoringDate" style="display:none;">
                                <div class="row gy-2 gx-3 align-items-center">
                                    <div class="col-auto">
                                        Date:
                                    </div>
                                    <div class="col">
                                        <button id="btnDeviceMonitoringDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                                            <span class="lblDeviceMonitoringDate"> Today </span>
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div id="dvDeviceFlushingMonitoring" style="display:none">
                            <div class="table-responsive mb-3">
                                <table data-role="none" id="tblDeviceMonitoring" class="table table-light table-hover table-striped" style="width:100%;">
                                    <thead>
                                        <tr class="align-top text-center" style="font-size:14px;">
                                            <th data-sort="int" style="cursor:pointer;">#</th>
                                            <th data-sort="string" style="cursor:pointer;">FOWST</th>
                                            <th data-sort="string" style="cursor:pointer;">Group</th>
                                            <th data-sort="string" style="cursor:pointer;">Device Name</th>
                                            <th data-sort="string" style="cursor:pointer;">Device Serial Number</th>
                                            <th data-sort="date" style="cursor:pointer;">Last Data Flush Received In Server</th>
                                            <th data-sort="date" style="cursor:pointer;">No. of Days Since Last Data Flush Received In Server</th>
                                            <th data-sort="date" style="cursor:pointer;">Last Date of Transaction Flushed From Device</th>
                                            <th data-sort="date" style="cursor:pointer;">No. of Days Since Last Date of Transaction Flushed From Device</th>
                                            <th data-sort="date" style="cursor:pointer;"> Last Data Sync </th>
                                            <th data-sort="date" style="cursor:pointer;">No. of Days Since Last Data Sync</th>
                                            <th data-sort="date" style="cursor:pointer;"> Installed Database Creation Date </th>
                                            <th data-sort="int" style="cursor:pointer;"> APK Version </th>
                                            <th data-sort="date" style="cursor:pointer;"> APK Build Date </th>
                                        </tr>
                                    </thead>
                                    <tbody></tbody>
                                </table>
                            </div>
                        </div>

                        <div id="dvDeviceAvailability" style="display:none;">
                            <div class="table-responsive mb-3">
                                <table data-role="none" id="tblDeviceAvailability" class="table table-light table-hover table-striped" style="width:100%;">
                                    <thead>
                                        <tr class="align-top text-center" style="font-size:14px;">
                                            <th data-sort="int" style="cursor:pointer;">#</th>
                                            <th data-sort="string" style="cursor:pointer;">Device Name</th>
                                            <th data-sort="string" style="cursor:pointer;">Available?</th>
                                            <th data-sort="string" style="cursor:pointer;">Reason for Inability</th>
                                        </tr>
                                    </thead>
                                    <tbody></tbody>
                                </table>
                            </div>
                        </div>

                        <div id="panDeviceAvailabilitySummary" data-bs-backdrop="true" class="modal fade" style="display:none;">
                            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title">Device Availability Summary<span class="lblPopUpHeaderLibraryName"></span></h5>
                                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                    </div>
                                    <div class="modal-body" id="panSetBegBalanceInventoryLedgercontent-bs">
                                        <div style="margin-bottom:1rem;">
                                            <br />
                                            <table data-role="none" id="tblDeviceAvailabilitySummary" class="table table-light table-hover table-striped" style="width:100%;">
                                                <thead>
                                                    <tr class="align-top text-left" style="font-size:14px;">
                                                        <th data-sort="int" style="cursor:pointer;">Total Number Device</th>
                                                        <th data-sort="int" style="cursor:pointer;">Total Active Device</th>
                                                        <th data-sort="int" style="cursor:pointer;">Total InActive Device</th>
                                                        <th data-sort="int" style="cursor:pointer;">%</th>
                                                    </tr>
                                                </thead>
                                                <tbody></tbody>
                                            </table>
                                        </div>
                                    </div>
                                    <div class="modal-footer">
                                        <div class="row">
                                            <div class="col">
                                                <button data-role="none" class="btn btn-secondary btn-block" data-dismiss="modal">Close</button>
                                            </div>
                                            @*<div class="col">
                                    <button data-role="none" class="btn btn-success btn-block btnSetReasonforUnavailability">Save</button>
                                </div>*@
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div id="dvDeviceFlushingQueueMonitoringHistory" style="display:none">
                            <div class="table-responsive mb-3">
                                <table data-role="none" id="tblDeviceFlushingQueueHistory" class="table table-light table-hover table-striped" style="width:100%;">
                                    <thead>
                                        <tr class="align-top text-center" style="font-size:14px;">
                                            <th data-sort="int" style="cursor:pointer;">#</th>
                                            <th data-sort="string" style="cursor:pointer;">Date and Time</th>
                                            <th data-sort="string" style="cursor:pointer;">No of queued zip files for flushing</th>
                                        </tr>
                                    </thead>
                                    <tbody></tbody>
                                </table>
                            </div>
                        </div>

                        <div id="dvDeviceFlushingQueue" style="display:none">
                            <div class="row mb-3 gy-2 gx-3 align-items-center">
                                <div class="col-sm">
                                    <div class="input-group">
                                        <input data-role="none" class="form-control" placeholder="Search here" id="txtFlushingSearch" />
                                        <button data-role="none" class="btn btn-secondary" id="btnRefreshDeviceFlushingQueue">Refresh</button>
                                    </div>
                                </div>
                            </div>

                            <table data-role="none" id="tblDeviceFlushingQueue" class="table table-light table-hover table-striped" style="width:100%;">
                                <thead>
                                    <tr class="align-top text-center" style="font-size:14px;">
                                        <th class="fixed-col text-nowrap" data-sort="int" style="cursor:pointer;">#</th>
                                        <th class="fixed-col text-nowrap" data-sort="int" style="cursor:pointer;">Device Name</th>
                                        <th class="fixed-col text-nowrap" data-sort="string" style="cursor:pointer;">File Date Received in Mobiz Server</th>
                                        <th class="fixed-col text-nowrap" data-sort="string" style="cursor:pointer;">File Process Start</th>
                                        @*<th class="fixed-col" data-sort="string" style="cursor:pointer;">File Process End</th>
                                        <th class="fixed-col" data-sort="string" style="cursor:pointer;">File Process Duration(In Seconds)</th>*@
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>

                        <div id="dvDeviceUtilization" style="display:none">
                            <div class="row row-cols-1 row-cols-sm-2 row-cols-md-4 g-3 mb-3">
                                <div class="col">
                                    Date: <br />
                                    <button id="btnDeviceUtilizationDate" data-role="none" class="btn btn-secondary btn-block">
                                        <span class="lblDeviceUtilizationDate"> Today </span>
                                    </button>
                                </div>

                                <div class="col">
                                    Store: <br />
                                    <button id="btnDeviceUtilizationFOWST" data-role="none" class="btn btn-secondary btn-block">
                                        <span class="lblDeviceUtilizationFOWST"> Store </span>
                                    </button>
                                </div>

                                <div class="col">
                                    Group: <br />
                                    <button id="btnDeviceUtilizationGroupFOWST" data-role="none" class="btn btn-secondary btn-block">
                                        <span class="lblDeviceUtilizationGroupFOWST"> Group Store </span>
                                    </button>
                                </div>

                                <div class="col">
                                    Store Status: <br />
                                    <select data-role="none" class="form-select" id="selDeviceUtilizationStoreStatus">
                                        <option value="">All</option>
                                        <option value="Store and device are operational." selected>Store and device are operational.</option>
                                        <option value="Store is operational. No device is deployed yet.">Store is operational. No device is deployed yet.</option>
                                        <option value="Store is operational. Device is on hold.">Store is operational. Device is on hold.</option>
                                        <option value="Store is operational. Waiting for device to arrive.">Store is operational. Waiting for device to arrive.</option>
                                        <option value="Store is operational. No device available. Stolen.">Store is operational. No device available. Stolen.</option>
                                        <option value="Store is operational. No device available. Broken.">Store is operational. No device available. Broken.</option>
                                        <option value="Store is permanently closed.">Store is permanently closed.</option>
                                        <option value="Store is temporarily closed for holiday.">Store is temporarily closed for holiday.</option>
                                        <option value="Store is temporarily closed for company activity.">Store is temporarily closed for company activity.</option>
                                        <option value="Others">Others</option>
                                    </select>
                                </div>
                            </div>

                            <div class="mb-3 divDevUtilSummary" style="display:none;">
                                <h4>Summary</h4>
                                <div class="table-responsive">
                                    <table id="tblDeviceUtilUsageSummary" data-role="none" class="table table-light table-hover table-striped" style="width:100%;">
                                        <thead>
                                            <tr class="text-center">
                                                <th>#</th>
                                                <th>Region</th>
                                                <th>No. of Stores</th>
                                                <th id="thDevUtilUsage"></th>
                                                <th>Utilization</th>
                                            </tr>
                                        </thead>
                                        <tbody></tbody>
                                    </table>
                                </div>
                            </div>

                            <table id="tblDeviceUtilUsageDetails" data-role="none" class="table table-light table-hover table-striped" style="width:100%;">
                                <thead>
                                    <tr class="text-center"></tr>
                                </thead>
                                <tbody></tbody>
                            </table>

                            <fieldset class="border p-2 mb-3 d-none" style="display:none;">
                                <legend class="w-auto float-none text-secondary" style="font-size:14px;">All Active Stores Status Summary</legend>
                                <div class="row row-cols-1 row-cols-sm-1 row-cols-md-2 g-2 mb-3">
                                    <div class="col">
                                        <div class="table-scroll">
                                            <table data-role="none" class="table table-light table-striped table-hover text-center">
                                                <thead>
                                                    <tr class="text-center align-top" style="font-size:14px;">
                                                        <th>Status</th>
                                                        <th>Frequency Count</th>
                                                        <th>% Distribution</th>
                                                    </tr>
                                                </thead>
                                                <tbody id="tbodyDeviceUtilStoreSummary"></tbody>
                                                <tfoot>
                                                    <tr>
                                                        <td>Total</td>
                                                        <td id="tfootDeviceUtilTotalStore"></td>
                                                        <td id="tfootDeviceUtilTotalStorePercentage"></td>
                                                    </tr>
                                                </tfoot>
                                            </table>
                                        </div>
                                    </div>
                                    <div class="col align-self-center">
                                        <div style="height:300px">
                                            <canvas id="chartDeviceUtilStoreStatus"></canvas>
                                        </div>
                                    </div>
                                </div>
                                <div class="row row-cols-1 row-cols-sm-1 row-cols-md-2 g-2 mt-3">
                                    <div class="col">
                                        <div class="text-center">
                                            <i>Click table to view more details</i>
                                        </div>
                                        <div class="table-scroll">
                                            <table data-role="none" class="table table-light table-striped table-hover text-center">
                                                <thead>
                                                    <tr class="text-center align-top" style="font-size:14px;">
                                                        <th>Utilization</th>
                                                        <th>No. of Stores</th>
                                                        <th>% Usage</th>
                                                    </tr>
                                                </thead>
                                                <tbody id="tbodyDeviceUtilUsageSummary"></tbody>
                                                <tfoot>
                                                    <tr>
                                                        <td>Total Store Count</td>
                                                        <td id="tfootDeviceUtilUsageTotalStore"></td>
                                                        <td id="tfootDeviceUtilUsageTotalStorePercentage"></td>
                                                    </tr>
                                                </tfoot>
                                            </table>
                                        </div>
                                    </div>
                                    <div class="col align-self-center">
                                        <div style="height:300px">
                                            <canvas id="chartDeviceUtilUsage"></canvas>
                                        </div>
                                    </div>
                                </div>
                            </fieldset>

                            <div class="table-responsive mb-3 d-none">
                                <table data-role="none" id="tblDeviceUtilization" class="table table-light table-hover table-striped" style="width:100%;">
                                    <thead>
                                        <tr class="align-top" style="font-size:14px;">
                                            <th data-sort="int" style="cursor:pointer;">#</th>
                                            <th data-sort="int" style="cursor:pointer;">Store Code</th>
                                            <th data-sort="int" style="cursor:pointer;">Store</th>
                                            <th data-sort="int" style="cursor:pointer;">Status</th>
                                            <th data-sort="string" style="cursor:pointer;">Company Or Franchise Owned </th>
                                            <th data-sort="string" style="cursor:pointer;">Group/Region</th>
                                            <th data-sort="string" style="cursor:pointer;">Usage in a Days</th>
                                            <th data-sort="string" style="cursor:pointer;">Last Sales Data Imported To Server</th>
                                        </tr>
                                    </thead>
                                    <tbody></tbody>
                                </table>
                            </div>
                        </div>

                        <div id="dvDeviceUtilizationByHour" style="display:none">
                            <div class="row row-cols-1 row-cols-sm-2 row-cols-md-4 g-3 mb-3">
                                <div class="col">
                                    Date: <br />
                                    <button id="btnDeviceUtilizationDateByHourDate" data-role="none" class="btn btn-secondary btn-block">
                                        <span class="lblDeviceUtilizationByHourDate"> Today </span>
                                    </button>
                                </div>

                                <div class="col">
                                    Store: <br />
                                    <button id="btnDeviceUtilizationFOWSTByHourFOWST" data-role="none" class="btn btn-secondary btn-block">
                                        <span class="lblDeviceUtilizationByHourFOWST"> Store </span>
                                    </button>
                                </div>

                                <div class="col">
                                    Group: <br />
                                    <button id="btnDeviceUtilizationByHourGroupFOWST" data-role="none" class="btn btn-secondary btn-block">
                                        <span class="lblDeviceUtilizationByHourGroupFOWST"> Group Store </span>
                                    </button>
                                </div>

                            </div>

                            <div class="table-responsive mb-3">
                                <table data-role="none" id="tblDeviceUtilizationByHour" class="table table-light table-hover table-striped" style="width:100%;">
                                </table>
                            </div>
                        </div>

                    </div>
                </div>
            </div>            
        </td>
    </tr>
</table>

<div id="modal_invoicedetails" data-bs-backdrop="true" class="modal fade" style="display:none;">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Invoice Details</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div id="lblILTRXNo" class="w-100 font-weight-bold text-center"></div>
                <div id="lblILSINo" class="w-100 font-weight-bold text-center"></div>

                <div id="lblILSIDate" class="w-100 font-weight-bold text-center"></div>

                <div class="row mt-3">
                    <div class="col-auto">
                        Customer Name:
                    </div>
                    <div id="lblILCustomerName" class="col font-weight-bold">

                    </div>
                </div>

                <div class="row">
                    <div class="col-auto">
                        Customer ID No.:
                    </div>
                    <div id="lblILCustomerIDNo" class="col font-weight-bold">

                    </div>
                </div>

                <div id="listInvItems" class="mt-3 mb-5"></div>
                <hr />
                <div class="row">
                    <div class="col-auto">
                        Gross Sales:
                    </div>
                    <div id="lblILGrossSales" class="col text-right font-weight-bold">

                    </div>
                </div>
                <div id="divForGovDisc" class="mt-3 mb-3">
                    <div class="row divForSCPWD">
                        <div class="col-auto">
                            Gross to <span class="lblILDiscType"></span>:
                        </div>
                        <div id="lblILGrossToDiscountType" class="col text-right">

                        </div>
                    </div>
                    <div class="row divForNAAC divForSP">
                        <div class="col-auto">
                            Less 12% VAT:
                        </div>
                        <div id="lblILLessVAT" class="col text-right">

                        </div>
                    </div>
                    <div class="row divForNAAC divForSP divForSCPWD">
                        <div class="col-auto">
                            VAT-Ex Sales:
                        </div>
                        <div id="lblILVATEx" class="col text-right">

                        </div>
                    </div>
                    <div class="row">
                        <div class="col-auto">
                            <span class="lblILDiscType"></span> Discount Rate:
                        </div>
                        <div id="lblILDiscountRate" class="col text-right">

                        </div>
                    </div>
                    <div class="row">
                        <div class="col-auto">
                            <span class="lblILDiscType"></span> Discount Amount:
                        </div>
                        <div id="lblILDiscountAmount" class="col text-right">

                        </div>
                    </div>
                    <div class="row">
                        <div class="col-auto">
                            Net Amount:
                        </div>
                        <div id="lblILNetAmount" class="col text-right">

                        </div>
                    </div>
                    <div class="row divForSCPWD divSCPWDDiscEx">
                        <div class="col-auto">
                            <span class="lblILDiscType"></span> Discount Exempt:
                        </div>
                        <div id="lblILDiscountEx" class="col text-right">

                        </div>
                    </div>
                    <div class="row divForNAAC">
                        <div class="col-auto">
                            Add VAT:
                        </div>
                        <div id="lblILAddVAT" class="col text-right">

                        </div>
                    </div>
                    <div class="row divForSCPWD divGrossRegCus">
                        <div class="col-auto">
                            Gross to Regular Customers:
                        </div>
                        <div id="lblILGrossRegCus" class="col text-right">

                        </div>
                    </div>
                </div>
                <div id="divForOtherDisc" class="mt-3 mb-3">
                    <div class="row">
                        <div class="col-auto">
                            <span id="lblILOtherDiscType"></span>:
                        </div>
                        <div id="lblILOtherDiscPercentage" class="col text-right">

                        </div>
                    </div>
                    <div class="row">
                        <div class="col-auto">
                            Discount Amount:
                        </div>
                        <div id="lblILOtherDiscAmount" class="col text-right">

                        </div>
                    </div>
                </div>
                <ul class="list-unstyled" id="listILCharges"></ul>
                <div class="row">
                    <div class="col-auto">
                        Amount Due:
                    </div>
                    <div id="lblILAmountDue" class="col text-right font-weight-bold">

                    </div>
                </div>
                <div id="listInvPayments"></div>
                <div class="row">
                    <div class="col-auto">
                        Change:
                    </div>
                    <div id="lblILChange" class="col text-right">

                    </div>
                </div>

                <div id="divRefNo" style="display:none">
                    <hr />
                    <div class="row">
                        <div class="col-auto">
                            Reference No.:
                        </div>
                        <div id="lblILRefNo" class="col text-right">

                        </div>
                    </div>
                </div>

                <hr />

                <div class="row">
                    <div class="col-auto">
                        VAT Sales:
                    </div>
                    <div id="lblILVATSales" class="col text-right">

                    </div>
                </div>
                <div class="row">
                    <div class="col-auto">
                        VAT Amount:
                    </div>
                    <div id="lblILVATAmount" class="col text-right">

                    </div>
                </div>
                <div class="row">
                    <div class="col-auto">
                        Non-VAT:
                    </div>
                    <div id="lblILNonVAT" class="col text-right">

                    </div>
                </div>
                <div class="row">
                    <div class="col-auto">
                        VAT-Ex Sales:
                    </div>
                    <div id="lblILVATExSales" class="col text-right">

                    </div>
                </div>
                <div class="row">
                    <div class="col-auto">
                        VAT-Ex Tax:
                    </div>
                    <div id="lblILVATExTax" class="col text-right">

                    </div>
                </div>
                <div class="row">
                    <div class="col-auto">
                        Zero-Rated VAT Sales:
                    </div>
                    <div id="lblILZeroRatedVAT" class="col text-right">

                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popupSummarizedSalesByStoreDetails" class="modal fade" style="display:none;">
    <div class="modal-dialog modal-xl modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Store Details</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <table data-role="none" id="tblSalesReportDetails" class="table table-light table-hover table-striped" style="width:100%;">
                    <thead class="align-top text-center" style="font-size:14px;">
                        <tr>
                            <th class="table-active fixed-col excludecolspan" colspan="1"></th>
                            <th colspan="10">Sales Information</th>
                            <th class="table-active thSalesByStorePaymentType" colspan="1">Card Payment <button data-role="none" class="btn" id="btnToggleMoreDetailsSalesByStorePaymentTypeDetails" data-toggle="tooltip" title="Show details"><span class="bi-chevron-right"></span></button></th>
                            <th class="thSalesByStoreEWallet" colspan="1">E-Wallet <button data-role="none" class="btn" id="btnToggleMoreDetailsSalesByStoreEWalletDetails" data-toggle="tooltip" title="Show details"><span class="bi-chevron-right"></span></button></th>
                            <th class="table-active thSalesByStoreDeliveries" colspan="1">Deliveries <button data-role="none" class="btn" id="btnToggleMoreDetailsSalesByStoreDeliveriesDetails" data-toggle="tooltip" title="Show details"><span class="bi-chevron-right"></span></button></th>
                            <th colspan="5">Order Method</th>
                        </tr>
                        <tr>
                            <th class="table-active fixed-col" data-sort="date" style="cursor:pointer;"> Date</th>

                            <th data-sort="accounting" class="text-nowrap" style="cursor:pointer;"> Transaction Count </th>
                            <th data-sort="accounting" class="text-nowrap" style="cursor:pointer;"> Average Check </th>
                            <th data-sort="accounting" class="text-nowrap" style="cursor:pointer;"> Gross Sales </th>
                            <th data-sort="accounting" class="text-nowrap" style="cursor:pointer;"> Sales Return/Refund </th>
                            <th data-sort="accounting" class="text-nowrap" style="cursor:pointer;"> Total Discount </th>
                            <th data-sort="accounting" class="text-nowrap" style="cursor:pointer;"> Net Sales </th>
                            <th data-sort="accounting" class="text-nowrap" style="cursor:pointer;"> Total Cash</th>
                            <th data-sort="accounting" class="text-nowrap" style="cursor:pointer;"> Total Gift Cert</th>
                            <th data-sort="accounting" class="text-nowrap" style="cursor:pointer;"> Total Voucher</th>
                            <th data-sort="accounting" class="text-nowrap" style="cursor:pointer;"> Total PAS</th>

                            <th class="table-active text-nowrap colSalesByStoreAccumulatedPayment" data-sort="accounting" style="cursor:pointer; display:revert; min-width:200px;">Total Card Payment</th>
                            <th class="table-active text-nowrap colSalesByStorePaymentdetails" data-sort="accounting" style="cursor:pointer; display:none;">Credit Card</th>
                            <th class="table-active text-nowrap colSalesByStorePaymentdetails" data-sort="accounting" style="cursor:pointer; display:none;">Debit Card</th>

                            <th class="colSalesByStoreAccumulatedEWallet text-nowrap" data-sort="accounting" style="cursor:pointer; display:revert; min-width:200px;">Total E-Wallet</th>
                            <th class="colSalesByStoreEWalletdetails text-nowrap" data-sort="accounting" style="cursor:pointer; display:none;">G Cash</th>
                            <th class="colSalesByStoreEWalletdetails text-nowrap" data-sort="accounting" style="cursor:pointer; display:none;">Pay Maya</th>
                            <th class="colSalesByStoreEWalletdetails text-nowrap" data-sort="accounting" style="cursor:pointer; display:none;">SMAC</th>
                            <th class="colSalesByStoreEWalletdetails text-nowrap" data-sort="accounting" style="cursor:pointer; display:none;">Gold Card</th>
                            <th class="colSalesByStoreEWalletdetails text-nowrap" data-sort="accounting" style="cursor:pointer; display:none;">Shopee Pay</th>
                            <th class="colSalesByStoreEWalletdetails text-nowrap" data-sort="accounting" style="cursor:pointer; display:none;">Shopee E-Voucher</th>
                            <th class="colSalesByStoreEWalletdetails text-nowrap" data-sort="accounting" style="cursor:pointer; display:none;">Lazada</th>
                            <th class="colSalesByStoreEWalletdetails text-nowrap" data-sort="accounting" style="cursor:pointer; display:none;">Share Treats</th>

                            <th class="table-active colSalesByStoreAccumulatedDeliveries text-nowrap" data-sort="accounting" style="cursor:pointer; display:revert; min-width:200px;">Total Delivery Aggregators</th>
                            <th class="table-active colSalesByStoreDeliveryDetails text-nowrap" data-sort="accounting" style="cursor:pointer; display:none;">Food Panda</th>
                            <th class="table-active colSalesByStoreDeliveryDetails text-nowrap" data-sort="accounting" style="cursor:pointer; display:none;">Grab</th>
                            <th class="table-active colSalesByStoreDeliveryDetails text-nowrap" data-sort="accounting" style="cursor:pointer; display:none;">Pood Trip</th>
                            <th class="table-active colSalesByStoreDeliveryDetails text-nowrap" data-sort="accounting" style="cursor:pointer; display:none;">DeliRush</th>
                            <th class="table-active colSalesByStoreDeliveryDetails text-nowrap" data-sort="accounting" style="cursor:pointer; display:none;">Eatigo</th>
                            <th class="table-active colSalesByStoreDeliveryDetails text-nowrap" data-sort="accounting" style="cursor:pointer; display:none;">Booky</th>
                            <th class="table-active colSalesByStoreDeliveryDetails text-nowrap" data-sort="accounting" style="cursor:pointer; display:none;">Pickaroo</th>
                            <th class="table-active colSalesByStoreDeliveryDetails text-nowrap" data-sort="accounting" style="cursor:pointer; display:none;">Local Delivery Partners</th>

                            <th data-sort="accounting" class="text-nowrap" style="cursor:pointer;">Total E-Payment</th>
                            <th data-sort="accounting" class="text-nowrap" style="cursor:pointer;">Total Walk In</th>
                            <th data-sort="accounting" class="text-nowrap" style="cursor:pointer;">Total Delivery</th>
                            <th data-sort="accounting" class="text-nowrap" style="cursor:pointer; min-width: 200px;">Cashier</th>
                            <th data-sort="accounting" class="text-nowrap" style="cursor:pointer;">Latest Transaction Date</th>
                        </tr>
                    </thead>
                    <tbody></tbody>
                    <tfoot>
                        <tr>
                            <th class="fixed-col excludecolspan" style="text-align:right">Totals:</th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                        </tr>
                    </tfoot>
                </table>
            </div>
        </div>
    </div>
</div>

<div id="popupDeviceUtilizationDetails" class="modal fade" style="display:none;">
    <div class="modal-dialog modal-xl modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Device Utilization Details</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                                
            </div>
        </div>
    </div>
</div>

<div id="popupFOWSTStatus" data-backdrop="static" class="modal fade" style="display:none;">
    <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Store Status</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    <label class="form-label">Store Status:</label>
                    <select data-role="none" id="selFOWSTStatus" class="form-select">
                        <option value="Store and device are operational.">Store and device are operational.</option>
                        <option value="Store is operational. No device is deployed yet.">Store is operational. No device is deployed yet.</option>
                        <option value="Store is operational. Device is on hold.">Store is operational. Device is on hold.</option>
                        <option value="Store is operational. Waiting for device to arrive.">Store is operational. Waiting for device to arrive.</option>
                        <option value="Store is operational. No device available. Stolen.">Store is operational. No device available. Stolen.</option>
                        <option value="Store is operational. No device available. Broken.">Store is operational. No device available. Broken.</option>
                        <option value="Store is permanently closed.">Store is permanently closed.</option>
                        <option value="Store is temporarily closed for holiday.">Store is temporarily closed for holiday.</option>
                        <option value="Store is temporarily closed for company activity.">Store is temporarily closed for company activity.</option>
                        <option value="Others">Others</option>
                    </select>
                </div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col">
                        <button data-role="none" class="btn btn-secondary btn-lg btn-block" data-dismiss="modal">Cancel</button>
                    </div>
                    <div class="col">
                        <button data-role="none" class="btn btn-success btn-lg btn-block" id="btnSaveFOWSTStatus">Save</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="panInAbilityReason" data-backdrop="static" class="modal fade" style="display:none;">
    <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Reason for Unavailability<span class="lblPopUpHeaderLibraryName"></span></h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body" id="panSetBegBalanceInventoryLedgercontent-bs">
                <div class="mb-3">
                    <label class="form-label">Enter Reason for device (<i id="lblDeviceName"></i>) unavailability: <br /> <br /></label>
                    <textarea id="txtReasonforUnavailability" data-role="none" class="form-control" style="resize:none; height:150px;"> </textarea>
                </div>

                <div id="divDeviceFowst"></div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col">
                        <button data-role="none" class="btn btn-secondary btn-lg btn-block" data-dismiss="modal">Cancel</button>
                    </div>
                    <div class="col">
                        <button data-role="none" class="btn btn-success btn-lg btn-block btnSetReasonforUnavailability">Save</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="panFowstOpHrs" data-backdrop="static" class="modal fade" style="display:none;">
    <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Store Schedules</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body" id="panSetBegBalanceInventoryLedgercontent-bs">
                <label class="mb-3"></label>
                <div class="row">
                    <div class="col-6">
                        <div class="form-check">
                            <label class="form-check-label" for="fowstophrs-day-2">Monday</label>
                            <input data-role="none" type="checkbox" class="form-check-input" id="fowstophrs-day-2" name="fowstophrs-day" value="2">
                        </div>
                    </div>
                    <div class="col-6">
                        <div class="form-check">
                            <label class="form-check-label" for="fowstophrs-day-6">Friday</label>
                            <input data-role="none" type="checkbox" class="form-check-input" id="fowstophrs-day-6" name="fowstophrs-day" value="6">
                        </div>
                    </div>
                    <div class="col-6">
                        <div class="form-check">
                            <label class="form-check-label" for="fowstophrs-day-3">Tuesday</label>
                            <input data-role="none" type="checkbox" class="form-check-input" id="fowstophrs-day-3" name="fowstophrs-day" value="3">
                        </div>
                    </div>
                    <div class="col-6">
                        <div class="form-check">
                            <label class="form-check-label" for="fowstophrs-day-7">Saturday</label>
                            <input data-role="none" type="checkbox" class="form-check-input" id="fowstophrs-day-7" name="fowstophrs-day" value="7">
                        </div>
                    </div>
                    <div class="col-6">
                        <div class="form-check">
                            <label class="form-check-label" for="fowstophrs-day-4">Wednesday</label>
                            <input data-role="none" type="checkbox" class="form-check-input" id="fowstophrs-day-4" name="fowstophrs-day" value="4">
                        </div>
                    </div>
                    <div class="col-6">
                        <div class="form-check">
                            <label class="form-check-label" for="fowstophrs-day-1">Sunday</label>
                            <input data-role="none" type="checkbox" class="form-check-input" id="fowstophrs-day-1" name="fowstophrs-day" value="1">
                        </div>
                    </div>
                    <div class="col-6">
                        <div class="form-check">
                            <label class="form-check-label" for="fowstophrs-day-5">Thursday</label>
                            <input data-role="none" type="checkbox" class="form-check-input" id="fowstophrs-day-5" name="fowstophrs-day" value="5">
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col">
                        <button data-role="none" class="btn btn-secondary btn-lg btn-block" data-dismiss="modal">Cancel</button>
                    </div>
                    <div class="col">
                        <button data-role="none" name="btnSave" class="btn btn-success btn-lg btn-block">Save</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
    
<div data-role="content" class="mainContent mainContentForDashboard">
    <ul data-role="listview" style="display:none;">
        @*ayaw lang ni imind DO NOT REMOVE by arnie*@
        <li id="li2DaysAgo"><a>2 Days Ago</a></li>
        <li id="liYesterday"><a>Yesterday</a></li>
        <li id="liToday"><a>Today</a></li>
        <li id="liWeekToDate"><a>Week To Date</a></li>
        <li id="liMonthToDate"><a>Month To Date</a></li>
        <li id="liCustomPeriod"><a>Custom Period</a></li>
        <li id="btnDone"><a>Done</a></li>
        <li id="txtDateFrom"><a>Done</a></li>
        <li id="txtDateTo"><a>Done</a></li>
        <li class="btnSalesAnalytics"><a>Sales Analytics</a></li>
    </ul>

    <div id="popupCustomerDetailsWithID" class="modal fade">
        <div class="modal-dialog modal modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Customer Details</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-auto mb-3">
                            <img src="/Content/images/NoPhoto.jpg" class="img-thumbnail" style="width:150px; height:150px;">
                        </div>
                        <div class="col-sm-6 mb-3">
                            <h4 id="lblCustomerName">Juan Dela Cruz</h4>
                            <p id="lblBirthDate">ID: 1234</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popSliceByGroups" class="modal fade">
        <div class="modal-dialog modal-fullscreen">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Dashboard Filters</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3 row">
                        <label class="col-form-label col-md-auto">
                            Period:
                        </label>
                        <div class="col-md-auto">
                            <button data-role="none" id="btnPeriodSlice" class="btn btn-secondary">
                                Today
                            </button>
                        </div>
                        <label class="col-form-label col-md-auto">
                            Filter by:
                        </label>
                        <div class="col-md-auto">
                            <select data-role="none" id="selSliceByGroups" class="form-select">
                                <option value="">Please select Group</option>
                            </select>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-md-3 mb-2">
                            <table data-role="none" class="table table-light table-striped table-hover" id="tbltopStoresInGroup" style="width:100%;">
                                <thead>
                                    <tr class="align-top text-center" style="font-size:14px;">
                                        <th style="width:50px;">#</th>
                                        <th>Top 5 Store</th>
                                        <th style="width:100px;">Gross Sales</th>
                                        <th style="width:100px;">ADS</th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>
                        <div class="col-md-9">
                            <div style="min-height:400px;">
                                <canvas id="chartGroupSalesReport" width="400" height="500"></canvas>
                            </div>
                        </div>
                    </div>

                    <br /><br />

                    <div class="row mb-12">
                        
                        <h3 id="lblSliceByLabel"> </h3>

                        <div>
                            <div style="min-height:400px;">
                                <canvas id="chartGroupSalesReport1" width="400" height="500"></canvas>
                            </div>
                            
                            <div class="table-responsive mb-3">
                                <table id="tblGroupSalesReportTable1" data-role="none" style="display:table;" class="table table-light table-striped table-hover headerfitwidth">
                                    <thead id="tblSecondChartHeader"></thead>
                                    <tbody id="tblSecondChartBody"></tbody>
                                    <tfoot id="tblSecondChartFooter"></tfoot>
                                </table>
                            </div>
                        </div>
                    </div>
                    
                    <br /><br />

                    <div class="row mb-12">
                        
                        <h3 id="lblDiceByLabel"> </h3>

                        <div style="margin:20px">
                            <div style="min-height:400px;">
                                <canvas id="chartGroupSalesReport2" width="400" height="500"></canvas>
                            </div>
                            
                            <div class="table-responsive mb-3">
                                <table id="tblGroupSalesReportTable2" data-role="none" style="display:table;" class="table table-light table-striped table-hover headerfitwidth">
                                    <thead id="tblSecondChartHeader"></thead>
                                    <tbody id="tblSecondChartBody"></tbody>
                                    <tfoot id="tblSecondChartFooter"></tfoot>
                                </table>
                            </div>
                        </div>
                    </div>
                    
                </div>
                <div class="modal-footer">
                    <button data-role="none" class="btn btn-secondary" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <div id="popInventoryLevelApproveDisapprove" class="modal fade">
            <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Inventory Count Approval/Disapproval</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <ul class="nav nav-tabs" id="nav-tab" role="tablist">
                            <li class="nav-item">
                                <a data-role="none" class="nav-link active" data-toggle="tab" href="#divILADTransaction">Transaction</a>
                            </li>
                            <li class="nav-item">
                                <a data-role="none" class="nav-link" id="btnTabILADHistory" data-toggle="tab" href="#divILADHistory">History</a>
                            </li>
                        </ul>

                        <div class="tab-content">
                            <div class="tab-pane fade show active p-3" id="divILADTransaction">
                                <div id="divILAppDisHeader" style="display:block;">
                                    <div class="text-right mb-3">
                                        <button id="btnRefreshILAP" style="margin-right: 10px; display: block;" data-role="none" class="btn btn-success">
                                            Refresh
                                        </button>
                                    </div>                                    
                                    <h6 class="mb-3"><b id="txtInventoryLevelApproveDisapproveFowstName"></b></h6>
                                    <div class="table-responsive">
                                        <table data-role="none" class="table table-light table-striped table-hover headerfitwidth">
                                            <thead>
                                                <tr class="align-top text-center" style="font-size:14px;">
                                                    <th data-sort="accounting" style="cursor:pointer;"> # </th>
                                                    <th data-sort="date" style="cursor:pointer;"> Physical Count Date and Time </th>
                                                    <th data-sort="accounting" style="cursor:pointer;"> No of Unapproved Count </th>
                                                    <th data-sort="string" style="cursor:pointer;"> Counted By </th>
                                                    <th data-sort="string" style="cursor:pointer;"> Remarks </th>
                                                </tr>
                                            </thead>
                                            <tbody id="tblnventoryLevelApproveDisapprove"></tbody>
                                        </table>
                                        <br />
                                    </div>
                                </div>

                                <div id="divILAppDisDetails" style="display:none;">
                                    <div class="text-left mb-3">
                                        <button id="btnBackILAP" style="margin-right: 10px; display: block;" data-role="none" class="btn btn-secondary">
                                            Back
                                        </button>
                                    </div>
                                    <h6 class="mb-3"><b id="txtInventoryLevelApproveDisapproveDate"></b></h6>
                                    <div class="table-responsive">
                                        <table data-role="none" class="table table-light table-striped table-hover headerfitwidth">
                                            <thead>
                                                <tr class="align-top text-center" style="font-size:14px;">
                                                    <th style="text-align:center;"><label><input type="checkbox" data-role="none" class="form-check-input" name="checkbox-0" id="btnCheckAll" /></label></th>
                                                    <th data-sort="accounting" style="cursor:pointer;"> # </th>
                                                    <th data-sort="string" style="cursor:pointer;"> Bin Name </th>
                                                    <th data-sort="date" style="cursor:pointer;"> Inventory Count Date </th>
                                                    <th data-sort="string" style="cursor:pointer;"> Product Name </th>
                                                    <th data-sort="accounting" style="cursor:pointer;"> Observed Count </th>
                                                    <th data-sort="string" style="cursor:pointer;"> Unit Measure </th>
                                                    <th data-sort="accounting" style="cursor:pointer;"> Variance </th>
                                                    <th colspan="2" style="cursor:pointer;"> Action </th>
                                                </tr>
                                            </thead>
                                            <tbody id="tblnventoryLevelApproveDisapproveDetails"></tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>

                            <div class="tab-pane fade p-3" id="divILADHistory">
                                <div class="text-right mb-3">
                                    <button id="btnRefreshILAPHistory" style="margin-right: 10px; display: block;" data-role="none" class="btn btn-success">
                                        Refresh
                                    </button>
                                </div>
                                <h6 class="mb-3"><b><span style="color:#F00">*</span> History of all approved/disapproved item(s) of <span id="txtILADHistoryFowstName"></span></b></h6>
                                <table data-role="none" class="table table-light table-striped table-hover headerfitwidth">
                                    <thead>
                                        <tr id="tblILADHistoryHeader" class="align-top text-center" style="font-size:14px;">
                                            <th data-sort="float" style="cursor:pointer;">#</th>
                                            <th data-sort="string" style="cursor:pointer;">FOWST</th>
                                            <th data-sort="string" style="cursor:pointer;">Bin</th>
                                            <th data-sort="date" style="cursor:pointer;">Inventory Count Date</th>
                                            <th data-sort="string" style="cursor:pointer;">Product Name</th>
                                            <th data-sort="float" style="cursor:pointer;">Observed Count</th>
                                            <th data-sort="string" style="cursor:pointer;">Unit Measure</th>
                                            <th data-sort="accounting" style="cursor:pointer;">Variance</th>
                                            <th data-sort="string" style="cursor:pointer;">Status</th>
                                            <th data-sort="string" style="cursor:pointer;">Modified By</th>
                                            <th data-sort="date" style="cursor:pointer;">Modified Date & Time</th>
                                            @*<th style="cursor:pointer;"></th>*@
                                        </tr>
                                    </thead>
                                    <tbody id="tblILADHistoryBody"></tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col">
                                <button data-role="none" class="btn btn-secondary btn-block btnCloseILAP">Close</button>
                            </div>
                            <div class="col divbtnILAD" style="display:none;">
                                <button data-role="none" id="btnILADChbDisapp" class="btn btn-danger btn-block">Disapprove</button>
                            </div>
                            <div class="col divbtnILAD" style="display:none;">
                                <button data-role="none" id="btnILADChbApp" class="btn btn-success btn-block">Approve</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    <div id="popTransDetails" class="modal fade">
            <div class="modal-dialog modal-fullscreen modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Invoice Details</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div class="row mb-3">
                            <div class="col-sm-auto mb-3">Invoice Date and Time: <span id="txtInvoiceDate" style="font-weight:bold;"></span></div>
                            <div class="col-sm-auto mb-3">Invoice Number: <span id="txtInvoiceNumber" style="font-weight:bold;"></span></div>
                            <div class="col-sm-auto mb-3">Sales Representative: <span id="txtSalesRep" style="font-weight:bold;"></span></div>
                            @*<div class="col-sm text-right">
                                --hidden kay mali ang details if naay discount
                                <button data-role="none" id="butCopyToClipboard" class="btn btn-light border"><span class="bi-box-arrow-up-right"></span> Show Preview</button>
                            </div>*@
                        </div>

                        <div class="mb-3">
                            <table data-role="none" id="tblTransactionDetailsBody" class="table table-light table-hover table-striped headerfitwidth">
                                <thead>
                                    <tr class="align-top text-center" style="font-size:14px;">
                                        @*<th data-sort="int" style="cursor:pointer;"> Invoice No. </th>
                        <th data-sort="date" style="cursor:pointer;"> Invoice Date </th>*@
                                        <th data-sort="string" style="cursor:pointer;cursor:pointer;"> Product Description </th>
                                        <th data-sort="accounting" style="text-align: center;cursor:pointer;"> Qty </th>
                                        @*<th data-sort="accounting" style="text-align: center;cursor:pointer;"> Qty Returned </th>*@
                                        <th data-sort="string" style="cursor:pointer;cursor:pointer;"> Unit Measure </th>
                                        @*<th data-sort="string" style="cursor:pointer;cursor:pointer;"> Returned Product For Rebate</th>
                        <th data-sort="accounting" style="cursor:pointer;cursor:pointer;"> Rebate Qty </th>
                        <th data-sort="string" style="cursor:pointer;cursor:pointer;"> Rebate Unit Measure</th>*@
                                        <th data-sort="accounting" style="text-align: right;cursor:pointer;"> Amount </th>
                                        <th data-sort="accounting" style="text-align: right;cursor:pointer;"> Sub Total </th>
                                        @*<th data-sort="accounting" style="text-align: right;cursor:pointer;"> Discount </th>*@
                                        @*<th data-sort="string" style="cursor:pointer;"> Sales Representative(s) </th>*@
                                        <th data-sort="string" style="cursor:pointer;"> Customer </th>
                                        <th data-sort="string" style="cursor:pointer;"> Last Updated By </th>
                                    </tr>
                                </thead>
                                <tbody id="tblTransactionDetailsBody"></tbody>
                            </table>
                        </div>

                        <div id="dvSeniorCitizen" style="visibility:hidden; display:none">
                            <h5 class="mb-3">Senior Citizen/PWD Information</h5>
                            <table data-role="none" id="tblSeniorCitizen" class="table table-light table-hover table-striped headerfitwidth">
                                <thead>
                                    <tr class="align-top text-center" style="font-size:14px;">
                                        <th>
                                            Name
                                        </th>
                                        <th>
                                            Picture of SC/PWD ID
                                        </th>
                                    </tr>
                                </thead>
                                <tbody id="tbodySeniorCitizen"></tbody>
                            </table>
                        </div>

                        <div id="divImage" class="tooltip-window">
                            <div class="tooltip-content">
                                <div>
                                    <h4><span id="lblSCFullName"></span></h4>
                                </div>

                                <div id="imgSCIDImage" style="text-align:center">

                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer text-right">
                        <button data-role="none" class="btn btn-secondary" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>

    <div id="divCacMcr" subpage="dashboard" style="display:none;">

    </div>

</div>