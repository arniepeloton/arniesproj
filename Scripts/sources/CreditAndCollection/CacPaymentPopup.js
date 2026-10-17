require([
    'common',
    'promise!/Scripts/sources/ERP/FinancialDetails/controller.js?v=' + assetversion
    //'/Scripts3/Offline/Browser/LibraryOrganizationBanks.js',
    //'/Scripts3/Offline/Browser/LibraryCountries.js'
], function () {
    var mBankID;
    var mBankID2;
    $(document).ready(function () {
        var isDiscountTypeLoaded = false;
        var mFinancialDetails = new ERP.FinancialDetails.controller();
        var mAccountDetails; // array        

        window.CacPopup = {
            params: null,
            popupname: '',
            returnVal: null,
            isInAddPage: false,
            amounttendered: 0,
            FromEcommerce: false,
            FromWebPos: false,
            ChequeGC: {
                txtNewCGC: null,
                txtCGCID: null,
                selCGCBank: null,
                txtCGCBranchID: null,
                txtCGCBranch: null,
                txtCGCAccountName: null,
                txtCGCAccountNo: null,
                txtCGCAccountNoAlphaNumeric: null,
                txtCGCChequeGCNo: null,
                txtCGCValue: null,
                btnChequeDate: null,
                btnCGCBack: null,
                form: null,
                isFinishedRequiredData: false,
                intSalesPayCustomerNameIndex: 0,
                cgcState: 'New',
                arrCGC: [],
                totalcgc: 0,
                editIndex: null,
                Clear: function () {
                    this.intSalesPayCustomerNameIndex = 0;
                    this.cgcState = 'New';
                    this.arrCGC = [];
                    this.totalcgc = 0;
                    CacPopup.popupname = '';

                    $('#listCGC').empty().append('List is empty');
                    $('#lblBalChange').text('UNPAID');
                    $("#txtBalChange, .txtCGC").text('0.00');
                },
                init: function (param) {
                    this.form = $('#formFMCGChequeGC');
                    this.txtNewCGC = this.form.find('[name="txtNewCGC"]');
                    this.txtCGCID = this.form.find('[name="txtCGCID"]');
                    this.selCGCBank = this.form.find('[name="selCGCBank"]');
                    this.txtCGCBranchID = this.form.find('[name="txtCGCBranchID"]');
                    this.txtCGCBranch = this.form.find('[name="txtCGCBranch"]');
                    this.txtCGCAccountName = this.form.find('[name="txtCGCAccountName"]');
                    this.txtCGCAccountNo = this.form.find('[name="txtCGCAccountNo"]');
                    this.txtCGCAccountNoAlphaNumeric = $('#divChequeSection [name="txtCGCAccountNoAlphaNumeric"]');
                    this.txtCGCChequeGCNo = this.form.find('[name="txtCGCChequeGC"]');
                    this.txtCGCValue = this.form.find('[name="txtCGCValue"]');
                    this.butCGCBank = this.form.find('[name="butCGCBank"]');
                    this.butAddCheckingAccount = $('#pageNewCGG [name="butAddCheckingAccount"]');
                    this.butDeactivateCheckingAccount = $('#pageNewCGG [name="butDeactivateCheckingAccount"]');
                    this.btnChequeDate = this.form.find('[id="btnChequeDate"]');
                    this.txtCGCValue = this.form.find('[name="txtCGCValue"]');
                    this.btnCGCBack = this.form.find('[id="btnCGCBack"]');
                    CacPopup.params = param;
                    var mBankName;
                    var mFinancialDetailID;
                    var mBankName2;
                    var mBankBranchName2;
                    var mAccountName2;
                    var mAccountNumber2;

                    this.txtCGCAccountNoAlphaNumeric.click(function(e) {
                        var pTxt = $(e.target);
                        pTxt.select();
                        document.execCommand("copy");
                        WebToast('Copied to clipboard');
                    });

                    if (proj.IsBootstrapDesign()) {
                        $('#popFMCGChequeGC-bs').modal('show').on('hidden.bs.modal', function () {
                            
                        }).find('[data-dismiss=modal]').unbind('click').click(function(e) {
                            e.preventDefault();

                            if (IsEcommerce) {
                                $('#popupCustomerPayment').modal('show').on('hidden.bs.modal', function () {
                                    if (!IsEcommerce) {
                                        $('#popupCustomerPayment').unbind('hidden.bs.modal');
                                        $('#popupAccountDetails').modal('show');
                                    }                        
                                });
                            }                        

                            return false;
                        });
                        $('#listCGC').addClass('list-group');
                    } else {
                        $('#popFMCGChequeGC').popup('open');
                    }
                    
                    CacPopup.popupname = 'cgc';

                    if (this.isFinishedRequiredData == false) {
                        this.LoadRequiredData()
                        this.isFinishedRequiredData = true;
                    }

                    this.Main();

                    BindKeypress('#formFMCGChequeGC', this.selCGCBank, false, 'select');
                    BindKeypress('#formFMCGChequeGC', this.txtCGCBranch, false, 'text');
                    BindKeypress('#formFMCGChequeGC', this.txtCGCAccountName, false, 'text');
                    BindKeypress('#formFMCGChequeGC', this.txtCGCAccountNo, false, 'text');
                    BindKeypress('#formFMCGChequeGC', this.txtCGCChequeGCNo, false, 'text');
                    BindKeypress('#formFMCGChequeGC', this.txtCGCValue, true, 'text');

                    $('[name=radChequeOrGiftCert],[name=tabAccountDetailsFromToButton]').unbind('click').click(function() {
                        FilterAccountDetails();
                    });

                    $('#btnAddCGC').unbind('click').click(function () {
                        CacPopup.ChequeGC.Add();
                    });

                    this.butCGCBank.text('Select a Bank').unbind('click').click(function (e) {
                        e.preventDefault();
                        window.CacPopup.BrowseBank({
                            Click: function(vData) {
                                mBankID = vData.ID;
                                mBankName = vData.CompanyName;
                                CacPopup.ChequeGC.butCGCBank.text(vData.CompanyName);
                                if (proj.IsBootstrapDesign()) {
                                    $('#popFMCGChequeGC-bs').modal('show');
                                } else {
                                    $('#popFMCGChequeGC').popup('open');
                                }
                            },
                            Close: function() {
                                if (proj.IsBootstrapDesign()) {
                                    $('#popFMCGChequeGC-bs').modal('show');
                                } else {
                                    $('#popFMCGChequeGC').popup('open');
                                }
                            }
                        });
                    });

                    this.selCGCBank.unbind('change').change(function () {
                        mBankID = $(this).val();
                        mBankName = $(this).find('option:selected').text();
                    })

                    this.btnCGCBack.unbind('click').click(function (e) {
                        e.preventDefault();

                        $('#pageListOfCGC').pageshow();
                    });

                    this.butAddCheckingAccount.unbind('click').click(function (e) {
                        e.preventDefault();

                        if (CacPopup.FromEcommerce)
                            Proceed();
                        else
                            PromptUserAccess('Add Customer Financial Details', {
                                OnClickOKSuccess: function () {                                                                                        
                                    Proceed();
                                },
                                OnFailed: function () {
                                    if (proj.IsBootstrapDesign()) {
                                        $('#popFMCGChequeGC-bs').modal('show');
                                    } else {
                                        $('#popFMCGChequeGC').popup('open');
                                    }
                                }
                            });
                        function Proceed() {
                            var pOrgID, pOrgID2;
                            if (proj.CacMode == 'AP') {
                                pOrgID = sessionStorage.OrgID;
                                pOrgID2 = sessionStorage.CustomerOrganizationID;
                            } else {
                                pOrgID = sessionStorage.CustomerOrganizationID;
                                pOrgID2 = sessionStorage.OrgID;
                            }
                            pOrgID = ReadyGuid(pOrgID);
                            pOrgID2 = ReadyGuid(pOrgID2);
                            var pType = $('[name=radChequeOrGiftCert]:checked').val();
                            var pFromTo = $("#tabAccountDetailsFromTo button.active").text(); // From or To
                            if (pType == 'online-payment') {
                                if (pFromTo == 'To')
                                    pOrgID = pOrgID2;
                            }

                            mFinancialDetails.Show({
                                CustomerOrgID: pOrgID,
                                FromEcommerce: CacPopup.FromEcommerce,
                                FromWebPos: CacPopup.FromWebPos,
                                PaymentType: pType,
                                AfterSave: function() {
                                    GetFinancialDetails();
                                    if (proj.IsBootstrapDesign()) {
                                        $('#popFMCGChequeGC-bs').modal('show');
                                    } else {
                                        $('#popFMCGChequeGC').popup('open');
                                    }
                                }
                            });
                        }
                    });
                    
                    this.butDeactivateCheckingAccount.unbind('click').click(function (e) {
                        e.preventDefault();

                        PromptUserAccess('Deactivate Customer Financial Details', {
                            OnClickOKSuccess: function () {                                                                                        
                                if (mFinancialDetailID == null) {
                                    WebToast('Please select an account to deactivate.', 'top');
                                    return;
                                }

                                NewMessageBox('Are you sure you want to deactivate this checking account?', 'Deactivate', [
                                    {
                                        Text: 'Yes',
                                        Click: function() {
                                            AjaxSend({
                                                url: '/CreditAndCollection/DeactivateFinancialDetails',
                                                data: {
                                                    ID: mFinancialDetailID
                                                },
                                                success: function(vData) {
                                                    GetFinancialDetails();
                                                    NewMessageBox('The checking account was successfully deactivated.', 'Deactivated', null,
                                                    {
                                                        HidePrevPopup: false,
                                                        Close: function() {
                                                            if (proj.IsBootstrapDesign()) {
                                                                $('#popFMCGChequeGC-bs').modal('show');
                                                            } else {
                                                                $('#popFMCGChequeGC').popup('open');
                                                            }
                                                        }
                                                    });
                                                }
                                            });
                                        }
                                    },
                                    {
                                        Text: 'No'
                                    }
                                ], {
                                    HidePrevPopup: false,
                                    Close: function() {
                                        if (proj.IsBootstrapDesign()) {
                                            $('#popFMCGChequeGC-bs').modal('show');
                                        } else {
                                            $('#popFMCGChequeGC').popup('open');
                                        }
                                    }
                                });
                            },
                            OnFailed: function () {
                                if (proj.IsBootstrapDesign()) {
                                    $('#popFMCGChequeGC-bs').modal('show');
                                } else {
                                    $('#popFMCGChequeGC').popup('open');
                                }
                            }
                        });
                    });

                    this.btnChequeDate.unbind('click').click(function (e) {
                        e.preventDefault();
                        if (proj.IsBootstrapDesign()) {
                            //$('#popFMCGChequeGC-bs').modal('hide');
                        } else {
                            $('#popFMCGChequeGC').popup('close');
                        }
                        SingleCalendar({
                            HidePrevPopup: false,
                            Click: function (e) {
                                if (proj.IsBootstrapDesign()) {
                                    $('#popFMCGChequeGC-bs').modal('show');
                                } else {
                                    $('#popFMCGChequeGC').popup('open');
                                }
                                CacPopup.ChequeGC.btnChequeDate.text(e);
                                CacPopup.ChequeGC.txtCGCValue.focus();
                            },
                            Cancel: function () {
                                if (proj.IsBootstrapDesign()) {
                                    $('#popFMCGChequeGC-bs').modal('show');
                                } else {
                                    $('#popFMCGChequeGC').popup('open');
                                }
                                CacPopup.ChequeGC.txtCGCValue.focus();
                            }
                        });
                    });

                    $('#formFMCGChequeGC').unbind('submit').submit(function (e) {
                        console.log('btnCGCBack.click js64MNYVVNPT!');
                        e.preventDefault();
                        //mBankID = $(this).find('[name=selCGCBank]').val();
                        var pBankIsRequired;

                        if (CStr2(mBankID).trim() != '')
                            pBankIsRequired = false;
                        else if ($('[name=radChequeOrGiftCert]:checked').val() == 'cheque')
                            pBankIsRequired = true;
                        else
                            pBankIsRequired = false;
                        if (pBankIsRequired
                            || $.trim(CacPopup.ChequeGC.txtCGCChequeGCNo.val()) == "" || $.trim(CacPopup.ChequeGC.btnChequeDate.text()) == "Set Date"
                            || $.trim(CacPopup.ChequeGC.txtCGCValue.val()) == "") {
                            $('#txtChequeGCMsg').show();
                        }
                        else {
                            $('#txtChequeGCMsg').hide();
                            var chequeDate = CacPopup.ChequeGC.btnChequeDate.text();

                            if (IsDate(chequeDate)) {
                                chequeDate = new Date(chequeDate);
                            } else {
                                chequeDate = false;
                            }

                            var pOtherData;
                            var pType = $('[name=radChequeOrGiftCert]:checked').val();
                            if (pType == 'online-payment') {
                                if ((mBankID == null)
                                    || (mBankID2 == null)) {
                                    $('#txtChequeGCMsg').show();
                                    return;
                                }
                                pOtherData = {
                                    BankID2: mBankID2,
                                    BankName2: mBankName2,
                                    BankBranchName2: mBankBranchName2,
                                    AccountName2: mAccountName2,
                                    AccountNumber2: mAccountNumber2
                                };
                            }
                        
                            AjaxSend({
                                url: 'CreditAndCollection/ChequeDetail',
                                data: {
                                    BankID: mBankID,
                                    CheckNumber: CacPopup.ChequeGC.txtCGCChequeGCNo.val(),
                                    Mode: proj.CacMode
                                },
                                success: function(vData) {
                                    if (vData.ChequeDetail != null) {
                                        NewMessageBox('This cheque was already used as payment.<br /><br />' +
                                            'Cheque No.: ' + vData.ChequeDetail.CheckNumber + '<br />' +
                                            'Cheque Date: ' + FormatDate(vData.ChequeDetail.CheckDate, {DontDisplay12Am: true}) + '<br />' +
                                            'Cheque Amount: ' + AccountingFormat(vData.ChequeDetail.CheckAmount) + '<br />' +
                                            'Bank: ' + vData.ChequeDetail.BankName + '<br />' +
                                            'Bank Branch: ' + vData.ChequeDetail.BankBranchName + '<br />' +
                                            'Payment Date: ' + FormatDate(vData.ChequeDetail.PaymentDate, {DontDisplay12Am: true}) + '<br />' +
                                            'Payor: ' + vData.ChequeDetail.Payor + '<br />' +
                                            'Payee: ' + vData.ChequeDetail.Payee + '<br />' +
                                            'Encoder: ' + vData.ChequeDetail.Encoder, 'Duplicate Cheque');
                                        return;
                                    }
                                    Proceed();
                                }
                            });

                            function Proceed() {
                                var pData = [
                                    CacPopup.ChequeGC.txtCGCID.val(), // 0
                                    mBankID, // 1
                                    mBankName, // 2
                                    CacPopup.ChequeGC.txtCGCBranchID.val(), // 3
                                    CacPopup.ChequeGC.txtCGCBranch.val(), // 4
                                    CacPopup.ChequeGC.txtCGCAccountName.val(), // 5
                                    CacPopup.ChequeGC.txtCGCAccountNo.val(), // 6
                                    CacPopup.ChequeGC.txtCGCChequeGCNo.val(), // 7
                                    (chequeDate == false ? "" : chequeDate.getMonth()), // 8 Cheque Date Month
                                    (chequeDate == false ? "" : chequeDate.getDate()), // 9 Cheque Date Day
                                    (chequeDate == false ? "" : chequeDate.getFullYear()), // 10  Cheque Date Year
                                    CacPopup.ChequeGC.txtCGCValue.val(), // 11
                                    CacPopup.ChequeGC.cgcState, // 12
                                    CacPopup.ChequeGC.editIndex, // 13 editIndex daw
                                    $('[name=radChequeOrGiftCert]:checked').val(), // 14
                                    pOtherData // 15

                                ];
                                if (CacPopup.ChequeGC.editIndex == null) {
                                    pData[13] = CacPopup.ChequeGC.arrCGC.length;
                                    CacPopup.ChequeGC.arrCGC.push(pData);
                                }
                                else {
                                    CacPopup.ChequeGC.arrCGC[CacPopup.ChequeGC.editIndex] = pData
                                    CacPopup.ChequeGC.editIndex = null; // set to null after edit
                                }

                                CacPopup.isInAddPage = true;
                                CacPopup.ChequeGC.intSalesPayCustomerNameIndex = CacPopup.ChequeGC.intSalesPayCustomerNameIndex + 1;
                                CacPopup.ChequeGC.LoadSalesPayCheckList();
                                CacPopup.returnVal = CacPopup.ChequeGC.arrCGC;
                                //$('#popFMCGChequeGC').popup('close');
                            }
                        }
                        return false;
                    })

                    var mTable = $('#tableFinancialDetails');
                    var mDataTable;

                    function FilterAccountDetails() {
                        mFinancialDetailID = null;
                        var pType = $('[name=radChequeOrGiftCert]:checked').val();
                        if (pType == 'gift-cert')
                            $('#divChequeSection').hide();
                        else
                            $('#divChequeSection').show();
                        if (pType == 'online-payment')
                            $('#tabAccountDetailsFromTo').show();
                        else
                            $('#tabAccountDetailsFromTo').hide();

                        var pOrgID, pOrgID2;
                        if (proj.CacMode == 'AP') {
                            pOrgID = sessionStorage.OrgID;
                            pOrgID2 = sessionStorage.CustomerOrganizationID;
                        } else {
                            pOrgID = sessionStorage.CustomerOrganizationID;
                            pOrgID2 = sessionStorage.OrgID;
                        }
                        pOrgID = ReadyGuid(pOrgID);
                        pOrgID2 = ReadyGuid(pOrgID2);
                        var pFromTo = $("#tabAccountDetailsFromTo button.active").text(); // From or To

                        var pRows = mAccountDetails.filter(function(vData, vIndex) {
                            if (pType == 'cheque') {
                                if (CBool(vData.OnlinePayment))
                                    return false;
                                return ReadyGuid(vData.CustomerOrgID) == pOrgID;
                            } else if (pType == 'online-payment') {
                                if (!CBool(vData.OnlinePayment))
                                    return false;
                                if (pFromTo == 'From')
                                    return ReadyGuid(vData.CustomerOrgID) == pOrgID;
                                else
                                    return ReadyGuid(vData.CustomerOrgID) == pOrgID2;
                            }
                        });
                        mDataTable.clear();
                        mDataTable.rows.add(pRows);
                        mDataTable.draw();

                        if (pType == 'online-payment') {
                            if (pFromTo == 'From') {
                                CacPopup.ChequeGC.txtCGCAccountNoAlphaNumeric.val(CacPopup.ChequeGC.txtCGCAccountNo.val());
                                $('[name=radFinancialDetails][value=' + mBankID + ']').prop('checked', 1);
                            } else {
                                CacPopup.ChequeGC.txtCGCAccountNoAlphaNumeric.val(mAccountNumber2);
                                $('[name=radFinancialDetails][value=' + mBankID2 + ']').prop('checked', 1);
                            }
                        }
                    }

                    function GetFinancialDetails() {
                        var pOrgID

                        // BEGIN delete this code 2021_11_08__13_18_51_0402
                        //if (proj.CacMode == 'AP') 
                        //    pOrgID = sessionStorage.OrgID
                        //else
                        //    pOrgID = sessionStorage.CustomerOrganizationID;
                        // END delete this code 2021_11_08__13_18_51_0402
                            
                        AjaxSend({
                            url: '/CreditAndCollection/GetFinancialDetails',
                            data: {
                                CustomerOrgID: sessionStorage.OrgID,
                                OrgID2: sessionStorage.CustomerOrganizationID
                            },
                            success: function(vData) {
                                ClearTable();
                                DisplayTable(vData.Rows);
                            }
                        });
                    }

                    function ClearTable() {
                        mFinancialDetailID = null;
                        if ($.fn.DataTable.isDataTable(mTable)) {
                            //empty the table first before destroying datatabase para dili magdoble ang header
                            mTable.empty();
                            if (mDataTable == null)
                                mDataTable = mTable.DataTable();
                            mDataTable.destroy();
                        }
                    }
                    function DisplayTable(vRows) {
                        mAccountDetails = vRows;
                        mDataTable = mTable.DataTable({
                            retrieve: true, // https://datatables.net/manual/tech-notes/3
                            data: vRows,
                            columns: [
                                {
                                    'searchable':false,
                                    'orderable':false,
                                    'width':'1%',
                                    'className': 'dt-body-center',
                                    'render': function (data, type, full, meta){
                                        return '<input type="radio" name="radFinancialDetails">';
                                    }
                                },
                                {
                                    data: function(vData) {
                                        return vData.BankName + '';
                                    }
                                },
                                {
                                    data: function(vData) {
                                        return vData.BankBranch + '';
                                    }
                                },
                                {
                                    data: function(vData) {
                                        return vData.AccountName + '';
                                    }
                                },
                                {
                                    data: function(vData) {
                                        return vData.AccountNumber + '';
                                    }
                                }
                            ],
                            'rowCallback': function(row, data, dataIndex) {
                                $(row).data('data', data);
                                var pRadioButton = $(row).find('[name=radFinancialDetails]');
                                pRadioButton.val(data.BankOrgID);
                                pRadioButton.prop('checked', data.Checked);
                                pRadioButton.unbind('click').click(radio_OnClick);
                                $(row).unbind('click').click(function() {
                                    pRadioButton.prop('checked', true);
                                    radio_OnClick.apply(pRadioButton);
                                });
                            },
                            'initComplete': function(){
                                $('#tableFinancialDetails').removeClass('dataTable');
                            },
                            sDom: '<"top">rt<"bottom"ip><"clear">'
                        });
                        FilterAccountDetails();
                    }

                    function radio_OnClick() {
                        var pRadio = $(this);
                        var pTr = $(pRadio).closest('tr');
                        pTr = $(pTr);
                        var pDR = pTr.data('data');
                        pDR.Checked = this.checked;
                        
                        var pType = $('[name=radChequeOrGiftCert]:checked').val();
                        var pFromTo = $("#tabAccountDetailsFromTo button.active").text(); // From or To

                        mFinancialDetailID = pDR.ID;
                        if ((pType == 'online-payment')
                            && (pFromTo == 'To')) {

                            mBankID2 = pDR.BankOrgID;
                            mBankName2 = pDR.BankName;
                            mBankBranchName2 = pDR.BankBranch;
                            mAccountName2 = pDR.AccountName;
                            mAccountNumber2 = pDR.AccountNumber;
                            CacPopup.ChequeGC.txtCGCAccountNoAlphaNumeric.val(AlphaNumeric(pDR.AccountNumber));
                        } else {
                            mBankID = pDR.BankOrgID;
                            mBankName = pDR.BankName;
                            CacPopup.ChequeGC.butCGCBank.text(mBankName);
                            CacPopup.ChequeGC.txtCGCBranch.val(pDR.BankBranch);
                            CacPopup.ChequeGC.txtCGCAccountName.val(pDR.AccountName);
                            CacPopup.ChequeGC.txtCGCAccountNo.val(pDR.AccountNumber);
                            CacPopup.ChequeGC.txtCGCAccountNoAlphaNumeric.val(AlphaNumeric(pDR.AccountNumber));
                        }
                    }

                    GetFinancialDetails();
                }, // END init: function (param) {
                Add: function () {
                    this.txtCGCID.val('');
                    this.selCGCBank.val('');
                    this.txtCGCBranchID.val('');
                    this.txtCGCBranch.val('');
                    this.txtCGCAccountName.val('');
                    this.txtCGCAccountNo.val('');
                    this.txtCGCAccountNoAlphaNumeric.val('');
                    this.txtCGCChequeGCNo.val('');
                    this.txtCGCValue.val('');
                    this.btnChequeDate.text('Set Date');
                    this.cgcState = 'New';

                    $('#pageNewCGG').pageshow();
                    $('#txtChequeGCMsg').hide();
                    PopupReposition(); //NewMessageBox.js
                    CacPopup.ChequeGC.selCGCBank.focus();
                },
                Edit: function (data) {
                    var chequedate = new Date(data[10], data[8], data[9]);

                    $('#pageNewCGG').pageshow();
                    $('#txtChequeGCMsg').hide();
                    this.txtCGCID.val(data[0]);
                    this.selCGCBank.val(data[1]);
                    this.txtCGCBranchID.val(data[3]);
                    this.txtCGCBranch.val(data[4]);
                    this.txtCGCAccountName.val(data[5]);
                    this.txtCGCAccountNo.val(data[6]);
                    this.txtCGCAccountNoAlphaNumeric.val(AlphaNumeric(data[6]));
                    this.txtCGCChequeGCNo.val(data[7]);
                    this.txtCGCValue.val(data[11]);
                    this.btnChequeDate.text((IsDate(chequedate) ? $.format.date(chequedate, 'MMM dd, yyyy') : "Set Date"))
                    CacPopup.ChequeGC.cgcState = data[12];
                    this.editIndex = data[13];
                    if (data[15] != null) {
                        mBankID = data[1];
                        mBankID2 = data[15].BankID2;
                    }
                    $('[name=radChequeOrGiftCert][value="' + data[14] + '"]').prop('checked', 1);
                    $('[name=radChequeOrGiftCert][value="' + data[14] + '"]').click();
                    $('[name=radFinancialDetails][value=' + data[1] + ']').prop('checked', 1);
                    if (data[14] == 'gift-cert')
                        $('#divChequeSection').hide();
                    else
                        $('#divChequeSection').show();
                    
                    PopupReposition(); //NewMessageBox.js
                    setTimeout(function () {
                        CacPopup.ChequeGC.selCGCBank.focus();
                    }, 100)
                },
                Delete: function (data) {
                    this.arrCGC[data[13]][12] = "Remove";
                    this.LoadSalesPayCheckList();
                },
                Main: function () {
                    $('#pageListOfCGC').pageshow();
                    this.LoadSalesPayCheckList();
                    PopupReposition(); //NewMessageBox.js
                },
                LoadRequiredData: function () {
                    var dis = this;
                    AjaxSend({
                        url: 'Browser/LibraryOrganizationBanks',
                        data: { IsAll: 1 },
                        success: function (vData) {
                            var vRows = vData.Results.Rows;
                            dis.selCGCBank.empty();
                            dis.selCGCBank.append(new Option('Please Select', ''));
                            if (vRows.length > 0) {
                                for (var i = 0; i < vRows.length; i++) {
                                    dis.selCGCBank.append(new Option(vRows[i].CompanyName, vRows[i].ID));
                                }
                            }
                        }
                    })
                },
                LoadSalesPayCheckList: function () {
                    var listCGC = $('#listCGC');
                    var isEmpty = true;

                    listCGC.empty();

                    if (this.arrCGC.length > 0) {
                        for (var i = 0; i < this.arrCGC.length; i++) {
                            var dis = this.arrCGC[i];
                            dis.push(i); // add index # in array, use for editing
                            if (dis[12] != "Remove") {
                                listCGC.append($('<li class="list-group-item">')
                                    .data('data', dis)
                                    .append($('<div>').html(dis[7] + '<br>'))
                                    .append($('<div>').html('Amount: ' + (dis[11] == 0 ? '0.00' : AccountingFormat(dis[11]))))
                                    .click(function () {
                                        CacPopup.ChequeGC.Edit($(this).data('data'));
                                    })
                                )
                                isEmpty = false; // set to false when there's no remove state in arrCGC, found in index 13, w/c means add, edit, remove
                            }
                        }

                        listCGC.find('li').first().addClass('navList-a');
                    }

                    if (isEmpty) {
                        listCGC.append($('<li class="list-group-item">').text('List is empty'));
                    }

                    $('#pageListOfCGC').pageshow();
                    PopupReposition();
                    this.TotalCGC();
                },
                TotalCGC: function () {
                    if (this.arrCGC.length > 0) {
                        this.totalcgc = 0;
                        for (var i = 0; i < this.arrCGC.length; i++) {
                            if (this.arrCGC[i][12] != "Remove") {
                                this.totalcgc = this.totalcgc + CDec2(this.arrCGC[i][11]);
                            }
                        }
                        //$('.txtCGC').text((this.totalcgc > 0 ? AccountingFormat(this.totalcgc) : '0.00'))
                        CacPopup.params.success(this.totalcgc)
                    }
                    CacPopup.ComputePayment();
                }
            },
            Discount: {
                IsVat: null,
                selDiscountType: null,
                txtDiscountPercent: null,
                txtDiscountAmount: null,
                txtDiscountRemarks: null,
                txtSalesPayNoOfSC: null,
                txtSalesPayNoOfDiners: null,
                txtDiscountPercentCeiling: null,
                btnAddDiscountCustomer: null,
                btnDiscountOk: null,
                listDiscountCustomer: null,
                form: null,
                popup: null,
                isDiscountTypeLoaded: false,
                isSeniorCitizenDiscount: false,
                showDiscountDetails: false,
                isSeniorCitizenPWDDiscount: false,
                longDiscountPercent: null,
                lblVATableSales: 0,
                dblTotalAmountDueForDiscount: 0,
                dblSubTotal: 0,
                taxPercent: 0,
                longChargeAmount: 0,
                IsDeliveryInfoPriceQuote: null,
                strAccessType: "",
                intSalesPayCustomerNameCntr: 0,
                intSalesPayCustomerNameIndex: 0,
                totaldiscount: 0,
                SubTotalOfVatExemptItems: 0,
                SCDiscountExempt: 0,
                GrossSalesToSC: 0,
                GrossSalesToNoneSC: 0,
                VatExemptSales: 0,
                DiscountedPriceAction: '',
                MaxSCDiscountableAmount: 0,
                seniorCitizenDiscount: 0,
                seniorCitizenNetAmount: 0,
                PhilippineTeamDiscount: 0,
                PhilippineTeamNetAmount: 0,
                IsApplyMaxSCDiscountableAmount: false,
                Clear: function () {
                    this.isSeniorCitizenDiscount = false;
                    this.showDiscountDetails = false;
                    this.isSeniorCitizenPWDDiscount = false;
                    this.longDiscountPercent = null;
                    this.lblVATableSales = 0;
                    this.dblTotalAmountDueForDiscount = 0;
                    this.dblSubTotal = 0;
                    this.taxPercent = 0;
                    this.longChargeAmount = 0;
                    this.IsDeliveryInfoPriceQuote = null;
                    this.strAccessType = '';
                    this.intSalesPayCustomerNameCntr = 0;
                    this.intSalesPayCustomerNameIndex = 0;
                    this.totaldiscount = 0;
                    this.SubTotalOfVatExemptItems = 0;
                    this.SCDiscountExempt = 0;
                    this.GrossSalesToSC = 0;
                    this.GrossSalesToNoneSC = 0;
                    this.VatExemptSales = 0;
                    this.DiscountedPriceAction = '';
                    this.MaxSCDiscountableAmount = 0;
                    this.seniorCitizenDiscount = 0;
                    this.seniorCitizenNetAmount = 0;
                    this.PhilippineTeamDiscount = 0;
                    this.PhilippineTeamNetAmount = 0;
                    this.IsApplyMaxSCDiscountableAmount = false;

                    if (CBool(sessionStorage.IsWebPos)) {
                        $('#webPosCartVATPercent, #webPosCartTotalVATExAmount, #webPosCartTotalDiscount, #lblSalesPayCharges').text('0.00');
                        $('#rowVATPercent, #rowAmountLessVAT').hide();
                    }
                },
                init: function (params) {
                    var dis = this;
                    this.selDiscountType = $('[name="selDiscountType"]').unbind();
                    this.txtDiscountPercent = $('[name="txtDiscountPercent"]');
                    this.txtDiscountAmount = $('[name="txtDiscountAmount"]');
                    this.txtDiscountRemarks = $('[name="txtDiscountRemarks"]');
                    this.txtSalesPayNoOfSC = $('[name="txtSalesPayNoOfSC"]');
                    this.txtSalesPayNoOfDiners = $('[name="txtSalesPayNoOfDiners"]');
                    this.txtDiscountPercentCeiling = $('[name="txtDiscountPercentCeiling"]');
                    this.btnAddDiscountCustomer = $('[name="btnAddDiscountCustomer"]').unbind();
                    this.listDiscountCustomer = $('[name="listDiscountCustomer"]');
                    this.btnDiscountOk = $('[name="btnDiscountOk"]').unbind();
                    this.popup = (proj.IsBootstrapDesign() ? $('#popFMCGDiscount-bs') : $('#popFMCGDiscount'));

                    if (proj.IsBootstrapDesign()) {
                        this.popup.modal('show');
                    } else {
                        this.popup.popup('open');
                    }

                    if (this.SubTotalOfVatExemptItems == 0) {
                        this.selDiscountType.val('').focus();
                        this.txtDiscountPercent.val('');
                        this.txtDiscountAmount.val('');
                        this.txtDiscountRemarks.val('');
                        this.txtSalesPayNoOfSC.val(1);
                        this.txtSalesPayNoOfDiners.val(1);
                        this.listDiscountCustomer.empty();
                    }                    

                    BindKeypress('#formFMCGDiscounts', this.selDiscountType, false, 'select');
                    BindKeypress('#formFMCGDiscounts', this.txtDiscountPercent, true, 'text');
                    BindKeypress('#formFMCGDiscounts', this.txtDiscountAmount, true, 'text');
                    BindKeypress('#formFMCGDiscounts', this.txtDiscountRemarks, false, 'text');
                    BindKeypress('#formFMCGDiscounts', this.txtSalesPayNoOfSC, true, 'text');
                    BindKeypress('#formFMCGDiscounts', this.txtSalesPayNoOfDiners, true, 'text');

                    if (this.isDiscountTypeLoaded == false) {
                        $('#divDiscountCustomerDetails').hide();
                        $('#divSCPWDDiscount').hide();

                        this.txtDiscountPercent.removeAttr("disabled");
                        this.txtDiscountAmount.removeAttr("disabled");
                        this.GetDiscountTypeList();
                        //this.GetCurrentDiscount();
                        this.isDiscountTypeLoaded = true;
                    }

                    this.ShowSalesPaymentsInfo();

                    this.selDiscountType.change(function (e) {
                        //if (e.which == 13) {
                            CacPopup.Discount.GetSelectedDiscount($.trim($(this).val()));
                            CacPopup.Discount.GetDiscountOnChange();
                            //PopupReposition(); // NewMessageBox.js
                        //}
                    })

                    this.btnDiscountOk.click(function (e) {
                        e.preventDefault();
                        if (dis.selDiscountType.val() == '') {
                            params.success({
                                DiscountType: '',
                                Percent: '',
                                Amount: '',
                                Remarks: '',
                                Customer: []
                            })
                            dis.Clear();
                            ERP.Payment.controller.ComputeTotalPaymentRcv();
                            dis.popup.modal('hide');
                            return;
                        }

                        if (dis.txtDiscountPercent.val() == '') {
                            NewMessageBox('Please enter percent discount');
                            return;
                        }

                        if (dis.txtDiscountAmount.val() == '') {
                            NewMessageBox('Please enter discount amount');
                            return;
                        }

                        if (dis.listDiscountCustomer.find('li').length == 0 || CDec2(dis.txtSalesPayNoOfSC.val()) > dis.listDiscountCustomer.find('li').length) {
                            NewMessageBox('Please enter ' + CDec2(dis.txtSalesPayNoOfSC.val()) + ' SCs/PWDs.');
                            return;
                        }

                        var arrCustomers = [];

                        dis.listDiscountCustomer.find('li').each(function () {
                            arrCustomers.push($(this).data('data'));
                        })

                        params.success({
                            DiscountType: dis.selDiscountType.val(),
                            Percent: CDec2(dis.txtDiscountPercent.val()),
                            Amount: CDec2(dis.txtDiscountAmount.val()),
                            Remarks: dis.txtDiscountRemarks.val(),
                            Customer: arrCustomers
                        })
                        dis.popup.modal('hide');
                    })

                    this.txtDiscountPercent.unbind().keydown(function (e) {
                        if (e.which == 107) {
                            // press + in numpad
                        }
                    });

                    this.btnAddDiscountCustomer.unbind().click(function () {
                        CacPopup.Discount.Customer.init();
                    })

                    AjaxSend({
                        url: 'Browser/LibraryFowstAll',
                        success: function (e) {
                            dis.IsVat = e.Results.Rows[0].IsVat;
                        }
                    })
                },
                ShowSalesPaymentsInfo: function () {
                    var dis = this;
                    AjaxSend({
                        url: '/SalesMenu/ShowSalesPaymentsInfo',
                        data: { InvoiceID: ERP.Payment.controller.WebPosInvoiceID },
                        success: function (e) {
                            dis.LoadSalesPaymentsInfo(e.Results.Rows[0]);
                        }
                    })                    
                },
                LoadSalesPaymentsInfo: function (vData) {
                    if (vData != undefined) {
                        this.dblTotalAmountDueForDiscount = 0;
                        this.dblTotalAmountDueForDiscount = vData.TotalAmountDue || 0;
                        this.dblSubTotal = vData.TransInvoiceItems || 0;
                        this.taxPercent = vData.TaxPercent || 0;
                        this.longDiscountPercent = vData.TransInvoiceItems;
                        this.IsDeliveryInfoPriceQuote = false;

                        this.RefreshSalesPaymentTotal();
                        this.RefreshTotalDiscount();
                    }
                },
                GetSelectedDiscount: function (discounttype) {
                    if (this.selDiscountType.val().toUpperCase() == '35F954A0-C67C-499C-AF60-F08E64477FDC') {
                        this.strAccessType = "Allow to grant VIP Discount";
                        this.showDiscountDetails = false;
                        this.isSeniorCitizenPWDDiscount = false;
                        this.isSeniorCitizenDiscount = false;
                    } else if (this.selDiscountType.val().toUpperCase() == '764EDACB-0131-4033-8E44-7B903368008C') {
                        this.strAccessType = "Allow to grant Regular Discount 1";
                        this.showDiscountDetails = false;
                        this.isSeniorCitizenPWDDiscount = false;
                        this.isSeniorCitizenDiscount = false;
                    } else if (this.selDiscountType.val().toUpperCase() == 'C4199347-6A00-45C9-BE11-8934DD63ECD1') {
                        this.strAccessType = "Allow to grant Regular Discount 2";
                        this.showDiscountDetails = false;
                        this.isSeniorCitizenPWDDiscount = false;
                        this.isSeniorCitizenDiscount = false;
                    } else if (this.selDiscountType.val().toUpperCase() == '8E2DCA9C-433F-42FC-A644-04476DBA056A') {
                        this.strAccessType = "Allow to grant Regular Discount 3";
                        this.showDiscountDetails = false;
                        this.isSeniorCitizenPWDDiscount = false;
                        this.isSeniorCitizenDiscount = false;
                    } else if (this.selDiscountType.val().toUpperCase() == '7804A35A-B531-4380-945E-FC9643E99843') {
                        this.strAccessType = "Allow to grant Employee Discount";
                        this.showDiscountDetails = false;
                        this.isSeniorCitizenPWDDiscount = false;
                        this.isSeniorCitizenDiscount = false;
                    } else if (this.selDiscountType.val().toUpperCase() == '055A0A33-DD98-47EC-ADD3-8B2009F3944A') {
                        this.strAccessType = "Allow to grant Senior Citizens' (SC) Discount";
                        $('#divDiscountCustomerDetails').show();
                        $('#divSCPWDDiscount').show();
                        this.showDiscountDetails = true;
                        this.isSeniorCitizenPWDDiscount = true;
                        this.isSeniorCitizenDiscount = true;
                        this.txtDiscountPercent.attr("disabled", "disabled");
                        this.txtDiscountAmount.attr("disabled", "disabled");
                    } else if (this.selDiscountType.val().toUpperCase() == '34A4032F-A142-4C15-A868-4AC4410434DF') {
                        this.strAccessType = "Allow to grant Person With Disability (PWD) Discount";
                        $('#divDiscountCustomerDetails').show();
                        this.isSeniorCitizenPWDDiscount = true;
                        this.showDiscountDetails = false;
                        this.isSeniorCitizenDiscount = false;
                        this.txtDiscountPercent.attr("disabled", "disabled");
                        this.txtDiscountAmount.attr("disabled", "disabled");
                    } else if (this.selDiscountType.val().toUpperCase() == '674AD791-188B-4238-B95C-97B5C2B9D544') {
                        this.strAccessType = "Allow to grant Senior Citizens' (SC) Discount";
                        $('#divDiscountCustomerDetails').show();
                        this.isSeniorCitizenPWDDiscount = true;
                        this.isSeniorCitizenDiscount = true;
                        this.showDiscountDetails = false;
                        this.txtDiscountPercent.attr("disabled", "disabled");
                        this.txtDiscountAmount.attr("disabled", "disabled");
                    } else if (this.selDiscountType.val().toUpperCase() == 'C2218C86-0450-4CD3-B019-3EDD43E51741') {
                        this.strAccessType = "Allow to grant GPC Discount";
                        this.showDiscountDetails = false;
                        this.isSeniorCitizenPWDDiscount = false;
                        this.isSeniorCitizenDiscount = false;
                    } else {
                        this.strAccessType = "";
                        this.showDiscountDetails = false;
                        this.isSeniorCitizenPWDDiscount = false;
                        this.isSeniorCitizenDiscount = false;
                    }
                },
                GetDiscountOnChange: function () {
                    var dis = this;
                    if (this.strAccessType == "") {
                        this.selDiscountType.val('');
                        this.txtDiscountPercent.val("0.00");
                        this.txtDiscountAmount.val("0.00");

                        this.GetOrgDiscountPercentRefresh(this.selDiscountType.val());
                    } else {
                        PromptUserAccess(this.strAccessType, {
                            OnClickOKSuccess: function () {
                                CacPopup.Discount.GetOrgDiscountPercentRefresh(CacPopup.Discount.selDiscountType.val());
                            },
                            OnFailed: function () {
                                dis.selDiscountType.val('');
                                dis.txtDiscountPercent.val("0.00");
                                dis.txtDiscountAmount.val("0.00");
                                CacPopup.Discount.GetOrgDiscountPercentRefresh(CacPopup.Discount.selDiscountType.val());
                            }
                        });
                    }
                },
                GetCurrentDiscount: function () {
                    if (OfflineMode) {
                        var discount = SalesPayment_offline().getCurrentDiscount(sessionStorage.CartOrderID);
                        if (discount.Rows != undefined) {
                            if (discount.Rows.length > 0) {
                                this.selDiscountType.val(discount.Rows[0].DiscountTypeID);
                                this.txtDiscountPercent.val(discount.Rows[0].DiscountPercentage);
                                this.txtDiscountRemarks.val(discount.Rows[0].DiscountRemarks);
                                this.txtDiscountPercentCeiling.val(discount.Rows[0].PercentDiscount);

                                $('#divAddCustomerDetails').hide();
                                $('#divSCPWDDiscount').hide();

                                this.txtDiscountPercent.removeAttr("disabled");
                                this.txtDiscountAmount.removeAttr("disabled");

                                this.GetSelectedDiscount();

                                if (this.isSeniorCitizenPWDDiscount) {
                                    var vDataShow = SalesPayment_offline().getSeniorCitizenPWDCustomer(sessionStorage.CartOrderID);
                                    //put the ids in sessionstorage

                                    if (vDataShow.Rows.length > 0) {

                                        arSalesPayCustomerName = new Array();
                                        for (var i = 0; i < vDataShow.Rows.length; i++) {
                                            arSalesPayCustomerName[intSalesPayCustomerNameID] = vDataShow.Rows[i].CustomerID;
                                            arSalesPayCustomerName[intSalesPayCustomerNameState] = "Edit";
                                            sessionStorage["SalesPayCustomerName-" + i] = JSON.stringify(arSalesPayCustomerName);

                                        }

                                        this.intSalesPayCustomerNameCntr = window.pageDiscounts.LoadSavedCustomerNameList(vDataShow.Rows, true);

                                    }


                                }

                                this.longDiscountPercent = discount.Rows[0].DiscountPercentage;
                            }

                        }
                    }
                },
                GetOrgDiscountPercentRefresh: function (strDiscountID) {
                    var dis = this;

                    AjaxSend({
                        url: 'DiscountSettings/getDiscountSettingsInfo',
                        data: { DiscountID: strDiscountID },
                        success: function (e) {
                            var vRows = e.DiscountData.Rows;
                            if (vRows.length > 0) {
                                dis.LoadOrgDiscountPercentRefresh(vRows[0]);
                            } else {
                                $('#txtDiscountPercentCeiling').val('');
                                $('#txtDiscountPercent').val('');
                                $("#txtDiscountAmount").val('');
                                $("#lblSalesPayTotalDiscount").text('');
                                $("#lblSalesPayCheckTotalDiscount").text('');
                                $("#lblSalesPayDebitCardTotalDiscount").text('');
                                $("#lblSalesPayCreditCardTotalDiscount").text('');
                                $("#lblSalesPaySRVTotalDiscount").text('');
                                $("#lblSalesPayOtherTypeTotalDiscount").text('');
                                RefreshSalesPaymentTotal();
                            }
                        }
                    })
                },
                LoadOrgDiscountPercentRefresh: function (vRows) {
                    this.txtDiscountPercentCeiling.val((vRows.PercentDiscount == "0" ? '' : AccountingFormat(vRows.PercentDiscount)))
                    this.txtDiscountPercent.val((vRows.PercentDiscount == "0" ? '' : AccountingFormat(vRows.PercentDiscount)))
                    this.longDiscountPercent = vRows.PercentDiscount;
                    this.RefreshTotalDiscount();
                },
                RefreshTotalDiscount: function () {
                    var dis = this;
                    var dblDiscount = 0;
                    var dblTotalAmount = 0;

                    if (dis.selDiscountType.val() == '') {

                        $('#rowVATPercent').hide();
                        $('#rowAmountLessVAT').hide();

                        $('#lblVATAmount').text('');
                        $('#lblTotalAmount').text('');

                        this.txtDiscountPercentCeiling.val('');
                        this.txtDiscountPercent.val('');
                        this.txtDiscountAmount.val('');

                    } else {
                        AjaxSend({
                            url: '/DiscountSettings/GetSSDItemsWithVatExempt',
                            data: { InvoiceID: ERP.Payment.controller.WebPosInvoiceID, FowstID: sessionStorage.FowstID },
                            success: function (e) {
                                if (e.Error == undefined) {
                                    dis.SubTotalOfVatExemptItems = e.Total;
                                    dis.DiscountedPriceAction = e.DiscountedPriceAction;

                                    dblTotalAmount = CDec2(dis.SubTotalOfVatExemptItems) / (1 + (CDec2(dis.taxPercent) / 100));

                                    if (dis.isSeniorCitizenDiscount) {
                                        if (CDec2(sessionStorage.AdditionalMaxAllowableDiscount) > 0) {
                                            dis.SubTotalOfVatExemptItems += CDec2(sessionStorage.AdditionalMaxAllowableDiscount);
                                        }

                                        dis.SCDiscountExempt = CDec2(dis.dblSubTotal) - CDec2(dis.SubTotalOfVatExemptItems);

                                        dblTotalAmount = dis.isSeniorCitizenPWDDiscount ? CDec2(dis.SubTotalOfVatExemptItems) / (1 + (CDec2(dis.taxPercent) / 100)) : CDec2(dis.dblSubTotal);

                                        $('#rowVATPercent').show();
                                        $('#rowAmountLessVAT').show();
                                        $('#rowVATBackPercent').hide();

                                        $('#lblVATPercentValue').text(dis.taxPercent);
                                        $('#webPosCartVATPercent').text(AccountingFormat(CDec2(dis.SubTotalOfVatExemptItems) - CDec2(dblTotalAmount)));

                                        if (CDec2(dis.txtSalesPayNoOfSC.val()) > 0 && CDec2(dis.txtSalesPayNoOfDiners.val()) > 0) {

                                            var proportionOfNoOfSeniors = parseFloat(dis.txtSalesPayNoOfSC.val()) / parseFloat(dis.txtSalesPayNoOfDiners.val());

                                            dis.GrossSalesToSC = proportionOfNoOfSeniors * dis.SubTotalOfVatExemptItems;

                                            var proportionofregular = (parseFloat(dis.txtSalesPayNoOfDiners.val()) - parseFloat(dis.txtSalesPayNoOfSC.val())) / parseFloat(dis.txtSalesPayNoOfDiners.val());
                                            dis.GrossSalesToNoneSC = proportionofregular * dis.SubTotalOfVatExemptItems;

                                            dis.VatExemptSales = dis.GrossSalesToSC / (1 + (CDec2(dis.taxPercent) / 100));

                                            if (dis.DiscountedPriceAction == 'Round Up' || dis.DiscountedPriceAction == 'Round Down') {
                                                //round off foregone vat and recompute VatExemptSales
                                                var ForeGoneVat = CDec2(dis.GrossSalesToSC) - CDec2(dis.VatExemptSales);

                                                ForeGoneVat = Math.round(ForeGoneVat);

                                                dis.VatExemptSales = dis.GrossSalesToSC - ForeGoneVat;
                                            }

                                            var seniorCitizenDiscountRate = parseFloat(dis.txtDiscountPercentCeiling.val()) * 0.01;

                                            if (dis.IsVAT != undefined) {
                                                if (!CBool(dis.IsVAT)) {
                                                    dis.VatExemptSales = dis.GrossSalesToSC;
                                                }
                                            }

                                            //MaxSCDiscountableAmount = getMaxSCDiscountableAmount() + CDec2(sessionStorage.AdditionalMaxAllowableDiscount);
                                            //MaxSCDiscountableAmount = MaxSCDiscountableAmount * CInt2($.trim($("#txtSalesPayNoOfSC").val()));

                                            //if(MaxSCDiscountableAmount != 0){
                                            //    if (MaxSCDiscountableAmount < VatExemptSales) {
                                            //        IsApplyMaxSCDiscountableAmount = true;
                                            //    }
                                            //}

                                            dis.seniorCitizenDiscount = (dis.IsApplyMaxSCDiscountableAmount == true ? dis.MaxSCDiscountableAmount : dis.VatExemptSales) * seniorCitizenDiscountRate;

                                            if (dis.DiscountedPriceAction == 'Round Up') {
                                                dis.seniorCitizenDiscount = Math.ceil(dis.seniorCitizenDiscount);
                                            } else if (dis.DiscountedPriceAction == 'Round Down') {
                                                dis.seniorCitizenDiscount = Math.floor(dis.seniorCitizenDiscount);
                                            }

                                            //VatExToSC should always be the Gross VAT-Ex Amount(variable VatExToSC) even if MaxSCDiscountableAmount is present
                                            dis.seniorCitizenNetAmount = dis.VatExemptSales - dis.seniorCitizenDiscount;

                                            //var totalBillToBePayed = seniorCitizenNetAmount + dis.GrossSalesToNoneSC;

                                            var weightedSCDiscountRate = proportionOfNoOfSeniors * seniorCitizenDiscountRate;

                                            //var totalBillBeforeSCDiscount = parseFloat($('#lblSalesPayAmountDue').val());

                                            //var totalBillToBePayed = weightedSCDiscountRate * totalBillBeforeSCDiscount;

                                            dis.GetInvoiceCharges(function (charge) {
                                                var AmountDueBeforeCharges = 0;
                                                var AmountDueNetofCharges = 0;

                                                AmountDueBeforeCharges = CDec2(dis.GrossSalesToNoneSC + dis.VatExemptSales + dis.SCDiscountExempt); //comment 06/28/17 change var SCDiscountExempt to seniorCitizenDiscount to fix charge issue
                                                //AmountDueBeforeCharges = CDec2(dis.GrossSalesToNoneSC + dis.VatExemptSales + dis.SCDiscountExempt - dis.seniorCitizenDiscount);
                                                AmountDueNetofCharges = CDec2(AmountDueBeforeCharges) + CDec2(charge); //comment 06/28/17 remove charges since it will be calculated in SCD page

                                                //gibalhin ang lblVATPercent dire para magamit sa amtDue na variable
                                                $('#lblVATPercent').text(AccountingFormat(CDec2(dis.GrossSalesToSC) - CDec2(dis.VatExemptSales)));

                                                var amtDue = dis.isSeniorCitizenPWDDiscount ? parseFloat($('#lblSalesPaySubTotal').text().replace(/,/g, "")) - parseFloat(CDec2($('#lblVATPercent').text().replace(/,/g, ""))) : $('#lblSalesPaySubTotal').text().replace(/,/g, "");


                                                dis.UpdateInvoiceForDiscount({
                                                    ID: ERP.Payment.controller.WebPosInvoiceID,
                                                    Vatable: CDec2((dis.GrossSalesToNoneSC / (1 + (CDec2(dis.taxPercent) / 100))) + (dis.SCDiscountExempt / (1 + (CDec2(dis.taxPercent) / 100)))),
                                                    VATExempt: CDec2(dis.VatExemptSales),
                                                    VAT: CDec2(((dis.GrossSalesToNoneSC + dis.SCDiscountExempt)) - ((dis.GrossSalesToNoneSC / (1 + (CDec2(dis.taxPercent) / 100))) + (dis.SCDiscountExempt / (1 + (CDec2(dis.taxPercent) / 100))))),
                                                    ForegoneVAT: CDec2(dis.GrossSalesToSC) - CDec2(dis.VatExemptSales),
                                                    SCDiscount: CDec2(dis.seniorCitizenDiscount),
                                                    TotalNonSCDiscount: 0,
                                                    AmountDueBeforeCharges: CDec2(AmountDueBeforeCharges),
                                                    AmountDueNetofCharges: CDec2(AmountDueNetofCharges)
                                                })

                                                $('#webPosCartTotalVATExAmount').text(AccountingFormat(CDec2(AmountDueNetofCharges)));

                                                dis.txtDiscountPercent.val(weightedSCDiscountRate * 100);

                                                dis.txtDiscountAmount.val(CDec2(dis.seniorCitizenDiscount));

                                                dis.longDiscountPercent = seniorCitizenDiscountRate * 100;

                                                dblDiscount = dis.txtDiscountAmount.val();

                                                $("#webPosCartTotalDiscount").attr('actualdisc', dblDiscount).text(AccountingFormat(dblDiscount));
                                                ERP.Payment.controller.ComputeTotalPaymentRcv();
                                            })
                                        }
                                    } else if (dis.selDiscountType.val().toUpperCase() == '21EBF55B-526C-4276-B3B7-0CBCA504F425') {
                                        $('#rowVATPercent').show();
                                        $('#rowAmountLessVAT').show();
                                        $('#rowVATBackPercent').show();
                                        $('#lblVATPercentValue').text(dis.taxPercent);

                                        var PhilippineTeamDiscountRate = parseFloat(dis.txtDiscountPercentCeiling.val()) * 0.01;

                                        dis.VatExemptSales = dis.SubTotalOfVatExemptItems / (1 + (CDec2(dis.taxPercent) / 100));

                                        dis.PhilippineTeamDiscount = dis.VatExemptSales * PhilippineTeamDiscountRate;

                                        dis.PhilippineTeamNetAmount = dis.VatExemptSales - dis.PhilippineTeamDiscount;

                                        dis.GetInvoiceCharges(function (charge) {
                                            var AmountDueBeforeCharges = 0;
                                            var AmountDueNetofCharges = 0;

                                            AmountDueBeforeCharges = CDec2(dis.VatExemptSales);
                                            AmountDueNetofCharges = CDec2(AmountDueBeforeCharges) + CDec2(charge);

                                            var avePerBill = (dis.PhilippineTeamNetAmount + CDec2(dis.SubTotalOfVatExemptItems) - CDec2(dblTotalAmount)) / dis.SubTotalOfVatExemptItems; // CDec2(SubTotalOfVatExemptItems) - CDec2(dblTotalAmount) = VAT

                                            dis.UpdateInvoiceItemsForDiscount({
                                                ID: ERP.Payment.controller.WebPosInvoiceID,
                                                DiscountPercentCeiling: dis.txtDiscountPercentCeiling.val(),
                                                AvePerBill: avePerBill
                                            })


                                            $('#lblVATPercent').text(AccountingFormat(CDec2(dis.SubTotalOfVatExemptItems) - CDec2(dblTotalAmount)));
                                            $('#lblVATBackPercent').text(AccountingFormat(CDec2(dis.SubTotalOfVatExemptItems) - CDec2(dblTotalAmount)));

                                            $('#webPosCartTotalVATExAmount').text(AccountingFormat(AmountDueNetofCharges));

                                            dis.txtDiscountPercent.val(PhilippineTeamDiscountRate * 100);

                                            dis.txtDiscountAmount.val(CDec2(dis.PhilippineTeamDiscount));

                                            dis.longDiscountPercent = PhilippineTeamDiscountRate * 100;

                                            dblDiscount = dis.txtDiscountAmount.val();

                                            $("#webPosCartTotalDiscount").attr('actualdisc', dblDiscount).text(AccountingFormat(dblDiscount));

                                            ERP.Payment.controller.ComputeTotalPaymentRcv();
                                        })
                                    } else {
                                        $('#rowVATPercent').hide();
                                        $('#rowAmountLessVAT').hide();
                                        $('#rowVATBackPercent').hide();

                                        $('#lblVATAmount').text('');
                                        $('#webPosCartTotalVATExAmount').text('');
                                        dis.txtDiscountAmount.val((CDec2(dis.longDiscountPercent) == "0" ? '' : AccountingFormat(CDec2($('#webPosCartSubTotal').text().replace(/,/g, "")) * (CDec2(dis.longDiscountPercent)) / 100)));


                                        dis.GetInvoiceCharges(function (charge) {
                                            var data = {
                                                ID: ERP.Payment.controller.WebPosInvoiceID,
                                                Vatable: (dis.dblSubTotal - (dis.dblSubTotal * (CDec2(dis.longDiscountPercent) / 100))) / (1 + (CDec2(dis.taxPercent) / 100)),
                                                VATExempt: 0,
                                                VAT: (dis.dblSubTotal - (dis.dblSubTotal * (CDec2(dis.longDiscountPercent) / 100))) - ((dis.dblSubTotal - (dis.dblSubTotal * (CDec2(dis.longDiscountPercent) / 100))) / (1 + (CDec2(dis.taxPercent) / 100))),
                                                ForegoneVAT: 0,
                                                SCDiscount: 0,
                                                TotalNonSCDiscount: 0,
                                                AmountDueBeforeCharges: (dis.dblSubTotal - (dis.dblSubTotal * (CDec2(dis.longDiscountPercent) / 100))),
                                                AmountDueNetofCharges: (dis.dblSubTotal - (dis.dblSubTotal * (CDec2(dis.longDiscountPercent) / 100))) + CDec2(charge)
                                            }

                                            if (dis.IsVAT != undefined) {
                                                if (!CBool(dis.IsVAT)) {
                                                    data = {
                                                        ID: ERP.Payment.controller.WebPosInvoiceID,
                                                        Vatable: (dis.dblSubTotal - (dis.dblSubTotal * (CDec2(dis.longDiscountPercent) / 100))) / (1 + (CDec2(dis.taxPercent) / 100)),
                                                        VATExempt: 0,
                                                        VAT: (dis.dblSubTotal - (dis.dblSubTotal * (CDec2(dis.longDiscountPercent) / 100))) - ((dis.dblSubTotal - (dis.dblSubTotal * (CDec2(dis.longDiscountPercent) / 100))) / (1 + (CDec2(dis.taxPercent) / 100))),
                                                        ForegoneVAT: 0,
                                                        SCDiscount: 0,
                                                        TotalNonSCDiscount: 0,
                                                        AmountDueBeforeCharges: (dis.dblSubTotal - (dis.dblSubTotal * (CDec2(dis.longDiscountPercent) / 100))),
                                                        AmountDueNetofCharges: (dis.dblSubTotal - (dis.dblSubTotal * (CDec2(dis.longDiscountPercent) / 100))) + CDec2(charge)
                                                    }

                                                }
                                            }

                                            dis.UpdateInvoiceForDiscount(data);

                                            dblDiscount = dis.txtDiscountAmount.val();

                                            $("#webPosCartTotalDiscount").attr('actualdisc', dblDiscount).text(AccountingFormat(dblDiscount));
                                            ERP.Payment.controller.ComputeTotalPaymentRcv();
                                        })
                                    }                                   
                                }
                            }
                        })
                    }
                },
                RefreshSalesPaymentTotal: function () {
                    var vDataFooter;
                    if (OfflineMode) {
                        vDataFooter = SalesPayment_offline().getSalesPaymentReceiptFooterAmountsInfo(sessionStorage.CartOrderID);
                        this.ShowSalesPaymentTotalResults(vDataFooter);
                    }
                },
                ShowSalesPaymentTotalResults: function (vDataFooter) {
                    var dblTotalSalesPay = 0;
                    var dblTotalAmountDue = 0;
                    var dblDiscount = 0;
                    var dblCharges = 0;
                    var dblTip = 0;
                    var dblLocalTaxAmount = 0;
                    var cashAmountTendered = parseFloat(CDec2($('#lblSalesPayTotalCash').text().replace(/,/g, "")));


                    if (vDataFooter.Rows.length > 0) {

                        dblDiscount = vDataFooter.Rows[0].DiscountAmount;
                        dblCharges = CDec2(vDataFooter.Rows[0].ChargeAmount);
                        dblTip = CDec2(vDataFooter.Rows[0].TipAmount);
                        dblLocalTaxAmount = CDec2(vDataFooter.Rows[0].LocalTaxAmount);
                    }

                    //Charge
                    //$("#lblSalesPayCharges").text(dblCharges > 0 ? dblCharges : AccountingFormat(dblCharges));

                    if (dblTip > 0) {
                        $("#divPaymentsTip").show();
                        $("#lblSalesPayTip").text(dblTip > 0 ? dblTip : AccountingFormat(dblTip));
                    } else {
                        $("#divPaymentsTip").hide();
                        $("#lblSalesPayTip").text('');

                    }

                    if (dblLocalTaxAmount > 0) {
                        $("#divPaymentsOtherTax").show();
                        $("#lblSalesPayOtherTax").text(dblLocalTaxAmount > 0 ? dblLocalTaxAmount : AccountingFormat(dblLocalTaxAmount));

                    } else {
                        $("#divPaymentsOtherTax").hide();
                        $("#lblSalesPayOtherTax").text('');
                    }

                    //$("#lblSalesPayCheckCharges").text(dblCharges + dblTip + dblLocalTaxAmount > 0 ? dblCharges + dblTip + dblLocalTaxAmount : AccountingFormat(dblCharges + dblTip + dblLocalTaxAmount));
                    //$("#lblSalesPayCreditCardCharges").text(dblCharges + dblTip + dblLocalTaxAmount > 0 ? dblCharges + dblTip + dblLocalTaxAmount : AccountingFormat(dblCharges + dblTip + dblLocalTaxAmount));
                    //$("#lblSalesPayDebitCardCharges").text(dblCharges + dblTip + dblLocalTaxAmount > 0 ? dblCharges + dblTip + dblLocalTaxAmount : AccountingFormat(dblCharges + dblTip + dblLocalTaxAmount));
                    //$("#lblSalesPaySRVCharges").text(dblCharges + dblTip + dblLocalTaxAmount > 0 ? dblCharges + dblTip + dblLocalTaxAmount : AccountingFormat(dblCharges + dblTip + dblLocalTaxAmount));
                    //$("#lblSalesPayOtherTypeCharges").text(dblCharges + dblTip + dblLocalTaxAmount > 0 ? dblCharges + dblTip + dblLocalTaxAmount : AccountingFormat(dblCharges + dblTip + dblLocalTaxAmount));



                    //                    }

                    var amtDue = this.isSeniorCitizenDiscount ? $('#lblTotalAmount').text().replace(/,/g, "") : $('#lblSalesPaySubTotal').text().replace(/,/g, "");


                    dblTotalAmountDue = parseFloat(CDec2(amtDue)) - parseFloat(CDec2($("#lblSalesPayTotalDiscount").text())) + parseFloat(CDec2($('#lblSalesPayCharges').text().replace(/,/g, ""))) + parseFloat(CDec2($('#lblSalesPayTip').text().replace(/,/g, "")));
                    dblTotalSalesPay = cashAmountTendered + parseFloat(CDec2($('#lblSalesPayTotalCheck').text().replace(/,/g, ""))) + parseFloat(CDec2($('#lblSalesPayTotalDebitCard').text().replace(/,/g, ""))) + parseFloat(CDec2($('#lblSalesPayTotalCreditCard').text().replace(/,/g, ""))) + parseFloat(CDec2($('#lblSalesPayTotalSRV').text().replace(/,/g, ""))) + parseFloat(CDec2($('#lblSalesPayTotalOtherType').text().replace(/,/g, ""))) - parseFloat(CDec2(dblTotalAmountDue));

                    var totalAmountTendered = parseFloat(CDec2($('#lblSalesPayTotalAmountTendered').text().replace(/,/g, "")));

                    if (dblTotalSalesPay >= 0) {
                        $("#lblSalesPayAmountLacking").text("Complete Payment");
                        $("#lblSalesPayTotalChangeAmount").text(AccountingFormat(dblTotalSalesPay));

                        //check for cash payments only. other types of payments dont have change! if they paid greater
                        //than the total amount due the change should be 0
                        if ((totalAmountTendered - dblTotalAmountDue) > 0) {
                            //if the cash amount tendered is greater than the change, then return the cash amount
                            if (dblTotalSalesPay > cashAmountTendered)
                                $("#lblSalesPayTotalChangeAmount").text(AccountingFormat(cashAmountTendered));
                            else if (cashAmountTendered > 0)
                                $("#lblSalesPayTotalChangeAmount").text(AccountingFormat(dblTotalSalesPay));
                            else
                                $("#lblSalesPayTotalChangeAmount").text("No Change");
                        } else
                            $("#lblSalesPayTotalChangeAmount").text(AccountingFormat(dblTotalSalesPay));
                    } else {
                        $("#lblSalesPayAmountLacking").text(AccountingFormat(-dblTotalSalesPay));
                        $("#lblSalesPayTotalChangeAmount").text("No Change");
                    }
                },
                GetDiscountTypeList: function () {
                    var vRows, dis = this;

                    function Display() {
                        dis.selDiscountType.empty();

                        dis.selDiscountType.append(new Option('Please Select', ''));
                        for (var p = 0; p <= vRows.length - 1; p++) {
                            if (CDec2(vRows[p].PercentDiscount) != 0) {
                                dis.selDiscountType.append(new Option(vRows[p].DiscountType, vRows[p].ID, false, false));
                            }
                        }
                    }

                    if (OfflineMode) {
                        vRows = SalesPayment_offline().getOrgDiscountType().Rows;
                        Display();
                    } else {
                        AjaxSend({
                            url: '/Browser/LibraryDiscountSettings',
                            success: function (e) {
                                vRows = e.Results.Rows;
                                Display();
                            }
                        })
                    }
                },
                Customer: {
                    txtCustomerPersonID: null,
                    txtCustomerFullname: null,
                    txtCustomerBirthday: null,
                    listDiscountCustomerID: null,
                    btnOkDiscountCustomer: null,
                    popup: null,
                    init: function (existingData) {
                        var dis = this;
                        this.txtCustomerPersonID = $('[name="txtCustomerPersonID"]');
                        this.txtCustomerFullname = $('[name="txtCustomerFullname"]');
                        this.txtCustomerBirthday = $('[name="txtCustomerBirthday"]');
                        this.btnAddDiscountCustomerID = $('[name="btnAddDiscountCustomerID"]').unbind();
                        this.listDiscountCustomerID = $('[name="listDiscountCustomerID"]');
                        this.btnOkDiscountCustomer = $('[name="btnOkDiscountCustomer"]').unbind();
                        this.popup = $('#popFMCGDiscountCustomerDetails');

                        this.listDiscountCustomerID.empty();

                        if (existingData == undefined) {
                            this.txtCustomerPersonID.val('');
                            this.txtCustomerFullname.val('');
                            this.txtCustomerBirthday.val('');
                        } else {
                            this.txtCustomerPersonID.val(existingData.LineID);
                            this.txtCustomerFullname.val(existingData.FullName);
                            this.txtCustomerBirthday.val($.format.date(new Date(existingData.Birthday), 'yyyy-MM-dd'));

                            for (var i in existingData.ValidIDs) {
                                this.ID.DisplayIDs(existingData.ValidIDs[i]);
                            }
                        }

                        this.popup.modal('show');

                        this.btnAddDiscountCustomerID.click(function () {
                            CacPopup.Discount.Customer.ID.init();
                        })

                        this.btnOkDiscountCustomer.click(function () {
                            if (CStr2(dis.txtCustomerFullname.val()) == '') {
                                NewMessageBox('Please enter customer\'s full name.');
                                return;
                            }

                            if (CStr2(dis.txtCustomerBirthday.val()) == '') {
                                NewMessageBox('Please enter customer\'s birthday.');
                                return;
                            }

                            if (dis.listDiscountCustomerID.find('li').length == 0) {
                                NewMessageBox('Please add at least one valid ID.');
                                return;
                            }

                            var arrIDs = [];

                            CacPopup.Discount.Customer.listDiscountCustomerID.find('li').each(function () {
                                arrIDs.push($(this).data('data'));
                            })

                            dis.DisplayCustomers({
                                LineID: dis.txtCustomerPersonID.val(),
                                FullName: dis.txtCustomerFullname.val(),
                                Birthday: dis.txtCustomerBirthday.val(),
                                ValidIDs: arrIDs
                            })

                            dis.popup.modal('hide');
                        })
                    },
                    DisplayCustomers: function (data) {
                        var txt = data.FullName;

                        if (!IsGuid(data.LineID)) data.LineID = NewID();

                        if ($('li[lineid="' + data.LineID + '"]').length == 0) {
                            CacPopup.Discount.listDiscountCustomer.append($('<li class="list-group-item">')
                                .attr('lineid', data.LineID)
                                .data('data', data)
                                .append($('<div class="row align-items-center">')
                                    .append($('<div class="col lbl">').text(txt))
                                    .append($('<div class="col-auto">')
                                        .append($('<button data-role="none" class="btn btn-danger">')
                                            .append($('<span class="bi-trash">'))
                                            .click(function (e) {
                                                e.stopPropagation();
                                                e.preventDefault();
                                                $(this).parents().closest('li').remove();
                                            })
                                        )
                                    )
                                )
                                .click(function () {
                                    CacPopup.Discount.Customer.init($(this).data('data'));
                                })
                            )
                        } else {
                            $('li[lineid="' + data.LineID + '"]').data('data', data).find('.lbl').text(txt);
                        }
                    },
                    ID: {
                        txtCustomerPersonID: null,
                        selIDType: null,
                        txtOtherIDType: null,
                        txtValidIDNo: null,
                        btnOkValidID: null,
                        popup: null,
                        init: function (existingData) {
                            var dis = this;
                            this.txtCustomerPersonID = $('[name="txtCustomerPersonID"]').unbind();
                            this.selIDType = $('[name="selIDType"]').unbind();
                            this.txtOtherIDType = $('[name="txtOtherIDType"]').unbind();
                            this.txtValidIDNo = $('[name="txtValidIDNo"]').unbind();
                            this.btnOkValidID = $('[name="btnOkValidID"]').unbind();
                            this.popup = $('#popFMCGDiscountCustomerDetailsID');

                            if (existingData == undefined) {
                                this.txtCustomerPersonID.val('');
                                this.selIDType.val('');
                                this.txtOtherIDType.val('');
                                this.txtValidIDNo.val('');
                            } else {
                                this.txtCustomerPersonID.val(existingData.LineID);
                                this.selIDType.val(existingData.IDType);
                                this.txtOtherIDType.val(existingData.OtherIDType);
                                this.txtValidIDNo.val(existingData.ValidIDNo);
                            }                            

                            this.selIDType.change(function () {
                                dis.txtOtherIDType.val('');
                                if ($(this).val() == 'Other') {
                                    $('#divOtherTypeID').show();
                                } else $('#divOtherTypeID').hide();
                            })

                            this.btnOkValidID.click(function () {
                                if (dis.selIDType.val() == '') {
                                    NewMessageBox('Please select valid ID');
                                    return;
                                }

                                if (dis.selIDType.val() == 'other' && $.trim(dis.txtOtherIDType.val()) == '') {
                                    NewMessageBox('Please enter other valid ID');
                                    return;
                                }

                                if ($.trim(dis.txtValidIDNo.val()) == '') {
                                    NewMessageBox('Please enter valid ID number');
                                    return;
                                }

                                dis.popup.modal('hide');

                                dis.DisplayIDs({
                                    LineID: dis.txtCustomerPersonID.val(),
                                    IDType: dis.selIDType.val(),
                                    OtherIDType: dis.txtOtherIDType.val(),
                                    ValidIDNo: dis.txtValidIDNo.val()
                                })
                            })

                            this.popup.modal('show');
                        },
                        DisplayIDs: function (data) {
                            var txt = (data.OtherIDType || data.IDType) + ': ' + data.ValidIDNo;

                            if (!IsGuid(data.LineID)) data.LineID = NewID();

                            if ($('li[lineid="' + data.LineID + '"]').length == 0) {                                
                                CacPopup.Discount.Customer.listDiscountCustomerID.append($('<li class="list-group-item">')
                                    .attr('lineid', data.LineID)
                                    .data('data', data)
                                    .append($('<div class="row align-items-center">')
                                        .append($('<div class="col lbl">').text(txt))
                                        .append($('<div class="col-auto">')
                                            .append($('<button data-role="none" class="btn btn-danger">')
                                                .append($('<span class="bi-trash">'))
                                                .click(function (e) {
                                                    e.stopPropagation();
                                                    e.preventDefault();
                                                    $(this).parents().closest('li').remove();
                                                })
                                            )
                                        )
                                    )
                                    .click(function () {
                                        CacPopup.Discount.Customer.ID.init($(this).data('data'));
                                    })
                                )
                            } else {
                                $('li[lineid="' + data.LineID + '"]').data('data', data).find('.lbl').text(txt);
                            }
                        }
                    }
                },
                UpdateInvoiceForDiscount: function (data) {
                    AjaxSend({
                        url: 'SalesMenu/UpdateInvoiceForDiscount',
                        data: { data: data },
                        success: function (e) {
                            //do nothing
                        }
                    })
                },
                UpdateInvoiceItemsForDiscount: function (data) {
                    AjaxSend({
                        url: 'SalesMenu/UpdateInvoiceItemsForDiscount',
                        data: { data: data },
                        success: function (e) {
                            //do nothing
                        }
                    })
                },
                GetInvoiceCharges: function (callback) {
                    AjaxSend({
                        url: 'SalesMenu/GetInvoiceCharges',
                        data: { InvoiceID: ERP.Payment.controller.WebPosInvoiceID },
                        success: function (e) {
                            callback(e.Results)
                        }
                    })
                }
            },
            OtherPayment: {
                form: null,
                txtOPID: null,
                selOPPaymentType: null,
                txtOPAvailablePts: null,
                txtOPIssuingOrg: null,
                txtOPCardHolder: null,
                txtOPCardNo: null,
                txtOPApprovalNo: null,
                txtOPAmount: null,
                selOPIssuanceDateMonth: null,
                selOPIssuanceDateDay: null,
                selOPIssuanceDateYear: null,
                selOPExpirationDateMonth: null,
                selOPExpirationDateDay: null,
                selOPExpirationDateYear: null,
                btnOkOtherPaymentDetails: null,
                isFinishedRequiredData: false,
                totalop: 0,
                arrOP: [],
                editIndex: null,
                opState: 'New',
                Clear: function () {
                    this.totalop = 0;
                    this.arrOP = [];
                    this.editIndex = null;
                    this.opState = 'New';
                    CacPopup.popupname = '';
                    $('#listOP').empty().append('<li>List is empty</li>');
                },
                init: function (param) {
                    //if (proj.CacMode == 'AP') {
                    //    NewMessageBox('This function is not yet implemented.');
                    //    return;
                    //}
                    var dis = this;
                    this.txtOPID = $('[name="txtOPID"]');
                    this.selOPPaymentType = $('[name="selOPPaymentType"]').unbind();
                    this.txtOPAvailablePts = $('[name="txtOPAvailablePts"]');
                    this.txtOPIssuingOrg = $('[name="txtOPIssuingOrg"]');
                    this.txtOPCardHolder = $('[name="txtOPCardHolder"]');
                    this.txtOPCardNo = $('[name="txtOPCardNo"]');
                    this.txtOPApprovalNo = $('[name="txtOPApprovalNo"]');
                    this.txtOPAmount = $('[name="txtOPAmount"]');
                    this.txtOPIssuanceDate = $('[name="txtOPIssuanceDate"]');
                    this.txtOPExpirationDate = $('[name="txtOPExpirationDate"]');
                    this.selOPIssuanceDateMonth = $('[name="selOPIssuanceDateMonth"]');
                    this.selOPIssuanceDateDay = $('[name="selOPIssuanceDateDay"]');
                    this.selOPIssuanceDateYear = $('[name="selOPIssuanceDateYear"]');
                    this.selOPExpirationDateMonth = $('[name="selOPExpirationDateMonth"]');
                    this.selOPExpirationDateDay = $('[name="selOPExpirationDateDay"]');
                    this.selOPExpirationDateYear = $('[name="selOPExpirationDateYear"]');
                    this.btnOkOtherPaymentDetails = $('[name="btnOkOtherPaymentDetails"]').unbind();

                    // witholding tax
                    var pSelPaymentTypeBasisForGrossTaxableAmount = $('[name="selPaymentTypeBasisForGrossTaxableAmount"]');
                    var pTxtPaymentTypeGrossTaxableAmount = $('[name="txtPaymentTypeGrossTaxableAmount"]');
                    pTxtPaymentTypeGrossTaxableAmount.val(param.CurrentPaymentsAppliedToInvoices);
                    var pTxtPaymentTypeVatRate = $('[name="txtPaymentTypeVatRate"]');
                    var pTxtPaymentTypeNetTaxableAmount = $('[name="txtPaymentTypeNetTaxableAmount"]');
                    var pSelPaymentTypeWitholdingTaxRate = $('[name="selPaymentTypeWitholdingTaxRate"]');
                    var pTxtPaymentWitholdingTaxAmount = $('[name="txtPaymentWitholdingTaxAmount"]');
                    CacPopup.params = param;

                    var mSelOPPaymentType = this.selOPPaymentType;
                    function Load_selOPPaymentType() {
                        AjaxSend({
                            url: '/Content/OtherPaymentTypes.txt',
                            type: 'GET',
                            dataType: 'text',
                            success: function(vData) {
                                if (vData.Error)
                                    NewMessageBox(vData.Error);
                                else if (typeof vData != 'string')
                                    NewMessageBox('unexpected response in Load_selOPPaymentType. ref: jsQT9LCXTAR7!');

                                var pSplits = vData.split('\n');
                                for (var pIndex = 0; pIndex <= pSplits.length - 1; pIndex++) {
                                    var pSplit = pSplits[pIndex].trim();
                                    var pExistingOption = mSelOPPaymentType.find('option[value="' + pSplit + '"]');
                                    if (pExistingOption.length > 0)
                                        continue;
                                    var pOption = new Option(pSplit, pSplit);
                                    mSelOPPaymentType.append(pOption);
                                }

                                mSelOPPaymentType.val(sessionStorage.DefaultOtherPaymentID);
                            }
                        });
                    } // END Load_selOPPaymentType
                    Load_selOPPaymentType();

                    if (proj.IsBootstrapDesign()) {
                        $('#popFMCGOtherPayment-bs').modal('show');
                        $('#listOP').addClass('list-group');
                    } else {
                        $('#popFMCGOtherPayment').popup('open');
                    }
                    
                    CacPopup.popupname = 'op';

                    if (this.isFinishedRequiredData == false) {
                        this.LoadRequiredData();
                        this.isFinishedRequiredData = true;
                    }

                    this.Main();

                    BindKeypress('#formFMCGOtherPayment', this.selOPPaymentType, false, 'select');
                    BindKeypress('#formFMCGOtherPayment', this.txtOPAvailablePts, false, 'text');
                    BindKeypress('#formFMCGOtherPayment', this.txtOPIssuingOrg, false, 'text');
                    BindKeypress('#formFMCGOtherPayment', this.txtOPCardHolder, false, 'text');
                    BindKeypress('#formFMCGOtherPayment', this.txtOPCardNo, true, 'text');
                    BindKeypress('#formFMCGOtherPayment', this.txtOPApprovalNo, false, 'text');
                    BindKeypress('#formFMCGOtherPayment', this.txtOPAmount, true, 'text');

                    this.selOPPaymentType.change(function () {
                        var pType = CacPopup.OtherPayment.selOPPaymentType.val();
                        if (pType == "V-Plus") {
                            $('#divAvailablePts, #divPaymentTypeSection').show();
                            $('#divPaymentTypeWithholdTax').hide();
                            CacPopup.OtherPayment.GetVPlusPoints();
                        } else if (pType == 'Witholding Tax') {
                            $('#divPaymentTypeSection').hide();
                            $('#divPaymentTypeWithholdTax').show();
                        } else {
                            $('#divAvailablePts').hide();
                            $('#divPaymentTypeSection').show();
                            $('#divPaymentTypeWithholdTax').hide();
                        }

                        dis.txtOPIssuingOrg.val(sessionStorage.OrgName);
                        dis.txtOPAmount.val($('#lblAmountLacking').text());
                    })

                    $('#btnOPBack').unbind('click').click(function () {
                        $('#pageListOfOP').pageshow();
                    })

                    $('#btnAddOP').unbind('click').click(function () {
                        CacPopup.OtherPayment.Add();
                    })

                    pTxtPaymentTypeGrossTaxableAmount.change(RecomputeWitholdingTax);
                    pTxtPaymentTypeVatRate.change(RecomputeWitholdingTax);
                    pTxtPaymentTypeNetTaxableAmount.keyup(function() {
                        var pNetTaxableAmount = pTxtPaymentTypeNetTaxableAmount.val();
                        var pWitholdingTaxAmount = pNetTaxableAmount * CDec2(pSelPaymentTypeWitholdingTaxRate.val()) / 100;
                        pTxtPaymentWitholdingTaxAmount.val(pWitholdingTaxAmount);
                    });
                    pSelPaymentTypeWitholdingTaxRate.change(RecomputeWitholdingTax);
                    function RecomputeWitholdingTax() {
                        var pNetTaxableAmount = CDec2(pTxtPaymentTypeGrossTaxableAmount.val()) / (1 + CDec2(pTxtPaymentTypeVatRate.val()) / 100);
                        pTxtPaymentTypeNetTaxableAmount.val(pNetTaxableAmount);

                        var pWitholdingTaxAmount = pNetTaxableAmount * CDec2(pSelPaymentTypeWitholdingTaxRate.val()) / 100;
                        pTxtPaymentWitholdingTaxAmount.val(pWitholdingTaxAmount);
                    }

                    pSelPaymentTypeBasisForGrossTaxableAmount.change(function() {
                        if (pSelPaymentTypeBasisForGrossTaxableAmount.val() == 'AmountAppliedToInvoices') {
                            pTxtPaymentTypeGrossTaxableAmount.val(param.AmountAppliedToInvoices);
                            pTxtPaymentTypeNetTaxableAmount.prop('readonly', true);
                        }  else if (pSelPaymentTypeBasisForGrossTaxableAmount.val() == 'UserDefined') {
                            pTxtPaymentTypeNetTaxableAmount.prop('readonly', false);
                        } else {
                            pTxtPaymentTypeGrossTaxableAmount.val(param.CurrentPaymentsAppliedToInvoices);
                            pTxtPaymentTypeNetTaxableAmount.prop('readonly', true);
                        }
                        RecomputeWitholdingTax();
                    });
                    this.btnOkOtherPaymentDetails.click(function (e) {
                        e.preventDefault();
                        var pData;

                        if ($.trim(CacPopup.OtherPayment.selOPPaymentType.val()) == "") {
                            $('#txtOPMsgRequired').pageshow();
                            return;
                        }

                        if (CacPopup.OtherPayment.selOPPaymentType.val() == 'Witholding Tax') {
                            if (($.trim(pTxtPaymentTypeGrossTaxableAmount.val()) == '')
                                || ($.trim(pTxtPaymentTypeVatRate.val()) == '')
                                || ($.trim(pTxtPaymentTypeNetTaxableAmount.val()) == '')
                                || ($.trim(pSelPaymentTypeWitholdingTaxRate.val()) == '')
                                || ($.trim(pTxtPaymentWitholdingTaxAmount.val()) == '')) {

                                $('#txtOPMsgRequired').pageshow();
                                return;
                            }
                        } else {
                            if ($.trim(CacPopup.OtherPayment.selOPPaymentType.val()) == "" || $.trim(CacPopup.OtherPayment.txtOPIssuingOrg.val()) == ""
                                || $.trim(CacPopup.OtherPayment.txtOPCardNo.val()) == "" || $.trim(CacPopup.OtherPayment.txtOPAmount.val()) == "") {
                                $('#txtOPMsgRequired').pageshow();
                                return;
                            }

                            if (CacPopup.OtherPayment.selOPPaymentType.val() == "V-Plus" && $.trim(CacPopup.OtherPayment.txtOPAvailablePts.val()) == "") {
                                $('#txtOPMsgRequired').pageshow();
                                return;
                            }

                        }

                        $('#txtOPMsgRequired, #txtOPMsgErrorIssuanceDate, #txtOPMsgErrorExpirationDate, #txtOPMsgErrorVPlus').hide();

                        if (CacPopup.OtherPayment.selOPPaymentType.val() == 'Witholding Tax')
                            pData = {
                                PaymentType: CacPopup.OtherPayment.selOPPaymentType.val(),
                                GrossTaxableAmount: pTxtPaymentTypeGrossTaxableAmount.val(),
                                VatRate: pTxtPaymentTypeVatRate.val(),
                                TaxableAmount: pTxtPaymentTypeNetTaxableAmount.val(),
                                WitholdingTaxRate: pSelPaymentTypeWitholdingTaxRate.val(),
                                Amount: pTxtPaymentWitholdingTaxAmount.val()
                            };
                        else {
                            var issDate = new Date(dis.txtOPIssuanceDate);
                            var expDate = new Date(dis.txtOPExpirationDate);

                            pData = [
                                CacPopup.OtherPayment.txtOPID.val(), // 0
                                CacPopup.OtherPayment.selOPPaymentType.val(), // 1
                                CacPopup.OtherPayment.txtOPIssuingOrg.val(), // 2
                                CacPopup.OtherPayment.txtOPCardHolder.val(), // 3
                                CacPopup.OtherPayment.txtOPCardNo.val(), // 4
                                (IsDate(issDate) ? '' : issDate.getMonth() + 1), // 5
                                (IsDate(issDate) ? '' : issDate.getDate()), // 6
                                (IsDate(issDate) ? '' : issDate.getFullYear()), // 7
                                (IsDate(expDate) ? '' : expDate.getMonth() + 1), // 8
                                (IsDate(expDate) ? '' : expDate.getDate()), // 9
                                (IsDate(expDate) ? '' : expDate.getFullYear()), // 10
                                CacPopup.OtherPayment.txtOPApprovalNo.val(), // 11
                                CacPopup.OtherPayment.txtOPAmount.val(), // 12
                                CacPopup.OtherPayment.opState // 13
                            ];
                        }                           

                        if (CacPopup.OtherPayment.editIndex == null) {
                            CacPopup.OtherPayment.arrOP.push(pData);
                        }
                        else {
                            CacPopup.OtherPayment.arrOP[CacPopup.OtherPayment.editIndex] = pData;
                            CacPopup.OtherPayment.editIndex = null;
                        }

                        CacPopup.isInAddPage = true;
                        CacPopup.OtherPayment.LoadOPList();
                        CacPopup.returnVal = CacPopup.OtherPayment.arrOP;
                        $('#popFMCGOtherPaymentDetails').modal('hide');
                    })
                    RecomputeWitholdingTax();
                },
                GetVPlusPoints: function () {
                    AjaxSend({
                        url: 'SalesPayments/GetVPlusPoints',
                        data: { CustomerID: $.trim(sessionStorage.CustomerOrganizationID) },
                        success: function (vData) {
                            if (!vData.Result == '') {
                                CacPopup.OtherPayment.txtOPAvailablePts.val(CDec2(vData.Result));
                            }
                        },
                        error: function (vError) {
                            $('#txtOPMsgErrorVPlus').pageshow();
                        }
                    });
                },
                Add: function () {
                    this.txtOPID.val('')
                    this.selOPPaymentType.val(CStr2(sessionStorage.DefaultOtherPaymentID))
                    this.txtOPAvailablePts.val('')
                    this.txtOPIssuingOrg.val(sessionStorage.OrgName)
                    this.txtOPCardNo.val('')
                    this.txtOPCardHolder.val('')
                    this.txtOPApprovalNo.val('')
                    this.txtOPAmount.val($('#lblAmountLacking').text())
                    this.txtOPIssuanceDate.val('')
                    this.txtOPExpirationDate.val('')                    
                    
                    $('#popFMCGOtherPaymentDetails').modal('show');
                    $('#divAvailablePts, #txtOPMsgRequired, #txtOPMsgErrorIssuanceDate, #txtOPMsgErrorExpirationDate, #txtOPMsgErrorVPlus, #divPaymentTypeWithholdTax').hide();
                    setTimeout(function () {
                        CacPopup.OtherPayment.selOPPaymentType.focus();
                    }, 100)
                },
                Edit: function (data) {
                    this.txtOPID.val(data[0]);
                    this.selOPPaymentType.val(data[1]);
                    this.txtOPIssuingOrg.val(data[2]);
                    this.txtOPCardHolder.val(data[3]);
                    this.txtOPCardNo.val(data[4]);
                    this.txtOPIssuanceDate.val((data[5] == '' ? '' : $.format.date(new Date(data[7] + '-' + data[6] + '-' + data[5]), 'yyyy-MM-dd')));
                    this.txtOPExpirationDate.val((data[8] == '' ? '' : $.format.date(new Date(data[10] + '-' + data[9] + '-' + data[8]), 'yyyy-MM-dd')));
                    this.txtOPApprovalNo.val(data[11]);
                    this.txtOPAmount.val(data[12]);
                    this.srvState = data[13];
                    this.editIndex = data[14];

                    $('#popFMCGOtherPaymentDetails').modal('show');
                    $('#txtOPMsgRequired, #txtOPMsgErrorIssuanceDate, #txtOPMsgErrorExpirationDate, #txtOPMsgErrorVPlus').hide();
                    PopupReposition(); //NewMessageBox.js
                    setTimeout(function () {
                        CacPopup.OtherPayment.selOPPaymentType.focus();
                    }, 100)
                },
                Delete: function (data) {
                    this.arrOP[data[14]][13] = "Remove"
                    this.LoadOPList();
                },
                Main: function () {
                    this.LoadOPList();
                    PopupReposition();
                },
                LoadRequiredData: function () {
                    this.selOPIssuanceDateYear.empty();
                    this.selOPIssuanceDateYear.append(new Option('Year', ''))
                    this.selOPExpirationDateYear.empty();
                    this.selOPExpirationDateYear.append(new Option('Year', ''))
                    for (var i = new Date().getFullYear() ; i >= 2000; i--) {
                        this.selOPIssuanceDateYear.append(new Option(i, i))
                        this.selOPExpirationDateYear.append(new Option(i, i))
                    }
                },
                LoadOPList: function () {
                    var listOP = $('#listOP');
                    var isEmpty = true;

                    listOP.empty();

                    if (this.arrOP.length > 0) {
                        for (var i = 0; i < this.arrOP.length; i++) {
                            var dis = this.arrOP[i];
                            if ($.isArray(dis)) {
                                dis.push(i); // add index # in array, use for editing
                                if (dis[13] != "Remove") {
                                    listOP.append($('<li class="list-group-item">')
                                        .data(dis)
                                        .click(function () {
                                            var data = $(this).data();
                                            CacPopup.OtherPayment.Edit(data);
                                        })
                                        .append($('<div class="divTable">')
                                            .append($('<div class=divTableRow>')
                                                .append($('<div class=divTableCell>')
                                                    .append($('<div>').html('<b>Card No: ' + dis[4] + '</b>'))
                                                    .append($('<div>').html('Amount: ' + (dis[12] == 0 ? '0.00' : AccountingFormat(dis[12]))))                                                    
                                                ) // END divTableCell
                                                .append($('<div class=divTableCell style="text-align:right">')
                                                    .append(
                                                        $('<div>').append(
                                                            $('<button data-role="none" id="btnAddOP" class="btn btn-danger">')
                                                                .html('<span class="bi-trash">')
                                                                .click(function (e) {
                                                                    e.stopPropagation();
                                                                    e.preventDefault();
                                                                    var pLI = $(e.target).closest('li');
                                                                    var pData = pLI.data();
                                                                    CacPopup.OtherPayment.Delete(pData);
                                                                    CacPopup.OtherPayment.LoadOPList();
                                                            })
                                                        )
                                                    )
                                                ) // END divTableCell
                                            ) // END divTableRow
                                        ) // END divTable
                                    );
                                    isEmpty = false; // set to false when there's no remove state in arrop, found in index 13, w/c means add, edit, remove
                                }
                            } else {
                                dis.Index = i
                                if (!CBool(dis.Remove)) {
                                    listOP.append($('<li class="list-group-item">')
                                        .data(dis)
                                        .append($('<div>').html('Amount: ' + (dis.Amount == 0 ? '0.00' : AccountingFormat(dis.Amount))))
                                    )
                                    isEmpty = false; // set to false when there's no remove state in arrop, found in index 13, w/c means add, edit, remove
                                }
                            }
                        }

                        listOP.find('li').first().addClass('navList-a');
                    }

                    if (isEmpty) {
                        listOP.append($('<li class="list-group-item">').text('List is empty'));
                    }

                    PopupReposition();
                    this.TotalOP();
                },
                TotalOP: function () {
                    if (this.arrOP.length > 0) {
                        this.totalop = 0;

                        for (var i = 0; i < this.arrOP.length; i++) {
                            var dis = this.arrOP[i];
                            if ($.isArray(dis)) {
                                if (dis[13] != "Remove") {
                                    this.totalop = CDec2(this.totalop) + CDec2(dis[12]);
                                }
                            } else {
                                if (!CBool(dis.Remove)) {
                                    this.totalop += CDec2(dis.Amount);
                                }      
                            }
                        }
                    }
                    CacPopup.params.success(this.totalop);
                    //CacPopup.Misc.TotalMisc();
                    //CacPopup.ComputePayment();
                }
            },
            Cash: {
                Clear: function () {
                    this.cashTendered = 0;
                    CacPopup.popupname = '';
                    $('#txtCash').text('0.00');
                },
                cashTendered: 0,
                init: function (params) {
                    if (proj.IsBootstrapDesign()) {
                        $('#popFMCGCash-bs').modal('show');
                    } else {
                        $('#popFMCGCash').popup('open');
                        PopupReposition();
                    }
                    
                    setTimeout(function () {
                        $('[name="txtCashAmount"]').focus();
                    }, 800)
                    var amount = $('[name="txtCashAmount"]');
                    var curCash = 0;
                    CacPopup.popupname = 'cash';
                    amount.val('');

                    BindKeypress('#formFMCGCash', amount, true, 'text');

                    $('.btnNumPad').unbind('click').click(function (e) {
                        e.preventDefault();
                        var val = $(this).attr('amount');

                        if (val == '0') {
                            curCash = curCash + '0';
                        }
                        else if (val == 'c') {
                            curCash = '';
                        }
                        else {
                            curCash = CDec2(curCash) + CDec2(val);
                        }

                        amount.val(curCash).focus();
                    })

                    $('#formFMCGCash').unbind('submit').submit(function (e) {
                        e.preventDefault();

                        //if (CDec2(params.Balance) > amount.val()) {
                        //    $('#txtCashMsg').show();
                        //}
                        //else {

                        //}
                        CacPopup.Cash.cashTendered = CDec2(amount.val());
                        params.success(amount.val());

                        if (proj.IsBootstrapDesign()) {
                            $('#popFMCGCash-bs').modal('hide');
                        } else {
                            $('#popFMCGCash').popup('close');
                        }
                        
                        $('#txtCashMsg').hide();
                        //FmcgBasicCart.txtQtyPlu.focus();
                        //CacPopup.ComputePayment();
                    })
                }
            },
            Card: {
                form: null,
                txtCardID: null,
                selCardType: null,
                selCreditCardType: null,
                selCardIssuingBank: null,
                selCardBankLocation: null,
                txtCardFname: null,
                txtCardMname: null,
                txtCardLname: null,
                selCardGenSuffix: null,
                txtCardNo: null,
                selCardIssuanceDateMonth: null,
                selCardIssuanceDateDay: null,
                selCardIssuanceDateYear: null,
                selCardExpirationDateMonth: null,
                selCardExpirationDateDay: null,
                selCardExpirationDateYear: null,
                txtCardApprovalNo: null,
                txtCardInstallmentNo: null,
                txtCardAmount: null,
                isFinishedRequiredData: false,
                arrCredit: [],
                arrDebit: [],
                editIndex: null,
                cdState: 'New',
                totalcardamt: 0,
                Clear: function () {
                    this.arrCredit = [];
                    this.arrDebit = [];
                    this.editIndex = null;
                    this.cdState = 'New';
                    this.totalcardamt = 0;
                    $('.txtCard').text('0.00');
                    CacPopup.popupname = '';

                    $('#listCard').empty().append('<li>List is empty</li>');
                },
                init: function (param) {
                    //if (proj.CacMode == 'AP') {
                    //    NewMessageBox('This function is not yet implemented.');
                    //    return;
                    //}

                    this.form = $('#formFMCGCard');
                    this.txtCardID = this.form.find('[name="txtCardID"]');
                    this.selCardType = this.form.find('[name="selCardType"]');
                    this.selCreditCardType = this.form.find('[name="selCreditCardType"]');
                    this.butCardIssuingBank = this.form.find('[name="butCardIssuingBank"]');
                    this.selCardIssuingBank = this.form.find('[name="selCardIssuingBank"]');
                    this.selCardBankLocation = this.form.find('[name="selCardBankLocation"]');
                    this.txtCardFname = this.form.find('[name="txtCardFname"]');
                    this.txtCardMname = this.form.find('[name="txtCardMname"]');
                    this.txtCardLname = this.form.find('[name="txtCardLname"]');
                    this.selCardGenSuffix = this.form.find('[name="selCardGenSuffix"]');
                    this.txtCardNo = this.form.find('[name="txtCardNo"]');
                    this.selCardIssuanceDateMonth = this.form.find('[name="selCardIssuanceDateMonth"]');
                    this.selCardIssuanceDateDay = this.form.find('[name="selCardIssuanceDateDay"]');
                    this.selCardIssuanceDateYear = this.form.find('[name="selCardIssuanceDateYear"]');
                    this.selCardExpirationDateMonth = this.form.find('[name="selCardExpirationDateMonth"]');
                    this.selCardExpirationDateDay = this.form.find('[name="selCardExpirationDateDay"]');
                    this.selCardExpirationDateYear = this.form.find('[name="selCardExpirationDateYear"]');
                    this.txtCardApprovalNo = this.form.find('[name="txtCardApprovalNo"]');
                    this.txtCardInstallmentNo = this.form.find('[name="txtCardInstallmentNo"]');
                    this.txtCardAmount = this.form.find('[name="txtCardAmount"]');
                    this.btnCardIssuanceDate = this.form.find('[id="btnCardIssuanceDate"]');
                    this.btnCardExpirationDate = this.form.find('[id="btnCardExpirationDate"]');
                    CacPopup.params = param;
                    var mBankID;
                    var mBankName;
                    
                    if(proj.IsBootstrapDesign()){
                        $('#popFMCGCard-bs').modal('show');
                        $('#listCard').addClass('list-group');
                    } else {
                        $('#popFMCGCard').popup('open');
                    }
                    
                    CacPopup.popupname = 'card';

                    if (this.isFinishedRequiredData == false) {
                        this.LoadRequiredData()
                        this.isFinishedRequiredData = true;
                    }

                    this.Main();

                    BindKeypress('#formFMCGCard', this.selCardType, false, 'select');
                    BindKeypress('#formFMCGCard', this.selCreditCardType, false, 'select');
                    BindKeypress('#formFMCGCard', this.selCardIssuingBank, false, 'select');
                    BindKeypress('#formFMCGCard', this.selCardBankLocation, false, 'select');
                    BindKeypress('#formFMCGCard', this.txtCardFname, false, 'text');
                    BindKeypress('#formFMCGCard', this.txtCardMname, false, 'text');
                    BindKeypress('#formFMCGCard', this.txtCardLname, false, 'text');
                    BindKeypress('#formFMCGCard', this.selCardGenSuffix, false, 'select');
                    BindKeypress('#formFMCGCard', this.txtCardNo, true, 'text');
                    BindKeypress('#formFMCGCard', this.selCardIssuanceDateMonth, false, 'select');
                    BindKeypress('#formFMCGCard', this.selCardIssuanceDateDay, false, 'select');
                    BindKeypress('#formFMCGCard', this.selCardIssuanceDateYear, false, 'select');
                    BindKeypress('#formFMCGCard', this.selCardExpirationDateMonth, false, 'select');
                    BindKeypress('#formFMCGCard', this.selCardExpirationDateDay, false, 'select');
                    BindKeypress('#formFMCGCard', this.selCardExpirationDateYear, false, 'select');
                    BindKeypress('#formFMCGCard', this.txtCardApprovalNo, true, 'text');
                    BindKeypress('#formFMCGCard', this.txtCardInstallmentNo, true, 'text');
                    BindKeypress('#formFMCGCard', this.txtCardAmount, true, 'text');

                    $('#btnCardBack').unbind('click').click(function () {
                        $('#divListOfCard').pageshow();
                    })

                    this.butCardIssuingBank.text('Select a Bank').unbind('click').click(function (e) {
                        e.preventDefault();
                        window.CacPopup.BrowseBank({
                            Click: function(vData) {
                                mBankID = vData.ID;
                                mBankName = vData.CompanyName;
                                CacPopup.Card.butCardIssuingBank.text(vData.CompanyName);
                                if (proj.IsBootstrapDesign()) {
                                    $('#popFMCGCard-bs').modal('show');
                                } else {
                                    $('#popFMCGCard').popup('open');
                                }
                            },
                            Close: function() {
                                if (proj.IsBootstrapDesign()) {
                                    $('#popFMCGCard-bs').modal('show');
                                } else {
                                    $('#popFMCGCard').popup('open');
                                }
                            }
                        });
                    });

                    this.selCardIssuingBank.unbind('change').change(function () {
                        mBankID = $(this).val();
                        mBankName = $(this).find('option:selected').text();
                    })

                    this.selCardType.change(function (e) {
                        if ($(this).val() == "Debit") {
                            $('#divFMCGCredit').hide();
                            CacPopup.Card.selCreditCardType.attr('hidden', '');
                        }
                        else {
                            $('#divFMCGCredit').show();
                            CacPopup.Card.selCreditCardType.removeAttr('hidden');
                        }
                    })

                    this.btnCardIssuanceDate.unbind('click').click(function (e) {
                        e.preventDefault();
                        SingleCalendar({
                            HidePrevPopup: false,
                            Click: function (e) {
                                if (proj.IsBootstrapDesign()) {
                                    $('#popFMCGCard-bs').modal('show');
                                } else {
                                    $('#popFMCGCard').popup('open');
                                }
                                CacPopup.Card.btnCardIssuanceDate.text(e);
                                CacPopup.Card.txtCardAmount.focus();
                            },
                            Close: function () {
                                if (proj.IsBootstrapDesign()) {
                                    $('#popFMCGCard-bs').modal('show');
                                } else {
                                    $('#popFMCGCard').popup('open');
                                }
                                CacPopup.Card.txtCardAmount.focus();
                                CacPopup.popupname = 'card';
                            }
                        })
                    })

                    this.btnCardExpirationDate.unbind('click').click(function (e) {
                        e.preventDefault();
                        SingleCalendar({
                            HidePrevPopup: false,
                            Click: function (e) {
                                if (proj.IsBootstrapDesign()) {
                                    $('#popFMCGCard-bs').modal('show');
                                } else {
                                    $('#popFMCGCard').popup('open');
                                }
                                CacPopup.Card.btnCardExpirationDate.text(e);
                                CacPopup.Card.txtCardAmount.focus();
                            },
                            Close: function () {
                                if (proj.IsBootstrapDesign()) {
                                    $('#popFMCGCard-bs').modal('show');
                                } else {
                                    $('#popFMCGCard').popup('open');
                                }
                                CacPopup.Card.txtCardAmount.focus();
                                CacPopup.popupname = 'card';
                            }
                        })
                    })

                    $('#btnAddCard').unbind('click').click(function () {
                        CacPopup.Card.Add();
                    })

                    this.form.unbind('submit').submit(function (e) {
                        e.preventDefault();
                        var pData;
                        var cardtype = $.trim(CacPopup.Card.selCardType.val());

                        if (cardtype == "Credit") {
                            if ($.trim(CacPopup.Card.selCreditCardType.val()) == "") {
                                $('#txtErrorMsgRequired').pageshow();
                                return;
                            }
                        }

                        if ((CStr2(mBankID).trim() == '') || $.trim(CacPopup.Card.txtCardLname.val()) == ""
                                    || $.trim(CacPopup.Card.txtCardFname.val()) == "" || $.trim(CacPopup.Card.txtCardNo.val()) == ""
                                    || $.trim(CacPopup.Card.txtCardAmount.val()) == "") {
                            $('#txtErrorMsgRequired').pageshow();
                            return;
                        }
                        else if ($.trim(CacPopup.Card.selCardIssuanceDateMonth.val()) != "" || $.trim(CacPopup.Card.selCardIssuanceDateDay.val()) != ""
                                || $.trim(CacPopup.Card.selCardIssuanceDateYear.val()) != "") {
                            if ($.trim(CacPopup.Card.selCardIssuanceDateMonth.val()) == "" || $.trim(CacPopup.Card.selCardIssuanceDateDay.val()) == ""
                                || $.trim(CacPopup.Card.selCardIssuanceDateYear.val()) == "") {
                                $('#txtErrorIssuanceDate').pageshow();
                                return;
                            }
                            else {
                                $('#txtErrorIssuanceDate').hide();
                            }
                        }
                        else if ($.trim(CacPopup.Card.selCardExpirationDateMonth.val()) != "" || $.trim(CacPopup.Card.selCardExpirationDateDay.val()) != ""
                                || $.trim(CacPopup.Card.selCardExpirationDateYear.val()) != "") {
                            if ($.trim(CacPopup.Card.selCardExpirationDateMonth.val()) == "" || $.trim(CacPopup.Card.selCardExpirationDateDay.val()) == ""
                                || $.trim(CacPopup.Card.selCardExpirationDateYear.val()) == "") {
                                $('#txtErrorExpirationDate').pageshow();
                                return;
                            }
                            else {
                                $('#txtErrorExpirationDate').hide();
                            }
                        }
                        else {
                            $('#txtErrorMsgRequired, #txtErrorIssuanceDate, #txtErrorExpirationDate').hide();

                            var issuanceDate = CacPopup.Card.btnCardIssuanceDate.text();
                            var expirationDate = CacPopup.Card.btnCardExpirationDate.text();

                            if (IsDate(issuanceDate)) {
                                issuanceDate = new Date(issuanceDate);
                            } else {
                                issuanceDate = false;
                            }

                            if (IsDate(expirationDate)) {
                                expirationDate = new Date(expirationDate);
                            } else {
                                expirationDate = false;
                            }

                            pData = [
                                    CacPopup.Card.txtCardID.val(), // 0
                                    (cardtype == "Debit" ? "" : CacPopup.Card.selCreditCardType.val()), // 1
                                    mBankID, // 2
                                    mBankName, // 3
                                    CacPopup.Card.txtCardLname.val(), // 4
                                    CacPopup.Card.txtCardFname.val(), // 5
                                    CacPopup.Card.txtCardMname.val(), // 6                                    
                                    CacPopup.Card.selCardGenSuffix.val(), // 7
                                    CacPopup.Card.selCardBankLocation.val(), // 8
                                    CacPopup.Card.selCardBankLocation.find('option:selected').text(), // 9
                                    CacPopup.Card.txtCardNo.val(), // 10
                                    (issuanceDate == false ? "" : issuanceDate.getMonth()), // 11 Issuance Date Month
                                    (issuanceDate == false ? "" : issuanceDate.getDate()), // 12 Issuance Date Day
                                    (issuanceDate == false ? "" : issuanceDate.getFullYear()), // 13  Issuance Date Year
                                    (expirationDate == false ? "" : expirationDate.getMonth()), // 14 Expiration Date Month
                                    (expirationDate == false ? "" : expirationDate.getDate()), // 15 Expiration Date Day
                                    (expirationDate == false ? "" : expirationDate.getFullYear()), // 16 Expiration Date Year
                                    CacPopup.Card.txtCardApprovalNo.val(), // 17
                                    CacPopup.Card.txtCardInstallmentNo.val(), // 18
                                    CacPopup.Card.txtCardAmount.val(), // 19
                                    CacPopup.Card.cdState, // 20
                                    cardtype // 21
                            ]

                            if (cardtype == "Debit") {
                                if (CacPopup.Card.editIndex == null) {
                                    CacPopup.Card.arrDebit.push(pData);
                                }
                                else {
                                    CacPopup.Card.arrDebit[CacPopup.Card.editIndex] = pData
                                    CacPopup.ChequeGC.editIndex = null; // set to null after edit
                                }
                            }
                            else {
                                if (CacPopup.Card.editIndex == null) {
                                    CacPopup.Card.arrCredit.push(pData);
                                }
                                else {
                                    CacPopup.Card.arrCredit[CacPopup.Card.editIndex] = pData
                                    CacPopup.ChequeGC.editIndex = null; // set to null after edit
                                }
                            }

                            CacPopup.isInAddPage = true;
                            CacPopup.Card.LoadCardList();
                            CacPopup.returnVal = CacPopup.Card.totalcardamt;
                        }
                    })
                },
                Add: function () {
                    this.selCardType.val('Debit').focus();
                    this.selCreditCardType.val('')
                    this.selCardIssuingBank.val('')
                    this.selCardBankLocation.val('')
                    this.txtCardFname.val('')
                    this.txtCardMname.val('')
                    this.txtCardLname.val('')
                    this.txtCardNo.val('')
                    this.selCardIssuanceDateMonth.val('')
                    this.selCardIssuanceDateDay.val('')
                    this.selCardIssuanceDateYear.val('')
                    this.selCardExpirationDateMonth.val('')
                    this.selCardExpirationDateDay.val('')
                    this.selCardExpirationDateYear.val('')
                    this.txtCardApprovalNo.val('')
                    this.txtCardInstallmentNo.val('')
                    this.txtCardAmount.val('')
                    this.btnCardIssuanceDate.text('Set Date');
                    this.btnCardExpirationDate.text('Set Date');
                    this.cdState = 'New';

                    $('#divNewCard').pageshow();
                    $('#divFMCGCredit, #txtErrorMsgRequired, #txtErrorIssuanceDate, #txtErrorExpirationDate').hide();
                    this.selCreditCardType.attr('hidden', '')
                    PopupReposition(); //NewMessageBox.js
                    setTimeout(function () {
                        CacPopup.Card.selCardType.focus();
                    }, 100)
                },
                Edit: function (data) {
                    this.txtCardID.val(data[0]);
                    this.selCardType.val(data[21]);
                    this.selCreditCardType.val(data[1]);
                    this.selCardIssuingBank.val(data[2]);
                    this.txtCardLname.val(data[4]);
                    this.txtCardFname.val(data[5]);
                    this.txtCardMname.val(data[6]);
                    this.selCardGenSuffix.val(data[7]);
                    this.selCardBankLocation.val(data[8]);
                    this.txtCardNo.val(data[10]);
                    this.selCardIssuanceDateMonth.val(data[11]);
                    this.selCardIssuanceDateDay.val(data[12]);
                    this.selCardIssuanceDateYear.val(data[13]);
                    this.selCardExpirationDateMonth.val(data[14]);
                    this.selCardExpirationDateDay.val(data[15]);
                    this.selCardExpirationDateYear.val(data[16]);
                    this.txtCardApprovalNo.val(data[17]);
                    this.txtCardInstallmentNo.val(data[18]);
                    this.txtCardAmount.val(data[19]);
                    this.cdState = data[20];
                    this.editIndex = data[22];

                    if (data[21] == "Debit") {
                        $('#divFMCGCredit').hide();
                        this.selCreditCardType.attr('hidden', '')
                    } else {
                        $('#divFMCGCredit').show();
                        this.selCreditCardType.removeAttr('hidden')
                    }

                    $('#divNewCard').pageshow();
                    $('#txtErrorMsgRequired, #txtErrorIssuanceDate, #txtErrorExpirationDate').hide();
                    PopupReposition(); //NewMessageBox.js
                    setTimeout(function () {
                        CacPopup.Card.selCardType.focus();
                    }, 100)
                },
                Delete: function (data) {
                    if (data[21] == "Debit") {
                        this.arrDebit[data[22]][20] = "Remove"
                    }
                    else {
                        this.arrCredit[data[22]][20] = "Remove"
                    }
                    this.LoadCardList();
                },
                Main: function () {
                    $('#divListOfCard').pageshow();
                    this.LoadCardList();
                    PopupReposition(); //NewMessageBox.js
                },
                LoadRequiredData: function () {
                    var dis = this;
                    AjaxSend({
                        url: 'Browser/LibraryBanks',
                        data: {IsAll : 1},
                        success: function (vData) {
                            var vRows = vData.Results.Rows;
                            dis.selCardIssuingBank.empty();
                            dis.selCardIssuingBank.append(new Option('Please Select', ''))
                            if (vRows.length > 0) {
                                for (var i = 0; i < vRows.length; i++) {
                                    dis.selCardIssuingBank.append(new Option(vRows[i].BankName, vRows[i].ID))
                                }
                            }
                        }
                    })

                    AjaxSend({
                        url: 'Browser/LibraryCountries',
                        data: {IsAll : 1},
                        success: function (vData) {
                            var vRows = vData.Results.Rows;
                            dis.selCardBankLocation.empty();
                            dis.selCardBankLocation.append(new Option('Please Select', ''))
                            if (vRows.length > 0) {
                                for (var i = 0; i < vRows.length; i++) {
                                    dis.selCardBankLocation.append(new Option(vRows[i].Name, vRows[i].ID))
                                }
                            }
                        }
                    })

                    this.selCardIssuanceDateYear.empty();
                    this.selCardIssuanceDateYear.append(new Option('Year', ''))
                    this.selCardExpirationDateYear.empty();
                    this.selCardExpirationDateYear.append(new Option('Year', ''))
                    for (var i = new Date().getFullYear() ; i >= 2000; i--) {
                        this.selCardIssuanceDateYear.append(new Option(i, i))
                        this.selCardExpirationDateYear.append(new Option(i, i))
                    }
                },
                LoadCardList: function () {
                    var listCard = $('#listCard');
                    var isDebitCardEmpty = true;
                    var isCreditCardEmpty = true;

                    listCard.empty();

                    if (this.arrDebit.length > 0 || this.arrCredit.length > 0) {
                        for (var i = 0; i < this.arrDebit.length; i++) {
                            var dis = this.arrDebit[i];
                            dis.push(i); // add index # in array, use for editing
                            if (dis[20] != "Remove") {
                                listCard.append($('<li class="list-group-item">')
                                    .data(dis)
                                    .append($('<div>').html('Card Type: ' + dis[21] + '<br>'))
                                    .append($('<div>').html('Amount: ' + (dis[19] == 0 ? '0.00' : AccountingFormat(dis[19]))))
                                )
                                isDebitCardEmpty = false; // set to false when there's no remove state in arrDebit, found in index 20, w/c means add, edit, remove
                            }
                        }

                        for (var i = 0; i < this.arrCredit.length; i++) {
                            var dis = this.arrCredit[i];
                            dis.push(i); // add index # in array, use for editing
                            if (dis[20] != "Remove") {
                                listCard.append($('<li class="list-group-item">')
                                    .data(dis)
                                    .append($('<div>').html('Card Type: ' + dis[21] + '<br>'))
                                    .append($('<div>').html('Amount: ' + (dis[19] == 0 ? '0.00' : AccountingFormat(dis[19]))))
                                )
                                isCreditCardEmpty = false; // set to false when there's no remove state in arrCredit, found in index 20, w/c means add, edit, remove
                            }
                        }

                        listCard.find('li').first().addClass('navList-a');
                    }

                    if (isDebitCardEmpty && isCreditCardEmpty) {
                        listCard.append($('<li class="list-group-item">').text('List is empty'))
                    }

                    $('#divListOfCard').pageshow();
                    PopupReposition();

                    this.TotalCardAmt();
                },
                TotalCardAmt: function () {
                    if (this.arrDebit.length > 0 || this.arrCredit.length > 0) {
                        this.totalcardamt = 0;
                        for (var i = 0; i < this.arrDebit.length; i++) {
                            var dis = this.arrDebit[i];
                            if (dis[20] != "Remove") {
                                this.totalcardamt = this.totalcardamt + CDec2(dis[19])
                            }
                        }

                        for (var i = 0; i < this.arrCredit.length; i++) {
                            var dis = this.arrCredit[i];
                            if (dis[20] != "Remove") {
                                this.totalcardamt = this.totalcardamt + CDec2(dis[19])
                            }
                        }
                        //$('.txtCard').text((this.totalcardamt > 0 ? AccountingFormat(this.totalcardamt) : '0.00'))
                        CacPopup.params.success(this.totalcardamt)
                    }

                    //CacPopup.ComputePayment();
                }
            },
            Srv: {
                form: null,
                txtSrvID: null,
                txtSrvTransMemoID: null,
                txtSrvNo: null,
                txtSrvPinCode: null,
                txtMainSrvAmount: null,
                txtSrvAmount: null,
                btnSrvBack: null,
                editIndex: null,
                srvState: 'New',
                arrSrv: [],
                totalsrv: 0,
                Clear: function () {
                    this.arrSrv = [];
                    this.totalsrv = 0;
                    this.editIndex = null;
                    this.srvState = 'New';
                    CacPopup.popupname = '';
                    $('#listSrv').empty().append('<li>List is empty</li>');
                },
                init: function (param) {
                    if (proj.CacMode == 'AP') {
                        NewMessageBox('This function is not yet implemented.');
                        return;
                    }
                    this.form = $('#formFMCGSrv');
                    this.txtSrvID = this.form.find('[name="txtSrvID"]');
                    this.txtSrvTransMemoID = this.form.find('[name="txtSrvTransMemoID"]');
                    this.txtSrvNo = this.form.find('[name="txtSrvNo"]');
                    this.txtSrvPinCode = this.form.find('[name="txtSrvPinCode"]');
                    this.txtMainSrvAmount = this.form.find('[name="txtMainSrvAmount"]');
                    this.txtSrvAmount = this.form.find('[name="txtSrvAmount"]');
                    this.btnSrvBack = this.form.find('#btnSrvBack');
                    this.txtSrvAmount.removeAttr('readonly');
                    CacPopup.params = param;

                    if (proj.IsBootstrapDesign()) {
                        $('#popFMCGSrv-bs').modal('show');
                        $('#listSrv').addClass('list-group');
                    } else {
                        $('#popFMCGSrv').popup('open');
                    }

                    CacPopup.popupname = 'srv';

                    this.Main();

                    BindKeypress('#formFMCGSrv', this.txtSrvNo, true, 'text');
                    BindKeypress('#formFMCGSrv', this.txtSrvPinCode, false, 'text');

                    this.btnSrvBack.unbind('click').click(function (e) {
                        e.preventDefault();
                        $('#pageListOfSrv').pageshow();
                    })

                    $('#btnScanSrvNo').unbind('click').click(function (e) {
                        e.preventDefault();
                        if (IsMobile) {
                            mobile.Scan({
                                Success: function (vData) {
                                    CacPopup.Srv.txtSrvNo.val(vData.Value);
                                }
                            });
                        } else {
                            $('#txtSrvMsgBarcode').pageshow();
                        }
                    })

                    $('#btnScanPinCode').unbind('click').click(function (e) {
                        e.preventDefault();
                        if (IsMobile) {
                            mobile.Scan({
                                Success: function (vData) {
                                    CacPopup.Srv.txtSrvPinCode.val(vData.Value);
                                }
                            });
                        } else {
                            $('#txtSrvMsgBarcode').pageshow();
                        }
                    })

                    $('#btnVerifySrv').unbind('click').click(function (e) {
                        e.preventDefault();
                        CacPopup.Srv.VerifySrv();
                    })

                    $('#btnAddSrv').unbind('click').click(function () {
                        CacPopup.Srv.Add();
                    })

                    this.form.unbind('submit').submit(function (e) {
                        e.preventDefault();
                        CacPopup.Srv.VerifySrv();
                    })
                },
                VerifySrv: function () {
                    if ($.trim(this.txtSrvNo.val()) == "" || $.trim(this.txtSrvPinCode.val()) == "") {
                        $('#txtSrvMsgRequired').pageshow();
                    }
                    else if ($.trim(this.txtSrvNo.val()) != "" && $.trim(this.txtSrvPinCode.val()) != ""
                            && CDec2($.trim(this.txtMainSrvAmount.val())) != 0) { // && CDec2($.trim(this.txtSrvAmount.val())) != 0
                        // go here after successfully verified srv
                        $('#txtSrvMsgRequired, #txtSrvMsgNotFound, #txtSrvMsgRequiredInterent, #txtSrvMsgBarcode').hide();

                        var pData = [
                            this.txtSrvID.val(), // 0
                            this.txtSrvTransMemoID.val(), // 1
                            this.txtSrvNo.val(), // 2
                            this.txtSrvPinCode.val(), // 3
                            this.txtMainSrvAmount.val(), // 4
                            this.txtSrvAmount.val(), // 5
                            this.srvState // 6
                        ];

                        if (this.editIndex == null) {
                            this.arrSrv.push(pData);
                        }
                        else {
                            this.arrSrv[this.editIndex] = pData
                            this.editIndex = null; // set to null after edit
                        }

                        CacPopup.isInAddPage = true;
                        this.LoadSrvList();
                        CacPopup.returnVal = this.arrSrv;
                    }
                    else {
                        // go here after both srvno and pinno if not empty
                        var pData = {
                            SRVNumber: $.trim(this.txtSrvNo.val()),
                            SRVPin: $.trim(this.txtSrvPinCode.val())
                        }

                        AjaxSend({
                            url: "SalesPayments/getSalesPaymentSRVInfo",
                            data: pData,
                            success: function (vData) {

                                if (vData.MemoID != "") {
                                    $('#txtSrvMsgRequired, #txtSrvMsgNotFound, #txtSrvMsgRequiredInterent, #txtSrvMsgBarcode').hide();

                                    CacPopup.Srv.txtSrvTransMemoID.val(vData.MemoID || '');
                                    CacPopup.Srv.txtMainSrvAmount.val(AccountingFormat(vData.SRVAmount) || '0.00');

                                    //if (CDec2($("#txtBalChange").text()) <= CDec2(vData.SRVAmount)) {             // commented 2016_11_10__09_44_54_4012
                                    //    CacPopup.Srv.txtSrvAmount.val($("#txtBalChange").text());                 // commented 2016_11_10__09_44_54_4012
                                    //} else {                                                                      // commented 2016_11_10__09_44_54_4012
                                        CacPopup.Srv.txtSrvAmount.val(AccountingFormat(vData.SRVAmount) || '0.00');
                                    //}                                                                             // commented 2016_11_10__09_44_54_4012

                                } else {

                                    CacPopup.Srv.txtSrvTransMemoID.val('');
                                    CacPopup.Srv.txtMainSrvAmount.val('0.00');
                                    CacPopup.Srv.txtSrvAmount.val('0.00');

                                    $('#txtSrvMsgNotFound').pageshow();
                                }

                            },
                            error: function (vError) {
                                $('#txtSrvMsgRequiredInterent').pageshow();
                                return;
                            }
                        });
                    }
                },
                Add: function () {
                    this.txtSrvID.val('')
                    this.txtSrvTransMemoID.val('')
                    this.txtSrvNo.val('')
                    this.txtSrvPinCode.val('')
                    this.txtMainSrvAmount.val('0.00')
                    this.txtSrvAmount.val('0.00')
                    this.srvState = 'New';
                    $('#pageNewSrv').pageshow();
                    $('#txtSrvMsgRequired, #txtSrvMsgNotFound, #txtSrvMsgRequiredInterent, #txtSrvMsgBarcode').hide();
                    PopupReposition();
                    setTimeout(function () {
                        CacPopup.Srv.txtSrvNo.focus();
                    }, 100)
                },
                Edit: function (data) {
                    this.txtSrvID.val(data[0]);
                    this.txtSrvTransMemoID.val(data[1]);
                    this.txtSrvNo.val(data[2]);
                    this.txtSrvPinCode.val(data[3]);
                    this.txtMainSrvAmount.val(data[4]);
                    this.txtSrvAmount.val(data[5]);
                    this.srvState = data[6];
                    this.editIndex = data[7];

                    $('#pageNewSrv').pageshow();
                    $('#txtSrvMsgRequired, #txtSrvMsgNotFound, #txtSrvMsgRequiredInterent, #txtSrvMsgBarcode').hide();
                    PopupReposition(); //NewMessageBox.js
                    setTimeout(function () {
                        CacPopup.Srv.txtSrvNo.focus();
                    }, 100)
                },
                Delete: function (data) {
                    this.arrSrv[data[7]][6] = "Remove"
                    this.LoadSrvList();
                },
                Main: function () {
                    $('#pageListOfSrv').pageshow();
                    this.LoadSrvList();
                    PopupReposition(); //NewMessageBox.js
                },
                LoadSrvList: function () {
                    var listSrv = $('#listSrv');
                    var isEmpty = true;

                    listSrv.empty();

                    if (this.arrSrv.length > 0) {
                        for (var i = 0; i < this.arrSrv.length; i++) {
                            var dis = this.arrSrv[i];
                            if (dis[6] != "Remove") {
                                listSrv.append($('<li class="list-group-item">')
                                    .data(dis)
                                    .append($('<div>').html('<b>SRV No: ' + dis[2] + '</b>'))
                                    .append($('<div>').html('Amount: ' + (dis[5] == 0 ? '0.00' : AccountingFormat(dis[5]))))
                                )
                                isEmpty = false; // set to false when there's no remove state in arrSrv, found in index 6, w/c means add, edit, remove
                            }
                        }

                        listSrv.find('li').first().addClass('navList-a');
                    }

                    if (isEmpty) {
                        listSrv.append($('<li class="list-group-item">').text('List is empty'));
                    }

                    $('#pageListOfSrv').pageshow();
                    PopupReposition();
                    this.TotalSrv();
                },
                TotalSrv: function () {
                    if (this.arrSrv.length > 0) {
                        this.totalsrv = 0;

                        for (var i = 0; i < this.arrSrv.length; i++) {
                            var dis = this.arrSrv[i];
                            dis.push(i); // add index # in array, use for editing
                            if (dis[6] != "Remove") {
                                this.totalsrv = CDec2(this.totalsrv) + CDec2(dis[5]);
                            }
                        }
                    }
                    CacPopup.params.success(this.totalsrv);
                    //CacPopup.Misc.TotalMisc();
                    //CacPopup.ComputePayment();
                }
            },
            Charges: {
                form: null,
                txtChargeID: null,
                selChargeType: null,
                txtChargePercentage: null,
                txtChargeAmount: null,
                txtChargeRemarks: null,
                editIndex: null,
                chargeState: 'New',
                arrCharge: [],
                totalcharge: 0,
                totalothertax: 0,
                lblVATableSales: 0,
                longChargeAmount: 0,
                Clear: function () {
                    this.arrCharge = [];
                    this.totalcharge = 0;
                    this.totalothertax = 0;
                    this.editIndex = null;
                    this.chargeState = 'New';
                    CacPopup.popupname = '';
                    $('#listCharge').empty().append('<li>List is empty</li>');
                },
                init: function (param) {
                    if (proj.CacMode == 'AP') {
                        NewMessageBox('This function is not yet implemented.');
                        return;
                    }
                    this.form = $('#formFMCGCharge');
                    this.selChargeType = this.form.find('[name="selChargeType"]');
                    this.txtChargePercentage = this.form.find('[name="txtChargePercentage"]');
                    this.txtChargeAmount = this.form.find('[name="txtChargeAmount"]');
                    this.txtChargeRemarks = this.form.find('[name="txtChargeRemarks"]');
                    CacPopup.params = param;

                    if (proj.IsBootstrapDesign()) {
                        $('#popFMCGCharges-bs').modal('show');
                        $('#listCharge').addClass('list-group');
                    } else {
                        $('#popFMCGCharges').popup('open');
                    }
                    
                    CacPopup.popupname = 'charges';

                    this.Main();

                    if (OfflineMode) {
                        var vData = SalesPayment_offline().getSalesPaymentReceiptFooterAmountsInfo(sessionStorage.CartOrderID);
                        if (vData.Rows.length > 0) {
                            // for service charge. this is the basis not thew total amount due or subtotal. see sm coin
                            this.lblVATableSales = CDec2(vData.Rows[0].TaxSales || 0) + CDec2(vData.Rows[0].VATExemptSale || 0);
                        }
                    }

                    BindKeypress('#formFMCGCharge', this.selChargeType, false, 'select');
                    BindKeypress('#formFMCGCharge', this.txtChargePercentage, true, 'text');
                    BindKeypress('#formFMCGCharge', this.txtChargeAmount, true, 'text');
                    BindKeypress('#formFMCGCharge', this.txtChargeRemarks, false, 'text');

                    $('#btnAddCharge').unbind('click').click(function () {
                        CacPopup.Charges.Add();
                    })

                    $('#btnChargeBack').unbind('click').click(function () {
                        $('#divListOfCharges').pageshow();
                    })

                    this.txtChargePercentage.unbind('keyup').keyup(function () {
                        if ($.trim($(this).val()) != "") {
                            var chargeAmount = CDec2($.trim($(this).val())) / 100;
                            CacPopup.Charges.longChargeAmount = chargeAmount * CDec2(CacPopup.Charges.lblVATableSales);
                            CacPopup.Charges.txtChargeAmount.val((CacPopup.Charges.longChargeAmount == 0 ? '0' : AccountingFormat(CacPopup.Charges.longChargeAmount)));
                        }
                    })

                    this.txtChargeAmount.unbind('keyup').keyup(function () {
                        if ($.trim($(this).val()) != "") {
                            var chargePercent = CDec2($.trim($(this).val())) / CDec2(CacPopup.Charges.lblVATableSales);
                            CacPopup.Charges.longChargeAmount = $(this).val();
                            CacPopup.Charges.txtChargePercentage.val(((chargePercent * 100) == 0 ? '0' : AccountingFormat(chargePercent * 100)));
                        }
                    })

                    this.form.unbind('submit').submit(function (e) {
                        e.preventDefault();

                        if ($.trim(CacPopup.Charges.txtChargeAmount.val()) == "") {
                            $('#txtChargeErrorMsgRequired').show();
                            return;
                        }
                        else {
                            $('#txtChargeErrorMsgRequired').hide();
                            CacPopup.isInAddPage = true;

                            var hasMatch = false;
                            var pDataCharges = CacPopup.Charges.arrCharge;

                            if (pDataCharges[0] != undefined) {
                                for (var i = 0; i < pDataCharges.length; i++) {
                                    if (CacPopup.Charges.selChargeType.val() == pDataCharges[i].ChargeType) {
                                        //charges

                                        pDataCharges[i] = {
                                            ChargeType: $.trim(CacPopup.Charges.selChargeType.val()),
                                            ChargeAmount: $.trim(CacPopup.Charges.selChargeType.val()) != 'Tip' && $.trim(CacPopup.Charges.selChargeType.val()) != 'Local Tax' ? CacPopup.Charges.longChargeAmount : 0,
                                            TipAmount: $.trim(CacPopup.Charges.selChargeType.val()) == 'Tip' ? CacPopup.Charges.longChargeAmount : 0,
                                            TaxAmount: $.trim(CacPopup.Charges.selChargeType.val()) == 'Local Tax' ? CacPopup.Charges.longChargeAmount : 0,
                                            Remarks: $.trim(CacPopup.Charges.txtChargeRemarks.val())
                                        };
                                        hasMatch = true;
                                        break;
                                    }
                                }


                                if (!hasMatch) {
                                    pDataCharges[1] = {
                                        ChargeType: $.trim(CacPopup.Charges.selChargeType.val()),
                                        ChargeAmount: $.trim(CacPopup.Charges.selChargeType.val()) != 'Tip' && $.trim(CacPopup.Charges.selChargeType.val()) != 'Local Tax' ? CacPopup.Charges.longChargeAmount : 0,
                                        TipAmount: $.trim(CacPopup.Charges.selChargeType.val()) == 'Tip' ? CacPopup.Charges.longChargeAmount : 0,
                                        TaxAmount: $.trim(CacPopup.Charges.selChargeType.val()) == 'Local Tax' ? CacPopup.Charges.longChargeAmount : 0,
                                        Remarks: $.trim(CacPopup.Charges.txtChargeRemarks.val())
                                    };
                                }
                            } else {
                                CacPopup.Charges.arrCharge[0] = {
                                    ChargeType: $.trim(CacPopup.Charges.selChargeType.val()),
                                    ChargeAmount: $.trim(CacPopup.Charges.selChargeType.val()) != 'Tip' && $.trim(CacPopup.Charges.selChargeType.val()) != 'Local Tax' ? CacPopup.Charges.longChargeAmount : 0,
                                    TipAmount: $.trim(CacPopup.Charges.selChargeType.val()) == 'Tip' ? CacPopup.Charges.longChargeAmount : 0,
                                    TaxAmount: $.trim(CacPopup.Charges.selChargeType.val()) == 'Local Tax' ? CacPopup.Charges.longChargeAmount : 0,
                                    Remarks: $.trim(CacPopup.Charges.txtChargeRemarks.val())
                                };
                            }
                            var pParams = GetUrlParameters();
                            var invoiceID = CStr2(pParams.InvoiceID) == '' ? sessionStorage.CartOrderID : pParams.InvoiceID;
                            var pData = {
                                InvoiceID: invoiceID,
                                SalesPaymentChargeList: pDataCharges
                            };

                            if (OfflineMode) {
                                if (SalesPayment_offline().SaveBillingCharges(pData) == 'ok') {
                                    // to be fixed
                                    CacPopup.Charges.GetCurrentCharges();
                                    CacPopup.RefreshSalesPaymentTotal();
                                    CacPopup.ComputePayment();
                                    CacPopup.Misc.TotalMisc();
                                }
                            }                            
                        }
                    })
                },
                Add: function () {
                    this.selChargeType.val('Service Charge');
                    this.txtChargePercentage.val('');
                    this.txtChargeAmount.val('');
                    this.txtChargeRemarks.val('');
                    $('#divNewCharges').pageshow();
                    $('#txtChargeErrorMsgRequired').hide();
                    PopupReposition();
                    setTimeout(function () {
                        CacPopup.Charges.selChargeType.focus();
                    }, 100)
                },
                Delete: function (data) {
                    var pData = {
                        InvoiceID: sessionStorage.CartOrderID,
                        ChargeType: data.ChargeType
                    };

                    if (SalesPayment_offline().DeleteBillCharges(pData) == 'ok') {
                        CacPopup.Charges.GetCurrentCharges();
                        //CacPopup.RefreshSalesPaymentTotal();
                        CacPopup.ComputePayment();
                    } else {
                        console.log('There was an error while processing your request.');
                    }
                },
                Main: function () {
                    $('#divListOfCharges').pageshow();
                    //this.LoadChargeList();
                    PopupReposition(); //NewMessageBox.js
                },
                GetCurrentCharges: function () {
                    if (OfflineMode) {
                        var total = 0.00;
                        var otherTax = 0.00;
                        var charges = 0.00;
                        var result = SalesPayment_offline().GetBillCharges(sessionStorage.CartOrderID);
                        var tip = 0.00;
                        this.totalcharge = 0;

                        if (result.Rows != undefined) {

                            for (var i = 0; i < result.Rows.length; i++) {
                                otherTax += parseFloat(result.Rows[i].LocalTaxAmount);
                                total += parseFloat(result.Rows[i].ChargeAmount) + parseFloat(result.Rows[i].LocalTaxAmount) + parseFloat(result.Rows[i].TipAmount);
                                charges += parseFloat(result.Rows[i].ChargeAmount);
                                tip += parseFloat(result.Rows[i].TipAmount);
                                this.totalcharge = this.totalcharge + total;
                            }

                            this.DisplayChargesList(result.Rows);

                        }
                        $("#lblSalesPayCharges").text(charges > 0 ? charges : AccountingFormat(charges));

                        //$("#lblSalesPayCheckCharges").text(AccountingFormat(total));
                        //$("#lblSalesPayCreditCardCharges").text(AccountingFormat(total));
                        //$("#lblSalesPayDebitCardCharges").text(AccountingFormat(total));
                        //$("#lblSalesPaySRVCharges").text(AccountingFormat(total));
                        //$("#lblSalesPayOtherTypeCharges").text(AccountingFormat(total));

                        $("#lblBillTotalCharges").text(AccountingFormat(total));

                        if (tip > 0) {
                            $("#divPaymentsTip").show();
                            $("#lblSalesPayTip").text(tip > 0 ? tip : AccountingFormat(tip));
                        } else {
                            $("#divPaymentsTip").hide();
                            $("#lblSalesPayTip").text('');

                        }


                        if (otherTax > 0) {
                            $("#divPaymentsOtherTax").show();
                            $("#lblSalesPayOtherTax").text(otherTax > 0 ? otherTax : AccountingFormat(otherTax));

                        } else {
                            $("#divPaymentsOtherTax").hide();
                            $("#lblSalesPayOtherTax").text('');

                        }

                    }
                },
                DisplayChargesList: function (vRows) {
                    var listCharge = $('#listCharge');
                    var isEmpty = true;
                    listCharge.empty();

                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            listCharge.append($('<li class="list-group-item">')
                                    .data(vRows[i])
                                    .append($('<div>').html('Charge Type: ' + vRows[i].ChargeType))
                                    .append($('<div>').html('Amount: ' + AccountingFormat(CDec2(vRows[i].ChargeAmount) + CDec2(vRows[i].LocalTaxAmount) + CDec2(vRows[i].TipAmount))))
                                    .append($('<div>').html('Remarks: ' + vRows[i].Remarks))
                                )
                            isEmpty = false;
                        }
                        listCharge.find('li').first().addClass('navList-a');
                    }

                    if (isEmpty) {
                        listCharge.append($('<li class="list-group-item">').text('List is empty'));
                    }
                    $('#divListOfCharges').pageshow();
                    PopupReposition();
                }
            },
            SoldTo: {
                form: null,
                txtSoldToSearch: null,
                txtCurrentCustomerName: null,
                selCustomerType: null,
                txtCustomerOrgName: null,
                txtCustomerOrgTin: null,
                txtCustomerOrgBldgNo: null,
                txtCustomerOrgStreet: null,
                selCustomerOrgCity: null,
                selCustomerOrgCountry: null,
                isFinishedRequiredData: false,
                txtCustomerPersonFName: null,
                txtCustomerPersonMName: null,
                txtCustomerPersonLName: null,
                txtCustomerPersonTin: null,
                init: function () {
                    if (proj.CacMode == 'AP') {
                        NewMessageBox('This function is not yet implemented.');
                        return;
                    }
                    this.form = $('#formFMCGSoldTo');
                    this.txtSoldToSearch = $('[name="txtSoldToSearch"]');
                    this.txtCurrentCustomerName = $('#txtCurrentCustomerName');
                    this.selCustomerType = this.form.find('[name="selCustomerType"]');
                    this.txtCustomerOrgName = this.form.find('[name="txtCustomerOrgName"]');
                    this.txtCustomerOrgTin = this.form.find('[name="txtCustomerOrgTin"]');
                    this.txtCustomerOrgBldgNo = this.form.find('[name="txtCustomerOrgBldgNo"]');
                    this.txtCustomerOrgStreet = this.form.find('[name="txtCustomerOrgStreet"]');
                    this.selCustomerOrgCity = this.form.find('[name="selCustomerOrgCity"]');
                    this.selCustomerOrgCountry = this.form.find('[name="selCustomerOrgCountry"]');
                    this.txtCustomerPersonFName = this.form.find('[name="txtCustomerPersonFName"]');
                    this.txtCustomerPersonMName = this.form.find('[name="txtCustomerPersonMName"]');
                    this.txtCustomerPersonLName = this.form.find('[name="txtCustomerPersonLName"]');
                    this.txtCustomerPersonTin = this.form.find('[name="txtCustomerPersonTin"]');

                    CacPopup.popupname = 'soldto';
                    $('#popFMCGSoldTo').popup('open');

                    if (this.isFinishedRequiredData == false) {
                        this.LoadRequiredData()
                        this.isFinishedRequiredData = true;
                    }

                    this.Main();

                    BindKeypress('#formFMCGSoldTo', this.selCustomerType, false, 'select');
                    BindKeypress('#formFMCGSoldTo', this.txtCustomerOrgName, false, 'text');
                    BindKeypress('#formFMCGSoldTo', this.txtCustomerOrgTin, true, 'text');
                    BindKeypress('#formFMCGSoldTo', this.txtCustomerOrgBldgNo, false, 'text');
                    BindKeypress('#formFMCGSoldTo', this.txtCustomerOrgStreet, false, 'text');
                    BindKeypress('#formFMCGSoldTo', this.selCustomerOrgCity, false, 'select');
                    BindKeypress('#formFMCGSoldTo', this.selCustomerOrgCountry, false, 'select');
                    BindKeypress('#formFMCGSoldTo', this.txtCustomerPersonFName, false, 'text');
                    BindKeypress('#formFMCGSoldTo', this.txtCustomerPersonMName, false, 'text');
                    BindKeypress('#formFMCGSoldTo', this.txtCustomerPersonLName, false, 'text');
                    BindKeypress('#formFMCGSoldTo', this.txtCustomerPersonTin, true, 'text');

                    this.txtSoldToSearch.unbind('keydown').keydown(function (e) {
                        if (e.which == 13 && $.trim($(this).val()) != "") {
                            e.preventDefault();
                            var vRows = OfflineBrowser.LibraryOrganization.GetData({
                                keywords: $.trim($(this).val()),
                                FilterCustomers: 1
                            }).Rows
                            CacPopup.SoldTo.LoadAvailableSearchedCustomer(vRows);
                            CacPopup.isInAddPage = true;
                            $(this).focusout();
                        }
                        else if (e.which == 107) {
                            // 107  = +
                            e.preventDefault();
                            CacPopup.SoldTo.Add();
                        }
                    })

                    this.selCustomerType.change(function () {
                        CacPopup.SoldTo.Add();
                    })

                    this.form.unbind('submit').submit(function (e) {
                        e.preventDefault();
                        if (CacPopup.SoldTo.selCustomerType.val() == "Organization") {
                            CacPopup.SoldTo.ValidateNewCustomerOrg();
                        }
                        else {
                            CacPopup.SoldTo.ValidateNewCustomerPerson();
                        }
                    })
                },
                ValidateNewCustomerOrg: function () {
                    if ($.trim(this.txtCustomerOrgName.val()) == "" || $.trim(this.txtCustomerOrgTin.val()) == "") {
                        $('#txtSoldToErrorMsgRequired').pageshow();
                    }
                    else {
                        $('#txtSoldToErrorMsgRequired').hide();

                        var pDataOrg = {
                            IsOrganization: 1,
                            OrganizationID: '',
                            PersonTitleID: '',
                            PersonDOB: '',
                            CompanyName: this.txtCustomerOrgName.val(),
                            TIN: $.trim(this.txtCustomerOrgTin.val()),
                            OrgTradeNameList: [],
                            OrgCategoryList: [],
                            IsAddAsCustomer: 1,
                            CompanyNameInitial: '',
                            IsFranchisor: 0,
                            OfficeAddressBuildingNo: $.trim(this.txtCustomerOrgBldgNo.val()),
                            OfficeAddressStreet: $.trim(this.txtCustomerOrgStreet.val()), //temporarily store the free type address here
                            OfficeAddressCityID: this.selCustomerOrgCity.val(),
                            OfficeAddressCountryID: this.selCustomerOrgCountry.val(),
                            BusinessAddressBuildingNo: $.trim(this.txtCustomerOrgBldgNo.val()),
                            BusinessAddressStreet: $.trim(this.txtCustomerOrgStreet.val()),
                            BusinessAddressCityID: this.selCustomerOrgCity.val(),
                            BusinessAddressCountryID: this.selCustomerOrgCountry.val()
                        }

                        if (OfflineMode) {
                            result = SalesPayment_offline().SaveOrganizationInfo(pDataOrg);
                            if (result != 'Existing') {
                                sessionStorage.CustomerOrganizationID = result;
                                pageCustomerDetails.LoadCustomer();
                                this.Main();
                            } else {
                                $('#txtSoldToExist').pageshow();
                                $('#txtKindExistName').text('Organization with the same corporate name already exists.');
                            }

                        }
                    }
                },
                ValidateNewCustomerPerson: function () {
                    if ($.trim(this.txtCustomerPersonFName.val()) == "" || $.trim(this.txtCustomerPersonMName.val()) == "" || $.trim(this.txtCustomerPersonLName.val()) == "") {
                        $('#txtSoldToErrorMsgRequired').pageshow();
                    }
                    else if ($.trim(this.txtCustomerPersonFName.val()).length <= 1 || $.trim(this.txtCustomerPersonMName.val()).length <= 1 || $.trim(this.txtCustomerPersonLName.val()).length <= 1) {
                        $('#txtSoldToFullName').pageshow();
                    }
                    else {
                        $('#txtSoldToErrorMsgRequired, #txtSoldToFullName').hide();

                        var pDataPerson = {
                            IsOrganization: 0,
                            OrganizationID: '',
                            PersonFirstName: this.txtCustomerPersonFName.val(),
                            PersonMiddleName: this.txtCustomerPersonMName.val(),
                            PersonSurname: this.txtCustomerPersonLName.val(),
                            PersonSuffixName: '',
                            PersonTitleID: '',
                            PersonDOB: '',
                            PersonMotherMaidenSurname: '',
                            PersonMotherMaidenFirstName: '',
                            PersonMotherMaidenMiddleName: '',
                            OrgTradeNameList: [],
                            OrgCategoryList: [],
                            IsAddAsCustomer: 1,
                            CompanyNameInitial: '',
                            IsFranchisor: 0,
                            TIN: $.trim(this.txtCustomerPersonTin.val()),
                            OfficeAddressBuildingNo: '',
                            OfficeAddressStreet: '',
                            OfficeAddressCityID: '',
                            OfficeAddressCountryID: '',
                            BusinessAddressBuildingNo: '',
                            BusinessAddressStreet: '',
                            BusinessAddressCityID: '',
                            BusinessAddressCountryID: ''
                        };

                        if (OfflineMode) {
                            result = SalesPayment_offline().SaveOrganizationInfo(pDataPerson);
                            if (result != 'Existing') {
                                sessionStorage.CustomerOrganizationID = result;
                                pageCustomerDetails.LoadCustomer();
                                this.Main();
                            } else {
                                $('#txtSoldToExist').pageshow();
                                $('#txtKindExistName').text('Person with the same information already exists.');
                            }
                        }
                    }
                },
                Add: function () {
                    //this.selCustomerType.val('Organization');
                    this.txtCustomerOrgName.val('');
                    this.txtCustomerOrgTin.val('');
                    this.txtCustomerOrgBldgNo.val('');
                    this.txtCustomerOrgStreet.val('');
                    this.selCustomerOrgCity.val('');
                    this.selCustomerOrgCountry.val('');
                    this.txtCustomerPersonFName.val('');
                    this.txtCustomerPersonMName.val('');
                    this.txtCustomerPersonLName.val('');
                    this.txtCustomerPersonTin.val('');
                    $('#divNewCustomer').pageshow();
                    $('#txtSoldToErrorMsgRequired, #txtSoldToExist, #txtSoldToFullName').hide();

                    if (this.selCustomerType.val() == "Organization") {
                        $('#divFMCGCustomerOrg').pageshow();
                        this.txtCustomerOrgName.removeAttr('hidden');
                        this.txtCustomerOrgTin.removeAttr('hidden');
                        this.txtCustomerOrgBldgNo.removeAttr('hidden');
                        this.txtCustomerOrgStreet.removeAttr('hidden');
                        this.selCustomerOrgCity.removeAttr('hidden');
                        this.selCustomerOrgCountry.removeAttr('hidden');
                        this.txtCustomerPersonFName.attr('hidden', '');
                        this.txtCustomerPersonMName.attr('hidden', '');
                        this.txtCustomerPersonLName.attr('hidden', '');
                        this.txtCustomerPersonTin.attr('hidden', '');
                        this.selCustomerOrgCountry.val('e783ed13-aa98-4cc1-918e-d49b05c0167e')
                    }
                    else {
                        $('#divFMCGCustomerPerson').pageshow();
                        this.txtCustomerOrgName.attr('hidden', '');
                        this.txtCustomerOrgTin.attr('hidden', '');
                        this.txtCustomerOrgBldgNo.attr('hidden', '');
                        this.txtCustomerOrgStreet.attr('hidden', '');
                        this.selCustomerOrgCity.attr('hidden', '');
                        this.selCustomerOrgCountry.attr('hidden', '');
                        this.txtCustomerPersonFName.removeAttr('hidden');
                        this.txtCustomerPersonMName.removeAttr('hidden');
                        this.txtCustomerPersonLName.removeAttr('hidden');
                        this.txtCustomerPersonTin.removeAttr('hidden');
                    }

                    PopupReposition();
                    setTimeout(function () {
                        CacPopup.SoldTo.selCustomerType.focus();
                    }, 100)
                },
                Main: function () {
                    $('#divListOfCustomer').pageshow();
                    this.txtSoldToSearch.val('').focus();
                    this.txtCurrentCustomerName.text(sessionStorage.CustomerOrganizationName);
                    $('#listAvailabelCustomer').empty();
                    PopupReposition();
                },
                LoadRequiredData: function () {
                    var city = SalesMenu_offline().getCityListPH().CityData.Rows;
                    var country = SalesMenu_offline().getCountryListAll().CountryData.Rows;

                    this.selCustomerOrgCity.empty();
                    this.selCustomerOrgCountry.empty();
                    this.selCustomerOrgCity.append(new Option('Please Select City', ''))
                    this.selCustomerOrgCountry.append(new Option('Please Select Country', ''))

                    for (var i = 0; i < city.length; i++) {
                        this.selCustomerOrgCity.append(new Option((city[i].City || ''), city[i].ID))
                    }

                    for (var i = 0; i < country.length; i++) {
                        this.selCustomerOrgCountry.append(new Option(country[i].Country, country[i].ID))
                    }
                    this.selCustomerOrgCountry.val('e783ed13-aa98-4cc1-918e-d49b05c0167e'); //set Phil as default
                },
                LoadAvailableSearchedCustomer: function (vRows) {
                    var listAvailabelCustomer = $('#listAvailabelCustomer');
                    listAvailabelCustomer.empty();

                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            listAvailabelCustomer.append($('<li>')
                                    .attr('id', 'cust-' + vRows[i].ID)
                                    .data(vRows[i])
                                    .append($('<div>').html(vRows[i].CompanyName))
                                    .click(function () {
                                        CacPopup.SoldTo.SetAsCustomer($(this).data())
                                    })
                                )
                        }
                        listAvailabelCustomer.find('li').first().addClass('navList-a');
                    }
                    else {
                        listAvailabelCustomer.append('<li>No Search Result found</li>')
                    }
                    PopupReposition();
                },
                SetAsCustomer: function (data) {
                    sessionStorage.CustomerOrganizationID = data.ID;
                    sessionStorage.CustomerOrganizationName = data.CompanyName;
                    this.txtCurrentCustomerName.text(data.CompanyName);
                    $('#popFMCGSoldTo').popup('close');
                    FmcgBasicCart.txtQtyPlu.focus();
                }
            },
            Misc: {
                totalmisc: 0,
                Clear: function () {
                    this.totalmisc = 0;
                    $('#txtMiscTotalCharge, #txtMiscTotalSrv, #txtMiscTotalOp').text('0.00')
                },
                init: function () {
                    if (proj.CacMode == 'AP') {
                        NewMessageBox('This function is not yet implemented.');
                        return;
                    }
                    NewMessageBoxClose(); // NewMessageBox.js
                    $('#popFMCGMisc').popup('open');
                    CacPopup.popupname = 'misc';
                    $('#listMisc').find('li').first().addClass('navList-a');
                    $('#txtMiscTotalCharge').text((CacPopup.Charges.totalcharge == 0 ? '0.00' : AccountingFormat(CacPopup.Charges.totalcharge)))
                    $('#txtMiscTotalSrv').text((CacPopup.Srv.totalsrv == 0 ? '0.00' : AccountingFormat(CacPopup.Srv.totalsrv)))
                    $('#txtMiscTotalOp').text((CacPopup.OtherPayment.totalop == 0 ? '0.00' : AccountingFormat(CacPopup.OtherPayment.totalop)))
                },
                ShowPopup: function (popup) {
                    $('#popFMCGMisc').popup('close');

                    if (popup == "op") {
                        //Other Payment
                        CacPopup.OtherPayment.init();
                    }
                    else if (popup == "charges") {
                        //Charges and Other Tax
                        CacPopup.Charges.init();
                    }
                    else if (popup == "srv") {
                        //Charges and Other Tax
                        CacPopup.Srv.init();
                    }
                },
                TotalMisc: function () {
                    this.totalmisc = CDec2(CacPopup.Charges.totalcharge) + CDec2(CacPopup.Srv.totalsrv) + CDec2(CacPopup.OtherPayment.totalop);
                    $('.txtMisc').text((this.totalmisc == 0 ? '0.00' : AccountingFormat(this.totalmisc)))
                }
            },
            RewardsProgram: {
                CardNo: '',
                Clear: function () {
                    this.CardNo = '';
                    $('#txtRewardCardNo').val('');
                },
                init: function (params) {
                    if (proj.IsBootstrapDesign()) {
                        $('#popFMCGRewardsProgram-bs').modal('show');
                    } else {
                        $('#popFMCGRewardsProgram').popup('open');
                    }

                    CacPopup.params = params;

                    $('#btnRewardsProgramOk').unbind('click').click(function () {
                        var no = $.trim($('#txtRewardCardNo').val());

                        if (no == '') {
                            WebToast('Please enter rewards card number', 'top');
                            return;
                        }

                        CacPopup.params.success(no);
                        
                        if (proj.IsBootstrapDesign()) {
                            $('#popFMCGRewardsProgram-bs').modal('hide');
                        } else {
                            $('#popFMCGRewardsProgram').popup('close');
                        }
                    })
                }
            },
            OnCredit: {
                CreditAmount: 0,
                CreditRemarks: '',
                Clear: function () {
                    this.CreditAmount = 0;
                    this.CreditRemarks = '';
                    $('#txtOnCreditAmt, #txtOnCreditRemarks').val('');
                },
                init: function (params) {
                    if (proj.IsBootstrapDesign()) {
                        $('#popFMCGOnCredit-bs').modal('show');
                    } else {
                        $('#popFMCGOnCredit').popup('open');
                    }

                    CacPopup.params = params;

                    $('#btnOnCreditOk').unbind('click').click(function () {
                        var amt = CDec2($.trim($('#txtOnCreditAmt').val()));
                        var remarks = $.trim($('#txtOnCreditRemarks').val());

                        if (amt == 0) {
                            WebToast('Please enter amount', 'top');
                            return;
                        }

                        CacPopup.OnCredit.CreditAmount = amt;
                        CacPopup.OnCredit.CreditRemarks = remarks;

                        CacPopup.params.success({
                            CreditAmount: CacPopup.OnCredit.CreditAmount,
                            CreditRemarks: CacPopup.OnCredit.CreditRemarks
                        });

                        if (proj.IsBootstrapDesign()) {
                            $('#popFMCGOnCredit-bs').modal('hide');
                        } else {
                            $('#popFMCGOnCredit').popup('close');
                        }
                    })
                }
            },
            RefreshSalesPaymentTotal: function () {
                var vDataFooter;

                if (OfflineMode) {
                    vDataFooter = SalesPayment_offline().getSalesPaymentReceiptFooterAmountsInfo(sessionStorage.CartOrderID);
                    this.ShowSalesPaymentTotalResults(vDataFooter);
                } else {
                    AjaxSend({
                        url: "SalesPayments/getSalesPaymentReceiptFooterAmountsInfo",
                        data: { InvoiceID: sessionStorage.CartOrderID },
                        success: function (vRetDataFooter) {
                            vDataFooter = vRetDataFooter.SalesPaymentData;
                            CacPopup.ShowSalesPaymentTotalResults(vDataFooter);
                        }
                    });
                }
            },
            ShowSalesPaymentTotalResults: function (vDataFooter) {
                var dblTotalSalesPay = 0;
                var dblTotalAmountDue = 0;
                var dblDiscount = 0;
                var dblCharges = 0;
                var dblTip = 0;
                var dblLocalTaxAmount = 0;
                var cashAmountTendered = parseFloat(CDec2(CacPopup.Cash.cashTendered));


                if (vDataFooter.Rows.length > 0) {

                    dblDiscount = vDataFooter.Rows[0].DiscountAmount;

                    dblCharges = CDec2(vDataFooter.Rows[0].ChargeAmount);
                    dblTip = CDec2(vDataFooter.Rows[0].TipAmount);
                    dblLocalTaxAmount = CDec2(vDataFooter.Rows[0].LocalTaxAmount);

                    CacPopup.Discount.totaldiscount = dblDiscount;
                    CacPopup.Charges.totalcharge = dblCharges;
                    CacPopup.Charges.totalothertax = dblLocalTaxAmount;
                }

            },
            ComputePayment: function () {
                var cashAmountTendered = parseFloat(CDec2(CacPopup.Cash.cashTendered));

                var dblTotalSalesPay = 0;
                var dblTotalAmountDue = 0;
                var amtDue = CacPopup.Discount.isSeniorCitizenDiscount ? $('#lblTotalAmount').text().replace(/,/g, "") : $('#txtTotal').text().replace(/,/g, "");

                dblTotalAmountDue = parseFloat(CDec2(amtDue)) - parseFloat(CacPopup.Discount.totaldiscount) - parseFloat(CacPopup.Charges.totalcharge) - parseFloat(CacPopup.Charges.totalothertax) - parseFloat(0) // amtDue - totaldiscount - charges - other tax - tip
                dblTotalSalesPay = (parseFloat(CDec2(CacPopup.Cash.cashTendered)) + parseFloat(CDec2(CacPopup.ChequeGC.totalcgc)) + parseFloat(CDec2(CacPopup.Card.totalcardamt)) + parseFloat(CDec2(CacPopup.Srv.totalsrv)) + parseFloat(CDec2(CacPopup.OtherPayment.totalop))) - (parseFloat(CDec2(amtDue)) - parseFloat(CDec2(0)) + parseFloat(CDec2(CacPopup.Charges.totalcharge)) + parseFloat(CDec2(0)) + parseFloat(CDec2(0)));
                // (parseFloat(CDec2(cash)) + parseFloat(CDec2(cgc)) + parseFloat(CDec2(debitcard)) + parseFloat(CDec2(creditcard)) + parseFloat(CDec2(srv)) + parseFloat(CDec2(othertype)))
                // - (parseFloat(CDec2(amtDue)) + parseFloat(CDec2(discount)) + parseFloat(CDec2(charges)) + parseFloat(CDec2(tip)) + parseFloat(CDec2(othertax)))

                var totalAmountTendered = parseFloat(CDec2(CacPopup.Cash.cashTendered)) + parseFloat(CDec2(CacPopup.ChequeGC.totalcgc)) + parseFloat(CDec2(CacPopup.Card.totalcardamt)) + parseFloat(CDec2(CacPopup.Srv.totalsrv)) + parseFloat(CDec2(CacPopup.OtherPayment.totalop));
                CacPopup.amounttendered = CDec2(totalAmountTendered);

                if (dblTotalSalesPay >= 0) {
                    $('#lblBalChange').text('CHANGE');
                    if ((totalAmountTendered - dblTotalAmountDue) > 0) {
                        //if the cash amount tendered is greater than the change, then return the cash amount
                        if (dblTotalSalesPay > cashAmountTendered)
                            $("#txtBalChange").text(cashAmountTendered > 0 ? AccountingFormat(cashAmountTendered) : '0.00');
                        else if (cashAmountTendered > 0)
                            $("#txtBalChange").text((dblTotalSalesPay > 0 ? AccountingFormat(dblTotalSalesPay) : '0.00'));
                        else
                            $("#txtBalChange").text('0.00') // $("#txtBalChange").text("No Change");
                    } else {
                        $("#txtBalChange").text((dblTotalSalesPay > 0 ? AccountingFormat(dblTotalSalesPay) : '0.00'));
                    }
                } else {
                    $('#lblBalChange').text('UNPAID');
                    $("#txtBalChange").text(AccountingFormat(-dblTotalSalesPay));
                }
            },
            Scroll: function (scrollableDiv, el) {
                ScrollToMiddle(scrollableDiv, el)
            },
            BrowseBank: function(vParams) {
                BrowsePopup('LibraryOrganizationBanks', 'Banks',
                    [
                        {
                            AutoSearch: true,
                            Sorting: false,
                            HidePrevPopup: false,
                            DisplayText: [
                                { 
                                    Column: 'CompanyName', 
                                    Label: 'Bank Name' 
                                }
                            ],
                            Click: function (e) {
                                vParams.Click(e);
                            },
                            Close: function() {
                                vParams.Close();
                            },
                            AllowEmptySearch: true,
                            StyleFormat: 'table'
                        }
                    ], { RankIfEmptySeach: 1 }
                );
            } // END BrowseBank: function(vParams) {
        }

        window.NavigateList = function (list, popupname, dir) {
            var lastVoidProductSelected = list.find('li.navList-a');
            var curListIndex = list.find('.navList-a').index();

            if (list.find('li').hasClass('navList-a')) {
                list.find('li.navList-a').removeClass('navList-a')
                if (dir == 38) {
                    // arrow up
                    list.find('li').eq(curListIndex - 1).addClass('navList-a');
                }
                else if (dir == 40) {
                    //arrow down
                    if (lastVoidProductSelected.is('li:last-child')) {
                        list.find('li').eq(0).addClass('navList-a')
                    }
                    else {
                        list.find('li').eq(curListIndex + 1).addClass('navList-a');
                    }
                }
                else if (dir == 13) {
                    //enter
                    if (popupname == 'cgc') {
                        CacPopup.ChequeGC.Edit(lastVoidProductSelected.data())
                    }
                    else if (popupname == 'card') {
                        CacPopup.Card.Edit(lastVoidProductSelected.data())
                    }
                    else if (popupname == 'srv') {
                        CacPopup.Srv.Edit(lastVoidProductSelected.data())
                    }
                    else if (popupname == 'op') {
                        CacPopup.OtherPayment.Edit(lastVoidProductSelected.data())
                    }
                    else if (popupname == 'soldto') {
                        CacPopup.SoldTo.SetAsCustomer(lastVoidProductSelected.data())
                    }
                    else if (popupname == 'misc') {
                        CacPopup.Misc.ShowPopup(lastVoidProductSelected.attr('popup'));
                    }
                }
                else if (dir == 46) {
                    //delete
                    if (popupname == 'cgc') {
                        CacPopup.ChequeGC.Delete(lastVoidProductSelected.data())
                    }
                    else if (popupname == 'card') {
                        CacPopup.Card.Delete(lastVoidProductSelected.data())
                    }
                    else if (popupname == 'srv') {
                        CacPopup.Srv.Delete(lastVoidProductSelected.data())
                    }
                    else if (popupname == 'op') {
                        CacPopup.OtherPayment.Delete(lastVoidProductSelected.data())
                    }
                    else if (popupname == 'charges') {
                        CacPopup.Charges.Delete(lastVoidProductSelected.data())
                    }
                }
                CacPopup.Scroll('#divAvailableScroll', list.find('li.navList-a'))
            }
        }

        function BindKeypress(form, elem, isNumber, inputType) {
            var prevSelectVal = '';
            var isSelectOpen = false;
            elem
                .unbind('keydown change')
                .attr('form', form)
                .attr('isnum', isNumber)
                .attr('inputtype', inputType)
                .on('keydown change', function (e) {
                    isSubmit = false;
                    if ($(this).attr('inputtype') == "text") {
                        if (CBool($(this).attr('isnum')) && !e.ctrlKey) {
                            if ((e.which != 46 || $(this).val().indexOf('.') != -1) && (e.which < 48 || e.which > 57)) {
                                // only nums and one decimal are allowed to input
                                if (e.which != 13 && (e.which < 96 || e.which > 105)) { // e.which < 96 || e.which > 105 using num pad
                                    if (e.which != 110 && e.which != 190 && e.which != 8 && e.which != 37 && e.which != 39) { // if user input '.' , it must not prevent default event (110(.) using numpad & 190(.) using char keys & 8 is backspace)
                                        e.preventDefault();
                                    }

                                    if ($(this).val().match(/\./g) != null && e.which != 8 && e.which != 37 && e.which != 39) { // only one decimal is allowed to input in textbox
                                        $(this).val().match(/\./g).length != 2 ? e.preventDefault() : ''
                                    }
                                }
                            }
                        }

                        if ($(this).attr('type') != 'number' && (e.which == 38 || e.which == 40)) { // arrow up & arrow down
                            $('[name="' + nextOnTabIndex($(this), e.which).attr('name') + '"]').focus();
                        }
                        else if ($(this).attr('type') == 'number' && (e.which == 38 || e.which == 40)) {
                            e.preventDefault(); // type number only
                            $('[name="' + nextOnTabIndex($(this), e.which).attr('name') + '"]').focus();
                        }

                        if (e.which == 13) { // submit form
                            $(form).submit()
                        }
                    }
                    else if ($(this).attr('inputtype') == "select") {
                        if (e.which == 38 || e.which == 40) {
                            //arrow up or down
                            $('[name="' + nextOnTabIndex($(this), e.which).attr('name') + '"]').focus()
                            e.preventDefault(); // do not show dropdown if using arrow keys
                        }
                        else if (e.which == 13) {
                            e.preventDefault();
                            $('#select-' + $(this).attr('name')).click();
                        }
                    }
                })

            //TO BE FIXED SOON
            //if (elem.attr('inputtype') == "select") {
            //    var time = setInterval(function () {
            //        if (elem.customselect()) {
            //            clearInterval(time)
            //        }
            //    }, 100)
            //}
        }

        function nextOnTabIndex(elem, keyCode) {
            var fields = $($(elem.attr('form'))
                          .find('input:not([type="submit"]):not(:hidden):not([readonly]), select:not([hidden]), textarea')
                          .toArray()
                          //.sort(function (a, b) {
                          //    return ((a.tabIndex > 0) ? a.tabIndex : 1000) - ((b.tabIndex > 0) ? b.tabIndex : 1000);
                          //})
                          );

            if (keyCode == 38) {
                //arrow up
                return fields.eq(((fields.index(elem) - 1)))
            }
            else {
                // arrow up & enter
                return fields.eq((fields.index(elem) + 1) % fields.length);
            }
        }
    })
    .on('keydown', function (e) {
        if (e.which == 107) {
            // pressing + (means showing addpage)
            if (CacPopup.popupname == "cgc") {
                //Cheque and Gift Certicate
                CacPopup.ChequeGC.Add();
            }
            else if (CacPopup.popupname == "card") {
                //Cheque and Gift Certicate
                CacPopup.Card.Add();
            }
            else if (CacPopup.popupname == "srv") {
                //SRV
                CacPopup.Srv.Add();
            }
            else if (CacPopup.popupname == "op") {
                //Other Payment
                CacPopup.OtherPayment.Add();
            }
            else if (CacPopup.popupname == "charges") {
                //Charges and Other Tax
                CacPopup.Charges.Add();
            }
            else if (CacPopup.popupname == "soldto") {
                //Sold To
                CacPopup.SoldTo.Add();
            }
        }
        // disable minue (-) shortcut key for now 2017_08_04__15_44_25_4454
        //else if (e.which == 109) {
        //    // pressing - (means go back to mainpage)
        //    if (CacPopup.popupname == "cgc") {
        //        //Cheque and Gift Certicate
        //        CacPopup.ChequeGC.Main();
        //    }
        //    else if (CacPopup.popupname == "card") {
        //        //Cheque and Gift Certicate
        //        CacPopup.Card.Main();
        //    }
        //    else if (CacPopup.popupname == "srv") {
        //        //SRV
        //        CacPopup.Srv.Main();
        //    }
        //    else if (CacPopup.popupname == "op") {
        //        //Other Payment
        //        CacPopup.OtherPayment.Main();
        //    }
        //    else if (CacPopup.popupname == "charges") {
        //        //Charges and Other Tax
        //        CacPopup.Charges.Main();
        //    }
        //    else if (CacPopup.popupname == "soldto") {
        //        //Charges and Other Tax
        //        CacPopup.SoldTo.Main();
        //    }
        //}
        else if (e.which == 38 || e.which == 40) {
            // arrow up and down and enter - used only in popup with list, to navigate within a list
            if (CacPopup.popupname == "cgc") {
                NavigateList($('#listCGC'), CacPopup.popupname, e.which)
            }
            else if (CacPopup.popupname == "card") {
                NavigateList($('#listCard'), CacPopup.popupname, e.which)
            }
            else if (CacPopup.popupname == "srv") {
                NavigateList($('#listSrv'), CacPopup.popupname, e.which)
            }
            else if (CacPopup.popupname == "op") {
                NavigateList($('#listOP'), CacPopup.popupname, e.which)
            }
            else if (CacPopup.popupname == "soldto") {
                NavigateList($('#listAvailabelCustomer'), CacPopup.popupname, e.which)
            }
            else if (CacPopup.popupname == "misc") {
                NavigateList($('#listMisc'), CacPopup.popupname, e.which)
            }
        }
        else if (e.which == 13 || e.which == 46) {
            //For editing & deleting list in popup payments // ENTER OR DELETE key
            if (CacPopup.popupname == "cgc" && CacPopup.isInAddPage == false) {
                NavigateList($('#listCGC'), CacPopup.popupname, e.which)
            }
            if (CacPopup.popupname == "card" && CacPopup.isInAddPage == false) {
                NavigateList($('#listCard'), CacPopup.popupname, e.which)
            }
            else if (CacPopup.popupname == "srv" && CacPopup.isInAddPage == false) {
                NavigateList($('#listSrv'), CacPopup.popupname, e.which)
            }
            else if (CacPopup.popupname == "op" && CacPopup.isInAddPage == false) {
                NavigateList($('#listOP'), CacPopup.popupname, e.which)
            }
            else if (CacPopup.popupname == "charges" && CacPopup.isInAddPage == false) {
                NavigateList($('#listCharge'), CacPopup.popupname, e.which)
            }
            else if (CacPopup.popupname == "soldto" && CacPopup.isInAddPage == false) {
                NavigateList($('#listAvailabelCustomer'), CacPopup.popupname, e.which)
            }
            else if (CacPopup.popupname == "misc" && CacPopup.isInAddPage == false) {
                if (e.which != 46) {
                    // DELETE not allowed
                    NavigateList($('#listMisc'), CacPopup.popupname, e.which)
                }
            }

            CacPopup.isInAddPage = false;
        }
    });
})