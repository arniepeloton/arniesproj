@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/People/PeopleInfoSummary.js?crc=')]);
    </script>

End Section

<div id="pagePeopleName" data-role="page">

    <div id"divHeader" data-role="header">
        <h1>Person Profile Summary</h1>
         <a href="#" data-icon="home" class="btnProfileBackToHome">Home</a>
    </div>

    <div data-role="content" >

        <div>
            <ul data-role="listview" data-inset="true">

				<li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Name
                        <a id='btnSumPeopleInfoName' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div data-role="fieldcontain" style="display:none;">
	                    <input type="text" readonly id="txtID"/>                
                    </div>

                    <div data-role="fieldcontain">
                        <label for="txtTitle">Title:</label>
	                    <input type="text" readonly id="txtTitle"/>
                    </div>

                    <div data-role="fieldcontain">
	                     <label for="txtSurname">Surname (Last Name):</label>
	                     <input type="text" readonly id="txtSurname"/>
                    </div>

                    <div data-role="fieldcontain">
	                     <label for="txtFirstName">First Name:</label>
	                     <input type="text" readonly id="txtFirstName"/>             
                    </div>

                    <div data-role="fieldcontain">
                        <label for="txtMiddleName">Full Middle Name:</label>
	                    <input type="text" readonly id="txtMiddleName"/>
                    </div>

                    <div data-role="fieldcontain">
                        <label for="txtPersonalSuffixName">Generational Suffix (Name):</label>
	                    <input type="text" readonly id="txtPersonalSuffixName"/>
                    </div>

                    <div data-role="fieldcontain">
                        <label for="txtProfSuffixName">Post Nominal Suffix:</label>
                        <input type="text" readonly id="txtProfSuffixName"/>
                    </div>

                    <div data-role="fieldcontain">
                        <label for="txtDateOfBirth">Date of Birth:</label>
                        <input type="text" readonly id="txtDateOfBirth"/>
                    </div>

                    <div data-role="fieldcontain">
                        <label for="txtNickName">Nick Name:</label>
                        <input type="text" readonly id="txtNickName"/>
                    </div>

                    <div>
                        <ul data-role="listview" data-inset="true">
			                <li data-role="list-divider">Mother's Maiden Name</li>
				            <li>
                                <div data-role="fieldcontain">
                                    <label for="txtMotherMaidenSurname">Mother's Maiden Surname (Last Name): <span style="color: #FF0000;" >*</span></label>
                                    <input type="text" readonly id="txtMotherMaidenSurname"/>
                                </div>

                                <div data-role="fieldcontain">
                                    <label for="txtMotherMaidenFirstName">Mother's First Name: <span style="color: #FF0000;" >*</span></label>
                                    <input type="text" readonly id="txtMotherMaidenFirstName"/>
                                </div>

                                <div data-role="fieldcontain">
                                    <label for="txtMotherMaidenMiddleName">Mother's Maiden Middle Name: <span style="color: #FF0000;" >*</span></label>
                                    <input type="text" readonly id="txtMotherMaidenMiddleName"/>
                                </div>
				            </li>

			            </ul>
                    </div>

                </li>

                <li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Alias
                        <a id='btnSumPeopleInfoAlias' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div>
                        <ul data-role="listview" id="lstPeopleAlias" data-inset="true">
                        </ul>
                    </div>
                </li>

                <li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Address
                        <a id='btnSumPeopleInfoAddress' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div>
                        <ul data-role="listview" id="lstPeopleAddress" data-inset="true">
                        </ul>
                    </div>
                </li>

                <li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Personal Info
                        <a id='btnSumPeoplePersonalInfo' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div data-role="fieldcontain">
                        <label for="txtGender">Gender:</label>
                        <input type="text" readonly id="txtGender"/>
                    </div>

                    <div data-role="fieldcontain">
                        <div style="display:none;"><input type="text" readonly id="txtCitizenshipID"/></div>
                        <label for="txtCitizenship">Citizenship:</label>
                        <input type="text" readonly id="txtCitizenship"/>
                    </div>

                    <div data-role="fieldcontain">
                        <div style="display:none;"><input type="text" readonly id="txtPOBCountryID"/></div>
                        <label for="txtPOBCountry">Country Of Birth:</label>
                        <input type="text" readonly id="txtPOBCountry"/>
                    </div>

                    <div data-role="fieldcontain">
                        <div style="display:none;"><input type="text" readonly id="txtPOBCityID"/></div>
                        <label for="txtPOBCity">Place Of Birth:</label>
                        <input type="text" readonly id="txtPOBCity"/>
                    </div>

                    <div data-role="fieldcontain">
                        <label for="txtHeight">Height:</label>
                        <input type="text" readonly id="txtHeight"/>
                    </div>

                    <div data-role="fieldcontain">
                        <label for="txtWeight">Weight:</label>
                        <input type="text" readonly id="txtWeight"/>
                    </div>

                    <div data-role="fieldcontain">
                        <label for="txtCivilStatus">Civil Status:</label>
                        <input type="text" readonly id="txtCivilStatus"/>
                    </div>
                </li>

                <li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Contact
                        <a id='btnSumPeopleContactNo' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div>
                        <ul data-role="listview" id="lstPeopleContact" data-inset="true">
			            </ul>
                    </div>
                </li>
                        
                <li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Educational Background
                        <a id='btnSumPeopleEducation' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div>
                        <ul data-role="listview" id="lstPeopleEduc" data-inset="true">
                        </ul>
                    </div>
                </li>

                <li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Employement History
                        <a id='btnSumPeopleEmpHistory' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div>
                        <ul data-role="listview" id="lstPeopleEmpHist" data-inset="true">
                        </ul>
                    </div>
                </li>

                <li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Related Person
                        <a id='btnSumPeopleRelatedPerson' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div>
                        <ul data-role="listview" id="lstPeopleRelPerson" data-inset="true">
                        </ul>
                    </div>
                </li>

                <li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Primary ID Reference
                        <a id='btnSumPeoplePrimeIDReference' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div data-role="fieldcontain">
                        <label for="txtResCertNo">Residence Certification No.:</label>
                        <input type="text" readonly id="txtResCertNo"/>
                    </div>

                    <div data-role="fieldcontain">
	                    <label for="txtSSSNo">SSS No.:</label>  
	                    <input type="text" readonly id="txtSSSNo"/>     
                    </div>

                    <div data-role="fieldcontain">
	                    <label for="txtPhilHealthNo">Phil. Health No.:</label>     
	                    <input type="text" readonly id="txtPhilHealthNo"/>    
                    </div>

                    <div data-role="fieldcontain">
	                    <label for="txtTIN">TIN:</label>
	                    <input type="text" readonly id="txtTIN"/>
                    </div>

                    <div data-role="fieldcontain">
	                    <label for="txtHDMFNo">HDMF No.:</label>
	                    <input type="text" readonly id="txtHDMFNo"/>
                    </div>

                    <div data-role="fieldcontain">
	                    <label for="txtNBINo">NBI No.:</label>
	                    <input type="text" readonly id="txtNBINo"/>
                    </div>

                    <div data-role="fieldcontain">
                        <label for="txtPassportNo">Passport No.:</label>
                        <input type="text" readonly id="txtPassportNo"/>
                    </div>
                </li>
				<li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        User Account
                        <a id='btnSumPeopleUserAccount' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        DTR PIN
                        <a id='btnDTRPIN' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
			</ul>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align:center"> <a id="btnPeopleSummaryDone" data-role="button">Done</a> </div>
        </div>
    </div>

</div>

