@*<link rel="stylesheet" href="/Content/bootstrap-5.0.0/css/bootstrap.css">
<link rel="stylesheet" href="/Content/bootstrap-5.0.0/bootstrapfive-icons.css">
<link type="text/css" href="/Content/bootstrap-custom.css" rel="Stylesheet" />*@
<script>
    require([GetPathFormat('/Scripts3/Bs_Stock.js?crc=')]);
</script>

<nav class="navbar navbar-dark sticky-md-top bg-primary" style="padding:0px !important;">
    <div class="row" style="margin:0px;">
        <div class="col-2" style="padding:0px !important;">
            <button data-role="none" id="btnStockInfo" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                <span class="bi-info-circle"></span>
                <div class="menu-description mt-1">Stock Info</div>
            </button>
        </div>
        <div class="col-2" style="padding:0px !important;">
            @*<button data-role="none" id="btnSalesPerson" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                <span class="bi-person-fill"></span>
                <div class="menu-description mt-1">Indicate Sales Rep</div>
            </button>*@
        </div>
        <div class="col-2" style="padding:0px !important;">
            @*<button data-role="none" id="btnSerialNumber" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                <span class="bi-upc"></span>
                <div class="menu-description mt-1">Serial Number</div>
            </button>*@
        </div>
        <div class="col-2" style="padding:0px !important;">
            @*<button data-role="none" id="btnInventoryBalances" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                <span class="bi-list-check"></span>
                <div class="menu-description mt-1">See Inventory Balances</div>
            </button>*@
        </div>
        <div class="col-4 offset-md-2 col-md-2" style="padding:0px !important;">
            <button data-role="none" name="butUnitUpdateCart" style="height:100%;" class="btn btn-success btn-block btn-lg rounded-0">
                <span class="bi-cart-fill"></span>
                <div class="menu-description mt-1">Update Cart</div>
            </button>
        </div>
    </div>
    <br />
    <div class="row" style="margin:0px; padding:0.5em; width:100%; color:#FFF; background-color:#1969de; font-weight:bold;">
        <div class="col-5 lblPosBranch" style="text-overflow:ellipsis; overflow:hidden; text-align:left; white-space:nowrap;">
            &nbsp;
        </div>
        <div class="col-7" style="text-overflow:ellipsis; overflow:hidden; text-align:right; white-space:nowrap;">
            <span id="clockStock">&nbsp;</span>
        </div>
    </div>
</nav>

