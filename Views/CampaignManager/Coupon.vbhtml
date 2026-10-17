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
            require([GetPathFormat('/Scripts/sources/CampaignManager/TargetAdInfo.js?crc='), GetPathFormat('/Scripts/script.js?crc=')]);
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
			<h2>Create Targeted Ad</h2>
            <br/>
                 <div id="divCouponDetails">
				    <form>
					    <fieldset>                           
                            <label>Ad Details</label>
						    <section><label for="txtAdName">Ad Name &nbsp;<span class="required">&nbsp; </span></label>
							    <div><input type="text" id="txtAdName" data-role="none" class="g12" required>
                                <br /><br /><span class="smallfontRed" id="spanAdName" style="display:none;"> &nbsp; This field is required  &nbsp;</span>
                                </div>
						    </section>
						    <section><label for="txtAdDescription">Ad Description </label>
							    <div><input type="text" id="txtAdDescription" data-role="none" class="g12">
                                <br /><br /><span class="smallfontRed" id="spanAdDescription" style="display:none;"> &nbsp; This field is required  &nbsp;</span>
                                </div>
						    </section>
                            <section><label>Ad Type &nbsp;<span class="required">&nbsp; </span></label>
							    <div><input type="radio" name="AdType" value="Promotional Campaign" id="rbPromotionalCampaign" data-role="none" class="g6" checked><label for="rbPromotionalCampaign">Promotional Campaign</label>
                                <br />
                                <br /><input type="radio" name="AdType" value="Coupon" id="rbCoupon" data-role="none"  class="g6"><label for="rbCoupon">Coupon</label>
                                <br /><br /><span class="smallfontRed" id="spanAdType" style="display:none;">&nbsp; This field is required &nbsp;</span>
                                </div>
						    </section>
                            <section><label for="txtMessage">Message &nbsp;<span class="required">&nbsp; </span></label>
							    <div><textarea id ="txtMessage" rows="4" cols="50" class="g12"></textarea>
                                <br /><br /><span class="smallfontRed" id="spanMessage" style="display:none;"> &nbsp; This field is required  &nbsp;</span>
                                </div>
						    </section>
                            <section><label for="selStartDateMonth">Ad Start Date &nbsp;<span class="required">&nbsp; </span></label>
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
                            <section><label for="selAdDuration">Ad Duration &nbsp;<span class="required">&nbsp; </span></label>
							    <div>
                                <select id="selAdDuration" data-role="none" class="g4 inline">
	                                <option value="">Select One</option>
                                    <option value="Fixed">Fixed Period</option>
                                    <option value="Unlimited">Unlimited</option>
                                    <option value="Valid for certain period">Valid for a certain time period after coupon has been received</option>
                                </select>
                                <br /><br /><span class="smallfontRed" id="spanAdDuration" style="display:none;"> &nbsp; This field is required  &nbsp;</span>
                                </div>
						    </section>
                            <section class="fixed" style="display: none;"><label for="selEndDateMonth">Coupon End Date &nbsp;<span class="required">&nbsp; </span></label>
							    <div>
                                    <select id="selEndDateMonth" data-role="none" class="g4">
	                                    <option value="" >MMM</option>
                                    </select>
                                    <select id="selEndDateDay" data-role="none" class="g4">
	                                    <option value="">DD</option>
                                    </select>                
                                    <select id="selEndDateYear" data-role="none" class="g4">
	                                    <option value="">YYYY</option>
                                    </select>  
                                    <br /><br /><span class="smallfontRed" id="spanEndDate" style="display:none;">&nbsp; This field is required &nbsp;</span>              
                                    @*<span class="smallfontRed" id="selStartDateLess" style="display:none;">&nbsp; Campaign Start Date must not be less than the date today &nbsp;</span> *@
                                 </div>   
						    </section>
                            <section class="coupon" style="display: none;"><label for="selCouponCodeLength">Coupon Code Length &nbsp;<span class="required">&nbsp; </span></label>
							    <div>
                                <select id="selCouponCodeLength" data-role="none" class="g4">
	                                <option value="">Select One</option>
                                    <option value="4">4</option>
                                    <option value="6">5</option>
                                    <option value="8">6</option>
                                    <option value="6">7</option>
                                    <option value="8">8</option>
                                    <option value="8">9</option>
                                    <option value="10">10</option>
                                </select>
                                <br /><br /><span class="smallfontRed" id="spanCouponCodeLength" style="display:none;"> &nbsp; This field is required  &nbsp;</span>
                                </div>
						    </section>
                            <section class="coupon" style="display: none;"><label for="selCouponCodeType">Coupon Code Type &nbsp;<span class="required">&nbsp; </span></label>
							    <div>
                                <select id="selCouponCodeType" data-role="none" class="g4">
	                                <option value="">Select One</option>
                                    <option value="Numeric">Numeric</option>
                                    <option value="Alphanumeric">Alphanumeric</option>
                                </select>
                                <br /><br /><span class="smallfontRed" id="spanCouponCodeType" style="display:none;"> &nbsp; This field is required  &nbsp;</span>
                                </div>
						    </section>
                            <section class="coupon" style="display: none;"><label for="txtNoOfCoupons">No. of Coupons to Create &nbsp;<span class="required">&nbsp; </span></label>
							    <div><input type="text" id="txtNoOfCoupons" data-role="none" maxlength ="8" class="g1" required>
                                <br /><br /><span class="smallfontRed" id="spanNoOfCoupons" style="display:none;"> &nbsp; This field is required  &nbsp;</span>
                                </div>
						    </section>
                            <section class="coupon" style="display: none;"><label>Add More Coupons Automatically? &nbsp;<span class="required">&nbsp; </span></label>
							    <div><input type="radio" name="autoAdd" value="1" id="rbYes" data-role="none" class="g6"><label for="rbYes">Yes</label>
                                <br />
                                <br /><input type="radio" name="autoAdd" value="0" id="rbNo" data-role="none"  class="g6" checked><label for="rbNo">No</label>
                                <br /><br />
                                <span>NOTE: New coupons can be generated automatically when they are running low </span>
                                <br /><br /><span class="smallfontRed" id="spanProduct" style="display:none;">&nbsp; This field is required &nbsp;</span>
                                </div>
						    </section>
                            <label>Target Demographic</label>
                            <section><label for="txtAgeFrom">Age Range &nbsp;<span class="required">&nbsp; </span></label>
							    <div>
                                <input type="text" id="txtAgeFrom" data-role="none" class="g1" maxlength="2" required><label>to</label><input type="text" id="txtAgeTo" maxlength="2" data-role="none" class="g1" required>
                                <br /><br /><span class="smallfontRed" id="spanAge" style="display:none;"> &nbsp; This field is required  &nbsp;</span>
                                <span class="smallfontRed" id="spanAgeValid" style="display:none;"> &nbsp; Please enter a valid age  &nbsp;</span>
                                </div>
						    </section>
                            <section><label for="selGender">Gender &nbsp;<span class="required">&nbsp; </span></label>
                                <div>
                                    <select id="selGender" data-role="none" class="g4">
	                                    <option value="">Select One</option>
                                        <option value="Male">Male</option>
                                        <option value="Female">Female</option>
                                        <option value="Both">Both</option>
                                    </select>
                                    <br /><br /><span class="smallfontRed" id="spanGender" style="display:none;"> &nbsp; This field is required  &nbsp;</span>
                                </div>
						    </section>
                            <section><label for="selCity">City or Town of Residence &nbsp;<span class="required">&nbsp; </span></label>
							    <div>
                                <select id="selCity" data-role="none" class="g4">
                                </select>
                                <br /><br /><span class="smallfontRed" id="spanCity" style="display:none;"> &nbsp; This field is required  &nbsp;</span>
                                </div>
						    </section>
                            <section>
							    <div><button id="btnCreateAd" data-role="none" >Create Targeted Ad</button></div>
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