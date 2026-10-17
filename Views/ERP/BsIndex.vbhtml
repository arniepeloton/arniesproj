@Code
    ViewData("Title") = "Mobiz EAS"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code
@Section head
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
    <link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />
    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
                require(['common'], function () {
                    require([GetPathFormat('/Scripts/sources/ERP/ERP.js?v=' + assetversion)]);
                })
            })
        })        
    </script>

    <style>
        .popover {
            display: none !important;
        }
    </style>
End Section

<div>
    <div id="popupImageGallery" custom-data-role="popup">
        <div class="popup-md">
            <div class="popup-header">
                File Attachments
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">
            </div>

            <div class="popup-footer">
                <button id="" data-role="none" class="btn custom-btn-theme-c btnCircle popup-close">
                    <img alt="Options" src="/Content/images/close-black.png" />
                </button>
            </div>
        </div>
    </div>

    <div data-role="popup" id="popupNewAdjustment" data-overlay-theme="a" data-theme="a" data-corners="false" style="visibility:collapse-x">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;">New Account Adjustment</h3></center>
        <div data-role="content" class="ui-content ui-body-d jPopupContent" style="width:1000px">
            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <div class="custom-ui-label">
                        <div>Reason for Adjustment: <span class="required">*</span></div>
                        <div>
                            <select id="cbReasonForAdjustment" data-role="none" class="form-control">
                                <option value="">Please select a reason</option>
                            </select>
                        </div>
                    </div>

                    <div name="divNewAdjustmentInvoice" class="custom-ui-label">
                        <div>Invoice to Adjust: <span class="required">*</span></div>
                        <div><button id="butInvoiceToAdjust">Select</button></div>
                    </div>

                    <div class="custom-ui-label">
                        <div>Invoice to Date:</div>
                        <div><span id="spanAdjustmentInvoiceToDate"></span></div>
                    </div>

                    <div class="custom-ui-label">
                        <div>Customer name:</div>
                        <div><span id="spanAdjustmentCustomer"></span></div>
                    </div>

                    <div id="divAccountAdjustmentChecks" style="margin-bottom:20px">
                        <table name="table">
                            <thead>
                                <tr>
                                    <th>Bank</th>
                                    <th>Check Number</th>
                                    <th>Check Amount</th>
                                    <th>Check Date</th>
                                    <th>Invoice No.</th>
                                    <th>Invoice Date</th>
                                    <th>Invoice Amount</th>
                                    <th>Debit Amount</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td name="bank"></td>
                                    <td name="checkNumber"></td>
                                    <td name="checkAmount"></td>
                                    <td name="checkDate"></td>
                                    <td name="invoiceNum"></td>
                                    <td name="invoiceDate"></td>
                                    <td name="invoiceAmount"></td>
                                    <td name="debitAmount"></td>
                                </tr>
                            </tbody>
                        </table>

                        <button name="butAdd" data-role="none" class="btn custom-btn-theme-b">Add</button>
                    </div>

                    <div class="custom-ui-label">
                        <div>Adjustment Amount: <span class="required">*</span></div>
                        <div><input id="txtAdjustmentAmount" type="number" data-role="none" class="form-control" /></div>
                    </div>

                    <div class="custom-ui-label">
                        <div>Remarks:</div>
                        <div><textarea id="txtAdjustmentRemarks" data-role="none" class="form-control"></textarea></div>
                    </div>
                </div>
                <div class="ui-block-b" style="padding-left:60px">
                    <div class="custom-ui-label">
                        <div>Invoice Gross Amount:</div>
                        <div><span id="spanAccountAdjustmentGrossInvoiceAmount">WTH36466Q5</span></div>
                    </div>
                    <div class="custom-ui-label">
                        <div>Invoice Net Amount (Of Adjustments):</div>
                        <div><span id="spanAccountAdjustmentNetInvoiceAmount">WTH36466Q5</span></div>
                    </div>
                    <div class="custom-ui-label">
                        <div>Net Invoice Balance:</div>
                        <div><span id="spanAccountAdjustmentNetInvoiceBalance">WTH36466Q5</span></div>
                    </div>
                </div>
            </div>
                    <br /><br />
                    <button id="btnAddAdjustment" data-role="none" class="btn custom-btn-theme-b">Save</button>
                    <br /><br />
                    <a data-role="none" data-rel="back" href="#" class="btn custom-btn-theme-c">Cancel</a>
                </div>
            </div>    

    <div id="popupCustomerAdminApproval" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-fullscreen">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Unapproved Request For Access To Online Store</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="row mb-3">
                        <div class="offset-md-6 col-md-6">
                            <input data-role="none" type="text" class="form-control" id="txtSearchCustomerAdmin" placeholder="Search here" />
                        </div>
                    </div>

                    <div class="table-responsive">
                        <table id="tableCustomerContactPersonAdmin" data-role="none" style="width:100%; display:table;" class="table table-light table-striped table-hover">
                            <thead>
                                <tr class="custom-theme-f">
                                    <th style="width:30px; text-align:center;">#</th>
                                    <th style="text-align:center;">Organization Name</th>
                                    <th style="text-align:center;">Person Name</th>
                                    <th style="width:200px; text-align:center;">Requested Date</th>
                                    <th style="text-align:center;"></th>
                                    <th style="text-align:center;"></th>
                                </tr>
                            </thead>
                            <tbody id="tbodyCustomerContactPersonAdmin"></tbody>
                        </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <button data-role="none" type="button" class="btn btn-secondary btn-lg" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="divEasHeader" class="mb-3 sticky-md-top" style="display:none;">
    <nav class="bg-primary text-white">
        <div class="container-fluid">
            <div class="row align-items-center">
                <div class="col">
                    <h5 class="m-0 OrganizationNameHeader">&nbsp;</h5>
                </div>
                <div class="col">
                    <label>FOWST: <a data-role="none" id="lblWorkFowstName" class="custom-link" style="color:#FFF;">Please select</a></label>
                </div>
                <div class="col-auto text-right">
                    <div class="dropdown">
                        <button type="button" data-role="none" class="btn btn-primary btn-lg text-white dropdown-toggle dropdown-toggle-split" data-toggle="dropdown" aria-expanded="false">
                            <span id="txtLoggedUserName"></span>
                        </button>
                        <ul class="dropdown-menu dropdown-menu-right">
                            <li><a class="dropdown-item" id="btnViewProfile" href="#"><span class="bi-person-fill"></span> View Profile</a></li>
                            <li><a class="dropdown-item" id="btnERPLogOut" href="#"><span class="bi-box-arrow-left"></span> Logout</a></li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
    </nav>
    <div class="row p-1" style="margin:0px; width:100%; color:#FFF; background-color:#1969de; font-weight:bold;">
        <div class="BreadCrumbs">
            <div class="ItemsBreadCrumbs">Home</div>
        </div>
    </div>
