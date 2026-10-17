@Code
    ViewData("Title") = "Fix Net Due"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
<script>
    require([GetPathFormat('/Scripts3/FixNetDue.js?crc=')]);
</script>
<form id="formMain">
<div data-role="fieldcontain">
    <label for="txtInvoiceNums">
        Invoice Numbers:</label>
    <input id="txtInvoiceNums" maxlength="500" />
</div>
<div data-role="fieldcontain">
    <label for="chkUpdate">
        Update</label>
    <input id="chkUpdate" type="checkbox" />
</div>
<div id="divResult"></div>
<button type="submit">Ok</button> 
</form>
