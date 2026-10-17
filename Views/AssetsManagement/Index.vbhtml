@Code
    ViewData("Title") = "Assets Management"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code
@Section head
<link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />
<link rel="stylesheet" href="/Content/jquery.dataTables2.css" />

<script>
    require(['bootstrapjs'], function () {
        require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
            require(['common'], function () {
                require(['promise!/Scripts/sources/ERP/AssetsManagement/main/controller.js?crc=' + assetversion], function () {
                    $('#divErpAssetsManagement').pageshow();
                })
            });            
        })
    })
</script>

End Section

<div id="divErpAssetsManagement">
    <div class="text-center divspinner">
        <div class="spinner-border text-primary mt-5 mb-3" role="status">
            <span class="visually-hidden"></span>
        </div>
        <br />
        Please wait ...
    </div>
</div>