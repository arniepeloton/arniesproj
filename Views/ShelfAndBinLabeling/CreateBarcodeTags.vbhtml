@Code
    ViewData("Title") = "Create Barcode Tags"
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
        require([GetPathFormat('/Scripts/sources/ShelfAndBinLabeling/CreateBarcodeTags.js?crc=')]);
    </script>

    <link rel="Stylesheet" type="text/css" href="/Content/flatui-css-custom.css" />
     <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
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
                <div id="btnMainPage" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    <div><img alt="Save" src="/Content/images/Serial_Number.png" width="25px" align="center" /><br /></div>
                    <div style="font-size:13px;margin-top:10px;">New Bar Code<br />Label List</div>
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
                    <div><img alt="Save" src="/Content/images/RecordLookup.png" width="25px" align="center" /><br /></div>
                    <div style="font-size:13px;margin-top:10px;">Lookup<br />Transaction</div>
                </div>
            </div>
            <div class="ui-block-e" style="width: 20%;">
                <div id="lnkCheckout"style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    <div><img alt="Save" src="/Content/images/save.png" width="25px" align="center" /><br /></div>
                    <div id="lblDownload" style="font-size:13px;margin-top:8px;">Download<br />Barcodes as PDF</div>
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

        <div data-role="popup" id="popupUoMBarcode" class="ui-corner-all" >
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
	        <center><h3>Select Barcode</h3></center>
            <div data-role="content">
                <span id="txtMeasureRequired" style="color:#ff0000; display:none;">Please enter at least one (1) quantity.</span>
                <ul data-role="listview" data-inset="true">
                    <li data-role="list-divider">List of Measurements</li>
                    <li>
                        <div>
                            <ul data-role="listview" id="lstUoM" data-inset="true">
                            </ul>
                        </div>
                    </li>
                </ul>
                <span id="divAddBarcode" style="display:block; ">
                    <button id="btnAddBarcode" data-role="none" class="btn custom-btn-theme-b btn-less-width">
                        Add<span class="lblAddBarcode"></span>
                    </button>    
                </span>
            </div>
        </div>
    </div>

    <div id="popLookup" custom-data-role="popup">
        <div class="popup-md">
            <div class="popup-header">
                List of Barcode Labels
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">
                <table id="dataTableBarcodeList" data-role="table" data-role="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                    <thead>
                        <tr class="custom-theme-f">
                            <th style="cursor:pointer; width:200px;"> Transaction No. </th>
                            <th style="cursor:pointer; width:200px;"> No. of Labels </th>
                            <th style="cursor:pointer; width:200px;"> Date Modified </th>
                        </tr>
                    </thead>
                    <tbody id="tBodyBarcodeList">
                        <tr>
                            <td colspan="3">Loading data ...</td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <div class="popup-footer align-right">
                <button data-role="none" class="btn custom-btn-theme-c btn-less-width popup-close">Close</button>
            </div>
        </div>
    </div>

    <div id="popSearchProduct" custom-data-role="popup">
        <div class="popup-md">
            <div class="popup-header">
                PIM Product
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">
                <table id="dataTableProductList" data-role="table" data-role="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                    <thead>
                        <tr class="custom-theme-f">
                            <th style="cursor:pointer; width:200px;"> # </th>
                            <th style="cursor:pointer; width:200px;"> Product No. </th>
                            <th style="cursor:pointer; width:200px;"> Product Name </th>
                        </tr>
                    </thead>
                    <tbody id="tBodyProductList">
                        <tr>
                            <td colspan="3">Loading data ...</td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <div class="popup-footer align-right">
                <button data-role="none" class="btn custom-btn-theme-c btn-less-width popup-close">Close</button>
            </div>
        </div>
    </div>

    <div data-role="content">
        <div id="addingContent">
            <h1>New Barcode</h1>
            <div style="float:right; margin-bottom: 1em; width:100%;">
                 <table style="float:left;">
                    <tr>
                        <td>
                            <label>
                                <input type="checkbox" id="chkCompanyName">Include Company Name
                            </label>
                        </td>
                        <td>
                            <label>
                                <input type="checkbox" id="chkPricing">Include Pricing
                            </label>
                        </td>
                        <td>
                            <a id="btnGenPriceLevel" href="#">Click here to choose price level</a>
                        </td>
                    </tr>
                </table>
                <span id="divAddLabel" style="display:block; float:right">
                    <button id="btnAddLabel" data-role="none" class="btn custom-btn-theme-b btn-less-width">
                        Add Bar Code Label<span class="lblAddLabelBarcode"></span>
                    </button>    
                </span>
            </div>
            <table data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-shadow ui-responsive table-stroke table-stripe">
                <thead>
                    <tr id="tblHeader" class="custom-theme-f txt-center">
                        <th>Product Code/No.</th>
                        <th>SKU Description</th>
                        <th>Unit of<br />Measure</th>
                        <th>Price<br />Level</th>
                        <th>UPC/Barcode</th>
                        <th>No. of Labels<br />to Print</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody id="tblBody">
                
                </tbody>                        
            </table>
        </div>
        <div id="lookupContent" style="margin-top:1em; display:none">
            <h1>Transaction No.: <span id="txtTransNo"></span></h1>
            <table style="float:left;">
                    <tr>
                        <td>
                            <label>
                                <input type="checkbox" id="chkCompanyNameLookUp">Include Company Name
                            </label>
                        </td>
                        <td>
                            <label>
                                <input type="checkbox" id="chkPricingLookUp">Include Pricing
                            </label>
                        </td>
                        <td>
                            <a id="btnGenPriceLevelLookUp" href="#">Click here to choose price level</a>
                        </td>
                    </tr>
                </table>
            <span id="divAddLabel2" style="display:block; float:right">
                <button id="btnAddLabelLookUp" data-role="none" class="btn custom-btn-theme-b btn-less-width">
                    Add Bar Code Label<span class="lblAddLabelBarcode"></span>
                </button>
            </span>
            <table data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-shadow ui-responsive table-stroke table-stripe">
                <thead>
                    <tr id="tblHeaderLookup" class="custom-theme-f txt-center">
                        <th>Product Code/No.</th>
                        <th>SKU Description</th>
                        <th>Unit of<br />Measure</th>
                        <th>Price<br />Level</th>
                        <th>UPC/Barcode</th>
                        <th>No. of Labels<br />to Print</th>
                        <th>Reprint</th>
                    </tr>
                </thead>
                <tbody id="tblBodyLookup">
                
                </tbody>                        
            </table>
        </div>
    </div>  
</div>