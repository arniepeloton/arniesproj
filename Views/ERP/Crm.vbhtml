@Code
    ViewData("Title") = "Support CRM"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code
@Section head
    <link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
    <script src="/Scripts/sources/CRMDashboard/customcalendar.js"></script>
    <script src="/Scripts/sources/crm/qrcodeNew.js"></script>
    <script>        
        require(['bootstrapjs'], function () {
            if (new URLSearchParams(location.search).get('IsPreview') == '1') {
                document.getElementById("parentdiv").style.display = 'none';
            } else {
                document.getElementById("parentdiv").removeAttribute('style')
            }

            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
                require(['common'], function () {
                    require([GetPathFormat('/Scripts/sources/CRMDashboard/db.js?crc=' + assetversion)]);
                })
            })
        })
    </script>
End Section

<style type="text/css">
    .dataTables_processing {
        /*Overwrite the Process Loading in datatables*/
        top: 0 !important;
        line-height: 50vh !important;
        height: 100% !important;
        margin-top: -20px !important;
    }

    div[custom-data-role="popup"] {
        display:none !important;
    }
</style>
<div id="parentdiv" style="display:none">
    <div class="mb-3">
        <nav class="navbar navbar-dark bg-primary sticky-top text-light pl-3 pr-3">
            <div style="white-space:nowrap; overflow:hidden; text-overflow:ellipsis;">
                <a href="javascript:history.back()" data-role="none" class="btn btn-primary">
                    <span class="bi-chevron-left"></span>
                </a>
                <span>Support CRM</span>
            </div>
        </nav>
        <div class="row p-1" style="margin:0px; width:100%; color:#FFF; background-color:#1969de; font-weight:bold;">
            <div class="BreadCrumbs">
                <div class="ItemsBreadCrumbs">&nbsp;</div>
            </div>
        </div>
    </div>

    <div id="divConsignee" class="container-fluid mb-5">
        <div subpage="maindb" id="pageLeadGen" style="display:block;">
            Loading, please wait ...
        </div>
    </div>
</div>