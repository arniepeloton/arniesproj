@Code
    ViewData("Title") = "JS Test"
    If CDec2(Request.QueryString("BodyOnly")) = 0 Then
        Layout = "~/Views/Shared/Layout3Single.vbhtml"
    End If
End Code
<script>
    require([GetPathFormat('/Scripts3/Test/Js.js?crc=')]);
</script>
<div data-role="collapsible">
    <h2>
        JS</h2>
    <div id="divTestJs">
    </div>
</div>
 