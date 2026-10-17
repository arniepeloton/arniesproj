@Code
    ViewData("Title") = "Feature List"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/PIMFeature/PIMFeatureMenu.js?crc=')]);
    </script>
    
End Section

<div id="pagePIMFeatureList" data-role="page">

    <div data-role="header">
        <h1>Feature List</h1>
    </div>

    <div data-role="content">

        <form id="frmPIMFeatureBrowser" style="margin: 0 -7px">
            <div>
                <input id="txtPIMFeatureSearch" type="search" placeholder="Search a Feature" value="" />
            </div>
        </form>

        <div id="divPIMFeatureSearchResults" style="display:none;">

            <div class="ListViewMarginBottom" id="divPIMFeatureRecordCount"></div>

            <ul id="lstPIMFeature" data-role="listview" data-filter="true">
            </ul>

        </div>

        <div id="divPIMFeatureSearchNoResults" style="display:none;">
	        <div>
                <div data-role="fieldcontain">
                    <label id="lblPIMFeatureNotFound"></label>
                </div>
            </div>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center;"> <a id="btnAddNewPIMFeature" data-role="button" style="display:none;">Add New Feature</a> </div>
	        <div style="text-align:center"> <a id="btnPIMFeatureBackToHome" data-role="button">Back To Home</a> </div>
        </div>

    </div>

</div>







