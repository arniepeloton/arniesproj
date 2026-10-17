@*@Code
    ViewData("Title") = "Discount Settings"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code*@
<script>
    require([GetPathFormat('/Scripts/sources/DiscountSettings/DiscountSettingsInfo.js?v=' + assetversion)]);
</script>
<style id="4">
    .inline
    {
        display:inline;
        margin:5px 5px 5px 0px;
        vertical-align:top;
    }
    .ui-grid-b
    {
        margin-left: 15px;
    }
    .ui-grid-b .ui-block-a
    {
        width: 50%;
        margin-top: 5px;
        margin-bottom: 5px;
    }
    .ui-grid-b .ui-block-b
    {
        width: 40%;
        margin-top: 5px;
        margin-bottom: 5px;
    }
    .ui-grid-b .ui-block-c
    {
        width: 10%;
        margin-top: 5px;
        margin-bottom: 5px;
    }
</style>
<div class="container-fluid">
    <div class="row mb-3">
        <label class="col-sm-2 mb-3 col-form-label" for="txtSeniorCitizenDiscount">Senior Citizen Discount (%):</label>
        <div class="col-sm-10">
            <input type="number" data-role="none" class="form-control" id="txtSeniorCitizenDiscount" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-2 mb-3 col-form-label" for="txtPersonWithDisabilityDiscount">Person With Disability (PWD) Discount (%):</label>
        <div class="col-sm-10">
            <input type="number" data-role="none" class="form-control" id="txtPersonWithDisabilityDiscount" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-2 mb-3 col-form-label" for="txtPNSTMDiscount">Philippine National Sports Team Discount (%):</label>
        <div class="col-sm-10">
            <input type="number" data-role="none" class="form-control" id="txtPNSTMDiscount" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-2 mb-3 col-form-label" for="txtSoloParentDiscount">Solo Parent Discount Discount (%):</label>
        <div class="col-sm-10">
            <input type="number" data-role="none" class="form-control" id="txtSoloParentDiscount" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-2 mb-3 col-form-label" for="txtEmployeeDiscount">Employee Discount (%):</label>
        <div class="col-sm-10">
            <input type="number" data-role="none" class="form-control" id="txtEmployeeDiscount" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-2 mb-3 col-form-label" for="txtRegularDiscount1">Regular Discount 1 (%):</label>
        <div class="col-sm-10">
            <input type="number" data-role="none" class="form-control" id="txtRegularDiscount1" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-2 mb-3 col-form-label" for="txtRegularDiscount2">Regular Discount 2 (%):</label>
        <div class="col-sm-10">
            <input type="number" data-role="none" class="form-control" id="txtRegularDiscount2" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-2 mb-3 col-form-label" for="txtRegularDiscount3">Regular Discount 3 (%):</label>
        <div class="col-sm-10">
            <input type="number" data-role="none" class="form-control" id="txtRegularDiscount3" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-2 mb-3 col-form-label" for="txtVIPDiscount">VIP Discount (%):</label>
        <div class="col-sm-10">
            <input type="number" data-role="none" class="form-control" id="txtVIPDiscount" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-2 mb-3 col-form-label" for="txtGPCDiscount">GPC Discount (%):</label>
        <div class="col-sm-10">
            <input type="number" data-role="none" class="form-control" id="txtGPCDiscount" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-2 mb-3 col-form-label" for="txtGPCDiscount">Show Headcount Details (Applicable for SC/PWD discount):</label>
        <div class="col-sm-10">
            <input type="checkbox" data-role="none" class="form-check-input" id="chbHeadcount" checked />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-2 mb-3" for="txtPersonWithDisabilityDiscount">When applying a discount, should the system allow only one profile to be entered?</label>
        <div class="col-sm-10">
            <div class="form-check form-check-inline">
                <input class="form-check-input" type="radio" id="radYesProfile" name="radDiscountProfile" value="Yes">
                <label class="form-check-label" for="radYesProfile">Yes</label>
            </div>
            <div class="form-check form-check-inline">
                <input class="form-check-input" type="radio" id="radNoProfile" name="radDiscountProfile" value="No">
                <label class="form-check-label" for="radNoProfile">No</label>
            </div>
        </div>
    </div>

    <br />

    <div class="row align-items-center mb-4 d-none">
        <div class="col-sm-6">
            <h3>Custom Discounts</h3>
        </div>
        <div class="col-sm-6 text-right">
            <button data-role="none" class="btn btn-success" id="btnAddCD">Add Custom Discount</button>
        </div>
    </div>

    <table data-role="none" class="table table-light table-striped table-hover d-none">
        <thead>
            <tr>
                <th data-sort="string" style="cursor:pointer;">#</th>
                <th data-sort="string" style="cursor:pointer;">Discount Name</th>
                <th data-sort="string" style="cursor:pointer;">Discount Description</th>
                <th data-sort="string" style="cursor:pointer;">Minimum Purchase Amount</th>
                <th data-sort="string" style="cursor:pointer;">Discount</th>
                <th data-sort="string" style="cursor:pointer;">Status</th>
                <th data-sort="string" style="cursor:pointer;">Last Updated</th>
                <th data-sort="string" style="cursor:pointer;"></th>
            </tr>
        </thead>
        <tbody id="tbodyCustomDiscount"></tbody>
    </table>
    <br /><br />
    <h3 class="mb-4 d-none">Max Allowable Discountable Amount (VAT-Ex) Subject to SCD (PhP)</h3>

    <table data-role="none" class="table table-light table-striped table-hover d-none">
        <thead>
            <tr>
                <th data-sort="string" style="cursor:pointer;">Trade Name</th>
                <th data-sort="string" style="cursor:pointer;">Max Allowable Discountable Amount</th>
            </tr>
        </thead>
        <tbody id="tblMaxAllowableDiscountableAmount"></tbody>
    </table>

    <div class="mt-4" style="text-align: center">
        <a id="btnDiscountSettingsBack" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
        <a id="butSave" data-role="none" class="btn btn-success btn-lg">Save</a>
    </div>
