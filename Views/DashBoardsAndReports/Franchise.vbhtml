<script>
        require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/Franchise.js?crc=' + assetversion)]);
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
    /* Button */
    .HotButtons .ui-btn-inner {
        
        padding-top: 1.4em;
    }
    
    
    .HotButtonsText .ui-btn-text {
        color:White;   
    }
    

    </style>

<div id="divFranchiseeOrders" subpage="divstorereplenishment" style="display:block;">
   <div data-role="content" style="padding-top: 0px;">
       <div class="row">
           <div class="col-sm-3 mb-3">
               <div style="display:flex;">
                   <b style="margin-right:1rem;">Group:</b>
                   <select id="selGroupList" data-role="none" class="form-control form-select"></select>
               </div>               
           </div>
           <div class="col-sm-3 mb-3">
               <b>Distribution Center:</b> <label id="lblFowstName"></label>
           </div>
           <div class="col-sm-3 mb-3">
               <b>No. of Members:</b>
               <label id="lblNoOfFranchisees"></label>
           </div>
           <div class="col-sm-3 mb-3">
               <button id="btnRefreshList" data-role="none" class="btn btn-secondary btn-block custom-btn-theme-b">
                   Refresh List
               </button>
           </div>
       </div>
   </div>
    <br /><br />
  
    <div style="overflow:auto;">
        <table data-role="table" id="tableFranchiseeOrders" class="ui-body-d ui-shadow table-stripe ui-responsive custom-tbl-responsive table table-light table-striped table-hover headerfitwidth" data-column-btn-theme="b">
            <thead class="custom-theme-f">
                <tr style="cursor: pointer">
                    <th></th>
                    <th data-sort="string">Cut Off Name</th>
                    <th data-sort="date">Cut Off Period</th>
                    <th data-sort="date">Last Received Sales Data</th>
                    <th data-sort="int" style="text-align: center;">No. of SKUs</th>
                    <th data-sort="float" style="text-align: right;">Gross Sales</th>
                    <th data-sort="int" style="text-align: center;">Status</th>
                </tr>

            </thead>

            <tbody id='tblFranchiseeOrdersBody'></tbody>

        </table>
    </div>    

    <div id="divRemoveFlag" style="display:none;">
        <p>
            This record has been flagged because you have not received data from <label id="lblRedFlagFranchiseeName"></label>
            for more than <label id="lblRedFlagLimit"></label> hours.
        </p>
        <p>
            You may contact the franchisee through the following:<br />
            @*    Contact Person: <label id="lblContactPerson"></label><br />*@
            Contact Number: <id class="lblContactNo"></id><br />
            Email Address: <id class="lblEmailAddress"></id>
        </p>
        <p>
            If you think this record has been falsely flagged, you may unflag it.
        </p>
        <p>
            Please provide a reason for unflagging:
            <textarea cols="5" rows="5" id="txtReasonRemoveRedFlag" data-role="none" class="form-control" style="resize:none; height:100px;"></textarea>
        </p>
    </div>
    <div id="popupRemoveFlag-bs" class="modal fade" style="display:none;">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Remove Flag</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body" id="divRemoveFlagcontent-bs">

                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col">
                            <button data-role="none" class="btn btn-success btn-block btnSaveReasonRemoveFlag">Save</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div data-role="popup" id="popupRemoveFlag" class="ui-corner-all" style="padding:20px"> 
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
              
        <div data-role="content">
                <center> <h3>Remove Flag</h3></center>
            <div id="divRemoveFlagcontent">

            </div>
            <br />
                <a data-role="button"  class="btnSaveReasonRemoveFlag" data-theme="c">Save</a> 
            @*<a data-role="button"  id="btnCancelRemoveFlag" data-theme="c">Cancel</a> *@
        </div>
    </div>

  
</div>

<div id="divProductBreakdown" style="display:none;">
    <div class="table-responsive mb-3" style="margin-bottom:1rem;">
        <table data-role="table" id="tableFranchiseeCutOffTable" class="ui-body-d ui-shadow table-stripe ui-responsive custom-tbl-responsive table table-light table-striped table-hover headerfitwidth" data-column-btn-theme="b">
            <thead class="custom-theme-f">
                <tr style="cursor: pointer">
                    <th></th>
                    <th data-sort="string">Stock No.</th>
                    <th data-sort="string">Stock Description</th>
                    <th data-sort="int" style="text-align: center;">Order Qty</th>
                    <th data-sort="string">Unit Measure</th>
                </tr>
            </thead>
            <tbody id='tblFranchiseeCutOffBody'></tbody>
        </table>
    </div>

    <div style="text-align: right;"><label id="lblSKUCountFranchiseOrder"></label></div>
    @*       <div style="text-align: right;"><label id="lblTotal" style="font-size: x-large" ></label></div>
    *@

    <div id="divOrderButtons" style="display:none; text-align:center;">
        <button data-role="none" id="btnFranchiseCentralDownloadFile" class="btn custom-btn-theme-b btn-less-width btn-success btn-lg">
            Download CSV
        </button>
        <button data-role="none" id="btnFranchiseDownloadPickingList" class="btn custom-btn-theme-a btn-less-width btn-warning btn-lg">
            Download Batch Picking List and Sorting List
        </button>
    </div>
