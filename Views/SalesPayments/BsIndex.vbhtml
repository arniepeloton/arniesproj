
@*<link type="text/css" href="/Content/bootstrap-custom.css" rel="Stylesheet" />*@
<script>
    AjaxSend({
            ExactUrl: '/Scripts3/view/BsPayments.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                $('#pageSalesPayment')
                    .html('')
                    .html(vData)
                    //.trigger('create');
                $('#divHeader').hide();
                $('#divHotButtons').hide();
                $('#divPayment').show();

                // https://stackoverflow.com/a/47760233/334045
                $('input[type=number]').on('wheel', function(e){
                    return false;
                });

                require([GetPathFormat('promise!/Scripts/sources/SalesPayments/Bs_SalesPayments.js?crc='),
                    GetPathFormat('/Scripts3/Browser/LibraryOrganizationBanks.js?crc='),
                    GetPathFormat('/Scripts3/Browser/LibraryCountries.js?crc=')
                ]);

                if (typeof proj.SalesPayments != 'undefined') {
                    if (typeof proj.SalesPayments.MainFunction == 'function') {
                        proj.SalesPayments.MainFunction();
                    }
                }
            }
        });   
</script>
<style>
    .menu-description {
        font-size: 13px;
        overflow: hidden;
        text-overflow: ellipsis;
        /*display: -webkit-box;*/
        -webkit-line-clamp: 2;
        -webkit-box-orient: vertical;
        height: 40px;
        cursor: pointer;
    }
</style>

<div id="pageSalesPayment" subpage="paymentpage" style="display:block;">
    <div id="divPaymentInfoLoading" class="d-flex position-absolute w-100 h-100 justify-content-center align-items-center" style="width:100%; text-align:center;">
        <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden"></span>
        </div>
    </div>
</div>

<div id="pageSalesPayCheck" subpage="paymentpage" style="display:none;">
    <nav class="navbar navbar-dark bg-primary sticky-md-top text-center" style="font-size:large; color:#FFF; display:block;">
        Check/ Gift Certificate Payment
    </nav>
    <div class="container-fluid mt-3">
        <div style="display: none;">
            <input data-role="none" class="form-control" type="text" readonly id="txtSalesPayCheckID" />
        </div>

        <table style="width:100%; background-color:#e8e8e8; border: 1px solid #CCC;" cellpadding="5">
            <tr>
                <td>Sub Total:</td>
                <td id="lblSalesPayCheckSubTotal" style="text-align: right;">0.00</td>
            </tr>
            <tr>
                <td>Less Discount Amount:</td>
                <td id="lblSalesPayCheckTotalDiscount" style="text-align: right;">0.00</td>
            </tr>
            <tr>
                <td>Add Charges:</td>
                <td id="lblSalesPayCheckCharges" style="text-align: right;">0.00</td>
            </tr>
            <tr>
                <td style="font-weight:bold;">Amount Due:</td>
                <td id="lblSalesPayCheckAmountDue" style="text-align: right; font-size: xx-large; font-weight:bold;">0.00</td>
            </tr>
            <tr>
                <td>Amount Tendered:</td>
                <td id="lblSalesPayCheckTotalAmountTendered" style="text-align: right;">0.00</td>
            </tr>
            <tr>
                <td>Amount Lacking:</td>
                <td id="lblSalesPayCheckAmountLacking" style="text-align: right;">0.00</td>
            </tr>
            <tr>
                <td>Change Amount:</td>
                <td id="lblSalesPayCheckTotalChangeAmount" style="text-align: right;">0.00</td>
            </tr>
        </table>

        <div style="display: none;"><input data-role="none" class="form-control" type="text" readonly id="txtSalesPayCheckBankID" /></div>

        <div class="row mb-3 mt-3">
            <label class="col-md-2">Options:</label>
            <div class="col-md-10">
                <div class="form-check form-check-inline">
                    <input data-role="none" class="form-check-input" type="radio" name="radOpPayCheck" id="radOpCheck" value="check">
                    <label class="form-check-label" for="radOpCheck">Bank Check</label>
                </div>
                <div class="form-check form-check-inline">
                    <input data-role="none" class="form-check-input" type="radio" name="radOpPayCheck" id="radOpGC" value="gc">
                    <label class="form-check-label" for="radOpGC">Gift Check/ Certificate</label>
                </div>
            </div>
        </div>

        <div id="divForCheck" style="display:none;">
            <div class="row mb-3">
                <label for="btnSalesPayCheckBank" class="col-md-2">Bank/ Issuing Organization: <span style="color: #FF0000;">*</span></label>
                <div class="col-md-10">
                    <div id="divCheckBankButton" class="mb-3">
                        <button id="btnSalesPayCheckBank" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
                    </div>

                    <div id="divCheckBankTextbox" class="mb-3" style="display:none;">
                        <input data-role="none" class="form-control" type="text" id="txtSalesPayCheckBankName" placeholder="Enter Bank name" />
                    </div>
                </div>
            </div>

            <div style="display: none;"><input data-role="none" class="form-control" type="text" readonly id="txtSalesPayCheckBankBranchID" /></div>

            <div class="row mb-3">
                <label for="txtSalesPayCheckBankBranch" class="col-md-2">Bank/ Issuing Organization Branch: <span style="color: #FF0000;">*</span></label>
                <div class="col-md-10">
                    <input data-role="none" class="form-control mb-3" type="text" id="txtSalesPayCheckBankBranch" style="display:none;" />

                    <div id="divCheckBankBranchButton" class="mb-3">
                        <button id="btnSalesPayCheckBankBranch" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
                    </div>
                </div>
            </div>

            <div class="row mb-3">
                <label for="txtSalesPayCheckAccountName" class="col-md-2">Account Name (Issued To): <span style="color: #FF0000;">*</span></label>
                <div class="col-md-10">
                    <input data-role="none" class="form-control" type="text" id="txtSalesPayCheckAccountName" />
                </div>
            </div>

            <div class="row mb-3">
                <label for="txtSalesPayCheckAccountNo" class="col-md-2">Account No: <span style="color: #FF0000;">*</span></label>
                <div class="col-md-10">
                    <input data-role="none" class="form-control" type="text" id="txtSalesPayCheckAccountNo" />
                </div>
            </div>
        </div>

        <div class="row mb-3">
            <label for="txtSalesPayCheckNo" class="col-md-2"><span id="lblPayCheckNo"></span></label>
            <div class="col-md-10">
                <input data-role="none" class="form-control" type="text" id="txtSalesPayCheckNo" />
            </div>
        </div>

        <div class="row mb-3">
            <label for="txtSalesPayCheckDate" class="col-md-2"><span id="lblPayCheckDate"></span></label>
            <div class="col-md-10">
                <input data-role="none" class="form-control" type="date" id="txtSalesPayCheckDate" placeholder="Tap to set date" />
            </div>
        </div>

        <div class="row mb-3">
            <label for="txtSalesPayCheckAmount" class="col-md-2"><span id="lblPayCheckAmount"></span></label>
            <div class="col-md-10">
                <input data-role="none" class="form-control" type="number" id="txtSalesPayCheckAmount" />
            </div>
        </div>

        <div class="row mt-5 mb-3">
            <div class="col-md mb-3 order-sm-2" style="text-align: center">
                <a id="btnSalesPayCheckSave" data-role="none" class="btn btn-block btn-success btn-lg">Ok</a>
            </div>
            <div id="btnSalesPayCheckRemove" class="col-md mb-3 order-sm-1" style="text-align: center">
                <a data-role="none" class="btn btn-block btn-danger btn-lg">Remove</a>
            </div>
            <div class="col-md mb-3 order-sm-0" style="text-align: center">
                <a id="btnSalesPayCheckCancel" data-role="none" class="btn btn-block btn-secondary btn-lg">Cancel</a>
            </div>
        </div>
    </div>
