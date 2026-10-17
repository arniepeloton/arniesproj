@Code 
    ViewData("Title") = "Customer Search"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <script src="Scripts3/CustomerSearch.js"></script>
End Section
<div id="pageKeywords" data-role="page">
    <div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>
            @ViewData("Title")</h1>
    </div>
    <div data-role="content">
        <form id="formSearch">
        <div>
            <input id="txtSearch" type="search" name="search" value="" /></div>
        <div>
            <input id="butSearch" type="submit" value="Next" /></div>
        </form>
    </div>
</div>
<div id="pageResults" data-role="page">
    <div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>
            Search result</h1>
    </div>
    <div data-role="content">
        <div class="ListViewMarginBottom" name="divRecordCount"></div>
        <ul id="ulResults" data-role="listview" data-filter="true">
        </ul>
        <div class="ListViewMarginTop" name="divRecordCount"></div>
        <div data-role="popup" id="popNoRecord">
            <p></p>
            <button id="butNoRecordOk">Ok</button>
            <button id="butNoRecordCancel">Cancel</button>
        </div>
    </div>
</div>
