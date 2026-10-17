<div id="popFMCGChequeGC-bs" data-backdrop="static" class="modal fade" tabindex="-1" style="display:none;">
    <div class="modal-dialog modal-lg modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Cheque / Gift Certificate / Online Payment</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body" id="popFMCGChequeGCcontent-bs">

            </div>
        </div>
    </div>
</div>

<div data-role="popup" id="popFMCGChequeGC" data-dismissible="true" data-overlay-theme="a" data-theme="a" data-corners="false">
    <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
    <center><h3 style="margin-left:10px; margin-right:10px;">Cheque / Gift Certificate</h3></center>
    <div data-role="content" id="popFMCGChequeGCcontent" class="ui-content ui-body-d">
        <div subpage="pageCGC" id="pageListOfCGC" class="jPopupContent" style="display:block;">
            <div class="divTable mb-3">
                <div class="divTableRow">
                    <div class="divTableCell">
                        Press ESC to close
                    </div>
                    <div class="divTableCell" style="text-align:right">
                        <button data-role="none" id="btnAddCGC" class="btn btn-less-width custom-btn-theme-c btn-success">Add (press +)</button>
                    </div>
                </div>
            </div>

            <ul id="listCGC" class="custom-list custom-list-plain">
                <li class="list-group-item">List is empty</li>
            </ul>

            <br />
            <a data-role="none" href="#" data-rel="back" class="btn custom-btn-theme-c btn-secondary btn-block btn-lg" data-dismiss="modal">Close</a>
        </div>

        <div subpage="pageCGC" id="pageNewCGG" style="display:none;">
            <span style="float:right;">Press (-) to go back</span>
            <br /><br />
            <div class="divTable">
                <div class="divTableRow">
                    <div class="divTableCell">
                        <div class="form-check">
                            <input type="radio" class="form-check-input" name="radChequeOrGiftCert" value="cheque" id="radChequeOrGiftCert1" checked style="margin-right:7px" />
                            <label class="form-check-label" for="radChequeOrGiftCert1">This is a cheque</label>
                        </div>                        
                    </div>
                    <div class="divTableCell">
                        <div class="form-check">
                            <input type="radio" class="form-check-input" name="radChequeOrGiftCert" value="gift-cert" id="radChequeOrGiftCert2" style="margin-right:7px" />
                            <label class="form-check-label" for="radChequeOrGiftCert2">This is a gift certificate</label>
                        </div>                        
                    </div>
                    <div class="divTableCell">
                        <div class="form-check">
                            <input type="radio" class="form-check-input" name="radChequeOrGiftCert" value="online-payment" id="radChequeOrGiftCert3" style="margin-right:7px" />
                            <label class="form-check-label" for="radChequeOrGiftCert3">This is an online payment</label>
                        </div>
                    </div>
                </div>
            </div>
            <br />
            <div id="divChequeSection">
            <div id="" class="divTable">
                <ul id="tabAccountDetailsFromTo" class="nav nav-tabs" role="tablist" style="flex-wrap:nowrap;">
                    <li class="nav-item" role="presentation">
                        <button name="tabAccountDetailsFromToButton" data-role="none" class="nav-link active" data-toggle="tab">From</button>
                    </li>
                    <li class="nav-item" role="presentation">
                        <button name="tabAccountDetailsFromToButton" data-role="none" class="nav-link" data-toggle="tab">To</button>
                    </li>
                </ul>
                <div class="divTableRow">
                    <div class="divTableCell">
                        <button data-role="none" name="butAddCheckingAccount" class="btn custom-btn-theme-c btn-secondary btn-block">Add Bank Account</button>
                    </div>
                    <div class="divTableCell">
                        <button data-role="none" name="butDeactivateCheckingAccount" class="btn custom-btn-theme-c btn-secondary btn-block">Deactivate Bank Account</button>
                    </div>
                </div>
            </div>
            <br />
            <div>
                <table id="tableFinancialDetails" data-role="table" data-mode="reflow" style="display:table; width:100%" class="ui-table ui-shadow ui-responsive table-stroke table-stripe table table-light table-hover">
                    <thead>
                        <tr class="custom-theme-f txt-center">
                            <th></th>
                            <th style="width: 100px;">Bank</th>
                            <th style="width: 200px;">Bank Branch</th>
                            <th style="width: 200px;">Account Name</th>
                            <th style="width: 200px;">Account Number</th>
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>
            </div>
                <br /><br />
                <div class="row">
                    <div class="col-sm-3">Bank Account No:</div>
                    <div class="col-sm-9"><input type="text" readonly name="txtCGCAccountNoAlphaNumeric" data-role="none" class="form-control" /></div>
                </div>
                <br />
            </div>
            <form id="formFMCGChequeGC" method="post">
                <span id="txtChequeGCMsg" style="color:#ff0000; display:none;">Fields with (<span style="color:#F00">*</span>) are required.</span>
                <div style="display:none">                    
                    <input type="text" name="txtCGCID" data-role="none" class="form-control" style="display:none" hidden />
                    <div class="custom-ui-label">
                        <div>Bank / Issuing Organization: <span class="required">*</span></div>
                        <div>
                            <button data-role="none" name="butCGCBank" class="btn custom-btn-theme-c">Search</button>
                            @*<select name="selCGCBank" data-role="none" class="form-control">
        <option value="">Please Select</option>
    </select>*@
                        </div>
                    </div>

                    <input type="text" name="txtCGCBranchID" data-role="none" class="form-control" style="display:none" hidden />
                    <div class="custom-ui-label">
                        <div>Branch:</div>
                        <div><input type="text" name="txtCGCBranch" data-role="none" class="form-control" /></div>
                    </div>

                    <div class="custom-ui-label">
                        <div>Acc. Name: (Issued To)</div>
                        <div><input type="text" name="txtCGCAccountName" data-role="none" class="form-control" /></div>
                    </div>
                    <div class="row">
                        <div class="col-sm-3">Acc. No:</div>
                        <div class="col-sm-9"><input type="text" readonly name="txtCGCAccountNo" data-role="none" class="form-control" /></div>
                    </div>
                </div>
                
                <div class="row">
                    <div class="col-sm-3">Cheque/GC/Online Transaction No: <span class="required">*</span></div>
                    <div class="col-sm-9"><input type="text" name="txtCGCChequeGC" data-role="none" class="form-control" /></div>
                </div>
                <br />
                <div class="row">
                    <div class="col-sm-3">Cheque/Online Transaction Date: <span class="required">*</span></div>
                    <div class="col-sm-9">
                        <button data-role="none" id="btnChequeDate" class="btn custom-btn-theme-c btn-secondary btn-block">Set Date</button>
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-sm-3">Amount: <span class="required">*</span></div>
                    <div class="col-sm-9"><input type="number" name="txtCGCValue" data-role="none" class="form-control number-only" /></div>
                </div>
                <br /><br />
                <div class="divTable">
                    <div class="divTableRow">
                        <div class="divTableCell">
                            <button id="btnCGCBack" class="btn btn-secondary btn-block btn-lg">Back</button>
                        </div>
                        <div class="divTableCell">
                            <input type="submit" id="btnSubmitCGC" data-role="button" class="btn btn-success btn-block btn-lg" value="Ok" />
                        </div>
                    </div>
                </div>
            </form>
        </div>
    </div>
