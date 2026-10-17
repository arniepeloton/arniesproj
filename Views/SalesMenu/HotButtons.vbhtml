<div data-role="page" id="pageMainHB">
    
        <div  style="padding: 0px;">
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
    .ui-btn-down-gray {
        font-family: Helvetica, Arial, sans-serif;
        text-decoration: none;
    }
    
    /* Button */
    .HotButtons .ui-btn-inner {        
        border-top:0px !important;
        border-color:none !important;
    }

    .HotButtonsText .ui-btn-text {
        color: White;
    }
    
    .detailimage3 {
        height: 35px;
        text-align: center;
        margin: 0;

    }

    .detailimage3 img {
        width: 100%;
    }
    #pageMainHB
    {
        background-color:#2D2D2D;
    }
</style>
    <script>
        require([GetPathFormat('/Scripts3/HotButtons.js?crc=')]);
    </script>    
            <link rel="stylesheet" href="/Content/bootstrap-custom.css" />

   <div data-role="header" data-position="fixed1" style="border: none">
        <div class="ui-grid-d" id="divPOSHeader">
            <div class="ui-block-a" style="width: 12%;">
                <div id="btnHomeHotButtons" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    <div><img alt="home" src="/Content/images/home.png" width="30px" align="center" /><br /></div>
                    <div style="font-size:13px; margin-top:10px;">Home</div>
                </div>
            </div>
            <div class="ui-block-b" style="width: 22%;">
                <div id="divBlockCustomerHotButtons">
                    <div id="btnCustomerHotButtons" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                        <span id="lblLabelHotButtons" style="font-size:13px;"><img alt="Customer" src="/Content/images/Customer.png" width="30px" align="center" /><br /></span>
                        <span id="lblCustomerNameHotButtons" style="font-size:13px;"></span>
                    </div>
                </div>
                <div style="display: none;" id="divBlockSupplierHotButtons">
                    <div id="btnSupplierHotButtons" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                        <span style="font-size:13px;"><img alt="Customer" src="/Content/images/supplier.png" width="30px" align="center" /><br /></span>
                        <span style="font-size:13px;" id="lblSupplierNameHotButtons"></span>
                    </div>
               </div>
            </div>
            <div class="ui-block-c" style="width: 22%;">
                <div id="btnPOSMode" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    <span><img alt="POS Mode" src="/Content/images/View_Mode.png" width="30px" align="center" /><br /></span>
                    <div id="lblCurrentPOSModeHotButtons" style="font-size:13px; margin-top:10px;"></div>
                </div>
            </div>
            <div class="ui-block-d" style="width: 22%;">
                <div id="btnMoreOptionsHotButtons" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    <div><img alt="More Options" src="/Content/images/More_Options.png" width="30px" align="center" /><br /></div>
                    <div style="font-size:13px;margin-top:10px;">More Options</div>
                </div>
            </div>
            <div class="ui-block-e" style="width: 22%;">
                <div id="btnViewCart"style="margin:0px; height: 80px;" data-role="button" data-theme="green" data-corners="false">
                    <span><img alt="View Cart" src="/Content/images/Update_Cart.png" width="30px" align="center" /><br /></span>
                    <span style="font-size:13px;">View Cart</span>
                </div>
            </div>
        </div>
        <div class="ui-grid-c" id="divBOMHeader" style="display:none">
        <div class="ui-block-a">
            <div id="btnNewCart" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                <span>New Cart</span>
            </div>
        </div>
        <div class="ui-block-b">
            <div id="btnBOMFowst" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                <span>Current Location:<br /></span>
                <span id="lblCurrentFOWST"></span>
            </div>
        </div>
        <div class="ui-block-c">
            <div id="btnRequestedFor" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                <span>Requested For:<br /></span>
                <span id="lblRequestedFor"></span>
            </div>
        </div>
        <div class="ui-block-d">
            <div id="btnMake"style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
               <span>Make</span>
            </div>
        </div>
    </div>
        <div>
            <div style="margin:0px;" data-role="button" data-theme="b" data-corners="false">
                <div class="lblPosBranch" style="float:left; text-overflow:ellipsis; overflow:hidden; width:40%; text-align:left; white-space:nowrap;">
                </div>
                <div style="float:right">
                    <span id="clockHotButtons"></span>
                </div>
                <div style="clear:both"></div>
            </div>
        </div>
        <div>
            <div style="height:150px; padding: 0px; margin: 0px; cursor:pointer;overflow:auto;" id="btnOrder" data-role="button" data-corners="false">
                
                <div class="ui-grid-a">
                    <div class="ui-block-a" style="width: 30%;">
                        <label id="txtQtyUnit" style="font-size:large;cursor:pointer;" />
                    </div>
                    <div class="ui-block-b" style="text-align: left;width: 70%;">
                        <label id="txtOrder" style="padding-top:1%;padding-bottom:1%;font-size:20px;cursor:pointer;"/>
                    </div>
                </div>
            </div>
        </div>
