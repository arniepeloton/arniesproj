@Code
    ViewData("Title") = "Home"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <script>
        require([GetPathFormat('/Scripts/sources/People/PeopleMenu.js?crc=')]);
        require([GetPathFormat('/Scripts/sources/People/PeopleDashBoard.js?crc=')]);
    </script> 
End Section
<div id="pageHome" data-role="page">
    <div id="pagePeopleMenu" data-role="header" data-position="not-fixed" style="background: none white;
        border: none;padding: 15px 15px 0px 15px;">
        <div>
            <a type="button" id="btnPeopleMenuPopUpMenu" data-rel="popup" data-icon="grid" data-inline="true"
                style="float: right;margin:0px;">Menu</a>
        </div> 
        <div class="ui-grid-a">
            <div class="ui-block-a" style="width: 125px;">
                <a id="btnPeopleMenuPeopleImage" style="cursor: pointer;">
                    <img src="/Content/images/NoPhoto.jpg" />
                </a>
            </div>
            <div class="ui-block-b" id="photoGrid">
                <div>
                    <a id="lblName" style="cursor: pointer;">Name Goes Here...</a>                                
                </div>
                <div>
                <label id="lblEmployer">
                        Currently employed at...</label></div>
            </div>
        </div>
    </div>
    <div data-role="content" style="padding-top:0px;">
        <div id="divPeopleMenuPopUp" data-role="popup">
            <ul data-role="listview" data-inset="true" style="min-width: 210px;">
                <li data-role="divider">Menu</li>
                @*<li><a href="#dvOrganization" data-rel="popup" data-position-to="window">Download Database</a></li>*@
                <li><a id="popPeopleMenuPeopleInfoSummary">Profile</a></li>
                <li><a id="btnSignOut">Log Out</a></li>
            </ul>
        </div>
        @* ' tanggal na as of Dec 20, 2013
        <div data-role="navbar" data-iconpos="left">
            <ul>
                <li><a id="tabHome" data-icon="home">Home</a></li>
                <li><a id="tabFind" data-icon="search">Find</a></li>
            </ul>
        </div>*@
        <div id="divHome">
            <div data-role="fieldcontain">
                <div data-role="collapsible" data-collapsed="false" data-theme="b" data-content-theme="d"
                    data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
                    <h2>
                        Messages</h2>
                    <ul data-role="listview" id="lstMessage">
                    </ul>
                </div>
                <div id="divOrgApps">
                </div>
            </div>
        </div>
        <div id="divFind" style="display: none;">
            <div>
                <ul data-role="listview" id="lstFind" data-inset="true">
                    <li><a id="btnPeopleMenuFindEstablishment">
                        <img src="/Content/images/box.png" />
                        <h3>
                            Establishments</h3>
                    </a></li>
                    <li><a>
                        <img src="/Content/images/box.png" />
                        <h3>
                            Deals</h3>
                    </a></li>
                    <li><a>
                        <img src="/Content/images/box.png" />
                        <h3>
                            Work</h3>
                    </a></li>
                </ul>
            </div>
        </div>

        <div id="dvOrganization" div data-role="popup" id="popupPanel" data-corners="false" data-theme="none" data-shadow="false" data-tolerance="0,0">
            <ul data-role="listview" id="lstOrganization" data-inset="true">
                <li data-role="list-divider">Select organization database to download</li>
                <li>
                    <table id="tblOrganization" style="width:100%">                    
                    </table>    
                </li>
            </ul>
        </div><!-- /panel -->
    </div>
