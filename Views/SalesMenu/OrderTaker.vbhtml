<div id="divHeaderOrderDetails" data-position="fixed1" style="background: none white; border: none; width:100%; z-index:1000;">        
    @*<h1 id="MainPageTitle">@ViewData("Title")</h1>
    <a href="#" data-icon="arrow-l" id="btnBack">Back</a>*@
    <div class="ui-grid-a" >
        <div class="ui-block-a">
            <div id="btnHomeOrderDetails" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                <div><img alt="home" src="/Content/images/home.png" width="30px" align="center" /><br /></div>
                <span style="font-size:13px;">Home</span>
            </div>
        </div>
        <div class="ui-block-b">
            <div id="btnOrderDetailsSave"style="margin:0px; height: 80px;" data-role="button" data-theme="green" data-corners="false">                    
                <div><img alt="Save" src="/Content/images/save.png" width="25px" align="center" /><br /></div>
                <span style="font-size:13px;">Save</span>
            </div>
        </div>
    </div>

    <div>
        <div style="margin: 0px;" data-role="button" data-theme="b" data-corners="false">
            <div class="lblPosBranch" style="float:left; text-overflow:ellipsis; overflow:hidden; width:40%; text-align:left; white-space:nowrap;">
            </div>
            <div style="float:right">
                <span id="clockOrder"></span>
            </div>
            <div style="clear:both"></div>            
        </div>
    </div>
</div>

