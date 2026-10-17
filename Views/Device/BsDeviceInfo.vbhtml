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
    require([GetPathFormat('/Scripts/sources/Device/DeviceInfo.js?crc=')]);        
</script>

@*<div id="pageOrganization" data-role="page">

    <div id"divHeader" data-role="header">
        <h1>Data Fields for Device Enrolment</h1>
    </div>

</div>*@

<div class="container-fluid">
    <div style="display:none;">
        <input type="text" data-role="none" class="form-control" id="txtID" />
    </div>

    <div class="row mb-4">
        <div class="col-12 text-right">
            <button data-role="none" id="btnRetrieveDeviceInfoFromQR" class="btn btn-success">Register Device using QR Code</button>
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-2 mb-3 col-form-label" for="txtPhoneName">Phone Name:</label>
        <div class="col-sm-10">
            <input type="text" data-role="none" class="form-control" id="txtPhoneName" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-2 mb-3 col-form-label" for="txtPhoneModel">Phone Model:</label>
        <div class="col-sm-10">
            <input type="text" data-role="none" class="form-control" id="txtPhoneModel" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-2 mb-3 col-form-label" for="txtIMEI">IMEI:</label>
        <div class="col-sm-9">
            <input type="text" data-role="none" class="form-control" id="txtIMEI" />
        </div>
        <div class="col-sm-1">
            <button data-role="none" id="btnScanIMEI" class="btn btn-light btn-block"><span class="bi-upc"></span></button>
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-2 mb-3 col-form-label" for="txtUUID">UUID:</label>
        <div class="col-sm-9">
            <input type="text" data-role="none" class="form-control" id="txtUUID" />
        </div>
        <div class="col-sm-1">
            <button data-role="none" id="btnScanUUID" class="btn btn-light btn-block"><span class="bi-upc"></span></button>
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-2 mb-3 col-form-label" for="txtSumDeviceUuidDisplay">Serial Number to Display on Receipt:</label>
        <div class="col-sm-9">
            <input type="text" data-role="none" class="form-control" id="txtUuidDisplay" placeholder="Leave this blank to display the device's original serial number" />
        </div>
        <div class="col-sm-1">
            <button data-role="none" id="btnScanUuidDisplay" class="btn btn-light btn-block"><span class="bi-upc"></span></button>
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-2 mb-3 col-form-label" for="txtOS">Platform (OS):</label>
        <div class="col-sm-10">
            <input type="text" data-role="none" class="form-control" id="txtOS" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-2 mb-3 col-form-label" for="txtDeviceNo">Device No. (System Generated):</label>
        <div class="col-sm-10">
            <input type="text" data-role="none" class="form-control" id="txtSumDeviceNo" readonly />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-2 mb-3 col-form-label" for="selAccreditation">BIR Accreditation:</label>
        <div class="col-sm-10">
            <select id="selAccreditation" data-role="none" class="form-select">
                <option value="mobiz">1MOBIZ, INC.</option>
                <option value="duc">Davao Unified Corporation</option>
            </select>
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-2 mb-3 col-form-label" for="selDeviceDeactivate">Active Device ?</label>
        <div class="col-sm-10">
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