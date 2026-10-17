@Code
    ViewData("Title") = "Dashboards and Reports"
    Layout = "~/Views/Shared/plain.vbhtml"
End Code

@Section head
    <link rel="stylesheet" href="/Content/css/jquery.jqplot.min.css" />
    <link rel="stylesheet" href="/Content/css/ui.dynatree.css" />
    <link rel="stylesheet" href="/Content/css/qunit.css" />
    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <link rel="stylesheet" href="/Content/metro-calendar.css" />
     
    <script>
        require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/SalesDashboardIndex.js?crc=')]);
        require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/DashboardsAndReportsCharts.js?crc=')]);
        require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/flatui.js?crc=')]);
    </script>
    
    <script type="text/javascript" src="/Scripts/jquery.min.js"></script>
    <script type="text/javascript" src="/Scripts/jquery.widget.min.js"></script>
    <script type="text/javascript" src="/Scripts/metro.min.js"></script>
    <script type="text/javascript" src="/Scripts/metro-calendar.js"></script>
    <script type="text/javascript" src="/Scripts/metro-datepicker.js"></script>
    @*<script type="text/javascript" src="/Scripts/backfix.min.js"></script>*@

    <style>
    
     .jqplot-xaxis-hide {
            display: none
        }
    </style>
End Section

