@Code
    ViewData("Title") = "Employee"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code

@Section head
    <script type="text/javascript" src="/Scripts/jquery.js"></script>
    <script type="text/javascript" src="/Scripts3/others/jquery.mobile.js"></script>    
    <script type="text/javascript" src="/Scripts/sources/Employee/Employee.js"></script>

    <link rel="stylesheet"  href="/Scripts3/others/themes/default/jquery.mobile.css" />    
End Section

<div>
    <ul data-role="listview">
        <li><a href="EmployeeInfo">Create New Employee</a></li>
        <li><a href="#">Edit Existing Employee</a></li>
    </ul>
</div>