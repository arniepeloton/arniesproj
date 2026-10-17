<!DOCTYPE html>
<html>
<head>
    <title>@ViewData("Title")</title>
    <meta http-equiv="X-UA-Compatible" content="IE=11">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <link rel="shortcut icon" href="/Content/images/mobiz.ico" />
    <script type="text/javascript" src="/Scripts/jquery.js"></script>
    <script type="text/javascript">
        if (location.hostname == 'localhost') {
            window.assetversion = '';
        } else {
            window.assetversion = new Date().getTime();
        }
    </script>

    <link rel="Stylesheet" type="text/css" href="/Content/bootstrap/css/bootstrap.css" />

    <!--<link rel="Stylesheet" type="text/css" href="../../Content/Site.css" />-->
    <!--<link rel="Stylesheet" type="text/css" href="../../Content/bootstrap/css/bootstrap-theme.css" />-->
    <link rel="Stylesheet" type="text/css" href="/Content/css/custom.css" />
    <link rel="Stylesheet" type="text/css" href="/Content/css/ribbon.css" />
    <link rel="Stylesheet" type="text/css" href="/Content/css/jquery.bxslider.css" />

    <script type="text/javascript" src="/Content/bootstrap/js/bootstrap.js"></script>
    <script type="text/javascript" src="/Scripts/jquery.bxslider.min.js"></script>
    <script type="text/javascript" src="/Scripts/js-custom.js"></script>
    <script type="text/javascript" src="/Scripts/bootstrap-tabcollapse.js"></script>

    @RenderSection("head", False)
</head>

