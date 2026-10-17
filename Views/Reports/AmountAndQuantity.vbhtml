@Code
    ViewData("Title") = "Amount and Quantity  Report"
    Layout = "~/Views/Main/Layout.vbhtml"
End Code

    <script type="text/javascript">
    
    </script>
    @If ProWeb.Common.UseMinifiedJavascripts Then
        @<text>
        <script src="/Scripts/minified/AmountAndQuantityReport.js" type="text/javascript"></script>
        </text>
    Else
        @<text>
        <script src="/Scripts/sources/Project/AmountAndQuantityReport.js" type="text/javascript"></script>
        </text>
    End If
    <style type="text/css">
        
    </style>

<form id="formMain">
<div id="divResult">
</div>
<div class="divInput">
    <table class="TableForm">
        <tr>
            <td class="label tdField">
                Supplier:
            </td>
            <td>
                <div id="divSupplier" style="display: inline">
                </div>
                <button id="butBrowseSupplier" class="button btn" type="button">Browse</button>
            </td>
        </tr>
        <tr>
            <td class="label tdField">
                Date from:
            </td>
            <td>
                <input id="txtDateFrom" name="txtDateFrom" />
            </td>
        </tr>
        <tr>
            <td class="label tdField">
                Date to:
            </td>
            <td>
                <input id="txtDateTo" name="txtDateTo" />
            </td>
        </tr>
        <tr>
            <td>
                Show customers:
            </td>
            <td>
                <input id="chkShowCustomers" name="chkShowCustomers" type="checkbox" value="" />
            </td>
        </tr>
    </table>
    <button id="butGenerate">Generate</button>
</div>
</form>
