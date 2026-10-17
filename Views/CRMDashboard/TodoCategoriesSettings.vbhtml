<div id="divTodoCategoriesSettings">
    <div class="row mb-3">
        <div class="col text-right">
            <button data-role="none" name="btnAddTodoCategory" class="btn btn-success">Add Category</button>
        </div>
    </div>
    <div class="table-responsive">
        <table id="tblTodoCategories" data-role="none" class="table table-light table-striped table-hover">
            <thead>
                <tr class="align-top text-center" style="font-size:14px;">
                    <th style="width:50px">#</th>
                    <th>Category</th>
                    <th style="width:350px">Last Updated</th>
                    <th style="width:50px"></th>
                </tr>
            </thead>
        </table>
    </div>

    <div id="popupTodoCategory" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">To-Do Category</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label class="form-label">Category Name:</label>
                        <input data-role="none" type="text" name="txtTodoCategoryName" class="form-control" />
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col">
                            <button data-role="none" class="btn btn-success btn-block btn-lg" name="btnSaveTodoCategory">Save</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>