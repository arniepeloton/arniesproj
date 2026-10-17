

<script>
    require([GetPathFormat('/Scripts/sources/KnowledgeManagementAndQuestionaire/KnowledgeManagementAndQuestionaire.js?crc=')
    ]);
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

    .btnNotRequired {
        border: 2px solid #FFF;
    }
</style>

<div id="pageSalesReport">
    
    <div data-role="content">


        <div style="width:100%" id="dvReportHeader">
            <div style="float:left">
                <button style="margin-right:10px;" data-role="none" class="btn btn-less-width btn-transparent btnRequired btnCashierSelectFowst">
                    <span class="lblCashierSelectedFowstGroup"> FOWST Location </span>
                    <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span>
                </button>
            </div>

            <div id="divAddLibrary" style="float:left">
                <div style="float:left">
                    <button id="btnCashierDateRange" style="margin-right:10px;" data-role="none" class="btn btn-less-width btn-transparent btnNotRequired">
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
                <select id="selPOSSalesReportRangeByYear"></select>
            </div>

            <div style="text-align:center"> <a class="btnPOSSalesReportView" data-role="button">View</a> </div>

        </div>

        <br /> <br />

        <div id="divDisplayPOSSalesReport">            
                <table data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe">
                    <thead>
                        <tr id="tblHeader" class="custom-theme-f">
                            <th> Inventory Form Date </th>
                            <th> Product Description </th>
                            <th> Beginning Balance </th>
                            <th> Sales </th>
                            <th> Transfer-Out </th>
                            <th> Receiving </th>
                            <th> Sales Returns </th>
                            <th> Inventory Count Variance </th>
                            <th> Ending Balance </th>
                        </tr>
                    </thead>
                        <tbody id="tbodyDetails"></tbody>
                    </table>
            </div>

            <div>
                <br />
                <br />

                @*<div style="text-align:center"> <a id="btnPOSSalesReportPrint" data-role="button"><h3>Print</h3></a> </div>
                <div style="text-align:center; visibility:hidden; display:none" id="dvDepositMonies"> <a id="btnDepositMonies" data-role="button"><h3>Deposit/Turn-over Monies</h3></a> </div>*@
                <div style="text-align:center"> <a id="btnPOSSalesReportCancel" data-role="button" data-rel="back"><h3>Go Back To Main</h3></a></div>
            </div>

            <div data-role="popup" id="popupMenu">
                <div data-role="popup" id="dvpopup" data-theme="a" class="ui-corner-all" data-dismissible="false" data-overlay-theme="a">
                    <p id="popMessage" style="padding:10px 20px;text-align:center"></p>
                </div>
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

    <div data-role="page" id="panDepositMonies" data-overlay-theme="a" data-theme="a" data-corners="false">
        @*<a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>*@
        @*<center><h3>Deposit Monies</h3></center>*@
        <div data-role="content" class="ui-content ui-body-d">

        </div>
    </div>
</div>

