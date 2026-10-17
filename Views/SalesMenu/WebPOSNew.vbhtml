@Code
    ViewData("Title") = "Web POS"
    
    If Request.IsAjaxRequest() Then
        Layout = ""
    Else
        Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
    End If
End Code
<style type="text/css">
    body {
        overflow:hidden;
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

    .webposwrapper-left {
        height: 100vh;
        display: grid;
        grid-template-rows: 128px 1fr 119px;
    }

    .webposwrapper-right {
        height: 100vh;
        display: grid;
        grid-template-rows: 128px 1fr 80px;
    }

    .webposcartcontent {
        flex: 1;
        overflow: auto;
    }
</style>

@*<link type="text/css" href="/Content/flatui-css-custom.css" rel="Stylesheet" />*@
<link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
<link rel="stylesheet" href="/Content/Ecommerce/css/lightbox.css">
<script>
    require(['bootstrapjs'], function () {
        require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
            require(['promise!/Scripts/sources/SalesMenu/WebPOS.js?v=' + assetversion]);
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

<div id="divWebPOS" style="display:block;">
    <table border="0" cellpadding="0" style="width:100%; height:100%; top:0; bottom:0; position:absolute;">
        <tbody>
            <tr>
                <td class="bg-light" style="vertical-align:top;">
                    <div class="vh-100 d-flex flex-column overflow-hidden">
                        <div class="flex-shrink-1">
                            <div class="bg-dark text-white" style="overflow:hidden;">
                                <div class="row row-cols-3 align-items-center">
                                    <div class="col-auto col-sm-auto">
                                        <button data-role="none" name="btnWebPosBack" class="btn btn-dark">
                                            <span class="bi-house-fill"></span> Home
                                        </button>
                                    </div>
                                    <div class="col-auto col-sm">
                                        <div id="lblCompanyName" style="font-size:25px; text-align:center;white-space:nowrap; text-overflow:ellipsis; overflow:hidden;">
                                            &nbsp;
                                        </div>
                                    </div>
                                    <div class="col-12 col-sm-auto text-right pr-3">
                                        <div>Store Branch: <span id="lblFowstName"></span></div>
                                        <div id="divClock"></div>
                                    </div>
                                </div>

                                <div class="p-3" style="display:none">
                                    <form id="formPunchItem" autocomplete="off">
                                        <input data-role="none" id="txtPunchItem" class="form-control form-control-lg" autocomplete="off" placeholder="Loading. Please wait" />
                                    </form>
                                </div>
                            </div>
                        </div>
                        <div name="maincontainer" class="flex-fill d-flex flex-row overflow-auto w-100 h-100">
                            <div name="divPOSViewLoading" class="pt-5" style="width:100%; text-align:center; display:block;">
                                <div class="spinner-border text-dark" role="status">
                                    <span class="visually-hidden"></span>
                                </div>
                            </div>
                            <div class="table-fixed w-100 h-100" name="divRetail" style="display:none">
                                <table data-role="none" name="tableProducts" class="table table-light table-hover table-striped">
                                    <thead class="table-light" style="font-size:12px; vertical-align:top;">
                                        <tr>
                                            <th class="txt-center fixed-col"> SKU # </th>
                                            <th class="txt-center fixed-col"> Product Description </th>
                                            <th class="txt-center fixed-col text-nowrap"> Order Qty </th>
                                            <th name="thServedQty" class="txt-center fixed-col text-nowrap"> Served Qty </th>
                                            <th class="txt-center fixed-col text-nowrap"> Packing Size </th>
                                            <th class="txt-center fixed-col text-nowrap"> Unit Price </th>
                                            <th class="txt-center fixed-col text-nowrap"> Line Total </th>
                                            @*<th class="txt-center fixed-col"> Payment Terms </th>*@
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr name="data" class="fixed-cell-width" style="display:none;font-size:20px;">
                                            <td name="ProductNumber" class="txt-left"> SKU # </td>
                                            <td name="ProductDescription" class="txt-left"> Product Description </td>
                                            <td name="OrderQty" class="txt-center"> Order Qty </td>
                                            <td name="ServedQty" class="txt-center"> </td>
                                            <td name="PackingSize" class="txt-center"> Packing Size </td>
                                            <td name="UnitPrice" class="txt-center"> Unit Price </td>
                                            <td name="LineTotal" class="txt-center"> Line Total </td>
                                            @*<td name="PaymentTerms" class="txt-center"> Payment Terms </td>*@
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                        <div class="flex-shrink-1">
                            <div class="row g-0" name="divLine">
                                <div class="col d-none">
                                    @*hide only*@
                                    <button data-role="none" name="butHotButtons" class="btn btn-secondary btn-block h-100 rounded-0">Order using Hot Buttons</button>
                                </div>
                                <div class="col text-center align-self-center" style="font-size:20px;">
                                    No. of Line Items: <b id="divNumberOfLineItems">0</b>
                                </div>
                                <div class="col text-center align-self-center" style="font-size:20px;">
                                    No. of Items: <b id="divNumberOfItems">0</b>
                                </div>
                            </div>
                            <div class="row gx-0" style="height:70%;">
                                <div class="col-2">
                                    <div data-role="none" name="butNew" class="btn btn-dark border-dark btn-block rounded-0 align-self-auto" style="height:100%">
                                        <span class="bi-cart4" style="font-size:25px;"></span>
                                        <br />
                                        <div class="text-truncate">New</div>
                                    </div>
                                </div>
                                <div class="col-2">
                                    <button data-role="none" name="butLookup" class="btn btn-dark border-dark btn-block rounded-0" style="height:100%">
                                        <span class="bi-search" style="font-size:25px;"></span>
                                        <br />
                                        <div class="text-truncate">Search</div>
                                    </button>
                                </div>
                                <div class="col-2">
                                    <button data-role="none" name="butSaveAsDraft" class="btn btn-dark border-dark btn-block rounded-0" style="height:100%">
                                        <span class="bi-save" style="font-size:25px;"></span>
                                        <br />
                                        <div class="text-truncate">Save As Draft</div>
                                    </button>
                                </div>
                                <div class="col-2">
                                    <button data-role="none" name="butOutput" class="btn btn-dark border-dark btn-block rounded-0" style="height:100%">
                                        <span class="bi-receipt" style="font-size:25px;"></span>
                                        <br />
                                        <div class="text-truncate">POS Reports</div>
                                    </button>
                                </div>
                                <div class="col-2">
                                    <button data-role="none" name="butSalesReturn" class="btn btn-dark border-dark btn-block rounded-0" style="height:100%">
                                        <span class="bi-box-arrow-left" style="font-size:25px;"></span>
                                        <br />
                                        <div class="text-truncate">Sales Return</div>
                                    </button>
                                </div>
                                <div class="col-2">
                                    <button data-role="none" name="butMoreOptions" class="btn btn-dark border-dark btn-block rounded-0" style="height:100%">
                                        <span class="bi-gear" style="font-size:25px;"></span>
                                        <br />
                                        <div class="text-truncate">More Options</div>
                                    </button>
                                </div>
                                <div style="display:none;">
                                    <button data-role="none" name="butPaymentSave" class="btn btn-outline-success btn-block" style="height:100%">
                                        Save Only
                                    </button>
                                    <button data-role="none" name="butPaymentSaveAndEmail" class="btn btn-outline-success btn-block" style="height:100%">
                                        Save and Email Receipt
                                    </button>
                                    <button data-role="none" style="display:none;" name="butPayInvoice">Pay</button>
                                    <input name="txtDateNeeded" data-role="none" type="text" id="btnInvoicingDateNeeded" class="form-control">
                                    <textarea name="txtBillingAddress" data-role="none" class="form-control" style="height:90px; max-height:90px; resize:none"></textarea>
                                    <textarea name="txtDeliveryAddress" data-role="none" class="form-control" style="height:90px; max-height:90px; resize:none"></textarea>
                                    <input id="txtPaymentManualReceiptNumber" maxlength="30" class="ui-input-text ui-body-c">
                                </div>
                            </div>
                        </div>
                    </div>
                </td>
                <td class="bg-dark" style="min-width:400px; width:400px; vertical-align:top;">
                    <div class="vh-100 d-flex flex-column overflow-hidden">
                        <div class="flex-shrink-1">
                            <div name="divReconstructData" class="bg-light" style="display:none;">
                                <div class="p-2 alert-danger font-weight-bold">
                                    @*<div class="bi-tools text-center" style="font-size:50px;"></div>*@
                                    <div class="row">
                                        <div class="col">Recontruct Sales Data</div>
                                        <div class="col-auto text-right"><a data-role="none" class="link-danger">EXIT</a></div>
                                    </div>
                                </div>
                                <div class="p-2">
                                    <div class="row align-items-center">
                                        <div for="txtRSDDate" class="col-2">Date:</div>
                                        <div class="col-4">
                                            <input data-role="none" type="date" class="form-control" id="txtRSDDate">
                                        </div>
                                        <div for="txtRSDTime" class="col-2">Time:</div>
                                        <div class="col-4">
                                            <input data-role="none" type="time" class="form-control" id="txtRSDTime">
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="bg-light pl-2 pr-2 pb-1">
                                <div class="row">
                                    <div class="col-auto">
                                        Sub Total:
                                    </div>
                                    <div id="webPosCartSubTotal" class="col text-right">
                                        0.00
                                    </div>
                                </div>
                                <div class="row" id="rowVATPercent" style="display:none;">
                                    <div class="col-auto">
                                        Less <span id="lblVATPercentValue"></span>% VAT:
                                    </div>
                                    <div id="webPosCartVATPercent" class="col text-right">
                                        0.00
                                    </div>
                                </div>
                                <div class="row" id="rowAmountLessVAT" style="display:none;">
                                    <div class="col-auto">
                                        Total VAT-Ex Amount:
                                    </div>
                                    <div id="webPosCartTotalVATExAmount" class="col text-right">
                                        0.00
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-auto">
                                        Less Discount Amount:
                                    </div>
                                    <div id="webPosCartTotalDiscount" class="col text-right">
                                        0.00
                                    </div>
                                </div>
                                <div class="row" id="rowVATBackPercent" style="display:none;">
                                    <div class="col-auto">
                                        Add VAT:
                                    </div>
                                    <div id="lblVATBackPercent" class="col text-right">
                                        0.00
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-auto">
                                        Add Charges:
                                    </div>
                                    <div id="lblSalesPayCharges" class="col text-right">
                                        0.00
                                    </div>
                                </div>
                                <div class="row" style="font-size:30px;">
                                    <div class="col-auto">
                                        <b>Amount Due:</b>
                                    </div>
                                    <div id="webPosCartTotal" class="col text-right font-weight-bolder">
                                        0.00
                                    </div>
                                </div>
                                <div class="row" style="font-size:16px;">
                                    <div class="col">
                                        Amount Tendered:
                                    </div>
                                    <div id="lblTotalPaymentRcv" class="col-auto text-right font-weight-bolder">
                                        0.00
                                    </div>
                                </div>
                                <div class="row text-danger" style="font-size:16px;">
                                    <div class="col">
                                        Amount Lacking:
                                    </div>
                                    <div id="lblAmountLacking" class="col-auto text-right font-weight-bolder">
                                        0.00
                                    </div>
                                </div>
                                <div class="row text-success" style="font-size:20px;">
                                    <div class="col">
                                        CHANGE:
                                    </div>
                                    <div id="lblPaymentChange" class="col-auto text-right font-weight-bolder">
                                        0.00
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="flex-fill d-flex flex-row overflow-auto">
                            <div class="row gx-0" style="height:100%;">
                                <div class="col-8">
                                    <button data-role="none" name="txtCustomer" class="btn btn-dark border-dark rounded-0 btn-block" style="height:100%;">
                                        Customer:
                                        <br /><span id="lblCustomerName">[CASH PAYMENT]</span>
                                    </button>
                                </div>
                                @*<div class="col">
            <button data-role="none" name="btnFOWST" class="btn btn-dark border-dark rounded-0 btn-block" style="height:100%; font-size:14px;">
                FOWST:
                <br />
            </button>
        </div>*@
                                <div class="col-4">
                                    <button data-role="none" id="btnPaymentDiscounts" class="btn btn-success btnpaymentmethod border-dark darker rounded-0 btn-block" style="height:100%;">
                                        Discounts
                                        <br />
                                        <b id="lblPaidDiscounts" style="font-size:25px;"></b>
                                    </button>
                                </div>
                                <div class="col-4">
                                    <button data-role="none" id="btnPaymentCash" class="btn btn-success btnpaymentmethod darker border-dark rounded-0 btn-block" style="height:100%;">
                                        Cash
                                        <br />
                                        <b id="lblPaidCash" style="font-size:25px;"></b>
                                    </button>
                                </div>
                                <div class="col-4">
                                    <button data-role="none" id="btnPaymentOther" class="btn btn-success btnpaymentmethod darker border-dark rounded-0 btn-block" style="height:100%;">
                                        Other Payment
                                        <br />
                                        <b id="lblPaidOther" style="font-size:25px;"></b>
                                    </button>
                                </div>
                                <div class="col-4">
                                    <button data-role="none" id="btnPaymentCheque" class="btn btn-success btnpaymentmethod darker border-dark rounded-0 btn-block" style="height:100%;">
                                        Cheque / GC
                                        <br />
                                        <b id="lblPaidCheque" style="font-size:25px;"></b>
                                    </button>
                                </div>
                                <div class="col-4">
                                    <button data-role="none" id="btnPaymentDCCard" class="btn btn-success btnpaymentmethod darker border-dark rounded-0 btn-block" style="height:100%;">
                                        Card
                                        <br />
                                        <b id="lblPaidDCCard" style="font-size:25px;"></b>
                                    </button>
                                </div>
                                <div class="col-4">
                                    <button data-role="none" id="btnPaymentSrv" class="btn btn-success btnpaymentmethod darker border-dark rounded-0 btn-block" style="height:100%;">
                                        SRV
                                        <br />
                                        <b id="lblPaidSrv" style="font-size:25px;"></b>
                                    </button>
                                </div>
                                <div class="col-4">
                                    <button data-role="none" id="btnPaymentOnCredit" class="btn btn-success btnpaymentmethod border-dark darker rounded-0 btn-block" style="height:100%;">
                                        On Credit
                                        <br />
                                        <b id="lblPaidOnCredit" style="font-size:25px;"></b>
                                    </button>
                                </div>
                                <div class="col-4">
                                    <button data-role="none" id="btnPaymentCharges" class="btn btn-success btnpaymentmethod border-dark darker rounded-0 btn-block" style="height:100%;">
                                        Charges
                                        <br />
                                        <b id="lblPaidCharges" style="font-size:25px;"></b>
                                    </button>
                                </div>
                                <div class="col-4">
                                    <button data-role="none" id="btnPaymentRewards" class="btn btn-success btnpaymentmethod border-dark darker rounded-0 btn-block" style="height:100%;">
                                        Rewards
                                        <br />
                                        <b id="lblRewardsCardNo"></b>
                                    </button>
                                </div>
                                <div class="col-4">
                                    <button data-role="none" id="btnDeliveryInfo" class="btn btn-success darker border-dark rounded-0 btn-block" style="height:100%;">
                                        Delivery Info
                                    </button>
                                </div>
                            </div>
                        </div>
                        <div class="flex-shrink-1">
                            <div class="row gx-0 text-center text-white bg-dark" style="font-size:12px; height:100%;">
                                <div class="col-6 align-self-center border-dark">
                                    <button data-role="none" class="btn btn-dark border-dark btn-block rounded-0" style="height:100%">
                                        <span class="bi-person-fill" style="font-size:25px;"></span>
                                        <br />
                                        <div class="text-truncate">Cashier: <b id="lblCashierName"></b></div>
                                    </button>
                                    <div class="d-none" style="font-size:12px;">
                                        @*<br />
                                            POS Permit No: <b id="bPosPermit">Initializing</b>
                                            <br />
                                            POS Serial No: <b id="bPosSerial">Initializing</b>*@
                                    </div>
                                </div>
                                <div class="col-6 text-success font-weight-bolder">
                                    <button data-role="none" name="butPaymentSaveAndPrint" class="btn btn-warning btn-lg btn-block rounded-0" style="height:100%;">
                                        PRINT RECEIPT
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                </td>
            </tr>
        </tbody>
    </table>

    <div id="popReports" data-backdrop="static" class="modal fade" tabindex="-1" style="display:none;">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title"></h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">

                </div>
            </div>
        </div>
    </div>
</div>