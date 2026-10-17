﻿<!DOCTYPE html>
<html>
<head>
    <title>@ViewData("Title")</title>
    @*<meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=no,maximum-scale=1">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <link rel="stylesheet" href="/Scripts3/others/themes/default/jquery.mobile.css" />
    <link rel="stylesheet" href="/Content/css/jQueryMobileOverrides.css" />
    <script type="text/javascript" src="/Scripts/jquery.js"> </script>
    <script type="text/javascript" src="/Scripts3/others/jquery.mobile.js"> </script>
    <script src="/Scripts3/others/require.js" data-main="/Scripts3/common/init.js"></script>*@
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=no,maximum-scale=1">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <script src="/Scripts3/others/require.js"></script>
    <script>
        if ((location.hostname == 'localhost')
            || (window.Cypress != null)) {

            window.assetversion = '';
        } else {
            window.assetversion = new Date().getTime();
        }

        window.IsBootstrap = true;
        window.IsEcommerce = true;

        function GetWindowPathFormat(vString) {
            //            alert(vString + '. Please use GetPathFormat');
            // change GetWindowPathFormat to GetPathFormat
            // debugger;
            console.log('change GetWindowPathFormat to GetPathFormat: ' + vString);
            return GetPathFormat(vString);
        };

        function GetPathFormat(vString) {

            if (window.location.protocol.substring(0, 4) != 'file')
                return vString;

            function parseUrl(url) {
                // http://stackoverflow.com/a/6644749/334045
                var a = document.createElement('a');
                a.href = url;
                return a;
            }; // function parseUrl

            function GetExtension(vPathName) {

                var pMatch = vPathName.match(/.+\.([\w]+)$/im);
                if (pMatch != null)
                    return pMatch[1];
                else
                    return '';

            }; // function GetExtension

            var pCurrent = window.location.pathname.toLowerCase();
            var pIndex = pCurrent.indexOf('www/') + 4;
            var pRoot = pCurrent.substring(0, pIndex); // path to www
            var pIndex = pCurrent.lastIndexOf('/') + 1; // to folder

            if (vString.substring(0, 1) == '/')
                vString = vString.substring(1); // remove leading slash

            var pParsed = parseUrl(vString);
            var pOut = pRoot + pParsed.pathname.substring(pIndex);

            if (GetExtension(pOut) == '') {
                pOut += '.html';
                pOut += pParsed.search + pParsed.hash;
                return pOut.toLowerCase();
            } else {
                pOut += pParsed.search + pParsed.hash;
                return pOut;
            };
        }; // function GetPathFormat

    </script>
    <link rel="stylesheet" href="/Content/Ecommerce/css/Ecommerce.css">

    @*<link rel="stylesheet" href="/Content/flatui-css-custom.css">
    <link rel="stylesheet" href="/Content/Ecommerce/css/bootstrap.min.css">*@

    <link rel="stylesheet" href="/Content/bootstrap-5.0.0/css/bootstrap.css">
    <link rel="stylesheet" href="/Content/bootstrap-5.0.0/bootstrapfive-icons.css">
    <link rel="stylesheet" href="/Content/bootstrap-5.0.0/css/bootstrapfive-custom.css">
    <link rel="stylesheet" href="/Content/metro-calendar.css">

    @*<link rel="stylesheet" href="/Content/Ecommerce/css/main.css">
    <link rel="stylesheet" href="/Content/Ecommerce/css/blue.css">
    <link rel="stylesheet" href="/Content/Ecommerce/css/owl.carousel.css">
    <link rel="stylesheet" href="/Content/Ecommerce/css/owl.transitions.css">
    <link rel="stylesheet" href="/Content/Ecommerce/css/animate.min.css">
    <link rel="stylesheet" href="/Content/Ecommerce/css/rateit.css">
    <link rel="stylesheet" href="/Content/Ecommerce/css/bootstrap-select.min.css">*@
    <link rel="stylesheet" href="/Content/Ecommerce/css/lightbox.css">

    <link rel="stylesheet" href="/Content/Ecommerce/css/font-awesome.css">
    <link rel="stylesheet" href="/Content/css/jquery.bxslider.css">
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
    <link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />
    <link rel="Stylesheet" type="text/css" href="/Content/css/jquery.bxslider.css" />
    <script src="/Scripts3/common/init.js?crc="></script>       
    
    <script type="text/javascript">
        function logoerror(dis) {
            var tradename = CStr2($(dis).attr('alt'));

            $(dis).parent().text(tradename)

            $('head')
                .prepend('<title>'+ tradename +' Online Store</title>')
        }

        function logoload(dis) {
            var tradename = CStr2($(dis).attr('alt'));

            $.ajax({
                url: '/Ecommerce/GetBanner?OrgID=' + sessionStorage.OrgID,
                success: function (e) {
                    var vRows = JSON.parse(e).d.Results.Rows;

                    if (vRows.length > 0) {
                        var banner = vRows[0].FileName;
                        var description = 'Shop and Order now!';
                        var title = tradename + ' Online Store';
                        var url = location.href;

                        $('head')
                        .prepend('<meta property="og:type" content="website" />')
                        .prepend('<meta property="og:url" content="' + url + '" />')
                        .prepend('<meta property="og:title" content="' + title + '" />')
                        .prepend('<meta property="og:description" content="' + description + '" />')
                        .prepend('<meta property="og:image" content="' + location.origin + '/Content/Ecommerce/' + sessionStorage.OrgID.toLowerCase() + '/' + banner + '" />')
                        .prepend('<title>' + title + '</title>')
                    }
                }
            })
        }
        //require(['/Content/Ecommerce/js/lightbox-2.11.1.js', GetPathFormat('/Scripts/sources/Ecommerce/Ecommerce.js?crc=' + assetversion),
        //    '/Scripts/sources/Ecommerce/ImageViewer.js?crc=' + assetversion]);
    </script>

    <style>
        .ui-loader {
            display: none !important;
        }

        .ui-page-active{
            outline:none;
            background-color: #f8f9fa !important;
        }

        [data-role="popup"]{
            display: none !important;
        }
    </style>

    @RenderSection("head", False)
