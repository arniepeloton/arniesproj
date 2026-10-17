@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    @RenderSection("head", False)
End Section
<script>
    require([GetPathFormat('/Scripts3/SalesMenuPopUp.js?crc=')]);
</script>
<div data-role="page">
    <div data-role="header" data-position="fixed1" style="background: none white; border: none">
        <div class="ui-grid-a">
            <div class="ui-block-a">
                <div style="margin: 10px 0 0 10px">
                    <a id="btnMobizGoToHome" href="#">
                        <img src="/Content/images/MobizNewLogo.png" height="48px" width="112px" style="padding-left: 10px;" />
                    </a>
                </div>
            </div>
            <div class="ui-block-b">
                @*<a type="button" id="btnMobizPopUpMenu" data-rel="popup" data-icon="grid" data-inline="true" style="float:right;">Menu</a>*@
                <div id="divUserMenu" style="float: right; margin: 10px 15px 0 0">
                    Hi,
                    <div id="divUser" style="display: inline">
                        <a id="btnSalesMenuLogin">Please login</a>
                    </div>
                </div>
            </div>
        </div>
        @RenderSection("Header", False)
    </div>
    <div data-role="content" style="padding-top: 0px">
        @RenderBody()
    </div>
    <div data-role="footer_no_theme">
        <div style="text-align: center; padding-bottom: 15px">
            This mobile site is powered by Mobiz&#8482;.<br />
            Copyright 2013, 1Mobiz Inc., All Rights Reserved.<br />
            www.mobiz-apps.com
        </div>
    </div>
    <div data-role="popup" id="divMobizPopUpMenu">
        <ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="divider" style="font-weight: normal; font-size: 16px;">Choose an App</li>
            <li><a id="popMobizHome" style="font-weight: normal; font-size: 14px;">Home</a></li>
            <li><a id="popMobizPOS" style="font-weight: normal; font-size: 14px;">POS</a></li>
            @*            <li><a id="popMobizSFA" style="font-weight:normal; font-size: 14px;">SFA</a></li>
                <li><a id="popMobizCRM" style="font-weight:normal; font-size: 14px;">CRM</a></li>*@
            <li><a id="popMobizReports" style="font-weight: normal; font-size: 14px;">Dashboards and Reports</a></li>
            <li><a id="popMobizSettings" style="font-weight: normal; font-size: 14px;">Settings</a></li>
        </ul>
    </div>
</div>
<div id="pageMobizPopUpMenuPOS" data-role="page">
    <div>
        <ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="list-divider">POS</li>
            <li><a id="popMobizPOSPOS">Point of Sale (POS)</a></li>
            <li><a id="popMobizPOSSalesReturn">Sales Return</a></li>
        </ul>
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnMobizPOSBack" data-role="button">Back</a>
            </div>
        </div>
    </div>
</div>
<div id="pageMobizPopUpMenuReports" data-role="page">
    <div>
        <ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="list-divider">Dashboards and Reports</li>
            <li><a id="popMobizReportsPOS">POS Reports</a></li>
            @*<li><a id="popMobizReportsSFA">SFA Reports</a></li>
            <li><a id="popMobizReportsCRM">CRM Reports</a></li>*@
            <li><a id="popMobizReportsInventoryLedger" href="/InventoryLedger">Inventory Ledger</a></li>
        </ul>
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnMobizReportsBack" data-role="button">Back</a>
            </div>
        </div>
    </div>
</div>
<div id="pageMobizPopUpMenuPOSReports" data-role="page">
    <div>
        <ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="list-divider">POS Reports</li>
            <li><a id="popMobizPOSReportSalesReport">Cashier Sales Report</a></li>
            <li><a id="popMobizPOSReportManagerReport">Manager Sales Report</a></li>
            <li><a id="popMobizPOSReportSalesAdjustment">Sales Adjustment Report</a></li>
            <li><a id="popMobizPOSReportMallLessorReport">Mall Lessor Report</a></li>
        </ul>
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnMobizPOSReportBack" data-role="button">Back</a>
            </div>
        </div>
    </div>
</div>
<div id="pageMobizPopUpMenuPOSReportsMallLessorReport" data-role="page">
    <div>
        <ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="list-divider">Mall Lessor Reports</li>
            <li><a id="popMobizPOSReportMallLessorAyala">Ayala Tenant Monitoring Report</a></li>
            <li><a id="popMobizPOSReportMallLessorSM">SM Coin Report</a></li>
        </ul>
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnMobizPOSReportMallLessorBack" data-role="button">Back</a>
            </div>
        </div>
    </div>
</div>
<div id="pageMobizPopUpMenuSettings" data-role="page">
    <div>
        <ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="list-divider">Settings</li>
            <li><a id="popMobizSecuritySettings">Security Settings</a></li>
            <li><a id="popMobizCompanyProfile">Company Profile</a></li>
            <li><a id="popMobizCompanyPlaces">FOWST <span style="font-size: smaller;">(Factory, Office, Warehouse, Store, or Truck)</span></a></li>
            <li><a id="popMobizCompanyEmployees">Employees</a></li>
            <li><a id="popMobizCompanyCustomers">Customers/Clients</a></li>
            <li><a id="popMobizCompanySuppliers">Suppliers/Vendors</a></li>
            <li><a id="popMobizCompanyShippers">Shippers</a></li>
            <li><a id="popMobizReceiptSettings">Receipt Settings</a></li>
            <li><a id="popMobizSalesReturnsSettings">Sales Returns Settings</a></li>
        </ul>
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnMobizSettingsBack" data-role="button">Back</a>
            </div>
        </div>
    </div>
</div>
<div id="pageMobizPopUpMenuSecuritySettings" data-role="page">
    <div>
        <ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="list-divider">Settings</li>
            <li><a id="popMobizSecuritySettingsRoles">Roles</a></li>
            <li><a id="popMobizSecuritySettingsDevices">Devices</a></li>
        </ul>
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnMobizSecuritySettingsBack" data-role="button">Back</a>
            </div>
        </div>
    </div>
</div>
@RenderSection("OtherPages", False)
