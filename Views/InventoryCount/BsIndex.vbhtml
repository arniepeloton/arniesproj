@Code
    ViewData("Title") = "Inventory Count"
    
    If Request.IsAjaxRequest() Then
        Layout = ""
    Else
        Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
    End If
End Code

@Section head 

<link rel="stylesheet" href="/Content/jquery.dataTables2.css" />

    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v='], function () {
                require([GetPathFormat('/Scripts/sources/InventoryCount/Bs_InventoryCount.js?crc=')]);
            })
        })        
        sessionStorage.IsInventoryCountMode = false;
    </script>
     
<style>
    .divTitleHeader {
        white-space:nowrap;
        overflow:hidden;
        text-overflow:ellipsis;
    }

    .dataTables_wrapper{
        overflow: auto !important;
    }
</style>
End Section
 
<div id="pageMain" custom-data-role="page" style="display:block;">
    <nav class="navbar navbar-dark bg-primary sticky-top text-light pl-3 pr-3 mb-3">
        <div class="divTitleHeader">
            <button id="btnBackToMain" data-role="none" class="btn btn-primary">
                <span class="bi-chevron-left"></span>
            </button>
            @ViewData("Title")
        </div>
    </nav>

    <div class="container-fluidx">
        <div class="pl-3 pr-3 mb-3">
            <div class="w-100 border shadow-sm rounded-1 p-2">
                <div class="row align-items-center">
                    <div class="col-auto"><button data-role="none" class="btn bg-light text-secondary rounded-1"><span class="bi-person-fill"></span></button></div>
                    <div class="col-auto font-weight-bold">Logged-in user:</div>
                    <div class="col" id="lblLoggedInUser"></div>
                </div>
            </div>
        </div>

        <div class="row mb-3 g-0 not-summary pl-3 pr-3">
            <label class="col-auto col-form-label" for="selFOWST">Select FOWST:</label>
            <div class="col pl-3">
                <select id="selFOWST" data-role="none" class="form-select"></select>
            </div>
        </div>

        <div id="divICOption" class="row g-0 mb-3 pl-3 pr-3" style="display:none;">
            <label class="col-md-2 col-form-label" for="">How would you like to organize your count?</label>
            <div class="col-md-10">
                <select id="selInvCntOptions" data-role="none" class="form-select">
                    <option value="">Please select</option>
                    <option value="By Inventory-Count Products">By Inventory-Count Products</option>
                    <option value="By New Inventory-Count Products">By New Inventory-Count Products</option>
                    <option value="By Product Category">By Product Category</option>
                    <option value="By Product">By Product</option>
                    <option value="By Bin Location">By Bin Location</option>
                    @*<option value="By Area Location">By Area Location</option>
                    <option value="By Continuous Scanning of UPC/Barcode">By Continuous Scanning of UPC/Barcode</option>*@
                </select>
                <div class="list-group mb-3 d-none">
                    <a href="#" id="ByTableType" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center" style="padding:1em;"> <span class="bi-chevron-right"></span></a>
                    <a href="#" id="BySpatial" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center" style="padding:1em;"> <span class="bi-chevron-right"></span></a>
                    <a href="#" id="ByInventoryCountItems" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center" style="padding:1em;">By Inventory-Count Products  <span class="bi-chevron-right"></span></a>
                    <a href="#" id="ByBin" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center" style="padding:1em;"> <span class="bi-chevron-right"></span></a>
                    @*<a href="#" id="ByArea" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center" style="padding:1em;"> <span class="bi-chevron-right"></span></a>
                    <a href="#" id="ByUPCOrBarcode" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center" style="padding:1em;"> <span class="bi-chevron-right"></span></a>*@
                </div>
            </div>
        </div>

        <div id="PageCategories" subpage="invcnt" style="display:none;">
            <div class="">
                <h3 class="summary d-none">Summary</h3>
                <div class="row mb-3" style="display:none;">
                    <label class="col-md-2 col-form-label" for="btnSelectCategory">Category:</label>
                    <div class="col-md-10">
                        <button data-role="none" class="btn btn-secondary btn-block btnSelectCategory"><span class="lblSelectedCategory">Select Category</span></button>
                    </div>
                </div>

                <div class="not-summary row mb-3 d-none" id="divInventoryType">
                    <label class="col-md-2 col-form-label" for="selInventoryType">Inventory Type:</label>
                    <div class="col-md-10">
                        <select id="selInventoryType" data-role="none" class="form-select">
                            <option value="all">All</option>
                            <option value="finished">Finished Goods</option>
                            <option value="raw">Intermediate Goods and Raw Materials</option>
                        </select>
                    </div>
                </div>

                <div class="not-summary input-group mb-3 pl-3 pr-3">
                    <input data-role="none" type="search" class="form-control" placeholder="Search Product" id="txtSearchProducts">
                    <button data-role="none" class="btn btn-secondary" type="button" id="btnSearchProductBarcode">Scan Barcode</button>
                </div>

                <div id="dvDetails" style="width:100%; overflow:auto; margin-bottom:100px;">
                    <table id="tblProductsInCategories" data-role="none" class="table table-striped">
                        <thead>
                            <tr id="tblHeader" class="align-top text-center" style="font-size:14px;">
                                <th data-sort="string" style="cursor:pointer; text-align:center">Product Description</th>
                                <th data-sort="string" style="cursor:pointer; text-align:center"> </th>
                                <th data-sort="float" style="cursor:pointer; text-align:center; width:130px;">Observed Count</th>
                                @*<th data-sort="float" style="cursor:pointer; text-align:center">Inventory Balance From Server</th>
                                <th data-sort="float" style="cursor:pointer; text-align:center">Last Observed Count Input </th>
                                <th data-sort="float" style="cursor:pointer; text-align:center">Variance</th>
                                <th data-sort="float" style="cursor:pointer; text-align:center">Add Remarks</th>*@
                            </tr>
                        </thead>
                        <tbody></tbody>
                    </table>

                    @*<table style="margin-right:5px; display:table;" class="ui-table table-stroke table-stripe">
                        <thead>
                            <tr id="tblHeader" class="custom-theme-f">
                                <th data-sort="string" style="cursor:pointer; text-align:center">Product Description</th>
                                <th data-sort="string" style="cursor:pointer; text-align:center">Unit Measure</th>
                                <th data-sort="float" style="cursor:pointer; text-align:center">Input Observed Count</th>
                                <th data-sort="float" style="cursor:pointer; text-align:center">Inventory Balance From Server</th>
                                <th data-sort="float" style="cursor:pointer; text-align:center">Last Observed Count Input </th>
                                <th data-sort="float" style="cursor:pointer; text-align:center">Variance</th>
                                <th data-sort="float" style="cursor:pointer; text-align:center">Add Remarks</th>
                            </tr>
                        </thead>
                        <tbody id="tblBody"></tbody>
                    </table>*@
                </div>


                @*style="visibility:hidden; display:none"*@
                @*<ul data-role="listview" id="lstCategory" data-inset="true">
                    <li data-role="list-divider"> Select Category </li>
                </ul>

                <br /> <br />
                <a id="btnTurnOverInventory" data-role="button" style="font-size:large"><h4>Turn-Over Inventory</h4></a>*@

                <div class="text-center p-3 fixed-bottom bg-light border-top">
                    <button id="butInventoryCountBack" data-role="none" class="btn btn-secondary btn-lg">Back</button>
                    <button id="btnSaveObservedCountByTableList" data-role="none" class="btn btn-success btn-lg">Save</button>
                </div>
            </div>

            <div data-role="popup" id="panSelectInventoryBalance" data-overlay-theme="a" data-theme="a" data-corners="false">
                <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="btnInventoryBalanceClosePopUp">Close</a>
                <center><h3 style="margin-left:10px; margin-right:10px;">Inventory Count<span class="lblPopUpHeaderLibraryName"></span></h3></center>
                <div data-role="content" style="max-width:450px;" class="ui-content ui-body-d" data-theme="a">
                    <div style="margin-left:10px; margin-right:10px;">

                        <center><h3 style="margin-left:10px; margin-right:10px;"><span id="lblPopUpProductName"></span> <span id="lblPopUpProductID" style="visibility:hidden"></span></h3></center>

                        <br />

                        <table style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe">
                            <thead>
                                <tr id="tblInventoryCountHeader" class="custom-theme-f">
                                    <th data-sort="string" style="cursor:pointer;">Bin Name</th>
                                    <th data-sort="float" style="cursor:pointer;">Unit Measure</th>
                                    <th style="cursor:pointer;">Observed Count</th>
                                </tr>
                            </thead>
                            <tbody id="tblBins"></tbody>
                        </table>

                        <a id="btnBinCount" data-role="button" style="font-size:large"><h4>Done</h4></a>

                    </div>
                    <br />
                </div>
            </div>

            <div id="popProductBinUnitMeasuresByCategory" class="modal fade">
                <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title"><span id="lblProductBinUnitMeasuresLabelByCategory"></span></h5>
                            <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        </div>
                        <div class="modal-body">
                            <h4 class="btnFOWST mb-3"> </h4>

                            <table id="tblProductBinUnitMeasuresByCategory" class="table table-striped mb-3">
                                <thead>
                                    <tr class="custom-theme-f fixed-cell-width">
                                        <th class="txt-center"> Unit Measure </th>
                                        <th class="txt-center"> Observed Count </th>
                                        @*<th class="txt-center"> Packing Size Quantity </th>*@
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>

                            @*<div class="row mb-3 mt-3">
                                <label class="col-md-2 col-form-label mb-3" for="txtMaxCap">Max Capacity:</label>
                                <div class="col-md-8">
                                    <input type="number" id="txtMaxCap" data-role="none" class="form-control" />
                                </div>
                                <div class="col-md-2">
                                    <span id="MaxCapUnitMeasure"></span>
                                </div>
                            </div>

                            <div class="row mb-3">
                                <label class="col-md-2 col-form-label mb-3" for="txtReorderPoint">Reorder Point:</label>
                                <div class="col-md-10">
                                    <input type="number" id="txtReorderPoint" data-role="none" class="form-control" />
                                </div>
                            </div>

                            <div class="row mb-3">
                                <label class="col-md-2 col-form-label mb-3" for="txtReorderQty">Reorder Qty:</label>
                                <div class="col-md-10">
                                    <input type="number" id="txtReorderQty" data-role="none" class="form-control" />
                                </div>
                            </div>*@
                        </div>
                        <div class="modal-footer">
                            <div class="row">
                                <div class="col">
                                    <button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-lg btn-block">Cancel</button>
                                </div>
                                <div class="col">
                                    <button data-role="none" id="btnDoneProductBinUnitMeasuresByCategory" class="btn btn-success btn-lg btn-block">Ok</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="PageInventoryCountByCategories" subpage="invcnt" style="display:none;">
            <div class="row justify-content-center g-0">
                <div class="col-12 col-md-10 col-lg-8 col-xl-6">
                    <div class="input-group pl-3 pr-3 p-md-0 mb-3">
                        <input data-role="none" type="text" id="txtSearchICItems" class="form-control" placeholder="Search Product" />
                        @*<button data-role="none" class="btn btn-light border shadow-sm d-inline-flex align-items-center"><span class="bi-upc-scan mr-2"></span> Scan Barcode</button>*@
                    </div>

                    <div class="row pl-3 pr-3 p-md-0 g-0 mb-3">
                        <div class="col">
                            <div class="w-100 h-100 d-flex justify-content-center bg-danger-light shadow-sm rounded-1 p-2">
                                <div class="row align-items-center g-2 rounded-2">
                                    <div class="col text-danger">Missing</div>
                                    <div class="col-auto">
                                        <div id="lblNoOfPendingItems" class="w-100 font-weight-bold fs-2 fs-sm-6 text-right"></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col p-0 pl-3 pr-3">
                            <div class="w-100 h-100 d-flex justify-content-center bg-primary-light shadow-sm rounded-1 p-2">
                                <div class="row align-items-center g-2 rounded-2">
                                    <div class="col text-primary">Ready to submit</div>
                                    <div class="col-auto">
                                        <div id="lblNoOfItemsToBeSubmitted" class="w-100 font-weight-bold fs-2 fs-sm-6 text-right"></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col">
                            <div class="w-100 h-100 d-flex justify-content-center bg-success-light shadow-sm rounded-1 p-2">
                                <div class="row align-items-center g-2 rounded-2">
                                    <div class="col text-success">Submitted</div>
                                    <div class="col-auto">
                                        <div id="lblNoOfConfirmedItems" class="w-100 font-weight-bold fs-2 fs-sm-6 text-right"></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <ul id="listICCategories" class="nav-container overflow-auto border p-3 list-group list-group-horizontal list-unstyled"></ul>

                    <div id="listICCategorySku" style="margin-bottom:70px;"></div>

                    <div class="text-center p-3 fixed-bottom bg-light border-top">
                        <div class="row justify-content-center">
                            <div class="col-12 col-md-10 col-lg-8 col-xl-6">
                                <div class="row justify-content-between">
                                    <div class="col-auto">
                                        <button id="btnBackIC" data-role="none" class="btn btn-light border rounded-1">Back</button>
                                    </div>
                                    <div class="col-auto">
                                        <button id="btnSubmitIC" data-role="none" class="btn btn-success rounded-1">Submit Inventory Count</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="popProductUom" class="modal fade">
                <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                    <div class="modal-content">
                        <div class="modal-header border-0">
                            <div class="row align-items-center">
                                <div class="col">
                                    <div><b class="lblProductName">Chicken Fries 2KG</b></div>
                                    <div class="text-secondary"><span class="lblProductNo"></span> <span class="bi-dot"></span> Unit: <span class="lblProductUom"></span></div>
                                </div>
                                <div class="col-auto">
                                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                </div>
                            </div>
                        </div>
                        <div class="modal-body">
                            <div id="divNegativeValues" class="mb-3 bg-danger-light text-danger p-3 rounded-1" style="display:none;">
                                <div class="row align-items-center">
                                    <div class="col-auto">
                                        <span class="bi-exclamation-circle-fill"></span>
                                    </div>
                                    <div class="col">
                                        Value cannot be negative
                                    </div>
                                </div>
                            </div>
                            <div id="listProductUom" class="row align-items-center"></div>
                        </div>
                        <div class="modal-footer border-0">
                            <div class="row gx-2 justify-content-end">
                                <div class="col-auto">
                                    <button data-role="none" data-dismiss="modal" class="btn btn-light border rounded-1 shadow-sm">Cancel</button>
                                </div>
                                <div class="col-auto">
                                    <button data-role="none" id="btnConfirm" class="btn border rounded-1 shadow-sm">Confirm</button>
                                </div>
                                <div class="col-auto">
                                    <button data-role="none" id="btnConfirmAndNext" class="btn btn-success rounded-1 shadow-sm">Confirm & Next</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="text-center mt-5 mb-5 d-none">
            <button id="btnInventoryBack" data-role="none" class="btn btn-secondary btn-lg">Back To Home</button>
        </div>
    </div>  
