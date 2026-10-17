@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/Organization/OrganizationEmployee.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryPeople.js?crc=')
        ]);
    </script>
    
End Section

<div id="pageOrgEmployee" data-role="page">

    @*<div id"divHeader" data-role="header">
        <h1>Employee List</h1>
    </div>*@

     <div data-role="header" data-position="fixed1" style="background: none white; border: none">
        <h1>Employee List</h1>

        <div class="ui-grid-d">
            <div class="ui-block-a" style="width: 10%;">
                <div id="btnHome" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>Home</span>
                </div>
            </div>
            <div class="ui-block-b" style="width: 30%;">
                <div id="btnSystemAdmin" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>System Administration</span>
                </div>
            </div>
            <div class="ui-block-c" style="width: 30%;">
                <div id="btnDataLibraries" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>Data Libraries</span>
                </div>
            </div>
            <div class="ui-block-d" style="width: 30%;">
                <div id="btnCompanyEmployee" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>Employee</span>
                </div>
            </div>
        </div>

    </div>

    <div data-role="content" >

        <div>
            <ul data-role="listview" id="lstOrgEmployeeContainer" data-inset="true">   
				<li data-role="list-divider">Employee List</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnOrgAddEmployee" data-role="button" data-icon="plus" data-inline="true">Add Employee</a></div>

                    <form id="frmOrgEmployeeSearch">
                        <div><input id="txtOrgEmployeeSearch" type="search" value="" /></div>
                    </form>

                    <div data-role="fieldcontain" id="divOrgRecordCountEmployee"></div>
                    <div>
                        <ul data-role="listview" id="lstOrgEmployee" data-inset="true">   
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgEmployeeDone" data-role="button"><h3>Go Back to Main</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgEmpGoToSetupChecklist" data-role="button"><h3>Go to Setup Checklist</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgEmployeeInfo" data-role="page">

    <div data-role="header">
        <h1>Employee Info</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgEmpID"/>                
        </div>

        <div style="display:none;"><input type="text" readonly id="txtOrgPeopleID"/></div>
        <div data-role="fieldcontain">
	        <label for="btnOrgEmployee">Employee:</label>
            <button id="btnOrgEmployee" data-role="button">Please Select</button>
          
        </div>

        <div data-role="fieldcontain">
	        <label for="selOrgDateHiredMonth">Date Hired: <span style="color: #FF0000;" >*</span></label>
            <fieldset data-role="controlgroup" data-type="horizontal">
                <select id="selOrgDateHiredMonth">
	                <option value="">MMM</option>
                </select>
                <select id="selOrgDateHiredDay">
	                <option value="">DD</option>
                </select>                
                <select id="selOrgDateHiredYear">
	                <option value="">YYYY</option>
                </select>                
            </fieldset>
        </div>

        <div data-role="fieldcontain">
	        <label for="selOrgDeactivate">Deactivate record?</label>
            <select id="selOrgDeactivate" data-role="slider">
				<option value="0">No</option>
	            <option value="1">Yes</option>
            </select>
        </div>

        <div id="divOrgDeactivateDetails" style="display:none;">

            <div data-role="fieldcontain">
	            <label for="selOrgDateInActiveMonth">Deactivation Date: <span style="color: #FF0000;" >*</span></label>
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <select id="selOrgDateInActiveMonth">
	                    <option value="">MMM</option>
                    </select>
                    <select id="selOrgDateInActiveDay">
	                    <option value="">DD</option>
                    </select>                
                    <select id="selOrgDateInActiveYear">
	                    <option value="">YYYY</option>
                    </select>                
                </fieldset>
            </div>

            <div data-role="fieldcontain">
	            <label for="txtOrgInActiveReason">Reason for deactivating:</label>
                <textarea id="txtOrgInActiveReason"></textarea>
            </div>

            <div data-role="fieldcontain">
	            <label for="txtOrgComments">Comments:</label>
                <textarea id="txtOrgComments"></textarea>
            </div>

            <div data-role="fieldcontain">
	            <label for="selOrgBan">Ban this record for future transactions?</label>
                <select id="selOrgBan" data-role="slider">
				    <option value="0">No</option>
	                <option value="1">Yes</option>
                </select>
            </div>
        </div>

        <div>
            <ul data-role="listview" id="lstOrgEmployeeFowstContainer" data-inset="true">
				<li data-role="list-divider">This employee will be assigned to the FOWST below</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnOrgAddEmployeeFowst" data-role="button" data-icon="plus" data-inline="true">Add Fowst</a></div>                    
                    <div>
                        <ul data-role="listview" id="lstOrgEmployeeFowst" data-inset="true" data-split-icon="delete">
                        </ul>
                    </div>
                </li>
            </ul>
        </div>   

        <div>
            <ul data-role="listview" id="lstOrgEmployeeRoleContainer" data-inset="true">
				<li data-role="list-divider">Employee Access Role</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnOrgAddEmployeeRole" data-role="button" data-icon="plus" data-inline="true">Add Role</a></div>                    
                    <div>
                        <ul data-role="listview" id="lstOrgEmployeeRole" data-inset="true" data-split-icon="delete">
                        </ul>
                    </div>
                </li>
            </ul>
        </div>   

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgEmpInfoSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgEmpInfoSaveAndInvite" data-role="button"><h3>Send Invite</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgEmpInfoSaveAndConfirm" data-role="button"><h3>Confirm employment by signing in now</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgEmpInfoCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>


<div id="pageOrgEmployeeSignIn" data-role="page">

    <div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>Sign In</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain">
	        <label for="txtUsername">Username:</label>
            <input type="text" id="txtUsername"/>
        </div>
        
        <div data-role="fieldcontain">
	        <label for="txtPassword">Password:</label>
            <input type="password" id="txtPassword"/>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgEmpSignIn" data-role="button"><h3>Sign In</h3></a>
            </div>
        </div>

    </div>

</div>