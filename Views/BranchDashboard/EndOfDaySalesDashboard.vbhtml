@Code
    ViewData("Title") = "Retail Store Dashboard"
    Layout = "~/Views/DojoLayout/Layout2.vbhtml"
End Code
<script type="text/javascript" src="../../Scripts2/Dashboard/endOfDaySalesDashboard.js"> 
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

<script type="text/javascript" src="../../Scripts2/Dashboard/jsFilterByDesign.js"></script>
<script type="text/javascript">
    $(function () {
        $("#sales").click(function () {
            window.location = '../';
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
           @* <div style="float:left;padding:10px;width:100px;text-align:center" id="sales" class="LoginButtons">Sales</div>
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
                Sales To Bank Deposit Comparison <span id="bd-loading-1"></span>
            </h3>
            <div style="display: block;">
                <div id="ui-filter-by-main">
                    <div id="ui-filter-by">
                        Filter By: 
                        <select id="selectFilterBy">
                            <option value="select">Select Filter Option</option>
                            <option value="POS">POS</option>
                            <option value="Cashier">Cashier</option>
                        </select>
                    </div>
                    <div id="ui-filter-choice">
                    </div>
                    <div id="ui-display"></div>
                    <div id="ui-date-range">
                        <span>Date From: <input type="text" id="dateStart" class="textInput" /></span> <span>Date To:</span> <input type="text" id="dateEnd" class="textInput" /><br />
                        <span>* Default date range is Month-To-Date</span>
                    </div>
                    <div style="padding-left:10px;padding-bottom:10px;"><button id="buttonSearch">Search</button></div>
                </div>
                
                <!-- Presentation of Filter -->
                <div id="gridDiv" style="margin:auto 0;"></div>
            </div>
            <div style="display: none;" id="pop-up"></div>
        </div>

        <div style="clear:both;display:none;"></div>
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