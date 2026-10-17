@Code
    ViewData("Title") = "Proposed Purchase Orders"
    Layout = "~/Views/Main/Layout.vbhtml"
End Code
    @If ProWeb.Common.UseMinifiedJavascripts Then
        @<text>
        <script src="/Scripts/minified/ProposedPo.js" type="text/javascript"></script>
        </text>
    Else
        @<text>
        <script src="/Scripts/sources/Project/ProposedPo.js" type="text/javascript"></script>
        </text>
    End If
    <style type="text/css">
        
    </style>
<form id="formSearch">
<div id="divSearch">
    <div id="divResult">
    </div>
    <table class="TableForm">
        <tr>
            <td class="label tdField">
                Keywords:
            </td>
            <td>
                <input name="txtKeywords" style="width: 500px" title="Supplier, email, address, etc." />
            </td>
        </tr>
    </table>
    <button id="butShowStocks" style="margin-bottom: 10px;">Show Stocks</button>
</div>
</form>
<div id="divData" style="height: 400px">
</div>
<h2>
    Details</h2>
<div id="divDetails">
    <div class="Highlight">
        Not yet implemented<br />
        Click a row above and its contents will appear here
    </div>
    <table class="TableForm">
        <tr>
            <td>Supplier:</td>
            <td>
                <span name="lblSupplierName"></span>
            </td>
        </tr>
    </table>
</div>
