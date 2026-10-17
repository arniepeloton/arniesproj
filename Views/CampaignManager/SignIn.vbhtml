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
<body id="login">
		<header>
			<a href="http://www.mobiz-apps.com" title="Home" id="logo">Mobiz Marketing Campaigns</a>
		</header>
        
		<section id="content">
        <h3 class="indent-text">Campaign Manager</h3>
        <div style="margin-left: 10px;"><span class="smallfontRed" id="spanError" style="display:none;">&nbsp; Invalid username and password &nbsp;</span></div>

		<form id="frmPeopleLogger" method="POST" autocomplete="off">
			<fieldset>
				<section><label for="txtUsername">Username</label>
					<div><input type="text" id="txtUsername" name="username" data-role="none" autofocus></div>
				</section>
				<section><label for="txtPassword">Password</label>
					<div><input type="password" id="txtPassword" data-role="none" name="password"></div>
					@*<div><label for="remember" class="checkbox"><input type="checkbox" id="remember" name="remember">remember me</label></div>*@
                </section>
				<section>
                    <br />
					<div><center><button id="btnSignIn" data-role="none">Sign In</button></center></div>     
                    <br />  
				</section>
			</fieldset>
            <br />
            <center>Don't have a Mobiz account? <a href="../ImportantReminders">Sign Up here</a>
            <br /><br /><a href="../AccountRecovery" id="lnkForgotPassword">I can't access my Mobiz profile</a></center>
		</form>
		</section>
		<footer>@*Copyright by revaxarts.com 2011*@</footer>
		
</body>
</html>