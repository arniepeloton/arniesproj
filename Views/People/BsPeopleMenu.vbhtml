@Code
    ViewData("Title") = "Mobiz Home"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code
@Section head
    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
                require([GetPathFormat('/Scripts/sources/People/BsPeopleMenu.js?crc=' + assetversion)]);
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
            height:100%;
        }

        #divmcrmenu > div > div {
            display:none;
        }

        .btnmcrmenu:hover {
            cursor:pointer;
            border-color: #0d6efd !important;
            color: #0d6efd;
        }

        .btnmcrmenu > img {
            height:80px;
            width:80px;
        }
    </style>
End Section
<div id="pageHome" custom-data-role="page" style="display:block;">
    <nav class="bg-primary text-white mb-3 sticky-top">
        <div class="container-fluid">
            <table style="width:100%;">
                <tr>
                    <td><div><h5 class="m-0">Mobiz Complete Retail</h5></div></td>
                    <td style="width:50px;">
                        <button data-role="none" id="btnMessages" class="btn btn-primary btn-lg btn-block text-white">
                            <span class="bi-envelope-fill"></span>
                        </button>
                    </td>
                    <td style="width:50px;">
                        <div class="dropdown">
                            <button type="button" data-role="none" class="btn btn-primary btn-lg btn-block text-white dropdown-toggle dropdown-toggle-split" data-toggle="dropdown" aria-expanded="false">
                                @*<span class="bi-caret-down-fill"></span>*@
                            </button>
                            <ul class="dropdown-menu dropdown-menu-right">
                                <li class="p-3">
                                    <span id="lblName"></span>
                                </li>
                                <li><a class="dropdown-item" id="popPeopleMenuPeopleInfoSummary" href="#"><span class="bi-person-fill"></span> View Profile</a></li>
                                <li><a class="dropdown-item" id="popPeopleMenuToggleFullscreen" href="#"><span class="bi-fullscreen"></span> Toggle Fullscreen</a></li>
                                <li><a class="dropdown-item" id="btnSignOut" href="#"><span class="bi-box-arrow-left"></span> Logout</a></li>
                            </ul>
                        </div>
                    </td>
                </tr>
            </table>
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
            </div>            
        </div>

        <div id="divmcrloading" subpage="divmcr" style="width:100%; text-align:center;">
            <div class="spinner-border text-primary" role="status"></div>
            <br />
            Please wait ...
        </div>

        <div id="divnotamember" subpage="divmcr" style="width:100%; text-align:center; display:none;">
            You should be a member of an organization/company in order to use this program.
        </div>

        <div id="divmcrmenu" subpage="divmcr" style="display:none;">
            <div class="row row-cols-2 row-cols-sm-2 row-cols-md-3 row-cols-lg-4 text-center">
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
                <div class="col mb-3" id="btnStockReceiving">
                    <div class="bg-white border p-3 rounded btnmcrmenu">
                        <img src="/Content/images/icons/3.1.png" class="rounded mb-3">
                        <br />
                        Stock Receiving
                    </div>
                </div>
                <div class="col mb-3" id="btnStockTransfers">
                    <div class="bg-white border p-3 rounded btnmcrmenu">
                        <img src="/Content/images/icons/3.2.png" class="rounded mb-3">
                        <br />
                        Stock Transfers
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
                        <br />
                        Dashboards and Reports
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
                        <br />
                        System Administration
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
        <div id="divmessagesloading" subpage="divmessages" style="width:100%; text-align:center;">
            <div class="spinner-border text-primary" role="status"></div>
            <br />
            Please wait ...
        </div>

        <div id="divMessageContainer" subpage="divmessages" style="display:none;">

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
        <div class="row row-cols-2 row-cols-sm-2 row-cols-md-3 row-cols-lg-4 text-center">
            <div class="col mb-3" id="popTroubleshootingToolsReprintAReceipt">
                <div class="bg-white border p-3 rounded btnmcrmenu">
                    <img src="/Content/images/icons/10.1.png" class="rounded mb-3">
                    <br />
                    Reprint A Receipt
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
                        <h5 data-role="none" class="modal-title">SMS Gateway Settings</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label for="lblCurrentZReadDate">
                                Current Z-Read Date: <b id="lblCurrentZReadDate"></b>
                            </label>
                        </div>

                        <div class="mb-3">
                            <label for="txtSMSGateWayMobileNo" class="form-label">Change Z-Read Date To:</label>
                            <input id="txtZReadDate" data-role="none" class="form-control" />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col">
                                <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col">
                                <button data-role="none" id="btnSaveZReadDate" type="button" class="btn btn-success btn-block btn-lg">Save Z-Read Date</button>
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