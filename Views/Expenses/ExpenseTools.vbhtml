@Code
    ViewData("Title") = "Expense Tools"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head    
    <link rel="stylesheet" href="/Content/bootstrap/css/bootstrap.css" />
    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <link rel="stylesheet" href="/Content/bootstrap-custom.css" />
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
    <link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />
    <script>
        require([GetPathFormat('/Scripts/sources/Expenses/ExpenseTools.js?crc=' + assetversion)]);
    </script>
End Section
<div class="container">
    <div class="row">
        <div class="col-sm-4">
            Admin Password: <input type="password" id="txtAdminPassword" />
        </div>
    </div>
    <br />
    <div class="row">
        <div class="col-sm-4">
            Employee: <select id="selExpenseEmployee"></select>
        </div>        
    </div>
    <br />
    <div class="row" style="border:1px solid #CCC;">
        <div class="col-sm-12">
            <button id="btnRecompute">Recompute Expenses From Beginning</button>
            - Net Fund Balance of all Employees as of July 2020 is not accurate from October 2019 (first released of Expense Tracker)
            <br />
            - The purpose is to recompute from beginning to this CURRENT date
        </div>
    </div>
    <br />
    <div class="row" style="border:1px solid #CCC;">
        <div class="col-sm-4">
            FOWST: <select id="selExpenseFowst"></select>
        </div>
        <div class="col-sm-8">
            <button id="btnAssignFowst">Assign selected FOWST to selected Employee to his/her Receive Fund Trans/Cache table and Net Fund Balance Table</button>
            - No FOWST ID column in mentioned tables as of July 2020.
            <br />
            - The purpose is to assign FOWST ID in mentioned tables so that all the queries of Expense Module will work on Aug 6 2020.
        </div>
    </div>
    <br />
    <div class="row">
        <div class="col-sm-1">Result:</div>
        <div class="col-sm-11">
            <pre id="divResult"></pre>
        </div>
    </div>
</div>