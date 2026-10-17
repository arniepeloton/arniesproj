require([
        'common',
        'promise!/Scripts3/Browser/LibraryFowst.js?crc='
    ], function () {
        var m_Data;
        var m_DashBoardChartsData;
        var GroupDateBy;
        var IsSalesDistribution = true;

        var GroupDateBy; //para madisplay ang date sa chart
        var m_Ticks;
        var m_TicksBar;
        var m_Labels;
        var m_Slice;
        var m_Dice;

        $(function () {


            //$('#pageMain').attr('class', 'ui-page ui-body-c ui-page-active');
            if (!CBool(sessionStorage.PrintingOfflineData)) {
                GetAllFeatures();
            }

            sessionStorage.SearchMode = 'false';
            $('.txtSearch').val('');


            $('#liSecondChart').click(function () {
                $('#panSalesDistribution').popup('open');
            });


            $('#liThirdChartLabel').click(function () {
                $('#panDice').popup('open');
            });

            $('#dvMainChart').click(function () {
                if (IsSalesDistribution) {
                    $('#panSalesDistribution').popup('open');
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

            $('#selCashier').change(function () {
                m_DashBoardChartsData.CashierID = $('#selCashier').val();
                GetPOSSalesInvoiceHeader(m_DashBoardChartsData);
            })

            //dire ni sya kay ang value sa fowst depends on clicking sa chart
            $('#lnkTransactionDetails').click(function () {
                $('#txtSearchProduct').val('');
                //$('#divTransDetails').hide();
                GetPOSSalesInvoiceHeader(m_DashBoardChartsData);
                $('#chbTransViewOpt').prop('checked', false).checkboxradio('refresh');
                //GetTransactionDetails(m_DashBoardChartsData);
            });

            //dire ni sya kay ang value sa fowst depends on clicking sa chart
            $('#txtSearchProductTransDetails').keypress(function (e) {
                if (e.keyCode == 13) {
                    m_DashBoardChartsData.IsSearch = 1;
                    m_DashBoardChartsData.SearchString = $('#txtSearchProductTransDetails').val();
                    GetTransactionDetails(m_DashBoardChartsData);
                }
            });

            $('#txtSearchHeader').keypress(function (e) {
                if (e.keyCode == 13) {
                    m_DashBoardChartsData.IsSearch = 1;
                    m_DashBoardChartsData.SearchString = $('#txtSearchHeader').val();
                    GetPOSSalesInvoiceHeader(m_DashBoardChartsData);
                }
            });

            $('.txtSearch').keypress(function (e) {
                if (e.keyCode == 13) {
                    $('.btnSearch').click();
                }
            });

            $('.btnSearch').click(function () {

                BrowseAddresses();

            })
            //$('.txtSearch').keypress(function (e) {
            //    if (e.keyCode == 13) {

            //BrowseAddresses();

            //    }
            //});


        });

        window.GetPOSSalesReportByTrendOrSalesDistribution = function (data) {
            data.FowstID = data.Fowst //para makuha sa WebRequest.Instance.FowstID
            m_DashBoardChartsData = data;
            GroupDateBy = 'Day';
            ClearCharts();

            AjaxSend({
                url: 'DashboardsAndReports/GetPOSSalesReportByTrendOrSalesDistribution',
                data: data,
                success: function (vData) {
                    if (vData.ReportPOS.Rows.length > 0) {
                        $('#IsDataPresent').css('visibility', 'hidden');
                        $('#lblMainChartInstruction').css('visibility', 'visible');
                        $('#lblMainChartInstruction').text('* Click the chart to breakdown data');

                        m_Data = vData.ReportPOS.Rows;
                        DisplayTrend();
                    }
                    else {
                        $('#IsDataPresent').css('visibility', 'visible');
                        $('#lblMainChartInstruction').css('visibility', 'hidden');


                    }
                }
            }); //end ajaxsend
        }

        function BrowseAddresses() {

            Browser.LibraryFowst.InputData.OrganizationID = sessionStorage.OrgID;
            Browser.LibraryFowst.InputData.FilterFowst = 1;
            Browser.LibraryFowst.Click = function (e) {
                sessionStorage.FowstID = e.ID;
                sessionStorage.SearchMode = 'true';
                $('#txtSearchProduct').val('');
                window.GetData();
            };
            Browser.LibraryFowst.Show();

        };

        function DisplayTrend() {
            var m_Totals = [];
            var m_Days = [];
            var m_Months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
            var TotalSales = 0;
            var TotalForegoneVAT = 0;
            var TotalSalesToday = 0;
            var TotalDiscounts = 0;
            var tData;

            m_Totals[0] = [];

            var pLV = $('#tblMainChart').html('');

            tData = $('<tr class="custom-theme-f">')
                            .append($('<th style="text-align:left">')
                                .text('Date')
                            )
                            .append($('<th style="text-align:right">')
                                .text('Gross Sales')
                            )
                            .append($('<th style="text-align:right">')
                                .text('Discount')
                            )
                            .append($('<th style="text-align:right">')
                                .text('Vat Exempt Tax')
                            )
                            .append($('<th style="text-align:right">')
                                .text('Refunds and Exchanges')
                            )
                            .append($('<th style="text-align:right">')
                                .text('Net Sales')
                            )
                            .append($('<br />'))

            pLV.append(tData);

            if (m_Data.length > 0) {

                for (var i = 0; i < m_Data.length; i++) {

                    var m_Date = new Date(m_Data[i].InvoiceDate);
                    var m_Today = new Date();

                    m_Totals[0][i] = $.format.number(m_Data[i].Total);
                    TotalSales = TotalSales + parseFloat(m_Data[i].Total);
                    TotalForegoneVAT = TotalForegoneVAT + parseFloat(m_Data[i].ForegoneVAT);
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

                    tData = $('<tr>')
                            .append($('<th style="text-align:left">')
                                .text(m_Months[m_Date.getMonth()] + ' ' + m_Date.getDate() + ', ' + m_Date.getFullYear())
                            )
                            .append($('<th style="text-align:right">')
                                .text(AccountingFormat(m_Data[i].Total))
                            )
                            .append($('<th style="text-align:right">')
                                .text(AccountingFormat(m_Data[i].DiscountTotal))
                            )
                            .append($('<th style="text-align:right">')
                                .text(AccountingFormat(m_Data[i].ForegoneVAT))
                            )
                            .append($('<th style="text-align:right">')
                    //returns
                                .text('')
                            )
                            .append($('<th style="text-align:right">')
                                .text(AccountingFormat(parseFloat(m_Data[i].Total) - parseFloat(m_Data[i].DiscountTotal) - parseFloat(m_Data[i].ForegoneVAT)))
                            )
                            .append($('<br />'))

                    pLV.append(tData);

                }

                tData = $('<tr>')
                            .append($('<th style="text-align:left; font-size:x-large; font-weight:bolder">')
                                .text('Total')
                            )
                            .append($('<th style="text-align:right; font-size:x-large; font-weight:bolder">')
                                .text(AccountingFormat(TotalSales))
                            )
                            .append($('<th style="text-align:right; font-size:x-large; font-weight:bolder">')
                                .text(AccountingFormat(TotalDiscounts))
                            )
                            .append($('<th style="text-align:right; font-size:x-large; font-weight:bolder">')
                                .text(AccountingFormat(TotalForegoneVAT))
                            )
                            .append($('<th style="text-align:right; font-size:x-large; font-weight:bolder">')
                //returns
                                .text('')
                            )
                            .append($('<th style="text-align:right; font-size:x-large; font-weight:bolder">')
                                .text(AccountingFormat(parseFloat(TotalSales) - parseFloat(TotalDiscounts) - parseFloat(TotalForegoneVAT)))
                            )
                            .append($('<br />'))

                pLV.append(tData);

                $('#lblMainChartInstruction').css('visibility', 'visible');
                $('#lblMainChartInstruction').text('* Click the chart to breakdown data');

                m_Ticks = m_Days;

                DisplayLineGraph(m_Totals);
            }
        }

        function DisplayLineGraph(m_Totals) {

            $('#dvMainChart').empty();

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

            //$('#tblDateFormatLabel').css('visibility', 'visible')
        }

        function GetAllFeatures() {
            AjaxSend({
                url: 'DashboardsAndReports/GetOrganizationProductFeatures',
                success: function (vData) {
                    DisplayOrganizationProductFeatures(vData.Results.Rows);
                }
            });
        }

        function DisplayOrganizationProductFeatures(vRows) {
            var pLV = $('#lstSlice');
            var pLV2 = $('#lstDice');
            var pItem;

            //time slice
            pItem = $('<li>')
                .append($('<a class="custom-btn-theme-c">')
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
                .append($('<a class="custom-btn-theme-c">')
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

            //brand slice
            pItem = $('<li>')
                .append($('<a class="custom-btn-theme-c">')
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
                .append($('<a class="custom-btn-theme-c">')
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
            //            pItem = $('<li>')
            //                .append($('<a class="custom-btn-theme-c">')
            //                    .attr('ID', 'MinchBrand')
            //                    .attr('name', 'Brand')
            //                    .click(function (e) {
            //                        MinchData($(this).attr('name'));
            //                        $('#panMinch').popup('close');
            //                    })
            //                    .append($('<h3>')
            //                        .text('Brand')
            //                    )
            //                );

            //            pLV3.append(pItem);

            //sales associate slice
            pItem = $('<li>')
                .append($('<a class="custom-btn-theme-c">')
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
                                .append($('<a class="custom-btn-theme-c">')
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
                                .append($('<a class="custom-btn-theme-c">')
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
                                .append($('<a class="custom-btn-theme-c">')
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
                                .append($('<a class="custom-btn-theme-c">')
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
                                .append($('<a class="custom-btn-theme-c">')
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
                                .append($('<a class="custom-btn-theme-c">')
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
                                .append($('<a class="custom-btn-theme-c">')
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
                                .append($('<a class="custom-btn-theme-c">')
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
                                .append($('<a class="custom-btn-theme-c">')
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
                                .append($('<a class="custom-btn-theme-c">')
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
                                .append($('<a class="custom-btn-theme-c">')
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
                                .append($('<a class="custom-btn-theme-c">')
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
                                .append($('<a class="custom-btn-theme-c">')
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

            //CUSTOMER 
            pItem = $('<li>')
                .append($('<a class="custom-btn-theme-c">')
                    .attr('ID', 'SliceCustomer')
                    .attr('name', 'Customer')
                    .click(function (e) {
                        GetPOSSalesReportByCustomer();
                        $('#panSalesDistribution').popup('close');
                    })
                    .append($('<h3>')
                        .text('Customer')
                    )
                );
            pLV.append(pItem);

            pItem = $('<li>')
                .append($('<a class="custom-btn-theme-c">')
                    .attr('ID', 'SliceCustomer')
                    .attr('name', 'Customer')
                    .click(function (e) {
                        GetPOSSalesReportByCustomer();
                        $('#panDice').popup('close');
                    })
                    .append($('<h3>')
                        .text('Customer')
                    )
                );

            pLV2.append(pItem);
            //end of additional

            for (i = 0; i <= vRows.length - 1; i++) {
                pItem = $('<li>')
                        .append($('<a class="custom-btn-theme-c">')
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
                        .append($('<a class="custom-btn-theme-c">')
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
                //                pLV3.listview('refresh');
            }
            catch (e) {
                pLV.listview();
                pLV2.listview();
                //                pLV3.listview();
            }
        }

        function SliceData(SliceBy) {
            delete m_DashBoardChartsData["Filter1"];
            delete m_DashBoardChartsData["Filter1Option"];

            switch (SliceBy) {
                case "Day":
                    m_DashBoardChartsData.SliceBy = 'Day';
                    break;
                case "Time":
                    m_DashBoardChartsData.SliceBy = 'optTime';
                    window.location = '#dvSlice';
                    break;
                case "Brand":
                    m_DashBoardChartsData.SliceBy = 'optBrand';
                    window.location = '#dvSlice';
                    break;
                case "Division":
                    m_DashBoardChartsData.SliceBy = 'optDivisionDescription';
                    break;
                //additional by janine oct 30 12pm                       
                case "Product":
                    m_DashBoardChartsData.SliceBy = 'optProduct';
                    break;
                case "Delivery Method":
                    m_DashBoardChartsData.SliceBy = 'optDeliveryMethod';
                    break;
                case "Order Method":
                    m_DashBoardChartsData.SliceBy = 'optOrderMethod';
                    break;
                case "Gender":
                    m_DashBoardChartsData.SliceBy = 'optGender';
                    break;
                case "Age":
                    m_DashBoardChartsData.SliceBy = 'optAge';
                    break;
                //end of additional                 
                case "Season/Year":
                    m_DashBoardChartsData.SliceBy = 'Season/Year';
                    break;
                case "SalesRepresentative":
                    m_DashBoardChartsData.SliceBy = 'optSalesRepresentative';
                    break;
                default:
                    m_DashBoardChartsData.SliceBy = SliceBy;
                    break;
            }

            window.location = '#dvSlice';

            $('#dvSlice').css('display', 'block');

            $('#tblThirdChart').empty();
            $('#dvThirdChart').empty();
            $('#dvDice').css('display', 'none');

            $('#liSecondChartMainLabel').text('Showing Sales>');
            $('#liSecondChart').text(SliceBy);
            $('#lblSecondChartInstruction').text('* Click on a ' + SliceBy + ' to break down data')

            GetPOSSalesReportBySlice();
        }

        var dataTableSecondChart;

        function GetPOSSalesReportByCustomer() {
            var pData = {
                Fowst: m_DashBoardChartsData.Fowst,
                From: m_DashBoardChartsData.From,
                To: m_DashBoardChartsData.To
            };
            AjaxSend({
                url: 'DashboardsAndReports/GetPOSSalesReportByCustomer',
                data: pData,
                success: function (vData) {
                    DisplaySalesReportByCustomer(vData);
                    
                }
            }); //end ajaxsend

        }  // END function GetPOSSalesReportByCustomer() {

        function DisplaySalesReportByCustomer(vData) {

            var vRows = vData.ReportPOS.Rows;

            $('#btnDownloadSliceAsExcel').css('visibility', 'visible');
            $('#tblSecondChart').html('');

            if ($.fn.DataTable.isDataTable($('#tblSecondChart'))) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                $('#tblSecondChart').empty();
                dataTableSecondChart.destroy();
            }

            var buttons = [];
            if (!IsMobile) {
                buttons.push({ extend: 'csvHtml5', text: 'Download CSV', title: 'Sasles Report By Customer' });
            }

            dataTableSecondChart = $('#tblSecondChart').DataTable({
                pageLength: 25,
                //"order": [[ 4, "desc" ]],
                "data": vRows,
                "columns": [
                    {
                        title: "Customer",
                        "data": function (data) {
                            return CStr2(data.CompanyName);
                        }
                    },
                    {   
                        title: 'Gross Invoice Amount',
                        "data": function (data) {
                            return AccountingFormat(data.TotalAmount);
                        }
                    },
                    {   
                        title: 'Adjustments Amount',
                        "data": function (data) {
                            return AccountingFormat(data.AdjustmentAmount);
                        }
                    },
                    {   
                        title: 'Net Invoice Amount',
                        "data": function (data) {
                            return AccountingFormat(data.NetInvoiceAmount);
                        }
                    },
                    {   
                        title: 'Payment Amount',
                        "data": function (data) {
                            return AccountingFormat(data.TotalPayment);
                        }
                    },
                    {   
                        title: 'Net Invoice Balance',
                        "data": function (data) {
                            return AccountingFormat(data.NetInvoiceBalance);
                        }
                    }
                ],
                dom: 'Brtip',
                buttons: buttons
            });

            $('#tblSecondChart thead tr').addClass('custom-theme-f');
            $('#dvSlice').css('display', 'block');

            //dataTableSecondChart.on('click', 'td', function () {
            //    var data = dataTableSecondChart.row(this).data();
            //    if (data != undefined) {
            //        m_Filter1 = data.ProductFeature;
            //        $('#panDice').popup('open');
            //        $('#liThirdChartMainLabel').text('Showing Sales > ' + data.ProductFeature + ' > ');
            //        ShowHideDiceList();
            //    }
            //});
          
        } // END function DisplaySalesReportByCustomer(vData) {

        function GetPOSSalesReportBySlice() {
            var m_Totals = [];
            var m_TotalGross = 0;
            var m_TotalTransactionCount = 0;
            var m_TotalProductCount = 0;

            AjaxSend({
                url: 'DashboardsAndReports/GetPOSSalesReportBySlice',
                data: m_DashBoardChartsData,
                success: function (vData) {
                    if (vData.ReportPOS.Rows.length > 0) {

                        var vRows = vData.ReportPOS.Rows;

                        $('#btnDownloadSliceAsExcel').css('visibility', 'visible');
                        m_Totals[0] = [];
                        m_TicksBar = [];
                        m_Labels = [];

                        var tData = '';
                        var i = 0;

                        $('#tblSecondChart').html('');

                        if ($.fn.DataTable.isDataTable($('#tblSecondChart'))) {
                            //empty the table first before destroying datatabase para dili magdoble ang header
                            $('#tblSecondChart').empty();
                            dataTableSecondChart.destroy();
                        }

                        var buttons = [];
                        if (!IsMobile) {
                            buttons.push({ extend: 'csvHtml5', text: 'Download CSV', title: m_Slice + ' breakdown' })
                        }


                        m_Totals[1] = [];

                        if (m_Slice == 'Time') {
                            for (var j = 0; j < 24; j++) {
                                m_TicksBar[j] = j + ':00';
                                m_Labels[j] = 0;
                                m_Totals[0][j] = 0;
                                m_Totals[1][j] = 0;
                            }
                        }

                        //additional by janine. show count. october 30 5pm

                        for (i = 0; i <= vRows.length - 1; i++) {
                            if (m_Slice == 'Time') {
                                for (var j = 0;j<24;j++){
                                    if (vRows[i].ProductFeature == j + ':00') {
                                        m_Totals[0][j] = $.format.number(vRows[i].TransactionCount, vRows[i].ProductFeature);
                                    }
                                }
                            } else {
                                m_Totals[0][i] = $.format.number(vRows[i].TransactionCount, vRows[i].ProductFeature);

                                m_TotalTransactionCount = m_TotalTransactionCount + CDec2(vRows[i].TransactionCount);

                                m_TotalProductCount = m_TotalProductCount + (CDec2(vRows[i].ProductCount) == 0 ? CDec2(vRows[i].ProductCountSO) : CDec2(vRows[i].ProductCount));
                            }                            
                        }

                        //for chart, it will sort desc

                        for (i = 0; i <= vRows.length - 1; i++) {

                            if (m_Slice == 'Time') {
                                for (var j = 0; j < 24; j++) {
                                    if (vRows[i].ProductFeature == j + ':00') {
                                        m_Totals[1][j] = $.format.number(vRows[i].Total, vRows[i].ProductFeature);
                                    }
                                }
                            } else {
                                m_Totals[1][i] = $.format.number(vRows[i].Total, vRows[i].ProductFeature);                                
                            }

                            m_TotalGross = m_TotalGross + $.format.number(vRows[i].Total);
                        }
                        //end of additional

                        //manually sort desc for table details

                        dataTableSecondChart = $('#tblSecondChart').DataTable({
                            pageLength: 25,
                            "order": [[ 4, "desc" ]],
                            "data": vRows,
                            "columns": [
                                {
                                    title: m_Slice,
                                    "data": function (data) {
                                        return CStr2(data.ProductFeature);
                                    }
                                },
                                {   
                                    title: 'Gross Sales',
                                    "data": function (data) {
                                        return AccountingFormat($.format.number(data.Total))
                                    }
                                },
                                {
                                    title: 'Contribution Margin',
                                    "data": function (data) {
                                        return AccountingFormat((parseFloat(data.Total) / parseFloat(m_TotalGross)) * 100) + '%';
                                    }
                                },
                                {
                                    title: 'Transaction Count',
                                    "data": function (data) {
                                        return CDec2(data.TransactionCount);
                                    }
                                },
                                {
                                    title: 'Product Count',
                                    "data": function (data) {
                                        return CDec2((CDec2(data.ProductCount) == 0 ? data.ProductCountSO : data.ProductCount))
                                    }
                                }
                            ],
                            dom: 'Brtip',
                            buttons: buttons
                        });

                        $('#tblSecondChart thead tr').addClass('custom-theme-f');

                        dataTableSecondChart.on('click', 'td', function () {
                            var data = dataTableSecondChart.row(this).data();
                            if (data != undefined) {
                                m_Filter1 = data.ProductFeature;
                                $('#panDice').popup('open');
                                $('#liThirdChartMainLabel').text('Showing Sales > ' + data.ProductFeature + ' > ');
                                ShowHideDiceList();
                            }
                        });
                        
                        for (i = vRows.length - 1; i >= 0; i--) {
                            if (m_Slice == 'Time') {
                                for (var j = 0; j < 24; j++) {
                                    if (m_TicksBar[j] == vRows[i].ProductFeature) {
                                        m_Labels[j] = AccountingFormat($.format.number(vRows[i].Total));
                                    }
                                }
                            } else {
                                m_TicksBar[i] = vRows[i].ProductFeature;
                                m_Labels[i] = AccountingFormat($.format.number(vRows[i].Total));
                            }
                        }

                        if (m_Slice == 'Time') {
                            DisplaySliceBarGraphVertical(m_Totals);
                        } else {
                            DisplaySliceBarGraph(m_Totals);
                        }
                        
                    }
                }
            }); //end ajaxsend
        }

        //for 2nd chart
        function DisplaySliceBarGraph(m_Totals) {

            $('#dvSecondChart').empty('');

            var plot3 = $.jqplot('dvSecondChart', m_Totals, {
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
                        ticks: m_TicksBar
                    }
                },
                highlighter: { show: false }
            });

            $('#dvSecondChart .jqplot-xaxis-tick').css('display', 'none');
        }

        //for 2nd chart
        function DisplaySliceBarGraphVertical(m_Totals) {

            $('#dvSecondChart').empty('');

            var plot3 = $.jqplot('dvSecondChart', m_Totals, {
                animate: !$.jqplot.use_excanvas,
                seriesDefaults: {
                    renderer: $.jqplot.BarRenderer,
                    pointLabels: {
                        show: true
                    },
                    rendererOptions: {
                        barDirection: 'vertical'
                    }
                },
                axes: {
                    xaxis: {
                        renderer: $.jqplot.CategoryAxisRenderer,
                        ticks: m_TicksBar
                    },
                    yaxis: {
                        min: 0
                    }
                },
                highlighter: { show: true, showLabel: true } 
            });

        }

        function ShowHideDiceList() {
            $('#lstDice li a').each(function () {
                $(this).show();
            });

            $('#Dice' + m_Slice.replace(' ', '').replace('(', '').replace(')', '').replace('/', '').replace('\'', '').replace('Slice', '')).hide();

            //$('#lstDice').listview('refresh');
        }

        function DiceData(DiceBy, table, divChart, div) {
            var data;
            var Fowst;

            //Fowst = GetSelectedFowst();

            m_Dice = DiceBy;

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
                case "Time":
                    m_DashBoardChartsData.SliceBy = 'optTime';
                    m_DashBoardChartsData.Filter1Option = m_Slice;
                    m_DashBoardChartsData.Filter1 = m_Filter1;
                    window.location = '#' + divChart;
                    break;
                case "Brand":
                    m_DashBoardChartsData.SliceBy = 'optBrand';
                    m_DashBoardChartsData.Filter1Option = m_Slice;
                    m_DashBoardChartsData.Filter1 = m_Filter1;
                    window.location = '#' + divChart;
                    break;
                case "Division":
                    m_DashBoardChartsData.SliceBy = 'optDivisionDescription';
                    m_DashBoardChartsData.Filter1Option = m_Slice;
                    m_DashBoardChartsData.Filter1 = m_Filter1;
                    window.location = '#' + divChart;
                    break;
                case "SalesRepresentative":
                    m_DashBoardChartsData.SliceBy = 'optSalesRepresentative';
                    m_DashBoardChartsData.Filter1Option = m_Slice;
                    m_DashBoardChartsData.Filter1 = m_Filter1;
                    window.location = '#' + divChart;
                    break;
                //additional by janine nov 15               
                case "Product":
                    m_DashBoardChartsData.SliceBy = 'optProduct';
                    m_DashBoardChartsData.Filter1Option = m_Slice;
                    m_DashBoardChartsData.Filter1 = m_Filter1;
                    window.location = '#' + divChart;
                    break;
                case "Delivery Method":
                    m_DashBoardChartsData.SliceBy = 'optDeliveryMethod';
                    m_DashBoardChartsData.Filter1Option = m_Slice;
                    m_DashBoardChartsData.Filter1 = m_Filter1;
                    window.location = '#' + divChart;
                    break;
                case "Order Method":
                    m_DashBoardChartsData.SliceBy = 'optOrderMethod';
                    m_DashBoardChartsData.Filter1Option = m_Slice;
                    m_DashBoardChartsData.Filter1 = m_Filter1;
                    window.location = '#' + divChart;
                    break;
                case "Gender":
                    m_DashBoardChartsData.SliceBy = 'optGender';
                    m_DashBoardChartsData.Filter1Option = m_Slice;
                    m_DashBoardChartsData.Filter1 = m_Filter1;
                    window.location = '#' + divChart;
                    break;
                case "Age":
                    m_DashBoardChartsData.SliceBy = 'optAge';
                    m_DashBoardChartsData.Filter1Option = m_Slice;
                    m_DashBoardChartsData.Filter1 = m_Filter1;
                    window.location = '#' + divChart;
                    break;
                //end of additional               
                default:
                    m_DashBoardChartsData.SliceBy = DiceBy;
                    m_DashBoardChartsData.Filter1Option = m_Slice;
                    m_DashBoardChartsData.Filter1 = m_Filter1;
                    window.location = '#' + divChart;
            }

            $('#' + div).css('display', 'block');

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

            GetPOSSalesReportByDice('tblThirdChart', 'dvThirdChart');
        }

        var dataTableThirdChart;

        function GetPOSSalesReportByDice(table, div) {
            var m_Totals = [];
            var m_TotalGross = 0;
            var m_TotalTransactionCount = 0;
            var m_TotalProductCount = 0;

            AjaxSend({
                url: 'DashboardsAndReports/GetPOSSalesReportBySlice',
                data: m_DashBoardChartsData,
                success: function (vData) {

                    $('#' + table).html('');
                    $('#' + div).empty();

                    if (vData.ReportPOS.Rows.length > 0) {

                        var vRows = vData.ReportPOS.Rows;

                        $('#' + div).css('visibility', 'visible');
                        $('#btnDownloadDiceAsExcel').css('visibility', 'visible');

                        if ($.fn.DataTable.isDataTable($('#' + table))) {
                            //empty the table first before destroying datatabase para dili magdoble ang header
                            $('#' + table).empty();
                            dataTableThirdChart.destroy();
                        }

                        var buttons = [];
                        if (!IsMobile) {
                            buttons.push({ extend: 'csvHtml5', text: 'Download CSV', title: m_FilterLabel + ' breakdown' })
                        }

                        m_Totals[0] = [];
                        m_TicksPie = [];
                        m_Labels = [];

                        m_Totals[1] = [];

                        if (m_Dice == 'Time') {
                            for (var j = 0; j < 24; j++) {
                                m_TicksPie[j] = j + ':00';
                                m_Labels[j] = 0;
                                m_Totals[0][j] = 0;
                                m_Totals[1][j] = 0;
                            }
                        }

                        var tData = '';

                        //additional by janine. show count. october 30 5pm

                        for (i = 0; i <= vRows.length - 1; i++) {

                            if (m_Dice == 'Time') {
                                for (var j = 0; j < 24; j++) {
                                    if (vRows[i].ProductFeature == j + ':00') {
                                        m_Totals[0][j] = $.format.number(vRows[i].TransactionCount, vRows[i].ProductFeature);
                                    }
                                }
                            } else {

                                m_Totals[0][i] = $.format.number(vRows[i].TransactionCount, vRows[i].ProductFeature);
                            }

                            m_TotalTransactionCount = m_TotalTransactionCount + CDec2(vRows[i].TransactionCount);

                            m_TotalProductCount = m_TotalProductCount + (CDec2(vRows[i].ProductCount) == 0 ? CDec2(vRows[i].ProductCountSO) : CDec2(vRows[i].ProductCount));
                        }
                        //for chart, it will sort desc
                        //m_Totals[1] = [];
                        for (i = 0; i <= vRows.length - 1; i++) {
                            if (m_Dice == 'Time') {
                                for (var j = 0; j < 24; j++) {
                                    if (vRows[i].ProductFeature == j + ':00') {
                                        m_Totals[1][j] = $.format.number(vRows[i].Total, vRows[i].ProductFeature);
                                    }
                                }
                            } else {
                                m_Totals[1][i] = $.format.number(vRows[i].Total, vRows[i].ProductFeature);
                            }

                            m_TotalGross = m_TotalGross + $.format.number(vRows[i].Total);
                        }
                        //end of additional
                        
                        dataTableThirdChart = $('#' + table).DataTable({
                            pageLength: 25,
                            "order": [[4, "desc"]],
                            "data": vRows,
                            "columns": [
                                {
                                    title: m_FilterLabel,
                                    "data": function (data) {
                                        return CStr2(data.ProductFeature);
                                    }
                                },
                                {
                                    title: 'Gross Sales',
                                    "data": function (data) {
                                        return AccountingFormat($.format.number(data.Total))
                                    }
                                },
                                {
                                    title: 'Contribution Margin',
                                    "data": function (data) {
                                        return AccountingFormat((parseFloat(data.Total) / parseFloat(m_TotalGross)) * 100) + '%';
                                    }
                                },
                                {
                                    title: 'Transaction Count',
                                    "data": function (data) {
                                        return CDec2(data.TransactionCount);
                                    }
                                },
                                {
                                    title: 'Product Count',
                                    "data": function (data) {
                                        return CDec2((CDec2(data.ProductCount) == 0 ? data.ProductCountSO : data.ProductCount))
                                    }
                                }
                            ],
                            dom: 'Brtip',
                            buttons: buttons
                        });

                        $('#' + table + ' thead tr').addClass('custom-theme-f');

                        for (var i = vRows.length - 1; i >= 0; i--) {
                            if (m_Dice == 'Time') {
                                for (var j = 0; j < 24; j++) {
                                    if (m_TicksPie[j] == vRows[i].ProductFeature) {
                                        m_Labels[j] = AccountingFormat($.format.number(vRows[i].Total));
                                    }
                                }
                            } else {                                
                                m_Labels[i] = AccountingFormat($.format.number(vRows[i].Total));
                                m_TicksPie[i] = vRows[i].ProductFeature;
                            }
                        }

                        if (m_Dice == 'Time') {
                            DisplayDiceBarGraphVertical(m_Totals);
                        } else {
                            DisplayDiceBarGraph(m_Totals);
                        }
                    }
                }
            }); //end ajaxsend
        }

        function DisplayDiceBarGraph(m_Totals) {
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

        function DisplayDiceBarGraphVertical(m_Totals) {

            $('#dvThirdChart').empty('');

            var plot3 = $.jqplot('dvThirdChart', m_Totals, {
                animate: !$.jqplot.use_excanvas,
                seriesDefaults: {
                    renderer: $.jqplot.BarRenderer,
                    pointLabels: {
                        show: true
                    },
                    rendererOptions: {
                        barDirection: 'vertical'
                    }
                },
                axes: {
                    xaxis: {
                        renderer: $.jqplot.CategoryAxisRenderer,
                        ticks: m_TicksPie
                    },
                    yaxis: {
                        min: 0
                    }
                },
                highlighter: { show: true, showLabel: true }
            });

        }

        window.ClearCharts = function () {

            $('#dvSalesTrends').css('visibility', 'hidden');
            $('#dvSalesTrends').css('display', 'none');

            $('#dvMainChart').empty();
            $('#dvSecondChart').empty();
            $('#dvThirdChart').empty();

            $('#tblSecondChart').empty();
            $('#tblThirdChart').empty();

            $('#tdTotalGrossSales').html("0");
            $('#tdSalesToday').html("0");

            $('#dvSlice').css('display', 'none');
            $('#dvDice').css('display', 'none');

            $('#btnDownloadSliceAsExcel').css('visibility', 'hidden');
            $('#btnDownloadDiceAsExcel').css('visibility', 'hidden');

            $('#selSlice').val('');
            $('#selDice').val('');

            $('#selSlice').selectmenu('refresh');
            $('#selDice').selectmenu('refresh');

            $('#tblSalesOrder').css('visibility', 'hidden');
            $('#tblSalesOrderWeek').css('visibility', 'hidden');

            $('#IsDataPresent').html('-Graph will be shown when you click on a group or FOWST (Factory, Office, Store, Warehouse or Truck) name-')
            $('#IsDataPresent').css('visibility', 'visible');
            $('#IsDataPresent').css('lnkTransactionDetails', 'hidden');
        }



        window.GetSearchData = function (data) {

            m_DashBoardChartsData = data;
            GetTransactionDetails(data);
        }


    });