</div>
<div id="pagePeopleMsgType1Info" data-role="page">
    <div data-role="header">
        <h1>
            Message Details</h1>
    </div>
    <div data-role="content">
        <div data-role="fieldcontain" style="display: none;">
            <input type="text" readonly id="txtPeopleMsgT1ID" />
        </div>
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
                <div>
                    <legend id="lblPeopleMsgT1Subject" style="font-size: x-large;"></legend>
                </div>
                <br />
                <div>
                    <legend id="lblPeopleMsgT1From">From : </legend>
                </div>
                <div>
                    <legend id="lblPeopleMsgT1DateTime">Date/Time : </legend>
                </div>
            </fieldset>
        </div>
        <div data-role="fieldcontain">
            <br />
            <fieldset data-role="controlgroup">
                <div>
                    <legend id="lblPeopleMsgT1"></legend>
                </div>
            </fieldset>
        </div>
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnPeopleMsgT1Confirm" data-role="button">
                    <h3>
                        Confirm Employment</h3>
                    <p style="font-weight: normal; margin-top: -15px;">
                        Click on this button to confirm that you are an employee of this company.
                    </p>
                </a>
            </div>
            <div style="text-align: center">
                <a id="btnPeopleMsgT1Deny" data-role="button">
                    <h3>
                        Decline Employment Invite</h3>
                    <p style="font-weight: normal; margin-top: -15px;">
                        Click on this button if you are not an employee of this company.
                    </p>
                </a>
            </div>
            <div style="text-align: center">
                <a id="btnPeopleMsgT1Cancel" data-role="button">
                    <h3>
                        Cancel</h3>
                </a>
            </div>
        </div>
    </div>
</div>
<div id="pagePeopleMsgType2Info" data-role="page">
    <div data-role="header">
        <h1>
            Message Details</h1>
    </div>
    <div data-role="content">
        <div data-role="fieldcontain" style="display: none;">
            <input type="text" readonly id="txtPeopleMsgT2ID" />
        </div>
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
                <div>
                    <legend id="lblPeopleMsgT2Subject" style="font-size: x-large;"></legend>
                </div>
                <br />
                <div>
                    <legend id="lblPeopleMsgT2From">From : </legend>
                </div>
                <div>
                    <legend id="lblPeopleMsgT2DateTime">Date/Time : </legend>
                </div>
            </fieldset>
        </div>
        <div data-role="fieldcontain">
            <br />
            <fieldset data-role="controlgroup">
                <div>
                    <legend id="lblPeopleMsgT2"></legend>
                </div>
            </fieldset>
        </div>
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnPeopleMsgT2BackToHome" data-role="button">
                    <h3>Back To Main</h3>
                </a>
            </div>
        </div>
    </div>
</div>

<div id="pagePeopleMsgType2List" data-role="page">
    <div data-role="header">
        <h1>Messages</h1>
    </div>
    <div data-role="content">
        <div id="divMessageContainer">
        </div>
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnPeopleMsgT2ListBackToHome" data-role="button">
                    <h3>Back To Main</h3>
                </a>
            </div>
        </div>
    </div>
</div>

