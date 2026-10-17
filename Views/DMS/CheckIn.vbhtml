@Code
    ViewData("Title") = "Document Management System"
    Layout = "~/Views/Shared/DMSLayout.vbhtml"
End Code
@Section head
    <script>
        require(['/Scripts/sources/DMS/CheckIn.js?crc=']);
    </script>
End Section

<div class="container">
    <br />
    <div id="divBreadCrumbs" style="font-size:1.75rem; font-weight:500; overflow:auto;">
        Document Batch List
    </div>
    <br />
    <div id="pageListOfBatch" custom-data-role="page" style="display:block;">    
        <div class="row">
            <div class="col-sm-9">
                <input data-role="none" type="search" class="form-control" placeholder="Search Batch Number here" />
            </div>
            <div class="col-sm-3" style="text-align:right;">
                <button data-role="none" id="btnAddbatch" class="btn btn-primary">Create Document Batch</button>
            </div>
        </div>
        <br />
        <table class="table table-striped">
            <thead class="table-light">
                <tr>
                    <th>#</th>
                    <th>Batch Number</th>
                    <th>Document Category</th>
                    <th>Document Type</th>
                </tr>
            </thead>
            <tbody id="tbodyBatchDocuments">
                <tr>
                    <td colspan="4" style="text-align:center;">No results found</td>
                </tr>
            </tbody>
        </table>
    </div>
    <div id="pageCheckInDocs" custom-data-role="page" style="display:none;">
        <div class="mb-3 row">
            <label class="col-sm-2">Batch Number:</label>
            <div class="col-sm-4">
                <input data-role="none" type="text" id="txtCDBatchNumber" class="form-control" readonly>
            </div>
            <label class="col-sm-2">Office:</label>
            <div class="col-sm-4">
                <input data-role="none" type="text" id="txtCDOffice" class="form-control" readonly>
            </div>
        </div>
        <div class="mb-3 row">
            <label class="col-sm-2">Document Category:</label>
            <div class="col-sm-4">
                <input data-role="none" type="text" id="txtCDDocCategory" class="form-control" readonly>
            </div>
            <label class="col-sm-2">Document Type:</label>
            <div class="col-sm-4">
                <input data-role="none" type="text" id="txtCDDocType" class="form-control" readonly>
            </div>
        </div>
        <div class="mb-3 row">
            <label class="col-sm-2">Status:</label>
            <div class="col-sm-4">
                <input data-role="none" type="text" id="txtCDStatus" class="form-control" readonly>
            </div>
            <label class="col-sm-2">Document Count:</label>
            <div class="col-sm-4">
                <input data-role="none" type="text" id="txtCDDocCount" class="form-control" readonly>
            </div>
        </div>
        <div class="mb-3" style="text-align:right;">
            <input type="file" multiple id="fileCheckInDocs" style="display:none;" />
            <button data-role="none" id="btnChooseDocs" class="btn btn-primary">Check-In Document</button>
        </div>
        <table class="table table-striped">
            <thead class="table-light">
                <tr id="thCheckInDocs">
                    
                </tr>
            </thead>
            <tbody id="tbodyCheckInDocs"></tbody>
        </table>
        <br />
        <div style="text-align:center;">
            <button data-role="none" type="button" id="btnCancelAddedDocs" class="btn btn-secondary btn-lg" data-dismiss="modal">Cancel</button>
            <button data-role="none" type="button" id="btnSaveAddedDocs" class="btn btn-success btn-lg">Save</button>
        </div>
    </div>
</div>

<div class="modal fade" id="popupDocumentBatch" data-backdrop="static" data-keyboard="false" tabindex="-1" aria-labelledby="btnAddbatch" aria-hidden="true">
    <div class="modal-dialog modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="staticBackdropLabel">Document Batch</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body">
                <div id="divDocBatchErrorMsg" class="alert alert-danger fade show" role="alert" style="position:sticky; top:0; display:none">
                                     
                </div>
                <div class="mb-3 row">
                    <label for="txtBatchNumber" class="col-sm-3 col-form-label">Batch Number:</label>
                    <div class="col-sm-9">
                        <input data-role="none" type="text" id="txtBatchNumber" readonly class="form-control" value="1">
                    </div>
                </div>
                <div class="mb-3 row">
                    <label for="txtOffice" class="col-sm-3 col-form-label">Office:</label>
                    <div class="col-sm-9">
                        <select data-role="none" class="form-select" id="txtOffice">
                            <option value="">Please select</option>
                            <option value="IT Office">IT Office</option>
                            <option value="Accounting Office">Accounting Office</option>
                        </select>
                    </div>
                </div>
                <div class="mb-3 row">
                    <label for="txtBatchStatus" class="col-sm-3 col-form-label">Status:</label>
                    <div class="col-sm-9">
                        <input data-role="none" type="text" id="txtBatchStatus" readonly class="form-control" value="Open">
                    </div>
                </div>
                <div class="mb-3 row">
                    <label for="txtDocumentCount" class="col-sm-3 col-form-label">Document Count:</label>
                    <div class="col-sm-9">
                        <input data-role="none" type="text" id="txtDocumentCount" readonly class="form-control" value="0">
                    </div>
                </div>
                <div class="mb-3 row">
                    <label for="selDocumentCategory" class="col-sm-3 col-form-label">Document Category:</label>
                    <div class="col-sm-9">
                        <select data-role="none" id="selDocumentCategory" class="form-select">
                            <option value="">Please select Category</option>
                            <option value="Category 1">Category 1</option>
                            <option value="Category 2">Category 2</option>
                            <option value="Category 3">Category 3</option>
                        </select>
                    </div>
                </div>
                <div class="mb-3 row">
                    <label for="selDocumentType" class="col-sm-3 col-form-label">Document Type:</label>
                    <div class="col-sm-9">
                        <select data-role="none" id="selDocumentType" class="form-select">
                            <option value="">Please select Type</option>
                            <option value="Type 1">Type 1</option>
                            <option value="Type 2">Type 2</option>
                            <option value="Type 3">Type 3</option>
                        </select>
                    </div>
                </div>
                <div class="mb-3 row">
                    <label for="btnAddMetadata" class="col-sm-3 col-form-label">Metadata:</label>
                    <div class="col-sm-9" style="text-align:right;">
                        <button data-role="none" id="btnAddMetadata" class="btn btn-primary btn-sm">Add Metadata</button>
                    </div>                    
                </div>
                <div id="divDocMetadata"></div>
            </div>
            <div class="modal-footer">
                <button data-role="none" type="button" class="btn btn-light" data-dismiss="modal">Cancel</button>
                <button data-role="none" type="button" id="btnSaveBatchDocument" class="btn btn-success">Save</button>
            </div>
        </div>
    </div>
</div>

<div class="modal fade" id="popupCheckInDocs" data-backdrop="static" data-keyboard="false" tabindex="-1" aria-labelledby="btnAddbatch" aria-hidden="true">
    <div class="modal-dialog modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="staticBackdropLabel">Check-In Document</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    <label for="txtDocumentID" class="form-label">Document ID:</label>
                    <input data-role="none" type="text" class="form-control" id="txtDocumentID" readonly placeholder="Enter here">
                </div>
                <div id="divMetadataCheckInDoc"></div>
            </div>
            <div class="modal-footer">
                <button data-role="none" type="button" class="btn btn-light" data-dismiss="modal">Cancel</button>
                <button data-role="none" type="button" id="btnAddDocs" class="btn btn-success">Add</button>
            </div>
        </div>
    </div>
</div>