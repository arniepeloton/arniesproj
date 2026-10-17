@Code
    ViewData("Title") = "Gate Pass Creation and Approval"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code
@Section head
<link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />
<link rel="stylesheet" href="/Content/jquery.dataTables2.css" />

<script>
    require(['bootstrapjs'], function () {
        require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
            require(['common'], function () {
                require(['promise!/Scripts/sources/ERP/GatePass/main/controller.js?crc=' + assetversion], function () {
                    $('#divGatePass').pageshow();
                })
            });            
        })
    })
</script>

End Section

<div id="divGatePass">
    <div class="text-center">
        <div class="spinner-border text-primary mt-5 mb-3" role="status">
            <span class="visually-hidden"></span>
        </div>
        <br />
        Please wait ...
    </div>    
</div>