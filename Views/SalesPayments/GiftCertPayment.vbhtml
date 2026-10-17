<div data-role="content" style="padding:0px 20px 0px 20px">
    <div class="ui-grid-a">
        <div class="ui-block-a" style="width: 20%">
             <button data-icon="back" id="btnBackGiftCert">Back</button>
        </div>
        <div class="ui-block-b" style="text-align:right;width: 80%">
               <h2>Gift Certificate</h2>
        </div>
    </div>

        <div data-role="fieldcontain" style="display: none;">
            <input type="text" readonly id="txtSalesPayGiftCertID" />
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayGiftCertIssuingOrg">Issuing Organization: <span style="color: #FF0000;">*</span></label>
            <input type="text" id="txtSalesPayGiftCertIssuingOrg" />
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayGiftCertIssuedTo">Issued To:</label>
            <input type="text" id="txtSalesPayGiftCertIssuedTo" />
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayGiftCertNo">Gift Certificate No.: <span style="color: #FF0000;">*</span></label>
            <input type="text" id="txtSalesPayGiftCertNo" />
        </div>

        <div data-role="fieldcontain">
            <label>Date of Issue:</label>
            <fieldset data-role="controlgroup" data-type="horizontal">
                <select id="selSalesPayGiftCertDateOfIssueMonth">
                    <option value="">MMM</option>
                </select>
                <select id="selSalesPayGiftCertDateOfIssueDay">
                    <option value="">DD</option>
                </select>
                <select id="selSalesPayGiftCertDateOfIssueYear">
                    <option value="">YYYY</option>
                </select>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <label>Expiration Date: <span style="color: #FF0000;">*</span></label>
            <fieldset data-role="controlgroup" data-type="horizontal">
                <select id="selSalesPayGiftCertExpirationDateMonth">
                    <option value="">MMM</option>
                </select>
                <select id="selSalesPayGiftCertExpirationDateDay">
                    <option value="">DD</option>
                </select>
                <select id="selSalesPayGiftCertExpirationDateYear">
                    <option value="">YYYY</option>
                </select>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <label for="txtSalesPayGiftCertAmount">Amount: <span style="color: #FF0000;">*</span></label>
            <fieldset data-role="controlgroup">
                <input type="number" id="txtSalesPayGiftCertAmount" />
            </fieldset>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnSalesPayGiftCertSave" data-role="button">Ok</a>
            </div>
            <div style="text-align: center">
                <a id="btnSalesPayGiftCertRemove" data-role="button">Remove</a>
            </div>
            <div style="text-align: center">
                <a id="btnSalesPayGiftCertCancel" data-role="button">Cancel</a>
            </div>
        </div>
    </div>