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

</style>

    <script>
        require([GetPathFormat('/Scripts3/SalesMenu.js?crc=')]);
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
    </script>

      <div data-role="header" data-position="fixed1" style="background: none white; border: none">
        <div class="ui-grid-d" >
            <div class="ui-block-a" style="width: 12%;">
                <div id="btnHome" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                <span style="margin-top:5px;">Home</span>
                </div>
            </div>
            <div class="ui-block-b" style="width: 22%;">
                <div id="btnCustomer" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    <span id="lblLabel">Customer:<br /></span>
                    <span id="lblCustomerName"></span>
                </div>
            </div>
            <div class="ui-block-c" style="width: 22%;">
                <div id="btnPOSMode" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    <span id="IsPOSHeader">POS Mode:<br /></span>
                    <span id="lblCurrentPOSMode"></span>
                </div>
            </div>
            <div class="ui-block-d" style="width: 22%;">
                <div id="btnMoreOptions" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    <span>More <br />Options</span>
                </div>
            </div>
            <div class="ui-block-e" style="width: 22%;">
                <div id="lnkCheckout"style="margin:0px; height: 80px;" data-role="button" data-theme="green" data-corners="false">
                    <span id="lblPayNow">Pay <br />Now</span>
                </div>
            </div>
        </div>
        <div>
            <div style="margin:0px;" data-role="button" data-theme="b" data-corners="false">
                <span id="clock"></span>
            </div>
        </div>
   </div>

        <div data-role="popup" id="popupPayNowMode" data-theme="b" class="ui-corner-all">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
			<center><h3>Billing Option</h3></center>
            <div data-role="content">
                <a id="lnkProceedToPayments" data-role="button" data-theme="e"> <h3>Proceed to Payments</h3> </a>
                <a id="lnkPrintBill" data-role="button" data-theme="e"> <h3>Print Billing</h3> </a>
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
                <div style="text-align: center">
                    <a id="butSalesRepresentatives" data-role="button" data-theme="e">
                    <h3>Assign Sales Representatives</h3>
                    <p style="font-weight: normal; margin-top: -15px;">
                    Assign sales representatives for this transaction
                    </p>
                    </a>
                </div>
            </div>
       
		</div>
        <div data-role="popup" id="popupCustomerOptions" data-theme="b" class="ui-corner-all">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
			<center><h3>Customer</h3></center>
            <div data-role="content">
                   <div style="text-align: center">
                        <a id="btnClient" data-role="button" data-theme="e">
                        <h3>Customer Record Lookup</h3>
                        <p style="font-weight: normal; margin-top: -15px;"><label id="lnkCustomer" style="cursor:pointer;"></label>
                        <br />
                        Used to print “Named” receipts for government reporting. <br />It includes the name,
                        Tax Identification Number (TIN)<br /> and address of the customer.
                        </p>
                        </a>
                    </div>
                     <div style="text-align: center">
                        <a id="btnAddCustomer" data-role="button" data-theme="e">
                        <h3>Add New Customer</h3>
                        </a>
                    </div>
                    <div id="divCustomerDetailsRestaurant" style="display:none;text-align: center; font-weight: bold; border-radius: 1em; box-shadow: 0px 1px 3px rgba(0, 0, 0, 0.2); border: 1px solid #F4C63F;background: #FFE155; padding: 10px; margin-top: 8px;">
                              <h3>Customer / Table No.</h3>
                              <select id="selTableNo"></select>
                    </div>
                    <div id="divCustomerDetailsGasStation" style="display:none;text-align: center; font-weight: bold; border-radius: 1em; box-shadow: 0px 1px 3px rgba(0, 0, 0, 0.2); border: 1px solid #F4C63F;background: #FFE155; padding: 10px; margin-top: 8px;">
                              <h3>Gas Pump No.</h3>
                              <select id="selGasPumpNo"></select>
                    </div>
            </div>
       
		</div>
        <div data-role="popup" id="popupAddNewCustomer" data-theme="b" class="ui-corner-all">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
			<center><h3>Add New Customer</h3></center>

            <div data-role="content"  style="text-align: center; ">
            
                        <label for="selCustomerType">Customer Type</label>
                        <select id="selCustomerType">
                            <option value="Organization">Organization</option>
                            <option value="Person">Person</option>
                        </select>
       
                        <div id="divCustomerOrg">
                            <label for="txtCompanyName">Company Name <span style="color: #FF0000;" >*</span></label>
                            <input type="text" id="txtCompanyName" maxlength="255" style="text-align: center"/>
                       
                            <label for="txtCustomerTIN">Tax Identification Number (TIN) <span style="color: #FF0000;" >*</span></label>
                            <input type="text" id="txtCustomerTIN" maxlength="16" style="text-align: center"/>
                             <label for="txtCustomerOrgHouseNo">Address</label>
                             <input type="text" id="txtCustomerOrgHouseNo" maxlength="255" style="text-align: center" placeholder="House / Building No."/>
                              <input type="text" id="txtCustomerOrgAddress" maxlength="255" style="text-align: center" placeholder="Street"/>
                            <select id="selOrgCity">
                            </select>
                            <select id="selOrgCountry">
                            </select>

                        </div>
                        
                        <div id="divCustomerPerson" style="display:none;">
                            <label for="txtFirstName">First Name <span style="color: #FF0000;" >*</span></label>
                            <input type="text" id="txtFirstName" maxlength="255" style="text-align: center" />
                           
                            <label for="txtMiddleName">Middle Name <span style="color: #FF0000;" >*</span></label>
                            <input type="text" id="txtMiddleName" maxlength="255" style="text-align: center"/>
                
                            <label for="txtSurname">Surname <span style="color: #FF0000;" >*</span></label>
                            <input type="text" id="txtSurname" maxlength="255" style="text-align: center"/>

                            <label for="txtCustomerPersonTIN">Tax Identification Number (TIN)</label>
                            <input type="text" id="txtCustomerPersonTIN" maxlength="16" style="text-align: center"/>
                        </div>
                        <br />
		    	        <button id="btnCustomerNameSave" type="submit">Save</button>
                        <br />
