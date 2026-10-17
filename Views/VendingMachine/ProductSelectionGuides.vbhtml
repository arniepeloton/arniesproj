<script>
    require(['bootstrapjs'], function () {
        require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
            require(['/Scripts/sources/VendingMachine/ProductSelectionGuides.js?crc=' + assetversion])
        })
    })
</script>

<div class="row wrapper min-vh-100 flex-column flex-md-row">
    <div class="col-12 col-md-2 p-0 bg-light flex-shrink-1 mb-3">
        <nav class="navbar navbar-expand-md navbar-light bg-light align-items-start flex-md-column flex-row p-2">
            <a data-role="none" class="navbar-brand" href="#">Menu</a>
            <a data-role="none" href="#" class="navbar-toggler" data-toggle="collapse" data-target=".sidebarProductSelectionGuides">
                <span class="navbar-toggler-icon"></span>
            </a>
            <div class="collapse navbar-collapse sidebarProductSelectionGuides mt-2">
                <ul class="flex-column w-100 justify-content-between nav nav-pills">
                    <li><a class="nav-link active" role="button" data-role="none" data-toggle="tab" href="#tabVDMProductSelectionGuideTemplate">Template</a></li>
                    <li><a class="nav-link" role="button" data-role="none" data-toggle="tab" href="#tabVDMProductSelectionGuideFowst">Assigned Machine</a></li>
                </ul>
            </div>
        </nav>
    </div>

    <div class="col col-md bg-faded">
        <div class="tab-content">
            <div class="tab-pane fade active show" id="tabVDMProductSelectionGuideTemplate">
                <div class="row mb-3">
                    <div class="col-md-2">Name: <span class="required">*</span></div>
                    <div class="col-md"><input data-role="none" type="text" class="form-control" id="txtVDMProductSelectionGuideTemplateName" /></div>
                </div>

                <div class="row mb-3">
                    <div class="col-md-2">Description:</div>
                    <div class="col-md"><input data-role="none" type="text" class="form-control" id="txtVDMProductSelectionGuideTemplateDescription" /></div>
                </div>

                @*<div class="row mb-3">
                    <div class="col-md-2">Model-B Template: <span class="required">*</span></div>
                    <div class="col-md">
                        <select id="selVDMModelBTemplate" data-role="none" class="form-select">
                            
                        </select>
                    </div>
                </div>*@

                <div class="row align-items-center mb-3">
                    <div class="col">
                        <i>Note:</i> Please make the steps short and concise.
                    </div>
                    <div class="col text-right">
                        <button data-role="none" class="btn btn-success" id="btnVDMAddStep">Add Step</button>
                    </div>                
                </div>

                <ul id="divVDMSteps" style="list-style-type:decimal;">

                </ul>

                <div class="d-none" id="dvStepTemplate">
                    <div class="row mb-3">
                        <div class="col-md-2">Description:</div>
                        <div class="col-md">
                            <textarea class="form-control txtStep" style="min-height:200px;"></textarea>
                        </div>
                        <div class="col-auto">
                            <button data-role="none" class="btn btn-light text-danger btndelete" title="Remove this step"><span class="bi bi-trash-fill"></span></button>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="col-md-2">Image:</div>
                        <div class="col-md-auto">
                            <input data-role="none" type="file" accept="image/png, image/gif, image/jpeg" class="form-control fileimg w-auto" />
                        </div>
                        <div class="col-md-auto">
                            <a data-role="none" class="link-primary"></a>
                        </div>
                        <div class="col-md-auto">
                            <a data-role="none" class="link-danger" title="Remove Image"></a>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="col-md-2">On-screen Time:</div>
                        <div class="col-md">
                            <select data-role="none" class="form-select w-auto selOnScreenTime">
                                <option value="">None</option>
                                <option value="5">5 secs</option>
                                <option value="10">10 secs</option>
                                <option value="15">15 secs</option>
                                <option value="20">20 secs</option>
                                <option value="25">25 secs</option>
                                <option value="30">30 secs</option>
                                <option value="45">45 secs</option>
                                <option value="60">60 secs</option>
                            </select>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="col-md-2">Display Dispensing Info:</div>
                        <div class="col-md">
                            <input data-role="none" type="checkbox" class="form-check-input chbIsDisplayInfo" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="tab-pane fade" id="tabVDMProductSelectionGuideFowst">
                <div class="row mb-3 align-items-center">
                    <div class="col-sm">
                        This template will be assigned to the selected machines listed below.
                    </div>
                    <div class="col-sm-auto">
                        <button data-role="none" class="btn btn-success" id="btnVDMAddFOWST">Add Machine</button>
                    </div>
                </div>

                <div id="listVDMProductSelectionGuideFowst">

                </div>
            </div>
        </div>
    </div>
</div>

<div class="fixed-bottom text-center pb-3" style="display:block; left:50%; transform: translate(-50%, 0); right:auto">
    <button id="btnVDMSCancelPage" data-role="none" class="btn btn-secondary btn-lg">Cancel</button>
    <button id="btnVDMSavePage" data-role="none" class="btn btn-success btn-lg">Save</button>
</div>

<div id="popImagePreview" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered" style="max-width: none; width: auto; margin: 0 auto; display: flex; align-items: center; justify-content: center; min-height: 100vh;">
        <div class="modal-content" style="display:inline-block; max-width:none; width:auto;">
            <div class="modal-body text-center textvm-primary">
                <img data-role="none" />
            </div>
        </div>
    </div>
</div>