<!doctype html>
<html lang="en-us">
<head>
	<meta charset="utf-8">
	
	<title>Mobiz Profile</title>
	
	<meta name="description" content="">
	<meta name="author" content="revaxarts.com">
	
	
	<!-- Apple iOS and Android stuff -->
	<meta name="apple-mobile-web-app-capable" content="no">
	<meta name="apple-mobile-web-app-status-bar-style" content="black">
	<link rel="apple-touch-icon-precomposed" href="/Content/images/apple-touch-icon-precomposed.png">
	
	<!-- Apple iOS and Android stuff - don't remove! -->
	<meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=no,maximum-scale=1">
	
	<!-- Google Font and style definitions -->
	<link rel="stylesheet" href="http://fonts.googleapis.com/css?family=PT+Sans:regular,bold">
	<link rel="stylesheet" href="/Content/style.css">
	
	
	<!-- include the skins (change to dark if you like) -->
	<link rel="stylesheet" href="/Content/theme.css" class="theme">
	<!-- <link rel="stylesheet" href="css/dark/theme.css" class="theme"> -->
	
    <!-- Use Google CDN for jQuery and jQuery UI -->
	<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.6.1/jquery.min.js"></script>
	<script src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.8.12/jquery-ui.min.js"></script>
	
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
//        require(['common'], function () {
//            require([GetPathFormat('/Scripts/sources/People/PeopleInfoName.js?crc=')]);
//        });
    </script>

    <!--[if lt IE 9]>
	<script src="http://html5shim.googlecode.com/svn/trunk/html5.js"></script>
	<link rel="stylesheet" href="css/ie.css">
	<![endif]-->

    <script src="/Scripts/wl_Store.js"></script>
	<script src="/Scripts/wl_Widget.js"></script>
	
	<!-- the script which handles all the access to plugins etc... -->
	<script src="/Scripts/script.js"></script>

	 <script>
	     $(function () {
	         $('.accordion').accordion({
	             collapsible: true,
	             active: false
	         });

	         $('#lnkAlias').click(function () {
	             $('#formAlias').toggle();
	         });

	         $('#lnkAddress').click(function () {
	             $('#formAddress').toggle();
	         });

	         $('#lnkTelephone').click(function () {
	             $('#formTelephone').toggle();
	         });

	         $('#lnkMobile').click(function () {
	             $('#formMobile').toggle();
	         });

	         $('#lnkEmail').click(function () {
	             $('#formEmail').toggle();
	         });

	         $('#lnkEducationalBackground').click(function () {
	             $('#formEducationalBackground').toggle();
	         });

	         $('#lnkEmployment').click(function () {
	             $('#formEmployment').toggle();
	         });
	     });
	</script>

	<link rel="icon" type="image/ico" href="/Content/images/icon.ico"></link>
	<link rel="shortcut icon" href="/Content/images/icon.ico"></link>
