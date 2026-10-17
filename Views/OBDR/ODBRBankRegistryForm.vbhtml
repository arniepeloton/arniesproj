@Code
    ViewData("Title") = "Online Bank Deposit Registry " & ViewData("SubTitle")
    Layout = "~/Views/OBDRShared/Layout.vbhtml"
End Code

<script type="text/javascript" src="../../Scripts/sources/OBDR/plugins/plOBDR.js"></script>

<!-- Dialog Box -->
<div id="dialog-message">
    <div>
        <div>
            <div class="obdr-form-elements">
                <div>
                    <span>The Paper Tree Branch: </span>
                    <span style="padding-left:50px;"><select id="optBranches"><option>SELECT BRANCH</option></select></span>
                </div>
                 <div>
                    <span>The Paper Tree POS Number: </span>
                    <span style="padding-left:16px;"><select id="optMachineNumbers"><option>SELECT POS NUMBER</option></select></span>
                </div>
                 <div>
                    <span>Bank Account Number: </span>
                    <span style="padding-left:54px;"><input type="text" id="txtBankAccountNumber" disabled /></span>
                </div>
                 <div>
                    <span>Bank Account Name: </span>
                    <span style="padding-left:65px;"><input type="text" id="txtBankAccountName" disabled /></span>
                </div>
            </div>
        </div>
        <br />
        <hr />
        <div>
            <div class="obdr-form-elements">
                <div>
                    <span>POS Sales Date: </span>
                    <span style="padding-left:88px;"><input type="text" id="txtPOSSalesDate" value="" /></span>
                </div>
                <div>
                    <span>Date Deposited into Bank: </span>
                    <span style="padding-left:37px;"><input type="text" id="txtBankDepositedDate" value="" /></span>
                </div>
                <div>
                    <span>Transaction Type: </span>
                    <span style="padding-left:85px;"><select><option value="POSSales">POS Sales</option><option value="ECPay">ECPay</option></select></span>
                </div>
                <div>
                    <span>Roving Teller: </span>
                    <span style="padding-left:106px;"><input type="text" id="txtRovingTeller" size="30"/></span>
                </div>
            </div>
        </div>
        <br />
        <h3>TYPE OF DEPOSIT</h3>
        <hr />
        <div>
            <h3>Cash</h3>
            <br />
            <div>
                <span>Amount Deposited:</span>
                <span style="padding-left:80px;"><input type="text" id="txtCashDepositedAmount" style="text-align:right;font-size:24px;font-weight:bold;" /></span>
            </div>
        </div>
        <hr />
        <div>
            <h3>Check</h3>
            <br />
            <div>
                <table width="100%">
                    <thead>
                        <tr>
                            <th>Drawee Bank</th>
                            <th>Bank Branch</th>
                            <th>Check Date</th>
                            <th>Check Number</th>
                            <th>Check Amount</th>
                            <th>Payor</th>
                            <th>Action</th>
                        </tr>
                    </thead>
                    <tbody id="tPOSCheckDetails">
                        <tr>
                            <td colspan="7" align="center">No Record Found</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</div>




<div id="pos-deposit-registry-cancel" class="hide-dialog-box">
    <div>Cancel Reason</div>
    <div>
        <textarea id="pos-deposit-registry-reason">
        </textarea>
    </div>
