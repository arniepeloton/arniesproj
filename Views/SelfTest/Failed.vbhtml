@Code
    ViewData("Title") = "Data Integrity Test"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
<script>
    require([GetPathFormat('/Scripts/sources/SelfTest/Failed.js?crc=')]);
</script>
<div id="pageMain" data-role="page">
    <div data-role="header">        
        <h1>@ViewData("Title")</h1>
    </div>
    <div data-role="content" >
        <h3>Result: Failed</h3>
        <hr />
        <br />
        <div id ="divFailTestNoData" style="display:none;">
            Oops, it seems that your data is incomplete. As a result, the system is unable to process your data. Please make sure that you have 
            completed setting up your company information such as FOWST (Factory, Office, Warehouse, Store, Truck), products, and prices.
            <br /><br />
            We require those things to be completed first before we can test your data. 
            <br /><br />
            To set up your company information and upload your products, please sign in to <a href="http://mobiz-apps.com">mobiz-apps.com</a>, then proceed to System Administration. 
            If you need help in setting up your account, please email customercare@mobiz-apps.com or call (082) 286 - 9579. 
            <br /><br />
             Once you have completed the set up, please click 'Synchronize Data' to get the latest updates from the server. Thank you.
            <br /><br />

            <div style="text-align: center">
                <a id="btnSync" data-role="button">
                    <h3>Synchronize Data</h3>
                </a>
            </div>
        </div>
       <div id ="divFailTest" style="display:none;">
            The system has found some issues with your data. As a result, we have sent a report to Mobiz to look into the issue and fix it as soon as possible. 
            <br /><br />
            You are not advised to continue unless the application installed in your device has passed the test. <br /><br />
            To contact Mobiz, please email customercare@mobiz-apps.com or call (082) 286 - 9579. Thank you.
            
        </div>

       <div data-role="popup" id="popupMenu" >
            <div data-role="popup" id="dvpopup" data-theme="a" class="ui-corner-all" data-dismissible="false" data-overlay-theme="a">
                <p id="popMessage" style="padding:10px 20px;text-align:center"></p>
            </div>
        </div>
       
    </div>
</div>
