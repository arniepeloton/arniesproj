@Code
    ViewData("Title") = "Person Educational Background"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/People/PeopleEducation.js?crc=')]);
    </script>
    
End Section

<div id="pagePeopleEducList" data-role="page">
    <div data-role="header">
        <a id="btnPeopleEducationPopUpMenu" data-rel="popup" data-inline="true" class="ui-btn-right">Go To</a>
        <h1>Person Educational Background</h1>
    </div>

    <div data-role="content">
        <div data-role="fieldcontain"><a id="btnPeopleEducAdd" data-role="button" data-icon="plus" data-inline="true">Add Educational Background</a></div>
        <div data-role="fieldcontain" id="divRecordCount"></div>
        <div>
            <ul data-role="listview" id="lstPeopleEduc" data-inset="true">
            </ul>
        </div>
        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPeopleEducDone" data-role="button">Done</a> </div>
        </div>

        <div id="divPeopleEducationPopUpMenu" data-role="popup">
            <ul data-role="listview" data-inset="true" style="min-width: 210px;">
                <li data-role="divider">Menu</li>
                <li><a id="popPeopleEducationPeopleInfoName">Name</a></li>
                <li><a id="popPeopleEducationPeopleInfoAlias">Alias</a></li>
                <li><a id="popPeopleEducationPeopleInfoAddress">Address</a></li>
                <li><a id="popPeopleEducationPeoplePersonalInfo">Personal Information</a></li>
                <li><a id="popPeopleEducationPeopleContactNo">Educational Background</a></li>
                <li><a id="popPeopleEducationPeopleEmpHistory">Employment History</a></li>
                <li><a id="popPeopleEducationPeopleRelatedPerson">Related Person</a></li>
                <li><a id="popPeopleEducationPeoplePrimeIDReference">Primary ID Reference</a></li>
                <li><a id="popPeopleEducationPeopleUserAccount">User Account</a></li>
            </ul>
        </div>

    </div>

</div>

<div id="pagePeopleEducInfo" data-role="page">

    <div data-role="header">
        <h1>Educational Background Info</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtEducationID"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="selAcademicLevel" class="select">Academic Level: <span style="color: #FF0000;" >*</span></label>
            <select id="selAcademicLevel">
	            <option value="">Please Select</option>
	            <option value="Kindergarten">Kindergarten</option>
	            <option value="Elementary">Elementary</option>
	            <option value="High School">High School</option>
				<option value="Vocational">Vocational</option>
				<option value="College or University">College or University</option>
				<option value="Graduate or Post Graduate">Graduate or Post Graduate</option>
            </select>
        </div>

        <div data-role="fieldcontain">
            <div style="display:none;"><input type="text" readonly id="txtEducationCourseID"/></div>
	        <label for="btnEducationCourse">Course: <span style="color: #FF0000;" >*</span></label>
            <button id="btnEducationCourse" data-role="button">Please Select</button>
        </div>

        <div data-role="fieldcontain">
            <label for="txtInstitutionName">Institution Name: <span style="color: #FF0000;" >*</span></label>
            <input type="text" id="txtInstitutionName"/>
        </div>

        <div data-role="fieldcontain">
            <label for="txtHouseBuildingNo">House or Building No.:</label>
            <input type="text" id="txtHouseBuildingNo"/>
        </div>

        <div data-role="fieldcontain">
            <label for="txtAddressStreet">Street: <span style="color: #FF0000;" >*</span></label>
            <input type="text" id="txtAddressStreet"/>
        </div>

        <div data-role="fieldcontain">
	        <label for="btnAddressCity">City or Town: <span style="color: #FF0000;" >*</span></label>
            <div style="display:none;"><input type="text" readonly id="txtAddressCityID"/></div>
            <button id="btnAddressCity" data-role="button">Please Select</button>
        </div>

        <div data-role="fieldcontain">
	        <label for="btnAddressCountry">Country: <span style="color: #FF0000;" >*</span></label>
            <div style="display:none;"><input type="text" readonly id="txtAddressCountryID"/></div>
            <button id="btnAddressCountry" data-role="button">Please Select</button>
        </div>

        <div data-role="fieldcontain">
	        <label for="selYearStarted" class="select">Year Started: <span style="color: #FF0000;" >*</span></label>
            <select id="selYearStarted">
	            <option value="">YYYY</option>
            </select>                
        </div>

        <div data-role="fieldcontain">
	        <label for="selYearGraduated" class="select">Year Ended: <span style="color: #FF0000;" >*</span></label>
            <select id="selYearGraduated">
	            <option value="">YYYY</option>
            </select>                
        </div>

        <div data-role="fieldcontain">
	        <label for="selGraduated">Graduated</label>
            <select id="selGraduated" data-role="slider">
				<option value="0">No</option>
	            <option value="1">Yes</option>
            </select>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPeopleEducSave" data-role="button">Ok</a> </div>
	        <div style="text-align:center"> <a id="btnPeopleEducRemove" data-role="button">Remove</a> </div>
	        <div style="text-align:center"> <a id="btnPeopleEducCancel" data-role="button">Cancel</a> </div>
        </div>

    </div>

</div>
