define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function () {

    Browser.Init('LibrarySecurityRole', {
        // Url: optional; if omitted, follows vKey
        //  this is the name of the action. Controller URL is prepended internally

        Title: 'Security Role',
        AutoSearch: true,
        OnItem: function (vDataRow, vItem) {
            // PARAMS:
            //  vDataRow: object containing data. 
            //  vItem: $('<li>')

            vItem.append($('<a>')
                    .attr('href', 'LibrarySecurityRole?id=' + vDataRow.ID)
                    .data('data', vDataRow)
                    .append($('<h3>')
                        .attr("style", "white-space:normal")
                        .text(vDataRow.Code + ' - ' + vDataRow.Role)
                    )
                    .append($('<p>')
                        .attr("style", "white-space:normal")
                        .text('Description : ' + vDataRow.Description)
                    )
                    .click(function (e) {
                        e.preventDefault();
                        if ($.isFunction(Browser.LibrarySecurityRole.Click))
                            Browser.LibrarySecurityRole.Click($(this).data('data'));
                        history.back();

                        return false;
                    })
                );
        } // OnItem: function(vDataRow, vItem) {
    }); // Browser.Init('Customer', {

});
