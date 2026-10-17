<script>
    require([GetPathFormat('/Scripts/sources/SecurityRole/SecurityRolesUsers.js?v=' + assetversion),
                 GetPathFormat('/Scripts3/Browser/LibraryPeople.js?crc=')
        ]);
</script>

<div id="pageSecurityRoleUsers">
    <div class="row mb-4 align-items-center">
        <div class="col-6">
            <h1 class="display-6">User List</h1>
        </div>
        <div class="col-6 text-right">
            <button id="btnSecRoleUserAddUser" data-role="none" class="btn btn-success">Add User</button>
        </div>
    </div>

    <div class="mb-3" id="divSecRoleUserRecordCount"></div>
    
    <div data-role="none" id="lstSecRoleUser" class="mb-3 list-group"></div>
    
    <div class="text-center">
        <a id="btnSecRoleUserDone" data-role="none" class="btn btn-secondary btn-lg">Done</a>
    </div>
</div>