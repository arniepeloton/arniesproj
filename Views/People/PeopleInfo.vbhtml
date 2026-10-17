@Code
    ViewData("Title") = "People Information"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script type="text/javascript" src="../../Scripts/sources/People/PeopleInfo.js"></script>
    
End Section

<div id="pagePeopleMenu" data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">
        <div><input id="btnNewPeopleInfo" type="button" value="New Personal Info"/></div>
        <div><input id="btnEditPeopleInfo" type="button" value="Edit Personal Info"/></div>
    </div>
</div>

<div id="pagePeopleInfo" data-role="page">

    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain">
	        <input type="text" readonly style="display:none;" id="txtID"/>                
	        <label for="txtPeopleID">Person ID:</label><br />
	        <input type="text" id="txtPeopleID"/>                
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
                <a id="btnBrowsePresentAddressCity" data-role="button"">Select City</a>
            </div>
            <div data-role="fieldcontain">
	            <label for="txtPresentAddressCountry">Country:</label><br />
	            <input type="text" readonly style="display:none;" id="txtPresentAddressCountryID"/>
	            <input type="text" readonly id="txtPresentAddressCountry"/>
                <a id="btnBrowsePresentAddressCountry" data-role="button">Select Country</a>
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
                <a id="btnBrowsePermanentAddressCity" data-role="button">Select City</a>
            </div>
            <div data-role="fieldcontain">
	            <label for="txtPermanentAddressCountry">Country:</label><br />
	            <input type="text" readonly style="display:none;" id="txtPermanentAddressCountryID"/>
	            <input type="text" readonly id="txtPermanentAddressCountry"/>
                <a id="btnBrowsePermanentAddressCountry" data-role="button">Select Country</a>
            </div>    
        </div>

        <div data-role="fieldcontain">
	        <label for="txtDateOfBirth">Date Of Birth:</label><br />
            <select id="selDateOfBirthMonth">
	            <option>Month</option>
	            <option value="0">January</option>
				<option value="1">February</option>
				<option value="2">March</option>
				<option value="3">April</option>
                <option value="4">May</option>
                <option value="5">June</option>
                <option value="6">July</option>
                <option value="7">August</option>
                <option value="8">September</option>
                <option value="9">October</option>
                <option value="10">November</option>
                <option value="11">December</option>
            </select>
            <select id="selDateOfBirthDay">	                            
	            <option>Day</option>
            </select>
            <select id="selDateOfBirthYear">	                            
	            <option>Year</option>
            </select>
        </div>

        <div data-role="fieldcontain">
	        <label for="txtPlaceOfBirth">Place Of Birth:</label><br />
	        <input type="text" readonly style="display:none;" id="txtPlaceOfBirthID"/>
	        <input type="text" readonly id="txtPlaceOfBirth"/>
            <a id="btnBrowsePlaceOfBirth" data-role="button">Select City</a>
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
            <a id="btnBrowseCitizenship" data-role="button">Select Citizenship</a>
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
                <a id="btnBrowseFatherOccupation" data-role="button">Select Occupation</a>
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
                <a id="btnBrowseMotherOccupation" data-role="button">Select Occupation</a>
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
                <a id="btnBrowseParentAddressCity" data-role="button">Select City</a>
            </div>
            <div data-role="fieldcontain">
	            <label for="txtParentAddressCountry">Country:</label><br />
	            <input type="text" readonly style="display:none;" id="txtParentAddressCountryID"/>
	            <input type="text" readonly id="txtParentAddressCountry"/>
                <a id="btnBrowseParentAddressCountry" data-role="button">Select Country</a>
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

	    <div style="text-align:center"> <a href="#" id="btnPeopleInfoNext" data-role="button"">Next</a> </div>
	    <div style="text-align:center"> <a href="#" id="btnPeopleInfoCancel" data-role="button">Cancel</a> </div>

    </div>
</div>

<div id="pageDialogPeopleSearch" data-role="page">

    <div data-role="header">
        <h1>Search Person</h1>
    </div>

    <div data-role="content">

        <div>
            <div><input id="txtSearchPerson" type="text" value="" /></div>
            <div><input id="btnSearchPerson" type="button" value="Search"/></div>
        </div>

    </div>

</div>

<div id="pageDialogCitySearch" data-role="page">

    <div data-role="header">
        <h1>Search City</h1>
    </div>

    <div data-role="content">

        <div>
            <div><input id="txtSearchCityFor" readonly style="display:none;" type="text"/></div>
            <div><input id="txtSearchCity" type="text" value="" /></div>
            <div><input id="btnSearchCity" type="button" value="Search"/></div>
        </div>

    </div>

</div>

<div id="pageDialogCountrySearch" data-role="page">

    <div data-role="header">
        <h1>Search Country</h1>
    </div>

    <div data-role="content">

        <div>
            <div><input id="txtSearchCountryFor" readonly style="display:none;" type="text"/></div>
            <div><input id="txtSearchCountry" type="text" value="" /></div>
            <div><input id="btnSearchCountry" type="button" value="Search"/></div>
        </div>

    </div>

</div>

<div id="pageDialogCitizenshipSearch" data-role="page">

    <div data-role="header">
        <h1>Search Citizenship</h1>
    </div>

    <div data-role="content">

        <div>
            <div><input id="txtSearchCitizenship" type="text" value="" /></div>
            <div><input id="btnSearchCitizenship" type="button" value="Search"/></div>
        </div>

    </div>

</div>

<div id="pageDialogOccupationSearch" data-role="page">

    <div data-role="header">
        <h1>Search Occupation</h1>
    </div>

    <div data-role="content">

        <div>
            <div><input id="txtSearchOccupationFor" readonly style="display:none;" type="text" value="" /></div>
            <div><input id="txtSearchOccupation" type="text" value="" /></div>
            <div><input id="btnSearchOccupation" type="button" value="Search"/></div>
        </div>

    </div>

</div>

<div id="pageResultSearch" data-role="page">
    
    <div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>Search Result</h1>
    </div>

    <br />
    <div id="divRecordCount"></div>

    <br />
    <ul data-role="listview" id="lstResultList">
    </ul>

</div>

<div id="pagePeopleContacts" data-role="page">

    <div data-role="header">
        <h1>Person Contacts</h1>
    </div>

    <div data-role="content">

        <div><input id="btnContactAdd" type="button" value="Add New Contact"/></div>
        <br />
        <div>
            <ul data-role="listview" id="lstContactList">
            </ul>
        </div>
        <br />
        <div><input id="btnContactNext" type="button" value="Next"/></div>
        <div><input id="btnContactPrev" type="button" value="Previous"/></div>

    </div>

</div>