</div>

<div id="divErpMain" subpage="divErpMain" class="container-fluid mb-5" style="display:none;">
    @*<a data-role="none" id="btnSIDocScanner">SI Document Scanner</a>*@
    <div class="row row-cols-2 row-cols-sm-2 row-cols-md-3 row-cols-lg-4 row-cols-xl-5 row-cols-xxl-6 text-center">
        <div class="col mb-3">
            <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnErpRevenue">
                <div class="bi bi-box-arrow-in-down-right rounded mb-3 module-buttons"></div>
                Revenue
            </div>
        </div>
        <div class="col mb-3">
            <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnErpDisbursement">
                <div class="bi bi-box-arrow-up-right rounded mb-3 module-buttons"></div>
                Disbursement
            </div>
        </div>
        <div class="col mb-3">
            <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="butErpProductMaker">
                <div class="bi bi-box-seam rounded mb-3 module-buttons"></div>
                Product Maker
            </div>
        </div>
        <div class="col mb-3">
            <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnErpInventoryHandling">
                <div class="bi bi-boxes rounded mb-3 module-buttons"></div>
                Inventory Handling & Controls
            </div>
        </div>
        <div class="col mb-3">
            <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnErpDTR">
                <div class="bi bi-clock rounded mb-3 module-buttons"></div>
                DTR & HR
            </div>
        </div>
        <div class="col mb-3">
            <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnErpProjectManagement">
                <div class="bi bi-bricks rounded mb-3 module-buttons"></div>
                Project Management
            </div>
        </div>
        <div class="col mb-3">
            <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnErpCustomerSupport">
                <div class="bi bi-people rounded mb-3 module-buttons"></div>
                CRM
            </div>
        </div>
        <div class="col mb-3">
            <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnErpAssetsMgmt">
                <div class="bi bi-building rounded mb-3 module-buttons"></div>
                Asset Management
            </div>
        </div>
        <div class="col mb-3">
            <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnErpGatePass">
                <div class="bi bi-ticket-perferated rounded mb-3 module-buttons"></div>
                Gate Pass Creation and Approval
            </div>
        </div>
        <div class="col mb-3">
            <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnErpGPS">
                <div class="bi bi-pin-map rounded mb-3 module-buttons"></div>
                Fleet Tracking & Management
            </div>
        </div>
        <div class="col mb-3">
            <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnErpFinancialStatement">
                <div class="bi bi-graph-up-arrow rounded mb-3 module-buttons"></div>
                Financial Statements
            </div>
        </div>
        <div class="col mb-3">
            <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnErpContractDesigner">
                <div class="bi bi-pencil-square rounded mb-3 module-buttons"></div>
                Contract and Letter Designer
            </div>
        </div>
        <div class="col mb-3">
            <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnErpSettings">
                <div class="bi bi-gear rounded mb-3 module-buttons"></div>
                System Settings
            </div>
        </div>
    </div>
