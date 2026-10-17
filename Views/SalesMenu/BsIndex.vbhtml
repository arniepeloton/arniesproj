@Code
    ViewData("Title") = "Welcome"
    
    If Request.IsAjaxRequest() Then
        Layout = ""
    Else
        Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
    End If
End Code
<style type="text/css">
    body {
        overflow-x:hidden;
    }

    .menu-description {
        font-size: 13px;
        overflow: hidden;
        text-overflow: ellipsis;
        display: -webkit-box;
        -webkit-line-clamp: 2;
        -webkit-box-orient: vertical;
        height: 40px;
        cursor:pointer;
    }

    .modal, .modal-backdrop {
        bottom: 0 !important;
    }

    .no-hover.btn-outline-danger:hover{
        color: #dc3545 !important;
        background-color: initial !important;
    }

    .no-hover.btn-outline-success:hover{
        color: #28a745 !important;
        background-color: initial !important;
    }

    .required {
        color: #FF0000;
    }
</style>

@*<link type="text/css" href="/Content/flatui-css-custom.css" rel="Stylesheet" />*@
<link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
<link rel="stylesheet" href="/Content/Ecommerce/css/lightbox.css">
<script>
    require(['bootstrapjs'], function () {
        require(['/Content/bootstrap/js/bootstrap_modal.js?v='], function () {
            require([GetPathFormat('/Scripts3/Bs_SalesMenu.js?crc=')]);
            require([GetPathFormat('/Scripts3/ItemModifiers.js?crc=')]);
        })
    })        
    //        require([GetPathFormat('/Scripts3/others/JsToPDF/jquery-ui-1.8.17.custom.min.js?crc=')]);        
    //        require([GetPathFormat('/Scripts3/others/JsToPDF/jspdf.js?crc=')]);        
    //    require([GetPathFormat('/Scripts3/others/JsToPDF/html2canvas.js?crc=')]);
    //    require([GetPathFormat('/Scripts3/others/JsToPDF/jspdf.min.js?crc=')]);
    //    require([GetPathFormat('/Scripts3/others/JsToPDF/adler32cs.js?crc=')]);
    //    require([GetPathFormat('/Scripts3/others/JsToPDF/FileSaver.js?crc=')]),
    //    require([GetPathFormat('/Scripts3/others/JsToPDF/jspdf.plugin.addimage.js?crc=')]);
    //    require([GetPathFormat('/Scripts3/others/JsToPDF/jspdf.plugin.standard_fonts_metrics.js?crc=')]);
    //    require([GetPathFormat('/Scripts3/others/JsToPDF/jspdf.plugin.split_text_to_size.js?crc=')]);
    //    require([GetPathFormat('/Scripts3/others/JsToPDF/jspdf.plugin.from_html.js?crc=')]);
    //    require([GetPathFormat('/Scripts3/others/JsToPDF/basic.js?crc=')]);
    //    require([GetPathFormat('/Scripts3/others/JsToPDF/jspdf.debug.js?crc=')
    //        ]);

    function skupicerror(dis) {
        $(dis).attr('isimgerror', 1).attr('src', '../Content/images/box.png');
        Android.AddLog('Can\'t load image no. ' + $(dis).data('data').Ctr + ' for SKU: ' + $(dis).data('data').ProductName);

        if (CInt2($(dis).attr('isimgviewer')) == 1) {
            $('#lightboxOverlay, #lightbox').hide();
        }            
    }
</script>

<div id="divLoading" custom-data-role="page">
    <div class="d-flex position-absolute w-100 h-100 justify-content-center align-items-center">
        <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden"></span>
        </div>
    </div>
</div>

