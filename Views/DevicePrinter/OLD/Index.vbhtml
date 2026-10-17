@Code
    ViewData("Title") = "Printers"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/DevicePrinter/DevicePrinter.js?crc=')]);
    </script>
    
End Section

<div id="pageDevicePrinterList" data-role="page">

    <div data-role="header">
        <h1>Printer List</h1>
    </div>

    <div data-role="content">

        <form id="frmDevicePrinterBrowser" style="margin: 0 -7px">
            <div>
                <input id="txtDevicePrinterSearch" type="search" placeholder="Search a Printer" value="" />
            </div>
        </form>

        <div id="divDevicePrinterResults" style="display:none;">

            <div class="ListViewMarginBottom" id="divDevicePrinterRecordCount"></div>

            <ul id="lstDevicePrinter" data-role="listview" data-filter="true">
            </ul>

        </div>

        <div id="divDevicePrinterSearchNoResults" style="display:none;">
	        <div>
                <div data-role="fieldcontain">
                    <label id="lblDevicePrinterNotFound"></label>
                </div>
            </div>
        </div>

        <div>
            <br />
	        <div style="text-align:center;"> <a id="btnAddNewDevicePrinter" data-icon="plus" data-role="button"><h3>Add New Printer Role</h3></a> </div>
            <br />
	        <div style="text-align:center"> <a id="btnDevicePrinterBackToHome" data-role="button"><h3>Go Back to Main</h3></a> </div>
        </div>

    </div>

</div>







