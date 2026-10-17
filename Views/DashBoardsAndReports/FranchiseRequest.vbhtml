<script>
    require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/FranchiseRequest.js?crc=' + assetversion)]);
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


    </style>
      
       @*Group Settings lets you create new groups or add new members/ franchisees to your network. Please select a group to add new members to the list.
 <div class="ui-grid-a" style="padding: 0px;margin:0px">
            <div class="ui-block-a" >*@
            <select id="selFranchiseType" data-role="none" class="form-control form-select fixed" style="margin-bottom:1rem;">
               
                <option value="optFranchisee">My Group / Franchisee Members</option>
                <option value="optFranchisor">Franchisor Requests</option>
            </select> 

            @*   </div>
         <div class="ui-block-b" >
               
                <button id="btnAddFranchiseGroup">Group Settings</button>
 
            </div>

        </div>*@

    <div id="divFranchisorRequests" style="display:none;padding-top:10px">

        <table data-role="table" id="table-custom-2" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive table table-light table-striped table-hover headerfitwidth" data-column-btn-theme="b">
            <thead class="custom-theme-f">
                <tr style="cursor: pointer">
                    <th></th>
                    <th data-sort="date">Last Updated</th>
                    <th data-sort="string">My FOWST</th>
                    <th data-sort="string">Franchisor</th>
                    <th data-sort="string">Franchisor Address</th>
                    <th data-sort="string">Status</th>
                </tr>

            </thead>

            <tbody id='tblFranchiseMessagesBody'></tbody>

        </table>
     
     </div>     
                
    <div id="divFranchiseeRequests" style="padding-top:10px; overflow:auto;">


        <table data-role="table" id="table-custom-2" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive table table-light table-striped table-hover headerfitwidth" data-column-btn-theme="b">
            <thead class="custom-theme-f">
                <tr style="cursor: pointer">
                    <th></th>
                    <th data-sort="string">Group</th>
                    <th data-sort="string">Franchisee</th>
                    <th data-sort="string">Organization</th>
                    <th data-sort="string">Address</th>
                    <th data-sort="date">Last Updated</th>
                    <th data-sort="string">Status</th>
                </tr>

            </thead>

            <tbody id='tblFranchiseeManagerBody'></tbody>

        </table>
     
    </div>

<div id="dialogFranchisorStatus-bs" class="modal fade" style="display:none;">
    <div class="modal-dialog modal modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title"><label class="lblFranchisorName"></label></h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body" id="dialogFranchisorStatuscontent-bs">

            </div>
        </div>
    </div>
</div>
    

     <div data-role="popup" id="dialogFranchisorStatus" class="ui-corner-all" style="padding:20px"> 
          <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
              
              <div data-role="content">
                     <center> <h3><label class="lblFranchisorName"></label></h3></center>
                  <div id="dialogFranchisorStatuscontent">
                      <h2>
                          Status: <label id="lblCurrentStatus"></label>
                      </h2>
                      <hr />
                      <div>
                          Requested by <label id="lblRequestedBy"></label> on <label id="lblDateRequested"></label>
                      </div>
                      <br />
                      <div id="divApproved">
                          Approved by <label id="lblApprovedBy"></label> on <label id="lblDateActivated"></label>
                          <br />
                          <br />
                      </div>


                      <div id="divPendingFranchisee">
                          <p>A request has been sent to the administrator for approval. </p>
                      </div>

                      <div id="divPending">
                          <label id="lblFranchisorNameLabel"></label> is requesting your approval to be a franchisor of <label id="lblFranchiseeNameLabel"></label>
                          <br />
                          <br />
                          <div class="row">
                              <div class="col-sm-6">
                                  <button id="btnFranchiseRequestApproveRequest" data-role="none" class="btn custom-btn-theme-b btn-success btn-block">Approve</button>
                              </div>
                              <div class="col-sm-6">
                                  <button id="btnFranchiseRequestRejectRequest" data-role="none" class="btn custom-btn-theme-d btn-danger btn-block">Reject</button>
                              </div>
                          </div>
                      </div>
                  </div>
                       
                    <br />
              </div>
    </div>

    <div id="dialogFranchiseeStatus-bs" class="modal fade" style="display:none;">
        <div class="modal-dialog modal modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title"><label class="lblFranchiseeName"></label></h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body" id="dialogFranchiseeStatuscontent-bs">

                </div>
            </div>
        </div>
    </div>

    
    <div data-role="popup" id="dialogFranchiseeStatus" class="ui-corner-all"  style="padding:20px"> 
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
              
        <div data-role="content">
            <center> <h3><label class="lblFranchiseeName"></label></h3></center>
            <div id="dialogFranchiseeStatuscontent">
                <h2>
                    Status: <label id="lblCurrentStatusFranchisee"></label>
                </h2>
                <hr />
                <div>
                    Requested by <label id="lblRequestedByFranchisee"></label> on <label id="lblDateRequestedFranchisee"></label>
                </div>
                <br />
                <div id="divApprovedFranchisee">
                    Approved by <label id="lblApprovedByFranchisee"></label> on <label id="lblDateActivatedFranchisee"></label>
                    <br />
                    <br />



                </div>

                <div id="divPendingFranchiseeRequest">
                    <p>A request has been sent to the administrator for approval. </p>
                </div>
                <button data-role="none" id="btnAddFranchiseGroup" class="btn custom-btn-theme-b btn-success btn-block" style="display: none;">Group Settings</button>
                <div id="divRequestApproval">

                    <div class="row">
                        <div class="col-sm-2">
                            Group:
                        </div>
                        <div class="col-sm-10">
                            <select id="selFranchiseGroup" data-role="none" class="form-control form-select"></select>
                        </div>
                    </div>

                    <p>
                        <label id="lblFranchiseeNameLabelFranchisee"></label> is requesting your approval to be a franchisee.
                    </p>
                    <p>To be a part of your franchise network, the franchisee must belong to a group. Please select from the list above. </p>
                    <p>
                        If you don't have an existing franchise group or if you
                        want to add a new one, <a data-role="none" href="#" class="link-primary linkAddFranchiseGroup">please click here to add new group</a>.
                    </p>
                    <div class="row" id="divFranchiseeApproveReject">
                        <div class="col-sm-6">
                            <button id="btnApproveRequestFranchisee" data-role="none" class="btn btn-success btn-block">Approve</button>
                        </div>
                        <div class="col-sm-6">
                            <button id="btnRejectRequestFranchisee" data-role="none" class="btn btn-danger btn-block">Reject</button>
                        </div>
                    </div>
                </div>
            </div>
            <br />
        </div>
    </div>