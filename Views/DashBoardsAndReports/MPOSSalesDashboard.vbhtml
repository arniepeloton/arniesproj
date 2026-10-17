@Code
    ViewData("Title") = "Dashboards and Reports"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code

@Section head
    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
                require(['/Scripts/dom-to-image.min.js'], function () {
                require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/MPOSSalesDashboard.js?v=' + assetversion)]);
                })
            })
        })
    </script>

    <style>
        .mposdashboard h1 {
            font-size: clamp(3rem, 6vw, 4rem);
        }

        .mposdashboard h2 {
            font-size: clamp(3rem, 6vw, 3.5rem);
        }

        .mposdashboard h3 {
            font-size: clamp(2rem, 6vw, 3rem);
        }

        .mposdashboard h4 {
            font-size: clamp(1.5rem, 6vw, 2rem);
        }

        .mposdashboard h5 {
            font-size: clamp(1.25rem, 6vw, 1rem);
        }

        .mposdashboard h6 {
            font-size: clamp(1rem, 6vw, 0.5rem);
        }
    </style>
End Section

<div class="d-flex flex-column overflow-hidden h-100 mposdashboard">
    <div class="flex-shrink-1 bg-white shadow-sm rounded-1 mb-3 p-3">
        <div class="row m-0 gx-3 align-items-center">
            <div class="col-auto">
                <div class="btn-group">
                    <input data-role="none" type="radio" class="btn-check" name="btnMPOSSalesDashboard" id="btnMPOSSalesDashboardSystemWide" value="systemwide" autocomplete="off" checked>
                    <label class="btn btn-outline-secondary border-0 rounded" for="btnMPOSSalesDashboardSystemWide">System Wide</label>

                    <input data-role="none" type="radio" class="btn-check" name="btnMPOSSalesDashboard" id="btnMPOSSalesDashboardDetailedReport" value="detailedreport" autocomplete="off">
                    <label class="btn btn-outline-secondary border-0 rounded" for="btnMPOSSalesDashboardDetailedReport">Detailed Report</label>
                </div>
            </div>
            <div class="col text-right">
                <button data-role="none" class="btn btn-light border text-dark" id="btnSDRefreshSystemWide"><span class="bi-arrow-clockwise"></span></button>
                <button data-role="none" class="btn btn-light border text-dark" id="btnSDRefreshDetailedReport" style="display:none;"><span class="bi-arrow-clockwise"></span></button>
            </div>
        </div>        
    </div>

    <div class="d-flex flex-fill align-items-start flex-column overflow-auto w-100 rounded-1">
        <div class="row g-0 justify-content-center w-100 h-100">
            <div id="divSDSystemWide" subpage="pageMPOSSalesDashboard" class="col-md col h-100">
                <div class="w-100 rounded-1 mb-3">
                    <div class="row m-0 gx-3 align-items-center">
                        <div class="col-auto colDate">
                            <div class="row">
                                <label for="btnSystemWideSDDate" class="col-auto col-form-label">Date:</label>
                                <div class="col">
                                    <button data-role="none" class="btn btn-light border" id="btnSystemWideSDDate">Set Date</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="row g-0 gap-3 mb-3 justify-content-center text-secondary">
                    <div class="col-md p-3 bg-white shadow-sm rounded-1" id="divSDCO">
                        <div class="row mb-3">
                            <div class="col-auto align-content-center">
                                <div class="text-primary rounded-circle d-flex justify-content-center align-items-center" style="height: 75px; width: 75px; background-color: #dceaff;">
                                    <span class="bi-buildings" style="font-size: xx-large"></span>
                                </div>
                            </div>
                            <div class="col col-sm-6 text-primary">
                                <div class="row g-0">
                                    <div class="col-12 align-content-center"><h5 class="mb-0">Company-Owned</h5></div>
                                    <div class="col-12"><h2 class="font-weight-bold text-center mb-0" name="lblSDPercentage">0 / 0</h2></div>
                                </div>
                            </div>
                            <div class="col-sm mt-3 mt-sm-0 align-content-center">
                                <canvas id="chartSDCOPercentage" class="w-100 bg-white rounded" style="height:100px;"></canvas>
                            </div>
                        </div>

                        <div class="table-responsive">
                            <table data-role="none" class="table table-borderless table-striped">
                                <thead>
                                    <tr class="text-center text-secondary align-top">
                                        <th></th>
                                        <th data-toggle="tooltip" title="The total number of stores and their devices are operational">No. of Stores</th>
                                        <th>Open</th>
                                        <th>Closed</th>
                                        <th>Completed</th>
                                        <th>Not Sending</th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr class="text-center">
                                        <td class="rounded-top-bottom-left-1">MPOS</td>
                                        <td name="lblNoOfStoresMPOS">0</td>
                                        <td name="lblOverallMPOS">0</td>
                                        <td name="lblExemptedMPOS">0</td>
                                        <td name="lblCompletedMPOS">0</td>
                                        <td name="lblNotSendingMPOS">0</td>
                                        <td name="lblPercentageMPOS" class="rounded-top-bottom-right-1 font-weight-bold">0%</td>
                                    </tr>
                                    <tr class="text-center">
                                        <td class="rounded-top-bottom-left-1">3rd Party</td>
                                        <td name="lblNoOfStores3PP">0</td>
                                        <td name="lblOverall3PP">0</td>
                                        <td name="lblExempted3PP">0</td>
                                        <td name="lblCompleted3PP">0</td>
                                        <td name="lblNotSending3PP">0</td>
                                        <td name="lblPercentage3PP" class="rounded-top-bottom-right-1 font-weight-bold">0%</td>
                                    </tr>
                                </tbody>
                                <tfoot>
                                    <tr class="text-center font-weight-bold">
                                        <td>TOTAL</td>
                                        <td name="lblTotalNofStores"></td>
                                        <td name="lblTotalOverall"></td>
                                        <td name="lblTotalExempted"></td>
                                        <td name="lblTotalCompleted"></td>
                                        <td name="lblTotalNotSending"></td>
                                        <td></td>
                                    </tr>
                                </tfoot>
                            </table>
                        </div>
                    </div>
                    <div class="col-md p-3 bg-white shadow-sm rounded-1" id="divSDFO">
                        <div class="row mb-3">
                            <div class="col-auto align-content-center">
                                <div class="text-success rounded-circle d-flex justify-content-center align-items-center" style="height: 75px; width: 75px; background-color: #dcffe3;">
                                    <span class="bi-shop" style="font-size: xx-large"></span>
                                </div>
                            </div>
                            <div class="col col-sm-6 text-success">
                                <div class="row g-0">
                                    <div class="col-12 align-content-center"><h5 class="mb-0">Franchise</h5></div>
                                    <div class="col-12"><h2 class="font-weight-bold text-center mb-0" name="lblSDPercentage">0 / 0</h2></div>
                                </div>
                            </div>
                            <div class="col-sm mt-3 mt-sm-0 align-content-center" style="font-size:1.25rem;">
                                <canvas id="chartSDFOPercentage" class="w-100 bg-white rounded" style="height:100px;"></canvas>
                            </div>
                        </div>

                        <div class="table-responsive">
                            <table data-role="none" class="table table-borderless table-striped">
                                <thead>
                                    <tr class="text-center text-secondary align-top">
                                        <th></th>
                                        <th data-toggle="tooltip" title="The total number of stores and their devices are operational">No. of Stores</th>
                                        <th>Open</th>
                                        <th>Closed</th>
                                        <th>Completed</th>
                                        <th>Not Sending</th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr class="text-center">
                                        <td class="rounded-top-bottom-left-1">MPOS</td>
                                        <td name="lblNoOfStoresMPOS">0</td>
                                        <td name="lblOverallMPOS">0</td>
                                        <td name="lblExemptedMPOS">0</td>
                                        <td name="lblCompletedMPOS">0</td>
                                        <td name="lblNotSendingMPOS">0</td>
                                        <td name="lblPercentageMPOS" class="rounded-top-bottom-right-1 font-weight-bold">0%</td>
                                    </tr>
                                    <tr class="text-center">
                                        <td class="rounded-top-bottom-left-1">3rd Party</td>
                                        <td name="lblNoOfStores3PP">0</td>
                                        <td name="lblOverall3PP">0</td>
                                        <td name="lblExempted3PP">0</td>
                                        <td name="lblCompleted3PP">0</td>
                                        <td name="lblNotSending3PP">0</td>
                                        <td name="lblPercentage3PP" class="rounded-top-bottom-right-1 font-weight-bold">0%</td>
                                    </tr>
                                </tbody>
                                <tfoot>
                                    <tr class="text-center font-weight-bold">
                                        <td>TOTAL</td>
                                        <td name="lblTotalNofStores"></td>
                                        <td name="lblTotalOverall"></td>
                                        <td name="lblTotalExempted"></td>
                                        <td name="lblTotalCompleted"></td>
                                        <td name="lblTotalNotSending"></td>
                                        <td></td>
                                    </tr>
                                </tfoot>
                            </table>
                        </div>
                    </div>
                </div>

                <div class="p-3 bg-white shadow-sm rounded-1">
                    <div class="row m-0 gy-2 gx-3 mb-3 align-items-center">
                        <div class="col-md">
                            <h5>System Wide Details</h5>
                        </div>
                        <div class="col-md-auto">
                            <div class="row">
                                <label for="selSDDetailedOwnership" class="col-auto col-md-auto col-form-label">Ownership:</label>
                                <div class="col col-md-auto">
                                    <select data-role="none" class="form-select" id="selSDDetailedOwnership">
                                        <option value="All">All</option>
                                        <option value="Company-Owned">Company-Owned</option>
                                        <option value="Franchise">Franchise</option>
                                    </select>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-auto">
                            <div class="row">
                                <label for="selSDGroup" class="col-auto col-md-auto col-form-label">Group:</label>
                                <div class="col col-md-auto">
                                    <select data-role="none" class="form-select" id="selSDGroup">
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
                                        <option value="Third Party POS">Third Party POS</option>
                                    </select>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-auto d-none">
                            <div class="row">
                                <label for="selSDDetailedStatus" class="col-auto col-md-auto col-form-label">Status:</label>
                                <div class="col col-md-auto">
                                    <select data-role="none" class="form-select" id="selSDDetailedStatus">
                                        <option value="All">All</option>
                                        <option value="Utilized">Utilized</option>
                                        <option value="Not Utilized">Not Utilized</option>
                                        <option value="Closed">Closed</option>
                                    </select>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-auto">
                            <input data-role="none" class="form-control" placeholder="Search here" id="txtSDSearchDetails" />
                        </div>
                    </div>

                    <ul class="nav nav-tabs mb-3">
                        <li class="nav-item" role="presentation">
                            <button data-role="none" class="nav-link active" id="" data-toggle="tab" data-target="#tbSDCompleted"><span class="bi-check-circle-fill text-success"></span> Completed</button>
                        </li>
                        <li class="nav-item" role="presentation">
                            <button data-role="none" class="nav-link" id="" data-toggle="tab" data-target="#tbSDNotSending"><span class="bi-exclamation-triangle-fill text-warning"></span> Not Sending</button>
                        </li>
                        <li class="nav-item" role="presentation">
                            <button data-role="none" class="nav-link" id="" data-toggle="tab" data-target="#tbSDExempted"><span class="bi-x-circle-fill text-danger"></span> Exempted</button>
                        </li>
                    </ul>

                    <div class="tab-content">
                        <div class="tab-pane fade show active" id="tbSDCompleted">
                            <div class="table-responsive">
                                <table id="tblSDCompleted" data-role="none" class="table table-light table-striped table-hover">
                                    <thead>
                                        <tr class="text-center align-top">
                                            <th class="fixed-col position-sticky top-0" style="z-index:3;">#</th>
                                            <th class="fixed-col position-sticky top-0" style="z-index:3;">Group</th>
                                            <th class="fixed-col position-sticky top-0" style="z-index:3;">Store Code</th>
                                            <th class="fixed-col position-sticky top-0" style="z-index:3;">Store Name</th>
                                            @*<th class="fixed-col position-sticky top-0" style="z-index:3;">Status</th>*@
                                            <th class="fixed-col position-sticky top-0" style="z-index:3;">Gross Sales</th>
                                        </tr>
                                    </thead>
                                    <tbody></tbody>
                                </table>
                            </div>
                        </div>

                        <div class="tab-pane fade" id="tbSDNotSending">
                            <div class="table-responsive">
                                <table id="tblSDNotSending" data-role="none" class="table table-light table-striped table-hover">
                                    <thead>
                                        <tr class="text-center align-top">
                                            <th class="fixed-col position-sticky top-0" style="z-index:3;">#</th>
                                            <th class="fixed-col position-sticky top-0" style="z-index:3;">Group</th>
                                            <th class="fixed-col position-sticky top-0" style="z-index:3;">Store Code</th>
                                            <th class="fixed-col position-sticky top-0" style="z-index:3;">Store Name</th>
                                            @*<th class="fixed-col position-sticky top-0" style="z-index:3;">Status</th>*@
                                            <th class="fixed-col position-sticky top-0" style="z-index:3;">Gross Sales</th>
                                        </tr>
                                    </thead>
                                    <tbody></tbody>
                                </table>
                            </div>
                        </div>

                        <div class="tab-pane fade" id="tbSDExempted">
                            <div class="table-responsive">
                                <table id="tblSDExempted" data-role="none" class="table table-light table-striped table-hover">
                                    <thead>
                                        <tr class="text-center align-top">
                                            <th class="fixed-col position-sticky top-0" style="z-index:3;">#</th>
                                            <th class="fixed-col position-sticky top-0" style="z-index:3;">Group</th>
                                            <th class="fixed-col position-sticky top-0" style="z-index:3;">Store Code</th>
                                            <th class="fixed-col position-sticky top-0" style="z-index:3;">Store Name</th>
                                            @*<th class="fixed-col position-sticky top-0" style="z-index:3;">Status</th>*@
                                            <th class="fixed-col position-sticky top-0" style="z-index:3;">Gross Sales</th>
                                        </tr>
                                    </thead>
                                    <tbody></tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            
            <div id="divSDDetails" subpage="pageMPOSSalesDashboard" class="col-md col h-100" style="display:none;">
                <div id="templateDetailedReport" class="d-none col-md p-3 bg-white shadow-sm rounded-1">
                    <div class="w-100 text-center font-weight-bold text-primary">
                        <h5 class="mb-0" name="lblSDTitle"></h5>
                    </div>

                    <div class="row mt-3">
                        @*<div class="col-12">
                    Actual Sales
                </div>*@
                        <div class="col-12 text-center text-dark font-weight-bold">
                            <h4 name="lblSDActualSales">0.00</h4>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md">
                            <div class="row mt-3">
                                <div class="col">
                                    LY Same Date <span name="lblSD"></span>
                                    <div class="text-left font-weight-bold" name="lblSDLYSDS">0.00</div>
                                </div>
                                <div class="col-auto text-right">
                                    Growth
                                    <div class="text-right font-weight-bold" name="lblSDLYSDSGrowth">0%</div>
                                </div>
                            </div>

                            <hr />

                            <div class="row mt-3">
                                <div class="col">
                                    Avg. Daily <span name="lblSD"></span>
                                </div>
                                <div class="col text-right font-weight-bold" name="lblSDAvgDailySales">
                                    0.00
                                </div>
                            </div>

                            <div class="row mt-3">
                                <div class="col">
                                    No. of Stores
                                </div>
                                <div class="col text-right font-weight-bold" name="lblSDNoOfStores">
                                    0
                                </div>
                            </div>

                            @*<hr />*@

                            <div class="row mt-3 d-none">
                                <div class="col">
                                    Same Store <span name="lblSD"></span>
                                    <div class="text-left font-weight-bold" name="lblSDSSS">0.00</div>
                                </div>
                                <div class="col-auto text-right">
                                    Growth
                                    <div class="text-right font-weight-bold" name="lblSDSSSGrowth">0%</div>
                                </div>
                            </div>
                        </div>

                        <div class="col-md-auto mt-3 mt-md-0">
                            <div class="d-flex border-left border-top h-100"></div>
                        </div>

                        <div class="col-md">
                            <div class="text-center mt-3">
                                Breakdown
                            </div>

                            <div class="row mt-3">
                                <div class="col">
                                    Walk-in
                                </div>
                                <div class="col text-right font-weight-bold" name="lblSDDineIn">
                                    0.00
                                </div>
                            </div>

                            @*<div class="row mt-3">
                                <div class="col">
                                    Take-out
                                </div>
                                <div class="col text-right font-weight-bold" name="lblSDTakeOut">
                                    0.00
                                </div>
                            </div>*@

                            <div class="row mt-3">
                                <div class="col">
                                    Delivery
                                </div>
                                <div class="col text-right font-weight-bold" name="lblSDDelivery">
                                    0.00
                                </div>
                            </div>

                            @*<div class="row mt-3">
                                <div class="col">
                                    Drive-Thru
                                </div>
                                <div class="col text-right font-weight-bold" name="lblSDDriveThru">
                                    0.00
                                </div>
                            </div>*@
                        </div>
                    </div>
                </div>

                <div class="w-100 rounded-1 mb-3">
                    <div class="row m-0 gx-3 align-items-center">
                        <div class="col-auto colDate">
                            <div class="row">
                                <label for="btnSystemWideSDDate" class="col-auto col-form-label">POS Vendor:</label>
                                <div class="col">
                                    <select data-role="none" class="form-select" id="selSDPOSVendor">
                                        <option value="All">All</option>
                                        <option value="MPOS">MPOS</option>
                                        <option value="3PP">3rd Party POS</option>
                                    </select>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="mb-3">
                    <div class="row g-0 gap-3 mb-3 align-items-center">
                        <div class="col text-dark">
                            <h5>Yesterday</h5>
                        </div>
                        <div class="col text-secondary text-right" name="lblDateYesterday">

                        </div>
                    </div>
                    <div id="DR_Yesterday" class="row g-0 gap-3 text-secondary"></div>
                </div>

                <div class="mb-3">
                    <div class="row g-0 gap-3 mb-3 align-items-center">
                        <div class="col text-dark">
                            <h5>Month to Date</h5>
                        </div>
                        <div class="col text-secondary text-right" name="lblDateMTD">

                        </div>
                    </div>
                    <div id="DR_MTD" class="row g-0 gap-3 text-secondary"></div>
                </div>

                <div class="mb-3">
                    <div class="row g-0 gap-3 mb-3 align-items-center">
                        <div class="col text-dark">
                            <h5>Year to Date</h5>
                        </div>
                        <div class="col text-secondary text-right" name="lblDateYTD">

                        </div>
                    </div>
                    <div id="DR_YTD" class="row g-0 gap-3 text-secondary"></div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="" data-bs-backdrop="true" class="modal fade" style="display:none;">
    <div class="modal-dialog modal-fullwidth modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Detailed Report</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body p-0">
                
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