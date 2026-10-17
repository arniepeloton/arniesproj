<script type="text/javascript" src="/Scripts/PrintPDF/require.js"></script>
<script type="text/javascript" src="/Scripts/sources/BOMProjectExcel/GeneratePDF.js"></script>

<script type="text/javascript">
    require([GetPathFormat('/Scripts/sources/BOMProjectExcel/MaterialsRequisition.js?crc=')
    ]);
</script>

<div>
    <div>
        <div id="popMaterialsRequisition" custom-data-role="popup">
            <div class="popup">
                <div class="popup-header">
                    Materials Requisition Approval
                    <a class="popup-close">x</a>
                </div>

                <div class="popup-content">
                    <input data-role="none" class="txtRequisitionID" type="text" style="display:none;" />
                    <input data-role="none" class="txtProjectID" type="text" style="display:none;" />
                    <div class="custom-ui-label">
                        <div>Requisition Number:</div>
                        <div id="txtRequisitionNumber" style="font-weight: bold;"></div>
                    </div>

                    <div class="custom-ui-label">
                        <div>Project Name:</div>
                        <div id="txtProjectName" style="font-weight: bold;"></div>
                    </div>

                    <div class="custom-ui-label">
                        <div>Requisition Date and Time:</div>
                        <div id="txtDateTime" style="font-weight: bold;"></div>
                    </div>

                    <div class="custom-ui-label">
                        <div>Requested By:</div>
                        <div id="txtRequestedBy" style="font-weight: bold;"></div>
                    </div>

                    <div class="custom-ui-label">
                        <div>Status:</div>
                        <div id="txtStatus" style="font-weight: bold;"></div>
                    </div>

                    <div style="overflow:auto;">
                        <table data-role="table" data-mode="reflow" style="display:table;" cellpadding="2" cellspacing="0" class="ui-table ui-responsive table-stripe table-stroke custom-tbl-responsive" width="100%">
                            <thead>
                                <tr class="custom-theme-c txt-normal">
                                    <th>#</th>
                                    <th>Project Grouping</th>
                                    <th>SKU Name</th>
                                    <th>UoM</th>
                                    <th>Budgeted Qty</th>
                                    <th>Acc. Requested Qty</th>
                                    <th>Acc. Approved Qty</th>
                                    <th>Acc. Received Qty</th>
                                    <th>Remaining Budgeted Qty</th>
                                    <th>Requested Qty</th>
                                    <th>Vendor</th>
                                </tr>
                            </thead>

                            <tbody id="tbodyRequestedMaterials"></tbody>
                        </table>
                    </div>
                </div>

                <div class="popup-footer align-right">
                    <button data-role="none" id="btnCancelMatReq" class="btn custom-btn-theme-c btn-less-width">
                        Cancel
                    </button>
                    <button data-role="none" id="btnDisapprove" class="btn custom-btn-theme-d btn-less-width">
                        Disapprove
                    </button>
                    <button data-role="none" id="btnApprove" class="btn custom-btn-theme-b btn-less-width">
                        Approve
                    </button>
                </div>
            </div>
        </div>        

        <div id="popMaterialsRequisitionApproved" custom-data-role="popup">
            <div class="popup">
                <div class="popup-header">
                    Materials Requisition
                    <a class="popup-close">x</a>
                </div>

                <div class="popup-content">
                    <input data-role="none" class="txtRequisitionID" type="text" style="display:none;" />
                    <input data-role="none" class="txtProjectID" type="text" style="display:none;" />

                    <div class="custom-ui-label">
                        <div>Requisition Number:</div>
                        <div id="txtAppRequisitionNumber" style="font-weight: bold;"></div>
                    </div>

                    <div class="custom-ui-label">
                        <div>Project Name:</div>
                        <div id="txtAppProjectName" style="font-weight: bold;"></div>
                    </div>

                    <div class="custom-ui-label">
                        <div>Requisition Date and Time:</div>
                        <div id="txtAppDateTime" style="font-weight: bold;"></div>
                    </div>

                    <div class="custom-ui-label">
                        <div>Requested By:</div>
                        <div id="txtAppRequestedBy" style="font-weight: bold;"></div>
                    </div>

                    <div class="custom-ui-label">
                        <div>Status:</div>
                        <div style="font-weight: bold; color:#2A78BB;">Approved on <span id="txtApprovedDate"></span></div>
                    </div>

                    <div style="overflow:auto;">
                        <table data-role="table" data-mode="reflow" style="display:table;" cellpadding="2" cellspacing="0" class="ui-table ui-responsive table-stripe table-stroke custom-tbl-responsive" width="100%">
                            <thead>
                                <tr class="custom-theme-c txt-normal">
                                    <th>#</th>
                                    <th>Project Grouping</th>
                                    <th>SKU Name</th>
                                    <th>Budgeted Qty</th>
                                    <th>Requested Qty</th>
                                    <th>Approved Qty</th>
                                    <th>Unit Of Measure</th>
                                    <th>Vendor</th>
                                </tr>
                            </thead>

                            <tbody id="tbodyAppRequestedMaterials"></tbody>
                        </table>
                    </div>
                </div>

                <div class="popup-footer align-right">
                    <button data-role="none" id="btnCloseReq" class="btn custom-btn-theme-c btn-less-width">
                        Close
                    </button>
                    <button data-role="none" id="btnDownloadReq" class="btn custom-btn-theme-b btn-less-width">
                        Download Picking List
                    </button>
                    <button data-role="none" id="btnCreatePO" style="display:none;" class="btn custom-btn-theme-b btn-less-width">
                        Create PO
                    </button>
                </div>
            </div>
        </div>        

        <div data-role="popup" id="popMakePO" data-overlay-theme="a" data-theme="a" data-corners="false">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Purchase Order</h3></center>
            <div data-role="content">
                <input type="text" data-role="none" style="display:none;" id="txtMakePOReqID" />
                <div data-role="fieldcontain">
                    <label for="selInvoiceType">Invoice Type:</label>
                    <select id="selInvoiceType">
                        <option value="CHI-CAS">CHI-CAS</option>
                        <option value="TR">TR</option>
                    </select>
                </div>
                <div data-role="fieldcontain">
                    <label>Delivery Date: <span style="color:#F00;">*</span>
                        <span style="color:#F00; display:none;" id="spanValidationDelDate">This field is required</span>
                    </label>
                    <button id="btnDeliveryDate">Set Date</button>
                </div>
                <div data-role="fieldcontain">
                    <label for="selBillingAddress">Billing Address:</label>
                    <select id="selBillingAddress">
                        
                    </select>
                </div>
                <div data-role="fieldcontain">
                    <label for="selDeliveryAddress">Delivery Address:</label>
                    <select id="selDeliveryAddress">
                        
                    </select>
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

        <div id="popPendingRequest" custom-data-role="popup">
            <div class="popup-md">
                <div class="popup-header">
                    Pending Request
                    <a class="popup-close">x</a>
                </div>

                <div class="popup-content">
                    <div class="custom-ui-label">
                        <div>Material Name:</div>
                        <div id="txtReqMaterialName" style="font-weight: bold;"></div>
                    </div>

                    <div style="overflow:auto;">
                        <table data-role="table" data-mode="reflow" style="display:table;" cellpadding="2" cellspacing="0" class="ui-table ui-responsive table-stripe table-stroke custom-tbl-responsive" width="100%">
                            <thead>
                                <tr class="custom-theme-c txt-normal">
                                    <th>#</th>
                                    <th>Requisition #</th>
                                    <th>Requested Value</th>
                                    <th>Status</th>
                                    <th>Requested By</th>
                                    <th>Requested Date</th>
                                </tr>
                            </thead>

                            <tbody id="tbodyPendingRequest"></tbody>
                        </table>
                    </div>
                </div>

                <div class="popup-footer align-right">
                    <button data-role="none" class="btn custom-btn-theme-c btn-less-width popup-close">
                        Cancel
                    </button>
                </div>
            </div>
        </div>        
    </div>

    <div id="divHeaderMaterialRequisition" data-role="header" data-position="fixed1" style="background: none white; border: none; display:none;">
        <div class="ui-grid-d custom-theme-a txt-center">
            <div id="btnHome" class="ui-block-a btn custom-btn-theme-a" style="height:80px;">
                <div style="margin: 0px; font-size:15px;">
                    <img alt="Home" src="/Content/images/home.png" style="width:30px;" />
                    <br />
                    Home
                </div>
            </div>
            @*<div id="btnProjectName" class="ui-block-b btn custom-btn-theme-a" style="white-space:normal; height:80px;">
                    <div style="margin:0px; font-size:15px;">
                        <img alt="Project Name" src="/Content/images/home.png" style="width:30px;" />
                        <br />
                        <span id="txtProjecName">Project Name</span>
                    </div>
                </div>*@
            <div id="btnDashboard" class="ui-block-b btn custom-btn-theme-a" style="white-space:normal; height:80px;">
                <div style="margin:0px; font-size:15px;">
                    <img alt="Dashboard" src="/Content/images/dashboard.png" style="width:30px;" />
                    <br />
                    <span>Material Requisition Approval</span>
                </div>
            </div>
            <div class="ui-block-b">
                <div style="margin:0px;">
                </div>
            </div>
            <div class="ui-block-d">
                <div style="margin: 0px;">

                </div>
            </div>
            <div id="btnMaterialsRequisition2" class="ui-block-e btn custom-btn-theme-b" style="white-space:normal; height:80px;">
                <div style="margin:0px; font-size:15px;">
                    <img alt="Materials Requisition" src="/Content/images/plus-white.png" />
                    <br />
                    <span>Create Materials Requisition</span>
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

    <div id="divMatReqLoading" subpage="pageMaterialRequisition" style="display:block;">
        Loading, please wait ...
    </div>

    <div subpage="pageMaterialRequisition" id="pageDashboardMaterialsRequisiton" style="display:none;">
        <div data-role="">
            <div>
                <div style="float:left">
                    Filter by: <select data-role="none" id="selProjectNameDashboard" class="selProjectName select custom-theme-c"></select>
                </div>
                <div style="float:right">
                    <input type="search" id="srchReqMat" placeholder="Type to search ..." />
                </div>
                <div style="clear:both;"></div>
            </div>

            <br />
            
            <div style="overflow:auto; display:block;">
                <table id="tblRequisition" data-role="table" data-mode="reflow" style="display:table;" cellpadding="2" cellspacing="0" class="ui-table ui-responsive table-stripe table-stroke custom-tbl-responsive" width="100%">
                    <thead>
                        <tr class="custom-theme-f">
                            <th>#</th>
                            <th>Requisition Number</th>
                            <th>Requisition Date And Time</th>
                            <th>Project Name</th>
                            <th>Requested By</th>
                            <th>Requisition Value</th>
                            <th>Status</th>
                            @*<th></th>*@
                        </tr>
                    </thead>

                    <tbody id="tbodyRequisition">
                        <tr>
                            <td colspan="9" style="text-align:center;">No Result(s) Found</td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <div class="fixed-ActionBtn" style="display:block;">
                <button id="btnMaterialsRequisition" data-role="none" class="btn custom-btn-theme-d btnCircle">
                    <img class="img-plus-white" style="width:18px;" />
                </button>
                <ol class="secondaryActionBtn">
                    <li>
                        <button title="Refresh" id="btnRefreshMaterialReq" data-role="none" class="btn custom-btn-theme-e btnCircle-sm">
                            <span class="glyphicon glyphicon-refresh"></span>
                        </button>
                    </li>
                </ol>
            </div>
            @*<a data-role="button" id="btnPDF">pdf</a>*@
        </div>
    </div>

    <div id="pageNewMaterialsRequisiton" custom-data-role="popup">
        <div class="popup">
            <div class="popup-header">
                Add New Request
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content full-height">
                <div class="custom-ui-label">
                    <div>Project Name:</div>
                    <div>
                        <select id="selProjectNameMatReq" class="selProjectName"></select>
                    </div>
                </div>
                <div id="divMainParentHotButton" style="display:none;">
                    <div id="divParentHotButtons" class="highlights" style="overflow:auto; white-space:nowrap;">
                        <button id="btnMainHotButton" data-role="none" class="btn custom-btn-theme-b btn-sm btn-less-width">Main</button>
                    </div>
                </div>

                <div id="divMainHotButtons" style="display:none; text-align:center;">

                </div>

                <div id="divSubHotButtons" style="display:none;">

                </div>

                <div id="divItemCount" style="display:none;">
                    <div data-role="content">
                        <h3 id="txtParentItemName" style="text-transform:capitalize;"></h3>
                        <input data-role="none" type="text" id="txtRequisitionValue" style="display:none;" />

                        <div style="overflow:auto; display:block;" id="divCanvassLabor">
                            <table data-mode="reflow" cellpadding="3" cellspacing="0" class="custom-table table-stroke" width="100%">
                                <thead>
                                    <tr class="custom-theme-f">
                                        <th style="min-width: 80px; max-width:80px;">Item No.</th>
                                        <th style="min-width: 300px; max-width:300px;">Description</th>
                                        <th style="min-width: 100px; min-width:100px;">Budgeted Qty</th>
                                        <th style="min-width: 100px; min-width:100px;">Unit</th>
                                        <th style="min-width: 100px; min-width:100px;">Remaining Budgeted Qty</th>
                                        <th style="min-width: 100px; min-width:100px;">No. Of Materials Needed</th>
                                        <th style="min-width: 100px; min-width:100px;">Vendor</th>
                                    </tr>
                                </thead>

                                <tbody id="tBodyMaterialsCount"></tbody>
                            </table>
                        </div>
                        <br />
                        <br />
                        <button data-role="none" class="btn custom-btn-theme-b btn-large" id="btnSaveMaterialsRequisition">Save</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="divPrintApprovedMaterials" style="visibility:hidden; display:none;">
        <div class="custom-ui-label">
            <div>Requisition Number:</div>
            <div id="txtPrintRequisitionNumber" style="font-weight: bold;"></div>
        </div>

        <div class="custom-ui-label">
            <div>Project Name:</div>
            <div id="txtPrintProjectName" style="font-weight: bold;"></div>
        </div>

        <div class="custom-ui-label">
            <div>Date and Time:</div>
            <div id="txtPrintDateTime" style="font-weight: bold;"></div>
        </div>

        <div class="custom-ui-label">
            <div>Requested By:</div>
            <div id="txtPrintRequestedBy" style="font-weight: bold;"></div>
        </div>

        <div id="divTbl" style="overflow:auto;">
            <table data-mode="reflow" cellpadding="2" cellspacing="0" class="custom-table table-stroke" width="100%">
                <thead>
                    <tr class="custom-theme-c txt-normal">
                        <th>#</th>
                        <th>Project Grouping</th>
                        <th>SKU Name</th>
                        <th>Unit Of Measure</th>
                        <th>Approved Qty</th>
                        <th>Vendor</th>
                    </tr>
                </thead>

                <tbody id="tbodyPrintRequestedMaterials"></tbody>
            </table>
        </div>
    </div>
</div>