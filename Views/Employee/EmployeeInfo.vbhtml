@Code
    ViewData("Title") = "Employee Information"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code

@Section head

    <script type="text/javascript" src="../../Scripts/sources/Employee/EmployeeInfo.js"></script>
    
End Section

<div data-role="content">

    <div data-role="fieldcontain">
	    <label for="txtEmployeeID">Employee ID:</label><br />
	    <input type="text" id="txtEmployeeID"/>                
    </div>

    <div data-role="fieldcontain">
	    <label for="txtSurname">Surname:</label><br />
	    <input type="text" id="txtSurname"/>                
    </div>

    <div data-role="fieldcontain">
	    <label for="txtFirstName">First Name:</label><br />
	    <input type="text" id="txtFirstName"/>                
    </div>

    <div data-role="fieldcontain">
	    <label for="txtMiddleName">Middle Name:</label><br />
	    <input type="text" id="txtMiddleName"/>                
    </div>

    <div>
	    <div><label>Present Address</label></div>
        <div data-role="fieldcontain">
	        <label for="txtPresentAddressStreet">Street:</label><br />
	        <input type="text" id="txtPresentAddressStreet"/>                
        </div>
        <div data-role="fieldcontain">
	        <label for="txtPresentAddressCity">City:</label><br />
	        <input type="text" readonly style="display:none;" id="txtPresentAddressCityID"/>
	        <input type="text" readonly id="txtPresentAddressCity"/>
            <a data-role="button" id="btnBrowsePresentAddressCity" onclick="BrowseCity()">Select City</a>
        </div>
        <div data-role="fieldcontain">
	        <label for="txtPresentAddressCountry">Country:</label><br />
	        <input type="text" readonly style="display:none;" id="txtPresentAddressCountryID"/>
	        <input type="text" readonly id="txtPresentAddressCountry"/>
            <a data-role="button" id="btnBrowsePresentAddressCountry">Select Country</a>
        </div>    
    </div>

    <div>
	    <div><label>Permanent Address</label></div>
        <div data-role="fieldcontain">
	        <label for="txtPermanentAddressStreet">Street:</label><br />
	        <input type="text" id="txtPermanentAddressStreet"/>                
        </div>
        <div data-role="fieldcontain">
	        <label for="txtPermanentAddressCity">City:</label><br />
	        <input type="text" readonly style="display:none;" id="txtPermanentAddressCityID"/>
	        <input type="text" readonly id="txtPermanentAddressCity"/>
            <a data-role="button" id="btnBrowsePermanentAddressCity">Select City</a>
        </div>
        <div data-role="fieldcontain">
	        <label for="txtPermanentAddressCountry">Country:</label><br />
	        <input type="text" readonly style="display:none;" id="txtPermanentAddressCountryID"/>
	        <input type="text" readonly id="txtPermanentAddressCountry"/>
            <a data-role="button" id="btnBrowsePermanentAddressCountry">Select Country</a>
        </div>    
    </div>

    <div data-role="fieldcontain">
	    <label for="txtDateOfBirth">Date Of Birth:</label><br />
	    <input type="text" readonly id="txtDateOfBirth"/>                
    </div>

    <div data-role="fieldcontain">
	    <label for="txtPlaceOfBirth">Place Of Birth:</label><br />
	    <input type="text" readonly style="display:none;" id="txtPlaceOfBirthID"/>
	    <input type="text" readonly id="txtPlaceOfBirth"/>
        <a data-role="button" id="btnBrowsePlaceOfBirth">Select City</a>
    </div>    

    <div data-role="fieldcontain">
	    <label>Gender:</label><br />
        <select id="selGender">
	        <option>Gender</option>
	        <option value="M">Male</option>
			<option value="F">Female</option>
			<option value="O">Others</option>
        </select>
    </div> 
    
    <div data-role="fieldcontain">
	    <label for="txtCitizenship">Citizenship:</label><br />
	    <input type="text" readonly style="display:none;" id="txtCitizenshipID"/>
	    <input type="text" readonly id="txtCitizenship"/>
        <a data-role="button" id="btnBrowseCitizenship">Select Citizenship</a>
    </div>           

    <div data-role="fieldcontain">
	    <label for="txtHeight">Height:</label><br />
	    <input type="text" id="txtHeight"/>                
        <select id="selHeightMeasure">
	        <option>Measurement</option>
	        <option value="Foot">FT</option>
			<option value="Inches">INC</option>
			<option value="Centimeter">CM</option>
        </select>
    </div>

    <div data-role="fieldcontain">
	    <label for="txtWeight">Weight:</label><br />
	    <input type="text" id="txtWeight"/>                
        <select id="selWeightMeasure">
	        <option>Measurement</option>
	        <option value="Kilo">KG</option>
			<option value="Pound">LBS</option>
        </select>
    </div>

    <div data-role="fieldcontain">
	    <label>Civil Status:</label><br />
        <select id="selCivilStatus">
	        <option>Civil Status</option>
	        <option value="Single">Single</option>
			<option value="Married">Married</option>
			<option value="Annulled">Annulled</option>
			<option value="Divorced">Divorced</option>
			<option value="Widowed">Widowed</option>
        </select>
    </div>

    <div>
	    <div><label>Spouse</label></div>
        <div data-role="fieldcontain">
	        <label for="txtSpouseSurname">Surname:</label><br />
	        <input type="text" id="txtSpouseSurname"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="txtSpouseFirstName">First Name:</label><br />
	        <input type="text" id="txtSpouseFirstName"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="txtSpouseMiddleName">Middle Name:</label><br />
	        <input type="text" id="txtSpouseMiddleName"/>                
        </div>
    </div>

    <div>
	    <div><label>Father</label></div>
        <div data-role="fieldcontain">
	        <label for="txtFatherSurname">Surname:</label><br />
	        <input type="text" id="txtFatherSurname"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="txtFatherFirstName">First Name:</label><br />
	        <input type="text" id="txtFatherFirstName"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="txtFatherMiddleName">Middle Name:</label><br />
	        <input type="text" id="txtFatherMiddleName"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="txtFatherOccupation">Occupation:</label><br />
	        <input type="text" readonly style="display:none;" id="txtFatherOccupationID"/>
	        <input type="text" readonly id="txtFatherOccupation"/>
            <a data-role="button" id="btnBrowseFatherOccupation">Select Occupation</a>
        </div>
         
    </div>

    <div>
	    <div><label>Mother</label></div>
        <div data-role="fieldcontain">
	        <label for="txtMotherSurname">Surname:</label><br />
	        <input type="text" id="txtMotherSurname"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="txtMotherFirstName">First Name:</label><br />
	        <input type="text" id="txtMotherFirstName"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="txtMotherMiddleName">Middle Name:</label><br />
	        <input type="text" id="txtMotherMiddleName"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="txtMotherOccupation">Occupation:</label><br />
	        <input type="text" readonly style="display:none;" id="txtMotherOccupationID"/>
	        <input type="text" readonly id="txtMotherOccupation"/>
            <a data-role="button" id="btnBrowseMotherOccupation">Select Occupation</a>
        </div>

    </div>

        <div>
	    <div><label>Parent Address</label></div>
        <div data-role="fieldcontain">
	        <label for="txtParentAddressStreet">Street:</label><br />
	        <input type="text" id="txtParentAddressStreet"/>                
        </div>
        <div data-role="fieldcontain">
	        <label for="txtParentAddressCity">City:</label><br />
	        <input type="text" readonly style="display:none;" id="txtParentAddressCityID"/>
	        <input type="text" readonly id="txtParentAddressCity"/>
            <a data-role="button" id="btnBrowseParentAddressCity">Select City</a>
        </div>
        <div data-role="fieldcontain">
	        <label for="txtParentAddressCountry">Country:</label><br />
	        <input type="text" readonly style="display:none;" id="txtParentAddressCountryID"/>
	        <input type="text" readonly id="txtParentAddressCountry"/>
            <a data-role="button" id="btnBrowseParentAddressCountry">Select Country</a>
        </div>    
    </div>

    <div>
	    <div><label>Residence Certification</label></div>
        <div data-role="fieldcontain">
	        <label for="txtResidenceCertNo">Certification No.:</label><br />
	        <input type="text" id="txtResidenceCertNo"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="txtResidenceCertIssuedAt">Issued At:</label><br />
	        <input type="text" id="txtResidenceCertIssuedAt"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="txtResidenceCertIssuedOn">Issued On:</label><br />
	        <input type="text" readonly id="txtResidenceCertIssuedOn"/>                
        </div>
    </div>

    <div data-role="fieldcontain">
	    <label for="txtSSSNo">SSS No.:</label><br />
	    <input type="text" id="txtSSSNo"/>
    </div>

    <div data-role="fieldcontain">
	    <label for="txtPhilHealthNo">Phil. Health No.:</label><br />
	    <input type="text" id="txtPhilHealthNo"/>
    </div>

    <div data-role="fieldcontain">
	    <label for="txtTIN">TIN:</label><br />
	    <input type="text" id="txtTIN"/>
    </div>

    <div data-role="fieldcontain">
	    <label for="txtHDMFNo">HDMF No.:</label><br />
	    <input type="text" id="txtHDMFNo"/>
    </div>

    <div data-role="fieldcontain">
	    <label for="txtNBINo">NBI No.:</label><br />
	    <input type="text" id="txtNBINo"/>
    </div>

    <div>
	    <div><label>Passport Information</label></div>
        <div data-role="fieldcontain">
	        <label for="txtPassportNo">Passport No.:</label><br />
	        <input type="text" id="txtPassportNo"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="txtPassportIssuedAt">Issued At:</label><br />
	        <input type="text" id="txtPassportIssuedAt"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="txtPassportIssuedOn">Issued On:</label><br />
	        <input type="text" readonly id="txtPassportIssuedOn"/>                
        </div>
    </div>

	<div style="text-align:center"> <a href="#" id="btnNext" data-role="button" onclick="ShowEmployeeInfo('{8A86CB3D-21F2-4736-870D-F8C8226DC0F4}')">Next</a> </div>
	<div style="text-align:center"> <a href="#" id="btnCancel" data-role="button">Cancel</a> </div>

</div>