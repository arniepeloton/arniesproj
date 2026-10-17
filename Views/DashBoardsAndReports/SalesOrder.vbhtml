<div>
    <script>
        require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/SalesOrder.js?v='+assetversion)]);
    </script>
    <style>
        table.myTable {
                border-collapse:collapse;
                width: 100%;
            }
            .ui-table-reflow.ui-responsive {
                display: table;
            }

         .SearchText {
            margin:0px;
            border: 0px;
            text-shadow: 0px;
            background: #FFF;
            height: 40px;
            border-radius: 0px;

        }
        .ui-focus {
         -moz-box-shadow: none !important;
         -webkit-box-shadow: none !important;
         box-shadow: none !important;
        }
        .ui-shadow-inset {
            box-shadow:none !important;
        }

        .ui-dialog-contain
        {
            max-width: 700px;
        }

        .ui-dialog-background {
            opacity: 0.5;
            display: block !important;
            -webkit-transition: opacity 0.5s ease-in;
        }

        .ui-dialog-background.pop.in {
            opacity: 1;
            -webkit-transition: opacity 0.5s ease-in;
        }

        .ui-dialog {
            min-height: 100% !important;
            background: transparent !important;
        }

            #table-custom-2 tr th{
                vertical-align: middle;
            }
    </style>


    <div id="pageOrderCentral" subpage="divOrderFulfillmentWork">
        <div data-role="content" style="padding: 0px 5px 0px 5px;">
            <div class="row equal align-items-center">
                <div class="col-sm-3">
                    Order Date: <button data-role="none" id="btnSopaOrderDate" class="btn custom-btn-theme-c btn-secondary btn-block"></button>
                </div>
                <div class="col-sm-3">
                    Filter Status by:
                    <select id="selSopaFilter" data-role="none" class="form-control form-select">
                        <option value="">All</option>
                        <option value="New" selected>New</option>
                        <option value="Viewed">Viewed</option>
                        <option value="Modified">Modified</option>
                        <option value="Downloaded">Downloaded</option>
                    </select>
                </div>
                <div class="col-sm-3" name="hideInVendorPortal">
                    Filter Area by: <button data-role="none" id="butSopaFilterArea" class="btn custom-btn-theme-c btn-secondary  btn-block">[All Areas]</button>@*<div data-role="none" class="form-control fixed"></div>*@
                </div>
                <div class="col-sm-3">
                    Sort by:
                    <select id="selSopaSort" data-role="none" class="form-control form-select">
                        <option value="date needed" selected>Date Needed</option>
                        <option value="order date">Order Date</option>
                    </select>
                </div>
                <div class="col-sm-6" style="margin-top:10px">
                    <form id="formSearch">
                        <div class="input-group">
                            <input data-role="none" id="txtSearch" type="text" class="form-control" placeholder="Search orders">
                            <span class="input-group-btn">
                                <button id="btnSearch" data-role="none" class="btn btn-default btn-secondary rounded-0 rounded-right" type="button">Search</button>
                            </span>
                            &nbsp;|&nbsp;
                            <a href="" id="butSearchFromCache">Search from cache</a>&nbsp;|&nbsp;
                            <a href="" id="butSearchRecache">Recache</a>
                        </div>
                    </form>
                </div>
            </div>
        </div>
        <br />
        <div style="overflow:auto">
            <table data-role="table" id="table-custom-2" class="ui-body-d ui-shadow table-stripe ui-responsive custom-tbl-responsive  table table-light table-hover table-striped" data-column-btn-theme="b">
                <thead class="custom-theme-f">
                    <tr style="cursor: pointer" class="custom-theme-f">
                        <th></th>
                        <th>Serving FOWST</th>
                        <th>Area</th>
                        <th>Encoding Location</th>
                        <th>Order Date</th>
                        <th>Date Needed</th>
                        <th>Order No.</th>
                        <th>Customer PO Number</th>
                        <th>Customer PO Date</th>
                        <th>Customer</th>
                        <th>Customer Outlet</th>
                        <th>Customer Department</th>
                        <th>Last Modified By</th>
                        <th>Delivery Address</th>
                        <th>Delivery Method</th>
                        <th>Supplier</th>
                        <th>Remarks</th>
                        <th style="width:170px">Status</th>
                        <th name="netsuite" style="display:none">NetSuite Status</th>
                    </tr>

                </thead>

                <tbody id='tblSalesOrderReportBody'></tbody>

            </table>
        </div>

    </div>

    <div id="popupOrderStatus-bs" class="modal fade" data-backdrop="static" style="display:none;">
        <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Order Details</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body" id="popupOrderStatuscontent-bs">
                </div>
                <div class="modal-footer">
                    <button data-role="none" class="btn btn-secondary btn-lg" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <div id="popupOrderStatus" custom-data-role="popup" style="display:none;">
        <div class="popup">
            <div class="popup-header">
                Order Details<a class="popup-close">x</a>
            </div>
            <div class="popup-content" id="popupOrderStatuscontent">
                <div id="pageOrderStatus">
                    <h1 id="hdrSalesOrder"><label id="lblOrderNo"></label></h1>
                    <div id="MainNav1a" class="content_div">
                        <div data-role="content" style="margin-top:10px">
                            <div class="row">
                                <div class="col-sm-6">
                                    <div name="divToolbar" style="float: left">
                                        <div id="butAddProduct" title="Add Product" class="btn-trail">
                                            <span class="erp-fc-add-product s48 bi-plus"></span><br />
                                            <span>Add Product</span>
                                        </div>

                                        @*<div name="butCheckAll" title="Serve All" class="btn-trail">
                <span class="erp-fc-check-all s48"></span><br />
                <span>Serve All</span>
            </div>

            <div name="butUncheckAll" title="Unserve All" class="btn-trail">
                <span class="erp-fc-uncheck-all s48"></span><br />
                <span>Unserve All</span>
            </div>*@

                                        <div name="butFinalizePickList" title="Save & Finalize" class="btn-trail">
                                            <span class="erp-op finalsave s48 bi-check-circle"></span><br />
                                            <span>Save & Finalize</span>
                                        </div>

                                        <div id="butPrintSopa" title="Output" class="btn-trail">
                                            <span class="erp-op print s48 bi-printer"></span><br />
                                            <span>Output</span>
                                        </div>

                                        <div name="butDownloadDocument" title="Download Document" class="btn-trail">
                                            <span class="erp-fc-download-document s48 bi-file-arrow-down"></span><br />
                                            <span>Download Document</span>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-6 border-" style="text-align:right">
                                    <div class="form-check">                                        
                                        <label class="form-check-label" for="chkShowRemovedItems">
                                            <input class="form-check-input" data-role="none" type="checkbox" id="chkShowRemovedItems">
                                            Show items removed from the picking list?
                                        </label>
                                    </div>
                                </div>
                            </div>
                            <br />
                            <div>
                                <h4>
                                    Information
                                </h4>
                                <div class="row">
                                    <div class="col-sm-6">
                                        <table class="table table-bordered">
                                            <tr>
                                                <td style="width:200px;">Customer:</td>
                                                <td><label id="headerClientName">[CustomerName]</label></td>
                                            </tr>
                                            <tr id="divSopaDeliveryAddress">
                                                <td style="width:200px;">Delivery Address:</td>
                                                <td><label id="headerClientDeliveryAddress">[CustomerAddress]</label></td>
                                            </tr>
                                            <tr id="divFcCustomerOutlet">
                                                <td style="width:200px;">Customer Outlet:</td>
                                                <td><label id="headerCustomerFowst">[CustomerFowst]</label></td>
                                            </tr>
                                            <tr id="divFcCustomerDepartment">
                                                <td style="width:200px;">Customer Department:</td>
                                                <td><label id="headerCustomerBin">[CustomerBin]</label></td>
                                            </tr>
                                            <tr id="divSopaBillingAddress">
                                                <td style="width:200px;">Billing Address:</td>
                                                <td><label id="headerClientBillingAddress">[BillingAddress]</label></td>
                                            </tr>
                                            <tr>
                                                <td>Remarks:</td>
                                                <td>
                                                    <div id="divRemarksLabel" style="text-align:left;">
                                                        <label id="lblRemarks"></label>
                                                        <a id="butChangeRemarks" href="" style="margin-left:10px">Change Remarks</a>
                                                    </div>
                                                    <div id="divRemarksTextbox" style="text-align:left; display:none">
                                                        <textarea id="txtRemarks" data-role="none" class="form-control" style="height:100px; resize:none;"></textarea>
                                                        <a id="butSaveRemarks" href="" style="margin-left:10px">Save Remarks</a>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr id="divSopaApprovedBy">
                                                <td style="width:200px;">Approved By:</td>
                                                <td><label id="lblApprovedBy"></label></td>
                                            </tr>
                                            <tr id="divDucSoNum">
                                                <td style="width:200px;">DUC Sales Order No.:</td>
                                                <td><label id="lblDucSoNum"></label></td>
                                            </tr>
                                        </table>
                                    </div>
                                    <div class="col-sm-6">
                                        <table class="table table-bordered">
                                            <tr>
                                                <td>FOWST where this order was encoded:</td>
                                                <td><label id="headerEncodingFowst"></label></td>
                                            </tr>
                                            <tr id="divFcServingFowst">
                                                <td>FOWST that will serve this order:</td>
                                                <td><label id="headerFowst"></label><a id="butChangeFowst" href="" style="margin-left:10px">Change</a></td>
                                            </tr>
                                            <tr>
                                                <td>Order Date:</td>
                                                <td><label id="headerInvoiceDate"></label></td>
                                            </tr>
                                            <tr>
                                                <td>Date Needed:<!--date needed--></td>
                                                <td><label id="headerDateNeeded"></label></td>
                                            </tr>
                                            <tr id="divFcDateOfFlush">
                                                <td>Date of Flush (If orders are from mobile app):</td>
                                                <td><label id="headerDateCreated"></label></td>
                                            </tr>
                                            <tr>
                                                <td>Delivery Date:</td>
                                                <td><label id="headerDeliveryDate"></label></td>
                                            </tr>
                                            <tr>
                                                <td>Encoded By:</td>
                                                <td><label id="headerEncodedBy"></label></td>
                                            </tr>
                                            <tr>
                                                <td>Last Modified By:</td>
                                                <td><label id="headerSalesRep"></label></td>
                                            </tr>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <br />

                        <table data-role="table" id="tblInvoice" class="ui-body-d ui-shadow table-stripe ui-responsive custom-tbl-responsive table table-light table-hover table-striped" data-column-btn-theme="b">
                            <thead class="custom-theme-f">
                                <tr>
                                    <th>Stock No.</th>
                                    <th>Stock Description</th>
                                    <th>Vendor</th>
                                    <th>PO Status</th>
                                    <th style="text-align: center">Order Qty</th>
                                    <th style="text-align: center">Served Qty</th>
                                    <th>Unit Measure</th>
                                    @*<th style="text-align: right">Unit Price</th>
                                    <th style="text-align: right">Sub Total</th>*@
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody id='tblInvoiceBody'></tbody>
                        </table>
                        <br />
                        <div data-role="content">
                            <div style="text-align: right;"><label id="lblTotalOrderedQty"></label></div><br />
                            <div style="text-align: right;"><label id="lblTotalServedQty"></label></div><br />
                            @*<div style="text-align: right;"><label id="lblSKUCount"></label></div><br />
                            <div style="text-align: right;"><label id="lblTotal" style="font-size: x-large"></label></div>*@

                            <br />
                        </div>
                    </div>

                    <div id="MainNav1b" class="content_div">
                        <div data-role="content">
                            Signatory: <label id="lblSignatory"></label>
                            <br /><br />
                            <img id="imgSignature" alt="" />
                            <br />
                        </div>
                    </div>

                    <div id="MainNav1c" class="content_div">
                        <div data-role="content">
                            <div class="ui-grid-a">
                                <div class="ui-block-a" style="width:30%;vertical-align:middle">
                                    <label for="selInvoiceType" style="line-height: 3;">Invoice Type: <span style="color: #FF0000;">*</span></label>
                                </div>
                                <div class="ui-block-b" style="text-align:right;width:70%">
                                    <select id="selInvoiceType">
                                        <option value="Charge Invoice">Charge Invoice</option>
                                        <option value="Add New">Add New...</option>
                                    </select>
                                </div>
                            </div>
                            <div class="ui-grid-a">
                                <div class="ui-block-a" style="width:30%">
                                    <label for="txtInvoiceNo" style="line-height: 3;">Invoice Number: <span style="color: #FF0000;">*</span></label>
                                </div>
                                <div class="ui-block-b" style="text-align:right;width:70%">
                                    <input type="text" id="txtInvoiceNo" maxlength="15" />
                                </div>
                            </div>
                            <div class="ui-grid-a">
                                <div class="ui-block-a" style="width:30%">
                                    <label for="txtRemarks" style="line-height: 3;">Remarks:</label>
                                </div>
                                <div class="ui-block-b" style="text-align:right;width:70%">
                                    <textarea id="txtRemarks" cols="5" rows="5" maxlength="255"></textarea>
                                </div>
                            </div>
                            <br />
                            <div style="text-align: center">
                                <a id="btnSaveInvoiceDetails" data-role="button" data-theme="e">
                                    <h3>
                                        Save
                                    </h3>
                                </a>
                            </div>
                            <div style="text-align: center;">
                                <a id="btnPrintInvoiceDetails" data-role="button" data-theme="e">
                                    <h3>
                                        Print Invoice
                                    </h3>
                                </a>
                            </div>
                            <div data-role="popup" id="pageInvoiceType" data-theme="b" class="ui-corner-all">
                                <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
                                <center><h3>Type of Invoice</h3></center>
                                <div data-role="content">

                                    <label for="txtInvoiceType">New Invoice Type:</label>
                                    <input type="text" id="txtInvoiceType" />

                                    <label for="txtInvoiceTypeAbbr">New Invoice Type Abbreviation:</label>
                                    <input type="text" id="txtInvoiceTypeAbbr" />

                                    <br />
                                    <button id="btnAddInvoiceType" data-icon="add">Add</button>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="MainNav1d" class="content_div">
                        <div data-role="content">
                            <table data-role="table" id="tblStatus" data-mode="reflow" class="ui-responsive myTable custom-tbl-responsive">
                                <thead class="custom-theme-f">
                                    <tr style="cursor: pointer">
                                        <th data-sort="date">Date</th>
                                        <th data-sort="string">Action</th>
                                        <th data-sort="string">User</th>
                                    </tr>
                                </thead>
                                <tbody id='tblStatusTable'></tbody>
                            </table>
                            <hr />  <br />
                            <div class="ui-grid-a">
                                <div class="ui-block-a" style="width: 33%;padding-left:8px;">
                                    No. of Downloads:
                                </div>
                                <div class="ui-block-b" style="width: 5%;text-align:right">
                                    <label id="lblDownloadCount"></label>
                                </div>
                            </div>  <br />
                            <div class="ui-grid-a">
                                <div class="ui-block-a" style="width: 33%;padding-left:8px;">
                                    No. of Prints:
                                </div>
                                <div class="ui-block-b" style="width: 5%;text-align:right">
                                    <label id="lblPrintCount"></label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="popup-footer align-right">
                <button id="" data-role="none" class="btn custom-btn-theme-c btnCircle popup-close">
                    <img alt="Options" src="/Content/images/close-black.png" />
                </button>
            </div>
        </div>
    </div>

    <div id="popProductQtyEdit-bs" class="modal fade" style="display:none;">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Edit Quantity</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body" id="popProductQtyEditcontent-bs">
                </div>
            </div>
        </div>
    </div>

    <div data-role="popup" id="popProductQtyEdit" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="btnClosePopUp">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;">Edit Quantity</h3></center>
        <div data-role="content" class="ui-content ui-body-d jPopupContent" id="popProductQtyEditcontent">
            <div>
                <label name="lblQty">Qty</label>
                <input name="txtQtyEdit" class="form-control" />
            </div>
            <br /><br />
            <button name="butSave" data-role="none" class="btn custom-btn-theme-b btn-success btn-block"> Save </button>
        </div>
    </div>
</div>