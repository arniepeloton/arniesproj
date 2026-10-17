<script>
    require([GetPathFormat('/Scripts3/ProductGroup.js?crc=' + assetversion),
        GetPathFormat('/Scripts/jquery-ui-sortable.js')
    ]);
</script>
@*<script src="/Scripts/bootstrap.min.js?crc="></script>*@
<style type="text/css">
    .hbList li > div{
        border-left: 5px solid #fff;
    }
    .hbList li > div:hover{
        background-color: #f2f2f2;
        border-left: 5px solid #808080;
    }
</style>
<ul class="nav nav-tabs" id="tabHB" role="tablist">
    <li class="nav-item">
        <a class="nav-link active" data-toggle="tab" id="tbHBTempate" href="#tabContentHBMenu">Hot Button Template</a>
    </li>
    <li class="nav-item">
        <a class="nav-link" data-toggle="tab" id="tabHBFowst" href="#tabContenHBFowst">Assigned FOWST Locations</a>
    </li>
</ul>

<div class="tab-content">
    <div id="tabContentHBMenu" class="tab-pane fade show active pt-3">
        <div class="row mb-3">
            <div class="col">
                <p>Use this tool to create hot buttons. Tap on "Add Button" to add a new group or specific SKU. You can use your cursor to drag and drop lists into the desired order.</p>
            </div>
            <div class="col-auto">
                <div class="divVDMModel" style="display:none">
                    <div class="form-check">
                        <label class="form-check-label" for="chbIsDisplayAll">Display All Products in POS</label>
                        <input data-role="none" type="checkbox" class="form-check-input" id="chbIsDisplayAll" />
                    </div>
                </div>                
            </div>
        </div>        

        <div class="row mb-3">
            <div class="col-6">
                <h2 id="hdrTemplateName"></h2>
            </div>
            <div class="col-6" style="text-align:right;">
                <div class="btn-group">
                    <button data-role="none" class="btn btn-secondary dropdown-toggle" data-toggle="dropdown" aria-expanded="false">
                        Template Options
                    </button>
                    <ul class="dropdown-menu dropdown-menu-right shadow">
                        <li><button class="dropdown-item" data-role="none" id="btnGenerateHB"><span class="bi-arrow-clockwise text-success"></span> Generate H.B</button></li>
                        <li><button class="dropdown-item" data-role="none" id="btnEditName"><span class="bi-pencil text-warning"></span> Edit Name</button></li>
                        <li><button class="dropdown-item" data-role="none" id="btnDeleteTemplate"><span class="bi-trash text-danger"></span> Delete Template</button></li>
                        <li><button class="dropdown-item" data-role="none" id="btnDuplicateTemplate"><span class="bi-copy text-primary"></span> Duplicate Template</button></li>
                    </ul>
                </div>
            </div>
        </div>

        <div class="divVDMModel" style="display:none;">
            <div class="row mb-3 align-items-center">
                <div class="col-sm-auto">
                    VDM Model for this template:
                </div>
                <div class="col-sm">
                    <select data-role="none" class="form-select fixed" id="selVDMModel"></select>
                </div>
            </div>
        </div>

        <div>
            @*<div>
            <a id="btnAddHotButtonfake" data-icon="plus" data-role="button" data-inline="true" style="float:right; margin-top:-15px;">Options</a>
        </div>*@

            <div id="popEditTemplateName" class="modal fade" tabindex="-1">
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Edit Template Name</h5>
                            <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        </div>
                        <div class="modal-body">
                            <div class="mb-3">
                                <label for="txtEditName" class="form-label">Template Name:</label>
                                <input type="text" id="txtEditName" data-role="none" class="form-control" />
                            </div>
                        </div>
                        <div class="modal-footer">
                            <div class="row">
                                <div class="col">
                                    <button data-role="none" type="button" class="btn btn-secondary btn-lg btn-block" data-dismiss="modal">Cancel</button>
                                </div>
                                <div class="col">
                                    <button data-role="none" type="button" id="btnSaveName" class="btn btn-success btn-lg btn-block">Save</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="popNewAddGroup" class="modal fade" tabindex="-1">
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Group Name</h5>
                            <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        </div>
                        <div class="modal-body">
                            <div class="mb-3">
                                <label for="txtGroupName" class="form-label">Group Name: (maximum of 23 characters only)</label>
                                <input type="text" id="txtGroupName" data-role="none" class="form-control mb-3" placeholder="Enter group name" maxlength="23" />
                            </div>

                            <div class="mb-3">
                                <label for="selDefaultOPT" class="form-label">Default Other Payment Type:</label>
                                <select data-role="none" class="form-select" id="selDefaultOPT"></select>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <div class="row">
                                <div class="col">
                                    <button data-role="none" type="button" class="btn btn-secondary btn-lg btn-block" data-dismiss="modal">Cancel</button>
                                </div>
                                <div class="col">
                                    <button data-role="none" type="button" id="btnOkGroupName" class="btn btn-success btn-lg btn-block">Ok</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="popRenameHB" class="modal fade" tabindex="-1">
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Edit Hot Button</h5>
                            <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        </div>
                        <div class="modal-body">
                            <div class="mb-3">Please enter new name for <b id="txtCurrentHBName"></b></div>
                            <div class="mb-3">
                                <label id="lblRenameNote" for="txtRenameHB" class="form-label"></label>
                                <input type="text" data-role="none" class="form-control mb-3" id="txtRenameHB" placeholder="Enter new name" />
                            </div>

                            <div class="mb-3" style="display:none;">
                                <label for="selChangeDefaultOPT" class="form-label">Default Other Payment Type:</label>
                                <select data-role="none" class="form-select" id="selChangeDefaultOPT"></select>
                            </div>

                            <div class="divVDMModel" style="display:none;">
                                <div class="form-check form-switch mb-3">
                                    <label for="chbUseUOM" class="form-check-label">Check this if you want the user to select from its Units of Measure</label>
                                    <input class="form-check-input" type="checkbox" id="chbUseUOM">
                                </div>

                                <div class="ml-5" id="divSellingPreference" style="display:none;">
                                    Sold this SKU by:
                                    <div class="form-check mb-3">
                                        <label for="radSellingPreferenceWeight" class="form-check-label">Weight</label>
                                        <input class="form-check-input" type="radio" name="radSellingPreference" value="Weight" id="radSellingPreferenceWeight">
                                    </div>
                                    <div class="form-check mb-3">
                                        <label for="radSellingPreferenceVolume" class="form-check-label">Volume</label>
                                        <input class="form-check-input" type="radio" name="radSellingPreference" value="Volume" id="radSellingPreferenceVolume">
                                    </div>
                                </div>

                                <div class="form-check form-switch mb-3">
                                    <label for="chbDoNotIncludeBaseUOM" class="form-check-label">Do not include base unit in UoM selection in VDM GUI</label>
                                    <input class="form-check-input" type="checkbox" id="chbDoNotIncludeBaseUOM">
                                </div>

                                <div class="mb-3">
                                    When adding this item to the cart, recommend the item below as an additional purchase:
                                </div>

                                <div class="mb-3 ml-4">
                                    <div class="form-check mb-3">
                                        <label for="radRecomItemNoRecommendation" class="form-check-label">No recommendation</label>
                                        <input class="form-check-input" type="radio" name="radRecomItem" value="No Recommendation" id="radRecomItemNoRecommendation" checked>
                                    </div>
                                    <div class="row align-items-center">
                                        <div class="col-auto">
                                            <div class="form-check">
                                                <label for="radRecomItem" class="form-check-label">Recommend an item</label>
                                                <input class="form-check-input" type="radio" name="radRecomItem" value="with sku" id="radRecomItem">
                                            </div>
                                        </div>
                                        <div class="col">
                                            <div class="input-group" style="display:none;">
                                                <input class="form-control" type="text" id="txtRecomSKU" placeholder="Click to select SKU" readonly>
                                                <button data-role="none" class="btn btn-light border">Clear</button>
                                            </div>                                            
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="mb-3" style="display:none;">
                                <label for="txtMotorNo" class="form-label">Motor No.:</label>
                                <div class="input-group">
                                    <input data-role="none" type="text" id="txtMotorNo" class="form-control" readonly />
                                    <button data-role="none" id="btnHBMotorNoAssignment" class="btn btn-light border text-dark">Select Motors</button>
                                </div>
                                <select data-role="none" class="form-select d-none" id="selMotorNo"></select>
                            </div>

                            <div class="mb-3" style="display:none;">
                                <label for="txtBinNo" class="form-label">Bin No.:</label>
                                <div class="input-group">
                                    <input data-role="none" type="text" id="txtBinNo" class="form-control" readonly />
                                    <button data-role="none" id="btnHBBinNoAssignment" class="btn btn-light border text-dark">Select Bins</button>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <div class="row">
                                <div class="col">
                                    <button data-role="none" type="button" class="btn btn-secondary btn-lg btn-block" data-dismiss="modal">Cancel</button>
                                </div>
                                <div class="col">
                                    <button data-role="none" type="button" id="btnOkRename" class="btn btn-success btn-lg btn-block">Ok</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="popRenameSelectedHB" class="modal fade" tabindex="-1">
                <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Rename Hot Button</h5>
                            <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        </div>
                        <div class="modal-body">
                            <div class="mb-3">Please enter new name of the following:</div>

                            <div class="table-responsive">
                                <table class="table table-light table-striped table-hover">
                                    <thead>
                                        <tr class="align-top text-center" style="font-size:14px;">
                                            <th style="width:50px;">#</th>
                                            <th>Product No/Description</th>
                                            <th>Hot Button Name</th>
                                        </tr>
                                    </thead>
                                    <tbody id="tbodyRenameSelectedHB"></tbody>
                                </table>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <div class="row">
                                <div class="col">
                                    <button data-role="none" type="button" class="btn btn-secondary btn-lg btn-block" data-dismiss="modal">Cancel</button>
                                </div>
                                <div class="col">
                                    <button data-role="none" type="button" id="btnOkRenameSelectedHB" class="btn btn-success btn-lg btn-block">Ok</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="popHBSelectedSkus" class="modal fade" tabindex="-1">
                <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Selected SKUs</h5>
                            <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        </div>
                        <div class="modal-body">
                            <span class="mb-3">
                                <b>NOTE <span class="required">*</span>:</b>
                                Short Name is maximum of 50 characters only. Please change the short name into your desired name.
                            </span>

                            <table data-role="none" cellpadding="2" cellspacing="0" class="table table-striped table-hover">
                                <thead class="custom-theme-f">
                                    <tr>
                                        <th>SKU No</th>
                                        <th>SKU Name</th>
                                        <th>Short Name</th>
                                        <th></th>
                                    </tr>
                                </thead>

                                <tbody id="tBodySelectedSkus"></tbody>
                            </table>
                        </div>
                        <div class="modal-footer">
                            <div class="row">
                                <div class="col">
                                    <button data-role="none" type="button" class="btn btn-secondary btn-lg btn-block" data-dismiss="modal">Cancel</button>
                                </div>
                                <div class="col">
                                    <button data-role="none" type="button" id="btnOkSelectedSkus" class="btn btn-success btn-lg btn-block">Ok</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="popBrandCategory" class="modal fade" tabindex="-1">
                <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Selection</h5>
                            <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        </div>
                        <div class="modal-body">
                            <div class="mb-3">
                                Selected Brands and their categories will be generated.
                            </div>

                            <div id="divBrandLists">
                                <div class="accordion mb-3">
                                    <div class="card">
                                        <div class="card-header p-0">
                                            <div data-toggle="collapse" data-target="#divBrand" class="p-3">
                                                <div class="form-check form-check-inline">
                                                    <input data-role="none" type="checkbox" class="form-check-input" />
                                                    <label class="form-check-label">Brand</label>
                                                </div>
                                            </div>
                                        </div>
                                        <div id="divBrand" class="collapse show">
                                            <div class="card-body">

                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <div class="row">
                                <div class="col">
                                    <button data-role="none" type="button" class="btn btn-secondary btn-lg btn-block" data-dismiss="modal">Cancel</button>
                                </div>
                                <div class="col">
                                    <button data-role="none" type="button" id="btnOkBrandCategorySelection" class="btn btn-success btn-lg btn-block">Ok</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="popStoreAssignment" class="modal fade" tabindex="-1">
                <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Store Assignment</h5>
                            <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        </div>
                        <div class="modal-body">
                            <div class="row align-items-center mb-3">
                                <div class="col">
                                    This button (<i class="font-weight-bold"></i>) is only visible in the following stores:
                                </div>
                                <div class="col-auto">
                                    <button data-role="none" id="btnAddStoreAssignment" class="btn btn-secondary">Add Store</button>
                                </div>
                            </div>

                            <ul id="lstHBAssignedStores" class="list-group"></ul>
                        </div>
                        <div class="modal-footer">
                            <div class="row">
                                <div class="col">
                                    <button data-role="none" type="button" class="btn btn-secondary btn-lg btn-block" data-dismiss="modal">Cancel</button>
                                </div>
                                <div class="col">
                                    <button data-role="none" type="button" id="btnOkHBAssignedStores" class="btn btn-success btn-lg btn-block">Ok</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            @*<button data-role="none" id="btnAddGroup" class="btn custom-btn-theme-c btn-less-width btn-sm">Add Group</button>
        <button data-role="none" id="btnAddSku" class="btn custom-btn-theme-c btn-less-width btn-sm">Add SKU</button>*@

            <ul data-role="none" id="lstHotButtonList" class="list-group draggableList mb-5"></ul>

            <br />
            @*<table data-mode="reflow" cellpadding="2" cellspacing="0" class="custom-table table-stroke" width="100%">
            <thead class="custom-theme-f">
                <tr>
                    <th>DESCRIPTION</th>
                </tr>
            </thead>

            <tbody id="tBodyHotButtons">

            </tbody>
        </table>*@

            <div @*data-role="popup"*@ id="popupMenuHotButton" style="display:none;">
                <ul id="lstMenuHotButton" data-role="listview" data-inset="true" style="min-width:210px;">
                    <li data-role="divider" data-theme="a">Options</li>
                    <li id="btnAddGroup"><a>Add Group</a></li>
                    <li id="btnAddSku"><a>Add SKU</a></li>
                    @*<li id ="btnDeleteHotButtonSKUGroup"><a href="" data-rel="popup" data-position-to="window">Delete</a></li>
                <li id="btnEditHotButtonSKUGroup"><a href="" data-rel="popup" data-position-to="window">Edit</a></li>*@
                </ul>
            </div>
        </div>
    </div>

    <div id="tabContenHBFowst" class="tab-pane fade pt-3">
        <div class="row align-items-center">
            <div class="col-md-auto mb-3 mb-md-0">
                List of FOWST locations that will assign to this template.
            </div>
            <div class="col-md">
                <div class="input-group">
                    <input data-role="none" type="text" class="form-control" placeholder="Search here" />
                    <button type="button" id="btnHBAddFOWST" data-role="none" class="btn btn-success">
                        Add Store
                    </button>
                    <button type="button" data-role="none" class="btn btn-primary btn-light border" data-toggle="dropdown">
                        <span class="bi-gear"></span>
                    </button>
                    <ul class="dropdown-menu dropdown-menu-right">
                        <li><a class="dropdown-item" id="btnHBAllFOWST" href="#">Assign this HB template for use by All FOWST locations</a></li>
                        <li><a class="dropdown-item" id="btnHBFowstNoTemplate" href="#">See FOWST locations that have no assigned Hot Button template</a></li>
                    </ul>
                </div>
            </div>
        </div>

        <div class="mb-3"></div>

        <div class="row mb-3">
            <div class="col-sm-6">
                <div id="listHBFowst" class="list-group draggableList mb-5"></div>
            </div>
        </div>

        <div id="popHBFowstNoTemplate" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-lg modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title"></h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        List of FOWST locations that have no assigned Hot Button template. <br /><br />
                        <div id="listHBUnassignedFowst" class="list-group draggableList mb-5"></div>
                    </div>
                    <div class="modal-footer">
                        <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>        
    </div>
</div>

<div id="popVDMMotorAssignments" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">VDM Motor Assignments</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <table data-role="none" class="table table-light table-striped">
                    <thead>
                        <tr class="text-center">
                            <th>#</th>
                            <th>Category</th>
                            <th>SKU Name</th>
                            <th>Motor No.</th>
                        </tr>
                    </thead>
                    <tbody id="tbodyVDMMotorAssignments"></tbody>
                </table>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col">
                        <button data-role="none" type="button" class="btn btn-secondary btn-lg btn-block" data-dismiss="modal">Cancel</button>
                    </div>
                    <div class="col">
                        <button data-role="none" type="button" id="btnSaveHBVDMNow" class="btn btn-success btn-lg btn-block">Save</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="fixed-bottom text-center pb-3" style="display:block; left:50%; transform: translate(-50%, 0); right:auto">
    <button id="btnHotButtonsTemplateDone" data-role="none" class="btn btn-secondary btn-lg">
        Back
    </button>
    <button id="btnSaveHB" data-role="none" class="btn btn-success btn-lg">
        Save
    </button>
    <button id="btnAddHotButton" data-role="none" class="btn btn-warning btn-lg">
        Options
    </button>
</div>