</div>

<div id="divInventoryHandling" subpage="divErpMain" class="container-fluid" style="display:none;">

</div>

<div id="divDailyTimeRecord" subpage="divErpMain" class="container-fluid" style="display:none;">

</div>

<div id="divCustomerSupport" subpage="divErpMain" class="container-fluid" style="display:none;">

</div>

<div id="divGPS" subpage="divErpMain" class="container-fluid" style="display:none;">

</div>

<div id="divSystemSettings" subpage="divErpMain" class="container-fluid" style="display:none;">

</div>

<div id="divProjectManagement" subpage="divErpMain" class="container-fluid" style="display:none;">

</div>

<div id="divGatePass" subpage="divErpMain" class="container-fluid" style="display:none;">

</div>

<div id="divErpRevenue" subpage="divErpMain" class="container-fluid" style="display:none;">
    <div id="divRevMainMenu" subpage="divErpRevenue" style="display:block;">
        <div id="divTabRevenue">
            <ul class="nav nav-tabs" role="tablist" style="flex-wrap:nowrap;">
                <li class="nav-item" role="presentation">
                    <button data-role="none" id="btnTabRevenueDashboard" class="nav-link active" data-toggle="tab" data-target="#tabRevenueDashboard">Dashboard</button>
                </li>
                <li class="nav-item" role="presentation">
                    <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabRevenueWork">Revenue Modules</button>
                </li>
                <li class="nav-item" role="presentation">
                    <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabRevenueSettings">Settings</button>
                </li>
            </ul>
        </div>

        <div class="tab-content p-3">
            <div id="tabRevenueDashboard" class="tab-pane fade show active">
                <div class="mb-3">
                    <button id="butRevenueDashboardDateRange" value="1" data-role="none" class="btn btn-secondary">
                        <span> Date Range </span>
                    </button>
                </div>

                <div class="row" style="margin-top: 20px; margin-bottom: 20px">
                    <div class="col-xs-12 col-sm-6 col-md-3 mb-3">
                        <div class="dashboardGrid bg-light border p-3 rounded h-100" id="butReportRevenueSalesQuotes">
                            <div class="gHeader">
                                Sales Quotes
                            </div>
                            <div class="gBody" id="lblReportRevenueSalesQuotesBody">
                                0
                            </div>
                            <div class="gFooter" id="lblReportRevenueSalesQuotesFooter">

                            </div>
                        </div>
                    </div>

                    <div class="col-xs-12 col-sm-6 col-md-3 mb-3">
                        <div class="dashboardGrid bg-light border p-3 rounded h-100" id="butReportRevenueSalesOrders">
                            <div class="gHeader">
                                Sales Orders
                            </div>
                            <div class="gBody" id="lblReportRevenueSalesOrdersBody">
                                0
                            </div>
                            <div class="gFooter" id="lblReportRevenueSalesOrdersFooter">

                            </div>
                        </div>
                    </div>

                    <div class="col-xs-12 col-sm-6 col-md-3 mb-3">
                        <div class="dashboardGrid bg-light border p-3 rounded h-100" id="butReportRevenueBackOrders">
                            <div class="gHeader">
                                Backorders
                            </div>
                            <div class="gBody" id="lblReportRevenueBackOrdersFooter">
                                0
                            </div>
                            <div class="gFooter">

                            </div>
                        </div>
                    </div>

                    <div class="col-xs-12 col-sm-6 col-md-3 mb-3">
                        <div class="dashboardGrid bg-light border p-3 rounded h-100" id="butReportRevenueSalesInvoices">
                            <div class="gHeader">
                                Sales Invoices
                            </div>
                            <div class="gBody" id="lblReportRevenueInvoicesBody">
                                0
                            </div>
                            <div class="gFooter" id="lblReportRevenueInvoicesFooter">

                            </div>
                        </div>
                    </div>

                    <div class="col-xs-12 col-sm-6 col-md-3 mb-3">
                        <div class="dashboardGrid bg-light border p-3 rounded h-100" id="butReportRevenueGrossProfitabilityValidation">
                            <div class="gHeader">
                                Gross Profitability Validation
                            </div>
                            <div class="gBody" id="lblReportRevenueGrossProfitabilityValidationBody">
                                0
                            </div>
                            <div class="gFooter" id="lblReportRevenueGrossProfitabilityValidationFooter">

                            </div>
                        </div>
                    </div>

                    <div class="col-xs-12 col-sm-6 col-md-3 mb-3">
                        <div class="dashboardGrid bg-light border p-3 rounded h-100" id="btnReportNetSales">
                            <div class="gHeader">
                                Net Sales
                            </div>
                            <div class="gBody" id="lblNetSalesBody">
                                0
                            </div>
                            <div class="gFooter" id="lblNetSalesFooter">

                            </div>
                        </div>
                    </div>

                    <div class="col-xs-12 col-sm-6 col-md-3 mb-3">
                        <div class="dashboardGrid bg-light border p-3 rounded h-100" id="btnReportCac">
                            <div class="gHeader">
                                Credit and Collection
                            </div>
                            <div class="gBody" id="lblCacBody">
                                0
                            </div>
                            <div class="gFooter" id="lblCacFooter">

                            </div>
                        </div>
                    </div>

                    <div class="col-xs-12 col-sm-6 col-md-3 mb-3">
                        <div class="dashboardGrid bg-light border p-3 rounded h-100" id="btnReportSopa">
                            <div class="gHeader">
                                S.O.P.A
                            </div>
                            <div class="gBody" id="lblSopaBody">
                                0
                            </div>
                            <div class="gFooter" id="lblSopaFooter">

                            </div>
                        </div>
                    </div>

                    <div class="col-xs-12 col-sm-6 col-md-3 mb-3">
                        <div class="dashboardGrid bg-light border p-3 rounded h-100" id="butReportProfit">
                            <div class="gHeader">
                                Top Sellers and Grossers
                            </div>
                            <div class="gBody">

                            </div>
                            <div class="gFooter">

                            </div>
                        </div>
                    </div>

                    <div class="col-xs-12 col-sm-6 col-md-3 mb-3">
                        <div class="dashboardGrid bg-light border p-3 rounded h-100" id="btnReportCustomerAdmin">
                            <div class="gHeader">
                                Unapproved Request for Access
                            </div>
                            <div class="gBody" id="lblCustomerAdminBody">
                                0
                            </div>
                            <div class="gFooter">

                            </div>
                        </div>
                    </div>

                    <div class="col-xs-12 col-sm-6 col-md-3 mb-3">
                        <div class="dashboardGrid bg-light border p-3 rounded h-100" id="btnReportReprintRequests">
                            <div class="gHeader">
                                Reprint Requests
                            </div>
                            <div class="gBody" id="lblReprintRequestBody">
                                0
                            </div>
                            <div class="gFooter">

                            </div>
                        </div>
                    </div>

                    <div class="col-xs-12 col-sm-6 col-md-3 mb-3">
                        <div class="dashboardGrid bg-light border p-3 rounded h-100" id="butReportPriceUpdateRequests">
                            <div class="gHeader">
                                Price Update Requests
                            </div>
                            <div class="gBody" id="lblPriceUpdateRequests">
                                0
                            </div>
                            <div class="gFooter">

                            </div>
                        </div>
                    </div>
                </div>

                @*<div class="accordion mb-3">
                        <div class="card">
                            <div class="card-header p-0">
                                <button data-target="#" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                    <b>Other Reports</b>
                                </button>
                            </div>
                            <div id="" class="collapse show">
                                <div class="card-body">
                                    <a id="butSalesReportByCustomerAndSupplierProducts" href="#">Sales Report By Customer And Supplier Products </a> <br /> <br />
                                    <a id="butSalesReportByProductQuantity" href="#">Sales Report By Product Measurement Quantity per Supplier</a> <br /> <br />
                                    <a id="butSalesReportByProductQuantityPerMonth" href="#">Sales Report by Product Quantity per Month</a> <br /> <br />
                                    <a id="butSalesReportByProductBaseQuantity" href="#">Sales Report By Product Base Quantity per Supplier</a> <br /> <br />
                                    <a id="butInventoryMovementByFOWST" href="#">Inventory Movement By FOWST</a> <br /> <br />
                                </div>
                            </div>
                        </div>
                    </div>*@
            </div>

            <div id="tabRevenueWork" class="tab-pane fade">
                <div class="row row-cols-2 row-cols-sm-2 row-cols-md-3 row-cols-lg-4 row-cols-xl-5 row-cols-xxl-6 text-center">
                    <div class="col mb-3">
                        <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnRevPOS">
                            <div class="bi bi-pc-display-horizontal rounded mb-3 module-buttons"></div>
                            POS
                        </div>
                    </div>
                    <div class="col mb-3">
                        <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnRevSalesCrm">
                            <div class="bi bi-person rounded mb-3 module-buttons"></div>
                            Sales CRM
                        </div>
                    </div>
                    <div class="col mb-3">
                        <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnRevQuotation">
                            <div class="bi bi-chat-right-quote rounded mb-3 module-buttons"></div>
                            Quotation
                        </div>
                    </div>
                    <div class="col mb-3">
                        <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnRevOrderProcessing">
                            <div class="bi bi-list-check rounded mb-3 module-buttons"></div>
                            Sales Order
                        </div>
                    </div>
                    <div class="col mb-3">
                        <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="butStockTransfer">
                            <div class="bi bi-arrow-up-square rounded mb-3 module-buttons"></div>
                            Stock Transfer
                        </div>
                    </div>
                    <div class="col mb-3">
                        <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnRevOrderFulfillment">
                            <div class="bi bi-box-seam rounded mb-3 module-buttons"></div>
                            Fulfillment Center
                        </div>
                    </div>
                    <div class="col mb-3">
                        <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnRevInvoicing">
                            <div class="bi bi-receipt rounded mb-3 module-buttons"></div>
                            Invoicing
                        </div>
                    </div>
                    <div class="col mb-3">
                        <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="butDeliveryTracking">
                            <div class="bi bi-truck rounded mb-3 module-buttons"></div>
                            Delivery Tracking
                        </div>
                    </div>
                    <div class="col mb-3">
                        <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnRevCreditAndCollection">
                            <div class="bi bi-cash-stack rounded mb-3 module-buttons"></div>
                            Credit and Collection
                        </div>
                    </div>
                    <div class="col mb-3">
                        <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="butRevDeposits">
                            <div class="bi bi-bank rounded mb-3 module-buttons"></div>
                            Deposits
                        </div>
                    </div>
                    <div class="col mb-3">
                        <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="butErpSerialNumberTracking">
                            <div class="bi bi-upc rounded mb-3 module-buttons"></div>
                            Serial Number / IMEI Tracking
                        </div>
                    </div>
                    <div class="col mb-3">
                        <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnRevenuePriceCanvassing">
                            <div class="bi bi-tag rounded mb-3 module-buttons"></div>
                            Competitor Price Canvassing
                        </div>
                    </div>
                    @*<div class="col mb-3">
                            <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="butErpPurchaseOrderRequest">
                                <img src="/Content/images/PurchaseOrderRequest.png" />
                                <br /><br />
                                Purchase Order Request
                            </div>
                        </div>*@
                    @*<div class="col mb-3">
                            <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="butRevenuePriceLists"
                                <img src="/Content/images/disburse-pricelists.png" />
                                <br /><br />
                                Create and Download Price Lists
                            </div>
                        </div>*@
                    <div class="col mb-3">
                        <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="butErpAutoPrintInvoice">
                            <div class="bi bi-printer rounded mb-3 module-buttons"></div>
                            Auto Print Invoice
                        </div>
                    </div>
                    <div class="col mb-3">
                        <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnConsignee">
                            <div class="bi bi-person-check rounded mb-3 module-buttons"></div>
                            Consignee
                        </div>
                    </div>
                    @*<div class="col mb-3">
                            <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid btnOngoing">
                                <img src="/Content/images/cheque.png" />
                                <br /><br />
                                Cheque Clearing
                            </div>
                        </div>*@
                </div>
            </div>

            <div id="tabRevenueSettings" class="tab-pane fade">

            </div>
        </div>
    </div>

    <div id="divOrderFulfillment" subpage="divErpRevenue" style="display:none;">
        <div id="divOrderFulfillmentMain" subpage="divOrderFulfillment" style="display:block;">
            <div id="divOrderFulfillmentTab">
                <ul class="nav nav-tabs" role="tablist" style="flex-wrap:nowrap;">
                    <li class="nav-item" role="presentation">
                        <button data-role="none" class="nav-link active" data-toggle="tab" data-target="#tabOFDashboard">Dashboard</button>
                    </li>
                    <li class="nav-item" role="presentation">
                        <button data-role="none" id="tabOrderFulfillmentModule" class="nav-link" data-target="#tabOFWork">Fulfillment Center</button>
                    </li>
                    <li class="nav-item" role="presentation">
                        <button data-role="none" id="tabOrderFulfillmentSettings" class="nav-link" data-target="#tabOFSettings">Settings</button>
                    </li>
                </ul>
            </div>

            <div class="tab-content p-3">
                <div id="tabOFDashboard" class="tab-pane fade show active">

                </div>
                <div id="tabOFWork" class="tab-pane fade">
                    <div id="divOrderFulfillmentWork" style="padding-top:20px"></div>
                </div>
                <div id="tabOFSettings" class="tab-pane fade">
                    <div class="row mb-3">
                        <label class="col-md-auto col-form-label">Pick List Print Count:</label>
                        <div class="col-md-3">
                            <input data-role="none" class="form-control" type="number" name="txtPickListPrintCount" />
                        </div>
                    </div>

                    <div class="form-check" style="display:none">
                        <label class="form-check-label" for="chkFcSettingExcludeVendorPoInFc">Exclude Vendor PO in FC (Internal Order Fulfillment)</label>
                        <input data-role="none" class="form-check-input" id="chkFcSettingExcludeVendorPoInFc" type="checkbox" />
                    </div>

                    <div class="mt-5 text-center">
                        <button data-role="none" class="btn btn-success btn-lg" name="butSave">Save</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="divCreditAndCollection" subpage="divErpRevenue" style="display:none;">

    </div>

    <div id="divRevenuePriceCanvass" subpage="divErpRevenue" style="display:none;">
        <div id="divRevenuePriceCanvassMain" subpage="divRevenuePriceCanvass" style="display:block;">
            Loading ...
        </div>
    </div>

    <div id="divConsignee" subpage="divErpRevenue" style="display:none;">

    </div>
