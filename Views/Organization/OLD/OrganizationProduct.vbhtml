@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/Organization/OrganizationProduct.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMProduct.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMProductPublic.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMProductMeasure.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMMeasure.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMPriceLevel.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMMeasureLength.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMMeasureWeight.js?crc=')
        ]);
    </script>
    
End Section

<div id="pageOrgProduct" data-role="page">
     <div data-role="header" data-position="fixed1" style="background: none white; border: none">
        <div class="ui-grid-d">
            <div class="ui-block-a" style="width: 10%;">
                <div id="btnHome" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>Home</span>
                </div>
            </div>
            <div class="ui-block-b" style="width: 20%;">
                <div id="btnSystemAdmin" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>System Administration</span>
                </div>
            </div>
            <div class="ui-block-c" style="width: 20%;">
                <div id="btnDataLibraries" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>Data Libraries</span>
                </div>
            </div>
            <div class="ui-block-d" style="width: 20%;">
                <div id="btnPIM" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>PIM</span>
                </div>
            </div>
            <div class="ui-block-d" style="width: 30%;">
                <div id="btnPIMCompanyWide" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>Company Wide Product List</span>
                </div>
            </div>
        </div>

    </div>

    <div data-role="content" >

        <div>
            <ul data-role="listview" id="lstOrgProductContainer" data-inset="true">   
				<li data-role="list-divider">Your company's current product list</li>
                <li>
                    <form id="frmOrgProductSearch">
                        <div data-role="fieldcontain">
                            <label for="txtOrgProductSearch">Search Company Products: </label>
                            <input id="txtOrgProductSearch" type="search" value="" />
                         </div>
                    </form>
                    @*<div data-role="fieldcontain">This list is limited to show 50 SKU records.</div>*@
                    <div data-role="fieldcontain" id="divOrgProductRecordCount">
                        <label id="lblProductRecordcount"></label>
                        <a id="lnkProductList" style="cursor:pointer; font-size:16px;"> Click here to download Excel file to view full company product list.</a>
                    </div>
                    <div>
                        <ul data-role="listview" id="lstOrgProduct" data-inset="true">   
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div>
            <br />
            <div style="text-align: center">
                <a id="btnAddNewProducts" data-role="button"><h3>Add SKU</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgProductDone" data-role="button"><h3>Go Back to Main</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgProductGoToSetupChecklist" data-role="button"><h3>Go to Setup Checklist</h3></a>
            </div>
        </div>

    </div>
    <div id="divAddProductPopup" data-role="popup">
        <ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="divider">Add SKU Options</li>
            <li><a id="btnBarcodeSearch">Barcode / Search</a></li>
            <li><a id="btnExcelFile">Excel File</a></li>
            <li><a id="btnSubscribe">Subscribe to Company Product List</a></li>
        </ul>
    </div>
</div>

