@Code
    ViewData("Title") = "Security Settings"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/SecuritySettings/SecurityRolesMenu.js?crc=')]);
    </script>
    
End Section

<div id="pageSecurityRoleList" data-role="page">

    <div data-role="header">
        <h1>Security Role List</h1>
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

        <div id="divPIMMeasureSearchNoResults" style="display:none;">
	        <div>
                <div data-role="fieldcontain">
                    <label id="lblPIMMeasureNotFound"></label>
                </div>
            </div>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center;"> <a id="btnAddNewPIMMeasure" data-role="button" style="display:none;">Add New Measurement</a> </div>
	        <div style="text-align:center"> <a id="btnPIMMeasureBackToHome" data-role="button">Back To Home</a> </div>
        </div>

    </div>

</div>







