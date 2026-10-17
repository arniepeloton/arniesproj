<div id="popupOpportunity" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-fullscreen">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Opportunity of <span id="lblOppClientName"></span></h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    <table style="width:100%; table-layout:fixed;">
                        <thead>
                            <tr>
                                <td></td>
                                <td class="txt-center txt-bold">Won</td>
                                <td class="txt-center txt-bold">Lost</td>
                                <td class="txt-center txt-bold">In Progress</td>
                                <td class="txt-center txt-bold">Totals</td>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>Absolute Amount</td>
                                <td class="txt-center" id="txtAbsAmtWon"></td>
                                <td class="txt-center" id="txtAbsAmtLost"></td>
                                <td class="txt-center" id="txtAbsAmtInProgress"></td>
                                <td class="txt-center" id="txtAbsAmtTotal"></td>
                            </tr>
                            <tr>
                                <td>% Contribution</td>
                                <td class="txt-center" id="txtContributionWon"></td>
                                <td class="txt-center" id="txtContributionLost"></td>
                                <td class="txt-center" id="txtContributionInProgress"></td>
                                <td class="txt-center" id="txtContributionTotal"></td>
                            </tr>
                        </tbody>
                    </table>
                </div>

                <div class="table-responsive">
                    <table id="tblDataTableClientOpportunityPopup" data-role="none" style="display:table;" class="table table-light table-striped table-hover">
                        <thead>
                            <tr class="custom-theme-f text-center">
                                <th style="width:20px !important;"> # </th>
                                <th style="cursor:pointer;"> Name </th>
                                <th style="width:150px !important; cursor:pointer;"> Current Stage </th>
                                <th style="width:100px !important; cursor:pointer;"> Status </th>
                                <th style="width:120px !important; cursor:pointer;"> Size </th>
                                <th style="width:105px !important; cursor:pointer;"> Last Updated </th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody id="tbodyOpportunity"></tbody>
                    </table>
                </div>
            </div>
            <div class="modal-footer">
                <button data-role="none" type="button" class="btn btn-secondary btn-lg" data-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>

