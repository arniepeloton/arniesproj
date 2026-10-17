<link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />
<link rel="stylesheet" href="/Content/jquery.dataTables2.css" />

<script>
    require(['bootstrapjs'], function () {
        require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
            require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/wow.js?crc=' + assetversion)], function () {
            });
        })
    })
</script>

<div>
    <div id="divwowreport" class="row row-cols-1 row-cols-sm-1 row-cols-md-2 row-cols-lg-3 row-cols-xl-3 row-cols-xxl-4">
        <div class="text-center w-100">
            <div class="spinner-border text-primary text-center" role="status">
                <span class="visually-hidden"></span>
            </div>
            <div>Preparing your report ...</div>
        </div>
    </div>
</div>