</div>

<div id="pageSalesPayDebitCard" subpage="paymentpage" style="display:none;">
    <nav class="navbar navbar-dark bg-primary sticky-md-top text-center" style="font-size:large; color:#FFF; display:block;">
        Debit Card Payment
    </nav>
    <div class="container-fluid mt-3">
        <div style="display: none;">
            <input data-role="none" class="form-control" type="text" readonly id="txtSalesPayDebitCardID" />
        </div>

        <table style="width:100%; background-color:#e8e8e8; border: 1px solid #CCC;" cellpadding="5">
            <tr>
                <td>Sub Total:</td>
                <td id="lblSalesPayDebitCardSubTotal" style="text-align: right;">0.00</td>
            </tr>
            <tr>
                <td>Less Discount Amount:</td>
                <td id="lblSalesPayDebitCardTotalDiscount" style="text-align: right;">0.00</td>
            </tr>
            <tr>
                <td>Add Charges:</td>
                <td id="lblSalesPayDebitCardCharges" style="text-align: right;">0.00</td>
            </tr>
            <tr>
                <td style="font-weight:bold;">Amount Due:</td>
                <td id="lblSalesPayDebitCardAmountDue" style="text-align: right; font-size: xx-large; font-weight:bold;">0.00</td>
            </tr>
            <tr>
                <td>Amount Tendered:</td>
                <td id="lblSalesPayDebitCardTotalAmountTendered" style="text-align: right;">0.00</td>
            </tr>
            <tr>
                <td>Amount Lacking:</td>
                <td id="lblSalesPayDebitCardAmountLacking" style="text-align: right;">0.00</td>
            </tr>
            <tr>
                <td>Change Amount:</td>
                <td id="lblSalesPayDebitCardTotalChangeAmount" style="text-align: right;">0.00</td>
            </tr>
        </table>
        
        <div class="row mt-3" style="display:none;">
            <label for="selSalesPayDebitCardType" class="col-md-2">Debit Card: <span style="color: #FF0000;">*</span></label>
            <div class="col-md-10">
                <select id="selSalesPayDebitCardType" data-role="none" class="form-select">
                    <option value="">Please Select</option>
                    <option value="Visa">Visa</option>
                    <option value="Master Card">Master Card</option>
                    <option value="JCB">JCB</option>
                    <option value="Diners">Diners</option>
                    <option value="American Express">American Express</option>
                    <option value="Other Card">Other Card</option>
                </select>
            </div>
        </div>

        <div style="display: none;"><input type="text" readonly id="txtSalesPayDebitCardBankID" /></div>
        <div class="row mb-3 mt-3">
            <label for="btnSalesPayDebitCardBank" class="col-md-2">Issuing Bank: <span style="color: #FF0000;">*</span></label>
            <div class="col-md-10">
                <div id="divSalesPayDebitCardBankButton" class="mb-3">
                    <button id="btnSalesPayDebitCardBank" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
                </div>
                <div id="divSalesPayDebitCardBankTextbox" class="mb-3" style="display:none;">
                    <input type="text" id="txtSalesPayDebitCardBank" data-role="none" class="form-control" />
                </div>
            </div>
        </div>

        <div style="display: none;"><input type="text" readonly id="txtSalesPayDebitCardBankLocationID" /></div>
        <div class="row mb-3">
            <label for="btnSalesPayDebitCardBankLocation" class="col-md-2">Bank Location (Country):</label>
            <div class="col-md-10">
                <button id="btnSalesPayDebitCardBankLocation" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
            </div>
        </div>

        <div class="accordion mb-3">
            <div class="card">
                <div class="card-header p-0">
                    <button class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none">
                        Card Holder Name
                    </button>
                </div>
                <div class="collapse show">
                    <div class="card-body">
                        <div class="row mb-3">
                            <label for="txtSalesPayDebitCardLastName" class="col-md-2">Last Name: <span style="color: #FF0000;">*</span></label>
                            <div class="col-md-10">
                                <input type="text" id="txtSalesPayDebitCardLastName" data-role="none" class="form-control" />
                            </div>
                        </div>
                        <div class="row mb-3">
                            <label for="txtSalesPayDebitCardFirstName" class="col-md-2">First Name: <span style="color: #FF0000;">*</span></label>
                            <div class="col-md-10">
                                <input type="text" id="txtSalesPayDebitCardFirstName" data-role="none" class="form-control" />
                            </div>
                        </div>
                        <div class="row mb-3">
                            <label for="txtSalesPayDebitCardMiddleName" class="col-md-2">Middle Name:</label>
                            <div class="col-md-10">
                                <input type="text" id="txtSalesPayDebitCardMiddleName" data-role="none" class="form-control" />
                            </div>
                        </div>
                        <div class="row mb-3">
                            <label for="selSalesPayDebitPersonalSuffixName" class="col-md-2">Generational Suffix (Name):</label>
                            <div class="col-md-10">
                                <select id="selSalesPayDebitPersonalSuffixName" data-role="none" class="form-select">
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
                    </div>
                </div>
            </div>
        </div>        

        <div class="row mb-3">
            <label for="txtSalesPayDebitCardNo" class="col-md-2">Debit Card No.: <span style="color: #FF0000;">*</span></label>
            <div class="col-md-10">
                <input type="text" id="txtSalesPayDebitCardNo" data-role="none" class="form-control mb-3" maxlength="4" />
                Please enter the last four(4) digit of the card only.</p>
            </div>
        </div>

        <div class="row mb-3 d-none">
            <label for="txtSalesPayDebitCardIssuanceDate" class="col-md-2">Issuance Date:</label>
            <div class="col-md-10">
                <input type="date" id="txtSalesPayDebitCardIssuanceDate" data-role="none" class="form-control" />
            </div>
        </div>

        <div class="row mb-3 d-none">
            <label for="txtSalesPayDebitCardExpirationDate" class="col-md-2">Expiration Date:</label>
            <div class="col-md-10">
                <input type="date" id="txtSalesPayDebitCardExpirationDate" data-role="none" class="form-control" />
            </div>
        </div>

        <div class="row mb-3 d-none">
            <label for="txtSalesPayDebitCardApprovalNo" class="col-md-2">Approval No.:</label>
            <div class="col-md-10">
                <input type="text" id="txtSalesPayDebitCardApprovalNo" data-role="none" class="form-control" />
            </div>
        </div>

        <div class="row mb-3 d-none">
            <label for="txtSalesPayDebitCardNoOfPayments" class="col-md-2">Number of Installment Payments:</label>
            <div class="col-md-10">
                <input type="number" id="txtSalesPayDebitCardNoOfPayments" data-role="none" class="form-control" />
            </div>
        </div>

        <div class="row mb-3">
            <label for="txtSalesPayDebitCardTenderedAmount" class="col-md-2">Amount: <span style="color: #FF0000;">*</span></label>
            <div class="col-md-10">
                <input type="number" id="txtSalesPayDebitCardTenderedAmount" data-role="none" class="form-control" />
            </div>
        </div>

        <div class="row mt-5 mb-3">
            <div class="col-md mb-3 order-sm-2" style="text-align: center">
                <a id="btnSalesPayDebitCardSave" data-role="none" class="btn btn-block btn-success btn-lg">Ok</a>
            </div>
            <div id="btnSalesPayDebitCardRemove" class="col-md mb-3 order-sm-1" style="text-align: center">
                <a data-role="none" class="btn btn-block btn-danger btn-lg">Remove</a>
            </div>
            <div class="col-md mb-3 order-sm-0" style="text-align: center">
                <a id="btnSalesPayDebitCardCancel" data-role="none" class="btn btn-block btn-secondary btn-lg">Cancel</a>
            </div>
        </div>
    </div>
