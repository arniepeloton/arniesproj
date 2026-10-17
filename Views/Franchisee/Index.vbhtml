@Code
    ViewData("Title") = "Franchisee Registration"

    If Request.IsAjaxRequest() Then
        Layout = ""
    Else
        Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
    End If
End Code

@Section head

    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
                require([GetPathFormat('/Scripts/sources/Franchisee/Application.js?v=' + assetversion)]);
            })
        })
    </script>

    <style type="text/css">
        .ui-page-active {
            background-color: #f8f9fa !important;
        }
    </style>
End Section

<div class="container">
    <div class="row pt-3 pb-3">
        <div class="offset-md-3 col-md-6 bg-white rounded p-3 border">
            <h2 id="lblSignupAccountType" class="text-primary">Franchisee POS Registration</h2>
            <div>
                You are the franchisee of <b>@ViewData("FranchisorName")</b>. Please fill in all the information needed.
            </div>
            <br />
            <div id="divCompanyAcc" subpage="divCreation" style="display:block;">
                <div>
                    <b>1. Company Details</b>
                    <span style="color:#CCC">
                        &rarr; 2. Store Details
                    </span>
                    <span style="color:#CCC">
                        &rarr; 3. Administrator Account
                    </span>
                </div>
                <hr />
                <h4>Company Details</h4>
                <p class="text title-tag-line">Fields with <span class="required">*</span> are required</p>
                <form class="register-form outer-top-xs" role="form">
                    <div class="form-group mb-3">
                        <label class="info-title" for="txtCompanyName">Company Name <span class="required">*</span></label>
                        <input data-role="none" type="text" class="form-control" id="txtCompanyName">
                    </div>

                    <div class="form-group mb-3">
                        <label class="info-title" for="txtCompanyTIN">TIN <span class="required">*</span></label>
                        <input data-role="none" type="text" class="form-control" id="txtCompanyTIN">
                    </div>

                    <div class="form-group mb-3">
                        <label class="info-title" for="txtBusInd">Business Industry <span class="required">*</span></label>
                        <input data-role="none" type="text" class="form-control" id="txtBusInd">
                        <div id="listBusinessIndustry" class="list-group" style="display:none; max-width: 100%; box-shadow: 0px 5px 28px #CCC; overflow-y:auto; max-height:300px;"></div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="info-title" for="txtCompanyEmailAdd">Company Email Address <span class="required">*</span></label>
                        <input data-role="none" type="text" class="form-control" id="txtCompanyEmailAdd">
                    </div>

                    <div class="form-inline">
                        <div class="form-group mb-3">
                            <label class="info-title" for="txtCompanyMobileNo">Company Mobile Number <span class="required">*</span></label>
                            <div class="input-group">
                                <span class="input-group-text">+63</span>
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

                    <div class="mt-5 text-center">
                        <button id="btnNextStep" data-role="none" class="btn btn-success">NEXT STEP</button>
                    </div>
                </form>
            </div>

            <div id="divStoreDetails" subpage="divCreation" style="display:none;">
                <div>
                    <b style="color:#00a200;">1. Company Details</b>
                    &rarr;
                    <b>2. Store Details</b>
                    <span style="color:#CCC">
                        &rarr; 3. Administrator Account
                    </span>
                </div>
                <hr />
                <h4>Store Details</h4>
                <p class="text title-tag-line">Fields with <span class="required">*</span> are required</p>
                <form class="register-form outer-top-xs" role="form">
                    <div class="form-group mb-3">
                        <label class="info-title" for="txtStoreName">Store Name <span class="required">*</span></label>
                        <input data-role="none" type="text" class="form-control" id="txtStoreName">
                    </div>

                    <div class="form-group mb-3">
                        <label class="info-title" for="selStoreBusinessName">Business or Trade Name <span class="required">*</span></label>
                        <select data-role="none" id="selStoreBusinessName" class="form-select"></select>
                    </div>

                    <h4>Store Address</h4>

                    <div class="form-group mb-3">
                        <label class="info-title" for="txtStoreAddressBldgNo">House/Building Number <span class="required">*</span></label>
                        <input data-role="none" type="text" class="form-control" id="txtStoreAddressBldgNo">
                    </div>

                    <div class="form-group mb-3">
                        <label class="info-title" for="txtStoreAddressStreetName">Street Name <span class="required">*</span></label>
                        <input data-role="none" type="text" class="form-control" id="txtStoreAddressStreetName">
                    </div>

                    <div class="form-group mb-3">
                        <label class="info-title" for="selStoreAddressCity">City <span class="required">*</span></label>
                        <select data-role="none" id="selStoreAddressCity" class="form-select"></select>
                    </div>

                    <div class="form-group mb-3">
                        <label class="info-title" for="selStoreAddressCountry">Country <span class="required">*</span></label>
                        <select data-role="none" id="selStoreAddressCountry" class="form-select"></select>
                    </div>

                    <div id="divStoreErrMsg" style="color:#ff3737; text-align:center; font-size:14px; margin-bottom:25px; display:none;">Please complete the required field(s)</div>

                    <div class="mt-5 text-center">
                        <button id="btnStoreBack" data-role="none" class="btn btn-secondary">Back</button>
                        <button id="btnStoreNextStep" data-role="none" class="btn btn-success">NEXT STEP</button>
                    </div>
                </form>
            </div>

            <div id="divPersonalAcc" subpage="divCreation" style="display:none;">
                <div id="divBreadCrumbsForCompany">
                    <b style="color:#00a200;">1. Company Details</b>
                    &rarr;
                    <b style="color:#00a200;">2. Store Details</b>
                    &rarr;
                    <b>3. Administrator Account</b>
                    <hr />
                </div>
                <h4 id="titlePersonalAcc">Personal Account</h4>
                <p class="text title-tag-line">Fields with <span class="required">*</span> are required</p>
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
                        <p class="text title-tag-line"><b>NOTE</b>: Please enter a working email address. We will notify you thru e-mail once your registration is approved.</p>
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

    <div class="container text-center mb-5 mt-5">
        Powered by 1Mobiz Inc.
        <a data-role="none" class="custom-link" href="http://mobiz-apps.com/" target="_blank">www.mobiz-apps.com</a>
    </div>
</div>