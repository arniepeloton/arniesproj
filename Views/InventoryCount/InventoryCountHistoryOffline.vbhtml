<script>
    require([GetPathFormat('/Scripts/sources/InventoryCount/InventoryCountHistoryOffline.js?crc=' + assetversion)]);
</script>

<div id="pageInventoryReport">
    <div class="text-right mb-3">
        <button id="btnRefreshILAPHistory" style="margin-right: 10px; display: block;" data-role="none" class="btn btn-success">
            Refresh
        </button>
    </div>
    <h6 class="mb-3"><b><span class="required">*</span> History of all approved/disapproved item(s) of <span id="txtILADHistoryFowstName"></span></b></h6>
    <div class="table-responsive">
        <table data-role="none" class="table table-light table-striped table-hover headerfitwidth">
            <thead>
                <tr id="tblILADHistoryHeader" class="custom-theme-f">
                    <th data-sort="float" style="cursor:pointer;">#</th>
                    <th data-sort="string" style="cursor:pointer;">FOWST</th>
                    <th data-sort="string" style="cursor:pointer;">Bin</th>
                    <th data-sort="date" style="cursor:pointer;">Inventory Count Date</th>
                    <th data-sort="string" style="cursor:pointer;">Product Name</th>
                    <th data-sort="float" style="cursor:pointer;">Observed Count</th>
                    <th data-sort="string" style="cursor:pointer;">Unit Measure</th>
                    <th data-sort="accounting" style="cursor:pointer;">Variance</th>
                    <th data-sort="string" style="cursor:pointer;">Status</th>
                    <th data-sort="string" style="cursor:pointer;">Modified By</th>
                    <th data-sort="date" style="cursor:pointer;">Modified Date & Time</th>
                    @*<th style="cursor:pointer;"></th>*@
                </tr>
            </thead>
            <tbody id="tblILADHistoryBody2"></tbody>
        </table>
    </div>
</div>