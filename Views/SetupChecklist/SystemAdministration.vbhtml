@Code
    ViewData("Title") = "System Administration"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code

@Section head

    @*<link rel="stylesheet" href="/Content/flatui-css-custom.css" />*@
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
    <link rel="stylesheet" href="/Content/rowReorder.dataTables.min.css" />
    <link rel="stylesheet" href="/Content/responsive.dataTables.min.css" />
    @*<link rel="stylesheet" href="/Content/bootstrap-custom.css" />
        <link rel="stylesheet" href="/Content/bootstrap.css" />*@

    @*<script src="/Scripts/sources/SetupChecklist/SystemAdministration.js"></script>*@
    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
                require(['/Scripts/sources/SetupChecklist/SystemAdministration.js?v=' + assetversion,
                    '/Scripts/sources/SetupChecklist/SystemAdministrationQueries.js?v=' + assetversion,
                    '/Scripts/sources/DashBoardsAndReports/flatui.js?v=' + assetversion,
                    '/Scripts/sources/SetupChecklist/NewPromptUser.js?v=' + assetversion])
            })
        })
    </script>

    @*<script src="/Scripts/sources/SetupChecklist/SystemAdministrationQueries.js"></script>*@
    @*<script src="/Scripts/sources/SetupChecklist/SystemAdministrationView.js"></script>*@


End Section

<style type="text/css">
    .dataTables_processing {
        /*Overwrite the Process Loading in datatables*/
        top: 5px !important;
        line-height: 40vh !important;
        height: 100% !important;
    }

    .ui-table-cell-label {
        display: none;
    }
</style>

<div class="d-flex position-absolute w-100 h-100 justify-content-center align-items-center section">
    <div class="spinner-border text-primary" role="status"></div>
</div>

