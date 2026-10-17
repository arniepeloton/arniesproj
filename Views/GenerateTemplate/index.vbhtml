<!doctype html>
<html lang="en-us">
<head>
	<meta charset="utf-8">
	
	<title>Mobiz - PIM Generate Template</title>
	
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
            require([GetPathFormat('/Scripts/jquery-ui.min.js?crc='),
             GetPathFormat('/Scripts/sources/GenerateTemplate/GenerateTemplateInfo.js?crc='),
             GetPathFormat('/Scripts/script.js?crc=')]);
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
     @*       <li><a href="http://www.mobiz-apps.com/cart"><img src ="/Content/images/shopping_cart2.png" /> Cart</a></li>*@
			<li>Welcome, Guest <a href="SignIn">Sign In</a></li>
		</ul>	
	</div>

	<header>
		<a href="http://www.mobiz-apps.com" title="Home" id="logo">Mobiz</a>
		<div id="header">
			<ul id="headernav">
				<li><ul>
					<li><a href="http://www.mobiz-apps.com/whymobiz">Why Mobiz?</a></li>
					<li><a href="http://www.mobiz-apps.com/packages">Packages</a></li>
					<li><a href="http://www.mobiz-apps.com/about">About Us</a></li>
					<li><a href="#">Support</a>
					<ul>
						<li><a href="http://www.mobiz-apps.com/contact">Contact Us</a></li>
						<li><a href="http://www.mobiz-apps.com/documentation">Documentation</a></li>
						<li><a href="http://www.mobiz-apps.com/help">Help</a></li>
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
			<h2>Download PIM Excel Template</h2><br/>

            <p>This site is under construction.</p>
            @*<p>This generate template function lets you choose the attributes for your product and download it to an excel file.</p>
		    <form>
				<fieldset>
                    <label>List of Products (Class or Commodity)</label>
						<section><label for ="txtClass">What type of products would you like to upload? &nbsp;<span class="required">&nbsp; </span></label>
                            <div id="divClass"><input type="text" id="txtClass" placeholder="Enter keyword/s" class="g10" required><input type="hidden" id="hdnClassID" >
                            <a id="btnAddClass"><img src="../../Content/images/plus.png" id="imgAddClass" title="Get Product Features" /><label for="imgAddClass" title="Get Product Features">Get Product Features</label></a>
                            <br /><br /><span class="smallfontRed" id="spanClassRequired" style="display:none;">&nbsp; This field is required &nbsp;</span>
                            <span class="smallfontRed" id="spanClassButton" style="display:none;">&nbsp; Add features to the list by clicking the Get Product Features button. &nbsp;</span>
                            <span class="smallfontRed" id="spanClass" style="display:none;">Class or Commodity not found. Please try a different keyword and select an option from the autocomplete list.</span>
                            </div>
						</section>
				</fieldset>
			</form>	
            <form>
				<fieldset>
                    <label>List of Features to be included in Excel Template</label>
                       <section>
                            <label for ="selFeatures">Product Features</label>
                            <div><select id="selFeatures" data-role="none" class="g4" size="12"><optgroup label="Default Features">
                                <option value="Short Product Name" disabled>Short Product Name</option>
                                <option value="UNSPSC Classification by Class" disabled>UNSPSC Classification by Class</option>
                                <option value="Your Custom Classification" disabled>Your Custom Classification</option>
                                <option value="Brand" disabled>Brand</option>
                                <option value="Model Number" disabled>Model Number</option>
                                <option value="Internal Product Code" disabled>Internal Product Code</option>
                                <option value="Product Grouping Number" disabled>Product Grouping Number</option>
                                <option value="UNSPSC Commodity Type" disabled>UNSPSC Commodity Type</option>
                                <option value="Product Type" disabled>Product Type</option>
                                <option value="Suggested Retail Price (SRP)" disabled>Suggested Retail Price (SRP)</option>
                                <option value="Marketing Text" disabled>Marketing Text</option>
                                <option value="Warranty Policy" disabled>Warranty Policy</option>
                                <option value="UPC Code" disabled>Packing Size</option>
                                <option value="UPC Code" disabled>Content</option>
                                <option value="UPC Code" disabled>Product Dimensions</option>
                                <option value="UPC Code" disabled>Product Gross Weight</option>
                                </optgroup></select>
                                <a id="btnRemoveFeature"><img src="../../Content/images/minus.png" id="imgRemoveFeature" title="Remove" /><label for="imgRemoveFeature" title="Remove">Remove</label></a>
                                </div>
                       </section>
                       <section>
                            <label for ="selCustomFeatures">Your Custom Features</label>
                            <div><select id="selCustomFeatures" data-role="none" class="g4" size="6"></select>
                            <a id="btnRemove"><img src="../../Content/images/minus.png" id="imgRemove" title="Remove" /><label for="imgRemove" title="Remove">Remove</label></a>
                            </div>
                       </section>
                       <section><label for ="txtFeature">Add a Feature</label>
                            <div><input type="text" id="txtFeature" placeholder="Enter keyword/s" class="g4"><input type="hidden" id="hdnFeatureID" >
                            to <br /><br /><select id="selClass" data-role="none" class="g4"><option value="">Please Select Product Type</option></select>
                            <a id="btnAdd"><img src="../../Content/images/plus.png" id="imgAdd" title="Add" /><label for="imgAdd" title="Add">Add</label></a>
                            <br /><br /><span class="smallfontRed" id="spanAddRequired" style="display:none;">&nbsp; This field is required &nbsp;</span>
                            <span class="smallfontRed" id="spanAddButton" style="display:none;">&nbsp; Add a feature to the list by clicking the Add button. &nbsp;</span>
                            <span class="smallfontRed" id="spanAdd" style="display:none;">Feature not found. Please try a different keyword and select an option from the autocomplete list.</span>
                            </div>
					    </section>
                        
				</fieldset>
			</form>	
  
            <center><button id="btnDownload" data-role="none">Download Excel Template</button></center>
            <br />*@
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
		<div class="g1 footer-div">
			SUPPORT<br/>
			<a href="http://www.mobiz-apps.com/help">Help</a><br/>
			<a href="http://www.mobiz-apps.com/documentation">Documentation</a><br/>
			<a href="http://www.mobiz-apps.com/faq">FAQ</a><br/>
		</div>
		<div class="g5 footer-right">
			<a href="http://www.mobiz-apps.com">Mobiz Inc.</a> | <a href="#">Privacy</a> | <a href="#">Terms of Use</a>
			<br/><br/><br/><br/><br/><br/>
			This website is best viewed in Firefox.	<br/>
			Copyright by revaxarts.com 2011
		</div>
	</footer>
</body>
</html>