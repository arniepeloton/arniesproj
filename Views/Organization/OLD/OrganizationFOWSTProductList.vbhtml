@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
    <script>
        require([GetPathFormat('/Scripts/sources/Organization/OrganizationFOWSTProductList.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMProduct.js?crc=')
        ]);
    </script>

End Section

<div id="pageOrgFOWSTProductList" data-role="page">
    <div data-role="header">
        <h1>FOWST Product List</h1>
    </div>

    <div data-role="content">
        <div data-role="fieldcontain"><a id="btnOrgFOWSTProductAdd" data-role="button" data-icon="plus" data-inline="true">Add SKU to FOWST Location</a></div>

        <form id="frmOrgFowstProductSearch">
            <div><input id="txtOrgFowstProductSearch" type="search" value="" /></div>
        </form>

        <div data-role="fieldcontain" id="divOrgFOWSTProductRecordCount"></div>
        <div>
            <ul data-role="listview" id="lstOrgFOWSTProductList" data-inset="true">
            </ul>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgFOWSTProductListDone" data-role="button"><h3>Done</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgFOWSTProductInfo" data-role="page">
    <div data-role="header">
        <h1>FOWST Product Info</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgFOWSTProdID"/>                
        </div>

        <div style="display:none;"><input type="text" readonly id="txtOrgFOWSTID"/></div>
        <div style="display:none;"><input type="text" readonly id="txtOrgFOWSTProductID"/></div>
        <div data-role="fieldcontain">
	        <label for="btnOrgFOWSTProduct">Product:</label>
            <fieldset data-role="controlgroup">
                <button id="btnOrgFOWSTProduct" data-role="button">Please Select</button>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
	        <label for="txtOrgFOWSTProductReorderPoint">Reorder Point for this FOWST Location : <span style="color: #FF0000;">*</span></label>
            <fieldset data-role="controlgroup">
	            <input type="text" id="txtOrgFOWSTProductReorderPoint"/>
            </fieldset>
        </div>

        <div>
            <ul data-role="listview" id="lstOrgFOWSTProductBinContainer" data-inset="true">   
				<li data-role="list-divider">This SKU is located in the following bins.</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnAddOrgFOWSTProductBin" data-role="button" data-icon="plus" data-inline="true">Add another bin location for this SKU</a></div>
                    <div>
                        <ul data-role="listview" id="lstOrgFOWSTProductBin" data-inset="true">
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgFOWSTProductSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgFOWSTProductCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgFOWSTProductBinInfo" data-role="page">

    <div data-role="header">
        <h1>Product Bin Info</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgFOWSTProductBinID"/>                
        </div>

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgFOWSTBinID"/>                
        </div>
        <div data-role="fieldcontain">
	        <label for="txtOrgFOWSTBin">Bin : </label>
            <fieldset data-role="controlgroup">
	            <input type="text" id="txtOrgFOWSTBin"/>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
	        <label for="txtOrgFOWSTBinMin">Min : <span style="color: #FF0000;" >*</span></label>
            <fieldset data-role="controlgroup">
	            <input type="number" id="txtOrgFOWSTBinMin"/>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
	        <label for="txtOrgFOWSTBinMax">Max : <span style="color: #FF0000;" >*</span></label>
            <fieldset data-role="controlgroup">
	            <input type="number" id="txtOrgFOWSTBinMax"/>
            </fieldset>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgFOWSTProductBinSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgFOWSTProductBinCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>















