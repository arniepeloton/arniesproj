@Code
    ViewData("Title") = "Label List"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/VocabularyLabel/VocabularyLabelMenu.js?crc=')]);
    </script>
    
End Section

<div id="pageVocabularyLabelList" data-role="page">

    <div data-role="header">
        <h1>Label List</h1>
    </div>

    <div data-role="content">

        <form id="frmVocabularyLabelBrowser" style="margin: 0 -7px">
            <div>
                <input id="txtVocabularyLabelSearch" type="search" placeholder="Search a Label" value="" />
            </div>
        </form>

        <div id="divVocabularyLabelSearchResults" style="display:none;">

            <div class="ListViewMarginBottom" id="divVocabularyLabelRecordCount"></div>

            <ul id="lstVocabularyLabel" data-role="listview" data-filter="true">
            </ul>

        </div>

        <div id="divVocabularyLabelSearchNoResults" style="display:none;">
	        <div>
                <div data-role="fieldcontain">
                    <label id="lblVocabularyLabelNotFound"></label>
                </div>
            </div>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center;"> <a id="btnAddNewVocabularyLabel" data-role="button" style="display:none;">Add New Label</a> </div>
	        <div style="text-align:center"> <a id="btnVocabularyLabelBackToHome" data-role="button">Back To Home</a> </div>
        </div>

    </div>

</div>







