<script>
    require([GetPathFormat('/Scripts/sources/SystemBehaviorSettings/SalesOrder.js?v=' + assetversion)]);
</script>

<div class="container-fluid">
    <h1 class="display-6 mb-4">Sales Order Behavior</h1>

    <div class="row mb-3">
        <label class="col-sm-3 mb-3 col-form-label" for="selEnableInventoryCountMode">Enable Inventory Count Mode:</label>
        <div class="col-sm-9">
            <div class="form-check form-switch mb-3" style="font-size:25px;">
                <input class="form-check-input" data-role="none" type="checkbox" id="selEnableInventoryCountMode">
            </div>
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-3 mb-3 col-form-label" for="selRequireSignature">Require Signature:</label>
        <div class="col-sm-9">
            <div class="form-check form-switch mb-3" style="font-size:25px;">
                <input class="form-check-input" data-role="none" type="checkbox" id="selRequireSignature">
            </div>
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-3 mb-3 col-form-label" for="selRequireCustomerOutlet">Require Customer Outlet:</label>
        <div class="col-sm-9">
            <div class="form-check form-switch mb-3" style="font-size:25px;">
                <input class="form-check-input" data-role="none" type="checkbox" id="selRequireCustomerOutlet">
            </div>
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-3 mb-3 col-form-label" for="selRequireCustomerDepartment">Require Customer Department:</label>
        <div class="col-sm-9">
            <div class="form-check form-switch mb-3" style="font-size:25px;">
                <input class="form-check-input" data-role="none" type="checkbox" id="selRequireCustomerDepartment">
            </div>
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-3 mb-3 col-form-label" for="selShowQtyAvailableToCommit">Show Qty. Available To Commit:</label>
        <div class="col-sm-9">
            <div class="form-check form-switch mb-3" style="font-size:25px;">
                <input class="form-check-input" data-role="none" type="checkbox" id="selShowQtyAvailableToCommit">
            </div>
        </div>
    </div>

    <div class="mt-4" style="text-align: center">
        <a id="btnDone" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
        <a id="btnSaveSalesOrderSetting" data-role="none" class="btn btn-success btn-lg">Save</a>
    </div>         
</div>