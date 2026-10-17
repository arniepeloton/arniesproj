@Code
    'ViewData("Title") = "Ayala Tenant Monitoring"
    'Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
End Section

<script>
    require([GetPathFormat('/Scripts/sources/MallLessorMegaworld/MegaworldReport.js?crc=' + assetversion)]);
</script>



<div id="pageMegaworldReport" data-role="page">
    <div class="row">
        <div class="col-md-6 col-lg">
            <div class="row mb-3 align-items-center">
                <div class="col-auto mb-3 mb-md-0">
                    Mall:
                </div>
                <div class="col">
                    <select id="selMegaWorldMall" data-role="none" class="form-select"></select>
                </div>
            </div>
        </div>
        <div class="col-md-6 col-lg">
            <div class="row mb-3 align-items-center">
                <div class="col-auto mb-3 mb-md-0">
                    Store:
                </div>
                <div class="col">
                    <select id="selMegaworldFowst" data-role="none" class="form-select"></select>
                </div>
            </div>
        </div>
        <div class="col-md-6 col-lg" id="divPOSMachine" style="display:none;">
            <div class="row mb-3 align-items-center">
                <div class="col-auto mb-3 mb-md-0">
                    POS Machine:
                </div>
                <div class="col">
                    <select id="selMegaworldDevices" data-role="none" class="form-select"></select>
                </div>
            </div>
        </div>
        <div class="col-md-6 col-lg">
            <div class="row mb-3 align-items-center">
                <div class="col-auto mb-3 mb-md-0">
                    Date Covered:
                </div>
                <div class="col">
                    <div class="input-group">
                        <select id="selMegaworldMonth" data-role="none" class="form-select"></select>
                        <select id="selMegaworldYear" data-role="none" class="form-select"></select>
                    </div>
                </div>
            </div>            
        </div>
    </div>

    <div class="row mb-3 align-items-center">
        <div class="col-sm">
            Select the sales date for which you want to resend the text files, then click the 'Resend' button.
        </div>
        <div class="col-sm-auto text-right">
            <div class="btn-group">
                <button data-role="none" id="btnTextfilesRefresh" class="btn btn-secondary">Refresh</button>
                <button data-role="none" id="btnTextfilesResend" class="btn btn-success">Resend</button>
            </div>            
        </div>
    </div>

    <table id="tblMegaworldReport" data-role="none" class="table table-light table-hover table-striped">
        <thead>
            <tr class="text-center align-top" style="font-size:14px;">
                <th class="fixed-col" style="width:50px;"><input type="checkbox" data-role="none" class="form-check-input" title="Check/Uncheck All" /></th>
                <th class="fixed-col" style="width:50px;">#</th>
                <th class="fixed-col">Sales Date</th>
                <th class="fixed-col">Reported Date</th>
                <th class="fixed-col">Status</th>
                @*<th class="fixed-col" style="width:100px"></th>*@
            </tr>
        </thead>
        <tbody></tbody>
    </table>
</div>
