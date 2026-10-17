@Code
    ViewData("Title") = "Welcome"
    Layout = "~/Views/Shared/Plain.vbhtml"
End Code
<style>
    /*Green button*/
    .ui-btn-up-green {
        border: 1px solid #3A7214;
        background: #5BAB25;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, #7EC54E, #41AB25);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #7EC54E),
            color-stop(1, #41AB25));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#7EC54E', EndColorStr='#41AB25')";
    }
    .ui-btn-up-green a.ui-link-inherit {
        color: #fff;
    }
    .ui-btn-hover-green {
        border: 1px solid #176E00;
        background: #6EB64B;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, 
            #99D472, 
            #6EB64B);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #99D472),
            color-stop(1, #6EB64B));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#99D472', EndColorStr='#6EB64B')";
    }
    .ui-btn-hover-green a.ui-link-inherit {
        color: #fff;
    }
    .ui-btn-down-green {
        border: 1px solid #347722;
        background: #7EC54E;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, #569E39, #7EC54E);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #569E39),
            color-stop(1, #7EC54E));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#569E39', EndColorStr='#7EC54E')";
    }
    .ui-btn-down-green a.ui-link-inherit {
        color: #fff;
    }
    .ui-btn-up-green,
    .ui-btn-hover-green,
    .ui-btn-down-green {
        font-family: Helvetica, Arial, sans-serif;
        text-decoration: none;
    }
    
    /*Gray button*/
    .ui-btn-up-gray {
        border: 1px solid #595A57;
        background: #8F908F;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, #AAABAA, #8D8E8D);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #AAABAA),
            color-stop(1, #8D8E8D));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#AAABAA', EndColorStr='#8D8E8D')";
    }
    .ui-btn-up-gray a.ui-link-inherit {
        color: #fff;
    }
    .ui-btn-hover-gray {
        border: 1px solid #646464;
        background: #A1A0A0;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, 
            #B5B8B5, 
            #A1A0A0);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #B5B8B5),
            color-stop(1, #A1A0A0));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#B5B8B5', EndColorStr='#A1A0A0')";
    }
    .ui-btn-hover-gray a.ui-link-inherit {
        color: #fff;
    }
    .ui-btn-down-gray {
        border: 1px solid #777A77;
        background: #AAABAA;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, #999B99, #AAABAA);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #999B99),
            color-stop(1, #AAABAA));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#999B99', EndColorStr='#AAABAA')";
    }
    .ui-btn-down-gray a.ui-link-inherit {
        color: #fff;
    }
    .ui-btn-up-gray,
    .ui-btn-hover-gray,
    .ui-btn-down-gry {
        font-family: Helvetica, Arial, sans-serif;
        text-decoration: none;
    }
    
    /* Button */
    .HotButtons .ui-btn-inner {
        
        padding-top: 1.4em;
    }
    
    
    .HotButtonsText .ui-btn-text {
        color:White;   
    }
    

    #spanName
        {
            font-weight: bold;
            font-style: italic;
        }
        .ScrollWrapper
        {
            overflow: hidden;
            z-index: 1;
            position: relative;
            height: 240px;
            -webkit-border-radius: 10px;
            -moz-border-radius: 10px;
            -o-border-radius: 10px;
            border-radius: 10px;
            @*background: #e3e3e3;*@
        }
        .ScrollWrapper > div
        {
            width: 1650px;
            height: 100%;
            padding: 0;
            position: absolute;
            z-index: 1;
        }
        .ScrollWrapper > div > div
        {
            float: left;
            margin: 5px 0px 5px 5px;
            width: 150px;
            text-align: center;
        }
        .ScrollWrapper > div > div:last-child
        {
            margin: 5px;
        }
        .ScrollWrapper img
        {
            width: 144px;
            height: 144px;
        }
        .ScrollWrapper > div > div h3
        {
            margin: 0px;
        }
        .SectionHeader h2 {
            margin-top: 1px;
            margin-bottom: 1px;
            color: #bbbbbb;
        }
        .SectionHeader > div:nth-child(2) {
            text-align: right;
        }
        .SectionHeader a {
            text-decoration: none;
        }
        .Empty {
            padding: 10px;
            text-align: center;
        }

        .content_div {
            display: none;
            background-color:white;
            padding-top:10px;
        }
        .content_div:first-child {
            display: block;
        }

        table.myTable { 
            border-collapse:collapse; 
        }
        .ui-table-reflow.ui-responsive {
            display: table;
        }
        table.myTable th {
	        border-width: 1px;
	        padding: 8px;
	        border-style: solid;
	        border-color: #666666;
	        background-color: #dedede;
        }
        table.myTable td {
	        border-width: 1px;
	        padding: 8px;
	        border-style: solid;
	        border-color: #666666;
	        background-color: #ffffff;
        }

        .tableUnits td
    {
        padding-right: 10px;
        text-align: right;
    }


    .SearchText {
        margin:0px;
        border: 0px;
        text-shadow: 0px;
        background: #FFF;
        height: 40px;
        border-radius: 0px;
        
    }
    .ui-focus {
     -moz-box-shadow: none !important;
     -webkit-box-shadow: none !important;
     box-shadow: none !important;
    }
    .ui-shadow-inset {
        box-shadow:none !important;
    }
     .ui-page-active{
      padding-top:0px;
     }
     .detailimage {
        width: 80%;
        text-align: center;
        margin: 0;
        padding-top:0px;
    }

    .detailimage img {
        width: 100%;
    }
    .detailimage2 {

        text-align: center;
        margin: 0;

    }

    .detailimage2 img {
        width: 100%;
    }

    .detailimage3 img {
        width: 99%;
    }

    .hide1st {
        visibility: collapse
    }
</style>
    <link type="text/css" href="/Content/bootstrap-custom.css"  rel="Stylesheet" />
    <link type="text/css" href="/Content/flatui-css-custom.css" rel="Stylesheet" />
    <link rel="stylesheet" href="/Content/metro-calendar.css" />
    <link rel="stylesheet" href="/Content/Ecommerce/css/lightbox.css">
    <script>
        require([GetPathFormat('/Scripts3/SalesMenu.js?crc=')]);
        require([GetPathFormat('/Scripts3/ItemModifiers.js?crc=')]);
        //        require([GetPathFormat('/Scripts3/others/JsToPDF/jquery-ui-1.8.17.custom.min.js?crc=')]);        
        //        require([GetPathFormat('/Scripts3/others/JsToPDF/jspdf.js?crc=')]);        
        //    require([GetPathFormat('/Scripts3/others/JsToPDF/html2canvas.js?crc=')]);
        //    require([GetPathFormat('/Scripts3/others/JsToPDF/jspdf.min.js?crc=')]);
        //    require([GetPathFormat('/Scripts3/others/JsToPDF/adler32cs.js?crc=')]);
        //    require([GetPathFormat('/Scripts3/others/JsToPDF/FileSaver.js?crc=')]),
        //    require([GetPathFormat('/Scripts3/others/JsToPDF/jspdf.plugin.addimage.js?crc=')]);
        //    require([GetPathFormat('/Scripts3/others/JsToPDF/jspdf.plugin.standard_fonts_metrics.js?crc=')]);
        //    require([GetPathFormat('/Scripts3/others/JsToPDF/jspdf.plugin.split_text_to_size.js?crc=')]);
        //    require([GetPathFormat('/Scripts3/others/JsToPDF/jspdf.plugin.from_html.js?crc=')]);
        //    require([GetPathFormat('/Scripts3/others/JsToPDF/basic.js?crc=')]);
        //    require([GetPathFormat('/Scripts3/others/JsToPDF/jspdf.debug.js?crc=')
        //        ]);

        function skupicerror(dis) {
            $(dis).attr('isimgerror', 1).attr('src', '../Content/images/box.png');
            Android.AddLog('Can\'t load image no. ' + $(dis).data('data').Ctr + ' for SKU: ' + $(dis).data('data').ProductName);

            if (CInt2($(dis).attr('isimgviewer')) == 1) {
                $('#lightboxOverlay, #lightbox').hide();
            }            
        }
    </script>

      <div data-role="header" data-position="fixed1" class="hide1st" style="background: none white; border: none">
        <div>
            @*<div class="ui-grid-d" >
                <div class="ui-block-a" style="width: 12%;">
                    <div id="btnHome2" style="margin:0px; height: 80px;line-height:80px;" data-role="none" data-theme="b" data-corners="false" class="HotButtons btn custom-btn-theme-a">
                        <span style="margin-top:5px;">Home</span>
                    </div>
                </div>
                <div class="ui-block-b" style="width: 22%;">
                    <div id="divBlockCustomer2">
                         <div id="btnCustomer2" style="margin:0px; height: 80px;" class="btn custom-btn-theme-a" data-role="none" data-theme="b" data-corners="false">
                            <span id="lblLabel2" style="font-size:13px;"><img alt="Customer" src="/Content/images/Customer.png" width="30px" align="center" /><br /></span>
                            <span id="lblCustomerName2" style="font-size:13px;"></span>
                        </div>
                    </div>
                   <div style="display: none;" id="divBlockSupplier2">
                        <div id="btnSupplier2" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                            <span>Supplier:<br /></span>
                            <span id="lblSupplierName2"></span>
                        </div>
                   </div>
                </div>
                <div class="ui-block-c" style="width: 22%;">
                    <div id="btnPOSMode2" style="margin:0px; height: 80px;" class="btn custom-btn-theme-a" data-role="none" data-theme="b" data-corners="false">
                        <div id="IsPOSHeader2"><img alt="POS Mode" src="/Content/images/View_Mode.png" width="30px" align="center" /><br /></div>
                        <div id="lblCurrentPOSMode2" style="font-size:13px;margin-top:10px;"></div>
                    </div>
                </div>
                <div class="ui-block-d" style="width: 22%;line-height:80px;">
                    <div id="btnMoreOptions2" style="margin:0px; height: 80px;" class="btn custom-btn-theme-a" data-role="none" data-theme="b" data-corners="false">
                        <div><img alt="More Options" src="/Content/images/More_Options.png" width="30px" align="center" /><br /></div>
                        <div style="font-size:13px;margin-top:10px;">More <br /> Options</div>
                    </div>
                </div>
                <div class="ui-block-e" style="width: 22%;">
                    <div id="lnkCheckout2"style="margin:0px; height: 80px;" class="btn custom-btn-theme-b" data-role="none" data-theme="green" data-corners="false">
                        <div><img alt="More Options" src="/Content/images/Pay.png" width="30px" align="center" /><br /></div>
                        <div id="lblPayNow2" style="font-size:13px;margin-top:10px;">Pay Now</div>
                    </div>
                </div>
            </div>
            <div>
                <div style="margin:0px;" data-role="none" class="btn custom-btn-theme-f" data-corners="false">
                    <span id="clock2">&nbsp;</span>
                </div>
            </div>*@
        </div>
        <div class="ui-grid-d" >
            <div class="ui-block-a" style="width: 12%;">
                <div id="btnHome" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    <div><img alt="home" src="/Content/images/home.png" width="30px" align="center" /><br /></div>
                    <div style="font-size:13px; margin-top:10px;">Home</div>
                </div>
            </div>
            <div class="ui-block-b" style="width: 22%;">
                <div id="divBlockCustomer">
                     <div id="btnCustomer" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                        <span id="lblLabel" style="font-size:13px;"><img alt="Customer" src="/Content/images/Customer.png" width="30px" align="center" /><br /></span>
                        <span id="lblCustomerName" style="font-size:13px;"></span>
                    </div>
                </div>
               <div style="display: none;" id="divBlockSupplier">
                    <div id="btnSupplier" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                        <span style="font-size:13px;"><img alt="Customer" src="/Content/images/supplier.png" width="30px" align="center" /><br /></span>
                        <span id="lblSupplierName" style="font-size:13px;margin-top:10px;"></span>
                    </div>
               </div>
            </div>
            <div class="ui-block-c" style="width: 22%;">
                <div id="btnPOSMode" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    <div id="IsPOSHeader" style="font-size:13px;"><img alt="POS Mode" src="/Content/images/View_Mode.png" width="30px" align="center" /><br /></div>
                    <div id="lblCurrentPOSMode" style="font-size:13px;margin-top:6px;"></div>
                </div>
            </div>
            <div class="ui-block-d" style="width: 22%;">
                <div id="btnMoreOptions" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    <div><img alt="More Options" src="/Content/images/More_Options.png" width="30px" align="center" /><br /></div>
                    <div style="font-size:13px;margin-top:10px;">More Options</div>
                </div>
            </div>
            <div class="ui-block-e" style="width: 22%;">
                <div id="lnkCheckout"style="margin:0px; height: 80px;" data-role="button" data-theme="green" data-corners="false">
                    <div id="iconPay" style="display:block;"><img alt="Pay" src="/Content/images/Pay.png" width="30px" align="center" /><br /></div>
                    <div id="iconSave" style="display:none;"><img alt="Save" src="/Content/images/save.png" width="25px" align="center" /><br /></div>
                    <div id="lblPayNow" style="font-size:13px;margin-top:9px;">Pay Now</div>
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

        <div style="background-color: transparent; border-style: none; box-shadow:none;" data-role="popup" data-dismissible="false" id="popupLoad">
            <img src='../Scripts3/others/themes/default/images/ajax-loader.gif' style="opacity:0.2;" />
        </div>
         <div data-role="popup" id="popupPayNowMode" class="ui-corner-all" >
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
			<center><h3>Billing Option</h3></center>
            <div data-role="content">
                <a id="lnkProceedToPayments" data-role="button" data-theme="e"> <h3>Proceed to Payments</h3> </a>
                <a id="lnkBackToTableView" data-role="button" data-theme="e"> <h3>Go Back to Table View</h3> </a>
                 <a id="lnkPrintBill" data-role="button" data-theme="e"> <h3>Billing</h3> </a>
           </div>
       </div>

        <div data-role="popup" id="popupPOSMode" data-theme="b" class="ui-corner-all">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
			<center><h3 id="IsPOS">POS Mode</h3></center>
            <div data-role="content">
                <div id="divPOSMode">
                </div>
           </div>
       </div>

        <div data-role="popup" id="popupMoreOptions" data-theme="b" class="ui-corner-all" style="width:auto;">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
			<center><h3>More Options</h3></center>
            <div data-role="content">
                
                <div style="text-align: center">
                    <a id="btnFowst" data-role="button" data-theme="e">
                    <h3>Current FOWST Location</h3>
                    <p style="font-weight: normal; margin-top: -15px;"><label id="lnkPosBranch" style="cursor:pointer;"></label> 
                    <br />
                    Sales, inventory movements and payments will be tagged to this location.
                    </p>
                    </a>
                </div>
                <div style="text-align: center">
                    <a id="butNewSalesOrder" data-role="button" data-theme="e">
                    <h3>New Cart</h3>
                    <p style="font-weight: normal; margin-top: -15px;">
                    Puts the current cart on hold and displays a new one for the next customer.
                    </p>
                    </a>
                </div>
                <div style="text-align: center">
                    <a id="lnkSalesOrder" data-role="button" data-theme="e">
                    <h3>Recall Cart</h3>
                     <p style="font-weight: normal; margin-top: -15px;">
                     Used to retrieve carts that were placed on hold.
                    </p>
                    </a>
                </div>
                <div style="text-align: center">
                    <a id="butRecentlyBought" data-role="button" data-theme="e">
                    <h3>Recently Bought</h3>
                    <p style="font-weight: normal; margin-top: -15px;">
                    View recently bought items of the selected customer. <br /> 
                    Useful for regular customers who buy similar items frequently.
                    </p>
                    </a>
                </div>
                <div style="text-align: center">
                    <a id="butReprintReceipt" data-role="button" data-theme="e">
                    <h3>Reprint Receipt</h3>
                    <p style="font-weight: normal; margin-top: -15px;">
                    Allows you to reprint the receipt of your last transaction<br /> in case it did not print upon payment. <br /> 
                    </p>
                    </a>
                </div>
                <div id="salesRepOption" style="text-align: center">
                    <a id="butSalesRepresentatives" data-role="button" data-theme="e">
                    <h3>Assign Sales Representatives</h3>
                    <p style="font-weight: normal; margin-top: -15px;">
                    Assign sales representatives for this transaction
                    </p>
                    </a>
                </div>
                <div id="divTestPriceRequest" style="text-align: center">
                    <a id="butTestPriceRequest" data-role="button" data-theme="e">
                        <h3>Test Price Request</h3>
                        <p style="font-weight: normal; margin-top: -15px;">
                            Test your requested prices for this this transaction
                        </p>
                    </a>
                </div>
                
            </div>
        
		</div>   
        
<h3 id="hRsd" style="display: none; margin-top: 0">
    Reconstruct Sales Data</h3>

<div data-role="content" class="hide1st" style="padding:5px;">
    <div class="ui-grid-c" style="margin:0px; padding: 0px;">
        <div class="ui-block-a" style="width:50%;padding: 0px;margin-bottom:5px;">
            <form id="formSearch">
                <input id="txtSearch" type="search" placeholder="Search products" class="SearchText" style="height: 40px;padding:0px;" autofocus />

            </form>

        </div>
        <div id="divHideBarcodeScan" class="ui-block-b" style="width: 15%;padding: 0px;margin:3px;">
            <center>
                <div>
                    <img src="/Content/images/barcode96x96.png" style="height: 50px;" class="detailimage2" id="butBarCode" data-role="button" data-corners="false" />
                </div>
            </center>
        </div>
        <div id="divSearchMulti" class="ui-block-c" style="width: 15%;padding: 0px;margin:3px;">
            <center>
                <div>
                    <img src="/Content/images/worksheet.png" style="height: 50px;" class="detailimage2" data-role="button" data-corners="false" />
                </div>
            </center>
        </div>
        <div id="divHideHotButtonsButton" class="ui-block-d" style="width: 15%;padding: 0px;margin:3px;">
            <center>
                <div>
                    @*<a href="#" id="butHotButtonsShortcut">Hot Buttons</a>*@


                    <img src="/Content/images/hotb.png" class="detailimage2" style="height: 50px;" id="butHotButtonsShortcut" data-role="button" data-corners="false" />
                </div>
            </center>
        </div>
    </div>
    <a href="http://localhost:8080/SalesMenu/BsIndex?mode=pos&OrgID=cfe39df6-8809-4973-9515-810df75aed12&IsMobile=1&theme=bs">New UI</a>
</div>

<div data-role="content" id="divRecentlyBoughtPanel" style="display:none;">
<div class="ui-grid-a" style="margin:0px; padding: 0px;">
    <div class="ui-block-a">
        <div style="font-size: xx-large">
            Recently Bought
        </div>
        By <label id="lblCustomerNameRecent"></label>
    </div>
    <div class="ui-block-b" style="text-align:right;">
    <a id="lnkRecentlyBought" href="#pageRecentMenu" style="text-decoration: none;">Within the last 2 weeks &#x25BC;</a>
    </div>
</div>
<br />
Click on any item to quickly add to <label id="lblCustomerNameRecent2"></label>’s cart<br /><br />
<button id="butRecentlyBoughtAddAll" style="display:none">Add all recently bought items to cart</button>
    <div id="divRecentlyBoughtSection">
        @*  <div class="ListViewMarginBottom" name="divRecordCount">
        </div>
        <ul name="ulBrowser" data-role="listview" data-filter="true">
        </ul>
        <div class="ListViewMarginTop" name="divRecordCount">
        </div>*@
    </div>
    <div id="divRecentlyBoughtEmpty" class="Empty" style="display:none;">
        No recently bought item
    </div>

</div>
<div id="Nav1b" class="content_div">
    Recently Viewed Content</div>
<div id="Nav1c" class="content_div">
    Specials</div>

<div data-role="content" class="hide1st" id="divCartPanel">
    <div id="divCartSection" style="display: none">
        @*<div class="ui-grid-a ui-responsive">
                <div class="ui-block-a">
                    No. of SKU: <span id="spanSkuCount"></span>
                </div>
                <div class="ui-block-b" style="text-align:right; font-size:xx-large">
                    Cart Total: <span id="spanCartTotal">
                </div>
            </div>*@
        <div style="float: left;">
            <div style="font-size: xx-large">
                Cart
                <br />
            </div>
        </div>

        <div style="text-align: right">
            @*<div>
                    No. of SKU: <span id="spanSkuCount"></span>
                </div>*@
            <div>
                Cart Total:
            </div>
            <div style="font-size: xx-large">
                <span id="spanCartTotal"></span>
            </div>
        </div>
        <div id="divCartStock" style="display: none">
            <h3 name="spanStockDescription" style="white-space: normal"></h3>
            @*<h2 name="spanModifiers"></h2>*@
            <table class="tableUnits">
                <tr name="trTemplate">
                    <td name="tdQty"></td>
                    <td>@@</td>
                    <td name="tdPrice"></td>
                    <td>/</td>
                    <td name="tdUnit"></td>
                    <td name="tdLineTotal"></td>
                </tr>
            </table>
            <div class="divCartItemModifier"></div>
            <div>
                Item total: <span name="spanTotal"></span>
            </div>
        </div>

        <div data-role="fieldcontain" style="display:none">
            <label for="txtRemarks">
                Remarks:
            </label>
            <input id="txtRemarks" maxlength="999" />
            <button id="butSaveRemarks">Save Remarks</button>
        </div>
        <div id="divReconstructSalesData" style="display: none">
            <div id="divDiv" data-role="fieldcontain">
                <fieldset id="txtDate" data-role="DateCombo" data-timecombo="txtTime">
                    <legend>Date of Original Sales Transaction:</legend>
                </fieldset>
            </div>
            <div id="divDiv" data-role="fieldcontain">
                <fieldset id="txtTime" data-role="TimeCombo" data-seconds="true">
                    <legend>Time of Original Sales Transaction:</legend>
                </fieldset>
            </div>
            <div data-role="fieldcontain">
                <label for="txtRefNum">
                    Reference (Invoice) Number of Original Sales Transaction:
                </label>
                <input id="txtRefNum" maxlength="100" />
            </div>
            <div data-role="fieldcontain">
                <label for="cbReasonForReconstructingData">
                    Reason for Reconstructing Data:
                </label>
                <select id="cbReasonForReconstructingData"></select>
            </div>
        </div>
        <div style="height: 10px">
        </div>
        @*        <button id="lnkCheckout" data-theme="e">
                <h3>
                    Go To Checkout</h3>
            </button>*@
        <a id="lnkHold" href="#" data-role="button">
            <h3></h3>
        </a>
    </div>
    <div id="divCartEmpty" class="Empty">
        No items in cart
        <br />
    </div>
    @*    <a id="lnkSalesOrder" style="text-decoration: none;" href="#" data-role="button">
        <div name="divNumber">
            &#x25BC;</div>
        <div name="divDate">
        </div>
        </a>
        <button id="butNewSalesOrder">New Cart</button>*@
</div>


<div style="clear: both">
</div>
@Section OtherPages
    <div id="pageUserMenu" data-role="page">
        <div data-role="header">
            <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
            <h1>
                Menu</h1>
        </div>
        <div data-role="content">
            <ul data-role="listview">
                <li><a href="/SalesReturns">Stock Returns</a></li>
                <li><a href="#">Settings</a></li>
                <li><a href="#">Account</a></li>
                <li><a id="lnkLogout" href="/login3?mode=logout">Logout</a></li>
            </ul>
        </div>
    </div>
    <div id="pageRecentMenu" data-role="page">
        <div data-role="header">
            <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
            <h1>
                Menu</h1>
        </div>
        <div data-role="content">
            <ul data-role="listview">
                <li><a href="#">Within last week</a></li>
                <li><a href="#">Within the last 2 weeks</a></li>
                <li><a href="#">Within the last 4 weeks</a></li>
                <li><a href="#">Within the last 3 months</a></li>
                <li><a href="#">Within the last 6 months</a></li>
                <li><a href="#">Within last year</a></li>
                <li><a href="#">Week to date</a></li>
                <li><a href="#">Month to date</a></li>
                <li><a href="#">Quarter to date</a></li>
                <li><a href="#">Year to date</a></li>
                <li><a href="#">Custom</a></li>
            </ul>
        </div>
    </div>
    <div id="pageMenu" data-role="page">
        <div data-role="header">
            <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
            <h1>
                Menu</h1>
        </div>
        <div data-role="content">
            <ul data-role="listview">
                <li data-role="divider">Menu</li>
                <li><a href="#">Enter Customer Name</a></li>
                <li data-role="divider">View Purchase History</li>
                <li><a href="#">Last 2 Weeks of Purchases</a></li>
                <li><a href="#">Last 4 Weeks Purchases</a></li>
                <li><a href="#">Last 3 months of Purchases</a></li>
                <li><a href="#">Last 6 months of purchases</a></li>
                <li><a href="#">Last 1 year of Purchases</a></li>
                <li><a href="#">View Quoted Items</a></li>
                <li><a href="#">View Recently Searched Items</a></li>
                <li><a href="#">View Promos and Featured Items</a></li>
            </ul>
        </div>
    </div>
    @*Added by Janine Oct 17 2pm*@
    <div data-role="dialog" id="dialogInvoice">
	
		<div data-role="header" data-theme="d">
			<h1>Cart Preview</h1>
		</div>

		<div data-role="content" data-dom-cache="true">
            <label id="headerInvoiceDate"></label><br />
            <label id="headerClientName"></label>
            <br /><br />
            <table data-role="table" id="tblInvoice" data-mode="reflow" class="ui-responsive myTable">
                <thead>
                    <tr>
                        <th>Product No</th>
                        <th>Product Name</th>
                        <th>Qty</th>
                        <th>Unit Price</th>
                    </tr>                    
                </thead>                
                <tbody id='tblInvoiceBody'>
                </tbody>
            </table>
            <br />
            <div style="text-align: right;"><label id="lblSKUCount" ></label></div><br />
            <div style="text-align: right;"><label id="lblTotal" style="font-size: x-large" ></label></div>
            <br />
			<a data-role="button" data-theme="c" id="btnLoadCart">Load Cart</a>    
            <a data-role="button" data-rel="back" id="btnCancel" data-theme="c">Cancel</a> 
		</div>
	</div>
    <div data-role="page" id="pageOrderTaker" >
    </div>
    <div data-role="page" id="pageCustomerOptions" >
    </div>

   <div data-role="page" id="PreviewBill">
    <div data-role="content" style="padding:0px 20px 0px 20px">
        <div class="ui-grid-a">
            <div class="ui-block-a" style="width: 20%">
                 <button data-icon="back" id="btnBackPreviewBill">Back</button>
            </div>
            <div class="ui-block-b" style="text-align: right;width: 80%">
               <h2>Billing</h2>
            </div>
        </div>

        <div id="divDisplayBill">
                <table style="width: 100%">
                    <tr>
                        <td colspan="2" style="text-align:center">
                            <label id="lblOrganizationBusinessName" style="font-size:xx-large; font-weight:bold"></label>
                        </td>
                    </tr>
                    <tr>
                 
                        <td style="text-align:left">
                            <br />
                            <br />
                            Customer :
                        </td>
                        <td style="text-align:right">
                             <br />
                            <br />
                            <label id="lblCustomerNamePreviewBill"> - </label>                        
                        </td>
                    </tr>
                      <tr>
                        <td style="text-align:left">
                            Address :
                        </td>
                        <td style="text-align:right">
                            <label id="lblAddressPreviewBill"> -  </label>                        
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align:left">
                            TIN :
                        </td>
                        <td style="text-align:right">
                            <label id="lblTINPreviewBill"> -  </label>                        
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align:left">
                            Table # :
                        </td>
                        <td style="text-align:right">
                            <label id="lblTableNoPreviewBill"> -  </label>                        
                        </td>
                    </tr>
                </table>
                    <hr />
                <table style="width: 100%">
                    <tr>
                        <td colspan="2" style="text-align:center">
                            <label id="lblBillNoPreviewBill" style="font-size:xx-large; font-weight:bold"></label> <br />
                            <label id="lblDateTimePreviewBill" style="font-size:xx-large; font-weight:bold"></label>
                        </td>
                    </tr>
                    <tr>
                 
                        <td style="text-align:left">
                            <br />
                            Cashier :
                        </td>
                        <td style="text-align:right">
                        <br />
                            <label id="lblCashierNamePreviewBill"> -  </label>                        
                        </td>
                    </tr>
                </table>
       <br />
                    <div>
                         <table style="width:100%">
                            <thead>
                                <tr style="background-color:ButtonFace;">
                                    <td style="text-align:center; font-weight:bold; width: 35%; height:35px; border: 1px solid Gray;">Stock Description</td>
                                    <td style="text-align:center; font-weight:bold; width: 10%; height:35px; border: 1px solid Gray;">Qty</td>
                                    <td style="text-align:center; font-weight:bold; width: 15%; height:35px; border: 1px solid Gray;">Unit Measure</td>
                                    <td style="text-align:center; font-weight:bold; width: 20%; height:35px; border: 1px solid Gray;">Unit Price</td>
                                    <td style="text-align:center; font-weight:bold; width: 20%; height:35px; border: 1px solid Gray;">Sub Total</td>
                                </tr>
                            </thead>

                            <tbody id='tblBillBody'>
                 
                            </tbody>

                        </table>

                    </div>
                    <hr />
                    <table style="width: 100%">
                    <tr>
                        <td style="text-align:left; font-weight:bold">     
                             <br />                   
                            SKU Count :
                        </td>
                        <td style="text-align:right; font-weight:bold">
                         <br />    
                            <label id="lblSKUCountBilling"> - </label>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align:left; font-weight:bold">
                            Sub Total :
                        </td>
                        <td style="text-align:right; font-weight:bold">
                            <label id="lblSubTotalBilling"> - </label>
                        </td>
                    </tr>
                    <tr id="rowVATPercentBilling" style="display:none">
                        <td style="text-align:left; font-weight:bold">
                            Less : <label id="lblVATPercentValueBilling"></label>% VAT
                        </td>
                        <td style="text-align:right; font-weight:bold">
                            <label id="lblVATPercentBilling"> - </label>
                        </td>
                    </tr>
                    <tr id="rowAmountLessVATBilling" style="display:none">
                        <td style="text-align:left; font-weight:bold">
                            Total Amount :
                        </td>
                        <td style="text-align:right; font-weight:bold">
                            <label id="lblTotalAmountBilling"> - </label>
                        </td>
                    </tr>
      
                    <tr>
                        <td style="text-align:left; font-weight:bold">
                            Less : Discount Amount
                        </td>
                        <td style="text-align:right; font-weight:bold">
                            <label id="lblSalesPayTotalDiscountBilling"> - </label>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align:left; font-weight:bold">
                            Add : Charges
                        </td>
                        <td style="text-align:right; font-weight:bold">
                            <label id="lblSalesPayChargesBilling"> - </label>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align:left; font-weight:bold">
                            Add : Tip 
                        </td>
                        <td style="text-align:right; font-weight:bold">
                            <label id="lblTipBilling"> - </label>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align:left; font-weight:bold">
                            Add : Local Tax
                        </td>
                        <td style="text-align:right; font-weight:bold">
                            <label id="lblSalesPayOtherTaxBilling"> - </label>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align:left; font-size:xx-large; font-weight:bold">
                            <br />
                            Amount Due :
                        </td>
                        <td style="text-align:right; font-size:xx-large; font-weight:bold">
                           <br />
                            <label id="lblOriginalAmountDueBilling" style="display:none"></label>
                            <label id="lblSalesPayAmountDueBilling"> - </label>
                        </td>
                    </tr>

                    <tr>
                        <td style="text-align:left; font-weight:bold">     
                             <br />                   
                           VAT-able Sales :
                        </td>
                        <td style="text-align:right; font-weight:bold">
                         <br />    
                            <label id="lblVATableSales"> - </label>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align:left; font-weight:bold">
                            VAT Amount :
                        </td>
                        <td style="text-align:right; font-weight:bold">
                            <label id="lblVATAmount"> - </label>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align:left; font-weight:bold">
                           Non-VAT Sales :
                        </td>
                        <td style="text-align:right; font-weight:bold">
                            <label id="lblNonVATSales"> - </label>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align:left; font-weight:bold">
                            VAT Exempt Sales :
                        </td>
                        <td style="text-align:right; font-weight:bold">
                            <label id="lblVATExemptSales"> - </label>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align:left; font-weight:bold">
                            Zero-Rated VAT Sales :
                        </td>
                        <td style="text-align:right; font-weight:bold">
                            <label id="lblZeroRatedVATSales "> - </label>
                        </td>
                    </tr>
                </table>



            </div>

        <br />
        <hr />
         <h2>Add Tip</h2>
         <div class="ui-grid-c">
            <div class="ui-block-a" >
                 <div style="text-align: center">
                    <a id="btnAddTip10" data-role="button">
                    <h3>
                        10</h3>
                    </a>
                </div>
            </div>
            <div class="ui-block-b">       
                 <div style="text-align: center">
                    <a id="btnAddTip20" data-role="button">
                    <h3>
                        20</h3>
                    </a>
                </div>
            </div>
             <div class="ui-block-c">       
                 <div style="text-align: center">
                    <a id="btnAddTip50" data-role="button">
                    <h3>
                        50</h3>
                    </a>
                </div>
            </div>
             <div class="ui-block-d">       
                 <div style="text-align: center">
                    <a id="btnAddTip100" data-role="button">
                    <h3>
                        100</h3>
                    </a>
                </div>
            </div>
        </div>
         <div class="ui-grid-a">
            <div class="ui-block-a" >
                <label for="txtAddTip" style="font-size:x-large;margin-top:15px">Other Amount</label>
            </div>
            <div class="ui-block-b">       
                  <input type="number" id="txtAddTip" style="line-height: 2em;text-align: right;"/>
            </div>
        </div>
        <br />
        <hr />
    <h2>Adjustments</h2>
         <div class="ui-grid-a">
            <div class="ui-block-a">
                 <div style="text-align: center">
                    <a id="btnAddCharges" data-role="button" data-icon="add">
                    <h3>
                        Charges</h3>
                    </a>
                </div>
             
            </div>
            <div class="ui-block-b">
                 <div style="text-align: center">
                    <a id="btnAddDiscount" data-role="button"  data-icon="minus">
                    <h3>
                       Discounts</h3>
                    </a>
                </div>
           
            </div>
        </div>
        
        <br />
        <div style="text-align: center">
            <a id="btnPrintBilling" data-role="button" data-theme="e">
            <h3>
                Print Billing</h3>
            </a>
        </div>
         <div style="text-align: center">
            <a id="btnPaymentBilling" data-role="button" data-theme="e">
            <h3>
                Proceed to Payments</h3>
            </a>
        </div>
          <div style="text-align: center">
            <a id="btnBacktoTableViewBilling" data-role="button" data-theme="e">
            <h3>
                Go Back to Table View</h3>
            </a>
        </div>
         <br />
        <br />
         <br />
        <br />
    </div>

    <div data-role="popup" id="popupCharges" class="ui-corner-all"  style="padding: 20px;width: 400px">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
			<center><h3>Charges and Other Tax</h3></center>
            <div data-role="content">
                <label for="selChargeTypeBilling">Charge Type: </label>
                <select id="selChargeTypeBilling">
                    <option value="Service Charge">Service Charge</option>
                    <option value="Delivery Charge">Delivery Charge</option>
                    <option value="Other Charges">Other Charges</option>
                    <option value="Local Tax">Local Tax</option>
                </select>

                <label for="txtAddChargeBilling">Percentage (%): </label>
                <input type="number" id="txtAddChargePercentBilling" style="text-align: right;"/>

                <label for="txtAddChargeBilling">Amount: </label>
                <input type="number" id="txtAddChargeBilling" style="text-align: right;"/>

                <label for="txtChargeRemarksBilling">Remarks: </label>
                <textarea rows="3" id="txtChargeRemarksBilling"></textarea>
                <br />
                <button id="btnAddChargeBilling">Add</button>
                <br />
                <hr />
                 <br />
                <table style="width:100%">
                   <thead>
                        <tr style="background-color:ButtonFace;">
                            <th style="text-align: center">Type</th>
                            <th style="text-align: center">Amount</th>
                            <th style="text-align: center">Remarks</th>
                            <th></th>
                        </tr>
                                   
                    </thead> 
                            
                    <tbody id='tblChargesListBilling'>
                 
                    </tbody>
                  
                </table>

               <br />
               <hr />
               <div class="ui-grid-a">
                    <div class="ui-block-a">
                         <div>
                            <h3>
                                Total Charges :</h3>
                        </div>
             
                    </div>
                    <div class="ui-block-b">
                         <h3 id="lblBillTotalChargesBilling" style="text-align: right">
                           0.00
                        </h3>
           
                    </div>
                </div>

           </div>

       </div>

       <div data-role="popup" id="popupDiscounts" class="ui-corner-all"  style="padding: 20px; width: 400px">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
				<center><h3>Discounts</h3></center>
                <div data-role="content">
                    <div data-role="fieldcontain">
	                    <label for="selDiscountTypeBilling" class="select">Discount Type:</label>
                        <select id="selDiscountTypeBilling">
	                        <option value="">Please Select</option>
                        </select>    
                    </div>

                    <div style="display:none;"><input type="number" id="txtDiscountPercentCeilingBilling" readonly style="text-align: right;"/></div>
          
	                <label for="txtDiscountPercentBilling">Percent Discount (%): </label>
                    <input type="text" id="txtDiscountPercentBilling" style="text-align: right;"/>

                    <label for="txtDiscountAmountBilling">Discount Amount: </label>
                    <input type="text" id="txtDiscountAmountBilling" style="text-align: right;"/>

	                <label for="txtDiscountRemarksBilling">Remarks: </label>
                    <textarea rows="3" id="txtDiscountRemarksBilling"></textarea>
    

                    <div id="divAddCustomerDetailsBilling" style="display:none;">
                            <div id="divSCPWDDiscountBilling" style="display:none;">
                        
                                <label for="txtSalesPayNoOfSCBilling">Number of Senior Citizens: <span style="color: #FF0000;">*</span></label>
                                <input type="number" id="txtSalesPayNoOfSCBilling" value="1" style="text-align: right;"/>
                      
                        
                                <label for="txtSalesPayNoOfDinersBilling">Number of People (including the Senior Citizen): <span style="color: #FF0000;">*</span></label>
                                <input type="number" id="txtSalesPayNoOfDinersBilling" value="1" style="text-align: right;"/>
                      
                            </div>
                            <div>
                                <ul data-role="listview" id="lstCustomerContainerBilling" data-inset="true" >   
				                    <li data-role="list-divider">List of Senior Citizens / PWD Availing the Discount</li>
                                    <li>
                                        <div data-role="fieldcontain"><a id="btnAddNewCustomerBilling" data-role="button" data-icon="plus" data-inline="true">Add New Senior Citizen / PWD Customer</a></div>

                                        <form id="frmCustomerSearchBilling">
                                            <div><input id="txtCustomerSearchBilling" type="search" value="" placeholder="Type Customer Name or Identification Number" /></div>
                                        </form>

                                        <div>
                                            <ul data-role="listview" id="lstSalesPayCustomerNameBilling" data-inset="true">   
                                            </ul>
                                        </div>
                                    </li>
                                </ul>
                            </div>

                        
                    </div>
               @*         <br />
                           
                     <button id="btnApplyDiscountBilling">Apply Discount</button>*@
                </div>

       </div>
       
</div>

<div data-role="dialog" id="dialogPrintConfirmationBill" class="ui-corner-all" data-dismissible = "false">
    <div data-role="content">
        <h2>Print Confirmation</h2>
        Your transaction has been saved. Were you able to successfully print out a bill?

            <br />
            <div style="text-align: center">
            <a id="btnPrintYesBill" data-role="button">
            <h3>
                Yes</h3>
            </a>
        </div>
            <div style="text-align: center">
            <a id="btnPrintNoBill" data-role="button">
            <h3>
                No, it didn't print</h3>
            </a>
        </div>
    </div>

</div>

<div id="pageDiagnosePrinter" data-role="page">
    <div data-role="header">
            <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>
           Diagnose Printer</h1>
    </div>
    <div data-role="content" style="padding-top: 40px">
        <h3>You seem to have trouble printing</h3>
        Please make sure that:
        <ol>
            <li>Your printer is powered on.</li>
            <li>There is enough paper (and/or ink) on your printer.</li>
            <li>The printer cable is properly connected.</li>
            <li>If your printer is attached to a Wi-Fi router via LAN cable, please check the back of your printer for a lit LAN indicator light. If the indicator light is not lit, then your cable is not properly connected. Please re-attach LAN cable until the LAN indicator light at the back of your printer is on. Also, please make sure that your router’s LAN indicator light is properly lit to indicate a good connection.</li>
            <li>Reset your device if all else fails.</li>
        </ol>

          <br />
        <div style="text-align: center">
            <a id="btnReprint" data-role="button">
            <h3>Try to reprint again</h3>
            </a>
        </div>
        <div style="text-align: center">
            <a id="btnPrintOther" data-role="button">
            <h3>Print on another printer</h3>
            </a>
        </div>

         <br />
      
    </div>
</div>

<div id="pageOtherPrinter" data-role="page">
    <div data-role="header">
            <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>
           Print on Another Printer</h1>
    </div>
    <div data-role="content" style="padding-top: 40px">
        <h3>Please select a printer you want to use</h3>
        
        <select id="selOtherPrinter">
              
        </select>
     
         <h3>... or define a printer</h3>

          <div data-role="fieldcontain">
            <label for="txtNewPrinterName">Printer Name: <span style="color: #FF0000;" >*</span></label>
            <input type="text" id="txtNewPrinterName" maxlength="50"/>
          
         </div>
         <div data-role="fieldcontain">
            <label for="txtNewPrinterIP">Printer's IP Address: <span style="color: #FF0000;" >*</span> <a href="#" id="lnkInstruction">?</a></label>
           <input type="text" id="txtNewPrinterIP" maxlength="15"/>
     
        </div>
        <br />
        <div style="text-align: center">
            <a id="btnSavePrint" data-role="button">
            <h3>Save and Print</h3>
            </a>
        </div>

         <br />
    </div>

    <div data-role="popup" id="popupIPAddressInstruction" class="ui-corner-all">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
		
        <div data-role="content">
            <h2>Instructions on how to know your printer's IP address</h2>
            <h3>For Epson Thermal Receipt Printers</h3>
            <ol>
                <li>Turn off printer.</li>
                <li>Press the “Feed” button while printer is off.</li>
                <li>Turn on the printer while still pressing on the “Feed” button.</li>
                <li>Do not release the “Feed” button until printer prints out IP details.</li>
                <li>Use the IP address details to define printer on the previous page.</li>
            </ol>

             <br />
        </div>

    </div>


</div>

 <div data-role="dialog" id="dialogPrintConfirmation" class="ui-corner-all" data-dismissible = "false">
    <div data-role="content">
        <h2>Print Confirmation</h2>
        Your transaction has been saved. Were you able to successfully print out a receipt?

            <br />
            <div style="text-align: center">
            <a id="btnPrintYes" data-role="button">
            <h3>
                Yes</h3>
            </a>
        </div>
            <div style="text-align: center">
            <a id="btnPrintNo" data-role="button">
            <h3>
                No, it didn't print</h3>
            </a>
        </div>
    </div>

</div>

<div data-role="dialog" id="dialogCheckerLogIn" class="ui-corner-all" data-dismissible="false">
    <div data-role="content">
        <h2>Checker Log-in</h2>
        @*<form id="frmPeopleLogger" method="POST" autocomplete="off">*@
        <div class="alert alert-danger alert-dismissible" role="alert" id="spanError" style="display:none;text-align:center">
            Employee not found.
        </div>

            <div class="row">
                <div class="form-horizontal">
                    <div class="col-md-3">
                        <label class="control-label" style="color:#848385;" for="txtCheckerUsername">Username</label>
                    </div>
                    <div class="col-md-9">
                        <input type="text" class="form-control" data-role="none" name="username" id="txtCheckerUsername" />
                    </div>
                </div>
            </div>
            <br />
            <div class="row">
                <div class="form-horizontal">
                    <div class="col-md-3">
                        <label class="control-label" style="color:#848385;" for="txtCheckerPassword">Password</label>
                    </div>
                    <div class="col-md-9">
                        <input type="password" class="form-control" data-role="none" name="password" id="txtCheckerPassword" />
                    </div>
                </div>
            </div>
            <br />
            <div class="row">
                <div class="col-md-offset-3 col-md-9">
                    <button id="btnCheckerSignIn" class="btn btn-primary" data-role="none" style="width:100%;padding:5px">Sign In</button>
                </div>
            </div>

            <br />
            <br />
            <div class="row">
                <div class="form-horizontal">
                    <div class="col-md-3">
                        <label class="control-label" style="color:#848385;" for="txtCheckerBarcode">Employee Barcode</label>
                    </div>
                    <div class="col-md-9">
                        <input type="password" class="form-control" data-role="none" name="password" id="txtCheckerBarcode" />
                    </div>
                </div>
            </div>

        <br />
        <br />
        @*</form>*@
    </div>

</div>

<div id="pageViewTransactions" data-role="page">
    <div data-role="header" id="divHeaderTransPage" data-position="fixed" style="background: none white; border: none">
        <div class="ui-grid-b" >
            <div class="ui-block-a">
                <div id="btnHomeTransDetails" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    <div><img src="/Content/images/home.png" style="width:27px;" /></div>
                    <span style="font-size:13px;">Home</span>
                </div>
            </div>
            <div class="ui-block-b">
                <div id="btnRecordLookup" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    <div><img src="/Content/images/RecordLookup.png" style="width:20px;" /></div>
                    <span style="font-size:13px;">Search Record</span>
                </div>
            </div>
            <div class="ui-block-c">
                <div id="btnNewTransaction"style="margin:0px; height: 80px;" data-role="button" data-theme="green" data-corners="false">                    
                    <div><img src="/Content/images/Update_Cart.png" style="width:30px;" /></div>
                    <span style="font-size:13px;">New Transaction</span>
                </div>
            </div>
        </div>

        <div>
            <div style="margin: 0px;" data-role="button" data-theme="b" data-corners="false">
                <div class="lblPosBranch" style="float:left; text-overflow:ellipsis; overflow:hidden; width:40%; text-align:left; white-space:nowrap;">
                </div>
                <div style="float:right">
                    <span id="clockTransactionDetails"></span>
                </div>
                <div style="clear:both"></div>            
            </div>
        </div>
    </div>
    <div data-role="content" id="divContentTransPage">
        @*<div class="custom-ui-label">
            <div>PO No.:</div>
            <div>1234567</div>
        </div>*@

        <div style="text-align:center;">
            <h3 id="txtROPONo">PO No.: -</h3>
        </div>

        <div class="custom-ui-label">
            <div>Date and Time:</div>
            <div id="txtRODatePurchase">-</div>
            <div style="clear:both;"></div>
        </div>

        <div class="custom-ui-label">
            <div>Customer:</div>
            <div id="txtROCustomer">-</div>
            <div style="clear:both;"></div>
        </div>

        @*Order Branch *@

        <div class="custom-ui-label">
            <div>Status:</div>
            <div id="txtROStatus">-</div>
            <div style="clear:both;"></div>
        </div>

        <div class="custom-ui-label">
            <div>Item Sub Total:</div>
            <div id="txtSubTotal" style="font-weight:bold;">-</div>
            <div style="clear:both;"></div>
        </div>

        <br />

        <ul id="lstROPOItems" data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm ui-listview">
            <li>
                <a id="">
                    <b>Cheese Burger</b>
                    <br />
                    <br />
                    1 &nbsp; &nbsp; Order &nbsp;&nbsp; 95.00 &nbsp;&nbsp; 95.00
                    <br />
                    Item Total: 95.00
                </a>
            </li>
        </ul>
    </div>
</div>
End Section
 