@Code
    ViewData("Title") = "Mobiz Portal"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code
<link href="https://maxcdn.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css" rel="stylesheet">
<style type="text/css">
    html{
        height:100%;
    }
    body{
        background: rgb(19,16,149);
        background: linear-gradient(0deg, rgba(19,16,149,1) 0%, rgba(60,174,250,1) 100%); 
        top: 0; 
        left: 0; 
        right: 0; 
        bottom: 0; 
        color:#fff;
        background-repeat:no-repeat;
    }
    .login-container {
        width:300px;
        background-color: #FFF;
        padding:20px;
        position: absolute;
        left: 50%;
        top: 50%;
        transform: translate(-50%, -50%);
        border-radius:10px;
        color:#007bff;
        box-shadow: 0px 0px 60px #2196F3;
    }
    .ui-loader {
        display:none !important;
    }
</style>

<script type="text/javascript">
    require([GetPathFormat('/Scripts/sources/MobizPortal/MobizPortal.js?crc=')]);
</script>

<div class="login-container">
    @*<img src="/images/New-Mobiz-Logo.png" style="width:150px;" class="img-responsive">*@
    <h4 data-role="none">LOGIN</h4>
    <h6 style="text-align:center;">Welcome to Mobiz Portal</h6>
    <br />
    <form id="formMobizPortal" method="post">
        <div id="divAlertMsg" class="alert alert-danger" style="display:none;" role="alert">
            Invalid username or password
        </div>
        <div class="input-group mb-3">
            <div class="input-group-prepend">
                <span class="input-group-text"><span class="fa fa-user"></span></span>
            </div>
            <input data-role="none" type="text" class="form-control form-control-lg" id="txtUname" placeholder="Enter username">
            <span class="invalid-feedback">
                Please enter username.
            </span>
        </div>
        <div class="input-group mb-3">
            <div class="input-group-prepend">
                <span class="input-group-text"><span class="fa fa-lock"></span></span>
            </div>
            <input data-role="none" type="password" class="form-control form-control-lg" id="txtPassword" placeholder="Enter password">
            <span class="invalid-feedback">
                Please enter password.
            </span>
        </div>
        
        <br />
        <button data-role="none" id="btnLogin" type="submit" class="btn btn-success btn-lg btn-block">Login</button>
    </form>    
</div>