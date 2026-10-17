<script>
    require([GetPathFormat('/Scripts/sources/Organization/OrganizationAddress.js?v=' + assetversion),
    GetPathFormat('/Scripts3/Browser/LibraryCountries.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryCities.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryAreaCodes.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryTelcoPrefixes.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryOrganization.js?crc=')
    ]);
</script>

<div id="pageOrgFOWSTList" style="display:none;">
    @*    <div data-role="header">
            <h1>Organization FOWST List</h1>
        </div>
    *@
    <div data-role="header" data-position="fixed1" style="background: none white; border: none">
        <h1>Organization Places (FOWST) List</h1>

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
                <div id="btnCompanyFOWST" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>FOWST</span>
                </div>
            </div>
        </div>

    </div>

    <div data-role="content">
        <div data-role="fieldcontain"><a id="btnOrgFOWSTAdd" data-role="button" data-icon="plus" data-inline="true">Add a Place</a></div>
        <div data-role="fieldcontain"><span>* A "Place" can be a Factory, Office, Warehouse, Store or Truck (F.O.W.S.T.)</span></div>
        <div data-role="fieldcontain" id="divOrgFOWSTRecordCount"></div>
        <div>
            <ul data-role="listview" id="lstOrgFOWST" data-inset="true">
            </ul>
        </div>
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgFOWSTDone" data-role="button"><h3>Go Back to Main</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgFOWSTGoToSetupChecklist" data-role="button"><h3>Go to Setup Checklist</h3></a>
            </div>
        </div>
    </div>

</div>