</div>

<div id="pageSpatial" custom-data-role="page" style="display:none;">
    <div data-role="header">        
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">
        <div data-role="collapsible" data-iconpos="right" data-collapsed="false">
            <h3>Count Parameters</h3>
@*
            <div data-role="fieldcontain" class="ui-radio">
                <fieldset data-role="controlgroup">
                    <legend> Are you counting good stocks? </legend>

                    <input type="radio" name="rdChoiceGood" id="rcChoiceGoodYes" checked="checked"/>
         	        <label for="rcChoiceGoodYes">Yes</label>

         	        <input type="radio" name="rdChoiceGood" id="rcChoiceGoodNo"/>
         	        <label for="rcChoiceGoodNo">No</label>      
                </fieldset>
            </div>
            
            <div data-role="fieldcontain" class="ui-radio">
                <fieldset data-role="controlgroup">
                    <legend> Do you want to group the count for this SKU by expiration date(s)? </legend>

                    <input type="radio" name="rdChoiceExpiration" id="rcChoiceExpirationYes" />
         	        <label for="rcChoiceExpirationYes">Yes</label>

         	        <input type="radio" name="rdChoiceExpiration" id="rcChoiceExpirationNo" checked="checked"/>
         	        <label for="rcChoiceExpirationNo">No</label>
                </fieldset>
            </div>*@

            <div id="dvDate" data-role="fieldcontain" style="visibility:hidden; display:none">
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <legend> </legend>                                
	                <select id="optMonthValue" data-inline='true'> </select>                
	                <select id="optDayValue" data-inline='true'> </select>                
	                <select id="optYearValue" data-inline='true'> </select>
                </fieldset>
            </div>

            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup">
                    <legend> Bin/Shelf Location: </legend>
                     <select id="selBins">
                        <option> Select Bin/Shelf </option>
                    </select>
                </fieldset>
            </div>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
                <legend> SKU Search: </legend>                
                 <input type="text" id="txtSKU"/>
                 <a href="#" id="btnSKUGo" data-role="button">Go</a> 
                 <a href="#" id="btnScanBarcodeSKU" data-role="button">Scan QR or Bar Code</a>                  
            </fieldset>
        </div>       
    </div>

