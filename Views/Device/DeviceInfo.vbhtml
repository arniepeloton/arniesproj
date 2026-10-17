<style>
    .ui-icon-barcode {
        background-image: url("../Content/images/barcode.ico");
        background-repeat: no-repeat;
        background-position: center center;
        background-size: 50px 50px;
    }
</style>


<script>
    document.addEventListener('deviceready', this.onDeviceReady, false);
    require([GetPathFormat('/Scripts/sources/Device/DeviceInfo.js?v=' + assetversion)]);
</script>

@*<div id="pageOrganization" data-role="page">

        <div id"divHeader" data-role="header">
            <h1>Data Fields for Device Enrolment</h1>
        </div>

    </div>*@

<div class="">
    <div style="display:none;">
        <input type="text" data-role="none" class="form-control" id="txtID" />
    </div>

    <div class="row mb-4">
        <div class="col-12 text-right">
            <button data-role="none" id="btnRetrieveDeviceInfoFromQR" class="btn btn-success">Register Device using QR Code</button>
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-md-2 col-form-label" for="txtPhoneName">Phone Name:</label>
        <div class="col-md-10">
            <input type="text" data-role="none" class="form-control" id="txtPhoneName" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-md-2 col-form-label" for="txtPhoneModel">Phone Model:</label>
        <div class="col-md-10">
            <input type="text" data-role="none" class="form-control" id="txtPhoneModel" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-md-2 col-form-label" for="txtIMEI">IMEI:</label>
        <div class="col-md-10">
            <div class="input-group">
                <input type="text" data-role="none" class="form-control" id="txtIMEI" />
                <button data-role="none" id="btnScanIMEI" class="btn btn-secondary"><span class="bi-upc"></span></button>
            </div>
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-md-2 col-form-label" for="txtUUID">UUID:</label>
        <div class="col-md-10">
            <div class="input-group">
                <input type="text" data-role="none" class="form-control" id="txtUUID" />
                <button data-role="none" id="btnScanUUID" class="btn btn-secondary"><span class="bi-upc"></span></button>
            </div>
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-md-2 col-form-label" for="txtBusinessName">Business Trade Name to display in the receipt:</label>
        <div class="col-md-10">
            <input type="text" data-role="none" class="form-control" id="txtBusinessName" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-md-2 col-form-label" for="txtBusinessRegisteredName">Business Registered Name to display in the receipt:</label>
        <div class="col-md-10">
            <input type="text" data-role="none" class="form-control" id="txtBusinessRegisteredName" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-md-2 col-form-label" for="txtUuidDisplay">Serial number to display in the receipt:</label>
        <div class="col-md-10">
            <input type="text" data-role="none" class="form-control" id="txtUuidDisplay" placeholder="Leave this blank to display the device's original serial number" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-md-2 col-form-label" for="txtTinDisplay">TIN Branch Code to display in the receipt:</label>
        <div class="col-md-10">
            <input type="text" data-role="none" class="form-control" id="txtTinDisplay" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-md-2 col-form-label" for="txtOS">Platform (OS):</label>
        <div class="col-md-10">
            <input type="text" data-role="none" class="form-control" id="txtOS" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-md-2 col-form-label" for="txtDeviceNo">Device No. (System Generated):</label>
        <div class="col-md-10">
            <input type="text" data-role="none" class="form-control" id="txtSumDeviceNo" readonly />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-md-2 col-form-label" for="selMobizAppVersion">Mobiz App Version:</label>
        <div class="col-md-10">
            <select id="selMobizAppVersion" data-role="none" class="form-select">
                <option value="1">1</option>
                <option value="2">2</option>
            </select>
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-md-2 col-form-label" for="selAccreditation">BIR Accreditation:</label>
        <div class="col-md-10">
            <select id="selAccreditation" data-role="none" class="form-select">
                <option value="mobiz">1MOBIZ, INC.</option>
                @*<option value="duc">Davao Unified Corporation</option>*@
            </select>
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-md-2 col-form-label" for="selDeviceDeactivate">Active Device ?</label>
        <div class="col-md-10">
            <select id="selDeviceDeactivate" data-role="none" class="form-select">
                <option value="0">No</option>
                <option value="1">Yes</option>
            </select>
        </div>
    </div>

    <div class="mb-3" style="text-align: center">
        <a id="btnDeviceInfoCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
        <a id="btnDeviceInfoDone" data-role="none" class="btn btn-success btn-lg">Save</a>
    </div>
</div>