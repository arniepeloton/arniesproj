@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/Organization/OrganizationAffiliation.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryOrganization.js?crc=')
        ]);
    </script>
    
End Section

<div id="pageOrgAffiliationInfo" data-role="page">

    <div data-role="header">
        <h1>Intercompany Affiliations</h1>
    </div>

    <div data-role="content">

	    <div>Does this organization have a parent or holding company?</div>
        <select id="selOrgAffHasParentCompany" data-role="slider">
			<option value="0">No</option>
	        <option value="1">Yes</option>
        </select>

        <div id="divOrgAffParentCompanyDetails" style="display:none;">
            <div style="display:none;"><input type="text" readonly id="txtOrgAffHoldingCompanyID"/></div>
            <div data-role="fieldcontain">
	            <label for="btnOrgAffHoldingCompany">Parent or Holding Company: <span style="color: #FF0000;" >*</span></label>
                <fieldset data-role="controlgroup">
                    <button id="btnOrgAffHoldingCompany" data-role="button">Please Select</button>
                </fieldset>
            </div>
        </div>

        <div data-role="fieldcontain">
            <ul data-role="listview" id="lstOrgAffiliatesContainer" data-inset="true">     
                <li data-role="list-divider">Affiliate or Subsidiary Company</li>
                <li>
                    <div data-role = "fieldcontain"><a id="btnAddOrgAffiliates" data-role="button" data-icon="plus" data-inline="true">Add Affiliate or Subsidiary Company</a></div>
                    <div>
                        <ul data-role="listview" id="lstOrgAffiliates" data-inset="true">     
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgAffiliationDone" data-role="button"><h3>Done</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgAffiliatesInfo" data-role="page">

    <div data-role="header">
        <h1>Affiliate Company Info</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgAffiliateID"/>                
        </div>

        <div style="display:none;"><input type="text" readonly id="txtOrgAffiliateCompanyID"/></div>
        <div data-role="fieldcontain">
	        <label for="btnOrgAffiliateCompany">Affiliate or Subsidiary Company: <span style="color: #FF0000;" >*</span></label>
            <fieldset data-role="controlgroup">
                <button id="btnOrgAffiliateCompany" data-role="button">Please Select</button>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
	        <label for="txtOrgAffiliateRemarks">Remarks :</label>
            <fieldset data-role="controlgroup">
	            <input type="text" id="txtOrgAffiliateRemarks"/>
            </fieldset>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgAffiliateSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center;">
                <a id="btnOrgAffiliateRemove" data-role="button" style="display:none;"><h3>Remove</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgAffiliateCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>