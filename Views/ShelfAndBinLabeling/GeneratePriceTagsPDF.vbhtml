@Code
    ViewData("Title") = "Generate Barcode Labels"
    Layout = "~/Views/Shared/Layout4.vbhtml"
End Code

@Section head

    <script type="text/javascript" src="/Scripts/sources/crm/jspdf.js"></script>
    <script type="text/javascript" src="/Scripts/sources/crm/html2canvas.js"></script>
    <script type="text/javascript" src="/Scripts/sources/crm/jspdf.min.js"></script>
    <script type="text/javascript" src="/Scripts/sources/crm/CODE128.js"></script>
    <script type="text/javascript" src="/Scripts/sources/crm/JsBarcode.js"></script>
    <script type="text/javascript" src="/Scripts/sources/crm/qrcode.js"></script>
    <script type="text/javascript" src="/Scripts/sources/ShelfAndBinLabeling/GeneratePriceTagsPDF.js"></script>
    <script type="text/javascript" src="/Scripts/sources/Others/jquery.format.js"></script>
    

End Section

<div id="dvMain" data-role="page">
    <div style="width:100%; text-align:center; margin-top:100px;">
        Generating PDF File <span id="txtFrom">0</span> of <span id="txtTo">0</span>, please wait ...
        <br /><progress id="progBar" value="0" max="0"></progress>
    </div>

    <div id="QrCodeCanvass" style="visibility:hidden">
    </div>
</div>