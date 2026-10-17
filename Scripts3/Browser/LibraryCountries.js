define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function () {
    require([(OfflineMode ? '/Scripts3/Offline/Browser/LibraryCountries.js?crc=' : '')], function () {    
        $(function () {

            Browser.Init('LibraryCountries', {
                // Url: optional; if omitted, follows vKey
                //  this is the name of the action. Controller URL is prepended internally

                OnItem: function (vDataRow, vItem) {
                    // PARAMS:
                    //  vDataRow: object containing data. 
                    //  vItem: $('<li>')

                    vItem.append($('<a>')
                        .attr('href', 'LibraryCountries?id=' + vDataRow.ID)
                        .data('data', vDataRow)
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(vDataRow.Name)
                        )
                        .click(function (e) {
                            e.preventDefault();
                            if ($.isFunction(Browser.LibraryCountries.Click))
                                Browser.LibraryCountries.Click($(this).data('data'));
                            history.back();

                            return false;
                        })
                    );
                } // OnItem: function(vDataRow, vItem) {
            }); // Browser.Init('Customer', {

        });
    }); // require
});