<div id="pageOrgFOWSTInfo" style="display:block;">
    <div data-role="fieldcontain" style="display:none;">
        <input type="text" data-role="none" class="form-control" readonly id="txtOrgFOWSTID" />
    </div>

    <div class="row wrapper min-vh-100 flex-column flex-md-row">
        <div class="col-12 col-md-2 p-0 bg-light flex-shrink-1 mb-3">
            <nav class="navbar navbar-expand-sm navbar-light bg-light align-items-start flex-md-column flex-row p-2">
                <a data-role="none" class="navbar-brand" href="#">Menu</a>
                <a data-role="none" href="#" class="navbar-toggler" data-toggle="collapse" data-target=".sidebarFowst">
                    <span class="navbar-toggler-icon"></span>
                </a>
                <div class="collapse navbar-collapse sidebarFowst mt-2">
                    <ul class="flex-column w-100 justify-content-between nav nav-pills">
                        <li><a class="nav-link active" data-role="none" data-toggle="tab" href="#tabFowstInfo">Basic Information</a></li>
                        <li><a class="nav-link" data-role="none" data-toggle="tab" href="#tabFowstAddress">Address</a></li>
                        <li class="divExcludedInMC" style="display:none;"><a class="nav-link" data-role="none" data-toggle="tab" href="#tabFowstSections">Sections</a></li>
                        <li class="divExcludedInMC" style="display:none;"><a class="nav-link" data-role="none" data-toggle="tab" href="#tabFowstBusinessPermit">Business Permit</a></li>
                        <li><a class="nav-link" data-role="none" data-toggle="tab" href="#tabFowstContacts">Contacts</a></li>
                        <li class="divExcludedInMC" style="display:none;"><a class="nav-link" data-role="none" data-toggle="tab" href="#tabFowstAssignedDevices">Assigned Devices</a></li>
                        <li class="divExcludedInMC" style="display:none;"><a class="nav-link" data-role="none" data-toggle="tab" href="#tabFowstAssignedEmployees">Assigned Employees</a></li>
                        <li class="divExcludedInMC" style="display:none;"><a class="nav-link" data-role="none" data-toggle="tab" href="#tabFowstOperationHours">Operation Hours</a></li>
                        <li class="divExcludedInMC d-none" style="display:none;"><a class="nav-link" data-role="none" data-toggle="tab" href="#tabFowstReportToPrincipal">Report to Principal</a></li>
                        <li class="divExcludedInMC d-none" style="display:none;"><a class="nav-link" data-role="none" data-toggle="tab" href="#tabFowstShopperTrak">ShopperTrak</a></li>
                        <li class="divExcludedInMC" style="display:none;"><a class="nav-link" data-role="none" data-toggle="tab" href="#tabFowstPrinters">Printers</a></li>
                        <li class="divExcludedInMC" style="display:none;"><a class="nav-link" data-role="none" data-toggle="tab" href="#tabFowstQuota">Quota</a></li>
                        <li class="divExcludedInMC" style="display:none;"><a class="nav-link" data-role="none" data-toggle="tab" href="#tabFowstProductMapping">Product Maping</a></li>
                        @*<li class="divExcludedInMC" style="display:none;"><a class="nav-link" data-role="none" data-toggle="tab" href="#tabFowstPriceLevels">Price Levels</a></li>*@
                    </ul>
                </div>
            </nav>
        </div>

        <div class="col bg-faded">
            <div class="tab-content p-3">
                <div class="tab-pane fade active show" id="tabFowstInfo">
                    <div>
                        <h1 class="display-6 mb-4" id="txtFowstTitle"></h1>

                        <div class="row mb-3">
                            <label class="col-md-3 mb-3 col-form-label" for="selOrgFOWSTType">FOWST Type:</label>
                            <div class="col-md">
                                <select id="selOrgFOWSTType" class="form-select" data-role="none">
                                    <option value="">Please Select</option>
                                </select>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 mb-3 col-form-label" for="selOrgFOWSTStatus">FOWST Status:</label>
                            <div class="col-md">
                                <select id="selOrgFOWSTStatus" class="form-select" data-role="none">
                                    <option value="Store and device are operational.">Store and device are operational.</option>
                                    <option value="Store is operational. No device is deployed yet.">Store is operational. No device is deployed yet.</option>
                                    <option value="Store is operational. Device is on hold.">Store is operational. Device is on hold.</option>
                                    <option value="Store is operational. Waiting for device to arrive.">Store is operational. Waiting for device to arrive.</option>
                                    <option value="Store is operational. No device available. Stolen.">Store is operational. No device available. Stolen.</option>
                                    <option value="Store is operational. No device available. Broken.">Store is operational. No device available. Broken.</option>
                                    <option value="Store is permanently closed.">Store is permanently closed.</option>
                                    <option value="Store is temporarily closed for holiday.">Store is temporarily closed for holiday.</option>
                                    <option value="Store is temporarily closed for company activity.">Store is temporarily closed for company activity.</option>
                                    <option value="Others">Others</option>
                                </select>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 mb-3 col-form-label" for="selOrgFOWSTClassification">FOWST Classification:</label>
                            <div class="col-md">
                                <select id="selOrgFOWSTClassification" class="form-select" data-role="none">
                                    <option value="Same Store">Same Store</option>
                                    <option value="Full Year Impact">Full Year Impact</option>
                                    <option value="New Store">New Store</option>
                                </select>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTCode">Internal Code: <span>("Internal Code" pertains to your company's short name or code name for this FOWST)</span></label>
                            <div class="col-md">
                                <input type="text" data-role="none" class="form-control" id="txtOrgFOWSTCode" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTName">FOWST Full Name : <span style="color: #FF0000;">*</span></label>
                            <div class="col-md">
                                <input type="text" data-role="none" class="form-control" id="txtOrgFOWSTName" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTDescription">Description:</label>
                            <div class="col-md">
                                <input type="text" data-role="none" class="form-control" id="txtOrgFOWSTDescription" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 mb-3 col-form-label" for="selOrgFOWSTTradeName">Business or Trade Name: <span style="color: #FF0000;">*</span></label>
                            <div class="col-md">
                                <select id="selOrgFOWSTTradeName" class="form-select" data-role="none">
                                    <option value="">Please Select</option>
                                </select>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 mb-3 col-form-label" for="selOrgFOWSTReceiptSettings">Receipt Settings: <span style="color: #FF0000;">*</span></label>
                            <div class="col-md">
                                <select id="selOrgFOWSTReceiptSettings" class="form-select" data-role="none">
                                    <option value="">Please Select</option>
                                </select>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 mb-3 col-form-label" for="txtTINBranchCode">TIN Branch Code <i>(Last 5 digits of the TIN to be reflected in the receipt)</i>: <span style="color: #FF0000;">*</span></label>
                            <div class="col-md">
                                <input type="number" data-role="none" class="form-control" id="txtTINBranchCode" placeholder="00000" min="5" maxlength="5" />
                            </div>
                        </div>

                        <div class="row mb-3 divExcludedInMC">
                            <label class="col-md-3 mb-3 col-form-label">
                                Choose VAT Type:
                            </label>
                            <div class="col-md">
                                <div class="form-check form-check-inline">
                                    <input type="radio" class="form-check-input" name="rdoFowstVATType" id="rdoFowstVAT" value="1" />
                                    <label for="rdoFowstVAT" class="form-check-label">VAT</label>
                                </div>
                                <div class="form-check form-check-inline">
                                    <input type="radio" class="form-check-input" name="rdoFowstVATType" id="rdoFowstNoneVAT" value="0" />
                                    <label for="rdoFowstNoneVAT" class="form-check-label">Non-VAT</label>
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3 divExcludedInMC">
                            <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTVATRate">VAT Rate (%) <i>(This is the VAT Rate applicable to location of this FOWST)</i>:</label>
                            <div class="col-md">
                                <input type="text" data-role="none" class="form-control" id="txtOrgFOWSTVATRate" readonly />
                            </div>
                        </div>

                        <div class="row mb-3 divExcludedInMC">
                            <label class="col-md-3 mb-3 col-form-label" for="selDefaultPosView">Default POS View Mode:</label>
                            <div class="col-md">
                                <select id="selDefaultPosView" class="form-select" data-role="none">
                                    <option value="">Please Select</option>
                                    <option value="Retail View">Retail View</option>
                                    <option class="d-none" value="Table View">Table View</option>
                                    <option value="Hot Buttons View">Hot Buttons View</option>
                                    <option value="Hot Buttons with side cart">Hot Buttons with side cart View</option>
                                    <option class="d-none" value="FMCG View (Modern Look)">FMCG View (Modern Look)</option>
                                    <option class="d-none" value="FMCG View (DOS Look)">FMCG View (DOS Look)</option>
                                    <option class="d-none" value="Vending Machine">Vending Machine</option>
                                </select>
                            </div>
                        </div>

                        <div class="row mb-3 divExcludedInMC">
                            <label class="col-md-3 mb-3 col-form-label" for="selDefaultHBTemplate">Default Hot Button Template:</label>
                            <div class="col-md">
                                <select id="selDefaultHBTemplate" class="form-select" data-role="none"></select>
                            </div>
                        </div>

                        <div class="row mb-3 divExcludedInMC">
                            <label class="col-md-3 mb-3 col-form-label" for="selOrgActiveFOWST">Is this FOWST active?</label>
                            <div class="col-md">
                                <div class="form-check form-switch mb-3" style="font-size:20px;">
                                    <input class="form-check-input" data-role="none" type="checkbox" id="selOrgActiveFOWST">
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3 divExcludedInMC">
                            <label class="col-md-3 mb-3 col-form-label" for="selOrgAddFranchisee">Is this a franchisee?</label>
                            <div class="col-md">
                                <div class="form-check form-switch mb-3" style="font-size:20px;">
                                    <input class="form-check-input" data-role="none" type="checkbox" id="selOrgAddFranchisee">
                                </div>
                            </div>
                        </div>

                        <div style="display:none;"><input type="text" readonly id="txtOrgFOWSTFranchisorID" /></div>
                        <div class="row mb-3" id="divFranchisorInfo" style="display:none">
                            <label class="col-md-3 mb-3 col-form-label" for="selOrgAddFranchisee">Franchisor: <span style="color: #FF0000;">*</span></label>
                            <div class="col-md">
                                <button id="btnOrgFOWSTFranchisor" data-role="none" class="btn btn-secondary btn-block mb-3">Please Select</button>
                                <div id="divFranchiseeConfirmed">
                                    <strong>Note:</strong> A request will be sent to the franchisor administrator for approval.
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3 divExcludedInMC">
                            <label class="col-md-3 mb-3 col-form-label" for="selOrgFOWSTOwnershipType">Ownership of Place:</label>
                            <div class="col-md">
                                <select id="selOrgFOWSTOwnershipType" data-role="none" class="form-select">
                                    <option value="0">Owned</option>
                                    <option value="1">Rented or Leased</option>
                                </select>
                            </div>
                        </div>

                        <div id="divRentedOrLeasedInfo" style="display:none;">
                            <div class="row mb-3">
                                <label class="col-md-3 mb-3 col-form-label" for="selOrgFOWSTOwnershipType">Contract Start Date:</label>
                                <div class="col-md">
                                    <div class="input-group">
                                        <select id="selOrgFOWSTContractStartMonth" data-role="none" class="form-select fixed">
                                            <option value="">MMM</option>
                                        </select>
                                        <select id="selOrgFOWSTContractStartDay" data-role="none" class="form-select fixed">
                                            <option value="">DD</option>
                                        </select>
                                        <select id="selOrgFOWSTContractStartYear" data-role="none" class="form-select fixed">
                                            <option value="">YYYY</option>
                                        </select>
                                    </div>
                                </div>
                            </div>

                            <div class="row mb-3">
                                <label class="col-md-3 mb-3 col-form-label" for="selOrgFOWSTOwnershipType">Contract End Date:</label>
                                <div class="col-md">
                                    <div class="input-group">
                                        <select id="selOrgFOWSTContractEndMonth" data-role="none" class="form-select fixed">
                                            <option value="">MMM</option>
                                        </select>
                                        <select id="selOrgFOWSTContractEndDay" data-role="none" class="form-select fixed">
                                            <option value="">DD</option>
                                        </select>
                                        <select id="selOrgFOWSTContractEndYear" data-role="none" class="form-select fixed">
                                            <option value="">YYYY</option>
                                        </select>
                                    </div>
                                </div>
                            </div>

                            <div class="row mb-3">
                                <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTCostPerMonth">Cost per Squared Meter per Month or Squared Foot per Month:</label>
                                <div class="col-md">
                                    <input type="number" id="txtOrgFOWSTCostPerMonth" data-role="none" class="form-control" />
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3 divExcludedInMC">
                            <label class="col-md-3 mb-3 col-form-label" for="selOrgFOWSTIsMortgaged">This Asset is Mortgaged?</label>
                            <div class="col-md">
                                <div class="form-check form-switch mb-3" style="font-size:20px;">
                                    <input class="form-check-input" data-role="none" type="checkbox" id="selOrgFOWSTIsMortgaged">
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3 divExcludedInMC">
                            <label class="col-md-3 mb-3 col-form-label" for="selEnforceZRead">Enforce Z-Read</label>
                            <div class="col-md">
                                <div class="form-check form-switch mb-3" style="font-size:20px;">
                                    <input class="form-check-input" data-role="none" type="checkbox" id="selEnforceZRead">
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3 divExcludedInMC">
                            <label class="col-md-3 mb-3 col-form-label" for="selOrgFOWSTInsideMall">Is this place inside a mall?</label>
                            <div class="col-md">
                                <div class="form-check form-switch mb-3" style="font-size:20px;">
                                    <input class="form-check-input" data-role="none" type="checkbox" id="selOrgFOWSTInsideMall">
                                </div>
                            </div>
                        </div>

                        <div id="divOrgFOWSTInsideMall" style="display:none;">
                            <div class="row mb-3">
                                <label class="col-md-3 mb-3 col-form-label" for="selOrgFOWSTMallName">Name of Mall:</label>
                                <div class="col-md">
                                    <select id="selOrgFOWSTMallName" data-role="none" class="form-select">
                                        <option value="">Please Select</option>
                                        <option value="Ayala Mall">Ayala Malls (Ayala Land Corporation)</option>
                                        <option value="SM Mall">SM Malls (SM Prime Holdings, Inc.)</option>
                                        <option value="Gaisano Mall">Gaisano Mall</option>
                                        <option value="Megaworld">Megaworld Malls (Megaworld Corporation)</option>
                                        <option value="Robinson">Robinsons Malls (Robinsons Land Corporation)</option>
                                        <option value="PITX">PITX</option>
                                    </select>
                                </div>
                            </div>

                            <div id="divOrgFOWSTAyalaInfo" style="display:none;">
                                <div style="display:none;"><input type="text" readonly data-role="none" class="form-control" id="txtOrgFOWSTAyalaMallRecID" /></div>
                                <div class="row mb-3">
                                    <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTAyalaMallTenantCode">Tenant Code: <span style="color: #FF0000;">*</span></label>
                                    <div class="col-md">
                                        <input type="text" id="txtOrgFOWSTAyalaMallTenantCode" data-role="none" class="form-control" maxlength="10" />
                                    </div>
                                </div>

                                <div class="row mb-3">
                                    <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTAyalaMallTenantName">Tenant Name(.TXT File): <span style="color: #FF0000;">*</span></label>
                                    <div class="col-md">
                                        <input type="text" id="txtOrgFOWSTAyalaMallTenantName" data-role="none" class="form-control" maxlength="20" />
                                    </div>
                                </div>

                                <div class="row mb-3">
                                    <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTAyalaMallTenantNameForDBF">Tenant Name(.DBF File): <span style="color: #FF0000;">*</span></label>
                                    <div class="col-md">
                                        <input type="text" id="txtOrgFOWSTAyalaMallTenantNameForDBF" data-role="none" class="form-control" maxlength="20" />
                                    </div>
                                </div>

                                <div class="row mb-3">
                                    <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTAyalaMallMerchantsLeaseOutNumber">Merchants Lease-Out Number: <span style="color: #FF0000;">*</span></label>
                                    <div class="col-md">
                                        <input type="text" id="txtOrgFOWSTAyalaMallMerchantsLeaseOutNumber" data-role="none" class="form-control" maxlength="20" />
                                    </div>
                                </div>
                            </div>

                            <div id="divOrgFOWSTSMInfo" style="display:none;">
                                <div style="display:none;"><input type="text" readonly data-role="none" class="form-control" id="txtOrgFOWSTSMMallRecID" /></div>
                                <div class="row mb-3">
                                    <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTSMMallCode">Mall or Branch Code: <span style="color: #FF0000;">*</span></label>
                                    <div class="col-md">
                                        <input type="text" id="txtOrgFOWSTSMMallCode" data-role="none" class="form-control" maxlength="10" />
                                    </div>
                                </div>

                                <div class="row mb-3">
                                    <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTSMMallTenantNo">Tenant No.: <span style="color: #FF0000;">*</span></label>
                                    <div class="col-md">
                                        <input type="text" id="txtOrgFOWSTSMMallTenantNo" data-role="none" class="form-control" maxlength="10" />
                                    </div>
                                </div>

                                <div class="row mb-3">
                                    <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTSMMallClassCode">Class Code: <span style="color: #FF0000;">*</span></label>
                                    <div class="col-md">
                                        <input type="number" id="txtOrgFOWSTSMMallClassCode" data-role="none" class="form-control" />
                                    </div>
                                </div>

                                <div class="row mb-3">
                                    <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTSMMallTradeCode">Trade Code: <span style="color: #FF0000;">*</span></label>
                                    <div class="col-md">
                                        <input type="text" id="txtOrgFOWSTSMMallTradeCode" data-role="none" class="form-control" maxlength="10" />
                                    </div>
                                </div>

                                <div class="row mb-3">
                                    <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTSMMallOutletNo">Outlet No.: <span style="color: #FF0000;">*</span></label>
                                    <div class="col-md">
                                        <input type="number" id="txtOrgFOWSTSMMallOutletNo" data-role="none" class="form-control" />
                                    </div>
                                </div>

                                <div class="row mb-3">
                                    <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTSMMallSalesType">Sales Type: <span style="color: #FF0000;">*</span></label>
                                    <div class="col-md">
                                        <input type="text" id="txtOrgFOWSTSMMallSalesType" data-role="none" class="form-control" maxlength="6" />
                                    </div>
                                </div>
                            </div>

                            <div id="divOrgFOWSTMegaworldInfo" style="display:none;">
                                <div style="display:none;"><input type="text" readonly data-role="none" class="form-control" id="txtOrgFOWSTMegaworldRecID" /></div>

                                <div class="row mb-3">
                                    <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTMegaworldTenantNo">Tenant No.: <span style="color: #FF0000;">*</span></label>
                                    <div class="col-md">
                                        <input type="text" id="txtOrgFOWSTMegaworldTenantNo" data-role="none" class="form-control" maxlength="10" />
                                    </div>
                                </div>

                                <div class="row mb-3">
                                    <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTMegaworldSalesType">Sales Type: <span style="color: #FF0000;">*</span></label>
                                    <div class="col-md">
                                        <input type="text" id="txtOrgFOWSTMegaworldSalesType" data-role="none" class="form-control" maxlength="6" />
                                    </div>
                                </div>

                                <div class="row mb-3">
                                    <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTMegaworldSalesType">Number of POS terminals in this store: <span style="color: #FF0000;">*</span></label>
                                    <div class="col-md">
                                        <input type="text" id="txtOrgFOWSTMegaworldNoOfPOSTerminal" data-role="none" class="form-control" maxlength="6" />
                                    </div>
                                </div>
                            </div>

                            <div id="divOrgFOWSTRobinsonInfo" style="display:none;">
                                <div class="row mb-3">
                                    <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTRobinsonTenantId">Tenant ID.: <span style="color: #FF0000;">*</span></label>
                                    <div class="col-md">
                                        <input type="text" id="txtOrgFOWSTRobinsonTenantId" data-role="none" class="form-control" maxlength="50" />
                                    </div>
                                </div>

                                <div class="row mb-3">
                                    <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTRobinsonPosTerminalNum">POS Terminal No.: <span style="color: #FF0000;">*</span></label>
                                    <div class="col-md">
                                        <input type="text" id="txtOrgFOWSTRobinsonPosTerminalNum" data-role="none" class="form-control" maxlength="50" />
                                    </div>
                                </div>

                                <div class="row mb-3 divExcludedInMC">
                                    <label class="col-md-3 mb-3 col-form-label" for="chkOrgFOWSTOperates247">Operates 24/7</label>
                                    <div class="col-md">
                                        <div class="form-check form-switch mb-3" style="font-size:20px;">
                                            <input class="form-check-input" data-role="none" type="checkbox" id="chkOrgFOWSTOperates247">
                                        </div>
                                    </div>
                                </div>

                                <div class="row mb-3">
                                    <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTFtpServer">FTP Server:</label>
                                    <div class="col-md">
                                        <input type="text" id="txtOrgFOWSTFtpServer" data-role="none" class="form-control" maxlength="50" />
                                    </div>
                                </div>

                                <div class="row mb-3">
                                    <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTFtpUsername">FTP Username:</label>
                                    <div class="col-md">
                                        <input type="text" id="txtOrgFOWSTFtpUsername" data-role="none" class="form-control" maxlength="50" />
                                    </div>
                                </div>

                                <div class="row mb-3">
                                    <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTFtpPassword">FTP Password:</label>
                                    <div class="col-md">
                                        <input type="password" id="txtOrgFOWSTFtpPassword" data-role="none" class="form-control" maxlength="50" />
                                    </div>
                                </div>
                            </div>

                            <div id="divOrgFOWSTPITXInfo" style="display:none;">
                                <div class="row mb-3">
                                    <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTPITXTenantId">Tenant ID: <span style="color: #FF0000;">*</span></label>
                                    <div class="col-md">
                                        <input type="text" id="txtOrgFOWSTPITXTenantId" data-role="none" class="form-control" maxlength="50" />
                                    </div>
                                </div>

                                <div class="row mb-3">
                                    <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTPITXPosTerminalID">Terminal ID: <span style="color: #FF0000;">*</span></label>
                                    <div class="col-md">
                                        <input type="text" id="txtOrgFOWSTPITXPosTerminalID" data-role="none" class="form-control" maxlength="50" />
                                    </div>
                                </div>

                                @*<div class="row mb-3 divExcludedInMC">
                                    <label class="col-md-3 mb-3 col-form-label" for="chkOrgFOWSTOperates247">Operates 24/7</label>
                                    <div class="col-md">
                                        <div class="form-check form-switch mb-3" style="font-size:20px;">
                                            <input class="form-check-input" data-role="none" type="checkbox" id="chkOrgFOWSTOperates247">
                                        </div>
                                    </div>
                                </div>

                                <div class="row mb-3">
                                    <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTFtpServer">FTP Server:</label>
                                    <div class="col-md">
                                        <input type="text" id="txtOrgFOWSTFtpServer" data-role="none" class="form-control" maxlength="50" />
                                    </div>
                                </div>

                                <div class="row mb-3">
                                    <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTFtpUsername">FTP Username:</label>
                                    <div class="col-md">
                                        <input type="text" id="txtOrgFOWSTFtpUsername" data-role="none" class="form-control" maxlength="50" />
                                    </div>
                                </div>

                                <div class="row mb-3">
                                    <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTFtpPassword">FTP Password:</label>
                                    <div class="col-md">
                                        <input type="password" id="txtOrgFOWSTFtpPassword" data-role="none" class="form-control" maxlength="50" />
                                    </div>
                                </div>*@
                            </div>
                        </div>

                        <div id="divOrgFOWSTWarehouseTypeInfo" style="display:none;">
                            <div class="row mb-3">
                                <label class="col-md-3 mb-3 col-form-label" for="selOrgFOWSTWarehouseType">Warehouse Type:</label>
                                <div class="col-md">
                                    <select id="selOrgFOWSTWarehouseType" data-role="none" class="form-select">
                                        <option value="">Please Select</option>
                                    </select>
                                </div>
                            </div>
                        </div>

                        <div id="divOrgFOWSTRetailStoreTypeInfo" class="divExcludedInMC" style="display:none;">
                            <div class="row mb-3">
                                <label class="col-md-3 mb-3 col-form-label" for="selOrgFOWSTRetailStoreType">Type of Retail Store:</label>
                                <div class="col-md">
                                    <select id="selOrgFOWSTRetailStoreType" data-role="none" class="form-select">
                                        <option value="">Please Select</option>
                                    </select>
                                </div>
                            </div>
                        </div>

                        <div id="divOrgFOWSTRetailStoreLocationTypeInfo" class="divExcludedInMC" style="display:none;">
                            <div class="row mb-3">
                                <label class="col-md-3 mb-3 col-form-label" for="selOrgFOWSTRetailStoreLocationType">Location Type of Retail Store:</label>
                                <div class="col-md">
                                    <select id="selOrgFOWSTRetailStoreLocationType" data-role="none" class="form-select">
                                        <option value="">Please Select</option>
                                    </select>
                                </div>
                            </div>
                        </div>

                        <div id="divOrgFOWSTDeliveryEquipmentInfo" style="display:none;">
                            <div class="row mb-3">
                                <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTPlateNo">Plate No.:</label>
                                <div class="col-md">
                                    <input type="text" id="txtOrgFOWSTPlateNo" data-role="none" class="form-control" />
                                </div>
                            </div>

                            <div class="row mb-3">
                                <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTNoOfWheels">No. of Wheels:</label>
                                <div class="col-md">
                                    <input type="text" id="txtOrgFOWSTNoOfWheels" data-role="none" class="form-control" />
                                </div>
                            </div>

                            <div class="row mb-3">
                                <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTBodyColor">Body Color:</label>
                                <div class="col-md">
                                    <input type="text" id="txtOrgFOWSTBodyColor" data-role="none" class="form-control" />
                                </div>
                            </div>

                            <div class="row mb-3">
                                <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTLoadLimitWeight">Load Limit (in weight):</label>
                                <div class="col-md">
                                    <input type="text" id="txtOrgFOWSTLoadLimitWeight" data-role="none" class="form-control" />
                                </div>
                            </div>

                            <div class="row mb-3">
                                <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTLoadLimitDimensions">Load Limit (in Dimensions):</label>
                                <div class="col-md">
                                    <input type="text" id="txtOrgFOWSTLoadLimitDimensions" data-role="none" class="form-control" />
                                </div>
                            </div>

                            <div>
                                <div class="row mb-3 align-items-center">
                                    <div class="col-6 mb-3">
                                        <h5>LTO Registration Details</h5>
                                    </div>
                                    <div class="col-6 text-right">
                                        <button id="btnAddOrgFOWSTLTO" data-role="none" class="btn btn-success">Add LTO Registration</button>
                                    </div>
                                </div>

                                <div data-role="none" id="lstOrgFOWSTLTO" class="list-group mb-3"></div>
                            </div>

                            <div>
                                <div class="row mb-3 align-items-center">
                                    <div class="col-6 mb-3">
                                        <h5>LTFRB Registration Details</h5>
                                    </div>
                                    <div class="col-6 text-right">
                                        <button id="btnAddOrgFOWSTLTFRB" data-role="none" class="btn btn-success">Add LTFRB Registration</button>
                                    </div>
                                </div>

                                <div data-role="none" id="lstOrgFOWSTLTFRB" class="list-group mb-3"></div>
                            </div>
                        </div>

                        <div class="row mb-3 divExcludedInMC">
                            <label class="col-md-3 mb-3 col-form-label" for="selEnableHotButtonsContextualMenu">Enable hot buttons contextual menu:</label>
                            <div class="col-md">
                                <div class="form-check form-switch mb-3" style="font-size:20px;">
                                    <input class="form-check-input" data-role="none" type="checkbox" id="selEnableHotButtonsContextualMenu">
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 mb-3 col-form-label" for="selEnablePrintStickers">Print Stickers:</label>
                            <div class="col-md">
                                <div class="form-check form-switch mb-3" style="font-size:20px;">
                                    <input class="form-check-input" data-role="none" type="checkbox" id="selEnablePrintStickers">
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3 divExcludedInMC">
                            <label class="col-md-3 mb-3 col-form-label" for="selPrintQueueNumbers">Print queue numbers:</label>
                            <div class="col-md">
                                <div class="form-check form-switch mb-3" style="font-size:20px;">
                                    <input class="form-check-input" data-role="none" type="checkbox" id="selPrintQueueNumbers">
                                </div>
                            </div>
                        </div>
                        <div class="row mb-3">
                            <label class="col-md-3 mb-3 col-form-label">
                                POS Vendor:
                            </label>
                            <div class="col-md">
                                <div class="form-check form-check-inline">
                                    <input type="radio" class="form-check-input" name="radPosVendor" id="radPosVendor1" value="" />
                                    <label for="radPosVendor1" class="form-check-label">Mobiz</label>
                                </div>
                                <div class="form-check form-check-inline">
                                    <input type="radio" class="form-check-input" name="radPosVendor" id="radPosVendor2" value="1" />
                                    <label for="radPosVendor2" class="form-check-label">3rd Party</label>
                                </div>
                            </div>
                        </div>
                        <div id="div3rdPartyPosVendor" class="row mb-3">
                            <label class="col-md-3 mb-3 col-form-label">
                            </label>
                            <div class="col-md">
                                <button id="butPosVendor" data-role="none" class="btn btn-secondary btn-block">Please Select a 3rd POS Vendor</button>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="tab-pane fade" id="tabFowstAddress">
                    <div>
                        <h1 class="display-6 mb-4">Complete Address</h1>
                        <div style="display:none;">
                            <input type="text" readonly id="txtOrgFOWSTCityID" data-role="none" class="form-control" />
                            <input type="text" readonly id="txtOrgFOWSTCountryID" data-role="none" class="form-control" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTBuildingNo">House or Building No.:</label>
                            <div class="col-md">
                                <input type="text" data-role="none" class="form-control" id="txtOrgFOWSTBuildingNo" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTStreet">Street: <span style="color: #FF0000;">*</span></label>
                            <div class="col-md">
                                <input type="text" data-role="none" class="form-control" id="txtOrgFOWSTStreet" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTStreet">City or Town: <span style="color: #FF0000;">*</span></label>
                            <div class="col-md">
                                <button id="btnOrgFOWSTCity" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 mb-3 col-form-label" for="selOrgFOWSTRegion">Region: <span style="color: #FF0000;">*</span></label>
                            <div class="col-md">
                                <select id="selOrgFOWSTRegion" data-role="none" class="form-select">
                                    <option value="">Please Select</option>
                                    <optgroup label="Region I - Ilocos Region">
                                        <option value="Region I- Ilocos Norte">Region I- Ilocos Norte</option>
                                        <option value="Region I- Ilocos Sur">Region I- Ilocos Sur</option>
                                        <option value="Region I- La Union">Region I- La Union</option>
                                        <option value="Region I- Pangasinan">Region I- Pangasinan</option>
                                    </optgroup>
                                    <optgroup label="Region II - Cagayan Valley">
                                        <option value="Region II- Batanes">Region II- Batanes</option>
                                        <option value="Region II- Cagayan">Region II- Cagayan</option>
                                        <option value="Region II- Isabela">Region II- Isabela</option>
                                        <option value="Region II- Nueva Viscaya">Region II- Nueva Viscaya</option>
                                        <option value="Region II- Quirino">Region II- Quirino</option>
                                    </optgroup>
                                    <optgroup label="Region III - Central Luzon">
                                        <option value="Region III- Aurora">Region III- Aurora</option>
                                        <option value="Region III- Bataan">Region III- Bataan</option>
                                        <option value="Region III- Bulacan">Region III- Bulacan</option>
                                        <option value="Region III- Nueva Ecija">Region III- Nueva Ecija</option>
                                        <option value="Region III- Pampanga">Region III- Pampanga</option>
                                        <option value="Region III- Tarlac">Region III- Tarlac</option>
                                        <option value="Region III- Zambales">Region III- Zambales</option>
                                    </optgroup>
                                    <optgroup label="Region IV(A) - CALABARZON">
                                        <option value="Region IV(A)- Batangas">Region IV(A)- Batangas</option>
                                        <option value="Region IV(A)- Cavite">Region IV(A)- Cavite</option>
                                        <option value="Region IV(A)- Laguna">Region IV(A)- Laguna</option>
                                        <option value="Region IV(A)- Quezon">Region IV(A)- Quezon</option>
                                        <option value="Region IV(A)- Rizal">Region IV(A)- Rizal</option>
                                    </optgroup>
                                    <optgroup label="Region IV(B) - MIMAROPA">
                                        <option value="Region IV(B)- Occidental Mindoro">Region IV(B)- Occidental Mindoro</option>
                                        <option value="Region IV(B)- Oriental Mindoro">Region IV(B)- Oriental Mindoro</option>
                                        <option value="Region IV(B)- Palawan">Region IV(B)- Palawan</option>
                                        <option value="Region IV(B)- Romblon">Region IV(B)- Romblon</option>
                                        <option value="Region IV(B)- Marinduque">Region IV(B)- Marinduque</option>
                                    </optgroup>
                                    <optgroup label="Region V - Bicol Region">
                                        <option value="Region V- Albay">Region V- Albay</option>
                                        <option value="Region V- Catanduanes">Region V- Catanduanes</option>
                                        <option value="Region V- Camarines Norte">Region V- Camarines Norte</option>
                                        <option value="Region V- Camarines Sur">Region V- Camarines Sur</option>
                                        <option value="Region V- Masbate">Region V- Masbate</option>
                                        <option value="Region V- Sorsogon">Region V- Sorsogon</option>
                                    </optgroup>
                                    <optgroup label="Region VI - Western Visayas">
                                        <option value="Region VI- Aklan">Region VI- Aklan</option>
                                        <option value="Region VI- Antique">Region VI- Antique</option>
                                        <option value="Region VI- Capiz">Region VI- Capiz</option>
                                        <option value="Region VI- Guimeras">Region VI- Guimeras</option>
                                        <option value="Region VI- Ilo-ilo">Region VI- Ilo-ilo</option>
                                        <option value="Region VI- Negros Occidental">Region VI- Negros Occidental</option>
                                    </optgroup>
                                    <optgroup label="Region VII - Central Visayas">
                                        <option value="Region VII- Bohol">Region VII- Bohol</option>
                                        <option value="Region VII- Cebu">Region VII- Cebu</option>
                                        <option value="Region VII- Negros Oriental">Region VII- Negros Oriental</option>
                                        <option value="Region VII- Siquijor">Region VII- Siquijor</option>
                                    </optgroup>
                                    <optgroup label="Region VIII - Eastern Visayas">
                                        <option value="Region VIII- Biliran">Region VIII- Biliran</option>
                                        <option value="Region VIII- Eastern Samar">Region VIII- Eastern Samar</option>
                                        <option value="Region VIII- Leyte">Region VIII- Leyte</option>
                                        <option value="Region VIII- Northern Samar">Region VIII- Northern Samar</option>
                                        <option value="Region VIII- Samar">Region VIII- Samar</option>
                                        <option value="Region VIII- Southern Leytre">Region VIII- Southern Leyte</option>
                                    </optgroup>
                                    <optgroup label="Region IX - Zamboanga Peninsula">
                                        <option value="Region IX- Zamboanga del Norte">Region IX- Zamboanga del Norte</option>
                                        <option value="Region IX- Zamboanga del Sur">Region IX- Zamboanga del Sur</option>
                                        <option value="Region IX- Zamboanga Sibugay">Region IX- Zamboanga Sibugay</option>
                                    </optgroup>
                                    <optgroup label="Region X - Northern Mindanao">
                                        <option value="Region X- Bukidnon">Region X- Bukidnon</option>
                                        <option value="Region X- Camiguin">Region X- Camiguin</option>
                                        <option value="Region X- Lanao del Norte">Region X- Lanao del Norte</option>
                                        <option value="Region X- Misamis Occidental">Region X- Misamis Occidental</option>
                                        <option value="Region X- Misamis Oriental">Region X- Misamis Oriental</option>
                                    </optgroup>
                                    <optgroup label="Region XI - Davao Region">
                                        <option value="Region XI- Compostella Valley">Region XI- Compostella Valley</option>
                                        <option value="Region XI- Davao del Norte">Region XI- Davao del Norte</option>
                                        <option value="Region XI- Davao del Sur">Region XI- Davao del Sur</option>
                                        <option value="Region XI- Davao Oriental">Region XI- Davao Oriental</option>
                                    </optgroup>
                                    <optgroup label="Region XII - SOCCSKSARGEN">
                                        <option value="Region XII- Cotabato">Region XII- Cotabato</option>
                                        <option value="Region XII- Sarangani">Region XII- Sarangani</option>
                                        <option value="Region XII- South Cotabato">Region XII- South Cotabato</option>
                                        <option value="Region XII- Sultan Kudarat">Region XII- Sultan Kudarat</option>
                                    </optgroup>
                                    <optgroup label="Region XIII - CARAGA">
                                        <option value="Region XIII- Agusan del Norte">Region XIII- Agusan del Norte</option>
                                        <option value="Region XIII- Agusan del Sur">Region XIII- Agusan del Sur</option>
                                        <option value="Region XIII- Surigao del Norte">Region XIII- Surigao del Norte</option>
                                        <option value="Region XIII- Surigao del Sur">Region XIII- Surigao del Sur</option>
                                    </optgroup>
                                    <optgroup label="ARMM">
                                        <option value="ARMM- Basilan">ARMM- Basilan</option>
                                        <option value="ARMM- Lanao del Sur">ARMM- Lanao del Sur</option>
                                        <option value="ARMM- Maguindanao">ARMM- Maguindanao</option>
                                        <option value="ARMM- Sulu">ARMM- Sulu</option>
                                        <option value="ARMM- Tawi-tawi">ARMM- Tawi-tawi</option>
                                    </optgroup>
                                    <optgroup label="CAR">
                                        <option value="CAR- Abra">CAR- Abra</option>
                                        <option value="CAR- Apayao">CAR- Apayao</option>
                                        <option value="CAR- Benguet">CAR- Benguet</option>
                                        <option value="CAR- Ifugao">CAR- Ifugao</option>
                                        <option value="CAR- Kalinga">CAR- Kalinga</option>
                                        <option value="CAR- Mountain Province">CAR- Mountain Province</option>
                                    </optgroup>
                                    <optgroup label="NCR">
                                        <option value="NCR- Caloocan">NCR- Caloocan</option>
                                        <option value="NCR- Las Piñas">NCR- Las Piñas</option>
                                        <option value="NCR- Makati">NCR- Makati</option>
                                        <option value="NCR- Malabon">NCR- Malabon</option>
                                        <option value="NCR- Mandaluyong">NCR- Mandaluyong</option>
                                        <option value="NCR- Manila">NCR- Manila</option>
                                        <option value="NCR- Marikina">NCR- Marikina"</option>
                                        <option value="NCR- Muntinlupa">NCR- Muntinlupa</option>
                                        <option value="NCR- Navotas">NCR- Navotas</option>
                                        <option value="NCR- Parañaque">NCR- Parañaque</option>
                                        <option value="NCR- Pasay">NCR- Pasay</option>
                                        <option value="NCR- Pasig">NCR- Pasig</option>
                                        <option value="NCR- Pateros">NCR- Pateros</option>
                                        <option value="NCR- Quezon City">NCR- Quezon City</option>
                                        <option value="NCR- San Juan">NCR- San Juan</option>
                                        <option value="NCR- Taguig">NCR- Taguig</option>
                                        <option value="NCR- Valenzuela">NCR- Valenzuela</option>
                                    </optgroup>
                                </select>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 mb-3 col-form-label" for="btnOrgFOWSTCountry">Country: <span style="color: #FF0000;">*</span></label>
                            <div class="col-md">
                                <button id="btnOrgFOWSTCountry" data-role="none" class="btn btn-secondary btn-block">Philippines</button>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="tab-pane fade" id="tabFowstSections">
                    <div id="divOrgFOWSTSection" class="divExcludedInMC" style="display:none;">
                        <div class="row mb-3 align-items-center">
                            <div class="col-6 mb-3">
                                <h1 class="display-6">Sections</h1>
                            </div>
                            <div class="col-6 text-right">
                                <button id="btnAddOrgFOWSTSections" data-role="none" class="btn btn-success">Add Section</button>
                            </div>
                        </div>

                        <div data-role="none" id="lstOrgFOWSTSection" class="list-group mb-3">
                            <a data-role="none" href="#" class="list-group-item list-group-item-action">No result(s) found</a>
                        </div>
                    </div>
                </div>

                <div class="tab-pane fade" id="tabFowstBusinessPermit">
                    <div id="divOrgFOWSTBusinessPermit" class="divExcludedInMC">
                        <div class="row mb-3 align-items-center">
                            <div class="col-6 mb-3">
                                <h1 class="display-6">Business Permit</h1>
                            </div>
                            <div class="col-6 text-right">
                                <button id="btnAddOrgFOWSTBusinessPermit" data-role="none" class="btn btn-success">Add Business Permit</button>
                            </div>
                        </div>

                        <div data-role="none" id="lstOrgFOWSTBusinessPermit" class="list-group mb-3"></div>
                    </div>
                </div>

                <div class="tab-pane fade" id="tabFowstContacts">
                    <div id="tabFowstContacts" style="white-space:nowrap; overflow:auto;">
                        <ul class="nav nav-tabs" role="tablist" style="flex-wrap:nowrap;">
                            <li class="nav-item" role="presentation">
                                <button data-role="none" class="nav-link active" data-toggle="tab" data-target="#tabFowstTelephone">Telephone</button>
                            </li>
                            <li class="nav-item" role="presentation">
                                <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabFowstMobile">Mobile</button>
                            </li>
                            <li class="nav-item" role="presentation">
                                <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabFowstEmail">E-mail</button>
                            </li>
                        </ul>
                    </div>

                    <div class="tab-content">
                        <div class="tab-pane fade p-3 show active" id="tabFowstTelephone">
                            <div id="divOrgFOWSTTelephone">
                                <div class="mb-3 text-right">
                                    <button id="btnAddOrgFOWSTTelephone" data-role="none" class="btn btn-success">Add Telephone</button>
                                </div>

                                <div data-role="none" id="lstOrgFOWSTTelephone" class="list-group mb-3"></div>
                            </div>
                        </div>

                        <div class="tab-pane fade p-3" id="tabFowstMobile">
                            <div id="divOrgFOWSTMobile">
                                <div class="mb-3 text-right">
                                    <button id="btnAddOrgFOWSTMobile" data-role="none" class="btn btn-success">Add Mobile</button>
                                </div>

                                <div data-role="none" id="lstOrgFOWSTMobile" class="list-group mb-3"></div>
                            </div>
                        </div>

                        <div class="tab-pane fade p-3" id="tabFowstEmail">
                            <div id="divOrgFOWSTEmail">
                                <div class="mb-3 text-right">
                                    <button id="btnAddOrgFOWSTEmail" data-role="none" class="btn btn-success">Add E-mail</button>
                                </div>

                                <div data-role="none" id="lstOrgFOWSTEmail" class="list-group mb-3"></div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="tab-pane fade" id="tabFowstOperationHours">
                    <div id="divOrgFOWSTOperationHR" class="divExcludedInMC">
                        <h1 class="display-6 mb-4">Operation Hours</h1>

                        <div data-role="none" id="lstOrgFOWSTOperationHR" class="list-group"></div>

                        <table data-role="none" class="table table-striped table-hover">
                            <thead>
                                <tr class="custom-theme-f txt-center">
                                    <th>Day</th>
                                    <th>Opening Time</th>
                                    <th>Closing Time</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody id="tbodyFowstOperationHours"></tbody>
                        </table>
                    </div>
                </div>

                <div class="tab-pane fade" id="tabFowstReportToPrincipal">
                    <div id="divOrgFOWSTReportsToPrincipal" class="divExcludedInMC">
                        <h1 class="display-6 mb-4">Reports To Principal</h1>

                        <div class="row mb-3">
                            <label class="col-md-3 mb-3 col-form-label" for="selOrgFOWSTIsInclude">Include FOWST transactions in principal report?</label>
                            <div class="col-md">
                                <div class="form-check form-switch mb-3" style="font-size:20px;">
                                    <input class="form-check-input" data-role="none" type="checkbox" id="selOrgFOWSTIsInclude">
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 mb-3 col-form-label" for="txtAccountID">Account ID 1:</label>
                            <div class="col-md">
                                <input type="text" id="txtAccountID" data-role="none" class="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 mb-3 col-form-label" for="txtStoreID">Store ID 1:</label>
                            <div class="col-md">
                                <input type="text" id="txtStoreID" data-role="none" class="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 mb-3 col-form-label" for="txtAccountID2">Account ID 2:</label>
                            <div class="col-md">
                                <input type="text" id="txtAccountID2" data-role="none" class="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 mb-3 col-form-label" for="txtStoreID2">Store ID 2:</label>
                            <div class="col-md">
                                <input type="text" id="txtStoreID2" data-role="none" class="form-control" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="tab-pane fade" id="tabFowstShopperTrak">
                    <div id="divOrgFOWSTShopperTrak" class="divExcludedInMC">
                        <h1 class="display-6 mb-4">ShopperTrak</h1>

                        <div class="row mb-3">
                            <label class="col-md-3 mb-3 col-form-label" for="selOrgFOWSTIsIncludeToShopperTrak">Include FOWST transactions in shoppertrak report?</label>
                            <div class="col-md">
                                <div class="form-check form-switch mb-3" style="font-size:20px;">
                                    <input class="form-check-input" data-role="none" type="checkbox" id="selOrgFOWSTIsIncludeToShopperTrak">
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 mb-3 col-form-label" for="txtShopperTrakAccountID">Account ID:</label>
                            <div class="col-md">
                                <input type="text" id="txtShopperTrakAccountID" data-role="none" class="form-control" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="tab-pane fade" id="tabFowstPrinters">
                    <div id="divOrgFOWSTPrinter" class="divExcludedInMC">
                        <div class="row mb-4 align-items-center">
                            <div class="col-6 mb-3">
                                <h1 class="display-6">Printers</h1>
                            </div>
                            <div class="col-6 text-right">
                                <button id="btnAddOrgFOWSTPrinter" data-role="none" class="btn btn-success">Add Printer</button>
                            </div>
                        </div>

                        <input type="hidden" data-role="none" class="form-control" id="txtPrinterID" value="" />

                        <div data-role="none" id="lstOrgFOWSTPrinter" class="list-group"></div>

                        <table data-role="none" class="table table-striped table-hover">
                            <thead>
                                <tr class="custom-theme-f txt-center">
                                    <th>Printer Name</th>
                                    <th>Printer Role</th>
                                    <th>IP Address</th>
                                    <th style="width:50px;"></th>
                                </tr>
                            </thead>
                            <tbody id="tbodyFowstPrinter"></tbody>
                        </table>
                    </div>
                </div>

                <div class="tab-pane fade" id="tabFowstQuota">
                    <div id="divOrgSalesQuota" class="divExcludedInMC">
                        <h1 class="display-6 mb-4">Quota</h1>

                        <table data-role="none" class="table table-striped table-hover">
                            <thead>
                                <tr>
                                    <th style="text-align:center; font-size:large"> Month </th>
                                    <th style="text-align:center; font-size:large"> Quota </th>
                                    <th style="text-align:center; font-size:large"> Incentive (%) </th>
                                </tr>
                            </thead>

                            <tbody>
                                <tr>
                                    <td style="vertical-align:middle"> January </td>
                                    <td> <input id="txtOrgTeamQuotaTier1Jan" value="" data-role="none" class="form-control" />  </td>
                                    <td> <input id="txtOrgTeamIncentiveTier1Jan" value="" data-role="none" class="form-control" /> </td>
                                </tr>
                                <tr>
                                    <td style="vertical-align:middle"> February </td>
                                    <td> <input id="txtOrgTeamQuotaTier1Feb" value="" data-role="none" class="form-control" />  </td>
                                    <td> <input id="txtOrgTeamIncentiveTier1Feb" value="" data-role="none" class="form-control" /> </td>
                                </tr>
                                <tr>
                                    <td style="vertical-align:middle"> March </td>
                                    <td> <input id="txtOrgTeamQuotaTier1Mar" value="" data-role="none" class="form-control" />  </td>
                                    <td> <input id="txtOrgTeamIncentiveTier1Mar" value="" data-role="none" class="form-control" /> </td>
                                </tr>
                                <tr>
                                    <td style="vertical-align:middle"> April </td>
                                    <td> <input id="txtOrgTeamQuotaTier1Apr" value="" data-role="none" class="form-control" />  </td>
                                    <td> <input id="txtOrgTeamIncentiveTier1Apr" value="" data-role="none" class="form-control" /> </td>
                                </tr>
                                <tr>
                                    <td style="vertical-align:middle"> May </td>
                                    <td> <input id="txtOrgTeamQuotaTier1May" value="" data-role="none" class="form-control" />  </td>
                                    <td> <input id="txtOrgTeamIncentiveTier1May" value="" data-role="none" class="form-control" /> </td>
                                </tr>
                                <tr>
                                    <td style="vertical-align:middle"> June </td>
                                    <td> <input id="txtOrgTeamQuotaTier1Jun" value="" data-role="none" class="form-control" />  </td>
                                    <td> <input id="txtOrgTeamIncentiveTier1Jun" value="" data-role="none" class="form-control" /> </td>
                                </tr>
                                <tr>
                                    <td style="vertical-align:middle"> July </td>
                                    <td> <input id="txtOrgTeamQuotaTier1Jul" value="" data-role="none" class="form-control" />  </td>
                                    <td> <input id="txtOrgTeamIncentiveTier1Jul" value="" data-role="none" class="form-control" /> </td>
                                </tr>
                                <tr>
                                    <td style="vertical-align:middle"> August </td>
                                    <td> <input id="txtOrgTeamQuotaTier1Aug" value="" data-role="none" class="form-control" />  </td>
                                    <td> <input id="txtOrgTeamIncentiveTier1Aug" value="" data-role="none" class="form-control" /> </td>
                                </tr>
                                <tr>
                                    <td style="vertical-align:middle"> September </td>
                                    <td> <input id="txtOrgTeamQuotaTier1Sept" value="" data-role="none" class="form-control" />  </td>
                                    <td> <input id="txtOrgTeamIncentiveTier1Sept" value="" data-role="none" class="form-control" /> </td>
                                </tr>
                                <tr>
                                    <td style="vertical-align:middle"> October </td>
                                    <td> <input id="txtOrgTeamQuotaTier1Oct" value="" data-role="none" class="form-control" />  </td>
                                    <td> <input id="txtOrgTeamIncentiveTier1Oct" value="" data-role="none" class="form-control" /> </td>
                                </tr>
                                <tr>
                                    <td style="vertical-align:middle"> November </td>
                                    <td> <input id="txtOrgTeamQuotaTier1Nov" value="" data-role="none" class="form-control" />  </td>
                                    <td> <input id="txtOrgTeamIncentiveTier1Nov" value="" data-role="none" class="form-control" /> </td>
                                </tr>
                                <tr>
                                    <td style="vertical-align:middle"> December </td>
                                    <td> <input id="txtOrgTeamQuotaTier1Dec" value="" data-role="none" class="form-control" />  </td>
                                    <td> <input id="txtOrgTeamIncentiveTier1Dec" value="" data-role="none" class="form-control" /> </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>

                <div class="tab-pane fade" id="tabFowstProductMapping">
                    <div id="divProductMap" class="divExcludedInMC">
                        <h1 class="display-6 mb-4">Product Mapping</h1>

                        <form id="formProductMapping" enctype="multipart/form-data" method="post">
                            <h5 class="mb-4">Please upload an Excel spreadsheet with the following headers: Third Party Product Number, Third Party Product Name, MPOS Product Number</h5>
                            <h5 id="productMappingResult"></h5>
                            <div class="row mb-3">
                                <label class="col-sm-2 mb-3 col-form-label" for="objFile">Upload Excel File:</label>
                                <div class="col-sm-10">
                                    <input type="file" data-role="none" id="objFile" class="form-control" accept=".xls,.xlsx" name="objFile">
                                </div>
                            </div>
                            <div class="text-center mb-3">
                                <button id="btnPIMUploadBackToHome" data-role="none" class="btn btn-secondary btn-lg">Cancel</button>
                                <button type="submit" data-role="none" class="btn btn-success btn-lg">Upload File</button>
                                <button id="btnPIMUploadDownload" data-role="none" class="btn btn-secondary btn-lg">Download</button>
                            </div>
                        </form>
                    </div>
                </div>

                <div class="tab-pane fade" id="tabFowstPriceLevels">
                    <div class="divExcludedInMC">
                        <div class="text-right mb-3">
                            <button id="btnRevPriceLevel" data-role="none" class="btn btn-success">
                                Add Price Level
                            </button>
                        </div>

                        <div data-role="none" id="lstFowstPriceLevel" class="list-group mb-4"></div>
                    </div>
                </div>

                <div class="tab-pane fade" id="tabFowstAssignedDevices">
                    <table data-role="none" class="table table-light table-hover table-striped">
                        <thead>
                            <tr class="text-center align-items-center">
                                <th>#</th>
                                <th>Device Name</th>
                                <th>Serial Number</th>
                                <th>Registration Number</th>
                            </tr>
                        </thead>
                        <tbody id="tbodyOrgFOWSTDevices"></tbody>
                    </table>
                </div>

                <div class="tab-pane fade" id="tabFowstAssignedEmployees">
                    <input data-role="none" type="text" class="form-control mb-3" id="txtSearchAssignedEmp" placeholder="Search here" />
                    <table id="tblFOWSTAssignedEmployees" data-role="none" class="table table-light table-hover table-striped w-100">
                        <thead>
                            <tr class="text-center align-items-center">
                                <th class="fixed-col" style="width:50px;">#</th>
                                <th class="fixed-col">Employee Name</th>
                                <th class="fixed-col">Position</th>
                                <th class="fixed-col" style="width:50px;"></th>
                            </tr>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>

    <div class="fixed-bottom text-center pb-3" style="display:block; left:50%; transform: translate(-50%, 0); right:auto">
        <a id="btnOrgFOWSTCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
        <a id="btnOrgFOWSTSave" data-role="none" class="btn btn-success btn-lg">Save</a>
    </div>
