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

        <div data-role="fieldcontain" style="display:none;" >
	        <input type="text" readonly id="txtPIMProductFamilyID"/>                
        </div>

        <div data-role="fieldcontain">
            <div style="display:none;"><input type="text" readonly id="txtPIMProductFamilyLabelID"/></div>
	        <div><label for="btnPIMProductFamilyLabel">Product Family: <span style="color: #FF0000;" >*</span></label></div>
            <fieldset data-role="controlgroup">
                <button id="btnPIMProductFamilyLabel" data-role="button">Please Select</button>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
	        <div><label for="txtPIMProductFamilyLabelCountry">Country:</label></div>
            <fieldset data-role="controlgroup">
	            <input type="text" readonly id="txtPIMProductFamilyLabelCountry"/>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
	        <div><label for="txtPIMProductFamilyLabelLanguage">Language:</label></div>
            <fieldset data-role="controlgroup">
	            <input type="text" readonly id="txtPIMProductFamilyLabelLanguage"/>
            </fieldset>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPIMProductFamilyInfoDone" data-role="button">Done</a> </div>
        </div>

    </div>

</div>

