@Code
    ViewData("Title") = "Printers"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/DevicePrinter/PrinterSetup.js?crc=')]);
    </script>
    
End Section

<div data-role="page">

    <div data-role="header">
        <h1>Device Printer Setup</h1>
    </div>

    <div data-role="content">
        Welcome to the Device Printer Setup Assistant. To guide you in setting up a printer for your device, please follow the instructions
       that will be displayed in the dialog box. Please click on Start then choose a printer role.

        <br />

       <div style="text-align: center">
            <a id="btnStartSetup" data-role="button">
                <h3>Start</h3>
            </a>
        </div>
 
</div>







