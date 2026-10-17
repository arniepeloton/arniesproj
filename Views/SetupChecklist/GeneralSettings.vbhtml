<script>
    require(['bootstrapjs'], function () {
        require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
            require(['/Scripts/sources/SetupChecklist/GeneralSettings.js?v=' + assetversion])
        })
    })
</script>

<div class="row wrapper min-vh-100 flex-column flex-md-row">
    <div class="col-12 col-md-2 p-0 bg-light flex-shrink-1 mb-3">
        <nav class="navbar navbar-expand-md navbar-light bg-light align-items-start flex-md-column flex-row p-2">
            <a data-role="none" class="navbar-brand" href="#">Menu</a>
            <a data-role="none" href="#" class="navbar-toggler" data-toggle="collapse" data-target=".sidebarSystemAdminGenSettings">
                <span class="navbar-toggler-icon"></span>
            </a>
            <div class="collapse navbar-collapse sidebarSystemAdminGenSettings mt-2">
                <ul class="flex-column w-100 justify-content-between nav nav-pills">
                    <li><a class="nav-link active" role="button" data-role="none" data-toggle="tab" href="#tabGS2FA">2-Factor Authentication</a></li>
                    <li><a class="nav-link" role="button" data-role="none" data-toggle="tab" href="#tabGSExistingRecords">Search Existing Records</a></li>
                    <li><a class="nav-link" role="button" data-role="none" data-toggle="tab" href="#tabGSMCRAppModules">MCR App Modules</a></li>
                    <li><a class="nav-link" role="button" data-role="none" data-toggle="tab" href="#tabGSMoreSettings">More Settings</a></li>
                </ul>
            </div>
        </nav>
    </div>

    <div class="col col-md bg-faded">
        <div class="tab-content">
            <div class="tab-pane fade active show" id="tabGS2FA">
                <div class="alert alert-primary d-flex align-items-center mb-3" role="alert">
                    <span class="bi-info-circle-fill mr-3" style="font-size:20px;"></span>
                    <div>
                        Users must have at least one active email address when this feature is activated.
                    </div>
                </div>

                <div class="row mb-3 align-items-center">
                    <div class="col-md-auto">Enable email-based 2-factor authentication when logging in:</div>
                    <div class="col-md" style="font-size:20px;">
                        <div class="form-check form-switch">
                            <input data-role="none" class="form-check-input" id="chb2FA" type="checkbox">
                        </div>
                    </div>
                </div>

                <div class="mb-3 font-weight-bold">List of Active Employees</div>

                <div class="row mb-3 align-items-center">
                    <div class="col-md-auto">
                        Show list:
                    </div>
                    <div class="col-md-auto">
                        <select data-role="none" id="sel2FAFilter" class="form-select">
                            <option value="without">Without email address</option>
                            <option value="with">With email address</option>
                        </select>
                    </div>
                    <div class="col-md">
                        <input data-role="none" class="form-control" id="txtSearch2FA" placeholder="Search here" />
                    </div>
                </div>

                <div class="table-responsive mb-5">
                    <table data-role="none" id="tbl2FAEmployees" class="table table-light table-striped table-hover">
                        <thead>
                            <tr class="text-center align-items-center">
                                <th style="width:50px;">#</th>
                                <th>Employee Name</th>
                                <th>Email Address</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>
            </div>

            <div class="tab-pane fade" id="tabGSExistingRecords">
                <div class="mb-3">
                    <h5 class="mb-3">Settings below are required to search first for existing records before adding a new record. When the option is disabled, you can directly add a new record.</h5>

                    <div id="listSettings">
                        <div class="row mb-3 align-items-center">
                            <div class="col-md-2">FOWST</div>
                            <div class="col-md" style="font-size:20px;">
                                <div class="form-check form-switch">
                                    <input data-role="none" class="form-check-input" id="chbFOWST" type="checkbox">
                                </div>
                            </div>
                        </div>
                        <div class="row mb-3 align-items-center">
                            <div class="col-md-2">Authorized Devices</div>
                            <div class="col-md" style="font-size:20px;">
                                <div class="form-check form-switch">
                                    <input data-role="none" class="form-check-input" id="chbAuthorizedDevices" type="checkbox">
                                </div>
                            </div>
                        </div>
                        <div class="row mb-3 align-items-center">
                            <div class="col-md-2">Receipt Settings</div>
                            <div class="col-md" style="font-size:20px;">
                                <div class="form-check form-switch">
                                    <input data-role="none" class="form-check-input" id="chbReceiptSettings" type="checkbox">
                                </div>
                            </div>
                        </div>
                        <div class="row mb-3 align-items-center">
                            <div class="col-md-2">Security Roles</div>
                            <div class="col-md" style="font-size:20px;">
                                <div class="form-check form-switch">
                                    <input data-role="none" class="form-check-input" id="chbSecurityRoles" type="checkbox">
                                </div>
                            </div>
                        </div>
                        <div class="row mb-3 align-items-center">
                            <div class="col-md-2">Employees</div>
                            <div class="col-md" style="font-size:20px;">
                                <div class="form-check form-switch">
                                    <input data-role="none" class="form-check-input" id="chbEmployees" type="checkbox">
                                </div>
                            </div>
                        </div>
                        <div class="row mb-3 align-items-center">
                            <div class="col-md-2">Printer Roles</div>
                            <div class="col-md" style="font-size:20px;">
                                <div class="form-check form-switch">
                                    <input data-role="none" class="form-check-input" id="chbPrinterRoles" type="checkbox">
                                </div>
                            </div>
                        </div>
                        <div class="row mb-3 align-items-center">
                            <div class="col-md-2">Hot Button Settings</div>
                            <div class="col-md" style="font-size:20px;">
                                <div class="form-check form-switch">
                                    <input data-role="none" class="form-check-input" id="chbHotButtonSettings" type="checkbox">
                                </div>
                            </div>
                        </div>
                        <div class="row mb-3 align-items-center">
                            <div class="col-md-2">Group Settings</div>
                            <div class="col-md" style="font-size:20px;">
                                <div class="form-check form-switch">
                                    <input data-role="none" class="form-check-input" id="chbGroupSettings" type="checkbox">
                                </div>
                            </div>
                        </div>
                        <div class="row mb-3 align-items-center">
                            <div class="col-md-2">Customer / Client List</div>
                            <div class="col-md" style="font-size:20px;">
                                <div class="form-check form-switch">
                                    <input data-role="none" class="form-check-input" id="chbCustomerList" type="checkbox">
                                </div>
                            </div>
                        </div>
                        <div class="row mb-3 align-items-center">
                            <div class="col-md-2">Supplier / Vendor List</div>
                            <div class="col-md" style="font-size:20px;">
                                <div class="form-check form-switch">
                                    <input data-role="none" class="form-check-input" id="chbSupplierList" type="checkbox">
                                </div>
                            </div>
                        </div>
                        <div class="row mb-3 align-items-center">
                            <div class="col-md-2">Sales Team</div>
                            <div class="col-md" style="font-size:20px;">
                                <div class="form-check form-switch">
                                    <input data-role="none" class="form-check-input" id="chbSalesTeam" type="checkbox">
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="tab-pane fade" id="tabGSMoreSettings">
                <div class="row mb-3 gy-2">
                    <div class="col-md-3">Automatically approve franchise requests:</div>
                    <div class="col-md">
                        <div class="form-check form-switch">
                            <input data-role="none" type="checkbox" class="form-check-input" id="chbAutoApproveFR" />
                        </div>
                    </div>
                </div>

                <div class="row mb-3 gy-2">
                    <div class="col-md-3">Show announcements after logging in:</div>
                    <div class="col-md">
                        <div class="form-check form-switch">
                            <input data-role="none" type="checkbox" class="form-check-input" id="chbShowAnnouncement" />
                        </div>
                    </div>
                </div>

                <div class="row mb-3 gy-2 d-none">
                    <div class="col-md-3">Is it required for co-employees to enter their username/password to confirm that they received money from the crew/cashier?</div>
                    <div class="col-md">
                        <div class="form-check form-switch">
                            <input data-role="none" type="checkbox" class="form-check-input" id="chbRequiredCredentialsTurnoverMonies" />
                        </div>
                    </div>
                </div>

                <div class="row mb-3 gy-2 d-none">
                    <div class="col-md-3">Enable all features for VDM?</div>
                    <div class="col-md">
                        <div class="form-check form-switch">
                            <input data-role="none" type="checkbox" class="form-check-input" id="chbVDMFeatures" />
                        </div>
                    </div>
                </div>

                <div class="row mb-3 gy-2">
                    <div class="col-md-3">Download all product images after synchronizing the database:</div>
                    <div class="col-md">
                        <div class="form-check form-switch">
                            <input data-role="none" type="checkbox" class="form-check-input" id="chbDownloadSKUPics" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="tab-pane fade" id="tabGSMCRAppModules">
                <h5 class="mb-3">The following modules will be shown in MCR App:</h5>
                <div class="row mb-3 gy-2">
                    <div class="col-md-2">Point of Sale:</div>
                    <div class="col-md">
                        <div class="form-check form-switch">
                            <input data-role="none" type="checkbox" class="form-check-input" id="chbPOS" checked />
                        </div>
                    </div>
                </div>

                <div class="row mb-3 gy-2">
                    <div class="col-md-2">Price Checker:</div>
                    <div class="col-md">
                        <div class="form-check form-switch">
                            <input data-role="none" type="checkbox" class="form-check-input" id="chbPriceChecker" checked />
                        </div>
                    </div>
                </div>
                
                <div class="row mb-3 gy-2">
                    <div class="col-md-2">Purchase Order / Requisition:</div>
                    <div class="col-md">
                        <div class="form-check form-switch">
                            <input data-role="none" type="checkbox" class="form-check-input" id="chbPO" checked />
                        </div>
                    </div>
                </div>
                
                <div class="row mb-3 gy-2">
                    <div class="col-md-2">Sales Return:</div>
                    <div class="col-md">
                        <div class="form-check form-switch">
                            <input data-role="none" type="checkbox" class="form-check-input" id="chbSalesReturn" checked />
                        </div>
                    </div>
                </div>
                
                <div class="row mb-3 gy-2">
                    <div class="col-md-2">Stock Receiving:</div>
                    <div class="col-md">
                        <div class="form-check form-switch">
                            <input data-role="none" type="checkbox" class="form-check-input" id="chbStockReceiving" checked />
                        </div>
                    </div>
                </div>
                
                <div class="row mb-3 gy-2">
                    <div class="col-md-2">Stock Transfer:</div>
                    <div class="col-md">
                        <div class="form-check form-switch">
                            <input data-role="none" type="checkbox" class="form-check-input" id="chbStockTransfer" checked />
                        </div>
                    </div>
                </div>
                
                <div class="row mb-3 gy-2">
                    <div class="col-md-2">Inventory Count:</div>
                    <div class="col-md">
                        <div class="form-check form-switch">
                            <input data-role="none" type="checkbox" class="form-check-input" id="chbInventoryCount" checked />
                        </div>
                    </div>
                </div>
                
                <div class="row mb-3 gy-2">
                    <div class="col-md-2">Route and Expense Tracking:</div>
                    <div class="col-md">
                        <div class="form-check form-switch">
                            <input data-role="none" type="checkbox" class="form-check-input" id="chbRouteExpenseTracking" checked />
                        </div>
                    </div>
                </div>
                
                <div class="row mb-3 gy-2">
                    <div class="col-md-2">Product Maker:</div>
                    <div class="col-md">
                        <div class="form-check form-switch">
                            <input data-role="none" type="checkbox" class="form-check-input" id="chbProductMaker" checked />
                        </div>
                    </div>
                </div>
                
                <div class="row mb-3 gy-2">
                    <div class="col-md-2">Shelf and Bin Labelling:</div>
                    <div class="col-md">
                        <div class="form-check form-switch">
                            <input data-role="none" type="checkbox" class="form-check-input" id="chbShelfBinLabelling" checked />
                        </div>
                    </div>
                </div>
                
                <div class="row mb-3 gy-2">
                    <div class="col-md-2">Request and Approvals:</div>
                    <div class="col-md">
                        <div class="form-check form-switch">
                            <input data-role="none" type="checkbox" class="form-check-input" id="chbRequestApprovals" checked />
                        </div>
                    </div>
                </div>
                
                <div class="row mb-3 gy-2">
                    <div class="col-md-2">Cash Handling:</div>
                    <div class="col-md">
                        <div class="form-check form-switch">
                            <input data-role="none" type="checkbox" class="form-check-input" id="chbCashHandling" checked />
                        </div>
                    </div>
                </div>
                
                <div class="row mb-3 gy-2">
                    <div class="col-md-2">Dashboards and Reports:</div>
                    <div class="col-md">
                        <div class="form-check form-switch">
                            <input data-role="none" type="checkbox" class="form-check-input" id="chbDashboard" checked />
                        </div>
                    </div>
                </div>
                
                <div class="row mb-3 gy-2">
                    <div class="col-md-2">Reprint Online ZRead</div>
                    <div class="col-md">
                        <div class="form-check form-switch">
                            <input data-role="none" type="checkbox" class="form-check-input" id="chbReportZRead" checked />
                        </div>
                    </div>
                </div>
                
                <div class="row mb-3 gy-2">
                    <div class="col-md-2">Reconstruct Sales Data</div>
                    <div class="col-md">
                        <div class="form-check form-switch">
                            <input data-role="none" type="checkbox" class="form-check-input" id="chbReconstructSalesData" checked />
                        </div>
                    </div>
                </div>
                
                <div class="row mb-3 gy-2">
                    <div class="col-md-2">Lazy Update</div>
                    <div class="col-md">
                        <div class="form-check form-switch">
                            <input data-role="none" type="checkbox" class="form-check-input" id="chbLazyUpdate" checked />
                        </div>
                    </div>
                </div>
                
                <div class="row mb-3 gy-2">
                    <div class="col-md-2">Restore System</div>
                    <div class="col-md">
                        <div class="form-check form-switch">
                            <input data-role="none" type="checkbox" class="form-check-input" id="chbRestoreSystem" checked />
                        </div>
                    </div>
                </div>
                
                <div class="row mb-3 gy-2">
                    <div class="col-md-2">Customer Display</div>
                    <div class="col-md">
                        <div class="form-check form-switch">
                            <input data-role="none" type="checkbox" class="form-check-input" id="chbCustomerDisplay" checked />
                        </div>
                    </div>
                </div>
                
                <div class="row mb-3 gy-2">
                    <div class="col-md-2">Order Taker Setup</div>
                    <div class="col-md">
                        <div class="form-check form-switch">
                            <input data-role="none" type="checkbox" class="form-check-input" id="chbOrderTakerSetup" checked />
                        </div>
                    </div>
                </div>
                
                <div class="row mb-3 gy-2">
                    <div class="col-md-2">SMS Setup</div>
                    <div class="col-md">
                        <div class="form-check form-switch">
                            <input data-role="none" type="checkbox" class="form-check-input" id="chbSMSSetup" checked />
                        </div>
                    </div>
                </div>
                
                <div class="row mb-3 gy-2">
                    <div class="col-md-2">Adjust ZRead Date</div>
                    <div class="col-md">
                        <div class="form-check form-switch">
                            <input data-role="none" type="checkbox" class="form-check-input" id="chbAdjustZReadDate" checked />
                        </div>
                    </div>
                </div>
                
                <div class="row mb-3 gy-2">
                    <div class="col-md-2">Update Cache Database</div>
                    <div class="col-md">
                        <div class="form-check form-switch">
                            <input data-role="none" type="checkbox" class="form-check-input" id="chbUpdateCacheDB" checked />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="pop2FAEmailAdd" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">E-mail Address of <span id="lbl2FAEmpName"></span></h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div class="mb-3 text-right">
                    <button data-role="none" id="btnAdd2FAEmailAdd" class="btn btn-success">Add E-mail Address</button>
                </div>

                <div id="div2FAEmailAddList"></div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col">
                        <button data-role="none" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                    </div>
                    <div class="col">
                        <button data-role="none" id="btn2FAEmailAddSave" class="btn btn-success btn-block btn-lg">Save</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="fixed-bottom text-center pb-4">
    <button data-role="none" id="btnCancelGeneralSettings" class="btn btn-secondary btn-lg">Cancel</button>
    <button data-role="none" id="btnSaveGeneralSettings" class="btn btn-success btn-lg">Save</button>
</div>