// when requiring this file, prepend with "promise!"
define([GetPathFormat('/Scripts3/Browser/common.js?crc=')], function () {
    $(function () {

        Browser.Init('LibraryPeople', {
            // Url: optional; if omitted, follows vKey
            //  this is the name of the action. Controller URL is prepended internally

            OnItem: function (vDataRow, vItem) {
                // PARAMS:
                //  vDataRow: object containing data. 
                //  vItem: $('<li>')

                vItem.append($('<a>')
                    .attr('href', 'LibraryPeople?id=' + vDataRow.ID)
                    .data('data', vDataRow)
                    .append($('<h3>')
                        .attr("style", "white-space:normal")
                        .text(vDataRow.PeopleName)
                    )
                    .append($('<p>')
                        .attr("style", "white-space:normal")
                        .text('Date of Birth : ' + $.format.date(new Date(vDataRow.DateOfBirth), 'MMM dd, yyyy'))
                    )
                    .click(function (e) {
                        var pPage = $.mobile.activePage;
                        e.preventDefault();
                        if ($.isFunction(Browser.LibraryPeople.Click))
                            Browser.LibraryPeople.Click($(this).data('data'));
                        if (pPage == $.mobile.activePage)
                            history.back();

                        return false;
                    })
                );
            } // OnItem: function(vDataRow, vItem) {
//            , PageShow: function () {

//                Browser.LibraryPeople.ShowRightButton().unbind("click")
//                    .click(function (e) {
//                        e.preventDefault();
//                        e.stopPropagation();

//                        window.pagePeopleBrowser = $.mobile.activePage;
//                        ChangePage('/People#pagePeopleName');

//                    });
//            }



        }); // Browser.Init('Customer', {

    });

    if (OfflineMode) {
        var pDeferred = $.Deferred();
        require(['/Scripts3/Offline/Browser/LibraryPeople.js?crc='], function() {    
            pDeferred.resolve({});
        }); // require
        return pDeferred.promise();
    }; // if (OfflineMode) {
});