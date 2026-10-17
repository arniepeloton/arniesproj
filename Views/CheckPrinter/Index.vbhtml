@Code
    ViewData("Title") = "Test Printer Connection"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
<script>
    require([GetPathFormat('/Scripts/sources/CheckPrinter/CheckPrinter.js?crc=')]);
</script>

<div id="pageMain" data-role="page">
    <div data-role="header">        
        <h1>@ViewData("Title")</h1>
    </div>
    <div data-role="content">

        <div id="divWait">
        Please wait for a few moments while the system is testing local printer connectivity...
        </div>
        <div>
            <ul data-role="listview" id="lstDevicePrinterConnected" data-inset="true">   
				<li data-role="list-divider">Connected Printers</li>
                <li>

                    <div>
                        <ul data-role="listview" id="lvDevicePrinterConnected" data-inset="true" data-split-icon="delete" data-split-theme="d">   
                        </ul>
                    </div>
                </li>
            </ul>
            <ul data-role="listview" id="lstDevicePrinterDisconnected" data-inset="true">   
				<li data-role="list-divider">Disconnected Printers</li>
                <li>

                    <div>
                        <ul data-role="listview" id="lvDevicePrinterDisconnected" data-inset="true" data-split-icon="delete" data-split-theme="d">   
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div id="divNoPrinter" style="display:none;">
            There are no printer/s assigned to this device. As a result, you might not be able to use the printing functions of this application.  
            If you think that a printer is critical to your operation, 
            please click on 'Continue' then go to System Administration -> System Settings -> Device Printer Setup and follow the instructions.
            On the other hand, please click 'Continue' if you would like to proceed with the app.
        </div>
        <div id="divConnDisplay" style="display:none;">
            Your printer connection looks good! Please click 'Continue' to start using Mobiz.
        </div>
        <div id="divDisconDisplay" style="display:none;">
            Some printer/s assigned to this device are unreachable.
            As a result, you might not be able to use the printing functions of this application. 
            If you think that the disconnected printer/s shown above are critical to your operation, 
            please click 'Diagnose' to find out the cause of the problem and determine the solution.
            On the other hand, please click 'Continue' if you would like to proceed with the app.
            <br />
            <div style="text-align: center">
                <a id="btnDiagnose" data-role="button">
                    <h3>Diagnose</h3>
                </a>
            </div>
            <div style="text-align: center">
                <a id="btnRetry" data-role="button">
                    <h3>Retry</h3>
                </a>
            </div>

        </div>
        <div style="text-align: center">
            <a id="btnContinue" data-role="button" style="display:none;">
                <h3>Continue</h3>
            </a>
        </div>
    
    </div>
</div>

<div data-role="page" id="pageDiagnose">
    <div data-role="header">
        <h1>Diagnose Printer Problems</h1>
    </div>

    <div data-role="content">        
        <h3>Your printer might not be working due to some problems. </h3>
        <br />
        The most common reason is that it is not connected to your local network, 
        therefore your device could not reach it. To fix this problem, please make sure 
        that the router and modem are turned on and your printer is properly connected. 

        <hr />
        <h3>
        Please follow these steps to diagnose the problem:
        </h3>

        
        <ol>
            <li>Make sure the printer is plugged in and switched on. Green light indicates that the printer is on .</li>
            <li>Check if Local Area Network (LAN) cable is plugged in to the printer and the router or switch.  Check that the lights on both ends are on. </li>
            <li>Many printers have an online/offline indicator. Make sure the display indicates that the printer is online.</li>
            <li>Check if there is still enough paper.</li>
            <li>If the paper ejects but nothing is printed, check the orientation of the paper. It may be printing on the wrong side of the thermal paper.  Also check if the thermal paper is not yet past its expiration date.</li>
            <li>If you’re still having trouble, turn the printer off and on again. </li>
            <li>If you’re still experiencing a problem, call tech support.</li>

        </ol>

        <br />
            <div style="text-align: center">
                <a id="btnBack" data-role="button" data-rel="back">
                    <h3>Back</h3>
                </a>
            </div>
    </div>
</div>