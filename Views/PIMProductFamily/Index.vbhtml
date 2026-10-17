@Code
    ViewData("Title") = "Product Family List"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/PIMProductFamily/PIMProductFamilyMenu.js?crc=')]);
    </script>
    
End Section

<div id="pagePIMProductFamilyList" data-role="page">

    <div data-role="header">
        <h1>Product Family List</h1>
    </div>

    <div data-role="content">

        <form id="frmPIMProductFamilyBrowser" style="margin: 0 -7px">
            <div>
                <input id="txtPIMProductFamilySearch" type="search" placeholder="Search a Product Family" value="" />
            </div>
        </form>

        <div id="divPIMProductFamilySearchResults" style="display:none;">

            <div class="ListViewMarginBottom" id="divPIMProductFamilyRecordCount"></div>

            <ul id="lstPIMProductFamily" data-role="listview" data-filter="true">
            </ul>

        </div>

        <div id="divPIMProductFamilySearchNoResults" style="display:none;">
	        <div>
                <div data-role="fieldcontain">
                    <label id="lblPIMProductFamilyNotFound"></label>
                </div>
            </div>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center;"> <a id="btnAddNewPIMProductFamily" data-role="button" style="display:none;">Add New Prodct Family</a> </div>
	        <div style="text-align:center"> <a id="btnPIMProductFamilyBackToHome" data-role="button">Back To Home</a> </div>
        </div>

    </div>

</div>







