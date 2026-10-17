require([
    'common'
], function () {

    $(function () {
        GetSmSUsage();
    });


    function GetSmSUsage() {
        AjaxSend({
            url: 'CRMDashboard/GetSmSUsage',
            success: function (vData) {
                DisplayResults(vData.Results.Rows);
            }
        })
    }

    function DisplayResults(vRows) {
        var tblSmSUsage = $('#tblSmSUsagebody');

        tblSmSUsage.empty();

        for (var i = 0; i <= vRows.length - 1; i++) {

            if (vRows[i].Telco == 'Total') {
                tblSmSUsage.append($('<tr>')
                                        .css('cursor', 'pointer')
                                        .data(vRows[i])
                                        .append($('<td style="font-weight:bold; font-size:larger">')
                                            .text(vRows[i].Telco)
                                        )
                                        .append($('<td style="text-align:right; font-weight:bold; font-size:larger">')
                                            .text(vRows[i].DeviceToModem)
                                        )
                                        .append($('<td style="text-align:right; font-weight:bold; font-size:larger">')
                                            .text('-')
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text('Click Row to see Details')
                                        )
                                        .click(function () {
                                            GetSmSDetailsByTelco('All');
                                        })
                                        )
            } else {
                tblSmSUsage.append($('<tr>')
                                        .css('cursor', 'pointer')
                                        .data(vRows[i])
                                        .append($('<td>')
                                            .text(vRows[i].Telco)
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text(vRows[i].DeviceToModem)
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text('-')
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text('Click Row to see Details')
                                        )
                                        .click(function () {
                                            GetSmSDetailsByTelco($(this).data().ID);
                                        })
                                        )
            }
        }
    }

    function GetSmSDetailsByTelco(vFilter) {
        var data = { TelcoID: vFilter };

        AjaxSend({
            url: 'CRMDashboard/GetSmSUsagePerTelco',
            data: data,
            success: function (vData) {
                DisplaySmSDetailsByTelco(vData.Results.Rows);
            }
        })
    }

    function DisplaySmSDetailsByTelco(vRows) {
        var tblSmSUsage = $('#tblSmSUsageDetailbody');

        tblSmSUsage.empty();

        for (var i = 0; i <= vRows.length - 1; i++) {

                tblSmSUsage.append($('<tr>')
                                        .css('cursor', 'pointer')
                                        .data(vRows[i])
                                        .append($('<td>')
                                            .text(vRows[i].SendTo)
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text(vRows[i].Message)
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text(vRows[i].LastUpdated)
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text(vRows[i].SentBy)
                                        )                                        
                                        )
            }
        }    
})// end require