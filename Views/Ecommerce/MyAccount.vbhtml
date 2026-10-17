@Code
    ViewData("Title") = "Ecommerce"
    Layout = "~/Views/Shared/EcommerceLayout.vbhtml"
End Code
@Section head
    <script type="text/javascript">
        //var myaccountdetails = JSON.parse(@Html.Raw(Json.Encode(ViewData("MyAccountDetails"))));
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion, 'promise!/Scripts/sources/Ecommerce/ImageViewer.js?crc=' + assetversion], function () {
                require(['/Scripts/sources/Ecommerce/Ecommerce.js?crc=' + assetversion], function () {
                    require(['/Scripts/sources/Ecommerce/MyAccount.js?crc=' + assetversion])
                });
            })
        })
    </script>
End Section
<style type="text/css">
    .menu-list li{
        font-size:16px;
        color: #5a5a5a;
        padding:10px;
        cursor:pointer;
        border-top:1px solid #CCC;
        border-bottom:1px solid #CCC;
    }

    .menu-list li:first-child,
    .menu-list li:last-child {
        border:none !important;
    }

    .menu-list li:hover,
    .menu-list li.active {
        background-color: #0083c1;
        color: #FFF;
        text-shadow: none;
    }

    .badge {
        padding: 4px;
        border-radius: 10px;
        font-size: 12px;
        color: #FFF;
        text-shadow: none;
    }

    .badge.ba {
        background-color: #ff9800;
    }

    .badge.da {
        background-color: #558b2f;
    }
