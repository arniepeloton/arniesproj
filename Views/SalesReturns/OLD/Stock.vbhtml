<div data-role="page" id="pageMain">
    
        <div data-role="content" style="padding: 0px;">
<script>
    require(['common', GetPathFormat('/Scripts3/SalesReturnsStock.js?crc=')]);
</script>
<div class="ui-bar-c" style="border: none">
    <div class="ui-grid-a">
        <div class="ui-block-a" style="width: 43%; text-align: center">
            <img src="../../Content/images/box.png" />
        </div>
        <div class="ui-block-b" style="width: 57%">
            <h3>
                SKU #: <span id="spanStockNumber"></span>
                <br />
                <span id="spanStockDescription"></span><a href="#">(MORE INFO)</a>
            </h3>
        </div>
    </div>
    <div class="ui-bar-c" id="divUnit" data-theme="c" style="padding: 10px">
        <div class="ui-grid-b">
            <div class="ui-block-a">
                <span name="spanUnit"></span>
            </div>
            <div class="ui-block-b">
                Php <span name="spanPrice"></span>
            </div>
        </div>
        <div>
            Qty invoiced: <span name="spanQtyInvoiced"></span>
        </div>
        <div>
            Qty returned: <span name="spanQtyReturned"></span>
        </div>
        <div>
            Qty returnable: <span name="spanQtyReturnable"></span>
        </div>
        <div>
            <input name="txtQtyTemplate" type="number" placeholder="RETURN QTY" maxlength="5"/>
        </div>
        <div name="divReason">
            
        </div>
    </div>
    <div style="text-align: center; padding-bottom: 10px">
        <h3>
            Sub Total : PhP <span id="spanSubtotal"></span>
        </h3>
    </div>
</div>
<button id="butUpdateReturns">
    <h3>
        Update Returns Cart</h3>
</button>

     </div>
 </div>