<script>
    require([GetPathFormat('/Scripts/sources/OrganizationShort/OrganizationAddress.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryCountries.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryCities.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryAreaCodes.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryTelcoPrefixes.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryOrganization.js?crc=')
        ]);
</script>

<div id="pageOrgFOWSTList" style="display:block;">
    <div data-role="header">
        <h2>Organization Places (FOWST) List</h2>
    </div>

    <div data-role="content">
        <div data-role="fieldcontain"><a id="btnOrgFOWSTAdd" data-role="button" data-icon="plus" data-inline="true">Add a Place</a></div>
        <div data-role="fieldcontain"><span>* A "Place" can be a Factory, Office, Warehouse, Store or Truck (F.O.W.S.T.)</span></div>
        <div data-role="fieldcontain" id="divOrgFOWSTRecordCount"></div>
        <div>
            <ul data-role="listview" id="lstOrgFOWST" data-inset="true">
            </ul>
        </div>
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgFOWSTDone" data-role="button"><h3>Done</h3></a>
            </div>
        </div>
    </div>

</div>

<div id="pageOrgFOWSTInfo" style="display:none;">
    <div data-role="header">
        <h2>Organization Address</h2>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly  id="txtOrgFOWSTID"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="selOrgFOWSTType" class="select">Type:</label>
            <select id="selOrgFOWSTType">
            </select>
        </div>

        <div data-role="fieldcontain">
            <label for="txtOrgFOWSTCode">Code :</label>
	        <input type="text" id="txtOrgFOWSTCode"/>
        
        </div>

        <div data-role="fieldcontain">
           <label for="txtOrgFOWSTName">Name : <span style="color: #FF0000;" >*</span></label>
	       <input type="text" id="txtOrgFOWSTName"/>
           
        </div>

        <div data-role="fieldcontain">
            <label for="txtOrgFOWSTDescription">Description :</label>
	        <input type="text" id="txtOrgFOWSTDescription"/>
     
        </div>

        <div>
            <ul data-role="listview" data-inset="true">
			    <li data-role="list-divider">Complete Address</li>
				<li>
                    <div data-role="fieldcontain">
                        <label for="txtOrgFOWSTBuildingNo">House or Building No. : </label>
	                    <input type="text" id="txtOrgFOWSTBuildingNo"/>  
                    
                    </div>

                    <div data-role="fieldcontain">
                        <label for="txtOrgFOWSTStreet">Street : <span style="color: #FF0000;" >*</span></label>
	                   <input type="text" id="txtOrgFOWSTStreet"/> 
                        
                    </div>

                    <div data-role="fieldcontain">
	                    <label for="btnOrgFOWSTCity">City or Town: <span style="color: #FF0000;" >*</span></label>
                        <div style="display:none;"><input type="text" readonly id="txtOrgFOWSTCityID"/></div>
                        <button id="btnOrgFOWSTCity" data-role="button">Please Select</button>
                    
                    </div>

                    <div data-role="fieldcontain">
	                    <label for="btnOrgFOWSTCountry">Country: <span style="color: #FF0000;" >*</span></label>
                        <div style="display:none;"><input type="text" readonly id="txtOrgFOWSTCountryID"/></div>
                        <button id="btnOrgFOWSTCountry" data-role="button">Please Select</button>
                     
                    </div>
				</li>

			</ul>
        </div>

        <div data-role="fieldcontain">
	        <label for="selOrgAddFranchisee">Is this a franchisee?</label>
            <select id="selOrgAddFranchisee" data-role="slider">
				<option value="0">No</option>
	            <option value="1">Yes</option>
            </select>
        </div>

        <div id="divFranchisorInfo" style="display:none;">
            <div data-role="fieldcontain">
	            <label for="btnOrgFOWSTFranchisor">Franchisor: <span style="color: #FF0000;" >*</span></label>
                <div style="display:none;"><input type="text" readonly id="txtOrgFOWSTFranchisorID"/></div>
                <button id="btnOrgFOWSTFranchisor" data-role="button">Please Select</button>
                
            </div>
        </div>

        <div data-role="fieldcontain">
	        <label for="selOrgFOWSTOwnershipType" class="select">Ownership Type:</label>
            <select id="selOrgFOWSTOwnershipType">
                <option value="0">Owned</option>
                <option value="1">Rented or Leased</option>
            </select>
        </div>

        <div id="divRentedOrLeasedInfo" style="display:none;">

            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup" data-type="horizontal">
	                <div><label>Contract Start Date :</label></div>
                    <div>
                        <select id="selOrgFOWSTContractStartMonth">
	                        <option value="">MMM</option>
                        </select>
                        <select id="selOrgFOWSTContractStartDay">
	                        <option value="">DD</option>
                        </select>                
                        <select id="selOrgFOWSTContractStartYear">
	                        <option value="">YYYY</option>
                        </select>                
                     </div>            
                </fieldset>
            </div>

            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup" data-type="horizontal">
	                <div><label>Contract End Date :</label></div>
                    <div>
                        <select id="selOrgFOWSTContractEndMonth">
	                        <option value="">MMM</option>
                        </select>
                        <select id="selOrgFOWSTContractEndDay">
	                        <option value="">DD</option>
                        </select>                
                        <select id="selOrgFOWSTContractEndYear">
	                        <option value="">YYYY</option>
                        </select>                
                     </div>            
                </fieldset>
            </div>

            <div data-role="fieldcontain">
                <label for="txtOrgFOWSTCostPerMonth">Cost per Squared Meter per Month or Squared Foot per Month :</label>
	            <input type="number" id="txtOrgFOWSTCostPerMonth"/>  
                
            </div>

        </div>

        <div data-role="fieldcontain">
	        <label for="selOrgFOWSTIsMortgaged" class="select">This Asset is Mortgaged?</label>
            <select id="selOrgFOWSTIsMortgaged" data-role="slider" class="ui-slider-switch">
                <option value="0">No</option>
                <option value="1">Yes</option>
            </select>
        </div>

        <div id="divOrgFOWSTWarehouseTypeInfo" style="display:none;">
            <div data-role="fieldcontain">
	            <label for="selOrgFOWSTWarehouseType" class="select">Warehouse Type:</label>
                <select id="selOrgFOWSTWarehouseType">
                </select>
            </div>
        </div>

        <div id="divOrgFOWSTRetailStoreTypeInfo" style="display:none;">
            <div data-role="fieldcontain">
	            <label for="selOrgFOWSTRetailStoreType" class="select">Type of Retail Store:</label>
                <select id="selOrgFOWSTRetailStoreType">
                </select>
            </div>
        </div>

        <div id="divOrgFOWSTDeliveryEquipmentInfo" style="display:none;">
            <div data-role="fieldcontain">
                <label for="txtOrgFOWSTPlateNo">Plate No. : </label>
	            <input type="text" id="txtOrgFOWSTPlateNo"/>
               
            </div>

            <div data-role="fieldcontain">
                <label for="txtOrgFOWSTNoOfWheels">No. of Wheels: </label>
	            <input type="text" id="txtOrgFOWSTNoOfWheels"/> 
                
            </div>

            <div data-role="fieldcontain">
                <label for="txtOrgFOWSTBodyColor">Body Color : </label>
	            <input type="text" id="txtOrgFOWSTBodyColor"/>
               
            </div>

            <div data-role="fieldcontain">
                <label for="txtOrgFOWSTLoadLimitWeight">Load Limit (in weight) : </label>
	            <input type="text" id="txtOrgFOWSTLoadLimitWeight"/>
            </div>

            <div data-role="fieldcontain">
                <label for="txtOrgFOWSTLoadLimitDimensions">Load Limit (in Dimensions) : </label>
	            <input type="text" id="txtOrgFOWSTLoadLimitDimensions"/>
            </div>

            <div>
                <ul data-role="listview" id="lstOrgFOWSTLTOContainer" data-inset="true">   
				    <li data-role="list-divider">LTO Registration Details</li>
                    <li>
                        <div data-role="fieldcontain"><a id="btnAddOrgFOWSTLTO" data-role="button" data-icon="plus" data-inline="true">Add LTO Registration</a></div>
                        <div>
                            <ul data-role="listview" id="lstOrgFOWSTLTO" data-inset="true">
                            </ul>
                        </div>
                    </li>
                </ul>
            </div>

            <div>
                <ul data-role="listview" id="lstOrgFOWSTLTFRBContainer" data-inset="true">   
				    <li data-role="list-divider">LTFRB Registration Details</li>
                    <li>
                        <div data-role="fieldcontain"><a id="btnAddOrgFOWSTLTFRB" data-role="button" data-icon="plus" data-inline="true">Add LTFRB Registration</a></div>
                        <div>
                            <ul data-role="listview" id="lstOrgFOWSTLTFRB" data-inset="true">
                            </ul>
                        </div>
                    </li>
                </ul>
            </div>

        </div>

        <div id="divOrgFOWSTSection" style="display:none;">
            <ul data-role="listview" id="lstOrgFOWSTSectionContainer" data-inset="true">   
			    <li data-role="list-divider">Sections</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnAddOrgFOWSTSections" data-role="button" data-icon="plus" data-inline="true">Add Section</a></div>
                    <div>
                        <ul data-role="listview" id="lstOrgFOWSTSection" data-inset="true">
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div id="divOrgFOWSTBusinessPermit">
            <ul data-role="listview" id="lstOrgFOWSTBusinessPermitContainer" data-inset="true">   
			    <li data-role="list-divider">Business Permit</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnAddOrgFOWSTBusinessPermit" data-role="button" data-icon="plus" data-inline="true">Add Business Permit</a></div>
                    <div>
                        <ul data-role="listview" id="lstOrgFOWSTBusinessPermit" data-inset="true">
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div id="divOrgFOWSTTelephone">
            <ul data-role="listview" id="lstOrgFOWSTTelephoneContainer" data-inset="true">   
			    <li data-role="list-divider">Telephone</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnAddOrgFOWSTTelephone" data-role="button" data-icon="plus" data-inline="true">Add Telephone</a></div>
                    <div>
                        <ul data-role="listview" id="lstOrgFOWSTTelephone" data-inset="true">
                        </ul>
                    </div>
                </li>
            </ul>
        </div>        

        <div id="divOrgFOWSTMobile">
            <ul data-role="listview" id="lstOrgFOWSTMobileContainer" data-inset="true">   
			    <li data-role="list-divider">Mobile</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnAddOrgFOWSTMobile" data-role="button" data-icon="plus" data-inline="true">Add Mobile</a></div>
                    <div>
                        <ul data-role="listview" id="lstOrgFOWSTMobile" data-inset="true">
                        </ul>
                    </div>
                </li>
            </ul>
        </div>        

        <div id="divOrgFOWSTEmail">
            <ul data-role="listview" id="lstOrgFOWSTEmailContainer" data-inset="true">   
			    <li data-role="list-divider">Email</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnAddOrgFOWSTEmail" data-role="button" data-icon="plus" data-inline="true">Add Email</a></div>
                    <div>
                        <ul data-role="listview" id="lstOrgFOWSTEmail" data-inset="true">
                        </ul>
                    </div>
                </li>
            </ul>
        </div>        

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgFOWSTSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgFOWSTCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgFOWSTTruckRegInfo" style="display:none;">

    <div data-role="header">
        <h1>Registration Info</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgFOWSTTRegID"/>                
        </div>

        <div data-role="fieldcontain">
            <label for="txtOrgFOWSTTRegORNo">OR No. : <span style="color: #FF0000;" >*</span></label>
	        <input type="text" id="txtOrgFOWSTTRegORNo"/>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" data-type="horizontal">
	            <div><label>Issued Date :</label></div>
                <div>
                    <select id="selOrgFOWSTTRegIssuedMonth">
	                    <option value="">MMM</option>
                    </select>
                    <select id="selOrgFOWSTTRegIssuedDay">
	                    <option value="">DD</option>
                    </select>                
                    <select id="selOrgFOWSTTRegIssuedYear">
	                    <option value="">YYYY</option>
                    </select>                
                    </div>            
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" data-type="horizontal">
	            <div><label>Expiration Date :</label></div>
                <div>
                    <select id="selOrgFOWSTTRegExpirationMonth">
	                    <option value="">MMM</option>
                    </select>
                    <select id="selOrgFOWSTTRegExpirationDay">
	                    <option value="">DD</option>
                    </select>                
                    <select id="selOrgFOWSTTRegExpirationYear">
	                    <option value="">YYYY</option>
                    </select>                
                    </div>            
            </fieldset>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgFOWSTTRegSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgFOWSTTRegRemove" data-role="button"><h3>Remove</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgFOWSTTRegCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgFOWSTSectionInfo" style="display:none;">

    <div data-role="header">
        <h1>Section Info</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgFOWSTSecID"/>                
        </div>

        <div data-role="fieldcontain">
            <label for="txtOrgFOWSTSecCode">Code : </label>
	        <input type="text" id="txtOrgFOWSTSecCode"/>
        </div>

        <div data-role="fieldcontain">
            <label for="txtOrgFOWSTSecName">Name : <span style="color: #FF0000;" >*</span></label>
	        <input type="text" id="txtOrgFOWSTSecName"/>
        </div>

        <div data-role="fieldcontain">
            <label for="txtOrgFOWSTSecDescription">Description :</label>
	        <input type="text" id="txtOrgFOWSTSecDescription"/>
        </div>

        <div data-role="fieldcontain">
           <label for="txtOrgFOWSTSecFloorArea">Floor Area :</label>
	        <input type="number" id="txtOrgFOWSTSecFloorArea"/>
        </div>

        <div data-role="fieldcontain">
            <label for="txtOrgFOWSTSecSpecFeatures">Special Features :</label>
	        <input type="text" id="txtOrgFOWSTSecSpecFeatures"/>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgFOWSTSecSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgFOWSTSecRemove" data-role="button"><h3>Remove</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgFOWSTSecCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgFOWSTBusinessPermitInfo" style="display:none;">

    <div data-role="header">
        <h1>Business Permit Info</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgFOWSTBPermitRecID"/>                
        </div>

        <div data-role="fieldcontain">
            <label for="txtOrgFOWSTBPermitNo">Business Permit No. : <span style="color: #FF0000;" >*</span></label>
	         <input type="text" id="txtOrgFOWSTBPermitNo"/>
        </div>

        <div data-role="fieldcontain">
            <label for="txtOrgFOWSTBPermitID">Business Permit ID : </label>
	        <input type="text" id="txtOrgFOWSTBPermitID"/>
        </div>

        <div data-role="fieldcontain">
            <label for="txtOrgFOWSTBPermitTradeName">Trade Name : <span style="color: #FF0000;" >*</span></label>
	        <input type="text" id="txtOrgFOWSTBPermitTradeName"/>
        </div>

        <div>
            <ul data-role="listview" data-inset="true">
			    <li data-role="list-divider">Complete Address</li>
				<li>
                    <div data-role="fieldcontain">
                        <label for="txtOrgFOWSTBPermitBuildingNo">House or Building No. : </label>
	                     <input type="text" id="txtOrgFOWSTBPermitBuildingNo"/>
                    </div>

                    <div data-role="fieldcontain">
                        <label for="txtOrgFOWSTBPermitStreet">Street : <span style="color: #FF0000;" >*</span></label>
	                    <input type="text" id="txtOrgFOWSTBPermitStreet"/>
                    </div>

                    <div data-role="fieldcontain">
	                    <div><label for="btnOrgFOWSTBPermitCity">City or Town: <span style="color: #FF0000;" >*</span></label></div>
                        <div style="display:none;"><input type="text" readonly id="txtOrgFOWSTBPermitCityID"/></div>
                        <button id="btnOrgFOWSTBPermitCity" data-role="button">Please Select</button>
                        
                    </div>

                    <div data-role="fieldcontain">
	                    <div><label for="btnOrgFOWSTBPermitCountry">Country: <span style="color: #FF0000;" >*</span></label></div>
                        <div style="display:none;"><input type="text" readonly id="txtOrgFOWSTBPermitCountryID"/></div>
                        <button id="btnOrgFOWSTBPermitCountry" data-role="button">Please Select</button>
                       
                    </div>
				</li>

			</ul>
        </div>

        <div data-role="fieldcontain">
            <label for="txtOrgFOWSTBPermitORNo">Official Receipt No. :</label>
	        <input type="text" id="txtOrgFOWSTBPermitORNo"/>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" data-type="horizontal">
	            <div><label>Date Of Payment:</label></div>
                <select id="selOrgFOWSTBPermitDateOfPaymentMonth">
	                <option value="">MMM</option>
                </select>
                <select id="selOrgFOWSTBPermitDateOfPaymentDay">
	                <option value="">DD</option>
                </select>                
                <select id="selOrgFOWSTBPermitDateOfPaymentYear">
	                <option value="">YYYY</option>
                </select>                
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <label for="txtOrgFOWSTBPermitBPlateNo">Business Plate No.: <span style="color: #FF0000;" >*</span></label>
	        <input type="text" id="txtOrgFOWSTBPermitBPlateNo"/>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" data-type="horizontal">
	            <div><label>Date Issued:</label></div>
                <select id="selOrgFOWSTBPermitBPlateIssuedMonth">
	                <option value="">MMM</option>
                </select>
                <select id="selOrgFOWSTBPermitBPlateIssuedDay">
	                <option value="">DD</option>
                </select>                
                <select id="selOrgFOWSTBPermitBPlateIssuedYear">
	                <option value="">YYYY</option>
                </select>                
            </fieldset>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgFOWSTBPermitSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgFOWSTBPermitRemove" data-role="button"><h3>Remove</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgFOWSTBPermitCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgFOWSTTelInfo" style="display:none;">

    <div data-role="header">
        <h1>Telephone Info</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgFOWSTTelID"/>                
        </div>

        <div data-role="fieldcontain">

            <div class="ui-grid-a">
                <div style="display:none;">
                    <input type="text" readonly id="txtOrgFOWSTTelCountryID"/>
                    <input type="text" readonly id="txtOrgFOWSTTelAreaCodeID"/>
                </div>

                <div class="ui-block-a">
                    <button id="btnOrgFOWSTTelCountryCode" data-role="button">Country Code</button>
                </div>

                <div class="ui-block-b">
                    <button id="btnOrgFOWSTTelAreaCode" data-role="button">Area Code</button>
                </div>
            </div>

        </div>

        <div data-role="fieldcontain">
            <label for="txtOrgFOWSTTelNo">Telephone No.: <span style="color: #FF0000;" >*</span></label>
	        <input type="number" id="txtOrgFOWSTTelNo" maxlength="7"/>  
            
        </div>

        <div data-role="fieldcontain">
            <label for="txtOrgFOWSTTelExtension">Extension or Local No.:</label>
	        <input type="number" id="txtOrgFOWSTTelExtension" maxlength="10"/>
        </div>

        <div data-role="fieldcontain">
	        <label for="selOrgFOWSTTelType" class="select">Contact Type: <span style="color: #FF0000;" >*</span></label>
            <select id="selOrgFOWSTTelType">
	            <option value="">Please Select</option>
	            <option value="Work Telephone">Work Telephone</option>
				<option value="Personal Telephone">Personal Telephone</option>
				<option value="Third Party Telephone">Third Party Telephone</option>
            </select>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgFOWSTTelSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgFOWSTTelRemove" data-role="button"><h3>Remove</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgFOWSTTelCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgFOWSTMobileInfo" style="display:none;">

    <div data-role="header">
        <h1>Mobile Info</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgFOWSTMobileID"/>                
        </div>

        <div data-role="fieldcontain">

            <div class="ui-grid-a">
                <div style="display:none;">
                    <input type="text" readonly id="txtOrgFOWSTMobileCountryID"/>
                    <input type="text" readonly id="txtOrgFOWSTTelCoPrefixID"/>
                </div>

                <div class="ui-block-a">
                    <button id="btnOrgFOWSTMobileCountryCode" data-role="button">Country Code</button>
                </div>

                <div class="ui-block-b">
                    <button id="btnOrgFOWSTMobileTelCoPrefixCode" data-role="button">Telco Code</button>
                </div>
            </div>

        </div>

        <div data-role="fieldcontain">
            <label for="txtOrgFOWSTMobileNo">Mobile No.: <span style="color: #FF0000;" >*</span></label>
	        <input type="number" id="txtOrgFOWSTMobileNo" maxlength="7"/><
        </div>

        <div data-role="fieldcontain">
	        <label for="selOrgAddMobileType" class="select">Contact Type: <span style="color: #FF0000;" >*</span></label>
            <select id="selOrgAddMobileType">
	            <option value="">Please Select</option>
	            <option value="Work Mobile">Work Mobile</option>
				<option value="Personal Mobile">Personal Mobile</option>
				<option value="Third Party Mobile">Third Party Mobile</option>
            </select>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgFOWSTMobileSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgFOWSTMobileRemove" data-role="button"><h3>Remove</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgFOWSTMobileCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgFOWSTEmailInfo" style="display:none;">

    <div data-role="header">
        <h1>Email Info</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgFOWSTEmailID"/>                
        </div>

        <div data-role="fieldcontain">
            <label for="txtOrgFOWSTEmail">Email Address: <span style="color: #FF0000;" >*</span></label>
	        <input type="text" id="txtOrgFOWSTEmail"/>
        </div>

        <div data-role="fieldcontain">
	        <label for="selOrgFOWSTEmailType" class="select">Email Type: <span style="color: #FF0000;" >*</span></label>
            <select id="selOrgFOWSTEmailType">
	            <option value="">Please Select</option>
	            <option value="Work Email">Work Email</option>
				<option value="Personal Email">Personal Email</option>
				<option value="Third Party Email">Third Party Email</option>
            </select>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgFOWSTEmailSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgFOWSTEmailRemove" data-role="button"><h3>Remove</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgFOWSTEmailCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>