</head>
<body class="cnt-home" data-role="none">
    <header class="header-style-1">

        <!-- ============================================== TOP MENU ============================================== -->
        <div class="top-bar animate-dropdown">
            <div class="container text-right">
                <div id="btnUserOptions" class="dropdown" style="display:none;">
                    <a data-role="none" class="btn dropdown-toggle" href="#" role="button" id="dropdownMenuLink" data-toggle="dropdown" aria-expanded="false">
                        <span class="value" id="lblUser"></span>
                    </a>
                    <ul class="dropdown-menu dropdown-menu-right" aria-labelledby="dropdownMenuLink">
                        <li><a id="btndropdownMyAccount" class="dropdown-item" data-role="none" href="#">My Account</a></li>
                        <li><a id="btndropdownAddressBook" class="dropdown-item" data-role="none" href="#">Address Book</a></li>
                        <li><a id="btndropdownOrderHistory" class="dropdown-item" data-role="none" href="#">Order History</a></li>
                        <li><a id="btndropdownAccountsPayable" class="dropdown-item" data-role="none" href="#">Accounts Payable</a></li>
                        <li><a id="btnUserLogout" class="dropdown-item" data-role="none" href="#">Logout</a></li>
                    </ul>
                </div>
                <button data-role="none" class="btn" id="btnEcommerceLogin"><i class="bi-lock-fill"></i>Login</button>
                <!-- /.header-top-inner -->
            </div>
            <!-- /.container -->
        </div>
        <!-- /.header-top -->
        <!-- ============================================== TOP MENU : END ============================================== -->
        <div class="main-header">
            <div class="container">
                <div class="row">
                    <div class="col-sm-3 order-0 mb-3">
                        <!-- ============================================================= LOGO ============================================================= -->
                        <div class="logo"> <a data-role="none" id="btnOrgLogo" style="font-size:20px;"> <img id="imgOrgLogo"> </a> </div>
                        <!-- /.logo -->
                        <!-- ============================================================= LOGO : END ============================================================= -->
                    </div>
                    <!-- /.logo-holder -->

                    <div class="col order-2 order-sm-1 mb-3">
                        <div class="input-group mb-3">
                            <input data-role="none" class="form-control form-control-lg" id="txtHeaderSearchField" autocomplete="off" placeholder="Search item here..." />
                            <button data-role="none" class="btn btn-primary"><span class="bi-search"></span></button>
                        </div>
                    </div>
                    <!-- /.top-search-holder -->

                    <div class="col-sm-3 animate-dropdown text-right order-1 order-sm-2 mb-3">
                        <!-- ============================================================= SHOPPING CART DROPDOWN ============================================================= -->
                        <div class="dropdown">
                            <button data-role="none" class="btn btn-outline-primary dropdown-toggle btn-lg" type="button" id="btnCartDropdown" data-toggle="dropdown" aria-expanded="false">
                                <i class="bi-cart4"></i>
                                <span class="basket-item-count"><span class="count lblCountItems">0</span></span>
                                <span class="total-price-basket"> <b class="lbl">CART -</b> <span class="total-price"> <span class="sign">P</span> <span class="value" id="lblCartDropdownTotal">0.00</span> </span> </span>
                            </button>
                            <ul class="dropdown-menu dropdown-menu-right" id="listCartDropdown" style="max-height:200px; overflow-x:hidden; overflow-y:auto;" aria-labelledby="btnCartDropdown">
                                @*<li>
                                    <div class="cart-item product-summary p-2">
                                        <div class="row">
                                            <div class="col-3">
                                                <div class="image"> <a data-role="none" href="detail.html"><img src="/Content/Ecommerce/images/cart.jpg" class="img-thumbnail" alt=""></a> </div>
                                            </div>
                                            <div class="col-auto">
                                                <a data-role="none" href="#">Simple Product</a>
                                                <div class="price">$600.00</div>
                                            </div>
                                            <div class="col-2">
                                                <button data-role="none" class="btn text-danger">
                                                    <i class="bi-trash"></i>
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                    <!-- /.cart-item -->
                                    <div class="clearfix"></div>
                                    
                                    <!-- /.cart-total-->

                                </li>
                                <li><hr class="dropdown-divider"></li>
                                <li>
                                    <div class="clearfix cart-total p-2">
                                        <div class="pull-right"> <span class="text">Sub Total :</span><span class='price'>$600.00</span> </div>
                                        <div class="clearfix"></div>
                                        <a data-role="none" href="checkout.html" class="btn btn-upper btn-primary btn-block m-t-20">Checkout</a>
                                    </div>
                                </li>*@
                            </ul>
                        </div>
                    </div>
                    <!-- /.top-cart-row -->
                </div>
                <!-- /.row -->

            </div>
            <!-- /.container -->
        </div>
        <!-- /.main-header -->
        <!-- ============================================== NAVBAR ============================================== -->
        <nav class="navbar navbar-expand-lg navbar-primary bg-primary">
            <div class="container">
                @*<a class="navbar-brand" href="#">Navbar</a>*@
                <button data-role="none" class="btn btn-outline-primary navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
                    <span class="bi-justify"></span>
                </button>
                <div class="collapse navbar-collapse" id="navbarSupportedContent">
                    <ul class="navbar-nav me-auto mb-2 mb-lg-0" id="listMainNavBarMenu">
                        @*<li class="nav-item mr-2">
                            <button data-role="none" class="btn btn-primary active" aria-current="page" href="#">Home</button>
                        </li>
                        <li class="nav-item">
                            <button data-role="none" class="btn btn-primary" aria-current="page" href="#">Home</button>
                        </li>*@
                    </ul>
                </div>
            </div>
        </nav>
    </header>

    <div style="margin-bottom:80px;">
        @RenderBody()
    </div>

    <div class="divfloatingcart" style="display:none; position:fixed; bottom:25px; right:25px;">
        <button data-role="none" data-toggle="tooltip" data-placement="left" title="" class="btn btn-success rounded-circle floatingcart" data-original-title="View Cart" style="height:60px !important; width:60px !important; font-size:25px !important;">
            <i class="bi-cart4"></i>
        </button>
        <div class="lblCountItems basket-item-count" style="position: absolute; right: 0px; top:-10px;">0</div>
    </div>

    @*<footer class="footer color-bg">*@
    <div class="copyright-bar text-center mb-5" style="width:100%; bottom:0; z-index: 1; background: none; color: inherit;">
        <div class="container">
            Powered by 1Mobiz Inc.
            <a data-role="none" class="custom-link" href="http://mobiz-apps.com/">www.mobiz-apps.com</a>
            @*<div class="col-xs-12 col-sm-6 no-padding social">
                    <ul class="link">
                        <li class="fb pull-left"><a data-role="none" target="_blank" rel="nofollow" href="#" title="Facebook"></a></li>
                        <li class="tw pull-left"><a data-role="none" target="_blank" rel="nofollow" href="#" title="Twitter"></a></li>
                        <li class="googleplus pull-left"><a data-role="none" target="_blank" rel="nofollow" href="#" title="GooglePlus"></a></li>
                        <li class="rss pull-left"><a data-role="none" target="_blank" rel="nofollow" href="#" title="RSS"></a></li>
                        <li class="pintrest pull-left"><a data-role="none" target="_blank" rel="nofollow" href="#" title="PInterest"></a></li>
                        <li class="linkedin pull-left"><a data-role="none" target="_blank" rel="nofollow" href="#" title="Linkedin"></a></li>
                        <li class="youtube pull-left"><a data-role="none" target="_blank" rel="nofollow" href="#" title="Youtube"></a></li>
                    </ul>
                </div>
                <div class="col-xs-12 col-sm-6 no-padding">
                    <div class="clearfix payment-methods">
                        <ul>
                            <li><img src="/Content/Ecommerce/images/payments/1.png" alt=""></li>
                            <li><img src="/Content/Ecommerce/images/payments/2.png" alt=""></li>
                            <li><img src="/Content/Ecommerce/images/payments/3.png" alt=""></li>
                            <li><img src="/Content/Ecommerce/images/payments/4.png" alt=""></li>
                            <li><img src="/Content/Ecommerce/images/payments/5.png" alt=""></li>
                        </ul>
                    </div>
                     payment-methods
                </div>*@
        </div>
    </div>
    @*</footer>*@

    @*<script src="/Content/Ecommerce/js/jquery-1.11.1.min.js"></script>*@
    @*<script src="/Content/Ecommerce/js/bootstrap.min.js"></script>*@
    @*<script src="/Content/Ecommerce/js/bootstrap-hover-dropdown.min.js"></script>*@
    @*<script src="/Content/Ecommerce/js/owl.carousel.min.js"></script>*@
    @*<script src="/Content/Ecommerce/js/echo.min.js"></script>*@
    @*<script src="/Content/Ecommerce/js/jquery.easing-1.3.min.js"></script>*@
    @*<script src="/Content/Ecommerce/js/bootstrap-slider.min.js"></script>
        <script src="/Content/Ecommerce/js/jquery.rateit.min.js"></script>
        <script src="/Content/Ecommerce/js/lightbox.min.js"></script>
        <script src="/Content/Ecommerce/js/bootstrap-select.min.js"></script>
        <script src="/Content/Ecommerce/js/wow.min.js"></script>*@
    @*<script src="/Content/Ecommerce/js/scripts.js"></script>*@
</body>
</html>