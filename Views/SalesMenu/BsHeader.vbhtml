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
    require([GetPathFormat('/Scripts3/Bs_SalesMenu.js?crc=')]);
</script>
<nav class="navbar navbar-dark sticky-md-top bg-primary" style="padding:0px !important;">
    <div id="divPOSHeader" class="row" style="margin:0px !important;">
        <div class="col-3" style="padding:0px !important;">
            <button data-role="none" id="btnHome" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                <span id="iconHome" class="bi-house-fill"></span>
                <div id="lblHome" class="menu-description mt-1">Home</div>
            </button>
        </div>
        <div class="col-3" style="padding:0px !important;">
            <button data-role="none" id="btnCustomer" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                <span class="bi-person-fill"></span>
                <div class="menu-description mt-1"><span id="lblCustomerName">[CASH PAYMENT]</span></div>
            </button>
        </div>
        <div class="col-3" style="padding:0px !important; display:none;">
            @*hide this menu, gibutang na sya sa More Options *@
            <button data-role="none" id="btnPOSMode" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                <span class="bi-display"></span>
                <div class="menu-description mt-1" id="lblCurrentPOSMode"></div>
            </button>
        </div>
        <div class="col-3" style="padding:0px !important;">
            <button data-role="none" id="btnMoreOptions" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                <span class="bi-gear-fill"></span>
                <div class="menu-description mt-1">More Options</div>
            </button>
        </div>
        <div class="col-3" style="padding:0px !important;">
            <button data-role="none" id="lnkCheckout" style="height:100%;" class="btn btn-success btn-block btn-lg rounded-0">
                <span class="bi-arrow-right-square"></span>
                <div id="lblPayNow" class="menu-description mt-1">Go</div>
            </button>
        </div>
    </div>
    <div id="divBOMHeader" class="row" style="margin:0px !important; display:none;">
        <div class="col-3 col-md-2" style="padding:0px !important;">
            <a data-role="none" href="/People/PeopleMenu" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                <div class="menu-description mt-1">Home</div>
            </a>
        </div>
        <div class="col-3 col-md-2" style="padding:0px !important;">
            <button data-role="none" id="btnNewCart" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                <div class="menu-description mt-1">New Cart</div>
            </button>
        </div>
        <div class="col-3 col-md-2" style="padding:0px !important;">
            <button data-role="none" id="butBOMOptions" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                <div class="menu-description mt-1">More Options</div>
            </button>
        </div>
        <div class="col-3 offset-md-3 col-md-3" style="padding:0px !important;">
            <button data-role="none" id="btnMake" style="height:100%;" class="btn btn-success btn-block btn-lg rounded-0">
                <div class="menu-description mt-1">View Cart</div>
            </button>
        </div>
    </div>
    <br />
    <div class="row storedetails" style="margin:0px; padding:0.5em; width:100%; color:#FFF; background-color:#1969de; font-weight:bold;">
        <div class="col-5 lblPosBranch" style="text-overflow:ellipsis; overflow:hidden; text-align:left; white-space:nowrap;">
            &nbsp;
        </div>
        <div class="col-7" style="text-overflow:ellipsis; overflow:hidden; text-align:right; white-space:nowrap;">
            <span id="clock">&nbsp;</span>
        </div>
    </div>
</nav>
      <div data-role="popup" id="popupPayNowMode" class="ui-corner-all"  style="padding: 20px; display:none;">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
			<center><h3>Billing Option</h3></center>
            <div data-role="content">
                <a id="lnkProceedToPayments" data-role="button" data-theme="e"> <h3>Proceed to Payments</h3> </a>
                <a id="lnkBackToTableView" data-role="button" data-theme="e"> <h3>Go Back to Table View</h3> </a>
                <a id="lnkPrintBill" data-role="button" data-theme="e"> <h3>Billing</h3> </a>
           </div>
       </div>
     <div data-role="popup" id="popupPOSMode" class="ui-corner-all" style="padding: 20px; display:none;">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
			<center><h3 id="IsPOS">POS Mode</h3></center>
            <div data-role="content">
                <div id="divPOSMode">
                </div>
           </div>
       </div>
        
        <div data-role="popup" id="popupMoreOptions" class="ui-corner-all" style="width:auto;padding: 20px; display:none;">
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