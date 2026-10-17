@Code
    ViewData("Title") = "System Administration"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
    
    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />

    <script src="/Scripts/sources/SetupChecklist/SystemAdministration.js"></script>
    <script>
        //require([GetPathFormat('/Scripts/sources/SetupChecklist/SystemAdministration.js?crc=')]);
        require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/flatui.js?crc=')]);
    </script>

    <script src="/Scripts/sources/SetupChecklist/SystemAdministrationQueries.js"></script>
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
</style>

<div id="pageSetUpCheckList" data-role="page">
    <div class="custom-theme-a position-fixed divHeader">
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
                                <div id="divBtnPeriod" style="display:block; cursor:pointer; float:left;margin-bottom:10px; text-transform:capitalize;" class="btnPeriod">
                                    <span class="lblPeriod"> Today </span>
                                    <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span>
                                </div>
                            </div>*@
                    </div>
                </div>
                <div class="ui-block-b txt-right" style="padding:0px 16px 0px 20px;">
                    <div>
                        <div class="ui-grid-a">
                            <form method="post" id="formsearchdesktop" style="width:100%;">
                                <div class="ui-block-a" style="width:80%">
                                    <input type="text" placeholder="Search here" class="txtSearchdesktop" />
                                </div>
                                <div class="ui-block-b" style="padding:8px; width: 20%;">
                                    <a class="btn custom-btn-theme-a btnSearch">
                                        <span class="ui-icon ui-icon-search" style="display:inline-block;"></span>
                                        Search
                                    </a>
                                </div>
                            </form>
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
                    <a id="btnSearchMobile" class="btn custom-btn-theme-a">
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
                        <form method="post" id="formsearchmobile" style="width:100%">
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

    <div data-role="popup" id="popItemModifierName" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="btnClosePopUp">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;">Item Modifier Name</h3></center>
        <div data-role="content" class="ui-content ui-body-d" style="max-width:350px; word-wrap: break-word;">
            <div id="divErrorItemModifier" style="color:#F00; display:none;">
                Name must not be blank
                <br /><br />
            </div>
            <div>
                Item Modifier Name
                <input type="text" id="txtItemModifierNewName" />
            </div>
            <br />

            <button id="butSaveItemModifier" data-role="none" class="btn custom-btn-theme-b">Add</button>
            <br /><br />
            <button id="butCancelItemModifier" data-role="none" class="btn custom-btn-theme-c">Cancel</button>

        </div>
    </div>

    <div data-role="popup" id="panAddLibrary" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right btnClosePopUp">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;">Please type name of <span class="lblPopUpHeaderLibraryName"></span></h3></center>
        <div data-role="content" style="max-width:350px;">

            <input type="text" id="txtSearch" />

            <div id="divNoResultsFound" style="display:none;">
                <br />
                Oops! We can’t find what you’re looking for. Make sure <br /> your search is correctly spelled.
            </div>
            <div style="padding: 8px 15px 0px 15px; max-height:200px; overflow:auto;">
                <ul id="lstSearchFound" data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm">
                    @*<li>
                            <a id="btnSettings">
                                <h3>Samsung Mega</h3>
                                <br />
                                <p>IMEI: 357750050585396</p>
                                <p>UUID: RF1D666JBZV</p>
                            </a>
                        </li>*@
                </ul>
                <br />
            </div>

            <div id="btnAddToLibrary" class="divLibAddOption" style="display:none;">
                <br />
                If you’ve correctly spelled your search but can’t find it, <br /> you might want to <a class="custom-link">add a new <span class="lblAddLinkLibraryName"></span></a>.
                <br /><br />
            </div>
            <br />
            <div class="ui-grid-solo">
                <div class="ui-block-a">
                    <button id="btnSearchLibrary" data-role="none" class="btn custom-btn-theme-b">
                        Search
                    </button>
                    <br /><br />
                    <button id="btnCancelLibrary" data-role="none" class="btn custom-btn-theme-c">
                        Cancel
                    </button>
                </div>
            </div>
            <br />
            @*<div class="ui-grid-solo divAddLibrary" style="display:none;">
                    <div class="ui-block-a">
                        <button id="btnAddToLibrary" data-role="none" class="btn custom-btn-theme-f">
                            Add
                        </button>
                    </div>
                </div>*@
        </div>
    </div>

    <div data-role="popup" id="divAddProductPopup" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;" id="divAddProductTitle">Option</center>
        <div data-role="content">
            <div style="padding: 8px 15px 16px 15px;">
                <div id="listForAdding" style="display:none; visibility:hidden;">
                    <ul data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm">
                        <li>
                            <a id="btnBarcodeSearch">Searching a bar code or product description from the Mobiz public database</a>
                        </li>
                        <li>
                            <a id="btnImportByExcel">Importing data from an Excel file</a>
                        </li>
                        <li>
                            <a id="btnSubscribe">Subscribing to publicly available product lists</a>
                        </li>
                        @*<li>
                            <a id="btnPIMWebForm">Using Web Form</a>
                        </li>*@
                    </ul>
                </div>

                <div id="listForOption" style="display:none; visibility:hidden;">
                    <ul data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm">
                        <li>
                            <a id="btnAdjustProdPrices">Adjust prices</a>
                        </li>
                        <li>
                            <a id="btnFowstProdList">Create approved product lists by business location</a>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    </div>

    <div data-role="popup" id="divAdjustProdPricesPopup" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;">Adjust Product Prices</center>
        <div data-role="content">
            <div style="padding: 8px 15px 16px 15px;">
                <div id="btnBackToOption" style="display:block; cursor:pointer; text-transform:capitalize;">
                    <span class="ui-icon ui-icon-arrow-l" style="display:inline-block; margin-left:-12px;">
                        <span style="margin-left:25px;"> Back </span>
                    </span>
                </div>
                <br />
                <ul data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm">
                    <li>
                        <a id="btnSinglePrice">Single Product Price Adjustment</a>
                    </li>
                    <li>
                        <a id="btnBulkPrice">Bulk Price Adjustment</a>
                    </li>
                </ul>
            </div>
        </div>
    </div>

    <div data-role="popup" id="panOrgEmpMessage" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="btnClosePopUp">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;" id="TitleOrgEmp"></h3></center>
        <div data-role="content" style="max-width:350px; word-wrap: break-word;">

            <div id="MsgOrgEmp" style="font-weight:bold;"></div>

            <br />
            <br />

            <button id="btnOKOrgEmp" data-role="none" class="btn custom-btn-theme-b">
                Ok
            </button>

            <button id="btnReuploadPIM" data-role="none" class="btn custom-btn-theme-b" style="display:none;">
                Reupload
            </button>

            <div id="divBtnCancel" style="display:none;">
                <br />
                <button id="btnCancelOrgEmp" data-role="none" class="btn custom-btn-theme-c">
                    Cancel
                </button>
            </div>
            <br />
        </div>
    </div>

    <div data-role="popup" id="panSearchLibrary" data-overlay-theme="a" data-theme="a" data-corners="false" style="top:-60px;">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="btnClosePopUp">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;" id="TitleLibrary"></h3></center>
        <div data-role="content" style="max-width:350px; word-wrap: break-word;">
            <div>
                <span class="divBrowserContentTop"></span>
                <br />
            </div>

            <input type="text" id="txtSearchKeyword" autofocus />

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
            <br />
        </div>
    </div>

    <div data-role="popup" class="MoreReportsPopUp" id="panOption" data-overlay-theme="a" data-theme="a" data-corners="false" style="top:-130px;">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="btnCloseMoreOption">Close</a>
        <center><h3>Go To </h3></center>

        <div data-role="content">

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
                            <li><a id="popupbtnSalesReturn" class="custom-btn-theme-c">Sales Returns</a></li>
                            <li><a id="popupbtnDiscountSettings" class="custom-btn-theme-c">Discount Settings</a></li>
                            <li><a id="popupbtnPrinterRoles" class="custom-btn-theme-c">Printer Roles</a></li>
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
                            <li><a id="popupbtnPrinter" class="custom-btn-theme-c">Printers</a></li>
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
                            <li><a id="popupbtnKnowledgeManagementAndQuestionaire" class="custom-btn-theme-c">Knowledge Management (temp placeholder)</a></li>
                            <li><a id="popupbtnLoyaltyProgram" class="custom-btn-theme-c">Loyalty Program Settings</a></li>
                        </ul>
                    </div>
                </div>
            </div>

        </div>
    </div>

    <div data-role="popup" id="panAddEmail" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="btnClosePopUp">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;">Employee Email Address</h3></center>
        <div data-role="content" style="max-width:350px; word-wrap: break-word;">

            <span>There is no existing email address on record for <span class="peopleFirstName" style="font-weight:bold;"></span>.  Please enter his/her email address on the email entry field below:</span>

            <br />
            <br />
            <input type="text" id="txtEmpEmailAdd" placeholder="Enter email address" />
            <br />
            <br />

            <button id="btnSaveAndSend" data-role="none" class="btn custom-btn-theme-b">
                Save and Send Employment Confirmation
            </button>
            <br /><br />
            <button id="btnCancelEmail" data-role="none" class="btn custom-btn-theme-c">
                Cancel
            </button>
            <br />
        </div>
    </div>

    <div data-role="popup" id="panAddRawMaterials" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="btnClosePopUp">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;" id="BOMTitle">Content</h3></center>
        <div data-role="content" style="max-width:350px; word-wrap: break-word;">
            <div id="btnBackBomRawMaterials" style="display:block; cursor:pointer; text-transform:capitalize;">
                <span class="ui-icon ui-icon-arrow-l" style="display:inline-block;">
                    <div style="margin-left:25px; width:150px;"> Back to Search </div>
                </span>
            </div>
            <br />
            <div id="divBomContent" style="display:block; visibility:visible;">
                Each <span id="spanParentUnit" style="font-weight:bold;"></span> of <span id="spanParentProduct" style="font-weight:bold;"></span> contains <span id="spanQty" style="font-weight:bold;"></span> <span id="spanChildUnit" style="font-weight:bold;"></span> of </span> <span id="spanChildProduct" style="font-weight:bold;"></span>:<br />

                <br />
                <input id="txtQty" placeholder="Qty" />
                <select id="cbUnit">
                    <option value="pula">Loading data.</option>
                    <option value="berde">please wait</option>
                </select>
                <br />
                <button id="btnSaveBomContent" data-role="none" class="btn custom-btn-theme-b">
                    Save
                </button>
                <br /><br />
                <button id="btnCancelBomContent" data-role="none" class="btn custom-btn-theme-c">
                    Cancel
                </button>
                <br />
            </div>

            <div id="divBomUnitDefinition" style="display:none; visibility:hidden;">
                Each <span id="spanParentUnitUD" class="UnitDefinitionLabel"></span> of <span id="spanParentProductUD" class="UnitDefinitionLabel"></span> contains:<br />
                <br />

                <input id="txtQtyUD" placeholder="Qty" />
                <select id="cbUnitUD">
                    <option value="pula">Loading data.</option>
                    <option value="berde">please wait</option>
                </select>
                <br />
                <button id="btnSaveBomUnitDefinition" data-role="none" class="btn custom-btn-theme-b">
                    Save
                </button>
                <br /><br />
                <button id="btnCancelBomUnitDefinition" data-role="none" class="btn custom-btn-theme-c">
                    Back
                </button>
                <br />
            </div>
        </div>
    </div>

    @*<div data-role="popup" id="panAddKnowledgeManagement" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="btnClosePopUp">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;">Add Case</h3></center>
        <div data-role="content" style="width:850px; word-wrap: break-word;">
            <div>
                <div class="custom-ui-label">
                    <div>Case ID</div>
                    <div>
                        <input type="text" id="txtCaseID" placeholder="Enter Case Description" disabled="disabled" />
                    </div>
                </div>

                <div class="custom-ui-label">
                    <div>Case </div>
                    <div>
                        <input type="text" id="txtCase" placeholder="Enter Case Description" />
                    </div>
                </div>

                <div class="custom-ui-label">
                    <div>Solution </div>
                    <div>
                        <textarea id="txtSolution" placeholder="Enter Solution"></textarea>
                    </div>
                </div>

                <div class="custom-ui-label">
                    <div>Steps: </div>
                    <div>
                        <textarea id="txtSteps" placeholder="Enter Steps"></textarea>
                    </div>
                </div>

                <div class="custom-ui-label">
                    <div>Topic  </div>
                    <div>
                        <select id="selTopic"></select>
                    </div>
                </div>

                <div class="custom-ui-label">
                    <div>SubTopic  </div>
                    <div>
                        <select id="selSubTopic">
                            <option>General Topic</option>
                        </select>
                    </div>
                </div>

                <div class="custom-ui-label">
                    <div>Product  </div>
                    <div>
                        <select id="selProduct"></select>
                    </div>
                </div>

                <div class="custom-ui-label">
                    <div>Language  </div>
                    <div>
                        <select id="selLanguage"></select>
                    </div>
                </div>

                <div class="custom-ui-label">
                    <div>Tags  </div>
                    <div>
                        <input type="text" id="txtTags" placeholder="Enter Case Tags" />
                    </div>
                </div>

                <button id="btnSaveKM" data-role="none" class="btn custom-btn-theme-b">
                    Save
                </button>
                <br /><br />
                <button id="btnCancelKM" data-role="none" class="btn custom-btn-theme-c">
                    Cancel
                </button>

            </div>
            <br />
        </div>
    </div>*@

    <div class="popup-md" id="panAddKnowledgeManagement" custom-data-role="popup">
        <div class="popup-header">
            Case
            <a class="popup-close">x</a>
        </div>

        <div class="popup-content">            
            <input type="text" id="txtProblemDBID" style="display:none;" data-role="none" />
            <div class="custom-ui-label">
                <div>Topic  </div>
                <div>
                    <select id="selTopic">
                        <option>Select Topic</option>
                    </select>
                </div>
            </div>

            <div class="custom-ui-label">
                <div>SubTopic  </div>
                <div>
                    <select id="selSubTopic">
                        <option>Select SubTopic</option>
                    </select>
                </div>
            </div>

            <div class="custom-ui-label">
                <div>Product  </div>
                <div>
                    <select id="selProduct">
                        <option>Select Product</option>
                    </select>
                </div>
            </div>

            <div class="custom-ui-label">
                <div>Language  </div>
                <div>
                    <select id="selLanguage">
                        <option>Select Language</option>
                    </select>
                </div>
            </div>

            <div class="custom-ui-label">
                <div>Tags  </div>
                <div>
                    <input type="text" id="txtTags" placeholder="Enter Case Tags" />
                </div>
            </div>
                    
            @*<div class="custom-ui-label">
                <div>Case ID</div>
                <div>
                    <input type="text" id="txtCaseID" placeholder="Enter Case Description" disabled="disabled" />
                </div>
            </div>*@

            <div class="custom-ui-label">
                <div>Problem or Question </div>
                <div>
                    <input type="text" id="txtCase" placeholder="Enter Problem or Question" />
                </div>
            </div>

            <div class="panel">
                <div class="panel-title custom-theme-h">
                    Solution or Answer
                </div>

                <div class="panel-content txt-left">
                    <div style="text-align:right;">
                        <button id="btnAddSolution" data-role="none" class="btn custom-btn-theme-b btnCircle-sm">
                            <img alt="Add" src="/Content/images/plus-white.png" style="width:18px;" />
                        </button>
                    </div>
                    <div style="padding: 8px 15px 0px 15px;">
                        <ol id="listSolution" data-role="none" style=" margin:0 auto; padding:0;">
                            <li>
                                <div class="todoContainer" style="display:table; width:100%;">
                                    <div style="float:none; display:table-cell; width:auto; overflow:hidden; padding:8px; word-break:break-all;">
                                        Hello
                                        <ol>
                                            <li>asdasd</li>
                                            <li>asdasd</li>
                                        </ol>
                                    </div>
                                    <div style="padding:10px; display:table-cell; width:30px; vertical-align:middle;">
                                        <button data-role="none" title="Remove" class="btn custom-btn-theme-c btnCircle-xs btnShadow">
                                            <img alt="Remove" src="/Content/images/close-black.png" />
                                        </button>
                                    </div>
                                </div>
                            </li>
                        </ol>
                    </div>
                </div>
            </div>

            @*
            <div class="custom-ui-label">
                <div>Solution or Answer</div>
                <div>
                    <textarea id="txtSolution" placeholder="Enter Solution or Answer"></textarea>
                </div>
            </div>

            <div class="custom-ui-label">
                <div>Steps: </div>
                <div>
                    <textarea id="txtSteps" placeholder="Enter Steps"></textarea>
                </div>
            </div>*@            
        </div>

        <div class="popup-footer">
            <button id="btnCancelKM" data-role="none" class="btn custom-btn-theme-c btnCircle">
                <img alt="Options" src="/Content/images/close-black.png" />
            </button>
            <button id="btnSaveKM" data-role="none" class="btn custom-btn-theme-d btnCircle">
                <img alt="Options" src="/Content/images/save.png" />
            </button>            
        </div>
    </div>

    <div class="popup-md" id="panAddSolution" custom-data-role="popup">
        <div class="popup-header">
            Solution and Answer
            <a class="popup-close">x</a>
        </div>

        <div class="popup-content">
            <div class="custom-ui-label">
                <div>
                    Is this Private?
                </div>                
                <div>
                    <label for="chbPrivate" class="checkbox-only">
                        <input type="checkbox" name="chbPrivate" id="chbPrivate" data-iconpos="notext" />
                        Private
                    </label>
                </div>
            </div>
            
            
            <div id="divAllowedTeam" style="display:none">
                <button data-role="none" id="btnPrivateSolution" class="btn custom-btn-theme-b btn-less-width" style="float:right;">Add Team</button>
                <div style="clear:both;"></div>
                <br />
                Teams allowed to view:
                <div id="listofAllowedTeam" style="max-height: 150px; overflow:auto;">
                    
                </div>
                <br />
            </div>
            <hr />
            <br />
            <input type="text" id="txtSolutionID" style="display:none;" data-role="none" />
            <textarea id="txtSolutionAndAnswer" placeholder="Enter your solution or answer"></textarea>            

            <button data-role="none" id="btnSteps" title="Add step-by-step instructions" class="btn custom-btn-theme-b btnCircle-xs">
                <img alt="Add" src="/Content/images/plus-white.png" style="width:18px;" />
            </button>

            <ul id="listSolutionSteps" data-role="none" style="list-style-type: none; margin:0 auto; padding:0;">
            </ul>
        </div>

        <div class="popup-footer align-right">
            <div style="text-align:right;">
                <button id="btnCancelSolution" data-role="none" class="btn custom-btn-theme-c btnCircle">
                    <img alt="Options" src="/Content/images/arrow-left.png" />
                </button>
                <button id="btnOkSolution" title="Ok" data-role="none" class="btn custom-btn-theme-b btnCircle">
                    <img alt="Options" src="/Content/images/check.png" />
                </button>
            </div>
        </div>
    </div>

    <div data-role="popup" id="panNewTeam" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;">New Team</h3></center>
        <div data-role="content" style="width:300px; word-wrap: break-word;">
            <div class="custom-ui-label">
                <div>Team Name:</div>
                <div><input type="text" id="txtNewTeamName" /></div>
            </div>
            
            <br />
            <button data-role="none" id="btnOkNewTeam" class="btn custom-btn-theme-b">Ok</button>            
            <br /><br />
            <a data-role="none" data-rel="back" id="btnCancelNewTeam" class="btn custom-btn-theme-c">Cancel</a>            
        </div>
    </div>

    <div data-role="popup" id="panAddSteps" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="btnClosePopUp">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;">New Steps</h3></center>
        <div data-role="content" style="width:300px; word-wrap: break-word;">
            <textarea id="txtNewSteps" placeholder="Enter your solution or answer"></textarea>
            
            <br />
            <button data-role="none" id="btnOkAndAddNewSteps" class="btn custom-btn-theme-b">Ok and Add another Step</button>
            <br /><br />
            <button data-role="none" id="btnOkNewSteps" class="btn custom-btn-theme-g">Ok</button>            
            <br /><br />
            <button data-role="none" id="btnCancelNewSteps" class="btn custom-btn-theme-c">Cancel</button>            
        </div>
    </div>

    <div data-role="popup" id="panAddObject" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="btnClosePopUp">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;" id="lblAddObject">Add Case</h3></center>
        <div data-role="content" style="width:850px; word-wrap: break-word;">
        
            <div class="popup-content">
                <div class="custom-ui-label">
                    <div id="lblAddObjectTitle">Case ID</div>
                    <div>
                        <input type="text" id="txtObject" placeholder="" />
                    </div>
                </div>
            </div>

            <button id="btnSaveObject" data-role="none" class="btn custom-btn-theme-b">
                Save
            </button>
            
        </div>
    </div>
        @*<div class="popup" id="panAddObject" custom-data-role="popup">
                <div class="popup-header" id="lblAddObject">
                    Interaction Details
                    <a class="popup-close">x</a>
                </div>

                <div class="popup-content">
                    <div class="custom-ui-label">
                        <div id="lblAddObjectTitle">Case ID</div>
                        <div>
                            <input type="text" id="txtObject" placeholder="" />
                        </div>
                    </div>
                </div>

                <div class="popup-footer align-right">

                </div>
            </div>*@

        <div data-role="content" id="divContent">

            @*<button id="btnPrintPOCSlip" data-role="none" class="btn custom-btn-theme-d btn-less-width">Sample Print</button>*@
            <br />
            <div id="divSystemAdDashboard" style="display:block;">
                <div id="divForRequiredData">
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
                            <div id="btnSetupOrganization" class="btn custom-btn-theme-c border-theme-c divSetupOrganization" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <div style="text-align:left; margin-top:28px;"><span>Organization</span></div>
                                    <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">Click here to add more information about your organization. This information may be published to an online business directory, used for system-generated General Information Sheet (GIS) for the SEC or DTI, and/or other value-added services in the future. Your consent must be given before Mobiz publishes any of these information to the public.</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="btnSetupOrganizationVal" class="custom-theme-c border-theme-c divSetupOrganizationVal" style="margin:0px; padding:0px; height:208px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px;">
                                    <div><img id="imgOrg" src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="lblOrgLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="btnSetupOrganizationFOWST" class="btn custom-btn-theme-c border-theme-c divSetupOrganizationFOWST" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
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
                            <div id="" class="custom-theme-c border-theme-c divSetupOrganizationFOWSTVal" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px; height:62px;">
                                    <div><img id="imgFOWST" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="lblFOWSTCount">0</label></div>
                                    <div><label id="lblFOWSTLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="btnSetupDevice" class="btn custom-btn-theme-c border-theme-c divSetupDevice" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <div style="text-align:left; margin-top:28px;"><span>Authorized Devices</span></div>
                                    <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">At least one (1) device is needed to be registered for use in your company.</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="btnSetupDeviceVal" class="custom-theme-c border-theme-c divSetupDeviceVal" style="margin:0px; padding:0px; height:124px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px;">
                                    <div><img id="imgDevice" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="lblDeviceCount">0</label></div>
                                    <div><label id="lblDeviceLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="btnSetupReceiptSettings" class="btn custom-btn-theme-c border-theme-c divSetupReceiptSettings" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <div style="text-align:left; margin-top:28px;"><span>Receipt Settings</span></div>
                                    <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">Fill up the Receipt Settings Info for each FOWST.</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="btnSetupReceiptSettingsVal" class="custom-theme-c border-theme-c divSetupReceiptSettingsVal" style="margin:0px; padding:0px; height:124px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px;">
                                    <div><img id="imgReceiptSettings" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="lblReceiptSettingsCount">0</label></div>
                                    <div><label id="lblReceiptSettingsLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="btnSetupSecurity" class="btn custom-btn-theme-c border-theme-c divSetupSecurity" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <div style="text-align:left; margin-top:28px;"><span>Access Rights</span></div>
                                    <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">Add  or edit existing roles in your organization for fine-tuning staff's access rights to Mobiz apps.</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="btnSetupSecurityVal" class="custom-theme-c border-theme-c divSetupSecurityVal" style="margin:0px; padding:0px; height:145px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px;">
                                    <div><img id="imgSecurityAccess" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="lblSecurityAccessCount">0</label></div>
                                    <div><label id="lblSecurityAccessLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="btnSetupEmployee" class="btn custom-btn-theme-c border-theme-c divSetupEmployee" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <div style="text-align:left; margin-top:28px;"><span>Employees</span></div>
                                    <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">Each employee must be assigned his or her own user name/password and Security Role (cashier, manager, etc.) before he/she can use Mobiz apps.</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="btnSetupEmployeeVal" class="custom-theme-c border-theme-c divSetupEmployeeVal" style="margin:0px; padding:0px; height:145px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px;">
                                    <div><img id="imgEmployeeAccess" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="lblEmployeeCount">0</label></div>
                                    <div><label id="lblEmployeeLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="btnSetupProducts" class="btn custom-btn-theme-c border-theme-c divSetupProducts" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <div style="text-align:left; margin-top:28px;"><span>PIM <span style="font-size:14px;">(Product Information Management)</span></span></div>
                                    <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">At least one (1) product is needed. You may add your products via Excel upload or you may choose from the publicly available product database of Mobiz.</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="btnSetupProductsVal" class="custom-theme-c border-theme-c divSetupProductsVal" style="margin:0px; padding:0px; height:145px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px;">
                                    <div><img id="imgProduct" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="lblProductCount">0</label></div>
                                    <div><label id="lblProductLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="btnSalesReturnSettings" class="btn custom-btn-theme-c border-theme-c" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">Sales Return Settings</p>
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
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="btnDiscountSettings" class="btn custom-btn-theme-c border-theme-c" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">Discount Settings</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="" class="custom-theme-c border-theme-c" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px; height:62px;">
                                    <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="lblDiscountSettingsLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="btnPrinterRoles" class="btn custom-btn-theme-c border-theme-c" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">Printer Roles</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="" class="custom-theme-c border-theme-c" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px; height:62px;">
                                    <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="lblPrinterCount">0</label></div>
                                    <div><label id="lblPrinterLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="btnPrinter" class="btn custom-btn-theme-c border-theme-c divSalesOrderSetting" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">Printers</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="btnPrinterVal" class="custom-theme-c border-theme-c divSalesOrderSettingVal" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px;">
                                    <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="lblPrinterLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
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

                <div id="divForSystemBehavior">
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
                            <div id="btnQuantitySetting" class="btn custom-btn-theme-c border-theme-c divQuantitySetting" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <div style="text-align:left;">
                                        <p style="text-align:left; white-space: normal;">Decimals in Quantity Settings</p>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="btnQuantitySettingVal" class="custom-theme-c border-theme-c divQuantitySettingVal" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
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
                            <div id="btnPOSSetting" class="btn custom-btn-theme-c border-theme-c divPOSSetting" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">POS Settings</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="btnPOSSettingVal" class="custom-theme-c border-theme-c divPOSSettingVal" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
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
                            <div id="btnSalesOrderSetting" class="btn custom-btn-theme-c border-theme-c divSalesOrderSetting" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">Sales Order Settings</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="btnSalesOrderSettingVal" class="custom-theme-c border-theme-c divSalesOrderSettingVal" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
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
                            <div id="btnReceivingAndTransferOutSetting" class="btn custom-btn-theme-c border-theme-c divSalesOrderSetting" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">Receiving And Transfer-Out Settings</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="btnReceivingAndTransferOutVal" class="custom-theme-c border-theme-c divSalesOrderSettingVal" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
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
                            <div id="btnInventoryCountSetting" class="btn custom-btn-theme-c border-theme-c divSalesOrderSetting" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">Inventory Count Settings</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="btnInventoryCountVal" class="custom-theme-c border-theme-c divSalesOrderSettingVal" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
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
                            <div id="btnPurchaseOrderSetting" class="btn custom-btn-theme-c border-theme-c divSalesOrderSetting" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">Purchase Order Settings</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="btnPurchaseOrderVal" class="custom-theme-c border-theme-c divSalesOrderSettingVal" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px;">
                                    <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="lblPurchaseOrderLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="btnDailyBeginningBalanceSetting" class="btn custom-btn-theme-c border-theme-c divSalesOrderSetting" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">Daily Beginning Balance Settings</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="btnPurchaseOrderVal" class="custom-theme-c border-theme-c divSalesOrderSettingVal" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px;">
                                    <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="lblDailyBeginningBalanceLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="btnTableViewSetting" class="btn custom-btn-theme-c border-theme-c divSalesOrderSetting" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">Table View Settings</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div class="custom-theme-c border-theme-c divSalesOrderSettingVal" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px;">
                                    <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div id="divForOptionalData">
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

                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="btnHotButtons" class="btn custom-btn-theme-c border-theme-c divHotButtons" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
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
                            <div id="" class="custom-theme-c border-theme-c divHotButtonsVal" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px;">
                                    <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="lblHotButtonCount">0</label></div>
                                    <div><label id="lblHotButtonLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="btnGroupSettings" class="btn custom-btn-theme-c border-theme-c divGroupSettings" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
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
                            <div id="" class="custom-theme-c border-theme-c divGroupSettingsVal" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px;">
                                    <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="lblGroupSettingsCount">0</label></div>
                                    <div><label id="lblGroupSettingsLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="btnSetupCustomer" class="btn custom-btn-theme-c border-theme-c" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">Customer / Client</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="" class="custom-theme-c border-theme-c" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px; height:62px;">
                                    <div><img id="imgCustomer" src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="lblCustomerCount">0</label></div>
                                    <div><label id="lblCustomerLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="btnSetupSupplier" class="btn custom-btn-theme-c border-theme-c" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">Supplier / Vendor</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="" class="custom-theme-c border-theme-c" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px; height:62px;">
                                    <div><img id="imgSupplier" src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="lblSupplierCount">0</label></div>
                                    <div><label id="lblSupplierLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    @*<div class="ui-grid-a" style="padding:0px; overflow:visible;">
                            <div class="ui-block-a" style="width:60%; float:left;">
                                <div id="" class="btn custom-btn-theme-c border-theme-c" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                    <div style="padding:10px;">
                                        <p style="text-align:left; white-space: normal;">Shippers</p>
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
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="btnSalesTeam" class="btn custom-btn-theme-c border-theme-c" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">Sales Team</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="" class="custom-theme-c border-theme-c" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px; height:62px;">
                                    <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="lblSalesTeamCount">0</label></div>
                                    <div><label id="lblSalesTeamLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="btnItemModifiers" class="btn custom-btn-theme-c border-theme-c" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">Item Modifiers</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="" class="custom-theme-c border-theme-c" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px; height:62px;">
                                    <div><img src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="lblItemModifiersCount">0</label></div>
                                    <div><label id="lblItemModifiersLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                </div>
            </div>

            <div id="divSecond" style="display:none;">
                <div style="float:right">
                    <span id="divAddLibrary" style="display:block; float:right">
                        <button id="btnAddLibrary" data-role="none" class="btn custom-btn-theme-b btn-less-width">
                            Add <span class="lblAddLibraryName"></span>
                        </button>
                    </span>
                    <span id="divWorkSchedule" style="display:none; float:right; margin-right:5px;">
                        <button id="btnWorkSchedule" data-role="none" class="btn custom-btn-theme-b btn-less-width">
                            Work Schedule
                        </button>
                    </span>
                    <span id="divOptionAddProduct" style="display:none; float:right; margin-right:5px;">
                        <button id="btnOptionAddProduct" data-role="none" class="btn custom-btn-theme-b btn-less-width">
                            Options
                        </button>
                    </span>
                    <span style="clear:both;"></span>
                    <br /><br />
                </div>
                <br /><br /><br /><br />
                <div id="divInfoForFowstProdList" style="display:none;">
                    <span style="color:#F00">*</span> Click on the Fowst Location to add an SKU.
                    <br />
                    <br />
                </div>
                <div id="divDownloadFilePIM" style="display:none;">
                    <label id="lblProductRecordcount"></label>
                    <a id="lnkProductList" style="cursor:pointer; font-size:16px;"> Click here to download Excel file to view full company product list.</a>
                    <br />
                    <br />
                </div>
                <div style="width:100%; overflow:auto;">
                    <table id="tblSysAdminDataTable" data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-shadow ui-responsive table-stroke table-stripe">
                        <thead>
                            <tr id="tblHeaderDT" class="custom-theme-f txt-center"></tr>
                        </thead>
                        <tbody id="tblBodyDT"></tbody>
                    </table>
                </div>
            </div>

            <div id="UniversalDiv" style="display:none;">

            </div>
        </div>

    </div>
