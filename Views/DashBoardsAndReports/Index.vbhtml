@Code
    ViewData("Title") = "Dashboards and Reports"
    Layout = "~/Views/Shared/plain.vbhtml"
End Code

@Section head
    <link rel="stylesheet" href="/Content/css/jquery.jqplot.min.css" />
    <link rel="stylesheet" href="/Content/css/ui.dynatree.css" />
    <link rel="stylesheet" href="/Content/css/qunit.css" />
    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <link rel="stylesheet" href="/Content/bootstrap-custom.css" />
    <link rel="stylesheet" href="/Content/metro-calendar.css" />
    <link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
     
    <script>        
        require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/SalesDashboardIndex.js?crc=' + assetversion)]);
        require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/DashboardsAndReportsCharts.js?crc=')]);
        require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/flatui.js?crc=')]);
    </script>
    
    <style>    
     .jqplot-xaxis-hide {
            display: none
        }
    </style>
End Section

@*Header*@
    <div class="custom-theme-a position-fixed" id="divHeaderForDashboard">

        <div id="HeaderDesktopVersion">
            <div class="ui-grid-a ui-responsive">
                <div class="ui-block-a" style="overflow:hidden; display:flex;">
                    <div style="float:left; margin-top:5px; cursor:pointer;">
                        @*<a class="btnBack" data-iconpos="notext" data-theme="b" data-role="button" data-icon="back" title="Back">Back</a>*@
                        <a style="margin-top:8px;" class="btnHome" data-iconpos="notext" data-theme="b" data-role="button" data-icon="home" title="Home">Home</a>
                    </div>

                    <div style="float:left; margin-top:10px; margin-left:25px; margin-bottom:15px;">                            
                        <div class="OrganizationNameHeader" style="text-overflow:ellipsis; font-size:25px; white-space:nowrap; overflow:hidden;">
                            Company Name
                        </div>
                        <div id="divHeaderRequiredParameters" style="margin-top:15px; display:none;">
                            <div class="btnSelectFowst" style="display:none; cursor:pointer; float:left; margin-bottom:10px; margin-right:50px; text-transform:capitalize;">
                                <span class="lblSelectedFowstGroup"> Selected Fowst </span>
                                <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span>
                            </div>
                            <div id="divBtnPeriod2" style="display:none; cursor:pointer; float:left;margin-bottom:10px; text-transform:capitalize;">
                                <span class="lblPeriod2"> Today </span>
                                <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span>
                            </div>
                            <div id="divBtnPeriod" style="display:block; cursor:pointer; float:left;margin-bottom:10px; text-transform:capitalize;" class="btnPeriod">
                                <span class="lblPeriod"> Today </span>
                                <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="ui-block-b txt-right" style="padding:0px 16px 0px 20px;">
                    <div id="divSearchBox" style="margin-bottom:-10px; visibility:hidden; display:none;">
                        <div class="ui-grid-a">
                            <div class="ui-block-a" style="width:80%">
                                <input type="text" placeholder="Search invoice no., product description, sales representative or customer" class="txtSearchOLD" />
                            </div>
                            <div class="ui-block-b" style="padding:8px; width: 20%;">
                                <a class="btn custom-btn-theme-a btnSearchOLD">
                                    <span class="ui-icon ui-icon-search" style="display:inline-block;"></span>
                                    Search
                                </a>
                            </div>
                        </div>
                        <div class="txtSearchStatus" style="display:none; float:left; color:#FF5959; text-shadow:0px 0px 0px #FFF;">
                            Search box is required!
                        </div>
                    </div>

                    <br />

                    <div style="cursor:pointer; float:right; margin-top:-5px; margin-bottom:10px;" class="btnMoreReport">
                        <span> Go To </span>
                        <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span>
                    </div>
                </div>
            </div>
        </div>

        <div id="HeaderMobileVersion">
            <div class="ui-grid-solo">
                <div class="ui-block-a OrganizationNameHeader txt-center">
                    Company Name Here
                </div>
            </div>
            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <a class="btn custom-btn-theme-a btnHome">
                        <span class="ui-icon ui-icon-home" style="display:inline-block;">
                            <p style="margin:16px 0px 0px -7px; font-size:12px;">Home</p>
                        </span>
                    </a>
                </div>
                @*<div class="ui-block-b">
                    <a id="btnSearchMobile" class="btn custom-btn-theme-a">
                        <span class="ui-icon ui-icon-search" style="display:inline-block;">
                            <p style="margin:16px 0px 0px -11px; font-size:12px;">Search</p>
                        </span>
                    </a>
                </div>
                <div class="ui-block-c">
                    <a class="btn custom-btn-theme-a btnPeriod">
                        <span class="ui-icon ui-icon-grid" style="display:inline-block;">
                            <p style="margin:16px 0px 0px -7px; font-size:12px;">Period</p>
                        </span>
                    </a>
                </div>*@
                <div class="ui-block-b">
                    <a class="btn custom-btn-theme-a btnMoreReport">
                        <span class="ui-icon ui-icon-gear" style="display:inline-block;">
                            <p style="margin:16px 0px 0px -5px; font-size:12px;">Go To</p>
                        </span>
                    </a>
                </div>
            </div>
            <div class="contentSearchEngine">
                <div class="ui-grid-solo ui-responsive">
                    <div class="ui-block-a" style="padding:6px;">
                        <input type="text" placeholder="Search Here" class="txtSearch" />
                    </div>
                </div>
            </div>
            @*<div class="divLblPeriod">
                <div class="lblPeriod">
                    Today
                </div>
            </div>
            <br />*@
        </div>

        <div class="BreadCrumbs">
            <div class="ItemsBreadCrumbs">
                @*Main Dashboard*@
            </div>
        </div>

        @*PopUp PanDates*@

        @*<div data-role="popup" id="panDates" data-overlay-theme="a" data-theme="a" data-corners="false"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Period</h3></center>
            <div data-role="content">
                <form>
                    <ul data-role="listview" class="custom-theme-a" data-inset="true" data-corners="false">
                        <li id="li2DaysAgo"><a href="#">2 Days Ago</a></li>
                        <li id="liYesterday"><a href="#">Yesterday</a></li>
                        <li id="liToday"><a href="#">Today</a></li>
                        <li id="liWeekToDate"><a href="#">Week To Date</a></li>                        
                        <li id="liMonthToDate"><a href="#">Month To Date</a></li>
                        <li id="liCustomPeriod"><a href="#">Custom Period</a></li>
                    </ul>
                </form>
            </div>       
        </div>*@
        <div data-role="popup" id="panGoToOffline" data-overlay-theme="a" data-theme="a" data-corners="false"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Offline Reports</h3></center>
            <div data-role="content">
                <div style="padding: 8px 15px 16px 15px;">
                    <ul data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm">
                        <li><a id="btnOfflineCashier">Cashiers Sales Report</a></li>
                        <li><a id="btnOfflineManager">Managers Sales Report</a></li>
                        <li><a id="btnOfflineMonies">Monies Accountability</a></li>
                        <li><a id="btnOfflineInventories">Inventories Accountability</a></li>
                        <li><a id="btnOfflineTransaction">Transaction Details</a></li>
                        <li><a id="btnOfflineInventoryLedger">Inventory Ledger</a></li>
                        <li><a id="btnOfflineZRead">Z-Read</a></li>
                        <li><a id="btnOfflineSMCOINReport">SM COIN Report</a></li>
                    </ul>
                </div>
            </div>       
        </div>

        <div data-role="popup" id="panDates" data-overlay-theme="a" data-theme="a" data-corners="false"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Period</h3></center>
            <div data-role="content">
                <div @*class="DatesPopUp"*@ style="padding:10px;">
                    <ul data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm">
                        <li id="li2DaysAgo"><a>2 Days Ago</a></li>
                        <li id="liYesterday"><a>Yesterday</a></li>
                        <li id="liToday"><a>Today</a></li>
                        <li id="liWeekToDate"><a>Week To Date</a></li>                        
                        <li id="liMonthToDate"><a>Month To Date</a></li>
                        <li id="liCustomPeriod"><a>Custom Period</a></li>
                    </ul>
                </div>
            </div>       
        </div>

        @*PopUp PanCustomDates*@

        <div data-role="popup" id="panCustomDates" data-overlay-theme="a" data-theme="a" data-corners="false"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Custom Period</h3></center>
            <div data-role="content">
                <div class="ui-grid-a ui-responsive">
                    <div class="ui-block-a" style="padding:7px;">
                        <div class="metro" style="max-width:252px; overflow:auto;">
                            From: <span id="txtDateFrom"></span>
                            <div class="fromCalendar calendar"></div>
                        </div>
                    </div>
                    <div class="ui-block-b" style="padding:7px;">
                        <div class="metro" style="max-width:252px; overflow:auto;">
                            To: <span id="txtDateTo"></span>
                            <div class="toCalendar calendar"></div>
                        </div>
                    </div>
                </div>
                <br />
                <div class="ui-grid-a ui-responsive">
                    <div class="ui-block-a" style="padding:0px 7px 7px 7px;">
                        <a id="btnCancel" class="btn custom-btn-theme-c">
                            Cancel
                        </a>
                    </div>
                    <div class="ui-block-b" style="padding:0px 7px 7px 7px;">
                        <a id="btnDone" class="btn custom-btn-theme-g">
                            Done
                        </a>
                    </div>
                </div>
            </div>       
        </div>

        @*PopUp MoreReports*@

        <div data-role="popup" class="MoreReportsPopUp" id="panOption" data-overlay-theme="a" data-theme="a" data-corners="false"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="btnCloseMoreOption">Close</a>
            <center><h3>Go To</h3></center>

            <div data-role="content">

                <div class="noBorderList" data-role="collapsible-set" data-corners="false" data-theme="a">
                    <div style="padding:15px;">
                        <ul data-role="listview" data-theme="a">
                            <li><a class="btnMainDashboardPage">Main Dashboard</a></li>
                            <li><a class="btnSalesAnalyticsPage">Sales Analytics</a></li>
                            <li><a class="btnCreditAndCollectionPage">Credit And Collection</a></li>
                            <li><a class="btnReplenishmentProcessing">Store Replenishment Processing & Analytics</a></li>
                            <li><a class="btnSalesOrderProcessingAnalyticsPage">Sales Order Processing & Analytics</a></li>
                            <li><a id="butPendingSalesReturns" >Request and Approvals</a></li>
                            <li><a class="btnTrackDevice">GPS Location</a></li>
                        </ul>
                    </div>

                   @* <div data-role="collapsible" data-collapsed="false">
                        <h3>Online Reports</h3>
                        <ul data-role="listview">
                            <li><a class="btnSalesAnalyticsPage custom-btn-theme-c">Sales Analytics</a></li>
                            <li>
                                <a class="btnInventoryAnalytics custom-btn-theme-c">Inventory Analytics</a>
                            </li>*@

                            @*Inventory Analytics Sub Menu*@
                               @* <li class="IventoryAnalyticsSubList" style="display:none; margin:0px 10px 0px 10px; font-weight:normal;">
                                    <a class="btnInventoryReceiving custom-btn-theme-c">Inventory Receiving</a>
                                </li>
                                <li class="IventoryAnalyticsSubList" style="display:none; margin:0px 10px 0px 10px; font-weight:normal;">
                                    <a class="btnInventoryTransfer custom-btn-theme-c">Inventory Transfer</a>
                                </li>
                                <li class="IventoryAnalyticsSubList" style="display:none; margin:0px 10px 0px 10px; font-weight:normal;">
                                    <a class="btnInventoryCounts custom-btn-theme-c">Inventory Count</a>
                                </li>

                            <li><a class="btnReplenishmentProcessing custom-btn-theme-c">Store Replenishment Processing & Analytics</a></li>
                            <li><a class="btnSalesOrderProcessingAnalyticsPage custom-btn-theme-c">Sales Order Processing & Analytics</a></li>
                            <li><a class="btnTrackDevice custom-btn-theme-c">Track Device</a></li>
                        </ul>
                    </div>*@

                    <div data-role="collapsible">
                        <h3>Inventory Reports and Analytics</h3>
                        <ul data-role="listview">
                            <li><a class="btnMoreOptionInventoryBalance custom-btn-theme-c">Inventory Balance</a></li>
                            <li><a class="btnMoreOptionInventoryLedger custom-btn-theme-c">Inventory Ledger</a></li>
                            <li><a class="btnMoreOptionStockReceivingAndTransfers custom-btn-theme-c">Stock Receiving and Transfers</a></li>
                            @*<li><a class="custom-btn-theme-c">Inventory Transfer</a></li>*@
                            <li><a class="btnInventoryCounts custom-btn-theme-c">Physical Inventory and Variance Report</a></li>
                            <li><a class="btnInventoryLevel custom-btn-theme-c">Stock Inventory Levels</a></li>
                            @*<li><a id="btnMoreOptionInventoryFormReport" class="custom-btn-theme-c">Inventory Form </a></li>*@
                        </ul>
                    </div>
                    
                    <div data-role="collapsible">
                        <h3>On-Print POS Sales Reports</h3>
                        <ul data-role="listview">
                            <li><a id="popPOSReportCashierSalesReport" class="custom-btn-theme-c">Cashiers Sales Report</a></li>
                            <li><a id="popPOSReportManagerReport" class="custom-btn-theme-c">Manager Sales Report</a></li>
                            @*<li><a id="popPOSReportSalesAdjustment" class="custom-btn-theme-c">Sales Adjustment Report</a></li>*@
                            <li><a id="popPOSReportZReport" class="custom-btn-theme-c">Z Read Report</a></li>
                            <li><a id="popPOSReportSKUSalesReport" class="custom-btn-theme-c">Sales by SKU Report</a></li>
                        </ul>
                    </div>

                    <div data-role="collapsible">
                        <h3>Cash Handling</h3>
                        <ul data-role="listview">                            
                            <li><a id="popPOSReportDeposit" class="custom-btn-theme-c">Monies Accountability</a></li>
                        </ul>
                    </div>

                    <div data-role="collapsible">
                        <h3>Special Reports</h3>
                        <ul data-role="listview">
                            <li><a id="popPOSReportMallLessorAyala" class="custom-btn-theme-c">Ayala Tenant Monitoring Report</a></li>
                            <li><a id="popPOSReportMallLessorSM" class="custom-btn-theme-c">SM COIN Report</a></li>
                            <li><a id="popPOSReportPrincipal" class="custom-btn-theme-c">Report to Principal</a></li>
                            <li><a id="popPOSReportShopperTrak" class="custom-btn-theme-c">ShopperTrak</a></li>
                            <li><a id="popPOSReportInventoryForm" class="custom-btn-theme-c">Inventory Form Report</a></li>
                            <li><a id="popPOSReportSCDAndPWDReports" class="custom-btn-theme-c">SCD and PWD Reports</a></li>
                            <li><a id="popPOSSRVHistory" class="custom-btn-theme-c">SRV History</a></li>
                            @*<li><a id="popPOSReportEJournal" class="custom-btn-theme-c">E-Journal Report</a></li>*@
                        </ul>
                    </div>

                    <div data-role="collapsible">
                        <h3>Audit Trail</h3>
                        <ul data-role="listview">
                            <li><a id="liPOSTransactionDetail" class="custom-btn-theme-c">POS Transaction Detail</a></li>
                            <li><a id="liInventoryLedger" class="custom-btn-theme-c">Inventory Ledger</a></li>
                            <li><a id="liMDMDashboard" class="custom-btn-theme-c">MDM (Mobile Device Management) Dashboard</a></li>
                            <li><a id="liOfflineTransactionHistory" class="custom-btn-theme-c">Offline Sales Transaction History</a></li>
                            <li><a id="liOfflineProductHistory" class="custom-btn-theme-c">Offline Product History</a></li>
                        </ul>
                    </div>

                    <div data-role="collapsible">
                        <h3>Settings</h3>
                        <ul data-role="listview">
                            <li><a id="liDashboardSettings" class="custom-btn-theme-c">Dashboard Settings</a></li>
                            <li><a id="liFranchiseGroupSettings" class="custom-btn-theme-c">Group Settings</a></li>
                            <li><a id="liFranchiseRequests" class="custom-btn-theme-c">Franchise Requests</a></li>
                        </ul>
                    </div>
                </div>
            </div> 
        </div>

        @*PopUp OnClick FOWST Group*@

        <div data-role="popup" id="panGoTo" data-overlay-theme="a" data-theme="a" data-corners="false"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Go To</h3></center>

            <div data-role="content">
                <div style="padding:5px 0px 5px 0px;">
                    <a class="btnSalesAnalytics btn custom-btn-theme-c txt-left">
                        <div style="padding:0px 12px 0px 12px; font-weight:bold; text-shadow: 0px 0px 0px #FFF;">
                            Sales Analytics
                        </div>
                    </a>
                </div>

                <div style="padding:5px 0px 5px 0px;">
                    <a class="btnInventoryAnalytics btn custom-btn-theme-c txt-left">
                        <div style="padding:0px 12px 0px 12px; font-weight:bold; text-shadow: 0px 0px 0px #FFF;">
                            Inventory Reports and Analytics
                        </div>
                    </a>
                </div>

                @*Inventory Analytics Sub Menu*@
                        <div class="IventoryAnalyticsSubList" style="padding:5px 0px 5px 0px;display:none; margin:0px 10px 0px 10px; font-weight:normal;">
                            <a class="btnInventoryBalance btn custom-btn-theme-c txt-left">
                                <div style="padding:0px 12px 0px 12px;">
                                    Inventory Balance
                                </div>
                            </a>
                        </div>

                        <div class="IventoryAnalyticsSubList" style="padding:5px 0px 5px 0px;display:none; margin:0px 10px 0px 10px; font-weight:normal;">
                            <a class="btnInventoryLedger btn custom-btn-theme-c txt-left">
                                <div style="padding:0px 12px 0px 12px;">
                                    Inventory Ledger
                                </div>
                            </a>
                        </div>

                        <div class="IventoryAnalyticsSubList" style="padding:5px 0px 5px 0px;display:none; margin:0px 10px 0px 10px; font-weight:normal;">
                            <a class="btnStockReceivingTransfers btn custom-btn-theme-c txt-left">
                                <div style="padding:0px 12px 0px 12px;">
                                    Stock Receiving and Transfers
                                </div>
                            </a>
                        </div>

                        <div class="IventoryAnalyticsSubList" style="padding:5px 0px 5px 0px; display:none; margin:0px 10px 0px 10px; font-weight:normal;">
                            <a class="btnInventoryCounts btn custom-btn-theme-c txt-left">
                                <div style="padding:0px 12px 0px 12px;">
                                    Physical Inventory and Variance Report
                                </div>
                            </a>
                        </div>
                        <div class="IventoryAnalyticsSubList" style="padding:5px 0px 5px 0px; display:none; margin:0px 10px 0px 10px; font-weight:normal;">
                            <a class="btnInventoryLevel btn custom-btn-theme-c txt-left">
                                <div style="padding:0px 12px 0px 12px;">
                                    Stock Inventory Levels
                                </div>
                            </a>
                        </div>

                <div style="padding:5px 0px 5px 0px;">
                    <a class="btnReplenishmentProcessing btn custom-btn-theme-c txt-left">
                        <div style="padding:0px 12px 0px 12px; font-weight:bold; text-shadow: 0px 0px 0px #FFF;">
                            Store Replenishment Processing & Analytics
                        </div>
                    </a>
                </div>

                <div style="padding:5px 0px 5px 0px;">
                    <a class="btnSalesOrderProcessingAnalyticsPage btn custom-btn-theme-c txt-left">
                        <div style="padding:0px 12px 0px 12px; font-weight:bold; text-shadow: 0px 0px 0px #FFF;">
                            Sales Order Processing & Analytics
                        </div>
                    </a>
                </div>
            </div> 
        </div>

        @*PopUp Search Filter by Date-Range*@

        <div data-role="popup" id="panSearchFilter" data-overlay-theme="a" data-theme="a" data-corners="false"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Filter Search by Date-Range</h3></center>
            <div data-role="content">
                You Search for: <input type="text" />
                <div class="ui-grid-a ui-responsive">
                    <div class="ui-block-a" style="padding:7px;">
                        <div class="metro" style="max-width:252px; overflow:auto;">
                            From: <span id="SearchFiltertxtDateFrom"></span>
                            <div class="SearchFilterFromCalendar calendar"></div>
                        </div>
                    </div>
                    <div class="ui-block-b" style="padding:7px;">
                        <div class="metro" style="max-width:252px; overflow:auto;">
                            To: <span id="SearchFiltertxtDateTo"></span>
                            <div class="SearchFilterToCalendar calendar"></div>
                        </div>
                    </div>
                </div>
                <br />
                <div class="ui-grid-a ui-responsive">
                    <div class="ui-block-a" style="padding:0px 7px 7px 7px;">
                        <a id="SearchFilterbtnCancel" class="btn custom-btn-theme-c">
                            Cancel
                        </a>
                    </div>
                    <div class="ui-block-b" style="padding:0px 7px 7px 7px;">
                        <a id="SearchFilterbtnDone" class="btn custom-btn-theme-g">
                            Done
                        </a>
                    </div>
                </div>
            </div>       
        </div>

        @*PopUp List Of Fowst Group*@

        <div data-role="popup" id="panListOfFowstGroup" data-overlay-theme="a" data-theme="a" data-corners="false"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>View By</h3></center>
            <div data-role="content" style="width:200px;">
                Select a Fowst:
                <select id="selListOfFowstGroup" style="font-size:20px;">
                </select>
                <br />
                <button data-role="button" id="btnGoViewInventoryBalance">Go</button>
            </div>       
        </div>

        @*PopUp List Of Fowst Group*@

        <div data-role="popup" id="panHeaderListOfGroupFowst" data-overlay-theme="a" data-theme="a" data-corners="false"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Select a Fowst</h3></center>
            <div data-role="content">
                <div class="FowstGroupPopOp" style="padding: 8px 15px 16px 15px; max-height:200px; overflow:auto;">
                    <ul id="selHeaderListOfFowstGroup" data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm">
                        <li><a class="btn custom-btn-theme-c txt-left">Please wait ...</a></li>
                    </ul>
                </div>
            </div>       
        </div>

        @*PopUp Receiving Transfer Date Range*@

        <div data-role="popup" id="panReceivingTransferDateRange" data-overlay-theme="a" data-theme="a" data-corners="false"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Date Range</h3></center>
            <div data-role="content">
                <span id="spanDateRangeError" style="display:none; color:#F00;">Please select a valid date range. <br /></span>
                <div class="ui-grid-a ui-responsive">
                    <div class="ui-block-a" style="padding:7px;">
                        <div class="metro" style="max-width:252px; overflow:auto;">
                            From: <span id="txtReceivingTransferDateFrom"></span>
                            <div class="ReceivingTransferfromCalendar calendar"></div>
                        </div>
                    </div>
                    <div class="ui-block-b" style="padding:7px;">
                        <div class="metro" style="max-width:252px; overflow:auto;">
                            To: <span id="txtReceivingTransferDateTo"></span>
                            <div class="ReceivingTransfertoCalendar calendar"></div>
                        </div>
                    </div>
                </div>
                <br />
                <div class="ui-grid-a ui-responsive">
                    <div class="ui-block-a" style="padding:0px 7px 7px 7px;">
                        <a id="btnReceivingTransferCancel" class="btn custom-btn-theme-c">
                            Cancel
                        </a>
                    </div>
                    <div class="ui-block-b" style="padding:0px 7px 7px 7px;">
                        <a id="btnReceivingTransferDone" class="btn custom-btn-theme-g">
                            Done
                        </a>
                    </div>
                </div>
            </div>       
        </div>        

        @*PopUp Receiving Transfer Third Party Organization*@

        <div data-role="popup" id="panReceivingTransferThirdParty" data-overlay-theme="a" data-theme="a" data-corners="false"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Organization</h3></center>
            <div data-role="content" style="max-width:350px;">
                <div id="btnBackToReceivingTransferFowst" style="padding: 8px 15px 16px 15px; display:block; cursor:pointer; text-transform:capitalize;">
                    <span class="ui-icon ui-icon-arrow-l" style="display:inline-block; margin-left:-12px;">
                        <span style="margin-left:25px;"> Back </span>
                    </span>
                </div>
                <input type="search" id="txtSearchReceivingTransferOrg" />
                <div style="padding: 8px 15px 16px 15px; max-height:200px; overflow:auto;">
                    
                    <ul data-role="listview" id="lstReceivingTransferOrganization" class="li-theme-c noBorderList custom-list custom-list-sm">
                        <li>
                            <a href="#" class="btn custom-btn-theme-c txt-left" style="box-shadow:none !important; font-weight:bold !important; width:auto;">
                                Please wait while getting data ...
                            </a>
                        </li>
                    </ul>    
                </div>                
            </div>       
        </div>

        @*PopUp Receiving And Transfers Details*@

        <div data-role="popup" id="divFortableReceivingAndTransfersDetails" data-overlay-theme="a" data-theme="a" data-corners="false">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>&nbsp;Receiving and Transfers Product Details&nbsp;</h3></center>
            <div data-role="content">
                <div class="row ui-responsive">
                    <div class="col-sm-6" id="divReceivingAndTransfersDetails1">
                        <div class="ui-bar">
                            <h3 id="lblReceivingAndTransFersSummaryFrom"> </h3>
                            <br /> <br />
                            <table id="tableReceivingAndTransfersDetails1" data-role="table" style="display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                                <thead>
                                    <tr>
                                        <th>Product</th>
                                        <th>Unit Measure</th>
                                        <th>Quantity</th>
                                    </tr>
                                </thead>

                                <tbody id="tBodyReceivingAndTransfersDetails1"></tbody>
                            </table>

                        </div>
                    </div>
                    <div class="col-sm-6" id="divReceivingAndTransfersDetails2">
                        <div class="ui-bar">
                            <h3 id="lblReceivingAndTransFersSummaryTo"> </h3>
                            <br /> <br />
                            <table id="tableReceivingAndTransfersDetails2" data-role="table" style="display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                                <thead>
                                    <tr>
                                        <th>Product</th>
                                        <th>Unit Measure</th>
                                        <th>Quantity</th>
                                    </tr>
                                </thead>

                                <tbody id="tBodyReceivingAndTransfersDetails2"></tbody>
                            </table>
                        </div>
                    </div>
                </div>
                
            </div>
        </div>        

        @*PopUp Inventory Ledger Date*@

        <div data-role="popup" id="panInventoryLedgerDate" data-overlay-theme="a" data-theme="a" data-corners="false"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Date Range</h3></center>
            <div data-role="content">
                <span id="spanILDateRangeError" style="display:none; color:#F00;">Please select a valid date range. <br /></span>
                <div class="ui-grid-a ui-responsive">
                    <div class="ui-block-a" style="padding:7px;">
                        <div class="metro" style="max-width:252px; overflow:auto;">
                            From: <span id="txtInventoryLedgerDateFrom"></span>
                            <div class="InventoryLedgerfromCalendar calendar"></div>
                        </div>
                    </div>
                    <div class="ui-block-b" style="padding:7px;">
                        <div class="metro" style="max-width:252px; overflow:auto;">
                            To: <span id="txtInventoryLedgerDateTo"></span>
                            <div class="InventoryLedgertoCalendar calendar"></div>
                        </div>
                    </div>
                </div>
                <br />
                <div class="ui-grid-a ui-responsive">
                    <div class="ui-block-a" style="padding:0px 7px 7px 7px;">
                        <a id="btnInventoryLedgerCancel" class="btn custom-btn-theme-c">
                            Cancel
                        </a>
                    </div>
                    <div class="ui-block-b" style="padding:0px 7px 7px 7px;">
                        <a id="btnInventoryLedgerDone" class="btn custom-btn-theme-g">
                            Done
                        </a>
                    </div>
                </div>
            </div>       
        </div>

        <div data-role="popup" id="panReportTypeOption" data-overlay-theme="a" data-theme="a" data-corners="false"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right btnPopupClose">Close</a>
            <center><h3>Report Type</h3></center>
            <div data-role="content" style="max-width:350px; word-wrap: break-word;">
                <div style="padding: 8px 15px 0px 15px; overflow:auto;">
                    <ul id="lstReportType" data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm">
                        <li>
                            <a id="btnReportGroupSales">Group Sales</a>
                        </li>
                        <li>
                            <a id="btnReportGroupSalesInventory">Group Inventory Levels</a>
                        </li>
                        <li>
                            <a id="btnReportMoniesAccountability">Monies Accountability</a>
                        </li>
                        <li>
                            <a id="btnReportInventoriesAccountability">Inventories Accountability</a>
                        </li>
                    </ul>
                    <br />
                </div>
            </div>       
        </div>
    </div>
    
    <div id="divMainContentForDashboard" data-role="content" class="mainContent mainContentForDashboard">

    @*START Main Content Dashboard*@

    <div id="divMainDashboard" subpage="dashboard" style="display:none;">
        <div class="ui-grid-a ui-responsive">
            <div class="ui-block-a" style="padding-right:9px; overflow:hidden; display:flex;">
                <button style="margin-right:10px;" data-role="none" class="btn btn-less-width btn-transparent btnNotRequired btnPeriod">
                    <span class="lblPeriod"> Today </span>
                    <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span>
                </button>
            </div>
            <div class="ui-block-b txt-right" style="padding:0px 16px 0px 20px;">
                <div style="margin-bottom:-10px; visibility:visible; display:block;">
                    <button data-role="none" id="btnMCRMoreOptionForEAS" style="display:none;" class="btn custom-btn-theme-c btn-less-width btn-sm btnMoreReport">Go To</button>
                    @*<input type="search" placeholder="Search invoice no., product description, sales representative or customer" class="txtSearch" />*@                   
                </div>
                <br />
            </div>
        </div>
        <br />
        <div class="ui-grid-a ui-responsive" style="margin-top:-19px;">
            <div class="ui-block-a thirtyseventy-block-a" id="divAllSalesReport" style="padding-right:10px; padding-top:10px; display:none;">
                <a class="btn custom-btn-theme-b" id="btnOrgGroupTotal" style="padding:0px;">
                    <div style="padding:23px; font-weight:normal; text-shadow:0; font-size:30px; margin:0px auto; text-align:left">
                        <div style="font-weight:bold; font-size:32px; text-align:left; text-overflow:ellipsis; white-space:nowrap; overflow:hidden; padding:0px;" id="OrganizationName">Organization</div>
                        <hr style="border:1px solid #297E6D" />

                        <div class="row">
                            <div class="col-sm-6">
                                <div style="text-align:left; font-size:27px; font-weight:bold">Net Sales</div>
                            </div>
                            <div class="col-sm-6">
                                <div style="text-align:right; font-size:27px; font-weight:bold" id="TotalNetSales">-</div>
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
                    <div class="nothingdiv" style="height:135px;"></div>
                </a>
            </div>

            <div class="ui-block-b thirtyseventy-block-b" id="divGroupSalesReport" style="display:none;">
                <div class="MainFrame" id="MainFrame" style="overflow:auto; height:622px;">
                    <div class="ui-grid-b ui-responsive">
                        <div class="ui-block-a" style="padding-right:10px; padding-top:10px;">
                            <div style="height:301px; width:auto; border: 3px dashed #CCC;"></div>
                        </div>
                        <div class="ui-block-b" style="padding-right:10px; padding-top:10px;">
                            <div style="height:301px; width:auto; border: 3px dashed #CCC;"></div>
                        </div>
                        <div class="ui-block-c" style="padding-right:10px; padding-top:10px;">
                            <div style="height:301px; width:auto; border: 3px dashed #CCC;"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </div>



    @*START Sales Analytics Page*@

    <div id="divSalesAnalytics" subpage="dashboard" style="display:none;">

        <div class="ui-grid-solo ui-responsive">
            <div class="ui-block-a" style="padding-right:9px; overflow:hidden; display:flex;">
                <button style="margin-right:10px;" data-role="none" class="btn btn-less-width btn-transparent btnNotRequired btnPeriod">
                    <span class="lblPeriod"> Today </span>
                    <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span>
                </button>

                <button id="btnReportType" style="margin-right:10px;" data-role="none" class="btn btn-less-width btn-transparent btnNotRequired">
                    <span id="txtReportType"> Group Sales </span>
                    <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span>
                </button>
            </div>
            @*<div class="ui-block-b txt-right" style="padding:0px 16px 0px 20px;">
                    <div style="margin-bottom:-10px; visibility:visible; display:block;">
                        <div class="ui-grid-solo">
                            <div class="ui-block-a" style="float:right;">
                                <input type="search" placeholder="Search invoice no., product description, sales representative or customer" class="txtSearch" />
                            </div>
                            <div class="ui-block-b" style="padding:8px; width: 20%; visibility:hidden; display:none;">
                                <a class="btn custom-btn-theme-a btnSearch">
                                    <span class="ui-icon ui-icon-search" style="display:inline-block;"></span>
                                    Search
                                </a>
                            </div>
                        </div>
                        <div class="txtSearchStatus" style="display:none; float:left; color:#FF5959; text-shadow:0px 0px 0px #FFF;">
                            Search box is required!
                        </div>
                    </div>
                    <br />
                </div>*@
        </div>
        <br />

        <div id="divGroupSales">
            <div style="box-shadow:0px 0px 10px #CCC; background-color:#F9F9F9; padding:5px;">
                <div class="ui-grid-solo">
                    <div class="ui-block-solo">
                        <h1>
                            <label id="lblGroupName">
                            </label>
                        </h1>
                    </div>
                </div>

                <div class="ui-grid-b ui-responsive">
                    <div class="ui-block-a" style="padding-right:10px; padding-top:10px;">
                        <div class="custom-theme-b">
                            <div style="padding:15px;font-weight:normal; font-size:20px; margin:0px auto; min-height:100px" id="NetSalesHeight">
                                <div style="font-weight:bold; font-size:20px; text-align:center;">GROUP NET SALES</div>
                                <hr style="border:1px solid #297E6D" />

                                <div style="text-align:center; word-break:break-all;" class="SalesFontSize">
                                    <label id="lblGroupNetSales"></label>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="ui-block-b" style="padding-right:10px; padding-top:10px;">
                        <div class="custom-theme-c">
                            <div style="padding:15px;font-weight:normal; font-size:20px; margin:0px auto; min-height:100px;" id="GrossSalesHeight">
                                <div style="font-weight:bold; font-size:20px; text-align:center;">GROUP GROSS SALES</div>
                                <hr style="border:1px solid #CCC" />

                                <div style="text-align:center; word-break:break-all;" class="SalesFontSize">
                                    <label id="lblGroupGrossSales"></label>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="ui-block-c" style="padding-right:10px; padding-top:10px;">
                        <div class="custom-theme-c">
                            <div style="padding:15px;font-weight:normal; font-size:20px; margin:0px auto; height:auto" id="ExtrasTotalHeight">
                                <div style="text-align:left; font-size:13px">Group Discounts</div>
                                <div style="text-align:right; font-size:15px;"><label id="lblGroupDiscounts"></label></div>

                                <div style="text-align:left; font-size:13px">Vat Exempt Tax</div>
                                <div style="text-align:right; font-size:15px;"><label id="lblGroupForegoneVAT"></label></div>

                                <div style="text-align:left; font-size:13px">Group Refunds</div>
                                <div style="text-align:right; font-size:15px;"><label id="lblGroupRefunds"></label></div>

                                <div style="text-align:left; font-size:13px">Group Exchanges</div>
                                <div style="text-align:right; font-size:15px;"><label id="lblGroupExchanges"></label></div>
                            </div>
                        </div>
                    </div>
                </div>
                <br />
                <div data-role="fieldcontain">
                    <fieldset data-type="horizontal" data-role="controlgroup">
                        <legend>Display these FOWST locations:</legend>
                        <input type="checkbox" name="filterGroupSales" id="chbGroupSalesCompanyOwned" value="Company-Owned" checked />
                        <label for="chbGroupSalesCompanyOwned">Company-Owned</label>
                        <input type="checkbox" name="filterGroupSales" id="chbGroupSalesFranchised" value="Franchised" checked />
                        <label for="chbGroupSalesFranchised">Franchised</label>
                        <input type="checkbox" name="filterGroupSales" id="chbGroupSalesCoOwned" value="Co-Owned" checked />
                        <label for="chbGroupSalesCoOwned">Co-Owned</label>
                        <input type="checkbox" name="filterGroupSales" id="chbGroupSalesUnidentified" value="NULL" checked />
                        <label for="chbGroupSalesUnidentified">Unidentified</label>
                    </fieldset>
                </div>
                <br />
                <div class="ui-grid-solo">
                    <div class="ui-block-a" style="overflow:auto">
                        <table data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-responsive table-stroke ui-table ui-table-reflow custom-tbl-responsive">
                            <thead>
                                <tr class="custom-theme-f">
                                    <th data-sort="string" style="cursor:pointer;">FOWST</th>
                                    <th data-sort="string" style="cursor:pointer;">Ownership Type</th>
                                    <th data-sort="string" style="cursor:pointer;">Geographical Region</th>
                                    <th data-sort="string" style="cursor:pointer;">Owner</th>
                                    <th data-sort="string" style="cursor:pointer;">Organization Name</th>
                                    <th data-sort="accounting" style="text-align:center;cursor:pointer;">Net Sales</th>
                                    <th data-sort="percentage" style="text-align:center;cursor:pointer;">Net Sales Contribution(%)</th>
                                    <th data-sort="accounting" style="text-align:center;cursor:pointer;">Gross Sales</th>
                                    <th data-sort="accounting" style="text-align:center;cursor:pointer;">Gross Sales Contribution(%)</th>
                                    <th data-sort="accounting" style="text-align:center;cursor:pointer;">Discounts</th>
                                    <th data-sort="percentage" style="text-align:center;cursor:pointer;">Discounts Contribution(%)</th>
                                    <th data-sort="accounting" style="text-align:center;cursor:pointer;">Vat Exempt Tax</th>
                                    <th data-sort="percentage" style="text-align:center;cursor:pointer;">Vat Exempt Tax Contribution(%)</th>
                                    <th data-sort="accounting" style="text-align:center;cursor:pointer;">Refunds</th>
                                    <th data-sort="percentage" style="text-align:center;cursor:pointer;">Refunds Contribution(%)</th>
                                    <th data-sort="accounting" style="text-align:center;cursor:pointer;">Exchanges</th>
                                    <th data-sort="percentage" style="text-align:center;cursor:pointer;">Exchanges Contribution(%)</th>
                                </tr>
                            </thead>
                            <tbody id="reportTable"></tbody>
                        </table>
                    </div>
                    <br /> <br />
                </div>
            </div>

            <br />
            <div id="dvSalesTrends" style="visibility:hidden; display:none; box-shadow:0px 0px 10px #CCC; background-color:#EFEFEF; padding:5px;">
                <div class="ui-grid-solo">
                    <div class="ui-block-solo">
                        <h1>
                            <label id="lblSalesTrend">
                                Sales Trend
                            </label>
                        </h1>
                    </div>
                </div>

                <div class="panel">
                    <div class="panel-title custom-theme-f" id="dvSalesTrendFOWST">
                        Trend Line
                    </div>

                    <div class="panel-content txt-center">
                        <label id="IsDataPresent"> -Graph will be shown when you click on a group or FOWST (Factory, Office, Store, Warehouse or Truck)  name- </label>

                        <div id="dvMainChart">
                            <table style="text-align:center; width:100%">
                                <tr>
                                    <td>  </td>
                                </tr>
                            </table>
                        </div>

                        <div>
                            <br />
                            <label id="lblMainChartInstruction" style="color:Red; visibility: hidden; font-size:small"> * Click the chart to breakdown data</label>
                        </div>

                        <br />
                        <div>
                            <a href="#" id="lnkTransactionDetails" style="visibility:hidden">  Click here to view transaction details </a>
                        </div>

                        <br />
                        <br />
                        <table id="tblMainChart" data-role="table" data-mode="reflow" style="display:table;" class="ui-table ui-responsive table-stroke table-stripe"> </table>

                        <br />
                        <table style="text-align:center; width:100%; visibility:hidden" id="tblDateFormatLabel">
                            <tr>
                                <td> <label id="lblDateFormatMessage"></label>   </td>
                            </tr>
                        </table>
                    </div>
                </div>
                @*</div>*@

                <div id="dvSlice" style="display:none;" class="panel">
                    <div class="panel-title custom-theme-f">
                        <a id="liSecondChartMainLabel"></a>
                        <a href="#" id="liSecondChart"></a>
                    </div>

                    <div class="panel-content">
                        <div id="dvSecondChart"></div>
                        <div>
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
                            <br />
                            <label id="lblSecondChartInstruction" style="color:Red; font-size:small"> </label>
                            <br />

                            <br />
                            <table id="tblSecondChart" data-role="table" data-mode="reflow" style="display:table;" class="ui-table ui-responsive table-stroke table-stripe"> </table>
                        </div>
                    </div>
                </div>

                <br />
                <br />
                <div id="dvDice" style="display:none;" class="panel">
                    <div class="panel-title custom-theme-f">
                        <a id="liThirdChartMainLabel"></a>
                        <a href="#" id="liThirdChartLabel"></a>
                    </div>

                    <div class="panel-content">

                        <div id="dvThirdChart">
                        </div>

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

                        <br />

                        <table data-role="table" id="tblThirdChart" style="display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive"></table>
                    </div>
                </div>

                <div class="panel">
                    <div class="panel-title custom-theme-f">
                        Receipt Printed Status
                    </div>
                    <table id="tableReceiptPrintedStatus" @*data-role="table"*@ @*data-mode="reflow" style="display:table;"*@ class="ui-table ui-responsive table-stroke table-stripe">
                        <tr>
                            <th>Receipt Status</th>
                            <th>Count</th>
                            <th>Amount</th>
                        </tr>
                        <tr name="data">
                            <td name="Status"></td>
                            <td name="Count" style="text-align:right"></td>
                            <td name="NetInvoiceAmount" style="text-align:right"></td>
                        </tr>
                        <tr name="footer">
                            <td name="Status"></td>
                            <td name="Count" style="text-align:right"></td>
                            <td name="NetInvoiceAmount" style="text-align:right"></td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>

        <div data-role="popup" id="panSalesDistribution" data-overlay-theme="a" data-theme="a" data-corners="false">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Sales Distribution</h3></center>

            <div data-role="content">
                <div class="SalesDistributionPopUp" style="padding:10px;">
                    <ul data-role="listview" class="noBorderList" id="lstSlice"></ul>
                </div>
            </div>
        </div>

        <div data-role="popup" id="panDice" data-overlay-theme="a" data-theme="a" data-corners="false">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Sales Distribution</h3></center>

            <div data-role="content">
                <div class="SalesDistributionPopUp" style="padding:10px;">
                    <ul data-role="listview" class="noBorderList" id="lstDice"></ul>
                </div>
            </div>
        </div>

        @*<div data-role="popup" id="panSalesTrends" data-overlay-theme="a" data-theme="a" data-corners="false">*@

        <div id="divInventoryLevels" style="display:none; box-shadow:0px 0px 10px #CCC; background-color:#F9F9F9; padding:5px;">
            <div data-role="fieldcontain" style="float:left">
                <fieldset data-type="horizontal" data-role="controlgroup">
                    <legend>Display these FOWST locations:</legend>
                    <input type="checkbox" name="filterInventoryLvl" id="chbInventoryLvlCompanyOwned" checked />
                    <label for="chbInventoryLvlCompanyOwned">Company-Owned</label>
                    <input type="checkbox" name="filterInventoryLvl" id="chbInventoryLvlFranchised" checked />
                    <label for="chbInventoryLvlFranchised">Franchised</label>
                    <input type="checkbox" name="filterInventoryLvl" id="chbInventoryLvlCoOwned" checked />
                    <label for="chbInventoryLvlCoOwned">Co-Owned</label>
                    <input type="checkbox" name="filterInventoryLvl" id="chbInventoryLvlUnidentified" value="NULL" checked />
                    <label for="chbInventoryLvlUnidentified">Unidentified</label>
                </fieldset>
            </div>
            <div id="divGroupInventoryLevelsButtons" style="display:block; float:right">
                <button id="butGroupInventoryLevelsRecache" data-role="none" class="btn custom-btn-theme-b btn-less-width">
                    Get latest data
                </button>
            </div>
            <br />
            @* bookmark: arvin, GroupInventoryLevels *@
            <div id="divGroupInventoryLevelsResult" style="clear:both"></div>
        </div>

        <div id="divMoniesAcc" style="display:none; box-shadow:0px 0px 10px #CCC; background-color:#F9F9F9; padding:5px;">

        </div>

        <div id="divInventoriesAcc" style="display:none; box-shadow:0px 0px 10px #CCC; background-color:#F9F9F9; padding:5px;">

        </div>
    </div>

    <div data-role="popup" id="popInventoryLevelApproveDisapproveOption" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3>Select Action</h3></center>
        <div data-role="content" class="ui-content ui-body-d">
            <div style="padding: 8px 15px 16px 15px; overflow:auto;">
                <ul data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm">
                    <li id="liViewSetInventoryBalance" class="btnLIListofFowstGroup">
                        <a href="#" class="btn custom-btn-theme-c txt-left" style="box-shadow:none !important; font-weight:bold !important; width:auto;">
                            View Inventory Balance
                        </a>
                    </li>
                    <li id="liViewSetInventoryLevelApproval" class="btnLIListofFowstGroup">
                        <a href="#" class="btn custom-btn-theme-c txt-left" style="box-shadow:none !important; font-weight:bold !important; width:auto;">
                            Inventory Count Approval/Disapproval
                        </a>
                    </li>
                </ul>
            </div>
        </div>
    </div>

    <div class="popup" id="popInventoryLevelApproveDisapprove" custom-data-role="popup">
        <div class="popup-header">
            Inventory Count Approval/Disapproval
            <a class="popup-close btnCloseILAP">x</a>
        </div>

        <div class="popup-content">
            <div class="custom-tab">
                <ul>
                    <li class="active" custom-tab-content="div[id='divILADTransaction']">Inventory Count Transaction</li>
                    <li class="" id="btnTabILADHistory" custom-tab-content="div[id='divILADHistory']">Inventory Count History</li>
                </ul>
            </div>
            <div custom-tab-role="content" id="divILADTransaction">
                <br />
                <div id="divILAppDisHeader" style="display:block;">
                    <button id="btnRefreshILAP" style="margin-right: 10px; display: block;" data-role="none" class="btn btn-less-width custom-btn-theme-c">
                        Refresh
                    </button>
                    <h4><b id="txtInventoryLevelApproveDisapproveFowstName"></b></h4>
                    <div style="overflow:auto;">
                        <table data-role="table" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                            <thead>
                                <tr class="custom-theme-f">
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
                    <button id="btnBackILAP" style="margin-right: 10px; display: block;" data-role="none" class="btn btn-less-width custom-btn-theme-c">
                        Back
                    </button>
                    <h4><b id="txtInventoryLevelApproveDisapproveDate"></b></h4>
                    <div style="overflow:auto;">
                        <table data-role="table" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                            <thead>
                                <tr class="custom-theme-f">
                                    <th style="text-align:center;"><label style="position:relative;right:-0.5em;"><input type="checkbox" data-iconpos="notext" name="checkbox-0" id="btnCheckAll" /></label></th>
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
                        <br />
                    </div>
                </div>
            </div>

            <div custom-tab-role="content" id="divILADHistory">
                <br />
                <button id="btnRefreshILAPHistory" style="margin-right: 10px; display: block;" data-role="none" class="btn btn-less-width custom-btn-theme-c">
                    Refresh
                </button>

                <h4><b><span style="color:#F00">*</span> History of all approved/disapproved item(s) of <span id="txtILADHistoryFowstName"></span></b></h4>
                <table data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe ui-table-reflow custom-tbl-responsive">
                    <thead>
                        <tr id="tblILADHistoryHeader" class="custom-theme-f">
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

        <div class="popup-footer align-right">
            <button data-role="none" class="btn custom-btn-theme-c btn-less-width btnCloseILAP">Close</button>
            <span id="divbtnILAD" style="display:none;">
                <button data-role="none" id="btnILADChbDisapp" class="btn custom-btn-theme-d btn-less-width">Disapprove</button>
                <button data-role="none" id="btnILADChbApp" class="btn custom-btn-theme-b btn-less-width">Approve</button>
            </span>
        </div>
    </div>
    @*START Store Replenishment Processing & Analytics*@

    <div id="divSalesReplenishmentProcessing" subpage="dashboard" style="display:none;">
    </div>
    <div id="divSalesOrderProcessing" subpage="dashboard" style="display:none;">
    </div>
    <div id="divOnPrintPOSSalesReport" subpage="dashboard" style="display:none;">
    </div>
    <div id="divFranchiseRequest" subpage="dashboard" style="display:none;">
    </div>

    <div id="panTransactionDetails" custom-data-role="popup">
        <div class="popup">
            <div class="popup-header">
                POS Sales Transactions <a class="popup-close">x</a>
            </div>
            <div class="popup-content">
                <div style="float:left; font-size:medium; margin-top:10px">                 
                    Date: <label style="font-weight:bolder; font-size:large" id="lblSalesTransactionDate"> </label> &nbsp; &nbsp; &nbsp; 
                    FOWST: <label style="font-weight:bolder; font-size:large" id="lblFOWST"> </label> 
                </div>
                <div style="float:right;">
                    <label for="chbTransViewOpt">
                        Show All SKU Details
                        <input type="checkbox" name="chbTransViewOpt" id="chbTransViewOpt" />
                    </label>
                </div>
                <div style="clear:both"></div>

                <div style="font-size:medium; margin-top:10px">
                    <div data-role="fieldcontain">
                        <label for="selCashier">Cashier:  </label>
                        <select id="selCashier">
                            <option> All Cashier </option>
                        </select>
                    </div>                           
                </div>

                <div style="font-size:medium; margin-top:10px">
                    <div>
                        Total Gross Sales: <label style="font-weight:bolder; font-size:large" id="lblTotalGrossSales"> 0.00 </label> | &nbsp; &nbsp; &nbsp;
                        Total Deductive Adjustments: <label style="font-weight:bolder; font-size:large" id="lblTotalDeductiveAdjustments"> 0.00 </label> | &nbsp; &nbsp; &nbsp;
                        Total Additive Adjustments: <label style="font-weight:bolder; font-size:large" id="lblTotalAdditiveAdjustments"> 0.00 </label> | &nbsp; &nbsp; &nbsp;
                        Net Sales: <label style="font-weight:bolder; font-size:large" id="lblNetSales"> 0.00 </label> 
                    </div>
                    <br />
                    <div>
                        Total Cash: <label style="font-weight:bolder; font-size:large" id="lblTotalCash"> 0.00 </label> | &nbsp; &nbsp; &nbsp;
                        Total Check: <label style="font-weight:bolder; font-size:large" id="lblTotalCheck"> 0.00 </label> | &nbsp; &nbsp; &nbsp;
                        Total Debit Card: <label style="font-weight:bolder; font-size:large" id="lblTotalDebitCard"> 0.00 </label> | &nbsp; &nbsp; &nbsp;
                        Total Credit Card: <label style="font-weight:bolder; font-size:large" id="lblCreditCard"> 0.00 </label> | &nbsp; &nbsp; &nbsp;
                        Total SRV: <label style="font-weight:bolder; font-size:large" id="lblTotalSRV"> 0.00 </label> | &nbsp; &nbsp; &nbsp;
                        Total On-Credit: <label style="font-weight:bolder; font-size:large" id="lblTotalOnCredit"> 0.00 </label> | &nbsp; &nbsp; &nbsp;
                        Total Other Payments: <label style="font-weight:bolder; font-size:large" id="lblTotalOtherPayments"> 0.00 </label>
                    </div>

                    <br />
                    <div id="dvSalesByCashier">
                        Cashiers: <label style="font-weight:bolder; font-size:large" id="lblSalesByCashier"> 0.00 </label>                         
                    </div>

                    <br />

                </div>

                <div class="ui-grid-solo" id="divTransHeader" style="display:block;">
                    <input id="txtSearchHeader" placeholder="Search invoice no." type="search" />
                    <div class="ui-block-a" style="overflow:auto;">
                        <table data-role="table" id="tblTransactionHeader" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                            <thead>
                                <tr class="custom-theme-f">
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
                        <br />
                    </div>
                </div>

                <div class="ui-grid-solo" id="divTransDetails" style="display:none;">
                    <input id="txtSearchProductTransDetails" placeholder="Search invoice no., product description, sales representative or customer" type="search" />
                    <div class="ui-block-a" style="overflow:auto;">
                        <table data-role="table" id="tblTransactionDetails" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                            <thead>
                                <tr class="custom-theme-f">
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
                        <br />
                    </div>
                </div>

                <div class="popup" id="popTransDetails" custom-data-role="popup">
                    <div class="popup-header">
                        Invoice Details
                        <a class="popup-close">x</a>
                    </div>

                    <div class="popup-content">
                        <div class="ui-grid-solo">
                            <div class="custom-ui-label">
                                <div>Invoice Date and Time: </div>
                                <div id="txtInvoiceDate" style="font-weight:bold;"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>Invoice Number: </div>
                                <div id="txtInvoiceNumber" style="font-weight:bold;"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>Sales Representative: </div>
                                <div id="txtSalesRep" style="font-weight:bold;"></div>
                            </div>
                            <br />
                            @*<input id="txtSearchProduct" placeholder="Search invoice no., product description, sales representative or customer" type="search" />*@
                            <div class="ui-block-a" id="divTransactionDetails" style="overflow:auto;">
                                <table data-role="table" id="tblTransactionDetailsBody" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                                    <thead>
                                        <tr class="custom-theme-f">
                                            @*<th data-sort="int" style="cursor:pointer;"> Invoice No. </th>
                                                <th data-sort="date" style="cursor:pointer;"> Invoice Date </th>*@
                                            <th data-sort="string" style="cursor:pointer;cursor:pointer;"> Product Description </th>
                                            <th data-sort="accounting" style="text-align: center;cursor:pointer;"> Qty </th>
                                            <th data-sort="accounting" style="text-align: center;cursor:pointer;"> Qty Returned </th>
                                            <th data-sort="string" style="cursor:pointer;cursor:pointer;"> Unit Measure </th>
                                            <th data-sort="string" style="cursor:pointer;cursor:pointer;"> Returned Product For Rebate</th>
                                            <th data-sort="accounting" style="cursor:pointer;cursor:pointer;"> Rebate Qty </th>
                                            <th data-sort="string" style="cursor:pointer;cursor:pointer;"> Rebate Unit Measure</th>
                                            <th data-sort="accounting" style="text-align: right;cursor:pointer;"> Amount </th>
                                            <th data-sort="accounting" style="text-align: right;cursor:pointer;"> Sub Total </th>
                                            <th data-sort="accounting" style="text-align: right;cursor:pointer;"> Discount </th>
                                            @*<th data-sort="string" style="cursor:pointer;"> Sales Representative(s) </th>*@
                                            <th data-sort="string" style="cursor:pointer;"> Customer </th>
                                            <th data-sort="string" style="cursor:pointer;"> Last Updated By </th>
                                        </tr>
                                    </thead>
                                    <tbody id="tblTransactionDetailsBody"></tbody>
                                </table>

                                <br /> <br />

                                <div id="dvSeniorCitizen" style="visibility:hidden; display:none">
                                    <h1>Senior Citizen/PWD Information</h1>
                                    <table data-role="table" id="tblSeniorCitizen" style="margin-right:5px; display:table; text-align:center" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                                        <thead>
                                            <tr>
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
                        </div>
                    </div>

                    <div class="popup-footer align-right">
                        <button data-role="none" class="btn custom-btn-theme-c btn-less-width popup-close">Close</button>
                    </div>
                </div>
                @*
                        </li>
                    </ul>*@
                <b>POS Sales Returns</b><br /><br />
                @*<ul data-role="listview" data-inset="true">
                    <li data-role="list-divider">POS Sales Returns</li>
                    <li> *@
                <div class="ui-grid-solo">
                    <div class="ui-block-a">
                        <table data-role="table" style="margin-right:5px; display:table;" id="tblSalesReturnDetails" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                            <thead>
                                <tr class="custom-theme-f">
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
                    <br /> <br />
                </div>

                @*
                        </li>
                    </ul>*@
            </div>
            <div class="popup-footer align-right">
                <button id="btnCancelContact" data-role="none" class="btn custom-btn-theme-c btnCircle popup-close">
                    <img alt="Options" src="/Content/images/close-black.png" />
                </button>
            </div>
        </div>
    </div>

    <div id="divCacMcr" subpage="dashboard" style="display:none;">

    </div>

    @*START Inventory's Page*@

    <div id="divInventoryBalancePage" subpage="dashboard" style="display:none;">
        <table data-mode="reflow" class="ui-table ui-responsive table-stroke table-stripe">
            <thead class="custom-theme-f">
            <th> # </th>
            <th> Product No. </th>
            <th> Product Description </th>
            <th> Balance </th>
            <th> Unit Measure </th>
            <th> Last Movement Date </th>
            @*<th> Average Inventory </th>*@
            </thead>

            <tbody id="tblInventoryBalances"></tbody>
        </table>
    </div>

    <div id="divInventoryProductBreakdown" subpage="dashboard" style="display:none;">
        <table data-mode="reflow" class="ui-table ui-responsive table-stroke table-stripe">
            <thead class="custom-theme-f">
            <th> # </th>
            <th> Date and Time </th>
            <th> Transaction </th>
            <th> Qty </th>
            <th> Unit Measure </th>
            <th> Balance </th>
            </thead>

            <tbody id="tblInventoryProductBreakdown"></tbody>
        </table>
    </div>

</div>
  
    <br /> <br /> <br />
