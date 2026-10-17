@Code
    ViewData("Title") = "Purchasing"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code
@Section head
    <link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
                require(['common'], function () {
                    require(['datatables.net'], function () {
                        require([
                            '/Scripts/sources/ERP/Disbursement/dashboard/controller.js?crc=' + assetversion
                        ], function () {
                            require([
                            'promise!/Scripts/sources/ERP/OrderProcessing/main/controller.js?v=' + assetversion
                            ], function () {
                                var transtype, isAP;
                                if (new URLSearchParams(location.search).get('ShowAccountsPayable') == '1'){
                                    transtype = 'AccountsPayable';
                                    isAP = true;
                                } else {
                                    transtype = 'PurchaseOrder';
                                    isAP = false;
                                }

                                var mPurchaseOrderPageController = new ERP.OrderProcessing.work.controller({
                                    TransType: transtype,
                                    AccountsPayable: isAP,
                                });

                                mPurchaseOrderPageController.LoadTrans({
                                    ID: GetUrlParameters().ID,
                                    IncludeZeroQty: true
                                });

                                mPurchaseOrderPageController.ReadOnly();

                                $('#tabPurchasingWork').removeClass('tab-pane fade').removeAttr('custom-tab-role style');
                            })
                        })                        
                    })
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

<div id="divPurchasingMain" class="container-fluid mb-3 mt-3">
    <div class="tab-content"></div>
</div>