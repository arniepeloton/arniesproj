define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function() {
    require([(OfflineMode ? '/Scripts3/Offline/Browser/LibraryOrganizationBanks.js?crc=' : '')], function () {    
        $(function () {

            Browser.Init('LibraryOrganizationBanks', {
                // Url: optional; if omitted, follows vKey
                //  this is the name of the action. Controller URL is prepended internally
                OnItem: function (vDataRow, vItem) {
                    // PARAMS:
                    //  vDataRow: object containing data. 
                    //  vItem: $('<li>')

                    vItem.append($('<a>')
                        .attr('href', 'LibraryOrganizationBanks?id=' + vDataRow.ID)
                        .data('data', vDataRow)
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(vDataRow.CompanyName)
                        )
                        .click(function (e) {
                            e.preventDefault();
                            if ($.isFunction(Browser.LibraryOrganizationBanks.Click))
                                Browser.LibraryOrganizationBanks.Click($(this).data('data'));
                            history.back();

                            return false;
                        })
                    );
                } // OnItem: function(vDataRow, vItem) {

//                , PageShow: function () {

//                    Browser.LibraryOrganizationBanks.ShowManualEntry().unbind("click")
//                        .click(function (e) {
//                            e.preventDefault();
//                            e.stopPropagation();

//                            var pData = { IsManualEntry: true };
//                            Browser.LibraryOrganizationBanks.Click(pData);

//                            history.back();

//                            return false;
//                        });

//                }
            }); // Browser.Init('Customer', {

        }); // $(function () {
    }); // require
}); // define
