@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/PIMProductFamily/PIMProductFamilyInfo.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryCountryLanguageVocabularyLabel.js?crc=')
        ]);
    </script>
    
End Section

<div id="pagePIMProductFamilyInfo" data-role="page">

    <div data-role="header">
        <h1>Product Family Info</h1>
    </div>

    <div data-role="content" >

        <div data-role="fieldcontain">
	        <input type="text" readonly style="display:none;" id="txtPIMProductFamilyID"/>                
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	            <div><label for="btnPIMProductFamilyLabel">Product Family: <span style="color: #FF0000;" >*</span></label></div>
                <div><input type="text" readonly style="display:none;" id="txtPIMProductFamilyLabelID"/></div>
                <div><button id="btnPIMProductFamilyLabel" data-role="button">Please Select</button></div>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	            <div><label for="txtPIMProductFamilyLabelCountry">Country:</label></div>
	            <div><input type="text" readonly id="txtPIMProductFamilyLabelCountry"/></div>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	            <div><label for="txtPIMProductFamilyLabelLanguage">Language:</label></div>
	            <div><input type="text" readonly id="txtPIMProductFamilyLabelLanguage"/></div>
            </fieldset>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPIMProductFamilyInfoDone" data-role="button">Done</a> </div>
        </div>

    </div>

</div>

