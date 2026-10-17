@Code
    ViewData("Title") = "Konstruksyon"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <link rel="stylesheet" href="/Content/bootstrap-custom.css" />
    <link rel="stylesheet" href="/Content/bootstrap.css" />
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
    <link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />    
    <script>
        require([GetPathFormat('/Scripts/sources/Konstruksyon/Konstruksyon.js?crc=')]);
    </script>

    <style type="text/css">
        .buttonKonstruksyon{
            width:200px; 
            height:200px;
            border-radius:50%;
            display: inline-block;
            border:3px solid;            
            color: #FFF;
            text-shadow: none;
            font-size: 20px;
            cursor:pointer;
        }

        .buttonKonstruksyon.a{
            background-color: #03A9F4;
        }

        .buttonKonstruksyon.a:hover{
            border:3px solid #03A9F4;
            background-color: #FFF;
            color: #03A9F4;
        }

        .buttonKonstruksyon.b{
            background-color: #F44336;
        }

        .buttonKonstruksyon.b:hover{
            border:3px solid #F44336;
            background-color: #FFF;
            color: #F44336;
        }

        .buttonKonstruksyon.c{
            background-color: #FF9800;
        }

        .buttonKonstruksyon.c:hover{
            border:3px solid #FF9800;
            background-color: #FFF;
            color: #FF9800;
        }

        .buttonKonstruksyon.d{
            background-color: #4CAF50;
        }

        .buttonKonstruksyon.d:hover{
            border:3px solid #4CAF50;
            background-color: #FFF;
            color: #4CAF50;
        }

        .listSteps li:not(:last-child){
            margin-bottom:20px;
        }
    </style>
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
                    <label data-role="none" class="navbar-brand" href="#" style="height:auto !important; color:#FFF; text-shadow:none;">Konstruksyon</label>
                </div>

                <!-- Collect the nav links, forms, and other content for toggling -->
                <div class="collapse navbar-collapse" id="bs-example-navbar-collapse-1">
                    <ul class="nav navbar-nav navbar-right navbar-form">
                        <li style="margin-right:10px;">
                            <button data-role="none" data-toggle="modal" data-target="#modalSignUp" class="btn custom-btn-theme-b btn-less-width btn-sm">Sign Up</button>
                        </li>
                        <li>
                            <button data-role="none" id="btnLogin" data-toggle="modal" data-target="#modalLogin" class="btn custom-btn-theme-a btn-less-width btn-sm">Log In</button>
                        </li>
                    </ul>
                </div><!-- /.navbar-collapse -->
            </div><!-- /.container-fluid -->
        </nav>
    </div>
</div>
<div>
    <div id="carousel-example-generic" class="carousel slide" data-ride="carousel">
        <ol class="carousel-indicators">
            <li data-target="#carousel-example-generic" data-slide-to="0" class="active"></li>
            <li data-target="#carousel-example-generic" data-slide-to="1"></li>
            <li data-target="#carousel-example-generic" data-slide-to="2"></li>
        </ol>
        <div class="carousel-inner" role="listbox">
            <div class="item active">
                <img style="width:100%;" alt="First slide [900x500]" src="/Content/Konstruksyon/1banner.png" data-holder-rendered="true">
            </div>
            <div class="item">
                <img style="width:100%;" alt="First slide [900x500]" src="/Content/Konstruksyon/2banner.png" data-holder-rendered="true">
            </div>
            <div class="item">
                <img style="width:100%;" alt="First slide [900x500]" src="/Content/Konstruksyon/3banner.png" data-holder-rendered="true">
            </div>
        </div>
        <a data-role="none" class="left carousel-control" href="#carousel-example-generic" role="button" data-slide="prev">
            <span class="glyphicon glyphicon-chevron-left" aria-hidden="true"></span>
            <span class="sr-only">Previous</span>
        </a>
        <a data-role="none" class="right carousel-control" href="#carousel-example-generic" role="button" data-slide="next">
            <span class="glyphicon glyphicon-chevron-right" aria-hidden="true"></span>
            <span class="sr-only">Next</span>
        </a>
    </div>
    <br />

    <div class="container">
        <div class="row">
            <div class="col-sm-3" style="text-align:center">
                <div class="buttonKonstruksyon d" data-toggle="modal" data-target="#modalGeography">
                    <div style="width:200px; height:200px; text-align:center; vertical-align: middle; display: table-cell;">
                        <div>
                            Business<br /> Permit<br /> Application
                        </div>
                    </div>
                </div>
                <br /><br />
                <p style="text-align:center;">
                    Have a new business or renewing your permit for an existing one? Fill up your business permits online and skip all the lengthy manual paper and footwork.
                </p>
            </div>
            <div class="col-sm-3" style="text-align:center">
                <div id="btnPermit" class="buttonKonstruksyon a" data-toggle="modal" data-target="#modalGeography">
                    <div style="width:200px; height:200px; text-align:center; vertical-align: middle; display: table-cell;">
                        <div>
                            Building<br />Permit <br /> Application
                        </div>
                    </div>
                </div>
                <br /><br />
                <p style="text-align:center;">
                    Fill up online forms and view requirements for your upcoming project. No need to manually fill in forms and do lengthy footwork with our online service.
                </p>
            </div>
            <div class="col-sm-3" style="text-align:center">
                <div class="buttonKonstruksyon c" data-toggle="modal" data-target="#modalGeography">
                    <div style="width:200px; height:200px; text-align:center; vertical-align: middle; display: table-cell;">
                        <div>
                            Construction <br /> Software <br />Solutions
                        </div>
                    </div>
                </div>
                <br /><br />
                <p style="text-align:center;">
                    Want to automate project management and timekeeping for your construction projects? We have software solutions that run on tablets and phones to fit job site’s demanding environment.
                </p>
            </div>
            <div class="col-sm-3" style="text-align:center">
                <div class="buttonKonstruksyon b">
                    <div data-tooltip="true" title="Coming Soon" style="width:200px; height:200px; text-align:center; vertical-align: middle; display: table-cell;">
                        <div>
                            Professional<br /> Services <br /> Directory
                        </div>
                    </div>
                </div>
                <br /><br />
                <p style="text-align:center;">
                    This is where vendors and clients in the construction and realty industry can meet.
                </p>
            </div>
        </div>
    </div>       
</div>
<br />
<div style="background-color:#2C3E50">
    <div class="container">
        <div style="text-align:center; height:150px; color:#fff;">
            <h3 style="text-shadow:none;">
                Powered by Mobiz
            </h3>
        </div>
    </div>
</div>

<div class="modal fade" id="modalBuildingPermit" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                <h4 class="modal-title" id="myModalLabel">Get your building permits approved in 5 easy steps!</h4>
            </div>
            <div class="modal-body">
                <ol class="listSteps" style="list-style:decimal">
                    <li>
                        Select the city where your project is being built.
                    </li>
                    <li>
                        Create or sign in to your account.
                    </li>
                    <li>
                        Fill up a new building permit form or edit an existing form.
                    </li>
                    <li>
                        Download and pay for the level of service you prefer.
                    </li>
                    <li>
                        Process documents and wait for approval.
                    </li>
                </ol>
            </div>
            <div class="modal-footer">
                <button data-role="none" type="submit" id="btnStartNow" class="btn custom-btn-theme-b btn-less-width">Start Now</button>
            </div>
        </div>
    </div>
</div>

<div class="modal fade" id="modalGeography" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
    <div class="modal-dialog modal-sm" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                <h4 class="modal-title" id="myModalLabel">Choose Geography</h4>
            </div>
            <div class="modal-body">
                <div id="btnDavao" style="width:100px; height:100px; border:3px solid #ccc; border-radius:50%; text-align:center; vertical-align: middle; display: table-cell;">
                    <div>
                        Davao City
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="modal fade" id="modalLogin" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                <h4 class="modal-title" id="myModalLabel">Log In</h4>
            </div>
            <form id="formKonstruksyonLogin">
                <div class="modal-body">
                    Please enter the required fields<br /><br />
                    <span id="loginstat" class="required"></span>
                    <div class="form-group">
                        <span>Username <span class="required">*</span></span>
                        <input data-role="none" type="text" class="form-control" id="txtLoginUsername">
                    </div>
                    <div class="form-group">
                        <span>Password <span class="required">*</span></span>
                        <input data-role="none" type="password" class="form-control" id="txtLoginPassword">
                    </div>
                </div>
                <div class="modal-footer">
                    <button data-role="none" type="button" class="btn custom-btn-theme-m btn-less-width btn-sm" id="btnSignUpNow" style="float:left">Create Account</button>
                    <button data-role="none" type="button" class="btn custom-btn-theme-c btn-less-width btn-sm" data-dismiss="modal">Cancel</button>
                    <button data-role="none" type="submit" id="btnLoginOk" class="btn custom-btn-theme-b btn-less-width btn-sm">Log In</button>
                </div>
            </form>            
        </div>
    </div>