</div>

<div id="popFMCGDiscount-bs" data-backdrop="static" class="modal fade" tabindex="-1" style="display:none;">
    <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Discount</h5>
                @*<button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>*@
            </div>
            <div class="modal-body" id="popFMCGDiscountcontent-bs">

                <input type="text" name="txtDiscountPercentCeiling" data-role="none" class="form-control" style="display:none;" />
                <div class="custom-ui-label">
                    <div>Discount Type</div>
                    <div>
                        <select name="selDiscountType" data-role="none" class="form-select">
                            <option value="">Please Select</option>
                        </select>
                    </div>
                </div>
                <br />
                <div class="custom-ui-label">
                    <div>Percent Discount</div>
                    <div><input type="text" name="txtDiscountPercent" data-role="none" class="form-control" /></div>
                </div>
                <br />
                <div class="custom-ui-label">
                    <div>Discount Amount</div>
                    <div><input type="text" name="txtDiscountAmount" data-role="none" class="form-control" /></div>
                </div>
                <br />
                <div class="custom-ui-label">
                    <div>Remarks</div>
                    <div><input type="text" name="txtDiscountRemarks" data-role="none" class="form-control" /></div>
                </div>

                <div id="divDiscountCustomerDetails" style="display:none">
                    <br />
                    <div class="custom-ui-label">
                        <div>No. of Senior Citizens / PWD: <span style="color:#ff0000">*</span></div>
                        <div><input type="text" name="txtSalesPayNoOfSC" data-role="none" class="form-control" /></div>
                    </div>
                    <br />
                    <div class="custom-ui-label">
                        <div>No. of People (including the sr. citizen / PWD): <span style="color:#ff0000">*</span></div>
                        <div><input type="text" name="txtSalesPayNoOfDiners" data-role="none" class="form-control" /></div>
                    </div>
                    <br />
                    <div class="mb-3 font-weight-bold">
                        List of Sr. Citizens / PWD availing the discount
                    </div>
                    <div class="input-group mb-3">
                        <input type="text" data-role="none" class="form-control" name="txtCustomerSearch" placeholder="Search Existing Customer Name or ID Number" />
                        <button data-role="none" name="btnAddDiscountCustomer" class="btn btn-secondary">Add New</button>
                    </div>
                    <ul name="listDiscountCustomer" class="list-group">
                        <li class="list-group-item">List is empty</li>
                    </ul>
                </div>
            </div>
            <div class="modal-footer">
                <button data-role="none" name="btnDiscountOk" type="submit" class="btn btn-secondary btn-block btn-lg">
                    Close
                </button>
            </div>
        </div>
    </div>    
</div>

