@*@Code
    ViewData("Title") = "Payment Terms"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code*@
<script>
    require([GetPathFormat('/Scripts/sources/PaymentTerms/PaymentTerms.js?v=' + assetversion)]);
</script>

<h1 class="display-6 mb-4">Payment Terms</h1>

<input type="text" data-role="none" class="form-control" id="txtTermID" style="display:none;" />

<div class="row mb-3">
    <label class="col-sm-2 mb-3" for="txtTermName">Term Name: <span style="color:#F00">*</span></label>
    <div class="col-sm-10">
        <input type="text" data-role="none" id="txtTermName" class="form-control" />
    </div>
</div>

<div class="row mb-3">
    <label class="col-sm-2 mb-3" for="txtTermDescription">Term Description:</label>
    <div class="col-sm-10">
        <input type="text" data-role="none" id="txtTermDescription" class="form-control" />
    </div>
</div>

<div class="row mb-3">
    <label class="col-sm-2 mb-3" for="txtCreditTerm">Credit Term: (days) <span style="color:#F00">*</span></label>
    <div class="col-sm-10">
        <input type="text" data-role="none" id="txtCreditTerm" class="form-control" />
    </div>
</div>

<div class="row mb-3">
    <label class="col-sm-2 mb-3" for="selTermActive">Active?</label>
    <div class="col-sm-10">
        <div class="form-check form-switch mb-3" style="font-size:20px;">
            <input class="form-check-input" data-role="none" type="checkbox" id="selTermActive">
        </div>
    </div>
</div>

<div class="mt-4" style="text-align: center">
    <a id="btnCancelPaymentTerms" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
    <a id="btnSavePaymentTerms" data-role="none" class="btn btn-success btn-lg">Save</a>
</div>