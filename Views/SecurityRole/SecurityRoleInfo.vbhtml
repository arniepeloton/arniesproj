<script>
    require([GetPathFormat('/Scripts/sources/SecurityRole/SecurityRolesInfo.js?v=' + assetversion)
    ]);
</script>

<div data-role="content">
    <h1 class="display-6 mb-4">
        Security Role Info
    </h1>

    <div data-role="fieldcontain" style="display:none;">
        <input type="text" readonly data-role="none" class="form-control" id="txtSecurityRoleID" />
    </div>

    <div class="row mb-3">
        <label class="col-md-2 col-form-label" for="txtSumRoleCode">Code: <span style="color: #FF0000;">*</span></label>
        <div class="col-md-10">
            <input type="text" data-role="none" class="form-control" id="txtRoleCode" maxlength="15" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-md-2 col-form-label" for="txtRole">Role: <span style="color: #FF0000;">*</span></label>
        <div class="col-md-10">
            <input type="text" data-role="none" class="form-control" id="txtRole" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-md-2 col-form-label" for="txtRoleDescription">Description:</label>
        <div class="col-md-10">
            <input type="text" data-role="none" class="form-control" id="txtRoleDescription" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-auto col-md-2" for="chkRoleIsActive">Active?</label>
        <div class="col col-md-10">
            <div class="form-check form-switch mb-3" style="font-size:20px;">
                <input class="form-check-input" data-role="none" type="checkbox" id="chkRoleIsActive">
            </div>
        </div>
    </div>

    @*<div id="divTabSecurityRole" style="white-space:nowrap; overflow:auto;">
        <ul class="nav nav-tabs" role="tablist" style="flex-wrap:nowrap;">
            <li class="nav-item" role="presentation">
                <button data-role="none" class="nav-link active" data-toggle="tab" data-target="#tabSecurityRoleInfoPos">Mobiz Complete Retail (MCR)</button>
            </li>
            <li class="nav-item" role="presentation">
                <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabSecurityRoleInfoEas">Enterprise Application System (EAS)</button>
            </li>
        </ul>
    </div>*@

    <div class="tab-content">
        <div id="tabSecurityRoleInfoPos" class="tab-pane fade show active pt-3">
            <div class="accordion mb-3 colMCR">
                <div class="card">
                    <div class="card-header p-0">
                        <div class="row g-0 align-items-center">
                            <div class="col">
                                <button data-target="#divPOS" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                    POS
                                </button>
                            </div>
                            <div class="col-auto">
                                <div class="btn-group me-3 pr-3 pl-3 d-none" title="Show/Hide this menu according to the current access role" data-toggle="tooltip" accessrightmenu="POS">
                                    <input data-role="none" type="radio" class="btn-check" id="radShowPOS" value="Show" name="radAccesRightPOS" checked />
                                    <label class="btn btn-outline-secondary" for="radShowPOS">Show</label>
                                    <input data-role="none" type="radio" class="btn-check" id="radHidePOS" value="Hide" name="radAccesRightPOS" />
                                    <label class="btn btn-outline-secondary" for="radHidePOS">Hide</label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="divPOS" class="collapse show">
                        <div class="card-body">
                            <ul id="lstPOS" data-role="none" class="list-group mb-3"></ul>
                        </div>
                    </div>
                </div>
            </div>

            <div class="accordion mb-3 colMCR">
                <div class="card">
                    <div class="card-header p-0">
                        <div class="row g-0 align-items-center">
                            <div class="col">
                                <button data-target="#divPO" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                    Purchase Order
                                </button>
                            </div>
                            <div class="col-auto">
                                <div class="btn-group me-3 pr-3 pl-3 d-none" title="Show/Hide this menu according to the current access role" data-toggle="tooltip" accessrightmenu="PO">
                                    <input data-role="none" type="radio" class="btn-check" id="radShowPO" value="Show" name="radAccesRightPO" checked />
                                    <label class="btn btn-outline-secondary" for="radShowPO">Show</label>
                                    <input data-role="none" type="radio" class="btn-check" id="radHidePO" value="Hide" name="radAccesRightPO" />
                                    <label class="btn btn-outline-secondary" for="radHidePO">Hide</label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="divPO" class="collapse show">
                        <div class="card-body">
                            <ul id="lstPO" data-role="none" class="list-group mb-3"></ul>
                        </div>
                    </div>
                </div>
            </div>

            <div class="accordion mb-3 colMCR">
                <div class="card">
                    <div class="card-header p-0">
                        <div class="row g-0 align-items-center">
                            <div class="col">
                                <button data-target="#divReceivingAndTransfers" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                    Receiving and Transfers
                                </button>
                            </div>
                            <div class="col-auto">
                                <div class="btn-group me-3 pr-3 pl-3 d-none" title="Show/Hide this menu according to the current access role" data-toggle="tooltip" accessrightmenu="Receiving and Transfers">
                                    <input data-role="none" type="radio" class="btn-check" id="radShowRecTrans" value="Show" name="radAccesRightRecTrans" checked />
                                    <label class="btn btn-outline-secondary" for="radShowRecTrans">Show</label>
                                    <input data-role="none" type="radio" class="btn-check" id="radHideRecTrans" value="Hide" name="radAccesRightRecTrans" />
                                    <label class="btn btn-outline-secondary" for="radHideRecTrans">Hide</label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="divReceivingAndTransfers" class="collapse show">
                        <div class="card-body">
                            <ul id="lstReceivingAndTransfers" data-role="none" class="list-group mb-3"></ul>
                        </div>
                    </div>
                </div>
            </div>

            <div class="accordion mb-3 colMCR">
                <div class="card">
                    <div class="card-header p-0">
                        <div class="row g-0 align-items-center">
                            <div class="col">
                                <button data-target="#divInvCount" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                    Inventory Count
                                </button>
                            </div>
                            <div class="col-auto">
                                <div class="btn-group me-3 pr-3 pl-3 d-none" title="Show/Hide this menu according to the current access role" data-toggle="tooltip" accessrightmenu="Inventory Count">
                                    <input data-role="none" type="radio" class="btn-check" id="radShowIC" value="Show" name="radAccesRightIC" checked />
                                    <label class="btn btn-outline-secondary" for="radShowIC">Show</label>
                                    <input data-role="none" type="radio" class="btn-check" id="radHideIC" value="Hide" name="radAccesRightIC" />
                                    <label class="btn btn-outline-secondary" for="radHideIC">Hide</label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="divInvCount" class="collapse show">
                        <div class="card-body">
                            <ul id="lstInvCount" data-role="none" class="list-group mb-3"></ul>
                        </div>
                    </div>
                </div>
            </div>

            @*<div class="accordion mb-3 colMCR">
                <div class="card">
                    <div class="card-header p-0">
                        <button data-target="#divSFA" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                            SFA
                        </button>
                    </div>
                    <div id="divSFA" class="collapse show">
                        <div class="card-body">
                            <ul id="lstSFA" data-role="none" class="list-group mb-3"></ul>
                        </div>
                    </div>
                </div>
            </div>*@

            @*<div class="accordion mb-3 colMCR">
                <div class="card">
                    <div class="card-header p-0">
                        <button data-target="#CRM" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                            CRM
                        </button>
                    </div>
                    <div id="CRM" class="collapse show">
                        <div class="card-body">
                            <ul id="lstCRM" data-role="none" class="list-group mb-3"></ul>
                        </div>
                    </div>
                </div>
            </div>*@

            <div class="accordion mb-3 colMCR">
                <div class="card">
                    <div class="card-header p-0">
                        <div class="row g-0 align-items-center">
                            <div class="col">
                                <button data-target="#divRequestsAndApprovals" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                    Requests and Approvals
                                </button>
                            </div>
                            <div class="col-auto">
                                <div class="btn-group me-3 pr-3 pl-3 d-none" title="Show/Hide this menu according to the current access role" data-toggle="tooltip" accessrightmenu="Requests and Approvals">
                                    <input data-role="none" type="radio" class="btn-check" id="radShowReqApp" value="Show" name="radAccesRightReqApp" checked />
                                    <label class="btn btn-outline-secondary" for="radShowReqApp">Show</label>
                                    <input data-role="none" type="radio" class="btn-check" id="radHideReqApp" value="Hide" name="radAccesRightReqApp" />
                                    <label class="btn btn-outline-secondary" for="radHideReqApp">Hide</label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="divRequestsAndApprovals" class="collapse show">
                        <div class="card-body">
                            <ul id="lstRequestsAndApprovals" data-role="none" class="list-group mb-3"></ul>
                        </div>
                    </div>
                </div>
            </div>

            <div class="accordion mb-3 colMCR">
                <div class="card">
                    <div class="card-header p-0">
                        <div class="row g-0 align-items-center">
                            <div class="col">
                                <button data-target="#divSalesReturns" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                    Sales Returns
                                </button>
                            </div>
                            <div class="col-auto">
                                <div class="btn-group me-3 pr-3 pl-3 d-none" title="Show/Hide this menu according to the current access role" data-toggle="tooltip" accessrightmenu="Sales Returns">
                                    <input data-role="none" type="radio" class="btn-check" id="radShowSR" value="Show" name="radAccesRightSR" checked />
                                    <label class="btn btn-outline-secondary" for="radShowSR">Show</label>
                                    <input data-role="none" type="radio" class="btn-check" id="radHideSR" value="Hide" name="radAccesRightSR" />
                                    <label class="btn btn-outline-secondary" for="radHideSR">Hide</label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="divSalesReturns" class="collapse show">
                        <div class="card-body">
                            <ul id="lstSalesReturns" data-role="none" class="list-group mb-3"></ul>
                        </div>
                    </div>
                </div>
            </div>

            <div class="accordion mb-3 colMCR">
                <div class="card">
                    <div class="card-header p-0">
                        <div class="row g-0 align-items-center">
                            <div class="col">
                                <button data-target="#divDashboardsAndReports" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                    Dashboards and Reports
                                </button>
                            </div>
                            <div class="col-auto">
                                <div class="btn-group me-3 pr-3 pl-3 d-none" title="Show/Hide this menu according to the current access role" data-toggle="tooltip" accessrightmenu="Dashboard and Reports">
                                    <input data-role="none" type="radio" class="btn-check" id="radShowDashboard" value="Show" name="radAccesRightDashboard" checked />
                                    <label class="btn btn-outline-secondary" for="radShowDashboard">Show</label>
                                    <input data-role="none" type="radio" class="btn-check" id="radHideDashboard" value="Hide" name="radAccesRightDashboard" />
                                    <label class="btn btn-outline-secondary" for="radHideDashboard">Hide</label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="divDashboardsAndReports" class="collapse show">
                        <div class="card-body">
                            <ul id="lstDashboardsAndReports" data-role="none" class="list-group mb-3"></ul>
                        </div>
                    </div>
                </div>
            </div>

            <div class="accordion mb-3 d-none colMCR">
                <div class="card">
                    <div class="card-header p-0">
                        <div class="row g-0 align-items-center">
                            <div class="col">
                                <button data-target="#divTroubleshooting" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                    Troubleshooting Tools
                                </button>
                            </div>
                            <div class="col-auto">
                                <div class="btn-group me-3 pr-3 pl-3 d-none" title="Show/Hide this menu according to the current access role" data-toggle="tooltip" accessrightmenu="Troubleshooting and Maintenance Tools">
                                    <input data-role="none" type="radio" class="btn-check" id="radShowTools" value="Show" name="radAccesRightTools" checked />
                                    <label class="btn btn-outline-secondary" for="radShowTools">Show</label>
                                    <input data-role="none" type="radio" class="btn-check" id="radHideTools" value="Hide" name="radAccesRightTools" />
                                    <label class="btn btn-outline-secondary" for="radHideTools">Hide</label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="divTroubleshooting" class="collapse show">
                        <div class="card-body">
                            <ul id="lstTroubleshooting" data-role="none" class="list-group mb-3"></ul>
                        </div>
                    </div>
                </div>
            </div>

        </div>

        <div id="tabSecurityRoleInfoEas" class="tab-pane fade pt-3">
            <div class="accordion mb-3 colCRM">
                <div class="card">
                    <div class="card-header p-0">
                        <button data-target="#divSettingsCRMDashboard" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                            CRM Dashboard
                        </button>
                    </div>
                    <div id="divSettingsCRMDashboard" class="collapse show">
                        <div class="card-body">
                            <ul id="lstSettingsCRMDashboard" data-role="none" class="list-group mb-3"></ul>
                        </div>
                    </div>
                </div>
            </div>

            <div class="accordion mb-3 colCRM">
                <div class="card">
                    <div class="card-header p-0">
                        <button data-target="#divSettingsCRMTab" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                            CRM Tab
                        </button>
                    </div>
                    <div id="divSettingsCRMTab" class="collapse show">
                        <div class="card-body">
                            <ul id="lstSettingsCRMTab" data-role="none" class="list-group mb-3"></ul>
                        </div>
                    </div>
                </div>
            </div>

            <div class="accordion mb-3 colCRM">
                <div class="card">
                    <div class="card-header p-0">
                        <button data-target="#divSettingsCRMSettingsTab" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                            CRM Settings Tab
                        </button>
                    </div>
                    <div id="divSettingsCRMSettingsTab" class="collapse show">
                        <div class="card-body">
                            <ul id="lstSettingsCRMSettingsTab" data-role="none" class="list-group mb-3"></ul>
                        </div>
                    </div>
                </div>
            </div>

            <div id="divEASModules"></div>

            <div class="accordion mb-3 colMCR">
                <div class="card">
                    <div class="card-header p-0">
                        <button data-target="#divErp" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                            Enterprise Application System (EAS)
                        </button>
                    </div>
                    <div id="divErp" class="collapse show">
                        <div class="card-body">
                            <ul id="lstErp" data-role="none" class="list-group mb-3"></ul>
                        </div>
                    </div>
                </div>
            </div>

            @*<div class="colMCR" data-role="collapsible" data-collapsed="false" data-theme="c" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
                    <h2>
                        <label>Daily Time Record (DTR)</label>
                    </h2>
                    <div id="divErp">
                        <ul id="lstDtr" data-role="none" class="list-group mb-3"></ul>
                    </div>
                </div>*@

        </div>
    </div>

    <div class="accordion mb-3 colMCR">
        <div class="card">
            <div class="card-header p-0">
                <button data-target="#divEasAccSystemAdministration" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                    System Administration
                </button>
            </div>
            <div id="divEasAccSystemAdministration" class="collapse show">
                <div class="card-body">
                    <ul id="EasAccSystemAdministration" data-role="none" class="list-group mb-3"></ul>
                </div>
            </div>
        </div>
    </div>

    @*<div class="colMCR" data-role="collapsible" data-collapsed="false" data-theme="c" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
            <h2>
                <label>Mobile Data Management</label>
            </h2>
            <div id="divMDM">
                <ul id="lstMDM" data-role="none" class="list-group mb-3">
                </ul>
            </div>
        </div>*@

    <div class="position-fixed text-center pb-3" style="display: block; left: 50%; transform: translate(-50%, 0); right: auto; bottom:0; z-index:1030;">
        <a id="btnSecurityRoleInfoCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
        <a id="btnSecurityRoleInfoDone" data-role="none" class="btn btn-success btn-lg">Save</a>
    </div>
</div>