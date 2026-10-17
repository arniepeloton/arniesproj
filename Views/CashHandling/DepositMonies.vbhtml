@Code
    ViewData("Title") = "Turnover of Monies"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code

@Section head

    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v='], function () {
                require([GetPathFormat('/Scripts/sources/CashHandling/CashHandling.js?crc='),
                    GetPathFormat('/Scripts/sources/SignIn/SignInInfo.js?crc=')
                ]);
            })
        })        
    </script>

End Section

@*Header*@

<div id="pageMain">
    <nav class="navbar navbar-dark sticky-md-top bg-primary" style="padding:0px !important;">
        <div class="row" style="margin:0px !important;">
            <div class="col-4" style="padding:0px !important;">
                <button data-role="none" id="btnGoBackToCashHandling" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                    <span id="iconHome" class="bi-chevron-left"></span>
                    <div id="lblHome" class="menu-description mt-1">Back</div>
                </button>
            </div>
            <div class="col-4 offset-4" style="padding:0px !important;">
                <button data-role="none" id="btnSaveDepositMonies" style="height:100%;" class="btn btn-success btn-block btn-lg rounded-0">
                    <span id="iconPay" class="bi-eye"></span>
                    <div class="menu-description mt-1">View Summary</div>
                </button>
            </div>
        </div>
        <br />
        <div class="row" style="margin:0px; padding:0.5em; width:100%; color:#FFF; background-color:#1969de; font-weight:bold;">
            <div class="col-12" style="text-overflow:ellipsis; overflow:hidden; text-align:right; white-space:nowrap;">
                <span id="clockTurnover">&nbsp;</span>
            </div>
        </div>
    </nav>

    <div class="container-fluid">

        <table id="tblPOSSalesReport" class="mb-3 mt-3" align="center">
            <tr>
                <td colspan="2" style="text-align:center">
                    <label id="lblOrganizationBusinessName2" style="font-size:xx-large; font-weight:bolder">&nbsp;</label> <br />
                    <label id="lblOrganizationName2">&nbsp;</label> <br />
                    <label id="lblOrganizationFOWST2">&nbsp;</label> <br />
                    <label id="lblOrganizationFOWSTAddress2">&nbsp;</label>  <br />
                    <label id="lblOrganizationFOWSTTIN2">&nbsp;</label>  <br />
                    <label id="lblOrganizationFOWSTContactNumber2">&nbsp;</label>
                </td>
            </tr>
        </table>

        <div class="accordion mt-3">
            <div class="card">
                <div class="card-header p-0">
                    <button class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse" data-target="#collapseCash">
                        <b>Cash</b>
                    </button>
                </div>
                <div id="collapseCash" class="collapse show">
                    <div class="card-body">
                        <div class="row mb-3">
                            <div class="col-md-2 col-form-label">
                                <div class="form-check">
                                    <label class="form-check-label">
                                        <input data-role="none" type="radio" class="form-check-input" name="radio-choice-0" id="rdTotalAmount" checked="checked">
                                        Total Amount of Cash:
                                    </label>
                                </div>
                            </div>
                            <div class="col-md-10">
                                <input data-role="none" type="number" class="amtturnover form-control" id="txtTotalAmount" />
                            </div>
                        </div>

                        <div class="form-check mb-3">
                            <label class="form-check-label">
                                <input data-role="none" type="radio" class="form-check-input" name="radio-choice-0" id="rdDetailedBreakdown">
                                Detailed Breakdown of Cash Amount:
                            </label>
                        </div>

                        <div id="dvDetailedBreakdown" class="table-responsive" style="display:none; visibility:hidden">
                            <table class="table table-light table-striped headerfitwidth" style="width:100%; text-align:center">
                                <thead>
                                    <tr style="font-weight:bold; font-size:large">
                                        <td> Denomination </td>
                                        <td> Qty </td>
                                        <td> Sub Total </td>
                                    </tr>
                                </thead>

                                <tbody>
                                    <tr>
                                        <td>  PhP 0.01 </td>
                                        <td>  <input class="form-control amtturnover" data-role="none" type="number" id="txtNoOfOneCents" /> </td>
                                        <td id="lblNoOfOneCents">  - </td>
                                    </tr>

                                    <tr>
                                        <td>  PhP 0.05 </td>
                                        <td>  <input class="form-control amtturnover" data-role="none" type="number" id="txtNoOfFiveCents" /> </td>
                                        <td id="lblNoOfFiveCents">  - </td>
                                    </tr>

                                    <tr>
                                        <td> PhP 0.10 </td>
                                        <td> <input class="form-control amtturnover" data-role="none" type="number" id="txtNoOfTenCents" /> </td>
                                        <td id="lblNoOfTenCents"> - </td>
                                    </tr>

                                    <tr>
                                        <td> PhP 0.25 </td>
                                        <td> <input class="form-control amtturnover" data-role="none" type="number" id="txtNoOfTwentyFiveCents" /> </td>
                                        <td id="lblNoOfTwentyFiveCents"> - </td>
                                    </tr>

                                    <tr>
                                        <td> PhP 1.00 </td>
                                        <td> <input class="form-control amtturnover" data-role="none" type="number" id="txtNoOfOnePeso" /> </td>
                                        <td id="lblNoOfOnePeso"> - </td>
                                    </tr>

                                    <tr>
                                        <td> PhP 5.00 </td>
                                        <td> <input class="form-control amtturnover" data-role="none" type="number" id="txtNoOfFivePesos" /> </td>
                                        <td id="lblNoOfFivePesos"> - </td>
                                    </tr>

                                    <tr>
                                        <td> PhP 10.00 </td>
                                        <td> <input class="form-control amtturnover" data-role="none" type="number" id="txtNoOfTenPesos" /> </td>
                                        <td id="lblNoOfTenPesos"> - </td>
                                    </tr>

                                    <tr>
                                        <td> PhP 20.00 </td>
                                        <td> <input class="form-control amtturnover" data-role="none" type="number" id="txtNoOfTwentyPesos" /> </td>
                                        <td id="lblNoOfTwentyPesos"> - </td>
                                    </tr>

                                    <tr>
                                        <td> PhP 50.00 </td>
                                        <td> <input class="form-control amtturnover" data-role="none" type="number" id="txtNoOfFiftyPesos" /> </td>
                                        <td id="lblNoOfFiftyPesos"> - </td>
                                    </tr>

                                    <tr>
                                        <td> PhP 100.00 </td>
                                        <td> <input class="form-control amtturnover" data-role="none" type="number" id="txtNoOfOneHundredPesos" /> </td>
                                        <td id="lblNoOfOneHundredPesos"> - </td>
                                    </tr>

                                    <tr>
                                        <td> PhP 200.00 </td>
                                        <td> <input class="form-control amtturnover" data-role="none" type="number" id="txtNoOfTwoHundredPesos" /> </td>
                                        <td id="lblNoOfTwoHundredPesos"> - </td>
                                    </tr>

                                    <tr>
                                        <td> PhP 500.00 </td>
                                        <td> <input class="form-control amtturnover" data-role="none" type="number" id="txtNoOfFiveHundredPesos" /> </td>
                                        <td id="lblNoOfFiveHundredPesos"> - </td>
                                    </tr>

                                    <tr>
                                        <td> PhP 1,000.00 </td>
                                        <td> <input class="form-control amtturnover" data-role="none" type="number" id="txtNoOfOneThousandPesos" /> </td>
                                        <td id="lblNoOfOneThousandPesos"> - </td>
                                    </tr>

                                    <tr>
                                        <td> </td>
                                        <td style="font-weight:bold; font-size:large"> Total Amount: </td>
                                        <td style="font-weight:bold; font-size:large">  <label id="lblTotamAmount"> </label> </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div style="visibility:hidden; display:none" data-role="collapsible" data-collapsed="false" data-theme="c" data-content-theme="c" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d">
            <h4>Employee Shift Work</h4>

            <div data-role="fieldcontain">
                <b>Employee Shift From :</b>
                <div data-role="fieldcontain">
                    <label for="txtDateFrom">Date From :</label>
                    <fieldset id="txtDateFrom" data-role="DateCombo" data-timecombo="txtTimeFrom"></fieldset>
                </div>
                <div data-role="fieldcontain">
                    <label for="txtTimeFrom">Time From :</label>
                    <fieldset id="txtTimeFrom" data-role="TimeCombo" data-seconds="false"></fieldset>
                </div>
            </div>

            <br /> <br /> 
            <div>
                <b>Employee Shift To : </b>
                <div data-role="fieldcontain">
                    <label for="txtDateTo">Date To :</label>
                    <fieldset id="txtDateTo" data-role="DateCombo" data-timecombo="txtTimeTo"></fieldset>
                </div>
                <div data-role="fieldcontain">
                    <label for="txtTimeTo">Time To:</label>
                    <fieldset id="txtTimeTo" data-role="TimeCombo" data-seconds="false"></fieldset>
                </div>
            </div>                    
        </div>

        <div class="accordion mt-3">
            <div class="card">
                <div class="card-header p-0">
                    <button class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse" data-target="#collapseCheck">
                        <b>Checks</b>
                    </button>
                </div>
                <div id="collapseCheck" class="collapse">
                    <div class="card-body">
                        <div class="row mb-3">
                            <label for="txtTotalCheck" class="col-md-2">
                                Total Amount of Checks to Turnover:
                            </label>
                            <div class="col-md-10">
                                <input data-role="none" type="number" class="amtturnover form-control" id="txtTotalCheck" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="accordion mt-3">
            <div class="card">
                <div class="card-header p-0">
                    <button class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse" data-target="#collapseCreditCard">
                        <b>Credit Card</b>
                    </button>
                </div>
                <div id="collapseCreditCard" class="collapse">
                    <div class="card-body">
                        <div class="row mb-3">
                            <label for="txtTotalCreditCard" class="col-md-2">
                                Total Amount of Credit Card Slips to Turnover:
                            </label>
                            <div class="col-md-10">
                                <input data-role="none" type="number" class="amtturnover form-control" id="txtTotalCreditCard" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="accordion mt-3">
            <div class="card">
                <div class="card-header p-0">
                    <button class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse" data-target="#collapseDeditCard">
                        <b>Debit Card</b>
                    </button>
                </div>
                <div id="collapseDeditCard" class="collapse">
                    <div class="card-body">
                        <div class="row mb-3">
                            <label for="txtTotalDebitCard" class="col-md-2">
                                Total Amount of Debit Card Slips to Turnover:
                            </label>
                            <div class="col-md-10">
                                <input data-role="none" type="number" class="amtturnover form-control" id="txtTotalDebitCard" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="accordion mt-3">
            <div class="card">
                <div class="card-header p-0">
                    <button class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse" data-target="#collapseSRV">
                        <b>SRV</b>
                    </button>
                </div>
                <div id="collapseSRV" class="collapse">
                    <div class="card-body">
                        <div class="row mb-3">
                            <label for="txtTotalSRV" class="col-md-2">
                                Total Amount of SRV to Turnover:
                            </label>
                            <div class="col-md-10">
                                <input data-role="none" type="number" class="amtturnover form-control" id="txtTotalSRV" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="accordion mt-3">
            <div class="card">
                <div class="card-header p-0">
                    <button class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse" data-target="#collapseOP">
                        <b>Other Payment</b>
                    </button>
                </div>
                <div id="collapseOP" class="collapse">
                    <div class="card-body">
                        <div class="mb-3 text-right">
                            <button id="btnAddOtherPayment" data-role="none" class="btn btn-success">Add Other Payment</button>
                        </div>

                        <div id="divOtherPayment"></div>
                    </div>
                </div>
            </div>
        </div>

        <div class="accordion mt-3" style="display:none;">
            <div class="card">
                <div class="card-header p-0">
                    <button class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse" data-target="#collapsePettyCash">
                        <b>Petty Cash</b>
                    </button>
                </div>
                <div id="collapsePettyCash" class="collapse">
                    <div class="card-body">
                        <div class="row mb-3">
                            <label for="txtTotalPettyCash" class="col-md-2">
                                Total Amount of Petty Cash Fund to Turnover:
                            </label>
                            <div class="col-md-10">
                                <input data-role="none" type="number" class="amtturnover form-control" id="txtTotalPettyCash" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="mt-3 mb-5">
            <h4 class="mb-3">More Info</h4>

            <div class="form-check mb-3">
                <label class="form-check-label" for="rdTurnOver">Turn Over</label>
                <input class="form-check-input" data-role="none" type="radio" name="radTurnOverOfMonies" id="rdTurnOver" value="1" checked>
            </div>

            <div class="form-check mb-3">
                <label class="form-check-label" for="rdDeposit">Deposit</label>
                <input class="form-check-input" data-role="none" type="radio" name="radTurnOverOfMonies" id="rdDeposit" value="0">
            </div>

            @*<div class="form-check mb-3">
                <label class="form-check-label" for="rdCashCount">Cash Count Only</label>
                <input class="form-check-input" data-role="none" type="radio" name="radTurnOverOfMonies" id="rdCashCount" value="2">
            </div>*@

            <div class="row mb-3">
                <label class="col-md-2 col-form-label">From:</label>
                <div class="col-md-10">
                    <input class="form-control" data-role="none" type="text" id="txtDepositBy" readonly />
                </div>
            </div>

            <div class="row mb-3">
                <label class="col-md-2 col-form-label">To: <span class="required">*</span></label>
                <div class="col-md-10">
                    <span id="txtToValidation" class="mb-3" style="color:#F00; display:none;">This field is required</span>
                    <select id="optDepositTo" data-role="none" class="form-select">
                        <option>-Select-</option>
                        <option>My Company Collector</option>
                        <option>My Company Manager</option>
                        <option>My Company Cashier</option>
                    </select>
                </div>
            </div>

            <div style="display:none" id="dvDepositTo">
                <div class="row mb-3">
                    <label id="lblDepositTo" for="txtDepositTo" class="col-md-2 col-form-label"></label>
                    <div class="col-md-10">
                        <input class="form-control" data-role="none" type="search" id="txtDepositTo" />
                    </div>
                </div>
            </div>            

            <div style="display:none" id="dvFreeType">
                <div class="row mb-3">
                    <label for="txtFreeType" class="col-md-2 col-form-label"><span id="lblFreeType"></span> <span class="required">*</span></label>
                    <div class="col-md-10">
                        <input class="form-control" data-role="none" type="text" id="txtFreeType" />
                    </div>
                </div>
            </div>            

            <div style="display:none" id="divForBank">
                <div class="row mb-3">
                    <label for="txtBankBranch" class="col-md-2 col-form-label">Branch Name:</label>
                    <div class="col-md-10">
                        <input class="form-control" data-role="none" type="text" id="txtBankBranch" />
                    </div>
                </div>

                <div class="row mb-3">
                    <label for="txtDepositRefNo" class="col-md-2 col-form-label">Deposit Reference No.:</label>
                    <div class="col-md-10">
                        <input class="form-control" data-role="none" type="text" id="txtDepositRefNo" />
                    </div>
                </div>

                <div class="row mb-3">
                    <label for="txtServiceCharge" class="col-md-2 col-form-label">Service Charge:</label>
                    <div class="col-md-10">
                        <input class="form-control" data-role="none" type="number" id="txtServiceCharge" />
                    </div>
                </div>
            </div>            
        </div>

        <div class="text-center mt-5 mb-4 d-none">
            <button id="btnGoBackToCashHandling" data-role="none" class="btn btn-secondary btn-lg">Back</button>
        </div>
    </div>
