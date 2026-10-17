define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function() {
    $(function () {

        Browser.Init('LibraryDevicePrinter', {
            // Url: optional; if omitted, follows vKey
            //  this is the name of the action. Controller URL is prepended internally

            OnItem: function (vDataRow, vItem) {
                // PARAMS:
                //  vDataRow: object containing data. 
                //  vItem: $('<li>')

                vItem.append($('<a>')
                    .attr('href', 'LibraryDevicePrinter?id=' + vDataRow.ID)
                    .data('data', vDataRow)
                    .append($('<h3>')
                        .attr("style","white-space:normal")
                        .text(vDataRow.PrinterName)
                    )
                    .append($('<p>')
                        .attr("style", "white-space:normal")
                        .text("Printer Type : " + vDataRow.PrinterType)
                    )
                    .append($('<p>')
                        .attr("style", "white-space:normal")
                        .text("FOWST Assignment : " + vDataRow.Fowst)
                    )
                    .append($('<p>')
                        .attr("style", "white-space:normal")
                        .text("Fixed IP Address : " + vDataRow.IPAddress)
                    )
                    .append($('<p>')
                        .attr("style", "white-space:normal")
                        .text('Active : ' + (vDataRow.IsActive == 1 ? "Yes" : "No"))
                    )
                    .click(function (e) {
                        e.preventDefault();
                        if ($.isFunction(Browser.LibraryDevicePrinter.Click))
                            Browser.LibraryDevicePrinter.Click($(this).data('data'));
                        history.back();

                        return false;
                    })
                );
            } // OnItem: function(vDataRow, vItem) {
        }); // Browser.Init('Customer', {

    });
});
