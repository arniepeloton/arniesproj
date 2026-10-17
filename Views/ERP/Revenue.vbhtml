@Code
    ViewData("Title") = "Revenue"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code
@Section head
    <link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
    <script>        
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion, 'promise!/Scripts/sources/crm/qrcodeNew.js', '/Scripts/sources/CRMDashboard/customcalendar.js'], function () {
                require(['common'], function () {
                    require(['promise!/Scripts/sources/ERP/Revenue/main/controller.js?v=' + assetversion])
                })
            })
        })
    </script>
End Section

<style type="text/css">
    .dataTables_processing {
        /*Overwrite the Process Loading in datatables*/
        top: 0 !important;
        line-height: 50vh !important;
        height: 100% !important;
        margin-top: -20px !important;
    }

    div[custom-data-role="popup"] {
        display:none !important;
    }
</style>
<div>
    <div class="mb-3">
        <nav class="navbar navbar-dark bg-primary sticky-top text-light pl-3 pr-3">
            <div style="white-space:nowrap; overflow:hidden; text-overflow:ellipsis;">
                <a href="javascript:history.back()" data-role="none" class="btn btn-primary">
                    <span class="bi-chevron-left"></span>
                </a>
                <span>Revenue</span>
            </div>
        </nav>
        <div class="row p-1" style="margin:0px; width:100%; color:#FFF; background-color:#1969de; font-weight:bold;">
            <div class="BreadCrumbs">
                <div class="ItemsBreadCrumbs">&nbsp;</div>
            </div>
        </div>
    </div>    

    <div id="divErpRevenue" subpage="divErpMain" class="container-fluid mb-5">
        <div id="divRevMainMenu" subpage="divErpRevenue" style="display:block;">
            <div id="divTabRevenue">
                <ul class="nav nav-tabs" role="tablist" style="flex-wrap:nowrap;">
                    <li class="nav-item" role="presentation">
                        <button data-role="none" id="btnTabRevenueDashboard" class="nav-link active" data-target="#tabRevenueDashboard">Dashboard</button>
                    </li>
                    <li class="nav-item" role="presentation">
                        <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabRevenueWork">Revenue Modules</button>
                    </li>
                    <li class="nav-item" role="presentation">
                        <button data-role="none" id="btnTabRevenueSettings" class="nav-link" data-target="#tabRevenueSettings">Settings</button>
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
                                    Collections Report
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

    <div id="divCustomerSupport" subpage="divErpMain" class="container-fluid mb-5" style="display:none;">

    </div>

    <div id="divErpDeposits" subpage="divErpMain" class="container-fluid" style="display:none;"></div>

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

    <div id="popupSOPAReport" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-fullscreen">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Sales Order And Processing Analytics</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">

                </div>
                <div class="modal-footer">
                    <button data-role="none" type="button" class="btn btn-secondary btn-lg" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>
</div>