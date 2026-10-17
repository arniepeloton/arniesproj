@Code
    ViewData("Title") = "Retail Dashboard"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
<script src="/Scripts3/Browser/common.js"></script>
<script src="/Scripts3/Browser/LibraryCustomers.js"></script>
<script src="/Scripts3/RetailDashboard.js"></script>
<style>
    .DataGrid td
    {
        padding: 3px;
        vertical-align: top;
        text-align: right;
    }
</style>
<div style="margin-bottom: 10px">
    <select id="cbBranch">
        <option>[branch]</option>
        <option>Sales Composition by SKU</option>
        <option>Sales Composition by Time</option>
    </select>
</div>
<div id="cbDashboardType" style="margin-bottom: 10px">
    <select>
        <option>Sales and Profit</option>
        <option>Sales Composition by SKU</option>
        <option selected="selected">Sales Composition by Time</option>
    </select>
</div>
@*<label>
    <input id="chkCustomers" type="checkbox" checked="checked" />All customers</label>
<form id="formCustomer">
<div id="divCustomer" data-role="fieldcontain" style="display: none">
    <label for="txtCustomer">
        Customer:</label>
    <input id="txtCustomer" value="" />
</div>
</form>*@
<div id='divPeriod' data-role="navbar" style="margin-bottom: 10px">
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
    <h2 id="lblDateRangeLastYear">
    </h2>
    <h2 id="lblDateRangeThisYear">
    </h2>
</div>
<table class="DataGrid" id="dtMain">
    <tr>
        <th>
        </th>
        <th>
            This year
        </th>
        <th>
            Last year
        </th>
        <th>
            ABS Change
        </th>
        <th>
            % Change
        </th>
    </tr>
    <tr>
        <td>
            Sales
        </td>
        <td>
            12,345.67
        </td>
        <td>
            12,345.67
        </td>
        <td>
            12,345.67
        </td>
        <td>
            12,345.67
        </td>
    </tr>
    <tr>
        <td>
            COGS
        </td>
        <td>
            12,345.67
        </td>
        <td>
            12,345.67
        </td>
        <td>
            12,345.67
        </td>
        <td>
            12,345.67
        </td>
    </tr>
    <tr>
        <td>
            Gross Profit
        </td>
        <td>
            12,345.67
        </td>
        <td>
            12,345.67
        </td>
        <td>
            12,345.67
        </td>
        <td>
            12,345.67
        </td>
    </tr>
    <tr>
        <td>
            Gross Proft Margin
        </td>
        <td>
            12,345.67
        </td>
        <td>
            12,345.67
        </td>
        <td>
            12,345.67
        </td>
        <td>
            12,345.67
        </td>
    </tr>
</table>
<div data-role="fieldcontain">
    <fieldset data-role="controlgroup" data-type="horizontal">
        <legend>Report Type:</legend>
        <label>
            <input type="radio" name="optReportType" value="pos" checked="checked" />POS Transactions</label>
        <label>
            <input type="radio" name="optReportType" value="department" />Group by Department</label>
        @*<label>
            <input type="radio" name="optReportType" value="sku" />SKU</label>*@
    </fieldset>
</div>
<div id='divSku'>
    <div data-role="fieldcontain">
        <label for="cbSort">
            Sort by:</label>
        <select id="cbSort">
            <option value="CM to total sales">CM to total sales</option>
            <option value="Product category">Product category</option>
            <option value="Sales value">Sales value</option>
            <option value="Gross profit">Gross profit</option>
            <option value="CM to total gross profit">CM to total gross profit</option>
            <option value="Gross margin">Gross margin</option>
            <option value="Hits">Hits</option>
        </select>
    </div>
    <div id="divStocks">
        <label>
            <input type="checkbox" id="chkStocks" />Include stocks</label></div>
    <div data-role="fieldcontain">
        <fieldset data-role="controlgroup" data-type="horizontal">
            <legend>File Format:</legend>
            <label>
                <input type="radio" name="optFileFormat" value="pdf" checked="checked" />PDF</label>
            <label>
                <input type="radio" name="optFileFormat" value="excel" />Excel</label>
        </fieldset>
    </div>
    <label>
        <input type="checkbox" id="chkCompress" />Compress download as ZIP file</label>
</div>
<button id="butDownload">Download Now</button>