<div id="popFMCGDiscountCustomerDetails" data-backdrop="static" class="modal fade" tabindex="-1" style="display:none;">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Customer Details</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <input data-role="none" type="text" name="txtCustomerPersonID" style="display:none;" />
                <div class="mb-3">
                    <label class="form-label">Full Name: <span class="required">*</span></label>
                    <input data-role="none" type="text" class="form-control" name="txtCustomerFullname" />
                </div>

                <div class="mb-3">
                    <label class="form-label">Date of Birth: <span class="required">*</span></label>
                    <input data-role="none" type="date" class="form-control" name="txtCustomerBirthday" />
                </div>

                <div class="mb-3">
                    <div class="row mb-3 font-weight-bold align-items-center">
                        <div class="col">Identification Number</div>
                        <div class="col-auto"><button data-role="none" name="btnAddDiscountCustomerID" class="btn btn-secondary">Add ID</button></div>
                    </div>
                    <ul name="listDiscountCustomerID" class="list-group">
                        <li class="list-group-item">List is empty</li>
                    </ul>
                </div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col">
                        <a data-role="none" href="#" data-rel="back" data-dismiss="modal" class="btn custom-btn-theme-b btn-secondary btn-block btn-lg">
                            Cancel
                        </a>
                    </div>
                    <div class="col">
                        <button data-role="none" name="btnOkDiscountCustomer" class="btn custom-btn-theme-b btn-success btn-block btn-lg">
                            Ok
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popFMCGDiscountCustomerDetailsID" data-backdrop="static" class="modal fade" tabindex="-1" style="display:none;">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Valid ID Details</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    <label class="form-label">ID: <span class="required">*</span></label>
                    <select data-role="none" class="form-select" name="selIDType">
                        <option value="">Please Select</option>
                        <option value="Senior Citizens' ID">Senior Citizens' ID</option>
                        <option value="Persons With Disabilities (PWD) ID">Persons With Disabilities (PWD) ID</option>
                        <option value="Passport">Passport</option>
                        <option value="Driver's License">Driver's License</option>
                        <option value="SSS ID">SSS ID</option>
                        <option value="GSIS ID">GSIS ID</option>
                        <option value="PRC ID">PRC ID</option>
                        <option value="Postal ID">Postal ID</option>
                        <option value="Voter's ID">Voter's ID</option>
                        <option value="Other">Other</option>
                    </select>
                </div>

                <div id="divOtherTypeID" class="mb-3" style="display:none;">
                    <label class="form-label">Other ID: <span class="required">*</span></label>
                    <input type="text" data-role="none" class="form-control" name="txtOtherIDType" />
                </div>

                <div class="mb-3">
                    <label class="form-label">Valid ID No.: <span class="required">*</span></label>
                    <input type="text" data-role="none" class="form-control" name="txtValidIDNo" />
                </div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col">
                        <a data-role="none" href="#" data-rel="back" data-dismiss="modal" class="btn custom-btn-theme-b btn-secondary btn-block btn-lg">
                            Cancel
                        </a>
                    </div>
                    <div class="col">
                        <button data-role="none" name="btnOkValidID" class="btn custom-btn-theme-b btn-success btn-block btn-lg">
                            Ok
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popFMCGSrv-bs" data-backdrop="static" class="modal fade" tabindex="-1" style="display:none;">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Sales Return Voucher (SRV)</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body" id="popFMCGSrvcontent-bs">

            </div>
        </div>
    </div>
</div>

<div data-role="popup" id="popFMCGSrv" data-dismissible="true" data-overlay-theme="a" data-theme="a" data-corners="false">
    <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
    <center><h3 style="margin-left:10px; margin-right:10px;">Sales Return Voucher (SRV)</h3></center>
    <div data-role="content" id="popFMCGSrvcontent" class="ui-content ui-body-d jPopupContent">
        <div subpage="pageSrv" id="pageListOfSrv" style="display:block;">
            <div class="divTable">
                <div class="divTableRow">
                    <div class="divTableCell">
                        Press ESC to close
                    </div>
                    <div class="divTableCell" style="text-align:right">
                        <button data-role="none" id="btnAddSrv" class="btn btn-less-width custom-btn-theme-c btn-success">Add (press +)</button>
                    </div>
                </div>
            </div>
            <br />
            <ul id="listSrv" class="custom-list custom-list-plain">
                <li>List is empty</li>
            </ul>

            <br />
            <a data-role="none" href="#" data-rel="back" data-dismiss="modal" class="btn custom-btn-theme-c btn-secondary btn-block btn-lg">Close</a>
        </div>

        <div subpage="pageSrv" id="pageNewSrv" style="display:none;">
            <span style="float:right;">Press (-) to go back</span>
            <br /><br />
            <form id="formFMCGSrv" method="post">
                <div subpage="pageMsgSrv" id="txtSrvMsgRequired" style="color:#ff0000; display:none;">Fields with (<span style="color:#F00">*</span>) are required.</div>
                <div subpage="pageMsgSrv" id="txtSrvMsgNotFound" style="color:#ff0000; display:none;">SRV number not found.</div>
                <div subpage="pageMsgSrv" id="txtSrvMsgRequiredInterent" style="color:#ff0000; display:none;">This feature requires internet connection.</div>
                <div subpage="pageMsgSrv" id="txtSrvMsgBarcode" style="color:#ff0000; display:none;">Scan Barcode / QR Code feature is only available on the mobile app</div>
                <input type="text" data-role="none" class="form-control" name="txtSrvID" style="display:none" hidden />
                <input type="text" data-role="none" class="form-control" name="txtSrvTransMemoID" style="display:none" hidden />
                <div class="custom-ui-label">
                    <div>SRV No: <span style="color:#F00">*</span></div>
                    <div>
                        <div class="divTable">
                            <div class="divTableRow">
                                <div class="divTableCell">
                                    <input type="number" data-role="none" class="form-control number-only" name="txtSrvNo" />
                                </div>
                                <div class="divTableCell txt-top" style="width:65px;">
                                    <button id="btnScanSrvNo" data-role="none" class="btn custom-btn-theme-c btn-less-width btn-secondary">Scan</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <br />
                <div class="custom-ui-label">
                    <div>PIN Code: <span style="color:#F00">*</span></div>
                    <div>
                        <div class="divTable">
                            <div class="divTableRow">
                                <div class="divTableCell">
                                    <input type="password" data-role="none" class="form-control" name="txtSrvPinCode" />
                                </div>
                                <div class="divTableCell txt-top" style="width:65px;">
                                    <button id="btnScanPinCode" data-role="none" class="btn custom-btn-theme-c btn-less-width btn-secondary">Scan</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <br />
                <div class="custom-ui-label">
                    <div></div>
                    <div>
                        <button id="btnVerifySrv" data-role="none" class="btn custom-btn-theme-c btn-secondary btn-block">Verify SRV</button>
                    </div>
                </div>
                <br />
                <div class="custom-ui-label">
                    <div>SRV Amount Balance:</div>
                    <div><input type="text" data-role="none" class="form-control" name="txtMainSrvAmount" readonly /></div>
                </div>
                <br />
                <div class="custom-ui-label">
                    <div>SRV Payment Amount:</div>
                    <div><input type="text" data-role="none" class="form-control" name="txtSrvAmount" readonly /></div>
                </div>
                <br /><br />
                <div class="divTable">
                    <div class="divTableRow">
                        <div class="divTableCell">
                            <button id="btnSrvBack" class="btn custom-btn-theme-c btn-secondary btn-block btn-lg">Back</button>
                        </div>
                        <div class="divTableCell">
                            <input type="submit" id="btnSubmitSrv" data-role="none" class="btn custom-btn-theme-b btn-success btn-block btn-lg" value="Ok" />
                        </div>
                    </div>
                </div>
            </form>
        </div>
    </div>
