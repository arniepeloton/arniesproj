@Code
    ViewData("Title") = "Ecommerce"
    Layout = "~/Views/Shared/EcommerceLayout.vbhtml"
End Code
@Section head
    <script type="text/javascript">
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
                require(['/Scripts/sources/Ecommerce/Ecommerce.js?crc=' + assetversion], function () {
                    require(['/Scripts/sources/Ecommerce/Home.js?crc=' + assetversion])
                });
            })
        })
    </script>
End Section

<!-- ============================================== HEADER ============================================== -->
<!-- ============================================== HEADER : END ============================================== -->
<div class="body-content outer-top-xs" id="top-banner-and-menu">
    <div class="container">
        <div class="row">
            <!-- ============================================== SIDEBAR ============================================== -->
            <div class="col-xs-12 col-sm-12 col-md-3 sidebar">

                <!-- ================================== TOP NAVIGATION ================================== -->
                <div class="side-menu animate-dropdown outer-bottom-xs">
                    <div class="head bg-warning p-2"><i class="bi-justify"></i> Categories</div>
                    <nav class="yamm megamenu-horizontal">
                        <div id="listSidebarCategories" class="list-group list-group-flush border">
                            @*<a data-role="none" href="#" class="list-group-item list-group-item-light" aria-current="true">
                                The current link item
                            </a>*@
                        </div>
                    </nav>
                    <!-- /.megamenu-horizontal -->
                </div>
            </div>
            <!-- /.sidemenu-holder -->
            <!-- ============================================== SIDEBAR : END ============================================== -->
            <!-- ============================================== CONTENT ============================================== -->
            <div class="col-xs-12 col-sm-12 col-md-9 homebanner-holder">
                <div id="divProducts">
                    <ul id="listHomeBanner" class="bxslider"></ul>
                    <div id="divHomeAllProducts">
                        @*<div class="row row-cols-2 row-cols-sm-2 row-cols-md-4">
                                <div class="col mb-3">
                                    <div class="p-2 rounded bg-white">
                                        <img class="img-thumbnail rounded float-start mb-2" src="/Content/MobizImagePIM/CFE39DF6-8809-4973-9515-810DF75AED12/big/76e0d71b-9027-4dad-b3c0-ef10343dd757_big.jpg" />
                                        <div class="text-primary mb-2">Coke in Can</div>
                                        <div class="mb-2">P 150.00 per kg</div>
                                        <button data-role="none" class="btn btn-outline-success btn-block">Add to Cart</button>
                                    </div>
                                </div>
                                <div class="col mb-3">
                                    <div class="p-2 rounded bg-white">
                                        <img class="img-thumbnail rounded float-start mb-2" src="/Content/MobizImagePIM/CFE39DF6-8809-4973-9515-810DF75AED12/big/76e0d71b-9027-4dad-b3c0-ef10343dd757_big.jpg" />
                                        <div class="text-primary mb-2">Coke in Can</div>
                                        <div class="mb-2">P 150.00 per kg</div>
                                        <button data-role="none" class="btn btn-outline-success btn-block">Add to Cart</button>
                                    </div>
                                </div>
                                <div class="col mb-3">
                                    <div class="p-2 rounded bg-white">
                                        <img class="img-thumbnail rounded float-start mb-2" src="/Content/MobizImagePIM/CFE39DF6-8809-4973-9515-810DF75AED12/big/76e0d71b-9027-4dad-b3c0-ef10343dd757_big.jpg" />
                                        <div class="text-primary mb-2">Coke in Can</div>
                                        <div class="mb-2">P 150.00 per kg</div>
                                        <button data-role="none" class="btn btn-outline-success btn-block">Add to Cart</button>
                                    </div>
                                </div>
                                <div class="col mb-3">
                                    <div class="p-2 rounded bg-white">
                                        <img class="img-thumbnail rounded float-start mb-2" src="/Content/MobizImagePIM/CFE39DF6-8809-4973-9515-810DF75AED12/big/76e0d71b-9027-4dad-b3c0-ef10343dd757_big.jpg" />
                                        <div class="text-primary mb-2">Coke in Can</div>
                                        <div class="mb-2">P 150.00 per kg</div>
                                        <button data-role="none" class="btn btn-outline-success btn-block">Add to Cart</button>
                                    </div>
                                </div>
                            </div>*@
                    </div>
                </div>
            </div>
            <!-- /.homebanner-holder -->
            <!-- ============================================== CONTENT : END ============================================== -->
        </div>
        <!-- /.row -->
        <!-- ============================================== BRANDS CAROUSEL ============================================== -->
        <!-- /.logo-slider -->
        <!-- ============================================== BRANDS CAROUSEL : END ============================================== -->
    </div>
    <!-- /.container -->
</div>
<!-- /#top-banner-and-menu -->
<!-- ============================================================= FOOTER ============================================================= -->
<!-- ============================================================= FOOTER : END============================================================= -->
<!-- For demo purposes – can be removed on production -->
<!-- For demo purposes – can be removed on production : End -->
<!-- JavaScripts placed at the end of the document so the pages load faster -->
@*<script src="/Content/assets/js/jquery-1.11.1.min.js"></script>*@
@*<script src="/Content/assets/js/bootstrap.min.js"></script>
    <script src="/Content/assets/js/bootstrap-hover-dropdown.min.js"></script>
    <script src="/Content/assets/js/owl.carousel.min.js"></script>*@
@*<script src="/Content/assets/js/echo.min.js"></script>*@
@*<script src="/Content/assets/js/jquery.easing-1.3.min.js"></script>*@
@*<script src="/Content/assets/js/bootstrap-slider.min.js"></script>
    <script src="/Content/assets/js/jquery.rateit.min.js"></script>
    <script src="/Content/assets/js/lightbox.min.js"></script>
    <script src="/Content/assets/js/bootstrap-select.min.js"></script>
    <script src="/Content/assets/js/wow.min.js"></script>*@
@*<script src="/Content/assets/js/scripts.js"></script>*@