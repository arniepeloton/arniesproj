@Code
    ViewData("Title") = "EAS"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code

@Section head
    @*<link rel="Stylesheet" type="text/css" href="/Content/bootstrap/css/bootstrap.css" />
    <link rel="Stylesheet" type="text/css" href="/Content/css/SignIn.css" />*@
    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v='], function () {
                require([GetPathFormat('/Scripts/sources/SignIn/ERPSignInInfo.js?crc=' + assetversion)]);
            })
        })
        //require([GetPathFormat('/Scripts/pushnotification/push.min.js?crc=')]);
        //require([GetPathFormat('/Scripts/pushnotification/serviceWorker.js?crc=')]);
    </script>

    <script>
        //var pushButton = document.getElementById('btnNotif');

        //if ('serviceWorker' in navigator && 'PushManager' in window) {
        //    console.log('Service Worker and Push is supported');

        //    navigator.serviceWorker.register('/Scripts/service.js')
        //    .then(function (swReg) {
        //        console.log('Service Worker is registered', swReg);

        //        swRegistration = swReg;
        //    })
        //    .catch(function (error) {
        //        console.error('Service Worker Error', error);
        //    });
        //} else {
        //    console.warn('Push messaging is not supported');
        //    pushButton.innerHTML = 'Push Not Supported';
        //}

        //navigator.serviceWorker.register('/Scripts/service.js');
        //Notification.requestPermission(function (result) {
        //    if (result === 'granted') {
        //        navigator.serviceWorker.ready.then(function (registration) {
        //            registration.showNotification('Notification with ServiceWorker');
        //        });
        //    }
        //});
    </script>

@*<script src="/Scripts/pushnotification/push.min.js"></script>
<script src="/Scripts/pushnotification/serviceWorker.min.js"></script>*@
<script>
    function start() {
        if ('Notification' in window) {
            Notification.requestPermission();
        }
        Push.create("Hello world!", {
            body: "How's it hangin'?",
            icon: 'https://previews.123rf.com/images/coolvectorstock/coolvectorstock1808/coolvectorstock180802556/106911764-blood-sample-vector-icon-isolated-on-transparent-background-blood-sample-logo-concept.jpg',
            timeout: 4000,
            onClick: function () {
                window.focus();
                this.close();
            }
        });
    }

    function clear() {
        Push.clear();
    }
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

<div class="d-flex align-items-start flex-column bd-highlight h-100 w-100 position-absolute">
    <div class="mb-auto d-flex align-items-center justify-content-center bd-highlight overflow-auto w-100 h-100">
        <div class="p-3">
            <div class="mb-5 text-center">
                <h1 style="color:#335F8E; font-weight:bold;">EAS</h1>
                <h5>Enterprise Application System</h5>
                <p>Powered by <img src="/Content/images/MobizNewLogo.png" alt="Mobiz" style="vertical-align:baseline;" width="70" align="bottom"></p>
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
                <button id="btnSignIn" disabled class="btn btn-primary btn-lg btn-block" data-role="none">Sign In</button>
            </form>

            <div class="mb-3 mt-3">
                <a href="../AccountRecovery" data-role="none" style="text-decoration:none;">Forgot your password?</a>
            </div>

            <div class="mb-3">
                Don't have a Mobiz account?
                <a href="../SignUp/ImportantReminders" data-role="none" style="text-decoration:none;">Create My Personal Mobiz Account</a>
            </div>

            <div id="divDebugTestx"></div>
        </div>
    </div>    
</div>