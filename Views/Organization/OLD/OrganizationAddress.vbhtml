@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
    <script>
        require([GetPathFormat('/Scripts/sources/Organization/OrganizationAddress.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryCountries.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryCities.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryAreaCodes.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryTelcoPrefixes.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryOrganization.js?crc=')
        ]);
    </script>

End Section

<div id="pageOrgFOWSTList" data-role="page">
@*    <div data-role="header">
        <h1>Organization FOWST List</h1>
    </div>
*@
     <div data-role="header" data-position="fixed1" style="background: none white; border: none">
        <h1>Organization Places (FOWST) List</h1>

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
                <div id="btnCompanyFOWST" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>FOWST</span>
                </div>
            </div>
        </div>

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
                <a id="btnOrgFOWSTDone" data-role="button"><h3>Go Back to Main</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgFOWSTGoToSetupChecklist" data-role="button"><h3>Go to Setup Checklist</h3></a>
            </div>
        </div>
    </div>

</div>

<div id="pageOrgFOWSTInfo" data-role="page">
    <div data-role="header">
        <h1>Organization Address</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgFOWSTID"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="selOrgFOWSTType" class="select">Type:</label>
            <select id="selOrgFOWSTType">
            </select>
        </div>

        <div data-role="fieldcontain">
            <label for="txtOrgFOWSTCode">Internal Code: <span>("Internal Code" pertains to your company's short name or code name for this FOWST)</span></label>
            
            <input type="text" id="txtOrgFOWSTCode"/>
        </div>

        <div data-role="fieldcontain">
	        <label for="txtOrgFOWSTName">FOWST Full Name : <span style="color: #FF0000;" >*</span></label>
            <input type="text" id="txtOrgFOWSTName"/>
        </div>

        <div data-role="fieldcontain">
	        <label for="txtOrgFOWSTDescription">Description :</label>
            <input type="text" id="txtOrgFOWSTDescription"/>
 
        </div>

        <div data-role="fieldcontain">            
	        <label for="selOrgFOWSTTradeName">Business or Trade Name: <span style="color: #FF0000;" >*</span></label>                
            <select id="selOrgFOWSTTradeName"> </select>
        </div>
         <div data-role="fieldcontain">
            <label for="txtTINBranchCode">TIN Branch Code (<i>Last 4 digits of the TIN to be reflected in the receipt</i>) :</label>
            
            <input type="text" id="txtTINBranchCode" maxlength = "20"/>
        </div>
        <div data-role="fieldcontain">
            <label for="txtOrgFOWSTVATRate">VAT Rate (%) (<i>This is the VAT Rate applicable to location of this FOWST</i>) :</label>
           
            <input type="text" readonly id="txtOrgFOWSTVATRate"/>

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

                    <div style="display:none;"><input type="text" readonly id="txtOrgFOWSTCityID"/></div>
                    <div data-role="fieldcontain">
	                    <label for="btnOrgFOWSTCity">City or Town: <span style="color: #FF0000;" >*</span></label>
                        <button id="btnOrgFOWSTCity" data-role="button">Please Select</button>
               
                    </div>

                    <div style="display:none;"><input type="text" readonly id="txtOrgFOWSTCountryID"/></div>
                    <div data-role="fieldcontain">
	                    <label for="btnOrgFOWSTCountry">Country: <span style="color: #FF0000;" >*</span></label>
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

        <div style="display:none;"><input type="text" readonly id="txtOrgFOWSTFranchisorID"/></div>
        <div id="divFranchisorInfo" style="display:none;">
	        <label for="btnOrgFOWSTFranchisor">Franchisor: <span style="color: #FF0000;" >*</span></label>
            <div data-role="fieldcontain">
                <button id="btnOrgFOWSTFranchisor" data-role="button">Please Select</button>
       
            </div>
            <div id="divFranchiseeConfirmed">
             <br />
            <strong>Note:</strong> A request will be sent to the franchisor administrator for approval.
           </div>
        </div>
         <br />
        <div data-role="fieldcontain">
	        <label for="selOrgFOWSTOwnershipType" class="select">Ownership of Place:</label>
            <select id="selOrgFOWSTOwnershipType">
                <option value="0">Owned</option>
                <option value="1">Rented or Leased</option>
            </select>
        </div>

        <div id="divRentedOrLeasedInfo" style="display:none;">

            <div data-role="fieldcontain">
	            <label>Contract Start Date :</label>
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <select id="selOrgFOWSTContractStartMonth">
	                    <option value="">MMM</option>
                    </select>
                    <select id="selOrgFOWSTContractStartDay">
	                    <option value="">DD</option>
                    </select>                
                    <select id="selOrgFOWSTContractStartYear">
	                    <option value="">YYYY</option>
                    </select>                
                </fieldset>
            </div>

            <div data-role="fieldcontain">
	            <label>Contract End Date :</label>
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <select id="selOrgFOWSTContractEndMonth">
	                    <option value="">MMM</option>
                    </select>
                    <select id="selOrgFOWSTContractEndDay">
	                    <option value="">DD</option>
                    </select>                
                    <select id="selOrgFOWSTContractEndYear">
	                    <option value="">YYYY</option>
                    </select>                
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

        <div data-role="fieldcontain">
	        <label for="selOrgFOWSTInsideMall" class="select">Is this place inside a mall?</label>
            <select id="selOrgFOWSTInsideMall" data-role="slider">
			    <option value="0">No</option>
	            <option value="1">Yes</option>
            </select>
        </div>

        <div id="divOrgFOWSTInsideMall" style="display:none;">
            <div data-role="fieldcontain">
	            <label for="selOrgFOWSTMallName" class="select">Name of Mall:</label>
                <select id="selOrgFOWSTMallName">
			        <option value="">Please Select</option>
			        <option value="Ayala Mall">Ayala Mall</option>
	                <option value="SM Mall">SM Mall</option>
                </select>
            </div>

            <div id="divOrgFOWSTAyalaInfo" style="display:none;">
                <div data-role="fieldcontain">
                    <div style="display:none;"><input type="text" readonly id="txtOrgFOWSTAyalaMallRecID"/></div>
	                <label for="txtOrgFOWSTAyalaMallTenantCode">Tenant Code : <span style="color: #FF0000;" >*</span></label>
                    <input type="text" id="txtOrgFOWSTAyalaMallTenantCode" maxlength="10"/>
            
                </div>

                <div data-role="fieldcontain">
	                <label for="txtOrgFOWSTAyalaMallTenantName">Tenant Name : <span style="color: #FF0000;" >*</span></label>
                    <input type="text" id="txtOrgFOWSTAyalaMallTenantName" maxlength="20"/>
             
                </div>
            </div>

            <div id="divOrgFOWSTSMInfo" style="display:none;">
                <div data-role="fieldcontain">
                    <div style="display:none;"><input type="text" readonly id="txtOrgFOWSTSMMallRecID"/></div>
	                <label for="txtOrgFOWSTSMMallCode">Mall or Branch Code : <span style="color: #FF0000;" >*</span></label>
                    <input type="text" id="txtOrgFOWSTSMMallCode" maxlength="10"/>
           
                </div>

                <div data-role="fieldcontain">
	                <label for="txtOrgFOWSTSMMallTenantNo">Tenant No. : <span style="color: #FF0000;" >*</span></label>
                    <input type="text" id="txtOrgFOWSTSMMallTenantNo" maxlength="10"/>
       
                </div>

                <div data-role="fieldcontain">
	                <label for="txtOrgFOWSTSMMallClassCode">Class Code : <span style="color: #FF0000;" >*</span></label>
                    <input type="number" id="txtOrgFOWSTSMMallClassCode"/>
          
                </div>

                <div data-role="fieldcontain">
	                <label for="txtOrgFOWSTSMMallTradeCode">Trade Code : <span style="color: #FF0000;" >*</span></label>
                   <input type="text" id="txtOrgFOWSTSMMallTradeCode" maxlength="10"/>
                
                </div>

                <div data-role="fieldcontain">
	                <label for="txtOrgFOWSTSMMallOutletNo">Outlet No. : <span style="color: #FF0000;" >*</span></label>
                     <input type="number" id="txtOrgFOWSTSMMallOutletNo"/>
          
                </div>

                <div data-role="fieldcontain">
	                <label for="txtOrgFOWSTSMMallSalesType">Sales Type : <span style="color: #FF0000;" >*</span></label>
                    <input type="text" id="txtOrgFOWSTSMMallSalesType" maxlength="6"/>
      
                </div>

            </div>

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
	        <label for="txtOrgFOWSTPlateNo">Plate No. : </label>
            <div data-role="fieldcontain">
                <input type="text" id="txtOrgFOWSTPlateNo"/>
             
            </div>

	        <label for="txtOrgFOWSTNoOfWheels">No. of Wheels: </label>
            <div data-role="fieldcontain">
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

        <div id="divOrgFOWSTOperationHR">
            <ul data-role="listview" id="lstOrgFOWSTOperationHRContainer" data-inset="true">   
			    <li data-role="list-divider">Operation Hours</li>
                <li>
                    <div>
                        <ul data-role="listview" id="lstOrgFOWSTOperationHR" data-inset="true">
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div id="divOrgFOWSTReportsToPrincipal">
            <ul data-role="listview" id="lstOrgFOWSTSpecialReportContainer" data-inset="true">   
			    <li data-role="list-divider">Reports To Principal</li>
                <li>
                    <div data-role="fieldcontain">
                        <label for="selOrgFOWSTIsInclude">Include FOWST transactions in principal report?</label>
                        <select id="selOrgFOWSTIsInclude" data-role="slider" class="ui-slider-switch">
                            <option value="0">No</option>
                            <option value="1">Yes</option>
                        </select>
                    </div>
                    <div data-role="fieldcontain">
                        <label for="txtAccountID">Account ID: </label>
                        <input type="text" id="txtAccountID"/>
                    </div>
                    <div data-role="fieldcontain">
                        <label for="txtStoreID">Store ID: </label>
                        <input type="text" id="txtStoreID"/>
                    </div>
                </li>
            </ul>
        </div>

        <div id="divOrgFOWSTShopperTrak">
            <ul data-role="listview" id="lstOrgFOWSTShopperTrakContainer" data-inset="true">   
			    <li data-role="list-divider">ShopperTrak</li>
                <li>
                    <div data-role="fieldcontain">
                        <label for="selOrgFOWSTIsIncludeToShopperTrak">Include FOWST transactions in shoppertrak report?</label>
                        <select id="selOrgFOWSTIsIncludeToShopperTrak" data-role="slider" class="ui-slider-switch">
                            <option value="0">No</option>
                            <option value="1">Yes</option>
                        </select>
                    </div>
                    <div data-role="fieldcontain">
                        <label for="txtShopperTrakAccountID">Account ID: </label>
                        <input type="text" id="txtShopperTrakAccountID"/>
                    </div>                 
                </li>
            </ul>
        </div>
         <div id="divOrgFOWSTPrinter">
            <ul data-role="listview" id="lstOrgFOWSTvContainer" data-inset="true">   
			    <li data-role="list-divider">Printers</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnAddOrgFOWSTPrinter" data-role="button" data-icon="plus" data-inline="true">Add Printer</a></div>
                    <div>
                    <input type="hidden" id="txtPrinterID" value = ""/>
                        <ul data-role="listview" id="lstOrgFOWSTPrinter" data-inset="true">
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

<div id="pageOrgFOWSTTruckRegInfo" data-role="page">

    <div data-role="header">
        <h1>Registration Info</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;" >
	        <input type="text" readonly id="txtOrgFOWSTTRegID"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="txtOrgFOWSTTRegORNo">OR No. : <span style="color: #FF0000;" >*</span></label>
            <input type="text" id="txtOrgFOWSTTRegORNo"/>
   
        </div>

        <div data-role="fieldcontain">
	        <label>Issued Date :</label>
            <fieldset data-role="controlgroup" data-type="horizontal">
                <select id="selOrgFOWSTTRegIssuedMonth">
	                <option value="">MMM</option>
                </select>
                <select id="selOrgFOWSTTRegIssuedDay">
	                <option value="">DD</option>
                </select>                
                <select id="selOrgFOWSTTRegIssuedYear">
	                <option value="">YYYY</option>
                </select>                
            </fieldset>
        </div>

        <div data-role="fieldcontain">
	        <label>Expiration Date :</label>
            <fieldset data-role="controlgroup" data-type="horizontal">
                <select id="selOrgFOWSTTRegExpirationMonth">
	                <option value="">MMM</option>
                </select>
                <select id="selOrgFOWSTTRegExpirationDay">
	                <option value="">DD</option>
                </select>                
                <select id="selOrgFOWSTTRegExpirationYear">
	                <option value="">YYYY</option>
                </select>                
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

<div id="pageOrgFOWSTSectionInfo" data-role="page">

    <div data-role="header">
        <h1>Section Info</h1>
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
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

<div id="pageOrgFOWSTBusinessPermitInfo" data-role="page">

    <div data-role="header">
        <h1>Business Permit Info</h1>
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
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

                    <div style="display:none;"><input type="text" readonly id="txtOrgFOWSTBPermitCityID"/></div>
                    <div data-role="fieldcontain">
	                    <label for="btnOrgFOWSTBPermitCity">City or Town: <span style="color: #FF0000;" >*</span></label>
                        <button id="btnOrgFOWSTBPermitCity" data-role="button">Please Select</button>
                       
                    </div>

                    <div style="display:none;"><input type="text" readonly id="txtOrgFOWSTBPermitCountryID"/></div>
                    <div data-role="fieldcontain">
	                    <label for="btnOrgFOWSTBPermitCountry">Country: <span style="color: #FF0000;" >*</span></label>
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
	        <label>Date Of Payment:</label>
            <fieldset data-role="controlgroup" data-type="horizontal">
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
	        <label>Date Issued:</label>
            <fieldset data-role="controlgroup" data-type="horizontal">
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

<div id="pageOrgFOWSTTelInfo" data-role="page">

    <div data-role="header">
        <h1>Telephone Info</h1>
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
            <input type="text" readonly id="txtOrgFOWSTTelCountryID"/>
	        <input type="text" readonly id="txtOrgFOWSTTelID"/>                
	        <input type="text" readonly id="txtOrgFOWSTTelAreaCodeID"/>                
        </div>

        <div data-role="fieldcontain">

            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <button id="btnOrgFOWSTTelCountryCode" data-role="button">Country Code</button>
                </div>

                <div class="ui-block-b">
                    <button id="btnOrgFOWSTTelAreaCode" data-role="button">Area Code</button>
                </div>
            </div>

        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
	            <div><label for="txtOrgFOWSTTelNo">Telephone No.: <span style="color: #FF0000;" >*</span></label></div>
	            <div><input type="number" id="txtOrgFOWSTTelNo" maxlength="7"/></div>  
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
	            <div><label for="txtOrgFOWSTTelExtension">Extension or Local No.:</label></div>
	            <div><input type="number" id="txtOrgFOWSTTelExtension" maxlength="10"/></div>  
            </fieldset>
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

<div id="pageOrgFOWSTMobileInfo" data-role="page">

    <div data-role="header">
        <h1>Mobile Info</h1>
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgFOWSTMobileID"/>                
	        <input type="text" readonly id="txtOrgFOWSTMobileCountryID"/>                
	        <input type="text" readonly id="txtOrgFOWSTTelCoPrefixID"/>                
        </div>

        <div data-role="fieldcontain">

            <div class="ui-grid-a">
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
            <input type="number" id="txtOrgFOWSTMobileNo" maxlength="7"/>
    
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

<div id="pageOrgFOWSTEmailInfo" data-role="page">

    <div data-role="header">
        <h1>Email Info</h1>
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
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

<div id="pageOrgFOWSTOperationHRInfo" data-role="page">

    <div data-role="header">
        <h1>Operation Hour Info</h1>
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgFOWSTOptHRID"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="selOrgFOWSTOptHRDay">Day: </label>
            <select id="selOrgFOWSTOptHRDay" disabled>
	            <option value="1">Sunday</option>
	            <option value="2">Monday</option>
	            <option value="3">Tuesday</option>
	            <option value="4">Wednesday</option>
	            <option value="5">Thursday</option>
	            <option value="6">Friday</option>
	            <option value="7">Saturday</option>
            </select>   
        </div>

	    <label>Opening time: <span style="color: #FF0000;" >*</span></label>
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" data-type="horizontal">
                <select id="selOrgFOWSTOptOpenHR">
	                <option value="">HR </option>
                </select>   
                <select id="selOrgFOWSTOptOpenMin">
	                <option value="">MN</option>
                </select>                
                <select id="selOrgFOWSTOptOpenAMPM">
	                <option value="">AM/PM</option>
	                <option value="AM">AM</option>
	                <option value="PM">PM</option>
                </select>                
            </fieldset>
        </div>

	    <label>Closing time: <span style="color: #FF0000;" >*</span></label>
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" data-type="horizontal">
                <select id="selOrgFOWSTOptCloseHR">
	                <option value="">HR </option>
                </select>   
                <select id="selOrgFOWSTOptCloseMin">
	                <option value="">MN</option>
                </select>                
                <select id="selOrgFOWSTOptCloseAMPM">
	                <option value="">AM/PM</option>
	                <option value="AM">AM</option>
	                <option value="PM">PM</option>
                </select>                
            </fieldset>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgFOWSTOptHRSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgFOWSTOptHRCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgFOWSTPrinterInfo" data-role="page">

    <div data-role="header">
        <h1>Printer Roles</h1>
        <a href="#" data-icon="arrow-l" id="btnPrinterBack">Back</a>
    </div>

    <div data-role="content">
        <div id="divDevicePrinterResults" style="display:none;">

   
            <ul id="lstOrgFOWSTPrinterList" data-role="listview" data-filter="true">
            </ul>

        </div>

        <div id="divDevicePrinterSearchNoResults" style="display:none;">
	        <div>
                <div data-role="fieldcontain">
                    <label id="lblDevicePrinterNotFound">No records found.</label>
                </div>
            </div>
        </div>


        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnAddNewDevicePrinterOrg" data-role="button"><h3>Add New Printer Role</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnDevicePrinterBackToHome" data-role="button"><h3>Back To Home</h3></a>
            </div>
        </div>

    </div>

</div>
<div id="pageOrgFOWSTPrinterIP" data-role="page">

    <div data-role="header">
        <h1>Add Printer to FOWST</h1>
        <a href="#" data-icon="arrow-l" id="btnPrinterIPBack">Back</a>
    </div>
    <div data-role="content">
        <h3 id="txtPrinterRole"></h3>

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtPrinterRoleID"/>                
        </div>
        <div data-role="fieldcontain">
	         <label for="txtPrinterName">Printer Name: <span style="color: #FF0000;" >*</span></label>
	         <input type="text" id="txtPrinterName" maxlength ="50"/>
        </div>
        <div data-role="fieldcontain">
	         <label for="txtPrinterIPAddress">Printer IP Address: <span style="color: #FF0000;" >*</span></label>
	         <input type="text" id="txtPrinterIPAddress" maxlength ="15"/>
             <input type="hidden" id="txtPrinterFowstID" value = ""/>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgFOWSTPrinterSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgFOWSTPrinterRemove" style="display:none;" data-role="button"><h3>Remove</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgFOWSTPrinterCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>
        

    </div>
    

</div>
