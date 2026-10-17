@Code
    ViewData("Title") = "1Mobiz Inc."
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
    <link rel="Stylesheet" type="text/css" href="/Content/bootstrap/css/bootstrap.css" />
    <link rel="Stylesheet" type="text/css" href="/Content/css/SignIn.css" />
    <script>
        require([GetPathFormat('/Scripts/sources/BOMProjectExcel/SignInInfo.js?crc=')]);
    </script>
End Section

<div data-role="page" class="container" style="background-color:#F7F7F7;">
    <div class="maincontainer">
        <div class="row" align="center">
            <div class="borderline">
                <h1 style="color:#335F8E; font-weight:bold;">Project Manager</h1>
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
                    <a href="../SignUp/ImportantReminders" data-role="none">Create your Mobiz Account</a>
                </span>
            </div>
        </div>

        <br />
        
    </div>

</div>
