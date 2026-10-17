@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/PIMFeatureGroup/PIMFeatureGroupInfo.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryCountryLanguageVocabularyLabel.js?crc=')
        ]);
    </script>
    
End Section

<div id="pagePIMFeatureGroupInfo" data-role="page">

    <div data-role="header">
        <h1>Label Info</h1>
    </div>

    <div data-role="content" >

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtPIMFeatureGroupID"/>                
        </div>

        <div data-role="fieldcontain">
            <div style="display:none;"><input type="text" readonly id="txtPIMFeatureGroupLabelID"/></div>
	        <div><label for="btnPIMFeatureGroupLabel">Feature Group: <span style="color: #FF0000;" >*</span></label></div>
            <fieldset data-role="controlgroup">
                <button id="btnPIMFeatureGroupLabel" data-role="button">Please Select</button>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
	            <div><label for="txtPIMFeatureGroupLabelCountry">Country:</label></div>
	            <div><input type="text" readonly id="txtPIMFeatureGroupLabelCountry"/></div>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
	            <div><label for="txtPIMFeatureGroupLabelLanguage">Language:</label></div>
	            <div><input type="text" readonly id="txtPIMFeatureGroupLabelLanguage"/></div>
            </fieldset>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPIMFeatureGroupInfoDone" data-role="button">Done</a> </div>
        </div>

    </div>

</div>

