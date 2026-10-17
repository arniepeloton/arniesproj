<style>
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
    /* Button */
    .HotButtons .ui-btn-inner {
        
        padding-top: 1.4em;
    }
    
</style>
@* notes by Arvin 2016_08_17__16_13_14_5333:
    SalesMenu.js is needed by POS hotbuttons
    but error in product maker
*@
<script>
    require([GetPathFormat('/Scripts3/SalesMenu.js?crc=')]);
</script>
    <div class="ui-grid-d" id="divPOSHeader">
        <div class="ui-block-a">
                <div id="btnHome" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    <div><img alt="home" src="/Content/images/home.png" width="30px" align="center" /><br /></div>
                    <div style="font-size:13px; margin-top:10px;">Home</div>
                </div>
            </div>
        <div class="ui-block-b">
            <div id="btnCustomer" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                <div style="margin-top:-10px"><img id="imgSupplier" alt="Customer" src="/Content/images/Customer.png" width="30px" align="center" /><br /></div>
                <span id="lblCustomerName" style="font-size:13px;"></span>
            </div>
        </div>
        <div class="ui-block-c">
            <div id="btnPOSMode" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                <div style="margin-top:-5px;"><img id="imgPosMode" alt="POS Mode" src="/Content/images/View_Mode.png" width="30px" align="center" /><br /></div>
                <div id="lblCurrentPOSMode" style="font-size:13px; line-height:20px !important;"></div>
            </div>
        </div>
        <div class="ui-block-d">
            <div id="btnMoreOptions" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                <div style="margin-top:-5px;"><img alt="More Options" src="/Content/images/More_Options.png" width="30px" align="center" /><br /></div>
                <span id="spanMoreOptions" style="font-size:13px;">More Options</span>
            </div>
        </div>
        <div class="ui-block-e">
            <div id="lnkCheckout"style="margin:0px; height: 80px;" data-role="button" data-theme="green" data-corners="false">
                <div style="margin-top:-5px;"><img src="/Content/images/Save.png" width="25px" /><br /></div>
                <span id="lblPayNow"  style="font-size:13px;">Go</span>
            </div>
        </div>
    </div>
    <div class="ui-grid-c" id="divBOMHeader" style="display:none">
        <div class="ui-block-a">
            <a href="/People/PeopleMenu" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                <span>Home</span>
            </a>
        </div>
        <div class="ui-block-b">
            <div id="btnNewCart" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                <span>New Cart</span>
            </div>
        </div>
        <div class="ui-block-c">
            <div id="butBOMOptions" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                <span>More Options<br /></span>
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
                <span id="clock"></span>
            </div>
            <div style="clear:both"></div>
        </div>
    </div>
      <div data-role="popup" id="popupPayNowMode" class="ui-corner-all"  style="padding: 20px">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
			<center><h3>Billing Option</h3></center>
            <div data-role="content">
                <a id="lnkProceedToPayments" data-role="button" data-theme="e"> <h3>Proceed to Payments</h3> </a>
                <a id="lnkBackToTableView" data-role="button" data-theme="e"> <h3>Go Back to Table View</h3> </a>
                <a id="lnkPrintBill" data-role="button" data-theme="e"> <h3>Billing</h3> </a>
           </div>
       </div>
     <div data-role="popup" id="popupPOSMode" class="ui-corner-all" style="padding: 20px">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
			<center><h3 id="IsPOS">POS Mode</h3></center>
            <div data-role="content">
                <div id="divPOSMode">
                </div>
           </div>
       </div>
        
        <div data-role="popup" id="popupMoreOptions" class="ui-corner-all" style="width:auto;padding: 20px">
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
@*                <div style="text-align: center">
                    <a id="butRecentlyBought" data-role="button" data-theme="e">
                    <h3>Recently Bought</h3>
                    <p style="font-weight: normal; margin-top: -15px;">
                    View recently bought items of the selected customer. <br /> 
                    Useful for regular customers who buy similar items frequently.
                    </p>
                    </a>
                </div>*@
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
                <div style="text-align: center" id="divChangeMenuButtons2" style="display:none;">
                    <a id="btnChangeMenuButtons2" data-role="button" data-theme="e">
                        <h3>Change Hot Button Template</h3>
                        <p style="font-weight: normal; margin-top: -15px;">
                            Lets you use a different set of hot buttons. Administrators <br />can customize these buttons online.
                        </p>
                    </a>
                </div>
            </div>
       
		</div>