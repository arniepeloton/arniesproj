

    <script>
        require([GetPathFormat('/Scripts/sources/ReportsPOS/SalesReport.js?crc=' + assetversion),
            (IsMobile ? '' : '/Scripts/dom-to-image.min.js')]);
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

<div id="pageSalesReport">
    @*<div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>*@

    <div data-role="content">

        @*     @If Common.WebRequest.QueryString("IsMobile") <> "1" Then
            @<applet name="jzebra" code="jzebra.PrintApplet.class" archive="/Scripts/jzebra.jar" width="10" height="10">
            </applet>
        End If*@

        <div class="row m-0 gy-2 gx-3 mb-3 align-items-center">
            <div class="col-md-auto">
                <div class="row">
                    <label for="btnCashierSelectFowst" class="col-auto col-md-auto col-form-label">Store:</label>
                    <div class="col col-md-auto">
                        <button style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block btnCashierSelectFowst">
                            <span class="lblCashierSelectedFowstGroup"> Please select </span>
                        </button>
                    </div>
                </div>
            </div>
            <div class="col-md-auto d-none">
                <div class="row">
                    <label for="selXReadMachine" class="col-auto col-md-auto col-form-label">Machine:</label>
                    <div class="col col-md-auto">
                        <select data-role="none" class="form-select" id="selXReadMachine">
                            <option>(Select a store first)</option>
                        </select>
                    </div>
                </div>
            </div>
            <div class="col-md-auto d-none">
                <div class="row">
                    <label for="selXReadCashier" class="col-auto col-md-auto col-form-label">User:</label>
                    <div class="col col-md-auto">
                        <select data-role="none" class="form-select" id="selXReadCashier">
                            <option>(Select a store first)</option>
                        </select>
                    </div>
                </div>
            </div>
            <div class="col-md-auto">
                <div class="row">
                    <label for="btnSystemWideDate" class="col-auto col-md-auto col-form-label">Date:</label>
                    <div class="col col-md-auto">
                        <button id="btnCashierDateRange" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                            <span id="CashierDateRangeFrom">Today</span>
                            <span id="CashierDateRangeSeparator" style="display:none;">to</span>
                            <span id="CashierDateRangeTo"></span>
                        </button>
                    </div>
                </div>
            </div>
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


        <div id="divPOSSalesReportRangeByDate" style="display:none;">

            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <legend> From: : </legend>
                    <select id="selPOSSalesReportByDateFromMonth">
                        <option value="">MMM</option>
                    </select>
                    <select id="selPOSSalesReportByDateFromDay">
                        <option value="">DD</option>
                    </select>
                    <select id="selPOSSalesReportByDateFromYear">
                        <option value="">YYYY</option>
                    </select>
                </fieldset>
            </div>

            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <legend> To : </legend>
                    <select id="selPOSSalesReportByDateToMonth">
                        <option value="">MMM</option>
                    </select>
                    <select id="selPOSSalesReportByDateToDay">
                        <option value="">DD</option>
                    </select>
                    <select id="selPOSSalesReportByDateToYear">
                        <option value="">YYYY</option>
                    </select>
                </fieldset>
            </div>

            <div style="text-align:center"> <a class="btnPOSSalesReportView" data-role="button">View</a> </div>
        </div>

        <div id="divPOSSalesReportRangeByWeek" style="display:none;">

            <div data-role="fieldcontain">
                <label for="selPOSSalesReportRangeByWeekFrom">From:</label>
                <select id="selPOSSalesReportRangeByWeekFrom">
                    <option value="">Please Select</option>
                </select>
            </div>

            <div data-role="fieldcontain">
                <label for="selPOSSalesReportRangeByWeekTo">To:</label>
                <select id="selPOSSalesReportRangeByWeekTo">
                    <option value="">Please Select</option>
                </select>
            </div>

            <div data-role="fieldcontain">
                <label for="selPOSSalesReportRangeByWeekYear">Year:</label>
                <select id="selPOSSalesReportRangeByWeekYear">
                    <option value="">Please Select</option>
                    <option value="This year.">This year.</option>
                    <option value="This year and last year.">This year and last year.</option>
                    <option value="This year and last 2 years.">This year and last 2 years.</option>
                    <option value="This year and last 3 years.">This year and last 3 years.</option>
                </select>
            </div>

            <div style="text-align:center"> <a class="btnPOSSalesReportView" data-role="button">View</a> </div>
        </div>

        <div id="divPOSSalesReportRangeByMonth" style="display:none;">

            <div data-role="fieldcontain">
                <label for="selPOSSalesReportRangeByMonthFrom">From:</label>
                <select id="selPOSSalesReportRangeByMonthFrom">
                    <option value="">Please Select</option>
                </select>
            </div>

            <div data-role="fieldcontain">
                <label for="selPOSSalesReportRangeByMonthTo">To:</label>
                <select id="selPOSSalesReportRangeByMonthTo">
                    <option value="">Please Select</option>
                </select>
            </div>

            <div data-role="fieldcontain">
                <label for="selPOSSalesReportRangeByMonthYear">Year:</label>
                <select id="selPOSSalesReportRangeByMonthYear">
                    <option value="">Please Select</option>
                    <option value="This year.">This year.</option>
                    <option value="This year and last year.">This year and last year.</option>
                    <option value="This year and last 2 years.">This year and last 2 years.</option>
                    <option value="This year and last 3 years.">This year and last 3 years.</option>
                </select>
            </div>

            <div style="text-align:center"> <a class="btnPOSSalesReportView" data-role="button">View</a> </div>

        </div>

        <div id="divPOSSalesReportRangeByQuarter" style="display:none;">

            <div data-role="fieldcontain">
                <label for="selPOSSalesReportRangeByQtrFrom">From:</label>
                <select id="selPOSSalesReportRangeByQtrFrom">
                    <option value="">Please Select</option>
                    <option value="1">1st Quarter</option>
                    <option value="2">2nd Quarter</option>
                    <option value="3">3rd Quarter</option>
                    <option value="4">4th Quarter</option>
                </select>
            </div>

            <div data-role="fieldcontain">
                <label for="selPOSSalesReportRangeByQtrTo">To:</label>
                <select id="selPOSSalesReportRangeByQtrTo">
                    <option value="">Please Select</option>
                    <option value="1">1st Quarter</option>
                    <option value="2">2nd Quarter</option>
                    <option value="3">3rd Quarter</option>
                    <option value="4">4th Quarter</option>
                </select>
            </div>

            <div data-role="fieldcontain">
                <label for="selPOSSalesReportRangeByQtrYear">Year:</label>
                <select id="selPOSSalesReportRangeByQtrYear">
                    <option value="">Please Select</option>
                    <option value="This year.">This year.</option>
                    <option value="This year and last year.">This year and last year.</option>
                    <option value="This year and last 2 years.">This year and last 2 years.</option>
                    <option value="This year and last 3 years.">This year and last 3 years.</option>
                </select>
            </div>
            <div style="text-align:center"> <a class="btnPOSSalesReportView" data-role="button">View</a> </div>

        </div>

        <div id="divPOSSalesReportRangeByYear" style="display:none">

            <div data-role="fieldcontain">
                <label for="selPOSSalesReportRangeByYear">Year:</label>
                <select id="selPOSSalesReportRangeByYear">
                </select>
            </div>

            <div style="text-align:center"> <a class="btnPOSSalesReportView" data-role="button">View</a> </div>

        </div>

        <div id="divWelcomeMessage" subpage="pagesalesreport" style="display:block; text-align:center;">
            <h5>Please select store first</h5>
        </div>

        <div class="row justify-content-center m-3" id="divPrintReportContainer" subpage="pagesalesreport" style="display:none;">
            <div class="col-md-8 col-lg-5 rounded-1 bg-white" style="border:1px solid #ccc; padding:15px;">
                <div id="divDisplayPOSSalesReport" style="display:none">
                    <table id="tblPOSSalesReport" align="center" style="width:100%;">
                        <tr>
                            <td colspan="2" style="text-align:center">
                                <label id="lblOrganizationBusinessName" style="font-size:xx-large; font-weight:bolder"></label> <br />
                                <label id="lblOrganizationName"> </label> <br />
                                <label id="lblOrganizationFOWST"> </label> <br />
                                <label id="lblOrganizationFOWSTAddress"> </label>  <br />
                                <label id="lblOrganizationFOWSTTIN"> </label>  <br />
                                @*<label id="lblOrganizationFOWSTContactNumber"> </label>  <br />*@
                                <label id="lblOrganizationFOWSTMachineSN"> </label>  <br />
                                <label id="lblOrganizationFOWSTPTUNo"> </label>  <br />
                                <label id="lblOrganizationFOWSTMIN"> </label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="text-align:center">
                                <br />
                                <label id="lblReportType" style="font-size:xx-large; font-weight:bolder">CASHIER'S</label> <br />
                                <label id="lblReportRange" style="font-weight:bold">SALES REPORT</label> <br />
                                <label id="lblReportDate" style="font-weight:bold"> Date : 01/01/2014 </label>
                            </td>
                        </tr>

                        <tr id='lblCashierLabel'>
                            <td style="text-align:left">
                                <br />
                                Cashier :
                            </td>
                            <td style="text-align:right">
                                <br />
                                <label id="lblCashierName"> -  </label>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align:left; font-weight:bold">
                                <br />
                                Gross Sales :
                            </td>
                            <td style="text-align:right; font-weight:bold">
                                <br />
                                <label id="lblGrossSales"> - </label>
                            </td>
                        </tr>
                        <tr style="vertical-align:baseline">
                            <td style="text-align:left">
                                Total Sales Discount

                                <ul id="listDiscountBreakdown" class="mb-0 font-italic"></ul>
                            </td>
                            <td style="text-align:right">
                                <label id="lblTotalSalesDiscount"> - </label>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align:left">
                                Total VAT Exempt Tax
                            </td>
                            <td style="text-align:right">
                                <label id="lblTotalVATExemptAmount"> - </label>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align:left; font-weight:bold" colspan="2">
                                <br />
                                Summary of Adjustments
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align:left">
                                Total Sales Refund for the Period
                            </td>
                            <td style="text-align:right">
                                <label id="lblTotalSalesRefundforthePeriod"> - </label>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align:left">
                                Total Sales Returns Subject to Merchandise Exchange (SRV Issued) &nbsp; &nbsp; &nbsp;
                            </td>
                            <td style="text-align:right">
                                <label id="lblTotalSalesReturnsSubjecttoMerchandiseExchange"> - </label>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align:left; font-weight:bold">
                                Net of Adjustments:
                            </td>
                            <td style="text-align:right; font-weight:bold">
                                <label id="lblTotalNetofAdjustments"> - </label>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align:left; font-weight:bold" colspan="2">
                                <hr />
                            </td>
                        </tr>

                        <tr id="divCharges">
                            <td style="text-align:left">
                                Add Charges:
                            </td>
                            <td style="text-align:right">
                                <label id="lblTotalCharges"> - </label>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align:left; font-weight:bold">
                                Net Sales:
                            </td>
                            <td style="text-align: right; font-weight: bold; font-size: xx-large;">
                                <label id="lblTotalNetSales"> - </label>
                            </td>
                        </tr>
                        @*<tr>
            <td style="text-align:left; font-weight:bold">
                Add Used Up SRV
            </td>
            <td style="text-align:right; font-weight:bold">
                <label id="lblTotalIssuedSRV"> - </label>
            </td>
        </tr>
        <tr>
            <td style="text-align:left" colspan="2">
                <hr />
            </td>
        </tr>
        <tr>
            <td style="text-align:left; font-weight:bold">
                Net Sales Adjusted for Exchanges:
            </td>
            <td></td>
        </tr>
        <tr>
            <td></td>
            <td style="text-align:right">
                <label id="lblTotalNetSalesAdjustedforExchanges" style="font-size:xx-large; font-weight:bolder"> </label>
            </td>
        </tr>*@
                        <tr>
                            <td colspan="2" style="text-align:center">
                                <br />
                                <label style="font-weight:bold"> Inventory of Payments </label>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align:left; font-weight:bold">
                                <br />
                                Cash :
                            </td>
                            <td style="text-align:right; font-weight:bold">
                                <br />
                                <label id="lblTotalCash"> - </label>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align:left; font-weight:bold">
                                Check :
                            </td>
                            <td style="text-align:right; font-weight:bold">
                                <label id="lblTotalCheck"> - </label>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align:left; font-weight:bold">
                                Debit Card :
                            </td>
                            <td style="text-align:right; font-weight:bold">
                                <label id="lblTotalDebitCard"> - </label>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align:left; font-weight:bold">
                                Credit Card :
                            </td>
                            <td style="text-align:right; font-weight:bold">
                                <label id="lblTotalCreditCard"> - </label>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align:left; font-weight:bold">
                                SRV :
                            </td>
                            <td style="text-align:right; font-weight:bold">
                                <label id="lblTotalSRV"> - </label>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align:left; font-weight:bold">
                                On Credit :
                            </td>
                            <td style="text-align:right; font-weight:bold">
                                <label id="lblOnCredit"> - </label>
                            </td>
                        </tr>
                        @*<tr id="divOtherTender" style="display:none;">
            <td style="text-align:left; font-weight:bold">
                Other Tender :
            </td>
            <td style="text-align:right; font-weight:bold">
                <label id="lblTotalOtherTender"> - </label>
            </td>
        </tr>*@
                        <tr id="divSMAC" style="display:none;">
                            <td style="text-align:left; font-weight:bold">
                                SMAC :
                            </td>
                            <td style="text-align:right; font-weight:bold">
                                <label id="lblTotalSMAC"> - </label>
                            </td>
                        </tr>
                        <tr id="divEPlus" style="display:none;">
                            <td style="text-align:left; font-weight:bold">
                                E-Plus :
                            </td>
                            <td style="text-align:right; font-weight:bold">
                                <label id="lblTotalEPlus"> - </label>
                            </td>
                        </tr>
                        <tr id="divVoucher" style="display:none;">
                            <td style="text-align:left; font-weight:bold">
                                Voucher :
                            </td>
                            <td style="text-align:right; font-weight:bold">
                                <label id="lblTotalVoucher"> - </label>
                            </td>
                        </tr>
                        @*<tr id="divGCash" style="display:none;">
            <td style="text-align:left; font-weight:bold">
                GCash :
            </td>
            <td style="text-align:right; font-weight:bold">
                <label id="lblTotalGCash"> - </label>
            </td>
        </tr>*@
                        <tr>
                            <td colspan="2">
                                <table class="w-100 font-weight-bold">
                                    <tbody id="tblOtherPaymentBody"></tbody>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align:left; font-weight:bold" colspan="2">
                                <hr />
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align:left; font-weight:bold">
                                Total Inventory of Payments
                            </td>
                            <td style="text-align:right">
                                <label id="lblTotalInventoryofPayments" style="font-weight:bolder"> - </label>
                            </td>
                        </tr>
                        <tr class="divSRVLiability" style="display:none">
                            <td style="text-align:left;">
                                Adjustments (SRV Liability):
                            </td>
                            <td style="text-align:right">
                                <label id="lblSRVLiability"> - </label>
                            </td>
                        </tr>

                        <tr class="divSRVLiability" style="display:none">
                            <td style="text-align:left; font-weight:bold" colspan="2">
                                <hr />
                            </td>
                        </tr>
                        <tr class="divSRVLiability" style="display:none">
                            <td style="text-align:left; font-weight:bold;">
                                Current Standing:
                            </td>
                            <td style="text-align: right; font-weight: bold; font-size: xx-large;">
                                <label id="lblExcessofInventoryofPaymentsvsNetSales"> - </label>
                            </td>
                        </tr>
                        @*<tr>
            <td colspan="2" style="text-align:center">
                <br />
                <label style="font-weight:bold" id="InventoryofPaymentsAudit"> Inventory of Payments Audit </label>
            </td>
        </tr>
        <tr>
            <td style="text-align:left; font-weight:bold">
                <br />
                Net Sales Adjusted for Exchanges
            </td>
            <td style="text-align:right; font-weight:bold">
                <br />
                <label id="lblNetSalesAdjustedforExchanges"> - </label>
            </td>
        </tr>

        <tr>
            <td style="text-align:left; font-weight:bold">
                Total Inventory of Payments
            </td>
            <td style="text-align:right; font-weight:bold">
                <label id="lblTotalInventoryofPayments2"> - </label>
            </td>
        </tr>

        <tr>
            <td style="text-align:left; font-weight:bold" colspan="2">
                <hr />
            </td>
        </tr>
        <tr>
            <td style="text-align:left; font-weight:bold">
                <label id="lblOverageLabel"> - </label>
            </td>
            <td style="text-align:right; font-weight:bold">
                <label id="lblOverage"> - </label>
            </td>
        </tr>*@

                        @*<tr>
            <td colspan="2" style="text-align:center">
                <br />
                <label style="font-weight:bold"> Turnovers and Deposits </label>
            </td>
        </tr>

        <tr>
            <td colspan="2" style="text-align:center">
                <br />
                <table width="100%">
                    <thead>
                        <tr>
                            <th style="text-align:left">Action</th>
                            <th style="text-align:left">To</th>
                            <th style="text-align:right">Amount</th>
                        </tr>
                    </thead>
                    <tbody id="tBodyTurnOverAndDeposits"></tbody>

                </table>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <hr />
            </td>
        </tr>
        <tr>
            <td style="text-align:left; font-weight:bold">
                Total Turnovers and Deposits
            </td>
            <td style="text-align:right; font-weight:bold">
                <label id="lblTotalTotalTurnoversAndDeposits" style="font-weight:bold"> - </label>
            </td>
        </tr>*@

                        <tr>
                            <td colspan="2" style="text-align:center">
                                <br />
                                <label style="font-weight:bold"> Sales Grouped by VAT Type </label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <table width="100%">
                                    <tr>
                                        <td style="text-align:left">
                                            <br />
                                            VAT-able Sales
                                        </td>
                                        <td style="text-align:right">
                                            <br />
                                            <label id="lblVATableSales"> - </label>
                                        </td>
                                        <td style="text-align:right">
                                            <br />
                                            <label id="lblVATableSalesPercent"> - </label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align:left">
                                            VAT-Amount
                                        </td>
                                        <td style="text-align:right">
                                            <label id="lblVATAmount"> - </label>
                                        </td>
                                        <td style="text-align:right">
                                            <label id="lblVATAmountPercent"> - </label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align:left">
                                            Non-VAT
                                        </td>
                                        <td style="text-align:right">
                                            <label id="lblNonVAT"> - </label>
                                        </td>
                                        <td style="text-align:right">
                                            <label id="lblNonVATPercent"> - </label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align:left">
                                            VAT-Exempt
                                        </td>
                                        <td style="text-align:right">
                                            <label id="lblVATExempt"> - </label>
                                        </td>
                                        <td style="text-align:right">
                                            <label id="lblVATExemptPercent"> - </label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align:left">
                                            Zero-Rated VAT
                                        </td>
                                        <td style="text-align:right">
                                            <label id="lblZeroRatedVAT"> - </label>
                                        </td>
                                        <td style="text-align:right">
                                            <label id="lblZeroRatedVATPercent"> - </label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3">
                                            <hr />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td style="text-align:right">
                                            <label id="lblTotalAllVat"> </label>
                                        </td>
                                        <td style="text-align:right">
                                            <label id="lblTotalAllVatPercent"> </label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="text-align:center">
                                <br />
                                <label style="font-weight:bold"> Other Information </label>
                            </td>
                        </tr>
                        @*<tr>
                            <td style="text-align:left;">
                                Voided Qty:
                            </td>
                            <td style="text-align:right;">
                                <label id="lblVoidedItemQty"> - </label>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align:left;">
                                Total Void Amount:
                            </td>
                            <td style="text-align:right;">
                                <label id="lblTotalVoidAmount"> - </label>
                            </td>
                        </tr>*@
                        <tr>
                            <td style="text-align:left; font-weight:bold">
                                Date and Time :
                            </td>
                            <td style="text-align:right; font-weight:bold">
                                <label id="lblDateandTime"> - </label>
                            </td>
                        </tr>
                    </table>
                </div>

                <div id="dvManagerSalesReportExcessSection" style="display:none">
                    <table style="width:100%;">
                        <tr>
                            <td colspan="3" style="text-align:center">
                                <br />
                                <label style="font-weight:bold"> Sales Grouped by Cashier </label>
                            </td>
                        </tr>
                        <tbody id="tbGroupByCashier">
                            <br />
                        </tbody>

                        <tr>
                            <td colspan="3" style="text-align:center">
                                <br />
                                <label style="font-weight:bold"> Sales Grouped by Machine </label>
                            </td>
                        </tr>

                        <tbody id="tbGroupByMachine">
                            <br />
                        </tbody>

                        <tr>
                            <td colspan="3" style="text-align:center">
                                <br />
                                <label style="font-weight:bold"> Sales Grouped by Cashier & Machine </label>
                            </td>
                        </tr>

                        <tbody id="tbGroupByCashierAndMachine">
                            <br />
                        </tbody>

                        <tr>
                            <td colspan="3" style="text-align:center">
                                <br />
                                <label style="font-weight:bold"> Other Information </label>
                            </td>
                        </tr>

                        <tr>
                            <td colspan="2">
                                <label style="font-weight:bold"> Total Trans. Count </label>
                            </td>
                            <td id="lblTotalTransCount" class="text-right"></td>
                        </tr>

                        <tr>
                            <td colspan="2">
                                <label style="font-weight:bold"> Sales Trans. Count </label>
                            </td>
                            <td id="lblSalesTransCount" class="text-right"></td>
                        </tr>

                        <tr>
                            <td colspan="2">
                                <label style="font-weight:bold"> Sales Line Item Count </label>
                            </td>
                            <td id="lblSalesLineItemCount" class="text-right"></td>
                        </tr>

                        <tr>
                            <td colspan="2">
                                <label style="font-weight:bold"> SRV Trans. Count </label>
                            </td>
                            <td id="lblSRVTransCount" class="text-right"></td>
                        </tr>

                        <tr>
                            <td colspan="2">
                                <label style="font-weight:bold"> SRV Line Item Count </label>
                            </td>
                            <td id="lblSRVLineItemCount" class="text-right"></td>
                        </tr>

                        <tr>
                            <td colspan="2">
                                <label style="font-weight:bold"> Refund Trans. Count </label>
                            </td>
                            <td id="lblRefundTransCount" class="text-right"></td>
                        </tr>

                        <tr>
                            <td colspan="2">
                                <label style="font-weight:bold"> Refund Line Item Count </label>
                            </td>
                            <td id="lblRefundLineItemCount" class="text-right"></td>
                        </tr>

                        <tr>
                            <td colspan="2">
                                <label style="font-weight:bold"> Total Customer/Guest Count </label>
                            </td>
                            <td id="lblCustomerCount" class="text-right"></td>
                        </tr>

                        <tr>
                            <td colspan="2">
                                <br />
                                <label style="font-weight:bold"> Printed by </label>
                            </td>
                            <td id="lblPrintedby" class="text-right"></td>
                        </tr>

                        <tr>
                            <td colspan="2">
                                <br />
                                <label style="font-weight:bold"> Date and Time </label>
                            </td>
                            <td id="lblDateandTime2" class="text-right"></td>
                        </tr>

                    </table>
                </div>
            </div>
        </div>

        <div id="dvSalesReportOfflineMessage" style="visibility:hidden; display:none">
            <h4>Oops, we cannot connect to the Mobiz Cloud.</h4>
            <br />
            <p>We can only show your device's Cashier's Sales Report since we cannot retrieve other information from the cloud.</p>
        </div>

        <div style="margin-top:1.5rem; text-align:center;">
            <button data-role="none" id="btnPOSSalesReportCancel" class="btn custom-btn-theme-c btn-less-width btn-secondary btn-lg d-none" style="margin-right:5px;">Go Back To Main</button>
            <button data-role="none" id="btnPOSSalesReportPrint" class="btn custom-btn-theme-b btn-less-width btn-success btn-lg" style="margin-right:5px;">Print</button>
            <span style="text-align:center; visibility:hidden; display:none" id="dvDepositMonies"><button data-role="none" id="btnDepositMonies" class="btn custom-btn-theme-b btn-less-width btn-dark btn-lg" style="margin-right:5px;">Deposit/Turn-over Monies</button></span>
            <span style="text-align:center; visibility:hidden; display:none" id="dvPOSSalesReportPrintAyalaConsolidatedReport"><button data-role="none" id="btnDepositMonies" class="btn custom-btn-theme-b btn-less-width btn-warning btn-lg" style="margin-right:5px;">Print Ayala Consolidated Report</button></span>
        </div>

        <div data-role="popup" id="popupMenu">
            <div data-role="popup" id="dvpopup" data-theme="a" class="ui-corner-all" data-dismissible="false" data-overlay-theme="a">
                <p id="popMessage" style="padding:10px 20px;text-align:center"></p>
            </div>
        </div>


        @*<div data-role="page" id="panDepositMonies" data-overlay-theme="a" data-theme="a" data-corners="false">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Deposit Monies</h3></center>
            <div data-role="content" class="ui-content ui-body-d" style="width:500px; height:500px;">

                <fieldset data-role="controlgroup" data-type="horizontal">

                    <legend>Deposit/Turn-over To: </legend>
                    <select id="optDepositTo">
                        <option>Others</option>
                        <option>Bank</option>
                        <option>Employee</option>
                    </select>
                    <input type="text" id="txtDepositTo" />

                </fieldset>
                    <div data-role="fieldcontain">

                        <label for="txtDepositBy"> Deposit/Turn-over By: </label>
                        <input type="text" id="txtDepositBy" readonly />

                    </div>

                    <div data-role="fieldcontain">

                        <label for="txtTotalAmount"> Total Amount to Deposit/Turn-over: </label>
                        <input type="text" id="txtTotalAmount" />

                    </div>

                    <div style="text-align:center"> <a id="btnSaveDepositMonies" data-role="button">Save</a> </div>

            </div>
        </div>*@
    </div>


    <div data-role="page" id="panDepositMonies" data-overlay-theme="a" data-theme="a" data-corners="false">
        @*<a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>*@
        @*<center><h3>Deposit Monies</h3></center>*@
        <div data-role="content" class="ui-content ui-body-d">

        </div>
    </div>
</div>

