@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/People/PeopleInfoAlias.js?crc=')]);
    </script>

End Section

<div id="pagePeopleAliasNameList" data-role="page">
    <div data-role="header">
        <a class="btnProfileBackToHome" data-icon="home">Home</a>
        <a id="btnPeopleAliasPopUpMenu" data-rel="popup" data-inline="true" class="ui-btn-right">Go To</a>
        <h1>Person Alias List</h1>
    </div>

    <div data-role="content">
        <div data-role="fieldcontain"><a id="btnPeopleNameAliasAdd" data-role="button" data-icon="plus" data-inline="true">Add Alias</a></div>
        <div data-role="fieldcontain" id="divRecordCount"></div>
        <div>
            <ul data-role="listview" id="lstPeopleAlias" data-inset="true">
            </ul>
        </div>
        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPeopleAliasDone" data-role="button">Done</a> </div>
        </div>

        <div id="divPeopleAliasPopUpMenu" data-role="popup">
            <ul data-role="listview" data-inset="true" style="min-width: 210px;">
                <li data-role="divider">Menu</li>
                <li><a id="popPeopleAliasPeopleInfoName">Name</a></li>
                <li><a id="popPeopleAliasPeopleInfoAddress">Address</a></li>
                <li><a id="popPeopleAliasPeoplePersonalInfo">Personal Information</a></li>
                <li><a id="popPeopleAliasPeopleContact">Contact List</a></li>
                <li><a id="popPeopleAliasPeopleEducation">Educational Background</a></li>
                <li><a id="popPeopleAliasPeopleEmpHistory">Employment History</a></li>
                <li><a id="popPeopleAliasPeopleRelatedPerson">Related Person</a></li>
                <li><a id="popPeopleAliasPeoplePrimeIDReference">Primary ID Reference</a></li>
                <li><a id="popPeopleAliasPeopleUserAccount">User Account</a></li>
            </ul>
        </div>

    </div>

</div>


<div id="pagePeopleAliasName" data-role="page">
    <div data-role="header">
        <h1>Person Alias</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtAliasID"/>                
        </div>

        <div data-role="fieldcontain">
	        <div><label for="btnAliasTitle">Title:</label></div>
            <div style="display:none;"><input type="text" readonly id="txtAliasTitleID"/></div>
            <button id="btnAliasTitle" data-role="button">Please Select</button>
        </div>

        <div data-role="fieldcontain">
            <label for="txtAliasSurname">Surname (Last Name): <span style="color: #FF0000;" >*</span></label>
            <input type="text" id="txtAliasSurname"/>
        </div>

        <div data-role="fieldcontain">
            <label for="txtAliasFirstName">First Name: <span style="color: #FF0000;" >*</span></label>
            <input type="text" id="txtAliasFirstName"/>
        </div>

        <div data-role="fieldcontain">
            <label for="txtAliasMiddleName">Full Middle Name: <span style="color: #FF0000;" >*</span></label>
            <input type="text" id="txtAliasMiddleName"/>
        </div>

        <div data-role="fieldcontain">
	        <label for="selAliasPersonalSuffixName" class="select">Generational Suffix (Name):</label>
            <select id="selAliasPersonalSuffixName">
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

        <div>
            <br />
            <br />
	        <div style="text-align:center"><a id="btnPeopleAliasNameSave" data-role="button">Ok</a></div>
	        <div style="text-align:center"> <a id="btnPeopleAliasNameRemove" data-role="button">Remove</a> </div>
	        <div style="text-align:center"> <a id="btnPeopleAliasNameCancel" data-role="button">Cancel</a> </div>
        </div>

    </div>

</div>

