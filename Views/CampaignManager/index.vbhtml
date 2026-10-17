<!doctype html>
<html lang="en-us">
<head>
	<meta charset="utf-8">
	
	<title>Mobiz Account Management</title>
	
	<meta name="description" content="">
	<meta name="author" content="revaxarts.com">
	
	
	<!-- Apple iOS and Android stuff -->
	<meta name="apple-mobile-web-app-capable" content="no">
	<meta name="apple-mobile-web-app-status-bar-style" content="black">
	<link rel="apple-touch-icon-precomposed" href="/Content/images/apple-touch-icon-precomposed.png">
	
	<!-- Apple iOS and Android stuff - don't remove! -->
	<meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=no,maximum-scale=1">
	
	<!-- Google Font and style definitions -->
	<link rel="stylesheet" href="/Content/style.css">
	
	
	<!-- include the skins (change to dark if you like) -->
	<link rel="stylesheet" href="/Content/theme.css" class="theme">
	<!-- <link rel="stylesheet" href="css/dark/theme.css" class="theme"> -->
	
	   <script src="/Scripts3/others/require.js"></script>
    <script>

        function GetWindowPathFormat(vString) {
            // alert(vString + '. Please use GetPathFormat');
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
    <script src="/Scripts3/common/init.js?crc="></script>
    <script>
        require(['common'], function () {
            require([GetPathFormat('/Scripts/sources/CampaignManager/CampaignSignIn.js?crc='), GetPathFormat('/Scripts/script.js?crc=')]);
        });
    </script>

    <!--[if lt IE 9]>
	<script src="http://html5shim.googlecode.com/svn/trunk/html5.js"></script>
	<link rel="stylesheet" href="css/ie.css">
	<![endif]-->
	<link rel="icon" type="image/ico" href="/Content/images/icon.ico"></link>
	<link rel="shortcut icon" href="/Content/images/icon.ico"></link>
</head>
<body>
	<div id="pageoptions">
		<ul>
         @*   <li><a href="http://www.mobiz-apps.com/cart"><img src ="/Content/images/shopping_cart2.png" /> Cart</a></li>*@
			<li id="menuSignOut" style="display:none;">Welcome, <label id="lblName"></label> <a id="lnkSignOut">Sign Out</a></li>
            <li id="menuSignIn">Welcome, Guest <a href="/campaignmanager/signin">Sign In</a></li>
		</ul>
	</div>

	<header>
		<a href="#" title="Home" id="logo">Mobiz</a>
		<div id="header">
			@*<ul id="headernav">
				<li><ul>
					<li><a href="http://www.mobiz-apps.com/whymobiz">Why Mobiz?</a></li>
					<li><a href="http://www.mobiz-apps.com/subscribe">Subscribe</a></li>
					<li><a href="http://www.mobiz-apps.com/about">About Us</a></li>
					<li><a href="#">Support</a>
					<ul>
						<li><a href="http://www.mobiz-apps.com/contact">Contact Us</a></li>
						<li><a href="http://www.mobiz-apps.com/UsersManual">User's Manual</a></li>
						<li><a href="http://www.mobiz-apps.com/faq">FAQ</a></li>
					</ul>
					</li>
				</ul></li>
			</ul>*@
			@*<div id="searchbox">
				<form id="searchform">
					<input type="search" name="query" id="search" placeholder="Search">
				</form>
			</div>*@
		</div>
	</header>
		<section id="content">
			<br/>
			<div class="g12">
			    <h2>Campaign Manager</h2>
                <br/>
 
			    <table class="datatable">
				    <thead>
					    <tr>
						    <th>Coupon Code</th>
						    <th>Coupon Name</th>
						    <th>Coupon Description</th>
						    <th>Coupon Code Type</th>
						    <th>Age Range</th>
                            <th>Gender</th>
                            <th>City</th>
					    </tr>
				    </thead>
				    <tbody>
				    </tbody>
			    </table>

                <br />
			    <a href="/campaignmanager/addnew"><button id="btnNewCampaign" data-role="none" >Add New Campaign</button></a>
            
                <input type="hidden" id="txtPeopleID">
		
			</div>
		</section>
		<footer id ="hidethis" data-role="footer">
			<div class="g2 footer-div">
				<img src="/Content/images/qrcode.jpg"/><br/>
				Mobile Version QR Code
			</div>
			<div class="g2 footer-div">
				PRODUCT<br/>
				<a href="http://www.mobiz-apps.com/whymobiz">Why Mobiz?</a><br/>
				<a href="#">Latest News</a><br/>
			</div>
			<div class="g2 footer-div">
				COMPANY<br/>
				<a href="http://www.mobiz-apps.com/about">About Us</a><br/>
				<a href="#">History</a><br/>
				<a href="http://www.mobiz-apps.com/contact">Contact</a><br/>
			</div>
			<div class="g2 footer-div">
				SUPPORT<br/>
				<a href="http://www.mobiz-apps.com/UsersManual">User's Manual</a><br/>
				<a href="http://www.mobiz-apps.com/faq">FAQ</a><br/>
			</div>
			<div class="g4 footer-right">
				<a href="http://www.mobiz-apps.com">Mobiz Inc.</a> | <a href="#">Privacy</a> | <a href="#">Terms of Use</a>
				<br/><br/><br/><br/><br/><br/>
				This website is best viewed in Firefox.	<br/>
				Copyright by revaxarts.com 2011
			</div>
	</footer>
</body>
</html>