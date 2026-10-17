<script>
    require([GetPathFormat('/Scripts/sources/Organization/OrganizationSummary.js?crc=')]);
</script>

<div class="container-fluid" >
    <div>
        <input data-role="none" style="display:none;" type="text" readonly id="txtSumID" />

        <div class="row mb-4">
            <div class="col-6">
                <h1 class="display-6">Company Info</h1>
            </div>
            <div class="col-6 text-right">
                <button data-role="none" id="btnEditSumOrgInfo" class="btn btn-success">Edit Company Info</button>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtSumCorporateName">Name:</label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtSumCorporateName" readonly />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="imgSumOrgLogo">Organization Logo:</label>
            <div class="col-sm-10">
                <img data-role="none" id="imgSumOrgLogo" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="lstSumTradeName">Trade Name List:</label>
            <div class="col-sm-10">
                <ul class="list-group" id="lstSumTradeName"></ul>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtSumOwnershipType">Ownership Type:</label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtSumOwnershipType" readonly />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtSumRegistrationType">Registration Type:</label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtSumRegistrationType" readonly />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtSumRegistrationNo">Registration No.:</label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtSumRegistrationNo" readonly />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtSumRegistrationDate">Registration Date:</label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtSumRegistrationDate" readonly />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtSumTIN">Tax Identification No. (TIN):</label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtSumTIN" readonly />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtSumPurpose">Primary Purpose/Activity/Industry Presently Engaged In:</label>
            <div class="col-sm-10">
                <textarea data-role="none" class="form-control" style="height:100px; resize:none" id="txtSumPurpose" readonly></textarea>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="lstSumCategory">Category:</label>
            <div class="col-sm-10">
                <ul class="list-group" id="lstSumCategory">
                    <li class="list-group-item">No result(s) found</li>
                </ul>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="lstSumIndustryClassification">Industry Classification:</label>
            <div class="col-sm-10">
                <ul class="list-group" id="lstSumIndustryClassification">
                    <li class="list-group-item">No result(s) found</li>
                </ul>
            </div>
        </div>

        <div>
            <h4 class="mb-3">Complete Principal Office Address</h4>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtSumOfficeAddressBuildingNo">House or Building No.:</label>
                <div class="col-sm-10">
                    <input type="text" data-role="none" class="form-control" id="txtSumOfficeAddressBuildingNo" readonly />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtSumOfficeAddressStreet">Street:</label>
                <div class="col-sm-10">
                    <input type="text" data-role="none" class="form-control" id="txtSumOfficeAddressStreet" readonly />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtSumOfficeAddressCity">City or Town:</label>
                <div class="col-sm-10">
                    <input type="text" data-role="none" class="form-control" id="txtSumOfficeAddressCity" readonly />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtSumOfficeAddressCountry">Country:</label>
                <div class="col-sm-10">
                    <input type="text" data-role="none" class="form-control" id="txtSumOfficeAddressCountry" readonly />
                </div>
            </div>
        </div>
            
        <div>
            <h4 class="mb-3">Complete Business Address</h4>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtSumBusinessAddressBuildingNo">House or Building No.:</label>
                <div class="col-sm-10">
                    <input type="text" data-role="none" class="form-control" id="txtSumBusinessAddressBuildingNo" readonly />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtSumBusinessAddressStreet">Street:</label>
                <div class="col-sm-10">
                    <input type="text" data-role="none" class="form-control" id="txtSumBusinessAddressStreet" readonly />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtSumBusinessAddressCity">City or Town:</label>
                <div class="col-sm-10">
                    <input type="text" data-role="none" class="form-control" id="txtSumBusinessAddressCity" readonly />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtSumBusinessAddressCountry">Country:</label>
                <div class="col-sm-10">
                    <input type="text" data-role="none" class="form-control" id="txtSumBusinessAddressCountry" readonly />
                </div>
            </div>
        </div>

        <table class="mb-3" style="width:100%;">
            <tr>
                <td style="width:10px; padding-right:10px;"><input data-role="none" class="form-check-input" type="checkbox" id="chkSumIsFranchisor"></td>
                <td>
                    <label data-role="none" class="form-check-label col-11" for="chkSumIsFranchisor" style="font-size:25px;">
                        Are you a franchisor?
                    </label>
                </td>
            </tr>
        </table>

        <div class="row mb-3" id="divSumFranchiseeContainer" style="display:none;">
            <label class="col-sm-2 mb-3 col-form-label" for="lstSumOrgFranchisee">Franchisee List:</label>
            <div class="col-sm-10">
                <ul class="list-group" id="lstSumOrgFranchisee">
                    <li class="list-group-item">No result(s) found</li>
                </ul>
            </div>
        </div>

        <div id="divSumOrgOrganizationInfo">
            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtSumBusinessScope">Business Scope:</label>
                <div class="col-sm-10">
                    <input type="text" data-role="none" class="form-control" id="txtSumBusinessScope" readonly />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtSumNoOfRetailOutlet">No. of Retail Outlets:</label>
                <div class="col-sm-10">
                    <input type="text" data-role="none" class="form-control" id="txtSumNoOfRetailOutlet" readonly />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtSumNoOfOffice">No. of Offices:</label>
                <div class="col-sm-10">
                    <input type="text" data-role="none" class="form-control" id="txtSumNoOfOffice" readonly />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtSumNoOfWarehouse">No. of Warehouses:</label>
                <div class="col-sm-10">
                    <input type="text" data-role="none" class="form-control" id="txtSumNoOfWarehouse" readonly />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtSumNoOfEmployees">No. of Employees:</label>
                <div class="col-sm-10">
                    <input type="text" data-role="none" class="form-control" id="txtSumNoOfEmployees" readonly />
                </div>
            </div>
        </div>

        <div id="divSumOrgPersonInfo" style="display:none;">
            <h4 class="mb-3">Complete Address</h4>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtSumOrgPersonBuildingNo">House or Building No.:</label>
                <div class="col-sm-10">
                    <input type="text" data-role="none" class="form-control" id="txtSumOrgPersonBuildingNo" readonly />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtSumOrgPersonAddressStreet">Street:</label>
                <div class="col-sm-10">
                    <input type="text" data-role="none" class="form-control" id="txtSumOrgPersonAddressStreet" readonly />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtSumOrgPersonAddressCity">City or Town:</label>
                <div class="col-sm-10">
                    <input type="text" data-role="none" class="form-control" id="txtSumOrgPersonAddressCity" readonly />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtSumOrgPersonAddressCountry">Country:</label>
                <div class="col-sm-10">
                    <input type="text" data-role="none" class="form-control" id="txtSumOrgPersonAddressCountry" readonly />
                </div>
            </div>
        </div>
    </div>

    <div id="lstOrgSumBrandsDetails">
        <div class="row mb-4 mt-4">
            <div class="col-6">
                <h1 class="display-6">Brands</h1>
            </div>
            <div class="col-6 text-right">
                <button data-role="none" id="btnEditSumOrgBrands" class="btn btn-success">Edit Brands</button>
            </div>
        </div>

        <ul id="lstSumOrganizationBrand" class="list-group mb-3">
            <li class="list-group-item">No result(s) found</li>
        </ul>
    </div>

    <div id="lstFinancialReportInfoHeader">
        <div class="row mb-4 mt-4">
            <div class="col-6">
                <h1 class="display-6">Financial Report Information</h1>
            </div>
            <div class="col-6 text-right">
                <button data-role="none" id="btnEditSumOrgFinancialInfo" class="btn btn-success">Edit Financial Info</button>
            </div>
        </div>

        <div id="lstFinancialReportInfoDetails">
            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtSumFinancialFiscalYearEnd">Fiscal Year End:</label>
                <div class="col-sm-10">
                    <input type="text" data-role="none" class="form-control" id="txtSumFinancialFiscalYearEnd" readonly />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtSumExternalAuditor">Name of External Auditor:</label>
                <div class="col-sm-10">
                    <input type="text" data-role="none" class="form-control" id="txtSumExternalAuditor" readonly />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtSumSingningPartnerExternalAuditor">Name of Signing Partner of External Auditor:</label>
                <div class="col-sm-10">
                    <input type="text" data-role="none" class="form-control" id="txtSumSingningPartnerExternalAuditor" readonly />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtSumSECAccreditationNoOfAuditor">SEC Accreditation Number of External Auditor (If Applicable):</label>
                <div class="col-sm-10">
                    <input type="text" data-role="none" class="form-control" id="txtSumSECAccreditationNoOfAuditor" readonly />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtSumVATType">VAT Type:</label>
                <div class="col-sm-10">
                    <input type="text" data-role="none" class="form-control" id="txtSumVATType" readonly />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtSumBIRTaxGroup">BIR Tax Group:</label>
                <div class="col-sm-10">
                    <input type="text" data-role="none" class="form-control" id="txtSumBIRTaxGroup" readonly />
                </div>
            </div>
        </div>
    </div>

    <div>
        <div id="lstContactInfoHeader" class="row mb-4 mt-4">
            <div class="col-6">
                <h1 class="display-6">Contact Information</h1>
            </div>
            <div class="col-6 text-right">
                <button data-role="none" id="btnEditSumOrgContact" class="btn btn-success">Edit Contacts</button>
            </div>
        </div>

        <ul id="lstSumOrganizationContact" class="list-group mb-3">
            <li class="list-group-item">No result(s) found</li>
        </ul>
    </div>

    <div style="display:none;">
        <div id="lstContactInfoHeader" class="row mb-4 mt-4">
            <div class="col-6">
                <h1 class="display-6">Places List (FOWST)</h1>
            </div>
            <div class="col-6 text-right">
                <button data-role="none" id="btnEditSumOrgFOWST" class="btn btn-success">Edit Places</button>
            </div>
        </div>

        <ul id="lstSumFOWST" class="list-group mb-3">
            <li class="list-group-item">No result(s) found</li>
        </ul>
    </div>

    <div>
        <div id="lstContactInfoHeader" class="row mb-4 mt-4">
            <div class="col-6">
                <h1 class="display-6">Representative List</h1>
            </div>
            <div class="col-6 text-right">
                <button data-role="none" id="btnEditSumOrgRepresentative" class="btn btn-success">Edit List</button>
            </div>
        </div>

        <ul id="lstSumRepresentative" class="list-group mb-3">
            <li class="list-group-item">No result(s) found</li>
        </ul>
    </div>

    <div>
        <div id="lstOwnerInfoHeader" class="row mb-4 mt-4">
            <div class="col-6">
                <h1 class="display-6">Directors, Officers and Stockholders, Partners or Owners</h1>
            </div>
            <div class="col-6 text-right">
                <button data-role="none" id="btnEditSumOrgOwner" class="btn btn-success">Edit List</button>
            </div>
        </div>

        <ul id="lstSumOrganizationOwner" class="list-group mb-3">
            <li class="list-group-item">No result(s) found</li>
        </ul>
    </div>

    <div>
        <div id="lstHoldingCompanyInfoHeader" class="row mb-4 mt-4">
            <div class="col-6">
                <h1 class="display-6">Intercompany Affiliations</h1>
            </div>
            <div class="col-6 text-right">
                <button data-role="none" id="btnEditSumOrgAffiliation" class="btn btn-success">Edit List</button>
            </div>
        </div>

        <div id="lstHoldingCompanyInfoDetails">
            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="selSumOrgHasParentCompany">Does this organization have a parent or holding company?</label>
                <div class="col-sm-10">
                    <select id="selSumOrgHasParentCompany" disabled data-role="none" class="form-select">
                        <option value="0">No</option>
                        <option value="1">Yes</option>
                    </select>
                </div>
            </div>

            <div class="row mb-3" id="divSumOrgAffParentCompanyDetails" style="display:none;">
                <label class="col-sm-2 mb-3 col-form-label" for="txtSumOrgAffHoldingCompany">Parent or Holding Company:</label>
                <div class="col-sm-10">
                    <input type="text" data-role="none" class="form-control" id="txtSumOrgAffHoldingCompany" readonly />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="lstSumOrgAffiliates">Affiliate or Subsidiary Company:</label>
                <div class="col-sm-10">
                    <ul class="list-group" id="lstSumOrgAffiliates">
                        <li class="list-group-item">No result(s) found</li>
                    </ul>
                </div>
            </div>
        </div>        
    </div>

    <div id="lstEmployeeInfoHeader" style="display:none;">
        <div class="row mb-4 mt-4">
            <div class="col-6">
                <h1 class="display-6">Employee List</h1>
            </div>
            <div class="col-6 text-right">
                <button data-role="none" id="btnEditSumOrgEmployee" class="btn btn-success">Edit List</button>
            </div>
        </div>

        <ul id="lstSumEmployee" class="list-group mb-3">
            <li class="list-group-item">No result(s) found</li>
        </ul>
    </div>

    <div id="lstCustomerInfoHeader" style="display:none;">
        <div class="row mb-4 mt-4">
            <div class="col-6">
                <h1 class="display-6">Customer List</h1>
            </div>
            <div class="col-6 text-right">
                <button data-role="none" id="btnEditSumOrgCustomer" class="btn btn-success">Edit List</button>
            </div>
        </div>

        <ul id="lstSumCustomer" class="list-group mb-3">
            <li class="list-group-item">No result(s) found</li>
        </ul>
    </div>

    <div id="lstSupplierInfoHeader" style="display:none;">
        <div class="row mb-4 mt-4">
            <div class="col-6">
                <h1 class="display-6">Supplier List</h1>
            </div>
            <div class="col-6 text-right">
                <button data-role="none" id="btnEditSumOrgSupplier" class="btn btn-success">Edit List</button>
            </div>
        </div>

        <ul id="lstSumSupplier" class="list-group mb-3">
            <li class="list-group-item">No result(s) found</li>
        </ul>
    </div>
    
    <div style="text-align: center">
        <a id="btnOrgSummaryDone" data-role="none" class="btn btn-secondary btn-lg">Done</a>
    </div>
</div>