
<div id="divCartDIV">
    <div id="divCartHeaderPOS">
        <div class="ui-grid-solo" >
                @*<div class="ui-block-a">
                    <a href="../People/PeopleMenu"><img src="/Content/images/MobizNewLogo.png" height="48px" width="112px" style="padding-left: 10px;"/></a>
   
                </div>*@
                 <div>
                     <div style="font-size: large;">
                         GT: (Excl. Charges &amp; Discounts)</div>
                </div>
         </div>

            <div style="text-align: right;font-size: 45px">
                <span id="spanCartTotal">0.00</span>
            </div>

            <ul data-role="listview" data-inset="true">
                <li id="pushTemplate">
                    <a class="btnListItem">
                        <div class="ui-grid-b ui-responsive">                            
	                        <div class="ui-block-a" style="width: 25%; text-align:left;">
	                	        <label id="txtQtyUnit" style="font-size:14px;cursor:pointer;">
                                    
                                </label>
	            	        </div>
                            <div class="ui-block-b" style="text-align: left;width: 74%; white-space:nowrap;">
                                <div id="txtOrder" style="padding-top:1%;padding-bottom:1%;cursor:pointer; white-space:initial;">
                                    <label style="font-size:16px;">Your cart is empty</label>
                                </div>
                            </div>
                        </div>
                    </a>
                    <a data-icon="delete" class="btnListDelete" data-iconpos="notext">Delete</a>
                </li>
            </ul>
            <ul data-role="listview" data-inset="true" id="pushSection">
	            
            </ul>
            @*<div style="height:150px; padding: 0px; margin: 0px; cursor:pointer; overflow:hidden" id="pushTemplate" data-role="button" data-corners="false">
	            <div class="ui-grid-b">
	                <div class="ui-block-a" style="text-align: left;width: 65%;">
	                	<label id="txtOrder" style="padding-top:1%;padding-bottom:1%;font-size:20px;cursor:pointer;"/>
	                </div>
	                <div class="ui-block-b" style="width: 30%;">
	                	<label id="txtQtyUnit" style="font-size:14px;cursor:pointer;"/>
	            	</div>
                    <div class="ui-block-c" style="width: 5%; line-height:150px;">
                        <div style="position:absolute;">
                            <a data-role="button" style="margin-top:45px; float:right; position:fixed;" data-icon="delete" data-iconpos="notext">Delete</a>
                        </div>                        
                    </div>
	        	</div>
	        </div>
            <div id="pushSection">
                
            </div>*@
            <br />
      @*  <form id="formSearch" >
            <input id="txtSearch" type="search" placeholder="Search products" class="SearchText" style="height: 40px;padding:0px;" autofocus />
            
        </form>*@
        
     </div>
     <div id="divCartHeaderBOM" style="display:none;">
        <div class="ui-grid-solo" >
                @*<div class="ui-block-a">
                    <a href="../People/PeopleMenu"><img src="/Content/images/MobizNewLogo.png" height="48px" width="112px" style="padding-left: 10px;"/></a>
   
                </div>*@
                 <div class="ui-block-a">
                     <div style="font-size: large;">
                        Product Maker</div>
                </div>
         </div>
   <br />
     </div>
	@*<div data-role="content" id="divCartPanel">
	    <div id="divCartSection">
	        <div id="divCartStock"  style="display: none">
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
                <label name="lblItemTotal">Item total: </label><span name="spanTotal"></span>
            </div>
        </div>

        <div data-role="fieldcontain"  style="display:none"> 
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
	     </div>
	</div>*@
</div>