</div>

<div id="popFMCGOtherPayment-bs" data-backdrop="static" class="modal fade" tabindex="-1" style="display:none;">
    <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Other Payment</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body" id="popFMCGOtherPaymentcontent-bs">
                <div class="divTable">
                    <div class="divTableRow">
                        <div class="divTableCell">
                            Press ESC to close
                        </div>
                        <div class="divTableCell" style="text-align:right">
                            <button data-role="none" id="btnAddOP" class="btn btn-less-width custom-btn-theme-c btn-success">Add (press +)</button>
                        </div>
                    </div>
                </div>
                <br />
                <ul id="listOP" class="custom-list custom-list-plain">
                    <li>List is empty</li>
                </ul>
            </div>
            <div class="modal-footer">
                <a data-role="none" href="#" data-rel="back" data-dismiss="modal" class="btn custom-btn-theme-b btn-secondary btn-block btn-lg">
                    Close
                </a>
            </div>
        </div>
    </div>
</div>

<div id="popFMCGOtherPaymentDetails" data-backdrop="static" data-keyboard="false" class="modal fade" tabindex="-1" style="display:none;">
    <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Other Payment</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div subpage="pageMsgOP" id="txtOPMsgRequired" style="color:#ff0000; display:none;">Fields with (<span style="color:#F00">*</span>) are required.</div>
                <div subpage="pageMsgOP" id="txtOPMsgErrorIssuanceDate" style="color:#ff0000; display:none;">Please enter valid card issuance date.</div>
                <div subpage="pageMsgOP" id="txtOPMsgErrorExpirationDate" style="color:#ff0000; display:none;">Please enter valid card expiration date.</div>
                <div subpage="pageMsgOP" id="txtOPMsgErrorVPlus" style="color:#ff0000; display:none;">Getting V-Plus Points are required internet connection.</div>
                <input type="text" data-role="none" class="form-control" name="txtOPID" style="display:none" hidden />
                <div class="mb-3">
                    <label class="form-label">Payment Type: <span style="color:#ff0000">*</span></label>
                    <select data-role="none" class="form-select" name="selOPPaymentType">
                        <option value="">Please Select</option>
                        <option value="V-Plus">V-Plus</option>
                        <option value="SMAC">SMAC</option>
                        <option value="E-Plus">E-Plus</option>
                        <option value="Discount Voucher / Online Line Deals">Discount Voucher / Online Line Deals</option>
                        <option value="Witholding Tax">Witholding Tax</option>
                    </select>
                </div>

                <div id="divPaymentTypeSection">
                    <div class="mb-3" style="display:none" id="divAvailablePts">
                        <label class="form-label">Available Points: <span style="color:#ff0000">*</span></label>
                        <input type="text" data-role="none" class="form-control" name="txtOPAvailablePts" readonly />
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Card/ Voucher Holder Name:</label>
                        <input type="text" data-role="none" class="form-control" name="txtOPCardHolder" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Card/ Reference No.: <span style="color:#ff0000">*</span></label>
                        <input type="number" data-role="none" class="form-control number-only" name="txtOPCardNo" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Approval No.:</label>
                        <input type="number" data-role="none" class="form-control number-only" name="txtOPApprovalNo" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Payment Amount: <span style="color:#ff0000">*</span></label>
                        <input type="number" data-role="none" class="form-control number-only" name="txtOPAmount" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Issuing Organization: <span style="color:#ff0000">*</span></label>
                        <input type="text" data-role="none" class="form-control" name="txtOPIssuingOrg" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Issuance Date:</label>
                        <input type="date" data-role="none" class="form-control" name="txtOPIssuanceDate" />
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Expiration Date:</label>
                        <input type="date" data-role="none" class="form-control" name="txtOPExpirationDate" />
                    </div>
                </div>

                <div id="divPaymentTypeWithholdTax">
                    <div class="mb-3">
                        <label class="form-label">Basis for Gross Taxable Amount:</label>
                        <select name="selPaymentTypeBasisForGrossTaxableAmount" data-role="none" class="form-select">
                            <option value="NetInvoiceAmount">Net Invoice Amount</option>
                            <option value="AmountAppliedToInvoices">Specific Payment Applied to the Invoice</option>
                            <option value="UserDefined">User-Defined Net Taxable Amount</option>
                        </select>
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Gross Taxable Amount: <span style="color:#ff0000">*</span></label>
                        <input type="text" data-role="none" class="form-control" readonly name="txtPaymentTypeGrossTaxableAmount" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label">VAT Rate (%): <span style="color:#ff0000">*</span></label>
                        <input type="text" data-role="none" class="form-control" readonly name="txtPaymentTypeVatRate" value="12" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Net Taxable Amount:</label>
                        <input type="text" data-role="none" class="form-control" readonly name="txtPaymentTypeNetTaxableAmount" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Witholding Tax Rate: <span style="color:#ff0000">*</span></label>
                        <select name="selPaymentTypeWitholdingTaxRate" data-role="none" class="form-select">
                            <option value="1">1% Withholding Tax on Goods</option>
                            <option value="2">2% Withholding Tax on Services</option>
                            <option value="5">5% Withholding Tax on Rent</option>
                        </select>
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Witholding Tax Amount:</label>
                        <input type="text" data-role="none" class="form-control" readonly name="txtPaymentWitholdingTaxAmount" />
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col">
                        <a data-role="none" href="#" data-rel="back" data-dismiss="modal" class="btn custom-btn-theme-b btn-secondary btn-block btn-lg">
                            Cancel
                        </a>
                    </div>
                    <div class="col">
                        <button data-role="none" name="btnOkOtherPaymentDetails" class="btn custom-btn-theme-b btn-success btn-block btn-lg">
                            Ok
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div data-role="popup" id="popFMCGOtherPayment" data-dismissible="true" data-overlay-theme="a" data-theme="a" data-corners="false">
    <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
    <center><h3 style="margin-left:10px; margin-right:10px;">Other Payment</h3></center>
    <div data-role="content" id="popFMCGOtherPaymentcontent" class="ui-content ui-body-d jPopupContent">
        
    </div>
