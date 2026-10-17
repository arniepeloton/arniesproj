<script>
    require([GetPathFormat('/Scripts/sources/CRMDashboard/GeneralSettings.js?crc=' + assetversion)
            ]);
</script>

<div class="row">
    <div class="col-md-3">
        Automatically import data whenever new records are made in customer and vendor libraries?
    </div>
    <div class="col-md-9">
        <div class="form-check form-switch">
            <input class="form-check-input" type="checkbox" id="selCRMAutoImport">
        </div>
    </div>
</div>
<br /><hr /><br />
<div>
    Whenever you save a unique CRM record in the CRM module, 
    Mobiz can also save the CRM record in the customer, supplier and/or employee libraries. 
    The benefit of saving a CRM record into other libraries is to have a unified record ID of the 
    company and/or person whatever role it/he/she might take in relation to your organization. 
    This enables the easy consolidation of transactional data of suppliers/customers/employees/contacts 
    who are actually one and the same entity.
</div>
<br />
<div class="row">
    <div class="col-md-3">
        Please put a check mark before the libraries where you would like to automatically save unique CRM records into:
    </div>
    <div class="col-md-9">
        <div class="form-check mb-3">
            <label class="form-check-label">
                <input data-role="none" type="checkbox" id="chbAutoImportToCustomer" class="form-check-input" />
                Customer/ Guest/ Tenant/ Lessee Library
            </label>
        </div>
        <div class="form-check mb-3">
            <label class="form-check-label">
                <input data-role="none" type="checkbox" id="chbAutoImportToSupplier" class="form-check-input" />
                Supplier/ Vendor Library
            </label>
        </div>
    </div>
</div>

<div class="mt-5 text-center">
    <button data-role="none" class="btn btn-secondary btn-lg" id="btnCancelCRMSettings">Cancel</button>
    <button data-role="none" class="btn btn-success btn-lg" id="btnSaveCRMSettings">Save</button>    
</div>