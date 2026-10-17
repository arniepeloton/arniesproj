@Code
    ViewData("Title") = "Retail Dashboard By Product"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code

<script src="/Scripts3/RetailDashboardByCategory.js"></script>

<style>
 .DataGrid td 
 {
     padding:3px;
     vertical-align:top;
     text-align:right;
 }
</style>

<div style="margin-bottom: 10px">
    <select id="cbBranch">
        <option>[branch]</option>
        <option>Sales Composition by SKU</option>
        <option>Sales Composition by Time</option>
    </select>
</div>

<div style="margin-bottom: 10px">
    <select>
        <option>Sales and Profit</option>
        <option selected="selected">Sales Composition by SKU</option>
        <option>Sales Composition by Time</option>
    </select>
</div>
<div data-role="navbar" style="margin-bottom: 10px">
    <ul>
        <li><a id="lnkWtd" data-role="button" href="#" class="ui-btn-active">WTD</a></li>
        <li><a id="lnkMtd" href="#">MTD</a></li>
        <li><a id="lnkQtd" href="#">QTD</a></li>
        <li><a id="lnkYtd" href="#">YTD</a></li>
    </ul>
</div>

<div data-role="header">
    <h1 id="lblPeriod">
        Week to Date</h1>
</div>