</div>

<div id="popFMCGCash-bs" data-backdrop="static" class="modal fade" tabindex="-1" style="display:none;">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Cash</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body" id="popFMCGCashcontent-bs">

            </div>
        </div>
    </div>
</div>

<div data-role="popup" id="popFMCGCash" data-dismissible="true" data-overlay-theme="a" data-theme="a" data-corners="false">
    <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
    <center><h3 style="margin-left:10px; margin-right:10px;">Cash</h3></center>
    <div data-role="content" id="popFMCGCashcontent" class="ui-content ui-body-d jPopupContent">
        <form id="formFMCGCash" method="post">
            <span id="txtCashMsg" style="color:#ff0000; display:none;">Please pay full amount due</span>
            <input type="number" name="txtCashAmount" class="form-control number-only" data-role="none" placeholder="Enter Amount here" style="height: 50px; font-size: 30px; text-align: right;" />
            <br />
            <div class="divTable autoheight">
                <div class="divTableRow">
                    <div class="divTableCell">
                        <button class="btnNumPad  btn btn-light border-secondary btn-lg btn-block" amount="1000">1000</button>
                    </div>
                    <div class="divTableCell">
                        <button class="btnNumPad  btn btn-light border-secondary btn-lg btn-block" amount="500">500</button>
                    </div>
                    <div class="divTableCell">
                        <button class="btnNumPad  btn btn-light border-secondary btn-lg btn-block" amount="200">200</button>
                    </div>
                </div>
                <div class="divTableRow">
                    <div class="divTableCell">
                        <button class="btnNumPad  btn btn-light border-secondary btn-lg btn-block" amount="100">100</button>
                    </div>
                    <div class="divTableCell">
                        <button class="btnNumPad  btn btn-light border-secondary btn-lg btn-block" amount="50">50</button>
                    </div>
                    <div class="divTableCell">
                        <button class="btnNumPad  btn btn-light border-secondary btn-lg btn-block" amount="20">20</button>
                    </div>
                </div>
                <div class="divTableRow">
                    <div class="divTableCell">
                        <button class="btnNumPad  btn btn-light border-secondary btn-lg btn-block" amount="10">10</button>
                    </div>
                    <div class="divTableCell">
                        <button class="btnNumPad  btn btn-light border-secondary btn-lg btn-block" amount="5">5</button>
                    </div>
                    <div class="divTableCell">
                        <button class="btnNumPad  btn btn-light border-secondary btn-lg btn-block" amount="1">1</button>
                    </div>
                </div>
                <div class="divTableRow">
                    <div class="divTableCell">
                        <button class="btnNumPad  btn btn-light border-secondary btn-lg btn-block" amount="0.25">0.25</button>
                    </div>
                    <div class="divTableCell">
                        <button class="btnNumPad  btn btn-light border-secondary btn-lg btn-block" amount="0">0</button>
                    </div>
                    <div class="divTableCell">
                        <button class="btnNumPad  btn btn-light border-secondary btn-lg btn-block" amount="c">C</button>
                    </div>
                </div>
            </div>
            <br />
            <button type="submit" class="btn btn-secondary btn-block btn-lg">Ok</button>
        </form>
    </div>
</div>

<div id="popFMCGCard-bs" data-backdrop="static" class="modal fade" tabindex="-1" style="display:none;">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Card</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body" id="popFMCGCardcontent-bs">

            </div>
        </div>
    </div>
</div>

