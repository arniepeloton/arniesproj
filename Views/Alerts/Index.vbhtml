@Code
    ViewData("Title") = "Alerts"
    Layout = "~/Views/Shared/Mobiz.vbhtml"
End Code
<script>
    require([GetPathFormat('/Scripts3/Alerts.js?crc=')]);
</script>
<h2>@ViewData("Title")</h2>
<ul data-role="listview">
    <li><a href="/Alerts/NoPrice">No Price</a></li>
</ul>