</div>

    <div class="ui-grid-c">
        <div class="ui-block-a">
             <div id="btnSearchHotButtons" style="margin:0px; height: 80px;" data-role="button" data-theme="c" data-corners="false">
                <center><img src="/Content/images/search.png" class="detailimage3"/></center>
                <div style="font-size:13px;">Search</div>
            </div>
        </div>
        <div class="ui-block-b">
            <div id="butBarCodeHotButtons" style="margin:0px; height: 80px;text-align: center;" data-role="button" data-theme="c" data-corners="false">
                <center><img src="/Content/images/barcode96x96.png" class="detailimage3"/></center>
                <div style="font-size:13px;">Scan Barcode</div>
            </div>
        </div>
        @*<div class="ui-block-c">
           <div id="btnDeleteQTY" style="margin:0px; height: 80px;" data-role="button" data-theme="c" data-corners="false">
                <span style="font-size:32px;">-</span><br />
                <div style="font-size:13px;">Remove Qty</div>
            </div>
        </div>
        <div class="ui-block-d">
            
             <div id="btnAddQTY" style="margin:0px; height: 80px;" data-role="button" data-theme="c" data-corners="false">
                <span style="font-size:32px;">+</span><br />
                <div style="font-size:13px;">Add Qty</div>
            </div>
        </div>*@
    </div>
        <div class="ui-grid-c">

              <div class="ui-block-a">
                        <div style="margin:0px; height: 80px;" data-role="button" data-theme="green" data-corners="false" class="HotButtons HotButtonsText" id="btnHotButtonMain">
                            <span id="spanHotButtonMain">Main</span>
                        </div>
                    </div>
                    <div class="ui-block-b">
                        <div style="margin:0px; height: 80px;" data-role="button" data-theme="c" data-corners="false" class="HotButtons HotButtonsText" id="btnHotButtonGroup1">
                            <span id="spanHotButtonGroup1"></span>
                        </div>
                    </div>
                    <div class="ui-block-c">
                        <div style="margin:0px; height: 80px;" data-role="button" data-theme="c" data-corners="false" class="HotButtons HotButtonsText" id="btnHotButtonGroup2">
                            <span id="spanHotButtonGroup2"></span>
                        </div>
                    </div>
                    <div class="ui-block-d">
                        <div style="margin:0px; height: 80px;" data-role="button" data-theme="c" data-corners="false" class="HotButtons HotButtonsText" id="btnHotButtonGroup3">
                            <span id="spanHotButtonGroup3"></span>
                        </div>
                    </div>
         </div>

    <div class="divTable" name="divHotButtonsRow" id="divHotButtonsTemplate">
        <div class="divTableRow">
            <div style="margin:0px; padding:0; height:80px; border-top: 1px solid #FFF; border-top-color: rgba(255,255,255,.3);" data-role="button" data-theme="a" data-corners="false" class="divTableCell HotButtons HotButtonsText" name="btnHotButton0">
                <span name="btnHotButtonItem0"></span>
            </div>
            <div style="margin:0px; padding:0; height:80px; border-top: 1px solid #FFF; border-top-color: rgba(255,255,255,.3);" data-role="button" data-theme="a" data-corners="false" class="divTableCell HotButtons HotButtonsText" name="btnHotButton1">
                <span name="btnHotButtonItem1"></span>
            </div>
            <div style="margin:0px; padding:0; height:80px; border-top: 1px solid #FFF; border-top-color: rgba(255,255,255,.3);" data-role="button" data-theme="a" data-corners="false" class="divTableCell HotButtons HotButtonsText" name="btnHotButton2">
                <span name="btnHotButtonItem2"></span>
            </div>
        </div>
    </div>
         
    <div id="divHotButtonsSection">
        <div name="divHotButtonsRowNext">
         </div>
    </div>

    <br />
       <div data-role="page" id="pageTemplate">
            <div data-role="content" >

                    <div>
                        <ul data-role="listview" id="lstOrgHotButtonContainer" data-inset="true">   
				            <li data-role="list-divider">Select a Template </li>
                            <li>
                                <form id="frmHotButtonSearch">
                                    <div><input id="txtHotButtonSearch" type="search" value="" /></div>
                                </form>

                                <div data-role="fieldcontain" id="divHotButtonTemplate"></div>
                                <div>
                                    <ul data-role="listview" id="lstHotButtonTemplate" data-inset="true">   
                                    </ul>
                                </div>
                            </li>
                        </ul>
                    </div>

                    <div>
                        <br />
	                    <div style="text-align:center"> <a id="btnHotButtonsBack" data-role="button">Back</a> </div>
                    </div>

                </div>
       </div>
      <div data-role="popup" id="popupPOSModeHotButtons" data-theme="b" class="ui-corner-all">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
			<center><h3>POS Mode</h3></center>
            <div data-role="content">
                <div id="divPOSModeHotButtons">
                </div>
           </div>
       </div>

        <div data-role="popup" id="popupMoreOptionsHotButtons" data-theme="b" class="ui-corner-all" style="width:auto;">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
			<center><h3>More Options</h3></center>
            <div data-role="content">
                
                <div style="text-align: center">
                    <a id="btnFowstHotButtons" data-role="button" data-theme="e">
                    <h3>Current FOWST Location</h3>
                    <p style="font-weight: normal; margin-top: -15px;"><label id="lnkPosBranchHotButtons" style="cursor:pointer;"></label> 
                    <br />
                    Sales, inventory movements and payments will be tagged to this location.
                    </p>
                    </a>
                </div>
                 <div style="text-align: center">
                    <a id="butNewSalesOrderHotButtons" data-role="button" data-theme="e">
                    <h3>New Cart</h3>
                    <p style="font-weight: normal; margin-top: -15px;">
                    Puts the current cart on hold and displays a new one for the next customer.
                    </p>
                    </a>
                </div>
                <div style="text-align: center">
                    <a id="lnkSalesOrderHotButtons" data-role="button" data-theme="e">
                    <h3>Recall Cart</h3>
                     <p style="font-weight: normal; margin-top: -15px;">
                     Used to retrieve carts that were placed on hold.
                    </p>
                    </a>
                </div>

                <div style="text-align: center">
                    <div style="font-weight: bold; border-radius: 1em; box-shadow: 0px 1px 3px rgba(0, 0, 0, 0.2); border: 1px solid #F4C63F;background: #FFE155; padding: 10px;">

                        <h3>Price Level</h3>
                        <select id="selPriceLevel">
                        </select>
                    </div>
                </div>
                <div style="text-align: center" id="divChangeMenuButtons" style="display:none;">
                    <a id="btnChangeMenuButtons" data-role="button" data-theme="e">
                    <h3>Change Hot Button Template</h3>
                    <p style="font-weight: normal; margin-top: -15px;">
                    Lets you use a different set of hot buttons. Administrators <br />can customize these buttons online.
                    </p>
                    </a>
                </div>

