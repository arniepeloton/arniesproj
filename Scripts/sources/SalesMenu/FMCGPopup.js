require([
    'common',
    '/Scripts3/Offline/Browser/LibraryOrganization.js?crc=',
    '/Scripts3/Offline/Browser/LibraryOrganizationBanks.js',
    '/Scripts3/Offline/Browser/LibraryCountries.js'
], function () {
    $(document).ready(function () {
        var isDiscountTypeLoaded = false;

        window.Popup = {
            params: null,
            popupname: '',
            returnVal: null,
            isInAddPage: false,
            amounttendered: 0,
            ChequeGC: {
                txtNewCGC: null,
                txtCGCID: null,
                selCGCBank: null,
                txtCGCBranchID: null,
                txtCGCBranch: null,
                txtCGCAccountName: null,
                txtCGCAccountNo: null,
                txtCGCChequeGCNo: null,
                txtCGCValue: null,
                btnChequeDate: null,
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
                    Popup.popupname = '';

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
                    this.txtCGCChequeGCNo = this.form.find('[name="txtCGCChequeGC"]');
                    this.txtCGCValue = this.form.find('[name="txtCGCValue"]');
                    this.btnChequeDate = this.form.find('[id="btnChequeDate"]');
                    this.btnCGCBack = this.form.find('[id="btnCGCBack"]');
                    Popup.params = param;

                    $('#popFMCGChequeGC').popup('open');
                    Popup.popupname = 'cgc';

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

                    $('#btnAddCGC').unbind('click').click(function () {
                        Popup.ChequeGC.Add();
                    })

                    this.btnChequeDate.unbind('click').click(function (e) {
                        e.preventDefault();
                        SingleCalendar({
                            Click: function (e) {
                                $('#popFMCGChequeGC').popup('open');
                                Popup.ChequeGC.btnChequeDate.text(e);
                                Popup.ChequeGC.txtCGCValue.focus();
                            },
                            Close: function () {
                                $('#popFMCGChequeGC').popup('open');
                                Popup.ChequeGC.txtCGCValue.focus();
                                Popup.popupname = 'cgc';
                            }
                        })
                    })

                    $('#formFMCGChequeGC').unbind('submit').submit(function (e) {
                        e.preventDefault();
                        if ($.trim(Popup.ChequeGC.selCGCBank.val()) == "" || $.trim(Popup.ChequeGC.txtCGCAccountNo.val()) == ""
                            || $.trim(Popup.ChequeGC.txtCGCChequeGCNo.val()) == "" || $.trim(Popup.ChequeGC.btnChequeDate.text()) == "Set Date"
                            || $.trim(Popup.ChequeGC.txtCGCValue.val()) == "") {
                            $('#txtChequeGCMsg').show();
                            PopupReposition();
                            //ResizePopupBackground();
                        }
                        else {
                            $('#txtChequeGCMsg').hide();
                            var chequeDate = Popup.ChequeGC.btnChequeDate.text();

                            if (IsDate(chequeDate)) {
                                chequeDate = new Date(chequeDate);
                            } else {
                                chequeDate = false;
                            }

                            var pData = [
                                        Popup.ChequeGC.txtCGCID.val(), // 0
                                        Popup.ChequeGC.selCGCBank.val(), // 1
                                        Popup.ChequeGC.selCGCBank.find('option:selected').text(), // 2
                                        Popup.ChequeGC.txtCGCBranchID.val(), // 3
                                        Popup.ChequeGC.txtCGCBranch.val(), // 4
                                        Popup.ChequeGC.txtCGCAccountName.val(), // 5
                                        Popup.ChequeGC.txtCGCAccountNo.val(), // 6
                                        Popup.ChequeGC.txtCGCChequeGCNo.val(), // 7
                                        (chequeDate == false ? "" : chequeDate.getMonth() + 1), // 11 Cheque Date Month
                                        (chequeDate == false ? "" : chequeDate.getDate()), // 12 Cheque Date Day
                                        (chequeDate == false ? "" : chequeDate.getFullYear()), // 13  Cheque Date Year
                                        Popup.ChequeGC.txtCGCValue.val(), // 11
                                        Popup.ChequeGC.cgcState // 12
                            ];
                            if (Popup.ChequeGC.editIndex == null) {
                                Popup.ChequeGC.arrCGC.push(pData);
                            }
                            else {
                                Popup.ChequeGC.arrCGC[Popup.ChequeGC.editIndex] = pData
                                Popup.ChequeGC.editIndex = null; // set to null after edit
                            }

                            Popup.isInAddPage = true;
                            Popup.ChequeGC.intSalesPayCustomerNameIndex = Popup.ChequeGC.intSalesPayCustomerNameIndex + 1;
                            Popup.ChequeGC.LoadSalesPayCheckList();
                            Popup.returnVal = Popup.ChequeGC.arrCGC;
                            //$('#popFMCGChequeGC').popup('close');
                        }
                    })

                    this.btnCGCBack.unbind('click').click(function (e) {
                        e.preventDefault();
                        $('#pageListOfCGC').pageshow();
                        PopupReposition();
                    })
                },
                Add: function () {
                    this.txtCGCID.val('');
                    this.selCGCBank.val('');
                    this.txtCGCBranchID.val('');
                    this.txtCGCBranch.val('');
                    this.txtCGCAccountName.val('');
                    this.txtCGCAccountNo.val('');
                    this.txtCGCChequeGCNo.val('');
                    this.txtCGCValue.val('');
                    this.btnChequeDate.text('Set Date');
                    this.cgcState = 'New';

                    $('#pageNewCGG').pageshow();
                    $('#txtChequeGCMsg').hide();
                    PopupReposition(); //NewMessageBox.js
                    Popup.ChequeGC.selCGCBank.focus();
                    //ResizePopupBackground();
                },
                Edit: function (data) {
                    var chequedate = new Date(data[8] + '/' + data[9] + '/' + data[10]);

                    this.txtCGCID.val(data[0]);
                    this.selCGCBank.val(data[1]);
                    this.txtCGCBranchID.val(data[3]);
                    this.txtCGCBranch.val(data[4]);
                    this.txtCGCAccountName.val(data[5]);
                    this.txtCGCAccountNo.val(data[6]);
                    this.txtCGCChequeGCNo.val(data[7]);
                    this.txtCGCValue.val(data[11]);
                    this.btnCardIssuanceDate.text((IsDate(chequedate) ? $.format.date(chequedate, 'MMM dd, yyyy') : "Set Date"))
                    Popup.ChequeGC.cgcState = data[12];
                    this.editIndex = data[13];
                    $('#pageNewCGG').pageshow();
                    $('#txtChequeGCMsg').hide();
                    PopupReposition(); //NewMessageBox.js
                    //ResizePopupBackground();
                    setTimeout(function () {
                        Popup.ChequeGC.selCGCBank.focus();
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
                    //ResizePopupBackground();
                },
                LoadRequiredData: function () {
                    var vRows = Sql.Execute('SELECT ID, CompanyName FROM LibraryOrganization WHERE LibraryOrganization.IsABank = 1 ORDER BY LibraryOrganization.CompanyName').Rows;
                    this.selCGCBank.empty();
                    this.selCGCBank.append(new Option('Please Select', ''))
                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            this.selCGCBank.append(new Option(vRows[i].CompanyName, vRows[i].ID))
                        }
                    }
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
                                listCGC.append($('<li>')
                                    .data('data', dis)
                                    .append($('<div>').html(dis[7] + '<br>'))
                                    .append($('<div>').html('Amount: ' + (dis[11] == 0 ? '0.00' : AccountingFormat(dis[11]))))
                                    .click(function () {
                                        Popup.ChequeGC.Edit($(this).data('data'))
                                    })
                                )
                                isEmpty = false; // set to false when there's no remove state in arrCGC, found in index 12, w/c means add, edit, remove
                            }
                        }

                        listCGC.find('li').first().addClass('navList-a');
                    }

                    if (isEmpty) {
                        listCGC.append($('<li>').text('List is empty'));
                    }

                    $('#pageListOfCGC').pageshow();
                    //ResizePopupBackground();
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
                        $('.txtCGC').text((this.totalcgc > 0 ? AccountingFormat(this.totalcgc) : '0.00'))
                    }
                    Popup.ComputePayment();
                }
            },
            Discount: {
                selDiscountType: null,
                txtDiscountPercent: null,
                txtDiscountAmount: null,
                txtDiscountRemarks: null,
                txtSalesPayNoOfSC: null,
                txtSalesPayNoOfDiners: null,
                txtDiscountPercentCeiling: null,
                txtFullname: null,
                txtSurname: null,
                txtFirstName: null,
                txtMiddleName: null,
                selPersonalSuffixName: null,
                btnDiscountCustomerBirthday: null,
                txtIDText: null,
                txtOtherIDType: null,
                selIDType: null,
                form: null,
                formCustomerDetails: null,
                formFMCGDiscountCustomerID: null,
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
                SCDiscountExempt: 0,
                SubTotalOfVatExemptItems: 0,
                GrossSalesToSC: 0,
                GrossSalesToNoneSC: 0,
                VatExemptSales: 0,
                MaxSCDiscountableAmount: 0,
                seniorCitizenDiscount: 0,
                IsApplyMaxSCDiscountableAmount: false,
                seniorCitizenNetAmount: 0,
                longChargeAmount: 0,
                IsDeliveryInfoPriceQuote: null,
                strAccessType: "",
                intSalesPayCustomerNameCntr: 0,
                intSalesPayCustomerNameIndex: 0,
                totaldiscount: 0,
                arrCustomer: new Array(),
                arrCustomerIDs: new Array(),
                isNewCustomerID: false,
                indexCustomerID: 0,
                ctrIndexCustomerID: 0,
                isNewCustomer: false,
                Clear: function () {
                    this.isDiscountTypeLoaded = false;
                    this.isSeniorCitizenDiscount = false;
                    this.isSeniorCitizenPWDDiscount = false;
                    this.longDiscountPercent = null;
                    this.lblVATableSales = 0;
                    this.dblTotalAmountDueForDiscount = 0;
                    this.dblSubTotal = 0;
                    this.SCDiscountExempt = 0;
                    this.SubTotalOfVatExemptItems = 0;
                    this.GrossSalesToSC = 0;
                    this.GrossSalesToNoneSC = 0;
                    this.VatExemptSales = 0;
                    this.MaxSCDiscountableAmount = 0;
                    this.seniorCitizenDiscount = 0;
                    this.IsApplyMaxSCDiscountableAmount = false;
                    this.seniorCitizenNetAmount = 0;
                    this.taxPercent = 0;
                    this.longChargeAmount = 0;
                    this.IsDeliveryInfoPriceQuote = null;
                    this.strAccessType = "";
                    this.intSalesPayCustomerNameCntr = 0;
                    this.intSalesPayCustomerNameIndex = 0;
                    this.totaldiscount = 0;
                    this.arrCustomer = new Array();
                    this.arrCustomerIDs = new Array();
                    this.isNewCustomerID = false;
                    this.indexCustomerID = 0;
                    this.ctrIndexCustomerID = 0;
                    this.isNewCustomer = false;

                    $('[name="txtDiscountPercent"]').val('');
                    $('[name="txtDiscountAmount"]').val('');
                    $('[name="txtDiscountRemarks"]').val('');
                    $('#divDiscountCustomerDetails, #divDiscountCustomerList').hide();
                    $('#listDiscountCustomer').empty().append('<li>List is empty</li>');
                },
                init: function (params) {
                    var discount = Popup.Discount;
                    this.initElements();
                    this.popup = $('#popFMCGDiscount');
                    Popup.popupname = 'discount';

                    this.popup.popup('open');
                    $('#lblTitleDiscount').text('Discount');
                    $('#pageDiscountInfo').pageshow();

                    if (!FmcgBasicCart.isFromRecallCart && CDec2($('#txtLessDiscount').text()) == 0) {
                        this.selDiscountType.val('').focus();
                        this.txtDiscountPercent.val('');
                        this.txtDiscountAmount.val('');
                        this.txtDiscountRemarks.val('');
                        $('#divDiscountCustomerDetails, #divDiscountCustomerList').hide();
                        PopupReposition();
                    }

                    this.selDiscountType.focus();

                    BindKeypress('#formFMCGDiscounts', this.selDiscountType, false, 'select');
                    BindKeypress('#formFMCGDiscounts', this.txtDiscountPercent, true, 'text');
                    BindKeypress('#formFMCGDiscounts', this.txtDiscountAmount, true, 'text');
                    BindKeypress('#formFMCGDiscounts', this.txtDiscountRemarks, false, 'text');
                    BindKeypress('#formFMCGDiscounts', this.txtSalesPayNoOfSC, true, 'text');
                    BindKeypress('#formFMCGDiscounts', this.txtSalesPayNoOfDiners, true, 'text');

                    BindKeypress('#formFMCGDiscountCustomer', this.txtFullname, false, 'text');
                    BindKeypress('#formFMCGDiscountCustomer', this.txtSurname, false, 'text');
                    BindKeypress('#formFMCGDiscountCustomer', this.txtFirstName, false, 'text');
                    BindKeypress('#formFMCGDiscountCustomer', this.txtMiddleName, false, 'text');
                    BindKeypress('#formFMCGDiscountCustomer', this.selPersonalSuffixName, false, 'select');

                    BindKeypress('#formFMCGDiscountCustomerID', this.selIDType, false, 'select');
                    BindKeypress('#formFMCGDiscountCustomerID', this.txtIDText, false, 'text');
                    BindKeypress('#formFMCGDiscountCustomerID', this.txtOtherIDType, false, 'text');

                    if (this.isDiscountTypeLoaded == false) {
                        $('#divDiscountCustomerDetails').hide();
                        $('#divSCPWDDiscount').hide();

                        this.txtDiscountPercent.removeAttr("disabled");
                        this.txtDiscountAmount.removeAttr("disabled");
                        this.GetDiscountTypeList();
                        //this.GetCurrentDiscount();                        
                    }

                    //this.ShowSalesPaymentsInfo();

                    this.txtFullname.unbind('keyup').keyup(function () {
                        if ($.trim($(this).val()) == "") {
                            Popup.Discount.txtSurname.removeAttr('disabled').val('');
                            Popup.Discount.txtFirstName.removeAttr('disabled').val('');
                            Popup.Discount.txtMiddleName.removeAttr('disabled').val('');
                        } else {
                            Popup.Discount.txtSurname.attr('disabled', true);
                            Popup.Discount.txtFirstName.attr('disabled', true);
                            Popup.Discount.txtMiddleName.attr('disabled', true);
                        }
                    })

                    this.txtSurname.unbind('keyup').keyup(function () {
                        if ($.trim($(this).val()) == "" && $.trim(Popup.Discount.txtFirstName.val()) == "" && $.trim(Popup.Discount.txtMiddleName.val()) == "") {
                            Popup.Discount.txtFullname.removeAttr('disabled');
                        } else {
                            Popup.Discount.txtFullname.attr('disabled', true).val('');
                        }
                    })

                    this.txtFirstName.unbind('keyup').keyup(function () {
                        if ($.trim($(this).val()) == "" && $.trim(Popup.Discount.txtSurname.val()) == "" && $.trim(Popup.Discount.txtMiddleName.val()) == "") {
                            Popup.Discount.txtFullname.removeAttr('disabled', 'false');
                        } else {
                            Popup.Discount.txtFullname.attr('disabled', true).val('');
                        }
                    })

                    this.txtMiddleName.unbind('keyup').keyup(function () {
                        if ($.trim($(this).val()) == "" && $.trim(Popup.Discount.txtFirstName.val()) == "" && $.trim(Popup.Discount.txtSurname.val()) == "") {
                            Popup.Discount.txtFullname.removeAttr('disabled');
                        } else {
                            Popup.Discount.txtFullname.attr('disabled', true).val('');
                        }
                    })

                    $('#txtCustomerSearch').unbind('keypress').keypress(function (e) {
                        if (e.keyCode == 13) {
                            e.preventDefault();
                            BrowsePopup('LibraryOrganization', 'Customer', [
                                    {
                                        Search: $.trim($(this).val()),
                                        DisplayText: [{ Column: 'CompanyName' }],
                                        Validation: function(e){
                                            var isExist = false;

                                            for (var i = 0; i < discount.arrCustomer.length; i++){
                                                if (discount.arrCustomer[i][0] == e.ID) {
                                                    isExist = true;
                                                    break;
                                                }
                                            }

                                            if(isExist){
                                                WebToast('Selected customer already exist.', 'top');
                                                return false
                                            } else {
                                                return true;
                                            }
                                        },
                                        Click: function (e) {
                                            $('#txtCustomerSearch').val('');
                                            var vData = SalesPayment_offline().getOrganizationInfo(e.ID).OrganizationData.Rows;

                                            if (vData.length > 0) {
                                                //displayCustomerNameList(vData.OrganizationData.Rows);
                                                discount.isNewCustomer = false;
                                                discount.arrCustomer.push([
                                                    vData[0].ID, //0 ID
                                                    vData[0].PersonTitleID, //1 TITLE ??
                                                    '', //2 TITLE NAME ??
                                                    vData[0].PersonFullname, //3
                                                    vData[0].PersonSurname,//4
                                                    vData[0].PersonFirstName,//5
                                                    vData[0].PersonMiddleName,//6
                                                    vData[0].PersonSuffixName,//7
                                                    (vData[0].PersonDateOfBirth != '1/1/1900' ? new Date(vData[0].PersonDateOfBirth).getMonth() : ''),//8
                                                    (vData[0].PersonDateOfBirth != '1/1/1900' ? new Date(vData[0].PersonDateOfBirth).getDate() : ''),//9
                                                    (vData[0].PersonDateOfBirth != '1/1/1900' ? new Date(vData[0].PersonDateOfBirth).getFullYear() : ''),//10
                                                    'Edit' // 11 only 
                                                ])
                                                SalesPayment_offline().saveSeniorCitizenPWDCustomer(vData[0].ID, sessionStorage.CartOrderID);
                                                discount.LoadSalesPayCustomerNameList();
                                            } else {
                                                WebToast('No data found.', 'top');
                                            }
                                        },
                                        Close: function () {
                                            $('#popFMCGDiscount').popup('open');
                                        },
                                        Offline: true
                                    }
                            ], {FilterTypePerson: 1, FilterCustomers: 1})
                        }
                    })

                    this.btnDiscountCustomerBirthday.unbind('click').click(function (e) {
                        e.preventDefault();
                        SingleCalendar({
                            Click: function (e) {
                                Popup.Discount.btnDiscountCustomerBirthday.text(e);
                            },
                            Close: function () {
                                $('#popFMCGDiscount').popup('open');
                                Popup.Discount.txtFullname.focus();
                                Popup.popupname = 'discount';
                            }
                        })
                    })

                    this.selIDType.unbind('change').change(function (e) {
                        e.preventDefault();
                        if($(this).val() == "Other"){
                            $('#divOtherTypeID').show();
                        } else {
                            $('#divOtherTypeID').hide();
                        }
                    })

                    $('#btnAddDiscountCustomer').unbind('click').click(function (e) {
                        e.preventDefault();
                        Popup.Discount.Add();
                    })

                    $('#btnBackDiscountCustomer').unbind('click').click(function () {
                        $('#lblTitleDiscount').text('Discount');
                        Popup.Discount.Main();
                    })

                    $('#btnAddDiscountCustomerID').unbind('click').click(function (e) {
                        e.preventDefault();
                        $('#lblTitleDiscount').text('Valid Customer ID');
                        $('#pageDiscountCustomerID').pageshow();
                        Popup.Discount.selIDType.val('').focus();
                        Popup.Discount.txtOtherIDType.val('');
                        Popup.Discount.txtIDText.val('');
                        Popup.Discount.isNewCustomerID = true;
                        PopupReposition();
                    })

                    $('#btnCancelDiscountCustomerID').unbind('click').click(function () {
                        $('#lblTitleDiscount').text('Customer');
                        $('#pageDiscountCustomer').pageshow();
                        Popup.Discount.txtFullname.focus();
                        Popup.Discount.isNewCustomerID = false;
                        PopupReposition();
                    })

                    this.selDiscountType.keydown(function (e) {
                        if (e.which == 13) {
                            Popup.Discount.GetSelectedDiscount($.trim($(this).val()));
                            Popup.Discount.GetDiscountOnChange();
                            PopupReposition(); // NewMessageBox.js
                        }
                    })
                    .change(function (e) {
                        discount.GetSelectedDiscount($.trim($(this).val()));
                        discount.GetDiscountOnChange();
                        PopupReposition(); // NewMessageBox.js
                    })

                    this.form.unbind('submit').submit(function (e) {
                        e.preventDefault();
                        if (Popup.Discount.selDiscountType.val() == '34a4032f-a142-4c15-a868-4ac4410434df'
                            || Popup.Discount.selDiscountType.val() == '055a0a33-dd98-47ec-add3-8b2009f3944a'
                            || Popup.Discount.selDiscountType.val() == '674ad791-188b-4238-b95c-97b5c2b9d544') {
                            if (CDec2(discount.txtSalesPayNoOfSC.val()) != discount.arrCustomer.length) {
                                WebToast('Please add at least ' + CDec2(discount.txtSalesPayNoOfSC.val()) + ' customer (sr. citizen/pwd)', 'top')
                                return;
                            }

                            if (discount.arrCustomer.length == 0) {
                                WebToast('Please add at least one customer (sr. citizen/pwd)', 'top')
                                return;
                            }
                        }

                        params.success({
                            DiscountType: Popup.Discount.selDiscountType.val(),
                            Percent: CDec2(Popup.Discount.txtDiscountPercent.val()),
                            Amount: CDec2(Popup.Discount.txtDiscountAmount.val()),
                            Remarks: Popup.Discount.txtDiscountRemarks.val()
                        })
                        discount.popup.popup('close');
                    })

                    this.formCustomerDetails.unbind('submit').submit(function (e) {
                        e.preventDefault();
                        var fullname = $.trim(Popup.Discount.txtFullname.val());
                        var firstname = $.trim(Popup.Discount.txtFirstName.val());
                        var middlename = $.trim(Popup.Discount.txtMiddleName.val());
                        var surnname = $.trim(Popup.Discount.txtSurname.val());

                        if (middlename == "" && firstname == "" && surnname == "") {
                            if (fullname == "" || fullname.length <= 1) {
                                WebToast('Please enter fullname.', 'top');
                                return;
                            }
                        }

                        if (fullname == "" || fullname.length <= 1) {
                            if (surnname == "" || surnname.length <= 1) {
                                WebToast('Please enter surname.', 'top');
                                return;
                            }

                            if (firstname == "" || firstname.length <= 1) {
                                WebToast('Please enter first name.', 'top');
                                return;
                            }

                            if (middlename == "" || middlename.length <= 1) {
                                WebToast('Please enter middle name.', 'top');
                                return;
                            }
                        }

                        discount.SaveSalesPayCustomerName();
                    })

                    this.formFMCGDiscountCustomerID.unbind('submit').submit(function (e) {
                        e.preventDefault();

                        if(discount.arrCustomerIDs.length > 0){
                            //check first if there's existing id type
                            for (var i = 1; i < discount.arrCustomerIDs.length; i++){
                                if (discount.arrCustomerIDs[2] == discount.selIDType.val()) {
                                    //then override the existing record
                                    discount.isNewCustomerID = false;
                                    discount.indexCustomerID = discount.arrCustomerIDs[2];
                                }
                            }
                        }
                        
                        var newData = [
                            discount.selIDType.val(),
                            discount.txtOtherIDType.val(),
                            discount.txtIDText.val(),
                            (discount.isNewCustomerID ? 'New' : 'Edit'),
                            (discount.isNewCustomerID ? discount.ctrIndexCustomerID : discount.indexCustomerID)
                        ]

                        if (discount.isNewCustomerID) {
                            discount.arrCustomerIDs.push(newData)
                            discount.ctrIndexCustomerID += 1;
                        } else {
                            discount.arrCustomerIDs[discount.indexCustomerID] = newData;
                        }
                        
                        discount.DisplayCustomerID();

                        $('#btnCancelDiscountCustomerID').click() // go back to discount customer details
                    })

                    this.txtDiscountPercent.keydown(function (e) {
                        if (e.which == 107) {
                            // press + in numpad
                        }
                    })

                    this.txtSalesPayNoOfDiners.blur(function (e) {
                        discount.GetOrgDiscountPercentRefresh(discount.selDiscountType.val());
                        discount.RefreshTotalDiscount();
                    })

                    this.txtSalesPayNoOfSC.blur(function (e) {
                        if (CDec2($(this).val()) > CDec2(discount.txtSalesPayNoOfDiners.val())) {
                            discount.txtSalesPayNoOfDiners.val($(this).val())
                        }
                        discount.GetOrgDiscountPercentRefresh(discount.selDiscountType.val());
                        discount.RefreshTotalDiscount();
                    })
                },
                initElements: function(){
                    this.form = $('#formFMCGDiscounts');
                    this.selDiscountType = this.form.find('[name="selDiscountType"]');
                    this.txtDiscountPercent = this.form.find('[name="txtDiscountPercent"]');
                    this.txtDiscountAmount = this.form.find('[name="txtDiscountAmount"]');
                    this.txtDiscountRemarks = this.form.find('[name="txtDiscountRemarks"]');
                    this.txtSalesPayNoOfSC = this.form.find('[name="txtSalesPayNoOfSC"]');
                    this.txtSalesPayNoOfDiners = this.form.find('[name="txtSalesPayNoOfDiners"]');
                    this.txtDiscountPercentCeiling = this.form.find('[name="txtDiscountPercentCeiling"]');

                    this.formCustomerDetails = $('#formFMCGDiscountCustomer');
                    this.txtFullname = this.formCustomerDetails.find('[name="txtFullname"]');
                    this.txtSurname = this.formCustomerDetails.find('[name="txtSurname"]');
                    this.txtFirstName = this.formCustomerDetails.find('[name="txtFirstName"]');
                    this.txtMiddleName = this.formCustomerDetails.find('[name="txtMiddleName"]');
                    this.selPersonalSuffixName = this.formCustomerDetails.find('[name="selPersonalSuffixName"]');
                    this.btnDiscountCustomerBirthday = this.formCustomerDetails.find('[name="btnDiscountCustomerBirthday"]');

                    this.formFMCGDiscountCustomerID = $('#formFMCGDiscountCustomerID');
                    this.txtOtherIDType = this.formFMCGDiscountCustomerID.find('[name="txtOtherIDType"]');
                    this.txtIDText = this.formFMCGDiscountCustomerID.find('[name="txtIDText"]');
                    this.selIDType = this.formFMCGDiscountCustomerID.find('[name="selIDType"]');
                },
                Add: function () {
                    $('#lblTitleDiscount').text('Customer');                    
                    $('#pageDiscountCustomer').pageshow();
                    $('#txtCustomerPersonID').val('');
                    $('#listDiscountCustomerID').empty();
                    this.txtFullname.val('');
                    this.txtFirstName.val('');
                    this.txtMiddleName.val('');
                    this.txtSurname.val('');
                    this.selPersonalSuffixName.val('');
                    this.btnDiscountCustomerBirthday.text('Set Date');
                    this.arrCustomerIDs = [];
                    this.indexCustomerID = 0;
                    this.ctrIndexCustomerID = 0;
                    this.isNewCustomer = true;
                    PopupReposition(); //NewMessageBox.js
                    Popup.Discount.txtFullname.focus();
                    //ResizePopupBackground();
                },
                Main: function () {
                    $('#pageDiscountInfo').pageshow();
                    Popup.Discount.selDiscountType.focus();
                    PopupReposition(); //NewMessageBox.js
                    //ResizePopupBackground();
                },
                SaveSalesPayCustomerName: function(){
                    var ids = Popup.Discount.arrCustomerIDs;
                    var seniorCitizensIDNo;
                    var pwdIDNo;
                    var passportNo;
                    var driversLicenseNo;
                    var sssNo;
                    var gsisIDNo;
                    var prcIDNo;
                    var postalIDNo;
                    var votersIDNo;
                    var otherIDNo;
                    var otherIDType;

                    if(ids.length == 0){
                        WebToast('Please add at least one valid identification number.', 'top');
                        return;
                    }

                    for (var i = 0; i < ids.length; i++){
                        switch (ids[0]) {
                            case "Senior Citizens' ID":
                                seniorCitizensIDNo = ids[2];
                                break;
                            case "Persons With Disabilities (PWD) ID":
                                pwdIDNo = ids[2];
                                break;
                            case "Passport":
                                passportNo = ids[2];
                                break;
                            case "Driver's License":
                                driversLicenseNo = ids[2];
                                break;
                            case "SSS ID":
                                sssNo = ids[2];
                                break;
                            case "GSIS ID":
                                gsisIDNo = ids[2];
                                break;
                            case "PRC ID":
                                prcIDNo = ids[2];
                                break;
                            case "Postal ID":
                                postalIDNo = ids[2];
                                break;
                            case "Voter's ID":
                                votersIDNo = ids[2];
                                break;
                            case "Other":
                                otherIDType = ids[1];
                                otherIDNo = ids[2];
                                break;
                        }
                    }

                    var pDataTradeName = [];
                    var pDataCategory = [];
                    var txtFullname = $.trim(this.txtFullname.val());
                    var txtFirstName = $.trim(this.txtFirstName.val());
                    var txtMiddleName = $.trim(this.txtMiddleName.val());
                    var txtSurname = $.trim(this.txtSurname.val());
                    var selPersonalSuffixName = $.trim(this.selPersonalSuffixName.val());
                    var bday = $.trim(this.btnDiscountCustomerBirthday.text())

                    var pDataPerson = {
                        IsOrganization: 0,
                        OrganizationID: $("#txtCustomerPersonID").val(),
                        PersonFullName: txtFullname,
                        PersonFirstName: txtFirstName,
                        PersonMiddleName: txtMiddleName,
                        PersonSurname: txtSurname,
                        PersonSuffixName: selPersonalSuffixName,
                        PersonTitleID: $.trim(''),
                        PersonDOB: $.trim(bday),
                        PersonSeniorCitizensIDNo: seniorCitizensIDNo,
                        PersonPWDIDNo: pwdIDNo,
                        PersonPassportNo: passportNo,
                        PersonDriversLicenseNo: driversLicenseNo,
                        PersonSSSNo: sssNo,
                        PersonGSISIDNo: gsisIDNo,
                        PersonPRCIDNo: prcIDNo,
                        PersonPostalIDNo: postalIDNo,
                        PersonVotersIDNo: votersIDNo,
                        PersonOtherIDType: otherIDType,
                        PersonOtherIDNo: otherIDNo,
                        PersonMotherMaidenSurname: '',
                        PersonMotherMaidenFirstName: '',
                        PersonMotherMaidenMiddleName: '',
                        OrgTradeNameList: pDataTradeName,
                        OrgCategoryList: pDataCategory,
                        IsAddAsCustomer: 1, //(IsNewSalesPayCustomerName ? 1 : 0),
                        CompanyNameInitial: '',
                        IsFranchisor: 0,
                        TIN: '',
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

                        var result = SalesPayment_offline().SaveOrganizationInfo(pDataPerson);
                        if (result != 'Existing') {

                            //$("#txtCustomerPersonID").val(result);

                            this.arrCustomer.push([
                                result, //0 ID
                                '', //1 TITLE ??
                                '', //2 TITLE NAME ??
                                txtFullname, //3
                                txtSurname,//4
                                txtFirstName,//5
                                txtMiddleName,//6
                                selPersonalSuffixName,//7
                                (IsDate(bday) ? new Date(bday).getMonth() : ''),//8
                                (IsDate(bday) ? new Date(bday).getDate() : ''),//9
                                (IsDate(bday) ? new Date(bday).getFullYear() : ''),//10
                                (CBool(this.isNewCustomer) ? 'New' : 'Edit') // 11 temp only 
                            ])
                            SalesPayment_offline().saveSeniorCitizenPWDCustomer(result, sessionStorage.CartOrderID);
                            this.LoadSalesPayCustomerNameList();
                            $('#btnBackDiscountCustomer').click();
                        } else {
                            WebToast("Person with the same information already exists.", "top");
                        }
                    }
                },
                LoadSalesPayCustomerNameList: function(){
                    var listDiscountCustomer = $('#listDiscountCustomer');
                    var customer = this.arrCustomer;

                    listDiscountCustomer.empty();

                    if (customer.length > 0) {
                        for (var i = 0; i < customer.length; i++) {
                            var data = customer[i];

                            listDiscountCustomer.append($('<li>')
                                    .attr('id', data[0])
                                    .attr('data-icon', 'delete')
                                    .append($('<a>')
                                        .data('data', data)
                                        .append((CStr2(data[3]) == '' ? data[4] + ', ' + data[5] + ' ' + data[6] : data[3]))
                                        .click(function () {
                                            Popup.Discount.Add();
                                            var data = $(this).data('data');
                                            var bday = new Date(data[8] + '-' + data[9] + '-' + data[10]);

                                            $('#txtCustomerPersonID').val(data[0]);
                                            Popup.Discount.txtFullname.val(data[3]);
                                            Popup.Discount.txtSurname.val(data[4]);
                                            Popup.Discount.txtFirstName.val(data[5]);
                                            Popup.Discount.txtMiddleName.val(data[6]);
                                            Popup.Discount.selPersonalSuffixName.val(data[7]);
                                            Popup.Discount.btnDiscountCustomerBirthday.text((IsDate(bday) ? $.format.date(new Date(bday, 'MMM dd, yyyy')) : 'Set Date'));

                                            var vRows = SalesPayment_offline().getOrganizationInfo(data[0]).OrganizationData.Rows;

                                            if (CStr2(vRows[0].PersonSeniorCitizensIDNo) != '') {
                                                Popup.Discount.arrCustomerIDs.push([
                                                    'Senior Citizens\' ID', '', vRows[0].PersonSeniorCitizensIDNo, 'Edit'
                                                ])
                                                Popup.Discount.ctrIndexCustomerID += 1;
                                            }

                                            if (CStr2(vRows[0].PersonPWDIDNo) != '') {
                                                Popup.Discount.arrCustomerIDs.push([
                                                    'Persons With Disabilities (PWD) ID', '', vRows[0].PersonPWDIDNo, 'Edit'
                                                ])
                                                Popup.Discount.ctrIndexCustomerID += 1;
                                            }

                                            if (CStr2(vRows[0].PersonPassportNo) != '') {
                                                Popup.Discount.arrCustomerIDs.push([
                                                    'Passport', '', vRows[0].PersonPassportNo, 'Edit'
                                                ])
                                                Popup.Discount.ctrIndexCustomerID += 1;
                                            }

                                            if (CStr2(vRows[0].PersonDriversLicenseNo) != '') {
                                                Popup.Discount.arrCustomerIDs.push([
                                                    'Driver\'s License', '', vRows[0].PersonDriversLicenseNo, 'Edit'
                                                ])
                                                Popup.Discount.ctrIndexCustomerID += 1;
                                            }

                                            if (CStr2(vRows[0].PersonSSSNo) != '') {
                                                Popup.Discount.arrCustomerIDs.push([
                                                    'SSS ID', '', vRows[0].PersonSSSNo, 'Edit'
                                                ])
                                                Popup.Discount.ctrIndexCustomerID += 1;
                                            }

                                            if (CStr2(vRows[0].PersonGSISIDNo) != '') {
                                                Popup.Discount.arrCustomerIDs.push([
                                                    'GSIS ID', '', vRows[0].PersonGSISIDNo, 'Edit'
                                                ])
                                                Popup.Discount.ctrIndexCustomerID += 1;
                                            }

                                            if (CStr2(vRows[0].PersonPRCIDNo) != '') {
                                                Popup.Discount.arrCustomerIDs.push([
                                                    'PRC ID', '', vRows[0].PersonPRCIDNo, 'Edit'
                                                ])
                                                Popup.Discount.ctrIndexCustomerID += 1;
                                            }

                                            if (CStr2(vRows[0].PersonPostalIDNo) != '') {
                                                Popup.Discount.arrCustomerIDs.push([
                                                    'Postal ID', '', vRows[0].PersonPostalIDNo, 'Edit'
                                                ])
                                                Popup.Discount.ctrIndexCustomerID += 1;
                                            }

                                            if (CStr2(vRows[0].PersonVotersIDNo) != '') {
                                                Popup.Discount.arrCustomerIDs.push([
                                                    'Voter\'s ID', '', vRows[0].PersonVotersIDNo, 'Edit'
                                                ])
                                                Popup.Discount.ctrIndexCustomerID += 1;
                                            }

                                            if (CStr2(vRows[0].PersonOtherIDType) != '') {
                                                Popup.Discount.arrCustomerIDs.push([
                                                    'Other', vRows[0].PersonOtherIDType, vRows[0].PersonOtherIDNo, 'Edit'
                                                ])
                                                Popup.Discount.ctrIndexCustomerID += 1;
                                            }
                                            Popup.Discount.DisplayCustomerID();
                                        })
                                    )
                                    .append($('<a>')
                                        .data('data', data)
                                        .click(function () {
                                            var data = $(this).data('data');
                                            NewMessageBox('Are you sure you want to remove this record?', 'Message', [
                                                    {
                                                        Text: 'Yes',
                                                        Click: function () {
                                                            Popup.Discount.arrCustomer = $.grep(Popup.Discount.arrCustomer, function (e) {
                                                                return e[0] != data[0]
                                                            });
                                                            Popup.Discount.LoadSalesPayCustomerNameList();
                                                            SalesPayment_offline().removeSeniorCitizenPWDCustomer((data[12] != undefined ? data[12] : data[0])) // data[12] ID from TransInvoiceSeniorCitizenPWDCustomer
                                                        }
                                                    }, { Text: 'No' }
                                            ], {
                                                Close: function () {
                                                    $('#popFMCGDiscount').popup('open');
                                                }
                                            })
                                        })
                                    )
                                )
                        }
                        listDiscountCustomer.listview('refresh');
                    } else {
                        listDiscountCustomer.append('<li>List is empty</li>');
                    }
                    
                    PopupReposition();
                    //ResizePopupBackground();
                },
                DisplayCustomerID: function(){
                    var listDiscountCustomerID = $('#listDiscountCustomerID');
                    var vIDs = Popup.Discount.arrCustomerIDs;

                    listDiscountCustomerID.empty();

                    for (var i = 0; i < vIDs.length; i++) {
                        var vData = vIDs[i];
                        if(vData[3] != 'Remove'){
                            listDiscountCustomerID.append($('<li>')
                                .attr('data-icon', 'delete')
                                .append($('<a>')
                                    .data('data', vData)
                                    .html((vData[0] == 'Other' ? vData[1] : vData[0]) + '<br>' + vData[2])
                                    .click(function () {
                                        $('#btnAddDiscountCustomerID').click();

                                        Popup.Discount.selIDType.val($(this).data('data')[0]).focus();
                                        Popup.Discount.txtOtherIDType.val($(this).data('data')[1]);
                                        Popup.Discount.txtIDText.val($(this).data('data')[2]);
                                        Popup.Discount.isNewCustomerID = false;
                                        Popup.Discount.indexCustomerID = $(this).data('data')[4];
                                    })
                                )
                                .append($('<a>')
                                    .data('data', vData)
                                    .click(function () {
                                        var data = $(this).data('data');
                                        NewMessageBox('Are you sure you want to remove this record?', 'Message',[
                                                {
                                                    Text: 'Yes',
                                                    Click: function () {
                                                        Popup.Discount.arrCustomerIDs[data[4]][3] = 'Remove';
                                                        Popup.Discount.DisplayCustomerID();

                                                        if ($.trim($('#txtCustomerPersonID').val()) != '') {
                                                            SalesPayment_offline().removeSeniorCitizenPWDCustomerIdentification($.trim($('#txtCustomerPersonID').val()), data[0])
                                                        }
                                                    }
                                                }, { Text: 'No' }
                                        ], {
                                            Close: function () {
                                                $('#popFMCGDiscount').popup('open');
                                            }
                                        })
                                    })
                                )
                            )
                        }
                    }

                    listDiscountCustomerID.listview('refresh');
                    PopupReposition();
                    //ResizePopupBackground();
                },
                ShowSalesPaymentsInfo: function () {
                    this.LoadSalesPaymentsInfo(SalesPayment_offline().ShowSalesPaymentsInfo(sessionStorage.CartOrderID).Rows[0])
                },
                LoadSalesPaymentsInfo: function (vData) {
                    if (vData != undefined) {
                        this.dblTotalAmountDueForDiscount = 0;
                        this.dblTotalAmountDueForDiscount = vData.TotalAmountDue || 0;
                        this.dblSubTotal = 0;
                        this.dblSubTotal = vData.TransInvoiceItems || 0;
                        this.taxPercent = vData.TaxPercent || 0;

                        this.RefreshSalesPaymentTotal();
                        this.RefreshTotalDiscount();
                    }
                },
                GetSelectedDiscount: function (discount) {
                    $('#divDiscountCustomerDetails').hide();
                    $('#divDiscountCustomerList').hide();

                    this.txtDiscountPercent.removeAttr('disabled');
                    this.txtDiscountAmount.removeAttr('disabled');

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
                        $('#divDiscountCustomerList').show();
                        this.showDiscountDetails = true;
                        this.isSeniorCitizenPWDDiscount = true;
                        this.isSeniorCitizenDiscount = true;
                        this.txtDiscountPercent.attr("disabled", "disabled");
                        this.txtDiscountAmount.attr("disabled", "disabled");

                        this.txtSalesPayNoOfSC.val((IsGuid(discount) ? 1 : (discount.Rows[0].NoOfSeniorCitizens || 1)));
                        this.txtSalesPayNoOfDiners.val((IsGuid(discount) ? 1 : (discount.Rows[0].NoOfPeople || 1)));
                    } else if (this.selDiscountType.val().toUpperCase() == '34A4032F-A142-4C15-A868-4AC4410434DF') {
                        this.strAccessType = "Allow to grant Person With Disability (PWD) Discount";
                        //----- SAME LANG DAW ANG DISCOUNT SA Sr.Citizend & PWD - modified by arnie may5,17 10:58am
                        //$('#divDiscountCustomerList').show();
                        //this.isSeniorCitizenPWDDiscount = true;
                        //this.showDiscountDetails = false;
                        //this.isSeniorCitizenDiscount = false;
                        //this.txtDiscountPercent.attr("disabled", "disabled");
                        //this.txtDiscountAmount.attr("disabled", "disabled");
                        $('#divDiscountCustomerDetails').show();
                        $('#divDiscountCustomerList').show();
                        this.showDiscountDetails = true;
                        this.isSeniorCitizenPWDDiscount = true;
                        this.isSeniorCitizenDiscount = true;
                        this.txtDiscountPercent.attr("disabled", "disabled");
                        this.txtDiscountAmount.attr("disabled", "disabled");

                        this.txtSalesPayNoOfSC.val((IsGuid(discount) ? 1 : (discount.Rows[0].NoOfSeniorCitizens || 1)));
                        this.txtSalesPayNoOfDiners.val((IsGuid(discount) ? 1 : (discount.Rows[0].NoOfPeople || 1)));
                    } else if (this.selDiscountType.val().toUpperCase() == '674AD791-188B-4238-B95C-97B5C2B9D544') {
                        this.strAccessType = "Allow to grant Senior Citizens' (SC) Discount";
                        $('#divDiscountCustomerDetails').hide();
                        $('#divDiscountCustomerList').show();
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
                    if (this.strAccessType == "") {
                        $('#selDiscountType').val('').selectmenu('refresh');
                        $("#txtDiscountPercent").val("0.00");
                        $("#txtDiscountAmount").val("0.00");

                        this.GetOrgDiscountPercentRefresh(this.selDiscountType.val());
                        this.RefreshTotalDiscount();

                    } else {
                        window.pageSecuritySourcePage = $.mobile.activePage;
                        PromptUserAccess(this.strAccessType, {
                            OnClickOKSuccess: function () {

                                //$.mobile.changePage(window.pageSecuritySourcePage);

                                Popup.Discount.GetOrgDiscountPercentRefresh(Popup.Discount.selDiscountType.val());

                                //                                proj.RefreshTotalDiscount();

                            },
                            OnFailed: function () {
                                //$.mobile.changePage(window.pageSecuritySourcePage);
                                $('#selDiscountType').val('').selectmenu('refresh');
                                $("#txtDiscountPercent").val("0.00");
                                $("#txtDiscountAmount").val("0.00");
                                Popup.Discount.GetOrgDiscountPercentRefresh(Popup.Discount.selDiscountType.val());

                                //proj.RefreshTotalDiscount();
                            }
                        });
                    }

                    this.RefreshSalesPaymentTotalBilling();
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

                                this.GetSelectedDiscount(discount);

                                if (this.isSeniorCitizenPWDDiscount) {
                                    var vData = SalesPayment_offline().getSeniorCitizenPWDCustomer(sessionStorage.CartOrderID);
                                    //put the ids in sessionstorage

                                    if (vData.Rows.length > 0) {

                                        arSalesPayCustomerName = new Array();
                                        for (var i = 0; i < vData.Rows.length; i++) {
                                            this.arrCustomer.push([
                                                    vData.Rows[0].CustomerID, //0 ID
                                                    vData.Rows[0].PersonTitleID, //1 TITLE ??
                                                    '', //2 TITLE NAME ??
                                                    vData.Rows[0].Fullname, //3
                                                    vData.Rows[0].Surname,//4
                                                    vData.Rows[0].FirstName,//5
                                                    vData.Rows[0].MiddleName,//6
                                                    vData.Rows[0].SuffixName,//7
                                                    (vData.Rows[0].Birthdate != '1/1/1900' ? new Date(vData.Rows[0].Birthdate).getMonth() : ''),//8
                                                    (vData.Rows[0].Birthdate != '1/1/1900' ? new Date(vData.Rows[0].Birthdate).getDate() : ''),//9
                                                    (vData.Rows[0].Birthdate != '1/1/1900' ? new Date(vData.Rows[0].Birthdate).getFullYear() : ''),//10
                                                    'Edit', // 11 only 
                                                    vData.Rows[0].ID // 12
                                            ])
                                        }

                                        this.LoadSalesPayCustomerNameList();
                                    }
                                }

                                this.longDiscountPercent = discount.Rows[0].DiscountPercentage;
                            }

                        }
                    }
                },
                GetOrgDiscountPercentRefresh: function (strDiscountID) {
                    if (OfflineMode) {
                        var vRows = SalesPayment_offline().getOrgDiscountPercentRefresh(strDiscountID).Rows[0];

                        if (vRows != undefined) {
                            this.LoadOrgDiscountPercentRefresh(vRows);
                        } else {
                            this.txtDiscountPercentCeiling.val('');
                            this.txtDiscountPercent.val('');
                            this.txtDiscountAmount.val('');
                            $("#txtLessDiscount").text('0.00');
                            this.longDiscountPercent = 0;
                            this.RefreshSalesPaymentTotal();
                        }
                    }
                },
                LoadOrgDiscountPercentRefresh: function (vRows) {
                    this.txtDiscountPercentCeiling.val((vRows.PercentDiscount == "0" ? '' : AccountingFormat(vRows.PercentDiscount)))
                    this.txtDiscountPercent.val((vRows.PercentDiscount == "0" ? '' : AccountingFormat(vRows.PercentDiscount)))
                    this.longDiscountPercent = vRows.PercentDiscount;

                    this.RefreshTotalDiscount();
                },
                RefreshTotalDiscount: function () {
                    var dblDiscount = 0;
                    var dblTotalAmount = 0;

                    if (this.selDiscountType.val() == '') {

                        $('#rowVATPercent').hide();
                        $('#rowAmountLessVAT').hide();

                        $('#lblVATAmount').text('');
                        $('#lblTotalAmount').text('');
                        $('#txtLessDiscount').text('0.00');
                        this.txtDiscountPercentCeiling.val('');
                        this.txtDiscountPercent.val('');
                        this.txtDiscountAmount.val('');
                        this.totaldiscount = 0;

                        var pIU;
                        var pParams = GetUrlParameters();
                        var pInvoiceID = CStr2(pParams.InvoiceID) == '' ? sessionStorage.CartOrderID : pParams.InvoiceID;

                        pIU = new InsertUpdate('private.TransInvoice');

                        pIU.Add('Vatable', this.dblSubTotal / (1 + (CDec2(this.taxPercent) / 100)));
                        pIU.Add('VATExempt', 0);
                        pIU.Add('VAT', this.dblSubTotal - (this.dblSubTotal / (1 + (CDec2(this.taxPercent) / 100))));
                        pIU.Add('ForegoneVAT', 0);
                        pIU.Add('SCDiscount', 0);
                        pIU.Add('TotalNonSCDiscount', 0);
                        pIU.Add('AmountDueBeforeCharges', this.dblSubTotal);
                        pIU.Add('AmountDueNetofCharges', this.dblSubTotal + CDec2(this.GetInvoiceCharges(pInvoiceID)));

                        pSql = pIU.GetUpdate("ID = " + ReadyString(pInvoiceID));

                        Sql.ExecuteSql(pSql);

                    } else {
                        if (FmcgBasicCart.isFromRecallCart == false) {
                            this.taxPercent = CDec2(SalesPayment_offline().getTaxPercent(sessionStorage.CartOrderID).Rows[0].TaxPercent);
                        }
                        this.SubTotalOfVatExemptItems = this.GetSSDItemsWithVatExempt(sessionStorage.CartOrderID);
                        this.SCDiscountExempt = CDec2(AccountingFormat(this.dblSubTotal)) - CDec2(this.SubTotalOfVatExemptItems);
                        dblTotalAmount = this.isSeniorCitizenDiscount ? CDec2(this.SubTotalOfVatExemptItems) / (1 + (CDec2(this.taxPercent) / 100)) : CDec2(this.dblSubTotal);

                        //dblDiscount = parseFloat(CDec2($('#lblSalesPaySubTotal').text().replace(/,/g, ""))) * (parseFloat(CDec2($('#txtDiscountPercent').val().replace(/,/g, "")))/100);
                        //dblDiscount = Math.round(parseFloat(CDec2($('#lblSalesPaySubTotal').text().replace(/,/g, ""))) * (parseFloat(CDec2($('#txtDiscountPercent').val().replace(/,/g, ""))) / 100) / 0.05) * 0.05;


                        if (this.isSeniorCitizenDiscount) {

                            $('#rowVATPercent').show();
                            $('#rowAmountLessVAT').show();

                            $('#lblVATPercentValue').text(this.taxPercent);

                            //                        vatSale = vData.Rows[0].TotalAmountDue / (1 + (vData.Rows[0].TaxPercent / 100));
                            $('#lblVATPercent').text(AccountingFormat(CDec2(this.SubTotalOfVatExemptItems) - CDec2(dblTotalAmount)));
                            $('#lblTotalAmount').text(AccountingFormat(dblTotalAmount));

                            if (CDec2($.trim(this.txtSalesPayNoOfSC.val())) > 0 && CDec2($.trim(this.txtSalesPayNoOfDiners.val())) > 0) {

                                var proportionOfNoOfSeniors = parseFloat($.trim(this.txtSalesPayNoOfSC.val())) / parseFloat($.trim(this.txtSalesPayNoOfDiners.val()));
                                //var GrossSales = this.dblSubTotal;
                                this.GrossSalesToSC = proportionOfNoOfSeniors * this.SubTotalOfVatExemptItems;


                                var proportionofregular = (parseFloat($.trim(this.txtSalesPayNoOfDiners.val())) - parseFloat($.trim(this.txtSalesPayNoOfSC.val()))) / parseFloat($.trim(this.txtSalesPayNoOfDiners.val()));
                                this.GrossSalesToNoneSC = proportionofregular * this.SubTotalOfVatExemptItems;

                                this.VatExemptSales = this.GrossSalesToSC / (1 + (CDec2(this.taxPercent) / 100));
                                var seniorCitizenDiscountRate = parseFloat(this.txtDiscountPercentCeiling.val()) * 0.01;

                                this.MaxSCDiscountableAmount = this.GetMaxSCDiscountableAmount();
                                this.MaxSCDiscountableAmount = this.MaxSCDiscountableAmount * CInt2($.trim(this.txtSalesPayNoOfSC.val()));

                                if (this.MaxSCDiscountableAmount != 0) {
                                    if (this.MaxSCDiscountableAmount < this.VatExemptSales) {
                                        this.IsApplyMaxSCDiscountableAmount = true;
                                    }
                                } else {

                                }

                                this.seniorCitizenDiscount = (this.IsApplyMaxSCDiscountableAmount == true ? this.MaxSCDiscountableAmount : this.VatExemptSales) * seniorCitizenDiscountRate;
                                                                
                                //var seniorCitizenDiscountBasedOnVatEx = AccountingFormat(VatExToSC * seniorCitizenDiscountRate);

                                //VatExToSC should always be the Gross VAT-Ex Amount(variable VatExToSC) even if MaxSCDiscountableAmount is present
                                this.seniorCitizenNetAmount = CDec2(AccountingFormat(this.VatExemptSales)) - CDec2(AccountingFormat(this.seniorCitizenDiscount));

                                //var totalBillToBePayed = this.seniorCitizenNetAmount + GrossSalesToNoneSC;

                                var weightedSCDiscountRate = proportionOfNoOfSeniors * seniorCitizenDiscountRate;

                                //var totalBillBeforeSCDiscount = parseFloat($('#lblSalesPayAmountDue').val());

                                //var totalBillToBePayed = weightedSCDiscountRate * totalBillBeforeSCDiscount;

                                var pIU;
                                var AmountDueBeforeCharges = 0;
                                var AmountDueNetofCharges = 0;

                                AmountDueBeforeCharges = CDec2(this.GrossSalesToNoneSC + this.VatExemptSales + this.SCDiscountExempt);
                                AmountDueNetofCharges = CDec2(AmountDueBeforeCharges) + CDec2(this.GetInvoiceCharges(sessionStorage.CartOrderID));

                                pIU = new InsertUpdate('private.TransInvoice');

                                pIU.Add('Vatable', CDec2((this.GrossSalesToNoneSC / (1 + (CDec2(this.taxPercent) / 100))) + (this.SCDiscountExempt / (1 + (CDec2(this.taxPercent) / 100)))));
                                pIU.Add('VATExempt', CDec2(this.VatExemptSales));
                                pIU.Add('VAT', CDec2(((this.GrossSalesToNoneSC + this.SCDiscountExempt)) - ((this.GrossSalesToNoneSC / (1 + (CDec2(this.taxPercent) / 100))) + (this.SCDiscountExempt / (1 + (CDec2(this.taxPercent) / 100))))));
                                pIU.Add('ForegoneVAT', CDec2(this.GrossSalesToSC) - CDec2(this.VatExemptSales));
                                pIU.Add('SCDiscount', CDec2(this.seniorCitizenDiscount));
                                pIU.Add('TotalNonSCDiscount', 0);
                                pIU.Add('AmountDueBeforeCharges', AmountDueBeforeCharges);
                                pIU.Add('AmountDueNetofCharges', AmountDueNetofCharges);

                                pSql = pIU.GetUpdate("ID = " + ReadyString(sessionStorage.CartOrderID));

                                Sql.ExecuteSql(pSql);

                                $('#lblVATPercent').text(AccountingFormat(CDec2(this.GrossSalesToSC) - CDec2(this.VatExemptSales)));
                                $('#lblTotalAmount').text(AccountingFormat(AmountDueNetofCharges)); //AccountingFormat(this.GrossSalesToNoneSC + this.VatExemptSales)
                                $('#txtLessDiscount').text(AccountingFormat(this.seniorCitizenDiscount)); //seniorCitizenDiscountBasedOnVatEx

                                this.txtDiscountPercent.val(CDec2(AccountingFormat(weightedSCDiscountRate * 100)));

                                this.txtDiscountAmount.val(CDec2(AccountingFormat(this.seniorCitizenDiscount))); //seniorCitizenDiscountBasedOnVatEx
                                this.totaldiscount = this.seniorCitizenDiscount; //seniorCitizenDiscountBasedOnVatEx;

                                this.longDiscountPercent = seniorCitizenDiscountRate * 100;

                                //proj.RefreshTotalDiscount();
                            }
                        } else {
                            var total = (CDec2(this.longDiscountPercent) == "0" ? '' : AccountingFormat(CDec2($('#txtSubTotal').text().replace(/,/g, "")) * (CDec2(this.longDiscountPercent)) / 100))
                            $('#rowVATPercent').hide();
                            $('#rowAmountLessVAT').hide();

                            $('#lblVATAmount').text('');
                            $('#lblTotalAmount').text('');

                            $('#txtLessDiscount').text(total);
                            this.totaldiscount = total;
                            this.txtDiscountAmount.val(total);

                            var pIU;
                            var pParams = GetUrlParameters();
                            var pInvoiceID = CStr2(pParams.InvoiceID) == '' ? sessionStorage.CartOrderID : pParams.InvoiceID;

                            pIU = new InsertUpdate('private.TransInvoice');

                            pIU.Add('Vatable', (this.dblSubTotal - (this.dblSubTotal * (CDec2(this.longDiscountPercent) / 100))) / (1 + (CDec2(this.taxPercent) / 100)));
                            pIU.Add('VATExempt', 0);
                            pIU.Add('VAT', (this.dblSubTotal - (this.dblSubTotal * (CDec2(this.longDiscountPercent) / 100))) - ((this.dblSubTotal - (this.dblSubTotal * (CDec2(this.longDiscountPercent) / 100))) / (1 + (CDec2(this.taxPercent) / 100))));
                            pIU.Add('ForegoneVAT', 0);
                            pIU.Add('SCDiscount', 0);
                            pIU.Add('TotalNonSCDiscount', 0);
                            pIU.Add('AmountDueBeforeCharges', (this.dblSubTotal - (this.dblSubTotal * (CDec2(this.longDiscountPercent) / 100))));
                            pIU.Add('AmountDueNetofCharges', (this.dblSubTotal - (this.dblSubTotal * (CDec2(this.longDiscountPercent) / 100))) + CDec2(this.GetInvoiceCharges(pInvoiceID)));

                            pSql = pIU.GetUpdate("ID = " + ReadyString(pInvoiceID));

                            Sql.ExecuteSql(pSql);

                        }
                    }
                    dblDiscount = this.txtDiscountAmount.val();  //(dblTotalAmount * (parseFloat(CDec2($('#txtDiscountPercent').val().replace(/,/g, ""))) / 100) / 0.05) * 0.05;
                    //this.RefreshSalesPaymentTotal_onchange();
                    Popup.ComputePayment();
                    //janine feb 24 2015 11am
                    if (OfflineMode) {
                        var vData = SalesPayment_offline().getSalesPaymentReceiptFooterAmountsInfo(sessionStorage.CartOrderID);
                        if (vData.Rows.length > 0) {
                            // for service charge. this is the basis not thew total amount due or subtotal. see sm coin

                            this.lblVATableSales = CDec2(vData.Rows[0].TaxSales || 0) + CDec2(vData.Rows[0].VATExemptSale || 0);

                        }
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
                        dblDiscount = AccountingFormat(vDataFooter.Rows[0].DiscountAmount);
                        dblCharges = CDec2(vDataFooter.Rows[0].ChargeAmount);
                        dblTip = CDec2(vDataFooter.Rows[0].TipAmount);
                        dblLocalTaxAmount = CDec2(vDataFooter.Rows[0].LocalTaxAmount);

                        $('#txtLessDiscount').text(dblDiscount);
                        this.txtDiscountAmount.val(dblDiscount);
                        this.totaldiscount = dblDiscount;
                    }

                    Popup.ComputePayment();
                },
                GetDiscountTypeList: function () {
                    var vRows = SalesPayment_offline().getOrgDiscountType().Rows;

                    this.selDiscountType.empty();

                    this.selDiscountType.append(new Option('Please Select', ''));
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        if (CDec2(vRows[p].PercentDiscount) != 0) {
                            this.selDiscountType.append(new Option(vRows[p].DiscountType, vRows[p].ID, false, false));
                        }
                    }

                    this.isDiscountTypeLoaded = true;
                },
                RefreshSalesPaymentTotal_onchange: function () {
                    var cashAmountTendered = parseFloat(CDec2(Popup.Cash.cashTendered));

                    var dblTotalSalesPay = 0;
                    var dblTotalAmountDue = 0;
                    var amtDue = this.isSeniorCitizenDiscount ? $('#lblTotalAmount').text().replace(/,/g, "") : $('#txtSubTotal').text().replace(/,/g, "");

                    if (this.isSeniorCitizenDiscount) {
                        dblTotalAmountDue = parseFloat(CDec2(amtDue)) - parseFloat(CDec2(Popup.Discount.totaldiscount)) + parseFloat(CDec2(Popup.Charges.totalcharge)) + parseFloat(CDec2(Popup.Charges.totalothertax)) + parseFloat(CDec2(0));
                    } else {
                        dblTotalAmountDue = parseFloat(CDec2(amtDue)) - parseFloat(CDec2(Popup.Discount.totaldiscount)) + parseFloat(CDec2(Popup.Charges.totalcharge)) + parseFloat(CDec2(Popup.Charges.totalothertax)) + parseFloat(CDec2(0));
                    }

                    dblTotalSalesPay = (parseFloat(CDec2(cashAmountTendered)) + parseFloat(CDec2(Popup.ChequeGC.totalcgc)) + parseFloat(CDec2(Popup.Card.totalcardamt)) + parseFloat(CDec2(Popup.Srv.totalsrv)) + parseFloat(CDec2(Popup.OtherPayment.totalop))) - (parseFloat(CDec2(amtDue)) - parseFloat(CDec2(Popup.Discount.totaldiscount)) + parseFloat(CDec2(Popup.Charges.totalcharge)) + parseFloat(CDec2(0)) + parseFloat(CDec2(Popup.Charges.totalothertax)));;
                    $('#txtTotal').text(AccountingFormat(dblTotalAmountDue))

                    var totalAmountTendered = parseFloat(CDec2(Popup.Cash.cashTendered)) + parseFloat(CDec2(Popup.ChequeGC.totalcgc)) + parseFloat(CDec2(Popup.Card.totalcardamt)) + parseFloat(CDec2(Popup.Srv.totalsrv)) + parseFloat(CDec2(Popup.OtherPayment.totalop));

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
                RefreshSalesPaymentTotalBilling: function () {
                    var pData = [];
                    var pDataDiscount = [];
                    var pDataSeniorCitizenPWD = [];

                    if (this.selDiscountType.val() != "") {
                        intIndex = 0;
                        pDataDiscount[intIndex] = {
                            ID: this.selDiscountType.val(),
                            DiscountPercent: this.txtDiscountPercent.val(),
                            DiscountRemarks: this.txtDiscountRemarks.val()
                        };
                        //Senior Citizen PWD
                        //check first if SCD or PWD
                        if (this.isSeniorCitizenPWDDiscount) {
                            for (var i = 0; i < this.arrCustomer.length; i++){
                                pDataSeniorCitizenPWD.push({
                                    ID: this.arrCustomer[i][0] //ID
                                })
                            }
                        }
                    }

                    var pData;

                    if (CStr2(this.selDiscountType.val()) == '') {
                        pData = {
                            InvoiceID: sessionStorage.CartOrderID
                        };
                        SalesPayment_offline().RemoveBillingDiscount(pData);
                        if (OfflineMode) {
                            //Temporarily not needed
                            //return SalesPayment_offline().getSalesPaymentReceiptFooterAmountsInfo(sessionStorage.CartOrderID);
                        } else {
                            //Get Footer Amounts ||| Discount.js 479 code
                        }
                    } else {
                        pData = {
                            InvoiceID: sessionStorage.CartOrderID,
                            TotalDiscountAmount: this.totaldiscount,
                            NoOfPeople: this.selDiscountType.val().toUpperCase() == '055A0A33-DD98-47EC-ADD3-8B2009F3944A' ? $.trim(this.txtSalesPayNoOfDiners.val()) : "", //for senior citizen discount
                            NoOfSeniorCitizens: this.selDiscountType.val().toUpperCase() == '055A0A33-DD98-47EC-ADD3-8B2009F3944A' ? $.trim(this.txtSalesPayNoOfSC.val()) : "", //for senior citizen discount
                            SalesPaymentDiscountList: pDataDiscount,
                            SalesPaymentSeniorCitizenPWDList: pDataSeniorCitizenPWD
                        };

                        if (OfflineMode) {
                            SalesPayment_offline().SaveBillingDiscount(pData);
                            //Temporarily not needed
                            //return SalesPayment_offline().getSalesPaymentReceiptFooterAmountsInfo(sessionStorage.CartOrderID);
                        } else {
                            //ONLINE Discount.js line 503 code
                        }
                    }
                },
                GetSSDItemsWithVatExempt: function () {
                    var pDT;

                    pSql = "Select Sum(SubTotal) as Total " +
                            " from TransInvoiceItems " +
                            " inner join LibraryOrganizationPIM " +
                            " ON LibraryOrganizationPIM.ProductID = TransInvoiceItems.ProductID " +
                            " where InvoiceID = " + ReadyString(sessionStorage.CartOrderID) +
                            " AND LibraryOrganizationPIM.IsVatExempt=1";

                    pDT = Sql.Execute(pSql);

                    return pDT.Rows[0].Total;
                },
                GetMaxSCDiscountableAmount: function(){
                    var pSql = '';

                    pSql = 'Select MaxAllowableDiscountableAmount from LibraryOrganizationTradeName ' +
                            'inner join private.LibraryReceiptSettings on private.LibraryReceiptSettings.OrgTradeNameID = LibraryOrganizationTradeName.ID ' +
                            'inner join private.LibraryReceiptSettingsFowst on private.LibraryReceiptSettingsFowst.ReceiptSettingsID = private.LibraryReceiptSettings.ID ' +                    
                            'where private.LibraryReceiptSettingsFowst.FowstID = ' + ReadyString(sessionStorage.FowstID);

                    return CDec2(Sql.ExecuteScalar(pSql));
                },
                GetInvoiceCharges: function () {
                    pSql = "Select SUM(ChargeAmount) from TransInvoiceCharges where InvoiceID=" + ReadyString(sessionStorage.CartOrderID);

                    return CDec2(Sql.ExecuteScalar(pSql));
                },
                GetTaxPercent: function () {
                    var tax = SalesPayment_offline().ShowSalesPaymentsInfo(sessionStorage.CartOrderID).Rows[0];
                    this.taxPercent = tax.TaxPercent;
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
                btnOPIssuanceDate: null,
                btnOPExpirationDate: null,
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
                    Popup.popupname = '';
                    $('#listOP').empty().append('<li>List is empty</li>');
                },
                init: function (param) {
                    this.form = $('#formFMCGOtherPayment');
                    this.txtOPID = this.form.find('[name="txtOPID"]');
                    this.selOPPaymentType = this.form.find('[name="selOPPaymentType"]');
                    this.txtOPAvailablePts = this.form.find('[name="txtOPAvailablePts"]');
                    this.txtOPIssuingOrg = this.form.find('[name="txtOPIssuingOrg"]');
                    this.txtOPCardHolder = this.form.find('[name="txtOPCardHolder"]');
                    this.txtOPCardNo = this.form.find('[name="txtOPCardNo"]');
                    this.txtOPApprovalNo = this.form.find('[name="txtOPApprovalNo"]');
                    this.txtOPAmount = this.form.find('[name="txtOPAmount"]');
                    this.btnOPIssuanceDate = this.form.find('[id="btnOPIssuanceDate"]');
                    this.btnOPExpirationDate = this.form.find('[id="btnOPExpirationDate"]');
                    Popup.params = param;

                    $('#popFMCGOtherPayment').popup('open');
                    Popup.popupname = 'op';

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
                        if (Popup.OtherPayment.selOPPaymentType.val() == "V-Plus") {
                            $('#divAvailablePts').show();
                            Popup.OtherPayment.GetVPlusPoints();
                        }
                        else {
                            $('#divAvailablePts').hide();
                        }
                    })

                    this.btnOPIssuanceDate.unbind('click').click(function (e) {
                        e.preventDefault();
                        SingleCalendar({
                            Click: function (e) {
                                $('#popFMCGOtherPayment').popup('open');
                                Popup.OtherPayment.btnOPIssuanceDate.text(e);
                                Popup.OtherPayment.txtOPAmount.focus();
                            },
                            Close: function () {
                                $('#popFMCGOtherPayment').popup('open');
                                Popup.OtherPayment.txtOPAmount.focus();
                                Popup.popupname = 'op';
                            }
                        })
                    })

                    this.btnOPExpirationDate.unbind('click').click(function (e) {
                        e.preventDefault();
                        SingleCalendar({
                            Click: function (e) {
                                $('#popFMCGOtherPayment').popup('open');
                                Popup.OtherPayment.btnOPExpirationDate.text(e);
                                Popup.OtherPayment.txtOPAmount.focus();
                            },
                            Close: function () {
                                $('#popFMCGOtherPayment').popup('open');
                                Popup.OtherPayment.txtOPAmount.focus();
                                Popup.popupname = 'op';
                            }
                        })
                    })

                    this.form.unbind('submit').submit(function (e) {
                        e.preventDefault();
                        var pData;

                        if ($.trim(Popup.OtherPayment.selOPPaymentType.val()) == "" || $.trim(Popup.OtherPayment.txtOPIssuingOrg.val()) == ""
                            || $.trim(Popup.OtherPayment.txtOPCardNo.val()) == "" || $.trim(Popup.OtherPayment.txtOPAmount.val()) == "") {
                            $('#txtOPMsgRequired').pageshow();
                            PopupReposition();
                            //ResizePopupBackground();
                            return;
                        }

                        if (Popup.OtherPayment.selOPPaymentType.val() == "V-Plus" && $.trim(Popup.OtherPayment.txtOPAvailablePts.val()) == "") {
                            $('#txtOPMsgRequired').pageshow();
                            PopupReposition();
                            //ResizePopupBackground();
                            return;
                        }

                        $('#txtOPMsgRequired, #txtOPMsgErrorIssuanceDate, #txtOPMsgErrorExpirationDate, #txtOPMsgErrorVPlus').hide();

                        var issuanceDate = Popup.OtherPayment.btnOPIssuanceDate.text();
                        var expirationDate = Popup.OtherPayment.btnOPExpirationDate.text();

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
                            Popup.OtherPayment.txtOPID.val(), // 0
                            Popup.OtherPayment.selOPPaymentType.val(), // 1
                            Popup.OtherPayment.txtOPIssuingOrg.val(), // 2
                            Popup.OtherPayment.txtOPCardHolder.val(), // 3
                            Popup.OtherPayment.txtOPCardNo.val(), // 4
                            (issuanceDate == false ? "" : issuanceDate.getMonth() + 1), // 5 Issuance Date Month
                            (issuanceDate == false ? "" : issuanceDate.getDate()), // 6 Issuance Date Day
                            (issuanceDate == false ? "" : issuanceDate.getFullYear()), // 7  Issuance Date Year
                            (expirationDate == false ? "" : expirationDate.getMonth() + 1), // 8 Expiration Date Month
                            (expirationDate == false ? "" : expirationDate.getDate()), // 9 Expiration Date Day
                            (expirationDate == false ? "" : expirationDate.getFullYear()), // 10 Expiration Date Year
                            Popup.OtherPayment.txtOPApprovalNo.val(), // 11
                            Popup.OtherPayment.txtOPAmount.val(), // 12
                            Popup.OtherPayment.opState // 13
                        ]

                        if (Popup.OtherPayment.editIndex == null) {
                            Popup.OtherPayment.arrOP.push(pData);
                        }
                        else {
                            Popup.OtherPayment.arrOP[Popup.OtherPayment.editIndex] = pData;
                            Popup.OtherPayment.editIndex = null;
                        }

                        Popup.isInAddPage = true;
                        Popup.OtherPayment.LoadOPList();
                        Popup.returnVal = Popup.OtherPayment.arrOP;
                    })

                    $('#btnAddOP').unbind('click').click(function () {
                        $('#pageNewOP').pageshow();
                        PopupReposition();
                    })
                },
                GetVPlusPoints: function () {
                    AjaxSend({
                        url: 'SalesPayments/GetVPlusPoints',
                        data: { CustomerID: $.trim(sessionStorage.CustomerOrganizationID) },
                        success: function (vData) {
                            if (!vData.Result == '') {
                                Popup.OtherPayment.txtOPAvailablePts.val(CDec2(vData.Result));
                            }
                        },
                        error: function (vError) {
                            $('#txtOPMsgErrorVPlus').pageshow();
                        }
                    });
                },
                Add: function () {
                    this.txtOPID.val('')
                    this.selOPPaymentType.val('')
                    this.txtOPAvailablePts.val('')
                    this.txtOPIssuingOrg.val('')
                    this.txtOPCardNo.val('')
                    this.txtOPCardHolder.val('')
                    this.txtOPApprovalNo.val('')
                    this.txtOPAmount.val('')
                    this.btnOPIssuanceDate.text('Set Date')
                    this.btnOPExpirationDate.text('Set Date')
                    $('#pageNewOP').pageshow();
                    $('#divAvailablePts, #txtOPMsgRequired, #txtOPMsgErrorIssuanceDate, #txtOPMsgErrorExpirationDate, #txtOPMsgErrorVPlus').hide();
                    PopupReposition();
                    //ResizePopupBackground();
                    setTimeout(function () {
                        Popup.OtherPayment.selOPPaymentType.focus();
                    }, 100)
                },
                Edit: function (data) {
                    var issuancedate = new Date(data[5] + '/' + data[6] + '/' + data[7]);
                    var expirationdate = new Date(data[8] + '/' + data[9] + '/' + data[10]);

                    this.txtOPID.val(data[0]);
                    this.selOPPaymentType.val(data[1]);
                    this.txtOPIssuingOrg.val(data[2]);
                    this.txtOPCardHolder.val(data[3]);
                    this.txtOPCardNo.val(data[4]);
                    this.txtOPApprovalNo.val(data[11]);
                    this.txtOPAmount.val(data[12]);
                    this.btnOPIssuanceDate.text((IsDate(issuancedate) ? $.format.date(issuancedate, 'MMM dd, yyyy') : "Set Date"));
                    this.btnOPExpirationDate.text((IsDate(expirationdate) ? $.format.date(expirationdate, 'MMM dd, yyyy') : "Set Date"));
                    this.srvState = data[13];
                    this.editIndex = data[14];

                    $('#pageNewOP').pageshow();
                    $('#txtOPMsgRequired, #txtOPMsgErrorIssuanceDate, #txtOPMsgErrorExpirationDate, #txtOPMsgErrorVPlus').hide();
                    PopupReposition(); //NewMessageBox.js
                    //ResizePopupBackground();
                    setTimeout(function () {
                        Popup.OtherPayment.selOPPaymentType.focus();
                    }, 100)
                },
                Delete: function (data) {
                    this.arrOP[data[14]][13] = "Remove"
                    this.LoadOPList();
                },
                Main: function () {
                    $('#pageListOfOP').pageshow();
                    this.LoadOPList();
                    PopupReposition();
                    //ResizePopupBackground();
                },
                LoadRequiredData: function () {
                    
                },
                LoadOPList: function () {
                    var listOP = $('#listOP');
                    var isEmpty = true;

                    listOP.empty();

                    if (this.arrOP.length > 0) {
                        for (var i = 0; i < this.arrOP.length; i++) {
                            var dis = this.arrOP[i];
                            dis.push(i); // add index # in array, use for editing
                            if (dis[13] != "Remove") {
                                listOP.append($('<li>')
                                    .data('data', dis)
                                    .append($('<div>').html('<b>Card No: ' + dis[4] + '</b>'))
                                    .append($('<div>').html('Amount: ' + (dis[12] == 0 ? '0.00' : AccountingFormat(dis[12]))))
                                    .click(function () {
                                        Popup.OtherPayment.Edit($(this).data('data'))
                                    })
                                )
                                isEmpty = false; // set to false when there's no remove state in arrop, found in index 13, w/c means add, edit, remove
                            }
                        }

                        listOP.find('li').first().addClass('navList-a');
                    }

                    if (isEmpty) {
                        listOP.append($('<li>').text('List is empty'));
                    }

                    $('#pageListOfOP').pageshow();
                    //ResizePopupBackground();
                    PopupReposition();                    
                    this.TotalOP();
                },
                TotalOP: function () {
                    if (this.arrOP.length > 0) {
                        this.totalop = 0;

                        for (var i = 0; i < this.arrOP.length; i++) {
                            var dis = this.arrOP[i];
                            if (dis[13] != "Remove") {
                                this.totalop = CDec2(this.totalop) + CDec2(dis[12]);
                            }
                        }
                    }
                    Popup.Misc.TotalMisc();
                    Popup.ComputePayment();
                }
            },
            Cash: {
                Clear: function () {
                    this.cashTendered = 0;
                    Popup.popupname = '';
                    $('#txtCash').text('0.00');
                },
                cashTendered: 0,
                init: function (params) {
                    $('#popFMCGCash').popup('open');
                    $('[name="txtCashAmount"]').focus();
                    var amount = $('[name="txtCashAmount"]');
                    var curCash = 0;
                    Popup.popupname = 'cash';
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
                        Popup.Cash.cashTendered = CDec2(amount.val());
                        params.success(amount.val());
                        $('#popFMCGCash').popup('close');
                        $('#txtCashMsg').hide();
                        FmcgBasicCart.txtQtyPlu.focus();
                        Popup.ComputePayment();
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
                txtCardApprovalNo: null,
                txtCardInstallmentNo: null,
                txtCardAmount: null,
                btnCardIssuanceDate: null,
                btnCardExpirationDate: null,
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
                    //this.btnCardIssuanceDate.text('Set Date');
                    //this.btnCardExpirationDate.text('Set Date');
                    $('.txtCard').text('0.00');
                    Popup.popupname = '';

                    $('#listCard').empty().append('<li>List is empty</li>');
                },
                init: function (param) {
                    this.form = $('#formFMCGCard');
                    this.txtCardID = this.form.find('[name="txtCardID"]');
                    this.selCardType = this.form.find('[name="selCardType"]');
                    this.selCreditCardType = this.form.find('[name="selCreditCardType"]');
                    this.selCardIssuingBank = this.form.find('[name="selCardIssuingBank"]');
                    this.selCardBankLocation = this.form.find('[name="selCardBankLocation"]');
                    this.txtCardFname = this.form.find('[name="txtCardFname"]');
                    this.txtCardMname = this.form.find('[name="txtCardMname"]');
                    this.txtCardLname = this.form.find('[name="txtCardLname"]');
                    this.selCardGenSuffix = this.form.find('[name="selCardGenSuffix"]');
                    this.txtCardNo = this.form.find('[name="txtCardNo"]');
                    this.txtCardApprovalNo = this.form.find('[name="txtCardApprovalNo"]');
                    this.txtCardInstallmentNo = this.form.find('[name="txtCardInstallmentNo"]');
                    this.txtCardAmount = this.form.find('[name="txtCardAmount"]');
                    this.btnCardIssuanceDate = this.form.find('[id="btnCardIssuanceDate"]');
                    this.btnCardExpirationDate = this.form.find('[id="btnCardExpirationDate"]');
                    this.btnCardBack = this.form.find('[id="btnCardBack"]');
                    Popup.params = param;

                    $('#popFMCGCard').popup('open');
                    Popup.popupname = 'card';

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
                    BindKeypress('#formFMCGCard', this.txtCardApprovalNo, true, 'text');
                    BindKeypress('#formFMCGCard', this.txtCardInstallmentNo, true, 'text');
                    BindKeypress('#formFMCGCard', this.txtCardAmount, true, 'text');

                    $('#btnAddCard').unbind('click').click(function () {
                        Popup.Card.Add();
                    })

                    this.selCardType.change(function (e) {
                        if ($(this).val() == "Debit") {
                            $('#divFMCGCredit').hide();
                            Popup.Card.selCreditCardType.attr('hidden', '');
                        }
                        else {
                            $('#divFMCGCredit').show();
                            Popup.Card.selCreditCardType.removeAttr('hidden');
                        }

                        PopupReposition();
                    })

                    this.btnCardIssuanceDate.unbind('click').click(function (e) {
                        e.preventDefault();
                        SingleCalendar({
                            Click: function (e) {
                                $('#popFMCGCard').popup('open');
                                Popup.Card.btnCardIssuanceDate.text(e);
                                Popup.Card.txtCardAmount.focus();
                            },
                            Close: function () {
                                $('#popFMCGCard').popup('open');
                                Popup.Card.txtCardAmount.focus();
                                Popup.popupname = 'card';
                            }
                        })
                    })

                    this.btnCardExpirationDate.unbind('click').click(function (e) {
                        e.preventDefault();
                        SingleCalendar({
                            Click: function (e) {
                                $('#popFMCGCard').popup('open');
                                Popup.Card.btnCardExpirationDate.text(e);
                                Popup.Card.txtCardAmount.focus();
                            },
                            Close: function () {
                                $('#popFMCGCard').popup('open');
                                Popup.Card.txtCardAmount.focus();
                                Popup.popupname = 'card';
                            }
                        })
                    })

                    this.form.unbind('submit').submit(function (e) {
                        e.preventDefault();
                        var pData;
                        var cardtype = $.trim(Popup.Card.selCardType.val());

                        if (cardtype == "Credit") {
                            if ($.trim(Popup.Card.selCreditCardType.val()) == "") {
                                $('#txtErrorMsgRequired').pageshow();
                                PopupReposition();
                                //ResizePopupBackground();
                                return;
                            }
                        }

                        if ($.trim(Popup.Card.selCardIssuingBank.val()) == "" || $.trim(Popup.Card.txtCardLname.val()) == ""
                                    || $.trim(Popup.Card.txtCardFname.val()) == "" || $.trim(Popup.Card.txtCardNo.val()) == ""
                                    || $.trim(Popup.Card.txtCardAmount.val()) == "") {
                            $('#txtErrorMsgRequired').pageshow();
                            PopupReposition();
                            //ResizePopupBackground();
                            return;
                        }
                        else {
                            $('#txtErrorMsgRequired, #txtErrorIssuanceDate, #txtErrorExpirationDate').hide();

                            var issuanceDate = Popup.Card.btnCardIssuanceDate.text();
                            var expirationDate = Popup.Card.btnCardExpirationDate.text();

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
                                    Popup.Card.txtCardID.val(), // 0
                                    (cardtype == "Debit" ? "" : Popup.Card.selCreditCardType.val()), // 1
                                    Popup.Card.selCardIssuingBank.val(), // 2
                                    Popup.Card.selCardIssuingBank.find('option:selected').text(), // 3
                                    Popup.Card.txtCardLname.val(), // 4
                                    Popup.Card.txtCardFname.val(), // 5
                                    Popup.Card.txtCardMname.val(), // 6                                    
                                    Popup.Card.selCardGenSuffix.val(), // 7
                                    Popup.Card.selCardBankLocation.val(), // 8
                                    Popup.Card.selCardBankLocation.find('option:selected').text(), // 9
                                    Popup.Card.txtCardNo.val(), // 10
                                    (issuanceDate == false ? "" : issuanceDate.getMonth() + 1), // 11 Issuance Date Month
                                    (issuanceDate == false ? "" : issuanceDate.getDate()), // 12 Issuance Date Day
                                    (issuanceDate == false ? "" : issuanceDate.getFullYear()), // 13  Issuance Date Year
                                    (expirationDate == false ? "" : expirationDate.getMonth() + 1), // 14 Expiration Date Month
                                    (expirationDate == false ? "" : expirationDate.getDate()), // 15 Expiration Date Day
                                    (expirationDate == false ? "" : expirationDate.getFullYear()), // 16 Expiration Date Year
                                    Popup.Card.txtCardApprovalNo.val(), // 17
                                    Popup.Card.txtCardInstallmentNo.val(), // 18
                                    Popup.Card.txtCardAmount.val(), // 19
                                    Popup.Card.cdState, // 20
                                    cardtype // 21
                            ]

                            if (cardtype == "Debit") {
                                if (Popup.Card.editIndex == null) {
                                    Popup.Card.arrDebit.push(pData);
                                }
                                else {
                                    Popup.Card.arrDebit[Popup.Card.editIndex] = pData
                                    Popup.ChequeGC.editIndex = null; // set to null after edit
                                }
                            }
                            else {
                                if (Popup.Card.editIndex == null) {
                                    Popup.Card.arrCredit.push(pData);
                                }
                                else {
                                    Popup.Card.arrCredit[Popup.Card.editIndex] = pData
                                    Popup.ChequeGC.editIndex = null; // set to null after edit
                                }
                            }

                            Popup.isInAddPage = true;
                            Popup.Card.LoadCardList();
                            Popup.returnVal = Popup.Card.totalcardamt;
                        }
                    })

                    this.btnCardBack.unbind('click').click(function (e) {
                        e.preventDefault();
                        $('#divListOfCard').pageshow();
                        PopupReposition();
                    })
                },
                Add: function () {
                    this.selCardType.val('Debit').focus();
                    this.selCreditCardType.val('')
                    this.selCardIssuingBank.val('')
                    this.selCardBankLocation.val('e783ed13-aa98-4cc1-918e-d49b05c0167e')
                    this.txtCardFname.val('')
                    this.txtCardMname.val('')
                    this.txtCardLname.val('')
                    this.txtCardNo.val('')
                    this.txtCardApprovalNo.val('')
                    this.txtCardInstallmentNo.val('')
                    this.txtCardAmount.val('')
                    this.btnCardIssuanceDate.text('Set Date')
                    this.btnCardExpirationDate.text('Set Date')
                    this.cdState = 'New';

                    $('#divNewCard').pageshow();
                    $('#divFMCGCredit, #txtErrorMsgRequired, #txtErrorIssuanceDate, #txtErrorExpirationDate').hide();
                    this.selCreditCardType.attr('hidden', '')
                    PopupReposition(); //NewMessageBox.js
                    setTimeout(function () {
                        Popup.Card.selCardType.focus();
                    }, 100)
                    //ResizePopupBackground();
                },
                Edit: function (data) {
                    var issuancedate = new Date(data[11] + '/' + data[12] + '/' + data[13]);
                    var expirationdate = new Date(data[14] + '/' + data[15] + '/' + data[16]);

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
                    this.txtCardApprovalNo.val(data[17]);
                    this.txtCardInstallmentNo.val(data[18]);
                    this.txtCardAmount.val(data[19]);
                    this.btnCardIssuanceDate.text((IsDate(issuancedate) ? $.format.date(issuancedate, 'MMM dd, yyyy') : "Set Date"));
                    this.btnCardExpirationDate.text((IsDate(expirationdate) ? $.format.date(expirationdate, 'MMM dd, yyyy') : "Set Date"));
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
                        Popup.Card.selCardType.focus();
                    }, 100)
                    //ResizePopupBackground();
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
                    //ResizePopupBackground();
                },
                LoadRequiredData: function () {
                    var vRows = Sql.Execute('SELECT ID, CompanyName FROM LibraryOrganization WHERE LibraryOrganization.IsABank = 1 ORDER BY LibraryOrganization.CompanyName').Rows;
                    this.selCardIssuingBank.empty();
                    this.selCardIssuingBank.append(new Option('Please Select', ''))
                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            this.selCardIssuingBank.append(new Option(vRows[i].CompanyName, vRows[i].ID))
                        }
                    }

                    vRows = Sql.Execute('SELECT ID, Name FROM LibraryCountries ORDER BY LibraryCountries.Name').Rows;
                    this.selCardBankLocation.empty();
                    this.selCardBankLocation.append(new Option('Please Select', ''))
                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            this.selCardBankLocation.append(new Option(vRows[i].Name, vRows[i].ID))
                        }
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
                                listCard.append($('<li>')
                                    .data('data', dis)
                                    .append($('<div>').html('Card Type: ' + dis[21] + '<br>'))
                                    .append($('<div>').html('Amount: ' + (dis[19] == 0 ? '0.00' : AccountingFormat(dis[19]))))
                                    .click(function () {
                                        Popup.Card.Edit($(this).data('data'))
                                    })
                                )
                                isDebitCardEmpty = false; // set to false when there's no remove state in arrDebit, found in index 20, w/c means add, edit, remove
                            }
                        }

                        for (var i = 0; i < this.arrCredit.length; i++) {
                            var dis = this.arrCredit[i];
                            dis.push(i); // add index # in array, use for editing
                            if (dis[20] != "Remove") {
                                listCard.append($('<li>')
                                    .data('data', dis)
                                    .append($('<div>').html('Card Type: ' + dis[21] + '<br>'))
                                    .append($('<div>').html('Amount: ' + (dis[19] == 0 ? '0.00' : AccountingFormat(dis[19]))))
                                    .click(function () {
                                        Popup.Card.Edit($(this).data('data'))
                                    })
                                )
                                isCreditCardEmpty = false; // set to false when there's no remove state in arrCredit, found in index 20, w/c means add, edit, remove
                            }
                        }

                        listCard.find('li').first().addClass('navList-a');
                    }

                    if (isDebitCardEmpty && isCreditCardEmpty) {
                        listCard.append($('<li>').text('List is empty'))
                    }
                    
                    $('#divListOfCard').pageshow();
                    //ResizePopupBackground();
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
                        $('.txtCard').text((this.totalcardamt > 0 ? AccountingFormat(this.totalcardamt) : '0.00'))
                    }

                    Popup.ComputePayment();
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
                editIndex: null,
                srvState: 'New',
                arrSrv: [],
                totalsrv: 0,
                Clear: function () {
                    this.arrSrv = [];
                    this.totalsrv = 0;
                    this.editIndex = null;
                    this.srvState = 'New';
                    Popup.popupname = '';
                    $('#listSrv').empty().append('<li>List is empty</li>');
                },
                init: function (param) {
                    this.form = $('#formFMCGSrv');
                    this.txtSrvID = this.form.find('[name="txtSrvID"]');
                    this.txtSrvTransMemoID = this.form.find('[name="txtSrvTransMemoID"]');
                    this.txtSrvNo = this.form.find('[name="txtSrvNo"]');
                    this.txtSrvPinCode = this.form.find('[name="txtSrvPinCode"]');
                    this.txtMainSrvAmount = this.form.find('[name="txtMainSrvAmount"]');
                    this.txtSrvAmount = this.form.find('[name="txtSrvAmount"]');
                    Popup.params = param;

                    $('#popFMCGSrv').popup('open');
                    Popup.popupname = 'srv';

                    this.Main();

                    BindKeypress('#formFMCGSrv', this.txtSrvNo, true, 'text');
                    BindKeypress('#formFMCGSrv', this.txtSrvPinCode, false, 'text');

                    $('#btnScanSrvNo').unbind('click').click(function (e) {
                        e.preventDefault();
                        if (IsMobile) {
                            mobile.Scan({
                                Success: function (vData) {
                                    Popup.Srv.txtSrvNo.val(vData.Value);
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
                                    Popup.Srv.txtSrvPinCode.val(vData.Value);
                                }
                            });
                        } else {
                            $('#txtSrvMsgBarcode').pageshow();
                        }
                    })

                    $('#btnVerifySrv').unbind('click').click(function (e) {
                        e.preventDefault();
                        Popup.Srv.VerifySrv();
                    })

                    $('#btnAddSrv').unbind('click').click(function () {
                        $('#pageNewSrv').pageshow();
                        PopupReposition();
                    })

                    this.form.unbind('submit').submit(function (e) {
                        e.preventDefault();
                        Popup.Srv.VerifySrv();
                    })
                },
                VerifySrv: function () {
                    if ($.trim(this.txtSrvNo.val()) == "" || $.trim(this.txtSrvPinCode.val()) == "") {
                        $('#txtSrvMsgRequired').pageshow();
                        PopupReposition();
                        //ResizePopupBackground();
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
                        ]

                        if (this.editIndex == null) {
                            this.arrSrv.push(pData);
                        }
                        else {
                            this.arrSrv[this.editIndex] = pData
                            this.editIndex = null; // set to null after edit
                        }

                        Popup.isInAddPage = true;
                        this.LoadSrvList();
                        Popup.returnVal = this.arrSrv;
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

                                    Popup.Srv.txtSrvTransMemoID.val(vData.MemoID || '');
                                    Popup.Srv.txtMainSrvAmount.val(AccountingFormat(vData.SRVAmount) || '0.00');

                                    if (CDec2($("#txtBalChange").text()) <= CDec2(vData.SRVAmount)) {
                                        Popup.Srv.txtSrvAmount.val($("#txtBalChange").text());
                                    } else {
                                        Popup.Srv.txtSrvAmount.val(AccountingFormat(vData.SRVAmount) || '0.00');
                                    }

                                } else {

                                    Popup.Srv.txtSrvTransMemoID.val('');
                                    Popup.Srv.txtMainSrvAmount.val('0.00');
                                    Popup.Srv.txtSrvAmount.val('0.00');

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
                    //ResizePopupBackground();
                    setTimeout(function () {
                        Popup.Srv.txtSrvNo.focus();
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
                    //ResizePopupBackground();
                    setTimeout(function () {
                        Popup.Srv.txtSrvNo.focus();
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
                    //ResizePopupBackground();
                },
                LoadSrvList: function () {
                    var listSrv = $('#listSrv');
                    var isEmpty = true;

                    listSrv.empty();

                    if (this.arrSrv.length > 0) {
                        for (var i = 0; i < this.arrSrv.length; i++) {
                            var dis = this.arrSrv[i];
                            if (dis[6] != "Remove") {
                                listSrv.append($('<li>')
                                    .data('data', dis)
                                    .append($('<div>').html('<b>SRV No: ' + dis[2] + '</b>'))
                                    .append($('<div>').html('Amount: ' + (dis[5] == 0 ? '0.00' : AccountingFormat(dis[5]))))
                                    .click(function () {
                                        Popup.Srv.Edit($(this).data('data'))
                                    })
                                )
                                isEmpty = false; // set to false when there's no remove state in arrSrv, found in index 6, w/c means add, edit, remove
                            }
                        }

                        listSrv.find('li').first().addClass('navList-a');
                    }

                    if (isEmpty) {
                        listSrv.append($('<li>').text('List is empty'));
                    }

                    $('#pageListOfSrv').pageshow();
                    //ResizePopupBackground();
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

                    Popup.Misc.TotalMisc();
                    Popup.ComputePayment();
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
                    Popup.popupname = '';
                    $('#listCharge').empty().append('<li>List is empty</li>');
                },
                init: function (param) {
                    this.form = $('#formFMCGCharge');
                    this.selChargeType = this.form.find('[name="selChargeType"]');
                    this.txtChargePercentage = this.form.find('[name="txtChargePercentage"]');
                    this.txtChargeAmount = this.form.find('[name="txtChargeAmount"]');
                    this.txtChargeRemarks = this.form.find('[name="txtChargeRemarks"]');
                    Popup.params = param;

                    $('#popFMCGCharges').popup('open');
                    Popup.popupname = 'charges';

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
                        Popup.Charges.Add();
                    })

                    this.txtChargePercentage.unbind('keyup').keyup(function () {
                        if ($.trim($(this).val()) != "") {
                            var chargeAmount = CDec2($.trim($(this).val())) / 100;
                            Popup.Charges.longChargeAmount = chargeAmount * CDec2(Popup.Charges.lblVATableSales);
                            Popup.Charges.txtChargeAmount.val((Popup.Charges.longChargeAmount == 0 ? '0' :AccountingFormat(Popup.Charges.longChargeAmount)));
                        }
                    })

                    this.txtChargeAmount.unbind('keyup').keyup(function () {
                        if ($.trim($(this).val()) != "") {
                            var chargePercent = CDec2($.trim($(this).val())) / CDec2(Popup.Charges.lblVATableSales);
                            Popup.Charges.longChargeAmount = $(this).val();
                            Popup.Charges.txtChargePercentage.val(((chargePercent * 100) == 0 ? '0' : AccountingFormat(chargePercent * 100)));
                        }
                    })

                    this.form.unbind('submit').submit(function (e) {
                        e.preventDefault();

                        if ($.trim(Popup.Charges.txtChargeAmount.val()) == "") {
                            $('#txtChargeErrorMsgRequired').show();
                            PopupReposition();
                            //ResizePopupBackground();
                            return;
                        }
                        else {
                            $('#txtChargeErrorMsgRequired').hide();
                            Popup.isInAddPage = true;

                            var hasMatch = false;
                            var pDataCharges = Popup.Charges.arrCharge;

                            if (pDataCharges[0] != undefined) {
                                for (var i = 0; i < pDataCharges.length; i++) {
                                    if (Popup.Charges.selChargeType.val() == pDataCharges[i].ChargeType) {
                                        //charges

                                        pDataCharges[i] = {
                                            ChargeType: $.trim(Popup.Charges.selChargeType.val()),
                                            ChargeAmount: $.trim(Popup.Charges.selChargeType.val()) != 'Tip' && $.trim(Popup.Charges.selChargeType.val()) != 'Local Tax' ? Popup.Charges.longChargeAmount : 0,
                                            TipAmount: $.trim(Popup.Charges.selChargeType.val()) == 'Tip' ? Popup.Charges.longChargeAmount : 0,
                                            TaxAmount: $.trim(Popup.Charges.selChargeType.val()) == 'Local Tax' ? Popup.Charges.longChargeAmount : 0,
                                            Remarks: $.trim(Popup.Charges.txtChargeRemarks.val())
                                        };
                                        hasMatch = true;
                                        break;
                                    }
                                }


                                if (!hasMatch) {
                                    pDataCharges[1] = {
                                        ChargeType: $.trim(Popup.Charges.selChargeType.val()),
                                        ChargeAmount: $.trim(Popup.Charges.selChargeType.val()) != 'Tip' && $.trim(Popup.Charges.selChargeType.val()) != 'Local Tax' ? Popup.Charges.longChargeAmount : 0,
                                        TipAmount: $.trim(Popup.Charges.selChargeType.val()) == 'Tip' ? Popup.Charges.longChargeAmount : 0,
                                        TaxAmount: $.trim(Popup.Charges.selChargeType.val()) == 'Local Tax' ? Popup.Charges.longChargeAmount : 0,
                                        Remarks: $.trim(Popup.Charges.txtChargeRemarks.val())
                                    };
                                }
                            } else {
                                Popup.Charges.arrCharge[0] = {
                                    ChargeType: $.trim(Popup.Charges.selChargeType.val()),
                                    ChargeAmount: $.trim(Popup.Charges.selChargeType.val()) != 'Tip' && $.trim(Popup.Charges.selChargeType.val()) != 'Local Tax' ? Popup.Charges.longChargeAmount : 0,
                                    TipAmount: $.trim(Popup.Charges.selChargeType.val()) == 'Tip' ? Popup.Charges.longChargeAmount : 0,
                                    TaxAmount: $.trim(Popup.Charges.selChargeType.val()) == 'Local Tax' ? Popup.Charges.longChargeAmount : 0,
                                    Remarks: $.trim(Popup.Charges.txtChargeRemarks.val())
                                };
                            }
                            var pParams = GetUrlParameters();
                            var invoiceID = CStr2(pParams.InvoiceID) == '' ? sessionStorage.CartOrderID : pParams.InvoiceID;
                            var pData = {
                                InvoiceID: invoiceID,
                                SalesPaymentChargeList: pDataCharges
                            };
                            
                            if (SalesPayment_offline().SaveBillingCharges(pData) == 'ok') {
                                // to be fixed
                                Popup.Charges.GetCurrentCharges();
                                Popup.RefreshSalesPaymentTotal();
                                Popup.ComputePayment();
                                Popup.Misc.TotalMisc();
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
                    //ResizePopupBackground();
                    setTimeout(function () {
                        Popup.Charges.selChargeType.focus();
                    }, 100)
                },
                Delete: function(data){
                    var pData = {
                        InvoiceID: sessionStorage.CartOrderID,
                        ChargeType: data.ChargeType
                    };

                    if (SalesPayment_offline().DeleteBillCharges(pData) == 'ok') {
                        Popup.Charges.GetCurrentCharges();
                        //Popup.RefreshSalesPaymentTotal();
                        Popup.ComputePayment();
                    } else {
                        console.log('There was an error while processing your request.');
                    }
                },
                Main: function () {
                    $('#divListOfCharges').pageshow();
                    //this.LoadChargeList();
                    PopupReposition(); //NewMessageBox.js
                    //ResizePopupBackground();
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

                    if(vRows.length > 0){
                        for (var i = 0; i < vRows.length; i++) {
                            listCharge.append($('<li>')
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
                        listCharge.append($('<li>').text('List is empty'));
                    }
                    $('#divListOfCharges').pageshow();
                    //ResizePopupBackground();
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
                    this.form = $('#formFMCGSoldTo');
                    this.txtSoldToSearch = $('[name="txtSoldToSearch"]');
                    this.txtCurrentCustomerName = $('#txtCurrentCustomerName');
                    this.btnSoldToBack = this.form.find('#btnSoldToBack');
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

                    Popup.popupname = 'soldto';
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

                    $('#btnAddSoldTo').unbind('click').click(function () {
                        Popup.SoldTo.Add();
                    })

                    this.txtSoldToSearch.unbind('keydown').keydown(function (e) {
                        if (e.which == 13 && $.trim($(this).val()) != "") {
                            e.preventDefault();
                            var vRows = OfflineBrowser.LibraryOrganization.GetData({
                                keywords: $.trim($(this).val()),
                                FilterCustomers: 1
                            }).Rows
                            Popup.SoldTo.LoadAvailableSearchedCustomer(vRows);
                            Popup.isInAddPage = true;
                            $(this).focusout();
                        }
                        else if (e.which == 107) {
                            // 107  = +
                            e.preventDefault();
                            Popup.SoldTo.Add();
                        }
                    })

                    this.selCustomerType.change(function () {
                        Popup.SoldTo.Add();
                    })

                    this.form.unbind('submit').submit(function (e) {
                        e.preventDefault();
                        if (Popup.SoldTo.selCustomerType.val() == "Organization") {
                            Popup.SoldTo.ValidateNewCustomerOrg();
                        }
                        else {
                            Popup.SoldTo.ValidateNewCustomerPerson();
                        }
                    })

                    this.btnSoldToBack.unbind('click').click(function (e) {
                        e.preventDefault();
                        $('#divListOfCustomer').pageshow();
                        PopupReposition();
                    })
                },
                ValidateNewCustomerOrg: function () {
                    if ($.trim(this.txtCustomerOrgName.val()) == "" || $.trim(this.txtCustomerOrgTin.val()) == "") {
                        $('#txtSoldToErrorMsgRequired').pageshow();
                        PopupReposition();
                        //ResizePopupBackground();
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
                        PopupReposition();
                        //ResizePopupBackground();
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
                    //ResizePopupBackground();
                    setTimeout(function () {
                        Popup.SoldTo.selCustomerType.focus();
                    }, 100)
                },
                Main: function () {
                    $('#divListOfCustomer').pageshow();
                    this.txtSoldToSearch.val('').focus();
                    this.txtCurrentCustomerName.text(sessionStorage.CustomerOrganizationName);
                    $('#listAvailabelCustomer').empty();
                    PopupReposition();
                    //ResizePopupBackground();
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
                                        Popup.SoldTo.SetAsCustomer($(this).data())
                                    })
                                )
                        }
                        listAvailabelCustomer.find('li').first().addClass('navList-a');
                    }
                    else {
                        listAvailabelCustomer.append('<li>No Search Result found</li>')
                    }
                    PopupReposition();
                    //ResizePopupBackground();
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
                    NewMessageBoxClose(); // NewMessageBox.js
                    $('#popFMCGMisc').popup('open');
                    Popup.popupname = 'misc';
                    $('#listMisc').find('li').removeClass('navList-a');
                    $('#listMisc').find('li').first().addClass('navList-a');
                    $('#txtMiscTotalCharge').text((Popup.Charges.totalcharge == 0 ? '0.00' : AccountingFormat(Popup.Charges.totalcharge)))
                    $('#txtMiscTotalSrv').text((Popup.Srv.totalsrv == 0 ? '0.00' : AccountingFormat(Popup.Srv.totalsrv)))
                    $('#txtMiscTotalOp').text((Popup.OtherPayment.totalop == 0 ? '0.00' : AccountingFormat(Popup.OtherPayment.totalop)))

                    $('#btnMiscCharges').unbind('click').click(function () {
                        Popup.Misc.ShowPopup('charges');
                    })

                    $('#btnMiscSrv').unbind('click').click(function () {
                        Popup.Misc.ShowPopup('srv');
                    })

                    $('#btnMiscOp').unbind('click').click(function () {
                        Popup.Misc.ShowPopup('op');
                    })
                },
                ShowPopup: function (popup) {
                    $('#popFMCGMisc').popup('close');
                    
                    if (popup == "op") {
                        //Other Payment
                        Popup.OtherPayment.init();
                    }
                    else if (popup == "charges") {
                        //Charges and Other Tax
                        NewMessageBox('This feature is ongoing development', 'Message', [], {
                            Close: function () {
                                $('#popFMCGMisc').popup('open');
                            }
                        })
                        return;
                        Popup.Charges.init();
                    }
                    else if (popup == "srv") {
                        //Charges and Other Tax
                        Popup.Srv.init();
                    }
                },
                TotalMisc: function () {
                    this.totalmisc = CDec2(Popup.Charges.totalcharge) + CDec2(Popup.Srv.totalsrv) + CDec2(Popup.OtherPayment.totalop);
                    $('.txtMisc').text((this.totalmisc == 0 ? '0.00' : AccountingFormat(this.totalmisc)))
                }
            },
            RefreshSalesPaymentTotal: function(){
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
                            Popup.ShowSalesPaymentTotalResults(vDataFooter);
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
                var cashAmountTendered = parseFloat(CDec2(Popup.Cash.cashTendered));


                if (vDataFooter.Rows.length > 0) {

                    dblDiscount = vDataFooter.Rows[0].DiscountAmount;

                    dblCharges = CDec2(vDataFooter.Rows[0].ChargeAmount);
                    dblTip = CDec2(vDataFooter.Rows[0].TipAmount);
                    dblLocalTaxAmount = CDec2(vDataFooter.Rows[0].LocalTaxAmount);

                    Popup.Discount.totaldiscount = dblDiscount;
                    Popup.Charges.totalcharge = dblCharges;
                    Popup.Charges.totalothertax = dblLocalTaxAmount;
                }
                
            },
            ComputePayment: function () {
                var cashAmountTendered = parseFloat(CDec2(Popup.Cash.cashTendered));

                var dblTotalSalesPay = 0;
                var dblTotalAmountDue = 0;
                var amtDue = Popup.Discount.isSeniorCitizenDiscount ? CDec2($('#lblTotalAmount').text()) : CDec2($('#txtSubTotal').text());

                dblTotalAmountDue = parseFloat(CDec2(amtDue)) - parseFloat(CDec2(Popup.Discount.totaldiscount)) - parseFloat(CDec2(Popup.Charges.totalcharge)) - parseFloat(CDec2(Popup.Charges.totalothertax)) - parseFloat(0) // amtDue - totaldiscount - charges - other tax - tip
                dblTotalSalesPay = (parseFloat(CDec2(Popup.Cash.cashTendered)) + parseFloat(CDec2(Popup.ChequeGC.totalcgc)) + parseFloat(CDec2(Popup.Card.totalcardamt)) + parseFloat(CDec2(Popup.Srv.totalsrv)) + parseFloat(CDec2(Popup.OtherPayment.totalop))) - (parseFloat(CDec2(amtDue)) - parseFloat(CDec2(Popup.Discount.totaldiscount)) + parseFloat(CDec2(Popup.Charges.totalcharge)) + parseFloat(CDec2(0)) + parseFloat(CDec2(Popup.Charges.totalothertax)));
                // (parseFloat(CDec2(cash)) + parseFloat(CDec2(cgc)) + parseFloat(CDec2(debitcard)) + parseFloat(CDec2(creditcard)) + parseFloat(CDec2(srv)) + parseFloat(CDec2(othertype)))
                // - (parseFloat(CDec2(amtDue)) + parseFloat(CDec2(discount)) + parseFloat(CDec2(charges)) + parseFloat(CDec2(tip)) + parseFloat(CDec2(othertax)))

                var totalAmountTendered = parseFloat(CDec2(Popup.Cash.cashTendered)) + parseFloat(CDec2(Popup.ChequeGC.totalcgc)) + parseFloat(CDec2(Popup.Card.totalcardamt)) + parseFloat(CDec2(Popup.Srv.totalsrv)) + parseFloat(CDec2(Popup.OtherPayment.totalop));
                Popup.amounttendered = CDec2(totalAmountTendered);

                $('#txtTotal').text((CInt2(dblTotalAmountDue) == 0 ? '0.00' : AccountingFormat(dblTotalAmountDue)));
                FmcgBasicCart.FinalAmountDue = dblTotalAmountDue;

                if (dblTotalSalesPay >= 0) {
                    $('#lblBalChange').text((CInt2(dblTotalAmountDue) == 0 ? 'UNPAID' : 'CHANGE'));
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
            }
        }

        window.NavigateList = function (list, popupname, dir) {
            try{
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
                            Popup.ChequeGC.Edit(lastVoidProductSelected.data('data'))
                        }
                        else if (popupname == 'card') {
                            Popup.Card.Edit(lastVoidProductSelected.data('data'))
                        }
                        else if (popupname == 'srv') {
                            Popup.Srv.Edit(lastVoidProductSelected.data('data'))
                        }
                        else if (popupname == 'op') {
                            Popup.OtherPayment.Edit(lastVoidProductSelected.data('data'))
                        }
                        else if (popupname == 'soldto') {
                            Popup.SoldTo.SetAsCustomer(lastVoidProductSelected.data('data'))
                        }
                        else if (popupname == 'misc') {
                            Popup.Misc.ShowPopup(lastVoidProductSelected.attr('popup'));
                        }
                    }
                    else if (dir == 46) {
                        //delete
                        if (popupname == 'cgc') {
                            Popup.ChequeGC.Delete(lastVoidProductSelected.data('data'))
                        }
                        else if (popupname == 'card') {
                            Popup.Card.Delete(lastVoidProductSelected.data('data'))
                        }
                        else if (popupname == 'srv') {
                            Popup.Srv.Delete(lastVoidProductSelected.data('data'))
                        }
                        else if (popupname == 'op') {
                            Popup.OtherPayment.Delete(lastVoidProductSelected.data('data'))
                        }
                        else if (popupname == 'charges') {
                            Popup.Charges.Delete(lastVoidProductSelected.data('data'))
                        }
                    }
                    Popup.Scroll('#divAvailableScroll', list.find('li.navList-a'))
                }
            } catch(e){
                WebToast('error')
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
                        if (CBool($(this).attr('isnum'))) {
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
            if (Popup.popupname == "cgc") {
                //Cheque and Gift Certicate
                Popup.ChequeGC.Add();
            }
            else if (Popup.popupname == "card") {
                //Card
                Popup.Card.Add();
            }
            else if (Popup.popupname == "srv") {
                //SRV
                Popup.Srv.Add();
            }
            else if (Popup.popupname == "op") {
                //Other Payment
                Popup.OtherPayment.Add();
            }
            else if (Popup.popupname == "charges") {
                //Charges and Other Tax
                Popup.Charges.Add();
            }
            else if (Popup.popupname == "soldto") {
                //Sold To
                Popup.SoldTo.Add();
            }
            else if (Popup.popupname == "discount") {
                //Discount
                Popup.Discount.Add();
            }
        }
        else if (e.which == 109) {
            // pressing - (means go back to mainpage)
            if (Popup.popupname == "cgc") {
                //Cheque and Gift Certicate
                Popup.ChequeGC.Main();
            }
            else if (Popup.popupname == "card") {
                //Cheque and Gift Certicate
                Popup.Card.Main();
            }
            else if (Popup.popupname == "srv") {
                //SRV
                Popup.Srv.Main();
            }
            else if (Popup.popupname == "op") {
                //Other Payment
                Popup.OtherPayment.Main();
            }
            else if (Popup.popupname == "charges") {
                //Charges and Other Tax
                Popup.Charges.Main();
            }
            else if (Popup.popupname == "soldto") {
                //Charges and Other Tax
                Popup.SoldTo.Main();
            }
            else if (Popup.popupname == "discount") {
                //Charges and Other Tax
                Popup.Discount.Main();
            }
        }
        else if (e.which == 38 || e.which == 40) {
            // arrow up and down and enter - used only in popup with list, to navigate within a list
            if (Popup.popupname == "cgc") {
                NavigateList($('#listCGC'), Popup.popupname, e.which)
            }
            else if (Popup.popupname == "card") {
                NavigateList($('#listCard'), Popup.popupname, e.which)
            }
            else if (Popup.popupname == "srv") {
                NavigateList($('#listSrv'), Popup.popupname, e.which)
            }
            else if (Popup.popupname == "op") {
                NavigateList($('#listOP'), Popup.popupname, e.which)
            }
            else if (Popup.popupname == "soldto") {
                NavigateList($('#listAvailabelCustomer'), Popup.popupname, e.which)
            }
            else if (Popup.popupname == "misc") {
                NavigateList($('#listMisc'), Popup.popupname, e.which)
            }
        }
        else if (e.which == 13 || e.which == 46) {
            //For editing & deleting list in popup payments // ENTER OR DELETE key
            if (Popup.popupname == "cgc" && Popup.isInAddPage == false) {
                NavigateList($('#listCGC'), Popup.popupname, e.which)
            }
            if (Popup.popupname == "card" && Popup.isInAddPage == false) {
                NavigateList($('#listCard'), Popup.popupname, e.which)
            }
            else if (Popup.popupname == "srv" && Popup.isInAddPage == false) {
                NavigateList($('#listSrv'), Popup.popupname, e.which)
            }
            else if (Popup.popupname == "op" && Popup.isInAddPage == false) {
                NavigateList($('#listOP'), Popup.popupname, e.which)
            }
            else if (Popup.popupname == "charges" && Popup.isInAddPage == false) {
                NavigateList($('#listCharge'), Popup.popupname, e.which)
            }
            else if (Popup.popupname == "soldto" && Popup.isInAddPage == false) {
                NavigateList($('#listAvailabelCustomer'), Popup.popupname, e.which)
            }
            else if (Popup.popupname == "misc" && Popup.isInAddPage == false) {
                if (e.which != 46) {
                    // DELETE not allowed
                    NavigateList($('#listMisc'), Popup.popupname, e.which)
                }
            }

            Popup.isInAddPage = false;
        }
    })
})