</div>

<div id="pageOrgFOWSTTruckRegInfo" style="display:none;">
    <div data-role="fieldcontain" style="display:none;">
        <input type="text" data-role="none" class="form-control" readonly id="txtOrgFOWSTTRegID" />
    </div>

    <h1 class="display-6 mb-4">Registration Info</h1>

    <div class="row mb-3">
        <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTTRegORNo">OR No.: <span style="color: #FF0000;">*</span></label>
        <div class="col-md">
            <input type="text" id="txtOrgFOWSTTRegORNo" data-role="none" class="form-control" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-md-3 mb-3 col-form-label" for="selOrgFOWSTOwnershipType">Issued Date:</label>
        <div class="col-sm-1">
            <select id="selOrgFOWSTTRegIssuedMonth" data-role="none" class="form-select">
                <option value="">MMM</option>
            </select>
        </div>
        <div class="col-sm-1">
            <select id="selOrgFOWSTTRegIssuedDay" data-role="none" class="form-select">
                <option value="">DD</option>
            </select>
        </div>
        <div class="col-sm-1">
            <select id="selOrgFOWSTTRegIssuedYear" data-role="none" class="form-select">
                <option value="">YYYY</option>
            </select>
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-md-3 mb-3 col-form-label" for="selOrgFOWSTOwnershipType">Expiration Date:</label>
        <div class="col-sm-1">
            <select id="selOrgFOWSTTRegExpirationMonth" data-role="none" class="form-select">
                <option value="">MMM</option>
            </select>
        </div>
        <div class="col-sm-1">
            <select id="selOrgFOWSTTRegExpirationDay" data-role="none" class="form-select">
                <option value="">DD</option>
            </select>
        </div>
        <div class="col-sm-1">
            <select id="selOrgFOWSTTRegExpirationYear" data-role="none" class="form-select">
                <option value="">YYYY</option>
            </select>
        </div>
    </div>

    <div class="mt-4 text-center">
        <a id="btnOrgFOWSTTRegCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
        @*<a id="btnOrgFOWSTTRegRemove" data-role="none" class="btn btn-danger btn-lg">Remove</a>*@
        <a id="btnOrgFOWSTTRegSave" data-role="none" class="btn btn-success btn-lg">Ok</a>
    </div>
