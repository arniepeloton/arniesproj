<script>
    require([GetPathFormat('/Scripts/sources/SystemBehaviorSettings/PrintSettings.js?crc=' + assetversion)]);
</script>

<div class="container-fluid">
    <div class="row mb-3">
        <label class="col-sm-3 mb-3" for="selIsAutoPrintOrders">Auto-Print Orders that are ready for invoicing?</label>
        <div class="col-sm-9">
            <div class="form-check form-switch mb-3" style="font-size:20px;">
                <input class="form-check-input" data-role="none" type="checkbox" id="selIsAutoPrintOrders">
            </div>
        </div>
    </div>
    
    <h5 class="mb-4">List of Invoice Types</h5>

    <table data-role="none" class="table table-striped table-hover display dt-responsive">
        <thead>
            <tr>
                <th style="width:50px;">#</th>
                <th class="txt-center"> Document Name </th>
                <th class="txt-center"> Module Source </th>
                <th class="txt-center"> Print Limit </th>
                <th class="txt-center"> No. of Copies per Print </th>
                <th class="txt-center"> Label of Copies </th>
                <th class="txt-center"> Active </th>
                <th class="txt-center"> Print Automatically </th>
            </tr>
        </thead>
        <tbody id="tblBodyAutoPrintInvoice"></tbody>
    </table>

    <div class="mt-4" style="text-align: center">
        <a id="btnPrintSettingsDone" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
        <a id="btnSavePrintSettings" data-role="none" class="btn btn-success btn-lg">Save</a>
    </div>
</div>