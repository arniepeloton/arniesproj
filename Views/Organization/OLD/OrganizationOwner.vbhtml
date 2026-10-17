@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/Organization/OrganizationOwner.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryPeople.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryJobOccupation.js?crc=')
        ]);
    </script>
    
End Section

<div id="pageOwnerList" data-role="page">
    <div data-role="header">
        <h1>Directors, Officers and Stockholders, Partners or Owners</h1>
    </div>

    <div data-role="content">
        <div data-role="fieldcontain"><a id="btnAddOrgOwner" data-role="button" data-icon="plus" data-inline="true" style="height:80px; width:270px;">Add Directors, Officers and Stockholders, Partners or Owners</a></div>

        <div data-role="fieldcontain" id="divOrgOwnerRecordCount"></div>
        <div>
            <ul data-role="listview" id="lstOrgOwner" data-inset="true">
            </ul>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgOwnerDone" data-role="button"><h3>Done</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgOwnerInfo" data-role="page">

    <div data-role="header">
        <h1>Directors, Officers and Stockholders, Partners or Owners Info</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgOwnerID"/>                
        </div>

	    <div style="display:none;"><input type="text" readonly id="txtOrgOwnerPeopleID"/></div>
        <div data-role="fieldcontain">
	        <label for="btnOrgOwner">Owner: <span style="color: #FF0000;" >*</span></label>
            <fieldset data-role="controlgroup">
                <button id="btnOrgOwner" data-role="button">Please Select</button>
            </fieldset>
        </div>

        <div style="display:none;"><input type="text" readonly id="txtOrgOwnerJobOccupationID"/></div>
        <div data-role="fieldcontain">
	        <label for="btnOrgOwnerJobOccupation">Position: <span style="color: #FF0000;" >*</span></label>
            <fieldset data-role="controlgroup">
                <button id="btnOrgOwnerJobOccupation" data-role="button">Please Select</button>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
	        <label for="txtOrgOwnerDepartmentBusinessUnit">Department or Business Unit:</label>
            <fieldset data-role="controlgroup">
	            <input type="text" id="txtOrgOwnerDepartmentBusinessUnit"/>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <label><input type="checkbox" id="chkOrgOwnerIsIncorporator" /> Incorporator</label>
        </div>

        <div data-role="fieldcontain">
            <label><input type="checkbox" id="chkOrgOwnerIsDirector" /> Director</label>
        </div>

        <div data-role="fieldcontain">
            <label><input type="checkbox" id="chkOrgOwnerIsStockHolder" /> Stock Holder</label>
        </div>

        <div data-role="fieldcontain">
            <label><input type="checkbox" id="chkOrgOwnerIsOfficer" /> Officer</label>
        </div>

        <div data-role="fieldcontain">
            <label><input type="checkbox" id="chkOrgOwnerIsExecutiveCommittee" /> Executive Committee</label>
        </div>

        <div data-role="fieldcontain">
            <label><input type="checkbox" id="chkOrgOwnerIsOwner" /> Owner or Proprietor</label>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgOwnerSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center;">
                <a id="btnOrgOwnerRemove" data-role="button" style="display:none;"><h3>Remove</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgOwnerCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>

