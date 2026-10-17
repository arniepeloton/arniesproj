require([
    'common',
     'tablesort',
    'jquery.plugin.html2canvas',
    'promise!/Scripts3/Browser/LibraryFowst.js?crc=',
    GetPathFormat('/Scripts3/Browser/LibraryPeople.js?crc='),
], function () {

    var countFowst = 0;
    var newRangeType = "";
    var newdateFrom = new Date();
    var newdateTo = new Date();
    var dateFrom;
    var dateTo;

    IsDownloadExcelFile = 0;

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

    $(function () {

        $(".CashierfromCalendar").calendar({
            format: "mm/dd/yyyy",
            multiSelect: false,
            otherDays: true,
            getDates: function (data) {
                $("#txtCashierDateFrom").text(data);
            }
        });

        $(".CashiertoCalendar").calendar({
            format: "mm/dd/yyyy",
            multiSelect: false,
            otherDays: true,
            getDates: function (data) {
                $("#txtCashierDateTo").text(data);
            }
        });
        
        $('body')
        .on('click', '#btnInventoryFormFowst', function () {
            GetAllFOWST();
        }).on('click', '#btnCashierDateRange', function () {
            PopupPeriod({
                Display2DaysAgo: false,
                DisplayYesterday: true,
                DisplayToday: true,
                DisplayWeekToDate: false,
                DisplayMonthToDate: false,
                DisplayCustomPeriod: true,
                Click: function (e) {
                    if (e.Period == 'Yesterday') {
                        rangeType = "Yesterday";
                        var date = $.format.date(new Date(e.From), 'MM/d/yyyy');
                        dateFrom = date;
                        dateTo = date;
                        $('#btnCashierDateRange').text('Yesterday');
                        getInventoryForm();
                    } else if (e.Period == 'Today') {
                        rangeType = "Today";
                        var date = $.format.date(new Date(e.From), 'MM/d/yyyy');
                        dateFrom = date;
                        dateTo = date;
                        $('#btnCashierDateRange').text('Today');
                        getInventoryForm();
                    } else if (e.Period == 'Custom') {
                        dateFrom = $.format.date(new Date(e.From), 'MM/dd/yyyy');
                        dateTo = $.format.date(new Date(e.To), 'MM/dd/yyyy');
                        dateYear = '';
                        rangeType = "By Date";
                        $('#btnCashierDateRange').text(dateFrom + ' to ' + dateTo);

                        if ($('.lblInventoryFormFowstFowstGroup').text() != "FOWST Location") {
                            getInventoryForm();
                        }
                    }
                }
            })
            //$('#panCashierDate').popup('open');
        })
        .on('click', '#liYesterday', function () {
            dateYear = "This year.";
            newRangeType = "Yesterday";
            $('#selPOSSalesReportRange').val(newRangeType).selectmenu('refresh');
            dateFrom = (newdateFrom.getMonth() + 1) + "/" + (newdateFrom.getDate() - 1) + "/" + newdateFrom.getFullYear();
            dateTo = (newdateTo.getMonth() + 1) + "/" + (newdateTo.getDate() - 1) + "/" + newdateFrom.getFullYear();
            IsViewOnly = 1;
            getInventoryForm();
            $('#CashierDateRangeFrom').text('Yesterday');
            $('#CashierDateRangeSeparator').hide();
            $('#CashierDateRangeTo').text('');
            $('.lblReportDate2').text('Yesterday') //for Cash Handling Module
            $('#panCashierDate').popup('close');
        })
        .on('click', '#liToday', function () {
            dateYear = "This year.";
            newRangeType = "Today";
            $('#selPOSSalesReportRange').val(newRangeType).selectmenu('refresh');
            dateFrom = (newdateFrom.getMonth() + 1) + "/" + (newdateFrom.getDate()) + "/" + newdateFrom.getFullYear();
            dateTo = (newdateTo.getMonth() + 1) + "/" + (newdateTo.getDate()) + "/" + newdateFrom.getFullYear();
            IsViewOnly = 1;
            getInventoryForm();
            $('#CashierDateRangeFrom').text('Today');
            $('#CashierDateRangeSeparator').hide();
            $('#CashierDateRangeTo').text('');
            $('.lblReportDate2').text('Today') //for Cash Handling Module
            $('#panCashierDate').popup('close');
        })
        .on('click', '#liCustomPeriod', function () {
            $('#panCashierDate').popup('close');
            $('#panCashierCustomDate').popup('open');
        })
        .on('click', '#btnCashierCancel', function () {
            $('#panCashierCustomDate').popup('close');
        })
        .on('click', '#btnCashierDone', function (e) {
            dateFrom = new Date($('#txtCashierDateFrom').text());
            dateTo = new Date($('#txtCashierDateTo').text());
            var dateNow = new Date();

                $('#spanCashierDateRangeError').hide();
                $('#btnCashierDateRange').removeClass('btnRequired').addClass('btnNotRequired').attr('title', $('#txtCashierDateFrom').text() + ' to ' + $('#txtCashierDateTo').text())
                $('#CashierDateRangeFrom').text($('#txtCashierDateFrom').text());
                $('#CashierDateRangeTo').text($('#txtCashierDateTo').text());
                $('#CashierDateRangeSeparator').show();
                $('.lblReportDate2').text($('#txtCashierDateFrom').text() + ' to ' + $('#txtCashierDateTo').text()) //for Cash Handling Module
                $('#panCashierCustomDate').popup('close');
                dateFrom = $('#txtCashierDateFrom').text();
                dateTo = $('#txtCashierDateTo').text();
                dateYear = '';
                newRangeType = "Day";
                $('#selPOSSalesReportRange').val(newRangeType).selectmenu('refresh');

                if ($('.lblInventoryFormFowstFowstGroup').text() != "FOWST Location") {
                    IsViewOnly = 1;
                    getInventoryForm();
                }
            //}
        })
        //.on('keyDown', function (e) {
        //    if (e.keyDown == 13) {
        //        alert('hi');
        //    }
        //})

        $(document).ready(function () {
            if (OfflineMode) {
                Browser.OfflineMode = true;
            }

            if (sessionStorage.FowstID == undefined || sessionStorage.FowstID != "" || sessionStorage.FowstID == null) {
                GetAllFOWST();
            }
            else if (CBool(sessionStorage.PrintingOfflineData)) {
                $('#dvSalesReportOfflineMessage').css('visibility', 'visible');
                $('#dvSalesReportOfflineMessage').css('display', 'block');
                IsViewOnly = 1;
                getInventoryForm();
            }
        });

        $('#txtSearchProduct').keydown(function (e) {
            if (e.keyCode == 13) {
                getInventoryForm();
            }
        })

        $('#btnDownloadAsExcel').click(function () {
            IsDownloadExcelFile = 1;
            getInventoryForm();
        });

    });

    function GetAllFOWST() {
        var option = [
                {
                    AutoSearch: true,
                    DisplayText: [{ Column: 'FowstName' }],
                    Click: function (e) {
                        sessionStorage.FowstID = e.ID;
                        sessionStorage.SelectedFOWST = e.ID;
                        $('.lblInventoryFormFowstFowstGroup').text(e.FowstName);
                        $('.lblCashierSelectedFowstGroup2').text(e.FowstName); //for Cash Handling UI
                        $('.btnCashierSelectFowst').removeClass('btnRequired').addClass('btnNotRequired');
                        //$('#btnPOSSalesReportView').click();
                        dateYear = "This year.";
                        newRangeType = (newRangeType == "" ? "Today" : newRangeType);
                        IsViewOnly = 1;
                        getInventoryForm();
                    }
                }
        ]

        if (OfflineMode) {
            option[0].Offline = true;
        }

        BrowsePopup('LibraryFowst', 'Select FOWST', option, { FilterFowst: 1 });
        return
        if (OfflineMode) {
            if (Web.HandleOrganization() == false)
                return;

            var vData = OfflineBrowser.LibraryFowst.GetData({
                FilterFowst: 1
            });

            ShowFOWST(vData);

        } else {

            AjaxSend({
                url: 'Browser/LibraryFowst',
                data: { FilterFowst: 1 },
                success: function (vData) {
                    ShowFOWST(vData);
                }
            });

        }
    }

    function ShowFOWST(vData) {


        pLV = $('#listCashierFowst');
        pLV.empty();

        var vRows;

        if (OfflineMode)
            vRows = vData;
        else
            vRows = vData.Results;

        countFowst = vRows.Rows.length;

        if (vRows.Rows.length == 1) {

            if (vRows.Rows[0].IsFOWSTInsideAMall == '1' && vRows.Rows[0].FOWSTMallName == 'SM Mall') {
                IsFowstInSMMall = true;
            } else {
                IsFowstInSMMall = false;
            }


            sessionStorage.FowstID = vRows.Rows[0].ID;
            sessionStorage.SelectedFOWST = vRows.Rows[0].ID;
            $('.lblInventoryFormFowstFowstGroup').text(vRows.Rows[0].FowstName);
            $('#panCashierFowst').popup('close');
            $('.btnCashierSelectFowst').removeClass('btnRequired').addClass('btnNotRequired');
            dateYear = "This year.";
            newRangeType = (newRangeType == "" ? "Today" : newRangeType);
            $('#selPOSSalesReportRange').val(newRangeType).selectmenu('refresh');
            IsViewOnly = 1;
            getInventoryForm();
        }
        else {
            
            return;
            $('#panCashierFowst').popup('open');

            for (var i = 0; i <= vRows.Rows.length - 1; i++) {
                pItem = $('<li>')
                            .append($('<a>')
                            .attr('ID', vRows.Rows[i].ID)
                            .attr('IsFOWSTInsideAMall', vRows.Rows[i].IsFOWSTInsideAMall)
                             .attr('FOWSTMallName', vRows.Rows[i].FOWSTMallName)
                                .click(function () {

                                    if ($(this).attr('IsFOWSTInsideAMall') == '1' && $(this).attr('FOWSTMallName') == 'SM Mall') {
                                        IsFowstInSMMall = true;
                                    } else {
                                        IsFowstInSMMall = false;
                                    }

                                    
                                })
                                .append($('<h3>')
                                    .text(vRows.Rows[i].FowstName)
                                )
                            )
                pLV.append(pItem);
            }
        }

        try {
            pLV.listview('refresh');
        }
        catch (e) {
            pLV.listview();
        }

    }

    function getInventoryForm() {
        var pSql;
        var vData;

        if (dateFrom == undefined) {
            newdateFrom = new Date();
            dateFrom = (newdateFrom.getMonth() + 1) + "/" + newdateFrom.getDate() + "/" + newdateFrom.getFullYear();
        }

        if (dateTo == undefined) {
            newdateTo = new Date();
            dateTo = (newdateTo.getMonth() + 1) + "/" + newdateTo.getDate() + "/" + newdateFrom.getFullYear();
        }

        if (IsMobile) {
            pSql = 'Select TransDailyBeginningBalance.*, ProductName, LibraryPeople.SurName || \', \' ||  LibraryPeople.FirstName Employee ' + 
                'from TransDailyBeginningBalance ' +
                'INNER JOIN LibraryPIMProduct on LibraryPIMProduct.id = TransDailyBeginningBalance.ProductID ' +
                'INNER JOIN LibraryPeople on LibraryPeople.id = TransDailyBeginningBalance.PeopleID ' +
                'WHERE date(BeginningBalanceDate) between ' +
                ReadyString($.format.date(new Date(dateFrom), 'yyyy-MM-dd')) + ' AND ' +
                ReadyString($.format.date(new Date(dateTo), 'yyyy-MM-dd')) + ' AND ' +
                'FOWSTID = ' + ReadyString(sessionStorage.SelectedFOWST);
                        
            vData = Sql.Execute(pSql);

            displayInventoryForm(vData.Rows)
        } else {           
                        
            var data = {FOWSTID:sessionStorage.SelectedFOWST,
                From: dateFrom,
                To: dateTo,
                ProductName: $('#txtSearchProduct').val(),
                IsDownloadExcelFile: IsDownloadExcelFile
            }

            AjaxSend({
                url: "InventoryForm/getInventoryForm",
                data: data,
                success: function (vData) {
                    
                    if (IsDownloadExcelFile) {
                        window.location = GetPathFormat('InventoryForm/DownloadInventoryReport?p=' + vData.Results);
                    } else {
                        displayInventoryForm(vData.Results.Rows);
                    }
                    IsDownloadExcelFile = 0;
                }
            });
            
            
        }
    
        var tableinventoryformreport = '';
        function displayInventoryForm(vRows) {
            if ($.fn.DataTable.isDataTable($('#tblInvetoryFormReport'))) {
                $('#tblInvetoryFormReport').empty();
                $('#tblInvetoryFormReport').DataTable().clear().destroy();
            }

            tableinventoryformreport = $('#tblInvetoryFormReport').DataTable({
                pageLength: 25,
                "ordering": false,
                "data": vRows,
                "columns": [
                    {
                        "data": function (data) {
                            return CStr2(data.BeginningBalanceDate);
                        }
                    },
                    {
                        "data": function (data) {
                            return CStr2(data.ProductName);
                        }
                    },
                    {
                        "data": function (data) {
                            return CStr2(data.BegBalance);
                        }
                    },
                    {
                        "data": function (data) {
                            return CStr2(data.Sales);
                        }
                    },
                    {
                        "data": function (data) {
                            return CStr2(data.Transfers);
                        }
                    },
                    {
                        "data": function (data) {
                            return CStr2(data.Receiving);
                        }
                    },
                    {
                        "data": function (data) {
                            return CStr2(data.SalesReturns);
                        }
                    },
                    {
                        "data": function (data) {
                            return CStr2(data.InventoryCountVariance);
                        }
                    },
                    {
                        "data": function (data) {
                            return CStr2(data.EndBalance);
                        }
                    },
                    {
                        "data": function (data) {
                            return CStr2(data.Employee);
                        }
                    }
                ],
                "sDom": '<"top">rt<"bottom"ip><"clear">',
                initComplete: function () {
                    $('#tblInvetoryFormReport').removeClass('dataTable');
                }
            });
            return
            var pLV = $('#tbodyDetails');

            pLV.empty();

            for (var i = 0; i <= vRows.length - 1; i++) {
                pItem = $('<tr>')
                            .attr('id', vRows[i].ID)
                            .append($('<td>')
                                .text(vRows[i].BeginningBalanceDate)
                                //.text($.format.date(new Date(vRows[i].BeginningBalanceDate), 'MM/dd/yyyy'))
                            )
                            .append($('<td>')
                                .text(vRows[i].ProductName)
                            )
                            .append($('<td>')
                                .text(CInt2(vRows[i].BegBalance))
                            )
                            .append($('<td>')
                                .text(CInt2(vRows[i].Sales))
                            )
                            .append($('<td>')
                                .text(CInt2(vRows[i].Transfers))
                            )
                            .append($('<td>')
                                .text(CInt2(vRows[i].Receiving))
                            )
                            .append($('<td>')
                                .text(CInt2(vRows[i].SalesReturns))
                            )
                            .append($('<td>')
                                .text(CInt2(vRows[i].InventoryCountVariance))
                            )
                            .append($('<td>')
                                .text(CInt2(vRows[i].EndBalance))
                            )
                            .append($('<td>')
                                .text(vRows[i].Employee)
                            )

                pLV.append(pItem);
            }
        }

    }
});
