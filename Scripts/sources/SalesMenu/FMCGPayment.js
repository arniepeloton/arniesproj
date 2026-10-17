require([
    'common',
    //(OfflineMode ? '/Scripts/sources/SalesPayments/SalesPayments_offline.js?crc=' : '')
], function () {
    $(document).ready(function () {

        window.FmcgPayment = {
            IsPrintReceipt: false,
            IsEmailReceipt: false,
            IsDeliveryInfoPriceQuote: false,
            SaveReceipt: function (isPrintReceipt) {
                if (CDec2(FmcgBasicCart.FinalAmountDue) > CDec2(Popup.amounttendered)) {
                    BlinkMessage('Please pay full amount due');
                    return;
                }

                this.IsPrintReceipt = false;
                this.IsEmailReceipt = false;
                this.IsDeliveryInfoPriceQuote = false;
                var pData = {}
                var DeliveryInfo = SalesPayment_offline().ShowDeliveryInfo(sessionStorage.CartOrderID, sessionStorage.FowstID).Rows[0];
                var pParams = GetUrlParameters();

                pData = {
                    ID: CStr2(pParams.InvoiceID) == '' ? sessionStorage.CartOrderID : pParams.InvoiceID,
                    CustomerID: sessionStorage.CustomerOrganizationID, //$.trim(DeliveryInfo.CustomerID),
                    CusRepresentativeID: $.trim(DeliveryInfo.RepresentativeID),
                    BillToID: $.trim(DeliveryInfo.BillingAddressID),
                    ShipToID: $.trim(DeliveryInfo.DeliveryAddressID),
                    EmailAddress: $.trim(""),
                    PickUpLocationID: $.trim(DeliveryInfo.PickUpLocation),
                    DeliveryMethod: ($.trim(DeliveryInfo.DeliveryMethod) == "Deliver (Downtown)" || $.trim(DeliveryInfo.DeliveryMethod) == "Deliver (Out Of Town)" ? "Deliver" : $.trim(DeliveryInfo.DeliveryMethod)),
                    IsDownTown: DeliveryInfo.IsDownTown,
                    IsRushDelivery: DeliveryInfo.IsRushDelivery,
                    DeliverWithin: DeliveryInfo.DeliverWithin,
                    DeliveryType: DeliveryInfo.DeliveryType,
                    PickupDate: $.format.date(new Date(DeliveryInfo.PickupDateMonth + '-' + DeliveryInfo.PickupDateDay + '-' + DeliveryInfo.PickupDateYear + ' ' + DeliveryInfo.PickupDateHour + ':' + DeliveryInfo.PickupDateMinute + ' ' + DeliveryInfo.PickupDateAMPM)),
                    CashAmount: Popup.Cash.cashTendered, //temp only,
                    IsPrintReceipt: isPrintReceipt
                };

                if (SalesPayment_offline().SaveDeliveryInfo(pData)) {
                    localStorage.removeItem('CartCustomerID');

                    this.SaveSalesPayment(sessionStorage.CartOrderID, false, pData)
                }
            },
            SaveSalesPayment: function (pInvoiceID, blnIsCheckChangeTY, vPdata) {
                var pData = [];
                var pDataDiscount = [];
                var pDataSeniorCitizenPWD = [];
                var pDataEmail = [];
                var pDataCheck = [];
                var pDataDebitCard = [];
                var pDataCreditCard = [];
                var pDataSRV = [];
                var pDataOtherType = [];
                var intIndex;
                var pError = {};

                if (Popup.ChequeGC.arrCGC.length > 0) {
                    var cgcData = Popup.ChequeGC.arrCGC;
                    
                    for (var i = 0; i < cgcData.length; i++) {
                        var date = "";
                        if (cgcData[i][8] != "" && cgcData[i][9] != "" && cgcData[i][10] != "") {
                            date = cgcData[i][8] + '/' + cgcData[i][9] + '/' + cgcData[i][10]
                        }

                        pDataCheck.push({
                            ID: cgcData[i][0],
                            BankID: cgcData[i][1],
                            BankName: cgcData[i][2],
                            BankBranchID: cgcData[i][3],
                            BankBranchName: cgcData[i][4],
                            AccountName: cgcData[i][5],
                            AccountNumber: cgcData[i][6],
                            CheckNumber: cgcData[i][7],
                            CheckDate: date,
                            CheckAmount: cgcData[i][11],
                            RecState: cgcData[i][12],
                        })
                    }
                }

                var debitAmount = 0;
                if(Popup.Card.arrDebit.length > 0){
                    var debitData = Popup.Card.arrDebit;

                    for (var i = 0; i < debitData.length; i++) {
                        var dateIssuanceDate = "";
                        if (debitData[i][11] != "" && debitData[i][12] != "" && debitData[i][13] != "") {
                            dateIssuanceDate = debitData[i][11] + '/' + debitData[i][12] + '/' + debitData[i][13]
                        }

                        var dateExpirationDate = "";
                        if (debitData[i][14] != "" && debitData[i][15] != "" && debitData[i][16] != "") {
                            dateExpirationDate = debitData[i][14] + '/' + debitData[i][15] + '/' + debitData[i][16]
                        }

                        pDataDebitCard.push({
                            ID: debitData[i][0],
                            DebitCardType: debitData[i][1],
                            DebitCardBankID: debitData[i][2],
                            DebitCardBank: debitData[i][3],
                            HolderLastName: debitData[i][4],
                            HolderFirstName: debitData[i][6],
                            HolderMiddleName: debitData[i][5],
                            HolderGenerationalSuffix: debitData[i][7],
                            BankCountryID: debitData[i][8],
                            DebitCardNumber: debitData[i][10],
                            DebitCardIssuanceDate: dateIssuanceDate,
                            DebitCardExpirationDate: dateExpirationDate,
                            ApprovalNo: debitData[i][17],
                            NoOfInstallmentPayments: debitData[i][18],
                            DebitCardAmount: debitData[i][19],
                            RecState: debitData[i][20]
                        })

                        debitAmount += CDec2(debitData[i][19]);
                    }
                }

                var creditAmount = 0;
                if (Popup.Card.arrCredit.length > 0) {
                    var creditData = Popup.Card.arrCredit;

                    for (var i = 0; i < creditData.length; i++) {
                        var dateIssuanceDate = "";
                        if (creditData[i][11] != "" && creditData[i][12] != "" && creditData[i][13] != "") {
                            dateIssuanceDate = creditData[i][11] + '/' + creditData[i][12] + '/' + creditData[i][13]
                        }

                        var dateExpirationDate = "";
                        if (creditData[i][14] != "" && creditData[i][15] != "" && creditData[i][16] != "") {
                            dateExpirationDate = creditData[i][14] + '/' + creditData[i][15] + '/' + creditData[i][16]
                        }

                        pDataDebitCard.push({
                            ID: creditData[i][0],
                            DebitCardType: creditData[i][1],
                            DebitCardBankID: creditData[i][2],
                            DebitCardBank: creditData[i][3],
                            HolderLastName: creditData[i][4],
                            HolderFirstName: creditData[i][6],
                            HolderMiddleName: creditData[i][5],
                            HolderGenerationalSuffix: creditData[i][7],
                            BankCountryID: creditData[i][8],
                            DebitCardNumber: creditData[i][10],
                            DebitCardIssuanceDate: dateIssuanceDate,
                            DebitCardExpirationDate: dateExpirationDate,
                            ApprovalNo: creditData[i][17],
                            NoOfInstallmentPayments: creditData[i][18],
                            DebitCardAmount: creditData[i][19],
                            RecState: creditData[i][20]
                        })

                        creditAmount += CDec2(creditData[i][19]);
                    }
                }

                if (CStr2(Popup.Discount.selDiscountType) != '') {
                    if ($.trim(Popup.Discount.selDiscountType.val()) != '') {
                        pDataDiscount[0] = {
                            ID: $.trim(Popup.Discount.selDiscountType.val()),
                            DiscountPercent: Popup.Discount.longDiscountPercent,
                            DiscountRemarks: $.trim(Popup.Discount.txtDiscountRemarks.val())
                        }
                    }
                }                

                if (Popup.Discount.isSeniorCitizenPWDDiscount) {
                    for (var i = 0; i < Popup.Discount.arrCustomer.length; i++) {
                        pDataSeniorCitizenPWD.push({
                            ID: Popup.Discount.arrCustomer[i][0] //ID
                        })
                    }
                }

                if (Popup.Srv.arrSrv.length > 0) {
                    for (var i = 0; i < Popup.Srv.arrSrv.length; i++){
                        pDataSRV.push({
                            ID: Popup.Srv.arrSrv[i][0],
                            MemoID: Popup.Srv.arrSrv[i][1],
                            ConsumedAmount: Popup.Srv.arrSrv[i][5],
                            RecState: Popup.Srv.arrSrv[i][6],
                            SrvNum: Popup.Srv.arrSrv[i][2],
                            Pin: Popup.Srv.arrSrv[i][3]
                        });
                    }
                }

                pData = {
                    EmailAddress: "",
                    CustomerID: vPdata.CustomerID,
                    InvoiceID: vPdata.ID,
                    IsForGovReport: 0,
                    CashAmount: vPdata.CashAmount,
                    DebitCardAmount: debitAmount,
                    CreditCardAmount: creditAmount,
                    CheckPaymentAmount: Popup.ChequeGC.totalcgc,
                    SRVAmount: Popup.Srv.totalsrv,
                    OtherTypeAmount: Popup.OtherPayment.totalop,
                    ChargeAmount: Popup.Charges.totalcharge,
                    TotalAmountDue: (Popup.Discount.showDiscountDetails ? CDec2($('#lblTotalAmount').text()) : CDec2($('#txtSubTotal').text())),
                    TotalDiscountAmount: Popup.Discount.totaldiscount,
                    TotalAmountTendered: Popup.amounttendered,
                    CustomerChangeCash: CDec2($('#txtBalChange').text()),
                    NoOfPeople: (Popup.Discount.showDiscountDetails ? CDec2($('[name="txtSalesPayNoOfDiners"]').val()) : 0), //for senior citizen discount
                    NoOfSeniorCitizens: (Popup.Discount.showDiscountDetails ? CDec2($('[name="txtSalesPayNoOfSC"]').val()) : 0), //for senior citizen discount
                    SalesPaymentCheckList: pDataCheck,
                    SalesPaymentDebitCardList: pDataDebitCard,
                    SalesPaymentCreditCardList: pDataCreditCard,
                    SalesPaymentSRVList: pDataSRV,
                    SalesPaymentDiscountList: pDataDiscount,
                    SalesPaymentSeniorCitizenPWDList: pDataSeniorCitizenPWD,
                    SalesPaymentOtherTypeList: pDataOtherType,
                    OnCreditAmount: 0
                };
                
                if (!SalesPayment_offline().SaveSalesPayments(pData, pError)) {
                    Android.AddLog(pError.Message);
                    console.log(pError.Message)
                    return;
                }

                //SalesPayment_offline().UpdateSalesTextFile(vPdata.ID);

                //sessionStorage.removeItem('CustomerOrganizationID');
                sessionStorage.IsNewCart = true;
                sessionStorage.removeItem('CustomerOrganizationID');
                Popup.amounttendered = 0; // set 0 after saved so that if user hit ctrl+s or F10 it will prevent to save trans
                BlinkMessage('Successfully Saved!');
                FmcgBasicCart.txtQtyPlu.focus(); // focus on the main txtfield which is in the cart                
                
                if (vPdata.IsPrintReceipt) {
                    //As of March 21, 2019, no realtime printing, need to be fixed
                    //if (FmcgBasicCart.isRealTimePrint) {
                    //    PrintSalesInvoiceFooter();
                    //    //PrintReceiptNow(false, false, true); // FMCGCart.js
                    //} else {
                        //PrintReceiptNow(true, true, true);  // FMCGCart.js

                        var vDataPrinterType = SalesPayment_offline().getOrgFOWSTPrinterType();

                        if (vDataPrinterType.Rows.length > 0) {
                            if (vDataPrinterType.Rows[0].PrinterType == 'Epson TM-U220B Printer') {
                                PrintReceiptNow(true, true, true);
                            } else if (vDataPrinterType.Rows[0].PrinterType == '80MM Mini Thermal Printer') {

                            } else if (vDataPrinterType.Rows[0].PrinterType == 'Windows Print Server') {
                                PrintReceiptViaWindowsPrintServer(vPdata.ID);
                            } else {
                                PrintReceiptNow(true, true, true);
                            }
                        } else {
                            PrintReceiptNow(true, true, true);
                        }
                    //}                    
                } else {
                    NewCart(); // clear cart after save
                }
            }
        }

        $('body')
        .on('click', '#btnFMCGChequeGC', function () {
            Popup.ChequeGC();
        })
        .on('click', '#btnFMCGDiscount', function () {
            Popup.Discount();
        })
        .on('click', '#btnFMCGCharges', function () {
            Popup.Charges();
        })
        .on('click', '#btnFMCGSrv', function () {
            Popup.SRV();
        })
        .on('click', '#btnFMCGOtherPayment', function () {
            Popup.OtherPayment();
        })
        .on('click', '#btnFMCGCash', function () {
            Popup.Cash();
        })
        .on('click', '#btnFMCGMisc', function () {
            KeyF10();
        })
        .on('click', '#btnFMCGCard', function () {
            KeyF6();
        })
        .on('click', '#btnFMCGGiftCheck', function () {
            KeyF3();
        })
        .on('click', '#btnFMCGSave', function () {
            KeyCtrlS();
        })
        .on('click', '#btnFMCGPrint', function () {
            KeyF2();
        })
    })
    .on('keydown', function (e) {
        if (e.which == 113) {
            // F2 for print receipt
            e.preventDefault();
            KeyF2();
        }
        else if ((e.which == '115' || e.which == '83') && (e.ctrlKey || e.metaKey)) {
            // Ctrl + S
            e.preventDefault();
            KeyCtrlS();
        }
        else if (e.which == 114) {
            // F3 (Gift C or Cheque)
            e.preventDefault();
            //if (OfflineMode) {
            //    Android.Toast('Gift Cheque is under construction');
            //}
            //return;
            KeyF3();
        }
        else if (e.which == 117) {
            // F6 (Card)
            e.preventDefault();
            //if (OfflineMode) {
            //    Android.Toast('Card is under construction');
            //}
            //return;
            KeyF6();
        }
        else if (e.which == 121){
            // F10
            e.preventDefault();
            //if (OfflineMode) {
            //    Android.Toast('Misc is under construction');
            //}
            //return;
            KeyF10();
        }
    })
    .on('keyup', function (e) {
        if (e.which == 44) {
            // Print Screen key
            e.preventDefault();
            BlinkMessage('Printing Receipt ...');
            setTimeout(function () {
                FmcgPayment.SaveReceipt(true);                
            }, 200)
        }
    })

    function KeyF10() {
        if (sessionStorage.isNewCart == false || sessionStorage.isNewCart == undefined) {
            if (FmcgBasicCart.CountCart()) {
                //execute when cart is not empty
                Popup.Misc.init();
            }
        }
    }

    function KeyF6() {
        if (sessionStorage.isNewCart == false || sessionStorage.isNewCart == undefined) {
            if (FmcgBasicCart.CountCart()) {
                //execute when cart is not empty
                Popup.Card.init()
            }
        }
    }

    function KeyF3() {
        if (sessionStorage.isNewCart == false || sessionStorage.isNewCart == undefined) {
            if (FmcgBasicCart.CountCart()) {
                //execute when cart is not empty
                Popup.ChequeGC.init();
            }
        }
    }

    function KeyCtrlS() {        
        if (FmcgBasicCart.CartTotalQty > 0) {
            BlinkMessage('Saving ...');
            setTimeout(function () {
                FmcgPayment.SaveReceipt(false);
            }, 300)
        }
        else {
            BlinkMessage('Cart is empty');
        }
    }

    function KeyF2() {
        if (FmcgBasicCart.CartTotalQty > 0) {
            BlinkMessage('Printing Receipt ...');
            setTimeout(function () {
                FmcgPayment.SaveReceipt(true);
            }, 200)
        }
        else {
            BlinkMessage('Cart is empty');
        }
    }
})