<div class="container-fluid">
    <table style="width:100%;">
        <tr>
            <td style="width: 120px; padding: 10px;">
                <img src="../Content/images/box.png" id="imgStockInfo" style="width: 100px;" />
            </td>
            <td id="divItemDetailsStockNumberCell" style="padding:10px;">
                <div style="font-size:1.50em;">
                    <div id="divItemDetailsStockNumber">
                        SKU #: <span id="spanStockNumber"></span>
                        <br />
                    </div>
                    <span id="spanStockDescription"></span>@*<a href="#">(MORE INFO)</a>*@
                </div>
                <div class="mt-3 row" id="divVATExempt" style="display:none;">
                    <label for="selVATExempt" class="col-md-1">VAT Exempt:</label>
                    <div class="col-md-1">
                        <select id="selVATExempt" class="form-select">
                            <option value="0">No</option>
                            <option value="1">Yes</option>
                        </select>
                    </div>
                </div>
            </td>
        </tr>
    </table>

    <div id="divStockInfoLoading" subpage="pageStockInfo" style="display:block; width:100%; text-align:center;">
        <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden"></span>
        </div>
        <br /> Please wait while getting data ...
    </div>

    <div id="divStockInfo" subpage="pageStockInfo" style="display:none;">
        <h3>Stock Info</h3>
        <br />
        <div id="divUnit" class="mb-3" style="display:none">
            <div class="divUnitContainer">
                <div name="divStockMinimumCapacity">
                    Minimum Capacity: <span name="spanStockMinimumCapacity"></span>
                </div>
                <div name="divStockMaximumCapacity">
                    Maximum Capacity: <span name="spanStockMaximumCapcity"></span>
                </div>
                <div class="row">
                    <div class="col-6">
                        <table>
                            <tr>
                                <td><span name="spanPriceLevel">Price Level</span>:</td>
                                <td class="pl-3">
                                    <div name="divPriceLevel">
                                    </div>
                                </td>
                            </tr>
                            <tr name="divRequestedUnitPrice" style="display: none;">
                                <td>Requested Unit Price:</td>
                                <td class="pl-3"><span name="spanRequestedUnitPrice"></span></td>
                            </tr>
                            <tr>
                                <td><span name="spanUnit">Can</span>:<span name="spanUnitMeasureID" style="display:none;"></span></td>
                                <td class="pl-3"><span name="spanPrice"></span></td>
                            </tr>
                            <tr>
                                <td><span name="spanUnitQtyLabel">Can per Can</span>:</td>
                                <td class="pl-3"><span name="spanUnitQty"></span></td>
                            </tr>
                            <tr>
                                <td>Price per <span name="spanBaseUnit">Can</span>:</td>
                                <td class="pl-3"><span name="spanPricePerBase"></span></td>
                            </tr>
                            <tr>
                                <td>Savings per <span name="spanBaseUnit">Can</span>:</td>
                                <td class="pl-3"><span name="spanSavingsPerBase"></span></td>
                            </tr>
                            <tr name="divTotalSavings">
                                <td>Savings per <span name="spanUnit">Can</span>:</td>
                                <td class="pl-3"><span name="spanTotalSavings"></span></td>
                            </tr>
                            <tr name="divSrp">
                                <td>SRP:</td>
                                <td class="pl-3"><span name="spanSrp"></span></td>
                            </tr>
                        </table>
                    </div>
                    <div class="col-6 offset-md-3 col-md-3">
                        <div class="input-group mb-3">
                            <button data-role="none" name="butUnitMinus" class="btn btn-danger" type="button" style="font-size:20px;"><span class="bi-dash"></span></button>
                            <input data-role="none" type="text" name="txtQtyTemplate" style="font-size:18px; text-align:center;" class="form-control" placeholder="Qty">
                            <button data-role="none" name="butUnitPlus" class="btn btn-success" type="button" style="font-size:20px;"><span class="bi-plus"></span></button>
                        </div>
                        <button data-role="none" name="butUnitUpdateCart" class="btn btn-success btn-block" style="height:80px;"><span class="bi-cart-fill"></span> Update Cart</button>
                    </div>
                </div>
            </div>
        </div>

        <div id="divStockInfoUnit">

        </div>

        <div id="divSubTotal" style="text-align: center; padding-bottom: 10px">
            <h3>
                Sub Total : PhP <span id="spanSubtotal">0.00</span>
            </h3>
        </div>
    </div>

    <div id="divSalesRep" subpage="pageStockInfo" style="display:none;">
        <h3>Sales Representatives</h3>
        <br />
        <input data-role="none" id="txtProductSalesRepSearch" type="search" class="form-control form-control-lg" placeholder="Search Sales Representative here" value="" />
        <br />
        <div id="lstSalesRep" class="list-group">
        </div>
    </div>

    <div id="divSerialNumber" subpage="pageStockInfo" style="display:none;">
        <h3>Product Serial Number</h3>
        <br />
        <div class="input-group mb-3">
            <input data-role="none" id="txtProductSerial" class="form-control form-control-lg" placeholder="Enter Serial Number here" autocomplete="off" />
            <button id="butSerialBarCode" data-role="none" class="btn btn-secondary btn-lg"><span class="bi-upc-scan"></span></button>
        </div>
        <div id="lstProductSerial" class="list-group">
        </div>
    </div>

    <div id="divInventoryBalances" subpage="pageStockInfo" style="display:none;">
        <h3>Inventory Balances</h3>
        <br />
        <table id="tblInventoryBalance"></table>
    </div>

    <div class="mt-5 mb-5 text-center">
        <button data-role="none" id="btnStockInfoBack" class="btn btn-secondary btn-lg">Back to Main Page</button>
    </div>    
</div>