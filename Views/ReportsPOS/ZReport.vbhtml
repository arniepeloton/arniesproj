@Code
    ViewData("Title") = "Z-Report"
    If Request.IsAjaxRequest() Then
        Layout = ""
    Else
        Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
    End If
End Code
@*
@Section head
*@
    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=', (IsMobile ? '' : '/Scripts/dom-to-image.min.js')], function () {
                require([GetPathFormat('/Scripts/sources/ReportsPOS/ZReport.js?v=' + assetversion)
                ]);
            })
        })        
    </script>
    @*
End Section
*@
<div>
    <nav class="navbar navbar-dark bg-primary sticky-sm-top text-center mb-3" style="font-size:large; color:#FFF; display:block;">
        @ViewData("Title")
    </nav>

    <div class="container">
        @*
        @If Common.WebRequest.QueryString("IsMobile") <> "1" Then
            @<applet name="jzebra" code="jzebra.PrintApplet.class" archive="/Scripts/jzebra.jar" width="10" height="10">
            </applet>
        End If
        *@
        <div class="row m-0 gy-2 gx-3 mb-3 align-items-center">
            <div class="col-md-auto">
                <div class="row">
                    <label for="btnZReportSelectFowst" class="col-auto col-md-auto col-form-label">Store:</label>
                    <div class="col col-md-auto">
                        <button style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block btnZReportSelectFowst">
                            <span class="lblZReportSelectedFowstGroup"> Please select </span>
                        </button>
                    </div>
                </div>
            </div>
            <div class="col-md-auto d-none">
                <div class="row">
                    <label for="selZReadMachine" class="col-auto col-md-auto col-form-label">Machine:</label>
                    <div class="col col-md-auto">
                        <select data-role="none" class="form-select" id="selZReadMachine">
                            <option>(Select a store first)</option>
                        </select>
                    </div>
                </div>
            </div>
            <div class="col-md-auto">
                <div class="row">
                    <label for="btnSystemWideDate" class="col-auto col-md-auto col-form-label">Date:</label>
                    <div class="col col-md-auto">
                        <button id="btnZReportDateRange" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                            <span id="ZReportDateRangeFrom">Today</span>
                            <span id="ZReportDateRangeSeparator" style="display:none;">to</span>
                            <span id="ZReportDateRangeTo"></span>
                        </button>
                    </div>
                </div>
            </div>
        </div>

        <div data-role="popup" id="panZReportFowst" data-overlay-theme="a" data-theme="a" data-corners="false">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Select a Fowst</h3></center>
            <div data-role="content" class="ui-content ui-body-d">
                <div class="FowstGroupPopOp" style="padding:10px;">
                    <ul id="listZReportFowst" data-role="listview" class="noBorderList">
                        <li><a class="btn custom-btn-theme-c txt-left">Please wait</a></li>
                    </ul>
                </div>
            </div>
        </div>

        <div data-role="popup" id="panZReportDate" data-overlay-theme="a" data-theme="a" data-corners="false">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Period</h3></center>
            <div data-role="content" class="ui-content ui-body-d">
                <div class="DatesPopUp" style="padding:10px;">
                    <ul data-role="listview" class="noBorderList">
                        <li id="liZReportYesterday"><a href="#" class="btn custom-btn-theme-c txt-left" style="box-shadow:none !important; font-weight:bold !important; width:auto;">Yesterday</a></li>
                        <li id="liZReportToday"><a href="#" class="btn custom-btn-theme-c txt-left" style="box-shadow:none !important; font-weight:bold !important; width:auto;">Today</a></li>
                        <li id="liZReportCustomPeriod"><a href="#" class="btn custom-btn-theme-c txt-left" style="box-shadow:none !important; font-weight:bold !important; width:auto;">Custom Period</a></li>
                    </ul>
                </div>
            </div>
        </div>

        <div data-role="popup" id="panZreportCustomDate" data-overlay-theme="a" data-theme="a" data-corners="false">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Date Range</h3></center>
            <div data-role="content" class="ui-content ui-body-d">
                <span id="spanZreportDateRangeError" style="display:none; color:#F00;">Please select a valid date range. <br /></span>
                <div class="ui-grid-a ui-responsive">
                    <div class="ui-block-a" style="padding:7px;">
                        <div class="metro" style="max-width:255px; overflow:auto;">
                            From: <span id="txtZreportDateFrom"></span>
                            <div class="ZreportfromCalendar calendar"></div>
                        </div>
                    </div>
                    <div class="ui-block-b" style="padding:7px;">
                        <div class="metro" style="max-width:255px; overflow:auto;">
                            To: <span id="txtZreportDateTo"></span>
                            <div class="ZreporttoCalendar calendar"></div>
                        </div>
                    </div>
                </div>
                <br />
                <div class="ui-grid-a ui-responsive">
                    <div class="ui-block-a" style="padding:0px 7px 7px 7px;">
                        <a id="btnZreportCancel" class="btn custom-btn-theme-c">
                            Cancel
                        </a>
                    </div>
                    <div class="ui-block-b" style="padding:0px 7px 7px 7px;">
                        <a id="btnZreportDone" class="btn custom-btn-theme-g">
                            Done
                        </a>
                    </div>
                </div>
            </div>
        </div>

        <div data-role="fieldcontain" style="visibility:hidden; display:none">
            <label for="selZReportRange" class="select">Show report range</label>
            <select id="selZReportRange" data-role="none" class="form-select">
                <option value="">Please Select</option>
                <option value="Today" selected>Today</option>
                <option value="Yesterday">Yesterday</option>
                <option value="By Date">By Date</option>
            </select>
        </div>

        <div id="divZReportByDate" style="display:none;">
            <div data-role="fieldcontain">
                <label>Date: </label>
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <select id="selZReportByDateMonth">
                        <option value="">MMM</option>
                    </select>
                    <select id="selZReportByDateDay">
                        <option value="">DD</option>
                    </select>
                    <select id="selZReportByDateYear">
                        <option value="">YYYY</option>
                    </select>
                </fieldset>
            </div>
        </div>

        <div class="alert alert-danger" id="divLastZReadInfo" style="display:none;"></div>

        <div id="divZReportNoData" style="display:none; text-align:center;"><h1>No report available</h1></div>

        <div id="divZReportView" class="row justify-content-center p-3" style="display:none;">
            <div style="border:1px solid #ccc; padding:15px;" class="col-md-8 col-lg-5 rounded-1 bg-white">
                <div data-role="fieldcontain" style="text-align:center;">
                    <span id="lblTradeName" style="font-size: xx-large; font-weight: bolder">Trade Name Goes Here</span>
                </div>
                <div data-role="fieldcontain" style="text-align:center;">
                    <span id="lblCorporateName" style="font-size:medium;">Corporate Name Goes Here</span>
                </div>
                <div data-role="fieldcontain" style="text-align:center;">
                    <span id="lblBranchName" style="font-size:medium;">Branch Name Goes Here</span>
                </div>
                <div data-role="fieldcontain" style="text-align:center;">
                    <span id="lblAddress" style="font-size:medium;">Address Goes Here</span>
                </div>
                <div data-role="fieldcontain" style="text-align:center;">
                    <span id="lblTIN" style="font-size:medium;">TIN Goes Here</span>
                </div>
                @*<div data-role="fieldcontain" style="text-align:center;">
            <span id="lblTelNo" style="font-size:medium;">Tel. No. Goes Here</span>
        </div>*@
                <div data-role="fieldcontain" style="text-align:center;">
                    <span id="lblMachineSerialNo" style="font-size:medium;">Serial No. Goes Here</span>
                </div>
                <div data-role="fieldcontain" style="text-align:center;">
                    <span id="lblPTUNo" style="font-size:medium;">Serial No. Goes Here</span>
                </div>
                <div data-role="fieldcontain" style="text-align:center;">
                    <span id="lblMachineIDNo" style="font-size:medium;">Serial No. Goes Here</span>
                </div>

                <br />
                <div data-role="fieldcontain" style="text-align:center;">
                    <span id="lblZReport" style="font-size: xx-large; font-weight: bolder">Z-Read</span>
                </div>
                <div data-role="fieldcontain" style="text-align:center;">
                    <span id="lblZDate" style="font-size:medium;">Date Goes Here</span>
                </div>

                <div data-role="fieldcontain" style="text-align:center; display:none;">
                    <span id="lblIsOffline" style="font-size:medium;"></span>
                </div>
                <br />

                <div class="ui-body">
                    <div data-role="fieldcontain" style="font-weight: bold">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>Gross Sales:</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadGrossSales" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                    <hr />
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>Sales Discount:</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadSalesDiscount" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                        <table id="tblDiscountsBreakdown" style="width:250px; font-style:italic;"></table>
                    </div>
                    <hr />
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>Vat Exempt Tax:</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadVatExemptTAX" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                    <hr />
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:center; font-weight:bold;">
                                <span style="font-size:larger;">Summary of Adjustments</span>
                            </div>
                            @*<div class="col">
                </div>*@
                        </div>
                    </div>
                    <br />
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>Total Sales Refund for the  Period</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadTotalSalesRefund" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>Total Sales Returns Subject to Merchadise Exchange (SRV Issued)</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadTotalSRV" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                    <br />
                    <div data-role="fieldcontain">
                        <div class="row" style="font-weight: bold">
                            <div class="col" style="text-align:left;">
                                <span>Net of Adjustments:</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadNetOfAdjustment" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                    <hr />
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>Add Charges:</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadCharges" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                    <hr>
                    <div data-role="fieldcontain">
                        <div class="row align-items-center" style="font-weight: bold">
                            <div class="col" style="text-align:left;">
                                <b>Net Sales:</b>
                            </div>
                            <div class="col">
                                <b id="lblZReadNetSales" style="float: right; text-align: right; font-size:xx-large">0.00</b>
                            </div>
                        </div>
                    </div>
                    @*<div data-role="fieldcontain">
            <div class="row">
                <div class="col" style="text-align:left;">
                    <span>Used Up SRV:</span>
                </div>
                <div class="col">
                    <span id="lblZReadIssuedSRV" style="float: right; text-align: right;">0.00</span>
                </div>
            </div>
        </div>
        <hr />
        <div data-role="fieldcontain">
            <div class="row">
                <div class="col" style="text-align:left;">
                    <b>Net Sales Adjusted for Exchanges:</b>
                </div>
                <div class="col">
                    <b id="lblZReadNetSalesAdjustedForExchanges" style="float: right; text-align: right;">0.00</b>
                </div>
            </div>
        </div>*@
                </div>

                <br />
                <div data-role="fieldcontain" style="text-align:center; font-weight:bold;">
                    <span id="lblZReadInventoryOfPayments" style="font-size:larger;">Inventory of Payments</span>
                </div>
                <br />
                <div class="ui-body">
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>Cash</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadCash" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>Check</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadCheck" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>Debit Card</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadDebitCard" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>Credit Card</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadCreditCard" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>SRV</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadSRV" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>On Credit</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadOnCredit" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                    <div data-role="fieldcontain" id="divOtherType" style="display:none;">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <table id="tblOtherPaymentType" style="width:100%">
                                    <tbody></tbody>
                                </table>
                            </div>
                            @*<div class="col">
                        <span id="lblZReadOtherPaymentType" style="float: right; text-align: right;">0.00</span>
                    </div>*@
                        </div>
                    </div>
                    @*<div data-role="fieldcontain" id="divOtherType" style="display:none;">
                <div class="row">
                    <div class="col" style="text-align:left;">
                        <span>Total Other Tender</span>
                    </div>
                    <div class="col">
                        <span id="lblZReadOtherPaymentType" style="float: right; text-align: right;">0.00</span>
                    </div>
                </div>
            </div>
            <div id="divOtherTypeDetailed" style="display:none;">
                <div data-role="fieldcontain">
                    <div class="row">
                        <div class="col" style="text-align:left;">
                            <span>SMAC</span>
                        </div>
                        <div class="col">
                            <span id="lblZReadSMAC" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                <div data-role="fieldcontain">
                    <div class="row">
                        <div class="col" style="text-align:left;">
                            <span>E-Plus</span>
                        </div>
                        <div class="col">
                            <span id="lblZReadEPlus" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                <div data-role="fieldcontain">
                    <div class="row">
                        <div class="col" style="text-align:left;">
                            <span>Voucher</span>
                        </div>
                        <div class="col">
                            <span id="lblZReadVoucher" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>*@
                </div>

                <hr>
                <div data-role="fieldcontain">
                    <div class="row align-items-center">
                        <div class="col" style="text-align:left; font-weight:bold;">
                            Total Payments:
                        </div>
                        <div class="col">
                            <span id="lblZReadTotalInvPayment" style="float: right; text-align: right; font-weight:bold;">0.00</span>
                        </div>
                    </div>
                </div>

                <div id="divSRVLiability" style="display:none;">
                    <div data-role="fieldcontain">
                        <div class="row align-items-center">
                            <div class="col" style="text-align:left;">
                                Adjustments (SRV Liability):
                            </div>
                            <div class="col">
                                <span id="lblZReadSRVLiability" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>


                    <hr>
                    <div data-role="fieldcontain">
                        <div class="row align-items-center">
                            <div class="col" style="text-align:left; font-weight:bold;">
                                Current Standing:
                            </div>
                            <div class="col">
                                <span id="lblZReadCurrentStanding" style="float: right; text-align: right; font-weight:bold; font-size:xx-large">0.00</span>
                            </div>
                        </div>
                    </div>
                </div>                

                <br />
                <div data-role="fieldcontain" style="text-align:center;">
                    <span id="lblZReadSalesGroupbyVATType" style="font-size:larger; font-weight:bold;">Sales Grouped by VAT Type</span>
                </div>
                <br />
                <div class="ui-body">
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>VAT-able Sales</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadVATableSalesPercent" style="float: right; text-align: right;">0.00%</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadVATableSales" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>VAT Amount</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadVATAmountPercent" style="float: right; text-align: right;">0.00%</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadVATAmount" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>Non-VAT</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadNonVATPercent" style="float: right; text-align: right;">0.00%</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadNonVATAmount" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>VAT-Exempt</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadVATExemptPercent" style="float: right; text-align: right;">0.00%</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadVATExemptAmount" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>Zero-Rated VAT</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadZeroRatedVATPercent" style="float: right; text-align: right;">0.00%</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadZeroRatedVATAmount" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                    <hr>
                    <div data-role="fieldcontain">
                        <div class="row align-items-center">
                            <div class="col" style="text-align:left; font-weight:bold;">
                            </div>
                            <div class="col">
                                <span id="lblZReadTotalVATTypePercent" style="float: right; text-align: right; font-weight:bold;">0.00%</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadTotalVATType" style="float: right; text-align: right; font-weight:bold;">0.00</span>
                            </div>
                        </div>
                    </div>
                </div>

                <br />
                <div data-role="fieldcontain" style="text-align:center;">
                    <span id="lblZReadOtherInfo" style="font-size:larger; font-weight:bold;">Other Information</span>
                </div>
                <br />
                <div class="ui-body">
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>Total Trans. Count</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadTotalTransCount" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>Sales Trans. Count</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadSalesTransCount" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>Sales Line Item Count</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadSalesLineItemCount" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>SRV Trans. Count</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadSRVTransCount" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>SRV Line Item Count</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadSRVLineItemCount" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>Refund Trans. Count</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadRefundTransCount" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>Refund Line Item Count</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadRefundLineItemCount" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>Customer Count</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadCustomerCount" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>Prev. Day EOD Invoice Count</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadPrevInvoiceCount" style="float: right; text-align: right;">0</span>
                            </div>
                        </div>
                    </div>
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>Curr Day Starting Invoice Count</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadStartingInvoiceCount" style="float: right; text-align: right;">0</span>
                            </div>
                        </div>
                    </div>
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>Curr Day EOD Invoice Count</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadEndingInvoiceCount" style="float: right; text-align: right;">0</span>
                            </div>
                        </div>
                    </div>
                    @*<div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>Voided Qty</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadVoidedItemQty" style="float: right; text-align: right;">0</span>
                            </div>
                        </div>
                    </div>
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>Total Void Amount</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadTotalVoidAmount" style="float: right; text-align: right;">0</span>
                            </div>
                        </div>
                    </div>*@
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>Previous Reading:</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadOldGrandTotal" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>Present Reading:</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadNewGrandTotal" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>

                    <div data-role="fieldcontain">
                        <div class="row">
                            <div class="col" style="text-align:left;">
                                <span>Z Counter</span>
                            </div>
                            <div class="col">
                                <span id="lblZReadZReadPrintCount" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>        

        <div class="container" id="divZReadPrintButton" style="margin-top:1.5rem; text-align:center;">
            <div class="row justify-content-center">
                <div class="col-sm col-md-auto mb-3 order-md-2">
                    @*<button data-role="none" id="btnPOSSalesZReportCancel" class="btn custom-btn-theme-c btn-less-width btn-secondary btn-lg" style="margin-right:5px;">Go Back To Main</button>*@
                    <span id="divPOSSalesZReportPrint" class="mb-3" style="text-align: center; display:nonex;">
                        <button data-role="none" id="btnPOSSalesZReportPrint" class="btn custom-btn-theme-b btn-less-width btn-success btn-lg btn-block" style="margin-right:5px;">Print</button>
                    </span>
                </div>
                <div class="col-sm col-md-auto mb-3 order-md-1">
                    <button data-role="none" id="btnPOSSalesZReportView" class="btn custom-btn-theme-b btn-less-width btn-light border btn-lg btn-block" style="margin-right:5px;">Refresh</button>
                </div>
                <div class="col-sm col-md-auto mb-3 order-md-1">
                    <button data-role="none" id="btnPOSSalesZReportBack" class="btn btn-secondary btn-lg btn-block" style="margin-right:5px;">Back</button>
                </div>
                <div class="col-sm col-md-auto mb-3 order-md-0" id="divPOSSalesZReportEOD" style="text-align: center; display:none;">
                    <span>
                        <button data-role="none" id="btnPOSSalesZReportEOD" class="btn custom-btn-theme-b btn-less-width btn-warning btn-lg btn-block" style="margin-right:5px;">
                            EOD (Conclude the transaction for today)
                        </button>
                    </span>
                </div>
            </div>
            
            
	        @*<div style="text-align:center"> <a id="btnPOSSalesZReportPrintTemp" data-role="button">Print Temp</a> </div>*@
            
            @*<div id="divPOSSalesEJournal" style="text-align: center;">
                <a id="btnPOSSalesEJournal" data-role="button">
                    <h3>Download E-Journal</h3>                    
                </a>
            </div>*@
        </div>

        <div data-role="popup" id="popupMenu" >
            <div data-role="popup" id="dvpopup" data-theme="a" class="ui-corner-all" data-dismissible="false" data-overlay-theme="a">
                <p id="popMessage" style="padding:10px 20px;text-align:center"></p>
            </div>
        </div>

    </div>