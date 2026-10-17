require([
    'common',
    'jquery.plugin.html2canvas',
    GetPathFormat('/Scripts3/Browser/LibraryOrganizationRepresentative.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc=')
    ], function () {

    require([(OfflineMode ? '/Scripts3/SalesPayments_offline.js?crc=' : '')], function () {

        $(function () {

            var intFAFCTotalLenght = 40;
            var intFBTotalLenght = 27;
            var intFCDoubleTotalLenght = 18;

            var intDesktopPrintLenght = 45;

            var IsPrintReceipt = false;
            var IsEmailReceipt = false;
            var IsSaleSPaymentSaving = false;
            var IsDeliveryInfoPriceQuote;

            var IsNewSalesPayCheck = true;
            var arSalesPayCheck = new Array();
            var intSalesPayCheckCntr = 0;
            var intSalesPayCheckIndex = 0;

            var intSalesPayCheckRecID = 0;
            var intSalesPayCheckBankID = 1;
            var intSalesPayCheckBank = 2;
            var intSalesPayCheckBankBranchID = 3;
            var intSalesPayCheckBankBranch = 4;
            var intSalesPayCheckAccountName = 5;
            var intSalesPayCheckAccountNo = 6;
            var intSalesPayCheckNumber = 7;
            var intSalesPayCheckDateMonth = 8;
            var intSalesPayCheckDateDay = 9;
            var intSalesPayCheckDateyear = 10;
            var intSalesPayCheckAmount = 11;
            var intSalesPayCheckState = 12;

            var IsNewSalesPayDebitCard = true;
            var arSalesPayDebitCard = new Array();
            var intSalesPayDebitCardCntr = 0;
            var intSalesPayDebitCardIndex = 0;

            var intSalesPayDebitCardRecID = 0;
            var intSalesPayDebitCardType = 1;
            var intSalesPayDebitCardBankID = 2;
            var intSalesPayDebitCardBank = 3;
            var intSalesPayDebitCardHolderLName = 4;
            var intSalesPayDebitCardHolderFName = 5;
            var intSalesPayDebitCardHolderMName = 6;
            var intSalesPayDebitCardHolderGenSuffix = 7;
            var intSalesPayDebitCardBankCountryID = 8;
            var intSalesPayDebitCardBankCountry = 9;
            var intSalesPayDebitCardNumber = 10;
            var intSalesPayDebitCardIssuanceDateMonth = 11;
            var intSalesPayDebitCardIssuanceDateDay = 12;
            var intSalesPayDebitCardIssuanceDateYear = 13;
            var intSalesPayDebitCardExpirationDateMonth = 14;
            var intSalesPayDebitCardExpirationDateDay = 15;
            var intSalesPayDebitCardExpirationDateYear = 16;
            var intSalesPayDebitCardApprovalNumber = 17;
            var intSalesPayDebitCardNoOfInstallment = 18;
            var intSalesPayDebitCardAmount = 19;
            var intSalesPayDebitCardState = 20;

            var IsNewSalesPayCreditCard = true;
            var arSalesPayCreditCard = new Array();
            var intSalesPayCreditCardCntr = 0;
            var intSalesPayCreditCardIndex = 0;

            var intSalesPayCreditCardRecID = 0;
            var intSalesPayCreditCardType = 1;
            var intSalesPayCreditCardBankID = 2;
            var intSalesPayCreditCardBank = 3;
            var intSalesPayCreditCardHolderLName = 4;
            var intSalesPayCreditCardHolderFName = 5;
            var intSalesPayCreditCardHolderMName = 6;
            var intSalesPayCreditCardHolderGenSuffix = 7;
            var intSalesPayCreditCardBankCountryID = 8;
            var intSalesPayCreditCardBankCountry = 9;
            var intSalesPayCreditCardNumber = 10;
            var intSalesPayCreditCardIssuanceDateMonth = 11;
            var intSalesPayCreditCardIssuanceDateDay = 12;
            var intSalesPayCreditCardIssuanceDateYear = 13;
            var intSalesPayCreditCardExpirationDateMonth = 14;
            var intSalesPayCreditCardExpirationDateDay = 15;
            var intSalesPayCreditCardExpirationDateYear = 16;
            var intSalesPayCreditCardApprovalNumber = 17;
            var intSalesPayCreditCardNoOfInstallment = 18;
            var intSalesPayCreditCardAmount = 19;
            var intSalesPayCreditCardState = 20;

            var IsNewSalesPaySRV = true;
            var arSalesPaySRV = new Array();
            var intSalesPaySRVCntr = 0;
            var intSalesPaySRVIndex = 0;

            var intSalesPaySRVRecID = 0;
            var intSalesPaySRVMemoID = 1;
            var intSalesPaySRVNo = 2;
            var intSalesPaySRVPinCode = 3;
            var intSalesPaySRVAmount = 4;
            var intSalesPaySRVConsumedAmount = 5;
            var intSalesPaySRVState = 6;

            var intSalesPaymentEmailCntr = -1;
            var strSalesPaymentEmail = "";

            if (OfflineMode)
                SalesPayment_offline().Test();

            window.SalesPayment = {};

            setMonthCollection("selSalesPayCheckDateMonth");
            setYearCollection("selSalesPayCheckDateYear");

            setMonthCollection("selSalesPayDebitCardIssuanceDateMonth");
            setYearCollection("selSalesPayDebitCardIssuanceDateYear");
            setMonthCollection("selSalesPayDebitCardExpirationDateMonth");
            setYearCollection("selSalesPayDebitCardExpirationDateYear");

            setMonthCollection("selSalesPayCreditCardIssuanceDateMonth");
            setYearCollection("selSalesPayCreditCardIssuanceDateYear");
            setMonthCollection("selSalesPayCreditCardExpirationDateMonth");
            setYearCollection("selSalesPayCreditCardExpirationDateYear");

            setMonthCollection("selSalesPayGiftCertDateOfIssueMonth");
            setYearCollection("selSalesPayGiftCertDateOfIssueYear");
            setMonthCollection("selSalesPayGiftCertExpirationDateMonth");
            setYearCollection("selSalesPayGiftCertExpirationDateYear");

            setMonthCollection("selSalesPayLoyaltyCardIssuanceDateMonth");
            setYearCollection("selSalesPayLoyaltyCardIssuanceDateYear");
            setMonthCollection("selSalesPayLoyaltyCardExpirationDateMonth");
            setYearCollection("selSalesPayLoyaltyCardExpirationDateYear");

            setMonthCollection("selDeliveryInfoPODateMonth");
            setYearCollection("selDeliveryInfoPODateYear");

            setMonthCollection("selPickUpMonth");
            setYearCollection("selPickUpYear");
            setHoursCollection("selPickUpHour");
            setMinutesCollection("selPickUpMin");

            getOrgDiscountType();

            $(document).ready(function () {

                ShowSalesPaymentsInfo();

                window.pageSourcePage = $.mobile.activePage;

            });

            $("#txtSalesPaymentCash").bind('keyup', function () {

                var pSalesPayCash = parseFloat(CDec2($.trim($(this).val().replace(/,/g, ""))));
                $("#lblSalesPayTotalCash").text((pSalesPayCash > 0 ? AccountingFormat(pSalesPayCash) : "0.00"));

                RefreshSalesPaymentTotal();
            });

            $("#txtSalesPaymentCash").keydown(function (e) {
                if (e.keyCode != 110 && e.keyCode != 190) {
                    if (!NumberOnly(e)) return;
                }
            });

            $("#txtSalesPaymentCash").blur(function () {
                var pSalesPayCash = parseFloat(CDec2($("#txtSalesPaymentCash").val().replace(/,/g, "")));
                $("#txtSalesPaymentCash").val((pSalesPayCash > 0 ? AccountingFormat(pSalesPayCash) : ""));
            });

            $('#btnSalesPayCheckBank').click(function () {

                Browser.LibraryOrganizationBanks.Click = function (e) {

                    if(e.IsManualEntry != undefined){
                        if(e.IsManualEntry){
                            ShowHideCheckBank(false);
                        }
                    }else{
                        ShowHideCheckBank(true);

                        $('#txtSalesPayCheckBankID').val(e.ID)
                        $('#btnSalesPayCheckBank').text(e.CompanyName)
                        .button('refresh');

                        $('#txtSalesPayCheckBankBranchID').val('')
                        $('#btnSalesPayCheckBankBranch').text("Please Select")
                        .button('refresh');
                    }

                };
                Browser.LibraryOrganizationBanks.Show();


////////                Browser.LibraryBanks.Click = function (e) {

////////                    if(e.IsManualEntry != undefined){
////////                        if(e.IsManualEntry){

////////                            ShowHideCheckBank(false);
////////                        }
////////                    }else{
////////                        ShowHideCheckBank(true);

////////                        $('#txtSalesPayCheckBankID').val(e.ID)
////////                        $('#btnSalesPayCheckBank').text(e.BankName)
////////                        .button('refresh');

////////                        $('#txtSalesPayCheckBankBranchID').val('')
////////                        $('#btnSalesPayCheckBankBranch').text("Please Select")
////////                        .button('refresh');
////////                    }

////////                };
////////                Browser.LibraryBanks.Show();

            });

            $('#btnSalesPayCheckBankBranch').click(function () {

                Browser.LibraryBankBranches.InputData.BankID = $("#txtSalesPayCheckBankID").val();
                Browser.LibraryBankBranches.Click = function (e) {

                    $('#txtSalesPayCheckBankBranchID').val(e.ID)
                    $('#btnSalesPayCheckBankBranch').text(e.BranchName)
                    .button('refresh');

                };
                Browser.LibraryBankBranches.Show();

            });

            $('#selSalesPayCheckDateMonth').change(function (e) {
                e.preventDefault();

                setDayCollection('selSalesPayCheckDateMonth', 'selSalesPayCheckDateDay', 'selSalesPayCheckDateYear', true);

            });

            $('#selSalesPayCheckDateYear').change(function (e) {
                e.preventDefault();

                setDayCollection('selSalesPayCheckDateMonth', 'selSalesPayCheckDateDay', 'selSalesPayCheckDateYear', true);

            });

            $("#txtSalesPayCheckAmount").keydown(function (e) {
                if (e.keyCode != 110 && e.keyCode != 190) {
                    if (!NumberOnly(e)) return;
                }
            });

            $("#txtSalesPayCheckAmount").blur(function () {
                var pSalesPayCheck = parseFloat(CDec2($("#txtSalesPayCheckAmount").val().replace(/,/g, "")));
                $("#txtSalesPayCheckAmount").val((pSalesPayCheck > 0 ? AccountingFormat(pSalesPayCheck) : "0.00"));
            });

            $('#btnSalesPayAddCheck').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageSalesPayCheck');
                ShowHideCheckBank(true);

                IsNewSalesPayCheck = true
                intSalesPayCheckCntr = intSalesPayCheckCntr + 1;
                intSalesPayCheckIndex = intSalesPayCheckCntr;

                ClearSalesPayCheckFields();

            });

            $('#btnSalesPayCheckSave').click(function (e) {
                e.preventDefault();

                if ($('#divCheckBankButton').is(':visible')){

                    if ($.trim($("#txtSalesPayCheckBankID").val()) == "") {
                        MessageBox("Please select bank.",
                               "Bank",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                        );
                        return;
                    }

//////                    if ($.trim($("#txtSalesPayCheckBankBranchID").val()) == "") {
//////                        MessageBox("Please select bank branch.",
//////                               "Bank Branch",
//////                                [
//////                                    { Text: 'Ok'
//////                                    }
//////                                ]
//////                        );
//////                        return;
//////                    }

                }else{
                    if ($.trim($("#txtSalesPayCheckBankName").val()) == "") {
                        MessageBox("Please enter bank name.",
                               "Bank",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                        );
                        return;
                    }
                }

                if ($.trim($("#txtSalesPayCheckBankBranch").val()) == "") {
                    MessageBox("Please enter bank branch.",
                            "Bank Branch",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                    return;
                }

                if ($.trim($("#txtSalesPayCheckAccountName").val()) == "") {
                    MessageBox("Please enter account name.",
                           "Account Name",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                    return;
                }

                if ($.trim($("#txtSalesPayCheckAccountNo").val()) == "") {
                    MessageBox("Please enter account number.",
                           "Account Number",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                    return;
                }

                if ($.trim($("#txtSalesPayCheckNo").val()) == "") {
                    MessageBox("Please enter check number.",
                           "Check Number",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                    return;
                }

                if ($.trim($("#selSalesPayCheckDateMonth").val()) == "" ||
                    $.trim($("#selSalesPayCheckDateDay").val()) == "" ||
                    $.trim($("#selSalesPayCheckDateYear").val()) == "") {

                    MessageBox("Please enter check date.",
                            "Check Date",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                    return;
                }

                if ($.trim($("#txtSalesPayCheckAmount").val()) == "" || parseFloat(CDec2($.trim($("#txtSalesPayCheckAmount").val().replace(/,/g, ""))) || '0') == 0) {
                    MessageBox("Please enter check amount.",
                           "Check Amount",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                    return;
                }

                SaveSalesPayCheck();

            });

            $('#btnSalesPayCheckRemove').click(function (e) {
                e.preventDefault();

                MessageBox("Remove this record?",
                    'Remove',
                    [
                        { Text: 'Yes',
                            Click: function () {
                                RemoveSalesPayCheck();
                            }
                        }
                    ,
                        { Text: 'No'
                        }
                    ]
                );
            });

            $('#btnSalesPayCheckCancel').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageSalesPayment');

            });

            $('#btnSalesPayDebitCardBank').click(function () {

                Browser.LibraryOrganizationBanks.Click = function (e) {

                    if(e.IsManualEntry != undefined){
                        if(e.IsManualEntry){
                            ShowHideDebitCardBank(false);
                        }
                    }else{
                        ShowHideDebitCardBank(true);

                        $('#txtSalesPayDebitCardBankID').val(e.ID)
                        $('#btnSalesPayDebitCardBank').text(e.CompanyName)
                        .button('refresh');
                    }

                };
                Browser.LibraryOrganizationBanks.Show();

            });

            $('#btnSalesPayDebitCardBankLocation').click(function () {

                Browser.LibraryCountries.Click = function (e) {

                    $('#txtSalesPayDebitCardBankLocationID').val(e.ID)
                    $('#btnSalesPayDebitCardBankLocation').text(e.Name)
                    .button('refresh');

                };
                Browser.LibraryCountries.Show();

            });

            $('#selSalesPayDebitCardIssuanceDateMonth').change(function (e) {
                e.preventDefault();

                setDayCollection('selSalesPayDebitCardIssuanceDateMonth', 'selSalesPayDebitCardIssuanceDateDay', 'selSalesPayDebitCardIssuanceDateYear', true);

            });

            $('#selSalesPayDebitCardIssuanceDateYear').change(function (e) {
                e.preventDefault();

                setDayCollection('selSalesPayDebitCardIssuanceDateMonth', 'selSalesPayDebitCardIssuanceDateDay', 'selSalesPayDebitCardIssuanceDateYear', true);

            });

            $('#selSalesPayDebitCardExpirationDateMonth').change(function (e) {
                e.preventDefault();

                setDayCollection('selSalesPayDebitCardExpirationDateMonth', 'selSalesPayDebitCardExpirationDateDay', 'selSalesPayDebitCardExpirationDateYear', true);

            });

            $('#selSalesPayDebitCardExpirationDateYear').change(function (e) {
                e.preventDefault();

                setDayCollection('selSalesPayDebitCardExpirationDateMonth', 'selSalesPayDebitCardExpirationDateDay', 'selSalesPayDebitCardExpirationDateYear', true);

            });

            $("#txtSalesPayDebitCardNoOfPayments").keydown(function (e) {
                if (!NumberOnly(e)) return;
            });

            $("#txtSalesPayDebitCardTenderedAmount").keydown(function (e) {
                if (e.keyCode != 110 && e.keyCode != 190) {
                    if (!NumberOnly(e)) return;
                }
            });

            $("#txtSalesPayDebitCardTenderedAmount").blur(function () {
                var pSalesPayCC = parseFloat(CDec2($("#txtSalesPayDebitCardTenderedAmount").val().replace(/,/g, "")));
                $("#txtSalesPayDebitCardTenderedAmount").val((pSalesPayCC > 0 ? AccountingFormat(pSalesPayCC) : "0.00"));
            });

            $('#btnSalesPayAddDebitCard').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageSalesPayDebitCard');
                ShowHideDebitCardBank(true);

                IsNewSalesPayDebitCard = true
                intSalesPayDebitCardCntr = intSalesPayDebitCardCntr + 1;
                intSalesPayDebitCardIndex = intSalesPayDebitCardCntr;

                ClearSalesPayDebitCardFields();

            });

            $('#btnSalesPayDebitCardSave').click(function (e) {
                e.preventDefault();

//                if ($.trim($("#selSalesPayDebitCardType").val()) == "") {
//                    MessageBox("Please select Debit card type.",
//                           "Debit Card Type",
//                            [
//                                { Text: 'Ok'
//                                }
//                            ]
//                    );
//                    return;
//                }

                if ($('#divSalesPayDebitCardBankButton').is(':visible')){

                    if ($.trim($("#txtSalesPayDebitCardBankID").val()) == "") {
                        MessageBox("Please select Debit card bank.",
                               "Debit Card Bank",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                        );
                        return;
                    }

                }else{
                    if ($.trim($("#txtSalesPayDebitCardBank").val()) == "") {
                        MessageBox("Please enter bank name.",
                               "Bank",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                        );
                        return;
                    }

                }

                if ($.trim($("#txtSalesPayDebitCardLastName").val()) == "") {
                    MessageBox("Please enter last name of Debit card holder.",
                           "Debit Card Holder",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                    return;
                }

                if ($.trim($("#txtSalesPayDebitCardFirstName").val()) == "") {
                    MessageBox("Please enter first name of Debit card holder.",
                           "Debit Card Holder",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                    return;
                }

                if ($.trim($("#txtSalesPayDebitCardNo").val()) == "") {
                    MessageBox("Please enter Debit card number.",
                           "Debit Card Number",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                    return;
                }

                if ($.trim($("#selSalesPayDebitCardIssuanceDateMonth").val()) != "" ||
                    $.trim($("#selSalesPayDebitCardIssuanceDateDay").val()) != "" ||
                    $.trim($("#selSalesPayDebitCardIssuanceDateYear").val()) != "") {

                    if ($.trim($("#selSalesPayDebitCardIssuanceDateMonth").val()) == "" ||
                        $.trim($("#selSalesPayDebitCardIssuanceDateDay").val()) == "" ||
                        $.trim($("#selSalesPayDebitCardIssuanceDateYear").val()) == "") {

                        MessageBox("Please enter valid Debit card issuance date.",
                               "Issuance Date",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                        );
                        return;
                    }
                }

                if ($.trim($("#selSalesPayDebitCardExpirationDateMonth").val()) != "" ||
                    $.trim($("#selSalesPayDebitCardExpirationDateDay").val()) != "" ||
                    $.trim($("#selSalesPayDebitCardExpirationDateYear").val()) != "") {

                    if ($.trim($("#selSalesPayDebitCardExpirationDateMonth").val()) == "" ||
                        $.trim($("#selSalesPayDebitCardExpirationDateDay").val()) == "" ||
                        $.trim($("#selSalesPayDebitCardExpirationDateYear").val()) == "") {

                        MessageBox("Please enter valid Debit card expiration date.",
                               "Expiration Date",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                        );
                        return;
                    }
                }

                if ($.trim($("#txtSalesPayDebitCardTenderedAmount").val()) == "" || parseFloat(CDec2($.trim($("#txtSalesPayDebitCardTenderedAmount").val().replace(/,/g, ""))) || '0') == 0) {
                    MessageBox("Please enter Debit card amount.",
                           "Amount",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                    return;
                }

                SaveSalesPayDebitCard();
            });

            $('#btnSalesPayDebitCardRemove').click(function (e) {
                e.preventDefault();

                MessageBox("Remove this record?",
                    'Remove',
                    [
                        { Text: 'Yes',
                            Click: function () {
                                RemoveSalesPayDebitCard();
                            }
                        }
                    ,
                        { Text: 'No'
                        }
                    ]
                );

            });

            $('#btnSalesPayDebitCardCancel').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageSalesPayment');
            });

            $('#btnSalesPayCreditCardBank').click(function () {

                Browser.LibraryOrganizationBanks.Click = function (e) {

                    if(e.IsManualEntry != undefined){
                        if(e.IsManualEntry){
                            ShowHideCreditCardBank(false);
                        }
                    }else{
                        ShowHideCreditCardBank(true);

                        $('#txtSalesPayCreditCardBankID').val(e.ID)
                        $('#btnSalesPayCreditCardBank').text(e.CompanyName)
                        .button('refresh');
                    }

                };
                Browser.LibraryOrganizationBanks.Show();

            });

            $('#btnSalesPayCreditCardBankLocation').click(function () {

                Browser.LibraryCountries.Click = function (e) {

                    $('#txtSalesPayCreditCardBankLocationID').val(e.ID)
                    $('#btnSalesPayCreditCardBankLocation').text(e.Name)
                    .button('refresh');

                };
                Browser.LibraryCountries.Show();

            });

            $('#selSalesPayCreditCardIssuanceDateMonth').change(function (e) {
                e.preventDefault();

                setDayCollection('selSalesPayCreditCardIssuanceDateMonth', 'selSalesPayCreditCardIssuanceDateDay', 'selSalesPayCreditCardIssuanceDateYear', true);

            });

            $('#selSalesPayCreditCardIssuanceDateYear').change(function (e) {
                e.preventDefault();

                setDayCollection('selSalesPayCreditCardIssuanceDateMonth', 'selSalesPayCreditCardIssuanceDateDay', 'selSalesPayCreditCardIssuanceDateYear', true);

            });

            $('#selSalesPayCreditCardExpirationDateMonth').change(function (e) {
                e.preventDefault();

                setDayCollection('selSalesPayCreditCardExpirationDateMonth', 'selSalesPayCreditCardExpirationDateDay', 'selSalesPayCreditCardExpirationDateYear', true);

            });

            $('#selSalesPayCreditCardExpirationDateYear').change(function (e) {
                e.preventDefault();

                setDayCollection('selSalesPayCreditCardExpirationDateMonth', 'selSalesPayCreditCardExpirationDateDay', 'selSalesPayCreditCardExpirationDateYear', true);

            });

            $("#txtSalesPayCreditCardNoOfPayments").keydown(function (e) {
                if (!NumberOnly(e)) return;
            });

            $("#txtSalesPayCreditCardTenderedAmount").keydown(function (e) {
                if (e.keyCode != 110 && e.keyCode != 190) {
                    if (!NumberOnly(e)) return;
                }
            });

            $("#txtSalesPayCreditCardTenderedAmount").blur(function () {
                var pSalesPayCC = parseFloat(CDec2($("#txtSalesPayCreditCardTenderedAmount").val().replace(/,/g, "")));
                $("#txtSalesPayCreditCardTenderedAmount").val((pSalesPayCC > 0 ? AccountingFormat(pSalesPayCC) : "0.00"));
            });

            $('#btnSalesPayAddCreditCard').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageSalesPayCreditCard');
                ShowHideCreditCardBank(true);

                IsNewSalesPayCreditCard = true
                intSalesPayCreditCardCntr = intSalesPayCreditCardCntr + 1;
                intSalesPayCreditCardIndex = intSalesPayCreditCardCntr;

                ClearSalesPayCreditCardFields();

            });

            $('#btnSalesPayCreditCardSave').click(function (e) {
                e.preventDefault();

                if ($.trim($("#selSalesPayCreditCardType").val()) == "") {
                    MessageBox("Please select credit card type.",
                           "Credit Card Type",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                    return;
                }

                if ($('#divSalesPayCreditCardBankButton').is(':visible')){

                    if ($.trim($("#txtSalesPayCreditCardBankID").val()) == "") {
                        MessageBox("Please select credit card bank.",
                               "Credit Card Bank",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                        );
                        return;
                    }

                }else{
                    if ($.trim($("#txtSalesPayCreditCardBank").val()) == "") {
                        MessageBox("Please enter bank name.",
                               "Bank",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                        );
                        return;
                    }

                }

                if ($.trim($("#txtSalesPayCreditCardLastName").val()) == "") {
                    MessageBox("Please enter last name of credit card holder.",
                           "Credit Card Holder",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                    return;
                }

                if ($.trim($("#txtSalesPayCreditCardFirstName").val()) == "") {
                    MessageBox("Please enter first name of credit card holder.",
                           "Credit Card Holder",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                    return;
                }

                if ($.trim($("#txtSalesPayCreditCardNo").val()) == "") {
                    MessageBox("Please enter credit card number.",
                           "Credit Card Number",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                    return;
                }

                if ($.trim($("#selSalesPayCreditCardIssuanceDateMonth").val()) != "" ||
                    $.trim($("#selSalesPayCreditCardIssuanceDateDay").val()) != "" ||
                    $.trim($("#selSalesPayCreditCardIssuanceDateYear").val()) != "") {

                    if ($.trim($("#selSalesPayCreditCardIssuanceDateMonth").val()) == "" ||
                        $.trim($("#selSalesPayCreditCardIssuanceDateDay").val()) == "" ||
                        $.trim($("#selSalesPayCreditCardIssuanceDateYear").val()) == "") {

                        MessageBox("Please enter valid credit card issuance date.",
                               "Issuance Date",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                        );
                        return;
                    }
                }

                if ($.trim($("#selSalesPayCreditCardExpirationDateMonth").val()) != "" ||
                    $.trim($("#selSalesPayCreditCardExpirationDateDay").val()) != "" ||
                    $.trim($("#selSalesPayCreditCardExpirationDateYear").val()) != "") {

                    if ($.trim($("#selSalesPayCreditCardExpirationDateMonth").val()) == "" ||
                        $.trim($("#selSalesPayCreditCardExpirationDateDay").val()) == "" ||
                        $.trim($("#selSalesPayCreditCardExpirationDateYear").val()) == "") {

                        MessageBox("Please enter valid credit card expiration date.",
                               "Expiration Date",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                        );
                        return;
                    }
                }

                if ($.trim($("#txtSalesPayCreditCardTenderedAmount").val()) == "" || parseFloat(CDec2($.trim($("#txtSalesPayCreditCardTenderedAmount").val().replace(/,/g, ""))) || '0') == 0) {
                    MessageBox("Please enter credit card amount.",
                           "Amount",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                    return;
                }

                SaveSalesPayCreditCard();
            });

            $('#btnSalesPayCreditCardRemove').click(function (e) {
                e.preventDefault();

                MessageBox("Remove this record?",
                    'Remove',
                    [
                        { Text: 'Yes',
                            Click: function () {
                                RemoveSalesPayCreditCard();
                            }
                        }
                    ,
                        { Text: 'No'
                        }
                    ]
                );

            });

            $('#btnSalesPayCreditCardCancel').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageSalesPayment');
            });

            $("#txtSalesPaySRVNo").keydown(function (e) {
                if (e.keyCode == 13) {
                    $("#btnSalesPaySRVVerify").click();
                } else if (!NumberOnly(e)) {
                    return;
                }
            });

            $('#btnSalesPayScanSRVNumber').click(function () {
                mobile.Scan({
                    Success: function(vData) {
                        $('#txtSalesPaySRVNo').val(vData.Value);
                    }
                });
            });

            $("#txtSalesPaySRVPinCode").keydown(function (e) {
                if (e.keyCode == 13) {
                    $("#btnSalesPaySRVVerify").click();
                }
            });

            $('#btnSalesPayScanSRVPin').click(function () {
                try {
                    mobile.Scan({
                        Success: function(vData) {
                            $('#txtSalesPaySRVPinCode').val(vData.Value);
                        }
                    });
                } catch (e) {
                    alert("Failed to scan: " + e);
                };
            });

            $("#btnSalesPaySRVVerify").click(function (e) {
                LoadSalesPaymentSRVInfo();
            });

            $("#txtSalesPaySRVConsumedAmount").keydown(function (e) {
                if (e.keyCode != 110 && e.keyCode != 190) {
                    if (!NumberOnly(e)) return;
                }
            });

            $("#txtSalesPaySRVConsumedAmount").blur(function () {
                var pSalesPaySRV = parseFloat(CDec2($("#txtSalesPaySRVConsumedAmount").val().replace(/,/g, "")));
                $("#txtSalesPaySRVConsumedAmount").val((pSalesPaySRV > 0 ? AccountingFormat(pSalesPaySRV) : "0.00"));
            });

            $('#btnSalesPayAddSRV').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageSalesPaySRV');

                IsNewSalesPaySRV = true
                intSalesPaySRVCntr = intSalesPaySRVCntr + 1;
                intSalesPaySRVIndex = intSalesPaySRVCntr;

                ClearSalesPaySRVFields();
            });

            $('#btnSalesPaySRVSave').click(function (e) {
                e.preventDefault();

                if ($.trim($("#txtSalesPaySRVTransMemoID").val()) == "") {
                    MessageBox("Please enter SRV Number.",
                           "SRV Number",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                    return;
                }

                if ($("#txtSalesPaySRVNo").val() == "") {
                    MessageBox("Please enter SRV number.",
                                "SRV Number",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                            );
                     
                    $("#txtSalesPaySRVTransMemoID").val('');
                    $("#txtSalesPaySRVAmount").val('0.00');
                    $("#txtSalesPaySRVConsumedAmount").val('0.00');

                    return;
                }

                if ($("#txtSalesPaySRVPinCode").val() == "") {
                    MessageBox("Please enter SRV pin code.",
                                "SRV Pin Code",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                            );

                    $("#txtSalesPaySRVTransMemoID").val('');
                    $("#txtSalesPaySRVAmount").val('0.00');
                    $("#txtSalesPaySRVConsumedAmount").val('0.00');

                    return;
                }

                if ($.trim($("#txtSalesPaySRVConsumedAmount").val()) == "" || parseFloat(CDec2($.trim($("#txtSalesPaySRVConsumedAmount").val().replace(/,/g, ""))) || '0') == 0) {
                    MessageBox("Please enter SRV amount to consume.",
                           "SRV Amount",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                    return;
                }

                if (parseFloat(CDec2($.trim($("#txtSalesPaySRVConsumedAmount").val().replace(/,/g, "")))) > parseFloat(CDec2($.trim($("#txtSalesPaySRVAmount").val().replace(/,/g, ""))))){
                    MessageBox("Sorry! You only have " + $("#txtSalesPaySRVAmount").val() + " on your SRV credit.",
                           "SRV Amount",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                    return;
                }

                SaveSalesPaySRV();

            });

            $('#btnSalesPaySRVRemove').click(function (e) {
                e.preventDefault();

                MessageBox("Remove this record?",
                    'Remove',
                    [
                        { Text: 'Yes',
                            Click: function () {
                                RemoveSalesPaySRV();
                            }
                        }
                    ,
                        { Text: 'No'
                        }
                    ]
                );

            });

            $('#btnSalesPaySRVCancel').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageSalesPayment');

            });

            $('#btnSalesPayAddGC').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageSalesPayGiftCert');

            });

            $('#btnSalesPayGiftCertSave').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageSalesPayment');

            });

            $('#btnSalesPayGiftCertRemove').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageSalesPayment');

            });

            $('#btnSalesPayGiftCertCancel').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageSalesPayment');

            });

            $('#btnSalesPayAddLoyaltyCard').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageSalesPayLoyaltyCard');

            });

            $('#btnSalesPayLoyaltyCardSave').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageSalesPayment');

            });

            $('#btnSalesPayLoyaltyCardRemove').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageSalesPayment');

            });

            $('#btnSalesPayLoyaltyCardCancel').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageSalesPayment');

            });

            $('#btnOrderBy').click(function () {

                Browser.LibraryOrganizationRepresentative.InputData.OrganizationID = $("#txtSalesPayCustomerID").val();
                Browser.LibraryOrganizationRepresentative.Click = function (e) {

                    $('#txtOrderedByID').val(e.ID);
                    $('#btnOrderBy').text(e.RepresentativeName)
                        .button('refresh');

                    $('#txtDeliveryInfoTelNo').val(e.TelNo);
                    $('#txtDeliveryInfoFaxNo').val(e.FaxNo);
                    $('#txtDeliveryInfoMobileNo').val(e.MobileNo);
                    $('#txtDeliveryInfoEmail').val(e.Email);

                };
                Browser.LibraryOrganizationRepresentative.Show();

            });

            $('#btnBillTo').click(function () {

                Browser.LibraryFowst.InputData.OrganizationID = $("#txtSalesPayCustomerID").val();
                Browser.LibraryFowst.Click = function (e) {

                    $('#txtBillToID').val(e.ID)
                    $('#btnBillTo').text(e.Address)
                    .button('refresh');

                };
                Browser.LibraryFowst.Show();

            });

            $('#btnShipTo').click(function () {

                Browser.LibraryFowst.InputData.OrganizationID = $("#txtSalesPayCustomerID").val();
                Browser.LibraryFowst.Click = function (e) {

                    $('#txtShipToID').val(e.ID)
                    $('#btnShipTo').text(e.Address)
                    .button('refresh');

                };
                Browser.LibraryFowst.Show();

            });

            $('#btnPickLocation').click(function () {

                AjaxSend({
                    url: "DeliveryInfo/getCurrentSubcriberOrgID",
                    success: function (vData) {
                        if (vData.DeliveryInfoData != "") {
                            Browser.LibraryFowst.InputData.OrganizationID = vData.DeliveryInfoData;
                            Browser.LibraryFowst.Click = function (e) {

                                $('#txtPickUpLocationID').val(e.ID)
                                $('#btnPickLocation').text(e.BranchName)
                                .button('refresh');

                            };
                            Browser.LibraryFowst.Show();
                        }
                    }
                });

            });

            $('#selDeliveryMode').change(function (e) {
                e.preventDefault();

                SetDeliveryMode();

            });

            $('#rdoRush').click(function () {
                SetDeliveryDownTown($(this).is(":checked"));
            });

            $('#rdoNotRush').click(function () {
                SetDeliveryDownTown(!$(this).is(":checked"));

            });

            $("#txtDeliverWithIn").keydown(function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            });

            $('#rdoNormalRoute').click(function () {

                SetDeliveryOutOfTown($(this).is(":checked"));

            });

            $('#rdoBus').click(function () {

                SetDeliveryOutOfTown(!$(this).is(":checked"));

            });

            $('#rdoCustomerIsWaiting').click(function () {

                SetPickUp($(this).is(":checked"));

            });

            $('#rdoDateAndTime').click(function () {

                SetPickUp(!$(this).is(":checked"));

            });

            $('#selPickUpMonth').change(function (e) {
                e.preventDefault();

                setDayCollection('selPickUpMonth', 'selPickUpDay', 'selPickUpYear', true);

            });

            $('#selPickUpYear').change(function (e) {
                e.preventDefault();

                setDayCollection('selPickUpMonth', 'selPickUpDay', 'selPickUpYear', true);

            });

            $('#butSaveReceipt').click(function (e) {
                e.preventDefault();

                IsPrintReceipt = false;
                IsEmailReceipt = false;
                IsDeliveryInfoPriceQuote = false;

                SaveDeliveryInfo();

            });

            $('#btnSalesPaymentDone').click(function (e) {
                e.preventDefault();

                IsPrintReceipt = true;
                IsEmailReceipt = false;
                IsDeliveryInfoPriceQuote = false;

                SaveDeliveryInfo();

            });

            $('#btnSalesPaymentBack').click(function (e) {
                e.preventDefault();

                history.back();

            });

            $('#btnSalesPaymentEmail').click(function (e) {
                e.preventDefault();

                IsPrintReceipt = false;
                IsEmailReceipt = true;
                IsDeliveryInfoPriceQuote = false;

                SaveDeliveryInfo();

            });

            $('#btnDeliveryInfoPriceQoute').click(function (e) {
                e.preventDefault();


                IsPrintReceipt = false;
                IsEmailReceipt = true;
                IsDeliveryInfoPriceQuote = true;

                SaveDeliveryInfo();

            });

            $('#btnSalesPaymentPrintEmail').click(function (e) {
                e.preventDefault();

                IsPrintReceipt = true;
                IsEmailReceipt = true;
                IsDeliveryInfoPriceQuote = false;

                SaveDeliveryInfo();

            });

            $('#btnSalesPaymentEmailOK').click(function (e) {
                e.preventDefault();

                strSalesPaymentEmail = "";
                for (var i = 0; i <= intSalesPaymentEmailCntr; i++) {

                    if ($("#chkEmail-" + i).is(":checked")) {
                        var pEmailData = $("#chkEmail-" + i).data('Data');

                        strSalesPaymentEmail = strSalesPaymentEmail + (strSalesPaymentEmail.length > 0 ? ", " : "") + pEmailData.EmailAddress
                    }

                }

                if ($.trim($("#txtSalesPayEmailAddress").val()) == "" && strSalesPaymentEmail == "") {
                    MessageBox("Please enter email address.",
                           "Email Address",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                    return;
                } else {
                    if ($.trim($("#txtSalesPayEmailAddress").val()).length > 0) {
                        strSalesPaymentEmail = strSalesPaymentEmail + (strSalesPaymentEmail.length > 0 ? ", " : "") + $.trim($("#txtSalesPayEmailAddress").val())
                    }
                }

                SaveDeliveryInfo();

            });

            $('#btnSalesPaymentEmailCancel').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageSalesPayment');

            });

            $('#selDiscountType').change(function (e) {
                e.preventDefault();
                
                var strAccessType = "";

                if($('#selDiscountType').val().toUpperCase() == '35F954A0-C67C-499C-AF60-F08E64477FDC'){
                    strAccessType = "Allow to grant VIP Discount";
                }else if($('#selDiscountType').val().toUpperCase() == '764EDACB-0131-4033-8E44-7B903368008C'){
                    strAccessType = "Allow to grant Regular Discount 1";
                }else if($('#selDiscountType').val().toUpperCase() == 'C4199347-6A00-45C9-BE11-8934DD63ECD1'){
                    strAccessType = "Allow to grant Regular Discount 2";
                }else if($('#selDiscountType').val().toUpperCase() == '8E2DCA9C-433F-42FC-A644-04476DBA056A'){
                    strAccessType = "Allow to grant Regular Discount 3";
                }else if($('#selDiscountType').val().toUpperCase() == '7804A35A-B531-4380-945E-FC9643E99843'){
                    strAccessType = "Allow to grant Employee Discount";
                }

                if(strAccessType == ""){
                    $('#selDiscountType').val('').selectmenu('refresh');
                    $("#txtDiscountPercent").val("0.00");

                    RefreshTotalDiscount();
                    RefreshSalesPaymentTotal();
                }else{
                    window.pageSecuritySourcePage = $.mobile.activePage;
                    PromptUserAccess(strAccessType, {
                        OnClickOKSuccess: function () {

                            $.mobile.changePage(window.pageSecuritySourcePage);
                            getOrgDiscountPercentRefresh($('#selDiscountType').val());

                            RefreshTotalDiscount();
                            RefreshSalesPaymentTotal();

                        },
                        OnFailed: function () {
                            $.mobile.changePage(window.pageSecuritySourcePage);
                            $('#selDiscountType').val('').selectmenu('refresh');
                            $("#txtDiscountPercent").val("0.00");

                            RefreshTotalDiscount();
                            RefreshSalesPaymentTotal();
                        }
                    });
                }

            });

            $("#txtDiscountPercent").keydown(function (e) {
                if (e.keyCode != 110 && e.keyCode != 190) {
                    if (!NumberOnly(e)) return;
                }
            });

            $("#txtDiscountPercent").bind('keyup', function () {

                var pSalesPayDiscount = parseFloat(($.trim($(this).val().replace(/,/g, "")) == '' ? '0' : $.trim($(this).val().replace(/,/g, ""))));
                var pSalesPayDiscountCeiling = parseFloat(CDec2($.trim($("#txtDiscountPercentCeiling").val().replace(/,/g, ""))));

                if(parseFloat(CDec2(pSalesPayDiscountCeiling)) < pSalesPayDiscount && pSalesPayDiscount > 0){

                    pSalesPayDiscount = parseFloat(CDec2($.trim($("#txtDiscountPercentCeiling").val().replace(/,/g, ""))));

                    MessageBox("Sorry. You are allowed to give maximum of " + AccountingFormat(pSalesPayDiscount) + "% discount only.",
                           $("#selDiscountType option:selected").text(),
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );

                    $("#txtDiscountPercent").val((pSalesPayDiscount > 0 ? AccountingFormat(pSalesPayDiscount) : "0.00"));
                }

                RefreshTotalDiscount();
                RefreshSalesPaymentTotal();

            });

            function ShowHideCheckBank(IsShow){

                $('#txtSalesPayCheckBankID').val('')
                $('#btnSalesPayCheckBank').text("Please Select").button('refresh');
                $('#txtSalesPayCheckBankName').val('')

                $('#txtSalesPayCheckBankBranchID').val('')
                $('#btnSalesPayCheckBankBranch').text("Please Select").button('refresh');
                $('#txtSalesPayCheckBankBranch').val('')
                
                $("#divCheckBankButton").hide();
                $("#divCheckBankBranchButton").hide();
                $("#divCheckBankTextbox").hide();
                $("#divCheckBankBranchTextbox").hide();

                if(IsShow){
                    $("#divCheckBankButton").show();
////                    $("#divCheckBankBranchButton").show();
                }else{
                    $("#divCheckBankTextbox").show();
////                    $("#divCheckBankBranchTextbox").show();
                }

                $("#divCheckBankBranchTextbox").show();
            }

            function ShowHideDebitCardBank(IsShow){
                $('#txtSalesPayDebitCardBankID').val('')
                $('#btnSalesPayDebitCardBank').text("Please Select").button('refresh');
                $('#txtSalesPayDebitCardBank').val('')

                $("#divSalesPayDebitCardBankButton").hide();
                $("#divSalesPayDebitCardBankTextbox").hide();

                if(IsShow){
                    $("#divSalesPayDebitCardBankButton").show();
                }else{
                    $("#divSalesPayDebitCardBankTextbox").show();
                }
            }

            function ShowHideCreditCardBank(IsShow){
                $('#txtSalesPayCreditCardBankID').val('')
                $('#btnSalesPayCreditCardBank').text("Please Select").button('refresh');
                $('#txtSalesPayCreditCardBank').val('')

                $("#divSalesPayCreditCardBankButton").hide();
                $("#divSalesPayCreditCardBankTextbox").hide();

                if(IsShow){
                    $("#divSalesPayCreditCardBankButton").show();
                }else{
                    $("#divSalesPayCreditCardBankTextbox").show();
                }
            }

            function getOrgDiscountType() {

                AjaxSend({
                    url: 'DiscountSettings/getDiscountSettings',
                    success: function (vData) {

                        var vRows = vData.DiscountData.Rows;

                        $('#selDiscountType').empty();

                        $('#selDiscountType').append(new Option('Please Select', ''));
                        for (var p = 0; p <= vRows.length - 1; p++) {
                            $('#selDiscountType').append(new Option(vRows[p].DiscountType, vRows[p].ID, false, false));
                        }
                    },
                    error: function (vError) {
                        var pDummy;
                    }
                });

            }

            function getOrgDiscountPercentRefresh(strDiscountID) {
                var pData={}
                pData.DiscountID = strDiscountID

                AjaxSend({
                    url: 'DiscountSettings/getDiscountSettingsInfo',
                    data: pData,
                    success: function (vData) {

                        if(vData.DiscountData.Rows.length > 0){
                            $('#txtDiscountPercentCeiling').val(AccountingFormat(vData.DiscountData.Rows[0].PercentDiscount));
                            $('#txtDiscountPercent').val(AccountingFormat(vData.DiscountData.Rows[0].PercentDiscount));
                            RefreshTotalDiscount();
                            RefreshSalesPaymentTotal();
                        }

                    },
                    error: function (vError) {
                        var pDummy;
                    }
                });

            }

            function RefreshTotalDiscount() {
                var dblDiscount = 0;
                
                dblDiscount = parseFloat(CDec2($('#lblSalesPaySubTotal').text().replace(/,/g, ""))) * (parseFloat(CDec2($('#txtDiscountPercent').val().replace(/,/g, "")))/100);

                $("#lblSalesPayTotalDiscount").text(AccountingFormat(dblDiscount));
                $("#lblSalesPayCheckTotalDiscount").text(AccountingFormat(dblDiscount));
                $("#lblSalesPayDebitCardTotalDiscount").text(AccountingFormat(dblDiscount));
                $("#lblSalesPayCreditCardTotalDiscount").text(AccountingFormat(dblDiscount));
                $("#lblSalesPaySRVTotalDiscount").text(AccountingFormat(dblDiscount));
            }

            function SetDeliveryMode() {
                var pParams = GetUrlParameters();

                $('#divDeliverDownTown').hide();
                $('#divDeliverOutOfTown').hide();
                $('#divPickup').hide();

                if ($('#selDeliveryMode').val() == "Deliver (Downtown)") {
                    $('#divDeliverDownTown').show();
                }
                else if ($('#selDeliveryMode').val() == "Deliver (Out Of Town)") {
                    $('#divDeliverOutOfTown').show();
                }
                else if ($('#selDeliveryMode').val() == "Pick Up") {
                    $('#divPickup').show();

                    $('#divPickUpType').show();
                    $("#rdoCustomerIsWaiting").attr("checked", true).checkboxradio("refresh");
                    $("#rdoDateAndTime").attr("checked", false).checkboxradio("refresh");
                    SetPickUp(true);
                }

            }

            function SetDeliveryDownTown(IsRush) {

                $('#divRush').hide();
                $('#divNotRush').hide();
                if (IsRush) {
                    $('#divRush').show();
                }
                else {
                    $('#txtDeliverWithIn').val('0');
                    $('#selHourMinute').val('').selectmenu('refresh');
                    $('#divNotRush').show();
                }

            }

            function SetDeliveryOutOfTown(IsNormalRoute) {

                $('#divBus').hide();
                if (!IsNormalRoute) {
                    $('#divBus').show();
                }

            }

            function SetPickUp(IsCustomerWaiting) {

                $('#divCustomerIsWaiting').hide();
                $('#divDateAndTime').hide();
                if (IsCustomerWaiting) {
                    $('#selPickUpMonth').val('').selectmenu('refresh');
                    $('#selPickUpDay').val('').selectmenu('refresh');
                    $('#selPickUpYear').val('').selectmenu('refresh');
                    $('#selPickUpHour').val('').selectmenu('refresh');
                    $('#selPickUpMin').val('').selectmenu('refresh');
                    $('#selPickUpAMPM').val('').selectmenu('refresh');

                    $('#divCustomerIsWaiting').show();
                }
                else {
                    $('#divDateAndTime').show();
                }
            }

            function ShowSalesPaymentsInfo() {
                var pData = {};
                var pParams = GetUrlParameters();

                pData.InvoiceID = pParams.InvoiceID;

                ClearSalesPaymentsInfoFields();

                AjaxSend({
                    url: "SalesPayments/getInvoiceTotalAmountDue",
                    data: pData,
                    success: function (vData) {

                        $("#txtSalesPayCustomerID").val(vData.CustomerID || '');
                        $("#lblSalesPaySubTotal").text(AccountingFormat(vData.TotalAmountDue || 0));
                        
                        $("#lblSalesPayCheckSubTotal").text(AccountingFormat(vData.TotalAmountDue || 0));
                        $("#lblSalesPayDebitCardSubTotal").text(AccountingFormat(vData.TotalAmountDue || 0));
                        $("#lblSalesPayCreditCardSubTotal").text(AccountingFormat(vData.TotalAmountDue || 0));
                        $("#lblSalesPaySRVSubTotal").text(AccountingFormat(vData.TotalAmountDue || 0));

                        $('#pSkuCount').text(vData.SkuCount);
                        $('#pTotalQuantity').text(vData.TotalQuantity);
                        if (CStr2(vData.RefNum).trim() != '')
                            $('#divSaveReceipt').show();
                        RefreshSalesPaymentTotal();

                        //Delivery Info
                        IsDeliveryInfoProceedToPayments = false;
                        IsDeliveryInfoPriceQuote = false;

                        //default Value for POS Pickup
                        $('#selDeliveryMode').val('Pick Up').selectmenu('refresh');
                        SetDeliveryMode();

                        ShowDeliveryInfo();

                    }
                });

            };

            function ShowDeliveryInfo() {
                var pData = { FowstID: sessionStorage.FowstID };
                var pParams = GetUrlParameters();
                pData.InvoiceID = pParams.InvoiceID;

                ClearDeliveryInfoFields();

                AjaxSend({
                    url: "DeliveryInfo/getDeliveryInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.Error) {
                            MessageBox(vData.Error);
                            return;
                        };
                        if (vData.DeliveryInfoData.Rows.length > 0) {

                            $("#txtOrgCustomer").val(vData.DeliveryInfoData.Rows[0].CustomerName || '');

                            $("#txtOrderedByID").val(vData.DeliveryInfoData.Rows[0].RepresentativeID || '');
                            $("#btnOrderBy").text(vData.DeliveryInfoData.Rows[0].Representative || 'Please Select').button('refresh');

                            $("#txtBillToID").val(vData.DeliveryInfoData.Rows[0].BillingAddressID || '');
                            $("#btnBillTo").text(vData.DeliveryInfoData.Rows[0].BillingAddress || 'Please Select').button('refresh');

                            $("#txtShipToID").val(vData.DeliveryInfoData.Rows[0].DeliveryAddressID || '');
                            $("#btnShipTo").text(vData.DeliveryInfoData.Rows[0].DeliveryAddress || 'Please Select').button('refresh');

                            $("#txtDeliveryInfoTelNo").val(vData.DeliveryInfoData.Rows[0].TelNo || '');
                            $("#txtDeliveryInfoFaxNo").val(vData.DeliveryInfoData.Rows[0].FaxNo || '');
                            $("#txtDeliveryInfoMobileNo").val(vData.DeliveryInfoData.Rows[0].MobileNo || '');
                            $("#txtDeliveryInfoEmail").val(vData.DeliveryInfoData.Rows[0].Email || '');

                            $("#txtPickUpLocationID").val(vData.DeliveryInfoData.Rows[0].PickUpLocationID || '');
                            $("#btnPickLocation").text(vData.DeliveryInfoData.Rows[0].PickUpLocation || 'Please Select').button('refresh');

                            //Set Delivery Method
                            if (vData.DeliveryInfoData.Rows[0].DeliveryMethod == 'Deliver') {
                                if (vData.DeliveryInfoData.Rows[0].IsDownTown == 1) {
                                    $("#selDeliveryMode").val('Deliver (Downtown)').selectmenu('refresh');
                                }
                                else {
                                    $("#selDeliveryMode").val('Deliver (Out Of Town)').selectmenu('refresh');
                                }
                            }
                            else {
                                $("#selDeliveryMode").val('Pick Up').selectmenu('refresh');
                            }
                            SetDeliveryMode();

                            //Set if Rush or Not
                            if (vData.DeliveryInfoData.Rows[0].IsRushDelivery == 1) {
                                $("#rdoRush").attr("checked", true).checkboxradio("refresh");
                                $("#rdoNotRush").attr("checked", false).checkboxradio("refresh");
                                $("#txtDeliverWithIn").val(vData.DeliveryInfoData.Rows[0].DeliverWithin || '');
                                $("#selHourMinute").val('Working Minutes').selectmenu('refresh');
                                SetDeliveryDownTown(true);
                            }
                            else {
                                $("#rdoRush").attr("checked", false).checkboxradio("refresh");
                                $("#rdoNotRush").attr("checked", true).checkboxradio("refresh");
                                SetDeliveryDownTown(false);
                            }

                            //Set Normal Route, Bus or Customer id Waiting
                            if (vData.DeliveryInfoData.Rows[0].DeliveryType == 'Normal Route Delivery') {
                                $("#rdoNormalRoute").attr("checked", true).checkboxradio("refresh");
                                $("#rdoBus").attr("checked", false).checkboxradio("refresh");
                                SetDeliveryOutOfTown(true);
                            }
                            else if (vData.DeliveryInfoData.Rows[0].DeliveryType == 'Bus') {
                                $("#rdoNormalRoute").attr("checked", false).checkboxradio("refresh");
                                $("#rdoBus").attr("checked", true).checkboxradio("refresh");
                                SetDeliveryOutOfTown(false);
                            }
                            else if (vData.DeliveryInfoData.Rows[0].DeliveryType == 'Customer Is Waiting') {
                                $("#rdoCustomerIsWaiting").attr("checked", true).checkboxradio("refresh");
                                $("#rdoDateAndTime").attr("checked", false).checkboxradio("refresh");
                                SetPickUp(true);
                            }
                            else {
                                if (vData.DeliveryInfoData.Rows[0].DeliveryMethod.toLowerCase() == 'pickup') {
                                    $("#rdoCustomerIsWaiting").attr("checked", false).checkboxradio("refresh");
                                    $("#rdoDateAndTime").attr("checked", true).checkboxradio("refresh");

                                    $("#selPickUpMonth").val(vData.DeliveryInfoData.Rows[0].PickupDateMonth || '').selectmenu('refresh');
                                    $("#selPickUpYear").val(vData.DeliveryInfoData.Rows[0].PickupDateYear || '').selectmenu('refresh');
                                    setDayCollection('selPickUpMonth', 'selPickUpDay', 'selPickUpYear', true);
                                    $("#selPickUpDay").val(vData.DeliveryInfoData.Rows[0].PickupDateDay || '').selectmenu('refresh');

                                    $("#selPickUpHour").val(vData.DeliveryInfoData.Rows[0].PickupDateHour || '').selectmenu('refresh');
                                    $("#selPickUpMin").val(vData.DeliveryInfoData.Rows[0].PickupDateMinute || '').selectmenu('refresh');
                                    $("#selPickUpAMPM").val(vData.DeliveryInfoData.Rows[0].PickupDateAMPM || '').selectmenu('refresh');

                                    SetPickUp(false);
                                }
                            }

                            //Get OutOfTownBus Default
                            AjaxSend({
                                url: "DeliveryInfo/getOutOfTownBusDefault",
                                data: pData,
                                success: function (vData) {

                                    $("#lblOutOfTownBus").text('Maximum service time : ' + vData.OutOfTownBusDefault + ' hour(s)' || 'Maximum service time : 2 hour(s)');

                                }
                            });

                            //Get Pick Up Customer Is Waiting Default
                            AjaxSend({
                                url: "DeliveryInfo/getPickUpCustomerIsWaitingDefault",
                                data: pData,
                                success: function (vData) {

                                    $("#lblPickUpCustomerIsWaiting").text('Maximum service time : ' + vData.PickUpCustomerIsWaitingDefault + ' Minute(s)' || 'Maximum service time : 30 Minute(s)');

                                }
                            });

                            AjaxSend({
                                url: "Browser/LibraryOrganizationRepresentative",
                                data: { Top1: 1 },
                                success: function (vData) {
                                    var pDR = vData.Results.Rows[0];
                                    if (pDR == undefined) return;
                                    $('#txtOrderedByID').val(pDR.ID);
                                    $('#btnOrderBy').text(pDR.RepresentativeName)
                                        .button('refresh');

                                    $('#txtDeliveryInfoTelNo').val(pDR.TelNo);
                                    $('#txtDeliveryInfoFaxNo').val(pDR.FaxNo);
                                    $('#txtDeliveryInfoMobileNo').val(pDR.MobileNo);
                                    $('#txtDeliveryInfoEmail').val(pDR.Email);
                                }
                            });

                        }
                    }
                });

            };

            function LoadSalesPayCheck(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arSalesPayCheck = new Array();
                    arSalesPayCheck[intSalesPayCheckRecID] = vRows[p].ID;
                    arSalesPayCheck[intSalesPayCheckBankID] = vRows[p].BankID;
                    arSalesPayCheck[intSalesPayCheckBank] = vRows[p].BankName;
                    arSalesPayCheck[intSalesPayCheckBankBranchID] = vRows[p].BankBranchID;
                    arSalesPayCheck[intSalesPayCheckBankBranch] = vRows[p].BankBranchName;
                    arSalesPayCheck[intSalesPayCheckAccountName] = vRows[p].AccountName;
                    arSalesPayCheck[intSalesPayCheckAccountNo] = vRows[p].AccountNumber;
                    arSalesPayCheck[intSalesPayCheckNumber] = vRows[p].CheckNumber;

                    arSalesPayCheck[intSalesPayCheckDateMonth] = vRows[p].CheckDateMonth;
                    arSalesPayCheck[intSalesPayCheckDateDay] = vRows[p].CheckDateDay;
                    arSalesPayCheck[intSalesPayCheckDateyear] = vRows[p].CheckDateYear;

                    arSalesPayCheck[intSalesPayCheckAmount] = AccountingFormat(vRows[p].CheckAmount);
                    arSalesPayCheck[intSalesPayCheckState] = "";

                    sessionStorage["SalesPayCheck-" + p] = JSON.stringify(arSalesPayCheck);

                };

                intSalesPayCheckCntr = vRows.length;
                LoadSalesPayCheckList();

            };

            function LoadSalesPayDebitCard(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arSalesPayDebitCard = new Array();
                    arSalesPayDebitCard[intSalesPayDebitCardRecID] = vRows[p].ID;
                    arSalesPayDebitCard[intSalesPayDebitCardType] = vRows[p].DebitCardType;
                    arSalesPayDebitCard[intSalesPayDebitCardBankID] = vRows[p].DebitCardBankID;
                    arSalesPayDebitCard[intSalesPayDebitCardBank] = vRows[p].DebitCardBankName;
                    arSalesPayDebitCard[intSalesPayDebitCardHolderLName] = vRows[p].HolderLastName;
                    arSalesPayDebitCard[intSalesPayDebitCardHolderFName] = vRows[p].HolderFirstName;
                    arSalesPayDebitCard[intSalesPayDebitCardHolderMName] = vRows[p].HolderMiddleName;
                    arSalesPayDebitCard[intSalesPayDebitCardHolderGenSuffix] = vRows[p].HolderGenerationalSuffix;
                    arSalesPayDebitCard[intSalesPayDebitCardBankCountryID] = vRows[p].BankCountryID;
                    arSalesPayDebitCard[intSalesPayDebitCardBankCountry] = vRows[p].BankCountry;

                    arSalesPayDebitCard[intSalesPayDebitCardNumber] = vRows[p].DebitCardNumber;
                    arSalesPayDebitCard[intSalesPayDebitCardIssuanceDateMonth] = vRows[p].DebitCardIssuanceDateMonth;
                    arSalesPayDebitCard[intSalesPayDebitCardIssuanceDateDay] = vRows[p].DebitCardIssuanceDateDay;
                    arSalesPayDebitCard[intSalesPayDebitCardIssuanceDateYear] = vRows[p].DebitCardIssuanceDateYear;
                    arSalesPayDebitCard[intSalesPayDebitCardExpirationDateMonth] = vRows[p].DebitCardExpirationDateMonth;
                    arSalesPayDebitCard[intSalesPayDebitCardExpirationDateDay] = vRows[p].DebitCardExpirationDateDay;
                    arSalesPayDebitCard[intSalesPayDebitCardExpirationDateYear] = vRows[p].DebitCardExpirationDateYear;

                    arSalesPayDebitCard[intSalesPayDebitCardApprovalNumber] = vRows[p].ApprovalNo;
                    arSalesPayDebitCard[intSalesPayDebitCardNoOfInstallment] = vRows[p].NoOfInstallmentPayments;
                    arSalesPayDebitCard[intSalesPayDebitCardAmount] = AccountingFormat(vRows[p].DebitCardAmount);
                    arSalesPayDebitCard[intSalesPayDebitCardState] = "";

                    sessionStorage["SalesPayDebitCard-" + p] = JSON.stringify(arSalesPayDebitCard);

                };

                intSalesPayDebitCardCntr = vRows.length;
                LoadSalesPayDebitCardList();

            };

            function LoadSalesPayCreditCard(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arSalesPayCreditCard = new Array();
                    arSalesPayCreditCard[intSalesPayCreditCardRecID] = vRows[p].ID;
                    arSalesPayCreditCard[intSalesPayCreditCardType] = vRows[p].CreditCardType;
                    arSalesPayCreditCard[intSalesPayCreditCardBankID] = vRows[p].CreditCardBankID;
                    arSalesPayCreditCard[intSalesPayCreditCardBank] = vRows[p].CreditCardBankName;
                    arSalesPayCreditCard[intSalesPayCreditCardHolderLName] = vRows[p].HolderLastName;
                    arSalesPayCreditCard[intSalesPayCreditCardHolderFName] = vRows[p].HolderFirstName;
                    arSalesPayCreditCard[intSalesPayCreditCardHolderMName] = vRows[p].HolderMiddleName;
                    arSalesPayCreditCard[intSalesPayCreditCardHolderGenSuffix] = vRows[p].HolderGenerationalSuffix;
                    arSalesPayCreditCard[intSalesPayCreditCardBankCountryID] = vRows[p].BankCountryID;
                    arSalesPayCreditCard[intSalesPayCreditCardBankCountry] = vRows[p].BankCountry;

                    arSalesPayCreditCard[intSalesPayCreditCardNumber] = vRows[p].CreditCardNumber;
                    arSalesPayCreditCard[intSalesPayCreditCardIssuanceDateMonth] = vRows[p].CreditCardIssuanceDateMonth;
                    arSalesPayCreditCard[intSalesPayCreditCardIssuanceDateDay] = vRows[p].CreditCardIssuanceDateDay;
                    arSalesPayCreditCard[intSalesPayCreditCardIssuanceDateYear] = vRows[p].CreditCardIssuanceDateYear;
                    arSalesPayCreditCard[intSalesPayCreditCardExpirationDateMonth] = vRows[p].CreditCardExpirationDateMonth;
                    arSalesPayCreditCard[intSalesPayCreditCardExpirationDateDay] = vRows[p].CreditCardExpirationDateDay;
                    arSalesPayCreditCard[intSalesPayCreditCardExpirationDateYear] = vRows[p].CreditCardExpirationDateYear;

                    arSalesPayCreditCard[intSalesPayCreditCardApprovalNumber] = vRows[p].ApprovalNo;
                    arSalesPayCreditCard[intSalesPayCreditCardNoOfInstallment] = vRows[p].NoOfInstallmentPayments;
                    arSalesPayCreditCard[intSalesPayCreditCardAmount] = AccountingFormat(vRows[p].CreditCardAmount);
                    arSalesPayCreditCard[intSalesPayCreditCardState] = "";

                    sessionStorage["SalesPayCreditCard-" + p] = JSON.stringify(arSalesPayCreditCard);

                };

                intSalesPayCreditCardCntr = vRows.length;
                LoadSalesPayCreditCardList();

            };

            function LoadSalesPaySRV(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arSalesPaySRV = new Array();
                    arSalesPaySRV[intSalesPaySRVRecID] = vRows[p].ID;
                    arSalesPaySRV[intSalesPaySRVMemoID] = vRows[p].MemoID;
                    arSalesPaySRV[intSalesPaySRVNo] = vRows[p].SRVNo;
                    arSalesPaySRV[intSalesPaySRVPinCode] = vRows[p].PinCode;
                    arSalesPaySRV[intSalesPaySRVAmount] = AccountingFormat(vRows[p].SRVAmount);
                    arSalesPaySRV[intSalesPaySRVConsumedAmount] = AccountingFormat(vRows[p].ConsumedAmount);
                    arSalesPaySRV[intSalesPaySRVState] = "";

                    sessionStorage["SalesPaySRV-" + p] = JSON.stringify(arSalesPaySRV);

                };

                intSalesPaySRVCntr = vRows.length;
                LoadSalesPaySRVList();

            };

            function ShowSalesPayCheckInfo(arData, index) {

                $.mobile.changePage('#pageSalesPayCheck');

                ClearSalesPayCheckFields();

                intSalesPayCheckIndex = index;

                $("#txtSalesPayCheckID").val(arData[intSalesPayCheckRecID] || '');

                if(arData[intSalesPayCheckBankID] == ""){
                    ShowHideCheckBank(false);
                    $("#txtSalesPayCheckBankName").val(arData[intSalesPayCheckBank] || '');
                }else{
                    ShowHideCheckBank(true);
                    $("#txtSalesPayCheckBankID").val(arData[intSalesPayCheckBankID] || '');
                    $("#btnSalesPayCheckBank").text(arData[intSalesPayCheckBank] || 'Please Select').button('refresh');
                    $("#txtSalesPayCheckBankBranchID").val(arData[intSalesPayCheckBankBranchID] || '');
                    $("#btnSalesPayCheckBankBranch").text(arData[intSalesPayCheckBankBranch] || 'Please Select').button('refresh');
                }

                $("#txtSalesPayCheckBankBranch").val(arData[intSalesPayCheckBankBranch] || '');

                $("#txtSalesPayCheckAccountName").val(arData[intSalesPayCheckAccountName] || '');
                $("#txtSalesPayCheckAccountNo").val(arData[intSalesPayCheckAccountNo] || '');
                $("#txtSalesPayCheckNo").val(arData[intSalesPayCheckNumber] || '');

                $("#selSalesPayCheckDateMonth").val(arData[intSalesPayCheckDateMonth] || '').selectmenu('refresh');
                $("#selSalesPayCheckDateDay").val(arData[intSalesPayCheckDateDay] || '').selectmenu('refresh');
                $("#selSalesPayCheckDateYear").val(arData[intSalesPayCheckDateyear] || '').selectmenu('refresh');

                $("#txtSalesPayCheckAmount").val(arData[intSalesPayCheckAmount] || '');

                IsNewSalesPayCheck = false;

                $("#btnSalesPayCheckRemove").show();

            };

            function ShowSalesPayDebitCardInfo(arData, index) {

                $.mobile.changePage('#pageSalesPayDebitCard');

                ClearSalesPayDebitCardFields();

                intSalesPayDebitCardIndex = index;

                $("#txtSalesPayDebitCardID").val(arData[intSalesPayDebitCardRecID] || '');
                $("#selSalesPayDebitCardType").val(arData[intSalesPayDebitCardType] || '').selectmenu('refresh');

                if(arData[intSalesPayDebitCardBankID] == ""){
                    ShowHideDebitCardBank(false);
                    $("#txtSalesPayDebitCardBank").val(arData[intSalesPayDebitCardBank] || '');
                }else{
                    ShowHideDebitCardBank(true);
                    $("#txtSalesPayDebitCardBankID").val(arData[intSalesPayDebitCardBankID] || '');
                    $("#btnSalesPayDebitCardBank").text(arData[intSalesPayDebitCardBank] || 'Please Select').button('refresh');
                }

                $("#txtSalesPayDebitCardBankLocationID").val(arData[intSalesPayDebitCardBankCountryID] || '');
                $("#btnSalesPayDebitCardBankLocation").text(arData[intSalesPayDebitCardBankCountry] || 'Please Select').button('refresh');

                $("#txtSalesPayDebitCardLastName").val(arData[intSalesPayDebitCardHolderLName] || '');
                $("#txtSalesPayDebitCardFirstName").val(arData[intSalesPayDebitCardHolderFName] || '');
                $("#txtSalesPayDebitCardMiddleName").val(arData[intSalesPayDebitCardHolderMName] || '');
                $("#selSalesPayDebitPersonalSuffixName").val(arData[intSalesPayDebitCardHolderGenSuffix] || '').selectmenu('refresh');

                $("#txtSalesPayDebitCardNo").val(arData[intSalesPayDebitCardNumber] || '');

                $("#selSalesPayDebitCardIssuanceDateMonth").val(arData[intSalesPayDebitCardIssuanceDateMonth] || '').selectmenu('refresh');
                $("#selSalesPayDebitCardIssuanceDateDay").val(arData[intSalesPayDebitCardIssuanceDateDay] || '').selectmenu('refresh');
                $("#selSalesPayDebitCardIssuanceDateYear").val(arData[intSalesPayDebitCardIssuanceDateYear] || '').selectmenu('refresh');
                $("#selSalesPayDebitCardExpirationDateMonth").val(arData[intSalesPayDebitCardExpirationDateMonth] || '').selectmenu('refresh');
                $("#selSalesPayDebitCardExpirationDateDay").val(arData[intSalesPayDebitCardExpirationDateDay] || '').selectmenu('refresh');
                $("#selSalesPayDebitCardExpirationDateYear").val(arData[intSalesPayDebitCardExpirationDateYear] || '').selectmenu('refresh');

                $("#txtSalesPayDebitCardApprovalNo").val(arData[intSalesPayDebitCardApprovalNumber] || '');
                $("#txtSalesPayDebitCardNoOfPayments").val(arData[intSalesPayDebitCardNoOfInstallment] || '');

                $("#txtSalesPayDebitCardTenderedAmount").val(arData[intSalesPayDebitCardAmount] || '');

                IsNewSalesPayDebitCard = false;

                $("#btnSalesPayDebitCardRemove").show();

            };

            function ShowSalesPayCreditCardInfo(arData, index) {

                $.mobile.changePage('#pageSalesPayCreditCard');

                ClearSalesPayCreditCardFields();

                intSalesPayCreditCardIndex = index;

                $("#txtSalesPayCreditCardID").val(arData[intSalesPayCreditCardRecID] || '');
                $("#selSalesPayCreditCardType").val(arData[intSalesPayCreditCardType] || '').selectmenu('refresh');

                if(arData[intSalesPayCreditCardBankID] == ""){
                    ShowHideCreditCardBank(false);
                    $("#txtSalesPayCreditCardBank").val(arData[intSalesPayCreditCardBank] || '');
                }else{
                    ShowHideCreditCardBank(true);
                    $("#txtSalesPayCreditCardBankID").val(arData[intSalesPayCreditCardBankID] || '');
                    $("#btnSalesPayCreditCardBank").text(arData[intSalesPayCreditCardBank] || 'Please Select').button('refresh');
                }

                $("#txtSalesPayCreditCardBankLocationID").val(arData[intSalesPayCreditCardBankCountryID] || '');
                $("#btnSalesPayCreditCardBankLocation").text(arData[intSalesPayCreditCardBankCountry] || 'Please Select').button('refresh');

                $("#txtSalesPayCreditCardLastName").val(arData[intSalesPayCreditCardHolderLName] || '');
                $("#txtSalesPayCreditCardFirstName").val(arData[intSalesPayCreditCardHolderFName] || '');
                $("#txtSalesPayCreditCardMiddleName").val(arData[intSalesPayCreditCardHolderMName] || '');
                $("#selSalesPayPersonalSuffixName").val(arData[intSalesPayCreditCardHolderGenSuffix] || '').selectmenu('refresh');

                $("#txtSalesPayCreditCardNo").val(arData[intSalesPayCreditCardNumber] || '');

                $("#selSalesPayCreditCardIssuanceDateMonth").val(arData[intSalesPayCreditCardIssuanceDateMonth] || '').selectmenu('refresh');
                $("#selSalesPayCreditCardIssuanceDateDay").val(arData[intSalesPayCreditCardIssuanceDateDay] || '').selectmenu('refresh');
                $("#selSalesPayCreditCardIssuanceDateYear").val(arData[intSalesPayCreditCardIssuanceDateYear] || '').selectmenu('refresh');
                $("#selSalesPayCreditCardExpirationDateMonth").val(arData[intSalesPayCreditCardExpirationDateMonth] || '').selectmenu('refresh');
                $("#selSalesPayCreditCardExpirationDateDay").val(arData[intSalesPayCreditCardExpirationDateDay] || '').selectmenu('refresh');
                $("#selSalesPayCreditCardExpirationDateYear").val(arData[intSalesPayCreditCardExpirationDateYear] || '').selectmenu('refresh');

                $("#txtSalesPayCreditCardApprovalNo").val(arData[intSalesPayCreditCardApprovalNumber] || '');
                $("#txtSalesPayCreditCardNoOfPayments").val(arData[intSalesPayCreditCardNoOfInstallment] || '');

                $("#txtSalesPayCreditCardTenderedAmount").val(arData[intSalesPayCreditCardAmount] || '');

                IsNewSalesPayCreditCard = false;

                $("#btnSalesPayCreditCardRemove").show();

            };

            function ShowSalesPaySRVInfo(arData, index) {

                $.mobile.changePage('#pageSalesPaySRV');

                ClearSalesPaySRVFields();

                intSalesPaySRVIndex = index;

                $("#txtSalesPayTransSRVID").val(arData[intSalesPaySRVRecID] || '');

                $("#txtSalesPaySRVTransMemoID").val(arData[intSalesPaySRVMemoID] || '');

                $("#txtSalesPaySRVNo").val(arData[intSalesPaySRVNo]);
                $("#txtSalesPaySRVPinCode").val(arData[intSalesPaySRVPinCode]);

                $("#txtSalesPaySRVAmount").val(arData[intSalesPaySRVAmount] || '');
                $("#txtSalesPaySRVConsumedAmount").val(arData[intSalesPaySRVConsumedAmount] || '');

                IsNewSalesPaySRV = false;

                $("#btnSalesPaySRVRemove").show();


            };

            function LoadSalesPaymentSRVInfo() {
                var pData = {};

                if ($("#txtSalesPaySRVNo").val() == "") {
                    MessageBox("Please enter SRV number.",
                                "SRV Number",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                            );

                    $("#txtSalesPaySRVTransMemoID").val('');
                    $("#txtSalesPaySRVAmount").val('0.00');
                    $("#txtSalesPaySRVConsumedAmount").val('0.00');


                    return;
                }

                if ($("#txtSalesPaySRVPinCode").val() == "") {
                    MessageBox("Please enter SRV pin code.",
                                "SRV Pin Code",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                            );

                    $("#txtSalesPaySRVTransMemoID").val('');
                    $("#txtSalesPaySRVAmount").val('0.00');
                    $("#txtSalesPaySRVConsumedAmount").val('0.00');


                    return;
                }

                pData.SRVNumber = $("#txtSalesPaySRVNo").val();
                pData.SRVPin = $("#txtSalesPaySRVPinCode").val();

                AjaxSend({
                    url: "SalesPayments/getSalesPaymentSRVInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.MemoID != "") {
                            $("#txtSalesPaySRVTransMemoID").val(vData.MemoID || '');
                            $("#txtSalesPaySRVAmount").val(AccountingFormat(vData.SRVAmount) || '0.00');

                            if(CDec2($("#lblSalesPaySRVAmountLacking").text()) <= CDec2(vData.SRVAmount)){
                                $("#txtSalesPaySRVConsumedAmount").val($("#lblSalesPaySRVAmountLacking").text());
                            }else{
                                $("#txtSalesPaySRVConsumedAmount").val(AccountingFormat(vData.SRVAmount) || '0.00');
                            }

                        } else {

                            $("#txtSalesPaySRVTransMemoID").val('');
                            $("#txtSalesPaySRVAmount").val('0.00');
                            $("#txtSalesPaySRVConsumedAmount").val('0.00');

                            MessageBox("SRV number not found.",
                                "SRV Number",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                            );
                        }

                    }
                });
            }

            function ClearSalesPaymentsInfoFields() {
                // dont clear
                // sessionStorage.clear();
                proj.ClearSessionStorage();

                $("#lblSalesPaySubTotal").text('0.00');
                $("#lblSalesPayTotalDiscount").text('0.00');
                $("#lblSalesPayCharges").text('0.00');

                $("#lblSalesPayAmountDue").text('0.00');

                $("#lblSalesPayTotalAmountTendered").text('0.00');
                $("#lblSalesPayAmountLacking").text('0.00');
                $("#lblSalesPayTotalChangeAmount").text('0.00');

                $("#lblSalesPayTotalCash").text('0.00');
                $("#txtSalesPaymentCash").val('');

                $("#lblSalesPayTotalCheck").text('0.00');
                $("#lblSalesPayTotalDebitCard").text('0.00');
                $("#lblSalesPayTotalCreditCard").text('0.00');
                $("#lblSalesPayTotalSRV").text('0.00');
                $("#lblSalesPayTotalGiftCert").text('0.00');
                $("#lblSalesPayTotalLoyaltyCard").text('0.00');
                $("#lblSalesPayTotalCreditTerms").text('0.00');

                $("#txtDiscountPercentCeiling").val('0.00');
                $("#txtDiscountPercent").val('0.00');
                
            }

            function ClearSalesPayCheckFields() {

                $("#txtSalesPayCheckID").val('');

                $("#txtSalesPayCheckBankID").val('');
                $("#btnSalesPayCheckBank").text('Please Select').button('refresh');
                $("#txtSalesPayCheckBankName").val('');

                $("#txtSalesPayCheckBankBranchID").val('');
                $("#btnSalesPayCheckBankBranch").text('Please Select').button('refresh');
                $("#txtSalesPayCheckBankBranch").val('');

                $("#txtSalesPayCheckAccountName").val('');
                $("#txtSalesPayCheckAccountNo").val('');
                $("#txtSalesPayCheckNo").val('');

                $("#selSalesPayCheckDateMonth").val('').selectmenu('refresh');
                $("#selSalesPayCheckDateDay").val('').selectmenu('refresh');
                $("#selSalesPayCheckDateYear").val('').selectmenu('refresh');

                $("#txtSalesPayCheckAmount").val('0.00');

                $("#btnSalesPayCheckRemove").hide();

            }

            function ClearSalesPayDebitCardFields() {

                $("#txtSalesPayDebitCardID").val('');

                $("#selSalesPayDebitCardType").val('').selectmenu('refresh');

                $("#txtSalesPayDebitCardBankID").val('');
                $("#btnSalesPayDebitCardBank").text('Please Select').button('refresh');
                $("#txtSalesPayDebitCardBank").val('');

                $("#txtSalesPayDebitCardBankLocationID").val('');
                $("#btnSalesPayDebitCardBankLocation").text('Please Select').button('refresh');

                $("#txtSalesPayDebitCardLastName").val('');
                $("#txtSalesPayDebitCardFirstName").val('');
                $("#txtSalesPayDebitCardMiddleName").val('');
                $("#selSalesPayDebitPersonalSuffixName").val('').selectmenu('refresh');
                $("#txtSalesPayDebitCardNo").val('');

                $("#selSalesPayDebitCardIssuanceDateMonth").val('').selectmenu('refresh');
                $("#selSalesPayDebitCardIssuanceDateDay").val('').selectmenu('refresh');
                $("#selSalesPayDebitCardIssuanceDateYear").val('').selectmenu('refresh');

                $("#selSalesPayDebitCardExpirationDateMonth").val('').selectmenu('refresh');
                $("#selSalesPayDebitCardExpirationDateDay").val('').selectmenu('refresh');
                $("#selSalesPayDebitCardExpirationDateYear").val('').selectmenu('refresh');

                $("#txtSalesPayDebitCardApprovalNo").val('');
                $("#txtSalesPayDebitCardNoOfPayments").val('1');
                $("#txtSalesPayDebitCardTenderedAmount").val('0.00');

                $("#btnSalesPayDebitCardRemove").hide();

            }

            function ClearSalesPayCreditCardFields() {

                $("#txtSalesPayCreditCardID").val('');

                $("#selSalesPayCreditCardType").val('').selectmenu('refresh');

                $("#txtSalesPayCreditCardBankID").val('');
                $("#btnSalesPayCreditCardBank").text('Please Select').button('refresh');
                $("#txtSalesPayCreditCardBank").val('');

                $("#txtSalesPayCreditCardBankLocationID").val('');
                $("#btnSalesPayCreditCardBankLocation").text('Please Select').button('refresh');

                $("#txtSalesPayCreditCardLastName").val('');
                $("#txtSalesPayCreditCardFirstName").val('');
                $("#txtSalesPayCreditCardMiddleName").val('');
                $("#selSalesPayPersonalSuffixName").val('').selectmenu('refresh');
                $("#txtSalesPayCreditCardNo").val('');

                $("#selSalesPayCreditCardIssuanceDateMonth").val('').selectmenu('refresh');
                $("#selSalesPayCreditCardIssuanceDateDay").val('').selectmenu('refresh');
                $("#selSalesPayCreditCardIssuanceDateYear").val('').selectmenu('refresh');

                $("#selSalesPayCreditCardExpirationDateMonth").val('').selectmenu('refresh');
                $("#selSalesPayCreditCardExpirationDateDay").val('').selectmenu('refresh');
                $("#selSalesPayCreditCardExpirationDateYear").val('').selectmenu('refresh');

                $("#txtSalesPayCreditCardApprovalNo").val('');
                $("#txtSalesPayCreditCardNoOfPayments").val('1');
                $("#txtSalesPayCreditCardTenderedAmount").val('0.00');

                $("#btnSalesPayCreditCardRemove").hide();

            }

            function ClearSalesPaySRVFields() {

                $("#txtSalesPayTransSRVID").val('');

                $("#txtSalesPaySRVTransMemoID").val('');
                $("#txtSalesPaySRVNo").val('');
                $("#txtSalesPaySRVPinCode").val('');

                $("#txtSalesPaySRVAmount").val('0.00');
                $("#txtSalesPaySRVConsumedAmount").val('0.00');

                $("#btnSalesPaySRVRemove").hide();

            }

            function ClearDeliveryInfoFields() {

                $("#txtOrgCustomer").val('');

                $("#txtOrderedByID").val('');
                $("#btnOrderBy").text('Please Select').button('refresh');

                $("#txtBillToID").val('');
                $("#btnBillTo").text('Please Select').button('refresh');

                $("#txtShipToID").val('');
                $("#btnShipTo").text('Please Select').button('refresh');

                $("#txtDeliveryInfoTelNo").val('');
                $("#txtDeliveryInfoFaxNo").val('');
                $("#txtDeliveryInfoMobileNo").val('');
                $("#txtDeliveryInfoEmail").val('');

                $("#txtPickUpLocationID").val('');
                $("#btnPickLocation").text('Please Select').button('refresh');

                $("#selDeliveryMode").val('');
                SetDeliveryMode();

                $("#txtDeliverWithIn").val('');
                $("#selHourMinute").val('').selectmenu('refresh');
                $("#rdoNotRush").attr("checked", true).checkboxradio("refresh");
                SetDeliveryDownTown(false);

                $("#rdoNormalRoute").attr("checked", true).checkboxradio("refresh");
                SetDeliveryOutOfTown(true);

                $("#rdoCustomerIsWaiting").attr("checked", true).checkboxradio("refresh");
                SetPickUp(true);

                $("#selPickUpMonth").val('').selectmenu('refresh');
                $("#selPickUpDay").val('').selectmenu('refresh');
                $("#selPickUpYear").val('').selectmenu('refresh');
                $("#selPickUpHour").val('').selectmenu('refresh');
                $("#selPickUpMin").val('').selectmenu('refresh');
                $("#selPickUpAMPM").val('').selectmenu('refresh');

                $("#selHoldOrder").val('0').slider("refresh");

            }

            function LoadCustomerEmailList(vRows) {
                var pLV = $('#fldsetSalesPayEmail');
                var pItem;

                intSalesPaymentEmailCntr = vRows.length - 1;

                pLV.html('');
                for (var i = 0; i <= vRows.length - 1; i++) {

                    pItem = $('<input>')
                                .attr("type", 'checkbox')
                                .attr('id', 'chkEmail-' + i)
                                .attr('checked', false)
                                .data('Data', vRows[i])
                            .append($('<label>')
                                .attr('for', 'chkEmail-' + i)
                                .text((vRows[i].EmailAddress || 'No Email Address'))
                            );

                    pLV.append(pItem);
                }

                $('#divSalesPaymentsEmail').trigger("create");

                if (vRows.length <= 0) {
                    $("#divSalesPaymentEmailNone").show();
                } else {
                    $("#divSalesPaymentEmailNone").hide();
                }

                $.mobile.changePage("#divSalesPaymentPromptEmail");
                return;

            };

            function SaveDeliveryInfo() {
                var pData = [];
                var pDataEmail = [];
                var pPODate = "";
                var pDeliveryType = "";
                var pDeliverWithin = 0;
                var pPickupDate = "";
                var pParams = GetUrlParameters();

                if(!IsDeliveryInfoPriceQuote) {
                    if($.trim($('#lblSalesPayTotalAmountTendered').text()) == "-"){
                        MessageBox("Please pay full amount due.",
                               "Insufficient Payment",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                        );
                        return;
                    }

                    if ((parseFloat(CDec2($('#lblSalesPaySubTotal').text().replace(/,/g, ""))) - parseFloat(CDec2($('#lblSalesPayTotalDiscount').text().replace(/,/g, "")))) > parseFloat(CDec2($('#lblSalesPayTotalAmountTendered').text().replace(/,/g, "")))) {
                        MessageBox("Please pay full amount due.",
                               "Insufficient Payment",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                        );
                        return;
                    }
                }
                
                if($("#chkForGovReport").is(":checked")) {

                    if ($.trim($("#txtOrderedByID").val()) == "") {
                        MessageBox("Please select customer representative.",
                                    "Customer Representative",
                                    [
                                        { Text: 'Ok' }
                                    ]
                        );
                        return;
                    }

                }

                if ($.trim($("#selDeliveryMode").val()) == "") {
                    MessageBox("Please select delivery mode.",
                                "Delivery Mode",
                                [
                                    { Text: 'Ok' }
                                ]
                        );
                    return;
                }
                else {
                    if ($.trim($("#selDeliveryMode").val()) == "Deliver (Downtown)") {

                        if ($.trim($("#txtBillToID").val()) == "") {
                            MessageBox("Please select billing address.",
                                    "Billing Address",
                                    [
                                        { Text: 'Ok' }
                                    ]
                            );
                            return;
                        }

                        if ($.trim($("#txtShipToID").val()) == "") {
                            MessageBox("Please select delivery address.",
                                    "Delivery Address",
                                    [
                                        { Text: 'Ok' }
                                    ]
                            );
                            return;
                        }

                        if ($("#rdoRush").is(":checked")) {
                            if ($.trim($("#txtDeliverWithIn").val()) == "") {

                                MessageBox("Please enter minute(s) or hour(s) value.",
                                        "Hours or Minutes",
                                        [
                                            { Text: 'Ok' }
                                        ]
                                    );

                                return;
                            }

                            if (!$.isNumeric($("#txtDeliverWithIn").val())) {
                                MessageBox("Please enter valid minute(s) or hour(s) value.",
                                            "Hours or Minutes",
                                            [
                                                { Text: 'Ok'
                                                }
                                            ]
                                    );
                                return;
                            }

                            if ($.trim($("#selHourMinute").val()) == "") {

                                MessageBox("Please select if value is in minutes or hours.",
                                        "Hours or Minutes",
                                        [
                                            { Text: 'Ok' }
                                        ]
                                    );

                                return;
                            }

                        }
                    }

                    if ($.trim($("#selDeliveryMode").val()) == "Pick Up") {
                        if ($("#rdoDateAndTime").is(":checked")) {

                            if ($.trim($("#selPickUpMonth").val()) == "" ||
                                    $.trim($("#selPickUpDay").val()) == "" ||
                                    $.trim($("#selPickUpYear").val()) == "" ||
                                    $.trim($("#selPickUpHour").val()) == "" ||
                                    $.trim($("#selPickUpMin").val()) == "" ||
                                    $.trim($("#selPickUpAMPM").val()) == "") {

                                MessageBox("Please enter valid pick up date and time.",
                                                "Pick Up Date and Time",
                                                [
                                                    { Text: 'Ok' }
                                                ]
                                    );
                                return;
                            }

                        }
                    }

                }

                pDeliverWithin = $("#txtDeliverWithIn").val();
                if ($.trim($("#selHourMinute").val()) == "Working Hours") {
                    pDeliverWithin = $("#txtDeliverWithIn").val() * 60;
                }

                if ($.trim($("#selDeliveryMode").val()) == "Deliver (Out Of Town)") {
                    if ($("#rdoNormalRoute").is(":checked")) {
                        pDeliveryType = "Normal Route Delivery"
                    }
                    else if ($("#rdoBus").is(":checked")) {
                        pDeliveryType = "Bus"
                    }
                }
                else if ($.trim($("#selDeliveryMode").val()) == "Pick Up") {
                    if ($("#rdoCustomerIsWaiting").is(":checked")) {
                        pDeliveryType = "Customer Is Waiting"
                    }
                }

                if ($.trim($("#selPickUpMonth").val()) != "" &&
                    $.trim($("#selPickUpDay").val()) != "" &&
                    $.trim($("#selPickUpYear").val()) != "" &&
                    $.trim($("#selPickUpHour").val()) != "" &&
                    $.trim($("#selPickUpMin").val()) != "" &&
                    $.trim($("#selPickUpAMPM").val()) != "") {

                    pPickupDate = $("#selPickUpMonth").val() + '/' + $("#selPickUpDay").val() + '/' + $("#selPickUpYear").val() + " " + $("#selPickUpHour").val() + ":" + $("#selPickUpMin").val() + ":00 " + $("#selPickUpAMPM").val();
                }

                if (IsDeliveryInfoPriceQuote) {

                    if (strSalesPaymentEmail == "") {

                        pDataEmail = {
                            CustomerID: $("#txtSalesPayCustomerID").val()
                        };

                        AjaxSend({
                            url: "SalesPayments/getCustomerEmails",
                            data: pDataEmail,
                            success: function (vDataEmail) {
                                LoadCustomerEmailList(vDataEmail.SalesPaymentData.Rows);
                            }
                        });

                        return;
                    }
                }

                pData = {
                    ID: pParams.InvoiceID,
                    CustomerID: $.trim($("#txtSalesPayCustomerID").val()),
                    CusRepresentativeID: $.trim($("#txtOrderedByID").val()),
                    BillToID: $.trim($("#txtBillToID").val()),
                    ShipToID: $.trim($("#txtShipToID").val()),
                    EmailAddress: $.trim(strSalesPaymentEmail),
                    PickUpLocationID: $.trim($("#txtPickUpLocationID").val()),
                    DeliveryMethod: ($.trim($("#selDeliveryMode").val()) == "Deliver (Downtown)" || $.trim($("#selDeliveryMode").val()) == "Deliver (Out Of Town)" ? "Deliver" : "Pickup"),
                    IsDownTown: ($.trim($("#selDeliveryMode").val()) == "Deliver (Downtown)" ? "1" : "0"),
                    IsRushDelivery: ($("#rdoRush").is(":checked") ? "1" : "0"),
                    DeliverWithin: pDeliverWithin,
                    DeliveryType: pDeliveryType,
                    PickupDate: pPickupDate,
                };

                AjaxSend({
                    url: "DeliveryInfo/SaveInvoiceDeliveryInfo",
                    data: pData,
                    success: function (vSaveData) {

                        if (vSaveData.DeliveryInfoData == "InvalidEmail") {
                            MessageBox("Please enter valid email address.",
                                    "Invalid email",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                            );
                            return;
                        }
                        else if (vSaveData.DeliveryInfoData == "MinutesOrHoursError") {
                            MessageBox("Please enter valid minutes or hours value.",
                                    "Minutes or Hours",
                                    [
                                        { Text: 'Ok' }
                                    ]
                                    );
                            return;
                        }
                        else if (vSaveData.DeliveryInfoData == "PickUpDateAndTimeError") {
                            MessageBox("Please select valid pick up date and time.",
                                    "Pick Up Date and Time",
                                    [
                                        { Text: 'Ok' }
                                    ]
                                    );
                            return;
                        }

                        localStorage.removeItem('CartCustomerID');

                        if (IsDeliveryInfoPriceQuote) {
                            EmailQuotation(pData);
                        }
                        else{
                            VerifySalesPayment();
                        }
                    }
                });

            };

            function EmailQuotation(pData) {

                AjaxSend({
                    url: "DeliveryInfo/SaveInvoiceToQuotation",
                    data: pData,
                    success: function (vSaveQuoteData) {

                        MessageBox("Please wait while sending the price quote.",
                                            "Email Price Quote",
                                            [
                                                { Text: 'Ok'
                                                }
                                            ]
                                    );

                        var pEmailQuoteData = [];
                        pEmailQuoteData = {
                            QuotationID: vSaveQuoteData.DeliveryInfoData,
                            EmailAddress: $.trim(strSalesPaymentEmail)
                        };

                        AjaxSend({
                            url: "DeliveryInfo/SendEmailQuotation",
                            data: pEmailQuoteData,
                            success: function (vSendQuoteData) {
                                MessageBox("Your transaction has been saved. An email price quote has been sent to the email address(es) that you specified.",
                                        "Email Price Quote",
                                        [
                                            { Text: 'Ok',
                                                Click: function () {
                                                    window.location = GetPathFormat("/SalesMenu?mode=pos&OrgID=" + sessionStorage.OrgID);
                                                }
                                            }
                                        ]
                                );

                            }
                        });
                    }
                });

            };

            function VerifySalesPayment() {
                var pParams = GetUrlParameters();
                var pInvoiceID = pParams.InvoiceID;

                if ((parseFloat(CDec2($('#lblSalesPayAmountDue').text().replace(/,/g, ""))) - parseFloat(CDec2($('#lblSalesPayTotalDiscount').text().replace(/,/g, "")))) > parseFloat(CDec2($('#lblSalesPayTotalAmountTendered').text().replace(/,/g, "")))) {
                    MessageBox("Please pay full amount due.",
                           "Insufficient Payment",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                    return;
                }

                sessionStorage.CartOnHoldCheckPayment = 0;
                if (parseFloat(CDec2($('#lblSalesPayAmountDue').text().replace(/,/g, ""))) < parseFloat(CDec2($('#lblSalesPayTotalCheck').text().replace(/,/g, "")))) {
                    MessageBox("The check amount exceeds the total amount due. Do you want to proceed with the payment or consume the amount by adding items to cart?",
                           "Check Payment",
                            [
                                { Text: 'Proceed',
                                    Click: function () {
                                        $.mobile.changePage(window.pageSourcePage);
                                        SaveSalesPayment(pParams.InvoiceID, true);
                                        return;
                                    }
                                }
                            ,
                                { Text: 'Hold transaction while customer adds items to cart',
                                    Click: function () {
                                        sessionStorage.CartOnHoldCheckPayment = 1;
                                        sessionStorage.CartOnHoldCheckPaymentOrderID = pParams.InvoiceID;
                                        window.location = GetPathFormat("/SalesMenu?mode=pos&OrgID=" + sessionStorage.OrgID);
                                        return;
                                    }
                                }
                            ]
                    );
                    return;
                }

                SaveSalesPayment(pParams.InvoiceID, false);

            };

            function SaveSalesPayment(pInvoiceID, blnIsCheckChangeTY){
                var pData = [];
                var pDataDiscount = [];
                var pDataEmail = [];
                var pDataCheck = [];
                var pDataDebitCard = [];
                var pDataCreditCard = [];
                var pDataSRV = [];
                var intIndex;

                if (IsEmailReceipt) {
                    if (strSalesPaymentEmail == "") {

                        pDataEmail = {
                            CustomerID: $("#txtSalesPayCustomerID").val()
                        };

                        AjaxSend({
                            url: "SalesPayments/getCustomerEmails",
                            data: pDataEmail,
                            success: function (vDataEmail) {

                                LoadCustomerEmailList(vDataEmail.SalesPaymentData.Rows);
                            }
                        });

                        return;
                    }
                }

                //Discount
                if($("#selDiscountType").val() != ""){
                    intIndex = 0;
                    pDataDiscount[intIndex] = {
                        ID: $("#selDiscountType").val(),
                        DiscountPercent: $("#txtDiscountPercent").val(),
                        DiscountRemarks: $("#txtDiscountRemarks").val()
                    };
                }

                //Check
                intIndex = 0;
                for (var i = 0; i <= intSalesPayCheckCntr; i++) {

                    if (sessionStorage["SalesPayCheck-" + i] != undefined) {

                        arSalesPayCheck = new Array();
                        arSalesPayCheck = JSON.parse(sessionStorage["SalesPayCheck-" + i]);

                        var dtCheckDate = "";
                        if (arSalesPayCheck[intSalesPayCheckDateMonth] != "" &&
                            arSalesPayCheck[intSalesPayCheckDateDay] != "" &&
                            arSalesPayCheck[intSalesPayCheckDateyear] != "") {

                            dtCheckDate = arSalesPayCheck[intSalesPayCheckDateMonth] + "/" + arSalesPayCheck[intSalesPayCheckDateDay] + "/" + arSalesPayCheck[intSalesPayCheckDateyear];
                        }

                        pDataCheck[intIndex] = {
                            ID: arSalesPayCheck[intSalesPayCheckRecID],
                            BankID: arSalesPayCheck[intSalesPayCheckBankID],
                            BankName: arSalesPayCheck[intSalesPayCheckBank],
                            BankBranchID: arSalesPayCheck[intSalesPayCheckBankBranchID],
                            BankBranchName: arSalesPayCheck[intSalesPayCheckBankBranch],
                            AccountName: arSalesPayCheck[intSalesPayCheckAccountName],
                            AccountNumber: arSalesPayCheck[intSalesPayCheckAccountNo],
                            CheckNumber: arSalesPayCheck[intSalesPayCheckNumber],
                            CheckDate: dtCheckDate,
                            CheckAmount: arSalesPayCheck[intSalesPayCheckAmount],
                            RecState: arSalesPayCheck[intSalesPayCheckState]
                        };

                        intIndex = intIndex + 1;

                    }
                }

                //Debit Card
                intIndex = 0;
                for (var i = 0; i <= intSalesPayDebitCardCntr; i++) {

                    if (sessionStorage["SalesPayDebitCard-" + i] != undefined) {

                        arSalesPayDebitCard = new Array();
                        arSalesPayDebitCard = JSON.parse(sessionStorage["SalesPayDebitCard-" + i]);

                        var dtDebitCardIssuanceDate = "";
                        if (arSalesPayDebitCard[intSalesPayDebitCardIssuanceDateMonth] != "" &&
                            arSalesPayDebitCard[intSalesPayDebitCardIssuanceDateDay] != "" &&
                            arSalesPayDebitCard[intSalesPayDebitCardIssuanceDateYear] != "") {

                            dtDebitCardIssuanceDate = arSalesPayDebitCard[intSalesPayDebitCardIssuanceDateMonth] + "/" + arSalesPayDebitCard[intSalesPayDebitCardIssuanceDateDay] + "/" + arSalesPayDebitCard[intSalesPayDebitCardIssuanceDateYear];
                        }

                        var dtDebitCardExpirationDate = "";
                        if (arSalesPayDebitCard[intSalesPayDebitCardExpirationDateMonth] != "" &&
                            arSalesPayDebitCard[intSalesPayDebitCardExpirationDateDay] != "" &&
                            arSalesPayDebitCard[intSalesPayDebitCardExpirationDateYear] != "") {

                            dtDebitCardExpirationDate = arSalesPayDebitCard[intSalesPayDebitCardExpirationDateMonth] + "/" + arSalesPayDebitCard[intSalesPayDebitCardExpirationDateDay] + "/" + arSalesPayDebitCard[intSalesPayDebitCardExpirationDateYear];
                        }

                        pDataDebitCard[intIndex] = {
                            ID: arSalesPayDebitCard[intSalesPayDebitCardRecID],
                            DebitCardType: arSalesPayDebitCard[intSalesPayDebitCardType],
                            DebitCardBankID: arSalesPayDebitCard[intSalesPayDebitCardBankID],
                            DebitCardBank: arSalesPayDebitCard[intSalesPayDebitCardBank],
                            HolderLastName: arSalesPayDebitCard[intSalesPayDebitCardHolderLName],
                            HolderFirstName: arSalesPayDebitCard[intSalesPayDebitCardHolderFName],
                            HolderMiddleName: arSalesPayDebitCard[intSalesPayDebitCardHolderMName],
                            HolderGenerationalSuffix: arSalesPayDebitCard[intSalesPayDebitCardHolderGenSuffix],
                            BankCountryID: arSalesPayDebitCard[intSalesPayDebitCardBankCountryID],
                            DebitCardNumber: arSalesPayDebitCard[intSalesPayDebitCardNumber],
                            DebitCardIssuanceDate: dtDebitCardIssuanceDate,
                            DebitCardExpirationDate: dtDebitCardExpirationDate,
                            ApprovalNo: arSalesPayDebitCard[intSalesPayDebitCardApprovalNumber],
                            NoOfInstallmentPayments: arSalesPayDebitCard[intSalesPayDebitCardNoOfInstallment],
                            DebitCardAmount: arSalesPayDebitCard[intSalesPayDebitCardAmount],
                            RecState: arSalesPayDebitCard[intSalesPayDebitCardState]
                        };

                        intIndex = intIndex + 1;

                    }
                }

                //Credit Card
                intIndex = 0;
                for (var i = 0; i <= intSalesPayCreditCardCntr; i++) {

                    if (sessionStorage["SalesPayCreditCard-" + i] != undefined) {

                        arSalesPayCreditCard = new Array();
                        arSalesPayCreditCard = JSON.parse(sessionStorage["SalesPayCreditCard-" + i]);

                        var dtCreditCardIssuanceDate = "";
                        if (arSalesPayCreditCard[intSalesPayCreditCardIssuanceDateMonth] != "" &&
                            arSalesPayCreditCard[intSalesPayCreditCardIssuanceDateDay] != "" &&
                            arSalesPayCreditCard[intSalesPayCreditCardIssuanceDateYear] != "") {

                            dtCreditCardIssuanceDate = arSalesPayCreditCard[intSalesPayCreditCardIssuanceDateMonth] + "/" + arSalesPayCreditCard[intSalesPayCreditCardIssuanceDateDay] + "/" + arSalesPayCreditCard[intSalesPayCreditCardIssuanceDateYear];
                        }

                        var dtCreditCardExpirationDate = "";
                        if (arSalesPayCreditCard[intSalesPayCreditCardExpirationDateMonth] != "" &&
                            arSalesPayCreditCard[intSalesPayCreditCardExpirationDateDay] != "" &&
                            arSalesPayCreditCard[intSalesPayCreditCardExpirationDateYear] != "") {

                            dtCreditCardExpirationDate = arSalesPayCreditCard[intSalesPayCreditCardExpirationDateMonth] + "/" + arSalesPayCreditCard[intSalesPayCreditCardExpirationDateDay] + "/" + arSalesPayCreditCard[intSalesPayCreditCardExpirationDateYear];
                        }

                        pDataCreditCard[intIndex] = {
                            ID: arSalesPayCreditCard[intSalesPayCreditCardRecID],
                            CreditCardType: arSalesPayCreditCard[intSalesPayCreditCardType],
                            CreditCardBankID: arSalesPayCreditCard[intSalesPayCreditCardBankID],
                            CreditCardBank: arSalesPayCreditCard[intSalesPayCreditCardBank],
                            HolderLastName: arSalesPayCreditCard[intSalesPayCreditCardHolderLName],
                            HolderFirstName: arSalesPayCreditCard[intSalesPayCreditCardHolderFName],
                            HolderMiddleName: arSalesPayCreditCard[intSalesPayCreditCardHolderMName],
                            HolderGenerationalSuffix: arSalesPayCreditCard[intSalesPayCreditCardHolderGenSuffix],
                            BankCountryID: arSalesPayCreditCard[intSalesPayCreditCardBankCountryID],
                            CreditCardNumber: arSalesPayCreditCard[intSalesPayCreditCardNumber],
                            CreditCardIssuanceDate: dtCreditCardIssuanceDate,
                            CreditCardExpirationDate: dtCreditCardExpirationDate,
                            ApprovalNo: arSalesPayCreditCard[intSalesPayCreditCardApprovalNumber],
                            NoOfInstallmentPayments: arSalesPayCreditCard[intSalesPayCreditCardNoOfInstallment],
                            CreditCardAmount: arSalesPayCreditCard[intSalesPayCreditCardAmount],
                            RecState: arSalesPayCreditCard[intSalesPayCreditCardState]
                        };

                        intIndex = intIndex + 1;

                    }
                }

                //SRV
                intIndex = 0;
                for (var i = 0; i <= intSalesPaySRVCntr; i++) {

                    if (sessionStorage["SalesPaySRV-" + i] != undefined) {

                        arSalesPaySRV = new Array();
                        arSalesPaySRV = JSON.parse(sessionStorage["SalesPaySRV-" + i]);

                        pDataSRV[intIndex] = {
                            ID: arSalesPaySRV[intSalesPaySRVRecID],
                            MemoID: arSalesPaySRV[intSalesPaySRVMemoID],
                            ConsumedAmount: arSalesPaySRV[intSalesPaySRVConsumedAmount],
                            RecState: arSalesPaySRV[intSalesPaySRVState]
                        };

                        intIndex = intIndex + 1;

                    }
                }

                pData = {
                    EmailAddress: (IsEmailReceipt ? $.trim(strSalesPaymentEmail) : ""),
                    InvoiceID: pInvoiceID,
                    IsForGovReport: ($("#chkForGovReport").is(":checked") ? "1" : "0"),
                    CashAmount: $.trim($("#lblSalesPayTotalCash").text()),
                    DebitCardAmount: $("#lblSalesPayTotalDebitCard").text(),
                    CreditCardAmount: $("#lblSalesPayTotalCreditCard").text(),
                    CheckPaymentAmount: $("#lblSalesPayTotalCheck").text(),
                    SRVAmount: $("#lblSalesPayTotalSRV").text(),
                    TotalAmountDue: $("#lblSalesPaySubTotal").text(),
                    TotalDiscountAmount: $("#lblSalesPayTotalDiscount").text(),
                    TotalAmountTendered: $("#lblSalesPayTotalAmountTendered").text(),
                    CustomerChangeCash: ($("#lblSalesPayTotalChangeAmount").text() == "No Change" ? "0.00" : (blnIsCheckChangeTY ? "0.00" : $("#lblSalesPayTotalChangeAmount").text())),

                    SalesPaymentCheckList: pDataCheck,
                    SalesPaymentDebitCardList: pDataDebitCard,
                    SalesPaymentCreditCardList: pDataCreditCard,
                    SalesPaymentSRVList: pDataSRV,
                    SalesPaymentDiscountList: pDataDiscount
                };

                AjaxSend({
                    url: "SalesPayments/SaveSalesPayments",
                    data: pData,
                    success: function (vData) {

                        if (vData.Error != undefined) {
                            MessageBox(vData.Error);
                            return;
                        };

                        if (vData.SalesPaymentData == "InsufficientPayment") {
                            MessageBox("Please pay full amount due.",
                                   "Insufficient Payment",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                            );
                        } else if (vData.SalesPaymentData == "InvalidEmail") {
                            MessageBox("Please enter valid email address.",
                                   "Invalid email",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                            );
                        } else if (vData.SalesPaymentData == "Invoice has been changed") {
                            MessageBox("Invoice has been changed while processing payment. Please refresh to reload invoice details.",
                                   "Invoice Changes",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                            );
                        } else if (vData.SalesPaymentData == "DoubleEntry") {
                            MessageBox('Double entry');
                            return;
                        } else {

                            if (IsEmailReceipt) {

                                MessageBox("Please wait while sending the receipt.",
                                                    "Email Receipt",
                                                    [
                                                        { Text: 'Ok'
                                                        }
                                                    ]
                                            );

                                AjaxSend({
                                    url: "SalesPayments/SendEmailReceipt",
                                    data: pData,
                                    success: function (vData) {
                                        if (!IsPrintReceipt) {
                                            MessageBox("Your transaction has been saved. An email receipt has been sent to the email address(es) that you specified.",
                                                    "Email Receipt",
                                                    [
                                                        { Text: 'Ok',
                                                            Click: function () {
                                                                if (!IsPrintReceipt) {
                                                                    window.location = GetPathFormat("/SalesMenu?mode=pos&OrgID=" + sessionStorage.OrgID);
                                                                }
                                                            }
                                                        }
                                                    ]
                                            );
                                        }else{
                                            $.mobile.changePage(window.pageSourcePage);
                                            PrintReceipt(pInvoiceID);
                                        }
                                    }
                                });
                            }else if (IsPrintReceipt) {
                                PrintReceipt(pInvoiceID);
                            }; // if (IsPrintReceipt) {

                            if (!IsEmailReceipt && !IsPrintReceipt)
                                MessageBox('Transaction was successfully saved');
                        }; // end if

                    }
                });
            }

            function PrintReceipt(pInvoiceID){

                if (InAndroid) {
                    PrintPOSReceipt(pInvoiceID);
                } else {

                    var japplet = document.jzebra;
                    var blnIsPrinterFound = false;

                    if (japplet != null) {
                        while (!blnIsPrinterFound) {

                            if (FindDesktopPrinter("")) {
                                blnIsPrinterFound = true;

                                PrintPOSDesktopReceipt(pInvoiceID);

                            };

                        }
                    } else {
                        MessageBox("Jzebra applet was not loaded.",
                                "Jzebra",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                        );
                    }
                }
            }

            function SaveSalesPayCheck() {

                arSalesPayCheck = new Array();
                arSalesPayCheck[intSalesPayCheckRecID] = $("#txtSalesPayCheckID").val();
                arSalesPayCheck[intSalesPayCheckBankID] = $("#txtSalesPayCheckBankID").val();
                arSalesPayCheck[intSalesPayCheckBank] = ($('#divCheckBankButton').is(':visible') ? $("#btnSalesPayCheckBank").text() : $("#txtSalesPayCheckBankName").val());
                arSalesPayCheck[intSalesPayCheckBankBranchID] = $("#txtSalesPayCheckBankBranchID").val();
////                arSalesPayCheck[intSalesPayCheckBankBranch] = ($('#divCheckBankButton').is(':visible') ? $("#btnSalesPayCheckBankBranch").text() : $("#txtSalesPayCheckBankBranch").val());
                arSalesPayCheck[intSalesPayCheckBankBranch] = $("#txtSalesPayCheckBankBranch").val();
                arSalesPayCheck[intSalesPayCheckAccountName] = $("#txtSalesPayCheckAccountName").val();
                arSalesPayCheck[intSalesPayCheckAccountNo] = $("#txtSalesPayCheckAccountNo").val();
                arSalesPayCheck[intSalesPayCheckNumber] = $("#txtSalesPayCheckNo").val();
                arSalesPayCheck[intSalesPayCheckDateMonth] = $("#selSalesPayCheckDateMonth").val();
                arSalesPayCheck[intSalesPayCheckDateDay] = $("#selSalesPayCheckDateDay").val();
                arSalesPayCheck[intSalesPayCheckDateyear] = $("#selSalesPayCheckDateYear").val();
                arSalesPayCheck[intSalesPayCheckAmount] = AccountingFormat($("#txtSalesPayCheckAmount").val());
                arSalesPayCheck[intSalesPayCheckState] = (IsNewSalesPayCheck ? "New" : "Edit");

                sessionStorage["SalesPayCheck-" + intSalesPayCheckIndex] = JSON.stringify(arSalesPayCheck);

                $.mobile.changePage('#pageSalesPayment');
                LoadSalesPayCheckList();

            };

            function SaveSalesPayDebitCard() {

                arSalesPayDebitCard = new Array();
                arSalesPayDebitCard[intSalesPayDebitCardRecID] = $("#txtSalesPayDebitCardID").val();
                arSalesPayDebitCard[intSalesPayDebitCardType] = $("#selSalesPayDebitCardType").val();
                arSalesPayDebitCard[intSalesPayDebitCardBankID] = $("#txtSalesPayDebitCardBankID").val();
                arSalesPayDebitCard[intSalesPayDebitCardBank] = ($('#divSalesPayDebitCardBankButton').is(':visible') ? $("#btnSalesPayDebitCardBank").text() : $("#txtSalesPayDebitCardBank").val());
                arSalesPayDebitCard[intSalesPayDebitCardHolderLName] = $("#txtSalesPayDebitCardLastName").val();
                arSalesPayDebitCard[intSalesPayDebitCardHolderFName] = $("#txtSalesPayDebitCardFirstName").val();
                arSalesPayDebitCard[intSalesPayDebitCardHolderMName] = $("#txtSalesPayDebitCardMiddleName").val();
                arSalesPayDebitCard[intSalesPayDebitCardHolderGenSuffix] = $("#selSalesPayDebitPersonalSuffixName").val();
                arSalesPayDebitCard[intSalesPayDebitCardBankCountryID] = $("#txtSalesPayDebitCardBankLocationID").val();
                arSalesPayDebitCard[intSalesPayDebitCardBankCountry] = $("#btnSalesPayDebitCardBankLocation").text();
                arSalesPayDebitCard[intSalesPayDebitCardNumber] = $("#txtSalesPayDebitCardNo").val();
                arSalesPayDebitCard[intSalesPayDebitCardIssuanceDateMonth] = $("#selSalesPayDebitCardIssuanceDateMonth").val();
                arSalesPayDebitCard[intSalesPayDebitCardIssuanceDateDay] = $("#selSalesPayDebitCardIssuanceDateDay").val();
                arSalesPayDebitCard[intSalesPayDebitCardIssuanceDateYear] = $("#selSalesPayDebitCardIssuanceDateYear").val();
                arSalesPayDebitCard[intSalesPayDebitCardExpirationDateMonth] = $("#selSalesPayDebitCardExpirationDateMonth").val();
                arSalesPayDebitCard[intSalesPayDebitCardExpirationDateDay] = $("#selSalesPayDebitCardExpirationDateDay").val();
                arSalesPayDebitCard[intSalesPayDebitCardExpirationDateYear] = $("#selSalesPayDebitCardExpirationDateYear").val();
                arSalesPayDebitCard[intSalesPayDebitCardApprovalNumber] = $("#txtSalesPayDebitCardApprovalNo").val();
                arSalesPayDebitCard[intSalesPayDebitCardNoOfInstallment] = $("#txtSalesPayDebitCardNoOfPayments").val();
                arSalesPayDebitCard[intSalesPayDebitCardAmount] = AccountingFormat($("#txtSalesPayDebitCardTenderedAmount").val());
                arSalesPayDebitCard[intSalesPayDebitCardState] = (IsNewSalesPayDebitCard ? "New" : "Edit");

                sessionStorage["SalesPayDebitCard-" + intSalesPayDebitCardIndex] = JSON.stringify(arSalesPayDebitCard);

                $.mobile.changePage('#pageSalesPayment');
                LoadSalesPayDebitCardList();

            };

            function SaveSalesPayCreditCard() {

                arSalesPayCreditCard = new Array();
                arSalesPayCreditCard[intSalesPayCreditCardRecID] = $("#txtSalesPayCreditCardID").val();
                arSalesPayCreditCard[intSalesPayCreditCardType] = $("#selSalesPayCreditCardType").val();
                arSalesPayCreditCard[intSalesPayCreditCardBankID] = $("#txtSalesPayCreditCardBankID").val();
                arSalesPayCreditCard[intSalesPayCreditCardBank] = ($('#divSalesPayCreditCardBankButton').is(':visible') ? $("#btnSalesPayCreditCardBank").text() : $("#txtSalesPayCreditCardBank").val());
                arSalesPayCreditCard[intSalesPayCreditCardHolderLName] = $("#txtSalesPayCreditCardLastName").val();
                arSalesPayCreditCard[intSalesPayCreditCardHolderFName] = $("#txtSalesPayCreditCardFirstName").val();
                arSalesPayCreditCard[intSalesPayCreditCardHolderMName] = $("#txtSalesPayCreditCardMiddleName").val();
                arSalesPayCreditCard[intSalesPayCreditCardHolderGenSuffix] = $("#selSalesPayPersonalSuffixName").val();
                arSalesPayCreditCard[intSalesPayCreditCardBankCountryID] = $("#txtSalesPayCreditCardBankLocationID").val();
                arSalesPayCreditCard[intSalesPayCreditCardBankCountry] = $("#btnSalesPayCreditCardBankLocation").text();
                arSalesPayCreditCard[intSalesPayCreditCardNumber] = $("#txtSalesPayCreditCardNo").val();
                arSalesPayCreditCard[intSalesPayCreditCardIssuanceDateMonth] = $("#selSalesPayCreditCardIssuanceDateMonth").val();
                arSalesPayCreditCard[intSalesPayCreditCardIssuanceDateDay] = $("#selSalesPayCreditCardIssuanceDateDay").val();
                arSalesPayCreditCard[intSalesPayCreditCardIssuanceDateYear] = $("#selSalesPayCreditCardIssuanceDateYear").val();
                arSalesPayCreditCard[intSalesPayCreditCardExpirationDateMonth] = $("#selSalesPayCreditCardExpirationDateMonth").val();
                arSalesPayCreditCard[intSalesPayCreditCardExpirationDateDay] = $("#selSalesPayCreditCardExpirationDateDay").val();
                arSalesPayCreditCard[intSalesPayCreditCardExpirationDateYear] = $("#selSalesPayCreditCardExpirationDateYear").val();
                arSalesPayCreditCard[intSalesPayCreditCardApprovalNumber] = $("#txtSalesPayCreditCardApprovalNo").val();
                arSalesPayCreditCard[intSalesPayCreditCardNoOfInstallment] = $("#txtSalesPayCreditCardNoOfPayments").val();
                arSalesPayCreditCard[intSalesPayCreditCardAmount] = AccountingFormat($("#txtSalesPayCreditCardTenderedAmount").val());
                arSalesPayCreditCard[intSalesPayCreditCardState] = (IsNewSalesPayCreditCard ? "New" : "Edit");

                sessionStorage["SalesPayCreditCard-" + intSalesPayCreditCardIndex] = JSON.stringify(arSalesPayCreditCard);

                $.mobile.changePage('#pageSalesPayment');
                LoadSalesPayCreditCardList();

            };

            function SaveSalesPaySRV() {
                var intExist = CheckSalesPaySRVExist();

                if (intExist <= 0) {
                    arSalesPaySRV = new Array();
                    arSalesPaySRV[intSalesPaySRVRecID] = $("#txtSalesPayTransSRVID").val();
                    arSalesPaySRV[intSalesPaySRVMemoID] = $("#txtSalesPaySRVTransMemoID").val();
                    arSalesPaySRV[intSalesPaySRVNo] = $("#txtSalesPaySRVNo").val();
                    arSalesPaySRV[intSalesPaySRVPinCode] = $("#txtSalesPaySRVPinCode").val();
                    arSalesPaySRV[intSalesPaySRVAmount] = AccountingFormat($("#txtSalesPaySRVAmount").val());
                    arSalesPaySRV[intSalesPaySRVConsumedAmount] = AccountingFormat($("#txtSalesPaySRVConsumedAmount").val());
                    arSalesPaySRV[intSalesPaySRVState] = (IsNewSalesPaySRV ? "New" : "Edit");

                    sessionStorage["SalesPaySRV-" + intSalesPaySRVIndex] = JSON.stringify(arSalesPaySRV);

                    $.mobile.changePage('#pageSalesPayment');
                    LoadSalesPaySRVList();
                } else {
                    MessageBox("SRV number already listed.",
                                "SRV Number",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                            );
                }
            };

            function CheckSalesPaySRVExist() {
                var intExist = 0;

                for (var i = 0; i <= intSalesPaySRVCntr; i++) {

                    if (sessionStorage["SalesPaySRV-" + i] != undefined) {

                        if (i != intSalesPaySRVIndex) {
                            arSalesPaySRVTmp = new Array();
                            arSalesPaySRVTmp = JSON.parse(sessionStorage["SalesPaySRV-" + i]);

                            if (arSalesPaySRVTmp[intSalesPaySRVMemoID] == $("#txtSalesPaySRVTransMemoID").val() && arSalesPaySRVTmp[intSalesPaySRVState] != "Remove") {
                                intExist = intExist + 1;
                            }
                        }
                    }
                }

                return intExist;
            };

            function RemoveSalesPayCheck() {

                arSalesPayCheck = new Array();
                arSalesPayCheck[intSalesPayCheckRecID] = $("#txtSalesPayCheckID").val();
                arSalesPayCheck[intSalesPayCheckBankID] = $("#txtSalesPayCheckBankID").val();
                arSalesPayCheck[intSalesPayCheckBank] = ($('#divCheckBankButton').is(':visible') ? $("#btnSalesPayCheckBank").text() : $("#txtSalesPayCheckBankName").val());
                arSalesPayCheck[intSalesPayCheckBankBranchID] = $("#txtSalesPayCheckBankBranchID").val();
                arSalesPayCheck[intSalesPayCheckBankBranch] = ($('#divCheckBankButton').is(':visible') ? $("#btnSalesPayCheckBankBranch").text() : $("#txtSalesPayCheckBankBranch").val());
                arSalesPayCheck[intSalesPayCheckAccountName] = $("#txtSalesPayCheckAccountName").val();
                arSalesPayCheck[intSalesPayCheckAccountNo] = $("#txtSalesPayCheckAccountNo").val();
                arSalesPayCheck[intSalesPayCheckNumber] = $("#txtSalesPayCheckNo").val();
                arSalesPayCheck[intSalesPayCheckDateMonth] = $("#selSalesPayCheckDateMonth").val();
                arSalesPayCheck[intSalesPayCheckDateDay] = $("#selSalesPayCheckDateDay").val();
                arSalesPayCheck[intSalesPayCheckDateyear] = $("#selSalesPayCheckDateYear").val();
                arSalesPayCheck[intSalesPayCheckAmount] = $("#txtSalesPayCheckAmount").val();
                arSalesPayCheck[intSalesPayCheckState] = "Remove";

                sessionStorage["SalesPayCheck-" + intSalesPayCheckIndex] = JSON.stringify(arSalesPayCheck);

                $.mobile.changePage('#pageSalesPayment');
                LoadSalesPayCheckList();
            };

            function RemoveSalesPayDebitCard() {

                arSalesPayDebitCard = new Array();
                arSalesPayDebitCard[intSalesPayDebitCardRecID] = $("#txtSalesPayDebitCardID").val();
                arSalesPayDebitCard[intSalesPayDebitCardType] = $("#selSalesPayDebitCardType").val();
                arSalesPayDebitCard[intSalesPayDebitCardBankID] = $("#txtSalesPayDebitCardBankID").val();
                arSalesPayDebitCard[intSalesPayDebitCardBank] = ($('#divSalesPayDebitCardBankButton').is(':visible') ? $("#btnSalesPayDebitCardBank").text() : $("#txtSalesPayDebitCardBank").val());
                arSalesPayDebitCard[intSalesPayDebitCardHolderLName] = $("#txtSalesPayDebitCardLastName").val();
                arSalesPayDebitCard[intSalesPayDebitCardHolderFName] = $("#txtSalesPayDebitCardFirstName").val();
                arSalesPayDebitCard[intSalesPayDebitCardHolderMName] = $("#txtSalesPayDebitCardMiddleName").val();
                arSalesPayDebitCard[intSalesPayDebitCardHolderGenSuffix] = $("#selSalesPayPersonalSuffixName").val();
                arSalesPayDebitCard[intSalesPayDebitCardBankCountryID] = $("#txtSalesPayDebitCardBankLocationID").val();
                arSalesPayDebitCard[intSalesPayDebitCardBankCountry] = $("#btnSalesPayDebitCardBankLocation").text();
                arSalesPayDebitCard[intSalesPayDebitCardNumber] = $("#txtSalesPayDebitCardNo").val();
                arSalesPayDebitCard[intSalesPayDebitCardIssuanceDateMonth] = $("#selSalesPayDebitCardIssuanceDateMonth").val();
                arSalesPayDebitCard[intSalesPayDebitCardIssuanceDateDay] = $("#selSalesPayDebitCardIssuanceDateDay").val();
                arSalesPayDebitCard[intSalesPayDebitCardIssuanceDateYear] = $("#selSalesPayDebitCardIssuanceDateYear").val();
                arSalesPayDebitCard[intSalesPayDebitCardExpirationDateMonth] = $("#selSalesPayDebitCardExpirationDateMonth").val();
                arSalesPayDebitCard[intSalesPayDebitCardExpirationDateDay] = $("#selSalesPayDebitCardExpirationDateDay").val();
                arSalesPayDebitCard[intSalesPayDebitCardExpirationDateYear] = $("#selSalesPayDebitCardExpirationDateYear").val();
                arSalesPayDebitCard[intSalesPayDebitCardApprovalNumber] = $("#txtSalesPayDebitCardApprovalNo").val();
                arSalesPayDebitCard[intSalesPayDebitCardNoOfInstallment] = $("#txtSalesPayDebitCardNoOfPayments").val();
                arSalesPayDebitCard[intSalesPayDebitCardAmount] = $("#txtSalesPayDebitCardTenderedAmount").val();
                arSalesPayDebitCard[intSalesPayDebitCardState] = "Remove";

                sessionStorage["SalesPayDebitCard-" + intSalesPayDebitCardIndex] = JSON.stringify(arSalesPayDebitCard);

                $.mobile.changePage('#pageSalesPayment');
                LoadSalesPayDebitCardList();
            };

            function RemoveSalesPayCreditCard() {

                arSalesPayCreditCard = new Array();
                arSalesPayCreditCard[intSalesPayCreditCardRecID] = $("#txtSalesPayCreditCardID").val();
                arSalesPayCreditCard[intSalesPayCreditCardType] = $("#selSalesPayCreditCardType").val();
                arSalesPayCreditCard[intSalesPayCreditCardBankID] = $("#txtSalesPayCreditCardBankID").val();
                arSalesPayCreditCard[intSalesPayCreditCardBank] = ($('#divSalesPayCreditCardBankButton').is(':visible') ? $("#btnSalesPayCreditCardBank").text() : $("#txtSalesPayCreditCardBank").val());
                arSalesPayCreditCard[intSalesPayCreditCardHolderLName] = $("#txtSalesPayCreditCardLastName").val();
                arSalesPayCreditCard[intSalesPayCreditCardHolderFName] = $("#txtSalesPayCreditCardFirstName").val();
                arSalesPayCreditCard[intSalesPayCreditCardHolderMName] = $("#txtSalesPayCreditCardMiddleName").val();
                arSalesPayCreditCard[intSalesPayCreditCardHolderGenSuffix] = $("#selSalesPayPersonalSuffixName").val();
                arSalesPayCreditCard[intSalesPayCreditCardBankCountryID] = $("#txtSalesPayCreditCardBankLocationID").val();
                arSalesPayCreditCard[intSalesPayCreditCardBankCountry] = $("#btnSalesPayCreditCardBankLocation").text();
                arSalesPayCreditCard[intSalesPayCreditCardNumber] = $("#txtSalesPayCreditCardNo").val();
                arSalesPayCreditCard[intSalesPayCreditCardIssuanceDateMonth] = $("#selSalesPayCreditCardIssuanceDateMonth").val();
                arSalesPayCreditCard[intSalesPayCreditCardIssuanceDateDay] = $("#selSalesPayCreditCardIssuanceDateDay").val();
                arSalesPayCreditCard[intSalesPayCreditCardIssuanceDateYear] = $("#selSalesPayCreditCardIssuanceDateYear").val();
                arSalesPayCreditCard[intSalesPayCreditCardExpirationDateMonth] = $("#selSalesPayCreditCardExpirationDateMonth").val();
                arSalesPayCreditCard[intSalesPayCreditCardExpirationDateDay] = $("#selSalesPayCreditCardExpirationDateDay").val();
                arSalesPayCreditCard[intSalesPayCreditCardExpirationDateYear] = $("#selSalesPayCreditCardExpirationDateYear").val();
                arSalesPayCreditCard[intSalesPayCreditCardApprovalNumber] = $("#txtSalesPayCreditCardApprovalNo").val();
                arSalesPayCreditCard[intSalesPayCreditCardNoOfInstallment] = $("#txtSalesPayCreditCardNoOfPayments").val();
                arSalesPayCreditCard[intSalesPayCreditCardAmount] = AccountingFormat($("#txtSalesPayCreditCardTenderedAmount").val());
                arSalesPayCreditCard[intSalesPayCreditCardState] = "Remove";

                sessionStorage["SalesPayCreditCard-" + intSalesPayCreditCardIndex] = JSON.stringify(arSalesPayCreditCard);

                $.mobile.changePage('#pageSalesPayment');
                LoadSalesPayCreditCardList();
            };

            function RemoveSalesPaySRV() {

                arSalesPaySRV = new Array();
                arSalesPaySRV[intSalesPaySRVRecID] = $("#txtSalesPayTransSRVID").val();
                arSalesPaySRV[intSalesPaySRVMemoID] = $("#txtSalesPaySRVTransMemoID").val();
                arSalesPaySRV[intSalesPaySRVNo] = $("#txtSalesPaySRVNo").val();
                arSalesPaySRV[intSalesPaySRVPinCode] = $("#txtSalesPaySRVPinCode").val();
                arSalesPaySRV[intSalesPaySRVAmount] = $("#txtSalesPaySRVAmount").val();
                arSalesPaySRV[intSalesPaySRVConsumedAmount] = $("#txtSalesPaySRVConsumedAmount").val();
                arSalesPaySRV[intSalesPaySRVState] = "Remove";

                sessionStorage["SalesPaySRV-" + intSalesPaySRVIndex] = JSON.stringify(arSalesPaySRV);

                $.mobile.changePage('#pageSalesPayment');
                LoadSalesPaySRVList();
            };

            function LoadSalesPayCheckList() {
                var pLV = $('#lstSalesPayCheck');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intSalesPayCheckCntr; i++) {

                    if (sessionStorage["SalesPayCheck-" + i] != undefined) {

                        arSalesPayCheck = new Array();
                        arSalesPayCheck = JSON.parse(sessionStorage["SalesPayCheck-" + i]);

                        if (arSalesPayCheck[intSalesPayCheckState] != "Remove") {

                            pItem = $('<li id="lst' + i + '">')
                            .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arSalesPayCheck)
                                    .click(function (e) {
                                        e.preventDefault();
                                        ShowSalesPayCheckInfo($(this).data('Data'), $(this).attr('ID'));
                                    })
                                    .append($('<h3>')
                                        .attr("style", "white-space:normal")
                                        .text(arSalesPayCheck[intSalesPayCheckNumber])
                                    )
                                    .append($('<p>')
                                        .attr("style", "white-space:normal")
                                        .text("Amount : " + arSalesPayCheck[intSalesPayCheckAmount])
                                    )
                            );


                            pLV.append(pItem);

                        }

                    }
                }

                pLV.listview('refresh');

                RefreshSalesPayCheckTotal();
            };

            function LoadSalesPayDebitCardList() {
                var pLV = $('#lstSalesPayDebitCard');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intSalesPayDebitCardCntr; i++) {

                    if (sessionStorage["SalesPayDebitCard-" + i] != undefined) {

                        arSalesPayDebitCard = new Array();
                        arSalesPayDebitCard = JSON.parse(sessionStorage["SalesPayDebitCard-" + i]);

                        if (arSalesPayDebitCard[intSalesPayDebitCardState] != "Remove") {

                            pItem = $('<li id="lst' + i + '">')
                            .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arSalesPayDebitCard)
                                    .click(function (e) {
                                        e.preventDefault();
                                        ShowSalesPayDebitCardInfo($(this).data('Data'), $(this).attr('ID'));
                                    })
                                    .append($('<h3>')
                                        .attr("style", "white-space:normal")
                                        .text(arSalesPayDebitCard[intSalesPayDebitCardApprovalNumber])
                                    )
                                    .append($('<p>')
                                        .attr("style", "white-space:normal")
                                        .text("Amount : " + arSalesPayDebitCard[intSalesPayDebitCardAmount])
                                    )
                            );


                            pLV.append(pItem);

                        }

                    }
                }

                pLV.listview('refresh');

                RefreshSalesPayDebitCardTotal();

            };

            function LoadSalesPayCreditCardList() {
                var pLV = $('#lstSalesPayCreditCard');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intSalesPayCreditCardCntr; i++) {

                    if (sessionStorage["SalesPayCreditCard-" + i] != undefined) {

                        arSalesPayCreditCard = new Array();
                        arSalesPayCreditCard = JSON.parse(sessionStorage["SalesPayCreditCard-" + i]);

                        if (arSalesPayCreditCard[intSalesPayCreditCardState] != "Remove") {

                            pItem = $('<li id="lst' + i + '">')
                            .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arSalesPayCreditCard)
                                    .click(function (e) {
                                        e.preventDefault();
                                        ShowSalesPayCreditCardInfo($(this).data('Data'), $(this).attr('ID'));
                                    })
                                    .append($('<h3>')
                                        .attr("style", "white-space:normal")
                                        .text(arSalesPayCreditCard[intSalesPayCreditCardApprovalNumber])
                                    )
                                    .append($('<p>')
                                        .attr("style", "white-space:normal")
                                        .text("Amount : " + arSalesPayCreditCard[intSalesPayCreditCardAmount])
                                    )
                            );


                            pLV.append(pItem);

                        }

                    }
                }

                pLV.listview('refresh');

                RefreshSalesPayCreditCardTotal();

            };

            function LoadSalesPaySRVList() {
                var pLV = $('#lstSalesPaySRV');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intSalesPaySRVCntr; i++) {

                    if (sessionStorage["SalesPaySRV-" + i] != undefined) {

                        arSalesPaySRV = new Array();
                        arSalesPaySRV = JSON.parse(sessionStorage["SalesPaySRV-" + i]);

                        if (arSalesPaySRV[intSalesPaySRVState] != "Remove") {

                            pItem = $('<li id="lst' + i + '">')
                            .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arSalesPaySRV)
                                    .click(function (e) {
                                        e.preventDefault();
                                        ShowSalesPaySRVInfo($(this).data('Data'), $(this).attr('ID'));
                                    })
                                    .append($('<h3>')
                                        .attr("style", "white-space:normal")
                                        .text("SRV No. : " + arSalesPaySRV[intSalesPaySRVNo])
                                    )
                                    .append($('<p>')
                                        .attr("style", "white-space:normal")
                                        .text("Amount : " + arSalesPaySRV[intSalesPaySRVConsumedAmount])
                                    )
                            );


                            pLV.append(pItem);

                        }

                    }
                }

                pLV.listview('refresh');

                RefreshSalesPaySRVTotal();
            };

            function RefreshSalesPayCheckTotal() {

                var dblTotalSalesPayCheck = 0;

                for (var i = 0; i <= intSalesPayCheckCntr; i++) {

                    if (sessionStorage["SalesPayCheck-" + i] != undefined) {

                        arSalesPayCheck = new Array();
                        arSalesPayCheck = JSON.parse(sessionStorage["SalesPayCheck-" + i]);

                        if (arSalesPayCheck[intSalesPayCheckState] != "Remove") {

                            dblTotalSalesPayCheck = dblTotalSalesPayCheck + parseFloat(CDec2(arSalesPayCheck[intSalesPayCheckAmount].replace(/,/g, "")));
                        }
                    }
                }

                $("#lblSalesPayTotalCheck").text((dblTotalSalesPayCheck > 0 ? AccountingFormat(dblTotalSalesPayCheck) : "0.00"));

                RefreshSalesPaymentTotal();
            };

            function RefreshSalesPayDebitCardTotal() {

                var dblTotalSalesPayCC = 0;

                for (var i = 0; i <= intSalesPayDebitCardCntr; i++) {

                    if (sessionStorage["SalesPayDebitCard-" + i] != undefined) {

                        arSalesPayDebitCard = new Array();
                        arSalesPayDebitCard = JSON.parse(sessionStorage["SalesPayDebitCard-" + i]);

                        if (arSalesPayDebitCard[intSalesPayDebitCardState] != "Remove") {

                            dblTotalSalesPayCC = dblTotalSalesPayCC + parseFloat(CDec2(arSalesPayDebitCard[intSalesPayDebitCardAmount].replace(/,/g, "")));
                        }
                    }
                }

                $("#lblSalesPayTotalDebitCard").text((dblTotalSalesPayCC > 0 ? AccountingFormat(dblTotalSalesPayCC) : "0.00"));

                RefreshSalesPaymentTotal();
            };

            function RefreshSalesPayCreditCardTotal() {

                var dblTotalSalesPayCC = 0;

                for (var i = 0; i <= intSalesPayCreditCardCntr; i++) {

                    if (sessionStorage["SalesPayCreditCard-" + i] != undefined) {

                        arSalesPayCreditCard = new Array();
                        arSalesPayCreditCard = JSON.parse(sessionStorage["SalesPayCreditCard-" + i]);

                        if (arSalesPayCreditCard[intSalesPayCreditCardState] != "Remove") {

                            dblTotalSalesPayCC = dblTotalSalesPayCC + parseFloat(CDec2(arSalesPayCreditCard[intSalesPayCreditCardAmount].replace(/,/g, "")));
                        }
                    }
                }

                $("#lblSalesPayTotalCreditCard").text((dblTotalSalesPayCC > 0 ? AccountingFormat(dblTotalSalesPayCC) : "0.00"));

                RefreshSalesPaymentTotal();
            };

            function RefreshSalesPaySRVTotal() {

                var dblTotalSalesPaySRV = 0;

                for (var i = 0; i <= intSalesPaySRVCntr; i++) {

                    if (sessionStorage["SalesPaySRV-" + i] != undefined) {

                        arSalesPaySRV = new Array();
                        arSalesPaySRV = JSON.parse(sessionStorage["SalesPaySRV-" + i]);

                        if (arSalesPaySRV[intSalesPaySRVState] != "Remove") {

                            dblTotalSalesPaySRV = dblTotalSalesPaySRV + parseFloat(arSalesPaySRV[intSalesPaySRVConsumedAmount].replace(/,/g, ""));
                        }
                    }
                }

                $("#lblSalesPayTotalSRV").text((dblTotalSalesPaySRV > 0 ? AccountingFormat(dblTotalSalesPaySRV) : "0.00"));
                RefreshSalesPaymentTotal();
            };

            function RefreshSalesPaymentTotal() {

                var dblTotalSalesPay = 0;
                var dblTotalAmountDue = 0;

                dblTotalAmountDue = parseFloat(CDec2($('#lblSalesPaySubTotal').text().replace(/,/g, ""))) - parseFloat(CDec2($('#lblSalesPayTotalDiscount').text().replace(/,/g, ""))) + parseFloat(CDec2($('#lblSalesPayCharges').text().replace(/,/g, "")));
                dblTotalSalesPay = (parseFloat(CDec2($('#lblSalesPayTotalCash').text().replace(/,/g, ""))) + parseFloat(CDec2($('#lblSalesPayTotalCheck').text().replace(/,/g, ""))) + parseFloat(CDec2($('#lblSalesPayTotalDebitCard').text().replace(/,/g, ""))) + parseFloat(CDec2($('#lblSalesPayTotalCreditCard').text().replace(/,/g, ""))) + parseFloat(CDec2($('#lblSalesPayTotalSRV').text().replace(/,/g, "")))) - (parseFloat(CDec2($('#lblSalesPaySubTotal').text().replace(/,/g, ""))) - parseFloat(CDec2($('#lblSalesPayTotalDiscount').text().replace(/,/g, ""))));

                $("#lblSalesPayAmountDue").text(AccountingFormat(dblTotalAmountDue));
                $("#lblSalesPayCheckAmountDue").text($("#lblSalesPayAmountDue").text());
                $("#lblSalesPayDebitCardAmountDue").text($("#lblSalesPayAmountDue").text());
                $("#lblSalesPayCreditCardAmountDue").text($("#lblSalesPayAmountDue").text());
                $("#lblSalesPaySRVAmountDue").text($("#lblSalesPayAmountDue").text());

                $("#lblSalesPayTotalAmountTendered").text(AccountingFormat((parseFloat(CDec2($('#lblSalesPayTotalCash').text().replace(/,/g, ""))) + parseFloat(CDec2($('#lblSalesPayTotalCheck').text().replace(/,/g, ""))) + parseFloat(CDec2($('#lblSalesPayTotalDebitCard').text().replace(/,/g, ""))) + parseFloat(CDec2($('#lblSalesPayTotalCreditCard').text().replace(/,/g, ""))) + parseFloat(CDec2($('#lblSalesPayTotalSRV').text().replace(/,/g, ""))))));
                $("#lblSalesPayCheckTotalAmountTendered").text($("#lblSalesPayTotalAmountTendered").text());
                $("#lblSalesPayDebitCardTotalAmountTendered").text($("#lblSalesPayTotalAmountTendered").text());
                $("#lblSalesPayCreditCardTotalAmountTendered").text($("#lblSalesPayTotalAmountTendered").text());
                $("#lblSalesPaySRVTotalAmountTendered").text($("#lblSalesPayTotalAmountTendered").text());

                if (dblTotalSalesPay >= 0) {
                    $("#lblSalesPayAmountLacking").text("Complete Payment");
                    $("#lblSalesPayTotalChangeAmount").text(AccountingFormat(dblTotalSalesPay));
                } else {
                    $("#lblSalesPayAmountLacking").text(AccountingFormat(-dblTotalSalesPay));
                    $("#lblSalesPayTotalChangeAmount").text("No Change");
                }

                $("#lblSalesPayCheckAmountLacking").text($("#lblSalesPayAmountLacking").text());
                $("#lblSalesPayDebitCardAmountLacking").text($("#lblSalesPayAmountLacking").text());
                $("#lblSalesPayCreditCardAmountLacking").text($("#lblSalesPayAmountLacking").text());
                $("#lblSalesPaySRVAmountLacking").text($("#lblSalesPayAmountLacking").text());

                $("#lblSalesPayCheckTotalChangeAmount").text($("#lblSalesPayTotalChangeAmount").text());
                $("#lblSalesPayDebitCardTotalChangeAmount").text($("#lblSalesPayTotalChangeAmount").text());
                $("#lblSalesPayCreditCardTotalChangeAmount").text($("#lblSalesPayTotalChangeAmount").text());
                $("#lblSalesPaySRVTotalChangeAmount").text($("#lblSalesPayTotalChangeAmount").text());

            };

            function PrintPOSReceipt(pInvoiceID) {
                var strTemp = '';
                var strPrint = '';
                var strPrinterIP = '';
                var IsShowSalesRep = false;
                var IsHasCheckSalesPaymentExcess = false;
                var pData = {};
                pData.InvoiceID = pInvoiceID;

                //Get Printer IP Address
                AjaxSend({
                    url: "Organization/getOrgFOWSTPrinterIP",
                    success: function (vDataPrint) {

                        if (vDataPrint.OrganizationFOWSTData.Rows.length > 0) {
                            
                            strPrinterIP = vDataPrint.OrganizationFOWSTData.Rows[0].PrinterIPAddress;

                            //New set of text to print
                            NewPrintBuilder();

                            //Get Header
                            AjaxSend({
                                url: "SalesPayments/getReceiptHeaderInfo",
                                data: pData,
                                success: function (vDataHeader) {

                                    IsShowSalesRep = false;
                                    if (vDataHeader.SalesPaymentData.Rows.length > 0) {

                                        IsShowSalesRep = (vDataHeader.SalesPaymentData.Rows[0].IsShowSalesRep == 1 ? true : false);

                                        strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.SalesPaymentData.Rows[0].BusinessTradeName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                                        strPrint = '';
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.SalesPaymentData.Rows[0].CorporateName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        strPrint = strPrint + strTemp;

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'TIN ' + $.trim((vDataHeader.SalesPaymentData.Rows[0].TIN || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        strPrint = strPrint + strTemp;

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.SalesPaymentData.Rows[0].CompleteAddress || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        strPrint = strPrint + strTemp;

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Tel. No.' + $.trim((vDataHeader.SalesPaymentData.Rows[0].TelNo || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                        strPrint = strPrint + strTemp;

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                        strPrint = strPrint + strTemp;

                                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                        strPrint = '';
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('POS Accreditation', 20, ' ', 1), 19, ': ' + $.trim((vDataHeader.SalesPaymentData.Rows[0].POSAccreditationNo || '')), intFAFCTotalLenght, '', 0, 0);
                                        strPrint = strPrint + strTemp;

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('POS Registration No.', 20, ' ', 1), 19, ': ' + $.trim((vDataHeader.SalesPaymentData.Rows[0].POSPermitNo || '')), intFAFCTotalLenght, '', 0, 0);
                                        strPrint = strPrint + strTemp;

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Machine ID', 20, ' ', 1), 19, ': ' + vDataHeader.SalesPaymentData.Rows[0].MIN, intFAFCTotalLenght, '', 0, 0);
                                        strPrint = strPrint + strTemp;

                                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                        strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                        //Get Items
                                        AjaxSend({
                                            url: "SalesPayments/getSalesPaymentInvoiceHeaderInfo",
                                            data: pData,
                                            success: function (vDataInvHeader) {

                                                strPrint = '';
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Customer', 8, ' ', 1), 7, ': ' + $.trim((vDataInvHeader.SalesPaymentData.Rows[0].CustomerName || '')), intFAFCTotalLenght, '', 0, 0);
                                                strPrint = strPrint + strTemp;

                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Address', 8, ' ', 1), 7, ': ' + $.trim((vDataInvHeader.SalesPaymentData.Rows[0].CustomerAddress || '')), intFAFCTotalLenght, '', 0, 0);
                                                strPrint = strPrint + strTemp;

                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('TIN', 8, ' ', 1), 7, ': ' + $.trim((vDataInvHeader.SalesPaymentData.Rows[0].CustomerTIN || '')), intFAFCTotalLenght, '', 0, 0);
                                                strPrint = strPrint + strTemp;
                                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                strPrint = '';
                                                strTemp = FormatTextToPrint(intFCDoubleTotalLenght, 'SI ' + SetPrefixPostfix($.trim((vDataInvHeader.SalesPaymentData.Rows[0].InvoiceNumber || '')), 12, '0', 0), intFCDoubleTotalLenght, '', 0, '', 0, 1);
                                                strPrint = strPrint + strTemp;

                                                strTemp = FormatTextToPrint(intFCDoubleTotalLenght, ($.format.date(new Date(vDataInvHeader.SalesPaymentData.Rows[0].InvoiceDate), 'MMM dd, yyyy HH:mm') || ''), intFCDoubleTotalLenght, '', 0, '', 0, 1);
                                                strPrint = strPrint + strTemp;

                                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Cashier: ' + ($.trim(vDataInvHeader.SalesPaymentData.Rows[0].CashierName || '')), intFAFCTotalLenght, '', 0, '', 0, 0);
                                                ReadyTextToPrint('\n' + strPrint, 3, 1, 1, 0, 0, 0, 0, 0);

                                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                //Get Items
                                                AjaxSend({
                                                    url: "SalesPayments/getSalesPaymentReceiptBodyInfo",
                                                    data: pData,
                                                    success: function (vDataItem) {

                                                        strPrint = '';
                                                        for (var p = 0; p <= vDataItem.SalesPaymentData.Rows.length - 1; p++) {

                                                            var strStockDescription = $.trim((vDataItem.SalesPaymentData.Rows[p].StockDescription || ''));
                                                            var strUnitPrice = SetPrefixPostfix(AccountingFormat(vDataItem.SalesPaymentData.Rows[p].UnitPrice || '0'), intFAFCTotalLenght - 30, ' ', 0);
                                                            var strQty = SetPrefixPostfix(AccountingFormat((vDataItem.SalesPaymentData.Rows[p].Quantity || '0'), 0), 8, ' ', 0);
                                                            var strUnitMeasure = SetPrefixPostfix((vDataItem.SalesPaymentData.Rows[p].UnitMeasure || ''), 10, ' ', 0);
                                                            var strSubTotal = SetPrefixPostfix(AccountingFormat(vDataItem.SalesPaymentData.Rows[p].SubTotal || '0'), intFAFCTotalLenght - 22, ' ', 0);

                                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, strStockDescription, 28, strUnitPrice, intFAFCTotalLenght, '', 0, 0);
                                                            strPrint = strPrint + strTemp;

                                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, strQty, 8, strUnitMeasure, 20, strSubTotal, intFAFCTotalLenght, 0);
                                                            strPrint = strPrint + strTemp;

                                                            if (IsShowSalesRep) {

                                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Sales Person: ' + ($.trim(vDataItem.SalesPaymentData.Rows[p].SalesPerson || '')), intFAFCTotalLenght, '', 0, '', 0, 0);
                                                                strPrint = strPrint + strTemp;

                                                            }

                                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, '_______________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                            strPrint = strPrint + strTemp;

                                                        };

                                                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                        //Get Footer Amounts
                                                        AjaxSend({
                                                            url: "SalesPayments/getSalesPaymentReceiptPaymentInfo",
                                                            data: pData,
                                                            success: function (vDataPayment) {

                                                                //Get Footer Amounts
                                                                AjaxSend({
                                                                    url: "SalesPayments/getSalesPaymentReceiptFooterAmountsInfo",
                                                                    data: pData,
                                                                    success: function (vDataFooter) {

                                                                        var strSKUCount = SetPrefixPostfix(AccountingFormat((vDataFooter.SalesPaymentData.Rows[0].SKUCount || '0'), 0), intFAFCTotalLenght - 14, ' ', 0);
                                                                        var strTotalAmountDue = SetPrefixPostfix(AccountingFormat(vDataFooter.SalesPaymentData.Rows[0].TotalAmountDue || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                                                        var strTotalDiscount = SetPrefixPostfix(AccountingFormat(vDataFooter.SalesPaymentData.Rows[0].DiscountAmount || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                                                        var strNetAmountDue = SetPrefixPostfix(AccountingFormat(vDataFooter.SalesPaymentData.Rows[0].NetAmountDue || '0'), intFAFCTotalLenght - 22, ' ', 0);

                                                                        strPrint = '';
                                                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'SKU Count: ', 12, strSKUCount, intFAFCTotalLenght, '', 0, 0);
                                                                        strPrint = strPrint + strTemp;

                                                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Sub Total: ', 20, strTotalAmountDue, intFAFCTotalLenght, '', 0, 0);
                                                                        strPrint = strPrint + strTemp;

                                                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Total Discount:   ', 20, strTotalDiscount, intFAFCTotalLenght, '', 0, 0);
                                                                        strPrint = strPrint + strTemp;

                                                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Amount Due:   ', 20, strNetAmountDue, intFAFCTotalLenght, '', 0, 0);
                                                                        strPrint = strPrint + strTemp;
                                                                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);


                                                                        if (vDataPayment.SalesPaymentData.Rows.length > 0) {

                                                                            var strCashPayment = SetPrefixPostfix(AccountingFormat((vDataPayment.SalesPaymentData.Rows[0].CashPayment || '0')), intFAFCTotalLenght - 7, ' ', 0);
                                                                            var strCheckPayment = SetPrefixPostfix(AccountingFormat(vDataPayment.SalesPaymentData.Rows[0].CheckPaymentAmount || '0'), intFAFCTotalLenght - 8, ' ', 0);
                                                                            var strDebitCardAmount = SetPrefixPostfix(AccountingFormat(vDataPayment.SalesPaymentData.Rows[0].DebitCardAmount || '0'), intFAFCTotalLenght - 14, ' ', 0);
                                                                            var strCreditCardAmount = SetPrefixPostfix(AccountingFormat(vDataPayment.SalesPaymentData.Rows[0].CreditCardAmount || '0'), intFAFCTotalLenght - 14, ' ', 0);
                                                                            var strSRVAmount = SetPrefixPostfix(AccountingFormat(vDataPayment.SalesPaymentData.Rows[0].SRVAmount || '0'), intFAFCTotalLenght - 6, ' ', 0);
                                                                            var strCustomerChangeCash = SetPrefixPostfix(AccountingFormat(vDataPayment.SalesPaymentData.Rows[0].CustomerChangeCash || '0'), intFAFCTotalLenght - 9, ' ', 0);

                                                                            strPrint = '\n';
                                                                            if (vDataPayment.SalesPaymentData.Rows[0].CashPayment > 0) {
                                                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Cash:', 5, strCashPayment, intFAFCTotalLenght, '', 0, 0);
                                                                                strPrint = strPrint + strTemp;
                                                                            }
                                                                            if (vDataPayment.SalesPaymentData.Rows[0].CheckPaymentAmount > 0) {
                                                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Check:', 6, strCheckPayment, intFAFCTotalLenght, '', 0, 0);
                                                                                strPrint = strPrint + strTemp;
                                                                            }
                                                                            if (vDataPayment.SalesPaymentData.Rows[0].CreditCardAmount > 0) {
                                                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Debit Card:', 12, strDebitCardAmount, intFAFCTotalLenght, '', 0, 0);
                                                                                strPrint = strPrint + strTemp;
                                                                            }
                                                                            if (vDataPayment.SalesPaymentData.Rows[0].CreditCardAmount > 0) {
                                                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Credit Card:', 12, strCreditCardAmount, intFAFCTotalLenght, '', 0, 0);
                                                                                strPrint = strPrint + strTemp;
                                                                            }
                                                                            if (vDataPayment.SalesPaymentData.Rows[0].SRVAmount > 0) {
                                                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'SRV:', 4, strSRVAmount, intFAFCTotalLenght, '', 0, 0);
                                                                                strPrint = strPrint + strTemp;
                                                                            }

                                                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Change:', 7, strCustomerChangeCash, intFAFCTotalLenght, '', 0, 0);
                                                                            strPrint = strPrint + strTemp;

                                                                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                                            if(vDataPayment.SalesPaymentData.Rows[0].CheckPaymentAmount > vDataFooter.SalesPaymentData.Rows[0].NetAmountDue){
                                                                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '* Excess payments made through checks are not entitled to cash refunds.', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                                                ReadyTextToPrint('\n' + strPrint, 3, 1, 1, 0, 0, 0, 0, 0);
                                                                            }

                                                                        }

                                                                        strPrint = '';
                                                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                                        strPrint = strPrint + strTemp;

                                                                        var strVATSales = SetPrefixPostfix(AccountingFormat(vDataFooter.SalesPaymentData.Rows[0].TaxSales || '0'), intFAFCTotalLenght - 20, ' ', 0);
                                                                        var strVATAmount = SetPrefixPostfix(AccountingFormat(vDataFooter.SalesPaymentData.Rows[0].TaxAmount || '0'), intFAFCTotalLenght - 15, ' ', 0);
                                                                        var strNonVAT = SetPrefixPostfix('0.00', intFAFCTotalLenght - 22, ' ', 0);
                                                                        var strVATExempt = SetPrefixPostfix('0.00', intFAFCTotalLenght - 22, ' ', 0);
                                                                        var strVATZeroRated = SetPrefixPostfix('0.00', intFAFCTotalLenght - 25, ' ', 0);

                                                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-able Sales: ', 18, strVATSales, intFAFCTotalLenght, '', 0, 0);
                                                                        strPrint = strPrint + strTemp;

                                                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT Amount: ', 13, strVATAmount, intFAFCTotalLenght, '', 0, 0);
                                                                        strPrint = strPrint + strTemp;

                                                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Non-VAT Sales: ', 20, strNonVAT, intFAFCTotalLenght, '', 0, 0);
                                                                        strPrint = strPrint + strTemp;

                                                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT Exempt Sales: ', 20, strVATExempt, intFAFCTotalLenght, '', 0, 0);
                                                                        strPrint = strPrint + strTemp;

                                                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Zero-Rated VAT Sales: ', 23, strVATZeroRated, intFAFCTotalLenght, '', 0, 0);
                                                                        strPrint = strPrint + strTemp;

                                                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                                        strPrint = strPrint + strTemp;

                                                                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                                        //Get Footer Messages
                                                                        strPrint = '';
                                                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'This Serves as Your Official Receipt.', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                                        strPrint = strPrint + strTemp;

                                                                        AjaxSend({
                                                                            url: "SalesPayments/getSalesPaymentReceiptFooterMessagesInfo",
                                                                            data: pData,
                                                                            success: function (vDataMsg) {

                                                                                for (var p = 0; p <= vDataMsg.SalesPaymentData.Rows.length - 1; p++) {
                                                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataMsg.SalesPaymentData.Rows[p].MessageFooter || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                                                                    strPrint = strPrint + strTemp;
                                                                                };

                                                                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                                                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'This business is made more efficient by Mobiz. Visit us at www.mobiz-apps.com', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                                                //Print Barcode
                                                                                strPrint = 'SI|' + vDataInvHeader.SalesPaymentData.Rows[0].OrganizationID + '|' + vDataInvHeader.SalesPaymentData.Rows[0].OrganizationBranchID + '|' + SetPrefixPostfix($.trim((vDataInvHeader.SalesPaymentData.Rows[0].InvoiceNumber || '')), 12, '0', 0)
                                                                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 1, 0, 0);

                                                                                strPrint = SetPrefixPostfix($.trim((vDataInvHeader.SalesPaymentData.Rows[0].InvoiceNumber || '')), 12, '0', 0);
                                                                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 1, 0);

                                                                                ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 1);

                                                                                PrintText(strPrinterIP);

                                                                                //Print SRV
                                                                                PrintSalesPaymentSRVReceipt(0, pInvoiceID);

                                                                            }
                                                                        });

                                                                    }
                                                                });

                                                            }
                                                        });

                                                    }
                                                });

                                            }
                                        });

                                    }
                                }
                            });

                        }
                        else {
                            MessageBox("No available printer for this branch. Please contact your system administrator.",
                                   "Printer",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                            );
                        }
                    }
                });

            }

            function PrintPOSDesktopReceipt(pInvoiceID) {
                var strTemp = '';
                var strPrint = '';
                var IsShowSalesRep = false;
                var pData = {};
                pData.InvoiceID = pInvoiceID;

                var japplet = document.jzebra;

                //Get Header
                AjaxSend({
                    url: "SalesPayments/getReceiptHeaderInfo",
                    data: pData,
                    success: function (vDataHeader) {

                        IsShowSalesRep = false;
                        if (vDataHeader.SalesPaymentData.Rows.length > 0) {

                            IsShowSalesRep = (vDataHeader.SalesPaymentData.Rows[0].IsShowSalesRep == 1 ? true : false);

                            strPrint = SetTextAlign($.trim(vDataHeader.SalesPaymentData.Rows[0].BusinessTradeName || ''), 1, intDesktopPrintLenght);
                            japplet.append(strPrint + "\n");

                            strPrint = SetTextAlign($.trim(vDataHeader.SalesPaymentData.Rows[0].CorporateName || ''), 1, intDesktopPrintLenght);
                            japplet.append(strPrint + "\n");

                            strPrint = SetTextAlign("TIN " + $.trim(vDataHeader.SalesPaymentData.Rows[0].TIN || ''), 1, intDesktopPrintLenght);
                            japplet.append(strPrint + "\n");

                            strPrint = SetTextAlign($.trim(vDataHeader.SalesPaymentData.Rows[0].CompleteAddress || ''), 1, intDesktopPrintLenght);
                            japplet.append(strPrint + "\n");

                            strPrint = SetTextAlign("Tel. No. " + $.trim(vDataHeader.SalesPaymentData.Rows[0].TelNo || ''), 1, intDesktopPrintLenght);
                            japplet.append(strPrint + "\n");

                            strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                            japplet.append(strPrint + "\n");

                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('POS Accred. No.', 0, 15), 15, ': ' + $.trim((vDataHeader.SalesPaymentData.Rows[0].POSAccreditationNo || '')), intDesktopPrintLenght, '', 0);
                            japplet.append(strPrint + "\n");

                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('POS Reg. No.', 0, 12), 12, ': ' + $.trim((vDataHeader.SalesPaymentData.Rows[0].POSPermitNo || '')), intDesktopPrintLenght, '', 0);
                            japplet.append(strPrint + "\n");

                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Machine ID', 0, 10), 10, ': ' + vDataHeader.SalesPaymentData.Rows[0].MIN, intDesktopPrintLenght, '', 0);
                            japplet.append(strPrint + "\n");

                            strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                            japplet.append(strPrint + "\n");


                            //Get Items
                            AjaxSend({
                                url: "SalesPayments/getSalesPaymentInvoiceHeaderInfo",
                                data: pData,
                                success: function (vDataInvHeader) {

                                    strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Customer            ', 0, 20), 20, ': ' + $.trim((vDataInvHeader.SalesPaymentData.Rows[0].CustomerName || '')), intDesktopPrintLenght, '', 0);
                                    japplet.append(strPrint + "\n");

                                    strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Address             ', 0, 20), 20, ': ' + $.trim((vDataInvHeader.SalesPaymentData.Rows[0].CustomerAddress || '')), intDesktopPrintLenght, '', 0);
                                    japplet.append(strPrint + "\n");

                                    strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('TIN                 ', 0, 20), 20, ': ' + $.trim((vDataInvHeader.SalesPaymentData.Rows[0].CustomerTIN || '')), intDesktopPrintLenght, '', 0);
                                    japplet.append(strPrint + "\n");

                                    strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                    japplet.append(strPrint + "\n");

                                    strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Sales Inv. No. ', 0, 15), 15, ': ' + SetPrefixPostfix($.trim((vDataInvHeader.SalesPaymentData.Rows[0].InvoiceNumber || '')), 12, '0', 0), intDesktopPrintLenght, '', 0);
                                    japplet.append(strPrint + "\n");

                                    strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Date/Time      ', 0, 15), 15, ': ' + ($.format.date(new Date(vDataInvHeader.SalesPaymentData.Rows[0].InvoiceDate), 'MMM dd, yyyy HH:mm') || ''), intDesktopPrintLenght, '', 0);
                                    japplet.append(strPrint + "\n");

                                    strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Cashier        ', 0, 15), 15, ': ' + ($.trim(vDataInvHeader.SalesPaymentData.Rows[0].CashierName || '')), intDesktopPrintLenght, '', 0);
                                    japplet.append(strPrint + "\n");

                                    strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                    japplet.append(strPrint + "\n");

                                    //Get Items
                                    AjaxSend({
                                        url: "SalesPayments/getSalesPaymentReceiptBodyInfo",
                                        data: pData,
                                        success: function (vDataItem) {

                                            strPrint = '';
                                            for (var p = 0; p <= vDataItem.SalesPaymentData.Rows.length - 1; p++) {

                                                var strStockDescription = $.trim((vDataItem.SalesPaymentData.Rows[p].StockDescription || ''));
                                                var strUnitPrice = SetPrefixPostfix(AccountingFormat(vDataItem.SalesPaymentData.Rows[p].UnitPrice || '0'), intDesktopPrintLenght - 34, ' ', 0);
                                                var strQty = SetPrefixPostfix(AccountingFormat((vDataItem.SalesPaymentData.Rows[p].Quantity || '0'), 0), 8, ' ', 0);
                                                var strUnitMeasure = SetPrefixPostfix((vDataItem.SalesPaymentData.Rows[p].UnitMeasure || ''), 10, ' ', 0);
                                                var strSubTotal = SetPrefixPostfix(AccountingFormat(vDataItem.SalesPaymentData.Rows[p].SubTotal || '0'), intDesktopPrintLenght - 26, ' ', 0);


                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, strStockDescription, 32, strUnitPrice, intDesktopPrintLenght, '', 0);
                                                japplet.append(strPrint + "\n");

                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, strQty, 8, strUnitMeasure, 24, strSubTotal, intDesktopPrintLenght);
                                                japplet.append(strPrint + "\n");

                                                if (IsShowSalesRep) {

                                                    strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Sales Person: ' + ($.trim(vDataItem.SalesPaymentData.Rows[p].SalesPerson || '')), intDesktopPrintLenght, '', 0, '', 0, 0);
                                                    japplet.append(strPrint + "\n");

                                                }

                                                strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                                japplet.append(strPrint + "\n");

                                            };

                                            //Get Footer Amounts
                                            AjaxSend({
                                                url: "SalesPayments/getSalesPaymentReceiptPaymentInfo",
                                                data: pData,
                                                success: function (vDataPayment) {

                                                    //Get Footer Amounts
                                                    AjaxSend({
                                                        url: "SalesPayments/getSalesPaymentReceiptFooterAmountsInfo",
                                                        data: pData,
                                                        success: function (vDataFooter) {

                                                            var strSKUCount = SetPrefixPostfix(AccountingFormat((vDataFooter.SalesPaymentData.Rows[0].SKUCount || '0'), 0), intDesktopPrintLenght - 13, ' ', 0);
                                                            var strTotalAmountDue = SetPrefixPostfix(AccountingFormat(vDataFooter.SalesPaymentData.Rows[0].TotalAmountDue || '0'), intDesktopPrintLenght - 20, ' ', 0);
                                                            var strTotalDiscount = SetPrefixPostfix(AccountingFormat(vDataFooter.SalesPaymentData.Rows[0].DiscountAmount || '0'), intDesktopPrintLenght - 20, ' ', 0);
                                                            var strNetAmountDue = SetPrefixPostfix(AccountingFormat(vDataFooter.SalesPaymentData.Rows[0].NetAmountDue || '0'), intDesktopPrintLenght - 20, ' ', 0);

                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'SKU Count: ', 11, strSKUCount, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Sub Total: ', 18, strTotalAmountDue, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Total Discount: ', 18, strTotalDiscount, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Amount Due: ', 18, strNetAmountDue, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            if (vDataPayment.SalesPaymentData.Rows.length > 0) {

                                                                var strCashPayment = SetPrefixPostfix(AccountingFormat((vDataPayment.SalesPaymentData.Rows[0].CashPayment || '0')), intDesktopPrintLenght - 8, ' ', 0);
                                                                var strCheckPayment = SetPrefixPostfix(AccountingFormat(vDataPayment.SalesPaymentData.Rows[0].CheckPaymentAmount || '0'), intDesktopPrintLenght - 9, ' ', 0);
                                                                var strDebitCardAmount = SetPrefixPostfix(AccountingFormat(vDataPayment.SalesPaymentData.Rows[0].DebitCardAmount || '0'), intDesktopPrintLenght - 15, ' ', 0);
                                                                var strCreditCardAmount = SetPrefixPostfix(AccountingFormat(vDataPayment.SalesPaymentData.Rows[0].CreditCardAmount || '0'), intDesktopPrintLenght - 15, ' ', 0);
                                                                var strSRVAmount = SetPrefixPostfix(AccountingFormat(vDataPayment.SalesPaymentData.Rows[0].SRVAmount || '0'), intDesktopPrintLenght - 7, ' ', 0);
                                                                var strCustomerChangeCash = SetPrefixPostfix(AccountingFormat(vDataPayment.SalesPaymentData.Rows[0].CustomerChangeCash || '0'), intDesktopPrintLenght - 10, ' ', 0);

                                                                japplet.append("\n");

                                                                if (vDataPayment.SalesPaymentData.Rows[0].CashPayment > 0) {
                                                                    strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Cash: ', 6, strCashPayment, intDesktopPrintLenght, '', 0);
                                                                    japplet.append(strPrint + "\n");
                                                                }
                                                                if (vDataPayment.SalesPaymentData.Rows[0].CheckPaymentAmount > 0) {
                                                                    strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Check: ', 7, strCheckPayment, intDesktopPrintLenght, '', 0);
                                                                    japplet.append(strPrint + "\n");
                                                                }
                                                                if (vDataPayment.SalesPaymentData.Rows[0].CreditCardAmount > 0) {
                                                                    strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Debit Card: ', 13, strDebitCardAmount, intDesktopPrintLenght, '', 0);
                                                                    japplet.append(strPrint + "\n");
                                                                }
                                                                if (vDataPayment.SalesPaymentData.Rows[0].CreditCardAmount > 0) {
                                                                    strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Credit Card: ', 13, strCreditCardAmount, intDesktopPrintLenght, '', 0);
                                                                    japplet.append(strPrint + "\n");
                                                                }
                                                                if (vDataPayment.SalesPaymentData.Rows[0].SRVAmount > 0) {
                                                                    strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'SRV: ', 5, strSRVAmount, intDesktopPrintLenght, '', 0);
                                                                    japplet.append(strPrint + "\n");
                                                                }

                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Change: ', 8, strCustomerChangeCash, intDesktopPrintLenght, '', 0);
                                                                japplet.append(strPrint + "\n");

                                                                if(vDataPayment.SalesPaymentData.Rows[0].CheckPaymentAmount > vDataFooter.SalesPaymentData.Rows[0].NetAmountDue){
                                                                    strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, '* Excess payments made through checks are not entitled to cash refunds.', intDesktopPrintLenght, '', 0, '', 0);
                                                                    japplet.append(strPrint + "\n");
                                                                }

                                                            }

                                                            strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                                            japplet.append(strPrint + "\n");

                                                            var strVATSales = SetPrefixPostfix(AccountingFormat(vDataFooter.SalesPaymentData.Rows[0].TaxSales || '0'), intDesktopPrintLenght - 18, ' ', 0);
                                                            var strVATAmount = SetPrefixPostfix(AccountingFormat(vDataFooter.SalesPaymentData.Rows[0].TaxAmount || '0'), intDesktopPrintLenght - 14, ' ', 0);
                                                            var strNonVAT = SetPrefixPostfix('0.00', intDesktopPrintLenght - 20, ' ', 0);
                                                            var strVATExempt = SetPrefixPostfix('0.00', intDesktopPrintLenght - 20, ' ', 0);
                                                            var strVATZeroRated = SetPrefixPostfix('0.00', intDesktopPrintLenght - 24, ' ', 0);

                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'VAT-able Sales: ', 16, strVATSales, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'VAT Amount: ', 12, strVATAmount, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Non-VAT Sales: ', 18, strNonVAT, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'VAT Exempt Sales: ', 18, strVATExempt, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'VAT Zero-Rated Sales: ', 22, strVATZeroRated, intDesktopPrintLenght, '', 0);
                                                            japplet.append(strPrint + "\n");

                                                            strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                                            japplet.append(strPrint + "\n");

                                                            //Get Footer Messages
                                                            strPrint = SetTextAlign('This Serves as Your Official Receipt.', 1, intDesktopPrintLenght);
                                                            japplet.append(strPrint + "\n");

                                                            AjaxSend({
                                                                url: "SalesPayments/getSalesPaymentReceiptFooterMessagesInfo",
                                                                data: pData,
                                                                success: function (vDataMsg) {

                                                                    for (var p = 0; p <= vDataMsg.SalesPaymentData.Rows.length - 1; p++) {
                                                                        if (p <= vDataMsg.SalesPaymentData.Rows.length - 1) {
                                                                            strPrint = SetTextAlign($.trim((vDataMsg.SalesPaymentData.Rows[p].MessageFooter || '')), 1, intDesktopPrintLenght);
                                                                            japplet.append(strPrint + "\n");
                                                                        }
                                                                    };

                                                                    strPrint = SetTextAlign('This business is made more efficient by Mobiz', 1, intDesktopPrintLenght);
                                                                    japplet.append(strPrint + "\n");

                                                                    strPrint = SetTextAlign('Visit us at www.mobiz-apps.com', 1, intDesktopPrintLenght);
                                                                    japplet.append(strPrint + "\n");

                                                                    japplet.append("\n\n\n\n\n\n\n\n\n\n\n\n\n");

                                                                    //                                                                    japplet.print();

                                                                    //Print SRV
                                                                    PrintSalesPaymentSRVReceipt(0, pInvoiceID);


                                                                }
                                                            });

                                                        }
                                                    });

                                                }
                                            });

                                        }
                                    });

                                }
                            });

                        }
                    }
                });
            }

            function PrintSalesPaymentSRVReceipt(intSalesPaySRVPrintCnt, pInvoiceID) {


                if (intSalesPaySRVCntr > 0 && intSalesPaySRVPrintCnt <= intSalesPaySRVCntr) {

                    if (sessionStorage["SalesPaySRV-" + intSalesPaySRVPrintCnt] != undefined) {

                        arSalesPaySRV = new Array();
                        arSalesPaySRV = JSON.parse(sessionStorage["SalesPaySRV-" + intSalesPaySRVPrintCnt]);

                        if (InAndroid) {
                            PrintPOSSRVReceipt(arSalesPaySRV[intSalesPaySRVMemoID], intSalesPaySRVPrintCnt, pInvoiceID);
                        } else {
                            PrintPOSDesktopSRVReceipt(arSalesPaySRV[intSalesPaySRVMemoID], intSalesPaySRVPrintCnt, pInvoiceID);
                        }
                    } else {
                        PrintSalesPaymentSRVReceipt(intSalesPaySRVPrintCnt + 1, pInvoiceID);
                    }

                } else {

                    if (!InAndroid) {
                        var japplet = document.jzebra;
                        var blnIsPrinterFound = false;

                        if (japplet != null) {
                            while (!blnIsPrinterFound) {

                                if (FindDesktopPrinter("")) {
                                    blnIsPrinterFound = true;

                                    japplet.print();
                                    jzebraDonePrinting(pInvoiceID);

                                };

                            }
                        } else {
                            MessageBox("Jzebra applet was not loaded.",
                                    "Jzebra",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                            );
                        }
                    }else{
                        setTimeout(VerifyPrintReceipt(pInvoiceID), 2000)
                    }

                }

            }

            function jzebraDonePrinting(pInvoiceID){
                var japplet = document.jzebra;
                var IsDonePrinting = false;
                var intCntr = 0;

                if (japplet != null) {

                    IsDonePrinting = false;
                    while(!IsDonePrinting && intCntr < 30000){
                        if (japplet.isDonePrinting()) {

                            IsDonePrinting = true;

	                        var e = japplet.getException();
                            if(e == null){//Success
                                setTimeout(VerifyPrintReceipt(pInvoiceID), 2000);
                                return;
                            }else{
                                MessageBox("Unable to print the receipt. Please check your printer and press OK to reprint the receipt.",
                                        "Printer Problem",
                                        [
                                            { Text: 'Ok',
                                                Click: function () {
                                                    $.mobile.changePage(window.pageSourcePage);
                                                    PrintReceipt(pInvoiceID);
                                                    return;
                                                }
                                            }
                                        ]
                                );
                            }
	                    }

                        intCntr = intCntr + 1;
                    }

                    if(!IsDonePrinting){//Printing takes too much time.
                        MessageBox("Unable to print the receipt. Please check your printer and press OK to reprint the receipt.",
                                "Printer Problem",
                                [
                                    { Text: 'Ok',
                                        Click: function () {
                                            $.mobile.changePage(window.pageSourcePage);
                                            PrintReceipt(pInvoiceID);
                                            return;
                                        }
                                    }
                                ]
                        );
                    }

                } else {
                    MessageBox("Jzebra applet was not loaded.",
                            "Jzebra",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                }
            }

            function VerifyPrintReceipt(pInvoiceID){
                MessageBox("Your transaction has been saved. Were you able to successfully print out a receipt?",
                            "Sales Payment",
                            [
                                { Text: 'Yes',
                                    Click: function () {
                                        window.location = GetPathFormat("/SalesMenu?mode=pos&OrgID=" + sessionStorage.OrgID);
                                    }
                                },
                                { Text: 'No',
                                    Click: function () {
                                        
                                        window.pageSecuritySourcePage = $.mobile.activePage;
                                        PromptUserAccess('POS Receipt Reprint', {
                                            OnClickOKSuccess: function () {

                                                $.mobile.changePage(window.pageSourcePage);
                                                PrintReceipt(pInvoiceID);
                                            }
                                        });
                                    }
                                }
                            ], null, true
                );
            }

            function PrintPOSSRVReceipt(pMemoID, intSalesPaySRVPrintCnt, pInvoiceID) {
                var strTemp = '';
                var strPrint = '';
                var strPrinterIP = '';
                var pData = {};

                AjaxSend({
                    url: "Organization/getOrgFOWSTPrinterIP",
                    success: function (vDataPrint) {

                        if (vDataPrint.OrganizationFOWSTData.Rows.length > 0) {
                            
                            strPrinterIP = vDataPrint.OrganizationFOWSTData.Rows[0].PrinterIPAddress;

                            pData.MemoID = pMemoID;

                            AjaxSend({
                                url: "SalesPayments/getSalesPaymentSRVReceiptHeaderInfo",
                                data: pData,
                                success: function (vDataSRVChange) {
                                    if (vDataSRVChange.SalesPaymentData.Rows.length > 0) {
                                        if (parseFloat(vDataSRVChange.SalesPaymentData.Rows[0].SRVNewAmount || 0) <= 0) {
                                            PrintSalesPaymentSRVReceipt(intSalesPaySRVPrintCnt + 1, pInvoiceID);
                                        }else{

                                            //Get Header
                                            AjaxSend({
                                                url: "SalesPayments/getReceiptHeaderInfo",
                                                data: pData,
                                                success: function (vDataRHeader) {

                                                    if (vDataRHeader.SalesPaymentData.Rows.length > 0) {

                                                        //New set of text to print
                                                        NewPrintBuilder();

                                                        strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataRHeader.SalesPaymentData.Rows[0].BusinessTradeName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                                        ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                                                        strPrint = '';
                                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataRHeader.SalesPaymentData.Rows[0].CorporateName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                                        strPrint = strPrint + strTemp;

                                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'TIN ' + $.trim((vDataRHeader.SalesPaymentData.Rows[0].TIN || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                                        strPrint = strPrint + strTemp;

                                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataRHeader.SalesPaymentData.Rows[0].CompleteAddress || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                                        strPrint = strPrint + strTemp;

                                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Tel. No.' + $.trim((vDataRHeader.SalesPaymentData.Rows[0].TelNo || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                                        strPrint = strPrint + strTemp;

                                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                        strPrint = strPrint + strTemp;

                                                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                        //Get SRV Deatils
                                                        AjaxSend({
                                                            url: "SalesPayments/getSalesPaymentSRVReceiptHeaderInfo",
                                                            data: pData,
                                                            success: function (vDataSRV) {

                                                                if (vDataSRV.SalesPaymentData.Rows.length > 0) {

                                                                    strPrint = '';
                                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Customer', 8, ' ', 1), 7, ': ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerName || '')), intFAFCTotalLenght, '', 0, 0);
                                                                    strPrint = strPrint + strTemp;

                                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Address', 8, ' ', 1), 7, ': ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerAddress || '')), intFAFCTotalLenght, '', 0, 0);
                                                                    strPrint = strPrint + strTemp;

                                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('TIN', 8, ' ', 1), 7, ': ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerTIN || '')), intFAFCTotalLenght, '', 0, 0);
                                                                    strPrint = strPrint + strTemp;
                                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                                    strPrint = FormatTextToPrint(intFCDoubleTotalLenght, 'SRV ' + SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].SRVNumber || '')), 12, '0', 0), intFCDoubleTotalLenght, '', 0, '', 0, 1);
                                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                                                                    //Print QR
                                                                    strPrint = SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].SRVNumber || '')), 12, '0', 0);
                                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 1, 0);

                                                                    strPrint = FormatTextToPrint(intFCDoubleTotalLenght, 'Pin ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].PinCode || '')), intFCDoubleTotalLenght, '', 0, '', 0, 1);
                                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                                                                    //Print Barcode 1D
                                                                    strPrint = $.trim((vDataSRV.SalesPaymentData.Rows[0].PinCode || ''));
                                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 1, 0);

                                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                                    strPrint = '';
                                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Bought From', 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].BoughtToBranchName || '')), intFAFCTotalLenght, '', 0, 0);
                                                                    strPrint = strPrint + strTemp;

                                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Returned To', 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].ReturnToBranchName || '')), intFAFCTotalLenght, '', 0, 0);
                                                                    strPrint = strPrint + strTemp;

                                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Processed By', 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].CashierName || '')), intFAFCTotalLenght, '', 0, 0);
                                                                    strPrint = strPrint + strTemp;

                                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Approved By', 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].CashierName || '')), intFAFCTotalLenght, '', 0, 0);
                                                                    strPrint = strPrint + strTemp;

                                                                    ReadyTextToPrint('\n' + strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                                    var strInvoiceNo = SetPrefixPostfix(SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].InvoiceNumber || '')), 12, '0', 0), intFAFCTotalLenght - 13, ' ', 0);
                                                                    var strInvoiceAmount = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].InvoiceAmount || '0'), intFAFCTotalLenght - 16, ' ', 0);
                                                                    var strInvoicePayment = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].InvoicePayment || '0'), intFAFCTotalLenght - 24, ' ', 0);

                                                                    strPrint = '';
                                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Invoice No.', 11, strInvoiceNo, intFAFCTotalLenght, '', 0, 0);
                                                                    strPrint = strPrint + strTemp;

                                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Invoice Amount', 14, strInvoiceAmount, intFAFCTotalLenght, '', 0, 0);
                                                                    strPrint = strPrint + strTemp;

                                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Less: Invoice Payment', 22, strInvoicePayment, intFAFCTotalLenght, '', 0, 0);
                                                                    strPrint = strPrint + strTemp;
                                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                                    var dblInvBalance = (vDataSRV.SalesPaymentData.Rows[0].InvoiceAmount || 0) - (vDataSRV.SalesPaymentData.Rows[0].InvoicePayment || 0);
                                                                    var strInvoiceBalance = SetPrefixPostfix(AccountingFormat(dblInvBalance), intFAFCTotalLenght - 17, ' ', 0);
                                                                    var strValueOfReturns = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVOrigAmount || '0'), intFAFCTotalLenght - 18, ' ', 0);
                                                                    var strRestockingFee = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVRestockingFee || '0'), intFAFCTotalLenght - 31, ' ', 0);

                                                                    strPrint = '';
                                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Invoice Balance Net of Payment', 15, strInvoiceBalance, intFAFCTotalLenght, '', 0, 0);
                                                                    strPrint = strPrint + strTemp;

                                                                    strPrint = strPrint + '\n';

                                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Value of Returns', 16, strValueOfReturns, intFAFCTotalLenght, '', 0, 0);
                                                                    strPrint = strPrint + strTemp;

                                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Less: One-Time Restocking Fee', 29, strRestockingFee, intFAFCTotalLenght, '', 0, 0);
                                                                    strPrint = strPrint + strTemp;

                                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                                    var strNetValueOfReturns = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount || '0'), intFAFCTotalLenght - 22, ' ', 0);

                                                                    strPrint = '';
                                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Net Value of Returns', 20, strNetValueOfReturns, intFAFCTotalLenght, '', 0, 0);
                                                                    strPrint = strPrint + strTemp;

                                                                    strPrint = strPrint + '\n';

                                                                    var dblInvoiceBalanceNetPaymentAndReturns = dblInvBalance - vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount;
                                                                    var strInvoiceBalanceNetPaymentAndReturns = SetPrefixPostfix(AccountingFormat(dblInvoiceBalanceNetPaymentAndReturns), intFAFCTotalLenght - 22, ' ', 0);

                                                                    if (dblInvoiceBalanceNetPaymentAndReturns < 0) {
                                                                        strInvoiceBalanceNetPaymentAndReturns = 'NONE';
                                                                    }

                                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Invoice Balance Net  Payments and Returns', 20, strInvoiceBalanceNetPaymentAndReturns, intFAFCTotalLenght, '', 0, 0);
                                                                    strPrint = strPrint + strTemp;

                                                                    ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                                    //Get SRV Deatils
                                                                    AjaxSend({
                                                                        url: "SalesPayments/getSalesPaymentSRVReceiptInvoices",
                                                                        data: pData,
                                                                        success: function (vDataSRVInv) {

                                                                            if (vDataSRVInv.SalesPaymentData.Rows.length > 0) {

                                                                                strPrint = '';
                                                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Less: Used Portion of SRV', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                                                strPrint = strPrint + strTemp;

                                                                                for (var p = 0; p <= vDataSRVInv.SalesPaymentData.Rows.length - 1; p++) {

                                                                                    var strInvAndDate = 'SI' + SetPrefixPostfix($.trim((vDataSRVInv.SalesPaymentData.Rows[p].InvoiceNumber || '')), 12, '0', 0) + ' ' + ($.format.date(new Date(vDataSRVInv.SalesPaymentData.Rows[p].InvoiceDate), 'MM/dd/yy HH:mm') || '');
                                                                                    var strUsedAmount = SetPrefixPostfix(AccountingFormat(vDataSRVInv.SalesPaymentData.Rows[p].UsedAmount || '0'), intFAFCTotalLenght - 30, ' ', 0);


                                                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, strInvAndDate, 28, strUsedAmount, intFAFCTotalLenght, '', 0, 0);
                                                                                    strPrint = strPrint + strTemp;

                                                                                };

                                                                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                                            }

                                                                            strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                                            var strNetSRV = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount || '0'), intFAFCTotalLenght - 18, ' ', 0);

                                                                            strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Net Value of SRV', 16, strNetSRV, intFAFCTotalLenght, '', 0, 0);
                                                                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                                                            strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                                            strPrint = FormatTextToPrint(intFAFCTotalLenght, 'This SRV is as good as cash.', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                                                            strPrint = '';
                                                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, '* Present and surrender this SRV to       cashier as form of payment.', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                                            strPrint = strPrint + strTemp;

                                                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, '* Do not lose or crumple this receipt.', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                                            strPrint = strPrint + strTemp;

                                                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, '* To get electronic copy of this SRV,     please register with Customer Service.', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                                            strPrint = strPrint + strTemp;

                                                                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                                            strPrint = '\n';
                                                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'This business is made more efficient by Mobiz. Visit us at www.mobiz-apps.com', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                                            strPrint = strPrint + strTemp;

                                                                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                                            //Print Barcode
                                                                            strPrint = 'SRV|' + vDataSRV.SalesPaymentData.Rows[0].SubscriberOrganizationID + '|' + vDataSRV.SalesPaymentData.Rows[0].ReturnToBranchID + '|' + SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].SRVNumber || '')), 12, '0', 0) + '|' + $.trim((vDataSRV.SalesPaymentData.Rows[0].PinCode || ''));
                                                                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 1, 0, 0);

                                                                            ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 1);

                                                                            PrintText(strPrinterIP);
                                                                            PrintSalesPaymentSRVReceipt(intSalesPaySRVPrintCnt + 1, pInvoiceID);
                                                                        }
                                                                    });
                                                                }
                                                            }
                                                        });

                                                    } else {
                                                        MessageBox("This branch has no receipt settings. Please contact your system adminstrator.",
                                                               "Receipt Settings",
                                                                [
                                                                    { Text: 'Ok'
                                                                    }
                                                                ]
                                                        );
                                                    }

                                                }
                                            });
                                        }
                                    }
                                }
                            });
                        }
                        else {
                            MessageBox("No available printer for this branch. Please contact your system administrator.",
                                   "Printer",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                            );
                        }

                    }
                });

            }

            function PrintPOSDesktopSRVReceipt(pMemoID, intSalesPaySRVPrintCnt, pInvoiceID) {
                var strTemp = '';
                var strPrint = '';
                var pData = {};
                var japplet = document.jzebra;

                pData.MemoID = pMemoID;

                AjaxSend({
                    url: "SalesPayments/getSalesPaymentSRVReceiptHeaderInfo",
                    data: pData,
                    success: function (vDataSRVChange) {
                        if (vDataSRVChange.SalesPaymentData.Rows.length > 0) {
                            if (parseFloat(vDataSRVChange.SalesPaymentData.Rows[0].SRVNewAmount || 0) <= 0) {
                                PrintSalesPaymentSRVReceipt(intSalesPaySRVPrintCnt + 1, pInvoiceID);
                            }else{
                                //Get Header
                                AjaxSend({
                                    url: "SalesPayments/getReceiptHeaderInfo",
                                    data: pData,
                                    success: function (vDataHeader) {

                                        if (vDataHeader.SalesPaymentData.Rows.length > 0) {


                                            strPrint = SetTextAlign($.trim(vDataHeader.SalesPaymentData.Rows[0].BusinessTradeName || ''), 1, intDesktopPrintLenght);
                                            japplet.append(strPrint + "\n");

                                            strPrint = SetTextAlign($.trim(vDataHeader.SalesPaymentData.Rows[0].CorporateName || ''), 1, intDesktopPrintLenght);
                                            japplet.append(strPrint + "\n");

                                            strPrint = SetTextAlign("TIN " + $.trim(vDataHeader.SalesPaymentData.Rows[0].TIN || ''), 1, intDesktopPrintLenght);
                                            japplet.append(strPrint + "\n");

                                            strPrint = SetTextAlign($.trim(vDataHeader.SalesPaymentData.Rows[0].CompleteAddress || ''), 1, intDesktopPrintLenght);
                                            japplet.append(strPrint + "\n");

                                            strPrint = SetTextAlign("Tel. No. " + $.trim(vDataHeader.SalesPaymentData.Rows[0].TelNo || ''), 1, intDesktopPrintLenght);
                                            japplet.append(strPrint + "\n");

                                            strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                            japplet.append(strPrint + "\n");

                                            //Get SRV Deatils
                                            AjaxSend({
                                                url: "SalesPayments/getSalesPaymentSRVReceiptHeaderInfo",
                                                data: pData,
                                                success: function (vDataSRV) {

                                                    if (vDataSRV.SalesPaymentData.Rows.length > 0) {

                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Customer ', 0, 9), 9, ': ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerName || '')), intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Address  ', 0, 9), 9, ': ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerAddress || '')), intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('TIN      ', 0, 9), 9, ': ' + $.trim((vDataSRV.SalesPaymentData.Rows[0].CustomerTIN || '')), intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('SRV No. ', 0, 8), 8, ': ' + SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].SRVNumber || '')), 12, '0', 0), intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Pin     ', 0, 8), 8, ': ' + $.trim(vDataSRV.SalesPaymentData.Rows[0].PinCode || ''), intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Date and Time', 0, 14), 14, ': ' + ($.format.date(new Date(vDataSRV.SalesPaymentData.Rows[0].SRVDateTime), 'MMM dd, yyyy HH:mm') || ''), intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Bought From ', 0, 12), 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].BoughtToBranchName || '')), intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Returned To ', 0, 12), 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].ReturnToBranchName || '')), intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Processed By', 0, 12), 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].CashierName || '')), intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Approved By ', 0, 12), 12, ': ' + ($.trim(vDataSRV.SalesPaymentData.Rows[0].CashierName || '')), intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                                        japplet.append(strPrint + "\n");

                                                        var strInvoiceNo = SetPrefixPostfix(SetPrefixPostfix($.trim((vDataSRV.SalesPaymentData.Rows[0].InvoiceNumber || '')), 12, '0', 0), intDesktopPrintLenght - 13, ' ', 0);
                                                        var strInvoiceAmount = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].InvoiceAmount || '0'), intDesktopPrintLenght - 16, ' ', 0);
                                                        var strInvoicePayment = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].InvoicePayment || '0'), intDesktopPrintLenght - 23, ' ', 0);

                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Invoice No.', 0, 11), 11, strInvoiceNo, intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Invoice Amount', 0, 14), 14, strInvoiceAmount, intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Less: Invoice Payment', 0, 21), 21, strInvoicePayment, intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                                        japplet.append(strPrint + "\n");

                                                        var dblInvBalance = (vDataSRV.SalesPaymentData.Rows[0].InvoiceAmount || 0) - (vDataSRV.SalesPaymentData.Rows[0].InvoicePayment || 0);
                                                        var strInvoiceBalance = SetPrefixPostfix(AccountingFormat(dblInvBalance), intDesktopPrintLenght - 32, ' ', 0);
                                                        var strValueOfReturns = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVOrigAmount || '0'), intDesktopPrintLenght - 18, ' ', 0);
                                                        var strRestockingFee = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVRestockingFee || '0'), intDesktopPrintLenght - 31, ' ', 0);

                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Invoice Balance Net of Payment', 0, 30), 30, strInvoiceBalance, intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        japplet.append("\n");
                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Value of Returns', 0, 16), 16, strValueOfReturns, intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Less: One-Time Restocking Fee', 0, 29), 29, strRestockingFee, intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                                        japplet.append(strPrint + "\n");

                                                        var strNetValueOfReturns = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount || '0'), intDesktopPrintLenght - 22, ' ', 0);

                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Net Value of Returns', 0, 20), 20, strNetValueOfReturns, intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");
                                                        japplet.append("\n");

                                                        var dblInvoiceBalanceNetPaymentAndReturns = dblInvBalance - vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount;
                                                        var strInvoiceBalanceNetPaymentAndReturns = SetPrefixPostfix(AccountingFormat(dblInvoiceBalanceNetPaymentAndReturns), intDesktopPrintLenght - 21, ' ', 0);

                                                        if (dblInvoiceBalanceNetPaymentAndReturns < 0) {
                                                            strInvoiceBalanceNetPaymentAndReturns = SetPrefixPostfix('NONE', intDesktopPrintLenght - 21, ' ', 0);
                                                        }

                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Invoice Balance Net', 0, 19), 19, strInvoiceBalanceNetPaymentAndReturns, intDesktopPrintLenght, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Payments and Returns', 0, intDesktopPrintLenght), intDesktopPrintLenght, '', 0, '', 0);
                                                        japplet.append(strPrint + "\n");

                                                        //Get SRV Deatils
                                                        AjaxSend({
                                                            url: "SalesPayments/getSalesPaymentSRVReceiptInvoices",
                                                            data: pData,
                                                            success: function (vDataSRVInv) {

                                                                if (vDataSRVInv.SalesPaymentData.Rows.length > 0) {

                                                                    strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Less: Used Portion of SRV', 0, intDesktopPrintLenght), intDesktopPrintLenght, '', 0, '', 0);
                                                                    japplet.append(strPrint + "\n");

                                                                    for (var p = 0; p <= vDataSRVInv.SalesPaymentData.Rows.length - 1; p++) {

                                                                        var strInvAndDate = 'SI' + SetPrefixPostfix($.trim((vDataSRVInv.SalesPaymentData.Rows[p].InvoiceNumber || '')), 12, '0', 0) + ' ' + ($.format.date(new Date(vDataSRVInv.SalesPaymentData.Rows[p].InvoiceDate), 'MM/dd/yy HH:mm') || '');
                                                                        var strUsedAmount = SetPrefixPostfix(AccountingFormat(vDataSRVInv.SalesPaymentData.Rows[p].UsedAmount || '0'), intDesktopPrintLenght - 30, ' ', 0);

                                                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, strInvAndDate, 28, strUsedAmount, intDesktopPrintLenght, '', 0);
                                                                        japplet.append(strPrint + "\n");

                                                                    };

                                                                }

                                                                strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                var strNetSRV = SetPrefixPostfix(AccountingFormat(vDataSRV.SalesPaymentData.Rows[0].SRVNewAmount || '0'), intDesktopPrintLenght - 18, ' ', 0);

                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Net Value of SRV', 0, 16), 16, strNetSRV, intDesktopPrintLenght, '', 0);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign(' ', 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('This SRV is as good as cash.', 1, intDesktopPrintLenght), intDesktopPrintLenght, '', 0, '', 0);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('* Present and surrender this SRV to cashier.', 0, intDesktopPrintLenght), intDesktopPrintLenght, '', 0, '', 0);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('  as form of payment.', 0, intDesktopPrintLenght), intDesktopPrintLenght, '', 0, '', 0);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('* Do not lose or crumple this receipt.', 0, intDesktopPrintLenght), intDesktopPrintLenght, '', 0, '', 0);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('* To get electronic copy of this SRV, please', 0, intDesktopPrintLenght), intDesktopPrintLenght, '', 0, '', 0);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('  register with Customer Service.', 0, intDesktopPrintLenght), intDesktopPrintLenght, '', 0, '', 0);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign('This business is made more efficient by Mobiz', 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                strPrint = SetTextAlign('Visit us at www.mobiz-apps.com', 1, intDesktopPrintLenght);
                                                                japplet.append(strPrint + "\n");

                                                                japplet.append("\n\n\n\n\n\n\n\n\n\n\n\n\n");

                                                                //                                                japplet.print();
                                                                PrintSalesPaymentSRVReceipt(intSalesPaySRVPrintCnt + 1, pInvoiceID);
                                                            }
                                                        });
                                                    }
                                                }
                                            });

                                        } else {
                                            MessageBox("This branch has no receipt settings. Please contact your system adminstrator.",
                                                   "Receipt Settings",
                                                    [
                                                        { Text: 'Ok'
                                                        }
                                                    ]
                                            );

                                        }

                                    }
                                });
                            }
                        }
                    }
                });

            }
        });
    }); // require SalesPayments_offline
});