</style>
<div class="body-content outer-top-xs" id="top-banner-and-menu">
    <div class="container-fluid">
        <div class="">
            <div class="row">
                <div class="col-md-3">
                    <div class="bg-white border p-3 rounded">
                        <ul id="navMyAccount" class="nav nav-pills flex-column mb-auto">
                            <li class="nav-item active" id="btnMAMyProfile"><a data-role="none" class="nav-link active" href="">My Account</a></li>
                            <li class="nav-item" id="btnMAAddressBook"><a data-role="none" class="nav-link" href="">Address Book</a></li>
                            <li class="nav-item" id="btnMAOrderHistory"><a data-role="none" class="nav-link" href="">Order History</a></li>
                            <li class="nav-item" id="btnMAAccountsPayable"><a data-role="none" class="nav-link" href="">Accounts Payable</a></li>
                        </ul>
                    </div>                    
                </div>
                <div class="col-md-9">
                    <div class="bg-white border p-3 rounded">
                        <div id="divMyProfile" subpage="pageMyAccount" style="display:none;">
                            <h1 class="display-6 mb-4 text-secondary">My Account</h1>

                            <ul class="nav nav-tabs" id="nav-tab" role="tablist">
                                <li class="nav-item hidetab">
                                    <button data-role="none" class="nav-link active" data-toggle="tab" data-target="#tabCompanyProfile">Company Profile</button>
                                </li>
                                <li class="nav-item">
                                    <button data-role="none" class="nav-link" id="btnTabILADHistory" data-toggle="tab" data-target="#tabAdminInfo">Administrator Info</button>
                                </li>
                            </ul>

                            <div class="tab-content">
                                <div id="tabCompanyProfile" class="tab-pane fade show active p-3">
                                    <div class="row mb-3">
                                        <label class="col-md-2 col-form-label mb-3" for="txtMACompanyName">Company Name: <span class="required">*</span></label>
                                        <div class="col-md-10">
                                            <input data-role="none" type="text" class="form-control" id="txtMACompanyName">
                                        </div>
                                    </div>

                                    <div class="row mb-3">
                                        <label class="col-md-2 col-form-label mb-3" for="txtMACompanyTIN">TIN:</label>
                                        <div class="col-md-10">
                                            <input data-role="none" type="text" class="form-control" id="txtMACompanyTIN">
                                        </div>
                                    </div>

                                    <div class="row mb-3">
                                        <label class="col-md-2 col-form-label mb-3" for="txtMABusInd">Business Industry: <span class="required">*</span></label>
                                        <div class="col-md-10">
                                            <input data-role="none" type="text" class="form-control" id="txtMABusInd" readonly>
                                            <div id="listBusinessIndustry" class="list-group" style="display:none; position: absolute; max-width: 100%; box-shadow: 0px 5px 28px #CCC; overflow-y:auto; max-height:300px;"></div>
                                        </div>
                                    </div>

                                    <div class="row mb-3">
                                        <label class="col-md-2 col-form-label mb-3" for="txtMACompanyEmailAdd">Company Email Address: <span class="required">*</span></label>
                                        <div class="col-md-10">
                                            <input data-role="none" type="text" class="form-control" id="txtMACompanyEmailAdd">
                                        </div>
                                    </div>

                                    <div class="row mb-3">
                                        <label class="col-md-2 col-form-label mb-3" for="txtMACompanyMobileNo">Company Mobile Address: <span class="required">*</span></label>
                                        <div class="col-md-10">
                                            <div class="input-group">
                                                <input data-role="none" type="text" value="+63" maxlength="3" class="form-control" disabled style="width:50px;">
                                                <input data-role="none" type="text" placeholder="9**********" maxlength="10" class="form-control" id="txtMACompanyMobileNo" style="width:110px;">
                                            </div>
                                        </div>
                                    </div>

                                    <h3 class="mb-3">Company's Main Address</h3>

                                    <div class="row mb-3">
                                        <label class="col-md-2 col-form-label mb-3" for="txtMACompanyAddressBldgNo">House/Building Number: <span class="required">*</span></label>
                                        <div class="col-md-10">
                                            <input data-role="none" type="text" class="form-control" id="txtMACompanyAddressBldgNo">
                                        </div>
                                    </div>

                                    <div class="row mb-3">
                                        <label class="col-md-2 col-form-label mb-3" for="txtMACompanyAddressStreetName">Street Name: <span class="required">*</span></label>
                                        <div class="col-md-10">
                                            <input data-role="none" type="text" class="form-control" id="txtMACompanyAddressStreetName">
                                        </div>
                                    </div>

                                    <div class="row mb-3">
                                        <label class="col-md-2 col-form-label mb-3" for="selMACompanyAddressCity">City: <span class="required">*</span></label>
                                        <div class="col-md-10">
                                            <select data-role="none" id="selMACompanyAddressCity" class="form-select"></select>
                                        </div>
                                    </div>

                                    <div class="row mb-3">
                                        <label class="col-md-2 col-form-label mb-3" for="selMACompanyAddressCountry">Country: <span class="required">*</span></label>
                                        <div class="col-md-10">
                                            <select data-role="none" id="selMACompanyAddressCountry" class="form-select"></select>
                                        </div>
                                    </div>
                                </div>
                                <div id="tabAdminInfo" class="tab-pane fade p-3">
                                    <div class="row mb-3">
                                        <div class="col-auto mb-3">
                                            <div style="width: 80px; height: 80px; border-radius: 50%; background-color: #CCC; text-align: center; vertical-align: middle; display: table-cell; color: #FFF; font-size: 30px;">
                                                <span class="bi-person-fill"></span>
                                            </div>
                                        </div>
                                        <div class="col">
                                            <div id="divActualName" subpage="divMAName" style="display:block;">
                                                <a data-role="none" id="btnMAEditName" href="#">Edit Name</a>
                                                <h3 id="txtMAFullName"></h3>
                                            </div>
                                            <div id="divEditName" subpage="divMAName" style="display:none;">
                                                <div class="mb-3">
                                                    <label for="txtMAFirstName" class="form-label">First Name: <span class="required">*</span></label>
                                                    <input data-role="none" type="text" class="form-control" id="txtMAFirstName">
                                                </div>
                                                <div class="mb-3">
                                                    <label for="txtMAMiddleName" class="form-label">Middle Name:</label>
                                                    <input data-role="none" type="text" class="form-control" id="txtMAMiddleName">
                                                </div>
                                                <div class="mb-3">
                                                    <label for="txtMALastName" class="form-label">Last Name: <span class="required">*</span></label>
                                                    <input data-role="none" type="text" class="form-control" id="txtMALastName">
                                                </div>
                                                <div class="mb-3">
                                                    <label for="txtMALastName" class="form-label">Generational Suffix:</label>
                                                    <select data-role="none" id="selMAPersonGenSuffix" class="form-select">
                                                        <optgroup label="Select your suffix name">
                                                            <option value="">No Suffix Name</option>
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
                                                        </optgroup>
                                                    </select>
                                                </div>
                                                <div class="text-right">
                                                    <button data-role="none" class="btn btn-light" id="btnMACancelName">Cancel Edit Name</button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row mb-3">
                                        <label class="col-md-2 col-form-label mb-3">Date of Birth: <span class="required">*</span></label>
                                        <div class="col-md-10">
                                            <div class="input-group">
                                                <select data-role="none" id="selMABirthMonth" class="form-select fixed">
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
                                                <select data-role="none" id="selMABirthDay" class="form-select fixed"></select>
                                                <input data-role="none" type="text" placeholder="Year" maxlength="4" class="form-control" id="txtMABirthYear" style="width:70px;">
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row mb-3">
                                        <label class="col-md-2 col-form-label mb-3" for="txtMAEmailAddress">Email Address: <span class="required">*</span></label>
                                        <div class="col-md-10">
                                            <input data-role="none" type="text" class="form-control" id="txtMAEmailAddress">
                                        </div>
                                    </div>
                                    <hr />
                                    <h4 class="mb-3">Login Credentials</h4>

                                    <div>
                                        <div id="divActualPassword" subpage="divPassword" style="display:block;">
                                            <button data-role="none" id="btnMAChangePassword" class="btn btn-secondary">Update Login Credentials</button>
                                        </div>
                                        <div id="divEditPassword" subpage="divPassword" style="display:none;">
                                            <div class="row mb-3">
                                                <label class="col-md-2 col-form-label mb-3" for="txtMAUsername">New Username: <span class="required">*</span></label>
                                                <div class="col-md-10">
                                                    <input data-role="none" type="text" class="form-control" id="txtMAUsername">
                                                </div>
                                            </div>
                                            <div class="row mb-3">
                                                <label class="col-md-2 col-form-label mb-3" for="txtMANewPassword">New Password: <span class="required">*</span></label>
                                                <div class="col-md-10">
                                                    <input data-role="none" type="password" class="form-control" id="txtMANewPassword">
                                                </div>
                                            </div>
                                            <div class="text-right">
                                                <button data-role="none" id="btnMACancelNewPassword" class="btn btn-light">
                                                    Cancel Update Login Credentials
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                    <hr />
                                    <h4>Security Questions</h4>
                                    <p class="text title-tag-line mb-3"><b>NOTE</b>: You must remember the answers to the security questions you selected. This will be used to reset your account in case you forget your password.</p>

                                    <div class="row mb-3">
                                        <label class="col-md-2 col-form-label mb-3" for="selMASecQuestion1">Question No.1:</label>
                                        <div class="col-md-10">
                                            <select id="selMASecQuestion1" data-role="none" class="form-select mb-3">
                                                <option value="">Please Select</option>
                                                <option value="Where is your mother's birthplace?">Where is your mother's birthplace?</option>
                                                <option value="Who is your best childhood friend?">Who is your best childhood friend?</option>
                                                <option value="What is the name of your first pet?">What is the name of your first pet?</option>
                                                <option value="Who is your favorite teacher?">Who is your favorite teacher?</option>
                                                <option value="Who is your favorite historical person?">Who is your favorite historical person?</option>
                                                <option value="What is your grandfather's occupation?">What is your grandfather's occupation?</option>
                                            </select>
                                            <div id="divStaticSecQ1" subpage="divSecQ1" style="display:block;">
                                                <a data-role="none" id="btnMAEditSecQ1Ans" href="#">Edit answer security question no. 1 </a>
                                            </div>
                                            <div id="divEditSecQ1" subpage="divSecQ1" style="display:none;">
                                                <input data-role="none" type="text" placeholder="Answer of Question No. 1" class="form-control mb-3" id="txtMASecQuestion1Answer">
                                                <button data-role="none" id="btnMACancelEditSecQ1Ans" class="btn btn-light">Cancel Edit Q1 Answer</button>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row mb-3">
                                        <label class="col-md-2 col-form-label mb-3" for="selMASecQuestion2">Question No.2:</label>
                                        <div class="col-md-10">
                                            <select id="selMASecQuestion2" data-role="none" class="form-select mb-3">
                                                <option value="">Please Select</option>
                                                <option value="Where is your mother's birthplace?">Where is your mother's birthplace?</option>
                                                <option value="Who is your best childhood friend?">Who is your best childhood friend?</option>
                                                <option value="What is the name of your first pet?">What is the name of your first pet?</option>
                                                <option value="Who is your favorite teacher?">Who is your favorite teacher?</option>
                                                <option value="Who is your favorite historical person?">Who is your favorite historical person?</option>
                                                <option value="What is your grandfather's occupation?">What is your grandfather's occupation?</option>
                                            </select>
                                            <div id="divStaticSecQ2" subpage="divSecQ2" style="display:block;">
                                                <a data-role="none" id="btnMAEditSecQ2Ans" href="#">Edit answer security question no. 2 </a>
                                            </div>
                                            <div id="divEditSecQ2" subpage="divSecQ2" style="display:none;">
                                                <input data-role="none" type="text" placeholder="Answer of Question No. 2" class="form-control mb-3" id="txtMASecQuestion2Answer">
                                                <button data-role="none" id="btnMACancelEditSecQ2Ans" class="btn btn-light">Cancel Edit Q2 Answer</button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div id="divErrMsg" style="color:#ff3737; text-align:center; font-size:14px; margin-bottom:25px; display:none;"></div>
                            <br />
                            <div class="text-center">
                                <button id="btnMASaveChanges" data-toggle="modal" data-target="#confirmviapassword" data-role="none" class="btn btn-primary">SAVE CHANGES</button>
                            </div>
                        </div>
                        <div id="divMyAddressBook" subpage="pageMyAccount" style="display:none;">
                            <div class="row mb-4">
                                <div class="col-sm-8 mb-3">
                                    <h1 class="display-6 text-secondary">Address Book</h1>
                                </div>
                                <div class="col-sm-4 text-right">
                                    <button id="btnMAAddAddress" data-toggle="modal" data-target="#popupNewAddress" data-role="none" class="btn btn-success">Add New Address</button>
                                </div>
                            </div>

                            <div class="table-responsive">
                                <table id="tblMyAddress" data-role="none" style="width:100%;" class="table table-light table-hover table-striped">
                                    <thead>
                                        <tr>
                                            <th style="width:50px;">#</th>
                                            <th>My Address</th>
                                            <th style="width:150px;"></th>
                                        </tr>
                                    </thead>
                                    <tbody id="tbodyMyAddress"></tbody>
                                </table>
                            </div>
                        </div>
                        <div id="divMyOrderHistory" subpage="pageMyAccount" style="display:none;">
                            <div id="divOrderList" subpage="divMyOrderHistory" style="display:block;">
                                <h1 class="display-6 mb-4 text-secondary">My Order History</h1>

                                <div class="table-responsive">
                                    <table id="tblMyOrderList" data-role="none" style="width:100%;" class="table table-light table-hover table-striped">
                                        <thead>
                                            <tr>
                                                <th style="width:50px;">#</th>
                                                <th>Order No.</th>
                                                <th>Addresses</th>
                                                <th>Order Date</th>
                                                <th>Total</th>
                                            </tr>
                                        </thead>
                                        <tbody id="tbodyMyOrderList"></tbody>
                                    </table>
                                </div>
                            </div>
                            <div id="divOrderDetails" subpage="divMyOrderHistory" style="display:none;">
                                <span style="color:#5a5a5a; font-size:18px;"><a data-role="none" id="linkMyOrderHistory" class="custom-link">My Order History</a> <span class="bi-chevron-right"></span> Order Details</span>
                                <br /><br />
                                <div class="row">
                                    <div class="col-md-6">
                                        <b> Order No.:</b> <span id="lblODOrderNo"></span>
                                        <br />
                                        <b>Billing Address:</b> <span id="lblODBillingAddress"></span>
                                        <br />
                                        <b>Delivery Address:</b> <span id="lblODDeliveryAddress"></span>
                                    </div>
                                    <div class="col-md-6">
                                        <b>Order Date:</b> <span id="lblODOrderDate"></span>
                                        <br />
                                        <b>Order Total:</b> <span id="lblODOrderTotal"></span>
                                    </div>
                                </div>
                                <br />
                                <h4>Your Order</h4>
                                <br />
                                <table class="table table-striped" style="width:100%;">
                                    <thead>
                                        <tr>
                                            <th id="thOHVendor" style="width:200px;">Vendor</th>
                                            <th>Product Name</th>
                                            <th style="text-align:center;">Order Qty</th>
                                            <th style="text-align:center;">Price</th>
                                            <th style="text-align:center;">Line Total</th>
                                        </tr>
                                    </thead>
                                    <tbody id="tbodyMyOrderSku"></tbody>
                                </table>
                            </div>
                        </div>
                        <div id="divMAAccountsPayable" subpage="pageMyAccount" style="display:none;"></div>
                    </div>                    
                </div>
            </div>
        </div>        
    </div>