</div>

<div id="pageStockViewer" custom-data-role="page" style="display:none;">
    <div data-role="header">        
        <h1>@ViewData("Title")</h1>
        <a href="#" data-icon="arrow-l" data-rel="back" >Back</a>
    </div>

         <div class="ui-bar-c" style="border: none">
         <div data-role="content">        
             <div class="ui-grid-a">
                <div class="ui-block-a" style="width: 43%; text-align: center">
                    <img src="Content/images/box.png" />
                </div>
                <div class="ui-block-b" style="width: 57%">
                    <h3>
                        SKU #: <span id="lblStockNumber"></span>
                        <br />
                        <span id="lblStockDescription"></span><a href="#">(MORE INFO)</a>
                    </h3>
                </div>
            </div>

        @*<div data-role="fieldcontain" class="ui-radio">
            <fieldset data-role="controlgroup">
                <legend> Are you counting good stocks? </legend>

                <input type="radio" name="rdChoiceGood" id="rcChoiceGoodYes" checked="checked"/>
         	    <label for="rcChoiceGoodYes">Yes</label>

         	    <input type="radio" name="rdChoiceGood" id="rcChoiceGoodNo"/>
         	    <label for="rcChoiceGoodNo">No</label>      
            </fieldset>
        </div>
            
        <div data-role="fieldcontain" class="ui-radio">
            <fieldset data-role="controlgroup">
                <legend> Do you want to group the count for this SKU by expiration date(s)? </legend>

                <input type="radio" name="rdChoiceExpiration" id="rcChoiceExpirationYes" />
         	    <label for="rcChoiceExpirationYes">Yes</label>

         	    <input type="radio" name="rdChoiceExpiration" id="rcChoiceExpirationNo" checked="checked"/>
         	    <label for="rcChoiceExpirationNo">No</label>
            </fieldset>
        </div>*@

        <br />
        <br />
            <div data-role="collapsible" data-iconpos="right" data-collapsed="false" data-theme="b" style="border: 1px solid; border-top:none;">
                <h3>Input Count</h3>
            
                <div data-role="fieldcontain">
                    <label for="selBinsStockViewer"> Bin/Shelf Location: </label>
                    <select id="selBinsStockViewer">
                        <option> Select Bin/Shelf </option>
                    </select>                    
                </div>

                <div data-role="fieldcontain">
                    <label for="txtBinMaxCap"> Max Cap (Qty) for this Bin/Shelf : </label>
                    <input type="text" id="txtBinMaxCap" />
                </div>

                <div data-role="fieldcontain">
                    <label for="txtBinMinCap"> Min Cap (Qty) for this Bin/Shelf : </label>
                    <input type="text" id="txtBinMinCap" />
                </div>

                <hr />

                <h4>Input Observed Counts:</h4>

                <div data-role="fieldcontain">
                   <fieldset data-role="controlgroup" data-type="horizontal">
                        <legend><b>&nbsp; &nbsp; Expiration Date </b></legend>
                        <fieldset id="txtExpirationDate" data-role="DateCombo"> </fieldset>
                    </fieldset>
                </div>

                <div data-role="fieldcontain">
                    <form>
                        <ul data-role="listview" id="lstUnitMeasureFrame" data-inset="true" data-icon="delete">
                            <li>
                                <table id="lstUnitMeasure" style="width:100%">
                    
                                </table>    
                            </li>
                        </ul>
                    </form>
                </div>

                <div data-role="fieldcontain">	        
	                <div> <a id="btnDoneUnitMeasure" href="#" data-role="button" data-theme="e">Add Observed Counts to Tally</a> </div>
                </div>
            </div>

        <br />
        <br />

         <div data-role="collapsible" data-iconpos="right" data-collapsed="false" data-theme="b" style="border: 1px solid; border-top:none;">
            <h3>Tallies</h3>
                <ul data-role="listview" id="lstCounts" data-inset="true" data-split-icon="delete">
                </ul>
         </div>

         <br />        
        <br />

        <div id="dvSave">
            <a href="#" id="btnSaveTallies" data-role="button" data-theme="e">Save Tallies</a> 
         </div>
    </div>
    </div>

    @*<div class="popup-md" id="popSMSLogs" custom-data-role="popup">
        <div class="popup-header">
            Bins
            <a class="popup-close">x</a>
        </div>

        <div class="popup-content">
            <table id="tblSizesUnitMeasureFrame" data-role="table" data-mode="reflow" class="ui-responsive">
                <thead>
                    <tr>
                        <th> Product Name </th>
                        <th> Quantity </th>
                    </tr>
                </thead>
                <tbody id="lstBins"></tbody>
            </table>   
        </div>

        <div class="popup-footer">

        </div>
    </div>*@
