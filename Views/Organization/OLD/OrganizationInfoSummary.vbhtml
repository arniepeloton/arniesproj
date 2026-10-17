@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require(['/Scripts/sources/Organization/OrganizationSummary.js']);
    </script>
    
End Section

<div id="pageOrganization" data-role="page">

    <div id"divHeader" data-role="header">
        <h1>Organization Information Summary</h1>
    </div>

    <div data-role="content" >

        <div>
            <ul data-role="listview" data-inset="true">

				<li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Organization Name
                        <a href='OrganizationInfo' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div data-role="fieldcontain">
	                    <input type="text" readonly style="display:none;" id="txtID"/>                
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                        <div><label for="txtCorporateName">Corporate Name: <span style="color: #FF0000;" >*</span></label></div>
	                        <div><input type="text" readonly id="txtCorporateName"/></div>
                        </fieldset>
                    </div>

                    <div>
                        <ul data-role="listview" id="lstTradeNameContainer" data-inset="true">   
				            <li data-role="list-divider">Trade Name List</li>
                            <li>
                                <div>
                                    <ul data-role="listview" id="lstTradeName" data-inset="true">
                                    </ul>
                                </div>
                            </li>
                        </ul>
                    </div>


                    <div data-role="fieldcontain">
	                    <label for="txtOwnershipType" class="select">Ownership Type:</label>
	                    <div><input type="text" readonly id="txtOwnershipType"/></div>              
                    </div>

                    <div data-role="fieldcontain">
                        <label><input type="checkbox" disabled id="chkIsFranchisor" /> Are you a franchisor?</label>
                    </div>

                    <div data-role="fieldcontain">
	                    <label for="txtRegistrationType" class="select">Registration Type:</label>
	                    <div><input type="text" readonly id="txtRegistrationType"/></div>              
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                        <div><label id="lblRegistrationNo" for="txtRegistrationNo">Registration No.: <span style="color: #FF0000;" >*</span></label></div>
	                        <div><input type="text" readonly id="txtRegistrationNo"/></div>  
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup" data-type="horizontal">
	                        <div><label for="txtRegistrationDate" id="lblRegistrationDate">Registrion Date: <span style="color: #FF0000;" >*</span></label></div>
	                        <div><input type="text" readonly id="txtRegistrationDate"/></div>              
                        </fieldset>
                    </div>
    
                    <div data-role="fieldcontain">
	                    <label for="txtTIN" class="select">TIN:</label>
	                    <div><input type="text" readonly id="txtTIN"/></div>              
                    </div>

                    <div>
                        <ul data-role="listview" data-inset="true">
			                <li data-role="list-divider">Complete Principlal Office Address</li>
				            <li>
                                <div data-role="fieldcontain">
                                    <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                                    <div><label for="txtOfficeAddressBuildingNo">House or Building No.</label></div>
	                                    <div><input type="text" readonly id="txtOfficeAddressBuildingNo"/></div>  
                                    </fieldset>
                                </div>

                                <div data-role="fieldcontain">
                                    <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                                    <div><label for="txtOfficeAddressStreet">Street</label></div>
	                                    <div><input type="text" readonly id="txtOfficeAddressStreet"/></div>  
                                    </fieldset>
                                </div>

                                <div data-role="fieldcontain">
                                    <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                                    <div><label for="txtOfficeAddressCity">City or Town:</label></div>
	                                    <div><input type="text" readonly id="txtOfficeAddressCity"/></div>  
                                    </fieldset>
                                </div>

                                <div data-role="fieldcontain">
                                    <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                                    <div><label for="txtOfficeAddressCountry">Country:</label></div>
	                                    <div><input type="text" readonly id="txtOfficeAddressCountry"/></div>  
                                    </fieldset>
                                </div>
				            </li>

			            </ul>
                    </div>

                    <div>
                        <ul data-role="listview" data-inset="true">
			                <li data-role="list-divider">Complete Business Address</li>
				            <li>
                                <div data-role="fieldcontain">
                                    <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                                    <div><label for="txtBusinessAddressBuildingNo">House or Building No.</label></div>
	                                    <div><input type="text" readonly id="txtBusinessAddressBuildingNo"/></div>  
                                    </fieldset>
                                </div>

                                <div data-role="fieldcontain">
                                    <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                                    <div><label for="txtBusinessAddressStreet">Street</label></div>
	                                    <div><input type="text" readonly id="txtBusinessAddressStreet"/></div>  
                                    </fieldset>
                                </div>

                                <div data-role="fieldcontain">
                                    <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                                    <div><label for="txtBusinessAddressCity">City or Town:</label></div>
	                                    <div><input type="text" readonly id="txtBusinessAddressCity"/></div>  
                                    </fieldset>
                                </div>

                                <div data-role="fieldcontain">
                                    <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                                    <div><label for="txtBusinessAddressCountry">Country:</label></div>
	                                    <div><input type="text" readonly id="txtBusinessAddressCountry"/></div>  
                                    </fieldset>
                                </div>
				            </li>

			            </ul>
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                        <div><label for="txtPurpose">Primary Purpose/Activity/Industry Presently Engaged In:</label></div>
	                        <div><textarea readonly id="txtPurpose"></textarea></div>  
                        </fieldset>
                    </div>

                    <div>
                        <ul data-role="listview" id="lstIndustryClassification" data-inset="true" data-split-icon="delete" data-split-theme="d">   
                        </ul>
                    </div>

                    <div data-role="fieldcontain">
	                    <div><label for="txtBusinessScope" class="select">Business Scope:</label></div>  
	                    <div><input type="text" readonly id="txtBusinessScope"/></div>  
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                        <div><label for="txtNoOfRetailOutlet">No. of Retail Outlets:</label></div>
	                        <div><input type="text" readonly id="txtNoOfRetailOutlet"/></div>  
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                        <div><label for="txtNoOfOffice">No. of Offices:</label></div>
	                        <div><input type="text" readonly id="txtNoOfOffice"/></div>  
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                        <div><label for="txtNoOfWarehouse">No. of Warehouses:</label></div>
	                        <div><input type="text" readonly id="txtNoOfWarehouse"/></div>  
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                        <div><label for="txtNoOfEmployees">No. of Employees:</label></div>
	                        <div><input type="text" readonly id="txtNoOfEmployees"/></div>  
                        </fieldset>
                    </div>

                </li>

                <li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Branch List
                        <a href='OrganizationBranch' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div>
                        <ul data-role="listview" id="lstBranch" data-inset="true">
                        </ul>
                    </div>
                </li>

                <li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Contact Information
                        <a href='OrganizationContact' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div>
                        <ul data-role="listview" id="lstOrganizationContact" data-inset="true">
                        </ul>
                    </div>
                </li>

                <li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Intercompany Affiliations
                        <a href='OrganizationAffiliation' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div data-role="fieldcontain">
	                    <input type="text" readonly style="display:none;" id="txtAffiliationID"/>                
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                        <div>Does this organization have a parent or holding company?</div>
                            <div>
                                <select id="selHasParentCompany" data-role="slider">
				                    <option value="0">No</option>
	                                <option value="1">Yes</option>
                                </select>
                            </div>
                        </fieldset>
                    </div>

                    <div id="divAffiliationParentCompanyDetails" style="display:none;">
                        <div data-role="fieldcontain">
                            <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                            <div><label for="txtAffiliationCompanyName">Parent Company:</label></div>
	                            <div><input type="text" readonly id="txtAffiliationCompanyName"/></div>  
                            </fieldset>
                        </div>

                        <div data-role="fieldcontain">
	                        <div><label for="txtAffiliationRegistrationType">Registration Type of Parent Company:</label></div>
	                        <div><input type="text" readonly id="txtAffiliationRegistrationType"/></div>  
                        </div>

                        <div data-role="fieldcontain">
                            <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                            <div><label id="lblAffilationRegistrationNo" for="txtAffiliationRegistrationNo">Registration No. of Parent Company:</label></div>
	                            <div><input type="text" readonly id="txtAffiliationRegistrationNo"/></div>  
                            </fieldset>
                        </div>

                        <div data-role="fieldcontain">
                            <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                            <div><label id="lblAffilationRegistrationDate" for="txtAffiliationRegistrationDate">Registrion Date of Parent Company:</label></div>
	                            <div><input type="text" readonly id="txtAffiliationRegistrationDate"/></div>  
                            </fieldset>
                        </div>

                        <div>
                            <ul data-role="listview" data-inset="true">
			                    <li data-role="list-divider">Address of Parent Company</li>
				                <li>
                                    <div data-role="fieldcontain">
                                        <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                                        <div><label for="txtAffiliationAddressBuildingNo">House or Building No.</label></div>
	                                        <div><input type="text" readonly id="txtAffiliationAddressBuildingNo"/></div>  
                                        </fieldset>
                                    </div>

                                    <div data-role="fieldcontain">
                                        <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                                        <div><label for="txtAffiliationAddressStreet">Street : <span style="color: #FF0000;" >*</span></label></div>
	                                        <div><input type="text" readonly id="txtAffiliationAddressStreet"/></div>  
                                        </fieldset>
                                    </div>

                                    <div data-role="fieldcontain">
                                        <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                                        <div><label for="txtAffiliationAddressCity">City or Town:</label></div>
                                            <div><input type="text" readonly style="display:none;" id="txtAffiliationAddressCityID"/></div>
	                                        <div><input type="text" readonly id="txtAffiliationAddressCity"/></div>  
                                        </fieldset>
                                    </div>

                                    <div data-role="fieldcontain">
                                        <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                                        <div><label for="txtAffiliationAddressCountry">Country:</label></div>
                                            <div><input type="text" readonly style="display:none;" id="txtAffiliationAddressCountryID"/></div>
	                                        <div><input type="text" readonly id="txtAffiliationAddressCountry"/></div>  
                                        </fieldset>
                                    </div>
				                </li>

			                </ul>
                        </div>
                    </div>

                    <div data-role="fieldcontain">
                        <ul data-role="listview" id="lstAffiliatesContainer" data-inset="true">     
                            <li data-role="list-divider">Affiliate or Subsidiary Company</li>
                            <li>
                                <div>
                                    <ul data-role="listview" id="lstAffiliates" data-inset="true">     
                                    </ul>
                                </div>
                            </li>
                        </ul>
                    </div>

                </li>

                <li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Directors, Officers and Stockholders, Partners or Owners
                        <a href='OrganizationOwner' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div>
                        <ul data-role="listview" id="lstOrganizationOwner" data-inset="true">
                        </ul>
                    </div>
                </li>

                <li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Financial Report Information
                        <a href='OrganizationFinancialReport' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                    <label for="txtFinancialFiscalYearEnd" class="select">Fiscal Year End:</label>
	                        <div><input type="text" readonly id="txtFinancialFiscalYearEnd"/></div>  
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                        <div><label for="txtExternalAuditor">Name of External Auditor:</label></div>
	                        <div><input type="text" readonly id="txtExternalAuditor"/></div>  
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                        <div><label for="txtSingningPartnerExternalAuditor">Name of Signing Partner of External Auditor:</label></div>
	                        <div><input type="text" readonly id="txtSingningPartnerExternalAuditor"/></div>  
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                        <div><label for="txtSECAccreditationNoOfAuditor">SEC Accreditation Number of External Auditor (If Applicable):</label></div>
	                        <div><input type="text" readonly id="txtSECAccreditationNoOfAuditor"/></div>  
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                        <label for="txtVATType" class="select">VAT Type:</label>
	                        <div><input type="text" readonly id="txtVATType"/></div>  
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                        <label for="txtBIRTaxGroup" class="select">BIR Tax Group:</label>
	                        <div><input type="text" readonly id="txtBIRTaxGroup"/></div>  
                        </fieldset>
                    </div>

                </li>
                
                <li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Employee List
                        <a href='OrganizationEmployee' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div>
                        <ul data-role="listview" id="lstEmployee" data-inset="true">
                        </ul>
                    </div>
                </li>

                <li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Customer List
                        <a href='OrganizationCustomer' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div>
                        <ul data-role="listview" id="lstCustomer" data-inset="true">
                        </ul>
                    </div>
                </li>

                <li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Supplier List
                        <a href='OrganizationSupplier' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div>
                        <ul data-role="listview" id="lstSupplier" data-inset="true">
                        </ul>
                    </div>
                </li>
                

			</ul>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align:center"> <a id="btnOrgSummaryBack" data-role="button">Done</a> </div>
        </div>
    </div>

</div>

