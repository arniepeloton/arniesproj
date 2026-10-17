<script>
    require([GetPathFormat('/Scripts/sources/Ecommerce/GeneralSettings.js?v=' + assetversion)]);
</script>

<div class="container-fluid">
    <div id="popupDropdownQty" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Dropdown Qty Interval</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <table style="width:100%;">
                        <tr>
                            <td>
                                List of Dropdown Quantities
                                <br />
                                <b>Note:</b> Alias will be shown to the customer.
                            </td>
                            <td style="text-align:right;"><button data-role="none" id="btnAddQtyChoice" class="btn btn-success">Add Choice</button></td>
                        </tr>
                    </table>
                    <br />
                    <table style="width:100%;">
                        <thead>
                            <tr style="text-align:center;">
                                <td>Qty Interval <span class="required">*</span></td>
                                <td>Alias</td>
                                <td></td>
                            </tr>
                        </thead>
                        <tbody id="tbodyQtyInterval"></tbody>
                    </table>
                    <div id="divQtyInputs">

                    </div>
                    <br /><br />
                    <table style="width:100%;">
                        <tr>
                            <td>The choices above will apply to the selected Product Categories below.</td>
                            <td style="text-align:right;"><button data-role="none" id="btnAddQtyCategory" class="btn btn-success">Add Category</button></td>
                        </tr>
                    </table>
                    <br />
                    <div id="divQtyChoicesCategories">

                    </div>
                </div>
                <div class="modal-footer">
                    <button data-role="none" type="button" id="btnOkQtyInterval" class="btn btn-success btn-block">Ok</button>
                    <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <ul class="nav nav-tabs" id="myTab" role="tablist">
        <li class="nav-item" role="presentation">
            <a class="nav-link active" id="generalsettings-tab" data-toggle="tab" href="#generalsettings" role="tab" aria-controls="generalsettings" aria-selected="true">Settings</a>
        </li>
        <li class="nav-item" role="presentation">
            <a class="nav-link" id="datasourcetarget-tab" data-toggle="tab" href="#datasourcetarget" role="tab" aria-controls="datasourcetarget" aria-selected="false">Data Source / Target</a>
        </li>
    </ul>

    <div class="tab-content">
        <div class="tab-pane fade show active p-3" id="generalsettings" role="tabpanel" aria-labelledby="generalsettings-tab">
            <div class="row mb-3">
                <label class="col-sm-3 mb-3" for="selIsAllowBuyersSelectFromVendor">Allow online buyers to select from vendor list?</label>
                <div class="col-sm-9">
                    <div class="form-check form-switch mb-3" style="font-size:20px;">
                        <input class="form-check-input" data-role="none" type="checkbox" id="selIsAllowBuyersSelectFromVendor">
                    </div>
                </div>
            </div>

            <div class="row mb-3" id="divForShowVendorName" style="display: none;">
                <label class="col-sm-3 mb-3" for="selIsVendorName">Show vendor name?</label>
                <div class="col-sm-9">
                    <div class="form-check form-switch mb-3" style="font-size:20px;">
                        <input class="form-check-input" data-role="none" type="checkbox" id="selIsVendorName">
                    </div>
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-3 mb-3" for="selAcceptDeliveryOrders">Do you accept delivery orders?</label>
                <div class="col-sm-9">
                    <div class="form-check form-switch mb-3" style="font-size:20px;">
                        <input class="form-check-input" data-role="none" type="checkbox" id="selAcceptDeliveryOrders">
                    </div>
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-3 mb-3" for="selAcceptPickUpOrders">Do you allow pick-up orders?</label>
                <div class="col-sm-9">
                    <div class="form-check form-switch mb-3" style="font-size:20px;">
                        <input class="form-check-input" data-role="none" type="checkbox" id="selAcceptPickUpOrders">
                    </div>
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-3 mb-3" for="txtCheckoutMessage">
                    Check-out Message: <br /><i style="font-weight:normal;">(Please write your message to your customers during their check-out order or leave blank if don't have any message)</i>
                </label>
                <div class="col-sm-9">
                    <textarea id="txtCheckoutMessage" class="form-control" placeholder="Enter here your message" style="height:200px; width:100%; resize:none;"></textarea>
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-3 mb-3" for="selAcceptPickUpOrders">
                    <span class="required">*</span> Ecommerce default FOWST: <br /><i style="font-weight:normal;">(all orders process by this e-commerce server is tagged with this FOWST location)</i>
                </label>
                <div class="col-sm-9">
                    <select id="selEcommerceDefaultFOWST" data-role="none" class="form-select"></select>
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-3 mb-3" for="txtEcommerceOnlineStoreName">
                    <span class="required">*</span> E-Commerce / Online Store Name:
                </label>
                <div class="col-sm-9">
                    <input type="text" id="txtEcommerceOnlineStoreName" data-role="none" class="form-control mb-3" placeholder="Enter here your e-commerce online store name" />
                    <div id="divEcommerceOnlineStoreUrl" class="mb-3"></div>
                    <span style="font-weight:normal;">
                        <b><i>Important Notes:</i></b> You can change the online store name at any time but it is best not to change this too often so that your customers can easily recall your online store name and they also do not get redirected to old, non-existing online store links.
                    </span>
                </div>
            </div>

            <h5 class="mb-4">Payment Method</h5>

            <div class="row mb-3">
                <label class="col-sm-3 mb-3" for="selPMPaypal">
                    Paypal
                </label>
                <div class="col-sm-9">
                    <div class="form-check form-switch mb-3" style="font-size:20px;">
                        <input class="form-check-input" data-role="none" type="checkbox" id="selPMPaypal">
                    </div>
                </div>
            </div>

            <h1 class="display-6 mb-4">Dropdown Quantity Interval</h1>

            <div class="row mb-3">
                <div class="col-8">
                    Use this tool to provide specific quantity options for the products in your E-Commerce website.
                </div>
                <div class="col-4 text-right">
                    <button data-role="none" id="btnAddDropdownQty" class="btn btn-success">Add Qty Options</button>
                </div>
            </div>

            <table data-role="none" class="table table-striped table-hover">
                <thead>
                    <tr>
                        <th>#</th>
                        <th>Qty Choices</th>
                        <th>Applied to Categories</th>
                        <td style="width:40px;"></td>
                    </tr>
                </thead>
                <tbody id="tbodyDropdownQtyInterval"></tbody>
            </table>

            <div class="mt-4" style="text-align: center">
                <a id="btnCancelEcommerceGeneralSettings" data-role="none" class="btn btn-secondary btn-lg mr-3">Back to Main</a>
                <a id="btnSaveEcommerceGeneralSettings" data-role="none" class="btn btn-success btn-lg">Save</a>
            </div>
        </div>

        <div class="tab-pane fade p-3" id="datasourcetarget" role="tabpanel" aria-labelledby="datasourcetarget-tab">
            <div class="row mb-3">
                <label class="col-sm-3 mb-3" for="txtEcommerceDataSourceIP">
                    <span class="required">*</span> Data Source Server IP:
                </label>
                <div class="col-sm-9">
                    <input type="text" id="txtEcommerceDataSourceIP" data-role="none" class="form-control mb-3" placeholder="Enter here your data source IP address" />
                    <span style="font-weight:normal;">
                        Please indicate the IP address (and any applicable port) of the data source server. The data source server is where libraries such as customer, product information, distribution center locations, etc. are centrally stored. The data source server is typically the main EAS server of your company.  This e-commerce server will get these data from the source server.
                        <br /><br />
                        <b><i>Important Notes:</i></b>
                        Please indicate an <i>external IP address</i> (and applicable port number if any) if your data source server is on a different AKA external network relative to this ecommerce server.
                        Please indicate an <i>internal IP address</i> (and applicable port number if any) if the data source server is on the same local network as this ecommerce server.
                    </span>
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-sm-3 mb-3" for="txtEcommerceOrderDataTargetIP">
                    <span class="required">*</span> E-Commerce Order Data Target Server IP:
                </label>
                <div class="col-sm-9">
                    <input type="text" id="txtEcommerceOrderDataTargetIP" data-role="none" class="form-control mb-3" placeholder="Enter here your e-commerce order data target IP address" />
                    <span style="font-weight:normal;">
                        Please indicate the target server IP address (and any applicable port) on this field. The target server is where order data from this e-commerce server will be sent, stored and processed. The target server is typically the main or regional EAS server of your company.
                        <br /><br />
                        <b><i>Important Notes:</i></b>
                        Please indicate an <i>external IP address</i> (and applicable port number if any) if your target server is on a different AKA external network relative to this ecommerce server.
                        Please indicate an <i>internal IP address</i> (and applicable port number if any) if your target server is on the same local network as this ecommerce server.
                    </span>
                </div>
            </div>

            <div id="divEcommerceGeneralSettingsGetDataLogContainer" style="height:0px; overflow-y: scroll; display: flex; flex-direction: column-reverse;">
                <div id="divEcommerceGeneralSettingsGetDataLog"></div>
            </div>

            <div class="mt-4" style="text-align: center">
                <a data-rel="back" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
                <a id="butEcommerceGeneralSettingsGetData" data-role="none" class="btn btn-success btn-lg">Get data from data source server</a>
            </div>
        </div>
    </div>
</div>