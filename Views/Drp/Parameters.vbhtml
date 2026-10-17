@Code
    ViewData("Title2") = "Parameters"
    Layout = "Master.vbhtml"
End Code
    <script type="text/javascript">
    
    </script>
    @If ProWeb.Common.UseMinifiedJavascripts Then
        @<text>
        <script src="/Scripts/minified/DrpParameters.js" type="text/javascript"></script>
        </text>
    Else
        @<text>
        <script src="/Scripts/sources/Project/DrpParameters.js" type="text/javascript"></script>
        </text>
    End If
    <style type="text/css">
        .Indent
        {
            padding-left: 30px;
        }
    </style>
<div id="divDrpParameters">
    <div name="divResult">
    </div>
    <table class="tableForm">
        <tr>
            <td>
                PO Processing Lead Time (in Days)
            </td>
            <td>
                <input name="txtPoProcessingLeadTime" />
            </td>
        </tr>
        <tr>
            <td>
                Vendor Lead Time (in Days)
            </td>
            <td>
                <input name="txtVendorLeadTime" />
            </td>
        </tr>
        <tr>
            <td>
                Shipping Lead Time (in Days)
            </td>
            <td>
                <input name="txtShippingLeadTime" />
            </td>
        </tr>
        <tr>
            <td>
                Total Lead Time (in Days)
            </td>
            <td>
                <span name="spanTotalLeadTime"></span>
            </td>
        </tr>
        <tr>
            <td>
                Default currency
            </td>
            <td>
                <div name="divCurrency">
                </div>
                <button name="butCurrency">Change</button>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                @*Make firm orders for planned orders in X weeks*@
                Firm up the next <input name="txtFirmOrders" style="width:30px" /> weeks of planned purchase orders
            </td>
        </tr>
        <tr>
            <td>
                Frequency of recomputation of DRP tables (in days)
            </td>
            <td>
                <input name="txtFrequency" />
            </td>
        </tr>
        <tr>
            <td>
                Forecast horizon (in weeks)
            </td>
            <td>
                <input name="txtForecastHorizon" />
            </td>
        </tr>
        <tr>
            <td>
                Safety stock duration (in weeks)
            </td>
            <td>
                <input name="txtSafetyStockDuration" />
            </td>
        </tr>
        <tr>
            <td title="This will be the default if there is no default lot size in the supplier and/or SKU library">
                Default lot size
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td class="Indent">
                Qty:
            </td>
            <td>
                <input name="txtLotSize" />
            </td>
        </tr>
        <tr>
            <td class="Indent">
                Unit measure:
            </td>
            <td>
                <select name="cbLotSize">
                    <option value="1">1st unit measure</option>
                    <option value="2">2nd unit measure</option>
                    <option value="3">3rd unit measure</option>
                    <option value="4">4st unit measure</option>
                </select>
            </td>
        </tr>
        <tr>
            <td title="Minimum Order Quantity. This will be the default if there is no default MOQ in the supplier and/or SKU library">
                Default MOQ
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td class="Indent">
                Qty:
            </td>
            <td>
                <input name="txtMoq" />
            </td>
        </tr>
        <tr>
            <td class="Indent">
                Unit measure:
            </td>
            <td>
                <select name="cbMoq">
                    <option value="1">1st unit measure</option>
                    <option value="2">2nd unit measure</option>
                    <option value="3">3rd unit measure</option>
                    <option value="4">4st unit measure</option>
                </select>
            </td>
        </tr>
        <tr>
            <td>
                <button name="butSave">Save</button>
            </td>
            <td>
            </td>
        </tr>
    </table>
</div>
