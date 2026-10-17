require([
    'common'
    //'/Scripts3/Offline/Browser/LibraryOrganizationBanks.js',
    //'/Scripts3/Offline/Browser/LibraryCountries.js'
], function () {
    $(document).ready(function () {
        var isDiscountTypeLoaded = false;

        window.CacPopup = {
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
                selCGCDateMonth: null,
                selCGCDateDay: null,
                selCGCDateYear: null,
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
                    this.txtCGCChequeGCNo = this.form.find('[name="txtCGCChequeGC"]');
                    this.selCGCDateMonth = this.form.find('[name="selCGCDateMonth"]');
                    this.selCGCDateDay = this.form.find('[name="selCGCDateDay"]');
                    this.selCGCDateYear = this.form.find('[name="selCGCDateYear"]');
                    this.btnChequeDate = this.form.find('[id="btnChequeDate"]');
                    this.txtCGCValue = this.form.find('[name="txtCGCValue"]');
                    CacPopup.params = param;

                    $('#popFMCGChequeGC').popup('open');
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
                    BindKeypress('#formFMCGChequeGC', this.selCGCDateMonth, false, 'select');
                    BindKeypress('#formFMCGChequeGC', this.selCGCDateDay, false, 'select');
                    BindKeypress('#formFMCGChequeGC', this.selCGCDateYear, false, 'select');
                    BindKeypress('#formFMCGChequeGC', this.txtCGCValue, true, 'text');

                    $('#btnAddCGC').unbind('click').click(function () {
                        CacPopup.ChequeGC.Add();
                    })

                    this.btnChequeDate.unbind('click').click(function () {
                        SingleCalendar({
                            Click: function (e) {
                                $('#popFMCGChequeGC').popup('open');
                                CacPopup.ChequeGC.btnChequeDate.text(e);
                                CacPopup.ChequeGC.txtCGCValue.focus();
                            },
                            Cancel: function () {
                                $('#popFMCGChequeGC').popup('open');
                                CacPopup.ChequeGC.txtCGCValue.focus();
                            }
                        })
                    })

                    $('#formFMCGChequeGC').unbind('submit').submit(function (e) {
                        e.preventDefault();
                        if ($.trim(CacPopup.ChequeGC.selCGCBank.val()) == "" || $.trim(CacPopup.ChequeGC.txtCGCAccountNo.val()) == ""
                            || $.trim(CacPopup.ChequeGC.txtCGCChequeGCNo.val()) == ""
                            || $.trim(CacPopup.ChequeGC.txtCGCValue.val()) == "" || $.trim(CacPopup.ChequeGC.btnChequeDate.text()) == "Set Date") {
                            $('#txtChequeGCMsg').show();
                        }
                        else {
                            $('#txtChequeGCMsg').hide();
                            var pData = [
                                        CacPopup.ChequeGC.txtCGCID.val(), // 0
                                        CacPopup.ChequeGC.selCGCBank.val(), // 1
                                        CacPopup.ChequeGC.selCGCBank.find('option:selected').text(), // 2
                                        CacPopup.ChequeGC.txtCGCBranchID.val(), // 3
                                        CacPopup.ChequeGC.txtCGCBranch.val(), // 4
                                        CacPopup.ChequeGC.txtCGCAccountName.val(), // 5
                                        CacPopup.ChequeGC.txtCGCAccountNo.val(), // 6
                                        CacPopup.ChequeGC.txtCGCChequeGCNo.val(), // 7
                                        CacPopup.ChequeGC.selCGCDateMonth.val(), // 8
                                        CacPopup.ChequeGC.selCGCDateDay.val(), // 9
                                        CacPopup.ChequeGC.selCGCDateYear.val(), // 10
                                        CacPopup.ChequeGC.txtCGCValue.val(), // 11
                                        CacPopup.ChequeGC.btnChequeDate.text(), // 12
                                        CacPopup.ChequeGC.cgcState // 13
                            ];
                            if (CacPopup.ChequeGC.editIndex == null) {
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
                    this.selCGCDateMonth.val('');
                    this.selCGCDateDay.val('');
                    this.selCGCDateYear.val('');
                    this.txtCGCValue.val('');
                    this.btnChequeDate.text('Set Date');
                    this.cgcState = 'New';

                    $('#pageNewCGG').pageshow();
                    $('#txtChequeGCMsg').hide();
                    PopupReposition(); //NewMessageBox.js
                    CacPopup.ChequeGC.selCGCBank.focus();
                },
                Edit: function (data) {
                    this.txtCGCID.val(data[0]);
                    this.selCGCBank.val(data[1]);
                    this.txtCGCBranchID.val(data[3]);
                    this.txtCGCBranch.val(data[4]);
                    this.txtCGCAccountName.val(data[5]);
                    this.txtCGCAccountNo.val(data[6]);
                    this.txtCGCChequeGCNo.val(data[7]);
                    this.selCGCDateMonth.val(data[8]);
                    this.selCGCDateDay.val(data[9]);
                    this.selCGCDateYear.val(data[10]);
                    this.txtCGCValue.val(data[11]);
                    this.btnChequeDate.text(data[12]);
                    CacPopup.ChequeGC.cgcState = data[13];
                    this.editIndex = data[14];
                    $('#pageNewCGG').pageshow();
                    $('#txtChequeGCMsg').hide();
                    PopupReposition(); //NewMessageBox.js
                    setTimeout(function () {
                        CacPopup.ChequeGC.selCGCBank.focus();
                    }, 100)
                },
                Delete: function (data) {
                    this.arrCGC[data[14]][13] = "Remove";
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
                        url: 'Browser/LibraryBanks',
                        data: { IsAll: 1 },
                        success: function (vData) {
                            var vRows = vData.Results.Rows;
                            dis.selCGCBank.empty();
                            dis.selCGCBank.append(new Option('Please Select', ''))
                            if (vRows.length > 0) {
                                for (var i = 0; i < vRows.length; i++) {
                                    dis.selCGCBank.append(new Option(vRows[i].BankName, vRows[i].ID))
                                }
                            }
                        }
                    })

                    this.selCGCDateYear.empty();
                    this.selCGCDateYear.append(new Option('Year', ''))
                    for (var i = new Date().getFullYear() ; i >= 2000; i--) {
                        this.selCGCDateYear.append(new Option(i, i))
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
                            if (dis[13] != "Remove") {
                                listCGC.append($('<li>')
                                    .data(dis)
                                    .append($('<div>').html(dis[7] + '<br>'))
                                    .append($('<div>').html('Amount: ' + (dis[11] == 0 ? '0.00' : AccountingFormat(dis[11]))))
                                )
                                isEmpty = false; // set to false when there's no remove state in arrCGC, found in index 13, w/c means add, edit, remove
                            }
                        }

                        listCGC.find('li').first().addClass('navList-a');
                    }

                    if (isEmpty) {
                        listCGC.append($('<li>').text('List is empty'));
                    }

                    $('#pageListOfCGC').pageshow();
                    PopupReposition();
                    this.TotalCGC();
                },
                TotalCGC: function () {
                    if (this.arrCGC.length > 0) {
                        this.totalcgc = 0;
                        for (var i = 0; i < this.arrCGC.length; i++) {
                            if (this.arrCGC[i][13] != "Remove") {
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
                selDiscountType: null,
                txtDiscountPercent: null,
                txtDiscountAmount: null,
                txtDiscountRemarks: null,
                txtSalesPayNoOfSC: null,
                txtSalesPayNoOfDiners: null,
                txtDiscountPercentCeiling: null,
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
                init: function (params) {
                    this.form = $('#formFMCGDiscounts');
                    this.selDiscountType = this.form.find('[name="selDiscountType"]');
                    this.txtDiscountPercent = this.form.find('[name="txtDiscountPercent"]');
                    this.txtDiscountAmount = this.form.find('[name="txtDiscountAmount"]');
                    this.txtDiscountRemarks = this.form.find('[name="txtDiscountRemarks"]');
                    this.txtSalesPayNoOfSC = this.form.find('[name="txtSalesPayNoOfSC"]');
                    this.txtSalesPayNoOfDiners = this.form.find('[name="txtSalesPayNoOfDiners"]');
                    this.txtDiscountPercentCeiling = this.form.find('[name="txtDiscountPercentCeiling"]');
                    this.popup = $('#popFMCGDiscount');

                    this.CacPopup.popup('open');

                    this.selDiscountType.val('').focus();
                    this.txtDiscountPercent.val('');
                    this.txtDiscountAmount.val('');
                    this.txtDiscountRemarks.val('');

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

                    this.selDiscountType.keydown(function (e) {
                        if (e.which == 13) {
                            CacPopup.Discount.GetSelectedDiscount($.trim($(this).val()));
                            CacPopup.Discount.GetDiscountOnChange();
                            PopupReposition(); // NewMessageBox.js
                        }
                    })

                    this.form.unbind('submit').submit(function (e) {
                        e.preventDefault();
                        if (CacPopup.Discount.selDiscountType.val() == '') {
                            console.log('Please select discount type');
                            return;
                        }
                        else if (CacPopup.Discount.txtDiscountPercent.val() == '') {
                            console.log('Please enter percent discount');
                            return;
                        }
                        else if (CacPopup.Discount.txtDiscountAmount.val() == '') {
                            console.log('Please enter discount amount');
                            return;
                        }
                        else {
                            params.success({
                                DiscountType: CacPopup.Discount.selDiscountType.val(),
                                Percent: CDec2(CacPopup.Discount.txtDiscountPercent.val()),
                                Amount: CDec2(CacPopup.Discount.txtDiscountAmount.val()),
                                Remarks: CacPopup.Discount.txtDiscountRemarks.val()
                            })
                            this.CacPopup.popup('close');
                        }
                    })

                    this.txtDiscountPercent.keydown(function (e) {
                        if (e.which == 107) {
                            // press + in numpad
                        }
                    })
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
                        //proj.RefreshTotalDiscount();

                    } else {
                        window.pageSecuritySourcePage = $.mobile.activePage;
                        PromptUserAccess(this.strAccessType, {
                            OnClickOKSuccess: function () {

                                //$.mobile.changePage(window.pageSecuritySourcePage);

                                CacPopup.Discount.GetOrgDiscountPercentRefresh(CacPopup.Discount.selDiscountType.val());

                                //                                proj.RefreshTotalDiscount();

                            },
                            OnFailed: function () {
                                //$.mobile.changePage(window.pageSecuritySourcePage);
                                $('#selDiscountType').val('').selectmenu('refresh');
                                $("#txtDiscountPercent").val("0.00");
                                $("#txtDiscountAmount").val("0.00");
                                CacPopup.Discount.GetOrgDiscountPercentRefresh(CacPopup.Discount.selDiscountType.val());

                                //proj.RefreshTotalDiscount();
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
                    if (OfflineMode) {
                        var vRows = SalesPayment_offline().getOrgDiscountPercentRefresh(strDiscountID).Rows[0];

                        if (vRows != undefined) {
                            this.LoadOrgDiscountPercentRefresh(vRows);
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
                },
                LoadOrgDiscountPercentRefresh: function (vRows) {
                    this.txtDiscountPercentCeiling.val((vRows.PercentDiscount == "0" ? '' : AccountingFormat(vRows.PercentDiscount)))
                    this.txtDiscountPercent.val((vRows.PercentDiscount == "0" ? '' : AccountingFormat(vRows.PercentDiscount)))
                    this.longDiscountPercent = vRows.PercentDiscount;
                },
                RefreshTotalDiscount: function () {
                    var dblDiscount = 0;
                    var dblTotalAmount = 0;

                    if ($('#selDiscountType').val() == '') {

                        $('#rowVATPercent').hide();
                        $('#rowAmountLessVAT').hide();

                        $('#lblVATAmount').text('');
                        $('#lblTotalAmount').text('');

                        this.txtDiscountPercentCeiling.val('');
                        this.txtDiscountPercent.val('');
                        this.txtDiscountAmount.val('');

                    } else {
                        dblTotalAmount = this.isSeniorCitizenDiscount ? CDec2(this.dblSubTotal) / (1 + (CDec2(this.taxPercent) / 100)) : CDec2(this.dblSubTotal);

                        //dblDiscount = parseFloat(CDec2($('#lblSalesPaySubTotal').text().replace(/,/g, ""))) * (parseFloat(CDec2($('#txtDiscountPercent').val().replace(/,/g, "")))/100);
                        //dblDiscount = Math.round(parseFloat(CDec2($('#lblSalesPaySubTotal').text().replace(/,/g, ""))) * (parseFloat(CDec2($('#txtDiscountPercent').val().replace(/,/g, ""))) / 100) / 0.05) * 0.05;


                        if (this.isSeniorCitizenDiscount) {

                            $('#rowVATPercent').show();
                            $('#rowAmountLessVAT').show();

                            $('#lblVATPercentValue').text(taxPercent);

                            //                        vatSale = vData.Rows[0].TotalAmountDue / (1 + (vData.Rows[0].TaxPercent / 100));
                            $('#lblVATPercent').text(AccountingFormat(CDec2(this.dblSubTotal) - CDec2(dblTotalAmount)));
                            $('#lblTotalAmount').text(AccountingFormat(dblTotalAmount));
                            this.txtDiscountAmount.val((CDec2(this.longDiscountPercent) == "0" ? '' : AccountingFormat(CDec2($('#txtTotal').text().replace(/,/g, "")) * (CDec2(longDiscountPercent)) / 100)));
                        } else {

                            $('#rowVATPercent').hide();
                            $('#rowAmountLessVAT').hide();

                            $('#lblVATAmount').text('');
                            $('#lblTotalAmount').text('');
                            this.txtDiscountAmount.val((CDec2(this.longDiscountPercent) == "0" ? '' : AccountingFormat(CDec2($('#txtTotal').text().replace(/,/g, "")) * (CDec2(this.longDiscountPercent)) / 100)));

                        }
                    }
                    dblDiscount = this.txtDiscountAmount.val();  //(dblTotalAmount * (parseFloat(CDec2($('#txtDiscountPercent').val().replace(/,/g, ""))) / 100) / 0.05) * 0.05;
                    //RefreshSalesPaymentTotal_onchange();

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
                    var vRows = SalesPayment_offline().getOrgDiscountType().Rows;

                    this.selDiscountType.empty();

                    this.selDiscountType.append(new Option('Please Select', ''));
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        if (CDec2(vRows[p].PercentDiscount) != 0) {
                            this.selDiscountType.append(new Option(vRows[p].DiscountType, vRows[p].ID, false, false));
                        }
                    }
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
                    this.form = $('#formFMCGOtherPayment');
                    this.txtOPID = this.form.find('[name="txtOPID"]');
                    this.selOPPaymentType = this.form.find('[name="selOPPaymentType"]');
                    this.txtOPAvailablePts = this.form.find('[name="txtOPAvailablePts"]');
                    this.txtOPIssuingOrg = this.form.find('[name="txtOPIssuingOrg"]');
                    this.txtOPCardHolder = this.form.find('[name="txtOPCardHolder"]');
                    this.txtOPCardNo = this.form.find('[name="txtOPCardNo"]');
                    this.txtOPApprovalNo = this.form.find('[name="txtOPApprovalNo"]');
                    this.txtOPAmount = this.form.find('[name="txtOPAmount"]');
                    this.selOPIssuanceDateMonth = this.form.find('[name="selOPIssuanceDateMonth"]');
                    this.selOPIssuanceDateDay = this.form.find('[name="selOPIssuanceDateDay"]');
                    this.selOPIssuanceDateYear = this.form.find('[name="selOPIssuanceDateYear"]');
                    this.selOPExpirationDateMonth = this.form.find('[name="selOPExpirationDateMonth"]');
                    this.selOPExpirationDateDay = this.form.find('[name="selOPExpirationDateDay"]');
                    this.selOPExpirationDateYear = this.form.find('[name="selOPExpirationDateYear"]');
                    CacPopup.params = param;

                    $('#popFMCGOtherPayment').popup('open');
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
                    BindKeypress('#formFMCGOtherPayment', this.selOPIssuanceDateMonth, false, 'select');
                    BindKeypress('#formFMCGOtherPayment', this.selOPIssuanceDateDay, false, 'select');
                    BindKeypress('#formFMCGOtherPayment', this.selOPIssuanceDateYear, false, 'select');
                    BindKeypress('#formFMCGOtherPayment', this.selOPExpirationDateMonth, false, 'select');
                    BindKeypress('#formFMCGOtherPayment', this.selOPExpirationDateDay, false, 'select');
                    BindKeypress('#formFMCGOtherPayment', this.selOPExpirationDateYear, false, 'select');

                    this.selOPPaymentType.change(function () {
                        if (CacPopup.OtherPayment.selOPPaymentType.val() == "V-Plus") {
                            $('#divAvailablePts').show();
                            CacPopup.OtherPayment.GetVPlusPoints();
                        }
                        else {
                            $('#divAvailablePts').hide();
                        }
                    })

                    $('#btnAddOP').unbind('click').click(function () {
                        CacPopup.OtherPayment.Add();
                    })

                    this.form.unbind('submit').submit(function (e) {
                        e.preventDefault();
                        var pData;

                        if ($.trim(CacPopup.OtherPayment.selOPPaymentType.val()) == "" || $.trim(CacPopup.OtherPayment.txtOPIssuingOrg.val()) == ""
                            || $.trim(CacPopup.OtherPayment.txtOPCardNo.val()) == "" || $.trim(CacPopup.OtherPayment.txtOPAmount.val()) == "") {
                            $('#txtOPMsgRequired').pageshow();
                            return;
                        }

                        if (CacPopup.OtherPayment.selOPPaymentType.val() == "V-Plus" && $.trim(CacPopup.OtherPayment.txtOPAvailablePts.val()) == "") {
                            $('#txtOPMsgRequired').pageshow();
                            return;
                        }

                        if ($.trim(CacPopup.OtherPayment.selOPIssuanceDateMonth.val()) != "" || $.trim(CacPopup.OtherPayment.selOPIssuanceDateDay.val()) != ""
                                || $.trim(CacPopup.OtherPayment.selOPIssuanceDateYear.val()) != "") {
                            if ($.trim(CacPopup.OtherPayment.selOPIssuanceDateMonth.val()) == "" || $.trim(CacPopup.OtherPayment.selOPIssuanceDateDay.val()) == ""
                                || $.trim(CacPopup.OtherPayment.selOPIssuanceDateYear.val()) == "") {
                                $('#txtOPMsgErrorIssuanceDate').pageshow();
                                return;
                            }
                        }

                        if ($.trim(CacPopup.OtherPayment.selOPExpirationDateMonth.val()) != "" || $.trim(CacPopup.OtherPayment.selOPExpirationDateDay.val()) != ""
                                || $.trim(CacPopup.OtherPayment.selOPExpirationDateYear.val()) != "") {
                            if ($.trim(CacPopup.OtherPayment.selOPExpirationDateMonth.val()) == "" || $.trim(CacPopup.OtherPayment.selOPExpirationDateDay.val()) == ""
                                || $.trim(CacPopup.OtherPayment.selOPExpirationDateYear.val()) == "") {
                                $('#txtOPMsgErrorExpirationDate').pageshow();
                                return;
                            }
                        }

                        $('#txtOPMsgRequired, #txtOPMsgErrorIssuanceDate, #txtOPMsgErrorExpirationDate, #txtOPMsgErrorVPlus').hide();

                        pData = [
                            CacPopup.OtherPayment.txtOPID.val(), // 0
                            CacPopup.OtherPayment.selOPPaymentType.val(), // 1
                            CacPopup.OtherPayment.txtOPIssuingOrg.val(), // 2
                            CacPopup.OtherPayment.txtOPCardHolder.val(), // 3
                            CacPopup.OtherPayment.txtOPCardNo.val(), // 4
                            CacPopup.OtherPayment.selOPIssuanceDateMonth.val(), // 5
                            CacPopup.OtherPayment.selOPIssuanceDateDay.val(), // 6
                            CacPopup.OtherPayment.selOPIssuanceDateYear.val(), // 7
                            CacPopup.OtherPayment.selOPExpirationDateMonth.val(), // 8
                            CacPopup.OtherPayment.selOPExpirationDateDay.val(), // 9
                            CacPopup.OtherPayment.selOPExpirationDateYear.val(), // 10
                            CacPopup.OtherPayment.txtOPApprovalNo.val(), // 11
                            CacPopup.OtherPayment.txtOPAmount.val(), // 12
                            CacPopup.OtherPayment.opState // 13
                        ]

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
                    })
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
                    this.selOPPaymentType.val('')
                    this.txtOPAvailablePts.val('')
                    this.txtOPIssuingOrg.val('')
                    this.txtOPCardNo.val('')
                    this.txtOPCardHolder.val('')
                    this.txtOPApprovalNo.val('')
                    this.txtOPAmount.val('')
                    this.selOPIssuanceDateMonth.val('')
                    this.selOPIssuanceDateDay.val('')
                    this.selOPIssuanceDateYear.val('')
                    this.selOPExpirationDateMonth.val('')
                    this.selOPExpirationDateDay.val('')
                    this.selOPExpirationDateYear.val('')
                    $('#pageNewOP').pageshow();
                    $('#divAvailablePts, #txtOPMsgRequired, #txtOPMsgErrorIssuanceDate, #txtOPMsgErrorExpirationDate, #txtOPMsgErrorVPlus').hide();
                    PopupReposition();
                    PopupReposition(); //NewMessageBox.js
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
                    this.selOPIssuanceDateMonth.val(data[5]);
                    this.selOPIssuanceDateDay.val(data[6]);
                    this.selOPIssuanceDateYear.val(data[7]);
                    this.selOPExpirationDateMonth.val(data[8]);
                    this.selOPExpirationDateDay.val(data[9]);
                    this.selOPExpirationDateYear.val(data[10]);
                    this.txtOPApprovalNo.val(data[11]);
                    this.txtOPAmount.val(data[12]);
                    this.srvState = data[13];
                    this.editIndex = data[14];

                    $('#pageNewOP').pageshow();
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
                    $('#pageListOfOP').pageshow();
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
                            dis.push(i); // add index # in array, use for editing
                            if (dis[13] != "Remove") {
                                listOP.append($('<li>')
                                    .data(dis)
                                    .append($('<div>').html('<b>Card No: ' + dis[4] + '</b>'))
                                    .append($('<div>').html('Amount: ' + (dis[12] == 0 ? '0.00' : AccountingFormat(dis[12]))))
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
                    $('#popFMCGCash').popup('open');
                    $('[name="txtCashAmount"]').focus();
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
                        $('#popFMCGCash').popup('close');
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
                    this.selCardIssuanceDateMonth = this.form.find('[name="selCardIssuanceDateMonth"]');
                    this.selCardIssuanceDateDay = this.form.find('[name="selCardIssuanceDateDay"]');
                    this.selCardIssuanceDateYear = this.form.find('[name="selCardIssuanceDateYear"]');
                    this.selCardExpirationDateMonth = this.form.find('[name="selCardExpirationDateMonth"]');
                    this.selCardExpirationDateDay = this.form.find('[name="selCardExpirationDateDay"]');
                    this.selCardExpirationDateYear = this.form.find('[name="selCardExpirationDateYear"]');
                    this.txtCardApprovalNo = this.form.find('[name="txtCardApprovalNo"]');
                    this.txtCardInstallmentNo = this.form.find('[name="txtCardInstallmentNo"]');
                    this.txtCardAmount = this.form.find('[name="txtCardAmount"]');
                    CacPopup.params = param;

                    $('#popFMCGCard').popup('open');
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

                        if ($.trim(CacPopup.Card.selCardIssuingBank.val()) == "" || $.trim(CacPopup.Card.txtCardLname.val()) == ""
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

                            pData = [
                                    CacPopup.Card.txtCardID.val(), // 0
                                    (cardtype == "Debit" ? "" : CacPopup.Card.selCreditCardType.val()), // 1
                                    CacPopup.Card.selCardIssuingBank.val(), // 2
                                    CacPopup.Card.selCardIssuingBank.find('option:selected').text(), // 3
                                    CacPopup.Card.txtCardLname.val(), // 4
                                    CacPopup.Card.txtCardFname.val(), // 5
                                    CacPopup.Card.txtCardMname.val(), // 6                                    
                                    CacPopup.Card.selCardGenSuffix.val(), // 7
                                    CacPopup.Card.selCardBankLocation.val(), // 8
                                    CacPopup.Card.selCardBankLocation.find('option:selected').text(), // 9
                                    CacPopup.Card.txtCardNo.val(), // 10
                                    CacPopup.Card.selCardIssuanceDateMonth.val(), // 11
                                    CacPopup.Card.selCardIssuanceDateDay.val(), // 12
                                    CacPopup.Card.selCardIssuanceDateYear.val(), // 13
                                    CacPopup.Card.selCardExpirationDateMonth.val(), // 14
                                    CacPopup.Card.selCardExpirationDateDay.val(), // 15
                                    CacPopup.Card.selCardExpirationDateYear.val(), // 16
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
                                listCard.append($('<li>')
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
                                listCard.append($('<li>')
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
                        listCard.append($('<li>').text('List is empty'))
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
                    this.form = $('#formFMCGSrv');
                    this.txtSrvID = this.form.find('[name="txtSrvID"]');
                    this.txtSrvTransMemoID = this.form.find('[name="txtSrvTransMemoID"]');
                    this.txtSrvNo = this.form.find('[name="txtSrvNo"]');
                    this.txtSrvPinCode = this.form.find('[name="txtSrvPinCode"]');
                    this.txtMainSrvAmount = this.form.find('[name="txtMainSrvAmount"]');
                    this.txtSrvAmount = this.form.find('[name="txtSrvAmount"]');
                    CacPopup.params = param;

                    $('#popFMCGSrv').popup('open');
                    CacPopup.popupname = 'srv';

                    this.Main();

                    BindKeypress('#formFMCGSrv', this.txtSrvNo, true, 'text');
                    BindKeypress('#formFMCGSrv', this.txtSrvPinCode, false, 'text');

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
                        ]

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

                                    if (CDec2($("#txtBalChange").text()) <= CDec2(vData.SRVAmount)) {
                                        CacPopup.Srv.txtSrvAmount.val($("#txtBalChange").text());
                                    } else {
                                        CacPopup.Srv.txtSrvAmount.val(AccountingFormat(vData.SRVAmount) || '0.00');
                                    }

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
                                listSrv.append($('<li>')
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
                        listSrv.append($('<li>').text('List is empty'));
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
                    this.form = $('#formFMCGCharge');
                    this.selChargeType = this.form.find('[name="selChargeType"]');
                    this.txtChargePercentage = this.form.find('[name="txtChargePercentage"]');
                    this.txtChargeAmount = this.form.find('[name="txtChargeAmount"]');
                    this.txtChargeRemarks = this.form.find('[name="txtChargeRemarks"]');
                    CacPopup.params = param;

                    $('#popFMCGCharges').popup('open');
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

                            if (SalesPayment_offline().SaveBillingCharges(pData) == 'ok') {
                                // to be fixed
                                CacPopup.Charges.GetCurrentCharges();
                                CacPopup.RefreshSalesPaymentTotal();
                                CacPopup.ComputePayment();
                                CacPopup.Misc.TotalMisc();
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
            }
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
        else if (e.which == 109) {
            // pressing - (means go back to mainpage)
            if (CacPopup.popupname == "cgc") {
                //Cheque and Gift Certicate
                CacPopup.ChequeGC.Main();
            }
            else if (CacPopup.popupname == "card") {
                //Cheque and Gift Certicate
                CacPopup.Card.Main();
            }
            else if (CacPopup.popupname == "srv") {
                //SRV
                CacPopup.Srv.Main();
            }
            else if (CacPopup.popupname == "op") {
                //Other Payment
                CacPopup.OtherPayment.Main();
            }
            else if (CacPopup.popupname == "charges") {
                //Charges and Other Tax
                CacPopup.Charges.Main();
            }
            else if (CacPopup.popupname == "soldto") {
                //Charges and Other Tax
                CacPopup.SoldTo.Main();
            }
        }
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
    })
})