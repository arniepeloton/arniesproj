@Code
    ViewData("Title") = "Disbursement"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code
@Section head
    <link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
    <script>        
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
                require(['common'], function () {
                    require(['promise!/Scripts/sources/ERP/Disbursement/main/main.js?v=' + assetversion])
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
                <span>Disbursement</span>
            </div>
        </nav>
        <div class="row p-1" style="margin:0px; width:100%; color:#FFF; background-color:#1969de; font-weight:bold;">
            <div class="BreadCrumbs">
                <div class="ItemsBreadCrumbs">&nbsp;</div>
            </div>
        </div>
    </div>    

    <div id="divErpDisbursement" subpage="divErpMain" class="container-fluid" style="display:block;">
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
</div>