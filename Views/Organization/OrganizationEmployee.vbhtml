<script>
    require([GetPathFormat('/Scripts/sources/Organization/OrganizationEmployee.js?v=' + assetversion),
    //GetPathFormat('/Scripts3/Browser/LibraryPeople.js?crc='),
    GetPathFormat('/Scripts/sources/Organization/OrganizationEmployeeAdd.js?crc=')
    ]);
</script>

<div id="pageOrgEmployee" style="display:none;">

    @*<div id"divHeader" data-role="header">
            <h1>Employee List</h1>
        </div>*@

    <div data-role="content">

        <div>
            <ul data-role="listview" id="lstOrgEmployeeContainer" data-inset="true">
                <li data-role="list-divider">Employee List</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnOrgAddEmployee" data-role="button" data-icon="plus" data-inline="true">Add Employee</a></div>

                    <form id="frmOrgEmployeeSearch">
                        <div><input id="txtOrgEmployeeSearch" type="search" value="" /></div>
                    </form>

                    <div data-role="fieldcontain" id="divOrgRecordCountEmployee"></div>
                    <div>
                        <ul data-role="listview" id="lstOrgEmployee" data-inset="true">
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgEmployeeDone" data-role="button"><h3>Go Back to Main</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgEmpGoToSetupChecklist" data-role="button"><h3>Go to Setup Checklist</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgEmployeeInfo" style="display:none;">

    <div style="display:none;">
        <input type="text" data-role="none" class="form-control" readonly id="txtOrgEmpID" />
        <input type="text" data-role="none" class="form-control" readonly id="txtOrgPeopleID" />
    </div>

    <div class="row wrapper min-vh-100 flex-column flex-md-row">
        <div class="col-12 col-md-2 p-0 bg-light flex-shrink-1 mb-3">
            <nav class="navbar navbar-expand-sm navbar-light bg-light align-items-start flex-md-column flex-row p-2">
                <a data-role="none" class="navbar-brand" href="#">Menu</a>
                <a data-role="none" href="#" class="navbar-toggler" data-toggle="collapse" data-target=".sidebarEmployee">
                    <span class="navbar-toggler-icon"></span>
                </a>
                <div class="collapse navbar-collapse sidebarEmployee mt-2">
                    <ul class="flex-column w-100 justify-content-between nav nav-pills">
                        <li><a class="nav-link active" role="button" data-role="none" data-toggle="tab" href="#tabEmployeeInfo">Employee Info</a></li>
                        <li><a class="nav-link" role="button" data-role="none" data-toggle="tab" href="#tabEmployeeContactInfo">Contact Info</a></li>
                        <li><a class="nav-link" role="button" data-role="none" data-toggle="tab" href="#tabEmployeeFowst">FOWST</a></li>
                        <li><a class="nav-link" role="button" data-role="none" data-toggle="tab" href="#tabEmployeeAccessRight">Access Rights</a></li>
                        <li><a class="nav-link" role="button" data-role="none" data-toggle="tab" href="#tabEmployeeUserLabel">User Label</a></li>
                        <li><a class="nav-link d-none" role="button" data-role="none" data-toggle="tab" href="#tabEmployeePayFrequency">Pay Frequency</a></li>
                        <li><a class="nav-link" role="button" data-role="none" data-toggle="tab" href="#tabEmployeePayGrade">Pay Grade</a></li>
                        <li><a class="nav-link" role="button" data-role="none" data-toggle="tab" href="#tabEmployeeFinancialInformation">Financial Information</a></li>
                        <li><a class="nav-link" role="button" data-role="none" data-toggle="tab" href="#tabEmployeeDeMinimis">De Minimis</a></li>
                        <li><a class="nav-link" role="button" data-role="none" data-toggle="tab" href="#tabEmployeeRelationshipPerson">Org. Relationship to Person</a></li>
                        <li id="liEmployeeResetMobizAcc" style="display:none;"><a class="nav-link" role="button" data-role="none" data-toggle="tab" href="#tabEmployeeResetMobizAcc">Reset Mobiz Account</a></li>
                    </ul>
                </div>
            </nav>
        </div>

        <div class="col bg-faded mb-5">
            <div class="tab-content p-md-3">
                <div class="tab-pane fade active show" id="tabEmployeeInfo">
                    <div id="divAddEmpNav" class="mb-3" style="float:right; visibility:hidden; display:none;">
                        <button id="btnChangeEmployee" data-role="none" class="btn btn-success">Change Selected Employee</button>
                        @*<button id="btnAddNewEmployee">Add New Employee</button>*@
                    </div>

                    <div class="row mb-3">
                        <label class="col-md-2 col-form-label">Employee Name:</label>
                        <div class="col-md-10">
                            <div id="divbtnOld" style="display:none">
                                @*<button id="btnOrgEmployee" data-role="button">Please Select</button>*@
                                <input type="text" data-role="none" class="form-control" id="txtOrgEmployee" readonly />
                            </div>
                            <div id="divbtnNew" style="display:none">
                                @*<button id="btnOrgEmployeeNEW" data-role="button">Please Select</button>*@
                                <input type="text" data-role="none" class="form-control" id="txtOrgEmployeeNew" readonly />
                            </div>
                            <div id="divEditEmpName" style="display:none;">
                                <div class="row">
                                    <div class="col-md">
                                        <fieldset>
                                            <legend class="w-auto float-none text-secondary" style="font-size:14px;">First Name <span class="required">*</span></legend>
                                            <input type="text" data-role="none" class="form-control" id="txtOrgEmployeeFirstName" />
                                        </fieldset>
                                    </div>
                                    <div class="col-md">
                                        <fieldset>
                                            <legend class="w-auto float-none text-secondary" style="font-size:14px;">Middle Name</legend>
                                            <input type="text" data-role="none" class="form-control" id="txtOrgEmployeeMiddleName" />
                                        </fieldset>
                                    </div>
                                    <div class="col-md">
                                        <fieldset>
                                            <legend class="w-auto float-none text-secondary" style="font-size:14px;">Last Name <span class="required">*</span></legend>
                                            <input type="text" data-role="none" class="form-control" id="txtOrgEmployeeLastName" />
                                        </fieldset>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-md-2 col-form-label">Date Hired: <span style="color: #FF0000;">*</span></label>
                        <div class="col-md-auto">
                            <div class="input-group">
                                <select id="selOrgDateHiredMonth2" data-role="none" class="form-select">
                                    <option value="">MMM</option>
                                </select>
                                <select id="selOrgDateHiredDay2" data-role="none" class="form-select">
                                    <option value="">DD</option>
                                </select>
                                <select id="selOrgDateHiredYear2" data-role="none" class="form-select">
                                    <option value="">YYYY</option>
                                </select>
                            </div>
                        </div>
                    </div>

                    <div class="row mb-3" style="display:none;">
                        <label class="col-md-2 col-form-label" for="txtSystemGeneratedID">System Generated ID No.:</label>
                        <div class="col-md-10">
                            <input type="text" id="txtSystemGeneratedID" data-role="none" class="form-control" />
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-md-2 col-form-label" for="txtCompanyID">Company ID No.:</label>
                        <div class="col-md-10">
                            <input type="text" id="txtCompanyID" data-role="none" class="form-control" />
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-md-2 col-form-label" for="txtSSSID">SSS ID No.:</label>
                        <div class="col-md-10">
                            <input type="text" id="txtSSSID" data-role="none" class="form-control" />
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-md-2 col-form-label" for="txtPhilHealthID">PhilHealth ID No.:</label>
                        <div class="col-md-10">
                            <input type="text" id="txtPhilHealthID" data-role="none" class="form-control" />
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-md-2 col-form-label" for="txtPagIbigID">Pag-Ibig ID No.:</label>
                        <div class="col-md-10">
                            <input type="text" id="txtPagIbigID" data-role="none" class="form-control" />
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-md-2 col-form-label" for="txtTINID">TIN ID No.:</label>
                        <div class="col-md-10">
                            <input type="text" id="txtTINID" data-role="none" class="form-control" />
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-md-2 col-form-label" for="txtDepartment">Department:</label>
                        <div class="col-md-10">
                            <input type="search" id="txtDepartment" data-role="none" class="form-control" placeholder="Search Department" />
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-md-2 col-form-label" for="txtTeam">Team:</label>
                        <div class="col-md-10">
                            <input type="search" id="txtTeam" data-role="none" class="form-control SearchText" placeholder="Search Team" />
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-auto col-md-2" for="selOrgDeactivate">Deactivate record?</label>
                        <div class="col col-md-10">
                            <div class="form-check form-switch mb-3" style="font-size:20px;">
                                <input class="form-check-input" data-role="none" type="checkbox" id="selOrgDeactivate">
                            </div>
                        </div>
                    </div>

                    <div id="divOrgDeactivateDetails" style="display:none;">
                        <div class="row mb-3">
                            <label class="col-md-2 col-form-label">Deactivation Date: <span style="color: #FF0000;">*</span></label>
                            <div class="col-md-auto">
                                <div class="input-group">
                                    <select id="selOrgDateInActiveMonth" data-role="none" class="form-select">
                                        <option value="">MMM</option>
                                    </select>
                                    <select id="selOrgDateInActiveDay" data-role="none" class="form-select">
                                        <option value="">DD</option>
                                    </select>
                                    <select id="selOrgDateInActiveYear" data-role="none" class="form-select">
                                        <option value="">YYYY</option>
                                    </select>
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-2 col-form-label" for="txtOrgInActiveReason">Reason for deactivating:</label>
                            <div class="col-md-10">
                                <textarea id="txtOrgInActiveReason" data-role="none" class="form-control" style="height:100px; resize:none;"></textarea>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-2 col-form-label" for="txtOrgComments">Comments:</label>
                            <div class="col-md-10">
                                <textarea id="txtOrgComments" data-role="none" class="form-control" style="height:100px; resize:none;"></textarea>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-auto col-md-2" for="selOrgBan">Ban this record for future transactions?</label>
                            <div class="col col-md-10">
                                <div class="form-check form-switch mb-3" style="font-size:20px;">
                                    <input class="form-check-input" data-role="none" type="checkbox" id="selOrgBan">
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-md-2 col-form-label" for="txtOrg3rdPartySystemName">Third Party System Name:</label>
                        <div class="col-md-10">
                            <input type="text" id="txtOrg3rdPartySystemName" data-role="none" class="form-control" />
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-md-2 col-form-label" for="txtOrg3rdPartySystemID">Third Party System ID:</label>
                        <div class="col-md-10">
                            <input type="text" id="txtOrg3rdPartySystemID" data-role="none" class="form-control" />
                        </div>
                    </div>
                </div>

                <div class="tab-pane fade" id="tabEmployeeContactInfo">
                    <div id="divExpenseTrackerTab" style="white-space:nowrap; overflow:auto;">
                        <ul class="nav nav-tabs" role="tablist" style="flex-wrap:nowrap;">
                            <li class="nav-item" role="presentation">
                                <button data-role="none" class="nav-link active" data-toggle="tab" data-target="#tabEmpContactTelephone">Telephone</button>
                            </li>
                            <li class="nav-item" role="presentation">
                                <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabEmpContactMobile">Mobile</button>
                            </li>
                            <li class="nav-item" role="presentation">
                                <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabEmpContactEmail">E-mail Address</button>
                            </li>
                        </ul>
                    </div>
                    <div class="tab-content p-3">
                        <div id="tabEmpContactTelephone" class="tab-pane fade show active">
                            <div class="text-right mb-3">
                                <button id="btnAddEmployeeTelephone" data-role="none" class="btn btn-success">
                                    Add Telephone
                                </button>
                            </div>

                            <ul data-role="none" id="lstEmployeeTelephone" class="list-group"></ul>
                        </div>
                        <div id="tabEmpContactMobile" class="tab-pane fade">
                            <div class="text-right mb-3">
                                <button id="btnAddEmployeeMobile" data-role="none" class="btn btn-success">
                                    Add Mobile
                                </button>
                            </div>

                            <ul data-role="none" id="lstEmployeeMobile" class="list-group"></ul>
                        </div>
                        <div id="tabEmpContactEmail" class="tab-pane fade">
                            <div class="text-right mb-3">
                                <button id="btnAddEmployeeEmail" data-role="none" class="btn btn-success">
                                    Add Email Address
                                </button>
                            </div>

                            <ul data-role="none" id="lstEmployeeEmail" class="list-group"></ul>
                        </div>
                    </div>
                </div>

                <div class="tab-pane fade" id="tabEmployeeFowst">
                    <div class="row mb-3">
                        <div class="col-md">
                            This employee will be assigned to ff FOWST (Factory, Office, Warehouse, Store, or Truck) location(s)
                        </div>
                        <div class="col-md text-right">
                            @*<button id="btnOrgRemoveAllEmployeeFowst" data-role="none" class="btn btn-success">Remove All FOWST</button>*@
                            <button id="btnOrgSelectionEmployeeFowst" data-role="none" class="btn btn-secondary" value="0">Select All FOWST</button>
                            <button id="btnOrgRemoveEmployeeSelectedFowst" data-role="none" class="btn btn-danger">Remove Selected FOWST</button>
                            <button id="btnOrgAddEmployeeFowst" data-role="none" class="btn btn-success">Add FOWST</button>
                        </div>
                    </div>

                    <div data-role="none" id="lstOrgEmployeeFowst" class="list-group mb-3"></div>
                </div>

                <div class="tab-pane fade" id="tabEmployeeAccessRight">
                    <div class="text-right mb-3">
                        <button id="btnOrgAddEmployeeRole" data-role="none" class="btn btn-success">Add Access Rights</button>
                    </div>

                    <div data-role="none" id="lstOrgEmployeeRole" class="list-group mb-3"></div>
                </div>

                <div class="tab-pane fade" id="tabEmployeeUserLabel">
                    <div class="row mb-3">
                        <div class="col-md">
                            Please choose the label that best describes the role of this person in your organization.
                        </div>
                        <div class="col-md text-right"></div>
                    </div>

                    <div id="divEmpRoleTags" class="mb-3"></div>
                </div>

                <div class="tab-pane fade" id="tabEmployeePayFrequency">
                    <div class="form-check mb-3" style="font-size:20px;">
                        <input class="form-check-input" data-role="none" type="radio" id="pf_daily" name="payfrequency" value="Daily" checked="checked">
                        <label class="form-check-label" for="pf_daily">Daily</label>
                    </div>

                    <div class="form-check mb-3" style="font-size:20px;">
                        <input class="form-check-input" data-role="none" type="radio" id="pf_weekly" name="payfrequency" value="Weekly">
                        <label class="form-check-label" for="pf_weekly">Weekly</label>
                    </div>

                    <div class="form-check mb-3" style="font-size:20px;">
                        <input class="form-check-input" data-role="none" type="radio" id="pf_semimonthly" name="payfrequency" value="Semi-Monthly">
                        <label class="form-check-label" for="pf_semimonthly">Semi-Monthly</label>
                    </div>

                    <div class="form-check mb-3" style="font-size:20px;">
                        <input class="form-check-input" data-role="none" type="radio" id="pf_monthly" name="payfrequency" value="Monthly">
                        <label class="form-check-label" for="pf_monthly">Monthly</label>
                    </div>

                    <div class="form-check mb-3" style="font-size:20px;">
                        <input class="form-check-input" data-role="none" type="radio" id="pf_projectbased" name="payfrequency" value="Project-based">
                        <label class="form-check-label" for="pf_projectbased">Project-based</label>
                    </div>
                </div>

                <div class="tab-pane fade" id="tabEmployeeWorkSchedule">
                    <div class="row mb-3">
                        <div class="col-md">
                            Please choose employee work schedule
                        </div>
                        <div class="col-md text-right">
                            <button id="btnEmpWorkSchedule" data-role="none" class="btn btn-success">Please Select</button>
                        </div>
                    </div>

                    <div class="table-responsive">
                        <table data-role="none" class="table table-striped table-hover">
                            <thead>
                                <tr class="custom-theme-h">
                                    <th>
                                        Schedule Name
                                    </th>
                                    <th>
                                        Schedule Description
                                    </th>
                                    <th>
                                        Schedule Type
                                    </th>
                                    <th>
                                        From
                                    </th>
                                    <th>
                                        To
                                    </th>
                                    <th>
                                        Schedule Work Days
                                    </th>
                                </tr>
                            </thead>
                            <tbody id="tblWorkScheduleBody"></tbody>
                        </table>
                    </div>
                </div>

                <div class="tab-pane fade" id="tabEmployeePayGrade">
                    <div class="row mb-3">
                        <div class="col-md">
                            Please choose employee Pay Grade
                        </div>
                        <div class="col-md text-right">
                            <button id="btnEmpPayType" data-role="none" class="btn btn-success">Please Select</button>
                        </div>
                    </div>

                    <div class="table-responsive">
                        <table data-role="none" class="table table-striped table-hover">
                            <thead>
                                <tr class="custom-theme-h">
                                    <th>
                                        Salary Grade Name
                                    </th>
                                    <th>
                                        Pay Type
                                    </th>
                                    <th>
                                        Basic Pay
                                    </th>
                                    <th>
                                        Work Hours Per Day
                                    </th>
                                </tr>
                            </thead>
                            <tbody id="tblPayGradeBody"></tbody>
                        </table>
                    </div>
                </div>

                <div class="tab-pane fade" id="tabEmployeeFinancialInformation">
                    <input type="text" data-role="none" class="form-control" id="txtBankID" style="display:none;" />

                    <div class="row mb-3">
                        <label class="col-md-2 col-form-label" for="btnBank">Name of Bank:</label>
                        <div class="col-md-10">
                            <button id="btnBank" data-role="none" class="btn btn-secondary btn-block">Select Bank</button>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-md-2 col-form-label" for="btnBankBranch">Branch Name:</label>
                        <div class="col-md-10">
                            <input type="text" data-role="none" class="form-control" id="btnBankBranch" />
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-md-2 col-form-label" for="txtAccountName">Account Name:</label>
                        <div class="col-md-10">
                            <input type="text" data-role="none" class="form-control" id="txtAccountName" />
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-md-2 col-form-label" for="txtAccountNo">Account No:</label>
                        <div class="col-md-10">
                            <input type="text" data-role="none" class="form-control" id="txtAccountNo" />
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-md-2 col-form-label" for="selBankRole">Bank Role::</label>
                        <div class="col-md-10">
                            <select id="selBankRole" data-role="none" class="form-select" readonly>
                                <option value="Payroll" selected>Disbursement Bank for Payroll</option>
                            </select>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-auto col-md-2" for="selIsDefaultAccount">Make Default Account For This Role?</label>
                        <div class="col col-md-10">
                            <div class="form-check form-switch mb-3" style="font-size:20px;">
                                <input class="form-check-input" data-role="none" type="checkbox" id="selIsDefaultAccount">
                            </div>
                        </div>
                    </div>
                </div>

                <div class="tab-pane fade" id="tabEmployeeDeMinimis">
                    @*<div class="row">
                            <div class="col-sm-6">
                                <br />
                                <div data-role="fieldcontain"><a id="btnOrgAddEmployeeDeminimis" data-role="button" data-icon="plus" data-inline="true">Add De Minimis</a></div>
                                <br />
                            </div>
                        </div>*@
                    @*<div class="row">
                            <div>

                            </div>
                        </div>*@

                    <table data-role="none" class="table table-light table-striped table-hover">
                        <thead>
                            <tr class="align-top text-center" style="font-size:14px;">
                                <th>
                                    Allowance Name
                                </th>
                                <th>
                                    Amount
                                </th>
                                <th>
                                    Max Amount
                                </th>
                                <th>
                                    Frequency
                                </th>
                                <th style="width: 50px;">
                                </th>
                            </tr>
                        </thead>
                        <tbody id="tblDeminimisBody"></tbody>
                    </table>
                </div>

                <div class="tab-pane fade" id="tabEmployeeRelationshipPerson">
                    <div class="row mb-3">
                        <label class="col-auto col-md-2" for="selSelfEmployed">Are you self-employed?</label>
                        <div class="col col-md-10">
                            <div class="form-check form-switch mb-3" style="font-size:20px;">
                                <input class="form-check-input" data-role="none" type="checkbox" id="selSelfEmployed">
                            </div>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-md-2" for="selManPower">
                            This person is directly hired by <span class="lblUserOrgName"></span>
                        </label>
                        <div class="col-md-10">
                            <div class="form-check form-switch mb-3" style="font-size:20px;">
                                <input class="form-check-input" data-role="none" type="checkbox" id="selManPower">
                            </div>
                            <i style="font-weight:normal;">Please enable if your company has a direct employer-employee relationship with this person. Choose "No" if there is none.</i>
                        </div>
                    </div>

                    <div id="divManPower" style="display:none;">
                        <div class="row mb-3">
                            <label class="col-form-label" for="btnEmpManPower">
                                Manpower Agency:
                            </label>
                            <div class="col-md-10">
                                <button data-role="none" class="btn btn-secondary btn-block mb-3" id="btnEmpManPower">Please select</button>
                                <i style="font-weight:normal">Please choose the company or manpower agency that has a direct employer-employee relationship with this person.</i>
                            </div>
                        </div>
                    </div>

                    <div id="divOtherRelationship" style="display:none;">
                        <div class="row mb-3">
                            <label class="col-auto col-md-2" for="selVendorTradingPartner">This person's service is provided by a vendor or trading/ business partner.</label>
                            <div class="col col-md-10">
                                <div class="form-check form-switch mb-3" style="font-size:20px;">
                                    <input class="form-check-input" data-role="none" type="checkbox" id="selVendorTradingPartner">
                                </div>
                            </div>
                        </div>

                        <div id="divVenderTradingPartner" style="display:none;">
                            <div class="row mb-3">
                                <label class="col-md-2 col-form-label" for="btnEmpVendorTradingPartner">Vendor/ Trading Partner's name:</label>
                                <div class="col-md-10">
                                    <button data-role="none" class="btn btn-secondary btn-block" id="btnEmpVendorTradingPartner">Please select</button>
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-auto col-md-2" for="selEmpCoUser">
                                This person is not an employee or trading partner staff. This person is simply a co-user of <span class="lblUserOrgName"></span>'s DTR machine.
                            </label>
                            <div class="col col-md-10">
                                <div class="form-check form-switch mb-3" style="font-size:20px;">
                                    <input class="form-check-input" data-role="none" type="checkbox" id="selEmpCoUser">
                                </div>
                                <i style="font-weight:normal;">Please enable in case the DTR machine you are using is used by more than one (1) orgnization who are non-trading partners but are located in the same premises.</i>
                            </div>
                        </div>

                        <div id="divCoUserOrgName" style="display:none;">
                            <div class="row mb-3">
                                <label class="col-md-2 col-form-label" for="btnEmpCoUser">Co-User Organization Name:</label>
                                <div class="col-md-10">
                                    <button data-role="none" class="btn btn-secondary btn-block" id="btnEmpCoUser">Please select</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div id="tabEmployeeResetMobizAcc" class="tab-pane fade">
                    <div class="mb-3" id="lblMobizAccountInfo">

                    </div>

                    <div class="row mb-3 align-items-center">
                        <div class="col-sm-2">
                            Current Username:
                        </div>
                        <div class="col-sm">
                            <input data-role="none" type="text" class="form-control" id="txtEmployeeCurrentUsername" readonly />
                        </div>
                    </div>

                    <div class="row mb-3 align-items-center">
                        <div class="col-sm-2">
                            New Username:
                        </div>
                        <div class="col-sm">
                            <input data-role="none" type="text" class="form-control" id="txtEmployeeNewUsername" />
                        </div>
                    </div>

                    <div class="row mb-3 align-items-center">
                        <div class="col-sm-2">
                            New Password:
                        </div>
                        <div class="col-sm">
                            <input data-role="none" type="password" class="form-control" id="txtEmployeeNewPassword" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    @*<div>
            <div class="row mb-4">
                <div class="col-6 col-form-label">
                    <h1 class="display-6">Work Schedule For This Employee</h1>
                </div>
                <div class="col-6 text-right">
                    <button id="btnOrgAddEmployeeRole" data-role="none" class="btn btn-success">Add Access Rights</button>
                </div>
            </div>

            <div data-role="none" id="lstOrgEmployeeRole" class="list-group mb-3"></div>
        </div>*@

    <div id="divConfirmSection" style="display:none; margin-bottom:5rem;">
        <div class="row mb-4">
            <div class="col-12 col-form-label">
                <h1 class="display-6">Employment Confirmation</h1>
            </div>
        </div>

        <p style="font-size:18px;">
            Mobiz needs <span class="peopleFirstName" style="font-weight:bold"></span> to confirm that he/she is currently employed in <span style="font-weight:bold" id="txtOrgName"></span>.
        </p>
        <p style="font-size:18px;">
            Let <span class="peopleFirstName" style="font-weight:bold"></span> confirm his/her employment on:
        </p>

        <div class="text-center">
            <a id="btnOrgEmpInfoSaveAndInvite" data-role="none" class="btn btn-secondary btn-block btn-lg mr-3">Send Invite</a>
            <a id="btnOrgEmpInfoSaveAndConfirm" data-role="none" class="btn btn-secondary btn-block btn-lg mr-3">Confirm employment by signing in now</a>
            <a id="btnOrgEmpInfoSendConfirmation" data-role="none" class="btn btn-secondary btn-block btn-lg mr-3">Send Confirmation via Email</a>
        </div>
    </div>

    <div id="divEmpSaveCancelBtn" class="fixed-bottom text-center pb-4">
        <a id="btnOrgEmpInfoCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
        <a id="btnOrgEmpInfoSave" data-role="none" class="btn btn-success btn-lg">Save</a>
    </div>