</div>

<div id="pageStockViewerDetail" custom-data-role="page" style="display:none;">
    <div data-role="header">        
        <h1>@ViewData("Title")</h1>
         <a href="#" data-icon="arrow-l" data-rel="back" >Back</a>
    </div>

    <div data-role="content">
       @* <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
                <legend> Product Number: </legend>                 
                 <label id='lblStockNumber-Detail' style="font-weight:bold"></label>                 
            </fieldset>
        </div>    
        
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
                <legend> Product Name: </legend>                 
                 <label id='lblStockDescription-Detail' style="font-weight:bold"></label>
            </fieldset>
        </div>*@
        <div class="ui-bar-c" style="border: none">

                <form>
                    <ul data-role="listview" id="lstSizesUnitMeasureFrame" data-inset="true" data-icon="delete">
                        <li data-role="list-divider">
                            <div data-role="fieldcontain" style="height:5px;">
                                Count by Sizes                            
                            </div>
                        </li>
                        <li>
                            <div data-role="fieldcontain">
                                <fieldset data-role="controlgroup">
                                    <legend> Bin/Shelf Location: </legend>
                                        <select id="selBinsStockViewerMultipleSize">
                                        <option> Select Bin/Shelf </option>
                                    </select>
                                </fieldset>
                            </div>
                            <table id="tblSizesUnitMeasureFrame" data-role="table" data-mode="reflow" class="ui-responsive">
                                <thead>
                                    <tr>
                                        <th> Product Name </th>
                                        <th> Size </th>
                                        <th> Unit Measure </th>
                                        <th> Count </th>
                                    </tr>
                                </thead>
                                <tbody id="lstSizesUnitMeasure">
                            
                                </tbody>
                            </table>    
                        </li>
                    </ul>
                </form>
        
        </div>
         <a href="#" id="btnSaveSizesTallies-Detail" data-role="button">Save</a> 
    </div>
