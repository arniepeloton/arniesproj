$(function () {

    var mData;

    $('#cbBranch').append(new Option('a', 'b'));

    $('#lnkWtd').click(function(e) {
        $('#lblPeriod').text('Week to Date');
        DisplayData(mData.wtd.Rows);
    });

    $('#lnkMtd').click(function(e) {
        $('#lblPeriod').text('Month to Date');
        DisplayData(mData.mtd.Rows);
    });

    $('#lnkQtd').click(function(e) {
        $('#lblPeriod').text('Quarter to Date');
        DisplayData(mData.qtd.Rows);
    });

    $('#lnkYtd').click(function(e) {
        $('#lblPeriod').text('Year to Date');
        DisplayData(mData.ytd.Rows);
    });

    AjaxSend({
        url: '/RetailDashboard/GetDataByCategory',
        success: function(vData) {
            var pDummy;
            mData = vData;
            DisplayData(vData.wtd.Rows);
        },
        error: function(vError) {
            var pDummy;
        }
    });

    function DisplayData(vRows) {

        var pTable = $('#dtMain');
        var pRow;

        for (var p = 0; p <= 3; p++) {
            pRow = $(pTable.children('tbody').children()[p + 1]);
            $(pRow.children()[1]).text(AccountingFormat(vRows[p].ThisYear));
            $(pRow.children()[2]).text(AccountingFormat(vRows[p].LastYear));
            $(pRow.children()[3]).text(AccountingFormat(vRows[p].AbsChange));
            $(pRow.children()[4]).text(AccountingFormat(vRows[p].PercentChange));
        };

    };
});