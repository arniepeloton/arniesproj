@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/SecurityRole/SecurityRolesUsers.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryPeople.js?crc=')
        ]);
    </script>
    
End Section

<div id="pageSecurityRoleUsers" data-role="page">

    <div id"divHeader" data-role="header">
        <h1>User List</h1>
    </div>

    <div data-role="content" >

        <div>
            <ul data-role="listview" id="lstSecUserListContainer" data-inset="true">   
				<li data-role="list-divider">User List</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnSecRoleUserAddUser" data-role="button" data-icon="plus" data-inline="true">Add User</a></div>
                    <div data-role="fieldcontain" id="divSecRoleUserRecordCount"></div>
                    <div>
                        <ul data-role="listview" id="lstSecRoleUser" data-inset="true" data-split-icon="delete" data-split-theme="d">   
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center;">
                <a id="btnSecRoleUserDone" data-role="button"><h3>Done</h3></a>
            </div>
        </div>

    </div>

</div>




