<div data-role="content" style="padding:0px 20px 0px 20px">
    <div class="ui-grid-a">
        <div class="ui-block-a" style="width: 20%">
            <button data-icon="back" id="btnBackLoyaltyCard">Back</button>
        </div>
        <div class="ui-block-b" style="text-align:right;width: 80%">
               <h2>Loyalty Card</h2> 
        </div>
    </div>
 
        <div data-role="fieldcontain" style="display: none;">
            <input type="text" readonly id="txtSalesPayLoyaltyCardID" />
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayLoyaltyCardIssuingOrg">Issuing Organization: <span style="color: #FF0000;">*</span></label>
            <fieldset data-role="controlgroup">
                <input type="text" id="txtSalesPayLoyaltyCardIssuingOrg" />
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayLoyaltyCarHolder">Card Holder:</label>
            <fieldset data-role="controlgroup">
                <input type="text" id="txtSalesPayLoyaltyCarHolder" />
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayLoyaltyCardNo">Card No.: <span style="color: #FF0000;">*</span></label>
            <fieldset data-role="controlgroup">
                <input type="text" id="txtSalesPayLoyaltyCardNo" />
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <label>Issuance Date:</label>
            <fieldset data-role="controlgroup" data-type="horizontal">
                <select id="selSalesPayLoyaltyCardIssuanceDateMonth">
                    <option value="">MMM</option>
                </select>
                <select id="selSalesPayLoyaltyCardIssuanceDateDay">
                    <option value="">DD</option>
                </select>
                <select id="selSalesPayLoyaltyCardIssuanceDateYear">
                    <option value="">YYYY</option>
                </select>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <label>Expiration Date:</label>
            <fieldset data-role="controlgroup" data-type="horizontal">
                <select id="selSalesPayLoyaltyCardExpirationDateMonth">
                    <option value="">MMM</option>
                </select>
                <select id="selSalesPayLoyaltyCardExpirationDateDay">
                    <option value="">DD</option>
                </select>
                <select id="selSalesPayLoyaltyCardExpirationDateYear">
                    <option value="">YYYY</option>
                </select>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayLoyaltyCardApprovalNo">Approval No.:</label>
            <fieldset data-role="controlgroup">
                <input type="text" id="txtSalesPayLoyaltyCardApprovalNo" />
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayLoyaltyCardAmount">Amount: <span style="color: #FF0000;">*</span></label>
            <fieldset data-role="controlgroup">
                <input type="number" id="txtSalesPayLoyaltyCardAmount" />
            </fieldset>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnSalesPayLoyaltyCardSave" data-role="button">Ok</a>
            </div>
            <div style="text-align: center">
                <a id="btnSalesPayLoyaltyCardRemove" data-role="button">Remove</a>
            </div>
            <div style="text-align: center">
                <a id="btnSalesPayLoyaltyCardCancel" data-role="button">Cancel</a>
            </div>
        </div>
    </div>