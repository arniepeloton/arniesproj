@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/Organization/OrganizationFinancialReport.js?crc=')]);
    </script>

End Section

<div id="pageOwnerList" data-role="page">
    <div data-role="header">
        <h1>Financial Report info</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgFinancialReportID"/>                
        </div>

        <div data-role="fieldcontain">
	        <label>Fiscal Year End: <span style="color: #FF0000;" >*</span></label>
            <fieldset data-role="controlgroup" data-type="horizontal">
                <select id="selOrgFiscalMonthEnd">
	                <option value="">MMM</option>
                </select>
                <select id="selOrgFiscalDayEnd">
	                <option value="">DD</option>
                </select>                
            </fieldset>
        </div>

        <div data-role="fieldcontain">
	        <label for="txtOrgExternalAuditor">Name of External Auditor:</label>
            <input type="text" id="txtOrgExternalAuditor"/>
        </div>

        <div data-role="fieldcontain">
	        <label for="txtOrgSingningPartnerExternalAuditor">Name of Signing Partner of External Auditor:</label>
            <input type="text" id="txtOrgSingningPartnerExternalAuditor"/>

        </div>

        <div data-role="fieldcontain">
	        <label for="txtOrgSECAccreditationNoOfAuditor">SEC Accreditation Number of External Auditor (If Applicable):</label>
            <input type="text" id="txtOrgSECAccreditationNoOfAuditor"/>

        </div>

        <div data-role="fieldcontain">
	        <label for="selOrgVATType" class="select">VAT Type:</label>
            <select id="selOrgVATType">
	            <option value="">Please Select</option>
				<option value="Zero-Rated Sale">Zero-Rated Sale</option>
				<option value="VAT Exempt Sales">VAT Exempt Sales</option>
				<option value="Sales Subject to VAT">Sales Subject to VAT</option>
				<option value="Sales to Government">Sales to Government</option>
            </select>
        </div>

        <div data-role="fieldcontain">
	        <label for="selOrgBIRTaxGroup" class="select">BIR Tax Group:</label>
            <select id="selOrgBIRTaxGroup">
	            <option value="">Please Select</option>
				<option value="Top 10,000 Private Corporations">Top 10,000 Private Corporations</option>
				<option value="Top 5,000 Private Corporations">Top 5,000 Private Corporations</option>
            </select>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgFinancialReportDone" data-role="button"><h3>Done</h3></a>
            </div>
        </div>

    </div>

</div>
