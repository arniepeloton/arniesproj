@Code
    ViewData("Title") = "Vocabulary"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/Vocabulary/VocabularyMenu.js?crc=')]);
    </script>
    
End Section

<div id="pageVocabularyList" data-role="page">

    <div data-role="header">
        <h1>Vocabulary List</h1>
    </div>

    <div data-role="content">

        <form id="frmVocabularyBrowser" style="margin: 0 -7px">
            <div>
                <input id="txtVocabularySearch" type="search" placeholder="Search a Word" value="" />
            </div>
        </form>

        <div id="divVocabularySearchResults" style="display:none;">

            <div class="ListViewMarginBottom" id="divVocabularyRecordCount"></div>

            <ul id="lstVocabulary" data-role="listview" data-filter="true">
            </ul>

        </div>

        <div id="divVocabularySearchNoResults" style="display:none;">
	        <div>
                <div data-role="fieldcontain">
                    <label id="lblVocabularyNotFound"></label>
                </div>
            </div>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center;"> <a id="btnAddNewVocabulary" data-role="button" style="display:none;">Add New Word</a> </div>
	        <div style="text-align:center"> <a id="btnVocabularyBackToHome" data-role="button">Back To Home</a> </div>
        </div>

    </div>

</div>







