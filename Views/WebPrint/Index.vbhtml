@Code
    ViewData("Title") = "Print"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
<link rel="stylesheet" href="/Content/flatui-css-custom.css" />
<script>
    require([GetPathFormat('/Scripts/sources/WebPrint/WebPrint.js?crc=')]);
</script>
End Section


<button data-role="none" id="btnPrintNow" class="btn custom-btn-theme-b btn-less-width btn-sm">Print Now</button>
<br /><br />
<div id="divPrintHtml">
    
</div>