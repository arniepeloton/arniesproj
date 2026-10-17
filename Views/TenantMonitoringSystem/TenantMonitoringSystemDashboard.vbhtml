@Code
    ViewData("Title") = "Tenant Monitoring Dashboard"
    Layout = "~/Views/Shared/plain.vbhtml"
End Code

@Section head
    <link rel="stylesheet" href="/Content/css/jquery.jqplot.min.css" />
    <link rel="stylesheet" href="/Content/css/ui.dynatree.css" />
    <link rel="stylesheet" href="/Content/css/qunit.css" />
    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <link rel="stylesheet" href="/Content/metro-calendar.css" />

    <script>

        require([GetPathFormat('/Scripts/sources/TenantMonitoringSystem/TenantMonitoringSystemDashboard.js?crc=')]);
        require([GetPathFormat('/Scripts/sources/TenantMonitoringSystem/TenantMonitoringSystemCharts.js?crc=')]);
        require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/flatui.js?crc=')]);
    </script>

    <style>
        .jqplot-xaxis-hide {
            display: none;
        }
    </style>
End Section

@*Header*@
<div class="custom-theme-a position-fixed" id="divHeaderForDashboard">

    <div id="HeaderDesktopVersion">
        <div class="ui-grid-a ui-responsive">
            <div class="ui-block-a" style="padding-right:9px; overflow:hidden; display:flex;">
                <div style="float:left; margin-left:10px; margin-top:5px; cursor:pointer;">
                    @*<a class="btnBack" data-iconpos="notext" data-theme="b" data-role="button" data-icon="back" title="Back">Back</a>*@
                    <a style="margin-top:8px;" class="btnHome2" data-iconpos="notext" data-theme="b" data-role="button" data-icon="home" title="Home">Home</a>
                </div>

                <div style="float:left; margin-top:10px; margin-left:25px; margin-bottom:15px;">
                    <div class="OrganizationNameHeader" style="text-overflow:ellipsis; font-size:25px; white-space:nowrap; overflow:hidden;">
                        GMall Of Davao
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
                    @*<div class="txtSearchStatus" style="display:none; float:left; color:#FF5959; text-shadow:0px 0px 0px #FFF;">
                        Search box is required!
                    </div>*@
                </div>

                <br />

                <div style="cursor:pointer; float:right; margin-top:-5px; margin-bottom:10px;" class="btnMoreReport2">
                    <span> Go To </span>
                    <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span>
                </div>
            </div>
        </div>
    </div>

    <div id="HeaderMobileVersion">
        <div class="ui-grid-solo">
            <div class="ui-block-a OrganizationNameHeader txt-center">
                GMall Of Davao
            </div>
        </div>
        <div class="ui-grid-a">
            <div class="ui-block-a">
                <a class="btn custom-btn-theme-a btnHome2">
                    <span class="ui-icon ui-icon-home" style="display:inline-block;">
                        <p style="margin:16px 0px 0px -7px; font-size:12px;">Home</p>
                    </span>
                </a>
            </div>
          
            <div class="ui-block-b">
                <a class="btn custom-btn-theme-a btnMoreReport2">
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
        
    </div>

    <div class="BreadCrumbs">
        <div class="ItemsBreadCrumbs">
            Main Dashboard
        </div>
    </div>

    @*<div data-role="popup" id="panGoToOffline" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3>Offline Reports</h3></center>
        <div data-role="content">
            <div class="FowstGroupPopOp" style="padding: 8px 15px 16px 15px; max-height:200px; overflow:auto;">
                <ul data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm">
                    <li><a id="btnOfflineCashier">Cashiers Sales Report</a></li>
                    <li><a id="btnOfflineMonies">Monies Accountability</a></li>
                    <li><a id="btnOfflineInventories">Inventories Accountability</a></li>
                    <li><a id="btnOfflineTransaction">Transaction Details</a></li>
                    <li><a id="btnOfflineInventoryLedger">Inventory Ledger</a></li>
                </ul>
            </div>
        </div>
    </div>*@

    <div data-role="popup" id="popColumn" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3>Columns</h3></center>
        <div data-role="content">
            <div style="max-height:400px; overflow:auto;">
                <label><input type="checkbox" class="chbColumns" name="chbColumns" headno="1" checked />Tenant's Store Name</label>
                <label><input type="checkbox" class="chbColumns" name="chbColumns" headno="2" checked />Store Address</label>
                <label><input type="checkbox" class="chbColumns" name="chbColumns" headno="3" checked />Mall or Branch Code</label>
                <label><input type="checkbox" class="chbColumns" name="chbColumns" headno="4" checked />Tenant No.</label>
                <label><input type="checkbox" class="chbColumns" name="chbColumns" headno="5" checked />Class Code</label>
                <label><input type="checkbox" class="chbColumns" name="chbColumns" headno="6" checked />Trade Code</label>
                <label><input type="checkbox" class="chbColumns" name="chbColumns" headno="7" checked />Outlet No.</label>
                <label><input type="checkbox" class="chbColumns" name="chbColumns" headno="8" checked />Sales type</label>
                <label><input type="checkbox" class="chbColumns" name="chbColumns" headno="9" checked />Net Sales</label>
                <label><input type="checkbox" class="chbColumns" name="chbColumns" headno="10" checked />Net Sales Contribution</label>
                <label><input type="checkbox" class="chbColumns" name="chbColumns" headno="11" checked />Gross Sales</label>
                <label><input type="checkbox" class="chbColumns" name="chbColumns" headno="12" checked />Gross Sales Contribution</label>
                <label><input type="checkbox" class="chbColumns" name="chbColumns" headno="13" checked />Refunds</label>
                <label><input type="checkbox" class="chbColumns" name="chbColumns" headno="14" checked />Refunds Contribution</label>
                <label><input type="checkbox" class="chbColumns" name="chbColumns" headno="15" checked />Exchanges</label>
                <label><input type="checkbox" class="chbColumns" name="chbColumns" headno="16" checked />Exchanges Contribution</label>
            </div>
        </div>
    </div>

    <div class="popup" id="popChart" custom-data-role="popup">
        <div class="popup-header">            
            <a data-role="none" class="popup-close">x</a>
        </div>

        <div class="popup-content">
            <div id="divMainDistribution" style="display:block">
                <div style="text-align:left; color:#000; text-shadow:none;">
                    @*<button style="margin-right:10px;padding: 5px;" data-role="none" class="btn btn-less-width btn-transparent btnNotRequired btnPeriod">
                        <span class="lblPeriod">Today</span>
                        <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span>
                    </button>*@
                    Filter by: 
                    <select id="selDistributionOption" data-role="none" class="select sm-select custom-theme-c">
                        <option value="Class Code">Class Code</option>
                        <option value="Trade Code">Trade Code</option>
                        <option value="Sales Type">Sales Type</option>
                    </select>
                </div>
                <div id="divDistributionChart" style="text-align:center; color:#000; text-shadow:none;">
                
                </div>
            </div>
            <div id="divSecondaryDistribution" style="display:none;">
                <div style="text-align:left;">
                    <button id="btnBackDistribution" style="margin-right:10px;padding: 5px;" data-role="none" class="btn btn-less-width btn-transparent">
                        <span class="ui-icon ui-icon-arrow-l" style="display:inline-block; margin-left:10px;"></span>
                        <span>Back</span>                        
                    </button>
                </div>
                <div id="divSecondDistributionChart" style="text-align:center; color:#000; text-shadow:none;">
                    
                </div>
            </div>
        </div>

        <div class="popup-footer align-right">
            <button data-role="none" class="btn custom-btn-theme-c btn-less-width popup-close">
                Close
            </button>
        </div>
    </div>

    <div data-role="popup" id="panDates" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3>Period</h3></center>
        <div data-role="content">
            <div style="padding:10px;">
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

    @*<div data-role="popup" class="MoreReportsPopUp" id="panOption" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="btnCloseMoreOption">Close</a>
        <center><h3>Go To</h3></center>

        <div data-role="content">

            <div class="noBorderList" data-role="collapsible-set" data-corners="false" data-theme="a">
                <div style="padding:15px;">
                    <ul data-role="listview" data-theme="a">
                        <li><a class="btnMainDashboardPage">Main Dashboard</a></li>
                        <li><a class="btnSalesAnalyticsPage">Sales Analytics</a></li>
                        <li><a class="btnReplenishmentProcessing">Store Replenishment Processing & Analytics</a></li>
                        <li><a class="btnSalesOrderProcessingAnalyticsPage">Sales Order Processing & Analytics</a></li>
                        <li><a id="butPendingSalesReturns">Request and Approvals</a></li>
                        <li><a class="btnTrackDevice">GPS Location</a></li>
                    </ul>
                </div>

                <div data-role="collapsible">
                    <h3>Inventory Reports and Analytics</h3>
                    <ul data-role="listview">
                        <li><a class="btnMoreOptionInventoryBalance custom-btn-theme-c">Inventory Balance</a></li>
                        <li><a class="btnMoreOptionInventoryLedger custom-btn-theme-c">Inventory Ledger</a></li>
                        <li><a class="btnMoreOptionStockReceivingAndTransfers custom-btn-theme-c">Stock Receiving and Transfers</a></li>
                        <li><a class="btnInventoryCounts custom-btn-theme-c">Physical Inventory and Variance Report</a></li>
                    </ul>
                </div>

                <div data-role="collapsible">
                    <h3>On-Print POS Sales Reports</h3>
                    <ul data-role="listview">
                        <li><a id="popPOSReportCashierSalesReport" class="custom-btn-theme-c">Cashiers Sales Report</a></li>
                        <li><a id="popPOSReportManagerReport" class="custom-btn-theme-c">Manager Sales Report</a></li>
                        <li><a id="popPOSReportSalesAdjustment" class="custom-btn-theme-c">Sales Adjustment Report</a></li>
                        <li><a id="popPOSReportZReport" class="custom-btn-theme-c">Z Report</a></li>
                    </ul>
                </div>

                <div data-role="collapsible">
                    <h3>Cash Handling</h3>
                    <ul data-role="listview">
                        <li><a id="popPOSReportDeposit" class="custom-btn-theme-c">Deposits Report</a></li>
                    </ul>
                </div>

                <div data-role="collapsible">
                    <h3>Special Reports</h3>
                    <ul data-role="listview">
                        <li><a id="popPOSReportMallLessorAyala" class="custom-btn-theme-c">Ayala Tenant Monitoring Report</a></li>
                        <li><a id="popPOSReportMallLessorSM" class="custom-btn-theme-c">SM COIN Report</a></li>
                        <li><a id="popPOSReportPrincipal" class="custom-btn-theme-c">Report to Principal</a></li>
                        <li><a id="popPOSReportShopperTrak" class="custom-btn-theme-c">ShopperTrak</a></li>
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
    </div>*@

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
            <select id="selListOfFowstGroup" style="font-size:20px;"></select>
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

    @*PopUp Receiving Transfer FOWST*@

    <div data-role="popup" id="panReceivingTransferFowst" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3>&nbsp;FOWST or Vendor of Interest&nbsp;</h3></center>
        <div data-role="content">
            <div>
                <span id="spanRecTransFowst1Error" style="display:none; color:#F00"><br />Please select a FOWST.</span>
                <select id="selFowst1">
                    <option value="">Please Select Fowst (Required)</option>
                </select>
            </div>
            <br />
            <div>
                <span id="spanRecTransFowst2Error" style="display:none; color:#F00"><br />Please select a another FOWST.</span>
                <select id="selFowst2">
                    <option value="">Please Select Fowst or Vendor (Optional)</option>
                </select>
                <div id="divThirdParty" style="display:none;">
                    <br />
                    <strong>Vendor:</strong><span id="txtVendorName"></span>
                </div>
            </div>
            <br />
            <button id="btnReceivingTransferFowstDone" data-role="none" class="btn custom-btn-theme-b">Done</button>
            <br /><br />
            <button id="btnReceivingTransferFowstCancel" data-role="none" class="btn custom-btn-theme-c">Cancel</button>
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
            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <div class="ui-bar">
                        <h3 id="lblReceivingAndTransFersSummaryFrom"> </h3>
                        <br /> <br />
                        <table id="tableReceivingAndTransfersDetails1" data-role="table" style="display:table;" class="ui-table ui-responsive table-stroke table-stripe">
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
                <div class="ui-block-b">
                    <div class="ui-bar">
                        <h3 id="lblReceivingAndTransFersSummaryTo"> </h3>
                        <br /> <br />
                        <table id="tableReceivingAndTransfersDetails2" data-role="table" style="display:table;" class="ui-table ui-responsive table-stroke table-stripe">
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

    @*PopUp Inventory Balance *@

    <div data-role="popup" id="panInventoryBalanceOptions" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3>Choose Inventory Type</h3></center>
        <div data-role="content">
            <div style="padding: 8px 15px 16px 15px; max-height:200px; overflow:auto;">
                <ul data-role="listview" id="lstReceivingTransferOrganization" class="li-theme-c noBorderList custom-list custom-list-sm">
                    <li id="liViewAll" class="btnLIListofFowstGroup">
                        <a href="#" class="btn custom-btn-theme-c txt-left" style="box-shadow:none !important; font-weight:bold !important; width:auto;">
                            All Inventory Types
                        </a>
                    </li>
                    <li id="liViewProducts" class="btnLIListofFowstGroup">
                        <a href="#" class="btn custom-btn-theme-c txt-left" style="box-shadow:none !important; font-weight:bold !important; width:auto;">
                            Finished Goods
                        </a>
                    </li>
                    <li id="liViewRawMaterials" class="btnLIListofFowstGroup">
                        <a href="#" class="btn custom-btn-theme-c txt-left" style="box-shadow:none !important; font-weight:bold !important; width:auto;">
                            Intermediate Goods and Raw Materials
                        </a>
                    </li>
                </ul>
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

    @*<div id="divMainDashboard" style="display:none;">
        <div class="ui-grid-a ui-responsive">
            <div class="ui-block-a" style="padding-right:9px; overflow:hidden; display:flex;">
                <button style="margin-right:10px;" data-role="none" class="btn btn-less-width btn-transparent btnNotRequired btnPeriod">
                    <span class="lblPeriod"> Today </span>
                    <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span>
                </button>
            </div>
            <div class="ui-block-b txt-right" style="padding:0px 16px 0px 20px;">
                <div style="margin-bottom:-10px; visibility:visible; display:block;">
                    <div class="ui-grid-solo">
                        <div class="ui-block-a" style="float:right;">
                            <input type="search" placeholder="Search invoice no., product description, sales representative or customer" class="txtSearch" />
                        </div>
                    </div>
                    <div class="txtSearchStatus" style="display:none; float:left; color:#FF5959; text-shadow:0px 0px 0px #FFF;">
                        Search box is required!
                    </div>
                </div>
                <br />
            </div>
        </div>
        <br />
        <div class="ui-grid-a ui-responsive" style="margin-top:-19px;">
            <div class="ui-block-a thirtyseventy-block-a" style="padding-right:10px; padding-top:10px;">
                <a class="btn custom-btn-theme-b" style="padding:0px;">
                    <div style="padding:23px; font-weight:normal; text-shadow:0; font-size:30px; margin:0px auto; text-align:left">
                        <div style="font-weight:bold; font-size:32px; text-align:left; text-overflow:ellipsis; white-space:nowrap; overflow:hidden; padding:0px;" id="OrganizationName">Organization</div>
                        <hr style="border:1px solid #297E6D" />

                        <div style="text-align:left; font-size:16px">Net Sales</div>
                        <div style="text-align:right; font-size:32px;" id="TotalNetSales">-</div>

                        <div style="text-align:left; font-size:16px">Gross Sales</div>
                        <div style="text-align:right; font-size:25px;" id="TotalGrossSales">-</div>

                        <div style="text-align:left; font-size:16px">Discounts</div>
                        <div style="text-align:right; font-size:25px;" id="TotalDiscounts">-</div>

                        <div style="text-align:left; font-size:16px">Refunds</div>
                        <div style="text-align:right; font-size:25px;" id="TotalRefunds">-</div>

                        <div style="text-align:left; font-size:16px">Exchanges</div>
                        <div style="text-align:right; font-size:25px;" id="TotalExchanges">-</div>

                        <div style="text-align:left; font-size:16px">FOWST COUNT:</div>
                        <div style="text-align:right; font-size:25px;" id="TotalFowstCount">-</div>
                    </div>
                    <div class="nothingdiv" style="height:135px;"></div>
                </a>
            </div>

            <div class="ui-block-b thirtyseventy-block-b">
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

    </div>*@



    @*START Sales Analytics Page*@

    <div id="divSalesAnalytics">

        <div class="ui-grid-solo ui-responsive">
            <div class="ui-block-a" style="padding-right:9px; overflow:hidden; display:flex;">
                <button style="margin-right:10px;" data-role="none" class="btn btn-less-width btn-transparent btnNotRequired btnPeriod">
                    <span class="lblPeriod">Today</span>
                    <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span>
                </button>

                <button id="btnViewType" style="margin-right:10px;" data-role="none" class="btn btn-less-width btn-transparent btnNotRequired">
                    <span id="txtViewType">Sales Trend</span>
                    <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span>
                </button>
            </div>
        </div>
        <br />

        <div id="divGroupSales">
            <div style="box-shadow:0px 0px 10px #CCC; background-color:#F9F9F9; padding:5px;">
                <div class="ui-grid-solo">
                    <div id="dvGroupChart" style="text-align:center; color:#000; text-shadow:none;">
                    </div>
                    <br />
                </div>

                <div class="ui-grid-solo">
                    <div style="text-align:right;">
                        <button id="btnColumns" data-inline="true">Columns</button>
                    </div>
                    <div class="ui-block-a" style="overflow:auto">                        
                        <table id="tblReport" data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe">
                            <thead>
                                <tr id="reportTableHeader" class="custom-theme-f">
                                    <th headno="1" data-sort="string" style="cursor:pointer;">Tenant's Store Name</th>
                                    <th headno="2" data-sort="string" style="cursor:pointer;">Store Address</th>
                                    <th headno="3" data-sort="string" style="cursor:pointer;">Mall or Branch Code</th>
                                    <th headno="4" data-sort="string" style="cursor:pointer;">Tenant No.</th>
                                    <th headno="5" data-sort="string" style="cursor:pointer;">Class Code</th>
                                    <th headno="6" data-sort="string" style="cursor:pointer;">Trade Code</th>
                                    <th headno="7" data-sort="string" style="cursor:pointer;">Outlet No.</th>
                                    <th headno="8" data-sort="string" style="cursor:pointer;">Sales Type</th>
                                    <th headno="9" data-sort="float" style="text-align:center;cursor:pointer;">Net Sales</th>
                                    <th headno="10" data-sort="int" style="text-align:center;cursor:pointer;">Net Sales Contribution(%)</th>
                                    <th headno="11" data-sort="float" style="text-align:center;cursor:pointer;">Gross Sales</th>
                                    <th headno="12" data-sort="int" style="text-align:center;cursor:pointer;">Gross Sales Contribution(%)</th>
                                    <th headno="13" data-sort="float" style="text-align:center;cursor:pointer;">Refunds</th>
                                    <th headno="14" data-sort="int" style="text-align:center;cursor:pointer;">Refunds Contribution(%)</th>
                                    <th headno="15" data-sort="float" style="text-align:center;cursor:pointer;">Exchanges</th>
                                    <th headno="16" data-sort="int" style="text-align:center;cursor:pointer;">Exchanges Contribution(%)</th>                                    
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
                        @*<div>
                            <a href="#" id="lnkTransactionDetails" style="visibility:hidden">  Click here to view transaction details </a>
                        </div>*@

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

                <div id="dvSlice" style="visibility:hidden" class="panel">
                    <div class="panel-title custom-theme-f">
                        <a id="liSecondChartMainLabel"></a>
                        <a href="#" id="liSecondChart"></a>
                    </div>

                    <div class="panel-content">
                        <div id="dvSecondChart"></div>
                        <div>
                            @*<table>
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
                            <br />*@

                            <br />
                            <table id="tblSecondChart" data-role="table" data-mode="reflow" style="display:table;" class="ui-table ui-responsive table-stroke table-stripe"> </table>
                        </div>
                    </div>
                </div>

                <br />
                <br />
                <div id="dvDice" style="visibility:hidden" class="panel">
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

                        <table id="tblThirdChart" class="ui-table ui-responsive table-stroke table-stripe"></table>
                    </div>
                </div>
            </div>
        </div>

        <div data-role="popup" id="panSalesDistribution" data-overlay-theme="a" data-theme="a" data-corners="false">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3 style="padding:0px 10px 0px 10px;">Sales Distribution</h3></center>
            <div data-role="content">
                <div style="padding:10px;">
                    <ul data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm" id="lstSlice"></ul>
                </div>
            </div>
        </div>

        <div data-role="popup" id="panDice" data-overlay-theme="a" data-theme="a" data-corners="false">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3 style="padding:0px 10px 0px 10px;">Sales Distribution</h3></center>
            <div data-role="content">
                <div style="padding:10px;">
                    <ul data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm" id="lstDice"></ul>
                </div>
            </div>
        </div>

        @*<div data-role="popup" id="panSalesTrends" data-overlay-theme="a" data-theme="a" data-corners="false">*@

        @*<div id="divInventoryLevels" style="display:none; box-shadow:0px 0px 10px #CCC; background-color:#F9F9F9; padding:5px;">
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
            <br />*@
            @* bookmark: arvin, GroupInventoryLevels *@
            @*<div id="divGroupInventoryLevelsResult" style="clear:both"></div>
        </div>*@

        <div id="divMoniesAcc" style="display:none; box-shadow:0px 0px 10px #CCC; background-color:#F9F9F9; padding:5px;">

        </div>

        <div id="divInventoriesAcc" style="display:none; box-shadow:0px 0px 10px #CCC; background-color:#F9F9F9; padding:5px;">

        </div>
    </div>


    @*START Store Replenishment Processing & Analytics*@

    <div id="divSalesReplenishmentProcessing" style="display:none;">
    </div>
    <div id="divSalesOrderProcessing" style="display:none;">
    </div>
    <div id="divOnPrintPOSSalesReport" style="display:none;">
    </div>
    <div id="divFranchiseRequest" style="display:none;">
    </div>

    <div id="panTransactionDetails" style="display:none;">

        <b>POS Sales Transactions</b><br />
        @* <ul data-role="listview" data-inset="true" id="lstTransactionDetails">
            <li data-role="list-divider">POS Sales Transactions</li>
            <li>*@
        @*<form id="formSearch">*@
        <input id="txtSearchProduct" placeholder="Search invoice no., product description, sales representative or customer" type="search" autofocus />

        @*</form>*@
        <div class="ui-grid-solo">
            <div class="ui-block-a" id="divTransactionDetails" style="overflow:auto;">
                <table style="margin-right:5px;" id="tblTransactionDetails" class="ui-table ui-responsive table-stroke table-stripe">
                    <thead>
                        <tr class="custom-theme-f">
                            <th> Invoice No. </th>
                            <th> Invoice Date </th>
                            <th> Product Description </th>
                            <th style="text-align: center;"> Qty </th>
                            <th> Unit Measure </th>
                            <th> Returned Product For Rebate</th>
                            <th> Rebate Qty </th>
                            <th> Rebate Unit Measure</th>
                            <th style="text-align: right;"> Amount </th>
                            <th style="text-align: right;"> Sub Total </th>
                            <th style="text-align: right;"> Discount </th>
                            <th> Sales Representative(s) </th>
                            <th> Customer </th>
                            <th> Last Updated By </th>
                        </tr>
                    </thead>
                    <tbody id="tblTransactionDetailsBody"></tbody>
                </table>
                <br />

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
                <table style="margin-right:5px;" id="tblSalesReturnDetails" class="ui-table ui-responsive table-stroke table-stripe">
                    <thead>
                        <tr class="custom-theme-f">
                            <th> Memo Number </th>
                            <th> Memo Date </th>
                            <th> Srv No. </th>
                            <th style="text-align: right;"> Amount </th>
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



    @*START Inventory's Page*@

    <div id="divInventoryBalancePage" style="display:none;">
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

    <div id="divInventoryProductBreakdown" style="display:none;">
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
