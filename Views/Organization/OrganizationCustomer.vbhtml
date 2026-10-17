<script>
    require([GetPathFormat('/Scripts/sources/Organization/OrganizationCustomer.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryOrganization.js?crc=')
        ]);
</script>

<div id="newpageAddCustomer" style="display:block;">
    <div>
        <ul data-role="listview" data-inset="true">
            <li data-role="list-divider">
                <span style="font-size:small;">Fields marked with <span style="color:Red"> * </span> are required.</span>
            </li>

            <li>
                <div data-role="fieldcontain">
                    <label for="selCustomerType">Customer Type</label>
                    <select id="selCustomerType">
                        <option value="Organization">Organization</option>
                        <option value="Person">Person</option>
                    </select>
                </div>

                <div id="divCustomerOrg" style="display:block;">
                    <div data-role="fieldcontain">
                        <label for="txtCompanyName">Company Name <span style="color: #FF0000;">*</span></label>
                        <input type="text" id="txtCompanyName" maxlength="255" style="text-align: center" />
                    </div>
                    <div data-role="fieldcontain">
                        <label for="txtCustomerTIN">Tax Identification Number (TIN) <span style="color: #FF0000;">*</span></label>
                        <input type="text" id="txtCustomerTIN" maxlength="16" style="text-align: center" />
                    </div>
                    <div data-role="fieldcontain">
                        <div class="ui-grid-a">
                            <div class="ui-block-a" style="width:23%;">
                                <label for="txtCustomerOrgHouseNo">Address</label>
                            </div>

                            <div class="ui-block-b" style="width:77%;">
                                <input type="text" id="txtCustomerOrgHouseNo" maxlength="255" style="text-align: center" placeholder="House / Building No." />
                                <br /><br />
                                <input type="text" id="txtCustomerOrgAddress" maxlength="255" style="text-align: center" placeholder="Street" />
                                <br /><br />
                                <select id="selOrgCity"></select>
                                <br /><br />
                                <select id="selOrgCountry"></select>
                            </div>
                        </div>
                    </div>
                </div>

                <div id="divCustomerPerson" style="display:none;">
                    <div data-role="fieldcontain">
                        <label for="txtFirstName">First Name <span style="color: #FF0000;">*</span></label>
                        <input type="text" id="txtFirstName" maxlength="255" style="text-align: center" />
                    </div>
                    <div data-role="fieldcontain">
                        <label for="txtMiddleName">Middle Name <span style="color: #FF0000;">*</span></label>
                        <input type="text" id="txtMiddleName" maxlength="255" style="text-align: center" />
                    </div>
                    <div data-role="fieldcontain">
                        <label for="txtSurname">Surname <span style="color: #FF0000;">*</span></label>
                        <input type="text" id="txtSurname" maxlength="255" style="text-align: center" />
                    </div>
                    <div data-role="fieldcontain">
                        <label for="txtCustomerPersonTIN">Tax Identification Number (TIN)</label>
                        <input type="text" id="txtCustomerPersonTIN" maxlength="16" style="text-align: center" />
                    </div>
                </div>
            </li>
        </ul>
        <br /><br />
        <button id="btnCustomerNameSave" type="submit"><h3>Save</h3></button>
        <button id="btnCustomerNameCancel" type="submit"><h3>Cancel</h3></button>
    </div>
</div>