</div>

<div id="pageOrgFOWSTSectionInfo" style="display:none;">
    <div data-role="fieldcontain" style="display:none;">
        <input type="text" readonly data-role="none" class="form-control" id="txtOrgFOWSTSecID" />
    </div>

    <h1 class="display-6 mb-4">Section Info</h1>

    <div class="row mb-3">
        <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTSecCode">Code:</label>
        <div class="col-md">
            <input type="text" id="txtOrgFOWSTSecCode" data-role="none" class="form-control" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTSecName">Name: <span style="color: #FF0000;">*</span></label>
        <div class="col-md">
            <input type="text" id="txtOrgFOWSTSecName" data-role="none" class="form-control" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTSecDescription">Description:</label>
        <div class="col-md">
            <input type="text" id="txtOrgFOWSTSecDescription" data-role="none" class="form-control" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTSecFloorArea">Floor Area:</label>
        <div class="col-md">
            <input type="number" id="txtOrgFOWSTSecFloorArea" data-role="none" class="form-control" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTSecSpecFeatures">Special Features:</label>
        <div class="col-md">
            <input type="text" id="txtOrgFOWSTSecSpecFeatures" data-role="none" class="form-control" />
        </div>
    </div>

    <div id="divOrgFOWSTSectionBin">
        <div class="row mb-3 align-items-center">
            <div class="col-6 mb-3">
                <h1 class="display-6">Bin</h1>
            </div>
            <div class="col-6 text-right">
                <button id="btnAddOrgFOWSTSectionBins" data-role="none" class="btn btn-success">Add Bin</button>
            </div>
        </div>

        <div data-role="none" id="lstOrgFOWSTSectionBin" class="list-group mb-3">
            <a data-role="none" href="#" class="list-group-item list-group-item-action">No result(s) found</a>
        </div>
    </div>

    <div id="divOrgFOWSTSectionPeople">
        <div class="row mb-3 align-items-center">
            <div class="col-6 mb-3">
                <h1 class="display-6">Section Picker</h1>
            </div>
            <div class="col-6 text-right">
                <button id="btnAddOrgFOWSTSectionPeople" data-role="none" class="btn btn-success">Add Section Picker</button>
            </div>
        </div>

        <div data-role="none" id="lstOrgFOWSTSectionPeople" class="list-group mb-3">
            <a data-role="none" href="#" class="list-group-item list-group-item-action">No result(s) found</a>
        </div>
    </div>

    <div class="mt-4 text-center">
        <a id="btnOrgFOWSTSecCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
        @*<a id="btnOrgFOWSTSecRemove" data-role="none" class="btn btn-danger btn-lg">Remove</a>*@
        <a id="btnOrgFOWSTSecSave" data-role="none" class="btn btn-success btn-lg">Ok</a>
    </div>
