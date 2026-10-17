@Code
    ViewData("Title") = "Supplier Stock"
    Layout = "~/Views/Main/Layout.vbhtml"
End Code

    <script type="text/javascript">
    
    </script>
    @If ProWeb.Common.UseMinifiedJavascripts Then
        @<text>
        <script src="/Scripts/minified/SupplierStock.js" type="text/javascript"></script>
        </text>
    Else
        @<text>
        <script src="/Scripts/sources/Project/SupplierStock.js" type="text/javascript"></script>
        </text>
    End If
    <style type="text/css">
        .Indent
        {
            padding-left: 30px;
        }
    </style>

<div id="divSupplierStock">
    <div name="divResult">
    </div>
    <table>
        <tr>
            <td>Supplier:</td>
            <td><span name="spanSupplier"></span></td>
        </tr>
        <tr>
            <td>Stock number:</td>
            <td><span name="spanStockNumber"></td>
        </tr>
        <tr>
            <td>Stock description:</td>
            <td><span name="spanStockDescription"></span></td>
        </tr>
    </table>
    <table>
        <tr>
            <td>
                Vendor Lead time (in days)
            </td>
            <td>
                <input name="txtLeadTime" />
            </td>
        </tr>
        <tr>
            <td>
                Lot size
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
                </select>
            </td>
        </tr>
        <tr>
            <td title="Minimum Order Quantity">
                MOQ
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
