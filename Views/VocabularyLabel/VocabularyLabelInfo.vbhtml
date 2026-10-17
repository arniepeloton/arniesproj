@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/VocabularyLabel/VocabularyLabelInfo.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryCountryLanguage.js?crc=')
        ]);
    </script>
    
End Section

<div id="pageVocabularyLabelInfo" data-role="page">

    <div data-role="header">
        <h1>Label Info</h1>
    </div>

    <div data-role="content" >

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtVocabularyLabelID"/>                
        </div>

        <div data-role="fieldcontain">
            <div style="display:none;"><input type="text" readonly id="txtVocabularyLabelLanguageID"/></div>
	        <div><label for="btnVocabularyLabelLanguage">Language: <span style="color: #FF0000;" >*</span></label></div>
            <fieldset data-role="controlgroup">
                <button id="btnVocabularyLabelLanguage" data-role="button">Please Select</button>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
	            <div><label for="txtVocabularyLabel">Label: <span style="color: #FF0000;" >*</span></label></div>
	            <div><input type="text" id="txtVocabularyLabel"/></div>
            </fieldset>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnVocabularyLabelInfoDone" data-role="button">Done</a> </div>
        </div>

    </div>

</div>

