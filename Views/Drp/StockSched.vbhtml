@Code
    ViewData("Title") = "Stock Schedule - Distribution Resource Planning"
    Layout = "Master.vbhtml"
End Code
    <script type="text/javascript">
    
    </script>
    @If ProWeb.Common.UseMinifiedJavascripts Then
        @<text>
        <script src="/Scripts/minified/StockSched.js" type="text/javascript"></script>
        </text>
    Else
        @<text>
        <script src="/Scripts/sources/Project/StockSched.js" type="text/javascript"></script>
        </text>
    End If
    <style type="text/css">
        .NegativeNumber
        {
            background-color: Red;
            color: White;
        }
        .SelectedSupplier
        {
            background-color: #cfe48b;
        }
    </style>
<div id="divStockSched" class="widgets">
    <div class="widget">
        <h3 class="handle">
            SKU's</h3>
        <div>
            <div name="divStocks"></div>
        </div>
    </div>
    <div name="divTables">
        <div>
            <span>Forecast horizon (number of weeks):</span><span>
                <input name="txtWeeksCount" type="number" title="1 to 30" style="width: 50px; margin-top:8px" value="@ViewData("txtWeeksCount")" /></span> <span><button name="butWeeksCount">Confirm</button></span>
        </div>
        <table class="TableForm" name="tableSched">
            <tr>
                <th>
                </th>
                <th name="thTopRow">
                    Past Due
                </th>
            </tr>
            <tr>
                <td>
                    <b>Gross Requirements</b>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    Advance Orders
                </td>
                <td name="tdAdvanceOrders">
                </td>
            </tr>
            <tr>
                <td>
                    Backorders
                </td>
                <td name="tdBackOrders">
                </td>
            </tr>
            <tr name="trForecast">
                <td>
                    Forecast
                </td>
                <td name="tdForecast">
                </td>
            </tr>
            <tr>
                <td>
                    <b>Total Gross Requirements</b>
                </td>
                <td name="tdGrossRequirements">
                </td>
            </tr>
            <tr>
                <td>
                    Scheduled Receipts
                </td>
                <td name="tdScheduledReceipts">
                </td>
            </tr>
            <tr>
                <td>
                    Projected Avaialble to Commit Qty
                </td>
                <td name="tdProjectedAvaialbletoCommitQty">
                </td>
            </tr>
            <tr>
                <td>
                    Planned Orders
                </td>
                <td>
                </td>
            </tr>
        </table>
    </div>
    <div class="widget">
        <h3 class="handle">
            SKU Details</h3>
        <div name="divDetails">
            <table class="TableForm" style="margin-bottom: 30px">
                <tr>
                    <td>
                        Stock Number:
                    </td>
                    <td>
                        <span name="divStockNunber"></span>
                    </td>
                </tr>
                <tr>
                    <td>
                        Stock Description:
                    </td>
                    <td>
                        <span name="divStockDescription"></span>
                    </td>
                </tr>
                <tr>
                    <td>
                        Stocks on hand:
                    </td>
                    <td>
                        <span name="spanStockOnHand"></span>
                    </td>
                </tr>
                <tr>
                    <td>
                        Date of last count approval:
                    </td>
                    <td>
                        <span name="spanDateLastApprovedCount"></span>
                    </td>
                </tr>
                <tr>
                    <td>
                        Safety stock:
                    </td>
                    <td>
                        <span name="spanSafetyStock">not yet implemented</span>
                    </td>
                </tr>
            </table>
            <table name="tableSupplier" class="TableForm" style="margin-bottom: 30px">
                <tr name="trSupplier">
                    <td>
                        Vendor:
                    </td>
                </tr>
                <tr name="trPrice">
                    <td>
                        Vendor price:
                    </td>
                </tr>
                <tr name="trLeadTime">
                    <td>
                        Vendor lead time (in days):
                    </td>
                </tr>
                <tr name="trMoq">
                    <td title="Minimum Order Quantity">
                        MOQ:
                    </td>
                </tr>
                <tr name="trMoqUnit">
                    <td title="Minimum Order Quantity Unit">
                        MOQ Unit:
                    </td>
                </tr>
                <tr name="trMoqTotal">
                    <td title="Minimum Order Quantity">
                        MOQ total:
                    </td>
                </tr>
                <tr name="trLotSize">
                    <td>
                        Lot size:
                    </td>
                </tr>
                <tr name="trLotSizeUnit">
                    <td>
                        Lot size unit:
                    </td>
                </tr>
                <tr name="trLotSizeTotal">
                    <td>
                        Lot size total:
                    </td>
                </tr>
            </table>
        </div>
    </div>
    
    <div name="divSupplierReason">
        <table class="TableForm">
            <tr>
                <td>
                    Reason for choosing this supplier:
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    Remarks:
                </td>
                <td>
                    <textarea></textarea>
                </td>
            </tr>
        </table>
    </div>
</div>
