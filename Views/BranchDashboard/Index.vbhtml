@Code
    ViewData("Title") = "Retail Store Dashboard"
    Layout = "~/Views/DojoLayout/Layout2.vbhtml"
End Code
<script type="text/javascript" src="../../Scripts2/Dashboard/dashboard.js"> 
</script>
<style>
    .LastImportBulb
    {
        background-color: red;
        border-radius: 3px 3px 3px 3px;
        display:inline-block;
        height: 15px;
        margin-right: 3px;
        width: 15px;
        vertical-align: bottom;
    }
    .divLegend 
    {
        margin-top: 20px;
    }
    .divLegend span 
    {
        margin-right:20px;
    }
    .divLogType
        {
            float: left;
            margin: 10px;
            padding: 10px;
            cursor: pointer;
            width: 250px;
        }
        .divLogType .LogTypeImage
        {
            width: 100px;
            height: 100px;
            float: left;
            margin-right: 10px;
        }
        .divLogType .LogTypeHead
        {
            font-size: 16pt;
            display: block;
        }
        .divSlideButton
        {
            background-color: white;
            border: 1px solid grey;
            width: 20px;
            height: 20px;
            margin: 5px;
            cursor: pointer;
            float: left;
            -webkit-border-radius: 0px;
            -moz-border-radius: 0px;
            border-radius: 0px;
        }
        .divSlideButtonActive
        {
            background-color: Orange;
        }
        .divSlide
        {
            width: 830px;
            height: 450px;
            float: left;
        }
        .LoginButtons 
        {
            background: #ffffff; /* Old browsers */
            background: -moz-linear-gradient(top, #ffffff 0%, #f7f7f7 100%); /* FF3.6+ */
            background: -webkit-gradient(linear, left top, left bottom, color-stop(0%,#ffffff), color-stop(100%,#f7f7f7)); /* Chrome,Safari4+ */
            background: -webkit-linear-gradient(top, #ffffff 0%,#f7f7f7 100%); /* Chrome10+,Safari5.1+ */
            background: -o-linear-gradient(top, #ffffff 0%,#f7f7f7 100%); /* Opera 11.10+ */
            background: -ms-linear-gradient(top, #ffffff 0%,#f7f7f7 100%); /* IE10+ */
            background: linear-gradient(top, #ffffff 0%,#f7f7f7 100%); /* W3C */
            filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#ffffff', endColorstr='#f7f7f7',GradientType=0 ); /* IE6-9 */
            box-shadow: 0px 0px 5px;
            margin: 5px;
            
        }
        .LoginButtons a:hover
        {
            text-decoration: none;
        }
</style>
<script type="text/javascript">
    $(function () {
        $("#deposit").click(function () {
            window.location = 'EndOfDaySalesDashboard/';
        });
    });
</script>

<h1 id="bd-title-header">@ViewData("Title")</h1>
<hr />
<p>The Paper Tree Store Cluster</p>

  <div id="ducDojoTabControl">
   <div id="tab1">
        <!-- Menu Bar -->
        <div id="ducDojoMenuBar">
            @*<div style="float:left;padding:10px;width:100px;text-align:center" id="sales" class="LoginButtons">Sales</div>
            <div style="float:left;padding:10px;width:100px;text-align:center" id="sku" class="LoginButtons">SKU</div>
            <div style="float:left;padding:10px;width:100px;text-align:center" id="deposit" class="LoginButtons">Sales: Deposit</div>*@
            <button id="sales" style="width:150px;">Sales</button>
            <button id="sku" style="width:150px;">SKU</button>
            <button id="deposit" style="width:150px;">Sales: Deposit</button>
        </div>
        <div style="clear:both;"></div>

        <!-- Dojo Grid -->
        <div id="ducDojoDashboardLabel">Please wait while loading all the records in the grid...</div>
        <div style="float: right;">
            <button id="btnPreviousWeek"> << </button> <button id="btnNextWeek"> >> </button>
        </div>
        <div style="clear:both;display:none;"></div>
        <hr />
        <div class="widget">
            <h3 class="handle">
                Sales Comparison: Per Branch Comparison <span id="bd-loading-1"></span>
            </h3>
            <div style="display: block;">
                <div id="gridDiv" style="margin:auto 0;"></div>
                <div class="divLegend clearfix">
                        <span>Legend:</span>
                        <div class="LastImportBulb" style="background-color: Green"></div><span>Last transmission was 10 (or less) minutes ago</span>
                        <div class="LastImportBulb"></div><span>Last transmission was more than 10 minutes ago</span>
                        <span>Current server time:<span id="spanServerTime"></span></span>
                    </div>
                </div>
            <div style="display: block;">
                <select id="bd-trailing-branch">
                </select>
                <div id="Chart">
                </div>
            </div>
        </div>
        <div style="clear:both;"></div>
        <!-- End Menu Bar -->
    </div>
    <div id="tab2">
        <!-- Menu Bar -->
        <div id="ducMonthlyDojoMenuBar"></div>
        <!-- Dojo Grid -->
        <div id="ducDojoDashboardLabel">Please wait while loading all the records in the grid...</div>
        <hr />
        <div class="widget">
            <h3 class="handle">
                Sales Comparison: Per Branch Comparison <span id="bd-loading-1"></span>
            </h3>
            <div style="display: block;">
                <div id="gridDiv" style="margin:auto 0;"></div>
            </div>
        </div>
        <br />
        <div class="widget">
            <h3 class="handle">
                Sales Comparison: Per Branch Comparison <span id="bd-loading-1"></span>
            </h3>
            <div style="display: block;">
                <select id="bd-trailing-branch">
                </select>
                <div id="Chart">
                </div>
            </div>
        </div>
        <div style="clear:both;"></div>
        <!-- End Menu Bar -->
    </div>
    <div id="tab3">
        <!-- Menu Bar -->
            
        <!-- End Menu Bar -->    
    </div>
</div>

<!--
<div class="tab">
    <ul>
        <li><a href="#tabs-1">SALES</a></li>
        <li><a href="#tabs-2">SKU</a></li>
    </ul>
    <div id="tabs-1">
        <div class="widgets">
            
            <div class="widget">
                <h3 class="handle">
                    POS Sales and Deposit Comparison: Per Branch <span id="bd-loading-2"></span>
                </h3>
                <div style="display: block;">
                    <div>
                        <span><button id="btnToday">Today</button></span>
                        <span><button id="btnYesterday">Yesterday</button></span>
                        <span><button id="btnOnTheOtherDay">On The Other Day</button></span>
                    </div>
                    <table class="bd-table">
                        <thead>
                            <tr>
                                <th></th>
                                <th colspan="2">A</th>
                                <th></th>
                                <th>B</th>
                                <th>C</th>
                                <th colspan="3">Variance</th>
                            </tr>
                            <tr>
                                <th>Branch Description</th>
                                <th>Date Deposited</th>
                                <th>Deposited Amount</th>
                                <th>Sales Date</th>
                                <th>
                                    End of Day
                                    <br />
                                    Sales
                                    <br />
                                    (Local POS Data)
                                </th>
                                <th>
                                    End of Day
                                    <br />
                                    Sales
                                    <br />
                                    (Consolidated Server Data)
                                </th>
                                <th>A - B</th>
                                <th>A - C</th>
                                <th>B - C</th>
                            </tr>
                        </thead>
                        <tbody id="obdr-results">
                            <tr>
                                <td colspan="9" align="center">No Record Found</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
</div> -->