<div id="pageOrgProductInfo" data-role="page">

    <div data-role="header">
        <h1>Product Info</h1>
        <a href="#divProductInfoPopup" data-rel="popup" class="ui-btn-right">Menu</a>
        
       @* <div class="ui-grid-c" >
            <div class="ui-block-a" style="width: 35%;">
                <div id="btnProductInfo" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    <span>Product Information</span>
                </div>
            </div>
            <div class="ui-block-b" style="width: 35%;">
                <div id="btnProductMeasurements" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    <span>Product Measurements</span>
                </div>
            </div>
            <div class="ui-block-c" style="width: 30%;">
                <div id="btnProductPrices" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    <span>Product Prices</span>
                </div>
            </div>
            <div class="ui-block-d" style="width: 25%;  display:none;">
                <div id="btnProductBOM" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    <span>Bill Of Materials / Kitting</span>
                </div>
            </div>
        </div>*@
    </div>

    <div data-role="content">
    <div data-role="popup" id="divProductInfoPopup" data-theme="b">
        <ul data-role="listview" data-inset="true" style="min-width:210px;">
            <li data-role="list-divider">Choose an action</li>
            <li><a id="btnProductInfo" href="#">Product Information</a></li>
            <li><a id="btnProductMeasurements" href="#">Product Measurements</a></li>
            <li><a id="btnProductPrices" href="#">Product Prices</a></li>
            <li><a id="btnProductBOM" href="#">Bill Of Materials</a></li>
        </ul>
    </div>
        <div id="divProductInfoContainer" style = "margin: 0px 7px 0 7px;">
            <div data-role="fieldcontain" style="display:none;">
	            <input type="text" readonly id="txtOrgPIMProductID"/>                
            </div>

            <div id="btnProductBrowser">
            </div>
            <div style="display:none;"><input type="text" readonly id="txtOrgProductID"/></div>
            <div data-role="fieldcontain">
	            <label for="txtOrgProductBrand">Brand : <span style="color: #FF0000;" >*</span></label>
                <input type="text" id="txtOrgProductBrand" maxlength="50"/>
            </div>

            <div data-role="fieldcontain">
	            <label for="txtOrgProductNo">Product No. : <span style="color: #FF0000;" >*</span></label>
                <input type="text" id="txtOrgProductNo" maxlength="50"/>
            </div>

            <div data-role="fieldcontain">
	            <label for="txtOrgProductName">Product Name : <span style="color: #FF0000;">*</span></label>
                <input type="text" id="txtOrgProductName" maxlength="250"/>
            </div>

            <div data-role="fieldcontain">
	            <label for="txtOrgProductModelNo">Model No. : </label>
                <input type="text" id="txtOrgProductModelNo" maxlength="50"/>
            </div>

            <div data-role="fieldcontain">
	            <label for="txtOrgProductGroupNo">Product Group No. : </label>
                <input type="text" id="txtOrgProductGroupNo" maxlength="50"/>
            </div>

            <div data-role="fieldcontain">
	            <label for="txtOrgProductDivision">Division : </label>
                <input type="text" id="txtOrgProductDivision" maxlength="50"/>
            </div>

            <div data-role="fieldcontain">
	            <label for="txtOrgProductCategory">Category : </label>
                <input type="text" id="txtOrgProductCategory" maxlength="50"/>
            </div>

            <div data-role="fieldcontain">
	            <label for="txtOrgProductMarketingText">Marketing Text : </label>
                <input type="text" id="txtOrgProductMarketingText" maxlength="50"/>
            </div>

            <div data-role="fieldcontain">
	            <label for="txtOrgProductWarranty">Warranty : </label>
                <input type="text" id="txtOrgProductWarranty" maxlength="50"/>
            </div>

            <div data-role="fieldcontain">
	            <label for="txtOrgProductPublishedBy">Published By : </label>
                <input type="text" id="txtOrgProductPublishedBy" readonly/>
            </div>

            <div data-role="fieldcontain">
	            <label for="txtOrgProductUploadedBy">Uploaded By : </label>
                <input type="text" id="txtOrgProductUploadedBy" readonly/>
            </div>

	        <div>Do not share transaction data with the brand owner?</div>
            <select id="selOrgProductDoNotShareData" data-role="slider">
			    <option value="0">No</option>
	            <option value="1">Yes</option>
            </select>

            <div>
	            <label>Activation Date: <span style="color: #FF0000;" >*</span></label>
            </div>
            <fieldset data-role="controlgroup" data-type="horizontal">
                <select id="selOrgProductActivationDateMonth">
	                <option value="">MMM</option>
                </select>
                <select id="selOrgProductActivationDateDay">
	                <option value="">DD</option>
                </select>                
                <select id="selOrgProductActivationDateYear">
	                <option value="">YYYY</option>
                </select>                
            </fieldset>

	        <div>Deactivate record?</div>
            <select id="selOrgProductDeactivate" data-role="slider">
			    <option value="0">No</option>
	            <option value="1">Yes</option>
            </select>

            <div id="divOrgProductDeactivateDetails" style="display:none;">

	            <label>Deactivation Date: <span style="color: #FF0000;" >*</span></label>
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <select id="selOrgProductDateInActiveMonth">
	                    <option value="">MMM</option>
                    </select>
                    <select id="selOrgProductDateInActiveDay">
	                    <option value="">DD</option>
                    </select>                
                    <select id="selOrgProductDateInActiveYear">
	                    <option value="">YYYY</option>
                    </select>                
                </fieldset>

                <div data-role="fieldcontain">
	                <label for="txtOrgProductInActiveReason">Reason for deactivating:</label>
                    <fieldset data-role="controlgroup">
	                    <textarea id="txtOrgProductInActiveReason"></textarea>
                    </fieldset>
                </div>

                <div data-role="fieldcontain">
	                <label for="txtOrgProductComments">Comments:</label>
                    <fieldset data-role="controlgroup">
	                    <textarea id="txtOrgProductComments"></textarea>
                    </fieldset>
                </div>

	            <div>Ban this record for future transactions?</div>
                <select id="selOrgProductBan" data-role="slider">
				    <option value="0">No</option>
	                <option value="1">Yes</option>
                </select>

            </div>

        </div>

        <div id="divProductMeasurementContainer" style = "display:none; margin: 0px 7px 0 7px;">

            <div data-role="fieldcontain"><a id="btnOrgProductMeasurementAdd" data-role="button" data-icon="plus" data-inline="true">Add Measurement</a></div>
            <div data-role="fieldcontain" id="divOrgProductMeasurementRecordCount"></div>
            <div>
                <ul data-role="listview" id="lstOrgProductMeasurement" data-inset="true">
                </ul>
            </div>

        </div>

        <div id="divProductPriceContainer" style = "display:none; margin: 0px 7px 0 7px;">
            <div data-role="fieldcontain"><a id="btnOrgProductPriceLevelAdd" data-role="button" data-icon="plus" data-inline="true">Add Product Price</a></div>
            <div data-role="fieldcontain" id="divOrgProductPriceRecordCount"></div>
            <div>
                <ul data-role="listview" id="lstOrgProductPrice" data-inset="true">
                </ul>
            </div>

        </div>

        <div id="divProductBOMContainer" style="display:none; margin: 0px 7px 0 7px;">
            
            <div>
                <h3><span id="spanProductShortName"></span></h3>
                <p>Use this tool to create Bill Of Materials/Kit for this product. Tap on "Add Component" to add specific components.</p>
            </div>

            <div data-role="fieldcontain"><a id="btnAddProductBOMMain" data-role="button" data-icon="plus" data-inline="true">Add Component</a></div>

            <div id="divProductBOM" style="display:none;">
    
                <div name="divMain_ProductBOMSKU" class="col" data-role="collapsible_clone" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d">
                    <h3><span></span></h3>
                    <div name="divProductBOMSub">
                    </div>
                </div>

            </div>

        </div>

        <div>  
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgProductInfoSave" data-role="button">
                <h3>Ok</h3>
                </a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgProductInfoCancel" data-role="button">
                <h3>Cancel</h3>
                </a>
            </div>


