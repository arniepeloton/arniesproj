@Code
    ViewData("Title") = "Security Settings"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/SecurityRole/SecurityRolesMenu.js?crc=')]);
    </script>
    
End Section

<div id="pageSecurityRoleList" data-role="page">

    @*<div data-role="header">
        <h1>Security Role List</h1>
    </div>*@

     <div data-role="header" data-position="fixed1" style="background: none white; border: none">
        <h1>Security Role List</h1>

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
                <div id="btnSecurityRoles" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>Roles</span>
                </div>
            </div>
        </div>

    </div>

    <div data-role="content">

        <form id="frmSecurityRoleBrowser" style="margin: 0 -7px">
            <div>
                <input id="txtSecurityRoleSearch" type="search" placeholder="Search a Security Role" value="" />
            </div>
        </form>

        <div id="divSecurityRoleSearchResults" style="display:none;">

            <div class="ListViewMarginBottom" id="divSecurityRoleRecordCount"></div>

            <ul id="lstSecurityRole" data-role="listview" data-filter="true">
            </ul>

        </div>

        <div id="divSecurityRoleSearchNoResults" style="display:none;">
	        <div>
                <div data-role="fieldcontain">
                    <label id="lblSecurityRoleNotFound"></label>
                </div>
            </div>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center;">
                <a id="btnAddNewSecurityRole" data-role="button" ><h3>Add New Role</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnSecurityRoleBackToHome" data-role="button"><h3>Go Back to Main</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnSecurityRoleGoToSetupChecklist" data-role="button"><h3>Go to Setup Checklist</h3></a>
            </div>
        </div>

    </div>

</div>







