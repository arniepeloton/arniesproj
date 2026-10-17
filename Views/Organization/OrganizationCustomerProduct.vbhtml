<script>
    require([GetPathFormat('/Scripts/sources/Organization/OrganizationCustomerProduct.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMProduct.js?crc=')
        ]);
</script>

<div id="pageOrgCustProduct" style="display:block;">

    <div id"divHeader" data-role="header">
        <h2>Customer's Product List</h2>
    </div>

    <div data-role="content" >

        <div>
            <div data-role="fieldcontain"><a id="btnOrgCustProductAdd" data-role="button" data-icon="plus" data-inline="true">Add Product</a></div>
            <ul data-role="listview" id="lstOrgCustProductContainer" data-inset="true">   
				<li data-role="list-divider">Your customers's current product list</li>
                <li>
                    <form id="frmOrgCustProductSearch">
                        <div><input id="txtOrgCustProductSearch" type="search" placeholder="Search Customer Products" value="" /></div>
                    </form>
                    <div data-role="fieldcontain">This list is limited to show 50 SKU records only.</div>
                    <div data-role="fieldcontain" id="divOrgCustProductRecordCount" style="display:none;">
                        <label id="lblCustProductRecordcount"></label>
                        <a id="lnkCustProductList" style="display:none; cursor:pointer; font-size:16px;"> Click here to download Excel file to view full company product list.</a>
                    </div>
                    <div>
                        <ul data-role="listview" id="lstOrgCustProduct" data-inset="true">   
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgCustProductDone" data-role="button"><h3>Done</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgCustProductInfo" style="display:none;">

    <div data-role="header">
        <h2>Customer Product Info</h2>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgCustomerID"/>                
	        <input type="text" readonly id="txtOrgCustPIMProductID"/>                
        </div>

        <div style="display:none;"><input type="text" readonly id="txtOrgCustProductID"/></div>
        <div data-role="fieldcontain">
	        <label for="btnOrgCustProduct">Product:</label>
            <button id="btnOrgCustProduct" data-role="button">Please Select</button>
            
        </div>

        <div data-role="fieldcontain">
	        <label for="selOrgCustProductActivationDateMonth">Activation Date: <span style="color: #FF0000;" >*</span></label>
            <fieldset data-role="controlgroup" data-type="horizontal">
                <select id="selOrgCustProductActivationDateMonth">
	                <option value="">MMM</option>
                </select>
                <select id="selOrgCustProductActivationDateDay">
	                <option value="">DD</option>
                </select>                
                <select id="selOrgCustProductActivationDateYear">
	                <option value="">YYYY</option>
                </select>                
            </fieldset>
       </div>

         <div data-role="fieldcontain">
	        <label for ="selOrgCustProductDeactivate">Deactivate record?</label>
            <select id="selOrgCustProductDeactivate" data-role="slider">
			    <option value="0">No</option>
	            <option value="1">Yes</option>
            </select>
        </div>

        <div id="divOrgCustProductDeactivateDetails" style="display:none;">

	        <label for ="selOrgCustProductDateInActiveMonth">Deactivation Date: <span style="color: #FF0000;" >*</span></label>
            <fieldset data-role="controlgroup" data-type="horizontal">
                <select id="selOrgCustProductDateInActiveMonth">
	                <option value="">MMM</option>
                </select>
                <select id="selOrgCustProductDateInActiveDay">
	                <option value="">DD</option>
                </select>                
                <select id="selOrgCustProductDateInActiveYear">
	                <option value="">YYYY</option>
                </select>                
            </fieldset>

            <div data-role="fieldcontain">
	            <label for="txtOrgCustProductInActiveReason">Reason for deactivating:</label>
                <textarea id="txtOrgCustProductInActiveReason"></textarea>
               
            </div>

            <div data-role="fieldcontain">
	            <label for="txtOrgCustProductComments">Comments:</label>
                <textarea id="txtOrgCustProductComments"></textarea>
                
            </div>

	        <label for ="selOrgCustProductBan">Ban this record for future transactions?</label>
            <select id="selOrgCustProductBan" data-role="slider">
				<option value="0">No</option>
	            <option value="1">Yes</option>
            </select>

        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgCustProductInfoSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgCustProductInfoCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>
