@Code
    ViewData("Title") = "Contract And Letter Designer"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code
@Section head

<script>
    require(['bootstrapjs'], function () {
        require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
            require(['common'], function () {
                require(['promise!/Scripts/summernote.js?v=' + assetversion], function () {
                    require(['promise!/Scripts/sources/ERP/ContractDesigner/main/controller.js?v=' + assetversion]);
                })
            });            
        })
    })
</script>

    <style>
        .popover {
            display:none;
        }
    </style>

End Section

<div id="divErpContractDesigner">
    <div class="text-center">
        <div class="spinner-border text-primary mt-5 mb-3" role="status">
            <span class="visually-hidden"></span>
        </div>
        <br />
        Please wait ...
    </div>
</div>