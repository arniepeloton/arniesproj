@Code
    ViewData("Title") = "Web POS"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <link rel="stylesheet" href="/Content/bootstrap-custom.css" />
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
    <link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />
    <script>
        require(['promise!/Scripts/sources/SalesMenu/WebPOS.js?v=' + assetversion]);
    </script>

    <style type="text/css">
        .samplebtn {
            height: 100px;
            text-align: center;
            vertical-align: middle;
            font-size: 18px;
            border-radius: 3px;
            cursor: pointer;
            border: 1px solid #f2f2f2;
            display: flex;
            justify-content: center;
            align-items: center;
        }

        .erp-main-grid {
            text-align: center;
            margin-top: 15px;
            padding: 5px;
            border: 3px solid transparent;
        }

            .erp-main-grid:hover {
                border: 3px solid #CCC;
                border-radius: 2px;
                cursor: pointer;
            }

        .btn-trail {
            padding: 3px;
            display: inline-block;
            margin-right: 5px;
            text-align: center;
            text-shadow: none !important;
            font-weight: bold;
            color: #000;
            background-color: #FFF;
        }

            .btn-trail > span {
                font-size: 10px;
            }

            .btn-trail:hover {
                background-color: #3498db;
                color: #fff;
                cursor: pointer;
            }

        .elementsForErp {
            display: inline !important;
        }

        .dashboardGrid {
            background-color: #fffafa;
            height: 150px;
            padding: 10px;
            border-radius: 2px;
            border: 2px solid rgba(0,0,0, 0);
        }

            .dashboardGrid:hover {
                border: 2px solid #3498db;
                cursor: pointer;
            }

            .dashboardGrid > .gHeader {
                font-weight: bold;
                color: #3498db;
            }

            .dashboardGrid > .gBody {
                font-size: 25px;
                word-break: break-word;
                margin: 15px 0;
                text-align: center;
                color: #2c3e50;
            }

            .dashboardGrid > .gFooter {
                color: #b5b5b5;
                text-align: center;
                /*bottom:10px;
            position:absolute;*/
            }
    </style>
End Section

<nav class="bg-primary custom-theme-a text-white mb-3 sticky-top">
    <div class="container-fluid">
        <table style="width:100%;">
            <tr>
                <td style="width:50px;">
                    <button data-role="none" id="btnWebPosBack" class="btn btn-primary custom-btn-theme-a btn-lg btn-block text-white">
                        <span class="bi-chevron-left glyphicon glyphicon-chevron-left"></span>
                    </button>
                </td>
                <td>POS</td>
            </tr>
        </table>
    </div>
</nav>

<br />

