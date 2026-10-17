@Code
    ViewData("Title") = "Mobiz Home"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code
@Section head
    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
                require([GetPathFormat('/Scripts/sources/People/PeopleMenu.js?crc=' + assetversion)]);
                require([GetPathFormat('/Scripts/sources/People/PeopleDashBoard.js?crc=' + assetversion)]);
            })
        })
    </script>
    <style type="text/css">
        .ui-page-active, body {
            outline: none;
            background-color: #f8f9fa !important;
        }

        .btnmcrmenu {
            height: 100%;
            color: var(--bs-gray-dark);
        }

        #divmcrmenu > div > div {
            display: none;
        }

        .btnmcrmenu:hover {
            cursor: pointer;
            box-shadow: 0 0.5rem 1rem rgb(0 0 0 / 15%) !important;
            color: #0d6efd;
        }

        .btnmcrmenu > img {
            height: 80px;
            width: 80px;
        }
        .module-buttons {
            font-size: 80px;
        }

        .messages {
            cursor: pointer;
            color: var(--bs-dark);
        }

        .messages .col-auto,
        .messages .text-nowrap {
            color: var(--bs-gray);
        }

        .messages.active,
        .messages.active .col-auto,
        .messages.active .text-nowrap {
            color: var(--bs-primary);
        }
    </style>
End Section
<div id="divmcrloading" custom-data-role="page" style="width:100%; text-align:center;">
    <div class="d-flex position-absolute w-100 h-100 justify-content-center align-items-center flex-column">
        <div class="spinner-border text-primary" role="status"></div>
    </div>
</div>

