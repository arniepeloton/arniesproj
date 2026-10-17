@Code
    ViewData("Title") = "Web POS Flush"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code

@Section head

    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
                require([GetPathFormat('/Scripts/sources/SignIn/WebPosFlush.js?crc=' + assetversion)]);
            })
        })        
    </script>

<style type="text/css">
    .ui-page-active, body {
        outline: none;
        background-color: #f8f9fa !important;
    }
</style>
        
End Section

<div class="container" custom-data-role="page" id="pageProgress">
    <div data-role="content">
        <div id="divProgressLog"></div>
    </div>
</div>