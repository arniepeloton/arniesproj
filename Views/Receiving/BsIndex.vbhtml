@Code
    ViewData("Title") = "Receipts and Transfers"
    
    If Request.IsAjaxRequest() Then
        Layout = ""
    Else
        Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
    End If
End Code

@Section head
<style>
    .menu-description {
        font-size: 13px;
        overflow: hidden;
        text-overflow: ellipsis;
        display: -webkit-box;
        -webkit-line-clamp: 2;
        -webkit-box-orient: vertical;
        height: 39px;
        cursor:pointer;
    }

    .required {
        color: #FF0000;
    }
</style>

@*<link rel="stylesheet" href="/Content/flatui-css-custom.css" />*@
<link rel="stylesheet" href="/Content/Ecommerce/css/lightbox.css">
    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
                require([GetPathFormat('/Scripts/sources/Receiving/Bs_index.js?crc=' + assetversion)]);
            })
        })        

        function skupicerror(dis) {
            $(dis).attr('isimgerror', 1).attr('src', '../Content/images/box.png');
            Android.AddLog('Can\'t load image no. ' + $(dis).data('data').Ctr + ' for SKU: ' + $(dis).data('data').ProductName);

            if (CInt2($(dis).attr('isimgviewer')) == 1) {
                $('#lightboxOverlay, #lightbox').hide();
            }
        }
    </script>
    
End Section

