@Code
    ViewData("Title") = "Inventory Count"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head 

<link rel="stylesheet" href="/Content/flatui-css-custom.css" />
<link rel="stylesheet" href="/Content/bootstrap-custom.css" />
<link rel="stylesheet" href="/Content/jquery.dataTables2.css" />

    <script>
        require([GetPathFormat('/Scripts/sources/InventoryCount/InventoryCount.js?crc=')]);
        sessionStorage.IsInventoryCountMode = false;
    </script>
     
<style>
    .btnRequired {
        -webkit-transition: box-shadow 1s linear;
        -moz-transition: box-shadow 1s linear;
        -ms-transition: box-shadow 1s linear;
        -o-transition: box-shadow 1s linear;
        transition: box-shadow 1s linear;
        border: 2px solid #FFF;
    }

    .btnRequired {
        border: 2px solid #F00;
    }

    .btnNotRequired {
        border: 2px solid #FFF;
    }
</style>
End Section
 
<div id="pageMain" data-role="page">
    <div data-role="header">        
        <h1>@ViewData("Title")</h1>
        <a href="#" data-icon="arrow-l" id="btnBackToMain">Back</a>
    </div>

    <div data-role="content">
        
        <div data-role="fieldcontain">
            <label for="selFOWST">FOWST</label>
            <select id="selFOWST"></select>
        </div>

        <div>
            <ul data-role="listview" data-inset="true">
                <li data-role="list-divider"> How would you like to organize your count? </li>
                <li><a id="ByTableType"><h1>By Product Category</h1></a></li>
                <li><a id="BySpatial">By Product</a></li>
	            <li><a id="ByBin">By Bin Location</a></li>
                <li><a id="ByArea">By Area Location</a></li>
                @*<li><a id="ByBrand">By Brand</a></li>
    <li><a id="ByNegativeBalances">By Negative Balances</a></li>
    <li><a href="" id="ByRank">By Rank</a></li>
    <li><a id="ByRecentlyReceived">By Recently Received and Transferred</a></li>
    <li><a href="" id="ByVendor">By Vendor</a></li>*@
    <li><a href="" id="ByUPCOrBarcode">By Continuous Scanning of UPC/Barcode</a></li>
            </ul>
        </div>
        
        <br />
        <br />
        <div style="text-align: center">
            <a id="btnInventoryBack" data-role="button" style="font-size:large"><h4>Back To Home</h4></a>
        </div>
        
    </div>  
</div>

<div id="pageSpatial" data-role="page">
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

<div id="pageStockViewer" data-role="page">
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

<div id="pageStockViewerDetail" data-role="page">
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

<div id="pageGroup" data-role="page">
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

