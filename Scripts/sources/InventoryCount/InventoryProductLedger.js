require([
    'common', 'tablesort'
], function () {
    $(function () {
        GetProductLedger();
    });

    function GetProductLedger(ProductID) {
        data = {
            ProductID: GetUrlParameters().ProductID,
            Within: GetUrlParameters().Range,
            Fowst: GetUrlParameters().FOWSTID,
            BinID: GetUrlParameters().BINID
        }
        AjaxSend({
            url: 'InventoryCount/GetInventoryCountReportByProduct',
            data: data,
            success: function (vData) {
                DisplayInventoryCountReportByProduct(vData.InventoryCountReport.Rows)
            }
        });
    }

    function DisplayInventoryCountReportByProduct(vRows) {
        pLV = $('#tblProductLedgerBody')

        pLV.html('');

        for (var i = 0; i <= vRows.length - 1; i++) {
            pItem = $('<tr>')
                    .append($('<th>')
                        .append($('<span>')
                            .text(vRows[i].ProductNo + '-' + vRows[i].ProductName)
                        )
                    )
                    .append($('<td>')
                        .append($('<span>')
                            .text(vRows[i].BinName)
                        )
                    )
                    .append($('<td>')
                        .append($('<span>')
                            .text(vRows[i].Quantity)
                        )
                    )
                    .append($('<td>')
                        .append($('<span>')
                            .text(vRows[i].Balance)
                        )
                    )
                    .append($('<td>')
                        .append($('<span>')
                            .text(vRows[i].LastUpdated)
                        )
                    )
                    .append($('<td>')
                        .append($('<span>')
                            .text(vRows[i].Source)
                        )
                    )
            pLV.append(pItem)
        }

        $('#tblProductLedger').table("refresh");
    }
});