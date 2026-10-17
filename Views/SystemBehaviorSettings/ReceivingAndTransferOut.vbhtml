<script>
    require([GetPathFormat('/Scripts/sources/SystemBehaviorSettings/ReceivingAndTransferOut.js?v=' + assetversion)]);
</script>

<div class="row wrapper min-vh-100 flex-column flex-lg-nowrap flex-md-row">
    <div class="col-12 col-md-2 p-0 bg-light flex-shrink-1 mb-3">
        <nav class="navbar navbar-expand-md navbar-light bg-light align-items-start flex-md-column flex-row p-2">
            <a data-role="none" class="navbar-brand" href="#">Menu</a>
            <a data-role="none" href="#" class="navbar-toggler" data-toggle="collapse" data-target=".sidebarStockRecTrans">
                <span class="navbar-toggler-icon"></span>
            </a>
            <div class="collapse navbar-collapse sidebarStockRecTrans mt-2">
                <ul class="flex-column w-100 justify-content-between nav nav-pills">
                    <li><a class="nav-link active" role="button" data-role="none" data-toggle="tab" href="#tabStockRecTransSettings">General Settings</a></li>
                    <li><a class="nav-link" role="button" data-role="none" data-toggle="tab" href="#tabStockRecTransSKURanking">SKU Ranking</a></li>
                    <li><a class="nav-link" role="button" data-role="none" data-toggle="tab" href="#tabStockRecTransSKUMaxQty">Max Quantity</a></li>
                </ul>
            </div>
        </nav>
    </div>

    <div class="col bg-faded">
        <div class="tab-content p-3">
            <div class="tab-pane fade active show" id="tabStockRecTransSettings">
                <div class="row mb-3">
                    <label class="col-md-3 mb-3" for="rdBadCount">Allow the user to specify the total number of defective goods?</label>
                    <div class="col-md-9">
                        <div class="form-check form-switch" style="font-size:20px;">
                            <input class="form-check-input" data-role="none" type="checkbox" name="rdBadCount" checked id="rdBadCount">
                        </div>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-md-3 mb-3" for="rdRemindUserAboutTheDraftTransactionsStockRecTransfer">Remind the user about the draft transactions (mobile app):</label>
                    <div class="col-md-9">
                        <div class="form-check form-switch" style="font-size:20px;">
                            <input class="form-check-input" data-role="none" type="checkbox" name="rdRemindUserAboutTheDraftTransactionsStockRecTransfer" id="rdRemindUserAboutTheDraftTransactionsStockRecTransfer">
                        </div>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-md-3 mb-3" for="rdRecTransPackSizeDiscprepancy">Allow users to receive and transfer stocks with packing size discrepancies:</label>
                    <div class="col-md-9">
                        <div class="form-check form-switch" style="font-size:20px;">
                            <input class="form-check-input" data-role="none" type="checkbox" name="rdRecTransPackSizeDiscprepancy" id="rdRecTransPackSizeDiscprepancy">
                        </div>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-md-3 mb-3" for="txtSRDefaultSourceDocType">Stock Receiving default source document type:</label>
                    <div class="col-md-9">
                        <input data-role="none" type="text" id="txtSRDefaultSourceDocType" class="form-control" />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-md-3 mb-3 col-form-label" for="rdHideStocksQuantity">How do you want your receiving stocks to be presented?</label>
                    <div class="col-md-9">
                        <div class="form-check mb-3" style="font-size:20px;">
                            <input class="form-check-input" data-role="none" type="radio" name="rdChoiceHowToReceivedStocks" value="on" checked id="rdHideStocksQuantity">
                            <label class="form-check-label" for="rdHideStocksQuantity"> Hide expected quantities when receiving stocks </label>
                        </div>

                        <div class="form-check mb-3" style="font-size:20px;">
                            <input class="form-check-input" data-role="none" type="radio" name="rdChoiceHowToReceivedStocks" value="off" id="rdShowStocksQuantity">
                            <label class="form-check-label" for="rdShowStocksQuantity">Show expected quantities when receiving stocks</label>
                        </div>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-md-3 mb-3 col-form-label" for="rdWithinTheCompanyOrOrganization">What is your default Organization in 'Receive In/Transfer <i>From</i>' Stock Receiving and Stock Transfer?</label>
                    <div class="col-md-9">
                        <div class="form-check mb-3" style="font-size:20px;">
                            <input class="form-check-input" data-role="none" type="radio" name="rdWhatIsYourDefaultSourceOfSKU" value="on" checked id="rdWithinTheCompanyOrOrganization">
                            <label class="form-check-label" for="rdWithinTheCompanyOrOrganization">Internal Organization</label>
                        </div>

                        <div id="divInternalRecInDefaultSetting" class="ml-5" style="display:block; font-size:18px;">
                            <div class="form-check mb-3">
                                <input class="form-check-input" data-role="none" type="radio" name="rdReceiveInInternal" value="no" checked id="rdReceiveInInternalNoDefault">
                                <label class="form-check-label" for="rdReceiveInInternalNoDefault">No default internal outlet (let user choose)</label>
                            </div>

                            <div class="form-check mb-3">
                                <input class="form-check-input" data-role="none" type="radio" name="rdReceiveInInternal" value="set" id="rdReceiveInInternalSetDefault">
                                <label class="form-check-label" for="rdReceiveInInternalSetDefault">Set the device's designated FOWST automatically.</label>
                            </div>
                        </div>

                        <div class="form-check mb-3" style="font-size:20px;">
                            <input class="form-check-input" data-role="none" type="radio" name="rdWhatIsYourDefaultSourceOfSKU" value="off" id="rdThirdPartyCopanyOrOrganization">
                            <label class="form-check-label" for="rdThirdPartyCopanyOrOrganization">External Organization</label>
                        </div>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-md-3 mb-3 col-form-label" for="rdInternalOrganization">What is your default Organization in 'Receive From/Transfer <i>To</i>' Stock Receiving and Stock Transfer?</label>
                    <div class="col-md-9">
                        <div class="form-check mb-3" style="font-size:20px;">
                            <input class="form-check-input" data-role="none" type="radio" name="rdReceiveIn" value="on" checked id="rdInternalOrganization">
                            <label class="form-check-label" for="rdInternalOrganization">Internal Organization</label>
                        </div>

                        <div class="form-check mb-3" style="font-size:20px;">
                            <input class="form-check-input" data-role="none" type="radio" name="rdReceiveIn" value="off" id="rdExternalOrganization">
                            <label class="form-check-label" for="rdExternalOrganization">External Organization</label>
                        </div>

                        <div id="divExternalDefaultSetting" class="ml-5" style="display:none; font-size:18px;">
                            <div class="form-check mb-3">
                                <input class="form-check-input" data-role="none" type="radio" name="rdReceiveFromExternal" value="no" checked id="rdExternalNoDefault">
                                <label class="form-check-label" for="rdExternalNoDefault">No default external organization (let user choose)</label>
                            </div>

                            <div class="form-check mb-3">
                                <input class="form-check-input" data-role="none" type="radio" name="rdReceiveFromExternal" value="set" id="rdExternalSetDefault">
                                <label class="form-check-label" for="rdExternalSetDefault">Set default external organization</label>
                            </div>

                            <div id="divSRThirdParty" class="ml-5" style="display:none;">
                                <div class="mb-3">
                                    <label class="form-label">Select Supplier: <span class="required">*</span></label>
                                    <button data-role="none" id="btnSRThirdPartySupplier" class="btn btn-secondary btn-block">Please select</button>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label">Select Supplier's Outlet: <span class="required">*</span></label>
                                    <button data-role="none" id="btnSRThirdPartySupplierOutlet" class="btn btn-secondary btn-block">Please select</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="mt-4" style="text-align: center">
                    <a name="btnDone" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
                    <a name="btnSaveReceivingSettings" data-role="none" class="btn btn-success btn-lg">Save</a>
                </div>
            </div>

            <div class="tab-pane fade" id="tabStockRecTransSKURanking">
                <div class="row mb-3">
                    <div class="col">
                        <input data-role="none" type="text" class="form-control" id="txtSearchStockRecTrans" placeholder="Search here" />
                    </div>
                    <div class="col-auto">
                        <button data-role="none" id="btnStockRecTransAddSKU" class="btn btn-success">Add Product</button>
                    </div>
                </div>

                <div class="mb-3">You can use your cursor to drag and drop lists into the desired order.</div>

                <ul data-role="none" class="list-group" id="listStockRecTransSKURanking"></ul>

                <div class="mt-4" style="text-align: center">
                    <a name="btnDone" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
                    <a name="btnSaveReceivingSettings" data-role="none" class="btn btn-success btn-lg">Save</a>
                </div>
            </div>

            <div class="tab-pane fade" id="tabStockRecTransSKUMaxQty">
                <div class="mb-3">Set the maximum quantity (in base unit) of the SKUs that are allowed to be received or transferred.</div>

                <div class="row mb-3 align-items-center">
                    <div class="col-md">
                        <input data-role="none" type="text" id="txtSearchSKUMaxQty" class="form-control" placeholder="Search here" />
                    </div>
                    <div class="col-md text-right">
                        <div class="form-check form-switch form-check-inline">
                            <input class="form-check-input" type="checkbox" id="chbShowSKUWithMaxQty">
                            <label class="form-check-label" for="chbShowSKUWithMaxQty">Show SKUs with max quantity</label>
                        </div>
                    </div>
                </div>
                <table data-role="none" id="tblMaxQtyRecTrans" class="table table-light table-striped table-hover w-100">
                    <thead class="align-top text-center" style="font-size:14px;">
                        <tr>
                            <th class="fixed-col" style="width: 50px;">#</th>
                            <th class="fixed-col">Product No.</th>
                            <th class="fixed-col">Product Description</th>
                            <th class="fixed-col">Max Base Unit</th>
                            <th class="fixed-col">Max Quantity</th>
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>

                <div class="text-center">
                    <button data-role="none" id="btnSaveMaxQty" class="btn btn-success btn-lg">Save</button>
                </div>
            </div>
        </div>
    </div>
</div>