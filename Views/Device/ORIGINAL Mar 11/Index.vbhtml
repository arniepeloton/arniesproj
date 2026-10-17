@Code
    ViewData("Title") = "Authorized Device List"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/Device/DeviceInfoMenu.js?crc=')]);
    </script>
    
End Section

<div id="pageDeviceMenu" data-role="page">
@*    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>*@
     <div data-role="header" data-position="fixed1" style="background: none white; border: none">
        <h1>@ViewData("Title")</h1>

        <div class="ui-grid-d">
            <div class="ui-block-a" style="width: 10%;">
                <div id="btnHome" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>Home</span>
                </div>
            </div>
            <div class="ui-block-b" style="width: 30%;">
                <div id="btnSystemAdmin" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>System Administration</span>
                </div>
            </div>
            <div class="ui-block-c" style="width: 20%;">
                <div id="btnSystemSettings" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>System Settings</span>
                </div>
            </div>
            <div class="ui-block-d" style="width: 20%;">
                <div id="btnSecuritySettings" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>Security Settings</span>
                </div>
            </div>
            <div class="ui-block-e" style="width: 20%;">
                <div id="btnDevice" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>Authorized Device</span>
                </div>
            </div>
        </div>

    </div>

    <div data-role="content">
        
        <form id="frmDeviceSearch">
            <div><input id="txtSearchDevice" type="search" value="" /></div>
        </form>

        <div id="divDeviceSearchResults" style="display:none;">

            <div class="ListViewMarginBottom" id="divDeviceRecordCount"></div>

            <ul id="lstDevices" data-role="listview" data-filter="true">
            </ul>

        </div>

        <div id="divDeviceSearchNoResults" style="display:none;">
	        <div>
                <div data-role="fieldcontain">
                    <label id="lblDeviceNotFound"></label>
                </div>
            </div>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnNewDeviceInfo" data-role="button"><h3>Enroll a Device or Computer</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnDoneDeviceInfo" data-role="button"><h3>Go Back to Main</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnDeviceGoToSetupChecklist" data-role="button"><h3>Go to Setup Checklist</h3></a>
            </div>
        </div>


    </div>

</div>





