<script>
    require([GetPathFormat('/Scripts/sources/SecurityRole/SecurityRolesSummary.js?v=' + assetversion)]);
</script>

<div id="pageSecurityRole">

    @*<div id"divHeader" data-role="header">
        <h1>Security Role Summary</h1>
        <a href="#" data-icon="arrow-l" id="btnBackSecurityRolesSummary">Back</a>
    </div>*@

    <h1 class="display-6 mb-4">Security Role Summary</h1>

    <div data-role="content" >
        <div data-role="none" style="display:none;">
            <input type="text" readonly data-role="none" class="form-control" id="txtSumSecurityRoleID" />
        </div>

        <div>
            <div class="row mb-4 align-items-center">
                <div class="col-6 col-form-label">
                    <h5 class="mb-3">Security Role</h5>
                </div>
                <div class="col-6 text-right">
                    <button id="btnEditSumSecRoleInfo" data-role="none" class="btn btn-success">Edit Role</button>
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-md-2 col-form-label" for="txtSumRoleCode">Code: <span style="color: #FF0000;">*</span></label>
                <div class="col-md-10">
                    <input type="text" data-role="none" class="form-control" id="txtSumRoleCode" maxlength="15" readonly />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-md-2 col-form-label" for="txtSumRole">Role: <span style="color: #FF0000;">*</span></label>
                <div class="col-md-10">
                    <input type="text" data-role="none" class="form-control" id="txtSumRole" readonly />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-md-2 col-form-label" for="txtSumRoleDescription">Description:</label>
                <div class="col-md-10">
                    <input type="text" data-role="none" class="form-control" id="txtSumRoleDescription" readonly />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-auto col-md-2" for="chkSumRoleIsActive">Active?</label>
                <div class="col col-md-10">
                    <div class="form-check form-switch mb-3" style="font-size:20px;">
                        <input class="form-check-input" data-role="none" disabled type="checkbox" id="chkSumRoleIsActive">
                    </div>
                </div>
            </div>
        </div>

        <div>
            <div class="row mb-4 align-items-center">
                <div class="col-6 col-form-label">
                    <h5 class="mb-3">Security Role User</h5>
                </div>
                <div class="col-6 text-right">
                    <button id="btnEditSumSecRoleUser" data-role="none" class="btn btn-success">Edit Role User</button>
                </div>
            </div>

            <div data-role="none" id="lstSumSecurityRoleUser" class="list-group mb-3"></div>
        </div>

        <div class="mt-4 text-center">
            <a id="btnSecRoleSummaryDone" data-role="none" class="btn btn-secondary btn-lg">Done</a>
        </div>
    </div>
</div>