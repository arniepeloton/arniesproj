@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/DevicePrinter/DevicePrinterDevice.js?crc='),
            GetPathFormat('/Scripts3/Browser/LibraryDeviceFowst.js?crc=')
            ]);
    </script>
    
End Section

<div id="pageDevicePrinterDeviceList" data-role="page">

    <div id"divHeader" data-role="header">
        <h1>Printer Device List</h1>
    </div>

    <div data-role="content" >

        <div>
            <ul data-role="listview" id="lstDevicePrinterDevContainer" data-inset="true">   
				<li data-role="list-divider">Devices</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnAddDevicePrinterDevice" data-role="button" data-icon="plus" data-inline="true">Add a Device</a></div>
                    <div data-role="fieldcontain"><span style="font-size:smaller; font-weight:normal;">* Devices that can use this printer.</span></div>
                    <div data-role="fieldcontain" id="divRecordCountDevicePrinterDevice"></div>
                    <div>
                        <ul data-role="listview" id="lstDevicePrinterDevice" data-inset="true" data-split-icon="delete" data-split-theme="d">   
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnDevicePrinterDeviceDone" data-role="button">Done</a> </div>
        </div>

    </div>

</div>

<div id="pageDevicePrinterDeviceInfo" data-role="page">

    <div data-role="header">
        <h1>Device Info</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtDevicePrinterDeviceRecID"/>                
        </div>

        <div style="display:none;"><input type="text" readonly id="txtDevicePrinterDeviceID"/></div>
        <div data-role="fieldcontain">
	        <label for="btnDevicePrinterDevice">Device : <span style="color: #FF0000;" >*</span></label>
            <fieldset data-role="controlgroup">
                <button id="btnDevicePrinterDevice" data-role="button">Please Select</button>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
	        <label for="txtDevicePrinterPhoneModel">Phone Model : </label>
            <fieldset data-role="controlgroup">
	            <input type="text" readonly id="txtDevicePrinterPhoneModel"/>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
	        <label for="txtDevicePrinterDeviceIMEI">IMEI :</label>
            <fieldset data-role="controlgroup">
	            <input type="text" readonly id="txtDevicePrinterDeviceIMEI"/>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
	        <label for="txtDevicePrinterDeviceUUID">UUID :</label>
            <fieldset data-role="controlgroup">
	            <input type="text" readonly id="txtDevicePrinterDeviceUUID"/>
            </fieldset>
        </div>

        <div data-role="fieldcontain">  
            <label for="txtDevicePrinterDeviceOS">Platform : </label>
            <input type="text" readonly id="txtDevicePrinterDeviceOS"/>
        </div>

        <div data-role="fieldcontain">
            <label><input type="checkbox" disabled id="chkDevicePrinterDeviceIsActive" /> Active</label>
        </div>


        <div>  
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnDevicePrinterDeviceSave" data-role="button">Ok</a> </div>
	        <div style="text-align:center"> <a id="btnDevicePrinterDeviceCancel" data-role="button">Cancel</a> </div>
        </div>

    </div>

</div>