<script>
    require([GetPathFormat('/Scripts/sources/Device/DeviceInfoSummary.js?v=' + assetversion)]);
</script>

<div id="pageSumDevice" class="">

    @*<div id"divHeader" data-role="header">
            <h1>Device Information Summary</h1>
            <a href="#" data-icon="arrow-l" id="btnBack" style="display:none;">Back</a>
        </div>*@

    <div>
        <div style="display:none;">
            <input type="text" data-role="none" class="form-control" readonly id="txtSumDeviceID" />
        </div>

        <div class="row mb-4">
            <div class="col-md">
                <h1 class="display-6">Device Information Summary</h1>
            </div>
            <div class="col-md-auto text-right">
                <button data-role="none" id="butResetSalesHistory" class="btn btn-outline-success">Start Encoding Actual Transactions</button>
                <button data-role="none" id="btnEditDevice" class="btn btn-success">Edit Info</button>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtSumDevicePhoneName">Phone Name:</label>
            <div class="col-md-10">
                <input type="text" data-role="none" class="form-control" id="txtSumDevicePhoneName" readonly />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtSumDevicePhoneModel">Phone Model:</label>
            <div class="col-md-10">
                <input type="text" data-role="none" class="form-control" id="txtSumDevicePhoneModel" readonly />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtSumDeviceIMEI">IMEI:</label>
            <div class="col-md-10">
                <input type="text" data-role="none" class="form-control" id="txtSumDeviceIMEI" readonly />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtSumDeviceUUID">UUID:</label>
            <div class="col-md-10">
                <input type="text" data-role="none" class="form-control" id="txtSumDeviceUUID" readonly />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtSumBusinessName">Business Trade Name to display in the receipt:</label>
            <div class="col-md-10">
                <input type="text" data-role="none" class="form-control" id="txtSumBusinessName" readonly />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtSumBusinessRegisteredName">Business Registered Name to display in the receipt:</label>
            <div class="col-md-10">
                <input type="text" data-role="none" class="form-control" id="txtSumBusinessRegisteredName" readonly />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtSumDeviceUuidDisplay">Serial number to display in the receipt:</label>
            <div class="col-md-10">
                <input type="text" data-role="none" class="form-control" id="txtSumDeviceUuidDisplay" readonly />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtSumDeviceTinDisplay">TIN Branch Code to display in the receipt:</label>
            <div class="col-md-10">
                <input type="text" data-role="none" class="form-control" id="txtSumDeviceTinDisplay" readonly />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtSumDeviceOS">OS:</label>
            <div class="col-md-10">
                <input type="text" data-role="none" class="form-control" id="txtSumDeviceOS" readonly />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtSumDeviceNo">Device No.:</label>
            <div class="col-md-10">
                <input type="text" data-role="none" class="form-control" id="txtSumDeviceNo" readonly />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtSumMobizAppVersion">Mobiz App Version:</label>
            <div class="col-md-10">
                <input type="text" data-role="none" class="form-control" id="txtSumMobizAppVersion" readonly />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtSumAccreditation">BIR Accreditation:</label>
            <div class="col-md-10">
                <input type="text" data-role="none" class="form-control" id="txtSumAccreditation" readonly />
            </div>
        </div>
    </div>

    <div id="popupResetConfirmationPassword" data-bs-backdrop="true" class="modal fade" style="display:none;">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Security Dialog</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <p>By clicking this button, you will encode an actual transaction using this device, as you have obtained your Permit to Use (PTU) from the BIR.</p>
                    <p>Please note that this action can be performed only once because it will reset the transaction numbering to 1, signifying the start of actual transactions.</p>
                    <p>For security purposes, this action requires assistance from the Mobiz Team. The Mobiz Team must enter their password below to proceed.</p>

                    <input data-role="none" type="password" id="txtResetPassword" class="form-control form-control-lg" placeholder="Enter password here" />
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col">
                            <button data-role="none" id="btnProceedDeviceReset" class="btn btn-success btn-block btn-lg">Proceed</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div>
        <div class="row mb-4">
            <div class="col-md-6">
                <h1 class="display-6">Places (FOWST)</h1>
            </div>
            <div class="col-md-6 text-right">
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