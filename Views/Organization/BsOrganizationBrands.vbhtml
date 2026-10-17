<script>
    require([GetPathFormat('/Scripts/sources/Organization/OrganizationBrands.js?crc=')
        ]);
</script>

<div id="pageOrgBrand" style="display:block;">

    <h1 class="display-6 mb-3">Brand List</h1>

    <div class="text-right mb-3">
        <button data-role="none" class="btn btn-success">Add Brand</button>
    </div>

    <ul class="list-group mb-5" id="lstOrgBrand"></ul>

    <div style="text-align: center">
        <a id="btnOrgBrandDone" data-role="none" class="btn btn-secondary btn-lg">Done</a>
    </div>
</div>

<div id="pageOrgBrandInfo" style="display:none;">
    <h1 class="display-6 mb-3">Brand Info</h1>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" data-role="none" class="form-control" readonly id="txtOrgBrandRecID"/>                
        </div>

        <div class="row mb-5">
            <label class="col-sm-2 mb-3 col-form-label" for="txtOrgBrandName">Brand Name: <span style="color:#F00;">*</span></label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtOrgBrandName" />
            </div>
        </div>

        <div style="text-align: center">
            <a id="btnOrgBrandInfoCancel" data-role="none" class="btn btn-secondary btn-lg mr-3">Cancel</a>
            <a id="btnOrgBrandInfoSave" data-role="none" class="btn btn-success btn-lg">Save</a>
        </div>
    </div>
</div>
