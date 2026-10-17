@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/Device/DevicePrinter.js?crc='),
            GetPathFormat('/Scripts3/Browser/LibraryDevicePrinter.js?crc=')
            ]);
    </script>
    
End Section

<div id="pageDevicePrinter" data-role="page">

    <div id"divHeader" data-role="header">
        <h1>Device Printer</h1>
    </div>

    <div data-role="content" >

        <div>
            <ul data-role="listview" id="lstPlacesContainer" data-inset="true">   
				<li data-role="list-divider">Printer</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnAddDevicePrinter" data-role="button" data-icon="plus" data-inline="true">Add a Printer</a></div>
                    <div data-role="fieldcontain" id="divRecordCountDevicePrinter"></div>
                    <div>
                        <ul data-role="listview" id="lstDevicePrinter" data-inset="true" data-split-icon="delete" data-split-theme="d">   
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnDevicePrinterDone" data-role="button"><h3>Done</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageDevicePrinterInfo" data-role="page">

    <div data-role="header">
        <h1>Device Printer Info</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtDevicePrinterDeviceRecID"/>                
        </div>

        <div style="display:none;"><input type="text" readonly id="txtDevicePrinterID"/></div>
        <div data-role="fieldcontain">
	        <label for="btnDevicePrinter">Printer : <span style="color: #FF0000;" >*</span></label>
            <fieldset data-role="controlgroup">
                <button id="btnDevicePrinter" data-role="button">Please Select</button>
            </fieldset>
        </div>

        <div data-role="fieldcontain">  
            <label for="txtDevicePrinterType">Printer Type : </label>
            <input type="text" readonly id="txtDevicePrinterType"/>
        </div>

        <div data-role="fieldcontain">  
            <label for="txtDevicePrinterFOWST">Printer FOWST Assignment : </label>
            <input type="text" readonly id="txtDevicePrinterFOWST"/>
        </div>

        <div data-role="fieldcontain">  
            <label for="txtDevicePrinterIP">Fixed IP Address : </label>
            <input type="text" readonly id="txtDevicePrinterIP"/>
        </div>

        <div data-role="fieldcontain">
            <label><input type="checkbox" disabled id="chkPrinterIsActive" /> Active</label>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnDevicePrinterSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center;">
                <a id="btnDevicePrinterRemove" data-role="button" style="display:none;"><h3>Remove</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnDevicePrinterCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>

