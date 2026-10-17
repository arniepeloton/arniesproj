<script>
    require([GetPathFormat('/Scripts/sources/InventoryCount/InventoryCountApprovalOrDisapproval.js?crc=')]);
</script>

<div id="pageMain">

    <div data-role="content">
        <br /> <br />

            <div style="text-align:center;"> <a id="btnViewReport" data-role="button" data-icon="grid">View Report</a> </div>
            <div id="divInventoryReport" style="overflow:auto;">
                <table data-role="table" id="tblInventoryReport" data-mode="reflow" class="ui-body-d ui-shadow table-stripe ui-responsive custom-tbl-responsive" data-column-btn-theme="b">
                    <thead class="custom-theme-f">
                        <tr style="cursor: pointer">
                            @*<th data-sort="string">BIN</th>*@
                            <th data-sort="string">FOWST</th>
                            <th data-sort="string">Physical Count Date</th>
                            <th data-sort="string">No of Unapproved Count</th>
                            <th data-sort="string">Counted By</th>
                        </tr>
                    </thead>
                    <tbody id='tblInventoryReportBody'></tbody>
                </table>
            </div>


            <br />

        </div>

        <div id="divForInventoryLedger" style="display:none;">

        </div>

    </div>
</div>

<div id="pageProductLedger" data-role="page">
    <div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">
        <div data-role="fieldcontain" id="dvIsResaleable">
            <fieldset data-role="controlgroup">
                <legend> List of Inventory Counted within: </legend>
                <label id="lblWithin"> </label>
            </fieldset>
        </div>

        <table data-role="table" id="tblProductLedger" data-mode="reflow" class="ui-responsive">
            <thead>
                <tr>
                    <th>Product Name #</th>
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