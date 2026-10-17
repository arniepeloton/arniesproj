@Code
    ViewData("Title") = "Organization Information"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/Organization/OrganizationMenu.js?crc=')]);
    </script>
    
End Section

<div id="pageOrganizationList" data-role="page">

    <div data-role="header">
        <h1>Organization List</h1>
        <a id="btnOrgBackToHomeIcon" data-iconpos="notext" data-icon="home" class="ui-btn-left">Options</a>
    </div>

    <div data-role="content">

        <form id="frmOrgBrowser" style="margin: 0 -7px">
            <div>
                <input id="txtOrgSearch" type="search" placeholder="Search an Organization" value="" />
            </div>
        </form>

        <div id="divOrgSearchResults" style="display:none;">

            <div class="ListViewMarginBottom" id="divOrgRecordCount"></div>

            <ul id="lstOrganization" data-role="listview" data-filter="true">
            </ul>

        </div>

        <div id="divOrgSearchNoResults" style="display:none;">
	        <div>
                <div data-role="fieldcontain">
                    <label id="lblOrgNotFound"></label>
                </div>
            </div>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center;">
                <a id="btnAddNewOrganization" data-role="button" style="display:none;"><h3>Add New Organization</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgBackToHome" data-role="button"><h3>Go Back to Main</h3></a>
            </div>
        </div>

    </div>

</div>







