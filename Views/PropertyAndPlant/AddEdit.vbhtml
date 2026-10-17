@Code
    ViewData("Title") = "Property And Plant"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
    @*<script src="/Scripts3/Browser/common.js"></script>         
    <script src="/Scripts3/Browser/LibraryCountries.js"></script>
    <script src="/Scripts3/Browser/LibraryCities.js"></script>
    <script src="/Scripts3/Browser/LibraryAreaCodes.js"></script>
    <script src="/Scripts3/Browser/LibraryTelcoPrefixes.js"></script>
    <script type="text/javascript" src="../../Scripts/sources/PropertyAndPlant/AddEdit.js"></script>*@
    
    <script>
        require(['/Scripts3/Browser/common.js']);
        require(['/Scripts3/Browser/LibraryCountries.js']);
        require(['/Scripts3/Browser/LibraryCities.js']);
        require(['/Scripts3/Browser/LibraryAreaCodes.js']);
        require(['/Scripts3/Browser/LibraryTelcoPrefixes.js']);
        require(['/Scripts/sources/PropertyAndPlant/AddEdit.js']);
    </script>

End Section

<div id="PropertyAndPlant" data-role="page">
    <div data-role="header">
        <h1>Property And Plant</h1>
    </div>

    <div data-role="content">
        <div data-role="fieldcontain"> 
            <label style="font-weight:bold" for="cmbType">Type :</label>
            <select id="cmbType"> 
                @*<option id="optWarehouse"> Warehouse </option>
                <option id="optBranch"> Branch </option>
                <option id="optOffice"> Office </option>
                <option id="optDeliveryEquipment"> Delivery Equipment </option>*@
            </select>
        </div>

        <div data-role="fieldcontain">
            <label id = 'lblCode' style="font-weight:bold" for="txtCode"> Code :</label>
            <input type="text" id="txtCode" placeholder="Enter Code"/>
        </div>

        <div data-role="fieldcontain">
            <label id = 'lblName' style="font-weight:bold" for="txtName"> Name :</label>
            <input type="text" id="txtName" placeholder="Enter Name"/>
        </div>

        <div data-role="fieldcontain">
            <label id = 'lblName' style="font-weight:bold" for="txtDescription"> Description :</label>
            <input type="text" id="txtDescription" placeholder="Enter Description"/>
        </div>
                
        <div data-role="fieldcontain">
                <fieldset data-role="controlgroup" data-type="vertical">
                    <legend> <b> Address : </b></legend>

                    <div data-role="controlgroup">
                        <label style="font-weight:bold" for="txtHouseNo">House or Building No. :</label>
                        <input type="text" id="txtHouseNo" placeholder="Enter House or Building No."/>                        
                    </div>

                    <div data-role="controlgroup">
                        <label style="font-weight:bold" for="txtStreet">Street :</label>
                        <input type="text" id="txtStreet" placeholder="Enter Street"/>                        
                    </div>

                    <div data-role="controlgroup">
                        <label style="font-weight:bold" for="butCityorTown">City or Town :</label>
                        <button id="butCityorTown">Select City or Town</button>
                    </div>
                    
                    <div data-role="controlgroup">
                        <label style="font-weight:bold" for="butCountry">Country :</label>
                        <button id="butCountry">Select Country</button>
                    </div>
                        
                </fieldset>
            </div>

        <div id="dvDeliveryEquiptmentInfo">
            <div data-role="fieldcontain" id="dvPlateNo">
                <label style="font-weight:bold" for="txtPlateNo">Plate No. :</label>
                <input type="text" id="txtPlateNo" placeholder="Enter Plate No"/>            
            </div>

            <div data-role="fieldcontain" id="dvNoofWheels">
                <label style="font-weight:bold" for="txtNoofWheels">No of Wheels :</label>
                <input type="text" id="txtNoofWheels" placeholder="Enter No of Wheels "/>
            </div>

            <div data-role="fieldcontain" id="dvBodyColor">
                <label style="font-weight:bold" for="txtBodyColor">Body Color :</label>
                <input type="text" id="txtBodyColor" placeholder="Enter Body Color"/>
            </div>

            <div data-role="fieldcontain" id="dvLoadLimitWeight)">
                <label style="font-weight:bold" for="txtLoadLimitWeight">Load Limit (in weight) :</label>
                <input type="text" id="txtLoadLimitWeight" placeholder="Enter Load Limit"/>
            </div>

            <div data-role="fieldcontain" id="dvLoadLimitDimensions">
                <label style="font-weight:bold" for="txtLoadLimitDimensions">Load Limit (in dimensions) :</label>
                <input type="text" id="txtLoadLimitDimensions" placeholder="Enter Load Limit"/>
            </div>

            <div id="dvLTO">
                <ul data-role="listview" data-inset="true">               
                    <li data-role="list-divider"> LTO Registration Details</li>     

                    <li>
                        <div> 
                            <a href="#Registration" id="btnLTO" data-role="button" data-icon="plus" data-inline="true" data-mini="true">Add New LTO Registration</a> 
                        </div> 

                        <div>
                            <ul id="lstLTO" data-role="listview" data-inset="true" data-split-icon="delete"> </ul>
                        </div>
                    
                    </li>
                </ul>            
            </div>

            <div id="dvLTFRB">
                <ul data-role="listview" data-inset="true">               
                    <li data-role="list-divider"> LTFRB Registration Details</li>     

                    <li>
                        <div> 
                            <a href="#Registration" id="btnLTFRB" data-role="button" data-icon="plus" data-inline="true" data-mini="true">Add New LTFRB Registration</a> 
                        </div> 

                        <div>
                            <ul id="lstLTFRB" data-role="listview" data-inset="true" data-split-icon="delete"> </ul>
                        </div>
                    
                    </li>
                </ul>            
            </div>
        </div>

        <div id="dvWarehouseType" data-role="fieldcontain">
            <label style="font-weight:bold" for="cmbType">Warehouse Type :</label>
            <select id="cmbWarehouse"> 
            @*    <option id="optWarehouseStore"> Warehouse Store </option>
                <option id="optCrossDockingWarehouse "> Cross Docking Warehouse </option>                
                <option id="optRegularWarehouse">  Regular Warehouse </option>
                <option id="optDistributionCenter"> Distribution Center </option>*@
            </select>
        </div>

        <div id="dvRetailStore" data-role="fieldcontain">
            <label style="font-weight:bold" for="cmbType">Type of Retail Store :</label>
            <select id="cmbRetailStore"> 
               @* <option id="optMallAttachedSupermarket"> Mall-Attached Supermarket </option>
                <option id="optStandAloneSupermarket"> Stand Alone Supermarket </option>                
                <option id="optDepartmentStore">  Department Store </option>
                <option id="optHypermarket"> Hypermarket </option>
                <option id="optSpecialtyStore"> Specialty Store </option>
                <option id="optConvenienceStore"> Convenience or General Store </option>
                <option id="optMomandPopStore"> Mom-and-Pop or Sari-Sari Store </option>
                <option id="optKiosk"> Kiosk </option>
                <option id="optCounterTop"> Counter Top </option>
                <option id="optShelfSpace"> Shelf Space </option>
                <option id="optMobileStore"> Mobile Store (Van or Truck Store) </option>*@
            </select>
        </div>

        <div id="dvSections">
            <ul data-role="listview" data-inset="true">               
                <li data-role="list-divider">Sections</li>     

                <li>
                    <div> 
                        <a href="#Sections" id="butSections" data-role="button" data-icon="plus" data-inline="true" data-mini="true">Add New Section</a> 
                    </div> 

                    <div>
                        <ul id="lstSection" data-role="listview" data-inset="true" data-split-icon="delete"> </ul>
                    </div>
                    
                </li>
            </ul>            
        </div>

        

        <div>
            <ul data-role="listview" data-inset="true" >
                <li data-role="list-divider">Business Permit</li>                

                <li>
                    <div> 
                        <a href="#BusinessPermit" id="btnBusinessPermit" data-role="button" data-icon="plus" data-inline="true" data-mini="true">Add New Business Permit</a> 
                    </div>

                    <div>
                        <ul id="lstBusinessPermit" data-role="listview" data-inset="true" data-split-icon="delete"> </ul>
                    </div>
                </li>                
            </ul>            
        </div>
        
        <div>
            <ul data-role="listview" data-inset="true">               
                <li data-role="list-divider">Telephone</li>
                <li>
                    <div> 
                        <a href="#Telephone" id="btnTelephone" data-role="button" data-icon="plus" data-inline="true" data-mini="true">Add New Telephone Contact</a> 
                    </div> 

                    <div>
                        <ul id="lstTelephone" data-role="listview" data-inset="true" data-split-icon="delete"> </ul>
                    </div>                    
                </li>
            </ul>            
        </div>
        
        <div>
            <ul data-role="listview" data-inset="true">               
                <li data-role="list-divider">Mobile</li>                
                <li>
                    <div> 
                        <a href="#Mobile" id="btnMobile" data-role="button" data-icon="plus" data-inline="true" data-mini="true">Add New Mobile Contact</a> 
                    </div> 

                    <div>
                        <ul id="lstMobile" data-role="listview" data-inset="true" data-split-icon="delete"> </ul>
                    </div>                    
                </li>
            </ul>            
        </div>

        <div>
            <ul data-role="listview" data-inset="true">               
                <li data-role="list-divider">Email</li>                
                <li>
                    <div> 
                        <a href="#EMail" id="btnEmail" data-role="button" data-icon="plus" data-inline="true" data-mini="true">Add New Email Contact</a>
                    </div> 

                    <div>
                        <ul id="lstEmail" data-role="listview" data-inset="true" data-split-icon="delete"> </ul>
                    </div>                    
                </li>
            </ul>            
        </div>

        <div>
            
        </div>
        
        @*<div>
            <ul id="Ul1" data-role="listview" data-inset="true" data-inset="true">
                <li data-role="list-divider">Floor Area</li>

                <li>
                    <div data-role="fieldcontain" id="Div1">
                        <label style="font-weight:bold" for="txtSquaredMeters">Squared Meters :</label>
                        <input type="text" id="txtSquaredMeters" placeholder="Enter Squared Meters"/>
                    </div>

                    <div data-role="fieldcontain" id="Div2">
                        <label style="font-weight:bold" for="txtSquaredFeet">Squared Feet :</label>
                        <input type="text" id="txtSquaredFeet" placeholder="Enter Squared Feet"/>
                    </div>
                </li>

            </ul>
        </div>

        <div>
            <ul id="Ul2" data-role="listview" data-inset="true" data-inset="true">
                <li data-role="list-divider">Volume Capacity</li>

                <li>
                    <div data-role="fieldcontain" id="Div3">
                        <label style="font-weight:bold" for="txtCubicMeters">Cubic Meters :</label>
                        <input type="text" id="txtCubicMeters" placeholder="Enter Cubic Meters"/>
                    </div>

                    <div data-role="fieldcontain" id="Div4">
                        <label style="font-weight:bold" for="txtCubicFeet">Cubic Feet :</label>
                        <input type="text" id="txtCubicFeet" placeholder="Enter Cubic Feet"/>
                    </div>
                </li>
            </ul>
        </div>
        *@

        <div data-role="fieldcontain">
            <label for="cmbOwnershipType">Ownership Type :</label>
            <select id="cmbOwnershipType"> 
                <option value="0"> Owned </option>
                <option value="1"> Rented or Leased </option>
            </select>
        </div>

        <div data-role="fieldcontain" id="dvContractStart">
            <fieldset data-role="controlgroup" data-type="horizontal">
                <legend>Contract Start Date :</legend>
                <div>
                    <select id="selContractStartMonth"><option value="">Month</option></select>
                    <select id="selContractStartDay"> <option value="">Day</option> </select>
                    <select id="selContractStartYear"><option value="">Year</option></select>
                </div>        
            </fieldset>
        </div>

        <div data-role="fieldcontain" id="dvContractEnd">
            <fieldset data-role="controlgroup" data-type="horizontal">
                <legend>Contract End Date :</legend>
                <div>
                    <select id="selContractEndMonth"><option value="">Month</option></select>
                    <select id="selContractEndDay"> <option value="">Day</option> </select>
                    <select id="selContractEndYear"><option value="">Year</option></select>
                </div>        
            </fieldset>
        </div>

       @* <div data-role="fieldcontain" >
            <label style="font-weight:bold" for="txtContractStart"></label>
            <input type="text" id="txtContractStart" placeholder="Enter Contract Date Start"/>
        </div>

        <div data-role="fieldcontain" >
            <label style="font-weight:bold" for="txtContractEnd"></label>
            <input type="text" id="txtContractEnd" placeholder="Enter Contract Date End"/>
        </div>*@

        <div data-role="fieldcontain" id="dvCostPerMonth">
            <label for="txtCostPerMonth">Cost per Squred Meter per Month or Squared Foot per Month :</label>
            <input type="number" id="txtCostPerMonth" placeholder="Enter Cost per Squred Meter per Month or Squared Foot per Month"/>
        </div>

        <div data-role="fieldcontain" id="dvCostPerMonth">
            <label for="optSlider">This Asset is Mortgaged? :</label>
            <select id="optSlider" data-role="slider" class="ui-slider-switch">
                <option value="0">No</option>
                <option value="1">Yes</option>
            </select>
        </div>

        <br /> <br />

        <div data-role="fieldcontain">
            <div> <a href="#" id="btnDone" data-role="button">Done</a> </div> 
            <div> <a href="#" id="btnCancel" data-role="button">Cancel</a> </div> 
        </div>
    </div>
