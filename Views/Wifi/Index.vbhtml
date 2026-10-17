@Code
    ViewData("Title") = "Wifi"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
<script>
    require([GetPathFormat('/Scripts3/Wifi.js?crc=')]);
</script>
<div data-role="fieldcontain">
    <label>
        <input type="checkbox" id="chkWifiActive" />
        Enable Wifi</label>
</div>
<div id="divWifiList">
</div>
<button id="butRefreshWifiList"><h3>Refresh Wifi List</h3></button>
<a data-role="button" href="#" data-rel="back"><h3>Back</h3></a>
@*<a href="/signin">Sign in</a>*@
@Section OtherPages
    <div id="pageItem" data-role="page">
        <div data-role="header">
            <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
            <h1>
                Wifi Network</h1>
        </div>
        <div data-role="content">
            <div data-role="fieldcontain">
                <label for="txtSsid">
                    SSID:</label>
                <input id="txtSsid" />
            </div>
            <div data-role="fieldcontain">
                <label for="txtPassword">
                    Password:</label>
                <input id="txtPassword" type="password" />
            </div>
            <button id="butSave"><h3>Save and connect</h3></button>
        </div>
    </div>
End Section
