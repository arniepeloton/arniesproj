@Code
    ViewData("Title") = "Sales Returns"
    Layout = "~/Views/Shared/Mobiz.vbhtml"
End Code
<script>
@Section Header

End Section
    require([GetPathFormat('/Scripts3/SalesReturns.js?crc=B1A2DB83')]);
</script>
<h1>
    Sales Returns</h1>
@*@If Common.WebRequest.QueryString("IsMobile") <> "1" Then
    @<applet name="jzebra" code="jzebra.PrintApplet.class" archive="/Scripts/jzebra.jar" width="10" height="10">
            </applet>
End If*@
<div id="divPosBranch" style="margin-bottom: 13px;">
    <a id="lnkPosBranch" style="text-decoration: none;" href="#">Please choose a branch &#x25BC;</a>
</div>
<form id="formSearch">
<div data-role="fieldcontain">
    <label for="txtSearch">
        Invoice no.:</label>
    <input id="txtSearch" type="search" placeholder="Please enter receipt number" autofocus />
    <button id="butBarCode">Scan Barcode / QR Code</button>
</div>
</form>
@*<div data-role="collapsible-set">
    <div data-role="collapsible" data-collapsed="false">
        <h3>
            Enter Invoice</h3>
        <div data-role="content">
        </div>
    </div>
    <div data-role="collapsible">
        <h3>
            Customer Name</h3>
        <div data-role="content">
            <form id="formSearchCustomer">
            <div data-role="fieldcontain">
                <label for="txtSearchCustomer">
                    Customer:</label>
                <input id="txtSearchCustomer" type="search" />
            </div>
            </form>
            <form id="formSearchItem">
            <div data-role="fieldcontain">
                <label for="txtSearchItem">
                    Invoice no. or product:</label>
                <input id="txtSearchItem" type="search" />
            </div>
            </form>
        </div>
    </div>
</div>
*@
<style>
    .MiniNavBar.ui-navbar li .ui-btn .ui-btn-inner
    {
        padding: 0;
    }
</style>
<div data-role="navbar" class="MiniNavBar1">
    <ul>
        <li><a href="#" data-href="Nav1a">Receipt Contents</a></li>
        <li><a href="#" data-href="Nav1b">Return Cart</a></li>
    </ul>
</div>
<div id="Nav1a" class="content_div">
    <div id="divInvoiceContents">
        <div id="divReturnables" style="display: none">
            <h3 name="spanStockDescription" style="white-space: normal">
            </h3>
            <table class="tableUnits">
                <tr name="trTemplate" style="cursor: pointer">
                    <td name="tdQty">
                    </td>
                    <td name="tdUnit">
                    </td>
                    <td name="tdPrice">
                    </td>
                    <td name="tdLineTotal">
                    </td>
                    <td name="tdQuantityReturned">
                    </td>
                </tr>
            </table>
            <div>
                Item total: <span name="spanTotal"></span>
            </div>
        </div>
    </div>
</div>
<div id="Nav1b" class="content_div">
    Return Cart
    <div id="divCartSection" style="display: none">
        @*<a id="lnkSalesOrder" style="text-decoration: none;" href="#" data-role="button">
            <div name="divNumber">
            </div>
            <div name="divDate">
            </div>
            </a>
             <button id="">New Cart</button>*@
        <div id="divCartStock" style="display: none">
            <h3 name="spanStockDescription" style="white-space: normal">
            </h3>
            <table class="tableUnits">
                <tr name="trTemplate">
                    <td name="tdQty">
                    </td>
                    <td name="tdUnit">
                    </td>
                    <td name="tdPrice">
                    </td>
                    <td name="tdLineTotal">
                    </td>
                </tr>
            </table>
            <div>
                Item total: <span name="spanTotal"></span>
            </div>
        </div>
        <div style="font-weight: bold; padding-top: 10px;">
            Total: <span id="spanCartTotal"></span>
        </div>
        <button id="butCheckOut">Check Out Returns Cart</button>
    </div>
    <div id="divCartEmpty" class="Empty">
        No items in cart
    </div>
        <div id="divPeopleMenuPopUp" data-role="popup">
            <ul data-role="listview" data-inset="true" style="min-width: 210px;">
                <li data-role="divider">Menu</li>
                @*<li><a href="#dvOrganization" data-rel="popup" data-position-to="window">Download Database</a></li>*@
                <li><a id="popPeopleMenuPeopleInfoSummary">Profile</a></li>
                <li><a id="btnSignOut">Log Out</a></li>
            </ul>
        </div>

</div>
@Section OtherPages
    <div id="divCheckOut" data-role="page">
        <div data-role="header">
            <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
            <h1>
                Returns Check Out</h1>
               @* @If Common.WebRequest.QueryString("IsMobile") <> "1" Then
                    @<applet name="jzebra" code="jzebra.PrintApplet.class" archive="/Scripts/jzebra.jar" width="10" height="10">
                            </applet>
                End If*@
        </div>
        <div data-role="content">
            <div id="divCheckOutMessage">
            </div>
            <a id="butCheckOutNow" name="butCheckOutNow" data-role="button"><h3>Make A Merchandise Exchange</h3>
                <p style="font-weight: normal; margin-top: -15px;">
                    Issue a Sales Return Voucher (SRV) to this customer</p>
            </a>
            <a id="butCheckOutRefund" name="butCheckOutNow" data-role="button"><h3>Issue A Refund</h3>
                <p style="font-weight: normal; margin-top: -15px;">
                    This function is currently applicable to pure cash purchases only. You must have sufficient authorization to issue a refund.</p>
            </a>
            <a id="divCheckOutBack" data-role="button"><h3>Back</h3>
                <p style="font-weight: normal; margin-top: -15px;">
                    Go back to the previous page</p>
            </a>
        </div>
    </div>
End Section
