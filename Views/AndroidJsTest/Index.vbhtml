@Code
    ViewData("Title") = "Android JS Test"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
@Section head
<link rel="stylesheet" href="/Scripts3/others/jasmine-2.0.0/jasmine.css" />
End Section
<script>
    require([GetPathFormat('/Scripts3/AndroidJsTest.js?crc=')]);
</script>

<h2>@ViewData("Title")</h2>
<div id="divJasmine"></div>