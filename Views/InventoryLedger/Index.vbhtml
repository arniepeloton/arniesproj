@*@Code
        ViewData("Title") = "Inventory Ledger"
        Layout = "~/Views/Shared/Mobiz.vbhtml"
    End Code

    @Section Header

    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <link rel="stylesheet" href="/Content/metro-calendar.css" />
    End Section*@

<script>
    require([GetPathFormat('/Scripts3/InventoryLedger.js?v=' + assetversion)]);
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

<div id="pageInventoryReport" style="display:grid;">

    <div data-role="content" style="display:grid;">
        <div class="row">
            <div class="col-6 col-md-4 col-lg-auto mb-3">
                FOWST: <br />
                <button id="btnInventoryLedgerFowst" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                    <span class="lblSelectedFowstGroup">FOWST Location</span>
                </button>
            </div>
            <div class="col-6 col-md-4 col-lg-auto mb-3">
                Date: <br />
                <button id="btnInventoryLedgerDateRange" style="margin-right:10px;" data-role="none" class="btn btn-less-width custom-btn-theme-c btn-secondary btn-block">
                    Date Range
                </button>
            </div>
            <div id="divILSearch" class="col-12 col-md-4 mb-3">
                Search Product: <br />
                <input data-role="none" type="text" id="txtILSearch" class="form-control" placeholder="Search Product here" readonly />
            </div>
            <div class="col-12 col-md-4 mb-3">
                <br class="d-none d-md-block" />
                <button id="btnBeginningBalance" data-role="none" class="btn btn-success btn-fixed-width custom-btn-theme-b btn-less-width btn-block mb-3" style="display: none;">
                    Set Beg Balance <span class="lblAddLibraryName"></span>
                </button>

                <button style="display:none;" id="btnInventoryLedgerGO" data-role="none" class="btn btn-success btn-fixed-width custom-btn-theme-b btn-less-width btn-block">
                    GO <span class="lblAddLibraryName"></span>
                </button>
            </div>
        </div>

        <div id="divSecond" style="display:block;">
            <div id="dvSummaryIL" style="width:100%; overflow:auto; display:none">
                <table style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe table table-bordered">
                    <thead>
                        <tr id="tblHeader" class="custom-theme-f">
                            <th colspan="4"> Ledger Summary   (Click on a transaction type below to view its detailed ledger)</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td colspan="3"> Beg. Balance for the Period(Last Ending Balance Before <span id="LastBalance"></span>) </td>
                            <td> <span id="spBeginningBalance"> </span></td>
                        </tr>
                        <tr>
                            <td colspan="3"> Ending Balance for the Period </td>
                            <td> <span id="spEndingBalance"> </span></td>
                        </tr>
                        <tr>
                            <th colspan="2">Deductions</th>
                            <th colspan="2">Additions</th>
                        </tr>
                        <tr>
                            <td id="lnkGrossSales" name="mFilter" style="text-decoration:underline; cursor:pointer;"> Gross Sales </td>
                            <td> <span id="spGrossSales"> </span></td>
                            <td id="lnkSalesReturns" name="mFilter" style="text-decoration:underline; cursor:pointer;"> Sales Returns </td>
                            <td> <span id="spSalesReturns"> </span></td>
                        </tr>
                        <tr>
                            <td id="lnkStockWidrawal" name="mFilter" style="text-decoration:underline; cursor:pointer;"> Stock Transfers (Withdrawals) </td>
                            <td> <span id="spStockTransfers"> </span></td>
                            <td id="lnkStockReceiving" name="mFilter" style="text-decoration:underline; cursor:pointer;"> Stock Receiving </td>
                            <td> <span id="spStockReceiving"> </span></td>
                        </tr>
                        <tr>
                            <td id="lnkUsage" name="mFilter" style="text-decoration:underline; cursor:pointer;"> Usage </td>
                            <td> <span id="spUsage"> </span></td>
                            <td id="lnkProduction" name="mFilter" style="text-decoration:underline; cursor:pointer;"> Production </td>
                            <td> <span id="spProduction"> </span></td>
                        </tr>
                        <tr>
                            <td id="lnkInventoryCountVariances" name="mFilter" style="text-decoration:underline; cursor:pointer;"> Total Inventory Count Variances </td>
                            <td> <span id="spInventoryCountVariancesNegative"> </span></td>
                            @*<td id="lnkInventoryCountVariances" name="mFilter" style="text-decoration:underline; cursor:pointer;"> Total Inventory Count Variances </td>
                                <td> <span id="spInventoryCountVariances"> </span></td>*@
                            <td id="lnkInventoryCountVariances2" name="mFilter" style="text-decoration:underline; cursor:pointer;"> Total Inventory Count Variances </td>
                            <td> <span id="spInventoryCountVariancesPositive"> </span></td>
                        </tr>
                        <tr>
                            <td> Net Sales </td>
                            <td> <span id="spNetSales"> </span></td>
                        </tr>
                    </tbody>
                </table>
                <br /><br />
            </div>
        </div>

        <div id="dvDetailsIL" subpage="pageInventoryLedgerReport" style="width:100%; overflow:auto; display:none">
            <table data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe table table-striped table-hover">
                <thead>
                    <tr id="tblHeader" class="custom-theme-f">
                        <th data-sort="string" style="cursor:pointer;">Bin</th>
                        <th data-sort="string" style="cursor:pointer;">Transaction Reference</th>
                        <th data-sort="string" style="cursor:pointer;">Customer/Vendor</th>
                        <th data-sort="string" style="cursor:pointer;">Transaction Type</th>
                        <th data-sort="float" style="cursor:pointer;">Movement</th>
                        <th data-sort="float" style="cursor:pointer;">Price</th>
                        <th data-sort="float" style="cursor:pointer;">Balance</th>
                        <th data-sort="date" style="cursor:pointer;">Posting Date on Server</th>
                        <th data-sort="date" style="cursor:pointer;">Updated By</th>
                    </tr>
                </thead>
                <tbody id="tblBodyIL"></tbody>
            </table>
        </div>

        <div id="divNoPageIL" subpage="pageInventoryLedgerReport" style="display:block; text-align:center">
            <h4>No Data Available</h4>
        </div>

        <div id="divSetBagContent" subpage="pageInventoryLedgerReport" style="display:none;">
            SKU Name: <span class="lblSkuName" style="font-weight:bold;"></span>
            <br /><br />
            FOWST: <span class="lblSelectedFowstGroup" style="font-weight:bold;"></span>
            <br /><br />
            <span id="spanILError" style="display:none; color:#F00;">Opps! You need to set balance on desired BIN Location. <br /></span>
            <table id="tableSetBal" class="table table-light table-striped table-hover">
                <thead>
                    <tr align="center">
                        <th>Set Balance</th>
                        <th>Unit Measure</th>
                        <th>Date and Time</th>
                        <th>BIN Location</th>
                    </tr>
                </thead>
                <tbody id="tBodySetBegBal"></tbody>
            </table>
        </div>

        <div id="panSetBegBalanceInventoryLedger-bs" data-backdrop="true" class="modal fade" style="display:none;">
            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Set Beginning Balance <span class="lblPopUpHeaderLibraryName"></span></h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body" id="panSetBegBalanceInventoryLedgercontent-bs">

                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col">
                                <button data-role="none" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col">
                                <button data-role="none" class="btn btn-success btn-block btnSetBeginningBalance">Save</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div data-role="popup" id="panSetBegBalanceInventoryLedger" data-overlay-theme="a" data-theme="a" data-corners="false">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="btnClosePopUp">Close</a>
            <center><h3 style="margin-left:10px; margin-right:10px;">Set Beginning Balance<span class="lblPopUpHeaderLibraryName"></span></h3></center>
            <div data-role="content" class="ui-content ui-body-d">
                <div style="margin-left:10px; margin-right:10px;">
                    <div id="panSetBegBalanceInventoryLedgercontent">

                    </div>

                    <div style="text-align:right;">
                        <button data-role="none" class="btn custom-btn-theme-b btn-less-width btnSetBeginningBalance">
                            Save
                        </button>
                    </div>

                    @*<input type="text" id="txtBeginningBalance" />

                        <center><h3 style="margin-left:10px; margin-right:10px;">Apply to Bin: <span class="lblPopUpHeaderLibraryName"></span></h3></center>
                        <select id="selBins"> </select>

                        <br />
                        <br />
                        <div class="ui-grid-solo">
                            <div class="ui-block-a">
                                <button id="btnSetBeginningBalance" data-role="none" class="btn custom-btn-theme-b">
                                    Set Beginning Balance
                                </button>
                            </div>
                        </div>*@
                </div>
                <br />
            </div>
        </div>

        @*<div data-role="collapsible" data-collapsed="false" data-theme="c" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">

            <h4>
                <label>
                    <h4>Transaction Details</h4>
                </label>
            </h4>

            @*<div id="dvDetails" style="visibility:hidden; display:none">

                <br />

                <div data-role="fieldset">
                    <label for="btnSetBeginningBalance">Set Beginning Balance</label>
                    <fieldset data-role="controlgroup">
                        <a data-role="button" id="btnSetBeginningBalance"> (SET) </a>
                    </fieldset>

                </div>

                <br />

                <div data-role="fieldcontain">
                    <label for="selFilter" style="font-weight:bold">Filter Transaction By: </label>
                    <select id="selFilter">
                        <option>None</option>
                        <option>Sales Return</option>
                        <option>Receiving</option>
                        <option>Invoice</option>
                        <option>Inventory Count</option>
                    </select>
                </div>

                <br />

                <div id="divTable">
                </div>

                <br /> <br />
                <hr />
                <br />
                <div>
                    <h3>Summary</h3> <br />
                    <table width="800">
                        <tbody id="tblSummary" style="font-weight:bold"></tbody>
                    </table>
                </div>
                <br /> <br />
            </div>*@

        @*<div data-role="popup" id="panCustomDates" data-overlay-theme="a" data-theme="a" data-corners="false">
                <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
                <center><h3>Custom Period</h3></center>
                <div data-role="content">
                    <div class="ui-grid-a ui-responsive">
                        <div class="ui-block-a" style="padding:7px;">
                            <div class="metro" style="max-width:252px; overflow:auto;">
                                From: <span id="txtDateFrom"></span>
                                <div class="fromCalendar calendar"></div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="padding:7px;">
                            <div class="metro" style="max-width:252px; overflow:auto;">
                                To: <span id="txtDateTo"></span>
                                <div class="toCalendar calendar"></div>
                            </div>
                        </div>
                    </div>
                    <br />
                    <div class="ui-grid-a ui-responsive">
                        <div class="ui-block-a" style="padding:0px 7px 7px 7px;">
                            <a id="btnCancel" class="btn custom-btn-theme-c">
                                Cancel
                            </a>
                        </div>
                        <div class="ui-block-b" style="padding:0px 7px 7px 7px;">
                            <a id="btnDone" class="btn custom-btn-theme-g">
                                Done
                            </a>
                        </div>
                    </div>
                </div>
            </div>*@
    </div>
</div>
