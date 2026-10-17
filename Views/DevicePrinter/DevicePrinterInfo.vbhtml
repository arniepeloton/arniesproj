<script>
    require([GetPathFormat('/Scripts/sources/DevicePrinter/DevicePrinterInfo.js?v=' + assetversion),
                 GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc=')
        ]);
</script>

<div id="pageDevicePrinterInfo" style="display:block;">

    @*<div data-role="header">
        <h1>Printer Information</h1>
        <a href="#" data-icon="arrow-l" id="btnBackPrinterRole">Back</a>
    </div>*@

    <h1 class="display-6 mb-4">Printer Information</h1>

    <div data-role="content" >

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtDevicePrinterID"/>                
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="selPrinterRole">Printer Role: <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <select id="selPrinterRole" data-role="none" class="form-select">
                    <option value="">Please Select</option>
                    <option value="Bar">Bar</option>
                    <option value="Kitchen">Kitchen</option>
                    <option value="Stock Room">Stock Room</option>
                    <option value="Warehouse">Warehouse</option>
                    <option value="Others">Others</option>
                </select>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="selPrinterType">Printer Type: <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <select id="selPrinterType" data-role="none" class="form-select">
                    <option value="">Please Select</option>
                    <option value="Epson TM-T88V Printer">Epson TM-T88V Printer</option>
                    <option value="Epson TM-U220B Printer">Epson TM-U220B Printer</option>
                    <option value="80MM Mini Thermal Printer">80MM Mini Thermal Printer</option>
                    <option value="Windows Print Server">Windows Print Server</option>
                </select>
            </div>
        </div>

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="chkIsActive">Is Active?</label>
            <div class="col-sm-10">
                <div class="form-check form-switch mb-3" style="font-size:20px;">
                    <input class="form-check-input" data-role="none" type="checkbox" id="chkIsActive">
                </div>
            </div>
        </div>

        <h5 class="mb-3">Categories to print on this printer</h5>

        <div class="mb-3" style="max-height:300px; overflow:auto">
            <table id="tblCategories" style="width:100%"></table>
        </div>        

        <div class="mt-4 text-center">
            <a id="btnDevicePrinterInfoCancel" data-role="none" class="btn btn-secondary btn-lg">Cancel</a>
            <a id="btnDevicePrinterInfoDone" data-role="none" class="btn btn-success btn-lg">Save</a>
        </div>
    </div>
</div>