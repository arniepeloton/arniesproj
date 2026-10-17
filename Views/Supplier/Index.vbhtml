@Code
    ViewData("Title") = "Supplier's Default Lot Size and MOQ (Min Order Quanitity)"
    Layout = "~/Views/Main/Layout.vbhtml"
End Code

    <script type="text/javascript">
    
    </script>
    @If ProWeb.Common.UseMinifiedJavascripts Then
        @<text>
        <script src="/Scripts/minified/Supplier.js" type="text/javascript"></script>
        </text>
    Else
        @<text>
        <script src="/Scripts/sources/Project/Supplier.js" type="text/javascript"></script>
        </text>
    End If
    <style type="text/css">
        .Indent
        {
            padding-left: 30px;
        }
    </style>

<div id="divSupplier">
    <div name="divResult">
    </div>
    <table>
        <tr>
            <td>Supplier:</td>
            <td><span name="spanSupplier"></span><button name="butBrowseSupplier">Browse...</button></td>
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
                    <option value="1">1st unit measure</option>
                    <option value="2">2nd unit measure</option>
                    <option value="3">3rd unit measure</option>
                    <option value="4">4st unit measure</option>
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