</div>

<div id="BusinessPermit" data-role="page">
    <div data-role="header">
        <h1>Business Permit</h1>
    </div>

    <div data-role="content">
        <div data-role="fieldcontain">
            <label style="font-weight:bold" for="txtBusinessPermitNumber">Business Permit Number :</label>
            <input type="text" id="txtBusinessPermitNumber" placeholder="Enter Business Permit Number"/>
        </div>

        <div data-role="fieldcontain">        
            <label style="font-weight:bold" for="txtBusinessID">Business ID :</label>
            <input type="text" id="txtBusinessID" placeholder="Enter Business ID"/>
        </div>

        <div data-role="fieldcontain">        
            <label style="font-weight:bold" for="txtTradeName">Trade Name :</label>
            <input type="text" id="txtTradeName" placeholder="Enter Trade Name"/>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" data-type="vertical">
                <legend> <b> Address : </b></legend>

                <div data-role="controlgroup">
                    <label style="font-weight:bold" for="txtBusinessPermitHouseNo">House or Building No. :</label>
                    <input type="text" id="txtBusinessPermitHouseNo" placeholder="Enter House or Building No."/>                        
                </div>

                <div data-role="controlgroup">
                    <label style="font-weight:bold" for="txtBusinessPermitStreet">Street :</label>
                    <input type="text" id="txtBusinessPermitStreet" placeholder="Enter Street"/>                        
                </div>

                <div data-role="controlgroup">
                    <label style="font-weight:bold" for="butCityorTown">City or Town :</label>
                    <button id="butBusinessPermitCityorTown">Select City or Town</button>
                </div>
                    
                <div data-role="controlgroup">
                    <label style="font-weight:bold" for="butCountry">Country :</label>
                    <button id="butBusinessPermitCountry">Select Country</button>
                </div>
                        
            </fieldset>
        </div>
        
        <div data-role="fieldcontain">        
            <label style="font-weight:bold" for="txtOfficialReceiptNumber">Official Receipt Number :</label>
            <input type="text" id="txtOfficialReceiptNumber" placeholder="Enter Official Receipt Number "/>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" data-type="horizontal">
                <legend> <b> Date of Payment : </b></legend>
                <div>
                    <select id="selDOPMonth"><option value="">Month</option></select>
                    <select id="selDOPDay"> <option value="">Day</option> </select>
                    <select id="selDOPYear"><option value="">Year</option></select>
                </div>        
            </fieldset>
        </div>

        <div data-role="fieldcontain">        
            <label style="font-weight:bold" for="txtBusinessPlateNo">Business Plate No. :</label>
            <input type="number" id="txtBusinessPlateNo" placeholder="Enter Business Plate No."/>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" data-type="horizontal">
                <legend> <b> Date Issued : </b></legend>
                <div>
                    <select id="selDIMonth"><option value="">Month</option></select>
                    <select id="selDIDay"> <option value="">Day</option> </select>
                    <select id="selDIYear"><option value="">Year</option></select>
                </div>        
            </fieldset>
        </div>
        
        @*<div data-role="fieldcontain" id="dvLTORegistrationNumber">
            <label style="font-weight:bold" for="txtLTORegistrationNumber">LTO Registration Number :</label>
            <input type="text" id="txtLTORegistrationNumber" placeholder="Enter LTO Registration Number"/>            
        </div>

        <div data-role="fieldcontain" id="dvGrossWeight">
            <label style="font-weight:bold" for="txtGrossWeight">Gross Weight :</label>
            <input type="text" id="txtGrossWeight" placeholder="Enter Gross Weight"/>            
        </div>

        <div data-role="fieldcontain" id="dvNetWeight">
            <label style="font-weight:bold" for="txtNetWeight">Net Weight :</label>
            <input type="text" id="txtNetWeight" placeholder="Enter Net Weight"/>            
        </div>

        <div data-role="fieldcontain" id="dvGarageAddressButton">
            <div> <a href="#Addresses" id="btnGarageAddress" data-role="button" data-icon="plus" data-inline="true" data-mini="true">Add New Garage Address</a> </div> 
        </div>*@

       @* <div data-role="fieldcontain" id="dvGarageAddress">
            <ul id="lstGarageAddress" data-role="listview" data-inset="true" data-split-icon="delete">
                <li data-role="list-divider">Garage Address(es)</li>                
            </ul>
        </div>

        <br /> <br />*@
    </div>

    <div data-role="footer">
        <div> <a href="#" id="btnBusinessPermitOk" data-role="button">Ok</a> </div> 
        <div> <a href="#PropertyAndPlant" id="btnBusinessPermitCancel" data-role="button">Cancel</a> </div> 
    </div>
