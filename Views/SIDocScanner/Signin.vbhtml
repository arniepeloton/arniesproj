@Code
    ViewData("Title") = "1Mobiz Inc."
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code

@Section head
    @*<link rel="Stylesheet" type="text/css" href="/Content/bootstrap/css/bootstrap.css" />
        <link rel="Stylesheet" type="text/css" href="/Content/css/SignIn.css" />*@
    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v='], function () {
                require([GetPathFormat('/Scripts/sources/SIDocScanner/SignIn.js?crc=' + assetversion)]);
            })
        })
    </script>

    <style type="text/css">
        body {
            overflow-x: hidden;
        }
    </style>
End Section

<div style="position: absolute; left: 50%; top: 50%; transform: translate(-50%, -50%); -ms-transform: translate(-50%, -50%); -webkit-transform: translate(-50%, -50%);">
    <div class="mb-5 text-center">
        <h2 class="text-primary">SI Document Scanner</h2>
        <h4 class="text-secondary">Login</h4>
    </div>

    <div class="alert alert-danger mb-3" role="alert" id="spanError" style="display:none;text-align:center">
        Invalid username and password
    </div>

    <form id="frmPeopleLogger" method="POST" autocomplete="off">
        <div class="mb-3">
            <div class="input-group mb-3">
                <span class="bi-person-fill input-group-text border-primary bg-primary text-white"></span>
                <input type="text" placeholder="Username" class="form-control form-control-lg border-primary" data-role="none" name="username" id="txtUsername" />
            </div>
        </div>
        <div class="mb-4">
            <div class="input-group mb-3">
                <span class="bi-key-fill input-group-text border-primary bg-primary text-white"></span>
                <input type="password" placeholder="Password" class="form-control form-control-lg border-primary" data-role="none" name="password" id="txtPassword" />
            </div>
        </div>
        <button id="btnSignIn" class="btn btn-primary btn-lg btn-block" data-role="none">Sign In</button>
    </form>
</div>