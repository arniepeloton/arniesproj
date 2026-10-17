@Code
    ViewData("Title") = "Dashboards and Reports"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code

@Section head
    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
                require(['/Scripts/dom-to-image.min.js'], function () {
                    require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/MPOSUtilization.js?v=' + assetversion)]);
                })
            })
        })
    </script>

    <style>
        .mposutil h1 {
            font-size: clamp(3rem, 6vw, 4rem);
        }

        .mposutil h2 {
            font-size: clamp(3rem, 6vw, 3.5rem);
        }

        .mposutil h3 {
            font-size: clamp(2rem, 6vw, 3rem);
        }

        .mposutil h4 {
            font-size: clamp(1.5rem, 6vw, 2rem);
        }

        .mposutil h5 {
            font-size: clamp(1.25rem, 6vw, 1rem);
        }

        .mposutil h6 {
            font-size: clamp(1rem, 6vw, 0.5rem);
        }
    </style>
End Section

<div class="d-flex flex-column overflow-hidden h-100 mposutil">
    <div class="flex-shrink-1 bg-white shadow-sm mb-3 rounded-1">
        <div class="row m-0 g-sm-0 gy-3 p-3 align-items-center">
            <div class="col-sm-auto">
                <div class="btn-group w-100">
                    <input data-role="none" type="radio" class="btn-check" name="btnMPOSUtil" id="btnMPOSUtilSystemWide" value="systemwide" autocomplete="off" checked>
                    <label class="btn btn-outline-secondary border-0 rounded" for="btnMPOSUtilSystemWide">System Wide</label>

                    <input data-role="none" type="radio" class="btn-check" name="btnMPOSUtil" id="btnMPOSUtilMonthlyUtil" value="monthlyutil" autocomplete="off">
                    <label class="btn btn-outline-secondary border-0 rounded" for="btnMPOSUtilMonthlyUtil">Utilization Report</label>

                    <input data-role="none" type="radio" class="btn-check" name="btnMPOSUtil" id="btnMPOSUtilFranchisedStores" value="franchisedstores" autocomplete="off">
                    <label class="btn btn-outline-secondary border-0 rounded" for="btnMPOSUtilFranchisedStores">Franchised Stores</label>
                </div>
            </div>
            <div class="col-12 col-sm text-right">
                <button data-role="none" class="btn btn-light border" id="btnMPOSUtilDownloadReports">Download</button>
            </div>
        </div>
    </div>

    <div class="d-flex flex-fill align-items-start flex-column overflow-auto w-100 bg-white shadow-sm rounded-1">
        <div class="row g-0 p-3 justify-content-center w-100 h-100">
            <div id="divSystemWide" subpage="pageMPOSUtil" name="System Wide" class="col-md col h-100">
                <div class="row m-0 gy-2 gx-3 mb-3 align-items-center">
                    <div class="col-md-auto">
                        <div class="row">
                            <label for="btnSystemWideDate" class="col-2 col-md-auto col-form-label">Report:</label>
                            <div class="col col-md-auto">
                                <select data-role="none" class="form-select" id="selSWReport">
                                    <option value="Summary">Summary</option>
                                    <option value="ByStore">Store Trends</option>
                                    <option value="Breakdown">Breakdown per Store</option>
                                    <option value="Flushing History">Flushing History</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-auto divSummaryDate">
                        <div class="row">
                            <label for="btnSystemWideDate" class="col-2 col-md-auto col-form-label">Date:</label>
                            <div class="col col-md-auto">
                                <button data-role="none" id="btnSystemWideDate" class="btn btn-light border btn-block">Set Date</button>
                                <input data-role="none" type="date" class="form-control d-none" id="txtMPOSDate">
                            </div>
                        </div>
                    </div>
                    <div class="col-md-auto divFHDate" style="display:none">
                        <div class="row">
                            <label for="butSWFHDate" class="col-2 col-md-auto col-form-label">Date:</label>
                            <div class="col col-md-auto">
                                <button data-role="none" id="butSWFHDate" class="btn btn-light border btn-block">Set Date</button>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-auto divFHDate" style="display:none">
                        <div class="row">
                            <label for="selSDFHGroup" class="col-2 col-md-auto col-form-label">Group:</label>
                            <div class="col col-md-auto">
                                <select data-role="none" class="form-select" id="selSDFHGroup">
                                    <option value="All">All</option>
                                    <option name="group_co" value="CO - GMAN">CO - GMAN</option>
                                    <option name="group_co" value="CO - GMAS">CO - GMAS</option>
                                    <option name="group_co" value="CO - North Luzon">CO - North Luzon</option>
                                    <option name="group_co" value="CO - South Luzon">CO - South Luzon</option>
                                    <option name="group_co" value="CO - Visayas">CO - Visayas</option>
                                    <option name="group_co" value="CO - Mindanao">CO - Mindanao</option>
                                    <option name="group_fo" value="Cebu">Cebu</option>
                                    <option name="group_fo" value="FO - GMAN">FO - GMAN</option>
                                    <option name="group_fo" value="FO - GMAS">FO - GMAS</option>
                                    <option name="group_fo" value="FO - North Luzon">FO - North Luzon</option>
                                    <option name="group_fo" value="FO - South Luzon">FO - South Luzon</option>
                                    <option name="group_fo" value="FO - Visayas">FO - Visayas</option>
                                    <option name="group_fo" value="FO - Mindanao">FO - Mindanao</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-auto divFHDate" style="display:none">
                        <input data-role="none" class="form-control" placeholder="Search here" id="txtSDFHSearch" />
                    </div>
                </div>

                <div id="divSWSummary" subpage="divSystemWide" style="display:block;">
                    <div class="row gy-3 mb-3">
                        @*row-cols-2 row-cols-sm-2 row-cols-md-3 row-cols-lg-3*@
                        <div class="col-12 col-sm-6 col-md-6 col-lg-4">
                            <div id="btnSummaryCO" class="p-2 rounded-1 text-center shadow-sm h-100" style="background-color: #dceaff;">
                                <div class="mb-3 text-left text-primary"><span class="bi-buildings"></span> Company-Owned Stores </div>
                                <div class="row g-0 align-items-center">
                                    <div class="col border-primary border-right">
                                        <h1 class="text-primary font-weight-bold mb-0" id="lblCompanyOwnedStoreCount">0</h1>
                                        <span class="text-secondary">Utilized</span>
                                    </div>
                                    <div class="col border-primary border-left">
                                        <h1 class="font-weight-bold text-secondary mb-0" id="lblCONotUtilizedCount">0</h1>
                                        <span class="text-secondary">Not Utilized</span>
                                    </div>
                                </div>
                                <div class="d-flex align-items-center justify-content-center" style="gap:15px;">
                                    <h4 class="text-secondary" id="lblCompanyOwnedStoreCounttOver" data-toggle="tooltip" title="Total CO Stores">0</h4>
                                    <h5 class="text-success" id="lblCompanyOwnedStoreCountPercentage" data-toggle="tooltip" title="Closed company stores not included in percentage.">0%</h5>
                                </div>
                            </div>
                        </div>
                        <div class="col-12 col-sm-6 col-md-6 col-lg-4">
                            <div id="btnSummaryFO" class="p-2 rounded-1 text-center shadow-sm h-100" style="background-color: #dcffe3;">
                                <div class="mb-3 text-left text-success"><span class="bi-shop"></span> Franchise Stores </div>
                                <div class="row g-0 align-items-center">
                                    <div class="col border-success border-right">
                                        <h1 class="text-success font-weight-bold mb-0" id="lblFranchiseStoreCount">0</h1>
                                        <span class="text-secondary">Utilized</span>
                                    </div>
                                    <div class="col border-success border-left">
                                        <h1 class="font-weight-bold text-secondary mb-0" id="lblFONotUtilizedCount">0</h1>
                                        <span class="text-secondary">Not Utilized</span>
                                    </div>
                                </div>
                                <div class="d-flex align-items-center justify-content-center" style="gap:15px;">
                                    <h4 class="text-secondary" id="lblFranchiseStoreCounttOver" data-toggle="tooltip" title="Total FO Stores">0</h4>
                                    <h5 class="text-success" id="lblFranchiseStorePercentage" data-toggle="tooltip" title="Closed franchise stores not included in percentage.">0%</h5>
                                </div>
                            </div>
                        </div>
                        <div class="col-12 col-sm-12 col-md-12 col-lg-4">
                            <div id="btnSummarySystemWide" class="p-2 rounded-1 text-center shadow-sm h-100" style="background-color: #fff3d0;">
                                <div class="mb-3 text-left text-warning"><span class="bi-globe"></span> System-Wide Stores</div>
                                <div class="row g-0 align-items-center">
                                    <div class="col border-warning border-right">
                                        <h1 class="text-warning font-weight-bold mb-0" id="lblSystemWideStoreCount" title="No. of Utilized">0</h1>
                                        <span class="text-secondary">Utilized</span>
                                    </div>
                                    <div class="col border-warning border-left">
                                        <h1 class="text-secondary font-weight-bold mb-0" id="lblSystemWideStoreCountNU" title="No. of Not Utilized">0</h1>
                                        <span class="text-secondary">Not Utilized</span>
                                    </div>
                                </div>
                                <div class="d-flex align-items-center justify-content-center" style="gap:15px;">
                                    <h4 class="text-secondary" id="lblSystemWideStoreCountOver" title="Total">0</h4>
                                    <h5 class="text-success" id="lblSystemWideStoreCountPercentage" title="Percentage">0%</h5>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-md-12 col-lg-6 order-0">
                            <div class="row mb-3">
                                <div class="col">
                                    <div class="p-2 rounded-1 text-center shadow-sm h-100 bg-light">
                                        <div class="row g-0 align-items-center mb-3">
                                            <div class="col-12">
                                                <span class="text-secondary font-weight-bold">Closed<br />Stores</span>
                                            </div>
                                            <div id="btnSummaryCOClosed" class="col-sm border-right" style="border-color: #ddd;">
                                                <h1 class="text-primary font-weight-bold mb-0" id="lblCloseCOStoreCount">0</h1>
                                                <h5 class="text-secondary mb-0">CO</h5>
                                            </div>
                                            <div id="btnSummaryFOClosed" class="col-sm border-left" style="border-color: #ddd;">
                                                <h1 class="text-success font-weight-bold mb-0" id="lblCloseFOStoreCount">0</h1>
                                                <h5 class="text-secondary mb-0">FO</h5>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col">
                                    <div class="p-2 rounded-1 text-center shadow-sm h-100 bg-light">
                                        <div class="row g-0 align-items-center">
                                            <div class="col-12">
                                                <span class="text-secondary font-weight-bold">No. of Stores<br />with 1 transaction</span>
                                            </div>
                                            <div class="col-sm border-right" style="border-color: #ddd;">
                                                <h1 class="text-primary font-weight-bold mb-0" id="lblFewestCOTransCount">0</h1>
                                                <h5 class="text-secondary mb-0">CO</h5>
                                            </div>
                                            <div class="col-sm border-left" style="border-color: #ddd;">
                                                <h1 class="text-success font-weight-bold mb-0" id="lblFewestFOTransCount">0</h1>
                                                <h5 class="text-secondary mb-0">FO</h5>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col">
                                    <div class="p-2 rounded-1 text-center shadow-sm h-100 bg-light">
                                        <div class="row g-0 align-items-center">
                                            <div class="col-12">
                                                <span class="text-secondary font-weight-bold">Pending<br />Queues</span>
                                            </div>
                                            <div class="col-sm border-right" style="border-color: #ddd;">
                                                <h1 class="text-primary font-weight-bold mb-0" id="lblCOUtilizedCount">0</h1>
                                                <h5 class="text-secondary mb-0">CO</h5>
                                            </div>
                                            <div class="col-sm border-left" style="border-color: #ddd;">
                                                <h1 class="text-success font-weight-bold mb-0" id="lblFOUtilizedCount">0</h1>
                                                <h5 class="text-secondary mb-0">FO</h5>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row gy-3 mb-3">
                                <div class="col">
                                    <canvas id="chartDeviceUtilCOAndFO" class="w-100 bg-white rounded" style="height:200px;"></canvas>
                                </div>
                                <div class="col">
                                    <canvas id="chartDeviceUtilCO" class="w-100 bg-white rounded" style="height:200px;"></canvas>
                                </div>
                                <div class="col">
                                    <canvas id="chartDeviceUtilFO" class="w-100 bg-white rounded" style="height:200px;"></canvas>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-12 col-lg-6 order-1">
                            <div class="d-flex flex-column bg-white border-light shadow-sm rounded-1 p-2">
                                <div class="flex-shrink-1 mb-3">
                                    <div class="row align-items-center">
                                        <div class="col-2 text-center">
                                            Regions:
                                        </div>
                                        <div class="col-10">
                                            <div id="divFORegions" class="btn-group w-100 overflow-auto text-nowrap pl-2 pr-2"></div>
                                        </div>
                                    </div>
                                </div>
                                @*<div id="divDMReport" class="pt-3">

                        </div>*@
                                <table data-role="none" class="table table-striped">
                                    <thead>
                                        <tr class="text-center">
                                            <th>Name</th>
                                            <th>Fully Utilized</th>
                                            <th>Utilized</th>
                                            <th>Not Utilized</th>
                                            <th>No. of Stores</th>
                                            <th>%</th>
                                        </tr>
                                    </thead>
                                    <tbody id="tbodyFORegionDMs"></tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>

                <div id="divSWByStores" subpage="divSystemWide" style="display:none;">
                    <div class="row">
                        <div class="col-md" style="height:500px;">
                            <canvas id="chartFullyUtilizedPerRegionCO" class="w-100 bg-white rounded"></canvas>
                        </div>
                        <div class="col-md" style="height:500px;">
                            <canvas id="chartFullyUtilizedPerRegionFO" class="w-100 bg-white rounded"></canvas>
                        </div>
                    </div>
                </div>

                <div id="divSWBreakdown" subpage="divSystemWide" style="display:none;">
                    <div class="row h-100">
                        <div class="col-sm h-100">
                            <div class="bg-white rounded">
                                <h6 class="text-primary mb-3">Breakdown of CO Stores (Demo)</h6>

                                <div class="text-secondary">
                                    <div class="mb-3">
                                        <div class="row mb-2">
                                            <div class="col">GMAN</div>
                                            <div class="col-auto" id="COGMANPercentage">-</div>
                                        </div>
                                        <div class="progress">
                                            <div class="progress-bar" id="pgCOGMANPercentage"></div>
                                        </div>
                                    </div>

                                    <div class="mb-3">
                                        <div class="row mb-2">
                                            <div class="col">GMAS</div>
                                            <div class="col-auto" id="COGMASPercentage">-</div>
                                        </div>
                                        <div class="progress">
                                            <div class="progress-bar" id="pgCOGMASPercentage"></div>
                                        </div>
                                    </div>

                                    <div class="mb-3">
                                        <div class="row mb-2">
                                            <div class="col">North Luzon</div>
                                            <div class="col-auto" id="CONorthLuzonPercentage">-</div>
                                        </div>
                                        <div class="progress">
                                            <div class="progress-bar" id="pgCONorthLuzonPercentage"></div>
                                        </div>
                                    </div>

                                    <div class="mb-3">
                                        <div class="row mb-2">
                                            <div class="col">South Luzon</div>
                                            <div class="col-auto" id="COSouthLuzonPercentage">-</div>
                                        </div>
                                        <div class="progress">
                                            <div class="progress-bar" id="pgCOSouthLuzonPercentage"></div>
                                        </div>
                                    </div>

                                    <div class="mb-3">
                                        <div class="row mb-2">
                                            <div class="col">Visayas</div>
                                            <div class="col-auto" id="COVisayasPercentage">-</div>
                                        </div>
                                        <div class="progress">
                                            <div class="progress-bar" id="pgCOVisayasPercentage"></div>
                                        </div>
                                    </div>

                                    <div class="mb-3">
                                        <div class="row mb-2">
                                            <div class="col">Mindanao</div>
                                            <div class="col-auto" id="COMindanaoPercentage">-</div>
                                        </div>
                                        <div class="progress">
                                            <div class="progress-bar" id="pgCOMindanaoPercentage"></div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm h-100">
                            <div class="bg-white rounded">
                                <h6 class="text-primary mb-3">Breakdown of FO Stores (Demo)</h6>

                                <div class="text-secondary">
                                    <div class="mb-3">
                                        <div class="row mb-2">
                                            <div class="col">Cebu</div>
                                            <div class="col-auto" id="FOCebuPercentage">-</div>
                                        </div>
                                        <div class="progress">
                                            <div class="progress-bar" id="pgFOCebuPercentage"></div>
                                        </div>
                                    </div>

                                    <div class="mb-3">
                                        <div class="row mb-2">
                                            <div class="col">GMAN</div>
                                            <div class="col-auto" id="FOGMANPercentage">-</div>
                                        </div>
                                        <div class="progress">
                                            <div class="progress-bar" id="pgFOGMANPercentage"></div>
                                        </div>
                                    </div>

                                    <div class="mb-3">
                                        <div class="row mb-2">
                                            <div class="col">GMAS</div>
                                            <div class="col-auto" id="FOGMASPercentage">-</div>
                                        </div>
                                        <div class="progress">
                                            <div class="progress-bar" id="pgFOGMASPercentage"></div>
                                        </div>
                                    </div>

                                    <div class="mb-3">
                                        <div class="row mb-2">
                                            <div class="col">North Luzon</div>
                                            <div class="col-auto" id="FONorthLuzonPercentage">-</div>
                                        </div>
                                        <div class="progress">
                                            <div class="progress-bar" id="pgFONorthLuzonPercentage"></div>
                                        </div>
                                    </div>

                                    <div class="mb-3">
                                        <div class="row mb-2">
                                            <div class="col">South Luzon</div>
                                            <div class="col-auto" id="FOSouthLuzonPercentage">-</div>
                                        </div>
                                        <div class="progress">
                                            <div class="progress-bar" id="pgFOSouthLuzonPercentage"></div>
                                        </div>
                                    </div>

                                    <div class="mb-3">
                                        <div class="row mb-2">
                                            <div class="col">Visayas</div>
                                            <div class="col-auto" id="FOVisayasPercentage">-</div>
                                        </div>
                                        <div class="progress">
                                            <div class="progress-bar" id="pgFOVisayasPercentage"></div>
                                        </div>
                                    </div>

                                    <div class="mb-3">
                                        <div class="row mb-2">
                                            <div class="col">Mindanao</div>
                                            <div class="col-auto" id="FOMindanaoPercentage">-</div>
                                        </div>
                                        <div class="progress">
                                            <div class="progress-bar" id="pgFOMindanaoPercentage"></div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div id="divSWFlushingHistory" subpage="divSystemWide" style="display:none;">
                    <table id="tblSWFlushingHistory" data-role="none" class="table table-light table-hover table-striped">
                        <thead>
                            <tr class="text-center">
                                <th class="position-sticky top-0" style="z-index:3;">#</th>
                                <th class="position-sticky top-0" style="z-index:3;">Group</th>
                                <th class="position-sticky top-0" style="z-index:3;">Store Code</th>
                                <th class="position-sticky top-0" style="z-index:3;">Store Name</th>
                                <th class="position-sticky top-0" style="z-index:3;">Unflushed Count</th>
                            </tr>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>
            </div>

            <div id="divMPOSUtilizationReport" subpage="pageMPOSUtil" name="By Stores" class="col-md col h-100" style="display:none;">
                <div class="row m-0 gy-2 gx-3 mb-3 align-items-center">
                    <div class="col-sm-auto">
                        <div class="row">
                            <label for="selMUDisplayReportBy" class="col-2 col-sm-auto col-form-label">Display:</label>
                            <div class="col col-sm-auto">
                                <select data-role="none" class="form-select" id="selMUDisplayReportBy">
                                    <option value="By Group">By Group</option>
                                    <option value="By Ownership">By Ownership</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="col-sm-auto">
                        <div class="row">
                            <label for="selMUPeriod" class="col-2 col-sm-auto col-form-label">Period:</label>
                            <div class="col col-sm-auto">
                                <select data-role="none" class="form-select" id="selMUPeriod">
                                    <option value="Yearly">Yearly</option>
                                    <option value="Monthly" selected>Monthly</option>
                                    <option value="Custom">Custom</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="col-sm-auto">
                        <div class="row">
                            <label for="selMUYear" class="col-2 col-sm-auto col-form-label">Year:</label>
                            <div class="col col-sm-auto">
                                <select data-role="none" class="form-select" id="selMUYear"></select>
                            </div>
                        </div>

                        <div class="row" style="display:none;">
                            <label for="btnMUCustomPeriod" class="col-sm-auto col-form-label">Date:</label>
                            <div class="col-sm-auto">
                                <button data-role="none" id="btnMUCustomPeriod" class="btn btn-light border btn-block">Set Date</button>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-md">
                        <canvas id="chartMUCO" style="min-height: 300px;" class="w-100 bg-white rounded"></canvas>
                    </div>
                    <div class="col-md">
                        <canvas id="chartMUFO" style="min-height: 300px;" class="w-100 bg-white rounded"></canvas>
                    </div>
                </div>

                <div class="table-responsive">
                    <table id="tblMPOSMonthlyUtil" data-role="none" class="table table-light table-striped table-hover">
                        <thead>
                            <tr class="text-center align-top" style="font-size:14px;">
                                <th>Regions</th>
                                <th>No. of Stores</th>
                                <th>Jan</th>
                                <th>Feb</th>
                                <th>Mar</th>
                                <th>Apr</th>
                                <th>May</th>
                                <th>Jun</th>
                                <th>Jul</th>
                                <th>Aug</th>
                                <th>Sep</th>
                                <th>Oct</th>
                                <th>Nov</th>
                                <th>Dec</th>
                            </tr>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>
            </div>

            <div id="divByFranchisedStores" subpage="pageMPOSUtil" name="By Franchised Stores" class="col-md col h-100 bg-white rounded" style="display:none;">
                <div class="row m-0 gy-2 gx-3 mb-3 align-items-center">
                    <div class="col-sm-auto">
                        Date:
                    </div>
                    <div class="col-sm-auto">
                        <button data-role="none" id="btnDatePeriodFORegions" class="btn btn-light border btn-block">Set Date</button>
                    </div>
                    <div class="col-sm-auto">
                        Region:
                    </div>
                    <div class="col-sm-auto">
                        <select data-role="none" id="selMPOSFORegions" class="form-select"></select>
                    </div>
                    <div class="col-sm-auto">
                        <input data-role="none" id="txtSearchMPOSFORegions" type="text" class="form-control" placeholder="Search here" />
                    </div>
                </div>

                <table id="tblMPOSSummaryFOStores" data-role="none" class="table table-light table-hover table-striped">
                    <thead>
                        <tr class="text-center align-top" style="font-size:14px;">
                            <th class="fixed-col position-sticky top-0" style="z-index:3;">Region</th>
                            <th class="fixed-col position-sticky top-0" style="z-index:3;">District / Area Managers</th>
                            <th class="fixed-col position-sticky top-0" style="z-index:3;">Fully Utilized</th>
                            <th class="fixed-col position-sticky top-0" style="z-index:3;">Utilized</th>
                            <th class="fixed-col position-sticky top-0" style="z-index:3;">Not Utilized</th>
                            <th class="fixed-col position-sticky top-0" style="z-index:3;">No. of Stores</th>
                            <th class="fixed-col position-sticky top-0" style="z-index:3;">Percentage</th>
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>
            </div>
        </div>
    </div>