@*                <div style="text-align: center">
                    <a id="btnTagAsVIPOrder" data-role="button" data-theme="e">
                    <h3>Tag as VIP Order</h3>
                    <p style="font-weight: normal; margin-top: -15px;">
                    For VIP Customers
                    </p>
                    </a>
                </div>*@
@*                <div style="text-align: center">
                    <div style="font-weight: bold; border-radius: 1em; box-shadow: 0px 1px 3px rgba(0, 0, 0, 0.2); border: 1px solid #F4C63F;background: #FFE155; padding: 10px;">

                        <h3>Payment Mode</h3>
                        <select id="selPaymentMode">
                        </select>
                    </div>
                </div>*@

            </div>
       
		</div>

 
      <div data-role="popup" id="popupDecimalInput" class="ui-corner-all" style="padding: 20px;">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
			<center><h3 id="hdrQty"></h3></center>
            <div data-role="content">
                 <div id="divQty" style="text-align: center; font-weight: bold;">
                        Quantity
                        <input id="txtDecimalQty" type="text" maxlength="15" style="text-align: center; "/>
                        <span id="spanErrorQty" style="display:none; color: red; font-size:small;">Please enter a valid quantity</span>
		    	        <button id="btnSaveQty" type="submit">Ok</button>

                        <br />  
                 </div>
           </div>
       </div>

    </div>
</div>

