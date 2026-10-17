@Code
    ViewData("Title") = "DRP Tables"
    Layout = "~/Views/Shared/Drp.vbhtml"
End Code
<style>
    .BoldText
    {
        font-size: 14pt;
    }
    .PpoValue
    {
        font-size: 12pt;
        font-weight: bold;
    }
</style>
<script>
    require(['Drp/DrpStockDetails'], function () {
        DrpStockDetails.StockID = '@ViewData("StockID")';
        DrpStockDetails.GetData();
    });
</script>
<div id="tabDrpPpo">
</div>
<div>
    <h2>@ViewData("Title")</h2>
    <button id="butDrpStockDetailsRefresh" data-dojo-type="dijit.form.Button" data-dojo-props="iconClass:'dijitIconNewTask'">
        Refresh</button>
</div>
<table class="BoldText">
    <tr>
        <td>
            Stock number:
        </td>
        <td>
            <span id="spanStockNumber"></span>
        </td>
    </tr>
    <tr>
        <td>
            Stock description:
        </td>
        <td>
            <span id="spanStockDescription"></span>
        </td>
    </tr>
</table>
<div id="divDrpStockDetailsGrid" style="width: 100%;">
</div>
<div style="height: 30px">
</div>
<table>
    <tr>
        <td>
            <table>
                <tr>
                    <td colspan="2" class="BoldText">
                        DRP Parameters
                    </td>
                </tr>
                <tr>
                    <td>
                        Planning horizon:
                    </td>
                    <td>
                        <span id="spanPlanningHorizon"></span>week[s]
                    </td>
                </tr>
                <tr>
                    <td>
                        Safety stock duration:
                    </td>
                    <td>
                        <span id="spanSafetyStockDuration"></span>week[s]
                    </td>
                </tr>
                <tr>
                    <td>
                        Shipper
                    </td>
                    <td>
                        <span id="spanShipperName"></span>
                    </td>
                </tr>
                <tr>
                    <td>
                        Qty on hand:
                    </td>
                    <td>
                        <span id="spanQtyOnHand"></span>
                    </td>
                </tr>
                <tr>
                    <td>
                        Qty committed:
                    </td>
                    <td>
                        <span id="spanQtyCommitted"></span>
                    </td>
                </tr>
                <tr>
                    <td>
                        Safety stock:
                    </td>
                    <td>
                        <span id="spanSafetyStock"></span>
                    </td>
                </tr>
                <tr>
                    <td>
                        Qty available to commit:
                    </td>
                    <td>
                        <span id="spanQtyAvailableToCommit"></span>
                    </td>
                </tr>
            </table>
        </td>
        <td style="padding-left:30px">
            <table>
                <tr>
                    <td>
                        Product lead time:
                    </td>
                    <td>
                        <span id="spanProductLeadTime">not yet implemented</span>
                    </td>
                </tr>
                <tr>
                    <td>
                        Vendor lead time:
                    </td>
                    <td>
                        <span id="spanVendortLeadTime"></span>
                    </td>
                </tr>
                <tr>
                    <td>
                        Shipping lead time:
                    </td>
                    <td>
                        <span id="spanShippingLeadTime"></span>
                    </td>
                </tr>
                <tr>
                    <td>
                        PO processing lead time:
                    </td>
                    <td>
                        <span id="spanPoProcessingLeadTime"></span>
                    </td>
                </tr>
                <tr>
                    <td>
                        Total lead time:
                    </td>
                    <td>
                        <b><span id="spanTotalLeadTime"></span></b>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<div style="height: 30px">
</div>
<div class="BoldText">
    Vendors and Shippers</div>
<div id="divDrpDetailsVendorsGrid">
</div>
