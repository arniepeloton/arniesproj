<div data-role="content" style="padding:0px 20px 0px 20px">
    <div class="ui-grid-a">
        <div class="ui-block-a" style="width: 20%">
            <button data-icon="back" id="btnBackCreditCard">Back</button>
        </div>
        <div class="ui-block-b" style="text-align: right;width: 80%">
            <h2> Credit Card Payment</h2>    
        </div>
    </div>


    <div data-role="fieldcontain" style="display: none;">
            <input type="text" readonly id="txtSalesPayCreditCardID" />
        </div>

        <div class="ui-body ui-body-b">
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Sub Total:</label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPayCreditCardSubTotal" style="float: right; text-align: right;">
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
                        <label id="lblSalesPayCreditCardTotalDiscount" style="float: right; text-align: right;">
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
                        <label id="lblSalesPayCreditCardCharges" style="float: right;text-align: right;">
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
                            <label id="lblSalesPayCreditCardAmountDue" style="float: right; text-align: right; font-size: xx-large;">0.00</label>
                        </div>
                    </div>
                Else
                    @<div>
                        <label>
                            Amount Due:</label></div>
                    @<div>
                        <label id="lblSalesPayCreditCardAmountDue" style="float: right; text-align: right; font-size: xx-large;">
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
                        <label id="lblSalesPayCreditCardTotalAmountTendered" style="float: right; text-align: right;">
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
                        <label id="lblSalesPayCreditCardAmountLacking" style="float: right; text-align: right;">
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
                        <label id="lblSalesPayCreditCardTotalChangeAmount" style="float: right; text-align: right;">
                            0.00</label>
                    </div>
                </div>
            </div>
        </div>
        
        <div class="custom-ui-label">
            <div>Credit Card: <span style="color: #FF0000;">*</span></div>
            <div>
                <select id="selSalesPayCreditCardType">
                    <option value="">Please Select</option>
                    <option value="Visa">Visa</option>
                    <option value="Master Card">Master Card</option>
                    <option value="UnionPay">UnionPay</option>
                    <option value="JCB">JCB</option>
                    <option value="Diners">Diners</option>
                    <option value="American Express">American Express</option>
                    <option value="Other Card">Other Card</option>
                </select>
            </div>
        </div>

        <div id="divCreditCardTypeOther" data-role="fieldcontain" style="display:none;">
            <label for="txtSalesPayCreditCardTypeOther">Other Card Type : <span style="color: #FF0000;">*</span></label>
            <input type="text" id="txtSalesPayCreditCardTypeOther" />
  
        </div>

        <div style="display: none;"><input type="text" readonly id="txtSalesPayCreditCardBankID" /></div>
        <div class="custom-ui-label">
            <div>Issuing Bank: <span style="color: #FF0000;">*</span></div>
            <div>
                <div id="divSalesPayCreditCardBankButton">
                    <button id="btnSalesPayCreditCardBank" data-role="button">Please Select</button>
                </div>
                <div id="divSalesPayCreditCardBankTextbox" style="display:none;">
                    <input type="text" id="txtSalesPayCreditCardBank" />
                </div>
            </div>
        </div>

        <div style="display: none;"><input type="text" readonly id="txtSalesPayCreditCardBankLocationID" /></div>
        <div class="custom-ui-label">
            <div>Bank Location (Country):</div>
            <div><button id="btnSalesPayCreditCardBankLocation" data-role="button">Please Select</button></div>
        </div>

        <div>
            <ul data-role="listview" id="lstSalesPayCardHolder" data-inset="true">
                <li data-role="list-divider">Card Holder</li>
                <li>
                    <div data-role="fieldcontain">
                        <label for="txtSalesPayCreditCardLastName">Last Name: <span style="color: #FF0000;">*</span></label>
                        <input type="text" id="txtSalesPayCreditCardLastName" />
                    
                    </div>

                    <div data-role="fieldcontain">
                        <label for="txtSalesPayCreditCardFirstName">First Name: <span style="color: #FF0000;">*</span></label>
                        <input type="text" id="txtSalesPayCreditCardFirstName" />
      
                    </div>

                    <div data-role="fieldcontain">
                        <label for="txtSalesPayCreditCardMiddleName">Middle Name:</label>
                       <input type="text" id="txtSalesPayCreditCardMiddleName" />
                    
                    </div>

                    <div class="custom-ui-label">
                        <div style="font-weight:normal;">Generational Suffix (Name):</div>
                        <div>
                            <select id="selSalesPayPersonalSuffixName">
                                <option value="">Please Select</option>
                                <option value="Sr.">Senior (Sr.)</option>
                                <option value="Jr.">Junior (Jr.)</option>
                                <option value="II">II</option>
                                <option value="III">III</option>
                                <option value="IV">IV</option>
                                <option value="V">V</option>
                                <option value="VI">VI</option>
                                <option value="VII">VII</option>
                                <option value="VIII">VIII</option>
                                <option value="IX">IX</option>
                                <option value="X">X</option>
                            </select>
                        </div>
                    </div>
                </li>
            </ul>
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayCreditCardNo">Credit Card No.: <span style="color: #FF0000;">*</span> <p>Please enter the last four(4) digit of the card only.</p></label>
           
            <input type="text" id="txtSalesPayCreditCardNo" maxlength="4"/>
      
        </div>

        <div class="custom-ui-label">
            <div>Issuance Date:</div>
            <div>
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <select id="selSalesPayCreditCardIssuanceDateMonth">
                        <option value="">MMM</option>
                    </select>
                    <select id="selSalesPayCreditCardIssuanceDateDay">
                        <option value="">DD</option>
                    </select>
                    <select id="selSalesPayCreditCardIssuanceDateYear">
                        <option value="">YYYY</option>
                    </select>
                </fieldset>
            </div>
        </div>

        <div class="custom-ui-label">
            <div>Expiration Date:</div>
            <div>
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <select id="selSalesPayCreditCardExpirationDateMonth">
                        <option value="">MMM</option>
                    </select>
                    <select id="selSalesPayCreditCardExpirationDateDay">
                        <option value="">DD</option>
                    </select>
                    <select id="selSalesPayCreditCardExpirationDateYear">
                        <option value="">YYYY</option>
                    </select>
                </fieldset>
            </div>
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayCreditCardApprovalNo">Approval No.:</label>
            <input type="text" id="txtSalesPayCreditCardApprovalNo" />
       
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayCreditCardNoOfPayments">Number of Installment Payments:</label>
           <input type="number" id="txtSalesPayCreditCardNoOfPayments" />
    
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayCreditCardTenderedAmount">Amount: <span style="color: #FF0000;">*</span></label>
            <input type="number" id="txtSalesPayCreditCardTenderedAmount" maxlength="10" />
    
        </div>
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnSalesPayCreditCardSave" data-role="button">Ok</a>
            </div>
            <div style="text-align: center">
                <a id="btnSalesPayCreditCardRemove" data-role="button">Remove</a>
            </div>
            <div style="text-align: center">
                <a id="btnSalesPayCreditCardCancel" data-role="button">Cancel</a>
            </div>
        </div>
</div>