</div>

<div id="divErpDisbursement" subpage="divErpMain" class="container-fluid" style="display:none;">
    <div id="divDisburseMainMenu" subpage="divErpDisbursement" style="display:block;">
        <div id="divTabDisbursement">
            <ul class="nav nav-tabs" role="tablist" style="flex-wrap:nowrap;">
                <li class="nav-item" role="presentation">
                    <button data-role="none" id="btnTabDisbursementDashboard" class="nav-link active" data-toggle="tab" data-target="#tabDisbursementDashboard">Dashboard</button>
                </li>
                <li class="nav-item" role="presentation">
                    <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabDisbursementWork">Disbursement Modules</button>
                </li>
                <li class="nav-item" role="presentation">
                    <button data-role="none" id="btnTabDisbursementSettings" class="nav-link" data-target="#tabDisbursementSettings">Settings</button>
                </li>
            </ul>
        </div>

        <div class="tab-content p-3">
            <div id="tabDisbursementDashboard" class="tab-pane fade show active">

            </div>

            <div id="tabDisbursementWork" class="tab-pane fade">
                <div class="row row-cols-2 row-cols-sm-2 row-cols-md-3 row-cols-lg-4 row-cols-xl-5 row-cols-xxl-6 text-center">
                    <div class="col mb-3">
                        <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="butErpItemRequisition">
                            <div class="bi bi-journal-check rounded mb-3 module-buttons"></div>
                            Item Requisition
                        </div>
                    </div>
                    <div class="col mb-3">
                        <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnDisbursePriceCanvassing">
                            <div class="bi bi-tag rounded mb-3 module-buttons"></div>
                            Vendor Price Canvassing
                        </div>
                    </div>
                    <div class="col mb-3">
                        <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnDisbursePurchasing">
                            <div class="bi bi-cart-check rounded mb-3 module-buttons"></div>
                            Purchasing
                        </div>
                    </div>
                    <div class="col mb-3">
                        <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="butDisbursementExpenseTracking">
                            <div class="bi bi-coin rounded mb-3 module-buttons"></div>
                            Expense Tickets
                        </div>
                    </div>
                    @*<div class="col-sm-3">
                            commented by arnie on 9-3-2020, hindi na daw gagamitin, bale ang Expense Ticket na ang gagamitin
                            <div class="erp-main-grid" id="btnDisburseJO">
                                <img src="/Content/images/jo.png" />
                                <br /><br />
                                Job Order
                            </div>
                        </div>*@
                    <div class="col mb-3">
                        <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="butDisbursmentStockReceiving">
                            <div class="bi bi-arrow-down-square rounded mb-3 module-buttons"></div>
                            Stock Receiving
                        </div>
                    </div>
                    <div class="col mb-3">
                        <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="butDisbursementAccountsPayable">
                            <div class="bi bi-cash-stack rounded mb-3 module-buttons"></div>
                            Accounts Payable Verification
                        </div>
                    </div>
                    @*<div class="col mb-3">
                            <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="">
                                <div class="bi bi-cash-coin rounded mb-3 module-buttons"></div>
                                Disbursements
                            </div>
                        </div>*@
                    <div class="col mb-3">
                        <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid btnOngoing" id="">
                            <div class="bi bi-card-checklist rounded mb-3 module-buttons"></div>
                            Cheque Clearing
                        </div>
                    </div>
                </div>
            </div>

            <div id="tabDisbursementSettings" class="tab-pane fade">

            </div>
        </div>
    </div>

    <div id="divPriceCanvass" subpage="divErpDisbursement" style="display:none;">
        <div id="divPriceCanvassMain" subpage="divPriceCanvass" style="display:block;">
            Loading ...
        </div>
    </div>

    <div id="divShowItemRequisition" subpage="divErpDisbursement" style="display: none;">
        <div id="divShowItemRequisitionMain" subpage="divShowItemRequisition" style="display: block;">
            <div id="divShowItemRequisitionTab">
                <ul class="nav nav-tabs" role="tablist" style="flex-wrap:nowrap;">
                    <li class="nav-item" role="presentation">
                        <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabShowItemRequisitionDashboard">Dashboard</button>
                    </li>
                    <li class="nav-item" role="presentation">
                        <button data-role="none" class="nav-link active" data-toggle="tab" data-target="#tabShowItemRequisitionWork">Item Requisition</button>
                    </li>
                    <li class="nav-item" role="presentation">
                        <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabShowItemRequisitionSettings">Settings</button>
                    </li>
                </ul>
            </div>

            <div class="tab-content p-3">
                <div id="tabShowItemRequisitionDashboard" class="tab-pane fade">

                </div>
                <div id="tabShowItemRequisitionWork" class="tab-pane fade show active">

                </div>
                <div id="tabShowItemRequisitionSettings" class="tab-pane fade">

                </div>
            </div>
        </div>
    </div>

    <div id="divPurchasing" subpage="divErpDisbursement" style="display:none;">

    </div>

    <div id="divJO" subpage="divErpDisbursement" style="display:none;">
        <div id="divJOMain" subpage="divJO" style="display:block;">
            <div id="divPurchasingTab" class="custom-tab">
                <ul>
                    <li class="active" custom-tab-content="#tabJODashboard">Job Order Dashboard</li>
                    <li custom-tab-content="#tabJOWork">Job Order</li>
                    <li custom-tab-content="#tabJOSettings">Job Order Settings</li>
                </ul>
            </div>
            <div id="tabJODashboard" custom-tab-role="content" style="display:block;">
                Dashboard
            </div>
            <div id="tabJOWork" custom-tab-role="content" style="display:none;">
                <div>
                    <div title="New" class="btn-trail">
                        <span class="erp-jo new s48"></span><br />
                        <span>New</span>
                    </div>
                    <div title="Lookup" class="btn-trail">
                        <span class="erp-jo search s48"></span><br />
                        <span>Lookup</span>
                    </div>
                    <div title="Save & Finalize" class="btn-trail">
                        <span class="erp-jo finalsave s48"></span><br />
                        <span>Save & Finalize</span>
                    </div>
                    <div title="Save a Copy" class="btn-trail">
                        <span class="erp-jo saveacopy s48"></span><br />
                        <span>Save a Copy</span>
                    </div>
                    <div title="Save as Draft" class="btn-trail">
                        <span class="erp-jo saveasdraft s48"></span><br />
                        <span>Save as Draft</span>
                    </div>
                    <div title="Output" class="btn-trail">
                        <span class="erp-jo print s48"></span><br />
                        <span>Output</span>
                    </div>
                    <div title="Pay" class="btn-trail">
                        <span class="erp-jo pay s48"></span><br />
                        <span>Pay</span>
                    </div>
                </div>

                <br /><br />

                <div id="accRevOrderHeader" custom-data-role="accordion">
                    <div class="accordion-header active">
                        Order Header
                    </div>
                    <div class="accordion-content">
                        <div id="divJOWorkTab" class="custom-tab">
                            <ul>
                                <li class="active" custom-tab-content="#tabJOWorkBasicInfo">Basic Info</li>
                            </ul>
                        </div>

                        <div id="tabJOWorkBasicInfo" custom-tab-role="content" style="display:block;">
                            <div class="row">
                                <div class="col-sm-8">
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>Vendor: <span class="required">*</span></label>
                                                <input data-role="none" type="text" class="form-control" />
                                            </div>
                                        </div>
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>Sales Rep:</label>
                                                <input data-role="none" type="text" class="form-control" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>PO Number:</label>
                                                <input data-role="none" type="text" class="form-control" />
                                            </div>
                                        </div>
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>Payment Terms:</label>
                                                <input data-role="none" type="text" class="form-control" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <div class="row">
                                                <div class="col-sm-6">
                                                    <div class="form-group">
                                                        <label>Quote Number:</label>
                                                        <input data-role="none" type="text" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-sm-6">
                                                    <div class="form-group">
                                                        <label>Quote Date:</label>
                                                        <input data-role="none" type="text" id="btnJOQuoteDate" class="form-control" readonly />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>Date Needed: <span class="required">*</span></label>
                                                <input data-role="none" type="text" id="btnJODateNeeded" class="form-control" readonly />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <label>Billing Address: <span class="required">*</span></label>
                                        <div style="margin-top:-5px; display:inline-block; float:right;">
                                            <button data-role="none" class="btn custom-btn-theme-b btn-less-width btn-xs" style="float:right;"><img class="img-plus-white" /></button>
                                            <button data-role="none" class="btn custom-btn-theme-b btn-less-width btn-xs" style="float:right;"><img class="img-search-white" /></button>
                                        </div>
                                        <textarea data-role="none" class="form-control" style="max-height:55px; resize:none"></textarea>
                                    </div>
                                    <div class="form-group">
                                        <label>Delivery Address:<span class="required">*</span></label>
                                        <div style="margin-top:-5px; display:inline-block; float:right;">
                                            <button data-role="none" class="btn custom-btn-theme-b btn-less-width btn-xs" style="float:right;"><img class="img-plus-white" /></button>
                                            <button data-role="none" class="btn custom-btn-theme-b btn-less-width btn-xs" style="float:right;"><img class="img-search-white" /></button>
                                        </div>
                                        <textarea data-role="none" class="form-control" style="max-height:55px; resize:none"></textarea>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <br /><br />

                <div id="accRevOrderLine" custom-data-role="accordion">
                    <div class="accordion-header active">
                        Order Line Items
                    </div>
                    <div class="accordion-content">
                        <div style="overflow:auto;">
                            <table data-role="table" data-role="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                                <thead>
                                    <tr class="custom-theme-f fixed-cell-width">
                                        <th class="txt-center"> SKU # </th>
                                        <th class="txt-center"> Product Description </th>
                                        <th class="txt-center"> Order Qty </th>
                                        <th class="txt-center"> Packing Size </th>
                                        <th class="txt-center"> Unit Price </th>
                                        <th class="txt-center"> Line Total </th>
                                        <th class="txt-center"> Payment Terms </th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>
                    </div>
                </div>

                <br /><br />

                <div id="accRevOrderLine" custom-data-role="accordion">
                    <div class="accordion-header active">
                        Order Footer
                    </div>
                    <div class="accordion-content">
                        <div class="row">
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label>Remarks:</label>
                                    <textarea data-role="none" class="form-control" style="max-height:55px; resize:none"></textarea>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
            <div id="tabJOSettings" custom-tab-role="content" style="display:none;">
                Settings
            </div>
        </div>
    </div>

    <div id="divdivErpDisbursementAccountsPayable" subpage="divErpDisbursement" style="display:none;">

    </div>

    <div id="divErpExpenseTracking" subpage="divErpDisbursement" style="display:none;"></div>
</div>

<div id="divErpDeposits" subpage="divErpMain" class="container-fluid" style="display:none;"></div>
<div id="divErpReports" subpage="divErpMain" class="container" style="display:none;"></div>
<div id="divErpAssetsManagement" subpage="divErpMain" class="container-fluid" style="display:none;"></div>
<div id="divErpProductMaker" subpage="divErpMain" class="container-fluid" style="display:none;"></div>
<div id="divErpFinancialStatement" subpage="divErpMain" class="container-fluid" style="display:none;"></div>
<div id="divErpContractDesigner" subpage="divErpMain" class="container-fluid" style="display:none;"></div>
<div id="divErpKnowledgeBase" subpage="divErpMain" class="container-fluid" style="display:none;"></div>
<div id="divPopupUrlPage"></div>