</div>

<div id="popupProductBreakdown-bs" subpage="divstorereplenishment" style="display:none;">
    <div id="popupProductBreakdowncontent-bs">

    </div>
</div>
    
<div id="popupProductBreakdown" custom-data-role="popup" style="display:none;">
    <div class="popup">
        <div class="popup-header">
            Product Breakdown<a class="popup-close">x</a>
        </div>
        <div class="popup-content" id="popupProductBreakdowncontent">

        </div>
        <div class="popup-footer align-right">
            <button id="" data-role="none" class="btn custom-btn-theme-c btnCircle popup-close">
                <img alt="Options" src="/Content/images/close-black.png" />
            </button>
        </div>
    </div>
</div>

<div id="divSalesBreakdown" style="display:none;">
    <div style="overflow:auto; margin-bottom:1rem;">
        <table data-role="table" id="tableSalesBreakdown" class="ui-body-d ui-shadow table-stripe ui-responsive custom-tbl-responsive table table-light table-striped table-hover headerfitwidth" data-column-btn-theme="b">
            <thead class="custom-theme-f">
                <tr style="cursor: pointer">
                    <th></th>
                    <th data-sort="string">Franchisee</th>
                    <th data-sort="float" style="text-align: right;">Gross Sales</th>
                    <th data-sort="float" style="text-align: right;">Contribution to Group Sales</th>
                </tr>
            </thead>
            <tbody id='tblSalesBreakdownBody'></tbody>
        </table>
    </div>    
</div>

<div id="popupSalesBreakdown-bs" subpage="divstorereplenishment" style="display:none;">
    <div id="popupSalesBreakdowncontent-bs">

    </div>
</div>

<div id="popupSalesBreakdown" custom-data-role="popup" style="display:none;">
    <div class="popup">
        <div class="popup-header">
            Sales Breakdown<a class="popup-close">x</a>
        </div>
        <div class="popup-content" id="popupSalesBreakdowncontent">

        </div>
        <div class="popup-footer align-right">
            <button id="" data-role="none" class="btn custom-btn-theme-c btnCircle popup-close">
                <img alt="Options" src="/Content/images/close-black.png" />
            </button>
        </div>
    </div>
</div>

<div id="divFranchiseeLastReceived" style="display:none;">
    <div style="overflow:auto; margin-bottom:1rem;">
        <table data-role="table" id="tableFranchiseeLastReceived" class="ui-body-d ui-shadow table-stripe ui-responsive custom-tbl-responsive table table-light table-striped table-hover headerfitwidth" data-column-btn-theme="b">
            <thead class="custom-theme-f">
                <tr style="cursor: pointer">
                    <th></th>
                    <th data-sort="string">Franchisee</th>
                    <th data-sort="date">Last Received Sales Data</th>
                    <th data-sort="int" style="text-align: center;">Status</th>
                    <th data-sort="string">Red Flag Reason</th>
                    <th data-sort="string">Unflag Reason</th>
                </tr>
            </thead>
            <tbody id='tblLastReceivedBody'></tbody>
        </table>
    </div>    
</div>

<div id="popupFranchiseeLastReceived-bs" subpage="divstorereplenishment" style="display:none;">
    <div id="popupFranchiseeLastReceivedcontent-bs">

    </div>
</div>

<div id="popupFranchiseeLastReceived" custom-data-role="popup" style="display:none;">
    <div class="popup">
        <div class="popup-header">
            Last Received Date<a class="popup-close">x</a>
        </div>
        <div class="popup-content" id="popupFranchiseeLastReceivedcontent">

        </div>
        <div class="popup-footer align-right">
            <button id="" data-role="none" class="btn custom-btn-theme-c btnCircle popup-close">
                <img alt="Options" src="/Content/images/close-black.png" />
            </button>
        </div>
    </div>
</div>