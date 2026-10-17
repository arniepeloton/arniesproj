require([
    'common',
    ], function () {
        $(function () {

            LoadPOSTransactionFOWST();

            $('#selTransactionRangeType').val('optDate');
            $('#selPOSTransactionFOWST').val('');
            setYearCollection('selYear');
            setYearCollection('selFromYear');
            setYearCollection('selToYear');

            $('#selTransactionRangeType').change(function () {
                $('#dvTransactionRangeType').css('visibility', 'visible');
                $('#dvTransactionRangeType').css('display', 'block');

                if ($('#selTransactionRangeType option:selected').val() == 'optMonth') {
                    $('#dvMonth').css('visibility', 'visible');
                    $('#dvMonth').css('display', 'block');
                    $('#dvDay').css('visibility', 'hidden');
                    $('#dvDay').css('display', 'none');
                }
                else if ($('#selTransactionRangeType option:selected').val() == 'optDate') {
                    $('#dvMonth').css('visibility', 'hidden');
                    $('#dvMonth').css('display', 'none');
                    $('#dvDay').css('visibility', 'visible');
                    $('#dvDay').css('display', 'block');
                }
                else {
                    $('#dvTransactionRangeType').css('visibility', 'hidden');
                    $('#dvTransactionRangeType').css('display', 'none');
                    $('#dvMonth').css('visibility', 'hidden');
                    $('#dvMonth').css('display', 'none');
                    $('#dvDay').css('visibility', 'hidden');
                    $('#dvDay').css('display', 'none');
                }
            });

            $('#btnGenerateData').click(function () {
                var data;

                ClearData();

                if ($('#selPOSTransactionFOWST').val() == '') {
                    MessageBox("Please select store",
                                "FOWST",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                    return;
                }

                if ($('#selPOSTransactionFOWSTDevices').val() == '') {
                    MessageBox("Please select machine",
                                "FOWST",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                    return;
                }

                if ($('#selTransactionRangeType option:selected').val() == 'optMonth') {
                    if ($('#selMonth').val() == '') {
                        MessageBox("Please select month to generate",
                                "Month",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                    }
                    else if ($('#selYear').val() == '') {
                        MessageBox("Please select year to generate",
                                "Year",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                    }
                    else {
                        data = { IsMonth: 1,
                            Month: $('#selMonth :selected').index(),
                            Year: $('#selYear').val(),
                            OrgID: sessionStorage.OrgID,
                            FOWSTID: $('#selPOSTransactionFOWST').val()
                        };
                        GetRawData(data);
                    }
                }
                else if ($('#selTransactionRangeType option:selected').val() == 'optDate') {
                    if (($('#txtAuditDateFrom').val() == '' || $('#txtAuditDateTo').val() == '')) {
                        MessageBox("Please provide proper date(s)",
                                "Date",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                    }
                    else {
                        data = { IsMonth: 0,
                            From: $.format.date(new Date($('#txtAuditDateFrom').val()), 'M/d/yyyy h:mm:ss a'),
                            To: $.format.date(new Date($('#txtAuditDateTo').val()), 'M/d/yyyy h:mm:ss a'),
                            OrgID: sessionStorage.OrgID,
                            FOWSTID: $('#selPOSTransactionFOWST').val()
                        };
                        GetRawData(data);
                    }
                }
                else {

                }
            });
        }); //inner function

        function LoadPOSTransactionFOWST() {
            var pData = {};
            pData.OrganizationID = sessionStorage.OrgID;

            AjaxSend({
                url: "Browser/LibraryFowst",
                data: pData,
                success: function (vDataPlaces) {

                    $('#selPOSTransactionFOWST').empty();
                    $('#selPOSTransactionFOWST').append(new Option("Select FOWST", ""));
                    if (vDataPlaces.Results.Rows.length > 0) {
                        for (var i = 0; i <= vDataPlaces.Results.Rows.length - 1; i++) {
                            $('#selPOSTransactionFOWST').append(new Option(vDataPlaces.Results.Rows[i].FowstName, vDataPlaces.Results.Rows[i].ID));
                        }
                    }

                    $('#selPOSTransactionFOWST').unbind().change(function () {
                        var selPOSTransactionFOWSTDevices = $('#selPOSTransactionFOWSTDevices').empty();

                        AjaxSend({
                            url: 'Browser/LibraryDeviceFowst',
                            data: { FOWSTID: $(this).val() },
                            success: function (vDevice) {

                                selPOSTransactionFOWSTDevices.append(new Option('Please select machine', ''));

                                for (var i in vDevice.Results.Rows) {
                                    selPOSTransactionFOWSTDevices.append(new Option(vDevice.Results.Rows[i].PhoneName, vDevice.Results.Rows[i].DeviceID));
                                }
                            }
                        })
                    })
                }
            });

            for (var i = 1; i <= 31; i++) {
                $('.selday').append(new Option(i, i));
            }

            $('#txtAuditDateFrom').val($.format.date(new Date(), 'yyyy-MM-01'));
            $('#txtAuditDateTo').val($.format.date(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0), 'yyyy-MM-dd'));
        }

        function GetRawData(data) {
            Bs.Loading('show');
            //var pUrl = MergeUrlParameters(data, '/DashboardsAndReports/GenerateNewAuditTrailData');
            //window.open(pUrl, '_blank');
            //return
            data = $.extend(data, {
                DeviceID: $('#selPOSTransactionFOWSTDevices').val()
            })
            AjaxSend({
                url: 'DashboardsAndReports/GenerateNewAuditTrailData',
                data: data,
                success: function (vData) {
                    Bs.Loading('hide');
                    if (vData.POSTransactionDetail == "") {
                        //                        DisplayRawData(vData.POSTransactionDetail.Rows, data);
                        MessageBox("No record(s) found.",
                                "POS Transaction Detail",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                    } else {
                        window.location = GetPathFormat("/DashboardsAndReports/DownloadExcelReportFile?FileName=" + $.trim(vData.POSTransactionDetail));
                    }

                }
            });
        }

        var StartingInvoiceNo = 0;
        var EndingInvoiceNo = 0;
        var InvoiceCount = 0;
        var IsInvoiceCountskip = 'No';
        var CurrentInvoiceNo = 0;

        var StartingMemoNo = 0;
        var EndingMemoNo = 0;
        var MemoCount = 0;
        var IsMemoCountSkip = 'No';
        var CurrentMemoNo = 0;

        function DisplayRawData(vRows, data) {
            var m_Month = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
            var m_Count = 0;

            if (vRows[0].Transaction_Type == 'Sales Returns') {
                StartingMemoNo = vRows[0].Transaction_ID;
                CurrentMemoNo = vRows[0].Transaction_ID;
            }
            else {
                StartingInvoiceNo = vRows[0].Transaction_ID;
                CurrentInvoiceNo = vRows[0].Transaction_ID;
            }

            for (var i = 0; i <= vRows.length - 1; i++) {

                if (vRows[i].Transaction_Type == 'Sale Returns') {
                    if (i == vRows.length - 1) {
                        EndingMemoNo = vRows[i].Transaction_ID;
                    }
                    MemoCount++;

                    if (CurrentMemoNo - vRows[i].Transaction_ID == -1 || CurrentMemoNo - vRows[i].Transaction_ID == 0) {
                        CurrentMemoNo = vRows[i].Transaction_ID;
                    }
                    else
                        IsMemoCountSkip = 'Yes';
                }
                else if (vRows[i].Transaction_Type == 'Invoice') {
                    if (InvoiceCount == 0 && i != 0) {
                        EndingMemoNo = vRows[i - 1].Transaction_ID;
                        StartingInvoiceNo = vRows[i].Transaction_ID;
                        CurrentInvoiceNo = vRows[i].Transaction_ID;
                    }

                    if (i == vRows.length - 1) {
                        EndingInvoiceNo = vRows[i].Transaction_ID;
                    }

                    if (CurrentInvoiceNo - vRows[i].Transaction_ID == -1 || CurrentInvoiceNo - vRows[i].Transaction_ID == 0) {
                        CurrentInvoiceNo = vRows[i].Transaction_ID;
                    }
                    else
                        IsInvoiceCountskip = 'Yes';

                    InvoiceCount++;
                }

                tData = $('<tr>')
                    .append($('<th>')
                        .text(vRows[i].Account_ID)
                    )
                    .append($('<td>')
                        .text(vRows[i].Store_ID)
                    )
                    .append($('<td>')
                        .text(vRows[i].Transaction_MIN)
                    )
                    .append($('<td>')
                        .text(vRows[i].Transaction_ID)
                    )
                    .append($('<td>')
                        .text(vRows[i].Transaction_Date)
                    )
                    .append($('<td>')
                        .text(vRows[i].UPC_Code)
                    )
                    .append($('<td>')
                        .text(vRows[i].Sold_Qty)
                    )
                    .append($('<td>')
                        .text(AccountingFormat(vRows[i].Sold_Amt))
                    )
                    .append($('<td>')
                        .text(AccountingFormat(vRows[i].Sold_Amt_Discount))
                    )
                    .append($('<td>')
                        .text(vRows[i].Transaction_Type)
                    )
                    .append($('<td>')
                        .text(m_Month[parseInt(vRows[i].Transaction_Month) - 1])
                    )
                    .append($('<td>')
                        .text(vRows[i].Transaction_Year)
                    )
                    .append($('<td>')
                        .text(vRows[i].Transaction_ReferenceSI)
                    )

                $('#tblPOSRawDataBody').append(tData);
            }


            $('#tblPOSRawDataTable');//.table('refresh');

            GetSummary(data);
        }

        function GetSummary(data) {
            AjaxSend({
                url: 'DashboardsAndReports/GetSummary',
                data: data,
                success: function (vData) {
                    DisplaySummary(vData.POSTransactionDetail.Rows);
                }
            });
        }

        function DisplaySummary(vRows, data) {
            var m_Month = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

            for (var i = 0; i <= vRows.length - 1; i++) {
                var m_Start = 0;
                var m_End = 0;
                var m_IsSkipped = 'Yes';

                if (vRows[i].Transaction_Type == 'Invoice') {
                    m_Start = StartingInvoiceNo;
                    m_End = EndingInvoiceNo;
                    m_IsSkipped = IsInvoiceCountskip;
                }
                else {
                    m_Start = StartingMemoNo;
                    m_End = EndingMemoNo;
                    m_IsSkipped = IsMemoCountSkip;
                }

                tData = $('<tr>')
                    .append($('<th>')
                        .text(vRows[i].Account_ID)
                    )
                    .append($('<td>')
                        .text(vRows[i].Store_ID)
                    )
                    .append($('<td>')
                        .text(vRows[i].Transaction_MIN)
                    )
                    .append($('<td>')
                        .text(AccountingFormat(vRows[i].Total_Sales))
                    )
                    .append($('<td>')
                        .text(AccountingFormat(vRows[i].Total_Discounts))
                    )
                    .append($('<td>')
                        .text(vRows[i].Transaction_Type)
                    )
                    .append($('<td>')
                        .text(m_Month[parseInt(vRows[i].Transaction_Month) - 1])
                    )
                    .append($('<td>')
                        .text(vRows[i].Transaction_Year)
                    )
                    .append($('<td>')
                        .text(m_Start)
                    )
                    .append($('<td>')
                        .text(m_End)
                    )
                    .append($('<td>')
                        .text(m_IsSkipped)
                    )

                $('#tblPOSSummaryBody').append(tData);
            }

            $('#tblPOSSummary');//.table('refresh');
        }

        function ClearData() {
            $('#tblPOSRawDataBody').html('');
            $('#tblPOSRawDataTable');//.table('refresh');
            $('#tblPOSSummaryBody').html('');
            $('#tblPOSSummary');//.table('refresh');

            StartingInvoiceNo = 0;
            EndingInvoiceNo = 0;
            InvoiceCount = 0;
            IsInvoiceCountskip = 'No';
            CurrentInvoiceNo = 0;

            StartingMemoNo = 0;
            EndingMemoNo = 0;
            MemoCount = 0;
            IsMemoCountSkip = 'No';
            CurrentMemoNo = 0;
        }
    });

//require([
//    ], function () {
//        $(function () {

//        }); //inner function

//        function PageShow() {
//        }
//    });