</div>

<div id="pageOrgFOWSTBusinessPermitInfo" style="display:none;">
    <div data-role="fieldcontain" style="display:none;">
        <input type="text" readonly data-role="none" class="form-control" id="txtOrgFOWSTBPermitRecID" />
    </div>

    <h1 class="display-6 mb-4">Business Permit Info</h1>

    <div class="row mb-3">
        <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTBPermitNo">Business Permit No.: <span style="color: #FF0000;">*</span></label>
        <div class="col-md">
            <input type="text" id="txtOrgFOWSTBPermitNo" data-role="none" class="form-control" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTBPermitID">Business Permit ID:</label>
        <div class="col-md">
            <input type="text" id="txtOrgFOWSTBPermitID" data-role="none" class="form-control" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTBPermitTradeName">Trade Name: <span style="color: #FF0000;">*</span></label>
        <div class="col-md">
            <input type="text" id="txtOrgFOWSTBPermitTradeName" data-role="none" class="form-control" />
        </div>
    </div>

    <div>
        <h1 class="display-6 mb-4">Complete Address</h1>

        <div class="row mb-3">
            <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTBPermitBuildingNo">House or Building No.: <span style="color: #FF0000;">*</span></label>
            <div class="col-md">
                <input type="text" id="txtOrgFOWSTBPermitBuildingNo" data-role="none" class="form-control" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTBPermitStreet">Street: <span style="color: #FF0000;">*</span></label>
            <div class="col-md">
                <input type="text" id="txtOrgFOWSTBPermitStreet" data-role="none" class="form-control" />
            </div>
        </div>

        <div style="display:none;"><input type="text" readonly id="txtOrgFOWSTBPermitCityID" /></div>
        <div class="row mb-3">
            <label class="col-md-3 mb-3 col-form-label" for="btnOrgFOWSTBPermitCity">City or Town: <span style="color: #FF0000;">*</span></label>
            <div class="col-md">
                <button id="btnOrgFOWSTBPermitCity" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
            </div>
        </div>

        <div style="display:none;"><input type="text" readonly id="txtOrgFOWSTBPermitCountryID" /></div>
        <div class="row mb-3">
            <label class="col-md-3 mb-3 col-form-label" for="btnOrgFOWSTBPermitCountry">Country: <span style="color: #FF0000;">*</span></label>
            <div class="col-md">
                <button id="btnOrgFOWSTBPermitCountry" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
            </div>
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-md-3 mb-3 col-form-label" for="btnDateOfPayment">Date of Payment:</label>
        <div class="col-md">
            <button id="btnDateOfPayment" data-role="none" class="btn btn-secondary btn-block">Set Date</button>
        </div>
    </div>

    @*<div class="row mb-3">
            <label class="col-md-3 mb-3 col-form-label" for="selOrgFOWSTOwnershipType">Date Of Payment:</label>
            <div class="col-sm-1">
                <select id="selOrgFOWSTBPermitDateOfPaymentMonth" data-role="none" class="form-select">
                    <option value="">MMM</option>
                </select>
            </div>
            <div class="col-sm-1">
                <select id="selOrgFOWSTBPermitDateOfPaymentDay" data-role="none" class="form-select">
                    <option value="">DD</option>
                </select>
            </div>
            <div class="col-sm-1">
                <select id="selOrgFOWSTBPermitDateOfPaymentYear" data-role="none" class="form-select">
                    <option value="">YYYY</option>
                </select>
            </div>
        </div>*@

    <div class="row mb-3 mt-1">
        <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTBPermitORNo">Official Receipt No.:</label>
        <div class="col-md">
            <input type="text" data-role="none" class="form-control" id="txtOrgFOWSTBPermitORNo" />
        </div>
    </div>

    <div class="row mb-3 mt-1">
        <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTBPermitBPlateNo">Business Plate No.: <span style="color: #FF0000;">*</span></label>
        <div class="col-md">
            <input type="text" data-role="none" class="form-control" id="txtOrgFOWSTBPermitBPlateNo" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-md-3 mb-3 col-form-label" for="btnDateIssued">Date Issued:</label>
        <div class="col-md">
            <button id="btnDateIssued" data-role="none" class="btn btn-secondary btn-block">Set Date</button>
        </div>
    </div>

    @*<div class="row mb-3">
            <label class="col-md-3 mb-3 col-form-label" for="selOrgFOWSTOwnershipType">Date Issued:</label>
            <div class="col-sm-1">
                <select id="selOrgFOWSTBPermitBPlateIssuedMonth" data-role="none" class="form-select">
                    <option value="">MMM</option>
                </select>
            </div>
            <div class="col-sm-1">
                <select id="selOrgFOWSTBPermitBPlateIssuedDay" data-role="none" class="form-select">
                    <option value="">DD</option>
                </select>
            </div>
            <div class="col-sm-1">
                <select id="selOrgFOWSTBPermitBPlateIssuedYear" data-role="none" class="form-select">
                    <option value="">YYYY</option>
                </select>
            </div>
        </div>*@

    <div class="mt-4 text-center">
        <a id="btnOrgFOWSTBPermitCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
        @*<a id="btnOrgFOWSTBPermitRemove" data-role="none" class="btn btn-danger btn-lg">Remove</a>*@
        <a id="btnOrgFOWSTBPermitSave" data-role="none" class="btn btn-success btn-lg">Ok</a>
    </div>
