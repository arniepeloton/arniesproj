@Code
    ViewData("Title") = "City List"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code

<div>
    <div>
        <input id="txtSearch" type="text" name="txtSearch" value="" /></div>
    <div>
        <input id="butSearch" type="button"" value="Go" /></div>
</div>

<div>
<br />
    <ul data-role="listview">
        <li><a href="#">City1</a></li>
        <li><a href="#">City2</a></li>
        <li><a href="#">City3</a></li>
    </ul>
