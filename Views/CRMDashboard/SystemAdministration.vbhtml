@*@Code
    ViewData("Title") = "System Administration"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code*@

@Section head

    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <link rel="stylesheet" href="/Content/bootstrap-custom.css" />

    @*<script src="/Scripts/sources/CRMDashboard/SystemAdministration.js"></script>*@
    @*<script>
        //require([GetPathFormat('/Scripts/sources/SetupChecklist/SystemAdministration.js?crc=')]);
        require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/flatui.js?crc='),
            GetPathFormat('/Scripts/sources/CRMDashboard/SystemAdministration.js?crc='),
            GetPathFormat('/Scripts/sources/SetupChecklist/NewPromptUser.js?crc='),
            GetPathFormat('/Scripts/sources/CRMDashboard/SystemAdministrationQueries.js?crc=')]);
    </script>

    <script src="/Scripts/sources/SetupChecklist/SystemAdministrationView.js"></script>*@
    @*<script src="/Scripts/sources/SetupChecklist/NewPromptUser.js"></script>*@

End Section

<div id="CRMpageSetUpCheckList" data-role="page">
    <div class="custom-theme-a position-fixed divHeader d-none">
        <div id="HeaderDesktopVersion">
            <div class="ui-grid-a ui-responsive">
                <div class="ui-block-a" style="padding-right:9px; overflow:hidden; display:flex;">
                    <div style="float:left; margin-left:10px; cursor:pointer;">
                        @*<a class="btnBack" data-iconpos="notext" data-theme="b" data-role="button" data-icon="back" title="Back">Back</a>*@
                        <a style="margin-top:8px;" class="btnHome" data-iconpos="notext" data-theme="b" data-role="button" data-icon="home" title="Home">Home</a>
                    </div>

                    <div style="float:left; margin-top:7px; margin-left:25px;">
                        <div class="OrganizationNameHeader" style="text-overflow:ellipsis; font-size:25px; white-space:nowrap; overflow:hidden;">
                            Company Name
                        </div>
                        @*<div style="margin-top:15px">
                                <div id="CRMdivBtnPeriod" style="display:block; cursor:pointer; float:left;margin-bottom:10px; text-transform:capitalize;" class="btnPeriod">
                                    <span class="lblPeriod"> Today </span>
                                    <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span>
                                </div>
                            </div>*@
                    </div>
                </div>
                <div class="ui-block-b txt-right" style="padding:0px 16px 0px 20px;">
                    <div>
                        <div class="ui-grid-a">
                            
                        </div>
                        <div class="txtSearchStatus" style="display:none; float:left; color:#FF5959; text-shadow:0px 0px 0px #FFF;">
                            Search box is required!
                        </div>
                    </div>

                    <br />

                    <div style="cursor:pointer; float:right; margin-top:-20px; margin-bottom:10px;" class="btnMoreOption">
                        <span> Go To </span>
                        <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span>
                    </div>
                </div>
            </div>
        </div>

        <div id="HeaderMobileVersion">
            <div class="ui-grid-solo">
                <div class="ui-block-a OrganizationNameHeader txt-center">
                    Company Name Here
                </div>
            </div>
            <div class="ui-grid-b">
                <div class="ui-block-a">
                    <a class="btn custom-btn-theme-a btnHome">
                        <span class="ui-icon ui-icon-home" style="display:inline-block;">
                            <p style="margin:16px 0px 0px -7px; font-size:12px;">Home</p>
                        </span>
                    </a>
                </div>
                <div class="ui-block-b">
                    <a id="CRMbtnSearchMobile" class="btn custom-btn-theme-a">
                        <span class="ui-icon ui-icon-search" style="display:inline-block;">
                            <p style="margin:16px 0px 0px -11px; font-size:12px;">Search</p>
                        </span>
                    </a>
                </div>
                @*<div class="ui-block-c">
                        <a class="btn custom-btn-theme-a btnPeriod">
                            <span class="ui-icon ui-icon-grid" style="display:inline-block;">
                                <p style="margin:16px 0px 0px -7px; font-size:12px;">Period</p>
                            </span>
                        </a>
                    </div>*@
                <div class="ui-block-c">
                    <a class="btn custom-btn-theme-a btnMoreOption">
                        <span class="ui-icon ui-icon-gear" style="display:inline-block;">
                            <p style="margin:16px 0px 0px -13px; font-size:12px;">Options</p>
                        </span>
                    </a>
                </div>
            </div>
            <div class="contentSearchEngine">
                <div class="ui-grid-solo ui-responsive">
                    <div class="ui-block-a" style="padding:6px;">
                        <form method="post" id="CRMformsearchmobile" style="width:100%">
                            <input type="text" placeholder="Search Here" class="txtSearchmobile" />
                        </form>
                    </div>
                </div>
            </div>
            <div class="divLblPeriod">
                <div class="lblPeriod">
                    Today
                </div>
            </div>
            <br />
        </div>

        <div class="BreadCrumbs">
            <div class="ItemsBreadCrumbs">
                System Administration
            </div>
        </div>
    </div>

    <div data-role="popup" id="CRMpopItemModifierName" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="CRMbtnClosePopUp">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;">Item Modifier Name</h3></center>
        <div data-role="content" class="ui-content ui-body-d" style="max-width:350px; word-wrap: break-word;">
            <div id="CRMdivErrorItemModifier" style="color:#F00; display:none;">
                Name must not be blank
                <br /><br />
            </div>
            <div>
                Item Modifier Name
                <input type="text" id="CRMtxtItemModifierNewName" />
            </div>
            <br />

            <button id="CRMbutSaveItemModifier" data-role="none" class="btn custom-btn-theme-b">Add</button>
            <br /><br />
            <button id="CRMbutCancelItemModifier" data-role="none" class="btn custom-btn-theme-c">Cancel</button>

        </div>
    </div>

    <div id="CRMpanAddLibrary" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Please type name of <span class="lblPopUpHeaderLibraryName"></span></h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <input type="text" id="CRMtxtSearch" autocomplete="off" data-role="none" class="form-control" />
                    <br />
                    <div id="CRMdivNoResultsFound" style="display:none;">
                        <br />
                        Oops! We can’t find what you’re looking for. Make sure <br /> your search is correctly spelled.
                    </div>
                    <div style="max-height:200px; overflow:auto;">
                        <ul id="CRMlstSearchFound" class="list-group"></ul>
                        <br />
                    </div>

                    <div id="CRMbtnAddToLibrary" data-dismiss="modal" class="divLibAddOption" style="display:none;">
                        <br />
                        If you’ve correctly spelled your search but can’t find it, <br /> you might want to <a class="custom-link">add a new <span class="lblAddLinkLibraryName"></span></a>.
                        <br /><br />
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-6">
                            <button data-role="none" type="button" id="CRMbtnCancelLibrary" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col-6">
                            <button data-role="none" type="button" id="CRMbtnSearchLibrary" class="btn btn-success btn-block">Search</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div data-role="popup" id="CRMdivAddProductPopup" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;" id="CRMdivAddProductTitle">Option</center>
        <div data-role="content">
            <div style="padding: 8px 15px 16px 15px;">
                <div id="CRMlistForAdding" style="display:none; visibility:hidden;">
                    <ul data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm">
                        <li>
                            <a id="CRMbtnBarcodeSearch">Searching a bar code or product description from the Mobiz public database</a>
                        </li>
                        <li>
                            <a id="CRMbtnImportByExcel">Importing data from an Excel file</a>
                        </li>
                        <li>
                            <a id="CRMbtnSubscribe">Subscribing to publicly available product lists</a>
                        </li>
                    </ul>
                </div>

                <div id="CRMlistForOption" style="display:none; visibility:hidden;">
                    <ul data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm">
                        <li>
                            <a id="CRMbtnAdjustProdPrices">Adjust prices</a>
                        </li>
                        <li>
                            <a id="CRMbtnFowstProdList">Create approved product lists by business location</a>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    </div>

    <div data-role="popup" id="CRMdivAdjustProdPricesPopup" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;">Adjust Product Prices</center>
        <div data-role="content">
            <div style="padding: 8px 15px 16px 15px;">
                <div id="CRMbtnBackToOption" style="display:block; cursor:pointer; text-transform:capitalize;">
                    <span class="ui-icon ui-icon-arrow-l" style="display:inline-block; margin-left:-12px;">
                        <span style="margin-left:25px;"> Back </span>
                    </span>
                </div>
                <br />
                <ul data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm">
                    <li>
                        <a id="CRMbtnSinglePrice">Single Product Price Adjustment</a>
                    </li>
                    <li>
                        <a id="CRMbtnBulkPrice">Bulk Price Adjustment</a>
                    </li>
                </ul>
            </div>
        </div>
    </div>

    <div data-role="popup" id="CRMpanOrgEmpMessage" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="CRMbtnClosePopUp">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;" id="CRMTitleOrgEmp"></h3></center>
        <div data-role="content" style="max-width:350px; word-wrap: break-word;">

            <div id="CRMMsgOrgEmp" style="font-weight:bold;"></div>

            <br />
            <br />

            <button id="CRMbtnOKOrgEmp" data-role="none" class="btn custom-btn-theme-b">
                Ok
            </button>

            <button id="CRMbtnReuploadPIM" data-role="none" class="btn custom-btn-theme-b" style="display:none;">
                Reupload
            </button>

            <div id="CRMdivBtnCancel" style="display:none;">
                <br />
                <button id="CRMbtnCancelOrgEmp" data-role="none" class="btn custom-btn-theme-c">
                    Cancel
                </button>
            </div>
            <br />
        </div>
    </div>

    <div data-role="popup" id="CRMpanSearchLibrary" data-overlay-theme="a" data-theme="a" data-corners="false" style="top:-60px;">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="CRMbtnClosePopUp">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;" id="CRMTitleLibrary"></h3></center>
        <div data-role="content" style="max-width:350px; word-wrap: break-word;">
            <div>
                <span class="divBrowserContentTop"></span>
                <br />
            </div>

            <input type="text" id="CRMtxtSearchKeyword" autofocus />

            <div style="padding: 8px 15px 0px 15px; max-height:200px; overflow:auto;">
                <ul id="CRMlstOfLibrary" data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm">
                    @*<li>
                            <a id="CRM">
                                asd
                            </a>
                        </li>*@
                </ul>
                <br>
            </div>
            <br />
        </div>
    </div>

    <div data-role="popup" class="MoreReportsPopUp" id="CRMpanOption" data-overlay-theme="a" data-theme="a" data-corners="false" style="top:-130px;">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="CRMbtnCloseMoreOption">Close</a>
        <center><h3>Go To </h3></center>

        <div data-role="content">

            <div style="max-height:400px; overflow:auto;">
                <div class="noBorderList" data-role="collapsible-set" data-corners="false" data-theme="a">
                    <div data-role="collapsible">
                        <h3>Required Data</h3>
                        <ul data-role="listview">
                            <li><a id="CRMpopupbtnOrganization" class="custom-btn-theme-c">Organization</a></li>
                            @*<li><a id="CRMpopupbtnFowst" class="custom-btn-theme-c">Fowst Locations</a></li>
                                <li><a id="CRMpopupbtnAuthDevices" class="custom-btn-theme-c">Authorized Devices</a></li>
                                <li><a id="CRMpopupbtnReceiptSettings" class="custom-btn-theme-c">Receipt Settings</a></li>*@
                            <li><a id="CRMpopupbtnSecurityRoles" class="custom-btn-theme-c">Security Roles</a></li>
                            <li><a id="CRMpopupbtnEmployees" class="custom-btn-theme-c">Employees</a></li>
                            @*<li><a id="CRMpopupbtnProducts" class="custom-btn-theme-c">PIM (Product Information Management)</a></li>
                                <li><a id="CRMpopupbtnSalesReturn" class="custom-btn-theme-c">Sales Returns</a></li>
                                <li><a id="CRMpopupbtnDiscountSettings" class="custom-btn-theme-c">Discount Settings</a></li>
                                <li><a id="CRMpopupbtnPrinterRoles" class="custom-btn-theme-c">Printer Roles</a></li>
                                <li><a id="CRMpopupbtnPIM" class="custom-btn-theme-c">PIM (Product Information Management)</a></li>*@
                        </ul>
                    </div>

                    @*<div data-role="collapsible">
                            <h3>System Behavior</h3>
                            <ul data-role="listview">
                                <li><a id="CRMpopupbtnQuantitySettings" class="custom-btn-theme-c">Decimals in Quantity Settings</a></li>
                                <li><a id="CRMpopupbtnPOSSettings" class="custom-btn-theme-c">POS Settings</a></li>
                                <li><a id="CRMpopupbtnSalesOrder" class="custom-btn-theme-c">Sales Order Settings</a></li>
                                <li><a id="CRMpopupbtnReceivingAndTransferOut" class="custom-btn-theme-c">Receiving And Transfer-Out Settings</a></li>
                                <li><a id="CRMpopupbtnInventoryCount" class="custom-btn-theme-c">Inventory Count Settings</a></li>
                            </ul>
                        </div>

                        <div data-role="collapsible">
                            <h3>Optional Data</h3>
                            <ul data-role="listview">
                                <li><a id="CRMpopupbtnHotButton" class="custom-btn-theme-c">Hot Button Settings</a></li>
                                <li><a id="CRMpopupbtnGroupSettings" class="custom-btn-theme-c">Group Settings</a></li>
                                <li><a id="CRMpopupbtnCustomer" class="custom-btn-theme-c">Customer / Client</a></li>
                                <li><a id="CRMpopupbtnSupplier" class="custom-btn-theme-c">Supplier / Vendor</a></li>
                                <li><a id="CRMpopupbtnShippers" class="custom-btn-theme-c">Shippers</a></li>
                                <li><a id="CRMpopupbtnSalesTeam" class="custom-btn-theme-c">Sales Team</a></li>
                                <li><a id="CRMpopupbtnItemModifiers" class="custom-btn-theme-c">Item Modifiers</a></li>
                                <li><a id="CRMpopupbtnKnowledgeManagementAndQuestionaire" class="custom-btn-theme-c">Knowledge Management (temp placeholder)</a></li>
                            </ul>
                        </div>*@
                </div>
            </div>

        </div>
    </div>

    <div data-role="popup" id="CRMpanAddEmail" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="CRMbtnClosePopUp">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;">Employee Email Address</h3></center>
        <div data-role="content" style="max-width:350px; word-wrap: break-word;">

            <span>There is no existing email address on record for <span class="peopleFirstName" style="font-weight:bold;"></span>.  Please enter his/her email address on the email entry field below:</span>

            <br />
            <br />
            <input type="text" id="CRMtxtEmpEmailAdd" placeholder="Enter email address" />
            <br />
            <br />

            <button id="CRMbtnSaveAndSend" data-role="none" class="btn custom-btn-theme-b">
                Save and Send Employment Confirmation
            </button>
            <br /><br />
            <button id="CRMbtnCancelEmail" data-role="none" class="btn custom-btn-theme-c">
                Cancel
            </button>
            <br />
        </div>
    </div>

    <div data-role="popup" id="CRMpanAddRawMaterials" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="CRMbtnClosePopUp">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;" id="CRMBOMTitle">Content</h3></center>
        <div data-role="content" style="max-width:350px; word-wrap: break-word;">
            <div id="CRMbtnBackBomRawMaterials" style="display:block; cursor:pointer; text-transform:capitalize;">
                <span class="ui-icon ui-icon-arrow-l" style="display:inline-block;">
                    <div style="margin-left:25px; width:150px;"> Back to Search </div>
                </span>
            </div>
            <br />
            <div id="CRMdivBomContent" style="display:block; visibility:visible;">
                Each <span id="CRMspanParentUnit" style="font-weight:bold;"></span> of <span id="CRMspanParentProduct" style="font-weight:bold;"></span> contains <span id="CRMspanQty" style="font-weight:bold;"></span> <span id="CRMspanChildUnit" style="font-weight:bold;"></span> of </span> <span id="CRMspanChildProduct" style="font-weight:bold;"></span>:<br />

                <br />
                <input id="CRMtxtQty" placeholder="Qty" />
                <select id="CRMcbUnit">
                    <option value="pula">Loading data.</option>
                    <option value="berde">please wait</option>
                </select>
                <br />
                <button id="CRMbtnSaveBomContent" data-role="none" class="btn custom-btn-theme-b">
                    Save
                </button>
                <br /><br />
                <button id="CRMbtnCancelBomContent" data-role="none" class="btn custom-btn-theme-c">
                    Cancel
                </button>
                <br />
            </div>

            <div id="CRMdivBomUnitDefinition" style="display:none; visibility:hidden;">
                Each <span id="CRMspanParentUnitUD" class="UnitDefinitionLabel"></span> of <span id="CRMspanParentProductUD" class="UnitDefinitionLabel"></span> contains:<br />
                <br />

                <input id="CRMtxtQtyUD" placeholder="Qty" />
                <select id="CRMcbUnitUD">
                    <option value="pula">Loading data.</option>
                    <option value="berde">please wait</option>
                </select>
                <br />
                <button id="CRMbtnSaveBomUnitDefinition" data-role="none" class="btn custom-btn-theme-b">
                    Save
                </button>
                <br /><br />
                <button id="CRMbtnCancelBomUnitDefinition" data-role="none" class="btn custom-btn-theme-c">
                    Back
                </button>
                <br />
            </div>
        </div>
    </div>

    <div data-role="popup" id="CRMpanAddObject" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="CRMbtnClosePopUp">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;" id="CRMlblAddObject">Add Case</h3></center>
        <div data-role="content" style="width:850px; word-wrap: break-word;">

            <div class="popup-content">
                <div class="custom-ui-label">
                    <div id="CRMlblAddObjectTitle">Case ID</div>
                    <div>
                        <input type="text" id="CRMtxtObject" placeholder="" />
                    </div>
                </div>
            </div>

            <button id="CRMbtnSaveObject" data-role="none" class="btn custom-btn-theme-b">
                Save
            </button>

        </div>
    </div>

    <div id="CRMpanKbUrl" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">KB URL</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body" id="divKbFilter">
                    <input data-role="none" type="text" style="display:none" id="txtStageID" />

                    <div class="mb-3">
                        <label for="CRMtxtKbUrl" class="form-label">Enter your KB URL:<span style="color:#ff0000">*</span></label>
                        <input data-role="none" type="text" id="CRMtxtKbUrl" class="form-control" />
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-md order-1 order-md-0">
                            <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col-md order-1 order-md-0">
                            <button data-role="none" type="button" id="CRMbtnSaveKbUrl" class="btn btn-success btn-block btn-lg">Save</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div data-role="content" id="CRMdivContent">

        @*<button id="CRMbtnPrintPOCSlip" data-role="none" class="btn custom-btn-theme-d btn-less-width">Sample Print</button>*@
        @*<br />*@
        <div id="CRMdivSystemAdDashboard" style="display:none;">
            <div id="CRMdivForRequiredData">
                <div>
                    <div class="custom-theme-f" style="padding:10px; font-size:24px; font-weight:bold;">
                        Required Data
                    </div>
                    <h4>
                        Please make sure that you provide the correct information on the things below.  Mobiz will not work properly without these required setup.
                    </h4>
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
                <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                    <div class="ui-block-a" style="width:60%; float:left;">
                        <div id="CRMbtnSetupOrganization" class="btn custom-btn-theme-c border-theme-c divSetupOrganization" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                            <div style="padding:10px;">
                                <div style="text-align:left; margin-top:28px;"><span>Organization</span></div>
                                <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">Click here to add more information about your organization. This information may be published to an online business directory, used for system-generated General Information Sheet (GIS) for the SEC or DTI, and/or other value-added services in the future. Your consent must be given before Mobiz publishes any of these information to the public.</p>
                            </div>
                        </div>
                    </div>
                    <div class="ui-block-b" style="width:40%; float:right;">
                        <div id="CRMbtnSetupOrganizationVal" class="custom-theme-c border-theme-c divSetupOrganizationVal" style="margin:0px; padding:0px; height:208px;" data-role="none" data-theme="c" data-corners="false">
                            <div style="text-align:center; padding:10px;">
                                <div><img id="CRMimgOrg" src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                <div><label id="CRMlblOrgLastUpdated">(08/20/2014)</label></div>
                            </div>
                        </div>
                    </div>
                    <div style="clear:both;"></div>
                </div>
                @*<div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="CRMbtnSetupOrganizationFOWST" class="btn custom-btn-theme-c border-theme-c divSetupOrganizationFOWST" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <div style="text-align:left; margin-top:28px;"><span>FOWST Locations</span></div>
                                    <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">
                                        At least one (1) record of a FOWST (Factory, Office, Warehouse, Store or Truck) is needed.
                                        Each FOWST location keeps track of its own inventory movements and balances. If you want a
                                        location to have its own inventory ledger (of movements and balances), you must add a FOWST
                                        location record for it.
                                    </p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="CRM" class="custom-theme-c border-theme-c divSetupOrganizationFOWSTVal" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px; height:62px;">
                                    <div><img id="CRMimgFOWST" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="CRMlblFOWSTCount">0</label></div>
                                    <div><label id="CRMlblFOWSTLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="CRMbtnSetupDevice" class="btn custom-btn-theme-c border-theme-c divSetupDevice" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <div style="text-align:left; margin-top:28px;"><span>Authorized Devices</span></div>
                                    <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">At least one (1) device is needed to be registered for use in your company.</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="CRMbtnSetupDeviceVal" class="custom-theme-c border-theme-c divSetupDeviceVal" style="margin:0px; padding:0px; height:124px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px;">
                                    <div><img id="CRMimgDevice" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="CRMlblDeviceCount">0</label></div>
                                    <div><label id="CRMlblDeviceLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="CRMbtnSetupReceiptSettings" class="btn custom-btn-theme-c border-theme-c divSetupReceiptSettings" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <div style="text-align:left; margin-top:28px;"><span>Receipt Settings</span></div>
                                    <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">Fill up the Receipt Settings Info for each FOWST.</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="CRMbtnSetupReceiptSettingsVal" class="custom-theme-c border-theme-c divSetupReceiptSettingsVal" style="margin:0px; padding:0px; height:124px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px;">
                                    <div><img id="CRMimgReceiptSettings" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="CRMlblReceiptSettingsCount">0</label></div>
                                    <div><label id="CRMlblReceiptSettingsLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>*@
                <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                    <div class="ui-block-a" style="width:60%; float:left;">
                        <div class="btn custom-btn-theme-c border-theme-c divSetupSecurity" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                            <div style="padding:10px;">
                                <div style="text-align:left; margin-top:28px;"><span>Access Rights</span></div>
                                <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">Add  or edit existing roles in your organization for fine-tuning staff's access rights to Mobiz apps.</p>
                            </div>
                        </div>
                    </div>
                    <div class="ui-block-b" style="width:40%; float:right;">
                        <div id="CRMbtnSetupSecurityVal" class="custom-theme-c border-theme-c divSetupSecurityVal" style="margin:0px; padding:0px; height:145px;" data-role="none" data-theme="c" data-corners="false">
                            <div style="text-align:center; padding:10px;">
                                <div><img id="CRMimgSecurityAccess" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                                <div><label id="CRMlblSecurityAccessCount">0</label></div>
                                <div><label id="CRMlblSecurityAccessLastUpdated">(08/20/2014)</label></div>
                            </div>
                        </div>
                    </div>
                    <div style="clear:both;"></div>
                </div>
                <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                    <div class="ui-block-a" style="width:60%; float:left;">
                        <div id="CRMbtnSetupEmployee" class="btn custom-btn-theme-c border-theme-c divSetupEmployee" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                            <div style="padding:10px;">
                                <div style="text-align:left; margin-top:28px;"><span>User</span></div>
                                <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">Each user must be assigned his or her own user name/password and Security Role (cashier, manager, etc.) before he/she can use Mobiz apps.</p>
                            </div>
                        </div>
                    </div>
                    <div class="ui-block-b" style="width:40%; float:right;">
                        <div id="CRMbtnSetupEmployeeVal" class="custom-theme-c border-theme-c divSetupEmployeeVal" style="margin:0px; padding:0px; height:145px;" data-role="none" data-theme="c" data-corners="false">
                            <div style="text-align:center; padding:10px;">
                                <div><img id="CRMimgEmployeeAccess" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                                <div><label id="CRMlblEmployeeCount">0</label></div>
                                <div><label id="CRMlblEmployeeLastUpdated">(08/20/2014)</label></div>
                            </div>
                        </div>
                    </div>
                    <div style="clear:both;"></div>
                </div>
                @*<div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="CRMbtnSalesReturnSettings" class="btn custom-btn-theme-c border-theme-c" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">Sales Return Settings</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="CRM" class="custom-theme-c border-theme-c" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px; height:62px;">
                                    <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label>(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="CRMbtnDiscountSettings" class="btn custom-btn-theme-c border-theme-c" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">Discount Settings</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="CRM" class="custom-theme-c border-theme-c" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px; height:62px;">
                                    <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="CRMlblDiscountSettingsLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="CRMbtnPrinterRoles" class="btn custom-btn-theme-c border-theme-c" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">Printer Roles</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="CRM" class="custom-theme-c border-theme-c" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px; height:62px;">
                                    <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="CRMlblPrinterCount">0</label></div>
                                    <div><label id="CRMlblPrinterLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>*@
                @*<div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="CRMbtnPrinterSetup" class="btn custom-btn-theme-c border-theme-c" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">Device Printer Setup</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="CRM" class="custom-theme-c border-theme-c" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px; height:62px;">
                                    <div><img id="CRM" src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="CRM">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="CRMbtnPIM" class="btn custom-btn-theme-c border-theme-c" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;"><span>PIM <span style="font-size:14px;">(Product Information Management)</span></span></p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="CRM" class="custom-theme-c border-theme-c" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px; height:62px;">
                                    <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label>(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>*@
            </div>

            @*<div id="CRMdivForSystemBehavior">
                    <br />
                    <div class="ui-grid-solo" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a">
                            <div class="custom-theme-f" style="padding:10px; font-size:24px; font-weight:bold;">
                                System Behavior
                            </div>
                            <h4>
                                You can make Mobiz Complete Retail more suitable to your organization's work style by specifying some behaviors in these modules.
                            </h4>
                        </div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="CRMbtnQuantitySetting" class="btn custom-btn-theme-c border-theme-c divQuantitySetting" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <div style="text-align:left;">
                                        <p style="text-align:left; white-space: normal;">Decimals in Quantity Settings</p>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="CRMbtnQuantitySettingVal" class="custom-theme-c border-theme-c divQuantitySettingVal" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px;">
                                    <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label>(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="CRMbtnPOSSetting" class="btn custom-btn-theme-c border-theme-c divPOSSetting" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">POS Settings</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="CRMbtnPOSSettingVal" class="custom-theme-c border-theme-c divPOSSettingVal" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px;">
                                    <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label>(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="CRMbtnSalesOrderSetting" class="btn custom-btn-theme-c border-theme-c divSalesOrderSetting" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">Sales Order Settings</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="CRMbtnSalesOrderSettingVal" class="custom-theme-c border-theme-c divSalesOrderSettingVal" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px;">
                                    <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label>(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="CRMbtnReceivingAndTransferOutSetting" class="btn custom-btn-theme-c border-theme-c divSalesOrderSetting" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">Receiving And Transfer-Out Settings</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="CRMbtnReceivingAndTransferOutVal" class="custom-theme-c border-theme-c divSalesOrderSettingVal" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px;">
                                    <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label>(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="CRMbtnInventoryCountSetting" class="btn custom-btn-theme-c border-theme-c divSalesOrderSetting" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">Inventory Count Settings</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="CRMbtnInventoryCountVal" class="custom-theme-c border-theme-c divSalesOrderSettingVal" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px;">
                                    <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label>(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="CRMbtnPurchaseOrderSetting" class="btn custom-btn-theme-c border-theme-c divSalesOrderSetting" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">Purchase Order Settings</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="CRMbtnPurchaseOrderVal" class="custom-theme-c border-theme-c divSalesOrderSettingVal" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px;">
                                    <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="CRMlblPurchaseOrderLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="CRMbtnDailyBeginningBalanceSetting" class="btn custom-btn-theme-c border-theme-c divSalesOrderSetting" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">Daily Beginning Balance Settings</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="CRMbtnPurchaseOrderVal" class="custom-theme-c border-theme-c divSalesOrderSettingVal" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px;">
                                    <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="CRMlblDailyBeginningBalanceLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                </div>*@

            <div id="CRMdivForOptionalData">
                <br />
                <div class="ui-grid-solo" style="padding:0px; overflow:visible;">
                    <div class="ui-block-a">
                        <div class="custom-theme-f" style="padding:10px; font-size:24px; font-weight:bold;">
                            Optional Data
                        </div>
                        <h4>
                            Add these optional data to enhance the functions of Mobiz Complete Retail's different modules.
                        </h4>
                    </div>
                </div>

                @*<div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="CRMbtnHotButtons" class="btn custom-btn-theme-c border-theme-c divHotButtons" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <div style="text-align:left; margin-top:28px;"><span>Hot Buttons Settings</span></div>
                                    <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">
                                        Predetermine what products and services are available to your cashier and sales team at a glance.
                                        Hot buttons are programmable keys (by authorized administrators at the office) that act as short
                                        cuts to products.  No need to type or barcode scan products when they are accessible in just a
                                        few taps.
                                    </p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="CRM" class="custom-theme-c border-theme-c divHotButtonsVal" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px;">
                                    <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="CRMlblHotButtonCount">0</label></div>
                                    <div><label id="CRMlblHotButtonLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="CRMbtnGroupSettings" class="btn custom-btn-theme-c border-theme-c divGroupSettings" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <div style="text-align:left; margin-top:28px;"><span>Group Settings</span></div>
                                    <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">
                                        You can group your stores and assign a management and support team to each group. Group sales
                                        reports are viewable only to authorized persons (or teams) and quick replenishment of products
                                        are handled easily by the support team through an online order replenishment dashboard.
                                    </p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="CRM" class="custom-theme-c border-theme-c divGroupSettingsVal" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px;">
                                    <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="CRMlblGroupSettingsCount">0</label></div>
                                    <div><label id="CRMlblGroupSettingsLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="CRMbtnSetupCustomer" class="btn custom-btn-theme-c border-theme-c" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">Customer / Client</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="CRM" class="custom-theme-c border-theme-c" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px; height:62px;">
                                    <div><img id="CRMimgCustomer" src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="CRMlblCustomerCount">0</label></div>
                                    <div><label id="CRMlblCustomerLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="CRMbtnSetupSupplier" class="btn custom-btn-theme-c border-theme-c" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">Supplier / Vendor</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="CRM" class="custom-theme-c border-theme-c" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px; height:62px;">
                                    <div><img id="CRMimgSupplier" src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="CRMlblSupplierCount">0</label></div>
                                    <div><label id="CRMlblSupplierLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="CRM" class="btn custom-btn-theme-c border-theme-c" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">Shippers</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="CRM" class="custom-theme-c border-theme-c" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px; height:62px;">
                                    <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label>(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>*@
            </div>
        </div>

        <div id="CRMdivSecond" style="display:block;">
            <div class="row wrapper min-vh-100 flex-column flex-sm-row">
                <div class="col-12 col-md-2 p-0 bg-light flex-shrink-1">
                    <div class="navbar navbar-expand-sm navbar-light bg-light align-items-start flex-sm-column flex-row p-2">
                        <a data-role="none" class="navbar-brand" href="#">Menu</a>
                        <a data-role="none" href="#" class="navbar-toggler" data-toggle="collapse" data-target=".sidebarCRM">
                            <span class="navbar-toggler-icon"></span>
                        </a>
                        <div class="collapse navbar-collapse sidebarCRM mt-2">
                            <ul class="flex-column w-100 justify-content-between nav nav-pills">
                                <li class="nav-item"><a class="nav-link active" id="CRMbtnSetupSecurity" data-toggle="tab" href="">Access Rights</a></li>
                                <li class="nav-item"><a class="nav-link" id="CRMbtnClientStaging" data-toggle="tab" href="">Client Staging</a></li>
                                <li class="nav-item"><a class="nav-link" id="CRMbtnKnowledgebase" href="">KB URL Settings</a></li>
                                <li class="nav-item"><a class="nav-link" id="CRMbtnCRMSettings" href="">General Settings</a></li>
                                <li class="nav-item"><a class="nav-link" id="CRMbtnSalesTeam" data-toggle="tab" href="">Sales Team</a></li>
                                <li class="nav-item"><a class="nav-link" id="CRMbtnActivityQuota" data-toggle="tab" href="">Activity Quota</a></li>
                                <li class="nav-item"><a class="nav-link" id="CRMbtnTicketNotes" data-toggle="tab" href="">Ticket Notes</a></li>
                                <li class="nav-item"><a class="nav-link" id="CRMbtnFunnelTemplate" data-toggle="tab" href="">Funnel Template</a></li>
                            </ul>
                        </div>
                    </div>
                </div>

                <div class="col bg-faded">
                    <div class="tab-content">
                        <div id="tabCRMAccessRights" class="tab-pane fade show active">
                            <div class="row mb-3">
                                <div class="col">
                                    <form method="post" id="CRMformsearchdesktop" style="width:100%;">
                                        <input data-role="" type="text" placeholder="Search here" class="txtSearchdesktop form-control" />
                                    </form>
                                </div>
                                <div class="col text-right">
                                    <button id="CRMbtnAddLibrary" data-role="none" class="btn btn-success">
                                        Add <span class="lblAddLibraryName"></span>
                                    </button>
                                </div>
                            </div>
                            @*<div class="mb-3 text-right">
                                
                            </div>*@
                            @*<br /><br /><br /><br />
                            <div id="CRMdivInfoForFowstProdList" style="display:none;">
                                <span style="color:#F00">*</span> Click on the Fowst Location to add an SKU.
                                <br />
                                <br />
                            </div>
                            <div id="CRMdivDownloadFilePIM" style="display:none;">
                                <label id="CRMlblProductRecordcount"></label>
                                <a id="CRMlnkProductList" style="cursor:pointer; font-size:16px;"> Click here to download Excel file to view full company product list.</a>
                                <br />
                                <br />
                            </div>*@
                            <div  class="table-responsive">
                                <table data-role="none" data-mode="reflow" style="display:table;" class="table table-light table-hover table-striped">
                                    <thead>
                                        <tr id="CRMtblHeader" class="custom-theme-f txt-center"></tr>
                                    </thead>
                                    <tbody id="CRMtblBody"></tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>            
        </div>

        <div id="CRMUniversalDiv" style="display:none;">

        </div>
    </div>

</div>