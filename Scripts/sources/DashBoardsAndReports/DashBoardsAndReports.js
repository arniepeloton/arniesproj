require([
    'common',
    'barRenderer',
    'pieRenderer',
    'categoryAxisRenderer',
    'pointLabels',
    'cursor',
    'highlighter'
    ], function () {

        var m_SalesReturnData;

        var m_IsDateRange = 0;
        var m_Period = '';
        var m_Slice = '';
        var m_FilterLabel = ''

        var GroupDateBy = '';
        var GraphSeries = [];
        var ProductClassFeature = [];
        var m_Data;
        var m_Ticks;
        var m_Labels;

        var m_DateFrom;
        var m_DateTo;

        var m_PreviousSelected = 'day';

        var m_Filter1 = '';
        var m_TicksBar;
        var m_TicksPie;

        $("[data-role=header]").fixedtoolbar({ tapToggle: false });

        var IsSalesDistribution = true;

        $(function () {

            GetFowst();
            PageShow();
            // see /Scripts3/common/common.js
            //startclock('#clock');


            $('#btnPeriod').click(function () {
                if (IsSalesDistribution) {
                    $('#panDates').popup('open');
                }
                else {
                    $('#panSalesQuotaRange').popup('open');
                }
            });

            $('#btnGraphType').click(function () {
                $('#panSettings').popup('open');
            });

            $('#btnFOWST').click(function () {
                $('#panFOWST').popup('open');
            });

            $('#btnGo').click(function () {
                GetTrendingOrSalesDistributionData();
            });

            $('#li2DaysAgo').click(function () {
                $('#panDates').popup('close');
                $('#lblPeriod').text('[ ' + $('#li2DaysAgo').text() + ']');
                SetPeriod('2DaysAgo', 0);
                SetSettings();
            });

            $('#liYesterday').click(function () {
                $('#panDates').popup('close');
                $('#lblPeriod').text('[ ' + $('#liYesterday').text() + ']');
                SetPeriod('Yesterday', 0);
                SetSettings();
            });

            $('#liToday').click(function () {
                $('#panDates').popup('close');
                $('#lblPeriod').text('[ ' + $('#liToday').text() + ']');
                SetPeriod('Today', 0);
                SetSettings();
            });

            $('#liWeekToDate').click(function () {
                $('#panDates').popup('close');
                $('#lblPeriod').text('[ ' + $('#liWeekToDate').text() + ']');
                SetPeriod('WeekToDate', 0);
                SetSettings();
            });

            $('#liMonthToDate').click(function () {
                $('#panDates').popup('close');
                $('#lblPeriod').text('[ ' + $('#liMonthToDate').text() + ']');
                SetPeriod('MonthToDate', 0);
                SetSettings();
            });

            $('#liCustomPeriod').click(function () {
                $('#panDates').popup('close');
                $('#panCustomPeriod').popup('open');
            });

            $('#btnDoneCustomerPeriod').click(function () {
                $('#panCustomPeriod').popup('close');
                $('#lblPeriod').text('[ ' +
                $.format.date($('#txtDateFrom').DateCombo('val'), 'MM.dd.yy') + '--' +
                $.format.date($('#txtDateTo').DateCombo('val'), 'MM.dd.yy') +
                ' ]');
                SetPeriod('CustomPeriod', 1);
                SetSettings();
            });

            $('#btnDoneSettings').click(function () {
                $('#lblPeriod').text('');

                if ($('#selSettingsDetails').val() == 'optSalesTrendAndDistribution') {
                    $('#btnFOWSTTeamLabel').html('FOWST');
                    $('#lblFOWST').html('');
                    GetFowst();
                    $('#liMainChart').html('Sales Trend');
                    IsSalesDistribution = true;
                }
                else {
                    $('#btnFOWSTTeamLabel').html('Team');
                    $('#lblFOWST').html('');
                    GetAllTeams();
                    SetMonth();
                    $('#liMainChart').html('Sales Ratio');
                    IsSalesDistribution = false;
                }

                SetSettings();
            });

            $('#selDice').change(function () {
                if ($('#selDice').val() == '') {

                }
                else {
                    DiceData();
                }
            });

            $('#dvSecondChart').bind('jqplotDataClick',
                function (ev, seriesIndex, pointIndex, data) {
                    m_Filter1 = m_TicksBar[pointIndex];

                    $('#panDice').popup('open');

                    $('#liThirdChartMainLabel').text('Showing Sales>' + m_Filter1 + '>');

                    ShowHideDiceList();

                }
            );

            //            $('#dvThirdChart').bind('jqplotDataClick',
            //                function (ev, seriesIndex, pointIndex, data) {
            //                    m_Filter1 = m_Filter1 + '|' + m_TicksBar[pointIndex];

            //                    $('#panMinch').popup('open');

            //                    $('#liFourthChartMainLabel').text('Showing Sales>' + m_Filter1 + '>');

            //                    ShowHideMinchList();

            //                }
            //            );

            $('#dvMainChart').click(function () {
                if (IsSalesDistribution) {
                    $('#panSalesDistribution').popup('open');
                }
            });

            $('#liSecondChart').click(function () {
                $('#panSalesDistribution').popup('open');
            });


            $('#liThirdChartLabel').click(function () {
                $('#panDice').popup('open');
            });

            $('#liSalesDistributionByDay').click(function () {
                $('#panSalesDistribution').popup('close');
            });

            $('#liSalesDistributionByMonth').click(function () {
                var Fowst;

                Fowst = GetSelectedFowst();

                data = { GroupBy: 'Month',
                    Fowst: Fowst,
                    From: m_DateFrom,
                    To: m_DateTo
                };

                GetPOSSalesReportByTrendOrSalesDistribution(data)

                $('#panSalesDistribution').popup('close');
            });

            $('#liDiceByBrand').click(function () {
                DiceData('Brand', 'tblThirdChart', 'dvThirdChart', 'dvDice');
                $('#panDice').popup('close');
            });

            $('#liDiceByDivision').click(function () {
                DiceData('Division', 'tblThirdChart', 'dvThirdChart', 'dvDice');
                $('#panDice').popup('close');
            });

            $('#btnHome').click(function (e) {
                e.preventDefault();

                window.location = '/People/PeopleMenu';

            });

            $('#btnDownloadSliceAsExcel').click(function () {

            });

            $('#btnDownloadDiceAsExcel').click(function () {

            });

            $('#lnkTransactionDetails').click(function () {
                GetTransactionDetails();
                $('#txtSearchProduct').val('');
            });

            //            $('#txtSearchProduct').keypress(function (e) {
            //                if (e.keyCode == 13) {
            //                    m_SalesReturnData.IsSearch = 1;
            //                    m_SalesReturnData.SearchString = $('#txtSearchProduct').val();
            //                    GetTransactionDetails();
            //                }
            //            });
            $('#formSearch').submit(function (e) {
                e.preventDefault();
                m_SalesReturnData.IsSearch = 1;
                m_SalesReturnData.SearchString = $('#txtSearchProduct').val();
                GetTransactionDetails();
                return false;
            });
            //additional by janine
            $('#btnTransDetailsBack').click(function (e) {

                e.preventDefault();
                $.mobile.changePage('#pageMain');

            });
            //end of additional

        }); //inner function

        function PageShow() {

            if (ValidPrevPage(arguments)) {
                GetAllFeatures();
                
                data = { GroupBy: 'Day',
                    Fowst: GetUrlParameters().FowstID,
                    From: new Date(GetUrlParameters().From),
                    To: new Date(GetUrlParameters().To),
                    IsRawData: 0
                };

                m_SalesReturnData = data;

                GetPOSSalesReportByTrendOrSalesDistribution(data);
            };

            function ValidPrevPage(args) {
                try {
                    return args[1].prevPage.attr('data-role') != 'dialog';
                } catch (e) {
                    return true;
                };
            };
        }

        function SetSettings() {
            $('#panSettings').popup('close');

            $('#lblGraphType').text('[ ' + $('#selSettings :selected').text() + ' ]');
            $('#lblGraphTypeDetail').text('[ ' + $('#selSettingsDetails :selected').text() + ' ]');
        }

        function GetAllFeatures() {
            AjaxSend({
                url: 'DashboardsAndReports/GetOrganizationProductFeatures',
                success: function (vData) {
                    DisplayOrganizationProductFeatures(vData.Results.Rows);
                }
            });
        }

        function GetSalesReturnsDetails() {
            var m_Data = m_SalesReturnData;

            m_Data.IsRawData = 1;

            AjaxSend({
                url: 'DashboardsAndReports/GetSalesReturnsData',
                data: m_Data,
                success: function (vData) {
                    DisplaySalesReturnDetails(vData.Results.Rows);
                }
            });
        }

        function DisplaySalesReturnDetails(vRows) {
            var pLV = $('#tblSalesReturnDetailsBody')

            $('#tblSalesReturnDetailsBody tr').empty();
            //$('#lstTransactionDetails li').not(':eq(0)').not(':eq(0)').remove();

            for (var i = 0; i <= vRows.length - 1; i++) {

                pItem = $('<tr>')
                    .append($('<th>')
                          .text(vRows[i].MemoNumber)
                    )
                    .append($('<th>')
                          .text(vRows[i].MemoDate)
                    )
                    .append($('<th>')
                          .text(vRows[i].SrvNum)
                    )
                    .append($('<th>')
                          .text(AccountingFormat(vRows[i].Amount))
                    )

                pLV.append(pItem);

            }

            $('#lstFOWSTFrame').listview('refresh');
        }

        function GetTransactionDetails() {
            var m_Data = m_SalesReturnData;

            m_Data.IsRawData = 1;
            GetSalesReturnsDetails();

            AjaxSend({
                url: 'DashboardsAndReports/GetPOSSalesReportByTrendOrSalesDistribution',
                data: m_Data,
                success: function (vData) {
                    DisplayTransactionDetails(vData.ReportPOS.Rows);
                    m_SalesReturnData.IsSearch = 0;
                }
            });
        }

        function DisplayTransactionDetails(vRows) {

            var pLV = $('#tblTransactionDetailsBody');

            pLV.html('');

            //$('#lstTransactionDetails li').not(':eq(0)').not(':eq(0)').remove();

            for (var i = 0; i <= vRows.length - 1; i++) {

                pItem = $('<tr>')
                    .append($('<td>')
                          .text(vRows[i].InvoiceNo)
                    )
                    .append($('<td>')
                          .text(vRows[i].InvoiceDate)
                    )
                    .append($('<td>')
                          .text(vRows[i].ProductDescription)
                    )
                    .append($('<td style="text-align: center">')
                          .text($.format.number(CDec2(vRows[i].Qty), '#,###'))
                    )
                    .append($('<td>')
                          .text(vRows[i].UnitMeasure)
                    )
                    .append($('<td style="text-align: right">')
                          .text(AccountingFormat(vRows[i].Price))
                    )
                    .append($('<td style="text-align: right">')
                          .text(AccountingFormat(vRows[i].SubTotal))
                    )
                    .append($('<td style="text-align: right">')
                          .text(AccountingFormat(vRows[i].Discount))
                    )
                    .append($('<td>')
                          .text(vRows[i].SalesRepresentative)
                    )
                    .append($('<td>')
                          .text(vRows[i].Customer)
                    )

                pLV.append(pItem);

            }

            try {
                $('#lstFOWSTFrame').listview('refresh');
            }
            catch (e) {
                $('#lstFOWSTFrame').listview();
            }

            //            $('#panTransactionDetails').popup('open');
            $.mobile.changePage('#panTransactionDetails');

        }


        function DisplayOrganizationProductFeatures(vRows) {
            var pLV = $('#lstSlice');
            var pLV2 = $('#lstDice');
            var pLV3 = $('#lstMinch');

            //time slice
            pItem = $('<li>')
                .append($('<a>')
                    .attr('ID', 'SliceTime')
                    .attr('name', 'Time')
                    .click(function (e) {
                        m_Slice = $(this).attr('name');
                        SliceData(m_Slice);
                        ShowHideDiceList();
                        $('#panSalesDistribution').popup('close');
                    })
                    .append($('<h3>')
                        .text('Time')
                    )
                );

            pLV.append(pItem);

            //time dice
            pItem = $('<li>')
                .append($('<a>')
                    .attr('ID', 'DiceTime')
                    .attr('name', 'Time')
                    .click(function (e) {
                        DiceData($(this).attr('name'), 'tblThirdChart', 'dvThirdChart', 'dvDice');
                        $('#panDice').popup('close');
                    })
                    .append($('<h3>')
                        .text('Time')
                    )
                );

            pLV2.append(pItem);

            //time minch
            pItem = $('<li>')
                .append($('<a>')
                    .attr('ID', 'MinchTime')
                    .attr('name', 'Time')
                    .click(function (e) {
                        MinchData($(this).attr('name'));
                        $('#panMinch').popup('close');
                    })
                    .append($('<h3>')
                        .text('Time')
                    )
                );

            pLV3.append(pItem);

            //brand slice
            pItem = $('<li>')
                .append($('<a>')
                    .attr('ID', 'SliceBrand')
                    .attr('name', 'Brand')
                    .click(function (e) {
                        m_Slice = $(this).attr('name');
                        SliceData(m_Slice);
                        ShowHideDiceList();
                        $('#panSalesDistribution').popup('close');
                    })
                    .append($('<h3>')
                        .text('Brand')
                    )
                );

            pLV.append(pItem);

            //brand dice
            pItem = $('<li>')
                .append($('<a>')
                    .attr('ID', 'DiceBrand')
                    .attr('name', 'Brand')
                    .click(function (e) {
                        DiceData($(this).attr('name'), 'tblThirdChart', 'dvThirdChart', 'dvDice');
                        $('#panDice').popup('close');
                    })
                    .append($('<h3>')
                        .text('Brand')
                    )
                );

            pLV2.append(pItem);

            //brand minch
            pItem = $('<li>')
                .append($('<a>')
                    .attr('ID', 'MinchBrand')
                    .attr('name', 'Brand')
                    .click(function (e) {
                        MinchData($(this).attr('name'));
                        $('#panMinch').popup('close');
                    })
                    .append($('<h3>')
                        .text('Brand')
                    )
                );

            pLV3.append(pItem);

            //sales associate slice
            pItem = $('<li>')
                .append($('<a>')
                    .attr('ID', 'SliceSalesRepresentative')
                    .attr('name', 'SalesRepresentative')
                    .click(function (e) {
                        m_Slice = $(this).attr('name');
                        SliceData(m_Slice);
                        ShowHideDiceList();
                        $('#panSalesDistribution').popup('close');
                    })
                    .append($('<h3>')
                        .text('Sales Representative')
                    )
                );

            pLV.append(pItem);


            //sales associate dice
            pItem = $('<li>')
                                .append($('<a>')
                                    .attr('ID', 'DiceSalesRepresentative')
                                    .attr('name', 'SalesRepresentative')
                                    .click(function (e) {
                                        DiceData($(this).attr('name'), 'tblThirdChart', 'dvThirdChart', 'dvDice');
                                        $('#panDice').popup('close');
                                    })
                                    .append($('<h3>')
                                        .text('Sales Representative')
                                    )
                                );

            pLV2.append(pItem);

            pItem = $('<li>')
                                .append($('<a>')
                                    .attr('ID', 'SliceDivision')
                                    .attr('name', 'Division')
                                    .click(function (e) {
                                        m_Slice = $(this).attr('name');
                                        SliceData(m_Slice);
                                        ShowHideDiceList();
                                        $('#panSalesDistribution').popup('close');
                                    })
                                    .append($('<h3>')
                                        .text('Division')
                                    )
                                );

            pLV.append(pItem);

            pItem = $('<li>')
                                .append($('<a>')
                                    .attr('ID', 'DiceDivision')
                                    .attr('name', 'Division')
                                    .click(function (e) {
                                        DiceData($(this).attr('name'), 'tblThirdChart', 'dvThirdChart', 'dvDice');
                                        $('#panDice').popup('close');
                                    })
                                    .append($('<h3>')
                                        .text('Division')
                                    )
                                );

             pLV2.append(pItem);

             //additional by janine. fixed . october 30 11am
             //Product
             pItem = $('<li>')
                                .append($('<a>')
                                    .attr('ID', 'SliceProduct')
                                    .attr('name', 'Product')
                                    .click(function (e) {
                                        m_Slice = $(this).attr('name');
                                        SliceData(m_Slice);
                                        ShowHideDiceList();
                                        $('#panSalesDistribution').popup('close');
                                    })
                                    .append($('<h3>')
                                        .text('Product')
                                    )
                                );

             pLV.append(pItem);

             pItem = $('<li>')
                                .append($('<a>')
                                    .attr('ID', 'DiceProduct')
                                    .attr('name', 'Product')
                                    .click(function (e) {
                                        DiceData($(this).attr('name'), 'tblThirdChart', 'dvThirdChart', 'dvDice');
                                        $('#panDice').popup('close');
                                    })
                                    .append($('<h3>')
                                        .text('Product')
                                    )
                                );

             pLV2.append(pItem);

             //DELIVERY METHOD
             pItem = $('<li>')
                                .append($('<a>')
                                    .attr('ID', 'SliceDeliveryMethod')
                                    .attr('name', 'Delivery Method')
                                    .click(function (e) {
                                        m_Slice = $(this).attr('name');
                                        SliceData(m_Slice);
                                        ShowHideDiceList();
                                        $('#panSalesDistribution').popup('close');
                                    })
                                    .append($('<h3>')
                                        .text('Delivery Method')
                                    )
                                );

             pLV.append(pItem);

             pItem = $('<li>')
                                .append($('<a>')
                                    .attr('ID', 'DiceDeliveryMethod')
                                    .attr('name', 'Delivery Method')
                                    .click(function (e) {
                                        DiceData($(this).attr('name'), 'tblThirdChart', 'dvThirdChart', 'dvDice');
                                        $('#panDice').popup('close');
                                    })
                                    .append($('<h3>')
                                        .text('Delivery Method')
                                    )
                                );

             pLV2.append(pItem);

             //ORDER METHOD
             pItem = $('<li>')
                                .append($('<a>')
                                    .attr('ID', 'SliceOrderMethod')
                                    .attr('name', 'Order Method')
                                    .click(function (e) {
                                        m_Slice = $(this).attr('name');
                                        SliceData(m_Slice);
                                        ShowHideDiceList();
                                        $('#panSalesDistribution').popup('close');
                                    })
                                    .append($('<h3>')
                                        .text('Order Method')
                                    )
                                );

             pLV.append(pItem);

             pItem = $('<li>')
                                .append($('<a>')
                                    .attr('ID', 'DiceOrderMethod')
                                    .attr('name', 'Order Method')
                                    .click(function (e) {
                                        DiceData($(this).attr('name'), 'tblThirdChart', 'dvThirdChart', 'dvDice');
                                        $('#panDice').popup('close');
                                    })
                                    .append($('<h3>')
                                        .text('Order Method')
                                    )
                                );

             pLV2.append(pItem);

             //CUSTOMER GENDER
             pItem = $('<li>')
                                .append($('<a>')
                                    .attr('ID', 'SliceGender')
                                    .attr('name', 'Gender')
                                    .click(function (e) {
                                        m_Slice = $(this).attr('name');
                                        SliceData(m_Slice);
                                        ShowHideDiceList();
                                        $('#panSalesDistribution').popup('close');
                                    })
                                    .append($('<h3>')
                                        .text('Gender')
                                    )
                                );

             pLV.append(pItem);

             pItem = $('<li>')
                                .append($('<a>')
                                    .attr('ID', 'DiceGender')
                                    .attr('name', 'Gender')
                                    .click(function (e) {
                                        DiceData($(this).attr('name'), 'tblThirdChart', 'dvThirdChart', 'dvDice');
                                        $('#panDice').popup('close');
                                    })
                                    .append($('<h3>')
                                        .text('Gender')
                                    )
                                );

             pLV2.append(pItem);


             //CUSTOMER AGE
             pItem = $('<li>')
                                .append($('<a>')
                                    .attr('ID', 'SliceAge')
                                    .attr('name', 'Age')
                                    .click(function (e) {
                                        m_Slice = $(this).attr('name');
                                        SliceData(m_Slice);
                                        ShowHideDiceList();
                                        $('#panSalesDistribution').popup('close');
                                    })
                                    .append($('<h3>')
                                        .text('Age')
                                    )
                                );

             pLV.append(pItem);

             pItem = $('<li>')
                                .append($('<a>')
                                    .attr('ID', 'DiceAge')
                                    .attr('name', 'Age')
                                    .click(function (e) {
                                        DiceData($(this).attr('name'), 'tblThirdChart', 'dvThirdChart', 'dvDice');
                                        $('#panDice').popup('close');
                                    })
                                    .append($('<h3>')
                                        .text('Age')
                                    )
                                );

             pLV2.append(pItem);
             //end of additional




            for (i = 0; i <= vRows.length - 1; i++) {
                pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', 'Slice' + vRows[i].Label)
                            .attr('name', vRows[i].Label)
                            .click(function (e) {
                                m_Slice = $(this).attr('name');
                                SliceData(m_Slice);
                                ShowHideDiceList();
                                $('#panSalesDistribution').popup('close');
                            })
                            .append($('<h3>')
                                .text(vRows[i].Label)
                            )
                        );

                pLV.append(pItem);

                pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', 'Dice' + vRows[i].Label)
                            .attr('name', vRows[i].Label)
                            .click(function (e) {
                                DiceData($(this).attr('name'), 'tblThirdChart', 'dvThirdChart', 'dvDice');
                                $('#panDice').popup('close');
                            })
                            .append($('<h3>')
                                .text(vRows[i].Label)
                            )
                        );

                pLV2.append(pItem);
            }

            try {
                pLV.listview('refresh');
                pLV2.listview('refresh');
                pLV3.listview('refresh');
            }
            catch (e) {
                pLV.listview();
                pLV2.listview();
            }
        }

        function GetSalesReturnsData() {
            AjaxSend({
                url: 'DashboardsAndReports/GetSalesReturnsData',
                data: m_SalesReturnData,
                success: function (vData) {
                    if (vData.Results.Rows) {
                        $('#tdNetSales').html('PhP ' + AccountingFormat(CDec2($('#tdTotalGrossSales').text()) - CDec2(vData.Results.Rows[0].Total) - CDec2($('#tdDiscounts').text())));
                        $('#tdTotalGrossSales').html('PhP ' + $('#tdTotalGrossSales').text());
                        $('#tdDiscounts').html('PhP ' + $('#tdDiscounts').text());
                        $('#tdSalesReturn').html('PhP ' + AccountingFormat(vData.Results.Rows[0].Total));
                    }
                }
            });
        }

        function GetLastPOSSalesTransaction() {
            AjaxSend({
                url: 'DashboardsAndReports/GetLastPOSSalesTransaction',
                data: m_SalesReturnData,
                success: function (vData) {
                    if (vData.Results.Rows) {
                        $('#tdInvoiceDateAndTime').html(vData.Results.Rows[0].LastInvoiceDate);
                        $('#tdInvoiceNumber').html(vData.Results.Rows[0].LastInvoiceNumber);
                    }
                }
            });
        }



        function ShowHideDiceList() {
            $('#lstDice li a').each(function () {
                $(this).show();
            });

            $('#Dice' + m_Slice.replace(' ', '').replace('(', '').replace(')', '').replace('/', '').replace('\'', '').replace('Slice', '')).hide();

            $('#lstDice').listview('refresh');
        }

        function SliceData(SliceBy) {
            var data;
            m_DateFrom = new Date(m_DateFrom);
            m_DateTo = new Date(m_DateTo);

            var Fowst;

            Fowst = GetSelectedFowst();

            if (Fowst == 0) {
                MessageBox("Please select FOWST",
                                "FOWST",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                            );
                $('#selSlice').val('');
                $('#selSlice').selectmenu('refresh');
                return;
            }
            
            switch (SliceBy) {
                case "Day":
                    data = { SliceBy: 'Day',
                        Fowst: Fowst,
                        From: (m_DateFrom.getMonth() + 1) + '/' + m_DateFrom.getDate() + '/' + m_DateFrom.getFullYear(),
                        To: (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear()
                    };
                    break;
                case "Time":
                    data = {
                        SliceBy: 'optTime',
                        Fowst: Fowst,
                        From: (m_DateFrom.getMonth() + 1) + '/' + m_DateFrom.getDate() + '/' + m_DateFrom.getFullYear(),
                        To: (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear()
                    };
                    break;
                case "Brand":
                    data = { SliceBy: 'optBrand',
                        Fowst: Fowst,
                        From: (m_DateFrom.getMonth() + 1) + '/' + m_DateFrom.getDate() + '/' + m_DateFrom.getFullYear(),
                        To: (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear()
                    };
                    window.location = '#dvSlice';
                    break;
                case "Division":
                    data = { SliceBy: 'optDivisionDescription',
                        Fowst: Fowst,
                        From: (m_DateFrom.getMonth() + 1) + '/' + m_DateFrom.getDate() + '/' + m_DateFrom.getFullYear(),
                        To: (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear()
                    };
                    window.location = '#dvSlice';
                    break;
                //additional by janine oct 30 12pm       
                case "Product":
                    data = { SliceBy: 'optProduct',
                        Fowst: Fowst,
                        From: (m_DateFrom.getMonth() + 1) + '/' + m_DateFrom.getDate() + '/' + m_DateFrom.getFullYear(),
                        To: (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear()
                    };
                    window.location = '#dvSlice';

                    $('#divShowProductCount').show();
                    break;
                case "Delivery Method":
                    data = { SliceBy: 'optDeliveryMethod',
                        Fowst: Fowst,
                        From: (m_DateFrom.getMonth() + 1) + '/' + m_DateFrom.getDate() + '/' + m_DateFrom.getFullYear(),
                        To: (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear()
                    };
                    window.location = '#dvSlice';
                    break;
                case "Order Method":
                    data = { SliceBy: 'optOrderMethod',
                        Fowst: Fowst,
                        From: (m_DateFrom.getMonth() + 1) + '/' + m_DateFrom.getDate() + '/' + m_DateFrom.getFullYear(),
                        To: (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear()
                    };
                    window.location = '#dvSlice';
                    break;
                case "Gender":
                    data = { SliceBy: 'optGender',
                        Fowst: Fowst,
                        From: (m_DateFrom.getMonth() + 1) + '/' + m_DateFrom.getDate() + '/' + m_DateFrom.getFullYear(),
                        To: (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear()
                    };
                    window.location = '#dvSlice';
                    break;
                case "Age":
                    data = { SliceBy: 'optAge',
                        Fowst: Fowst,
                        From: (m_DateFrom.getMonth() + 1) + '/' + m_DateFrom.getDate() + '/' + m_DateFrom.getFullYear(),
                        To: (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear()
                    };
                    window.location = '#dvSlice';
                    break;
                //end of additional   
                case "Season/Year":
                    data = { SliceBy: 'Season/Year',
                        Fowst: Fowst,
                        From: (m_DateFrom.getMonth() + 1) + '/' + m_DateFrom.getDate() + '/' + m_DateFrom.getFullYear(),
                        To: (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear()
                    };
                    window.location = '#dvSlice';
                    break;
                case "SalesRepresentative":
                    data = { SliceBy: 'optSalesRepresentative',
                        Fowst: Fowst,
                        From: (m_DateFrom.getMonth() + 1) + '/' + m_DateFrom.getDate() + '/' + m_DateFrom.getFullYear(),
                        To: (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear()
                    };
                    window.location = '#dvSlice';
                    break;
                default:
                    data = { SliceBy: SliceBy,
                        Fowst: Fowst,
                        From: (m_DateFrom.getMonth() + 1) + '/' + m_DateFrom.getDate() + '/' + m_DateFrom.getFullYear(),
                        To: (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear()
                    };
                    window.location = '#dvSlice';
                    break;
            }

            $('#dvSlice').css('visibility', 'visible');

            $('#tblThirdChart').empty();
            $('#dvThirdChart').empty();
            $('#dvDice').css('visibility', 'hidden');

            $('#liSecondChartMainLabel').text('Showing Sales>');
            $('#liSecondChart').text(SliceBy);
            $('#lblSecondChartInstruction').text('* Click on a ' + SliceBy + ' to break down data')

            GetPOSSalesReportBySlice(data);
        }

        function GetFowst() {
            var data = { FilterFowst: 0
            }

            AjaxSend({
                url: 'Browser/LibraryFowst',
                data: data,
                success: function (vData) {
                    DisplayFowst(vData.Results.Rows);
                }
            });
        }

        window.SetPeriod = function (mPeriod, mIsDateRange) {
            m_Period = mPeriod;
            m_IsDateRange = mIsDateRange;            
        }

        function DisplayFowst(vRows) {
            var pLV = $("#tblFOWST");


            $('#tblFOWST tbody').empty();
            $('#lstFOWSTFrame li').not(':eq(0)').not(':eq(0)').remove();

            for (var i = 0; i <= vRows.length - 1; i++) {

                var checked = undefined;

                if (vRows[i].ID == sessionStorage.FowstID) {
                    checked = 'checked';
                }

                pItem = $('<tr>')
                    .append($('<td>')
                        .append($('<input type=radio data-theme=e />')
                            .attr('ID', vRows[i].ID)
                            .attr('name', 'FOWST')
                            .attr('checked', checked)
                            .attr('data-mini', 'true')
                            .data('data', vRows[i])
                            .click(function () {
                                $('#panFOWST').popup('close');
                                $('#lblFOWST').text('[ ' + $(this).data('data').FowstName + ' ]')
                            })
                        )
                        .append($('<label>')
                            .attr('for', vRows[i].ID)
                            .text(vRows[i].FowstName)
                        )
                    )//td                    

                pLV.append(pItem);

                $('#' + vRows[i].ID).checkboxradio();

            }

            $('#lstFOWSTFrame').listview('refresh');

            var m_Date = new Date();
            var Fowst = '';

            if (sessionStorage.FowstID != undefined) {
                data = { GroupBy: 'Day',
                    Fowst: sessionStorage.FowstID,
                    From: (m_Date.getMonth() + 1) + '/' + m_Date.getDate() + '/' + m_Date.getFullYear(),
                    To: (m_Date.getMonth() + 1) + '/' + m_Date.getDate() + '/' + m_Date.getFullYear()
                };

                GetPOSSalesReportByTrendOrSalesDistribution(data);
            }
        }

        function DiceData(DiceBy, table, divChart, div) {
            var data;
            var Fowst;

            Fowst = GetSelectedFowst();

            if (Fowst == 0) {
                MessageBox("Please select FOWST",
                                "FOWST",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                            );
                $('#selDice').val('');
                $('#selDice').selectmenu('refresh');
                return;
            }

            switch (DiceBy) {
                case "Brand":
                    data = { SliceBy: 'optBrand',
                        Filter1Option: m_Slice,
                        Filter1: m_Filter1,
                        Fowst: Fowst,
                        From: (m_DateFrom.getMonth() + 1) + '/' + m_DateFrom.getDate() + '/' + m_DateFrom.getFullYear(),
                        To: (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear()
                    };
                    window.location = '#' + divChart;
                    break;
                case "Division":
                    data = { SliceBy: 'optDivisionDescription',
                        Filter1Option: m_Slice,
                        Filter1: m_Filter1,
                        Fowst: Fowst,
                        From: (m_DateFrom.getMonth() + 1) + '/' + m_DateFrom.getDate() + '/' + m_DateFrom.getFullYear(),
                        To: (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear()
                    };
                    window.location = '#' + divChart;
                    break;
                case "SalesRepresentative":
                    data = { SliceBy: 'optSalesRepresentative',
                        Filter1Option: m_Slice,
                        Filter1: m_Filter1,
                        Fowst: Fowst,
                        From: (m_DateFrom.getMonth() + 1) + '/' + m_DateFrom.getDate() + '/' + m_DateFrom.getFullYear(),
                        To: (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear()
                    };
                    window.location = '#' + divChart;
                    break;
                //additional by janine oct 30       
                case "Product":
                    data = { SliceBy: 'optProduct',
                        Filter1Option: m_Slice,
                        Filter1: m_Filter1,
                        Fowst: Fowst,
                        From: (m_DateFrom.getMonth() + 1) + '/' + m_DateFrom.getDate() + '/' + m_DateFrom.getFullYear(),
                        To: (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear()
                    };
                    window.location = '#' + divChart;
                    break;
                case "Delivery Method":
                    data = { SliceBy: 'optDeliveryMethod',
                        Filter1Option: m_Slice,
                        Filter1: m_Filter1,
                        Fowst: Fowst,
                        From: (m_DateFrom.getMonth() + 1) + '/' + m_DateFrom.getDate() + '/' + m_DateFrom.getFullYear(),
                        To: (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear()
                    };
                    window.location = '#' + divChart;
                    break;
                case "Order Method":
                    data = { SliceBy: 'optOrderMethod',
                        Filter1Option: m_Slice,
                        Filter1: m_Filter1,
                        Fowst: Fowst,
                        From: (m_DateFrom.getMonth() + 1) + '/' + m_DateFrom.getDate() + '/' + m_DateFrom.getFullYear(),
                        To: (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear()
                    };
                    window.location = '#' + divChart;
                    break;
                case "Gender":
                    data = { SliceBy: 'optGender',
                        Filter1Option: m_Slice,
                        Filter1: m_Filter1,
                        Fowst: Fowst,
                        From: (m_DateFrom.getMonth() + 1) + '/' + m_DateFrom.getDate() + '/' + m_DateFrom.getFullYear(),
                        To: (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear()
                    };
                    window.location = '#' + divChart;
                    break;
                case "Age":
                    data = { SliceBy: 'optAge',
                        Filter1Option: m_Slice,
                        Filter1: m_Filter1,
                        Fowst: Fowst,
                        From: (m_DateFrom.getMonth() + 1) + '/' + m_DateFrom.getDate() + '/' + m_DateFrom.getFullYear(),
                        To: (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear()
                    };
                    window.location = '#' + divChart;
                    break;
                //end of additional  
                default:
                    data = { SliceBy: DiceBy,
                        Filter1Option: m_Slice,
                        Filter1: m_Filter1,
                        Fowst: Fowst,
                        From: (m_DateFrom.getMonth() + 1) + '/' + m_DateFrom.getDate() + '/' + m_DateFrom.getFullYear(),
                        To: (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear()
                    };
                    window.location = '#' + divChart;
            }

            $('#' + div).css('visibility', 'visible');

            switch (DiceBy) {
                case 'Size ID':
                    m_FilterLabel = 'Size';
                    break;
                case 'Sport Activity':
                    m_FilterLabel = 'Sport';
                    break;
                case 'Gender Age':
                    m_FilterLabel = 'Gender';
                    break;
                case 'Season/Year':
                    m_FilterLabel = 'Season';
                    break;
                default:
                    m_FilterLabel = DiceBy;
            }

            $('#liThirdChartLabel').text(m_FilterLabel);

            GetPOSSalesReportByDice(data, 'tblThirdChart', 'dvThirdChart');
        }

        function GetTrendingOrSalesDistributionData() {
            var Fowst;
            GroupDateBy = 'Day';
            m_DateFrom = new Date();
            m_DateTo = new Date();
            var FowstCount = 0;

            Fowst = GetSelectedFowst();

            if (m_IsDateRange == 1) {
                if (!IsDate($('#txtDateFrom').DateCombo('val')) || !IsDate($('#txtDateTo').DateCombo('val'))) {
                    MessageBox("Please put valid date",
                                "FOWST",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                            );
                    return;
                }
                else {
                    m_DateFrom = $('#txtDateFrom').DateCombo('val');
                    m_DateTo = $('#txtDateTo').DateCombo('val');
                }
            }
            else {
                switch (m_Period) {
                    case "2DaysAgo":
                        //back date from
                        m_DateFrom.setDate(m_DateFrom.getDate() - 2);
                        m_DateTo.setDate(m_DateTo.getDate() - 2);
                        break; ;
                    case "Yesterday":
                        //back date from    
                        m_DateFrom.setDate(m_DateFrom.getDate() - 1);
                        m_DateTo.setDate(m_DateTo.getDate() - 1);
                        break;
                    case "Today":
                        //                        m_DateFrom = (m_DateFrom.getMonth() + 1) + '/' + m_DateFrom.getDate() + '/' + m_DateFrom.getFullYear();
                        //                        m_DateTo = (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear();
                        break;
                    case "WeekToDate":
                        m_DateFrom.setDate(m_DateFrom.getDate() - m_DateFrom.getDay());
                        m_DateTo = (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear();
                        break;
                    case "MonthToDate":
                        GroupDateBy = 'Month';
                        m_DateFrom = (m_DateFrom.getMonth() + 1) + '/1/' + m_DateFrom.getFullYear();
                        m_DateTo = (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear();
                        break;
                    case "Month":
                        GroupDateBy = 'Month';
                        m_DateFrom = (m_DateFrom.getMonth() + 1) + '/1/' + m_DateFrom.getFullYear();
                        m_DateTo = (parseInt($('#selSalesQuotaMonth :selected').val()) + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear();
                        break;
                    case "Quarter":
                        GroupDateBy = 'Quarter';
                        m_DateFrom = (m_DateFrom.getMonth() + 1) + '/1/' + m_DateFrom.getFullYear();
                        m_DateTo = $('#selSalesQuotaQuarter').val();
                        break;
                    case "Quarter":
                        GroupDateBy = 'Quarter';
                        m_DateFrom = (m_DateFrom.getMonth() + 1) + '/1/' + m_DateFrom.getFullYear();
                        m_DateTo = $('#selSalesQuotaQuarter').val();
                        break;
                }
            }

            if (Fowst == 0) {
                if (IsSalesDistribution) {
                    MessageBox("Please select FOWST",
                                "FOWST",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                            );
                }
                else {
                    MessageBox("Please select Team",
                                "Team",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                            );
                }
                return;
            }

            ClearCharts();

            data = { GroupBy: GroupDateBy,
                Fowst: Fowst,
                From: m_DateFrom,
                To: m_DateTo,
                IsRawData: 0
            };

            m_SalesReturnData = data;

            if (IsSalesDistribution) {
                GetPOSSalesReportByTrendOrSalesDistribution(data);
            }
            else {
                GetSalesOrderData(data, GroupDateBy);
            }

        }

        function GetPOSSalesReportByTrendOrSalesDistribution(data) {
            AjaxSend({
                url: 'DashboardsAndReports/GetPOSSalesReportByTrendOrSalesDistribution',
                data: data,
                success: function (vData) {
                    if (vData.ReportPOS.Rows.length > 0) {
                        $('#IsDataPresent').css('visibility', 'hidden');
                        $('#lblMainChartInstruction').css('visibility', 'visible');
                        $('#lblMainChartInstruction').text('* Click the chart to breakdown data');
                        $('#tblGrossSalesLabels').css('visibility', 'visible');
                        $('#tblLastInvoiceDetails').css('visibility', 'visible');
                        $('#dvSalesDistributionTable').css('display', 'block');
                        $('#dvSalesOrderTable').css('display', 'none');
                        m_Data = vData.ReportPOS.Rows;
                        DisplayTrend();
                    }
                    else {
                        $('#IsDataPresent').css('visibility', 'visible');
                        $('#lblMainChartInstruction').css('visibility', 'hidden');
                        $('#tblGrossSalesLabels').css('visibility', 'hidden');
                        $('#tblLastInvoiceDetails').css('visibility', 'hidden');
                        $('#tblSalesOrder').css('visibility', 'hidden');
                        $('#tblSalesOrderEmployeeContribution').css('visibility', 'hidden');

                    }
                }
            }); //end ajaxsend
        }

        function DisplaySaleDistribution() {
            var m_Days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
            var m_Months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
            var m_Totals = [];
            var TotalSales = 0;

            m_Totals[0] = [];
            m_Labels = [];
            m_Ticks = [];

            if ($('#rdDaySalesDistribution:checked').val() == 'on') {
                for (var j = 0; j < 7; j++) {
                    m_Totals[0][j] = 0;
                    m_Labels[j] = 0;
                }
                m_Ticks = m_Days;
            }
            else if ($('#rdMonthSalesDistribution:checked').val() == 'on') {
                for (var j = 0; j < 12; j++) {
                    m_Totals[0][j] = 0;
                    m_Labels[j] = 0;
                }
                m_Ticks = m_Months;
            }

            var p = 0;
            var TotalSalesToday = 0;
            var m_Today = new Date();

            for (var i = 0; i < m_Data.length; i++) {
                var m_Date = new Date(m_Data[i].InvoiceDate)
                var m_Day = 0;

                TotalSales = TotalSales + parseFloat(m_Data[i].Total);

                if ($.format.date(m_Today, "MM/dd/yyyy") == $.format.date(new Date(m_Data[i].InvoiceDate), "MM/dd/yyyy")) {
                    TotalSalesToday += parseFloat(m_Data[i].Total);
                }

                if ($('#rdDaySalesDistribution:checked').val() == 'on') {
                    m_Day = m_Date.getDay();
                    m_Totals[0][m_Day] = m_Totals[0][m_Day] + $.format.number(m_Data[i].Total);
                    m_Labels[m_Day] = AccountingFormat(m_Totals[0][m_Day]);
                }
                else if ($('#rdMonthSalesDistribution:checked').val() == 'on') {
                    m_Day = m_Data[i].InvoiceDate - 1;
                    m_Totals[0][m_Day] = m_Totals[0][m_Day] + $.format.number(m_Data[i].Total);
                    m_Labels[m_Day] = AccountingFormat($.format.number(m_Data[i].Total));
                }
                else if ($('#rdHourSalesDistribution:checked').val() == 'on') {
                    m_Totals[0][i] = $.format.number(m_Data[i].Total);
                    m_Labels[i] = AccountingFormat($.format.number(m_Data[i].Total));
                    m_Ticks[i] = m_Data[i].InvoiceDate + ':00';
                }
            }

            DisplayVerticalBarGraph(m_Totals);

            $('#tdTotalGrossSales').text(AccountingFormat(TotalSales));
            $('#tdSalesToday').text(AccountingFormat(TotalSalesToday));
        }

        function DisplayTrend() {
            var m_Totals = [];
            var m_Days = [];
            var m_Months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
            var TotalSales = 0;
            var TotalSalesToday = 0;
            var TotalDiscounts = 0;
            m_Totals[0] = [];

            if (m_Data.length > 0) {
                for (var i = 0; i < m_Data.length; i++) {
                    var m_Date = new Date(m_Data[i].InvoiceDate);
                    var m_Days;
                    var m_Today = new Date();

                    m_Totals[0][i] = $.format.number(m_Data[i].Total);
                    TotalSales = TotalSales + parseFloat(m_Data[i].Total);
                    TotalDiscounts = TotalDiscounts + parseFloat(m_Data[i].DiscountTotal);

                    if ($.format.date(m_Today, "MM/dd/yyyy") == $.format.date(new Date(m_Data[i].InvoiceDate), "MM/dd/yyyy")) {
                        TotalSalesToday += parseFloat(m_Data[i].Total);
                    }

                    if (GroupDateBy == 'Day') {
                        m_Days[i] = (m_Date.getMonth() + 1) + '/' + m_Date.getDate();
                        $('#lblDateFormatMessage').css('visibility', 'hidden');
                    }
                    else if (GroupDateBy == 'Month') {
                        m_Days[i] = m_Months[m_Data[i].InvoiceDate - 1];
                        $('#lblDateFormatMessage').text('Date Format: Month/Date');
                        $('#lblDateFormatMessage').css('visibility', 'hidden');
                    }
                }

                $('#tdTotalGrossSales').text(AccountingFormat(TotalSales));
                $('#tdSalesToday').text(AccountingFormat(TotalSalesToday));
                $('#tdDiscounts').html(AccountingFormat(TotalDiscounts));
                $('#lblMainChartInstruction').css('visibility', 'visible');
                $('#lblMainChartInstruction').text('* Click the chart to breakdown data');
                $('#tblGrossSalesLabels').css('visibility', 'visible');
                $('#tblLastInvoiceDetails').css('visibility', 'visible');

                m_Ticks = m_Days;

                GetSalesReturnsData();
                GetLastPOSSalesTransaction();
                DisplayLineGraph(m_Totals);

            }
        }

        function GetPOSSalesReportBySlice(data) {
            var m_Totals = [];
            var m_TotalGross = 0;
            var m_TotalTransactionCount = 0;
            var m_TotalProductCount = 0;

            AjaxSend({
                url: 'DashboardsAndReports/GetPOSSalesReportBySlice',
                data: data,
                success: function (vData) {
                    if (vData.ReportPOS.Rows.length > 0) {

                        var vRows = vData.ReportPOS.Rows;

                        $('#btnDownloadSliceAsExcel').css('visibility', 'visible');

                        m_TicksBar = [];
                        m_Labels = [];

                        var tData = '';
                        var i = 0;

                        $('#tblSecondChart').html('');

                        m_Totals[0] = [];

                        //additional by janine. show count. october 30 5pm

                        for (i = 0; i <= vRows.length - 1; i++) {

                            m_Totals[0][i] = $.format.number(vRows[i].TransactionCount, vRows[i].ProductFeature);

                            m_TotalTransactionCount = m_TotalTransactionCount + CDec2(vRows[i].TransactionCount);

                            m_TotalProductCount = m_TotalProductCount + CDec2(vRows[i].ProductCount);
                        }
                        //for chart, it will sort desc
                        m_Totals[1] = [];
                        for (i = 0; i <= vRows.length - 1; i++) {

                            m_Totals[1][i] = $.format.number(vRows[i].Total, vRows[i].ProductFeature);

                            m_TotalGross = m_TotalGross + $.format.number(vRows[i].Total);
                        }
                        //end of additional


                        tData = $('<tr>')
                                            .append($('<th>')
                                                .text(m_Slice)
                                            )
                                            .append($('<th style="text-align:right">')
                                                .text('Gross Sales')
                                            )
                                            .append($('<th style="text-align:right">')
                                                .text('Contribution Margin')
                                            )
                                            .append($('<th style="text-align:right">')
                                                .text('Transaction Count')
                                            )
                                            .append($('<th style="text-align:right">')
                                                .text('Product Count')
                                            )
                                            .append($('<br />'))



                        $('#tblSecondChart').append(tData);

                        tData = $('<tr>')
                                .append($('<td colspan="7">')
                                    .append($('<br />'))
                                )

                        $('#tblSecondChart').append(tData);
                        //manually sort desc for table details
                        for (i = vRows.length - 1; i >= 0; i--) {
                            m_TicksBar[i] = vRows[i].ProductFeature;
                            m_Labels[i] = AccountingFormat($.format.number(vRows[i].Total));

                            tData = $('<tr>')
                                        .append($('<td>')
                                            .append($('<a href=#>')
                                                .attr('data', vRows[i].ProductFeature)
                                                .click(function () {
                                                    m_Filter1 = $(this).attr('data');
                                                    $('#panDice').popup('open');
                                                    $('#liThirdChartMainLabel').text('Showing Sales>' + $(this).attr('data') + '>');
                                                    ShowHideDiceList();
                                                })
                                                .text(vRows[i].ProductFeature)
                                            )
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text(AccountingFormat($.format.number(vRows[i].Total)))
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text(AccountingFormat((parseFloat(vRows[i].Total) / parseFloat(m_TotalGross)) * 100) + '%')
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text(vRows[i].TransactionCount)
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text(AccountingFormat(vRows[i].ProductCount))
                                        )

                            $('#tblSecondChart').append(tData);
                        }

                        tData = $('<tr>')
                                .append($('<td colspan="7">')
                                    .append($('<hr>'))
                                )

                        $('#tblSecondChart').append(tData);

                        tData = $('<tr>')
                                        .append($('<td>')
                                            .text('Total')
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text(AccountingFormat(m_TotalGross))
                                        )
                                        .append($('<td style="text-align:right">')
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text(m_TotalTransactionCount)
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text(AccountingFormat(m_TotalProductCount))
                                        )

                        $('#tblSecondChart').append(tData);

                        DisplayHorizontalBarGraph(m_Totals);
                    }
                }
            }); //end ajaxsend
        }

        function GetPOSSalesReportByDice(data, table, div) {
            var m_Totals = [];
            var m_TotalGross = 0;
            var m_TotalTransactionCount = 0;
            var m_TotalProductCount = 0;

            AjaxSend({
                url: 'DashboardsAndReports/GetPOSSalesReportBySlice',
                data: data,
                success: function (vData) {

                    $('#' + table).html('');
                    $('#' + div).empty();

                    if (vData.ReportPOS.Rows.length > 0) {

                        var vRows = vData.ReportPOS.Rows;

                        $('#' + div).css('visibility', 'visible');
                        $('#btnDownloadDiceAsExcel').css('visibility', 'visible');


                        m_TicksPie = [];
                        m_Labels = [];

                        var tData = '';

                        m_Totals[0] = [];

                        //additional by janine. show count. october 30 5pm

                        for (i = 0; i <= vRows.length - 1; i++) {

                            m_Totals[0][i] = $.format.number(vRows[i].TransactionCount, vRows[i].ProductFeature);

                            m_TotalTransactionCount = m_TotalTransactionCount + CDec2(vRows[i].TransactionCount);

                            m_TotalProductCount = m_TotalProductCount + CDec2(vRows[i].ProductCount);
                        }
                        //for chart, it will sort desc
                        m_Totals[1] = [];
                        for (i = 0; i <= vRows.length - 1; i++) {

                            m_Totals[1][i] = $.format.number(vRows[i].Total, vRows[i].ProductFeature);

                            m_TotalGross = m_TotalGross + $.format.number(vRows[i].Total);
                        }
                        //end of additional

                        tData = $('<tr>')
                                        .append($('<th>')
                                            .text(m_FilterLabel)
                                        )
                                        .append($('<th style="text-align:right">')
                                            .text('Gross Sales')
                                        )
                                        .append($('<th style="text-align:right">')
                                            .text('Contribution Margin')
                                        )
                                        .append($('<th style="text-align:right">')
                                            .text('Transaction Count')
                                        )
                                        .append($('<th style="text-align:right">')
                                            .text('Product Count')
                                        )
                                        .append($('<br />'))

                        $('#' + table).append(tData);

                        tData = $('<tr>')
                                .append($('<td colspan="6">')
                                    .append($('<br />'))
                                )

                        $('#' + table).append(tData);

                        for (var i = vRows.length - 1; i >= 0; i--) {
                            m_TicksPie[i] = vRows[i].ProductFeature;
                            m_Labels[i] = AccountingFormat($.format.number(vRows[i].Total));

                            tData = $('<tr>')
                                        .append($('<td>')
                                            .text(vRows[i].ProductFeature)
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text(AccountingFormat($.format.number(vRows[i].Total)))
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text(AccountingFormat((parseFloat(vRows[i].Total) / parseFloat(m_TotalGross)) * 100) + '%')
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text(vRows[i].TransactionCount)
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text(AccountingFormat(vRows[i].ProductCount))
                                        )

                            $('#' + table).append(tData);
                        }

                        tData = $('<tr>')
                                .append($('<td colspan="6">')
                                    .append($('<hr>'))
                                )

                        $('#' + table).append(tData);

                        tData = $('<tr>')
                                        .append($('<td>')
                                            .text('Total')
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text(AccountingFormat(m_TotalGross))
                                        )
                                        .append($('<td style="text-align:right">')
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text(m_TotalTransactionCount)
                                        )
                                        .append($('<td style="text-align:right">')
                                            .text(AccountingFormat(m_TotalProductCount))
                                        )

                        $('#' + table).append(tData);
                        DisplayPieGraph(m_Totals);
                    }
                }
            }); //end ajaxsend
        }

        function DisplayPieGraph(m_Totals) {


            //            var plot1 = $.jqplot('dvThirdChart', m_Totals, {
            //                seriesDefaults: {
            //                    renderer: jQuery.jqplot.PieRenderer,
            //                    rendererOptions: { showDataLabels: true, labels: m_TicksPie }
            //                },
            //                legend: { show: true, location: 'e' }
            //            });
            var plot1 = $.jqplot('dvThirdChart', m_Totals, {
                animate: !$.jqplot.use_excanvas,
                seriesDefaults: {
                    renderer: $.jqplot.BarRenderer,
                    pointLabels: {
                        show: true
                        //                        labels: m_Labels
                    },
                    rendererOptions: {
                        barDirection: 'horizontal'
                    }
                },
                axes: {
                    xaxis: {
                        drawBaseline: false
                    },
                    yaxis: {
                        renderer: $.jqplot.CategoryAxisRenderer,
                        ticks: m_TicksPie
                    }
                },
                highlighter: { show: false }
            });

            $('#dvThirdChart .jqplot-xaxis-tick').css('display', 'none');
        }



        function DisplayLineGraph(m_Totals) {

            var plot1 = $.jqplot('dvMainChart', m_Totals, {
                animate: true,
                animateReplot: true,
                axes: {
                    xaxis: {
                        renderer: $.jqplot.CategoryAxisRenderer,
                        ticks: m_Ticks
                    },
                    yaxis: {
                        min: 0
                        //max: 500000
                    }
                },
                highlighter: {
                    show: true,
                    showLabel: true,
                    tooltipAxes: 'y',
                    sizeAdjust: 7.5, tooltipLocation: 'ne',
                    tooltipContentEditor: function (str, seriesIndex, pointIndex, jqPlot) {
                        return AccountingFormat(m_Totals[seriesIndex][pointIndex]);
                    }
                }
            });

            $('#tblDateFormatLabel').css('visibility', 'visible')
        }


        function DisplayVerticalBarGraph(m_Totals) {

            //$.jqplot.config.enablePlugins = true;

            var plot2 = $.jqplot('dvMainChart', m_Totals, {
                animate: !$.jqplot.use_excanvas,
                seriesDefaults: {
                    renderer: $.jqplot.BarRenderer,
                    pointLabels: {
                        show: true,
                        labels: m_Labels
                    }
                },
                axes: {
                    xaxis: {
                        renderer: $.jqplot.CategoryAxisRenderer,
                        ticks: m_Ticks
                    },
                    yaxis: {

                }
            },
            highlighter: { show: false }
        });
    }

    //for 2nd chart
    function DisplayHorizontalBarGraph(m_Totals) {

        $('#dvSecondChart').empty('');

        var plot3 = $.jqplot('dvSecondChart', m_Totals, {
            animate: !$.jqplot.use_excanvas,
            seriesDefaults: {
                renderer: $.jqplot.BarRenderer,
                pointLabels: {
                    show: true
                },
                rendererOptions: {
                    barDirection: 'horizontal'
                }
            },
            axes: {
                xaxis: {
                    drawBaseline: false
                },
                yaxis: {
                    renderer: $.jqplot.CategoryAxisRenderer,
                    ticks: m_TicksBar
                }
            },
            highlighter: { show: false }
        });

        $('#dvSecondChart .jqplot-xaxis-tick').css('display', 'none');
    }


    function GetSelectedFowst() {
        var Fowst = '';
        var FowstCount = 0;

        $('#tblFOWST tr td input').each(function (m_index) {
            if ($(this).attr("checked") == "checked") {
                Fowst += $(this).attr("ID") + "|";
                FowstCount++;
            }
        });

        if (FowstCount == 0) {
            return 0;
        }
        else {
            return Fowst
        }
    }

    function ClearCharts() {
        $('#dvMainChart').empty();
        $('#dvSecondChart').empty();
        $('#dvThirdChart').empty();

        $('#tblSecondChart').empty();
        $('#tblThirdChart').empty();

        $('#tdTotalGrossSales').html("0");
        $('#tdSalesToday').html("0");

        $('#dvSlice').css('visibility', 'hidden');
        $('#dvDice').css('visibility', 'hidden');

        $('#btnDownloadSliceAsExcel').css('visibility', 'hidden');
        $('#btnDownloadDiceAsExcel').css('visibility', 'hidden');

        $('#selSlice').val('');
        $('#selDice').val('');

        $('#selSlice').selectmenu('refresh');
        $('#selDice').selectmenu('refresh');

        $('#tblSalesOrder').css('visibility', 'hidden');
        $('#tblSalesOrderWeek').css('visibility', 'hidden');
    }

    function GetSign() {
        switch (GetUrlParameters().ProductClassFeature) {
            case 'optSalesValue': return 'P'; break;
            case 'optSalesVolume': return ''; break;
            case 'optHits': return 'Hits'; break;
        }

    }

});

