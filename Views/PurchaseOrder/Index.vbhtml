<script>
    require([GetPathFormat('/Scripts/sources/PurchaseOrder/PurchaseOrder.js?crc=')]);
</script>

<div data-role="content">
    <div data-role="fieldcontain">
        <input data-role="none" type="text" id="txtPurchaseOrderID" style="display:none;" />
        <label for="selPurchaseOrderSlider">Restrict Billing and Delivery address to Purchasing FOWST ?</label>                
        <select data-role="slider" id="selPurchaseOrderSlider">
            <option value="0">No</option>
            <option value="1">Yes</option>                
        </select>
    </div>        
    <br />
    <a id="btnSavePurchaseOrderSettings" data-role="button" href="#"><h3>Save</h3></a>
    <a id="btnCancelPurchaseOrderSettings" data-role="button" href="#"><h3>Cancel</h3></a>                  
</div>