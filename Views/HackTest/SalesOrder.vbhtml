@Code
    ViewData("Title") = "Sales Order Test"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
<script>
    require(['/Scripts3/SalesMenu.js']);
</script>
<div id="divContentParent">
    <div id="divContent">
        <h1>
            Sales Order Test</h1>
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" data-type="horizontal">
                <legend>Choose:</legend><button id="butCustomer">browse...</button>
            </fieldset>
        </div>
        <form id="formSearch">
        <div data-role="fieldcontain">
            <label for="txtSearch">
                Search:</label>
            <input id="txtSearch" type="search" />
        </div>
        </form>
    </div>
</div>
<h2>
    Remove list item</h2>
<ul id="lst" data-role="listview" data-filter="true">
    <li>nice</li>
    <li>nice</li>
    <li>nice</li>
    <li>nice</li>
</ul>
<button id="butRemoveItem">Remove Item</button>
<h2 style="margin-top: 100px">
    Dialog Within Dialog Test</h2>
<a href="#pageDialog" data-role="button">Show Page</a>
<a href="#pageDialog" data-role="button" data-rel="dialog">Show Dialog (not working)</a>
<div data-role="fieldcontain">
    <fieldset data-role="controlgroup" data-type="horizontal">
        <legend>:</legend><span></span>
    </fieldset>
</div>
<div data-role="controlgroup">
    <h2>
        Local File Test (for PhoneGap)</h2>
    <a href="/Scripts/jQuery.js">"/Scripts/jQuery.js"</a>
    <a href="Scripts/jQuery.js">"Scripts/jQuery.js"</a>
</div>
@Section OtherPages
    <div id="pageDialog" data-role="page">
        <div data-role="header">
            <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
            <h1>
                Dialog Box</h1>
        </div>
        <div data-role="content">
        </div>
    </div>
End Section
