<script>
    require([GetPathFormat('/Scripts/sources/InventoryCount/InventoryCountReport.js?crc=' + assetversion)]);
</script>

<div id="pageMain">

    <div data-role="content">


        <div id="divForPhysicalInventory" subpage="inventoryacc" style="display:block;">
            <div class="row">

                <div class="col-6 col-md-auto mb-3">
                    Date: <br />
                    <button id="btnInventoryCountDate" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                        <span class="lblInventoryCountDate"> Today </span>
                    </button>
                </div>

                <div class="col-6 col-md-auto mb-3">
                    FOWST: <br />
                    <button id="btnInventoryCountFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                        <span class="lblInventoryCountFOWST"> FOWST/Store </span>
                    </button>
                </div>

                @*<div class="col-sm-2">
                    List of Inventory Counted within:
                </div>
                <div class="col-sm-10">
                    <select id="selReportRange" data-role="none" class="form-control form-select">
                        <option value=""> -Select Option- </option>
                        <option value="optThisWeek"> This Week </option>
                        <option value="optLastWeek"> Last Week </option>
                        <option value="optLastTwoWeeks"> Last 2 Weeks </option>
                        <option value="optLastThreeWeeks"> Last 3 Weeks </option>
                        <option value="optLastMonth"> Last Month </option>
                    </select>
                </div>*@
            </div>
            @*<br />
            <div class="row">
                <div class="col-sm-2">
                    Select FOWST:
                </div>
                <div class="col-sm-10">
                    <select id="selFowst" data-role="none" class="form-control form-select"></select>
                </div>
            </div>*@
            @*<br />
            <div class="row">
                <div class="col-sm-offset-2 offset-sm-2 col-sm-10">
                    <button id="btnViewReport" data-role="none" class="btn custom-btn-theme-c btn-secondary btn-block">View Report</button>
                </div>
            </div>*@

            @*<div data-role="fieldcontain">
                    <label for="selFowst">Product :</label>
                    <input type="text" for="btnSearchItem" id="txtSearchItem" data-type="search" placeholder="Search products"/>
                </div>      *@
            <br /><br />
            <div id="divInventoryReport" style="overflow:auto;">
                <table data-role="table" id="tblInventoryReport" data-mode="reflow" class="ui-body-d ui-shadow table-stripe ui-responsive custom-tbl-responsive table table-light table-striped table-hover headerfitwidth" data-column-btn-theme="b">
                    <thead class="custom-theme-f">
                        <tr style="cursor: pointer">
                            @*<th data-sort="string">BIN</th>*@
                            <th data-sort="string">Product No</th>
                            <th data-sort="string">Product Group No</th>
                            <th data-sort="string">Category</th>
                            <th data-sort="string">Product Description</th>
                            <th data-sort="string">Unit Measure</th>
                            <th data-sort="float">Expected Balance (E)</th>
                            <th data-sort="float">Observed Physical Count (O)</th>
                            <th data-sort="string">Physical Count Date </th>
                            <th data-sort="float">Nominal Value of Variance (O-E)</th>
                            <th data-sort="float"><abbr title="Variance Percentage"> Variance in % [(O-E)/E]</abbr></th>
                            <th data-sort="float">Value (COGS) of Variance</th>
                            @*<th data-sort="string">Counted By</th>
                            <th data-sort="string">Turned Over to</th>
                            <th data-sort="string">Turned Over Date</th>
                            <th data-sort="string">Remarks</th>*@
                        </tr>
                    </thead>
                    <tbody id='tblInventoryReportBody'></tbody>
                </table>
            </div>


            <br />

            @*<table data-role="table" id="tblInventoryReportTotals" data-mode="reflow" class="ui-responsive">
                    <thead>
                        <tr>
                            <th>Total Value of All Variances</th>
                            <th>Total Value of Negative Variances</th>
                            <th>Total Value of Positive Variances</th>
                        </tr>
                    </thead>
                    <tbody id='tblInventoryReportTotalsBody'>
                    </tbody>
                </table>*@
        </div>

        <div id="divForInventoryLedger" subpage="inventoryacc" style="display:none;">

        </div>

    </div>
</div>

<div id="pageProductLedger" data-role="page" style="display:none;">
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
            <tbody id='tblProductLedgerBody'>
            </tbody>
        </table>
    </div>
</div>