</div>

<div id="Telephone" data-role="page">
    <div data-role="header">
        <h1>Telephone</h1>
    </div>

    <div data-role="content">
        <div class="ui-grid-a">
            <div class="ui-block-a">
                <button id="butTelephoneCountryCode">Country Code</button>
            </div>

            <div class="ui-block-b">
                <button id="butTelephoneAreaCode">Area Code</button>
            </div>
        </div>

        <div data-role="fieldcontain">
            <label style="font-weight:bold" for="txtTelephoneNumber">Number :</label>
            <input type="text" id="txtTelephoneNumber" placeholder="Enter Telephone No"/>
        </div>

        <div data-role="fieldcontain">
            <label style="font-weight:bold" for="txtTelephoneLocal">Local :</label>
            <input type="text" id="txtTelephoneLocal" placeholder="Enter Local No"/ style="width:50%">
        </div>

    </div>

    <div data-role="footer">
        <div> <a href="#" id="btnTelephoneOk" data-role="button">Ok</a> </div> 
        <div> <a href="#PropertyAndPlant" id="btnTelephoneCancel" data-role="button">Cancel</a> </div> 
    </div>
</div>

<div id="Mobile" data-role="page">
    <div data-role="header">
        <h1>Mobile</h1>
    </div>

    <div data-role="content">
        <div class="ui-grid-a">
            <div class="ui-block-a">
                <button id="butMobileCountryCode">Country Code</button>
            </div>

            <div class="ui-block-b">
                <button id="butMobileTelcoCode">Telco Code</button>
            </div>
        </div>

        <div data-role="fieldcontain">
            <label style="font-weight:bold" for="txtMobileNumber">Number :</label>
            <input type="text" id="txtMobileNumber" placeholder="Enter Mobile No"/>
        </div>

    </div>

    <div data-role="footer">
        <div> <a href="#" id="btnMobileOk" data-role="button">Ok</a> </div> 
        <div> <a href="#PropertyAndPlant" id="btnMobileCancel" data-role="button">Cancel</a> </div> 
    </div>
