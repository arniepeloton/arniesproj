define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function() {
    require([(OfflineMode ? '/Scripts3/Offline/Browser/LibraryOrganizationEmployeeFOWST.js?crc=' : '')], function() {    

        $(function () {

            Browser.Init('LibraryOrganizationEmployeeFOWST', {
                // Url: optional; if omitted, follows vKey
                //  this is the name of the action. Controller URL is prepended internally

                Title: 'FOWST Representative',
                AutoSearch: true,
                OnItem: function (vDataRow, vItem) {
                    // PARAMS:
                    //  vDataRow: object containing data. 
                    //  vItem: $('<li>')

                    vItem.append($('<a>')
                        .attr('href', 'LibraryOrganizationEmployeeFOWST?id=' + vDataRow.ID)
                        .data('data', vDataRow)
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(vDataRow.PeopleName)
                        )
                        .click(function (e) {
                            e.preventDefault();
                            if ($.isFunction(Browser.LibraryOrganizationEmployeeFOWST.Click))
                                Browser.LibraryOrganizationEmployeeFOWST.Click($(this).data('data'));
                            history.back();

                            return false;
                        })
                    );
                } // OnItem: function(vDataRow, vItem) {
            }); // Browser.Init('Customer', {
        });

    });
});
