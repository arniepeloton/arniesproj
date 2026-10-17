<script>
    require([GetPathFormat('/Scripts/sources/Receiving/ReceivingAndTransfersReport.js?crc=' + assetversion)]);
</script>

<style>
    .btnRequired {
    -webkit-transition: box-shadow 1s linear;
    -moz-transition: box-shadow 1s linear;
    -ms-transition: box-shadow 1s linear;
    -o-transition: box-shadow 1s linear;
    transition: box-shadow 1s linear;
    border: 2px solid #FFF;
    }
    .btnRequired {
        border: 2px solid #F00;
    }
    
    .btnNotRequired 
    {
        border:2px solid #FFF;   
    }
    
    .ui-table-cell-label-top
    {
        width:100%;   
    }
</style>

<div id="pageReceivingAndTransfersReport">
    
    <div data-role="content">

        <div style="display:block">
            <div class="row">
                <div class="col-6 col-md-4 col-lg-auto mb-3 order-0 order-md-0">
                    Date:<br />
                    <button id="btnReceivingTransferDateRange" style="margin-right:10px;" data-role="none" class="btn custom-btn-theme-c btn-secondary btn-block">Select Date</button>
                </div>
                <div class="col-12 col-md-4 col-lg-auto mb-3 order-3 order-md-1">
                    Transaction Type:<br />
                    <select id="selReceivingTransferTransType" data-role="none" class="form-control form-select">
                        <option value="">Please Transaction Type</option>
                        <option value="Stock Receiving and Transfers">Stock Receiving and Transfers</option>
                        <option value="Stock Receiving">Stock Receiving</option>
                        <option value="Stock Transfers">Stock Transfers</option>
                    </select>
                </div>
                <div class="col-6 col-md-4 col-lg-auto mb-3 order-1 order-md-2">
                    Select FOWST:<br />
                    <button id="btnRecevingTransferFowst" style="margin-right:10px;" data-role="none" class="btn custom-btn-theme-c btn-secondary btn-block">
                        <span id="txtRecTransFowstName">FOWST</span>
                    </button>
                </div>
            </div>
            <div style="display:none;">
                <input type="text" id="txtReceivingTransferFowstID" readonly />
                <input type="text" id="txtReceivingTransferFowstVendorID" readonly />
            </div>
        </div>

        <br /> <br /> 

        <div id="divFortableReceivingAndTransfers" style="display:none; overflow:auto;">
            <table id="tableReceivingAndTransfers" data-role="none" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive table table-light table-striped table-hover headerfitwidth">
                <colgroup>
                    <col style="border:1px solid #B8B8B8;">
                    <col span="3" style="cursor:pointer; border:1px solid #B8B8B8;">
                    <col span="3" style="border:1px solid #B8B8B8;">
                </colgroup>
                <thead>
                    <tr class="custom-theme-f">
                        <th></th>
                        <th colspan="3" style="background-color:#335F8E; text-align:center; color:white;"><span id="lblFowst"> From A to B </span></th>
                        <th class="table2" colspan="3" style="background-color:#454545; text-align:center; display:none; color:white;"><span id="lblFowst2"> From B to A </span></th>
                    </tr>
                    <tr>
                        <th></th>
                        <th>Trans Count</th>
                        <th>SKU Count</th>
                        <th>Trans Value in COGS</th>
                        <th class="table2" style="display:none;">Trans Count</th>
                        <th class="table2" style="display:none;">SKU Count</th>
                        <th class="table2" style="display:none;">Trans Value in COGS</th>
                    </tr>
                </thead>

                <tbody id="tBodyFowstReportSummary">
                    <tr id="tRowStockTransfer" name="Stock Transfers">
                        <td>Stock Transfer</td>
                        <td class="btnSubReportTransT1" style="cursor:pointer;" id="colgroupATransTransCount"></td>
                        <td class="btnSubReportTransT1" style="cursor:pointer;" id="colgroupATransSKUCount"></td>
                        <td class="btnSubReportTransT1" style="cursor:pointer;" id="colgroupATransCOGS"></td>
                        <td id="colgroupBTransTransCount" class="table2 btnSubReportTransT2" style="display:none; cursor:pointer;"></td>
                        <td id="colgroupBTransSKUCount" class="table2 btnSubReportTransT2" style="display:none; cursor:pointer;"></td>
                        <td id="colgroupBTransCOGS" class="table2 btnSubReportTransT2" style="display:none; cursor:pointer;"></td>
                    </tr>
                    <tr id="tRowStockReceiving" name="Stock Receiving">
                        <td>Stock Receiving</td>
                        <td class="btnSubReportRecT1" style="cursor:pointer;" id="colgroupARecTransCount"></td>
                        <td class="btnSubReportRecT1" style="cursor:pointer;" id="colgroupARecSKUCount"></td>
                        <td class="btnSubReportRecT1" style="cursor:pointer;" id="colgroupARecCOGS"></td>
                        <td id="colgroupBRecTransCount" class="table2 btnSubReportRecT2" style="display:none; cursor:pointer;"></td>
                        <td id="colgroupBRecSKUCount" class="table2 btnSubReportRecT2" style="display:none; cursor:pointer;"></td>
                        <td id="colgroupBRecCOGS" class="table2 btnSubReportRecT2" style="display:none; cursor:pointer;"></td>
                    </tr>
                    <tr id="tRowVariance">
                        <td>Variance</td>
                        <td id="colgroupAVarianceTransCount"></td>
                        <td id="colgroupAVarianceSKUCount"></td>
                        <td id="colgroupAVarianceCOGS"></td>
                        <td id="colgroupBVarianceTransCount" class="table2" style="display:none;"></td>
                        <td id="colgroupBVarianceSKUCount" class="table2" style="display:none;"></td>
                        <td id="colgroupBVarianceCOGS" class="table2" style="display:none;"></td>
                    </tr>
                </tbody>
            </table>
        </div>

        <br />

        <div id="dvSummaryFromTo1" style="width:100%; overflow:auto; display:none;">
            <table data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe table table-light table-striped table-hover headerfitwidth">
                <thead>
                    <tr id="tblHeader" class="custom-theme-f">
                        <th data-sort="string" style="cursor:pointer;"><span id="lblFOWST"> </span></th>
                        <th data-sort="string" style="cursor:pointer;">Trans Count</th>
                        <th data-sort="float" style="cursor:pointer;">SKU Count</th>
                        <th data-sort="date" style="cursor:pointer;">Total Value in COGS</th>
                    </tr>

                </thead>
                <tbody id="tbFOWSTReportSummary"></tbody>
            </table>
            <br /><br />
        </div>
        
        <div id="dvSummaryFromTo2" style="width:100%; overflow:auto; display:none;">
            <table data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe table table-light table-striped table-hover headerfitwidth">
                <thead>
                    <tr id="tblHeader" class="custom-theme-f">
                        <th data-sort="string" style="cursor:pointer;"><span id="lblFOWST2"> </span></th>
                        <th data-sort="string" style="cursor:pointer;">Trans Count</th>
                        <th data-sort="float" style="cursor:pointer;">SKU Count</th>
                        <th data-sort="date" style="cursor:pointer;">Total Value in COGS</th>
                    </tr>

                </thead>
                <tbody id="tbFOWSTAndOrganizationReportSummary"></tbody>
            </table>
            <br /><br />
        </div>

        <div id="dvSummaryFromTo3" style="width:100%; overflow:auto; display:none;">
            <h3 id="txtStockType"></h3>
            <table data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive table table-light table-striped table-hover headerfitwidth">
                <thead>
                    <tr id="tblHeader" class="custom-theme-f">
                        <th data-sort="string" style="cursor:pointer;">Trans Type</th>
                        <th data-sort="string" style="cursor:pointer;">Trans Ref</th>
                        <th data-sort="string" style="cursor:pointer;">Trans Date</th>
                        <th data-sort="date" style="cursor:pointer;">From</th>
                        <th data-sort="date" style="cursor:pointer;">To</th>
                        <th data-sort="date" style="cursor:pointer;">Remarks</th>
                        <th data-sort="date" style="cursor:pointer;">NetSuite No.</th>
                        <th data-sort="date" style="cursor:pointer;">NetSuite Error</th>
                        <th data-sort="date" style="cursor:pointer;">NetSuite DateTime</th>
                        @*<th data-sort="string" style="cursor:pointer;"><span id="lblReceivingAndTransFersSummaryFrom"></span></th>
                    <th data-sort="string" style="cursor:pointer;"><span id="lblReceivingAndTransFersSummaryTo"></span></th>*@
                        @*<th data-sort="string" style="cursor:pointer;">Trans Date</th>
                    <th data-sort="string" style="cursor:pointer;">Type</th>
                    <th data-sort="float" style="cursor:pointer;">Ref No.</th>
                    <th data-sort="date" style="cursor:pointer;">From</th>
                    <th data-sort="date" style="cursor:pointer;">To</th>
                    <th data-sort="date" style="cursor:pointer;">No. of SKUs</th>
                    <th data-sort="date" style="cursor:pointer;">Value in COGS</th>
                    <th data-sort="string" style="cursor:pointer;">Remarks</th>*@
                    </tr>

                </thead>
                <tbody id="tbFOWSTAndOrganizationSubReportSummary"></tbody>
            </table>
            <br /><br />
        </div>

        <div id="divNoPage" style="display:block; text-align:center">
            <h4>No Data Available</h4>
        </div>

    </div>

    <div id="divFortableReceivingAndTransfersDetails-bs" data-backdrop="true" class="modal fade" style="display:none;">
        <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Receiving and Transfers Product Details</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body" id="divFortableReceivingAndTransfersDetailscontent-bs">

                </div>
            </div>
        </div>
    </div>

    <div data-role="popup" style="display:none;" id="divFortableReceivingAndTransfersDetails" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3>&nbsp;Receiving and Transfers Product Details&nbsp;</h3></center>
        <div data-role="content" id="divFortableReceivingAndTransfersDetailscontent">
            <div class="row ui-responsive">
                <div class="col-sm-6" id="divReceivingAndTransfersDetails1">
                    <div class="ui-bar">
                        <h3 id="lblReceivingAndTransFersSummaryFrom"> </h3>
                        <br />
                        <table id="tableReceivingAndTransfersDetails1" data-role="table" style="display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive table table-light table-striped table-hover headerfitwidth">
                            <thead>
                                <tr>
                                    <th>Product</th>
                                    <th>Unit Measure</th>
                                    <th>Quantity</th>
                                </tr>
                            </thead>

                            <tbody id="tBodyReceivingAndTransfersDetails1"></tbody>
                        </table>

                    </div>
                </div>
                <div class="col-sm-6" id="divReceivingAndTransfersDetails2">
                    <div class="ui-bar">
                        <h3 id="lblReceivingAndTransFersSummaryTo"> </h3>
                        <br />
                        <table id="tableReceivingAndTransfersDetails2" data-role="table" style="display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive table table-light table-striped table-hover headerfitwidth">
                            <thead>
                                <tr>
                                    <th>Product</th>
                                    <th>Unit Measure</th>
                                    <th>Quantity</th>
                                </tr>
                            </thead>

                            <tbody id="tBodyReceivingAndTransfersDetails2"></tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>     

    @*PopUp Receiving Transfer FOWST*@

    <div id="panReceivingTransferFowst-bs" data-backdrop="true" class="modal fade" style="display:none;">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">FOWST or Vendor of Interest</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body" id="panReceivingTransferFowstcontent-bs">

                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" class="btn btn-secondary btn-block btnReceivingTransferFowstCancel">Close</button>
                        </div>
                        <div class="col">
                            <button data-role="none" class="btn btn-success btn-block btnReceivingTransferFowstDone">Done</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div data-role="popup" id="panReceivingTransferFowst" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3>&nbsp;FOWST or Vendor of Interest&nbsp;</h3></center>
        <div data-role="content" class="jPopupContent">
            <div id="panReceivingTransferFowstcontent">
                <div>
                    <span id="spanRecTransFowst1Error" style="display:none; color:#F00"><br />Please select a FOWST.</span>
                    <select id="selFowst1" data-role="none" class="form-control form-select">
                        <option value="">Please Select Fowst (Required)</option>
                    </select>
                </div>
                <br />
                <div>
                    <span id="spanRecTransFowst2Error" style="display:none; color:#F00"><br />Please select a another FOWST.</span>
                    <select id="selFowst2" data-role="none" class="form-control form-select">
                        <option value="">Please Select Fowst or Vendor (Optional)</option>
                    </select>
                    <div id="divThirdParty" style="display:none;">
                        <br />
                        <strong>Vendor:</strong><span id="txtVendorName"></span>
                    </div>
                </div>
            </div>            
            <br /><br />
            <button data-role="none" class="btn custom-btn-theme-b btnReceivingTransferFowstDone">Done</button>
            <br /><br />
            <button data-role="none" class="btn custom-btn-theme-c btnReceivingTransferFowstCancel">Cancel</button>
        </div>
    </div>

</div>