<div data-role="popup" id="popFMCGCard" data-position-to="window" data-dismissible="true" data-overlay-theme="a" data-theme="a" data-corners="false">
    <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
    <center><h3 style="margin-left:10px; margin-right:10px;">Debit / Credit Card</h3></center>
    <div data-role="content" id="popFMCGCardcontent" class="ui-content ui-body-d jPopupContent">
        <div subpage="pageCard" id="divListOfCard" style="display:block;">
            <div class="divTable">
                <div class="divTableRow">
                    <div class="divTableCell">
                        Press ESC to close
                    </div>
                    <div class="divTableCell" style="text-align:right">
                        <button data-role="none" id="btnAddCard" class="btn btn-less-width custom-btn-theme-c btn-success">Add (press +)</button>
                    </div>
                </div>
            </div>
            <br />
            <ul id="listCard" class="custom-list custom-list-plain">
                <li class="list-group-item">List is empty</li>
            </ul>

            <br />
            <a data-role="none" href="#" data-rel="back" data-dismiss="modal" class="btn custom-btn-theme-c btn-secondary btn-lg btn-block">Close</a>
        </div>

        <div subpage="pageCard" id="divNewCard" style="display:none">
            <form id="formFMCGCard" method="post">
                <div subpage="pageCardError" id="txtErrorMsgRequired" style="color:#ff0000; display:none;">Fields with (<span style="color:#F00">*</span>) are required.</div>
                <div subpage="pageCardError" id="txtErrorIssuanceDate" style="color:#ff0000; display:none;">Please enter valid card issuance date.</div>
                <div subpage="pageCardError" id="txtErrorExpirationDate" style="color:#ff0000; display:none;">Please enter valid card expiration date.</div>
                <input type="text" data-role="none" class="form-control" name="txtCardID" style="display:none;" />
                <div class="custom-ui-label">
                    <div>Card Type:</div>
                    <div>
                        <select name="selCardType" data-role="none" class="form-select">
                            <option value="Debit">Debit</option>
                            <option value="Credit">Credit</option>
                        </select>
                    </div>
                </div>
                <br />
                <div class="custom-ui-label" id="divFMCGCredit" style="display:none;">
                    <div>Credit Card: <span style="color:#ff0000">*</span></div>
                    <div>
                        <select name="selCreditCardType" data-role="none" class="form-select">
                            <option value="Visa">Visa</option>
                            <option value="Master Card">Master Card</option>
                            <option value="Union Pay">Union Pay</option>
                            <option value="JCB">JCB</option>
                            <option value="Diners">Diners</option>
                            <option value="American Express">American Express</option>
                            <option value="Other Card">Other Card</option>
                        </select>
                    </div>
                    <br />
                </div>
                
                <div class="custom-ui-label">
                    <div>Issuing Bank: <span style="color:#ff0000">*</span></div>
                    <div>
                        <button data-role="none" name="butCardIssuingBank" class="btn custom-btn-theme-c btn-secondary btn-block">Search</button>
                        @*<select name="selCardIssuingBank" data-role="none" class="form-control"></select>*@
                    </div>
                </div>
                <br />
                <div class="custom-ui-label">
                    <div>Bank Location (Country):</div>
                    <div>
                        <select name="selCardBankLocation" data-role="none" class="form-select"></select>
                    </div>
                </div>
                <br />
                <h4>Card Holder</h4>
                <div class="custom-ui-label">
                    <div>First Name: <span style="color:#ff0000">*</span></div>
                    <div>
                        <input data-role="none" type="text" class="form-control" name="txtCardFname" />
                    </div>
                </div>
                <br />
                <div class="custom-ui-label">
                    <div>Middle Name:</div>
                    <div>
                        <input data-role="none" type="text" class="form-control" name="txtCardMname" />
                    </div>
                </div>
                <br />
                <div class="custom-ui-label">
                    <div>Last Name: <span style="color:#ff0000">*</span></div>
                    <div>
                        <input data-role="none" type="text" class="form-control" name="txtCardLname" />
                    </div>
                </div>
                <br />
                <div class="custom-ui-label">
                    <div>Generational Suffix:</div>
                    <div>
                        <select name="selCardGenSuffix" class="form-select" data-role="none">
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
                <br />
                <div class="custom-ui-label">
                    <div>Card No: <span style="color:#ff0000">*</span> (Last 4 digit of the card only)</div>
                    <div>
                        <input data-role="none" type="text" class="form-control" name="txtCardNo" maxlength="4" />
                    </div>
                </div>
                <br />
                <div class="custom-ui-label">
                    <div>Approval No:</div>
                    <div>
                        <input data-role="none" type="text" class="form-control" name="txtCardApprovalNo" />
                    </div>
                </div>
                <br />
                <div class="custom-ui-label">
                    <div>No. of Installment Payments:</div>
                    <div>
                        <input data-role="none" type="text" class="form-control" name="txtCardInstallmentNo" />
                    </div>
                </div>
                <br />
                <div class="custom-ui-label">
                    <div>Amount: <span style="color:#ff0000">*</span></div>
                    <div>
                        <input data-role="none" type="text" class="form-control" name="txtCardAmount" />
                    </div>
                </div>
                <br />
                <div class="custom-ui-label">
                    <div>Issuance Date:</div>
                    <div>
                        <button data-role="none" id="btnCardIssuanceDate" class="btn custom-btn-theme-c btn-secondary btn-block">Set Date</button>
                    </div>
                </div>
                <br />
                <div class="custom-ui-label">
                    <div>Expiration Date:</div>
                    <div>
                        <button data-role="none" id="btnCardExpirationDate" class="btn custom-btn-theme-c btn-secondary btn-block">Set Date</button>
                    </div>
                </div>
                <br /><br />
                <div class="divTable">
                    <div class="divTableRow">
                        <div class="divTableCell">
                            <button id="btnCardBack" class="btn btn-secondary btn-block btn-lg">Back</button>
                        </div>
                        <div class="divTableCell">
                            <input type="submit" id="btnSubmitCard" data-role="button" class="btn btn-success btn-block btn-lg" value="Ok" />
                        </div>
                    </div>
                </div>
            </form>
        </div>
    </div>
</div>

<div id="popFMCGCharges-bs" data-backdrop="static" class="modal fade" tabindex="-1" style="display:none;">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Charges and Other Tax</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body" id="popFMCGChargescontent-bs">

            </div>
        </div>
    </div>
</div>

