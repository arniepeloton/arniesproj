<script>
    require([
            GetPathFormat('/Scripts/sources/Organization/OrganizationRepresentative.js?v=' + assetversion),
            //GetPathFormat('/Scripts3/Browser/LibraryCountries.js?crc='),
            //GetPathFormat('/Scripts3/Browser/LibraryAreaCodes.js?crc='),
            //GetPathFormat('/Scripts3/Browser/LibraryTelcoPrefixes.js?crc=')
    ]);
</script>

<div id="pageRepList" style="display:block">
    <h1 class="display-6 mb-3">Organization Representative List</h1>

    <div data-role="content">
        <div class="mb-3 text-right">
            <button data-role="none" id="btnOrgRepAdd" class="btn btn-success">Add Representative</button>
        </div>

        <div class="mb-3" id="divOrgRepRecordCount"></div>
        <div>
            <ul data-role="none" class="list-group mb-5" id="lstOrgRepresentative"></ul>
        </div>

        <div style="text-align: center">
            <a id="btnOrgRepDone" data-role="none" class="btn btn-secondary btn-lg">Done</a>
        </div>
    </div>
</div>

<div id="pageOrgRepContactTelInfo" style="display:none;">
    <h1 class="display-6 mb-3">Contact Telephone Info</h1>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
            <input type="text" data-role="none" class="form-control" readonly id="txtOrgRepContactTelID" />
            <input type="text" data-role="none" class="form-control" readonly id="txtOrgRepTelCountryID" />
            <input type="text" data-role="none" class="form-control" readonly id="txtOrgRepTelAreaCodeID" />
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="btnOrgRepTelCountryCode">Country Code:</label>
            <div class="col-sm-10">
                <button id="btnOrgRepTelCountryCode" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="btnOrgRepTelAreaCode">Area Code:</label>
            <div class="col-sm-10">
                <button id="btnOrgRepTelAreaCode" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtOrgRepContactNoTel">Telephone No.: <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <input type="number" data-role="none" class="form-control" id="txtOrgRepContactNoTel" maxlength="7" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtOrgRepContactExtensionTel">Extension or Local No.: <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <input type="number" data-role="none" class="form-control" id="txtOrgRepContactExtensionTel" maxlength="10" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="selOrgRepContactTypeTel">Contact Type: <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <select id="selOrgRepContactTypeTel" data-role="none" class="form-select">
                    <option value="">Please Select</option>
                    <option value="Work Telephone">Work Telephone</option>
                    <option value="Personal Telephone">Personal Telephone</option>
                    <option value="Third Party Telephone">Third Party Telephone</option>
                </select>
            </div>
        </div>

        <div style="text-align: center">
            <a id="btnOrgRepContactTelCancel" data-role="none" class="btn btn-secondary btn-lg mr-3">Cancel</a>
            <a id="btnOrgRepContactTelRemove" data-role="none" class="btn btn-danger btn-lg mr-3" style="display:none;">Remove</a>
            <a id="btnOrgRepContactTelSave" data-role="none" class="btn btn-success btn-lg">Save</a>
        </div>
    </div>
</div>

<div id="pageOrgRepContactMobileInfo" style="display:none;">
    <h1 class="display-6 mb-3">Contact Mobile Info</h1>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
            <input type="text" data-role="none" class="form-control" readonly id="txtOrgRepContactMobileID" />
            <input type="text" data-role="none" class="form-control" readonly id="txtOrgRepMobileCountryID" />
            <input type="text" data-role="none" class="form-control" readonly id="txtOrgRepTelCoPrefixID" />
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="btnOrgRepTelCountryCode">Country Code:</label>
            <div class="col-sm-10">
                <button id="btnOrgRepMobileCountryCode" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="btnOrgRepTelAreaCode">Telco Code:</label>
            <div class="col-sm-10">
                <button id="btnOrgRepMobileTelCoPrefixCode" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtOrgRepContactNoMobile">Mobile No.: <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <input type="number" data-role="none" class="form-control" id="txtOrgRepContactNoMobile" maxlength="7" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="selOrgRepContactTypeMobile">Mobile No.: <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <select id="selOrgRepContactTypeMobile" data-role="none" class="form-select">
                    <option value="">Please Select</option>
                    <option value="Work Mobile">Work Mobile</option>
                    <option value="Personal Mobile">Personal Mobile</option>
                    <option value="Third Party Mobile">Third Party Mobile</option>
                </select>
            </div>
        </div>

        <div style="text-align: center">
            <a id="btnOrgRepContactMobileCancel" data-role="none" class="btn btn-secondary btn-lg mr-3">Cancel</a>
            <a id="btnOrgRepContactMobileRemove" data-role="none" class="btn btn-danger btn-lg mr-3" style="display:none;">Remove</a>
            <a id="btnOrgRepContactMobileSave" data-role="none" class="btn btn-success btn-lg">Save</a>
        </div>
    </div>
