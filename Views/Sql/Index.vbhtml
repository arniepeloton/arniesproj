@Code
    ViewData("Title") = "SQL"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
@Section head
    <style>
    </style>
End Section
<script>
    require([GetPathFormat('/Scripts3/Sql.js?crc=')]);
</script>
<h2>@ViewData("Title")</h2>

<div data-role="fieldcontain">
    <label for="txtDbConnectPassword">
        SQL Password:
    </label>
    <input id="txtSqlPassword" type="password" autofocus />
</div>

<div data-role="fieldcontain">
    <label for="txtTimeout">
        Timeout (seconds):
    </label>
    <input id="txtTimeout" type="number" />
</div>

<textarea id="txtSql" spellcheck="false" style="max-height:300px;"></textarea>
<button id="butRun">Run</button>
<button id="butInsert">Insert</button>
<div id="divElapsedSeconds">
</div>
<div id="divResult" style="overflow:auto;">
</div>