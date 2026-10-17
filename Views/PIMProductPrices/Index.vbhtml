@Code
    ViewData("Title") = "Product Price Adjustment"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/PIMProductPrice/PIMProductPriceMenu.js?crc=')]);
    </script>
    
End Section

<div id="pagePIMProductPriceList" data-role="page">

    <div data-role="header">
        <h1>Product Price List</h1>
    </div>

    <div data-role="content">

        <form id="frmPIMProductPriceBrowser" style="margin: 0 -7px">
            <div>
                <input id="txtPIMProductPriceSearch" type="search" placeholder="Search a Product" value="" />
            </div>
        </form>

        <div id="divPIMProductPriceSearchResults" style="display:none;">

            <div class="ListViewMarginBottom" id="divPIMProductPriceRecordCount"></div>

            <ul id="lstPIMProductPrice" data-role="listview" data-filter="true">
            </ul>

        </div>

        <div id="divPIMProductPriceSearchNoResults" style="display:none;">
	        <div>
                <div data-role="fieldcontain">
                    <label id="lblPIMProductPriceNotFound"></label>
                </div>
            </div>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPIMProductPriceBackToHome" data-role="button">Back To Home</a> </div>
        </div>

    </div>

</div>