</div>

<div id="pageSalesPayCreditCard" subpage="paymentpage" style="display:none;">
    <nav class="navbar navbar-dark bg-primary sticky-md-top text-center" style="font-size:large; color:#FFF; display:block;">
        Credit Card Payment
    </nav>
    <div class="container-fluid mt-3">
        <div style="display: none;">
            <input data-role="none" class="form-control" type="text" readonly id="txtSalesPayCreditCardID" />
        </div>

        <table style="width:100%; background-color:#e8e8e8; border: 1px solid #CCC;" cellpadding="5">
            <tr>
                <td>Sub Total:</td>
                <td id="lblSalesPayCreditCardSubTotal" style="text-align: right;">0.00</td>
            </tr>
            <tr>
                <td>Less Discount Amount:</td>
                <td id="lblSalesPayCreditCardTotalDiscount" style="text-align: right;">0.00</td>
            </tr>
            <tr>
                <td>Add Charges:</td>
                <td id="lblSalesPayCreditCardCharges" style="text-align: right;">0.00</td>
            </tr>
            <tr>
                <td style="font-weight:bold;">Amount Due:</td>
                <td id="lblSalesPayCreditCardAmountDue" style="text-align: right; font-size: xx-large; font-weight:bold;">0.00</td>
            </tr>
            <tr>
                <td>Amount Tendered:</td>
                <td id="lblSalesPayCreditCardTotalAmountTendered" style="text-align: right;">0.00</td>
            </tr>
            <tr>
                <td>Amount Lacking:</td>
                <td id="lblSalesPayCreditCardAmountLacking" style="text-align: right;">0.00</td>
            </tr>
            <tr>
                <td>Change Amount:</td>
                <td id="lblSalesPayCreditCardTotalChangeAmount" style="text-align: right;">0.00</td>
            </tr>
        </table>

        <div class="row mt-3">
            <label for="selSalesPayCreditCardType" class="col-md-2">Credit Card: <span style="color: #FF0000;">*</span></label>
            <div class="col-md-10">
                <select id="selSalesPayCreditCardType" data-role="none" class="form-select">
                    <option value="">Please Select</option>
                    <option value="Visa">Visa</option>
                    <option value="Master Card">Master Card</option>
                    <option value="JCB">JCB</option>
                    <option value="Diners">Diners</option>
                    <option value="American Express">American Express</option>
                    <option value="Other Card">Other Card</option>
                </select>
            </div>
        </div>

        <div style="display: none;"><input type="text" readonly id="txtSalesPayCreditCardBankID" /></div>
        <div class="row mb-3 mt-3">
            <label for="btnSalesPayCreditCardBank" class="col-md-2">Issuing Bank: <span style="color: #FF0000;">*</span></label>
            <div class="col-md-10">
                <div id="divSalesPayCreditCardBankButton" class="mb-3">
                    <button id="btnSalesPayCreditCardBank" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
                </div>
                <div id="divSalesPayCreditCardBankTextbox" class="mb-3" style="display:none;">
                    <input type="text" id="txtSalesPayCreditCardBank" data-role="none" class="form-control" />
                </div>
            </div>
        </div>

        <div style="display: none;"><input type="text" readonly id="txtSalesPayCreditCardBankLocationID" /></div>
        <div class="row mb-3">
            <label for="btnSalesPayCreditCardBankLocation" class="col-md-2">Bank Location (Country):</label>
            <div class="col-md-10">
                <button id="btnSalesPayCreditCardBankLocation" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
            </div>
        </div>

        <div class="accordion mb-3">
            <div class="card">
                <div class="card-header p-0">
                    <button class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none">
                        Card Holder Name
                    </button>
                </div>
                <div class="collapse show">
                    <div class="card-body">
                        <div class="row mb-3">
                            <label for="txtSalesPayCreditCardLastName" class="col-md-2">Last Name: <span style="color: #FF0000;">*</span></label>
                            <div class="col-md-10">
                                <input type="text" id="txtSalesPayCreditCardLastName" data-role="none" class="form-control" />
                            </div>
                        </div>
                        <div class="row mb-3">
                            <label for="txtSalesPayCreditCardFirstName" class="col-md-2">First Name: <span style="color: #FF0000;">*</span></label>
                            <div class="col-md-10">
                                <input type="text" id="txtSalesPayCreditCardFirstName" data-role="none" class="form-control" />
                            </div>
                        </div>
                        <div class="row mb-3">
                            <label for="txtSalesPayCreditCardMiddleName" class="col-md-2">Middle Name:</label>
                            <div class="col-md-10">
                                <input type="text" id="txtSalesPayCreditCardMiddleName" data-role="none" class="form-control" />
                            </div>
                        </div>
                        <div class="row mb-3">
                            <label for="selSalesPayCreditPersonalSuffixName" class="col-md-2">Generational Suffix (Name):</label>
                            <div class="col-md-10">
                                <select id="selSalesPayCreditPersonalSuffixName" data-role="none" class="form-select">
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
                    </div>
                </div>
            </div>
        </div>

        <div class="row mb-3">
            <label for="txtSalesPayCreditCardNo" class="col-md-2">Credit Card No.: <span style="color: #FF0000;">*</span></label>
            <div class="col-md-10">
                <input type="text" id="txtSalesPayCreditCardNo" data-role="none" class="form-control mb-3" maxlength="4" />
                Please enter the last four(4) digit of the card only.</p>
            </div>
        </div>

        <div class="row mb-3 d-none">
            <label for="txtSalesPayCreditCardIssuanceDate" class="col-md-2">Issuance Date:</label>
            <div class="col-md-10">
                <input type="date" id="txtSalesPayCreditCardIssuanceDate" data-role="none" class="form-control" />
            </div>
        </div>

        <div class="row mb-3 d-none">
            <label for="txtSalesPayCreditCardExpirationDate" class="col-md-2">Expiration Date:</label>
            <div class="col-md-10">
                <input type="date" id="txtSalesPayCreditCardExpirationDate" data-role="none" class="form-control" />
            </div>
        </div>

        <div class="row mb-3 d-none">
            <label for="txtSalesPayCreditCardApprovalNo" class="col-md-2">Approval No.:</label>
            <div class="col-md-10">
                <input type="text" id="txtSalesPayCreditCardApprovalNo" data-role="none" class="form-control" />
            </div>
        </div>

        <div class="row mb-3 d-none">
            <label for="txtSalesPayCreditCardNoOfPayments" class="col-md-2">Number of Installment Payments:</label>
            <div class="col-md-10">
                <input type="number" id="txtSalesPayCreditCardNoOfPayments" data-role="none" class="form-control" />
            </div>
        </div>

        <div class="row mb-3">
            <label for="txtSalesPayCreditCardTenderedAmount" class="col-md-2">Amount: <span style="color: #FF0000;">*</span></label>
            <div class="col-md-10">
                <input type="number" id="txtSalesPayCreditCardTenderedAmount" data-role="none" class="form-control" />
            </div>
        </div>

        <div class="row mt-5 mb-3">
            <div class="col-md mb-3 order-sm-2" style="text-align: center">
                <a id="btnSalesPayCreditCardSave" data-role="none" class="btn btn-block btn-success btn-lg">Ok</a>
            </div>
            <div id="btnSalesPayCreditCardRemove" class="col-md mb-3 order-sm-1" style="text-align: center">
                <a data-role="none" class="btn btn-block btn-danger btn-lg">Remove</a>
            </div>
            <div class="col-md mb-3 order-sm-0" style="text-align: center">
                <a id="btnSalesPayCreditCardCancel" data-role="none" class="btn btn-block btn-secondary btn-lg">Cancel</a>
            </div>
        </div>
    </div>