</div>

<div id="panSetMaxAllowableDiscountableAmount" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Enter Max Allowable Discountable Amount <span class="lblPopUpHeaderLibraryName"></span></h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                Enter Amount (PhP):
                <br />
                <input type="number" data-role="none" class="form-control" id="txtMaxAllowableDiscountableAmount" />
            </div>
            <div class="modal-footer">
                <button data-role="none" type="button" id="btnSetMaxAllowableDiscountableAmount" class="btn btn-success btn-block">Set Max Allowable Discountable Amount</button>
                <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
            </div>
        </div>
    </div>
</div>

<div id="panCustomDiscount" class="modal fade" data-backdrop="static" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Custom Discount Details</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    <label class="form-label" for="txtCDName">Name: <span class="required">*</span></label>
                    <input type="text" data-role="none" class="form-control" id="txtCDName" />
                </div>

                <div class="mb-3">
                    <label class="form-label" for="txtCDDescription">Description:</label>
                    <textarea data-role="none" class="form-control" id="txtCDDescription" style="height:100px; resize:none;"></textarea>
                </div>

                <div class="mb-3">
                    <label class="form-label" for="txtCDMinPurchaseAmt">Minimum Purchase Amount: <span class="required">*</span></label>
                    <input type="number" data-role="none" class="form-control" id="txtCDMinPurchaseAmt" />
                </div>

                <div class="mb-3">
                    <label class="form-label" for="txtCDValue">Discount Type: <span class="required">*</span></label>
                    <div class="btn-group w-100 mb-3">
                        <input data-role="none" type="radio" class="btn-check" name="radCD" id="radCDFixAmount" value="fixamount" autocomplete="off" checked>
                        <label class="btn btn-outline-success" for="radCDFixAmount">Fix Amount</label>

                        <input data-role="none" type="radio" class="btn-check" name="radCD" id="radCDPercentage" value="percentage" autocomplete="off">
                        <label class="btn btn-outline-success" for="radCDPercentage">Percentage</label>
                    </div>
                    <input type="text" data-role="none" class="form-control" id="txtCDValue" placeholder="Ex: 123.00" />
                </div>

                <div class="row mb-3">
                    <label class="col-auto" for="chbCDIsActive">Active?</label>
                    <div class="col">
                        <div class="form-check form-switch">
                            <input data-role="none" class="form-check-input" type="checkbox" id="chbCDIsActive">
                        </div>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-auto" for="chbCDIsApplyToAllFOWST">Apply this discount to all FOWST locations?</label>
                    <div class="col">
                        <div class="form-check form-switch">
                            <input data-role="none" class="form-check-input" type="checkbox" id="chbCDIsApplyToAllFOWST">
                        </div>
                    </div>
                </div>

                <div id="divCDFowst" class="mb-3" style="display:none;">
                    <div class="row mb-3">
                        <div class="col">
                            This discount is only available to selected FOWST location(s) below:
                        </div>
                        <div class="col-auto">
                            <button data-role="none" id="btnCDAddFowst" class="btn btn-success">Add FOWST</button>
                        </div>
                    </div>

                    <ul data-role="none" id="listCDFowsts" class="list-group"></ul>
                </div>

                <div class="row mb-3">
                    <label class="col-auto" for="chbCDAvailability">Set discount period? <i>If off, it can be used every day</i>.</label>
                    <div class="col">
                        <div class="form-check form-switch">
                            <input data-role="none" class="form-check-input" type="checkbox" id="chbCDAvailability">
                        </div>
                    </div>
                </div>

                <div id="divCDAvailability" class="mb-3" style="display:none;">
                    <div class="row mb-3">
                        <div class="col-1">
                            From:
                        </div>
                        <div class="col">
                            <input data-role="none" type="date" class="form-control" id="txtCDAvailabilityFrom" />
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-1">
                            To:
                        </div>
                        <div class="col">
                            <input data-role="none" type="date" class="form-control" id="txtCDAvailabilityTo" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col">
                        <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                    </div>
                    <div class="col">
                        <button data-role="none" type="button" id="btnCustomDiscountOkay" class="btn btn-success btn-block">Okay</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>