</div>

<div class="modal fade" id="modalSignUp" data-backdrop="static" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                <h4 class="modal-title" id="myModalLabel">Create your Account</h4>
            </div>
            <div class="modal-body">
                <h3>Personal Information</h3>
                <div class="form-group">
                    <span for="">First Name <span class="required">*</span></span>
                    <input data-role="none" type="text" class="form-control" id="txtFirstname">
                </div>
                <div class="form-group">
                    <span for="">Middle Name</span>
                    <input data-role="none" type="text" class="form-control" id="txtMiddlename">
                </div>
                <div class="form-group">
                    <span for="">Last Name <span class="required">*</span></span>
                    <input data-role="none" type="text" class="form-control" id="txtLastname">
                </div>
                <div class="form-group">
                    <span for="">Generational Suffix</span>
                    <select data-role="none" class="form-control" id="selGenSuffix">
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
                <div class="form-group" style="display:-webkit-inline-box;">
                    <span for="">Date of Birth <span class="required">*</span></span>
                    <select data-role="none" class="form-control fixed" id="selBirthMonth">
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
                    <select data-role="none" id="selBirthDay" class="form-control unicase-form-control text-input fixed">
                        <option value="">Day</option>
                        <option value="1">1</option>
                        <option value="2">2</option>
                        <option value="3">3</option>
                        <option value="4">4</option>
                        <option value="5">5</option>
                        <option value="6">6</option>
                        <option value="7">7</option>
                        <option value="8">8</option>
                        <option value="9">9</option>
                        <option value="10">10</option>
                        <option value="11">11</option>
                        <option value="12">12</option>
                        <option value="13">13</option>
                        <option value="14">14</option>
                        <option value="15">15</option>
                        <option value="16">16</option>
                        <option value="17">17</option>
                        <option value="18">18</option>
                        <option value="19">19</option>
                        <option value="20">20</option>
                        <option value="21">21</option>
                        <option value="22">22</option>
                        <option value="23">23</option>
                        <option value="24">24</option>
                        <option value="25">25</option>
                        <option value="26">26</option>
                        <option value="27">27</option>
                        <option value="28">28</option>
                        <option value="29">29</option>
                        <option value="30">30</option>
                        <option value="31">31</option>
                    </select>
                    &nbsp;<input data-role="none" type="text" placeholder="Year" maxlength="4" class="form-control unicase-form-control text-input" id="txtBirthYear" style="width:70px;">
                </div>

                <h3>Login Information</h3>

                <div class="form-group">
                    <span for="">Email Address <span class="required">*</span></span>
                    <p class="text title-tag-line" style="font-size:14px;"><i>NOTE: You must remember the email address you enter. This will be used to reset your account in case you forget your password.</i></p>
                    <input data-role="none" type="email" class="form-control" id="txtEmail">
                </div>
                <div class="form-group">
                    <span for="">Username <span class="required">*</span> (minimum of 8 characters)</span>
                    <input data-role="none" type="text" class="form-control" id="txtUsername">
                </div>
                <div class="form-group">
                    <span for="">Password <span class="required">*</span> (minimum of 8 characters)</span>
                    <input data-role="none" type="password" class="form-control" id="txtPassword">
                </div>
                <div class="form-group">
                    <span for="">Re-enter Password <span class="required">*</span></span>
                    <input data-role="none" type="password" class="form-control" id="txtConfirmPassword">
                </div>

                <h3>Security Questions</h3>

                <div class="form-group">
                    <span for="">Question No. 1 <span class="required">*</span></span>
                    <select id="selSecQuestion1" data-role="none" class="form-control unicase-form-control text-input">
                        <option value="">Please Select</option>
                        <option value="Where is your mother's birthplace?">Where is your mother's birthplace?</option>
                        <option value="Who is your best childhood friend?">Who is your best childhood friend?</option>
                        <option value="What is the name of your first pet?">What is the name of your first pet?</option>
                        <option value="Who is your favorite teacher?">Who is your favorite teacher?</option>
                        <option value="Who is your favorite historical person?">Who is your favorite historical person?</option>
                        <option value="What is your grandfather's occupation?">What is your grandfather's occupation?</option>
                    </select><br />
                    <input data-role="none" type="password" class="form-control" id="txtSecAnswerQ1" placeholder="Answer of Question No. 1">
                </div>
                <div class="form-group">
                    <span for="">Question No. 2 <span class="required">*</span></span>
                    <select id="selSecQuestion2" data-role="none" class="form-control unicase-form-control text-input">
                        <option value="">Please Select</option>
                        <option value="Where is your mother's birthplace?">Where is your mother's birthplace?</option>
                        <option value="Who is your best childhood friend?">Who is your best childhood friend?</option>
                        <option value="What is the name of your first pet?">What is the name of your first pet?</option>
                        <option value="Who is your favorite teacher?">Who is your favorite teacher?</option>
                        <option value="Who is your favorite historical person?">Who is your favorite historical person?</option>
                        <option value="What is your grandfather's occupation?">What is your grandfather's occupation?</option>
                    </select><br />
                    <input data-role="none" type="password" class="form-control" id="txtSecAnswerQ2" placeholder="Answer of Question No. 2">
                </div>
            </div>
            <div class="modal-footer">
                <span id="signupstat" class="required"></span>
                <button data-role="none" type="button" class="btn custom-btn-theme-c btn-less-width btn-sm" data-dismiss="modal">Cancel</button>
                <button data-role="none" type="submit" id="btnSignUpNext" class="btn custom-btn-theme-b btn-less-width btn-sm">Next</button>
            </div>
        </div>
    </div>
</div>

<div class="modal fade" id="modalSignUpNext" data-backdrop="static" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                <h4 class="modal-title" id="myModalLabel">Create your Account</h4>
            </div>
            <div class="modal-body">
                Are you part of any organization?
            </div>
            <div class="modal-footer">
                <button data-role="none" type="button" class="btn custom-btn-theme-c btn-less-width btn-sm" id="btnSignUpBack" style="float:left">Back</button>
                <button data-role="none" type="button" id="btnSignUpNo" class="btn custom-btn-theme-c btn-less-width btn-sm">No, create my account now</button>
                <button data-role="none" type="submit" id="btnSignUpYes" class="btn custom-btn-theme-b btn-less-width btn-sm">Yes</button>
            </div>
        </div>
    </div>
</div>

<div class="modal fade" id="modalSuccess" data-backdrop="static" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                <h4 class="modal-title" id="myModalLabel">Message</h4>
            </div>
            <div class="modal-body">
                Your account has been successfully created. 
                <br /><br />
                Before you can use your account, please activate your account using the link
                that we sent to your email address (<b id="lblUserEmail"></b>)
            </div>
            <div class="modal-footer">
                <button data-role="none" data-dismiss="modal" type="button" class="btn custom-btn-theme-b btn-less-width btn-sm">Close</button>
            </div>
        </div>
    </div>
</div>

