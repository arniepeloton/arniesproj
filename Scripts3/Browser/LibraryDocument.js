define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function() {
    $(function () {
        
        Browser.Init('LibraryDocument', {
            // Url: optional; if omitted, follows vKey
            //  this is the name of the action. Controller URL is prepended internally

            OnItem: function (vDataRow, vItem) {
                // PARAMS:
                //  vDataRow: object containing data. 
                //  vItem: $('<li>')
                vItem.append($('<a>')
                    .attr('href', 'LibraryDocument?id=' + vDataRow.ID)
                    .data('data', vDataRow)
                    .append($('<h3>')
                        .attr("style", "white-space:normal")
                        .text(vDataRow.ReleaseDoc)
                    )
                    .click(function (e) {
                        var pPage = $.mobile.activePage;
                        e.preventDefault();
                        if ($.isFunction(Browser.LibraryDocument.Click))
                            Browser.LibraryDocument.Click($(this).data('data'));
                        if (pPage == $.mobile.activePage)
                            history.back();

                        return false;
                    })
                );
            } // OnItem: function(vDataRow, vItem) {
        }); // Browser.Init('Customer', {
    });
});