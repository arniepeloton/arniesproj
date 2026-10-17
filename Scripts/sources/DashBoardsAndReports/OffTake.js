require([
    'common', 'tablesort'
    ], function () {

        var ID = '';

        var m_DateFrom;
        var m_DateTo;
        var m_Period = '';
        var m_Data;
        var m_IsDateRange = 0;
        var Fowst = '';

        PageShow();

        $(function () {

            $('#btnHome').click(function (e) {
                e.preventDefault();
                window.location = '/People/PeopleMenu';
            });

            $('#btnPeriod').click(function () {

                $('#selPeriod').val('');
                try {
                    $('#selPeriod').selectmenu('refresh');
                }
                catch (e) {
                    $('#selPeriod').selectmenu();
                }

                $('#panDates').popup('open');
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

            $('#liYearToDate').click(function () {
                $('#panDates').popup('close');
                $('#lblPeriod').text('[ ' + $('#liYearToDate').text() + ']');
                SetPeriod('YearToDate', 0);
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

            $('#btnDoneCustomPeriod').click(function () {
                $('#panCustomPeriod').popup('close');
                $('#lblPeriod').text('[ ' +
                $.format.date($('#txtDateFrom').DateCombo('val'), 'MM.dd.yy') + '--' +
                $.format.date($('#txtDateTo').DateCombo('val'), 'MM.dd.yy') +
                ' ]');
                SetPeriod('CustomPeriod', 1);
                SetSettings();
            });

            $('#selPeriod').change(function () {
                $('#panDates').popup('close');

                if ($('#selPeriod').val() == 'CustomPeriod') {
                    $('#panDates').popup('close');
                    $('#panCustomPeriod').popup('open');
                }
                else {
                    $('#lblPeriod').text('[ ' + $('#selPeriod :selected').text() + ']');
                    SetPeriod($('#selPeriod').val(), 0);
                    SetSettings();
                }
            });

            //            $('#selPeriod:option').click(function (e) {
            //                //if (e.target.tagName == "option") {
            //                    $('#panDates').popup('close');

            //                    if ($('#selPeriod').val() == 'CustomPeriod') {
            //                        $('#panDates').popup('close');
            //                        $('#panCustomPeriod').popup('open');
            //                    }
            //                    else {
            //                        $('#lblPeriod').text('[ ' + $('#selPeriod :selected').text() + ']');
            //                        SetPeriod($('#selPeriod').val(), 0);
            //                        SetSettings();
            //                    }
            //                //};
            //            });

            $('#btnGraphType').click(function () {
                $('#panSettings').popup('open');
            });

            $('#btnFOWST').click(function () {
                $('#panFOWST').popup('open');
            });

            $('#btnDoneSettings').click(function () {
                SetSettings();
            });

            $('#btnGo').click(function () {
                GetOfftake();
                GetFowstBin();
            });

            $('#btnSearchProductOrBin').click(function () {
                SearchProductOrBin();
            });

            $('#txtSearch').keypress(function (e) {
                if (e.keyCode == 13) {
                    SearchProductOrBin();
                }
            });
        });

        function PageShow() {

            $("[data-role=header]").fixedtoolbar({ tapToggle: false });

            var pPage = $.mobile.activePage;
            var pHeader = pPage.find("[data-role=header]");
            var pHeight = pHeader.height();

            pPage.css('padding-top', pHeight + 'px');

            pHeader.fixedtoolbar({ tapToggle: false,
                updatePagePadding: false    // http://jquerymobile.com/test/docs/toolbars/bars-fixed-options.html
                // http://stackoverflow.com/questions/10545540/jquery-mobile-disable-tap-to-toggle-fixed-header-and-footer
            });

            if (ValidPrevPage(arguments)) {
                GetFowst();
                SetPeriod('Today', 0);
                $('#lblPeriod').text('[ Today ]');
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

            //$('#lblGraphType').text('[ Trend ]');
            $('#lblGraphTypeDetail').text('[ ' + $('#selSettingsDetails :selected').text() + ' ]');
        }

        function SetPeriod(mPeriod, mIsDateRange) {
            m_Period = mPeriod;
            m_IsDateRange = mIsDateRange;
        }

        function GetFowst() {
            var data = { FilterFowst: 0
            }

            AjaxSend({
                url: 'DashboardsAndReports/GetFowstFromLedger',
                data: data,
                success: function (vData) {
                    DisplayFowst(vData.Results.Rows);
                }
            });
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
                                Fowst = $(this).data('data').ID;
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
            //var Fowst = '';

            if (sessionStorage.FowstID != undefined) {
                data = { GroupBy: 'Day',
                    Fowst: sessionStorage.FowstID,
                    From: (m_Date.getMonth() + 1) + '/' + m_Date.getDate() + '/' + m_Date.getFullYear(),
                    To: (m_Date.getMonth() + 1) + '/' + m_Date.getDate() + '/' + m_Date.getFullYear()
                };

                //GetPOSSalesReportByTrendOrSalesDistribution(data);
            }
        }

        function GetOfftake() {
            var Fowst;
            m_DateFrom = new Date();
            m_DateTo = new Date();

            Fowst = GetSelectedFowst();

            if (Fowst == 0) {
                MessageBox("Please select FOWST", "FOWST",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                            );
                return;
            }

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
            m_Data = {
                Fowst: Fowst,
                From: m_DateFrom,
                To: m_DateTo,
                IsRawData: 0
            };
            GetOffTakeData();
        }

        function GetOffTakeData() {
            AjaxSend({
                url: 'DashboardsAndReports/GetOffTake',
                data: m_Data,
                success: function (vData) {
                    if (vData.Results.Rows.length > 0) {
                        DisplayOffTakeData(vData.Results.Rows);
                        $('#IsDataPresent').css('visibility', 'hidden');
                        $('#dvOffTakeDetails').css('visibility', 'visible');
                    }
                    else {
                        $('#IsDataPresent').css('visibility', 'visible');
                        $('#dvOffTakeDetails').css('visibility', 'hidden');
                    }

                }
            }); //end ajaxsend
        }

        function DisplayOffTakeData(vRows) {
            $('#tblDetails').html('');

            var ProductID = '';
            var BinID = '';
            var ProductName = '';
            var BinName = '';
            var Total = 0;
            var RecordHasProblems = false;
            var tData;

            ProductID = vRows[0].ProductID;
            BinID = vRows[0].BinID;
            ProductName = vRows[0].ProductName;
            BinName = vRows[0].BinName;

            for (i = 0; i <= vRows.length - 1; i++) {

                if ((BinID != vRows[i].BinID) || (ProductID != vRows[i].ProductID)) {

                    AppendToTableDetails(RecordHasProblems, ProductName, BinName, Total, vRows[i - 1]);

                    ProductID = vRows[i].ProductID;
                    BinID = vRows[i].BinID;
                    ProductName = vRows[i].ProductName;
                    BinName = vRows[i].BinName;
                    RecordHasProblems = false;
                    Total = 0;
                }

                Total += parseFloat(vRows[i].Quantity)

                //flag if Qty is positive
                if (parseFloat(vRows[i].Quantity) < 0) {
                    RecordHasProblems = true;
                }

                if (i == vRows.length - 1) {
                    AppendToTableDetails(RecordHasProblems, ProductName, BinName, Total, vRows[i]);
                }
            }
        }

        function AppendToTableDetails(RecordHasProblems, ProductName, BinName, Total, vRows) {
            if (RecordHasProblems) {
                Total = '***';
            }
            tData = $('<tr>')
                            .append($('<td style="text-align:left">')
                                .text(ProductName)
                            )
                            .append($('<td style="text-align:left">')
                                .text(BinName)
                            )
                            .append($('<td style="text-align:right">')
                                .text(Total)
                            )
                            .data('data', vRows)
                            .attr('ID', vRows.ID)
                            .click(function () {
                                m_Data.BinID = $(this).data('data').BinID;
                                m_Data.ProductID = $(this).data('data').ProductID;
                                GetProductOfftakeDetails();
                            });
            $('#tblDetails').append(tData);
        }

        function GetProductOfftakeDetails() {
            AjaxSend({
                url: 'DashboardsAndReports/GetProductOfftakeDetails',
                data: m_Data,
                success: function (vData) {
                    if (vData.Results.Rows.length > 0) {
                        DisplayProductOfftakeDetails(vData.Results.Rows);
                    }
                }
            }); //end ajaxsend
        }

        function DisplayProductOfftakeDetails(vRows) {
            $('#tblOfftakeDetailsBody').html('');

            $('#lblProductName').html(vRows[0].ProductName);
            $('#lblFOWSTName').html(vRows[0].FOWSTName);
            $('#lblBin').html(vRows[0].BinName);

            for (i = 0; i <= vRows.length - 1; i++) {
                tData = $('<tr>')
                    .append($('<td style="text-align:left">')
                        .text(vRows[i].TransactionType)
                    )
                    .append($('<td style="text-align:left">')
                        .text(vRows[i].LastUpdated)
                    )
                    .append($('<td style="text-align:right">')
                        .text(parseInt(vRows[i].Qty))
                    )
                    .append($('<td style="text-align:right">')
                        .text(parseInt(vRows[i].Balance))
                    )
                $('#tblOfftakeDetailsBody').append(tData);
            }

            $.mobile.changePage('#pageTransactionOfftakeDetails', 'pop', true, true);

            try {
                $('#tblOfftakeDetails').table('refresh');
            }
            catch (e) {
                $('#tblOfftakeDetails').table();
            }
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

        function GetFowstBin() {
            AjaxSend({
                url: 'DashboardsAndReports/GetFowstBin',
                data: m_Data,
                success: function (vData) {
                    if (vData.Results.Rows.length > 0) {
                        DisplayFowstBin(vData.Results.Rows);
                    }
                }
            }); //end ajaxsend
        }

        function DisplayFowstBin(vRows) {
            $('#selBin').empty();
            $('#selBin').append(new Option('All Bins', 'All', false));
            for (i = 0; i <= vRows.length - 1; i++) {
                $('#selBin').append(new Option(vRows[i].Name, vRows[i].ID, false));
            }

            try {
                $('#selBin').selectmenu('refresh');
            }
            catch (e) {
                $('#selBin').selectmenu();
            }
        }

        function SearchProductOrBin() {
            m_Data.IsRawData = 1;
            m_Data.ProductName = $('#txtSearch').val();
            m_Data.BinID = $('#selBin').val();
            GetOffTakeData();
        }

        //from janine's code for sorting column headers

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



            switch (data.column) {
                case 1:
                    $("#lblColumnName").text('Product');
                    break;
                case 2:
                    $("#lblColumnName").text('Bin');
                    break;
                case 3:
                    $("#lblColumnName").text('Offtake');
                    break;
            }



        });

        $("tr").slice(1).click(function () {

            $(".awesome").removeClass("awesome");
            $(this).addClass("awesome");
        });


        //end of stupid table
    });