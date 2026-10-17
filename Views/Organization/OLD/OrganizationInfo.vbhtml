@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/Organization/OrganizationInfo.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryCities.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryCountries.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryOrganizationCategory.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryIndustryClassification.js?crc=')
        ]);
    </script>
    
End Section

<div id="pageOrganizationInfo" data-role="page">

    <div id"divHeader" data-role="header">
        <h1>Organization Information</h1>
    </div>

    <div data-role="content" >

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgID"/>                
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
                <legend>Choose a customer type:</legend>
     	        <input type="radio" name="rdoOrgType" id="rdoOrgOrganization" value="1" checked="checked" />
     	        <label for="rdoOrgOrganization">Organization</label>

     	        <input type="radio" name="rdoOrgType" id="rdoOrgPerson" value="0"  />
     	        <label for="rdoOrgPerson">Person</label>

            </fieldset>
        </div>

        <div id="divOrgOrganization">
            <div data-role="fieldcontain">
	            <label for="txtOrgCorporateName">Corporate Name: <span style="color: #FF0000;" >*</span></label>
	            <input type="text" id="txtOrgCorporateName"/>

            </div>

            <div data-role="fieldcontain">
	            <label for="txtOrgCorporateNameInitial">Corporate Name Initials: <span style="color: #FF0000;" >*</span></label>
                <input type="text" id="txtOrgCorporateNameInitial"/>
            </div>

            <div>
                <ul data-role="listview" id="lstOrgTradeNameContainer" data-inset="true">   
				    <li data-role="list-divider">Trade Name List</li>
                    <li>
                        <div data-role="fieldcontain"><a id="btnAddOrgTradeName" data-role="button" data-icon="plus" data-inline="true">Add Business or Trade Name</a></div>
                        <div>
                            <ul data-role="listview" id="lstOrgTradeName" data-inset="true">
                            </ul>
                        </div>
                    </li>
                </ul>
            </div>

            <div data-role="fieldcontain">
	            <label for="selOrgOwnershipType" class="select">Ownership Type:</label>
                <select id="selOrgOwnershipType">
	                <option value="">Please Select</option>
				    <option value="Sole Proprietorship">Sole Proprietorship</option>
				    <option value="Partnership">Partnership</option>
				    <option value="Corporation">Corporation</option>
				    <option value="Cooperative">Cooperative</option>
				    <option value="GOCC">GOCC</option>
	                <option value="Government Institution">Government Institution</option>
                </select>
            </div>

            <div data-role="fieldcontain">
	            <label for="selOrgRegistrationType" class="select">Registration Type: <span style="color: #FF0000;" >*</span></label>
                <select id="selOrgRegistrationType">
	                <option value="">Please Select</option>
				    <option value="SEC">SEC</option>
				    <option value="DTI">DTI</option>
				    <option value="CDA">CDA</option>
				    <option value="DOLE">DOLE</option>
				    <option value="Unregistered">Unregistered</option>
                </select>
            </div>

            <div data-role="fieldcontain">
	            <label id="lblOrgRegistrationNo" for="txtOrgRegistrationNo">Registration No.: <span style="color: #FF0000;" >*</span></label>
                <input type="text" id="txtOrgRegistrationNo"/>
                
            </div>

            <div data-role="fieldcontain">
	            <label id="lblOrgRegistrationDate">Registrion Date: <span style="color: #FF0000;" >*</span></label>
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <select id="selOrgRegistrationDateMonth">
	                    <option value="">MMM</option>
                    </select>
                    <select id="selOrgRegistrationDateDay">
	                    <option value="">DD</option>
                    </select>                
                    <select id="selOrgRegistrationDateYear">
	                    <option value="">YYYY</option>
                    </select>                
                </fieldset>
            </div>

            <div data-role="fieldcontain">
	            <label for="txtOrgCorporateTIN">Corporate Tax Identification No. (TIN): <span style="color: #FF0000;" >*</span></label>
                <input type="text" id="txtOrgCorporateTIN"/>

            </div>

            <div>
                <ul data-role="listview" data-inset="true">
			        <li data-role="list-divider">Complete Principlal Office Address</li>
				    <li>
                        <div data-role="fieldcontain">
	                        <label for="txtOrgOfficeAddressBuildingNo">House or Building No. : </label>
                            <input type="text" id="txtOrgOfficeAddressBuildingNo"/>
                            
                        </div>

                        <div data-role="fieldcontain">
	                        <label for="txtOrgOfficeAddressStreet">Street : <span style="color: #FF0000;" >*</span></label>
                            <input type="text" id="txtOrgOfficeAddressStreet"/>
                   
                        </div>

                        <div style="display:none;"><input type="text" readonly id="txtOrgOfficeAddressCityID"/></div>
                        <div data-role="fieldcontain">
	                        <label for="btnOrgOfficeAddressCity">City or Town: <span style="color: #FF0000;" >*</span></label>
                            <button id="btnOrgOfficeAddressCity" data-role="button">Please Select</button>
                           
                        </div>

                        <div style="display:none;"><input type="text" readonly id="txtOrgOfficeAddressCountryID"/></div>
                        <div data-role="fieldcontain">
	                        <label for="btnOrgOfficeAddressCountry">Country: <span style="color: #FF0000;" >*</span></label>
                            <button id="btnOrgOfficeAddressCountry" data-role="button">Please Select</button>
         
                        </div>
				    </li>

			    </ul>
            </div>

            <div>
                <ul data-role="listview" data-inset="true">
			        <li data-role="list-divider">Complete Business Address</li>
				    <li>

                        <div data-role="fieldcontain">
                            <label><input type="checkbox" id="chkOrgSameAsPricipalAddress" /> Same as Principal Address</label>
                        </div>

                        <div data-role="fieldcontain">
	                        <label for="txtOrgBusinessAddressBuildingNo">House or Building No.</label>
                            <input type="text" id="txtOrgBusinessAddressBuildingNo"/>
                        
                        </div>

                        <div data-role="fieldcontain">
	                        <label for="txtOrgBusinessAddressStreet">Street : <span style="color: #FF0000;" >*</span></label>
                            <input type="text" id="txtOrgBusinessAddressStreet"/>
               
                        </div>

                        <div style="display:none;"><input type="text" readonly id="txtOrgBusinessAddressCityID"/></div>
                        <div data-role="fieldcontain">
	                        <label for="btnOrgBusinessAddressCity">City or Town: <span style="color: #FF0000;" >*</span></label>
                            <button id="btnOrgBusinessAddressCity" data-role="button">Please Select</button>
                 
                        </div>

                        <div style="display:none;"><input type="text" readonly id="txtOrgBusinessAddressCountryID"/></div>
                        <div data-role="fieldcontain">
	                        <label for="btnOrgBusinessAddressCountry">Country: <span style="color: #FF0000;" >*</span></label>
                            <button id="btnOrgBusinessAddressCountry" data-role="button">Please Select</button>
                         
                        </div>
				    </li>

			    </ul>
            </div>

            <div data-role="fieldcontain">
	            <label for="txtOrgPurpose">Primary Purpose/Activity/Industry Presently Engaged In:</label>
                <textarea id="txtOrgPurpose"></textarea>
     
            </div>

            <div>
                <ul data-role="listview" id="lstOrgCategoryContainer" data-inset="true">   
				    <li data-role="list-divider">Category List</li>
                    <li>
                        <div data-role="fieldcontain"><a id="btnAddOrgCategory" data-role="button" data-icon="plus" data-inline="true">Add Category</a></div>
                        <div>
                            <ul data-role="listview" id="lstOrgCategory" data-inset="true" data-split-icon="delete" data-split-theme="d">
                            </ul>
                        </div>
                    </li>
                </ul>
            </div>

            <div>
                <ul data-role="listview" id="lstOrgIndustryClassificationContainer" data-inset="true">   
				    <li data-role="list-divider">Industry Classification List</li>
                    <li>
                        <div data-role="fieldcontain"><a id="btnAddOrgIndustryClassification" data-role="button" data-icon="plus" data-inline="true">Add Industry Classification</a></div>
                        <div>
                            <ul data-role="listview" id="lstOrgIndustryClassification" data-inset="true" data-split-icon="delete" data-split-theme="d">   
                            </ul>
                        </div>
                    </li>
                </ul>
            </div>

            <div data-role="fieldcontain">
                <label><input type="checkbox" id="chkOrgIsFranchisor" /> Are you a franchisor?</label>
             
            </div>

            <div id="divFranchiseeContainer" style="display:none;">
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
               <br />
            <div data-role="fieldcontain">
	            <label for="selOrgBusinessScope" class="select">Business Scope:</label>
                <select id="selOrgBusinessScope">
	                <option value="">Please Select</option>
				    <option value="Multinational">Multinational</option>
				    <option value="National">National</option>
				    <option value="Regional">Regional</option>
				    <option value="City or Municipality">City or Municipality</option>
				    <option value="Barangay">Barangay</option>
                </select>
            </div>

            <div data-role="fieldcontain">
	            <label for="txtOrgNoOfRetailOutlet">No. of Retail Outlets:</label>
                <input type="number" id="txtOrgNoOfRetailOutlet"/>
    
            </div>

            <div data-role="fieldcontain">
	            <label for="txtOrgNoOfOffice">No. of Offices:</label>
                <input type="number" id="txtOrgNoOfOffice"/>
         
            </div>

            <div data-role="fieldcontain">
	            <label for="txtOrgNoOfWarehouse">No. of Warehouses:</label>
                <input type="number" id="txtOrgNoOfWarehouse"/>
      
            </div>

            <div data-role="fieldcontain">
	            <label for="txtOrgNoOfEmployees">No. of Employees:</label>
                <input type="number" id="txtOrgNoOfEmployees"/>
         
            </div>

        </div>

        <div id="divOrgPerson" style="display:none;">

            <div style="display:none;"><input type="text" readonly id="txtOrgTitleID"/></div>
            <div data-role="fieldcontain">
	            <label for="btnOrgTitle">Title:</label>
                <button id="btnOrgTitle" data-role="button">Please Select</button>
      
            </div>

            <div data-role="fieldcontain">
	            <label for="txtOrgSurname">Surname (Last Name): <span style="color: #FF0000;" >*</span></label>
                <input type="text" id="txtOrgSurname"/>
            </div>

            <div data-role="fieldcontain">
	            <label for="txtOrgFirstName">First Name: <span style="color: #FF0000;" >*</span></label>
                <input type="text" id="txtOrgFirstName"/>
          
            </div>

            <div data-role="fieldcontain">
	            <label for="txtOrgMiddleName">Full Middle Name: <span style="color: #FF0000;" >*</span></label>
                <input type="text" id="txtOrgMiddleName"/>
            
            </div>

            <div data-role="fieldcontain">
	            <label for="selOrgPersonalSuffixName" class="select">Generational Suffix (Name):</label>
                <select id="selOrgPersonalSuffixName">
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

            <div data-role="fieldcontain">
	            <label>Date Of Birth: <span style="color: #FF0000;" >*</span></label>
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <select id="selOrgDOBMonth">
	                    <option value="">MMM</option>
                    </select>
                    <select id="selOrgDOBDay">
	                    <option value="">DD</option>
                    </select>                
                    <select id="selOrgDOBYear">
	                    <option value="">YYYY</option>
                    </select>                
                </fieldset>
            </div>

            <div>
                <ul data-role="listview" data-inset="true">
			        <li data-role="list-divider">Mother's Maiden Name</li>
				    <li>
                        <div data-role="fieldcontain">
	                        <label for="txtOrgMotherMaidenSurname">Maiden Surname (Last Name): <span style="color: #FF0000;" >*</span></label>
                            <input type="text" id="txtOrgMotherMaidenSurname"/>
                       
                        </div>

                        <div data-role="fieldcontain">
	                        <label for="txtOrgMotherMaidenFirstName">First Name: <span style="color: #FF0000;" >*</span></label>
                            <input type="text" id="txtOrgMotherMaidenFirstName"/>
                    
                        </div>

                        <div data-role="fieldcontain">
	                        <label for="txtOrgMotherMaidenMiddleName">Maiden Middle Name:</label>
                            <input type="text" id="txtOrgMotherMaidenMiddleName"/>
                         
                        </div>
				    </li>

			    </ul>
            </div>

            <div>
                <ul data-role="listview" data-inset="true">
			        <li data-role="list-divider">Complete Address</li>
				    <li>
                        <div data-role="fieldcontain">
	                        <label for="txtOrgPersonAddressBuildingNo">House or Building No. : </label>
                            <input type="text" id="txtOrgPersonAddressBuildingNo"/>
                         
                        </div>

                        <div data-role="fieldcontain">
	                        <label for="txtOrgPersonAddressStreet">Street : </label>
                            <input type="text" id="txtOrgPersonAddressStreet"/>
                      
                        </div>

                        <div style="display:none;"><input type="text" readonly id="txtOrgPersonAddressCityID"/></div>
                        <div data-role="fieldcontain">
	                        <label for="btnOrgPersonAddressCity">City or Town: </label>
                            <button id="btnOrgPersonAddressCity" data-role="button">Please Select</button>
                        
                        </div>

                        <div style="display:none;"><input type="text" readonly id="txtOrgPersonAddressCountryID"/></div>
                        <div data-role="fieldcontain">
	                        <label for="btnOrgPersonAddressCountry">Country: </label>
                             <button id="btnOrgPersonAddressCountry" data-role="button">Please Select</button>
                            
                        </div>
				    </li>

			    </ul>
            </div>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgInfoDone" data-role="button"><h3>Done</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgTradeName" data-role="page">

    <div data-role="header">
        <h1>Trade Name</h1>
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgTradeNameID"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="txtOrgBusinessTradeName">Business or Trade Name: <span style="color: #FF0000;" >*</span></label>
	        <input type="text" id="txtOrgBusinessTradeName"/>
        </div>

        <div data-role="fieldcontain">
	        <label for="txtOrgBusinessTradeNameInitial">Business or Trade Name Initials: <span style="color: #FF0000;" >*</span></label>
            <input type="text" id="txtOrgBusinessTradeNameInitial"/>
        </div>


        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgTradeNameSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center;">
                <a id="btnOrgTradeNameRemove" data-role="button"><h3>Remove</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgTradeNameCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>
