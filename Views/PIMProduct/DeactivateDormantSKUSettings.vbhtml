<script>
    require([GetPathFormat('/Scripts/sources/PIMProduct/DeactivateDormantSKUSettings.js?v=' + assetversion)]);
</script>

<div class="container-fluid">
    <h1 class="display-6 mb-4">Settings for Deactivating Dormant SKUs</h1>

    <div class="row mb-3">
        <label class="col-sm-2 mb-3 col-form-label" for="txtRegularDiscount3">Dormant Period:</label>
        <div class="col-sm-10">
            <select data-role="none" id="selDormantPeriod" class="form-select form-select-lg mb-3">
                <option value="180">At least 180 Days or Approximately 1/2 Year</option>
                <option value="360">At least 360 Days or Approximately 1 Year</option>
                <option value="540">At least 540 Days or Approximately 1 & 1/2 Years</option>
                <option value="720">At least 720 Days or Approximately 2 Years</option>
                <option value="900">At least 900 Days or Approximately 2 & 1/2 Years</option>
                <option value="1080">At least 1,080 Days or Approximately 3 Years</option>
            </select>
            <div>
                This software tool flags (and can auto deactivate) dormant SKUs to lighten your database queries.
                <br />
                SKUs without any movement for at least the number of days you specify will be flagged for deactivation.
            </div>
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-2 mb-3 col-form-label" for="txtRegularDiscount3">Auto Clean Up:</label>
        <div class="col-sm-10">
            <div class="form-check form-switch mb-3" style="font-size:25px;">
                <input class="form-check-input" data-role="none" type="checkbox" id="selDormantSKUsIsAutoCleanUp">
                <label class="form-check-label" for="selSlider"></label>
            </div>
        </div>
    </div>

    <div class="mt-4" style="text-align: center">
        <a id="btnCancelDormant" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
        <a id="btnSaveDormant" data-role="none" class="btn btn-success btn-lg">Save</a>
    </div>
</div>