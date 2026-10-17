@Code
    ViewData("Title") = "No Price Alerts"
    Layout = "~/Views/Shared/Mobiz.vbhtml"
End Code
<script>
    require([GetPathFormat('/Scripts3/AlertsNoPrice.js?crc=')]);
</script>
<button id="butRefresh">Refresh</button>
<div id="divMessage">
</div>
<div id="divTable">
</div>
<button id="butDelete">Delete</button>
