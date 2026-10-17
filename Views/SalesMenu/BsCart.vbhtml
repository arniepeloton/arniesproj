<style type="text/css">
    .mainsku {
        font-size: 13px;
    }
    
    .addons {
        font-size: 11px;
    }

    .mainsku > td:nth-child(2),
    .addons > td:nth-child(2) {
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
        max-width: 0px;
    }
</style>
<div id="divCartDIV">
    <div id="divCartHeaderPOS">
        <div class="">
            <div class="row g-0 align-items-center font-weight-bold pb-2 pt-2 bg-secondary text-light">
                <div class="text-left pl-2 col" style="font-size:14px;">Gross Total</div>
                <div id="spanCartTotal" class="text-right pr-2 col-auto" style="font-size:18px;">0.00</div>
            </div>
            <div class="table-scrollx">
                <table data-role="none" id="tblSideCartItems" class="table table-borderless table-sm">
                    <thead style="background-color:#ccc;">
                        <tr class="text-center" style="font-size:x-small">
                            <th style="width:30px">Qty</th>
                            <th>Description</th>
                            <th style="width:30px">Price</th>
                            <th style="width:30px">Total</th>
                        </tr>
                    </thead>
                </table>
            </div>
        </div>
        
        <div class="d-none">
            <div class="ui-grid-solo">
                @*<div class="ui-block-a">
                <a href="../People/PeopleMenu"><img src="/Content/images/MobizNewLogo.png" height="48px" width="112px" style="padding-left: 10px;"/></a>

            </div>*@
                <div>
                    <div class="p-2" style="font-size: large;">
                        GT: (Excl. Charges &amp; Discounts)
                    </div>
                </div>
            </div>

            <div class="p-2" style="text-align: right;font-size: 45px">
                <span id="spanCartTotalx">0.00</span>
            </div>

            <ul class="list-group">
                <li id="listEmptyCart">
                    <div style="text-align:center; color: #a9afb5; font-size:25px;">
                        <span class="bi-cart4" style="font-size:100px;"></span>
                        <br />
                        Cart is empty
                    </div>
                </li>

                <li id="pushTemplate" data-role="none" class="list-group-item list-group-item-action" aria-current="true" style="padding:0px !important; display:none;">
                    <table class="btnListItem" style="width:100%;">
                        <tbody>
                            <tr>
                                <td id="txtQtyUnit" style="width:100px; text-align:center; font-weight:bold;"></td>
                                <td id="txtOrder" style="padding:10px;"></td>
                                <td style="width:50px; text-align:center; font-size:20px; color:#FFF;" class="bg-danger btnListDelete">
                                    <span class="bi-trash"></span>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </li>
            </ul>
            <ul class="list-group" id="pushSection"></ul>
        </div>        
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
