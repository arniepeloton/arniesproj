<script>
    require([GetPathFormat('/Scripts/sources/Centers/Centers.js?v=' + assetversion)
        ]);
</script>

<div class="container-fluid">
    <div class="row mb-3">
        <label class="col-sm-2 mb-3" for="txtCentersName">Center's Name: <span class="required">*</span></label>
        <div class="col-sm-10">
            <input type="text" data-role="none" id="txtCentersName" class="form-control" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-2 mb-3" for="txtCentersDescription">Description:</label>
        <div class="col-sm-10">
            <input type="text" data-role="none" id="txtCentersDescription" class="form-control" />
        </div>
    </div>

    <div class="row mb-3">
        <label class="col-sm-2 mb-3" for="selCentersStatus">Description:</label>
        <div class="col-sm-10">
            <select id="selCentersStatus" data-role="none" class="form-select">
                <option value="1">Active</option>
                <option value="0">Inactive</option>
            </select>
        </div>
    </div>

    <div class="mt-4" style="text-align: center">
        <a id="btnCancelCenter" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
        <a id="btnSaveCenter" data-role="none" class="btn btn-success btn-lg">Save</a>
    </div>
</div>