</div>

<div id="pageSalesPaySRV" subpage="paymentpage" style="display:none;">
    <nav class="navbar navbar-dark bg-primary sticky-md-top text-center" style="font-size:large; color:#FFF; display:block;">
        Sales Return Voucher (SRV)
    </nav>
    <div class="container-fluid mt-3">
        <div style="display: none;">
            <input data-role="none" class="form-control" type="text" readonly id="txtSalesPayTransSRVID" />
        </div>

        <table style="width:100%; background-color:#e8e8e8; border: 1px solid #CCC;" cellpadding="5">
            <tr>
                <td>Sub Total:</td>
                <td id="lblSalesPaySRVSubTotal" style="text-align: right;">0.00</td>
            </tr>
            <tr>
                <td>Less Discount Amount:</td>
                <td id="lblSalesPaySRVTotalDiscount" style="text-align: right;">0.00</td>
            </tr>
            <tr>
                <td>Add Charges:</td>
                <td id="lblSalesPaySRVCharges" style="text-align: right;">0.00</td>
            </tr>
            <tr>
                <td style="font-weight:bold;">Amount Due:</td>
                <td id="lblSalesPaySRVAmountDue" style="text-align: right; font-size: xx-large; font-weight:bold;">0.00</td>
            </tr>
            <tr>
                <td>Amount Tendered:</td>
                <td id="lblSalesPaySRVTotalAmountTendered" style="text-align: right;">0.00</td>
            </tr>
            <tr>
                <td>Amount Lacking:</td>
                <td id="lblSalesPaySRVAmountLacking" style="text-align: right;">0.00</td>
            </tr>
            <tr>
                <td>Change Amount:</td>
                <td id="lblSalesPaySRVTotalChangeAmount" style="text-align: right;">0.00</td>
            </tr>
        </table>

        <div style="display: none;"><input type="text" readonly id="txtSalesPaySRVTransMemoID" /></div>
        <div class="row mb-3 mt-3">
            <label for="txtSalesPaySRVNo" class="col-md-2">SRV No.: <span style="color: #FF0000;">*</span></label>
            <div class="col-md-10">
                <div class="input-group">
                    <input data-role="none" type="number" class="form-control" id="txtSalesPaySRVNo" maxlength="8">
                    <button data-role="none" class="btn btn-secondary" type="button" id="btnSalesPayScanSRVNumber">Scan SRV</button>
                </div>
            </div>
        </div>

        <div class="row mb-3">
            <label for="" class="col-md-2">PIN Code: <span style="color: #FF0000;">*</span></label>
            <div class="col-md-10">
                <div class="input-group">
                    <input data-role="none" type="text" class="form-control" id="txtSalesPaySRVPinCode">
                    <button data-role="none" class="btn btn-secondary" type="button" id="btnSalesPayScanSRVPin">Scan PIN Code</button>                    
                </div>
            </div>
        </div>

        <div class="row mb-3">
            <div class="offset-md-2 col-md-10">
                <button data-role="none" class="btn btn-secondary btn-block" type="button" id="btnSalesPaySRVVerify">Verify SRV</button>
            </div>
        </div>

        <div class="row mb-3">
            <label for="txtSalesPaySRVAmount" class="col-md-2">SRV Amount: <span style="color: #FF0000;">*</span></label>
            <div class="col-md-10">
                <input data-role="none" class="form-control" type="number" readonly id="txtSalesPaySRVAmount" />
            </div>
        </div>

        <div class="row mb-3">
            <label for="" class="col-md-2">Amount: <span style="color: #FF0000;">*</span></label>
            <div class="col-md-10">
                <input data-role="none" class="form-control" type="number" readonly id="txtSalesPaySRVConsumedAmount" />
            </div>
        </div>

        <div class="row mt-5 mb-3">
            <div class="col-md mb-3 order-sm-2" style="text-align: center">
                <a id="btnSalesPaySRVSave" data-role="none" class="btn btn-block btn-success btn-lg">Ok</a>
            </div>
            <div id="btnSalesPaySRVRemove" class="col-md mb-3 order-sm-1" style="text-align: center">
                <a data-role="none" class="btn btn-block btn-danger btn-lg">Remove</a>
            </div>
            <div class="col-md mb-3 order-sm-0" style="text-align: center">
                <a id="btnSalesPaySRVCancel" data-role="none" class="btn btn-block btn-secondary btn-lg">Cancel</a>
            </div>
        </div>
    </div>