<div class="modal fade" id="modalSignUpOrganization" data-backdrop="static" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                <h4 class="modal-title" id="myModalLabel">Organization Info</h4>
            </div>
            <div class="modal-body">
                <form class="register-form outer-top-xs" role="form">
                    <div class="form-group">
                        <span>Company Name: <span class="required">*</span></span>
                        <input data-role="none" type="text" class="form-control unicase-form-control text-input" id="txtCompanyName">
                    </div>

                    <div class="form-group">
                        <span>TIN:</span>
                        <input data-role="none" type="text" class="form-control unicase-form-control text-input" id="txtCompanyTIN">
                    </div>

                    <div class="form-group">
                        <span>Business Industry: <span class="required">*</span></span>
                        <input data-role="none" type="text" class="form-control unicase-form-control text-input" id="txtBusInd">
                        <div id="listBusinessIndustry" class="list-group" style="display:none; position: absolute; max-width: 100%; box-shadow: 0px 5px 28px #CCC; overflow-y:auto; max-height:300px;"></div>
                    </div>

                    <div class="form-group">
                        <span>Company Email Address: <span class="required">*</span></span>
                        <input data-role="none" type="text" class="form-control unicase-form-control text-input" id="txtCompanyEmailAdd">
                    </div>

                    <div class="form-inline">
                        <div class="form-group">
                            <span>Company Mobile Number: <span class="required">*</span></span>
                            <input data-role="none" type="text" value="+63" maxlength="3" class="form-control unicase-form-control text-input" disabled="" style="width:50px;">

                            <input data-role="none" type="text" placeholder="9**********" maxlength="10" class="form-control unicase-form-control text-input" id="txtCompanyMobileNo" style="width:110px;">
                        </div>
                    </div>
                    
                    <h3>Company Address</h3>

                    <div class="form-group">
                        <span>House/Building Number: <span class="required">*</span></span>
                        <input data-role="none" type="text" class="form-control unicase-form-control text-input" id="txtCompanyAddressBldgNo">
                    </div>

                    <div class="form-group">
                        <span>Street Name: <span class="required">*</span></span>
                        <input data-role="none" type="text" class="form-control unicase-form-control text-input" id="txtCompanyAddressStreetName">
                    </div>

                    <div class="form-group">
                        <span>City: <span class="required">*</span></span>
                        <select data-role="none" id="selCompanyAddressCity" class="form-control unicase-form-control text-input"><option value="">Please select</option><option value="88ed49c0-8b5d-46af-8697-bc4e427ff166"></option><option value="caa7b159-8688-460a-a5d4-f9588f7f77ab">Agusan  del  norte</option><option value="979a7352-1774-426d-8663-b08ee2bed1e2">Agusan Del Sur</option><option value="a80701c9-ebbf-45a7-ae51-132f376d4502">Alaminos</option><option value="50f27de8-6497-4cde-8017-eb45372192bf">aleosan</option><option value="156f0b8c-cbc8-4cbc-84f5-aa60a8291f85">Angeles</option><option value="49ceb6c2-a46e-4b2d-aa35-62a7efd0c155">Antipolo</option><option value="d60475ea-13b4-4439-b59a-bcbf4178954d">Asuncion</option><option value="702c8416-a998-4992-bec5-18560e9087b9">Bacolod</option><option value="cb5b13cd-fbab-450b-aa6a-7f45b1d52264">Bacoor</option><option value="72e29235-75bf-4cf1-b6ee-5b5f10ce2854">Baganga</option><option value="69112fc7-033f-4b39-8159-d8e99919462c">Bago</option><option value="25f9e7ef-d368-40d9-a81b-8b14b01bf231">Baguio</option><option value="0b9158c2-5576-4a5b-853e-e54ee0df55da">Bais</option><option value="0cd4df7b-fe0b-400e-b541-3cdd9b4c5f16">Balanga</option><option value="546662ba-0749-428c-9dcf-4d5a6dd226cc">Bangkal</option><option value="7bc17350-94c9-4d56-8ba8-75d875498177">Bansalan</option><option value="7d03a532-60a4-451f-a3d8-8de418acc700">Batac</option><option value="2f463874-0735-44d0-a6a7-5d0271c784f8">Batangas City</option><option value="cfe8467b-04ef-46ae-bf93-74d0e54d1e8a">Bayawan</option><option value="0c59ac40-0ad0-42cb-9160-a2be87524978">Baybay</option><option value="68393ed4-3a1d-46eb-b7da-03cc9f2ad593">Bayugan</option><option value="69d4587a-575c-41b0-98af-366efc2185a1">Bilig City</option><option value="2c6f67a2-d161-4f6c-8584-715cbfaf03a0">Biñan</option><option value="67188d93-4692-48b4-b922-b66d5cd8d03e">Binondo</option><option value="03af869e-de9d-4c26-9b8f-0fdcb46f4a96">Bislig City</option><option value="91d9cade-6220-480f-8383-e162f19b168f">Bogo</option><option value="641f6a41-5f22-4fe1-8369-269ca4b89172">Borongan</option><option value="1390f0d2-0e1b-4fe6-9d9e-ce7c3b371d12">Bukidnon</option><option value="90ab0147-578d-450f-9b98-e602a8ff3f36">BUNAWAN</option><option value="af9a6d2a-7680-4638-ae36-ea1c3b12c112">Butuan City</option><option value="29e7ea03-fdd4-4bb1-8483-238b5cb34b51">CABADBARAN</option><option value="da5d73ee-bc1c-4dc4-9eb2-51cc9c6347a8">Cabanatuan</option><option value="ff244f44-b1b4-4396-8198-deeb85aee020">Cabuyao</option><option value="3ed5df52-0d26-45fb-bbd9-2e535e50eacd">Cadiz</option><option value="eca6b2f0-ee2e-4422-9f8b-ed21ca87c646">Cagayan de Oro City</option><option value="ab1661a6-fe73-4ee3-a3bc-8a4b4bb6f84a">Calamba</option><option value="edda0c17-78e2-41a1-9a87-9fa2a6a192e5">Calapan</option><option value="511cc39e-e6d7-4c51-bf4a-8758b81dc819">Calbayog</option><option value="0adb8cfa-0c20-4b9d-bc46-7f8305b30c2d">Caloocan City</option><option value="c644e397-5029-43a4-9e3a-89f6680ca646">Candon</option><option value="f10f7dfa-4fa1-4e09-b6d9-e28f14025fb8">Canlaon</option><option value="dc6cce5c-e946-4f11-9f5e-f0c41b9af601">Carcar</option><option value="07cc430a-559a-44a6-8fec-3d7746e732e4">Carmen District</option><option value="a317e5fb-0f5e-4ee9-b35e-2626efc5d14a">Catbalogan</option><option value="e0687729-69d3-460a-b828-9d3200c1e725">Cauayan</option><option value="9219519a-df27-49d7-a49a-000861c7028c">Cavite City</option><option value="033743ff-9508-4422-b978-49403415581a">cebu</option><option value="47b7e6c6-62cd-4743-9349-393e44c875db">Compostela</option><option value="3318e78b-b2bd-4f9e-a2fc-51b7d90c2d6f">Cotabato City</option><option value="e5c6df36-377d-42cb-882c-085d8e292af5">Dagupan</option><option value="723c9146-6f6a-45a1-b465-49c7857617a2">Danao</option><option value="b7c8c5a3-5270-465e-8d40-22d732044921">Dapitan</option><option value="f3aced6f-5cab-4cfc-9014-90091798b32b">Dasmariñas</option><option value="44b324a3-50ac-434e-8770-b7b6cc085f8f">Davao   Oriental</option><option value="316a03d3-611a-4376-a2b2-7d0dfd7a4fbc">Davao City</option><option value="5e98541d-4c52-4351-b5cf-0e38a5e0def5">DAVAO DEL NORTE</option><option value="36b912e8-3269-45c5-ade1-23869fe6a327">Digos City</option><option value="f5501ee0-635d-41db-95db-0832a6671d2e">Dipolog</option><option value="39f33a07-cf38-4505-a1fb-913a6ec622bf">Don Carlos</option><option value="59dd72a0-9485-442f-92aa-ea084613582b">Dumaguete</option><option value="da387846-9583-4c7c-964b-1c929ef2d1af">El Salvador</option><option value="4def4985-c852-4ed6-ba96-fc0770768f8d">Escalante</option><option value="314aacec-3588-49d6-a862-efde3b244d1b">Fort Bonifacio, Taguig</option><option value="b2f60a0d-953d-4834-949a-292782455d5d">Gapan</option><option value="65eadb7d-f37d-4c1c-a0d9-8861fd623ffc">General Santos</option><option value="919f5d7f-561e-469c-84f8-474d94deaadc">General Santos (Gensan) City</option><option value="02824fbb-b7c3-4716-b1c2-c73a83a5a979">gingoog city</option><option value="0d93f159-e352-464d-9b95-1aa4d92bebc8">GUANGDONG, CHINA</option><option value="30dda271-c6ef-488b-9177-3519d887332d">Guihulngan</option><option value="f8cbccf8-b93b-47a8-adb8-5481bb8698bb">Himamaylan</option><option value="e82552a7-15fb-430a-bb78-16b7f7b1f29c">Ilagan</option><option value="d04c6a18-4a3d-4313-bfd8-62351c31264b">Iligan</option><option value="4d748a49-fca4-4836-9f63-5b60fda96e54">Iloilo City</option><option value="33333d96-5987-41e5-9d78-ff81213dde36">Imus</option><option value="3969c1ff-b7c6-414d-944b-9bde2bcbf5a8">Iriga</option><option value="8c525a18-fd3f-4176-bc88-8adc9a07ac4b">Isabela</option><option value="d1108176-929d-49ba-a3cf-7e885882a932">Isulan</option><option value="71bb46ec-5d3f-4b4c-91df-5c5dda0e3f7a">Kabacan</option><option value="aab69591-fadf-4a2d-abc8-3f7881e0c3a6">Kabankalan</option><option value="9636a978-8e4e-4008-92da-cef9b7a65bbf">Kapalong</option><option value="e03c0f79-0d82-4450-a699-1b54a9359301">Kiblawan</option><option value="b2f77fbc-fd10-4182-880a-762d7eefbfb8">Kidapawan</option><option value="50724131-12b6-457f-9395-d7f161228490">Koronadal City</option><option value="c7db5b15-f463-4d3e-b8a4-60fd50dee105">La Carlota</option><option value="27e7679e-75b2-461f-870d-d5a2e8e75a0a">Lamitan</option><option value="63017536-6e1c-4788-9bc5-1129d91185b5">Laoag</option><option value="f6c327ae-1b04-4fa7-92cd-3a5ef29d1053">Lapu-Lapu</option><option value="2baf36db-bb8f-4b15-847f-cf350a735bb0">Las Piñas</option><option value="9f948899-0e5f-4796-aaa6-7e4f876e07ef">Legazpi</option><option value="e7f8380d-3ee4-4ef3-be37-dec9680e9c12">Libungan</option><option value="20b2fb75-bc90-4f7b-b647-4eec428b7ae2">Ligao</option><option value="2a673846-8bc2-422a-8890-77e80ed315c6">Lipa</option><option value="f126e380-a855-4581-8a81-35906874c45d">Lucena</option><option value="aea816fc-5c76-4f7c-905a-c9d6ac293e4a">Luna</option><option value="c42fe3af-a0f0-49d6-a474-7c0e0d7b9f7c">Lupon</option><option value="edd945a5-7875-41ac-8039-dfccce8712e7">M'lang (Mlang)</option><option value="827bc84c-5672-4e22-a14a-0e8dde0f2d81">Maasin</option><option value="c2db9eb0-5929-4870-a97f-9c801be9471c">Mabalacat</option><option value="4b904ada-15bf-4cbc-8056-77a3380efd27">MACO</option><option value="11c92513-eb42-47f7-bf0b-6006b9fb7a9d">Magsaysay</option><option value="90e1aeb5-0727-4c53-912a-cde55d501694">Magsaysay</option><option value="bdb67e3d-81a2-45fb-b156-2676d2d6bb6a">Makati</option><option value="95aa3d41-8904-46b9-9969-2cc5a4c79e37">Makilala</option><option value="b6553910-77a2-4a2e-a2f1-f6bad5e3b7cd">Malabon</option><option value="03ca68e1-7936-498a-b353-c6619d7b88a0">Malate</option><option value="341decf1-f35e-405b-ab0a-b9c6e11d055d">Malaybalay</option><option value="7b2709f3-8691-4076-ad30-991cc334a67a">Malita,Davao del Sur</option><option value="aa5f7d8a-8965-47c6-8358-2d6a5548e2fd">Malolos</option><option value="323a4a0b-fa16-4f6d-95d3-f01aaa17a3c4">MANDALUYONG</option><option value="630dde70-a9ac-4a47-8012-07848d5d29a7">Mandaluyong City</option><option value="510b6cc0-2225-4001-aef7-ff9dc55e2573">MANDAUE CITY</option><option value="3720fdb5-f983-4e5f-adc5-c60310479cca">Manila City</option><option value="cd515b48-bc46-46bf-94a2-6313f1b013e9">MARAGUSAN</option><option value="df2221e2-6220-4707-b0b4-9951de10a732">Maramag</option><option value="ef1e2a14-c33d-47f2-a6f7-36382c567ee6">Marawi</option><option value="7b8e2443-bfee-4ccb-a7b9-0bad05b39da0">Marikina</option><option value="ff00a156-0c8d-486a-8803-f86979e6a950">Masbate City</option><option value="416d73fe-ac41-4aea-ad87-f6e075b26331">MATALAM</option><option value="642baf3b-79f8-4686-bdc9-b63b6404dc09">Mati</option><option value="c52205a6-3284-405f-8533-58c1dde6c34c">Meycauayan</option><option value="c08bf619-30f9-44d4-984c-470d2dcff82b">Midsayap</option><option value="5207d2d6-e1bc-48e7-8acd-1ab7917fd49a">Mintal</option><option value="39969a39-69c5-44ee-bdab-88ebee9b60b7">Monkayo</option><option value="f2efc1ab-a700-4a33-b82a-e5f09f36ccb3">Montevista</option><option value="d23d102e-0fd7-4a5b-8358-cc1acbdc87b2">Muñoz</option><option value="e4ad37de-6aaf-40c1-8606-2c61952a2958">Muntinlupa</option><option value="d877d0dd-583b-4359-b802-fc05f8ff5425">Nabunturan</option><option value="47db5769-701c-410e-b534-e8b7a1ecad07">Naga, Camarines Sur</option><option value="75abf849-0986-4156-bccb-32042fac7987">Naga, Cebu</option><option value="a8d5a7ec-d9ec-4ab9-8cb5-9bc631536d10">NASIPIT</option><option value="36255a22-fea4-4a3c-9689-6500da04f929">Navotas</option><option value="46fd493f-0f8f-47b4-ae3e-fd7c06eb1897">North Upi</option><option value="57ac4085-55a7-42e8-a6ba-724fae8a6410">Olongapo</option><option value="b231507f-d453-47f1-b697-737ba3c5a52b">Ormoc</option><option value="0528cba9-cb6f-4d55-8b66-55b9854e5e13">Oroquieta</option><option value="78fb05d7-6e82-476d-9cf6-80ecb0d47f64">Ozamiz</option><option value="853e8f61-3ebd-43b0-84da-fbc259786bb1">Padada</option><option value="047d431f-309c-4ff2-92b6-0fe94f1a49d0">Pagadian</option><option value="a9fbc158-4175-449f-9135-a978a91ddd9f">Palayan</option><option value="37b83793-68c0-4c0f-950c-60e806fc329f">Panabo City</option><option value="da2000e8-b25f-4710-8df9-85ce762820df">Pantukan</option><option value="de2959b7-b061-41af-9e15-a32f405d98b0">Paquibato</option><option value="64682b45-d717-4d50-a797-6005ca14f89e">Parañaque</option><option value="5b11694e-6a90-4771-9732-8d4bd54a1fe5">Pasay</option><option value="12839f50-dec8-4868-a70f-028731f9a116">Pasig</option><option value="75e92305-10e0-47fd-aade-14971ffd9424">Passi</option><option value="110dca52-67b5-48df-9ba2-e9b90cf22308">Pigcawayan</option><option value="a281a978-45fe-4485-9478-3fc31f786616">PIKIT</option><option value="ff556274-9a34-4f7e-9e0d-52270e362faa">Poblacion Buluan</option><option value="faf02e18-7ba0-4838-a482-4b3d633ca10f">Polomolok</option><option value="b6a6209e-13a9-41a3-bdc2-48563a5165a6">Puerto Princesa</option><option value="c639c2b6-4c3c-421e-9e11-80b7eaebf67f">Quezon City</option><option value="24164905-5303-491d-8633-5ea6cb2cda0d">Roxas</option><option value="84cc4427-5661-4e68-a55a-35ac7df2f800">Sagay</option><option value="29297416-4fa0-4781-b3eb-ef88aff00018">Samal</option><option value="a599f22b-7de1-40c8-91bf-b1a665c9dd86">San Carlos, Negros Occidental</option><option value="098c16ed-e6fa-4fd7-9137-0dd89f6da384">San Carlos, Pangasinan</option><option value="f4817a03-7fbf-4e80-ac54-27d87b998bde">San Fernando, La Union</option><option value="52e06de0-2de7-4242-a5d1-402ee1165a84">San Fernando, Pampanga</option><option value="3e292684-4a9d-4b4a-a2d6-3165abca23ac">San Francisco</option><option value="f2309be1-b089-4927-bc83-673f8347b4e8">San Francisco Agusan</option><option value="975fba41-4913-494c-aff7-d005f9764df5">San Jose del Monte</option><option value="b300553d-94a6-4a38-a3ee-5d04d8f2977e">San Jose, Nueva Ecija</option><option value="a0aa5a8f-2621-4b5b-ad58-802898b34f1c">San Juan</option><option value="ed139570-0721-43ca-9c6f-d446193e360f">San Pablo</option><option value="79c78485-3034-4dce-b9b6-3f0d509a0488">San Pedro</option><option value="fc156f05-a718-41f3-bd9f-ee7708ecf8c3">Santa Rosa</option><option value="d854ceac-d34a-4cf4-816e-5b81e7338d6b">Santiago</option><option value="c08ffe21-ec21-4cd6-8bb4-dcf211a3d88f">Sasa</option><option value="f68e1d18-3d53-4fa6-b48b-038a6a2a18aa">Silay</option><option value="e15318da-1a42-43c5-9305-5858ec51a8e8">Sipalay</option><option value="3381cc8b-e72c-45f1-b37a-b00a08e6eee7">Sorsogon City</option><option value="8b543ad4-3e2b-43e0-bea2-a8efaf50bca9">Sta. Cruz</option><option value="10542b4a-e3b7-41c2-b98b-23e77a8844bb">Sto. Tomas</option><option value="65a31a46-4ab2-45c1-a881-e30a4497d47b">Sucat</option><option value="81792589-8c49-4c29-b31c-c23bd0e83376">Surigao City</option><option value="a2265ef9-f236-43cd-ba4a-b87db00c0f82">Surigao Del Sur</option><option value="49b0325f-058c-4009-af9f-e8972fe869e6">Tabaco</option><option value="6e2c7d48-665e-4079-8683-61f210302fdf">Tabuk</option><option value="dc105123-a188-48e8-b9ac-1efc4dd0ae06">Tacloban</option><option value="aec5a1ff-48f5-46fa-b74f-99be5a592124">Tacurong</option><option value="595c7680-93c6-45cc-8979-5d9685c10380">Tagaytay</option><option value="2e10de13-b20a-4e7d-bfbe-460a9eb76dbd">Tagbilaran</option><option value="b442d676-b05c-4a67-9f0b-11686d76e004">Taguig</option><option value="f77e9144-a4b0-4b1c-bab9-c7c749b8022b">Tagum City</option><option value="2697b045-c185-4b16-bb01-7169d2850f37">Talisay, Cebu</option><option value="e4e79595-71f7-4190-8b2f-b0e0abb2b929">Talisay, Negros Occidental</option><option value="eb558b40-ca68-45be-a4e6-195a5417c0fd">Tanauan</option><option value="811c4d84-3759-4ce1-8b7e-ffef0659f951">Tandag</option><option value="7802833c-d80e-41f9-966c-25de2390051e">Tangub</option><option value="7eb570b3-d72a-4c4f-8bba-9c2ed8f0e391">Tanjay</option><option value="4e12a9eb-6de2-4c75-a69d-7f068c544f55">Tarlac City</option><option value="53622820-8b30-4d2d-9176-b5404590c135">Tayabas</option><option value="1a7a934e-87a0-4ea8-ba5d-fea93eae4c63">Toledo</option><option value="7c350c1e-8634-4ff6-8880-b66d3ad86c61">Trece Martires</option><option value="cb833783-db5a-41ba-85c9-880c89026617">Trento</option><option value="a303e41a-e2fd-446d-a826-5c2cd5ac2a86">Tuguegarao</option><option value="41ae8070-1997-4656-8721-b255cf7512dd">ULAS</option><option value="266d266d-0c82-45cf-b61e-d7f4b4c002ad">Urdaneta</option><option value="c38e00cf-b22a-48c1-bf85-5150576a94a9">Valencia</option><option value="b5e92a3e-20fc-4553-8042-5af238130179">Valenzuela</option><option value="6451f5f7-d170-4c5a-9dc0-c76ffc752608">Victorias</option><option value="92059241-2080-4c75-84a1-4b2206705aa7">Vigan</option><option value="d891f5aa-f21f-4930-80b5-cbe8d5fd7515">Zamboanga</option><option value="efce438b-bae6-43ef-96cf-5431752cc013">Zamboanga City</option></select>
                    </div>

                    <div class="form-group">
                        <span>Country: <span class="required">*</span></span>
                        <select data-role="none" id="selCompanyAddressCountry" class="form-control unicase-form-control text-input"><option value="">Please select</option><option value="a7758d6b-b0e5-4426-89f4-0cb987eccd93">Aruba</option><option value="5b2b14b8-662f-4bcd-9488-eb611076b627">Afghanistan</option><option value="3a3c6580-7062-4237-942c-f309b68adfed">Angola</option><option value="70254b76-7c9e-48f8-9c53-282ebc216ffc">Anguilla</option><option value="37c2f0b4-4b77-4261-aac5-b1246c18626c">Åland Islands</option><option value="239c2209-82f0-4931-acca-e9de7fd31e3a">Albania</option><option value="94f8899a-4705-45f1-b060-90c7e7e17a7b">Andorra</option><option value="ea0874aa-d58a-4d5e-8c53-164a031fe0e8">United Arab Emirates</option><option value="8154e13e-f7ea-44b0-8378-06879b0b7d3d">Argentina</option><option value="f5140a81-6e49-4de5-b235-d09f184286b4">Armenia</option><option value="52aca56b-ef53-47f5-b63e-4eb5a69e3016">American Samoa</option><option value="8377fa19-f5a9-4700-b9be-3a72558dd953">Antarctica</option><option value="e789b9a9-e8d4-4f1a-819b-2beb6f61e359">French Southern Territories</option><option value="4dcbf9d0-8d78-4422-904b-3221a9f7be24">Antigua and Barbuda</option><option value="3e161d26-c802-4c69-a29f-9a4c33630e7d">Australia</option><option value="4bf1e7bd-99d5-4f32-adec-3825b25a26ee">Austria</option><option value="9a9e1ec3-5ddb-4052-a476-66b5a7f09f93">Azerbaijan</option><option value="33ce9887-fe47-4970-8b9e-9f87adf9ceba">Burundi</option><option value="f9039372-152d-4353-b4ee-ffe5da40b9ba">Belgium</option><option value="a2fe1632-f070-45d4-8c6f-3fb0a81eae3a">Benin</option><option value="a9fd7df2-200e-4b5a-bf95-c8f3789efa4c">Bonaire, Sint Eustatius and Saba</option><option value="82195d7c-5ea2-4ec2-b8d8-421215c0eeeb">Burkina Faso</option><option value="32f9960e-1950-45fa-9c69-d429249779da">Bangladesh</option><option value="84d07b76-4995-415e-ad20-b6df501aaff6">Bulgaria</option><option value="0c769a32-4e29-4aad-996b-3f817c6ce867">Bahrain</option><option value="2f416631-2751-4d56-9e4c-878f0225a798">Bahamas</option><option value="4ea5e39a-d7af-41ac-bbcd-efe9843e96fc">Bosnia and Herzegovina</option><option value="e12cd13b-fdac-41d1-ae2b-c12bd614dd0d">Saint Barthélemy</option><option value="9e35f7a9-8ea6-482e-add0-57d80b9855e4">Belarus</option><option value="17f48e06-7fdc-4d68-9ae2-f8ca616aba45">Belize</option><option value="53456619-6ee1-4a03-8ab8-565490583142">Bermuda</option><option value="4572be12-0121-4731-b24b-c963a1f8a320">Bolivia, Plurinational State of</option><option value="1ba80cc4-97ce-4cf3-9710-3d06a0da717c">Brazil</option><option value="914f45af-8ba4-4e89-a1ed-dd24372a902a">Barbados</option><option value="466bf664-37aa-46e0-9935-87944a743230">Brunei Darussalam</option><option value="076a98f2-54d0-4b34-b38b-45dca9732802">Bhutan</option><option value="0581584e-c893-4019-89d9-a2d65e048f9f">Bouvet Island</option><option value="07a39476-f616-45c9-91dc-33a0e9055c06">Botswana</option><option value="d6583aed-da59-4542-8ea6-3bdcc1c8e0c4">Central African Republic</option><option value="1f29d437-1376-44db-87ac-de6111a955a1">Canada</option><option value="bc59e1c9-eec3-4b39-a36f-5f2624797298">Cocos (Keeling) Islands</option><option value="172d9d53-9aad-4337-97d7-fb7e7dd4f5ae">Switzerland</option><option value="523288b1-ff83-4c56-873d-258cec10f5fc">Chile</option><option value="9f7ab85c-bad6-4c3c-bc0e-98f26ed7b48a">China</option><option value="b886b103-021c-4b49-952e-df25c309d6f4">Côte d'Ivoire</option><option value="f0d94662-4657-43f0-b25d-3db3a6bd0932">Cameroon</option><option value="2e7cc651-5b11-4fe8-ada9-b1b5fa2c6f1d">Congo, the Democratic Republic of the</option><option value="11062225-2d79-4364-922d-0b5674364999">Congo</option><option value="ce329c3b-6c13-4f8f-8790-b790618bc12a">Cook Islands</option><option value="ede57ec2-7bc7-476f-b423-cab215f03fc6">Colombia</option><option value="300a85d2-933d-45f1-b3d2-6deb8856a26d">Comoros</option><option value="8d266b3f-c1da-470e-b04a-305c11249281">Cape Verde</option><option value="f11e4b7f-7ae7-4ad7-818f-b6ced1c0c691">Costa Rica</option><option value="824abb9b-c1d2-44d9-a7b4-b1f388af01cf">Cuba</option><option value="6178cdb2-1e07-4481-977c-b11d6ae49759">Curaçao</option><option value="9cf28017-ca3e-4c12-976f-93ab7b6eccc1">Christmas Island</option><option value="407d2d84-9b74-46cb-b373-86401ed871e3">Cayman Islands</option><option value="a5ae0e43-5fe3-4278-ab9b-0b1655992d1b">Cyprus</option><option value="39fa7ff3-40f9-4206-9b16-31a21bfa0766">Czech Republic</option><option value="4161a7e0-3030-4698-a9d7-d3296c259f28">Germany</option><option value="515b9179-0926-414b-b3da-a8fbbf162d70">Djibouti</option><option value="219e22a6-02c0-4016-aef4-eb1ca19f7141">Dominica</option><option value="246a5a2e-9405-44e9-b9ff-31f49e197db2">Denmark</option><option value="ab60e069-09ff-4627-a72f-f947b7469c5d">Dominican Republic</option><option value="975a825d-9ac1-40ef-a72d-318b6a77ff76">Algeria</option><option value="f382c335-b698-49dc-94d1-743f5eec36ab">Ecuador</option><option value="a80ac0ea-e5be-4ae9-b7da-b27d32a84d99">Egypt</option><option value="fa8f041a-370c-46de-9baa-45eb95955872">Eritrea</option><option value="af67c6fc-1064-44f9-a745-23a529c837a1">Western Sahara</option><option value="165f3edd-06f9-498f-8458-8ce45f1d99b5">Spain</option><option value="502dd4b3-1625-47e9-8fa2-8ed3cf491c22">Estonia</option><option value="3ae59c60-608f-4b6b-85f2-63fb4ba35506">Ethiopia</option><option value="eadc8aea-1664-4e8f-8163-ab7af53f9e8e">Finland</option><option value="3a274b60-5241-4249-8e07-1ac27df8098c">Fiji</option><option value="acbad216-57b1-45e2-8006-f03370688ee6">Falkland Islands (Malvinas)</option><option value="6664ff49-40f0-4a1c-a71c-cb48bacab6d6">France</option><option value="fa0664b7-b327-490d-8b0f-9e617322fe4e">Faroe Islands</option><option value="1f58a622-3b0b-4e91-bf24-70afcb2a2eca">Micronesia, Federated States of</option><option value="0b672c8c-acad-40f0-8fe3-81518dff649a">Gabon</option><option value="6aedcfa3-7b0f-404e-8e8a-b024607a8581">United Kingdom</option><option value="1b3cb5ef-5f4b-4bec-8549-7f7f334b6be1">Georgia</option><option value="ebbb0f74-6b9b-44d5-a2ae-b63b1cf0c8c0">Guernsey</option><option value="424691ba-9c7e-4b75-854c-c255e961c8b8">Ghana</option><option value="bb743e1a-d2c1-47a3-a76d-abc9f83bfcb8">Gibraltar</option><option value="857195a5-7df7-4561-a368-07cbeb5eff38">Guinea</option><option value="07d2e94d-7afc-4d45-bdc5-7cb564400ca9">Guadeloupe</option><option value="d98c9b4c-a993-4777-bac6-b7229ee0ae5c">Gambia</option><option value="36c0b944-7034-467a-a381-4d9e2a303324">Guinea-Bissau</option><option value="ded3ca16-75ea-434f-8ffe-c1ca616046f7">Equatorial Guinea</option><option value="973ec71b-a761-4e8d-8879-98651eaf4118">Greece</option><option value="cf10f8e1-f343-4fab-8651-f8d0f23e7d70">Grenada</option><option value="b6aabecb-f6ff-43a7-ac0e-2aeb226c46b5">Greenland</option><option value="6a60e40c-64b7-450c-8149-5c4df4067312">Guatemala</option><option value="5c2596c5-3d85-466e-90e0-3921e2db917b">French Guiana</option><option value="b7b089eb-b8f0-481b-9e83-3c55d1f932c9">Guam</option><option value="8bf85434-ee3a-4281-a2a7-94dd856d300b">Guyana</option><option value="64fa13cb-1e76-40e0-8153-f4417f30d64b">Hong Kong</option><option value="4dcdcc90-33cb-4a8b-8179-3ed19ccac3cf">Heard Island and McDonald Islands</option><option value="232b404b-11fd-4101-adef-921a2e5d74a2">Honduras</option><option value="1e3bd171-def4-4534-927c-861120ff16b5">Croatia</option><option value="bca0063c-9dbb-47eb-88b0-7494d3f57681">Haiti</option><option value="f33690c3-f196-4b9d-aa00-85246500d61d">Hungary</option><option value="24ea9414-0a98-465d-9d0f-7c4d17a6e93e">Indonesia</option><option value="8d004745-edf8-464d-8fea-aa1d1736a362">Isle of Man</option><option value="19c4f0df-fd48-49df-a5da-8cb27199d38f">India</option><option value="81834caa-bd66-49a2-9835-8861bc7acb21">British Indian Ocean Territory</option><option value="9d571e8e-1be4-4854-b759-b97524d47653">Ireland</option><option value="2d9381a5-2e8c-4b64-a6b1-31edea52d550">Iran, Islamic Republic of</option><option value="89422a8d-5c93-4f0a-97a8-91e8498efeca">Iraq</option><option value="c27a5dc6-e4da-47ff-92a6-105b87acd9f5">Iceland</option><option value="b3a7df8d-1b27-4822-a5c8-f87886976ab8">Israel</option><option value="4913488e-9668-4f1e-ae72-604e695d0607">Italy</option><option value="3a0343b9-3a47-4da6-946a-b187d1ed8cb9">Jamaica</option><option value="a0b04e79-0b45-45f3-861a-bb618b4974fe">Jersey</option><option value="f6a8a2d8-da60-4688-ae32-d11824b0c8b9">Jordan</option><option value="f1a3e676-6f0a-4f8a-a1cf-90c539aad5ed">Japan</option><option value="1e089d7b-2536-42f0-a5db-fa9ae42570ea">Kazakhstan</option><option value="ce072c2e-37bf-4380-a516-fc35a9e85fca">Kenya</option><option value="c0f004d6-de88-4025-8533-fc610daa9b1c">Kyrgyzstan</option><option value="34341c16-6d0d-4c52-a492-9501252389ef">Cambodia</option><option value="303bbbb9-50bc-47e3-aad2-807ecdecdc13">Kiribati</option><option value="00937786-c16a-4d75-b115-dddaf798e751">Saint Kitts and Nevis</option><option value="9b3c9b93-5e2b-49f8-997b-dc2310cffe92">Korea, Republic of</option><option value="7bd5854c-0ba5-49cc-979a-b3df28b76b95">Kuwait</option><option value="da8ff0a4-a294-4b25-ba73-62f585b0cad6">Lao People's Democratic Republic</option><option value="e1d1e715-a93b-4f85-8d4c-0135d42f7ad5">Lebanon</option><option value="013348fa-0f27-436d-a215-080e27407edb">Liberia</option><option value="d0f425c3-1290-4d87-9421-6e790140e24b">Libya</option><option value="0326cc7a-4e1f-4643-a307-80c4e18cd7ff">Saint Lucia</option><option value="cfe909f6-375a-40a2-9209-4735e611649c">Liechtenstein</option><option value="39b8b3d1-590d-43b1-bc03-a73480014a7a">Sri Lanka</option><option value="2f47a3df-3ba0-4c2f-9143-b023b76f4d74">Lesotho</option><option value="0e74944f-23ad-4318-ba26-34f24394e535">Lithuania</option><option value="ec87bb09-7ba3-4cde-a50c-91f73315b7f2">Luxembourg</option><option value="c3b08590-afac-4ec8-8cf5-5f49a3f27f79">Latvia</option><option value="695be870-4e41-4e44-820c-c033dae2a575">Macao</option><option value="c8a5056d-fc8f-4f9f-83bf-fbcc974ce683">Saint Martin (French part)</option><option value="7ad0a004-3598-49ed-bad7-499cdac45653">Morocco</option><option value="a003048d-2b58-4fa3-a3a5-72b546c13ed5">Monaco</option><option value="acbc9675-b987-4adb-9dbd-c99dec5390cd">Moldova, Republic of</option><option value="de79b9bc-9a7c-4f91-882f-0ff23f02dbff">Madagascar</option><option value="f174b5f4-e5e3-4a4f-9b54-525683016bbd">Maldives</option><option value="52198295-52c9-4901-b873-6598350dec37">Mexico</option><option value="4df46d8c-3898-4828-9875-01736517cb29">Marshall Islands</option><option value="9f8b1b6c-ec91-4d71-aa8c-acb1ec43129b">Macedonia, The Former Yugoslav Republic of</option><option value="1de24f48-2396-46e9-bfb6-28c5288628bf">Mali</option><option value="8e440261-da15-4917-a6f9-5f5e720f33ad">Malta</option><option value="37775aa3-839f-4675-8a6d-efa0fa9f4cd1">Myanmar</option><option value="7c337a37-3575-4a71-9b92-0dd4a2d57a71">Montenegro</option><option value="f5933f5f-06fd-4eb9-a336-5cab04fa94ee">Mongolia</option><option value="623a2a8f-5810-4b22-9779-4a9288311f48">Northern Mariana Islands</option><option value="106eab0f-e0ac-48a2-9881-5588df17c218">Mozambique</option><option value="f1d3ba78-8671-4d02-b2e0-6e89ce66c1d4">Mauritania</option><option value="6d0d4c64-d95d-4a9c-8673-a183e9b58953">Montserrat</option><option value="66759d20-5a3e-410c-bab9-dc01d6d4f736">Martinique</option><option value="9ffd3973-8258-4f1b-b6e6-94d2cb7f09c3">Mauritius</option><option value="9ee66031-bf45-4f3f-a435-26e6c4d6ed76">Malawi</option><option value="65c3b0d1-7793-423c-898e-f710054e48c0">Malaysia</option><option value="dab61ab0-6b77-4fc7-ac8c-2a99de0bc239">Mayotte</option><option value="08235ffd-9f0f-46e8-ada2-cd137667029b">Namibia</option><option value="311d12c3-ba2a-45b7-9afd-29610faebce0">New Caledonia</option><option value="84e8fadb-1402-4ee0-a6dc-5eb69d9214d7">Niger</option><option value="f6d2f549-9774-46ea-8d29-d127b9a3bfa0">Norfolk Island</option><option value="e6359c45-bec7-4491-8822-0a0d35e849a8">Nigeria</option><option value="dad91ada-3c4a-43c6-8833-f352d472e6a6">Nicaragua</option><option value="d2fe04ba-06b7-41a7-952b-23510e526c63">Niue</option><option value="54262a33-c028-49dd-9918-e5f6ef1e0ce1">Netherlands</option><option value="03413ac9-3584-4a0a-aef2-47dd8f15e08a">Norway</option><option value="1129a928-bb69-4a1f-8f4b-235fe5d91f09">Nepal</option><option value="f4a410ad-acb5-4eef-9fef-1dc41250fcec">Nauru</option><option value="c6d5939d-406f-471d-909c-95139a31d91c">New Zealand</option><option value="04f694c1-72cd-4e5b-9363-92218500441a">Oman</option><option value="caeb576a-91a2-4a84-910d-1ab667ed0276">Pakistan</option><option value="3db15cbc-ca24-4b58-b288-58bfebaf3813">Panama</option><option value="80011c92-65ae-4ec9-91c5-867a6ad732db">Pitcairn</option><option value="28c3fb3f-4a07-41f4-a3fb-fdc52c24874e">Peru</option><option value="e783ed13-aa98-4cc1-918e-d49b05c0167e">Philippines</option><option value="3ae887e7-f81e-4e6d-aef6-f2b0ec447bd0">Palau</option><option value="38d83f96-1dcc-40f3-89d0-e9c291c91401">Papua New Guinea</option><option value="d6452fe0-40be-4532-aff5-f8a427f5299c">Poland</option><option value="966e4cbf-fbd2-403b-bf83-8d67b426d02b">Puerto Rico</option><option value="cf67b226-a75e-44b5-9e09-dc809c77beab">Korea, Democratic People's Republic of</option><option value="8f456808-888f-4c8c-a003-dfab68c54cd2">Portugal</option><option value="7eb5414a-8de1-4406-bcda-ea25ed9c4c9f">Paraguay</option><option value="62d063f0-57e7-4a01-9d28-6fe9fbd68086">Palestinian Territory, Occupied</option><option value="2900e56c-42fe-4af9-81dc-c9e6e003849f">French Polynesia</option><option value="763400dc-1c1c-4e52-a540-4ad562efb5a7">Qatar</option><option value="2e42d27d-a964-434d-9763-3cb86473a94b">Réunion</option><option value="767e570f-01b0-4729-8a0f-02d36dc85a8c">Romania</option><option value="7fb6ce24-b743-4508-b905-baedd0dfe5e9">Russian Federation</option><option value="4a5218ed-a7d9-4f42-bcb9-75382f85f957">Rwanda</option><option value="d4927a3d-8a97-4c07-a3d9-df7fb1dc16a5">Saudi Arabia</option><option value="63990b06-43c0-4485-96c2-e0a8dd4a479e">Sudan</option><option value="0312dfb3-9742-4edf-abda-f5ed747e7e26">Senegal</option><option value="a0d9f8fb-ca30-428b-9909-b01eb8b5a192">Singapore</option><option value="32aae313-d18e-4053-bfa0-52cda953ccd5">South Georgia and the South Sandwich Islands</option><option value="56659c95-18fd-4b1a-b34e-fda90c3798ee">Saint Helena, Ascension and Tristan da Cunha</option><option value="693772c7-15b0-4ae2-916b-9f12bed95cd2">Svalbard and Jan Mayen</option><option value="a179905d-7bdd-46a2-94a2-d45780674edc">Solomon Islands</option><option value="339afcc1-aaed-4493-a271-7b0096292fc1">Sierra Leone</option><option value="bb92a919-418d-4528-8920-8115682cde80">El Salvador</option><option value="54450617-033c-4ebd-bd50-46588d6b908e">San Marino</option><option value="0ecc5cc2-5fa5-4179-9456-6468cfc3424f">Somalia</option><option value="be57837d-56f4-403c-8f9e-8cefa61f5598">Saint Pierre and Miquelon</option><option value="86b74f26-8d53-4b6d-9492-8fd82cd7b676">Serbia</option><option value="1f4a26c0-7d51-45ea-94a3-6dba09cdd65b">South Sudan</option><option value="3c2a479e-70cc-4a8f-ada0-d66fbad9c47f">Sao Tome and Principe</option><option value="34acecb8-9c33-4d6a-96ab-a1175d72d64a">Suriname</option><option value="a0dcc790-9ab7-4e41-b5ce-b2d8473984d6">Slovakia</option><option value="6e3ff603-83e4-4c36-a337-1d81fbbd91bc">Slovenia</option><option value="d8aa4da9-a829-427a-bac7-0f51c1100eac">Sweden</option><option value="cb1df2dd-334f-44ca-bd1f-7e2924be1ba0">Swaziland</option><option value="56eef76f-a59e-4076-92ce-77146f842fef">Sint Maarten (Dutch part)</option><option value="88bf0a43-470d-4eca-bd2d-ea3cca5397c5">Seychelles</option><option value="5793d89e-2a98-406a-90cf-7a89c4757ee4">Syrian Arab Republic</option><option value="05f4e383-4931-47f3-b83d-c2b4ee5db31d">Turks and Caicos Islands</option><option value="f23255b4-3261-47a2-8fa7-37bf7378acf5">Chad</option><option value="3324bdcc-f5bb-4b57-8cbd-d1e12aaf4d07">Togo</option><option value="7003da49-6da0-48fb-b397-96548c034754">Thailand</option><option value="fec482c2-77ec-4c4d-a5cf-c29862e689cc">Tajikistan</option><option value="9958b7f7-3984-46b6-8a22-69a86bfd3100">Tokelau</option><option value="73a22c6c-0a63-48f7-a111-03fdb46ec030">Turkmenistan</option><option value="4d28c805-ed11-4836-869e-708aa94c9f95">Timor-Leste</option><option value="9cf6b494-b0b4-4060-815c-9ba03854d270">Tonga</option><option value="20719875-a313-4f7a-8363-7ee591207463">Trinidad and Tobago</option><option value="e325efc1-5b1e-4bb7-89c7-8d05510aa53c">Tunisia</option><option value="dbefb87d-1ccf-42a1-9082-041f035c3af5">Turkey</option><option value="1f69fb0b-bc2a-47fb-b432-7360d01e94c2">Tuvalu</option><option value="74fb9683-0850-4567-8020-44175c8f1169">Taiwan, Province of China</option><option value="1ee3a440-f265-4955-9489-9a8a45676587">Tanzania, United Republic of</option><option value="bb0ecf88-de10-41d4-b1b5-549f2bbdda93">Uganda</option><option value="742ae33e-6ec0-43e7-9463-31a409678dbe">Ukraine</option><option value="29cd073f-3900-4683-8b6e-5ef8396db511">United States Minor Outlying Islands</option><option value="df54caf4-be91-4152-b0d0-e4b160a55f84">Uruguay</option><option value="e4e48cd8-5a9b-4764-b709-8219566f8958">United States of America</option><option value="babb1311-73fa-42bb-9bff-c9f78a89f441">Uzbekistan</option><option value="dfbb6f15-372c-49b5-97ac-e3f1dfd8ed63">Holy See (Vatican City State)</option><option value="ce26a32a-9aee-423b-8e0a-b24b3f23575b">Saint Vincent and the Grenadines</option><option value="7d475246-e452-4c2c-9ac1-8d95d9555107">Venezuela, Bolivarian Republic of</option><option value="236e3f30-1bb6-4c78-9139-8d7b53381a3d">Virgin Islands, British</option><option value="6a5e99ee-7350-4320-b42a-62c9c49f34c1">Virgin Islands, U.S.</option><option value="9dd33e0a-3d21-4fcc-a68f-5ce4b5d39280">Viet Nam</option><option value="a4e1a115-ac8d-4b2b-96dd-e5c5b12cff98">Vanuatu</option><option value="7a25b873-ae5a-419d-a224-d3348e7685c2">Wallis and Futuna</option><option value="0aee1f8a-21b0-4f78-87bf-9d5f7c5e8ec3">Samoa</option><option value="7bdb372a-0fdf-45c0-a2a6-983ea9f720f7">Yemen</option><option value="f4ae8eb5-8ddb-471c-9afd-94579ead86ed">South Africa</option><option value="dc9ee313-67e6-483a-b8d4-ec00066256a7">Zambia</option><option value="5f462915-66c2-4ad4-bff0-2b7b9a3baa80">Zimbabwe</option></select>
                    </div>
                </form>
            </div>
            <div class="modal-footer">
                <span id="divCompanyErrMsg" style="color:red;"></span>
                <button data-role="none" type="button" id="btnCancelOrg" class="btn custom-btn-theme-c btn-less-width btn-sm">Back</button>
                <button data-role="none" type="submit" id="btnOkOrg" class="btn custom-btn-theme-b btn-less-width btn-sm">Create my Account</button>
            </div>
        </div>
    </div>
