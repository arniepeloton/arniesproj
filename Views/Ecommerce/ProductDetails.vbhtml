@Code
    ViewData("Title") = "Ecommerce"
    Layout = "~/Views/Shared/EcommerceLayout.vbhtml"
End Code
@Section head
    <script type="text/javascript">
        //sessionStorage.ProductDetails = @Html.Raw(Json.Encode(ViewData("ProductDetails")));
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion, 'promise!/Scripts/sources/Ecommerce/ImageViewer.js?crc=' + assetversion], function () {
                require(['promise!/Scripts/sources/Ecommerce/Ecommerce.js?crc=' + assetversion], function () {
                    require(['/Scripts/sources/Ecommerce/ProductDetails.js?crc=' + assetversion])
                });
            })
        })        
</script>
End Section
<div class="body-content outer-top-xs" id="top-banner-and-menu">
    <div class='container bg-white border p-3 rounded'>
        <div class='row single-product'>
            <div class='col-md-1 sidebar'>
            </div><!-- /.sidebar -->
            <div class='col-md-9'>
                <div class="detail-block">
                    <div class="row  wow fadeInUp">
                        <div class="col-xs-12 col-sm-6 col-md-5 gallery-holder">
                            <div class="product-item-holder size-big single-product-gallery small-gallery">
                                <div id="owl-single-product" class="owl-carousel owl-theme" style="display:block;">
                                    <div class="owl-wrapper-outer">
                                        <div id="singleProductPic" class="bxslider">

                                        </div>
                                    </div>
                                </div><!-- /.single-product-slider -->

                                <div class="single-product-gallery-thumbs gallery-thumbs" style="display:none;">
                                    <div id="owl-single-product-thumbnails" class="owl-carousel owl-theme" style="display:block;">
                                        <div class="owl-wrapper-outer">
                                            <div id="singleProductPicThumbnails" class="owl-wrapper" style="display:block;"></div>
                                        </div>
                                    </div><!-- /#owl-single-product-thumbnails -->
                                </div><!-- /.gallery-thumbs -->
                            </div><!-- /.single-product-gallery -->
                        </div><!-- /.gallery-holder -->
                        <div class='col-sm-6 col-md-7 product-info-block'>
                            <div class="product-info">
                                <h1 class="name" id="lblProductName"></h1>

                                <div class="rating-reviews m-t-20" style="display:none;">
                                    <div class="row">
                                        <div class="col-sm-3">
                                            <div class="rating rateit-small"></div>
                                        </div>
                                        <div class="col-sm-8">
                                            <div class="reviews">
                                                <a data-role="none" href="#" class="lnk">(13 Reviews)</a>
                                            </div>
                                        </div>
                                    </div><!-- /.row -->
                                </div><!-- /.rating-reviews -->

                                <div class="stock-container info-container m-t-10" style="display:none;">
                                    <div class="row">
                                        <div class="col-sm-2">
                                            <div class="stock-box">
                                                <span class="label">Availability :</span>
                                            </div>
                                        </div>
                                        <div class="col-sm-9">
                                            <div class="stock-box">
                                                <span class="value">In Stock</span>
                                            </div>
                                        </div>
                                    </div><!-- /.row -->
                                </div><!-- /.stock-container -->
                                @*<div class="description-container m-t-20">
                                        Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
                                    </div>*@<!-- /.description-container -->

                                <div id="divForCompanyProduct" style="display:none;">
                                    <div class="price-container info-container m-t-20">
                                        <div class="row">
                                            <div class="col-sm-12">
                                                <div class="price-box">
                                                    <span class="price" id="lblProductPrice"></span>
                                                    @*<span class="price-strike">$900.00</span>*@
                                                </div>
                                            </div>
                                        </div><!-- /.row -->
                                    </div><!-- /.price-container -->

                                    <div class="quantity-container info-container">
                                        <div id="divUpchargeNote"></div>
                                        <br />
                                        <div class="row">

                                            @*<div class="col-sm-2">
                                                    <span class="label">Qty :</span>
                                                </div>*@

                                            <div class="col-auto">
                                                Quantity:
                                            </div>

                                            <div class="col">
                                                <div id="divQtyStyleOption" style="position:relative;">

                                                </div>
                                                @*<input data-role="none" type="number" autocomplete="off" class="form-control unicase-form-control text-input" id="txtCompanyAddressCity" placeholder="Search City Address">
                                                    <div id="listCities" class="list-group" style="display:block; position:absolute; max-width: 100%; box-shadow: 0px 5px 28px #CCC; overflow-y:auto; max-height:130px; left:0; right:0;">
                                                        <a data-role="none" href="#" class="list-group-item">0.25</a>
                                                        <a data-role="none" href="#" class="list-group-item">0.25</a>
                                                        <a data-role="none" href="#" class="list-group-item">0.25</a>
                                                        <a data-role="none" href="#" class="list-group-item">0.25</a>
                                                    </div>*@
                                                @*<select data-role="none" class="form-control unicase-form-control" style="width:100%;">
                                                        <option>1</option>
                                                        <option>2</option>
                                                        <option>3</option>
                                                    </select>*@
                                                @*<div style="display:inline-flex;">
                                                        <button data-role="none" style="width:50px; height:45px;" class="btn custom-btn-theme-c btn-less-width btn-sm btnDecreaseQty" data-toggle="tooltip" data-placement="bottom" title="" data-original-title="Previous">
                                                            <span class="glyphicon glyphicon-minus"></span>
                                                        </button>
                                                        <input data-role="none" id="txtProductQty" style="width:80px; height:45px;" type="text" value="1" class="form-control unicase-form-control txtQty">
                                                        <button data-role="none" style="width:50px; height:45px;" class="btn custom-btn-theme-c btn-less-width btn-sm btnIncreaseQty" data-toggle="tooltip" data-placement="bottom" title="" data-original-title="Next">
                                                            <span class="glyphicon glyphicon-plus"></span>
                                                        </button>
                                                    </div>*@
                                            </div>

                                            <div class="col-auto">
                                                <button data-role="none" id="btnAddToCart" class="btn btn-primary btn-less-width btn-lg"><i class="fa fa-shopping-cart inner-right-vs"></i> ADD TO CART</button>
                                            </div>
                                        </div><!-- /.row -->
                                    </div><!-- /.quantity-container -->
                                </div>
                            </div><!-- /.product-info -->
                        </div><!-- /.col-sm-7 -->
                    </div><!-- /.row -->
                </div>

                <div class="product-tabs inner-bottom-xs wow fadeInUp mt-4">
                    <h4>Product Description</h4>
                    <p id="pMarketingText" class="text"></p>
                </div><!-- /.product-tabs -->
                <!-- ============================================== UPSELL PRODUCTS : END ============================================== -->

            </div><!-- /.col -->
            <div class="clearfix"></div>
        </div><!-- /.row -->
        <div id="divForVendorProduct" class="mt-4" style="display:none;">
            <div>
                Choose the best vendor for you:
            </div>
            <br />
            <div class="table-responsive">
                <table data-role="none" style="width:100%" class="table table-light table-striped table-hover headerfitwidth">
                    <thead>
                        <tr>
                            <th class="item"></th>
                            <th class="item">Vendor Code</th>
                            <th class="item" id="tdVendorName">Vendor Name</th>
                            <th class="item txt-center">Price</th>
                            <th class="item txt-center">Unit Measure</th>
                            <th class="item txt-center" style="width:150px;">Order Quantity</th>
                            <th class="item txt-center">Line Total</th>
                        </tr>
                    </thead>
                    <tbody id="tbodySkuVendor"></tbody><!-- /tbody -->
                </table><!-- /table -->
            </div>
            <br />
            <div class="text-right">
                <button data-role="none" id="btnVendorAddtoCart" class="btn btn-success btn-less-width" style="height:70px !important; width:200px; background-color:#14c332;"><i class="fa fa-shopping-cart inner-right-vs"></i> ADD TO CART</button>
            </div>
        </div>
    </div>
</div>