@*@Code
    ViewData("Title") = IIf(Request.Params.Item("mode") = "AP", "Accounts Payable", "Credit and Collection")
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code*@

<link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />
<link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
<script>
    require(['bootstrapjs'], function () {
        require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
            //require(['common', ], function () {
                require([GetPathFormat('/Scripts/sources/CreditAndCollection/CreditAndCollection.js?v=' + assetversion)], function () {
                    require(['promise!/Scripts/sources/ERP/OrderProcessing/dashboard/controller.js?v=' + assetversion])
                });
            //});
        })
    })        
</script>


<style type="text/css">
    .dataTables_processing {
        /*Overwrite the Process Loading in datatables*/
        top: 0 !important;
        line-height: 50vh !important;
        height: 100% !important;
        margin-top: -20px !important;
    }

    .right{
        float:right;
        width: 240px;
    }

    .left{
        overflow:hidden; 
        float:none;
    }

    .fixed-cell-width > th,
    .fixed-cell-width > td {
        white-space:nowrap;
    }

    .ui-btn-up-green {
        border: 1px solid #3A7214;
        background: #5BAB25;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, #7EC54E, #41AB25);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #7EC54E),
            color-stop(1, #41AB25));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#7EC54E', EndColorStr='#41AB25')";
    }
    .ui-btn-up-green a.ui-link-inherit {
        color: #fff;
    }
    .ui-btn-hover-green {
        border: 1px solid #176E00;
        background: #6EB64B;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, 
            #99D472, 
            #6EB64B);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #99D472),
            color-stop(1, #6EB64B));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#99D472', EndColorStr='#6EB64B')";
    }
    .ui-btn-hover-green a.ui-link-inherit {
        color: #fff;
    }
    .ui-btn-down-green {
        border: 1px solid #347722;
        background: #7EC54E;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, #569E39, #7EC54E);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #569E39),
            color-stop(1, #7EC54E));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#569E39', EndColorStr='#7EC54E')";
    }
    .ui-btn-down-green a.ui-link-inherit {
        color: #fff;
    }
    .ui-btn-up-green,
    .ui-btn-hover-green,
    .ui-btn-down-green {
        font-family: Helvetica, Arial, sans-serif;
        text-decoration: none;
    }

    .table-padding td {
        padding:5px;
    }

    .divLeft {
        float:none;
        width:auto;
        overflow:auto;
        padding:3px;
    }

    .divRight{
        width:250px;
        float:right;
        padding:3px;
    }

    .navList-a {
        border: 2px solid #000 !important;
    }
</style>

<nav class="navbar navbar-dark bg-primary sticky-top text-light pl-3 pr-3 mb-3">
    <div style="white-space:nowrap; overflow:hidden; text-overflow:ellipsis;">
        <a href="javascript:history.back()" data-role="none" class="btn btn-primary">
            <span class="bi-chevron-left"></span>
        </a>
        <span id="lblCacTitle"></span>
    </div>
</nav>

<div id="divMainCac" class="container-fluid">    

    <div id="popupImageGallery" custom-data-role="popup">
        <div class="popup-md">
            <div class="popup-header">
                File Attachments
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">
                <div>
                    <div style="float:left">
                        <div class="custom-checkbox">
                            <label class="checkbox-only" for="chbImageSelectAll">
                                <input type="checkbox" id="chbImageSelectAll" data-iconpos="notext" />
                            </label>
                            <div>Select All</div>
                        </div>
                    </div>
                    <div style="float:right">
                        <button id="btnDeleteAttachment" data-role="none" class="btn custom-btn-theme-c btn-less-width btn-sm">Delete</button>
                    </div>
                    <div style="clear:both;"></div>
                </div>
                
                <div id="divImageGallery" style="padding:0;">

                </div>
            </div>

            <div class="popup-footer">
                <button id="" data-role="none" class="btn custom-btn-theme-c btnCircle popup-close">
                    <img alt="Options" src="/Content/images/close-black.png" />
                </button>
            </div>
        </div>
    </div>

    <div id="popupAdjustment" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-fullscreen">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Account Adjustments</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div title="New" id="btnNewAccAdjustment" class="custom-btn-theme-c" style="padding:3px; display:inline-block; margin-right:5px;">
                        <img src="/Content/images/newadjustment.png" width="40px" />
                    </div>
                    <br /><br />
                    <div style="overflow:auto;">
                        <table id="tableAdjustmentInvoice" class="table table-light table-hover table-striped" style="width:100%;">
                            <tr class="fixed-cell-width">
                                <td class="txt-center">
                                    Invoice Date:<br />
                                    <div name="InvoiceDate" style="font-size:20px; text-align:center; margin-top: 10px;">June 12, 2016</div>
                                </td>
                                <td class="txt-center">
                                    Invoice No:<br />
                                    <div name="InvoiceNumber" style="font-size:20px; text-align:center; margin-top: 10px;">SI-134</div>
                                </td>
                                <td class="txt-center">
                                    Gross Invoice Amount:<br />
                                    <div name="GrossInvoiceAmount" style="font-size:20px; text-align:center; margin-top: 10px;">20,000.00</div>
                                </td>
                                <td class="txt-center">
                                    Net Adjustments: <br />
                                    <div name="NetAdjustments" style="font-size:20px; text-align:center; margin-top: 10px;">-</div>
                                </td>
                                <td class="txt-center">
                                    Net Invoice Amount:<br />
                                    <div name="NetInvoiceAmount" style="font-size:20px; text-align:center; margin-top: 10px;">-</div>
                                </td>
                                <td class="txt-center">
                                    Accum. Prev. Payments<br />
                                    <div name="AccumPrevPayments" style="font-size:20px; text-align:center; margin-top: 10px;">-</div>
                                </td>
                                <td class="txt-center">
                                    Net Invoice Balance<br />
                                    <div name="NetInvoiceBalance" style="font-size:20px; text-align:center; margin-top: 10px;">-</div>
                                </td>
                                <td class="txt-center">
                                    Invoice Terms:<br />
                                    <div name="InvoiceTerms" style="font-size:20px; text-align:center; margin-top: 10px;">90</div>
                                </td>
                                <td class="txt-center">
                                    Invoice Due Date<br />
                                    <div name="InvoiceDueDate" style="font-size:20px; text-align:center; margin-top: 10px;">-</div>
                                </td>
                            </tr>
                        </table>
                    </div>

                    <br />

                    <table id="tableInvoiceAdjustments" data-role="mode" style="margin-right:5px; display:table;" class="table table-light table-hover table-striped">
                        <thead>
                            <tr class="custom-theme-f">
                                <th data-sort="" style="cursor:pointer;"> Adjustment Date </th>
                                <th data-sort="" style="cursor:pointer;"> Adjustment Ref # </th>
                                <th data-sort="" style="cursor:pointer;"> Adjustment Type </th>
                                <th data-sort="" style="cursor:pointer;"> Reason for Adjustment </th>
                                <th data-sort="" style="cursor:pointer;"> Adjustment Amount </th>
                                <th data-sort="" style="cursor:pointer;"> Remarks </th>
                                <th data-sort="" style="cursor:pointer;"> Updated by </th>
                                <th data-sort="" style="cursor:pointer;">  </th>
                            </tr>
                        </thead>
                        <tbody id="tbodyAccAdjustment">
                            <tr>
                                <td name="AdjustmentDate"></td>
                                <td name="AdjustmentRef"></td>
                                <td name="AdjustmentType">Debit Memo</td>
                                <td name="Reason">Service Charge</td>
                                <td name="Amount" style="text-align:right">200.00</td>
                                <td name="Remarks"></td>
                                <td name="UpdatedBy"></td>
                                <td><a class="btnAdjustmentRemove" data-iconpos="notext" data-theme="b" data-role="button" data-icon="delete" title="Delete">Delete</a></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td></td>
                                <td>Credit Memo</td>
                                <td>Sales Discount</td>
                                <td style="text-align:right">(500.00)</td>
                                <td></td>
                                <td></td>
                                <td><a class="btnAdjustmentRemove" data-iconpos="notext" data-theme="b" data-role="button" data-icon="delete" title="Delete">Delete</a></td>
                            </tr>
                        </tbody>

                        <tfoot>
                            <tr>
                                <td colspan="4" style="text-align:right; font-weight:bold;">Net Adjustments to Invoice</td>
                                <td id="tdNetAdjustmentsToInvoice">(300.00)</td>
                            </tr>
                        </tfoot>
                    </table>
                </div>
                <div class="modal-footer">
                    <button data-role="none" type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <div id="popupNewAdjustment" class="modal fade" data-backdrop="static" data-keyboard="false" tabindex="-1">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">New Account Adjustment</h5>
                    @*<button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>*@
                </div>
                <div class="modal-body">
                    <div class="mb-3 row">
                        <label class="col-form-label col-md-3" for="cbReasonForAdjustment">Reason for Adjustment: <span class="required">*</span></label>
                        <div class="col-md-9">
                            <select id="cbReasonForAdjustment" data-role="none" class="form-select">
                                <option value="">Please select a reason</option>
                            </select>
                        </div>
                    </div>

                    <div class="mb-3 row" name="divNewAdjustmentInvoice">
                        <label class="col-form-label col-md-3" for="butInvoiceToAdjust">Invoice to Adjust: <span class="required">*</span></label>
                        <div class="col-md-9">
                            <button id="butInvoiceToAdjust" data-role="none" class="btn btn-secondary btn-block">Select</button>
                        </div>
                    </div>

                    <div class="mb-3 row">
                        <label class="col-md-3" for="spanAdjustmentInvoiceToDate">Invoice to Date:</label>
                        <div class="col-md-9">
                            <span id="spanAdjustmentInvoiceToDate"></span>
                        </div>
                    </div>

                    <div class="mb-3 row">
                        <label class="col-md-3" for="spanAdjustmentCustomer">Customer name:</label>
                        <div class="col-md-9">
                            <span id="spanAdjustmentCustomer"></span>
                        </div>
                    </div>

                    <div id="divAccountAdjustmentChecks" class="mb-3 border rounded p-3">
                        <div class="text-right mb-3">
                            <button name="butAdd" data-role="none" class="btn btn-success">Add Check</button>
                        </div>
                        <table name="table" class="table table-light table-hover table-striped">
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
                    </div>

                    <div class="mb-3 row">
                        <label class="col-form-label col-md-3" for="txtAdjustmentAmount">Adjustment Amount: <span class="required">*</span></label>
                        <div class="col-md-9">
                            <input id="txtAdjustmentAmount" type="number" data-role="none" class="form-control" />
                        </div>
                    </div>

                    <div class="mb-3 row">
                        <label class="col-form-label col-md-3" for="txtAdjustmentRemarks">Remarks:</label>
                        <div class="col-md-9">
                            <textarea id="txtAdjustmentRemarks" data-role="none" class="form-control"></textarea>
                        </div>
                    </div>

                    <div class="mb-3 row">
                        <label class="col-md-3" for="spanAccountAdjustmentGrossInvoiceAmount">Invoice Gross Amount:</label>
                        <div class="col-md-9">
                            <span id="spanAccountAdjustmentGrossInvoiceAmount">WTH36466Q5</span>
                        </div>
                    </div>

                    <div class="mb-3 row">
                        <label class="col-md-3" for="spanAccountAdjustmentNetInvoiceAmount">Invoice Net Amount (Of Adjustments):</label>
                        <div class="col-md-9">
                            <span id="spanAccountAdjustmentNetInvoiceAmount">WTH36466Q5</span>
                        </div>
                    </div>

                    <div class="mb-3 row">
                        <label class="col-md-3" for="spanAccountAdjustmentNetInvoiceBalance">Net Invoice Balance:</label>
                        <div class="col-md-9">
                            <span id="spanAccountAdjustmentNetInvoiceBalance">WTH36466Q5</span>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" type="button" class="btn btn-secondary btn-lg btn-block" data-dismiss="modal" id="btnCloseAdjustment">Close</button>
                        </div>
                        <div class="col">
                            <button data-role="none" type="button" class="btn btn-success btn-lg btn-block" id="btnAddAdjustment">Save</button>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>

    <div data-role="popup" id="popupNewAdjustment-maybeDuplicate" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;">New Account Adjustment</h3></center>
        <div data-role="content" class="ui-content ui-body-d jPopupContent">
            <div class="custom-ui-label">
                <div>Invoice to Adjust: <span class="required">*</span></div>
                @*<button id="butInvoiceToAdjust">Select</button>*@
                <span id="butInvoiceToAdjust">Select</span>
            </div>

            <div class="custom-ui-label">
                <div>Invoice to Date:</div>
                <div><span id="spanAdjustmentInvoiceToDate"></span></div>
            </div>

            <div class="custom-ui-label">
                <div>Customer name:</div>
                <div><span id="spanAdjustmentCustomer"></span></div>
            </div>

            <div class="custom-ui-label">
                <div>Reason for Adjustment: <span class="required">*</span></div>
                <div>
                    <select id="cbReasonForAdjustment" data-role="none" class="form-control">
                        <option value="">Please select a reason</option>
                    </select>
                </div>
            </div>

            <div id="divAccountAdjustmentChecks-maybeDuplicate" style="margin-bottom:20px">
                <table name="table">
                    <thead>
                        <tr>
                            <th>Bank</th>
                            <th>Check Number</th>
                            <th>Check Amount</th>
                            <th>Check Date</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td name="bank"></td>
                            <td name="checkNumber"></td>
                            <td name="checkAmount"></td>
                            <td name="checkDate"></td>
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

            <br /><br />
            <button id="btnAddAdjustment" data-role="none" class="btn custom-btn-theme-b">Save</button>
            <br /><br />
            <a data-role="none" data-rel="back" href="#" class="btn custom-btn-theme-c">Cancel</a>
        </div>
    </div>

    <div id="popupInvoice" custom-data-role="popup">
        <div class="popup">
            <div class="popup-header">
                Invoice Details
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content" id="divContentCrmViewInvoice">

                @*<div style="overflow:auto;">
                        <table data-role="table" data-role="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                            <thead>
                                <tr class="custom-theme-f">
                                    <th data-sort="" style="cursor:pointer;"> Product No. </th>
                                    <th data-sort="" style="cursor:pointer;"> Description </th>
                                    <th data-sort="" style="cursor:pointer;"> Qty </th>
                                    <th data-sort="" style="cursor:pointer;"> Unit </th>
                                    <th data-sort="" style="cursor:pointer;"> Unit Price </th>
                                    <th data-sort="" style="cursor:pointer;"> Subtotal </th>
                                    <th data-sort="" style="cursor:pointer;"> Accumulated Return Qty </th>
                                    <th data-sort="" style="cursor:pointer;"> Adjusted Subtotal </th>
                                </tr>
                            </thead>
                            <tbody id="">
                            </tbody>
                        </table>
                    </div>*@
            </div>

            <div class="popup-footer">
                <button id="" data-role="none" class="btn custom-btn-theme-c btnCircle popup-close">
                    <img alt="Options" src="/Content/images/close-black.png" />
                </button>
            </div>
        </div>
    </div>

    <div id="divPaymentsPopup">

    </div>

    <div id="popupSOALink" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">SOA Link</h5>
                    <!--<button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>-->
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-md-6" id="divSoaLinkContactInfo">
                        </div>
                        <div class="col-md-6">
                            SOA link is valid from <b id="lblSOALinkDateFrom"></b> to <b id="lblSOALinkDateTo"></b>
                            <br /><br />
                            <div class="row">
                                <div class="col-auto">
                                    Use host:
                                </div>
                                <div class="col">
                                    <div class="form-check">
                                        <input data-role="none" class="form-check-input" type="radio" name="radSOALink" id="radSOALinkDomain" value="domain" checked>
                                        <label data-role="none" class="form-check-label" for="radSOALinkDomain">Domain Name</label>
                                    </div>
                                    <div class="form-check">
                                        <input data-role="none" class="form-check-input" type="radio" name="radSOALink" id="radSOALinkEx" value="ex">
                                        <label data-role="none" class="form-check-label" for="radSOALinkEx">External IP Address</label>
                                    </div>
                                    <div class="form-check">
                                        <input data-role="none" class="form-check-input" type="radio" name="radSOALink" id="radSOALinkIn" value="in">
                                        <label data-role="none" class="form-check-label" for="radSOALinkIn">Internal IP Address</label>
                                    </div>
                                </div>
                            </div>                           
                            <br />
                            <textarea class="form-control" readonly style="resize:none; height:300px;" id="txtgeneratedsoalink"></textarea>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" type="button" class="btn btn-secondary btn-lg btn-block" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col">
                            <button data-role="none" type="button" id="btnCopySOALink" class="btn btn-success btn-lg btn-block">Copy Message</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="divCaCLoading" subpage="pageCac" style="display:block;">Loading, please wait ...</div>

    <div id="divCaCMainPage" subpage="pageCac" style="display:none">
        <div id="divTabCac" style="white-space:nowrap; overflow:auto;">
            <ul class="nav nav-tabs" id="divGatePassTab" role="tablist" style="flex-wrap:nowrap;">
                <li class="nav-item" role="presentation">
                    <button data-role="none" class="nav-link active" data-toggle="tab" data-target="#tabCacDashboard" type="button" role="tab" aria-controls="dashboard" aria-selected="true">Dashboard</button>
                </li>
                <li class="nav-item" role="presentation">
                    <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabCacWork" type="button" role="tab" aria-controls="work" aria-selected="false">Credit and Collection</button>
                </li>
                <li class="nav-item" role="presentation">
                    <button data-role="none" class="nav-link" id="btntabCacSettings" data-target="#tabCacSettings" type="button" role="tab" aria-controls="settings" aria-selected="false">Settings</button>
                </li>
            </ul>
        </div>

        <div class="tab-content p-3">
            <div id="tabCacDashboard" class="tab-pane fade show active">

            </div>

            <div id="tabCacWork" class="tab-pane fade">
                <div class="row">
                    <div class="col-sm-6">
                        <span id="divCacMcrFowst" style="display:none;">Select F.O.W.S.T: <select data-role="none" class="form-control fixed" id="selCacFowst"></select><br /><br /></span>
                        <select name="selCustomerBalanceFilter" data-role="none" class="select custom-theme-c form-select fixed">
                            <option value="">Show customers with balance</option>
                            <option value="withoutbalance">Show customers without balance</option>
                            <option value="all">Show all customers</option>
                        </select>
                    </div>
                    <div class="col-sm-6">
                        <input data-role="none" type="search" id="txtCacSearch" class="form-control" placeholder="Search here" />
                    </div>
                </div>
                
                <div style="overflow:auto; margin-top:1rem;">
                    <table id="tbodyCustomerWithCredit" data-role="table" data-mode="reflow" style="margin-right:5px; display:table; width:100%;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive  table table-light table-hover table-striped">
                        <thead>
                            <tr class="custom-theme-f">
                                <th name="thClientName" data-sort="" style="cursor:pointer;"> Client Name </th>
                                <th data-sort="" style="cursor:pointer;"> Contact Info </th>
                                <th data-sort="" style="cursor:pointer;"> Business Industry </th>
                                <th data-sort="" style="cursor:pointer;"> Default Credit Terms </th>
                                <th data-sort="" style="cursor:pointer;"> Credit Limit </th>
                                <th name="thAccountsReceivableBalance" data-sort="" style="cursor:pointer;"> Accounts Receivable Balance </th>
                                <th name="thWeightedAverageDaysOverdue " data-sort="" style="cursor:pointer;"> Weighted Average Days Overdue </th>
                                <th name="thAssignedSalesTeam" data-sort="" style="cursor:pointer;"> Assigned Sales Team </th>
                            </tr>
                        </thead>
                        <tbody>
                            @*<tr id="btnrow">
                    <td>Arnie Peloton</td>
                    <td>0907-421-1266</td>
                    <td>Cellphone Shop</td>
                    <td>90 days</td>
                    <td>Php 50,000.00</td>
                    <td>Php 33,580.00</td>
                    <td>Team Davao</td>
                </tr>*@
                        </tbody>
                    </table>
                </div>
            </div>

            <div id="tabCacSettings" class="tab-pane fade">

            </div>
        </div>        
    </div>
</div>