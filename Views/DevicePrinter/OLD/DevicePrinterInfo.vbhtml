@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/DevicePrinter/DevicePrinterInfo.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc=')
        ]);
    </script>
    
End Section

<div id="pageDevicePrinterInfo" data-role="page">

    <div data-role="header">
        <h1>Printer Information</h1>
        <a href="#" data-icon="arrow-l" id="btnBackPrinterRole">Back</a>
    </div>

    <div data-role="content" >

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtDevicePrinterID"/>                
        </div>

        <div data-role="fieldcontain">
             <label for="selPrinterRole" class="select">Printer Role: <span style="color: #FF0000;" >*</span></label>
            <select id="selPrinterRole">
	            <option value="">Please Select</option>
                <option value="Bar">Bar</option>
                <option value="Kitchen">Kitchen</option>
                <option value="Stock Room">Stock Room</option>
                <option value="Warehouse">Warehouse</option>
                <option value="Others">Others</option>
            </select>
        </div>

        <div data-role="fieldcontain">
	        <label for="selPrinterType" class="select">Printer Type: <span style="color: #FF0000;" >*</span></label>
            <select id="selPrinterType">
	            <option value="">Please Select</option>
				<option value="Epson TM-T88V Printer">Epson TM-T88V Printer</option>
				<option value="80MM Mini Thermal Printer">80MM Mini Thermal Printer</option>
            </select>
        </div>

         <div id="divCategories" style="display:none;">
            <ul data-role="listview" id="lstCategoriesContainer" data-inset="true">   
				<li data-role="list-divider">Categories to print on this printer</li>
                <li>
        @*            <label><input type="checkbox" id="chkSelectAll" /> Select All</label>*@
                    <table id="tblCategories" style="width:100%">                    
                    </table>  
                </li>
            </ul>
        </div>

        <div data-role="fieldcontain" id ="divActive" style="display:none;">
            <label><input type="checkbox" id="chkIsActive" /> Active</label>
        </div>
        <br />
        <button id="btnDevicePrinterInfoDone">Save</button>

        <a id="btnDevicePrinterInfoCancel" data-role="button">Done</a> 

        
    </div>

</div>