</div>

<div id="pageSalesPayOtherPaymentType" subpage="paymentpage" style="display:none;">
    <nav class="navbar navbar-dark bg-primary sticky-md-top text-center" style="font-size:large; color:#FFF; display:block;">
        Other Payment Type
    </nav>
    <div class="container-fluid mt-3">
        <div style="display: none;">
            <input data-role="none" class="form-control" type="text" readonly id="txtSalesPayOtherPaymentTypeID" />
        </div>

        <table style="width:100%; background-color:#e8e8e8; border: 1px solid #CCC;" cellpadding="5">
            <tr>
                <td>Sub Total:</td>
                <td id="lblSalesPayOtherTypeSubTotal" style="text-align: right;">0.00</td>
            </tr>
            <tr>
                <td>Less Discount Amount:</td>
                <td id="lblSalesPayOtherTypeTotalDiscount" style="text-align: right;">0.00</td>
            </tr>
            <tr>
                <td>Add Charges:</td>
                <td id="lblSalesPayOtherTypeCharges" style="text-align: right;">0.00</td>
            </tr>
            <tr>
                <td style="font-weight:bold;">Amount Due:</td>
                <td id="lblSalesPayOtherTypeAmountDue" style="text-align: right; font-size: xx-large; font-weight:bold;">0.00</td>
            </tr>
            <tr>
                <td>Amount Tendered:</td>
                <td id="lblSalesPayOtherTypeTotalAmountTendered" style="text-align: right;">0.00</td>
            </tr>
            <tr>
                <td>Amount Lacking:</td>
                <td id="lblSalesPayOtherTypeAmountLacking" style="text-align: right;">0.00</td>
            </tr>
            <tr>
                <td>Change Amount:</td>
                <td id="lblSalesPayOtherTypeTotalChangeAmount" style="text-align: right;">0.00</td>
            </tr>
        </table>

        <div class="row mb-3 mt-3">
            <label for="selOtherPaymentType" class="col-md-2">Payment Type: <span style="color: #FF0000;">*</span></label>
            <div class="col-md-10">
                <select id="selOtherPaymentType" data-role="none" class="form-select">
                    <option value="">Please select</option>
                    <option value="AliPay">AliPay</option>
                    <option value="Complimentary">Complimentary</option>
                    <option value="Discount Voucher / On Line Deals">Discount Voucher / On Line Deals</option>
                    <option value="E-Plus">E-Plus</option>
                    <option value="FoodPanda">Food Panda</option>
                    <option value="GCash">GCash</option>
                    <option value="GoldCard">Gold Card</option>
                    <option value="GrabFood">Grab Food</option>
                    <option value="LocalDelivery">Local Delivery</option>
                    <option value="PayMaya">PayMaya</option>                    
                    <option value="ShopeeEVoucher">Shopee E-Voucher</option>
                    <option value="ShopeePay">Shopee Pay</option>
                    <option value="SMAC">SMAC</option>
                    <option value="V-Plus">V Plus</option>                    

                    @*<option value="">Please select</option>
                        <option value="Complimentary">Complimentary</option>
                        <option value="Discount Voucher / On Line Deals">Discount Voucher / On Line Deals</option>
                        <option value="E-Plus">E-Plus</option>
                        <option value="GCash">GCash</option>
                        <option value="SMAC">SMAC</option>
                        <option value="V-Plus">V Plus</option>*@
                                @*<option value="Room In">Room In</option>
                        <option value="Send Bill">Send Bill</option>*@
                </select>
            </div>
        </div>

        <div id="divForVSED" style="display:block;">
            <div class="row mb-3">
                <label for="txtSalesPayOtherPaymentTypeCardNo" class="col-md-2">Card/ Reference No.: <span style="color: #FF0000;">*</span> <span id="lblRefNoCharLength"></span></label>
                <div class="col-md-10">
                    <div class="input-group">
                        <input data-role="none" class="form-control rounded" type="text" id="txtSalesPayOtherPaymentTypeCardNo" />
                        <span class="input-group-text bg-white" id="vouchervalidation" style="display:none"></span>
                    </div>                    
                </div>
            </div>

            <div class="row mb-3 d-none">
                <label for="txtSalesPayOtherPaymentTypeCardHolder" class="col-md-2">Card/ Voucher Holder Name:</label>
                <div class="col-md-10">
                    <input data-role="none" class="form-control" type="text" id="txtSalesPayOtherPaymentTypeCardHolder" />
                </div>
            </div>

            <div class="row mb-3 d-none">
                <label for="txtSalesPayOtherPaymentTypeApprovalNo" class="col-md-2">Payment Approval No.:</label>
                <div class="col-md-10">
                    <input data-role="none" class="form-control" type="text" id="txtSalesPayOtherPaymentTypeApprovalNo" />
                </div>
            </div>

            <div class="row mb-3">
                <label for="txtSalesPayOtherPaymentTypeAmount" class="col-md-2">Payment Amount: <span style="color: #FF0000;">*</span></label>
                <div class="col-md-10">
                    <input data-role="none" class="form-control" type="number" id="txtSalesPayOtherPaymentTypeAmount" />
                </div>
            </div>

            <div class="row mb-3 d-none">
                <label for="txtSalesPayOtherPaymentTypeIssuingOrg" class="col-md-2">Issuing Organization:</label>
                <div class="col-md-10">
                    <input data-role="none" class="form-control" type="text" id="txtSalesPayOtherPaymentTypeIssuingOrg" />
                </div>
            </div>

            <div class="row mb-3 d-none">
                <label for="txtSalesPayOtherPaymentTypeAvailablePoints" class="col-md-2">Available Points:</label>
                <div class="col-md-10">
                    <input data-role="none" class="form-control" type="text" id="txtSalesPayOtherPaymentTypeAvailablePoints" />
                </div>
            </div>

            <div class="row mb-3 d-none">
                <label for="txtSalesPayOtherPaymentTypeCardIssuanceDate" class="col-md-2">Issuance Date:</label>
                <div class="col-md-10">
                    <input data-role="none" class="form-control" type="date" id="txtSalesPayOtherPaymentTypeCardIssuanceDate" />
                </div>
            </div>

            <div class="row mb-3 d-none">
                <label for="txtSalesPayOtherPaymentTypeCardExpirationDate" class="col-md-2">Expiration Date:</label>
                <div class="col-md-10">
                    <input data-role="none" class="form-control" type="date" id="txtSalesPayOtherPaymentTypeCardExpirationDate" />
                </div>
            </div>
        </div>

        <div id="divForRoomIn" class="row mb-3" style="display:none;">
            <label for="txtSalesPayOtherPaymentTypeRoomNo" class="col-md-2">Room No: <span style="color: #FF0000;">*</span></label>
            <div class="col-md-10">
                <input data-role="none" class="form-control" type="text" id="txtSalesPayOtherPaymentTypeRoomNo" />
            </div>
        </div>

        <div class="row mb-3 d-none">
            <label for="txtSalesPayOtherPaymentTypeRemarks" class="col-md-2">Remarks:</label>
            <div class="col-md-10">
                <textarea data-role="none" class="form-control" id="txtSalesPayOtherPaymentTypeRemarks" style="height:100px; resize:none;"></textarea>
            </div>
        </div>

        <div id="divForGCash" style="display:none">
            <div class="row mb-3">
                <div class="offset-md-2 col-md-10">
                    <button data-role="none" class="btn btn-secondary btn-block" id="btnGCashPayBal">Auto Fill Payment Balance</button>
                </div>
            </div>
            <div class="row mb-3">
                <label for="txtSalesPayOtherPaymentTypeGCashRefNo" class="col-md-2">GCash Reference Number: <span style="color: #FF0000;">*</span></label>
                <div class="col-md-10">
                    <input data-role="none" class="form-control" type="number" id="txtSalesPayOtherPaymentTypeGCashRefNo" />
                </div>
            </div>
        </div>

        <div class="row mt-5 mb-3">
            <div class="col-md mb-3 order-sm-2" style="text-align: center">
                <a id="btnSalesPayOtherPaymentTypeSave" data-role="none" class="btn btn-block btn-success btn-lg">Ok</a>
            </div>
            <div id="btnSalesPayOtherPaymentTypeRemove" class="col-md mb-3 order-sm-1" style="text-align: center">
                <a data-role="none" class="btn btn-block btn-danger btn-lg">Remove</a>
            </div>
            <div class="col-md mb-3 order-sm-0" style="text-align: center">
                <a id="btnSalesPayOtherPaymentTypeCancel" data-role="none" class="btn btn-block btn-secondary btn-lg">Cancel</a>
            </div>
        </div>
    </div>
