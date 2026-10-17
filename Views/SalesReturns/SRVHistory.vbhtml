

<script>
    require([GetPathFormat('/Scripts/sources/SalesReturns/SRVHistory.js?crc=' + assetversion)
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

        @*<div style="width:100%" id="dvReportHeader">
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
                </div>
            </div>

            <div style="clear:both;"></div>

        </div>*@

        <div class="row mb-3 gy-2 gx-3 align-items-center">
            <div class="col-md-auto">
                <div class="row">
                    <label for="btnSRVDatePeriod" class="col-2 col-md-auto col-form-label">Date:</label>
                    <div class="col col-md-auto">
                        <button data-role="none" id="btnSRVDatePeriod" class="btn btn-light border btn-block"><span class="lblSalesPaymentInformationReportDate "> Today </span></button>
                    </div>
                </div>
            </div>
            <div class="col-md-auto">
                <div class="row">
                    <label for="btnSRVFowst" class="col-2 col-md-auto col-form-label">Store:</label>
                    <div class="col col-md-auto">
                        <button data-role="none" id="btnSRVFowst" class="btn btn-light border btn-block"><span class="lblSalesPaymentInformationReportFOWST"> Select Store </span></button>
                    </div>
                </div>
            </div>
            <div class="col-md-auto d-none">
                <div class="row">
                    <label for="btnSRVGroup" class="col-2 col-md-auto col-form-label">Group:</label>
                    <div class="col col-md-auto">
                        <button data-role="none" id="btnSRVGroup" class="btn btn-light border btn-block"><span class="lblSalesPaymentInformationReportGroupFOWST"> Select Group </span></button>
                    </div>
                </div>
            </div>

            <div class="col-md-auto">
                <div class="row">
                    <label for="btnSRVFowst" class="col-2 col-md-auto col-form-label">Show by:</label>
                    <div class="col col-md-auto">
                        <select data-role="none" id="selSRVFilterBy" class="form-select">
                            <option value="all">All Sales Returns</option>
                            <option value="Merchandise Exchange">Merchandise Exchange</option>
                            <option value="Refund">Refund</option>
                        </select>
                    </div>
                </div>
            </div>

            <div class="col-md-auto">
                <input data-role="none" class="form-control" placeholder="Search here" autocomplete="off" id="txtSearchSrvHistory" />
            </div>
        </div>

        <div id="divDisplayPOSSalesReport">
            <table id="tblSRVHistory" data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="display ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive table table-light table-striped table-hover headerfitwidth">
                <thead>
                    <tr id="tblHeader" class="align-top text-center" style="font-size:14px;">
                        <th style="cursor:pointer; min-width:150px;"> Sales Return Date </th>
                        <th style="cursor:pointer; min-width:150px;"> FOWST Location </th>
                        <th style="cursor:pointer; min-width:150px;"> Customer </th>
                        <th style="cursor:pointer; min-width:150px;"> Customer Industry </th>
                        <th style="cursor:pointer; min-width:150px;"> Transacted By </th>
                        <th style="cursor:pointer; min-width:150px;"> Approved By </th>
                        <th style="cursor:pointer; min-width:150px;"> Item Code & Description </th>
                        <th style="cursor:pointer;"> Sales Return Type </th>
                        <th style="cursor:pointer;"> Returned Qty For This Transaction </th>
                        <th style="cursor:pointer;"> Total Value Of Returned Qty </th>
                        <th style="cursor:pointer;"> Total Returned Qty For This Sales Invoice </th>
                        <th style="cursor:pointer;"> Original Qty In Sales Invoice </th>
                        <th style="cursor:pointer;"> Net Qty In Sales Invoice </th>
                        <th style="cursor:pointer;"> Transaction No. </th>
                        <th style="cursor:pointer;"> Sales Invoice No. </th>
                        <th style="cursor:pointer;"> Sales Invoice Date </th>
                    </tr>
                </thead>
                <tbody id="tbodyDetails"></tbody>
            </table>
        </div>

        @*<div>
            <br />
            <br />
            <div style="text-align:center"> <a id="btnPOSSalesReportCancel" data-role="button" data-rel="back"><h3>Go Back To Main</h3></a></div>
        </div>*@

        <div data-role="popup" id="popupMenu">
            <div data-role="popup" id="dvpopup" data-theme="a" class="ui-corner-all" data-dismissible="false" data-overlay-theme="a">
                <p id="popMessage" style="padding:10px 20px;text-align:center"></p>
            </div>
        </div>

    </div>
    
</div>

