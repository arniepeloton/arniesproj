<script>
    $('#pageHotButton').css('top', $('#divHotButtonHeader').height() + 'px')
    require([GetPathFormat('/Scripts3/Bs_HotButtons.js?crc=')]);
</script>

<div id="pageMainHB" class="bg-dark" style="height:100%;">
    <nav id="divHotButtonHeader" class="navbar navbar-dark sticky-md-top bg-primary" style="padding:0px !important; position:fixed; top:0px; left:0px; right:0px; z-index:1;">
        <div id="divPOSHeader" class="row" style="margin:0px !important;">
            <div class="bg-warning text-dark font-weight-bold pl-2 pr-2 w-100" name="hRsd" style="display:none;">
                <div class="row align-items-center">
                    <div class="col">Reconstruct Sales Data</div>
                    <div class="col-auto"><button data-role="none" class="btn" name="btnExitRSD">Exit</button></div>
                </div>
            </div>
            <div class="col-3 col-md-2" style="padding:0px !important;">
                <button data-role="none" id="btnHomeHotButtons" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                    <span id="iconHBHome" class="bi-house-fill"></span>
                    <div id="lblHBHome" class="menu-description mt-1">Home</div>
                </button>
            </div>
            <div class="col-3 col-md-2" style="padding:0px !important;">
                <div id="divBlockCustomerHotButtons">
                    <button data-role="none" id="btnCustomerHotButtons" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                        <span class="bi-person-fill"></span>
                        <div class="menu-description mt-1"><span id="lblCustomerNameHotButtons">[CASH PAYMENT]</span></div>
                    </button>
                </div>
                <div style="display:none;" id="divBlockSupplierHotButtons">
                    <button data-role="none" id="btnSupplierHotButtons" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                        <span class="bi-person-fill"></span>
                        <div class="menu-description mt-1"><span id="lblSupplierNameHotButtons">&nbsp;</span></div>
                    </button>
                </div>
            </div>
            <div class="col-3 col-md-2" style="padding:0px !important; display:none;">
                @*hide this menu*@
                <button data-role="none" id="btnPOSMode" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                    <span class="bi-display"></span>
                    <div class="menu-description mt-1" id="lblCurrentPOSModeHotButtons"></div>
                </button>
            </div>
            <div class="col-3 col-md-2" style="padding:0px !important;">
                <button data-role="none" id="btnMoreOptionsHotButtons" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                    <span class="bi-gear-fill"></span>
                    <div class="menu-description mt-1">More Options</div>
                </button>
            </div>
            <div class="col-3 offset-md-3 col-md-3" style="padding:0px !important;">
                <button data-role="none" id="btnViewCart" style="height:100%;" class="btn btn-success btn-block btn-lg rounded-0">
                    <span id="iconPay" class="bi-cart4"></span>
                    <div class="menu-description mt-1">View Cart</div>
                </button>
            </div>
        </div>

        <div id="divBOMHeader" class="row" style="margin:0px !important; display:none;">
            <div class="col-3 col-md-2" style="padding:0px !important;">
                <button data-role="none" id="btnNewCart" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                    <div class="menu-description mt-1">New Cart</div>
                </button>
            </div>
            <div class="col-3 col-md-2" style="padding:0px !important;">
                <button data-role="none" id="btnBOMFowst" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                    <div class="menu-description mt-1">Current Location: <span id="lblCurrentFOWST"></span></div>
                </button>
            </div>
            <div class="col-3 col-md-2" style="padding:0px !important;">
                <button data-role="none" id="btnRequestedFor" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                    <div class="menu-description mt-1">Requested for: <span id="lblRequestedFor"></span></div>
                </button>
            </div>
            <div class="col-3 offset-md-3 col-md-3" style="padding:0px !important;">
                <button data-role="none" id="btnMake" style="height:100%;" class="btn btn-success btn-block btn-lg rounded-0">
                    <div class="menu-description mt-1">View Cart</div>
                </button>
            </div>
        </div>
        <br />
        <div class="row" style="margin:0px; padding:0.5em; width:100%; color:#FFF; background-color:#1969de; font-weight:bold;">
            <div class="col-5 lblPosBranch" style="text-overflow:ellipsis; overflow:hidden; text-align:left; white-space:nowrap;">
                &nbsp;
            </div>
            <div class="col-7" style="text-overflow:ellipsis; overflow:hidden; text-align:right; white-space:nowrap;">
                <span id="clockHotButtons">&nbsp;</span>
            </div>
        </div>
        <div style="height:150px; padding: 10px; margin: 0px; cursor:pointer; overflow-y:auto; overflow-x:hidden; background-color:#FFF; width:100%;" id="btnOrder">
            <div class="row">
                <div class="col-4 text-center">
                    <label id="txtQtyUnit" style="font-size:large;cursor:pointer;">&nbsp;</label>
                </div>
                <div class="col-8">
                    <label id="txtOrder" style="padding-top:1%;padding-bottom:1%;font-size:20px;cursor:pointer;">&nbsp;</label>
                </div>
            </div>
        </div>
    </nav>
    <div id="divHotButtonContent" style="padding: 0px; height:100%;">
        <div class="btn-group btn-group-lg" style="width:100%;">
            <button data-role="none" id="btnSearchHotButtons" class="btn btn-light rounded-0">
                <span class="bi-search"></span> Search
            </button>
            <button data-role="none" id="butBarCodeHotButtons" class="btn btn-light rounded-0">
                <span class="bi-upc-scan"></span> Scan Barcode
            </button>
            @*<button data-role="none" id="btnDeleteQTY" class="btn btn-danger">
                <span class="bi-dash"></span> Remove Qty
            </button>
            <button data-role="none" id="btnAddQTY" class="btn btn-success">
                <span class="bi-plus"></span> Add Qty
            </button>*@
        </div>

        <div class="row m-0" style="background-color:#FFF;">
            <div class="col-3 p-0">
                <button data-role="none" id="btnHotButtonMain" class="btn btn-outline-success active btn-block rounded-0" style="height:80px;">
                    <span id="spanHotButtonMain"></span>
                </button>
            </div>
            <div class="col-3 p-0">
                <button data-role="none" id="btnHotButtonGroup1" class="btn btn-outline-success btn-block rounded-0 no-hover" style="height:80px;">
                    <span id="spanHotButtonGroup1"></span>
                </button>
            </div>
            <div class="col-3 p-0">
                <button data-role="none" id="btnHotButtonGroup2" class="btn btn-outline-success btn-block rounded-0 no-hover" style="height:80px;">
                    <span id="spanHotButtonGroup2"></span>
                </button>
            </div>
            <div class="col-3 p-0">
                <button data-role="none" id="btnHotButtonGroup3" class="btn btn-outline-success btn-block rounded-0 no-hover" style="height:80px;">
                    <span id="spanHotButtonGroup3"></span>
                </button>
            </div>
        </div>

        <div id="divHotButtonLoading" class="mt-3 mb-3" style="width:100%; text-align:center;">
            <div class="spinner-border text-light" role="status">
                <span class="visually-hidden"></span>
            </div>
        </div>

        <div name="divHotButtonsRow" id="divHotButtonsTemplate">
            <div class="row m-0">
                <div style="border:1px solid #272a2d; min-height:80px;" class="col-4 HotButtons HotButtonsText bg-dark p-0">
                    <button data-role="none" name="btnHotButton0" class="btn btn-dark btn-block rounded-0" style="height:100%;">
                        <div name="btnHotButtonItem0" style="word-break:break-word;"></div>
                    </button>
                </div>
                <div style="border:1px solid #272a2d; min-height:80px;" class="col-4 HotButtons HotButtonsText bg-dark p-0">
                    <button data-role="none" name="btnHotButton1" class="btn btn-dark btn-block rounded-0" style="height:100%;">
                        <div name="btnHotButtonItem1" style="word-break:break-word;"></div>
                    </button>
                </div>
                <div style="border:1px solid #272a2d; min-height:80px;" class="col-4 HotButtons HotButtonsText bg-dark p-0">
                    <button data-role="none" name="btnHotButton2" class="btn btn-dark btn-block rounded-0" style="height:100%;">
                        <div name="btnHotButtonItem2" style="word-break:break-word;"></div>
                    </button>
                </div>
            </div>
        </div>
         
        <div id="divHotButtonsSection">
            <div name="divHotButtonsRowNext">
             </div>
        </div>        

    <br />
       <div data-role="page" id="pageTemplate" style=" display: none;">
            <div data-role="content" >

                    <div>
                        <ul data-role="listview" id="lstOrgHotButtonContainer" data-inset="true">   
				            <li data-role="list-divider">Select a Template </li>
                            <li>
                                <form id="frmHotButtonSearch">
                                    <div><input data-role="none" id="txtHotButtonSearch" type="search" value="" /></div>
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
      <div data-role="popup" id="popupPOSModeHotButtons" data-theme="b" class="ui-corner-all" style="display: none;">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
			<center><h3>POS Mode</h3></center>
            <div data-role="content">
                <div id="divPOSModeHotButtons">
                </div>
           </div>
       </div>

        <div data-role="popup" id="popupMoreOptionsHotButtons" data-theme="b" class="ui-corner-all" style="width:auto; display:none;">
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
                        @*<select id="selPriceLevel">
                        </select>*@
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

        <div id="popupDecimalInput" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content" id="divQty">
                    <div class="modal-header">
                        <h5 class="modal-title" id="hdrQty"></h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <span class="mb-3"></span>Quantity:
                        <input data-role="none" id="txtDecimalQty" class="form-control mb-3" type="text" maxlength="15" style="text-align: center; " />
                        <span id="spanErrorQty" style="display:none; color: red; font-size:small;">Please enter a valid quantity</span>
                    </div>
                    <div class="modal-footer">
                        <button data-role="none" type="button" id="btnSaveQty" class="btn btn-success btn-block">Save</button>
                        <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>