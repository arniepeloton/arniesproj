@Code
    ViewData("Title") = "test"
    Layout = "~/Views/Shared/Layout4.vbhtml"
End Code

@Section head    

<script type="text/javascript" src="/Scripts/sources/crm/jspdf.js"></script>
<script type="text/javascript" src="/Scripts/sources/crm/html2canvas.js"></script>
<script type="text/javascript" src="/Scripts/sources/crm/jspdf.min.js"></script>
<script type="text/javascript" src="/Scripts/sources/crm/qrcode.js"></script>
<script type="text/javascript" src="/Scripts/sources/crm/test.js"></script>

End Section

<div id="dvMain" data-role="page">

    <div data-role="content">
        @*<div>Sample Bin Format</div> <br />*@

        <div class="ui-bar ui-bar-c" style="height:190px; text-align:center">
            <br />
            <label style="font-size:40px;">Bin Sample Format </label> <br /> <br />
            <label id="lblSampleFormat" style="font-size:50px;"></label> <br /> <br />
            <label id="lblSampleFormatLabel" style="font-size:25px;"></label>       
        </div>

        <br />

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
                <td>
                    <label>
                        <input type="checkbox"  id="chkArea">Area (1 to N)
                    </label>
                </td>
                <td>
                    <label>
                        <input type="checkbox" id="chkRack">Rack
                    </label>
                </td>
                <td>
                    <label>
                        <input type="checkbox" id="chkSection">Section
                    </label>
                </td>
                <td>
                    <label>
                        <input type="checkbox" id="chkShelf">Shelf
                    </label>
                </td>
                <td>
                    <label>
                        <input type="checkbox" id="chkAisle">Aisle
                    </label>
                </td>
                <td>
                    <label>
                        <input type="checkbox" id="chkSide">Side
                    </label>
                </td>
            </tr>
            <tr>
                <td>
                    <label>
                        @*Enter number of*@
                    </label>
                </td>
                <td>
                    <label>
                        <input readonly type="number" id="txtArea" />
                    </label>
                </td>
                @*<td>
                        <label>
                            <input type="number" />
                        </label>
                    </td>*@
                <td>
                    <label>
                        <input readonly type="number" id="txtRack" />
                    </label>
                </td>
                <td>
                    <label>
                        <input readonly type="number" id="txtSection" />
                    </label>
                </td>
                <td>
                    <label>
                        <input readonly type="number" id="txtShelf" />
                    </label>
                </td>
                <td>
                    <label>
                        <input readonly type="number" id="txtAisle" />
                    </label>
                </td>
                <td>
                    <label>
                        <input readonly type="number" id="txtSide" />
                    </label>
            </tr>
        </table>
    </div>

    <a id="GenerateQRCodeToPDF" data-role="button">Generate QR Code</a>

    <div id="QrCodeCanvass" style="visibility:hidden">
    </div>

</div>





