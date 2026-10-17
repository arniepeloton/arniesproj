@Code
    'ViewData("Title") = "Ayala Tenant Monitoring"
    'Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
End Section

<script>
    require([GetPathFormat('/Scripts/sources/MallLessorAyala/AyalaReport.js?crc=' + assetversion)]);
</script>



<div id="pageAyalaReport" data-role="page">
    <div class="row">
        <div class="col-sm-2 mb-3">
            Generate Report:
        </div>
        <div class="col-sm-10 mb-3">
            <select id="selAyalaReportRange" data-role="none" class="form-select">
                <option value="">Please Select</option>
                <option value="Today">Today</option>
                <option value="Yesterday">Yesterday</option>
                <option value="By Date">By Date</option>
            </select>
        </div>
    </div>

    <div id="divAyalaReportByDate" style="display:none;">
        <div class="row">
            <div class="col-sm-2 mb-3">
                Select Date:
            </div>
            <div class="col-sm-2 mb-3">
                <select id="selAyalaReportByDateMonth" data-role="none" class="form-select">
                    <option value="">MMM</option>
                </select>
            </div>
            <div class="col-sm-2 mb-3">
                <select id="selAyalaReportByDateDay" data-role="none" class="form-select">
                    <option value="">DD</option>
                </select>
            </div>
            <div class="col-sm-2 mb-3">
                <select id="selAyalaReportByDateYear" data-role="none" class="form-select">
                    <option value="">YYYY</option>
                </select>
            </div>
        </div>
    </div>

    <div class="mt-4 text-center">
        <span id="divReportLink" style="display:none;">
            <button id="lnkReportLink" data-role="none" class="btn btn-secondary btn-lg">Download Report File</button>
        </span>
        <button id="btnAyalaReportGenerate" data-role="none" class="btn btn-success btn-lg">Generate Report</button>        
    </div>
</div>