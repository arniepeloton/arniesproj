@Code
    ViewData("Title") = "Ecommerce"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <link rel="stylesheet" href="/Content/bootstrap-custom.css" />
    <link rel="stylesheet" href="/Content/bootstrap.css" />
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
    <link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />
    <script type="text/javascript">
        require(['/Scripts/sources/Konstruksyon/MyProfile.js?crc='])
    </script>
End Section
<div style="background-color:#2C3E50">
    <div class="container">
        <nav>
            <div class="container-fluid">
                <!-- Brand and toggle get grouped for better mobile display -->
                <div class="navbar-header">
                    <button data-role="none" type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#bs-example-navbar-collapse-1" aria-expanded="false">
                        <span class="sr-only">Toggle navigation</span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                    </button>
                    <label data-role="none" class="navbar-brand" href="#" style="height:auto !important; color:#FFF; text-shadow:none;">Profile</label>
                </div>
            </div><!-- /.container-fluid -->
        </nav>
    </div>
</div>
<br />
<div class="container">
    <div class="row">
        <div class="col-md-offset-3 col-md-6">
            <h4 style="color:#CCC">Welcome to your account</h4>
            <br />
            <div class="row">
                <div class="col-sm-3">
                    <div style="width: 80px; height: 80px; border-radius: 50%; background-color: #CCC; text-align: center; vertical-align: middle; display: table-cell; color: #FFF; font-size: 30px;">
                        <span class="glyphicon glyphicon-user"></span>
                    </div>
                </div>
                <div class="col-sm-9">
                    <div id="divActualName" subpage="divMAName" style="display:block;">
                        <a data-role="none" class="custom-link" id="btnMAEditName" href="#">Edit Name</a>
                        <h3 id="txtMAFullName"></h3>
                    </div>
                    <div id="divEditName" subpage="divMAName" style="display:none;">
                        <a data-role="none" class="custom-link" id="btnMACancelName" href="#">Cancel</a><br />
                        <input data-role="none" type="text" placeholder="First Name *" class="form-control unicase-form-control text-input" id="txtMAFirstName">
                        <input data-role="none" type="text" placeholder="Middle Name" class="form-control unicase-form-control text-input" id="txtMAMiddleName">
                        <input data-role="none" type="text" placeholder="Last Name *" class="form-control unicase-form-control text-input" id="txtMALastName">
                        <select data-role="none" id="selMAPersonGenSuffix" class="form-control unicase-form-control text-input">
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
                </div>
            </div>
            <br /><br />
            <div class="form-inline">
                <div class="form-group">
                    <label class="info-title">Date of Birth</label>
                    <select data-role="none" id="selMABirthMonth" class="form-control unicase-form-control text-input fixed">
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
                    <select data-role="none" id="selMABirthDay" class="form-control unicase-form-control text-input fixed"></select>
                    <input data-role="none" type="text" placeholder="Year" maxlength="4" class="form-control unicase-form-control text-input" id="txtMABirthYear" style="width:70px;">
                </div>
            </div>
            <br />
            <div class="form-group">
                <label class="info-title" for="txtMAEmailAddress">Email Address</label>
                <input data-role="none" type="text" class="form-control unicase-form-control text-input" id="txtMAEmailAddress">
            </div>
            <br />
            <div class="form-group">
                <label class="info-title" for="txtMAUsername">Username</label>
                <input data-role="none" type="text" class="form-control unicase-form-control text-input" id="txtMAUsername">
            </div>
            <br />
            <div class="form-group">
                <div id="divActualPassword" subpage="divPassword" style="display:block;">
                    <a class="custom-link" data-role="none" id="btnMAChangePassword" href="#">Change Password</a>
                </div>
                <div id="divEditPassword" subpage="divPassword" style="display:none;">
                    <label class="info-title" for="txtMANewPassword">New Password</label>
                    <a class="custom-link" data-role="none" id="btnMACancelNewPassword" style="float:right;" href="#">Cancel New Password</a>
                    <input data-role="none" type="password" class="form-control unicase-form-control text-input" id="txtMANewPassword">
                </div>
            </div>
            <hr />
            <h4>Security Questions</h4>
            <p class="text title-tag-line"><b>NOTE</b>: You must remember the answers to the security questions you selected. This will be used to reset your account in case you forget your password.</p>
            <br />
            <div class="form-group">
                <label class="info-title" for="selMASecQuestion1">Question No. 1</label>
                <span>
                    <select id="selMASecQuestion1" data-role="none" class="form-control unicase-form-control text-input">
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
                <div id="divStaticSecQ1" subpage="divSecQ1" style="display:block;">
                    <a class="custom-link" data-role="none" id="btnMAEditSecQ1Ans" href="#">Edit answer security question no. 1 </a>
                </div>
                <div id="divEditSecQ1" subpage="divSecQ1" style="display:none;">
                    <input data-role="none" type="text" placeholder="Answer of Question No. 1" class="form-control unicase-form-control text-input" id="txtMASecQuestion1Answer">
                    <br />
                    <a class="custom-link" data-role="none" id="btnMACancelEditSecQ1Ans" href="#">Cancel Edit</a>
                </div>
            </div>

            <div class="form-group">
                <label class="info-title" for="selMASecQuestion2">Question No. 2</label>
                <span>
                    <select id="selMASecQuestion2" data-role="none" class="form-control unicase-form-control text-input">
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
                <div id="divStaticSecQ2" subpage="divSecQ2" style="display:block;">
                    <a class="custom-link" data-role="none" id="btnMAEditSecQ2Ans" href="#">Edit answer security question no. 2 </a>
                </div>
                <div id="divEditSecQ2" subpage="divSecQ2" style="display:none;">
                    <input data-role="none" type="text" placeholder="Answer of Question No. 2" class="form-control unicase-form-control text-input" id="txtMASecQuestion2Answer">
                    <br />
                    <a class="custom-link" data-role="none" id="btnMACancelEditSecQ2Ans" href="#">Cancel Edit</a>
                </div>
            </div>

            <div id="divErrMsg" style="color:#ff3737; text-align:center; font-size:14px; margin-bottom:25px; display:none;"></div>
            <br /><br />
            <div style="text-align:right;">                
                <button id="btnMACancel" data-role="none" class="btn btn-default btn-sm btn-less-width">Cancel</button>
                &nbsp;
                <button id="btnMASaveChanges" data-toggle="modal" data-target="#confirmviapassword" data-role="none" class="btn custom-btn-theme-b btn-sm btn-less-width">Save Changes</button>
            </div>
        </div>
    </div>
</div>

<div class="modal fade bs-example-modal-sm" id="confirmviapassword" tabindex="-1" role="dialog" aria-labelledby="mySmallModalLabel">
    <div class="modal-dialog modal-sm" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                <h4 data-role="none" class="modal-title">Enter your password</h4>
            </div>
            <div class="modal-body">
                <div class="form-group">
                    <label>For your security, please re-enter your current password to continue</label>
                    <input data-role="none" placeholder="Your password" autofocus type="password" class="form-control unicase-form-control text-input" id="txtMACurrentPassword">
                </div>
                <div id="divMAErrMsg" style="color:#ff3737; text-align:center; font-size:14px; margin-bottom:25px; display:none;">Invalid password</div>
            </div>
            <div class="modal-footer">
                <button data-role="none" id="btnMACancelSave" type="button" class="btn btn-default btn-sm btn-less-width" data-dismiss="modal">Cancel</button>
                <button data-role="none" id="btnMAContinueSave" type="button" class="btn custom-btn-theme-b btn-sm btn-less-width">Continue</button>
            </div>
        </div>
    </div>
</div>