<div id="pagePopUpMenuDashboardsAndReports" data-role="page">
    <div data-role="header">
        <h1>
            Dashboards and Reports</h1>
    </div>
    <div>
        @*<ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="list-divider">Reports</li>
            <li><a id="popReportsPOS">POS Reports</a></li>*@ @*<li><a id="popReportsSFA">SFA Reports</a></li>
            <li><a id="popReportsCRM">CRM Reports</a></li>*@ @*<li><a id="popReportsInventoryLedger" href="/InventoryLedger">Inventory Ledger</a></li>
            <li><a id="popReportsInventoryCount" href="/InventoryCount/Report">Inventory Count Report</a></li>
            <li><a id="popReportsNoPrice" href="/Alerts/NoPrice">Products Without Prices</a></li>
        </ul>*@
        <br />
        <div data-role="collapsible" data-theme="b" data-iconpos="right" data-collapsed="false">
            <h3>
                Online Dashboards</h3>
            <ul data-role="listview">
                <li><a id="liSales" href="#">Sales</a></li>
                 <li><a id="liOrders" href="#">Sales Orders</a></li>
                <li><a id="liRefunds" href="#">Refunds</a></li>
                <li><a id="liExchange" href="#">Exchange</a></li>
                <li><a id="liReceiving" href="#">Receiving</a></li>
                <li><a id="liTransferOut" href="#">Transfer-Out</a></li>
                <li><a id="liInventory" href="#">Inventory Count</a></li>
                <li><a id="liTrackDevice" href="#">Track Device</a></li>
                @*<li><a id="liCRM" href="#">Sales Order Performance Tracking</a></li>*@
            </ul>
        </div>
        <br />
        <div data-role="collapsible" data-theme="b" data-iconpos="right">
            <h3>
                On-Print POS Sales Reports</h3>
            <ul data-role="listview" id="ulReportType">
                <li><a id="popPOSReportCashierSalesReport">Cashier Sales Report</a></li>
                <li><a id="popPOSReportManagerReport">Manager Sales Report</a></li>
                <li><a id="popPOSReportSalesAdjustment">Sales Adjustment Report</a></li>
                <li><a id="popPOSReportZReport">Z Report</a></li>
            </ul>
        </div>
        <br />
        <div data-role="collapsible" data-theme="b" data-iconpos="right">
            <h3>
                Special Reports</h3>
            <ul data-role="listview" id="ulReportType">
                <li><a id="popPOSReportMallLessorAyala">Ayala Tenant Monitoring Report</a></li>
                <li><a id="popPOSReportMallLessorSM">SM COIN Report</a></li>
                <li><a id="popPOSReportPrincipal">Report to Principal</a></li>
                <li><a id="popPOSReportShopperTrak">ShopperTrak</a></li>
            </ul>
        </div>

        <br />
        <div data-role="collapsible" data-theme="b" data-iconpos="right">
            <h3>
                Audit Trail</h3>
            <ul data-role="listview">
                <li><a id="liPOSTransactionDetail">POS Transaction Detail</a></li>
                <li><a id="liInventoryLedger">Inventory Ledger</a></li>
                <li><a id="liMDMDashboard">MDM (Mobile Device Management) Dashboard</a></li>
            </ul>
        </div>

        <br />
        <div data-role="collapsible" data-theme="b" data-iconpos="right">
            <h3>Settings</h3>
            <ul data-role="listview">
                <li><a id="liDashboardSettings">Dashboard Settings</a></li>
            </ul>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnDashboardsAndReportsBack" data-role="button">Back To Home</a>
            </div>
        </div>
    </div>
</div>
<div id="pagePopUpMenuTroubleShootingTools" data-role="page">
    <div>
        <ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="list-divider">Troubleshooting Tools</li>
            <li><a id="popTroubleshootingToolsReprintAReceipt">Reprint A Receipt</a></li>
            <li><a id="popTroubleshootingToolsReconstructSalesData">Reconstruct Sales Data</a></li>
            <li><a id="popTroubleshootingToolsUpdateDatabase">Synchronize Data</a></li>            
            <li><a id="popTroubleshootingToolsFlushData">Flush Data</a></li>
            <li><a id="popTroubleshootingToolsLazyUpdate">Lazy Update</a></li>
        </ul>

        <div data-role="popup" id="popupMenu" >
            <div data-role="popup" id="dvpopup" data-theme="a" class="ui-corner-all" data-dismissible="false" data-overlay-theme="a">
                <p id="popMessage" style="padding:10px 20px;text-align:center"></p>
            </div>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnTroubleshootingToolsBack" data-role="button">Back To Home</a>
            </div>
        </div>
    </div>
