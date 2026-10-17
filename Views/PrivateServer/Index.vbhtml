@Code
    ViewData("Title") = "Private Server Setup"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
@Section head
    <script>
        require(['/Scripts3/PrivateServer.js?crc=']);
    </script>
End Section
<div data-role="collapsible" data-collapsed="false" data-theme="c" data-content-theme="d" data-collapsed-icon="arrow-r">
    <h2>
        Database Setup</h2>
    <div id="divDatabaseMessage">
        
    </div>
    <form id="formDbConnect">
    <div data-role="fieldcontain">
        <label for="txtServer">
            Server:</label>
        <input id="txtServer" value="(local)" />
    </div>
    <div data-role="fieldcontain">
        <label for="txtDatabase">
            Database:</label>
        <input id="txtDatabase" value="Mobiz" />
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
            <input id="txtUsername" value="sa" />
        </div>
        <div data-role="fieldcontain">
            <label for="txtPassword">
                Password:</label>
            <input id="txtPassword" type="password" />
        </div>
    </div>
    <button type="submit">Save</button>
    </form>
</div>
