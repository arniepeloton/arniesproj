@Code
    ViewData("Title") = "File Not Found"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
<script>
    require(['/Scripts3/FileNotFound.js?crc=']);
</script>
<div style="text-align:center">
    <h3>The following page was not found:</h3>
    <a id="lnkUrl" href="#">Please wait. Loading URL of the missing page.</a>
</div>