<div id="popupNewOpportunity" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">New Opportunity</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div id="divTabTicket" style="white-space:nowrap; overflow:auto;">
                    <ul class="nav nav-tabs mb-3" style="flex-wrap:nowrap;">
                        <li class="nav-item" role="presentation">
                            <button data-role="none" class="nav-link active" data-toggle="tab" data-target="#tabOpportunity">Opportunity</button>
                        </li>
                        <li class="nav-item" role="presentation">
                            <button data-role="none" class="nav-link" id="btnTabOppComment" data-toggle="tab" data-target="#tabOpportunityComment">Comments</button>
                        </li>
                        <li class="nav-item" role="presentation">
                            <button data-role="none" class="nav-link" id="btnTabOppStage" data-toggle="tab" data-target="#tabOpportunityStage">Staging</button>
                        </li>
                    </ul>
                </div>

                <div class="tab-content p-3">
                    <div id="tabOpportunity" class="tab-pane fade show active">
                        <input data-role="none" type="text" id="txtOpportunityID" style="display:none;" />
                        <div id="divOppStatus" class="mb-3 row" style="display:none;">
                            <label for="" class="col-md-3">Status:</label>
                            <div class="col-md">
                                <div class="form-check form-check-inline">
                                    <input data-role="none" class="form-check-input" type="radio" name="radOppStatus" id="radOppNA" value="In Progress" checked>
                                    <label class="form-check-label" for="radOppNA">In Progress</label>
                                </div>

                                <div class="form-check form-check-inline">
                                    <input data-role="none" class="form-check-input" type="radio" name="radOppStatus" id="radOppWin" value="Win">
                                    <label class="form-check-label" for="radOppWin">Win</label>
                                </div>

                                <div class="form-check form-check-inline">
                                    <input data-role="none" class="form-check-input" type="radio" name="radOppStatus" id="radOppLost" value="Lost">
                                    <label class="form-check-label" for="radOppLost">Lost</label>
                                </div>
                            </div>
                        </div>

                        <div class="mb-3 row">
                            <label for="selOppClientName" class="col-form-label col-md-3">Client Name: <span class="required">*</span></label>
                            <div class="col-md">
                                <select id="selOppClientName" data-role="none" class="form-select"></select>
                            </div>
                        </div>

                        <div class="mb-3 row">
                            <label for="txtOpportunityName" class="col-form-label col-md-3">Opportunity Name: <span class="required">*</span></label>
                            <div class="col-md">
                                <input data-role="none" class="form-control" type="text" id="txtOpportunityName" />
                            </div>
                        </div>

                        <div class="mb-3 row">
                            <label for="txtOpportunityDescription" class="col-form-label col-md-3">Opportunity Description:</label>
                            <div class="col-md">
                                <textarea data-role="none" class="form-control" id="txtOpportunityDescription" style="height:100px; max-height:100px; resize:none"></textarea>
                            </div>
                        </div>

                        <div class="mb-3 row">
                            <label for="txtOpportunitySize" class="col-form-label col-md-3">Opportunity Size:</label>
                            <div class="col-md">
                                <input data-role="none" class="form-control" type="text" id="txtOpportunitySize" />
                            </div>
                            <div class="col-md">
                                <input data-role="none" class="form-control" type="text" id="txtOpportunitySizePeriod" placeholder="Period" />
                            </div>
                        </div>

                        @*<h4>Contact Info</h4>*@

                        <div class="accordion mb-3" style="display:none;">
                            <div class="card">
                                <div class="card-header p-0">
                                    <button data-target="" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                        <b>Telephone</b>
                                    </button>
                                </div>
                                <div class="collapse show">
                                    <div class="mb-3 text-right">
                                        <button id="btnAddOppTelephone" data-role="none" class="btn btn-success">Add Telephone</button>
                                    </div>

                                    <ul id="lstOppTelephone" data-role="none" class="list-group"></ul>
                                </div>
                            </div>
                        </div>

                        <div class="accordion mb-3" style="display:none;">
                            <div class="card">
                                <div class="card-header p-0">
                                    <button data-target="" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                        <b>Mobile</b>
                                    </button>
                                </div>
                                <div class="collapse show">
                                    <div class="mb-3 text-right">
                                        <button id="btnAddOppMobile" data-role="none" class="btn btn-success">Add Mobile</button>
                                    </div>

                                    <ul id="lstOppMobile" data-role="none" class="list-group"></ul>
                                </div>
                            </div>
                        </div>

                        <div class="accordion mb-3" style="display:none;">
                            <div class="card">
                                <div class="card-header p-0">
                                    <button data-target="" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                        <b>Email</b>
                                    </button>
                                </div>
                                <div class="collapse show">
                                    <div class="mb-3 text-right">
                                        <button id="btnAddOppEmail" data-role="none" class="btn btn-success">Add Email</button>
                                    </div>

                                    <ul id="lstOppEmail" data-role="none" class="list-group"></ul>
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3 align-items-center">
                            <div class="col-md">
                                <h4>Contact Person</h4>
                            </div>
                            <div class="col-md text-right">
                                <button id="btnAddOppContactPerson" data-role="none" class="btn btn-success">Add Contact Person</button>
                            </div>
                        </div>

                        <ul id="lstOppContactPerson" data-role="listview" class="list-group"></ul>
                    </div>

                    <div id="tabOpportunityComment" class="tab-pane fade">
                        <div class="mb-3" style="text-align:right;"><button data-role="none" id="btnOppAddComment" class="btn btn-success">Add Comment</button></div>
                        <ul id="listOppComments" data-role="none" class="list-group"></ul>
                    </div>

                    <div id="tabOpportunityStage" class="tab-pane fade">
                        <div class="mb-3 row">
                            <label for="selOpportunityFunnelTemplate" class="col-form-label col-md-3">Funnel Template</label>
                            <div class="col-md">
                                <select id="selOpportunityFunnelTemplate" data-role="none" class="form-select"></select>
                            </div>
                        </div>

                        <ul id="listOppTemplate" data-role="none" class="list-group"></ul>
                        <ul id="listOppTemplateOnChange" data-role="none" class="list-group"></ul>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col-md order-2 p-1 order-md-0">
                        <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                    </div>
                    <div class="col-md order-0 p-1 order-md-1">
                        <button data-role="none" type="button" id="btnSaveOpp" class="btn btn-success btn-block btn-lg btnSaveTicket">Save</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popOppTelInfo" class="modal fade" tabindex="-1" data-backdrop="static">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Telephone Info</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div data-role="fieldcontain" style="display:none;">
                    <input type="text" readonly id="txtOppTelCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                    <input type="text" readonly id="txtOppTelID" />
                    <input type="text" readonly id="txtOppTelAreaCodeID" />
                </div>

                <div class="mb-3">
                    <label for="btnOppTelCountryCodeDisable" class="form-label">Country Code: <span style="color: #FF0000;">*</span></label>
                    <button data-role="none" id="btnOppTelCountryCodeDisable" class="btn btn-secondary btn-block">
                        <span id="txtOppCountryCode">PHL - 63</span>
                    </button>
                </div>

                <div class="mb-3">
                    <label for="btnOppTelAreaCode" class="form-label">Area Code: <span style="color: #FF0000;">*</span></label>
                    <button data-role="none" id="btnOppTelAreaCode" class="btn btn-secondary btn-block">
                        <span id="txtOppAreaCode">Select</span>
                    </button>
                </div>

                <div class="mb-3">
                    <label for="txtOppTelephoneNum" class="form-label">Telephone Number: <span style="color: #FF0000;">*</span></label>
                    <input data-role="none" class="form-control" type="number" id="txtOppTelephoneNum" />
                </div>

                <div class="mb-3">
                    <label for="txtOppTelephoneExt" class="form-label">Extension or Local No.:</label>
                    <input data-role="none" class="form-control" type="number" id="txtOppTelephoneExt" maxlength="10" />
                </div>

                <div class="mb-3">
                    <label for="selOppTelType" class="form-label">Contact Type: <span style="color: #FF0000;">*</span></label>
                    <select id="selOppTelType" data-role="none" class="form-select">
                        <option value="">Please Select</option>
                        <option value="Work Telephone">Work Telephone</option>
                        <option value="Personal Telephone">Personal Telephone</option>
                        <option value="Third Party Telephone">Third Party Telephone</option>
                    </select>
                </div>
            </div>
            <div class="modal-footer">
                <div class="col-md order-1 order-md-0">
                    <button data-role="none" type="button" id="btnCancelOppTelephone" class="btn btn-secondary btn-lg">Close</button>
                </div>
                <div class="col-md order-0 order-md-1">
                    <button data-role="none" type="button" id="btnSaveOppTelephone" class="btn btn-success btn-lg">Ok</button>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popOppMobileInfo" class="modal fade" tabindex="-1" data-backdrop="static">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Mobile Info</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div data-role="fieldcontain" style="display:none;">
                    <input type="text" readonly id="txtOppMobileCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                    <input type="text" readonly id="txtOppMobileID" />
                    <input type="text" readonly id="txtOppTelCoPrefixID" />
                </div>

                <div class="mb-3">
                    <label for="btnOppMobileCountryCodeDisable" class="form-label">Country Code: <span style="color: #FF0000;">*</span></label>
                    <button data-role="none" id="btnOppMobileCountryCodeDisable" class="btn btn-secondary btn-block">
                        <span id="txtOppMobileCountryCode">PHL - 63</span>
                    </button>
                </div>

                <div class="mb-3">
                    <label for="btnOppMobileTelCode" class="form-label">Tel Code: <span style="color: #FF0000;">*</span></label>
                    <button data-role="none" id="btnOppMobileTelCode" class="btn btn-secondary btn-block">
                        <span id="txtOppTelCode">Select</span>
                    </button>
                </div>

                <div class="mb-3">
                    <label for="txtOppMobileNo" class="form-label">Mobile Number: <span style="color: #FF0000;">*</span></label>
                    <input data-role="none" class="form-control" type="number" id="txtOppMobileNo" />
                </div>

                <div class="mb-3">
                    <label for="selOppMobileType" class="form-label">Contact Type: <span style="color: #FF0000;">*</span></label>
                    <select id="selOppMobileType" data-role="none" class="form-select">
                        <option value="">Please Select</option>
                        <option value="Work Mobile">Work Mobile</option>
                        <option value="Personal Mobile">Personal Mobile</option>
                        <option value="Third Party Mobile">Third Party Mobile</option>
                    </select>
                </div>
            </div>
            <div class="modal-footer">
                <div class="col-md order-1 order-md-0">
                    <button data-role="none" type="button" id="btnCancelOppMobile" class="btn btn-secondary btn-lg">Close</button>
                </div>
                <div class="col-md order-0 order-md-1">
                    <button data-role="none" type="button" id="btnSaveOppMobile" class="btn btn-success btn-lg">Ok</button>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popOppEmailInfo" class="modal fade" tabindex="-1" data-backdrop="static">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Email Address</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div data-role="fieldcontain" style="display:none;">
                    <input type="text" readonly id="txtOppEmailID" />
                </div>
                
                <div class="mb-3">
                    <label for="txtOppEmailAdd" class="form-label">Email Address: <span style="color: #FF0000;">*</span></label>
                    <input data-role="none" class="form-control" type="email" id="txtOppEmailAdd" />
                </div>

                <div class="mb-3">
                    <label for="selOppEmailType" class="form-label">Email Type: <span style="color: #FF0000;">*</span></label>
                    <select id="selOppEmailType" data-role="none" class="form-select">
                        <option value="">Please Select</option>
                        <option value="Work Email">Work Email</option>
                        <option value="Personal Email">Personal Email</option>
                        <option value="Third Party Email">Third Party Email</option>
                    </select>
                </div>
            </div>
            <div class="modal-footer">
                <div class="col-md order-1 order-md-0">
                    <button data-role="none" type="button" id="btnCancelOppEmailAddress" class="btn btn-secondary btn-lg">Close</button>
                </div>
                <div class="col-md order-0 order-md-1">
                    <button data-role="none" type="button" id="btnSaveOppEmailAddress" class="btn btn-success btn-lg">Ok</button>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popOppContactPersonProfile" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Person Profile</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <input type="text" id="txtOppPeopleID" style="display:none;" data-role="none" />
                <input type="text" id="txtOppContactPersonID" style="display:none;" data-role="none" />

                <div class="mb-3 row">
                    <label for="txtOppContactFirstName" class="col-form-label col-md-3">Full Name:</label>
                    <div class="col-md">
                        <input data-role="none" class="form-control" type="text" id="txtOppContactFirstName" placeholder="First Name" />
                    </div>
                    <div class="col-md">
                        <input data-role="none" class="form-control" type="text" id="txtOppContactMiddleName" placeholder="Middle Name" />
                    </div>
                    <div class="col-md">
                        <input data-role="none" class="form-control" type="text" id="txtOppContactLastName" placeholder="Last Name" />
                    </div>
                </div>

                <div class="mb-3 row">
                    <label for="selOppClientName" class="col-form-label col-md-3">Generational Suffix:</label>
                    <div class="col-md">
                        <select id="selOppContactGenSuffix" data-role="none" class="form-select">
                            <option value="">None</option>
                            <option value="Jr.">Jr.</option>
                            <option value="Sr.">Sr.</option>
                            <option value="I">I</option>
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

                <div class="mb-3 row">
                    <label for="txtOppContactPersonPosition" class="col-form-label col-md-3">Position/Title:</label>
                    <div class="col-md">
                        <input data-role="none" class="form-control" type="text" id="txtOppContactPersonPosition" />
                    </div>
                </div>

                <div class="mb-3 row">
                    <label for="txtOppContactPersonPositionDescription" class="col-form-label col-md-3">Position/Title Description:</label>
                    <div class="col-md">
                        <input data-role="none" class="form-control" type="text" id="txtOppContactPersonPositionDescription" />
                    </div>
                </div>

                <div class="accordion mb-3">
                    <div class="card">
                        <div class="card-header p-0">
                            <button data-target="" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                <b>Address</b>
                            </button>
                        </div>
                        <div class="collapse show">
                            <div class="card-body">
                                <input data-role="none" type="text" id="txtOppContactPersonAddressID" style="display:none;" />
                                <div class="mb-3 row">
                                    <label for="txtOppContactPersonHouseBuildingNo" class="col-form-label col-md-3">House or Building No:</label>
                                    <div class="col-md">
                                        <input data-role="none" class="form-control" type="text" id="txtOppContactPersonHouseBuildingNo" />
                                    </div>
                                </div>

                                <div class="mb-3 row">
                                    <label for="txtOppContactPersonStreet" class="col-form-label col-md-3">Street:</label>
                                    <div class="col-md">
                                        <input data-role="none" class="form-control" type="text" id="txtOppContactPersonStreet" />
                                    </div>
                                </div>

                                <div class="mb-3 row">
                                    <label for="txtOppContactPersonStreet" class="col-form-label col-md-3">City Or Town:</label>
                                    <div class="col-md">
                                        <select id="selOppContactPersonCity" data-role="none" class="form-select"></select>
                                    </div>
                                </div>

                                <div class="mb-3 row">
                                    <label for="selOppContactPersonCountry" class="col-form-label col-md-3">Country:</label>
                                    <div class="col-md">
                                        <select id="selOppContactPersonCountry" data-role="none" class="form-select"></select>
                                    </div>
                                </div>

                                <div class="mb-3 row">
                                    <label for="selOppContactPersonTypeAddress" class="col-form-label col-md-3">Type of Address:</label>
                                    <div class="col-md">
                                        <select id="selOppContactPersonTypeAddress" data-role="none" class="form-select">
                                            <option value="Permanent Address">Permanent Address</option>
                                            <option value="Non-Permanent Address">Non-Permanent Address</option>
                                            <option value="Provincial Address">Provincial Address</option>
                                        </select>
                                    </div>
                                </div>
                            </div>                            
                        </div>
                    </div>
                </div>

                <h3 class="mb-3">Person Contact Info</h3>

                <div class="accordion mb-3">
                    <div class="card">
                        <div class="card-header p-0">
                            <button data-target="" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                <b>Telephone Number</b>
                            </button>
                        </div>
                        <div class="collapse show">
                            <div class="card-body">
                                <div id="divOppContactPersonTel" subpage="contactpersontelpage" style="display:block;">
                                    <div class="mb-3 text-right">
                                        <button id="btnAddOppContactPersonTelephone" data-role="none" class="btn btn-success">Add Telephone</button>
                                    </div>

                                    <ul id="lstOppContactPersonTelephone" data-role="none" class="list-group"></ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="accordion mb-3">
                    <div class="card">
                        <div class="card-header p-0">
                            <button data-target="" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                <b>Mobile Number</b>
                            </button>
                        </div>
                        <div class="collapse show">
                            <div class="card-body">
                                <div id="divOppContactPersonMobileHome" subpage="contactpersonmobilepage" style="display:block;">
                                    <div class="mb-3 text-right">
                                        <button id="btnAddOppContactPersonMobile" data-role="none" class="btn btn-success">Add Mobile</button>
                                    </div>

                                    <ul id="lstOppContactPersonMobile" data-role="none" class="list-group"></ul>
                                </div>
                            </div>                            
                        </div>
                    </div>
                </div>

                <div class="accordion mb-3">
                    <div class="card">
                        <div class="card-header p-0">
                            <button data-target="" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                <b>Email Address</b>
                            </button>
                        </div>
                        <div class="collapse show">
                            <div class="card-body">
                                <div id="divOppContactPersonEmailHome" subpage="contactpersonemailpage" style="display:block;">
                                    <div class="mb-3 text-right">
                                        <button id="btnAddOppContactPersonEmail" data-role="none" class="btn btn-success">Add Email</button>
                                    </div>

                                    <ul id="lstOppContactPersonEmail" data-role="none" class="list-group"></ul>
                                </div>
                            </div>                            
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col-md order-2 p-1 order-md-0">
                        <button data-role="none" type="button" id="btnOppCancelContact" class="btn btn-secondary btn-block btn-lg">Cancel</button>
                    </div>
                    <div class="col-md order-0 p-1 order-md-1">
                        <button data-role="none" type="button" id="btnOppOkContact" class="btn btn-success btn-block btn-lg">Ok</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popOppContactPersonTelInfo" class="modal fade" tabindex="-1" data-backdrop="static">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Telephone Info</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div data-role="fieldcontain" style="display:none;">
                    <input type="text" readonly id="txtOppContactPersonTelCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                    <input type="text" readonly id="txtOppContactPersonTelID" />
                    <input type="text" readonly id="txtOppContactPersonTelAreaCodeID" />
                </div>

                <div class="mb-3">
                    <label for="btnOppContactPersonTelCountryCodeDisable" class="form-label">Country Code: <span style="color: #FF0000;">*</span></label>
                    <button data-role="none" id="btnOppContactPersonTelCountryCodeDisable" class="btn btn-secondary btn-block">
                        <span id="txtOppContactPersonCountryCode">PHL - 63</span>
                    </button>
                </div>

                <div class="mb-3">
                    <label for="btnOppContactPersonTelAreaCode" class="form-label">Area Code: <span style="color: #FF0000;">*</span></label>
                    <button data-role="none" id="btnOppContactPersonTelAreaCode" class="btn btn-secondary btn-block">
                        <span id="txtOppContactPersonAreaCode">Select</span>
                    </button>
                </div>

                <div class="mb-3">
                    <label for="txtOppContactPersonTelephoneNum" class="form-label">Telephone Number: <span style="color: #FF0000;">*</span></label>
                    <input data-role="none" class="form-control" type="number" id="txtOppContactPersonTelephoneNum" />
                </div>

                <div class="mb-3">
                    <label for="txtOppContactPersonTelephoneExt" class="form-label">Extension or Local No.:</label>
                    <input data-role="none" class="form-control" type="number" id="txtOppContactPersonTelephoneExt" maxlength="10" />
                </div>

                <div class="mb-3">
                    <label for="selOppContactPersonTelType" class="form-label">Contact Type: <span style="color: #FF0000;">*</span></label>
                    <select id="selOppContactPersonTelType" data-role="none" class="form-select">
                        <option value="">Please Select</option>
                        <option value="Work Telephone">Work Telephone</option>
                        <option value="Personal Telephone">Personal Telephone</option>
                        <option value="Third Party Telephone">Third Party Telephone</option>
                    </select>
                </div>
            </div>
            <div class="modal-footer">
                <div class="col-md order-1 order-md-0">
                    <button data-role="none" type="button" id="btnCancelOppContactPersonTelephone" class="btn btn-secondary btn-lg">Close</button>
                </div>
                <div class="col-md order-0 order-md-1">
                    <button data-role="none" type="button" id="btnSaveOppContactPersonTelephone" class="btn btn-success btn-lg">Ok</button>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popOppContactPersonMobileInfo" class="modal fade" tabindex="-1" data-backdrop="static">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Mobile Info</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div data-role="fieldcontain" style="display:none;">
                    <input type="text" readonly id="txtOppContactPersonMobileCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                    <input type="text" readonly id="txtOppContactPersonMobileID" />
                    <input type="text" readonly id="txtOppContactPersonTelCoPrefixID" />
                </div>

                <div class="mb-3">
                    <label for="btnOppContactPersonMobileCountryCodeDisable" class="form-label">Country Code: <span style="color: #FF0000;">*</span></label>
                    <button data-role="none" id="btnOppContactPersonMobileCountryCodeDisable" class="btn btn-secondary btn-block">
                        <span id="txtOppContactPersonMobileCountryCode">PHL - 63</span>
                    </button>
                </div>

                <div class="mb-3">
                    <label for="btnOppContactPersonMobileTelCode" class="form-label">Tel Code: <span style="color: #FF0000;">*</span></label>
                    <button data-role="none" id="btnOppContactPersonMobileTelCode" class="btn btn-secondary btn-block">
                        <span id="txtOppContactPersonTelCode">Select</span>
                    </button>
                </div>

                <div class="mb-3">
                    <label for="txtOppContactPersonMobileNo" class="form-label">Mobile Number: <span style="color: #FF0000;">*</span></label>
                    <input data-role="none" class="form-control" type="number" id="txtOppContactPersonMobileNo" />
                </div>

                <div class="mb-3">
                    <label for="selOppContactPersonMobileType" class="form-label">Contact Type: <span style="color: #FF0000;">*</span></label>
                    <select id="selOppContactPersonMobileType" data-role="none" class="form-select">
                        <option value="">Please Select</option>
                        <option value="Work Mobile">Work Mobile</option>
                        <option value="Personal Mobile">Personal Mobile</option>
                        <option value="Third Party Mobile">Third Party Mobile</option>
                    </select>
                </div>
            </div>
            <div class="modal-footer">
                <div class="col-md order-1 order-md-0">
                    <button data-role="none" type="button" id="btnCancelOppContactPersonMobile" class="btn btn-secondary btn-lg">Close</button>
                </div>
                <div class="col-md order-0 order-md-1">
                    <button data-role="none" type="button" id="btnSaveOppContactPersonMobile" class="btn btn-success btn-lg">Ok</button>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popOppContactPersonEmailInfo" class="modal fade" tabindex="-1" data-backdrop="static">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Email Address</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div data-role="fieldcontain" style="display:none;">
                    <input type="text" readonly id="txtOppContactPersonEmailID" />
                </div>

                <div class="mb-3">
                    <label for="txtOppContactPersonEmailAdd" class="form-label">Email Address: <span style="color: #FF0000;">*</span></label>
                    <input data-role="none" class="form-control" type="email" id="txtOppContactPersonEmailAdd" />
                </div>

                <div class="mb-3">
                    <label for="selOppContactPersonEmailType" class="form-label">Email Type: <span style="color: #FF0000;">*</span></label>
                    <select id="selOppContactPersonEmailType" data-role="none" class="form-select">
                        <option value="">Please Select</option>
                        <option value="Work Email">Work Email</option>
                        <option value="Personal Email">Personal Email</option>
                        <option value="Third Party Email">Third Party Email</option>
                    </select>
                </div>
            </div>
            <div class="modal-footer">
                <div class="col-md order-1 order-md-0">
                    <button data-role="none" type="button" id="btnCancelOppContactPersonEmailAddress" class="btn btn-secondary btn-lg">Close</button>
                </div>
                <div class="col-md order-0 order-md-1">
                    <button data-role="none" type="button" id="btnSaveOppContactPersonEmailAddress" class="btn btn-success btn-lg">Ok</button>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popOppNewComment" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Add Comment</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <form method="post" enctype="multipart/form-data" id="formOppPic">
                    <input data-role="none" type="file" accept="image/*" name="fileOppPic" id="fileOppPic" style="margin-left:-1500px; position:absolute;">
                </form>
                <div class="mb-3 text-right">
                    <button data-role="none" class="btn btn-success btnOppAttachImage">Attach Image</button>
                </div>
                <div id="txtOppNewComment" contenteditable="" style="min-height:250px; max-height:250px;" placeholder="Type the note here ..."></div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col-md order-1 order-md-0">
                        <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Close</button>
                    </div>
                    <div class="col-md order-1 order-md-0">
                        <button data-role="none" type="button" id="btnSaveOppComment" class="btn btn-success btn-block btn-lg">Save Comment</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="divMainOpportunity" subpage="pageOpportunity" style="display:none;">
    <div class="fixed-ActionBtn" style="display:none;">
        <button id="btnAddOpportunity" data-role="none" class="btn custom-btn-theme-d btnCircle">
            <img alt="Add" src="/Content/images/plus-white.png" style="width:18px;" />
        </button>
    </div>
</div>