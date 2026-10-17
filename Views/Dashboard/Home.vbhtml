<!doctype html>
<html lang="en-us">
<head>
	<meta charset="utf-8">
	
	<title>Mobiz Dashboard</title>
	
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
            require([GetPathFormat('/Scripts/sources/SignUp/SignUpInfo.js?crc='), GetPathFormat('/Scripts/script.js?crc=')]);
        });
    </script>

    <!-- all Whitelabel Plugins -->
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
            <li><label id="lblName"></label> <a href="/dashboard/signin?mode=logout" id="lnkSignIn">Sign Out</a></li>
		</ul>
	</div>

	<header>
		<a href="/dashboard" title="Home" id="logo">Mobiz</a>
		<div id="header">
			<ul id="headernav">
				<li><ul>
					<li><a href="/dashboard/subscription">Subscription</a></li>
                    <li><a href="../dashboard">Update Manager</a></li>
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
			<h2>Account Recovery</h2>
            <br/>
         @*   <p>This site is under construction.</p>*@
				<div id="wrapper">

                <div id="divFullDetails">
                <p>Fill in all the fields to retrieve security question.</p>
				    <form>
					    <fieldset>
                        <label>Personal Information</label>
						    <section><label for="txtFirstName">Full Name <span class="required">&nbsp; </span></label>
							    <div><input type="text" id="txtFirstName" class="g4" data-role="none" placeholder="First Name" required><input type="text" id="txtMiddleName" class="g4" data-role="none" placeholder="Middle Name" required><input id="txtSurname" type="text" class="g4" data-role="none" placeholder="Last Name" required>
                                <br /><br /><span class="smallfontRed" id="spanName" style="display:none;"> &nbsp; This field is required  &nbsp;</span></div>
						    </section>
                            <section><label for="selPersonalSuffixName">Generational Suffix <span class="required">&nbsp; </span></label>
							    <div>
                                <select id="selPersonalSuffixName" data-role="none" class="g4">
	                                <option value="">Please Select</option>
				                    <option value="Sr.">Senior (Sr.)</option>
				                    <option value="Jr.">Junior (Jr.)</option>
				                    <option value="II">II</option>
				                    <option value="III">III</option>
				                    <option value="IV">IV</option>
	                                <option value="V">V</option>
				                    <option value="VI">VI</option>
	                                <option value="VII">VII</option>
				                    <option value="VIII">VIII</option>
				                    <option value="IX">IX</option>
				                    <option value="X">X</option>
                                </select>
                                <input type="checkbox" id="chbNoSuffix" data-role="none" value="None" name="suffix" class="g8"/><label for="chbNoSuffix" data-role="none">I don't have a generational suffix</label>
                                <br /><br /><br /><span class="smallfontRed indent-text" id="spanSuffix" style="display:none;">&nbsp; This field is required &nbsp;</span>
                                </div>
						    </section>
						    <section><label for="selDOBMonth">Date of Birth <span class="required">&nbsp; </span></label>
							    <div>
                                    <select id="selDOBMonth" data-role="none" class="g4">
	                                    <option value="" >MMM</option>
                                    </select>
                                    <select id="selDOBDay" data-role="none" class="g4">
	                                    <option value="">DD</option>
                                    </select>                
                                    <select id="selDOBYear" data-role="none" class="g4">
	                                    <option value="">YYYY</option>
                                    </select>  
                                    <br /><br /><span class="smallfontRed" id="spanDateOfBirth" style="display:none;"> &nbsp; This field is required  &nbsp;</span>              
                                 </div>       
						    </section>			
                            <section><label for="txtMotherMaidenFirstName">Mother's Maiden Name <span class="required">&nbsp; </span></label>
							    <div><input type="text" id="txtMotherMaidenFirstName" class="g4" data-role="none" placeholder="Mother's First Name" required><input type="text" id="txtMotherMaidenMiddleName" class="g4" data-role="none" placeholder="Mother's Maiden Middle Name" required><input type="text" id="txtMotherMaidenSurname" class="g4" data-role="none" placeholder="Mother's Maiden Last Name" required>
						        <br /><br /><span class="smallfontRed" id="spanMotherName" style="display:none;"> &nbsp; This field is required  &nbsp;</span></div>
                            </section>
						    <section>
							    <div><button id="btnSubmit" data-role="none" >Submit</button></div>
						    </section>
                            
					    </fieldset>
				    </form>	
				</div>
                <div id="divSecurityQuestion" style="display:none;">
                <p>To verify your account, please answer the security questions below. Note: Answers are case-sensitive.</p>
				    <form>
                        <fieldset>
                            <label>Security Question</label>
						    <section><label for="txtQ1AnswerAccountRecovery">Question No.1 &nbsp;<span class="required">&nbsp; </span></label>
							    <div><label id="lblQuestion1" for="txtQ1AnswerAccountRecovery" class="inline">Question 1 Goes Here? </label>
                                <input type="text" id="txtQ1AnswerAccountRecovery" data-role="none" class="g8 right" required>
                                <br /><br /><span class="smallfontRed" id="spanAnswer1AccountRecovery" style="display:none;"> &nbsp; This field is required  &nbsp;</span></div>
						    </section>
						    <section><label for="txtQ2AnswerAccountRecovery">Question No.2 &nbsp;<span class="required">&nbsp; </span></label>
							    <div><label id="lblQuestion2" for="txtQ2AnswerAccountRecovery" class="inline">Question 2 Goes Here? </label>
                                <input type="text" id="txtQ2AnswerAccountRecovery" data-role="none" class="g8 right" required>
                                <br /><br /><span class="smallfontRed" id="spanAnswer2AccountRecovery" style="display:none;"> &nbsp; This field is required  &nbsp;</span></div>
						    </section>
                            <section>
							    <div><button id="btnSubmitSecurity" data-role="none" >Submit</button></div>
						    </section>
          
					    </fieldset>
				    </form>	
				</div>
                <div id="divUserAccount" style="display:none;">
                <p>Almost there! Please reset your user name and password to start using Mobiz.</p>
				    <form>
					    <fieldset>
                            <label>User Account</label>
						    <section><label for="txtUserNameUsrAcct">User Name &nbsp;<span class="required">&nbsp; </span></label>
							    <div><input type="text" id="txtUserNameUsrAcct" data-role="none" required>
                                <br /><span class="smallfontRed" id="spanUserNameUsrAcct" style="display:none;"> &nbsp; This field is required  &nbsp;</span>
                                <span class="smallfontRed" id="spanUserNameUsrAcctChars" style="display:none;"> &nbsp; User Name must be at least 8 characters long  &nbsp;</span>
                                <span class="smallfontRed" id="spanUserNameUsrAcctAlphanum" style="display:none;">&nbsp; User Name must contain only alphanumeric and underscore characters &nbsp;</span>
                                </div>
                            </section>
						    <section><label for="txtPasswordUsrAcct">Password &nbsp;<span class="required">&nbsp; </span></label>
							    <div><input type="password" id="txtPasswordUsrAcct" data-role="none" required>
                                <br /><span class="smallfontRed" id="spanPasswordUsrAcct" style="display:none;"> &nbsp; This field is required  &nbsp;</span>
                                <span class="smallfontRed" id="spanPasswordUsrAcctChars" style="display:none;"> &nbsp; Password must be at least 8 characters long  &nbsp;</span>
                                </div>
						    </section>
						    <section><label for="txtConfirmPasswordUsrAcct">Confirm Password &nbsp;<span class="required">&nbsp; </span></label>
							    <div><input type="password" id="txtConfirmPasswordUsrAcct" data-role="none" required>
                                <br /><span class="smallfontRed" id="spanConfirmPasswordUsrAcct" style="display:none;"> &nbsp; This field is required  &nbsp;</span>
                                <span class="smallfontRed" id="spanPasswordRepeatUsrAcct" style="display:none;"> &nbsp; The password you entered did not match  &nbsp;</span></div>
						    </section>
						    <section>
							    <div><button id="btnSubmitUsrAcct" data-role="none" >Submit</button></div>
						    </section>
                            
					    </fieldset>
				    </form>	
				</div>
                
                <input type="hidden" id="hdnPage" value="accountRecovery">
                <input type="hidden" id="txtPeopleID">
                <input type="hidden" id="txtQ1ID">
                <input type="hidden" id="txtQ2ID">
			</div>
			
			</div>
		</section>
		<footer id ="hidethis" data-role="footer">
			<div class="g2 footer-div">

			</div>
			<div class="g2 footer-div">

			</div>
			<div class="g2 footer-div">

			</div>
			<div class="g2 footer-div">

			</div>
			<div class="g4 footer-right">
                <br />
				Copyright 2013, 1Mobiz Inc., All Rights Reserved.
                <br /><br />
			</div>
	</footer>
</body>
</html>