</div>

<div id="pageOrgRepContactEmail" style="display:none;">
    <h1 class="display-6 mb-3">Contact Email Info</h1>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
            <input type="text" data-role="none" class="form-control" readonly id="txtOrgRepContactEmailID" />
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtOrgRepContactEmail">Email Address: <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <input type="email" data-role="none" class="form-control" id="txtOrgRepContactEmail" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="selOrgRepContactTypeEmail">Email Type: <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <select id="selOrgRepContactTypeEmail" data-role="none" class="form-select">
                    <option value="">Please Select</option>
                    <option value="Work Email">Work Email</option>
                    <option value="Personal Email">Personal Email</option>
                    <option value="Third Party Email">Third Party Email</option>
                </select>
            </div>
        </div>

        <div style="text-align: center">
            <a id="btnOrgRepContactEmailCancel" data-role="none" class="btn btn-secondary btn-lg mr-3">Cancel</a>
            <a id="btnOrgRepContactEmailRemove" data-role="none" class="btn btn-danger btn-lg mr-3" style="display:none;">Remove</a>
            <a id="btnOrgRepContactEmailSave" data-role="none" class="btn btn-success btn-lg">Save</a>
        </div>
    </div>
</div>

<div id="pageOrgRepInfo" style="display:none;">
    <h1 class="display-6 mb-3">Organization Representative</h1>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
            <input type="text" data-role="none" class="form-control" readonly id="txtOrgRepID" />
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtOrgRepTitle">Title:</label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtOrgRepTitle" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtOrgRepLastName">Last Name: <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtOrgRepLastName" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtOrgRepFirstName">First Name: <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtOrgRepFirstName" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtOrgRepMiddleName">Middle Name:</label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtOrgRepMiddleName" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="selOrgRepGenSuffixName">Generational Suffix (Name):</label>
            <div class="col-sm-10">
                <select id="selOrgRepGenSuffixName" data-role="none" class="form-select">
                    <option value="">Please Select</option>
                    <option value="Sr.">Senior (Sr.)</option>
                    <option value="Jr.">Junior (Jr.)</option>
                    <option value="II">II</option>
                    <option value="III">III</option>
                    <option value="IV">IV</option>
                    <option value="V">V</option>
                    <option value="VI">VI</option>
                    <option value="VII">VII</option>
                    <option value="VIII">VIII</option>
                    <option value="IX">IX</option>
                    <option value="X">X</option>
                </select>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="selOrgRepIsActive">Is this representative is active?</label>
            <div class="col-sm-10">
                <select id="selOrgRepIsActive" data-role="none" class="form-select">
                    <option value="0">No</option>
                    <option value="1">Yes</option>
                </select>
            </div>
        </div>

        <div class="mb-5">
            <div class="row mb-3">
                <div class="col-6">
                    <h4>Telephone</h4>
                </div>
                <div class="col-6 text-right">
                    <button data-role="none" id="btnOrgRepContactTelAdd" class="btn btn-success">Add Telephone</button>
                </div>
            </div>

            <div class="mb-3" id="divOrgRepRecordCountTel"></div>

            <ul data-role="none" id="lstOrgRepContactTel" class="list-group mb-3">
                <li class="list-group-item">No result(s) found</li>
            </ul>
        </div>

        <div class="mb-5">
            <div class="row mb-3">
                <div class="col-6">
                    <h4>Mobile</h4>
                </div>
                <div class="col-6 text-right">
                    <button data-role="none" id="btnOrgRepContactMobAdd" class="btn btn-success">Add Mobile</button>
                </div>
            </div>

            <div class="mb-3" id="divOrgRepRecordCountMob"></div>

            <ul data-role="none" id="lstOrgRepContactMob" class="list-group mb-3">
                <li class="list-group-item">No result(s) found</li>
            </ul>
        </div>

        <div class="mb-5">
            <div class="row mb-3">
                <div class="col-6">
                    <h4>Email Address</h4>
                </div>
                <div class="col-6 text-right">
                    <button data-role="none" id="btnOrgRepContactEmailAdd" class="btn btn-success">Add Email Address</button>
                </div>
            </div>

            <div class="mb-3" id="divOrgRepRecordCountEmail"></div>

            <ul data-role="none" id="lstOrgRepContactEmail" class="list-group mb-3">
                <li class="list-group-item">No result(s) found</li>
            </ul>
        </div>

        <div style="text-align: center">
            <a id="btnOrgRepCancel" data-role="none" class="btn btn-secondary btn-lg mr-3">Cancel</a>
            <a id="btnOrgRepSave" data-role="none" class="btn btn-success btn-lg">Save</a>
        </div>
    </div>
</div>