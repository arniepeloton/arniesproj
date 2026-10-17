
<div data-role="content" style="padding:0px 20px 0px 20px">
    <div class="ui-grid-a">
        <div class="ui-block-a" style="width: 20%">
            <button data-icon="back" id="btnBackOtherPaymentType">Back</button>
        </div>
        <div class="ui-block-b" style="text-align:right;width: 80%">
            <h2> Other Payment Type</h2>
        </div>
    </div>

    <div data-role="fieldcontain" style="display: none;">
        <input type="text" readonly id="txtSalesPayOtherPaymentTypeID" />
    </div>
    <div class="ui-body ui-body-b">
        <div data-role="fieldcontain">
            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <label>
                        Sub Total:
                    </label>
                </div>
                <div class="ui-block-b">
                    <label id="lblSalesPayOtherTypeSubTotal" style="float: right; text-align: right;">
                        0.00
                    </label>
                </div>
            </div>
        </div>
        <div data-role="fieldcontain">
            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <label>
                        Less : Discount Amount
                    </label>
                </div>
                <div class="ui-block-b">
                    <label id="lblSalesPayOtherTypeTotalDiscount" style="float: right; text-align: right;">
                        0.00
                    </label>
                </div>
            </div>
        </div>
        <div data-role="fieldcontain">
            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <label>
                        Add : Charges
                    </label>
                </div>
                <div class="ui-block-b">
                    <label id="lblSalesPayOtherTypeCharges" style="float: right; text-align: right;">
                        0.00
                    </label>
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
                            <label id="lblSalesPayOtherTypeAmountDue" style="float: right; text-align: right; font-size: xx-large;">0.00</label>
                        </div>
                    </div>
            Else
                @<div>
                        <label>
                            Amount Due:</label></div>
                @<div>
                        <label id="lblSalesPayOtherTypeAmountDue" style="float: right; text-align: right; font-size: xx-large;">
                            0.00</label></div>
            End If
        </div>
        <div data-role="fieldcontain">
            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <label>
                        Amount Tendered:
                    </label>
                </div>
                <div class="ui-block-b">
                    <label id="lblSalesPayOtherTypeTotalAmountTendered" style="float: right; text-align: right;">
                        0.00
                    </label>
                </div>
            </div>
        </div>
        <div data-role="fieldcontain">
            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <label>
                        Amount Lacking:
                    </label>
                </div>
                <div class="ui-block-b">
                    <label id="lblSalesPayOtherTypeAmountLacking" style="float: right; text-align: right;">
                        0.00
                    </label>
                </div>
            </div>
        </div>
        <div data-role="fieldcontain">
            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <label>
                        Change Amount:
                    </label>
                </div>
                <div class="ui-block-b">
                    <label id="lblSalesPayOtherTypeTotalChangeAmount" style="float: right; text-align: right;">
                        0.00
                    </label>
                </div>
            </div>
        </div>
    </div>
    
    <div data-role="fieldcontain">
        <label for="selOtherPaymentType">Payment Type: <span style="color: #FF0000;">*</span></label>
        <select id="selOtherPaymentType">
            <option value="">Please select</option>
            <option value="Complimentary">Complimentary</option>
            <option value="Discount Voucher / On Line Deals">Discount Voucher / On Line Deals</option>
            <option value="E-Plus">E-Plus</option>
            <option value="GCash">GCash</option>
            <option value="SMAC">SMAC</option>
            <option value="V-Plus">V Plus</option>
            @*<option value="Room In">Room In</option>
            <option value="Send Bill">Send Bill</option>*@
        </select>
    </div>

    <div id="divForVSED" style="display:block;">
        <div data-role="fieldcontain">
            <label for="txtSalesPayOtherPaymentTypeAvailablePoints">Available Points: <span style="color: #FF0000;">*</span></label>
            <input type="text" id="txtSalesPayOtherPaymentTypeAvailablePoints" />
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayOtherPaymentTypeIssuingOrg">Issuing Organization: <span style="color: #FF0000;">*</span></label>
            <input type="text" id="txtSalesPayOtherPaymentTypeIssuingOrg" />
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayOtherPaymentTypeCardHolder">Card Holder:</label>
            <input type="text" id="txtSalesPayOtherPaymentTypeCardHolder" />

        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayOtherPaymentTypeCardNo">Card No.: <span style="color: #FF0000;">*</span></label>
            <input type="text" id="txtSalesPayOtherPaymentTypeCardNo" />

        </div>

        <div class="custom-ui-label">
            <div>Issuance Date:</div>
            <div>
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <select id="selSalesPayOtherPaymentTypeCardIssuanceDateMonth">
                        <option value="">MMM</option>
                    </select>
                    <select id="selSalesPayOtherPaymentTypeCardIssuanceDateDay">
                        <option value="">DD</option>
                    </select>
                    <select id="selSalesPayOtherPaymentTypeCardIssuanceDateYear">
                        <option value="">YYYY</option>
                    </select>
                </fieldset>
            </div>
        </div>

        <div class="custom-ui-label">
            <div>Expiration Date:</div>
            <div>
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <select id="selSalesPayOtherPaymentTypeCardExpirationDateMonth">
                        <option value="">MMM</option>
                    </select>
                    <select id="selSalesPayOtherPaymentTypeCardExpirationDateDay">
                        <option value="">DD</option>
                    </select>
                    <select id="selSalesPayOtherPaymentTypeCardExpirationDateYear">
                        <option value="">YYYY</option>
                    </select>
                </fieldset>
            </div>
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayOtherPaymentTypeApprovalNo">Approval No.:</label>
            <input type="text" id="txtSalesPayOtherPaymentTypeApprovalNo" />
        </div>
    </div>

    <div id="divForRoomIn" data-role="fieldcontain" style="display:none;">
        <label for="txtSalesPayOtherPaymentTypeRoomNo">Room No: <span style="color: #FF0000;">*</span></label>
        <input type="text" id="txtSalesPayOtherPaymentTypeRoomNo" />
    </div>

    <div data-role="fieldcontain">
        <label for="txtSalesPayOtherPaymentTypeAmount">Amount: <span style="color: #FF0000;">*</span></label>
        <input type="number" id="txtSalesPayOtherPaymentTypeAmount" maxlength="10" />
    </div>

    <div id="divForRoomInSendBill" data-role="fieldcontain" style="display:none;">
        <label for="txtSalesPayOtherPaymentTypeRemarks">Remarks:</label>
        <input type="text" id="txtSalesPayOtherPaymentTypeRemarks" />
    </div>

    <div id="divForGCash" data-role="fieldcontain" style="display:none">
        <div class="custom-ui-label">
            <div></div>
            <div>
                <button data-role="button" id="btnGCashPayBal">Auto Fill Payment Balance</button>
            </div>
        </div>
        <div data-role="fieldcontain">
            <label for="txtSalesPayOtherPaymentTypeGCashRefNo">GCash Reference Number: <span style="color: #FF0000;">*</span></label>
            <input type="number" id="txtSalesPayOtherPaymentTypeGCashRefNo" />
        </div>
    </div>

    <div>
        <br />
        <br />
        <div style="text-align: center">
            <a id="btnSalesPayOtherPaymentTypeSave" data-role="button">Ok</a>
        </div>
        <div style="text-align: center">
            <a id="btnSalesPayOtherPaymentTypeRemove" data-role="button">Remove</a>
        </div>
        <div style="text-align: center">
            <a id="btnSalesPayOtherPaymentTypeCancel" data-role="button">Cancel</a>
        </div>
    </div>

</div>