<div id="pageCountByBinAndArea" data-role="page">

    <div>
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

    <div id="pageCountByBin" data-role="content">

        <div class="ui-grid-a ui-responsive">
            <div class="ui-block-a">
                <button data-role="none" class="btn custom-btn-theme-b btn-less-width btn-sm" id="btnManuallyAddBinTags">
                    <img class="img-plus-white" style="width:18px;" /> Add Bin Address
                </button>                
            </div>            
            <div class="ui-block-b">
                <button data-role="none" class="btn custom-btn-theme-e btn-less-width btn-sm" id="btnFilterBinList">
                    <img class="img-filter" style="width:18px;" /> <span id="lblFilterBins">Bin List Filtered By: Active Bins</span>
                </button>
            </div>
        </div>

        <br /> <br />
        <div class="ui-grid-b">
            <div class="ui-block-a" style="width:30%; text-align:left">
                <div style="padding-top:15px">
                    <label>Search Bin </label>
                </div>
            </div>
            <div class="ui-block-b" style="width:55%; text-align:left">
                <input type="text" id="txtSearchNamedBin" />
            </div>
            <div class="ui-block-c" style="width:15%; text-align:left">
                <div>
                    <img src="/Content/images/barcode96x96.png" style="height: 45px;" class="detailimage2" id="butBinBarcode" data-role="button" data-corners="false" />
                </div>
            </div>
        </div>

        <br />

        <div>
            <table id="tblNamedBins" style="margin-right:5px; display:table;" class="display ui-table table-stroke table-stripe">
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
            </table>
        </div>

    </div>

    <div id="popProductBinCount" custom-data-role="popup">
        <div class="popup-md">
            <div class="popup-header">
                <span id="lblProductBinMappingLabel"></span>
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">

                <div class="ui-grid-a">
                    <div class="ui-block-a" style="margin-bottom:10px">
                            <h4 class="btnFOWST"> </h4>
                    </div>
                    <div class="ui-block-b">
                        <div style="text-align:right">
                            <button data-role="none" class="btn custom-btn-theme-b btn-less-width btn-sm" id="btnAddProduct">
                                <img class="img-plus-white" style="width:18px;" /> Add Product
                            </button>
                        </div>
                    </div>
                </div>

                <div class="ui-grid-b">
                    <div class="ui-block-a" style="width:30%; text-align:left">
                        <div style="padding-top:15px">
                            <label>Search product within the list</label>
                        </div>
                    </div>
                    <div class="ui-block-b" style="width:55%; text-align:left">
                        <input type="text" id="txtSearchBinProduct" />
                    </div>
                    <div class="ui-block-c" style="width:15%; text-align:left">
                        <div>
                            <img src="/Content/images/barcode96x96.png" style="height: 45px;" class="detailimage2" id="butBinProductBarcode" data-role="button" data-corners="false" />
                        </div>
                    </div>
                </div>

                @*<div class="ui-grid-a">
                    <div class="ui-block-a" style="width:40%">
                    </div>
                    <div class="ui-block-b" style="width:60%">
                        
                    </div>
                </div>*@

                <br />

                <table id="tblBinProducts" style="margin-right:5px; display:table;" class="display ui-table table-stroke table-stripe custom-tbl-responsive">
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

            <div class="popup-footer align-right">
                <div class="ui-grid-a">
                    <div class="ui-block-a">                        
                    </div>
                    <div class="ui-block-b">
                        @*<a id="btnSaveObservedCountByBin" data-role="button" style="font-size:large">Save</a>*@
                        <button id="btnSaveObservedCountByBin" data-role="none" data-title="Save" class="btn custom-btn-theme-b btnCircle popup-save">
                            <img class="img-save" alt="Save" style="width:18px;" />
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popProductBarcode" custom-data-role="popup">
        <div class="popup-md">
            <div class="popup-header">
                <span id="lblProductBinUnitMeasuresLabel"></span>
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">

                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <h4 class="btnFOWST"> </h4>
                    </div>
                    <div class="ui-block-b" style="text-align:right">
                        <img src='/Content/images/InventoryBarCodeInput.png' style='height: 45px;' class='detailimage2' data-corners='false' />
                    </div>
                </div>

                <br />

                <table id="tblProductBarcode" style="margin-right:5px; display:table;" class="display ui-table table-stroke table-stripe custom-tbl-responsive">
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

            <div class="popup-footer align-right">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                    </div>
                    <div class="ui-block-b">
                        @*<a id="btnDoneProductBinUnitMeasures" data-role="button" style="font-size:large">Done</a>*@
                        <button id="btnDoneProductBarcode" data-role="none" data-title="Save" class="btn custom-btn-theme-b btnCircle popup-save">
                            <img class="img-check-white" alt="Save" style="width:18px;" />
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popProductBinUnitMeasures" custom-data-role="popup">
        <div class="popup-md">
            <div class="popup-header">
                <span id="lblProductBinUnitMeasuresLabel"></span>
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">

                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <h4 class="btnFOWST"> </h4>
                    </div>
                    <div class="ui-block-b" style="text-align:right">
                        <img src='/Content/images/InventoryCount.png' style='height: 45px;' class='detailimage2' data-corners='false' />
                    </div>
                </div>

                <table id="tblProductBinUnitMeasures" style="margin-right:5px; display:table;" class="display ui-table table-stroke table-stripe custom-tbl-responsive">
                    <thead>
                        <tr class="custom-theme-f fixed-cell-width">
                            <th class="txt-center"> Observed Count </th>
                            <th class="txt-center"> Unit Measure </th>
                            <th class="txt-center"> Packing Size Quantity </th>                            
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>

                <br /> <br /> 

                <div>
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="padding-top:15px">
                            Max Cap
                        </div>
                        <div class="ui-block-b">
                            <input type="number" id="txtMaxCap" /> <span id="MaxCapUnitMeasure"></span>
                        </div>
                    </div>

                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="padding-top:15px">
                            Reorder Point
                        </div>
                        <div class="ui-block-b">
                            <input type="number" id="txtReorderPoint" />
                        </div>
                    </div>

                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="padding-top:15px">
                            Reorder Qty
                        </div>
                        <div class="ui-block-b">
                            <input type="number" id="txtReorderQty" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="popup-footer align-right">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                    </div>
                    <div class="ui-block-b">
                        @*<a id="btnDoneProductBinUnitMeasures" data-role="button" style="font-size:large">Done</a>*@
                        <button id="btnDoneProductBinUnitMeasures" data-role="none" data-title="Save" class="btn custom-btn-theme-b btnCircle popup-save">
                            <img class="img-check-white" alt="Save" style="width:18px;" />
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <div id="popAddBinAddress" custom-data-role="popup">
        <div class="popup-md">
            <div class="popup-header">
                <span id="lblProductBinUnitMeasuresLabel">Add Bin Address</span>
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">
                <div data-role="fieldcontain">
                    <label for="txtBinAddress">Enter Bin Address: </label>
                    <input type="text" id="txtBinAddress" />
                </div>

                <div data-role="fieldcontain">
                    <label for="txtBinRemarks">Enter Remarks: </label>
                    <input type="text" id="txtBinRemarks" />
                </div>
            </div>

            <div class="popup-footer align-right">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                    </div>
                    <div class="ui-block-b">
                        <button id="btnAddBinAddress" data-role="none" data-title="Save" class="btn custom-btn-theme-b btnCircle popup-save">
                            <img class="img-save" alt="Save" style="width:18px;" />
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    @*//////////////*@

    <div id="pageCountByArea" data-role="content">

        <div class="popup-content">
            @*<button data-role="none" class="btn custom-btn-theme-e btn-less-width btn-sm" id="btnPriceLevel">
                    <img class="img-filter" style="width:18px;" /> <span id="lblPriceLevel">Price Level: None</span>
                </button>*@
            <button data-role="none" class="btn custom-btn-theme-e btn-less-width btn-sm" id="btnArea">
                <img class="img-filter" style="width:18px;" /> <span id="lblArea">Area: None</span>
            </button>
            <button data-role="none" class="btn custom-btn-theme-e btn-less-width btn-sm" id="btnBins">
                <img class="img-filter" style="width:18px;" /> <span id="lblBinAddress">Bin Address: None</span>
            </button>
        </div>

        <br /> <br />

        <div class="ui-grid-b">
            <div class="ui-block-a" style="width:30%; text-align:left">
                <div style="padding-top:15px">
                    <label>Search product or bin within the list</label>
                </div>
            </div>
            <div class="ui-block-b" style="width:55%; text-align:left">
                <input type="text" id="txtSearchAreaProduct" />
            </div>
            <div class="ui-block-c" style="width:15%; text-align:left">
                <div>
                    <img src="/Content/images/barcode96x96.png" style="height: 45px;" class="detailimage2" id="butAreaProductBarcode" data-role="button" data-corners="false" />
                </div>
            </div>
        </div>

        <div style="overflow:auto;">
            <table id="tblShelfPrices" data-role="table" data-role="reflow" style="margin-right:5px; display:table;" class="display ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
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

    <div id="pageCountInformation" data-role="page">
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

    <div id="pageEmptyDefaultBin" data-role="page">

        <input type="text" id="txtSearchItem2" data-type="search" placeholder="Search products" />

        <div data-role="fieldcontain">
            <ul data-role="listview" data-filter="true" id="lstProducts2">
                <li data-role="list-divider" class="ui-li-count"> aaa </li>
            </ul>
        </div>


    </div>

    <div id="PageCountByUPC" data-role="page">

        <div data-role="header">
            <h1>@ViewData("Title")</h1>
            <a href="#" data-icon="arrow-l" data-rel="back" id="btnBackToMain">Back</a>
        </div>

        <div data-role="content">
            @*<div data-role="fieldcontain">
                <fieldset data-role="controlgroup">
                    <legend> Bin/Shelf Location: </legend>
                    <select id="selBinsByUPC">
                        <option> Select Bin/Shelf </option>
                    </select>
                </fieldset>
            </div>*@

            <a href="#" id="btnCountByUPC" data-role="button">Start Counting</a>
            <br />

            <table id="tblReportTable" data-role="table" data-role="reflow" style="margin-right:5px; display:table; width:100%;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
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

            <br />
            <a id="btnSaveAllContinousCount" data-role="button">Save All</a>
        </div>
    </div>

    <div id="PageCategories" data-role="page">

        <div data-role="header">
            <h1>@ViewData("Title")</h1>
            <a href="#" data-icon="arrow-l" id="btnBackToInventoryCountPage">Back</a>
        </div>

        <div data-role="content">

            <div>
                <button style="margin-right:10px;" data-role="none" class="btn btn-less-width btn-transparent btnRequired btnSelectCategory">
                    <span class="lblSelectedCategory"> Select Category </span>
                    <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span>
                </button>
            </div>
            <br />
            <div style="padding:5px;">
                <div class="ui-grid-a">
                    <div class="ui-block-a" style="width:80%;padding: 0px;margin-bottom:3px; margin-left:3px">
                        <div>
                            <input type="text" id="txtSearchProducts" placeholder="Search Product" style="height: 40px;padding:0px;" autofocus />
                        </div>
                    </div>
                    <div id="divHideBarcodeScan" class="ui-block-b" style="width: 15%;padding: 0px;margin:2px; margin-top:0px">
                        <center>
                            <div>
                                <img src="/Content/images/barcode96x96.png" style="height: 40px;" class="detailimage2" id="btnSearchProductBarcode" data-role="button" data-corners="false" />
                            </div>
                        </center>
                    </div>
                </div>
            </div>

            <br />
            <div id="dvDetails" style="width:100%; overflow:auto;">
                <table id="tblProductsInCategories" style="margin-right:5px; display:table;" class="ui-table table-stroke table-stripe">
                    <thead>
                        <tr id="tblHeader" class="custom-theme-f">
                            <th data-sort="string" style="cursor:pointer; text-align:center">Product Description</th>
                            <th data-sort="string" style="cursor:pointer; text-align:center">Unit Measure</th>
                            <th data-sort="float" style="cursor:pointer; text-align:center">Input Observed Count</th>
                            <th data-sort="float" style="cursor:pointer; text-align:center">Inventory Balance From Server</th>
                            <th data-sort="float" style="cursor:pointer; text-align:center">Last Observed Count Input </th>
                            <th data-sort="float" style="cursor:pointer; text-align:center">Variance</th>
                            @*<th data-sort="float" style="cursor:pointer; text-align:center">Add Remarks</th>*@
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
            <a id="btnSaveObservedCountByTableList" data-role="button" style="font-size:large"><h4>Save</h4></a>
            <a id="btnCategoriesBack" data-role="button" style="font-size:large"><h4>Back</h4></a>
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
                                <th style="cursor:pointer;">
                                    Input Observed Count
                                </th>
                            </tr>
                        </thead>
                        <tbody id="tblBins"></tbody>
                    </table>

                    <a id="btnBinCount" data-role="button" style="font-size:large"><h4>Done</h4></a>

                </div>
                <br />
            </div>
        </div>
    </div>

    <div id="PageCountByTableType" data-role="page">

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

    <div id="PageMyCounts" data-role="page">

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

    <div data-role="dialog" id="DialogUserAccess" class="ui-corner-all" data-dismissible="false">
        <div data-role="content">
            <h2>User Access</h2>
            Please Enter Employee Username and Password

            <br /> <br />
            <div data-role="fieldcontain">
                <label>Username: </label>
                <input type="text" id="txtUsername" />
            </div>
            <div data-role="fieldcontain">
                <label>Password: </label>
                <input type="password" id="txtPassword" />
            </div>

            <br />
            <div data-role="fieldcontain" id="dvInvalidUsernameOrPassword" style="visibility:hidden; display:none">
                <label style="color:red; font-weight:bold">* Invalid Username / Password</label>
            </div>
            <br />

            <a id="btnVerifyEmployee" data-role="button" style="font-size:large"><h4>Sign In</h4></a>
            <a data-rel="back" data-role="button" style="font-size:large"><h4>Back</h4></a>
        </div>
    </div>

    <div data-role="dialog" id="dialogVerifySignOut" class="ui-corner-all" data-dismissible="false">
        <div data-role="content">
            <h2>Switch User</h2>
            Do you want to switch user?

            <br /> <br />
            <div style="text-align: center">
                <a id="btnVerifySignOutYes" data-role="button">
                    <h3>
                        Yes
                    </h3>
                </a>
            </div>
            <div style="text-align: center">
                <a id="btnVerifySignOutNo" data-role="button">
                    <h3>
                        No
                    </h3>
                </a>
            </div>
        </div>
    </div>
