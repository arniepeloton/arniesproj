@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/PIMProduct/PIMProductInfo.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryCustomers.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMProductFamily.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryUNSPSCCommodity.js?crc=')
        ]);
    </script>
    
End Section

<div id="pagePIMProductInfo" data-role="page">

    <div data-role="header">
        <h1>Product Info</h1>
    </div>

    <div data-role="content" >

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtPIMProductID"/>                
        </div>

        <div data-role="fieldcontain">
            <div style="display:none;"><input type="text" readonly id="txtPIMProductOrganizationID"/></div>
	        <div><label for="btnPIMProductOrganization">Product: <span style="color: #FF0000;" >*</span></label></div>
            <fieldset data-role="controlgroup">
                <button id="btnPIMProductOrganization" data-role="button">Please Select</button>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
	            <div><label for="txtPIMProductNo">Product No.: <span style="color: #FF0000;" >*</span></label></div>
	            <div><input type="text" id="txtPIMProductNo"/></div>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
	            <div><label for="txtPIMProductName">Product Name: <span style="color: #FF0000;" >*</span></label></div>
	            <div><input type="text" id="txtPIMProductName"/></div>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <label><input type="checkbox" id="chkPIMProductIsActive" /> Is this an active product?</label>
        </div>

        <div data-role="fieldcontain">
            <div style="display:none;"><input type="text" readonly id="txtPIMProductFamilyID"/></div>
	        <div><label for="btnPIMProductFamily">Product Family: <span style="color: #FF0000;" >*</span></label></div>
            <fieldset data-role="controlgroup">
                <button id="btnPIMProductFamily" data-role="button">Please Select</button>
            </fieldset>
        </div>

        <div>
            <ul data-role="listview" data-inset="true">
			    <li data-role="list-divider">Product UNSPSC Category</li>
				<li>
                    <div data-role="fieldcontain">
                        <div style="display:none;"><input type="text" readonly id="txtPIMProductUNSPSCSegmentIDOrig"/></div>
                        <div style="display:none;"><input type="text" readonly id="txtPIMProductUNSPSCSegmentID"/></div>
                        <fieldset data-role="controlgroup">
	                        <div><label for="txtPIMProductUNSPSCSegment">Segment:</label></div>
	                        <div><input type="text" readonly id="txtPIMProductUNSPSCSegment"/></div>
                        </fieldset>
                    </div>
                    <div data-role="fieldcontain">
                        <div style="display:none;"><input type="text" readonly id="txtPIMProductUNSPSCFamilyIDOrig"/></div>
                        <div style="display:none;"><input type="text" readonly id="txtPIMProductUNSPSCFamilyID"/></div>
                        <fieldset data-role="controlgroup">
	                        <div><label for="txtPIMProductUNSPSCFamily">Family: </label></div>
	                        <div><input type="text" readonly id="txtPIMProductUNSPSCFamily"/></div>
                        </fieldset>
                    </div>
                    <div data-role="fieldcontain">
                        <div style="display:none;"><input type="text" readonly id="txtPIMProductUNSPSCClassIDOrig"/></div>
                        <div style="display:none;"><input type="text" readonly id="txtPIMProductUNSPSCClassID"/></div>
                        <fieldset data-role="controlgroup">
	                        <div><label for="txtPIMProductUNSPSCClass">Class: </label></div>
	                        <div><input type="text" readonly id="txtPIMProductUNSPSCClass"/></div>
                        </fieldset>
                    </div>
                    <div data-role="fieldcontain">
                        <div style="display:none;"><input type="text" readonly id="txtPIMProductUNSPSCCommIDOrig"/></div>
                        <div style="display:none;"><input type="text" readonly id="txtPIMProductUNSPSCCommID"/></div>
	                    <div><label for="btnPIMProductUNSPSCCommdity">UNSPSC Commodity: <span style="color: #FF0000;" >*</span></label></div>
                        <fieldset data-role="controlgroup">
                            <button id="btnPIMProductUNSPSCCommdity" data-role="button">Please Select</button>
                        </fieldset>
                    </div>
				</li>

			</ul>
        <div>

        <div>
            <ul data-role="listview" id="lstPIMUNSPSCCommFeature" data-inset="true">   
				<li data-role="list-divider">Product Feature</li>
                <li>
                    <div id="divProductFeature">
                    <div>
                </li>
            </ul>

        <div>

        <br />
        <br />
	    <div style="text-align:center"> <a id="btnPIMProductInfoDone" data-role="button">Done</a> </div>
        </div>

    </div>

</div>

