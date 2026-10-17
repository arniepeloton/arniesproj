<script>
    require([GetPathFormat('/Scripts/sources/CRMDashboard/ClientStage.js?crc=' + assetversion)]);
</script>

<div id="divLoadingClientStage" subpage="pageClientStage" style="display:block; text-align:center;">Loading, please wait ...</div>

<div id="divMainClientStagePage" subpage="pageClientStage" style="display:none;">
    <div id="popStage" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Stage</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body" id="divKbFilter">
                    <input data-role="none" type="text" style="display:none" id="txtStageID" />

                    <div class="mb-3">
                        <label for="txtStageName" class="form-label">Stage Name:<span style="color:#ff0000">*</span></label>
                        <input data-role="none" type="text" id="txtStageName" class="form-control" />
                    </div>

                    <div class="mb-3">
                        <label for="txtStageDescription" class="form-label">Description:</label>
                        <textarea data-role="none" id="txtStageDescription" class="form-control" style="height:150px; resize:none;"></textarea>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-md order-1 order-md-0">
                            <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col-md order-1 order-md-0">
                            <button data-role="none" type="button" id="btnOkAddStage" class="btn btn-success btn-block btn-lg">Ok</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <input data-role="none" type="text" style="display:none" id="txtGroupStageID" />
    <div class="mb-3 row align-items-center">
        <label class="col-md-2" for="txtGroupStageName">Group Stage Name: <span style="color:#ff0000">*</span></label>
        <div class="col-md">
            <input data-role="none" type="text" class="form-control" id="txtGroupStageName" />
        </div>        
    </div>

    <div>
        <div class="mb-3 row align-items-center">
            <div class="col">
                <h4>Stages</h4>
            </div>
            <div class="col text-right">
                <button data-role="none" class="btn btn-success" id="btnAddNewStage">Add New Stage</button>
            </div>
        </div>

        <ul id="listStages" data-role="none" class="list-group">
            @*<li>
                <a>
                    Nothing to show
                    <br /><br />
                    <p>asdasdasdasd</p>
                </a>
                <a data-iconpos="notext" data-icon="delete">Delete</a>
            </li>*@
        </ul>
    </div>
    
    <div class="mt-5 mb-5 text-center">
        <button data-role="none" class="btn btn-secondary btn-lg" id="btnCancelStage">Cancel</button>
        <span id="divRemoveStage" style="display:none;"><button data-role="none" class="btn btn-danger btn-lg" id="btnRemoveStage">Remove</button></span>
        <button data-role="none" class="btn btn-success btn-lg" id="btnSaveStage">Save</button>
    </div>    
</div>