</div>

<div id="pageOrgFOWSTTelInfo" style="display:none;">

    @*<div data-role="header">
            <h1>Telephone Info</h1>
            <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        </div>*@

    <h1 class="display-6 mb-4">Telephone Info</h1>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
            <input type="text" readonly id="txtOrgFOWSTTelCountryID" />
            <input type="text" readonly id="txtOrgFOWSTTelID" />
            <input type="text" readonly id="txtOrgFOWSTTelAreaCodeID" />
        </div>

        <div class="row mb-3">
            <label class="col-md-3 mb-3 col-form-label" for="btnOrgFOWSTTelCountryCode">Country Code:</label>
            <div class="col-md">
                <button id="btnOrgFOWSTTelCountryCode" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-3 mb-3 col-form-label" for="btnOrgFOWSTTelAreaCode">Area Code:</label>
            <div class="col-md">
                <button id="btnOrgFOWSTTelAreaCode" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTTelNo">Telephone No.: <span style="color: #FF0000;">*</span></label>
            <div class="col-md">
                <input type="number" id="txtOrgFOWSTTelNo" data-role="none" class="form-control" maxlength="7" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTTelExtension">Extension or Local No.: <span style="color: #FF0000;">*</span></label>
            <div class="col-md">
                <input type="number" id="txtOrgFOWSTTelExtension" data-role="none" class="form-control" maxlength="10" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-3 mb-3 col-form-label" for="selOrgFOWSTTelType">Contact Type: <span style="color: #FF0000;">*</span></label>
            <div class="col-md">
                <select id="selOrgFOWSTTelType" data-role="none" class="form-select">
                    <option value="">Please Select</option>
                    <option value="Work Telephone">Work Telephone</option>
                    <option value="Personal Telephone">Personal Telephone</option>
                    <option value="Third Party Telephone">Third Party Telephone</option>
                </select>
            </div>
        </div>

        <div class="mt-4 text-center">
            <a id="btnOrgFOWSTTelCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
            @*<a id="btnOrgFOWSTTelRemove" data-role="none" class="btn btn-danger btn-lg">Remove</a>*@
            <a id="btnOrgFOWSTTelSave" data-role="none" class="btn btn-success btn-lg">Ok</a>
        </div>
    </div>
