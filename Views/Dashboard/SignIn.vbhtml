<!doctype html>
<html lang="en-us">
<head>
    <meta charset="utf-8">
    <title>Sign in to Mobiz</title>
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
    <!-- Loading JS Files this way is not recommended! Merge them but keep their order -->
    <!--[if lt IE 9]>
	<script src="http://html5shim.googlecode.com/svn/trunk/html5.js"></script>
	<link rel="stylesheet" href="css/ie.css">
	<![endif]-->
    <script src="/Scripts3/others/require.js"></script>
    <script>
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
    <script src="/Scripts3/common/init.js?crc=AFC5254C"></script>
    <script>
        require(['common'], function () {
            require([GetPathFormat('/Scripts/sources/Dashboard/DashboardSignIn.js?crc=19E98E6B')]);
        });
    </script>
    <link rel="icon" type="image/ico" href="/Content/images/icon.ico"></link>
    <link rel="shortcut icon" href="/Content/images/icon.ico"></link>
</head>
<body id="login">
    <div style="text-align: center; margin-top: 24px">
        <a href="/dashboard" title="Home" id="logo" style="margin: auto;width:112px;">Mobiz</a>
        
    </div>
    <section id="content">
        <h3 class="indent-text">Dashboard</h3>
        <div class="smallfontRed" ><center><span id="spanError" style="display:none;">Invalid username and password</span></center></div>

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
					<div><center>
                    <button id="btnSignIn" data-role="none">Sign in</button>
                    <div id="divSignIn" style="display: none;"><center><img src="/Content/images/loading.gif" /><br />Signing in...</center></div>
                    </center>
                    </div>     
                    <br />  
				</section>
			</fieldset>
            <br />
            
		</form>
		</section>
    <footer>@*Copyright by revaxarts.com 2011*@</footer>
</body>
</html>
