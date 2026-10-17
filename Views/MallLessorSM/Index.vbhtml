@Code
    'ViewData("Title") = "SM COIN"
    'Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
End Section

<script>
    require([GetPathFormat('/Scripts/sources/MallLessorSM/SMReport.js?crc=' + assetversion)]);
</script>



<div id="pageSMReport" data-role="page">
    <div class="row">
        <div class="col-sm-2 mb-3">
            Generate Report:
        </div>
        <div class="col-sm-10 mb-3">
            <select id="selSMReportRange" data-role="none" class="form-select">
                <option value="">Please Select</option>
                <option value="Today">Today</option>
                <option value="Yesterday">Yesterday</option>
                <option value="By Date">By Date</option>
            </select>
        </div>
    </div>

    <div id="divSMReportByDate" style="display:none;">
        <div class="row">
            <div class="col-sm-2 mb-3">
                Select Date:
            </div>
            <div class="col-sm-2 mb-3">
                <select id="selSMReportByDateMonth" data-role="none" class="form-select">
                    <option value="">MMM</option>
                </select>
            </div>
            <div class="col-sm-2 mb-3">
                <select id="selSMReportByDateDay" data-role="none" class="form-select">
                    <option value="">DD</option>
                </select>
            </div>
            <div class="col-sm-2 mb-3">
                <select id="selSMReportByDateYear" data-role="none" class="form-select">
                    <option value="">YYYY</option>
                </select>
            </div>
        </div>
    </div>

    <div class="mt-4 text-center">
        <span id="divSmReportLink" style="display:none;">
            <button id="lnkSmReportLink" data-role="none" class="btn btn-secondary btn-lg">Download Report File</button>
        </span>
        <button id="btnSMReportGenerate" data-role="none" class="btn btn-success btn-lg">Generate Report</button>
    </div>

    <hr />
    <div class="row mb-3">
        <form id="formImportMallLessorSummaryFile">
            <label class="col-sm-3 mb-3" for="fileTemplate">Import Mall Lessor Summary File:</label>
            <div class="col-sm-9">
                <input data-role="none" name="fileTemplate" type="file" class="form-control" accept=".mrt" />
            </div>
            <button type="submit" data-role="none" class="btn btn-success btn-lg">Import File</button>
        </form>
    </div>
</div>