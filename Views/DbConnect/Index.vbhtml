@Code
    ViewData("Title") = "DB Connect"
    Layout = "~/Views/Shared/Mobiz.vbhtml"
End Code
<script>
    require([GetPathFormat('/Scripts3/DbConnect.js?crc=')]);
</script>
<h1>@ViewData("Title")</h1>
<form id="formMain">
<div data-role="fieldcontain">
    <label for="txtDbConnectPassword">
        DB Connect Password:</label>
    <input id="txtDbConnectPassword" type="password" autofocus />
</div>
<div data-role="fieldcontain">
    <label for="txtServer">
        Server:</label>
    <input id="txtServer" />
</div>
<div data-role="fieldcontain">
    <label for="txtDatabase">
        Database:</label>
    <input id="txtDatabase" />
</div>
<div data-role="fieldcontain">
    <label for="chkIntegratedSecurity">
        Integrated Security</label>
    <input id="chkIntegratedSecurity" type="checkbox" />
</div>
<div id="divUser">
    <div data-role="fieldcontain">
        <label for="txtUsername">
            Username:</label>
        <input id="txtUsername" />
    </div>
    <div data-role="fieldcontain">
        <label for="txtPassword">
            Password:</label>
        <input id="txtPassword" type="password" />
    </div>
</div>
<div data-role="fieldcontain">
    <label for="chkSaveAsEcommerceServerSqlServer">
        Save As Ecommerce Server SQL Server
    </label>
    <input id="chkSaveAsEcommerceServerSqlServer" type="checkbox" />
</div>
    <div data-role="fieldcontain">
        <label for="txtWebPosRemoteServerIp">
            Web POS remote server IP:
        </label>
        <input id="txtWebPosRemoteServerIp" />
    </div>
<button type="submit">Save</button>
</form>

<div data-role="collapsible" data-collapsed="false" data-content-theme="d" style="margin-top:30px">
    <h3>Database For Flushing</h3>
    <div id="divFlushingDatabaseLogs">
        Public database name is always Mobiz. If the public database is not present 
    </div>
    <div data-role="fieldcontain">
        <label for="txtFlushingDatabaseServer">
            Server:
        </label>
        <input id="txtFlushingDatabaseServer" />
    </div>
    <div data-role="fieldcontain">
        <label for="txtFlushingDatabaseDatabase">
            Database:
        </label>
        <input id="txtFlushingDatabaseDatabase" readonly value="Mobiz" />
    </div>
    <div data-role="fieldcontain">
        <label for="chkFlushingDatabaseIntegratedSecurity">
            Integrated Security
        </label>
        <input id="chkFlushingDatabaseIntegratedSecurity" type="checkbox" />
    </div>
    <div id="divUser">
        <div data-role="fieldcontain">
            <label for="txtFlushingDatabaseUsername">
                Username:
            </label>
            <input id="txtFlushingDatabaseUsername" />
        </div>
        <div data-role="fieldcontain">
            <label for="txtFlushingDatabasePassword">
                Password:
            </label>
            <input id="txtFlushingDatabasePassword" type="password" />
        </div>
    </div>
    <div data-role="fieldcontain">
        <label for="txtFlushingDatabaseFolder">
            Folder:
        </label>
        <input id="txtFlushingDatabaseFolder" placeholder="Mobiz (public) database will be created here" />
    </div>
    <button id="butSaveFlushingDatabase">Save</button>
</div>

<div data-role="collapsible" data-collapsed="false" data-content-theme="d" style="margin-top:30px">
    <h3>Synonyms</h3>
    <div id="divSynonymLogs">

    </div>
    <div>Private database names (leave blank to auto detect):</div>
    <textarea id="txtPrivateDatabases"></textarea>
    <button id="butSetSynonyms">Set synonyms</button>
</div>

<div data-role="collapsible" data-collapsed="false" data-content-theme="d" style="margin-top:30px">
    <h3>SQL Server Database Backup File Archiver</h3>
    <div id="divBackupArchiver">

    </div>
    <button id="butBackupArchiver">Get files</button>
    <button id="butBackupArchiverExecute">Archive old files</button>
    <button id="butBackupArchiverGetArchives">Get archives</button>
    <button id="butBackupArchiverDeleteArchives">Delete old archives</button>
</div>

<div data-role="collapsible" data-collapsed="false" data-content-theme="d" style="margin-top:30px">
    <h3>Others</h3>
    <div id="divOthersLog">

    </div>
    <button id="butOthersDetachWebPosDb">Detach WebPosDb databases</button>
    <button id="butOthersAttachWebPosDb">Attach WebPosDb databases</button>
</div>