</div>

<div id="EMail" data-role="page">
    <div data-role="header">
        <h1>EMail</h1>
    </div>

    <div data-role="content">
        <div data-role="fieldcontain">
            <label style="font-weight:bold" for="txtEMail">EMail :</label>
            <input type="text" id="txtEMail" placeholder="Enter EMail"/>
        </div>
    </div>

    <div data-role="footer">
        <div> <a href="#" id="btnEMailOk" data-role="button">Ok</a> </div> 
        <div> <a href="#PropertyAndPlant" id="btnEMailCancel" data-role="button">Cancel</a> </div> 
    </div>
</div>

<div id="Addresses" data-role="page">
    <div data-role="header">
        <h1>Address</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain">
            <label style="font-weight:bold" for="txtGarageStreet">Street :</label>
            <input type="text" id="txtGarageStreet" placeholder="Enter Street Name"/>            
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" data-type="horizontal">
                <legend> <b> City or Town : </b></legend>
                <div>
                    <button id="butGarageCityorTown">Select City or Town</button>
                </div>        
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" data-type="horizontal">
                <legend> <b> Country : </b></legend>
                <div>
                    <button id="butGarageCountry">Select Country</button>
                </div>        
            </fieldset>
        </div>
    </div>

    <div data-role="footer">
        <div> <a href="#" id="btnAddressesOk" data-role="button">Ok</a> </div> 
        <div> <a href="#BusinessPermit" id="btnAddressesCancel" data-role="button">Cancel</a> </div> 
    </div>
