@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/PIMUNSPSCCommodityFeature/PIMUNSPSCCommodityFeatureInfo.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMFeature.js?crc=')
        ]);
    </script>
    
End Section

<div id="pagePIMUNSPSCCommodityFeatureInfo" data-role="page">

    <div data-role="header">
        <h1>UNSPSC Commodity Feature Info</h1>
    </div>

    <div data-role="content" >

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtPIMUNSPSCCommodityFeatureRecID"/>                
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
	            <div><label for="txtPIMUNSPSCComFeatureSegment">UNSPSC Segment:</label></div>
	            <div><input type="text" readonly id="txtPIMUNSPSCComFeatureSegment"/></div>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
	            <div><label for="txtPIMUNSPSCComFeatureFamily">UNSPSC Family:</label></div>
	            <div><input type="text" readonly id="txtPIMUNSPSCComFeatureFamily"/></div>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
	            <div><label for="txtPIMUNSPSCComFeatureClass">UNSPSC Class:</label></div>
	            <div><input type="text" readonly id="txtPIMUNSPSCComFeatureClass"/></div>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
	            <div><label for="txtPIMUNSPSCComFeatureCommodity">UNSPSC Commodity:</label></div>
	            <div><input type="text" readonly id="txtPIMUNSPSCComFeatureCommodity"/></div>
            </fieldset>
        </div>

        <div>
            <ul data-role="listview" id="lstPIMFeatureStructureContainer" data-inset="true">   
				<li data-role="list-divider">Commodity Feature</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnAddPIMUNSPSCCommFeature" data-role="button" data-icon="plus" data-inline="true">Add Feature</a></div>
                    <div>
                        <ul data-role="listview" id="lstPIMUNSPSCCommFeature" data-inset="true">
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPIMUNSPSCCommFeatureInfoDone" data-role="button">Done</a> </div>
        </div>

    </div>

</div>

<div id="pageUNSPSCCommFeature" data-role="page">

    <div data-role="header">
        <h1>Feature Details</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtPIMUNSPSCCommFeatureID"/>                
        </div>

        <div data-role="fieldcontain">
            <div style="display:none;"><input type="text" readonly id="txtPIMUNSPSCCommFeatureFeatureID"/></div>
	        <div><label for="btnPIMUNSPSCCommFeature">Feature: <span style="color: #FF0000;" >*</span></label></div>
            <fieldset data-role="controlgroup">
                <button id="btnPIMUNSPSCCommFeature" data-role="button">Please Select</button>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
	        <div>Is this a key feature?</div>
            <select id="selKeyFeature" data-role="slider">
				<option value="0">No</option>
	            <option value="1">Yes</option>
            </select>
        </div>

        <div data-role="fieldcontain">
	        <div>Is this a searchable feature?</div>
            <select id="selSearchableFeature" data-role="slider">
				<option value="0">No</option>
	            <option value="1">Yes</option>
            </select>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
	            <div><label for="txtPIMUNSPSCCommFeatureSortOrder">Sort Order: <span style="color: #FF0000;" >*</span></label></div>
	            <div><input type="number" id="txtPIMUNSPSCCommFeatureSortOrder"/></div>              
            </fieldset>
        </div>

        <div>  
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPIMUNSPSCCommFeatureSave" data-role="button">Ok</a> </div>
	        <div style="text-align:center"> <a id="btnPIMUNSPSCCommFeatureRemove" data-role="button">Remove</a> </div>
	        <div style="text-align:center"> <a id="btnPIMUNSPSCCommFeatureCancel" data-role="button">Cancel</a> </div>
        </div>

    </div>

</div>