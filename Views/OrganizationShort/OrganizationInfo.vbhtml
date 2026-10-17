<script>
    require([GetPathFormat('/Scripts/sources/OrganizationShort/OrganizationInfo.js?crc='),
                GetPathFormat('/Scripts3/Browser/LibraryCities.js?crc='),
                GetPathFormat('/Scripts3/Browser/LibraryCountries.js?crc='),
                GetPathFormat('/Scripts3/Browser/LibraryOrganizationCategory.js?crc='),
                GetPathFormat('/Scripts3/Browser/LibraryPeoplePreSuffix.js?crc=')
        ]);
</script>

<div id="pageOrganizationInfo" style="display:block;">

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
                <label for="txtOrgCorporateTIN">Corporate Tax Identification No. (TIN): <span style="color: #FF0000;" >*</span></label>
	             <input type="text" id="txtOrgCorporateTIN"/>  
           
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
                <label><input type="checkbox" id="chkOrgIsFranchisor" /> Are you a franchisor?</label>
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

                        <div data-role="fieldcontain">
	                        <div><label for="btnOrgOfficeAddressCity">City or Town: <span style="color: #FF0000;" >*</span></label></div>
                            <div style="display:none;"><input type="text" readonly id="txtOrgOfficeAddressCityID"/></div>
                            <button id="btnOrgOfficeAddressCity" data-role="button">Please Select</button>
                            
                        </div>

                        <div data-role="fieldcontain">
	                        <div><label for="btnOrgOfficeAddressCountry">Country: <span style="color: #FF0000;" >*</span></label></div>
                            <div style="display:none;"><input type="text" readonly id="txtOrgOfficeAddressCountryID"/></div>
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

                        <div data-role="fieldcontain">
	                        <div><label for="btnOrgBusinessAddressCity">City or Town: <span style="color: #FF0000;" >*</span></label></div>
                            <div style="display:none;"><input type="text" readonly id="txtOrgBusinessAddressCityID"/></div>
                            <button id="btnOrgBusinessAddressCity" data-role="button">Please Select</button>
                         
                        </div>

                        <div data-role="fieldcontain">
	                        <div><label for="btnOrgBusinessAddressCountry">Country: <span style="color: #FF0000;" >*</span></label></div>
                            <div style="display:none;"><input type="text" readonly id="txtOrgBusinessAddressCountryID"/></div>
                           <button id="btnOrgBusinessAddressCountry" data-role="button">Please Select</button>
                     
                        </div>
				    </li>

			    </ul>
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

        </div>

        <div id="divOrgPerson" style="display:none;">

            <div data-role="fieldcontain">
	            <div><label for="btnOrgTitle">Title:</label></div>
                <div style="display:none;"><input type="text" readonly id="txtOrgTitleID"/></div>
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
                <fieldset data-role="controlgroup" data-type="horizontal">
	                <div><label>Date Of Birth: </label></div>
                    <div>
                        <select id="selOrgDOBMonth">
	                        <option value="">MMM</option>
                        </select>
                        <select id="selOrgDOBDay">
	                        <option value="">DD</option>
                        </select>                
                        <select id="selOrgDOBYear">
	                        <option value="">YYYY</option>
                        </select>                
                     </div>            
                </fieldset>
            </div>

            <div>
                <ul data-role="listview" data-inset="true">
			        <li data-role="list-divider">Mother's Maiden Name</li>
				    <li>
                        <div data-role="fieldcontain">
                            <label for="txtOrgMotherMaidenSurname">Maiden Surname (Last Name): </label>
	                       <input type="text" id="txtOrgMotherMaidenSurname"/>  
                            
                        </div>

                        <div data-role="fieldcontain">
                            <label for="txtOrgMotherMaidenFirstName">First Name: </label>
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

                        <div data-role="fieldcontain">
	                        <div><label for="btnOrgPersonAddressCity">City or Town: </label></div>
                            <div style="display:none;"><input type="text" readonly id="txtOrgPersonAddressCityID"/></div>
                            <button id="btnOrgPersonAddressCity" data-role="button">Please Select</button>
                           
                        </div>

                        <div data-role="fieldcontain">
	                        <div><label for="btnOrgPersonAddressCountry">Country: </label></div>
                            <div style="display:none;"><input type="text" readonly id="txtOrgPersonAddressCountryID"/></div>
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
                <a id="btnOrgInfoDone" data-role="button"><h3>Save</h3></a>
                <a id="btnOrgInfoCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgTradeName" style="display:none;">

    <div data-role="header">
        <h1>Trade Name</h1>
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
            <div style="text-align: center">
                <a id="btnOrgTradeNameRemove" data-role="button"><h3>Remove</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgTradeNameCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>
