<div custom-data-role="page" id="pageMain" style="display:none;">
    <nav class="bg-primary text-white mb-3 sticky-top">
        <div class="container-fluid">
            <table style="width:100%;">
                <tr>
                    <td style="width:50px;">
                        <button data-role="none" id="btnStockBack" class="btn btn-primary btn-lg btn-block text-white">
                            <span class="bi-chevron-left"></span>
                        </button>
                    </td>
                    <td><div><h5 class="m-0">SKU Details</h5></div></td>
                </tr>
            </table>
        </div>
    </nav>

    <div class="container-fluid">
        <script>
            require(['common', GetPathFormat('/Scripts3/Bs_SalesReturnsStock.js?v=' + assetversion)]);
        </script>
        <div class="ui-bar-c" style="border: none">
            <div class="row align-items-center">
                <div class="col-auto" style="text-align: center">
                    <img src="../../Content/images/box.png" />
                </div>
                <div class="col">
                    <h3>
                        SKU #: <span id="spanStockNumber"></span>
                        <br />
                        <span id="spanStockDescription"></span><a href="#">(MORE INFO)</a>
                    </h3>
                </div>
            </div>
            <div class="ui-bar-c" id="divUnit" data-theme="c" style="padding: 10px">
                <div class="row">
                    <div class="col">
                        <b name="spanUnit"></b>
                    </div>
                    <div class="col">
                        Php <span name="spanPrice"></span>
                    </div>
                </div>
                <div>
                    Qty invoiced: <span name="spanQtyInvoiced"></span>
                </div>
                <div>
                    Qty returned: <span name="spanQtyReturned"></span>
                </div>
                <div>
                    Qty returnable: <span name="spanQtyReturnable"></span>
                </div>
                <div class="mt-2 mb-2">
                    <input name="txtQtyTemplate" data-role="none" class="form-control" type="number" placeholder="RETURN QTY" maxlength="5" />
                </div>
                <div name="divReason">
            
                </div>
            </div>
            <div style="text-align: center;">
                <h3>
                    Sub Total : PhP <span id="spanSubtotal"></span>
                </h3>
            </div>
        </div>
        <button id="butUpdateReturns" data-role="none" class="btn btn-success btn-lg btn-block mt-5 mb-5">
            Update Returns Cart
        </button>
     </div>
 </div>