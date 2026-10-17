@Code
    ViewData("Title") = "Document Management System"
    Layout = "~/Views/Shared/DMSLayout.vbhtml"
End Code
@Section head
    <script>
        require([GetPathFormat('/Scripts/sources/DMS/VerifyRecords.js?crc=')]);
    </script>
End Section

<div class="container">
    <br />
    <h3>Verify Records</h3>
    <br />
    <div>
        <div class="mb-3 row">
            <label class="col-sm-2 col-form-label">Enter Batch Number:</label>
            <div class="col-sm-10">
                <form id="formVerifyRecords">
                    <div class="input-group mb-3">
                        <input data-role="none" type="text" class="form-control" aria-describedby="button-addon2">
                        <button data-role="none" class="btn btn-success" type="button" id="button-addon2">Submit</button>
                    </div>
                </form>                
            </div>
        </div>

        <div>
            <table class="table table-striped">
                <thead class="table-light">
                    <tr>
                        <th style="width:50px;">#</th>
                        <th style="width:150px;">Document ID</th>
                        <th style="width:200px;">Document Category</th>
                        <th style="width:150px;">Document Type</th>
                        <th style="width:200px; text-align:center;">Meta Data</th>
                        <th style="width:200px;"></th>
                    </tr>
                </thead>
                <tbody id="tbodyVerifyDocs">
                    <tr>
                        <td colspan="7" style="text-align:center;">Empty</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</div>

<div class="modal fade" id="popupViewDocs" data-backdrop="static" data-keyboard="false" tabindex="-1">
    <div class="modal-dialog modal-fullscreen">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="staticBackdropLabel">View Document ID: <b id="lblViewDocID"></b></h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body" style="overflow:hidden; padding:0;">
                <div class="row" style="height:100%;">
                    <div class="col-sm-9">
                        <iframe id="iframeViewDocument" style="width:100%; height:100%;"></iframe>
                    </div>
                    <div class="col-sm-3">
                        Metadata:
                        <br /><br />
                        <div id="metadata"></div>
                    </div>
                </div>                
            </div>
            <div class="modal-footer" style="display:block; text-align:center;">
                <button data-role="none" type="button" class="btn btn-light" data-dismiss="modal">Cancel</button>
                <button data-role="none" type="button" id="" class="btn btn-success">Tag as Verified</button>
                <button data-role="none" type="button" id="" class="btn btn-danger">Reject</button>
            </div>
        </div>
    </div>
</div>