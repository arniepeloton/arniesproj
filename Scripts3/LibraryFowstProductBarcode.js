define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function () {
    $(function () {

        Browser.Init('LibraryFowstProductBarcode', {
            // Url: optional; if omitted, follows vKey
            //  this is the name of the action. Controller URL is prepended internally

            OnItem: function (vDataRow, vItem) {
                // PARAMS:
                //  vDataRow: object containing data. 
                //  vItem: $('<li>')
                vItem.append($('<a>')
                    .attr('href', 'LibraryFowstProductBarcode?id=' + vDataRow.ID)
                    .data('data', vDataRow)
                    .append($('<h3>')
                        .attr("style", "white-space:normal")
                        .text("Transaction #"+vDataRow.TransactionNum)
                    )
                    .append($('<p>')
                        .attr("style", "white-space:normal")
                        .text("Product Name: " + vDataRow.SkuDesc)
                    )
                    .append($('<p>')
                        .attr("style", "white-space:normal")
                        .text("Measure: " + vDataRow.UoM)
                    )
                    .append($('<p>')
                        .attr("style", "white-space:normal")
                        .text("Barcode: " + vDataRow.Barcode)
                    )
                    .append($('<p>')
                        .attr("style", "white-space:normal")
                        .text("Num of Labels: " + vDataRow.NumLabels)
                    )
                    .append($('<p>')
                        .attr("style", "white-space:normal")
                        .text("Transaction Time: "+vDataRow.LastUpdated)
                    )
                    .click(function (e) {
                        var pPage = $.mobile.activePage;
                        e.preventDefault();
                        if ($.isFunction(Browser.LibraryFowstProductBarcode.Click))
                            Browser.LibraryFowstProductBarcode.Click($(this).data('data'));
                        if (pPage == $.mobile.activePage)
                            history.back();

                        return false;
                    })
                );
            } // OnItem: function(vDataRow, vItem) {
        }); // Browser.Init('Customer', {
    });
});