<div id="pageMain" custom-data-role="page" style="display:none;">
    <nav class="navbar navbar-dark sticky-md-top bg-primary" style="padding:0px !important;">
        <div class="bg-warning text-dark font-weight-bold pl-2 pr-2 w-100" name="hRsd" style="display:none;">
            <div class="row align-items-center">
                <div class="col">Reconstruct Sales Data</div>
                <div class="col-auto"><button data-role="none" class="btn" name="btnExitRSD">Exit</button></div>
            </div>
        </div>
        <div class="row" style="margin:0px !important;">
            <div class="col-3 col-md-2" style="padding:0px !important;">
                <button data-role="none" id="btnHome" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                    <span id="iconHome" class="bi-house-fill"></span>
                    <div id="lblHome" class="menu-description mt-1">Home</div>
                </button>
            </div>
            <div class="col-3 col-md-2" style="padding:0px !important;">
                <div id="divBlockCustomer">
                    <button data-role="none" id="btnCustomer" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                        <span class="bi-person-fill"></span>
                        <div class="menu-description mt-1"><span id="lblCustomerName">[CASH PAYMENT]</span></div>
                    </button>
                </div>
                <div style="display:none;" id="divBlockSupplier">
                    <button data-role="none" id="btnSupplier" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                        <span class="bi-person-fill"></span>
                        <div class="menu-description mt-1"><span id="lblSupplierName">&nbsp;</span></div>
                    </button>
                </div>
            </div>
            <div class="col-3 col-md-2" style="padding:0px !important; display:none;">
                @*hide this menu, gibutang na sya sa More Options *@
                <button data-role="none" id="btnPOSMode" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                    <span class="bi-display"></span>
                    <div class="menu-description mt-1" id="lblCurrentPOSMode"></div>
                </button>
            </div>
            <div class="col-3 col-md-2" style="padding:0px !important;">
                <button data-role="none" id="btnMoreOptions" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                    <span class="bi-gear-fill"></span>
                    <div class="menu-description mt-1">More Options</div>
                </button>
            </div>
            <div class="col-3 offset-md-3 col-md-3" style="padding:0px !important;">
                <button data-role="none" id="lnkCheckout" style="height:100%;" class="btn btn-success btn-block btn-lg rounded-0">
                    <span id="iconPay" class="bi-cash"></span>
                    <span id="iconSave" class="bi-check-circle-fill" style="display:none;"></span>
                    <div id="lblPayNow" class="menu-description mt-1">Pay Now</div>
                </button>
            </div>
        </div>
        <br />
        <div class="row" style="margin:0px; padding:0.5em; width:100%; color:#FFF; background-color:#1969de; font-weight:bold;">
            <div class="col-5 lblPosBranch" style="text-overflow:ellipsis; overflow:hidden; text-align:left; white-space:nowrap;">
                &nbsp;
            </div>
            <div class="col-7" style="text-overflow:ellipsis; overflow:hidden; text-align:right; white-space:nowrap;">
                <span id="clock">&nbsp;</span>
            </div>
        </div>
    </nav>

    <div style="margin-top:15px; margin-bottom:50px;">        
        <div id="divCartPanel" subpage="pageMain" style="display:block;">
            <div class="container-fluid">
                <div class="input-group mb-3">
                    <input data-role="none" id="txtSearch" class="form-control form-control-lg" placeholder="Search Products" autocomplete="off" />
                    <button id="butBarCode" title="Barcode Scanner" data-role="none" class="btn btn-secondary btn-lg divHideBarcodeScan"><span class="bi-upc-scan"></span></button>
                    <button id="divSearchMulti" title="Add Multiple Products" data-role="none" class="btn btn-secondary btn-lg"><span class="bi-list-check"></span></button>
                    <button id="butHotButtonsShortcut" title="Hot Button Mode" data-role="none" class="btn btn-secondary btn-lg divHideHotButtonsButton"><span class="bi-grid-3x3-gap-fill"></span></button>
                </div>

                <div class="row mb-3">
                    <div class="col-2"><h3>Cart</h3></div>
                    <div class="col-10 text-right"><h3><b id="spanCartTotal"></b></h3></div>
                </div>
            </div>

            <ul id="divCartRetail" class="list-group"></ul>

            <div id="divCartImage"></div>
        </div>        

        <div id="divRecentlyBoughtPanel" subpage="pageMain" style="display:none;">
            <div class="container-fluid">
                <div class="row mb-3">
                    <div class="col-6">
                        <div style="font-size: xx-large">
                            Recently Bought
                        </div>
                        By <label id="lblCustomerNameRecent"></label>
                    </div>
                    <div class="col-6 text-right">
                        <button data-role="none" id="lnkRecentlyBought" href="#pageRecentMenu" class="btn btn-light">Within the last 2 weeks &#x25BC;</button>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-6">
                        Click on any item to quickly add to <label id="lblCustomerNameRecent2"></label>’s cart
                    </div>
                    <div class="col-6 text-right">
                        <button data-role="none" id="butRecentlyBoughtAddAll" class="btn btn-secondary">Add all recently bought items to cart</button>
                    </div>
                </div>
            </div>
            
            <ul id="divRecentlyBoughtSection" class="list-group"></ul>
        </div>
    </div>
</div>

<div id="pageCustomerOptions" custom-data-role="page" style="display:none;">

</div>

<div id="pageStockInfo" custom-data-role="page" style="display:none;">

</div>

<div id="pagePayment" custom-data-role="page" style="display:none;">
    <div id="divPaymentInfoLoading" class="mt-5" style="width:100%; text-align:center;">
        <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden"></span>
        </div>
    </div>
</div>

<div id="pageDuplicateReceipt" custom-data-role="page" style="display:none;">
    <div id="divDuplicateReceiptLoading" class="mt-5" style="width:100%; text-align:center;">
        <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden"></span>
        </div>
    </div>
</div>

<div id="pageHotButton" custom-data-role="page" style="display:none;">
    <div id="divHotButtonLoading" class="mt-5" style="width:100%; text-align:center;">
        <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden"></span>
        </div>
    </div>
</div>

<div id="popupChangePriceLevel" class="modal fade">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Change Prive Level</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <select id="selPriceLevel" data-role="none" class="form-select"></select>
            </div>
            <div class="modal-footer">
                <button data-role="none" data-dismiss="modal" class="btn btn-success btn-block">Ok</button>
            </div>
        </div>
    </div>
</div>

<div class="mt-5 mb-3 text-center container-fluid" style=" display: none;">
    This new User Inteface (UI) of Retail Mode is still under development, if you encountered some errors or if you do not find the feature that you are looking for, you may click the button below to switch back to old UI?
    <br /><br />
    <button data-role="none" id="btnSwitchOldUI" class="btn btn-outline-dark">Switch back now</button>
</div>