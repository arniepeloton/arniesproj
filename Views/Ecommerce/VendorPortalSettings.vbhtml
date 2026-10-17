<script>
    require([GetPathFormat('/Scripts/sources/Ecommerce/VendorPortalSettings.js?v=' + assetversion)]);
</script>

<div data-role="content">
    <h1 class="display-6 mb-4">Vendor Portal Settings</h1>

    <div class="row mb-3">
        <div class="col-sm-3 mb-3" for="selIsShowVendor3rdPartyCustomerName">Show vendor 3rd party customer name?</div>
        <div class="col-sm-9">
            <div class="form-check form-switch mb-3" style="font-size:20px;">
                <input class="form-check-input" data-role="none" type="checkbox" id="selIsShowVendor3rdPartyCustomerName">
            </div>
        </div>
    </div>

    <div class="row mb-3">
        <div class="col-sm-3 mb-3" for="txtEcommerceVendorPortalName">Vendor Portal Name: <span class="required">*</span></div>
        <div class="col-sm-9">
            <span class="mb-3">
                Important Note: You can change the Vendor Portal name at any time but it is best not to change this too often so that your vendors/suppliers can easily recall your Vendor Portal name, and they also do not get redirected to old, non-existing web links.
            </span>
            <input type="text" data-role="none" class="form-control mb-3" id="txtEcommerceVendorPortalName" placeholder="Enter here your vendor portal name" />
            <span id="spanEcommerceVendorPortalNameError" class="mb-3" style="color:red; display:none"></span>
            <a data-role="none" class="link-info" id="divEcommerceVendorPortalUrl"></a>
        </div>
    </div>

    <div class="mt-4 text-center">
        <a id="btnCancelEcommerceVendorPortalSettings" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
        <a id="btnSaveEcommerceVendorPortalSettings" data-role="none" class="btn btn-success btn-lg">Save</a>
    </div>
</div>