@Code
    ViewData("Title") = "CRM"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
    <link rel="Stylesheet" type="text/css" href="/Content/bootstrap/css/bootstrap.css" />
    <link rel="Stylesheet" type="text/css" href="/Content/css/SignIn.css" />
    <script>
        require([GetPathFormat('/Scripts/sources/SignIn/CRMSignInInfo.js?crc=')]);
    </script>
End Section

<style type="text/css">
.custom-list
{
    list-style-type:none;
    padding:0px;
    margin: 0;
}
.li-theme-c > li a
{
    background-color: #f2f2f2 !important;
    color:#000 !important;
    background-image: none !important;
    transition: background-color 0.2s;
    text-shadow: 0px 1px 1px #C3C3C3 !important;
}
.li-theme-c > li a:hover
{
    background-color: #e0e0e0 !important;
}

.li-theme-c > li
{
    background-color:#FFF !important;
}

.noBorderList li
{
    border-style:none; 
}
.custom-list > .ui-li > .ui-btn-inner.selectedList
{
    border: 2px solid #3498db !important;
}

.custom-list > li > .ui-btn-inner > .ui-btn-text > a > .ui-li-thumb
{
    left: 0px !important;
    top: 2px !important;
    margin: 10px !important;
}

.custom-list > li > .ui-btn-inner > .ui-btn-text > .ui-link-inherit
{
    min-height: 80px !important;   
}

.custom-list-sm > li > .ui-btn-inner > .ui-btn-text > .ui-link-inherit
{
    min-height: 0px !important;   
}

.custom-list > .ui-li > .ui-btn-inner 
{
    border: 1px solid #DDD !important;   
}

.custom-list > li 
{
    margin: 15px 0px 5px 0px !important;   
}

.custom-list > li > .ui-li .ui-btn-text a.ui-link-inherit
{
    white-space:normal;   
}
</style>


<div data-role="page" class="container">
    <div class="maincontainer">
        <div class="row" align="center">
            <div class="borderline">
                <h1 style="color:#335F8E; font-weight:bold;">CRM</h1>
                <br />
                <p>Powered by <img src="/Content/images/MobizNewLogo.png" alt="Mobiz" style="vertical-align:baseline;" width="70" align="bottom"></p>
            </div>
        </div>
        <br />
         <div class="alert alert-danger alert-dismissible" role="alert" id="spanError" style="display:none;text-align:center">
            Invalid username and password
        </div>
        <form id="frmPeopleLogger" method="POST" autocomplete="off">
        <div class="row">
            <div class="form-horizontal">
                <div class="col-md-3">
                    <label class="control-label" style="color:#848385;" for="txtUsername">Username</label>
                </div>
                <div class="col-md-9">
                    <input type="text" class="form-control" data-role="none" name="username" id="txtUsername"  />
                </div>
            </div>
        </div>
        <br />
        <div class="row">
            <div class="form-horizontal">
                <div class="col-md-3">
                    <label class="control-label" style="color:#848385;" for="txtPassword">Password</label>
                </div>
                <div class="col-md-9">
                    <input type="password" class="form-control" data-role="none" name="password" id="txtPassword" />
                </div>
            </div>
        </div>
        <br />
        <div class="row">
            <div class="col-md-offset-3 col-md-9">
                <button id="btnSignIn" class="btn btn-primary" data-role="none" style="width:100%;padding:5px">Sign In</button>
            </div>
        </div>
        </form>
        <br />
        <div class="row">
            <div class="col-md-offset-3 col-md-9">
                <span style="font-size:14px"><a href="../AccountRecovery" data-role="none">Forgot your password?</a></span>
            </div>
        </div>
        <div class="row">
            <div class="col-md-offset-3 col-md-9">
                <span style="font-size:14px">
                    Don't have a Mobiz account?
                    <a href="../SignUp/ImportantReminders" data-role="none">Create My Personal Mobiz Account</a>
                </span>
            </div>
        </div>

        <br />
        <div id="divDebugTest"></div>
    </div>
</div>