<div id="pageHome" custom-data-role="page" style="display:none;">
    <nav class="bg-primary text-white mb-3 sticky-top">
        <div class="container-fluid">
            <div class="row gx-0 align-items-center">
                <div class="col text-truncate">
                    <h5 class="m-0">Mobiz Complete Retail <span class="lblVersion d-none" style="font-size:12px;">v2.000.000</span></h5>
                </div>
                <div class="col-auto d-none">
                    <button data-role="none" id="btnUserManual" class="btn btn-primary btn-lg text-white">
                        <span class="bi-question-circle-fill"></span>
                    </button>
                </div>
                <div name="divButMessages" class="col-auto">
                    <button data-role="none" id="btnMessages" class="btn btn-primary btn-lg text-white">
                        <span class="bi-envelope-fill"></span>
                    </button>
                </div>
                <div name="divButOnline" class="col-auto" style="display:none">
                    <button data-role="none" id="btnOnline" class="btn btn-primary btn-lg text-white">
                        <span class="bi-cloud-fill"></span>
                    </button>
                </div>
                <div class="col-auto" style="display:none">
                    <button data-role="none" id="btnAnnouncement" class="btn btn-primary btn-lg text-white">
                        <span class="bi-megaphone-fill"></span>
                    </button>
                </div>
                <div class="col-auto">
                    <div class="dropdown">
                        <button type="button" data-role="none" class="btn btn-primary btn-lg text-white dropdown-toggle dropdown-toggle-split" data-toggle="dropdown" aria-expanded="false">
                            @*<span class="bi-caret-down-fill"></span>*@
                        </button>
                        <ul class="dropdown-menu dropdown-menu-right">
                            <li class="p-3">
                                <span id="lblName"></span>
                            </li>
                            @*<li><a class="dropdown-item" id="popPeopleMenuPeopleInfoSummary" href="#"><span class="bi-person-fill"></span> View Profile</a></li>
                                <li><a class="dropdown-item" id="popPeopleMenuToggleFullscreen" href="#"><span class="bi-fullscreen"></span> Toggle Fullscreen</a></li>*@
                            <li><a class="dropdown-item" id="btnSignOut" href="#"><span class="bi-box-arrow-left"></span> Logout</a></li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
    </nav>    

    <div class="container-fluid">
        <div id="divemploymentinvitation">

        </div>
        <div class="row mb-3" id="divCompany" style="display:none;">
            <div class="col-auto col-form-label">Select Company:</div>
            <div class="col-12 col-sm-auto">
                <select data-role="none" id="selPeopleOrg" class="form-select form-select-lg"></select>
            </div>
        </div>

        <div id="devicedetails" class="bg-white border p-3 rounded mb-3" style="display:none;">
            <table class="mb-3">
                <thead>
                    <tr>
                        <th>Device Used In</th>
                        <th>Posting Date</th>
                    </tr>
                </thead>
                <tbody id="fowstdevice"></tbody>
            </table>
            <div style="font-size:14px;">
                <div><b>User Role:</b> <span id="lblUserRole"></span></div>
                <div><b>Database Creation Start:</b> <span id="lblDBStart"></span></div>
                <div><b>Database Creation End:</b> <span id="lblDBEnd"></span></div>
                <div><b>Last Database Sync:</b> <span id="lblDBSync"></span></div>
            </div>
        </div>

        <div id="divnotamember" subpage="divmcr" style="width:100%; text-align:center; display:none;">
            You should be a member of an organization/company in order to use this program.
        </div>

        <div id="divmcrmenu" subpage="divmcr">
            <div class="row row-cols-2 row-cols-sm-2 row-cols-md-3 row-cols-lg-4 row-cols-xl-5 row-cols-xxl-6 text-center">
                <div class="col mb-3" id="btnPOS">
                    <div class="bg-white border p-3 rounded btnmcrmenu">
                        <img src="/Content/images/icons/1.png" class="rounded mb-3">
                        <br />
                        Point of Sales (POS)
                    </div>
                </div>
                <div class="col mb-3" id="btnPriceChecker">
                    <div class="bg-white border p-3 rounded btnmcrmenu">
                        <img src="/Content/images/icons/PriceCheckerIcon.png" class="rounded mb-3">
                        <br />
                        Price Checker
                    </div>
                </div>
                <div class="col mb-3" id="btnPO">
                    <div class="bg-white border p-3 rounded btnmcrmenu">
                        <img src="/Content/images/icons/1.png" class="rounded mb-3">
                        <br />
                        Purchase Order / Requisition
                    </div>
                </div>
                <div class="col mb-3" id="btnPickProducts">
                    <div class="bg-white border p-3 rounded btnmcrmenu">
                        <img src="/Content/images/icons/FC4.png" class="rounded mb-3">
                        <br />
                        Pick Products
                    </div>
                </div>
                <div class="col mb-3" id="btnWarehouseReleasingTracker">
                    <div class="bg-white border p-3 rounded btnmcrmenu">
                        <img src="/Content/images/icons/FC5.png" class="rounded mb-3">
                        <br />
                        Warehouse Releasing Tracker
                    </div>
                </div>
                <div class="col mb-3" id="btnInvoicePickedItems">
                    <div class="bg-white border p-3 rounded btnmcrmenu">
                        <img src="/Content/images/icons/FC5.png" class="rounded mb-3">
                        <br />
                        Invoice Picked Items
                    </div>
                </div>
                <div class="col mb-3" id="btnSalesReturns">
                    <div class="bg-white border p-3 rounded btnmcrmenu">
                        <img src="/Content/images/icons/2.png" class="rounded mb-3">
                        <br />
                        Sales Returns
                    </div>
                </div>
                @*<div class="col mb-3" id="butWebPos" style="display:none;">
                        <a class="bg-white border p-3 rounded btnmcrmenu" style="display:block; text-decoration:none;">
                            <img src="/Content/images/POS.jpg" class="rounded mb-3">
                            <div class="bi bi-pc-display-horizontal rounded mb-3 module-buttons"></div>
                            Web POS
                        </a>
                    </div>*@
                <div class="col mb-3" id="btnStockReceiving">
                    <div class="bg-white border p-3 rounded btnmcrmenu">
                        <img src="/Content/images/icons/3.1.png" class="rounded mb-3">
                        <div class="bi bi-arrow-down-square rounded mb-3 module-buttons"></div>
                        <br />
                        Stock Receiving
                    </div>
                </div>
                <div class="col mb-3" id="btnStockTransfers">
                    <div class="bg-white border p-3 rounded btnmcrmenu">
                        <img src="/Content/images/icons/3.2.png" class="rounded mb-3">
                        <div class="bi bi-arrow-up-square rounded mb-3 module-buttons"></div>
                        <br />
                        Stock Transfers
                    </div>
                </div>
                <div class="col mb-3" id="btnErpInventoryHandling" style="display:none;">
                    <div class="bg-white border p-3 rounded btnmcrmenu">
                        <img src="/Content/images/inventorycontrol.png" class="rounded mb-3">
                        <div class="bi bi-boxes rounded mb-3 module-buttons"></div>
                        Inventory Handling & Controls
                    </div>
                </div>
                <div class="col mb-3" id="btnInventoryCount">
                    <div class="bg-white border p-3 rounded btnmcrmenu">
                        <img src="/Content/images/icons/4.png" class="rounded mb-3">
                        <br />
                        Inventory Count
                    </div>
                </div>
                <div class="col mb-3" id="btnExpenseTracking">
                    <div class="bg-white border p-3 rounded btnmcrmenu">
                        <img src="/Content/images/icons/Expense.png" class="rounded mb-3">
                        <br />
                        Route and Expense Tracking
                    </div>
                </div>
                <div class="col mb-3" id="btnTickets" style="display:none">
                    <div class="bg-white border p-3 rounded btnmcrmenu">
                        <img src="/Content/images/icons/Expense.png" class="rounded mb-3">
                        <br />
                        Tickets
                    </div>
                </div>
                <div class="col mb-3" id="btnProductMaker">
                    <div class="bg-white border p-3 rounded btnmcrmenu">
                        <img src="/Content/images/icons/7.png" class="rounded mb-3">
                        <br />
                        Product Maker
                    </div>
                </div>
                <div class="col mb-3" id="btnShelfBinLabelling">
                    <div class="bg-white border p-3 rounded btnmcrmenu">
                        <img src="/Content/images/icons/8.png" class="rounded mb-3">
                        <div class="bi bi-qr-code rounded mb-3 module-buttons"></div>
                        <br />
                        Shelf And Bin Labelling
                    </div>
                </div>
                <div class="col mb-3" id="btnRequestApprovals">
                    <div class="bg-white border p-3 rounded btnmcrmenu">
                        <img src="/Content/images/icons/Approvals.png" class="rounded mb-3">
                        <br />
                        Request and Approvals
                    </div>
                </div>
                <div class="col mb-3" id="btnCashHandling">
                    <div class="bg-white border p-3 rounded btnmcrmenu">
                        <img src="/Content/images/icons/12.png" class="rounded mb-3">
                        <br />
                        Cash Handling
                    </div>
                </div>
                <div class="col mb-3" id="btnDashboardAndReports">
                    <div class="bg-white border p-3 rounded btnmcrmenu">
                        <img src="/Content/images/icons/5.png" class="rounded mb-3">
                        <div class="bi bi-graph-up-arrow rounded mb-3 module-buttons"></div>
                        <br />
                        Dashboards and Reports
                    </div>
                </div>
                <div class="col mb-3 position-relative" id="btnIssueTickets">
                    <span id="lblCountNewReports" class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger" style="right:0;"></span>
                    <div class="bg-white border p-3 rounded btnmcrmenu">
                        <div class="bi bi-ticket-detailed rounded mb-3 module-buttons"></div>
                        Issue Tickets
                    </div>
                </div>
                <div class="col mb-3" id="btnPrivateMessages">
                    <div class="bg-white border p-3 rounded btnmcrmenu">
                        <div class="bi bi-megaphone rounded mb-3 module-buttons"></div>
                        In-app Announcements
                    </div>
                </div>
                <div class="col mb-3" id="btnTroubleshooting">
                    <div class="bg-white border p-3 rounded btnmcrmenu">
                        <img src="/Content/images/icons/9.png" class="rounded mb-3">
                        <br />
                        Troubleshooting and Maintenance Tools
                    </div>
                </div>
                <div class="col mb-3" id="btnSystemAdmin">
                    <div class="bg-white border p-3 rounded btnmcrmenu">
                        <img src="/Content/images/icons/11.png" class="rounded mb-3">
                        <div class="bi bi-gear rounded mb-3 module-buttons"></div>
                        System Settings
                    </div>
                </div>
                <div class="col mb-3" id="btnFranchiseeUpdate">
                    <div class="bg-white border p-3 rounded btnmcrmenu">
                        <div class="bi bi-arrow-repeat rounded mb-3 module-buttons"></div>
                        Get Latest Franchisor Data Updates
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="pageMessages" custom-data-role="page" style="display:none;">
    <nav class="bg-primary text-white mb-3 sticky-top">
        <div class="container-fluid">
            <table style="width:100%;">
                <tr>
                    <td style="width:50px;">
                        <button data-role="none" id="btnPeopleMsgT2BackToHome" class="btn btn-primary btn-lg btn-block text-white">
                            <span class="bi-chevron-left"></span>
                        </button>
                    </td>
                    <td><div><h5 class="m-0">Messages</h5></div></td>
                </tr>
            </table>
        </div>
    </nav>

    <div class="container-fluid">
        <div id="divmessagesloading" subpage="divmessages">
            <div class="d-flex position-absolute w-100 h-100 justify-content-center align-items-center">
                <div class="spinner-border text-primary" role="status"></div>
                <br />
                Please wait ...
            </div>            
        </div>

        <div id="divMessageContainer" subpage="divmessages" style="display:none">

        </div>
    </div>