<div data-role="popup" id="popFMCGCharges" data-dismissible="true" data-overlay-theme="a" data-theme="a" data-corners="false">
    <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
    <center><h3 style="margin-left:10px; margin-right:10px;">Charges and Other Tax</h3></center>
    <div data-role="content" id="popFMCGChargescontent" class="ui-content ui-body-d jPopupContent">
        <div subpage="pageCharges" id="divListOfCharges" style="display:block;">
            <div class="divTable">
                <div class="divTableRow">
                    <div class="divTableCell">
                        Press ESC to close
                    </div>
                    <div class="divTableCell" style="text-align:right">
                        <button data-role="none" id="btnAddCharge" class="btn btn-less-width custom-btn-theme-c btn-success">Add (press +)</button>
                    </div>
                </div>
            </div>
            <br />
            <ul id="listCharge" class="custom-list custom-list-plain">
                <li class="list-group-item">List is empty</li>
            </ul>

            <br />
            <a data-role="none" href="#" data-rel="back" data-dismiss="modal" class="btn custom-btn-theme-c btn-secondary btn-lg btn-block">Close</a>
        </div>

        <div subpage="pageCharges" id="divNewCharges" style="display:none">
            <form id="formFMCGCharge" method="post">
                <div subpage="pageChargeError" id="txtChargeErrorMsgRequired" style="color:#ff0000; display:none;">Fields with (<span style="color:#F00">*</span>) are required.</div>
                <input type="text" data-role="none" class="form-control" name="txtChargeID" style="display:none;" />
                <div class="custom-ui-label">
                    <div>Card Type:</div>
                    <div>
                        <select name="selChargeType" data-role="none" class="form-select">
                            <option value="Service Charge">Service Charge</option>
                            <option value="Delivery Charge">Delivery Charge</option>
                            <option value="Other Charges">Other Charges</option>
                            <option value="Local Tax">Local Tax</option>
                        </select>
                    </div>
                </div>
                <br />
                <div class="custom-ui-label">
                    <div>Percentage (%):</div>
                    <div>
                        <input type="number" data-role="none" class="form-control number-only" name="txtChargePercentage" />
                    </div>
                </div>
                <br />
                <div class="custom-ui-label">
                    <div>Amount: <span style="color:#ff0000">*</span></div>
                    <div>
                        <input type="number" data-role="none" class="form-control number-only" name="txtChargeAmount" />
                    </div>
                </div>
                <br />
                <div class="custom-ui-label">
                    <div>Remarks:</div>
                    <div>
                        <textarea data-role="none" class="form-control" name="txtChargeRemarks" style="min-height:100px; max-height:100px;"></textarea>
                    </div>
                </div>
                <br />
                <input type="submit" data-role="none" class="btn custom-btn-theme-b btn-success btn-block btn-lg mb-3" id="btnSubmitCharges" value="Ok" />
            </form>

            <button data-role="none" id="btnChargeBack" class="btn custom-btn-theme-c btn-secondary btn-block btn-lg">Cancel</button>
        </div>
    </div>
</div>

<div id="popFMCGSoldTo-bs" class="modal fade" tabindex="-1" style="display:none;">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Customer Info</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body" id="popFMCGSoldTocontent-bs">

            </div>
        </div>
    </div>
</div>

<div data-role="popup" id="popFMCGSoldTo" data-dismissible="true" data-overlay-theme="a" data-theme="a" data-corners="false">
    <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
    <center><h3 style="margin-left:10px; margin-right:10px;">Customer Info</h3></center>
    <div data-role="content" id="popFMCGSoldTocontent" class="ui-content ui-body-d jPopupContent">
        <div subpage="pageSoldTo" id="divListOfCustomer" style="display:block;">
            <div class="divTable">
                <div class="divTableRow">
                    <div class="divTableCell">
                        <input type="text" data-role="none" class="form-control" name="txtSoldToSearch" placeholder="Search existing customer" />
                    </div>
                    <div class="divTableCell txt-top" style="text-align:right; width: 135px;">
                        <button data-role="none" id="btnAddSoldTo" class="btn btn-less-width custom-btn-theme-c btn-sm">Add (press +)</button>
                    </div>
                </div>
            </div>
            <br />
            <div>
                Current Customer: <span style="font-weight:bold;" id="txtCurrentCustomerName"></span>
            </div>

            <div id="divAvailableScroll" style="max-height:300px; overflow:auto">
                <ul id="listAvailabelCustomer" class="custom-list custom-list-plain"></ul>
            </div>

            <br />
            <a data-role="button" href="#" data-rel="back">Close</a>
        </div>

        <div subpage="pageSoldTo" id="divNewCustomer" style="display:none">
            <span style="float:right;">Press (-) to go back</span>
            <br /><br />
            <form id="formFMCGSoldTo" method="post">
                <div subpage="pageSoldToError" id="txtSoldToErrorMsgRequired" style="color:#ff0000; display:none;">Fields with (<span style="color:#F00">*</span>) are required.</div>
                <div subpage="pageSoldToError" id="txtSoldToExist" style="color:#ff0000; display:none;"><span id="txtKindExistName"></span> already exist.</div>
                <div subpage="pageSoldToError" id="txtSoldToFullName" style="color:#ff0000; display:none;">Full Name is required.</div>
                <input type="text" data-role="none" class="form-control" name="txtChargeID" style="display:none;" />
                <div class="custom-ui-label">
                    <div>Customer Type:</div>
                    <div>
                        <select name="selCustomerType" data-role="none" class="form-control">
                            <option value="Organization">Organization</option>
                            <option value="Person">Person</option>
                        </select>
                    </div>
                </div>

                <div subpage="pageNewCustomer" id="divFMCGCustomerOrg" style="display:block;">
                    <div class="custom-ui-label">
                        <div>Company Name: <span style="color:#ff0000">*</span></div>
                        <div>
                            <input type="text" data-role="none" class="form-control" name="txtCustomerOrgName" />
                        </div>
                    </div>

                    <div class="custom-ui-label">
                        <div>TIN: <span style="color:#ff0000">*</span></div>
                        <div>
                            <input type="number" data-role="none" class="form-control number-only" name="txtCustomerOrgTin" />
                        </div>
                    </div>

                    <div class="custom-ui-label">
                        <div>Address:</div>
                        <div>
                            <input type="text" data-role="none" class="form-control" name="txtCustomerOrgBldgNo" placeholder="House / Bldg. No." />
                            <input type="text" data-role="none" class="form-control" name="txtCustomerOrgStreet" placeholder="Street" />
                            <select name="selCustomerOrgCity" data-role="none" class="form-control"></select>
                            <select name="selCustomerOrgCountry" data-role="none" class="form-control"></select>
                        </div>
                    </div>
                </div>

                <div subpage="pageNewCustomer" id="divFMCGCustomerPerson" style="display:none;">
                    <div class="custom-ui-label">
                        <div>First Name: <span style="color:#ff0000">*</span></div>
                        <div>
                            <input type="text" data-role="none" class="form-control" name="txtCustomerPersonFName" />
                        </div>
                    </div>

                    <div class="custom-ui-label">
                        <div>Middlen Name: <span style="color:#ff0000">*</span></div>
                        <div>
                            <input type="text" data-role="none" class="form-control" name="txtCustomerPersonMName" />
                        </div>
                    </div>

                    <div class="custom-ui-label">
                        <div>Last Name: <span style="color:#ff0000">*</span></div>
                        <div>
                            <input type="text" data-role="none" class="form-control" name="txtCustomerPersonLName" />
                        </div>
                    </div>

                    <div class="custom-ui-label">
                        <div>TIN:</div>
                        <div>
                            <input type="number" data-role="none" class="form-control number-only" name="txtCustomerPersonTin" />
                        </div>
                    </div>
                </div>

                <input type="submit" data-role="button" id="btnSubmitCharges" value="Save" />
                <button id="btnSoldToBack">Back</button>
            </form>
        </div>
    </div>
