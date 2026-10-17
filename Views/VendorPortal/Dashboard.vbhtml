@Code
    ViewData("Title") = "Vendor Portal Dashboard"
    Layout = "~/Views/Shared/VendorPortalLayout.vbhtml"
End Code
@Section head
    <script type="text/javascript">
        require(['/Content/Ecommerce/js/lightbox-2.11.1.js', '/Scripts/sources/VendorPortal/Dashboard.js?v=' + assetversion], function () {
            lightbox.option({
                'resizeDuration': 200,
                'wrapAround': true,
                'imageFadeDuration': 300,
                'alwaysShowNavOnTouchDevices': true
            })
        })
    </script>    
End Section

<nav class="navbar navbar-primary">
    <div class="container-fluid">
        <!-- Brand and toggle get grouped for better mobile display -->
        <div class="navbar-header">
            <button data-role="none" type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#bs-example-navbar-collapse-1" aria-expanded="false">
                <span class="sr-only">Toggle navigation</span>
                <span class="icon-bar"></span>
                <span class="icon-bar"></span>
                <span class="icon-bar"></span>
            </button>
            <a data-role="none" class="navbar-brand" href="#">Vendor Portal</a>
        </div>

        <!-- Collect the nav links, forms, and other content for toggling -->
        <div class="collapse navbar-collapse" id="bs-example-navbar-collapse-1">
            <ul class="nav navbar-nav">
                <li id="menu-canvass"><a data-role="none" href="#">Canvass Forms</a></li>
                <li id="menu-fulfullment-center"><a data-role="none" href="#">Purchase Orders</a></li>
            </ul>
            <ul class="nav navbar-nav navbar-right">
                <li class="dropdown">
                    <a data-role="none" id="menu-vendorname" href="#" class="dropdown-toggle" data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false"> <span class="caret"></span></a>
                    <ul class="dropdown-menu">
                        <li>
                            <div class="divTable" style="color:#4e4d4d; font-size:14px; padding:0px 15px;">
                                <div class="divTableRow">
                                    <div class="divTableCell" style="width:60px;">
                                        <button data-role="none" class="btn custom-btn-theme-h btn-less-width btnCircle">
                                            <span class="glyphicon glyphicon-user"></span>
                                        </button>
                                    </div>
                                    <div class="divTableCell" style="width:120px;" id="lblUserName">

                                    </div>
                                </div>                                
                            </div>
                        </li>
                        <li role="separator" class="divider"></li>
                        <li id="menu-myprofile"><a data-role="none" href="#">My Profile</a></li>
                        <li id="menu-logout"><a data-role="none" href="#">Logout</a></li>
                    </ul>
                </li>
            </ul>
        </div><!-- /.navbar-collapse -->
    </div><!-- /.container-fluid -->

    <div class="BreadCrumbs">
        <div class="container-fluid">
            <div class="ItemsBreadCrumbs">
                
            </div>
        </div>
    </div>
</nav>
<div class="container-fluid" style="margin-top:35px;">
    <div id="divVendorDashboard" custom-data-role="page" style="display:block">Dashboard</div>
    <div id="divVendorCanvass" custom-data-role="page" style="display:none"></div>
    <div id="divOrderFulfillmentWork" custom-data-role="page" style="display:none"></div>
    <div id="divVendorProfile" custom-data-role="page" style="display:none"></div>
</div>