</div>

<div id="pageGroup" custom-data-role="page" style="display:none;">
    <div data-role="header">        
        <h1>@ViewData("Title")</h1>
        <a href="#" data-icon="arrow-l" id="btnBack">Back</a>
    </div>
    <div data-role="content"> 
            <div style="text-align:center" class="ui-bar ui-bar-b" id="GroupTitle"> </div>
            <div id="dvProductGroup"> 
                <div id="dvGroup">
                    <fieldset data-role="controlgroup">
                        <legend id="lblGroup"> </legend>                
                        <select id="selGroup"> </select>
                        <a id="btnSearchGroup" data-role="button">Go</a>
                    </fieldset>
                </div>
            
                <div id="dvGroupDate" style="visibility:hidden; display:none">
                    <table width="100%">
                        <tr> 
                            <td> From : </td> 
                            <td> <fieldset id="txtDateFrom" data-role="DateCombo" data-usecurrent="1"> </fieldset> </td>
                        </tr>
                        <tr>
                            <td> To :  </td> 
                            <td> <fieldset id="txtDateTo" data-role="DateCombo" data-usecurrent="1"> </fieldset> </td>                        
                        </tr>
                    </table>

                    <div>
                        <a id="btnGoDateRange" href="#" data-role="button">Go</a>
                    </div>
                </div>
            </div>

        <div data-role="fieldcontain">
            <div style="text-align:center" class="ui-bar ui-bar-b"> <h3> Products </h3> </div>
        </div>
        
            <input type="text" id="txtSearchItem" placeholder="Search products"/>
            <a id="btnScanBarCode" data-role="button">Press to Scan Bar Code</a>
         
            <div data-role="fieldcontain">            
                <ul data-role="listview" id="lstProducts">
                    <li data-role="list-divider"> </li>
                </ul>
            </div>
    </div>
</div>

