@Code
    ViewData("Title") = "Document Management System"
    Layout = "~/Views/Shared/DMSLayout.vbhtml"
End Code
@Section head
    <script>
        require([GetPathFormat('/Scripts/sources/DMS/Settings.js?crc=')]);
    </script>
End Section

<div class="container">
    <br />
    <h3>Settings</h3>
    <br />
    <div>
        <nav>
            <div class="nav nav-tabs" id="nav-tab" role="tablist">
                <a class="nav-link active" id="" data-toggle="tab" href="#nav-home">Metadata</a>
                <a class="nav-link" id="" data-toggle="tab" href="#nav-profile">Document Categories</a>
                <a class="nav-link" id="" data-toggle="tab" href="#nav-contact">Document Types</a>
                <a class="nav-link" id="" data-toggle="tab" href="#nav-archive">Archive Locations</a>
            </div>
        </nav>
        <br />
        <div class="tab-content" id="nav-tabContent">
            <div class="tab-pane fade show active" id="nav-home">
                <div class="mb-3" style="text-align:right;">
                    <button data-role="none" id="btnAddMetadata" class="btn btn-primary">Add Metadata Label</button>
                </div>
                <table class="table table-striped">
                    <thead class="table-light">
                        <tr>
                            <th>#</th>
                            <th>Metadata Label</th>
                            <th>Last Updated</th>
                            <th>Updated By</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td colspan="5" style="text-align:center;">No results found</td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="tab-pane fade" id="nav-profile">
                <div class="mb-3" style="text-align:right;">
                    <button data-role="none" id="btnAddDocumentCategory" class="btn btn-primary">Add Document Category</button>
                </div>
                <table class="table table-striped">
                    <thead class="table-light">
                        <tr>
                            <th>#</th>
                            <th>Document Category</th>
                            <th>Last Updated</th>
                            <th>Updated By</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td colspan="5" style="text-align:center;">No results found</td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="tab-pane fade" id="nav-contact">
                <div class="mb-3" style="text-align:right;">
                    <button data-role="none" id="btnAddDocumentType" class="btn btn-primary">Add Document Type</button>
                </div>
                <table class="table table-striped">
                    <thead class="table-light">
                        <tr>
                            <th>#</th>
                            <th>Document Type</th>
                            <th>Last Updated</th>
                            <th>Updated By</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td colspan="5" style="text-align:center;">No results found</td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="tab-pane fade" id="nav-archive">
                <div class="mb-3" style="text-align:right;">
                    <button data-role="none" id="btnAddArchiveLocation" class="btn btn-primary">Add Archive Location</button>
                </div>
                <table class="table table-striped">
                    <thead class="table-light">
                        <tr>
                            <th>#</th>
                            <th>Location Name</th>
                            <th>Floor</th>
                            <th>Cabinet</th>
                            <th>Drawer</th>
                            <th>Shelf</th>
                            <th>Last Updated</th>
                            <th>Updated By</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td colspan="9" style="text-align:center;">No results found</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</div>

<div class="modal" id="popupMetadata" tabindex="-1">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Metadata</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    <label for="" class="form-label">Metadata Label:</label>
                    <input data-role="none" type="text" class="form-control" id="" placeholder="Enter here">
                </div>
                <div class="mb-3 row">
                    <label class="col-sm-4">Active Metadata?</label>
                    <div class="col-sm-8">
                        <div class="form-check form-switch">
                            <input class="form-check-input" type="checkbox" id="chbMetadataActive" checked>
                            <label class="form-check-label" for="chbMetadataActive">Yes</label>
                        </div>
                    </div>
                </div>                
            </div>
            <div class="modal-footer">
                <button data-role="none" type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                <button data-role="none" type="button" class="btn btn-primary">Save</button>
            </div>
        </div>
    </div>
</div>

<div class="modal" id="popupDocumentCategory" tabindex="-1">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Document Category</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    <label for="" class="form-label">Category Name:</label>
                    <input data-role="none" type="text" class="form-control" id="" placeholder="Enter here">
                </div>
                <div class="mb-3 row">
                    <label class="col-sm-4">Active Category?</label>
                    <div class="col-sm-8">
                        <div class="form-check form-switch">
                            <input class="form-check-input" type="checkbox" id="chbCategoryActive" checked>
                            <label class="form-check-label" for="chbCategoryActive">Yes</label>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button data-role="none" type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                <button data-role="none" type="button" class="btn btn-primary">Save</button>
            </div>
        </div>
    </div>
</div>

<div class="modal" id="popupDocumentType" tabindex="-1">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Document Type</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    <label for="" class="form-label">Type Name:</label>
                    <input data-role="none" type="text" class="form-control" id="" placeholder="Enter here">
                </div>
                <div class="mb-3 row">
                    <label class="col-sm-4">Active Type?</label>
                    <div class="col-sm-8">
                        <div class="form-check form-switch">
                            <input class="form-check-input" type="checkbox" id="chbTypeActive" checked>
                            <label class="form-check-label" for="chbTypeActive">Yes</label>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button data-role="none" type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                <button data-role="none" type="button" class="btn btn-primary">Save</button>
            </div>
        </div>
    </div>
</div>

<div class="modal" id="popupArchiveLocation" tabindex="-1">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Archive Location</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    <label for="" class="form-label">Location Name:</label>
                    <input data-role="none" type="text" class="form-control" id="" placeholder="Enter here">
                </div>
                <div class="mb-3">
                    <label for="" class="form-label">Floor:</label>
                    <input data-role="none" type="text" class="form-control" id="" placeholder="Enter here">
                </div>
                <div class="mb-3">
                    <label for="" class="form-label">Cabinet:</label>
                    <input data-role="none" type="text" class="form-control" id="" placeholder="Enter here">
                </div>
                <div class="mb-3">
                    <label for="" class="form-label">Drawer:</label>
                    <input data-role="none" type="text" class="form-control" id="" placeholder="Enter here">
                </div>
                <div class="mb-3">
                    <label for="" class="form-label">Shelf:</label>
                    <input data-role="none" type="text" class="form-control" id="" placeholder="Enter here">
                </div>
            </div>
            <div class="modal-footer">
                <button data-role="none" type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                <button data-role="none" type="button" class="btn btn-primary">Save</button>
            </div>
        </div>
    </div>
</div>