</div>
<div id="pagePopUpMenuPOSReports" data-role="page">
    <div data-role="header">
        <h1>
            Dashboards and Reports</h1>
    </div>
    <div data-role="content">
        <div data-role="collapsible" data-iconpos="right">
            <h3>
                Online Sales Dashboard</h3>
            <ul data-role="listview" data-inset="true">
                <li data-role="list-divider">Report Period </li>
                <li>
                    <div id="dvSortByDay">
                        @*<div data-role="fieldcontain">
	                        <label for="selPOSSalesReportRange" class="select"> </label>*@
                        <select id="selPOSSalesReportRange">
                            <option value="">Please Select</option>
                            <option value="Today">Today</option>
                            <option value="Yesterday">Yesterday</option>
                            <option value="Day">Range by day</option>
                        </select>
                        @*</div>*@
                        <div id="divPOSSalesReportRangeByDate" style="display: none;">
                            <div data-role="fieldcontain">
                                <fieldset data-role="controlgroup" data-type="horizontal">
                                    <legend id="lblPOSReportRangeByDateFrom">From : </legend>
                                    <select id="selPOSSalesReportByDateFromMonth">
                                        <option value="">MMM</option>
                                    </select>
                                    <select id="selPOSSalesReportByDateFromDay">
                                        <option value="">DD</option>
                                    </select>
                                    <select id="selPOSSalesReportByDateFromYear">
                                        <option value="">YYYY</option>
                                    </select>
                                </fieldset>
                            </div>
                            <div data-role="fieldcontain" id="divPOSReportRangeByDateTo">
                                <fieldset data-role="controlgroup" data-type="horizontal">
                                    <legend>To : </legend>
                                    <select id="selPOSSalesReportByDateToMonth">
                                        <option value="">MMM</option>
                                    </select>
                                    <select id="selPOSSalesReportByDateToDay">
                                        <option value="">DD</option>
                                    </select>
                                    <select id="selPOSSalesReportByDateToYear">
                                        <option value="">YYYY</option>
                                    </select>
                                </fieldset>
                            </div>
                        </div>
                    </div>
                </li>
            </ul>
            <ul data-role="listview" id="lstFOWSTFrame" data-inset="true">
                <li data-role="list-divider" id="liFowstCount"></li>
                <li>
                    <table id="tblFOWST" style="width: 100%">
                    </table>
                </li>
            </ul>
            <ul data-role="listview" data-inset="true">
                <li data-role="list-divider">Sort By </li>
                <li>
                    <fieldset data-role="controlgroup">
                        <input type="radio" name="rdSort" id="rdTrendingSales" checked="checked" />
                        <label for="rdTrendingSales">
                            Trending Sales
                            <div style="font-size: smaller; font-weight: normal">
                                See a time series graph of your sales
                            </div>
                        </label>
                        <input type="radio" name="rdSort" id="rdSalesDistribution" />
                        <label for="rdSalesDistribution">
                            Sales Distribution
                            <div style="font-size: smaller; font-weight: normal">
                                See your sales grouped according to day, hour, month or quarter
                            </div>
                        </label>
                        @*<input type="radio" name="rdSort" id="rdSliceAndDice" />
     	                <label for="rdSliceAndDice">
                            Slice and Dice
                            <div style="font-size: smaller; font-weight: normal"> Be a data ninja and slice and dice your sales data whichever you like </div>
                        </label>*@
                    </fieldset>
                    <div id="divPOSSalesReportGroup">
                        <div data-role="fieldcontain">
                            <label for="selGroupBy">
                                Group by
                            </label>
                            <select id='selGroupBy'>
                                <option value=''>Select Group </option>
                                <option value='optDay'>Day </option>
                                <option value='optMonth'>Month </option>
                                <option value='optQuarter'>Quarter </option>
                                <option value='optYear'>Year </option>
                                <option value='optOperatingHours'>Operating Hours </option>
                            </select>
                        </div>
                    </div>
                    <br />
                    <h3>
                        Slice and Dice
                    </h3>
                    <div style="font-size: smaller; font-weight: normal">
                        Be a data ninja and slice and dice your sales data whichever you like
                    </div>
                    <div id="divPOSSalesReportRangeByFeatures">
                        <div data-role="fieldcontain">
                            <label for="selSliceFeature">
                                Slice according to
                            </label>
                            <select id="selSliceFeature">
                                <option value="">Select Option </option>
                                <option value="optSalesValue">Sales Value </option>
                                <option value="optSalesVolume">Sales Volume </option>
                                <option value="optHits">Hits </option>
                            </select>
                            <label for="selDiceFeature">
                                Start Dicing into
                            </label>
                            <select id="selDiceFeature">
                                <option value="">Select Group </option>
                                <option value="optBrand">Brand </option>
                                <option value="optDivisionDescription">Division Description </option>
                            </select>
                            <label for="selMinchFeature">
                                Then Mince into
                            </label>
                            <select id="selMinchFeature">
                                <option value="">Select Group </option>
                                <option value="optBrand">Brand </option>
                                <option value="optDivisionDescription">Division Description </option>
                            </select>
                        </div>
                    </div>
                </li>
            </ul>
            <br />
            <br />
            <div>
                <a id="btnViewOnline" data-role="button">View Online</a>
            </div>
        </div>
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnPOSReportBack" data-role="button">Back To Home</a>
            </div>
        </div>
    </div>
    @*content*@