<div id="pageMain" custom-data-role="page" style="display:block;">
    <nav class="navbar navbar-dark bg-primary mb-3 sticky-top" style="padding:0px !important;">
        <div class="row" style="margin:0px !important;">
            <div class="col-3 col-md-2" style="padding:0px !important;">
                <button data-role="none" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0 btnBack">
                    <span id="iconHome" class="bi-house-fill"></span>
                    <div id="lblHome" class="menu-description mt-1">Home</div>
                </button>
            </div>
            <div class="col-3 col-md-2" style="padding:0px !important;">
                <button data-role="none" id="btnNewCart" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                    <span class="bi-cart4"></span>
                    <div class="menu-description mt-1">New Transaction</div>
                </button>
            </div>
            <div class="col-3 col-md-2" style="padding:0px !important;">
                <button data-role="none" id="btnRecordLookup" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                    <span class="bi-search"></span>
                    <div class="menu-description mt-1">Record Search</div>
                </button>
            </div>
            <div class="col-3 offset-md-3 col-md-3" style="padding:0px !important;">
                <button data-role="none" id="btnInputItemDetails" style="height:100%;" class="btn btn-success btn-block btn-lg rounded-0">
                    <span class="bi-cart4"></span>
                    <div class="menu-description mt-1">Go To Receiving Cart</div>
                </button>
            </div>
        </div>
        <br />
        <div class="row" style="margin:0px; padding:0.5em; width:100%; color:#FFF; background-color:#1969de; font-weight:bold;">
            <div class="offset-5 col-7" style="text-overflow:ellipsis; overflow:hidden; text-align:right; white-space:nowrap;">
                <span id="clockStock">&nbsp;</span>
            </div>
        </div>
    </nav>

    <div id="panAddOutlet" class="modal fade">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Add New Bin <span class="lblPopUpHeaderLibraryName"></span></h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label for="txtFowstBinName" class="form-label">Enter Outlet Name: <span class="lblPopUpHeaderLibraryName"></span></label>
                        <input data-role="none" type="text" class="form-control" id="txtFowstBinName" />
                    </div>

                    <div class="mb-3">
                        <label for="txtFowstBinDescription" class="form-label">Enter Outlet Description: <span class="lblPopUpHeaderLibraryName"></span></label>
                        <input data-role="none" type="text" class="form-control" id="txtFowstBinDescription" />
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-lg btn-block">Cancel</button>
                        </div>
                        <div class="col">
                            <button data-role="none" id="btnSaveFowstBin" class="btn btn-success btn-lg btn-block">Save Outlet</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="container-fluid" id="divReceivingContent">

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
            <div data-role="fieldcontain" id="dvIsResaleable" style="visibility:hidden; display: none">
                <div class="row mb-3">
                    <label class="col-md-2 col-form-label" for="selIsResaleable">The items you are transfering out are in resaleable condition?</label>
                    <div class="col-md-10">
                        <select id="selIsResaleable" data-role="none" class="form-select mb-3">
                            <option value=""> Select Choice </option>
                            <option value="ResaleableYes"> Yes </option>
                            <option value="ResaleableNo"> No </option>
                        </select>
                        <select id="selResaleableAnswer" data-role="none" class="form-select"></select>
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-md-2 col-form-label" for="lblResaleableNo" style="visibility:hidden">Remarks:</label>
                    <div class="col-md-10">
                        <textarea id="txtResaleableNo" data-role="none" class="form-control" style="visibility:hidden"></textarea>
                    </div>
                </div>
            </div>
        </div>

        <div class="row mb-3">
            <label id="lblReceivingPlace" class="col-md-2 col-form-label" for="btnRecInTransFromPlace">Receive In:</label>
            <div class="col-md-10">
                <button id="btnRecInTransFromPlace" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
            </div>
        </div>

        <div class="row mb-3">
            <label id="lblSKUSource" class="col-md-2 col-form-label" for="btnRecFromTransToPlace">Receive From:</label>
            <div class="col-md-10">
                <button id="btnRecFromTransToPlace" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
            </div>
        </div>

        <div style="display:none;">
            <div class="row mb-3">
                <label id="lblReceivingPlaceXXX" class="col-md-2 col-form-label" for="selReceivingPlace">Receive In:</label>
                <div class="col-md-10">
                    <select id="selReceivingPlace" data-role="none" class="form-select">
                        <option> Select Place </option>
                    </select>
                </div>
            </div>

            <div class="row mb-3">
                <label id="lblSKUSourceXXX" class="col-md-2 col-form-label" for="selSourceOfSKU">Receive From:</label>
                <div class="col-md-10">
                    <select id="selSourceOfSKU" data-role="none" class="form-select">
                        <option value=""> Select Source of SKU </option>
                        <option value="valIntraOrganization" id='optIntraOrganization'> Within the Company or Organization </option>
                        <option value="valThirdPartyVendor" id='optThirdPartyVendor'> Third party Company or Organization </option>
                    </select>
                </div>
            </div>

            <div class="row mb-3" id="dvIntraOrganization" style="visibility:hidden; display:none">
                <label id="lblSKUSourceXXX" class="col-md-2 col-form-label" for="selIntraOrganization"></label>
                <div class="col-md-10">
                    <select id="selIntraOrganization" data-role="none" class="form-select">
                        <option> Select Place </option>
                    </select>
                </div>
            </div>

            <div id="dvVendor" style="visibility:hidden; display:none">
                <div class="row mb-3">
                    <label class="col-md-2 col-form-label" for="lblSourceOfSKU">Vendor: <span style="color: #FF0000;">*</span></label>
                    <div class="col-md-10">
                        <input type="search" data-role="none" class="form-control" placeholder="Type or search Third Party Company" id="lblSourceOfSKU" />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-md-2 col-form-label" for="lblThirdPartyOutlet">Third Party Outlet:</label>
                    <div class="col-md-10">
                        <input type="search" data-role="none" class="form-control" placeholder="Type or search Third Party Company" id="lblThirdPartyOutlet" />
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-md-2 col-form-label" for="lblThirdPartyOutletDepartment">Third Party Outlet Department:</label>
                    <div class="col-md-10">
                        <input type="search" data-role="none" class="form-control" placeholder="Type or search Third Party Company" id="lblThirdPartyOutletDepartment" />
                    </div>
                </div>

                <div class="text-center">
                    <button id="btnAddOutlet" class="btn btn-success btn-lg" data-role="none">Add Outlet</button>
                </div>
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

    <div class="container-fluid headerInformation">

        <h5> Source Document References:</h5>
        <hr />

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="optMonthValue">Source Document Date: <span style="color: #FF0000;">*</span></label>
            <div class="col-md-6">
                <div class="input-group">
                    <select id="optMonthValue" data-role="none" class="form-select"> </select>
                    <select id="optDayValue" data-role="none" class="form-select"> </select>
                    <select id="optYearValue" data-role="none" class="form-select"> </select>
                </div>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtDocumentType">Source Document Type: <span style="color: #FF0000;">*</span></label>
            <div class="col-md-10">
                <input type="text" data-role="none" class="form-control" id="txtDocumentType" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtDocumentNo">Source Document No.: <span style="color: #FF0000;">*</span></label>
            <div class="col-md-10">
                <input type="text" data-role="none" class="form-control" id="txtDocumentNo" />
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="txtPONo">PO No.</label>
            <div class="col-md-10">
                <input type="text" data-role="none" class="form-control" id="txtPONo" />
            </div>
        </div>
    </div>

    <div class="container-fluid">
        <h5>Shipping/Delivery Details:</h5>
        <hr />

        <div class="row mb-3">
            <label class="col-md-2 col-form-label" for="rcChoiceYes">Delivered Through 3PL?</label>
            <div class="col-md-10">
                <div class="btn-group" role="group">
                    <input data-role="none" type="radio" class="btn-check" name="rdChoice" id="rcChoiceYes">
                    <label class="btn btn-outline-secondary" for="rcChoiceYes">Yes</label>

                    <input data-role="none" type="radio" class="btn-check" name="rdChoice" id="rcChoiceNo" checked>
                    <label class="btn btn-outline-secondary" for="rcChoiceNo">No</label>
                </div>
            </div>
        </div>

        <div id="dv3pl" style="visibility:hidden; display:none">
            <div class="row mb-3">
                <label class="col-md-2 col-form-label" for="txtShipper">Shipper/Delivery Company:</label>
                <div class="col-md-10">
                    <div class="input-group">
                        <input data-role="none" type="text" class="form-control" id="txtShipper">
                        <button data-role="none" id="btnShipper" class="btn btn-secondary">Go</button>
                    </div>
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-md-2 col-form-label" for="txtVessel">Vessel:</label>
                <div class="col-md-10">
                    <input type="text" data-role="none" class="form-control" id="txtVessel" />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-md-2 col-form-label" for="optMonthBLValue">Date Prepared:</label>
                <div class="col-md-6">
                    <div class="input-group">
                        <select id="optMonthBLValue" data-role="none" class="form-select"> </select>
                        <select id="optDayBLValue" data-role="none" class="form-select"> </select>
                        <select id="optYearBLValue" data-role="none" class="form-select"> </select>
                    </div>
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-md-2 col-form-label" for="txtVoyageNo">Voyage No.:</label>
                <div class="col-md-10">
                    <input type="text" data-role="none" class="form-control" id="txtVoyageNo" />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-md-2 col-form-label" for="txtWayBill">B/L or WayBill No.:</label>
                <div class="col-md-10">
                    <input type="text" data-role="none" class="form-control" id="txtWayBill" />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-md-2 col-form-label" for="optMonthInvoiceValue">Invoice Date:</label>
                <div class="col-md-6">
                    <div class="input-group">
                        <select id="optMonthInvoiceValue" data-role="none" class="form-select"> </select>
                        <select id="optDayInvoiceValue" data-role="none" class="form-select"> </select>
                        <select id="optYearInvoiceValue" data-role="none" class="form-select"> </select>
                    </div>
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-md-2 col-form-label" for="txtInvoiceNo">Invoice No.:</label>
                <div class="col-md-10">
                    <input type="text" data-role="none" class="form-control" id="txtInvoiceNo" />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-md-2 col-form-label" for="txtFreightAndHandling">Freight And Handling:</label>
                <div class="col-md-10">
                    <input type="text" data-role="none" class="form-control" id="txtFreightAndHandling" />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-md-2 col-form-label" for="txtArrastre">Arrastre:</label>
                <div class="col-md-10">
                    <input type="text" data-role="none" class="form-control" id="txtArrastre" />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-md-2 col-form-label" for="txtTotal">Total:</label>
                <div class="col-md-10">
                    <input type="text" data-role="none" class="form-control" id="txtTotal" />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-md-2 col-form-label" for="txtTotalCC">Total CC:</label>
                <div class="col-md-10">
                    <input type="text" data-role="none" class="form-control" id="txtTotalCC" />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-md-2 col-form-label" for="txtTotalCBM">Total CBM:</label>
                <div class="col-md-10">
                    <input type="text" data-role="none" class="form-control" id="txtTotalCBM" />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-md-2 col-form-label" for="txtTotalWeightInkg">Total Weight in Kg:</label>
                <div class="col-md-10">
                    <input type="text" data-role="none" class="form-control" id="txtTotalWeightInkg" />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-md-2 col-form-label" for="txtTotalCostofBundle">Total Cost of Bundle:</label>
                <div class="col-md-10">
                    <input type="text" data-role="none" class="form-control" id="txtTotalCostofBundle" />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-md-2 col-form-label" for="txtBundles">Bundles:</label>
                <div class="col-md-10">
                    <input type="text" data-role="none" class="form-control" id="txtBundles" />
                </div>
            </div>
        </div>

        <div class="mb-5">
            <h5>Other Information :</h5>
            <hr />

            <div class="row mb-3">
                <label class="col-md-2 col-form-label" for="txtTransportedBy">Transported By: <span style="color: #FF0000;">*</span></label>
                <div class="col-md-10">
                    <div class="input-group mb-3">
                        <input type="search" data-role="none" class="form-control" id="txtTransportedBy" placeholder="Enter name here" />
                        <button class="btn btn-secondary" data-role="none" id="btnSearchTransportedBy">Search</button>
                    </div>
                </div>
            </div>

            <div id="divPackSizeDiscrepency" style="display:none;">
                <div class="accordion mb-3">
                    <div class="card">
                        <div class="card-header p-0">
                            <button data-target="#divPSD" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                <b>Packaging Size Discrepancies</b>
                            </button>
                        </div>
                        <div id="divPSD" class="collapse">
                            <div class="card-body">
                                <div class="form-check mb-4">
                                    <input data-role="none" class="form-check-input" type="radio" name="radPSD" id="radPSDNoDiscrepancy" checked>
                                    <label class="form-check-label" for="radPSDNoDiscrepancy">
                                        No Discrepancy
                                    </label>
                                </div>
                                <div class="form-check mb-4">
                                    <input data-role="none" class="form-check-input" type="radio" name="radPSD" id="radPSDShortLanded">
                                    <label class="form-check-label" for="radPSDShortLanded">
                                        <b>Short-landed</b>
                                        <br />
                                        <span>Product quantities that come <i>short</i> of the standard packing size.</span>
                                        <br /><br />
                                        <span><i>e.g.</i> Standard packing size is 12 cups per pack. Items arrived at 11 cups per pack.</span>
                                    </label>
                                </div>
                                <div class="form-check">
                                    <input data-role="none" class="form-check-input" type="radio" name="radPSD" id="radPSDOverLanded">
                                    <label class="form-check-label" for="radPSDOverLanded">
                                        <b>Over-landed</b>
                                        <br />
                                        <span>Product quantities that come <i>over</i> of the standard packing size.</span>
                                        <br /><br />
                                        <span><i>e.g.</i> Standard packing size is 12 cups per pack. Items arrived at 13 cups per pack.</span>
                                    </label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popReceivingTransferOption" class="modal fade">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="txtTitlePopup"></h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div subpage="pageError" id="msgOrganization" style="display:none; color:#FF0000">Please select organization</div>
                    <div subpage="pageError" id="msgOutlet" style="display:none; color:#FF0000">Please select outlet</div>

                    <div class="mb-3">
                        <label for="selOrganization" class="form-label">Organization:</label>
                        <select id="selOrganization" data-role="none" class="form-select"></select>
                    </div>

                    <div class="mb-3">
                        <label for="selReceivingPlaceOption" class="form-label">Outlet:</label>
                        <button id="selReceivingPlaceOption" data-role="none" class="btn btn-secondary btn-block">Please select</button>
                    </div>

                    <div class="mb-3">
                        <label for="selDepartment" class="form-label">Department:</label>
                        <select id="selDepartment" data-role="none" class="form-select"></select>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" id="btnReceivingTransferOptionCancel" data-dismiss="modal" class="btn btn-secondary btn-lg btn-block">Cancel</button>
                        </div>
                        <div class="col">
                            <button data-role="none" id="btnReceivingTransferOptionOk" class="btn btn-success btn-lg btn-block">Ok</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popHistory" class="modal fade">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Record Lookup</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body p-0">
                    <ul id="listRecordLookup" class="list-group list-group-flush">
                        <li class="list-group-item">
                            <div class="row align-items-center mb-3">
                                <div class="col">RCV #: 152</div>
                                <div class="col text-right"><span class="badge rounded-pill bg-success-transparent text-success px-2 py-1 d-inline-flex align-items-center gap-1"><i class="bi bi-check-circle-fill mr-2"></i> Finalized</span></div>
                            </div>
                            <div class="row align-items-center mb-3 border g-0 rounded-1 bg-light shadow-sm mb-3">
                                <div class="col-sm p-1 p-sm-3 text-center text-sm-left"><span class="text-secondary" style="font-size:14px">From:</span> VM Cabaguio</div>
                                <div class="col-sm-1 p-1 p-sm-3 text-center">
                                    <span class="d-none d-sm-block"><i class="bi bi-arrow-right bg-primary-light p-2 text-primary rounded-pill"></i></span>
                                    <span class="d-block d-sm-none"><i class="bi bi-arrow-down bg-primary-light p-2 text-primary rounded-pill"></i></span>                                    
                                </div>
                                <div class="col-sm p-1 p-sm-3 text-center text-sm-right"><span class="text-secondary" style="font-size:14px">To:</span> Avenzo Place</div>
                            </div>
                            <div class="row gy-3">
                                <div class="col-12 col-sm-6">
                                    <div class="row align-items-center">
                                        <div class="col-auto col-sm-12 text-secondary mb-sm-1" style="font-size:14px">Received:</div>
                                        <div class="col col-sm-12 text-right text-sm-left">Sep 11, 2026</div>
                                    </div>
                                </div>
                                <div class="col-12 col-sm-6">
                                    <div class="row align-items-center">
                                        <div class="col-auto col-sm-12 text-secondary mb-sm-1" style="font-size:14px">SKU:</div>
                                        <div class="col col-sm-12 text-right text-sm-left">6</div>
                                    </div>
                                </div>
                                <div class="col-12 col-sm-6">
                                    <div class="row align-items-center">
                                        <div class="col-auto col-sm-12 text-secondary mb-sm-1" style="font-size:14px">Carrier:</div>
                                        <div class="col col-sm-12 text-right text-sm-left">Juan Cruz</div>
                                    </div>
                                </div>
                                <div class="col-12 col-sm-6">
                                    <div class="row align-items-center">
                                        <div class="col-auto col-sm-12 text-secondary mb-sm-1" style="font-size:14px">Last Updated:</div>
                                        <div class="col col-sm-12 text-right text-sm-left">Sep 11, 2026 4:30pm</div>
                                    </div>
                                </div>
                            </div>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="pageCart" custom-data-role="page" style="display:none;">
    <nav class="navbar navbar-dark bg-primary sticky-top text-light p-0">
        <div class="row" style="margin:0px !important;">
            <div class="col-4" style="padding:0px !important;">
                <button data-role="none" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0 btnBackRecCart">
                    <span id="iconHome" class="bi-arrow-left-circle"></span>
                    <div id="lblHome" class="menu-description mt-1">Back</div>
                </button>
            </div>
            <div class="col-4 offset-4" style="padding:0px !important;">
                <button data-role="none" id="btnDone" style="height:100%;" class="btn btn-success btn-block btn-lg rounded-0">
                    <span class="bi-cart4"></span>
                    <div class="menu-description mt-1">Save Receiving Cart</div>
                </button>
            </div>
        </div>
    </nav>


    @*commented by Arnie 1-17-2023 - NEW UI na gamit like sa Inventory Count
        <div class="container-fluid" id="divCartContent">
            <div class="input-group mb-3">
                <input data-role="none" id="txtSearchItem" class="form-control form-control-lg" placeholder="Search Products" autocomplete="off" />
                <button id="btnScanBarCode" title="Barcode Scanner" data-role="none" class="btn btn-secondary btn-lg divHideBarcodeScan"><span class="bi-upc-scan"></span></button>
                <button id="btnReceiveByContinuousScanning" data-role="none" class="btn btn-secondary btn-lg"><span class="bi-upc"></span></button>
            </div>
        </div>
        <ul data-role="none" id="lstItems" class="list-group mb-3"></ul>*@

    <h3 class="summary ml-3 mb-3 d-none">Summary</h3>

    <ul id="listICCategories" class="nav-container overflow-auto border p-3 list-group list-group-horizontal list-unstyled"></ul>

    <div id="listICCategorySku" style="margin-bottom:70px;"></div>

    <div class="p-3 not-summary d-none">
        <input data-role="none" id="txtSearchRawMat" class="form-control mb-3" placeholder="Search Product" autocomplete="off" />

        <div class="row mb-3 align-items-center">
            <div class="col-auto">
                Show:
            </div>
            <div class="col">
                <select data-role="none" class="form-select" id="selRawMatShow">
                    <option value="all">All Products</option>
                    <option value="only">Inventory Count Products</option>
                </select>
            </div>
        </div>
    </div>

    <div class="table-responsive mb-5 d-none">
        <table id="tblRawMatItems" class="table table-light table-striped">
            <thead>
                <tr class="align-top text-center" style="font-size:14px;">
                    <th data-sort="string" style="cursor:pointer; text-align:center">Product Description</th>
                    <th data-sort="string" style="cursor:pointer; text-align:center">Unit Measure</th>
                    <th data-sort="float" style="cursor:pointer; text-align:center; width:100px;">Good Count</th>
                    <th data-sort="float" style="cursor:pointer; text-align:center; width:100px;">Bad Count</th>
                </tr>
            </thead>
            <tbody></tbody>
        </table>
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
                            <tr class="align-top text-center" style="font-size:14px;">
                                <th class="txt-center"> Unit Measure </th>
                                <th class="txt-center"> Good Count </th>
                                <th class="txt-center"> Bad Count </th>
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
                            <button data-role="none" id="btnDoneProductBinUnitMeasuresByCategory" class="btn btn-success btn-lg btn-block">Ok</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="pageReadOnly" custom-data-role="page" style="display:none;">
    <nav class="navbar navbar-dark bg-primary mb-3 sticky-md-top" style="padding:0px !important;">
        <div class="row" style="margin:0px !important;">
            <div class="col-4 col-md-2" style="padding:0px !important;">
                <button data-role="none" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0 btnBack">
                    <span id="iconHome" class="bi-house-fill"></span>
                    <div id="lblHome" class="menu-description mt-1">Home</div>
                </button>
            </div>
            @*<div class="col-4 col-md-2" style="padding:0px !important;">
                <button data-role="none" id="btnRORecordSearch" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                    <span class="bi-search"></span>
                    <div class="menu-description mt-1">Record Search</div>
                </button>
            </div>*@
            <div class="col-4 offset-4" style="padding:0px !important;">
                <button data-role="none" id="btnNewReceivingCart" style="height:100%;" class="btn btn-success btn-block btn-lg rounded-0">
                    <span class="bi-cart4"></span>
                    <div class="menu-description mt-1">New Transaction</div>
                </button>
            </div>
        </div>
        <br />
        <div class="row" style="margin:0px; padding:0.5em; width:100%; color:#FFF; background-color:#1969de; font-weight:bold;">
            <div class="offset-5 col-7" style="text-overflow:ellipsis; overflow:hidden; text-align:right; white-space:nowrap;">
                <span id="clockReadOnly">&nbsp;</span>
            </div>
        </div>
    </nav>

    <div class="container-fluid">
        <div class="row mb-3">
            <label class="col-md-2 font-weight-bold mb-3" for="txtROReceiveIn">Receive In:</label>
            <div class="col-md-10" id="txtROReceiveIn">
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-md-2 font-weight-bold mb-3" for="txtROReceiveFrom">Receive From:</label>
            <div class="col-md-10" id="txtROReceiveFrom">
            </div>
        </div>

        <div class="headerInformation">
            <h3>Source Document References:</h3>
            <hr />

            <div class="row mb-3">
                <label class="col-md-2 font-weight-bold mb-3" for="txtRODocDate">Source Document Date:</label>
                <div class="col-md-10" id="txtRODocDate">
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-md-2 font-weight-bold mb-3" for="txtRODocType">Source Document Type:</label>
                <div class="col-md-10" id="txtRODocType">
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-md-2 font-weight-bold mb-3" for="txtRODocNo">Source Document No.:</label>
                <div class="col-md-10" id="txtRODocNo">
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-md-2 font-weight-bold mb-3" for="txtROPONo">PO No.:</label>
                <div class="col-md-10" id="txtROPONo">
                </div>
            </div>

            <div id="divRO3PL" style="display:none;">
                <h3>Shipping/Delivery Details:</h3>
                <hr />

                <div class="row mb-3">
                    <label class="col-md-2 font-weight-bold mb-3" for="txtROShipper">Shipper/Delivery Company:</label>
                    <div class="col-md-10" id="txtROShipper">
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-md-2 font-weight-bold mb-3" for="txtROVessel">Vessel:</label>
                    <div class="col-md-10" id="txtROVessel">
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-md-2 font-weight-bold mb-3" for="txtRODatePrepared">Date Prepared:</label>
                    <div class="col-md-10" id="txtRODatePrepared">
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-md-2 font-weight-bold mb-3" for="txtROVoyageNo">Voyage No.:</label>
                    <div class="col-md-10" id="txtROVoyageNo">
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-md-2 font-weight-bold mb-3" for="txtROWayBill">B/L or Waybill No.:</label>
                    <div class="col-md-10" id="txtROWayBill">
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-md-2 font-weight-bold mb-3" for="txtROInvoiceDate">Invoice Date:</label>
                    <div class="col-md-10" id="txtROInvoiceDate">
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-md-2 font-weight-bold mb-3" for="txtROInvoiceNo">Invoice No.:</label>
                    <div class="col-md-10" id="txtROInvoiceNo">
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-md-2 font-weight-bold mb-3" for="txtROFreightAndHandling">Freight And Handling:</label>
                    <div class="col-md-10" id="txtROFreightAndHandling">
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-md-2 font-weight-bold mb-3" for="txtROArrastre">Arrastre:</label>
                    <div class="col-md-10" id="txtROArrastre">
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-md-2 font-weight-bold mb-3" for="txtROTotal">Total:</label>
                    <div class="col-md-10" id="txtROTotal">
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-md-2 font-weight-bold mb-3" for="txtROTotalCC">Total CC:</label>
                    <div class="col-md-10" id="txtROTotalCC">
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-md-2 font-weight-bold mb-3" for="txtROTotalCBM">Total CBM:</label>
                    <div class="col-md-10" id="txtROTotalCBM">
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-md-2 font-weight-bold mb-3" for="txtROTotalWeightInKg">Total Weight in Kg:</label>
                    <div class="col-md-10" id="txtROTotalWeightInKg">
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-md-2 font-weight-bold mb-3" for="txtROTotalCostOfBundle">Total Cost of Bundle:</label>
                    <div class="col-md-10" id="txtROTotalCostofBundle">
                    </div>
                </div>

                <div class="row mb-3">
                    <label class="col-md-2 font-weight-bold mb-3" for="txtROBundles">Bundles:</label>
                    <div class="col-md-10" id="txtROBundles">
                    </div>
                </div>
            </div>
        </div>        

        <h3>Other Information:</h3>
        <hr />

        <div class="row mb-3">
            <label class="col-md-2 font-weight-bold mb-3" for="txtROTransportedBy">Transported By:</label>
            <div class="col-md-10" id="txtROTransportedBy">
            </div>
        </div>

        <h3>Receiving Cart:</h3>
        <hr />
        <ul id="lstROReceivingCart" data-role="none" class="list-group mb-5">
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

