@*@Code
    ViewData("Title") = "POS Transaction Detail"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code*@
@Section head
    @*<link rel="stylesheet" href="/Content/css/jquery.jqplot.min.css" />
    <link rel="stylesheet" href="/Content/css/ui.dynatree.css" />
    <link rel="stylesheet" href="/Content/css/qunit.css" />*@
    
End Section
<script>
    require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/POSTransactionDetail.js?crc=' + assetversion)]);
</script>

<div data-role="page">
    <div class="row mb-3 d-none">
        <label class="col-md-2 mb-3">
            Transaction Range Type:
        </label>
        <div class="col-md-auto">
            <select id="selTransactionRangeType" data-role="none" class="form-select">
                <option value=""> Select Range Type </option>
                <option value="optMonth">Month </option>
                <option value="optDate" selected>Date </option>
            </select>
        </div>
    </div>
    <div id="dvTransactionRangeType">
        <div class="row mb-3">
            <label class="col-md-2 mb-3">
                Select Store:
            </label>
            <div class="col-md-auto">
                <select id="selPOSTransactionFOWST" data-role="none" class="form-select"></select>
            </div>
        </div>

        <div class="row mb-3 align-items-center">
            <div class="col-md-2">
                Select Store Machine:
            </div>
            <div class="col-md-auto">
                <select data-role="none" class="form-select" id="selPOSTransactionFOWSTDevices"></select>
            </div>
        </div>

        <div id="dvMonth" style="visibility:hidden; display:none">
            <div class="row mb-3">
                <label class="col-md-2 col-form-label mb-3">
                    Month:
                </label>
                <div class="col-md-2 mb-3">
                    <select id="selMonth" data-role="none" class="form-select">
                        <option value="" selected="selected">Select Month </option>
                        <option value="1">January </option>
                        <option value="2">February </option>
                        <option value="3">March </option>
                        <option value="4">April </option>
                        <option value="5">May </option>
                        <option value="6">June </option>
                        <option value="7">July </option>
                        <option value="8">August</option>
                        <option value="9">September </option>
                        <option value="10">October </option>
                        <option value="11">November </option>
                        <option value="12">December </option>
                    </select>
                </div>
                <div class="col-md-2 mb-3">
                    <select id="selYear" data-role="none" class="form-select">
                        <option value="" selected="selected">Select Year </option>
                        <option value="2013">2013 </option>
                        <option value="2014">2014 </option>
                        <option value="2015">2015 </option>
                    </select>
                </div>
            </div>
        </div>

        <div id="dvDay">
            <div class="row mb-3">
                <label class="col-md-2 col-form-label mb-3">
                    Date Period:
                </label>
                <div class="col-md-auto mb-3">
                    <input data-role="none" type="date" class="form-control" id="txtAuditDateFrom" />
                    @*<select id="selFromMonth" data-role="none" class="form-select">
                        <option value="1">January </option>
                        <option value="2">February </option>
                        <option value="3">March </option>
                        <option value="4">April </option>
                        <option value="5">May </option>
                        <option value="6">June </option>
                        <option value="7">July </option>
                        <option value="8">August</option>
                        <option value="9">September </option>
                        <option value="10">October </option>
                        <option value="11">November </option>
                        <option value="12">December </option>
                    </select>*@
                </div>
                <div class="col-md-auto mb-3 align-self-center">
                    to
                    @*<select id="selFromDay" data-role="none" class="form-select selday"></select>*@
                </div>
                <div class="col-md-auto">
                    <input data-role="none" type="date" class="form-control" id="txtAuditDateTo" />
                    @*<select id="selFromYear" data-role="none" class="form-select selyear">
                        <option value="" selected="selected">Select Year </option>
                        <option value="2013">2013 </option>
                        <option value="2014">2014 </option>
                        <option value="2015">2015 </option>
                    </select>*@
                </div>
            </div>

            <div class="row mb-3 d-none">
                <label class="col-md-2 col-form-label mb-3">
                    Date To:
                </label>
                <div class="col-md-auto mb-3">
                    <select id="selToMonth" data-role="none" class="form-select">
                        <option value="1">January </option>
                        <option value="2">February </option>
                        <option value="3">March </option>
                        <option value="4">April </option>
                        <option value="5">May </option>
                        <option value="6">June </option>
                        <option value="7">July </option>
                        <option value="8">August</option>
                        <option value="9">September </option>
                        <option value="10">October </option>
                        <option value="11">November </option>
                        <option value="12">December </option>
                    </select>
                </div>
                <div class="col-md-2 mb-3">
                    <select id="selToDay" data-role="none" class="form-select selday"></select>
                </div>
                <div class="col-md-2">
                    <select id="selToYear" data-role="none" class="form-select selyear">
                        <option value="" selected="selected">Select Year </option>
                        <option value="2013">2013 </option>
                        <option value="2014">2014 </option>
                        <option value="2015">2015 </option>
                    </select>
                </div>
            </div>
        </div>

        <div class="mt-4 text-center">
            <button id="btnGenerateData" data-role="none" class="btn btn-success btn-lg">Download File</button>
        </div>
    </div>
    <ul data-role="listview" data-inset="true" style="display:none;">
        <li data-role="list-divider">Raw Data</li>
        <li>
            <table data-role="table" id="tblPOSRawDataTable" data-mode="reflow" class="ui-responsive">
                <thead>
                    <tr>
                        <th> Account ID </th>
                        <th> Store ID </th>
                        <th> MIN </th>
                        <th> Transaction </th>
                        <th> Transaction Date </th>
                        <th> UPC Code </th>
                        <th> Sold Qty </th>
                        <th> Sold Amt </th>
                        <th> Sold Amt Discount </th>
                        <th> Transaction Type </th>
                        <th> MONTH </th>
                        <th> YEAR </th>
                        <th> Reference SI </th>
                    </tr>
                </thead>
                <tbody id="tblPOSRawDataBody"></tbody>
            </table>
        </li>
    </ul>
    <ul data-role="listview" data-inset="true" style="display:none;">
        <li data-role="list-divider">Summary</li>
        <li>
            <table data-role="table" id="tblPOSSummary" data-mode="reflow" class="ui-responsive">
                <thead>
                    <tr>
                        <th> Company Name </th>
                        <th> FOWST </th>
                        <th> MIN </th>
                        <th> Total Sales </th>
                        <th> Total Discounts </th>
                        <th> Transaction Type </th>
                        <th> MONTH </th>
                        <th> YEAR </th>
                        <th> OR Start Range </th>
                        <th> OR End Range </th>
                        <th> Is Series Skip?</th>
                    </tr>
                </thead>
                <tbody id="tblPOSSummaryBody"></tbody>
            </table>
        </li>
    </ul>
</div>