</div>

<div id="popupEmploymentInvitation" class="modal fade">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Employement Invitation</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <input type="text" data-role="none" style="display:none;" readonly id="txtPeopleMsgT1ID" />
                <h5 id="lblPeopleMsgT1Subject"></h5>

                <div class="mb-3">From: <b id="lblPeopleMsgT1From"></b></div>

                <div class="mb-3">Date/Time: <b id="lblPeopleMsgT1DateTime"></b></div>

                <div id="lblPeopleMsgT1"></div>
            </div>
            <div class="modal-footer">
                <button data-role="none" id="btnPeopleMsgT1Confirm" class="btn btn-success btn-block btn-lg">Accept</button>
                <button data-role="none" id="btnPeopleMsgT1Deny" class="btn btn-danger btn-block btn-lg">Decline</button>
                <button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-block btn-lg">Cancel</button>
            </div>
        </div>
    </div>
</div>

<div id="pagePopUpMenuTroubleShootingTools" custom-data-role="page" style="display:none;">
    <nav class="bg-primary text-white mb-3 sticky-top">
        <div class="container-fluid">
            <table style="width:100%;">
                <tr>
                    <td style="width:50px;">
                        <button data-role="none" id="btnTroubleshootingToolsBack" class="btn btn-primary btn-lg btn-block text-white">
                            <span class="bi-chevron-left"></span>
                        </button>
                    </td>
                    <td><div><h5 class="m-0">Troubleshooting Tools</h5></div></td>
                </tr>
            </table>
        </div>
    </nav>

    <div class="container-fluid">
        <div class="row row-cols-2 row-cols-sm-2 row-cols-md-3 row-cols-lg-4 row-cols-xl-5 row-cols-xxl-6 text-center">
            <div class="col mb-3" id="popTroubleshootingToolsReportAnIssue">
                <div class="bg-white border p-3 rounded btnmcrmenu">
                    <img src="/Content/images/icons/6.png" class="rounded mb-3">
                    <br />
                    Report An Issue
                </div>
            </div>
            <div class="col mb-3" id="popTroubleshootingToolsReprintAReceipt">
                <div class="bg-white border p-3 rounded btnmcrmenu">
                    <img src="/Content/images/icons/10.1.png" class="rounded mb-3">
                    <br />
                    Reprint A Receipt
                </div>
            </div>
            <div class="col mb-3" id="popTroubleshootingToolsReprintZRead">
                <div class="bg-white border p-3 rounded btnmcrmenu">
                    <img src="/Content/images/icons/10.1.png" class="rounded mb-3">
                    <br />
                    Reprint ZRead
                </div>
            </div>
            <div class="col mb-3" id="popTroubleshootingToolsReconstructSalesData">
                <div class="bg-white border p-3 rounded btnmcrmenu">
                    <img src="/Content/images/icons/10.2.png" class="rounded mb-3">
                    <br />
                    Reconstruct Sales Data
                </div>
            </div>
            <div class="col mb-3" id="popTroubleshootingToolsUpdateDatabase">
                <div class="bg-white border p-3 rounded btnmcrmenu">
                    <img src="/Content/images/icons/10.3.png" class="rounded mb-3">
                    <br />
                    Synchronize Data
                </div>
            </div>
            <div class="col mb-3" id="popTroubleshootingToolsFlushData">
                <div class="bg-white border p-3 rounded btnmcrmenu">
                    <img src="/Content/images/icons/10.4.png" class="rounded mb-3">
                    <br />
                    Flush Data
                </div>
            </div>
            <div class="col mb-3" id="popTroubleshootingToolsLazyUpdate">
                <div class="bg-white border p-3 rounded btnmcrmenu">
                    <img src="/Content/images/icons/10.5.png" class="rounded mb-3">
                    <br />
                    Lazy Update
                </div>
            </div>
            <div class="col mb-3" id="popTroubleshootingToolsRestoreDatabase">
                <div class="bg-white border p-3 rounded btnmcrmenu">
                    <img src="/Content/images/icons/10.6.png" class="rounded mb-3">
                    <br />
                    Restore System
                </div>
            </div>
            <div class="col mb-3" id="lnkSecondDisplaySetup" style="display:none">
                <div class="bg-white border p-3 rounded btnmcrmenu">
                    <img src="/Content/images/icons/10.7.png" class="rounded mb-3">
                    <br />
                    Customer Display
                </div>
            </div>
            <div class="col mb-3" id="lnkOrderTakerSetup" style="display:none">
                <div class="bg-white border p-3 rounded btnmcrmenu">
                    <img src="/Content/images/icons/OrderTakerSetupIcon.png" class="rounded mb-3">
                    <br />
                    Order Taker Setup
                </div>
            </div>
            <div class="col mb-3" id="lnkSendingTransactionBySMS" style="display:none">
                <div class="bg-white border p-3 rounded btnmcrmenu">
                    <img src="/Content/images/icons/OrderTakerSetupIcon.png" class="rounded mb-3">
                    <br />
                    Sending Transaction Via SMS Setup
                </div>
            </div>
            <div class="col mb-3" id="lnkZRead">
                <div class="bg-white border p-3 rounded btnmcrmenu">
                    <img src="/Content/images/icons/OrderTakerSetupIcon.png" class="rounded mb-3">
                    <br />
                    Adjust Z-Read Date
                </div>
            </div>
            <div class="col mb-3" id="popTroubleshootingToolsUpdateCacheDatabase">
                <div class="bg-white border p-3 rounded btnmcrmenu">
                    <img src="/Content/images/icons/10.3.png" class="rounded mb-3">
                    <br />
                    Update Cache Database
                </div>
            </div>
        </div>

        <div class="modal fade" id="panLazySync" tabindex="-1" role="dialog" aria-labelledby="mySmallModalLabel">
            <div class="modal-dialog modal-dialog-centered" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 data-role="none" class="modal-title">Lazy Sync</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div data-role="none" class="list-group" id="lstMinch">
                            <a class="list-group-item list-group-item-action" href="#">Password changes </a>
                            <a class="list-group-item list-group-item-action" href="#">Employee Security Access Levels </a>
                            <a class="list-group-item list-group-item-action" href="#">FOWST List</a>
                            <a class="list-group-item list-group-item-action" href="#">Authorized Employees and FOWST assignments</a>
                            <a class="list-group-item list-group-item-action" href="#">Autorized Device List and Device-FOWST assignments</a>
                            <a class="list-group-item list-group-item-action" href="#">Printer Roles and IP Settings</a>
                            <a class="list-group-item list-group-item-action" href="#">Organization Employee</a>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col">
                                <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col">
                                <button data-role="none" id="btnLazyUpdateSyncNow" type="button" class="btn btn-success btn-block btn-lg">Sync Now</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="modal fade" id="panSendingTransactionBySMS" tabindex="-1" role="dialog" aria-labelledby="mySmallModalLabel">
            <div class="modal-dialog modal-dialog-centered" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 data-role="none" class="modal-title">SMS Gateway Settings</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div class="form-check mb-3">
                            <label class="form-check-label" for="chkAllowDeviceToSendTransactionViaSMS">
                                Allow this device to send transaction via SMS
                            </label>
                            <input id="chkAllowDeviceToSendTransactionViaSMS" class="form-check-input" type="checkbox" />
                        </div>

                        <div class="mb-3">
                            <label for="txtSMSGateWayMobileNo" class="form-label"> SMS Gateway Mobile No.: </label>
                            <input id="txtSMSGateWayMobileNo" value="09451067096" data-role="none" class="form-control" />
                        </div>

                        <div style="display:none;">
                            <div data-role="collapsible" data-collapsed="false" data-theme="b" data-content-theme="d"
                                 data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">

                                <label for="txtOverrideSMSGateWayMobileNo"> Override SMS Gateway Mobile No.: </label>

                                <input id="txtOverrideSMSGateWayMobileNo" placeholder="Optional. Use this mobile number if the system cannot connect to the default SMS gateway mobile number." />
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button data-role="none" id="btnSaveSMSGateWayMobileNo" type="button" class="btn btn-success btn-block btn-lg">Save Settings</button>
                        <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                    </div>
                </div>
            </div>
        </div>

        <div class="modal fade" id="panEditZReadDate" tabindex="-1" role="dialog" aria-labelledby="mySmallModalLabel">
            <div class="modal-dialog modal-dialog-centered" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 data-role="none" class="modal-title">ZRead Date</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label for="lblCurrentZReadDate">
                                Current Z-Read Date: <b id="lblCurrentZReadDate"></b>
                            </label>
                        </div>

                        <div class="mb-3">
                            <label for="txtZReadDate" class="form-label">Change Z-Read Date To:</label>
                            <input id="txtZReadDate" type="date" data-role="none" class="form-control" />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col">
                                <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col">
                                <button data-role="none" id="btnSaveZReadDate" type="button" class="btn btn-success btn-block btn-lg">Save</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="modal fade" id="panUpdateCacheDB">
            <div class="modal-dialog modal-dialog-centered" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 data-role="none" class="modal-title">Update Cache Database</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            Due to some issues while caching your sales data on your device for faster retrieval, it may no longer be accurate.
                            You can update your cached sales data by retrieving it from the server.
                            <br /><br />
                            Before proceeding, please ensure there is no unflushed data and that your internet connection is stable.
                            <br /><br />
                            Please provide the details below and tap <i>Update Now</i> button to begin.
                        </div>

                        <div class="mb-3">
                            <label for="selCacheDBReportType" class="form-label"> Report Type: </label>
                            <select data-role="none" class="form-select" id="selCacheDBReportType">
                                <option value="zread">Z-Read</option>
                                <option value="xread">X-Read (Cashier's Sales Report)</option>
                            </select>
                        </div>

                        <div class="mb-3">
                            <label for="txtCacheDBDateFrom" class="form-label"> Store: </label>
                            <select data-role="none" class="form-select" id="selCacheDBStore"></select>
                        </div>

                        <div class="mb-3" style="display:none;">
                            <label for="selCacheDBCashierName" class="form-label"> Cashier Name: </label>
                            <select data-role="none" class="form-select" id="selCacheDBCashierName"></select>
                        </div>

                        <div class="row gy-2 align-items-center mb-3">
                            <label for="txtCacheDBDateFrom" class="col-12"> Date From:</label>
                            <div class="col-4 text-right d-none">
                                <input data-role="none" type="checkbox" class="form-check-input" id="chbAllDayDateFrom" checked="checked" />
                                <label class="form-check-label" for="chbAllDayDateFrom">All Day</label>
                            </div>
                            <div class="col">
                                <input data-role="none" type="date" class="form-control" id="txtCacheDBDateFrom" />
                            </div>
                            <div class="col-4 d-none">
                                <input data-role="none" type="time" class="form-control" id="txtCacheDBTimeFrom" />
                            </div>
                        </div>

                        <div class="row gy-2 align-items-center mb-3">
                            <label for="txtCacheDBDateTo" class="col-12"> Date To:</label>
                            <div class="col-4 text-right d-none">
                                <input data-role="none" type="checkbox" class="form-check-input" id="chbAllDayDateTo" checked="checked" />
                                <label class="form-check-label" for="chbAllDayDateTo">All Day</label>
                            </div>
                            <div class="col">
                                <input data-role="none" type="date" class="form-control" id="txtCacheDBDateTo" />
                            </div>
                            <div class="col-4 d-none">
                                <input data-role="none" type="time" class="form-control" id="txtCacheDBTimeTo" />
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col">
                                <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col">
                                <button data-role="none" id="btnUpdateCacheDBNow" type="button" class="btn btn-success btn-block btn-lg">Update Now</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="pagePopUpRestoreDatabase" custom-data-role="page" style="display:none;">
    <nav class="bg-primary text-white mb-3 sticky-top">
        <div class="container-fluid">
            <table style="width:100%;">
                <tr>
                    <td style="width:50px;">
                        <button data-role="none" id="btnRestoreSystemBack" class="btn btn-primary btn-lg btn-block text-white">
                            <span class="bi-chevron-left"></span>
                        </button>
                    </td>
                    <td><div><h5 class="m-0">Restore System</h5></div></td>
                </tr>
            </table>
        </div>
    </nav>

    <div class="container-fluid">
        <h5 class="mb-3">Please tell us what system issue(s) you are having:</h5>

        <div class="mb-3 form-check">
            <input type="checkbox" class="form-check-input" name="chkPrivate1" id="chkPrivate1">
            <label for="chkPrivate1" class="form-check-label">I can’t find/view any products</label>
        </div>

        <div class="mb-3 form-check">
            <input type="checkbox" class="form-check-input" name="chkPrivate2" id="chkPrivate2">
            <label for="chkPrivate2" class="form-check-label">A lot of product records that were viewable before are now missing</label>
        </div>

        <div class="mb-3 form-check">
            <input type="checkbox" class="form-check-input" name="chkPrivate3" id="chkPrivate3">
            <label for="chkPrivate3" class="form-check-label">I can’t see prices of products</label>
        </div>

        <div class="mb-3 form-check">
            <input type="checkbox" class="form-check-input" name="chkPublic1" id="chkPublic1">
            <label for="chkPublic1" class="form-check-label">I can’t access the POS module</label>
        </div>

        <div class="mt-4 text-center">
            <button data-role="none" id="btnSubmitProblems" class="btn btn-success btn-lg">Submit Issue(s)</button>
        </div>
    </div>
</div>

<div id="pageReportAnIssue" custom-data-role="page" style="display:none;">
    <nav class="bg-primary text-white mb-3 sticky-top">
        <div class="container-fluid">
            <table style="width:100%;">
                <tr>
                    <td style="width:50px;">
                        <button data-role="none" id="btnReportAnIssueBack" class="btn btn-primary btn-lg btn-block text-white">
                            <span class="bi-chevron-left"></span>
                        </button>
                    </td>
                    <td><div><h5 class="m-0">Report An Issue</h5></div></td>
                </tr>
            </table>
        </div>
    </nav>

    <div class="container-fluid pb-5">

    </div>
</div>

<div id="popupIssueTickets" class="modal fade">
    <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Issue Tickets</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                @*<div class="row mb-3">
                    <div class="col-md-6 mb-3"><input data-role="none" type="text" id="txtSearchTicket" placeholder="Search here" class="form-control" /></div>
                    <div class="col-md-6 text-right">
                        <button id="btnCRMTicketRefresh" data-toggle="tooltip" data-placement="top" title="Refresh" data-role="none" class="btn btn-secondary">
                            Refresh List
                        </button>
                        <button data-toggle="tooltip" data-placement="top" title="Help" data-role="none" class="btn btn-secondary btnKbHelp">
                            Help
                        </button>
                        <button id="btnDashboardAddTicket" data-role="none" class="btn btn-success">
                            Add Issue Ticket
                        </button>
                    </div>
                </div>*@
                
                <div class="table-responsive">
                    <table id="tblDataTableTickets" data-role="none" style="display:table; width:100%" class="table table-light table-striped table-hover">
                        <thead>
                            <tr class="custom-theme-f">
                                <th style="width:20px !important;"> # </th>
                                <th style="cursor:pointer; vertical-align:middle;"> Client Name </th>
                                <th style="cursor:pointer; width:100px"> Unresolved </th>
                                <th style="cursor:pointer; width:100px"> Resolved </th>
                                <th style="cursor:pointer; width:100px"> Cancelled </th>
                                <th style="cursor:pointer; width:100px"> Total </th>
                            </tr>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>                
            </div>
        </div>
    </div>
</div>

<div id="popupPrivateMessage" class="modal fade">
    <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable h-100">
        <div class="modal-content h-100 rounded-0">
            <div class="modal-body p-0 overflow-hidden">
                <table name="transhotbuttons" border="0" cellpadding="0" style="width: 100%; height: 100%; top: 0px; bottom: 0px; position: absolute;">
                    <tbody>
                        <tr>
                            <td id="colLeft" class="h-100 align-top border-right border-light position-relative" style="width:400px;">
                                <div class="d-flex align-items-start flex-column bd-highlight h-100 w-100 position-absolute">
                                    <div class="p-2 w-100 bg-light border-top bd-highlight">
                                        <div class="row h-100 align-items-center">
                                            <div class="col-auto">
                                                <h5>Announcements</h5>
                                            </div>
                                            <div class="col text-right">
                                                <button data-role="none" class="btn btn-success" data-toggle="modal" data-target="#popupCreateMessage">Add New</button>
                                            </div>
                                            <div class="col-auto">
                                                <button type="button" data-dismiss="modal" data-role="none" class="btn btn-primary btn-light border d-block d-md-none">
                                                    <span class="bi-x-lg"></span>
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                    <div id="divMessages" class="mb-auto bd-highlight overflow-auto w-100 h-100"></div>
                                </div>
                            </td>
                            <td id="colRight" class="h-100 align-top position-relative d-none d-md-block">
                                <div class="d-flex align-items-start flex-column bd-highlight h-100 w-100 position-absolute">
                                    <div class="p-2 w-100 bg-light border-top bd-highlight">
                                        <div class="row h-100 align-items-center">
                                            <div class="col-auto">
                                                <button type="button" id="btnMsgBack" data-role="none" class="btn btn-primary btn-light border d-block d-md-none">
                                                    <span class="bi-chevron-left"></span>
                                                </button>
                                            </div>
                                            <div class="col text-center">
                                                <span class="font-weight-bold" id="lblPMTTitle"></span>
                                                @*<span id="lblPMDateCreated" style="color: var(--bs-gray)"></span>*@
                                            </div>
                                            <div class="col-auto text-right">
                                                <div class="btn-group">
                                                    <div class="dropdown" id="divMsgOption">
                                                        <button type="button" data-role="none" class="btn btn-primary btn-light border btn-block" data-toggle="dropdown">
                                                            <span class="bi-three-dots-vertical"></span>
                                                        </button>
                                                        <ul class="dropdown-menu dropdown-menu-right">
                                                            <li><a class="dropdown-item" id="btnMsgEdit" href="#"><span class="bi-pencil text-warning"></span> Edit</a></li>
                                                            <li><a class="dropdown-item" id="btnMsgDelete" href="#"><span class="bi-trash text-danger"></span> Delete</a></li>
                                                        </ul>
                                                    </div>
                                                    <button type="button" data-dismiss="modal" data-role="none" class="btn btn-primary btn-light border d-none d-md-block">
                                                        <span class="bi-x-lg"></span>
                                                    </button>
                                                </div>                                                
                                            </div>
                                        </div>
                                    </div>
                                    <div class="mb-auto bd-highlight overflow-auto w-100 h-100 p-3">
                                        <div class="row justify-content-center">
                                            <div id="lblPMMessageContent" class="col-8 rounded border p-3">
                                                
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</div>

<div id="popupCreateMessage" data-backdrop="static" class="modal fade">
    <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">New Message</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <input data-role="none" class="form-control d-none" type="text" id="txtPMID">
                <div class="mb-3">
                    <label class="form-label" for="txtTitle">Title:</label>
                    <input data-role="none" class="form-control" type="text" id="txtPMTitle">
                </div>

                <div class="mb-3">
                    <label class="form-label" for="txtMessage">Message:</label>
                    <textarea data-role="none" class="form-control" id="txtPMMessage" cols="25" rows="5" style="height:200px; resize:none;"></textarea>
                </div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col">
                        <button data-role="none" class="btn btn-secondary btn-lg btn-block" data-dismiss="modal">Cancel</button>
                    </div>
                    <div class="col">
                        <button data-role="none" id="btnMessageSave" class="btn btn-success btn-lg btn-block">Save</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popupAnnouncement" class="modal fade">
    <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header bg-warning">
                <h5 class="modal-title">Announcements</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div class="row mb-3 g-0 align-items-center text-warning" style="font-size:30px;">
                    <div class="col-auto">
                        <span class="bi-stars"></span>
                    </div>
                    <div class="col">Latest</div>
                </div>
                <div id="divLatestAnnouncements"></div>

                <div class="row mb-3 g-0 align-items-center text-dark"  style="font-size:20px;">
                    <div class="col">Previous</div>
                </div>
                <div id="divOtherAnnouncements"></div>
            </div>
        </div>
    </div>
</div>

@*<div name="divVersionContainer" class="container-fluid" style="visibility:collapse">
    <div class="bg-white border p-3 rounded text-center mb-3">
        <div name="divVersion"></div>
    </div>
</div>*@
