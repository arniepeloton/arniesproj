

<script>
    require([GetPathFormat('/Scripts/sources/InventoryForm/InventoryForm.js?crc=')
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

        <div class="row" id="dvReportHeader">
            <div class="col-6 col-md-3 mb-3">
                FOWST: <br />
                <button style="margin-right:10px;" id="btnInventoryFormFowst" data-role="none" class="btn btn-secondary btn-block text-truncate">
                    <span class="lblInventoryFormFowstFowstGroup"> FOWST Location </span>
                </button>
            </div>
            <div class="col-6 col-md-3 mb-3">
                Date: <br />
                <button id="btnCashierDateRange" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                    <span id="CashierDateRangeFrom">Today</span>
                    <span id="CashierDateRangeSeparator" style="display:none;">to</span>
                    <span id="CashierDateRangeTo"></span>
                </button>
            </div>
            <div class="col-6 col-md-3 mb-3">
                <br class="d-none d-md-block" />
                <input type="text" data-role="none" class="form-control" id="txtSearchProduct" placeholder="Search Product here" />
            </div>
            <div class="col-6 col-md-3 mb-3">
                <br class="d-none d-md-block" />
                <button id="btnDownloadAsExcel" style="margin-right:10px;" data-role="none" class="btn btn-secondary btn-block">
                    Download As Excel
                </button>
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

        <br /> <br />

        <div id="divDisplayPOSSalesReport">            
            <table id="tblInvetoryFormReport" data-role="table" data-mode="reflow" style="margin-right:5px; display:table; width:100%;" class="ui-table ui-responsive table-stroke table-stripe table table-light table-striped table-hover">
                <thead>
                    <tr id="tblHeader" class="align-top text-center" style="font-size:14px;">
                        <th data-sort="date" style="cursor:pointer;"> Inventory Form Date </th>
                        <th data-sort="string" style="cursor:pointer;"> Product Description </th>
                        <th data-sort="float" style="cursor:pointer;"> Beginning Balance </th>
                        <th data-sort="float" style="cursor:pointer;"> Sales </th>
                        <th data-sort="float" style="cursor:pointer;"> Transfer-Out </th>
                        <th data-sort="float" style="cursor:pointer;"> Receiving </th>
                        <th data-sort="float" style="cursor:pointer;"> Sales Returns </th>
                        <th data-sort="float" style="cursor:pointer;"> Inventory Count Variance </th>
                        <th data-sort="float" style="cursor:pointer;"> Ending Balance </th>
                        <th data-sort="string" style="cursor:pointer;"> Employee </th>
                    </tr>
                </thead>
                <tbody id="tbodyDetails"></tbody>
            </table>
        </div>

            <div>
                <br />
                <br />

                @*<div style="text-align:center"> <a id="btnPOSSalesReportPrint" data-role="button"><h3>Print</h3></a> </div>
    <div style="text-align:center; visibility:hidden; display:none" id="dvDepositMonies"> <a id="btnDepositMonies" data-role="button"><h3>Deposit/Turn-over Monies</h3></a> </div>
    <div style="text-align:center"> <a id="btnPOSSalesReportCancel" data-role="button" data-rel="back"><h3>Go Back To Main</h3></a></div>*@
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

