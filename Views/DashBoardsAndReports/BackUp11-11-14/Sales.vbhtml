@Code
    ViewData("Title") = "Dashboards and Reports"
    Layout = "~/Views/Shared/plain.vbhtml"
End Code

@Section head
    <link rel="stylesheet" href="/Content/css/jquery.jqplot.min.css" />
    <link rel="stylesheet" href="/Content/css/ui.dynatree.css" />
    <link rel="stylesheet" href="/Content/css/qunit.css" />
     
    <script>
        require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/DashBoardsAndReports.js?crc=')]);
        require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/crm.js?crc=')]);        
    </script>

    <style>
    
     .jqplot-xaxis-hide {
            display: none
        }
        
    /*Green button*/
    .ui-btn-up-green {
        border: 1px solid #3A7214;
        background: #5BAB25;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, #7EC54E, #41AB25);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #7EC54E),
            color-stop(1, #41AB25));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#7EC54E', EndColorStr='#41AB25')";
    }
    .ui-btn-up-green a.ui-link-inherit {
        color: #fff;
    }
    .ui-btn-hover-green {
        border: 1px solid #176E00;
        background: #6EB64B;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, 
            #99D472, 
            #6EB64B);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #99D472),
            color-stop(1, #6EB64B));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#99D472', EndColorStr='#6EB64B')";
    }
    .ui-btn-hover-green a.ui-link-inherit {
        color: #fff;
    }
    .ui-btn-down-green {
        border: 1px solid #347722;
        background: #7EC54E;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, #569E39, #7EC54E);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #569E39),
            color-stop(1, #7EC54E));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#569E39', EndColorStr='#7EC54E')";
    }
    .ui-btn-down-green a.ui-link-inherit {
        color: #fff;
    }
    .ui-btn-up-green,
    .ui-btn-hover-green,
    .ui-btn-down-green {
        font-family: Helvetica, Arial, sans-serif;
        text-decoration: none;
    }
    
    /*Gray button*/
    .ui-btn-up-gray {
        border: 1px solid #595A57;
        background: #8F908F;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, #AAABAA, #8D8E8D);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #AAABAA),
            color-stop(1, #8D8E8D));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#AAABAA', EndColorStr='#8D8E8D')";
    }
    .ui-btn-up-gray a.ui-link-inherit {
        color: #fff;
    }
    .ui-btn-hover-gray {
        border: 1px solid #646464;
        background: #A1A0A0;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, 
            #B5B8B5, 
            #A1A0A0);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #B5B8B5),
            color-stop(1, #A1A0A0));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#B5B8B5', EndColorStr='#A1A0A0')";
    }
    .ui-btn-hover-gray a.ui-link-inherit {
        color: #fff;
    }
    .ui-btn-down-gray {
        border: 1px solid #777A77;
        background: #AAABAA;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, #999B99, #AAABAA);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #999B99),
            color-stop(1, #AAABAA));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#999B99', EndColorStr='#AAABAA')";
    }
    .ui-btn-down-gray a.ui-link-inherit {
        color: #fff;
    }
    .ui-btn-up-gray,
    .ui-btn-hover-gray,
    .ui-btn-down-gry {
        font-family: Helvetica, Arial, sans-serif;
        text-decoration: none;
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

@*Header*@
   <div data-role="header" data-position="fixed" style="background: none white; border: none">
        <div class="ui-grid-d" >    
            <div class="ui-block-a">
                <div id="btnHome" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                    <span>Home</span> <br />                    
                </div>
            </div>
            <div class="ui-block-b">
                <div id="btnPeriod" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                    <span>Period</span> <br />
                    <span id="lblPeriod"></span>
                </div>
            </div>
            <div class="ui-block-c">
                <div id="btnGraphType" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                    <span>Graph Type</span> <br />
                    <span id="lblGraphType"></span><br />
                    <span id="lblGraphTypeDetail"></span>
                </div>
            </div>
            <div class="ui-block-d">
                <div id="btnFOWST" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                    <span id="btnFOWSTTeamLabel">FOWST</span> <br />
                    <span id="lblFOWST"></span>
                </div>
            </div>            
            <div class="ui-block-e">
                <div id="btnGo"style="margin:0px; height: 80px;" data-role="button" data-theme="green" data-corners="false" class="HotButtons">
                    <span>GO</span>
                </div>
            </div>
       
        </div>
    </div>

    <div data-role="content">
        
        <div data-role="popup" id="panDates" data-theme="b" class="ui-corner-all"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Period</h3></center>
            <div data-role="content">
                <form>
                    <ul data-role="listview" data-inset="true" data-theme="e">
                        <li id="li2DaysAgo"><a href="#">2 Days Ago</a></li>
                        <li id="liYesterday"><a href="#">Yesterday</a></li>
                        <li id="liToday"><a href="#">Today</a></li>
                        <li id="liWeekToDate"><a href="#">Week To Date</a></li>                        
                        <li id="liMonthToDate"><a href="#">Month To Date</a></li>
                        <li id="liCustomPeriod"><a href="">Custom Period</a></li>
                    </ul>
                </form>
            </div>       
        </div>

        <div data-role="popup" id="panCustomPeriod" data-theme="b" class="ui-corner-all"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Period</h3></center>
            <div data-role="content">
                <form>
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <legend><b>From: </b></legend>
                        <fieldset id="txtDateFrom" data-role="DateCombo" data-usecurrent="1"> </fieldset>
                    </fieldset>
            
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <legend><b>To: </b></legend>
                        <fieldset id="txtDateTo" data-role="DateCombo" data-usecurrent="1"> </fieldset>
                    </fieldset>

                    <a id="btnDoneCustomerPeriod" data-role="button" href="#" data-theme="e">Done</a>
                </form>
            </div>       
        </div>

        <div data-role="popup" id="panSalesQuotaRange" data-theme="b" class="ui-corner-all"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Period</h3></center>
            <div data-role="content">
                <form>
                    <ul data-role="listview" data-inset="true" data-theme="e">
                        <li id="liMonth"><a href="#">Month</a></li>
                        <li id="liQuarter"><a href="#">Quarter</a></li>
                        <li id="liYearToDate"><a href="#">YearToDate</a></li>
                    </ul>
                </form>
            </div>       
        </div>

        <div data-role="popup" id="panSalesQuotaMonthPeriod" data-theme="b" class="ui-corner-all"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Period</h3></center>
            <div data-role="content">
                <form>
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <legend><b>Month: </b></legend>
                        <select id="selSalesQuotaMonth">                            
                        </select>
                    </fieldset>                    
                    <a id="btnDoneSalesQuotaMonthPeriod" data-role="button" href="#" data-theme="e">Done</a>
                </form>
            </div>       
        </div>

        <div data-role="popup" id="panSalesQuotaQuarterPeriod" data-theme="b" class="ui-corner-all"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Period</h3></center>
            <div data-role="content">
                <form>
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <legend><b>Month: </b></legend>
                        <select id="selSalesQuotaQuarter">                            
                            <option value="Q1" selected="selected"> Quarter 1 (Jan-Mar)</option>
                            <option value="Q2"> Quarter 2 (Apr-Jun)</option>
                            <option value="Q3"> Quarter 3 (Jul-Sep)</option>
                            <option value="Q4"> Quarter 4 (Oct-Dec)</option>
                        </select>
                    </fieldset>
                    <a id="btnDoneSalesQuotaQuarterPeriod" data-role="button" href="#" data-theme="e">Done</a>
                </form>
            </div>       
        </div>

        <div data-role="popup" id="panFOWST" data-theme="b" class="ui-corner-all"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>FOWST</h3></center>
            <div data-role="content">
                <form>
                    <table id="tblFOWST" style="width:100%">                    
                            </table>    
                </form>
            </div>       
        </div>

        <div data-role="popup" id="panSettings" data-theme="b" class="ui-corner-all"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Graph Type</h3></center>
            <div data-role="content">
                <form>
                    <select id="selSettings">
                        <option value="optSales">Sales</option>
                        <option value="optRefunds">Refunds</option>
                        <option value="optExchange">Exchange</option>
                        <option value="optReceiving">Receiving</option>
                        <option value="optTransferOut">Transfer-Out</option>
                        <option value="optInventoryCount">Inventory Count</option>
                    </select>

                    <select id="selSettingsDetails">
                        <option value="optSalesTrendAndDistribution">Sales Trend and Distribution</option>
                        <option value="optSalesRatioOrderToTarget">Sales Ratio</option>
                    </select>
                    <br />
                    <a href="#" data-role="button" id="btnDoneSettings" data-theme="e">Done</a>
                </form>
            </div>       
        </div>

        <div data-role="popup" id="panSalesDistribution" data-theme="b" class="ui-corner-all"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Sales Distribution</h3></center>
            <div data-role="content">
                <form>
                    <ul data-role="listview" data-inset="true" data-theme="e" id="lstSlice">
                      
                    </ul>
                </form>
            </div>       
        </div>

        <div data-role="popup" id="panDice" data-theme="b" class="ui-corner-all"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Sales Distribution</h3></center>
            <div data-role="content">
                <form>
                    <ul data-role="listview" data-inset="true" data-theme="e" id="lstDice">
                       
                    </ul>
                </form>
            </div>       
        </div>

        <div data-role="popup" id="panMinch" data-theme="b" class="ui-corner-all"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Sales Distribution</h3></center>
            <div data-role="content">
                <form>
                    <ul data-role="listview" data-inset="true" data-theme="e" id="lstMinch">
                       
                    </ul>
                </form>
            </div>       
        </div>

        <ul data-role="listview" data-inset="true">
            <li data-role="list-divider" id="liMainChart">Sales Trend</li>
            <li>
                <div style="text-align:center">
                    <label id="IsDataPresent"> -No Data Available- </label>
                </div>
                <div id="dvMainChart"> 
                    <table style="text-align:center; width:100%">
                        <tr>
                            <td>  </td>
                        </tr>                
                   </table>
                </div>

                <div>
                    <br />
                    <label id="lblMainChartInstruction" style="color:Red; visibility: hidden; font-size:small"> * Click the chart to breakdown data</label>
                </div>
                <br />
                <table style="text-align:center; width:100%; visibility:hidden" id="tblDateFormatLabel">
                    <tr>
                        <td> <label id="lblDateFormatMessage"></label>   </td>              
                    </tr>                
               </table>

               <br />
               <div id="dvSalesDistributionTable">
                    <table data-role="table" class="ui-responsive" style="width:100%; visibility:hidden; text-align:center" id="tblGrossSalesLabels">
                        <thead>
                            <tr>                            
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <th> Total Gross sales for the period </th>
                                <td id="tdTotalGrossSales"> 0  </td>                            
                            </tr>
                            <tr>
                                <th> Less Discounts </th>
                                <td id="tdDiscounts"> 0 </td>                            
                            </tr>

                            <tr>
                                <th> Less Sales Returns </th>
                                <td id="tdSalesReturn"> 0 </td>                            
                            </tr>

                            <tr>
                                <th></th>
                                <td>  </td>                            
                            </tr>

                            <tr>
                                <th> Total Net Sales for the period </th>
                                <td id="tdNetSales"> 0 </td>
                            </tr>
                            <tr>
                                <th> <a href="#" id="lnkTransactionDetails">  Click here to view transaction details </a> </th>
                                <td>  </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <hr />
                                </td>
                            </tr>
                        
                        </tbody>                    
                   </table>               

               <br />                              
                   <table data-role="table" data-mode="reflow" class="ui-responsive" style="width:100%; visibility:hidden" id="tblLastInvoiceDetails">
                        <thead>
                            <tr>
                                <th> Date and Time of Last Transaction Received in Server </th>
                                <th> Last Invoice Number Received in Server </th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td id="tdInvoiceDateAndTime"> 0  </td>
                                <td id="tdInvoiceNumber"> 0 </td>
                            </tr>
                        </tbody>
                   </table>
                </div>

               <div id="dvSalesOrderTable">               
               <table data-role="table" data-mode="reflow" class="ui-body-d ui-shadow table-stripe ui-responsive" style="width:100%; visibility:hidden" id="tblSalesOrderWeek">
                    <thead>
                        <tr>
                            <th align="center">
                                <label id="lblWeekOrMonth">  </label> 
                            </th>
                            <th align="right">
                                <label style="background-color:#73C774"> &nbsp; &nbsp; &nbsp; &nbsp; </label> &nbsp; Target
                            </th>
                            <th>
                                &nbsp; &nbsp; <label style="background-color:#C7754C"> &nbsp; &nbsp; &nbsp; &nbsp; </label> &nbsp; Actual
                            </th>
                        </tr>
                     
                    </thead>
                    <tbody id="tblSalesOrderWeekDetails">

                    </tbody>                    
                </table>
                   <br /> <br />
                   
                   <table data-role="table" data-mode="reflow" class="ui-body-d ui-shadow table-stripe ui-responsive" style="width:100%; visibility:hidden; display: table" id="tblSalesOrder">
                        <thead>
                            <tr>
                                <th style="text-align:center"> Sales Order No. </th>
                                <th style="text-align:center"> Invoice Date </th>
                                <th style="text-align:center"> Customer Name </th>
                                <th style="text-align:center"> Sales Order Value </th>
                            <tr>
                        </thead>
                        <tbody id="tblSalesOrderDetails">
                        </tbody>                    
                   </table>
               </div>
            </li>
        </ul>        
    </div>

    <div id="dvSlice" style="visibility:hidden">
        <ul data-role="listview" data-inset="true">
            <li data-role="list-divider" data-theme="e">&nbsp;<a id="liSecondChartMainLabel" style="font-size:large"></a><a href="#" id="liSecondChart" style="font-size:large"></a> </li>
            <li> 
                <div id="dvSecondChart"></div>
                <div>                        
                        <label id="lblSecondChartInstruction" style="color:Red; font-size:small"> </label>
                        <br /> <br />
                        <table id="tblSecondChart" style="width:50%"> </table>             
                </div>                
             </li>
        </ul>        
    </div>

    <div id="dvDice" style="visibility:hidden">
        <ul data-role="listview" data-inset="true">
            <li data-role="list-divider" data-theme="e">&nbsp;<a id="liThirdChartMainLabel" style="font-size:large"></a> <a href="#" id="liThirdChartLabel" style="font-size:large"></a> </li>
            <li>
                <div id="dvThirdChart">                             
                        </div>
                <br /> <br />
                <table id="tblThirdChart" style="width:50%"> </table>
             </li>
        </ul>        
    </div>

    <div id="dvMinch" style="visibility:hidden">
        <ul data-role="listview" data-inset="true">
            <li data-role="list-divider" data-theme="e">&nbsp;<a id="liFourthChartMainLabel" style="font-size:large"></a> <a href="#" id="liFourthChartLabel" style="font-size:large"></a> </li>
            <li>
                <div id="dvFourthChart">                             
                </div>
                <br /> <br />
                <table id="tblFourthChart" style="width:50%"> </table>
             </li>
        </ul>        
    </div>

    <div data-role="popup" id="panTransactionDetails" data-theme="b" class="ui-corner-all">        
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Transaction Details</h3></center>
            <div data-role="content">
                    <ul data-role="listview" data-inset="true" id="lstTransactionDetails">
                        <li data-role="list-divider">POS Sales Transactions</li>
                        <li>
                            <div data-role="fieldcontain">
                                <label for="txtSearchProduct"> Search Product </label>
                                <input id="txtSearchProduct"/>
                            </div>
                            
                            <br />
                           <table data-role="table" data-mode="reflow" class="ui-body-d ui-shadow table-stripe ui-responsive" text-align:center" id="tblTransactionDetails">
                                <thead>
                                    <th style="border-bottom: 1px solid #d6d6d6;"> Invoice No. </th>
                                    <th style="border-bottom: 1px solid #d6d6d6;"> Invoice Date </th>
                                    <th style="border-bottom: 1px solid #d6d6d6;"> Product Description </th>
                                    <th style="border-bottom: 1px solid #d6d6d6;"> Qty </th>
                                    <th style="border-bottom: 1px solid #d6d6d6;"> Unit Measure </th>                        
                                    <th style="border-bottom: 1px solid #d6d6d6;"> Amount </th>
                                    <th style="border-bottom: 1px solid #d6d6d6;"> Sub Total </th>
                                    <th style="border-bottom: 1px solid #d6d6d6;"> Discount </th>
                                    <th style="border-bottom: 1px solid #d6d6d6;"> Sales Representative </th>                                    
                                </thead>
                                <tbody id="tblTransactionDetailsBody">                                                
                                </tbody>                    
                           </table>
                        </li>                       
                    </ul>

                    <ul data-role="listview" data-inset="true">
                        <li data-role="list-divider">POS Sales Returns</li>
                        <li> 
                            <table data-role="table" data-mode="reflow" class="ui-responsive" id="tblSalesReturnDetails">
                                <thead>
                                    <th> Memo Number </th>
                                    <th> Memo Date </th>                                
                                    <th> Srv No. </th>
                                    <th> Amount </th>
                                </thead>
                                <tbody id="tblSalesReturnDetailsBody">
                                </tbody>
                           </table>
                        </li>
                    </ul>
            </div>       
        </div>

    @*<a href="#" data-role="button" id="btnDownloadDiceAsExcel" style="visibility:hidden">Download raw data as excel</a>*@

    <br /> <br /> <br />

@*</div>*@