@*	        <div style="text-align:center"> <a id="btnOrgProductInfoSave" data-role="button">Ok</a> </div>
	        <div style="text-align:center"> <a id="btnOrgProductInfoCancel" data-role="button">Cancel</a> </div>*@
        </div>

    </div>

</div>

<div id="pageSKUMeasurement" data-role="page">
    <div data-role="header">
        <h1>Measurement Details</h1>
    </div>

    <div data-role="content">
        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtRecProductMeasurementID"/>                
	        <input type="text" readonly id="txtRecProductMeasurementPackingSizeUnitID"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="btnPackingSizeUnit">Packing Size Unit <span style="color: #FF0000;">*</span></label>
            <fieldset data-role="controlgroup">
                <button id="btnPackingSizeUnit" data-role="button">Please Select</button>
            </fieldset>
        </div>

	    <div>
            <div data-role="fieldcontain">
	            <label for="txtPackingSizeQty">Packing Size Qty <span style="color: #FF0000;">*</span></label>
                <fieldset data-role="controlgroup">
                    <input type="number" id="txtPackingSizeQty" style="text-align: right;"/>
                </fieldset>
            </div>
        </div>

	    <div>
            <div data-role="fieldcontain">
	            <label for="txtPackingSizeVolDiscount">Volume Discount (%)</label>
                <fieldset data-role="controlgroup">
                    <input type="number" id="txtPackingSizeVolDiscount" style="text-align: right;"/>
                </fieldset>
            </div>
        </div>

	    <div>
            <div data-role="fieldcontain">
	            <label for="txtPackingSizeContentQty">Content Qty</label>
                <fieldset data-role="controlgroup">
                    <input type="number" id="txtPackingSizeContentQty" style="text-align: right;"/>
                </fieldset>
            </div>
        </div>

        <div data-role="fieldcontain">
	        <label for="btnPackingSizeContentUnit">Content Unit</label>
            <fieldset data-role="controlgroup">
                <button id="btnPackingSizeContentUnit" data-role="button">Please Select</button>
            </fieldset>
        </div>

	    <div>
            <div data-role="fieldcontain">
	            <label for="txtPackingSizeWeight">Packing Size Weight</label>
                <fieldset data-role="controlgroup">
                    <input type="number" id="txtPackingSizeWeight" style="text-align: right;"/>
                </fieldset>
            </div>
        </div>

        <div data-role="fieldcontain">
	        <label for="btnPackingSizeWeightUnit">Packing Size Weight Unit</label>
            <fieldset data-role="controlgroup">
                <button id="btnPackingSizeWeightUnit" data-role="button">Please Select</button>
            </fieldset>
        </div>

        <div>
            <ul data-role="listview" data-inset="true">   
				<li data-role="list-divider">Dimensions</li>
                <li>
	                <div>
                        <div data-role="fieldcontain">
	                        <label for="txtPackingSizeDimensionLength">Dimension Length</label>
                            <fieldset data-role="controlgroup">
                                <input type="number" id="txtPackingSizeDimensionLength" style="text-align: right;"/>
                            </fieldset>
                        </div>
                    </div>

                    <div data-role="fieldcontain">
	                    <label for="btnPackingSizeDimLengthUnit">Dimension Length Unit</label>
                        <fieldset data-role="controlgroup">
                            <button id="btnPackingSizeDimLengthUnit" data-role="button">Please Select</button>
                        </fieldset>
                    </div>

	                <div>
                        <div data-role="fieldcontain">
	                        <label for="txtPackingSizeDimensionWidth">Dimension Width</label>
                            <fieldset data-role="controlgroup">
                                <input type="number" id="txtPackingSizeDimensionWidth" style="text-align: right;"/>
                            </fieldset>
                        </div>
                    </div>

                    <div data-role="fieldcontain">
	                    <label for="btnPackingSizeDimWidthUnit">Dimension Width Unit</label>
                        <fieldset data-role="controlgroup">
                            <button id="btnPackingSizeDimWidthUnit" data-role="button">Please Select</button>
                        </fieldset>
                    </div>

	                <div>
                        <div data-role="fieldcontain">
	                        <label for="txtPackingSizeDimensionHeight">Dimension Height</label>
                            <fieldset data-role="controlgroup">
                                <input type="number" id="txtPackingSizeDimensionHeight" style="text-align: right;"/>
                            </fieldset>
                        </div>
                    </div>

                    <div data-role="fieldcontain">
	                    <label for="btnPackingSizeDimensionHeightUnit">Dimension Height Unit</label>
                        <fieldset data-role="controlgroup">
                            <button id="btnPackingSizeDimensionHeightUnit" data-role="button">Please Select</button>
                        </fieldset>
                    </div>
                </li>
            </ul>
        </div>

        <div>
            <ul data-role="listview" id="lstProductMeasurementBarcodeContainer" data-inset="true">
                <li data-role="list-divider">Product Measurement Barcode</li>
                <li>

                    <div style="padding:5px;">
                        <div class="ui-grid-a" style="margin:0px; padding: 2px;">
                            <div class="ui-block-a" style="width:78%;height: 50px;padding: 0px;margin-bottom:5px;">
                                <input id="txtProductMeasurementBarcode" type="text" placeholder="Enter Barcode Number Here" style="height: 40px; padding:0px;" />
                            </div>
                            <div class="ui-block-b" style="width: 22%;height: 50px;padding: 0px;margin:0px;">
                                <center><div >
                                    <img src="/Content/images/barcode96x96.png" class="detailimage2" id="butMeasurementBarCode" data-role="button" data-corners="false"/>
                                </div>
                                </center>
                            </div>
                        </div>
 
                    </div>
                    <br />
                    <div>
                        <ul data-role="listview" id="lstProductMeasurementBarcode" data-inset="true" data-split-icon="delete">
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div style="text-align: center">
            <a id="btnProductMeasurementSave" data-role="button">
            <h3>Save</h3>
            </a>
        </div>

        <div style="text-align: center;">
            <a id="btnProductMeasurementRemove" data-role="button" style="display:none;">
            <h3>Remove</h3>
            </a>
        </div>

        <div style="text-align: center">
            <a id="btnProductMeasurementCancel" data-role="button">
            <h3>Cancel</h3>
            </a>
        </div>

    </div>

