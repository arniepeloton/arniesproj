@Code
    ViewData("Title") = "Receipts and Transfers"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
  <style>

       .detailimage img {
        width: 100%;
    }
    .detailimage2 {

        text-align: center;
        margin: 0;

    }

    .detailimage2 img {
        width: 100%;
    }

    .SearchText {
        margin:0px;
        border: 0px;
        text-shadow: 0px;
        background: #FFF;
        height: 40px;
        border-radius: 0px;
        
    }
    
    .ui-btn-up-green {
        border: 1px solid #3A7214;
        background: #5BAB25;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, #7EC54E, #41AB25);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #7EC54E),
            color-stop(1, #41AB25));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#7EC54E', EndColorStr='#41AB25')";
    }
    .ui-btn-up-green a.ui-link-inherit {
        color: #fff;
    }
    .ui-btn-hover-green {
        border: 1px solid #176E00;
        background: #6EB64B;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, 
            #99D472, 
            #6EB64B);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #99D472),
            color-stop(1, #6EB64B));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#99D472', EndColorStr='#6EB64B')";
    }
    .ui-btn-hover-green a.ui-link-inherit {
        color: #fff;
    }
    .ui-btn-down-green {
        border: 1px solid #347722;
        background: #7EC54E;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, #569E39, #7EC54E);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #569E39),
            color-stop(1, #7EC54E));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#569E39', EndColorStr='#7EC54E')";
    }
    .ui-btn-down-green a.ui-link-inherit {
        color: #fff;
    }
    .ui-btn-up-green,
    .ui-btn-hover-green,
    .ui-btn-down-green {
        font-family: Helvetica, Arial, sans-serif;
        text-decoration: none;
    }

    </style>

<link rel="stylesheet" href="/Content/flatui-css-custom.css" />
<link rel="stylesheet" href="/Content/Ecommerce/css/lightbox.css">
    <script>
        require([GetPathFormat('/Scripts/sources/Receiving/index.js?crc=')]);

        function skupicerror(dis) {
            $(dis).attr('isimgerror', 1).attr('src', '../Content/images/box.png');
            Android.AddLog('Can\'t load image no. ' + $(dis).data('data').Ctr + ' for SKU: ' + $(dis).data('data').ProductName);

            if (CInt2($(dis).attr('isimgviewer')) == 1) {
                $('#lightboxOverlay, #lightbox').hide();
            }
        }
    </script>
    
End Section

<div id="pageMain" data-role="page">
    <div data-role="header" id="divHeaderReceiving" data-position="fixed" style="background: none white; border: none">        
        @*<h1 id="MainPageTitle">@ViewData("Title")</h1>
        <a href="#" data-icon="arrow-l" id="btnBack">Back</a>*@
        <div class="ui-grid-d" >
            <div class="ui-block-a">
                <div class="btnBack" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                    <div><img alt="home" src="/Content/images/home.png" width="30px" align="center" /><br /></div>
                    <div style="font-size:13px; margin-top:10px;">Home</div>
                </div>
            </div>
            <div class="ui-block-b">
                <div id="btnNewCart" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    <div><img alt="home" src="/Content/images/Update_Cart.png" width="30px" align="center" /><br /></div>
                    <div style="font-size:13px; margin-top:10px;">New Transaction</div>
                </div>
            </div>
            <div class="ui-block-c">
                <div style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                @*<span style="margin-top:5px;">Home</span>*@
            </div>
            </div>            
            <div class="ui-block-d">
                <div id="btnRecordLookup" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                    <div><img alt="home" src="/Content/images/RecordLookup.png" width="20px" align="center" /><br /></div>
                    <div style="font-size:13px; margin-top:10px;">Record Search</div>
                </div>
            </div>
            <div class="ui-block-e">
                <div id="btnInputItemDetails"style="margin:0px; height: 80px;" data-role="button" data-theme="green" data-corners="false">                    
                    <div><img alt="home" src="/Content/images/Update_Cart.png" width="30px" align="center" /><br /></div>
                    <div style="font-size:13px; margin-top:10px;">Go To Receiving Cart</div>
                </div>
            </div>
        </div>

        <div>
            <div style="margin: 0px;" data-role="button" data-theme="b" data-corners="false">
                <div class="lblPosBranch" style="float:left; text-overflow:ellipsis; overflow:hidden; width:40%; text-align:left; white-space:nowrap;">
                </div>
                <div style="float:right">
                    <span id="clockStock"></span>
                </div>
                <div style="clear:both"></div>            
            </div>
        </div>
        
    </div>

    <div data-role="popup" id="panAddOutlet" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="btnClosePopUp">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;">Add New Bin<span class="lblPopUpHeaderLibraryName"></span></h3></center>
        <div data-role="content" style="max-width:350px;" class="ui-content ui-body-d" data-theme="a">
            <div style="margin-left:10px; margin-right:10px;">

                <center><h3 style="margin-left:10px; margin-right:10px;">Enter Outlet Name : <span class="lblPopUpHeaderLibraryName"></span></h3></center>
                <input type="text" id="txtFowstBinName" />                    
                
                <br /> 

                <center><h3 style="margin-left:10px; margin-right:10px;">Enter Outlet Description : <span class="lblPopUpHeaderLibraryName"></span></h3></center>
                <input type="text" id="txtFowstBinDescription" />                    

                <br /> <br />

                <div class="ui-grid-solo">
                    <div class="ui-block-a">                        
                        <button id="btnSaveFowstBin" data-role="none" class="btn custom-btn-theme-b">
                            Save Outlet
                        </button>
                    </div>
                </div>

                <br />

                </div>      
            </div>
            <br />
        </div>

    <div data-role="content" id="divReceivingContent">
        
        <div data-role="fieldcontain" style="visibility:hidden; display:none">
            <label for="selReceiveOrTransfer"></label>
            <select id="selReceiveOrTransfer">
                <option value="ReceiveItems" id='optReceive'> Receive Items </option>
                <option value="TransferItems" id='optTransfer'> Transfer out Items </option>
            </select>
        </div>

        <div id="dvReceiving">
            @*<br />  <br /> <br />  <br /> 
            <div class="ui-grid-a" style="margin:1px; padding: 0px;">
                <div class="ui-block-a" style="width:75%;padding: 8px;margin-bottom:3px;">
                    <input type="text" id="txtSearchStockTransfer" data-type="search" placeholder="Search Stock Transfer" class="SearchText" style="height: 40px;padding:0px;" autofocus />*@

                    @*<input type="text" for="btnSearchItem" id="txtSearchItem" data-type="search" placeholder="Search products" class="SearchText" style="height: 40px;padding:0px;" autofocus />*@

                @*</div>
                <div class="ui-block-b" style="width: 20%;padding: 0px;margin:3px;">
                    <center>
                        <div>
                            <img src="/Content/images/barcode96x96.png" style="height: 50px;" class="detailimage2" id="btnScanTransferOutQrCode" data-role="button" data-corners="false" />
                        </div>
                    </center>
                </div>*@
                @*<div class="ui-block-c" style="width: 15%;padding: 0px;margin:3px;">
                    <center>
                        <div>
                            <img src="/Content/images/ContinuousScan.png" class="detailimage2" style="height: 50px;" id="btnReceiveByContinuousScanning" data-role="button" data-corners="false" />
                        </div>
                    </center>
                </div>*@
            @*</div>*@

            @*<div data-role="fieldcontain">
                <label for="txtSearchStockTransfer"></label>
                
            </div>

            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup">
                    <legend>  </legend>
                    <a id="btnScanTransferOutQrCode" data-role="button">Scan Transfer-Out QR Code</a>
                </fieldset>
            </div>

            <br>*@
        </div>

        @*<br />*@

        <div id="dvTransferOut">

            @*<br />*@

            <div data-role="fieldcontain" id="dvIsResaleable" style="visibility:hidden; display: none">

                <fieldset data-role="controlgroup">
                    <legend> The items you are transfering out are in resaleable condition? </legend>
                    <select id="selIsResaleable">
                        <option value=""> Select Choice </option>
                        <option value="ResaleableYes"> Yes </option>
                        <option value="ResaleableNo"> No </option>
                    </select>
                    <select id="selResaleableAnswer"></select>
                </fieldset>

                <div data-role="fieldcontain" id="dvResaleable">
                    <label id="lblResaleableNo" for="txtResaleableNo" style="visibility:hidden">Remarks</label>
                    <textarea id="txtResaleableNo" style="visibility:hidden"></textarea>
                </div>
            </div>

        </div>

        <div class="custom-ui-label">
            <div id="lblReceivingPlace"> Receive In: </div>
            <div><button id="btnRecInTransFromPlace">Please Select</button></div>
        </div>

        <div class="custom-ui-label">
            <div id="lblSKUSource"> Receive From: </div>
            <div><button id="btnRecFromTransToPlace">Please Select</button></div>
        </div>        
        
        <div style="display:none;">
            <br /><br /> <hr />
            <div data-role="fieldcontain">
                <label for="selReceivingPlace" id="lblReceivingPlaceXXX"> Receive In: </label>
                <select id="selReceivingPlace">
                    <option> Select Place </option>
                </select>
            </div>

            <div data-role="fieldcontain">
                <label for="selSourceOfSKU" id="lblSKUSourceXXX"> Receive From: </label>
                <select id="selSourceOfSKU">
                    <option value=""> Select Source of SKU </option>
                    <option value="valIntraOrganization" id='optIntraOrganization'> Within the Company or Organization </option>
                    <option value="valThirdPartyVendor" id='optThirdPartyVendor'> Third party Company or Organization </option>
                </select>
            </div>

            <div data-role="fieldcontain" id="dvIntraOrganization" style="visibility:hidden; display:none">
                <label for="selIntraOrganization"> &nbsp </label>
                <select id="selIntraOrganization">
                    <option> Select Place </option>
                </select>
            </div>

            <div data-role="fieldcontain" id="dvVendor" style="visibility:hidden; display:none">

                <div data-role="fieldcontain">
                    <label for="lblSourceOfSKU">Vendor:  &nbsp; <span style="color: #FF0000;">*</span> </label>
                    <input type="search" placeholder="Type or search Third Party Company" id="lblSourceOfSKU" />
                </div>

                <div data-role="fieldcontain">
                    <label for="lblThirdPartyOutlet">Third Party Outlet : </label>
                    <input type="search" placeholder="Type or search Third Party Company FOWST" id="lblThirdPartyOutlet" />
                </div>

                <div data-role="fieldcontain">
                    <label for="lblThirdPartyOutletDepartment">Third Party Outlet Department: </label>
                    <input type="search" placeholder="Type or search Third Party Company FOWST Outlet" id="lblThirdPartyOutletDepartment" />
                </div>

                <fieldset data-role="controlgroup">
                    <legend> </legend>
                    <a id="btnAddOutlet" class="ui-btn-inline ui-icon-plus" data-role="button">Add Outlet</a>
                </fieldset>
            </div>

            <div data-role="fieldcontain" id="dvFOWST">

                @*<fieldset data-role="controlgroup">
                    <legend> </legend>

                </fieldset>*@
            </div>

            <div data-role="fieldcontain" id="dvOutlet">


                @*<fieldset data-role="controlgroup">
                    <legend> Outlet: </legend>

                </fieldset>*@
            </div>
        </div>
    </div>
    <div data-role="content" id="headerInformation">

        <h4> Source Document References:</h4>
        <hr />

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" data-type="horizontal">
                <legend> Source Document Date: &nbsp; <span style="color: #FF0000;">*</span> </legend>

                <select id="optMonthValue" data-inline='true'> </select>
                <select id="optDayValue" data-inline='true'> </select>
                <select id="optYearValue" data-inline='true'> </select>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <label for="txtDocumentType">Source Document Type: &nbsp; <span style="color: #FF0000;">*</span> </label>
            <input type="text" id="txtDocumentType" />
        </div>

        <div data-role="fieldcontain">
            <label for="txtDocumentNo">Source Document No. &nbsp; <span style="color: #FF0000;">*</span> </label>
            <input type="text" id="txtDocumentNo" />
        </div>

        <div data-role="fieldcontain">
            <label for="txtPONo">PO No. </label>
            <input type="text" id="txtPONo" />
        </div>
    </div>

    <div data-role="content">
        <h4>Shipping/Delivery Details:</h4>
        <hr />

        <div data-role="fieldcontain" class="ui-radio">
            <fieldset data-role="controlgroup" data-type="horizontal" data-role="fieldcontain">
                <legend> Delivered Through 3PL?: </legend>

                <input type="radio" name="rdChoice" id="rcChoiceYes" />
                <label for="rcChoiceYes">Yes</label>

                <input type="radio" name="rdChoice" id="rcChoiceNo" checked="checked" />
                <label for="rcChoiceNo">No</label>

            </fieldset>
        </div>

        <div id='dv3pl' style="visibility:hidden; display:none">
            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup">
                    <legend> Shipper/Delivery Company: </legend>

                    <input type="text" id="txtShipper" />
                    <a id="btnShipper" data-role="button">Go</a>
                </fieldset>
            </div>

            <div data-role="fieldcontain">
                <label for="txtVessel">Vessel: </label>
                <input type="text" id="txtVessel" />
            </div>

            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <legend> Date Prepared : </legend>

                    <select id="optMonthBLValue" data-inline='true'> </select>
                    <select id="optDayBLValue" data-inline='true'> </select>
                    <select id="optYearBLValue" data-inline='true'> </select>
                </fieldset>
            </div>

            <div data-role="fieldcontain">
                <label for="txtVoyageNo">Voyage No. : </label>
                <input type="text" id="txtVoyageNo" />
            </div>

            <div data-role="fieldcontain">
                <label for="txtWayBill">B/L or WayBill No.: </label>
                <input type="text" id="txtWayBill" />
            </div>

            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <legend> Invoice Date : </legend>

                    <select id="optMonthInvoiceValue" data-inline='true'> </select>
                    <select id="optDayInvoiceValue" data-inline='true'> </select>
                    <select id="optYearInvoiceValue" data-inline='true'> </select>
                </fieldset>
            </div>

            <div data-role="fieldcontain">
                <label for="txtInvoiceNo">Invoice No.: </label>
                <input type="text" id="txtInvoiceNo" />
            </div>

            <div data-role="fieldcontain">
                <label for="txtFreightAndHandling">Freight And Handling: </label>
                <input type="text" id="txtFreightAndHandling" />
            </div>

            <div data-role="fieldcontain">
                <label for="txtArrastre">Arrastre: </label>
                <input type="text" id="txtArrastre" />
            </div>

            <div data-role="fieldcontain">
                <label for="txtTotal">Total: </label>
                <input type="text" id="txtTotal" />
            </div>

            <div data-role="fieldcontain">
                <label for="txtTotalCC">Total CC: </label>
                <input type="text" id="txtTotalCC" />
            </div>

            <div data-role="fieldcontain">
                <label for="txtTotalCBM">Total CBM: </label>
                <input type="text" id="txtTotalCBM" />
            </div>

            <div data-role="fieldcontain">
                <label for="txtTotalWeightInkg">Total Weight in Kg: </label>
                <input type="text" id="txtTotalWeightInkg" />
            </div>

            <div data-role="fieldcontain">
                <label for="txtTotalCostofBundle">Total Cost of Bundle: </label>
                <input type="text" id="txtTotalCostofBundle" />
            </div>

            <div data-role="fieldcontain">
                <label for="txtBundles">Bundles: </label>
                <input type="text" id="txtBundles" />
            </div>

        </div>

        <div>
            <h4>Other Information :</h4>
            <hr />

            <div data-role="fieldcontain">
                <label for="txtTransportedBy">Transported By: &nbsp; <span style="color: #FF0000;">*</span> </label>
                <input type="search" placeholder="Type or search name" id="txtTransportedBy" />
            </div>


        </div>

        <div data-role="popup" id="popupMoreOptions" data-theme="b" class="ui-corner-all" style="width:auto;">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Record Search</h3></center>
            <div data-role="content">

                <div style="text-align: center">
                    <a id="btnReceivingPopup" data-role="button" data-theme="e">
                        <h3 id="lblRecordSearchType">Receiving</h3>
                        <p style="font-weight: normal; margin-top: -15px;">
                            <label id="lnkPosBranch" style="cursor:pointer;"></label>
                            <br />
                            <label id="lblRecordSearchDescription"> View Finalized Receiving Transactions or Continue working on Draft Receiving Transactions. </label>                            
                        </p>
                    </a>
                </div>
                <div style="text-align: center">
                    <a id="btnPOPopup" data-role="button" data-theme="e">
                        <h3>Finalized PO</h3>
                        <p style="font-weight: normal; margin-top: -15px;">
                            <br />
                            Receive Finalized POs.
                        </p>
                    </a>
                </div>
                <div style="text-align: center">
                    <a id="butTransferOutSearchType" data-role="button" data-theme="e">
                        <h3>Transfer Out</h3>
                        <p style="font-weight: normal; margin-top: -15px;">
                            <br />
                            Receive items from Suppliers or FOWST.
                        </p>
                    </a>
                </div>
            </div>

        </div>
    </div>

    <div data-role="popup" id="panAddOutlet" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="btnClosePopUp">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;">Add New Bin<span class="lblPopUpHeaderLibraryName"></span></h3></center>
        <div data-role="content" style="max-width:350px;" class="ui-content ui-body-d" data-theme="a">
            <div style="margin-left:10px; margin-right:10px;">

                <center><h3 style="margin-left:10px; margin-right:10px;">Enter Outlet Name : <span class="lblPopUpHeaderLibraryName"></span></h3></center>
                <input type="text" id="txtFowstBinName" />                    
                
                <br /> 

                <center><h3 style="margin-left:10px; margin-right:10px;">Enter Outlet Description : <span class="lblPopUpHeaderLibraryName"></span></h3></center>
                <input type="text" id="txtFowstBinDescription" />                    

                <br /> <br />

                <div class="ui-grid-solo">
                    <div class="ui-block-a">                        
                        <button id="btnSaveFowstBin" data-role="none" class="btn custom-btn-theme-b">
                            Save Outlet
                        </button>
                    </div>
                </div>

                <br />

                </div>      
            </div>
            <br />
        </div>
    
    <div data-role="popup" id="popReceivingTransferOption" data-dismissible="false" data-overlay-theme="a" data-theme="a" data-corners="false">
        @*<a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>*@
        <center><h3 style="margin-left:10px; margin-right:10px;" id="txtTitlePopup">&nbsp;</h3></center>
        <div data-role="content" class="ui-content ui-body-d jPopupContent" data-theme="a">
            <div subpage="pageError" id="msgOrganization" style="display:none; color:#FF0000">Please select organization</div>
            <div subpage="pageError" id="msgOutlet" style="display:none; color:#FF0000">Please select outlet</div>
            <div class="custom-ui-label">
                <div>Organization:</div>
                <div>
                    <select id="selOrganization"></select>
                </div>
            </div>
            <div class="custom-ui-label">
                <div>Outlet:</div>
                <div>
                    <select id="selReceivingPlaceOption">
                        <option> Please Select </option>
                    </select>
                </div>
            </div>
            <div class="custom-ui-label">
                <div>Department:</div>
                <div>
                    <select id="selDepartment">
                        <option> Please Select </option>
                    </select>
                </div>
            </div>

            <br />

            <button id="btnReceivingTransferOptionOk">Ok</button>
            @*<button id="btnReceivingTransferOptionCancel" data-rel="back">Cancel</button>*@
            <a href="#" data-rel="back" data-role="button" id="btnReceivingTransferOptionCancel">Cancel</a>
        </div>
    </div>
 </div>

<div id="pageCart" data-role="page">
    @*<div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1 id="dvpageCartHeader">Receiving Cart</h1>
    </div>*@

    <div data-role="header" id="divHeaderCart" @*data-position="fixed"*@ style="background: none white; border: none">        
        @*<h1 id="MainPageTitle">@ViewData("Title")</h1>
        <a href="#" data-icon="arrow-l" id="btnBack">Back</a>*@
        <div class="ui-grid-d">
            <div class="ui-block-a">
                <div class="btnBack" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    <span style="margin-top:5px;">Home</span>
                </div>
            </div>
            <div class="ui-block-b">
                <div style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    @*<span style="margin-top:5px;">Home</span>*@
                </div>
            </div>
            <div class="ui-block-c">
                <div style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    @*<span style="margin-top:5px;">Home</span>*@
                </div>
            </div>
            <div class="ui-block-d">
                <div style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    @*<span style="margin-top:5px;">Home</span>*@
                </div>
            </div>
            <div class="ui-block-e">
                <div id="btnDone" style="margin:0px; height: 80px; word-wrap: break-word;" data-role="button" data-theme="green" data-corners="false">                    
                    <span style="margin-top:5px;">Save Receiving Cart</span>
                </div>
            </div>
        </div>
        <div>
            <div style="margin: 0px;" data-role="button" data-theme="b" data-corners="false">
                <div class="lblPosBranch" style="float:left; text-overflow:ellipsis; overflow:hidden; width:40%; text-align:left; white-space:nowrap;">
                </div>
                <div style="float:right">
                    <span id="clockReceivingCart"></span>
                </div>
                <div style="clear:both"></div>            
            </div>
        </div>
        
    </div>

    <div data-role="content" id="divCartContent">
        <table style="width:100%;">
            <tr>
                <td>
                    <input type="text" for="btnSearchItem" id="txtSearchItem" data-type="search" placeholder="Search products" class="SearchText" style="height: 40px;padding:0px;" autofocus />
                </td>
                <td style="width:70px;">
                    <center>
                        <img src="/Content/images/barcode96x96.png" style="height: 50px;" class="detailimage2" id="btnScanBarCode" data-role="button" data-corners="false" />
                    </center>                    
                </td>
                <td style="width:70px;">
                    <center>
                        <img src="/Content/images/ContinuousScan.png" class="detailimage2" style="height: 50px;" id="btnReceiveByContinuousScanning" data-role="button" data-corners="false" />
                    </center>
                </td>
            </tr>
        </table>

        @*<div data-role="fieldcontain">
                <a id="btnScanBarCode" data-role="button">Press to Scan Bar Code</a>
            </div>*@

        @*<br />

            <div data-role="fieldcontain">
                <a id="btnReceiveByContinuousScanning" data-role="button">Receive Items using Continuous Scanning of UPC/Barcode</a>
            </div>*@

        @*<div data-role="fieldcontain">
                <a id="btnScanBarCode" data-role="button">Press to Scan Bar Code</a>
            </div>*@

        <div>
            <h3>Receiving Cart</h3>
            <ul data-role="listview" id="lstItems" data-inset="false" data-split-icon="delete">
            </ul>
        </div>

        <div>
            @*<div> <a href="#" id="btnDone" data-role="button">Save Receiving Cart</a> </div>*@
            <br />
            <a href="#" id="btnBackRecCart" data-role="button"><h3>Back</h3></a>
        </div>

    </div>
</div>

<div id="pageReadOnly" data-role="page">
    <div data-role="header" data-position="fixed" style="background: none white; border: none">        
        @*<h1 id="MainPageTitle">@ViewData("Title")</h1>
        <a href="#" data-icon="arrow-l" id="btnBack">Back</a>*@
        <div class="ui-grid-d" >
            <div class="ui-block-a">
                <div class="btnBack" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                    <span style="margin-top:5px;">Home</span>
                </div>
            </div>
            <div class="ui-block-b">
                <div style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                @*<span style="margin-top:5px;">Home</span>*@
            </div>
            </div>
            <div class="ui-block-c">
                <div style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                @*<span style="margin-top:5px;">Home</span>*@
            </div>
            </div>            
            <div class="ui-block-d">
                <div id="btnRORecordSearch" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                    <span style="margin-top:5px;">Record Search</span>
                </div>
            </div>
            <div class="ui-block-e">
                <div id="btnNewReceivingCart" style="margin:0px; height: 80px;" data-role="button" data-theme="green" data-corners="false">                    
                    <span style="margin-top:5px;">New Transaction</span>
                </div>
            </div>
        </div>

        <div>
            <div style="margin: 0px;" data-role="button" data-theme="b" data-corners="false">
                <div class="lblPosBranch" style="float:left; text-overflow:ellipsis; overflow:hidden; width:40%; text-align:left; white-space:nowrap;">
                </div>
                <div style="float:right">
                    <span id="clockReadOnly"></span>
                </div>
                <div style="clear:both"></div>            
            </div>
        </div>        
    </div>

    <div data-role="content">
        <div class="custom-ui-label">
            <div>Receive In:</div>
            <div id="txtROReceiveIn"></div>
            <div style="clear:both;"></div>
        </div>

        <div class="custom-ui-label">
            <div>Receive From:</div>
            <div id="txtROReceiveFrom"></div>
            <div style="clear:both;"></div>
        </div>

        <h3>Source Document References:</h3>

        <div class="custom-ui-label">
            <div>Source Document Date:</div>
            <div id="txtRODocDate"></div>
            <div style="clear:both;"></div>
        </div>

        <div class="custom-ui-label">
            <div>Source Document Type:</div>
            <div id="txtRODocType"></div>
            <div style="clear:both;"></div>
        </div>

        <div class="custom-ui-label">
            <div>Source Document No.:</div>
            <div id="txtRODocNo"></div>
            <div style="clear:both;"></div>
        </div>

        <div class="custom-ui-label">
            <div>PO No.:</div>
            <div id="txtROPONo"></div>
            <div style="clear:both;"></div>
        </div>

        <h3>Other Information:</h3>

        <div class="custom-ui-label">
            <div>Transported By:</div>
            <div id="txtROTransportedBy"></div>
            <div style="clear:both;"></div>
        </div>

        <h3>Receiving Cart:</h3>
        <br />
        <ul id="lstROReceivingCart" data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm ui-listview">
            @*<li>
                <a>
                    FRIED CHICKEN
                    <br /><br />
                    <div class="custom-ui-label" style="font-weight:normal !important;">
                        <div>Receive Good:</div>
                        <div>124</div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="custom-ui-label" style="font-weight:normal !important;">
                        <div>Receive Bad:</div>
                        <div>124</div>
                        <div style="clear:both;"></div>
                    </div>
                </a>
            </li>*@
        </ul>
    </div>
</div>

<div id="pageUnitMeasure" data-role="page">
    <div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>Unit Measure</h1>
    </div>

    <div data-role="content">                
    </div>

</div>

<div id="pageCounter" data-role="page">
    <div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>Counter</h1>
    </div>

    <div data-role="content">
        

        <div data-role="fieldcontain">	        
	        <div> <a id="btnPlusOne" data-role="button"><h1>+1 Add </h1></a> </div>
        </div>
                
        <div data-role="fieldcontain">	        
            <fieldset data-role="controlgroup">
                <legend> Count </legend>
                <input type="number" id="txtCount" value="0" step="1"/>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" data-type="horizontal">
                <legend> </legend>                
                <a id="btnMinusOne" data-role="button">-1 Subtract </a> 	            
                <a id="btnReset" data-role="button" data-inline="true">Reset</a> 	            
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" data-type="horizontal">
                <legend> </legend>                
                <label id='lblCountError' style="color:Red; visibility:hidden"> Count Quantity should greater than or equal to 0 </label>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <div> <a id="btnBackCounter" data-role="button">Done</a> </div>
        </div>
    </div>

</div>

<div id="pageStockViewer" data-role="page">

    @*<style>
        /*Green button*/
        .ui-btn-up-green {
            border: 1px solid #3A7214;
            background: #5BAB25;
            font-weight: bold;
            color: #fff;
            text-shadow: 0 -1px 1px #000;
            background-image: -moz-linear-gradient(top, #7EC54E, #41AB25);
            background-image: -webkit-gradient(linear,left top,left bottom, color-stop(0, #7EC54E), color-stop(1, #41AB25));
            -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#7EC54E', EndColorStr='#41AB25')";
        }

            .ui-btn-up-green a.ui-link-inherit {
                color: #fff;
            }

        .ui-btn-hover-green {
            border: 1px solid #176E00;
            background: #6EB64B;
            font-weight: bold;
            color: #fff;
            text-shadow: 0 -1px 1px #000;
            background-image: -moz-linear-gradient(top, #99D472, #6EB64B);
            background-image: -webkit-gradient(linear,left top,left bottom, color-stop(0, #99D472), color-stop(1, #6EB64B));
            -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#99D472', EndColorStr='#6EB64B')";
        }

            .ui-btn-hover-green a.ui-link-inherit {
                color: #fff;
            }

        .ui-btn-down-green {
            border: 1px solid #347722;
            background: #7EC54E;
            font-weight: bold;
            color: #fff;
            text-shadow: 0 -1px 1px #000;
            background-image: -moz-linear-gradient(top, #569E39, #7EC54E);
            background-image: -webkit-gradient(linear,left top,left bottom, color-stop(0, #569E39), color-stop(1, #7EC54E));
            -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#569E39', EndColorStr='#7EC54E')";
        }

            .ui-btn-down-green a.ui-link-inherit {
                color: #fff;
            }

        .ui-btn-up-green, .ui-btn-hover-green, .ui-btn-down-green {
            font-family: Helvetica, Arial, sans-serif;
            text-decoration: none;
        }

        /*Gray button*/
        .ui-btn-up-gray {
            border: 1px solid #595A57;
            background: #8F908F;
            font-weight: bold;
            color: #fff;
            text-shadow: 0 -1px 1px #000;
            background-image: -moz-linear-gradient(top, #AAABAA, #8D8E8D);
            background-image: -webkit-gradient(linear,left top,left bottom, color-stop(0, #AAABAA), color-stop(1, #8D8E8D));
            -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#AAABAA', EndColorStr='#8D8E8D')";
        }

            .ui-btn-up-gray a.ui-link-inherit {
                color: #fff;
            }

        .ui-btn-hover-gray {
            border: 1px solid #646464;
            background: #A1A0A0;
            font-weight: bold color: #fff;
            text-shadow: 0 -1px 1px #000;
            background-image: -moz-linear-gradient(top, #B5B8B5, #A1A0A0);
            background-image: -webkit-gradient(linear,left top,left bottom, color-stop(0, #B5B8B5), color-stop(1, #A1A0A0));
            -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#B5B8B5', EndColorStr='#A1A0A0')";
        }

            .ui-btn-hover-gray a.ui-link-inherit {
                color: #fff;
            }

        .ui-btn-down-gray {
            border: 1px solid #777A77;
            background: #AAABAA;
            font-weight: bold;
            color: #fff;
            text-shadow: 0 -1px 1px #000;
            background-image: -moz-linear-gradient(top, #999B99, #AAABAA);
            background-image: -webkit-gradient(linear,left top,left bottom, color-stop(0, #999B99), color-stop(1, #AAABAA));
            -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#999B99', EndColorStr='#AAABAA')";
        }

            .ui-btn-down-gray a.ui-link-inherit {
                color: #fff;
            }

        .ui-btn-up-gray, .ui-btn-hover-gray, .ui-btn-down-gry {
            font-family: Helvetica, Arial, sans-serif;
            text-decoration: none;
        }

        /* Button */
        .UpdateCartButton .ui-btn-inner {
            width: auto;
            height: 65px;
        }

        .UpdateCartPlusminusButtonText .ui-btn-text {
            font-size: 50px;
            color: White;
        }
    </style>
    <script>
        require([GetPathFormat('/Scripts3/Stock.js?crc=')]);
    </script>
    <div data-role="header" data-position="fixed1" style="background: none white; border: none">
        <div class="ui-grid-d">
            <div class="ui-block-a" style="width: 20%;">
                <div id="btnStockInfo" style="margin: 0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                    <span>Stock Info</span>
                </div>
            </div>
            <div class="ui-block-b" style="width: 20%;">
                <div id="btnSalesPerson" style="margin: 0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    <span>
                        Indicate
                        <br />
                        Sales Representatives
                    </span>
                </div>
            </div>
            <div class="ui-block-c" style="width: 20%;">
                <div id="btnSerialNumber" style="margin: 0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    <span>
                        Enter
                        <br />
                        Serial Number(s)
                    </span>
                </div>
            </div>
            <div class="ui-block-d" style="width: 20%;">
                <div id="btnInventoryBalances" style="margin: 0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    <span>
                        See
                        <br />
                        Inventory Balances
                    </span>
                </div>
            </div>
            <div class="ui-block-e" style="width: 20%;">
                <div name="butUnitUpdateCart" style="margin: 0px; height: 80px;" data-role="button" data-theme="green" data-corners="false" class="HotButtons">
                    <span>Update Cart</span>
                </div>
            </div>
        </div>
        <div>
            <div style="margin: 0px;" data-role="button" data-theme="b" data-corners="false">
                <span id="clockStock"></span>
            </div>
        </div>
    </div>
    <div class="ui-bar-c" style="border: none">
        <div class="ui-grid-a">
            <div class="ui-block-a" style="width: 43%; text-align: center">
                <img src="../Content/images/box.png" />
            </div>
            <div class="ui-block-b" style="width: 57%">
                <h3>
                    SKU #: <span id="spanStockNumber"></span>
                    <br />
                    <span id="spanStockDescription"></span><a href="#">(MORE INFO)</a>
                </h3>
            </div>
        </div>
        <div id="divUnit" name1="divUnit" data-theme="c" style="padding: 10px">
            <div id="divStockMinimumCapacity" class="ui-grid-a">
                <div class="ui-block-a">
                    Minimum Capacity:
                </div>
                <div class="ui-block-b">
                    <span id="spanStockMinimumCapacity">1,234,567</span>
                </div>
            </div>
            <div id="divStockMaximumCapacity" class="ui-grid-a">
                <div class="ui-block-a">
                    Maximum Capacity:
                </div>
                <div class="ui-block-b">
                    <span id="spanStockMaximumCapcity">1,234,567</span>
                </div>
            </div>
            <div class="ui-grid-a">
                <div class="ui-block-a" style="width: 67%; margin-top: 10px;">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="margin-top: 30px;">
                            <span name="spanPriceLevel"></span>
                        </div>
                        <div class="ui-block-b">
                            <div name="divPriceLevel">
                            </div>
                        </div>
                    </div>
                    <div name="divRequestedUnitPrice" class="ui-grid-a" style="display: none">
                        <div class="ui-block-a">
                            <span>Requested Unit Price</span>
                        </div>
                        <div class="ui-block-b">
                            Php <span name="spanRequestedUnitPrice"></span>
                        </div>
                    </div>
                    <div class="ui-grid-a">
                        <div class="ui-block-a">
                            <span name="spanUnit"></span>
                        </div>
                        <div class="ui-block-b">
                            Php <span name="spanPrice"></span>
                        </div>
                    </div>
                    <div class="ui-grid-a">
                        <div class="ui-block-a">
                            <span name="spanUnitQtyLabel"></span>
                        </div>
                        <div class="ui-block-b">
                            <span name="spanUnitQty"></span>
                        </div>
                    </div>
                    <div class="ui-grid-a">
                        <div class="ui-block-a">
                            Price per <span name="spanBaseUnit"></span>
                        </div>
                        <div class="ui-block-b">
                            <span name="spanPricePerBase"></span>
                        </div>
                    </div>
                    <div class="ui-grid-a">
                        <div class="ui-block-a">
                            Savings per <span name="spanBaseUnit"></span>
                        </div>
                        <div class="ui-block-b">
                            <span name="spanSavingsPerBase"></span>
                        </div>
                    </div>
                    <div name="divTotalSavings" class="ui-grid-a">
                        <div class="ui-block-a">
                            Savings per <span name="spanUnit"></span>
                        </div>
                        <div class="ui-block-b">
                            <span name="spanTotalSavings"></span>
                        </div>
                    </div>
                    <div name="divSrp" class="ui-grid-a" style="display: none">
                        <div class="ui-block-a">
                            SRP:
                        </div>
                        <div class="ui-block-b">
                            <span name="spanSrp"></span>
                        </div>
                    </div>
                </div>
                <div class="ui-block-b" style="width: 33%;">
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="width: 50%;">
                            <input name="txtQtyTemplate" type="number" placeholder="QTY" style="height: 73px;" maxlength="5" />
                        </div>
                        <div class="ui-block-b" style="width: 50%">
                            <div name="butUnitUpdateCart" data-role="button" data-theme="green" data-corners="false" class="UpdateCartButton">
                                <span style="font-size: small;">Update Cart</span>
                            </div>
                        </div>
                    </div>
                    <div class="ui-grid-a" style="margin-top: -10px;">
                        <div class="ui-block-a">
                            <div name="butUnitMinus" data-role="button" data-theme="gray" data-corners="false" class="UpdateCartButton UpdateCartPlusminusButtonText">
                                <span style="font-size: xx-large;">-</span>
                            </div>
                        </div>
                        <div class="ui-block-b">
                            <div name="butUnitPlus" data-role="button" data-theme="gray" data-corners="false" class="UpdateCartButton UpdateCartPlusminusButtonText">
                                <span style="font-size: xx-large;">+</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div data-role="collapsible_clone">
                <h6>
                    More
                </h6>
                <div data-role="collapsible_clone" name="divPriceAdjustments">
                    <h6>
                        Request a Price Adjustment
                    </h6>
                    <div class="ui-grid-a">
                        <div class="ui-block-a">
                            Original Price
                        </div>
                        <div class="ui-block-b">
                            <span name="spanOriginalPrice"></span>
                        </div>
                    </div>
                    <div class="ui-grid-a">
                        <div class="ui-block-a">
                            Last Price Given to This Customer
                        </div>
                        <div class="ui-block-b">
                            <span name="spanLastPrice">none</span>
                        </div>
                    </div>
                    <div class="ui-grid-a">
                        <div class="ui-block-a">
                            Date
                        </div>
                        <div class="ui-block-b">
                            <span name="spanLastPriceDate">none</span>
                        </div>
                    </div>
                    <div class="ui-grid-a">
                        <div class="ui-block-a">
                            Adjust Price To:
                        </div>
                        <div class="ui-block-b">
                            <input name="txtNewPriceTemplate" type="number" />
                        </div>
                    </div>
                    <button name="butValidateAdjustedPrice">Validate Adjusted Price</button> <span name="spanPriceResult"></span>
                </div>
            </div>
        </div>
        <div id="divSubTotal" style="text-align: center; padding-bottom: 10px">
            <h3>
                Sub Total : PhP <span id="spanSubtotal"></span>
            </h3>
        </div>
        <div id="divProductSalesPersonContainer" style="display: none; margin: 0px 7px 0 7px;">
            <div>
                <input id="txtProductSalesRepSearch" type="search" placeholder="Search a Sales Representative" value="" />
            </div>
            <div id="divProductSalesPerson">
                <ul data-role="listview" id="lstSalesRepContainer" data-inset="true">
                    <li data-role="list-divider">Sales Representative List</li>
                    <li>
                        <div>
                            <ul data-role="listview" id="lstSalesRep" data-inset="true"></ul>
                        </div>
                    </li>
                </ul>
            </div>
            <br />
        </div>
        <div id="divProductSerialContainer" style="display: none; margin: 0px 7px 0 7px;">
            <div>
                <input id="txtProductSerial" type="search" placeholder="Enter Serial Number" value="" />
                <button id="butSerialBarCode">Scan Barcode</button>
            </div>
            <div id="divProductSerial">
                <ul data-role="listview" id="lstProductSerialContainer" data-inset="true">
                    <li data-role="list-divider">Product Serial Number(s)</li>
                    <li>
                        <div data-role="fieldcontain" id="divProductSerialRecordCount">
                        </div>
                        <div>
                            <ul data-role="listview" id="lstProductSerial" data-inset="true"></ul>
                        </div>
                    </li>
                </ul>
            </div>
            <br />
        </div>
        <div id="divProductInventoryBalances" style="display: none; margin: 0px 7px 0 7px;">
            <div id="divProductInventory">
                <ul data-role="listview" id="lstProductInventoryContainer" data-inset="true">
                    <li data-role="list-divider">Inventory Balances</li>
                    <li>
                        <div data-role="fieldcontain" id="divProductInventoryRecordCount">
                        </div>
                        <div>
                            <ul data-role="listview" id="lstProductInventoryBalances" data-inset="true">
                                <table id="tblInventoryBalance" style="width: 75%"></table>
                            </ul>
                        </div>
                    </li>
                </ul>
            </div>
            <br />
        </div>
    </div>
    <div id="divReportNoPrice">
        <a id="butReportNoPrice" data-role="button">
            <h3>
                Report No Price
            </h3>
            <p style="font-weight: normal; margin-top: -15px;">
                Report this product to your manager
            </p>
        </a>
    </div>
    <div id="divRequestPrice" data-role="popup" data-dismissible="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <div data-role="content">
            Requested price:
            <div id="divRequestPriceMessage">
            </div>
            <form id="formRequestedPrice">
                <div data-role="fieldcontain">
                    <label for="txtRequestedPrice">
                        Discounted amount:
                    </label>
                    <input id="txtRequestedPrice" type="number" />
                </div>
                <div data-role="fieldcontain">
                    <label for="txtRequestedPriceDiscountPercent">
                        Discount percent:
                    </label>
                    <input id="txtRequestedPriceDiscountPercent" type="number" />
                </div>
                <div data-role="fieldcontain">
                    <label for="cbRequestedPriceBasePrice">
                        Base price:
                    </label>
                    <select id="cbRequestedPriceBasePrice"></select>
                </div>
                <input type="submit" value="Ok" />
            </form>
        </div>
    </div>*@


    <div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>Receiving Cart</h1>
    </div>

    <div data-role="content">        
        <div class="ui-bar-c" style="border: none">
            <br />
             <div class="ui-grid-a">
                <div class="ui-block-a" style="width: 43%; text-align: center">
                    <img src="../Content/images/box.png" id="imgReceivingStockInfo" style="width: 100px;" />
                </div>
                <div class="ui-block-b" style="width: 57%">
                    <span style="font-size:20px;">
                        SKU #: <span id="spanStockNumber"></span>
                        <br />
                        <span id="spanStockDescription"></span><a href="#">(MORE INFO)</a>
                    </span>
                </div>
            </div>

            <div data-theme="c" style="padding: 10px">            
                <div style="text-align:center" class="ui-bar ui-bar-b"> <h3 id="dvGoodCount"> Good Count </h3> </div>
                <table id="lstUnitMeasure" style="width:100%"></table>
            </div>

            <div data-theme="c" style="padding: 10px; visibility:hidden; display:none" id="dvStockViewerUnitMeasure">
                <div style="text-align:center" class="ui-bar ui-bar-b"> <h3> Bad Count </h3> </div>
                <table id="lstBadOrder" style="width:100%"></table>

                 <div data-role="fieldcontain" id="dvBadOrderReason">
                    <label for="selBadOrder"> <h3> Bad Order Reason: </h3> </label>
                    <select id="selBadOrder">
                        <option value="Please choose a reason for return"> Please choose a reason for return </option>
                        <option value="Expireditem"> Expired item </option>
                        <option value="About-to-expireitem"> About-to-expire item </option>
                        <option value="Factory-defectivepackaging"> Factory-defective packaging </option>
                        <option value="Factory-defectiveproduct "> Factory-defective product </option>
                        <option value="Non-Factory-DamagedGoods"> Non-Factory-Damaged Goods </option>
                        <option value="Non-Factory-DamagedPackaging"> Non-Factory-Damaged Packaging </option>
                        <option value="Obsoleteitem"> Obsolete item </option>
                        <option value="Productrecall"> Product recall </option>
                    </select>
                </div>
            </div>

        </div>

        <br />
        @*July 21 2020-Adding Serial Number*@
        <div class="ui-bar-c" style="border: none">
            <br />
            <div id="divProductSerialContainer">
                <div>
                    <input id="txtProductSerials" type="text" placeholder="Enter Serial Number" value="" class="SearchText" data-type="search" />
                    <button id="butSerialBarCode">Scan Barcode</button>
                </div>
                <div id="divProductSerial">
                    <ul data-role="listview" id="lstProductSerialContainer" data-inset="true">
                        <li data-role="list-divider">Product Serial Number(s)</li>
                        <li>
                            <div data-role="fieldcontain" id="divProductSerialRecordCount">
                            </div>
                            <div>
                                <ul data-role="listview" id="lstProductSerial" data-inset="true"></ul>
                            </div>
                        </li>
                    </ul>
                </div>
                <br />
            </div>
        </div>

        <br />

        <div id="divUpdateCart">
            <button id="btnDoneUnitMeasure">
                <h3> Update Cart </h3>
            </button>
        </div>

        <div id="divUpdateBarcode">
            <button id="btnUpdateBarcode">
                <h3> Add Barcode </h3>
            </button>
        </div>

        <button id="btnBackCount">
            <h3> Back </h3>
        </button>
    </div>

    <div data-role="popup" id="panSetBarcode" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="btnClosePopUp">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;">Enter Barcode<span class="lblPopUpHeaderLibraryName"></span></h3></center>
        <div data-role="content" style="max-width:350px;" data-theme="a">
            <div style="margin-left:10px; margin-right:10px;">
                <input type="text" id="txtBarcode" />

                <br />
                <br />
                <div class="ui-grid-solo">
                    <div class="ui-block-a">
                        <a id="btnSaveBarcodeOnline" data-role="button" class="btn custom-btn-theme-b">
                            SaveBarcode
                        </a>
                    </div>
                </div>
            </div>
            <br />
        </div>
    </div>
</div>

<div id="pageStockViewerDetail" data-role="page">
    <div data-role="header">  
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>      
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">
       <div class="ui-bar-c" style="border: none">
             @*<div class="ui-grid-a">
                <div class="ui-block-a" style="width: 43%; text-align: center">
                    <img src="Content/images/box.png" />
                </div>
                <div class="ui-block-b" style="width: 57%">
                    <h3>
                        SKU #: <span id="spanStockNumberDetail"></span>
                        <br />
                        <span id="spanStockDescriptionDetail"></span><a href="#">(MORE INFO)</a>
                    </h3>
                </div>
            </div>*@

            <div data-role="fieldcontain">
                <form>
                    <ul data-role="listview" id="lstSizesUnitMeasureFrame" data-inset="true" data-icon="delete">
                        <li data-role="list-divider">
                            <div data-role="fieldcontain" style="height:5px;">
                                Receiving or Tranfer-Out by Sizes                            
                            </div>
                        </li>
                        <li>
                            <table id="tblSizesUnitMeasureFrame" data-role="table" data-mode="reflow" class="ui-responsive" style="width:100%">
                                <thead>
                                    <tr>
                                        <th> Product Name </th>
                                        <th> Size </th>
                                        <th> Unit Measure </th>
                                        <th> Retail Price </th>
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
        </div>
        <button id="btnSaveSizesTallies-Detail">
                <h3> Update Cart </h3>
            </button>
         @*<a href="#" id="" data-role="button">Update Cart</a> *@
    </div>
</div>

<div id="pagePrintPage" data-role="page">
    <div data-role="header">  
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>      
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">
        <a href="#" id="" data-role="button">Print</a>          
    </div>
</div>