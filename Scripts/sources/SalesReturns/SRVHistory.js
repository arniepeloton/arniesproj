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
    var dateFrom = $.format.date(new Date(), 'MMM dd, yyyy');
    var dateTo = $.format.date(new Date(), 'MMM dd, yyyy');
    var SeniorCitizenAndPWDDiscountTable = $('#tblSeniorCitizenAndPWDDiscount');
    var SeniorCitizenAndPWDDiscountDatatable;
    var SRVHistoryTable = $('#tblSRVHistory')
    var SRVHistoryDatatable;    

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
        
        //$('body').on('click', '.lblCashierSelectedFowstGroup', function () {
        //    if (countFowst > 1) {
        //        $('#panCashierFowst').popup('open');
        //    }
        //}).on('click', '#btnCashierDateRange', function () {
        //    $('#panCashierDate').popup('open');
        //}).on('click', '#liYesterday', function () {
        //    dateYear = "This year.";
        //    newRangeType = "Yesterday";
        //    $('#selPOSSalesReportRange').val(newRangeType).selectmenu('refresh');
        //    dateFrom = (newdateFrom.getMonth() + 1) + "/" + (newdateFrom.getDate() - 1) + "/" + newdateFrom.getFullYear();
        //    dateTo = (newdateTo.getMonth() + 1) + "/" + (newdateTo.getDate() - 1) + "/" + newdateFrom.getFullYear();
        //    IsViewOnly = 1;
        //    getSCAndPWDDiscount();
        //    $('#CashierDateRangeFrom').text('Yesterday');
        //    $('#CashierDateRangeSeparator').hide();
        //    $('#CashierDateRangeTo').text('');
        //    $('.lblReportDate2').text('Yesterday') //for Cash Handling Module
        //    $('#panCashierDate').popup('close');
        //})
        //        .on('click', '#liToday', function () {
        //            dateYear = "This year.";
        //            newRangeType = "Today";
        //            $('#selPOSSalesReportRange').val(newRangeType).selectmenu('refresh');
        //            dateFrom = (newdateFrom.getMonth() + 1) + "/" + (newdateFrom.getDate()) + "/" + newdateFrom.getFullYear();
        //            dateTo = (newdateTo.getMonth() + 1) + "/" + (newdateTo.getDate()) + "/" + newdateFrom.getFullYear();
        //            IsViewOnly = 1;
        //            getSCAndPWDDiscount();
        //            $('#CashierDateRangeFrom').text('Today');
        //            $('#CashierDateRangeSeparator').hide();
        //            $('#CashierDateRangeTo').text('');
        //            $('.lblReportDate2').text('Today') //for Cash Handling Module
        //            $('#panCashierDate').popup('close');
        //        })
        //        .on('click', '#liCustomPeriod', function () {
        //            $('#panCashierDate').popup('close');
        //            $('#panCashierCustomDate').popup('open');
        //        })
        //        .on('click', '#btnCashierCancel', function () {
        //            $('#panCashierCustomDate').popup('close');
        //        })
        //        .on('click', '#btnCashierDone', function (e) {
        //            dateFrom = new Date($('#txtCashierDateFrom').text());
        //            dateTo = new Date($('#txtCashierDateTo').text());
        //            var dateNow = new Date();

        //                $('#spanCashierDateRangeError').hide();
        //                $('#btnCashierDateRange').removeClass('btnRequired').addClass('btnNotRequired').attr('title', $('#txtCashierDateFrom').text() + ' to ' + $('#txtCashierDateTo').text())
        //                $('#CashierDateRangeFrom').text($('#txtCashierDateFrom').text());
        //                $('#CashierDateRangeTo').text($('#txtCashierDateTo').text());
        //                $('#CashierDateRangeSeparator').show();
        //                $('.lblReportDate2').text($('#txtCashierDateFrom').text() + ' to ' + $('#txtCashierDateTo').text()) //for Cash Handling Module
        //                $('#panCashierCustomDate').popup('close');
        //                dateFrom = $('#txtCashierDateFrom').text();
        //                dateTo = $('#txtCashierDateTo').text();
        //                dateYear = '';
        //                newRangeType = "Day";
        //                $('#selPOSSalesReportRange').val(newRangeType).selectmenu('refresh');

        //                if ($('#lblCashierSelectedFowstGroup').text() != "FOWST Location") {
        //                    IsViewOnly = 1;
        //                    getSCAndPWDDiscount();
        //                }
        //        })
         

        $(document).ready(function () {
            if (OfflineMode) {
                Browser.OfflineMode = true;
            }

            if (CStr2(sessionStorage.SelectedFOWST) != '') {
                $('#btnSRVFowst').text(sessionStorage.SelectedFOWSTName);
            }

            if (CStr2(sessionStorage.SelectedGroup) != '') {
                $('#btnSRVGroup').text(sessionStorage.SelectedGroupName);
            }

            GetSRVHistory();
            //if (sessionStorage.FowstID == undefined || sessionStorage.FowstID != "" || sessionStorage.FowstID == null) {
            //    GetAllFOWST();
            //}
            //else if (CBool(sessionStorage.PrintingOfflineData)) {
            //    $('#dvSalesReportOfflineMessage').css('visibility', 'visible');
            //    $('#dvSalesReportOfflineMessage').css('display', 'block');
            //    IsViewOnly = 1;
            //    getSCAndPWDDiscount();
            //}

            $('#btnSRVDatePeriod').click(function () {
                PopupPeriod({
                    Click: function (e) {
                        if (e.Period == 'Custom' || e.Period == 'Custom Date') {
                            $('#btnSRVDatePeriod').text(e.From + ' to ' + e.To);
                        } else {
                            $('#btnSRVDatePeriod').text(e.Period);
                        }
                        dateFrom = e.From;
                        dateTo = e.To;
                        GetSRVHistory();
                    }
                })
            })

            $('#btnSRVFowst').click(function () {
                BrowsePopup('LibraryFowst', 'Select FOWST', [
                    {
                        AutoSearch: true,
                        DisplayText: [{ Column: 'FowstName' }],
                        Click: function (e) {
                            sessionStorage.SelectedFOWST = e.ID;
                            sessionStorage.SelectedFOWSTName = e.FowstName;
                            sessionStorage.SelectedGroup = '';
                            $('#btnSRVFowst').text(e.FowstName);
                            $('#btnSRVGroup').text('Select Group');
                            GetSRVHistory();
                        }
                    }
                ], { IsAll: true, IsByEmployeeFOWST: true })
            })

            $('#btnSRVGroup').click(function () {
                BrowsePopup('LibraryFranchiseGroup', 'Select Group', [
                        {
                            AutoSearch: true,
                            DisplayText: [{ Column: 'GroupName' }],
                            Click: function (e) {
                                sessionStorage.SelectedGroup = e.ID;
                                sessionStorage.SelectedGroupName = e.GroupName;
                                sessionStorage.SelectedFOWST = '';
                                sessionStorage.SelectedFOWSTName = '';
                                $('#btnSRVGroup').text(e.GroupName);
                                $('#btnSRVFowst').text('Select FOWST');
                                GetSRVHistory();
                            }
                        }
                    ], { FilterByUserGroup: true }
                )
            })

            $('#selSRVFilterBy').change(function () {
                GetSRVHistory();
            })

            $('#txtSearchSrvHistory').unbind('keydown').keydown(function (e) {
                if (e.which == 13) {
                    GetSRVHistory();
                }                
            })
        });

        //$('#txtSearchProduct').keydown(function (e) {
        //    if (e.keyCode == 13) {
        //        getSCAndPWDDiscount();
        //    }
        //})

        //$('#btnDownloadAsExcel').click(function () {
        //    IsDownloadExcelFile = 1;
        //    getSCAndPWDDiscount();
        //});

    });

    function GetAllFOWST() {


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
            $('.lblCashierSelectedFowstGroup').text(vRows.Rows[0].FowstName);
            $('#panCashierFowst').popup('close');
            $('.btnCashierSelectFowst').removeClass('btnRequired').addClass('btnNotRequired');
            dateYear = "This year.";
            newRangeType = (newRangeType == "" ? "Today" : newRangeType);
            $('#selPOSSalesReportRange').val(newRangeType).selectmenu('refresh');
            IsViewOnly = 1;
            getSCAndPWDDiscount();
        }
        else {
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

                                    sessionStorage.FowstID = $(this).attr('id');
                                    sessionStorage.SelectedFOWST = $(this).attr('id');
                                    $('.lblCashierSelectedFowstGroup').text($(this).text());
                                    $('.lblCashierSelectedFowstGroup2').text($(this).text()); //for Cash Handling UI
                                    $('#panCashierFowst').popup('close');
                                    $('.btnCashierSelectFowst').removeClass('btnRequired').addClass('btnNotRequired');
                                    //$('#btnPOSSalesReportView').click();
                                    dateYear = "This year.";
                                    newRangeType = (newRangeType == "" ? "Today" : newRangeType);
                                    $('#selPOSSalesReportRange').val(newRangeType).selectmenu('refresh');
                                    IsViewOnly = 1;
                                    getSCAndPWDDiscount();
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

    function GetSRVHistory() {

        if (CStr2(sessionStorage.SelectedFOWST) == '' && CStr2(sessionStorage.SelectedGroup) == '') {
            return;
        }
      
        AjaxSend({
            url: "SalesReturns/GetSRVHistory",
            data: {
                FowstID: sessionStorage.SelectedFOWST,
                GroupID: sessionStorage.SelectedGroup,
                DateFrom: dateFrom,
                DateTo: dateTo,
                FilterBy: $('#selSRVFilterBy').val(),
                Search: $.trim($('#txtSearchSrvHistory').val())
            },
            success: function (vData) {
                
                DisplaySRVHistory(vData.Results.Rows);
            }
        });
            
            
        }
    
    function DisplaySRVHistory(vRows) {

        if ($.fn.DataTable.isDataTable(SRVHistoryTable)) {
            //empty the table first before destroying datatabase para dili magdoble ang header
            SRVHistoryTable.empty();
            SRVHistoryDatatable.destroy();
        }

        SRVHistoryDatatable = SRVHistoryTable.DataTable({
            "data": vRows,
            "columns": [               
                {
                    "data": function (data) {
                        return $.format.date(new Date(data.SRDate), 'MM-dd-yy hh:mm a');
                    }
                },
                { "data": "FowstName" },
                { "data": "CompanyName" },
                { "data": "IndustryClass" },
                { "data": "TransactedBy" },
                {
                    "data": function (data) {
                        return CStr2(data.ApprovedBy);
                    }, className: 'text-center'
                },
                { "data": "ItemDescription" },
                {
                    "data": function (data) {
                        var p = data.SRType;

                        if (p == 'Merchandise Exchange') {
                            p += '<br><b>SRV #:</b> ' + data.SrvNum +
                                    '<br><b>PIN:</b> ' + data.Pin
                        }

                        return p;
                    }, className: 'text-center'
                },
                {
                    "data": function (data) {
                        return CDec2(data.ReturnedQtyForThisTrans);
                    }, className: 'text-center'
                },
                {
                    "data": function (data) {
                        return AccountingFormat(data.TotalValueOfReturnedQty);
                    }, className: 'text-center'
                },
                {
                    "data": function (data) {
                        return CDec2(data.TotalReturnedQtyForThisInvoice);
                    }, className: 'text-center'
                },
                {
                    "data": function (data) {
                        return CDec2(data.OrigQtyInInvoice);
                    }, className: 'text-center'
                },
                {
                    "data": function (data) {
                        return CDec2(data.NetQtyInOrigInvoice);
                    }, className: 'text-center'
                },
                {
                    "data": function (data) {
                        return data.TransactionNumber;
                    }, className: 'text-center'
                },
                {
                    "data": function (data) {
                        return data.InvoiceNumber;
                    }, className: 'text-center'
                },
                {
                    "data": function (data) {
                        return $.format.date(new Date(data.InvoiceDate), 'MM-dd-yy hh:mm a');
                    }
                },
            ],           
            "sDom": '<"top">rt<"bottom"ip><"clear">',
            "aaSorting": [],
            //buttons: [
            //    {
            //        extend: 'csvHtml5',
            //        text: 'Download CSV',
            //        title: 'Sales Return and Refund History'
            //    }
            //],
            //"columnDefs": [{ "targets": 0, "orderable": true }],
            //"aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
            initComplete: function () {
                $('#tblSRVHistory').removeClass('dataTable');
            }
        });
    }
});
