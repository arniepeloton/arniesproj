<script>
    require(['bootstrapjs'], function () {
        require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
            require(['/Scripts/sources/VendingMachine/Ads.js?crc=' + assetversion])
        })
    })
</script>

<div class="row wrapper min-vh-100 flex-column flex-md-row">
    <div class="col-12 col-md-2 p-0 bg-light flex-shrink-1 mb-3">
        <nav class="navbar navbar-expand-md navbar-light bg-light align-items-start flex-md-column flex-row p-2">
            <a data-role="none" class="navbar-brand" href="#">Menu</a>
            <a data-role="none" href="#" class="navbar-toggler" data-toggle="collapse" data-target=".sidebarPhysicalButtonsSettings">
                <span class="navbar-toggler-icon"></span>
            </a>
            <div class="collapse navbar-collapse sidebarPhysicalButtonsSettings mt-2">
                <ul class="flex-column w-100 justify-content-between nav nav-pills">
                    <li><a class="nav-link active" role="button" data-role="none" data-toggle="tab" href="#tabVDMAdsPortrait">Portrait Ads</a></li>
                    <li><a class="nav-link" role="button" data-role="none" data-toggle="tab" href="#tabVDMIdleAds">Idle Ads</a></li>
                    <li><a class="nav-link" role="button" data-role="none" data-toggle="tab" href="#tabVDMAdsFowst">Assigned Machine</a></li>
                </ul>
            </div>
        </nav>
    </div>

    <div class="col col-md bg-faded">
        <div class="tab-content">
            <input type="file" id="fileUploadAds" accept="image/*" multiple style="visibility:hidden; display:none;" data-role="none">

            <div class="row mb-3">
                <div class="col-md-2">Ads Name: <span class="required">*</span></div>
                <div class="col-md"><input data-role="none" type="text" class="form-control" id="txtVDMAdsName" /></div>
            </div>

            <div class="row mb-3">
                <div class="col-md-2">Description:</div>
                <div class="col-md"><input data-role="none" type="text" class="form-control" id="txtVDMAdsDescription" /></div>
            </div>

            <div class="tab-pane fade active show" id="tabVDMAdsPortrait">
                <div class="row align-items-center mb-3">
                    <div class="col text-right">
                        <button data-role="none" class="btn btn-success" id="btnVDMAdsUpload"><span class="bi-upload mr-2"></span>Upload Ads</button>
                    </div>
                </div>

                <div id="divVDMPortraitAds" class="row row-cols-2 row-cols-sm-3 row-cols-md-4 row-cols-lg-5">
                </div>
            </div>

            <div class="tab-pane fade" id="tabVDMIdleAds">
                <div class="row align-items-center mb-3">
                    <div class="col text-right">
                        <button data-role="none" class="btn btn-success" id="btnVDMIdleAdsUpload"><span class="bi-upload mr-2"></span>Upload Ads</button>
                    </div>
                </div>

                <div id="divVDMIdleAds" class="row row-cols-2 row-cols-sm-3 row-cols-md-4 row-cols-lg-5">
                </div>
            </div>

            <div class="tab-pane fade" id="tabVDMAdsFowst">
                <div class="row mb-3 align-items-center">
                    <div class="col-sm">
                        This template will be assigned to the selected machines listed below.
                    </div>
                    <div class="col-sm-auto">
                        <button data-role="none" class="btn btn-success" id="btnVDMAdsAddFOWST">Add Machine</button>
                    </div>
                </div>

                <div id="listVDMAdsFowst">

                </div>
            </div>
        </div>
    </div>
</div>

<div class="fixed-bottom text-center pb-3" style="display:block; left:50%; transform: translate(-50%, 0); right:auto">
    <button id="btnVDMSCancelAds" data-role="none" class="btn btn-secondary btn-lg">Cancel</button>
    <button id="btnVDMSaveAds" data-role="none" class="btn btn-success btn-lg">Save</button>
</div>

<div id="popAdsImagePreview" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered" style="max-width: none; width: auto; margin: 0 auto; display: flex; align-items: center; justify-content: center; min-height: 100vh;">
        <div class="modal-content" style="display:inline-block; max-width:none; width:auto;">
            <div class="modal-body text-center textvm-primary">
                <img data-role="none" />
            </div>
        </div>
    </div>
</div>