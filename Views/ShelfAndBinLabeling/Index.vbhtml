@Code
    ViewData("Title") = "Shelf And Bin Labeling"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head 

    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <link rel="stylesheet" href="/Content/bootstrap-custom.css" />
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />    

    <script>
        require([GetPathFormat('/Scripts/sources/ShelfAndBinLabeling/index.js?crc=')]);
    </script>     

End Section
 
<div id="pageMainBin" data-role="page">
    @*<div data-role="header">
            <h1>@ViewData("Title")</h1>
        </div>*@

    <div id="divBinHeader">
        <div class="custom-theme-a">
            <div id="HeaderDesktopVersion2" class="container">
                <div class="ui-grid-a" style="height:53px;">
                    <div class="ui-block-a" style="padding-right:9px; overflow:hidden; display:flex;">
                        <div style="float:left; margin-top:10px; margin-bottom:0px; overflow:auto;">
                            <div class="OrganizationNameHeader" style="text-overflow:ellipsis; font-size:25px; white-space:nowrap; overflow:hidden;">
                                Company Name
                            </div>
                        </div>
                    </div>
                    <div class="ui-block-b" style="padding:0px 0px 0px 20px;">
                        <div class="ui-grid-a">
                            <div class="ui-block-a">
                                <div style="margin-top:10px; margin-bottom:0px; text-overflow:ellipsis; white-space:nowrap; overflow:hidden;">
                                    <label>FOWST:</label> <br />
                                    <a data-role="none" id="lblWorkFowstName" class="custom-link" style="color:#FFF;"></a>
                                </div>
                            </div>
                            <div class="ui-block-b">

                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="HeaderMobileVersion2" style="display:none;">
                <div class="ui-grid-solo">
                    <div class="ui-block-a OrganizationNameHeader txt-center" style="white-space:nowrap; overflow:hidden; text-overflow:ellipsis;">
                        Company Name Here
                    </div>
                </div>
                <div class="ui-grid-b">
                    <div class="ui-block-a">
                        <a class="btn custom-btn-theme-a btnHome">
                            <span class="ui-icon ui-icon-home" style="display:inline-block;">
                                <p style="margin:16px 0px 0px 0px; position:relative; transform: translate(-50%, 50%); font-size:12px;">Home</p>
                            </span>
                        </a>
                    </div>
                    <div class="ui-block-b">
                        <a id="btnSearchMobile" class="btn custom-btn-theme-a">
                            <span class="ui-icon ui-icon-search" style="display:inline-block;">
                                <p style="margin:16px 0px 0px 0px; position:relative; transform: translate(-50%, 50%); font-size:12px;">Search</p>
                            </span>
                        </a>
                    </div>
                    <div class="ui-block-c">
                        <a class="btn custom-btn-theme-a btnSettings">
                            <span class="ui-icon ui-icon-gear" style="display:inline-block;">
                                <p style="margin:16px 0px 0px 0px; position:relative; transform: translate(-50%, 50%); font-size:12px;">Settings</p>
                            </span>
                        </a>
                    </div>
                </div>
                <div class="contentSearchEngine">
                    <div class="ui-grid-solo ui-responsive">
                        <div class="ui-block-a" style="padding:6px;">
                            <input type="text" placeholder="Search Here" class="txtSearchProspect" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="BreadCrumbs">
            <div class="container">
                <div class="ItemsBreadCrumbs">
                    Home
                </div>
            </div>
        </div>
    </div>

    <br />

    <div data-role="content">

        <div>
            <div id="divTabRevenue" class="custom-tab">
                <ul>
                    <li id="liCreateShelfOrBinID" custom-tab-content="#dvBAT" class="active">Bin Address Tags</li>
                    <li id="liDownloadShelfOrBinID" custom-tab-content="#dvSPT">Shelf Labels</li>
                    <li id="liCreateBarcodeLabels" custom-tab-content="#dvPBT">Item Labels</li>
                    @*<li id="liAssignProductsToBin">Assign Products to Bin</li>*@
                </ul>
            </div>

            <br />
            <br />

            <div id="dvBAT" custom-tab-role="content">
                <div>
                    <button data-role="none" class="btn custom-btn-theme-b btn-less-width btn-sm" id="btnDownloadBinTagsPDF">
                        <img class="img-downloadbin-white" style="width:18px;" /> &nbsp; Download Bin Tags as PDF
                    </button>
                    <button data-role="none" class="btn custom-btn-theme-b btn-less-width btn-sm" id="btnAutoGenerateBinTags">
                        <img class="img-autogen-white" style="width:18px;" /> &nbsp; Auto Generate Bin Tags
                    </button>
                    <button data-role="none" class="btn custom-btn-theme-b btn-less-width btn-sm" id="btnManuallyAddBinTags">
                        <img class="img-addbat-white" style="width:18px;" /> &nbsp; Manually Add Bin Tags
                    </button>
                    <button data-role="none" class="btn custom-btn-theme-d btn-less-width btn-sm" id="btnDeactivateAllBins">
                        <img class="img-deactivatebin-white" style="width:20px;" /> &nbsp; Deactivate All Bin Tags
                    </button>
                </div>

                <br />
                <br />

                <div>
                    <button data-role="none" class="btn custom-btn-theme-e btn-less-width btn-sm" id="btnFilterBinList">
                        <img class="img-filter" style="width:18px;" /> <span id="lblFilterBins">Bin List Filtered By: Active Bins</span>
                    </button>
                </div>

                <br />

                <div style="overflow:auto;">
                    <table id="tblBins" data-role="table" data-role="reflow" style="margin-right:5px; display:table;" class="display ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                        <thead>
                            <tr class="custom-theme-f fixed-cell-width">
                                <th class="txt-center" style="width:10px" id="thSelectAll"> Select All </th>
                                <th class="txt-center"> Area </th>
                                <th class="txt-center"> Bin Address </th>
                                <th class="txt-center"> Remarks </th>
                                <th class="txt-center"> Status </th>
                                <th class="txt-center"> No of products mapped </th>
                            </tr>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>
            </div>

            <div id="dvSPT" custom-tab-role="content" style="display:none">
                <div>
                    <button data-role="none" class="btn custom-btn-theme-b btn-less-width btn-sm" id="btnDownloadShelfPriceTagsPDF">
                        <img class="img-printer-white" style="width:18px;" /> Download Shelf Labels as PDF
                    </button>
                </div>

                <br />
                <br />

                <div>
                    <button data-role="none" class="btn custom-btn-theme-e btn-less-width btn-sm" id="btnPriceLevel">
                        <img class="img-filter" style="width:18px;" /> <span id="lblPriceLevel">Price Level: None</span>
                    </button>
                    <button data-role="none" class="btn custom-btn-theme-e btn-less-width btn-sm" id="btnArea">
                        <img class="img-filter" style="width:18px;" /> <span id="lblArea">Area: None</span>
                    </button>
                    <button data-role="none" class="btn custom-btn-theme-e btn-less-width btn-sm" id="btnBins">
                        <img class="img-filter" style="width:18px;" /> <span id="lblBinAddress">Bin Address: None</span>
                    </button>   
                    <button data-role="none" class="btn custom-btn-theme-e btn-less-width btn-sm" id="btnShowSPT">
                        <img class="img-filter" style="width:18px;" /> <span id="lblShowDisplay">Show: All</span>
                    </button>                    
                </div>

                <br />

                <div style="overflow:auto;">
                    <table id="tblShelfPrices" data-role="table" data-role="reflow" style="margin-right:5px; display:table;" class="display ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                        <thead>
                            <tr class="custom-theme-f fixed-cell-width">
                                <th class="txt-center" style="width:10px" id="thShelfSelectAll"> Select All </th>
                                <th class="txt-center"> BIN </th>
                                <th class="txt-center"> Product Description </th>
                                <th class="txt-center"> Unit Measure </th>
                                <th class="txt-center"> Price </th>
                                <th class="txt-center"> Price Last Updated</th>
                            </tr>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>
            </div>

            <div id="dvPBT" custom-tab-role="content" style="display:none">
                <div>
                    <button data-role="none" class="btn custom-btn-theme-b btn-less-width btn-sm" id="btnDownloadProductLabelsPDF">
                        <img class="img-printer-white" style="width:18px;" /> Download Item Labels as PDF
                    </button>
                </div>

                <br />
                <br />

                <div>
                    @*<button data-role="none" class="btn custom-btn-theme-e btn-less-width btn-sm" id="btnPriceLevel">
                        <img class="img-filter" style="width:18px;" /> <span id="lblPriceLevel">Price Level: None</span>
                    </button>*@
                    <button data-role="none" class="btn custom-btn-theme-e btn-less-width btn-sm" id="btnProductArea">
                        <img class="img-filter" style="width:18px;" /> <span id="lblProductArea">Area: None</span>
                    </button>
                    <button data-role="none" class="btn custom-btn-theme-e btn-less-width btn-sm" id="btnProductBins">
                        <img class="img-filter" style="width:18px;" /> <span id="lblProductBinAddress">Bin Address: None</span>
                    </button>
                    <button data-role="none" class="btn custom-btn-theme-e btn-less-width btn-sm" id="btnProductBarcodeFilter">
                        <img class="img-filter" style="width:18px;" /> <span id="lblProductBarcodeFilter">Show: All Products In Named Bins</span>
                    </button>
                </div>

                <br />

                <div style="overflow:auto;">
                    <table id="tblProductBarcode" data-role="table" data-role="reflow" style="margin-right:5px; display:table;" class="display ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                        <thead>
                            <tr class="custom-theme-f fixed-cell-width">
                                <th class="txt-center" style="width:10px" id="thBarcodeSelectAll"> Select All </th>
                                <th class="txt-center"> Product No </th>
                                <th class="txt-center"> Product Description </th>
                                <th class="txt-center"> Unit Measure </th>
                                <th class="txt-center"> Price </th>
                                <th class="txt-center"> Barcode </th>
                            </tr>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
    

    <div id="popProductBinMapping" custom-data-role="popup">
        <div class="popup-md">
            <div class="popup-header">
                <span id="lblProductBinMappingLabel"></span>
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">

                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <div>                            
                            <h4 class="btnFOWST"> </h4>
                        </div>
                    </div>
                    <div class="ui-block-b">
                        <div style="text-align:right">
                            <button data-role="none" class="btn custom-btn-theme-b btn-less-width btn-sm" id="btnAddProduct">
                                <img class="img-plus-white" style="width:18px;" /> Add Product
                            </button>
                        </div>
                    </div>
                </div>
                <br />
                <br />

                <table id="tblBinProducts" data-role="table" data-role="reflow" style="margin-right:5px; display:table;" class="display ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                    <thead>
                        <tr class="custom-theme-f fixed-cell-width">
                            <th class="txt-center" style="width:20%"> SKU No. </th>
                            <th class="txt-center"> Product Description</th>
                            <th class="txt-center" style="width:10px"> </th>
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>
            </div>
        </div>

        <div class="popup-footer align-right">
            <button data-role="none" data-title="Close" class="btn custom-btn-theme-c btnCircle popup-close">
                <img class="img-arrow-left" style="width:18px;" />
            </button>
        </div>
    </div>
    
    <div id="popProductBinUnmaping" custom-data-role="popup">
        <div class="popup-md">
            <div class="popup-header">
                <span>More Options</span>
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">
                <div>
                    Are you sure you want to unMap <span id="unMapProduct"> </span> in <span id="unMapBin"> </span> bin

                    <br />

                </div>

                <button class="btn custom-btn-theme-c"> Yes, unmap this product </button>
                <button class="btn custom-btn-theme-c"> No, cancel this action </button>

            </div>

            <div class="popup-footer align-right">
                <button data-role="none" data-title="Close" class="btn custom-btn-theme-c btnCircle popup-close">
                    <img alt="Cancel" class="img-arrow-left" style="width:18px;" />
                </button>
            </div>
        </div>
    </div>

    <div id="popAutoGenerateBinTags" custom-data-role="popup">
        <div class="popup-md">
            <div class="popup-header">
                <span>Auto Generate Bin Tags</span>
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">

                <br />

                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <div>
                            <h4 class="btnFOWST"> </h4>
                        </div>
                    </div>
                    <div class="ui-block-b" style="text-align:right; padding-top:20px">
                        <a href="#" id="lnkBATExample">Click here to view example</a>
                    </div>
                </div>
                
                <br /> <br /> <br />

                <div class="ui-grid-a">
                    <div class="ui-block-a" style="width:50%">
                        <label>Which unit would you like to use to identify general bin locations?</label>
                    </div>
                    <div class="ui-block-b" style="text-align:center">
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <label><input type="radio" name="BinIdentifier" id="chkAisle" checked="checked">Aisles</label>
                            <label><input type="radio" name="BinIdentifier" id="chkRack">Rack</label>
                        </fieldset>

                    </div>
                </div>

                <div class="ui-grid-a">
                    <div class="ui-block-a" style="width:50%;padding-top:15px">
                        <label style="margin-top:5px"> Enter Area name for this <span id="lblAislesOrRack1">aisles</span></label>
                    </div>
                    <div class="ui-block-b">
                        <input type="text" id="txtArea" />
                    </div>
                </div>

                <div class="ui-grid-a">
                    <div class="ui-block-a" style="width:50%;padding-top:15px">
                        <label style="margin-top:5px"> How many <span id="lblAislesOrRack2">aisles</span> do you have? </label>
                    </div>
                    <div class="ui-block-b">
                        <input type="number" id="txtRackOrAisles" />
                    </div>
                </div>

                <div class="ui-grid-a">
                    <div class="ui-block-a" style="width:50%;padding-top:15px">
                        <label> How many sections per rack do you have? </label>
                    </div>
                    <div class="ui-block-b">
                        <input type="number" id="txtSection" />
                    </div>
                </div>

                <div class="ui-grid-a">
                    <div class="ui-block-a" style="width:50%;padding-top:15px">
                        <label> How many shelves per section do you have? </label>
                    </div>
                    <div class="ui-block-b">
                        <input type="number" id="txtShelf" />
                    </div>
                </div>

                <br />

                <br />
                <br />

                <div style="text-align:right">
                    <button data-role="none" class="btn custom-btn-theme-b btn-less-width btn-sm" id="btnAutoGenerateBinAddresses">
                        <img class="img-plus-white" style="width:18px;" /> Generate Bin Addresses
                    </button>
                    <button data-role="none" class="btn custom-btn-theme-b btn-less-width btn-sm" id="btnCancelAutoGenerateBinAddresses">
                        <img class="img-cancel-white" style="width:18px;" /> No, Cancel Bin Generation
                    </button>
               </div>
            </div>

            <div class="popup-footer align-right">
                <button data-role="none" data-title="Close" class="btn custom-btn-theme-c btnCircle popup-close">
                    <img alt="Cancel" class="img-arrow-left" style="width:18px;" />
                </button>
            </div>
        </div>
    </div>

    <div id="popBATExample" custom-data-role="popup">
        <div class="popup-md">
            <div class="popup-header">
                <span>Bin Address Example</span>
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">
                
                <div style="text-align:center">
                    <img src="Content/images/BATExample.png" style="width:800px; height:500px"/>
                </div>
                <br />
                <br />
            </div>

            <div class="popup-footer align-right">
                <button data-role="none" data-title="Close" class="btn custom-btn-theme-c btnCircle popup-close">
                    <img alt="Cancel" class="img-arrow-left" style="width:18px;" />
                </button>
            </div>
        </div>
    </div>

    <div id="popDeactivateBin" custom-data-role="popup">
        <div class="popup-md">
            <div class="popup-header">
                <span class="lblBinLabelHeader"></span>
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <div>
                            <h4 class="btnFOWST"> </h4>
                        </div>
                    </div>
                    <div class="ui-block-b">
                    </div>
                </div>
                
                <p>The following products are mapped to this bin:</p>

                <table id="tblFowstBinProduct" data-role="table" data-role="reflow" style="margin-right:5px; display:table;" class="display ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                    <thead>
                        <tr class="custom-theme-f fixed-cell-width">
                            <th class="txt-center"> Product Description </th>
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>

                <br />
                <br />

                When you deactivate this bin, the bin-specific quantities of these products will be set to zero. <br /> <br />
                Are you sure you want to continue deactivating this bin?

                <br />
                <br />
                <br />

                <div style="text-align:right">
                    <button data-role="none" class="btn custom-btn-theme-b btn-less-width btn-sm" id="btnDeactivateBin">
                        <img class="img-trash-white" style="width:18px;" /> Yes, Deactivate and Zero Out Products Mapped in this Bin
                    </button>
                    <br /> <br /> 
                    <button data-role="none" class="btn custom-btn-theme-b btn-less-width btn-sm" id="btnCancelDeactivationBin">
                        <img class="img-cancel-white" style="width:18px;" /> No, Cancel Deactivation of this Bin
                    </button>

                </div>
            </div>

            <div class="popup-footer align-right">
                <button data-role="none" data-title="Close" class="btn custom-btn-theme-c btnCircle popup-close">
                    <img alt="Cancel" class="img-arrow-left" style="width:18px;" />
                </button>
            </div>
        </div>
    </div>

    <div id="popAddItemBarcode" custom-data-role="popup">
        <div class="popup-md">
            <div class="popup-header">
                <span class="lblItemLabelHeader"></span>
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">
               
                <p>Enter Item Barcode</p>
                               
                <div style="text-align:right">
                    <button data-role="none" class="btn custom-btn-theme-b btn-less-width btn-sm" id="btnDeactivateBin">
                        <img class="img-trash-white" style="width:18px;" /> Save Item Barcode
                    </button>
                    <br /> <br />
                    <button data-role="none" class="btn custom-btn-theme-b btn-less-width btn-sm" id="btnCancelDeactivationBin">
                        <img class="img-cancel-white" style="width:18px;" /> Cancel Adding Item Barcode
                    </button>

                </div>
            </div>

            <div class="popup-footer align-right">
                <button data-role="none" data-title="Close" class="btn custom-btn-theme-c btnCircle popup-close">
                    <img alt="Cancel" class="img-arrow-left" style="width:18px;" />
                </button>
            </div>
        </div>
    </div>