</div>

<div id="popupOtherPayment" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Add Other Payment</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    <label class="form-label">Select Other Payment:</label>
                    <select id="selOtherPayment" data-role="none" class="form-select">
                        @*<option value="">Please Select</option>
                        <option value="Alipay">Alipay</option>
                        <option value="Complimentary">Complimentary</option>
                        <option value="DiscountVoucher">Discount Voucher / On Line Deals</option>
                        <option value="EPlus">E-Plus</option>
                        <option value="FoodPanda">Food Panda</option>
                        <option value="GCash">GCash</option>
                        <option value="GoldCard">Gold Card</option>
                        <option value="GrabFood">Grab Food</option>
                        <option value="LocalDelivery">Local Delivery</option>
                        <option value="PayMaya">PayMaya</option>
                        <option value="ShopeeEVoucher">Shopee E-Voucher</option>
                        <option value="ShopeePay">Shopee Pay</option>
                        <option value="SMAC">SMAC</option>
                        <option value="VPlus">VPlus</option>*@
                    </select>
                </div>

                <div class="mb-3">
                    <label class="form-label">Amount:</label>
                    <input class="form-control" data-role="none" type="number" id="txtOPTotalAmount" />
                </div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col"><button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button></div>
                    <div class="col"><button data-role="none" type="button" id="btnSaveOP" class="btn btn-success btn-block">Add</button></div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="DialogUserAccess" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Confirmation Dialog</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div class="mb-3">Hello <b id="lblReceiverName"></b>! Please enter your username and password to confirm that you received the money.</div>

                <div class="alert alert-danger mb-3 mt-3" role="alert" id="dvInvalidUsernameOrPassword" style="display:none;text-align:center">
                    Invalid username and password
                </div>

                <div class="mb-3">
                    <label class="form-label" for="txtUsername">Username:</label>
                    <input class="form-control" data-role="none" type="text" id="txtUsername" />
                </div>

                <div class="mb-3">
                    <label class="form-label" for="txtPassword">Password:</label>
                    <input class="form-control" data-role="none" type="password" id="txtPassword" />
                </div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col"><button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button></div>
                    <div class="col"><button data-role="none" type="button" id="btnVerifyEmployee" class="btn btn-success btn-block">Confirm Now</button></div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popupConfirmationDialog" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Summary Details</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div class="mb-3 font-italic alert alert-danger">
                    NOTE: Please double check the details below before saving.
                </div>

                <div class="row align-items-center mb-3 font-weight-bold">
                    <div class="col">Turned over amount:</div>
                    <div id="lblTotalTurnedoverAmount" class="col text-right"></div>
                </div>

                <div class="mb-3 text-center">***** BREAKDOWN *****</div>

                <div id="divBreakdown" class="mb-3"></div>

                <div class="row mb-3 align-items-center">
                    <div class="col-2">
                        From:
                    </div>
                    <div class="col font-weight-bold" id="lblFrom"></div>
                </div>

                <div class="row mb-3 align-items-center">
                    <div class="col-2">
                        To:
                    </div>
                    <div class="col font-weight-bold" id="lblTo"></div>
                </div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col"><button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button></div>
                    <div class="col"><button data-role="none" type="button" id="btnSaveNow" class="btn btn-success btn-block">Save</button></div>
                </div>
            </div>
        </div>
    </div>
</div>