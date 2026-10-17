require([
    'common',
    ], function () {
        var IsShowTableNo = 0;
        var IsShowGasPumpNo = 0;
        var IsAutoServiceCharge = 0;
        var mErrorOnLoad = false;
        var OtherPaymentID = '';

        $(function () {
            GetSystemBehavior();

            window.POS = {
                GetPOSData: function () {
                    GetSystemBehavior();
                }
            }

            $('body')
                .on('change', '#selCustomerTableNo', function () {
                if ($('#selCustomerTableNo').is(':checked')) {
                    IsShowTableNo = 1;
                    $('#divCustomerTableNumCount').show();
                } else {
                    IsShowTableNo = 0;
                    $('#divCustomerTableNumCount').hide();
                }
            })
                .on('change', '#selGasPumpNo', function () {
                    if ($('#selGasPumpNo').is(':checked')) {
                        IsShowGasPumpNo = 1;
                        $('#divGasPumpCount').show();
                    } else {
                        IsShowGasPumpNo = 0;
                        $('#divGasPumpCount').hide();
                    }
                })
                .on('click', '[name="btnCustomerInputFieldsCancel"]', function () {
                    //history.back();
                    window.location.hash = 'SystemAdministration';
                })
                .on('click', '[name="btnCustomerInputFieldsSave"]', function () {
                    if (mErrorOnLoad) {
                        NewMessageBox('Saving has been disabled due to error during loading. Please reload page.');
                        return;
                    }
                    if (IsShowTableNo == 1) {
                        if ($.trim($('#txtTableNoLimit').val()) == '') {
                            NewMessageBox("Please enter number of tables.", "No. of Tables");
                            return;
                        } else {
                            if (!validateNumber($.trim($('#txtTableNoLimit').val()))) {
                                NewMessageBox("Please enter a valid number.", "No. of Tables");
                                return;
                            }
                        }
                    }

                    if (IsShowGasPumpNo == 1) {
                        if ($.trim($('#txtGasPumpNoLimit').val()) == '') {
                            NewMessageBox("Please enter number of gas pumps.", "No. of Gas Pumps");
                            return;
                        } else {
                            if (!validateNumber($.trim($('#txtGasPumpNoLimit').val()))) {
                                NewMessageBox("Please enter a valid number.", "No. of Gas Pumps");
                                return;
                            }
                        }
                    }

                    if (IsAutoServiceCharge) {
                        if (CStr2($('#txtServiceCharge').val()) == '') {
                            NewMessageBox("Please enter service charge", "Service Charge");
                            return;
                        }
                    }

                    SaveSystemBehavior();
                })
                .on('click', '#lnkSCPWDInfo', function () {
                    NewMessageBox($(this).find('img').attr('alt'), 'Info');
                })
                .on('change', '#selAutoServiceCharge', function () {
                    IsAutoServiceCharge = CBool($(this).is(':checked'));
                    if (IsAutoServiceCharge) {
                        $('#divServiceChargeField').show();
                    } else {
                        $('#divServiceChargeField').hide();
                    }
                    $('#txtServiceCharge').val('').focus();
                })
                .on('change', '#selRequiredOPRefNumber', function () {
                    if (CBool($(this).is(':checked'))) {
                        $('#divOPRefNumCharLength').show();
                    } else {
                        $('#divOPRefNumCharLength').hide();
                    }
                })
                .on('keydown', '#txtServiceCharge', function (e) {
                    if(!NumberDotOnly($(this), e)){
                        return;
                    }
                })
                .on('click', '#btnNewOtherPayment', function () {
                    OtherPaymentID = '';
                    $('#txtNewOtherPayment, #txtOPRefNumCharLength').val('');
                    $('#selOPRefNumDataType').val('alphanumeric');
                    $('#selRequiredOPRefNumber').prop('checked', 0);
                    $('#popupNewOtherPayment').modal('show').unbind('shown.bs.modal').on('shown.bs.modal', function() {
                        $('#txtNewOtherPayment').focus()
                    })
                })
                .on('click', '#btnSaveNewOtherPayment', function() {
                    var txtNewOtherPayment = $.trim($('#txtNewOtherPayment').val());
                    var selRequiredOPRefNumber = $('#selRequiredOPRefNumber').is(':checked');
                    var txtOPRefNumCharLength = $.trim($('#txtOPRefNumCharLength').val());
                    var selOPRefNumDataType = $.trim($('#selOPRefNumDataType').val());

                    if (txtNewOtherPayment == '') {
                        WebToast('Please enter description', 'top');
                        $('#txtNewOtherPayment').focus()
                        return;
                    }

                    if (selRequiredOPRefNumber && (txtOPRefNumCharLength.length == 0 || CInt2(txtOPRefNumCharLength) == 0)) {
                        WebToast('Please enter reference number character length', 'top');
                        $('#txtOPRefNumCharLength').focus()
                        return;
                    }

                    AjaxSend({
                        url: '/POSSettings/SaveNewOtherPayment',
                        data: {
                            OPID: OtherPaymentID,
                            Description: txtNewOtherPayment,
                            IsRequired: selRequiredOPRefNumber,
                            RefNoCharLength: txtOPRefNumCharLength,
                            DataType: selOPRefNumDataType
                        },
                        success: function (e){
                            if (e.Results == 'saved') {
                                $('#popupNewOtherPayment').modal('hide');
                                WebToast('Successfully saved', 'top');
                                GetOtherPayments();
                            } else {
                                console.log(e.Results)
                            }
                        }
                    })                
                })
                .on('keyup', '#txtSearchOtherPayment', function() {
                    var value = $.trim($(this).val());
                    $("#listOTherPayments li").show().filter(function() {
                        return $(this).text().toLowerCase().trim().indexOf(value) == -1;
                    }).hide();
                })
        });

        function GetSystemBehavior() {
            Init();
            AjaxSend({
                url: 'POSSettings/GetSystemBehavior',
                success: function (vData) {

                    if (vData.Error == undefined) {

                        $("#selEnableDiningMode").prop('checked', CInt2(vData.IsEnableDiningMode));//.slider("refresh");
                        $("#selEnableBillingOption").prop('checked', CInt2(vData.IsEnableBillingOption));//.slider("refresh");
                        $("#selWhenPrintOrderSlip").prop('checked', vData.WhenDoYouWantToPrintOrderSlip || 'Not Applicable');//.selectmenu("refresh");
                        $("#selPrintOrderSlip").prop('checked', CInt2(vData.IsPrintOrderSlip));//.slider("refresh");
                        $("#selEnableVATExemptOption").prop('checked', CInt2(vData.IsEnableVATExemptOption));//.slider("refresh");
                        $("#selEnforceZRead").prop('checked', CInt2(vData.EnforceZRead));//.slider("refresh");
                        $('#selOnly1SalesRep').prop('checked', CInt2(vData.Only1SalesRep));//.slider("refresh");
                        $('#selRequireSalesRep').prop('checked', CInt2(vData.RequireSalesRep));//.slider("refresh");
                        $('#selDontAllowGreaterFreeTypePrices').prop('checked', CInt2(vData.DontAllowGreaterFreeTypePrices));//.slider("refresh");
                        $('#selAlwaysTestFreeTypeAPrice').prop('checked', CInt2(vData.AlwaysTestFreeTypeAPrice));//.slider("refresh");
                        $('#selSearchOnlyForBarcode').prop('checked', CInt2(vData.SearchOnlyForBarcode));//.slider("refresh");
                        $('#selShowCondensedProductInformationInSearchResult').prop('checked', CInt2(vData.ShowCondensedProductInformationInSearchResult));//.slider("refresh");
                        $('#selShowCondensedProductInformationInCart').prop('checked', CInt2(vData.ShowCondensedProductInformationInCart));//.slider("refresh");
                        $('#selShowCondensedProductInformationInItemDetailsPage').prop('checked', CInt2(vData.ShowCondensedProductInformationInItemDetailsPage));//.slider("refresh");
                        if (!IsNumeric(vData.VatPercent))
                            vData.VatPercent = 12;
                        $('#txtVatPercent').val(vData.VatPercent);
                        $('#selVatable').prop('checked', CInt2(vData.Vatable));//.slider("refresh");

                        $('#selHideBarcodeScan').prop('checked', CInt2(vData.HideBarcodeScan));//.slider("refresh");
                        $('#selHideHotButtonsButton').prop('checked', CInt2(vData.HideHotButtonsButton));//.slider("refresh");
                        $('#selHideSearchWithinResults').prop('checked', CInt2(vData.HideSearchWithinResults));//.slider("refresh");
                        $('#selPrintDispatchSlip').prop('checked', CInt2(vData.PrintDispatchSlip));//.slider("refresh");

                        $('[name="CanTransactWithUnflushedData"][value="' + CInt2(CBool(vData.CanTransactWithUnflushedData)) + '"]').prop('checked', 1);//.slider("refresh");
                        $('#selAutoServiceCharge').prop('checked', CInt2(vData.IsAutoServiceCharge));//.slider("refresh");
                        $('#txtServiceCharge').val(CDec2(vData.ServiceCharge));

                        $('#selPictureOfIDDurationSCPWDDiscount').prop('checked', CInt2(vData.PictureOfIDDurationSCPWDDiscount || '0'));//.slider("refresh");
                        $('#setAllowOfflineSrvPayments').prop('checked', CInt2(vData.AllowOfflineSrvPayments || '0'));//.slider("refresh");
                        $('#txtMaxMobizAppDatabaseAge').val(vData.MaxMobizAppDatabaseAge);

                        if (CInt2(vData.IsAutoServiceCharge) == 1) {
                            $('#divServiceChargeField').show();
                        } else {
                            $('#divServiceChargeField').hide();
                        }
                        $('[name="radServiceChargeBasis"]').removeAttr('checked');//.checkboxradio('refresh')
                        $('[name="radServiceChargeBasis"][value="' + vData.ServiceChargeBasis + '"]').prop('checked', true);//.checkboxradio('refresh')

                        $('[name="radFMCGOption"]').removeAttr('checked');//.checkboxradio('refresh')
                        $('[name="radFMCGOption"][value="' + vData.IsLineItemDisplay + '"]').prop('checked', true);//.checkboxradio('refresh')

                        $('[name="radFMCGPrint"]').removeAttr('checked');//.checkboxradio('refresh')
                        $('[name="radFMCGPrint"][value="' + vData.RealTimePrintReceipt + '"]').prop('checked', true);//.checkboxradio('refresh')

                        $('#selSetAllItemsEligibleForSCPWD').prop('checked', CInt2(vData.AllItemsEligibleForSCPWDDiscount));//.slider("refresh");

                        $('[name="radRoundUpPrices"]').removeAttr('checked');//.checkboxradio('refresh')
                        $('[name="radRoundUpPrices"][value="' + vData.RoundUpSellingPrices + '"]').prop('checked', true);//.checkboxradio('refresh')
                        $('.RoundUpSellingPricesCentavo').val(vData.RoundUpSellingPricesCentavo);

                        $('#selEnablePaymentCheck').prop('checked', CInt2(vData.ShowPaymentCheck));
                        $('#selEnablePaymentDebitCard').prop('checked', CInt2(vData.ShowPaymentDebit));
                        $('#selEnablePaymentCreditCard').prop('checked', CInt2(vData.ShowPaymentCredit));
                        $('#selEnablePaymentSRV').prop('checked', CInt2(vData.ShowPaymentSRV));
                        $('#selEnableOtherPayment').prop('checked', CInt2(vData.ShowOtherPayment));
                        $('#selEnablePaymentOnCredit').prop('checked', CInt2(vData.ShowPaymentOnCredit));
                        $('#selEnablePaymentBillingInfo').prop('checked', CInt2(vData.ShowPaymentBillingInfo));
                        $('#selEnablePaymentDeliveryInfo').prop('checked', CInt2(vData.ShowPaymentDeliveryInfo));
                        $('#selEnablePaymentDiscounts').prop('checked', CInt2(vData.ShowPaymentDiscounts));
                        $('#selEnablePaymentCharges').prop('checked', CInt2(vData.ShowPaymentCharges));
                        $('#selEnablePaymentRewards').prop('checked', CInt2(vData.ShowPaymentRewards));
                        $('#selEnablePaymentSalesRepresentative').prop('checked', CInt2(vData.ShowPaymentSalesReprensentative));
                        $('#selEnablePaymentDeliveryPayment').prop('checked', CInt2(vData.ShowDeliveryPayment));

                        $('#txtUnflushedReminderMinutes').val(vData.UnflushedReminderMinutes);
                        $('#txtUnflushedReminderMessage').val(vData.UnflushedReminderMessage);
                        $('#txtUnflushedCountSendingInterval').val(vData.UnflushedCountSendingInterval);

                        if (vData.IsShowTableNo) {

                            IsShowTableNo = 1;

                            $('#selCustomerTableNo').prop('checked', '1');
                            $('#divCustomerTableNumCount').show();

                        }
                        else {

                            IsShowTableNo = 0;

                            $('#selCustomerTableNo').removeAttr('checked');
                            $('#divCustomerTableNumCount').hide();

                        }

                        if (CStr2(vData.NoOfTables) != '')
                            $('#txtTableNoLimit').val(vData.NoOfTables);

                        if (vData.IsShowGasPumpNo) {

                            IsShowGasPumpNo = 1;

                            $('#selGasPumpNo').prop('checked', '1');
                            $('#divGasPumpCount').show();

                        }
                        else {

                            IsShowGasPumpNo = 0;

                            $('#selGasPumpNo').removeAttr('checked');
                            $('#divGasPumpCount').hide();

                        }

                        if (CStr2(vData.NoOfGasPumps) != '')
                            $('#txtGasPumpNoLimit').val(vData.NoOfGasPumps);

                        try {

                            $('#selCustomerTableNo');//.slider('refresh');
                            $('#selGasPumpNo');//.slider('refresh');
                        }
                        catch (e) {
                            $('#selCustomerTableNo');//.slider();
                            $('#selGasPumpNo');//.slider();
                        }
                        DisplayOrderSlipFowst(vData);
                    } else {
                        NewMessageBox('Saving has been disabled due to error during loading. Please reload page. ' + vData.Error);
                        mErrorOnLoad = true;
                    }

                    AddOtherPaymentsToLibrary();
                }
            });
        }

        function AddOtherPaymentsToLibrary() {
            AjaxSend({
                url: '/POSSettings/AddOtherPaymentsToLibrary',
                success: function(e){
                    if (e.Results == 'saved'){
                        GetOtherPayments();
                    } else {
                        console.log(e.Results)
                    }
                }
            })
        }

        function GetOtherPayments() {
            AjaxSend({
                url: '/POSSettings/GetOtherPayments',
                success: function(e){
                    var vRows = e.Results.Rows;
                    var listOTherPayments = $('#listOTherPayments');

                    listOTherPayments.empty();

                    for (var i = 0; i < vRows.length; i++){
                        listOTherPayments.append($('<li class="list-group-item">').text((i + 1) + '. ' + vRows[i].Description)
                            .data('data', vRows[i])
                            .click(function () {
                                var data = $(this).data('data');
                                OtherPaymentID = data.ID;
                                $('#txtNewOtherPayment').val(data.Description);                                
                                $('#selOPRefNumDataType').val((CStr2(data.DataType) == '' ? 'alphanumeric' : data.DataType));                                
                                $('#selRequiredOPRefNumber').prop('checked', CInt2(data.StrictReferenceCharLength));

                                if (CBool(data.StrictReferenceCharLength)) {
                                    $('#txtOPRefNumCharLength').val(data.PaymentReferenceLength);
                                    $('#divOPRefNumCharLength').show();
                                } else {
                                    $('#txtOPRefNumCharLength').val('');
                                    $('#divOPRefNumCharLength').hide();
                                }

                                $('#popupNewOtherPayment').modal('show');
                            })
                        )
                    }
                }
            })
        }

        function DisplayOrderSlipFowst(vData) {
            var pRows = GetStringObject('OrderSlipFowsts.Rows', vData);;
            if (!$.isArray(pRows)) 
                throw new Error('OrderSlipFowsts.Rows must be an array. ref: jsYDZRH47KWS!');

            var pContainer = $('#divOrderSlipFowsts');
            var pContents = pContainer.children();
            pContents.hide();

            for (var p = 0; p <= pRows.length - 1; p++) {
                var pDR = pRows[p];
                var pClone = pContents.clone();
                var pCheckBoxID = 'chkOrderSlip' + pDR.ID;
                var pCheckBox = pClone.find('input');
                pCheckBox.attr('id', pCheckBoxID);
                pCheckBox.prop('checked', CBool(pDR.ConsolidateOrderSlipPrinting));
                pCheckBox.data('data', pDR);
                var pLabel = pClone.find('label');
                pLabel.text(pDR.Name);
                pLabel.attr('for', pCheckBoxID);
                pContainer.append($('<div class="form-check mb-3" style="font-size:20px;">')
                        .append(pCheckBox).append(pLabel)
                    )
                //pContainer.append(pLabel);
                //pContainer.append(pCheckBox);
                //pClone.show();
            }

            pContainer.find('[type="checkbox"]');//.checkboxradio();
        }

        function validateNumber(number) {
            var re = /^[0-9]*$/;

            return re.test(number);
        }

        function SaveSystemBehavior() {

            var data = {
                IsEnableDiningMode: $.trim($("#selEnableDiningMode").is(':checked')),
                IsEnableBillingOption: $.trim($("#selEnableBillingOption").is(':checked')),
                WhenDoYouWantToPrintOrderSlip: $.trim($("#selWhenPrintOrderSlip").is(':checked')),
                IsPrintOrderSlip: $.trim($("#selPrintOrderSlip").is(':checked')),
                IsShowTableNo: IsShowTableNo,
                IsShowGasPumpNo: IsShowGasPumpNo,
                NoOfTables: $.trim($('#txtTableNoLimit').val()),
                NoOfGasPumps: $.trim($('#txtGasPumpNoLimit').val()),
                IsEnableVATExemptOption: $.trim($("#selEnableVATExemptOption").is(':checked')),
                EnforceZRead: $.trim($("#selEnforceZRead").is(':checked')),
                Only1SalesRep: $.trim($("#selOnly1SalesRep").is(':checked')),
                RequireSalesRep: $.trim($("#selRequireSalesRep").is(':checked')),
                DontAllowGreaterFreeTypePrices: $.trim($("#selDontAllowGreaterFreeTypePrices").is(':checked')),
                AlwaysTestFreeTypeAPrice: $.trim($("#selAlwaysTestFreeTypeAPrice").is(':checked')),
                SearchOnlyForBarcode: $.trim($("#selSearchOnlyForBarcode").is(':checked')),
                ShowCondensedProductInformationInSearchResult: $.trim($("#selShowCondensedProductInformationInSearchResult").is(':checked')),
                ShowCondensedProductInformationInCart: $.trim($("#selShowCondensedProductInformationInCart").is(':checked')),
                ShowCondensedProductInformationInItemDetailsPage: $.trim($("#selShowCondensedProductInformationInItemDetailsPage").is(':checked')),
                VatPercent: $.trim($("#txtVatPercent").val()),
                Vatable: $.trim($("#selVatable").is(':checked')),
                PictureOfIDDurationSCPWDDiscount: $.trim($("#selPictureOfIDDurationSCPWDDiscount").is(':checked')),
                AllowOfflineSrvPayments: $.trim($("#setAllowOfflineSrvPayments").is(':checked')),
                MaxMobizAppDatabaseAge: CInt2($("#txtMaxMobizAppDatabaseAge").val()),

                HideBarcodeScan: $.trim($("#selHideBarcodeScan").is(':checked')),
                HideHotButtonsButton: $.trim($("#selHideHotButtonsButton").is(':checked')),
                HideSearchWithinResults: $.trim($("#selHideSearchWithinResults").is(':checked')),
                PrintDispatchSlip: $.trim($("#selPrintDispatchSlip").is(':checked')),

                IsLineItemDisplay: ($('[name="radFMCGOption"]:checked').val() || 1),
                RealTimePrintReceipt: 0, // By default print receipt after payment ($('[name="radFMCGPrint"]:checked').val() || 1),

                AllItemsEligibleForSCPWDDiscount: $.trim($("#selSetAllItemsEligibleForSCPWD").is(':checked')),
                ConsolidateOrderSlipPrintingFowstIDs: GetCheckedFowsts(),

                IsAutoServiceCharge: IsAutoServiceCharge,
                ServiceCharge: CDec2($('#txtServiceCharge').val()),
                ServiceChargeBasis: ($('[name="radServiceChargeBasis"]:checked').val() || ''),
                CanTransactWithUnflushedData: CBool($('[name="CanTransactWithUnflushedData"]:checked').val()),
                RoundUpSellingPrices: ($('[name="radRoundUpPrices"]:checked').val() || ''),

                ShowPaymentCheck: $.trim($("#selEnablePaymentCheck").is(':checked')),
                ShowPaymentDebit: $.trim($("#selEnablePaymentDebitCard").is(':checked')),
                ShowPaymentCredit: $.trim($("#selEnablePaymentCreditCard").is(':checked')),
                ShowPaymentSRV: $.trim($("#selEnablePaymentSRV").is(':checked')),
                ShowOtherPayment: $.trim($("#selEnableOtherPayment").is(':checked')),
                ShowPaymentOnCredit: $.trim($("#selEnablePaymentOnCredit").is(':checked')),
                ShowPaymentBillingInfo: $.trim($("#selEnablePaymentBillingInfo").is(':checked')),
                ShowPaymentDeliveryInfo: $.trim($("#selEnablePaymentDeliveryInfo").is(':checked')),
                ShowPaymentDiscounts: $.trim($("#selEnablePaymentDiscounts").is(':checked')),
                ShowPaymentCharges: $.trim($("#selEnablePaymentCharges").is(':checked')),
                ShowPaymentRewards: $.trim($("#selEnablePaymentRewards").is(':checked')),
                ShowPaymentSalesReprensentative: $.trim($("#selEnablePaymentSalesRepresentative").is(':checked')),
                ShowDeliveryPayment: $.trim($("#selEnablePaymentDeliveryPayment").is(':checked')),
                UnflushedReminderMinutes: CInt2($("#txtUnflushedReminderMinutes").val()),
                UnflushedReminderMessage: $.trim($("#txtUnflushedReminderMessage").val()),
                UnflushedCountSendingInterval: $.trim($("#txtUnflushedCountSendingInterval").val()),
            };

            var pRoundUpSellingPrices = $('[name="radRoundUpPrices"]:checked').val();
            data.RoundUpSellingPrices = pRoundUpSellingPrices;
            if (CStr2(pRoundUpSellingPrices).trim() != '')
                data.RoundUpSellingPricesCentavo = $('.RoundUpSellingPricesCentavo.' + pRoundUpSellingPrices).val();
                
            AjaxSend({
                url: 'POSSettings/SaveSystemBehavior',
                data: data,
                success: function (vData) {
                    if (vData.Result == "Success") {
                        //history.back();
                        WebToast('Successfully saved', 'top');
                        window.location.hash = 'SystemAdministration';
                    }
                    else {
                        NewMessageBox("An error occured while saving POS settings.", "Decimal Settings");
                    }
                }
            });
        }

        function GetCheckedFowsts() {
            var pOut = [];
            var pContainer = $('#divOrderSlipFowsts');
            var pCheckBoxes = pContainer.find('input:checked');
            for (var p = 0; p <= pCheckBoxes.length - 1; p++) {
                var pDR = $(pCheckBoxes[p]).data('data');
                pOut.push(pDR.ID);
            }
            return pOut;
        }
    
        function Init() {
            var pSpan = $('.spanRoundUpSellingPricesCentavo.once');
            if (pSpan.length == 0)
                return;

            var pCB = $('<select class="RoundUpSellingPricesCentavo once" style="width:50px"  />');
            pSpan.after(pCB)

            pCB = $('<select class="RoundUpSellingPricesCentavo always" style="width:50px"  />');
            $('.spanRoundUpSellingPricesCentavo.always').after(pCB)

            pCB = $('.RoundUpSellingPricesCentavo');
            for (var pItem of [0, 25, 50, 75]) {
                var pOption = new Option(pItem, pItem);
                pCB.append(pOption);
            }
        };
    });