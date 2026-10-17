@*@Code
        ViewData("Title") = "Inventory Ledger"
        Layout = "~/Views/Shared/Mobiz.vbhtml"
    End Code

    @Section Header

    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    End Section*@

@*<link rel="stylesheet" href="/Content/flatui-css-custom.css" />
<link rel="stylesheet" href="/Content/bootstrap-custom.css" />
<link rel="stylesheet" href="/Content/jquery.dataTables2.css" />*@    

<script>
    require([GetPathFormat('promise!/Scripts/sources/InventoryLedger/InventoryBalance.js?crc=' + assetversion)]);
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
</style>

<div id="pageInventoryBalanceReport">
    <div data-role="content">
        <div id="divSecond" style="display:block;">
            @*<div id="divAddLibrary" style="text-align:left;">
            <h3 id="txtProductDescription"> </h3>
            <br />
        </div>*@
            <div class="row">
                <div class="col-6 col-sm-6 col-md-4 col-lg-3 mb-3">
                    Select FOWST <span class="required">*</span>: <br />
                    <button data-role="none" class="btn custom-btn-theme-c btn-secondary btn-block text-truncate" id="btnInventoryBalanceFowst"><span class="lblSelectedFowstGroup"> FOWST Location </span></button>
                </div>
                <div class="col-6 col-sm-6 col-md-4 col-lg-3 mb-3">
                    Inventory Type <span class="required">*</span>:<br />
                    <button data-role="none" class="btn custom-btn-theme-c btn-secondary btn-block text-truncate" id="btnInventoryBalanceOptions"><span class="lblSelectedInventoryType"> Choose Inventory Type </span></button>
                </div>
                <div class="col-6 col-sm-6 col-md-4 col-lg-3 mb-3">
                    Product Status: <br />
                    <button data-role="none" value="0" class="btn custom-btn-theme-c btn-secondary btn-block" id="btnInventoryBalanceStatus"><span class="lblInventoryProductStatus"> Active </span></button>
                </div>
                <div class="col-6 col-sm-6 col-md-4 col-lg-3 mb-3">
                    Last Posting Date: <br />
                    <button data-role="none" value="1" class="btn custom-btn-theme-c btn-secondary btn-block text-truncate" id="btnInventoryBalanceMovementYear"><span class="lblInventoryProductMovementYear"> All SKU with Movement for the Last 1 Year </span></button>
                </div>                
            </div>

            <div class="row mb-3">
                <div class="col-auto mb-3 order-3 order-md-0">
                    <input data-role="none" class="form-control" type="search" id="txtInventoryBalSearch" placeholder="Search Product here" />
                </div>
                <div class="col-6 col-md-4 col-lg-auto mb-3 order-1 order-md-1 d-none">
                    <button data-role="none" value="1" class="btn custom-btn-theme-c btn-secondary btn-block" id="butScanQr"><span class=""> Scan QR Code </span></button>
                </div>
                <div class="col-6 col-md-4 col-lg-auto mb-3 order-2 order-md-2 d-none">
                    <button data-role="none" value="1" class="btn custom-btn-theme-c btn-secondary btn-block" id="btnSearchMoreProducts"><span class=""> Search Entire Database </span></button>
                </div>
            </div>

            <div style="overflow:auto;">
                <table id="tblBalances" data-role="table" data-mode="reflow" style="display:table; width:100%;" class="display ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive table table-light table-striped table-hover">
                    <thead>
                        <tr class="custom-theme-f fixed-cell-width align-top text-center" style="font-size:14px;">
                            @*<th>  </th>*@
                            <th> Product No </th>
                            <th> Product Description </th>
                            <th> Brand </th>
                            @*<th> IsActive </th>*@
                            <th> Balance In Base Unit </th>
                            <th> Unit Measure </th>
                            <th> Inventory Value by COGS </th>
                            <th> Inventory Value by SRP </th>
                            <th> Last Balance Posting Date </th>
                            <th> No. of UnApproved Inventory Count </th>
                            <th> Last Inventory Count Date </th>
                            @*<th> Last Inventory Count By </th>*@
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>
            </div>

            <div data-role="popup" id="panInventoryBalanceOptions" data-overlay-theme="a" data-theme="a" data-corners="false" style="display:none;">
                <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
                <center><h3>Choose Inventory Type</h3></center>
                <div data-role="content">
                    <div style="padding: 8px 15px 16px 15px; overflow:auto;">
                        <ul data-role="listview" id="lstReceivingTransferOrganization" class="li-theme-c noBorderList custom-list custom-list-sm">
                            <li id="liViewAll" class="btnLIListofFowstGroup">
                                <a href="#" class="btn custom-btn-theme-c txt-left" style="box-shadow:none !important; font-weight:bold !important; width:auto;">
                                    All Inventory Types
                                </a>
                            </li>
                            <li id="liViewProducts" class="btnLIListofFowstGroup">
                                <a href="#" class="btn custom-btn-theme-c txt-left" style="box-shadow:none !important; font-weight:bold !important; width:auto;">
                                    Finished Goods
                                </a>
                            </li>
                            <li id="liViewRawMaterials" class="btnLIListofFowstGroup">
                                <a href="#" class="btn custom-btn-theme-c txt-left" style="box-shadow:none !important; font-weight:bold !important; width:auto;">
                                    Intermediate Goods and Raw Materials
                                </a>
                            </li>
                            <li id="liViewAllByCategory" class="btnLIListofFowstGroup">
                                <a href="#" class="btn custom-btn-theme-c txt-left" style="box-shadow:none !important; font-weight:bold !important; width:auto;">
                                    All Inventory Types Filtered By Category
                                </a>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>

            <div id="panSetBegBalance-bs" data-backdrop="true" class="modal fade" style="display:none;">
                <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Set Beginning Balance</h5>
                            <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        </div>
                        <div class="modal-body" id="panSetBegBalancecontent-bs">

                        </div>
                        <div class="modal-footer">
                            <div class="row">
                                <div class="col">
                                    <button data-role="none" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Close</button>
                                </div>
                                <div class="col">
                                    <button data-role="none" class="btn btn-success btn-block btn-lg btnSetBeginningBalance">Set Inventory Balance</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div data-role="popup" id="panSetBegBalance" data-overlay-theme="a" data-theme="a" data-corners="false">
                <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="btnClosePopUp">Close</a>
                <center><h3 style="margin-left:10px; margin-right:10px;">Set Beginning Balance<span class="lblPopUpHeaderLibraryName"></span></h3></center>
                <div data-role="content" style="max-width:350px;" class="ui-content ui-body-d" data-theme="a">
                    <div style="margin-left:10px; margin-right:10px;">
                        <div id="panSetBegBalancecontent">
                            <div class="row mb-3">
                                <label class="col-md-3 col-form-label">Enter Beginning Balance:</label>
                                <div class="col-md">
                                    <input type="text" id="txtBeginningBalance" data-role="none" class="form-control" />
                                </div>
                            </div>

                            <div class="row mb-3">
                                <label class="col-md-3 col-form-label">Unit Measure:</label>
                                <div class="col-md">
                                    <select id="selUnitMeasure" data-role="none" class="form-control form-select"> </select>
                                </div>
                            </div>

                            <div class="row mb-3">
                                <label class="col-md-3 col-form-label">Date And Time:</label>
                                <div class="col-md">
                                    <input type="datetime-local" id="dtBegBalanceDateTime" value="" data-role="none" class="form-control">
                                </div>
                            </div>
                        </div>

                        <br />
                        <br />
                        <div class="ui-grid-solo">
                            <div class="ui-block-a">
                                <button data-role="none" class="btn custom-btn-theme-b btnSetBeginningBalance">
                                    Set Inventory Balance
                                </button>
                            </div>
                        </div>
                    </div>
                    <br />
                </div>
            </div>

            <div class="popup" id="panInventoryCountApprovalByTransaction" custom-data-role="popup" style="display:none;">
                <div class="popup-header">
                    Inventory Count Approval/Disapproval
                    <a class="popup-close">x</a>
                </div>

                <div class="popup-content">
                    <center><b><span id="lblPopUpProductName2x"></span> <span class="lblPopUpHeaderLibraryName"></span></b></center>

                    <br />

                    <div class="table-responsive">
                        <table data-role="table" data-mode="reflow" style="margin-right:5px; display:table; width:100%;" class="ui-table ui-responsive table-stroke table-stripe ui-table-reflow custom-tbl-responsive">
                            <thead>
                                <tr id="tblInventoryCountApprovalCountHeader2" class="align-top text-center" style="font-size:14px;">
                                    <th data-sort="float" style="cursor:pointer;">#</th>
                                    <th data-sort="string" style="cursor:pointer;">FOWST</th>
                                    <th data-sort="string" style="cursor:pointer;">Bin</th>
                                    <th data-sort="date" style="cursor:pointer;">Inventory Count Date</th>
                                    <th data-sort="float" style="cursor:pointer;">Quantity</th>
                                    <th data-sort="string" style="cursor:pointer;">Unit Measure</th>
                                    <th data-sort="accounting" style="cursor:pointer;">Variance</th>
                                    <th colspan="2">Action</th>
                                    @*<th style="cursor:pointer;"></th>*@
                                </tr>
                            </thead>
                            <tbody id="tblInventoryCountApprovalBody2"></tbody>
                        </table>
                    </div>
                </div>

                <div class="popup-footer align-right">
                    <button data-role="none" class="btn custom-btn-theme-b btn-less-width popup-close">
                        Done
                    </button>
                </div>
            </div>

            <div id="panInventoryCountApproval-bs" data-backdrop="true" class="modal fade" style="display:none;">
                <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Inventory Count Approval/Disapproval</h5>
                            <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        </div>
                        <div class="modal-body" id="panInventoryCountApprovalcontent-bs">

                        </div>
                    </div>
                </div>
            </div>

            <div class="popup" id="panInventoryCountApproval" custom-data-role="popup" style="display:none;">
                <div class="popup-header">
                    Inventory Count Approval/Disapproval
                    <a class="popup-close">x</a>
                </div>

                <div class="popup-content" id="panInventoryCountApprovalcontent">
                    <span id="lblPopUpProductName2"></span> <br />
                    <div class="table-responsive">
                        <table data-role="table" data-mode="reflow" style="margin-right:5px; display:table; width:100%;" class="ui-table ui-responsive table-stroke table-stripe ui-table-reflow custom-tbl-responsive table table-light table-hover table-striped headerfitwidth">
                            <thead>
                                <tr id="tblInventoryCountApprovalCountHeader" class="align-top text-center" style="font-size:14px;">
                                    <th data-sort="float" style="cursor:pointer;">#</th>
                                    <th data-sort="string" style="cursor:pointer;">FOWST</th>
                                    <th data-sort="string" style="cursor:pointer;">Bin</th>
                                    <th data-sort="date" style="cursor:pointer;">Inventory Count Date</th>
                                    <th data-sort="float" style="cursor:pointer;">Observed Count</th>
                                    <th data-sort="string" style="cursor:pointer;">Unit Measure</th>
                                    <th data-sort="accounting" style="cursor:pointer;">Variance</th>
                                    <th colspan="2">Action</th>
                                    @*<th style="cursor:pointer;"></th>*@
                                </tr>
                            </thead>
                            <tbody id="tblInventoryCountApprovalBody"></tbody>
                        </table>
                    </div>
                </div>

                <div class="popup-footer align-right">
                    <button id="btnInventoryCountApprovalDone" data-role="none" class="btn custom-btn-theme-b btn-less-width">
                        Done
                    </button>
                </div>
            </div>

            <div id="panInventoryCountApprovalHistory-bs" data-backdrop="true" class="modal fade" style="display:none;">
                <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Inventory Count Approval/Disapproval History</h5>
                            <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        </div>
                        <div class="modal-body" id="panInventoryCountApprovalHistorycontent-bs">

                        </div>
                    </div>
                </div>
            </div>

            <div class="popup" id="panInventoryCountApprovalHistory" custom-data-role="popup" style="display:none;">
                <div class="popup-header">
                    Inventory Count Approval/Disapproval History
                    <a class="popup-close">x</a>
                </div>

                <div class="popup-content" id="panInventoryCountApprovalHistorycontent">
                    <span id="lblPopUpProductNameAppHistory"></span> <br />
                    <div class="table-responsive">
                        <table data-role="table" data-mode="reflow" style="margin-right:5px; display:table; width:100%;" class="ui-table ui-responsive table-stroke table-stripe ui-table-reflow custom-tbl-responsive table table-light table-hover table-striped headerfitwidth">
                            <thead>
                                <tr id="tblInventoryCountApprovalHistoryHeader" class="align-top text-center" style="font-size:14px;">
                                    <th data-sort="float" style="cursor:pointer;">#</th>
                                    <th data-sort="string" style="cursor:pointer;">FOWST</th>
                                    <th data-sort="string" style="cursor:pointer;">Bin</th>
                                    <th data-sort="date" style="cursor:pointer;">Inventory Count Date</th>
                                    <th data-sort="float" style="cursor:pointer;">Observed Count</th>
                                    <th data-sort="string" style="cursor:pointer;">Unit Measure</th>
                                    <th data-sort="accounting" style="cursor:pointer;">Variance</th>
                                    <th data-sort="string" style="cursor:pointer;">Status</th>
                                    <th data-sort="string" style="cursor:pointer;">Modified By</th>
                                    <th data-sort="date" style="cursor:pointer;">Modified Date & Time</th>
                                    @*<th style="cursor:pointer;"></th>*@
                                </tr>
                            </thead>
                            <tbody id="tblInventoryCountApprovalHistoryBody"></tbody>
                        </table>
                    </div>
                </div>

                <div class="popup-footer align-right">
                    <button id="btnInventoryCountApprovalHistoryClose" data-role="none" class="btn custom-btn-theme-c btn-less-width popup-close">
                        Close
                    </button>
                </div>
            </div>

            <div id="panSelectInventoryBalance-bs" data-backdrop="true" class="modal fade" style="display:none;">
                <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Inventory Count</h5>
                            <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        </div>
                        <div class="modal-body" id="panSelectInventoryBalancecontent-bs">

                        </div>
                        <div class="modal-footer">
                            <div class="row">
                                <div class="col">
                                    <button data-role="none" class="btn btn-secondary btn-block" data-dismiss="modal">Close</button>
                                </div>
                                <div class="col">
                                    <button data-role="none" class="btn btn-success btn-block btnSaveInventoryCountAndProductCap">Done</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div data-role="popup" id="panSelectInventoryBalance" data-overlay-theme="a" data-theme="a" data-corners="false">
                <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="btnInventoryBalanceClosePopUp">Close</a>
                <center><h3 style="margin-left:10px; margin-right:10px;">Inventory Count<span class="lblPopUpHeaderLibraryName"></span></h3></center>
                <div data-role="content" class="ui-content ui-body-d" data-theme="a">
                    <div style="margin-left:10px; margin-right:10px;">
                        <div id="panSelectInventoryBalancecontent">
                            <span id="lblPopUpProductName"></span>
                            <br />
                            <div class="table-responsive">
                                <table data-role="table" data-mode="reflow" style="margin-right:5px; display:table; width:100%;" class="ui-table ui-responsive table-stroke table-stripe table table-light table-hover table-striped headerfitwidth">
                                    <thead>
                                        <tr id="tblInventoryCountHeader" class="custom-theme-f align-top text-center" style="font-size:14px;">
                                            <th data-sort="string" style="cursor:pointer;">Bin Name</th>
                                            @*<th data-sort="float" style="cursor:pointer;">Quantity</th>*@
                                            <th data-sort="float" style="cursor:pointer;">Current Balance</th>
                                            <th data-sort="float" style="cursor:pointer;">Obrserved Count(In base quantity)</th>
                                            <th data-sort="float" style="cursor:pointer;">Max Cap</th>
                                            <th data-sort="float" style="cursor:pointer;">Reorder Point</th>
                                            <th data-sort="float" style="cursor:pointer;">Reorder Qty</th>
                                            @*<th style="cursor:pointer;">Action</th>*@
                                        </tr>
                                    </thead>
                                    <tbody id="tblInventoryCountBody"></tbody>
                                </table>
                            </div>
                        </div>

                        <br /> <br />

                        <div class="ui-grid-b">
                            <div class="ui-block-a">

                            </div>
                            <div class="ui-block-b">

                            </div>
                            <div class="ui-block-c">
                                <button id="btnSaveInventoryCountAndProductCap" data-role="none" class="btn custom-btn-theme-b">
                                    Done
                                </button>
                            </div>
                        </div>
                    </div>
                    <br />
                </div>
            </div>

            <div data-role="popup" id="panSetInventoryBalance" data-overlay-theme="a" data-theme="a" data-corners="false">
                <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="btnInventoryBalanceClosePopUp">Close</a>
                <center><h3 style="margin-left:10px; margin-right:10px;">Inventory Count<span class="lblPopUpHeaderLibraryName"></span></h3></center>
                <div data-role="content" style="max-width:450px;" class="ui-content ui-body-d" data-theme="a">
                    <div style="margin-left:10px; margin-right:10px;">

                        <center><h3 style="margin-left:10px; margin-right:10px;"><span id="lblPopUpProductName"></span> <span class="lblPopUpHeaderLibraryName"></span></h3></center>

                        <div id="dvInventoryCount">

                            <div data-role="fieldcontain">
                                <label id="lblBin" for="selBins" style="font-weight:bold"> Bin Name </label>
                                @*<select id="selBins"> </select>*@
                                <br />
                            </div>

                            <br />

                            <div>
                                <label for="txtInventoryCount" style="font-weight:bold"> Enter Count Quantity </label>
                                <input type="text" id="txtInventoryCount" placeholder="Enter Count Quantity" />
                            </div>

                            <br />

                            <div>
                                <label for="selUnitMeasureForInventoryCount" style="font-weight:bold"> Select Unit Measure </label>
                                <select id="selUnitMeasureForInventoryCount"> </select>
                            </div>

                        </div>

                        <br />
                        <br />
                        <div class="ui-grid-solo">
                            <div class="ui-block-a">
                                <button id="btnSetInventoryCount" data-role="none" class="btn custom-btn-theme-b">
                                    Save Inventory Count
                                </button>
                            </div>
                        </div>
                    </div>
                    <br />
                </div>
            </div>

            <div id="panCompanyWideInventoryBalance-bs" data-backdrop="true" class="modal fade" style="display:none;">
                <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Company Wide Inventory Balance</h5>
                            <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        </div>
                        <div class="modal-body" id="panCompanyWideInventoryBalancecontent-bs">
                            <div style="white-space:nowrap; overflow:auto;">
                                <ul class="nav nav-tabs" id="nav-tab" role="tablist" style="flex-wrap:nowrap;">
                                    <li class="nav-item">
                                        <button data-role="none" class="nav-link active btnFOWSTInventoryBalance" data-toggle="tab" data-target="#divFOWSTInventoryBalance-bs">Inventory Balance for this Location</button>
                                    </li>
                                    <li class="nav-item">
                                        <button data-role="none" class="nav-link btnCompanyWideInventoryBalance" data-toggle="tab" data-target="#divCompayWideInventoryBalance-bs">Company Wide Inventory Balance</button>
                                    </li>
                                </ul>
                            </div>

                            <div class="tab-content">
                                <div class="tab-pane fade show active p-3" id="divFOWSTInventoryBalance-bs"></div>

                                <div class="tab-pane fade p-3" id="divCompayWideInventoryBalance-bs"></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="panCompanyWideInventoryBalance" custom-data-role="popup" style="display:none;">
                <div class="popup-md">
                    <div class="popup-header">
                        Company Wide Inventory Balance <a class="popup-close">x</a>
                    </div>
                    <div class="popup-content">
                        <div id="divTabRevenue" class="custom-tab">
                            <ul>
                                <li class="active btnFOWSTInventoryBalance" custom-tab-content="#divFOWSTInventoryBalance">Inventory Balance for this Location</li>
                                <li class="btnCompanyWideInventoryBalance" custom-tab-content="#divCompayWideInventoryBalance">Company Wide Inventory Balance</li>
                            </ul>
                        </div>
                        <div id="divFOWSTInventoryBalance" custom-tab-role="content" style="display: block;" tab-container="divTabRevenue">
                            <div class="row">
                                <div class="ui-grid-solo" style="display:block;">

                                    <table align="center" width="100%">
                                        <tr class="d-none">
                                            <td colspan="6"><h1 style="text-align:center"><span id="lblCompanyWideProduct">This is the item</span></h1></td>
                                        </tr>
                                        <tr>
                                            <td style="text-align:left; font-size:medium; font-weight:bold"> Product No. : </td>
                                            <td style="text-align:left; margin:20px; font-size:medium; font-weight:bold" id="lblProductNo"> </td>
                                            <td style="width:10px"> </td>
                                            <td style="text-align:left; margin:20px; font-size:medium; font-weight:bold"> On-Hand : </td>
                                            <td style="width:20px"> </td>
                                            <td><span id="lblOnHand">0</span></td>
                                        </tr>
                                        <tr>
                                            <td style="text-align:left; font-size:medium; font-weight:bold"> FOWST : </td>
                                            <td style="text-align:left; margin:20px; font-size:medium; font-weight:bold" id="lblFOWSTName"> </td>
                                            <td style="width:10px"> </td>
                                            <td style="text-align:left; margin:20px; font-size:medium; font-weight:bold"> Commited Qty : </td>
                                            <td style="width:20px"> </td>
                                            <td><span id="lblCommited">0</span></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td style="margin:20px"></td>
                                            <td style="width:10px"> </td>
                                            <td><h4>Availabel to Commit</h4></td>
                                            <td style="width:20px"> </td>
                                            <td><span id="lblAvailableToCommit">0</span></td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </div>

                        <div id="divCompayWideInventoryBalance" custom-tab-role="content" tab-container="divTabRevenue">
                            <div class="table-responsive">
                                <table data-role="table" id="tblCompayWideInventoryBalance" style="margin-right:5px; display:table; width:100%;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive table table-light table-striped table-hover headerfitwidth">
                                    <thead>
                                        <tr class="align-top text-center" style="font-size:14px;">
                                            <th> FOWST </th>
                                            <th style="width:180px;"> Assigned Order Qty </th>
                                            <th> Delivery Method </th>
                                            <th> On-Hand </th>
                                            <th> Commited Qty </th>
                                            <th> Available to Commit </th>
                                        </tr>
                                    </thead>
                                    <tbody id="tblBodyCompayWideInventoryBalance"></tbody>
                                </table>
                            </div>
                            <div class="fixed-bottom mb-3 text-center">
                                <button name="butSaveOrderQty" data-role="none" class="btn btn-success btn-lg">
                                    Save
                                </button>
                            </div>
                        </div>
                        @*<div style="float:right;">
                        <b></b>
                    </div>
                    <div style="float:left;">
                        <button id="btnFOWSTInventoryBalance">FOWST Inventory Balance</button>
                    </div>
                    <div style="float:left;">
                        <button id="btnCompanyWideInventoryBalance">Company Wide Inventory Balance</button>
                    </div>
                    <div style="clear:both"></div>*@

                        <div id="divCompanyWideInventoryBalancePriceLevel" class="d-none">
                            <h4>Price Levels</h4><br />

                            <div class="table-responsive">
                                <table data-role="table" data-mode="reflow" style="margin-right:5px; display:table; width:100%;" class="ui-table ui-responsive table-stroke table-stripe table table-light table-striped table-hover headerfitwidth">
                                    <thead>
                                        <tr class="align-top text-center" style="font-size:14px;">
                                            <th>#</th>
                                            <th>Price Level</th>
                                            <th>Price</th>
                                        </tr>
                                    </thead>
                                    <tbody id="tblBodyProductPrice"></tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                    <div class="popup-footer align-right">
                        <button id="btnCancelContact" data-role="none" class="btn custom-btn-theme-c btnCircle popup-close">
                            <img alt="Options" src="/Content/images/close-black.png" />
                        </button>
                    </div>
                </div>
            </div>
        </div>
        <div id="divForInventoryLedger" style="display:none;"></div>
        <div id="popupViewInventoryLedger-bs" data-backdrop="true" class="modal fade" style="display:none;">
            <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">View Inventory Ledger</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body" id="popupViewInventoryLedgercontent-bs">
                        
                    </div>
                </div>
            </div>
        </div>
        <div id="popupViewInventoryLedger" custom-data-role="popup" style="display:none;">
            <div class="popup">
                <div class="popup-header">
                    View Inventory Ledger
                    <a class="popup-close">x</a>
                </div>
                <div class="popup-content" id="popupViewInventoryLedgercontent">

                </div>
                <div class="popup-footer align-right">
                    <button data-role="none" class="popup-close btn custom-btn-theme-c btnCircle">
                        <img alt="Options" src="/Content/images/close-black.png" />
                    </button>
                </div>
            </div>
        </div>        
    </div>
</div>