</div>

<div id="Sections" data-role="page">
    <div data-role="header">
        <h1>Section</h1>
    </div>
    
    <div data-role="content">
        <div data-role="fieldcontain">
            <label style="font-weight:bold" for="txtSectionCode">Code :</label>
            <input type="text" id="txtSectionCode" placeholder="Enter Section Code"/>
        </div>

        <div data-role="fieldcontain">
            <label style="font-weight:bold" for="txtSectionName">Name :</label>
            <input type="text" id="txtSectionName" placeholder="Enter Section Name"/>            
        </div>

        <div data-role="fieldcontain">
            <label style="font-weight:bold" for="txtSectionDescription">Description :</label>
            <input type="text" id="txtSectionDescription" placeholder="Enter Section Description"/>            
        </div>

        <div data-role="fieldcontain" id="Div1">
            <label style="font-weight:bold" for="txtSectionFloorArea">Floor Area:</label>
            <input type="number" id="txtSectionFloorArea" placeholder="Enter Floor Area"/>
        </div>
        
        <div data-role="fieldcontain">
            <label style="font-weight:bold" for="txtSectionSpecialFeatures">Special Features :</label>
            <input type="text" id="txtSectionSpecialFeatures" placeholder="Enter Section Special Features"/>
        </div>
    </div>

    <div data-role="footer">
        <div> <a href="#" id="btnSectionsOk" data-role="button">Ok</a> </div> 
        <div> <a href="#PropertyAndPlant" id="btnSectionsCancel" data-role="button">Cancel</a> </div> 
    </div>