<div id="pageCountByBinAndArea" custom-data-role="page" style="display:none;">
    <nav class="navbar navbar-dark bg-primary sticky-top text-light pl-3 pr-3 mb-3">
        <div class="divTitleHeader">
            <button data-role="none" class="btn btn-primary btnBackToInventoryCountPage">
                <span class="bi-chevron-left"></span>
            </button>
            Inventory Count <span class="lblInventoryType"></span>
        </div>
    </nav>

    <div class="container-fluid">
        <div class="summary mb-3" style="display:none;">
            <h3>Summary</h3>
            <div>Please review your changes before you save them.</div>
        </div>
        

        <div id="pageCountByBin">
            <div class="row mb-3 d-none">
                @*pati kaning filter, add, search, apil ni sa original UI for Bin Loc, pakiremove lang ang 'd-none'*@
                <div class="col-md-6 order-md-0 mb-3">
                    <div class="input-group">
                        <input data-role="none" type="search" class="form-control" placeholder="Search Bin" id="txtSearchNamedBin">
                        <button data-role="none" class="btn btn-secondary" type="button" id="butBinBarcode">Scan Barcode</button>
                    </div>
                </div>

                <div class="col-md-6 order-md-1">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" class="btn btn-success btn-block" id="btnManuallyAddBinTags" style="height:100%;">
                                Add Bin Address
                            </button>
                        </div>
                        <div class="col">
                            <button data-role="none" class="btn btn-warning btn-block" id="btnFilterBinList">
                                <span id="lblFilterBins">Bin List Filtered By: Active Bins</span>
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            <div class="mb-5 table-responsive">
                @*START of original UI for Bin Loc
        <table id="tblNamedBins" class="table table-striped">
            <thead>
                <tr class="custom-theme-f fixed-cell-width">
                    <th class="txt-center"> Area </th>
                    <th class="txt-center"> Bin Address </th>
                    <th class="txt-center"> Remarks </th>
                    <th class="txt-center"> Status </th>
                    <th class="txt-center"> No of products mapped </th>
                    <th class="txt-center"> ID </th>
                </tr>
            </thead>
            <tbody></tbody>
        </table>*
        END of original UI for Bin Loc*@

                <div class="row justify-content-center g-0">
                    <div class="col-12 col-md-10 col-lg-8 col-xl-8">
                        <div class="row mb-3 not-summary">
                            <label class="col-md-auto col-form-label" for="selFOWST">Select FOWST:</label>
                            <div class="col-md">
                                <select id="lblWorkFowstName" data-role="none" class="form-select"></select>
                                @*<button data-role="none" id="lblWorkFowstName" class="btn btn-secondary btn-block">Select FOWST</button>*@
                            </div>
                        </div>

                        <div class="input-group pl-3 pr-3 p-md-0 mb-3">
                            <input data-role="none" type="text" id="" class="form-control" placeholder="Search Product" />
                            @*<button data-role="none" class="btn btn-light border shadow-sm d-inline-flex align-items-center"><span class="bi-upc-scan mr-2"></span> Scan Barcode</button>*@
                        </div>

                        <div class="row pl-3 pr-3 p-md-0 g-0 mb-3">
                            <div class="col">
                                <div class="w-100 h-100 d-flex justify-content-center bg-danger-light shadow-sm rounded-1 p-2">
                                    <div class="row align-items-center g-2 rounded-2">
                                        <div class="col text-danger">Missing</div>
                                        <div class="col-auto">
                                            <div id="lblBinMissingCount" class="w-100 font-weight-bold fs-2 fs-sm-6 text-right"></div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col p-0 pl-3 pr-3">
                                <div class="w-100 h-100 d-flex justify-content-center bg-primary-light shadow-sm rounded-1 p-2">
                                    <div class="row align-items-center g-2 rounded-2">
                                        <div class="col text-primary">Ready to submit</div>
                                        <div class="col-auto">
                                            <div id="lblBinReadyCount" class="w-100 font-weight-bold fs-2 fs-sm-6 text-right"></div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col">
                                <div class="w-100 h-100 d-flex justify-content-center bg-success-light shadow-sm rounded-1 p-2">
                                    <div class="row align-items-center g-2 rounded-2">
                                        <div class="col text-success">Submitted</div>
                                        <div class="col-auto">
                                            <div id="lblBinSubmittedCount" class="w-100 font-weight-bold fs-2 fs-sm-6 text-right"></div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <ul id="listBinCategories" class="nav-container bg-white rounded-2 overflow-auto border p-3 list-group list-group-horizontal list-unstyled"></ul>
                        
                        <div id="listBinCategorySku" @*style="margin-bottom:70px;"*@>
                            <div class="d-flex justify-content-center p-5">
                                <div class="spinner-border textvm-primary text-center" role="status">
                                    <span class="visually-hidden"></span>
                                </div>
                            </div>
                        </div>

                        <div class="text-center p-3 bg-light border-top">
                            <div class="row justify-content-center">
                                <div class="col-12">
                                    <div class="row justify-content-between">
                                        <div class="col">
                                            <button id="btnBinLocBack" data-role="none" class="btn btn-light border rounded-1">Back</button>
                                        </div>
                                        <div class="col">
                                            <button id="btnBinLocSave" data-role="none" class="btn btn-success rounded-1">Save</button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                @*OLD UI*@
                @*commented by Arnie 071326*@
                @*<table id="tblNamedBins" class="table table-striped">
                    <thead>
                        <tr class="custom-theme-f fixed-cell-width">
                            <th class="txt-center"> Bin No </th>
                            <th class="txt-center"> SKU No. </th>
                            <th class="txt-center"> SKU Name </th>
                            <th class="txt-center"> Base Unit Measure </th>
                            <th class="txt-center" style="width:120px;"> Observed Count </th>
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>*@
            </div>
        </div>

        <div id="pageCountByArea" data-role="content" style="display:none;">
            <div class="row mb-3">
                <div class="col-md-6 order-md-0 mb-3">
                    <div class="input-group">
                        <input data-role="none" type="search" class="form-control" placeholder="Search product or bin within the list" id="txtSearchAreaProduct">
                        <button data-role="none" class="btn btn-secondary" type="button" id="butAreaProductBarcode">Scan Barcode</button>
                    </div>
                </div>

                <div class="col-md-6 order-md-1">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" class="btn btn-success btn-block" id="btnArea" style="height:100%;">
                                Area: None
                            </button>
                        </div>
                        <div class="col">
                            <button data-role="none" class="btn btn-warning btn-block" id="btnBins">
                                <span id="lblBinAddress">Bin Address: None</span>
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            <div style="overflow:auto;">
                <table id="tblShelfPrices" class="table table-striped">
                    <thead>
                        <tr class="custom-theme-f fixed-cell-width">
                            <th class="txt-center"> BIN Address </th>
                            <th class="txt-center"> Remarks </th>
                            <th class="txt-center"> Product Description </th>
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>
            </div>
        </div>
    </div>    

    <div id="popProductBinCount" class="modal fade">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title"><span id="lblProductBinMappingLabel"></span></h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="row mb-3 align-content-center">
                        <div class="col">
                            <h4 class="btnFOWST"> </h4>
                        </div>
                        <div class="col text-right">
                            <button data-role="none" class="btn btn-success" id="btnAddProduct">
                                Add Product
                            </button>
                        </div>
                    </div>

                    <div class="input-group mb-3">
                        <input data-role="none" type="search" class="form-control" placeholder="Search product within the list" id="txtSearchBinProduct">
                        <button data-role="none" class="btn btn-secondary" type="button" id="butBinProductBarcode">Scan Barcode</button>
                    </div>

                    <div class="table-responsive">
                        <table id="tblBinProducts" class="table table-striped mb-3">
                            <thead>
                                <tr class="custom-theme-f fixed-cell-width">
                                    <th class="txt-center"> SKU No. </th>
                                    <th class="txt-center"> Description</th>
                                    <th class="txt-center"> Barcode(In base quantity)</th>
                                    <th class="txt-center"> Observed Count(In base quantity) </th>
                                    <th class="txt-center"> ProductSearchString </th>
                                    <th class="txt-center"> ProductID </th>
                                </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>                    
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-lg btn-block">Cancel</button>
                        </div>
                        <div class="col">
                            <button data-role="none" id="btnSaveObservedCountByBin" class="btn btn-success btn-lg btn-block">Save</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popProductBarcode" class="modal fade">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title"><span id="lblProductBinUnitMeasuresLabel"></span></h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <h4 class="btnFOWST mb-3"> </h4>

                    <table id="tblProductBarcode" class="table table-striped mb-3">
                        <thead>
                            <tr class="custom-theme-f fixed-cell-width">
                                <th class="txt-center"> Unit Measure </th>
                                <th class="txt-center"> Packing Size Quantity </th>
                                <th class="txt-center"> Current Barcode </th>
                                <th class="txt-center"> Enter Barcode </th>
                                <th class="txt-center"> </th>
                            </tr>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-lg btn-block">Cancel</button>
                        </div>
                        <div class="col">
                            <button data-role="none" id="btnDoneProductBarcode" class="btn btn-success btn-lg btn-block">Ok</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popProductBinUnitMeasures" class="modal fade">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title"><span id="lblProductBinUnitMeasuresLabel"></span></h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <h4 class="btnFOWST mb-3"> </h4>
                    
                    <table id="tblProductBinUnitMeasures" class="table table-striped mb-3">
                        <thead>
                            <tr class="custom-theme-f fixed-cell-width">
                                <th class="txt-center"> Observed Count </th>
                                <th class="txt-center"> Unit Measure </th>
                                <th class="txt-center"> Packing Size Quantity </th>
                            </tr>
                        </thead>
                        <tbody></tbody>
                    </table>

                    <div class="row mb-3 mt-3">
                        <label class="col-md-2 col-form-label mb-3" for="txtMaxCap">Max Capacity:</label>
                        <div class="col-md-8">
                            <input type="number" id="txtMaxCap" data-role="none" class="form-control" />
                        </div>
                        <div class="col-md-2">
                            <span id="MaxCapUnitMeasure"></span>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-md-2 col-form-label mb-3" for="txtReorderPoint">Reorder Point:</label>
                        <div class="col-md-10">
                            <input type="number" id="txtReorderPoint" data-role="none" class="form-control" />
                        </div>
                    </div>

                    <div class="row mb-3">
                        <label class="col-md-2 col-form-label mb-3" for="txtReorderQty">Reorder Qty:</label>
                        <div class="col-md-10">
                            <input type="number" id="txtReorderQty" data-role="none" class="form-control" />
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-lg btn-block">Cancel</button>
                        </div>
                        <div class="col">
                            <button data-role="none" id="btnDoneProductBinUnitMeasures" class="btn btn-success btn-lg btn-block">Ok</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>    
    
    <div id="popAddBinAddress" class="modal fade">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Add Bin Address</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label for="txtBinAddress" class="form-label">Enter Bin Address:</label>
                        <input data-role="none" type="text" class="form-control" id="txtBinAddress" />
                    </div>

                    <div class="mb-3">
                        <label for="txtBinRemarks" class="form-label">Enter Remarks:</label>
                        <textarea data-role="none" class="form-control" id="txtBinRemarks" style="height:100px; resize:none"></textarea>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-lg btn-block">Cancel</button>
                        </div>
                        <div class="col">
                            <button data-role="none" id="btnAddBinAddress" class="btn btn-success btn-lg btn-block">Add</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>



    @*<div id="pageBinContent" custom-data-role="page" style="display:none;">
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

