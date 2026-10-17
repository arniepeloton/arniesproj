require([
    'common', 'tablesort',
    GetPathFormat('/Scripts3/Browser/LibraryPeople.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryOrganization.js?crc='),
    GetPathFormat('/Scripts3/model/CashHandling_offline.js?crc='),
    (OfflineMode ? '/Scripts3/Offline/Browser/LibraryFowst.js?crc=' : '')
], function () {

    var DepositReport = {};

    DepositReport = new App.model.CashHandling_offline();

    $(function () {

        $('document').ready(function () {
            $('#divAll').attachDragger();
            $('#divDetails').attachDragger();

            //stupid table js

            // Helper function to convert a string of the form "Mar 15, 1987" into
            // a Date object.
            var date_from_string = function (str) {
                var months = ["jan", "feb", "mar", "apr", "may", "jun", "jul",
                              "aug", "sep", "oct", "nov", "dec"];
                var pattern = "^([a-zA-Z]{3})\\s*(\\d{2}),\\s*(\\d{4})$";
                var re = new RegExp(pattern);
                var DateParts = re.exec(str).slice(1);

                var Year = DateParts[2];
                var Month = $.inArray(DateParts[0].toLowerCase(), months);
                var Day = DateParts[1];
                return new Date(Year, Month, Day);
            }

            var moveBlanks = function (a, b) {

                if (a < b) {
                    if (a == "")
                        return 1;
                    else
                        return -1;
                }
                if (a > b) {
                    if (b == "")
                        return -1;
                    else
                        return 1;
                }
                return 0;
            };
            var moveBlanksDesc = function (a, b) {
                // Blanks are by definition the smallest value, so we don't have to
                // worry about them here
                if (a < b)
                    return 1;
                if (a > b)
                    return -1;
                return 0;
            };

            var table = $("table").stupidtable({

                "date": function (a, b) {
                    // Get these into date objects for comparison.

                    aDate = new Date(a);
                    bDate = new Date(b);

                    return aDate - bDate;
                },
                "moveBlanks": moveBlanks,
                "moveBlanksDesc": moveBlanksDesc
            });

            table.on("beforetablesort", function (event, data) {
                // data.column - the index of the column sorted after a click
                // data.direction - the sorting direction (either asc or desc)
                $("#msg").text("Sorting index " + data.column)
            });

            table.on("aftertablesort", function (event, data) {

                var th = $(this).find("th");
                th.find(".arrow").remove();
                var dir = $.fn.stupidtable.dir;

                var arrow = data.direction === dir.ASC ? "&uarr;" : "&darr;";
                th.eq(data.column).append('<span class="arrow">' + arrow + '</span>');

            });

            $("tr").slice(1).click(function () {

                $(".awesome").removeClass("awesome");
                $(this).addClass("awesome");
            });

            $('#txtDateFrom, #txtDateTo').text($.format.date(new Date(), 'MMM dd, yyyy'))
            //end of stupid table

            $('body').on('click', '#btnReceivingTransferDateRange', function () {
                $('#panReceivingTransferDateRange').popup('open');
            })
            .on('click', '.btnPeriod', function () {
                PopupPeriod({
                    Click: function (e) {
                        $('#txtDateFrom').text(e.From);
                        $('#txtDateTo').text(e.To);
                        if (e.Period == '2 days ago') {
                            $('#li2DaysAgo').click();
                        } else if (e.Period == 'Yesterday') {
                            $('#liYesterday').click();
                        } else if (e.Period == 'Today') {
                            $('#liToday').click();
                        } else if (e.Period == 'Week to Date') {
                            $('#liWeekToDate').click();
                        } else if (e.Period == 'Month to Date') {
                            $('#liMonthToDate').click();
                        } else if (e.Period == 'Custom') {
                            m_Period = 'CustomPeriod';
                            sessionStorage.Period = m_Period;                            
                            $('.lblPeriod').text(e.From + ' - ' + e.To);
                            GetDepositReport();
                        }
                    }
                })
            })
            .on('click', '.btnSelectFowst', function (e) {
                e.preventDefault();
                $('#panHeaderListOfGroupFowst').popup('open');
            })
            .on('click', '#btnReceivingTransferDone', function (e) {
                e.preventDefault();
                var dateFrom = new Date($('#txtReceivingTransferDateFrom').text());
                var dateTo = new Date($('#txtReceivingTransferDateTo').text());
                var dateNow = new Date();

                if (!IsDate(dateFrom) || !IsDate(dateTo)) {
                    return;
                }

                if (dateFrom > dateTo || dateFrom > dateNow || dateTo > dateNow) {
                    $('#spanDateRangeError').show();
                }
                else {
                    $('#spanDateRangeError').hide();
                    $('#btnReceivingTransferDateRange').removeClass('btnRequired').addClass('btnNotRequired').attr('title', $('#txtReceivingTransferDateFrom').text() + ' to ' + $('#txtReceivingTransferDateTo').text())
                    $('#DateRangeFrom').text($('#txtReceivingTransferDateFrom').text());
                    $('#DateRangeTo').text($('#txtReceivingTransferDateTo').text());
                    $('#DateRangeSeparator').show();
                    $('#panReceivingTransferDateRange').popup('close');

                    if ($('#txtReceivingTransferFowstID').val() != "") {
                        //sessionStorage.SelectedFOWST
                        GetDepositReport();
                    }
                }
            })
            .on('click', '#btnReceivingTransferCancel', function (e) {
                e.preventDefault();
                $('#panReceivingTransferDateRange').popup('close');
            })
            .on("mouseenter", 'td', function (event) {
                $(this).find("span.tooltip").css({
                    top: event.pageY + 5 + "px",
                    left: event.pageX + 5 + "px"
                }).show();
            })
            .on("mouseleave", 'td', function () {
                $("span.tooltip").hide();
            })
            .on("click", ".lblSelectedEmployeeOrEntity", function () {
                BrowsePopup('LibraryOrganizationEmployee', 'Employee', [
                {
                    DisplayText: [
                        { Column: 'PeopleName' }
                    ],
                    Click: function (vData) {
                        sessionStorage.SelectedEmployeeID = vData.ID;
                        $('.lblSelectedEmployeeOrEntity').text(vData.PeopleName);
                        GetDepositReport();

                        if ($.trim($('.lblPeriod:first').text()) == "2 Days Ago") {
                            $('#li2DaysAgo').click();
                        }
                        else if ($.trim($('.lblPeriod:first').text()) == "Yesterday") {
                            $('#liYesterday').click();
                        }
                        else if ($.trim($('.lblPeriod:first').text()) == "Today") {
                            $('#liToday').click();
                        }
                        else if ($.trim($('.lblPeriod:first').text()) == "Week To Date") {
                            $('#liWeekToDate').click();
                        }
                        else if ($.trim($('.lblPeriod:first').text()) == "Month To Date") {
                            $('#liMonthToDate').click();
                        }
                        else {
                            $('#btnDone').click();
                        }
                    }
                }
                ]);
            })
            .on('click', '#btnTurnOverFilter', function () {
                $('#panTurnOverFilter').popup('open');
            })
            .on('click', '#btnMonieAccFOWST', function () {
                var options = [
                    {
                        DisplayText: [{ Column: 'FowstName' }],
                        AutoSearch: true,
                        Click: function (e) {
                            sessionStorage.SelectedFOWST = e.ID;
                            sessionStorage.SelectedFOWSTName = e.FowstName;
                            GetDepositReport();
                        }
                    }
                ]

                if(OfflineMode) {
                    options[0].Offline = true;
                }

                BrowsePopup('LibraryFowst', 'Select FOWST', options);
            })
        });

    });

    window.GetDepositReport = function () {
        var pData = {};

        //pData.DateFrom = $('#DateRangeFrom').text();
        //pData.DateTo = $('#DateRangeTo').text();        

        if (sessionStorage.SelectedFOWST == undefined || sessionStorage.SelectedFOWST == '') {
            $('#btnMonieAccFOWST').click();
            return;
        }

        $('.lblSelectedFowstGroup').text(sessionStorage.SelectedFOWSTName);
        $('.btnSelectFowst').removeClass('btnRequired').addClass('btnNotRequired');

        if (sessionStorage.ReportType == 'MoniesAccountability') {
            $('#divDateDepositReport').hide();
        }
        else {
            $('#divDateDepositReport').show();
        }

        var nDate = new Date();
        var nMonth = nDate.getMonth() + 1;

        pData.DateFrom = ($('#txtDateFrom').text() || nDate.getFullYear() + '-' + (nMonth < 10 ? '0' + nMonth : nMonth) + '-' + nDate.getDate());
        pData.DateTo = ($('#txtDateTo').text() || nDate.getFullYear() + '-' + (nMonth < 10 ? '0' + nMonth : nMonth) + '-' + nDate.getDate());
        pData.FowstID = sessionStorage.SelectedFOWST;
        pData.EmployeeID = sessionStorage.SelectedEmployeeID;
        pData.FowstVendorID = $('#txtReceivingTransferFowstID').val(); //$('#selFowst2 option:selected').text()

        if (OfflineMode) {

            var vData;

            vData = DepositReport.GetAllDetails(pData, 1);

            //vData = ProcessCashHandling(vData, 1);

            if (vData.length == 0) {
                $('div[id="divNoPage"]').css('visibility', 'visible');
                $('div[id="divNoPage"]').css('display', 'block');
                $('div[id="divMoniesAcc2"]').hide();
                return;
            }

            $('div[id="divMoniesAcc2"]').show();

            if (vData[0].Rows != undefined) {
                if (vData[0].Rows.length > 0) {
                    $('div[id="divNoPage"]').css('visibility', 'hidden');
                    $('div[id="divNoPage"]').css('display', 'none');
                    $('div[id="dvDepositReportTable"]').css('visibility', 'visible');
                    $('div[id="dvDepositReportTable"]').css('display', 'block');
                    DisplayDeposits2(vData[0].Rows);
                    //DisplayDeposits2(vData.Rows);
                } else {
                    $('div[id="divNoPage"]').css('visibility', 'visible');
                    $('div[id="divNoPage"]').css('display', 'block');
                    $('div[id="dvDepositReportTable"]').css('visibility', 'hidden');
                    $('div[id="dvDepositReportTable"]').css('display', 'none');
                }
            }

        } else {
            pData.isHeaderDetails = true;
            AjaxSend({
                url: 'CashHandling/GetDeposits',
                data: pData,
                success: function (vData) {
                    if (vData.Results.Rows.length > 0) {
                        $('div[id="divNoPage"]').css('visibility', 'hidden');
                        $('div[id="divNoPage"]').css('display', 'none');
                        $('div[id="dvDepositReportTable"]').css('visibility', 'visible');
                        $('div[id="dvDepositReportTable"]').css('display', 'block');
                        $('div[id="divTurnOverDetails"]').hide();
                        //DisplayDeposits(vData.Results.Rows);
                        vData = DepositReport.ProcessCashHandling(pData, vData.Results, 1);
                        DisplayDeposits2(vData[0].Rows);
                    } else {
                        //vData = ProcessCashHandling(vData.Results, 1);
                        $('div[id="divNoPage"]').css('visibility', 'visible');
                        $('div[id="divNoPage"]').css('display', 'block');
                        $('div[id="dvDepositReportTable"]').css('visibility', 'hidden');
                        $('div[id="dvDepositReportTable"]').css('display', 'none');
                        $('div[id="divTurnOverDetails"]').hide();
                    }
                }
            })
        }
    }

    $('.lblSelectedFowstGroup').text(sessionStorage.SelectedFOWSTName);
    GetDepositReport();

    function DisplayDeposits2(vRows) {
        var pLV = $('tbody[id="tBodyTurnOver"]');
        var pData = {};

        pData.DateFrom = $('#txtDateFrom').text();
        pData.DateTo = $('#txtDateTo').text();
        pData.FowstID = sessionStorage.SelectedFOWST;
        pData.EmployeeID = sessionStorage.SelectedEmployeeID;
        pData.FowstVendorID = $('#txtReceivingTransferFowstID').val();

        pLV.empty();

        for (var i = 0; i <= vRows.length - 1; i++) {
            var totalSales = (CDec2(vRows[i].SalesAmount) + CDec2(vRows[i].TotalAcceptedFromPrevTurnOver)) - CDec2(vRows[i].DiscountAmount) - CDec2(vRows[i].VatExemptTax) - CDec2(vRows[i].TotalReturns);
            var TotalMoniesRemainingOnHand = (CDec2(vRows[i].TotalAmount) - CDec2(totalSales) + CDec2(vRows[i].ServiceCharge));

            var ctr = pLV.find('tr[id="' + vRows[i].PeopleID + '"]').length;
            var ind = pLV.find('tr[id="' + vRows[i].PeopleID + '"]').index();
            var depositedMonies = 0, turnedoverMonies = 0;

            //if (CBool(vRows[i].IsTurnOver)) {
            turnedoverMonies = CDec2(vRows[i].TotalTurnOver);
            //}
            //else {
            depositedMonies = CDec2(vRows[i].TotalDeposit);
            //}

            var availableMonies = AccountingFormat(depositedMonies + CDec2(vRows[i].ServiceCharge));

            if (ctr == 0) {
                pItem = $('<tr>')
                        .attr('id', vRows[i].PeopleID)
                        .append($('<td>')
                            .text(vRows[i].Cashier)
                        )
                //                        .append($('<td>')
                //                            .text('')
                //                        )
                //                        .append($('<td>')
                //                            .text(vRows[i].FowstName)
                //                        )
                        .append($('<td style="text-align:right">')
                            .text(AccountingFormat(totalSales))
                            .append($('<span>')
                                .attr('class', 'netsales_' + vRows[i].PeopleID)
                                .css('display', 'none')
                                .text(AccountingFormat(totalSales))
                            )
                        )
                        .append($('<td style="text-align:right">')
                            .text(AccountingFormat(availableMonies))
                            .append($('<span>')
                                .attr('class', 'available_' + vRows[i].PeopleID)
                                .css('display', 'none')
                                .text(AccountingFormat(availableMonies))
                            )
                        )
                        .append($('<td style="text-align:right">')
                            .text(AccountingFormat(turnedoverMonies))
                        )
                        .append($('<td style="text-align:right">')
                            .text(AccountingFormat((CDec2(availableMonies) + CDec2(turnedoverMonies)) - CDec2(totalSales)))
                        )
                        .css('cursor', 'pointer')
                        .data('data', vRows[i])
                        .click(function (e) {
                            var pData = {};
                            var nDate = new Date();
                            var nMonth = nDate.getMonth() + 1;

                            pData.DateFrom = ($('#txtDateFrom').text() || nDate.getFullYear() + '-' + (nMonth < 10 ? '0' + nMonth : nMonth) + '-' + nDate.getDate());
                            pData.DateTo = ($('#txtDateTo').text() || nDate.getFullYear() + '-' + (nMonth < 10 ? '0' + nMonth : nMonth) + '-' + nDate.getDate());
                            pData.FowstID = sessionStorage.SelectedFOWST;

                            if (OfflineMode) {
                                pData.PeopleID = $(this).attr('id');
                                pData.FowstVendorID = $('#txtReceivingTransferFowstID').val();
                                $('div[id="divTurnOverDetails"]').show();
                                var moniesData = DepositReport.GetAllDetails(pData, 0);
                                //moniesData = DepositReport.ProcessCashHandling(pData, moniesData, 0);
                                DisplayAllDetails(moniesData[0], moniesData[1].Rows, moniesData[2].Rows);
                                DisplayTurnover(moniesData[1].Rows);
                                DisplayDeposit(moniesData[2].Rows);
                            }
                            else {
                                pData.IsGetDetails = true;
                                pData.PeopleID = $(this).data('data').PeopleID;
                                pData.isHeaderDetails = false;
                                GetDepositsDetails(pData);
                            }
                        })
                pLV.append(pItem);
            }
            else {
                var pRow = pLV.find('tr:eq(' + ind + ')');
                var totaAvailableMonies = CDec2(pRow.find('td:eq(2)').text());

                if ($.inArray(AccountingFormat(totalSales), $('.netsales_' + vRows[i].PeopleID).text().replace('"', '').split('|')) == -1) {
                    var prevElem = pRow.find('td:eq(1)').children();
                    pRow.find('td:eq(1)').text(AccountingFormat(CDec2(pRow.find('td:eq(1)').contents().get(0).nodeValue) + CDec2(totalSales))).append(prevElem);
                    $('.netsales_' + vRows[i].PeopleID).append('|' + AccountingFormat(totalSales));
                }

                if ($.inArray(AccountingFormat(availableMonies), $('.available_' + vRows[i].PeopleID).text().replace('"', '').split('|')) == -1) {
                    var prevElem = pRow.find('td:eq(2)').children();
                    pRow.find('td:eq(2)').text(AccountingFormat(CDec2(pRow.find('td:eq(2)').contents().get(0).nodeValue) + (depositedMonies + CDec2(vRows[i].ServiceCharge)))).append(prevElem);
                    $('.available_' + vRows[i].PeopleID).append('|' + AccountingFormat(availableMonies));
                }
                //pRow.find('td:eq(2)').text(AccountingFormat(CDec2(pRow.find('td:eq(2)').text()) - (depositedMonies + CDec2(vRows[i].ServiceCharge))));
                pRow.find('td:eq(3)').text(AccountingFormat(CDec2(pRow.find('td:eq(3)').text()) + turnedoverMonies));

                var variance = (CDec2(pRow.find('td:eq(2)').contents().get(0).nodeValue) + CDec2(pRow.find('td:eq(3)').text())) - CDec2(pRow.find('td:eq(1)').contents().get(0).nodeValue);

                pRow.find('td:eq(4)').text(AccountingFormat(variance));
            }

            //prevTotalSales = totalSales;
        }
        //pLV.parent().table('refresh');
    }

    function GetDepositsDetails(pData) {
        var mTurnOverData;
        var mData;
        var newpDT = new Array();

        $('#tBodyAllDetails').append($('<tr>')
            .append($('<td>')
                .attr('colspan', 12)
                .css('text-align', 'center')
                .text('Please wait ...')
            )
        )

        AjaxSend({
            url: 'CashHandling/GetDeposits',
            data: pData,
            success: function (vData) {
                $('div[id="divTurnOverDetails"]').show();
                mData = DepositReport.ProcessCashHandling(pData, vData.Results, 0);

                AjaxSend({
                    url: 'CashHandling/GetTurnover',
                    data: pData,
                    success: function (vData) {

                        mTurnOverData = vData.Results.Rows;

                        AjaxSend({
                            url: 'CashHandling/GetAllDeposits',
                            data: pData,
                            success: function (vData) {

                                DisplayAllDetails(mData, mTurnOverData, vData.Results.Rows);
                                DisplayTurnover(mTurnOverData);
                                DisplayDeposit(vData.Results.Rows);
                            }
                        });
                    }
                });
            }
        })
    }

    function DisplayDeposit(vRows) {
        var pLV = $('tbody[id="tBodyDepositDetails"]');

        pLV.empty();

        if (vRows.length > 0) {
            for (var i = 0; i < vRows.length; i++) {
                pLV.append($('<tr>')
                    .append($('<td>')
                        .text($.format.date(new Date(vRows[i].DepositDate), 'MMM dd, yyyy HH:mm'))
                    )
                    .append($('<td>')
                        .text(vRows[i].DepositedBy)
                    )
                    .append($('<td>')
                        .text(vRows[i].DepositedTo)
                    )
                    .append($('<td>')
                        .css('text-align', 'right')
                        .text(AccountingFormat(vRows[i].AmountDeposit))
                    )
                    .append($('<td>')
                        .css('text-align', 'right')
                        .text(AccountingFormat(CDec2(vRows[i].ServiceCharge)))
                    )
                    .append($('<td>')
                        .text(vRows[i].DepositReferenceNo)
                    )
                    .append($('<td>')
                        .text(vRows[i].BankName)
                    )
                    .append($('<td>')
                        .text(vRows[i].BankBranchName)
                    )
                )
            }
        }
        else {
            pLV.append($('<tr>')
                .append($('<td>')
                    .text('No Result(s) Found')
                    .css('text-align', 'center')
                    .attr('colspan', 10)
                )
            )
        }

        pLV.parent().table('refresh');
    }

    function DisplayTurnover(vRows) {
        var pLV = $('tbody[id="tBodyTurnOverDetails"]');

        pLV.empty();

        if (vRows.length > 0) {
            for (var i = 0; i < vRows.length; i++) {
                pLV.append($('<tr>')
                    .append($('<td>')
                        .text($.format.date(new Date(vRows[i].DateToTurnover), 'MMM dd, yyyy HH:mm'))
                    )
                    .append($('<td>')
                        .text(vRows[i].TurnoverBy)
                    )
                    .append($('<td>')
                        .text(vRows[i].TurnoverTo)
                    )
                    .append($('<td>')
                        .css('text-align', 'right')
                        .text(AccountingFormat(vRows[i].AmountTurnover))
                    )
                )
            }
        }
        else {
            pLV.append($('<tr>')
                .append($('<td>')
                    .text('No Result(s) Found')
                    .css('text-align', 'center')
                    .attr('colspan', 10)
                )
            )
        }

        pLV.parent().table('refresh');
    }

    function DisplayAllDetails(vRows, vRowsTurnOver, vRowsDeposits) {
        var pLV = $('#tBodyAllDetails');
        var lv = $('tbody[id="tbodyDetails"]');

        pLV.empty();
        lv.empty();

        if (vRows.length > 0) {
            $('#txtPersonName').text(vRows[0].Cashier);
            for (var i = 0; i < vRows.length; i++) {

                var rowInd = lv.find('tr[salestract="' + $.format.date(new Date(vRows[i].SalesTransactionDate), 'MMM dd, yyyy') + '"]').index();
                var thisRow = lv.find('tr:eq(' + rowInd + ')');

                //if (vRows[i].GeneratedSales != undefined) {
                var TotalGeneratedSalesAndAcceptedMonies = CDec2(vRows[i].GeneratedSales) + CDec2(vRows[i].AcceptedMonies);
                var TotalDisbursedAndTuronedOverMonies = 0 + CDec2(vRows[i].TurnedOverMonies);
                var TotalMoniesRemainingOnHand = (CDec2(vRows[i].GeneratedSales) + CDec2(vRows[i].AcceptedMonies)) - (0 + CDec2(vRows[i].TurnedOverMonies));
                var isTurnOver = '-';

                if (vRows[i].IsTurnOver != undefined) {
                    isTurnOver = (vRows[i].IsTurnOver == 1 ? "Turnover" : "Deposit");
                }

                if (lv.find('tr[pplid="' + vRows[i].PeopleID + '"]').length > 0 && $.format.date(new Date(thisRow.find('td:eq(0)').text()), 'MMM dd, yyyy') == $.format.date(new Date(vRows[i].SalesTransactionDate), 'MMM dd, yyyy')) {
                    //thisRow.find('td:eq(3)').text(AccountingFormat(CDec2(thisRow.find('td:eq(3)').text()) + CDec2(vRows[i].AcceptedMonies)))
                    thisRow.find('td:eq(0)').text(vRows[i].SalesTransactionDate)
                    //thisRow.find('td:eq(8)').text(AccountingFormat((CDec2(thisRow.find('td:eq(5)').text()) + CDec2(thisRow.find('td:eq(6)').text())) - CDec2(thisRow.find('td:eq(5)').text())))
                }
                else {
                    var mDate;
                    if (IsDate(vRows[i].SalesTransactionDate)) {
                        mDate = vRows[i].SalesTransactionDate;
                    } else {
                        mDate = vRows[i].DepositDate;
                    }
                    lv.append($('<tr>')
                            .attr('pplid', vRows[i].PeopleID)
                            .attr('salestract', $.format.date(new Date(mDate), 'MMM dd, yyyy'))
                            .append($('<td>')
                                .css('border-left', '1px solid #757575')
                                .text(mDate)
                            )
                            .append($('<td>')
                                .css('display', 'none')
                                .text(vRows[i].Cashier)
                            )
                            .append($('<td>')
                                .css('text-align', 'right')
                                .css('border-left', '1px solid #757575')
                                .text(AccountingFormat(CDec2(vRows[i].GeneratedSales)))
                            )
                            .append($('<td>')
                                .css('text-align', 'right')
                                .text(AccountingFormat(CDec2(vRows[i].DiscountAmount)))
                            )
                            .append($('<td>')
                                .css('text-align', 'right')
                                .text(AccountingFormat(CDec2(vRows[i].VatExemptTax)))
                            )
                            .append($('<td>')
                                .css('text-align', 'right')
                                .text(AccountingFormat(CDec2(vRows[i].TotalReturns)))
                            )
                            .append($('<td>')
                                .css('text-align', 'right')
                                .text(AccountingFormat(CDec2(vRows[i].GeneratedSales) - (CDec2(vRows[i].DiscountAmount) + CDec2(vRows[i].VatExemptTax) + CDec2(vRows[i].TotalReturns))))
                            )
                            .append($('<td>')
                                .css('text-align', 'right')
                                .text(AccountingFormat(vRows[i].AcceptedMonies))
                            )
                            .append($('<td>')
                                .text(vRows[i].AcceptedMoniesFrom)
                            )
                            .append($('<td>')
                                .css('text-align', 'right')
                                .text(AccountingFormat((CDec2(vRows[i].GeneratedSales) + CDec2(vRows[i].AcceptedMonies)) - CDec2(vRows[i].DiscountAmount) - CDec2(vRows[i].VatExemptTax) - CDec2((vRows[i].TotalReturns))))
                            )
                            .append($('<td>')
                                .css('text-align', 'right')
                                .css('border-left', '1px solid #757575')
                                .text('-')
                            )
                            .append($('<td>')
                                .css('text-align', 'right')
                                .text('-')
                            )
                            .append($('<td>')
                                .css('text-align', 'right')
                                .text('-')
                            )
                            .append($('<td>')
                                .css('text-align', 'right')
                                .text('-')
                            )
                            .append($('<td>')
                                .css('text-align', 'right')
                                .text('-')
                            )
                            .append($('<td>')
                                .attr('pplid', vRows[i].PeopleID)
                                .data('data', vRows[i])
                                .text('')
                                .click(function (e) {
                                    e.stopPropagation();

                                    if (CBool($(this).attr('ismulti')) == false) {
                                        return false;
                                    }

                                    var pData = {};
                                    pData.FowstID = sessionStorage.SelectedFOWST;
                                    pData.PeopleID = $(this).attr('pplid');

                                    var vData = DepositReport.GetTurnover(pData, new Date($('#txtDateFrom').text()), new Date($('#txtDateTo').text()));
                                    var details = '';

                                    for (var i = 0; i < vData.Rows.length; i++) {
                                        if (i == 0) {
                                            details = '<div style="float:left">' + vData.Rows[i].TurnoverTo + '</div> <div style="float:right">' + vData.Rows[i].AmountTurnover + '</div> <div style="clear:both"></div>';
                                        }
                                        else {
                                            details += '<br> <div style="float:left">' + vData.Rows[i].TurnoverTo + '</div> <div style="float:right">' + vData.Rows[i].AmountTurnover + '</div> <div style="clear:both"></div>';
                                        }
                                    }

                                    NewMessageBox(details, 'Turnedover Details');
                                })
                            )
                            .append($('<td>')
                                .css('text-align', 'right')
                                .text('-')
                            )
                        )
                }
                //}
            }
        }

        if (vRowsDeposits.length > 0) {
            for (var d = 0; d < vRowsDeposits.length; d++) {
                var rowInd = lv.find('tr[salestract="' + $.format.date(new Date(vRowsDeposits[d].DepositDate), 'MMM dd, yyyy') + '"]').index();
                var thisRow = lv.find('tr:eq(' + rowInd + ')');

                thisRow.find('td:eq(10)').text(AccountingFormat(CDec2(thisRow.find('td:eq(10)').text()) + CDec2(vRowsDeposits[d].AmountDeposit)));
                thisRow.find('td:eq(11)').text(AccountingFormat(CDec2(thisRow.find('td:eq(11)').text()) + CDec2(vRowsDeposits[d].ServiceCharge)));
                thisRow.find('td:eq(12)').text(AccountingFormat(CDec2(thisRow.find('td:eq(10)').text()) + CDec2(thisRow.find('td:eq(11)').text())));
            }
        }

        if (vRowsTurnOver.length > 0) {
            var isMultiLoadFinished = false;
            for (var t = 0; t < vRowsTurnOver.length; t++) {
                var rowInd = lv.find('tr[salestract="' + $.format.date(new Date(vRowsTurnOver[t].DateToTurnover), 'MMM dd, yyyy') + '"]').index();
                var thisRow = lv.find('tr:eq(' + rowInd + ')');
                var isMultipleTurnedover = false;

                if (vRowsTurnOver[t].TurnoverTo == thisRow.find('td:eq(15)').text()) {
                    isMultipleTurnedover = true;
                }

                thisRow.find('td:eq(14)').css('border-left', '1px solid #757575').text(AccountingFormat(CDec2(thisRow.find('td:eq(14)').text()) + CDec2(vRowsTurnOver[t].AmountTurnover)));
                if (isMultiLoadFinished == false) {
                    thisRow.find('td:eq(15)')
                    .attr('ismulti', isMultipleTurnedover)
                    .append((isMultipleTurnedover == true ? 'Multiple Person <br> <a class="custom-link" data-role="none">View Details</a>' : vRowsTurnOver[t].TurnoverTo));
                }
                thisRow.find('td:eq(13)').text(AccountingFormat(CDec2(thisRow.find('td:eq(9)').text()) - CDec2(thisRow.find('td:eq(12)').text())));
                thisRow.find('td:eq(16)').css('border-left', '1px solid #757575').text(AccountingFormat(CDec2(thisRow.find('td:eq(14)').text()) - CDec2(thisRow.find('td:eq(13)').text())));

                isMultiLoadFinished = true;
            }

            $('#tbodyDetails tr').each(function (i) {

                $(this).find('td:eq(13)').text(AccountingFormat(CDec2($(this).find('td:eq(9)').find('b').remove('b').end().text()) - CDec2($(this).find('td:eq(12)').text())));

                if ($(this).find('td:eq(13)').text() == '-' && $(this).find('td:eq(14)').text() == '-' && $(this).find('td:eq(12)').text() == '-') {
                    //if with sales with no turnover
                    $(this).find('td:eq(16)').text($(this).find('td:eq(9)').find('b').remove('b').end().text());
                } else {
                    //if with sales and turnover
                    $(this).find('td:eq(16)').text(AccountingFormat(CDec2($(this).find('td:eq(14)').text()) - CDec2($(this).find('td:eq(13)').text())));
                }
            })
        }
        else {
            $('#tbodyDetails tr').each(function (i) {

                $(this).find('td:eq(13)').text(AccountingFormat(CDec2($(this).find('td:eq(9)').find('b').remove('b').end().text()) - CDec2($(this).find('td:eq(12)').text())));

                if ($(this).find('td:eq(13)').text() == '-' && $(this).find('td:eq(14)').text() == '-' && $(this).find('td:eq(12)').text() == '-') {
                    //if with sales with no turnover
                    $(this).find('td:eq(16)').text($(this).find('td:eq(9)').find('b').remove('b').end().text());
                } else {
                    //if with sales and turnover
                    $(this).find('td:eq(16)').text(AccountingFormat(CDec2($(this).find('td:eq(14)').text()) - CDec2($(this).find('td:eq(13)').text())));
                }
            })
        }        

        //lv.parent().table('refresh');
    }

    function DisplayDeposits(vRows) {
        var pLV = $('#tblBodyDepositTable');

        pLV.empty();

        for (var i = 0; i <= vRows.length - 1; i++) {
            pItem = $('<tr>')
                        .append($('<td>')
                            .text($.format.date(new Date(vRows[i].SalesTransactionDate), 'MM/dd/yyyy'))
                        )
                        .append($('<td>')
                            .text(vRows[i].DepositDate)
                        )
                       .append($('<td>')
                            .text(vRows[i].Cashier)
                        )
                        .append($('<td>')
                            .text(vRows[i].DepositTo)
                        )
                        .append($('<td style="text-align:right">')
                            .text(AccountingFormat(vRows[i].SalesAmount))
                        )
                        .append($('<td style="text-align:right">')
                            .text(AccountingFormat(vRows[i].TotalAcceptedFromPrevTurnOver))
                        )
                        .append($('<td style="text-align:right">')
                            .text(AccountingFormat(vRows[i].TotalAmount))
                        )
                        .append($('<td style="text-align:right">')
                            .text(AccountingFormat(vRows[i].TurnOverVariance))
                        )

            //pItem = $('<tr>')                            
            //           .append($('<td>')
            //                .text(vRows[i].DepositDate)
            //            )
            //            .append($('<td>')
            //                .html('<b>' + vRows[i].DepositedByName + '</b>')
            //            )
            //            .append($('<td>')
            //                .html('<b>' + (vRows[i].DepositedTo || vRows[i].DepositedToByName) + '</b>')
            //            )
            //            .append($('<td>')
            //                .html('<b>' + AccountingFormat(vRows[i].TotalAmount))
            //            )
            //            .append($('<td>')
            //                .text(AccountingFormat(vRows[i].TotalCash))
            //                .append($('<span>')
            //                    .addClass('tooltip')
            //                    .append($('<span>')
            //                         .addClass('tooltip-container')
            //                         .append($('<span>')
            //                            .addClass('grid-right')
            //                            .html('<table>' +
            //                                        '<tr>' +
            //                                            '<td colspan=12 style=\'text-align:center\'>' +
            //                                                'Cash Denomination(PhP)' +
            //                                            '</td>' +
            //                                        '</tr>' +
            //                                        '<tr>' +
            //                                            '<td> 0.05 </td>' +
            //                                            '<td> 0.10 </td>' +
            //                                            '<td> 0.25 </td>' +
            //                                            '<td> 1.00 </td>' +
            //                                            '<td> 5.00 </td>' +
            //                                            '<td> 10.00 </td>' +
            //                                            '<td> 20.00 </td>' +
            //                                            '<td> 50.00 </td>' +
            //                                            '<td> 100.00 </td>' +
            //                                            '<td> 200.00 </td>' +
            //                                            '<td> 500.00 </td>' +
            //                                            '<td> 1,000.00 </td>' +
            //                                        '</tr>' +
            //                                        '<tr>' +
            //                                            '<td>' + (CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfFiveCents) + '</td>' +
            //                                            '<td>' + (CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfTenCents) + '</td>' +
            //                                            '<td>' + (CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfTwentyFiveCents) + '</td>' +
            //                                            '<td>' + (CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfOnePeso) + '</td>' +
            //                                            '<td>' + (CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfFivePesos) + '</td>' +
            //                                            '<td>' + (CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfTenPesos) + '</td>' +
            //                                            '<td>' + (CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfTwentyPesos) + '</td>' +
            //                                            '<td>' + (CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfFiftyPesos) + '</td>' +
            //                                            '<td>' + (CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfOneHundredPesos) + '</td>' +
            //                                            '<td>' + (CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfTwoHundredPesos) + '</td>' +
            //                                            '<td>' + (CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfFiveHundredPesos) + '</td>' +
            //                                            '<td>' + (CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfOneThousandPesos) + '</td>' +
            //                                        '</tr>' +
            //                                    '</table>')
            //                            )
            //                            //.addClass('grid-right')
            //                            //.html('<b> Denomination <b/> <br> 0.05 <br /> ' + (CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfFiveCents))
            //                            //)
            //                            //.append($('<span>')
            //                            //.addClass('grid-right')
            //                            //.html('<br> 0.10 <br /> ' + (CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfTenCents))
            //                            //)
            //                            //.append($('<span>')
            //                            //.addClass('grid-right')
            //                            //.html('<br> 0.25 <br /> ' + (CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfTwentyFiveCents))
            //                            //)
            //                            //.append($('<span>')
            //                            //.addClass('grid-right')
            //                            //.html('<br> 1 <br /> ' + (CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfOnePeso))
            //                            //)
            //                            //.append($('<span>')
            //                            //.addClass('grid-right')
            //                            //.html('<br> 5.00 <br /> ' + (CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfFivePesos))
            //                            //)
            //                            //.append($('<span>')
            //                            //.addClass('grid-right')
            //                            //.html('<br> 10.00 <br /> ' + (CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfTenPesos))
            //                            //)
            //                            //.append($('<span>')
            //                            //.addClass('grid-right')
            //                            //.html('<br> 20.00<br /> ' + (CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfTwentyPesos))
            //                            //)
            //                            //.append($('<span>')
            //                            //.addClass('grid-right')
            //                            //.html('<br> 50.00 <br /> ' + (CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfFiftyPesos))
            //                            //)
            //                            //.append($('<span>')
            //                            //.addClass('grid-right')
            //                            //.html('<br> 100.00 <br /> ' + (CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfOneHundredPesos))
            //                            //)
            //                            //.append($('<span>')
            //                            //.addClass('grid-right')
            //                            //.html('<br> 200.00 <br /> ' + (CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfTwoHundredPesos))
            //                            //)
            //                            //.append($('<span>')
            //                            //.addClass('grid-right')
            //                            //.html('<br> 500.00 <br /> ' + (CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfFiveHundredPesos))
            //                            //)
            //                            //.append($('<span>')
            //                            //.addClass('grid-right')
            //                            //.html('<br> 1,000.00 <br /> ' + (CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfOneThousandPesos))
            //                            //)
            //                     )
            //                )                            

            //            )
            //            .append($('<td>')
            //                .text(AccountingFormat(vRows[i].TotalCheck))
            //            )
            //            .append($('<td>')
            //                .text(AccountingFormat(vRows[i].TotalCreditCard))
            //            )
            //            .append($('<td>')
            //                .text(AccountingFormat(vRows[i].TotalDebitCard))
            //            )
            //            .append($('<td>')
            //                .text(AccountingFormat(vRows[i].TotalSRV))
            //            )
            //            .append($('<td>')
            //                .text(AccountingFormat(vRows[i].TotalPettyCash))
            //            )
            //            //.append($('<td>')
            //            //    .text(CInt2(vRows[i].IsFromTotalAmount)==1?'N/A':vRows[i].NoOfFiveCents)
            //            //)
            //            //.append($('<td>')
            //            //    .text(CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfTenCents)
            //            //)
            //            //.append($('<td>')
            //            //    .text(CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfTwentyFiveCents)
            //            //)
            //            //.append($('<td>')
            //            //    .text(CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfOnePeso)
            //            //)
            //            //.append($('<td>')
            //            //    .text(CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfFivePesos)
            //            //)
            //            //.append($('<td>')
            //            //    .text(CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfTenPesos)
            //            //)
            //            //.append($('<td>')
            //            //    .text(CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfTwentyPesos)
            //            //)
            //            //.append($('<td>')
            //            //    .text(CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfFiftyPesos)
            //            //)
            //            //.append($('<td>')
            //            //    .text(CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfOneHundredPesos)
            //            //)
            //            //.append($('<td>')
            //            //    .text(CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfTwoHundredPesos)
            //            //)
            //            //.append($('<td>')
            //            //    .text(CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfFiveHundredPesos)
            //            //)
            //            //.append($('<td>')
            //            //    .text(CInt2(vRows[i].IsFromTotalAmount) == 1 ? 'N/A' : vRows[i].NoOfOneThousandPesos)
            //            //)
            //            .append($('<td>')
            //                .text('TBI')
            //            )
            //            .append($('<td>')
            //                .text('TBI')
            //            )
            pLV.append(pItem);
        }
    }

});