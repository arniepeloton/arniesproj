@Code
    ViewData("Title") = "POS Settings"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/SystemBehaviorSettings/SalesOrder.js?crc=')]);
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
                        Sales Order Behavior
                    </div>
                </li>
                <li>
                    <div data-role="fieldcontain">           
                        <label for="selEnableInventoryCountMode">Enable Inventory Count Mode</label>                
                        <select data-role="slider" id="selEnableInventoryCountMode">
                            <option value="0">No</option>
                            <option value="1">Yes</option>                
                        </select>
                    </div>
                    <div data-role="fieldcontain">           
                        <label for="selRequireSignature">Require Signature</label>                
                        <select data-role="slider" id="selRequireSignature">
                            <option value="0">No</option>
                            <option value="1">Yes</option>                
                        </select>
                    </div>
                </li>        
			</ul>
        </div>

        <br />
        <button id="btnSaveSalesOrderSetting">Save</button>

        <a id="btnDone" data-role="button">Done</a>            
    </div>

</div>