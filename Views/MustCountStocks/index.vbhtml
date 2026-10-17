@Code
    ViewData("Title") = "Must Count Stocks"
    Layout = "~/Views/Shared/Drp.vbhtml"
End Code
<script>
    require(['Drp/MustCountStocks']);
</script>
<h2>@ViewData("Title")</h2>
<button id="butMustCountStocksRefresh" data-dojo-type="dijit.form.Button" data-dojo-props="iconClass:'dijitIconNewTask'">
    Refresh</button>
<span id="spanbutMustCountStocksCache"></span>
<div id="divMustCountStocksGrid" style="height: 400px; width: 100%">
</div>
