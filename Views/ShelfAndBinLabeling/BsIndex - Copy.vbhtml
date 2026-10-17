@Code
    ViewData("Title") = "Shelf And Bin Labeling"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code

@Section head   

    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
                require([GetPathFormat('/Scripts/sources/ShelfAndBinLabeling/BsIndex.js?crc=' + assetversion)]);
            })
        })
    </script>

End Section
 
<div id="pageMainBin" class="mb-5">
    <nav class="bg-primary text-white mb-3 sticky-top">
        <div class="container-fluid">
            <table style="width:100%;">
                <tr>
                    <td style="width:50px;">
                        <button data-role="none" id="btnShelfBack" class="btn btn-primary btn-lg btn-block text-white">
                            <span class="bi-chevron-left"></span>
                        </button>
                    </td>
                    <td><div><h5 class="m-0">Shelf and Bin Labeling</h5></div></td>
                </tr>
            </table>
        </div>
    </nav>

    <div class="container-fluid">
        <div class="row mb-3">
            <div class="col-auto col-form-label">FOWST:</div>
            <div class="col-12 col-sm-auto">
                <button data-role="none" id="lblWorkFowstName" class="btn btn-success btn-block">Please select FOWST</button>
            </div>
        </div>

        <div id="divIHModules" class="row row-cols-2 row-cols-sm-2 row-cols-md-3 row-cols-lg-4 row-cols-xl-5 row-cols-xxl-6 text-center">
            <div class="col mb-3">
                <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="liCreateShelfOrBinID">
                    <div class="bi bi-inbox rounded mb-3 module-buttons"></div>
                    Bin Address Tags
                </div>
            </div>
            <div class="col mb-3">
                <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="liDownloadShelfOrBinID">
                    <div class="bi bi-bookshelf rounded mb-3 module-buttons"></div>
                    Shelf Labels
                </div>
            </div>
            <div class="col mb-3">
                <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="liCreateBarcodeLabels">
                    <div class="bi bi-tag rounded mb-3 module-buttons"></div>
                    Item Labels
                </div>
            </div>
        </div>
    </div>

    <div id="popBinAddressTags" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-fullscreen">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Bin Address Tags</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    @*<div class="mb-3">

                        </div>*@

                    <div class="row" style="margin-bottom:1rem;">
                        <div class="col-sm-6">
                            <button data-role="none" class="btn btn-secondary mb-1" id="btnFilterBinList">
                                <span id="lblFilterBins">Bin List Filtered By: Active Bins</span>
                            </button>
                            <button data-role="none" class="btn btn-secondary mb-1" id="btnBATFOWST">
                                <span id="lblBATFOWST">FOWST: None</span>
                            </button>
                            <button data-role="none" class="btn btn-secondary mb-1" id="btnBATArea">
                                <span id="lblBATArea">Area: None</span>
                            </button>
                        </div>

                        <div class="col-sm-6" style="text-align:right">
                            <button data-role="none" class="btn btn-success mb-1" id="btnDownloadBinTagsPDF">
                                Download Bin Tags as PDF
                            </button>
                            <button data-role="none" class="btn btn-success mb-1" id="btnAutoGenerateBinTags">
                                Auto Generate Bin Tags
                            </button>
                            <button data-role="none" class="btn btn-success mb-1" id="btnManuallyAddBinTags">
                                Manually Add Bin Tags
                            </button>
                            <button data-role="none" class="btn btn-danger mb-1" id="btnDeactivateAllBins">
                                Deactivate All Bin Tags
                            </button>
                        </div>
                    </div>

                    @*<div class="row" style="margin-bottom:1rem;">
                        <div class="col-sm-6">                            
                        </div>

                        <div class="col-sm-6" style="text-align:right">                         
                            <button data-role="none" class="btn btn-danger mb-1" id="btnDeactivateAllBins">
                                Deactivate All Bin Tags
                            </button>
                        </div>
                    </div>*@

                    <div style="overflow:auto;">
                        <table id="tblBins" data-role="none" style="width:100%;" data-keyboard="true" class="table table-light table-striped table-hover headerfitwidth">
                            <thead>
                                <tr class="custom-theme-f fixed-cell-width">
                                    <th class="txt-center" style="width:10px" id="thSelectAll"> <a data-role="none" class="link-primary">Select All</a> </th>
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
                <div class="modal-footer">
                    <button data-role="none" class="btn btn-secondary btn-lg" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <div id="popShelfLabels" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-fullscreen">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Shelf Labels</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="row" style="margin-bottom:1rem;">
                        <div class="col-sm-6">
                            <button data-role="none" class="btn btn-secondary mb-1" id="btnFOWST">
                                <span id="lblFOWST">FOWST: None</span>
                            </button>
                            <button data-role="none" class="btn btn-secondary mb-1" id="btnShelfLabelArea">
                                <span id="lblArea">Area: None</span>
                            </button>
                            <button data-role="none" class="btn btn-secondary mb-1" id="btnShelfLabelBins">
                                <span id="lblBinAddress">Bin Address: None</span>
                            </button>
                            @*<button data-role="none" class="btn btn-secondary mb-1" id="btnShowSPT">
                                    <span id="lblShowDisplay">Show: All</span>
                                </button>*@
                        </div>
                        <div class="col-sm-6" style="text-align:right">
                            <button data-role="none" class="btn btn-success mb-1" id="btnAddProduct">
                                <span id="lblShowDisplay">Add product to bin</span>
                            </button>
                            <button data-role="none" class="btn btn-danger mb-1" id="btnDeleteProduct">
                                <span id="lblShowDisplay">Delete product from bin</span>
                            </button>
                            <button data-role="none" class="btn btn-success mb-1" id="btnDownloadShelfPriceTagsPDF">
                                Download shelf labels as PDF
                            </button>
                            <button data-role="none" class="btn btn-success mb-1" id="btnAddProductByScan">
                                Scan to add product
                            </button>
                        </div>
                    </div>

                    <div style="overflow:auto;">
                        <table id="tblShelfPrices" data-role="none" style="width:100%;" class="table table-light table-striped table-hover headerfitwidth">
                            <thead>
                                <tr class="custom-theme-f fixed-cell-width">
                                    <th class="txt-center" style="width:10px" id="thShelfSelectAll"> Select All </th>
                                    <th class="txt-center"> BIN </th>
                                    <th class="txt-center"> Product Description </th>
                                    <th class="txt-center"> Unit Measure </th>
                                    @*<th class="txt-center"> Price </th>
                                        <th class="txt-center"> Price Last Updated</th>*@
                                </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <button data-role="none" class="btn btn-secondary btn-lg" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <div id="popItemLabels" class="modal fade">
        <div class="modal-dialog modal-fullscreen">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Item Labels</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <button data-role="none" class="btn btn-secondary mb-1" id="btnDownloadProductLabelsPDF">
                            Download Item Labels as PDF
                        </button>
                        <button data-role="none" class="btn btn-secondary mb-1" id="btnProductArea">
                            <span id="lblProductArea">Area: None</span>
                        </button>
                        <button data-role="none" class="btn btn-secondary mb-1" id="btnProductBins">
                            <span id="lblProductBinAddress">Bin Address: None</span>
                        </button>
                        <button data-role="none" class="btn btn-secondary mb-1" id="btnProductBarcodeFilter">
                            <span id="lblProductBarcodeFilter">Show: All Products In Named Bins</span>
                        </button>
                    </div>

                    <div style="overflow:auto;">
                        <table id="tblProductBarcode" data-role="none" style="width:100%;" class="table table-light table-striped table-hover headerfitwidth">
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
                <div class="modal-footer">
                    <button data-role="none" class="btn btn-secondary btn-lg" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>
    
    <div id="popProductBinMapping" class="modal fade">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title"><span id="lblProductBinMappingLabel"></span></h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="row align-items-center">
                        @*<div class="col-sm-6 mb-3">
                            <h4 class="btnFOWST"> </h4>
                        </div>
                        <div class="col-sm-6 mb-3 text-right">
                            <button data-role="none" class="btn btn-success" id="btnAddProduct">
                                Add Product
                            </button>
                        </div>*@
                        <div class="col-sm-6 mb-3">
                            <button data-role="none" class="btn btn-success" id="btnAddProduct">
                                Add product using camera as scanner
                            </button>
                        </div>
                        <div class="col-sm-6 mb-3 text-right">
                            <button data-role="none" class="btn btn-success" id="btnAddProduct">
                                Add product using product library
                            </button>
                        </div>
                    </div>

                    @*<div class="table-responsive mb-3">
                        <table id="tblBinProducts" data-role="none" style="width:100%;" class="table table-light table-striped table-hover">
                            <thead>
                                <tr class="custom-theme-f fixed-cell-width">
                                    <th class="txt-center" style="width:20%"> SKU No. </th>
                                    <th class="txt-center"> Product Description</th>
                                    <th class="txt-center" style="width:10px"> </th>
                                </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>*@
                </div>
            </div>
        </div>
    </div>

    <div id="popAutoGenerateBinTags" class="modal fade">
        <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Auto Generate Bin Tags</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="row align-items-center">
                        <div class="col-md-6 mb-3">
                            <h4 class="btnFOWST"> </h4>
                        </div>
                        <div class="col-md-6 mb-3 text-right">
                            <button data-role="none" class="btn btn-success" id="lnkBATExample">Click here to view example</button>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-auto col-form-label">
                            <label>Which unit would you like to use to identify general bin locations?</label>
                        </div>
                        <div class="col-auto">
                            <div class="btn-group" role="group" aria-label="Basic checkbox toggle button group">
                                <input data-role="none" type="radio" class="btn-check" name="BinIdentifier" id="chkAisle" checked="checked">
                                <label class="btn btn-outline-dark" for="chkAisle">Aisles</label>

                                <input data-role="none" type="radio" class="btn-check" name="BinIdentifier" id="chkRack">
                                <label class="btn btn-outline-dark" for="chkRack">Rack Phasing</label>
                            </div>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-md-2"> Enter Area name for this <span id="lblAislesOrRack1">aisles</span>:</label>
                        <div class="col-md">
                            <select data-role="none" class="form-select" id="txtArea"></select>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-md-2"> How many <span id="lblAislesOrRack2">aisles</span> do you have? </label>
                        <div class="col-md" id="dvAislesOrRack2">
                            <input data-role="none" class="form-control" type="number" id="txtRackOrAisles" />
                        </div>

                        <label class="col-md-2"> How many sections per rack do you have? </label>
                        <div class="col-md" id="dvSection">
                            <input data-role="none" class="form-control" type="number" id="txtSection" />
                        </div>

                        <div class="col-md-2">
                            <button data-role="none" id="btnGo" class="btn btn-success btn-block">Go</button>
                        </div>

                        @*<label id="lblStartAtRackNo" class="col-md-3" style="display:none"> Generate bin tags starting at rack No. </label>
                        <div class="col-md-3">
                            <input data-role="none" class="form-control" type="number" id="txtStartAtRackNo" style="display:none" />
                        </div>*@
                    </div>
                

                    @*<div class="row mb-3">
                        
                        <label id="lblStartAtSectionNo" class="col-md-3" style="display:none"> Generate bin tags starting at section No. </label>
                        <div class="col-md-3">
                            <input data-role="none" class="form-control" type="number" id="txtStartAtSectionNo" style="display:none" />
                        </div>
                    </div>
                    
                    <div class="row mb-3">
                        <label class="col-md-3"> How many shelves per section do you have? </label>
                        <div class="col-md-9">
                            <input data-role="none" class="form-control" type="number" id="txtShelf" />
                        </div>
                    </div>*@

                    <div class="table-responsive">
                        <table class="table table-light table-striped table-hover">
                            <thead>
                                <tr class="align-top text-center" style="font-size:14px;">
                                    <th style="width:50px;">#</th>
                                    <th>FOWST Name</th>
                                    <th>Area Name</th>
                                    <th>Rack No.</th>
                                    <th>Section No.</th>
                                    <th style="width:150px;">No. of Shelves</th>
                                    <th>Generated BATs</th>
                                </tr>
                            </thead>
                            <tbody id="tbodyAutoGenerateBinTags"></tbody>
                        </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-md-6 order-1 order-md-0">
                            <button data-role="none" id="btnCancelAutoGenerateBinAddresses" style="height:100%;" class="btn btn-secondary btn-block">No, Cancel Bin Generation</button>
                        </div>
                        <div class="col-md-6 order-0 pb-sm-3 pb-md-0 order-md-1">
                            <button data-role="none" id="btnAutoGenerateBinAddresses" style="height:100%;" class="btn btn-success btn-block">Generate Bin Addresses</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popBATExample" class="modal fade">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Bin Address Example</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div style="text-align:center; ">
                        <img src="/Content/images/BATExample.png" style="width:100%;" />
                    </div>
                </div>
            </div>
        </div>
    </div>    

    <div id="popDeactivateBin" class="modal fade">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title"><span class="lblBinLabelHeader"></span></h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <h4 class="btnFOWST mb-3"> </h4>

                    <p>The following products are mapped to this bin:</p>

                    <div class="table-responsive mb-3">
                        <table id="tblFowstBinProduct" data-role="none" style="width:100%;" class="table table-light table-striped table-hover">
                            <thead>
                                <tr class="custom-theme-f fixed-cell-width">
                                    <th class="txt-center"> Product Description </th>
                                </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>

                    <p>
                        When you deactivate this bin, the bin-specific quantities of these products will be set to zero.
                        Are you sure you want to continue deactivating this bin?
                    </p>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-sm-6 order-1 order-sm-0">
                            <button data-role="none" id="btnCancelDeactivationBin" style="height:100%;" class="btn btn-secondary btn-block">No, Cancel Deactivation of this Bin</button>
                        </div>
                        <div class="col-sm-6 order-0 order-sm-1">
                            <button data-role="none" id="btnDeactivateBin" style="height:100%;" class="btn btn-success btn-block">Yes, Deactivate and Zero Out Products Mapped in this Bin</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popAddItemBarcode" class="modal fade">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title"><span class="lblItemLabelHeader"></span></h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <p>Enter Item Barcode</p>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-sm-6 order-1 order-sm-0">
                            <button data-role="none" id="" data-dismiss="modal" style="height:100%;" class="btn btn-secondary btn-block">Cancel Adding Item Barcode</button>
                        </div>
                        <div class="col-sm-6 order-0 order-sm-1">
                            <button data-role="none" id="" style="height:100%;" class="btn btn-success btn-block">Save Item Barcode</button>
                        </div>
                    </div>
                </div>
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