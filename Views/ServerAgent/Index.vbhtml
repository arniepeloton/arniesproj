@Code
    ViewData("Title") = "Server Agent"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
<script>
    require([GetPathFormat('/Scripts3/ServerAgent.js?crc=')]);
</script>
<div>
    Last response time <span id="spanLastResponse"></span>
</div>
<div>
    Last error: <span id="spanError"></span>
</div>
<div id="divLog">
</div>
