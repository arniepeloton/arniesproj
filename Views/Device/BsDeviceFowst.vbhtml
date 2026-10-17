<script>
    require([GetPathFormat('/Scripts/sources/Device/DeviceFowst.js?crc='),
            GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc=')
            ]);
</script>

<div id="pageDeviceFowst" class="container-fluid" style="display:block">

    @*<div id"divHeader" data-role="header">
        <h1>FOWST</h1>
    </div>*@
    <div class="row mb-4">
        <div class="col-6">
            <h1 class="display-6">List of FOWST</h1>
        </div>
        <div class="col-6 text-right">
            <button data-role="none" id="btnAddDeviceFOWST" class="btn btn-success">Add a Place</button>
        </div>
    </div>

    <div class="mb-3">
        * Places where this device can be used
    </div>

    <ul id="lstDeviceFowst" class="list-group mb-4">
        <li class="list-group-item">No result(s) found</li>
    </ul>

    <div style="text-align: center">
        <a id="btnDeviceFowstDone" data-role="none" class="btn btn-secondary btn-lg">Done</a>
    </div>
</div>

<div id="pageDeviceFowstInfo" class="container-fluid" style="display:none">
    <div>
        <h1 class="display-6 mb-4">Device-Fowst Info</h1>

        <div style="display:none;">
            <input type="text" data-role="none" class="form-control" readonly id="txtDeviceFowstID" />
            <input type="text" data-role="none" class="form-control" readonly id="txtFowstID" />
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="btnFOWST">Device FOWST Assignment: <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <button id="btnFOWST" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
            </div>
        </div>

        <div class="row mb-3" id="divPrinterFowstName">
            <label class="col-sm-2 mb-3 col-form-label" for="selPrinterFowstName">Printer Name</label>
            <div class="col-sm-10">
                <select id="selPrinterFowstName" data-role="none" class="form-select"></select>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtTerminalNo">Terminal No:</label>
            <div class="col-sm-10">
                <input data-role="none" class="form-control" type="text" readonly id="txtTerminalNo" />
            </div>
        </div>

        @*<div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtPrintServer">Print Server IP address or URL:</label>
                <div class="col-sm-10">
                    <input data-role="none" class="form-control" type="text" id="txtPrintServer" />
                </div>
            </div>*@

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="selIsDeviceAuthorizedofBIR">Is this device authorized for P.O.S use of the B.I.R?</label>
            <div class="col-sm-10">
                <select id="selIsDeviceAuthorizedofBIR" data-role="none" class="form-select">
                    <option value="0">No</option>
                    <option value="1" selected="selected">Yes</option>
                </select>
            </div>
        </div>

        <div id="dvBIRInfo">
            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtPOSRegNo">POS Registration No.: <span style="color: #FF0000;">*</span></label>
                <div class="col-sm-10">
                    <input data-role="none" class="form-control" type="text" id="txtPOSRegNo" />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtDevFowstBIRMachineNo">Machine Identification No. Issued by BIR: <span style="color: #FF0000;">*</span></label>
                <div class="col-sm-10">
                    <input data-role="none" class="form-control" type="text" id="txtDevFowstBIRMachineNo" />
                </div>
            </div>
        </div>

        <div id="dvNoneBIRInfo" style="visibility:hidden; display:none">
            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtDevFowstBIRMachineNo">Please state reason for non-registration</label>
                <div class="col-sm-10">
                    <div class="form-check">
                        <input class="form-check-input" type="radio" name="NoneRegistrationReason" id="NoneRegistrationReason1" checked>
                        <label class="form-check-label" for="NoneRegistrationReason1">
                            Do not want to register at this time
                        </label>
                    </div>

                    <div class="form-check">
                        <input class="form-check-input" type="radio" name="NoneRegistrationReason" id="NoneRegistrationReason2">
                        <label class="form-check-label" for="NoneRegistrationReason2">
                            POS registration is in progress
                        </label>
                    </div>
                </div>
            </div>

            <div id="dvRegistrationInProgress" style="visibility:hidden; display:none">
                <div class="row mb-3">
                    <label class="col-sm-2 mb-3 col-form-label" for="txtDevFowstBIRApplicationReferenceNo">Application Reference Number: <span style="color: #FF0000;">*</span></label>
                    <div class="col-sm-10">
                        <input data-role="none" class="form-control" type="text" id="txtDevFowstBIRApplicationReferenceNo" />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 mb-3 col-form-label" for="selDevFowstBIRApplicationDateMonth">Application Reference Number: <span style="color: #FF0000;">*</span></label>
                    <div class="col-sm-1">
                        <select id="selDevFowstBIRApplicationDateMonth" data-role="none" class="form-select">
                            <option value="">MMM</option>
                        </select>
                    </div>
                    <div class="col-sm-1">
                        <select id="selDevFowstBIRApplicationDateDay" data-role="none" class="form-select">
                            <option value="">DD</option>
                        </select>
                    </div>
                    <div class="col-sm-1">
                        <select id="selDevFowstBIRApplicationBalDateYear" data-role="none" class="form-select">
                            <option value="">YYYY</option>
                        </select>
                    </div>
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="selIsPrintUnOfficialReceipt">Print UnOfficial receipt?</label>
                <div class="col-sm-10">
                    <select id="selIsPrintUnOfficialReceipt" data-role="none" class="form-select">
                        <option value="0">No</option>
                        <option value="1" selected="selected">Yes</option>
                    </select>
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="selIsShowCompanyHeader">Show company details on receipt header?</label>
                <div class="col-sm-10">
                    <select id="selIsShowCompanyHeader" data-role="none" class="form-select">
                        <option value="0">No</option>
                        <option value="1" selected="selected">Yes</option>
                    </select>
                </div>
            </div>
        </div>

        @*<div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtDevFowstPrinterIP">Device Printer IP:</label>
                <div class="col-sm-10">
                    <input data-role="none" class="form-control" type="text" id="txtDevFowstPrinterIP" />
                </div>
            </div>*@

        <div class="row mb-3" id="divSMMachineNo" style="display:none;">
            <label class="col-sm-2 mb-3 col-form-label" for="txtDevFOWSTSMMachineNo">Machine No. (Assigned by SM Management):</label>
            <div class="col-sm-10">
                <input data-role="none" class="form-control" type="text" id="txtDevFOWSTSMMachineNo" />
            </div>
        </div>
    </div>

    <div>
        <h1 class="display-6 mb-4">Beginning Balance</h1>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="selDevFOWSTBegBalDateMonth">Sales Beginning Balance Date:</label>
            <div class="col-sm-1">
                <select id="selDevFOWSTBegBalDateMonth" data-role="none" class="form-select">
                    <option value="">MMM</option>
                </select>
            </div>
            <div class="col-sm-1">
                <select id="selDevFOWSTBegBalDateDay" data-role="none" class="form-select">
                    <option value="">DD</option>
                </select>
            </div>
            <div class="col-sm-1">
                <select id="selDevFOWSTBegBalDateYear" data-role="none" class="form-select">
                    <option value="">YYYY</option>
                </select>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtDevFOWSTBegBalance">Sales Beginning Balance :</label>
            <div class="col-sm-10">
                <input data-role="none" class="form-control" type="number" id="txtDevFOWSTBegBalance" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtDevFOWSTInvoiceStartNo">Last Used Invoice No. :</label>
            <div class="col-sm-10">
                <input data-role="none" class="form-control" type="number" id="txtDevFOWSTInvoiceStartNo" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtDevFOWSTSRVStartNo">Last Used SRV No. :</label>
            <div class="col-sm-10">
                <input data-role="none" class="form-control" type="number" id="txtDevFOWSTSRVStartNo" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtDevFOWSTRefStartNo">Last Used Refund No. :</label>
            <div class="col-sm-10">
                <input data-role="none" class="form-control" type="number" id="txtDevFOWSTRefStartNo" />
            </div>
        </div>
    </div>

    <div class="mb-3" style="text-align: center">
        <a id="btnDevFowstCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
        <a id="btnDevFowstSave" data-role="none" class="btn btn-success btn-lg">Save</a>
    </div>
</div>