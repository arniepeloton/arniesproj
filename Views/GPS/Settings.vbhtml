@*@Code
        ViewData("Title") = "Discount Settings"
        Layout = "~/Views/Shared/Layout3.vbhtml"
    End Code*@
<script>
    require([GetPathFormat('/Scripts/sources/GPS/Settings.js?v=' + assetversion)]);
</script>

<div data-role="content">
    <div class="row mb-3">
        <label class="col-sm-3 mb-3" for="selIsRequireGPS">Require GPS while Logging In and Out in DTR:</label>
        <div class="col-sm-9">
            <div class="form-check form-switch mb-3" style="font-size:20px;">
                <input class="form-check-input" data-role="none" type="checkbox" id="selIsRequireGPS">
            </div>
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-3 mb-3" for="selIsRequireGPS">Interval in Getting GPS Coordinates. Every</label>
        <div class="col-sm-3">
            <input id="txtGPSInterval" type="number" data-role="none" class="form-control" />
        </div>
        <div class="col-sm-2">
            Minutes
        </div>
    </div>

    <div class="mt-4" style="text-align: center">
        <a id="btnGPSSettingsBack" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
        <a id="butSaveGPSSettings" data-role="none" class="btn btn-success btn-lg">Save</a>
    </div>
</div>