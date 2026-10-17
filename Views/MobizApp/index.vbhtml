@Code
    ViewData("Title") = "Mobiz Apps Update"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/MobizApp/MobizApp.js?crc=')
            ]);
    </script>
    
End Section

<div id="pageMobizAppUpdate" data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">

        <div style="text-align:center;">
            <h3>Please download a copy of the latest evaluation of Mobiz POS at this address:</h3>
            <a id="lnkMobizAppUpdate" style="cursor:pointer; font-size:16px;">www.philhr.com/mobizapp/downloadfile</a>

            <h3>or simply scan the QR Code below using a QR Code Scanner on your Android Phone to download the Mobiz POS App</h3>
            <h4>(We recommend Zebra Crossing's "Barcode Scanner" app on Google's Play Store)</h4>
            
            <div>
                <img style="width:210px; height:50px;" src="/Content/MobizAppsUpdate/ZebraCrossing.jpg" />
            </div>
            
            <a id="btnMobizAppUpdateQR" style="cursor:pointer;">
                <img src="/Content/MobizAppsUpdate/MobizUpdateQR.png" />
            </a>
        </div>

    </div>

</div>

