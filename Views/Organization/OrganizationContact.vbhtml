<script>
    require([GetPathFormat('/Scripts/sources/Organization/OrganizationContact.js?v=' + assetversion),
                 //GetPathFormat('/Scripts3/Browser/LibraryCountries.js?crc='),
                 //GetPathFormat('/Scripts3/Browser/LibraryAreaCodes.js?crc='),
                 //GetPathFormat('/Scripts3/Browser/LibraryTelcoPrefixes.js?crc=')
    ]);
</script>

<div id="pageContactList" style="display:block;">
    <h1 class="display-6 mb-3">Organization Contact</h1>

    <div data-role="content">
        <div class="accordion mt-3">
            <div class="card">
                <div class="card-header p-0">
                    <button class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse" data-target="#orgcontacttelephone">
                        Telephone
                    </button>
                </div>
                <div id="orgcontacttelephone" class="collapse show">
                    <div class="card-body">
                        <div class="mb-3 text-right">
                            <button data-role="none" id="btnOrgContactTelAdd" class="btn btn-success">Add Telephone</button>
                        </div>
                        <ul id="lstOrgContactTel" class="list-group"></ul>
                    </div>
                </div>
            </div>
        </div>

        <div class="accordion mt-3">
            <div class="card">
                <div class="card-header p-0">
                    <button class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse" data-target="#orgcontactfax">
                        Fax
                    </button>
                </div>
                <div id="orgcontactfax" class="collapse show">
                    <div class="card-body">
                        <div class="mb-3 text-right">
                            <button data-role="none" id="btnOrgContactFaxAdd" class="btn btn-success">Add Fax</button>
                        </div>
                        <ul id="lstOrgContactFax" class="list-group"></ul>
                    </div>
                </div>
            </div>
        </div>

        <div class="accordion mt-3">
            <div class="card">
                <div class="card-header p-0">
                    <button class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse" data-target="#orgcontactmobile">
                        Mobile
                    </button>
                </div>
                <div id="orgcontactmobile" class="collapse show">
                    <div class="card-body">
                        <div class="mb-3 text-right">
                            <button data-role="none" id="btnOrgContactMobAdd" class="btn btn-success">Add Mobile</button>
                        </div>
                        <ul id="lstOrgContactMob" class="list-group"></ul>
                    </div>
                </div>
            </div>
        </div>

        <div class="accordion mt-3">
            <div class="card">
                <div class="card-header p-0">
                    <button class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse" data-target="#orgcontactemail">
                        Email
                    </button>
                </div>
                <div id="orgcontactemail" class="collapse show">
                    <div class="card-body">
                        <div class="mb-3 text-right">
                            <button data-role="none" id="btnOrgContactEmailAdd" class="btn btn-success">Add Email</button>
                        </div>
                        <ul id="lstOrgContactEmail" class="list-group"></ul>
                    </div>
                </div>
            </div>
        </div>

        <div class="accordion mt-3">
            <div class="card">
                <div class="card-header p-0">
                    <button class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse" data-target="#orgcontactwebsite">
                        Website
                    </button>
                </div>
                <div id="orgcontactwebsite" class="collapse show">
                    <div class="card-body">
                        <div class="mb-3 text-right">
                            <button data-role="none" id="btnOrgContactWebsiteAdd" class="btn btn-success">Add Website</button>
                        </div>
                        <ul id="lstOrgContactWebsite" class="list-group"></ul>
                    </div>
                </div>
            </div>
        </div>

        <div class="mt-3" style="text-align: center">
            <a id="btnOrgContactDone" data-role="none" class="btn btn-secondary btn-lg">Done</a>
        </div>
    </div>
</div>

<div id="pageOrgContactTelInfo" style="display:none;">
    <h1 class="display-6 mb-3">Contact Telephone Info</h1>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
            <input type="text" data-role="none" class="form-control" readonly id="txtOrgContactTelID" />
            <input type="text" data-role="none" class="form-control" readonly id="txtOrgTelCountryID" />
            <input type="text" data-role="none" class="form-control" readonly id="txtOrgTelAreaCodeID" />
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="btnOrgTelCountryCode">Country Code:</label>
            <div class="col-sm-10">
                <button id="btnOrgTelCountryCode" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="btnOrgTelAreaCode">Area Code:</label>
            <div class="col-sm-10">
                <button id="btnOrgTelAreaCode" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtOrgContactNoTel">Telephone No.: <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <input type="number" data-role="none" class="form-control" id="txtOrgContactNoTel" maxlength="7" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtOrgContactExtensionTel">Extension or Local No.:</label>
            <div class="col-sm-10">
                <input type="number" data-role="none" class="form-control" id="txtOrgContactExtensionTel" maxlength="10" />
            </div>
        </div>

        <div style="text-align: center">
            <a id="btnOrgContactTelCancel" data-role="none" class="btn btn-secondary btn-lg mr-3">Cancel</a>
            <a id="btnOrgContactTelRemove" data-role="none" class="btn btn-danger btn-lg mr-3" style="display:none;">Remove</a>
            <a id="btnOrgContactTelSave" data-role="none" class="btn btn-success btn-lg">Save</a>
        </div>
    </div>
