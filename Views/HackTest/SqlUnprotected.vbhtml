@Code
    ViewData("Title") = "Hack Test - Sql Unprotected"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
<script>
    require(['/Scripts3/SalesMenu.js']);
</script>
<h1>
    Sql Unprotected</h1>
<div data-role="fieldcontain">
    <label>
        Search:
        <input id="txtSearch" value="" /></label>
</div>
<button id="butSearch">Go</button>

@Section OtherPages

<div id="divtemp" data-role="page">
    <script>
        console.log('divtemp');
    </script>
    <div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>asdasdasdasd</h1>
    </div>
    <div data-role="content">
        
    </div>
</div>
End Section