</div>
<div id="pagePopUpMenuSettings" data-role="page">
    <div>
        <ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="list-divider">System Administration</li>
            <li>
                <a id="popSetupChecklist">
                    <h4>Setup Checklist</h4> 
                    <p style="white-space: normal">The setup checklist is a convenient way for you to keep track of your setup progress. This checklist walks you through the important data that need to be set up before use of Mobiz by your organization. </p>
                </a>
            </li>
            <li><a id="popSystemSettings">System Settings</a></li>
            <li><a id="popDataLibraries">Data Libraries </a></li>
        </ul>
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnSettingsBack" data-role="button">Back To Home</a>
            </div>
        </div>
    </div>
</div>
<div id="pagePopSystemSettings" data-role="page">
    <div>
        <ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="list-divider">System Settings</li>
            <li><a id="popSecuritySettings">Security Settings</a></li>
            <li><a id="popCompanyProfile">Company Profile</a></li>
            <li><a id="popReceiptSettings">Receipt Settings</a></li>
            <li><a id="popSalesReturnsSettings">Sales Returns Settings</a></li>
            <li><a id="popDiscountSettings">Discount Settings</a></li>
            <li><a id="popPIMGroups">Hot Buttons Settings</a></li>
          @*  <li><a id="popTableView">Table View Settings</a></li>*@
            <li><a id="popPrinterRoles">Printer Roles</a></li>
            <li><a id="popSystemBehavior">System Behavior</a></li>
            <li><a id="popPrinterSetup">Device Printer Setup</a></li>
        </ul>
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnSystemSettingsBack" data-role="button">Back To Home</a>
            </div>
        </div>
    </div>
</div>

<div id="pagePopSystemBehavior" data-role="page">
    <div>
        <ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="list-divider">System Behavior</li>
            <li><a id="popDecimalInQuantity">Decimals in Quantity Settings</a></li>
            <li><a id="popPOSSettings">POS Settings</a></li>
        </ul>
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnSystemAdministrationBack" data-role="button">Back To Home</a>
            </div>
        </div>
    </div>
</div>

<div id="pagePopDataLibraries" data-role="page">
    <div>
        <ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="list-divider">Data Libraries</li>
            <li><a id="popCompanyPlaces">FOWST <span style="font-size: smaller;">(Factory, Office, Warehouse, Store, or Truck)</span></a></li>
            <li><a id="popCompanyEmployees">Employees</a></li>
            <li><a id="popCompanyProducts">PIM <span style="font-size: smaller;">(Product Information Management)</span></a></li>
            <li><a id="popCompanyCustomers">Customers/Clients</a></li>
            <li><a id="popCompanySuppliers">Suppliers/Vendors</a></li>
            <li><a id="popCompanyShippers">Shippers</a></li>
            <li><a id="popCRM">CRM</a></li>
@*            <li><a id="popCompanyPrinters">Printers</a></li>*@
            @*<li><a id="popWorkingHours">Working Hours</a></li>*@
        </ul>
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnDataLibrariesBack" data-role="button">Back To Home</a>
            </div>
        </div>
    </div>
</div>

<div id="pagePopUpEmployeeSettings" data-role="page">
    <div>
        <ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="list-divider">Employee Settings</li>
            <li><a id="popEmployeeList"> Employee List </a></li>
            <li><a id="popEmployeeWorkSchedule">Work Schedule</a></li>
            <li><a id="popEmployeePayGrade">Pay Grade (this feature is not yet implemented)</a></li>
        </ul>
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnEmployeesBack" data-role="button">Back To Home</a>
            </div>
        </div>
    </div>
</div>

<div id="pagePopUpMenuSecuritySettings" data-role="page">
    <div>
        <ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="list-divider">Settings</li>
            <li><a id="popSecuritySettingsDevices">Authorized Devices</a></li>
            <li><a id="popSecuritySettingsRoles">Roles</a></li>
            <li><a id="popSecuritySettingsMDM">MDM (Mobile Device Management) Settings</a></li>
        </ul>
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnSecuritySettingsBack" data-role="button">Back To Home</a>
            </div>
        </div>
    </div>