</div>

<script>

    function statusChangeCallback(response) {  // Called with the results from FB.getLoginStatus().
        console.log('statusChangeCallback');
        console.log(response);                   // The current login status of the person.
        if (response.status === 'connected') {   // Logged into your webpage and Facebook.
            testAPI();
        } else {                                 // Not logged into your webpage or we are unable to tell.
            document.getElementById('status').innerHTML = 'Please log ' +
              'into this webpage.';
        }
    }


    function checkLoginState() {               // Called when a person is finished with the Login Button.
        FB.getLoginStatus(function (response) {   // See the onlogin handler
            statusChangeCallback(response);
        });
    }


    window.fbAsyncInit = function () {
        FB.init({
            appId: '1380757268756754',
            cookie: true,                     // Enable cookies to allow the server to access the session.
            xfbml: true,                     // Parse social plugins on this webpage.
            version: 'v4.0'           // Use this Graph API version for this call.
        });


        FB.getLoginStatus(function (response) {   // Called after the JS SDK has been initialized.
            //statusChangeCallback(response);        // Returns the login status.
        });
    };


    (function (d, s, id) {                      // Load the SDK asynchronously
        var js, fjs = d.getElementsByTagName(s)[0];
        if (d.getElementById(id)) return;
        js = d.createElement(s); js.id = id;
        js.src = "https://connect.facebook.net/en_US/sdk.js";
        fjs.parentNode.insertBefore(js, fjs);
    }(document, 'script', 'facebook-jssdk'));


    function testAPI() {                      // Testing Graph API after login.  See statusChangeCallback() for when this call is made.
        console.log('Welcome!  Fetching your information.... ');
        FB.api('/me', function (response) {
            console.log('Successful login for: ' + JSON.stringify(response));
            document.getElementById('status').innerHTML =
              'Thanks for logging in, ' + response.name + '!';
            document.getElementById('btnFBLogout').style.display = 'block';
        });
    }

    var accesstoken;
    function logout() {
        //FB.logout(accesstoken)
        FB.logout(function (response) {
            console.log('logged out - ' + JSON.stringify(response))
            document.getElementById('btnFBLogin').style.display = 'block';
            document.getElementById('btnFBLogout').style.display = 'none';
            FB.Auth.setAuthResponse(null, 'unknown');
        });
    }

    function login() {
        FB.login(function (response) {
            accesstoken = response;
            console.log(accesstoken);
            console.log('logged in - ' + JSON.stringify(response))
            document.getElementById('btnFBLogin').style.display = 'none';
            document.getElementById('btnFBLogout').style.display = 'block';
            FB.api('/me', function (response) {
                document.getElementById('status').innerHTML =
              'Thanks for logging in, ' + response.name + '!';
            });
        });
    }

</script>

<!--<fb:login-button scope="public_profile,email" onlogin="checkLoginState();">
</fb:login-button>-->
<button data-role="none" id="btnFBLogin" style="display:block;" onclick="login()">Login</button>
<button data-role="none" id="btnFBLogout" style="display:none;" onclick="logout()">Logout</button>

<div id="status">
</div>
<div id="fb-root"></div>
<script async defer crossorigin="anonymous" src="https://connect.facebook.net/en_US/sdk.js#xfbml=1&version=v4.0&appId=1380757268756754&autoLogAppEvents=1"></script>
<div class="fb-login-button" data-width="" data-size="large" data-button-type="login_with" data-auto-logout-link="true" data-use-continue-as="false"></div>