</div>
    @*<div id="pageBinContent" data-role="page">
            <div data-role="header">
                <h1>@ViewData("Title")</h1>
            </div>

            <div data-role="content">
                <div data-role="fieldcontain">
                    <fieldset data-role="controlgroup">
                        <legend> Bin Search: </legend>
                         <input type="text" id="txtBINSearch"/>
                         <a href="#" id="btnBinGo" data-role="button">Go</a>
                         <a href="#" id="btnScanBarcodeBin" data-role="button">Scan QR or Bar Code</a>
                    </fieldset>
                </div>

                <div data-role="fieldcontain">
                    <ul data-role="listview" id="lstBinItems" data-inset="true" data-split-icon="delete">
                    </ul>
                </div>
            </div>
        </div>*@

    <div id="pageAddBin" data-role="page">
        <div data-role="header">
            <h1>Bin</h1>
        </div>

        <div data-role="content">
            <div data-role="fieldcontain">
                <label>
                    Can’t find the Bin
                    Location you are
                    looking for?
                </label>
            </div>

            <div data-role="fieldcontain">
                <a href="#" id="btnRetrySearchBin" data-role="button">Retry searching for the Bin Number</a>
            </div>

            <div data-role="fieldcontain">
                <a href="#" id="btnAddNewBin" data-role="button">Add a new Bin Number</a>
            </div>

            <div id="dvAddBin" style="visibility:hidden; display:none" data-role="fieldcontain">
                <div data-role="fieldcontain">
                    <label for="txtBinNumber">Input New Bin Number: </label> <br />
                    <input type="text" id="txtBinNumber" />
                </div>

                <a href="#" id="btnSaveBin" data-role="button">Save Bin</a>
            </div>
        </div>
    </div>

    <div id="pageRenameBin" data-role="dialog">
        <div data-role="header">
            <h1>Rename Bin</h1>
        </div>

        <div data-role="content">
            <label style="color:Red">
                Please put name to this Bin
            </label>

            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup">
                    <legend> Bin Name :  </legend>
                    <input type="text" id="txtNewBinName" />
                </fieldset>
            </div>

            <a href="#" id="btnSaveBinName" data-role="button">Save</a>
        </div>
    </div>
