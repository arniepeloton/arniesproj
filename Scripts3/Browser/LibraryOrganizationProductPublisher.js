define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function() {
    $(function () {

        Browser.Init('LibraryOrganizationProductPublisher', {
            // Url: optional; if omitted, follows vKey
            //  this is the name of the action. Controller URL is prepended internally

            OnItem: function (vDataRow, vItem) {
                // PARAMS:
                //  vDataRow: object containing data. 
                //  vItem: $('<li>')

                vItem.append($('<a>')
                    .attr('href', 'LibraryDevice?id=' + vDataRow.ID)
                    .data('data', vDataRow)
                    .append($('<h3>')
                        .attr("style","white-space:normal")
                        .text(vDataRow.CompanyName)
                    )
                    .append($('<p>')
                        .attr("style", "white-space:normal")
                        .text("No. of Products : " + vDataRow.NoOfProducts)
                    )
                    .append($('<p>')
                        .attr("style", "white-space:normal")
                        .text("Last Updated on " + $.format.date(new Date(vDataRow.LastUpdated)))
                    )
                    .click(function (e) {
                        e.preventDefault();
                        if ($.isFunction(Browser.LibraryDevice.Click))
                            Browser.LibraryOrganizationProductPublisher.Click($(this).data('data'));
                        history.back();

                        return false;
                    })
                );
            } // OnItem: function(vDataRow, vItem) {
        }); // Browser.Init('Customer', {

    });
});
