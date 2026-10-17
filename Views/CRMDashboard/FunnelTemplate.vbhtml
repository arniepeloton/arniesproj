<script>
    require([GetPathFormat('/Scripts/sources/CRMDashboard/FunnelTemplate.js?crc=' + assetversion)]);
</script>

<div id="popNewStage" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-lg modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Stage Name</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body" id="divKbFilter">
                <textarea data-role="none" class="form-control" id="txtStageDescription" style="max-height:100px; height:150px; resize:none"></textarea>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col-md order-2 order-md-0">
                        <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                    </div>
                    <div class="col-md order-1 order-md-1">
                        <button data-role="none" type="button" id="btnOkNewStage" class="btn btn-success btn-block btn-lg">Ok</button>
                    </div>
                    <div class="col-md order-0 order-md-2">
                        <button data-role="none" type="button" id="btnOkAddNewStage" class="btn btn-success btn-block btn-lg">Ok and Add New</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popNewChildStage" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-lg modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="txtTitleSubList">Stage Name</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body" id="divKbFilter">
                <textarea data-role="none" class="form-control" id="txtChildStageDescription" style="max-height:100px; height:150px; resize:none"></textarea>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col-md order-2 order-md-0">
                        <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                    </div>
                    <div class="col-md order-1 order-md-1">
                        <button data-role="none" type="button" id="btnOkNewChildStage" class="btn btn-success btn-block btn-lg">Ok</button>
                    </div>
                    <div class="col-md order-0 order-md-2">
                        <button data-role="none" type="button" id="btnOkAddChildNewStage" class="btn btn-success btn-block btn-lg">Ok and Add New</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="divLoadingFunnelTemplate" subpage="pageFunnelTemplate" style="display:block;"><div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div></div></div>

<div id="divMainFunnelTemplatePage" subpage="pageFunnelTemplate" style="display:none;">
    <div data-role="content">
        <input data-role="none" type="text" id="txtFunnelTemplateID" style="display:none;" />

        <div class="mb-3 row align-items-center">
            <label class="col-md-2" for="butActivityDateRange">Template Name: <span class="required">*</span></label>
            <div class="col-md">
                <input data-role="none" type="text" class="form-control" id="txtFunnelTemplateName" />
            </div>
        </div>

        <div class="mb-3 row align-items-center">
            <div class="col-md-auto">
                <b><span class="required">*</span> NOTE:</b> You can use your cursor to drag and drop lists into the desired order.
            </div>
            <div class="col-md text-right">
                <button data-role="none" class="btn btn-success" id="btnNewStage">Add New Stage</button>
            </div>
        </div>

        <ol id="listFunnelTemplate" data-role="none" style=" margin:0 auto;" class="draggableList"></ol>

        <div class="mt-5 mb-5 text-center">
            <button data-role="none" class="btn btn-secondary btn-lg" id="btnCancelFunnelTemplate">Cancel</button>
            <button data-role="none" class="btn btn-danger btn-lg" id="btnDeleteFunnelTemplate">Delete Template</button>
            <button data-role="none" class="btn btn-success btn-lg" id="btnSaveFunnelTemplate">Save</button>
        </div>
    </div>    
</div>