</div>
<!-- ################################ -->
<div class="tab">
    <ul>
        <li><a href="#tabs-1" id="event-tabs-1">Current Bank Branch Deposit</a></li>
        <li><a href="#tabs-2" id="event-tabs-2">Historical Bank Branch Deposit</a></li>
        <li><a href="#tabs-3">Settings</a></li>
    </ul>
    <div id="tabs-1">   
        <div>
           <button id="btnRegisterDeposits">Register Branch Deposits</button>
        </div>
        <table width="100%" cellpadding="0" cellspacing="0" border="0" class="display">
            <thead>
                <tr>
                    <th> </th>
                    <th>Account No.</th>
                    <th>Account Name</th>
                    <th>The Paper Tree Branch</th>
                    <th>The Paper Tree POS #</th>
                    <th>Deposited Amount</th>
                    <th>TPT Sales Date</th>
                    <th>Deposit for</th>
                    <th>Roving Teller</th>
                </tr>
            </thead>
            <tbody id="tpt-pos-deposit-display">
                <tr>
                    <td colspan="8" align="center">No current deposit as of @Format(Now, "dd MMMM, yyyy")</td>
                </tr>
            </tbody>
        </table>
    </div>
    <div id="tabs-2">
        <div>
            <table>
            <tr>
                <td>Date from: </td>
                <td><input type="text" value="" id="dtDateFrom" /> </td>
            </tr>
            <tr>
                <td>Date to: </td>
                <td><input type="text" value="" id="dtDateTo" /></td>
            </tr>
            <tr>
                <td>The Paper Tree POS Branch: </td>
                <td><select id="pos-deposit-history-branches"><option>Default</option></select></td>
            </tr>
            <tr>
                <td></td>
                <td><button id="pos-deposit-registry-search">Search</button></td>
            </tr>
        </table>
        </div>
        <table width="100%" id="tblCurrentBankBranchDeposits" cellpadding="0" cellspacing="0" border="0" class="display">
            <thead>
                <tr>
                    <th> </th>
                    <th>Account No.</th>
                    <th>Account Name</th>
                    <th>The Paper Tree Branch</th>
                    <th>The Paper Tree POS #</th>
                    <th>Deposited Amount</th>
                    <th>TPT Sales Date</th>
                    <th>Deposit for</th>
                    <th>Roving Teller</th>
                </tr>
            </thead>
            <tbody id="pos-deposit-history-display">
                <tr>
                    <td colspan="8" align="center">No current deposit as of @Format(Now, "dd MMMM, yyyy")</td>
                </tr>
            </tbody>
        </table>
    </div>
    <div id="tabs-3">
        <!-- e-Deposit Slip Entry Design
        <div id="deposit-entry-main-design-left">
            <div id="deposit-entry-main-design-currency">
                <h2>CURRENCY</h2>
                <div id="deposit-entry-main-design-currency-style">
                    <span><input type="checkbox" id="deposit-entry-main-design-currency-PHP" value="PHP" checked /> PHP</span>
                    <span><input type="checkbox" id="deposit-entry-main-design-currency-OTHERS" value="OTHERS" /> OTHERS</span>
                </div>
                <div id="deposit-entry-main-design-currency-style1">
                    <span><input type="checkbox" id="deposit-entry-main-design-currency-USD" value="USD" /> USD</span>
                    <span><input type="text" id="deposit-entry-main-design-currency-other-text" disabled value="" /></span>
                </div>
            </div>

            <div id="deposit-entry-main-design-typeofaccount">
                <h2>TYPE OF ACCOUNT</h2>
                <div id="deposit-entry-main-design-typeofaccount-style">
                    <span><input type="checkbox" id="deposit-entry-main-design-typeofaccount-PHP" value="SAVINGS" checked /> SAVINGS</span>
                    <span><input type="checkbox" id="deposit-entry-main-design-typeofaccount-OTHERS" value="TD/SSA" /> TD/SSA</span>
                </div>
                <div id="deposit-entry-main-design-typeofaccount-style1">
                    <span><input type="checkbox" id="deposit-entry-main-design-typeofaccount-CHECKING" value="CHECKING" /> CHECKING</span>
                    <span><input type="checkbox" id="deposit-entry-main-design-typeofaccount-OTHERS" value="OTHERS" /> OTHERS
                    <input type="text" id="deposit-entry-main-design-typeofaccount-other-text" disabled value="" /></span>
                </div>
            </div>
            <div class="clear"></div>
        </div>
        <div id="deposit-entry-main-design-denomination">
            <p>
                PLEASE LIST ON THE DETAILS COLUMN, IF CASH DEPOSIT, ITS DENOMINATION & NUMBER OF PIECES. AND IF CHECK DEPOSIT. THE CHECK
                DETAILS FOR FOREIGN CURRENCY CASH DEPOSIT, PLEASE ALSO ATTACH A LIST OF INDICATING THE SERIAL NUMBER OF THE NOTES FOR DEPOSIT
            </p>
            <table width="100%" cellpadding="0" cellspacing="0">
                <thead>
                    <tr>
                        <th colspan="3">DETAILS</th>
                    </tr>
                    <tr>
                        <th>IF CASH -   DENOMINATION</th>
                        <th>NO. OF PIECES</th>
                        <th rowspan="2">AMOUNT</th>
                    </tr>
                    <tr>
                        <th>IF CHECK -   DRAWEE BANK-BRANCH</th>
                        <th>CHECK NUMBER</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td colspan="3" align="center"><a href="#">Add Denominatio / Check Number</a></td>
                    </tr>
                    <tr id="display-denomination">
                        <td align="right">0</td>
                        <td align="right">0</td>
                        <td align="right">0.00</td>
                    </tr>
                    <tr id="footer-denomination">
                        <td colspan="2" align="right">TOTAL DEPOSIT</td>
                        <td>&nbsp;</td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="clear"></div>-->
    </div>
</div>