</div>

<div id="pageOrgFOWSTMobileInfo" style="display:none;">

    @*<div data-role="header">
            <h1>Mobile Info</h1>
            <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        </div>*@

    <h1 class="display-6 mb-4">Mobile Info</h1>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
            <input type="text" readonly id="txtOrgFOWSTMobileID" />
            <input type="text" readonly id="txtOrgFOWSTMobileCountryID" />
            <input type="text" readonly id="txtOrgFOWSTTelCoPrefixID" />
        </div>

        <div class="row mb-3">
            <label class="col-md-3 mb-3 col-form-label" for="btnOrgFOWSTMobileCountryCode">Country Code:</label>
            <div class="col-md">
                <button id="btnOrgFOWSTMobileCountryCode" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-3 mb-3 col-form-label" for="btnOrgFOWSTMobileTelCoPrefixCode">Telco Code:</label>
            <div class="col-md">
                <button id="btnOrgFOWSTMobileTelCoPrefixCode" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTMobileNo">Mobile No.: <span style="color: #FF0000;">*</span></label>
            <div class="col-md">
                <input type="number" id="txtOrgFOWSTMobileNo" data-role="none" class="form-control" maxlength="7" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-3 mb-3 col-form-label" for="selOrgAddMobileType">Contact Type: <span style="color: #FF0000;">*</span></label>
            <div class="col-md">
                <select id="selOrgAddMobileType" data-role="none" class="form-select">
                    <option value="">Please Select</option>
                    <option value="Work Mobile">Work Mobile</option>
                    <option value="Personal Mobile">Personal Mobile</option>
                    <option value="Third Party Mobile">Third Party Mobile</option>
                </select>
            </div>
        </div>

        <div class="mt-4 text-center">
            <a id="btnOrgFOWSTMobileCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
            @*<a id="btnOrgFOWSTMobileRemove" data-role="none" class="btn btn-danger btn-lg">Remove</a>*@
            <a id="btnOrgFOWSTMobileSave" data-role="none" class="btn btn-success btn-lg">Ok</a>
        </div>
    </div>
