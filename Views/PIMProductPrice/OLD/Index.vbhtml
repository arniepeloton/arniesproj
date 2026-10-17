@Code
    ViewData("Title") = "Product Price Adjustment"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/PIMProductPrice/PIMProductPriceMenu.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryPIMPriceLevel.js?crc=')
        ]);
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

<div id="pagePIMProductPriceInfo" data-role="page">

    <div data-role="header">
        <h1>Product Price Info</h1>
    </div>

    <div data-role="content" >

        <div style="display: none;"><input type="text" readonly id="txtPIMProductID" /></div>
        <div data-role="fieldcontain">
            <label for="txtPIMProduct">Product :</label>
            <input type="text" readonly id="txtPIMProduct" />
        </div>

        <div style="display: none;"><input type="text" readonly id="txtPIMPriceLevelID" /></div>
        <div data-role="fieldcontain">
            <label for="btnPIMPriceLevel">Price Level: <span style="color: #FF0000;">*</span></label>
            <fieldset data-role="controlgroup">
                <button id="btnPIMPriceLevel" data-role="button">Please Select</button>
            </fieldset>
        </div>

        <div>
            <ul data-role="listview" id="lstPIMUNSPSCCommFeature" data-inset="true">   
				<li data-role="list-divider">Product Size(s)</li>
                <li>
                    <div id="divProductPrice">
                    <div>
                </li>
            </ul>

        <div>

        <br />
        <br />
	    <div style="text-align:center"> <a id="btnPIMProductPriceSave" data-role="button">Save Prices</a> </div>
	    <div style="text-align:center"> <a id="btnPIMProductPriceCancel" data-role="button">Cancel</a> </div>
        </div>

    </div>

</div>






