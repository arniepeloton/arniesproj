<link rel="stylesheet" href="/Content/flatui-css-custom.css" />
<link rel="stylesheet" href="/Content/metro-calendar.css" />

<script type="text/javascript" src="/Scripts/sources/BOMProjectExcel/GeneratePDF.js"></script>

<script type="text/javascript">
    require([GetPathFormat('/Scripts/sources/BOMProjectExcel/PurchaseOrder.js?crc=')
    ]);
</script>
<div>
    <div id="popPO" custom-data-role="popup">
        <div class="popup">
            <div class="popup-header">
                Purchase Order
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">
                <input data-role="none" id="txtRequisitionID" type="text" style="display:none;" />
                <input data-role="none" id="txtProjectID" type="text" style="display:none;" />
                <div class="custom-ui-label">
                    <div>PO Number:</div>
                    <div id="txtPONumber" style="font-weight: bold;"></div>
                </div>

                <div class="custom-ui-label">
                    <div>Project Name:</div>
                    <div id="txtPOProjectName" style="font-weight: bold;"></div>
                </div>

                <div class="custom-ui-label">
                    <div>Supplier Name:</div>
                    <div id="txtSupplierName" style="font-weight: bold;"></div>
                </div>

                <div class="custom-ui-label">
                    <div>Supplier Address:</div>
                    <div id="txtSupplierAddress" style="font-weight: bold;"></div>
                </div>

                <div style="overflow:auto;">
                    <table data-role="table" data-mode="reflow" style="display:table;" cellpadding="2" cellspacing="0" class="ui-table ui-responsive table-stripe table-stroke custom-tbl-responsive" width="100%">
                        <thead>
                            <tr class="custom-theme-c txt-normal">
                                <th>#</th>
                                <th>Item Code</th>
                                <th>Description</th>
                                <th>Qty</th>
                                <th>Unit Measure</th>
                                <th>Price</th>
                                <th>Sub Total</th>
                            </tr>
                        </thead>

                        <tbody id="tbodyPO"></tbody>
                    </table>
                    <div id="txtTotalPO" style="float:right; font-weight:bold; font-size:18px;">

                    </div>
                </div>
            </div>

            <div class="popup-footer align-right">
                <button data-role="none" id="btnCancelPO" class="btn custom-btn-theme-c btn-less-width">
                    Close
                </button>
                <button data-role="none" id="btnDownloadPO" class="btn custom-btn-theme-b btn-less-width">
                    Downloand P.O
                </button>
            </div>
        </div>

        <div data-role="popup" id="popMakePO" data-overlay-theme="a" data-theme="a" data-corners="false">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Purchase Order</h3></center>
            <div data-role="content">
                <input type="text" data-role="none" style="display:none;" id="txtMakePOReqID" />
                <div data-role="fieldcontain" style="display:none;">
                    <label for="selInvoiceType">Invoice Type:</label>
                    <select id="selInvoiceType">
                        <option value="CHI-CAS">CHI-CAS</option>
                        <option value="TR">TR</option>
                    </select>
                </div>
                <div data-role="fieldcontain">
                    <label>
                        Delivery Date: <span style="color:#F00;">*</span>
                        <span style="color:#F00; display:none;" id="spanValidationDelDate">This field is required</span>
                    </label>
                    <button id="btnDeliveryDate">Set Date</button>
                </div>
                <div data-role="fieldcontain">
                    <label for="selBillingAddress">Billing Address:</label>
                    <select id="selBillingAddress"></select>
                </div>
                <div data-role="fieldcontain">
                    <label for="selDeliveryAddress">Delivery Address:</label>
                    <select id="selDeliveryAddress"></select>
                </div>
                <div data-role="fieldcontain">
                    <label for="txtRemarks">Remarks:</label>
                    <textarea id="txtRemarks"></textarea>
                </div>

                <br />

                <button data-role="none" id="btnOkMakePO" class="btn custom-btn-theme-b">Ok</button>
                <br /><br />
                <button data-role="none" id="btnCancelMakePO" data-rel="back" class="btn custom-btn-theme-c">Cancel</button>
            </div>
        </div>
    </div>

    <div id="divHeaderProjectPO" data-role="header" data-position="fixed1" style="background: none white; border: none; display:none;">
        <div class="ui-grid-d custom-theme-a txt-center">
            <div id="btnHome" class="ui-block-a btn custom-btn-theme-a" style="height:80px;">
                <div style="margin: 0px; font-size:15px;">
                    <img alt="Home" src="/Content/images/home.png" style="width:30px;" />
                    <br />
                    Home
                </div>
            </div>
            <div class="ui-block-b" style="white-space:normal; height:80px;">
                <div style="margin:0px; font-size:15px;">
                    @*<img alt="Dashboard" src="/Content/images/dashboard.png" style="width:30px;" />
                        <br />
                        <span>Dashboard</span>*@
                </div>
            </div>
            <div class="ui-block-b" style="white-space:normal; height:80px;">
                <div style="margin:0px; font-size:15px;">
                    @*<img alt="Materials Requisition" src="/Content/images/dashboard.png" style="width:30px;" />
                        <br />
                        <span>Materials Requisition</span>*@
                </div>
            </div>
            <div class="ui-block-d">
                <div style="margin: 0px;">

                </div>
            </div>
            <div id="btnNewPO" class="ui-block-e btn custom-btn-theme-b" style="height:80px;">
                <div style="margin: 0px; font-size:15px;">
                    <img alt="Create New PO" src="/Content/images/plus-white.png" />
                    <br />
                    <span>Create New PO</span>
                </div>
            </div>
            @*<div class="ui-block-e btn custom-btn-theme-g" style="height:80px;">
                    <div id="btnSaveMaterialsRequisition" style="margin: 0px; font-size:15px;">
                        <img alt="Go" src="/Content/images/save.png" style="width:26px;" />
                        <br />
                        Save
                    </div>
                </div>*@
        </div>
    </div>

    <div subpage="pagePurchaseOrder" id="pagePOLoading" style="display:block;">
        Loading, please wait ...
    </div>

    <div subpage="pagePurchaseOrder" id="pageDashboardPurchaseOrder" style="display:none;">
        <div data-role="content">
            <div>
                <div style="float:left">
                    Filter by: 
                    <select data-role="none" id="selProjectList" class="select custom-theme-c">
                        <option>Filter By Project</option>
                    </select>
                </div>
                <div style="float:right">
                    <input type="search" id="txtSearchPO" placeholder="Search PO No., Project Name, Supplier Name, Delivery Date" />
                </div>
                <div style="clear:both;"></div>
            </div>

            <br />

            <div style="overflow:auto; display:block;">
                <table id="tblProjectPO" data-role="table" data-mode="reflow" style="display:table;" cellpadding="2" cellspacing="0" class="ui-table ui-responsive table-stripe table-stroke custom-tbl-responsive" width="100%">
                    <thead>
                        <tr class="custom-theme-f">
                            <th>#</th>
                            <th>PO Number</th>
                            <th>Requisition Number</th>
                            <th>Project Name</th>
                            <th>Supplier Name</th>
                            <th>Requested By</th>
                            <th>Delivery Date</th>
                            <th>Amount</th>
                            <th>Date and Time Created</th>
                        </tr>
                    </thead>

                    <tbody id="tbodyPurchaseOrder">
                        <tr>
                            <td colspan="10" style="text-align:center;">No Result(s) Found</td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <div class="fixed-ActionBtn" style="display:block;">
                <button id="btnRefreshProjectPO" data-role="none" class="btn custom-btn-theme-d btnCircle">
                    <span class="glyphicon glyphicon-refresh"></span>
                </button>
            </div>
        </div>
    </div>
</div>