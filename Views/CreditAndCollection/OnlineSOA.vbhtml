@Code
    ViewData("Title") = "Online SOA"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code
@Section head
    <link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
                require(['common'], function () {
                    require(['/Scripts/sources/CreditAndCollection/OnlineSOA.js?v=' + assetversion])
                });
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

    .right{
        float:right;
        width: 240px;
    }

    .left{
        overflow:hidden; 
        float:none;
    }

    .fixed-header-width > th {
        white-space:nowrap;
    }

    /* copied from \Views\CreditAndCollection\Index.vbhtml */
    .divLeft {
        float:none;
        width:auto;
        overflow:auto;
        padding:3px;
    }

    /* copied from \Views\CreditAndCollection\Index.vbhtml */
    .divRight{
        width:250px;
        float:right;
        padding:3px;
    }
</style>

<div id="divloading" custom-data-role="page" class="mt-5 text-center" style="display:block;">
    <div class="spinner-border text-primary" role="status" style="width: 3rem; height: 3rem;">
        <span class="visually-hidden"></span>
    </div>
</div>

<div id="divmain" custom-data-role="page" style="display:none;">
    <div class="alert alert-info m-3" role="alert">
        <span>For your security, you can only access your SOA from <b id="lblSOALinkDateFrom"></b> to <b id="lblSOALinkDateTo"></b> </span>
    </div>

    <div id="divMAAccountsPayable" class="container-fluid">

    </div>
</div>