<body>
    <div id="toTop" title="Back to Top">&nbsp;</div>
    <div class="navbar navbar-default" role="navigation">
        <div class="container">
            <ul class="primary-header-list" style="margin-bottom:2px;">
                <li><span id="greetings">Welcome Guest!</span> <a href="/signin">Sign In</a></li>
                <li><img src="/Content/images/philippines.ico" height="18px" width="20px" /> <span id="country">Philippines</span></li>
            </ul>
            <div class="navbar-header">
                <button type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#menu">
                    <span class="sr-only">Toggle navigation</span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                </button>
                <a class="navbar-brand" href="/signin" title="Home"><img src="/Content/images/New-Mobiz-Logo.png" style="width:150px;" class="img-responsive" /></a>
            </div>

            <div class="collapse navbar-collapse" id="menu">
                <ul class="nav navbar-nav">
                    <li class="dropdown">
                        <a href="#" class="dropdown-toggle" data-toggle="dropdown">Solutions <span class="caret"></span></a>
                        <ul class="dropdown-menu" role="menu">
                            <div class="solutiondropdown">
                                <li>
                                    <div style="padding:5px 0px 0px 10px;">
                                        <div class="row">
                                            <a style="color:#ffffff;">
                                                <div class="col-md-4">
                                                    <strong><h4>Kiosk and Food Truck</h4></strong>
                                                    <p>
                                                        <ul style="margin-left:-15px;" class="navlink">
                                                            <li><a href="http://mobiz-apps.com/Solution/FranchiseKiosksandStalls/">Franchise Kiosks and Stalls</a></li>
                                                            <li><a href="http://mobiz-apps.com/Solution/FoodTrucks/">Food Trucks</a></li>
                                                        </ul>
                                                    </p>
                                                </div>
                                            </a>
                                            <a style="color:#ffffff;">
                                                <div class="col-md-4">
                                                    <strong><h4>Full-Sized Store and Restaurant</h4></strong>
                                                    <p>
                                                        <ul style=" margin-left:-15px;" class="navlink">
                                                            <li><a href="http://mobiz-apps.com/Solution/HardwareandConstruction/">Hardware and Construction Supply</a></li>
                                                            <li><a href="http://mobiz-apps.com/Solution/PharmacyandBeautyShop/">Pharmacy and Beauty Shop</a></li>
                                                            <li><a href="http://mobiz-apps.com/Solution/SchoolandOffice/">School and Office Supplies</a></li>
                                                            <li><a href="http://mobiz-apps.com/Solution/ComputerandMobile/">Computer and Mobile Shops</a></li>
                                                            <li><a href="http://mobiz-apps.com/Solution/OtherRetail/">Other Retail Stores</a></li>
                                                        </ul>
                                                    </p>
                                                </div>
                                            </a>
                                            <a style="color:#ffffff;">
                                                <div class="col-md-4">
                                                    <strong><h4>Field and Inside Sales Force</h4></strong>
                                                    <p>
                                                        <ul style="margin-left:-15px;" class="navlink">
                                                            <li><a href="http://mobiz-apps.com/Solution/ManufacturerAndBrandOwners/">Manufacturers and Brand Owners</a></li>
                                                            <li><a href="http://mobiz-apps.com/Solution/Distributors">Distributors</a></li>
                                                            <li><a href="http://mobiz-apps.com/Solution/Wholesalers">Wholesalers</a></li>
                                                        </ul>
                                                    </p>
                                                </div>
                                            </a>
                                            @*<a style="color:#ffffff;">
                                                    <div class="col-md-4">
                                                        <strong><h4>Micro Businesses</h4></strong>
                                                        <p>
                                                            <ul style="margin-left:-15px;" class="navlink">
                                                                <li><a href="http://mobiz-apps.com/Solution/MicroRetailer/">Sari-Sari Store, Carenderia or Turo-Turo</a></li>
                                                            </ul>
                                                        </p>
                                                    </div>
                                                </a>*@
                                        </div>

                                        @*<div class="row">
                                                <a style="color:#ffffff;">
                                                    <div class="col-md-4">
                                                        <strong><h4>Field and Inside Sales Force</h4></strong>
                                                        <p>
                                                            <ul style="margin-left:-15px;" class="navlink">
                                                                <li><a href="http://mobiz-apps.com/Solution/ManufacturerAndBrandOwners/">Manufacturers and Brand Owners</a></li>
                                                                <li><a href="http://mobiz-apps.com/Solution/Distributors">Distributors</a></li>
                                                                <li><a href="http://mobiz-apps.com/Solution/Wholesalers">Wholesalers</a></li>
                                                            </ul>
                                                        </p>
                                                    </div>
                                                </a>
                                                <!--<a style="color:#ffffff;">
                                                    <div class="col-md-4">
                                                        <strong><h4>Restaurants</h4></strong>
                                                        <p>
                                                            <ul style="margin-left:-15px;" class="navlink">
                                                                <li><a href="/Solution/FastFood">Fast Food (Counter Order)</a></li>
                                                                <li><a href="/Solution/TableOrder">Fast Casual or Fine Dining (Table Order)</a></li>
                                                            </ul>
                                                        </p>
                                                    </div>
                                                </a>-->
                                            </div>*@
                                    </div>
                                </li>
                            </div>
                        </ul>
                    </li>
                    @*<li class="dropdown">
                            <a href="#" class="dropdown-toggle" data-toggle="dropdown">Subscribe <span class="caret"></span></a>
                            <ul class="dropdown-menu" role="menu">
                                <div class="subscribedropdown">
                                    <li>
                                        <div style="padding:5px 0px 0px 10px;" class="navlink">
                                            <div class="row">
                                                <div class="col-md-12" align="left">
                                                    <strong><h3>Complete Retail</h3></strong>
                                                </div>
                                                <div class="col-md-4">
                                                    <a href="http://mobiz-apps.com/Subscribe/FranchiseEdition" style="color:#ffffff;">
                                                        <strong><h4>Franchise Edition</h4></strong>
                                                        <p>
                                                            <ul style="margin-left:-15px;">
                                                                <li>Franchise Kiosks and Stalls</li>
                                                                <li>Food Trucks</li>
                                                            </ul>
                                                        </p>
                                                    </a>
                                                </div>
                                                <div class="col-md-4" class="navlink">
                                                    <a href="http://mobiz-apps.com/Subscribe/StandardEdition" style="color:#ffffff;">
                                                        <strong><h4>Standard Edition</h4></strong>
                                                        <p>
                                                            <ul style=" margin-left:-15px;">
                                                                <li>Hardware and Construction Supply</li>
                                                                <li>Pharmacy and Beauty Shop</li>
                                                                <li>School and Office Supplies</li>
                                                                <li>Computer and Mobile Shops</li>
                                                                <li>Other Retail Stores</li>
                                                            </ul>
                                                        </p>
                                                    </a>
                                                </div>
                                                <div class="col-md-4" class="navlink">
                                                    <a href="http://mobiz-apps.com/Subscribe/StarterEdition" style="color:#ffffff;">
                                                        <strong><h4>Starter Edition</h4></strong>
                                                        <p>
                                                            <ul style="margin-left:-15px;" class="navlink">
                                                                <li>Sari-Sari Store, Carenderia or Turo-Turo</li>
                                                            </ul>
                                                        </p>
                                                    </a>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12" align="left">
                                                    <strong><h3>Deal Maker</h3></strong>
                                                </div>
                                                <div class="col-md-4" class="navlink">
                                                    <a href="http://mobiz-apps.com/Subscribe/DealMaker" style="color:#ffffff;">
                                                        <strong><h4>Field and Inside Sales Force</h4></strong>
                                                        <p>
                                                            <ul style="margin-left:-15px;" class="navlink">
                                                                <li>Manufacturers and Brand Owners</li>
                                                                <li>Distributors</li>
                                                                <li>Wholesalers</li>
                                                            </ul>
                                                        </p>
                                                    </a>
                                                </div>
                                            </div>
                                        </div>
                                    </li>
                                </div>
                            </ul>
                        </li>*@
                    <li class="dropdown">
                        <a href="#" class="dropdown-toggle" data-toggle="dropdown">Support <span class="caret"></span></a>
                        <ul class="dropdown-menu" role="menu">
                            <div class="supportdropdown">
                                <li>
                                    <div style="padding:2px 0px 0px 10px;" class="navlink">
                                        <div class="row">
                                            <div class="col-xs-12" style="padding:5px 0px 0px 15px;">
                                                <a href="http://mobiz-apps.com/Support/ContactUs/" style="color:#ffffff;">
                                                    <h4>Contact Us</h4>
                                                </a>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-xs-12" style="padding:5px 0px 0px 15px;">
                                                <a href="http://mobiz-apps.com/Support/UsersManual/" style="color:#ffffff;">
                                                    <h4>User's Manual</h4>
                                                </a>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-xs-12" style="padding:5px 0px 0px 15px;">
                                                <a href="http://mobiz-apps.com/Support/FAQ/" style="color:#ffffff;">
                                                    <h4>FAQ</h4>
                                                </a>
                                            </div>
                                        </div>
                                    </div>
                                </li>
                            </div>

                        </ul>
                    </li>
                    <li><a href="http://mobiz-apps.com/About/">About Us</a></li>
                </ul>
            </div>
        </div>
    </div>

    @RenderBody()

    <div class="footer">
        <br />
        <div class="container">
            <div class="row">
                <div class="col-sm-3">
                    <img src="/Content/images/qrcode.jpg" class="img-thumbnail" style="width:110px" />
                </div>
                <div class="col-sm-3">
                    <label><h7>COMPANY</h7></label>
                    <p class="navlink"><a href="http://mobiz-apps.com/About/" style="color:#FFF">About Us</a></p>
                    <p class="navlink"><a href="http://mobiz-apps.com/Support/ContactUs/" style="color:#FFF">Contact Us</a></p>
                </div>
                <div class="col-sm-3">
                    <label><h7>SUPPORT</h7></label>
                    <p class="navlink"><a href="http://mobiz-apps.com/Support/UsersManual/" style="color:#FFF">User's Manual</a></p>
                    <p class="navlink"><a href="http://mobiz-apps.com/Support/FAQ/" style="color:#FFF">FAQ</a></p>
                </div>
                <div class="col-sm-3">
                    <label><h7>MOBIZ COMPLETE RETAIL</h7></label>
                    <p class="navlink"><a href="http://mobiz-apps.com/Subscribe/Free/" style="color:#FFF">Free</a></p>
                    <p class="navlink"><a href="http://mobiz-apps.com/Subscribe/KioskAndFoodTruck/" style="color:#FFF">Kiosk and Food Truck</a></p>
                    <p class="navlink"><a href="http://mobiz-apps.com/Subscribe/FullSizedStoreAndRestaurant/" style="color:#FFF">Full-Sized Store and Restaurant</a></p>
                </div>
            </div>
            <br />
            <div class="row" align="center">
                <div class="col-sm-12">
                    &copy; 2013, 1Mobiz Inc., All Rights Reserved.
                </div>
            </div>
        </div>
    </div>
</body>
</html>