@Code
    ViewData("Title") = "Compile Pages"
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
    require([GetPathFormat('/Scripts3/CompilePages.js?crc=')]);
</script>

<h2>@ViewData("Title")</h2>
<div id="divMessage"></div>
<button id="butClearLog">Clear Message Log</button>
<div id="divLog"></div>