<span>Please fill in all the required fields (*)</span>
                  </div>

        </div>
        
<h3 id="hRsd" style="display: none; margin-top: 0">
    Reconstruct Sales Data</h3>

<div data-role="content" style="padding:5px;">
 
    <div class="ui-grid-b" style="margin:0px; padding: 0px;">
        <div class="ui-block-a" style="width:70%;padding: 0px;margin-bottom:5px;">
            <form id="formSearch">
                <input id="txtSearch" type="search" placeholder="Search products" class="SearchText" style="height: 40px;padding:0px;" autofocus />
            
            </form>

        </div>
        <div class="ui-block-b" style="width: 15%;padding: 0px;margin:0px;">
            <center><div >
                <img src="/Content/images/barcode96x96.png" style="height: 50px;" class="detailimage2" id="butBarCode" data-role="button" data-corners="false"/>
            </div>
            </center>
        </div>
        <div class="ui-block-c" style="width: 15%;padding: 0px;margin:0px;">
            <center><div >
                @*<a href="#" id="butHotButtonsShortcut">Hot Buttons</a>*@
               
               
                <img src="/Content/images/hotb.jpg" class="detailimage2" style="height: 50px;" id="butHotButtonsShortcut" data-role="button" data-corners="false"/>
            </div>
            </center>
        </div>
    </div>
 
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

<div data-role="content" id="divCartPanel">
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
            <h3 name="spanStockDescription" style="white-space: normal">
            </h3>
            <table class="tableUnits">
                <tr name="trTemplate">
                    <td name="tdQty">
                    </td>
                    <td name="tdUnit">
                    </td>
                    <td name="tdPrice">
                    </td>
                    <td name="tdLineTotal">
                    </td>
                </tr>
            </table>
            <div>
                Item total: <span name="spanTotal"></span>
            </div>
        </div>

        <div data-role="fieldcontain" style="display:none">
            <label for="txtRemarks">
                Remarks:</label>
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
                    Reference (Invoice) Number of Original Sales Transaction:</label>
                <input id="txtRefNum" maxlength="100" />
            </div>
            <div data-role="fieldcontain">
                <label for="cbReasonForReconstructingData">
                    Reason for Reconstructing Data:</label>
                <select id="cbReasonForReconstructingData">
                </select>
            </div>
        </div>
        <div style="height: 10px">
        </div>
@*        <button id="lnkCheckout" data-theme="e">
            <h3>
                Go To Checkout</h3>
        </button>*@
        <a id="lnkHold" href="#" data-role="button">
        <h3>
        </h3>
        </a>
    </div>
    <div id="divCartEmpty" class="Empty">
        No items in cart
       

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
End Section
