@Code
    ViewData("Title") = "Ecommerce"
    Layout = "~/Views/Shared/EcommerceLayout.vbhtml"
End Code
@Section head
    <script type="text/javascript">
        //var additionalData = @Html.Raw(Json.Encode(ViewData("BusinessIndustries")));
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
                require(['/Scripts/sources/Ecommerce/Ecommerce.js?crc=' + assetversion], function () {
                    require(['/Scripts/sources/Ecommerce/Signin.js?crc=' + assetversion])
                });
            })
        })
    </script>
End Section
<div class="body-content outer-top-xs" id="top-banner-and-menu">
    <div class="container">
        <div class="sign-in-page bg-white border p-3 rounded">
            <div id="divSignin" subpage="pageSignin" style="display:block;">
                <div class="row">
                    <!-- Sign-in -->
                    <div class="col-md-6 col-sm-6 sign-in">
                        <div id="divConfirmedEmail" style="display:none;" class="alert alert-success" role="alert">You have confirmed your email address. You may now signin.</div>
                        <h4 class="">Sign in</h4>
                        <p class="">Hello, Welcome to your account.</p>
                        @*<div class="social-sign-in outer-top-xs">
                    <a data-role="none" href="#" class="facebook-sign-in"><i class="fa fa-facebook"></i> Sign In with Facebook</a>
                    <a data-role="none" href="#" class="twitter-sign-in"><i class="fa fa-twitter"></i> Sign In with Twitter</a>
                </div>*@
                        <form id="formLogin" class="register-form outer-top-xs" role="form">
                            <div id="divErrLoginMsg" style="color:#ff3737; text-align:center; font-size:14px; margin-bottom:25px; display:none;">Invalid username / password</div>
                            <div class="form-group mb-3">
                                <label class="info-title" for="txtuname">Username <span class="required">*</span></label>
                                <input data-role="none" type="text" class="form-control form-control-lg" id="txtuname">
                            </div>
                            <div class="form-group mb-3">
                                <label class="info-title" for="txtpword">Password <span class="required">*</span></label>
                                <input data-role="none" type="password" class="form-control form-control-lg" id="txtpword">
                            </div>
                            <div class="radio outer-xs">
                                <button data-role="none" type="submit" class="btn btn-primary btn-lg">Login</button>
                                <br /><br />
                                <a data-role="none" id="btnForgotPassword" href="#" class="forgot-password pull-left">Forgot your Password?</a>
                            </div>
                        </form>
                    </div>
                    <!-- Sign-in -->
                    <!-- create a new account -->
                    <div class="col-md-6 col-sm-6 create-new-account">
                        <h4 class="checkout-subtitle">Create a new account</h4>
                        <p class="text title-tag-line">Options to create your account.</p>
                        <div class="register-form outer-top-xs">
                            <div class="form-group mb-3">
                                <label class="info-title">&nbsp;</label>
                                <div class="btn-group" style="width:100%;">
                                    <button data-role="none" type="button" class="btn btn-warning  btn-lg dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                        I'm a Customer <span class="caret"></span>
                                    </button>
                                    <ul data-role="none" class="dropdown-menu">
                                        <li class="dropdown-header">Create account as:</li>
                                        <li><a id="btnPersonalAcc" class="dropdown-item" data-role="none" href="#">Personal</a></li>
                                        <li><a id="btnCompanyAcc" class="dropdown-item" data-role="none" href="#">Company</a></li>
                                    </ul>
                                </div>
                            </div>
                            <div class="form-group mb-3">
                                <div class="info-title" style="text-align:center; width:100%; color:#797979;">OR</div>
                                <button id="btnCreateAccountSupplier" data-role="none" class="btn btn-success btn-lg btn-block">I'm a Supplier</button>
                            </div>
                        </div>
                    </div>
                    <!-- create a new account -->
                </div><!-- /.row -->
            </div>            
            <div id="divCreation" subpage="pageSignin" style="display:none;">
                @*<a id="btnBackToLogin" data-role="none" href="#" style="font-size:16px;">
                    <span class="glyphicon glyphicon-chevron-left" style="font-size:14px;"></span>
                    GO BACK TO LOGIN
                </a>
                <br /><br />*@
                <div class="row">
                    <div class="offset-md-3 col-md-6 sign-in">
                        <h2 id="lblSignupAccountType">Signup as Customer</h2>
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
                            <form class="register-form outer-top-xs" role="form">
                                <div class="form-group mb-3">
                                    <label class="info-title" for="txtCompanyName">Company Name <span class="required">*</span></label>
                                    <input data-role="none" type="text" class="form-control" id="txtCompanyName">
                                </div>

                                <div class="form-group mb-3">
                                    <label class="info-title" for="txtCompanyTIN">TIN</label>
                                    <input data-role="none" type="text" class="form-control" id="txtCompanyTIN">
                                </div>

                                <div class="form-group mb-3">
                                    <label class="info-title" for="txtBusInd">Business Industry <span class="required">*</span></label>
                                    <input data-role="none" type="text" class="form-control" id="txtBusInd">
                                    <div id="listBusinessIndustry" class="list-group" style="display:none; position: absolute; max-width: 100%; box-shadow: 0px 5px 28px #CCC; overflow-y:auto; max-height:300px;"></div>
                                </div>                                

                                <div class="form-group mb-3">
                                    <label class="info-title" for="txtCompanyEmailAdd">Company Email Address <span class="required">*</span></label>
                                    <input data-role="none" type="text" class="form-control" id="txtCompanyEmailAdd">
                                </div>

                                <div class="form-inline">
                                    <div class="form-group mb-3">
                                        <label class="info-title" for="txtCompanyMobileNo">Company Mobile Number <span class="required">*</span></label>
                                        <div class="input-group">
                                            <input data-role="none" type="text" value="+63" maxlength="3" class="form-control" disabled style="width:50px;">
                                            <input data-role="none" type="text" placeholder="9**********" maxlength="10" class="form-control" id="txtCompanyMobileNo">
                                        </div>
                                    </div>
                                </div>                                

                                <h4>Company's Main Address</h4>

                                <div class="form-group mb-3">
                                    <label class="info-title" for="txtCompanyAddressBldgNo">House/Building Number <span class="required">*</span></label>
                                    <input data-role="none" type="text" class="form-control" id="txtCompanyAddressBldgNo">
                                </div>

                                <div class="form-group mb-3">
                                    <label class="info-title" for="txtCompanyAddressStreetName">Street Name <span class="required">*</span></label>
                                    <input data-role="none" type="text" class="form-control" id="txtCompanyAddressStreetName">
                                </div>

                                <div class="form-group mb-3">
                                    <label class="info-title" for="selCompanyAddressCity">City <span class="required">*</span></label>
                                    <select data-role="none" id="selCompanyAddressCity" class="form-select"></select>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="info-title" for="selCompanyAddressCountry">Country <span class="required">*</span></label>
                                    <select data-role="none" id="selCompanyAddressCountry" class="form-select"></select>
                                </div>
                                
                                <div id="divCompanyErrMsg" style="color:#ff3737; text-align:center; font-size:14px; margin-bottom:25px; display:none;">Please complete the required field(s)</div>

                                <div class="mt-5">
                                    <button id="btnBackToSignin" data-role="none" class="btn btn-secondary" style="float:left;">Back</button>
                                    <button id="btnNextStep" data-role="none" class="btn btn-success" style="float:right;">NEXT STEP</button>
                                </div>                                
                            </form>
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
                            <form class="register-form outer-top-xs" role="form">
                                <div class="form-group mb-3">
                                    <label class="info-title" for="txtPersonFirstName">First Name <span class="required">*</span></label>
                                    <input data-role="none" type="text" class="form-control" id="txtPersonFirstName">
                                </div>

                                <div class="form-group mb-3">
                                    <label class="info-title" for="txtPersonMiddleName">Middle Name</label>
                                    <input data-role="none" type="text" class="form-control" id="txtPersonMiddleName">
                                </div>

                                <div class="form-group mb-3">
                                    <label class="info-title" for="txtPersonLastName">Last Name <span class="required">*</span></label>
                                    <input data-role="none" type="text" class="form-control" id="txtPersonLastName">
                                </div>

                                <div class="form-group mb-3">
                                    <label class="info-title" for="selPersonGenSuffix">Generational Suffix</label>
                                    <select data-role="none" id="selPersonGenSuffix" class="form-select">
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
                                    <div class="form-group mb-3">
                                        <label class="info-title">Date of Birth <span class="required">*</span></label>
                                        <div class="input-group">
                                            <select data-role="none" id="selPersonBirthMonth" class="form-select fixed">
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
                                            <select data-role="none" id="selPersonBirthDay" class="form-select fixed"></select>
                                            <input data-role="none" type="text" placeholder="Year" maxlength="4" class="form-control" id="txtPersonBirthYear" style="width:70px;">
                                        </div>                                        
                                    </div>
                                </div>                                

                                <div class="form-group mb-3">
                                    <label class="info-title" for="txtPersonEmailAdd">Email Address <span class="required">*</span></label>
                                    <p class="text title-tag-line"><b>NOTE</b>: You must remember the email address you enter. This will be used to reset your account in case you forget your password.</p>
                                    <input data-role="none" type="text" class="form-control" id="txtPersonEmailAdd">
                                </div>

                                <h4>Home Address</h4>
                                <div class="form-group mb-3">
                                    <label class="info-title" for="txtPersonAddressBldgNo">House/Building Number <span class="required">*</span></label>
                                    <input data-role="none" type="text" class="form-control" id="txtPersonAddressBldgNo">
                                </div>

                                <div class="form-group mb-3">
                                    <label class="info-title" for="txtPersonAddressStreetName">Street Name <span class="required">*</span></label>
                                    <input data-role="none" type="text" class="form-control" id="txtPersonAddressStreetName">
                                </div>

                                <div class="form-group mb-3">
                                    <label class="info-title" for="selPersonAddressCity">City <span class="required">*</span></label>
                                    <select data-role="none" id="selPersonAddressCity" class="form-select"></select>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="info-title" for="selPersonAddressCountry">Country <span class="required">*</span></label>
                                    <select data-role="none" id="selPersonAddressCountry" class="form-select"></select>
                                </div>

                                <h4>Login Credentials</h4>
                                <div class="form-group mb-3">
                                    <label class="info-title" for="txtPersonUsername">User Name <span class="required">*</span> (minimum of 8 characters)</label>
                                    <input data-role="none" type="text" class="form-control" id="txtPersonUsername">
                                </div>

                                <div class="form-group mb-3">
                                    <label class="info-title" for="txtPersonPassword">Password <span class="required">*</span> (minimum of 8 characters)</label>
                                    <input data-role="none" type="password" class="form-control" id="txtPersonPassword">
                                </div>

                                <div class="form-group mb-3">
                                    <label class="info-title" for="txtPersonConfirmPassword">Confirm Password <span class="required">*</span></label>
                                    <input data-role="none" type="password" class="form-control" id="txtPersonConfirmPassword">
                                </div>

                                <h4>Security Questions</h4>
                                <p class="text title-tag-line"><b>NOTE</b>: You must remember the answers to the security questions you selected. This will be used to reset your account in case you forget your password.</p>
                                
                                <div class="form-group mb-3">
                                    <label class="info-title" for="selSecQuestion1">Question No. 1 <span class="required">*</span></label>
                                    <div class="input-group">
                                        <select id="selSecQuestion1" data-role="none" class="form-select">
                                            <option value="">Please Select</option>
                                            <option value="Where is your mother's birthplace?">Where is your mother's birthplace?</option>
                                            <option value="Who is your best childhood friend?">Who is your best childhood friend?</option>
                                            <option value="What is the name of your first pet?">What is the name of your first pet?</option>
                                            <option value="Who is your favorite teacher?">Who is your favorite teacher?</option>
                                            <option value="Who is your favorite historical person?">Who is your favorite historical person?</option>
                                            <option value="What is your grandfather's occupation?">What is your grandfather's occupation?</option>
                                        </select>
                                        <input data-role="none" type="text" placeholder="Answer of Question No. 1" class="form-control" id="txtSecQuestion1Answer">
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="info-title" for="selSecQuestion2">Question No. 2 <span class="required">*</span></label>
                                    <div class="input-group">
                                        <select id="selSecQuestion2" data-role="none" class="form-select">
                                            <option value="">Please Select</option>
                                            <option value="Where is your mother's birthplace?">Where is your mother's birthplace?</option>
                                            <option value="Who is your best childhood friend?">Who is your best childhood friend?</option>
                                            <option value="What is the name of your first pet?">What is the name of your first pet?</option>
                                            <option value="Who is your favorite teacher?">Who is your favorite teacher?</option>
                                            <option value="Who is your favorite historical person?">Who is your favorite historical person?</option>
                                            <option value="What is your grandfather's occupation?">What is your grandfather's occupation?</option>
                                        </select>
                                        <input data-role="none" type="text" placeholder="Answer of Question No. 2" class="form-control" id="txtSecQuestion2Answer">
                                    </div>
                                </div>

                                <div id="divErrMsg" style="color:#ff3737; text-align:center; font-size:14px; margin-bottom:25px; display:none;">Please complete the required field(s)</div>

                                <div style="clear:both;" class="mt-5">
                                    <button id="btnBackToCompany" data-role="none" class="btn btn-secondary" style="float:left;">Back</button>
                                    <button id="btnCreateAccount" data-role="none" class="btn btn-primary" style="float:right;">CREATE ACCOUNT</button>
                                </div>                                
                            </form>
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
                            We sent an activation link to <b id="lblSentTo"></b>.
                            To be able to use your account, please use the sent link to activate your account.
                        </h4>
                        <br />
                        <button data-role="none" class="btn btn-primary btnGoToLogin" style="float:left;">GO TO LOGIN</button>
                    </div>
                </div>
            </div>
            <div id="divForgotPassword" subpage="pageSignin" style="display:none;">
                <div class="row">
                    <div class="col-md-offset-3 col-md-6 sign-in">
                        <div id="divFPUser" subpage="divForgotuser" style="display:none;">
                            <form>
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

                                <div class="form-group mb-3">
                                    <label class="info-title">First Name <span class="required">*</span></label>
                                    <input data-role="none" autofocus type="text" class="form-control" id="txtFPFirstname">
                                </div>
                                <div class="form-group mb-3">
                                    <label class="info-title">Middle Name</label>
                                    <input data-role="none" autofocus type="text" class="form-control" id="txtFPMiddlename">
                                </div>
                                <div class="form-group mb-3">
                                    <label class="info-title">Last Name <span class="required">*</span></label>
                                    <input data-role="none" autofocus type="text" class="form-control" id="txtFPLastname">
                                </div>
                                <div class="form-group mb-3">
                                    <label class="info-title" for="selFPPersonGenSuffix">Generational Suffix</label>
                                    <select data-role="none" id="selFPPersonGenSuffix" class="form-select">
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
                                <div class="form-group mb-3">
                                    <label>Please provide the email address you used during registration</label>
                                    <input data-role="none" autofocus type="text" class="form-control" id="txtFPEmailAdd">
                                </div>
                                <div id="divFPErrMsg" style="color:#ff3737; text-align:center; font-size:14px; margin-bottom:25px; display:none;">Please complete the required field(s)</div>
                                <div style="clear:both;">
                                    <button id="btnFPBack" data-role="none" class="btn btn-secondary" style="float:left;">Back</button>
                                    <button id="btnFPNext" data-role="none" class="btn btn-success" style="float:right;">NEXT</button>
                                </div>
                            </form>
                        </div>
                        <div id="divFPSecQ" subpage="divForgotuser" style="display:none;">
                            <form>
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
                                
                                <div class="form-group mb-3">
                                    <label id="lblSecQ1"></label>
                                    <input data-role="none" autofocus type="text" class="form-control" id="txtFPSecQ1Ans">
                                </div>

                                <div class="form-group mb-3">
                                    <label id="lblSecQ2"></label>
                                    <input data-role="none" autofocus type="text" class="form-control" id="txtFPSecQ2Ans">
                                </div>
                                <div id="divFPSecQErrMsg" style="color:#ff3737; text-align:center; font-size:14px; margin-bottom:25px; display:none;">Please complete the required field(s)</div>
                                <div style="clear:both;">
                                    <button id="btnFPSecQBack" data-role="none" class="btn btn-secondary" style="float:left;">Back</button>
                                    <button id="btnFPSecQNext" data-role="none" class="btn btn-success" style="float:right;">NEXT</button>
                                </div>
                            </form>
                        </div>
                        <div id="divFPResetPassword" subpage="divForgotuser" style="display:none;">
                            <form>
                                <div>
                                    <b style="color:#00a200;">1. Details &rarr; 2. Security Questions</b>
                                    <b> &rarr; 3. Reset</b>
                                </div>
                                <hr />
                                <h4>Reset Your Account</h4>
                                <p class="text title-tag-line">Please fill the required fields</p>
                                
                                <div class="form-group mb-3">
                                    <label>New Username <span class="required">*</span></label>
                                    <input data-role="none" autofocus type="text" class="form-control" id="txtFPNewUsername">
                                </div>

                                <div class="form-group mb-3">
                                    <label>New Password <span class="required">*</span></label>
                                    <input data-role="none" autofocus type="password" class="form-control" id="txtFPNewPassword">
                                </div>
                                <div id="divFPResetErrMsg" style="color:#ff3737; text-align:center; font-size:14px; margin-bottom:25px; display:none;">Please complete the required field(s)</div>
                                <div style="clear:both;">
                                    <button id="btnFPResetBack" data-role="none" class="btn btn-secondary" style="float:left;">Back</button>
                                    <button id="btnFPResetSave" data-role="none" class="btn btn-primary" style="float:right;">SAVE CHANGES</button>
                                </div>
                            </form>
                        </div>
                        <div id="divFPSuccess" subpage="divForgotuser" style="display:none;">
                            <h2 style="color:#00a200;">Yehey! Your account was successfully updated.</h2>
                            <br />
                            <button data-role="none" class="btn btn-primary btnGoToLogin" style="float:left;">GO TO LOGIN</button>
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
                <button data-role="none" type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                <button data-role="none" type="button" class="btn btn-primary">Save changes</button>
            </div>
        </div><!-- /.modal-content -->
    </div><!-- /.modal-dialog -->
</div><!-- /.modal -->