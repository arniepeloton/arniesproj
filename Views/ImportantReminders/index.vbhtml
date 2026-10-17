<!doctype html>
<html lang="en-us">
<head>
	<meta charset="utf-8">
	
	<title>Mobiz - Important Reminders</title>
	
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
	
	
	<!-- Use Google CDN for jQuery and jQuery UI -->
	
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
            require([GetPathFormat('/Scripts/sources/SignUp/SignUpInfo.js?crc='), GetPathFormat('/Scripts/script.js?crc=')]);
        });
	</script>


	<!-- Loading JS Files this way is not recommended! Merge them but keep their order -->
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
			<li>Welcome, Guest <a href="signin">Sign In</a></li>
		</ul>
	</div>

	<header>
		<a href="/signin" title="Home" id="logo">Mobiz</a>
		<div id="header">
			<ul id="headernav">
				<li><ul>
					<li><a href="http://www.mobiz-apps.com/whymobiz">Why Mobiz?</a></li>
                    <li><a href="http://www.mobiz-apps.com/features">Features</a></li>
					<li><a href="http://www.mobiz-apps.com/subscribe">Subscribe</a></li>
					<li><a href="http://www.mobiz-apps.com/about">About Us</a></li>
					<li><a href="#">Support</a>
					<ul>
						<li><a href="http://www.mobiz-apps.com/contact">Contact Us</a></li>
						<li><a href="http://www.mobiz-apps.com/usersmanual">User's Manual</a></li>
						<li><a href="http://www.mobiz-apps.com/faq">FAQ</a></li>
					</ul>
					</li>
				</ul></li>
			</ul>
			@*<div id="searchbox">
				<form id="searchform">
					<input type="search" name="query" id="search" placeholder="Search">
				</form>
			</div>*@
		</div>
	</header>

	<section id="content">
		<div class="g12">
			<br/>
			<h2>Important Reminders</h2><br/>

                <p>Your <strong>Mobiz Account</strong> belongs to you and not to the organization that you are/were connected with. 
                Your account sticks with you and you can use it whenever Mobiz is used anywhere in the world. 
                It is your permanent access to all of Mobiz services.</p>

                <p>An example of where you can find Mobiz is in applications like your company's POS and other 
                business apps. You can also use your account for Mobiz services such as online shopping, career 
                hunting, and your payroll and HR services. All you need for access is your user name and password 
                (and in some sensitive apps, a correctly registered device).</p>

                <p><strong>As your Mobiz Account is like your personal key to all of Mobiz services, we strongly advise 
                you to keep your username and password a very good secret.</strong> You can change your user name and/or 
                password anytime you need or want. On our part, we have implemented a vast amount of ways to 
                keep your data secure.</p>

                <p>Finally, your Mobiz Account contains information that's necessary for HR services. Thus, 
                it is to your best interest to <strong>fill in the information fields as accurately as you can</strong> to make 
                processing your payroll and benefits a breeze.</p>

                <p>From the Mobiz Team, thank you and welcome to Mobiz!</p>

                <input type="checkbox" id="chbAgree" data-role="none" value="yes" name="agree"/><label for="chbAgree" data-role="none">&nbsp; I understand and agree with these reminders</label>
                <br /><br />
                <button id="btnProceed" data-role="none" >Proceed to Sign Up</button>

                <input type="hidden" id="hdnPage" value="privacy">
                
		</div>
	</section>
		<footer id ="hidethis" data-role="footer">
			<div class="g2 footer-div">
				<img src="/Content/images/qrcode.jpg"/><br/>
			</div>
			<div class="g2 footer-div">
				PRODUCT<br/>
				<a href="http://mobiz-apps.com/whymobiz">Why Mobiz?</a><br/>
				<a href="http://mobiz-apps.com/features">Features</a><br/>
			</div>
			<div class="g2 footer-div">
				COMPANY<br/>
				<a href="http://mobiz-apps.com/about">About Us</a><br/>
				<a href="http://mobiz-apps.com/contact">Contact</a><br/>
			</div>
			<div class="g2 footer-div">
				SUPPORT<br/>
				<a href="http://mobiz-apps.com/usersmanual">User's Manual</a><br/>
				<a href="http://mobiz-apps.com/faq">FAQ</a><br/>
			</div>
			<div class="g4 footer-right">
				<a href="http://mobiz-apps.com/home">1Mobiz Inc.</a> | <a href="#">Privacy</a> | <a href="#">Terms of Use</a>
				<br/><br/><br/><br/><br/><br/>
				Copyright 2013, 1Mobiz Inc., All Rights Reserved.
			</div>
	</footer>
</body>
</html>