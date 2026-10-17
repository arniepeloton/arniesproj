@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/People/PeopleInfoName.js?crc=')]);
    </script>
    
End Section

<div id="pagePeopleName" data-role="page">

    <div id"divHeader" data-role="header">
        <a id="btnPeopleInfoPopUpMenu" data-rel="popup" data-inline="true" class="ui-btn-right">Go To</a>
        <h1>Person Name and Birth Date</h1>
    </div>

    <div data-role="content" >

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtID"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="btnTitle">Title:</label>
            <div style="display:none;"><input type="text" readonly id="txtTitleID"/></div>
            <button id="btnTitle" data-role="button">Please Select</button>
        </div>

        <div data-role="fieldcontain">
            <label for="txtSurname">Surname (Last Name): <span style="color: #FF0000;" >*</span></label>
            <input type="text" id="txtSurname"/>
        </div>

        <div data-role="fieldcontain">
            <label for="txtFirstName">First Name: <span style="color: #FF0000;" >*</span></label>
            <input type="text" id="txtFirstName"/>
        </div>

        <div data-role="fieldcontain">
            <label for="txtMiddleName">Full Middle Name: <span style="color: #FF0000;" >*</span></label>
            <input type="text" id="txtMiddleName"/>
        </div>

        <div data-role="fieldcontain">
	        <label for="selPersonalSuffixName" class="select">Generational Suffix (Name):</label>
            <select id="selPersonalSuffixName">
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
           
	        <label for="selDOBMonth">Date Of Birth: <span style="color: #FF0000;" >*</span></label>
            <fieldset data-role="controlgroup" data-type="horizontal">
                    <select id="selDOBMonth">
	                    <option value="">MMM</option>
                    </select>
                    <select id="selDOBDay">
	                    <option value="">DD</option>
                    </select>                
                    <select id="selDOBYear">
	                    <option value="">YYYY</option>
                    </select>                           
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <label for="txtNickName">Nick Name:</label>
            <input type="text" id="txtNickName"/>
        </div>

        <div>
            <ul data-role="listview" data-inset="true">
			    <li data-role="list-divider">Mother's Maiden Name</li>
				<li>
                    <div data-role="fieldcontain">
                        <label for="txtMotherMaidenSurname">Mother's Maiden Surname (Last Name): <span style="color: #FF0000;" >*</span></label>
                        <input type="text" id="txtMotherMaidenSurname"/>
                    </div>

                    <div data-role="fieldcontain">
                        <label for="txtMotherMaidenFirstName">Mother's First Name: <span style="color: #FF0000;" >*</span></label>
                        <input type="text" id="txtMotherMaidenFirstName"/>
                    </div>

                    <div data-role="fieldcontain">
                        <label for="txtMotherMaidenMiddleName">Mother's Maiden Middle Name:</label>
                        <input type="text" id="txtMotherMaidenMiddleName"/>
                    </div>
				</li>

			</ul>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPeopleNameDone" data-role="button">Done</a> </div>
        </div>

        <div id="divPeopleInfoPopUpMenu" data-role="popup">
            <ul data-role="listview" data-inset="true" style="min-width: 210px;">
                <li data-role="divider">Menu</li>
                <li><a id="popPeopleInfoPeopleInfoAlias">Alias</a></li>
                <li><a id="popPeopleInfoPeopleInfoAddress">Address</a></li>
                <li><a id="popPeopleInfoPeoplePersonalInfo">Personal Information</a></li>
                <li><a id="popPeopleInfoPeopleContact">Contact List</a></li>
                <li><a id="popPeopleInfoPeopleEducation">Educational Background</a></li>
                <li><a id="popPeopleInfoPeopleEmpHistory">Employment History</a></li>
                <li><a id="popPeopleInfoPeopleRelatedPerson">Related Person</a></li>
                <li><a id="popPeopleInfoPeoplePrimeIDReference">Primary ID Reference</a></li>
                <li><a id="popPeopleInfoPeopleUserAccount">User Account</a></li>
            </ul>
        </div>

    </div>

</div>

<div id="pagePeopleConflictList" data-role="page">
    <div data-role="header">
        <h1>Person Conflict Information</h1>
    </div>

    <div data-role="content">
        <div data-role="fieldcontain" id="divConflictRecordCount"></div>
        <div>
            <ul data-role="listview" id="lstPeopleConflict" data-inset="true">
            </ul>
        </div>
        <div data-role="fieldcontain">Do you want to continue?</div>
        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPeopleConflictYes" data-role="button">Yes</a> </div>
	        <div style="text-align:center"> <a id="btnPeopleConflictNo" data-role="button">No</a> </div>
        </div>

    </div>

</div>