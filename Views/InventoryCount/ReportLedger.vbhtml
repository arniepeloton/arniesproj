@Code
    ViewData("Title") = "Product Inventory Ledger"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/InventoryCount/InventoryProductLedger.js?crc=')]);
    </script>

End Section    

<div>

    <div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">
        @*<div data-role="fieldcontain" id="dvIsResaleable">
            <fieldset data-role="controlgroup">
                <legend> List of Inventory Counted within: </legend>
                <label id="lblWithin"> </label>
            </fieldset>
        </div>*@

        <table data-role="table" id="tblProductLedger" data-mode="reflow" data-role="table" class="ui-body-d ui-shadow table-stripe ui-responsive" data-column-btn-theme="b">
            <thead>
                <tr>
                    <th>Product Name </th>
                    <th>BIN </th>
                    <th>Quantity</th>
                    <th>Balance</th>
                    <th>Last Updated</th>
                    <th>Source</th>
                </tr>
            </thead>
            <tbody id='tblProductLedgerBody'></tbody>
        </table>
    </div>
</div>