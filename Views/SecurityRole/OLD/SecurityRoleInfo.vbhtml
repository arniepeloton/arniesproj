@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/SecurityRole/SecurityRolesInfo.js?crc=')
        ]);
    </script>

End Section

<div id="pageSecurityRoleInfo" data-role="page">

    <div data-role="header">
        <h1>Security Role Info</h1>
    </div>

    <div data-role="content" >

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtSecurityRoleID"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="txtRoleCode">Code: <span style="color: #FF0000;" >*</span></label>
            <input type="text" id="txtRoleCode" maxlength="15"/>
            
        </div>

        <div data-role="fieldcontain">
	        <label for="txtRole">Role: <span style="color: #FF0000;" >*</span></label>
            <input type="text" id="txtRole"/>
        </div>

        <div data-role="fieldcontain">
	        <label for="txtRoleDescription">Description: </label>
            <input type="text" id="txtRoleDescription"/>
        </div>

        <div data-role="fieldcontain">
            <label><input type="checkbox" id="chkRoleIsActive" /> Active</label>
        </div>

        <div data-role="collapsible" data-collapsed="false" data-theme="c" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
            <h2>
                <label>POS</label>
            </h2>
            <div id="divPOS">
                <ul id="lstPOS" data-role="listview" data-inset="true">
                </ul>
            </div>
        </div>

        <div data-role="collapsible" data-collapsed="false" data-theme="c" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
            <h2>
                <label>Receiving and Transfers</label>
            </h2>
            <div id="divReceivingAndTransfers">
                <ul id="lstReceivingAndTransfers" data-role="listview" data-inset="true">
                </ul>
            </div>
        </div>

        <div data-role="collapsible" data-collapsed="false" data-theme="c" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
            <h2>
                <label>Inventory Count</label>
            </h2>
            <div id="divInvCount">
                <ul id="lstInvCount" data-role="listview" data-inset="true">
                </ul>
            </div>
        </div>

        <div data-role="collapsible" data-collapsed="false" data-theme="c" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
            <h2>
                <label>SFA</label>
            </h2>
            <div id="divSFA">
                <ul id="lstSFA" data-role="listview" data-inset="true">
                </ul>
            </div>
        </div>

        <div data-role="collapsible" data-collapsed="false" data-theme="c" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
            <h2>
                <label>CRM</label>
            </h2>
            <div id="CRM">
                <ul id="lstCRM" data-role="listview" data-inset="true">
                </ul>
            </div>
        </div>

        <div data-role="collapsible" data-collapsed="false" data-theme="c" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
            <h2>
                <label>Dashboards and Reports</label>
            </h2>
            <div id="divDashboardsAndReports">
                <ul id="lstDashboardsAndReports" data-role="listview" data-inset="true">
                </ul>
            </div>
        </div>

        <div data-role="collapsible" data-collapsed="false" data-theme="c" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
            <h2>
                <label>System Administration</label>
            </h2>
            <div id="divSettings">
                <ul id="lstSettings" data-role="listview" data-inset="true">
                </ul>
            </div>
        </div>

          <div data-role="collapsible" data-collapsed="false" data-theme="c" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
            <h2>
                <label>Mobile Data Management</label>
            </h2>
            <div id="divMDM">
                <ul id="lstMDM" data-role="listview" data-inset="true">
                </ul>
            </div>
        </div>


        <div>
            <br />
            <br />
            <div style="text-align: center;">
                <a id="btnSecurityRoleInfoDone" data-role="button"><h3>Done</h3></a>
            </div>
        </div>

    </div>

</div>

