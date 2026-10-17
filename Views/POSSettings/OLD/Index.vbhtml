@Code
    ViewData("Title") = "POS Settings"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/POSSettings/POSSettings.js?crc=')]);
    </script>
    
End Section

<div id="pageDeviceMenu" data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">

        <div>
            <ul data-role="listview" data-inset="true">

				<li data-role="list-divider">
                    <div data-role="fieldcontain">
                        POS Behavior
                    </div>
                </li>
                <li>
                    <div data-role="fieldcontain">           
                        <label for="selEnableDiningMode">Enable Dining Mode</label>                
                        <select data-role="slider" id="selEnableDiningMode">
                            <option value="0">No</option>
                            <option value="1">Yes</option>                
                        </select>
                    </div>

                    <div data-role="fieldcontain">           
                        <label for="selEnableBillingOption">Enable Billing Option</label>                
                        <select data-role="slider" id="selEnableBillingOption">
                            <option value="0">No</option>
                            <option value="1">Yes</option>                
                        </select>
                    </div>

                    <div data-role="fieldcontain">           
                        <label for="selWhenPrintOrderSlip">When do you want to print Order Slip</label>                
                        <select id="selWhenPrintOrderSlip">
                            <option value="Not Applicable">Not Applicable</option>
                            <option value="After Payment">After Payment</option>                
                            <option value="Before Payment">Before Payment</option>
                        </select>
                    </div>

                    <div data-role="fieldcontain">           
                        <label for="selPrintOrderSlip">Print Order Slip</label>                
                        <select data-role="slider" id="selPrintOrderSlip">
                            <option value="0">No</option>
                            <option value="1">Yes</option>                
                        </select>
                    </div>

                     <div data-role="fieldcontain">           
                        <label for="selEnableBillingOption">Enable VAT Exempt Option</label>                
                        <select data-role="slider" id="selEnableVATExemptOption">
                            <option value="0">No</option>
                            <option value="1">Yes</option>                
                        </select>
                    </div>
                </li>
        
				<li data-role="list-divider">
                    <div data-role="fieldcontain">
                        Customer Input Fields
                    </div>
                </li>
                <li>
                    <div data-role="fieldcontain">           
                        <label for="selCustomerTableNo">Show Customer or Table Number:</label>                
                        <select data-role="slider" id="selCustomerTableNo">
                            <option value="optAllowNo">No</option>
                            <option value="optAllowYes">Yes</option>                
                        </select>
                    </div>
 
                    <div id="divCustomerTableNumCount" data-role="fieldcontain" style="display:none;">           
                        <label for="txtTableNoLimit">No. of Tables:</label>                
                        <input id="txtTableNoLimit" type="text" maxlength="5"/>
                    </div>

                    <div data-role="fieldcontain">           
                        <label for="selGasPumpNo">Show Gas Pump Number:</label>                
                        <select data-role="slider" id="selGasPumpNo">
                            <option value="optAllowNo">No</option>
                            <option value="optAllowYes">Yes</option>                
                        </select>
                    </div>
                    <div id="divGasPumpCount" data-role="fieldcontain" style="display:none;">           
                        <label for="txtGasPumpNoLimit">No. of Gas Pumps:</label>                
                        <input id="txtGasPumpNoLimit" type="text" maxlength="5"/>
                    </div>
                </li>

			</ul>
        </div>

        <br />
        <button id="btnCustomerInputFieldsSave">Save</button>

        <a id="btnCustomerInputFieldsCancel" data-role="button">Done</a>            
    </div>

</div>