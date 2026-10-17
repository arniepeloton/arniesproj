define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function() {
    $(function () {

        Browser.Init('LibraryPropertyAndPlant', {
            // Url: optional; if omitted, follows vKey
            //  this is the name of the action. Controller URL is prepended internally

            OnItem: function (vDataRow, vItem) {
                // PARAMS:
                //  vDataRow: object containing data. 
                //  vItem: $('<li>')

                vItem.append($('<a>')
                    .attr('href', 'LibraryPropertyAndPlant?id=' + vDataRow.ID)
                    .data('data', vDataRow)
                    .append($('<h3>')
                        .text(vDataRow.Name)
                    )
                    .click(function (e) {
                        e.preventDefault();
                        if ($.isFunction(Browser.LibraryPropertyAndPlant.Click))
                            Browser.LibraryPropertyAndPlant.Click($(this).data('data'));
                        history.back();

                        return false;
                    })
                );
            } // OnItem: function(vDataRow, vItem) {
        }); // Browser.Init('Customer', {

    });
});