</div>

<div id="popupMPOSDetailedFOStores" data-bs-backdrop="true" class="modal fade" style="display:none;">
    <div class="modal-dialog modal-fullwidth modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Detailed Report</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body p-0">
                <div class="row m-0 gy-2 gx-3 mb-3 align-items-center p-3">
                    <div class="col-sm">
                        Direct Manager: <span class="lblDMName font-weight-bold"></span>
                    </div>
                    <div class="col-sm">
                        <input data-role="none" id="txtSearchMPOSDetailedFORegions" type="text" class="form-control" placeholder="Search here" />
                    </div>
                    @*<div class="col-sm-auto">
                            <button data-role="none" class="btn btn-secondary">Export to CSV</button>
                        </div>*@
                </div>
                <table id="tblMPOSDetailedFOStores" data-role="none" class="table table-light table-hover table-striped">
                    <thead>
                        <tr class="text-center"></tr>
                    </thead>
                    <tbody></tbody>
                </table>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col text-right">
                        <button data-role="none" data-dismiss="modal" class="btn btn-secondary">Close</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popupStoreStatusTagging" data-bs-backdrop="true" class="modal fade" style="display:none;">
    <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title"></h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    <label class="form-label">Current Status:</label>
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
            <div class="modal-footer">
                <div class="row">
                    <div class="col">
                        <button data-role="none" class="btn btn-secondary btn-lg btn-block" data-dismiss="modal">Cancel</button>
                    </div>
                    <div class="col">
                        <button data-role="none" class="btn btn-success btn-lg btn-block" id="btnSaveStoreStatusTagging">Save</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popupFlushing" data-bs-backdrop="true" class="modal fade p-0" style="display:none;">
    <div class="modal-dialog modal-xl modal-dialog-centered modal-fullscreen-lg-down">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title"></h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body p-0">
                <div class="row p-3">
                    <div class="col">
                        <input data-role="none" class="form-control" placeholder="Search here" id="txtSDFlushingSearch" />
                    </div>
                    <div class="col-auto">
                        <button data-role="none" class="btn btn-secondary" id="btnRefreshFlushing">Refresh</button>
                    </div>                    
                </div>
                <table id="tblMPOSFlushing" data-role="none" class="table table-light table-hover table-striped">
                    <thead>
                        <tr class="text-center">
                            <th class="position-sticky top-0" style="z-index:3;">#</th>
                            <th class="position-sticky top-0" style="z-index:3;">Store Name</th>
                            <th class="position-sticky top-0" style="z-index:3;">Flushing Date</th>
                            <th class="position-sticky top-0" style="z-index:3;">Unflushed Count</th>
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col text-right">
                        <button data-role="none" class="btn btn-secondary btn-lg" data-dismiss="modal">Cancel</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popupFlushingHistory" data-bs-backdrop="true" class="modal fade p-0" style="display:none;">
    <div class="modal-dialog modal-dialog-centered modal-fullscreen-lg-down">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Flushing History</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    Store Name: <b id="lblFHStoreName"></b>
                </div>

                <table id="tblMPOSFlushingHistory" data-role="none" class="table table-light table-hover table-striped">
                    <thead>
                        <tr class="text-center">
                            <th class="position-sticky top-0" style="z-index:3;">#</th>
                            <th class="position-sticky top-0" style="z-index:3;">Flushing Date</th>
                            <th class="position-sticky top-0" style="z-index:3;">Count</th>
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>
            </div>
            <div class="modal-footer d-none">
                <div class="row">
                    <div class="col text-right">
                        <button data-role="none" class="btn btn-secondary btn-lg" data-dismiss="modal">Cancel</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popupFlushingBreakdown" data-bs-backdrop="true" class="modal fade p-0" style="display:none;">
    <div class="modal-dialog modal-dialog-centered modal-fullscreen-lg-down">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Breakdown</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    Flushing Date: <b id="lblFBDate"></b>
                </div>

                <table id="tblMPOSFlushingBreakdown" data-role="none" class="table table-light table-hover table-striped">
                    <thead>
                        <tr class="text-center">
                            <th class="position-sticky top-0" style="z-index:3;">#</th>
                            <th class="position-sticky top-0" style="z-index:3;">Transaction Date</th>
                            <th class="position-sticky top-0" style="z-index:3;">Count</th>
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>
            </div>
            <div class="modal-footer d-none">
                <div class="row">
                    <div class="col text-right">
                        <button data-role="none" class="btn btn-secondary btn-lg" data-dismiss="modal">Cancel</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popupStoreStatus" data-bs-backdrop="true" class="modal fade p-0" style="display:none;">
    <div class="modal-dialog modal-xl modal-dialog-centered modal-fullscreen-lg-down">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title"></h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body p-0">
                <div class="p-3 text-right">
                    <div class="row gy-2 gx-3 align-items-center">
                        <div class="col-auto">
                            Ownership:
                        </div>
                        <div class="col-auto">
                            <select data-role="none" class="form-select" id="selStoreOwnership">
                                <option value="All">All</option>
                                <option value="CO">Company-owned</option>
                                <option value="FO">Franchise</option>
                            </select>
                        </div>
                        <div class="col-auto">
                            Status:
                        </div>
                        <div class="col-auto">
                            <select data-role="none" class="form-select" id="selStoreStatus">
                                <option value="All">All</option>
                                <option value="Utilized">Utilized</option>
                                <option value="Not Utilized">Not Utilized</option>
                                <option value="Closed">Closed</option>
                            </select>
                        </div>
                        <div class="col">
                            <input data-role="none" type="text" class="form-control" id="txtStoreStatusSearch" placeholder="Search here" />
                        </div>
                        <div class="col-auto">
                            <button data-role="none" class="btn btn-secondary" id="btnDownloadStoreStatus">Download</button>
                        </div>
                    </div>
                </div>
                <table id="tblMPOSStoreStatus" data-role="none" class="table table-light table-hover table-striped">
                    <thead>
                        <tr class="text-center">
                            <th class="position-sticky top-0" style="z-index:3;">#</th>
                            <th class="position-sticky top-0" style="z-index:3;">Region</th>
                            <th class="position-sticky top-0" style="z-index:3;">Store Code</th>
                            <th class="position-sticky top-0" style="z-index:3;">Store Name</th>
                            <th class="position-sticky top-0" style="z-index:3;">Status</th>
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col text-right">
                        <button data-role="none" class="btn btn-secondary btn-lg" data-dismiss="modal">Cancel</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popupFewestTrans" data-bs-backdrop="true" class="modal fade p-0" style="display:none;">
    <div class="modal-dialog modal-xl modal-dialog-centered modal-fullscreen-lg-down">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Stores with 1 transaction</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body p-0">
                <div class="row gy-2 gx-3 p-3 align-items-center">
                    <div class="col-auto">
                        Ownership:
                    </div>
                    <div class="col-auto">
                        <select data-role="none" class="form-select" id="selFewestStoreOwnership">
                            <option value="All">All</option>
                            <option value="Company-Owned">Company-owned</option>
                            <option value="Franchise">Franchise</option>
                        </select>
                    </div>
                    <div class="col">
                        <input data-role="none" type="text" class="form-control" id="txtFewestStoreStatusSearch" placeholder="Search here" />
                    </div>
                    <div class="col-auto">
                        <button data-role="none" class="btn btn-secondary" id="btnDownloadFewestTrans">Download</button>
                    </div>
                </div>

                <table id="tblMPOSFewestTrans" data-role="none" class="table table-light table-hover table-striped">
                    <thead>
                        <tr class="text-center">
                            <th class="position-sticky top-0" style="z-index:3;">#</th>
                            <th class="position-sticky top-0" style="z-index:3;">Region</th>
                            <th class="position-sticky top-0" style="z-index:3;">Store Code</th>
                            <th class="position-sticky top-0" style="z-index:3;">Store Name</th>
                            <th class="position-sticky top-0" style="z-index:3;">Status</th>
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col text-right">
                        <button data-role="none" class="btn btn-secondary btn-lg" data-dismiss="modal">Cancel</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>