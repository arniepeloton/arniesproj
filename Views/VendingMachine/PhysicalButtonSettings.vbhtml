<script>
    require(['bootstrapjs'], function () {
        require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
            require(['/Scripts/sources/VendingMachine/PhysicalButtonSettings.js?crc=' + assetversion])
        })
    })
</script>

<div class="row wrapper min-vh-100 flex-column flex-md-row">
    <div class="col-12 col-md-2 p-0 bg-light flex-shrink-1 mb-3">
        <nav class="navbar navbar-expand-md navbar-light bg-light align-items-start flex-md-column flex-row p-2">
            <a data-role="none" class="navbar-brand" href="#">Menu</a>
            <a data-role="none" href="#" class="navbar-toggler" data-toggle="collapse" data-target=".sidebarPhysicalButtonsSettings">
                <span class="navbar-toggler-icon"></span>
            </a>
            <div class="collapse navbar-collapse sidebarPhysicalButtonsSettings mt-2">
                <ul class="flex-column w-100 justify-content-between nav nav-pills">
                    <li><a class="nav-link active" role="button" data-role="none" data-toggle="tab" href="#tabVDMPBTemplate">Template</a></li>
                    <li><a class="nav-link" role="button" data-role="none" data-toggle="tab" href="#tabVDMPBFowst">Assigned Machine</a></li>
                </ul>
            </div>
        </nav>
    </div>

    <div class="col col-md bg-faded">
        <div class="tab-content">
            <div class="tab-pane fade active show" id="tabVDMPBTemplate">
                <div class="row mb-3">
                    <div class="col-md-2">Name: <span class="required">*</span></div>
                    <div class="col-md"><input data-role="none" type="text" class="form-control" id="txtVDMPBTemplateName" /></div>
                </div>

                <div class="row mb-3">
                    <div class="col-md-2">Description:</div>
                    <div class="col-md"><input data-role="none" type="text" class="form-control" id="txtVDMPBTemplateDescription" /></div>
                </div>

                <div class="row mb-3">
                    <div class="col-md-2">Model-B Template: <span class="required">*</span></div>
                    <div class="col-md">
                        <select id="selVDMModelBTemplate" data-role="none" class="form-select">
                            
                        </select>
                    </div>
                </div>

                <div class="mb-3 text-right">
                    <button data-role="none" class="btn btn-success" id="btnVDMAddPage">Add Page</button>
                </div>

                <div id="divVDMPages">

                </div>

                <div class="accordion mt-3 d-none" id="dvPageTemplate">
                    <div class="card">
                        <div class="card-header p-0">
                            <button class="btn btn-light btn-block text-left p-3 rounded-0 font-weight-bold lblPageName" data-role="none" data-toggle="collapse">
                                Page 1
                            </button>
                        </div>
                        <div class="collapse show">
                            <div class="card-body">
                                <div class="row mb-3">
                                    <div class="col-md-2">Page Name: <span class="required">*</span></div>
                                    <div class="col-md"><input data-role="none" type="text" class="form-control txtPageName" /></div>
                                </div>

                                <div class="row mb-3">
                                    <div class="col-md-2">Purpose: <span class="required">*</span></div>
                                    <div class="col-md">
                                        <select data-role="none" class="form-select selPagePurpose">
                                            <option value="">Please Select</option>
                                            <option value="Product Selection">Product Selection</option>
                                            <option value="Order Quantity Selection">Order Quantity Selection</option>
                                        </select>
                                    </div>
                                </div>

                                <div class="row mb-3">
                                    <div class="col-md-2">Text Content: <span class="required">*</span></div>
                                    <div class="col-md"><input data-role="none" type="text" class="form-control txtPageContent" /></div>
                                </div>

                                <table data-role="none" class="table table-light table-striped text-center tblPageSelectionButtons" style="table-layout:fixed;">
                                    <thead>
                                        <tr class="text-center align-top" style="font-size:14px;">
                                            <th>Selection Button</th>
                                            <th>Action</th>
                                            <th>Button Label</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        @*<tr>
                                            <td class="text-center">Button 1</td>
                                            <td><button data-role="none" class="btn btn-secondary border btn-block">Please Select</button></td>
                                            <td><input data-role="none" type="text" class="form-control" /></td>
                                        </tr>*@
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="tab-pane fade" id="tabVDMPBFowst">
                <div class="row mb-3 align-items-center">
                    <div class="col-sm">
                        This template will be assigned to the selected machines listed below.
                    </div>
                    <div class="col-sm-auto">
                        <button data-role="none" class="btn btn-success" id="btnVDMAddFOWST">Add Machine</button>
                    </div>
                </div>

                <div id="listVDMPBFowst">

                </div>
            </div>
        </div>
    </div>
</div>

<div class="fixed-bottom text-center pb-3" style="display:block; left:50%; transform: translate(-50%, 0); right:auto">
    <button id="btnVDMSCancelPage" data-role="none" class="btn btn-secondary btn-lg">Cancel</button>
    <button id="btnVDMSavePage" data-role="none" class="btn btn-success btn-lg">Save</button>
</div>