@Code
    ViewData("Title") = "Online Bank Deposit Registry: Admin"
    Layout = "~/Views/Main/Layout.vbhtml"
End Code

<link href="../../Content/styles.css" rel="stylesheet" type="text/css" />
<script src="../../Scripts/sources/OBDR/plugins/plAccountManagements.js"></script>
<script src="../../Scripts/sources/OBDR/BankRegistry.js"></script>
<script src="../../Scripts/sources/OBDR/plugins/plOBDR.js"></script>
<div class="tab">
    <ul>
        <li><a href="#tabs-1">Sales and Deposits History</a></li>
        <li><a href="#tabs-2">Parameters</a></li>
        <li><a href="#tabs-3" id="tab-4-event">Assign POS Machine Number</a></li>
        <li><a href="#tabs-4" id="tab-3-event">User Account Management for Bank Personnel</a></li>
    </ul>
    <div id="tabs-1">
        <div>
            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <th>Branch Name:</th>
                    <th>Account No.: </th>
                    <th>Account Name: </th>
                    <th>POS No.: </th>
                    <th>Date From: </th>
                    <th>Date To: </th>
                </tr>
                <tr>
                    <td align="center"><select><option>Branch Name</option></select></td>
                    <td align="center"><span>0-000-000000-0</span></td>
                    <td align="center"><span>No Name</span></td>
                    <td align="center"><span><select style="margin-botoom:5px;"><option>POS 1</option></select></span></td>
                    <td align="center"><span><input type="text" id="dtFrom" style="margin-top:3px;" /></span></td>
                    <td align="center"><span><input type="text" id="dtTo" /></span></td>
                </tr>
            </table>
        </div>
        <div><button>Search</button></div>
        <div>
            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                <thead>
                    <tr>
                        <th>#</th>
                        <th>Deposit Date</th>
                        <th>Encoded By</th>
                        <th>Bank</th>
                        <th>Deposit Amount</th>
                        <th>Date of Sales</th>
                        <th>Absolute Variance</th>
                        <th>% Variance</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <th colspan="8">No Records</th>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>

    <div id="tabs-2">
        <div>
            <button id="btnAddTransactionType">Add Transaction Type</button>
        </div>
        <div>
            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                <thead>
                    <tr>
                        <th>#</th>
                        <th>Transaction Type</th>
                        <th>Last Updated By</th>
                        <th>Last Updated</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td colspan="3" align="center">No Records Found.</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>

    <div id="tabs-3">
        <div id="pos-machine-number-entry-form">
            <div>
                <span class="pos-machine-number-entry-form-label">POS Branches: </span>
                <span>
                    <select id="pos-machine-number-entry-form-branches-select">
                        <option>&nbsp;</option>
                    </select>
                </span>
            </div>
            <div>
                <span class="pos-machine-number-entry-form-label">POS Machine Number: </span>
                <span>
                    <select id="pos-machine-number-entry-form-pos-machine-select">
                        <option>&nbsp;</option>
                    </select>
                </span>
            </div>
            <div>
                <span class="pos-machine-number-entry-form-label">POS Description: </span>
                <span>
                    <input type="text" id="pos-machine-number-description" value="" size="30" /> <span> e.g. Cashier 1, POS Cashier 1, Cashier A, or POS Cashier A</span>
                </span>
            </div>
            <div>
                <button id="pos-machine-number-description-save">Save</button>
            </div>
            <br />
        </div>
        <div>
            <table width="100%">
                <thead>
                    <tr>
                        <th>POS Machine Number</th>
                        <th>POS Description</th>
                        <th>TPT Branch</th>
                    </tr>
                </thead>
                <tbody id="pos-machine-number-descriptions">
                    <tr>
                        <td colspan="4">&nbsp;</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>

    <div id="tabs-4">
        <div>
            <button id="btnAddNewUser">Add New User</button>
        </div>
            <table width="100%" id="tblUserAccountManagement" cellpadding="0" cellspacing="0" border="0" class="display">
                <thead>
                    <tr>
                        <th> </th>
                        <th>User Account Name</th>
                        <th>Email Address</th>
                        <th>Bank</th>
                        <th>Position</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td colspan="5" align="center" id="emptyRecords">No User Accounts</td>
                    </tr>
                </tbody>
            </table>
    </div>

<!-- Dialog Box -->
<div id="user-account-form" class="hide-dialog-box">
    <table cellpadding="10" cellspacing="0">
        <tr>
            <td>Last Name: </td>
            <td><input type="text" value="" size="50" id="txtLastname" /></td>
        </tr>
        <tr>
            <td>First Name: </td>
            <td><input type="text" value="" size="50" id="txtFirstname" /></td>
        </tr>
        <tr>
            <td>Middle Name: </td>
            <td><input type="text" value="" size="50" id="txtMiddlename" /></td>
        </tr>
        <tr>
            <td>Email Address: </td>
            <td><input type="text" value="" size="50" id="txtEmailAddress" /></td>
        </tr>
        <tr>
            <td>Username: </td>
            <td><input type="text" value="" size="50" id="txtUsername" /></td>
        </tr>
        <tr>
            <td>Password: </td>
            <td><input type="password" value="" size="50" id="txtPassword" /></td>
        </tr>
        <tr>
            <td>Confirm Password: </td>
            <td><input type="password" value="" size="50" id="txtConfirmPassword" /></td>
        </tr>
        <tr>
            <td>Bank: </td>
            <td><select id="library-banks"><option value="">Default</option></select></td>
        </tr> 
        <tr>
            <td>Position: </td>
            <td><select id="library-bank-positions"><option value="">Roving Teller</option><option>Manager</option></select></td>
        </tr>
    </table>
</div>

<div id="transaction-type" class="hide-dialog-box">
    <table cellpadding="10" cellspacing="0">
        <tr>
            <td>Transaction Type: </td>
            <td><input type="text" value="" size="50" id="txtTransactionType" /></td>
        </tr>
    </table>
</div>