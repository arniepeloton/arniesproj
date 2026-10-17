<script>
    require([GetPathFormat('/Scripts/sources/PIMUpload/PIMWebForm.js?v=' + assetversion)
            ]);
</script>

<div id="" style="display:block;">
    <h1 class="display-6 mb-4">PIM Web Form</h1>

    <div data-role="content">
        <input data-role="none" id="txtWebFormOrgID" class="form-control" style="display:none;" />

        <label class="mb-3">Fields marked with <span style="color:#F00">*</span> are required. </label>
        
        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="chbAsset">This is a Rentable Asset</label>
            <div class="col-sm-10">
                <div class="form-check form-switch mb-3" style="font-size:20px;">
                    <input class="form-check-input" data-role="none" type="checkbox" id="chbAsset" value="1">
                </div>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="chbAsset">This is an inventory-count item?</label>
            <div class="col-sm-10">
                <div class="form-check form-switch mb-3" style="font-size:20px;">
                    <input class="form-check-input" data-role="none" type="checkbox" id="chbIsInventoryCountItem" value="1">
                </div>
            </div>
        </div>
        
        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtWebFormProductNo">Product No.: <span style="color:#F00">*</span></label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtWebFormProductNo" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtWebFormShortName">Short Name: <span style="color:#F00">*</span></label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtWebFormShortName" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtWebFormBrandName">Brand:</label>
            <div class="col-sm-10">
                <div class="input-group">
                    <input type="text" data-role="none" class="form-control" id="txtWebFormBrandName" />
                    <button data-role="none" id="btnWebFormSearchBrand" class="btn btn-secondary">Search Existing Brand</button>
                </div>                
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtWebFormCategory">Category: <span style="color:#F00">*</span></label>
            <div class="col-sm-10">
                <div class="input-group">
                    <input type="text" data-role="none" class="form-control" id="txtWebFormCategory" />
                    <button data-role="none" id="btnWebFormSearchCategory" class="btn btn-secondary">Search Existing Category</button>
                </div>                
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtWebFormUPCBarcode">UPC Barcode:</label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtWebFormUPCBarcode" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtWebFormCostFromVendor">Cost From Vendor: <span style="color:#F00">*</span></label>
            <div class="col-sm-10">
                <input type="number" data-role="none" class="form-control" id="txtWebFormCostFromVendor" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtWebFormShippingCost">Landed Cost: <span style="color:#F00">*</span></label>
            <div class="col-sm-10">
                <input type="number" data-role="none" class="form-control" id="txtWebFormShippingCost" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtWebFormLandedCost">Shipping Cost:</label>
            <div class="col-sm-10">
                <input type="number" data-role="none" class="form-control" id="txtWebFormLandedCost" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtWebFormSRP">Suggested Retail Price (SRP): <span style="color:#F00">*</span></label>
            <div class="col-sm-10">
                <input type="number" data-role="none" class="form-control" id="txtWebFormSRP" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtWebFormRetailPrice">Retail Price: <span style="color:#F00">*</span></label>
            <div class="col-sm-10">
                <input type="number" data-role="none" class="form-control" id="txtWebFormRetailPrice" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtWebFormPackingSizeQty">Packing Size Qty: <span style="color:#F00">*</span></label>
            <div class="col-sm-10">
                <input type="number" data-role="none" class="form-control" id="txtWebFormPackingSizeQty" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtWebFormPackingSizeQty">Packing Size Unit: <span style="color:#F00">*</span></label>
            <div class="col-sm-10">
                <select id="selWebFormPackingSizeUnit" data-role="none" class="form-select"></select>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtWebFormContentQty">Content Qty:</label>
            <div class="col-sm-10">
                <input type="number" data-role="none" class="form-control" id="txtWebFormContentQty" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="selWebFormContentUnit">Content Unit:</label>
            <div class="col-sm-10">
                <select id="selWebFormContentUnit" data-role="none" class="form-select"></select>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtWebFormProductDimensionLength">Product Dimension Length:</label>
            <div class="col-sm-10">
                <input type="number" data-role="none" class="form-control" id="txtWebFormProductDimensionLength" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="selWebFormProductDimensionLengthUnit">Product Dimesion Length Unit:</label>
            <div class="col-sm-10">
                <select id="selWebFormProductDimensionLengthUnit" data-role="none" class="form-select"></select>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtWebFormProductDimensionWidth">Product Dimension Width:</label>
            <div class="col-sm-10">
                <input type="number" data-role="none" class="form-control" id="txtWebFormProductDimensionWidth" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="selWebFormProductDimensionWidthUnit">Product Dimesion Width Unit:</label>
            <div class="col-sm-10">
                <select id="selWebFormProductDimensionWidthUnit" data-role="none" class="form-select"></select>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtWebFormProductDimensionHeight">Product Dimension Height:</label>
            <div class="col-sm-10">
                <input type="number" data-role="none" class="form-control" id="txtWebFormProductDimensionHeight" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="selWebFormProductDimensionHeightUnit">Product Dimesion Height Unit:</label>
            <div class="col-sm-10">
                <select id="selWebFormProductDimensionHeightUnit" data-role="none" class="form-select"></select>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtWebFormProductWeight">Product Weight:</label>
            <div class="col-sm-10">
                <input type="number" data-role="none" class="form-control" id="txtWebFormProductWeight" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="selWebFormProductWeightUnit">Product Weight Unit:</label>
            <div class="col-sm-10">
                <select id="selWebFormProductWeightUnit" data-role="none" class="form-select"></select>
            </div>
        </div>

        <div class="mt-4 text-center">
            <a id="btnWebFormCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
            <a id="btnWebFormSave" data-role="none" class="btn btn-success btn-lg">Save</a>
        </div>
    </div>
</div>