@Code
    ViewData("Title") = "SQLite"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
@Section head
    <style>
        
    </style>
End Section
<script>
    require([GetPathFormat('/Scripts3/Sqlite.js?crc=')]);
</script>
<h2>@ViewData("Title")</h2>
<textarea id="txtSql" spellcheck="false" style="max-height:400px"></textarea>
<button id="butRun">Run (CTRL + Q)</button>
<div id="divElapsedSeconds">
</div>
<div id="divResult" style="overflow:auto; max-height:300px;">
</div>
<button id="butRefreshDbObjects2">Refresh Database Objects via Java</button>
<a href="@Url.Action("AutoIndexer")">Auto Indexer</a>
<div id="divDbsContainer" data-role="collapsible-set1" data-theme="d">
    <div data-role="collapsible">
        <h3>
            Database name</h3>
        <ol>
            <li>Tablename
                <ol>
                    <li>fieldname</li>
                    <li>field2name</li>
                </ol>
            </li>
            <li>Table 2 name>
                <ol>
                    <li>fieldname</li>
                    <li>field2name</li>
                </ol>
            </li>
        </ol>
    </div>
    <div data-role="collapsible" data-theme="e" data-content-theme="e">
        <h3>
            Database name</h3>
        <ol>
            <li>Tablename
                <ol>
                    <li>fieldname</li>
                    <li>field2name</li>
                </ol>
            </li>
            <li>Table 2 name>
                <ol>
                    <li>fieldname</li>
                    <li>field2name</li>
                </ol>
            </li>
        </ol>
    </div>
    <div data-role="collapsible">
        <h3>
            Database name</h3>
        <ol>
            <li>Tablename
                <ol>
                    <li>fieldname</li>
                    <li>field2name</li>
                </ol>
            </li>
            <li>Table 2 name>
                <ol>
                    <li>fieldname</li>
                    <li>field2name</li>
                </ol>
            </li>
        </ol>
    </div>
</div>
