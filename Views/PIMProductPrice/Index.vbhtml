<script>
    require([GetPathFormat('/Scripts/sources/PIMProductPrice/PIMProductPriceMenu.js?v=' + assetversion),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMPriceLevel.js?crc=')
        ]);
</script>

<div id="pagePIMProductPriceList" style="display:block;">
    <h1 class="display-6 mb-4">Product Price List</h1>

    <div data-role="content">

        <form autocomplete="off" id="frmPIMProductPriceBrowser">
            <div>
                <input id="txtPIMProductPriceSearch" data-role="none" class="form-control" type="search" placeholder="Search a Product" value="" />
            </div>
        </form>

        <div id="divPIMProductPriceSearchResults" style="display:none;">

            <div class="ListViewMarginBottom mb-3" id="divPIMProductPriceRecordCount"></div>

            <div id="lstPIMProductPrice" data-role="none" class="list-group mb-3">
            </div>

        </div>

        <div id="divPIMProductPriceSearchNoResults" style="display:none;">
	        <div>
                <div data-role="fieldcontain">
                    <label id="lblPIMProductPriceNotFound"></label>
                </div>
            </div>
        </div>

        <div class="mt-4 text-center">
            <a id="btnPIMProductPriceBackToHome" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
        </div>
    </div>
</div>

<div id="pagePIMProductPriceInfo" style="display:none;">
    <h1 class="display-6 mb-4">Product Price Info</h1>

    <div data-role="content" >       

        <div style="display: none;"><input type="text" readonly id="txtPIMPriceLevelID" /></div>
        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="btnPIMPriceLevel">Price Level: <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <button id="btnPIMPriceLevel" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
            </div>
        </div>

        <div style="display: none;"><input type="text" readonly id="txtPIMProductID" /></div>
        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtPIMProduct">Product:</label>
            <div class="col-sm-10">
                <input type="number" data-role="none" class="form-control" id="txtPIMProduct" readonly />
            </div>
        </div>

        <div>
            <h5 class="mb-3">Product Size(s)</h5>
            <div id="divProductPrice" class="mb-3"></div>
        </div>

        <div class="mt-4 text-center">
            <a id="btnPIMProductPriceCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
            <a id="btnPIMProductPriceSave" data-role="none" class="btn btn-success btn-lg">Save</a>
        </div>
    </div>
</div>