</div>

<div id="popFMCGMisc-bs" class="modal fade" tabindex="-1" style="display:none;">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Miscellaneuos Payment</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body" id="popFMCGMisccontent-bs">

            </div>
        </div>
    </div>
</div>

<div data-role="popup" id="popFMCGMisc" data-dismissible="true" data-overlay-theme="a" data-theme="a" data-corners="false">
    <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
    <center><h3 style="margin-left:10px; margin-right:10px;">Miscellaneuos Payment</h3></center>
    <div data-role="content" id="popFMCGMisccontent" class="ui-content ui-body-d jPopupContent">
        <div style="text-align:right;">
            <h3>TOTAL: <span class="txtMisc">0.00</span></h3>
        </div>
        <ul id="listMisc" class="custom-list custom-list-plain">
            <li class="navList-a" id="btnMiscCharges" popup="charges"><div>Charge and Other Tax <span id="txtMiscTotalCharge" style="float:right">0.00</span></div></li>
            <li id="btnMiscSrv" popup="srv"><div>SRV <span id="txtMiscTotalSrv" style="float:right">0.00</span></div></li>
            <li id="btnMiscOp" popup="op"><div>Other Payment <span id="txtMiscTotalOp" style="float:right">0.00</span></div></li>
        </ul>
        <br />
        <a data-role="button" href="#" data-rel="back">Close</a>
    </div>
</div>

<div id="popFMCGRewardsProgram-bs" class="modal fade" tabindex="-1" style="display:none;">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Rewards Program</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body" id="popFMCGRewardsProgramcontent-bs">

            </div>
        </div>
    </div>
</div>

<div data-role="popup" id="popFMCGRewardsProgram" data-dismissible="true" data-overlay-theme="a" data-theme="a" data-corners="false">
    <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
    <center><h3 style="margin-left:10px; margin-right:10px;">Rewards Program</h3></center>
    <div data-role="content" id="popFMCGRewardsProgramcontent" class="ui-content ui-body-d jPopupContent">
        <div>Enter Rewards Card No.: <span class="required">*</span></div>
        <input data-role="none" type="text" class="form-control" id="txtRewardCardNo" />
        <br /><br />
        <div class="row">
            <div class="col-sm-6">
                <a data-role="none" href="#" data-rel="back" data-dismiss="modal" class="btn custom-btn-theme-c btn-secondary btn-block btn-lg">Close</a>
            </div>
            <div class="col-sm-6">
                <button data-role="none" id="btnRewardsProgramOk" class="btn custom-btn-theme-c btn-success btn-block btn-lg">Ok</button>
            </div>
        </div>
    </div>
</div>

<div id="popFMCGOnCredit-bs" class="modal fade" tabindex="-1" style="display:none;">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">On Credit</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body" id="popFMCGOnCreditcontent-bs">

            </div>
        </div>
    </div>
</div>

<div data-role="popup" id="popFMCGOnCredit" data-dismissible="true" data-overlay-theme="a" data-theme="a" data-corners="false">
    <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
    <center><h3 style="margin-left:10px; margin-right:10px;">On Credit</h3></center>
    <div data-role="content" id="popFMCGOnCreditcontent" class="ui-content ui-body-d jPopupContent">
        <div>Amount: <span class="required">*</span></div>
        <input data-role="none" type="number" class="form-control" id="txtOnCreditAmt" />
        <br />
        <div>Remarks:</div>
        <textarea data-role="none" class="form-control" style="height:100px; resize:none;" id="txtOnCreditRemarks"></textarea>
        <br /><br />
        <div class="row">
            <div class="col-sm-6">
                <a data-role="none" href="#" data-rel="back" data-dismiss="modal" class="btn custom-btn-theme-c btn-secondary btn-block btn-lg">Close</a>
            </div>
            <div class="col-sm-6">
                <button data-role="none" id="btnOnCreditOk" class="btn custom-btn-theme-c btn-success btn-block btn-lg">Ok</button>
            </div>
        </div>
    </div>
</div>