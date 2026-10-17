@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/Device/DeviceInfoSummary.js?crc=')]);
    </script>
    
End Section

<div id="pageSumDevice" data-role="page">

    <div id"divHeader" data-role="header">
        <h1>Device Information Summary</h1>
        <a href="#" data-icon="arrow-l" id="btnBack" style="display:none;">Back</a>
    </div>

    <div data-role="content" >

        <div>
            <ul data-role="listview" data-inset="true">

				<li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Device Name
                        <a href='#' id='btnEditDevice' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div data-role="fieldcontain" style="display:none;">
	                    <input type="text" readonly id="txtSumDeviceID"/>                
                    </div>

                    <div data-role="fieldcontain">
	                    <label for="txtSumDevicePhoneName">Phone Name:</label>
                        <fieldset data-role="controlgroup">
	                        <input type="text" readonly id="txtSumDevicePhoneName"/>
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
	                    <label for="txtSumDevicePhoneModel">Phone Model:</label>
                        <fieldset data-role="controlgroup">
	                        <input type="text" readonly id="txtSumDevicePhoneModel"/>
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
	                    <label for="txtSumDeviceIMEI">IMEI:</label>
                        <fieldset data-role="controlgroup">
	                        <input type="text" readonly id="txtSumDeviceIMEI"/>
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
	                    <label for="txtSumDeviceUUID">UUID:</label>
                        <fieldset data-role="controlgroup">
	                        <input type="text" readonly id="txtSumDeviceUUID"/>
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
	                    <label for="txtSumDeviceOS">OS:</label>
                        <fieldset data-role="controlgroup">
	                        <input type="text" readonly id="txtSumDeviceOS"/>
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
	                    <label for="txtSumDeviceNo">Device No.:</label>
                        <fieldset data-role="controlgroup">
	                        <input type="text" readonly id="txtSumDeviceNo"/>
                        </fieldset>
                    </div>

                </li>

                <li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Places (FOWST)
                        <a href='#' id='btnEditDeviceFowst' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div>
                        <ul data-role="listview" id="lstSumDeviceFowst" data-inset="true">
                        </ul>
                    </div>
                </li>

@*                <li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Printer
                        <a href='#' id='btnEditDevicePrinter' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div>
                        <ul data-role="listview" id="lstSumDevicePrinter" data-inset="true">
                        </ul>
                    </div>
                </li>*@
                
			</ul>
        </div>


        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnDeviceSummaryBack" data-role="button"><h3>Done</h3></a>
            </div>
        </div>

    </div>

</div>

