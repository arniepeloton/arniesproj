<script>
    require([GetPathFormat('/Scripts/sources/Organization/OrganizationCustomerFOWSTProductList.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMProduct.js?crc=')
        ]);
</script>

<div id="pageOrgCustFOWSTProductList" style="display:block;">
    <div data-role="header">
        <h2>Customer's FOWST Product List</h2>
    </div>

    <div data-role="content">
        <div data-role="fieldcontain"><a id="btnOrgCustFOWSTProductAdd" data-role="button" data-icon="plus" data-inline="true">Add SKU to FOWST</a></div>

        <ul data-role="listview" id="lstOrgCustProductContainer" data-inset="true">   
			<li data-role="list-divider">Your Customers's FOWST product List</li>
            <li>
                <form id="frmOrgCustFowstProductSearch">
                    <div><input id="txtOrgCustFowstProductSearch" type="search" placeholder="Search Customer's FOWST Products" value="" /></div>
                </form>

                <div data-role="fieldcontain" id="divOrgCustFOWSTProductRecordCount"></div>
                <div>
                    <ul data-role="listview" id="lstOrgCustFOWSTProductList" data-inset="true">
                    </ul>
                </div>
            </li>
        </ul>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgCustFOWSTProductListDone" data-role="button"><h3>Done</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgCustFOWSTProductInfo" style="display:none;">
    <div data-role="header">
        <h2>Customer's FOWST Product Information</h2>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgCustFOWSTProdID"/>                
        </div>

        <div style="display:none;"><input type="text" readonly id="txtOrgCustFOWSTID"/></div>
        <div style="display:none;"><input type="text" readonly id="txtOrgCustFOWSTProductID"/></div>
        <div data-role="fieldcontain">
	        <label for="btnOrgCustFOWSTProduct">Product:</label>
            <fieldset data-role="controlgroup">
                <button id="btnOrgCustFOWSTProduct" data-role="button">Please Select</button>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
	        <label for="txtOrgCustFOWSTProductReorderPoint">Reorder Point for this FOWST Location : <span style="color: #FF0000;">*</span></label>
            <fieldset data-role="controlgroup">
	            <input type="text" id="txtOrgCustFOWSTProductReorderPoint"/>
            </fieldset>
        </div>
        
        <div>
            <ul data-role="listview" id="lstCustOrgFOWSTProductBinContainer" data-inset="true">   
				<li data-role="list-divider">This SKU is located in the following bins.</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnAddOrgCustFOWSTProductBin" data-role="button" data-icon="plus" data-inline="true">Add another bin location for this SKU</a></div>
                    <div>
                        <ul data-role="listview" id="lstOrgCustFOWSTProductBin" data-inset="true">
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgCustFOWSTProductSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgCustFOWSTProductCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgCustFOWSTProductBinInfo" style="display:none;">

    <div data-role="header">
        <h2>Product Bin Information</h2>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgCustFOWSTProductBinID"/>                
        </div>

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgCustFOWSTBinID"/>                
        </div>
        <div data-role="fieldcontain">
	        <label for="txtOrgCustFOWSTBin">Bin : </label>
            <fieldset data-role="controlgroup">
	            <input type="text" id="txtOrgCustFOWSTBin"/>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
	        <label for="txtOrgCustFOWSTBinMin">Minimum Qty : <span style="color: #FF0000;" >*</span></label>
            <fieldset data-role="controlgroup">
	            <input type="number" id="txtOrgCustFOWSTBinMin"/>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
	        <label for="txtOrgCustFOWSTBinMax">Maximum Qty : <span style="color: #FF0000;" >*</span></label>
            <fieldset data-role="controlgroup">
	            <input type="number" id="txtOrgCustFOWSTBinMax"/>
            </fieldset>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgCustFOWSTProductBinSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgCustFOWSTProductBinCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>















