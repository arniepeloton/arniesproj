@Code
    ViewData("Title") = "Adjustment Tool for E-Journal Entries"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code

<link rel="stylesheet" href="/Content/bootstrap-custom.css" />
<link rel="stylesheet" href="/Content/flatui-css-custom.css" />
<link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
<link rel="stylesheet" href="/Content/metro-calendar.css" />

<script>
    require([GetPathFormat('/Scripts/sources/Zapatero/TransactionDetail.js?crc=')]);
</script>

<div>

    <div class="container" style="margin-top:30px;">
        <div class="row">
            <div class="col-sm-6">
                <div class="row">
                    <div class="col-sm-6">
                        Machine Name:
                    </div>
                    <div class="col-sm-6">
                        <input data-role="none" type="text" id="lblMachineName" class="form-control" />
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-sm-6">
                        Machine ID Number (MIN):
                    </div>
                    <div class="col-sm-6">
                        <input data-role="none" type="text" id="lblMachineNo" class="form-control" disabled />
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-sm-6">
                        Branch:
                    </div>
                    <div class="col-sm-6">
                        <input data-role="none" type="text" id="lblBranch" class="form-control" disabled />
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-sm-6">
                        PTU:
                    </div>
                    <div class="col-sm-6">
                        <input data-role="none" type="text" id="lblRegistrationNo" class="form-control" disabled />
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-sm-6">
                        Month/Year:
                    </div>
                    <div class="col-sm-6">
                        <button data-role="none" class="btn custom-btn-theme-c" id="btnDateRange">Set Date Range</button>

                        @*<input data-role="none" type="text" class="form-control col-sm-5" style="float:left;" placeholder="Month" />
                        <input data-role="none" type="text" class="form-control col-sm-5" style="float:right;" placeholder="Year" />*@
                        @*<div data-role="fieldcontain" id="dvMonth">*@
                            
                            @*<fieldset data-role="controlgroup" data-type="horizontal">
                                <select id="selMonth" data-mini="true">
                                    <option value="" selected="selected">Month </option>
                                    <option value="1">January </option>
                                    <option value="2">February </option>
                                    <option value="3">March </option>
                                    <option value="4">April </option>
                                    <option value="5">May </option>
                                    <option value="6">June </option>
                                    <option value="7">July </option>
                                    <option value="8">August</option>
                                    <option value="9">September </option>
                                    <option value="10">October </option>
                                    <option value="11">November </option>
                                    <option value="12">December </option>
                                </select>
                                <select id="selYear" data-mini="true">
                                    <option value="" selected="selected">Year </option>
                                    <option value="2013">2013 </option>
                                    <option value="2014">2014 </option>
                                    <option value="2015">2015 </option>
                                    <option value="2016">2016 </option>
                                    <option value="2017">2017 </option>
                                    <option value="2018">2018 </option>
                                </select>
                            </fieldset>*@
                        @*</div>*@
                    </div>
                </div>
            </div>
            <div class="col-sm-6">
                <div class="row">
                    <div class="col-sm-6">
                        Original Gross Sales:
                    </div>
                    <div class="col-sm-6">
                        <input data-role="none" type="text" id="lblOriginalGross" class="form-control" disabled />
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-sm-6">
                        Target Gross Sales (Refer to Reported E-Sales):
                    </div>
                    <div class="col-sm-6">
                        <input data-role="none" type="text" class="form-control col-sm-5" style="float:left;" id="lblAdjustedGrossAmount" />
                        <input data-role="none" type="text" class="form-control col-sm-5" style="float:right;" id="lblAdjustedGrossPercentage"  />
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-sm-6">
                        Diff. Orig. vs Target Gross Sales:
                    </div>
                    <div class="col-sm-6">
                        <input data-role="none" type="text" id="lblDifference" class="form-control" disabled />
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-sm-6">
                        Actual Total of Best Fitted Data
                    </div>
                    <div class="col-sm-6">
                        <input data-role="none" type="text" id="lblActualTotalBestFitData" class="form-control" disabled />
                    </div>
                </div>
            </div>
        </div>
        <br />
        <div class="row">
            <div class="col-sm-3">
                <button data-role="none" class="btn custom-btn-theme-c" id="btnGetTransactionData">Retrieve Data</button> <span>-</span>                
            </div>
            <div class="col-sm-1">
                <button data-role="none" class="btn btn-transparent">
                    <span class="glyphicon glyphicon-chevron-right"></span>
                </button>
            </div>
            <div class="col-sm-3">
                @*<button data-role="none" class="btn custom-btn-theme-c" id="btnRecompute">Adjust Data for Best Fit</button> 
                <span><b>or</b></span>*@
                <button data-role="none" class="btn custom-btn-theme-c" id="btnRecomputeDiscount">Adjust Data using Discount</button>
            </div>
            <div class="col-sm-1">
                <button data-role="none" class="btn btn-transparent">
                    <span class="glyphicon glyphicon-chevron-right"></span>
                </button>
            </div>
            <div class="col-sm-4">
                <button data-role="none" class="btn custom-btn-theme-b" id="btnDownloadESalesData">Download E-Sales and E-Journal Data</button>
                @*<button data-role="none" class="btn custom-btn-theme-b" id="btnDownloadData"> Download Data</button> <span>-</span>
                <button data-role="none" class="btn custom-btn-theme-b" id="btnAdjustZRead"> Adjust Z-Read</button>*@
            </div>            
        </div>
        <br />
    </div>
            
    <div style="margin-top:20px; margin-bottom:20px">
        <div style="width:100%; overflow:auto;">

            <div id="divTabAssetCustomer" class="custom-tab">
                <ul>
                    <li class="active" custom-tab-content="#tabZRead">Z-Read</li>
                    <li custom-tab-content="#tabTransactionDetails">Transaction Details</li>
                </ul>
            </div>

            <div id="tabZRead" custom-tab-role="content" style="display:block">

                @*<div class="col-md-4 col-md-offset-8" style="margin-bottom:20px">
                    <button data-role="none" class="btn custom-btn-theme-b" id="btnSaveZReadData">Save Z-Read</button>
                </div>*@

                <div id="divTabAssetCustomer" class="custom-tab">
                    <ul>
                        <li class="active" custom-tab-content="#MonthlyZRead">Monthly Z-Read</li>
                        <li custom-tab-content="#DailyZRead">Daily Z-Read</li>
                    </ul>
                </div>

                <div id="MonthlyZRead" custom-tab-role="content" style="display:block">
                    <table id="tblMonthlyZRead" data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-shadow ui-responsive table-stroke table-stripe">
                        <thead>
                            <tr>
                                <th>Month</th>
                                <th>Year</th>
                                <th>Total Gross Sales</th>
                                <th>Total Discount</th>
                                <th>TotalExchange</th>
                                <th>Starting Invoice No</th>
                                <th>Ending Invoice No</th>
                            </tr>
                        </thead>
                        <tbody id="tblMonthlyZReadBody"></tbody>
                    </table>                    
                </div>

                <div id="DailyZRead" custom-tab-role="content" style="display:block">
                    <div>
                        Target Accumulated Total
                        <input data-role="none" type="text" id="txtCurTargetAccumulatedTotal" class="form-control" />
                    </div>

                    <div>
                        Previous Accummulated Total
                        <input data-role="none" type="text" id="txtPreviousAccumulatedTotal" class="form-control" />
                    </div>

                    <table id="tblZRead" data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-shadow ui-responsive table-stroke table-stripe">
                        <thead>
                            <tr id="tblHeaderDT" class="custom-theme-f txt-center">
                                <th>Sales Date</th>
                                <th>Old Grand Total(Original)</th>
                                <th>Old Grand Total(Presentation)</th>
                                <th>Adjusted Grand Total</th>
                                <th>Gross Sales(Original)</th>
                                <th>Gross Sales(Presentation)</th>
                                <th>Adjusted Gross Sales</th>
                                <th>Cash Sales</th>
                                <th>Credit Sales</th>
                                <th>Discounts</th>
                                <th>Refunds</th>
                                <th>Exchanges</th>
                                <th>Beginning Invoice No.</th>
                                <th>Ending Invoice No</th>
                                <th>Reported?</th>
                            </tr>
                        </thead>
                        <tbody id="tblZReadBody"></tbody>
                    </table>
                    
                    <br />

                    <button id="btnDownloadDailyZReadData" data-mini="true">Download Daily E-Journal</button>
                </div>
                   
                </div>

            <div id="tabTransactionDetails" custom-tab-role="content" style="display:none">

                <div class="col-md-4 col-md-offset-8" style="margin-bottom:20px">
                    <button data-role="none" class="btn custom-btn-theme-b" id="btnSaveTransactionData">Save Details</button>
                </div>

                <table id="tblTransactionDetail" data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-shadow ui-responsive table-stroke table-stripe">
                    <thead>
                        <tr id="tblHeaderDT" class="custom-theme-f txt-center">
                            <th>Product Name</th>
                            <th>Transaction No</th>
                            <th>Transaction Date</th>
                            <th>Customer</th>
                            <th>Qty</th>
                            @*<th>Edited Qty</th>*@
                            <th>Unit Measure</th>
                            <th>Unit Price</th>
                            @*<th>Edited Unit Price</th>*@
                            <th>SubTotal</th>
                            <th>Discount Amount</th>
                            <th>Cashier Name</th>
                            <th>CashAmount</th>
                            <th>Check Payment Amount</th>
                            <th>Debit Card Amount</th>
                            <th>Credit Card Amount</th>
                            <th>SRV Amount</th>
                            <th>On CreditAmount</th>
                            <th>Other Type Amount</th>
                        </tr>
                    </thead>
                    <tbody id="tblTransactionDetailsBody"></tbody>
                </table>
            </div>
        </div>
    </div>

    @*<div>
        <button id="btnSaveTransactionData" data-mini="true">Save changes in transaction data</button>
    </div>*@
           
    <div data-role="popup" id="panSetDiscount" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="btnClosePopUp">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;">Adjust Data for Best Fit<span class="lblPopUpHeaderLibraryName"></span></h3></center>
        <div data-role="content" style="max-width:350px;" class="ui-content ui-body-d" data-theme="a">
            <div style="margin-left:10px; margin-right:10px;">
                
                @*<center><h3 style="margin-left:10px; margin-right:10px;">Enter Discount in percent(%)<span class="lblPopUpHeaderLibraryName"></span></h3></center>
                <input type="number" data-clear-btn="true" id="txtDiscount" value="">*@

                <br />
                <div class="ui-grid-solo">
                    <div class="ui-block-a">
                        <button id="btnAdjustBestFirstByRemovingItems" data-role="none" class="btn custom-btn-theme-b">
                            By Removing Items And Adjusting Qty
                        </button>
                        <br /> <br />
                        <button id="btnAdjustBestFirstByApplyDiscount" data-role="none" class="btn custom-btn-theme-b">
                           By Applying Discount
                        </button>

                    </div>
                </div>
            </div>
            <br />
        </div>
    </div>

    <div data-role="popup" id="panDateRange" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3>Select Date Range</h3></center>
        <div data-role="content" style="font-size:20px;">
            <div id="dvCalendar" class="ui-body-d" style="padding:5px; word-wrap: break-word;">
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
            </div>

            <br />
            <a class="btn custom-btn-theme-b" id="btnRequisitionOk" style="border-radius:4px;height:30px !important;">
                Ok
            </a>
            <br />
            <br />
            <a class="btn custom-btn-theme-c" id="btnRequisitionCancel" style="border-radius:4px;height:30px !important;">
                Cancel
            </a>
        </div>
    </div>
</div>