</div>

<div id="popDeminimis" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">De minimis</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div class="row mb-3">
                    <label class="col-sm-3 mb-3 col-form-label" for="btnDeminimis">Select De Minimis:</label>
                    <div class="col-sm-9">
                        <button id="btnDeminimis" data-role="none" class="btn btn-secondary btn-block"> Please Select </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-3 mb-3 col-form-label" for="lblMaxAmount">Max Amount:</label>
                    <div class="col-sm-9">
                        <label id="lblMaxAmount" data-role="none"> 0.00 </label>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-3 mb-3 col-form-label" for="lblMaxTaxExemptAmount">Max Tax Exempt Amount:</label>
                    <div class="col-sm-9">
                        <label id="lblMaxTaxExemptAmount" data-role="none"> 0.00 </label>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-3 mb-3 col-form-label" for="lblFrequencyofAllowance">Frequency of Allowance:</label>
                    <div class="col-sm-9">
                        <label id="lblFrequencyofAllowance" data-role="none"> 0.00 </label>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-sm-3 mb-3 col-form-label" for="txtDeminimisAmount">Enter Di Minimis Amount:</label>
                    <div class="col-sm-9">
                        <input type="text" id="txtDeminimisAmount" data-role="none" class="form-control" />
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button data-role="none" type="button" id="btnDoneDeminimis" class="btn btn-success btn-block">Ok</button>
                <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
            </div>
        </div>
    </div>