<div id="pageOrgCustomer" style="display:none;">

    <div id"divHeader" data-role="header">
        <h1>Customer List</h1>
    </div>

     <div data-role="header" data-position="fixed1" style="background: none white; border: none">
        <h1>Customers/Clients List</h1>

        <div class="ui-grid-d">
            <div class="ui-block-a" style="width: 10%;">
                <div id="btnHome" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>Home</span>
                </div>
            </div>
            <div class="ui-block-b" style="width: 30%;">
                <div id="btnSystemAdmin" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>System Administration</span>
                </div>
            </div>
            <div class="ui-block-c" style="width: 30%;">
                <div id="btnDataLibraries" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>Data Libraries</span>
                </div>
            </div>
            <div class="ui-block-d" style="width: 30%;">
                <div id="btnCompanyCustomer" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>Customers/Clients</span>
                </div>
            </div>
        </div>

    </div>

    <div data-role="content" >

        <div>
            <ul data-role="listview" id="lstOrgCustomerContainer" data-inset="true">   
				<li data-role="list-divider">Customers/Clients List</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnOrgCustAddCustomer" data-role="button" data-icon="plus" data-inline="true">Add New Customer</a></div>

                    <form id="frmOrgCustomerSearch">
                        <div><input id="txtOrgCustomerSearch" type="search" placeholder="Search Customer/Client" value="" /></div>
                    </form>

                    <div data-role="fieldcontain" id="divOrgCustRecordCountCustomer"></div>
                    <div>
                        <ul data-role="listview" id="lstOrgCustomer" data-inset="true">   
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgCustomerDone" data-role="button"><h3>Go Back to Main</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgCustomerGoToSetupChecklist" data-role="button"><h3>Go to Setup Checklist</h3></a>
            </div>
        </div>

        <div data-role="popup" id="popupAddNewCustomer" class="ui-corner-all" style="padding:20px;">

            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Add New Customer</h3></center>

            <div data-role="content" style="text-align: center; ">

                <label for="selCustomerType">Customer Type</label>
                <select id="selCustomerType">
                    <option value="Organization">Organization</option>
                    <option value="Person">Person</option>
                </select>

                <div id="divCustomerOrg">
                    <label for="txtCompanyName">Company Name <span style="color: #FF0000;">*</span></label>
                    <input type="text" id="txtCompanyName" maxlength="255" style="text-align: center" />

                    <label for="txtCustomerTIN">Tax Identification Number (TIN) <span style="color: #FF0000;">*</span></label>
                    <input type="text" id="txtCustomerTIN" maxlength="16" style="text-align: center" />
                    <label for="txtCustomerOrgHouseNo">Address</label>
                    <input type="text" id="txtCustomerOrgHouseNo" maxlength="255" style="text-align: center" placeholder="House / Building No." />
                    <input type="text" id="txtCustomerOrgAddress" maxlength="255" style="text-align: center" placeholder="Street" />
                    <select id="selOrgCity"></select>
                    <select id="selOrgCountry"></select>

                </div>

                <div id="divCustomerPerson" style="display:none;">
                    <label for="txtFirstName">First Name <span style="color: #FF0000;">*</span></label>
                    <input type="text" id="txtFirstName" maxlength="255" style="text-align: center" />

                    <label for="txtMiddleName">Middle Name <span style="color: #FF0000;">*</span></label>
                    <input type="text" id="txtMiddleName" maxlength="255" style="text-align: center" />

                    <label for="txtSurname">Surname <span style="color: #FF0000;">*</span></label>
                    <input type="text" id="txtSurname" maxlength="255" style="text-align: center" />

                    <label for="txtCustomerPersonTIN">Tax Identification Number (TIN)</label>
                    <input type="text" id="txtCustomerPersonTIN" maxlength="16" style="text-align: center" />
                </div>
                <br />
                <button id="btnCustomerNameSave" type="submit">Save</button>
                <br />
                <span>Please fill in all the required fields (*)</span>
            </div>

        </div>

    </div>

</div>

