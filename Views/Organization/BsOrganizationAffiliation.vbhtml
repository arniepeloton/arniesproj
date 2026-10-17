<script>
    require([GetPathFormat('/Scripts/sources/Organization/OrganizationAffiliation.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryOrganization.js?crc=')
        ]);
</script>

<div id="pageOrgAffiliationInfo" style="display:block;">
    <h1 class="display-6 mb-3">Intercompany Affiliations</h1>

    <div data-role="content">
        <div style="display:none;"><input type="text" data-role="none" class="form-control" readonly id="txtOrgAffHoldingCompanyID" /></div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="selOrgAffHasParentCompany">
                Does this organization have a parent or holding company?
            </label>
            <div class="col-sm-10">
                <select id="selOrgAffHasParentCompany" data-role="none" class="form-select">
                    <option value="0">No</option>
                    <option value="1">Yes</option>
                </select>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="btnOrgAffHoldingCompany">
                Parent or Holding Company:
            </label>
            <div class="col-sm-10">
                <button id="btnOrgAffHoldingCompany" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
            </div>
        </div>

        <div class="mb-5">
            <div class="row mb-3">
                <div class="col-6">
                    <h4>Affiliate or Subsidiary Company</h4>
                </div>
                <div class="col-6 text-right">
                    <button data-role="none" id="btnAddOrgAffiliates" class="btn btn-success">Add New</button>
                </div>
            </div>

            <ul data-role="none" id="lstOrgAffiliates" class="list-group mb-3">
                <li class="list-group-item">No result(s) found</li>
            </ul>
        </div>

        <div style="text-align: center">
            <a id="btnOrgAffiliationDone" data-role="none" class="btn btn-secondary btn-lg">Done</a>
        </div>
    </div>
</div>

<div id="pageOrgAffiliatesInfo" style="display:none;">
    <h1 class="display-6 mb-3">Affiliate Company Info</h1>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" data-role="none" class="form-control" readonly id="txtOrgAffiliateID"/>
            <input type="text" data-role="none" class="form-control" readonly id="txtOrgAffiliateCompanyID" />
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="btnOrgAffiliateCompany">
                Affiliate or Subsidiary Company: <span style="color: #FF0000;">*</span>
            </label>
            <div class="col-sm-10">
                <button id="btnOrgAffiliateCompany" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="btnOrgAffHoldingCompany">
                Remarks
            </label>
            <div class="col-sm-10">
                <textarea data-role="none" class="form-control" id="txtOrgAffiliateRemarks" style="height:100px;"></textarea>
            </div>
        </div>

        <div style="text-align: center">
            <a id="btnOrgAffiliateCancel" data-role="none" class="btn btn-secondary btn-lg mr-3">Cancel</a>
            <a id="btnOrgAffiliateRemove" data-role="none" class="btn btn-danger btn-lg mr-3" style="display:none;">Remove</a>
            <a id="btnOrgAffiliateSave" data-role="none" class="btn btn-success btn-lg">Save</a>
        </div>
    </div>
</div>