<script>
    require([GetPathFormat('/Scripts/sources/OnlineStoreCategories/CreateCategories.js?v=' + assetversion),
        //GetPathFormat('/Scripts/jquery-ui-sortable.js')
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
<p class="mb-4">Use this tool to create store category. Tap on "Add Button" to add a new group or specific SKU. You can use your cursor to drag and drop lists into the desired order.</p>

<div class="row">
    <div class="col-sm-4">
        <div class="form-check form-switch mb-3" style="font-size:20px;">
            <input class="form-check-input" data-role="none" type="checkbox" id="chbStoreCategoriesIsDefault">
            <label class="form-check-label" for="chbStoreCategoriesIsDefault">Use as default Store Categories</label>
        </div>
    </div>
    <div class="col-sm-8" style="text-align:right;">
        <div>            
            <button id="btnStoreCatGenerateHB" data-role="none" class="btn btn-success" data-inline="true">Generate Store Categories</button>
            <button id="btnStoreCatSaveName" data-role="none" class="btn btn-success" data-inline="true" style="display:none;">Save Name</button>
            <button id="btnStoreCatEditName" data-role="none" class="btn btn-warning" data-inline="true">Edit Name</button>
            <button id="btnStoreCatCancel" data-role="none" class="btn btn-danger" data-inline="true" style="display:none;">Cancel</button>
            <button id="btnStoreCatDeleteTemplate" data-role="none" class="btn btn-danger" data-inline="true">Delete Template</button>
        </div>        
    </div>
</div>

<div>
    <h1 id="hdrTemplateName" class="display-6 mb-4"></h1><div id="divEditName" class="mb-4" style="display:none;"><input data-role="none" class="form-control" type="text" id="txtStoreCatEditName" maxlength="50" /></div>
    @*<div>
        <a id="btnStoreCatAddHotButtonfake" data-icon="plus" data-role="button" data-inline="true" style="float:right; margin-top:-15px;">Options</a>
    </div>*@

    <div id="popupStoreCateNewAddGroup" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Group Name</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <b>NOTE <span class="required">*</span>:</b> Maximum of 23 characters only.
                    </div>                    

                    <input type="text" id="txtStoreCatGroupName" data-role="none" class="form-control mb-3" placeholder="Enter group name" maxlength="23" />
                    
                    <div id="divCategoryFeature" class="form-check mb-3" style="font-size:20px;">
                        <input class="form-check-input" data-role="none" type="checkbox" id="chbStoreCategoryFeature">
                        <label class="form-check-label" for="chbStoreCategoryFeature">
                            Featured Category
                            <br />
                            <span style="font-size:16px;">Checking this box will make this category appear on the navigation bar near the top of the online store screen. Check this if you want customers to pay attention to this category.</span>
                        </label>
                    </div>
                </div>
                <div class="modal-footer">
                    <button data-role="none" type="button" id="btnStoreCatOkGroupName" class="btn btn-success btn-block">Ok</button>
                    <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <div id="popupStoreCatRenameHB" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Rename Hot Button</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <span class="mb-3">Please enter new name for <b id="txtStoreCatCurrentHBName"></b></span>
                    <input type="text" data-role="none" class="form-control mb-3" id="txtStoreCatRenameHB" placeholder="Enter new name" />
                    <div id="divStoreCategoryFeature" class="form-check mb-3" style="font-size:20px;">
                        <input class="form-check-input" data-role="none" type="checkbox" id="chbEditStoreCategoryFeature">
                        <label class="form-check-label" for="chbEditStoreCategoryFeature">
                            Featured Category
                            <br /><br />
                            Checking this box will make this category appear on the navigation bar near the top of the online store screen. Check this if you want customers to pay attention to this category.
                        </label>
                    </div>
                </div>
                <div class="modal-footer">
                    <button data-role="none" type="button" id="btnStoreCatOkRename" class="btn btn-success btn-block">Ok</button>
                    <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <div id="popupStoreCatHBSelectedSkus" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Selected SKUs</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <span class="mb-3">
                        <b>NOTE <span class="required">*</span>:</b>
                        You can change the display name into your desired name.
                    </span>

                    <table data-role="none" cellpadding="2" cellspacing="0" class="table table-striped table-hover">
                        <thead class="custom-theme-f">
                            <tr>
                                <th>SKU Name</th>
                                <th>Display Name</th>
                                <th></th>
                            </tr>
                        </thead>

                        <tbody id="tBodyStoreCatSelectedSkus"></tbody>
                    </table>
                </div>
                <div class="modal-footer">
                    <button data-role="none" type="button" id="btnStoreCatOkSelectedSkus" class="btn btn-success btn-block">Ok</button>
                    <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    @*<button data-role="none" id="btnStoreCatAddGroup" class="btn custom-btn-theme-c btn-less-width btn-sm">Add Group</button>
    <button data-role="none" id="btnStoreCatAddSku" class="btn custom-btn-theme-c btn-less-width btn-sm">Add SKU</button>*@

    <ul id="lstStoreCatHotButtonList" class="list-group draggableList mb-5">
        @*<li>
                <div style ="display:flex;">
                    <span class="glyphicon glyphicon-menu-hamburger"></span>
                    Sample
                    <div class="dropdown" style="margin-top:-2px;">
                        <button data-role="none" class="btn btn-default btn-xs dropdown-toggle" type="button" id="dropdownMenu2" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">

                            <span class="caret"></span>
                        </button>
                        <ul class="dropdown-menu" aria-labelledby="dropdownMenu2">
                            <li><a data-role="none" href="#">Action</a></li>
                            <li><a data-role="none" href="#">Another action</a></li>
                            <li><a data-role="none" href="#">Something else here</a></li>
                        </ul>
                    </div>
                </div>
            </li>*@
    </ul>
    <br />

    <div @*data-role="popup"*@ id="popupStoreCatMenuHotButton" style="display:none;">
		<ul id="lstStoreCatMenuHotButton" data-role="listview" data-inset="true" style="min-width:210px;" >
			<li data-role="divider" data-theme="a" >Options</li>
            <li id="btnStoreCatAddGroup"><a>Add Group</a></li>
            <li id="btnStoreCatAddSku"><a>Add SKU</a></li>
            @*<li id ="btnDeleteHotButtonSKUGroup"><a href="" data-rel="popup" data-position-to="window">Delete</a></li>
            <li id="btnStoreCatEditHotButtonSKUGroup"><a href="" data-rel="popup" data-position-to="window">Edit</a></li>*@
		</ul>
	</div>
</div>

<div class="fixed-bottom text-center pb-3" style="display:block;">
    <button id="btnStoreCatHotButtonsTemplateDone" data-toggle="tooltip" data-placement="top" title="Back" data-role="none" class="btn btn-secondary btn-lg">
        Back
    </button>
    <button id="btnStoreCatSaveHB" data-role="none" data-toggle="tooltip" data-placement="top" title="Save" class="btn btn-success btn-lg">
        Save
    </button>
    <button id="btnStoreCatAddHotButton" data-role="none" data-toggle="tooltip" data-placement="top" title="Add" class="btn btn-warning btn-lg">
        Add
    </button>
</div>