<div id="pageOrgCustomerInfo" style="display:none;">

    <div data-role="header">
        <h1>Add New Customer</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgCustID"/>                
        </div>

        <div style="display:none;"><input type="text" readonly id="txtOrgCustCustomerID"/></div>
        <div data-role="fieldcontain">
	        <label for="btnOrgCustCustomer"></label>
            <fieldset data-role="controlgroup">
                <legend>Customer:</legend>
                <button id="btnOrgCustCustomer" data-role="button">Please Select</button>
            </fieldset>
        </div>

	    <label></label>
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" data-type="horizontal">
                <legend>Activation Date: <span style="color: #FF0000" >*</span></legend>
                <select id="selOrgCustActivationDateMonth">
	                <option value="">MMM</option>
                </select>
                <select id="selOrgCustActivationDateDay">
	                <option value="">DD</option>
                </select>                
                <select id="selOrgCustActivationDateYear">
	                <option value="">YYYY</option>
                </select>                
            </fieldset>
        </div>

	    <div data-role="fieldcontain">
            <label for="selOrgCustDeactivate"> Deactivate record? </label>
            <select id="selOrgCustDeactivate" data-role="slider">
			    <option value="0">No</option>
	            <option value="1">Yes</option>
            </select>            
        </div>
        

        <div id="divOrgCustDeactivateDetails" style="display:none;">

	        <label>Deactivation Date: <span style="color: #FF0000;" >*</span></label>
            <fieldset data-role="controlgroup" data-type="horizontal">
                <select id="selOrgCustDateInActiveMonth">
	                <option value="">MMM</option>
                </select>
                <select id="selOrgCustDateInActiveDay">
	                <option value="">DD</option>
                </select>                
                <select id="selOrgCustDateInActiveYear">
	                <option value="">YYYY</option>
                </select>                
            </fieldset>

            <div data-role="fieldcontain">
	            <label for="txtOrgCustInActiveReason">Reason for deactivating:</label>
                <fieldset data-role="controlgroup">
	                <textarea id="txtOrgCustInActiveReason"></textarea>
                </fieldset>
            </div>

            <div data-role="fieldcontain">
	            <label for="txtOrgCustComments">Comments:</label>
                <fieldset data-role="controlgroup">
	                <textarea id="txtOrgCustComments"></textarea>
                </fieldset>
            </div>

	        <div>Ban this record for future transactions?</div>
            <select id="selOrgCustBan" data-role="slider">
				<option value="0">No</option>
	            <option value="1">Yes</option>
            </select>

        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgCustInfoSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgCustInfoCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

        <div data-role="popup" id="popupAddNewCustomer" class="ui-corner-all" style="padding:20px;">

            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Add New Customer</h3></center>

            <div data-role="content" style="text-align: center; ">

                <label for="selCustomerType">Customer Type</label>
                <select id="selCustomerType">
                    <option value="Organization">Organization</option>
                    <option value="Person">Person</option>
                </select>

                <div id="divCustomerOrg">
                    <label for="txtCompanyName">Company Name <span style="color: #FF0000;">*</span></label>
                    <input type="text" id="txtCompanyName" maxlength="255" style="text-align: center" />

                    <label for="txtCustomerTIN">Tax Identification Number (TIN) <span style="color: #FF0000;">*</span></label>
                    <input type="text" id="txtCustomerTIN" maxlength="16" style="text-align: center" />
                    <label for="txtCustomerOrgHouseNo">Address</label>
                    <input type="text" id="txtCustomerOrgHouseNo" maxlength="255" style="text-align: center" placeholder="House / Building No." />
                    <input type="text" id="txtCustomerOrgAddress" maxlength="255" style="text-align: center" placeholder="Street" />
                    <select id="selOrgCity"></select>
                    <select id="selOrgCountry"></select>

                </div>

                <div id="divCustomerPerson" style="display:none;">
                    <label for="txtFirstName">First Name <span style="color: #FF0000;">*</span></label>
                    <input type="text" id="txtFirstName" maxlength="255" style="text-align: center" />

                    <label for="txtMiddleName">Middle Name <span style="color: #FF0000;">*</span></label>
                    <input type="text" id="txtMiddleName" maxlength="255" style="text-align: center" />

                    <label for="txtSurname">Surname <span style="color: #FF0000;">*</span></label>
                    <input type="text" id="txtSurname" maxlength="255" style="text-align: center" />

                    <label for="txtCustomerPersonTIN">Tax Identification Number (TIN)</label>
                    <input type="text" id="txtCustomerPersonTIN" maxlength="16" style="text-align: center" />
                </div>
                <br />
                <button id="btnCustomerNameSave" type="submit">Save</button>
                <br />
                <span>Please fill in all the required fields (*)</span>
            </div>

        </div>

    </div>

</div>
