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

    });

    function GetZReadDataFromServer() {
        var data = {OrgID: sessionStorage.OrgID}
        AjaxSend({
            url: 'Zapatero/GetRegisteredDevices',
            data: data,
            success: function (vData) {
                var vRows = vData.ReportPOS.Rows;
                var tBody = $('#tblBodyDT');

                tBody.empty();

                if (vRows.length > 0) {
                    for (var i = 0; i < vRows.length; i++) {

                        tBody                            
                            .append($('<tr>')
                                .data(vRows[i])                                
                                .append($('<td>')
                                    .text(vRows[i].PhoneName)
                                )
                                .append($('<td style=text-align:right>')
                                    .text(vRows[i].Location)
                                )
                                .append($('<td style=text-align:right>')
                                    .text(vRows[i].BIRMachineNo)
                                )
                                .append($('<td style=text-align:right>')
                                    .text(vRows[i].RegistrationNo)
                                )
                                .append($('<td style=text-align:right>')
                                    .text(vRows[i].ApplicationDate)
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

                                    window.open('TransactionDetail', '_self')

                                    //NewMessageBox("", "Select an Option", [
                                    //        {
                                    //            Text: 'Make and Execute a Monthly Tax Budget',
                                    //            Click: function () {
                                    //                window.open('TaxPreparation', '_self')
                                    //            }
                                    //        },
                                    //        {
                                    //            Text: 'Zap Latest Accumulated Gross Sales Reading',
                                    //            Click: function () {                                                    
                                    //                window.open('/Zapatero', '_self')
                                    //            }
                                    //        },
                                    //        {
                                    //            Text: 'Zap Ejournal Data (Item-Level Sales Data)',
                                    //            Click: function () {
                                    //                window.open('TransactionDetail', '_self')
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

});