<div id="divWebPOS">
    <div id="divMainWebPOS" class="container-fluid" subpage="divWebPOS" style="display:block;">
        <div class="clearfix">
            <div name="divToolbar" style="float: left">
                <div name="butNew" title="New" class="btn-trail">
                    <span class="erp-invoice new s48"></span><br />
                    <span>New</span>
                </div>
                <div name="butLookup" title="Lookup" class="btn-trail">
                    <span class="erp-invoice search s48"></span><br />
                    <span>Lookup</span>
                </div>
                <div name="butSalesOrderNumber" title="Lookup" class="btn-trail">
                    <span class="erp-op search s48"></span><br />
                    <span>Lookup Sales Order</span>
                </div>
                <div name="butSaveAsDraft" title="Save as Draft" class="btn-trail">
                    <span class="erp-op saveasdraft s48"></span><br />
                    <span>Save as Draft</span>
                </div>
                <div name="butOutput" title="Output" class="btn-trail">
                    <span class="erp-invoice print s48"></span><br />
                    <span>Output</span>
                </div>

                <div name="butCancel" title="Cancel" class="btn-trail">
                    <span class="erp-invoice-cancel"></span><br />
                    <span>Cancel</span>
                </div>

                <div name="butPayInvoice" title="Pay" class="btn-trail">
                    <span class="erp-po pay s48"></span><br />
                    <span>Pay</span>
                </div>

                <div name="butSalesReturn" title="Sales Return" class="btn-trail">
                    <img src="/Content/images/adjustment.png" width="50"><br />
                    <span>Sales Return</span>
                </div>
            </div>
            <div style="float:right; font-size:50px">
                <div name="divTransNum" style="font-size:30px"></div>
                <div name="divDraftOrFinalized"></div>
            </div>
        </div>
        <br /><br />

        <div id="divOPWorkTab" class="custom-tab">
            <ul>
                <li class="active" custom-tab-content="#tabOPWorkBasicInfo">Basic Info</li>
                <li custom-tab-content="#tabOPWorkAdditionalInfoTrading">Additional Info</li>
                <li custom-tab-content="#tabOPWorkAccountDetails">Account Details</li>
            </ul>
        </div>

        <div id="tabOPWorkBasicInfo" custom-tab-role="content" style="display: block;padding-top:20px">
            <div id="accRevOrderHeader" custom-data-role="accordion">
                <div class="accordion-header active">
                    Transaction Header
                </div>
                <div class="accordion-content">
                    <div class="row">
                        <div class="col-sm-8">
                            <div class="row">
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label>Customer: <span class="required">*</span></label>
                                        <button name="butCustomer" data-role="none" class="btn custom-btn-theme-b btn-less-width btn-xs" style="float: right;"><img class="img-search-white" /></button>
                                        <input name="txtCustomer" readonly data-role="none" type="text" class="form-control" />
                                    </div>
                                </div>
                                @*<div class="col-sm-6">
                                    <div class="form-group">
                                        <label>Invoice Type:</label>
                                        <select name="cbInvoiceType" data-role="none" type="text" class="form-control"></select>
                                    </div>
                                </div>*@
                                
                            </div>
                            <div class="row">
                                <div class="col-sm-6">
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>SO Number: <span class="required">*</span></label>
                                                <button name="butSalesOrderNumber" data-role="none" class="btn custom-btn-theme-b btn-less-width btn-xs" style="float: right;"><img class="img-search-white" /></button>
                                                <input name="txtSalesOrderNumber" readonly data-role="none" type="text" class="form-control" />
                                            </div>
                                        </div>
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>SO Date:</label>
                                                <button name="butSalesOrderDate" data-role="none" class="btn custom-btn-theme-b btn-less-width btn-xs" style="float: right;"><img class="img-search-white" /></button>
                                                <input name="txtSalesOrderDate" readonly data-role="none" type="text" id="btnInvoicingSODate" class="form-control" readonly />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label>Invoice Number:</label>
                                        <input name="txtInvoiceNumber" readonly data-role="none" type="text" class="form-control" />
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-6">
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>Quote Number:</label>
                                                <input name="txtQuoteNumber" readonly data-role="none" type="text" class="form-control" />
                                            </div>
                                        </div>
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>Quote Date:</label>
                                                <input name="txtQuoteDate" readonly data-role="none" type="text" id="btnInvoicingQuoteDate" class="form-control" readonly />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label>Manual Invoice Ref. Number:</label>
                                        <input name="txtRefInvoiceNumber" data-role="none" type="text" class="form-control" />
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-6">
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>PO Number: <span class="required">**</span></label>
                                                <input name="txtPoNumber" data-role="none" type="text" class="form-control" />
                                            </div>
                                        </div>
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>PO Date: <span class="required">**</span></label>
                                                <input name="txtPoDate" readonly data-role="none" type="text" id="btnInvoicingPODate" class="form-control" readonly />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label>Payment Terms:</label>
                                        <input name="txtPaymentTerms" data-role="none" type="text" class="form-control" />
                                    </div>
                                </div>
                            </div>
                            <div class="row">                                
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label>Sales Partner</label>
                                        <input name="txtSalesPartner" readonly data-role="none" type="text" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label>Sales Rep:</label>
                                        <input name="txtSalesRep" data-role="none" type="text" class="form-control" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-4">
                            <div class="form-group">
                                <label>Date Needed: <span class="required">*</span></label>
                                <div style="margin-top:-5px; display:inline-block; float:right;">
                                    <button name="butDateNeeded" data-role="none" class="btn custom-btn-theme-b btn-less-width btn-xs" style="float:right;"><img class="img-search-white" /></button>
                                </div>
                                <input name="txtDateNeeded" data-role="none" type="text" id="btnInvoicingDateNeeded" class="form-control" />
                            </div>
                            <div class="form-group">
                                <label>Billing Address<span class="required">*</span>:</label>
                                <div style="margin-top:-5px; display:inline-block; float:right;">
                                    <button name="butAddBillingAddress" data-role="none" class="btn custom-btn-theme-b btn-less-width btn-xs" style="float:right;"><img class="img-plus-white" /></button>
                                    <button name="butSearchBillingAddress" data-role="none" class="btn custom-btn-theme-b btn-less-width btn-xs" style="float:right;"><img class="img-search-white" /></button>
                                </div>
                                <textarea name="txtBillingAddress" data-role="none" class="form-control" style="height:90px; max-height:90px; resize:none"></textarea>
                            </div>
                            <div class="form-group">
                                <label>Delivery Address<span class="required">*</span>:</label>
                                <div style="margin-top:-5px; display:inline-block; float:right;">
                                    <button name="butAddDeliveryAddress" data-role="none" class="btn custom-btn-theme-b btn-less-width btn-xs" style="float:right;"><img class="img-plus-white" /></button>
                                    <button name="butSearchDeliveryAddress" data-role="none" class="btn custom-btn-theme-b btn-less-width btn-xs" style="float:right;"><img class="img-search-white" /></button>
                                </div>
                                <textarea name="txtDeliveryAddress" data-role="none" class="form-control" style="height:90px; max-height:90px; resize:none"></textarea>
                            </div>
                        </div>
                    </div>
                    <table style="width:100%">
                        <tr>
                            <td>
                                <b>Cashier Name:</b><br />
                                <span name="lblWebPOSCashierName"></span>
                            </td>
                            <td>
                                <b>POS Permit No.:</b><br />
                                <span name="lblWebPOSPermitNo"></span>
                            </td>
                            <td>
                                <b>POS Serial No.:</b><br />
                                <span name="lblWebPOSSerialNo"></span>
                            </td>
                            <td>
                                <b>No. of Line Items:</b><br />
                                <span name="lblWebPOSLineItems"></span>
                            </td>
                            <td>
                                <b>Cart Total:</b><br />
                                <span name="lblWebPOSCartTotal"></span>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>

            <br /><br />

            <div id="accRevOrderLine" custom-data-role="accordion">
                <div class="accordion-header active">
                    Transaction Line Items
                    <span class="txt-right">
                        <a name="butAddProduct" data-role="none" class="custom-link">Add product</a>
                    </span>
                </div>
                <div class="accordion-content">
                    <div style="overflow:auto;">
                        <table name="tableProducts" data-role="table" data-role="reflow" style="margin-right: 5px; display: table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                            <thead>
                                <tr class="custom-theme-f fixed-cell-width">
                                    <th class="txt-center"> SKU # </th>
                                    <th class="txt-center"> Product Description </th>
                                    <th class="txt-center"> Order Qty </th>
                                    <th name="thServedQty" class="txt-center"> Served Qty </th>
                                    <th class="txt-center"> Packing Size </th>
                                    <th class="txt-center"> Unit Price </th>
                                    <th class="txt-center"> Line Total </th>
                                    @*<th class="txt-center"> Payment Terms </th>*@
                                </tr>
                            </thead>
                            <tbody>
                                <tr name="data" class="fixed-cell-width">
                                    <td name="ProductNumber" class="txt-center"> SKU # </td>
                                    <td name="ProductDescription" class="txt-center"> Product Description </td>
                                    <td name="OrderQty" class="txt-center"> Order Qty </td>
                                    <td name="ServedQty" class="txt-center"> </td>
                                    <td name="PackingSize" class="txt-center"> Packing Size </td>
                                    <td name="UnitPrice" class="txt-center"> Unit Price </td>
                                    <td name="LineTotal" class="txt-center"> Line Total </td>
                                    @*<td name="PaymentTerms" class="txt-center"> Payment Terms </td>*@
                                </tr>
                            </tbody>
                            <tfoot>
                                <tr name="data" class="fixed-cell-width">
                                    <td name="ProductNumber" class="txt-center"></td>
                                    <td name="ProductDescription" class="txt-center"></td>
                                    <td name="OrderQty" class="txt-center"></td>
                                    <td name="ServedQty" class="txt-center"> </td>
                                    <td name="PackingSize" class="txt-center"></td>
                                    <td name="UnitPrice" class="txt-center"></td>
                                    <td name="LineTotal" class="txt-center"></td>
                                    <td name="PaymentTerms" class="txt-center"></td>
                                </tr>
                            </tfoot>
                        </table>
                        <!--<button name="butAddProduct">Add Product</button>-->
                    </div>
                </div>
            </div>

            <br /><br />

            <div id="accRevOrderLine" custom-data-role="accordion">
                <div class="accordion-header active">
                    Transaction Footer
                </div>
                <div class="accordion-content">
                    <div class="row">
                        <div class="col-sm-4">
                            <div class="form-group">
                                <label>Remarks:</label>
                                <textarea name="txtRemarks" data-role="none" class="form-control" style="max-height:55px; resize:none"></textarea>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </div>

        <div id="tabOPWorkAdditionalInfoTrading" custom-tab-role="content" style="display: none;padding-top:20px">
            <div class="row">
                <div class="col-sm-8">
                    <div class="row">
                        <div class="col-sm-6">
                            <div class="form-group">
                                <label>Customer Outlet:</label>
                                <div style="margin-top: -5px; display: inline-block; float: right;">
                                    <button name="butCustomerFowst" data-role="none" class="btn custom-btn-theme-b btn-less-width btn-xs" style="float: right;"><img class="img-search-white" /></button>
                                </div>
                                <input name="txtCustomerFowst" readonly data-role="none" type="text" class="form-control" />
                            </div>
                        </div>
                        <div class="col-sm-6">
                            <div class="form-group">
                                <label>Customer Department:</label>
                                <div style="margin-top: -5px; display: inline-block; float: right;">
                                    <button name="butCustomerBin" data-role="none" class="btn custom-btn-theme-b btn-less-width btn-xs" style="float: right;"><img class="img-search-white" /></button>
                                </div>
                                <input name="txtCustomerBin" readonly data-role="none" type="text" class="form-control" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="tabOPWorkAccountDetails" custom-tab-role="content" style="display: none;padding-top:20px">

        </div>
    </div>

    <div id="divWebPOSPayment" class="container" subpage="divWebPOS" style="display:none;">
        <div class="row">
            <div class="col-sm-6">
                <h2>Cart Total:</h2>
            </div>
            <div  class="col-sm-6" style="text-align:right">
                <h2><b id="webPosCartTotal">0.00</b></h2>
            </div>
        </div>
        <table cellpadding="5" cellspacing="0" style="width:100%;height:100%;border:1px solid #FFF; table-layout:fixed;">
            <tbody class="cell-border">
                <tr>
                    <td colspan="2">
                        <label>Manual Receipt Number:</label><br>
                        <input id="txtPaymentManualReceiptNumber" maxlength="30">
                    </td>
                </tr>
                <tr>
                    <td id="btnPaymentCash">
                        <label>Cash</label><br>
                        <div class="txt-right txt-bold" id="lblPaidCash">-</div>
                    </td>
                    <td id="btnPaymentDCCard">
                        <label>D/C Card</label><br>
                        <div class="txt-right txt-bold" id="lblPaidDCCard">-</div>
                    </td>
                </tr>
                <tr>
                    <td id="btnPaymentCheque">
                        <label>Cheque</label><br>
                        <div class="txt-right txt-bold" id="lblPaidCheque">-</div>
                    </td>
                    <td id="btnPaymentSrv">
                        <label>SRV</label><br>
                        <div class="txt-right txt-bold" id="lblPaidSrv">-</div>
                    </td>
                </tr>
                <tr>
                    <td id="btnPaymentOther" colspan="2">
                        <label>Other Payment</label><br>
                        <div class="txt-right txt-bold" id="lblPaidOther">-</div>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <label>Current Payment(s) Received</label><br>
                        <div class="txt-right txt-bold" id="lblTotalPaymentRcv">-</div>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <label>Current Payment(s) Applied To Invoice(s)</label><br>
                        <div id="lblCurrentPaymentsAppliedToInvoices" class="txt-right txt-bold">-</div>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <label>Current Payment(s) Not Applied To Invoice(s)</label><br>
                        <div id="lblCurrentPaymentsNotAppliedToInvoices" class="txt-right txt-bold">-</div>
                    </td>
                </tr>
                <tr id="trUnappliedPayments">
                    <td colspan="2">
                        <label>Accumulated Unapplied Payment(s) And Credit(s)</label><br>
                        <div id="divUnappliedPayments" class="txt-right txt-bold">-</div>
                    </td>
                </tr>
            </tbody>
        </table>
        <br />
        <div style="text-align:center;">
            <button data-role="none" name="btnBackToPOS" class="btn custom-btn-theme-c btn-less-width">Back to POS</button>
            <button data-role="none" name="butPaymentSaveAndPrint" class="btn custom-btn-theme-b btn-less-width">Save and Print Receipt</button>
            <button data-role="none" name="butPaymentSaveAndEmail" class="btn custom-btn-theme-b btn-less-width">Save and Email Receipt</button>
            <button data-role="none" name="butPaymentSave" class="btn custom-btn-theme-b btn-less-width">Save Only</button>
        </div>
    </div>
</div>