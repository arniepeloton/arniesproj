@Code
    ViewData("Title") = "Long Process"
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
    require([GetPathFormat('/Scripts3/AndroidJsTestLoingProcess.js?crc=')]);
</script>

<h2>@ViewData("Title")</h2>
<button id="butLongProcess1">AjaxSend Long Process (blocking)</button>
<button id="butLongProcess2">AjaxSend Long Process (must follow up)</button>
<button id="butFollowUp">Follow up</button>
<button id="butClear">Clear</button>
<div id="divLog"></div>