<script>
    require([GetPathFormat('/Scripts/sources/Organization/OrganizationFinancialReport.js?v=' + assetversion)]);
</script>

<div id="pageOwnerList">
    <h1 class="display-6 mb-3">Financial Report Info</h1>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
            <input data-role="none" class="form-control" type="text" readonly id="txtOrgFinancialReportID" />
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="selOrgDOBMonth">
                Fiscal Year End: <span style="color: #FF0000;">*</span>
            </label>
            <div class="col-sm-1">
                <select data-role="none" class="form-select" id="selOrgFiscalMonthEnd">
                    <option value="">Month</option>
                </select>
            </div>
            <div class="col-sm-1">
                <select data-role="none" class="form-select" id="selOrgFiscalDayEnd">
                    <option value="">Day</option>
                </select>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtOrgExternalAuditor">Name of External Auditor:</label>
            <div class="col-sm-10">
                <input data-role="none" class="form-control" type="text" id="txtOrgExternalAuditor" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtOrgSingningPartnerExternalAuditor">Name of Signing Partner of External Auditor:</label>
            <div class="col-sm-10">
                <input data-role="none" class="form-control" type="text" id="txtOrgSingningPartnerExternalAuditor" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtOrgSECAccreditationNoOfAuditor">SEC Accreditation Number of External Auditor (If Applicable):</label>
            <div class="col-sm-10">
                <input data-role="none" class="form-control" type="text" id="txtOrgSECAccreditationNoOfAuditor" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="selOrgVATType">VAT Type:</label>
            <div class="col-sm-10">
                <select id="selOrgVATType" data-role="none" class="form-select">
                    <option value="">Please Select</option>
                    <option value="Zero-Rated Sale">Zero-Rated Sale</option>
                    <option value="VAT Exempt Sales">VAT Exempt Sales</option>
                    <option value="Sales Subject to VAT">Sales Subject to VAT</option>
                    <option value="Sales to Government">Sales to Government</option>
                </select>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="selOrgBIRTaxGroup">BIR Tax Group:</label>
            <div class="col-sm-10">
                <select id="selOrgBIRTaxGroup" data-role="none" class="form-select">
                    <option value="">Please Select</option>
                    <option value="Top 10,000 Private Corporations">Top 10,000 Private Corporations</option>
                    <option value="Top 5,000 Private Corporations">Top 5,000 Private Corporations</option>
                </select>
            </div>
        </div>

        <div style="text-align: center">
            <a id="btnOrgFinancialReportCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
            <a id="btnOrgFinancialReportDone" data-role="button" class="btn btn-success btn-lg">Save</a>
        </div>
    </div>
</div>