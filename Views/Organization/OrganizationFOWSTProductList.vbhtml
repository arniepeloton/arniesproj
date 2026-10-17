<script>
    require([GetPathFormat('/Scripts/sources/Organization/OrganizationFOWSTProductList.js?v=' + assetversion),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMProduct.js?crc=')
        ]);
</script>

<div id="pageOrgFOWSTProductList" style="display:block;">
    <div class="row mb-4 align-items-center">
        <div class="col-6">
            <h1 class="display-6">FOWST Product List</h1>
        </div>
        <div class="col-6 text-right">
            <a id="btnOrgFOWSTProductAdd" data-role="none" class="btn btn-success">Add SKU to FOWST Location</a>
        </div>
    </div>    

    <div data-role="content">
        <form id="frmOrgFowstProductSearch">
            <div><input id="txtOrgFowstProductSearch" data-role="none" class="form-control" type="search" value="" /></div>
        </form>

        <div class="mb-3" id="divOrgFOWSTProductRecordCount"></div>
        
        <ul data-role="none" id="lstOrgFOWSTProductList" class="list-group mb-3"></ul>

        <div class="mt-4 text-center">
            <a id="btnOrgFOWSTProductListDone" data-role="none" class="btn btn-secondary btn-lg">Done</a>
        </div>
    </div>
</div>

<div id="pageOrgFOWSTProductInfo" style="display:none;">
    <h1 class="display-6 mb-4">FOWST Product Info</h1>

    <div data-role="content">

        <div style="display:none;">
	        <input type="text" data-role="none" class="form-control" readonly id="txtOrgFOWSTProdID"/>
            <input type="text" data-role="none" class="form-control" readonly id="txtOrgFOWSTID" />
            <input type="text" data-role="none" class="form-control" readonly id="txtOrgFOWSTProductID" />
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="btnOrgFOWSTProduct">Product:</label>
            <div class="col-sm-10">
                <button id="btnOrgFOWSTProduct" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtOrgFOWSTProductReorderPoint">Reorder Point for this FOWST Location : <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtOrgFOWSTProductReorderPoint" />
            </div>
        </div>

        <div class="row mb-3 align-items-center">
            <div class="col-6">
                <h5>This SKU is located in the following bins.</h5>
            </div>
            <div class="col-6 text-right">
                <a id="btnAddOrgFOWSTProductBin" data-role="none" class="btn btn-success">Add another bin location for this SKU</a>
            </div>
        </div>

        <ul data-role="none" id="lstOrgFOWSTProductBin" class="list-group mb-3"></ul>

        <div class="mt-4 text-center">
            <a id="btnOrgFOWSTProductCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
            <a id="btnOrgFOWSTProductSave" data-role="none" class="btn btn-success btn-lg">Ok</a>
        </div>
    </div>
</div>

<div id="pageOrgFOWSTProductBinInfo" style="display:none;">
    <h1 class="display-6 mb-4">Product Bin Info</h1>

    <div data-role="content">

        <div style="display:none;">
	        <input type="text" data-role="none" class="form-control" readonly id="txtOrgFOWSTProductBinID"/>   
            <input type="text" data-role="none" class="form-control" readonly id="txtOrgFOWSTBinID" />             
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtOrgFOWSTBin">Bin:</label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtOrgFOWSTBin" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtOrgFOWSTBinMin">Min : <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <input type="number" data-role="none" class="form-control" id="txtOrgFOWSTBinMin" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtOrgFOWSTBinMax">Max : <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <input type="number" data-role="none" class="form-control" id="txtOrgFOWSTBinMax" />
            </div>
        </div>

        <div class="mt-4 text-center">
            <a id="btnOrgFOWSTProductBinCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
            <a id="btnOrgFOWSTProductBinSave" data-role="none" class="btn btn-success btn-lg">Ok</a>
        </div>
    </div>
</div>