@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/Country/CountryLanguage.js?crc=')]);
    </script>

End Section

<div id="pageCountryLanguageList" data-role="page">
    <div data-role="header">
        <h1>Country Language List</h1>
    </div>

    <div data-role="content">
        <div data-role="fieldcontain"><a id="btnCountryLanguageAdd" data-role="button" data-icon="plus" data-inline="true">Add Language</a></div>
        <div data-role="fieldcontain" id="divCountryLanguageRecordCount"></div>
        <div>
            <ul data-role="listview" id="lstCountryLanguage" data-inset="true">
            </ul>
        </div>
        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnCountryLanguageDone" data-role="button">Done</a> </div>
        </div>

    </div>

</div>


<div id="pageCountryLanguageDetails" data-role="page">
    <div data-role="header">
        <h1>Language Details</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtCountryLanguageID"/>                
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	            <div><label for="txtCountryLanguage">Language: <span style="color: #FF0000;" >*</span></label></div>
	            <div><input type="text" id="txtCountryLanguage"/></div>
            </fieldset>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"><a id="btnCountryLanguageSave" data-role="button">Ok</a></div>
	        <div style="text-align:center"> <a id="btnCountryLanguageRemove" data-role="button">Remove</a> </div>
	        <div style="text-align:center"> <a id="btnCountryLanguageCancel" data-role="button">Cancel</a> </div>
        </div>

    </div>

</div>

