  <script>
      require([GetPathFormat('/Scripts/sources/PickProducts/PickProducts.js?crc=')]);
</script>
    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
<style>
    table.myTable { 
            border-collapse:collapse; 
            width: 100%;
        }
        .ui-table-reflow.ui-responsive {
            display: table;
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


    </style>
 <div id="pageMain">
     <div>FOWST: <span id="spanPickProductsFowst"></span></div>
        <div>Section: <span id="spanPickProductsSection"></span></div>
        <h1>Order No. <label id="lblOrderNo"></label></h1>
        <h2>Status: <label id="lblStatus" style="color: Green"></label></h2>
         Last modified on <label id="headerLastUpdated"></label>
        <div data-role="content" >
            
@*            <div class="ui-grid-a">
                <div class="ui-block-a" style="width:30%">
                    Sales Representative :
                </div>
                <div class="ui-block-b" style="text-align:right;width:70%">
                    <label id="headerSalesRep"></label>

                </div>
            </div>*@
            <br />
              <div class="ui-grid-a">
                <div class="ui-block-a" style="width:30%">
                    Order Date : 
                </div>
                <div class="ui-block-b" style="text-align:right;width:70%">
                    <label id="headerSODate"></label>
                </div>
            </div>
            <div class="ui-grid-a">
                <div class="ui-block-a" style="width:30%">
                    Delivery Date : 
                </div>
                <div class="ui-block-b" style="text-align:right;width:70%">
                    <label id="headerDeliveryDate"></label>
                </div>
            </div>
            <div class="ui-grid-a">
                <div class="ui-block-a" style="width:30%">
                    Customer : 

                </div>
                <div class="ui-block-b" style="text-align:right;width:70%">
                    <label id="headerClientName">[CustomerName]</label>

                </div>
            </div>
           <div class="ui-grid-a">
                <div class="ui-block-a" style="width:30%">

                    Delivery Address : 
                </div>
                <div class="ui-block-b" style="text-align:right;width:70%">
                    <label id="headerClientDeliveryAddress">[CustomerAddress]</label> 
                </div>
            </div>
                                    
        </div>
            
    <br />
     <div data-role="content" id="divCartPanel">
        <div id="divCartSection" >
        
            <div id="divCartStock" style="display: none">
                <h3 name="spanStockDescription" style="white-space: normal">
                </h3>
                <table class="tableUnits">
                    <tr name="trTemplate">
                        <td name="tdQty">
                        </td>
                        <td name="tdUnit">
                        </td>
                    </tr>
                </table>
                <table class="tableUnits">
                    <tr name="trTemplatePrep">
                        <td name="tdQtyPrep">
                        </td>
                        <td name="tdUnitPrep">
                        </td>
                    </tr>
                </table>
                 <table class="tableUnits">
                    <tr name="trTemplateRemarks">
                        <td name="tdRemarks">
                        </td>
                    </tr>
                </table>
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
        </div>
        <div id="divCartEmpty" class="Empty">
           No items in cart
           <br />
        </div>

    </div>

     <div style="text-align: center">
         <a id="butDone" data-role="button">
             <h3>I'm done picking items for this section</h3>
         </a>
     </div>

    <div style="text-align: center">
        <a id="btnPrint" data-role="button"  style="display:none;">
        <h3>Print picking list for this order</h3>
        </a>
    </div>
     <div style="text-align: center">
        <a id="btnOrderFulfillmentDone" data-role="button" style="display:none;">
        <h3>Save</h3>
        </a>
    </div>
     <div style="text-align: center">
        <a id="btnHomeMain" data-role="button">
        <h3>Go Back to Main</h3>
        </a>
    </div>
    <br /><br />
</div>
   



