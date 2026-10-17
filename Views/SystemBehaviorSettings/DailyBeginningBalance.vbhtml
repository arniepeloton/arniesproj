<script>
    require([GetPathFormat('/Scripts/sources/SystemBehaviorSettings/DailyBeginningBalance.js?v=' + assetversion)]);
</script>

<div class="container-fluid">
    <h1 class="display-6 mb-4">Daily Beginning and Ending Balance Behavior</h1>

    <div class="row mb-3">
        <label class="col-sm-3 mb-3" for="selDailyBeginningBalance">Save Daily Beginning and Ending Balance?</label>
        <div class="col-sm-9">
            <div class="form-check form-switch mb-3" style="font-size:20px;">
                <input class="form-check-input" data-role="none" type="checkbox" id="selDailyBeginningBalance">
            </div>
        </div>
    </div>

    <div class="mt-4" style="text-align: center">
        <a id="btnDone" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
        <a id="btnSaveDailyBeginningBalanceSettings" data-role="none" class="btn btn-success btn-lg">Save</a>
    </div>
</div>