<div id="pageAddBin" custom-data-role="page" style="display:none;">
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

<div id="pageCountInformation" custom-data-role="page" style="display:none;">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
                <legend> Fowst :  </legend>
                <label id="lblFowstInfo"></label>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
                <legend> Stock Number :  </legend>
                <label id="lblStockNumberInfo"></label>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
                <legend> Stock Description :  </legend>
                <label id="lblStockDescriptionInfo"></label>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
                <legend> Date Counted :  </legend>
                <label id="lblDateCountedInfo"></label>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <ul id="lstCountInformationDetails" data-role="listview"></ul>
        </div>

    </div>

</div>

<div id="pageRenameBin" class="modal fade">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Rename Bin</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    <label for="txtNewBinName" class="form-label">Bin Name: <span class="required">*</span></label>
                    <input data-role="none" type="text" class="form-control" id="txtNewBinName" />
                </div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col">
                        <button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-lg btn-block">Cancel</button>
                    </div>
                    <div class="col">
                        <button data-role="none" id="btnSaveBinName" class="btn btn-success btn-lg btn-block">Save</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="pageEmptyDefaultBin" custom-data-role="page" style="display:none;">

    <input type="text" id="txtSearchItem2" data-type="search" placeholder="Search products" />

    <div data-role="fieldcontain">
        <ul data-role="listview" data-filter="true" id="lstProducts2">
            <li data-role="list-divider" class="ui-li-count"> aaa </li>
        </ul>
    </div>


</div>

<div id="PageCountByUPC" custom-data-role="page" style="display:none;">
    <nav class="navbar navbar-dark bg-primary sticky-top text-light pl-3 pr-3 mb-3">
        <div class="divTitleHeader">
            <button data-role="none" class="btn btn-primary btnBackToInventoryCountPage">
                <span class="bi-chevron-left"></span>
            </button>
            Inventory Count <span class="lblInventoryType"></span>
        </div>
    </nav>

    <div class="container-fluid">
        <div class="text-right mb-3">
            <button id="btnCountByUPC" data-role="none" class="btn btn-secondary">Start Counting</button>
        </div>

        <div class="table-responsive mb-3">
            <table id="tblReportTable" class="table table-striped">
                <thead>
                    <tr class="custom-theme-f">
                        <th style="text-align:center">UPC/Barcode</th>
                        <th style="text-align:center">Product Description</th>
                        <th style="text-align:center">Unit Measure</th>
                        <th style="text-align:center">Total Count</th>
                    </tr>
                </thead>
                <tbody id="reportTable"></tbody>
            </table>
        </div>        

        <div class="text-center mb-5 mt-5">
            <button data-role="none" class="btn btn-secondary btn-lg btnBackToInventoryCountPage">Back</button>
            <button id="btnSaveAllContinousCount" data-role="none" class="btn btn-success btn-lg">Save All</button>
        </div>        
    </div>
</div>

