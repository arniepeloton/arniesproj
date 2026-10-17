<script>
    require([GetPathFormat('/Scripts/sources/CashHandling/DepositReport.js?crc=' + assetversion)]);
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

    .tooltip {
        position: absolute;
        display: none;
        z-index: 1000;
        background-color: black;
        color: white;
        max-width: 200px;
        box-shadow: 0px 4px 5px #CCC;
    }

    .tooltip-container {
        background: #000;
        display: table;
        text-shadow: 0px 1px 1px #000;
    }

    .grid-left, .grid-right {
        display: table-cell;
        padding: 5px;
    }

    .grid-left {        
        background: #3C3C3C;
        font-size: 12px;
    }

    .grid-right {
        background: #3C3C3C;
        font-size: 12px;
    }
</style>

<div id="pageDepositReport">
    
    <div data-role="popup" id="panTurnOverFilter" data-overlay-theme="a" data-theme="a" data-corners="false"> 
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3>Filter</h3></center>
        <div data-role="content" class="ui-content ui-body-d">
            <div style="padding: 8px 15px 16px 15px; max-height:200px; overflow:auto;">
                <ul data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm">
                    <li><a id="btnTurnover">Turnover</a></li>
                    <li><a id="btnDeposit">Deposit</a></li>
                </ul>
            </div>
        </div>       
    </div>

    <div data-role="content">

        <div id="divSecond" style="display:block;">
            
            <div style="display:block">
                <div class="row">
                    <div class="col-auto mb-3">
                        FOWST: <br />
                        <button id="btnMonieAccFOWST" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                            <span class="lblSelectedFowstGroup">FOWST Location</span>
                        </button>
                    </div>
                    <div class="col-auto mb-3">
                        Date: <br />
                        <button style="margin-right:10px;" data-role="none" class="btn btn-less-width custom-btn-theme-c btn-secondary btn-block btnPeriod">
                            <span class="lblPeriod"> Today </span>
                        </button>
                    </div>
                </div>

                <div style="width:100%">
                    <button id="btnReceivingTransferDateRange" style="margin-right:10px; visibility:hidden;display:none;" data-role="none" class="btn btn-less-width btn-transparent btnRequired"><span id="DateRangeFrom">Date</span> <span id="DateRangeSeparator" style="display:none;">to</span> <span id="DateRangeTo">Range</span><span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span></button>
                    @*<div style="float:left">
                        <button style="margin-right:10px;" data-role="none" class="btn btn-less-width btn-transparent btnRequired btnSelectFowst">
                            <span class="lblSelectedFowstGroup"> FOWST Location </span>
                            <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span>
                        </button>
                    </div>
                    <div style="float:left">
                        <button id="btnTurnOverFilter" style="margin-right:10px;" data-role="none" class="btn btn-less-width btn-transparent">
                            <span class="lblTurnOverFilter"> Turnover </span>
                            <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span>
                        </button>
                    </div>*@
                </div>
                
                <br />
                @*<div id="dvDetails" style="width:100%; overflow:auto; visibility:hidden; display:none">
                    <table data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe">
                        <thead>
                            <tr id="tblHeader" class="custom-theme-f">
                                <th data-sort="string" style="cursor:pointer;">Product No</th>
                                <th data-sort="string" style="cursor:pointer;">Product Description</th>
                                <th data-sort="float" style="cursor:pointer;">Balance</th>
                                <th data-sort="string" style="cursor:pointer;">Unit Measure</th>
                                <th data-sort="date" style="cursor:pointer;">Last Balance Posting Date</th>
                            </tr>
                        </thead>
                        <tbody id="tblBody"></tbody>
                    </table>
                </div>*@
            </div>

            <div id="divNoPage" style="display:block; text-align:center">
                <h4>No Data Available</h4>
            </div>

            <div id="divMoniesAcc2" style="display:block;">
                <div class="mb-3" style="width:100%;visibility:hidden;display:block" id="dvDepositReportTable">
                    <table data-role="table" data-mode="reflow" style="margin-right:5px; display:table; display:none;" class="ui-table ui-responsive table-stroke table-stripe">
                        <thead>
                            <tr id="tblHeader" class="custom-theme-f">
                            
                                <th data-sort="date">Sales Transaction Date</th>
                                <th data-sort="date">Turned Over Date</th>
                                <th data-sort="string">Turned-Over By</th>
                                <th data-sort="string">Turned-Over To</th>
                                <th data-sort="float" style="text-align:right">Sales Amount</th>
                                <th data-sort="float" style="text-align:right">Total Accepted Turned-Over</th>
                                <th data-sort="float" style="text-align:right">Total Turned Over</th>
                                <th data-sort="float" style="text-align:right">Variance after Turned Over</th>
                                @*<th rowspan="2">GPS Coordinates</th>
                <th rowspan="2">Signature Pad</th>*@
                            </tr>
                            @*<tr class="custom-theme-f">
                                <th>Cash</th>
                                <th>Check</th>
                                <th>Credit Card</th>
                                <th>Debit Card</th>
                                <th>SRV</th>
                                <th>Petty Cash</th>
                            </tr>*@
                        </thead>
                        <tbody id="tblBodyDepositTable"></tbody>
                    </table>

                    <div class="mb-3" style="margin-left:30px;"><i>NOTE: Please click on any row to view more details.</i></div>

                    <div class="table-responsive mb-3">
                        <table data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="table table-light table-striped table-hover">
                            <thead>
                                <tr id="" class="custom-theme-f">
                                    <th data-sort="string" style="text-align:center; text-shadow:0px 0px 0px; font-weight:normal; min-width:200px;">Person Name</th>
                                    @*<th data-sort="string">Entity Type</th>
                    <th data-sort="string">FOWST</th>*@
                                    <th data-sort="float" style="background-color:#298017; color:#FFF; text-shadow:0px 0px 0px; font-weight:normal; text-align:center;">Total Of Net Sales And Monies Received</th>
                                    <th data-sort="float" style="background-color:#4979EA; font-weight:normal; text-shadow:0px 0px 0px; text-align:center;">Total Of Deposits And Service Charges</th>
                                    <th data-sort="float" style="background-color:#4553FF; font-weight:normal; text-shadow:0px 0px 0px; text-align:center;">Monies Turned Over To Co-Employees</th>
                                    <th data-sort="float" style="background-color:#DC4242; color:#fff; font-weight:normal; text-shadow: 0px 0px 0px; text-align:center;">Gross Variance Of Monies Not Turned Over</th>
                                </tr>
                            </thead>
                            <tbody id="tBodyTurnOver"></tbody>
                        </table>
                    </div>
                </div>            

                <div id="divTurnOverDetails" style="display: none; ">
                    <h2 class="mb-3" id="txtPersonName"></h2>

                    <ul class="nav nav-tabs" id="nav-tab" role="tablist">
                        <li class="nav-item">
                            <a data-role="none" class="nav-link active" data-toggle="tab" href="#divDetails">All</a>
                        </li>
                        <li class="nav-item">
                            <a data-role="none" class="nav-link" data-toggle="tab" href="#divTurnover">Turnover</a>
                        </li>
                        <li class="nav-item">
                            <a data-role="none" class="nav-link" data-toggle="tab" href="#divDeposit">Deposit</a>
                        </li>
                    </ul>
                    
                    <div class="tab-content">
                        <div class="tab-pane fade show active p-3" id="divDetails" style="overflow:auto;">
                            <table data-role="none" class="table table-light table-hover table-striped">
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

                        <div class="tab-pane fade p-3" id="divTurnover" style="overflow:auto;">
                            <table data-role="none" class="table table-hover table-striped">
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

                        <div class="tab-pane fade p-3" id="divDeposit" style="overflow:auto;">
                            <table data-role="none" class="table table-hover table-striped">
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
                </div>
            </div>

            <div id="divInventoriesAcc2" style="display:none;">
                @*<h1>Under Construction</h1>*@

                @*<table data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe">
                    <thead>
                        <tr id="" class="custom-theme-f">                            
                            <th data-sort="date">Inventory Counted by</th>
                            <th data-sort="float">Inventory Counts Turned Over to</th>
                            <th data-sort="string">FOWST</th>
                            <th data-sort="string">Date and Time of Turn Over</th>
                            <th data-sort="float">Value of Counted Inventories Based on SRP (A)</th>
                            <th data-sort="float">Count Status</th>
                            <th data-sort="float">Value of System Balances Based on SRP as of Count Date/Time (B)</th>
                            <th data-sort="string">Turned Over To</th>
                            <th data-sort="float">Variance in Inventory Value (A-B)</th>
                        </tr>
                    </thead>
                    <tbody id="tBodyInventoriesAcc"></tbody>
                </table>*@
            </div>

                @*<div style="width:100%;visibility:hidden;display:none" id="dvDepositReportTable">
                <table data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe">
                    <thead>
                        <tr id="tblHeader" class="custom-theme-f">
                            <th data-sort="date" rowspan="2">Date/Time</th>
                            <th data-sort="string" rowspan="2">From</th>
                            <th data-sort="string" rowspan="2">To</th>
                            <th data-sort="float" rowspan="2">Total Amount</th>
                            <th colspan="6" style="text-align:center">Total Amount Denomination</th>
                            <th rowspan="2">GPS Coordinates</th>
                            <th rowspan="2">Signature Pad</th>
                        </tr>
                        <tr class="custom-theme-f">
                            <th>Cash</th>
                            <th>Check</th>
                            <th>Credit Card</th>
                            <th>Debit Card</th>
                            <th>SRV</th>
                            <th>Petty Cash</th>
                        </tr>
                    </thead>
                    <tbody id="tblBodyDepositTable"></tbody>
                </table>
            </div>*@

            </div>

    </div>

</div>
