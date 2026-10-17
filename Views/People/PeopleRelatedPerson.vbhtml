@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/People/PeopleRelatedPerson.js?crc=')]);
    </script>

End Section

<div id="pagePeopleRelPersonList" data-role="page">
    <div data-role="header">
        <a class="btnProfileBackToHome" data-icon="home">Home</a>
        <a id="btnPeopleRelatedPopUpMenu" data-rel="popup" data-inline="true" class="ui-btn-right">Go To</a>
        <h1>Related Person</h1>
    </div>
    
    <div data-role="content">

        <div  data-role="fieldcontain"><a id="btnPeopleRelPersonAdd" data-role="button" data-icon="plus" data-inline="true">Add Related Person</a></div>

        <div data-role="fieldcontain" id="divRecordCount"></div>

        <div>
            <ul data-role="listview" id="lstPeopleRelPerson" data-inset="true">
            </ul>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPeopleRelPersonDone" data-role="button">Done</a> </div>
        </div>

        <div id="divPeopleRelatedPopUpMenu" data-role="popup">
            <ul data-role="listview" data-inset="true" style="min-width: 210px;">
                <li data-role="divider">Menu</li>
                <li><a id="popPeopleRelatedPeopleInfoName">Name</a></li>
                <li><a id="popPeopleRelatedPeopleInfoAlias">Alias</a></li>
                <li><a id="popPeopleRelatedPeopleInfoAddress">Address</a></li>
                <li><a id="popPeopleRelatedPeoplePersonalInfo">Personal Information</a></li>
                <li><a id="popPeopleRelatedPeopleContactNo">Contact List</a></li>
                <li><a id="popPeopleRelatedPeopleEducation">Educational Background</a></li>
                <li><a id="popPeopleRelatedPeopleEmpHistory">Employment History</a></li>
                <li><a id="popPeopleRelatedPeoplePrimeIDReference">Primary ID Reference</a></li>
                <li><a id="popPeopleRelatedPeopleUserAccount">User Account</a></li>
            </ul>
        </div>

    </div>

</div>