<div id="PageCountByTableType" custom-data-role="page" style="display:none;">

    <div data-role="header">
        <h1>@ViewData("Title")</h1>
        <a href="#" data-icon="arrow-l" data-rel="back" id="btnBackToMain">Back</a>
    </div>

    <div data-role="popup" id="panRemarkPopup" data-overlay-theme="a" data-theme="a" data-corners="false" data-dismissible="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;" id="divAddProductTitle">Remarks</center>
        <div data-role="content" class="ui-content ui-body-d">
            <div style="padding: 8px 15px 16px 15px;" id="imgSelection">
                <textarea id="remarkText" cols="25" maxlength="250" style="text-align: none;"></textarea>
                <p id="remarkCount">250 char</p>
            </div>
            <div style="padding: 8px 15px 16px 15px;">
                <button id="btnSaveRemark" type="button">Save</button>
                <button id="btnClearRemark" type="button">Clear</button>
                <button id="btnCancelRemark" type="button">Cancel</button>
            </div>
        </div>
    </div>

    <div data-role="content">
        <div id="dvInventoryCount" style="width:100%; overflow:auto">
            <div class="ui-grid-a" style="margin:1px; padding: 0px;">
                <div class="ui-block-a" style="width:75%;padding: 8px;margin-bottom:3px;">
                    <input type="text" id="txtSearchProducts" data-type="search" placeholder="Search products" />
                </div>
                <div class="ui-block-b" style="width: 20%;padding: 0px;margin:3px;">
                    <center>
                        <div>
                            <img src="/Content/images/barcode96x96.png" style="height: 50px;" class="detailimage2" id="btnSearchProductBarcode" data-role="button" data-corners="false" />
                        </div>
                    </center>
                </div>
            </div>

            <div class="ui-grid-a" style="margin:1px; padding: 0px;">
                @*<div class="ui-block-a" style="width:45%;padding: 8px;margin-bottom:3px;">
                        <select id="selProductFilter">
                            <option value="optNoFilter" selected="selected">All Inventory Types</option>
                            <option value="optFinishedGoodsFilter">Finished Goods</option>
                            <option value="optRawMaterialsFilter">Intermediate Goods and Raw Materials</option>
                        </select>

                    </div>*@
                <div class="ui-block-b" style="width:50%;padding: 0px;margin:3px;">
                    <center>
                        <div>
                            <a data-role="button" id="btnGetBalancesFromServer">Get Inventory Balance from the Server</a>
                        </div>
                    </center>
                </div>
            </div>


            <br /> <br />


            <table data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe">
                <thead>
                    <tr id="tblHeader" class="custom-theme-f">
                        <th data-sort="string" style="cursor:pointer;">Product Description</th>
                        <th data-sort="string" style="cursor:pointer;">Unit Measure</th>
                        <th data-sort="float" style="cursor:pointer;">Input Observed Count</th>
                        <th data-sort="float" style="cursor:pointer; text-align:right">Inventory Balance From Server</th>
                        <th data-sort="float" style="cursor:pointer;">Last Inventory Count</th>
                        <th data-sort="float" style="cursor:pointer;">Variance</th>
                        <th data-sort="float" style="cursor:pointer;">Add Remarks</th>

                    </tr>
                </thead>
                <tbody id="tblInventoryCount"></tbody>
            </table>
        </div>

        <br /> <br />

        <a id="btnSaveObservedCountByTableList" data-role="button" style="font-size:large"><h4>Save</h4></a>
        <a id="btnSaveAndTurnOverObservedCountByTableList" data-role="button" style="font-size:large"><h4>Save And Turn Over Items</h4></a>
        <a data-rel="back" data-role="button" style="font-size:large"><h4>Back</h4></a>

        <div data-role="popup" id="popupMenu">
            <div data-role="popup" id="dvpopup" data-theme="a" class="ui-corner-all" data-dismissible="false" data-overlay-theme="a">
                <p id="popMessage" style="padding:10px 20px;text-align:center"></p>
            </div>
        </div>

    </div>

</div>

<div id="PageMyCounts" custom-data-role="page" style="display:none;">

    <div data-role="header">
        <h1>@ViewData("Title")</h1>
        <a href="#" data-icon="arrow-l" data-rel="back" id="btnBackToMain">Back</a>
    </div>

    <div data-role="content">
        <div id="dvInventoryCount" style="width:100%; overflow:auto">
            <br /> <br />


            <table data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe">
                <thead>
                    <tr id="tblHeader" class="custom-theme-f">
                        <th data-sort="string" style="cursor:pointer;">Product Description</th>
                        <th data-sort="string" style="cursor:pointer;">Unit Measure</th>
                        <th data-sort="float" style="cursor:pointer; text-align:right">Observed Count</th>
                        <th data-sort="float" style="cursor:pointer; text-align:right">Expected Value</th>
                        <th data-sort="float" style="cursor:pointer; text-align:right">Variance</th>
                        <th data-sort="float" style="cursor:pointer; text-align:right">Remarks</th>
                    </tr>
                </thead>
                <tbody id="tblMyInventoryCount"></tbody>
            </table>

            <br /> <br />

            <div data-role="collapsible" data-collapsed="false" data-theme="c" data-content-theme="c" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d">
                <h4>Turn Over Count</h4>

                <div data-role="fieldcontain">

                    <label for="txtDepositBy"> From: </label>
                    <input type="text" id="txtDepositBy" readonly />

                </div>

                <br />

                <div data-role="fieldcontain" id="dvDepositTo">
                    <label for="txtDepositTo"> To: </label>
                    <input type="text" data-type="search" id="txtDepositTo" placeholder="Search Employee" />
                </div>

                <br /> <br />

                <a id="btnSaveTurnOver" data-role="button" style="font-size:large"><h4>Save</h4></a>
                <a id="btnTurnOverBack" data-role="button" style="font-size:large"><h4>Back</h4></a>

            </div>
        </div>

    </div>


</div>

<div id="DialogUserAccess" class="modal fade">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">User Access</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div class="mb-3">Please Enter Employee Username and Password</div>

                <div class="mb-3" id="dvInvalidUsernameOrPassword" style="visibility:hidden; display:none">
                    <label style="color:red; font-weight:bold">* Invalid Username / Password</label>
                </div>

                <div class="mb-3">
                    <label for="txtUsername" class="form-label">Username:</label>
                    <input data-role="none" type="text" class="form-control" id="txtUsername" />
                </div>

                <div class="mb-3">
                    <label for="txtPassword" class="form-label">Username:</label>
                    <input data-role="none" type="password" class="form-control" id="txtPassword" />
                </div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col">
                        <button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-lg btn-block">Cancel</button>
                    </div>
                    <div class="col">
                        <button data-role="none" id="btnVerifyEmployee" class="btn btn-success btn-lg btn-block">Ok</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>