@Code
    ViewData("Title") = "Ecommerce"
    Layout = "~/Views/Shared/EcommerceLayout.vbhtml"
End Code
@Section head
    <script type="text/javascript">
        var myaccountdetails = JSON.parse(@Html.Raw(Json.Encode(ViewData("MyAccountDetails"))));
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion, 'promise!/Scripts/sources/Ecommerce/ImageViewer.js?crc=' + assetversion], function () {
                require(['/Scripts/sources/Ecommerce/Ecommerce.js?crc=' + assetversion], function () {
                    require(['/Scripts/sources/Ecommerce/Checkout.js?crc=' + assetversion])
                });
            })
        })
    </script>

    <style type="text/css">
        .btnCheckout {
            padding: 20px;
            font-size: 16px;
            text-align: center;
            border: 1px solid #CCC;
            border-radius: 5px;
            height: 100px;
            line-height: 3;
            color: #0083c1;
            text-shadow:none;
        }

        .btnCheckout:hover {
            border: 1px solid #0083c1;
            background-color: #0083c1;
            color: #FFF;
            cursor:pointer;
        }
    </style>
End Section
<div class="body-content outer-top-xs" id="top-banner-and-menu">
    <div class="container">
        <div class="bg-white border p-3 rounded">
            <h1 class="display-6 mb-4">Checkout</h1>

            <div class="alert alert-danger" id="divCheckoutMessage" role="alert" style="font-size:18px; text-align:center; display:none;">
                Due to current limitations, we are only accepting pickup orders paid via COD at our Sta. Ana store.

                <br /><br />Please present your order number to the guard on duty upon visiting the store.

                <br /><br />Thank you very much and stay safe.
            </div>

            <div class="mb-3">
                <h4>Your Order:</h4>

                <div class="table-responsive">
                    <table class="table" style="width:100%;">
                        <thead class="table-light">
                            <tr class="text-center">
                                <th></th>
                                <th id="thCheckoutVendor" style="width:100px;">Vendor</th>
                                <th>Product Name</th>
                                <th>Order Qty</th>
                                <th>Price</th>
                                <th>Line Total</th>
                            </tr>
                        </thead>
                        <tbody id="tbodyCheckoutCart"></tbody>
                    </table>
                </div>                
            </div>
            
            <div class="row">
                <div class="col-3 mb-3">
                    <h4>Comments:</h4>
                </div>
                <div class="col mb-3">
                    <textarea id="txtCheckoutComments" class="form-control unicase-form-control text-input" style="resize:none; height:100px; width:100%;" placeholder="Please write special instructions here, if you have any."></textarea>
                </div>

                <div class="w-100"></div>

                <div class="col-3 mb-3">
                    <h4>Delivery Method:</h4>
                </div>
                <div class="col mb-3">
                    <div class="form-check">
                        <input data-role="none" type="radio" class="form-check-input" id="radDMDelivery" name="radDeliveryMethod" disabled value="delivery" />
                        <label data-role="none" class="form-check-label" for="radDMDelivery">Delivery</label>
                        <div>
                            <div class="form-check">
                                <input data-role="none" type="radio" class="form-check-input" id="radDeliveryStandard" disabled name="radDeliveryType" value="pickup" />
                                <label data-role="none" class="form-check-label" for="radDeliveryStandard">Standard (In-House Delivery)</label>
                            </div>
                            <div class="form-check">
                                <input data-role="none" type="radio" class="form-check-input" id="radDeliveryThirdParty" disabled name="radDeliveryType" value="pickup" />
                                <label data-role="none" class="form-check-label" for="radDeliveryThirdParty">3rd Party Logistics (3PL)</label>
                            </div>
                        </div>
                    </div>

                    <div id="divStorePickup" class="mb-3" style="display:none;">
                        <div class="form-check">
                            <input data-role="none" type="radio" class="form-check-input" id="radDMStorePickup" name="radDeliveryMethod" checked value="Pick Up by Customer" />
                            <label data-role="none" class="form-check-label" for="radDMStorePickup">Store Pick Up</label>
                            <div>
                                The Paper Tree Sta. Ana
                                <br />
                                The Paper Tree Building, Sta. Ana Avenue, Davao City 8000, Philippines
                                <br />
                                Tel: 082.227-4313 up to 15 / Globe: +63917.719.7888 / Smart: +63999.885.6502
                            </div>
                        </div>
                    </div>
                </div>

                <div class="w-100"></div>

                <div class="col-3 mb-3">
                    <h4>Payment Method:</h4>
                </div>
                <div class="col mb-3">
                    <div class="form-check mb-3">
                        <input data-role="none" class="form-check-input" type="radio" id="radPMCOD" name="radPaymentMethod" checked value="cod" />
                        <label data-role="none" class="form-check-label" for="radPMCOD">Cash on Delivery or Pick Up (COD)</label>
                    </div>

                    <div class="form-check mb-3" id="divPMPaypal" style="display:none;">
                        <input data-role="none" class="form-check-input" type="radio" id="radPMPaypal" name="radPaymentMethod" value="paypal" />
                        <label data-role="none" class="form-check-label" for="radPMPaypal">PayPal</label>
                    </div>
                </div>
            </div>

            <div class="row mt-4">
                <div class="offset-sm-3 col-sm-3">
                    <div id="butContinueShopping" class="btnCheckout">
                        CONTINUE SHOPPING
                    </div>
                </div>
                <div class="col-sm-3">
                    <div id="butCheckouContinue" class="btnCheckout" style="background-color:#14c332; color: #FFF; border:none;">
                        CONFIRM ORDER
                    </div>
                </div>
            </div>
            <br />
            <div id="divCheckoutError" style="display:none;" class="alert alert-danger" role="alert"><strong>Oops!</strong> Please fill in the required fields.</div>
        </div>
        <div class="checkout-box" style="display:none;">
            <div class="row">
                <div class="col-md-8">
                    <div class="panel-group checkout-steps" id="accordion">
                        <!-- checkout-step-01  -->
                        <div class="panel panel-default checkout-step-01">

                            <!-- panel-heading -->
                            <div class="panel-heading">
                                <h4 class="unicase-checkout-title">
                                    <a data-role="none" data-toggle="collapse" class="" data-parent="#accordion" href="#collapseOne">
                                        <span>1</span>Checkout Method
                                    </a>
                                </h4>
                            </div>
                            <!-- panel-heading -->

                            <div id="collapseOne" class="panel-collapse collapse in">

                                <!-- panel-body  -->
                                <div class="panel-body">
                                    <div class="row">

                                        <!-- guest-login -->
                                        <div class="col-md-6 col-sm-6 guest-login">
                                            <h4 class="checkout-subtitle">Guest or Register Login</h4>
                                            <p class="text title-tag-line">Register with us for future convenience:</p>

                                            <!-- radio-form  -->
                                            <form class="register-form" role="form">
                                                <div class="radio radio-checkout-unicase">
                                                    <input data-role="none" id="guest" type="radio" name="text" value="guest" checked>
                                                    <label class="radio-button guest-check" for="guest">Checkout as Guest</label>
                                                    <br>
                                                    <input data-role="none" id="register" type="radio" name="text" value="register">
                                                    <label class="radio-button" for="register">Register</label>
                                                </div>
                                            </form>
                                            <!-- radio-form  -->

                                            <h4 class="checkout-subtitle outer-top-vs">Register and save time</h4>
                                            <p class="text title-tag-line ">Register with us for future convenience:</p>

                                            <ul class="text instruction inner-bottom-30">
                                                <li class="save-time-reg">- Fast and easy check out</li>
                                                <li>- Easy access to your order history and status</li>
                                            </ul>

                                            <button data-role="none" type="submit" class="btn-upper btn btn-primary checkout-page-button checkout-continue btn-less-width">Continue</button>
                                        </div>
                                        <!-- guest-login -->
                                        <!-- already-registered-login -->
                                        <div class="col-md-6 col-sm-6 already-registered-login">
                                            <h4 class="checkout-subtitle">Already registered?</h4>
                                            <p class="text title-tag-line">Please log in below:</p>
                                            <form class="register-form" role="form">
                                                <div class="form-group">
                                                    <label class="info-title" for="exampleInputEmail1">Email Address <span>*</span></label>
                                                    <input data-role="none" type="email" class="form-control unicase-form-control text-input" id="exampleInputEmail1" placeholder="">
                                                </div>
                                                <div class="form-group">
                                                    <label class="info-title" for="exampleInputPassword1">Password <span>*</span></label>
                                                    <input data-role="none" type="password" class="form-control unicase-form-control text-input" id="exampleInputPassword1" placeholder="">
                                                    <a data-role="none" href="#" class="forgot-password">Forgot your Password?</a>
                                                </div>
                                                <button data-role="none" type="submit" class="btn-upper btn btn-primary checkout-page-button btn-less-width">Login</button>
                                            </form>
                                        </div>
                                        <!-- already-registered-login -->

                                    </div>
                                </div>
                                <!-- panel-body  -->

                            </div><!-- row -->
                        </div>
                        <!-- checkout-step-01  -->
                        <!-- checkout-step-02  -->
                        <div class="panel panel-default checkout-step-02">
                            <div class="panel-heading">
                                <h4 class="unicase-checkout-title">
                                    <a data-role="none" data-toggle="collapse" class="collapsed" data-parent="#accordion" href="#collapseTwo">
                                        <span>2</span>Billing Information
                                    </a>
                                </h4>
                            </div>
                            <div id="collapseTwo" class="panel-collapse collapse">
                                <div class="panel-body">
                                    Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
                                </div>
                            </div>
                        </div>
                        <!-- checkout-step-02  -->
                        <!-- checkout-step-03  -->
                        <div class="panel panel-default checkout-step-03">
                            <div class="panel-heading">
                                <h4 class="unicase-checkout-title">
                                    <a data-role="none" data-toggle="collapse" class="collapsed" data-parent="#accordion" href="#collapseThree">
                                        <span>3</span>Shipping Information
                                    </a>
                                </h4>
                            </div>
                            <div id="collapseThree" class="panel-collapse collapse">
                                <div class="panel-body">
                                    Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
                                </div>
                            </div>
                        </div>
                        <!-- checkout-step-03  -->
                        <!-- checkout-step-04  -->
                        <div class="panel panel-default checkout-step-04">
                            <div class="panel-heading">
                                <h4 class="unicase-checkout-title">
                                    <a data-role="none" data-toggle="collapse" class="collapsed" data-parent="#accordion" href="#collapseFour">
                                        <span>4</span>Shipping Method
                                    </a>
                                </h4>
                            </div>
                            <div id="collapseFour" class="panel-collapse collapse">
                                <div class="panel-body">
                                    Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
                                </div>
                            </div>
                        </div>
                        <!-- checkout-step-04  -->
                        <!-- checkout-step-05  -->
                        <div class="panel panel-default checkout-step-05">
                            <div class="panel-heading">
                                <h4 class="unicase-checkout-title">
                                    <a data-role="none" data-toggle="collapse" class="collapsed" data-parent="#accordion" href="#collapseFive">
                                        <span>5</span>Payment Information
                                    </a>
                                </h4>
                            </div>
                            <div id="collapseFive" class="panel-collapse collapse">
                                <div class="panel-body">
                                    Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
                                </div>
                            </div>
                        </div>
                        <!-- checkout-step-05  -->
                        <!-- checkout-step-06  -->
                        <div class="panel panel-default checkout-step-06">
                            <div class="panel-heading">
                                <h4 class="unicase-checkout-title">
                                    <a data-role="none" data-toggle="collapse" class="collapsed" data-parent="#accordion" href="#collapseSix">
                                        <span>6</span>Order Review
                                    </a>
                                </h4>
                            </div>
                            <div id="collapseSix" class="panel-collapse collapse">
                                <div class="panel-body">
                                    Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
                                </div>
                            </div>
                        </div>
                        <!-- checkout-step-06  -->

                    </div><!-- /.checkout-steps -->
                </div>
                <div class="col-md-4">
                    <!-- checkout-progress-sidebar -->
                    <div class="checkout-progress-sidebar ">
                        <div class="panel-group">
                            <div class="panel panel-default">
                                <div class="panel-heading">
                                    <h4 class="unicase-checkout-title">Your Checkout Progress</h4>
                                </div>
                                <div class="">
                                    <ul class="nav nav-checkout-progress list-unstyled">
                                        <li><a data-role="none" href="#">Billing Address</a></li>
                                        <li><a data-role="none" href="#">Shipping Address</a></li>
                                        <li><a data-role="none" href="#">Shipping Method</a></li>
                                        <li><a data-role="none" href="#">Payment Method</a></li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                    <!-- checkout-progress-sidebar -->
                </div>
            </div><!-- /.row -->
        </div>
    </div>
</div>

<div class="modal fade" id="popupNewAddress" tabindex="-1" role="dialog" aria-labelledby="mySmallModalLabel">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                <h4 data-role="none" class="modal-title">New Address</h4>
            </div>
            <div class="modal-body">
                <div class="form-group">
                    <label for="txtCompleteAddress">Enter complete address: <span class="required">*</span></label>
                    <input data-role="none" autofocus type="text" required class="form-control unicase-form-control text-input" id="txtCompleteAddress">
                </div>
            </div>
            <div class="modal-footer">
                <button data-role="none" id="btnAddressCancel" type="button" class="btn btn-default btn-less-width" data-dismiss="modal">Cancel</button>
                <button data-role="none" id="btnAddressSave" type="submit" class="btn btn-primary btn-less-width">Save</button>
            </div>
        </div>
    </div>
</div>