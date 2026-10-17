@Code
    ViewData("Title") = "Planned Purchase Orders"
    Layout = "~/Views/Shared/Drp.vbhtml"
End Code
<script>
    require(['Drp/DrpStocks']);
</script>
<div id="tabDrpPpo">
</div>
<h2>@ViewData("Title")</h2>
<div>
    <form id="formDrpStocksSearch">
    Search:<input id="txtDrpStocksSearch" dojotype="dijit.form.TextBox" style="width:400px" /><button id="butDrpStocksSearch"
        type="submit" data-dojo-type="dijit.form.Button" data-dojo-props="iconClass:'dijitIconConfigure'">
        Search</button>
    </form>
</div>
<div id="divDrpStocksGrid" style="height: 500px; width: 100%">
</div>
