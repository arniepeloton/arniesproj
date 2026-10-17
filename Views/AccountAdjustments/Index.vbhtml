@*@Code
    ViewData("Title") = "Account Adjustments"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code*@
<script>
    require([GetPathFormat('/Scripts/sources/AccountAdjustments/AccountAdjustments.js?v=' + assetversion)]);
</script>

<div id="popupNewAdjustment" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="txtNewAdjustmentTitle"></h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div class="row mb-3">
                    <label class="col-sm-5 mb-3 col-form-label" for="selAdjustmentType">Adjustment Type: <span class="required">*</span></label>
                    <div class="col-sm-7">
                        <select id="selAdjustmentType" data-role="none" class="form-select">
                            <option value="">Please Select</option>
                            <option value="Debit Memo">Debit Memo</option>
                            <option value="Credit Memo">Credit Memo</option>
                        </select>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-5 mb-3" for="selAdjustmentType">Effect on <span id="lblAdjustmentTitle"></span> Accounts:</label>
                    <div class="col-sm-7">
                        <b id="txtAdjustmentEffect"></b>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-5 mb-3" for="txtAdjustmentReason">Reason for Adjustment: <span class="required">*</span></label>
                    <div class="col-sm-7">
                        <input type="text" data-role="none" class="form-control" id="txtAdjustmentReason" />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-5 mb-3" for="selAdjustmentStatus">Active:</label>
                    <div class="col-sm-7">
                        <div class="form-check form-switch mb-3" style="font-size:20px;">
                            <input class="form-check-input" data-role="none" type="checkbox" id="selAdjustmentStatus">
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col-6">
                        <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                    </div>
                    <div class="col-6">
                        <button data-role="none" type="button" id="btnSaveAdjustment" class="btn btn-success btn-block">Save</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="row mb-3 align-items-center">
    <div class="col-sm-6">
        <h3><b>Account Receivables</b> - Adjustments Types</h3>
    </div>
    <div class="col-sm-6 text-right">
        <button id="btnArAddAdjustmentType" data-role="none" class="btn btn-success">
            Add Adjustment Type
        </button>
    </div>
</div>

<table data-role="none" class="table table-striped table-hover display mb-5">
    <thead>
        <tr>
            <th data-sort="" style="cursor:pointer;"> Adjustment Type </th>
            <th data-sort="" style="cursor:pointer;"> Effect on AR Accounts </th>
            <th data-sort="" style="cursor:pointer;"> Reason for Adjustment </th>
            <th data-sort="" style="cursor:pointer;"> Active </th>
        </tr>
    </thead>
    <tbody id="tbodyAR"></tbody>
</table>

<div class="row mb-3 align-items-center">
    <div class="col-sm-6">
        <h3><b>Account Payable</b> - Adjustments Types</h3>
    </div>
    <div class="col-sm-6 text-right">
        <button id="btnApAddAdjustmentType" data-role="none" class="btn btn-success">
            Add Adjustment Type
        </button>
    </div>
</div>

<table data-role="none" class="table table-striped table-hover display mb-3">
    <thead>
        <tr>
            <th data-sort="" style="cursor:pointer;"> Adjustment Type </th>
            <th data-sort="" style="cursor:pointer;"> Effect on AR Accounts </th>
            <th data-sort="" style="cursor:pointer;"> Reason for Adjustment </th>
            <th data-sort="" style="cursor:pointer;"> Active </th>
        </tr>
    </thead>
    <tbody id="tbodyAP"></tbody>
</table>