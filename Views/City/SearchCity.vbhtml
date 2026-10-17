@Code 
    ViewData("Title") = "Search City"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code

@Section head
    <script type="text/javascript" src="../../Scripts/sources/City/SearchCity.js"></script>
End Section

<div data-role="page">

    <div id="divRecordCount"></div>

    <div>
        <div><input id="txtSearch" type="text" value="" /></div>
        <div><input id="btnSearch" type="button" value="Search" onclick="SearchCity()"/></div>
    </div>

    <div>
        <br />
        <ul data-role="listview" id="lstResultList">
        </ul>
    </div>

</div>