</div>

<div id="pageAddNewCustomer" subpage="paymentpage" style="display:none;">
    <nav class="navbar navbar-dark bg-primary sticky-md-top text-center" style="font-size:large; color:#FFF; display:block;">
        Customer Profile
    </nav>
    <div class="container-fluid mt-3">
        <div style="display:none;">
            <input data-role="none" class="form-control" type="text" readonly id="txtID" />
            <input data-role="none" class="form-control" type="text" readonly id="txtCustomerIndex" />
            <input data-role="none" class="form-control" type="text" readonly id="txtSCPWDCustomerID" />
        </div>
        @*
            <div data-role="fieldcontain">
                <label for="btnTitle">Title:</label>
                <div style="display:none;"><input type="text" readonly id="txtTitleID"/></div>
                <button id="btnTitle" data-role="button">Please Select</button>
            </div>*@
        You can enter customer name in fullname or in detailed format.

        <div class="row mb-3 mt-3">
            <label for="txtFullname" class="col-md-2">Full Name: <span style="color: #FF0000;">*</span></label>
            <div class="col-md-10">
                <input data-role="none" class="form-control" type="text" id="txtFullname" maxlength="255" />
            </div>
        </div>

        <div class="row mb-3" style="display:none">
            <label for="txtSurname" class="col-md-2">Surname (Last Name): <span style="color: #FF0000;">*</span></label>
            <div class="col-md-10">
                <input data-role="none" class="form-control" type="text" id="txtSurname" maxlength="255" />
            </div>
        </div>

        <div class="row mb-3" style="display: none">
            <label for="txtFirstName" class="col-md-2">First Name: <span style="color: #FF0000;">*</span></label>
            <div class="col-md-10">
                <input data-role="none" class="form-control" type="text" id="txtFirstName" maxlength="255" />
            </div>
        </div>

        <div class="row mb-3" style="display: none">
            <label for="txtMiddleName" class="col-md-2">Full Middle Name: <span style="color: #FF0000;">*</span></label>
            <div class="col-md-10">
                <input data-role="none" class="form-control" type="text" id="txtMiddleName" maxlength="255" />
            </div>
        </div>

        <div class="row mb-3" style="display: none">
            <label for="selPersonalSuffixName" class="col-md-2">Generational Suffix (Name):</label>
            <div class="col-md-10">
                <select id="selPersonalSuffixName" data-role="none" class="form-select">
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

        <div class="row mb-3">
            <label for="txtCustomerBirthDate" class="col-md-2">Date Of Birth: <span style="color: #FF0000;">*</span></label>
            <div class="col-md-10">
                <input data-role="none" class="form-control" type="date" id="txtSalesPayCustomerBirthDate" />
            </div>
        </div>

        <div class="row mb-3">
            <label for="txtSCPWDTIN" class="col-md-2">TIN (12-digit):</label>
            <div class="col-md-10">
                <input data-role="none" class="form-control" type="text" minlength="15" maxlength="17" id="txtSCPWDTIN" required />
            </div>
        </div>
        
        <div class="accordion mt-3">
            <div class="card">
                <div class="card-header p-0">
                    <button class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none">
                        Identification Number
                    </button>
                </div>
                <div class="collapse show">
                    <div class="card-body">
                        <div class="text-right mb-3">
                            <button data-role="none" id="btnSalesPayCustomerIdentificationAdd" class="btn btn-success"><span class="bi-plus"></span> Add</button>
                        </div>
                        <div id="lstSalesPayCustomerIdentification" class="list-group"></div>
                    </div>
                </div>
            </div>
        </div>
        
        <div id="divSoloParentChildInfo" class="accordion mt-3" style="display:none;">
            <div class="card">
                <div class="card-header p-0">
                    <button class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none">
                        Child Information
                    </button>
                </div>
                <div class="collapse show">
                    <div class="card-body">
                        <div class="row mb-3">
                            <label for="txtChildName" class="col-md-2">Name: <span class="required">*</span></label>
                            <div class="col-md-10">
                                <input data-role="none" class="form-control" type="text" id="txtChildName" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label for="txtChildBirthdate" class="col-md-2">Date of Birth: <span class="required">*</span></label>
                            <div class="col-md-10">
                                <input data-role="none" class="form-control" type="date" id="txtChildBirthdate" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label for="txtChildAge" class="col-md-2">Age: <span class="required">*</span></label>
                            <div class="col-md-10">
                                <input data-role="none" class="form-control" type="number" id="txtChildAge" readonly />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row mt-5 mb-3">
            <div class="col-md mb-3 order-sm-2" style="text-align: center">
                <a id="btnSalesPayCustomerNameSave" data-role="none" class="btn btn-block btn-success btn-lg">Ok</a>
            </div>
            <div id="btnSalesPayCustomerNameRemove" class="col-md mb-3 order-sm-1" style="text-align: center">
                <a data-role="none" class="btn btn-block btn-danger btn-lg">Remove</a>
            </div>
            <div class="col-md mb-3 order-sm-0" style="text-align: center">
                <a id="btnSalesPayCustomerNameCancel" data-role="none" class="btn btn-block btn-secondary btn-lg">Cancel</a>
            </div>
        </div>
    </div>

    <div id="popupAddNewID" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Valid Customer ID</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label for="selIDType" class="form-label">ID Type:</label>
                        <select id="selIDType" data-role="none" class="form-select">
                            <option value="">Please Select</option>
                            <option value="Senior Citizens' ID">Senior Citizens' ID</option>
                            <option value="Persons With Disabilities (PWD) ID">Persons With Disabilities (PWD) ID</option>
                            <option value="Philippine National Sports Team ID">Philippine National Sports Team ID</option> @*temporary lang sa ang Senior Citizens' ID. Philippine National Sports Team ID dapat ang nakabutang dire kay naa pa ginaapas na backend report*@
                            <option value="Solo Parent ID">Solo Parent ID</option>
                            @*<option value="Passport">Passport</option>
                            <option value="Driver's License">Driver's License</option>
                            <option value="SSS ID">SSS ID</option>
                            <option value="GSIS ID">GSIS ID</option>
                            <option value="PRC ID">PRC ID</option>
                            <option value="Postal ID">Postal ID</option>
                            <option value="Voter's ID">Voter's ID</option>*@
                            <option value="Other">Other</option>
                        </select>
                    </div>

                    <div id="divOtherIDType" class="mb-3" style="display:none;">
                        <label for="txtOtherIDType" class="form-label">Other ID Type:</label>
                        <input data-role="none" class="form-control" type="text" id="txtOtherIDType" maxlength="50" placeholder="Enter Other ID Type" />
                    </div>

                    <div class="mb-3">
                        <label for="txtIDText" class="form-label">ID Number:</label>
                        <input data-role="none" class="form-control" type="text" id="txtIDText" maxlength="20" placeholder="Enter a valid identification number" />
                    </div>
                    
                </div>
                <div class="modal-footer">
                    <button data-role="none" id="btnSalesPayCustomerIdentificationSave" class="btn btn-success btn-block">Ok</button>
                    <button data-role="none" id="btnSalesPayCustomerIdentificationRemove" class="btn btn-danger btn-block">Remove</button>
                    <button data-role="none" id="btnSalesPayCustomerIdentificationCancel" data-dismiss="modal" class="btn btn-secondary btn-block">Cancel</button>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="divSalesPaymentPromptEmail" subpage="paymentpage" style="display:none;">
    <nav class="navbar navbar-dark bg-primary sticky-md-top text-center" style="font-size:large; color:#FFF; display:block;">
        Email Address
    </nav>

    <div class="container-fluid mt-3">
        <div class="row mb-3">
            <label class="col-md-2 mb-3 col-form-label" for="txtSalesPayEmailAddress">
                Enter email address:
            </label>
            <div class="col-md-10">
                <input data-role="none" type="text" id="txtSalesPayEmailAddress" class="form-control form-control-lg mb-3" placeholder="Enter other email addresses here" />
                (Please separate multiple email addresses with a comma. Ex. sample1@email.com, sample2@email.com)
            </div>
        </div>

        <div class="text-center mt-4">
            <button data-role="none" id="btnSalesPaymentEmailCancel" class="btn btn-secondary btn-lg">Cancel</button>
            <button data-role="none" id="btnSalesPaymentEmailOK" class="btn btn-success btn-lg">Send</button>
        </div>
    </div>
</div>