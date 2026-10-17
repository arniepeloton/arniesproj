@Code
    ViewData("Title") = "Vendor Portal"
    Layout = "~/Views/Shared/VendorPortalLayout.vbhtml"
End Code
@Section head
    <script type="text/javascript">
        require(['/Scripts/sources/VendorPortal/Signin.js?crc='])
    </script>
End Section
<div class="body-content outer-top-xs" id="top-banner-and-menu">
    <div class="container">
        <div class="sign-in-page" style="border:none;">
            <div id="divSignin" subpage="pageSignin" style="display:block;">
                <div class="row">
                    <!-- Sign-in -->
                    <div class="col-sm-offset-3 col-sm-6 sign-in-page" style="box-shadow: 0 10px 20px rgba(0,0,0,0.19), 0 6px 6px rgba(0,0,0,0.23);">
                        <div style="text-align:center; color:#0083c1; font-size:26px; font-weight:bold;">
                            E-Commerce <br /> Vendor Portal
                        </div>
                        <br />
                        <div id="divConfirmedEmail" style="display:none;" class="alert alert-success" role="alert">You have confirmed your email address. You may now sign in.</div>
                        <h4 class="">Sign in</h4>
                        <p class="">Hello, Welcome!</p>
                        @*<div class="social-sign-in outer-top-xs">
                            <a data-role="none" href="#" class="facebook-sign-in"><i class="fa fa-facebook"></i> Sign In with Facebook</a>
                            <a data-role="none" href="#" class="twitter-sign-in"><i class="fa fa-twitter"></i> Sign In with Twitter</a>
                        </div>*@
                        <form id="formLogin" class="register-form outer-top-xs" role="form">
                            <div id="divErrLoginMsg" style="color:#ff3737; text-align:center; font-size:14px; margin-bottom:25px; display:none;">Invalid username / password</div>
                            <div class="form-group">
                                <label class="info-title" for="txtuname">Username <span>*</span></label>
                                <input data-role="none" type="text" class="form-control unicase-form-control text-input" id="txtuname">
                            </div>
                            <div class="form-group">
                                <label class="info-title" for="txtpword">Password <span>*</span></label>
                                <input data-role="none" type="password" class="form-control unicase-form-control text-input" id="txtpword">
                            </div>
                            <div class="radio outer-xs">
                                <button data-role="none" type="submit" class="btn btn-primary btn-lg">Login</button>
                                <br /><br />
                                <a data-role="none" id="btnForgotPassword" href="#" class="forgot-password pull-left">Forgot your Password?</a>
                            </div>
                            <hr />
                            <div style="text-align:center;"><a data-role="none" id="btnCreateAccountSupplier" href="#">Create a new Account</a></div>
                        </form>
                    </div>
                </div><!-- /.row -->

                <br /><br />
                <div style="text-align:center;">
                    <p>Powered by <a data-role="none" href="http://mobiz-apps.com/"><img src="/Content/images/MobizNewLogo.png" alt="Mobiz" style="vertical-align:baseline;" width="70" align="bottom"></a></p>
                </div>
            </div>
            <div id="divCreation" subpage="pageSignin" style="display:none;">
                @*<a id="btnBackToLogin" data-role="none" href="#" style="font-size:16px;">
                    <span class="glyphicon glyphicon-chevron-left" style="font-size:14px;"></span>
                    GO BACK TO LOGIN
                </a>
                <br /><br />*@
                <div class="row">
                    <div class="col-md-offset-3 col-sm-6 sign-in">
                        <h2 id="lblSignupAccountType">Signup as Supplier</h2>
                        <br />
                        <div id="divCompanyAcc" subpage="divCreation" style="display:none;">
                            <div>
                                <b>1. Company Details</b>
                                <span style="color:#CCC">
                                    &rarr; 2. Administrator Account
                                </span>
                            </div>
                            <hr />
                            <h4>Company Details</h4>
                            <p class="text title-tag-line">Fill in the required fields</p>
                            <div class="register-form outer-top-xs" role="form">
                                <div class="form-group">
                                    <label class="info-title" for="txtCompanyName">Company Name <span>*</span></label>
                                    <input data-role="none" type="text" class="form-control unicase-form-control text-input" id="txtCompanyName">
                                </div>

                                <div class="form-group">
                                    <label class="info-title" for="txtCompanyTIN">TIN</label>
                                    <input data-role="none" type="text" class="form-control unicase-form-control text-input" id="txtCompanyTIN">
                                </div>

                                <div class="form-group">
                                    <label class="info-title" for="txtBusInd">Business Industry <span>*</span></label>
                                    <input data-role="none" type="text" class="form-control unicase-form-control text-input" id="txtBusInd">
                                    <div id="listBusinessIndustry" class="list-group" style="display:none; position: absolute; max-width: 100%; box-shadow: 0px 5px 28px #CCC; overflow-y:auto; max-height:300px;"></div>
                                </div>

                                <div class="form-group">
                                    <label class="info-title" for="txtCompanyEmailAdd">Company Email Address <span>*</span></label>
                                    <input data-role="none" type="text" class="form-control unicase-form-control text-input" id="txtCompanyEmailAdd">
                                </div>

                                <div class="form-inline">
                                    <div class="form-group">
                                        <label class="info-title" for="txtCompanyMobileNo">Company Mobile Number <span>*</span></label>
                                        <input data-role="none" type="text" value="+63" maxlength="3" class="form-control unicase-form-control text-input" disabled style="width:50px;">
                                        @*<select data-role="none" id="selCompanyTelcoPrefix" class="form-control unicase-form-control text-input fixed"></select>*@
                                        <input data-role="none" type="text" placeholder="9**********" maxlength="10" class="form-control unicase-form-control text-input" id="txtCompanyMobileNo" style="width:110px;">
                                    </div>
                                </div>

                                <h4>Company's Main Address</h4>

                                <div class="form-group">
                                    <label class="info-title" for="txtCompanyAddressBldgNo">House/Building Number <span>*</span></label>
                                    <input data-role="none" type="text" class="form-control unicase-form-control text-input" id="txtCompanyAddressBldgNo">
                                </div>

                                <div class="form-group">
                                    <label class="info-title" for="txtCompanyAddressStreetName">Street Name <span>*</span></label>
                                    <input data-role="none" type="text" class="form-control unicase-form-control text-input" id="txtCompanyAddressStreetName">
                                </div>

                                <div class="form-group">
                                    <label class="info-title" for="selCompanyAddressCity">City <span>*</span></label>
                                    <select data-role="none" id="selCompanyAddressCity" class="form-control unicase-form-control text-input"></select>
                                </div>

                                <div class="form-group">
                                    <label class="info-title" for="selCompanyAddressCountry">Country <span>*</span></label>
                                    <select data-role="none" id="selCompanyAddressCountry" class="form-control unicase-form-control text-input"></select>
                                </div>

                                <div id="divCompanyErrMsg" style="color:#ff3737; text-align:center; font-size:14px; margin-bottom:25px; display:none;">Please complete the required field(s)</div>

                                <div>
                                    <button id="btnBackToSignin" data-role="none" class="btn btn-default btn-less-width" style="float:left;">Back</button>
                                    <button id="btnNextStep" data-role="none" class="btn btn-primary btn-less-width" style="float:right;">NEXT STEP</button>
                                </div>
                            </div>
                        </div>

                        <div id="divPersonalAcc" subpage="divCreation" style="display:none;">
                            <div id="divBreadCrumbsForCompany">
                                <b style="color:#00a200;">1. Company Details</b>
                                &rarr;
                                <b>2. Administrator Account</b>
                                <hr />
                            </div>
                            <h4 id="titlePersonalAcc">Personal Account</h4>
                            <p class="text title-tag-line">Fill in the required fields</p>
                            <div class="register-form outer-top-xs" role="form">
                                <div class="form-group">
                                    <label class="info-title" for="txtPersonFirstName">First Name <span>*</span></label>
                                    <input data-role="none" type="text" class="form-control unicase-form-control text-input" id="txtPersonFirstName">
                                </div>

                                <div class="form-group">
                                    <label class="info-title" for="txtPersonMiddleName">Middle Name</label>
                                    <input data-role="none" type="text" class="form-control unicase-form-control text-input" id="txtPersonMiddleName">
                                </div>

                                <div class="form-group">
                                    <label class="info-title" for="txtPersonLastName">Last Name <span>*</span></label>
                                    <input data-role="none" type="text" class="form-control unicase-form-control text-input" id="txtPersonLastName">
                                </div>

                                <div class="form-group">
                                    <label class="info-title" for="selPersonGenSuffix">Generational Suffix</label>
                                    <select data-role="none" id="selPersonGenSuffix" class="form-control unicase-form-control text-input">
                                        <option value="">None</option>
                                        <option value="Sr.">Sr.</option>
                                        <option value="Jr.">Jr.</option>
                                        <option value="I">I</option>
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
                                </div>

                                <div class="form-inline">
                                    <div class="form-group">
                                        <label class="info-title">Date of Birth <span>*</span></label>
                                        <select data-role="none" id="selPersonBirthMonth" class="form-control unicase-form-control text-input fixed">
                                            <option value="">Month</option>
                                            <option value="January">January</option>
                                            <option value="February">February</option>
                                            <option value="March">March</option>
                                            <option value="April">April</option>
                                            <option value="May">May</option>
                                            <option value="June">June</option>
                                            <option value="July">July</option>
                                            <option value="August">August</option>
                                            <option value="September">September</option>
                                            <option value="October">October</option>
                                            <option value="November">November</option>
                                            <option value="December">December</option>
                                        </select>
                                        <select data-role="none" id="selPersonBirthDay" class="form-control unicase-form-control text-input fixed"></select>
                                        <input data-role="none" type="text" placeholder="Year" maxlength="4" class="form-control unicase-form-control text-input" id="txtPersonBirthYear" style="width:70px;">
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="info-title" for="txtPersonEmailAdd">Email Address <span>*</span></label>
                                    <p class="text title-tag-line"><b>NOTE</b>: You must remember the email address you enter. This will be used to reset your account in case you forget your password.</p>
                                    <input data-role="none" type="text" class="form-control unicase-form-control text-input" id="txtPersonEmailAdd">
                                </div>

                                <h4>Home Address</h4>
                                <div class="form-group">
                                    <label class="info-title" for="txtPersonAddressBldgNo">House/Building Number <span>*</span></label>
                                    <input data-role="none" type="text" class="form-control unicase-form-control text-input" id="txtPersonAddressBldgNo">
                                </div>

                                <div class="form-group">
                                    <label class="info-title" for="txtPersonAddressStreetName">Street Name <span>*</span></label>
                                    <input data-role="none" type="text" class="form-control unicase-form-control text-input" id="txtPersonAddressStreetName">
                                </div>

                                <div class="form-group">
                                    <label class="info-title" for="selPersonAddressCity">City <span>*</span></label>
                                    <select data-role="none" id="selPersonAddressCity" class="form-control unicase-form-control text-input"></select>
                                </div>

                                <div class="form-group">
                                    <label class="info-title" for="selPersonAddressCountry">Country <span>*</span></label>
                                    <select data-role="none" id="selPersonAddressCountry" class="form-control unicase-form-control text-input"></select>
                                </div>

                                <h4>Login Credentials</h4>
                                <div class="form-group">
                                    <label class="info-title" for="txtPersonUsername">User Name <span>*</span> (minimum of 8 characters)</label>
                                    <input data-role="none" type="text" class="form-control unicase-form-control text-input" id="txtPersonUsername">
                                </div>

                                <div class="form-group">
                                    <label class="info-title" for="txtPersonPassword">Password <span>*</span> (minimum of 8 characters)</label>
                                    <input data-role="none" type="password" class="form-control unicase-form-control text-input" id="txtPersonPassword">
                                </div>

                                <div class="form-group">
                                    <label class="info-title" for="txtPersonConfirmPassword">Confirm Password <span>*</span></label>
                                    <input data-role="none" type="password" class="form-control unicase-form-control text-input" id="txtPersonConfirmPassword">
                                </div>

                                <h4>Security Questions</h4>
                                <p class="text title-tag-line"><b>NOTE</b>: You must remember the answers to the security questions you selected. This will be used to reset your account in case you forget your password.</p>
                                <br />
                                <div class="form-group">
                                    <label class="info-title" for="selSecQuestion1">Question No. 1 <span>*</span></label>
                                    <span>
                                        <select id="selSecQuestion1" data-role="none" class="form-control unicase-form-control text-input">
                                            <option value="">Please Select</option>
                                            <option value="Where is your mother's birthplace?">Where is your mother's birthplace?</option>
                                            <option value="Who is your best childhood friend?">Who is your best childhood friend?</option>
                                            <option value="What is the name of your first pet?">What is the name of your first pet?</option>
                                            <option value="Who is your favorite teacher?">Who is your favorite teacher?</option>
                                            <option value="Who is your favorite historical person?">Who is your favorite historical person?</option>
                                            <option value="What is your grandfather's occupation?">What is your grandfather's occupation?</option>
                                        </select>
                                    </span>
                                    <br />
                                    <span>
                                        <input data-role="none" type="text" placeholder="Answer of Question No. 1" class="form-control unicase-form-control text-input" id="txtSecQuestion1Answer">
                                    </span>
                                </div>

                                <div class="form-group">
                                    <label class="info-title" for="selSecQuestion2">Question No. 2 <span>*</span></label>
                                    <span>
                                        <select id="selSecQuestion2" data-role="none" class="form-control unicase-form-control text-input">
                                            <option value="">Please Select</option>
                                            <option value="Where is your mother's birthplace?">Where is your mother's birthplace?</option>
                                            <option value="Who is your best childhood friend?">Who is your best childhood friend?</option>
                                            <option value="What is the name of your first pet?">What is the name of your first pet?</option>
                                            <option value="Who is your favorite teacher?">Who is your favorite teacher?</option>
                                            <option value="Who is your favorite historical person?">Who is your favorite historical person?</option>
                                            <option value="What is your grandfather's occupation?">What is your grandfather's occupation?</option>
                                        </select>
                                    </span>
                                    <br />
                                    <span>
                                        <input data-role="none" type="text" placeholder="Answer of Question No. 2" class="form-control unicase-form-control text-input" id="txtSecQuestion2Answer">
                                    </span>
                                </div>

                                <div id="divErrMsg" style="color:#ff3737; text-align:center; font-size:14px; margin-bottom:25px; display:none;">Please complete the required field(s)</div>

                                <div style="clear:both;">
                                    <button id="btnBackToCompany" data-role="none" class="btn btn-default btn-less-width" style="float:left;">Back</button>
                                    <button id="btnCreateAccount" data-role="none" class="btn btn-primary btn-less-width" style="float:right;">CREATE ACCOUNT</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
            <div id="divSuccess" subpage="pageSignin" style="display:none;">
                <div class="row">
                    <div class="col-md-offset-3 col-sm-6">
                        <h2 style="color:#00a200;">Your account was successfully created</h2>
                        <br />
                        <h4>
                            Your vendor account needs to be verified by you and the online platform admin to make sure that your account really belongs to you (and not a bot or someone pretending to be you.)
                            <br /><br />
                            On your side, please check your email and verify your account by clicking on the activation link we’ve emailed you. We’ve sent the verification link to your registered email at: <b id="lblSentTo"></b>
                            <br /><br />
                            The platform administrator will also check your registration and approve it if everything looks fine.
                            <br /><br />
                            Thank you for registering.
                        </h4>
                        <br />
                        <button data-role="none" class="btn btn-primary btn-less-width btnGoToLogin" style="float:left;">GO TO LOGIN</button>
                    </div>
                </div>
            </div>
            <div id="divForgotPassword" subpage="pageSignin" style="display:none;">
                <div class="row">
                    <div class="col-md-offset-3 col-md-6 sign-in">
                        <div id="divFPUser" subpage="divForgotuser" style="display:none;">
                            <div>
                                <div>
                                    <b>1. Details</b>
                                    <span style="color:#CCC">
                                        &rarr; 2. Security Questions
                                        &rarr;
                                        3 .Reset
                                    </span>
                                </div>
                                <hr />
                                <h4>Forgot Password</h4>
                                <p class="text title-tag-line">Please fill the required fields</p>

                                <div class="form-group">
                                    <label class="info-title">First Name <span>*</span></label>
                                    <input data-role="none" autofocus type="text" class="form-control unicase-form-control text-input" id="txtFPFirstname">
                                </div>
                                <div class="form-group">
                                    <label class="info-title">Middle Name</label>
                                    <input data-role="none" autofocus type="text" class="form-control unicase-form-control text-input" id="txtFPMiddlename">
                                </div>
                                <div class="form-group">
                                    <label class="info-title">Last Name <span>*</span></label>
                                    <input data-role="none" autofocus type="text" class="form-control unicase-form-control text-input" id="txtFPLastname">
                                </div>
                                <div class="form-group">
                                    <label class="info-title" for="selFPPersonGenSuffix">Generational Suffix</label>
                                    <select data-role="none" id="selFPPersonGenSuffix" class="form-control unicase-form-control text-input">
                                        <option value="">None</option>
                                        <option value="Sr.">Sr.</option>
                                        <option value="Jr.">Jr.</option>
                                        <option value="I">I</option>
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
                                </div>
                                <div class="form-group">
                                    <label>Please provide the email address you used during registration</label>
                                    <input data-role="none" autofocus type="text" class="form-control unicase-form-control text-input" id="txtFPEmailAdd">
                                </div>
                                <div id="divFPErrMsg" style="color:#ff3737; text-align:center; font-size:14px; margin-bottom:25px; display:none;">Please complete the required field(s)</div>
                                <div style="clear:both;">
                                    <button id="btnFPBack" data-role="none" class="btn btn-default btn-less-width" style="float:left;">Back</button>
                                    <button id="btnFPNext" data-role="none" class="btn btn-primary btn-less-width" style="float:right;">NEXT</button>
                                </div>
                            </div>
                        </div>
                        <div id="divFPSecQ" subpage="divForgotuser" style="display:none;">
                            <div>
                                <div>
                                    <b style="color:#00a200;">1. Details</b>
                                    &rarr;
                                    <b>2. Security Questions</b>
                                    <span style="color:#CCC;">
                                        &rarr; 3 .Reset
                                    </span>
                                </div>
                                <hr />
                                <h4>Your Security Questions</h4>
                                <p class="text title-tag-line">Please provide your answers of your security questions</p>

                                <div class="form-group">
                                    <label id="lblSecQ1"></label>
                                    <input data-role="none" autofocus type="text" class="form-control unicase-form-control text-input" id="txtFPSecQ1Ans">
                                </div>

                                <div class="form-group">
                                    <label id="lblSecQ2"></label>
                                    <input data-role="none" autofocus type="text" class="form-control unicase-form-control text-input" id="txtFPSecQ2Ans">
                                </div>
                                <div id="divFPSecQErrMsg" style="color:#ff3737; text-align:center; font-size:14px; margin-bottom:25px; display:none;">Please complete the required field(s)</div>
                                <div style="clear:both;">
                                    <button id="btnFPSecQBack" data-role="none" class="btn btn-default btn-less-width" style="float:left;">Back</button>
                                    <button id="btnFPSecQNext" data-role="none" class="btn btn-primary btn-less-width" style="float:right;">NEXT</button>
                                </div>
                            </div>
                        </div>
                        <div id="divFPResetPassword" subpage="divForgotuser" style="display:none;">
                            <div>
                                <div>
                                    <b style="color:#00a200;">1. Details &rarr; 2. Security Questions</b>
                                    <b> &rarr; 3. Reset</b>
                                </div>
                                <hr />
                                <h4>Reset Your Account</h4>
                                <p class="text title-tag-line">Please fill the required fields</p>

                                <div class="form-group">
                                    <label>New Username <span>*</span></label>
                                    <input data-role="none" autofocus type="text" class="form-control unicase-form-control text-input" id="txtFPNewUsername">
                                </div>

                                <div class="form-group">
                                    <label>New Password <span>*</span></label>
                                    <input data-role="none" autofocus type="password" class="form-control unicase-form-control text-input" id="txtFPNewPassword">
                                </div>
                                <div id="divFPResetErrMsg" style="color:#ff3737; text-align:center; font-size:14px; margin-bottom:25px; display:none;">Please complete the required field(s)</div>
                                <div style="clear:both;">
                                    <button id="btnFPResetBack" data-role="none" class="btn btn-default btn-less-width" style="float:left;">Back</button>
                                    <button id="btnFPResetSave" data-role="none" class="btn btn-primary btn-less-width" style="float:right;">SAVE CHANGES</button>
                                </div>
                            </div>
                        </div>
                        <div id="divFPSuccess" subpage="divForgotuser" style="display:none;">
                            <h2 style="color:#00a200;">Yehey! Your account was successfully updated.</h2>
                            <br />
                            <button data-role="none" class="btn btn-primary btn-less-width btnGoToLogin" style="float:left;">GO TO LOGIN</button>
                        </div>
                    </div>
                </div>
            </div>
        </div><!-- /.sigin-in-->
    </div>

    @*<table style="width:100%; border:none; background-color: #f5f5f5;">
        <tr><td style="height:150px; text-align:center; padding:20px; vertical-align:middle; background-color: #c52c2c; font-size: 50px; color: #FFF;">Security Alert</td></tr>
        <tr><td style="padding:20px; font-size: 20px;">Hello Juan,<br /><br />Your Mobiz Ecommerce account has been reset!<br /><br />Date: March 15, 2018</td></tr>
        <tr><td style="padding:20px; vertical-align:middle; background-color: #dadada; font-size: 16px;">To know more about Mobiz, please <a href="http://mobiz-apps.com" target="_blank">click here</a></td></tr>
    </table>*@
</div>

<div data-role="none" class="modal fade" id="mymodal" tabindex="-1" role="dialog">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                <h4 data-role="none" class="modal-title">Modal title</h4>
            </div>
            <div class="modal-body">
                <p>One fine body&hellip;</p>
            </div>
            <div class="modal-footer">
                <button data-role="none" type="button" class="btn btn-default btn-less-width" data-dismiss="modal">Close</button>
                <button data-role="none" type="button" class="btn btn-primary btn-less-width">Save changes</button>
            </div>
        </div><!-- /.modal-content -->
    </div><!-- /.modal-dialog -->
</div><!-- /.modal -->