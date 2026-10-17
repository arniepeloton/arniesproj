@Code
    ViewData("Title") = "Assign Products to Bin"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head 
    <style>
        .ui-icon-barcode {
	        background-image: url("../Content/images/barcode.ico");
	        background-repeat: no-repeat;	        
	            background-position: center center;
	            background-size: 50px 50px;
        }
    </style>
    <script>
        document.addEventListener('deviceready', this.onDeviceReady, false);
        require([GetPathFormat('/Scripts/sources/ShelfAndBinLabeling/CreateShelfPriceTags.js?crc=')]);
    </script>

    <link rel="Stylesheet" type="text/css" href="/Content/flatui-css-custom.css" />
     
End Section

<div id="pageMain" data-role="page">
     <div data-role="header" data-position="fixed1" style="background: none white; border: none">
        <div class="ui-grid-d" >
            <div class="ui-block-a" style="width: 20%;">
                <div id="btnHome" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    <div><img alt="home" src="/Content/images/home.png" width="30px" align="center" /><br /></div>
                    <div style="font-size:13px; margin-top:10px;">Home</div>
                </div>
            </div>
            <div class="ui-block-b" style="width: 20%;">
                <div id="divBlockCustomer">
                     <div id="btnCustomer" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                        <span id="lblLabel" style="font-size:13px;"><br /></span>
                        <span id="lblCustomerName" style="font-size:13px;"></span>
                    </div>
                </div>
            </div>
            <div class="ui-block-c" style="width: 20%;">
                <div id="btnPOSMode" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    <div id="IsPOSHeader" style="font-size:13px;"><br /></div>
                    <div id="lblCurrentPOSMode" style="font-size:13px;margin-top:6px;"></div>
                </div>
            </div>
            <div class="ui-block-d" style="width: 20%;">
                <div id="btnMoreOptions" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    @*<div><img alt="More Options" src="/Content/images/More_Options.png" width="30px" align="center" /><br /></div>
                    <div style="font-size:13px;margin-top:10px;">Map Product <br />to Bin</div>*@
                </div>
            </div>
            <div class="ui-block-e" style="width: 20%;">
                <div id="lnkCheckout"style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    <div><img alt="Save" src="/Content/images/save.png" width="25px" align="center" /><br /></div>
                    <div id="lblDownload" style="font-size:13px;margin-top:8px;">Download<br />Tags as PDF</div>
                </div>
            </div>
        </div>
        <div>
            <div style="margin:0px;" data-role="button" data-theme="b" data-corners="false">
                <div class="lblPosBranch" style="float:left; text-overflow:ellipsis; overflow:hidden; width:40%; text-align:left; white-space:nowrap;">
                </div>
                <div style="float:right">
                    <span id="clock"></span>
                </div>
                <div style="clear:both"></div>
            </div>
        </div>
   </div>

    <div data-role="content">
        
        <div data-role="fieldcontain">
            <label for="selFOWST">FOWST</label>
            <select id="selFOWST"></select>
        </div>

        <div data-role="fieldcontain">
            <label for="selBINLocation">Shelf BIN Location:</label>
            <div class="custom-ui-button">
                <div class="ui-grid-a">
                    <div class="ui-block-a custom-ui-select" style="width:80%; padding-top:10px;">
                        <select id="selBINLocation" disabled>
                        </select>
                    </div>
                    <div class="ui-block-b" style="width:20%">
                        <a id="btnScanBIN" data-role="button" data-inline="true" class="ui-icon-barcode">&nbsp;</a>
                    </div>
                </div>
            </div>
        </div>

        <div data-role="fieldcontain" id="productDiv">
            <label for="btnSearchProduct">Identify Product in Shelf or BIN:</label>
            <div class="custom-ui-button">
                <div class="ui-grid-a">
                    <div class="ui-block-a" style="width:80%">
                        <input data-role="none" type="text" id="txtSearchProductID" style="display:none;" />
                        <button id="btnSearchProduct">Search Product</button>
                    </div>
                    <div class="ui-block-b" style="width:20%">
                        <a id="btnScanProduct" data-role="button" data-inline="true" class="ui-icon-barcode">&nbsp;</a>
                    </div> 
                </div>                           
            </div>
        </div>

        <div>
            <ul data-role="listview" data-inset="true">
                <li data-role="list-divider">List of Products in Shelf / BIN</li>
                <li>
                    <div>
                        <ul data-role="listview" id="lstProductsInBIN" data-inset="true">
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        @*<table data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-shadow ui-responsive table-stroke table-stripe">
            <thead>
                <tr id="tblHeader" class="custom-theme-f txt-center">
                    <th>Bin Address</th>
                    <th>SKU Description</th>
                </tr>
            </thead>
            <tbody id="tblBody">
                <tr>
                    <td>0.9.1.7</td>
                    <td>Globe</td>
                </tr>
                <tr>
                    <td>0.9.4.7</td>
                    <td>Smart</td>
                </tr>
            </tbody>                        
        </table>*@

        <br />
        <br />
        <div style="text-align: center">
            <a id="btnSaveAndMap" data-role="button"><h3>Save and map another shelf or bin</h3></a>

            <a id="btnSaveAndBackToMain" data-role="button"><h3>Save and go back to main page</h3></a>

            <a id="btnCancel" data-role="button"><h3>Cancel</h3></a>
        </div>
        
    </div>  
</div>