</div>

<div id="pageSKUPrice" data-role="page">
    <div data-role="header">
        <h1>Price Details</h1>
    </div>

    <div data-role="content">
        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtRecProductPriceID"/>                
	        <input type="text" readonly id="txtRecProductPriceLevelID"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="btnProductPriceLevel">Price Level <span style="color: #FF0000;">*</span></label>
            <fieldset data-role="controlgroup">
                <button id="btnProductPriceLevel" data-role="button">Please Select</button>
            </fieldset>
        </div>

	    <div>
            <div data-role="fieldcontain">
	            <label for="txtProductPrice">Price <span style="color: #FF0000;">*</span></label>
                <fieldset data-role="controlgroup">
                    <input type="number" id="txtProductPrice" style="text-align: right;"/>
                </fieldset>
            </div>
        </div>

        <div style="text-align: center">
            <a id="btnProductPriceSave" data-role="button">
            <h3>Save</h3>
            </a>
        </div>

        <div style="text-align: center;">
            <a id="btnProductPriceRemove" data-role="button" style="display:none;">
            <h3>Remove</h3>
            </a>
        </div>

        <div style="text-align: center">
            <a id="btnProductPriceCancel" data-role="button">
            <h3>Cancel</h3>
            </a>
        </div>

    </div>

</div>

