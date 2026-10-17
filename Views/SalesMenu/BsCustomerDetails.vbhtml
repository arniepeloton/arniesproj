@*<link rel="stylesheet" href="/Content/bootstrap-5.0.0/css/bootstrap.css">
<link rel="stylesheet" href="/Content/bootstrap-5.0.0/bootstrapfive-icons.css">*@

<nav class="navbar navbar-dark bg-primary sticky-md-top text-center" style="font-size:large; color:#FFF; display:block;">
    Customer Details
</nav>

<div class="container-fluid mt-3">
    <div class="row mb-3">
        <label class="col-md-2">
            Choose Customer:
        </label>
        <div class="col-md-10">
            <div class="input-group">
                <input data-role="none" id="btnClient" type="text" class="form-control" readonly placeholder="Click to select customer">
                <button data-role="none" id="btnAddCustomer" class="btn btn-success"><span class="bi-plus"></span> Add New</button>
            </div>
        </div>
    </div>

    <div id="divCustomerDetailsGasStation" class="mb-3" style="display:none;">
        <div class="row">
            <label class="col-md-2">
                Gas Pump No.:
            </label>
            <div class="col-md-10">
                <select data-role="none" class="form-select" id="selGasPumpNo"></select>
            </div>
        </div>
    </div>    

    <div class="row mb-3 d-none">
        <label class="col-md-2">
            Delivery Method:
        </label>
        <div class="col-md-10">
            <select data-role="none" class="form-select" id="selDeliveryMethod">
                <option value="Dine In">Dine In</option>
                <option value="Take Out">Take Out</option>
                <option value="Deliver (Downtown)">Delivery</option>
                <option value="PickUp">Pick Up</option>
            </select>
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-md-2">
            Order Method:
        </label>
        <div class="col-md-10">
            <select data-role="none" class="form-select" id="selOrderMethod">
                <option value="Walk In">Walk In</option>
                <option value="Phone-In">Phone-In</option>
                <option value="Text-In">Text-In</option>
                <option value="Online Order">Online Order</option>
            </select>
        </div>
    </div>

    <div class="row mb-3 d-none">
        <h4 class="col-md-12">Customer Demographics</h4>
    </div>
    

    <div class="row mb-3 d-none">
        <label class="col-md-2">
            Gender:
        </label>
        <div class="col-md-10">
            <select data-role="none" class="form-select" id="selGender">
                <option value="Boy">Boy</option>
                <option value="Girl">Girl</option>
                <option value="3rd Gender">3rd Gender</option>
                <option value="Family">Family</option>
                <option value="Group">Group</option>
                <option value="NA">NA</option>
            </select>
        </div>
    </div>

    <div class="row mb-3 d-none">
        <label class="col-md-2">
            Age:
        </label>
        <div class="col-md-10">
            <select data-role="none" class="form-select" id="selAge">
                <option value="Kid">Kid</option>
                <option value="Teenager">Teenager</option>
                <option value="20's">20's</option>
                <option value="30's">30's</option>
                <option value="40's">40's</option>
                <option value="50's">50's</option>
                <option value="60's and Up">60's and Up</option>
                <option value="Mixed Ages/ NA">Mixed Ages/ NA</option>
            </select>
        </div>
    </div>

    <div class="row mb-5 d-none">
        <label class="col-md-2">
            Total No of Guest (Diners):
        </label>
        <div class="col-md-10">
            <input data-role="none" class="form-control" type="number" id="lnkTotalNumberOfCustomer" />
        </div>
    </div>

    <button data-role="none" id="btnOkCustomer" class="btn btn-success mb-3 btn-block btn-lg">
        Ok
    </button>
    @*<button data-role="none" id="btnCancelCustomer" class="btn btn-secondary mb-3 btn-block btn-lg">
        Cancel
    </button>*@
</div>


