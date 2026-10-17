@Code
    ViewData("Title") = "Hack Test"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
<script src="Scripts3/HackTest/index.js"></script>
<h1>
    Welcome</h1>
<h2>
    Search Bank</h2>

<button id="butMessage">Message with buttons</button>
<div data-role="fieldcontain">
<form id="formSearch">
    <label for="txtSearch">
        Search Bank:</label>
    <input type="search" id="txtSearch" />
    
    Mobile Numbers:
    <input name="txtMobileNum" />
    <input name="txtMobileNum" />
    <input name="txtMobileNum" />
    <input name="txtMobileNum" />
    <input name="txtMobileNum" />
    <input name="txtMobileNum" />

    <button type="submit">Go</button>
    </form>
</div>

<ul id="ulResults" data-role="listview" data-filter="true">
</ul>


<div data-role="controlgroup">
    <h2>
        SQL Injection</h2>
    <a href="HackTest/SqlUnprotected" data-role="button">Unprotected</a>
    <button>Protected</button>
</div>
<div data-role="controlgroup">
    <h2>
        Cross Site Scripting</h2>
    <a href="HackTest/CrossSiteScriptingUnprotected" data-role="button">Unprotected</a>
    <button>Protected</button>
</div>
<div data-role="controlgroup">
    <h2>
        Cross Site Forgery</h2>
    <button>Unprotected</button> <button>Protected</button>
</div>
<div data-role="controlgroup">
    <h2>AjaxSend-ing an Array of Dictionary</h2>
    <button id="butAjaxDictionaryArray">Go</button>
</div>

<div data-role="controlgroup">
    <h2>Local File Test (for PhoneGap)</h2>
    <a href="/Scripts/jQuery.js">"/Scripts/jQuery.js"</a>
    <a href="Scripts/jQuery.js">"Scripts/jQuery.js"</a>
</div>

<a href="sms:444">Tap to text us!</a><br /><br /><br /><br />
<a href="sms:444?body=hello">Tap to say hello!</a>