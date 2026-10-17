require([
    'common',
    GetPathFormat('/Scripts3/Browser/LibraryPeople.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryOrganization.js?crc='),
    '/Scripts/sources/SalesPayments/SalesPayments_offline.js?crc=',
    '/Scripts/sources/ReportsPOS/SalesReport_offline.js?crc=',
    'promise!/Scripts3/Offline/Browser/LibraryFowst.js?crc=',
    'promise!/Scripts3/NewMessageBox.js?crc=' + assetversion
], function () {

    //metro.min.js needs jquery to load first before it can load itself

    require([
            '/Scripts/jquery.widget.min.js'
    ],
        function () {
            require([
                    '/Scripts/metro.min.js'
            ],
                function () {
                    //metro-calendar.js, metro-datepicker.js needs jquery.widget.min.js andmetro.min.js  to load first before it can load itself
                    require([
                            '/Scripts/metro-calendar.js',
                            '/Scripts/metro-datepicker.js'
                    ],
                    function () {

                        var CounterCashierSalesReport = true;
                        var cashiermanagerpage = "";
                        var IsBank = false;
                        var IsEmployee = false;
                        var vCompanyName = '';
                        var IsTotalAmount = true;
                        var IsThirdPartyCollector = true;
                        var vDataHeader
                        var TotalAmount = 0;
                        var DepositToID;
                        var IsEmployeeVerified = false;
                        var IsEmployee = false;
                        var arrOtherPayment = [];
                        var rowsOP;
                        var isRequiredEmpCredentials = true;

                        $(function () {

                            $('#txtDateFrom').DateCombo('val', new Date());
                            $('#txtDateTo').DateCombo('val', new Date());

                            GetCashierSalesReportPage();
                            startclock('#clockTurnover');

                            var pPage = $.mobile.activePage;
                            var pHeader = pPage.find("[data-role=header]");
                            var pHeight = pHeader.height() + 10;

                            //pPage.css('padding-top', pHeight + 'px');

                            if ($.isFunction(pHeader.fixedtoolbar)) // jqm 1.4.4 doesn't have fixedtoolbar function
                                pHeader.fixedtoolbar({ tapToggle: false,
                                    updatePagePadding: false
                                });
                            App.Deferred.fulfill('R87MBPXGSQ');

                            try {
                                if (CStr2(sessionStorage.FowstID) == '') {
                                    setTimeout(function () {
                                        BrowsePopup('LibraryFowst', 'Select Store branch', [
                                        {
                                            DisplayText: [{ Column: 'FowstName' }],
                                            Offline: true,
                                            AutoSearch: true,
                                            WithSearchBox: false,
                                            Click: function (e) {
                                                sessionStorage.SelectedFOWST = e.ID;
                                                sessionStorage.FowstID = e.ID;
                                                GetHeaderData();
                                            }
                                        }
                                        ], {
                                            FilterFowst: '1'
                                        })
                                    }, 400)
                                } else {
                                    GetHeaderData();
                                }
                            }
                            catch (e) {
                                e.message;
                            }

                            if (vCompanyName != '') {
                                $('#txtDepositBy').val($.trim(sessionStorage.CurrentUserFullName) + ' of ' + vCompanyName);
                            } else {
                                $('#txtDepositBy').val($.trim(sessionStorage.CurrentUserFullName));
                            }

                            if (GetUrlParameters().type == 'cashcountonly') {
                                $('[name="radTurnOverOfMonies"][value="2"]').prop('checked', 'checked');
                                $('#dvTurnoverDeposit, #dvDepositTo, #divForBank, #dvFreeType').hide();
                            } else {
                                $('[name="radTurnOverOfMonies"][value="1"]').prop('checked', 'checked');
                            }

                            try {
                                var pSqlOP = 'SELECT * FROM LibraryOtherPayments ORDER BY Description';
                                rowsOP = Sql.Execute(pSqlOP).Rows, selOtherPaymentType = $('#selOtherPayment');

                                selOtherPaymentType.empty();
                                selOtherPaymentType.append(new Option('Please select', ''));

                                for (var i = 0; i < rowsOP.length; i++) {
                                    selOtherPaymentType.append(new Option(rowsOP[i].Description, rowsOP[i].Value));
                                }
                            } catch (e) {
                                Android.AddLog(e.message);
                                //do nothing -- meaning walay table LibraryOtherPayments ang current database. dili pud sya stopper sa paggamit sa POS
                            }

                            isRequiredEmpCredentials = CBool(Business.SystemDefaults.GetSystemDefault('RequiredCredentialsTurnoverMonies', 1));
                            //$('[name="radTurnOverOfMonies"]').checkboxradio('refresh')

                            $('#btnFOWST').click(function () {
                                $('#panCashierFowst').popup('open');
                            });

                            $('#btnReportDate').click(function () {
                                $('#panCashierDate').popup('open');
                            });

                            $('#txtDepositTo').keypress(function (e) {
                                if (e.keyCode == 13) {
                                    switch ($('#optDepositTo').val()) {
                                        case 
                                            'Bank Teller': GetOrganizations();
                                            break;
                                        case 
                                            '3rd Party Courier': GetOrganizations();
                                            break;
                                        case 
                                            '3rd Party Collector': GetOrganizations();
                                            break;
                                        case 
                                            'My Company Collector': GetEmployees();
                                            break;
                                        case 
                                            'My Company Manager': GetEmployees();
                                            break;
                                        case 
                                            'My Company Cashier': GetEmployees();
                                            break;

                                    }
                                }
                            });

                            $('#optDepositTo').change(function () {
                                $('#txtDepositTo').val('');
                                $('#txtFreeType').val('');
                                $('#txtToValidation').hide();
                                GetDepositTo();
                            });

                            $('#btnSaveDepositMonies').click(function () {
                                ComputeTotalAmount();

                                if (CDec2(TotalAmount) == 0) {
                                    MessageBox('Please put amount to turnover.');
                                    return;
                                }

                                var isvalid = true, inp;

                                $('#dvDetailedBreakdown .amtturnover').each(function () {
                                    if (!Number.isInteger(CDec2($(this).val())) && IsNumeric(CDec2($(this).val()))) {
                                        inp = $(this);
                                        isvalid = false;
                                        return false;
                                    }
                                })

                                if (!isvalid) {
                                    NewMessageBox('Decimal value is not allowed.', 'Denomination Quantity', undefined, {
                                        Close: function () {
                                            inp.focus();
                                        }
                                    });
                                    return;
                                }

                                if (CStr2(sessionStorage.FowstID) == '') {
                                    BrowsePopup('LibraryFowst', 'Select Store branch', [
                                        {
                                            DisplayText: [{ Column: 'FowstName' }],
                                            Offline: true,
                                            AutoSearch: true,
                                            WithSearchBox: false,
                                            Click: function (e) {
                                                sessionStorage.SelectedFOWST = e.ID;
                                                sessionStorage.FowstID = e.ID;
                                                SaveDeposit();
                                            }
                                        }
                                    ], {
                                        FilterFowst: '1'
                                    })
                                } else {
                                    SaveDeposit();
                                }                                
                            });

                            $('#dvDetailedBreakdown .amtturnover').blur(function () {
                                var dis = $(this);
                                if (!Number.isInteger(CDec2(dis.val())) && IsNumeric(dis.val())) {
                                    NewMessageBox('Decimal value is not allowed.', 'Denomination Quantity', undefined, {
                                        Close: function () {
                                            dis.focus();
                                        }
                                    });
                                }
                            })

                            $('#btnGoBackToMain').click(function () {
                                var pParam = {};
                                pParam.IsMobile = 1;
                                pParam = $.param(pParam);
                                window.location = "/People/PeopleMenu?" + pParam;
                            });

                            $('#btnGoBackToCashHandling').click(function () {
                                var pParam = {};
                                pParam.IsMobile = 1;
                                pParam.OrgID = sessionStorage.OrgID;
                                pParam = $.param(pParam);
                                window.location = "/CashHandling/MoniesForDepositOrTurnOver?" + pParam;
                            });

                            $('#rdDetailedBreakdown').click(function () {
                                $('#dvDetailedBreakdown').css('visibility', 'visible');
                                $('#dvDetailedBreakdown').css('display', 'block');

                                $('#txtTotalAmount').attr('readonly', '');
                                $('#txtTotalAmount').css('display', 'none');

                                IsTotalAmount = false;

                                ComputeTotalDeposit();
                            });

                            $('#rdTotalAmount').click(function () {
                                $('#dvDetailedBreakdown').css('visibility', 'hidden');
                                $('#dvDetailedBreakdown').css('display', 'none');

                                $('#txtTotalAmount').css('display', 'block');
                                $('#txtTotalAmount').removeAttr('readonly', '');

                                IsTotalAmount = true;
                            });

                            $('#txtNoOfOneCents').change(function () {
                                ComputeTotalDeposit();
                            });
                            
                            $('#txtNoOfFiveCents').change(function () {
                                ComputeTotalDeposit();
                            });

                            $('#txtNoOfTenCents').change(function () {
                                ComputeTotalDeposit();
                            });

                            $('#txtNoOfTwentyFiveCents').change(function () {
                                ComputeTotalDeposit();
                            });

                            $('#txtNoOfOnePeso').change(function () {
                                ComputeTotalDeposit();
                            });

                            $('#txtNoOfFivePesos').change(function () {
                                ComputeTotalDeposit();

                            });

                            $('#txtNoOfTenPesos').change(function () {
                                ComputeTotalDeposit();
                            });

                            $('#txtNoOfTwentyPesos').change(function () {
                                ComputeTotalDeposit();
                            });

                            $('#txtNoOfFiftyPesos').change(function () {
                                ComputeTotalDeposit();
                            });

                            $('#txtNoOfOneHundredPesos').change(function () {
                                ComputeTotalDeposit();
                            });

                            $('#txtNoOfTwoHundredPesos').change(function () {
                                ComputeTotalDeposit();
                            });

                            $('#txtNoOfFiveHundredPesos').change(function () {
                                ComputeTotalDeposit();
                            });

                            $('#txtNoOfOneThousandPesos').change(function () {
                                ComputeTotalDeposit();
                            });

                            $('#rdDeposit').click(function () {
                                $('#optDepositTo').empty();
                                $('#optDepositTo').append('<option>-Select-</option><option>Bank Teller</option><option>3rd Party Courier</option><option>3rd Party Collector</option>');
                                $('#optDepositTo option:eq(0)').attr('selected', 'selected');
                                //$('#optDepositTo').selectmenu('refresh');
                                //$('#dvDepositTo').css('visibility', 'hidden');
                                $('#dvDepositTo').css('display', 'none');
                                $('#dvTurnoverDeposit').show();
                                DepositToID = '';
                            });

                            $('#rdTurnOver').click(function () {
                                $('#optDepositTo').empty();
                                $('#optDepositTo').append('<option>-Select-</option><option>My Company Collector</option><option>My Company Manager</option><option>My Company Cashier</option>');
                                $('#optDepositTo option:eq(0)').attr('selected', 'selected');
                                //$('#optDepositTo').selectmenu('refresh');
                                //$('#dvDepositTo').css('visibility', 'hidden');
                                $('#dvDepositTo').css('display', 'none');
                                $('#dvTurnoverDeposit').show();
                                DepositToID = '';
                            });

                            $('#rdCashCount').click(function () {
                                $('#dvTurnoverDeposit, #dvDepositTo, #divForBank, #dvFreeType').hide();
                            });

                            $('#btnVerifyEmployee').click(function () {
                                pSql = 'SELECT * FROM LibraryPeopleUserAccount WHERE PeopleID = ' + ReadyString(DepositToID);
                                pDT = Sql.Execute(pSql);


                                if (pDT.Error) {
                                    IsEmployeeVerified = false;
                                    $('#dvInvalidUsernameOrPassword').css('visibility', 'visible');
                                    $('#dvInvalidUsernameOrPassword').css('display', 'block');
                                };
                                if (pDT.Rows.length == 0) {
                                    IsEmployeeVerified = false;
                                    $('#dvInvalidUsernameOrPassword').css('visibility', 'visible');
                                    $('#dvInvalidUsernameOrPassword').css('display', 'block');
                                } else if (CBool(Android.VerifyHash($('#txtPassword').val(), pDT.Rows[0].UserPassword))) {
                                    IsEmployeeVerified = true;
                                    sessionStorage.TOUserName = $('#txtUsername').val();
                                    sessionStorage.TOPassword = $('#txtPassword').val();
                                    sessionStorage.IsFromTurnOverOfMoniesPage = true;
                                    $('#btnSaveDepositMonies').click();
                                }
                                else {
                                    IsEmployeeVerified = false;
                                    $('#dvInvalidUsernameOrPassword').css('visibility', 'visible');
                                    $('#dvInvalidUsernameOrPassword').css('display', 'block');
                                }
                            });

                            $('#txtServiceCharge').keydown(function (e) {
                                if (!NumberOnly(e)) {
                                    return;
                                }
                            })

                            $('#btnAddOtherPayment').click(function () {
                                $('#popupOtherPayment').modal('show');
                                $('#selOtherPayment').val('');
                                $('#txtOPTotalAmount').val('');
                            })

                            $('#btnSaveOP').click(function () {
                                var selOtherPayment = $('#selOtherPayment').val().replace(/\s+/g, '');
                                var txtOPTotalAmount = $('#txtOPTotalAmount').val();
                                var index = null;

                                if (selOtherPayment == '') {
                                    WebToast('Please select Other Payment', 'top');
                                    return;
                                }

                                if (txtOPTotalAmount == '') {
                                    WebToast('Please enter total amount of selected Other Payment', 'top');
                                    return;
                                }

                                $.grep(arrOtherPayment, function (e, i) {
                                    if (e.ID == selOtherPayment) {
                                        index = i;
                                    }
                                })

                                if (index == null) {
                                    arrOtherPayment.push({
                                        ID: selOtherPayment,
                                        Amount: txtOPTotalAmount
                                    })

                                    $('#divOtherPayment').append($('<div class="row mb-3">')
                                            .append($('<label class="col-md-2">').attr('for', 'txtTotal' + selOtherPayment).text('Total Amount of ' + $('#selOtherPayment option:selected').text() + ' to Turnover:'))
                                            .append($('<div class="col-md-10">')
                                                .append($('<input type="number" class="amtturnover form-control" id="txtTotal' + selOtherPayment + '">').val(txtOPTotalAmount))
                                            )
                                        );
                                } else {
                                    arrOtherPayment[index].Amount = txtOPTotalAmount;
                                    $('#txtTotal' + selOtherPayment).val(txtOPTotalAmount);
                                }

                                //check if column exist in cachemsr table
                                var vData = Sql.Execute("PRAGMA table_info(TransCashHandling)");
                                var IsExist = false;

                                IsOtherPaymentType = true;

                                for (var i = 0; i <= vData.Rows.length - 1; i++) {
                                    if (vData.Rows[i].name == "Total" + selOtherPayment) {
                                        IsExist = true;
                                        break;
                                    }
                                }

                                if (!IsExist) {
                                    Sql.Execute("ALTER TABLE TransCashHandling ADD COLUMN Total" + selOtherPayment + " INTEGER");
                                }

                                $('#popupOtherPayment').modal('hide');
                            })
                        });

                        function GetHeaderData() {
                            vDataHeader = SalesReport_offline().getReportHeader(0);

                            vDataHeader = vDataHeader.Rows[0];

                            //vDataHeader = JSON.parse(sessionStorage.ReportHeader);

                            vCompanyName = vDataHeader.OrgBusinessTradeName;

                            $('#lblOrganizationBusinessName2').text($.trim((vDataHeader.OrgBusinessTradeName || '')))
                            $('#lblOrganizationName2').text($.trim((vDataHeader.SubscriberOrganizationName || '')))
                            $('#lblOrganizationFOWST2').text($.trim((vDataHeader.OrgBranchName || '')))
                            $('#lblOrganizationFOWSTAddress2').text($.trim((vDataHeader.OrgCompleteAddress || '')))
                            $('#lblOrganizationFOWSTTIN2').text('TIN ' + $.trim((vDataHeader.OrgTIN || '')))
                            $('#lblOrganizationFOWSTContactNumber2').text('Tel. No. ' + $.trim((vDataHeader.OrgTelNo || '')))
                        }

                        function ComputeTotalAmount() {
                            TotalAmount = CDec2($('#txtTotalAmount').val() || 0) + CDec2($('#txtTotalCheck').val() || 0) +
                                            CDec2($('#txtTotalCreditCard').val() || 0) + CDec2($('#txtTotalDebitCard').val() || 0) +
                                            CDec2($('#txtTotalSRV').val() || 0) + CDec2($('#txtTotalPettyCash').val() || 0);

                            $.grep(arrOtherPayment, function (e) {
                                TotalAmount += CDec2($('#txtTotal' + e.ID).val());
                            })
                        }

                        function GetDepositTo() {
                            switch ($('#optDepositTo').val()) {
                                case 
                                        'Bank Teller': //GetOrganizations();
                                    $('#txtDepositTo').attr('placeholder', 'Search Bank');
                                    $('#lblDepositTo').text('Bank Name: ');
                                    $('#lblFreeType').text('Teller Name: ');
                                    $('#divForBank').hide();
                                    $('#txtBankBranch, #txtDepositRefNo, #txtServiceCharge').val('');
                                    IsThirdPartyCollector = true;
                                    break;
                                case 
                                    '3rd Party Courier': //GetOrganizations();
                                    $('#txtDepositTo').attr('placeholder', 'Search Organization');
                                    $('#lblDepositTo').text('Company Name: ');
                                    $('#lblFreeType').text('Messenger\'s Name: ');
                                    $('#divForBank').hide();
                                    $('#txtBankBranch, #txtDepositRefNo, #txtServiceCharge').val('');
                                    IsThirdPartyCollector = true;
                                    break;
                                case 
                                    '3rd Party Collector': //GetOrganizations();
                                    $('#txtDepositTo').attr('placeholder', 'Search Organization');
                                    $('#lblDepositTo').text('Company Name: ');
                                    $('#lblFreeType').text('Messenger\'s Name: ');
                                    $('#divForBank').hide();
                                    $('#txtBankBranch, #txtDepositRefNo, #txtServiceCharge').val('');
                                    IsThirdPartyCollector = true;
                                    break;
                                case 
                                    'My Company Collector': //GetEmployees();
                                    $('#txtDepositTo').attr('placeholder', 'Search Employee');
                                    $('#lblDepositTo').text('Employee Name: ');
                                    $('#dvFreeType').css('visibility', 'hidden');
                                    $('#dvFreeType').css('display', 'none');
                                    $('#divForBank').hide();
                                    $('#txtBankBranch, #txtDepositRefNo, #txtServiceCharge').val('');
                                    IsThirdPartyCollector = false;
                                    break;
                                case 
                                    'My Company Manager': //GetEmployees();
                                    $('#txtDepositTo').attr('placeholder', 'Search Employee');
                                    $('#lblDepositTo').text('Employee Name: ');
                                    $('#dvFreeType').css('visibility', 'hidden');
                                    $('#dvFreeType').css('display', 'none');
                                    $('#divForBank').hide();
                                    $('#txtBankBranch, #txtDepositRefNo, #txtServiceCharge').val('');
                                    IsThirdPartyCollector = false;
                                    break;
                                case 
                                    'My Company Cashier': //GetEmployees();
                                    $('#txtDepositTo').attr('placeholder', 'Search Employee');
                                    $('#lblDepositTo').text('Employee Name: ');
                                    $('#dvFreeType').css('visibility', 'hidden');
                                    $('#dvFreeType').css('display', 'none');
                                    $('#divForBank').hide();
                                    $('#txtBankBranch, #txtDepositRefNo, #txtServiceCharge').val('');
                                    IsThirdPartyCollector = false;
                                    break;

                                default:
                                    $('#lblDepositTo').text('To: ');
                                    $('#dvFreeType').css('visibility', 'visible');
                                    $('#txtBankBranch, #txtDepositRefNo, #txtServiceCharge').val('');
                                    IsThirdPartyCollector = true;
                            }

                            //$('#dvDepositTo').css('visibility', 'visible');
                            $('#dvDepositTo').css('display', 'block');
                        }

                        function ComputeTotalDeposit() {
                            var mTotal = 0;

                            if (IsNumeric($('#txtNoOfOneCents').val())) {
                                mTotal += $('#txtNoOfOneCents').val() * 0.01;
                                $('#lblNoOfOneCents').text('PhP ' + AccountingFormat($('#txtNoOfOneCents').val() * 0.01));
                            }
                            
                            if (IsNumeric($('#txtNoOfFiveCents').val())) {
                                mTotal += $('#txtNoOfFiveCents').val() * 0.05;
                                $('#lblNoOfFiveCents').text('PhP ' + AccountingFormat($('#txtNoOfFiveCents').val() * 0.05));
                            }

                            if (IsNumeric($('#txtNoOfTenCents').val())) {
                                mTotal += $('#txtNoOfTenCents').val() * 0.1;
                                $('#lblNoOfTenCents').text('PhP ' + AccountingFormat($('#txtNoOfTenCents').val() * 0.1));
                            }

                            if (IsNumeric($('#txtNoOfTwentyFiveCents').val())) {
                                mTotal += $('#txtNoOfTwentyFiveCents').val() * 0.25;
                                $('#lblNoOfTwentyFiveCents').text('PhP ' + AccountingFormat($('#txtNoOfTwentyFiveCents').val() * 0.25));
                            }

                            if (IsNumeric($('#txtNoOfOnePeso').val())) {
                                mTotal += $('#txtNoOfOnePeso').val() * 1;
                                $('#lblNoOfOnePeso').text('PhP ' + AccountingFormat($('#txtNoOfOnePeso').val() * 1));
                            }

                            if (IsNumeric($('#txtNoOfFivePesos').val())) {
                                mTotal += $('#txtNoOfFivePesos').val() * 5;
                                $('#lblNoOfFivePesos').text('PhP ' + AccountingFormat($('#txtNoOfFivePesos').val() * 5));
                            }

                            if (IsNumeric($('#txtNoOfTenPesos').val())) {
                                mTotal += $('#txtNoOfTenPesos').val() * 10;
                                $('#lblNoOfTenPesos').text('PhP ' + AccountingFormat($('#txtNoOfTenPesos').val() * 10));
                            }

                            if (IsNumeric($('#txtNoOfTwentyPesos').val())) {
                                mTotal += $('#txtNoOfTwentyPesos').val() * 20;
                                $('#lblNoOfTwentyPesos').text('PhP ' + AccountingFormat($('#txtNoOfTwentyPesos').val() * 20));
                            }

                            if (IsNumeric($('#txtNoOfFiftyPesos').val())) {
                                mTotal += $('#txtNoOfFiftyPesos').val() * 50;
                                $('#lblNoOfFiftyPesos').text('PhP ' + AccountingFormat($('#txtNoOfFiftyPesos').val() * 50));
                            }

                            if (IsNumeric($('#txtNoOfOneHundredPesos').val())) {
                                mTotal += $('#txtNoOfOneHundredPesos').val() * 100;
                                $('#lblNoOfOneHundredPesos').text('PhP ' + AccountingFormat($('#txtNoOfOneHundredPesos').val() * 100));
                            }

                            if (IsNumeric($('#txtNoOfTwoHundredPesos').val())) {
                                mTotal += $('#txtNoOfTwoHundredPesos').val() * 200;
                                $('#lblNoOfTwoHundredPesos').text('PhP ' + AccountingFormat($('#txtNoOfTwoHundredPesos').val() * 200));
                            }

                            if (IsNumeric($('#txtNoOfFiveHundredPesos').val())) {
                                mTotal += $('#txtNoOfFiveHundredPesos').val() * 500;
                                $('#lblNoOfFiveHundredPesos').text('PhP ' + AccountingFormat($('#txtNoOfFiveHundredPesos').val() * 500));
                            }

                            if (IsNumeric($('#txtNoOfOneThousandPesos').val())) {
                                mTotal += $('#txtNoOfOneThousandPesos').val() * 1000;
                                $('#lblNoOfOneThousandPesos').text('PhP ' + AccountingFormat($('#txtNoOfOneThousandPesos').val() * 1000));
                            }

                            $('#txtTotalAmount').val(mTotal);
                            $('#lblTotamAmount').text('PhP ' + AccountingFormat(mTotal));
                        }

                        function SaveDeposit() {
                            var pIU;
                            var pSql;
                            var ID = NewID();

                            Business.ZRead.HandleEnforceZRead();

                            if ($('input[name="radTurnOverOfMonies"]:checked').val() == '2') {
                                //FOR CASH COUNT ONLY
                                SaveCashCount();
                                return;
                            }

                            if ($.trim($('#optDepositTo').val()) == "-Select-") {
                                NewMessageBox('Please fill in the required field(s)');
                                $('#txtToValidation').show();
                                return false;
                            }
                            else {
                                $('#txtToValidation').hide();
                            }

                            if ($.trim($('#txtDepositTo').val()) == "") {
                                MessageBox($('#lblDepositTo').text().replace(': ', '') + ' is required');
                                return false;
                            }

                            if (IsEmployee) {
                                if (!IsEmployeeVerified) {
                                    $('#lblReceiverName').text($('#txtDepositTo').val());
                                    $('#DialogUserAccess').modal('show');
                                    return;
                                }
                            }

                            try {
                                if (CStr2(DepositToID) == '' || $('#txtDepositTo').val() == '' || (!CBool($('input[name="radTurnOverOfMonies"]:checked').val()) && $('#txtFreeType').val() == '')) {
                                    MessageBox('Please enter \'Deposit to\' information');
                                    return;
                                }

                                //if (!IsDate($('#txtDateFrom').DateCombo('val')) || !IsDate($('#txtDateTo').DateCombo('val'))) {
                                //    MessageBox('Please put proper employee shift date and time.');
                                //    return;
                                //}

                                $('#lblTotalTurnedoverAmount').text(AccountingFormat(TotalAmount));
                                var divBreakdown = $('#divBreakdown').empty();
                                var cash = CDec2($('#txtTotalAmount').val());
                                var onecents = CDec2($('#txtNoOfOneCents').val());
                                var fivecents = CDec2($('#txtNoOfFiveCents').val());
                                var tencents = CDec2($('#txtNoOfTenCents').val());
                                var twentyfivecents = CDec2($('#txtNoOfTwentyFiveCents').val());
                                var one = CDec2($('#txtNoOfOnePeso').val());
                                var five = CDec2($('#txtNoOfFivePesos').val());
                                var ten = CDec2($('#txtNoOfTenPesos').val());
                                var twenty = CDec2($('#txtNoOfTwentyPesos').val());
                                var fifty = CDec2($('#txtNoOfFiftyPesos').val());
                                var onehundred = CDec2($('#txtNoOfOneHundredPesos').val());
                                var twohundred = CDec2($('#txtNoOfTwoHundredPesos').val());
                                var fivehundred = CDec2($('#txtNoOfFiveHundredPesos').val());
                                var onethousand = CDec2($('#txtNoOfOneThousandPesos').val());
                                var check = CDec2($('#txtTotalCheck').val());
                                var creditcard = CDec2($('#txtTotalCreditCard').val());
                                var debitcard = CDec2($('#txtTotalDebitCard').val());
                                var srv = CDec2($('#txtTotalSRV').val());

                                if (CBool(IsTotalAmount)) {
                                    if (cash > 0) divBreakdown.append($('<div class="row p-2 border align-items-center">').append($('<div class="col">').text('Cash')).append($('<div class="col text-right">').text(AccountingFormat(cash))));
                                } else {
                                    if (onecents > 0) divBreakdown.append('<div class="row p-2 border align-items-center"><div class="col">0.01</div><div class="col text-center">x' + onecents + '</div><div class="col text-right">' + AccountingFormat(onecents * 0.01) + '</div></div>');
                                    if (fivecents > 0) divBreakdown.append('<div class="row p-2 border align-items-center"><div class="col">0.05</div><div class="col text-center">x' + fivecents + '</div><div class="col text-right">' + AccountingFormat(fivecents * 0.05) + '</div></div>');
                                    if (tencents > 0) divBreakdown.append('<div class="row p-2 border align-items-center"><div class="col">0.10</div><div class="col text-center">x' + tencents + '</div><div class="col text-right">' + AccountingFormat(tencents * 0.10) + '</div></div>');
                                    if (twentyfivecents > 0) divBreakdown.append('<div class="row p-2 border align-items-center"><div class="col">0.25</div><div class="col text-center">x' + twentyfivecents + '</div><div class="col text-right">' + AccountingFormat(twentyfivecents * 0.25) + '</div></div>');
                                    if (one > 0) divBreakdown.append('<div class="row p-2 border align-items-center"><div class="col">1</div><div class="col text-center">x' + one + '</div><div class="col text-right">' + AccountingFormat(one * 1) + '</div></div>');
                                    if (five > 0) divBreakdown.append('<div class="row p-2 border align-items-center"><div class="col">5</div><div class="col text-center">x' + five + '</div><div class="col text-right">' + AccountingFormat(five * 5) + '</div></div>');
                                    if (ten > 0) divBreakdown.append('<div class="row p-2 border align-items-center"><div class="col">10</div><div class="col text-center">x' + ten + '</div><div class="col text-right">' + AccountingFormat(ten * 10) + '</div></div>');
                                    if (twenty > 0) divBreakdown.append('<div class="row p-2 border align-items-center"><div class="col">20</div><div class="col text-center">x' + twenty + '</div><div class="col text-right">' + AccountingFormat(twenty * 20) + '</div></div>');
                                    if (fifty > 0) divBreakdown.append('<div class="row p-2 border align-items-center"><div class="col">50</div><div class="col text-center">x' + fifty + '</div><div class="col text-right">' + AccountingFormat(fifty * 50) + '</div></div>');
                                    if (onehundred > 0) divBreakdown.append('<div class="row p-2 border align-items-center"><div class="col">100</div><div class="col text-center">x' + onehundred + '</div><div class="col text-right">' + AccountingFormat(onehundred * 100) + '</div></div>');
                                    if (twohundred > 0) divBreakdown.append('<div class="row p-2 border align-items-center"><div class="col">200</div><div class="col text-center">x' + twohundred + '</div><div class="col text-right">' + AccountingFormat(twohundred * 200) + '</div></div>');
                                    if (fivehundred > 0) divBreakdown.append('<div class="row p-2 border align-items-center"><div class="col">500</div><div class="col text-center">x' + fivehundred + '</div><div class="col text-right">' + AccountingFormat(fivehundred * 500) + '</div></div>');
                                    if (onethousand > 0) divBreakdown.append('<div class="row p-2 border align-items-center"><div class="col">1000</div><div class="col text-center">x' + onethousand + '</div><div class="col text-right">' + AccountingFormat(onethousand * 1000) + '</div></div>');
                                }

                                if (check > 0) divBreakdown.append($('<div class="row p-2 border align-items-center">').append($('<div class="col">').text('Check')).append($('<div class="col text-right">').text(AccountingFormat(check))));
                                if (creditcard > 0) divBreakdown.append($('<div class="row p-2 border align-items-center">').append($('<div class="col">').text('Credit Card')).append($('<div class="col text-right">').text(AccountingFormat(creditcard))));
                                if (debitcard > 0) divBreakdown.append($('<div class="row p-2 border align-items-center">').append($('<div class="col">').text('Debit Card')).append($('<div class="col text-right">').text(AccountingFormat(debitcard))));
                                if (srv > 0) divBreakdown.append($('<div class="row p-2 border align-items-center">').append($('<div class="col">').text('SRV')).append($('<div class="col text-right">').text(AccountingFormat(srv))));

                                for (var i = 0; i <= arrOtherPayment.length - 1; i++) {
                                    divBreakdown.append($('<div class="row p-2 border align-items-center">').append($('<div class="col">').text(arrOtherPayment[i].ID)).append($('<div class="col text-right">').text(AccountingFormat(CDec2($('#txtTotal' + arrOtherPayment[i].ID).val() || 0)))));
                                }

                                $('#lblFrom').text($('#txtDepositBy').val());

                                if ($('#optDepositTo').val().indexOf('My') >= 0) {
                                    $('#lblTo').text($('#txtDepositTo').val() + ' of ' + vCompanyName);
                                } else {
                                    $('#lblTo').text($('#txtFreeType').val() + ', ' + $('#optDepositTo').val().replace('3rd Party', '') + ' of ' + $('#txtDepositTo').val());
                                }

                                $('#popupConfirmationDialog').modal('show');

                                $('#btnSaveNow').unbind('click').click(function () {
                                    pSql = 'Select TotalAmount, ID from private.TransCashHandling where DepositedTo = ' + ReadyString(Web.PeopleUserID()) +
                                        ' AND FOWSTID= ' + ReadyString(sessionStorage.SelectedFOWST) +
                                        ' order by DepositDate desc';


                                    TotalAcceptedFromPrevTurnOver = Sql.ExecuteRow(pSql);

                                    pIU = new InsertUpdate('private.TransCashHandling');

                                    pIU.Add('ID', ReadyString(ID));
                                    pIU.Add('FOWSTID', ReadyString(sessionStorage.FowstID));
                                    pIU.Add('DepositDate', proj.GetZReadDateSql());
                                    pIU.Add('LastUpdated', proj.SqliteNow)

                                    if (DepositToID != undefined) {
                                        pIU.Add('DepositedTo', ReadyString(DepositToID))
                                    }

                                    pIU.Add('DepositedToByName', ReadyString(($('#txtFreeType').val() != '' ? $('#txtFreeType').val() + ', ' + $('#optDepositTo').val().replace('3rd Party', '') + ' of ' + $('#txtDepositTo').val() : $('#txtDepositTo').val() + ' of ' + vCompanyName)));

                                    if ($.trim($('#optDepositTo').val()) == "Bank Teller") {
                                        pIU.Add('BankBranch', ($.trim($('#txtBankBranch').val()) == "" ? 'NULL' : ReadyString($('#txtBankBranch').val())));
                                        pIU.Add('DepositReferenceNo', ($.trim($('#txtBankBranch').val()) == "" ? 'NULL' : ReadyString($('#txtDepositRefNo').val())));
                                        pIU.Add('ServiceCharge', ($.trim($('#txtBankBranch').val()) == "" ? '0' : ReadyString($('#txtServiceCharge').val())));
                                    }

                                    pIU.Add('DepositedToPosition', ReadyString($('#optDepositTo').val()));
                                    pIU.Add('DepositedBy', ReadyString(Web.PeopleUserID()));
                                    pIU.Add('DepositedByName', ReadyString($('#txtDepositBy').val()));
                                    pIU.Add('IsFromTotalAmount', CDec2(IsTotalAmount));
                                    pIU.Add('TotalAmount', TotalAmount);

                                    pIU.Add('NoOfOneCents', onecents);
                                    pIU.Add('NoOfFiveCents', fivecents);
                                    pIU.Add('NoOfTenCents', tencents);
                                    pIU.Add('NoOfTwentyFiveCents', twenty);
                                    pIU.Add('NoOfOnePeso', one);
                                    pIU.Add('NoOfFivePesos', five);
                                    pIU.Add('NoOfTenPesos', ten);
                                    pIU.Add('NoOfTwentyPesos', twenty);
                                    pIU.Add('NoOfFiftyPesos', fifty);
                                    pIU.Add('NoOfOneHundredPesos', onehundred);
                                    pIU.Add('NoOfTwoHundredPesos', twohundred);
                                    pIU.Add('NoOfFiveHundredPesos', fivehundred);
                                    pIU.Add('NoOfOneThousandPesos', onethousand);

                                    pIU.Add('TotalCash', cash);
                                    pIU.Add('TotalCheck', check);
                                    pIU.Add('TotalCreditCard', creditcard);
                                    pIU.Add('TotalDebitCard', debitcard);
                                    pIU.Add('TotalSRV', srv);
                                    pIU.Add('TotalPettyCash', (CDec2($('#txtTotalPettyCash').val() || 0)));

                                    //pIU.Add('TotalAlipay', (CDec2($('#txtTotalAlipay').val() || 0)));
                                    //pIU.Add('TotalComplimentary', (CDec2($('#txtTotalComplimentary').val() || 0)));
                                    //pIU.Add('TotalDiscountVoucher', (CDec2($('#txtTotalDiscountVoucher').val() || 0)));
                                    //pIU.Add('TotalEPlus', (CDec2($('#txtTotalEPlus').val() || 0)));
                                    //pIU.Add('TotalFoodPanda', (CDec2($('#txtTotalFoodPanda').val() || 0)));
                                    //pIU.Add('TotalGCash', (CDec2($('#txtTotalGCash').val() || 0)));
                                    //pIU.Add('TotalGoldCard', (CDec2($('#txtTotalGoldCard').val() || 0)));
                                    //pIU.Add('TotalGrabFood', (CDec2($('#txtTotalGrabFood').val() || 0)));
                                    //pIU.Add('TotalLocalDelivery', (CDec2($('#txtTotalLocalDelivery').val() || 0)));
                                    //pIU.Add('TotalPayMaya', (CDec2($('#txtTotalPayMaya').val() || 0)));
                                    //pIU.Add('TotalShopeeEVoucher', (CDec2($('#txtTotalShopeeEVoucher').val() || 0)));
                                    //pIU.Add('TotalShopeePay', (CDec2($('#txtTotalShopeePay').val() || 0)));
                                    //pIU.Add('TotalSMAC', (CDec2($('#txtTotalSMAC').val() || 0)));
                                    //pIU.Add('TotalVPlus', (CDec2($('#txtTotalVPlus').val() || 0)));

                                    for (var i = 0; i <= arrOtherPayment.length - 1; i++) {
                                        pIU.Add("Total" + arrOtherPayment[i].ID, (CDec2($('#txtTotal' + arrOtherPayment[i].ID).val() || 0)));
                                    }

                                    if (TotalAcceptedFromPrevTurnOver != undefined) {
                                        pIU.Add('TotalAcceptedFromPrevTurnOver', (CDec2(TotalAcceptedFromPrevTurnOver.TotalAmount || 0)));
                                        pIU.Add('ParentID', (TotalAcceptedFromPrevTurnOver.ID == undefined ? ReadyString('') : ReadyString(TotalAcceptedFromPrevTurnOver.ID)))
                                    }                                    

                                    pIU.Add('TransactionDateAndTimeFrom', ReadyString($.format.date($('#txtDateFrom').DateCombo('val'), 'M/d/yyyy h:mm:ss a')));
                                    pIU.Add('TransactionDateAndTimeTo', ReadyString($.format.date($('#txtDateTo').DateCombo('val'), 'M/d/yyyy h:mm:ss a')));

                                    pIU.Add('IsTurnOver', $('input[name="radTurnOverOfMonies"]:checked').val());

                                    pSql = pIU.GetInsert();
                                    pSql = Sql.ExecuteSql(pSql); // store the number of affected records to pSql for debugging. expected result = 1

                                    pSql = 'Select ID from TransInvoice where UserID = ' + ReadyString(Web.PeopleUserID()) + ' AND CashHandlingID IS NULL'

                                    var pDt;

                                    pDt = Sql.Execute(pSql);

                                    for (var i = 0; i <= pDt.Rows.length - 1; i++) {
                                        pSql = 'Update TransInvoice set CashHandlingID = ' + ReadyString(ID) + ' WHERE ID = ' + ReadyString(pDt.Rows[i].ID)
                                        Sql.ExecuteSql(pSql);
                                    }

                                    pSql = 'Select ID from TransInventoryLedger where UserID = ' + ReadyString(Web.PeopleUserID()) + ' AND CashHandlingID IS NULL'

                                    var pDt;

                                    pDt = Sql.Execute(pSql);

                                    for (var i = 0; i <= pDt.Rows.length - 1; i++) {
                                        pSql = 'Update TransInventoryLedger set CashHandlingID = ' + ReadyString(ID) + ' WHERE ID = ' + ReadyString(pDt.Rows[i].ID)
                                        Sql.ExecuteSql(pSql);
                                    }

                                    if (CStr2(GetUrlParameters().viewreport) == '1') {
                                        //ang param gikan sa dashboard if need sa cashier na magcashcount pa sila before ipakita ang ON REPORTS (CSR, MSR, ZREAD)
                                        NewMessageBox('Your transaction was saved successfully. You can now view your report.', 'Message', [
                                            {
                                                Text: 'Back to Dashboard and Reports'
                                            }
                                        ], {
                                            Close: function () {
                                                sessionStorage.removeItem('FowstID');
                                                location = MergeUrlParameters({
                                                    OrgID: sessionStorage.OrgID,
                                                    IsMobile: CBool(IsMobile) ? 1 : 0,
                                                    InvetoryOfPaymentDone: 1
                                                }, '/DashBoardsAndReports');
                                            }
                                        })
                                        return;
                                    }

                                    NewMessageBox('Your transaction was saved successfully. Do you want to print Monies Turnover Slip?', 'Deposit/Turn-over Monies', [
                                        {
                                            Text: 'Yes', Click: function () {
                                                PrintNow(ID);
                                                var pParam = {};
                                                pParam.OrgID = sessionStorage.OrgID;
                                                pParam.IsMobile = 1;
                                                pParam = $.param(pParam);
                                                window.location = "/CashHandling/MoniesForDepositOrTurnOver?" + pParam;
                                            }
                                        }, {
                                            Text: 'No', Click: function () {
                                                RedirectAfterSave();
                                            }
                                        }
                                    ])

                                //MessageBox('Your transaction was saved successfully. Do you want to print Turnover Monies Slip?', 'Save', 
                                //    [
                                //        {
                                //            Text: 'Yes',
                                //            Click: function () {
                                //                PrintTurnOverSlip();
                                //            }
                                //        },
                                //        {
                                //            Text: 'No',
                                //            Click: function () {
                                //                RedirectAfterSave();
                                //            }
                                //        }
                                //    ])
                                })
                            }
                            catch (e) {
                                MessageBox('Oops, something went wrong while saving transaction. Please contact Mobiz ASAP.');
                                console.log(e);
                            }
                        }

                        function SaveCashCount() {
                            var pIU;
                            var pSql;
                            var ID = NewID();

                            pIU = new InsertUpdate('private.TransCashHandlingCashCount');
                            
                            pIU.Add('FOWSTID', ReadyString(sessionStorage.FowstID));
                            pIU.Add('IsFromTotalAmount', CDec2(IsTotalAmount));
                            pIU.Add('TotalAmount', TotalAmount);
                            pIU.Add('CashCountedBy', ReadyString(Web.PeopleUserID()));
                            pIU.Add('CashCountedByName', ReadyString($('#txtDepositBy').val()));
                            pIU.Add('NoOfOneCents', (CDec2($('#txtNoOfOneCents').val() || 0)));
                            pIU.Add('NoOfFiveCents', (CDec2($('#txtNoOfFiveCents').val() || 0)));
                            pIU.Add('NoOfTenCents', (CDec2($('#txtNoOfTenCents').val() || 0)));
                            pIU.Add('NoOfTwentyFiveCents', (CDec2($('#txtNoOfTwentyFiveCents').val() || 0)));
                            pIU.Add('NoOfOnePeso', (CDec2($('#txtNoOfOnePeso').val() || 0)));
                            pIU.Add('NoOfFivePesos', (CDec2($('#txtNoOfFivePesos').val() || 0)));
                            pIU.Add('NoOfTenPesos', (CDec2($('#txtNoOfTenPesos').val() || 0)));
                            pIU.Add('NoOfTwentyPesos', (CDec2($('#txtNoOfTwentyPesos').val() || 0)));
                            pIU.Add('NoOfFiftyPesos', (CDec2($('#txtNoOfFiftyPesos').val() || 0)));
                            pIU.Add('NoOfOneHundredPesos', (CDec2($('#txtNoOfOneHundredPesos').val() || 0)));
                            pIU.Add('NoOfTwoHundredPesos', (CDec2($('#txtNoOfTwoHundredPesos').val() || 0)));
                            pIU.Add('NoOfFiveHundredPesos', (CDec2($('#txtNoOfFiveHundredPesos').val() || 0)));
                            pIU.Add('NoOfOneThousandPesos', (CDec2($('#txtNoOfOneThousandPesos').val() || 0)));

                            pIU.Add('TotalCash', (CDec2($('#txtTotalAmount').val() || 0)));
                            pIU.Add('TotalCheck', (CDec2($('#txtTotalCheck').val() || 0)));
                            pIU.Add('TotalCreditCard', (CDec2($('#txtTotalCreditCard').val() || 0)));
                            pIU.Add('TotalDebitCard', (CDec2($('#txtTotalDebitCard').val() || 0)));
                            pIU.Add('TotalSRV', (CDec2($('#txtTotalSRV').val() || 0)));
                            pIU.Add('TotalAlipay', (CDec2($('#txtTotalAlipay').val() || 0)));
                            pIU.Add('TotalComplimentary', (CDec2($('#txtTotalComplimentary').val() || 0)));
                            pIU.Add('TotalDiscountVoucher', (CDec2($('#txtTotalDiscountVoucher').val() || 0)));
                            pIU.Add('TotalEPlus', (CDec2($('#txtTotalEPlus').val() || 0)));
                            pIU.Add('TotalFoodPanda', (CDec2($('#txtTotalFoodPanda').val() || 0)));
                            pIU.Add('TotalGCash', (CDec2($('#txtTotalGCash').val() || 0)));
                            pIU.Add('TotalGoldCard', (CDec2($('#txtTotalGoldCard').val() || 0)));
                            pIU.Add('TotalGrabFood', (CDec2($('#txtTotalGrabFood').val() || 0)));
                            pIU.Add('TotalLocalDelivery', (CDec2($('#txtTotalLocalDelivery').val() || 0)));
                            pIU.Add('TotalPayMaya', (CDec2($('#txtTotalPayMaya').val() || 0)));
                            pIU.Add('TotalShopeeEVoucher', (CDec2($('#txtTotalShopeeEVoucher').val() || 0)));
                            pIU.Add('TotalShopeePay', (CDec2($('#txtTotalShopeePay').val() || 0)));
                            pIU.Add('TotalSMAC', (CDec2($('#txtTotalSMAC').val() || 0)));
                            pIU.Add('TotalVPlus', (CDec2($('#txtTotalVPlus').val() || 0)));

                            pIU.Add('LastUpdated', proj.SqliteNow);
                            pIU.Add('DateImported', "NULL");

                            pSql = pIU.GetUpdate(" DATE(CashCountDate) = " + ReadyString($.format.date(new Date(proj.GetZReadDateSql()), 'yyyy-MM-dd')) +
                                    '\n AND FOWSTID = ' + ReadyString(sessionStorage.FowstID) +
                                    '\n AND CashCountedBy = ' + ReadyString(Web.PeopleUserID()));
                            //console.log(Sql.ExecuteSql(pSql))
                            if (Sql.ExecuteSql(pSql) == 0) {
                                pIU.Add('ID', ReadyString(ID));
                                pIU.Add('CashCountDate', proj.GetZReadDateSql());
                                Sql.Execute(pIU.GetInsert())
                            }

                            if (CStr2(GetUrlParameters().viewreport) == '1') {
                                NewMessageBox('Cash count has been successfully saved. You can now view your report.', 'Message', [
                                    {
                                        Text: 'Back to Dashboard and Reports'
                                    }
                                ], {
                                    Close: function () {
                                        sessionStorage.removeItem('FowstID');
                                        location = MergeUrlParameters({
                                            OrgID: sessionStorage.OrgID,
                                            IsMobile: CBool(IsMobile) ? 1 : 0,
                                            InvetoryOfPaymentDone: 1
                                        }, '/DashBoardsAndReports');
                                    }
                                })
                            } else {
                                NewMessageBox('Cash count has been successfully saved.', 'Message')
                            }
                        }

                        function RedirectAfterSave() {
                            var pParam = {};

                            if (IsThirdPartyCollector) {
                                if (IsMobile)
                                    pParam.OrgID = sessionStorage.OrgID;
                                    pParam.IsMobile = 1;
                                pParam = $.param(pParam);
                                window.location = "/CashHandling/MoniesForDepositOrTurnOver?" + pParam;
                            } else {
                                NewMessageBox('Would you like to Sign Out?', 'Sign Out',
                                [
                                    {
                                        Text: 'Yes',
                                        Click: function () {
                                            SignInCheckUser(); // SignInInfo.js
                                            sessionStorage.removeItem('TOUserName');
                                            sessionStorage.removeItem('TOPassword');
                                            sessionStorage.removeItem('IsFromTurnOverOfMoniesPage');
                                        }
                                    },
                                    {
                                        Text: 'No',
                                        Click: function () {
                                            var pParam = {};

                                            sessionStorage.removeItem('TOUserName');
                                            sessionStorage.removeItem('TOPassword');
                                            sessionStorage.removeItem('IsFromTurnOverOfMoniesPage');

                                            pParam.OrgID = sessionStorage.OrgID;
                                            pParam.IsMobile = 1;
                                            pParam = $.param(pParam);
                                            window.location = "/CashHandling/MoniesForDepositOrTurnOver?" + pParam;
                                        }
                                    }
                                ])
                            }
                        }

                        window.PrintTurnOverSlip = function () {
                            PrintTurnOverSlip();
                        }

                        function PrintNow(ID) {
                            var vDataPrinterType = SalesPayment_offline().getOrgFOWSTPrinterType();
                            var vData = Sql.ExecuteRow('SELECT * FROM TransCashHandling WHERE ID = ' + ReadyString(ID));

                            if (vDataPrinterType.Rows.length > 0) {
                                if (vDataPrinterType.Rows[0].PrinterType == 'Epson TM-U220B Printer') {
                                    //PrintPOSReceiptDotMatrix(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg);
                                } else if (vDataPrinterType.Rows[0].PrinterType == '80MM Mini Thermal Printer') {

                                } else if (vDataPrinterType.Rows[0].PrinterType == 'Windows Print Server') {
                                    PrintTurnOverSlipWPS();
                                } else if (vDataPrinterType.Rows[0].PrinterType == 'IMin IMin 58MM Printer') {
                                    try {
                                        window.pPrint.InitializeBluetoothPrinter();
                                    } catch (e) {
                                        console.log('InitializeBluetoothPrinter: ' + e);
                                    };
                                    PrintTurnOverSlipImin(vData);
                                } else {
                                    PrintTurnOverSlip();
                                }
                            } else {
                                PrintTurnOverSlip();
                            }
                        }

                        function PrintTurnOverSlipWPS() {
                            var vDataPrint = SalesPayment_offline().getOrgFOWSTPrinterIP();
                            //var vDataPrinterType = SalesPayment_offline().getOrgFOWSTPrinterType();
                            //console.log(vDataPrinterType)

                            var strPrinterIP;

                            if (vDataPrint.Rows[0].PrinterIPAddress != undefined) {
                                strPrinterIP = vDataPrint.Rows[0].PrinterIPAddress;
                            }

                            WindowsPrinter("MONIES TURNOVER SLIP", [0, 0, 0, 1, 0, 36]);
                            WindowsPrinter("newLine", [0, 0, 0, 1, 0, 36]);

                            WindowsPrinter($.trim((vDataHeader.OrgBusinessTradeName || '')), [0, 0, 0, 1, 0, 36]);
                            WindowsPrinter($.trim((vDataHeader.SubscriberOrganizationName || '')), [0, 0, 0, 1, 0, 36]);
                            WindowsPrinter($.trim((vDataHeader.OrgBranchName || '')), [0, 0, 0, 1, 0, 36]);
                            WindowsPrinter($.trim((vDataHeader.OrgCompleteAddress || '')), [0, 0, 0, 1, 0, 36]);
                            WindowsPrinter('Tel. No.' + $.trim((vDataHeader.OrgTelNo || '')), [0, 0, 0, 1, 0, 36]);

                            WindowsPrinter("newLine", [0, 0, 0, 1, 0, 36]);
                            WindowsPrinter("newLine", [0, 0, 0, 1, 0, 36]);

                            WindowsPrinter('Total Turned Over Amt.', [1, 22, 0, 0, 0, 36], " : " + AccountingFormat(TotalAmount), [1, 18, 0, 0, 0, 36]);

                            WindowsPrinter("newLine", [0, 0, 0, 1, 0, 36]);

                            WindowsPrinter(" -------------- Breakdown -------------- ", [0, 0, 0, 1, 0, 36]);

                            WindowsPrinter("newLine", [0, 0, 0, 1, 0, 36]);                            

                            if (!IsTotalAmount) {
                                WindowsPrinter('Detailed breakdown of Cash', [1, 36, 0, 0, 0, 36]);

                                if (CDec2($('#txtNoOfFiveCents').val()) != 0) {
                                    WindowsPrinter('  Php 0.05     x ' + CDec2($('#txtNoOfFiveCents').val()), [1, 22, 0, 0, 0, 36], " : " + AccountingFormat(0.05 * CDec2($('#txtNoOfFiveCents').val())), [1, 18, 0, 0, 0, 36]);
                                }

                                if (CDec2($('#txtNoOfTenCents').val()) != 0) {
                                    WindowsPrinter('  Php 0.10     x ' + CDec2($('#txtNoOfTenCents').val()), [1, 22, 0, 0, 0, 36], " : " + AccountingFormat(0.10 * CDec2($('#txtNoOfTenCents').val())), [1, 18, 0, 0, 0, 36]);
                                }

                                if (CDec2($('#txtNoOfTwentyFiveCents').val()) != 0) {
                                    WindowsPrinter('  Php 0.25     x ' + CDec2($('#txtNoOfTwentyFiveCents').val()), [1, 22, 0, 0, 0, 36], " : " + AccountingFormat(0.25 * CDec2($('#txtNoOfTwentyFiveCents').val())), [1, 18, 0, 0, 0, 36]);
                                }

                                if (CDec2($('#txtNoOfOnePeso').val()) != 0) {
                                    WindowsPrinter('  Php 1.00     x ' + CDec2($('#txtNoOfOnePeso').val()), [1, 22, 0, 0, 0, 36], " : " + AccountingFormat(1 * CDec2($('#txtNoOfOnePeso').val())), [1, 18, 0, 0, 0, 36]);
                                }

                                if (CDec2($('#txtNoOfFivePesos').val()) != 0) {
                                    WindowsPrinter('  Php 5.00     x ' + CDec2($('#txtNoOfFivePesos').val()), [1, 22, 0, 0, 0, 36], " : " + AccountingFormat(5 * CDec2($('#txtNoOfFivePesos').val())), [1, 18, 0, 0, 0, 36]);
                                }

                                if (CDec2($('#txtNoOfTenPesos').val()) != 0) {
                                    WindowsPrinter('  Php 10.00    x ' + CDec2($('#txtNoOfTenPesos').val()), [1, 22, 0, 0, 0, 36], " : " + AccountingFormat(10 * CDec2($('#txtNoOfTenPesos').val())), [1, 18, 0, 0, 0, 36]);
                                }

                                if (CDec2($('#txtNoOfTwentyPesos').val()) != 0) {
                                    WindowsPrinter('  Php 20.00    x ' + CDec2($('#txtNoOfTwentyPesos').val()), [1, 22, 0, 0, 0, 36], " : " + AccountingFormat(20 * CDec2($('#txtNoOfTwentyPesos').val())), [1, 18, 0, 0, 0, 36]);
                                }
                                
                                if (CDec2($('#txtNoOfFiftyPesos').val()) != 0) {
                                    WindowsPrinter('  Php 50.00    x ' + CDec2($('#txtNoOfFiftyPesos').val()), [1, 22, 0, 0, 0, 36], " : " + AccountingFormat(50 * CDec2($('#txtNoOfFiftyPesos').val())), [1, 18, 0, 0, 0, 36]);
                                }
                                
                                if (CDec2($('#txtNoOfOneHundredPesos').val()) != 0) {
                                    WindowsPrinter('  Php 100.00   x ' + CDec2($('#txtNoOfOneHundredPesos').val()), [1, 22, 0, 0, 0, 36], " : " + AccountingFormat(100 * CDec2($('#txtNoOfOneHundredPesos').val())), [1, 18, 0, 0, 0, 36]);
                                }

                                if (CDec2($('#txtNoOfTwoHundredPesos').val()) != 0) {
                                    WindowsPrinter('  Php 200.00   x ' + CDec2($('#txtNoOfTwoHundredPesos').val()), [1, 22, 0, 0, 0, 36], " : " + AccountingFormat(200 * CDec2($('#txtNoOfTwoHundredPesos').val())), [1, 18, 0, 0, 0, 36]);
                                }

                                if (CDec2($('#txtNoOfFiveHundredPesos').val()) != 0) {
                                    WindowsPrinter('  Php 500.00   x ' + CDec2($('#txtNoOfFiveHundredPesos').val()), [1, 22, 0, 0, 0, 36], " : " + AccountingFormat(500 * CDec2($('#txtNoOfFiveHundredPesos').val())), [1, 18, 0, 0, 0, 36]);
                                }

                                if (CDec2($('#txtNoOfOneThousandPesos').val()) != 0) {
                                    WindowsPrinter('  Php 1,000.00 x ' + CDec2($('#txtNoOfOneThousandPesos').val()), [1, 22, 0, 0, 0, 36], " : " + AccountingFormat(1000 * CDec2($('#txtNoOfOneThousandPesos').val())), [1, 18, 0, 0, 0, 36]);
                                }
                            } else {
                                WindowsPrinter('Total Cash', [1, 22, 0, 0, 0, 36], " : " + AccountingFormat($('#txtTotalAmount').val()), [1, 12, 0, 0, 0, 36]);
                            }

                            WindowsPrinter('Total Check', [1, 22, 0, 0, 0, 36], " : " + AccountingFormat($('#txtTotalCheck').val()), [1, 12, 0, 0, 0, 36]);
                            WindowsPrinter('Total Credit Card', [1, 22, 0, 0, 0, 36], " : " + AccountingFormat($('#txtTotalCreditCard').val()), [1, 12, 0, 0, 0, 36]);
                            WindowsPrinter('Total Debit Card', [1, 22, 0, 0, 0, 36], " : " + AccountingFormat($('#txtTotalDebitCard').val()), [1, 12, 0, 0, 0, 36]);
                            WindowsPrinter('Total SRV', [1, 22, 0, 0, 0, 36], " : " + AccountingFormat($('#txtTotalSRV').val()), [1, 12, 0, 0, 0, 36]);

                            WindowsPrinter("newLine", [0, 0, 0, 1, 0, 36]);

                            WindowsPrinter('From:', [1, 36, 0, 0, 0, 36]);

                            WindowsPrinter("newLine", [0, 0, 0, 1, 0, 36]);
                            WindowsPrinter("newLine", [0, 0, 0, 1, 0, 36]);

                            WindowsPrinter('____________________________________', [1, 36, 0, 0, 0, 36]);
                            WindowsPrinter("newLine", [0, 0, 0, 1, 0, 36]);
                            WindowsPrinter($('#txtDepositBy').val(), [0, 0, 0, 1, 0, 36]);

                            WindowsPrinter("newLine", [0, 0, 0, 1, 0, 36]);
                            WindowsPrinter("newLine", [0, 0, 0, 1, 0, 36]);

                            WindowsPrinter('To:', [1, 36, 0, 0, 0, 36]);

                            WindowsPrinter("newLine", [0, 0, 0, 1, 0, 36]);
                            WindowsPrinter("newLine", [0, 0, 0, 1, 0, 36]);

                            var mTo = '';

                            if ($('#optDepositTo').val().indexOf('My') >= 0) {
                                mTo = $('#txtDepositTo').val() + ' of ' + vCompanyName;
                            } else {
                                mTo = $('#txtFreeType').val() + ', ' + $('#optDepositTo').val().replace('3rd Party', '') + ' of ' + $('#txtDepositTo').val();
                            }

                            WindowsPrinter('____________________________________', [1, 36, 0, 0, 0, 36]);
                            WindowsPrinter("newLine", [0, 0, 0, 1, 0, 36]);
                            WindowsPrinter(mTo, [0, 0, 0, 1, 0, 36]);

                            WindowsPrinter("newLine", [0, 0, 0, 1, 0, 36]);
                            WindowsPrinter("newLine", [0, 0, 0, 1, 0, 36]);

                            WindowsPrinter("newLine", [0, 0, 0, 1, 0, 36]);
                            WindowsPrinter("Date and Time : " + $.format.date(new Date(), 'MMM dd, yyyy HH:mm'), [0, 0, 0, 1, 0, 36]);

                            WindowsPrinter("address", [0, 0, 0, 0, 0, 0], strPrinterIP, [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);
                            WindowsPrinter("startPrint", [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0], "", [0, 0, 0, 0, 0, 0]);

                            ClearCommandBuffer();

                            NewMessageBox('Were you able to print Monies Turnover Slip?', 'Verify Print', [
                                {
                                    Text: 'Yes', Click: function () {
                                        RedirectAfterSave();
                                    }
                                }, {
                                    Text: 'No', Click: function () {
                                        WebToast('Trying to print again. Please wait...', 'top');
                                        setTimeout(function () {
                                            PrintTurnOverSlipWPS();
                                        }, 1000)
                                    }
                                }
                            ])
                        }

                        function PrintTurnOverSlip() {

                            var vDataPrint = SalesPayment_offline().getOrgFOWSTPrinterIP();
                            //var vDataPrinterType = SalesPayment_offline().getOrgFOWSTPrinterType();
                            //console.log(vDataPrinterType)

                            var strPrinterIP;

                            if (vDataPrint.Rows[0].PrinterIPAddress != undefined) {
                                strPrinterIP = vDataPrint.Rows[0].PrinterIPAddress;
                            }

                            var intFAFCTotalLenght = 40;
                            var intFBTotalLenght = 27;
                            var intFCDoubleTotalLenght = 18;

                            var intDesktopPrintLenght = 45;

                            //New set of text to print
                            NewPrintBuilder();


                            strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Monies Turnover Slip', intFAFCTotalLenght, '', 0, '', 0, 1);
                            ReadyTextToPrint(strPrint + '\n', 3, 1, 1, 1, 1, 1, 0, 0, 0);
                            //ReadyTextToPrint(strPrint + '\n', 3, 1, 1, 1, 1, 1, 0, 0, 0);

                            //Get Header
                            strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.OrgBusinessTradeName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                            strPrint = '';
                            strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.SubscriberOrganizationName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            strPrint = strPrint + strTemp;

                            strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.OrgBranchName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            strPrint = strPrint + strTemp;

                            strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.OrgCompleteAddress || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            strPrint = strPrint + strTemp;

                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Tel. No.' + $.trim((vDataHeader.OrgTelNo || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            strPrint = strPrint + strTemp;

                            strPrint = strPrint + '\n';

                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                            strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Turned Over Amt.', 24, ' : ' + AccountingFormat(TotalAmount), intFAFCTotalLenght, '', 0, 1);
                            ReadyTextToPrint(strPrint + '\n', 3, 1, 1, 1, 0, 0, 0, 0);

                            strPrint = FormatTextToPrint(intFAFCTotalLenght, ' -------------- Breakdown -------------- ', intFAFCTotalLenght, '', intFAFCTotalLenght, '', 0, 1);
                            ReadyTextToPrint(strPrint + '\n', 3, 1, 1, 1, 0, 1, 0, 0);

                            if (!IsTotalAmount) {
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Detailed breakdown of Cash', intFAFCTotalLenght, '', 0, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0);

                                if (CDec2($('#txtNoOfFiveCents').val()) != 0) {
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '  Php 0.05     x ' + CDec2($('#txtNoOfFiveCents').val()), 24, ' : ' + AccountingFormat(0.05 * CDec2($('#txtNoOfFiveCents').val())), intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0);
                                }

                                if (CDec2($('#txtNoOfTenCents').val()) != 0) {
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '  Php 0.10     x ' + CDec2($('#txtNoOfTenCents').val()), 24, ' : ' + AccountingFormat(0.10 * CDec2($('#txtNoOfTenCents').val())), intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0);
                                }

                                if (CDec2($('#txtNoOfTwentyFiveCents').val()) != 0) {
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '  Php 0.25     x ' + CDec2($('#txtNoOfTwentyFiveCents').val()), 24, ' : ' + AccountingFormat(0.25 * CDec2($('#txtNoOfTwentyFiveCents').val())), intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0);
                                }

                                if (CDec2($('#txtNoOfOnePeso').val()) != 0) {
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '  Php 1.00     x ' + CDec2($('#txtNoOfOnePeso').val()), 24, ' : ' + AccountingFormat(1 * CDec2($('#txtNoOfOnePeso').val())), intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0);
                                }

                                if (CDec2($('#txtNoOfFivePesos').val()) != 0) {
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '  Php 5.00     x ' + CDec2($('#txtNoOfFivePesos').val()), 24, ' : ' + AccountingFormat(5 * CDec2($('#txtNoOfFivePesos').val())), intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0);
                                }

                                if (CDec2($('#txtNoOfTenPesos').val()) != 0) {
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '  Php 10.00    x ' + CDec2($('#txtNoOfTenPesos').val()), 24, ' : ' + AccountingFormat(10 * CDec2($('#txtNoOfTenPesos').val())), intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0);
                                }

                                if (CDec2($('#txtNoOfTwentyPesos').val()) != 0) {
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '  Php 20.00    x ' + CDec2($('#txtNoOfTwentyPesos').val()), 24, ' : ' + AccountingFormat(20 * CDec2($('#txtNoOfTwentyPesos').val())), intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0);
                                }

                                if (CDec2($('#txtNoOfFiftyPesos').val()) != 0) {
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '  Php 50.00    x ' + CDec2($('#txtNoOfFiftyPesos').val()), 24, ' : ' + AccountingFormat(50 * CDec2($('#txtNoOfFiftyPesos').val())), intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0);
                                }

                                if (CDec2($('#txtNoOfOneHundredPesos').val()) != 0) {
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '  Php 100.00   x ' + CDec2($('#txtNoOfOneHundredPesos').val()), 24, ' : ' + AccountingFormat(100 * CDec2($('#txtNoOfOneHundredPesos').val())), intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0);
                                }

                                if (CDec2($('#txtNoOfTwoHundredPesos').val()) != 0) {
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '  Php 200.00   x ' + CDec2($('#txtNoOfTwoHundredPesos').val()), 24, ' : ' + AccountingFormat(200 * CDec2($('#txtNoOfTwoHundredPesos').val())), intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0);
                                }

                                if (CDec2($('#txtNoOfFiveHundredPesos').val()) != 0) {
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '  Php 500.00   x ' + CDec2($('#txtNoOfFiveHundredPesos').val()), 24, ' : ' + AccountingFormat(500 * CDec2($('#txtNoOfFiveHundredPesos').val())), intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0);
                                }

                                if (CDec2($('#txtNoOfOneThousandPesos').val()) != 0) {
                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '  Php 1,000.00 x ' + CDec2($('#txtNoOfOneThousandPesos').val()), 24, ' : ' + AccountingFormat(1000 * CDec2($('#txtNoOfOneThousandPesos').val())), intFAFCTotalLenght, '', 0, 1);
                                    ReadyTextToPrint(strPrint + '\n', 3, 1, 1, 0, 0, 0, 0, 0);
                                }
                            } else {
                                strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Cash', 24, ' : ' + AccountingFormat($('#txtTotalAmount').val()), intFAFCTotalLenght, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0);
                            }
                            
                            strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Check', 24, ' : ' + AccountingFormat($('#txtTotalCheck').val()), intFAFCTotalLenght, '', 0, 1);
                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0);
                            strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Credit Card', 24, ' : ' + AccountingFormat($('#txtTotalCreditCard').val()), intFAFCTotalLenght, '', 0, 1);
                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0);
                            strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Debit Card', 24, ' : ' + AccountingFormat($('#txtTotalDebitCard').val()), intFAFCTotalLenght, '', 0, 1);
                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0);
                            strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total SRV', 24, ' : ' + AccountingFormat($('#txtTotalSRV').val()), intFAFCTotalLenght, '', 0, 1);
                            ReadyTextToPrint(strPrint + '\n\n', 3, 1, 1, 0, 0, 0, 0, 0);
                            //strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Total Petty Cash', 24, ' : ' + AccountingFormat($('#txtTotalPettyCash').val()), intFAFCTotalLenght, '', 0, 1);
                            //ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0);

                            strPrint = FormatTextToPrint(intFAFCTotalLenght, ' From: ', 9, ' ', intFAFCTotalLenght, '', 0, 1);
                            ReadyTextToPrint(strPrint + '\n\n', 3, 1, 1, 0, 0, 0, 0, 0);

                            strPrint = FormatTextToPrint(intFAFCTotalLenght, "________________________________________", intFAFCTotalLenght, '', 0, '', 0, 1);
                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                            strPrint = FormatTextToPrint(intFAFCTotalLenght, ' ' + $('#txtDepositBy').val(), 38, ' ', intFAFCTotalLenght, '', 0, 1);
                            ReadyTextToPrint(strPrint + '\n\n', 3, 1, 1, 0, 0, 0, 0, 0);

                            var mTo = '';

                            if ($('#optDepositTo').val().indexOf('My') >= 0) {
                                mTo = $('#txtDepositTo').val() + ' of ' + vCompanyName;
                            } else {
                                mTo = $('#txtFreeType').val() + ', ' + $('#optDepositTo').val().replace('3rd Party', '') + ' of ' + $('#txtDepositTo').val();
                            }

                            strPrint = FormatTextToPrint(intFAFCTotalLenght, ' To: ', 9, ' ', intFAFCTotalLenght, '', 0, 1);
                            ReadyTextToPrint(strPrint + '\n\n', 3, 1, 1, 0, 0, 0, 0, 0);

                            strPrint = FormatTextToPrint(intFAFCTotalLenght, "________________________________________", intFAFCTotalLenght, '', 0, '', 0, 1);
                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                            strPrint = FormatTextToPrint(intFAFCTotalLenght, ' ' + mTo, 38, ' ', intFAFCTotalLenght, '', 0, 1);
                            ReadyTextToPrint(strPrint + '\n\n', 3, 1, 1, 0, 0, 0, 0, 0);

                            strPrint = FormatTextToPrint(intFAFCTotalLenght, ' Date and Time ', 17, ' : ' + $.format.date(new Date(), 'MMM dd, yyyy HH:mm'), intFAFCTotalLenght, '', 0, 1);
                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0);

                            ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 1);
                            console.log(strPrinterIP)
                            PrintText("192.168.1.102");
                            ClearCommandBuffer();

                            $.mobile.changePage("#dialogVerifyPrintTurnoverSlip");

                            //setTimeout(function () {
                            //    MessageBox('Were you able to print Turnover Monies Slip?',
                            //    "Save",
                            //    [
                            //        {
                            //            Text: 'Yes',
                            //            Click: function () {
                            //                RedirectAfterSave();
                            //            }
                            //        },
                            //        {
                            //            Text: 'No',
                            //            Click: function () {
                            //                PrintTurnOverSlip();
                            //            }
                            //        }
                            //    ]); // END MessageBox
                            //}, 1000)
                        }

                        function PrintTurnOverSlipImin(vData) {
                            var vDataHeader = SalesReport_offline().getReportHeader(0).Rows[0];
                            var IsVAT = CBool(Sql.ExecuteScalar('Select IFNULL(IsVAT, 1) from LibraryOrganizationFOWST WHERE ID = ' + ReadyString(sessionStorage.FowstID)));

                            try {
                                window.pPrint.InitializeBluetoothPrinter();
                            } catch (e) {
                                console.log('InitializeBluetoothPrinter: ' + e);
                            };

                            NewPrintBuilder();

                            var strPrint = '';
                            IminPrintInColumns('', 'Monies Turnover Slip', '', 0, 10, 0, 0, 1, 0, 24, 36, 24);
                            IminfeedPaper();

                            //Get Header
                            IminPrintInColumns('', vDataHeader.OrgBusinessTradeName, '', 0, 10, 0, 0, 1, 0, 24, 24, 24);
                            IminPrintInColumns('', vDataHeader.SubscriberOrganizationName, '', 0, 10, 0, 0, 1, 0, 24, 24, 24);
                            IminPrintInColumns('', vDataHeader.OrgBranchName, '', 0, 10, 0, 0, 1, 0, 24, 24, 24);
                            IminPrintInColumns('', (!CBool(IsVAT) ? 'NON-' : '') + 'VAT TIN ' + $.trim((vDataHeader.TIN || '')), '', 0, 10, 0, 0, 1, 0, 24, 24, 24);
                            IminPrintInColumns('', vDataHeader.OrgCompleteAddress, '', 0, 10, 0, 0, 1, 0, 24, 24, 24);

                            IminfeedPaper();

                            IminPrintInColumns('Date: ' + $.format.date(new Date(vData.DepositDate), 'MM-dd-yyyy'), '', '', 12, 0, 0, 1, 0, 0, 24, 24, 24);

                            IminfeedPaper();

                            IminPrintInColumns('Turned Over Amt:', '', AccountingFormat(vData.TotalAmount), 6, 1, 5, 0, 0, 2, 24, 24, 24);

                            IminfeedPaper();
                            IminPrintInColumns('', '***** BREAKDOWN *****', '', 0, 10, 0, 0, 1, 0, 24, 24, 24);
                            if (CBool(vData.IsFromTotalAmount)) {
                                if (CDec2(vData.TotalCash) > 0) IminPrintInColumns('Cash:', '', AccountingFormat(vData.TotalCash), 6, 1, 5, 0, 0, 2, 24, 24, 24);
                            } else {
                                if (CDec2(vData.NoOfOneCents) > 0) IminPrintInColumns('P 0.01', 'x ' + CDec2(vData.NoOfOneCents), AccountingFormat(CDec2(vData.NoOfOneCents) * 0.01), 5, 2, 5, 0, 1, 2, 24, 24, 24);
                                if (CDec2(vData.NoOfFiveCents) > 0) IminPrintInColumns('P 0.05', 'x ' + CDec2(vData.NoOfFiveCents), AccountingFormat(CDec2(vData.NoOfFiveCents) * 0.05), 5, 2, 5, 0, 1, 2, 24, 24, 24);
                                if (CDec2(vData.NoOfTenCents) > 0) IminPrintInColumns('P 0.10', 'x ' + CDec2(vData.NoOfTenCents), AccountingFormat(CDec2(vData.NoOfTenCents) * 0.10), 5, 2, 5, 0, 1, 2, 24, 24, 24);
                                if (CDec2(vData.NoOfTwentyFiveCents) > 0) IminPrintInColumns('P 0.25', 'x ' + CDec2(vData.NoOfTwentyFiveCents), AccountingFormat(CDec2(vData.NoOfTwentyFiveCents) * 0.25), 5, 2, 5, 0, 1, 2, 24, 24, 24);
                                if (CDec2(vData.NoOfOnePeso) > 0) IminPrintInColumns('P 1.00', 'x ' + CDec2(vData.NoOfOnePeso), AccountingFormat(CDec2(vData.NoOfOnePeso) * 1), 5, 2, 5, 0, 1, 2, 24, 24, 24);
                                if (CDec2(vData.NoOfFivePesos) > 0) IminPrintInColumns('P 5.00', 'x ' + CDec2(vData.NoOfFivePesos), AccountingFormat(CDec2(vData.NoOfFivePesos) * 5), 5, 2, 5, 0, 1, 2, 24, 24, 24);
                                if (CDec2(vData.NoOfTenPesos) > 0) IminPrintInColumns('P 10.00', 'x ' + CDec2(vData.NoOfTenPesos), AccountingFormat(CDec2(vData.NoOfTenPesos) * 10), 5, 2, 5, 0, 1, 2, 24, 24, 24);
                                if (CDec2(vData.NoOfTwentyPesos) > 0) IminPrintInColumns('P 20.00', 'x ' + CDec2(vData.NoOfTwentyPesos), AccountingFormat(CDec2(vData.NoOfTwentyPesos) * 20), 5, 2, 5, 0, 1, 2, 24, 24, 24);
                                if (CDec2(vData.NoOfFiftyPesos) > 0) IminPrintInColumns('P 50.00', 'x ' + CDec2(vData.NoOfFiftyPesos), AccountingFormat(CDec2(vData.NoOfFiftyPesos) * 50), 5, 2, 5, 0, 1, 2, 24, 24, 24);
                                if (CDec2(vData.NoOfOneHundredPesos) > 0) IminPrintInColumns('P 100.00', 'x ' + CDec2(vData.NoOfOneHundredPesos), AccountingFormat(CDec2(vData.NoOfOneHundredPesos) * 100), 5, 2, 5, 0, 1, 2, 24, 24, 24);
                                if (CDec2(vData.NoOfTwoHundredPesos) > 0) IminPrintInColumns('P 200.00', 'x ' + CDec2(vData.NoOfTwoHundredPesos), AccountingFormat(CDec2(vData.NoOfTwoHundredPesos) * 200), 5, 2, 5, 0, 1, 2, 24, 24, 24);
                                if (CDec2(vData.NoOfFiveHundredPesos) > 0) IminPrintInColumns('P 500.00', 'x ' + CDec2(vData.NoOfFiveHundredPesos), AccountingFormat(CDec2(vData.NoOfFiveHundredPesos) * 500), 5, 2, 5, 0, 1, 2, 24, 24, 24);
                                if (CDec2(vData.NoOfOneThousandPesos) > 0) IminPrintInColumns('P 1000.00', 'x ' + CDec2(vData.NoOfOneThousandPesos), AccountingFormat(CDec2(vData.NoOfOneThousandPesos) * 1000), 5, 2, 5, 0, 1, 2, 24, 24, 24);
                            }                            

                            if (CDec2(vData.TotalCheck) > 0) IminPrintInColumns('Check/GC:', '', AccountingFormat(vData.TotalCheck), 6, 1, 5, 0, 0, 2, 24, 24, 24);
                            if (CDec2(vData.TotalDebitCard) > 0) IminPrintInColumns('Debit Card:', '', AccountingFormat(vData.TotalDebitCard), 6, 1, 5, 0, 0, 2, 24, 24, 24);
                            if (CDec2(vData.TotalCreditCard) > 0) IminPrintInColumns('Credit Card:', '', AccountingFormat(vData.TotalCreditCard), 6, 1, 5, 0, 0, 2, 24, 24, 24);
                            if (CDec2(vData.TotalSRV) > 0) IminPrintInColumns('SRV:', '', AccountingFormat(vData.TotalSRV), 6, 1, 5, 0, 0, 2, 24, 24, 24);

                            for (var i = 0; i < rowsOP.length; i++) {
                                var opname = rowsOP[i].Description.replace(/\s+/g, '');
                                var amt = CDec2(vData['Total' + opname]);
                                if (CDec2(amt) > 0) IminPrintInColumns(opname + ':', '', AccountingFormat(amt), 6, 1, 5, 0, 0, 2, 24, 24, 24);
                            }

                            IminfeedPaper();

                            IminPrintInColumns('*Please sign above your printed name*', '', '', 12, 0, 0, 1, 0, 0, 24, 24, 24);

                            IminfeedPaper();
                            IminfeedPaper();
                            IminfeedPaper();

                            IminPrintInColumns('__________________________________', '', '', 12, 0, 0, 0, 0, 0, 24, 24, 24);
                            IminPrintInColumns('From: ' + vData.DepositedByName, '', '', 12, 0, 0, 1, 0, 0, 24, 24, 24);

                            IminfeedPaper();
                            IminfeedPaper();
                            IminfeedPaper();

                            IminPrintInColumns('__________________________________', '', '', 12, 0, 0, 0, 0, 0, 24, 24, 24);
                            IminPrintInColumns('To: ' + vData.DepositedToByName, '', '', 12, 0, 0, 1, 0, 0, 24, 24, 24);

                            IminfeedPaper();
                            IminfeedPaper();
                            IminfeedPaper();
                            IminfeedPaper();
                            IminfeedPaper();
                        }

                        function GetOrganizations() {
                            IsEmployee = false;

                            BrowsePopup('LibraryOrganization', 'Search ' + $('#optDepositTo').val(), [
                                {
                                    Offline: true,
                                    Search: $.trim($('#txtDepositTo').val()),
                                    DisplayText: [{ Column: 'CompanyName' }],
                                    Click: function (e) {
                                        DepositToID = e.ID
                                        $('#txtDepositTo').val(e.CompanyName);
                                        $('#dvFreeType').css('visibility', 'visible');
                                        $('#dvFreeType').css('display', 'block');
                                        if ($.trim($('#optDepositTo').val()) == "Bank Teller") {
                                            $('#divForBank').show();
                                            $('#txtBankBranch, #txtDepositRefNo, #txtServiceCharge').val('');
                                        }
                                    }
                                }
                            ], {
                                FilterCustomers: 0
                            })
                        }

                        function GetEmployees() {
                            IsEmployee = true;

                            BrowsePopup('LibraryPeople', 'Search ' + $('#optDepositTo').val().split(' ')[2] + ' Name', [
                                {
                                    Offline: true,
                                    Search: $.trim($('#txtDepositTo').val()),
                                    DisplayText: [{ Column: 'PeopleName' }],
                                    Click: function (e) {
                                        DepositToID = e.ID
                                        $('#txtDepositTo').val(e.PeopleName);
                                        $('#lblReceiverName').text(e.PeopleName);
                                        $('#dvFreeType').css('visibility', 'hidden');
                                        $('#dvFreeType').css('display', 'none');

                                        if (isRequiredEmpCredentials) {
                                            $('#DialogUserAccess').modal('show');
                                        } else IsEmployeeVerified = true;
                                    }
                                }
                            ])
                        }

                        function GetCashierSalesReportPage() {
                            sessionStorage.POSSalesReportType = 0;
                            if (CounterCashierSalesReport == true) {
                                if (CBool(sessionStorage.PrintingOfflineData)) {
                                    AjaxSend({
                                        url: 'Login3/TestCookies',
                                        success: function (vData) {
                                            PromptUserAccess('Z Report', {
                                                OnClickOKSuccess: function () {

                                                    var pData = {};
                                                    pData.OrgID = sessionStorage["OrgID"];
                                                    pData.POSSalesReportType = 0;

                                                    AjaxSend({
                                                        url: 'ReportsPOS/setPOSSalesReportType',
                                                        data: pData,
                                                        success: function (vData) {
                                                            AjaxSend({
                                                                ExactUrl: '/ReportsPOS/SalesReport?cache=1&OrgID=' + sessionStorage.OrgID + '&IsManagerSalesReport=0&IsMobile=' + IsMobile,
                                                                type: 'GET',
                                                                dataType: 'text',
                                                                success: function (vData) {

                                                                    $('#divOnPrintPOSSalesReport').empty();
                                                                    $('#divOnPrintPOSSalesReport').append(vData).trigger('create');

                                                                    App.Deferred.fulfill('FQ26E5TP76');

                                                                    setMonthCollection("selPOSSalesReportByDateFromMonth");
                                                                    setYearCollection("selPOSSalesReportByDateFromYear");

                                                                    setMonthCollection("selPOSSalesReportByDateToMonth");
                                                                    setYearCollection("selPOSSalesReportByDateToYear");

                                                                    setDashboardDate();

                                                                    $('#dvReportHeader').css('visibility', 'hidden');
                                                                    $('#dvReportHeader').css('display', 'none');

                                                                    $('#dvDepositMonies').css('visibility', 'visible');
                                                                    $('#dvDepositMonies').css('display', 'block');


                                                                    $('.btnSelectFowst').show();

                                                                }
                                                            });
                                                        }
                                                    });

                                                }
                                            });
                                        },
                                        error: function (vError) {
                                            PromptUserAccess('Z Report', {
                                                OnClickOKSuccess: function () {
                                                    AjaxSend({
                                                        ExactUrl: '/ReportsPOS/SalesReport?cache=1&OrgID=' + sessionStorage.OrgID + '&IsManagerSalesReport=0&IsMobile=' + IsMobile,
                                                        type: 'GET',
                                                        dataType: 'text',
                                                        success: function (vData) {
                                                            $('#divOnPrintPOSSalesReport').empty();
                                                            $('#divOnPrintPOSSalesReport').append(vData).trigger('create');

                                                            App.Deferred.fulfill('FQ26E5TP76');

                                                            setMonthCollection("selPOSSalesReportByDateFromMonth");
                                                            setYearCollection("selPOSSalesReportByDateFromYear");

                                                            setMonthCollection("selPOSSalesReportByDateToMonth");
                                                            setYearCollection("selPOSSalesReportByDateToYear");

                                                            setDashboardDate();

                                                            $('#dvReportHeader').css('visibility', 'hidden');
                                                            $('#dvReportHeader').css('display', 'none');

                                                            $('#dvDepositMonies').css('visibility', 'visible');
                                                            $('#dvDepositMonies').css('display', 'block');


                                                            $('.btnSelectFowst').show();
                                                        }
                                                    });
                                                }
                                            });
                                        }
                                    });

                                    CounterCashierSalesReport = false;
                                } else {
                                    PromptUserAccess('Z Report', {
                                        OnClickOKSuccess: function () {
                                            var isPageReady = false;
                                            var pData = {};
                                            pData.OrgID = sessionStorage["OrgID"];
                                            pData.POSSalesReportType = 0;

                                            AjaxSend({
                                                url: 'ReportsPOS/setPOSSalesReportType',
                                                data: pData,
                                                success: function (vData) {
                                                    if (cashiermanagerpage == "") {
                                                        AjaxSend({
                                                            ExactUrl: '/ReportsPOS/SalesReport?cache=1&OrgID=' + sessionStorage.OrgID + '&IsManagerSalesReport=0&IsMobile=' + IsMobile,
                                                            type: 'GET',
                                                            dataType: 'text',
                                                            success: function (vData) {
                                                                isPageReady = true;
                                                                cashiermanagerpage = vData;
                                                                $('#divOnPrintPOSSalesReport').empty();
                                                                $('#divOnPrintPOSSalesReport').append(vData).trigger('create');

                                                                App.Deferred.fulfill('FQ26E5TP76');

                                                                setMonthCollection("selPOSSalesReportByDateFromMonth");
                                                                setYearCollection("selPOSSalesReportByDateFromYear");

                                                                setMonthCollection("selPOSSalesReportByDateToMonth");
                                                                setYearCollection("selPOSSalesReportByDateToYear");

                                                                setDashboardDate();

                                                                $('#dvReportHeader').css('visibility', 'hidden');
                                                                $('#dvReportHeader').css('display', 'none');

                                                                $('#dvDepositMonies').css('visibility', 'visible');
                                                                $('#dvDepositMonies').css('display', 'block');

                                                                $('.btnSelectFowst').show();

                                                                if (IsMobile) {
                                                                    $('#divDisplayPOSSalesReport').css('margin-left', '0%');
                                                                }
                                                            }
                                                        });
                                                    }
                                                    else {
                                                        $('#divOnPrintPOSSalesReport').empty();
                                                        $('#divOnPrintPOSSalesReport').append(cashiermanagerpage).trigger('create');
                                                        SalesReport.GetAllFowstinSalesReportPage();
                                                    }


                                                }
                                            });
                                        }
                                    });
                                }
                            } else {
                                //Do Nothing Here
                            }

                            $('#divMainDashboard').hide();
                            $('#divSalesAnalytics').hide();
                            $('#divSalesReplenishmentProcessing').hide();
                            $('#divFranchiseRequest').hide();
                            $('#divSalesOrderProcessing').hide();
                            $('#panTransactionDetails').hide();

                            $('#divOnPrintPOSSalesReport').show();


                            $('#panOption').popup('close');
                        }

                        function setDashboardDate() {
                            var m_DateFrom = new Date();
                            var m_DateTo = new Date();

                            $('.fromCalendar').calendar('unsetDate');
                            $('.toCalendar').calendar('unsetDate');

                            var newDateFrom = new Date($('#txtDateFrom').DateCombo('val'));
                            var newDateTo = new Date($('#txtDateTo').DateCombo('val'));
                            GroupDateBy = 'Month';
                            m_DateFrom.setDate(newDateFrom.getMonth() + 1) + '/' + newDateFrom.getDate() + '/' + newDateFrom.getFullYear();
                            m_DateTo.setDate(newDateTo.getMonth() + 1) + '/' + newDateTo.getDate() + '/' + newDateTo.getFullYear();
                            $("#selPOSSalesReportRange").val("Day").selectmenu('refresh');
                            $("#selZReportRange").val("By Date").selectmenu('refresh');


                            $('.fromCalendar').calendar('setDate', m_DateFrom);
                            $('.toCalendar').calendar('setDate', m_DateTo);

                            //for cashier and manager sales report

                            $('#selPOSSalesReportByDateFromMonth').val(m_DateFrom.getMonth() + 1).selectmenu('refresh');
                            $('#selPOSSalesReportByDateFromYear').val(m_DateFrom.getFullYear()).selectmenu('refresh');
                            setDayCollection('selPOSSalesReportByDateFromMonth', 'selPOSSalesReportByDateFromDay', 'selPOSSalesReportByDateFromYear', true);
                            $('#selPOSSalesReportByDateFromDay').val(m_DateFrom.getDate()).selectmenu('refresh');


                            $('#selPOSSalesReportByDateToMonth').val(m_DateTo.getMonth() + 1).selectmenu('refresh');
                            $('#selPOSSalesReportByDateToYear').val(m_DateTo.getFullYear()).selectmenu('refresh');
                            setDayCollection('selPOSSalesReportByDateToMonth', 'selPOSSalesReportByDateToDay', 'selPOSSalesReportByDateToYear', true);
                            $('#selPOSSalesReportByDateToDay').val(m_DateTo.getDate()).selectmenu('refresh');


                            //for z report

                            $('#selZReportByDateMonth').val(m_DateFrom.getMonth() + 1).selectmenu('refresh');
                            $('#selZReportByDateYear').val(m_DateFrom.getFullYear()).selectmenu('refresh');
                            setDayCollection('selZReportByDateMonth', 'selZReportByDateDay', 'selZReportByDateYear', true);
                            $('#selZReportByDateDay').val(m_DateFrom.getDate()).selectmenu('refresh');

                        }

                        function WindowsPrinter(cols1, args1, cols2, args2, cols3, args3) {
                            if ((cols3 == null || cols3 == undefined) && (cols2 != null || cols2 != undefined)) {
                                Android.WindowsPrinting(
                                                        {
                                                            "data": [
                                                                      { "cols1": cols1 },
                                                                      { "args1": [args1[0].toString(), args1[1].toString(), args1[2].toString(), args1[3].toString(), args1[4].toString(), args1[5].toString()] },
                                                                      { "cols2": cols2 },
                                                                      { "args2": [args2[0].toString(), args2[1].toString(), args2[2].toString(), args2[3].toString(), args2[4].toString(), args2[5].toString()] }
                                                            ]
                                                        }
                                );
                            } else if ((cols3 == undefined || cols3 == null) && (cols2 == null || cols2 == undefined)) {
                                Android.WindowsPrinting(
                                                        {
                                                            "data": [
                                                                      { "cols1": cols1 },
                                                                      { "args1": [args1[0].toString(), args1[1].toString(), args1[2].toString(), args1[3].toString(), args1[4].toString(), args1[5].toString()] }
                                                            ]
                                                        }
                                );
                            } else if (cols1 == "newLine") {
                                Android.WindowsPrinting(
                                                        {
                                                            "data": [
                                                                      { "newLine": "newLine" }
                                                            ]
                                                        }
                                );
                            } else if (cols1 == "startPrint") {
                                Android.WindowsPrinting(
                                                        {
                                                            "data": [
                                                                      { "startPrint": "startPrint" }
                                                            ]
                                                        }
                                );
                            } else if (cols1 == "address") {
                                Android.WindowsPrinting(
                                                        {
                                                            "data": [
                                                                      { "address": cols2 }
                                                            ]
                                                        }
                                );
                            } else {
                                Android.WindowsPrinting(
                                                        {
                                                            "data": [
                                                                      { "cols1": cols1 },
                                                                      { "args1": [args1[0].toString(), args1[1].toString(), args1[2].toString(), args1[3].toString(), args1[4].toString(), args1[5].toString()] },
                                                                      { "cols2": cols2 },
                                                                      { "args2": [args2[0].toString(), args2[1].toString(), args2[2].toString(), args2[3].toString(), args2[4].toString(), args2[5].toString()] },
                                                                      { "cols3": cols3 },
                                                                      { "args3": [args3[0].toString(), args3[1].toString(), args3[2].toString(), args3[3].toString(), args3[4].toString(), args3[5].toString()] }
                                                            ]
                                                        }
                                );
                            }
                        }
                    });
                });
        });
});