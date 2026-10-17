<script>
    require([GetPathFormat('/Scripts/sources/Organization/OrganizationInfo.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryCities.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryCountries.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryOrganizationCategory.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryIndustryClassification.js?crc=')
        ]);
</script>

<div id="pageOrganizationInfo" style="display:block;">

    <div id="popupOrgLogo" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="divAddProductTitle">Organization Logo</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <input type="file" id="fileUploadOrgLogo" accept="image/*" style="visibility:hidden; display:none;" data-role="none" />
                    <img id="imgTempOrgLogo" style="display:none;" />
                    <b>NOTE:</b> Please make that the picture file you are uploading conform to these specifications:
                    <br />
                    <ul>
                        <li>File Type: PNG</li>
                        <li>Recommended Background color: Transparent</li>
                        <li>Dimensions: 170 x 70</li>
                        <li>Max File Size: 500 KB</li>
                    </ul>
                </div>
                <div class="modal-footer">
                    <button data-role="none" type="button" id="btnUploadOrgLogo" class="btn btn-success btn-block">Upload Logo</button>
                    <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <h1 class="display-6 mb-3">Organization Information</h1>

    <div data-role="content" >

        <div style="display:none;">
	        <input data-role="none" type="text" readonly id="txtOrgID" class="form-control"/>                
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3" for="">
                Choose a customer type:
            </label>
            <div class="col-sm-10">
                <div class="form-check form-check-inline">
                    <input class="form-check-input" type="radio" name="rdoOrgType" id="rdoOrgOrganization" value="1">
                    <label class="form-check-label" for="rdoOrgOrganization">Organization</label>
                </div>
                <div class="form-check form-check-inline">
                    <input class="form-check-input" type="radio" name="rdoOrgType" id="rdoOrgPerson" value="0">
                    <label class="form-check-label" for="rdoOrgPerson">Person</label>
                </div>
            </div>
        </div>

        <div id="divOrgOrganization">
            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtOrgCorporateName">
                    Corporate Name: <span style="color: #FF0000;">*</span>
                </label>
                <div class="col-sm-10">
                    <input data-role="none" class="form-control" type="text" id="txtOrgCorporateName" />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtOrgCorporateNameInitial">
                    Corporate Name Initials: <span style="color: #FF0000;">*</span>
                </label>
                <div class="col-sm-10">
                    <input data-role="none" class="form-control" type="text" id="txtOrgCorporateNameInitial" />
                </div>
            </div>            

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="lstOrgTradeName">Organization Logo:</label>
                <div class="col-sm-8">
                    <img data-role="none" id="imgOrgLogo" />
                </div>
                <div class="col-sm-2">
                    <button data-role="none" id="btnAddOrgLogo" class="btn btn-success btn-block mb-2">Add Logo</button>
                    <button data-role="none" id="btnRemoveOrgLogo" style="display:none;" class="btn btn-danger btn-block mb-3">Remove Logo</button>
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="lstOrgTradeName">Trade Name List:</label>
                <div class="col-sm-8">
                    <ul class="list-group" id="lstOrgTradeName"></ul>
                </div>
                <div class="col-sm-2">
                    <button data-role="none" id="btnAddOrgTradeName" class="btn btn-success btn-block">Add Business or Trade Name</button>
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="selOrgOwnershipType">
                    Ownership Type:
                </label>
                <div class="col-sm-10">
                    <select id="selOrgOwnershipType" data-role="none" class="form-select">
                        <option value="">Please Select</option>
                        <option value="Sole Proprietorship">Sole Proprietorship</option>
                        <option value="Partnership">Partnership</option>
                        <option value="Corporation">Corporation</option>
                        <option value="Cooperative">Cooperative</option>
                        <option value="GOCC">GOCC</option>
                        <option value="Government Institution">Government Institution</option>
                    </select>
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="selOrgRegistrationType">
                    Registration Type: <span style="color: #FF0000;">*</span>
                </label>
                <div class="col-sm-10">
                    <select id="selOrgRegistrationType" data-role="none" class="form-select">
                        <option value="">Please Select</option>
                        <option value="SEC">SEC</option>
                        <option value="DTI">DTI</option>
                        <option value="CDA">CDA</option>
                        <option value="DOLE">DOLE</option>
                        <option value="Unregistered">Unregistered</option>
                    </select>
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtOrgRegistrationNo">
                    Registration No.: <span style="color: #FF0000;">*</span>
                </label>
                <div class="col-sm-10">
                    <input data-role="none" class="form-control" type="text" id="txtOrgRegistrationNo" />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="selOrgRegistrationDateMonth">
                    Registrion Date: <span style="color: #FF0000;">*</span>
                </label>
                <div class="col-sm-1">
                    <select data-role="none" class="form-select" id="selOrgRegistrationDateMonth">
                        <option value="">Month</option>
                    </select>
                </div>
                <div class="col-sm-1">
                    <select data-role="none" class="form-select" id="selOrgRegistrationDateDay">
                        <option value="">Day</option>
                    </select>
                </div>
                <div class="col-sm-1">
                    <select data-role="none" class="form-select" id="selOrgRegistrationDateYear">
                        <option value="">Year</option>
                    </select>
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtOrgCorporateTIN">
                    Corporate Tax Identification No. (TIN): <span style="color: #FF0000;">*</span>
                </label>
                <div class="col-sm-10">
                    <input data-role="none" class="form-control" type="text" id="txtOrgCorporateTIN" />
                </div>
            </div>

            <div>
                <h4 class="mb-3">Complete Principlal Office Address</h4>

                <div class="row mb-3">
                    <label class="col-sm-2 mb-3 col-form-label" for="txtOrgOfficeAddressBuildingNo">House or Building No.:</label>
                    <div class="col-sm-10">
                        <input type="text" data-role="none" class="form-control" id="txtOrgOfficeAddressBuildingNo" readonly />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 mb-3 col-form-label" for="txtOrgOfficeAddressStreet">Street:</label>
                    <div class="col-sm-10">
                        <input type="text" data-role="none" class="form-control" id="txtOrgOfficeAddressStreet" readonly />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 mb-3 col-form-label" for="txtSumBusinessAddressCity">City or Town:</label>
                    <div class="col-sm-10">
                        <div style="display:none;"><input data-role="none" class="form-control" type="text" readonly id="txtOrgOfficeAddressCityID" /></div>
                        <button id="btnOrgOfficeAddressCity" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 mb-3 col-form-label" for="btnOrgOfficeAddressCountry">Country:</label>
                    <div class="col-sm-10">
                        <div style="display:none;"><input data-role="none" class="form-control" type="text" readonly id="txtOrgOfficeAddressCountryID" /></div>
                        <button id="btnOrgOfficeAddressCountry" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
                    </div>
                </div>
            </div>

            <div>
                <h4 class="mb-3">Complete Business Address</h4>

                <table class="mb-3" style="width:100%;">
                    <tr>
                        <td style="width:10px; padding-right:10px;"><input data-role="none" class="form-check-input" type="checkbox" id="chkOrgSameAsPricipalAddress"></td>
                        <td>
                            <label data-role="none" class="form-check-label col-11" for="chkOrgSameAsPricipalAddress">
                                Same as Principal Address
                            </label>
                        </td>
                    </tr>
                </table>

                <div class="row mb-3">
                    <label class="col-sm-2 mb-3 col-form-label" for="txtOrgBusinessAddressBuildingNo">House or Building No.:</label>
                    <div class="col-sm-10">
                        <input type="text" data-role="none" class="form-control" id="txtOrgBusinessAddressBuildingNo" readonly />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 mb-3 col-form-label" for="txtOrgBusinessAddressStreet">Street:</label>
                    <div class="col-sm-10">
                        <input type="text" data-role="none" class="form-control" id="txtOrgBusinessAddressStreet" readonly />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 mb-3 col-form-label" for="txtSumBusinessAddressCity">City or Town:</label>
                    <div class="col-sm-10">
                        <div style="display:none;"><input data-role="none" class="form-control" type="text" readonly id="txtOrgBusinessAddressCityID" /></div>
                        <button id="btnOrgBusinessAddressCity" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 mb-3 col-form-label" for="btnOrgOfficeAddressCountry">Country:</label>
                    <div class="col-sm-10">
                        <div style="display:none;"><input data-role="none" class="form-control" type="text" readonly id="btnOrgBusinessAddressCountry" /></div>
                        <button id="btnOrgBusinessAddressCountry" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
                    </div>
                </div>
            </div>            

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtOrgPurpose">Primary Purpose/Activity/Industry Presently Engaged In:</label>
                <div class="col-sm-10">
                    <textarea data-role="none" class="form-control" style="height:100px; resize:none;" id="txtOrgPurpose"></textarea>
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="lstSumOrgFranchisee">Category List:</label>
                <div class="col-sm-8">
                    <ul class="list-group" id="lstOrgCategory"></ul>
                </div>
                <div class="col-sm-2">
                    <button data-role="none" id="btnAddOrgCategory" class="btn btn-success btn-block">Add Category</button>
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="lstSumOrgFranchisee">Industry Classification List:</label>
                <div class="col-sm-8">
                    <ul class="list-group" id="lstOrgIndustryClassification"></ul>
                </div>
                <div class="col-sm-2">
                    <button data-role="none" id="btnAddOrgIndustryClassification" class="btn btn-success btn-block">Add Industry Classification</button>
                </div>
            </div>

            <table class="mb-3" style="width:100%;">
                <tr>
                    <td style="width:10px; padding-right:10px;"><input data-role="none" class="form-check-input" type="checkbox" id="chkOrgIsFranchisor"></td>
                    <td>
                        <label data-role="none" class="form-check-label col-11" for="chkOrgIsFranchisor">
                            Are you a franchisor?
                        </label>
                    </td>
                </tr>
            </table>

            <div id="divFranchiseeContainer" class="mb-3" style="display:none;">
               @* <ul data-role="listview" id="lstOrgFranchiseeContainer" data-inset="true">   
				    <li data-role="list-divider">Franchisee List</li>
                    <li>
                        <div data-role="fieldcontain"><a id="btnAddOrgFranchisee" data-role="button" data-icon="plus" data-inline="true">Add Franchisee</a></div>
                        <div>
                            <ul data-role="listview" id="lstOrgFranchisee" data-inset="true" data-split-icon="delete" data-split-theme="d">   
                            </ul>
                        </div>
                    </li>
                </ul>*@
                <div id="divFranchisorToBeApproved"><br />
                    <strong>Note:</strong> A request will be sent to Mobiz for approval.
                </div> 
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="selOrgBusinessScope">Primary Purpose/Activity/Industry Presently Engaged In:</label>
                <div class="col-sm-10">
                    <select id="selOrgBusinessScope" data-role="none" class="form-select">
                        <option value="">Please Select</option>
                        <option value="Multinational">Multinational</option>
                        <option value="National">National</option>
                        <option value="Regional">Regional</option>
                        <option value="City or Municipality">City or Municipality</option>
                        <option value="Barangay">Barangay</option>
                    </select>
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtOrgNoOfRetailOutlet">No. of Retail Outlets:</label>
                <div class="col-sm-10">
                    <input type="number" data-role="none" class="form-control" id="txtOrgNoOfRetailOutlet" />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtOrgNoOfOffice">No. of Offices:</label>
                <div class="col-sm-10">
                    <input type="number" data-role="none" class="form-control" id="txtOrgNoOfOffice" />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtOrgNoOfWarehouse">No. of Warehouses:</label>
                <div class="col-sm-10">
                    <input type="number" data-role="none" class="form-control" id="txtOrgNoOfWarehouse" />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtOrgNoOfEmployees">No. of Employees:</label>
                <div class="col-sm-10">
                    <input type="number" data-role="none" class="form-control" id="txtOrgNoOfEmployees" />
                </div>
            </div>
        </div>

        <div id="divOrgPerson" style="display:none;">

            <div style="display:none;"><input data-role="none" class="form-control" type="text" readonly id="txtOrgTitleID"/></div>
            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="btnOrgTitle">Title:</label>
                <div class="col-sm-10">
                    <button id="btnOrgTitle" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtOrgSurname">Surname (Last Name): <span style="color: #FF0000;">*</span></label>
                <div class="col-sm-10">
                    <input data-role="none" class="form-control" type="text" id="txtOrgSurname" />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtOrgFirstName">First Name: <span style="color: #FF0000;">*</span></label>
                <div class="col-sm-10">
                    <input data-role="none" class="form-control" type="text" id="txtOrgFirstName" />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtOrgMiddleName">Full Middle Name: <span style="color: #FF0000;">*</span></label>
                <div class="col-sm-10">
                    <input data-role="none" class="form-control" type="text" id="txtOrgMiddleName" />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="selOrgPersonalSuffixName">Generational Suffix (Name):</label>
                <div class="col-sm-10">
                    <select id="selOrgPersonalSuffixName" data-role="none" class="form-select">
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
                <label class="col-sm-2 mb-3 col-form-label" for="selOrgDOBMonth">
                    Date Of Birth: <span style="color: #FF0000;">*</span>
                </label>
                <div class="col-sm-1">
                    <select data-role="none" class="form-select" id="selOrgDOBMonth">
                        <option value="">Month</option>
                    </select>
                </div>
                <div class="col-sm-1">
                    <select data-role="none" class="form-select" id="selOrgDOBDay">
                        <option value="">Day</option>
                    </select>
                </div>
                <div class="col-sm-1">
                    <select data-role="none" class="form-select" id="selOrgDOBYear">
                        <option value="">Year</option>
                    </select>
                </div>
            </div>

            <div>
                <h4 class="mb-3">Mother's Maiden Name</h4>

                <div class="row mb-3">
                    <label class="col-sm-2 mb-3 col-form-label" for="txtOrgMotherMaidenSurname">Maiden Surname (Last Name): <span style="color: #FF0000;">*</span></label>
                    <div class="col-sm-10">
                        <input data-role="none" class="form-control" type="text" id="txtOrgMotherMaidenSurname" />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 mb-3 col-form-label" for="txtOrgMotherMaidenFirstName">First Name: <span style="color: #FF0000;">*</span></label>
                    <div class="col-sm-10">
                        <input data-role="none" class="form-control" type="text" id="txtOrgMotherMaidenFirstName" />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 mb-3 col-form-label" for="txtOrgMotherMaidenMiddleName">Maiden Middle Name:</label>
                    <div class="col-sm-10">
                        <input data-role="none" class="form-control" type="text" id="txtOrgMotherMaidenMiddleName" />
                    </div>
                </div>
            </div>

            <div>
                <h4 class="mb-3">Complete Address</h4>

                <div class="row mb-3">
                    <label class="col-sm-2 mb-3 col-form-label" for="txtOrgPersonAddressBuildingNo">House or Building No.:</label>
                    <div class="col-sm-10">
                        <input type="text" data-role="none" class="form-control" id="txtOrgPersonAddressBuildingNo" readonly />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 mb-3 col-form-label" for="txtOrgPersonAddressStreet">Street:</label>
                    <div class="col-sm-10">
                        <input type="text" data-role="none" class="form-control" id="txtOrgPersonAddressStreet" readonly />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 mb-3 col-form-label" for="btnOrgPersonAddressCity">City or Town:</label>
                    <div class="col-sm-10">
                        <div style="display:none;"><input data-role="none" class="form-control" type="text" readonly id="txtOrgPersonAddressCityID" /></div>
                        <button id="btnOrgPersonAddressCity" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-2 mb-3 col-form-label" for="btnOrgPersonAddressCountry">Country:</label>
                    <div class="col-sm-10">
                        <div style="display:none;"><input data-role="none" class="form-control" type="text" readonly id="txtOrgPersonAddressCountryID" /></div>
                        <button id="btnOrgPersonAddressCountry" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
                    </div>
                </div>
            </div>
        </div>

        <div class="mb-3" style="text-align: center">
            <a id="btnOrgInfoCancel" data-role="none" class="btn btn-secondary btn-lg mr-3">Cancel</a>
            <a id="btnOrgInfoDone" data-role="none" class="btn btn-success btn-lg">Save</a>
        </div>

    </div>

</div>

<div id="pageOrgTradeName" style="display:none;">

    @*<div data-role="header">
        <h1>Trade Name</h1>
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
    </div>*@

    <h1 class="display-6 mb-3">Trade Name</h1>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input data-role="none" class="form-control" type="text" readonly id="txtOrgTradeNameID"/>                
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtOrgBusinessTradeName">Business or Trade Name: <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtOrgBusinessTradeName" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtOrgBusinessTradeNameInitial">Business or Trade Name Initials: <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtOrgBusinessTradeNameInitial" />
            </div>
        </div>

        <div class="mb-3" style="text-align: center">
            <a id="btnOrgTradeNameCancel" data-role="none" class="btn btn-secondary btn-lg mr-3">Cancel</a>
            <a id="btnOrgTradeNameRemove" data-role="none" class="btn btn-secondary btn-lg mr-3">Remove</a>
            <a id="btnOrgTradeNameSave" data-role="none" class="btn btn-success btn-lg">Save</a>
        </div>
    </div>
</div>