</div>

<div id="pageOrgFOWSTEmailInfo" style="display:none;">

    @*<div data-role="header">
            <h1>Email Info</h1>
            <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        </div>*@

    <h1 class="display-6 mb-4">Email Info</h1>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
            <input type="text" readonly id="txtOrgFOWSTEmailID" />
        </div>

        <div class="row mb-3">
            <label class="col-md-3 mb-3 col-form-label" for="txtOrgFOWSTEmail">Email Address: <span style="color: #FF0000;">*</span></label>
            <div class="col-md">
                <input type="email" id="txtOrgFOWSTEmail" data-role="none" class="form-control" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-3 mb-3 col-form-label" for="selOrgFOWSTEmailType">Email Type: <span style="color: #FF0000;">*</span></label>
            <div class="col-md">
                <select id="selOrgFOWSTEmailType" data-role="none" class="form-select">
                    <option value="">Please Select</option>
                    <option value="Work Mobile">Work Mobile</option>
                    <option value="Personal Mobile">Personal Mobile</option>
                    <option value="Third Party Mobile">Third Party Mobile</option>
                </select>
            </div>
        </div>

        <div class="mt-4 text-center">
            <a id="btnOrgFOWSTEmailCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
            @*<a id="btnOrgFOWSTEmailRemove" data-role="none" class="btn btn-danger btn-lg">Remove</a>*@
            <a id="btnOrgFOWSTEmailSave" data-role="none" class="btn btn-success btn-lg">Ok</a>
        </div>
    </div>
</div>

<div id="pageOrgFOWSTOperationHRInfo" style="display:none;">

    @*<div data-role="header">
            <h1>Operation Hour Info</h1>
            <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        </div>*@

    <h1 class="display-6 mb-4">Operation Hour Info</h1>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
            <input type="text" readonly id="txtOrgFOWSTOptHRID" />
        </div>

        <div class="row mb-3">
            <label class="col-md-2 mb-3 col-form-label" for="selOrgFOWSTOptHRDay">Day</label>
            <div class="col-md">
                <select id="selOrgFOWSTOptHRDay" data-role="none" class="form-select" disabled>
                    <option value="1">Sunday</option>
                    <option value="2">Monday</option>
                    <option value="3">Tuesday</option>
                    <option value="4">Wednesday</option>
                    <option value="5">Thursday</option>
                    <option value="6">Friday</option>
                    <option value="7">Saturday</option>
                </select>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 mb-3 col-form-label" for="selOrgFOWSTOwnershipType">Opening time:</label>
            <div class="col-sm-1">
                <select id="selOrgFOWSTOptOpenHR" data-role="none" class="form-select">
                    <option value="">HOUR</option>
                </select>
            </div>
            <div class="col-sm-1">
                <select id="selOrgFOWSTOptOpenMin" data-role="none" class="form-select">
                    <option value="">MIN</option>
                </select>
            </div>
            <div class="col-sm-1">
                <select id="selOrgFOWSTOptOpenAMPM" data-role="none" class="form-select">
                    <option value="">AM/PM</option>
                    <option value="AM">AM</option>
                    <option value="PM">PM</option>
                </select>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 mb-3 col-form-label" for="selOrgFOWSTOwnershipType">Closing time:</label>
            <div class="col-sm-1">
                <select id="selOrgFOWSTOptCloseHR" data-role="none" class="form-select">
                    <option value="">HOUR</option>
                </select>
            </div>
            <div class="col-sm-1">
                <select id="selOrgFOWSTOptCloseMin" data-role="none" class="form-select">
                    <option value="">MIN</option>
                </select>
            </div>
            <div class="col-sm-1">
                <select id="selOrgFOWSTOptCloseAMPM" data-role="none" class="form-select">
                    <option value="">AM/PM</option>
                    <option value="AM">AM</option>
                    <option value="PM">PM</option>
                </select>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 mb-3" for="isFOWSTClosed">Is this store closed on this day?</label>
            <div class="col-md">
                <div class="form-check form-switch" style="font-size:25px;">
                    <input class="form-check-input" type="checkbox" id="isFOWSTClosed">
                </div>
            </div>
        </div>

        <div class="mt-4 text-center">
            <a id="btnOrgFOWSTOptHRCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
            <a id="btnOrgFOWSTOptHRSave" data-role="none" class="btn btn-success btn-lg">Ok</a>
        </div>
    </div>
</div>

<div id="pageOrgFOWSTPrinterInfo" style="display:none;">

    @*<div data-role="header">
            <h1>Printer Roles</h1>
            <a href="#" data-icon="arrow-l" id="btnPrinterBack">Back</a>
        </div>*@

    <h1 class="display-6 mb-4">Printer Roles</h1>

    <div data-role="content">
        <div id="divDevicePrinterResults" style="display:none;">
            <div id="lstOrgFOWSTPrinterList" data-role="none" class="list-group mb-3"></div>
        </div>

        <div id="divDevicePrinterSearchNoResults" style="display:none;">
            <h5 id="lblDevicePrinterNotFound" class="mb-3">No records found.</h5>
        </div>

        <div class="mt-4 text-center">
            <a id="btnDevicePrinterBackToHome" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
            <a id="btnAddNewDevicePrinterOrg" data-role="none" class="btn btn-success btn-lg">Add New Printer Role</a>
        </div>
    </div>
</div>

<div id="pageOrgFOWSTPrinterIP" style="display:none;">

    @*<div data-role="header">
            <h1>Add Printer to FOWST</h1>
            <a href="#" data-icon="arrow-l" id="btnPrinterIPBack">Back</a>
        </div>*@

    <h1 class="display-6 mb-4">Add Printer to FOWST</h1>

    <div data-role="content">
        <h3 id="txtPrinterRole"></h3>

        <div data-role="fieldcontain" style="display:none;">
            <input type="text" readonly data-role="none" class="form-control" id="txtPrinterRoleID" />
        </div>

        <div class="row mb-3">
            <label class="col-md-3 mb-3 col-form-label" for="txtPrinterName">Printer Name: <span style="color: #FF0000;">*</span></label>
            <div class="col-md">
                <input type="text" id="txtPrinterName" data-role="none" class="form-control" maxlength="50" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-3 mb-3 col-form-label" for="selConnectionType">Printer Connection Type: <span style="color: #FF0000;">*</span></label>
            <div class="col-md">
                <select id="selConnectionType" data-role="none" class="form-select">
                    <option value="TCP/IP"> TPC/IP </option>
                    <option value="USB"> USB </option>
                    <option value="Bluetooth"> Bluetooth </option>
                </select>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-3 mb-3 col-form-label" for="txtPrinterIPAddress">Printer IP Address: <span style="color: #FF0000;">*</span></label>
            <div class="col-md">
                <input type="text" id="txtPrinterIPAddress" data-role="none" class="form-control" maxlength="15" />
                <input type="hidden" id="txtPrinterFowstID" data-role="none" class="form-control" value="" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-3 mb-3 col-form-label" for="selProductDescriptionInReceipt">Product Description to be use in Printing SI?</label>
            <div class="col-md">
                <select id="selProductDescriptionInReceipt" data-role="none" class="form-select">
                    <option value="ProductLongDescription" selected="selected">Product Long Description</option>
                    <option value="ProductShortDescription">Product Short Description</option>
                </select>
            </div>
        </div>

        <div class="row mb-3 divExcludedInMC">
            <label class="col-md-3 mb-3 col-form-label" for="selIsDispatchPrinter">Assign as dispatch printer?</label>
            <div class="col-md">
                <div class="form-check form-switch mb-3" style="font-size:20px;">
                    <input class="form-check-input" data-role="none" type="checkbox" id="selIsDispatchPrinter">
                </div>
            </div>
        </div>

        <div class="mt-4 text-center">
            <a id="btnOrgFOWSTPrinterCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
            @*<a id="btnOrgFOWSTPrinterRemove" data-role="none" class="btn btn-danger btn-lg">Remove</a>*@
            <a id="btnOrgFOWSTPrinterSave" data-role="none" class="btn btn-success btn-lg">Ok</a>
        </div>
    </div>
</div>

<div id="pageAddDevicePrinter" style="display:none;"></div>