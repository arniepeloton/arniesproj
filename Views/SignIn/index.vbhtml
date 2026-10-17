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
                require([GetPathFormat('/Scripts/sources/SignIn/SignInInfo.js?crc=' + assetversion)]);
            })
        })        
    </script>

    <style type="text/css">
        body {
            overflow-x: hidden;
        }
        .non-vending {
            display: none
        }
        .offline {
            display: none
        }
        .online {
            display: none
        }
        .logo {
            width: 296px;
            height: 120px;
            margin: auto;
            background-image: url("/Content/images/MobizNewLogo.png");
            background-size: contain;
            background-repeat: no-repeat;
            background-position: 50% 50%;
        }
    </style>
End Section

<div class="d-flex position-absolute w-100 h-100 justify-content-center align-items-center section">
    <div class="spinner-border text-primary" role="status"></div>
</div>

<div class="section d-none">
    <div class="d-flex justify-content-center flex-column p-3 align-items-center vh-100">
        <div class="mb-5 text-center">
            <div class="logodiv" style="visibility:hidden">
                @*<img class="img-responsive" src="/Content/images/MobizNewLogo.png" height="120px" width="296px" style="padding-top: 20px; object-fit: cover" />*@
                <div class="logo"></div>
            </div>
            <span class="mb-3 non-vending" style="color:#848385;">Business-Grade Apps In Your Pocket</span>
            <div class="online lblVersion" style="margin-top:16px">Mobiz Complete Retail</div>
            <div class="online lblVersion" name="divVersion"></div>
        </div>

        <div class="alert alert-danger mb-3" role="alert" id="spanError" style="display:none;text-align:center">
            Invalid username and password
        </div>

        <div class="alert alert-danger mb-3" role="alert" id="divErrorLowFreeSpace" style="display:none;text-align:center">
            @*<span class="bi-exclamation-triangle-fill" style="font-size:50px;"></span>*@
            <div id="spanErrorLowFreeSpace"></div>
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

        <div id="divDeviceSerial" method="POST" autocomplete="off" style="display:none;">
            <div class="mb-4">
                <label class="form-label" style="color:#848385;" for="txtDeviceSerial">New device serial:</label>
                <input type="text" class="form-control form-control-lg" data-role="none" id="txtDeviceSerial" />
            </div>

            <button id="butUseDeviceSerial" class="btn btn-primary btn-lg btn-block" data-role="none">Use this serial number</button>
            <button id="butGenerateDeviceSerial" class="btn btn-success btn-lg btn-block" data-role="none">Let Mobiz generate a serial number</button>
        </div>

        <div id="divCompanyOwnedOrFranchisee" method="POST" autocomplete="off" style="display:none;">
            <div class="alert alert-info mb-3" role="alert" style="text-align:center">
                Is your organization a company-owned entity or a franchisee?
            </div>
            <div class="form-check mb-3">
                <label class="form-check-label">
                    <input data-role="none" type="radio" class="form-check-input" name="optCompanyOwnedOrFranchisee" value="company-owned" data-iconpos="notext" />
                    Company-Owned
                </label>
            </div>

            <div class="form-check mb-3">
                <label class="form-check-label">
                    <input data-role="none" type="radio" class="form-check-input" name="optCompanyOwnedOrFranchisee" value="franchisee" data-iconpos="notext" />
                    Franchisee
                </label>
            </div>

            <button id="butCompanyOwnedOrFranchisee" class="btn btn-primary btn-lg btn-block" data-role="none">Confirm</button>
        </div>

        <div class="mb-3 mt-3 online">
            <a href="../AccountRecovery" data-role="none" style="text-decoration:none;">Forgot your password?</a>
        </div>

        <div class="mb-3 online">
            Don't have a Mobiz account?
            <a href="../SignUp/ImportantReminders" data-role="none" style="text-decoration:none;">Sign Up</a>
        </div>

        @*<video muted preload="auto" class="vh-100" style="margin: auto; max-width: 100vw" controls>
                <source src="/Content/VendingMachine/cfe39df6-8809-4973-9515-810df75aed12/ads_03262025_080536am_f43747ce-1f94-4cb9-98a1-dc46023fa3fc.webm" />
            </video>*@

        <div id="divDebugTest" style="display:none">
            <div class="mb-3">
                <a name="lnkPinApp" href="#" data-role="none" style="text-decoration:none;">Pin App (debug only)</a>
            </div>
            <div class="mb-3">
                <a name="lnkUnpinApp" href="#" data-role="none" style="text-decoration:none;">Unpin App (debug only)</a>
            </div>
            <div class="mb-3">
                <a name="lnkRemoveAsDeviceOwner" href="#" data-role="none" style="text-decoration:none;">Remove as device owner (debug only)</a>
            </div>
            <div class="mb-3">
                <a name="lnkUsbSerialSend" href="#" data-role="none" style="text-decoration:none;">UsbSerialSend (debug only)</a>
            </div>
        </div>
    </div>
</div>