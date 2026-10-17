<script>
    require([GetPathFormat('/Scripts/sources/SystemBehaviorSettings/InventoryCount.js?v=' + assetversion)]);
</script>

<div class="row wrapper min-vh-100 flex-column flex-lg-nowrap flex-md-row">
    <div class="col-12 col-md-2 p-0 bg-light flex-shrink-1 mb-3">
        <nav class="navbar navbar-expand-md navbar-light bg-light align-items-start flex-md-column flex-row p-2">
            <a data-role="none" class="navbar-brand" href="#">Menu</a>
            <a data-role="none" href="#" class="navbar-toggler" data-toggle="collapse" data-target=".sidebarInventoryCount">
                <span class="navbar-toggler-icon"></span>
            </a>
            <div class="collapse navbar-collapse sidebarInventoryCount mt-2">
                <ul class="flex-column w-100 justify-content-between nav nav-pills">
                    <li><a class="nav-link active" role="button" data-role="none" data-toggle="tab" href="#tabInventoryCountSettings">General Settings</a></li>
                    <li><a class="nav-link" role="button" data-role="none" data-toggle="tab" href="#tabInventoryCountSKURanking">SKU Ranking</a></li>
                </ul>
            </div>
        </nav>
    </div>

    <div class="col bg-faded">
        <div class="tab-content p-3">
            <div class="tab-pane fade active show" id="tabInventoryCountSettings">
                <div class="row mb-3">
                    <label class="col-md-3 mb-3 col-form-label" for="rdChoiceInventoryCountEffect">What happens after user(s) conduct a physical inventory count?</label>
                    <div class="col-md-9">
                        <div class="form-check mb-3" style="font-size:20px;">
                            <input class="form-check-input" data-role="none" type="radio" name="rdChoiceInventoryCountEffect" value="on" checked id="rdInventoryCountAutomaticEffect">
                            <label class="form-check-label" for="rdInventoryCountAutomaticEffect">Inventory Count automatically affect balances</label>
                        </div>

                        <div class="form-check mb-3" style="font-size:20px;">
                            <input class="form-check-input" data-role="none" type="radio" name="rdChoiceInventoryCountEffect" value="off" id="rdInventoryCountNeedsApproval">
                            <label class="form-check-label" for="rdInventoryCountNeedsApproval">Inventory Count needs to be approved before balances can take effect</label>
                        </div>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-md-3 mb-3 col-form-label" for="selInventoryCountDefault">Default Inventory Count Option</label>
                    <div class="col-md-9">
                        <select data-role="none" id="selInventoryCountDefault" class="form-select fixed">
                            <option value="">No Default</option>
                            <option value="By Product Category">By Product Category</option>
                            <option value="By Product">By Product</option>
                            <option value="By Inventory-Count Products">By Inventory-Count Products</option>
                            <option value="By Bin Location">By Bin Location</option>
                        </select>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-md-3 mb-3 col-form-label" for="btnInventoryCountDefaultUom">Show Default Unit Measure</label>
                    <div class="col-md-9">
                        <div class="form-check form-switch" style="font-size:20px;">
                            <input class="form-check-input" data-role="none" type="checkbox" checked id="btnInventoryCountDefaultUom">
                        </div>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-md-3 mb-3 col-form-label" for="btnInventoryCountAllowUom">Allow based unit measure input</label>
                    <div class="col-md-9">
                        <div class="form-check form-switch" style="font-size:20px;">
                            <input class="form-check-input" data-role="none" type="checkbox" checked id="btnInventoryCountAllowUom">
                        </div>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-md-3 mb-3 col-form-label" for="selAllowDecimalinInventoryCount">Allow decimal points when inputting count quantities</label>
                    <div class="col-md-9">
                        <div class="form-check form-switch mb-3" style="font-size:20px;">
                            <input class="form-check-input" data-role="none" type="checkbox" id="selAllowDecimalinInventoryCount">
                        </div>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-md-3 mb-3 col-form-label" for="selDontAllowViewZreadWhenNoInventory">Do not allow the user to view their Z-read if they have not completed the inventory for the day:</label>
                    <div class="col-md-9">
                        <div class="form-check form-switch mb-3" style="font-size:20px;">
                            <input class="form-check-input" data-role="none" type="checkbox" id="selDontAllowViewZreadWhenNoInventory">
                        </div>
                    </div>
                </div>

                <div class="mt-4" style="text-align: center">
                    <a name="btnDoneInvCnt" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
                    <a name="btnSaveInventoryCountSettings" data-role="none" class="btn btn-success btn-lg">Save</a>
                </div>
            </div>

            <div class="tab-pane fade" id="tabInventoryCountSKURanking">
                <div class="row mb-3">
                    <div class="col">
                        <input data-role="none" type="text" class="form-control" id="txtSearchInventoryCountTrans" placeholder="Search here" />
                    </div>
                    <div class="col-auto">
                        <button data-role="none" id="btnInventoryCountAddSKU" class="btn btn-success">Add Product</button>
                    </div>
                </div>

                <div class="mb-3">You can use your cursor to drag and drop lists into the desired order.</div>

                <ul data-role="none" class="list-group" id="listInventoryCountSKURanking"></ul>

                <div class="mt-4" style="text-align: center">
                    <a name="btnDoneInvCnt" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
                    <a name="btnSaveInventoryCountSettings" data-role="none" class="btn btn-success btn-lg">Save</a>
                </div>
            </div>
        </div>
    </div>
</div>