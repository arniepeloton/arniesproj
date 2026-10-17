<script>
    require([GetPathFormat('/Scripts/sources/PIMProductPrice/UploadDownloadExcelFile.js?crc=' + assetversion)
    ]);
</script>

<div id="pagePIMTemplate" style="display:block;">
    <h1 class="display-6 mb-4">Bulk Price Adjustment</h1>


    <div data-role="content">
        <div class="mb-3">
            To change the prices of multiple products by category or brand, please download the product list below.
            Once you have downloaded the file, you can update the values of the PRICE column. Save the file and upload it back to update the price.
        </div>        

        <h5 class="mb-3"> Download Price List</h5>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="selFilter">Filter:</label>
            <div class="col-sm-10">
                <select id="selFilter" class="form-select" data-role="none">
                    <option value="">All Products</option>
                    <option value="Brand">By Brand</option>
                    <option value="Category">By Category</option>
                </select>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="selPriceLevel">Price Level:</label>
            <div class="col-sm-10">
                <select id="selPriceLevel" class="form-select" data-role="none">
                    <option value="">All Price Levels</option>
                    <option value="01000012-0005-0000-0000-000000000000">Retail Price</option>
                    <option value="01000012-0004-0000-0000-000000000000">Wholesale Price</option>
                    <option value="EEF716C6-B290-4B28-B2A6-0A4F52CF4989">SRP (Suggested Retail Price)</option>
                    <option value="01000012-0001-0000-0000-000000000000">FREIGHT COST</option>
                    <option value="01000012-0002-0000-0000-000000000000">COST FROM VENDOR</option>
                    <option value="01000012-0003-0000-0000-000000000000">LANDED COST</option>
                </select>
            </div>
        </div>

        <div id="divResult" style="display:none;">
            <table id="tblResult" class="table table-striped table-hover mb-3" style="width:100%"></table>

            <div class="mt-4 text-center">
                <a data-role="none" class="btn btn-success btn-lg" id="btnDownloadByFilter">Download Excel File</a>
            </div>
        </div>

        <h5 class="mb-3"> Upload Price List</h5>

        <div class="mb-3">Note: This feature only updates the prices of EXISTING products in the organization with matching price levels.</div>
        
        <form @*action="Index"*@ id="formupload" enctype="multipart/form-data" method="post">
            <input class="form-control" data-role="none" type="text" name="OrgID" id="txtBulkPriceOrgID" style="display:none;">
            <div class="mb-3">
                <label for="objFile" class="form-label">Upload Excel File</label>
                <input class="form-control" data-role="none" type="file" name="objFile" id="txtBulkPriceobjFile">
            </div>
        </form>

        <div class="mt-4 text-center">
            <a data-role="none" class="btn btn-success btn-lg" id="btnBackToHome">Cancel</a>
        </div>
    </div>
</div>