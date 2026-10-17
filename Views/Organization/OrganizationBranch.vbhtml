@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require(['/Scripts/sources/Organization/OrganizationBranch.js']);
    </script>
    
End Section

<div id="pageBranchList" data-role="page">
    <div data-role="header">
        <a href="#popupMenu" data-rel="popup" data-inline="true" class="ui-btn-right">Go To</a>
        <h1>Organization Branch</h1>
    </div>


    <div data-role="content">
        <div data-role="fieldcontain">
            <ul data-role="listview" id="lstBranchContainer" data-inset="true">     
                <li data-role="list-divider">Branch List</li>
                <li>
                    <div data-role = "fieldcontain"><a href="#" id="btnAddBranch" data-role="button" data-icon="plus" data-inline="true">Add SWOT (Store, Warehouse, Office or Truck)</a></div>
                    <div>
                        <ul data-role="listview" id="lstBranch" data-inset="true">     
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnBranchNext" data-role="button">Next</a> </div>
	        <div style="text-align:center"> <a id="btnBranchBack" data-role="button">Back</a> </div>
	        <div style="text-align:center"> <a id="btnBranchDone" data-role="button">Done</a> </div>
        </div>

        <div id="popupMenu" data-role="popup">
            <ul data-role="listview" data-inset="true" style="min-width: 210px;">
                <li data-role="divider">Menu</li>
                <li><a href="OrganizationInfo">Name</a></li>
                <li><a href="OrganizationContact">Contact Information</a></li>
                <li><a href="OrganizationAffiliation">Affiliations</a></li>
                <li><a href="OrganizationOwner">Directors, Officers and Stockholders, Partners or Owners</a></li>
                <li><a href="OrganizationFinancialReport">Financial Report Information</a></li>
                <li><a href="OrganizationEmployee">Employee List</a></li>
                <li><a href="OrganizationCustomer">Customer List</a></li>
                <li><a href="OrganizationSupplier">Supplier List</a></li>
            </ul>
        </div>

    </div>

</div>

