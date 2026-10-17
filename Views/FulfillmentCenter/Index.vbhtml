@Code
    ViewData("Title") = "Fulfillment Center"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code
@Section head
<link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />
<link rel="stylesheet" href="/Content/jquery.dataTables2.css" />

<script>
    require(['bootstrapjs'], function () {
        require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
            require(['common'], function () {
                require(['promise!/Scripts/sources/ERP/FulfillmentCenter/main/controller.js?v=' + assetversion]);
            })
        })
    })
</script>

End Section

<div id="divErpFulfillmentCenter">
    <div class="text-center divspinner">
        <div class="spinner-border text-primary mt-5 mb-3" role="status">
            <span class="visually-hidden"></span>
        </div>
        <br />
        Please wait ...
    </div>
</div>