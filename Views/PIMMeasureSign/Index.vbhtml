@Code
    ViewData("Title") = "Measure Sign"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/PIMMeasureSign/PIMMeasureSignMenu.js?crc=')]);
    </script>
    
End Section

<div id="pagePIMMeasureSignList" data-role="page">

    <div data-role="header">
        <h1>Sign List</h1>
    </div>

    <div data-role="content">

        <form id="frmPIMMeasureSignBrowser" style="margin: 0 -7px">
            <div>
                <input id="txtPIMMeasureSignSearch" type="search" placeholder="Search a Sign" value="" />
            </div>
        </form>

        <div id="divPIMMeasureSignSearchResults" style="display:none;">

            <div class="ListViewMarginBottom" id="divPIMMeasureSignRecordCount"></div>

            <ul id="lstPIMMeasureSign" data-role="listview" data-filter="true">
            </ul>

        </div>

        <div id="divPIMMeasureSignSearchNoResults" style="display:none;">
	        <div>
                <div data-role="fieldcontain">
                    <label id="lblPIMMeasureSignNotFound"></label>
                </div>
            </div>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center;"> <a id="btnAddNewPIMMeasureSign" data-role="button" style="display:none;">Add New Sign</a> </div>
	        <div style="text-align:center"> <a id="btnPIMMeasureSignBackToHome" data-role="button">Back To Home</a> </div>
        </div>

    </div>

</div>







