@Code
    ViewData("Title") = "Sales Returns Settings"
    Layout = "~/Views/Shared/Mobiz.vbhtml"
End Code
<script>
    require([GetPathFormat('/Scripts3/SalesReturnsSettings.js?crc=')]);
</script>
<h1>
    Sales Returns Settings</h1>
<div data-role="fieldcontain" style="display:none">
    <label for="txtRestockingFeePercent">
        Restock Fee %:</label>
    <input id="txtRestockingFeePercent" autofocus />
</div>
<div data-role="fieldcontain">
    <label for="txtMaximumReturnablePeriod">
        Maximum Returnable Period (days):</label>
    <input id="txtMaximumReturnablePeriod" />
</div>
<div data-role="fieldcontain">
    <label for="cbReturnedPrice">
        When returned item has different price points (due to price breaks or volume pricing), follow:</label>
    <select id="cbReturnedPrice">
        <option value="invalid">Please choose one</option>
        <option>Average Price</option>
        <option>Minimum Price</option>
    </select>
</div>
<div data-role="fieldcontain">
    <label for="cbSRVUse">
        Are SRVs 1-time use only or would you allow customers to use SRVs multiple times (as long as said SRV have some remaining value)?					
    </label>
    <select id="cbSRVUse">
        <option value="invalid">Please choose one</option>
        <option>1-Time Use of SRV</option>
        <option>Multiple Use of SRV</option>
    </select>
</div>
<div data-role="fieldcontain">
    <label for="cbPrintRefundSignUp">
        Print out a refund sign-up slip every time a refund transaction is made?:</label>
    <select id="cbPrintRefundSignUp">
        <option value="invalid">Please choose one</option>
        <option value="1">Yes</option>
        <option value="0">No</option>
    </select>
</div>
<button id="butSave">Save</button>