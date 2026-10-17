@Code
    ViewData("Title") = "Schema Sync Sender"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
@Section head
End Section
<script>
    require([GetPathFormat('/Scripts3/SchemaSyncSender.js?crc=')]);
</script>
<h2>@ViewData("Title")</h2>
<div data-role="fieldcontain">
    <label for="txtSchemaSyncRemoteIP">
        Remote IP:</label>
    <input id="txtSchemaSyncRemoteIP" />
</div>
<button id="butSchemaSyncSend">Send</button>
<div id="divSchemaSyncLog"></div>