</div>

<div id="pageOrgEmployeeSignIn" style="display:none;">

    <h1 class="display-6">Sign In</h1>

    <div data-role="content">
        <span id="txtOrgDateHiredMonth" style="display:none;"></span>
        <span id="txtOrgDateHiredYear" style="display:none;"></span>
        <span id="txtOrgDateInActiveMonth" style="display:none;"></span>
        <span id="txtOrgDateInActiveYear" style="display:none;"></span>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtUsername">Username:</label>
            <div class="col-md-10">
                <input type="text" id="txtUsername" data-role="none" class="form-control" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtPassword">Password:</label>
            <div class="col-md-10">
                <input type="password" id="txtPassword" data-role="none" class="form-control" />
            </div>
        </div>

        <div class="mt-4 text-center">
            <a id="btnOrgEmpCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
            <a id="btnOrgEmpSignIn" data-role="none" class="btn btn-success btn-lg">Sign In</a>
        </div>
    </div>
</div>

<div id="pageOrgEmployeeAdd" style="display:none;">
    <div class="form-check mb-3" style="font-size:20px;">
        <input class="form-check-input" data-role="none" type="checkbox" value="yes" id="chbFirstPartySignUp" name="firstPartySignUp">
        <label class="form-check-label" for="chbFirstPartySignUp">I am filling this form on behalf of another person. Ask him/her to change username and/or passwords upon first sign in.</label>
    </div>

    <h5 class="mb-3">Fields marked with <span style="color:Red"> * </span> are required.</h5>

    <div>
        <h1 class="display-6 mb-4">Profile</h1>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtFirstName">First Name: <span style="color:Red">*</span></label>
            <div class="col-md-10">
                <input type="text" data-role="none" formnovalidate class="form-control mb-3" id="txtFirstName" maxlength="150" required>
                <span style="font-size:small; color:Red; display:none;" id="spanFName">This field is required</span>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtMiddleName">Middle Name: <span style="color:Red">*</span></label>
            <div class="col-md-10">
                <input type="text" data-role="none" formnovalidate class="form-control mb-3" id="txtMiddleName" maxlength="150" required>
                <span style="font-size:small; color:Red; display:none;" id="spanMName">This field is required</span>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtSurname">Last Name: <span style="color:Red">*</span></label>
            <div class="col-md-10">
                <input type="text" data-role="none" formnovalidate class="form-control mb-3" id="txtSurname" maxlength="150" required>
                <span style="font-size:small; color:Red; display:none;" id="spanLName">This field is required</span>
            </div>
        </div>

        <div class="row mb-3 align-items-center">
            <label class="col-md-2 col-form-label" for="txtSurname">Generational Suffix: <span style="color:Red">*</span></label>
            <div class="col-sm-3">
                <select id="selPersonalSuffixName" data-role="none" class="form-select">
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
                <span style="font-size:small; color:Red; display:none;" id="spanSuffix">This field is required</span>
            </div>
            <div class="col-sm-7">
                <div class="form-check mb-3" style="font-size:20px;">
                    <input class="form-check-input" data-role="none" type="checkbox" value="None" id="chbNoSuffix" name="suffix">
                    <label class="form-check-label" for="chbNoSuffix">I don't have a generational suffix</label>
                </div>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtNickname">Nick Name:</label>
            <div class="col-md-10">
                <input type="text" data-role="none" class="form-control mb-3" id="txtNickname" maxlength="150">
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label">Date of Birth: <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-1">
                <select id="selDOBMonth" data-role="none" class="form-select">
                    <option value="">MMM</option>
                </select>
                <span style="font-size:small; color:Red;display:none" id="spanDateOfBirth">This field is required</span>
                <span style="font-size:small; color:Red;display:none" id="spanDOBGreater">Date of Birth must not be greater than the date today</span>
                <span style="font-size:small; color:Red;display:none" id="spanDOB18">You must be at least 18 years old to register as a super administrator</span>
            </div>
            <div class="col-sm-1">
                <select id="selDOBDay" data-role="none" class="form-select">
                    <option value="">DD</option>
                </select>
            </div>
            <div class="col-sm-1">
                <select id="selDOBYear" data-role="none" class="form-select">
                    <option value="">YYYY</option>
                </select>
            </div>
        </div>
    </div>

    <div>
        <h1 class="display-6 mb-4">Mother's Maiden Name</h1>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtMotherMaidenSurname">Mother's Maiden Surname (Last Name): <span style="color:Red">*</span></label>
            <div class="col-md-10">
                <input type="text" data-role="none" formnovalidate class="form-control mb-3" id="txtMotherMaidenSurname" maxlength="150" required>
                <span style="font-size:small; color:Red; display:none;" id="spanMotherFName">This field is required</span>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtMotherMaidenFirstName">Mother's First Name: <span style="color:Red">*</span></label>
            <div class="col-md-10">
                <input type="text" data-role="none" formnovalidate class="form-control mb-3" id="txtMotherMaidenFirstName" maxlength="150" required>
                <span style="font-size:small; color:Red; display:none;" id="spanMotherMName">This field is required</span>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtMotherMaidenMiddleName">Mother's Maiden Middle Name: <span style="color:Red">*</span></label>
            <div class="col-md-10">
                <input type="text" data-role="none" formnovalidate class="form-control mb-3" id="txtMotherMaidenMiddleName" maxlength="150" required>
                <span style="font-size:small; color:Red; display:none;" id="spanMotherLName">This field is required</span>
            </div>
        </div>
    </div>

    <div>
        <h1 class="display-6 mb-4">Contact Details</h1>

        <h5 class="mb-3">Please provide active contact details. We will use the contact details you provide here to 1.) notify you of important matters and 2.) troubleshoot your Mobiz account</h5>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="selCallingCodeTel">Telephone Number:</label>
            <div class="col-sm-3">
                <select id="selCallingCodeTel" data-role="none" class="form-select mb-3">
                    <option value="" selected="selected">Please Select Country Code</option>
                </select>
                <span style="font-size:small; color:Red;display:none" id="spanCountryCodeTelNo">Please select a country code</span>
            </div>
            <div class="col-sm-3">
                <select id="selAreaCode" data-role="none" class="form-select mb-3">
                    <option value="" selected="selected">Please Select Country Code</option>
                </select>
                <span style="font-size:small; color:Red;display:none" id="spanAreaCode">Please select area code</span>
            </div>
            <div class="col-sm-4">
                <input type="text" id="txtTelephone" data-role="none" class="form-control" data-regex="^[0-9]{7}$" maxlength="7" placeholder="Enter your 7-digit mobile number" required>
                <span style="font-size:small; color:Red;display:none" id="spanTelNoValid">Please enter a valid telephone number</span>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="selCallingCodeMobile">Mobile Number:</label>
            <div class="col-sm-3">
                <select id="selCallingCodeMobile" data-role="none" class="form-select mb-3">
                    <option value="" selected="selected">Please Select Country Code</option>
                </select>
                <span style="font-size:small; color:Red;display:none" id="spanCountryCodeMobile">Please select a country code</span>
            </div>
            <div class="col-sm-3">
                <select id="selTelcoPrefix" data-role="none" class="form-select mb-3">
                    <option value="" selected="selected">Please Select Country Code</option>
                </select>
                <span style="font-size:small; color:Red;display:none" id="spanTelcoPrefix">Please select Mobile Number Prefix</span>
            </div>
            <div class="col-sm-4">
                <input type="text" id="txtMobile" data-role="none" class="form-control" data-regex="^[0-9]{7}$" maxlength="7" placeholder="Enter your 7-digit mobile number" required>
                <span style="font-size:small; color:Red;display:none" id="spanMobileValid">Please enter a valid mobile number</span>
            </div>
        </div>

        <div class="row mb-3 align-items-center">
            <label class="col-md-2 col-form-label" for="txtSurname">Email Address:</label>
            <div class="col-sm-6">
                <input type="email" id="txtEmail" data-role="none" class="form-control mb-3" maxlength="150" required>
                <span style="font-size:small; color:Red;display:none" id="spanEmailReq">This field is required</span>
                <span style="font-size:small; color:Red;display:none" id="spanEmail">Please enter a valid email address</span>
            </div>
            <div class="col-sm-4">
                <div class="form-check mb-3" style="font-size:20px;">
                    <input class="form-check-input" data-role="none" type="checkbox" id="chbNoEmail" value="NoEmail" name="email">
                    <label class="form-check-label" for="chbNoEmail">I don't have a generational suffix</label>
                </div>
            </div>
        </div>
    </div>

    <div>
        <h1 class="display-6 mb-4">User Account</h1>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtUserName">User Name: <span style="color:Red">*</span></label>
            <div class="col-md-10">
                <input type="text" data-role="none" class="form-control mb-3" id="txtUserName" maxlength="150" required>
                <span style="font-size:small; color:Red;display:none" id="spanUserName">This field is required</span>
                <span style="font-size:small; color:Red;display:none" id="spanUserNameChars">User Name must be at least 8 characters long</span>
                <span style="font-size:small; color:Red;display:none" id="spanUserNameAlphanum">User Name must contain only alphanumeric and underscore characters</span>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtPassWord">Password: <span style="color:Red">*</span></label>
            <div class="col-md-10">
                <input type="password" data-role="none" class="form-control mb-3" id="txtPassWord" maxlength="150" required>
                <span style="font-size:small; color:Red;display:none" id="spanPassword">This field is required</span>
                <span style="font-size:small; color:Red;display:none" id="spanPasswordChars">Password must be at least 8 characters long</span>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtConfirmPassword">Confirm Password: <span style="color:Red">*</span></label>
            <div class="col-md-10">
                <input type="password" data-role="none" class="form-control mb-3" id="txtConfirmPassword" maxlength="150" required>
                <span style="font-size:small; color:Red;display:none" id="spanConfirmPassword">This field is required</span>
                <span style="font-size:small; color:Red;display:none" id="spanPasswordRepeat">The password you entered did not match</span>
            </div>
        </div>
    </div>

    <div>
        <h1 class="display-6 mb-4">Security Question</h1>

        <h5 class="mb-3">Note: You must remember the answers to the security questions you selected. This will be used to reset your account in case you forget your password.</h5>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="selQuestion1">Question No. 1: <span style="color:Red">*</span></label>
            <div class="col-sm-5">
                <select id="selQuestion1" data-role="none" class="form-select">
                    <option value="">Please Select</option>
                    <option value="Where is your mother's birthplace?">Where is your mother's birthplace?</option>
                    <option value="Who is your best childhood friend?">Who is your best childhood friend?</option>
                    <option value="What is the name of your first pet?">What is the name of your first pet?</option>
                    <option value="Who is your favorite teacher?">Who is your favorite teacher?</option>
                    <option value="Who is your favorite historical person?">Who is your favorite historical person?</option>
                    <option value="What is your grandfather's occupation?">What is your grandfather's occupation?</option>
                </select>
                <span style="font-size:small; color:Red;display:none" id="spanAnswer1">This field is required</span>
                <span style="font-size:small; color:Red;display:none" id="spanQuestion1">Please choose a different question</span>
            </div>
            <div class="col-sm-5">
                <input type="text" id="txtQ1Answer" data-role="none" class="form-control" placeholder="Answer for question no. 1" maxlength="150" required>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="selQuestion2">Question No. 2: <span style="color:Red">*</span></label>
            <div class="col-sm-5">
                <select id="selQuestion2" data-role="none" class="form-select">
                    <option value="">Please Select</option>
                    <option value="Where is your mother's birthplace?">Where is your mother's birthplace?</option>
                    <option value="Who is your best childhood friend?">Who is your best childhood friend?</option>
                    <option value="What is the name of your first pet?">What is the name of your first pet?</option>
                    <option value="Who is your favorite teacher?">Who is your favorite teacher?</option>
                    <option value="Who is your favorite historical person?">Who is your favorite historical person?</option>
                    <option value="What is your grandfather's occupation?">What is your grandfather's occupation?</option>
                </select>
                <span style="font-size:small; color:Red;display:none" id="spanAnswer2">This field is required</span>
                <span style="font-size:small; color:Red;display:none" id="spanQuestion2">Please choose a different question</span>
            </div>
            <div class="col-sm-5">
                <input type="text" id="txtQ2Answer" data-role="none" class="form-control" placeholder="Answer for question no. 2" maxlength="150" required>
            </div>
        </div>
    </div>

    <div>
        <h1 class="display-6 mb-4">Daily Time Record (DTR) PIN</h1>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtPIN1">Enter PIN:</label>
            <div class="col-md-10">
                <input type="password" data-role="none" class="form-control mb-3" id="txtPIN1" maxlength="5" required>
                <span style="font-size:small; color:Red;display:none" id="spanPIN1">PIN mismatched</span>
                <span style="font-size:small; color:Red;display:none" id="spanPIN12">Enter a 5-digit pin</span>
                <span style="font-size:small; color:Red;display:none;" id="spanPINSequence">For your security, Mobiz does not allow PIN codes that follow forward or backward counting sequences like 12345, 54321, 67890, 02468, 86420, etc. Please enter a 5-digit PIN that is a non-counting sequence. Thank you.</span>
                <span style="font-size:small; color:Red;display:none;" id="spanPINDuplicate">We found a duplicate PIN.  Your sys admin does not allow duplicate PIN codes within your organization. Please enter a different 5-digit PIN and try again. Thank you.</span>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtPIN2">Re-enter PIN:</label>
            <div class="col-md-10">
                <input type="password" data-role="none" class="form-control mb-3" id="txtPIN2" maxlength="5" required>
                <span style="font-size:small; color:Red;display:none" id="spanPIN2">PIN mismatched</span>
            </div>
        </div>
    </div>

    <div class="form-check mb-3" style="font-size:25px;">
        <input class="form-check-input" data-role="none" type="checkbox" value="yes" id="chbCertifiedCorrect" name="certified">
        <label class="form-check-label" for="chbCertifiedCorrect">I certify that the information provided is correct</label>
    </div>

    <input type="hidden" id="hdnPage" value="signUp">

    <span class="mb-3" style="font-size:small; color:Red;display:none;" id="spanAlert">Oops! You might have missed filling in some required information.  Please check the sign up form above and type in the correct information inside data fields with an asterisk (*) sign.</span>

    <div class="mt-4 text-center">
        <a id="btnSignUpCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
        <a id="btnSignUp" data-role="none" class="btn btn-success btn-lg">Save Mobiz Account</a>
    </div>
