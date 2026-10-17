@Code
    ViewData("Title") = "Download Shelf Price Tags"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head 
    <style>
        .ui-icon-barcode {
	        background-image: url("../Content/images/barcode.ico");
	        background-repeat: no-repeat;	        
	            background-position: center center;
	            background-size: 50px 50px;
        }
    </style>
    <script>
        document.addEventListener('deviceready', this.onDeviceReady, false);
        require([GetPathFormat('/Scripts/sources/ShelfAndBinLabeling/DownloadShelfPriceTags.js?crc=')]);
    </script>

    <link rel="Stylesheet" type="text/css" href="/Content/flatui-css-custom.css" />
     
End Section
 
<div id="pageMain" data-role="page">
    <div data-role="header">        
        <h1>@ViewData("Title")</h1>        
    </div>

    <div data-role="content">
        
        <div data-role="fieldcontain">
            <label for="selFOWST">FOWST:</label>
            <select id="selFOWST"></select>
        </div>

        <div data-role="fieldcontain">
            <label for="selBINLocation">Shelf BIN Location:</label>
            <select id="selBINLocation" disabled>
                
            </select>
        </div>

        <br />
        <br />
        <div style="text-align: center">
            <a id="btnDownload" data-role="button"><h3>Download price tags</h3></a>
            <a id="btnCancel" data-role="button"><h3>Cancel</h3></a>
        </div>
        
    </div>  
</div>