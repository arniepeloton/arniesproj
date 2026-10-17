<script>
    require([GetPathFormat('/Scripts/sources/Organization/OrganizationOwner.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryPeople.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryJobOccupation.js?crc=')
        ]);
</script>

<div id="pageOwnerList" style="display:block;">
    <h1 class="display-6 mb-3">Directors, Officers and Stockholders, Partners or Owners</h1>

    <div data-role="content">
        <div class="mb-3 text-right">
            <button data-role="none" id="btnAddOrgOwner" class="btn btn-success">Add New</button>
        </div>

        <div class="mb-3" id="divOrgOwnerRecordCount"></div>

        <div>
            <ul data-role="none" id="lstOrgOwner" class="list-group mb-5">
            </ul>
        </div>

        <div style="text-align: center">
            <a id="btnOrgOwnerDone" data-role="none" class="btn btn-secondary btn-lg">Done</a>
        </div>
    </div>
</div>

<div id="pageOrgOwnerInfo" style="display:none;">
    <h1 class="display-6 mb-3">Directors, Officers and Stockholders, Partners or Owners Info</h1>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" data-role="none" class="form-control" readonly id="txtOrgOwnerID"/>
            <input type="text" data-role="none" class="form-control" readonly id="txtOrgOwnerPeopleID" />
            <input type="text" data-role="none" class="form-control" readonly id="txtOrgOwnerJobOccupationID" />
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="btnOrgOwner">
                Owner: <span style="color: #FF0000;">*</span>
            </label>
            <div class="col-sm-10">
                <button id="btnOrgOwner" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="btnOrgOwnerJobOccupation">
                Position: <span style="color: #FF0000;">*</span>
            </label>
            <div class="col-sm-10">
                <button id="btnOrgOwnerJobOccupation" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtOrgOwnerDepartmentBusinessUnit">
                Department or Business Unit:
            </label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtOrgOwnerDepartmentBusinessUnit" />
            </div>
        </div>

        <div class="form-check mb-3">
            <input class="form-check-input" type="checkbox" id="chkOrgOwnerIsIncorporator">
            <label class="form-check-label" for="chkOrgOwnerIsIncorporator">
                Incorporator
            </label>
        </div>

        <div class="form-check mb-3">
            <input class="form-check-input" type="checkbox" id="chkOrgOwnerIsDirector">
            <label class="form-check-label" for="chkOrgOwnerIsDirector">
                Director
            </label>
        </div>

        <div class="form-check mb-3">
            <input class="form-check-input" type="checkbox" id="chkOrgOwnerIsStockHolder">
            <label class="form-check-label" for="chkOrgOwnerIsStockHolder">
                Stock Holder
            </label>
        </div>

        <div class="form-check mb-3">
            <input class="form-check-input" type="checkbox" id="chkOrgOwnerIsOfficer">
            <label class="form-check-label" for="chkOrgOwnerIsOfficer">
                Officer
            </label>
        </div>

        <div class="form-check mb-3">
            <input class="form-check-input" type="checkbox" id="chkOrgOwnerIsExecutiveCommittee">
            <label class="form-check-label" for="chkOrgOwnerIsExecutiveCommittee">
                Executive Committee
            </label>
        </div>

        <div class="form-check mb-5">
            <input class="form-check-input" type="checkbox" id="chkOrgOwnerIsOwner">
            <label class="form-check-label" for="chkOrgOwnerIsOwner">
                Owner or Proprietor
            </label>
        </div>

        <div style="text-align: center">
            <a id="btnOrgOwnerCancel" data-role="none" class="btn btn-secondary btn-lg mr-3">Cancel</a>
            <a id="btnOrgOwnerRemove" data-role="none" class="btn btn-danger btn-lg mr-3" style="display:none;">Remove</a>
            <a id="btnOrgOwnerSave" data-role="none" class="btn btn-success btn-lg">Save</a>
        </div>
    </div>
</div>