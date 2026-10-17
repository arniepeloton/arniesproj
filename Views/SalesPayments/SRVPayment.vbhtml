<div data-role="content" style="padding:0px 20px 0px 20px">
    <div class="ui-grid-a">
        <div class="ui-block-a" style="width: 20%">
          <button data-icon="back" id="btnBackSRV">Back</button>
        </div>
        <div class="ui-block-b" style="text-align:right;width: 80%">
              <h2>Sales Return Voucher (SRV)</h2>    
        </div>
    </div>

        <div data-role="fieldcontain" style="display: none;">
            <input type="text" readonly id="txtSalesPayTransSRVID" />
        </div>
        <div class="ui-body ui-body-b">
            <div data-role="fieldcontain">
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <label>
                            Sub Total:</label>
                    </div>
                    <div class="ui-block-b">
                        <label id="lblSalesPaySRVSubTotal" style="float: right; text-align: right;">
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
                        <label id="lblSalesPaySRVTotalDiscount" style="float: right; text-align: right;">
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
                        <label id="lblSalesPaySRVCharges" style="float: right; text-align: right;">
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
                            <label id="lblSalesPaySRVAmountDue" style="float: right; text-align: right; font-size: xx-large;">0.00</label>
                        </div>
                    </div>
                Else
                    @<div>
                        <label>
                            Amount Due:</label></div>
                    @<div>
                        <label id="lblSalesPaySRVAmountDue" style="float: right; text-align: right; font-size: xx-large;">
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
                        <label id="lblSalesPaySRVTotalAmountTendered" style="float: right; text-align: right;">
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
                        <label id="lblSalesPaySRVAmountLacking" style="float: right; text-align: right;">
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
                        <label id="lblSalesPaySRVTotalChangeAmount" style="float: right; text-align: right;">
                            0.00</label>
                    </div>
                </div>
            </div>
        </div>
        
        <div style="display: none;"><input type="text" readonly id="txtSalesPaySRVTransMemoID" /></div>
        <div class="custom-ui-label">
            <div>SRV No.: <span style="color: #FF0000;">*</span></div>
            <div>
                <input type="number" id="txtSalesPaySRVNo" maxlength="8" />
                <a id="btnSalesPayScanSRVNumber" data-role="button">Scan SRV</a>
            </div>
        </div>

        <div class="custom-ui-label">
            <div>PIN Code: <span style="color: #FF0000;">*</span></div>
            <div>
                <input type="password" id="txtSalesPaySRVPinCode" />
                <a id="btnSalesPayScanSRVPin" data-role="button">Scan Pin Code</a>
                <br />
                <a id="btnSalesPaySRVVerify" data-role="button">Verify SRV</a>
            </div>
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPaySRVAmount">SRV Amount: <span style="color: #FF0000;">*</span></label>
            <input type="number" readonly id="txtSalesPaySRVAmount" />
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPaySRVConsumedAmount">Amount: <span style="color: #FF0000;">*</span></label>
            <input type="number" readonly id="txtSalesPaySRVConsumedAmount" />
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnSalesPaySRVSave" data-role="button">Ok</a>
            </div>
            <div style="text-align: center">
                <a id="btnSalesPaySRVRemove" data-role="button">Remove</a>
            </div>
            <div style="text-align: center">
                <a id="btnSalesPaySRVCancel" data-role="button">Cancel</a>
            </div>
        </div>
    </div>