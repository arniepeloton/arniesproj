require([
        'common'
], function () {
    var SelectedZReadDataID = '';
    var adjustments = 0;

    var FOWSTID = '';
    var DeviceID = '';
    var ZData;

    $(function () {

        GetZReadDataFromServer();

        $('#btnDeleteData').click(function () {
            $('#divDeleteData').popup('open');
        });

        $('#btnCancelDelete').click(function () {
            $('#divDeleteData').popup('close');
        });

        $('#btnSendZReadDataToServer').click(function () {
            $('#divSendDataToServer').popup('open');
        })

        $('#btnSendData').click(function () {
            SendZReadData();
            $('#divSendDataToServer').popup('close');
        })

        $('#btnGetZReadDataFroServer').click(function () {
            GetZReadDataFromServer();
        });

        $('#btnAccumDone').click(function () {
            $('#txtAccumGross' + SelectedZReadDataID).text(AccountingFormat(CDec2($('#txtNewAccumulatedAmount').val())));
            $('#txtAccumAdjustment' + SelectedZReadDataID).text(AccountingFormat(CDec2(adjustments)));
            $('#txtAccumAmount' + SelectedZReadDataID).text(AccountingFormat(CDec2($('#txtNewAccumulatedAmount').val()) - CDec2(adjustments)));
            $('#txtAccumDate' + SelectedZReadDataID).text($.format.date(new Date(), 'MMM dd, yyyy'));
            $('#divEnterAccumulatedAmount').popup('close');
        });

        $('#btnReceiveData').click(function () {
            GetZReadDataFromServer();
            $('#divDataReceiveInServer').popup('close');
        })

        $('#btnReceiveFailed').click(function () {
            $('#divDataReceiveFailed').popup('close');
        })

        $('#btnGetTransactionData').click(function () {
            GetTransactionDetails();
        })
    });

    function SendZReadData() {
        var ZReadData = [];

        var data = {
            ZReadData: ZReadData
        }

        var i = 0;

        $('#tblBodyDT tr').each(function () {
            if (IsNumeric($('#txtAccumAmount' + $(this).data().ID).text())) {
                ZReadData[i] = {
                    ID: $(this).data().ID,
                    AccumulatedDate: $('#txtAccumDate' + $(this).data().ID).text(),
                    AccumulatedAmount: $('#txtAccumGross' + $(this).data().ID).text()
                }
                i++;
            }            
        })

        AjaxSend({
            url: 'Zapatero/SendZReadData',
            data: data,
            success: function (vData) {
                if (CBool(vData.Results)) {
                    $('#divDataReceiveInServer').popup('open');
                } else {
                    $('#divDataReceiveFailed').popup('open');                    
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(errorThrown);
            }
        })            
    }

    function GetZReadDataFromServer() {
        var data = {OrgID: sessionStorage.OrgID}
        AjaxSend({
            url: 'Zapatero/GetZReadDataFromServer',
            data: data,
            success: function (vData) {
                var vRows = vData.ReportPOS.Rows;
                var tBody = $('#tblBodyDT');

                tBody.empty();

                if (vRows.length > 0) {
                    for (var i = 0; i < vRows.length; i++) {

                        adjustments = CDec2(vRows[i].PrevTotalDiscount) + CDec2(vRows[i].PrevSalesReturnRefund) + CDec2(vRows[i].PrevSalesReturnExchange) + CDec2(vRows[i].PrevServiceCharge) + CDec2(vRows[i].PrevChargeAmount);

                        tBody                            
                            .append($('<tr>')
                                .data(vRows[i])
                                .append($('<td style=text-align:right>')
                                    .text('1-' + (i + 1))
                                )
                                .append($('<td>')
                                    .text(vRows[i].Location)
                                )
                                .append($('<td style=text-align:right>')
                                    .text(vRows[i].RegistrationNo)
                                )
                                .append($('<td>')
                                    .text('Jan 3, 2017')
                                )
                                .append($('<td style=text-align:right>')
                                    .text(AccountingFormat(vRows[i].PrevGrossSales))
                                )
                                .append($('<td style=text-align:right>')
                                    .text(AccountingFormat(adjustments))
                                )
                                .append($('<td style=text-align:right>')
                                    .text(AccountingFormat(CDec2(vRows[i].PrevGrossSales) - CDec2(adjustments)))
                                )
                                .append($('<td>')
                                    .text(vRows[i].SalesDate)
                                )
                                .append($('<td id=txtAccumGross' + vRows[i].ID + ' style=text-align:right>')
                                    .text('')
                                )
                                .append($('<td id=txtAccumAdjustment' + vRows[i].ID + '>')
                                    .text('')
                                )
                                .append($('<td id=txtAccumAmount' + vRows[i].ID + '>')
                                    .text('')
                                )
                                .append($('<td id=txtAccumDate' + vRows[i].ID + '>')
                                    .text('')
                                )
                                .click(function () {
                                    
                                    SelectedZReadDataID = $(this).data().ID;
                                    adjustments = CDec2($(this).data().PrevTotalDiscount) + CDec2($(this).data().PrevSalesReturnRefund) + CDec2($(this).data().PrevSalesReturnExchange) + CDec2($(this).data().PrevServiceCharge) + CDec2($(this).data().PrevChargeAmount);
                                    $('#txtNewAccumulatedAmount').val('');

                                    sessionStorage.PhoneName = $(this).data().PhoneName;
                                    sessionStorage.BIRMachineNo = $(this).data().BIRMachineNo;
                                    sessionStorage.Location = $(this).data().Location;
                                    sessionStorage.RegistrationNo = $(this).data().RegistrationNo;
                                    sessionStorage.LocationID = $(this).data().FOWSTID;
                                    sessionStorage.DeviceID = $(this).data().DeviceID;

                                    $('#divEnterAccumulatedAmount').popup('open');

                                    //NewMessageBox("", "Select an Option", [
                                    //        {
                                    //            Text: 'Make and Execute a Monthly Tax Budget',
                                    //            Click: function () {                                                    
                                    //                $('#divEnterAccumulatedAmount').popup('open');
                                    //            }
                                    //        },
                                    //        {
                                    //            Text: 'Zap Latest Accumulated Gross Sales Reading',
                                    //            Click: function () {                                                    
                                    //                window.open('Zapatero/TransactionDetail', '_self')
                                    //            }
                                    //        },
                                    //        {
                                    //            Text: 'Zap Ejournal Data (Item-Level Sales Data)',
                                    //            Click: function () {
                                    //                window.open('Zapatero/TaxPreparation', '_self')
                                    //            }
                                    //        }
                                    //]);
                                })
                            )
                    }
                }
            },
            error: function (vError) {
                
            }
        });
    }

    //function GetTransactionDetails() {

    //    var maxDays = 0;

    //    var now = new Date($('#selMonth').val() + '/1/' + $('#selYear').val());

    //    maxDays = new Date(now.getFullYear(), now.getMonth() + 1, 0).getDate();

    //    var data = {
    //        OrgID: sessionStorage.OrgID,
    //        FOWSTID: FOWSTID,
    //        DeviceID: DeviceID,
    //        DateFrom: $('#selMonth').val() + '/1/' + $('#selYear').val(),
    //        DateTo: $('#selMonth').val() + '/' + maxDays + '/' + $('#selYear').val()
    //    }

    //    AjaxSend({
    //        url: 'Zapatero/GetSalesTransactionDetails',
    //        data: data,
    //        success: function (vData) {
    //            var vRows = vData.ReportPOS.Rows;
    //            var tBody = $('#tblTransactionDetailsBody');

    //            tBody.empty();

    //            if (vRows.length > 0) {
    //                for (var i = 0; i < vRows.length; i++) {

    //                    tBody
    //                        .append($('<tr>')
    //                            .data(vRows[i])
    //                            //.append($('<td>')
    //                            //    .text(vRows[i].ProductNo)
    //                            //)
    //                            .append($('<td>')
    //                                .text(vRows[i].ProductName)
    //                            )
    //                            .append($('<td style=text-align:right>')
    //                                .text(vRows[i].TransactionNo)
    //                            )
    //                            .append($('<td>')
    //                                .text(vRows[i].TransactionDate)
    //                            )
    //                            .append($('<td>')
    //                                .text(vRows[i].Customer)
    //                            )
    //                            .append($('<td style=text-align:right>')
    //                                .text(CInt2(vRows[i].Qty))
    //                            )
    //                            .append($('<td>')
    //                                .text(vRows[i].UnitMeasure)
    //                            )
    //                            .append($('<td style=text-align:right>')
    //                                .text(AccountingFormat(vRows[i].UnitPrice))
    //                            )
    //                            .append($('<td style=text-align:right>')
    //                                .text(AccountingFormat(vRows[i].SubTotal))
    //                            )
    //                            .append($('<td style=text-align:right>')
    //                                .text(AccountingFormat(vRows[i].DiscountAmount))
    //                            )
    //                            //.append($('<td style=text-align:right>')
    //                            //    .text(vRows[i].DiscountedUnitPrice)
    //                            //)
    //                            //.append($('<td style=text-align:right>')
    //                            //    .text(vRows[i].DiscountedSubTotal)
    //                            //)
    //                            //.append($('<td style=text-align:right>')
    //                            //    .text(vRows[i].VATPercent)
    //                            //)
    //                            //.append($('<td style=text-align:right>')
    //                            //    .text(vRows[i].VatableSales)
    //                            //)
    //                            //.append($('<td style=text-align:right>')
    //                            //    .text(vRows[i].VATAmount)
    //                            //)
    //                            .append($('<td>')
    //                                .text(vRows[i].CashierName)
    //                            )
    //                            .append($('<td style=text-align:right>')
    //                                .text(AccountingFormat(vRows[i].CashAmount))
    //                            )
    //                            .append($('<td style=text-align:right>')
    //                                .text(AccountingFormat(vRows[i].CheckPaymentAmount))
    //                            )
    //                            .append($('<td style=text-align:right>')
    //                                .text(AccountingFormat(vRows[i].DebitCardAmount))
    //                            )
    //                            .append($('<td style=text-align:right>')
    //                                .text(AccountingFormat(vRows[i].CreditCardAmount))
    //                            )
    //                            .append($('<td style=text-align:right>')
    //                                .text(AccountingFormat(vRows[i].SRVAmount))
    //                            )
    //                            .append($('<td style=text-align:right>')
    //                                .text(AccountingFormat(vRows[i].OnCreditAmount))
    //                            )
    //                            .append($('<td style=text-align:right>')
    //                                .text(AccountingFormat(vRows[i].OtherTypeAmount))
    //                            )
    //                            //.append($('<td style=text-align:right>')
    //                            //    .text(vRows[i].CustomerChange)
    //                            //)                            
    //                            .click(function () {
    //                            })
    //                        )
    //                }
    //            }
    //        }
    //    })
    //}
});

