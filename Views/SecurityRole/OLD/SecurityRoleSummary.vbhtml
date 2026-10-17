@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/SecurityRole/SecurityRolesSummary.js?crc=')]);
    </script>
    
End Section

<div id="pageSecurityRole" data-role="page">

    <div id"divHeader" data-role="header">
        <h1>Security Role Summary</h1>
        <a href="#" data-icon="arrow-l" id="btnBackSecurityRolesSummary">Back</a>
    </div>

    <div data-role="content" >

        <div>
            <ul data-role="listview" data-inset="true">

				<li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Security Role
                        <a id='btnEditSumSecRoleInfo' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div data-role="fieldcontain" style="display:none;">
	                    <input type="text" readonly id="txtSumSecurityRoleID"/>                
                    </div>

                    <div data-role="fieldcontain">
	                    <label for="txtSumRoleCode">Code: <span style="color: #FF0000;" >*</span></label>
                        <fieldset data-role="controlgroup">
	                        <div><input type="text" readonly id="txtSumRoleCode"/></div>
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
	                    <label for="txtSumRole">Role: <span style="color: #FF0000;" >*</span></label>
                        <fieldset data-role="controlgroup">
	                        <div><input type="text" readonly id="txtSumRole"/></div>
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
	                    <label for="txtSumRoleDescription">Description: </label>
                        <fieldset data-role="controlgroup">
	                        <div><input type="text" readonly id="txtSumRoleDescription"/></div>
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
                        <label><input type="checkbox" disabled id="chkSumRoleIsActive" /> Active</label>
                    </div>

                </li>

                <li id="lstSumSecurityRoleUserHeader" data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Security Role User
                        <a id='btnEditSumSecRoleUser' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div>
                        <ul data-role="listview" id="lstSumSecurityRoleUser" data-inset="true">   
                        </ul>
                    </div>
                </li>
			</ul>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center;">
                <a id="btnSecRoleSummaryDone" data-role="button"><h3>Done</h3></a>
            </div>
        </div>

    </div>

</div>