<div data-role="popup" id="popupTotalNumberOfCustomer" class="ui-corner-all" style="padding:20px; display:none">

    <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
    <center><h3>Guest Count</h3></center>

    <div data-role="content" style="text-align: center; ">
        <label for="txtTotalNumberOfCustomer">No. of Guest(Dinners)<span style="color: #FF0000;">*</span></label>
        <input type="number" id="txtTotalNumberOfCustomer" maxlength="3" style="text-align: center" value="1" />
    </div>

    <button id="btnSaveTotalNumberOfCustomer" type="submit">Save</button>

</div>

<div id="popupAddNewCustomer" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Add New Customer</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div class="mb-3">Please fill in all the required fields (<span style="color: #FF0000;">*</span>)</div>

                <div class="mb-3">
                    <label for="selCustomerType" class="form-label">Customer Type:</label>
                    <select id="selCustomerType" data-role="none" class="form-select">
                        <option value="Organization">Organization</option>
                        <option value="Person">Person</option>
                    </select>
                </div>

                <div id="divCustomerOrg">
                    <div class="mb-3">
                        <label for="txtCompanyName" class="form-label">Company Name: <span style="color: #FF0000;">*</span></label>
                        <input data-role="none" class="form-control" type="text" id="txtCompanyName" maxlength="255" />
                    </div>

                    <div class="mb-3">
                        <label for="txtCustomerTIN" class="form-label">Tax Identification Number (TIN): <span style="color: #FF0000;">*</span></label>
                        <input data-role="none" class="form-control" type="text" id="txtCustomerTIN" maxlength="17" />
                    </div>

                    <div class="mb-3">
                        <label for="txtCustomerTIN" class="form-label">Business Style: <span style="color: #FF0000;">*</span></label>
                        <input data-role="none" class="form-control" type="text" id="txtCompanyBusinessStyle" />
                    </div>

                    <div class="mb-3">
                        <label for="txtCustomerOrgHouseNo" class="form-label">Address</label>
                        <input data-role="none" class="form-control mb-3" type="text" id="txtCustomerOrgHouseNo" placeholder="House / Building No." />
                        <input data-role="none" class="form-control mb-3" type="text" id="txtCustomerOrgAddress" placeholder="Street" />
                        <select id="selOrgCity" data-role="none" class="form-select mb-3"></select>
                        <select id="selOrgCountry" data-role="none" class="form-select mb-3"></select>
                    </div>
                </div>

                <div id="divCustomerPerson" style="display:none;">
                    <div class="mb-3">
                        <label for="txtCustomerPersonFirstName" class="form-label">First Name: <span style="color: #FF0000;">*</span></label>
                        <input data-role="none" class="form-control" type="text" id="txtCustomerPersonFirstName" maxlength="255" />
                    </div>

                    <div class="mb-3">
                        <label for="txtCustomerPersonMiddleName" class="form-label">Middle Name: <span style="color: #FF0000;">*</span></label>
                        <input data-role="none" class="form-control" type="text" id="txtCustomerPersonMiddleName" maxlength="255" />
                    </div>

                    <div class="mb-3">
                        <label for="txtCustomerPersonSurname" class="form-label">Surname: <span style="color: #FF0000;">*</span></label>
                        <input data-role="none" class="form-control" type="text" id="txtCustomerPersonSurname" maxlength="255" />
                    </div>

                    <div class="mb-3">
                        <label for="txtCustomerPersonTIN" class="form-label">Tax Identification Number (TIN)</label>
                        <input data-role="none" class="form-control" type="text" id="txtCustomerPersonTIN" maxlength="16" />
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col"><button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-lg btn-block">Cancel</button></div>
                    <div class="col"><button data-role="none" id="btnCustomerNameSave" class="btn btn-success btn-lg btn-block">Ok</button></div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popupTableNo" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Dine In</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div id="divCustomerDetailsRestaurant" class="mb-3">
                    <label for="selTableNo" class="form-label">Select Table No.:</label>
                    <select id="selTableNo" data-role="none" class="form-select"></select>
                </div>
            </div>
            <div class="modal-footer">
                <button data-role="none" id="btnTableNoSave" class="btn btn-success btn-block">Ok</button>
                <button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-block">Cancel</button>
            </div>
        </div>
    </div>
</div>