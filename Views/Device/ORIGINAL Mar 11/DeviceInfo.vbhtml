@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
    <style>
        .ui-icon-barcode {
	        background-image: url("../Content/images/barcode.ico");
	        background-repeat: no-repeat;	        
	         background-position: center center;
	         background-size: 50px 50px;
        }
    </style>
    

    <script>
        document.addEventListener('deviceready', this.onDeviceReady, false);
        require([GetPathFormat('/Scripts/sources/Device/DeviceInfo.js?crc=')]);        
    </script>
    
End Section

<div id="pageOrganization" data-role="page">

    <div id"divHeader" data-role="header">
        <h1>Data Fields for Device Enrolment</h1>
    </div>

    <div data-role="content" >

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtID"/>                
        </div>
        
        @*<a id="btnRetrieveDeviceInfo" data-role="button">Auto Fill Using This Device's Info</a>*@

        <a id="btnRetrieveDeviceInfoFromQR" data-role="button">Register Device using QR Code</a>

        <div data-role="fieldcontain">
            <label for="txtPhoneName">Device Name:</label>
            <input type="text" id="txtPhoneName"/>
        </div>
        
        <div data-role="fieldcontain">  
                <label for="txtPhoneModel">Device Model: </label>
                <input type="text" id="txtPhoneModel"/>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
            <legend>IMEI:</legend>
                <input type="text" id="txtIMEI"/>
                <a id="btnScanIMEI" data-role="button" data-inline="true" class="ui-icon-barcode" data-iconpos="notext"></a>
            </fieldset>
            </div>
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
	            <legend>UUID:</legend>
	            <input type="text" id="txtUUID"/>
                <a id="btnScanUUID" data-role="button" data-inline="true" class="ui-icon-barcode" data-iconpos="notext"></a>
            </fieldset>

            <div data-role="fieldcontain">  
                <label for="txtOS">Platform:</label>
	            <input type="text" id="txtOS"/>
            </div>
        </div>

        <div data-role="fieldcontain">  
                <label for="txtDeviceNo">Device No (System Generated): </label>
                <input type="text" readonly id="txtDeviceNo"/>
        </div>

         <div data-role="fieldcontain">
            <label for="selDeviceDeactivate">Active Device?:</label>
            <select id="selDeviceDeactivate" data-role="slider">
				<option value="0">No</option>	
                <option value="1">Yes</option>            
            </select>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnDeviceInfoDone" data-role="button"><h3>Done</h3></a>
            </div>
        </div>

    </div>

</div>
