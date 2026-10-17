@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/People/PeopleEmpHistory.js?crc=')]);
    </script>
    
End Section

<div id="pagePeopleEmpHistList" data-role="page">
    <div data-role="header">
        <a id="btnPeopleEmpHistPopUpMenu" data-rel="popup" data-inline="true" class="ui-btn-right">Go To</a>
        <h1>Employment History List</h1>
    </div>

    <div data-role="content">
        <div data-role = "fieldcontain"><a id="btnPeopleEmpHistAdd" data-role="button" data-icon="plus" data-inline="true">Add Employment</a></div>
        <div data-role = "fieldcontain" id="divRecordCount"></div>
        <div>
            <ul data-role="listview" id="lstPeopleEmpHist" data-inset="true">
            </ul>
        </div>
        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPeopleEmpHistDone" data-role="button">Done</a> </div>
        </div>

        <div id="divPeopleEmpHistPopUpMenu" data-role="popup">
            <ul data-role="listview" data-inset="true" style="min-width: 210px;">
                <li data-role="divider">Menu</li>
                <li><a id="popPeopleEmpHistPeopleInfoName">Name</a></li>
                <li><a id="popPeopleEmpHistPeopleInfoAlias">Alias</a></li>
                <li><a id="popPeopleEmpHistPeopleInfoAddress">Address</a></li>
                <li><a id="popPeopleEmpHistPeoplePersonalInfo">Personal Information</a></li>
                <li><a id="popPeopleEmpHistPeopleContactNo">Contact List</a></li>
                <li><a id="popPeopleEmpHistPeopleEducation">Educational Background</a></li>
                <li><a id="popPeopleEmpHistPeopleRelatedPerson">Related Person</a></li>
                <li><a id="popPeopleEmpHistPeoplePrimeIDReference">Primary ID Reference</a></li>
                <li><a id="popPeopleEmpHistPeopleUserAccount">User Account</a></li>
            </ul>
        </div>

    </div>

</div>

<div id="pagePeopleEmpHistInfo" data-role="page">

    <div data-role="header">
        <h1>Employment Info</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtEmpHistID"/>                
        </div>

        <div data-role="fieldcontain">
            <label for="txtCompanyName">Company Name: <span style="color: #FF0000;" >*</span></label>
            <input type="text" id="txtCompanyName"/>

        </div>

        <div data-role="fieldcontain">
            <ul data-role="listview" id="lstPeopleEmpHistOccContainer" data-inset="true">     
                <li data-role="list-divider">Job Occupation</li>
                <li>
                    <div data-role = "fieldcontain"><a id="btnAddOccupation" data-role="button" data-icon="plus" data-inline="true">Add Job Occupation</a></div>
                    <div>
                        <ul data-role="listview" id="lstPeopleEmpHistOcc" data-inset="true">     
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPeopleEmpHistSave" data-role="button">Ok</a> </div>
	        <div style="text-align:center"> <a id="btnPeopleEmpHistRemove" data-role="button">Remove</a> </div>
	        <div style="text-align:center"> <a id="btnPeopleEmpHistCancel" data-role="button">Cancel</a> </div>
        </div>

    </div>

</div>

<div id="pagePeopleEmpHistJobOcc" data-role="page">

    <div data-role="header">
        <h1>Job Occupation Info</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtEmpHistJobOccID"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="btnJobOccupation">Job Occupation: <span style="color: #FF0000;" >*</span></label>
            <div style="display:none;"><input type="text" readonly id="txtJobOccupationID"/></div>
            <button id="btnJobOccupation" data-role="button">Please Select</button>
        </div>

        <div data-role="fieldcontain">
	        <label for="selDateStartMonth">Date Start: <span style="color: #FF0000;" >*</span></label>
            <fieldset data-role="controlgroup" data-type="horizontal">
                <select id="selDateStartMonth">
	                <option value="">MMM</option>
                </select>
                <select id="selDateStartDay">
	                <option value="">DD</option>
                </select>                
                <select id="selDateStartYear">
	                <option value="">YYYY</option>
                </select>                
            </fieldset>
        </div>

        <div data-role="fieldcontain">
	        <label for="selDateEndMonth">Date End: <span style="color: #FF0000;" >*</span></label>
            <fieldset data-role="controlgroup" data-type="horizontal">
                <select id="selDateEndMonth">
	                <option value="">MMM</option>
                </select>
                <select id="selDateEndDay">
	                <option value="">DD</option>
                </select>                
                <select id="selDateEndYear">
	                <option value="">YYYY</option>
                </select>                
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <label><input type="checkbox" id="chkIsUntilPresent" /> Continuous Employment</label>
        </div>


        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPeopleEmpHistJobOccSave" data-role="button">Ok</a> </div>
	        <div style="text-align:center"> <a id="btnPeopleEmpHistJobOccRemove" data-role="button">Remove</a> </div>
	        <div style="text-align:center"> <a id="btnPeopleEmpHistJobOccCancel" data-role="button">Cancel</a> </div>
        </div>

    </div>

</div>
