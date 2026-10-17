@Code
    ViewData("Title") = "System Administration"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
    
    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />

    <script>
        require([GetPathFormat('/Scripts/sources/SetupChecklist/SystemAdministration.js?crc=')]);
        require([GetPathFormat('/Scripts/sources/Organization/OrganizationAddress.js?crc=')]);
        require([GetPathFormat('/Scripts/sources/Device/DeviceInfo.js?crc=')]);
        require([GetPathFormat('/Scripts/sources/ReceiptSettings/ReceiptSettingsInfo.js?crc=')]);
        require([GetPathFormat('/Scripts/sources/SecurityRole/SecurityRolesInfo.js?crc=')]);
        require([GetPathFormat('/Scripts/sources/DevicePrinter/DevicePrinterInfo.js?crc=')]);
        require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/flatui.js?crc=')]);
    </script>

    <script src="/Scripts/sources/SetupChecklist/SystemAdministrationQueries.js"></script>
    <script src="/Scripts/sources/SetupChecklist/SystemAdministrationView.js"></script>

End Section

<div id="pageSetUpCheckList" data-role="page">
    <div class="custom-theme-a position-fixed">
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
                        <div style="margin-top:15px">
                            <div id="divBtnPeriod" style="display:block; cursor:pointer; float:left;margin-bottom:10px; text-transform:capitalize;" class="btnPeriod">
                                <span class="lblPeriod"> Today </span>
                                @*<span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span>*@
                            </div>
                        </div>
                    </div>
                </div>
                <div class="ui-block-b txt-right" style="padding:0px 16px 0px 20px;">
                    <div>
                        <div class="ui-grid-a" style="visibility: hidden;">
                            <div class="ui-block-a" style="width:80%">
                                <input type="text" placeholder="Search invoice no., product description, sales representative or customer" class="txtSearch" />
                            </div>
                            <div class="ui-block-b" style="padding:8px; width: 20%;">
                                <a class="btn custom-btn-theme-a btnSearch">
                                    <span class="ui-icon ui-icon-search" style="display:inline-block;"></span>
                                    Search
                                </a>
                            </div>
                        </div>
                        <div class="txtSearchStatus" style="display:none; float:left; color:#FF5959; text-shadow:0px 0px 0px #FFF;">
                            Search box is required!
                        </div>
                    </div>

                    <br />

                    <div style="cursor:pointer; float:right; margin-top:-20px; margin-bottom:10px;" class="btnMoreOption">
                        <span> More Options </span>
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
            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <a class="btn custom-btn-theme-a btnHome">
                        <span class="ui-icon ui-icon-home" style="display:inline-block;">
                            <p style="margin:16px 0px 0px -7px; font-size:12px;">Home</p>
                        </span>
                    </a>
                </div>
                @*<div class="ui-block-b">
                    <a id="btnSearchMobile" class="btn custom-btn-theme-a">
                        <span class="ui-icon ui-icon-search" style="display:inline-block;">
                            <p style="margin:16px 0px 0px -11px; font-size:12px;">Search</p>
                        </span>
                    </a>
                </div>
                <div class="ui-block-c">
                    <a class="btn custom-btn-theme-a btnPeriod">
                        <span class="ui-icon ui-icon-grid" style="display:inline-block;">
                            <p style="margin:16px 0px 0px -7px; font-size:12px;">Period</p>
                        </span>
                    </a>
                </div>*@
                <div class="ui-block-b">
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
                        <input type="text" placeholder="Search Here" class="txtSearch" />
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

    <div data-role="popup" id="panAddLibrary" data-overlay-theme="a" data-theme="a" data-corners="false" style="top:-160px;"> 
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="btnClosePopUp">Close</a>
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

    <div id="divAddProductPopup" data-role="popup">
        <ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="divider">Add SKU Options</li>
            <li><a id="btnBarcodeSearch">Barcode / Search</a></li>
            <li><a id="btnExcelFile">Excel File</a></li>
            <li><a id="btnSubscribe">Subscribe to Company Product List</a></li>
        </ul>
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
            <center><h3>More Options</h3></center>

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
                                <li><a id="popupbtnProducts" class="custom-btn-theme-c">Products</a></li>
                                <li><a id="popupbtnSalesReturn" class="custom-btn-theme-c">Sales Returns</a></li>
                                <li><a id="popupbtnDiscountSettings" class="custom-btn-theme-c">Discount Settings</a></li>
                                <li><a id="popupbtnPrinterRoles" class="custom-btn-theme-c">Printer Roles</a></li>
                                <li><a id="popupbtnPIM" class="custom-btn-theme-c">PIM (Product Information Management)</a></li>
                            </ul>
                        </div>

                        <div data-role="collapsible">
                            <h3>System Behavior</h3>
                            <ul data-role="listview">
                                <li><a id="popupbtnQuantitySettings" class="custom-btn-theme-c">Decimals in Quantity Settings</a></li>
                                <li><a id="popupbtnPOSSettings" class="custom-btn-theme-c">POS Settings</a></li>
                                <li><a id="popupbtnSalesOrder" class="custom-btn-theme-c">Sales Order Settings</a></li>
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
                            </ul>
                        </div>
                    </div>    
                </div>
                
            </div> 
        </div>

    <div data-role="content">
        <div id="divSystemAdDashboard" style="display:block; margin-top:105px;">
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
                                <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">Click here to add more information about your organization. This information may be published to an online business directory, used for system-generating General Information Sheet (GIS) for the SEC or DTI, and/or other value-added services in the future. Your consent must be given before Mobiz publishes any of these information to the public.</p>
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
                                <div style="text-align:left; margin-top:28px;"><span>Security Roles</span></div>
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
                                <div style="text-align:left; margin-top:28px;"><span>Products</span></div>
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
                </div>*@
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
                </div>
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
            </div>
        </div>

        <div id="divSecond" style="display:none; margin-top:105px;">
            <div id="divAddLibrary" style="text-align:right;">
                <button id="btnAddLibrary" data-role="none" class="btn custom-btn-theme-b btn-less-width">
                    Add <span class="lblAddLibraryName"></span>
                </button>
                <br /><br />
            </div>
            <div id="divInfoForFowstProdList" style="display:block;">
                <span style="color:#F00">*</span> Click on the Fowst Location to add an SKU.
                <br />
                <br />
            </div>
            <div style="max-height:400px; width:100%; overflow:auto;">
                <table data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe">
                    <thead>
                        <tr id="tblHeader" class="custom-theme-f">

                        </tr>
                    </thead>
                    <tbody id="tblBody">
                        
                    </tbody>                        
                </table>
            </div>
        </div>

        <div id="UniversalDiv" style="display:none; margin-top:122px;">
                        
        </div>

        <div id="divSubList" style="padding: 8px 15px 0px 15px; display:none; margin-top:90px;">
            <div id="divPIMSubList" style="display:none;">
                <ul data-role="listview"class="li-theme-c noBorderList custom-list custom-list-sm">
			        <li>
                        <a id="btnUploadDownloadProdList">
				            Upload / Download Product List
				        </a>
			        </li>
                    <li>
                        <a id="btnComWideProdList">
				            Company Wide Product List
				        </a>
			        </li>
                    <li>
                        <a id="btnFowstProdList">
				            FOWST Product List
				        </a>
			        </li>
                    <li>
                        <a id="btnAdjustProdPrices">
				            Adjust Product Prices
				        </a>
			        </li>
		        </ul>    
            </div>

            <div id="divAdjustProdSubList" style="display:none;">
                <ul data-role="listview"class="li-theme-c noBorderList custom-list custom-list-sm">
			        <li>
                        <a id="btnSinglePrice">
				            Single Product Price Adjustment
				        </a>
			        </li>
                    <li>
                        <a id="btnBulkPrice">
				            Bulk Price Adjustment
				        </a>
			        </li>
		        </ul>    
            </div>

        </div>

        <iframe id="divForPIM" style="display:none; margin-top:90px; width:100%; height:400px; overflow:auto;" frameborder="0">
                        
        </iframe >
    </div>

</div>