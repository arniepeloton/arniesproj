@Code
    ViewData("Title") = "Franchise Central"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
    <script>
        require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/Franchise.js?crc=')]);
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
    /* Add alternating row stripes */

    .table-stripe tbody tr:hover {
      background-color: #E6E6E6; /* non-RGBA fallback  */

    }
    /* Add stroke to the header and last item */
    .table-stripe thead th,
    .table-stripe tbody tr:hover {
      background-color: #E6E6E6; /* non-RGBA fallback  */

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
End Section

<div id="pageMain" data-role="page">
    <div data-role="header" data-position="fixed" style="background: none white; border: none">
        <div class="ui-grid-b" >
            <div class="ui-block-a">
                <div id="btnFranchiseCentralHome" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                <span style="margin-top:5px;">Home</span>
                </div>
            </div>
            <div class="ui-block-b" >
                <div id="btnFranchiseCentralOrders" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons" >
                  <span style="margin-top:5px;">Dashboard</span>
                </div>
            </div>
            <div class="ui-block-c">
                <div id="btnFranchiseCentralManager" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                   My <br /> Franchisees
                </div>
            </div>
        </div>
        <div>
            <div style="margin:0px;" data-role="button" data-theme="b" data-corners="false">
           
               <label id="">Franchise Central Dashboard</label>
           
            </div>
        </div>
   </div>
<div id="divFranchiseeOrders">


   <div data-role="content" style="padding: 0px;margin:0px">

        <div class="ui-grid-d" style="padding: 0px;margin:0px">
            <div class="ui-block-a" style="width:10%;padding: 20px 10px 0px 10px;">
                 <b>Group:</b>
            </div>
            <div class="ui-block-b" style="width:20%">
                 <select id="selGroupList">
                </select> 
               
            </div>
            <div class="ui-block-c" style="width:30%;padding: 20px 5px 0px 5px;" >
                <b>Distribution Center:</b>
                <label id="lblFowstName"></label>
            </div>
            <div class="ui-block-d" style="width:20%;padding: 20px 5px 0px 5px;">
                
                <b>No. of Franchisees:</b>
                <label id="lblNoOfFranchisees"></label>
            </div>
            <div class="ui-block-e" style="width:20%;" >
                
                <button id="btnRefreshList">Refresh List</button>
            </div>

             
        </div>
       
   </div>

  
    <table data-role="table" id="tableFranchiseeOrders" class="ui-body-d ui-shadow table-stripe ui-responsive" data-column-btn-theme="b">
            <thead>
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
                            
            <tbody id='tblFranchiseeOrdersBody'>
                 
            </tbody>
                  
        </table>

</div>
               
    <div id="divFranchiseeManager" style="display:none;">
         <div style="padding:5px;margin:0px">
            Group Settings lets you create new franchisee groups or add new franchisees to your network. Please select a group to add new members to the list.
            <br />
            <button id="btnAddFranchiseGroup">Group Settings</button>
 
          </div>
 
            
            <table data-role="table" id="table-custom-2" class="ui-body-d ui-shadow table-stripe ui-responsive" data-column-btn-theme="b">
                <thead>
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
                            
                <tbody id='tblFranchiseeManagerBody'>
                 
                </tbody>
                  
            </table>
     
    </div>

  
   <div id="popOptions" data-role="popup">
        <ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="divider">View Options</li>
            <li><a id="optSalesBreakdown">Sales Breakdown</a></li>
            <li><a id="optProductBreakdown">Product Breakdown</a></li>
            <li><a id="optFranchiseeLastReceived">Last Received by Franchisee</a></li>
        </ul>
    </div>
   
   
</div>

 <div data-role="dialog" id="dialogAddNewFranchisee" >
	   <div data-role="header" data-theme="d">
		<h1>Add New Franchisee</h1>
	</div>
        <div data-role="content">
        
            <p>You are about to add <label id="lblFowstNameAdd"></label> as your franchisee. </p>
            <p>Add the franchisee to a group:
                 <select id=""></select>
            </p>
            <p>If you click Continue, a confirmation message will be sent to the super administrator of the selected organization to approve your request.</p>
            <br />
            <br />
            <div class="ui-grid-a">
                <div class="ui-block-a" >
                    <button id="btnAddNewCancel">Cancel</button>
                </div>
                <div class="ui-block-b">
                    <button id="btnAddNewContinue">Continue</button>
                </div>
            </div>
	</div>

</div>
   
<div data-role="dialog" id="dialogFranchiseeStatus" >
	<div data-role="header" data-theme="d">
		<h1><label id="lblFranchiseeName"></label></h1>
	</div>
    <div data-role="content" data-dom-cache="true" style="padding-top: 0px;">
           
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
      
            Group: <label id="lblGroupName"></label>
          <div id="divAddToGrpButton" style="display:none;">  <button id="btnAddToGroup" >Add to Group</button></div>
        </div>
        
           
         <div id="divPendingFranchisee">
            <p>A request has been sent to the administrator for approval. </p>
        </div>

        <div id="divPending">
            <label id="lblFranchiseeNameLabel"></label> is requesting your approval to be a franchisee
            <br />
            <br />
            <div class="ui-grid-a">
                <div class="ui-block-a" >
                        <button id="btnApproveRequest">Approve</button>
                </div>
                <div class="ui-block-b">
                    <button id="btnRejectRequest">Reject</button>
                </div>
            </div>
        </div>
        <br />
        <a data-role="button"  id="btnFranchiseeStatusCancel" data-theme="c">Back</a> 
	</div>


  

</div>



<div data-role="page" id="pageFranchiseeStatus" >
    <div data-role="header">
        <h1 id="hdrTxt"></h1>
          <a href="#" data-icon="arrow-l" id="btnFranchiseeDetailsBack">Back</a>
    </div>

        <div id="divProductBreakdown" style="display:none;padding-top:49px">

             <table data-role="table" id="tableFranchiseeCutOffTable" class="ui-body-d ui-shadow table-stripe ui-responsive" data-column-btn-theme="b">
                <thead>
                    <tr style="cursor: pointer">
                        <th></th>
                        <th data-sort="string">Stock No.</th>
                        <th data-sort="string">Stock Description</th>
                        <th data-sort="int" style="text-align: center;">Order Qty</th>
                        <th data-sort="string">Unit Measure</th>

                    </tr>
                                   
                </thead> 
                            
                <tbody id='tblFranchiseeCutOffBody'>
                 
                </tbody>
                  
             </table>

            <br />
            <div style="text-align: right;"><label id="lblSKUCountFranchiseOrder" ></label></div><br />
     @*       <div style="text-align: right;"><label id="lblTotal" style="font-size: x-large" ></label></div>
    *@
            <br />

            <div class="ui-grid-a" id="divOrderButtons" style="display:none;">
                <div class="ui-block-a">
                    <div id="btnFranchiseCentralDownloadFile" style="margin:0px; height: 80px;font-weight:normal;" data-role="button" data-corners="false">
                  
                    <img src="../../Content/images/excel.png" width="45" height="45" /><br /> Download CSV
                    </div>
                </div>
                <div class="ui-block-b">
                    <div id="btnFranchiseDownloadPickingList" style="margin:0px; height: 80px;font-weight:normal" data-role="button" data-corners="false">
                     
                    <img src="../../Content/images/pdf.png" width="45" height="45" /><br /> Download Batch Picking List and Sorting List
                       
                    </div>
                </div>

            </div>



        </div>
        <div id="divSalesBreakdown"  style="display:none;padding-top:49px">
            <table data-role="table" id="tableSalesBreakdown" class="ui-body-d ui-shadow table-stripe ui-responsive" data-column-btn-theme="b">
                <thead>
                    <tr style="cursor: pointer">
                        <th></th>
                        <th data-sort="string">Franchisee</th>
                        <th data-sort="float" style="text-align: right;">Gross Sales</th>
                        <th data-sort="float" style="text-align: right;">Contribution to Group Sales</th>

                    </tr>
                                   
                </thead> 
                            
                <tbody id='tblSalesBreakdownBody'>
                 
                </tbody>
                  
             </table>
        </div>

        <div id="divFranchiseeLastReceived"  style="display:none;padding-top:49px">
            <table data-role="table" id="tableFranchiseeLastReceived" class="ui-body-d ui-shadow table-stripe ui-responsive" data-column-btn-theme="b">
                <thead>
                    <tr style="cursor: pointer">
                        <th></th>
                        <th data-sort="string">Franchisee</th>
                        <th data-sort="date">Last Received Sales Data</th>
                        <th data-sort="int" style="text-align: center;">Status</th>
                        <th data-sort="string">Red Flag Reason</th>
                        <th data-sort="string">Unflag Reason</th>
                    </tr>
                                   
                </thead> 
                            
                <tbody id='tblLastReceivedBody'>
                 
                </tbody>
                  
                </table>
        </div>

        

     <div data-role="popup" id="popupRemoveFlag" data-theme="b" class="ui-corner-all"> 
          <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
              
              <div data-role="content">
                     <center> <h3>Remove Flag</h3></center>
                    <p>This record has been flagged because you have not received data <br />from <label id="lblRedFlagFranchiseeName"></label> 
                    for more than <label id="lblRedFlagLimit"></label> hours. 
                    </p>
                    <p>You may contact the franchisee through the following:<br />
                @*    Contact Person: <label id="lblContactPerson"></label><br />*@
                    Contact Number: <label id="lblContactNo"></label><br />
                    Email Address: <label id="lblEmailAddress"></label>
                    </p>
                    <p>If you think this record has been falsely flagged, you may unflag it. 
                    </p>
                    <p>Please provide a reason for unflagging:
                    <textarea cols="5" rows ="5" id="txtReasonRemoveRedFlag"></textarea> 
                         </p>
                    <br />
                     <a data-role="button"  id="btnSaveReasonRemoveFlag" data-theme="c">Save</a> 
                    <a data-role="button"  id="btnCancelRemoveFlag" data-theme="c">Cancel</a> 
              </div>
    </div>

  

</div>





  