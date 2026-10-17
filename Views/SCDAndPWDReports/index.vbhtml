

<script>
    require([GetPathFormat('/Scripts/sources/SCDAndPWDReports/SCDAndPWDReports.js?crc=' + assetversion)
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
        <div class="row m-0 gy-2 gx-3 mb-3 align-items-center">
            <div class="col-md-auto">
                <div class="row">
                    <label for="btnSCDSelectFowst" class="col-3 col-md-auto col-form-label">Store:</label>
                    <div class="col col-md-auto">
                        <button style="margin-right:10px;" data-role="none" class="btn btn-light border btn-block btnSCDSelectFowst">
                            <span class="lblSCDSelectedFowstGroup"> Please select </span>
                        </button>
                    </div>
                </div>
            </div>
            <div class="col-md-auto divSummaryDate">
                <div class="row">
                    <label for="btnSystemWideDate" class="col-3 col-md-auto col-form-label">Date:</label>
                    <div class="col col-md-auto">
                        <button data-role="none" id="btnSCDDateRange" class="btn btn-light border btn-block">Set Date</button>
                    </div>
                </div>
            </div>
            <div class="col-md-auto">
                <div class="row">
                    <label for="btnSystemWideDate" class="col-3 col-md-auto col-form-label">Show Discount:</label>
                    <div class="col col-md-auto">
                        <select data-role="none" class="form-select" id="selDRDiscountType"></select>
                    </div>
                </div>
            </div>
            <div class="col-md">
                <input data-role="none" class="form-control" placeholder="Search here" id="txtSearchSCDPWDReports">
            </div>
        </div>

        <div class="table-responsive">
            <table id="tblSeniorCitizenAndPWDDiscount" data-role="none" class="table table-light table-striped table-hover">
                <thead>
                    <tr id="tblHeader" class="align-top text-center" style="font-size:14px;">
                        <th data-sort="date" style="cursor:pointer;"> Invoice Date </th>
                        <th data-sort="string" style="cursor:pointer;"> Invoice No </th>
                        <th data-sort="string" style="cursor:pointer;"> Gross Amount </th>
                        @*<th data-sort="float" style="cursor:pointer;"> No. Of Customers </th>
        <th data-sort="float" style="cursor:pointer;"> No. Of SC/PWD </th>*@
                        <th data-sort="string" style="cursor:pointer;"> VATable Sales </th>
                        <th data-sort="float" style="cursor:pointer;"> VAT Exempt Sales</th>
                        <th data-sort="float" style="cursor:pointer;"> Zero Rated Sales</th>
                        <th data-sort="float" style="cursor:pointer;"> Discount </th>
                        <th data-sort="string" style="cursor:pointer;"> Customer Name </th>
                        <th data-sort="string" style="cursor:pointer;"> Customer ID No </th>
                        @*<th data-sort="float" style="cursor:pointer;"> Gross Amount Subject to SC/PWD Discount </th>*@
                        <th data-sort="float" style="cursor:pointer;"> Discount Type </th>
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

    </div>
</div>