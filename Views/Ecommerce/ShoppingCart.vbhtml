@Code
    ViewData("Title") = "Ecommerce"
    Layout = "~/Views/Shared/EcommerceLayout.vbhtml"
End Code
@Section head
    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion, 'promise!/Scripts/sources/Ecommerce/ImageViewer.js?crc=' + assetversion], function () {
                require(['/Scripts/sources/Ecommerce/Ecommerce.js?crc=' + assetversion], function () {
                    require(['/Scripts/sources/Ecommerce/ShoppingCart.js?crc=' + assetversion])
                });
            })
        })      
    </script>
End Section
<div class="body-content outer-top-xs">
    <div class='container'>
        <h1 class="display-6 mb-4">Your cart</h1>
        <div class="shopping-cart bg-white border p-3 rounded">
            <div class="shopping-cart-table ">
                <div class="table-responsive">
                    <table class="table table-hover">
                        <thead class="table-light">
                            <tr class="text-center">
                                <th class="cart-romove item">Remove</th>
                                <th class="cart-description item">Image</th>
                                <th class="cart-vendor item">Vendor</th>
                                <th class="cart-product-name item">Product Name</th>
                                <th class="cart-qty item" style="width:150px;">Quantity</th>
                                <th class="cart-sub-total item">Price</th>
                                <th class="cart-total last-item">Subtotal</th>
                            </tr>
                        </thead><!-- /thead -->
                        @*<tfoot>
                <tr>
                    <td colspan="7">
                        <div class="shopping-cart-btn">
                            <span class="">
                                <a data-role="none" href="#" class="btn btn-upper btn-primary outer-left-xs btn-less-width">Continue Shopping</a>
                                <a data-role="none" href="#" class="btn btn-upper btn-primary pull-right outer-right-xs btn-less-width">Update shopping cart</a>
                            </span>
                        </div>
                    </td>
                </tr>
            </tfoot>*@
                        <tbody id="tbodyMyCart">
                            @*<tr>
                    <td class="romove-item"><a data-role="none" href="#" title="cancel" class="icon"><i class="fa fa-trash-o"></i></a></td>
                    <td class="cart-image">
                        <a data-role="none" class="entry-thumbnail" href="detail.html">
                            <img src="assets/images/products/p1.jpg" alt="">
                        </a>
                    </td>
                    <td class="cart-product-name-info">
                        <h4 class='cart-product-description'><a data-role="none" href="detail.html">Floral Print Buttoned</a></h4>
                        <div class="row">
                            <div class="col-sm-4">
                                <div class="rating rateit-small"></div>
                            </div>
                            <div class="col-sm-8">
                                <div class="reviews">
                                    (06 Reviews)
                                </div>
                            </div>
                        </div>
                        <div class="cart-product-info">
                            <span class="product-color">COLOR:<span>Blue</span></span>
                        </div>
                    </td>
                    <td class="cart-product-edit"><a data-role="none" href="#" class="product-edit">Edit</a></td>
                    <td class="cart-product-quantity">
                        <div class="quant-input">
                            <div class="arrows">
                                <div class="arrow plus gradient"><span class="ir"><i class="icon fa fa-sort-asc"></i></span></div>
                                <div class="arrow minus gradient"><span class="ir"><i class="icon fa fa-sort-desc"></i></span></div>
                            </div>
                            <input data-role="none" type="text" value="1">
                        </div>
                    </td>
                    <td class="cart-product-sub-total"><span class="cart-sub-total-price">$300.00</span></td>
                    <td class="cart-product-grand-total"><span class="cart-grand-total-price">$300.00</span></td>
                </tr>*@
                        </tbody><!-- /tbody -->
                    </table><!-- /table -->
                </div>
            </div><!-- /.shopping-cart-table -->
            <div class="offset-md-8 col-md-4 col-sm-12 cart-shopping-total">
                <table class="table">
                    <thead>
                        <tr>
                            <th>
                                @*<div class="cart-sub-total">
                                    Subtotal<span class="inner-left-md">$600.00</span>
                                </div>*@
                                <div class="cart-grand-total text-right">
                                    Grand Total: <span id="lblCartGrandTotal" class="inner-left-md"></span>
                                </div>
                            </th>
                        </tr>
                    </thead><!-- /thead -->
                    <tbody>
                        <tr>
                            <td>
                                <div class="cart-checkout-btn text-center pt-3">
                                    <button data-role="none" id="btnMyCartCheckout" class="btn btn-warning checkout-btn btn-lg btn-block mb-3">PROCEED TO CHEKOUT</button>
                                    <button id="btnShoppingCartContinueShopping" data-role="none" class="btn btn-success btn-block btn-lg">CONTINUE SHOPPING</button>
                                </div>
                            </td>
                        </tr>
                    </tbody><!-- /tbody -->
                </table><!-- /table -->
            </div><!-- /.cart-shopping-total -->
        </div>
    </div>
</div>