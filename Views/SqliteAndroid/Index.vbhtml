@Code
    ViewData("Title") = "Sqlite Android Test"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
<script>
    require([GetPathFormat('/Scripts3/SqliteAndroid.js?crc=')]);
</script>
<div data-role="collapsible" data-collapsed="true" data-theme="b" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d">
    <h2>InsertUpdate Class</h2>
    <textarea id="txtInsertUpdate"></textarea>
    <button id="butInsertUpdate">Execute</button>
</div>
<div data-role="collapsible" data-collapsed="true" data-theme="b" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d">
    <h2>AndroidJs.ExecSql</h2>
    <textarea id="txtExecSql"></textarea>
    <button id="butExecSql">Execute</button>
</div>

<div data-role="collapsible" data-collapsed="false" data-theme="b" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d">
    <h2>Server DB to local DB</h2>
    <button id="butServerToLocal">Execute</button>
</div>

<div data-role="collapsible" data-collapsed="false" data-theme="b" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d">
    <h2>From local DB</h2>
    <button id="butFromLocal">Execute</button>
    <div id="divFromLocal"></div>
</div>