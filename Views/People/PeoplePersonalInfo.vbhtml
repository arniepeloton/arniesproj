@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/People/PeoplePersonalInfo.js?crc=')]);
    </script>
    
End Section

<div id="pagePeoplePersonalInfo" data-role="page">

    <div data-role="header">
        <a class="btnProfileBackToHome" data-icon="home">Home</a>
        <a id="btnPeoplePersonalInfoPopUpMenu" data-rel="popup" data-inline="true" class="ui-btn-right">Go To</a>
        <h1>Personal Information</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtID"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="selGender" class="select">Gender: <span style="color: #FF0000;" >*</span></label>
            <select id="selGender">
	            <option value="">Please Select</option>
	            <option value="Male">Male</option>
			    <option value="Female">Female</option>
			    <option value="Others">Others</option>
            </select>
        </div>

        <div data-role="fieldcontain">
	        <div><label for="btnCitizenship">Citizenship:</label></div>
            <div style="display:none;"><input type="text" readonly id="txtCitizenshipID"/></div>
            <button id="btnCitizenship" data-role="button">Please Select</button>
        </div>

        <div data-role="fieldcontain">
	        <div><label for="btnPOBCountry">Country Of Birth:</label></div>
            <div style="display:none;"><input type="text" readonly id="txtPOBCountryID"/></div>
           <button id="btnPOBCountry" data-role="button">Please Select</button>
        </div>

        <div data-role="fieldcontain">
	        <div><label for="btnPOBCity">Place Of Birth:</label></div>
            <div style="display:none;"><input type="text" readonly id="txtPOBCityID"/></div>
            <button id="btnPOBCity" data-role="button">Please Select</button>
        </div>

        <div data-role="fieldcontain">
	        <div><label for="btnHeight">Height:</label></div>
            <button id="btnHeight" data-role="button">Please Set</button>
        </div>

        <div data-role="fieldcontain">
	        <div><label for="btnWeight">Weight:</label></div>
            <button id="btnWeight" data-role="button">Please Set</button>
        </div>

        <div data-role="fieldcontain">
	        <label for="selCivilStatus" class="select">Civil Status:</label>
            <select id="selCivilStatus">
	            <option value="">Please Select</option>
	            <option value="Single">Single</option>
				<option value="Married">Married</option>
				<option value="Annulled">Annulled</option>
				<option value="Divorced">Divorced</option>
				<option value="Legally Separated">Legally Separated</option>
				<option value="Widow or Widower">Widow or Widower</option>
            </select>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center; display:none;"> <a id="btnPeopleInfoNext" data-role="button">Next</a> </div>
	        <div style="text-align:center; display:none;"> <a id="btnPeopleInfoBack" data-role="button">Back</a> </div>
	        <div style="text-align:center"> <a id="btnPeopleInfoDone" data-role="button">Done</a> </div>
        </div>

        <div id="divPeoplePersonalInfoPopUpMenu" data-role="popup">
            <ul data-role="listview" data-inset="true" style="min-width: 210px;">
                <li data-role="divider">Menu</li>
                <li><a id="popPeoplePersonalInfoPeopleInfoName">Name</a></li>
                <li><a id="popPeoplePersonalInfoPeopleInfoAlias">Alias</a></li>
                <li><a id="popPeoplePersonalInfoPeopleInfoAddress">Address</a></li>
                <li><a id="popPeoplePersonalInfoPeopleContact">Contact List</a></li>
                <li><a id="popPeoplePersonalInfoPeopleEducation">Educational Background</a></li>
                <li><a id="popPeoplePersonalInfoPeopleEmpHistory">Employment History</a></li>
                <li><a id="popPeoplePersonalInfoPeopleRelatedPerson">Related Person</a></li>
                <li><a id="popPeoplePersonalInfoPeoplePrimeIDReference">Primary ID Reference</a></li>
                <li><a id="popPeoplePersonalInfoPeopleUserAccount">User Account</a></li>
            </ul>
        </div>

    </div>

</div>

<div id="pageHeight" data-role="page">

    <div data-role="header">
        <h1>Height Details</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain">
            <div>
                <ul data-role="listview" data-inset="true">
				    <li data-role="list-divider">Foot Inch</li>
				    <li>
	                    <div>
                            <fieldset data-role="controlgroup">
	                            <label for="txtHeightFt">Foot:</label>
	                            <input type="number" id="txtHeightFt"/>
                            </fieldset>
                            
                            <fieldset data-role="controlgroup">
	                            <label for="txtHeightFtInc">Inch:</label>
	                            <input type="number" id="txtHeightFtInc"/>
                            </fieldset>
                        </div>  
                    </li>

				    <li data-role="list-divider">Centimeter</li>
				    <li>
	                    <div>
                            <label for="txtHeightCM">Centimeter:</label>
                            <input type="number" id="txtHeightCM"/>
                        </div>  
                    </li>

				    <li data-role="list-divider">Inch</li>
				    <li>
	                    <div>
                            <label for="txtHeightInc">Inch:</label>
                            <input type="number" id="txtHeightInc"/>
                        </div>
                    </li>

			    </ul>
            </div>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"><a id="btnHeightOk" data-role="button">Ok</a></div>
	        <div style="text-align:center"> <a id="btnHeightCancel" data-role="button">Cancel</a> </div>
        </div>

    </div>

</div>

<div id="pageWeight" data-role="page">

    <div data-role="header">
        <h1>Weight Details</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain">
            <div>
                <ul data-role="listview" data-inset="true">
				    <li data-role="list-divider">Kilogram</li>
				    <li>
	                    <div>
                            <label for="txtWeightKg">Kg:</label>
                            <input type="number" id="txtWeightKg"/>
                        </div>  
                    </li>

				    <li data-role="list-divider">Pounds</li>
				    <li>
	                    <div>
                            <label for="txtWeightLbs">Lbs:</label>
                            <input type="number" id="txtWeightLbs"/>
                        </div>  
                    </li>
			    </ul>
            </div>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"><a id="btnWeightOk" data-role="button">Ok</a></div>
	        <div style="text-align:center"> <a id="btnWeightCancel" data-role="button">Cancel</a> </div>
        </div>

    </div>

</div>
