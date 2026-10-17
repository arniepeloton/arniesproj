@Code
    ViewData("Title") = "Add Shelf And Bin Label"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    @*<script type="text/javascript" src="/Scripts/sources/crm/jspdf.js"></script>
    <script type="text/javascript" src="/Scripts/sources/crm/html2canvas.js"></script>
    <script type="text/javascript" src="/Scripts/sources/crm/jspdf.min.js"></script>
    <script type="text/javascript" src="/Scripts/sources/crm/qrcode.js"></script>*@
    @*<script type="text/javascript" src="/Scripts/sources/crm/test.js"></script>*@

    <script>
        require([GetPathFormat('/Scripts/sources/ShelfAndBinLabeling/AddShelfAndBinLabel.js?crc=')]);
    </script>
 
    End Section

    <div id="dvMain" data-role="page">

		<div data-role="popup" id="popupLabelSize" data-theme="b" class="ui-corner-all">
			<a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
			<center><h3>&nbsp;&nbsp; Label Paper Size &nbsp;&nbsp;</h3></center>
			<div data-role="content" style="text-align: center; ">
				<select id="labalType">
	                <option value="">Please Select</option>
	                <option value="smallerSize">23.30mm x 88.90mm</option>
	                <option value="largerSize">38.1mm x 76.2mm</option>
	            </select>
	            <a id="paperSelection" data-role="button"><h3>Generate</h3></a>
			</div>
		</div>
        <div data-role="content">
            @*<div>Sample Bin Format</div> <br />*@

            <div class="ui-bar ui-bar-c" style="height:190px; text-align:center">
                <br />
                <label style="font-size:40px;">Bin Sample Format </label> <br /> <br />
                <label id="lblSampleFormat" style="font-size:50px;"></label> <br /> <br />
                <label id="lblSampleFormatLabel" style="font-size:25px;"></label>
            </div>

            <br />

            <div data-role="fieldcontain">
                <label for="selFOWST">FOWST</label>
                <select id="selFOWST"></select>
            </div>
            <table>
                <tr>
                    <td>
                        <label>
                        </label>
                    </td>
                    @*<td>
                        <label>
                            <input type="checkbox" name="checkbox-0 ">FOWST
                        </label>
                    </td>*@
                </tr>
                <tr>
                    <td>
                        <label>
                            <input type="checkbox" id="chkArea">Area (1 to N)
                        </label>
                    </td>
                    <td>
                        <label>
                            <input readonly type="number" id="txtArea" />
                        </label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>
                            <input type="checkbox" id="chkRack">Rack
                        </label>
                        <label>
                            <input type="checkbox" id="chkAisle">Aisle
                        </label>
                    </td>
                    <td>
                        <label>
                            <input readonly id="txtRack" />
                        </label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>
                            <input type="checkbox" id="chkSection">Section
                        </label>
                    </td>
                    <td>
                        <label>
                            <input readonly type="number" id="txtSection" />
                        </label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>
                            <input type="checkbox" id="chkShelf">Shelf
                        </label>
                    </td>
                    <td>
                        <label>
                            <input readonly type="number" id="txtShelf" />
                        </label>
                    </td>
                </tr>
                @*<tr>
                    <td>
                        <label>
                            <input type="checkbox" id="chkAisle">Aisle
                        </label>
                    </td>
                    <td>
                        <label>
                            <input readonly type="number" id="txtAisle" />
                        </label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>
                            <input type="checkbox" id="chkSide">Side
                        </label>
                    </td>
                    <td>
                        <label>
                            <input readonly type="number" id="txtSide" />
                        </label>
                    </td>
                </tr>*@
                <tr>
                    <td>
                        <label>
                            @*Enter number of*@
                        </label>
                    </td>

                    @*<td>
                        <label>
                            <input type="number" />
                        </label>
                    </td>*@

                </tr>
            </table>
        </div>

        <a id="GenerateQRCodeToPDF" data-role="button"><h3>Generate QR Code</h3></a>

        <a id="btnCancel" data-role="button"><h3>Cancel</h3></a>

        <div id="QrCodeCanvass" style="visibility:hidden">
        </div>

    </div>





