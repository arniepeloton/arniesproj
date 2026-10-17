@*@Code
    ViewData("Title") = "Sales Returns Settings"
    Layout = "~/Views/Shared/Mobiz.vbhtml"
End Code*@
<script>
    require([GetPathFormat('/Scripts3/SalesReturnsSettings.js?v=' + assetversion)]);
</script>
@*<h1>
    Sales Returns Settings</h1>*@
<div class="container-fluid">
    <div class="row mb-3" style="display:none;">
        <label class="col-sm-3 mb-3 col-form-label" for="txtRestockingFeePercent">Restocking Fee %:</label>
        <div class="col-sm-9">
            <input type="number" data-role="none" class="form-control" id="txtRestockingFeePercent" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-3 mb-3 col-form-label" for="txtMaximumReturnablePeriod">Maximum Returnable Period (days):</label>
        <div class="col-sm-9">
            <input type="number" data-role="none" class="form-control" id="txtMaximumReturnablePeriod" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-3 mb-3 col-form-label" for="cbReturnedPrice">When returned item has different price points (due to price breaks or volume pricing), follow:</label>
        <div class="col-sm-9">
            <select id="cbReturnedPrice" data-role="none" class="form-select">
                <option value="invalid">Please choose one</option>
                <option>Average Price</option>
                <option>Minimum Price</option>
            </select>
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-3 mb-3 col-form-label" for="cbSRVUse">Are SRVs 1-time use only or would you allow customers to use SRVs multiple times (as long as said SRV have some remaining value)?</label>
        <div class="col-sm-9">
            <select id="cbSRVUse" data-role="none" class="form-select">
                <option value="invalid">Please choose one</option>
                <option>1-Time Use of SRV</option>
                <option>Multiple Use of SRV</option>
            </select>
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-3 mb-3 col-form-label" for="cbPrintRefundSignUp">Print out a refund sign-up slip every time a refund transaction is made?</label>
        <div class="col-sm-9">
            <select id="cbPrintRefundSignUp" data-role="none" class="form-select">
                <option value="invalid">Please choose one</option>
                <option value="1">Yes</option>
                <option value="0">No</option>
            </select>
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-3 mb-3 col-form-label" for="chkHideMerchandiseExchange">Hide Merchandise Exchange:</label>
        <div class="col-sm-9">
            <div class="form-check form-switch mb-3" style="font-size:25px;">
                <input class="form-check-input" data-role="none" type="checkbox" id="chkHideMerchandiseExchange">
            </div>
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-3 mb-3 col-form-label" for="chkRestrictedTransactionLookupForSalesReturn">Restricted Transaction Lookup for Sales Return:</label>
        <div class="col-sm-9">
            <div class="form-check form-switch mb-3" style="font-size:25px;">
                <input class="form-check-input" data-role="none" type="checkbox" id="chkRestrictedTransactionLookupForSalesReturn">
            </div>
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-3 mb-3 col-form-label" for="txtAllowedReturnDuration">Allowed Return Duration (Minutes):</label>
        <div class="col-sm-9">
            <input type="number" data-role="none" class="form-control" id="txtAllowedReturnDuration" />
        </div>
    </div>

    <div class="mt-4" style="text-align: center">
        <a id="butCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
        <a id="butSalesReturnSave" data-role="none" class="btn btn-success btn-lg">Save</a>
    </div>
</div>