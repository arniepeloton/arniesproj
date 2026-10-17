require(['common'], function () {
    var mTable = $('#tableFlushingReport');
    var mDataTable;

    $('#butFlushingReportOrganization').click(function() {
        NewMessageBox('This function is not yet implemented. ref: jsBWFVL8MXP9');
    });

    $('#butFlushingReportFilter').click(function() {
        $('#txtFlushingReportMinimumDuration').val(CInt2($('#txtFlushingReportMinimumDuration').val()));
        DisplayTable();
    });

    function DisplayTable() {

        var pColumns = [
            {
                title: 'Organization',
                render: function(v1, v2, vData) {
                    return vData.CompanyName;
                }
            },
            {
                title: 'Device Name',
                render: function(v1, v2, vData) {
                    return vData.DeviceName;
                }
            },
            {
                title: 'Process Name',
                render: function(v1, v2, vData) {
                    return vData.ProcessName;
                }
            },
            {
                title: 'Duration (minutes)',
                render: function(v1, v2, vData) {
                    return AccountingFormat(CInt2(vData.ProcessDuration) / 1000 / 60);
                }
            },
            {
                title: 'Process Start',
                render: function(v1, v2, vData) {
                    return FormatDate(vData.ProcessStart, {
                        Format: 'MMM dd, yyyy hh:mm a'
                    });
                }
            },
            {
                title: 'Process End',
                render: function(v1, v2, vData) {
                    return FormatDate(vData.ProcessEnd, {
                        Format: 'MMM dd, yyyy hh:mm a'
                    });
                }
            }
        ];

        ClearTable();
        AjaxSend({
            url: '/Browser/TransFlushingReport',
            data: {
                MinimumDuration: CInt2($('#txtFlushingReportMinimumDuration').val()) * 1000 * 60
            },
            success: function(vData) {
                var pData = vData.Results.Rows;
                mDataTable = mTable.DataTable({
                    data: pData,
                    columns: pColumns
                });
                mTable.addClass('table-stripe').css('width', '100%')
                mTable.find('thead tr').addClass('custom-theme-f');
                PopupReposition();
            }
        });
    }

    function ClearTable() {
        if ($.fn.DataTable.isDataTable(mTable)) {
            //empty the table first before destroying datatabase para dili magdoble ang header
            mTable.empty();
            mDataTable.destroy();
        }
    }

});