<div data-role="page" id="pageMain">
    
        <div data-role="content" style="padding: 0px;">
<style>
    /*Green button*/
    .ui-btn-up-green
    {
        border: 1px solid #3A7214;
        background: #5BAB25;
        font-weight: bold;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, #7EC54E, #41AB25);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #7EC54E),
            color-stop(1, #41AB25));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#7EC54E', EndColorStr='#41AB25')";
    }
    .ui-btn-up-green a.ui-link-inherit
    {
        color: #fff;
    }
    .ui-btn-hover-green
    {
        border: 1px solid #176E00;
        background: #6EB64B;
        font-weight: bold;
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
    .ui-btn-hover-green a.ui-link-inherit
    {
        color: #fff;
    }
    .ui-btn-down-green
    {
        border: 1px solid #347722;
        background: #7EC54E;
        font-weight: bold;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, #569E39, #7EC54E);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #569E39),
            color-stop(1, #7EC54E));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#569E39', EndColorStr='#7EC54E')";
    }
    .ui-btn-down-green a.ui-link-inherit
    {
        color: #fff;
    }
    .ui-btn-up-green, .ui-btn-hover-green, .ui-btn-down-green
    {
        font-family: Helvetica, Arial, sans-serif;
        text-decoration: none;
    }
    
    /*Gray button*/
    .ui-btn-up-gray
    {
        border: 1px solid #595A57;
        background: #8F908F;
        font-weight: bold;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, #AAABAA, #8D8E8D);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #AAABAA),
            color-stop(1, #8D8E8D));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#AAABAA', EndColorStr='#8D8E8D')";
    }
    .ui-btn-up-gray a.ui-link-inherit
    {
        color: #fff;
    }
    .ui-btn-hover-gray
    {
        border: 1px solid #646464;
        background: #A1A0A0;
        font-weight: bold color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, 
            #B5B8B5, 
            #A1A0A0);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #B5B8B5),
            color-stop(1, #A1A0A0));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#B5B8B5', EndColorStr='#A1A0A0')";
    }
    .ui-btn-hover-gray a.ui-link-inherit
    {
        color: #fff;
    }
    .ui-btn-down-gray
    {
        border: 1px solid #777A77;
        background: #AAABAA;
        font-weight: bold;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, #999B99, #AAABAA);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #999B99),
            color-stop(1, #AAABAA));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#999B99', EndColorStr='#AAABAA')";
    }
    .ui-btn-down-gray a.ui-link-inherit
    {
        color: #fff;
    }
    .ui-btn-up-gray, .ui-btn-hover-gray, .ui-btn-down-gry
    {
        font-family: Helvetica, Arial, sans-serif;
        text-decoration: none;
    }
    
    /* Button */
    .UpdateCartButton .ui-btn-inner
    {
        width: auto;
        height: 65px;
    }
    
    .UpdateCartPlusminusButtonText .ui-btn-text
    {
        font-size: 50px;
        color: White;
    }
</style>
<script>
    require([GetPathFormat('/Scripts3/Stock.js?crc=')]);
</script>
<div data-role="header" data-position="fixed1" style="background: none white; border: none">
    <div class="ui-grid-d">
        <div class="ui-block-a" style="width: 20%;">
            <div id="btnStockInfo" style="margin: 0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                <div style="margin-top:-15px">
                    <img alt="Stock Info" src="/Content/images/Stock_Information.png" width="30px" align="center" /><br />
                    <div style="font-size:13px;">Stock <br /> Info</div>
                </div>                
            </div>
        </div>
        <div class="ui-block-b" style="width: 20%;">
            <div id="btnSalesPerson" style="margin: 0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                <img alt="Sales Rep" src="/Content/images/Sales_Rep.png" width="30px" align="center" /><br />
                <div style="font-size:13px;">Indicate Sales Rep</div>
            </div>
        </div>
        <div class="ui-block-c" style="width: 20%;">
            <div id="btnSerialNumber" style="margin: 0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                <img alt="Enter Serial Number" src="/Content/images/Serial_Number.png" width="30px" align="center" /><br />
                <div style="font-size:13px; margin-top:5px;">Enter Serial Number</div>
            </div>
        </div>
        <div class="ui-block-d" style="width: 20%;">
            <div id="btnInventoryBalances" style="margin: 0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                <img alt="See Inventory Balances" src="/Content/images/Inventory_Balances.png" width="25px" align="center" /><br />
                <div style="font-size:13px;">See Inventory Balances</div>
            </div>
        </div>
        <div class="ui-block-e" style="width: 20%;">
            <div name="butUnitUpdateCart" style="margin: 0px; height: 80px;" data-role="button" data-theme="green" data-corners="false" class="HotButtons">
                <div style="margin-top:-15px">
                    <img alt="Update Cart" src="/Content/images/Update_Cart.png" width="30px" align="center" /><br />
                    <div style="font-size:13px; margin-top:5px;">Update <br /> Cart</div>
                </div>
            </div>
        </div>
    </div>
    <div>
        <div style="margin: 0px;" data-role="button" data-theme="b" data-corners="false">
            <div class="lblPosBranch" style="float:left; text-overflow:ellipsis; overflow:hidden; width:40%; text-align:left; white-space:nowrap;">
            </div>
            <div style="float:right">
                <span id="clockStock"></span>
            </div>
            <div style="clear:both"></div>            
        </div>
    </div>
</div>
<div class="ui-bar-c" style="border: none">
    <br />
    <div class="divTable" style="width:100%; font-size:inherit;">
        <div class="divTableRow">
            <div id="divItemDetailsImage" class="divTableCell" style="width: 120px; padding: 10px;">
                <img src="../Content/images/box.png" id="imgStockInfo" style="width: 100px;" />
            </div>
            <div id="divItemDetailsStockNumberCell" class="divTableCell" style="vertical-align:top; padding:10px;">
                <h3>
                    <div id="divItemDetailsStockNumber">
                        SKU #: <span id="spanStockNumber"></span>
                        <br />
                    </div>
                    <span id="spanStockDescription"></span><a href="#">(MORE INFO)</a>
                </h3>
                <div data-role="fieldcontain" id="divVATExempt" style="display:none;">
                    <label for="selVATExempt">VAT Exempt</label>
                    <select data-role="slider" id="selVATExempt">
                        <option value="0">No</option>
                        <option value="1">Yes</option>
                    </select>
                </div>
            </div>
        </div>
    </div>
    @*<div class="ui-grid-a">
        <div id="divItemDetailsImage" class="ui-block-a" style="width: 43%; text-align: center">
            <img src="../Content/images/box.png" id="imgStockInfo" />
        </div>
        <div id="divItemDetailsStockNumberCell" class="ui-block-b" style="width: 57%">
            
        </div>
    </div>*@
    <div id="divUnit" name1="divUnit" data-theme="c" style="padding: 10px">
        <div id="divStockMinimumCapacity" class="ui-grid-a">
            <div class="ui-block-a">
                Minimum Capacity:
            </div>
            <div class="ui-block-b">
                <span id="spanStockMinimumCapacity">1,234,567</span>
            </div>
        </div>
        <div id="divStockMaximumCapacity" class="ui-grid-a">
            <div class="ui-block-a">
                Maximum Capacity:
            </div>
            <div class="ui-block-b">
                <span id="spanStockMaximumCapcity">1,234,567</span>
            </div>
        </div>
        <div class="ui-grid-a">
            <div class="ui-block-a" style="width: 67%; margin-top: 10px;">
                <div class="ui-grid-a">
                    <div class="ui-block-a" style="margin-top: 30px;">
                        <span name="spanPriceLevel"></span>
                    </div>
                    <div class="ui-block-b">
                        <div name="divPriceLevel">
                        </div>
                    </div>
                </div>
                <div name="divRequestedUnitPrice" class="ui-grid-a" style="display: none">
                    <div class="ui-block-a">
                        <span>Requested Unit Price</span>
                    </div>
                    <div class="ui-block-b">
                        Php <span name="spanRequestedUnitPrice"></span>
                    </div>
                </div>
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <span name="spanUnit"></span>   <span name="spanUnitMeasureID" style="display:none;"></span>
                    </div>
                    <div class="ui-block-b">
                        Php <span name="spanPrice"></span>
                    </div>
                </div>
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <span name="spanUnitQtyLabel"></span>
                    </div>
                    <div class="ui-block-b">
                        <span name="spanUnitQty"></span>
                    </div>
                </div>
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        Price per <span name="spanBaseUnit"></span>
                    </div>
                    <div class="ui-block-b">
                        <span name="spanPricePerBase"></span>
                    </div>
                </div>
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        Savings per <span name="spanBaseUnit"></span>
                    </div>
                    <div class="ui-block-b">
                        <span name="spanSavingsPerBase"></span>
                    </div>
                </div>
                <div name="divTotalSavings" class="ui-grid-a">
                    <div class="ui-block-a">
                        Savings per <span name="spanUnit"></span>
                    </div>
                    <div class="ui-block-b">
                        <span name="spanTotalSavings"></span>
                    </div>
                </div>
                <div name="divSrp" class="ui-grid-a" style="display: none">
                    <div class="ui-block-a">
                        SRP:
                    </div>
                    <div class="ui-block-b">
                        <span name="spanSrp"></span>
                    </div>
                </div>
            </div>
            <div class="ui-block-b" style="width: 33%;">
                <div class="ui-grid-a">
                    <div class="ui-block-a" style="width: 50%;">
                        <input name="txtQtyTemplate" type="number" placeholder="QTY" style="height: 73px;" maxlength="5" />
                    </div>
                    <div class="ui-block-b" style="width: 50%">
                        <div name="butUnitUpdateCart" data-role="button" data-theme="green" data-corners="false" class="UpdateCartButton">
                            <img alt="Update Cart" src="/Content/images/Update_Cart.png" width="30px" align="center" /><br />
                            <div style="font-size:13px; margin-top:5px;">Update <br /> Cart</div>
                        </div>
                    </div>
                </div>
                <div class="ui-grid-a" style="margin-top: -10px;">
                    <div class="ui-block-a">
                        <div name="butUnitMinus" data-role="button" data-theme="gray" data-corners="false" class="UpdateCartButton UpdateCartPlusminusButtonText">
                            <span style="font-size: xx-large;">-</span>
                        </div>
                    </div>
                    <div class="ui-block-b">
                        <div name="butUnitPlus" data-role="button" data-theme="gray" data-corners="false" class="UpdateCartButton UpdateCartPlusminusButtonText">
                            <span style="font-size: xx-large;">+</span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        @*
        <div name="divSalesRepList">
            <div>
                <h4>
                    Specify Sales Representatives</h4>
                <input name="txtSalesRep" />
                <div name="divSalesRep">
                </div>
            </div>
        </div>
*@
        <div data-role="collapsible_clone">
            <h6>
                More</h6>
            <div data-role="collapsible_clone" name="divPriceAdjustments">
                <h6>
                    Request a Price Adjustment</h6>
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        Original Price
                    </div>
                    <div class="ui-block-b">
                        <span name="spanOriginalPrice"></span>
                    </div>
                </div>
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        Last Price Given to This Customer
                    </div>
                    <div class="ui-block-b">
                        <span name="spanLastPrice">none</span>
                    </div>
                </div>
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        Date
                    </div>
                    <div class="ui-block-b">
                        <span name="spanLastPriceDate">none</span>
                    </div>
                </div>
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        Adjust Price To:
                    </div>
                    <div class="ui-block-b">
                        <input name="txtNewPriceTemplate" type="number" />
                    </div>
                </div>
                <button name="butValidateAdjustedPrice">Validate Adjusted Price</button> <span name="spanPriceResult"></span>
            </div>
            @*<div data-role="collapsible_clone">
                <h4>
                    Specify Picking Location(s) for This Item</h4>
                Not yet implemented
            </div>*@
        </div>
    </div>
    <div id="divSubTotal" style="text-align: center; padding-bottom: 10px">
        <h3>
            Sub Total : PhP <span id="spanSubtotal"></span>
        </h3>
    </div>

    <div id="divInventoryCountContainer" style="display: none; margin: 0px 7px 0 7px;">
         <div data-role="fieldcontain" class="ui-radio" style="visibility:hidden; display:none">
            <fieldset data-role="controlgroup">
                <legend> Are you counting good stocks? </legend>

                <input type="radio" name="rdChoiceGood" id="rcChoiceGoodYes" checked="checked"/>
         	    <label for="rcChoiceGoodYes">Yes</label>

         	    <input type="radio" name="rdChoiceGood" id="rcChoiceGoodNo"/>
         	    <label for="rcChoiceGoodNo">No</label>      
            </fieldset>
        </div>
        <div data-role="fieldset" id="dvReasonForBadCount" style="visibility:hidden; display:none">
            <label for="txtBadCountReason">Reason for bad count</label>
            <textarea id="txtBadCountReason"></textarea>
        </div>
        @*<div data-role="fieldcontain" class="ui-radio">
            <fieldset data-role="controlgroup">
                <legend> Do you want to group the count for this SKU by expiration date(s)? </legend>

                <input type="radio" name="rdChoiceExpiration" id="rcChoiceExpirationYes" />
         	    <label for="rcChoiceExpirationYes">Yes</label>

         	    <input type="radio" name="rdChoiceExpiration" id="rcChoiceExpirationNo" checked="checked"/>
         	    <label for="rcChoiceExpirationNo">No</label>
            </fieldset>
        </div>*@

        <br />
        <br />
        <div data-role="collapsible" data-iconpos="right" data-collapsed="false" data-theme="b" style="border: 1px solid; border-top:none;">
            <h3>Input Count</h3>
            
            <div data-role="fieldcontain">
                <label for="selBinsStockViewer"> Shelf/Bin Location: </label>
                <select id="selBinsStockViewer">
                    <option> Select Shelf/Bin </option>
                </select>                    
            </div>

            <div data-role="fieldcontain">
                <label for="txtBinMaxCap"> Max Cap (Qty) for this Shelf/Bin : </label>
                <input type="number" id="txtBinMaxCap" />
            </div>

            <div data-role="fieldcontain">
                <label for="txtBinMinCap"> Min Cap (Qty) for this Shelf/Bin : </label>
                <input type="number" id="txtBinMinCap" />
            </div>

            <hr />

            <h4>Input Observed Counts:</h4>

            <div data-role="fieldcontain">
               <fieldset data-role="controlgroup" data-type="horizontal">
                    <legend><b>&nbsp; &nbsp; Expiration Date </b></legend>
                    <fieldset id="txtExpirationDate" data-role="DateCombo"> </fieldset>
                </fieldset>
            </div>

            <div data-role="fieldcontain">
                <form>
                    <ul data-role="listview" id="lstUnitMeasureFrame" data-inset="true" data-icon="delete">
                        <li>
                            <table id="lstUnitMeasure" style="width:100%">
                    
                            </table>    
                        </li>

                      
                    </ul>
                </form>
            </div>

            <br />

            <div data-role="fieldcontain">	        
	            <div> <a id="btnDoneUnitMeasure" href="#" data-role="button" data-theme="e">Add Observed Counts to Tally</a> </div>
            </div>
        </div>

        <br />
        <br />

         <div data-role="collapsible" data-iconpos="right" data-collapsed="false" data-theme="b" style="border: 1px solid; border-top:none;">
            <h3>Tallies</h3>
                <ul data-role="listview" id="lstCounts" data-inset="true" data-split-icon="delete">
                </ul>
         </div>
          <br />        
        <br />

        <div class="ui-grid-b">
	        <div class="ui-block-a" style="width: 25%;">
                <label for="txtSuggestedOrder" style="font-weight:bold"> Suggested Order: </label>
              </div>
              <div class="ui-block-b" style="width: 50%;">
                        <input type="number" id="txtSuggestedOrder" style="height: 73px;" maxlength="5"/>
                    </div>
	        <div class="ui-block-c" style="width: 25%;">
               @* <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <input type="number" id="txtSuggestedOrder" style="height: 73px;" maxlength="5"/>
                    </div>*@
                    @*<div class="ui-block-b">*@
                        <a href="#" name="butUnitUpdateCart" id="btnUpdateCartTallies" data-role="button" data-theme="green" data-corners="false" class="UpdateCartButton">Update Cart</a>
                    @*</div>*@
                @*</div>*@
            </div>            
        </div>

        <a href="#" id="btnSaveTallies" data-role="button" style="visibility:hidden; display:none"></a> 

       @* <div>
            <
            <div data-role="fieldcontain">
                
                
            </div>
            <label id="lblSuggestedOrderUnitMeasure"> </label>
        </div>*@

        @*<div id="dvSave">
            <a href="#" id="btnUpdateCartTallies" data-role="button" data-theme="green"><h3>Update Cart</h3></a> 
        </div>*@
        <br />
    </div>

    <div id="divProductSalesPersonContainer" style="display: none; margin: 0px 7px 0 7px;">
        <div>
            <input id="txtProductSalesRepSearch" type="search" placeholder="Search a Sales Representative" value="" />
        </div>
        <div id="divProductSalesPerson">
            <ul data-role="listview" id="lstSalesRepContainer" data-inset="true">
                <li data-role="list-divider">Sales Representative List</li>
                <li>
                    <div>
                        <ul data-role="listview" id="lstSalesRep" data-inset="true">
                        </ul>
                    </div>
                </li>
            </ul>
        </div>
        <br />
    </div>
    <div id="divProductSerialContainer" style="display: none; margin: 0px 7px 0 7px;">
        <div>
            <input id="txtProductSerial" type="search" placeholder="Enter Serial Number" value="" />
            <button id="butSerialBarCode">Scan Barcode</button>
        </div>
        <div id="divProductSerial">
            <ul data-role="listview" id="lstProductSerialContainer" data-inset="true">
                <li data-role="list-divider">Product Serial Number(s)</li>
                <li>
                    <div data-role="fieldcontain" id="divProductSerialRecordCount">
                    </div>
                    <div>
                        <ul data-role="listview" id="lstProductSerial" data-inset="true">
                        </ul>
                    </div>
                </li>
            </ul>
        </div>
        <br />
    </div>
    <div id="divProductInventoryBalances" style="display: none; margin: 0px 7px 0 7px;">
        @*<div>
            <input id="txtProductSerial" type="search" placeholder="Enter Serial Number" value="" />
            <button id="butSerialBarCode">Scan Barcode</button>
        </div>*@
        <div id="divProductInventory">
            <ul data-role="listview" id="lstProductInventoryContainer" data-inset="true">
                <li data-role="list-divider">Inventory Balances</li>
                <li>
                    <div data-role="fieldcontain" id="divProductInventoryRecordCount">
                    </div>
                    <div>
                        <ul data-role="listview" id="lstProductInventoryBalances" data-inset="true">
                            <table id="tblInventoryBalance" style="width: 75%">
                            </table>
                        </ul>
                    </div>
                </li>
            </ul>
        </div>
        <br />
    </div>

    <div style="padding:10px;">
        <a href="#" data-role="button" data-rel="back"><h3>Back</h3></a>
    </div>
</div>
<div id="divReportNoPrice">
    <a id="butReportNoPrice" data-role="button">
    <h3>
        Report No Price</h3>
    <p style="font-weight: normal; margin-top: -15px;">
        Report this product to your manager</p>
    </a>
</div>
<div id="divRequestPrice" data-role="popup" data-dismissible="false">
    <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
    <div data-role="content">
        Requested price:
        <div id="divRequestPriceMessage">
        </div>
        <form id="formRequestedPrice">
        <div data-role="fieldcontain">
            <label for="txtRequestedPrice">
                Discounted amount:</label>
            <input id="txtRequestedPrice" type="number"  />
        </div>
        <div data-role="fieldcontain">
            <label for="txtRequestedPriceDiscountPercent">
                Discount percent:</label>
            <input id="txtRequestedPriceDiscountPercent" type="number" />
        </div>
        <div data-role="fieldcontain">
            <label for="cbRequestedPriceBasePrice">
                Base price:</label>
            <select id="cbRequestedPriceBasePrice">
            </select>
        </div>
        <input type="submit" value="Ok" />
        </form>
    </div>
</div>
</div></div>