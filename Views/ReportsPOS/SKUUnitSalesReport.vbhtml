@*@Code
    ViewData("Title") = "Z-Report"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
*@
    <script>
        require([GetPathFormat('/Scripts/sources/ReportsPOS/SKUUnitSalesReport.js?crc=' + assetversion)
            ]);
    </script>
    @*
End Section
*@
<div id="pageAyalaReport">
   @* <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>*@

    <div data-role="content">
@*
        @If Common.WebRequest.QueryString("IsMobile") <> "1" Then
            @<applet name="jzebra" code="jzebra.PrintApplet.class" archive="/Scripts/jzebra.jar" width="10" height="10">
            </applet>
        End If
*@
        <div class="row">
            <div class="col-auto mb-3">
                FOWST: <br />
                <button style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block btnZReportSelectFowst">
                    <span class="lblZReportSelectedFowstGroup"> FOWST Location </span>
                </button>
            </div>
            <div class="col-auto mb-3">
                Date: <br />
                <button id="btnZReportDateRange" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                    <span id="ZReportDateRangeFrom">Today</span>
                    <span id="ZReportDateRangeSeparator" style="display:none;">to</span>
                    <span id="ZReportDateRangeTo"></span>
                </button>
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
            <select id="selZReportRange">
	            <option value="">Please Select</option>
				<option value="Today">Today</option>
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

        <div id="divZReportView" style="display:none;">
            <br /><br />
            <div data-role="fieldcontain" style="text-align:center;">
	            <span id="lblTradeName" style="font-size:large;">Trade Name Goes Here</span>
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
            <div data-role="fieldcontain" style="text-align:center;">
	            <span id="lblTelNo" style="font-size:medium;">Tel. No. Goes Here</span>
            </div>

            <br />
            <div data-role="fieldcontain" style="text-align:center;">
	            <span id="lblZReport" style="font-size:large;">Z-Read</span>
            </div>
            <div data-role="fieldcontain" style="text-align:center;">
	            <span id="lblZDate" style="font-size:medium;">Date Goes Here</span>
            </div>
            <br />

            <div class="ui-body">
                <div data-role="fieldcontain">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>Gross Sales:</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblGrossSales" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                <div data-role="fieldcontain">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>Sales Discount:</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblSalesDiscount" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                <div data-role="fieldcontain">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>Vat Exempt Tax:</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblVatExemptTAX" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                 <div data-role="fieldcontain">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>Charges:</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblCharges" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                <div data-role="fieldcontain">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>Summary of Adjustments</span>
                        </div>
                        <div class="ui-block-b">
                        </div>
                    </div>
                </div>
                <div data-role="fieldcontain">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>Total Sales Refund for the  Period</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblTotalSalesRefund" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                <div data-role="fieldcontain">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>Total Sales Returns Subject to Merchadise Exchange (SRV Issued)</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblTotalSRV" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                <div data-role="fieldcontain">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>Net of Adjustments:</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblNetOfAdjustment" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                <hr noshade size=1 width="100%">
                <div data-role="fieldcontain">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="text-align:left; font-weight:bold;">
                            <span>Net Sales:</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblNetSales" style="float: right; text-align: right; font-weight:bold;">0.00</span>
                        </div>
                    </div>
                </div>
            </div>
            
            <br />
            <div data-role="fieldcontain" style="text-align:center;">
	            <span id="lblInventoryOfPayments" style="font-size:larger;">Inventory of Payments</span>
            </div>
            <br />
            <div class="ui-body">
                <div data-role="fieldcontain">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>Cash</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblCash" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                <div data-role="fieldcontain">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>Check</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblCheck" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                <div data-role="fieldcontain">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>Debit Card</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblDebitCard" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                <div data-role="fieldcontain">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>Credit Card</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblCreditCard" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                <div data-role="fieldcontain">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>SRV</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblSRV" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                <div data-role="fieldcontain">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>On Credit</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblOnCredit" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                <div data-role="fieldcontain" id="divOtherType" style="display:none;">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>Other Tender</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblOtherPaymentType" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                <div id="divOtherTypeDetailed" style="display:none;">
                    <div data-role="fieldcontain">
                        <div class="ui-grid-a">
                            <div class="ui-block-a" style="text-align:left;">
                                <span>SMAC</span>
                            </div>
                            <div class="ui-block-b">
                                <span id="lblSMAC" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                    <div data-role="fieldcontain">
                        <div class="ui-grid-a">
                            <div class="ui-block-a" style="text-align:left;">
                                <span>E-Plus</span>
                            </div>
                            <div class="ui-block-b">
                                <span id="lblEPlus" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                    <div data-role="fieldcontain">
                        <div class="ui-grid-a">
                            <div class="ui-block-a" style="text-align:left;">
                                <span>Voucher</span>
                            </div>
                            <div class="ui-block-b">
                                <span id="lblVoucher" style="float: right; text-align: right;">0.00</span>
                            </div>
                        </div>
                    </div>
                </div>
                
                <hr noshade size=1 width="100%">
                <div data-role="fieldcontain">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="text-align:left; font-weight:bold;">
                        </div>
                        <div class="ui-block-b">
                            <span id="lblTotalInvPayment" style="float: right; text-align: right; font-weight:bold;">0.00</span>
                        </div>
                    </div>
                </div>
            </div>

            <br />
            <div data-role="fieldcontain" style="text-align:center;">
	            <span id="lblSalesGroupbyVATType" style="font-size:larger;">Sales Grouped by VAT Type</span>
            </div>
            <br />
            <div class="ui-body">
                <div data-role="fieldcontain">
                    <div class="ui-grid-b">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>VAT-able Sales</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblVATableSalesPercent" style="float: right; text-align: right;">0.00%</span>
                        </div>
                        <div class="ui-block-c">
                            <span id="lblVATableSales" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                <div data-role="fieldcontain">
                    <div class="ui-grid-b">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>VAT Amount</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblVATAmountPercent" style="float: right; text-align: right;">0.00%</span>
                        </div>
                        <div class="ui-block-c">
                            <span id="lblVATAmount" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                <div data-role="fieldcontain">
                    <div class="ui-grid-b">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>Non-VAT</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblNonVATPercent" style="float: right; text-align: right;">0.00%</span>
                        </div>
                        <div class="ui-block-c">
                            <span id="lblNonVATAmount" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                <div data-role="fieldcontain">
                    <div class="ui-grid-b">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>VAT-Exempt</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblVATExemptPercent" style="float: right; text-align: right;">0.00%</span>
                        </div>
                        <div class="ui-block-c">
                            <span id="lblVATExemptAmount" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                <div data-role="fieldcontain">
                    <div class="ui-grid-b">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>Zero-Rated VAT</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblZeroRatedVATPercent" style="float: right; text-align: right;">0.00%</span>
                        </div>
                        <div class="ui-block-c">
                            <span id="lblZeroRatedVATAmount" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                <hr noshade size=1 width="100%">
                <div data-role="fieldcontain">
                    <div class="ui-grid-b">
                        <div class="ui-block-a" style="text-align:left; font-weight:bold;">
                        </div>
                        <div class="ui-block-b">
                            <span id="lblTotalVATTypePercent" style="float: right; text-align: right; font-weight:bold;">0.00%</span>
                        </div>
                        <div class="ui-block-c">
                            <span id="lblTotalVATType" style="float: right; text-align: right; font-weight:bold;">0.00</span>
                        </div>
                    </div>
                </div>
            </div>

            <br />
            <div data-role="fieldcontain" style="text-align:center;">
	            <span id="lblOtherInfo" style="font-size:larger;">Other Information</span>
            </div>
            <br />
            <div class="ui-body">
                <div data-role="fieldcontain">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>Total Trans. Count</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblTotalTransCount" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                <div data-role="fieldcontain">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>Sales Trans. Count</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblSalesTransCount" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                <div data-role="fieldcontain">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>Sales Line Item Count</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblSalesLineItemCount" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                <div data-role="fieldcontain">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>SRV Trans. Count</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblSRVTransCount" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                <div data-role="fieldcontain">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>SRV Line Item Count</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblSRVLineItemCount" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                                <div data-role="fieldcontain">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>Refund Trans. Count</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblRefundTransCount" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                <div data-role="fieldcontain">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>Refund Line Item Count</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblRefundLineItemCount" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                <div data-role="fieldcontain">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>Customer Count</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblCustomerCount" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                <div data-role="fieldcontain">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>Old Grand Total</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblOldGrandTotal" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>
                <div data-role="fieldcontain">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>New Grand Total</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblNewGrandTotal" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>

                <div data-role="fieldcontain">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="text-align:left;">
                            <span>Z-Read Print Count</span>
                        </div>
                        <div class="ui-block-b">
                            <span id="lblZReadPrintCount" style="float: right; text-align: right;">0.00</span>
                        </div>
                    </div>
                </div>

            </div>
	
        </div>

        <div style="margin-top:1.5rem; text-align:center;">
            <button data-role="none" id="btnPOSSalesZReportCancel" class="btn custom-btn-theme-c btn-less-width btn-secondary btn-lg" style="margin-right:5px;">Go Back To Main</button>
            <span id="divPOSSalesZReportPrint" style="text-align: center; display:none;">
                <button data-role="none" id="btnPOSSalesZReportPrint" class="btn custom-btn-theme-b btn-less-width btn-success btn-lg" style="margin-right:5px;">Print</button>
            </span>
        </div>

        <div data-role="popup" id="popupMenu" >
            <div data-role="popup" id="dvpopup" data-theme="a" class="ui-corner-all" data-dismissible="false" data-overlay-theme="a">
                <p id="popMessage" style="padding:10px 20px;text-align:center"></p>
            </div>
        </div>

    </div>

</div>

