<div data-role="content" style="padding:0px 20px 0px 20px">
    <div class="ui-grid-a">
        <div class="ui-block-a" style="width: 20%">
              <button data-icon="back" id="btnBackCheck">Back</button>
        </div>
        <div class="ui-block-b" style="text-align: right;width: 80%">
             <h2>Check/ Gift Certificate Payment</h2> 
        </div>
    </div>

    <div data-role="fieldcontain" style="display: none;">
        <input type="text" readonly id="txtSalesPayCheckID" />
    </div>

    <div class="ui-body ui-body-b">
        <div data-role="fieldcontain">
            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <label>
                        Sub Total:</label>
                </div>
                <div class="ui-block-b">
                    <label id="lblSalesPayCheckSubTotal" style="float: right; text-align: right;">
                        0.00</label>
                </div>
            </div>
        </div>
        <div data-role="fieldcontain">
            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <label>
                        Less : Discount Amount</label>
                </div>
                <div class="ui-block-b">
                    <label id="lblSalesPayCheckTotalDiscount" style="float: right; text-align: right;">
                        0.00</label>
                </div>
            </div>
        </div>
        <div data-role="fieldcontain">
            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <label>
                        Add : Charges</label>
                </div>
                <div class="ui-block-b">
                    <label id="lblSalesPayCheckCharges" style="float: right; text-align: right;">
                        0.00</label>
                </div>
            </div>
        </div>
        <div data-role="fieldcontain">
            @If Common.WebRequest.QueryString("IsMobile") <> "1" Then
                @<div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>Amount Due:</label>
                    </div>
                    <div class="ui-block-b">
                    <br />
                        <label id="lblSalesPayCheckAmountDue" style="float: right; text-align: right; font-size: xx-large;">0.00</label>
                    </div>
                </div>
            Else
                @<div>
                    <label>
                        Amount Due:</label></div>
                @<div>
                    <label id="lblSalesPayCheckAmountDue" style="float: right; text-align: right; font-size: xx-large;">
                        0.00</label></div>
            End If
        </div>
        <div data-role="fieldcontain">
            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <label>
                        Amount Tendered:</label>
                </div>
                <div class="ui-block-b">
                    <label id="lblSalesPayCheckTotalAmountTendered" style="float: right; text-align: right;">
                        0.00</label>
                </div>
            </div>
        </div>
        <div data-role="fieldcontain">
            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <label>
                        Amount Lacking:</label>
                </div>
                <div class="ui-block-b">
                    <label id="lblSalesPayCheckAmountLacking" style="float: right; text-align: right;">
                        0.00</label>
                </div>
            </div>
        </div>
        <div data-role="fieldcontain">
            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <label>
                        Change Amount:</label>
                </div>
                <div class="ui-block-b">
                    <label id="lblSalesPayCheckTotalChangeAmount" style="float: right; text-align: right;">
                        0.00</label>
                </div>
            </div>
        </div>
    </div>
<div style="display: none;"><input type="text" readonly id="txtSalesPayCheckBankID" /></div>
    <br />
    <div class="divTable" style="width:auto; font-size:inherit;">
        <div class="divTableRow">
            <div class="divTableCell txt-middle">Options:</div>
            <div class="divTableCell">
                <div class="custom-radio">
                    <label class="radio-only">
                        <input type="radio" data-iconpos="notext" name="radOpPayCheck" id="radOpCheck" value="check" />
                    </label>
                    <div>Bank Check</div>
                </div>
            </div>
            <div class="divTableCell">
                <div class="custom-radio">
                    <label class="radio-only">
                        <input type="radio" data-iconpos="notext" name="radOpPayCheck" id="radOpGC" value="gc" />
                    </label>
                    <div>Gift Check/ Certificate</div>
                </div>
            </div>
        </div>
    </div>

    <div id="divForCheck" style="display:none;">
        <div class="custom-ui-label">
            <div>Bank/ Issuing Organization: <span style="color: #FF0000;">*</span></div>
            <div>
                <div id="divCheckBankButton">
                    <button id="btnSalesPayCheckBank" data-role="button">Please Select</button>
                </div>

                <div id="divCheckBankTextbox" style="display:none;">
                    <input type="text" id="txtSalesPayCheckBankName" />
                </div>
            </div>
        </div>

        <div style="display: none;"><input type="text" readonly id="txtSalesPayCheckBankBranchID" /></div>
        <div data-role="fieldcontain">
            <label for="txtSalesPayCheckBankBranch">Bank/ Issuing Organization Branch: <span style="color: #FF0000;">*</span></label>
            <input type="text" id="txtSalesPayCheckBankBranch" style="display:none;" />

            <div id="divCheckBankBranchButton">
                <button id="btnSalesPayCheckBankBranch" data-role="button">Please Select</button>

            </div>



        </div>

        @*<div data-role="fieldcontain">
            <label for="txtSalesPayCheckAccountName">Account Name (Issued To): <span style="color: #FF0000;">*</span></label>
            <input type="text" id="txtSalesPayCheckAccountName" />

        </div>*@

        <div data-role="fieldcontain">
            <label for="txtSalesPayCheckAccountNo">Account No.: <span style="color: #FF0000;">*</span></label>
            <input type="text" id="txtSalesPayCheckAccountNo" />

        </div>
    </div>        

        <div data-role="fieldcontain">
            <label for="txtSalesPayCheckNo"><span id="lblPayCheckNo"></span></label>
            <input type="text" id="txtSalesPayCheckNo" />
 
        </div>
        
        <div class="custom-ui-label">
            <div><span id="lblPayCheckDate"></span></div>
            <div>
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <select id="selSalesPayCheckDateMonth">
                        <option value="">MMM</option>
                    </select>
                    <select id="selSalesPayCheckDateDay">
                        <option value="">DD</option>
                    </select>
                    <select id="selSalesPayCheckDateYear">
                        <option value="">YYYY</option>
                    </select>
                </fieldset>
            </div>
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayCheckAmount"><span id="lblPayCheckAmount"></span></label>
            <input type="number" id="txtSalesPayCheckAmount" />

        </div>
    <div>
        <br />
        <br />
        <div style="text-align: center">
            <a id="btnSalesPayCheckSave" data-role="button">Ok</a>
        </div>
        <div style="text-align: center">
            <a id="btnSalesPayCheckRemove" data-role="button">Remove</a>
        </div>
        <div style="text-align: center">
            <a id="btnSalesPayCheckCancel" data-role="button">Cancel</a>
        </div>
    </div>
                 
    </div>