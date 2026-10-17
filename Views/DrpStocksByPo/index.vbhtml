@Code
    ViewData("Title") = "Planned Purchase Orders By Vendor"
    Layout = "~/Views/Shared/Drp.vbhtml"
End Code
<script>
    require(['Drp/DrpStocksByPo']);
</script>
<div id="tabDrpPpo">
</div>
<h2>@ViewData("Title")</h2>
<div>
    <form id="formPpoVendorSearch">
    Search:<input id="txtPpoVendorSearch" dojotype="dijit.form.TextBox" /><button id="butPpoVendorSearch"
        type="submit" data-dojo-type="dijit.form.Button" data-dojo-props="iconClass:'dijitIconConfigure'">
        Search</button>
    </form>
</div>
<div id="divDrpStocksByPo">
</div>
