<script>
    require([GetPathFormat('/Scripts/sources/TableView/Settings.js?v=' + assetversion)]);
</script>

<div data-role="content">
    <div>
        <div id="popDiningArea" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Dining Area</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label class="form-label" for="txtDiningAreaName">Dining Area Name: <span style="color:#ff0000">*</span></label>
                            <input data-role="none" class="form-control" type="text" id="txtDiningAreaName" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtDiningNo">How many tables are in this dining area? <span style="color:#ff0000">*</span></label>
                            <input data-role="none" class="form-control" type="number" id="txtDiningNo" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtDiningPrefix">
                                Table Prefix Label:
                                <br />
                                <b>Optional:</b> You can add a prefix label applicable to this dining area to distinguish identical table numbers that belong to other dining areas.
                            </label>
                            <input data-role="none" class="form-control" type="text" id="txtDiningPrefix" />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnOkDiningArea" class="btn btn-success btn-block">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popTable" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Edit Table</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label class="form-label" for="txtTableName">Table Name:</label>
                            <input data-role="none" class="form-control" type="text" id="txtTableName" />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnOkTable" class="btn btn-success btn-block">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popNewTable" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">New Table</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label class="form-label" for="txtNewTableName">Table Name:</label>
                            <input data-role="none" class="form-control" type="text" id="txtNewTableName" />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnOkNewTable" class="btn btn-success btn-block">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>    

    <input data-role="none" type="text" id="txtTemplateID" style="display:none; visibility:hidden;" />

    <div class="row mb-3">
        <label class="col-sm-2 mb-3 col-form-label" for="txtTableViewTemplate">Table Name:</label>
        <div class="col-sm-10">
            <input data-role="none" class="form-control" type="text" id="txtTableViewTemplate" />
        </div>        
    </div>

    <div>
        <div class="row mb-3 align-items-center">
            <div class="col-9">
                <h3>Apply this template to selected location (F.O.W.S.T)</h3>
            </div>
            <div class="col-3 text-right">
                <button id="btnAddTableFowst" data-role="none" class="btn btn-success">
                    Add FOWST
                </button>
            </div>
        </div>

        <div data-role="none" id="lstTableFowst" class="list-group mb-3"></div>
    </div>

    <div>
        <div class="row mb-3 mt-4 align-items-center">
            <div class="col-9">
                <h3>Dining Area</h3>
            </div>
            <div class="col-3 text-right">
                <button id="btnAddDiningArea" data-role="none" class="btn btn-success">
                    Add Dining Area
                </button>
            </div>
        </div>

        <div class="mb-3">
            <span style="color:#ff0000">*</span><b>Note:</b>
            You can use your cursor to drag and drop lists into the desired order.
        </div>

        <ul data-role="none" id="lstDiningArea" class="list-group mb-3"></ul>
    </div>

    <div class="mt-4 text-center">
        <a id="btnDiningCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
        <a id="btnDiningRemove" data-role="none" class="btn btn-danger btn-lg" style="display:none;">Remove Template</a>
        <a id="btnDiningSave" data-role="none" class="btn btn-success btn-lg">Save</a>
    </div>
</div>