</div>
<div id="pagePopUpMenuMDMSettings" data-role="page">
    <div>
        <ul style="white-space: normal" data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="list-divider">Settings</li>
            <li><a id="popSecurityMDMSettingsCompanyWide">
                <h3>
                    Company-Wide White-Listed Apps</h3>
                <p style="white-space: normal">
                    Specify 3rd-party applications allowed to be installed on all of your company's
                    mobile devices.</p>
            </a></li>
            <li><a id="popSecurityMDMSettingsPolicies">
                <h3>
                    MDM Policies</h3>
                <p style="white-space: normal">
                    Create specific MDM policies to govern mobile devices used by differnt functional
                    or departmental groups within your organization.</p>
            </a></li>
        </ul>
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnSecurityMDMSettingsBack" data-role="button">Back To Home</a>
            </div>
        </div>
    </div>
</div>
<div id="pagePopUpMenuProducts" data-role="page">
    <div>
        <ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="list-divider">PIM</li>
            <li><a id="popPIMUploadDownloadProduct">Upload/Download Product List</a></li>
            <li><a id="popPIMProductCompanyWide">Company Wide Product List</a></li>
            <li><a id="popPIMFOWSTProduct">FOWST Product List</a></li>
            <li><a id="popPIMProductPrices">Adjust Product Prices</a></li>
        </ul>
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnPIMBack" data-role="button">Back To Home</a>
            </div>
        </div>
    </div>
</div>
<div id="pagePopUpMenuProductsUploadDownload" data-role="page">
    <div>
        <ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="list-divider">Upload/Download Product List</li>
            <li><a id="popPIMDownloadProductTemplate">Generate Excel Template for Product List</a></li>
            @*<li><a id="popPIMImportProductTemplate">Import Product List from Excel File</a></li>*@
            <li><a id="popPIMUpload">Upload Product List From Excel File</a></li>
        </ul>
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnPIMUploadDownloadBack" data-role="button">Back To Home</a>
            </div>
        </div>
    </div>
</div>
<div id="pagePopUpMenuAdjustPrice" data-role="page">
    <div>
        <ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="list-divider">Adjust Product Prices</li>
            <li><a id="popPriceChangeSingle">Single Product Price Adjustment</a></li>
            <li><a id="popPriceChangeBulk">Bulk Price Adjustment</a></li>
        </ul>
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnAdjustPriceBack" data-role="button">Back To Home</a>
            </div>
        </div>
    </div>
</div>
<div data-role="page" id="pageDownloadDatabaseSuccessful">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">
        <div data-role="fieldcontain">    
            <p>Your database is ready. Please sign in to start using Mobiz Complete Retail.</p>
        </div>
        <a data-role="button" id="btnSignInRedirect">Sign in</a>

    </div>

</div>

<div id="pagePopUpMenuControlPanelSettings" data-role="page">
    <div>
        <ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="list-divider">Control Panel</li>
            <li><a id="popSubscriptionDashboard">Subscription</a></li>
            <li><a id="popCreateMessage">Messages</a></li>
        </ul>
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnControlPanelBack" data-role="button">Back To Home</a>
            </div>
        </div>
    </div>
</div>

<div id="pagePopUpSubscriptionDashboard" data-role="page">
    <div data-role="header">        
        <h1>Mobiz Subscription Dashboard</h1>
    </div>

    <div data-role="content" data-dom-cache="true">
        <div><input id="txtSearch" type="search" value="" placeholder="Search" /></div>

        <br />
        <div id="divRecordCount"></div>
        <table data-role="table" id="tblDashboard" data-mode="reflow" class="ui-responsive">
            <thead>
                <tr>
                    <th>Subscription No</th>
                    <th>Super Administrator</th>
                    <th>Email Address</th>
                    <th>Mobile Number</th>
                    <th>Company Name</th>
                    <th>Location</th>
                    <th>Business Industry</th>
                    <th>Source</th>
                    <th>Email Verification</th>
                    <th>Last Updated</th>
                    <th></th>
                </tr>                    
            </thead>                
            <tbody id='tblDashboardBody'>
            </tbody>
        </table>

        <br />

    </div>

