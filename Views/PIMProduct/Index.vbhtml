@Code
    ViewData("Title") = "Product List"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/PIMProduct/PIMProductMenu.js?crc=')]);
    </script>
    
End Section

<div id="pagePIMProductList" data-role="page">

    <div data-role="header">
        <h1>Product List</h1>
    </div>

    <div data-role="content">

        <form id="frmPIMProductBrowser" style="margin: 0 -7px">
            <div>
                <input id="txtPIMProductSearch" type="search" placeholder="Search a Product" value="" />
            </div>
        </form>

        <div id="divPIMProductSearchResults" style="display:none;">

            <div class="ListViewMarginBottom" id="divPIMProductRecordCount"></div>

            <ul id="lstPIMProduct" data-role="listview" data-filter="true">
            </ul>

        </div>

        <div id="divPIMProductSearchNoResults" style="display:none;">
	        <div>
                <div data-role="fieldcontain">
                    <label id="lblPIMProductNotFound"></label>
                </div>
            </div>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center;"> <a id="btnAddNewPIMProduct" data-role="button" style="display:none;">Add New Prodct</a> </div>
	        <div style="text-align:center"> <a id="btnPIMProductBackToHome" data-role="button">Back To Home</a> </div>
        </div>

    </div>

</div>







