@Code
    ViewData("Title") = "Feature Group List"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/PIMFeatureGroup/PIMFeatureGroupMenu.js?crc=')]);
    </script>
    
End Section

<div id="pagePIMFeatureGroupList" data-role="page">

    <div data-role="header">
        <h1>Feature Group List</h1>
    </div>

    <div data-role="content">

        <form id="frmPIMFeatureGroupBrowser" style="margin: 0 -7px">
            <div>
                <input id="txtPIMFeatureGroupSearch" type="search" placeholder="Search a Feature Group" value="" />
            </div>
        </form>

        <div id="divPIMFeatureGroupSearchResults" style="display:none;">

            <div class="ListViewMarginBottom" id="divPIMFeatureGroupRecordCount"></div>

            <ul id="lstPIMFeatureGroup" data-role="listview" data-filter="true">
            </ul>

        </div>

        <div id="divPIMFeatureGroupSearchNoResults" style="display:none;">
	        <div>
                <div data-role="fieldcontain">
                    <label id="lblPIMFeatureGroupNotFound"></label>
                </div>
            </div>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center;"> <a id="btnAddNewPIMFeatureGroup" data-role="button" style="display:none;">Add New Feature Group</a> </div>
	        <div style="text-align:center"> <a id="btnPIMFeatureGroupBackToHome" data-role="button">Back To Home</a> </div>
        </div>

    </div>

</div>







