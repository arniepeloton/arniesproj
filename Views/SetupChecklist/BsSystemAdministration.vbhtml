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
                require(['/Scripts/sources/SetupChecklist/BsSystemAdministration.js?v=' + assetversion,
                    '/Scripts/sources/SetupChecklist/BsSystemAdministrationQueries.js?v=' + assetversion,
                    '/Scripts/sources/DashBoardsAndReports/flatui.js?v=' + assetversion])
            })
        })        
    </script>

    @*<script src="/Scripts/sources/SetupChecklist/SystemAdministrationQueries.js"></script>*@
    <script src="/Scripts/sources/SetupChecklist/SystemAdministrationView.js"></script>
    <script src="/Scripts/sources/SetupChecklist/NewPromptUser.js"></script>
    

End Section

<style type="text/css">
    .dataTables_processing {
        /*Overwrite the Process Loading in datatables*/
        top: 5px !important;
        line-height: 40vh !important;
        height: 100% !important;
    }

    .ui-table-cell-label {
        display:none;
    }
</style>

<div id="pageSetUpCheckList" data-role="page">
    <nav class="navbar navbar-dark bg-primary sticky-top" style="padding:0px !important;">
        <table style="width:100%; height:60px;">
            <tr>
                <td style="width:100px">
                    <button data-role="none" id="btnHome" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0 navbar-brand">
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
                        <ul id="lstSearchFound" class="list-group">
                        </ul>
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

    <div id="divAddProductPopup" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
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
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="btnFowstProdList">
                                Create Approved Product Lists By Business Location
                            </a>
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="btnDeactivateAllSku">
                                Deactivate All Products
                            </a>
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="btnActivateAllSku">
                                Reactivate Products
                            </a>
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="btnDownloadCompanyProductList">
                                Edit SKU Data In Bulk (Via Excel File)
                            </a>
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="btnPimPriceAdjust">
                                Change Prices Via Price Adjustment
                            </a>
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="butChangeProductAreas">
                                Change Product Areas or Sections
                            </a>
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="butPimMergeBrands">
                                Merge Duplicate Brands
                            </a>
                            <a href="#" class="list-group-item list-group-item-action pt-3 pb-3" data-dismiss="modal" id="butDeduplicateProductNumbers">
                                Deduplicate Product Numbers
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
                        <input type="file" id="objFile" name="objFile">
                        <input type="submit" value="Upload File">
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
                        <label class="col-sm-3 mb-3" for="txtPriceLevelName">Price Level Name:</label>
                        <div class="col-sm-9">
                            <input type="text" data-role="none" class="form-control" id="txtPriceLevelName" />
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-sm-3 mb-3" for="">Price Level Type:</label>
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
                        <label class="col-sm-3 mb-3" for="selPriceLevelActive">Active:</label>
                        <div class="col-sm-9">
                            <div class="form-check form-switch" style="font-size:20px;">
                                <input class="form-check-input" data-role="none" type="checkbox" id="selPriceLevelActive">
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
                    <button data-role="none" type="button" id="butSalesRepresentativeSave" class="btn btn-success btn-block">Save</button>
                    <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>                    
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
                <div class="modal-body" >
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
                    <button data-role="none" type="button" id="btnSaveBarcodeType" class="btn btn-success btn-block">Save</button>                    
                    <button data-role="none" type="button" id="btnRemoveBarcodeType" class="btn btn-danger btn-block" style="display:none;" data-dismiss="modal">Remove</button>                    
                    <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
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

                <div class="row mb-3">
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

                <div class="row mb-3">
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
                            <div id="lblHotButtonLastUpdated">(08/20/2014)</div>
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

                <div class="row mb-3" style="display:none;">
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

                <div class="row mb-3">
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

            <div id="divForEcommerce">
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
        </div>

        <div id="divSecond" style="display:none;">
            <div class="mb-4">
                <div id="divOptionStatus" style="float:left; display:block;">
                    <table>
                        <tr>
                            <td class="pr-3">Show:</td>
                            <td class="pr-3">
                                <select id="selOptionStatus" data-role="none" class="form-select form-select-lg">
                                    <option value="1">Active</option>
                                    <option value="0">Inactive</option>
                                </select>
                            </td>
                        </tr>
                    </table>
                </div>

                <div class="elementsForErp" style="float:left;">
                    @*dont mind if there's duplicate "formsearchdesktop" id on this file, this line use in erp*@
                    <form method="post" id="formsearchdesktop">
                        <input type="text" data-role="none" placeholder="Search here" class="txtSearchdesktop form-control form-control-lg">
                    </form>
                </div>
                <div style="float:right">
                    <span id="divAddLibrary" style="display:block; float:right">
                        <button id="btnAddLibrary" data-role="none" class="btn btn-success btn-lg">
                            Add <span class="lblAddLibraryName"></span>
                        </button>
                    </span>
                    <span id="divWorkSchedule" style="display:none; float:right; margin-right:5px;">
                        <button id="btnWorkSchedule" data-role="none" class="btn btn-success btn-lg">
                            Work Schedule
                        </button>
                    </span>
                    <span id="divOptionAddProduct" style="display:none; float:right; margin-right:5px;">
                        <button id="btnOptionAddProduct" data-role="none" class="btn btn-success btn-lg">
                            Options
                        </button>
                    </span>
                    <span style="clear:both;"></span>
                    <br /><br />
                </div>
                <div style="clear:both;"></div>
            </div>
            <div id="divInfoForFowstProdList" style="display:none;" class="mb-4">
                <span style="color:#F00">*</span> Click on the Fowst Location to add an SKU.
            </div>
            <div id="divDownloadFilePIM" style="display:none;" class="mb-4">
                <label id="lblProductRecordcount"></label>
                <a id="lnkProductList" style="cursor:pointer; font-size:16px;"> Click here to download Excel file to view full company product list.</a>
            </div>
            <form id="Supplierformupload" enctype="multipart/form-data" method="post" style="display:none;">
                <div style="display:none;"><input type="text" id="txtSupplierUploadExcelOrgID" name="OrgID" style="display:none;"></div>
                <input type="file" id="SupplierobjFile" name="objFile">
                <input data-role="none" type="text" name="IsCustomer" value="0" style="display:none;" />
                <input type="submit" id="btnSupplierUploadExcel" value="Upload File">
            </form>
            <div class="table-responsive">
                <table id="tblSysAdminDataTable" data-role="none" class="table table-striped table-hover">
                    <thead class="table-light">
                        <tr id="tblHeaderDT"></tr>
                    </thead>
                    <tbody id="tblBodyDT"></tbody>
                </table>
            </div>
        </div>

        <div id="UniversalDiv" style="display:none;">

        </div>
    </div>

</div>