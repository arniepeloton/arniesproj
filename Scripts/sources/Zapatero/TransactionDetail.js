require([
        'common',
        '/Scripts/jquery.dataTables.min.js',
        '/Scripts/jquery.dataTables.buttons.min.js'

], function () {
    var SelectedZReadDataID = '';
    var adjustments = 0;
    var TransactionDetailsTable = $('#tblTransactionDetail')
    var transactionDetailsDatatable;
    var DateFrom = undefined;
    var DateTo = undefined;


    require([
        '/Scripts/metro.min.js'
    ], function () {
        require([
            '/Scripts/metro-calendar.js',
            '/Scripts/metro-datepicker.js',
            '/Scripts/jquery.dataTables.min.js'
        ], function () {
           
            $(function () {

                $(document).ready(function () {
                    
                    $(".fromCalendar").calendar({
                        format: "yyyy-mm-dd",
                        multiSelect: false,
                        otherDays: true,
                        getDates: function (data) {
                            $("#txtDateFrom").text(data);
                        }
                    });

                    $(".toCalendar").calendar({
                        format: "yyyy-mm-dd",
                        multiSelect: false,
                        otherDays: true,
                        getDates: function (data) {
                            $("#txtDateTo").text(data);
                        }
                    });

                    $('#selMonth').val('').selectmenu('refresh');
                    $('#selYear').val('').selectmenu('refresh');
                    $('#lblBranch').val(sessionStorage.Location);
                    $('#lblRegistrationNo').val(sessionStorage.RegistrationNo);
                    $('#lblMachineName').val(sessionStorage.PhoneName);
                    $('#lblMachineNo').val(sessionStorage.BIRMachineNo);

                }).on('focusout', '#tblTransactionDetail tbody tr td input', function () {
                    RecomputeTotals();
                })

                $('#btnDateRange').click(function () {
                    $('#panDateRange').popup('open');
                })

                $('#btnRequisitionOk').click(function () {
                    DateFrom = $('#txtDateFrom').text();
                    DateTo = $('#txtDateTo').text();

                    $('#btnDateRange').text($('#txtDateFrom').text() + ' to ' + $('#txtDateTo').text())

                    $('#panDateRange').popup('close');
                })

                $('#btnGetTransactionData').click(function () {
                    $('#lblOriginalGross').val('')
                    $('#lblAdjustedGrossAmount').val('')
                    $('#lblAdjustedGrossPercentage').val('')
                    $('#lblDifference').val('')
                    $('#lblActualTotalBestFitData').val('')
                    GetTransactionDetails(false,false);
                })

                $('#btnDownloadESalesData').click(function () {
                    DownloadESalesData();
                    
                    var data = {};

                    data.OrgID = sessionStorage.OrgID;
                    data.DeviceID = sessionStorage.DeviceID;
                    data.LocationID = sessionStorage.LocationID;
                    data.LocationName = sessionStorage.Location;
                    data.DateFrom = DateFrom;
                    data.DateTo = DateTo;

                    AjaxSend({
                        url: 'ExternalServices/DownloadEJournal',
                        data: data,
                        timeout: 600000,
                        success: function (vData) {
                            if (vData != null) {
                                window.location = GetPathFormat("/ReportsPOS/DownloadReportFile?OrgID=" + sessionStorage.OrgID + '&ReportLink=' + vData.ReportPOS);
                                //window.location = GetPathFormat('http://localhost:8080/ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + vData.ReportPOS);
                                //window.location = GetPathFormat('http://http://119.93.147.163/ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + vData.ReportPOS); 
                            }
                        }
                    })
                })


                $('#btnAdjustZRead').click(function () {
                    var data = {};

                    data.OrgID = sessionStorage.OrgID;
                    data.DeviceID = sessionStorage.DeviceID;
                    data.LocationID = sessionStorage.LocationID;
                    data.DateFrom = DateFrom;
                    data.DateTo = DateTo;

                    AjaxSend({
                        url: 'ExternalServices/AdjustZRead',
                        data: data,
                        timeout: 600000,
                        success: function (vData) {
                            if (vData != null) {
                                window.location = GetPathFormat('http://http://119.93.147.163/ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + vData.ReportPOS);
                            }
                        }
                    })
                })
                
                $('#btnDownloadData').click(function () {
                    var data = {};

                    //SaveTransactionData();
                    data.OrgID = sessionStorage.OrgID;
                    data.DeviceID = sessionStorage.DeviceID;
                    data.LocationID = sessionStorage.LocationID;
                    data.LocationName = sessionStorage.Location;
                    data.DateFrom = DateFrom;
                    data.DateTo = DateTo;

                    AjaxSend({
                        url: 'ExternalServices/DownloadEJournal',
                        data: data,
                        timeout: 600000,
                        success: function (vData) {
                            if (vData != null) {
                                //localhost:8080
                                //window.location = GetPathFormat('http://localhost:8080/ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + vData.ReportPOS);
                                window.location = GetPathFormat('http://http://119.93.147.163/ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + vData.ReportPOS); 
                                //window.location = GetPathFormat('http://124.106.167.156:8083/ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + vData.ReportPOS);
                                //var url = 'http://124.106.167.156:8083/ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + vData.ReportPOS;
                                //window.open(url,'_blank' );
                            }
                        }
                    })

                })

                $('#btnSaveTransactionData').click(function () {
                    SaveTransactionData();
                })

                //$('#btnRecompute').click(function () {
                //    GetTransactionDetails(true,false);
                //})

                $('#btnRecomputeDiscount').click(function () {
                    //$('#panSetDiscount').popup('open');
                    //RecomputeforDiscount();
                    //$('#panSetDiscount').popup('close');
                    GetTransactionDetails(true, true);
                })

                //$('#btnApplyDiscount').click(function () {
                //    $('#panSetDiscount').popup('close');
                //})

                $('#lblAdjustedGrossAmount').keyup(function () {
                    $('#lblAdjustedGrossPercentage').val(AccountingFormat(CDec2($('#lblAdjustedGrossAmount').val()) / CDec2($('#lblOriginalGross').val()) * 100) + '%')
                    $('#lblDifference').val(AccountingFormat(CDec2($('#lblOriginalGross').val()) - CDec2($('#lblAdjustedGrossAmount').val())))
                })

                $('#lblAdjustedGrossPercentage').keyup(function () {
                    $('#lblAdjustedGrossAmount').val(AccountingFormat(CDec2($('#lblOriginalGross').val()) * (CDec2($('#lblAdjustedGrossPercentage').val().replace('%', '')) / 100)))
                    $('#lblDifference').val(AccountingFormat(CDec2($('#lblOriginalGross').val()) - CDec2($('#lblAdjustedGrossAmount').val())))
                })

                $('#btnSaveZReadData').click(function () {
                    SaveZReadData();
                })

                $('#btnGenerateDailyZRead').click(function () {
                    $('#panGenerateDailyZRead').popup('open');
                })

                $('#btnDownloadDailyZReadData').click(function () {
                    var mData = [];

                    $('#tblZReadBody tr').each(function (e) {
                        mData.push({
                            Date: $(this).find('td:eq(0)').text(),
                            GrandTotal: AccountingFormat($(this).find('td:eq(3) input').val()),
                            GrossSales: AccountingFormat($(this).find('td:eq(6) input').val()),
                            CashSales: AccountingFormat($(this).find('td:eq(7) input').val()),
                            CreditSales: AccountingFormat($(this).find('td:eq(8) input').val()),
                            BeginningInvoice: $(this).find('td:eq(12) input').val(),
                            EndingInvoice: $(this).find('td:eq(13) input').val(),
                            DeviceID: sessionStorage.DeviceID,
                            StoreID: sessionStorage.LocationID
                        })                        
                    });
                    
                    var data = {
                        mData: mData
                    }

                    AjaxSend({
                        url: 'ExternalServices/DownloadDailyZReadData',
                        data: data,
                        success: function (vData) {
                            window.location = GetPathFormat("/ReportsPOS/DownloadReportFile?OrgID=" + sessionStorage.OrgID + '&ReportLink=' + vData.Result);
                        }
                    })

                })
            });
        });
    });

    function GetTransactionDetails(IsFromESales, IsUseDiscountPercent) {

        var maxDays = 0;

        var now = new Date($('#selMonth').val() + '/1/' + $('#selYear').val());

        maxDays = new Date(now.getFullYear(), now.getMonth() + 1, 0).getDate();

        var data = {
            OrgID: sessionStorage.OrgID,
            LocationID: sessionStorage.LocationID,
            DeviceID: sessionStorage.DeviceID,
            DateFrom: DateFrom,
            DateTo: DateTo,
            IsFromESales: CBool(IsFromESales),
            ESalesPercentage: $('#lblAdjustedGrossAmount').val(),
            DiscountPercentage: $('#lblAdjustedGrossPercentage').val(),
            IsUseDiscountPercent: IsUseDiscountPercent,
            MIN: $('#lblMachineNo').val()
        }

        GetZReadData(data);
        GetMonthlyZReadData(data);

        AjaxSend({
            url: 'Zapatero/GetSalesTransactionDetails',
            data: data,
            timeout: 600000,
            success: function (vData) {
                if (vData.ReportPOS == 'Max Records') {
                    NewMessageBox('The data that you are trying to view cannot be loaded. Please shorten date range of the data that you want to view. ');
                } else {
                    var vRows = vData.ReportPOS.Rows;
                    var tBody = $('#tblTransactionDetailsBody');
                    var totalGross = 0;
                    var HasReported = false;

                    tBody.empty();

                    if (vRows.length > 0) {

                        for (var i = 0; i < vRows.length; i++) {

                            totalGross += CDec2(vRows[i].SubTotal);

                            if (CBool(vRows[i].IsReported)) {
                                HasReported = true;
                            }

                            tBody
                                .append($('<tr>')
                                    .data('data', vRows[i])                                   
                                    .append($('<td>')
                                        .text(vRows[i].ProductName)
                                    )
                                    .append($('<td style=text-align:right>')
                                        .append($('<input ' + (HasReported ? 'disabled' : '') + ' type="number" class="transactionno" value=' + CDec2(vRows[i].TransactionNo) + ' />')
                                        )
                                    )
                                    .append($('<td>')
                                        .text($.format.date(new Date(vRows[i].TransactionDate), 'MM/dd/yyyy'))
                                    )
                                    .append($('<td>')
                                        .text(vRows[i].Customer)
                                    )
                                    .append($('<td style=text-align:right>')
                                        .append($('<input ' + (HasReported ? 'disabled' : '') + ' type="number" class="qty" value=' + CInt2(vRows[i].Quantity) + ' id=txtQuantity' + vRows[i].ID + ' />')
                                            .data('data', vRows[i])
                                            .focusout(function () {
                                                $('#txtSubTotal' + $(this).data('data').ID).text(AccountingFormat(CDec2($(this).val()) * CDec2($('#txtUnitPrice' + $(this).data('data').ID).val())))
                                                // CDec2($(this).data('data').UnitPrice)))

                                                RecomputeTotals();
                                            })
                                        )
                                    )
                                    .append($('<td>')
                                        .text(vRows[i].UnitMeasure)
                                    )
                                    .append($('<td style=text-align:right>')
                                        .append($('<input ' + (HasReported ? 'disabled' : '') + ' type="number" class="unitprice" value=' + CDec2(vRows[i].UnitPrice) + ' id=txtUnitPrice' + vRows[i].ID + ' />')
                                            .data('data', vRows[i])
                                            .focusout(function () {
                                                $('#txtSubTotal' + $(this).data('data').ID).text(AccountingFormat(CDec2($(this).val()) * CDec2($('#txtQuantity' + $(this).data('data').ID).val())))
                                                RecomputeTotals();
                                            })
                                        )
                                    )
                                    .append($('<td style=text-align:right id=txtSubTotal' + vRows[i].ID + '>')
                                        .text(AccountingFormat(vRows[i].SubTotal))
                                    )
                                    .append($('<td style=text-align:right>')
                                        .text(AccountingFormat(vRows[i].DiscountAmount))
                                    )                                   
                                    .append($('<td>')
                                        .text(vRows[i].CashierName)
                                    )
                                    .append($('<td style=text-align:right>')
                                        .text(AccountingFormat(vRows[i].CashAmount))
                                    )
                                    .append($('<td style=text-align:right>')
                                        .text(AccountingFormat(vRows[i].CheckPaymentAmount))
                                    )
                                    .append($('<td style=text-align:right>')
                                        .text(AccountingFormat(vRows[i].DebitCardAmount))
                                    )
                                    .append($('<td style=text-align:right>')
                                        .text(AccountingFormat(vRows[i].CreditCardAmount))
                                    )
                                    .append($('<td style=text-align:right>')
                                        .text(AccountingFormat(vRows[i].SRVAmount))
                                    )
                                    .append($('<td style=text-align:right>')
                                        .text(AccountingFormat(vRows[i].OnCreditAmount))
                                    )
                                    .append($('<td style=text-align:right>')
                                        .text(AccountingFormat(vRows[i].OtherTypeAmount))
                                    )
                                    //.append($('<td style=text-align:right>')
                                    //    .text(vRows[i].CustomerChange)
                                    //)                            
                                    .click(function () {
                                    })
                                )
                        }
                    }

                    $('#lblAdjustedGross').val(AccountingFormat(totalGross));

                    if (CBool(IsFromESales)) {
                        if ($('#lblOriginalGross').val() == '') {
                            $('#lblOriginalGross').val(AccountingFormat(totalGross));
                        }

                        //$('#lblDifference').val('Margin(%)' + AccountingFormat((1 - (totalGross / CDec2($('#lblOriginalGross').val()))) * 100))
                        $('#lblActualTotalBestFitData').val(AccountingFormat(CDec2(totalGross)))
                    } else {
                        $('#lblOriginalGross').val(AccountingFormat(totalGross));
                    }
                }
            }
        })
    }

    function DownloadESalesData() {
        var data = {
            MIN: $('#lblMachineNo').val(),
            MonthYear: DateFrom,
            OrgID: sessionStorage.OrgID,
            LocationID: sessionStorage.LocationID,
            DeviceID: sessionStorage.DeviceID,
            DateFrom: DateFrom
        }

        AjaxSend({
            url: 'ExternalServices/DownloadESalesData',
            data: data,
            success: function (vData) {
                //var url = 'http://192.168.15.254:8083/ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + vData.ReportPOS;
                window.open(GetPathFormat("/ReportsPOS/DownloadReportFile?OrgID=" + sessionStorage.OrgID + '&ReportLink=' + vData.Result), '_blank');
                window.open(GetPathFormat("/ReportsPOS/DownloadReportFile?OrgID=" + sessionStorage.OrgID + '&ReportLink=' + vData.Ejournal),'_blank' );
                //window.location = 
                //window.location = 
                //window.location = GetPathFormat("/DashboardsAndReports/DownloadExcelReportFile?FileName=" + $.trim(vData.Result));
            }
        })
    }

    function RecomputeTotals() {
        var totalGross = 0;

        $('#tblTransactionDetailsBody tr').each(function () {
            
            //if (CInt2($(this).val()) > 0) {
            //totalGross += (CDec2($(this).find('.unitprice').val()) * CDec2($(this).find('.qty').val())) - CDec2($(this).data().DiscountAmount);
            totalGross += CDec2($('#txtSubTotal' + $(this).data('data').ID).text());
            //}
        });

        //transactionDetailsDatatable.columns(4).every(function () {
        //    var data = this.data();
        //    totalGross += (CDec2(transactionDetailsDatatable.row(data.UnitPrice)) * CDec2($(this).val())) - CDec2(transactionDetailsDatatable.row(data.DiscountAmount));
        //    // ... do something with data(), or this.nodes(), etc
        //});

        $('#lblActualTotalBestFitData').val(AccountingFormat(totalGross));
    }

    function RecomputeforDiscount() {
        var Discount;
        var tempUnitPrice = 0;
        var tempSubTotal = 0;
        var bestfitteddata = 0;

        Discount = CDec2($('#lblAdjustedGrossPercentage').val().replace('%', ''))

        $('#tblTransactionDetailsBody tr').each(function () {
            tempUnitPrice = CDec2($(this).find('.unitprice').val()) - (CDec2($(this).find('.unitprice').val()) * (1 - (Discount / 100)));
            tempSubTotal = tempUnitPrice * CDec2($(this).find('.qty').val());

            $(this).find('.unitprice').val(tempUnitPrice);
            $('#txtSubTotal' + $(this).data('data').ID).text(tempSubTotal);

            bestfitteddata += CDec2($('#txtSubTotal' + $(this).data('data').ID).text())
        });

        $('#lblActualTotalBestFitData').val(AccountingFormat(bestfitteddata));
    }
    

    function GetZReadData(data) {

        AjaxSend({
            url: 'Zapatero/GetZRead',
            data: data,
            success: function (vData) {

                var vRows = vData.ReportPOS.Rows;
                var tBody = $('#tblZReadBody');
                var totalGross = 0;
                var HasReported = false;

                tBody.empty();

                if (vRows.length > 0) {

                    var PreviousGrandTotal = CDec2($('#txtPreviousAccumulatedTotal').val());
                    var TargetGrandTotal = CDec2($('#txtCurTargetAccumulatedTotal').val());

                    var TargetSales = TargetGrandTotal - PreviousGrandTotal;

                    var TargetSalesPerDay = TargetSales / vRows.length

                    var TotalTargetSalesEvery3Days = 0;

                    var AdjustedGrandTotal = PreviousGrandTotal;

                    var randomCounter = 1;
                    var TotalRandomPercentage = 0;
                    var RunningGrossSales = 0;

                    for (var i = 0; i <= vRows.length-1; i++) {
                        var GrossSales = 0;
                        var CashAmount = 0;
                        var CreditAmount = 0;
                        var CurEndingAccumulatedTotal = 0;
                        var CurTargetAccumulatedTotal = 0;
                        var RandomPercentage = 0;

                        GrossSales = TargetSalesPerDay;

                        CashAmount = GrossSales;
                            
                        TotalTargetSalesEvery3Days = TargetSalesPerDay * 3;
                                               
                        
                        //if ((i + 1) % 3) {
                        //    //change target sales every 3 days
                            
                        //}
                        
                        if (randomCounter == 1) {
                            //random percetage of sales (20% - 40%)

                            RandomPercentage = Math.floor(Math.random() * (40 - 20) + 20);

                            if (i == vRows.length - 1) {
                                GrossSales = TargetGrandTotal - AdjustedGrandTotal;
                                //GrossSales = TargetSales - AdjustedGrandTotal;
                            } else {
                                GrossSales = TotalTargetSalesEvery3Days * (RandomPercentage / 100);
                            }                            

                            CashAmount = GrossSales * ((100 - RandomPercentage) / 100);
                            CreditAmount = GrossSales * (RandomPercentage / 100);

                            TotalRandomPercentage += RandomPercentage;

                            randomCounter += 1;

                        } else if (randomCounter == 2) {
                            //random percetage of sales (20% - 40%)

                            RandomPercentage = Math.floor(Math.random() * (40 - 20) + 20);
                            
                            if (RandomPercentage == TotalRandomPercentage) {
                                //if same random percentage as randomCounter == 1, add another 2-5% random
                                RandomPercentage += Math.floor(Math.random() * (5 - 2) + 2);
                            }

                            if (i == vRows.length - 1) {
                                GrossSales = TargetGrandTotal - AdjustedGrandTotal;
                            } else {
                                GrossSales = TotalTargetSalesEvery3Days * (RandomPercentage / 100);
                            }                            

                            CashAmount = GrossSales * ((100 - RandomPercentage) / 100);
                            CreditAmount = GrossSales * (RandomPercentage / 100);

                            TotalRandomPercentage += RandomPercentage;

                            randomCounter += 1;
                        }
                        else {                            
                            RandomPercentage = 100 - TotalRandomPercentage;

                            GrossSales = TotalTargetSalesEvery3Days * (RandomPercentage / 100);

                            CashAmount = GrossSales * ((100 - RandomPercentage) / 100);
                            CreditAmount = GrossSales * (RandomPercentage / 100);

                            //reset random variables
                            randomCounter = 1;
                            TotalRandomPercentage = 0;
                        }
                        
                        //RunningGrossSales += GrossSales;

                        if (i == vRows.length) {
                            //last day of the month
                            AdjustedGrandTotal = TargetGrandTotal;
                        } else {
                            AdjustedGrandTotal = AdjustedGrandTotal += GrossSales;
                        }

                        //var AdjustedGrandTotal = 0;
                        
                       
                        ////if (i == 0) {

                        ////    //end of z-read. need to adjust for declared esales

                        ////    CurEndingAccumulatedTotal = CDec2(vRows[i].PrevGrossSales);
                        ////    CurTargetAccumulatedTotal = CDec2( $('#txtCurTargetAccumulatedTotal').val());

                        ////    GrossSales = CurTargetAccumulatedTotal - CurEndingAccumulatedTotal;

                        ////    CashAmount = GrossSales - CDec2(vRows[i].CashAmount) - (CDec2(vRows[i].CreditCardAmount) + CDec2(vRows[i].DebitCardAmount)); //additional Cash to tally

                        ////    CashAmount = CashAmount + CDec2(vRows[i].CashAmount)
                            
                        ////    AdjustedGrandTotal = CDec2($('#txtCurTargetAccumulatedTotal').val());

                        ////} else if (i == vRows.length-1) {
                        ////    //first day of the month
                        ////    //need to adjust for previous accumulated sales

                        ////    GrossSales = (CDec2(vRows[i].PrevGrossSales) + CDec2(vRows[i].CurGrossSales)) - CDec2($('#txtPreviousAccumulatedTotal').val());

                        ////    CashAmount = GrossSales - CDec2(vRows[i].CashAmount) - (CDec2(vRows[i].CreditCardAmount) + CDec2(vRows[i].DebitCardAmount));

                        ////    CashAmount = CashAmount + CDec2(vRows[i].CashAmount)

                        ////    AdjustedGrandTotal = CDec2($('#txtPreviousAccumulatedTotal').val()) + GrossSales;

                        ////} else {
                        ////    //need to adjust cash sales incase cash sales + credit sales is not equal to gross sales

                        ////    GrossSales = CDec2(vRows[i].CurGrossSales);
                        ////    AdjustedGrandTotal = (CDec2(vRows[i].PrevGrossSales) + CDec2(vRows[i].CurGrossSales))

                        ////    var PrevAccuPlusCashPlushCreditCard = 0;

                        ////    PrevAccuPlusCashPlushCreditCard = CDec2(vRows[i].PrevGrossSales) + CDec2(vRows[i].CashAmount) + (CDec2(vRows[i].CreditCardAmount) + CDec2(vRows[i].DebitCardAmount));

                        ////    if (PrevAccuPlusCashPlushCreditCard - CDec2(vRows[i].CurGrossSales) != 0) {
                        ////        //cashamount = cashamount + (CurGross - Cash - Credit)
                        ////        CashAmount = CDec2(vRows[i].CashAmount) + (CDec2(vRows[i].CurGrossSales) - (CDec2(vRows[i].CashAmount) + (CDec2(vRows[i].CreditCardAmount) + CDec2(vRows[i].DebitCardAmount))));
                        ////    }else{
                        ////        CashAmount = CDec2(vRows[i].CashAmount);
                        ////    }
                            
                        ////}

                        tBody
                            .append($('<tr>')
                                .data('data', vRows[i])
                                .append($('<td>')
                                    .text($.format.date(new Date(vRows[i].SalesDate), 'MM/dd/yyyy'))
                                )
                                .append($('<td style=text-align:right>')
                                    .text(vRows[i].PrevGrossSales)
                                )
                                .append($('<td style=text-align:right>')
                                    .text(vRows[i].PresentationPrevGrossSales)
                                )
                                .append($('<td id=' + vRows[i].ID + '>')
                                    .html('<input id=AdjustedGT' + vRows[i].ID + ' type=text value=' + AdjustedGrandTotal + '>')
                                    //.click(function () {
                                    //    UpdateZReadReportedDay($(this).attr('id'), $('#chk' + $(this).attr('id')).prop('checked'));
                                    //})
                                )
                                .append($('<td style=text-align:right>')
                                    .text(vRows[i].CurGrossSales)
                                )                               
                                .append($('<td style=text-align:right>')
                                    .text(vRows[i].PresentationCurrGrossSales)
                                )
                                .append($('<td id=' + vRows[i].ID + '>')
                                    .html('<input id=AdjustedCurrGross' + vRows[i].ID + ' type=text value=' + GrossSales + '>')
                                    //.click(function () {
                                    //    UpdateZReadReportedDay($(this).attr('id'), $('#chk' + $(this).attr('id')).prop('checked'));
                                    //})
                                )
                                .append($('<td>')
                                    .html('<input id=CashSales' + vRows[i].ID + ' type=text value=' + CashAmount + '>')
                                )
                                .append($('<td>')
                                    .html('<input id=CreditSales' + vRows[i].ID + ' type=text value=' + CreditAmount + '>')
                                    //.html('<input id=CreditSales' + vRows[i].ID + ' type=text value=' + (CDec2(vRows[i].CreditCardAmount) + CDec2(vRows[i].DebitCardAmount)) + '>')
                                )
                                .append($('<td>')
                                    .text(vRows[i].CurTotalDiscount)
                                )
                                .append($('<td>')
                                    .text(vRows[i].CurSalesReturnRefund)
                                )
                                .append($('<td style=text-align:right>')
                                    .text(vRows[i].CurSalesReturnExchange)
                                )
                                .append($('<td>')
                                    .html('<input id=BegInvoice' + vRows[i].ID + ' type=text value=' + vRows[i].CurBegInvoiceNo + '>')
                                    //.text(vRows[i].CurBegInvoiceNo)
                                )
                                .append($('<td style=text-align:right>')
                                    .html('<input id=EndInvoice' + vRows[i].ID + ' type=text value=' + vRows[i].CurEndInvoiceNo + '>')
                                    //.text(vRows[i].CurEndInvoiceNo)
                                )
                                .append($('<td id=' + vRows[i].ID + '>')
                                    .html('<input ' + (HasReported ? 'disabled' : '') + ' id=chk' + vRows[i].ID + ' type=checkbox value=' + CInt2(vRows[i].IsReported) + '>')
                                    .click(function () {                                        
                                        UpdateZReadReportedDay($(this).attr('id'), $('#chk' + $(this).attr('id')).prop('checked'));
                                    })
                                )
                                .click(function () {
                                })
                            )

                        if (CBool(vRows[i].IsReported)) {
                            $('#chk' + vRows[i].ID).prop('checked', true);
                        }

                        if (CInt2(vRows[i].IsReported) == 1) {
                            HasReported = true;
                        }
                    }
                }

                //$('#lblAdjustedGross').val(AccountingFormat(totalGross));

                //if (CBool(IsFromESales)) {
                //    if ($('#lblOriginalGross').val() == '') {
                //        $('#lblOriginalGross').val(AccountingFormat(totalGross));
                //    }

                //    //$('#lblDifference').val('Margin(%)' + AccountingFormat((1 - (totalGross / CDec2($('#lblOriginalGross').val()))) * 100))
                //    $('#lblActualTotalBestFitData').val(AccountingFormat(CDec2(totalGross)))
                //} else {
                //    $('#lblOriginalGross').val(AccountingFormat(totalGross));
                //}

            }
        })
    }

    var dataTableMonthlyZRead;

    function GetMonthlyZReadData(data) {

        AjaxSend({
            url: 'ExternalServices/GetMonthlyZReadTotals',
            data: data,
            success: function (vData) {

                var vRows = vData.ReportPOS.Rows;

                if ($.fn.DataTable.isDataTable($('#tblMonthlyZRead'))) {
                    $('#tblMonthlyZRead').empty();
                    dataTableMonthlyZRead.destroy();
                }

                var buttons = [];
                if (!IsMobile) {
                    buttons.push({ extend: 'csvHtml5', text: 'Download CSV', title: 'Monthly ZRead' });
                }

                dataTableMonthlyZRead = $('#tblMonthlyZRead').DataTable({
                    "data": vRows,
                    "columns": [
                        { "data": "Month" },
                        { "data": "Year" },
                        { "data": "GrossSales" },
                        { "data": "TotalDiscount" },
                        { "data": "TotalExchange" },
                        { "data": "StartingInvoice" },
                        { "data": "EndingInvoice" }
                    ],
                    dom: 'Brtip',
                    buttons: buttons
                });
            }
        });
    }

    function SaveZReadData() {
        var ZReadID = [];

        $('#tblTransactionDetailsBody tr').each(function () {
            ZReadID.push({
                ID: $(this).data('data').ID,
            })
        });
    }

    function UpdateZReadReportedDay(ID, IsReported) {

        var data = {
            ID: ID,
            IsReported: IsReported
        };

        AjaxSend({
            url: 'Zapatero/UpdateZReadReportedDay',
            data: data,
            success: function (vData) {
                var mdata = {
                    OrgID: sessionStorage.OrgID,
                    LocationID: sessionStorage.LocationID,
                    DeviceID: sessionStorage.DeviceID,
                    DateFrom: DateFrom,
                    DateTo: DateTo
                }
                GetTransactionDetails(false,false);
                GetZReadData(mdata);
            }
        })
    }

    function SaveTransactionData() {
        var transData = [];
        
        var pData = {};

        var tempTotal = 0;
        var mCount = 0;
        var mTransaCount = 0;

        //$('#tblTransactionDetailsBody tr').each(function () {
        //    mTransaCount++;
        //});

        $('#tblTransactionDetailsBody tr').each(function () {
            //if (CInt2($(this).val()) > 0) {
            mTransaCount++;
                tempTotal += $('#txtSubTotal' + $(this).data('data').ID).val();

                var ToTalPayment = CDec2($(this).data('data').CashAmount) + 
                                    CDec2($(this).data('data').CheckPaymentAmount) + 
                                    CDec2($(this).data('data').DebitCardAmount) + 
                                    CDec2($(this).data('data').CreditCardAmount) + 
                                    CDec2($(this).data('data').SRVAmount) + 
                                    CDec2($(this).data('data').OnCreditAmount) + 
                                    CDec2($(this).data('data').OtherTypeAmount);

                transData.push({
                    ID: $(this).data('data').ID,
                    ProductNo: $(this).data('data').ProductNo,
                    ProductName: $(this).data('data').ProductName.substr(0, 75),
                    TransactionNo: $(this).find('.transactionno').val(), //$(this).data('data').TransactionNo,
                    TransactionDate : $(this).data('data').TransactionDate,
                    Customer: $(this).data('data').Customer,
                    OriginalQty: $(this).data('data').OriginalQuantity,
                    Qty : $(this).find('.qty').val(),
                    UnitMeasure: $(this).data('data').UnitMeasure,
                    OriginalUnitPrice: $(this).data('data').OriginalUnitPrice,
                    UnitPrice: $(this).find('.unitprice').val(),
                    SubTotal: $('#txtSubTotal' + $(this).data('data').ID).text(),
                    DiscountAmount : $(this).data('data').DiscountAmount,
                    DiscountedUnitPrice : $(this).data('data').DiscountedUnitPrice,
                    DiscountedSubTotal : $(this).data('data').DiscountedSubTotal,
                    CashierName : $(this).data('data').CashierName,
                    CashAmount : $(this).data('data').CashAmount,
                    CheckPaymentAmount : $(this).data('data').CheckPaymentAmount,
                    DebitCardAmount : $(this).data('data').DebitCardAmount,
                    CreditCardAmount : $(this).data('data').CreditCardAmount,
                    SRVAmount : $(this).data('data').SRVAmount,
                    OnCreditAmount : $(this).data('data').OnCreditAmount,
                    OtherTypeAmount : $(this).data('data').OtherTypeAmount,
                    VATPercent : $(this).data('data').VATPercent,
                    VatableSales : $('#txtSubTotal' + $(this).data('data').ID).text(), //compute this. it may change if they change the qty of the item
                    VATAmount: CInt2($(this).data('data').VATPercent.replace('%', '')) * CDec2($('#txtSubTotal' + $(this).data('data').ID).text()), //$(this).data('data').VATAmount;//compute this. it may change if they change the qty of the item
                    CustomerChange: ToTalPayment - CDec2($('#txtSubTotal' + $(this).data('data').ID).text()) //compute this. it may change if they change the qty of the item
                })

                mCount++;                

                //if (mCount == 1000) {
                    var data = {};

                    var maxDays = 0;

                    var now = new Date($('#selMonth').val() + '/1/' + $('#selYear').val());

                    maxDays = new Date(now.getFullYear(), now.getMonth() + 1, 0).getDate();

                    data.OrgID = sessionStorage.OrgID;
                    data.DeviceID = sessionStorage.DeviceID;
                    data.LocationID = sessionStorage.LocationID;
                    data.LocationName = sessionStorage.Location;
                    data.TransData = JSON.stringify(transData);
                    data.DateFrom = DateFrom;
                    data.DateTo = DateTo;
                    data.TransCount = mTransaCount;
                    
                    AjaxSend({
                        url: 'Zapatero/SaveTransactionData',
                        data: data,
                        timeout: 6000000,
                        success: function (vData) {
                            if (vData != null) {
                                //localhost:8080
                                //window.location = GetPathFormat('http://localhost:8080/ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + vData.ReportPOS);
                                //window.location = GetPathFormat('http://122.52.123.251/ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + vData.ReportPOS); 
                                //window.location = GetPathFormat('http://124.106.167.156:8083/ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + vData.ReportPOS);
                                var url = 'http://192.168.15.254:8083/ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + vData.ReportPOS;
                                //window.open(url,'_blank' );
                            }
                        }
                    })

                //    mCount = 0;
                //    transData = [];
                //}
            //}            
        })
        
        var data = {};

        var maxDays = 0;

        var now = new Date($('#selMonth').val() + '/1/' + $('#selYear').val());

        maxDays = new Date(now.getFullYear(), now.getMonth() + 1, 0).getDate();

        data.OrgID = sessionStorage.OrgID;
        data.DeviceID = sessionStorage.DeviceID;
        data.LocationID = sessionStorage.LocationID;
        data.LocationName = sessionStorage.Location;
        data.TransData = JSON.stringify(transData);
        data.DateFrom = DateFrom;
        data.DateTo = DateTo;
        data.TransCount = mTransaCount;
        data.IsLastBatch = 1;

        AjaxSend({
            url: 'Zapatero/SaveTransactionData',
            data: data,
            timeout: 600000,
            success: function (vData) {
                if (vData != null) {
                    //localhost:8080
                    //window.location = GetPathFormat('http://localhost:8080/ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + vData.ReportPOS);
                    window.location = GetPathFormat('http://122.52.123.251/ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + vData.ReportPOS); 
                    //window.location = GetPathFormat('http://124.106.167.156:8083/ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + vData.ReportPOS);
                    //var url = 'http://124.106.167.156:8083/ReportsPOS/DownloadReportFile?OrgID=' + sessionStorage.OrgID + '&ReportLink=' + vData.ReportPOS;
                }
            }
        })
    }
});

