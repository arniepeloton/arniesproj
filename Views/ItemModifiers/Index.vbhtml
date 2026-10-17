<script>
    require([GetPathFormat('/Scripts/sources/ItemModifiers/ItemModifiers.js?v=' + assetversion)]);
</script>

<div id="pageDeviceMenu" style="display:block;">
    
    @*START PopUps*@

    <div id="popRenameItemModifierName" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Rename Item Modifier</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">Item Modifier Name:</div>
                    <input type="text" id="txtRenameItemModifier" data-role="none" class="form-control mb-3" />
                    <span id="errorRenameItemModifier" style="color:#F00; display:none;">Please enter a name</span>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-6">
                            <button data-role="none" type="button" id="butCancelRenameItemModifier" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col-6">
                            <button data-role="none" type="button" id="butSaveRenameItemModifier" class="btn btn-success btn-block">Save</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popItemModifierName" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Item Modifier</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">Item Modifier Name:</div>
                    <input type="text" id="txtItemModifierNewName" data-role="none" class="form-control mb-3" />
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-6">
                            <button data-role="none" type="button" id="butCancelItemModifier" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col-6">
                            <button data-role="none" type="button" id="butSaveItemModifier" class="btn btn-success btn-block">Add</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="panIMAddOns" class="modal fade" tabindex="-1" data-backdrop="static">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Modifier</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label class="form-label" for="txtIMSku">SKU: <span class="required">*</span></label>
                        <input data-role="none" class="form-control" placeholder="Search Product" type="text" id="txtIMSku" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label" for="txtIMShortDescription">
                            Short Description: <span class="required">*</span> <br />
                            <i>
                                Please limit short description to 15 character max.
                                The short description will appear on the modifier User Interface (UI) visible to cashiers and staff.
                            </i>
                        </label>
                        <input data-role="none" class="form-control" onclick="this.select()" maxlength="15" type="text" id="txtIMShortDescription" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label" for="txtIMQuantity">Quantity: <span class="required">*</span></label>
                        <input data-role="none" class="form-control number-only" type="number" id="txtIMQuantity" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label" for="txtIMUnitMeasure">Unit Measure: <span class="required">*</span></label>
                        <input data-role="none" class="form-control" readonly type="text" id="txtIMUnitMeasure" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label" for="txtIMUnitPrice">Unit Price: <span class="required">*</span></label>
                        <input data-role="none" class="form-control number-only" type="number" id="txtIMUnitPrice" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label" for="txtIMPremiumPrice">Additional Premium Price: <span class="required">*</span></label>
                        <input data-role="none" class="form-control number-only" type="number" id="txtIMPremiumPrice" />
                    </div>
                    @*<br />
        <div class="mb-3">
            <label class="form-label" for="txtIMMaxSelection">Maximum Selection: <span class="required">*</span></label>
            <input data-role="none" class="form-control number-only" type="number" id="txtIMMaxSelection" />
        </div>*@
                    <div class="row mb-3 align-items-center">
                        <div class="col">
                            Do not show this modifier to the following stores:
                        </div>
                        <div class="col-auto text-right">
                            <button data-role="none" class="btn btn-light border" id="btnIMSelectAllStores">Select All Store</button>
                        </div>
                        <div class="col-auto text-right">
                            <button data-role="none" class="btn btn-danger" id="btnIMRemoveStores">Remove Store</button>
                        </div>
                        <div class="col-auto text-right">
                            <button data-role="none" class="btn btn-success" id="btnIMStores">Add Store</button>
                        </div>
                    </div>

                    <ul id="listIMStores" class="list-group"></ul>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-6">
                            <button data-role="none" type="button" id="btnCancelIMAddOns" class="btn btn-secondary btn-lg btn-block" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col-6">
                            <button data-role="none" type="button" id="btnAddIMAddOns" class="btn btn-success btn-lg btn-block">Ok</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="panIMRebates" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Return an Item</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <input data-role="none" type="text" id="txtRebateID" style="display:none;" />
                    <input data-role="none" type="text" id="txtSelectedSKUID" style="display:none;" />

                    <div class="mb-3">
                        <label class="form-label" for="txtIMSku">SKU to be returned:</label>
                        <button data-role="none" id="btnSKUName" class="btn btn-secondary btn-block txtSKUName"></button>
                    </div>

                    <div class="mb-3">
                        <label class="form-label" for="txtIMSku">Packing size quantity in base measure:</label>
                        <input data-role="none" class="form-control" type="text" id="txtIMReturnUnitQuantity" readonly />
                    </div>

                    <div class="mb-3">
                        <label class="form-label" for="txtIMReturnUnitMeasure">Unit Measure to be returned:</label>
                        <input data-role="none" class="form-control" type="text" id="txtIMReturnUnitMeasure" readonly />
                    </div>

                    <div class="mb-3">
                        <label class="form-label" for="txtIMReturnQuantity">Quantity to be returned:</label>
                        <input data-role="none" class="form-control" type="text" id="txtIMReturnQuantity" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label" for="txtIMRebatesAmount">Rebate Amount when returned:</label>
                        <input data-role="none" class="form-control" type="number" id="txtIMRebatesAmount" />
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-6">
                            <button data-role="none" type="button" id="btnCancelIMRebates" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col-6">
                            <button data-role="none" type="button" id="btnAddIMRebates" class="btn btn-success btn-block">Ok</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="panIMReplacements" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Replacements to Punched Item</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label class="form-label" for="txtIMReplacementLable">Replacement Item Label:</label>
                        <input type="text" id="txtIMReplacementLable" data-role="none" class="form-control" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label" for="txtIMStockNoAndDescription">Stock # and Description of Replacement:</label>
                        <input type="text" id="txtIMStockNoAndDescription" data-role="none" class="form-control" />
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-6">
                            <button data-role="none" type="button" id="btnCancelIMReplacements" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col-6">
                            <button data-role="none" type="button" id="btnAddIMReplacements" class="btn btn-success btn-block">Add</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="panIMCustomerPreferences" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Customer Preferences</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label class="form-label" for="txtIMCustomerPreferences">Preference:</label>
                        <input type="text" id="txtIMCustomerPreferences" data-role="none" class="form-control" />
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-6">
                            <button data-role="none" type="button" id="btnCancelIMCustomerPreferences" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col-6">
                            <button data-role="none" type="button" id="btnAddIMCustomerPreferences" class="btn btn-success btn-block">Ok</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="panIMProductCategories" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Product Categories</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div id="divErrorProductCategory" class="mb-3" style="color:#F00; display:none;">
                        Please select at least one
                    </div>
                    <div class="mb-3">
                        <label class="form-label" for="txtIMCategoryPreference">Preference:</label>
                        <input type="text" id="txtIMCategoryPreference" data-role="none" class="form-control" />
                    </div>
                    <div id="lstOfProductCategory" data-role="none" class="list-group"></div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-6">
                            <button data-role="none" type="button" id="btnCancelIMProductCategories" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col-6">
                            <button data-role="none" type="button" id="btnOkIMProductCategories" class="btn btn-success btn-block">Ok</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="panIMSelectedProductCategories" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">List of Selected Categories</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3 text-right">
                        <button data-role="none" id="btnAddNewItem" class="btn btn-success">+ Add</button>
                    </div>

                    <div id="lstOfSelectedProductCategory" data-role="none" class="list-group"></div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-6">
                            <button data-role="none" type="button" id="btnCancelSelectedIMProductCategories" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col-6">
                            <button data-role="none" type="button" id="btnAddSelectedIMProductCategories" class="btn btn-success btn-block">Add</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="panNonInventory" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Modifier</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label class="form-label" for="txtNIDescription">Description: <span class="required">*</span></label>
                        <input type="text" id="txtNIDescription" data-role="none" class="form-control" />
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-6">
                            <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col-6">
                            <button data-role="none" type="button" id="btnOkNI" class="btn btn-success btn-block">Ok</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    @*End PopUps*@

    <div data-role="content">
        <div class="row wrapper min-vh-100 flex-column flex-md-row">
            <div class="col-12 col-md-2 p-0 bg-light flex-shrink-1 mb-3">
                <nav class="navbar navbar-expand-sm navbar-light bg-light align-items-start flex-md-column flex-row p-2">
                    <a data-role="none" class="navbar-brand" href="#">Menu</a>
                    <a data-role="none" href="#" class="navbar-toggler" data-toggle="collapse" data-target=".sidebarIM">
                        <span class="navbar-toggler-icon"></span>
                    </a>
                    <div class="collapse navbar-collapse sidebarIM mt-2">
                        <ul class="flex-column w-100 justify-content-between nav nav-pills">
                            <li><a class="nav-link active" data-toggle="tab" href="#tabIMDetails">Modifier Details</a></li>
                            <li><a class="nav-link" data-role="none" data-toggle="tab" href="#tabIMDesignation">Designation</a></li>
                            <li><a class="nav-link" data-role="none" data-toggle="tab" href="#tabIMSchedule">Schedule</a></li>
                        </ul>
                    </div>
                </nav>
            </div>

            <div class="col bg-faded">
                <div class="tab-content">
                    <div class="tab-pane fade active show" id="tabIMDetails">
                        <div class="row mb-3 align-items-center">
                            <label class="col-sm-2">Modifier Name: <span class="required">*</span></label>
                            <div class="col-sm-10"><input data-role="none" type="text" class="form-control" id="txtGroupModifierName" /></div>
                        </div>

                        <div class="row mb-3 align-items-center">
                            <label class="col-sm-2">Modifier Description:</label>
                            <div class="col-sm-10"><input data-role="none" type="text" class="form-control" id="txtGroupModifierDescription" /></div>
                        </div>

                        <div class="row mb-3 align-items-center">
                            <label class="col-sm-2">Modifier Type: <span class="required">*</span></label>
                            <div class="col-sm-10">
                                <b id="lblModifierType" subpage="pageModifierType" style="display:block;"></b>
                                <select data-role="none" subpage="pageModifierType" class="form-select" id="selModifierType" style="display:block;">
                                    <option value="Inventory Affecting">Inventory Affecting</option>
                                    <option value="Non-Inventory Affecting">Non-Inventory Affecting</option>
                                    <option value="Return For Rebates">Return For Rebates</option>
                                </select>
                            </div>
                        </div>

                        <div class="row mb-3 align-items-center">
                            <div class="col-sm-2">This modifier is required?</div>
                            <div class="col-sm-10">
                                <div class="form-check form-switch mb-3" style="font-size:20px;">
                                    <input class="form-check-input" data-role="none" type="checkbox" id="selModifierRequired">
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3 align-items-center">
                            <div class="col-sm-2">Active Modifier?</div>
                            <div class="col-sm-10">
                                <div class="form-check form-switch mb-3" style="font-size:20px;">
                                    <input class="form-check-input" data-role="none" type="checkbox" id="selActiveModifier">
                                </div>
                            </div>
                        </div>

                        <div class="accordion mb-3" id="divInventoryAffecting" subpage="pageItemModifiers" style="display:none;">
                            <div class="card">
                                <div class="card-header p-0">
                                    <button data-target="#divIA" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                        <b>Modifier Options <span style="font-weight:normal; font-style:italic">(these are the options that users can choose from)</span></b>
                                    </button>
                                </div>
                                <div id="divIA" class="collapse show">
                                    <div class="card-body">
                                        <div class="row mb-3">
                                            <label class="col-sm-2">
                                                Select Options:
                                            </label>
                                            <div class="col-sm-10">
                                                <div class="form-check mb-3">
                                                    <input type="radio" data-role="none" class="form-check-input" id="optIASku" name="chbOptIA" value="fromlistsku" checked>
                                                    <label class="form-check-label" for="optIASku">Customer/user can choose from a list of SKU(s)</label>
                                                </div>

                                                <div class="form-check">
                                                    <input type="radio" data-role="none" class="form-check-input" id="optIABuyOneTakeOne" name="chbOptIA" value="buy1take1">
                                                    <label class="form-check-label" for="optIABuyOneTakeOne">System automatically chooses the same SKU as the one that the customer/user has selected. <span style="font-style:italic; font-weight:normal;">(This is useful for promos like Buy 1 Take 1)</span></label>
                                                </div>
                                            </div>
                                        </div>

                                        <div id="divForChooseFromSku" subpage="optionforia" style="display:block;">
                                            <div class="row mb-3">
                                                <label class="col-sm-2 col-form-label" for="txtIAMin">
                                                    Minimum selection:
                                                </label>
                                                <div class="col-sm-2">
                                                    <input type="text" data-role="none" class="form-control" id="txtIAMin">
                                                </div>
                                                <label class="col-sm-2 col-form-label" for="txtIAMax">
                                                    Maximum selection:
                                                </label>
                                                <div class="col-sm-2">
                                                    <input type="text" data-role="none" class="form-control" id="txtIAMax">
                                                </div>
                                                <div class="col-sm-4 text-right">
                                                    <button id="btnIMAddModifier" data-role="none" class="btn btn-success">
                                                        Add SKU
                                                    </button>
                                                </div>
                                            </div>

                                            <table data-role="none" class="table table-striped table-hover display mb-3">
                                                <thead>
                                                    <tr class="text-center">
                                                        <th style="width:30px;"> # </th>
                                                        <th> SKU # </th>
                                                        <th> SKU </th>
                                                        <th> Short Description </th>
                                                        @*<th style="width:150px;"> Max. Selection </th>*@
                                                        <th> Qty </th>
                                                        <th> Unit Measure </th>
                                                        <th> Unit Price </th>
                                                        <th> </th>
                                                        <th> </th>
                                                    </tr>
                                                </thead>

                                                <tbody id="tBodyListOfModifiers"></tbody>
                                            </table>
                                        </div>
                                        <div id="divForSameSku" subpage="optionforia" style="display:block;">
                                            @*<div class="row mb-3">
                                <div class="col-sm-2">
                                    Quantity: <span class="required">*</span>
                                </div>
                                <div class="col-sm-3">
                                    <input type="text" data-role="none" id="txtSameSkuQty" class="form-control" />
                                </div>
                            </div>
                            <br />
                            <div class="row mb-3">
                                <div class="col-sm-2">
                                    Discount (%): <span class="required">*</span>
                                </div>
                                <div class="col-sm-3">
                                    <input type="text" data-role="none" id="txtSameSkuDiscount" class="form-control" />
                                </div>
                            </div>
                            <br />*@
                                            <table data-role="none" class="table table-striped table-hover display mb-3">
                                                <thead>
                                                    <tr class="text-center">
                                                        <th> SKU </th>
                                                        <th> Buy Qty <span class="required">*</span> </th>
                                                        <th> Take Qty <span class="required">*</span> </th>
                                                        <th> Take Price </th>
                                                        <th> Take Discount (%) </th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <tr>
                                                        <td><span style="font-style:italic">SKU selected by the user</span></td>
                                                        <td><input type="text" data-role="none" class="form-control" id="txtSameSkuBuyQty" /></td>
                                                        <td><input type="text" data-role="none" class="form-control" id="txtSameSkuTakeQty" /></td>
                                                        <td><input type="text" data-role="none" class="form-control" id="txtSameSkuPrice" /></td>
                                                        <td><input type="text" data-role="none" class="form-control" id="txtSameSkuDiscount" /></td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="accordion mb-3" id="divNonInventoryAffecting" subpage="pageItemModifiers" style="display:none;">
                            <div class="card">
                                <div class="card-header p-0">
                                    <button data-target="#divNIA" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                        <b>Modifier Options <span style="font-weight:normal; font-style:italic">(these are the options that users can choose from)</span></b>
                                    </button>
                                </div>
                                <div id="divNIA" class="collapse show">
                                    <div class="card-body">
                                        <div class="row mb-3">
                                            <div class="col-sm-1">
                                                Choices:
                                            </div>
                                            <div class="col-sm-2">
                                                <div class="form-check mb-3">
                                                    <input type="radio" data-role="none" class="form-check-input" id="optNIASingle" name="affecting" value="radio">
                                                    <label class="form-check-label" for="optNIASingle">Single Select</label>
                                                </div>
                                            </div>
                                            <div class="col-sm-2">
                                                <div class="form-check mb-3">
                                                    <input type="radio" data-role="none" class="form-check-input" id="optNIAMultiple" name="affecting" value="checkbox">
                                                    <label class="form-check-label" for="optNIAMultiple">Multiple Select</label>
                                                </div>
                                            </div>
                                            <div class="col-sm-5">
                                                <div class="form-inline" id="divOptionForMultipleSelect" style="display:none;">
                                                    <div class="form-group">
                                                        <label for="txtNIAMax" style="font-weight:normal;">Maximum items to be selected: <i>(If 0 or empty, all items can be selected)</i></label>
                                                        <input type="text" data-role="none" class="form-control" id="txtNIAMax" style="width:77%">
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-sm-2 text-right">
                                                <button id="btnIMAddNonInventoryModifier" data-role="none" class="btn btn-success">
                                                    Add Modifier
                                                </button>
                                            </div>
                                        </div>

                                        <div class="row mb-3">
                                            <div class="col-sm-4">
                                                Enter a description for the field labelled "Others":
                                                <br />
                                                <i>(This custom field is used when a customer has specific instructions for their orders.)</i>
                                            </div>
                                            <div class="col-sm">
                                                <input data-role="none" class="form-control mb-3" id="txtNIAOthersLabel" />
                                            </div>
                                            <div class="col-sm-auto">Character limit:</div>
                                            <div class="col-sm-auto">
                                                <input data-role="none" type="number" class="form-control" id="txtNIAOthersCharLimit" style="width:100px;" />
                                            </div>
                                        </div>

                                        <div class="mb-3"><i><b>Note</b>: You can drag the items in any desired order.</i></div>

                                        <div id="lstNonInventoryModifiers" class="list-group mb-3"></div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="accordion mb-3" id="divItemRebates" subpage="pageItemModifiers" style="display:none;">
                            <div class="card">
                                <div class="card-header p-0">
                                    <button data-target="#divRebates" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                        <b>Return for Rebates</b>
                                    </button>
                                </div>
                                <div id="divRebates" class="collapse show">
                                    <div class="card-body">
                                        <div class="row mb-3">
                                            <div class="col-sm-10">
                                                <p><strong>Items to be Returned in Exchange for Rebates Against Punched Item</strong></p>
                                                <p>(These returned items can serve as discounts against punched item. Cashiers can choose one)</p>
                                            </div>
                                            <div class="col-sm-2 text-right">
                                                <button id="btnIMRebates" data-role="none" class="btn btn-success">
                                                    Add SKU
                                                </button>
                                            </div>
                                        </div>

                                        <table data-role="none" class="table table-striped table-hover display mb-3">
                                            <thead>
                                                <tr class="text-center">
                                                    @*<th> Packing Size Quantity </th>*@
                                                    <th> Unit Measure </th>
                                                    <th> SKU # and Description to be Returned </th>
                                                    <th> Quantity to be Returned </th>
                                                    <th> Rebate Amount </th>
                                                    <th> </th>
                                                </tr>
                                            </thead>

                                            <tbody id="tBodyItemsRebates"></tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3 mt-5">
                            <div class="col-auto">This modifier is applicable to:</div>
                            <div class="col">
                                <div class="form-check form-check-inline" style="font-size:20px;">
                                    <input class="form-check-input" data-role="none" type="radio" name="radModApp" value="category" id="modappCategory">
                                    <label class="form-check-label" for="modappCategory">Categories</label>
                                </div>

                                <div class="form-check form-check-inline" style="font-size:20px;">
                                    <input class="form-check-input" data-role="none" type="radio" name="radModApp" value="sku" id="modappSku">
                                    <label class="form-check-label" for="modappSku">SKUs</label>
                                </div>
                            </div>
                        </div>

                        <div class="accordion mb-3">
                            <div class="card">
                                <div class="card-header p-0">
                                    <button data-target="#divApplyTo" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                        <b>Applied to <span class="lblModApp" style="text-transform:capitalize">Categories</span></b>
                                    </button>
                                </div>
                                <div id="divApplyTo" class="collapse show">
                                    <div class="card-body">
                                        <div class="row mb-3">
                                            <div class="col-sm-10">Click "+" to add <span class="lblModApp">Categories</span></div>
                                            <div class="col-sm-2 text-right">
                                                <button id="btnIMProductCategories" data-role="none" class="btn btn-success">
                                                    Add <span class="lblModApp">Categories</span>
                                                </button>
                                            </div>
                                        </div>

                                        <table data-role="none" class="table table-striped table-hover display mb-3">
                                            <thead>
                                                <tr class="text-left">
                                                    <th> Preferences </th>
                                                    <th style="width:100px"></th>
                                                </tr>
                                            </thead>

                                            <tbody id="tBodyCategories">
                                                <tr>
                                                    <td> &nbsp; </td>
                                                    <td> &nbsp; </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="tab-pane fade" id="tabIMDesignation">
                        <div id="tabIMDesignation" style="white-space:nowrap; overflow:auto;">
                            <ul class="nav nav-tabs" role="tablist" style="flex-wrap:nowrap;">
                                <li class="nav-item" role="presentation">
                                    <button data-role="none" class="nav-link active" data-toggle="tab" data-target="#tabIMAssignedFowst">FOWST Locations</button>
                                </li>
                                <li class="nav-item" role="presentation">
                                    <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabIMAssignedCustomer">Customers</button>
                                </li>
                            </ul>
                        </div>

                        <div class="tab-content">
                            <div class="tab-pane fade p-3 show active" id="tabIMAssignedFowst">
                                <div class="row mb-3">
                                    <div class="col">
                                        This modifier will only be available to the FOWST locations listed below.
                                        If no assigned FOWST locations, it will be available to all FOWST locations.
                                    </div>
                                    <div class="col-auto text-right">
                                        <button data-role="none" id="btnIMRemoveAllAssignedFOWST" class="btn btn-danger">
                                            Remove All
                                        </button>
                                        <button data-role="none" id="btnIMAssignedFOWST" class="btn btn-success">
                                            Add FOWST Locations
                                        </button>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-6">
                                        <ul id="listIMAssignedFowstLocations" class="list-group">
                                            <li class="list-group-item">No assigned FOWST locations</li>
                                        </ul>
                                    </div>
                                </div>
                            </div>

                            <div class="tab-pane fade p-3" id="tabIMAssignedCustomer">
                                <div class="row mb-3">
                                    <div class="col-9">
                                        This modifier will only be available to the Customers listed below.
                                        If no assigned customers, it will be available to all Customers.
                                    </div>
                                    <div class="col text-right">
                                        <button data-role="none" id="btnIMAssignedCustomer" class="btn btn-success">
                                            Add Customers
                                        </button>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-6">
                                        <ul id="listIMAssignedCustomers" class="list-group">
                                            <li class="list-group-item">No assigned Customers</li>
                                        </ul>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="tab-pane fade" id="tabIMSchedule">
                        <div class="row mb-3">
                            <h4 class="col-auto" for="selRestrictedModifier">Set availability schedule?</h4>
                            <div class="col">
                                <div class="form-check form-switch form-check-inline mb-3" style="font-size:20px;">
                                    <input class="form-check-input" data-role="none" type="checkbox" id="selRestrictedModifier">
                                </div>
                            </div>
                        </div>

                        <div id="divForRestrictedModifier">
                            <div class="row mb-3">
                                <div class="col-auto">
                                    Select Option:
                                </div>
                                <div class="col-auto">
                                    <div class="form-check">
                                        <input data-role="none" type="radio" class="form-check-input" name="radIMAvailability" id="radAvailableByDay" value="byday" checked />
                                        <label for="radAvailableByDay" class="form-check-label">By Day/Time</label>
                                    </div>
                                </div>
                                <div class="col-auto">
                                    <div class="form-check">
                                        <input data-role="none" type="radio" class="form-check-input" name="radIMAvailability" id="radAvailableByDatePeriod" value="bydateperiod" />
                                        <label for="radAvailableByDatePeriod" class="form-check-label">By Date Period</label>
                                    </div>
                                </div>
                            </div>

                            <div id="tableRestrictedTime" class="divTable" style="font-size: inherit !important; width: auto !important;">
                                <div class="row mb-3 text-center align-items-center">
                                    <div class="col-2"></div>
                                    <div class="col-2">From</div>
                                    <div class="col-2">To</div>
                                </div>
                                <div class="row mb-3 align-items-center">
                                    <div class="col-2">Monday</div>
                                    <div class="col-2">
                                        <input type="text" data-role="none" id="txtModifierMondayFrom" class="btnRestrictedTime form-control" readonly />
                                    </div>
                                    <div class="col-2">
                                        <input type="text" data-role="none" id="txtModifierMondayTo" class="btnRestrictedTime form-control" readonly />
                                    </div>
                                    <div class="col">
                                        <button data-role="none" class="btn btn-secondary btnRestrictedClearField">Clear Field</button>
                                    </div>
                                </div>
                                <div class="row mb-3 align-items-center">
                                    <div class="col-2">Tuesday</div>
                                    <div class="col-2">
                                        <input type="text" data-role="none" id="txtModifierTuesdayFrom" class="btnRestrictedTime form-control" readonly />
                                    </div>
                                    <div class="col-2">
                                        <input type="text" data-role="none" id="txtModifierTuesdayTo" class="btnRestrictedTime form-control" readonly />
                                    </div>
                                    <div class="col">
                                        <button data-role="none" class="btn btn-secondary btnRestrictedClearField">Clear Field</button>
                                    </div>
                                </div>
                                <div class="row mb-3 align-items-center">
                                    <div class="col-2">Wednesday</div>
                                    <div class="col-2">
                                        <input type="text" data-role="none" id="txtModifierWednesdayFrom" class="btnRestrictedTime form-control" readonly />
                                    </div>
                                    <div class="col-2">
                                        <input type="text" data-role="none" id="txtModifierWednesdayTo" class="btnRestrictedTime form-control" readonly />
                                    </div>
                                    <div class="col">
                                        <button data-role="none" class="btn btn-secondary btnRestrictedClearField">Clear Field</button>
                                    </div>
                                </div>
                                <div class="row mb-3 align-items-center">
                                    <div class="col-2">Thursday</div>
                                    <div class="col-2">
                                        <input type="text" data-role="none" id="txtModifierThursdayFrom" class="btnRestrictedTime form-control" readonly />
                                    </div>
                                    <div class="col-2">
                                        <input type="text" data-role="none" id="txtModifierThursdayTo" class="btnRestrictedTime form-control" readonly />
                                    </div>
                                    <div class="col">
                                        <button data-role="none" class="btn btn-secondary btnRestrictedClearField">Clear Field</button>
                                    </div>
                                </div>
                                <div class="row mb-3 align-items-center">
                                    <div class="col-2">Friday</div>
                                    <div class="col-2">
                                        <input type="text" data-role="none" id="txtModifierFridayFrom" class="btnRestrictedTime form-control" readonly />
                                    </div>
                                    <div class="col-2">
                                        <input type="text" data-role="none" id="txtModifierFridayTo" class="btnRestrictedTime form-control" readonly />
                                    </div>
                                    <div class="col">
                                        <button data-role="none" class="btn btn-secondary btnRestrictedClearField">Clear Field</button>
                                    </div>
                                </div>
                                <div class="row mb-3 align-items-center">
                                    <div class="col-2">Saturday</div>
                                    <div class="col-2">
                                        <input type="text" data-role="none" id="txtModifierSaturdayFrom" class="btnRestrictedTime form-control" readonly />
                                    </div>
                                    <div class="col-2">
                                        <input type="text" data-role="none" id="txtModifierSaturdayTo" class="btnRestrictedTime form-control" readonly />
                                    </div>
                                    <div class="col">
                                        <button data-role="none" class="btn btn-secondary btnRestrictedClearField">Clear Field</button>
                                    </div>
                                </div>
                                <div class="row mb-3 align-items-center">
                                    <div class="col-2">Sunday</div>
                                    <div class="col-2">
                                        <input type="text" data-role="none" id="txtModifierSundayFrom" class="btnRestrictedTime form-control" readonly />
                                    </div>
                                    <div class="col-2">
                                        <input type="text" data-role="none" id="txtModifierSundayTo" class="btnRestrictedTime form-control" readonly />
                                    </div>
                                    <div class="col">
                                        <button data-role="none" class="btn btn-secondary btnRestrictedClearField">Clear Field</button>
                                    </div>
                                </div>
                            </div>

                            <div id="divIMByDatePeriod" style="display:none;">
                                <div class="row mb-3">
                                    <div class="col-1">Date From: <span class="required">*</span></div>
                                    <div class="col-3">
                                        <input data-role="none" type="text" id="txtIMDatePeriodFrom" class="form-control txtIMDatePeriod" readonly />
                                    </div>
                                </div>
                                <div class="row mb-3">
                                    <div class="col-1">Date To: <span class="required">*</span></div>
                                    <div class="col-3">
                                        <input data-role="none" type="text" id="txtIMDatePeriodTo" class="form-control txtIMDatePeriod" readonly />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>     

        <div id="divItemModifierMenuMobile" style="display:none;">
            <div id="btnMobileMenuIM" style="background-color:#335F8E; float:right; padding:7px; cursor:pointer;">
                <div style="border:2px solid #FFF; width:30px;"></div>
                <div style="border:2px solid #FFF; margin-top:7px; width:30px;"></div>
                <div style="border:2px solid #FFF; margin-top:7px; width:30px;"></div>
            </div>
            <div style="clear:both"></div>
            <div id="divListMenuIM" style="display:none;">
                <ul id="listMenuIM" class="custom-list-menu txt-left">
                    <li class="custom-btn-theme-c">
                        <a id="btnIMMenuAddOns" data-gotopage="notyetimplemented" class="disabled-menu">
                            <img alt="Add-Ons" src="/Content/images/AddOnsIcon.png" />
                            <div>Add-Ons (Cross-Sells)</div>
                        </a>
                    </li>
                    <li class="custom-btn-theme-c">
                        <a id="btnIMMenuItemReturned" data-gotopage="divItemRebates" class="active-menu">
                            <img alt="Add-Ons" src="/Content/images/RebatesReturnsIcon.png" />
                            <div>Return for Rebates</div>
                        </a>
                    </li> 
                    <li class="custom-btn-theme-c">
                        <a id="btnIMMenuReplacements" data-gotopage="notyetimplemented"class="disabled-menu">
                            <img alt="Add-Ons" src="/Content/images/ReplacementsIcon.png" />
                            <div>Replacements (Up-Sells)</div>
                        </a>
                    </li>
                    <li class="custom-btn-theme-c">
                        <a id="btnIMMenuCustomerPreferences" data-gotopage="notyetimplemented" class="disabled-menu">
                            <img alt="Add-Ons" src="/Content/images/PreferencesIcon.png" />
                            <div>Preferences</div>
                        </a>
                    </li>
                </ul>
            </div>    
        </div>        

        <div style="width:100%; display:none; visibility:hidden;">
            <div style="width:19%; float:left;">
                Item Modifier Name:
            </div>

            <div style="width:79%; float:right;">
                <span style="font-size:25px; font-weight:bold;" id="txtItemModifierName"></span>
                <a data-role="none" id="btnRenameItemModifier" class="custom-link">(Rename)</a>
            </div>
            <div style="clear:both;"></div>
        </div>        

        <div style="width:100%; display:none; visibility:hidden;">
            <div id="divItemModifierMenuDesktop" style="width:19%; float:left;">
                <br /><br />
                <div style="background-color:#F7F7F7; margin-top: -17px;">
                    <ul id="listMenuIM" class="custom-list-menu txt-left">
                        <li>
                            <a id="btnIMMenuAddOns" data-gotopage="divAddOnsItem" class="active-menu">
                                <img alt="Add-Ons" src="/Content/images/AddOnsIcon.png" />
                                <div>Add-Ons (Cross-Sells)</div>
                            </a>
                        </li>
                        <li>
                            <a id="btnIMMenuItemReturned" data-gotopage="divItemRebates">
                                <img alt="Add-Ons" src="/Content/images/RebatesReturnsIcon.png" />
                                <div>Return for Rebates</div>
                            </a>
                        </li> 
                        <li>
                            <a id="btnIMMenuReplacements" data-gotopage="notyetimplemented"class="disabled-menu">
                                <img alt="Add-Ons" src="/Content/images/ReplacementsIcon.png" />
                                <div>Replacements (Up-Sells)</div>
                            </a>
                        </li>
                        <li>
                            <a id="btnIMMenuCustomerPreferences" data-gotopage="notyetimplemented" class="disabled-menu">
                                <img alt="Add-Ons" src="/Content/images/PreferencesIcon.png" />
                                <div>Preferences</div>
                            </a>
                        </li>
                    </ul>
                </div>
            </div>

            <div id="divItemModifierMenuContent" style="width:79%; float:right;">
                <br /><br />
                <div id="divContainerMenuPage" style="display:block;">
                    <div id="divAddOnsItem" style="display:block;">
                        <div class="ui-grid-a">
                            <div class="ui-block-a">
                                <p><strong>Add-Ons to Punched Item</strong></p>
                                <p>(For Cross Selling. Cashiers can choose more than one add-on)</p>
                            </div>
                            <div class="ui-block-b">
                                <div style="text-align: right;">
                                    <button id="btnIMAddOns" data-role="none" class="btn custom-btn-theme-b btn-less-width">Add Modifier</button>
                                </div>
                            </div>
                        </div>
                        <br />
                        <table data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe">
                            <thead>
                                <tr class="custom-theme-f txt-center">
                                    <th> # </th>
                                    @*<th> SKU </th>*@
                                    <th> Short Description </th>                                    
                                    <th> Qty </th>
                                    <th> Unit Measure </th>
                                    <th> Unit Price </th>
                                </tr>
                            </thead>

                            <tbody id="tBodyAddOnItems">
                            </tbody>
                        </table>
                    </div>                    

                    <div id="divReplacements" style="display:none;">
                        <div class="ui-grid-a">
                            <div class="ui-block-a">
                                <p><strong>Replacements to Punched Item</strong></p>
                                <p>(For Upselling to Customers. Cashiers can choose one replacement)</p>
                            </div>
                            <div class="ui-block-b">
                                <div style="text-align: right;">
                                    <button id="btnIMAddReplacements" data-role="none" class="btn custom-btn-theme-b btn-less-width">Add Items</button>
                                </div>
                            </div>
                        </div>
                        <br />
                        <table data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe">
                            <thead>
                                <tr class="custom-theme-f txt-center">
                                    <th> Replacement Item Label </th>
                                    <th> Stock # and Description of Replacement </th>
                                </tr>
                            </thead>

                            <tbody id="tBodyReplacements">
                                <tr>
                                    <td> &nbsp; </td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td> &nbsp; </td>
                                    <td></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>

                    <div id="divPreferences" style="display:none;">
                        <div class="ui-grid-a">
                            <div class="ui-block-a">
                                <p><strong>Customer Preferences for Punched Item</strong></p>
                                <p>(These act as non-inventory-affecting modifiers for the punched item)</p>
                            </div>
                            <div class="ui-block-b">
                                <div style="text-align: right;">
                                    <button id="btnIMCustomerPreferences" data-role="none" class="btn custom-btn-theme-b btn-less-width">Add Preferences</button>
                                </div>
                            </div>
                        </div>
                        <br />
                        <table data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe">
                            <thead>
                                <tr class="custom-theme-f txt-left">
                                    <th> Preferences </th>
                                </tr>
                            </thead>

                            <tbody id="tBodyPreferences">
                                <tr>
                                    <td> &nbsp; </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>

                    <div id="notyetimplemented" style="display:none;">
                        <h2>Not yet implemented.</h2>
                    </div>
                </div>
            </div>
            <div style="clear:both;"></div>
        </div>

        <br /><br />
        <div class="fixed-bottom text-center pb-3" style="display:block; left:50%; transform: translate(-50%, 0); right:auto">
            <button id="btnCancelItemModifier" data-role="none" class="btn btn-secondary btn-lg">Cancel</button>
            <button id="btnSaveItemModifier" data-role="none" class="btn btn-success btn-lg">Save</button>
            <button id="btnSaveAsItemModifier" data-role="none" class="btn btn-light border text-dark btn-lg">Save As</button>
        </div>
    </div>
</div>