</div>

<div id="Registration" data-role="page">
    <div data-role="header">
        <h1>Registration Details</h1>
    </div>
    
    <div data-role="content">
        <div data-role="fieldcontain">
            <label style="font-weight:bold" for="txtORNo">O.R No. :</label>
            <input type="text" id="txtORNo" placeholder="Enter O.R No"/>
        </div>

        <div data-role="fieldcontain" id="dvRegistrationDateofIssue">
            <fieldset data-role="controlgroup" data-type="horizontal">
                <legend> <b> Date of Issue : </b></legend>
                <div>
                    <select id="selRegistrationDateofIssueMonth"><option value="">Month</option></select>
                    <select id="selRegistrationDateofIssueDay"> <option value="">Day</option> </select>
                    <select id="selRegistrationDateofIssueYear"><option value="">Year</option></select>
                </div>        
            </fieldset>
        </div>

        <div data-role="fieldcontain" id="dvRegistrationExpirationDate">
            <fieldset data-role="controlgroup" data-type="horizontal">
                <legend> <b> Expiration Date : </b></legend>
                <div>
                    <select id="selRegistrationExpirationDateMonth"><option value="">Month</option></select>
                    <select id="selRegistrationExpirationDateDay"> <option value="">Day</option> </select>
                    <select id="selRegistrationExpirationDateYear"><option value="">Year</option></select>
                </div>        
            </fieldset>
        </div>

    <div data-role="footer">
        <div> <a href="#" id="btnRegistrationOk" data-role="button">Ok</a> </div> 
        <div> <a href="#PropertyAndPlant" id="btnRegistrationCancel" data-role="button">Cancel</a> </div> 
    </div>
</div>