<div id="pageUnitMeasure" custom-data-role="page" style="display:none;">
    <div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>Unit Measure</h1>
    </div>

    <div data-role="content">
    </div>

</div>

<div id="pageCounter" custom-data-role="page" style="display:none;">
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
                <input type="number" id="txtCount" value="0" step="1" />
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

<div id="pageStockViewer" custom-data-role="page" style="display:none;">
    <nav class="navbar navbar-dark bg-primary sticky-top text-light pl-3 pr-3 mb-3">
        <div>
            <button data-role="none" class="btn btn-primary btnBackCount">
                <span class="bi-chevron-left"></span>
            </button>
            Product Details
        </div>
    </nav>

    <div class="container-fluid">
        <table data-role="none" class="mb-3">
            <tr>
                <td width="110px" class="text-center">
                    <img src="../Content/images/box.png" id="imgReceivingStockInfo" style="width: 100px;" />
                </td>
                <td>
                    <span style="font-size:20px;">
                        SKU #: <span id="spanStockNumber"></span>
                        <br />
                        <span id="spanStockDescription"></span>@*<a href="#">(MORE INFO)</a>*@
                    </span>
                </td>
            </tr>
        </table>

        <div class="mb-3">
            <div>
                <div style="text-align:center" class="mb-3"> <h3 id="dvGoodCount"> Good Count </h3> </div>
                <div id="lstUnitMeasure">

                </div>
            </div>

            <div style="visibility:hidden; display:none" id="dvStockViewerUnitMeasure">
                <div style="text-align:center" class="mb-3"> <h3> Bad Count </h3> </div>
                <div id="lstBadOrder">

                </div>
                <div class="row">
                    <label class="col-md-2 col-form-label" for="">Bad Order Reason:</label>
                    <div class="col-md-10">
                        <select id="selBadOrder" data-role="none" class="form-select">
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
        </div>

        <div class="mb-3">
            <div id="divProductSerialContainer">
                <div class="row mb-3">
                    <div class="col">
                        <h4>Product Serial Number(s)</h4>
                    </div>
                    <div class="col-auto text-right">
                        <button id="butSerialBarCode" data-role="none" class="btn btn-secondary">Scan Barcode</button>
                    </div>
                </div>

                <input id="txtProductSerials" type="text" placeholder="Enter Serial Number" value="" class="SearchText form-control mb-3" />

                <div data-role="none" id="lstProductSerial" class="list-group mb-3"></div>
            </div>
        </div>

        <div class="row mt-5 mb-3">
            <div class="col-sm mb-3 order-sm-2" style="text-align: center">
                <a id="btnDoneUnitMeasure" data-role="none" class="btn btn-block btn-success btn-lg">Update Cart</a>
            </div>
            <div class="col-sm mb-3 order-sm-1" style="text-align: center;">
                <a id="btnUpdateBarcode" data-role="none" class="btn btn-block btn-warning btn-lg">Add Barcode</a>
            </div>
            <div class="col-sm mb-3 order-sm-0" style="text-align: center">
                <a data-role="none" class="btn btn-block btn-secondary btn-lg btnBackCount">Cancel</a>
            </div>
        </div>
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

<div id="pageStockViewerDetail" custom-data-role="page" style="display:none;">
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
                                <tbody id="lstSizesUnitMeasure"></tbody>
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

<div id="pagePrintPage" custom-data-role="page" style="display:none;">
    <div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">
        <a href="#" id="" data-role="button">Print</a>
    </div>
</div>