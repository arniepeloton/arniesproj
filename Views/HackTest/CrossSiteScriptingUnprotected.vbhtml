@Code
    ViewData("Title") = "Hack Test - Cross Site Scripting Unprotected"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
<script src="/Scripts3/HackTest/CrossSiteScriptingUnprotected.js"></script>
<h1>
    Cross Site Scripting Unprotected</h1>
<div data-role="fieldcontain">
    <label>
        Comment:
    <input id="txtComment" value="" /></label>
</div>
<button id="butPost">Post</button>