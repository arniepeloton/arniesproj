@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/Vocabulary/VocabularyInfo.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryCountryLanguage.js?crc=')
        ]);
    </script>
    
End Section

<div id="pageVocabularyInfo" data-role="page">

    <div data-role="header">
        <h1>Word Info</h1>
    </div>

    <div data-role="content" >

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtVocabularyID"/>                
        </div>

        <div data-role="fieldcontain">
            <div style="display:none;"><input type="text" readonly id="txtVocabularyLanguageID"/></div>
	        <div><label for="btnVocabularyLanguage">Language: <span style="color: #FF0000;" >*</span></label></div>
            <fieldset data-role="controlgroup">
                <button id="btnVocabularyLanguage" data-role="button">Please Select</button>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
	            <div><label for="txtVocabularyWord">Word: <span style="color: #FF0000;" >*</span></label></div>
	            <div><input type="text" id="txtVocabularyWord"/></div>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
	            <div><label for="txtVocabularyDefinition">Definition:</label></div>
	            <div><textarea id="txtVocabularyDefinition"></textarea></div>  
            </fieldset>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnVocabularyInfoDone" data-role="button">Done</a> </div>
        </div>

    </div>

</div>

