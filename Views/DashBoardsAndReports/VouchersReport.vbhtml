<script>
    require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/VouchersReport.js?v=' + assetversion)]);
</script>

<div>
    <div class="row mb-3 gy-2 gx-3 align-items-center">
        <label class="col-sm-auto" for="selVRStatus">Status:</label>
        <div class="col-sm-auto">
            <select data-role="none" class="form-select" id="selVRStatus">
                <option value="All">All</option>
                <option value="Redeemed" selected>Redeemed</option>
                <option value="Unredeemed">Unredeemed</option>
                @*<option value="Expired">Expired</option>*@
            </select>
        </div>
        <div class="col-sm">
            <input data-role="none" class="form-control" placeholder="Search here" id="txtVRSearch" />
        </div>
        <div class="col-sm-auto">
            <button data-role="none" class="btn btn-secondary" id="btnVRDownload">Export to CSV</button>
        </div>
    </div>

    <table id="tblVouchersReport" data-role="none" class="table table-light table-hover table-striped">
        <thead>
            <tr class="text-center align-top" style="font-size:14px;">
                <th class="fixed-col" style="width:50px;">#</th>
                <th class="fixed-col">Date</th>
                <th class="fixed-col">Voucher Code</th>
                <th class="fixed-col">Type</th>
                <th class="fixed-col">Status</th>
                <th class="fixed-col">Region</th>
                <th class="fixed-col">Store Name</th>
                <th class="fixed-col">Store Code</th>
                @*<th class="fixed-col">Supercard Number</th>*@
            </tr>
        </thead>
        <tbody></tbody>
    </table>
</div>