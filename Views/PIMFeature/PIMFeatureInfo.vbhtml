@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/PIMFeature/PIMFeatureInfo.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMMeasure.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryCountryLanguageVocabularyLabel.js?crc=')
        ]);
    </script>
    
End Section

<div id="pagePIMFeatureInfo" data-role="page">

    <div data-role="header">
        <h1>Feature Info</h1>
    </div>

    <div data-role="content" >

        <div data-role="fieldcontain" style="display:none;" >
	        <input type="text" readonly id="txtPIMFeatureID"/>                
        </div>

        <div data-role="fieldcontain">
            <div style="display:none;"><input type="text" readonly id="txtPIMFeatureNameID"/></div>
	        <div><label for="btnPIMFeatureName">Feature: <span style="color: #FF0000;" >*</span></label></div>
            <fieldset data-role="controlgroup">
                <button id="btnPIMFeatureName" data-role="button">Please Select</button>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <div style="display:none;"><input type="text" readonly id="txtPIMFeatureLabelID"/></div>
	        <div><label for="btnPIMFeatureLabel">Label: <span style="color: #FF0000;" >*</span></label></div>
            <fieldset data-role="controlgroup">
                <button id="btnPIMFeatureLabel" data-role="button">Please Select</button>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
	        <label for="selPIMFeatureType" class="select">Feature Type:</label>
            <select id="selPIMFeatureType">
	            <option value="">Please Select</option>
				<option value="Comment/Essay Box">Comment/Essay Box</option>
				<option value="Date Only">Date</option>
				<option value="Time Only">Time</option>
				<option value="Date and Time">Date and Time</option>
				<option value="Multiple Option">Multiple Option</option>
				<option value="Textbox">Textbox</option>
				<option value="Textbox (Numeric)">Textbox (Numeric)</option>
				<option value="Textbox (Decimal)">Textbox (Decimal)</option>
            </select>
        </div>

        <div id="divPIMFeatureStructure" style="display:none;">

            <div data-role="fieldcontain">
	            <label for="selPIMFeatureDisplayFormat" class="select">Display Format:</label>
                <select id="selPIMFeatureDisplayFormat">
	                <option value="">Please Select</option>
				    <option value="Radio Button">Radio Button</option>
				    <option value="Drop Down List">Drop Down List</option>
                </select>
            </div>

            <ul data-role="listview" id="lstPIMFeatureStructureContainer" data-inset="true">   
				<li data-role="list-divider">Structured Options</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnAddPIMFeatureOption" data-role="button" data-icon="plus" data-inline="true">Add Option</a></div>
                    <div>
                        <ul data-role="listview" id="lstPIMFeatureOption" data-inset="true">
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div data-role="fieldcontain">
	        <div>Do you want to show a measurement?</div>
            <select id="selShowMeasurement" data-role="slider">
				<option value="0">No</option>
	            <option value="1">Yes</option>
            </select>
        </div>

        <div id="divFeatureMeasurement" style="display:none;">
            <div data-role="fieldcontain">
                <div style="display:none;"><input type="text" readonly id="txtPIMFeatureMeasureID"/></div>
	            <div><label for="btnPIMFeatureMeasure">Measurement: </label></div>
                <fieldset data-role="controlgroup">
                    <button id="btnPIMFeatureMeasure" data-role="button">Please Select</button>
                </fieldset>
            </div>

            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup">
	                <div><label for="txtPIMFeatureMeasureSign">Sign:</label></div>
	                <div><input type="text" readonly id="txtPIMFeatureMeasureSign"/></div>
                </fieldset>
            </div>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPIMFeatureInfoDone" data-role="button">Done</a> </div>
        </div>

    </div>

</div>

<div id="pageFeatureOption" data-role="page">

    <div data-role="header">
        <h1>Feature Options</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtPIMFeatureOptionID"/>                
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
	            <div><label for="txtPIMFeatureOption">Option: <span style="color: #FF0000;" >*</span></label></div>
	            <div><input type="text" id="txtPIMFeatureOption"/></div>              
            </fieldset>
        </div>

        <div>  
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPIMFeatureOptionSave" data-role="button">Ok</a> </div>
	        <div style="text-align:center"> <a id="btnPIMFeatureOptionRemove" data-role="button">Remove</a> </div>
	        <div style="text-align:center"> <a id="btnPIMFeatureOptionCancel" data-role="button">Cancel</a> </div>
        </div>

    </div>

</div>