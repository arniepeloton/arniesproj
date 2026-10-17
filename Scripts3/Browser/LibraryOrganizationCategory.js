define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function () {
    $(function () {

        Browser.Init('LibraryOrganizationCategory', {
            // Url: optional; if omitted, follows vKey
            //  this is the name of the action. Controller URL is prepended internally
            OnItem: function (vDataRow, vItem) {
                // PARAMS:
                //  vDataRow: object containing data. 
                //  vItem: $('<li>')

                vItem.append($('<a>')
                    .attr('href', 'LibraryOrganizationCategory?id=' + vDataRow.ID)
                    .data('data', vDataRow)
                    .append($('<h3>')
                        .attr("style", "white-space:normal")
                        .text(vDataRow.CategoryType)
                    )
                    .append($('<p>')
                        .text(vDataRow.Category)
                    )
                    .click(function (e) {
                        e.preventDefault();

                        if ($.isFunction(Browser.LibraryOrganizationCategory.Click))
                            Browser.LibraryOrganizationCategory.Click($(this).data('data'));

                        history.back();

                        return false;
                    })
                );

            } // OnItem: function(vDataRow, vItem) {
        }); // Browser.Init('LibraryCustomers', {

    });
});