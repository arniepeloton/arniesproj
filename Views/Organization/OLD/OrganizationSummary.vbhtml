@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/Organization/OrganizationSummary.js?crc=')]);
    </script>
    
End Section

<div id="pageOrganization" data-role="page">

 <div data-role="header" data-position="fixed1" style="background: none white; border: none">
    <h1>Organization Information Summary</h1>

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
            <div id="btnSystemSettings" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                <span>System Settings</span>
            </div>
        </div>
        <div class="ui-block-d" style="width: 30%;">
            <div id="btnCompanyProfile" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                <span>Company Profile</span>
            </div>
        </div>
    </div>

</div>


@*    <div id"divHeader" data-role="header">
        <h1>Organization Information Summary</h1>
    </div>
*@
    <div data-role="content" >

        <div>
            <ul data-role="listview" data-inset="true">

				<li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Name
                        <a id='btnEditSumOrgInfo' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div data-role="fieldcontain" style="display:none;">
	                    <input type="text" readonly id="txtSumID"/>                
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup">
	                        <div><label for="txtSumCorporateName">Name: <span style="color: #FF0000;" >*</span></label></div>
	                        <div><input type="text" readonly id="txtSumCorporateName"/></div>
                        </fieldset>
                    </div>

                    <div id="divSumOrgOrganizationInfo">
                        <div>
                            <ul data-role="listview" id="lstSumTradeNameContainer" data-inset="true">   
				                <li data-role="list-divider">Trade Name List</li>
                                <li>
                                    <div>
                                        <ul data-role="listview" id="lstSumTradeName" data-inset="true">
                                        </ul>
                                    </div>
                                </li>
                            </ul>
                        </div>

                        <div data-role="fieldcontain">
	                        <label for="txtSumOwnershipType" class="select">Ownership Type:</label>
	                        <div><input type="text" readonly id="txtSumOwnershipType"/></div>              
                        </div>

                        <div data-role="fieldcontain">
	                        <label for="txtSumRegistrationType" class="select">Registration Type:</label>
	                        <div><input type="text" readonly id="txtSumRegistrationType"/></div>              
                        </div>

                        <div data-role="fieldcontain">
                            <fieldset data-role="controlgroup">
	                            <div><label id="lblSumRegistrationNo" for="txtSumRegistrationNo">Registration No.: <span style="color: #FF0000;" >*</span></label></div>
	                            <div><input type="text" readonly id="txtSumRegistrationNo"/></div>  
                            </fieldset>
                        </div>

                        <div data-role="fieldcontain">
                            <fieldset data-role="controlgroup" data-type="horizontal">
	                            <div><label for="txtSumRegistrationDate" id="lblSumRegistrationDate">Registrion Date: <span style="color: #FF0000;" >*</span></label></div>
	                            <div><input type="text" readonly id="txtSumRegistrationDate"/></div>              
                            </fieldset>
                        </div>
    
                        <div data-role="fieldcontain">
	                        <label for="txtSumTIN" class="select">TIN:</label>
	                        <div><input type="text" readonly id="txtSumTIN"/></div>              
                        </div>

                        <div>
                            <ul data-role="listview" data-inset="true">
			                    <li data-role="list-divider">Complete Principal Office Address</li>
				                <li>
                                    <div data-role="fieldcontain">
                                        <fieldset data-role="controlgroup">
	                                        <div><label for="txtSumOfficeAddressBuildingNo">House or Building No.</label></div>
	                                        <div><input type="text" readonly id="txtSumOfficeAddressBuildingNo"/></div>  
                                        </fieldset>
                                    </div>

                                    <div data-role="fieldcontain">
                                        <fieldset data-role="controlgroup">
	                                        <div><label for="txtSumOfficeAddressStreet">Street</label></div>
	                                        <div><input type="text" readonly id="txtSumOfficeAddressStreet"/></div>  
                                        </fieldset>
                                    </div>

                                    <div data-role="fieldcontain">
                                        <fieldset data-role="controlgroup">
	                                        <div><label for="txtSumOfficeAddressCity">City or Town:</label></div>
	                                        <div><input type="text" readonly id="txtSumOfficeAddressCity"/></div>  
                                        </fieldset>
                                    </div>

                                    <div data-role="fieldcontain">
                                        <fieldset data-role="controlgroup">
	                                        <div><label for="txtSumOfficeAddressCountry">Country:</label></div>
	                                        <div><input type="text" readonly id="txtSumOfficeAddressCountry"/></div>  
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
                                        <fieldset data-role="controlgroup">
	                                        <div><label for="txtSumBusinessAddressBuildingNo">House or Building No.</label></div>
	                                        <div><input type="text" readonly id="txtSumBusinessAddressBuildingNo"/></div>  
                                        </fieldset>
                                    </div>

                                    <div data-role="fieldcontain">
                                        <fieldset data-role="controlgroup">
	                                        <div><label for="txtSumBusinessAddressStreet">Street</label></div>
	                                        <div><input type="text" readonly id="txtSumBusinessAddressStreet"/></div>  
                                        </fieldset>
                                    </div>

                                    <div data-role="fieldcontain">
                                        <fieldset data-role="controlgroup">
	                                        <div><label for="txtSumBusinessAddressCity">City or Town:</label></div>
	                                        <div><input type="text" readonly id="txtSumBusinessAddressCity"/></div>  
                                        </fieldset>
                                    </div>

                                    <div data-role="fieldcontain">
                                        <fieldset data-role="controlgroup">
	                                        <div><label for="txtSumBusinessAddressCountry">Country:</label></div>
	                                        <div><input type="text" readonly id="txtSumBusinessAddressCountry"/></div>  
                                        </fieldset>
                                    </div>
				                </li>

			                </ul>
                        </div>

                        <div data-role="fieldcontain">
                            <fieldset data-role="controlgroup">
	                            <div><label for="txtSumPurpose">Primary Purpose/Activity/Industry Presently Engaged In:</label></div>
	                            <div><textarea readonly id="txtSumPurpose"></textarea></div>  
                            </fieldset>
                        </div>

                        <div>
                            <ul data-role="listview" id="lstSumCategoryContainer" data-inset="true">   
				                <li data-role="list-divider">Category</li>
                                <li>
                                    <div>
                                        <ul data-role="listview" id="lstSumCategory" data-inset="true">
                                        </ul>
                                    </div>
                                </li>
                            </ul>
                        </div>

                        <div>
                            <ul data-role="listview" id="lstSumCategoryContainer" data-inset="true">   
				                <li data-role="list-divider">Industry Classification</li>
                                <li>
                                    <div>
                                        <ul data-role="listview" id="lstSumIndustryClassification" data-inset="true">
                                        </ul>
                                    </div>
                                </li>
                            </ul>
                        </div>

                        <div data-role="fieldcontain">
                            <label><input type="checkbox" disabled id="chkSumIsFranchisor" /> Are you a franchisor?</label>
                        </div>
                        <div id="divSumFranchiseeContainer" style="display:none;">
                            <ul data-role="listview" id="lstSumOrgFranchiseeContainer" data-inset="true">   
				                <li data-role="list-divider">Franchisee List</li>
                                <li>
                                    <div>
                                        <ul data-role="listview" id="lstSumOrgFranchisee" data-inset="true">   
                                        </ul>
                                    </div>
                                </li>
                            </ul>
                        </div>

                        <div data-role="fieldcontain">
	                        <div><label for="txtSumBusinessScope" class="select">Business Scope:</label></div>  
	                        <div><input type="text" readonly id="txtSumBusinessScope"/></div>  
                        </div>

                        <div data-role="fieldcontain">
                            <fieldset data-role="controlgroup">
	                            <div><label for="txtSumNoOfRetailOutlet">No. of Retail Outlets:</label></div>
	                            <div><input type="text" readonly id="txtSumNoOfRetailOutlet"/></div>  
                            </fieldset>
                        </div>

                        <div data-role="fieldcontain">
                            <fieldset data-role="controlgroup">
	                            <div><label for="txtSumNoOfOffice">No. of Offices:</label></div>
	                            <div><input type="text" readonly id="txtSumNoOfOffice"/></div>  
                            </fieldset>
                        </div>

                        <div data-role="fieldcontain">
                            <fieldset data-role="controlgroup">
	                            <div><label for="txtSumNoOfWarehouse">No. of Warehouses:</label></div>
	                            <div><input type="text" readonly id="txtSumNoOfWarehouse"/></div>  
                            </fieldset>
                        </div>

                        <div data-role="fieldcontain">
                            <fieldset data-role="controlgroup">
	                            <div><label for="txtSumNoOfEmployees">No. of Employees:</label></div>
	                            <div><input type="text" readonly id="txtSumNoOfEmployees"/></div>  
                            </fieldset>
                        </div>
                    </div>

                    <div id="divSumOrgPersonInfo" style="display:none;">
                        <div>
                            <ul data-role="listview" data-inset="true">
			                    <li data-role="list-divider">Complete Address</li>
				                <li>
                                    <div data-role="fieldcontain">
                                        <fieldset data-role="controlgroup">
	                                        <div><label for="txtSumOrgPersonBuildingNo">House or Building No.</label></div>
	                                        <div><input type="text" readonly id="txtSumOrgPersonBuildingNo"/></div>  
                                        </fieldset>
                                    </div>

                                    <div data-role="fieldcontain">
                                        <fieldset data-role="controlgroup">
	                                        <div><label for="txtSumOrgPersonAddressStreet">Street</label></div>
	                                        <div><input type="text" readonly id="txtSumOrgPersonAddressStreet"/></div>  
                                        </fieldset>
                                    </div>

                                    <div data-role="fieldcontain">
                                        <fieldset data-role="controlgroup">
	                                        <div><label for="txtSumOrgPersonAddressCity">City or Town:</label></div>
	                                        <div><input type="text" readonly id="txtSumOrgPersonAddressCity"/></div>  
                                        </fieldset>
                                    </div>

                                    <div data-role="fieldcontain">
                                        <fieldset data-role="controlgroup">
	                                        <div><label for="txtSumOrgPersonAddressCountry">Country:</label></div>
	                                        <div><input type="text" readonly id="txtSumOrgPersonAddressCountry"/></div>  
                                        </fieldset>
                                    </div>
				                </li>

			                </ul>
                        </div>
                    </div>

                </li>

                <li id="lstOrganizationBrandsHeader" data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Brands
                        <a id='btnEditSumOrgBrands' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li id="lstOrgSumBrandsDetails">
                    <div>
                        <ul data-role="listview" id="lstSumOrganizationBrand" data-inset="true">
                        </ul>
                    </div>
                </li>

                <li id="lstFinancialReportInfoHeader" data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Financial Report Information
                        <a id='btnEditSumOrgFinancialInfo' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li id="lstFinancialReportInfoDetails">

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup">
	                    <label for="txtSumFinancialFiscalYearEnd" class="select">Fiscal Year End:</label>
	                        <div><input type="text" readonly id="txtSumFinancialFiscalYearEnd"/></div>  
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup">
	                        <div><label for="txtSumExternalAuditor">Name of External Auditor:</label></div>
	                        <div><input type="text" readonly id="txtSumExternalAuditor"/></div>  
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup">
	                        <div><label for="txtSumSingningPartnerExternalAuditor">Name of Signing Partner of External Auditor:</label></div>
	                        <div><input type="text" readonly id="txtSumSingningPartnerExternalAuditor"/></div>  
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup">
	                        <div><label for="txtSumSECAccreditationNoOfAuditor">SEC Accreditation Number of External Auditor (If Applicable):</label></div>
	                        <div><input type="text" readonly id="txtSumSECAccreditationNoOfAuditor"/></div>  
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup">
	                        <label for="txtSumVATType" class="select">VAT Type:</label>
	                        <div><input type="text" readonly id="txtSumVATType"/></div>  
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup">
	                        <label for="txtSumBIRTaxGroup" class="select">BIR Tax Group:</label>
	                        <div><input type="text" readonly id="txtSumBIRTaxGroup"/></div>  
                        </fieldset>
                    </div>

                </li>

                <li id="lstContactInfoHeader" data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Contact Information
                        <a id='btnEditSumOrgContact' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li id="lstContactInfoDetails">
                    <div>
                        <ul data-role="listview" id="lstSumOrganizationContact" data-inset="true">
                        </ul>
                    </div>
                </li>

                <li data-role="list-divider" style="display:none;">
                    <div data-role="fieldcontain" style="height:5px;">
                        Places List (FOWST)
                        <a id='btnEditSumOrgFOWST' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li style="display:none;">
                    <div>
                        <ul data-role="listview" id="lstSumFOWST" data-inset="true">
                        </ul>
                    </div>
                </li>                

                <li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Representative List
                        <a id='btnEditSumOrgRepresentative' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div>
                        <ul data-role="listview" id="lstSumRepresentative" data-inset="true">
                        </ul>
                    </div>
                </li>                

                <li id="lstOwnerInfoHeader" data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Directors, Officers and Stockholders, Partners or Owners
                        <a id='btnEditSumOrgOwner' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li id="lstOwnerInfoDetails">
                    <div>
                        <ul data-role="listview" id="lstSumOrganizationOwner" data-inset="true">
                        </ul>
                    </div>
                </li>
                <li id="lstHoldingCompanyInfoHeader" data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Intercompany Affiliations
                        <a id='btnEditSumOrgAffiliation' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li id="lstHoldingCompanyInfoDetails">
	                <div>Does this organization have a parent or holding company?</div>
                    <select id="selSumOrgHasParentCompany" disabled data-role="slider">
				        <option value="0">No</option>
	                    <option value="1">Yes</option>
                    </select>
                    <div id="divSumOrgAffParentCompanyDetails" style="display:none;">
                        <div data-role="fieldcontain">
                            <fieldset data-role="controlgroup">
	                            <div><label for="txtSumOrgAffHoldingCompany">Parent or Holding Company: <span style="color: #FF0000;" >*</span></label></div>
                                <div><input type="text" readonly id="txtSumOrgAffHoldingCompany"/></div>
                            </fieldset>
                        </div>
                    </div>
                    <div data-role="fieldcontain">
                        <ul data-role="listview" id="lstSumOrgAffiliatesContainer" data-inset="true">     
                            <li data-role="list-divider">Affiliate or Subsidiary Company</li>
                            <li>
                                <div>
                                    <ul data-role="listview" id="lstSumOrgAffiliates" data-inset="true">     
                                    </ul>
                                </div>
                            </li>
                        </ul>
                    </div>
                </li>
                <li id="lstEmployeeInfoHeader" data-role="list-divider" style="display:none;">
                    <div data-role="fieldcontain" style="height:5px;">
                        Employee List
                        <a id='btnEditSumOrgEmployee' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li id="lstEmployeeInfoDetails" style="display:none;">
                    <div>
                        <ul data-role="listview" id="lstSumEmployee" data-inset="true">
                        </ul>
                    </div>
                </li>

                <li id="lstCustomerInfoHeader" data-role="list-divider" style="display:none;">
                    <div data-role="fieldcontain" style="height:5px;">
                        Customer List
                        <a id='btnEditSumOrgCustomer' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li id="lstCustomerInfoDetails" style="display:none;">
                    <div>
                        <ul data-role="listview" id="lstSumCustomer" data-inset="true">
                        </ul>
                    </div>
                </li>

                <li id="lstSupplierInfoHeader" data-role="list-divider" style="display:none;">
                    <div data-role="fieldcontain" style="height:5px;">
                        Supplier List
                        <a id='btnEditSumOrgSupplier' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li id="lstSupplierInfoDetails" style="display:none;">
                    <div>
                        <ul data-role="listview" id="lstSumSupplier" data-inset="true">
                        </ul>
                    </div>
                </li>

			</ul>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgSummaryDone" data-role="button"><h3>Go Back to Main</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgSummaryGoToSetupChecklist" data-role="button"><h3>Go to Setup Checklist</h3></a>
            </div>
        </div>

    </div>

</div>