</head>
<body>
	<div id="pageoptions">
		<ul>
           @* <li><a href="http://www.mobiz-apps.com/cart"><img src ="/Content/images/shopping_cart2.png" /> Cart</a></li>*@
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
			<br/>
			<div class="g12">
			<h2>Profile</h2>
            <br/>
                <div class="widget" id="widget_accordion">
						<div class="accordion">
							<h2><a href="#">Name</a></h2>
							<div>
				                <form>
					                <fieldset>
						                <label>Personal</label>
                                        <section><label for="selTitle">Title</label>
							                <div><select id="selTitle" data-role="none" class="g4"></select></div>
						                </section>
						                <section><label for="txtFirstNameProfile">Full Name &nbsp;<span class="required">&nbsp; </span></label>
							                <div><input type="text" id="txtFirstNameProfile" class="g4" data-role="none" placeholder="First Name" required><input type="text" id="txtMiddleNameProfile" class="g4" data-role="none" placeholder="Middle Name" required><input id="txtSurnameProfile" type="text" class="g4" data-role="none" placeholder="Last Name" required>
                                            <br /><br /><span class="smallfontRed" id="spanNameProfile" style="display:none;">This field is required.</span></div>
						                </section>
                                        <section><label for="txtGenerationalSuffix">Generational Suffix</label>
							                <div><input type="text" id="txtGenerationalSuffix" class="g4" data-role="none"></div>
						                </section>
                                        <section><label for="selPostNominalSuffix">Post Nominal Suffix</label>
							                <div><select id="selPostNominalSuffix" data-role="none" class="g4"></select></div>
						                </section>
						                <section><label for="selDOBMonthProfile">Date of Birth &nbsp;<span class="required">&nbsp; </span></label>
							                <div>
                                                <select id="selDOBMonthProfile" data-role="none" class="g4">
	                                                <option value="" >MMM</option>
                                                </select>
                                                <select id="selDOBDayProfile" data-role="none" class="g4">
	                                                <option value="">DD</option>
                                                </select>                
                                                <select id="selDOBYearProfile" data-role="none" class="g4">
	                                                <option value="">YYYY</option>
                                                </select>  
                                                <br /><br /><span class="smallfontRed" id="spanDateOfBirthProfile" style="display:none;">This field is required.</span>              
                                                </div>       
						                </section>
                                        <section><label for="txtNickname">Nickname</label>
							                <div><input type="text" id="txtNickname" class="g4" data-role="none"></div>
						                </section>
                                        <label>Mother's Maiden Name</label>					
                                        <section><label for="txtMotherMaidenFirstNameProfile">Mother's Maiden Name &nbsp;<span class="required">&nbsp; </span></label>
							                <div><input type="text" id="txtMotherMaidenFirstNameProfile" class="g4" data-role="none" placeholder="Mother's First Name" required><input type="text" id="txtMotherMaidenMiddleNameProfile" class="g4" data-role="none" placeholder="Mother's Maiden Middle Name" required><input type="text" id="txtMotherMaidenSurnameProfile" class="g4" data-role="none" placeholder="Mother's Maiden Last Name" required>
						                    <br /><br /><span class="smallfontRed" id="spanMotherNameProfile" style="display:none;">This field is required.</span></div>
                                        </section>
                                            <section>
							                    <div><button id="btnEditName" data-role="none" >Edit</button></div>
						                    </section>
          
					                    </fieldset>
				                    </form>	
							</div>
							
							<h2><a href="#">Alias</a></h2>
							<div>
                                <p><div class="g3">Alias List</div><div class="g9"><a href="#" id="lnkAlias"> + Add New</a></div></p>
                                <hr />
								
                                <form id="formAlias" style="display:none;">
					                <fieldset>
						                <section><label for="selTitleAlias">Title</label>
							                <div><select id="selTitleAlias" data-role="none" class="g4"></select></div>
						                </section>
						                <section><label for="txtFirstNameAlias">Full Name &nbsp;<span class="required">&nbsp; </span></label>
							                <div><input type="text" id="txtFirstNameAlias" class="g4" data-role="none" placeholder="First Name" required><input type="text" id="txtMiddleName" class="g4" data-role="none" placeholder="Middle Name" required><input id="txtSurname" type="text" class="g4" data-role="none" placeholder="Last Name" required>
                                            <br /><br /><span class="smallfontRed" id="spanFirstNameAlias" style="display:none;">This field is required.</span></div>
						                </section>
                                        <section><label for="txtGenerationalSuffixAlias">Generational Suffix</label>
							                <div><input type="text" id="txtGenerationalSuffixAlias" class="g4" data-role="none"></div>
						                </section>
                                        <section><label for="selPostNominalSuffixAlias">Post Nominal Suffix</label>
							                <div><select id="selPostNominalSuffixAlias" data-role="none" class="g4"></select></div>
						                </section>
                                        <section>
							                <div><button id="btnAddAlias" data-role="none" >Save</button></div>
						                </section>
          
					                    </fieldset>
				                    </form>	
							</div>
							
							<h2><a href="#">Address</a></h2>
							<div>
								<p><div class="g3">Address List</div><div class="g9"><a href="#" id="lnkAddress"> + Add New</a></div></p>
                                <hr />
								
                                <form id="formAddress" style="display:none;">
					                <fieldset>
						                <section><label for="txtHouseNo">House or Building No.</label>
							                <div><input type="text" id="txtHouseNo" class="g4" data-role="none"></div>
						                </section>
						                <section><label for="txtStreet">Street &nbsp;<span class="required">&nbsp; </span></label>
							                <div><input type="text" id="txtStreet" class="g4" data-role="none">
                                            <br /><br /><span class="smallfontRed" id="spanStreet" style="display:none;">This field is required.</span></div>
						                </section>
                                        <section><label for="selCity">City or Town &nbsp;<span class="required">&nbsp; </span></label>
							                <div><select id="selCity" data-role="none" class="g4"></select>
                                            <br /><br /><span class="smallfontRed" id="spanCity" style="display:none;">This field is required.</span></div>
						                </section>
                                        <section><label for="selCountry">Country &nbsp;<span class="required">&nbsp; </span></label>
							                <div><select id="selCountry" data-role="none" class="g4"></select>
                                            <br /><br /><span class="smallfontRed" id="spanCountry" style="display:none;">This field is required.</span></div>
						                </section>
                                        <section><label for="selAddressType">Type of Address &nbsp;<span class="required">&nbsp; </span></label>
							                <div><select id="selAddressType" data-role="none" class="g4"></select>
                                            <br /><br /><span class="smallfontRed" id="spanAddressType" style="display:none;">This field is required.</span></div>
						                </section>
                                        <section>
							                <div><button id="btnAddAddress" data-role="none" >Save</button></div>
						                </section>
          
					                    </fieldset>
				                    </form>	
							</div>

                            <h2><a href="#">Personal Information</a></h2>
							<div>
								<form>
					                <fieldset>
						                <section><label for="rbtMale">Gender &nbsp;<span class="required">&nbsp; </span></label>
							                <div>
                                                <div class="g4">
                                                    <input type="radio" name="gender" value="male" id="rbtMale" > Male</div>
                                                <div class="g4">
                                                    <input type="radio" name="gender" value="female" id="rbtFemale"> Female</div>
                                                <br /><span class="smallfontRed" id="spanGender" style="display:none;">This field is required.</span>
                                            </div>
						                </section>
						                <section><label for="selCitizenship">Citizenship</label>
							                <div><select id="selCitizenship" data-role="none" class="g4"></select></div>
						                </section>
                                        <section><label for="selCountryOfBirth">Country of Birth</label>
							                <div><select id="selCountryOfBirth" data-role="none" class="g4"></select></div>
						                </section>
                                        <section><label for="selPlaceOfBirth">Place of Birth</label>
							                <div><select id="selPlaceOfBirth" data-role="none" class="g4"></select></div>
						                </section>
                                        <section><label for="txtHeight">Height</label>
							                <div><input type="text" id="txtHeight" class="g4" data-role="none"></div>
						                </section>
                                        <section><label for="txtWeight">Weight</label>
							                <div><input type="text" id="txtWeight" class="g4" data-role="none"></div>
						                </section>
                                        <section><label for="selCivilStatus">Civil Status</label>
							                <div><select id="selCivilStatus" data-role="none" class="g4"></select></div>
						                </section>
                                        <section>
							                <div><button id="btnEditPersonalInfo" data-role="none" >Edit</button></div>
						                </section>
          
					                    </fieldset>
				                    </form>	
							</div>
							
							<h2><a href="#">Contact</a></h2>
							<div>
                                <div class="widget" id="widget_accordion">
						            <div class="accordion">
							            <h2><a href="#">Telephone</a></h2>
							            <div>
								            <p><div class="g3">Telephone</div><div class="g9"><a href="#" id="lnkTelephone"> + Add New</a></div></p>
                                            <hr />
								
                                            <form id="formTelephone" style="display:none;">
					                            <fieldset>
						                            <section><label for="cmbCallingCodeTel">Telephone Number  &nbsp;<span class="required">&nbsp; </span></label>
							                            <div><select id="cmbCallingCodeTel" data-role="none" class="g4"><option value="" selected="selected">Please Select Country Code</option></select><select id="cmbAreaCode" data-role="none" class="g4"><option value="" selected="selected">Please Select Area Code</option></select><input id="txtTelephone" data-regex="^[0-9\-]{8}$" data-role="none" class="g4 right" maxlength="7" required>
                                                        <br /><br /><span class="smallfontRed" id="spanTelNo" style="display:none;">This field is required.</span></div>
						                            </section>
						                            <section><label for="txtExt">Extension or Local No.</label>
							                            <div><input type="text" id="txtExt" class="g4" data-role="none">
                                                        </div>
						                            </section>
                                                    <section><label for="selContactType">Contact Type &nbsp;<span class="required">&nbsp; </span></label>
							                            <div><select id="selContactType" data-role="none" class="g4" required></select>
                                                        <br /><br /><span class="smallfontRed" id="spanContactType" style="display:none;">This field is required.</span></div>
						                            </section>
                                                    <section>
							                            <div><button id="btnAddTelephone" data-role="none" >Save</button></div>
						                            </section>
          
					                                </fieldset>
				                                </form>	

							            </div>
							
							            <h2><a href="#">Mobile</a></h2>
							            <div>
								            <p><div class="g3">Mobile</div><div class="g9"><a href="#" id="lnkMobile"> + Add New</a></div></p>
                                            <hr />
								
                                            <form id="formMobile" style="display:none;">
					                            <fieldset>
						                            <section><label for="cmbCallingCodeMobile">Mobile Number &nbsp;<span class="required">&nbsp; </span></label>
							                            <div><select id="cmbCallingCodeMobile" data-role="none" class="g4"><option value="" selected="selected">Please Select Country Code</option></select><select id="cmbTelcoPrefix" data-role="none" class="g4"><option value="" selected="selected">Please Select Mobile Number Prefix</option></select><input id="txtMobile" data-regex="^[0-9]{7}$" data-role="none" class="g4 right" maxlength="7" placeholder="Enter your 7-digit mobile number" required>
                                                        <br /><br /><span class="smallfontRed" id="spanMobile" style="display:none;">This field is required.</span></div>
						                            </section>
                                                    <section><label for="selContactTypeMobile">Contact Type &nbsp;<span class="required">&nbsp; </span></label>
							                            <div><select id="selContactTypeMobile" data-role="none" class="g4" required></select>
                                                        <br /><br /><span class="smallfontRed" id="spanContactTypeMobile" style="display:none;">This field is required.</span></div>
						                            </section>
                                                    <section>
							                            <div><button id="btnAddMobile" data-role="none" >Save</button></div>
						                            </section>
          
					                            </fieldset>
				                            </form>	
							            </div>

							            <h2><a href="#">Email</a></h2>
							            <div>
								           <p><div class="g3">Email</div><div class="g9"><a href="#" id="lnkEmail"> + Add New</a></div></p>
                                            <hr />
								
                                            <form id="formEmail" style="display:none;">
					                            <fieldset>
						                            <section><label for="txtEmail">Email Address  &nbsp;<span class="required">&nbsp; </span></label>
							                            <div><input type="text" id="txtEmail" class="g4" data-role="none">
                                                        <br /><br /><span class="smallfontRed" id="spanEmail" style="display:none;">This field is required.</span></div>
						                            </section>
                                                    <section><label for="selEmailType">Email Type  &nbsp;<span class="required">&nbsp; </span></label>
							                            <div><select id="selEmailType" data-role="none" class="g4" required></select>
                                                        <br /><br /><span class="smallfontRed" id="spanEmailType" style="display:none;">This field is required.</span></div>
						                            </section>
                                                    <section>
							                            <div><button id="btnAddEmail" data-role="none" >Save</button></div>
						                            </section>
          
					                            </fieldset>
				                            </form>	
							            </div>
							
						            </div>
					            </div>

							</div>

                            <h2><a href="#">Educational Background</a></h2>
							<div>
								<p><div class="g3">Educational Background List</div><div class="g9"><a href="#" id="lnkEducationalBackground"> + Add New</a></div></p>
                                <hr />
								
                                <form id="formEducationalBackground" style="display:none;">
					                <fieldset>
						                <section><label for="selAcademicLevel">Academic Level &nbsp;<span class="required">&nbsp; </span></label>
							                <div><select id="selAcademicLevel" data-role="none" class="g4" required></select>
                                            <br /><br /><span class="smallfontRed" id="spanAcademicLevel" style="display:none;">This field is required.</span></div>
						                </section>
                                        <section><label for="selCourse">Course &nbsp;<span class="required">&nbsp; </span></label>
							                <div><select id="selCourse" data-role="none" class="g4" required></select>
                                            <br /><br /><span class="smallfontRed" id="spanCourse" style="display:none;">This field is required.</span></div>
						                </section>
                                        <section><label for="selInstitutionName">Institution Name &nbsp;<span class="required">&nbsp; </span></label>
							                <div><select id="selInstitutionName" data-role="none" class="g4" required></select>
                                            <br /><br /><span class="smallfontRed" id="spanInstitutionName" style="display:none;">This field is required.</span></div>
						                </section>
                                        <section><label for="txtHouseNoEduc">House or Building No.</label>
							                <div><input type="text" id="txtHouseNoEduc" class="g4" data-role="none"></div>
						                </section>
						                <section><label for="txtStreetEduc">Street &nbsp;<span class="required">&nbsp; </span></label>
							                <div><input type="text" id="txtStreetEduc" class="g4" data-role="none">
                                            <br /><br /><span class="smallfontRed" id="spanStreetEduc" style="display:none;">This field is required.</span></div>
						                </section>
                                        <section><label for="selCityEduc">City or Town &nbsp;<span class="required">&nbsp; </span></label>
							                <div><select id="selCityEduc" data-role="none" class="g4"></select>
                                            <br /><br /><span class="smallfontRed" id="spanCityEduc" style="display:none;">This field is required.</span></div>
						                </section>
                                        <section><label for="selCountryEduc">Country &nbsp;<span class="required">&nbsp; </span></label>
							                <div><select id="selCountryEduc" data-role="none" class="g4"></select>
                                            <br /><br /><span class="smallfontRed" id="spanCountryEduc" style="display:none;">This field is required.</span></div>
						                </section>
                                        <section><label for="selYearStarted">Year Started &nbsp;<span class="required">&nbsp; </span></label>
							                <div><select id="selYearStarted" data-role="none" class="g4"></select>
                                            <br /><br /><span class="smallfontRed" id="spanYearStarted" style="display:none;">This field is required.</span></div>
						                </section>
                                        <section><label for="selYearEnded">Year Ended &nbsp;<span class="required">&nbsp; </span></label>
							                <div><select id="selYearEnded" data-role="none" class="g4"></select>
                                            <br /><br /><span class="smallfontRed" id="spanYearEnded" style="display:none;">This field is required.</span></div>
						                </section>
                                        <section><label for="rbtGraduatedYes">Graduated</label>
							                <div>
                                                <div class="g4">
                                                    <input type="radio" name="graduated" value="yes" id="rbtGraduatedYes" > Yes</div>
                                                <div class="g4">
                                                    <input type="radio" name="graduated" value="no" id="rbtGraduatedNo"> No</div>
                                                <br /><span class="smallfontRed" id="spanGraduated" style="display:none;">This field is required.</span>
                                            </div>
						                </section>
                                        
                                        <section>
							                <div><button id="btnAddEduc" data-role="none" >Save</button></div>
						                </section>
          
					                </fieldset>
				                </form>	
							</div>

                            <h2><a href="#">Employment History</a></h2>
							<div>
								<p><div class="g3">Employment History List</div><div class="g9"><a href="#" id="lnkEmployment"> + Add New</a></div></p>
                                <hr />
								
                                <form id="formEmployment" style="display:none;">
					                <fieldset>
						                <section><label for="txtCompanyName">Company Name &nbsp;<span class="required">&nbsp; </span></label>
							                <div><input type="text" id="txtCompanyName" class="g4" data-role="none">
                                            <br /><br /><span class="smallfontRed" id="spanCompanyName" style="display:none;">This field is required.</span></div>
						                </section>
                                        <section><label for="selOccupation">Occupation &nbsp;<span class="required">&nbsp; </span></label>
							                <div><select id="selOccupation" data-role="none" class="g4" required></select>
                                            <br /><br /><span class="smallfontRed" id="spanOccupation" style="display:none;">This field is required.</span></div>
						                </section>
                                        <section>
							                <div><button id="btnAddCompanyName" data-role="none" >Save</button></div>
						                </section>
          
					                </fieldset>
				                </form>
							</div>

                            <h2><a href="#">Related Person</a></h2>
							<div>
								<p><div class="g3">Related Person List</div><div class="g9"><a href="#" id="lnkRelatedPerson"> + Add New</a></div></p>
                                <hr />
								
                                <form id="formRelatedPerson" style="display:none;">
					                <fieldset>
						                <section><label for="selTitleRelatedPerson">Title</label>
							                <div><select id="selTitleRelatedPerson" data-role="none" class="g4"></select></div>
						                </section>
						                <section><label for="txtFirstNameRelatedPerson">Full Name &nbsp;<span class="required">&nbsp; </span></label>
							                <div><input type="text" id="txtFirstNameRelatedPerson" class="g4" data-role="none" placeholder="First Name" required><input type="text" id="txtMiddleName" class="g4" data-role="none" placeholder="Middle Name" required><input id="txtSurname" type="text" class="g4" data-role="none" placeholder="Last Name" required>
                                            <br /><br /><span class="smallfontRed" id="spanNameProfileRelatedPerson" style="display:none;">This field is required.</span></div>
						                </section>
                                        <section><label for="txtGenerationalSuffixRelatedPerson">Generational Suffix</label>
							                <div><input type="text" id="txtGenerationalSuffixRelatedPerson" class="g4" data-role="none"></div>
						                </section>
                                        <section><label for="selPostNominalSuffixRelatedPerson">Post Nominal Suffix</label>
							                <div><select id="selPostNominalSuffixRelatedPerson" data-role="none" class="g4"></select></div>
						                </section>
                                        <section><label for="selRelationship">Relationship</label>
							                <div><select id="selRelationship" data-role="none" class="g4"></select></div>
						                </section>
                                        <section><label for="rbtGraduatedYes">Contact this person in case of emergency?</label>
							                <div>
                                                <div class="g4">
                                                    <input type="radio" name="emergencyContact" value="yes" id="rbtContactYes" > Yes</div>
                                                <div class="g4">
                                                    <input type="radio" name="emergencyContact" value="no" id="rbtContactNo"> No</div>
                                                <br /><span class="smallfontRed" id="spanGraduated" style="display:none;">This field is required.</span>
                                            </div>
						                </section>
                                        <section>
							                <div><button id="btnAddRelatedPerson" data-role="none" >Save</button></div>
						                </section>
          
					                </fieldset>
				                </form>
							</div>

                            <h2><a href="#">Primary ID Reference</a></h2>
							<div>
								<form>
					                <fieldset>
						                <section><label for="txtResidenceCertNo">Residence Certification No.</label>
							                <div><input type="text" id="txtResidenceCertNo" class="g4" data-role="none"></div>
						                </section>
						                <section><label for="txtSSSNo">SSS No.</span></label>
							                <div><input type="text" id="txtSSSNo" class="g4" data-role="none">
                                            </div>
						                </section>
                                        <section><label for="txtPhilHealthNo">Phil. Health No.</label>
							                <div><input type="text" id="txtPhilHealthNo" class="g4" data-role="none"></div>
						                </section>
                                        <section><label for="txtTIN">TIN</label>
							                <div><input type="text" id="txtTIN" class="g4" data-role="none"></div>
						                </section>
                                        <section><label for="txtHDMFNo">HDMF No.</label>
							                <div><input type="text" id="txtHDMFNo" class="g4" data-role="none"></div>
						                </section>
                                        <section><label for="txtNBINo">NBI No.</label>
							                <div><input type="text" id="txtNBINo" class="g4" data-role="none"></div>
						                </section>
                                        <section><label for="txtPassportNo">Passport No.</label>
							                <div><input type="text" id="txtPassportNo" class="g4" data-role="none"></div>
						                </section>
                                        <section>
							                <div><button id="btnAddPrimaryID" data-role="none" >Save</button></div>
						                </section>
          
					                </fieldset>
				                </form>
							</div>

							<h2><a href="#">User Account</a></h2>
							<div>
				                <form>
					                <fieldset>
								    <label>User Account</label>
						                    <section><label for="txtUserName">User Name &nbsp;<span class="required">&nbsp; </span></label>
							                    <div><input type="text" id="txtUserName" class="g4" data-role="none" required>
                                                <br /><br /><span class="smallfontRed" id="spanUserName" style="display:none;">This field is required.</span></div>
						                    </section>
						                    <section><label for="txtPassword">Password &nbsp;<span class="required">&nbsp; </span></label>
							                    <div><input type="password" id="txtPassword" class="g4" data-role="none" required>
                                                <br /><br /><span class="smallfontRed" id="spanPassword" style="display:none;">This field is required.</span>
                                                </div>
						                    </section>
						                    <section><label for="txtConfirmPassword">Confirm Password &nbsp;<span class="required">&nbsp; </span></label>
							                    <div><input type="password" id="txtConfirmPassword" class="g4" data-role="none" required>
                                                <br /><br /><span class="smallfontRed" id="spanConfirmPassword" style="display:none;">This field is required.</span>
                                                <span class="smallfontRed" id="spanPasswordRepeat" style="display:none;">The password you entered did not match.</span></div>
						                    </section>
                                            <label>Security Question
                                            <br /><span>Note: You must remember the answers to the security questions you selected. This will be used to reset your account in case you forget your password.</span>
                                            </label>
						                    <section><label for="txtQ1Answer">Question No.1 &nbsp;<span class="required">&nbsp; </span></label>
							                    <div>
                                                    <select id="selQuestion1" data-role="none" class="g4">
	                                                    <option value="">Please Select</option>
				                                        <option value="Where is your mother's birthplace?">Where is your mother's birthplace?</option>
				                                        <option value="Who is your best childhood friend?">Who is your best childhood friend?</option>
				                                        <option value="What is the name of your first pet?">What is the name of your first pet?</option>
				                                        <option value="Who is your favorite teacher?">Who is your favorite teacher?</option>
				                                        <option value="Who is your favorite historical person?">Who is your favorite historical person?</option>
	                                                    <option value="What is your grandfather's occupation?">What is your grandfather's occupation?</option>
                                                    </select>
                                                <input id="txtQ1Answer" type="text" class="g8" data-role="none" required>
                                                <br /><br /><span class="smallfontRed" id="spanAnswer1" style="display:none;">This field is required.</span>
                                                <span class="smallfontRed" id="spanQuestion1" style="display:none;">Please choose a different question.</span></div>
						                    </section>
						                    <section><label for="txtQ2Answer">Question No.2 &nbsp;<span class="required">&nbsp; </span></label>
							                    <div>
                                                    <select id="selQuestion2" data-role="none" class="g4">
	                                                    <option value="">Please Select</option>
				                                        <option value="Where is your mother's birthplace?">Where is your mother's birthplace?</option>
				                                        <option value="Who is your best childhood friend?">Who is your best childhood friend?</option>
				                                        <option value="What is the name of your first pet?">What is the name of your first pet?</option>
				                                        <option value="Who is your favorite teacher?">Who is your favorite teacher?</option>
				                                        <option value="Who is your favorite historical person?">Who is your favorite historical person?</option>
	                                                    <option value="What is your grandfather's occupation?">What is your grandfather's occupation?</option>
                                                    </select>
                                                <input id="txtQ2Answer" type="text" class="g8" data-role="none" required>
                                                <br /><br /><span class="smallfontRed" id="spanAnswer2" style="display:none;">This field is required.</span>
                                                <span class="smallfontRed" id="spanQuestion2" style="display:none;">Please choose a different question.</span></div>
						                    </section>
                                            <section>
							                    <div><button id="btnEditUserAccount" data-role="none" >Edit</button></div>
						                    </section>
                                        </fieldset>
				                    </form>	

							</div>
				</div>
            </div>
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
				<br/><br/><br/><br/><br/><br/><br/>
				This website is best viewed using Firefox and Chrome web browsers.	<br/>
				Copyright by revaxarts.com 2011
			</div>
	</footer>
</body>
</html>