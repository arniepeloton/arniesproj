$(function () {

    var mData;
    var cbBranch = $('#cbBranch').change(cbBranch_Change);
    var mBranchID;
    var mPeriod = 'w'; // letter
    var mDecimals = 4;

    $('#lnkWtd').click(function (e) {
        $('#lblPeriod').text('Week to Date');
        mPeriod = 'w';
        DisplayData();
    });

    $('#lnkMtd').click(function (e) {
        $('#lblPeriod').text('Month to Date');
        mPeriod = 'm';
        DisplayData();
    });

    $('#lnkQtd').click(function (e) {
        $('#lblPeriod').text('Quarter to Date');
        mPeriod = 'q';
        DisplayData();
    });

    $('#lnkYtd').click(function (e) {
        $('#lblPeriod').text('Year to Date');
        mPeriod = 'y';
        DisplayData();
    });

    $('#chkCustomers').change(function() {
        if ($(this).prop('checked'))
            $('#divCustomer').hide();
        else
            $('#divCustomer').show();
    });

////    $('#formCustomer').submit(function(e) {
////        e.preventDefault(); // prevent form from submitting

////        Browser.LibraryCustomers.Click = function(e) {

////            // user clicked an item
////            $('#txtCustomer').val(e.CompanyName)

////        };
////        Browser.LibraryCustomers.Show($('#txtCustomer').val()); 

////        return false; // prevent form from submitting
////    });

    $('#cbDashboardType').change(function (e) {
        alert($('#cbDashboardType option:selected').text() + ' is not yet implemented');
    });

    $('#divSku').hide();
    $('[name=optReportType][value=pos]').prop('checked', true);
    $('[name=optReportType]').checkboxradio('refresh', true);
    GetData();

    function GetData() {

        AjaxSend({
            url: '/RetailDashboard/GetData',
            data: {BranchID: cbBranch.find('option:selected').val()},
            success: function (vData) {
                mBranchID = vData.BranchID;
                mData = vData;
                DisplayBranches(vData.Branches.Rows);
                DisplayData();
            },
            error: function (vError) {
                var pDummy;
            }
        });

    };

    function DisplayBranches(vRows) {

        var pItem;

        cbBranch.html('');
        for (var p = 0; p <= vRows.length - 1; p++) {
            pItem = new Option(vRows[p].Name, vRows[p].ID);
            cbBranch.append(pItem);
        };
        cbBranch.val(mBranchID).attr('selected');
        cbBranch.selectmenu('refresh', true);

    }; // DisplayBranches

    function DisplayData() {

        var pTable = $('#dtMain');
        var pRows;
        var pRow;

        $('#lblDateRangeThisYear').text(mData[mPeriod + 'tdDateRangeThisYear']);
        $('#lblDateRangeLastYear').text(mData[mPeriod + 'tdDateRangeLastYear']);

        pRows = mData[mPeriod + 'td'].Rows;

        for (var p = 0; p <= 3; p++) {
            pRow = $(pTable.children('tbody').children()[p + 1]);
            $(pRow.children()[1]).text(AccountingFormat(pRows[p].ThisYear, mDecimals));
            $(pRow.children()[2]).text(AccountingFormat(pRows[p].LastYear, mDecimals));
            $(pRow.children()[3]).text(AccountingFormat(pRows[p].AbsChange, mDecimals));
            $(pRow.children()[4]).text(AccountingFormat(pRows[p].PercentChange, mDecimals));
        };

    };

    function cbBranch_Change() {

        GetData();

    }; // cbBranch_Change

    $('#butDownload').click(function () {

        var pUrl;
        var pData = {BranchID: cbBranch.find('option:selected').val(),
            Period: mPeriod,
            Compress: $('#chkCompress').prop('checked')};

        switch ($('[name=optReportType]:checked').val()) {
            case 'pos':
                pUrl = 'GetExcel';
                break;

            case 'department':
            case 'sku':
                pData.Sort = $('#cbSort').val();
                pData.FileFormat = $('[name=optFileFormat]:checked').val();
                pUrl = 'DownloadByCategory';

            case 'department':
                if ($('#chkStocks').prop('checked'))
                    pData.ReportType = 'ByDepartmentAndSku';
                else
                    pData.ReportType = 'ByDeparment';
                break;

            case 'sku':
                pData.ReportType = 'BySku';
                break;

            default:
                alert('invalid mReportType');
                return;

        };

        pUrl = '/RetailDashboard/' + pUrl;

        AjaxSend({
            url: pUrl,
            data: pData,
            success: function (vData) {
                if (vData.Text != null) {
                    window.location = '/download/' + vData.Text;
                } else {
                    MessageBox('Failed');
                };
            },
            error: function (vError) {
                MessageBox(vError);
            }
        });

    });

    $('[name=optReportType]').change(function () {

        switch ($(this).val()) {
            case 'pos':
                $('#divSku').hide();
                break;

            case 'department':
                $('#divSku').show();
                $('#divStocks').show();
                break;

            case 'sku':
                $('#divSku').show();
                $('#divStocks').hide();

        };

    });
});