</div>

<div class="modal fade" id="confirmviapassword" tabindex="-1" role="dialog" aria-labelledby="mySmallModalLabel">
    <div class="modal-dialog modal-dialog-centered" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h5 data-role="none" class="modal-title">Enter your password</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>                
            </div>
            <div class="modal-body">
                <div class="form-group">
                    <label class="mb-3">For your security, please re-enter your password to continue</label>
                    <input data-role="none" placeholder="Your password" autofocus type="password" class="form-control" id="txtMACurrentPassword">
                </div>
                <div id="divMAErrMsg" style="color:#ff3737; text-align:center; font-size:14px; margin-bottom:25px; display:none;">Invalid password</div>
            </div>
            <div class="modal-footer">
                <button data-role="none" id="btnMACancelSave" type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                <button data-role="none" id="btnMAContinueSave" type="button" class="btn btn-success">CONTINUE</button>
            </div>
        </div>
    </div>
</div>

<div class="modal fade" id="popupNewAddress" tabindex="-1" role="dialog" aria-labelledby="mySmallModalLabel">
    <div class="modal-dialog modal-dialog-centered" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h5 data-role="none" class="modal-title">Address</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>                
            </div>
            <div class="modal-body">
                <div class="form-group">
                    <label for="txtCompleteAddress" class="mb-3">Enter complete address: <span class="required">*</span></label>
                    <input data-role="none" autofocus type="text" required class="form-control" id="txtCompleteAddress">
                </div>
            </div>
            <div class="modal-footer">
                <button data-role="none" id="btnAddressCancel" type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                <button data-role="none" id="btnAddressSave" type="submit" class="btn btn-success">Save</button>
            </div>            
        </div>
    </div>
</div>