@*Header*@
    <div class="custom-theme-a position-fixed">

        <div id="HeaderDesktopVersion">
            <div class="ui-grid-a ui-responsive">
                <div class="ui-block-a" style="padding-right:9px;">
                    <div style="float:left; margin-left:10px; cursor:pointer;">
                        @*<a class="btnBack" data-iconpos="notext" data-theme="b" data-role="button" data-icon="back" title="Back">Back</a>*@
                        <a style="margin-top:8px;" class="btnHome" data-iconpos="notext" data-theme="b" data-role="button" data-icon="home" title="Home">Home</a>
                    </div>
                    <div style="float:left; margin-top:7px; margin-left:25px;">                            
                        <div class="OrganizationNameHeader" style="width:415px; text-overflow:ellipsis !important; font-size:25px; white-space:nowrap; overflow:hidden;">
                            Company Name
                        </div>
                        <br />
                        <div style="cursor:pointer; float:left; margin-top:-3px; margin-bottom:10px;" class="btnPeriod">
                            <span class="lblPeriod"> Today </span> 
                            <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span>
                        </div>
                    </div>
                    <div style="clear:both;"></div>
                </div>
                <div class="ui-block-b txt-right" style="padding:0px 16px 0px 20px;">
                    <div style="margin-bottom:-10px;">
                        <input type="text" placeholder="Search here" class="txtSearch" />
                    </div>

                    <br />

                    <div style="cursor:pointer; float:right; margin-top:-10px; margin-bottom:10px;" class="btnMoreReport">
                        <span> More Options </span>
                        <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span>
                    </div>
                </div>
            </div>
        </div>

        <div id="HeaderMobileVersion">
            <div class="ui-grid-solo">
                <div class="ui-block-a OrganizationNameHeader txt-center" id="OrganizationNameHeader">
                    Company Name Here
                </div>
            </div>
            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <a class="btn custom-btn-theme-a btnHome">
                        <span class="ui-icon ui-icon-home" style="display:inline-block;"/>
                    </a>
                </div>
                <div class="ui-block-b">
                    <a id="btnSearchMobile" class="btn custom-btn-theme-a">
                        <span class="ui-icon ui-icon-search" style="display:inline-block;"/>
                    </a>
                </div>
            </div>
            <div class="contentSearchEngine">
                <div class="ui-grid-solo ui-responsive">
                    <div class="ui-block-a" style="padding:12px;">
                        <input id="txtSearchProduct" type="text" placeholder="Search Here" class="txtSearch" />
                    </div>
                </div>
            </div>
            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <a class="btn custom-btn-theme-a btnPeriod">
                        <div class="lblPeriod" style="text-overflow:ellipsis; white-space:nowrap; overflow:hidden;">
                            Today
                        </div>
                        <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-top:3px;"/>
                    </a>
                </div>
                <div class="ui-block-b">
                    <a class="btn custom-btn-theme-a btnMoreReport">
                        <div style="text-overflow:ellipsis; white-space:nowrap; overflow:hidden;">
                            More Options
                        </div>
                        <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-top:3px;"/>
                    </a>
                </div>
            </div>
        </div>

        <div class="BreadCrumbs">
            <div class="ItemsBreadCrumbs">
                Main Dashboard
            </div>
        </div>

        @*PopUp PanDates*@

        <div data-role="popup" id="panDates" data-overlay-theme="a" data-theme="a" data-corners="false"> 
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
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>More Options</h3></center>

            <div data-role="content">

                <div class="noBorderList" data-role="collapsible-set" data-corners="false" data-theme="a">
                    <div data-role="collapsible">
                        <h3>Online Reports</h3>
                        <ul data-role="listview">
                            <li><a class="BusinessAnalyticsPage custom-btn-theme-c">Sales Analytics</a></li>
                            <li><a class="InventoryAnalyticsPage custom-btn-theme-c">Inventory Analytics</a></li>
                            <li><a class="btnReplenishmentProcessing custom-btn-theme-c">Store Replenishment Processing & Analytics</a></li>
                            <li><a class="SalesOrderProcessingAnalyticsPage custom-btn-theme-c">Sales Order Processing & Analytics</a></li>
                        </ul>
                    </div>
                    
                    <div data-role="collapsible" data-collapsed="false">
                        <h3>On-Print POS Sales Reports</h3>
                        <ul data-role="listview">
                            <li><a id="popPOSReportCashierSalesReport" class="custom-btn-theme-c">Cashiers Sales Report</a></li>
                            <li><a id="popPOSReportManagerReport" class="custom-btn-theme-c">Manager Sales Report</a></li>
                            <li><a id="popPOSReportSalesAdjustment" class="custom-btn-theme-c">Sales Adjustment Report</a></li>
                            <li><a id="popPOSReportZReport" class="custom-btn-theme-c">Z Report</a></li>
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
                        </ul>
                    </div>

                    <div data-role="collapsible">
                        <h3>Settings</h3>
                        <ul data-role="listview">
                            <li><a id="liDashboardSettings" class="custom-btn-theme-c">Dashboard Settings</a></li>
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
                    <a class="btnSalesAnalyticsPage btn custom-btn-theme-c txt-left">
                        <div style="padding:0px 12px 0px 12px;">
                            Sales Analytics
                        </div>
                    </a>
                </div>

                <div style="padding:5px 0px 5px 0px;">
                    <a class="btnInventoryAnalyticsPage btn custom-btn-theme-c txt-left">
                        <div style="padding:0px 12px 0px 12px;">
                            Inventory Analytics
                        </div>
                    </a>
                </div>

                <div style="padding:5px 0px 5px 0px;">
                    <a class="btnReplenishmentProcessing btn custom-btn-theme-c txt-left">
                        <div style="padding:0px 12px 0px 12px;">
                            Store Replenishment Processing & Analytics
                        </div>
                    </a>
                </div>

                <div style="padding:5px 0px 5px 0px;">
                    <a class="btnSalesOrderProcessingAnalyticsPage btn custom-btn-theme-c txt-left">
                        <div style="padding:0px 12px 0px 12px;">
                            Sales Order Processing & Analytics
                        </div>
                    </a>
                </div>
            </div> 
        </div>

    </div>

    <div data-role="content" class="mainContent">

        @*START Main Content Dashboard*@

        <div id="divMainDashboard" style="display:block; margin-top:47px;">
            <div class="ui-grid-a ui-responsive" style="margin-top:-19px;">
                <div class="ui-block-a thirtyseventy-block-a" style="padding-right:10px; padding-top:10px;">
                    <a class="btn custom-btn-theme-b" style="padding:0px;">
                        <div style="padding:23px; font-weight:normal; text-shadow:0; font-size:30px; margin:0px auto; text-align:left">
                            <div style="font-weight:bold; font-size:32px; text-align:left; text-overflow:ellipsis; white-space:nowrap; overflow:hidden; padding:0px;" id="OrganizationName">Name Here</div>
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
                    @*<div class="ui-grid-b">
                        <div class="ui-block-a" style="padding-right:10px; padding-top:10px;">
                            <span>
                                <a class="btn custom-btn-theme-c btnView">
                                    <div style="padding:15px;font-weight:normal; color:#000; font-size:20px; margin:0px auto;">
                                        
                                        <div style="font-weight:bold; font-size:20px; text-align:left; text-overflow:ellipsis; white-space:nowrap; overflow:hidden;">
                                            <span>MINDANAO</span>
                                        </div>
                                    
                                        <hr style="border:1px solid #CCC" />

                                        <div style="text-align:left; font-size:13px">Net Sales</div>
                                        <div style="text-align:right; font-size:15px;">P 10,000</div>

                                        <div style="text-align:left; font-size:13px">Gross Sales</div>
                                        <div style="text-align:right; font-size:15px;">10,000</div>

                                        <div style="text-align:left; font-size:13px">Discounts</div>
                                        <div style="text-align:right; font-size:15px;">0</div>

                                        <div style="text-align:left; font-size:13px">Refunds</div>
                                        <div style="text-align:right; font-size:15px;">5,000</div>

                                        <div style="text-align:left; font-size:13px">Exchanges</div>
                                        <div style="text-align:right; font-size:15px;">3,000</div>
                                    </div>
                                </a>
                            </span>
                        </div>
                        <div class="ui-block-b" style="padding-right:10px; padding-top:10px;">
                            <span>
                                <a class="btn custom-btn-theme-c btnView">
                                    <div style="padding:15px;font-weight:normal; color:#000; font-size:20px; margin:0px auto;">
                                        <div style="font-weight:bold; font-size:20px; text-align:left; text-overflow:ellipsis; white-space:nowrap; overflow:hidden;">
                                            <span>OVERSEAS</span>
                                        </div>

                                        <hr style="border:1px solid #CCC" />

                                        <div style="text-align:left; font-size:13px">Net Sales</div>
                                        <div style="text-align:right; font-size:15px;">P 10,000</div>

                                        <div style="text-align:left; font-size:13px">Gross Sales</div>
                                        <div style="text-align:right; font-size:15px;">10,000</div>

                                        <div style="text-align:left; font-size:13px">Discounts</div>
                                        <div style="text-align:right; font-size:15px;">0</div>

                                        <div style="text-align:left; font-size:13px">Refunds</div>
                                        <div style="text-align:right; font-size:15px;">5,000</div>

                                        <div style="text-align:left; font-size:13px">Exchanges</div>
                                        <div style="text-align:right; font-size:15px;">3,000</div>
                                    </div>
                                </a>
                            </span>
                        </div>
                        <div class="ui-block-c" style="padding-right:10px; padding-top:10px;">
                            <span>
                                <a class="btn custom-btn-theme-c btnView">
                                    <div style="padding:15px;font-weight:normal; color:#000; font-size:20px; margin:0px auto;">
                                        <div style="font-weight:bold; font-size:20px; text-align:left; text-overflow:ellipsis; white-space:nowrap; overflow:hidden;">
                                            <span>UNSORTED</span>
                                        </div>

                                        <hr style="border:1px solid #CCC" />

                                        <div style="text-align:left; font-size:13px">Net Sales</div>
                                        <div style="text-align:right; font-size:15px;">P 10,000</div>

                                        <div style="text-align:left; font-size:13px">Gross Sales</div>
                                        <div style="text-align:right; font-size:15px;">10,000</div>

                                        <div style="text-align:left; font-size:13px">Discounts</div>
                                        <div style="text-align:right; font-size:15px;">0</div>

                                        <div style="text-align:left; font-size:13px">Refunds</div>
                                        <div style="text-align:right; font-size:15px;">5,000</div>

                                        <div style="text-align:left; font-size:13px">Exchanges</div>
                                        <div style="text-align:right; font-size:15px;">3,000</div>
                                    </div>
                                </a>
                            </span>
                        </div>
                    </div>*@
                </div>
                </div>
            </div>

        </div>


        
        @*START Sales Analytics Page*@

        <div id="divSalesAnalytics" style="display:none; margin-top:58px;">
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
                        <div style="padding:15px;font-weight:normal; font-size:20px; margin:0px auto;" id="NetSalesHeight">
                            <div style="font-weight:bold; font-size:20px; text-align:center;">NET SALES</div>
                            <hr style="border:1px solid #297E6D" />

                            <div style="text-align:center; word-break:break-all;" class="SalesFontSize">
                                <label id="lblGroupNetSales"></label>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="ui-block-b" style="padding-right:10px; padding-top:10px;">
                    <div class="custom-theme-c">
                        <div style="padding:15px;font-weight:normal; font-size:20px; margin:0px auto;" id="GrossSalesHeight">
                            <div style="font-weight:bold; font-size:20px; text-align:center;">GROSS SALES</div>
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
                            <div style="text-align:left; font-size:13px">Discounts</div>
                            <div style="text-align:right; font-size:15px;"><label id="lblGroupDiscounts"></label></div>

                            <div style="text-align:left; font-size:13px">Refunds</div>
                            <div style="text-align:right; font-size:15px;"><label id="lblGroupRefunds"></label></div>

                            <div style="text-align:left; font-size:13px">Exchanges</div>
                            <div style="text-align:right; font-size:15px;"><label id="lblGroupExchanges"></label></div>
                        </div>
                    </div>
                </div>
            </div>
            <br /><br />
            <div>
        <ul data-role="listview" data-inset="true">
            <li data-role="list-divider" id="liMainChart"></li>
            <li>
                <div style="text-align:center">
                        <label id="IsDataPresent"> -Graph will be shown when you click on a group or FOWST (Factory, Office, Store, Warehouse or Truck)  name- </label>
                </div>
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
                    <a href="#" id="lnkTransactionDetails">  Click here to view transaction details </a>
                </div>
                <br />
                <table style="text-align:center; width:100%; visibility:hidden" id="tblDateFormatLabel">
                    <tr>
                        <td> <label id="lblDateFormatMessage"></label>   </td>              
                    </tr>                
               </table>               
            </li>
        </ul>        
            </div>
            <br />
            <div class="ui-grid-solo">
                <div class="ui-block-a">
                    <table style="margin-right:5px;" class="ui-table ui-responsive table-stroke table-stripe">
                        <thead>
                            <tr class="custom-theme-f">
                                <th></th>
                                <th style="text-align:center;">Net Sales</th>
                                <th style="text-align:center;">Gross Sales</th>
                                <th style="text-align:center;">Discounts</th>
                                <th style="text-align:center;">Exchanges</th>
                                <th style="text-align:center;">Refunds</th>
                            </tr>
                        </thead>
                        <tbody id="reportTable">                            
                        </tbody>                        
                    </table>
                </div>
                <br /> <br /> 
            </div>

            <div data-role="popup" id="panSalesDistribution" data-theme="b" class="ui-corner-all"> 
                <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
                <center><h3>Sales Distribution</h3></center>
                <div data-role="content">
                    <form>
                        <div style="height:500px;overflow:auto">
                            <ul data-role="listview" data-inset="true" data-theme="e" id="lstSlice">                        
                            </ul>
                        </div>
                    </form>
                </div>       
            </div>

            <div data-role="popup" id="panDice" data-theme="b" class="ui-corner-all"> 
                <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
                <center><h3>Sales Distribution</h3></center>
                <div data-role="content">
                    <form>
                        <div style="height:500px;overflow:auto">
                            <ul data-role="listview" data-inset="true" data-theme="e" id="lstDice">                       
                            </ul>
                        </div>
                    </form>
                </div>       
            </div>

            <div id="dvSlice" style="visibility:hidden">
                <ul data-role="listview" data-inset="true">
                    <li data-role="list-divider">&nbsp;<a id="liSecondChartMainLabel" style="font-size:large"></a><a href="#" id="liSecondChart" style="font-size:large"></a> </li>
                    <li> 
                        <div id="dvSecondChart"></div>
                        <div>                        
                                <label id="lblSecondChartInstruction" style="color:Red; font-size:small"> </label>
                                <br /> <br />
                                <table id="tblSecondChart" class="ui-table ui-responsive table-stroke table-stripe"> </table>             
                        </div>                
                     </li>
                </ul>        
            </div>

            <br />

            <div id="dvDice" style="visibility:hidden">
                <ul data-role="listview" data-inset="true">
                    <li data-role="list-divider">&nbsp;<a id="liThirdChartMainLabel" style="font-size:large"></a> <a href="#" id="liThirdChartLabel" style="font-size:large"></a> </li>
                    <li>
                        <div id="dvThirdChart">                             
                                </div>
                        <br /> <br />
                        <table id="tblThirdChart" class="ui-table ui-responsive table-stroke table-stripe"> </table>
                     </li>
                </ul>        
            </div>

        </div>



        @*START Store Replenishment Processing & Analytics*@

    <div id="divSalesReplenishmentProcessing" style="display:none; margin-top:45px;">
    </div>

    <div id="divOnPrintPOSSalesReport" style="display:none; margin-top:45px;">
    </div>

    <div data-role="popup" id="panTransactionDetails" data-theme="b" class="ui-corner-all">        
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Transaction Details</h3></center>
            <div data-role="content">
                    <ul data-role="listview" data-inset="true" id="lstTransactionDetails">
                        <li data-role="list-divider">POS Sales Transactions</li>
                        <li>
                            <div data-role="fieldcontain">
                                <label for="txtSearchProduct"> Search Product </label>
                                <input id="txtSearchProduct"/>
                            </div>
                            
                            <br />
                           <table data-role="table" data-mode="reflow" class="ui-body-d ui-shadow table-stripe ui-responsive" text-align:center" id="tblTransactionDetails">
                                <thead>
                                    <th style="border-bottom: 1px solid #d6d6d6;"> Invoice No. </th>
                                    <th style="border-bottom: 1px solid #d6d6d6;"> Invoice Date </th>
                                    <th style="border-bottom: 1px solid #d6d6d6;"> Product Description </th>
                                    <th style="border-bottom: 1px solid #d6d6d6;"> Qty </th>
                                    <th style="border-bottom: 1px solid #d6d6d6;"> Unit Measure </th>                        
                                    <th style="border-bottom: 1px solid #d6d6d6;"> Amount </th>
                                    <th style="border-bottom: 1px solid #d6d6d6;"> Sub Total </th>
                                    <th style="border-bottom: 1px solid #d6d6d6;"> Discount </th>
                                    <th style="border-bottom: 1px solid #d6d6d6;"> Sales Representative </th>                                    
                                </thead>
                                <tbody id="tblTransactionDetailsBody">                                                
                                </tbody>                    
                           </table>
                        </li>                       
                    </ul>

                    <ul data-role="listview" data-inset="true">
                        <li data-role="list-divider">POS Sales Returns</li>
                        <li> 
                            <table data-role="table" data-mode="reflow" class="ui-responsive" id="tblSalesReturnDetails">
                                <thead>
                                    <th> Memo Number </th>
                                    <th> Memo Date </th>                                
                                    <th> Srv No. </th>
                                    <th> Amount </th>
                                </thead>
                                <tbody id="tblSalesReturnDetailsBody">
                                </tbody>
                           </table>
                        </li>
                    </ul>
            </div>       
        </div>
    <br /> <br /> <br />

@*</div>*@