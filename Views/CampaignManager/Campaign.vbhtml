<!doctype html>
<html lang="en-us">
<head>
	<meta charset="utf-8">
	
	<title>Mobiz Campaign Manager</title>
	
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
            require([GetPathFormat('/Scripts/sources/CampaignManager/CampaignInfo.js?crc='), GetPathFormat('/Scripts/script.js?crc=')]);
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
		<a href="../campaignmanager" title="Home" id="logo">Mobiz</a>
		<div id="header">
			<ul id="headernav">
				<li><ul>
					<li><a href="../campaignmanager">Campaigns</a></li>
                    <li><a href="#">Ad Tracker</a></li>
                    <li><a href="#">My Profile</a></li>

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
			<br/>
			<div class="g12">
			<h2>Campaign Manager</h2>
            <br/>
                 <div id="divCreateNewCampaign">
				    <form>
					    <fieldset>                           
                            <label>Mobiz Marketing Campaigns</label>
						    <section><label for="txtCampaignName">Campaign Name &nbsp;<span class="required">&nbsp; </span></label>
							    <div><input type="text" id="txtCampaignName" data-role="none" class="g12" required>
                                <br /><span class="smallfontRed" id="spanCampaignName" style="display:none;"> &nbsp; This field is required  &nbsp;</span>
                                </div>
						    </section>
                            <section><label for="txtCampaignDescription">Campaign Description</label>
							    <div><input type="text" id="txtCampaignDescription" data-role="none" class="g12">
                                </div>
						    </section>
						    @*<section><label for="chbInformationBlast">Campaign Type &nbsp;<span class="required">&nbsp; </span></label>
							   <div><input type="checkbox" id="chbInformationBlast" data-role="none" value="Information Blast" name="chk_group" class="g6"><label for="chbInformationBlast"> Information Blast</label>
                                <br />
                                <br /><input type="checkbox" id="chbFreeCoupons" data-role="none" value="Free Coupons" name="chk_group" class="g6" disabled><label for="chbFreeCoupons"> Free Coupons</label>
                                <br />
                                <br /><input type="checkbox" id="chbPremiumCoupons" data-role="none" value="Premium Coupons" name="chk_group" class="g6" disabled><label for="chbPremiumCoupons"> Premium Coupons</label>
                                <br /><br /><span class="smallfontRed" id="spanCampaignType" style="display:none;">&nbsp; This field is required &nbsp;</span>
                                </div>
						    </section>*@
                            <section><label for="selStartDateMonth">Campaign Start Date &nbsp;<span class="required">&nbsp; </span></label>
							    <div>
                                    <select id="selStartDateMonth" data-role="none" class="g4">
	                                    <option value="" >MMM</option>
                                    </select>
                                    <select id="selStartDateDay" data-role="none" class="g4">
	                                    <option value="">DD</option>
                                    </select>                
                                    <select id="selStartDateYear" data-role="none" class="g4">
	                                    <option value="">YYYY</option>
                                    </select>  
                                    <br /><br /><span class="smallfontRed" id="spanStartDate" style="display:none;">&nbsp; This field is required &nbsp;</span>              
                                    @*<span class="smallfontRed" id="selStartDateLess" style="display:none;">&nbsp; Campaign Start Date must not be less than the date today &nbsp;</span> *@
                                 </div>   
						    </section>
                            <section><label for="selCampaignDuration">Campaign Duration &nbsp;<span class="required">&nbsp; </span></label>
							    <div>
                                <select id="selCampaignDuration" data-role="none" class="g4" required>
	                                <option value="">Select One</option>
                                    <option value="1">1 week</option>
                                    <option value="2">2 weeks</option>
                                    <option value="3">3 weeks</option>
                                    <option value="4">4 weeks</option>
                                    <option value="5">5 weeks</option>
                                    <option value="6">6 weeks</option>
                                    <option value="7">7 weeks</option>
                                    <option value="8">8 weeks</option>
                                    <option value="9">9 weeks</option>
                                    <option value="10">10 weeks</option>
                                </select>
                                <br /><br /><span class="smallfontRed" id="spanCampaignDuration" style="display:none;"> &nbsp; This field is required  &nbsp;</span>
                                </div>
						    </section>
                            <section>
							    <div><button id="btnCreateCampaign" data-role="none" >Create Campaign</button></div>
						    </section>
					    </fieldset>
				    </form>	
				</div>
               
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