@Code
    ViewData("Title") = "Android JS Test"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
@Section head
<style>
    .DivInput 
    {
        background-color: #EEEEFF;
        border: solid 1px;
        text-shadow: none;
        padding-left: 10px;
    }
    .DivLog 
    {
        margin-bottom: 60px;
    }    
</style>
End Section
<script>
    require([GetPathFormat('/Scripts3/AndroidJsTest.js?crc=')]);
</script>

<h2>@ViewData("Title")</h2>
<a href="/signin" data-role="button">Sign In</a>
<div id="butTestUpload" data-role="button">
    Test upload<div style="font-size:10pt; font-weight:normal">        </div>
</div>
<div id="butStartOverall" data-role="button">
    Start overall test<div style="font-size:10pt; font-weight:normal">
        This will overwrite your database files</div>
</div>
<div id="divLog" style="overflow-x: auto"></div>