require([
    'common',
    'tablesort',
    GetPathFormat('/Scripts3/model/InventoryCountReport_Offline.js?crc='),
    (OfflineMode ? '/Scripts3/Offline/Browser/LibraryFowst.js?crc=' : '')

], function () {

    var InventoryCountReportOffline = {};

    InventoryCountReportOffline = new App.model.InventoryCountReport_Offline();

    $(function () {
        GetILAPHistory();

        $('#btnRefreshILAPHistory').click(function () {
            GetILAPHistory();
        });

        $('#btnTabILADHistory').unbind('click').click(function () {
            if ($('#tblILADHistoryBody tr').length == 0) {
                GetILAPHistory();
            }
        })

    })

    function GetILAPHistory() {

        var tblILADHistoryBody = $('#tblILADHistoryBody2');

        tblILADHistoryBody.empty().append('<tr><td colspan="11" style="text-align:center">Please wait while loading ...</td></tr>');
        var mData;
        var data;

        data = { FowstID: GetDefaultFOWST() };  

        mData = InventoryCountReportOffline.GetInventoryCountForApprovalHistory(data);

        var vRows = mData.Rows;

        tblILADHistoryBody.empty()

        if (vRows.length > 0) {
            for (var i = 0; i < vRows.length; i++) {
                tblILADHistoryBody
                .append($('<tr>')
                    .append($('<td>')
                        .text((i + 1) + '.')
                    )
                    .append($('<td>')
                        .text(vRows[i].FOWST)
                    )
                    .append($('<td>')
                        .text(vRows[i].Bin)
                    )
                    .append($('<td>')
                        .text($.format.date(new Date(vRows[i].CountDate), 'MMM dd, yyyy hh:mm a'))
                    )
                    .append($('<td>')
                        .text(vRows[i].ProductName)
                    )
                    .append($('<td>')
                        .text(vRows[i].Quantity)
                    )
                    .append($('<td>')
                        .text(vRows[i].UnitMeasure)
                    )
                    .append($('<td>')
                        .text(vRows[i].Variance)
                    )
                    .append($('<td>')
                        .css('color', (vRows[i].Status == 'Approved' ? '#168E03' : '#F00'))
                        .text(vRows[i].Status)
                    )
                    .append($('<td>')
                        .text(vRows[i].ModifiedBy)
                    )
                    .append($('<td>')
                        .text($.format.date(new Date(vRows[i].ModifiedDate), 'MMM dd, yyyy hh:mm a'))
                    )
                )
            }
        }
        else {
            tblILADHistoryBody.append('<tr><td colspan="11" style="text-align:center">No Result(s) Found</td></tr>')
        }

        //tblILADHistoryBody.parent().table('refresh');

        //AjaxSend({
        //    url: 'InventoryCount/GetInventoryCountForApprovalHistory',
        //    data: { FowstID: fowstid, IsAllItem: true },
        //    success: function (vData) {
        //        var vRows = vData.Results.Rows;
        //        tblILADHistoryBody.empty()

        //        if (vRows.length > 0) {
        //            for (var i = 0; i < vRows.length; i++) {
        //                tblILADHistoryBody
        //                .append($('<tr>')
        //                    .append($('<td>')
        //                        .text((i + 1) + '.')
        //                    )
        //                    .append($('<td>')
        //                        .text(vRows[i].FOWST)
        //                    )
        //                    .append($('<td>')
        //                        .text(vRows[i].Bin)
        //                    )
        //                    .append($('<td>')
        //                        .text($.format.date(new Date(vRows[i].CountDate), 'MMM dd, yyyy hh:mm a'))
        //                    )
        //                    .append($('<td>')
        //                        .text(vRows[i].ProductName)
        //                    )
        //                    .append($('<td>')
        //                        .text(vRows[i].Quantity)
        //                    )
        //                    .append($('<td>')
        //                        .text(vRows[i].UnitMeasure)
        //                    )
        //                    .append($('<td>')
        //                        .text(vRows[i].Variance)
        //                    )
        //                    .append($('<td>')
        //                        .css('color', (vRows[i].Status == 'Approved' ? '#168E03' : '#F00'))
        //                        .text(vRows[i].Status)
        //                    )
        //                    .append($('<td>')
        //                        .text(vRows[i].ModifiedBy)
        //                    )
        //                    .append($('<td>')
        //                        .text($.format.date(new Date(vRows[i].ModifiedDate), 'MMM dd, yyyy hh:mm a'))
        //                    )
        //                )
        //            }
        //        }
        //        else {
        //            tblILADHistoryBody.append('<tr><td colspan="11" style="text-align:center">No Result(s) Found</td></tr>')
        //        }

        //        tblILADHistoryBody.parent().table('refresh');
        //    }
        //});
    }

    function GetDefaultFOWST() {
        var vData = Business.LibraryOrganizationFOWST.GetData()

        if (vData.Rows.length > 0) {
            return vData.Rows[0].FowstID
        }

        //$('#selFOWST').empty();
        vData 

        //for (var i = 0; i <= vData.Rows.length - 1; i++) {
        //    if (vData.Rows[i].ID == undefined) {//server side getting fowst
        //        $('#selFOWST').append(new Option(vData.Rows[i].FowstName, vData.Rows[i].FowstID, false));
        //        $('#selFOWST').val(vData.Rows[i].FowstID);
        //    } else {
        //        $('#selFOWST').append(new Option(vData.Rows[i].FowstName, vData.Rows[i].ID, false));
        //        $('#selFOWST').val(vData.Rows[i].ID);
        //    }
        //}

        //$('#selFOWST').append(new Option('Select Client\'s FOWST', 'Others', false));

        //if (IsGuid($('#selFOWST').val()))
        //    sessionStorage.FowstID = $('#selFOWST').val();

        //try {
        //    $('#selFOWST').selectmenu('refresh');
        //}
        //catch (e) {
        //    $('#selFOWST').selectmenu();
        //}

    }
});