@Code
    ViewData("Title2") = "Planned Orders"
    Layout = "Master.vbhtml"
End Code
    <script type="text/javascript">
        
    </script>
    @If ProWeb.Common.UseMinifiedJavascripts Then
        @<text>
        <script src="/Scripts/minified/Drp.js" type="text/javascript"></script>
        </text>
    Else
        @<text>
        <script src="/Scripts/sources/Project/Drp.js" type="text/javascript"></script>
        </text>
    End If
    <style type="text/css">
        
    </style>
<div id="divDrpStocks">
    <a href="/Drp/StockSched">Stock Schedule</a>
    <form id="formMain">
    <div id="divResult">
    </div>
    <table class="TableForm">
        <tr>
            <td class="label tdField">
                Invoice date from:
            </td>
            <td>
                <input id="txtInvoiceDateFrom" name="txtInvoiceDateFrom" value="@Format(Now.Date.AddMonths(-1), cGlobal.FormatDateGrid)" />
            </td>
        </tr>
        <tr>
            <td class="label tdField">
                Invoice date to:
            </td>
            <td>
                <input id="txtInvoiceDateTo" name="txtInvoiceDateTo" value="@Format(Now.Date, cGlobal.FormatDateGrid)" />
            </td>
        </tr>
        <tr>
            <td class="label tdField">
                Keywords:
            </td>
            <td>
                <input name="txtKeywords" style="width: 500px" placeholder="Stock number or stock description" title="Stock number or stock description" />
            </td>
        </tr>
        <tr>
            <td class="label tdField">
                Purchasing Frequency:
            </td>
            <td>
                <select id="cbPurchasingFrequency" name="cbPurchasingFrequency" title="Semi monthly is not yet implemented">
                    <option value="weekly">Weekly</option>
                    <option value="semimonthly">Semi-monthly</option>
                    <option value="monthly">Monthly</option>
                </select>
            </td>
        </tr>
    </table>
    <button id="butShowStocks" style="margin-bottom: 10px;">Show Stocks</button>
    </form>
    <div id="divData" style="height: 400px">
    </div>
    <div id="divDetails" style="display: none">
        <table class="TableForm">
            <tr>
                <td>
                    Stock Number:
                </td>
                <td>
                    <span id="spanStockNumber"></span>
                </td>
            </tr>
            <tr>
                <td>
                    Stock Description:
                </td>
                <td>
                    <span id="spanStockDescription"></span>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    Qty Sold Per Period
                </td>
            </tr>
        </table>
    </div>
    <div id="divTasks" style="margin-top: 5px">
            <h2>Tasks and information</h2>
        <div id="divTasksResult">
        </div>
        Total line items:<span id="spanItemCount"></span><br />
        <div style="margin: 3px 0">
            <button id="butStockCheckAll" title="Include all items above">Check all</button> <button id="butStockCheckNone" title="Exclude all items above">Check none</button>
        </div>
        <div style="margin: 3px 0">
            Date needed:
            <input id="txtDateNeeded" /><br />
        </div>
        <button id="butGenerate" title="Generate proposed PO's based on the checked items above">Generate proposed PO</button><br />
        <a href="/drp/ProposedPo">View records</a>
    </div>
</div>
