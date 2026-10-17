require([
    'common'
    ], function () {
        //CREATED BY Janine Aug 24, 2013 1pm
        $(function () {
            var origSCDiscount;
            var origPWDDiscount;
            var origSPDiscount;
            var origEmpDiscount;
            var origRegDisc1;
            var origRegDisc2;
            var origRegDisc3;
            var origVIPDisc;
            var origGPCDisc;
            var origPNSTMDisc;
            var origMaxAllowableDiscountableAmount;
            var arrCD = [], arrCDFowst = [], cdindex = '';
            
            window.DiscountSetting = {
                GetDiscountSettings: function () {
                    getDiscountSettings();
                    getMaxAllowableDiscountableAmount();
                    getCustomDiscounts();
                }
            }

            DiscountSetting.GetDiscountSettings();

            //restrict to numeric, 2 decimal points

            $('body').on('keydown', '#txtSeniorCitizenDiscount', function (e) {
                if (e.keyCode != 110 && e.keyCode != 190) {
                    if (!NumberOnly(e)) return;
                }
            })
            .on('blur', '#txtSeniorCitizenDiscount', function (e) {
                var pSCDiscount = parseFloat($(this).val().replace(/,/g, ""));
                $(this).val((pSCDiscount > 0 ? pSCDiscount.toFixed(2) : ""));
            })
            .on('keydown', '#txtPersonWithDisabilityDiscount', function (e) {
                if (e.keyCode != 110 && e.keyCode != 190) {
                    if (!NumberOnly(e)) return;
                }
            })
            .on('blur', '#txtPersonWithDisabilityDiscount', function (e) {
                var pPWDDiscount = parseFloat($(this).val().replace(/,/g, ""));
                $(this).val((pPWDDiscount > 0 ? pPWDDiscount.toFixed(2) : ""));
            })
            .on('blur', '#txtSoloParentDiscount', function (e) {
                var disc = parseFloat($(this).val().replace(/,/g, ""));
                $(this).val((disc > 0 ? disc.toFixed(2) : ""));
            })
            .on('keydown', '#txtEmployeeDiscount', function (e) {
                if (e.keyCode != 110 && e.keyCode != 190) {
                    if (!NumberOnly(e)) return;
                }
            })
            .on('blur', '#txtEmployeeDiscount', function (e) {
                var pEmployeeDiscount = parseFloat($(this).val().replace(/,/g, ""));
                $(this).val((pEmployeeDiscount > 0 ? pEmployeeDiscount.toFixed(2) : ""));
            })
            .on('keydown', '#txtRegularDiscount1', function (e) {
                if (e.keyCode != 110 && e.keyCode != 190) {
                    if (!NumberOnly(e)) return;
                }
            })
            .on('blur', '#txtRegularDiscount1', function (e) {
                var pRegularDiscount1 = parseFloat($(this).val().replace(/,/g, ""));
                $(this).val((pRegularDiscount1 > 0 ? pRegularDiscount1.toFixed(2) : ""));
            })
            .on('keydown', '#txtRegularDiscount2', function (e) {
                if (e.keyCode != 110 && e.keyCode != 190) {
                    if (!NumberOnly(e)) return;
                }
            })
            .on('blur', '#txtRegularDiscount2', function (e) {
                var pRegularDiscount2 = parseFloat($(this).val().replace(/,/g, ""));
                $(this).val((pRegularDiscount2 > 0 ? pRegularDiscount2.toFixed(2) : ""));
            })
            .on('keydown', '#txtRegularDiscount3', function (e) {
                if (e.keyCode != 110 && e.keyCode != 190) {
                    if (!NumberOnly(e)) return;
                }
            })
            .on('blur', '#txtRegularDiscount3', function (e) {
                var pRegularDiscount3 = parseFloat($(this).val().replace(/,/g, ""));
                $(this).val((pRegularDiscount3 > 0 ? pRegularDiscount3.toFixed(2) : ""));
            })
            .on('keydown', '#txtVIPDiscount', function (e) {
                if (e.keyCode != 110 && e.keyCode != 190) {
                    if (!NumberOnly(e)) return;
                }
            })
            .on('blur', '#txtVIPDiscount', function (e) {
                var pVIPDiscount = parseFloat($(this).val().replace(/,/g, ""));
                $(this).val((pVIPDiscount > 0 ? pVIPDiscount.toFixed(2) : ""));
            })
            .on('keydown', '#txtGPCDiscount, #txtPNSTMDiscount', function (e) {
                if (e.keyCode != 110 && e.keyCode != 190) {
                    if (!NumberOnly(e)) return;
                }
            })
            .on('blur', '#txtGPCDiscount', function (e) {
                var pGPCDiscount = parseFloat($(this).val().replace(/,/g, ""));
                $(this).val((pGPCDiscount > 0 ? pGPCDiscount.toFixed(2) : ""));
            })
            .on('blur', '#txtPNSTMDiscount', function (e) {
                var PNSTMDiscount = parseFloat($(this).val().replace(/,/g, ""));
                $(this).val((PNSTMDiscount > 0 ? PNSTMDiscount.toFixed(2) : ""));
            })
            .on('click', '#butSave', function (e) {
                e.preventDefault();

                SaveDiscountSettings();
            })
            .on('click', '#btnDiscountSettingsBack', function (e) {
                e.preventDefault();

                //history.back();
                //window.open(GetPathFormat("/People/PeopleMenu"), "_self");
                window.location.hash = 'SystemAdministration';
            })
            .on('click', '#btnAddCD', function () {
                ClearCDFields();
                $('#panCustomDiscount').modal('show');
            })
            .on('change', '[name="radCD"]', function () {
                if ($(this).val() == 'fixmaount') {
                    $('#txtCDValue').attr('placeholder', 'Ex: 123.00');
                } else {
                    $('#txtCDValue').attr('placeholder', 'Ex: 10%');
                }
            })
            .on('change', '#chbCDIsApplyToAllFOWST', function () {
                if ($(this).is(':checked')) {
                    $('#divCDFowst').hide();
                } else {
                    $('#divCDFowst').show();
                }
            })
            .on('change', '#chbCDAvailability', function () {
                if ($(this).is(':checked')) {
                    $('#divCDAvailability').show();
                } else {
                    $('#txtCDAvailabilityFrom, #txtCDAvailabilityTo').val('');
                    $('#divCDAvailability').hide();
                }
            })
            .on('click', '#btnCDAddFowst', function () {
                BrowsePopup('LibraryFowst', 'Select FOWST Locations', [
                    {
                        AutoSearch: true,
                        SelectMultiple: true,
                        Message: 'You can select multiple FOWSTs. Click OK when you\'re done.',
                        DisplayText: [{ Column: 'FowstName' }],
                        Click: function (e) {
                            for (var i = 0; i < e.length; i++) {
                                var isexist = false;

                                $.grep(arrCDFowst, function (d, ind) {
                                    if (d.ID == e[i].ID) {
                                        isexist = true;
                                    }
                                })

                                if (!isexist) {
                                    arrCDFowst.push(e[i]);
                                }
                            }

                            DisplayCDFowst();
                        }, Close: function () {
                            $('#panCustomDiscount').modal('show');
                        }
                    }
                ])
            })
            .on('click', '#btnCustomDiscountOkay', function () {
                var txtCDName = $.trim($('#txtCDName').val());
                var txtCDDescription = $.trim($('#txtCDDescription').val());
                var txtCDMinPurchaseAmt = $.trim($('#txtCDMinPurchaseAmt').val());
                var txtCDValue = $.trim($('#txtCDValue').val());
                var txtCDAvailabilityFrom = $.trim($('#txtCDAvailabilityFrom').val());
                var txtCDAvailabilityTo = $.trim($('#txtCDAvailabilityTo').val());

                if (txtCDName == '') {
                    WebToast('Please enter discount name', 'top');
                    return;
                }

                if (txtCDMinPurchaseAmt == '') {
                    WebToast('Please enter minimum purchase amount', 'top');
                    return;
                }

                if (txtCDValue == '') {
                    WebToast('Please enter discount value', 'top');
                    return;
                }

                if (!$('#chbCDIsApplyToAllFOWST').is(':checked') && arrCDFowst.length == 0) {
                    WebToast('Please add at least one FOWST', 'top');
                    return;
                }

                if ($('#chbCDAvailability').is(':checked')) {
                    if (txtCDAvailabilityFrom == '') {
                        WebToast('Please set date "From"', 'top');
                        return;
                    }

                    if (txtCDAvailabilityTo == '') {
                        WebToast('Please set date "To"', 'top');
                        return;
                    }
                }

                if (CStr2(cdindex) == '') {
                    arrCD.push({
                        ID: '',
                        Name: txtCDName,
                        Description: txtCDDescription,
                        MinPurchaseAmount: txtCDMinPurchaseAmt,
                        Type: $('[name="radCD"]:checked').val(),
                        DiscountValue: txtCDValue,
                        IsActive: $('#chbCDIsActive').is(':checked'),
                        IsApplyToAllFowst: $('#chbCDIsApplyToAllFOWST').is(':checked'),
                        FowstList: arrCDFowst,
                        AvailabilityDateFrom: txtCDAvailabilityFrom,
                        AvailabilityDateTo: txtCDAvailabilityTo,
                        State: 'new'
                    })
                } else {
                    arrCD[cdindex].Name = txtCDName;
                    arrCD[cdindex].Description = txtCDDescription;
                    arrCD[cdindex].MinPurchaseAmount = txtCDMinPurchaseAmt;
                    arrCD[cdindex].Type = $('[name="radCD"]:checked').val();
                    arrCD[cdindex].DiscountValue = txtCDValue;
                    arrCD[cdindex].IsActive = $('#chbCDIsActive').is(':checked');
                    arrCD[cdindex].IsApplyToAllFowst = $('#chbCDIsApplyToAllFOWST').is(':checked');
                    arrCD[cdindex].FowstList = arrCDFowst;
                    arrCD[cdindex].AvailabilityDateFrom = txtCDAvailabilityFrom;
                    arrCD[cdindex].AvailabilityDateTo = txtCDAvailabilityTo;

                    if (arrCD[cdindex].State == 'edit') {
                        arrCD[cdindex].State = 'edited';
                    }
                }
                
                DisplayCD();
                $('#panCustomDiscount').modal('hide');
            })

            function getCustomDiscounts() {
                AjaxSend({
                    url: '/DiscountSettings/GetCustomDiscounts',
                    success: function (e) {
                        arrCD = e.Details
                        DisplayCD();
                    }
                })
            }

            function DisplayCD() {
                var tbodyCustomDiscount = $('#tbodyCustomDiscount');

                tbodyCustomDiscount.empty();

                if (arrCD.length > 0) {
                    for (var i = 0; i < arrCD.length; i++) {
                        arrCD[i].Index = i;

                        if (arrCD[i].State != 'remove') {
                            tbodyCustomDiscount.append($('<tr>')
                                .append($('<td>')
                                    .text((i + 1) + '.')
                                )
                                .append($('<td>')
                                    .text(arrCD[i].Name)
                                )
                                .append($('<td>')
                                    .text(arrCD[i].Description)
                                )
                                .append($('<td>')
                                    .text(AccountingFormat(arrCD[i].MinPurchaseAmount))
                                )
                                .append($('<td>')
                                    .text((arrCD[i].Type == 'fixamount' ? AccountingFormat(arrCD[i].DiscountValue) : arrCD[i].DiscountValue + '%'))
                                )
                                .append($('<td>')
                                    .text((CBool(arrCD[i].IsActive) ? 'Active' : 'Inactive'))
                                )
                                .append($('<td>')
                                    .html((arrCD[i].State == 'new' ? '' : $.format.date(new Date(CJsonDate(arrCD[i].LastUpdated)), 'MM-dd-yyyy hh:mm a') + '<br> by ' + arrCD[i].UserName))
                                )
                                .append($('<td style="width:50px; text-align:center;">')
                                    .append($('<button data-role="none" class="btn btn-danger">')
                                        .append($('<span class="bi-trash">'))
                                    )
                                    .data('data', arrCD[i])
                                    .click(function (e) {
                                        e.preventDefault();
                                        e.stopPropagation();
                                        var data = $(this).data('data');

                                        NewMessageBox('Are you sure you want to remove <b>' + data.Name + '</b>?', 'Confirmation Dialog', [
                                            {
                                                Text: 'Yes', Click: function () {
                                                    if (data.ID == '') {
                                                        arrCD.splice(data.Index, 1);
                                                    } else {
                                                        arrCD[data.Index].State = 'remove';
                                                    }

                                                    DisplayCD();
                                                }
                                            }, {
                                                Text: 'No', Click: function () {

                                                }
                                            }
                                        ])
                                    })
                                )
                                .data('data', arrCD[i])
                                .click(function () {
                                    var data = $(this).data('data');
                                    ClearCDFields();

                                    cdindex = data.Index;
                                    arrCDFowst = data.FowstList;
                                    $('#txtCDName').val(data.Name);
                                    $('#txtCDDescription').val(data.Description);
                                    $('#txtCDMinPurchaseAmt').val(data.MinPurchaseAmount);
                                    $('#txtCDValue').val(data.DiscountValue);
                                    $('[name="radCD"][value="' + data.Type + '"]').prop('checked', 1);
                                    $('#chbCDIsActive').prop('checked', CInt2(data.IsActive));
                                    $('#chbCDIsApplyToAllFOWST').prop('checked', CInt2(data.IsApplyToAllFowst));
                                    $('#txtCDAvailabilityFrom').val((CStr2(data.AvailabilityDateFrom) == '' ? '' : $.format.date(new Date(CJsonDate(data.AvailabilityDateFrom)), 'yyyy-MM-dd')));
                                    $('#txtCDAvailabilityTo').val((CStr2(data.AvailabilityDateTo) == '' ? '' : $.format.date(new Date(CJsonDate(data.AvailabilityDateTo)), 'yyyy-MM-dd')));
                                    $('#chbCDAvailability').prop('checked', (CStr2(data.AvailabilityDateFrom) == '' ? 0 : 1));
                                    DisplayCDFowst();

                                    if (!CBool(data.IsApplyToAllFowst)) {
                                        $('#divCDFowst').show();
                                    }

                                    if (CStr2(data.AvailabilityDateFrom) != '') {
                                        $('#divCDAvailability').show();
                                    }

                                    $('#panCustomDiscount').modal('show');
                                })
                            )
                        }
                    }
                } else {
                    tbodyCustomDiscount.append($('<tr>')
                                .append($('<td class="text-center" colspan="8">')
                                    .text('List is empty')
                                )
                            )
                }
            }

            function DisplayCDFowst() {
                var listCDFowsts = $('#listCDFowsts');
                listCDFowsts.empty();

                if (arrCDFowst.length > 0) {
                    for (var i = 0; i < arrCDFowst.length; i++) {
                        arrCDFowst[i].Index = i;
                        listCDFowsts.append($('<li class="list-group-item">')
                                .append($('<table class="w-100">')
                                    .append($('<tr>')
                                        .append($('<td>').text(arrCDFowst[i].FowstName))
                                        .append($('<td style="width:50px; text-align:center;">')
                                            .append($('<button data-role="none" class="btn btn-danger">')
                                                .append($('<span class="bi-trash">'))
                                                .data('data', arrCDFowst[i])
                                                .click(function () {
                                                    var data = $(this).data('data');
                                                    arrCDFowst.splice(data.Index, 1);
                                                    DisplayCDFowst();
                                                })
                                            )
                                        )
                                    )
                                )
                            )
                    }
                } else {
                    listCDFowsts.append($('<li class="list-group-item">').text('List is empty'));
                }
            }

            function ClearCDFields() {
                cdindex = '';
                arrCDFowst = [];
                $('#txtCDName, #txtCDDescription, #txtCDMinPurchaseAmt, #txtCDValue, #txtCDAvailabilityFrom, #txtCDAvailabilityTo').val('');
                $('#radCDFixAmount, #chbCDIsActive, #chbCDIsApplyToAllFOWST').prop('checked', 1);
                $('#chbCDAvailability').prop('checked', 0);
                $('#txtCDValue').attr('placeholder', 'Ex: 123.00');
                $('#divCDFowst, #divCDAvailability').hide();
                $('#listCDFowsts').empty();
                DisplayCDFowst();
            }

            function getDiscountSettings() {
                AjaxSend({
                    url: 'DiscountSettings/getDiscountSettings',
                    success: function (vData) {
                        var vRows = vData.DiscountData.Rows;
                        for (var p = 0; p < vRows.length; p++) {
                            if (vRows[p].DiscountType == 'Senior Citizen Discount') {
                                origSCDiscount = vRows[p].PercentDiscount;
                                $("#txtSeniorCitizenDiscount").val(vRows[p].PercentDiscount);
                                continue;
                            }
                            if (vRows[p].DiscountType == 'Person With Disability Discount') {
                                origPWDDiscount = vRows[p].PercentDiscount;
                                $("#txtPersonWithDisabilityDiscount").val(vRows[p].PercentDiscount);

                                continue;
                            }
                            if (vRows[p].DiscountType == 'Solo Parent Discount') {
                                origSPDiscount = vRows[p].PercentDiscount;
                                $("#txtSoloParentDiscount").val(vRows[p].PercentDiscount);

                                continue;
                            }
                            if (vRows[p].DiscountType == 'Employee Discount') {
                                origEmpDiscount = vRows[p].PercentDiscount;
                                $("#txtEmployeeDiscount").val(vRows[p].PercentDiscount);
                                continue;
                            }
                            if (vRows[p].DiscountType == 'Regular Discount 1') {
                                origRegDisc1 = vRows[p].PercentDiscount;
                                $("#txtRegularDiscount1").val(vRows[p].PercentDiscount);
                                continue;
                            }
                            if (vRows[p].DiscountType == 'Regular Discount 2') {
                                origRegDisc2 = vRows[p].PercentDiscount;
                                $("#txtRegularDiscount2").val(vRows[p].PercentDiscount);
                                continue;
                            }
                            if (vRows[p].DiscountType == 'Regular Discount 3') {
                                origRegDisc3 = vRows[p].PercentDiscount;
                                $("#txtRegularDiscount3").val(vRows[p].PercentDiscount);
                                continue;
                            }
                            if (vRows[p].DiscountType == 'VIP Discount') {
                                origVIPDisc = vRows[p].PercentDiscount;
                                $("#txtVIPDiscount").val(vRows[p].PercentDiscount);
                                continue;
                            }

                            if (vRows[p].DiscountType == 'GPC Discount') {
                                origGPCDisc = vRows[p].PercentDiscount;
                                $("#txtGPCDiscount").val(vRows[p].PercentDiscount);
                                continue;
                            }

                            if (vRows[p].DiscountType == 'Philippine National Sports Team Discount') {
                                origPNSTMDisc = vRows[p].PercentDiscount;
                                $("#txtPNSTMDiscount").val(vRows[p].PercentDiscount);
                                continue;

                            }
                            if (vRows[p].DiscountType == 'Max Allowable Discountable Amount Subject for SCD') {
                                origMaxAllowableDiscountableAmount = vRows[p].PercentDiscount;
                                $("#txtMaxAllowableDiscountableAmount").val(vRows[p].PercentDiscount);
                                continue;
                            }
                        }

                        $('#chbHeadcount').prop('checked', CBool(vData.ShowHeadcount));
                        $('[name="radDiscountProfile"][value="' + (vData.AllowOneDiscountProfileOnly || 'No') + '"]').prop('checked', 1);
                    } // success
                }); // AjaxSend

            }

            function getMaxAllowableDiscountableAmount() {
                AjaxSend({
                    url: 'DiscountSettings/getMaxAllowableDiscountableAmount',
                    success: function (vData) {
                        displayMaxAllowableDiscountableAmount(vData.Results.Rows);
                    } // success
                }); // AjaxSend
            }

            var TradeNameID = '';

            function displayMaxAllowableDiscountableAmount(vRows) {
                var pLV = $("#tblMaxAllowableDiscountableAmount");
                pLV.empty();
                if (vRows.length > 0) {
                    for (i = 0; i <= vRows.length - 1; i++) {
                        pItem = $('<tr>')
                                .attr('ID', vRows[i].ID)
                                .data('data', vRows[i])
                                .css('cursor', 'pointer')
                                .append($('<td>')
                                    .text(vRows[i].BusinessTradeName)
                                )
                                .append($('<td id=Max' + vRows[i].ID + '>')
                                    .text(vRows[i].MaxAllowableDiscountableAmount)
                                )
                                .click(function () {
                                    TradeNameID = $(this).attr('ID');
                                    $('#panSetMaxAllowableDiscountableAmount').modal('show')
                                })

                        pLV.append(pItem);
                    }
                }
            }

            $('#btnSetMaxAllowableDiscountableAmount').click(function () {
                $('#Max' + TradeNameID).text($('#txtMaxAllowableDiscountableAmount').val());
                $('#panSetMaxAllowableDiscountableAmount').modal('hide');
            });

            function SaveDiscountSettings() {

                var pData = {};
                var MaxAllowableDiscountableAmountPerTradeName = [];

                if (origSCDiscount != $.trim($("#txtSeniorCitizenDiscount").val()))
                    pData.SeniorCitizenDiscount = $.trim($("#txtSeniorCitizenDiscount").val());
                else
                    pData.SeniorCitizenDiscount = 'no changes';

                if (origPWDDiscount != $.trim($("#txtPersonWithDisabilityDiscount").val()))
                    pData.PersonWithDisabilityDiscount = $.trim($("#txtPersonWithDisabilityDiscount").val());
                else
                    pData.PersonWithDisabilityDiscount = 'no changes';

                if (origSPDiscount != $.trim($("#txtSoloParentDiscount").val()))
                    pData.SoloParentDiscount = $.trim($("#txtSoloParentDiscount").val());
                else
                    pData.SoloParentDiscount = 'no changes';

                if (origEmpDiscount != $.trim($("#txtEmployeeDiscount").val()))
                    pData.EmployeeDiscount = $.trim($("#txtEmployeeDiscount").val());
                else
                    pData.EmployeeDiscount = 'no changes';

                if (origRegDisc1 != $.trim($("#txtRegularDiscount1").val()))
                    pData.RegularDiscount1 = $.trim($("#txtRegularDiscount1").val());
                else
                    pData.RegularDiscount1 = 'no changes';

                if (origRegDisc2 != $.trim($("#txtRegularDiscount2").val()))
                    pData.RegularDiscount2 = $.trim($("#txtRegularDiscount2").val());
                else
                    pData.RegularDiscount2 = 'no changes';

                if (origRegDisc3 != $.trim($("#txtRegularDiscount3").val()))
                    pData.RegularDiscount3 = $.trim($("#txtRegularDiscount3").val());
                else
                    pData.RegularDiscount3 = 'no changes';

                if (origVIPDisc != $.trim($("#txtVIPDiscount").val()))
                    pData.VIPDiscount = $.trim($("#txtVIPDiscount").val());
                else
                    pData.VIPDiscount = 'no changes';

                if (origGPCDisc != $.trim($("#txtGPCDiscount").val()))
                    pData.GPCDiscount = $.trim($("#txtGPCDiscount").val());
                else
                    pData.GPCDiscount = 'no changes';
                
                if (origPNSTMDisc != $.trim($("#txtPNSTMDiscount").val()))
                    pData.PNSTMDiscount = $.trim($("#txtPNSTMDiscount").val());
                else
                    pData.PNSTMDiscount = 'no changes';


                if (origMaxAllowableDiscountableAmount != $.trim($("#txtMaxAllowableDiscountableAmount").val()))
                    pData.MaxAllowableDiscountableAmount = $.trim($("#txtMaxAllowableDiscountableAmount").val());
                else
                    pData.MaxAllowableDiscountableAmount = 'no changes';


                $('#tblMaxAllowableDiscountableAmount tr').each(function () {
                    MaxAllowableDiscountableAmountPerTradeName.push({
                        ID: $(this).attr('id'),
                        MaxAllowableDiscountableAmount: ($('#Max' + $(this).attr('id')).text() || '')
                    })
                });

                pData.MaxAllowableDiscountableAmountPerTradeName = MaxAllowableDiscountableAmountPerTradeName;
                pData.CustomDiscounts = arrCD;
                pData.ShowHeadCount = $('#chbHeadcount').is(':checked');
                pData.AllowOneDiscountProfileOnly = $('[name="radDiscountProfile"]:checked').val();

                AjaxSend({
                    url: 'DiscountSettings/SaveDiscountSettings',
                    data: pData,
                    success: function (vData) {

                        if (vData.DiscountSettingsData == 'True') {
                            //history.back();
                            //                            MessageBox("Discount Settings successfully saved.",
                            //                                        "Discount Settings",
                            //                                        [
                            //                                            { Text: 'Ok',
                            //                                                Click: function () {
                            //                                                    location.reload(); // reload the page
                            //                                                }
                            //                                            }
                            //                                        ]
                            //                            );
                            //                            return;
                            window.location.hash = 'SystemAdministration';
                            WebToast('Successfully saved', 'top');
                        }


                    } // success
                }); // AjaxSend

            }

        }); //End Inner function

    });