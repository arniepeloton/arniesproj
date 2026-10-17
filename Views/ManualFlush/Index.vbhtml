@Code
    ViewData("Title") = "Manual Flush"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
@Section head

End Section
<script>
    require([GetPathFormat('/Scripts3/ManualFlush_offline.js?crc=')]);
</script>

<h2>@ViewData("Title")</h2>
<h3>Select a client databases to flush</h3>
<div id="divDatabases"></div>