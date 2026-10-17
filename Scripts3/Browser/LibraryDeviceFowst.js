define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function() {
    $(function () {

        Browser.Init('LibraryDeviceFowst', {
            // Url: optional; if omitted, follows vKey
            //  this is the name of the action. Controller URL is prepended internally

            OnItem: function (vDataRow, vItem) {
                // PARAMS:
                //  vDataRow: object containing data. 
                //  vItem: $('<li>')

                vItem.append($('<a>')
                    .attr('href', 'LibraryDeviceFowst?id=' + vDataRow.ID)
                    .data('data', vDataRow)
                    .append($('<h3>')
                        .attr("style","white-space:normal")
                        .text(vDataRow.PhoneName)
                    )
                    .append($('<p>')
                        .attr("style", "white-space:normal")
                        .text("FOWST : " + vDataRow.FOWST)
                    )
                    .append($('<p>')
                        .attr("style", "white-space:normal")
                        .text("POS Registration No. : " + vDataRow.POSRegistrationNo)
                    )
                    .append($('<p>')
                        .attr("style", "white-space:normal")
                        .text("Terminal No. : " + vDataRow.TerminalNo)
                    )
                    .click(function (e) {
                        e.preventDefault();
                        if ($.isFunction(Browser.LibraryDeviceFowst.Click))
                            Browser.LibraryDeviceFowst.Click($(this).data('data'));
                        history.back();

                        return false;
                    })
                );
            } // OnItem: function(vDataRow, vItem) {
        }); // Browser.Init('Customer', {

    });
});
