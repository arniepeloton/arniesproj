@Code
    ViewData("Title") = "Franchise Requests"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
    <script>
        require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/FranchiseRequest.js?crc=')]);
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
        <div class="ui-grid-a" >
            <div class="ui-block-a">
                <div id="btnFranchiseCentralHome" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                <span style="margin-top:5px;">Home</span>
                </div>
            </div>
            <div class="ui-block-b">
                <div id="" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" >
                    Franchisor<br /> Manager
                </div>
            </div>
        </div>
        <div>
            <div style="margin:0px;" data-role="button" data-theme="b" data-corners="false">
           
               <label id="">Franchise Central</label>
           
            </div>
        </div>
   </div>


       
    <table data-role="table" id="table-custom-2" class="ui-body-d ui-shadow table-stripe ui-responsive" data-column-btn-theme="b">
        <thead>
            <tr style="cursor: pointer">
                <th></th>
                <th data-sort="date">Last Updated</th>
                <th data-sort="string">My FOWST</th>
                <th data-sort="string">Franchisor</th>
                <th data-sort="string">Franchisor Address</th>
                <th data-sort="string">Status</th>
            </tr>
                                   
        </thead> 
                            
        <tbody id='tblFranchiseMessagesBody'>
                 
        </tbody>
                  
    </table>
     
 
</div>


<div data-role="dialog" id="dialogFranchiseeStatus" >
	<div data-role="header" data-theme="d">
		<h1><label id="lblFranchisorName"></label></h1>
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
        </div>
        
           
         <div id="divPendingFranchisee">
            <p>A request has been sent to the administrator for approval. </p>
        </div>

        <div id="divPending">
            <label id="lblFranchisorNameLabel"></label> is requesting your approval to be a franchisor of <label id="lblFranchiseeNameLabel"></label>
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
        <a data-role="button"  id="btnFranchiseMessageCancel" data-theme="c">Back</a> 
	</div>


  

</div>