<div id="pageSetUpCheckList" class="d-none section">
    <nav class="navbar navbar-dark bg-primary sticky-top" style="padding:0px !important;">
        <table style="width:100%; height:60px;">
            <tr>
                <td style="width:100px">
                    <button data-role="none" id="btnHomeSystemAdmin" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0 navbar-brand">
                        <span id="iconHome" class="bi-house-fill"></span> Home
                    </button>
                </td>
                <td class="OrganizationNameHeader pl-3 text-light" style="font-size:25px;"></td>
            </tr>
        </table>
        <br />
        <div class="row" style="margin:0px; padding:0.5em; width:100%; color:#FFF; background-color:#1969de; font-weight:bold;">
            <div class="BreadCrumbs">
                <div class="ItemsBreadCrumbs">
                    &nbsp;
                </div>
            </div>
        </div>
    </nav>

    <div id="popVDMModel" class="modal fade" data-backdrop="static" data-keyboard="false" tabindex="-1">
        <div class="modal-dialog modal-xl modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">VMC Board</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-sm-12 col-md">
                            @*<div class="mb-3 d-none">
                                    <label class="form-label" for="selVDMType">Type: <span class="required">*</span></label>
                                    <select data-role="none" id="selVDMType" class="form-select">
                                        <option value="Model A">Model A</option>
                                        <option value="Model B">Model B</option>
                                    </select>
                                </div>*@

                            <div class="mb-3">
                                <label class="form-label">Board Name: <span class="required">*</span></label>
                                <input data-role="none" type="text" class="form-control" id="txtVDMName" />
                            </div>

                            <div id="divModelA" subpage="pageVDMType" style="display:block;">
                                <div class="mb-3">
                                    <label class="form-label">Model ID: <span class="required">*</span></label>
                                    <select data-role="none" class="form-select" id="selVDMModelID">
                                        <option value="">Please select</option>
                                        <option value="0">Main Board</option>
                                        <option value="1">Daughter Board</option>
                                    </select>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label">No. Of Spring Coil Motors: <span class="required">*</span></label>
                                    <input data-role="none" type="number" class="form-control" id="txtVDMNoOfSpringCoilMotors" />
                                </div>

                                <div class="mb-3">
                                    <label class="form-label">No. Of Direct Connection Motors: <span class="required">*</span></label>
                                    <input data-role="none" type="number" class="form-control" id="txtVDMNoOfDirectConnMotors" />
                                </div>

                                <div class="mb-3">
                                    <label class="form-label">Board UUID: <span class="required">*</span></label>
                                    <input data-role="none" type="text" class="form-control" id="txtVDMUUID" />
                                </div>
                            </div>

                            <div class="mb-3">
                                <label class="form-label">Remarks:</label>
                                <textarea data-role="none" class="form-control" id="txtVDMDescription" style="resize:none; height:200px;"></textarea>
                            </div>

                            <div id="divModelB" subpage="pageVDMType" style="display:none;">
                                <div class="mb-3">
                                    <label class="form-label">No. Of Physical Selection Buttons: <span class="required">*</span></label>
                                    <input data-role="none" type="number" class="form-control" id="txtVDMNoOfPhySelButtons" />
                                </div>

                                <div class="mb-3">
                                    <label class="form-label">No. Of Physical Navigation Buttons: <span class="required">*</span></label>
                                    <input data-role="none" type="number" class="form-control" id="txtVDMNoOfPhyNavButtons" />
                                </div>

                                <div class="mb-3">
                                    <label class="form-label">No. Of Motor Actuators: <span class="required">*</span></label>
                                    <input data-role="none" type="number" class="form-control" id="txtVDMNoOfMotorActuators" />
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-auto col-form-label">Active:</label>
                                <div class="col">
                                    <div class="form-check form-switch mb-3" style="font-size:25px;">
                                        <input class="form-check-input" data-role="none" type="checkbox" id="selVDMActive">
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-12 col-md">
                            <div class="mb-3">
                                <label class="form-label font-weight-bold">Sensor List: <span class="required">*</span></label>
                                <div id="divSensorList"></div>
                            </div>

                            <div class="mb-3">
                                <label class="form-label font-weight-bold">Devices: <span class="required">*</span></label>
                                <div id="divDeviceList"></div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row justify-content-center">
                        <div class="col-auto">
                            <button data-role="none" type="button" id="btnCancelLibrary" class="btn btn-secondary btn-lg" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col-auto">
                            <button data-role="none" id="btnVDMSave" class="btn btn-success btn-lg">Save</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popVDMModelWithListing" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-xl modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Machine Model</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="row mb-3 align-items-center">
                        <label class="col-12 col-sm-3 col-md-2 col-form-label">Name: <span class="required">*</span></label>
                        <div class="col-12 col-sm-auto">
                            <input data-role="none" type="text" class="form-control" id="txtVMName" />
                        </div>
                    </div>

                    @*<div class="row mb-3 align-items-center">
                            <label class="col-12 col-sm-3 col-md-2 col-form-label">No. of Boards: <span class="required">*</span> (including the main board)</label>
                            <div class="col-12 col-sm-auto">
                                <input data-role="none" type="number" class="form-control" id="txtVDMNoOfBoards" />
                            </div>
                        </div>*@

                    <div class="row mb-3  align-items-center">
                        <label class="col-12 col-sm-3 col-md-2 col-form-label">Main Board: <span class="required">*</span></label>
                        <div class="col-12 col-sm-auto">
                            <select data-role="none" id="selMainBoard" class="form-select"></select>
                        </div>
                    </div>

                    <div class="mt-3 mb-3">Main Board Mapping and Labelling</div>
                    <div id="divMainBoardMapping"></div>

                    <div class="row mb-3 align-items-center">
                        <div class="col font-weight-bold">List of Daughter Boards</div>
                        <div class="col-auto"><button data-role="none" id="btnAddDaughterBoard" class="btn btn-light border"><span class="bi bi-plus-lg"></span> Add</button></div>
                    </div>

                    <div id="listDaughterBoard" class="accordion"></div>
                </div>
                <div class="modal-footer">
                    <div class="row justify-content-center">
                        <div class="col-auto">
                            <button data-role="none" type="button" class="btn btn-secondary btn-lg" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col-auto">
                            <button data-role="none" id="btnVMWithListingSave" class="btn btn-success btn-lg">Save</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popVDMPlanogramTemplate" data-backdrop="static" data-keyboard="false" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-fullwidth modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Planogram Template</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <input data-role="none" type="hidden" id="txtPTID" class="d-none" />
                    <div class="row gy-2 gx-3 mb-3">
                        <label class="col-12 col-md-2 col-form-label">Select Machine: <span class="required">*</span></label>
                        <div class="col-12 col-md-auto">
                            <select data-role="none" id="selPTMachine" class="form-select"></select>
                        </div>
                    </div>

                    <div class="row gy-2 gx-3 mb-3">
                        <label class="col-12 col-md-2 col-form-label">Template Name: <span class="required">*</span></label>
                        <div class="col-12 col-md-auto">
                            <input data-role="none" type="text" class="form-control" id="txtPTHBName" />
                        </div>
                    </div>

                    <div class="table-responsive">
                        <table id="tblPlanogramTemplate" data-role="none" class="table table-light table-striped table-hover">
                            <thead>
                                <tr class="text-center align-top">
                                    <th class="text-nowrap">Motor No.</th>
                                    <th class="text-nowrap">Board No.</th>
                                    <th class="text-nowrap">Board Type</th>
                                    <th class="text-nowrap">Dispenser Type</th>
                                    <th class="text-nowrap">Capacity</th>
                                    <th class="text-nowrap">Product</th>
                                    <th class="text-nowrap">Category Name</th>
                                </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row justify-content-center">
                        <div class="col-auto">
                            <button data-role="none" type="button" class="btn btn-secondary btn-lg" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col-auto">
                            <button data-role="none" id="btnPTSave" class="btn btn-success btn-lg">Save</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popVDMAuthorizer" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">VDM Authorizer</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label class="form-label" for="txtVDMAuthorizeEmployee">Employee Name: <span class="required">*</span></label>
                        <input data-role="none" type="text" class="form-control" id="txtVDMAuthorizeEmployee" readonly />
                    </div>

                    <div class="mb-3">
                        <label class="form-label" for="txtVDMAuthorizeEmployeePIN">PIN: <span class="required vdmauthnew">*</span> <i class="vdmauthnew">(Inform the employee of his desired PIN)</i></label>
                        <input data-role="none" type="password" class="form-control vdmauthnew" id="txtVDMAuthorizeEmployeePIN" maxlength="5" />
                        <span class="vdmauthold" style="display:none;"><a href="#" class="link-primary">Click here to change PIN</a></span>
                    </div>

                    <div class="mb-3">
                        <div>Access Rights: <span class="required">*</span></div>
                        <br />
                        <div id="divVDMAccessRights"></div>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-6">
                            <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col-6">
                            <button data-role="none" id="btnVDMAuthorizerSave" class="btn btn-success btn-block btn-lg">Save</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popNewOutlet" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Selected Customer(s)</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <span id="lblNewOutletMsg"></span>
                    <br />
                    <ol id="listNewOutlet" data-role="none" style="list-style:decimal"></ol>
                </div>
                <div class="modal-footer">
                    <button data-role="none" type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                    <button data-role="none" type="button" id="btnSaveNewOutlet" class="btn btn-success">Save</button>
                </div>
            </div>
        </div>
    </div>

    <div id="popItemModifierName" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Item Modifier Name</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div id="divErrorItemModifier" style="color:#F00; display:none;">
                        Name must not be blank
                        <br /><br />
                    </div>
                    <div>
                        Item Modifier Name
                        <input type="text" id="txtItemModifierNewName" />
                    </div>
                </div>
                <div class="modal-footer">
                    <button data-role="none" type="button" id="butCancelItemModifier" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                    <button data-role="none" type="button" id="butSaveItemModifier" class="btn btn-success">Add</button>
                </div>
            </div>
        </div>
    </div>

    <div id="panAddLibrary" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Please type name of <span class="lblPopUpHeaderLibraryName"></span></h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <input type="text" id="txtSearch" autocomplete="off" data-role="none" class="form-control" />
                    <br />
                    <div id="divNoResultsFound" style="display:none;">
                        <br />
                        Oops! We can’t find what you’re looking for. Make sure <br /> your search is correctly spelled.
                    </div>
                    <div style="max-height:200px; overflow:auto;">
                        <ul id="lstSearchFound" class="list-group"></ul>
                        <br />
                    </div>

                    <div id="btnAddToLibrary" data-dismiss="modal" class="divLibAddOption" style="display:none;">
                        <br />
                        If you’ve correctly spelled your search but can’t find it, <br /> you might want to <a class="custom-link">add a new <span class="lblAddLinkLibraryName"></span></a>.
                        <br /><br />
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-6">
                            <button data-role="none" type="button" id="btnCancelLibrary" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col-6">
                            <button data-role="none" type="button" id="btnSearchLibrary" class="btn btn-success btn-block">Search</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="panPopUploadEmployeesTemplate" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Employees Template</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <form id="formUploadEmployees" enctype="multipart/form-data" method="post">
                        <div style="display:none;"><input type="text" id="OrgID" name="OrgID" style="display:none;"></div>
                        <input type="file" id="emptemplateobjFile" name="objFile" data-role="none" class="form-control mb-3">
                    </form>
                    <div class="mb-3">
                        or <button data-role="none" name="btnEmployeesTemplateDownload" class="btn text-primary">click here to download template</button>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-6">
                            <button data-role="none" type="button" id="btnCancelLibrary" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col-6">
                            <button data-role="none" id="btnEmpTemplateUpload" class="btn btn-success btn-block btn-lg">Upload File</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="panPopUploadEmployeeStoreAssignments" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Store Assignments</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <textarea data-role="none" type="text" name="errmsg" style="margin:-1000px; position:absolute;"></textarea>
                    <form id="formStoreAssignments" enctype="multipart/form-data" method="post">
                        <input data-role="none" type="text" name="OrgID" class="d-none">
                        <input data-role="none" type="text" name="Action" class="d-none">
                        <input type="file" id="fileStoreAssignments" name="fileStoreAssignments" data-role="none" class="form-control mb-3">
                    </form>
                    <div class="mb-3">
                        or <button data-role="none" name="btnEmployeeStoreAssignmentsDownload" class="btn text-primary">click here to download template</button>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-6">
                            <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col-6">
                            <button data-role="none" id="btnUploadStoreAssignments" class="btn btn-success btn-block btn-lg">Upload File</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="divAddProductPopup" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="divAddProductTitle">Option</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body p-0">
                    <div id="listForAdding" style="display:none; visibility:hidden;">
                        <div class="list-group">
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="btnBarcodeSearch">
                                Searching a bar code or product description from the Mobiz public database
                            </a>
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="btnImportByExcel">
                                Importing data from an Excel file
                            </a>
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="btnSubscribe">
                                Subscribing to publicly available product lists
                            </a>
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="btnPIMWebForm">
                                Using Web Form
                            </a>
                        </div>
                    </div>

                    <div id="listForOption" style="display:none; visibility:hidden;">
                        <div class="list-group">
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="btnAdjustProdPrices">
                                Adjust Prices
                            </a>
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="butBomIssues">
                                Bill of Material Issues
                            </a>
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="btnPimPriceAdjust">
                                Change Prices Via Price Adjustment
                            </a>
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="butChangeProductAreas">
                                Change Product Areas or Sections
                            </a>
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="btnFowstProdList">
                                Create Approved Product Lists By Business Location
                            </a>
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="btnDeactivateAllSku">
                                Deactivate All Products
                            </a>
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="butDeduplicateProductNumbers">
                                Deduplicate Product Numbers
                            </a>
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="lnkProductList">
                                Download Company Product List
                            </a>
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="btnDownloadCompanyProductList">
                                Edit SKU Data In Bulk (Via Excel File)
                            </a>
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="btnExportListAsPlainText">
                                Export List As Plain Text
                            </a>
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="btnExportListAsCSV">
                                Export List As CSV
                            </a>
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="butPimMergeBrands">
                                Merge Duplicate Brands
                            </a>
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="btnActivateAllSku">
                                Reactivate Products
                            </a>
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="btnDeactivateDormantSku">
                                Settings for Deactivating Dormant SKUs
                            </a>
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="butErpToMobizExporter">
                                Update EAS PIM From ERP Product Information
                            </a>
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="butPimSalesCaching">
                                Offtake / Sales Data Caching for Supply Chain Management
                            </a>
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="butPimProductsAndContentsExcel">
                                Download Bill Of Materials
                            </a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="divPopReActivate" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Deativate all products</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <form @*action="PIMUpload/Index"*@ id="formuploadReactivation" enctype="multipart/form-data" method="post">

                        @*  <label>Upload Product Excel File :</label>*@
                        <div style="display:none;"><input type="text" id="OrgID" name="OrgID" style="display:none;"></div>
                        <input type="file" id="objFile" name="objFile" class="form-control mb-3">

                        <input type="submit" data-role="none" class="btn btn-success btn-block" value="Upload File">
                    </form>
                    <br />
                    <p>Download the PIM template below to see an example of the format required.</p>
                    <a data-role="none" class="btn btn-secondary btn-block" id="lnkDownloadReActivationTemplate">Download PIM Template</a>
                </div>
            </div>
        </div>
    </div>

    <div id="divPopPassword" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Deativate all products</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    Please enter your password.
                    <br />
                    <input type="password" data-role="none" class="form-control" />
                </div>
                <div class="modal-footer">
                    <button data-role="none" type="button" name="close" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                    <button data-role="none" type="button" name="ok" class="btn btn-success">Ok</button>
                </div>
            </div>
        </div>
    </div>

    <div id="popConfirmSupplierDeactivation" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Confirm Dialog</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    Please confirm by entering you password.
                    <br />
                    <input type="password" data-role="none" id="txtSupplierDeactivatePassword" class="form-control" />
                </div>
                <div class="modal-footer">
                    <button data-role="none" type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                    <button data-role="none" type="button" id="btnDeactivateSupplierNow" class="btn btn-success">Confirm Now</button>
                </div>
            </div>
        </div>
    </div>

    <div id="divAdjustProdPricesPopup" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Adjust Product Prices</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body p-0">
                    @*<button data-role="none" id="btnBackToOption" class="btn btn-secondary">
                            <span class="bi-arrow-left-short"></span> Back
                        </button>
                        <br />*@
                    <div class="list-group">
                        <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" id="btnSinglePrice">
                            Single Product Price Adjustment
                        </a>
                        <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" id="btnBulkPrice">
                            Bulk Price Adjustment
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="panOrgEmpMessage" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="TitleOrgEmp"></h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div id="MsgOrgEmp" style="font-weight:bold;"></div>
                </div>
                <div class="modal-footer">
                    <button data-role="none" type="button" id="divBtnCancel" class="btn btn-light" style="display:none;" data-dismiss="modal">Cancel</button>
                    <button data-role="none" type="button" id="btnReuploadPIM" class="btn btn-secondary">Re-upload</button>
                    <button data-role="none" type="button" id="btnOKOrgEmp" class="btn btn-success">Ok</button>
                </div>
            </div>
        </div>
    </div>

    <div id="panSearchLibrary" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="TitleLibrary"></h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div>
                        <span class="divBrowserContentTop"></span>
                        <br />
                    </div>

                    <input data-role="none" class="form-control" type="text" id="txtSearchKeyword" autofocus />

                    <div style="padding: 8px 15px 0px 15px; max-height:200px; overflow:auto;">
                        <ul id="lstOfLibrary" data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm">
                            @*<li>
                                    <a id="">
                                        asd
                                    </a>
                                </li>*@
                        </ul>
                        <br>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="panExternalServices" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">External Services</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div style="max-height:400px; overflow:auto;">
                        <div class="noBorderList" data-role="collapsible-set" data-corners="false" data-theme="a">
                            <div data-role="collapsible" data-collapsed="false">
                                <h3>&nbsp;</h3>
                                <ul data-role="listview">
                                    <li><a id="popupbtnZapatero" class="custom-btn-theme-c">Zapatero</a></li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="panOption" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Go To</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div style="max-height:400px; overflow:auto;">
                        <div class="noBorderList" data-role="collapsible-set" data-corners="false" data-theme="a">
                            <div data-role="collapsible">
                                <h3>Required Data</h3>
                                <ul data-role="listview">
                                    <li><a id="popupbtnOrganization" class="custom-btn-theme-c">Organization</a></li>
                                    <li><a id="popupbtnFowst" class="custom-btn-theme-c">Fowst Locations</a></li>
                                    <li><a id="popupbtnAuthDevices" class="custom-btn-theme-c">Authorized Devices</a></li>
                                    <li><a id="popupbtnReceiptSettings" class="custom-btn-theme-c">Receipt Settings</a></li>
                                    <li><a id="popupbtnSecurityRoles" class="custom-btn-theme-c">Security Roles</a></li>
                                    <li><a id="popupbtnEmployees" class="custom-btn-theme-c">Employees</a></li>
                                    <li><a id="popupbtnProducts" class="custom-btn-theme-c">PIM (Product Information Management)</a></li>
                                    <li><a id="popupbtnProductBrands" class="custom-btn-theme-c">Brands Library</a></li>
                                    <li><a id="popupbtnProductCategories" class="custom-btn-theme-c">Product Categories</a></li>
                                    <li><a id="popupbtnSalesReturn" class="custom-btn-theme-c">Sales Returns</a></li>
                                    <li><a id="popupbtnDiscountSettings" class="custom-btn-theme-c">Discount Settings</a></li>
                                    <li><a id="popupbtnPrinterRoles" class="custom-btn-theme-c">Printer Roles</a></li>
                                    <li><a id="popupbtnPrinter" class="custom-btn-theme-c">Printers</a></li>
                                    @*<li><a id="popupbtnPIM" class="custom-btn-theme-c">PIM (Product Information Management)</a></li>*@
                                </ul>
                            </div>

                            <div data-role="collapsible">
                                <h3>System Behavior</h3>
                                <ul data-role="listview">
                                    <li><a id="popupbtnQuantitySettings" class="custom-btn-theme-c">Decimals in Quantity Settings</a></li>
                                    <li><a id="popupbtnPOSSettings" class="custom-btn-theme-c">POS Settings</a></li>
                                    <li><a id="popupbtnSalesOrder" class="custom-btn-theme-c">Sales Order Settings</a></li>
                                    <li><a id="popupbtnReceivingAndTransferOut" class="custom-btn-theme-c">Receiving And Transfer-Out Settings</a></li>
                                    <li><a id="popupbtnInventoryCount" class="custom-btn-theme-c">Inventory Count Settings</a></li>
                                    <li><a id="popupbtnPrintSettings" class="custom-btn-theme-c">Print Settings</a></li>
                                    <li><a id="popupbtnGPSSettings" class="custom-btn-theme-c">GPS Settings</a></li>
                                </ul>
                            </div>

                            <div data-role="collapsible">
                                <h3>Optional Data</h3>
                                <ul data-role="listview">
                                    <li><a id="popupbtnHotButton" class="custom-btn-theme-c">Hot Button Settings</a></li>
                                    <li><a id="popupbtnGroupSettings" class="custom-btn-theme-c">Group Settings</a></li>
                                    <li><a id="popupbtnCustomer" class="custom-btn-theme-c">Customer / Client</a></li>
                                    <li><a id="popupbtnSupplier" class="custom-btn-theme-c">Supplier / Vendor</a></li>
                                    @*<li><a id="popupbtnShippers" class="custom-btn-theme-c">Shippers</a></li>*@
                                    <li><a id="popupbtnSalesTeam" class="custom-btn-theme-c">Sales Team</a></li>
                                    <li><a id="popupbtnItemModifiers" class="custom-btn-theme-c">Item Modifiers</a></li>
                                    @*<li><a id="popupbtnKnowledgeManagementAndQuestionaire" class="custom-btn-theme-c">Knowledge Management (temp placeholder)</a></li>*@
                                    <li><a id="popupbtnLoyaltyProgram" class="custom-btn-theme-c">Loyalty Program Settings</a></li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="panAddEmail" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Employee Email Address</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <span>There is no existing email address on record for <span class="peopleFirstName" style="font-weight:bold;"></span>.  Please enter his/her email address on the email entry field below:</span>

                    <br />
                    <br />
                    <input data-role="none" class="form-control" type="text" id="txtEmpEmailAdd" placeholder="Enter email address" />
                </div>
                <div class="modal-footer">
                    <button data-role="none" type="button" id="btnCancelEmail" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                    <button data-role="none" type="button" id="btnSaveAndSend" class="btn btn-success">Save and Send Employment Confirmation</button>
                </div>
            </div>
        </div>
    </div>

    <div id="panAddRawMaterials" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="BOMTitle">Content</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div id="divBomContent" style="display:block;">
                    <div class="modal-body">
                        Each <span id="spanParentUnit" style="font-weight:bold;"></span> of <span id="spanParentProduct" style="font-weight:bold;"></span> contains <span id="spanQty" style="font-weight:bold;"></span> <span id="spanChildUnit" style="font-weight:bold;"></span> of </span> <span id="spanChildProduct" style="font-weight:bold;"></span>:<br />

                        <br />
                        <input data-role="none" class="form-control mb-3" id="txtQty" placeholder="Qty" />
                        <select id="cbUnit" data-role="none" class="form-select">
                            <option value="pula">Loading data.</option>
                            <option value="berde">please wait</option>
                        </select>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCancelBomContent" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnSaveBomContent" class="btn btn-success btn-block">Save</button>
                            </div>
                        </div>
                    </div>
                </div>

                <div id="divBomUnitDefinition" style="display:none;">
                    <div class="modal-body">
                        <button data-role="none" id="btnBackBomRawMaterials" style="display:block;" class="btn btn-secondary">
                            <span class="bi-arrow-left-short"></span> Back
                        </button>
                        <br />
                        Each <span id="spanParentUnitUD" class="UnitDefinitionLabel"></span> of <span id="spanParentProductUD" class="UnitDefinitionLabel"></span> contains:<br />

                        <br />
                        <input data-role="none" class="form-control mb-3" id="txtQtyUD" placeholder="Qty" />
                        <select id="cbUnitUD" data-role="none" class="form-select">
                            <option value="pula">Loading data.</option>
                            <option value="berde">please wait</option>
                        </select>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCancelBomUnitDefinition" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnSaveBomUnitDefinition" class="btn btn-success btn-block">Save</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="panAddPriceLevel" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Price Level</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <input data-role="none" type="text" id="txtPriceLevelCustomID" style="display:none;" />
                    <div class="row mb-3">
                        <label class="col-sm-3 form-label" for="txtPriceLevelName">Price Level Name:</label>
                        <div class="col-sm-9">
                            <input type="text" data-role="none" class="form-control" id="txtPriceLevelName" />
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-sm-3 form-label" for="">Price Level Type:</label>
                        <div class="col-sm-9">
                            <div class="form-check">
                                <input class="form-check-input" data-role="none" type="radio" name="rad-pricelevel" value="Price" id="rad-pricelevel-price" checked>
                                <label class="form-check-label" for="rad-pricelevel-price">
                                    Price
                                </label>
                            </div>
                            <div class="form-check">
                                <input class="form-check-input" data-role="none" type="radio" name="rad-pricelevel" value="Cost" id="rad-pricelevel-cost">
                                <label class="form-check-label" for="rad-pricelevel-cost">
                                    Cost
                                </label>
                            </div>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-sm-3 form-label" for="selPriceLevelActive">Active:</label>
                        <div class="col-sm-9">
                            <div class="form-check form-switch" style="font-size:20px;">
                                <input class="form-check-input" data-role="none" type="checkbox" id="selPriceLevelActive">
                            </div>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-sm-9 form-label" for="chbPriceLevelAvailability">This Price Level is only available during specific date/time?</label>
                        <div class="col-sm-3">
                            <div class="form-check form-switch" style="font-size:20px;">
                                <input class="form-check-input" data-role="none" type="checkbox" id="chbPriceLevelAvailability">
                            </div>
                        </div>
                    </div>

                    <div id="divPLAvailability" style="display:none;">
                        <div class="row mb-3">
                            <label class="col-sm-3 col-form-label">
                                From:
                            </label>
                            <div class="col-sm-9">
                                <input data-role="none" class="form-control" type="datetime-local" id="txtPLFromDate" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-sm-3 col-form-label">
                                To:
                            </label>
                            <div class="col-sm-9">
                                <input data-role="none" class="form-control" type="datetime-local" id="txtPLToDate" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-6">
                            <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col-6">
                            <button data-role="none" type="button" id="btnPriceLevelSave" class="btn btn-success btn-block">Save</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="panAddSalesRepresentative" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Sales Representative</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <input data-role="none" type="text" id="txtSalesRepresentativeCustomID" style="display:none;" />
                    <div class="row mb-3">
                        <label class="col-sm-3 mb-3" for="txtSalesRepresentativeName">Price Level Name:</label>
                        <div class="col-sm-9">
                            <input type="text" data-role="none" class="form-control" id="txtSalesRepresentativeName" />
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-sm-3 mb-3" for="selSalesRepresentativeActive">Active ?</label>
                        <div class="col-sm-9">
                            <div class="form-check form-switch mb-3" style="font-size:20px;">
                                <input class="form-check-input" data-role="none" type="checkbox" id="selSalesRepresentativeActive">
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-6">
                            <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col-6">
                            <button data-role="none" type="button" id="butSalesRepresentativeSave" class="btn btn-success btn-block">Save</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popNewOutlet" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Modifier</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    Loading ...
                </div>
                <div class="modal-footer">
                    <button data-role="none" type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                    <button data-role="none" type="button" id="btnSaveItemModifier" class="btn btn-success">Save</button>
                </div>
            </div>
        </div>
    </div>

    <div id="popupPOSModifier" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="divTable" style="position: absolute; bottom: 0; top: 0; padding: 0; margin: 0;">
                        <div class="divTableRow">
                            <div class="divTableCell txt-middle" style="font-size:20px; padding-left:10px; overflow:auto; position:relative;">
                                <div id="txtModifierGroupName" style="position:absolute; top:10px;">

                                </div>
                            </div>
                            <div class="divTableCell txt-middle" style="width:40%; padding:0px 10px;">
                                <div class="input-group">
                                    <div class="input-group-btn">
                                        <button data-role="none" class="btn custom-btn-theme-c btn-sm">
                                            <span class="glyphicon glyphicon-minus"></span>
                                        </button>
                                    </div>
                                    <input data-role="none" type="text" class="form-control" aria-label="...">
                                    <div class="input-group-btn">
                                        <button data-role="none" class="btn custom-btn-theme-b btn-sm">
                                            <span class="glyphicon glyphicon-plus"></span>
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-body">
                    <div class="divTable" style="font-size:18px;">
                        <div class="divTableRow">
                            <div class="divTableCell" style="position:relative; width:40%; border-right:1px solid #CCC;">
                                <div style="position: absolute; bottom: 0px; overflow: auto; top: 0; right:0; left:0; padding:10px;">
                                    <ul id="listMenuIM" class="custom-list-menu txt-left disable">
                                        @*<li>
                                                <a id="" data-gotopage="notyetimplemented" class="ui-link">
                                                    AAA bbb ccc
                                                </a>
                                            </li>
                                            <li>
                                                <a id="" data-gotopage="divItemRebates" class="active-menu ui-link">
                                                    BBB ccc aaa
                                                </a>
                                            </li>
                                            <li>
                                                <a id="" data-gotopage="notyetimplemented" class="ui-link">
                                                    CCC aaa bbb
                                                </a>
                                            </li>*@
                                    </ul>
                                </div>
                            </div>
                            <div class="divTableCell" style="position:relative;">
                                <div style="position: absolute; bottom: 0px; overflow-x: hidden; overflow-y:auto; top: 0; right:0; left:0; padding:10px;">
                                    <ul class="custom-list">
                                        @*<li>
                                                <div class="row">
                                                    <div class="col-sm-4">Modifier No. 1</div>
                                                    <div class="col-sm-8">
                                                        <div class="input-group">
                                                            <div class="input-group-btn">
                                                                <button data-role="none" class="btn custom-btn-theme-c btn-sm">
                                                                    <span class="glyphicon glyphicon-minus"></span>
                                                                </button>
                                                            </div>
                                                            <input data-role="none" type="text" class="form-control" aria-label="...">
                                                            <div class="input-group-btn">
                                                                <button data-role="none" class="btn custom-btn-theme-b btn-sm">
                                                                    <span class="glyphicon glyphicon-plus"></span>
                                                                </button>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </li>*@
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button data-role="none" type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                    <button data-role="none" type="button" class="btn btn-success">Ok</button>
                </div>
            </div>
        </div>
    </div>

    <div id="panAddBarcodeType" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Barcode Type</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <input data-role="none" type="text" id="txtBarcodeTypeID" style="display:none;" />
                    <div class="row mb-3">
                        <label class="col-sm-4 mb-3 col-form-label" for="txtBarcodeTypeName">Barcode Type <span class="required">*</span>:</label>
                        <div class="col-sm-8">
                            <input type="text" data-role="none" class="form-control" id="txtBarcodeTypeName" />
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-sm-4 mb-3 col-form-label" for="txtBarcodeStickerDimensions">Sticker Dimensions:</label>
                        <div class="col-sm-8">
                            <input type="text" data-role="none" class="form-control" id="txtBarcodeStickerDimensions" />
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col" id="btnRemoveBarcodeType" style="display:none;">
                            <button data-role="none" type="button" class="btn btn-danger btn-block" data-dismiss="modal">Remove</button>
                        </div>
                        <div class="col">
                            <button data-role="none" type="button" id="btnSaveBarcodeType" class="btn btn-success btn-block">Save</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popupPIMLib" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title"><span class="lblPIMLib"></span> Name</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    Enter <span class="lblPIMLib"></span> name:
                    <br />
                    <input type="text" data-role="none" class="form-control" id="txtPIMLib" />

                </div>
                <div class="modal-footer">
                    <button data-role="none" type="button" id="btnSavePIMLib" class="btn btn-success btn-block">Save</button>
                    <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <div id="popupVouchers" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <form id="formVoucher" enctype="multipart/form-data" method="post">
                    <div class="modal-header">
                        <h5 class="modal-title">Vouchers</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label class="form-label">Select Voucher template to upload:</label>
                            <input data-role="none" type="file" class="form-control" id="fileVouchers" name="fileVouchers" accept=".xls,.xlsx" />
                        </div>

                        <div class="mb-3">
                            Or you may click here to <a data-role="none" id="btnDownloadVoucherTemplate" class="link-primary">download the voucher template</a>.
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-sm">
                                <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-sm">
                                <button data-role="none" type="submit" id="btnUploadVoucher" class="btn btn-success btn-block btn-lg">Upload</button>
                            </div>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    </div>

    <div id="popupPromoCampaign" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Promo Campaign</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <form id="formPromoCampaign" enctype="multipart/form-data" method="post">
                        <input data-role="none" type="hidden" class="form-control" name="CampaignID" />
                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label">Campaign Name: <span class="required">*</span></label>
                            <div class="col-md">
                                <input data-role="none" type="text" class="form-control" name="CampaignName" required />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label">Promo Message: <span class="required">*</span></label>
                            <div class="col-md">
                                <textarea class="form-control" style="height:100px;" name="PromoMessage" required></textarea>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label">Promo Start: <span class="required">*</span></label>
                            <div class="col-md">
                                <input data-role="none" type="date" class="form-control fixed" name="PromoStart" required />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label">Promo End: <span class="required">*</span></label>
                            <div class="col-md">
                                <input data-role="none" type="date" class="form-control fixed" name="PromoEnd" required />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3">Allow multiple promo codes per transaction: <span class="required">*</span></label>
                            <div class="col-md">
                                <div class="form-check form-switch">
                                    <input data-role="none" type="checkbox" class="form-check-input" id="IsMultiplePromoCode" />
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label">Upload Promo Code:</label>
                            <div class="col-md">
                                <div class="input-group">
                                    <input data-role="none" type="file" class="form-control" name="filePromoCode" accept=".xls,.xlsx" />
                                    <button data-role="none" id="btnPromoCodeDownloadTemplate" class="btn btn-light border">Download Template</button>
                                </div>
                            </div>
                        </div>

                        <div class="row align-items-center mb-3 mt-5">
                            <div class="col"><h3>Qualifying Products</h3></div>
                            <div class="col-auto"><button data-role="none" id="btnPromoCampaignAddProduct" class="btn btn-light border">Add Product</button></div>
                        </div>

                        <input data-role="none" class="form-control mb-3" type="text" placeholder="Search product here" />

                        <ol data-role="none" class="list-group" id="listPromoCampaignProducts"></ol>
                    </form>                    
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-sm">
                            <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col-sm">
                            <button data-role="none" type="submit" id="btnSavePromoCampaign" class="btn btn-success btn-block btn-lg">Save</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popupPIMLibInfo" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title"><span class="lblPIMLib"></span> Info</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="row mb-3 align-items-center">
                        <div class="col-sm-auto">
                            <span class="lblPIMLib"></span> Name:
                        </div>
                        <div class="col-sm">
                            <input data-role="none" class="form-control" id="txtPIMLibInfoName" />
                            Note: You can change the Category name into your desired name.
                        </div>
                    </div>

                    <h5>List of Products</h5>

                    <div class="table-responsive">
                        <table id="tabLibProductsList" data-role="none" class="table table-light table-striped table-hover">
                            <thead>
                                <tr class="text-center">
                                    <th style="width:50px;">#</th>
                                    <th>Product No.</th>
                                    <th>Product Description</th>
                                    <th><span class="lblPIMLib"></span></th>
                                    <th>Last Modified</th>
                                    <th>Modified By</th>
                                </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-sm">
                            <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col-sm">
                            <button data-role="none" type="button" id="btnApplyChangesPIMLib" class="btn btn-success btn-block btn-lg">Apply Changes</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popupPIMLibEditName" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title"><span class="lblPIMLib"></span> Name</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <input data-role="none" type="hidden" id="txtPIMLibID" />
                    <div class="mb-3">
                        You're about to change the <span class="lblPIMLib"></span> name from <i id="lblPIMLibOldName"></i> to <i class="lblPIMLibNewName"></i>.
                    </div>
                    <div class="mb-3">
                        The following SKU's Brand will automatically change to <b class="lblPIMLibNewName"></b> when you click the Save button.
                    </div>
                    <div style="max-height:300px; overflow:auto;">
                        <table data-role="none" class="table table-striped table-hover">
                            <thead>
                                <tr class="custom-theme-f txt-center">
                                    <th style="width:50px;">#</th>
                                    <th>Product No.</th>
                                    <th>Product Name</th>
                                </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col">
                            <button data-role="none" type="button" id="btnSavePIMLibEditName" class="btn btn-success btn-block">Save</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popupPIMLibDeacName" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Deactivate <span class="lblPIMLib"></span> Name</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        You're about to deactivate a <span class="lblPIMLib"></span> name <b id="lblPIMLibNameDeac"></b>
                    </div>
                    <div class="mb-3">
                        The following SKUs will be affected when you deactivate a <span class="lblPIMLib"></span>.
                    </div>
                    <div class="mb-3" style="max-height:300px; overflow:auto;">
                        <table data-role="none" class="table table-striped table-hover">
                            <thead>
                                <tr class="custom-theme-f txt-center">
                                    <th style="width:50px;">#</th>
                                    <th>Product No.</th>
                                    <th>Product Name</th>
                                </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>
                    <div id="divForAvailablePIM" style="display:block;">
                        <div class="mb-3">
                            You are required to transfer the SKUs above to another <span class="lblPIMLib"></span>. Please click the button below to select.
                        </div>

                        <button data-role="none" id="btnSelectAnotherPIMLibName" class="btn btn-info btn-block">Select <span class="lblPIMLib"></span></button>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col">
                            <button data-role="none" type="button" id="btnDeactivatePIMLibName" class="btn btn-success btn-block">Deactivate Now</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popupFranchiseeReqDetails" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Franchisee Request Details</h4>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div>
                        <h5 class="mb-3">Company Details</h5>

                        <div class="mb-4 row">
                            <label class="col-sm-4">
                                Company Name:
                            </label>
                            <div class="col-sm-8"><b id="lblFRCompanyName"></b></div>
                        </div>

                        <div class="mb-4 row">
                            <label class="col-sm-4">
                                Company TIN:
                            </label>
                            <div class="col-sm-8"><b id="lblFRCompanyTIN"></b></div>
                        </div>

                        <div class="mb-4 row">
                            <label class="col-sm-4">
                                Business Industry:
                            </label>
                            <div class="col-sm-8"><b id="lblFRBusinessIndustry"></b></div>
                        </div>

                        <div class="mb-4 row">
                            <label class="col-sm-4">
                                Company Email Address:
                            </label>
                            <div class="col-sm-8"><b id="lblFRCompanyEmailAddress"></b></div>
                        </div>

                        <div class="mb-4 row">
                            <label class="col-sm-4">
                                Company Mobile No.:
                            </label>
                            <div class="col-sm-8"><b id="lblFRCompanyMobileNo"></b></div>
                        </div>

                        <div class="mb-4 row">
                            <label class="col-sm-4">
                                Company Address:
                            </label>
                            <div class="col-sm-8"><b id="lblFRCompanyAddress"></b></div>
                        </div>
                    </div>

                    <div>
                        <h5 class="mb-3">Store Details</h5>

                        <div class="mb-4 row">
                            <label class="col-sm-4">
                                Store Name:
                            </label>
                            <div class="col-sm-8"><b id="lblFRStoreName"></b></div>
                        </div>

                        <div class="mb-4 row">
                            <label class="col-sm-4">
                                Business or Trade Name:
                            </label>
                            <div class="col-sm-8"><b id="lblFRStoreBusinessTradeName"></b></div>
                        </div>

                        <div class="mb-4 row">
                            <label class="col-sm-4">
                                Store Address:
                            </label>
                            <div class="col-sm-8"><b id="lblFRStoreAddress"></b></div>
                        </div>
                    </div>

                    <div>
                        <h5 class="mb-3">Administrator</h5>

                        <div class="mb-4 row">
                            <label class="col-sm-4">
                                Name:
                            </label>
                            <div class="col-sm-8"><b id="lblFRAdminName"></b></div>
                        </div>

                        <div class="mb-4 row">
                            <label class="col-sm-4">
                                Email Address:
                            </label>
                            <div class="col-sm-8"><b id="lblFRAdminEmailAddress"></b></div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Close</button>
                        </div>
                        <div class="col">
                            <button data-role="none" type="button" id="btnFRDisapprove" class="btn btn-danger btn-block">Reject</button>
                        </div>
                        <div class="col">
                            <button data-role="none" type="button" id="btnFRApprove" class="btn btn-success btn-block">Approve</button>
                        </div>
                        <div class="col" style="display:none;">
                            <button data-role="none" type="button" id="btnFRRepopulateData" class="btn btn-success btn-block">Repopulate Data</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popupFranchiseeReqLink" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Franchisee POS Registration Link</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        Please copy the link below and send it to your franchisees. This link is for Franchisee POS Registration.
                    </div>

                    <input data-role="none" class="form-control" id="txtFranchiseeRegistrationForm" readonly />
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Close</button>
                        </div>
                        <div class="col">
                            <button data-role="none" type="button" id="btnCopyFRLink" class="btn btn-success btn-block">Copy Link</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popupRegionalGroupSettings" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Regional Group Information</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label class="form-label">Name:</label>
                        <input data-role="none" type="text" class="form-control" />
                    </div>

                    <div class="row mb-3">
                        <div class="col-auto">Active?</div>
                        <div class="col">
                            <div class="form-check form-switch">
                                <input data-role="none" type="checkbox" class="form-check-input" checked />
                            </div>
                        </div>
                    </div>

                    <div class="row mb-3 align-items-center">
                        <div class="col font-weight-bold">List of Groups</div>
                        <div class="col-auto"><button data-role="none" class="btn btn-success" name="btnAddGroup">Add Group</button></div>
                    </div>

                    <ul data-role="none" class="list-group"></ul>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Close</button>
                        </div>
                        <div class="col">
                            <button data-role="none" type="button" name="btnSave" class="btn btn-success btn-block btn-lg">Save</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popupFOWSTUpload" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Import FOWSTs by Excel file </h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <form id="formFOWSTUpload" enctype="multipart/form-data">
                        <div class="mb-3">
                            <label class="form-label" for="selFOWSTUploadTradeName">Business Trade Name:</label>
                            <select data-role="none" class="form-select" name="tradename" id="selFOWSTUploadTradeName"></select>
                        </div>
                        <div class="mb-3">
                            <label class="form-label" for="fileFOWSTUpload">Select file:</label>
                            <input data-role="none" type="file" id="fileFOWSTUpload" class="form-control" accept=".xls,.xlsx" name="file">
                        </div>
                    </form>

                    <div class="mb-3">
                        or <button data-role="none" name="btnFOWSTTemplateDownload" class="btn text-primary">click here to download template</button>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Close</button>
                        </div>
                        <div class="col">
                            <button data-role="none" type="button" name="btnFOWSTUpload" class="btn btn-success btn-block btn-lg">Upload</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popupFOWSTFranchisesStores" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-fullscreen-lg-down">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title"></h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <h5>List of employees assigned to this franchise's store</h5>
                    <table id="tblFranchisesStoresAssignedEmp" data-role="none" class="table table-striped table-hover w-100">
                        <thead class="table-light">
                            <tr class="text-center align-top" style="font-size:14px;">
                                <th class="fixed-col" style="width:50px;">#</th>
                                <th class="fixed-col">Employee Name</th>
                                <th class="fixed-col">Position</th>
                                <th class="fixed-col" style="width:50px;"></th>
                            </tr>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Close</button>
                        </div>
                        <div class="col">
                            <button data-role="none" type="button" id="btnRemoveFranchisesStoresAssignedEmp" class="btn btn-success btn-block btn-lg">Save</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="divContent" class="container-fluid mb-5">

        @*<button id="btnPrintPOCSlip" data-role="none" class="btn custom-btn-theme-d btn-less-width">Sample Print</button>*@
        <br />
        <div id="divSystemAdDashboard" style="display:block;">
            <div id="divForRequiredData" class="mb-4">
                <div class="mb-4">
                    <h1 class="display-6">Required Data</h1>
                    <h5>
                        Please make sure that you provide the correct information on the things below.  Mobiz will not work properly without these required setup.
                    </h5>
                </div>
                @*<div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div class="custom-theme-c border-theme-c" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <div style="text-align:left; margin-top:28px;"><span>Library Settings</span></div>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div class="custom-theme-c border-theme-c" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <div style="text-align:left; margin-top:28px;"><span>Status and Last Update</span></div>
                                </div>
                            </div>
                        </div>
                    </div>*@
                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnSetupOrganization" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Organization</b></h5>
                            <div>Click here to add more information about your organization. This information may be published to an online business directory, used for system-generated General Information Sheet (GIS) for the SEC or DTI, and/or other value-added services in the future. Your consent must be given before Mobiz publishes any of these information to the public.</div>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img id="imgOrg" src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblOrgLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnSetupOrganizationFOWST" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>FOWST Locations</b></h5>
                            <div>
                                At least one (1) record of a FOWST (Factory, Office, Warehouse, Store or Truck) is needed.
                                Each FOWST location keeps track of its own inventory movements and balances. If you want a
                                location to have its own inventory ledger (of movements and balances), you must add a FOWST
                                location record for it.
                            </div>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img id="imgFOWST" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblFOWSTCount">0</div>
                            <div id="lblFOWSTLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnSetupDevice" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Authorized Devices</b></h5>
                            <div>
                                At least one (1) device is needed to be registered for use in your company.
                            </div>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img id="imgDevice" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblDeviceCount">0</div>
                            <div id="lblDeviceLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnSetupReceiptSettings" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Receipt Settings</b></h5>
                            <div>
                                Fill up the Receipt Settings Info for each FOWST.
                            </div>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img id="imgReceiptSettings" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblReceiptSettingsCount">0</div>
                            <div id="lblReceiptSettingsLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnSetupSecurity" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Access Rights</b></h5>
                            <div>
                                Add or edit existing roles in your organization for fine-tuning staff's access rights to Mobiz apps.
                            </div>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img id="imgSecurityAccess" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblSecurityAccessCount">0</div>
                            <div id="lblSecurityAccessLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnSetupEmployee" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Employees / Users</b></h5>
                            <div>
                                Each employee must be assigned his or her own user name/password and Security Role (cashier, manager, etc.) before he/she can use Mobiz apps.
                            </div>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img id="imgEmployeeAccess" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblEmployeeCount">0</div>
                            <div id="lblEmployeeLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnSetupProducts" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>PIM <span style="font-size:14px;">(Product Information Management)</span></b></h5>
                            <div>
                                At least one (1) product is needed. You may add your products via Excel upload or you may choose from the publicly available product database of Mobiz.
                            </div>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img id="imgProduct" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblProductCount">0</div>
                            <div id="lblProductLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnSetupProductBrands" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Brands Library</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblProductBrandsCount">0</div>
                            <div id="lblProductBrandsLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnSetupProductCategories" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Product Categories</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblProductCategoriesCount">0</div>
                            <div id="lblProductCategoriesLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnSalesReturnSettings" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Sales Return Settings</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblSalesReturnSettingsLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnDiscountSettings" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Discount Settings</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblDiscountSettingsLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnPrinterRoles" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Printer Roles</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblPrinterRolesLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnPrinter" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Printers</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblPrintersLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnGeneralSettings" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>General Settings</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                @*<div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="btnPrinterSetup" class="btn custom-btn-theme-c border-theme-c" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">Device Printer Setup</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="" class="custom-theme-c border-theme-c" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px; height:62px;">
                                    <div><img id="" src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="btnPIM" class="btn custom-btn-theme-c border-theme-c" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;"><span>PIM <span style="font-size:14px;">(Product Information Management)</span></span></p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="" class="custom-theme-c border-theme-c" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px; height:62px;">
                                    <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label>(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>*@
            </div>

            <div id="divForSystemBehavior" class="mb-4">
                <div class="mb-4">
                    <h1 class="display-6">System Behavior</h1>
                    <h5>
                        You can make Mobiz Complete Retail more suitable to your organization's work style by specifying some behaviors in these modules.
                    </h5>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnQuantitySetting" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Decimals in Quantity Settings</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblQuantitySettingsLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnPOSSetting" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>POS Settings</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblPOSSettingsLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnSalesOrderSetting" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Sales Order Settings</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblSalesOrderSettingsLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnReceivingAndTransferOutSetting" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Receiving And Transfer-Out Settings</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblRecTransSettingsLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnInventoryCountSetting" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Inventory Count Settings</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblInventoryCountSettingsLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnPurchaseOrderSetting" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Purchase Order Settings</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblPurchaseOrderSettingsLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3 d-none">
                    <div class="col-9 pr-0">
                        <button id="butErpSettings" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>ERP Settings</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblErpSettingsLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnDailyBeginningBalanceSetting" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Daily Beginning Balance Settings</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblDailyBegBalSettingsLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3 d-none">
                    <div class="col-9 pr-0">
                        <button id="btnTableViewSetting" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Table View Settings</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblTableViewSettingsLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="butProductMakerSettings" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Product Maker Settings</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblProductMakerSettingsLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="butPrintSettings" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Print Settings</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblPrintSettingsLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnGPSSettings" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>GPS Settings</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblGPSSettingsLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                @*<div class="row mb-3">
                        <div class="col-9 pr-0">
                            <button id="btnModifierSetting" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                                <h5 class="mb-3"><b>Modifiers</b></h5>
                            </button>
                        </div>
                        <div class="col-3 pl-0">
                            <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                                <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                <div id="lblModifierLastUpdated">(08/20/2014)</div>
                            </button>
                        </div>
                    </div>*@
            </div>

            <div id="divForOptionalData" class="mb-4">
                <div class="mb-4">
                    <h1 class="display-6">Optional Data</h1>
                    <h5>
                        Add these optional data to enhance the functions of Mobiz Complete Retail's different modules.
                    </h5>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnHotButtons" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Hot Buttons Settings</b></h5>
                            <div>
                                Predetermine what products and services are available to your cashier and sales team at a glance.
                                Hot buttons are programmable keys (by authorized administrators at the office) that act as short
                                cuts to products.  No need to type or barcode scan products when they are accessible in just a
                                few taps.
                            </div>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblHotButtonCount">0</div>
                            <div id="lblHotButtonLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnGroupSettings" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Group Settings</b></h5>
                            <div>
                                You can group your stores and assign a management and support team to each group. Group sales
                                reports are viewable only to authorized persons (or teams) and quick replenishment of products
                                are handled easily by the support team through an online order replenishment dashboard.
                            </div>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblGroupSettingsCount">0</div>
                            <div id="lblGroupSettingsLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnRegionalGroupSettings" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Regional Group Settings</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblRegionalGroupSettingsCount">0</div>
                            <div id="lblRegionalGroupSettingsLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnSetupCustomer" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Customer / Client</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img id="imgCustomer" src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblCustomerCount">0</div>
                            <div id="lblCustomerLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnSetupSupplier" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Supplier / Vendor</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img id="imgSupplier" src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblSupplierCount">0</div>
                            <div id="lblSupplierLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                @*<div class="row mb-3">
                        <div class="col-9 pr-0">
                            <button id="" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                                <h5 class="mb-3"><b>Shippers</b></h5>
                            </button>
                        </div>
                        <div class="col-3 pl-0">
                            <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                                <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                <div id="lblShipperLastUpdated">(08/20/2014)</div>
                            </button>
                        </div>
                    </div>*@

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnSalesTeam" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Sales Team</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblSalesTeamCount">0</div>
                            <div id="lblSalesTeamLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnItemModifiers" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Item Modifiers</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblItemModifiersCount">0</div>
                            <div id="lblItemModifiersLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3 d-none">
                    <div class="col-9 pr-0">
                        <button id="btnExternalServices" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>External Services</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblExternalServicesCount">0</div>
                            <div id="lblExternalServicesLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnPaymentTerms" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Payment Terms</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblPaymentTermsCount">0</div>
                            <div id="lblPaymentTermsLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnAccountAdjustments" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Account Adjustments</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblAccountAdjustmentsCount">0</div>
                            <div id="lblAccountAdjustmentsLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnPriceLevel" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Price Levels</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblPriceLevelCount">0</div>
                            <div id="lblPriceLevelLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="butSysAdminSalesRep" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Sales Representatives</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblSalesRepCount">0</div>
                            <div id="lblSalesRepLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnProprietaryBarcodeTypes" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Proprietary Barcode Types</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblBarcodeTypesCount">0</div>
                            <div id="lblBarcodeTypesLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="butVouchers" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Vouchers</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblVouchersCount">0</div>
                            <div id="lblVouchersLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="butPromoCode" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Promo Code</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblPromoCodeCount">0</div>
                            <div id="lblPromoCodeLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3 d-none">
                    <div class="col-9 pr-0">
                        <button id="butUpdateServerIp" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Update Server IP</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblServerIPCount">0</div>
                            <div id="lblServerIPLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3 d-none">
                    <div class="col-9 pr-0">
                        <button id="butWebPosSettings" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>WebPOS Settings</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div>&nbsp;</div>
                            <div>&nbsp;</div>
                        </button>
                    </div>
                </div>
            </div>

            <div id="divForAccounting">
                <div class="mb-4">
                    <h1 class="display-6">Accounting Settings</h1>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnRevenueCenters" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Revenue Centers</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblRevenueCentersCount">0</div>
                            <div id="lblRevenueCentersLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnCostCenters" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Cost Centers</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblCostCentersCount">0</div>
                            <div id="lblCostCentersLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3 d-none">
                    <div class="col-9 pr-0">
                        <button id="btnChartOfAccounts" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Chart of Accounts</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblChartOfAccountsCount">0</div>
                            <div id="lblChartOfAccountsLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>
            </div>

            <div id="divForEcommerce" class="d-none">
                <div class="mb-4">
                    <h1 class="display-6">E-Commerce</h1>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnEcommerceGeneralSettings" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>General Settings</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblEcommerceGeneralCount">0</div>
                            <div id="lblEcommerceGeneralLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnEcommerceVendorPortalSettings" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Vendor Portal Settings</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblEcommerceVendorCount">0</div>
                            <div id="lblEcommerceVendorLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnOnlineStoreCategories" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Online Store Categories</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblOnlineStoreCategoriesCount">0</div>
                            <div id="lblOnlineStoreCategoriesLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnEcommerceBannerSettings" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Banner Settings</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblBannerSettingsCount">0</div>
                            <div id="lblBannerSettingsLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>
            </div>

            <div id="divForVendingMachine">
                <div class="mb-4">
                    <h1 class="display-6">Vending Machine</h1>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnVDMGeneralSettings" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>VDM General Settings</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="">0</div>
                            <div id="">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnVDMModels" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>VMC Boards</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="">0</div>
                            <div id="">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnVMCListing" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>VM Model with Controller Listing</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="">0</div>
                            <div id="">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnVMPlanogramTemplate" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>VM Planogram Template</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="">0</div>
                            <div id="">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnVDMAuthorizers" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>VDM Authorizers</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="">0</div>
                            <div id="">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3 d-none">
                    <div class="col-9 pr-0">
                        <button id="btnVDMPhysicalButtonSettings" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>VDM Physical Button Settings</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="">0</div>
                            <div id="">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3 d-none">
                    <div class="col-9 pr-0">
                        <button id="btnVDMProductSelectionGuidesSettings" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>VDM Product Selection Guides</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="">0</div>
                            <div id="">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnVDMRiceDispensingGuidesSettings" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>VDM Dispensing Guides</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="">0</div>
                            <div id="">(08/20/2014)</div>
                        </button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="btnVDMAdsSettings" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>VDM Ads</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="">0</div>
                            <div id="">(08/20/2014)</div>
                        </button>
                    </div>
                </div>
            </div>

            <div id="divForSystemAdministratorReports">
                <div class="mb-4">
                    <h1 class="display-6">System Administrator Reports</h1>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="butSystemAdministratorReportsFlushingReport" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Flushing Report</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblFlushingReportCount">0</div>
                            <div id="lblFlushingReportLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>
            </div>

            <div id="divForSystemAdministratorQS4VVLQPY2">
                <div class="mb-4">
                    <h1 class="display-6">Automated Data Management</h1>
                </div>

                <div class="row mb-3">
                    <div class="col-9 pr-0">
                        <button id="butSystemAdministratorAutoDataArchiving" data-role="none" class="btn btn-light btn-block rounded-0 border text-left" style="height:100%;">
                            <h5 class="mb-3"><b>Auto Data Archiving</b></h5>
                        </button>
                    </div>
                    <div class="col-3 pl-0">
                        <button data-role="none" class="btn btn-light btn-block rounded-0 border" style="height:100%;">
                            <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                            <div id="lblAutoDataArchivingCount">0</div>
                            <div id="lblAutoDataArchivingLastUpdated">(08/20/2014)</div>
                        </button>
                    </div>
                </div>
            </div>
        </div>

        <div id="divSecond" style="display:none;">
            <ul class="nav nav-tabs mb-3" id="divSecondTabs" role="tablist" style="display:none;">
                <li class="nav-item" role="presentation">
                    <button data-role="none" class="nav-link active" id="tabListOfData" data-toggle="tab" data-target="#listofdata" type="button" role="tab" aria-controls="home" aria-selected="true"></button>
                </li>
                <li class="nav-item" role="presentation">
                    <button data-role="none" class="nav-link" id="tabFranchiseerequest" data-toggle="tab" data-target="#franchiseerequest" type="button" role="tab" aria-controls="profile" aria-selected="false">Franchisee Requests <span id="ctrnewfranchiseereq" class="badge bg-danger"></span></button>
                </li>
                <li class="nav-item" role="presentation">
                    <button data-role="none" class="nav-link" id="tabFranchisesStores" data-toggle="tab" data-target="#franchisesstores" type="button" role="tab" aria-controls="profile" aria-selected="false">Franchises' Stores</button>
                </li>
                <li class="nav-item" role="presentation">
                    <button data-role="none" class="nav-link" id="tabSortingItemModifiers" data-toggle="tab" data-target="#sortingitemmodifier" type="button" role="tab" aria-controls="profile" aria-selected="false">Sort Item Modifiers</button>
                </li>
                <li class="nav-item" role="presentation">
                    <button data-role="none" class="nav-link" id="tabSortingProductCategories" data-toggle="tab" data-target="#sortproductcategories" type="button" role="tab" aria-controls="profile" aria-selected="false">Sort Order</button>
                </li>
            </ul>

            <div class="tab-content">
                <div class="tab-pane fade show active" id="listofdata">
                    <div class="row gy-2 mb-4">
                        <div class="col-md-auto">
                            <div id="divOptionStatus" style="display:block;">
                                <div class="row align-items-center">
                                    <label class="col-lg-auto col-auto control-label">Show:</label>
                                    <div class="col-lg-auto col-auto">
                                        <select id="selOptionStatus" data-role="none" class="form-select">
                                            <option value="1">Active</option>
                                            <option value="0">Inactive</option>
                                        </select>
                                    </div>
                                    <label class="col-lg-auto col-auto control-label colFOWSStatus">Status:</label>
                                    <div class="col-lg-auto col-auto colFOWSStatus">
                                        <select id="selOptionFOWSTStatus" data-role="none" class="form-select">
                                            <option value="">All</option>
                                            <option value="Store and device are operational.">Store and device are operational.</option>
                                            <option value="Store is operational. No device is deployed yet.">Store is operational. No device is deployed yet.</option>
                                            <option value="Store is operational. Device is on hold.">Store is operational. Device is on hold.</option>
                                            <option value="Store is operational. Waiting for device to arrive.">Store is operational. Waiting for device to arrive.</option>
                                            <option value="Store is operational. No device available. Stolen.">Store is operational. No device available. Stolen.</option>
                                            <option value="Store is operational. No device available. Broken.">Store is operational. No device available. Broken.</option>
                                            <option value="Store is permanently closed.">Store is permanently closed.</option>
                                            <option value="Store is temporarily closed for holiday.">Store is temporarily closed for holiday.</option>
                                            <option value="Store is temporarily closed for company activity.">Store is temporarily closed for company activity.</option>
                                            <option value="Others">Others</option>
                                        </select>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-md">
                            <form method="post" id="formsearchdesktop">
                                <input type="text" data-role="none" placeholder="Search here" class="txtSearchdesktop form-control">
                            </form>
                        </div>

                        <div class="col-md-auto">
                            <div id="divAddLibrary" style="display:block; float:right">
                                <button id="btnAddLibrary" data-role="none" class="btn btn-success">
                                    Add <span class="lblAddLibraryName"></span>
                                </button>
                            </div>
                            <div id="divOptionAddProduct" style="display:none; float:right; margin-right:5px;">
                                <button id="btnOptionAddProduct" data-role="none" class="btn btn-secondary">
                                    Options
                                </button>
                            </div>
                            <div id="divAddFranchisee" style="display:none; float:right; margin-right:5px;">
                                <button id="btnAddFranchisee" data-role="none" class="btn btn-secondary">
                                    Add Franchisee
                                </button>
                            </div>
                            <div style="float:right; margin-right:5px;">
                                <button id="btnExportToCSV" data-role="none" class="btn btn-secondary">
                                    Export to CSV
                                </button>
                            </div>
                            <div id="divWorkSchedule" style="display:none; float:right; margin-right:5px;">
                                <div class="btn-group">
                                    <button data-role="none" class="btn btn-secondary dropdown-toggle" data-toggle="dropdown" aria-expanded="false">
                                        Options
                                    </button>
                                    <ul class="dropdown-menu shadow">
                                        <li><button class="dropdown-item" data-role="none" id="btnWorkSchedule">Work Schedule</button></li>
                                        <li><button class="dropdown-item" data-role="none" id="btnEmployeeStoreAssigments">Store Assignments</button></li>
                                    </ul>
                                </div>
                            </div>
                            <div style="clear:both;"></div>
                        </div>
                    </div>
                    <div id="divInfoForFowstProdList" style="display:none;" class="mb-4">
                        <span style="color:#F00">*</span> Click on the Fowst Location to add an SKU.
                    </div>
                    @*<div id="divDownloadFilePIM" style="display:none;" class="mb-4">
                            <label id="lblProductRecordcount"></label>
                            <a id="lnkProductList" style="cursor:pointer; font-size:16px;"> Click here to download Excel file to view full company product list.</a>
                        </div>*@
                    <form id="Supplierformupload" enctype="multipart/form-data" method="post" style="display:none;">
                        <div style="display:none;"><input type="text" id="txtSupplierUploadExcelOrgID" name="OrgID" style="display:none;"></div>
                        <input type="file" id="SupplierobjFile" name="objFile">
                        <input data-role="none" type="text" name="IsCustomer" value="0" style="display:none;" />
                        <input type="submit" id="btnSupplierUploadExcel" value="Upload File">
                    </form>
                    <div class="table-responsive">
                        <table id="tblSysAdminDataTable" data-role="none" class="table table-striped table-hover">
                            <thead class="table-light align-top text-center" style="font-size:14px;">
                                <tr id="tblHeaderDT"></tr>
                            </thead>
                            <tbody id="tblBodyDT"></tbody>
                        </table>
                    </div>
                </div>
                <div class="tab-pane fade" id="franchiseerequest">
                    <div class="mb-3 row">
                        <div class="col-sm-6">
                            <div class="d-inline-flex align-items-center">
                                <span class="mr-3">Filter by Status:</span>
                                <select data-role="none" class="form-select fixed" id="selStatusFranchiseeRequests">
                                    <option value="new">New</option>
                                    <option value="1">Approved</option>
                                    <option value="0">Rejected</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-sm-6">
                            <input data-role="none" id="txtSearchFranchiseeRequests" class="form-control" placeholder="Search here" />
                        </div>
                    </div>
                    <div class="table-responsive">
                        <table id="tblFranchiseeRequests" data-role="none" class="table table-striped table-hover">
                            <thead class="table-light">
                                <tr class="text-center align-top" style="font-size:14px;">
                                    <th style="width:50px;">#</th>
                                    <th>Company Name</th>
                                    <th>Store Name</th>
                                    <th>Administrator</th>
                                    <th>Date Registered</th>
                                    <th>Status</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>
                </div>
                <div class="tab-pane fade" id="franchisesstores">
                    <div class="mb-3 row">
                        <div class="col-sm-6">
                            <input data-role="none" id="txtSearchFranchisesStores" class="form-control" placeholder="Search here" />
                        </div>
                    </div>
                    <table id="tblFranchisesStores" data-role="none" class="table table-striped table-hover w-100">
                        <thead class="table-light">
                            <tr class="text-center align-top" style="font-size:14px;">
                                <th class="fixed-col" style="width:50px;">#</th>
                                <th class="fixed-col">Code</th>
                                <th class="fixed-col">Name</th>
                                <th class="fixed-col">Address</th>
                                <th class="fixed-col">Last Updated</th>
                            </tr>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>
                <div class="tab-pane fade" id="sortingitemmodifier">
                    <div class="mb-3">
                        <b>NOTE:</b> Only Categories and SKUs with multiple Item Modifiers are displayed here. Please drag and drop into desired order.
                    </div>
                    <div id="divIMCatSku" class="pl-0 pl-sm-5 mb-3"></div>
                    <div class="text-center mt-4" style="position:fixed; bottom:20px; width:100%;">
                        <button data-role="none" id="btnIMSortingRefresh" class="btn btn-secondary btn-lg">Refresh List</button>
                        <button data-role="none" id="btnIMSortingSave" class="btn btn-success btn-lg">Save</button>
                    </div>
                </div>
                <div class="tab-pane fade" id="sortproductcategories">
                    <div class="mb-3">
                        <b>NOTE:</b> List of all inventory-item categories. Please drag and drop into the desired order. This order will be used to prioritize SKU categories across Inventory, Stock Receiving, and Stock Transfer.
                    </div>
                    <ul id="divIISKUCat" class="list-group list-group-numbered" style="max-width:500px;"></ul>
                    <div class="text-center mt-4" style="position:fixed; bottom:20px; width:100%;">
                        <button data-role="none" id="btnSKUCatSortingSave" class="btn btn-success btn-lg">Save</button>
                    </div>
                </div>
            </div>
        </div>

        <div id="UniversalDiv" style="display:none;">

        </div>
    </div>

</div>