<div id="pagePeopleRelPersonInfo" data-role="page">
    <div data-role="header">
        <h1>Related Person Information</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtRelPersonID"/>                
        </div>

        <div data-role="fieldcontain">
	        <div><label for="btnRelPersonTitle">Title:</label></div>
            <div style="display:none;"><input type="text" readonly id="txtRelPersonTitleID"/></div>
            <fieldset data-role="controlgroup">
                <button id="btnRelPersonTitle" data-role="button">Please Select</button>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <label for="txtRelPersonSurname">Surname (Last Name):</label>
            <input type="text" id="txtRelPersonSurname"/>
        </div>

        <div data-role="fieldcontain">
            <label for="txtRelPersonFirstName">First Name:</label>
            <input type="text" id="txtRelPersonFirstName"/>
        </div>

        <div data-role="fieldcontain">
            <label for="txtRelPersonMiddleName">Full Middle Name:</label>
            <input type="text" id="txtRelPersonMiddleName"/>
        </div>

        <div data-role="fieldcontain">
	        <label for="selRelPersonSuffixName">Generational Suffix (Name):</label>
            <select id="selRelPersonSuffixName">
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

        <div>
            <ul data-role="listview" id="lstPeoplePostSuffixContainer" data-inset="true">   
                <li data-role="list-divider">Post Nominal Suffix</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnAddPostSuffix" data-role="button" data-icon="plus" data-inline="true">Add Post Nominal Suffix</a></div>
                    <div>
                        <ul data-role="listview" id="lstPeoplePostSuffix" data-inset="true" data-split-icon="delete" data-split-theme="d">   
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div data-role="fieldcontain">
	        <label for="selRelation">Relationship to this person:</label>
            <select id="selRelation">
	            <option value="">Please Select</option>
				<option value="Spouse">Spouse</option>
				<option value="Fiancé">Fiancé</option>
				<option value="Son">Son</option>
				<option value="Daughter">Daughter</option>
				<option value="Father">Father</option>
	            <option value="Mother">Mother</option>
				<option value="Father-in-Law">Father-in-Law</option>
	            <option value="Mother-in-Law">Mother-in-Law</option>
				<option value="Uncle">Uncle</option>
				<option value="Auntie">Auntie</option>
				<option value="Grand Father">Grand Father</option>
				<option value="Grand Mother">Grand Mother</option>
				<option value="Brother">Brother</option>
				<option value="Brother-In-Law">Brother-In-Law</option>
				<option value="Sister">Sister</option>
				<option value="Sister-In-Law">Sister-In-Law</option>
				<option value="Cousin">Cousin</option>
				<option value="Ex-Spouse">Ex-Spouse</option>
				<option value="Friend">Friend</option>
				<option value="Best Friend">Best Friend</option>
				<option value="Boy Friend">Boy Friend</option>
				<option value="Girl Friend">Girl Friend</option>
				<option value="Neighbour">Neighbour</option>
				<option value="Boss">Boss</option>
				<option value="Co-Worker">Co-Worker</option>
				<option value="Classmate">Classmate</option>
				<option value="Others">Others</option>
            </select>
        </div>

	    <label for="selEmergencyRef">Contact this person in case of emergency?</label>
        <select id="selEmergencyRef" data-role="slider">
			<option value="0">No</option>
	        <option value="1">Yes</option>
        </select>

        <div id="divEmergencyDetails" style="display:none;">

            <ul data-role="listview" data-inset="true" style="min-width: 210px;">
                <li data-role="list-divider">Telephone</li>
                <li>
                    <div data-role="fieldcontain">

                        <div class="ui-grid-a">
                            <div class="ui-block-a">
                                <div style="display:none;"><input type="text" readonly id="txtTelCountryID"/></div>
                                <button id="btnTelCountryCode" data-role="button">Country Code</button>
                            </div>

                            <div class="ui-block-b">
                                <div style="display:none;"><input type="text" readonly id="txtTelAreaCodeID"/></div>
                                <button id="btnTelAreaCode" data-role="button">Area Code</button>
                            </div>
                        </div>

                    </div>

                    <div data-role="fieldcontain">
                        <label for="txtTelContactNo">Telephone No.: <span style="color: #FF0000;" >*</span></label>
                        <input type="number" id="txtTelContactNo" maxlength="7"/>
                    </div>

                    <div data-role="fieldcontain">
                        <label for="txtContactExtension">Extension or Local No.:</label>
                        <input type="number" id="txtContactExtension" maxlength="10"/>
                    </div>

                    <div data-role="fieldcontain">
	                    <label for="selContactTypeTel">Contact Type: <span style="color: #FF0000;" >*</span></label>
                        <select id="selContactTypeTel">
	                        <option value="">Please Select</option>
	                        <option value="Work Telephone">Work Telephone</option>
				            <option value="Personal Telephone">Personal Telephone</option>
				            <option value="Third Party Telephone">Third Party Telephone</option>
                        </select>
                    </div>
                </li>

                <li data-role="list-divider">Mobile</li>
                <li>
                    <div data-role="fieldcontain">

                        <div class="ui-grid-a">
                            <div class="ui-block-a">
                                <div style="display:none;"><input type="text" readonly id="txtMobileCountryID"/></div>
                                <button id="btnMobileCountryCode" data-role="button">Country Code</button>
                            </div>

                            <div class="ui-block-b">
                                <div style="display:none;"><input type="text" readonly id="txtTelCoPrefixID"/></div>
                                <button id="btnMobileTelCoPrefixCode" data-role="button">Telco Code</button>
                            </div>
                        </div>

                    </div>

                    <div data-role="fieldcontain">
                        <label for="txtMobileContactNo">Mobile No.: <span style="color: #FF0000;" >*</span></label>
                        <input type="number" id="txtMobileContactNo" maxlength="7"/>
                    </div>

                    <div data-role="fieldcontain">
	                    <label for="selContactTypeMobile">Contact Type: <span style="color: #FF0000;" >*</span></label>
                        <select id="selContactTypeMobile">
	                        <option value="">Please Select</option>
	                        <option value="Work Mobile">Work Mobile</option>
				            <option value="Personal Mobile">Personal Mobile</option>
				            <option value="Third Party Mobile">Third Party Mobile</option>
                        </select>
                    </div>
                </li>

                <li data-role="list-divider">Email</li>
                <li>
                   <div data-role="fieldcontain">
                        <label for="txtEmailAddress">Email :</label>
                        <input type="text" id="txtEmailAddress"/>
                    </div>

                    <div data-role="fieldcontain">
	                    <label for="selContactTypeEmail">Email Type: <span style="color: #FF0000;" >*</span></label>
                        <select id="selContactTypeEmail">
	                        <option value="">Please Select</option>
	                        <option value="Work Email">Work Email</option>
				            <option value="Personal Email">Personal Email</option>
				            <option value="Third Party Email">Third Party Email</option>
                        </select>
                    </div>
                </li>
            </ul>

        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"><a id="btnRelPersonSave" data-role="button">Ok</a></div>
	        <div style="text-align:center"> <a id="btnRelPersonRemove" data-role="button">Remove</a> </div>
	        <div style="text-align:center"> <a id="btnRelPersonCancel" data-role="button">Cancel</a> </div>
        </div>

    </div>

</div>









