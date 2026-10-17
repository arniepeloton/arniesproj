@Code
    ViewData("Title") = "Inventory Handling"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code
@Section head
    <link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />

    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
                require(['common'], function () {
                    AjaxSend({
                        ExactUrl: '/Scripts/sources/ERP/InventoryHandling/main.html?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&proj=eas&pid=' + sessionStorage.PeopleID + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            $('#divInventoryHandling').empty().append(vData);
                        }
                    });
                });
            })
        })
    </script>

End Section

<div id="divInventoryHandling">
    <div class="text-center">
        <div class="spinner-border text-primary mt-5 mb-3" role="status">
            <span class="visually-hidden"></span>
        </div>
        <br />
        Please wait ...
    </div>
</div>