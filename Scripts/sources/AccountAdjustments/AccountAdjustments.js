require(['common'],
function () {
    $(document).ready(function () {
        var accountType = '', adjustmentID = '';

        window.Adjustment = {
            Load: function () {
                LoadData();
            }
        }

        LoadData();

        $('body')
        .on('click', '#btnArAddAdjustmentType', function () {
            ClearFields();
            accountType = 'ar';
            $('#lblAdjustmentTitle').text('AR');
            $('#txtNewAdjustmentTitle').text('Account Receivable');
            $('#popupNewAdjustment').modal('show');
        })
        .on('click', '#btnApAddAdjustmentType', function () {
            ClearFields();
            accountType = 'ap';
            $('#lblAdjustmentTitle').text('AP');
            $('#txtNewAdjustmentTitle').text('Account Payable');
            $('#popupNewAdjustment').modal('show');
        })
        .on('change', '#selAdjustmentType', function () {
            var val = $(this).val();
            if(val != ""){
                if(accountType == 'ar'){
                    if (val == "Debit Memo") {
                        $('#txtAdjustmentEffect').text('Additive (+)');
                    } else {
                        $('#txtAdjustmentEffect').text('Deductive (-)');
                    }
                } else {
                    if (val == "Debit Memo") {
                        $('#txtAdjustmentEffect').text('Deductive (-)');
                    } else {
                        $('#txtAdjustmentEffect').text('Additive (+)');
                    }
                }
            } else {
                $('#txtAdjustmentEffect').text('');
            }
        })
        .on('click', '#btnSaveAdjustment', function () {
            SaveAdjustment();            
        })

        function SaveAdjustment() {
            var selAdjustmentType = $.trim($('#selAdjustmentType').val());
            var txtAdjustmentEffect = $.trim($('#txtAdjustmentEffect').text());
            var txtAdjustmentReason = $.trim($('#txtAdjustmentReason').val());
            var selAdjustmentStatus = $.trim(CInt2($('#selAdjustmentStatus').is(':checked')));
            var pData = {};

            if (selAdjustmentType == "") {
                WebToast('Please select adjustment type', 'top');
                return;
            }

            if (txtAdjustmentReason == "") {
                WebToast('Please enter a reason for adjustment', 'top');
                return;
            }

            $('#popupNewAdjustment').modal('hide');

            pData.ID = adjustmentID;
            pData.Type = selAdjustmentType;
            pData.Effect = txtAdjustmentEffect;
            pData.Reason = txtAdjustmentReason;
            pData.Status = selAdjustmentStatus;
            pData.AccountType = accountType;

            AjaxSend({
                url: 'AccountAdjustments/SaveAdjustment',
                data: pData,
                success: function (vData) {
                    if (vData.Results == 'saved') {
                        WebToast('Successfully Saved', 'top');
                        GetAdjustments();
                    } else {
                        NewMessageBox(vData.Results, 'Message');
                    }
                }
            })
        }

        function LoadData() {
            ClearFields();
            GetAdjustments();
        }

        function GetAdjustments() {
            AjaxSend({
                url: 'AccountAdjustments/GetAdjustments',
                success: function (vData) {
                    var tbodyAR = $('#tbodyAR');
                    var tbodyAP = $('#tbodyAP');
                    var vRowsAr = vData.Ar.Rows;
                    var vRowsAp = vData.Ap.Rows;

                    tbodyAR.empty();
                    tbodyAP.empty();

                    if (vRowsAr.length > 0) {
                        for (var i = 0; i < vRowsAr.length; i++){
                            tbodyAR.append($('<tr>')
                                    .data('data', vRowsAr[i])
                                    .append($('<td>')
                                        .text(vRowsAr[i].AdjustmentType)
                                    )
                                    .append($('<td>')
                                        .text(vRowsAr[i].Effect)
                                    )
                                    .append($('<td>')
                                        .text(vRowsAr[i].Reason)
                                    )
                                    .append($('<td>')
                                        .text(CBool(vRowsAr[i].IsActive) ? 'Active' : 'Inactive')
                                    )
                                    .click(function () {
                                        $('#btnArAddAdjustmentType').click();

                                        adjustmentID = $(this).data('data').ID;
                                        $('#selAdjustmentType').val($(this).data('data').AdjustmentType);//.selectmenu();//.selectmenu('refresh');
                                        $('#txtAdjustmentEffect').text($(this).data('data').Effect);
                                        $('#txtAdjustmentReason').val($(this).data('data').Reason);
                                        $('#selAdjustmentStatus').prop('checked', CInt2($(this).data('data').IsActive));//.slider();//.slider('refresh');
                                    })
                                )
                        }

                        tbodyAR.parent();//.table('refresh');
                    } else {
                        tbodyAR.append('<tr><td colspan="4" style="text-align:center;">No Results found</td></tr>');
                    }

                    if (vRowsAp.length > 0) {
                        for (var i = 0; i < vRowsAp.length; i++) {
                            tbodyAP.append($('<tr>')
                                    .data('data', vRowsAp[i])
                                    .append($('<td>')
                                        .text(vRowsAp[i].AdjustmentType)
                                    )
                                    .append($('<td>')
                                        .text(vRowsAp[i].Effect)
                                    )
                                    .append($('<td>')
                                        .text(vRowsAp[i].Reason)
                                    )
                                    .append($('<td>')
                                        .text(CBool(vRowsAp[i].IsActive) ? 'Active' : 'Inactive')
                                    )
                                    .click(function () {
                                        $('#btnApAddAdjustmentType').click();

                                        adjustmentID = $(this).data('data').ID;
                                        $('#selAdjustmentType').val($(this).data('data').AdjustmentType);//.selectmenu();//.selectmenu('refresh');
                                        $('#txtAdjustmentEffect').text($(this).data('data').Effect);
                                        $('#txtAdjustmentReason').val($(this).data('data').Reason);
                                        $('#selAdjustmentStatus').prop('checked', CInt2($(this).data('data').IsActive));//.slider();//.slider('refresh');
                                    })
                                )
                        }

                        tbodyAP.parent();//.table('refresh');
                    } else {
                        tbodyAP.append('<tr><td colspan="4" style="text-align:center;">No Results found</td></tr>');
                    }
                }
            })
        }

        function ClearFields() {
            adjustmentID = '';
            $('#selAdjustmentType').val('');//.selectmenu();//.selectmenu('refresh');
            $('#txtAdjustmentEffect').text('');
            $('#txtAdjustmentReason').val('');
            $('#selAdjustmentStatus').prop('checked', 1);//.slider();//.slider('refresh');
        }
    })
})