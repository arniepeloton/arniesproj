@Code
    ViewData("Title") = "Employee List"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code

@Section head
    <script type="text/javascript" src="/Scripts/sources/Employee/Employee.js"></script>
End Section

<div>
    <div>
        <input id="txtSearch" type="text" name="txtSearch" value="" /></div>
    <div>
        <input id="butSearch" type="button" onclick="EmployeeSearch()" value="Search" /></div>
</div>

<div>
    <ul data-role="listview">
        <li><a href="#">Emp1</a></li>
        <li><a href="#">Emp2</a></li>
        <li><a href="#">Emp3</a></li>
    </ul>
</div>