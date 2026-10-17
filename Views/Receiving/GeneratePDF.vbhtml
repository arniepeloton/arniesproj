@Code
    ViewData("Title") = "Receiving And Transfers"
    Layout = "~/Views/Shared/Layout4.vbhtml"
End Code

@Section head

    <script type="text/javascript" src="/Scripts/sources/crm/jspdf.js"></script>
    <script type="text/javascript" src="/Scripts/sources/crm/html2canvas.js"></script>
    <script type="text/javascript" src="/Scripts/sources/crm/jspdf.min.js"></script>
    <script type="text/javascript" src="/Scripts/sources/crm/qrcode.js"></script>
    <script type="text/javascript" src="/Scripts/sources/Receiving/GeneratePDF.js"></script>

End Section

<div id="dvMain" data-role="page">
    <div id="QrCodeCanvass" style="visibility:hidden">
    </div>
</div>
