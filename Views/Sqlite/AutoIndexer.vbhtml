@Code
    ViewData("Title") = "SQLite Auto Indexer"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
@Section head
    <style>
        
    </style>
End Section
<script>
    require([GetPathFormat('/Scripts3/SqliteAutoIndexer.js?crc=')]);
</script>
<h2>@ViewData("Title")</h2>
<textarea id="txtSql" spellcheck="false"></textarea>
<div id="divElapsedMilliseconds">
</div>
<h3>SQL result:</h3>
<div id="divResult">
</div>

<h3>Analysis with existing indices</h3>
<div id="divExisting"></div>

<h3>Analysis with needed indices</h3>
<div id="divNeeded"></div>

<h3>Recommendations:</h3>
<textarea id="txtRecommendations" spellcheck="false"></textarea>

<label for="chkExecute">Execute query (slow)</label>
<input id="chkExecute" type="checkbox" />
<div id="butAnalyze" data-role="button">
    Analyze<div style="font-size: 10pt; font-weight: normal"></div>
</div>
<div id="butApplyRecommendations" data-role="button">
    Apply Recommendations<div style="font-size: 10pt; font-weight: normal">This will execute the query</div>
</div>