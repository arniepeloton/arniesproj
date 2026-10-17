<script>
    require([GetPathFormat('/Scripts/sources/DevicePrinter/DevicePrinterSummary.js?v=' + assetversion)]);
</script>

<div id="pageSumDevicePrinter">
    <div class="row mb-3 align-items-center">
        <div class="col-6 mb-3">
            <h1 class="display-6">Printer Information</h1>
        </div>
        <div class="col-6 text-right">
            <button id="btnEditDevicePrinter" data-role="none" class="btn btn-success">Edit</button>
        </div>
    </div>

    <div data-role="fieldcontain" style="display:none;">
        <input type="text" readonly id="txtSumDevicePrinterID" />
    </div>

    <div class="row mb-3">
        <label class="col-sm-2 mb-3 col-form-label" for="txtSumDevicePrinterRole">Printer Role:</label>
        <div class="col-sm-10">
            <input type="text" id="txtSumDevicePrinterRole" data-role="none" class="form-control" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-2 mb-3 col-form-label" for="txtSumDevicePrinterType">Printer Type:</label>
        <div class="col-sm-10">
            <input type="text" id="txtSumDevicePrinterType" data-role="none" class="form-control" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-2 mb-3 col-form-label" for="chkSumIsActive">Is Active?</label>
        <div class="col-sm-10">
            <div class="form-check form-switch mb-3" style="font-size:20px;">
                <input class="form-check-input" data-role="none" type="checkbox" id="chkSumIsActive">
            </div>
        </div>
    </div>    

    <h5 class="mb-3">Categories to Print</h5>

    <div data-role="none" id="lstSumDevicePrinterCategories" class="list-group mb-3"></div>    

    <div class="mt-4 text-center">
        <a id="btnDevicePrinterSummaryBack" data-role="none" class="btn btn-secondary btn-lg">Done</a>
    </div>
</div>
