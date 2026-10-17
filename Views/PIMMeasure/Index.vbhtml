@Code
    ViewData("Title") = "Measurement List"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/PIMMeasure/PIMMeasureMenu.js?crc=')]);
    </script>
    
End Section

<div id="pagePIMMeasureList" data-role="page">

    <div data-role="header">
        <h1>Measurement List List</h1>
    </div>

    <div data-role="content">

        <form id="frmPIMMeasureBrowser" style="margin: 0 -7px">
            <div>
                <input id="txtPIMMeasureSearch" type="search" placeholder="Search a Type of Measurement" value="" />
            </div>
        </form>

        <div id="divPIMMeasureSearchResults" style="display:none;">

            <div class="ListViewMarginBottom" id="divPIMMeasureRecordCount"></div>

            <ul id="lstPIMMeasure" data-role="listview" data-filter="true">
            </ul>

        </div>

        <div id="divPIMMeasureSearchNoResults" style="display:none;">
	        <div>
                <div data-role="fieldcontain">
                    <label id="lblPIMMeasureNotFound"></label>
                </div>
            </div>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center;"> <a id="btnAddNewPIMMeasure" data-role="button" style="display:none;">Add New Measurement</a> </div>
	        <div style="text-align:center"> <a id="btnPIMMeasureBackToHome" data-role="button">Back To Home</a> </div>
        </div>

    </div>

</div>







