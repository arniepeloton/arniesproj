@Code
    ViewData("Title") = "Deposits"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code
@Section head
    <link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
    <script>        
        require(['bootstrapjs'], function () {
            if (new URLSearchParams(location.search).get('IsPreview') == '1') {
                document.getElementById("parentdiv").style.display = 'none';
            } else {
                document.getElementById("parentdiv").removeAttribute('style')
            }

            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
                require(['common'], function () {
                    require(['datatables.net'], function () {
                        require(['promise!/Scripts/sources/ERP/Deposits/main/controller.js?v=' + assetversion], function () {
                            ERP.Deposits.main.controller = ERP.Deposits.main.controller({
                                //mode: 'disbursement'
                                dontLoadData: true
                            });
                            var pID = GetUrlParameters().ID;
                            if (CStr2(pID) != '') {
                                ERP.Deposits.main.controller.Work.LoadData(pID);
                            }
                        });
                    });                    
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
    <nav class="navbar navbar-dark bg-primary sticky-top text-light pl-3 pr-3 mb-3">
        <div style="white-space:nowrap; overflow:hidden; text-overflow:ellipsis;">
            <a href="javascript:history.back()" data-role="none" class="btn btn-primary">
                <span class="bi-chevron-left"></span>
            </a>
            <span>Deposits</span>
        </div>
    </nav>

    <div id="divErpDeposits" class="container-fluid mb-5">

    </div>
</div>