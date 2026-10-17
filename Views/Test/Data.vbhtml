@Code
    ViewData("Title") = "Data Test"
    If CDec2(Request.QueryString("BodyOnly")) = 0 Then
        Layout = "~/Views/Shared/Layout3Single.vbhtml"
    End If
End Code
<script>
    require([GetPathFormat('/Scripts3/Test/Data.js?crc=')]);
</script>
<div data-role="collapsible">
    <h2>
        Data</h2>
    <div id="divTestData">
    </div>
</div>
 