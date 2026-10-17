<script>
    require(['bootstrapjs'], function () {
        require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
            require(['/Scripts/sources/VendingMachine/GeneralSettings.js?v=' + assetversion])
        })
    })
</script>

<div class="row wrapper min-vh-100 flex-column flex-md-row">
    <div class="col-12 col-md-2 p-0 bg-light flex-shrink-1 mb-3">
        <nav class="navbar navbar-expand-md navbar-light bg-light align-items-start flex-md-column flex-row p-2">
            <a data-role="none" class="navbar-brand" href="#">Menu</a>
            <a data-role="none" href="#" class="navbar-toggler" data-toggle="collapse" data-target=".sidebarVDMGenSettings">
                <span class="navbar-toggler-icon"></span>
            </a>
            <div class="collapse navbar-collapse sidebarVDMGenSettings mt-2">
                <ul class="flex-column w-100 justify-content-between nav nav-pills">
                    <li><a class="nav-link active" role="button" data-role="none" data-toggle="tab" href="#tabVDMPSGuides">Advertisements</a></li>
                    <li><a class="nav-link" role="button" data-role="none" data-toggle="tab" href="#tabVDMDefaultCoins">Default Coins</a></li>
                </ul>
            </div>
        </nav>
    </div>

    <div class="col col-md bg-faded">
        <div class="tab-content">
            <div class="tab-pane fade active show" id="tabVDMPSGuides">
                <div style="white-space:nowrap; overflow:auto;">
                    <ul class="nav nav-tabs" role="tablist" style="flex-wrap:nowrap;">
                        <li class="nav-item" role="presentation">
                            <button data-role="none" class="nav-link active" data-toggle="tab" data-target="#tabVDMPSGuideMenuPage">Menu Page</button>
                        </li>
                        <li class="nav-item" role="presentation">
                            <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabVDMPSGuideProductDetailsPage">Product Details Page</button>
                        </li>
                        <li class="nav-item" role="presentation">
                            <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabVDMPSGuideCartPage">Cart Page</button>
                        </li>
                        <li class="nav-item" role="presentation">
                            <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabVDMPSGuidePaymentPage">Payment Page</button>
                        </li>
                    </ul>
                </div>

                <div class="tab-content">
                    <div id="tabVDMPSGuideMenuPage" class="tab-pane fade pt-3 active show">
                        <div class="row align-items-center mb-3">
                            <div class="col text-right">
                                <button data-role="none" class="btn btn-light border" id="btnVDMMenuPageExistingFiles"><span class="bi-collection mr-2"></span> Insert from existing media</button>
                                <button data-role="none" class="btn btn-success" id="btnVDMMenuPageUpload"><span class="bi-upload mr-2"></span> Add Ads</button>
                            </div>
                        </div>

                        <div id="divMenuPageGuides" class="row g-0 row-cols-2 row-cols-sm-3 row-cols-md-4 row-cols-lg-5 divPSGuidesContainer">
                        </div>
                    </div>

                    <div id="tabVDMPSGuideProductDetailsPage" class="tab-pane fade pt-3">
                        <div class="row align-items-center mb-3">
                            <div class="col text-right">
                                <button data-role="none" class="btn btn-light border" id="btnVDMProductDetailsExistingFiles"><span class="bi-collection mr-2"></span> Insert from existing media</button>
                                <button data-role="none" class="btn btn-success" id="btnVDMProductDetailsPageUpload"><span class="bi-upload mr-2"></span> Add Ads</button>
                            </div>
                        </div>

                        <div id="divProductDetailsPageGuides" class="row g-0 row-cols-2 row-cols-sm-3 row-cols-md-4 row-cols-lg-5 divPSGuidesContainer">
                        </div>
                    </div>

                    <div id="tabVDMPSGuideCartPage" class="tab-pane fade pt-3">
                        <div class="row align-items-center mb-3">
                            <div class="col text-right">
                                <button data-role="none" class="btn btn-light border" id="btnVDMCartPageExistingFiles"><span class="bi-collection mr-2"></span> Insert from existing media</button>
                                <button data-role="none" class="btn btn-success" id="btnVDMCartPageUpload"><span class="bi-upload mr-2"></span> Add Ads</button>
                            </div>
                        </div>

                        <div id="divCartPageGuides" class="row g-0 row-cols-2 row-cols-sm-3 row-cols-md-4 row-cols-lg-5 divPSGuidesContainer">
                        </div>
                    </div>

                    <div id="tabVDMPSGuidePaymentPage" class="tab-pane fade pt-3">
                        <div class="row align-items-center mb-3">
                            <div class="col text-right">
                                <button data-role="none" class="btn btn-light border" id="btnVDMPaymentPageExistingFiles"><span class="bi-collection mr-2"></span> Insert from existing media</button>
                                <button data-role="none" class="btn btn-success" id="btnVDMPaymentPageUpload"><span class="bi-upload mr-2"></span> Add Ads</button>
                            </div>
                        </div>

                        <div id="divPaymentPageGuides" class="row g-0 row-cols-2 row-cols-sm-3 row-cols-md-4 row-cols-lg-5 divPSGuidesContainer">
                        </div>
                    </div>

                    <div class="fixed-bottom text-center pb-4">
                        <button data-role="none" class="btn btn-secondary btn-lg btnCancelGeneralSettings">Cancel</button>
                        <button data-role="none" id="btnSaveGeneralSettings" class="btn btn-success btn-lg">Save</button>
                    </div>
                </div>
            </div>

            <div class="tab-pane fade" id="tabVDMDefaultCoins">
                <h3 class="mb-3">Assign a default SKU to each denomination</h3>

                <div class="row mb-3">
                    <div class="col-2">
                        For 25 cents:
                    </div>
                    <div class="col-3">
                        <input data-role="none" class="form-control txtdenom denom_25" placeholder="Click to select SKU" readonly />
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-2">
                        For 1 Peso:
                    </div>
                    <div class="col-3">
                        <input data-role="none" class="form-control txtdenom denom_1" placeholder="Click to select SKU" readonly />
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-2">
                        For 5 Pesos:
                    </div>
                    <div class="col-3">
                        <input data-role="none" class="form-control txtdenom denom_5" placeholder="Click to select SKU" readonly />
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-2">
                        For 10 Pesos:
                    </div>
                    <div class="col-3">
                        <input data-role="none" class="form-control txtdenom denom_10" placeholder="Click to select SKU" readonly />
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-2">
                        For 20 Pesos:
                    </div>
                    <div class="col-3">
                        <input data-role="none" class="form-control txtdenom denom_20" placeholder="Click to select SKU" readonly />
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-2">
                        For 50 Pesos:
                    </div>
                    <div class="col-3">
                        <input data-role="none" class="form-control txtdenom denom_50" placeholder="Click to select SKU" readonly />
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-2">
                        For 100 Pesos:
                    </div>
                    <div class="col-3">
                        <input data-role="none" class="form-control txtdenom denom_100" placeholder="Click to select SKU" readonly />
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-2">
                        For 200 Pesos:
                    </div>
                    <div class="col-3">
                        <input data-role="none" class="form-control txtdenom denom_200" placeholder="Click to select SKU" readonly />
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-2">
                        For 500 Pesos:
                    </div>
                    <div class="col-3">
                        <input data-role="none" class="form-control txtdenom denom_500" placeholder="Click to select SKU" readonly />
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-2">
                        For 1000 Pesos:
                    </div>
                    <div class="col-3">
                        <input data-role="none" class="form-control txtdenom denom_1000" placeholder="Click to select SKU" readonly />
                    </div>
                </div>

                <div class="text-center pb-4">
                    <button data-role="none" class="btn btn-secondary btn-lg btnCancelGeneralSettings">Cancel</button>
                    <button data-role="none" id="btnSaveDefaultCoins" class="btn btn-success btn-lg">Save</button>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popVDMAds" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Advertisement</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    <label class="form-label">File: <span class="required">*</span></label>
                    <input type="file" id="fileInputVDMPSGuides" class="form-control" accept="image/*,video/*">
                </div>

                <div class="mb-3">
                    <label class="form-label">File Name: <span class="required">*</span></label>
                    <input data-role="none" class="form-control" id="txtAdFileName" />
                </div>

                <div class="mb-3">
                    <label class="form-label">Description:</label>
                    <textarea data-role="none" class="form-control" id="txtAdFileDescription" style="resize:none; height:200px;" placeholder="Optional"></textarea>
                </div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col">
                        <button data-role="none" type="button" class="btn btn-light btn-lg border btn-block" data-dismiss="modal">Cancel</button>
                    </div>
                    <div class="col">
                        <button data-role="none" type="button" id="butAddAds" class="btn btn-success btn-lg btn-block">Add</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>