<div id="pageBOMSKUInfo" data-role="page">
    <div data-role="header">
        <h1>Component Details</h1>
    </div>

    <div data-role="content">
        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtRecBOMProductID"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="txtBOMComponentName">Component Short Name:<span style="color: #FF0000;">*</span> </label>
            <fieldset data-role="controlgroup">
                <input type="text" id="txtBOMComponentName"/>
            </fieldset>
        </div>

        <div style="display:none;"><input type="text" readonly id="txtBOMProductID"/></div>
        <div data-role="fieldcontain">
	        <label for="btnBOMProduct">SKU Record: <span style="color: #FF0000;">*</span></label>
            <fieldset data-role="controlgroup">
                <button id="btnBOMProduct" data-role="button">Please Select</button>
            </fieldset>
        </div>

	    <div>
            <legend id="legendComponentMeasurement">How much this component do you need.</legend>
            <div data-role="fieldcontain">
	            <label for="txtBOMProductQty">Quantity: <span style="color: #FF0000;">*</span></label>
                <fieldset data-role="controlgroup">
                    <input type="number" id="txtBOMProductQty" style="text-align: right;"/>
                </fieldset>
            </div>

            <div data-role="fieldcontain">
                <legend id="legendUnitMeasure">Unit Of Measure <span style="color: #FF0000;">*</span></legend>
                <select id="selUnitMeasure">
                    <option value="">Please Select</option>
                </select>
            </div>
        </div>
                  
        <div data-role="fieldcontain">
            <legend id="legendComponentMethod">How do you make this. <span style="color: #FF0000;">*</span></legend>
            <select id="selComponentMethod">
                <option value="">Please Select</option>
                <option value="Made-to-Order">Made-to-Order</option>
                <option value="Batch Processing">Batch Processing</option>
                <option value="Store Bought">Store Bought</option>
            </select>
        </div>

        <div style="text-align: center">
            <a id="btnProductBOMSave" data-role="button">
            <h3>Save</h3>
            </a>
        </div>

        <div style="text-align: center;">
            <a id="btnProductBOMRemove" data-role="button" style="display:none;">
            <h3>Remove</h3>
            </a>
        </div>

        <div style="text-align: center">
            <a id="btnProductBOMCancel" data-role="button">
            <h3>Cancel</h3>
            </a>
        </div>

    </div>

