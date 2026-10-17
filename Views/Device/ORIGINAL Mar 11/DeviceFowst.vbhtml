@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/Device/DeviceFowst.js?crc='),
            GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc=')
            ]);
    </script>
    
End Section

<div id="pageDeviceFowst" data-role="page">

    <div id"divHeader" data-role="header">
        <h1>FOWST</h1>
    </div>

    <div data-role="content" >

        <div>
            <ul data-role="listview" id="lstPlacesContainer" data-inset="true">   
				<li data-role="list-divider">Places (FOWST)</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnAddDeviceFOWST" data-role="button" data-icon="plus" data-inline="true">Add a Place</a></div>
                    <div data-role="fieldcontain"><span style="font-size:smaller; font-weight:normal;">* Places where this device can be used</span></div>
                    <div data-role="fieldcontain" id="divRecordCountDeviceFowst"></div>
                    <div>
                        <ul data-role="listview" id="lstDeviceFowst" data-inset="true" data-split-icon="delete" data-split-theme="d">   
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnDeviceFowstDone" data-role="button"><h3>Done</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageDeviceFowstInfo" data-role="page">

    <div data-role="header">
        <h1>Device-Fowst Info</h1>
        <a href="#" data-icon="arrow-l" id="btnDeviceFowstBack">Back</a>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtDeviceFowstID"/>                
        </div>

        <div style="display:none;"><input type="text" readonly id="txtFowstID"/></div>
        <div data-role="fieldcontain">
	        <label for="btnFOWST">Device FOWST Assignment: <span style="color: #FF0000;" >*</span></label>
            <button id="btnFOWST" data-role="button">Please Select</button>

        </div>
        <div data-role="fieldcontain" id="divPrinterFowstName">
             <label for="selPrinterFowstName" class="select">Printer Name: <span style="color: #FF0000;" >*</span></label>
            <select id="selPrinterFowstName">
            </select>
        </div>
        <div data-role="fieldcontain">
	        <label for="txtPOSRegNo">POS Registration No.: <span style="color: #FF0000;" >*</span></label>
	        <input type="text" id="txtPOSRegNo"/>
        </div>

        <div data-role="fieldcontain">  
            <label for="txtDevFowstBIRMachineNo">Machine Identification No. Issued by BIR : <span style="color: #FF0000;" >*</span></label>
            <input type="text" id="txtDevFowstBIRMachineNo"/>
        </div>

        <div data-role="fieldcontain">  
            <label for="txtTerminalNo">Terminal No: </label>
            <input type="text" readonly id="txtTerminalNo"/>
        </div>
@*
        <div data-role="fieldcontain">  
            <label for="txtDevFowstPrinterIP">Device Printer IP: </label>
            <input type="text" id="txtDevFowstPrinterIP"/>
        </div>*@

        <div id="divSMMachineNo" style="display:none;">
            <div data-role="fieldcontain">
	            <label for="txtDevFOWSTSMMachineNo">Machine No. (Assigned by SM Management):</label>
                <input type="text" id="txtDevFOWSTSMMachineNo"/>
            </div>                
        </div>                

        <div id="divDeviceFOWSTBegBalance">
            <ul data-role="listview" id="lstDevFOWSTBegBalContainer" data-inset="true">   
			    <li data-role="list-divider">Beginning Balance</li>
                <li>
                    <div data-role="fieldcontain">
	                    <label for="selDevFOWSTBegBalDateMonth">Sales Beginning Balance Date :</label>
                        <fieldset data-role="controlgroup" data-type="horizontal">
                            <select id="selDevFOWSTBegBalDateMonth">
	                            <option value="">MMM</option>
                            </select>
                            <select id="selDevFOWSTBegBalDateDay">
	                            <option value="">DD</option>
                            </select>                
                            <select id="selDevFOWSTBegBalDateYear">
	                            <option value="">YYYY</option>
                            </select>                
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
	                    <label for="txtDevFOWSTBegBalance">Sales Beginning Balance :</label>
	                    <input type="number" id="txtDevFOWSTBegBalance"/>
                    </div>                

                    <div data-role="fieldcontain">
	                    <label for="txtDevFOWSTInvoiceStartNo">Last Used Invoice No. :</label>
	                    <input type="number" id="txtDevFOWSTInvoiceStartNo"/>
                        
                    </div>                

                    <div data-role="fieldcontain">
	                    <label for="txtDevFOWSTSRVStartNo">Last Used SRV No. :</label>
                        <input type="number" id="txtDevFOWSTSRVStartNo"/>
      
                    </div>                

                    <div data-role="fieldcontain">
	                    <label for="txtDevFOWSTRefStartNo">Last Used Refund No. :</label>
                        <input type="number" id="txtDevFOWSTRefStartNo"/>
                        
                    </div>                

                </li>
            </ul>
        </div>       

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnDevFowstSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnDevFowstCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>


    </div>

</div>
