@Code
    ViewData("Title") = "Monies for Deposit or Turn Over"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code

@Section head

    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v='], function () {
                require([GetPathFormat('/Scripts/sources/CashHandling/MoniesForDepositOrTurnOver.js?crc=')]);
            })
        })        
    </script>
End Section

@*Header*@

<div id="pageMain">
    <nav class="navbar navbar-dark bg-primary mb-3 sticky-top" style="padding:0px !important;">
        <div class="row" style="margin:0px !important;">
            <div class="col-3 col-md-2" style="padding:0px !important;">
                <button data-role="none" id="btnPOSSalesReportCancel" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                    <span id="iconHome" class="bi-house-fill"></span>
                    <div id="lblHome" class="menu-description mt-1">Home</div>
                </button>
            </div>
            <div class="col-3 col-md-2" style="padding:0px !important;">
                <button data-role="none" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0 btnCashierSelectFowst">
                    <span class="bi-building"></span>
                    <div class="menu-description mt-1 lblCashierSelectedFowstGroup">FOWST Location</div>
                </button>
            </div>
            <div class="col-3 col-md-2" style="padding:0px !important;">
                <button data-role="none" id="btnViewMoreDetails" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                    <span class="bi-eye"></span>
                    <div class="menu-description mt-1">View Details</div>
                </button>
            </div>
            <div class="col-3 offset-md-3 col-md-3" style="padding:0px !important;">
                <button data-role="none" id="btnDepositMonies" style="height:100%;" class="btn btn-success btn-block btn-lg rounded-0">
                    <span class="bi-cash"></span>
                    <div class="menu-description mt-1">Deposits/Turn-over Monies</div>
                </button>
            </div>
        </div>
        <br />
        <div class="row" style="margin:0px; padding:0.5em; width:100%; color:#FFF; background-color:#1969de; font-weight:bold;">
            <div class="offset-5 col-7" style="text-overflow:ellipsis; overflow:hidden; text-align:right; white-space:nowrap;">
                <span id="clockMonies">&nbsp;</span>
            </div>
        </div>
    </nav>

    <div class="container-fluid">                

        <div id="divMoniesHeader" style="display:block;">
            <div style="width:100%" id="dvReportHeader">
                <div style="float:left">
                    <button style="margin-right:10px; display:none;" data-role="none" class="btn btn-less-width btn-transparent btnRequired btnCashierSelectFowst">
                        <span class="lblCashierSelectedFowstGroup"> FOWST Location </span>
                        <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span>
                    </button>
                </div>

                <div id="divAddLibrary" style="float:left">
                    <div style="float:left">
                        <button id="btnCashierDateRange" style="margin-right:10px;visibility:hidden; display:none;" data-role="none" class="btn btn-less-width btn-transparent btnNotRequired">
                            <span id="CashierDateRangeFrom">Today</span>
                            <span id="CashierDateRangeSeparator" style="display:none;">to</span>
                            <span id="CashierDateRangeTo"></span>
                            <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span>
                        </button>
                        @*<label for="selPOSSalesReportRange">Show sales report range</label>*@
                        <div style="visibility:hidden; display:none;">
                            <select id="selPOSSalesReportRange">
                                <option value="">Please Select</option>
                                <option value="Today">Today</option>
                                <option value="Yesterday">Yesterday</option>
                                <option value="Day">Range by day</option>
                                <option value="Week">Range by week</option>
                                <option value="Month">Range by month</option>
                                <option value="Quarter">Range by quarter</option>
                                <option value="Year">Range by year</option>
                            </select>
                        </div>
                    </div>
                </div>
                <div style="clear:both;"></div>
            </div>
            @*<br />  <br />*@
            
            <div class="row mb-5">
                <button data-role="none" class="btn btn-success" id="btnPrint">Print</button>
                <div class="offset-sm-3 col-sm-6">
                    <div style="width:100%; text-align:center;">
                        <label style="font-weight:bold; font-size:30px;">Monies Turnover</label>
                        <br />
                        From: <label id="txtCurrentUserName"></label>
                    </div>
                    <br />
                    <div class="row mb-3" id="btnSalesDetails">
                        <div class="col-sm-8">Gross Sales: <a data-role="none" class="custom-link" style="font-size:14px;">(Click to view sales details)</a></div>
                        <div class="col-sm-4" id="txtGrossSales" style="text-align:right;">0</div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-sm-8">Less Sales Discounts:</div>
                        <div class="col-sm-4" id="txtSalesDiscounts" style="text-align:right;">0</div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-sm-8">Less VAT Exempt Tax:</div>
                        <div class="col-sm-4" id="txtVatExemptTax" style="text-align:right;">0</div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-sm-8">Less Sales Returns:</div>
                        <div class="col-sm-4" style="text-align:right;">
                            <div id="txtSalesReturns" style="border-bottom:1px solid #000; min-width: 100px; float: right; padding-bottom: 10px;">0</div>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-sm-8" style="font-size:20px; font-weight:bold;">Net Sales:</div>
                        <div class="col-sm-4" id="txtNetSales" style="text-align:right;font-size:20px; font-weight:bold;">0</div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-sm-8">Amount of Monies Received from Co-Employees:</div>
                        <div class="col-sm-4" style="text-align:right;">
                            <div id="txtMoniesReceived" style="border-bottom:1px solid #000; min-width: 100px; float: right; padding-bottom: 10px;">0</div>
                        </div>
                    </div>
                    <br />
                    @*<div>
                <div>Monies Received From:</div>
                <div style="text-align:right;">-</div>
            </div>*@

                    <div class="row mb-3">
                        <div class="col-sm-8" style="font-size:20px; font-weight:bold;">Total Of Net Sales And Monies Received from Co-Employees:</div>
                        <div class="col-sm-4" id="txtTotalNetSalesMoniesReceived" style="text-align:right;font-size:20px; font-weight:bold;">0</div>
                    </div>

                    <hr />

                    <div class="row mb-3">
                        <div class="col-sm-8">Deposits To Bank And 3rd Parties:</div>
                        <div class="col-sm-4" id="txtDepositsToBank3rd" style="text-align:right;">0</div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-sm-8">Service Charge(s):</div>
                        <div class="col-sm-4" style="text-align:right;">
                            <div id="txtServiceCharge" style="border-bottom:1px solid #000; min-width: 100px; float: right; padding-bottom: 10px;">0</div>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-sm-8" style="font-size:20px; font-weight:bold;">Total Of Deposits And Service Charges:</div>
                        <div class="col-sm-4" id="txtTotalDepositsAndServiceCharge" style="text-align:right;font-size:20px; font-weight:bold;">0</div>
                    </div>

                    <hr />

                    <div class="row mb-3">
                        <div class="col-sm-8">Total Available Monies After Deposits And Service Charges:</div>
                        <div class="col-sm-4" id="txtTotalAvailableMoniesAfter" style="text-align:right;">0</div>
                    </div>
                    <br>
                    <div class="row mb-3">
                        <div class="col-sm-8">Monies Turned Over To Co-Employees:</div>
                        <div class="col-sm-4" style="text-align:right;">
                            <div id="txtTurnedOverToCoEmployees" style="border-bottom:1px solid #000; min-width: 100px; float: right; padding-bottom: 10px;">0</div>
                        </div>
                    </div>

                    @*<div>
                <div>Turned Over To:</div>
                <div id="txtTurnedOverTo" style="text-align:right;">-</div>
            </div>*@
                    <br />

                    <div class="row">
                        <div class="col-sm-8" style="font-weight:bold; font-size:20px; color:#FF5858;">Shortage/Overage of Monies Turned Over and Deposited For The Day: <span style="font-size:14px; color: #333;">(* "-" sign means shortage while a "+" means overage)</span></div>
                        <div class="col-sm-4" id="txtGrossVariance" style="font-weight:bold; text-align:right; color:#FF5858;">0</div>
                    </div>

                    <hr />

                    <div class="row">
                        <div class="col-sm-8" style="font-weight:bold; font-size:18px; color:#9E1E1E;">Accumulated Shortage/Overage of Monies Turned Over and Deposited: <span style="font-size:14px; color: #333;">(* "-" sign means shortage while a "+" means overage)</span></div>
                        <div class="col-sm-4" id="txtAccumulatedGrossVariance" style="font-weight:bold; text-align:right; color:#9E1E1E;">0</div>
                    </div>

                    <div style="display:none;">
                        <div style="text-align:center"> <a id="btnViewMoreDetailsX" data-role="button"><h3>View Details</h3></a> </div>
                        <div style="text-align:center" id="dvDepositMonies"> <a id="btnDepositMoniesX" data-role="button"><h3>Deposit/Turn-over Monies</h3></a> </div>
                        <div style="text-align:center"> <a id="btnPOSSalesReportCancelX" data-role="button" data-rel="back"><h3>Go Back To Main</h3></a></div>
                    </div>

                </div>
            </div>            
        </div>

        <div id="divViewDetails" style="display:none;">
            <h2 id="txtPersonName" class="mb-3"></h2>

            <ul class="nav nav-tabs" id="myTab" role="tablist">
                <li class="nav-item" role="presentation">
                    <a class="nav-link active" data-toggle="tab" data-target="#divDetails" role="tab" aria-controls="divDetails" aria-selected="true">All Details</a>
                </li>
                <li class="nav-item" role="presentation">
                    <a class="nav-link" data-toggle="tab" data-target="#divTurnover" role="tab" aria-controls="divTurnover" aria-selected="false">Turnover</a>
                </li>
                <li class="nav-item" role="presentation">
                    <a class="nav-link" data-toggle="tab" data-target="#divDeposit" role="tab" aria-controls="divDeposit" aria-selected="false">Deposit</a>
                </li>
            </ul>

            <div class="tab-content">
                <div class="tab-pane fade pt-3 show active" id="divDetails" style="overflow:auto;">
                    <table data-role="none" style="display:table;" class="table table-light table-striped">
                        <thead>
                            <tr class="custom-theme-f" style="font-size:14px !important; text-align:center !important;">
                                <th style="text-align:center; min-width:150px;">Date</th>
                                <th style="text-align:center; min-width:150px; display:none;">Person</th>
                                <th style="background-color:#63E07A; color:#144809; text-shadow:0px 0px 0px; font-weight:normal; text-align:center; min-width:150px;">Gross Sales</th>
                                <th style="background-color:#63E07A; color:#144809; text-shadow:0px 0px 0px; font-weight:normal; text-align:center; min-width:150px;">Sales Discounts</th>
                                <th style="background-color:#63E07A; color:#144809; text-shadow:0px 0px 0px; font-weight:normal; text-align:center; min-width:150px;">Vat Exempt Tax</th>
                                <th style="background-color:#63E07A; color:#144809; text-shadow:0px 0px 0px; font-weight:normal; text-align:center; min-width:150px;">Sales Returns</th>
                                <th style="background-color:#63E07A; color:#144809; text-shadow:0px 0px 0px; font-weight:normal; text-align:center; min-width:150px;">Net Sales</th>
                                <th style="background-color:#63E07A; color:#144809; text-shadow:0px 0px 0px; font-weight:normal; text-align:center; min-width:150px;">Amount of Monies Received</th>
                                <th style="background-color:#63E07A; color:#144809; text-shadow:0px 0px 0px; font-weight:normal; text-align:center; min-width:150px;">Monies Received From</th>
                                <th style="background-color:#298017; color:#FFF; text-shadow:0px 0px 0px; font-weight:normal; text-align:center; min-width:200px;">Total Of Net Sales And Monies Received</th>
                                <th style="background-color:#4979EA; font-weight:normal; text-shadow:0px 0px 0px; text-align:center; min-width:150px;">Deposits To Bank And 3rd Parties</th>
                                <th style="background-color:#4979EA; font-weight:normal; text-shadow:0px 0px 0px; text-align:center; min-width:150px;">Service Charge(s)</th>
                                <th style="background-color:#4979EA; font-weight:normal; text-shadow:0px 0px 0px; text-align:center; min-width:150px;">Total Of Deposits And Service Charges</th>
                                <th style="background-color:#4979EA; font-weight:normal; text-shadow:0px 0px 0px; text-align:center; min-width:200px;">Total Available Monies After Deposits And Service Charges</th>
                                <th style="background-color:#4553FF; font-weight:normal; text-shadow:0px 0px 0px; text-align:center; min-width:150px;">Monies Turned Over To Co-Employees</th>
                                <th style="background-color:#4553FF; font-weight:normal; text-shadow:0px 0px 0px; text-align:center; min-width:150px;">Turned Over To</th>
                                <th style="background-color:#DC4242; color:#fff; font-weight:normal; text-shadow: 0px 0px 0px; text-align:center; min-width:200px;">Gross Variance Of Monies Not Turned Over</th>
                            </tr>
                        </thead>

                        <tbody id="tbodyDetails"></tbody>
                    </table>
                </div>

                <div class="tab-pane fade pt-3" id="divTurnover" style="overflow:auto;">
                    <table data-role="none" style="display:table;" class="table table-striped">
                        <thead>
                            <tr id="" style="background-color:#4553FF; font-weight:normal; text-shadow:0px 0px 0px; text-align:center; color:#FFF;">
                                <th data-sort="date">Date and Time</th>
                                <th data-sort="string">Turnedover By</th>
                                <th data-sort="string">Turnedover To</th>
                                <th data-sort="float">Turnedover Amount</th>
                            </tr>
                        </thead>
                        <tbody id="tBodyTurnOverDetails"></tbody>
                    </table>
                </div>

                <div class="tab-pane fade pt-3" id="divDeposit" style="overflow:auto;">
                    <table data-role="none" style="display:table;" class="table table-striped">
                        <thead>
                            <tr id="" style="background-color:#4979EA; font-weight:normal; text-shadow:0px 0px 0px; text-align:center; color:#FFF;">
                                <th data-sort="date">Date and Time</th>
                                <th data-sort="string">Deposited By</th>
                                <th data-sort="string">Deposited To</th>
                                <th data-sort="float">Deposited Amount</th>
                                <th data-sort="float">Service Charge</th>
                                <th data-sort="string">Deposit Reference No</th>
                                <th data-sort="string">Bank Name</th>
                                <th data-sort="string">Bank Branch</th>
                            </tr>
                        </thead>
                        <tbody id="tBodyDepositDetails"></tbody>
                    </table>
                </div>
            </div>

            <div class="mt-5 mb-5 text-center">
                <button id="btnBackMonies" data-role="none" class="btn btn-secondary btn-lg">Back</button>
            </div>
        </div>

        <div id="divViewSalesDetails" style="display:none;">
            <div id="divMainSalesDetails" style="display:block;">
                <h3 class="mb-3">Sales Details</h3>
                <div style="overflow:auto;">
                    <table data-role="none" style="display:table;" class="table table-light table-striped">
                        <thead>
                            <tr class="custom-theme-f">
                                <th style="text-align:center;">Invoice No.</th>
                                <th style="text-align:center;">Invoice Date</th>
                                <th style="text-align:center;">Customer</th>
                                <th style="text-align:center;">FOWST</th>
                                <th style="text-align:center;">SKU Count</th>
                                <th style="text-align:center;">Invoice Amount</th>
                                <th style="text-align:center;">Updated</th>
                            </tr>
                        </thead>

                        <tbody id="tbodySalesDetails"></tbody>
                    </table>
                </div>
                
                <div class="mt-5 mb-5 text-center">
                    <button id="btnBackSalesDetails" data-role="none" class="btn btn-secondary btn-lg">Back</button>
                </div>
            </div>
            <div id="divInvoiceDetails" style="display:none;">
                <h3 class="mb-3">Details for Invoice No. <span id="txtInvoiceNo"></span></h3>
                <div style="overflow:auto;">
                    <table data-role="none" style="display:table;" class="table table-light table-striped">
                        <thead>
                            <tr class="custom-theme-f">
                                <th style="text-align:center;">Invoice No.</th>
                                <th style="text-align:center;">Invoice Date</th>
                                <th style="text-align:center;">Product Description</th>
                                <th style="text-align:center;">Qty</th>
                                <th style="text-align:center;">Unit Measure</th>
                                <th style="text-align:center;">Rebate Product For Rebate</th>
                                <th style="text-align:center;">Rebate Qty</th>
                                <th style="text-align:center;">Rebate Unit Measure</th>
                                <th style="text-align:center;">Amount</th>
                                <th style="text-align:center;">Sub Total</th>
                                <th style="text-align:center;">Sales Representatives</th>
                                <th style="text-align:center;">Customer</th>
                                <th style="text-align:center;">Last Updated By</th>
                            </tr>
                        </thead>

                        <tbody id="tbodyInvoiceDetails"></tbody>
                    </table>
                </div>
                
                <div class="mt-5 mb-5 text-center">
                    <button id="btnBackInvoiceDetails" data-role="none" class="btn btn-secondary btn-lg">Back</button>
                </div>
            </div>
        </div>

        <table style="display:none; visibility:hidden;">
            <tr>
                <td colspan="2" style="text-align:center">
                    <h1>Monies for Deposit or Turn Over</h1>
                    <label id="lblDate"> </label>
                </td>
            </tr>

            <tr>
                <td>
                    <br /> <br />
                    <h3> Total Amount of Monies From Sales That Are Undeposited/Unturned Over </h3>
                </td>
                <td style="text-align:right">
                    <br /> <br /> &nbsp; &nbsp; &nbsp; &nbsp; 
                    <h3 id="lblTotalUndepositedAmountofMoniesFromSales"> </h3>
                </td>
            </tr>
                        
            <tr>      
                <td colspan="2" style="text-align:center; font-weight:bold"> <br /> <br /> Breakdown of Monies from Sales that are Undeposited / Unturned Over </td>
            </tr>

            <tr>
                <td colspan="2">
                    <br />

                    <table width="100%" align="center">
                        @*<thead>
                            <tr>
                                <th>SI #</th>
                                <th>Payment Type</th>
                                <th>Amount</th>
                            </tr>
                        </thead>*@
                        <tbody id="tbBreakDownofMoniesFromSales">
                            <tr>
                                <td>
                                    <h5> Gross Sales : </h5>
                                </td>
                                <td style="text-align:right">
                                    <label id="lblGrossSales"> </label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <h5> Less Returns : </h5>
                                </td>
                                <td style="text-align:right">
                                    <label id="lblReturns"> </label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <h5> Less Discounts : </h5>
                                </td>
                                <td style="text-align:right">
                                    <label id="lblDiscounts"> </label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <h4> Net Sales : </h4>
                                </td>
                                <td style="text-align:right">
                                    <label id="lblNetSales" style="font-size:x-large"> </label>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>

            <tr>
                <td colspan="2">
                    <br />
                     <hr />
                </td>
            </tr>

            <tr>
                <td>
                    <br /> <br />
                    <h3> Total Amount of Monies Accepted From Co-Employees that are Undeposited or UnTurned Over </h3>
                </td>
                <td style="text-align:right">
                    <br /> <br /> &nbsp; &nbsp; &nbsp; &nbsp; 
                    <h3 id="lblTotalUndepositedAmountofMoniesFromCoEmployees"> </h3>
                </td>
            </tr>

            <tr>
                <td colspan="2" style="text-align:center; font-weight:bold"> <br /> <br /> Co-Employees that are Undeposited/Unturned Over </td>
            </tr>

            <tr>
                <td colspan="2">
                    <br /> 
                    <table width="100%">
                        <thead>
                            <tr>
                                <th>Received From</th>
                                <th>Time</th>
                                <th>Amount</th>
                            </tr>
                        </thead>
                        
                        <tbody id="tbBreakDownofMoniesAccepted"></tbody>
                    </table>
                </td>
            </tr>

            <tr>
                <td colspan="2">
                    <br />
                    <hr />
                </td>
            </tr>

            <tr>
                <td>
                    <br /> <br />
                    <h3> Total Amount of Monies Accepted from Sales and Accepted Monies that are Undeposited/Unturned Over </h3>
                </td>
                <td style="text-align:right">
                    <br /> <br /> &nbsp; &nbsp; &nbsp; &nbsp; 
                    <h3 id="lblTotalUndepositedAmountofMoniesFromSalesAndAcceptedMonies"> </h3>
                </td>
            </tr>
        </table>

        <div data-role="popup" id="panCashierFowst" data-overlay-theme="a" data-theme="a" data-corners="false">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Select a Fowst</h3></center>
            <div data-role="content" class="ui-content ui-body-d">
                <div class="FowstGroupPopOp" style="padding:10px;">
                    <ul id="listCashierFowst" data-role="listview" class="noBorderList">
                        <li><a class="btn custom-btn-theme-c txt-left">Please wait</a></li>
                    </ul>
                </div>
            </div>
        </div>

        <div data-role="popup" id="panCashierDate" data-overlay-theme="a" data-theme="a" data-corners="false">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Period</h3></center>
            <div data-role="content" class="ui-content ui-body-d">
                <div class="DatesPopUp" style="padding:10px;">
                    <ul data-role="listview" class="noBorderList">
                        @*<li id="li2DaysAgo"><a href="#" class="btn custom-btn-theme-c txt-left" style="box-shadow:none !important; font-weight:bold !important; width:auto;">2 Days Ago</a></li>*@
                        <li id="liYesterday"><a href="#" class="btn custom-btn-theme-c txt-left" style="box-shadow:none !important; font-weight:bold !important; width:auto;">Yesterday</a></li>
                        <li id="liToday"><a href="#" class="btn custom-btn-theme-c txt-left" style="box-shadow:none !important; font-weight:bold !important; width:auto;">Today</a></li>
                        @*<li id="liWeekToDate"><a href="#" class="btn custom-btn-theme-c txt-left" style="box-shadow:none !important; font-weight:bold !important; width:auto;">Week To Date</a></li>
                        <li id="liMonthToDate"><a href="#" class="btn custom-btn-theme-c txt-left" style="box-shadow:none !important; font-weight:bold !important; width:auto;">Month To Date</a></li>*@
                        <li id="liCustomPeriod"><a href="#" class="btn custom-btn-theme-c txt-left" style="box-shadow:none !important; font-weight:bold !important; width:auto;">Custom Period</a></li>
                    </ul>
                </div>
            </div>
        </div>

        <div data-role="popup" id="panCashierCustomDate" data-overlay-theme="a" data-theme="a" data-corners="false">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Date Range</h3></center>
            <div data-role="content" class="ui-content ui-body-d">
                <span id="spanCashierDateRangeError" style="display:none; color:#F00;">Please select a valid date range. <br /></span>
                <div class="ui-grid-a ui-responsive">
                    <div class="ui-block-a" style="padding:7px;">
                        <div class="metro">
                            From: <span id="txtCashierDateFrom"></span>
                            <div class="CashierfromCalendar calendar"></div>
                        </div>
                    </div>
                    <div class="ui-block-b" style="padding:7px;">
                        <div class="metro">
                            To: <span id="txtCashierDateTo"></span>
                            <div class="CashiertoCalendar calendar"></div>
                        </div>
                    </div>
                </div>
                <br />
                <div class="ui-grid-a ui-responsive">
                    <div class="ui-block-a" style="padding:0px 7px 7px 7px;">
                        <a id="btnCashierCancel" class="btn custom-btn-theme-c">
                            Cancel
                        </a>
                    </div>
                    <div class="ui-block-b" style="padding:0px 7px 7px 7px;">
                        <a id="btnCashierDone" class="btn custom-btn-theme-g">
                            Done
                        </a>
                    </div>
                </div>
            </div>
        </div>

    </div>

</div>