<div id="divContentOrderDetails">
    <div data-role="popup" id="panDatesOrderDetails" data-overlay-theme="a" data-theme="a" data-corners="false"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Date Needed</h3></center>
            <div data-role="content" class="ui-content ui-body-d">
                <div class="ui-grid-solo ui-responsive">
                    <div class="ui-block-a" style="padding:7px;">
                        <div class="metro" style="max-width:252px; overflow:auto;">
                            Date: <span id="txtDateNowOrderDetails"></span>
                            <div class="OrderDetailsCalendar calendar"></div>
                        </div>
                    </div>
                </div>
                <br />
                <div class="ui-grid-a ui-responsive">
                    <div class="ui-block-a" style="padding:0px 7px 7px 7px;">
                        <a id="btnCalCancel" class="btn custom-btn-theme-c">
                            Cancel
                        </a>
                    </div>
                    <div class="ui-block-b" style="padding:0px 7px 7px 7px;">
                        <a id="btnCalDone" class="btn custom-btn-theme-g">
                            Done
                        </a>
                    </div>
                </div>
            </div>       
        </div>
    <center>
        <h1 id="orderTakerHeader">Purchase Order Details</h1>
    </center>
    <div style="padding: 0px 20px 20px 20px">
    <form>
        <div data-role="fieldcontain" id="supplierDetails">
            <label for="spanOrderTakerSupplier">
                Supplier: </label>
            <div class="custom-ui-button">
                <button id="spanOrderTakerSupplier" disabled>
                </button>
            </div>            
        </div>
        <div data-role="fieldcontain">
            <label for="spanOrderTakerCustomer">
                Customer's Organization: </label>
            <div class="custom-ui-button">
                <button id="spanOrderTakerCustomer" disabled></button>
            </div>            
        </div>

        <div data-role="fieldcontain">
            <label for="selFOWSTOrderTaker" id="lblSalesOrderFOWST">Purchasing Branch / FOWST <span style="color: #FF0000;" >*</span></label>
            <select id="selFOWSTOrderTaker">
            </select>
        </div>

        <div data-role="fieldcontain" id="divInvoiceTypeCode">
            <label for="selInvoiceTypeCode">Invoice Type</label>
            <select id="selInvoiceTypeCode">
                <option>Please Select Invoice Type</option>
                <option value="CHI-CAS">CHI-CAS</option>
                <option value="TR">TR</option>
            </select>
        </div>

        <div style="display:none">
            <input type="text" data-role="none" id="selDeliveryMonth" />
            <input type="text" data-role="none" id="selDeliveryDay" />
            <input type="text" data-role="none" id="selDeliveryYear" />
        </div>        
        <div data-role="fieldcontain">
            <label for="btnDeliveryDate">Delivery Date (Date Needed): <span style="color: #FF0000;" >*</span></label>
            <div class="custom-ui-button">
                <button id="btnDeliveryDate">-</button>
            </div>
    @*        <label><input type="checkbox" name="chbDeliveryDate" />Follow schedule </label>
            <fieldset id="fsDeliveryMonth" data-role="controlgroup" data-type="horizontal">
                <legend>Delivery Date: <span style="color: #FF0000;" >*</span></legend>
                <select id="selDeliveryMonth">
	                <option value="">MMM</option>
                </select>
                <select id="selDeliveryDay">
	                <option value="">DD</option>
                </select>                
                <select id="selDeliveryYear">
	                <option value="">YYYY</option>
                </select>                           
            </fieldset>*@
        </div>        
        
        <div data-role="fieldcontain">
            <label for="cbOrderTakerDeliveryAddress">
                Delivery Address: <span style="color: #FF0000;" >*</span></label>
            @*<div id="divShowDeliveryAddress" >
                
            </div>*@
            <select id="cbOrderTakerDeliveryAddress">
            </select>
        </div>        
   
        <div data-role="fieldcontain">
            <label for="cbOrderTakerBillingAddress">
                Billing Address: <span style="color: #FF0000;" >*</span></label>
            @*<div id="divShowBillingAddress" >
            </div>*@
            <select id="cbOrderTakerBillingAddress">
            </select>
        </div>

        <div data-role="fieldcontain">
            <label for="txtOrderTakerRemarks">
                        Remarks:</label>
            <textarea id="txtOrderTakerRemarks" cols="25" rows="5" style="height:100px;" maxlength="255"></textarea>
        </div>
        
       @*<br />
        <div data-role="collapsible" data-collapsed="true" data-theme="c" data-content-theme="d"
            data-icon="plus" data-inset="true" id="divCollapse">
            <h2>
                Add New Address</h2>

                <label for="txtDeliveryStreetNo">Street / Building No. <span style="color: #FF0000;" >*</span></label>
                <input type="text" id="txtDeliveryStreetNo" maxlength="250"/>

                <label for="txtDeliveryAddress">Street Name <span style="color: #FF0000;" >*</span></label>
                <input type="text" id="txtDeliveryAddress" maxlength="100"/>
             
                <label for="selDeliveryCity">City or Town <span style="color: #FF0000;" >*</span></label>
                <select id="selDeliveryCity">
                </select>
                  
                <label for="selDeliveryCountry">Country <span style="color: #FF0000;" >*</span></label>
                <select id="selDeliveryCountry">
                </select>
         
                <br />
                <a id="btnDeliveryAddressSave" data-role="button">Save Address</a>
 
        </div>*@
         <br />
        <div id ="divPurchaseOrderCollapsible" data-role="collapsible" data-collapsed="true" data-theme="c" data-content-theme="d"
            data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true" style="display:none;">
            <h2>
                Other Details</h2>

                <div id="divFowst">
           
            <label for="cbOrderTakerBillingAddress">                
         
               @* <a id="btnFOWSTOrderTaker" data-role="button"><label id="lnkFOWSTOrderTaker" style="cursor:pointer;">Select FOWST</label></a>*@
                </div>
                <div id="divPurchaseOrder">
                    <label for="txtPurchaseOrderNo">
                        Purchase Order No:</label>
                    <input type="text" id="txtPurchaseOrderNo" maxlength="20"/>
	                    <label for="selPOMonth">Purchase Order Date:</label>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                                <select id="selPOMonth">
	                                <option value="">MMM</option>
                                </select>
                                <select id="selPODay">
	                                <option value="">DD</option>
                                </select>                
                                <select id="selPOYear">
	                                <option value="">YYYY</option>
                                </select>                           
                        </fieldset>

                   <label for="txtBuyerName">
                        Buyer / Purchaser Name:</label>
                   <input type="text" id="txtBuyerName" maxlength="255"/>
     
                </div>            
                    

   
            </div>

        
         <br />
       
          @*<input type="submit" value="Save"/>*@          
   </form>
            @*<button id="btnCancelOrder">Cancel</button>*@
   </div>
</div>