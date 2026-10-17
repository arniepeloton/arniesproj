@Code
    ViewData("Title") = "UNSPSC Commodity Feature List"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/PIMUNSPSCCommodityFeature/PIMUNSPSCCommodityFeatureMenu.js?crc=')]);
    </script>
    
End Section

<div id="pagePIMUNSPSCCommodityFeatureList" data-role="page">

    <div data-role="header">
        <h1>UNSPSC Commodity Feature List</h1>
    </div>

    <div data-role="content">

        <form id="frmPIMUNSPSCCommodityFeatureBrowser" style="margin: 0 -7px">
            <div>
                <input id="txtPIMUNSPSCCommodityFeatureSearch" type="search" placeholder="Search a UNSPSC Commodity Feature" value="" />
            </div>
        </form>

        <div id="divPIMUNSPSCCommodityFeatureSearchResults" style="display:none;">

            <div class="ListViewMarginBottom" id="divPIMUNSPSCCommodityFeatureRecordCount"></div>

            <ul id="lstPIMUNSPSCCommodityFeature" data-role="listview" data-filter="true">
            </ul>

        </div>

        <div id="divPIMUNSPSCCommodityFeatureSearchNoResults" style="display:none;">
	        <div>
                <div data-role="fieldcontain">
                    <label id="lblPIMUNSPSCCommodityFeatureNotFound"></label>
                </div>
            </div>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center;"> <a id="btnAddNewPIMUNSPSCCommodityFeature" data-role="button" style="display:none;">Add UNSPSC Commodity Feature</a> </div>
	        <div style="text-align:center"> <a id="btnPIMUNSPSCCommodityFeatureBackToHome" data-role="button">Back To Home</a> </div>
        </div>

    </div>

</div>







