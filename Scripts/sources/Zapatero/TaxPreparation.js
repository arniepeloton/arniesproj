require([
        'common',
        '/Scripts/jquery.dataTables.min.js',
        '/Scripts/jquery.dataTables.buttons.min.js'

], function () {
    var SelectedZReadDataID = '';
    var adjustments = 0;
    var TransactionDetailsTable = $('#tblTransactionDetail')
    var transactionDetailsDatatable;

    $(function () {

        $(document).ready(function () {
            $('#selYear').val('').selectmenu('refresh');
            $('#lblBranch').val(sessionStorage.Location);
            $('#lblRegistrationNo').val(sessionStorage.RegistrationNo);
            $('#lblMachineName').val(sessionStorage.PhoneName);
            $('#lblMachineNo').val(sessionStorage.BIRMachineNo);
            DisplayData();
        }).on('click', '#btnRecacheAllMonths', function () {
            for (var i = 1; i <= 12; i++) {
                if (CInt2($('#txtSalesValue' + i).val()) > 0) {
                    GetTransactionDetails(i, $('#selYear').val(), $('#txtSalesValue' + i).val());
                    $('#lblComputedSales' + i).text('Computing...')
                }
            }

            setTimeout(function () {
                CurrentMonth = 1;
                FollowupComputedSalesData()
            }, 2000);
        }).on('focusout', '#tblTransactionDetail tbody tr td input', function () {
            RecomputeTotals(this);
        })

        $('#btnGo').click(function () {
            if (CInt2($('#selYear').val()) > 0) {
                NewMessageBox("The system is caching your data. This might take a while (depending on how many line transactions you have made per month). You can come back later to check the cached data.", "Cache Data")
                GetYearlySales();
            }
        })

    });


    function GetYearlySales() {
        var loading = 'Loading...';

        var data = {
            OrgID: sessionStorage.OrgID,
            LocationID: sessionStorage.LocationID,
            DeviceID: sessionStorage.DeviceID,
            Year: $('#selYear').val(),
        }

        AjaxSend({
            url: 'Zapatero/GetYearlySales',
            data: data,
            success: function (vData) {

            }
        });

        for (var i = 1; i <= 12; i++) {
            $('#lblActualSI' + i).text(loading);
            $('#lblActualSales' + i).text(loading);
            //$('#txtSIFrom' + mMonth).val();
            //$('#txtSITo' + mMonth).val();
            //$('#txtSalesPercent' + i).val(loading);
            //$('#txtSalesValue' + i).val(loading);
            //$('#lblComputedSI' + mMonth).text(CInt2(vRow[0].SITo));
            $('#lblComputedSales' + i).text(loading);

            FollowupData(i)
        }

        //setTimeout(function () {
        //    CurrentMonth = 1;
        //    FollowupData()
        //}, 2000);
    }

    var IsFromLineData = false;

    function FollowupData(mCurrentMonth) {
        var CurrentMonth = mCurrentMonth;

        var data = {
            OrgID: sessionStorage.OrgID,
            LocationID: sessionStorage.LocationID,
            DeviceID: sessionStorage.DeviceID,
            DateFrom: CurrentMonth + '/1/' + $('#selYear').val()
        }
        AjaxSend({
            url: 'Zapatero/GetDataHeader',
            data: data,
            success: function (vData) {
                if (vData != null) {
                    DisplayCurrentData(vData.ReportPOS.Rows);
                    //if (CurrentMonth == 12) {
                    //    return;
                    //} else {
                    //    CurrentMonth++;
                    //}
                } else {
                    setTimeout(function () {
                        FollowupData(CurrentMonth)
                    }, 2000)
                }
            }
        })
    }

    function FollowupComputedSalesData() {

        if (!$('#chk' + CurrentMonth).is(':checked')) {

            var data = {
                OrgID: sessionStorage.OrgID,
                LocationID: sessionStorage.LocationID,
                DeviceID: sessionStorage.DeviceID,
                DateFrom: CurrentMonth + '/1/' + $('#selYear').val()
            }

            if (CInt2($('#txtSalesValue' + CurrentMonth).val()) > 0) {
                AjaxSend({
                    url: 'Zapatero/GetDataHeader',
                    data: data,
                    success: function (vData) {
                        if (vData != null) {
                            if (vData.ReportPOS.Rows.length > 0) {
                                if (CDec2(vData.ReportPOS.Rows[0].AdjustedValue) > 0) {
                                    $('#lblComputedSales' + CurrentMonth).text(AccountingFormat(vData.ReportPOS.Rows[0].AdjustedValue));
                                    $('#lblFilename' + CurrentMonth).text(vData.ReportPOS.Rows[0].FileName);

                                    if (CurrentMonth == 12 || IsFromLineData) {
                                        return;
                                    }
                                    else {
                                        CurrentMonth++;
                                    }
                                }
                            }
                        }

                        setTimeout(function () {
                            IsFromLineData = false;
                            FollowupComputedSalesData();
                        }, 2000)

                    }
                })
            } else {
                CurrentMonth++;
                setTimeout(function () {
                    FollowupComputedSalesData();
                }, 2000)
            }
        } else {
            CurrentMonth++;

            setTimeout(function () {
                IsFromLineData = false;
                FollowupComputedSalesData();
            }, 2000)
        }
    }

    function DisplayCurrentData(vRow) {

        var mMonth = CStr2(((new Date(vRow[0].TransactionMonthYear)).getMonth() + 1));

        $('#lblActualSI' + mMonth).text(CInt2(vRow[0].SITo));
        $('#lblActualSales' + mMonth).text(AccountingFormat(vRow[0].OriginalValue));
        //$('#txtSIFrom' + mMonth).val();
        //$('#txtSITo' + mMonth).val();
        $('#txtSalesPercent' + mMonth).val(CInt2(vRow[0].TargetPercentage));
        $('#txtSalesValue' + mMonth).val(AccountingFormat(vRow[0].TargetValue));
        //$('#lblComputedSI' + mMonth).text(CInt2(vRow[0].SITo));
        $('#lblComputedSales' + mMonth).text(AccountingFormat(vRow[0].AdjustedValue));
        $('#lblFilename' + mMonth).text(vRow[0].FileName || '');
    }

    function DisplayData() {
        var tBody = $('#tblTransactionDetailsBody');
        var loading = '-';
        var mMonth = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"]
        tBody.empty();

        for (var i = 1; i <= 12; i++) {
            data = {
                i: i
            }

            tBody
                .append($('<tr>')
                    .append($('<td>')
                        .text(mMonth[i - 1])
                        )
                    .append($('<td ID="lblActualSI' + i + '">')
                        .text(loading)
                        )
                    .append($('<td ID="lblActualSales' + i + '">')
                        .text(loading)
                        )
                    //.append($('<td>')
                    //    .html('<input type="text" style="width:70px" ID="txtSIFrom' + i + '"/>')
                    //    )
                    //.append($('<td>')
                    //    .html('<input type="text" style="width:70px" ID="txtSITo' + i + '"/>')
                    //    )
                    .append($('<td>')
                        .html('<input type="text" style="width:50px" ID="txtSalesPercent' + i + '"/>')
                        )
                    .append($('<td>')
                        .html('<input type="text" ID="txtSalesValue' + i + '"/>')
                        )
                    .append($('<td>')
                        .html('<input type="checkbox" ID="chk' + i + '"/>')
                        )
                        .data(data)
                        .click(function () {
                            TagUntagMonthsData($(this).data().i, $('#selYear').val(), $('#chk' + $(this).data().i).is(':checked'));
                        })
                    //.append($('<td ID="lblComputedSI' + i + '">')
                    //    .text(loading)
                    //    )
                    .append($('<td ID="lblComputedSales' + i + '">')
                        .text(loading)
                        )
                    .append($('<td style="text-align:right">')
                        //.html('<button ID="RecacheSales' + i + '">Generate ' + mMonth[i - 1] + ' Data</button>')
                        .html('<select ID="selAction' + i + '" style="width:150px"> ' +
                                '<option>Select Action</option>' +
                                '<option>Generate Best Fit Data for ' + mMonth[i - 1] + '</option>' +
                                '<option>Download ' + mMonth[i - 1] + ' E-Journal</option>' +
                                '</select>')
                        .data(data)
                        .change(function () {
                            CurrentMonth = $(this).data().i;

                            if ($('#selAction' + CurrentMonth + ' option:selected').text().indexOf('Generate') >= 0) {
                                if (CDec2($('#txtSalesValue' + CurrentMonth).val()) > 0) {
                                    if (CDec2($('#lblComputedSales' + CurrentMonth).text()) == 0) {
                                        if (!$('#chk' + CurrentMonth).is(':checked')) {
                                            NewMessageBox("The system is generating your data. You can come back later to check and download the generated data.", "Generate Data")

                                            IsFromLineData = true;
                                            GetTransactionDetails(CurrentMonth, $('#selYear').val(), $('#txtSalesValue' + CurrentMonth).val())
                                            FollowupComputedSalesData()
                                        } else {
                                            NewMessageBox("Sorry, you can't adjust data to best fit if D.N.M(Do Not Modify) column is checked", "Generate Data")
                                        }
                                    }
                                }
                            } else if ($('#selAction' + CurrentMonth + ' option:selected').text().indexOf('Download') >= 0) {
                                if ($('#lblFilename' + CurrentMonth).text() != '') {
                                    //window.location = GetPathFormat('http://localhost:8080/ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + $('#lblFilename' + CurrentMonth).text());
                                    window.location = GetPathFormat('http://122.52.123.251/ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + $('#lblFilename' + CurrentMonth).text());
                                    //window.location = GetPathFormat('http://124.106.167.156:8083/ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + vData.ReportPOS);
                                    //var url = 'http://124.106.167.156:8083/ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + vData.ReportPOS;
                                    //window.open(url,'_blank' );
                                }
                            }
                        })
                        )
                    .append($('<td ID="lblFilename' + i + '" style="display:none">')
                        .text(loading)
                    )
               )
        }

        //for Generate All button/combo box

        tBody
            .append($('<tr>')
                .append($('<td>')
                    .text('')
                    )
                .append($('<td>')
                    .text('')
                    )
                .append($('<td>')
                    .text('')
                    )
                //.append($('<td>')
                //    .html('<input type="text" style="width:70px" ID="txtSIFrom' + i + '"/>')
                //    )
                //.append($('<td>')
                //    .html('<input type="text" style="width:70px" ID="txtSITo' + i + '"/>')
                //    )
                .append($('<td>')
                    .html('')
                    )
                .append($('<td>')
                    .html('')
                    )
                .append($('<td>')
                    .html('')
                    )
                //.append($('<td ID="lblComputedSI' + i + '">')
                //    .text(loading)
                //    )
                .append($('<td>')
                    .text('')
                    )
                .append($('<td style="text-align:right">')
                    //.html('<button ID="btnRecacheAllMonths"> Generate All Data </button>')
                    //)
                    .html('<select ID="selActionAll" style="width:150px"> ' +
                                '<option>Select Action</option>' +
                                '<option>Generate All Data</option>' +
                                '<option>Download All EJournal</option>' +
                                '</select>')
                    .change(function () {
                        if ($('#selActionAll option:selected').text().indexOf('Generate') >= 0) {

                            for (var i = 1; i <= 12; i++) {
                                if (CDec2($('#txtSalesValue' + i).val()) > 0) {
                                    if (CDec2($('#lblComputedSales' + i).text()) == 0) {
                                        if (!$('#chk' + i).is(':checked')) {
                                            GetTransactionDetails(i, $('#selYear').val(), $('#txtSalesValue' + i).val());
                                            $('#lblComputedSales' + i).text('Computing...')
                                        }
                                    }
                                }
                            }

                            NewMessageBox("The system is generating your data. You can come back later to check and download the generated data.", "Generate Data")

                            setTimeout(function () {
                                CurrentMonth = 1;
                                FollowupComputedSalesData()
                            }, 2000);

                        } else if ($('#selActionAll option:selected').text().indexOf('Download') >= 0) {

                            for (var i = 1; i <= 12; i++) {
                                if ($('#lblFilename' + i).text() != '') {
                                    //window.open(GetPathFormat('http://localhost:8080/ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + $('#lblFilename' + i).text()), '_blank');
                                    //window.location = GetPathFormat('http://localhost:8080/ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + $('#lblFilename' + i).text());
                                    window.location = GetPathFormat('http://122.52.123.251/ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + $('#lblFilename' + i).text());
                                    //window.location = GetPathFormat('http://124.106.167.156:8083/ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + vData.ReportPOS);
                                    //var url = 'http://124.106.167.156:8083/ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + vData.ReportPOS;
                                    //window.open(url,'_blank' );
                                }
                            }

                        }

                    })
                    )
                )
    }

    function TagUntagMonthsData(mMonth, mYear, isTag) {
        var data = {
            OrgID: sessionStorage.OrgID,
            LocationID: sessionStorage.LocationID,
            DeviceID: sessionStorage.DeviceID,
            Year: mYear,
            Month: mMonth,
            IsTag: isTag
        }

        AjaxSend({
            url: 'Zapatero/TagUntagMonthsData',
            data: data,
            success: function (vData) {

            }
        })
    }

    function GetTransactionDetails(mMonth, mYear, ESalesValue) {

        var maxDays = 0;

        var now = new Date(mMonth + '/1/' + mYear);

        maxDays = new Date(now.getFullYear(), now.getMonth() + 1, 0).getDate();

        var data = {
            OrgID: sessionStorage.OrgID,
            LocationID: sessionStorage.LocationID,
            DeviceID: sessionStorage.DeviceID,
            DateFrom: mMonth + '/1/' + mYear,
            DateTo: mMonth + '/' + maxDays + '/' + mYear,
            IsFromESales: true,
            ESalesPercentage: ESalesValue
        }

        AjaxSend({
            url: 'Zapatero/GetSalesTransactionDetails',
            data: data,
            success: function (vData) {
                
            }
        })
    }

    function RecomputeTotals(field) {
        var i;

        if (IsInteger(field.id.substr(field.id.length - 2))) {
            i = field.id.substr(field.id.length - 2)
        } else {
            i = field.id.substr(field.id.length - 1)
        }

        if (field.id.indexOf('Percent') >= 0) {
            $('#txtSalesValue' + i).val(AccountingFormat(CDec2($('#lblActualSales' + i).text()) * (CDec2($('#txtSalesPercent' + i).val()) / 100)));
        } else {
            if (CDec2($('#txtSalesValue' + i).val()) > 0) {
                $('#txtSalesPercent' + i).val(AccountingFormat((CDec2($('#txtSalesValue' + i).val()) / CDec2($('#lblActualSales' + i).text()))*100));
            }
        }
        
    }
});

