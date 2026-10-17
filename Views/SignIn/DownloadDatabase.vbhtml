@Code
    ViewData("Title") = "Download Database"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code

@Section head

    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
                require([GetPathFormat('/Scripts/sources/SignIn/DownloadDatabase.js?crc=' + assetversion)]);
            })
        })        
    </script>

<style type="text/css">
    .ui-page-active, body {
        outline: none;
        background-color: #f8f9fa !important;
    }
</style>


    
End Section

<div class="container" custom-data-role="page" id="pageDatabases" style="display:block;">
    <h1 class="display-5 mb-4 mt-4">@ViewData("Title")</h1>

    <div data-role="content">
        <div class="mb-3">Choose Database to download: </div>
        <table id="tblDatabase" style="width:100%">
            <tr>
                <td>
                    <div style="width:100%; text-align:center;">
                        <div class="spinner-border text-primary" role="status"></div>
                        <br />
                        Please wait ...
                    </div>
                </td>
            </tr>
        </table>
        <div class="mt-4 mb-4 row text-center">
            <div class="offset-lg-4 col-lg-4">                
                <button data-role="none" id="btnDownloadDatabase" class="btn btn-success btn-lg btn-block">Download Database(s)</button>
                <a data-role="none" data-rel="back" class="btn btn-secondary btn-lg btn-block">Back</a>
            </div>            
        </div>

        <div data-role="popup" id="popupMenu" style="display:none">
            <div data-role="popup" id="dvpopup" data-theme="a" class="ui-corner-all" data-dismissible="false" data-overlay-theme="a">
                <p id="popMessage" style="padding:10px 20px;text-align:center"></p>
            </div>
        </div>
    </div>
</div>

<div class="container" custom-data-role="page" id="pageDownloadError" style="display:none;">
    <h1 class="display-5 mb-4 mt-4">Importing Error</h1>

    <div data-role="content">
        <h3>Sorry, this device is not allowed to download database records. </h3>

        <br />
        <h4>If you want to use this device for transactions, please register using one of these options:</h4>
        <br />

        <h2>
            Option 1:
        </h2>

        <hr />

        If you are a super administrator or someone with device enrollment privileges, you can register this device online using the information below:
        <a href="#" data-toggle="modal" data-target="#HowOption1">How?</a>
        <br /> <br />

        <fieldset data-role="controlgroup">
            <legend> <b> Device Model: </b></legend>
            <label id="lblPhoneModel"> </label>
        </fieldset>

        <fieldset data-role="controlgroup">
            <legend> <b> IMEI: </b></legend>
            <label id="lblIMEI"> </label>
        </fieldset>

        <fieldset data-role="controlgroup">
            <legend> <b> UUID: </b></legend>
            <label id="lblUUID"> </label>
        </fieldset>

        <fieldset data-role="controlgroup">
            <legend> <b> Platform: </b></legend>
            <label id="lblOS"> </label>
        </fieldset>
        <hr />

        <br />
        <h2>Option 2:</h2>
        <hr />
        You can also register this device by scanning the QR code below with a smartphone that has super administrator or device enrollment privileges.
        <a href="#" data-toggle="modal" data-target="#HowOption1">How?</a>
        <br /> <br />
        <div align="center" id="dvDeviceQrCode" class="text-center"> </div>
        <hr />

        <br />
        <h2>Option 3:</h2>
        <hr />
        You can enroll this device by sending a request to your super administrator or person with device enrollment privileges.
        You will need internet access for this option.  Your device will be registered once your super administrator (or other authorized person) approves your request.
        <br /> <br />
        <div class="text-center">
            <button data-role="none" class="btn btn-success btn-lg" id="btnSendRequest">Click here to send request</button>
        </div>        
        <hr />
        <div class="text-center mb-4">
            <button data-role="none" class="btn btn-secondary" id="btnDone">Done</button>
        </div>

        <div id="HowOption1" class="modal fade">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Instructions</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <p>
                            Please note that only registered devices can be used with your Mobiz applications and databases.  Even if the user name and password credentials of users are correct, they will not be granted access if the device that they are using is not registered under your organization.  This is one of the many ways Mobiz helps protect your application and data.
                        </p>

                        <p>
                            If you are a super administrator (or someone with device enrollment privileges), you can enroll a device by signing in on a registered device or through the mobiz-apps.com website and then go to:
                        </p>

                        <ol>
                            <li>System Administration</li>
                            <li>System Settings</li>
                            <li>Security Settings</li>
                            <li>Devices</li>
                            <li>Click on the button labelled: Enroll a Device or Computer</li>
                            <li>Scan the QR Code on the device that you want to be registered or manually fill in the data fields required</li>
                            <li>Click "Done" when finished</li>
                        </ol>

                        <p>
                            After registration, the device should be ready for use with Mobiz in your organization.
                        </p>                        
                    </div>
                    <div class="modal-footer">
                        <button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-block btn-lg">Close</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
    
<div class="container" custom-data-role="page" id="pageDownloadDatabaseSuccessful" style="display:none;">
    <h1 class="display-5 mb-4 mt-4">@ViewData("Title")</h1>

    <div data-role="content">
        <div class="text-center">
            <div class="bg-white border text-primary rounded p-3">
                <div class="bi-check-circle-fill text-primary mb-4" style="font-size:100px;"></div>
                <p>Your database is ready. Please sign in to start using Mobiz Complete Retail.</p>
            </div>            
            <button data-role="none" class="btn btn-success btn-lg mt-4" id="btnSignInRedirect">Restart and sign in</button>
        </div>        

        @* Your database is ready. To ensure the accuracy and integrity of the reports coming from your data, it is recommended that you
            run the Data Integrity Test. The test may run for a few minutes so please be patient. Please do not click the back button, refresh page, or terminate
            the test while it is in the middle of processing. Thank you.
            <div style="text-align: center">
                <a id="btnTest" data-role="button">
                    <h3>Run Data Integrity Test</h3>
                </a>
            </div>
            <div style="text-align: center">
                <a id="btnSignInRedirect" data-role="button">
                    <h3>Proceed to Sign In</h3>
                </a>
            </div>*@

    </div>

</div>

<div class="container" custom-data-role="page" id="pageProgress" style="display:none;">
    <div data-role="content">
        <div id="divProgressLog"></div>
    </div>
</div>