</div>

<div id="popEmployeeTelInfo" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Telephone Info</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div style="display:none;">
                    <input type="text" data-role="none" class="form-control" readonly id="txtEmployeeTelCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                    <input type="text" data-role="none" class="form-control" readonly id="txtEmployeeTelID" />
                    <input type="text" data-role="none" class="form-control" readonly id="txtEmployeeTelAreaCodeID" />
                </div>

                <div class="mb-3">
                    <label class="form-label" for="btnEmployeeTelCountryCodeDisable">
                        Country Code: <span style="color: #FF0000;">*</span>
                        <span id="spanerrorEmployeeTelCountryCode" style="color:#F00; display:none;">This field is required</span>
                    </label>
                    <button id="btnEmployeeTelCountryCodeDisable" data-role="none" class="btn btn-secondary btn-block"><span id="txtEmployeeCountryCode">PHL - 63</span></button>
                </div>

                <div class="mb-3">
                    <label class="form-label" for="btnEmployeeTelAreaCode">
                        Area Code: <span style="color: #FF0000;">*</span>
                        <span id="spanerrorEmployeeAreaCode" style="color:#F00; display:none;">This field is required</span>
                    </label>
                    <button id="btnEmployeeTelAreaCode" data-role="none" class="btn btn-secondary btn-block"><span id="txtEmployeeAreaCode">Please Select</span></button>
                </div>

                <div class="mb-3">
                    <label class="form-label" for="txtEmployeeTelephoneNum">
                        Telephone Number: <span style="color: #FF0000;">*</span>
                        <span id="spanerrorEmployeeTelephoneNum" style="color:#F00; display:none;">This field is required</span>
                        <span id="spanerrorEmployeeTelephoneNumValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                    </label>
                    <input type="number" data-role="none" class="form-control" id="txtEmployeeTelephoneNum" />
                </div>

                <div class="mb-3">
                    <label class="form-label" for="txtEmployeeTelephoneExt">Extension or Local No.:</label>
                    <input type="text" data-role="none" class="form-control" id="txtEmployeeTelephoneExt" maxlength="10" />
                </div>

                <div class="mb-3">
                    <label class="form-label" for="selEmployeeTelType">Contact Type: <span style="color: #FF0000;">*</span> <span id="spanerrorEmployeeTelType" style="color:#F00; display:none;">This field is required</span></label>
                    <select id="selEmployeeTelType" data-role="none" class="form-select">
                        <option value="">Please Select</option>
                        <option value="Work Telephone">Work Telephone</option>
                        <option value="Personal Telephone">Personal Telephone</option>
                        <option value="Third Party Telephone">Third Party Telephone</option>
                    </select>
                </div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col-6">
                        <button data-role="none" type="button" id="btnCancelEmployeeTelephone" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                    </div>
                    <div class="col-6">
                        <button data-role="none" type="button" id="btnSaveEmployeeTelephone" class="btn btn-success btn-block btn-lg">Ok</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popEmployeeMobileInfo" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Mobile Info</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div style="display:none;">
                    <input type="text" data-role="none" class="form-control" readonly id="txtEmployeeMobileCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                    <input type="text" data-role="none" class="form-control" readonly id="txtEmployeeMobileID" />
                    <input type="text" data-role="none" class="form-control" readonly id="txtEmployeeTelCoPrefixID" />
                </div>

                <div class="mb-3">
                    <label class="form-label" for="btnEmployeeMobileCountryCodeDisable">
                        Country Code: <span style="color: #FF0000;">*</span>
                        <span id="spanerrorEmployeeMobileCountryCode" style="color:#F00; display:none;">This field is required</span>
                    </label>
                    <button id="btnEmployeeMobileCountryCodeDisable" data-role="none" class="btn btn-secondary btn-block"><span id="txtEmployeeMobileCountryCode">PHL - 63</span></button>
                </div>

                @*<div class="mb-3">
                        <label class="form-label" for="btnEmployeeMobileCountryCodeDisable">
                            Tel Code: <span style="color: #FF0000;">*</span>
                            <span id="spanerrorEmployeeMobileTelCode" style="color:#F00; display:none;">This field is required</span>
                        </label>
                        <button id="btnEmployeeMobileTelCode" data-role="none" class="btn btn-secondary btn-block"><span id="txtEmployeeTelCode">Select</span></button>
                    </div>*@

                <div class="mb-3">
                    <label class="form-label" for="txtEmployeeMobileNo">
                        Mobile Number: <span style="color: #FF0000;">*</span>
                        <span id="spanerrorEmployeeMobileNum" style="color:#F00; display:none;">This field is required</span>
                        <span id="spanerrorEmployeeMobileNumValidate" style="color:#F00; display:none;">Enter 10 digit number</span>
                    </label>
                    <input type="number" id="txtEmployeeMobileNo" data-role="none" class="form-control" placeholder="Eg. 91512345678" />
                </div>

                <div class="mb-3">
                    <label class="form-label" for="selEmployeeMobileType">Contact Type: <span style="color: #FF0000;">*</span> <span id="spanerrorEmployeeMobileType" style="color:#F00; display:none;">This field is required</span></label>
                    <select id="selEmployeeMobileType" data-role="none" class="form-select">
                        <option value="">Please Select</option>
                        <option value="Work Mobile">Work Mobile</option>
                        <option value="Personal Mobile">Personal Mobile</option>
                        <option value="Third Party Mobile">Third Party Mobile</option>
                    </select>
                </div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col-6">
                        <button data-role="none" type="button" id="btnCancelEmployeeMobile" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                    </div>
                    <div class="col-6">
                        <button data-role="none" type="button" id="btnSaveEmployeeMobile" class="btn btn-success btn-block btn-lg">Ok</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popEmployeeEmailInfo" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">E-mail Info</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div style="display:none;">
                    <input type="text" data-role="none" class="form-control" readonly id="txtEmployeeEmailID" />
                </div>

                <div class="mb-3">
                    <label class="form-label" for="txtEmployeeEmailAdd">
                        Email Address: <span style="color: #FF0000;">*</span>
                        <span id="spanerrorEmployeeEmailAddress" style="color:#F00; display:none;">This field is required</span>
                        <span id="spanerrorEmployeeEmailAddressValidate" style="color:#F00; display:none;">Enter valid email address</span>
                    </label>
                    <input type="email" id="txtEmployeeEmailAdd" data-role="none" class="form-control" />
                </div>

                <div class="mb-3">
                    <label class="form-label" for="selEmployeeEmailType">Contact Type: <span style="color: #FF0000;">*</span> <span id="spanerrorEmployeeEmailType" style="color:#F00; display:none;">This field is required</span></label>
                    <select id="selEmployeeEmailType" data-role="none" class="form-select">
                        <option value="">Please Select</option>
                        <option value="Work Email">Work Email</option>
                        <option value="Personal Email">Personal Email</option>
                        <option value="Third Party Email">Third Party Email</option>
                    </select>
                </div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col-6">
                        <button data-role="none" type="button" id="btnCancelEmployeeEmailAddress" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                    </div>
                    <div class="col-6">
                        <button data-role="none" type="button" id="btnSaveEmployeeEmailAddress" class="btn btn-success btn-block btn-lg">Ok</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>