@Code
    ViewData("Title") = "Sales Report"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/ReportsPOS/SalesReport.js?crc=')
            ]);
    </script>
    
End Section

<div id="pageSalesReport" data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">

   @*     @If Common.WebRequest.QueryString("IsMobile") <> "1" Then
            @<applet name="jzebra" code="jzebra.PrintApplet.class" archive="/Scripts/jzebra.jar" width="10" height="10">
            </applet>
        End If*@

        <div data-role="fieldcontain">
	        <label for="selPOSSalesReportRange" class="select">Show sales report range</label>
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

        </div>

        <div id="divPOSSalesReportRangeByWeek" style="display:none;">

            <div data-role="fieldcontain">
	            <label for="selPOSSalesReportRangeByWeekFrom" class="select">From:</label>
                <select id="selPOSSalesReportRangeByWeekFrom">
	                <option value="">Please Select</option>
                </select>
            </div>

            <div data-role="fieldcontain">
	            <label for="selPOSSalesReportRangeByWeekTo" class="select">To:</label>
                <select id="selPOSSalesReportRangeByWeekTo">
	                <option value="">Please Select</option>
                </select>
            </div>

            <div data-role="fieldcontain">
	            <label for="selPOSSalesReportRangeByWeekYear" class="select">Year:</label>
                <select id="selPOSSalesReportRangeByWeekYear">
	                <option value="">Please Select</option>
	                <option value="This year.">This year.</option>
	                <option value="This year and last year.">This year and last year.</option>
	                <option value="This year and last 2 years.">This year and last 2 years.</option>
	                <option value="This year and last 3 years.">This year and last 3 years.</option>
                </select>
            </div>

        </div>

        <div id="divPOSSalesReportRangeByMonth" style="display:none;">

            <div data-role="fieldcontain">
	            <label for="selPOSSalesReportRangeByMonthFrom" class="select">From:</label>
                <select id="selPOSSalesReportRangeByMonthFrom">
	                <option value="">Please Select</option>
                </select>
            </div>

            <div data-role="fieldcontain">
	            <label for="selPOSSalesReportRangeByMonthTo" class="select">To:</label>
                <select id="selPOSSalesReportRangeByMonthTo">
	                <option value="">Please Select</option>
                </select>
            </div>

            <div data-role="fieldcontain">
	            <label for="selPOSSalesReportRangeByMonthYear" class="select">Year:</label>
                <select id="selPOSSalesReportRangeByMonthYear">
	                <option value="">Please Select</option>
	                <option value="This year.">This year.</option>
	                <option value="This year and last year.">This year and last year.</option>
	                <option value="This year and last 2 years.">This year and last 2 years.</option>
	                <option value="This year and last 3 years.">This year and last 3 years.</option>
                </select>
            </div>

        </div>

        <div id="divPOSSalesReportRangeByQuarter" style="display:none;">

            <div data-role="fieldcontain">
	            <label for="selPOSSalesReportRangeByQtrFrom" class="select">From:</label>
                <select id="selPOSSalesReportRangeByQtrFrom">
	                <option value="">Please Select</option>
	                <option value="1">1st Quarter</option>
	                <option value="2">2nd Quarter</option>
	                <option value="3">3rd Quarter</option>
	                <option value="4">4th Quarter</option>
                </select>
            </div>

            <div data-role="fieldcontain">
	            <label for="selPOSSalesReportRangeByQtrTo" class="select">To:</label>
                <select id="selPOSSalesReportRangeByQtrTo">
	                <option value="">Please Select</option>
	                <option value="1">1st Quarter</option>
	                <option value="2">2nd Quarter</option>
	                <option value="3">3rd Quarter</option>
	                <option value="4">4th Quarter</option>
                </select>
            </div>

            <div data-role="fieldcontain">
	            <label for="selPOSSalesReportRangeByQtrYear" class="select">Year:</label>
                <select id="selPOSSalesReportRangeByQtrYear">
	                <option value="">Please Select</option>
	                <option value="This year.">This year.</option>
	                <option value="This year and last year.">This year and last year.</option>
	                <option value="This year and last 2 years.">This year and last 2 years.</option>
	                <option value="This year and last 3 years.">This year and last 3 years.</option>
                </select>
            </div>
        </div>

        <div id="divPOSSalesReportRangeByYear" style="display:none">

            <div data-role="fieldcontain">
	            <label for="selPOSSalesReportRangeByYear" class="select">Year:</label>
                <select id="selPOSSalesReportRangeByYear">
                </select>
            </div>

        </div>

        <br /> <br /> 

        <div id="divDisplayPOSSalesReport">
            <table id="tblPOSSalesReport" align="center" style="visibility:hidden; display:none">
                <tr>
                    <td colspan="2" style="text-align:center">
                        <label id="lblOrganizationBusinessName" style="font-size:xx-large; font-weight:bolder"></label> <br />
                        <label id="lblOrganizationName"> </label> <br />
                        <label id="lblOrganizationFOWST"> </label> <br />
                        <label id="lblOrganizationFOWSTAddress"> </label>  <br />
                        <label id="lblOrganizationFOWSTTIN"> </label>  <br />
                        <label id="lblOrganizationFOWSTContactNumber"> </label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align:center">
                        <br />
                        <label id= "lblReportType" style="font-size:xx-large; font-weight:bolder">CASHIER'S</label> <br />
                        <label id= "lblReportRange" style="font-weight:bold">SALES REPORT</label> <br />
                        <label id= "lblReportDate" style="font-weight:bold"> Date : 01/01/2014 </label>                        
                    </td>
                </tr>
                
                <tr>
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
                <tr>
                    <td style="text-align:left">
                        Total Sales Discount
                    </td>
                    <td style="text-align:right">
                        <label id="lblTotalSalesDiscount"> - </label>
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
                <tr>
                    <td style="text-align:left; font-weight:bold">
                        Net Sales:
                    </td>
                    <td style="text-align:right; font-weight:bold">
                        <label id="lblTotalNetSales"> - </label>
                    </td>
                </tr>
                <tr>
                    <td style="text-align:left; font-weight:bold">
                        Add : Issued SRV
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
                    <td>                        
                    </td>
                </tr>
                <tr>
                    <td>
                        
                    </td>
                    <td style="text-align:right">                        
                        <label id="lblTotalNetSalesAdjustedforExchanges" style="font-size:xx-large; font-weight:bolder"> </label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align:center">
                        <br /> <br /> <br />
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
                    <td style="text-align:left; font-weight:bold" colspan="2">
                        <hr />
                    </td>                    
                </tr>
                <tr>
                    <td style="text-align:left; font-weight:bold">
                        Total Inventory of Payments
                    </td>
                    <td>                        
                    </td>
                </tr>
                <tr>
                    <td>
                        
                    </td>
                    <td style="text-align:right">                        
                        <label id="lblTotalInventoryofPayments" style="font-size:xx-large; font-weight:bolder"> 1,123,456.78 </label>
                    </td>
                </tr>
                <tr>
                    <td style="text-align:left; font-weight:bold">
                        Excess of Inventory of Payments vs Net Sales :
                    </td>
                    <td style="text-align:right; font-weight:bold">
                        <label id="lblExcessofInventoryofPaymentsvsNetSales"> - </label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align:center">
                        <br /> 
                        <label style="font-weight:bold"> Inventory of Payments Audit </label>
                    </td>
                </tr>
                <tr>
                    <td style="text-align:left; font-weight:bold">
                        <br />
                        Net Sales Adjusted for Exchanges 
                    </td>
                    <td style="text-align:right; font-weight:bold">
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
                </tr>
                <tr>
                    <td colspan="2" style="text-align:center">
                        <br /> 
                        <label style="font-weight:bold"> Sales Grouped by VAT Type </label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <table>
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
                                <td>
                        
                                </td>
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
                    <td style="text-align:left; font-weight:bold">
                        Date and Time :
                    </td>
                    <td style="text-align:right; font-weight:bold">
                        <label id="lblDateandTime"> - </label>
                    </td>
                </tr>
            </table>
        </div>

        <div>
            <br />
            <br />            
            <div style="text-align:center"> <a id="btnPOSSalesReportView" data-role="button"><h3>View</h3></a> </div>
	        <div style="text-align:center"> <a id="btnPOSSalesReportPrint" data-role="button"><h3>Print</h3></a> </div>
	        <div style="text-align:center"> <a id="btnPOSSalesReportCancel" data-role="button" data-rel="back"><h3>Go Back To Main</h3></a></div>
        </div>

        <div data-role="popup" id="popupMenu" >
            <div data-role="popup" id="dvpopup" data-theme="a" class="ui-corner-all" data-dismissible="false" data-overlay-theme="a">
                <p id="popMessage" style="padding:10px 20px;text-align:center"></p>
            </div>
        </div>

    </div>

</div>