</div>

<div id="pageOrgContactFaxInfo" style="display:none;">
    <h1 class="display-6 mb-3">Contact Fax Info</h1>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
            <input type="text" data-role="none" class="form-control" readonly id="txtOrgContactFaxID" />
            <input type="text" data-role="none" class="form-control" readonly id="txtOrgFaxCountryID" />
            <input type="text" data-role="none" class="form-control" readonly id="txtOrgFaxAreaCodeID" />
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="btnOrgTelCountryCode">Country Code:</label>
            <div class="col-sm-10">
                <button id="btnOrgFaxCountryCode" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="btnOrgTelAreaCode">Area Code:</label>
            <div class="col-sm-10">
                <button id="btnOrgFaxAreaCode" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtOrgContactNoFax">Fax No.: <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <input type="number" data-role="none" class="form-control" id="txtOrgContactNoFax" maxlength="7" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtOrgContactExtensionFax">Extension or Local No.:</label>
            <div class="col-sm-10">
                <input type="number" data-role="none" class="form-control" id="txtOrgContactExtensionFax" maxlength="10" />
            </div>
        </div>

        <div style="text-align: center">
            <a id="btnOrgContactFaxCancel" data-role="none" class="btn btn-secondary btn-lg mr-3">Cancel</a>
            <a id="btnOrgContactFaxRemove" data-role="none" class="btn btn-danger btn-lg mr-3" style="display:none;">Remove</a>
            <a id="btnOrgContactFaxSave" data-role="none" class="btn btn-success btn-lg">Save</a>
        </div>
    </div>
</div>

<div id="pageOrgContactMobileInfo" style="display:none;">
    <h1 class="display-6 mb-3">Contact Mobile Info</h1>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
            <input type="text" data-role="none" class="form-control" readonly id="txtOrgContactMobileID" />
            <input type="text" data-role="none" class="form-control" readonly id="txtOrgMobileCountryID" />
            <input type="text" data-role="none" class="form-control" readonly id="txtOrgTelCoPrefixID" />
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="btnOrgTelCountryCode">Telco Code:</label>
            <div class="col-sm-10">
                <button id="btnOrgMobileTelCoPrefixCode" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="btnOrgTelAreaCode">Area Code:</label>
            <div class="col-sm-10">
                <button id="btnOrgFaxAreaCode" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtOrgContactNoMobile">Mobile No.: <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <input type="number" data-role="none" class="form-control" id="txtOrgContactNoMobile" maxlength="7" />
            </div>
        </div>

        <div style="text-align: center">
            <a id="btnOrgContactMobileCancel" data-role="none" class="btn btn-secondary btn-lg mr-3">Cancel</a>
            <a id="btnOrgContactMobileRemove" data-role="none" class="btn btn-danger btn-lg mr-3" style="display:none;">Remove</a>
            <a id="btnOrgContactMobileSave" data-role="none" class="btn btn-success btn-lg">Save</a>
        </div>
    </div>
</div>

<div id="pageOrgContactEmail" style="display:none;">
    <h1 class="display-6 mb-3">Contact Email Info</h1>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
            <input type="text" readonly id="txtOrgContactEmailID" />
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtOrgContactEmail">Email Address: <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <input type="email" data-role="none" class="form-control" id="txtOrgContactEmail" />
            </div>
        </div>

        <div style="text-align: center">
            <a id="btnOrgContactEmailCancel" data-role="none" class="btn btn-secondary btn-lg mr-3">Cancel</a>
            <a id="btnOrgContactEmailRemove" data-role="none" class="btn btn-danger btn-lg mr-3" style="display:none;">Remove</a>
            <a id="btnOrgContactEmailSave" data-role="none" class="btn btn-success btn-lg">Save</a>
        </div>
    </div>
</div>

<div id="pageOrgContactWebsite" style="display:none;">
    <h1 class="display-6 mb-3">Contact Website Info</h1>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
            <input type="text" readonly id="txtOrgContactWebsiteID" />
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtOrgContactWebsite">Website: <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtOrgContactWebsite" />
            </div>
        </div>

        <div style="text-align: center">
            <a id="btnOrgContactWebCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
            <a id="btnOrgContactWebRemove" data-role="none" class="btn btn-danger btn-lg" style="display:none;">Remove</a>
            <a id="btnOrgContactWebSave" data-role="none" class="btn btn-success btn-lg">Save</a>
        </div>
    </div>
</div>