</div>
<div data-role="dialog" data-close-btn="none" id="dialog">
	
	<div data-role="header" data-theme="d">
		<h1>Subscription Details</h1>
	</div>

	<div data-role="content" data-theme="c">

    <h3 id="hdrCompanyName"></h3>
    <div><strong>Office Address:</strong> <label id="lblOfficeAddress"></label></div>
    <div><strong>Business Address:</strong> <label id="lblBusinessAddress"></label></div>
    <div><strong>Promo Code:</strong> <label id="lblPromoCode"></label></div>
    <div><strong>Agent Name:</strong> <label id="lblAgentName"></label></div>
    <div><strong>Subscription Date:</strong> <label id="lblSubscriptionDate"></label></div>
    <br />
    <table data-role="table" id="tblDashboardDetails" data-mode="reflow" class="ui-responsive" border="1">
        <thead>
            <tr>
                <th>Product</th>
                <th>Concurrent Users</th>
                <th>Status</th>
                <th>Activation Date</th>
            </tr>                    
        </thead>                
        <tbody id='tblDashboardBodyDetails'>
        </tbody>
    </table>
        <br />
		<a href="dashboard" data-role="button" data-rel="back" data-theme="c">OK</a>    
	</div>
</div>

<div id="pagePopUpCreateMessage" data-role="page">
    <div data-role="header">        
        <h1>Create New Message</h1>
    </div>
    <div data-role="content">

        <div data-role="fieldcontain">
            <label for="selProduct">Product:</label>
	        <select id="selProduct">
				<option value="All Mobiz Products">All Mobiz Products</option>
				<option value="Mobiz Complete Retail">Mobiz Complete Retail</option>
            </select>
        </div>

        <div data-role="fieldcontain">
	            <label for="selCategory">Category:</label>
	            <select id="selCategory">
	                <option value="">Please Select Category</option>
                    <option value="Announcement">Announcement</option>
				    <option value="Update / Improvements">Update / Improvements</option>
				    <option value="Bug Fix">Bug Fix</option>
				    <option value="New Feature">New Feature</option>
                </select>
        </div>

        <div data-role="fieldcontain">
	            <label for="txtTitle">Title:</label>
	            <input type="text" id="txtTitle"  maxlength="50"> 
        </div>

        <div data-role="fieldcontain">
            <label for="txtMessage">Message:</label>
	        <textarea id="txtMessage" cols="25" rows="5" maxlength="2000"></textarea>
        </div>


        <br />
        <button id="btnSaveCreateMessage">Save</button>

        <a id="btnCreateMessageBack" data-role="button">Done</a> 

    </div>
</div>

<div id="pagePopUpLazyUpdate" data-role="page">
    <div>
        <ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="list-divider">System Administration</li>
            <li><a id="popSyncDataNow">Sync Data Now</a></li>
            <li><a id="popSendLastUpdatedToMobiz">Send Last Updated Data to Mobiz </a></li>
        </ul>
        <div>
            <br />
            <br />
            <div style="text-align: center">
                
            </div>
        </div>
    </div>

    <div data-role="popup" id="panLazySync" data-theme="b" class="ui-corner-all"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Sync</h3></center>
            <div data-role="content">
                <form>
                    <ul data-role="listview" data-inset="true" data-theme="e" id="lstMinch">
                       <li><a href="#" >Password changes </a></li>
                       <li><a href="#" >Employee Security Access Levels </a></li>
                       <li><a href="#" >FOWST List</a></li>
                       <li><a href="#" >Authorized Employees and FOWST assignments</a></li>
                       <li><a href="#" >Autorized Device List and Device-FOWST assignments</a></li>
                       <li><a href="#" >Printer Roles and IP Settings</a></li>
                       <li><a href="#" >Organization Employee</a></li>
                    </ul>
                </form>
                <a id="btnLazyUpdateSyncNow" data-role="button">Sync Now</a>
            </div>       
        </div>
</div>