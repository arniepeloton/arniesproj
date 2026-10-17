<script>
    require([GetPathFormat('/Scripts/sources/Device/DeviceInfoSummary.js?crc=')]);
    </script>

<div id="pageSumDevice" class="container-fluid">

    @*<div id"divHeader" data-role="header">
        <h1>Device Information Summary</h1>
        <a href="#" data-icon="arrow-l" id="btnBack" style="display:none;">Back</a>
    </div>*@

    <div>
        <div style="display:none;">
            <input type="text" data-role="none" class="form-control" readonly id="txtSumDeviceID" />
        </div>

        <div class="row mb-4">
            <div class="col-6">
                <h1 class="display-6">Device Information Summary</h1>
            </div>
            <div class="col-6 text-right">
                <button data-role="none" id="btnEditDevice" class="btn btn-success">Edit Info</button>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtSumDevicePhoneName">Phone Name:</label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtSumDevicePhoneName" readonly />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtSumDevicePhoneModel">Phone Model:</label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtSumDevicePhoneModel" readonly />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtSumDeviceIMEI">IMEI:</label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtSumDeviceIMEI" readonly />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtSumDeviceUUID">UUID:</label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtSumDeviceUUID" readonly />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtSumDeviceUuidDisplay">Serial Number to Display on Receipt:</label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtSumDeviceUuidDisplay" readonly />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtSumDeviceOS">OS:</label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtSumDeviceOS" readonly />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtSumDeviceNo">Device No.:</label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtSumDeviceNo" readonly />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtSumAccreditation">BIR Accreditation:</label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtSumAccreditation" readonly />
            </div>
        </div>
    </div>    

    <div>
        <div class="row mb-4">
            <div class="col-6">
                <h1 class="display-6">Places (FOWST)</h1>
            </div>
            <div class="col-6 text-right">
                <button data-role="none" id="btnEditDeviceFowst" class="btn btn-success">Edit Device FOWST</button>
            </div>
        </div>

        <ul id="lstSumDeviceFowst" class="list-group mb-3">
            <li class="list-group-item">No result(s) found</li>
        </ul>
    </div>

    @*<div>
        <div class="row mb-4">
            <div class="col-6">
                <h1 class="display-6">Printer</h1>
            </div>
            <div class="col-6 text-right">
                <button data-role="none" id="btnEditDevicePrinter" class="btn btn-success">Edit Device Printer</button>
            </div>
        </div>

        <ul id="lstSumDevicePrinter" class="list-group mb-3">
            <li class="list-group-item">No result(s) found</li>
        </ul>
    </div>*@

    <div class="mt-4" style="text-align: center">
        <a id="btnDeviceSummaryBack" data-role="none" class="btn btn-secondary btn-lg">Done</a>
    </div>
</div>