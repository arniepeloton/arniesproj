@Code
    'ViewData("Title") = "Report to Principal"
    'Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
    
End Section

<script>
    require([GetPathFormat('/Scripts/sources/ReportToPrincipal/ReportToPrincipal.js?crc=' + assetversion)]);
</script>


<div>
    <div class="mb-4">
        <h1 class="display-6 mb-3">Nike SEA Reports</h1>

        <div class="row">
            <div class="col-sm-1 mb-3">From:</div>
            <div class="col-sm-2 mb-3">
                <select id="selPOSSalesReportByDateFromMonth" data-role="none" class="form-select">
                    <option value="">MMM</option>
                </select>
            </div>
            <div class="col-sm-2 mb-3">
                <select id="selPOSSalesReportByDateFromDay" data-role="none" class="form-select">
                    <option value="">DD</option>
                </select>
            </div>
            <div class="col-sm-2 mb-3">
                <select id="selPOSSalesReportByDateFromYear" data-role="none" class="form-select">
                    <option value="">YYYY</option>
                </select>
            </div>
        </div>

        <div class="row">
            <div class="col-sm-1 mb-3">To:</div>
            <div class="col-sm-2 mb-3">
                <select id="selPOSSalesReportByDateToMonth" data-role="none" class="form-select">
                    <option value="">MMM</option>
                </select>
            </div>
            <div class="col-sm-2 mb-3">
                <select id="selPOSSalesReportByDateToDay" data-role="none" class="form-select">
                    <option value="">DD</option>
                </select>
            </div>
            <div class="col-sm-2 mb-3">
                <select id="selPOSSalesReportByDateToYear" data-role="none" class="form-select">
                    <option value="">YYYY</option>
                </select>
            </div>
        </div>

        <div class="mt-4 text-center">
            <button id="btnGenerateWeeklySalesReport" data-role="none" class="btn btn-secondary mb-3">Generate Nike Sales Report</button>
            <button id="btnGenerateWeeklyInventoryReport" data-role="none" class="btn btn-warning mb-3">Generate Nike Weekly Inventory Report</button>
            <button id="btnGenerateMonthlyInventoryReport" data-role="none" class="btn btn-success mb-3">Generate Nike Monthly Inventory Report</button>
        </div>
    </div>
    
    <div>
        <h1 class="display-6 mb-3">Onitsuka Reports</h1>

        <div class="row">
            <div class="col-sm-1 mb-3">From:</div>
            <div class="col-sm-2 mb-3">
                <select id="selOnitsukaSalesReportByDateFromMonth" data-role="none" class="form-select">
                    <option value="">MMM</option>
                </select>
            </div>
            <div class="col-sm-2 mb-3">
                <select id="selOnitsukaSalesReportByDateFromDay" data-role="none" class="form-select">
                    <option value="">DD</option>
                </select>
            </div>
            <div class="col-sm-2 mb-3">
                <select id="selOnitsukaSalesReportByDateFromYear" data-role="none" class="form-select">
                    <option value="">YYYY</option>
                </select>
            </div>
        </div>

        <div class="row">
            <div class="col-sm-1 mb-3">To:</div>
            <div class="col-sm-2 mb-3">
                <select id="selOnitsukaSalesReportByDateToMonth" data-role="none" class="form-select">
                    <option value="">MMM</option>
                </select>
            </div>
            <div class="col-sm-2 mb-3">
                <select id="selOnitsukaSalesReportByDateToDay" data-role="none" class="form-select">
                    <option value="">DD</option>
                </select>
            </div>
            <div class="col-sm-2 mb-3">
                <select id="selOnitsukaSalesReportByDateToYear" data-role="none" class="form-select">
                    <option value="">YYYY</option>
                </select>
            </div>
        </div>

        <div class="mt-4 text-center">
            <div class="row">
                <div class="col-sm-6 mb-3">
                    <button id="btnGenerateOnitsukaWeeklySalesReport" data-role="none" style="height:100%;" class="btn btn-success btn-block">Generate Onitsuka Sales Report for Sonak Sports Specialists, Inc.</button>
                </div>
                <div class="col-sm-6 mb-3">
                    <button id="btnGenerateOnitsukaInventoryReport" data-role="none" style="height:100%;" class="btn btn-secondary btn-block">Generate Onitsuka Inventory Report for Sonak Sports Specialists, Inc.</button>
                </div>
            </div>
            
            <div class="row">
                <div class="col-sm-6 mb-3">
                    <button id="btnGenerateOnitsukaWeeklySalesReportforSportsTownInc" data-role="none" style="height:100%;" class="btn btn-success btn-block">Generate Onitsuka Sales Report for Sports Town Inc</button>
                </div>
                <div class="col-sm-6 mb-3">
                    <button id="btnGenerateOnitsukaInventoryReportforSportsTownInc" data-role="none" style="height:100%;" class="btn btn-secondary btn-block">Generate Onitsuka Inventory Report Sports Town Inc</button>
                </div>
            </div>
        </div>
    </div>
</div>