</div>





<div id="pageBrowseByOrgOwner" data-role="page">
    <div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>List of Organizations with Publicly Available Products</h1>
    </div>

    <div data-role="content">
         Select an organization to subscribe to their product list.
        <form id="frmOrgOwnerSearch">
            <div><input id="txtSearchOrgOwner" type="search" value="" placeholder="Search Organization"/></div>
        </form>

        <div id="divOrgOwnerSearchResults" style="display:none;">

            <div class="ListViewMarginBottom" id="divOrgOwnerRecordCount"></div>

            <ul id="lstOrgOwner" data-role="listview" data-filter="true">
            </ul>

        </div>

        <div id="divOrgOwnerSearchNoResults" style="display:none;">
	        <div>
                <div data-role="fieldcontain">
                    <label id="lblOrgOwnerNotFound">No organizations found</label>
                </div>
            </div>
        </div>
    </div>
     <div id="divExpressCustomPopup" data-role="popup">
        <ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="divider">Option</li>
            <li><a id="btnExpress">Express (All Products)</a></li>
            <li><a id="btnCustom">Custom (Selected Products)</a></li>
        </ul>
    </div>
</div>

<div id="pageBarcodeSearch" data-role="page">
    <div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>Search All Publicly Available Products</h1>
    </div>

    <div data-role="content">
                
               
        <div style="padding:5px;">
 
            <div class="ui-grid-a" style="margin:0px; padding: 2px;">
                <div class="ui-block-a" style="width:78%;height: 50px;padding: 0px;margin-bottom:5px;">
                    <form id="frmSearchPublicProduct">
                        <input id="txtPublicSearch" type="text" placeholder="Search products" style="height: 40px; padding:0px;" autofocus />
                    </form>
                </div>
                <div class="ui-block-b" style="width: 22%;height: 50px;padding: 0px;margin:0px;">
                    <center><div >
                        <img src="/Content/images/barcode96x96.png" class="detailimage2" id="butBarCode" data-role="button" data-corners="false"/>
                    </div>
                    </center>
                </div>
            </div>
 
        </div>
        
    </div>

</div>

<div id="pageCustomAdd" data-role="page">
    <div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>Custom Add</h1>
    </div>

    <div data-role="content">
        <div data-role="fieldcontain">
            <label for="selFilter" class="select">Filter:</label>
            <select id="selFilter">
            <option value="">Please select</option>
            <option value="Brand">By Brand</option>
            <option value="Category">By Category</option>
            </select>
        </div>   

        <div id="divResult" style="display:none;">
            <ul data-role="listview" id="lstResultContainer" data-inset="true">   
                <li>
                    <table id="tblResult" style="width:100%">                    
                    </table>  
                </li>
            </ul>
            <div>
                <br />
                <div style="text-align: center">
                    <a id="btnAddProductsCustom" data-role="button"><h3>Add</h3></a>
                </div>
                 <div style="text-align: center">
                    <a id="btnAddProductsCustomCancel" data-role="button"><h3>Cancel</h3></a>
                </div>
            </div>

        </div>
        <div id="divNoResult" style="display:none;">No record(s) found</div>

       
    </div>

</div>
<div id="pageProductNamePreview" data-role="page">
    <div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>Preview Products</h1>
    </div>

    <div data-role="content">
       
        <div id="divResultName">
        You are about to add the following products to your company-wide product list:
            <ul data-role="listview" id="lstResultContainerName" data-inset="true">   
                <li>
                    <table id="tblResultName" style="width:100%">                    
                    </table>  
                </li>
            </ul>
            <div>
                <br />
                <div style="text-align: center">
                    <a id="btnAddProductsContinue" data-role="button"><h3>Continue</h3></a>
                </div>
                <div style="text-align: center">
                    <a id="btnAddProductsCancel" data-role="button"><h3>Cancel</h3></a>
                </div>
            </div>

        </div>
        <div id="divNoResultName" style="display:none;">No record(s) found</div>

       
    </div>

</div>










