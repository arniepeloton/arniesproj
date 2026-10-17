<script>
    require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/SalesOrderStockViewer.js?crc=')]);
</script>

<div id="popStockInfo" class="modal fade" tabindex="-1" data-backdrop="static">
    <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Stock Info</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                @*<nav class="navbar navbar-dark sticky-md-top bg-primary" style="padding:0px !important;">
                        <div class="row" style="margin:0px;">
                            <div class="col-2" style="padding:0px !important;">
                                <button data-role="none" id="btnStockInfo" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                                    <span class="bi-info-circle"></span>
                                    <div class="menu-description mt-1">Stock Info</div>
                                </button>
                            </div>
                            <div class="col-2" style="padding:0px !important;">
                                <button data-role="none" id="btnSalesPerson" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                                    <span class="bi-person-fill"></span>
                                    <div class="menu-description mt-1">Indicate Sales Rep</div>
                                </button>
                            </div>
                            <div class="col-2" style="padding:0px !important;">
                                <button data-role="none" id="btnSerialNumber" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                                    <span class="bi-upc"></span>
                                    <div class="menu-description mt-1">Serial Number</div>
                                </button>
                            </div>
                            <div class="col-2" style="padding:0px !important;">
                                <button data-role="none" id="btnInventoryBalances" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                                    <span class="bi-list-check"></span>
                                    <div class="menu-description mt-1">See Inventory Balances</div>
                                </button>
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
                    </nav>*@

                <table style="width:100%;">
                    <tr>
                        <td style="width: 120px; padding: 10px;">
                            <img src="../Content/images/box.png" id="imgStockInfo" style="width: 100px;" />
                        </td>
                        <td id="divItemDetailsStockNumberCell" style="padding:10px;">
                            <div style="font-size:1.50em;">
                                <div>
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

                <div id="divUnit" name1="divUnit" class="mb-3" style="display:none">
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
                                        <td><span name="spanUnit"></span>:<span name="spanUnitMeasureID" style="display:none;"></span></td>
                                        <td class="pl-3"><span name="spanPrice"></span></td>
                                    </tr>
                                    <tr>
                                        <td><span name="spanUnitQtyLabel"></span>:</td>
                                        <td class="pl-3"><span name="spanUnitQty"></span></td>
                                    </tr>
                                    <tr>
                                        <td>Price per <span name="spanBaseUnit"></span>:</td>
                                        <td class="pl-3"><span name="spanPricePerBase"></span></td>
                                    </tr>
                                    <tr>
                                        <td>Savings per <span name="spanBaseUnit"></span>:</td>
                                        <td class="pl-3"><span name="spanSavingsPerBase"></span></td>
                                    </tr>
                                    <tr name="divTotalSavings">
                                        <td>Savings per <span name="spanUnit"></span>:</td>
                                        <td class="pl-3"><span name="spanTotalSavings"></span></td>
                                    </tr>
                                    <tr name="divSrp" style="display:none;">
                                        <td>SRP:</td>
                                        <td class="pl-3"><span name="spanSrp"></span></td>
                                    </tr>
                                    <tr name="divSrp">
                                        <td>Edit Price:</td>
                                        <td class="pl-3"><span name="txtEditPrice"></span></td>
                                    </tr>
                                </table>
                            </div>
                            <div class="col-6 offset-md-2 col-md-4">
                                <div class="input-group mb-3">
                                    <button data-role="none" name="butUnitMinus" class="btn btn-danger" type="button" style="font-size:20px;"><span class="bi-dash-lg"></span></button>
                                    <input data-role="none" type="text" name="txtQtyTemplate" style="font-size:18px; text-align:center;" class="form-control" placeholder="Qty">
                                    <button data-role="none" name="butUnitPlus" class="btn btn-success" type="button" style="font-size:20px;"><span class="bi-plus-lg"></span></button>
                                </div>
                                @*<button data-role="none" name="butUnitUpdateCart" class="btn btn-outline-success btn-block" style="height:80px;"><span class="bi-plus-lg"></span> Add This SKU</button>*@
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
                        @*<div data-role="collapsible_clone">
                                <h4>
                                    Specify Picking Location(s) for This Item</h4>
                                Not yet implemented
                            </div>*@
                    </div>
                </div>

                <div id="divSubTotal" style="text-align: center; padding-bottom: 10px">
                    <h3>
                        Sub Total : PhP <span id="spanSubtotal">0.00</span>
                    </h3>
                </div>

                <div></div>

                <div id="divWarehouse">
                    
                    <div class="row align-items-center">
                        <div class="col-md-auto col-form-label">
                            <h5>Picking Location:</h5>
                        </div>
                        <div class="col-md-3">
                            <select id="selPckingLocationFOWST" data-role="none" class="form-select">
                                <option value="">Please select</option>
                            </select>
                        </div>
                    </div>
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
                    @*<div>
                            <input id="txtProductSerial" type="search" placeholder="Enter Serial Number" value="" />
                            <button id="butSerialBarCode">Scan Barcode</button>
                        </div>*@
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
                </div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col-md order-1 order-md-0">
                        <button data-role="none" type="button" class="btn btn-secondary btn-lg btn-block" data-dismiss="modal">Close</button>
                    </div>
                    <div class="col-md order-0 order-md-1">
                        <button data-role="none" type="button" name="butUnitUpdateCart" class="btn btn-success btn-lg btn-block">Add Product</button>
                    </div>
                </div>

            </div>
        </div>
    </div>
</div>